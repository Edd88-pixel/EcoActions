namespace EcoActions.Core;

public static class GestureCatalog
{
    public static IReadOnlyList<Gesture> All { get; } =
    [
        new("marche", "Mobilité", "Marcher pour un trajet court", "Marcher 2 km",
            "Un petit trajet à pied au quotidien.", "asset_f3138.png", "asset_c53bf.png", "#E3F2E4", "km à pied", 2,
            ["Choisissez un trajet habituel et accessible.", "Prenez des chaussures confortables.", "Marchez à votre rythme, sans objectif imposé."], AllowsFraction: true),
        new("gourde", "Consommation", "Emporter une gourde", "Prendre sa gourde",
            "De l’eau partout, sans déchet.", "asset_6d16b.png", "asset_3e75f.png", "#E5F2FF", "bouteille évitée", 1,
            ["Remplissez votre gourde avant de partir.", "Gardez-la à portée de main.", "Enregistrez les bouteilles jetables remplacées."]),
        new("repas", "Alimentation", "Choisir un repas végétal", "Un repas végétal",
            "Plus de légumes dans l’assiette.", "asset_66002.png", "asset_0dedc.png", "#FFE9DD", "repas végétal", 1,
            ["Choisissez des légumes de saison.", "Ajoutez des légumineuses et des céréales.", "Adaptez les portions pour éviter le gaspillage."]),
        new("energie", "Énergie", "Éteindre les appareils inutilisés", "Éteindre une lumière",
            "Des économies, tout simplement.", "asset_286c9.png", "asset_05cf3.png", "#FFF5CC", "extinction", 1,
            ["Repérez une lumière ou un appareil inutilisé.", "Éteignez-le lorsque c’est possible.", "Enregistrez le geste, sans inventer une consommation mesurée."]),
        new("escaliers", "Mobilité", "Prendre les escaliers", "Prendre les escaliers",
            "Quelques marches à votre rythme.", "asset_f3138.png", "asset_c53bf.png", "#E3F2E4", "montée", 1,
            ["Choisissez un escalier accessible.", "Montez à votre rythme si votre santé le permet.", "Comptez les montées réalisées."], "montées"),
        new("courses-pied", "Mobilité", "Faire une course à pied", "Une course à pied",
            "Une petite course près de chez vous.", "asset_f3138.png", "asset_c53bf.png", "#E3F2E4", "trajet à pied", 1,
            ["Repérez un commerce accessible à pied.", "Prenez un sac réutilisable.", "Comptez chaque trajet effectué."], "trajets à pied"),
        new("promenade", "Mobilité", "Sortir marcher près de chez soi", "Une promenade locale",
            "Profiter de son quartier sans voiture.", "asset_f3138.png", "asset_c53bf.png", "#E3F2E4", "promenade", 1,
            ["Choisissez un parcours près de chez vous.", "Partez directement à pied.", "Enregistrez votre sortie."], "promenades"),
        new("eau-robinet", "Consommation", "Choisir l’eau du robinet", "De l’eau sans emballage",
            "Lorsque l’eau est potable, évitez un emballage.", "asset_6d16b.png", "asset_3e75f.png", "#E5F2FF", "choix sans emballage", 1,
            ["Vérifiez que l’eau est potable dans votre lieu de vie.", "Remplissez un verre ou une carafe propre.", "Comptez les occasions sans bouteille jetable."], "choix sans emballage"),
        new("recharge", "Consommation", "Remplir sa gourde en chemin", "Recharger sa gourde",
            "Une recharge plutôt qu’un nouvel achat.", "asset_6d16b.png", "asset_3e75f.png", "#E5F2FF", "recharge", 1,
            ["Repérez un point d’eau potable.", "Remplissez votre gourde propre.", "Comptez les recharges effectuées."], "recharges"),
        new("contenant", "Consommation", "Réutiliser un contenant", "Réutiliser plutôt que jeter",
            "Donner un nouvel usage à ce que l’on possède.", "asset_6d16b.png", "asset_3e75f.png", "#E5F2FF", "réutilisation", 1,
            ["Choisissez un contenant propre et adapté à son usage.", "Réutilisez-le plutôt que de prendre du jetable.", "Comptez les réutilisations."], "réutilisations"),
        new("saison", "Alimentation", "Choisir des légumes de saison", "Des légumes de saison",
            "Composer son repas avec la saison.", "asset_66002.png", "asset_0dedc.png", "#FFE9DD", "repas de saison", 1,
            ["Repérez les légumes de saison dans votre région.", "Choisissez une recette simple.", "Comptez les repas préparés."], "repas de saison"),
        new("legumineuses", "Alimentation", "Cuisiner des légumineuses", "Lentilles, pois ou haricots",
            "Varier les protéines dans son assiette.", "asset_66002.png", "asset_0dedc.png", "#FFE9DD", "repas avec légumineuses", 1,
            ["Choisissez des lentilles, des pois ou des haricots.", "Préparez-les selon les indications du produit.", "Ajoutez des légumes et enregistrez le repas."], "repas avec légumineuses"),
        new("restes", "Alimentation", "Cuisiner les restes", "Un repas avec les restes",
            "Utiliser ce qui est déjà dans le réfrigérateur.", "asset_66002.png", "asset_0dedc.png", "#FFE9DD", "repas avec les restes", 1,
            ["Vérifiez la fraîcheur et la conservation des aliments.", "Préparez une recette adaptée aux restes disponibles.", "Comptez les repas réalisés."], "repas avec les restes"),
        new("veille", "Énergie", "Couper une veille inutile", "Couper une veille",
            "Débrancher un appareil dont on n’a plus besoin.", "asset_286c9.png", "asset_05cf3.png", "#FFF5CC", "veille coupée", 1,
            ["Choisissez un appareil que vous pouvez éteindre sans risque.", "Éteignez sa multiprise ou débranchez-le.", "Laissez branchés les équipements qui doivent fonctionner."], "veilles coupées"),
        new("jour", "Énergie", "Profiter de la lumière du jour", "Ouvrir les rideaux",
            "Éclairer naturellement quand cela suffit.", "asset_286c9.png", "asset_05cf3.png", "#FFF5CC", "éclairage évité", 1,
            ["Ouvrez les rideaux lorsque la lumière suffit.", "Éteignez la lampe devenue inutile.", "Comptez les éclairages évités."], "éclairages évités"),
        new("ecran", "Énergie", "Éteindre son écran en partant", "Un écran au repos",
            "Un écran éteint pendant une vraie pause.", "asset_286c9.png", "asset_05cf3.png", "#FFF5CC", "écran éteint", 1,
            ["Enregistrez votre travail.", "Éteignez l’écran pendant votre absence.", "Comptez les écrans éteints, sans inventer de kWh."], "écrans éteints")
    ];

    public static Gesture Get(string id) => All.Single(g => g.Id == id);

    public static IEnumerable<Gesture> Search(string query, string? category) => All.Where(g =>
        (category is null || g.Category == category) &&
        (g.Title.Contains(query.Trim(), StringComparison.CurrentCultureIgnoreCase) ||
         g.Category.Contains(query.Trim(), StringComparison.CurrentCultureIgnoreCase)));
}
