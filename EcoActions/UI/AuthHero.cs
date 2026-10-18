namespace EcoActions.UI;

public sealed class AuthHero : Grid
{
    public AuthHero(Action back)
    {
        HeightRequest = 212;
        IsClippedToBounds = true;
        Margin = new Thickness(-24, -12, -24, 0);
        var photo = new Image
        {
            Source = "asset_89395.png",
            Aspect = Aspect.AspectFit,
            VerticalOptions = LayoutOptions.Start,
            TranslationY = -28
        };
        SizeChanged += (_, _) => photo.HeightRequest = Width * 1402 / 1122;
        Add(photo);
        var returnButton = new ImageButton
        {
            Source = "asset_e16b4.png",
            Padding = 12,
            BackgroundColor = Colors.White,
            WidthRequest = 44,
            HeightRequest = 44,
            CornerRadius = 22,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(20, 16),
            AutomationId = "auth-back"
        };
        returnButton.Clicked += (_, _) => back();
        SemanticProperties.SetDescription(returnButton, "Retour à la connexion");
        Add(returnButton);
        var info = new ImageButton
        {
            Source = "asset_d2dcf.png",
            Padding = 12,
            BackgroundColor = Colors.White,
            WidthRequest = 44,
            HeightRequest = 44,
            CornerRadius = 22,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(20, 16),
            AutomationId = "auth-info"
        };
#if IOS
        if (OperatingSystem.IsIOSVersionAtLeast(26))
            returnButton.BackgroundColor = info.BackgroundColor = Colors.Transparent;
#endif
        SemanticProperties.SetDescription(info, "À propos de votre compte");
        info.Clicked += async (_, _) =>
        {
            if (Application.Current?.Windows[0].Page is { } page)
                await page.DisplayAlertAsync("EcoActions", "Avancez à votre rythme, un geste à la fois.", "Compris");
        };
        Add(info);
    }
}
