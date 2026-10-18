# EcoActions

Application Android et iOS native en .NET MAUI, réalisée à partir des maquettes Figma validées.

## Fonctionnalités

Création de compte, connexion et récupération par code ; catalogue et recherche ; fiches de gestes avec quantité ; journal avec annulation et rétablissement ; défi de sept gestes ; bilan par semaine ou mois ; profil et rappels quotidiens. Les statistiques proviennent des actions enregistrées, sans données utilisateur de démonstration. Les unités restent séparées : aucune conversion CO2 sans source vérifiée.

Les informations sont conservées dans un fichier chiffré, avec une clé protégée par le stockage sécurisé d’Android ou le trousseau iOS. Les mots de passe et codes de récupération sont hachés. 

## Développement

.NET SDK 10.0.401, workloads `maui-android` / `maui-ios` 10.0.401.1, Android SDK API36 et JDK21 ; Xcode27 pour iOS.

```sh
dotnet workload install maui-android maui-ios --version 10.0.401.1
dotnet restore EcoActions/EcoActions.csproj --locked-mode
dotnet build EcoActions/EcoActions.csproj -f net10.0-android -c Debug
dotnet build EcoActions/EcoActions.csproj -f net10.0-android -c Release
dotnet run --project EcoActions.Tests/EcoActions.Tests.csproj
```

Si les SDK ne sont pas détectés, préciser `-p:AndroidSdkDirectory=...` et `-p:JavaSdkDirectory=...`.

L’APK se trouve dans `EcoActions/bin/Release/net10.0-android/`. 

## Structure

- `EcoActions.Core` : comptes, validation, catalogue et journal.
- `EcoActions/Services` : session et persistance.
- `EcoActions/UI` : composants et styles.
- `EcoActions/Pages` : écrans.
- `EcoActions/Platforms/Android` : notifications et cycle Android.
- `EcoActions.Tests` : tests métier exécutables sans framework supplémentaire.


## Version iOS


## Examen pratique - activité 7

- Phase A : domaine, validation et persistance.
- Phase B : interfaces et navigation.
- Phase C : intégrations Android/iOS, ressources, assemblage et tests.
- Fonctionnalités livrées : catalogue de 16 gestes, recherche et filtres, favoris, quantités, journal avec retrait et rétablissement, défi, bilan et préférences de rappel.

