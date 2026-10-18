# EcoActions

Application Android et iOS native en .NET MAUI, réalisée à partir des maquettes Figma validées.

## Fonctionnalités

Création de compte, connexion et récupération par code ; catalogue et recherche ; fiches de gestes avec quantité ; journal avec annulation et rétablissement ; défi de sept gestes ; bilan par semaine ou mois ; profil et rappels quotidiens. Les statistiques proviennent des actions enregistrées, sans données utilisateur de démonstration. Les unités restent séparées : aucune conversion CO2 sans source vérifiée.

Les informations sont conservées dans un fichier chiffré, avec une clé protégée par le stockage sécurisé d’Android ou le trousseau iOS. Les mots de passe et codes de récupération sont hachés. Aucun serveur n’est nécessaire.

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

L’APK se trouve dans `EcoActions/bin/Release/net10.0-android/`. La signature de développement permet une installation locale ; elle ne constitue pas une signature de publication.

## Structure

- `EcoActions.Core` : comptes, validation, catalogue et journal.
- `EcoActions/Services` : session et persistance.
- `EcoActions/UI` : composants et styles.
- `EcoActions/Pages` : écrans.
- `EcoActions/Platforms/Android` : notifications et cycle Android.
- `EcoActions.Tests` : tests métier exécutables sans framework supplémentaire.

Les images et icônes proviennent du fichier Figma validé. Les licences Inter et OpenSans sont incluses. Les fenêtres de permission et la barre système restent celles de la plateforme utilisée.

## Version iOS

La cible `net10.0-ios` réutilise les mêmes écrans, le domaine et la persistance. Les rappels utilisent les notifications natives iOS. Le démarrage utilise UIScene, requis par Xcode27. Inter possède des noms PostScript distincts pour conserver les graisses correctes sur iOS.

```sh
dotnet restore EcoActions/EcoActions.csproj --locked-mode
dotnet build EcoActions/EcoActions.csproj -f net10.0-ios -c Release
```

Cette commande construit le simulateur Apple Silicon. La configuration d’appareil utilise un verrou NuGet séparé, car ses bibliothèques natives diffèrent :

```sh
dotnet restore EcoActions/EcoActions.csproj -p:BuildIpa=true --locked-mode
dotnet build EcoActions/EcoActions.csproj -f net10.0-ios -c Release -p:BuildIpa=true -p:EnableCodeSigning=false
```

L’IPA ainsi produit n’est pas signé : il nécessite une signature et un profil Apple valides avant installation sur un iPhone. Pour une distribution signée, omettre `EnableCodeSigning=false` et configurer l’identité et le profil de signature. Aucun certificat ni identifiant de compte Apple n’est inclus dans les sources.

## Examen pratique - activité 7

EcoActions est le projet attribué pour l’examen pratique 2026/2027. Ce dépôt rassemble l’application existante au 18 octobre 2026. Le découpage des commits décrit son import dans Git ; il ne reconstitue pas les dates de développement précédentes.

- Phase A : domaine, validation et persistance.
- Phase B : interfaces et navigation.
- Phase C : intégrations Android/iOS, ressources, assemblage et tests.
- Fonctionnalités livrées : catalogue de 16 gestes, recherche et filtres, favoris, quantités, journal avec retrait et rétablissement, défi, bilan et préférences de rappel.

Les noms A/B/C servent ici à organiser la livraison. L’énoncé de l’activité 7 ne détaille pas leur contenu. Les phases de l’ancien TP de calculatrice portaient sur code-behind, navigation et MVVM ; EcoActions conserve ses pages en code-behind et ses services séparés, sans revendiquer une conversion MVVM.
