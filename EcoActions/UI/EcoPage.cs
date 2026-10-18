using System.Security.Cryptography;
using EcoActions.Core;

namespace EcoActions.UI;

public abstract class EcoPage : ContentPage
{
    protected AppRouter Router { get; }
    protected Services.AppSession Session => Router.Session;
    protected VerticalStackLayout Body { get; } = new() { Spacing = 16, Padding = new Thickness(24, 12, 24, 20) };
    protected Label Error { get; } = Design.Text("", 12, color: Color.FromArgb("#A84040"));
    private readonly ActivityIndicator _busy = new() { Color = Design.Forest, IsVisible = false };
    private bool _running;
    private readonly string? _back;
    private readonly Dictionary<string, FormField> _fields = [];
    private readonly ScrollView _scroll;

    protected EcoPage(AppRouter router, string id, string? activeTab = null, string? back = null)
    {
        Router = router;
        _back = back;
        AutomationId = "screen-" + id;
        BackgroundColor = Colors.White;
        SafeAreaEdges = new Microsoft.Maui.SafeAreaEdges(Microsoft.Maui.SafeAreaRegions.All);
        var layout = new Grid { RowDefinitions = { new() { Height = GridLength.Star }, new() { Height = GridLength.Auto } } };
        var content = new VerticalStackLayout { Spacing = 0 };
        content.Add(Body);
        Error.IsVisible = false;
        var feedback = new VerticalStackLayout { Padding = new Thickness(24, 0, 24, 16), Spacing = 6 };
        feedback.Add(_busy);
        feedback.Add(Error);
        content.Add(feedback);
        _scroll = new ScrollView { Content = content, VerticalScrollBarVisibility = ScrollBarVisibility.Never };
        layout.Add(_scroll);
#if !IOS
        if (activeTab is not null)
        {
            var navigation = new NavigationBar(router, activeTab);
            layout.Add(navigation);
            Grid.SetRow(navigation, 1);
        }
#endif
        Content = layout;
    }

    protected void Header()
    {
        var row = Design.Columns(25, GridLength.Star, 36);
        row.VerticalOptions = LayoutOptions.Center;
        Design.Cell(row, Design.Icon("asset_c8051.png", 25), 0);
        Design.Cell(row, Design.Text("EcoActions", 20, "SemiBold", Design.Forest), 1);
        var initial = Session.Active?.Name[..1].ToUpperInvariant() ?? "E";
        var label = Design.Text(initial, 15, "SemiBold", Design.Forest);
        label.HorizontalTextAlignment = TextAlignment.Center;
        label.VerticalTextAlignment = TextAlignment.Center;
        var avatar = Design.SurfaceBox(label, 99, new Thickness(0), Design.Sage, false);
        avatar.HeightRequest = avatar.WidthRequest = 36;
        avatar.AutomationId = "open-profile";
        SemanticProperties.SetDescription(avatar, "Ouvrir mon profil");
        if (Session.Active is not null)
        {
            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) => Router.Show("profile");
            avatar.GestureRecognizers.Add(tap);
        }
        Design.Cell(row, avatar, 2);
        Body.Add(row);
    }

    protected Button Action(string text, string id, Func<Task> action, string arrow = "asset_00350.png")
        => Design.Link(text, id, async (_, _) => await RunAsync(action), arrow);

    protected void AddField(FormField field)
    {
        _fields.Add(field.Field, field);
        Body.Add(field);
    }

    protected async Task RunAsync(Func<Task> action)
    {
        if (_running) return;
        _running = true;
        Error.IsVisible = false;
        foreach (var field in _fields.Values) field.ClearError();
        Body.IsEnabled = false;
        _busy.IsRunning = _busy.IsVisible = true;
        try { await action(); }
        catch (Exception error) when (error is ArgumentException or InvalidOperationException or IOException or CryptographicException)
        {
            var message = error is ArgumentException or InvalidOperationException
                ? error.Message : "L’enregistrement n’a pas abouti. Réessayez sans fermer l’application.";
            if (error is InputError input && _fields.TryGetValue(input.Field, out var field))
            {
                field.ShowError(message);
                await _scroll.ScrollToAsync(field, ScrollToPosition.Center, false);
            }
            else
            {
                Error.Text = message;
                Error.IsVisible = true;
                await _scroll.ScrollToAsync(Error, ScrollToPosition.End, false);
            }
            SemanticScreenReader.Default.Announce(message);
        }
        finally
        {
            _running = false;
            Body.IsEnabled = true;
            _busy.IsRunning = _busy.IsVisible = false;
        }
    }

    protected override bool OnBackButtonPressed()
    {
        if (_back is null) return base.OnBackButtonPressed();
        Router.Show(_back);
        return true;
    }
}
