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

## État d'avancement au 22 août 2026

Ce tableau est l'état de référence du dépôt actuel. Il remplace les
anciennes mentions d'avancement ponctuelles présentes plus bas dans le
document.

Méthode d'estimation : le pourcentage mesure à la fois l'implémentation,
les tests automatisés et la qualification fonctionnelle. Les 26 phases
ont le même poids dans le calcul global. `Terminé` signifie que le code
prévu est présent ; une validation externe résiduelle peut encore être
indiquée. `Partiel` signifie qu'un workflow utilisable existe mais que
des critères importants manquent. `Embryonnaire` désigne uniquement des
briques réutilisables, sans workflow complet.

| Phase | Sujet | État | Avancement | Ce qui manque pour terminer |
|---:|---|---|---:|---|
| 0 | Sécurisation et nettoyage | Terminé | 95 % | Confirmer hors dépôt la révocation de toute ancienne clé publiée et effectuer une dernière revue sécurité avant diffusion. |
| 1 | Nouveau modèle métier | Terminé | 100 % | Rien pour le périmètre défini. |
| 2 | ExifTool comme moteur unique | Terminé | 95 % | La distribution Windows est packagée et couverte par des tests dédiés ; qualifier encore un corpus multi-format réel et évaluer `-stay_open` seulement si les mesures le justifient. |
| 3 | Moteur asynchrone | Terminé | 90 % | Tester systématiquement annulation et progression sur les gros traitements et mesurer l'absence de blocage UI. |
| 4 | Import et découverte | Terminé | 90 % | Tester de très gros dossiers, les arborescences partiellement inaccessibles et chaque format cible avec de vrais médias. |
| 5 | Import Session | Terminé | 90 % | Conserver aussi les erreurs de découverte dans la session et compléter les tests de mise à jour dynamique. |
| 6 | Nouvelle grille photo | Partiel | 85 % | Valider 1 000+ lignes, le scroll et toutes les combinaisons Ctrl/Shift ; envisager la virtualisation si les mesures l'exigent. |
| 7 | Thumbnails et preview | Partiel | 85 % | Qualifier RAW/HEIC/vidéos réels, mesurer mémoire et fluidité, et vérifier l'invalidation du cache sur un corpus important. |
| 8 | Éditeur de date complet | Terminé | 95 % | Ajouter des tests d'intégration ExifTool pour les offsets et les dates QuickTime. |
| 9 | Pending Changes | Terminé | 90 % | Généraliser l'indication `<multiple values>` à tous les éditeurs et ajouter davantage de tests de Reset multi-sélection. |
| 10 | Écriture ExifTool et backup | Partiel | 90 % | Le pipeline TIFF temporaire est couvert par un test Windows ; qualifier encore les erreurs partielles et chaque format média réel. |
| 11 | Undo / Reset | Terminé | 90 % | Ajouter des tests d'intégration de restauration physique et de scénarios Apply partiellement réussi. |
| 12 | Éditeur GPS | Terminé | 95 % | Qualifier l'écriture DMS/altitude par ExifTool sur plusieurs formats. |
| 13 | Carte WebView2 | Partiel | 85 % | Tester le runtime WebView2 sous Windows, les erreurs de chargement réseau et le comportement sur de grandes sélections. |
| 14 | Géocodage | Partiel | 85 % | Tester réellement Maps.co/Nominatim, gérer explicitement quotas/rate limiting et décider si le cache doit devenir persistant. |
| 15 | Preview et rapport Apply | Terminé | 95 % | Valider l'ergonomie du rapport sur de gros batchs et des mélanges succès/warnings/erreurs/annulations. |
| 16 | Import GPX | À faire | 0 % | Créer `GpxService`, parser les traces, gérer tolérance/timezone/interpolation, prévisualiser les correspondances et produire des patches. |
| 17 | Correction d'horloge appareil | Embryonnaire | 10 % | Les primitives Shift/timezone existent ; il manque le modèle d'offset appareil, l'estimation via GPX, le recalcul et l'UI dédiée. |
| 18 | Historique SQLite | Embryonnaire | 10 % | L'historique en mémoire existe ; il manque SQLite, les schémas, la persistance Apply, l'audit et la restauration depuis l'historique. |
| 19 | Suggestions GPS sans GPX | À faire | 0 % | Implémenter provenance, analyse temporelle/spatiale, score de confiance, preview et validation manuelle. |
| 20 | Timeline | À faire | 0 % | Créer le contrôle, le zoom, la sélection temporelle, les filtres et la synchronisation avec grille/carte. |
| 21 | Carte globale de session | Embryonnaire | 30 % | La carte affiche déjà plusieurs points ; il manque clustering, provenance, filtres, sélection par cluster et synchronisation avec timeline. |
| 22 | Détection d'anomalies | Embryonnaire | 10 % | Les statuts/filtres détectent quelques absences simples ; il manque le service de règles, sévérités, explications et suggestions. |
| 23 | Vidéos QuickTime / MP4 | Partiel | 60 % | Import, dates et GPS QuickTime sont présents ; il manque durée, thumbnail vidéo fiable, corpus de tests et qualification UTC/local multi-appareils. |
| 24 | Connecteur Immich | À faire | 0 % | Créer `ImmichService`, configuration/authentification, recherche d'assets, refresh metadata et ouverture/localisation. |
| 25 | Intégration Immich-AI | À faire | 0 % | Définir le contrat, importer le contexte événementiel, calculer les suggestions et créer le workflow de validation. |

### Synthèse

| Périmètre | Avancement estimé | Lecture |
|---|---:|---|
| MVP, phases 0 à 15 | **91 %** | Fonctionnel dans le code ; qualification multi-format réelle et performance encore incomplètes. |
| Beta, phases 16 à 23 | **15 %** | Quelques fondations réutilisables, mais GPX, historique persistant et analyse de session restent à construire. |
| Post-2.0, phases 24 à 25 | **0 %** | Aucun connecteur Immich ou Immich-AI présent. |
| Plan complet, phases 0 à 25 | **61 %** | Moyenne arithmétique non pondérée des 26 phases. |

### Validation actuellement acquise

-   compilation et publication Release .NET 10 confirmées avec 0 erreur ;
-   distribution Windows ExifTool complète : 502 fichiers copiés dans
    les sorties application, tests et publication ;
-   nouvelle suite de 14 tests : 12 réussis localement et 2 tests
    d'intégration ExifTool réservés au runner Windows ;
-   dernier workflow GitHub de référence réussi avec 8 tests sur 8 ; le
    workflow renforcé doit maintenant valider les 14 tests, la commande
    `exiftool -ver`, Apply, read-back, backup, restauration et chemin Unicode ;
-   aucun secret en clair, `BackgroundWorker`, thread manuel ou
    dépendance métier à ExifLibrary détecté.

### Prochaine priorité recommandée

1.  Exécuter le workflow GitHub renforcé et conserver son run comme
    validation Windows de l'ExifTool embarqué et des 14 tests.
2.  Qualifier les phases 0 à 15 sous Windows sur un corpus JPG, HEIC,
    RAW, MOV et MP4 et avec au moins 1 000 médias.
3.  Corriger les défauts révélés par cette qualification pour figer le
    MVP.
4.  Démarrer la phase 16 (`GpxService`) puis la phase 17, qui en dépend.
5.  Implémenter ensuite l'historique SQLite de la phase 18 avant les
    suggestions et outils d'analyse des phases 19 à 23.

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

Le dépôt couvre désormais le workflow MVP des phases **0 à 15**. Le
prochain jalon n'est plus `v2-alpha2`, mais une passe de qualification
complète du MVP sous Windows et sur médias réels.

Une fois cette qualification stabilisée, le chantier fonctionnel
suivant est **v2.0-beta1 --- phases 16 à 18** :

``` text
Import GPX
      ↓
Correction d'horloge appareil
      ↓
Historique SQLite
```

Les phases 19 à 23 doivent rester postérieures à ces fondations, et les
connecteurs Immich des phases 24 à 25 restent optionnels et post-2.0.
