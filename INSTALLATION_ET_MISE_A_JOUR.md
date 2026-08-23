# ExifTweaker — Installation et mises à jour

## Installation

Les releases produisent désormais un installeur Velopack `Setup.exe` en plus de l’archive ZIP historique.

Pour bénéficier des mises à jour intégrées, installer ExifTweaker avec le `Setup.exe`. Une exécution depuis Visual Studio ou directement depuis l’archive ZIP reste possible, mais elle n’est pas considérée comme une installation Velopack et ne peut pas s’auto-mettre à jour.

L’application est publiée en `win-x64` self-contained : le runtime .NET est embarqué. L’installeur vérifie également WebView2, nécessaire à la carte.

## Mise à jour

Au démarrage, ExifTweaker peut interroger les Releases GitHub. Si une version plus récente est disponible :

1. l’utilisateur voit la version installée et la nouvelle version ;
2. les notes de version sont affichées ;
3. le téléchargement ne démarre qu’après confirmation ;
4. Velopack vérifie le package téléchargé ;
5. l’utilisateur confirme le redémarrage ;
6. l’updater remplace l’application puis relance ExifTweaker.

Les paramètres restent dans `%LocalAppData%\\ExifTweaker` et ne sont donc pas remplacés par une mise à jour.

## Paramètres

`Paramètres` contient maintenant :

- recherche automatique des mises à jour au démarrage ;
- inclusion facultative des GitHub prereleases ;
- version installée ;
- bouton `Rechercher maintenant…`.

## GitHub Actions

Le workflow `Create release` conserve le bump automatique de version et les tests existants. Il produit désormais :

- le ZIP historique et son SHA-256 ;
- le Setup Velopack ;
- le package complet Velopack ;
- les deltas lorsqu’une release précédente compatible est disponible ;
- le feed `releases.win.json` utilisé par le client de mise à jour.

Le workflow utilise Velopack 1.2.0 côté SDK et CLI afin d’éviter un décalage de versions.
