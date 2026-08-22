# ExifTweaker 2.0 --- Plan d'implémentation complet

## Vision

ExifTweaker 2.0 est une application Windows native C# / .NET 10 /
WinForms destinée à corriger et enrichir les métadonnées des photos et
vidéos avant leur import dans une photothèque telle qu'Immich.

``` text
Carte SD / Téléphone / Dossier Windows
                 │
                 ▼
        ┌──────────────────┐
        │   ExifTweaker    │
        ├──────────────────┤
        │ Import session   │
        │ Analyse metadata │
        │ Date / heure     │
        │ GPS / carte      │
        │ GPX              │
        │ Suggestions      │
        │ Preview changes  │
        │ Apply + Backup   │
        └────────┬─────────┘
                 ▼
               Immich
```

### Principes directeurs

-   C# / .NET 10 / WinForms.
-   Application locale, sans backend ni Docker.
-   ExifTool comme moteur unique de métadonnées.
-   Modification par lot au centre du workflow.
-   Aucun fichier modifié avant une action explicite `Apply`.
-   Prévisualisation systématique des changements.
-   Backup et restauration.
-   Architecture indépendante d'Immich.
-   Traitements asynchrones et annulables.
-   Suggestions automatiques jamais appliquées sans validation.

## Architecture cible

``` text
ExifTweaker/
├── Program.cs
├── Forms/
│   ├── MainForm.cs
│   ├── DateShiftForm.cs
│   ├── ApplyChangesForm.cs
│   └── SettingsForm.cs
├── Controls/
│   ├── PhotoGridControl.cs
│   ├── PhotoPreviewControl.cs
│   ├── MetadataEditorControl.cs
│   ├── LocationEditorControl.cs
│   ├── MapControl.cs
│   └── PendingChangesControl.cs
├── Models/
│   ├── PhotoItem.cs
│   ├── PhotoMetadata.cs
│   ├── MetadataPatch.cs
│   ├── MetadataSnapshot.cs
│   ├── GpsCoordinate.cs
│   ├── ImportSession.cs
│   └── OperationResult.cs
├── Services/
│   ├── ExifToolService.cs
│   ├── MetadataService.cs
│   ├── FileDiscoveryService.cs
│   ├── ThumbnailService.cs
│   ├── GeocodingService.cs
│   ├── GpxService.cs
│   ├── SuggestionService.cs
│   ├── AnomalyDetectionService.cs
│   └── HistoryService.cs
├── Infrastructure/
│   ├── ProcessRunner.cs
│   ├── AppSettings.cs
│   └── exiftool/
└── Data/
    └── exiftweaker.db
```

Le découpage peut rester initialement dans un unique `.csproj`.

------------------------------------------------------------------------

## Phase 0 --- Sécurisation et nettoyage

**Objectif :** assainir le projet avant les évolutions fonctionnelles.

### À implémenter

-   Supprimer toute clé API hardcodée et révoquer les clés déjà
    publiées.
-   Introduire `AppSettings`.
-   Stocker localement fournisseur de géocodage, clé API, chemin
    ExifTool, stratégie de backup et parallélisme.
-   Supprimer code mort, `BackgroundWorker`, threads manuels et attentes
    actives.
-   Réserver `async void` aux event handlers.
-   Centraliser logs et erreurs.

### Validation

-   Aucun secret dans Git.
-   Pas de régression majeure.
-   Exceptions async observables.
-   Code de traitement indépendant de l'UI.

**État v2-alpha1 : réalisé.**

------------------------------------------------------------------------

## Phase 1 --- Nouveau modèle métier

**Objectif :** séparer fichier, métadonnées, modifications et UI.

### `PhotoItem`

``` text
PhotoItem
├── FilePath
├── FileName
├── Original : PhotoMetadata
├── PendingChanges : MetadataPatch
├── EffectiveMetadata
├── Status
└── HasPendingChanges
```

### `PhotoMetadata`

Prévoir : date de capture, offset/timezone, latitude, longitude,
altitude, appareil, objectif, orientation, dimensions, type MIME, type
fichier et dates filesystem.

Extensions futures : rating, titre, description, keywords, copyright et
auteur.

### `MetadataPatch`

Représente uniquement ce que l'utilisateur veut changer :

``` text
Original : Date 10:32 / GPS -
Patch    : Shift +02:00 / GPS Paris
Effective: Date 12:32 / GPS Paris
```

### Validation

-   L'UI ne manipule plus directement les tags EXIF.
-   Une modification existe sans écriture disque.
-   `EffectiveMetadata` est calculable à tout moment.

**État v2-alpha1 : réalisé.**

------------------------------------------------------------------------

## Phase 2 --- ExifTool comme moteur unique

**Objectif :** centraliser lecture et écriture des métadonnées.

### `ExifToolService`

Responsabilités : lecture, lecture batch, écriture, suppression de tags,
restauration et éventuellement extraction des previews.

-   Utiliser la sortie JSON.
-   Préférer les valeurs numériques pour GPS.
-   Éviter le parsing de texte humain.
-   Lire les fichiers en batch.
-   Étudier `-stay_open` pour garder ExifTool vivant durant la session.
-   Masquer complètement la CLI à l'UI.

### Pipeline

``` text
UI → MetadataService → ExifToolService → exiftool.exe
```

### Validation

-   Lecture batch fiable.
-   Chemins Unicode et espaces gérés.
-   Aucune commande ExifTool dans `Form1`.
-   Plus de dépendance métier à ExifLibrary.

**État v2-alpha1 : socle réalisé.**

------------------------------------------------------------------------

## Phase 3 --- Moteur asynchrone

**Objectif :** rendre les traitements fluides et annulables.

Remplacer `BackgroundWorker`, `Thread`, `Thread.Sleep`, attente active
et `Task.Run` imbriqués par :

-   `async/await`;
-   `CancellationToken`;
-   `IProgress<T>`;
-   `SemaphoreSlim`;
-   `Task.WhenAll`.

Opérations : `LoadFilesAsync`, `ReadMetadataAsync`,
`GenerateThumbnailsAsync`, `ApplyChangesAsync`, `ImportGpxAsync`.

### UX

``` text
Reading metadata
367 / 503
██████████████░░░░ 73 %
[Cancel]
```

### Validation

-   UI jamais bloquée.
-   Opérations longues annulables.
-   Progression exploitable.
-   Pas d'attente active.

**État v2-alpha1 : socle réalisé.**

------------------------------------------------------------------------

## Phase 4 --- Import et découverte

**Objectif :** construire une ouverture de session robuste.

### Entrées

-   Open files.
-   Open folder.
-   Drag & drop fichiers.
-   Drag & drop dossiers.
-   Parcours récursif configurable.

### `FileDiscoveryService`

Découverte, validation, déduplication, identification des formats et
remontée des erreurs.

### Formats cibles

JPG/JPEG, HEIC/HEIF, TIFF, PNG, DNG, CR2/CR3, NEF, ARW, RAF, ORF, RW2
puis MOV/MP4.

### Validation

-   Gros dossiers importables.
-   Pas de doublons.
-   Fichiers invalides isolés.
-   Import annulable.

**État v2-alpha1 : socle réalisé.**

------------------------------------------------------------------------

## Phase 5 --- Import Session

**Objectif :** introduire une unité de travail.

`ImportSession` contient médias, date d'ouverture, plage temporelle,
appareils, statistiques GPS/date, modifications et erreurs.

``` text
Session — 21 août 2026
347 fichiers
Sony A7 IV 212
iPhone     102
Pixel       33
18 → 21 août
294 avec GPS
53 sans GPS
0 modifications en attente
```

### Validation

-   Toute l'UI travaille sur une session.
-   Statistiques mises à jour dynamiquement.
-   Avertissement à la fermeture si modifications en attente.

------------------------------------------------------------------------

## Phase 6 --- Nouvelle grille photo

**Objectif :** faire de la grille le centre de contrôle.

Colonnes : sélection, thumbnail, filename, date, timezone, location,
appareil, dimensions, statut.

Statuts : `Unchanged`, `Modified`, `Metadata issue`, `Error`.

Sélection : Ctrl+A, Ctrl+clic, Shift+clic, plages. `Delete` retire de la
session mais ne supprime jamais le fichier.

Filtres : All, Modified, No GPS, No date, Errors.

### Validation

-   1 000+ lignes utilisables.
-   Multi-sélection fiable.
-   Mise à jour ciblée sans reconstruction complète de la grille.

------------------------------------------------------------------------

## Phase 7 --- Thumbnails et preview

**Objectif :** afficher rapidement les médias.

### `ThumbnailService`

-   embedded preview lorsque disponible ;
-   génération ;
-   cache RAM ;
-   cache disque ;
-   lazy loading ;
-   orientation EXIF ;
-   placeholder ;
-   invalidation.

La grande preview n'est chargée que pour le média actif.

### Validation

-   Pas d'explosion mémoire.
-   Scroll fluide.
-   Pas de verrou persistant sur les fichiers.
-   RAW affichés via preview intégrée si possible.

------------------------------------------------------------------------

## Phase 8 --- Éditeur de date complet

**Objectif :** correction temporelle batch.

### Set

Date, heure et timezone explicites.

### Shift

Années, mois, jours, heures, minutes, secondes. Les écarts relatifs
entre médias sont conservés.

### Timezone

Distinguer : 1. correction du timezone sans changer l'heure locale ; 2.
conversion du même instant vers un autre timezone.

### Preview

``` text
IMG01 10:30 → 12:47
IMG02 10:31 → 12:48
IMG03 10:34 → 12:51
```

### Synchronisation des tags

`MetadataService` traduit la notion métier de « date de capture » vers
les tags nécessaires : `DateTimeOriginal`, `CreateDate`, `ModifyDate`,
`OffsetTimeOriginal`, puis tags QuickTime pour les vidéos.

### Validation

-   Set / Shift / timezone en batch.
-   Secondes prises en charge.
-   Aucun fichier écrit avant Apply.

------------------------------------------------------------------------

## Phase 9 --- Pending Changes

**Objectif :** rendre l'édition non destructive.

``` text
Original + MetadataPatch → EffectiveMetadata
```

La grille affiche les valeurs effectives sans modifier le disque.

Actions : Undo edit, Reset selected, Reset all, Apply changes.

En multi-sélection, afficher `<multiple values>` lorsqu'une propriété
diffère.

### Validation

-   Aucun changement disque durant l'édition.
-   Reset instantané.
-   Modifications visuellement identifiables.

------------------------------------------------------------------------

## Phase 10 --- Écriture ExifTool et backup

**Objectif :** écriture fiable et récupérable.

``` text
Pending changes
→ Validation
→ Backup
→ ExifTool write
→ Read-back verification
→ Success / warning / error
```

-   Une erreur fichier ne stoppe pas le batch.
-   Conserver un original ou backup configurable.
-   Relire les tags critiques après écriture.
-   Ne supprimer le patch qu'après succès.

### Validation

-   Erreurs isolées.
-   Original récupérable.
-   Écriture vérifiée.
-   Rapport par fichier.

------------------------------------------------------------------------

## Phase 11 --- Undo / Reset

**Objectif :** deux niveaux d'annulation.

### Avant Apply

Ctrl+Z, pile d'actions, Reset selection, Reset all.

### Après Apply

Restauration depuis le backup.

### Validation

-   Une mauvaise opération batch est récupérable.
-   Distinction claire entre annulation d'un patch et restauration
    physique.

------------------------------------------------------------------------

## Phase 12 --- Éditeur GPS

**Objectif :** modèle GPS simple.

`GpsCoordinate` stocke latitude/longitude en degrés décimaux et altitude
optionnelle.

Actions : définir, appliquer à la sélection, supprimer, copier/coller
GPS.

Validation : latitude -90/+90, longitude -180/+180.

ExifTool gère la représentation EXIF DMS si nécessaire.

------------------------------------------------------------------------

## Phase 13 --- Carte WebView2

**Objectif :** édition visuelle moderne.

Stack : WebView2 + Leaflet + OpenStreetMap/fournisseur configurable.

``` text
C# → setMarker() → Leaflet
Leaflet → userClicked(lat,lon) → C#
```

Fonctions : point actif, clic pour déplacer, plusieurs points, zoom
sélection, distinction des médias sans GPS.

La carte crée seulement un `MetadataPatch`.

------------------------------------------------------------------------

## Phase 14 --- Géocodage

**Objectif :** découpler la recherche de lieu du fournisseur.

``` text
IGeocodingService
├── SearchAsync
└── ReverseAsync
```

Fournisseurs possibles : Maps.co, Nominatim, Mapbox, Google.

Afficher plusieurs résultats plutôt que choisir automatiquement le
premier. Ajouter reverse-geocoding et cache approprié.

------------------------------------------------------------------------

## Phase 15 --- Preview et rapport Apply

**Objectif :** dernière validation claire avant écriture.

``` text
Apply metadata changes
153 photos
Date: 137 changed
Location: 53 changed / 2 removed
JPEG 102 / HEIC 34 / RAW 17
Backup originals: Yes
[Cancel] [Apply]
```

Pendant l'écriture : progression, succès, warnings et erreurs.

Après : rapport détaillé et indication des fichiers restaurables.

------------------------------------------------------------------------

## Phase 16 --- Import GPX

**Objectif :** géolocaliser depuis une trace.

`GpxService` lit timestamp, latitude, longitude et altitude.

Matching sur la date de capture avec paramètres de tolérance, timezone,
offset appareil et interpolation.

``` text
IMG01 14:32:10 delta 2s  ✓
IMG02 14:33:42 delta 1s  ✓
IMG03 14:37:03 delta 38s ⚠
```

Les résultats deviennent des `MetadataPatch`, jamais des écritures
directes.

------------------------------------------------------------------------

## Phase 17 --- Correction d'horloge appareil

**Objectif :** traiter les appareils mal réglés.

Exemple : Photo 14:32 / GPX 12:32 → suggestion d'offset +02:00.

Permettre de : - définir l'offset ; - recalculer le matching GPX ; -
appliquer réellement un shift aux dates ; - ou utiliser l'offset
uniquement pour le matching.

Ces deux opérations restent distinctes.

------------------------------------------------------------------------

## Phase 18 --- Historique SQLite

**Objectif :** persistance des opérations.

Tables conceptuelles `Operation` et `OperationFile` avec timestamp,
type, fichier, before, after, backup et résultat.

Fonctions : historique Apply, détails, restauration, audit,
éventuellement reprise après crash.

Le fichier reste la source de vérité, pas SQLite.

------------------------------------------------------------------------

## Phase 19 --- Suggestions GPS sans GPX

**Objectif :** combler les trous à partir du contexte.

Sources : photos temporellement voisines, stabilité du cluster GPS,
distance temporelle, appareil, session.

Chaque localisation conserve sa provenance : `Existing`, `Manual`,
`GPX`, `Suggested`.

Jamais d'Apply automatique.

------------------------------------------------------------------------

## Phase 20 --- Timeline

**Objectif :** comprendre et sélectionner temporellement une session.

``` text
09:00 ──●●●────●──── Paris
12:00 ──────●●●●●── ?
15:00 ─────────●●●─ Versailles
```

Interactions : zoom temporel, sélection de plage, filtre appareil, trous
GPS, anomalies et synchronisation avec grille/carte.

------------------------------------------------------------------------

## Phase 21 --- Carte globale de session

**Objectif :** analyse spatiale complète.

Afficher points, clusters, sélection, GPS manuels, GPX, suggestions et
anomalies.

Clic cluster → sélection des médias correspondants.

Filtres : Existing GPS, GPX, Suggested, Manual, Missing.

Carte, grille et timeline partagent la même sélection.

------------------------------------------------------------------------

## Phase 22 --- Détection d'anomalies

**Objectif :** identifier les métadonnées suspectes sans IA lourde.

Règles : - date manquante ; - GPS manquant ; - date hors session ; -
saut GPS impossible ; - rupture d'horloge ; - timestamp dupliqué ; -
timezone suspect ; - GPS isolé ; - dates filesystem incohérentes.

Chaque anomalie fournit type, sévérité, explication, suggestion et
médias concernés.

------------------------------------------------------------------------

## Phase 23 --- Vidéos QuickTime / MP4

**Objectif :** traiter photos et vidéos ensemble.

Formats initiaux : MOV, MP4.

Gérer conventions UTC/locales, multiples dates QuickTime, GPS QuickTime,
thumbnail vidéo et durée.

`MetadataService` masque ces différences au reste de l'application.

------------------------------------------------------------------------

## Phase 24 --- Connecteur Immich

**Objectif :** intégrer Immich sans en faire une dépendance.

Workflow principal : ExifTweaker → fichiers corrigés → Immich.

Fonctions optionnelles : - identifier des assets ; - trouver ceux sans
GPS ; - récupérer du contexte ; - déclencher un refresh metadata ; -
ouvrir/localiser un asset.

Toute l'API reste derrière `ImmichService`.

------------------------------------------------------------------------

## Phase 25 --- Intégration Immich-AI

**Objectif :** exploiter le contexte événementiel.

Exemple :

``` text
Event: Annecy — 18–21 août
98 photos
71 autour d’Annecy
27 sans GPS
→ Suggest Annecy for 27 photos
```

Données possibles : événements, clusters temporels/géographiques,
personnes, voyages, confiance d'événement.

Immich-AI fournit le contexte ; ExifTweaker valide et écrit.

------------------------------------------------------------------------

# Fonctionnalités transversales

## Logging

Logging structuré pour ExifTool, temps de traitement, erreurs, Apply,
géocodage et GPX.

## Tests

Priorité : calcul de date, timezone, `MetadataPatch`, GPS, parsing JSON
ExifTool, génération de commandes, GPX matching et anomalies. Maintenir
un corpus de médias de test.

## Erreurs

Catégories : fichier inaccessible, format non supporté, metadata
invalide, ExifTool absent, écriture refusée, géocodage indisponible,
backup impossible.

## Paramètres

ExifTool, cache, backup, géocodage, carte, parallélisme, formats et
comportement d'import.

------------------------------------------------------------------------

# Découpage en versions

## v2.0-alpha1 --- Fondations

Phases 0--4 : sécurité, modèles, ExifTool, async, découverte/import.

## v2.0-alpha2 --- Workflow batch

Phases 5--11 : Import Session, grille, thumbnails, date, Pending
Changes, écriture sécurisée, Undo.

## v2.0-alpha3 --- Géolocalisation

Phases 12--15 : GPS editor, WebView2/Leaflet, géocodage,
Apply/reporting.

## v2.0-beta1 --- Géolocalisation automatique

Phases 16--18 : GPX, correction d'horloge, historique SQLite.

## v2.0-beta2 --- Analyse de session

Phases 19--23 : suggestions GPS, timeline, carte globale, anomalies,
vidéos.

## Post-2.0

Phases 24--25 : Immich et Immich-AI.

------------------------------------------------------------------------

# Définition du MVP ExifTweaker 2.0

Le MVP comprend :

-   fichiers, dossiers et drag & drop ;
-   principaux formats photo ;
-   grille, thumbnails, preview et multi-sélection ;
-   filtres ;
-   date Set / Shift / timezone ;
-   GPS coordonnées / suppression / recherche / carte / batch ;
-   Pending Changes ;
-   preview ;
-   Apply explicite ;
-   backup ;
-   read-back verification ;
-   Undo avant Apply ;
-   restauration après Apply ;
-   progression et annulation ;
-   erreurs par fichier ;
-   raccourcis clavier ;
-   aucune opération longue bloquant l'UI.

------------------------------------------------------------------------

# Ordre global

    Phase Sujet                   Priorité
  ------- ---------------------- ----------
        0 Sécurité / nettoyage    Critique
        1 Modèle métier           Critique
        2 ExifTool                Critique
        3 Async / cancellation    Critique
        4 Import / découverte     Critique
        5 Import Session           Haute
        6 Nouvelle grille          Haute
        7 Thumbnails / preview     Haute
        8 Éditeur Date             Haute
        9 Pending Changes         Critique
       10 Écriture / backup       Critique
       11 Undo / Reset            Critique
       12 GPS editor               Haute
       13 Carte WebView2           Haute
       14 Géocodage                Haute
       15 Apply / rapport          Haute
       16 GPX                     Moyenne
       17 Camera clock offset     Moyenne
       18 Historique SQLite       Moyenne
       19 Suggestions GPS         Moyenne
       20 Timeline                Moyenne
       21 Carte session           Moyenne
       22 Anomalies               Moyenne
       23 Vidéos                  Moyenne
       24 Immich                   Future
       25 Immich-AI                Future

------------------------------------------------------------------------

# Règles d'architecture

1.  L'UI ne connaît jamais les commandes ExifTool.
2.  Le domaine utilise des valeurs métier, pas les représentations EXIF
    brutes.
3.  Toute édition crée d'abord un `MetadataPatch`.
4.  Seul `Apply` modifie physiquement un média.
5.  Une écriture doit pouvoir être restaurée.
6.  Une erreur média ne stoppe pas un batch complet.
7.  Les opérations longues sont asynchrones et annulables.
8.  Immich reste optionnel.
9.  La provenance d'une suggestion est conservée.
10. Aucune suggestion n'est écrite sans validation.
11. Les fichiers restent la source de vérité.
12. Les services sont testables indépendamment de WinForms.

------------------------------------------------------------------------

# Prochaine étape

La base `v2-alpha1` couvre les phases **0 à 4**.

Le prochain chantier cohérent est **v2-alpha2 --- phases 5 à 11** :

``` text
ImportSession
      ↓
Nouvelle grille
      ↓
Thumbnails / preview
      ↓
Éditeur Date
      ↓
Pending Changes
      ↓
Preview
      ↓
Apply + backup
      ↓
Undo / Reset
```

Ce chantier transforme les nouvelles fondations en véritable workflow
d'édition batch non destructif.
