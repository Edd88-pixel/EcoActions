using EcoActions.UI;

namespace EcoActions.Pages;

public sealed class ConfirmationPage : EcoPage
{
    public ConfirmationPage(AppRouter router, string title, string description, string action, string destination)
        : base(router, "confirmation", back: destination)
    {
        var check = Design.SurfaceBox(Design.Icon("asset_3558d.png", 21), 99,
            new Thickness(6), Design.Forest, false);
        check.WidthRequest = check.HeightRequest = 32;
        check.HorizontalOptions = LayoutOptions.Start;
        Body.Add(check);
        Body.Add(Design.Title(title, description));
        Body.Add(Action(action, "confirmation-back", () =>
        { Router.Show(destination); return Task.CompletedTask; }));
    }
}
