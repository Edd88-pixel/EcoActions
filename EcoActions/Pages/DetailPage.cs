using System.Globalization;
using EcoActions.Core;
using EcoActions.UI;

namespace EcoActions.Pages;

public sealed class DetailPage : EcoPage
{
    public DetailPage(AppRouter router, Gesture gesture) : base(router, "detail", "catalogue", "catalogue")
    {
        Body.Add(Action("Les petits gestes", "detail-back", () =>
        { Router.Show("catalogue"); return Task.CompletedTask; }, "asset_e16b4.png"));
        Body.Add(Design.Photo(gesture.Image, 220));
        Body.Add(GestureViews.Category(gesture));
        var favorite = Design.Link(FavoriteLabel(), "favorite-gesture", async (sender, _) =>
            await RunAsync(async () =>
            {
                await Session.UpdateAccountAsync(account => FavoriteGestures.Toggle(account, gesture.Id));
                ((Button)sender!).Text = FavoriteLabel();
                SemanticProperties.SetDescription((Button)sender!, FavoriteLabel());
            }), "");
        string FavoriteLabel() => Session.Account.FavoriteGestureIds.Contains(gesture.Id)
            ? "Retirer des favoris" : "Ajouter aux favoris";
        Body.Add(favorite);
        Body.Add(Design.Title(gesture.Title, gesture.Description));
        Body.Add(Design.Text("Comment commencer", 20, "SemiBold"));
        Body.Add(Design.Text(string.Join("\n", gesture.Steps.Select((step, i) => $"{i + 1}. {step}")), color: Design.Muted));
        Body.Add(Design.Text("Ce que vous enregistrez", 20, "SemiBold"));
        Body.Add(Design.Text(Journal.Summary(gesture, gesture.DefaultQuantity) + " · " + gesture.Category,
            14, "SemiBold", Design.Forest));
        Body.Add(Design.Notice("Les distances, bouteilles et repas restent séparés. Une estimation CO2 exige une source vérifiée."));
        var quantity = new FormField("Quantité réalisée", "gesture-quantity", keyboard: Keyboard.Numeric);
        quantity.Input.Text = gesture.DefaultQuantity.ToString(CultureInfo.CurrentCulture);
        AddField(quantity);
        var presets = new HorizontalStackLayout { Spacing = 12 };
        foreach (var amount in new[] { 1, 2, 5 })
        {
            var preset = Action(amount.ToString(), "quantity-" + amount, () =>
            {
                quantity.Input.Text = amount.ToString(CultureInfo.CurrentCulture);
                return Task.CompletedTask;
            }, "");
            SemanticProperties.SetDescription(preset, "Choisir une quantité de " + amount);
            presets.Add(preset);
        }
        Body.Add(presets);
        Body.Add(Action("Enregistrer ce geste", "detail-record", async () =>
        {
            if (!decimal.TryParse(quantity.Value, NumberStyles.Number, CultureInfo.CurrentCulture, out var value))
                throw new InputError("quantity", "Indiquez une quantité valide.");
            await Session.AddAsync(gesture, value);
            Router.Show("journal");
        }));
    }
}
