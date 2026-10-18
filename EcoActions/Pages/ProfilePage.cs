using EcoActions.Core;
using EcoActions.UI;

namespace EcoActions.Pages;

public sealed class ProfilePage : EcoPage
{
    public ProfilePage(AppRouter router) : base(router, "profile", "home", "home")
    {
        Header();
        Body.Add(Design.Title("Votre espace.", "Simple, personnel, à votre rythme."));
        var initial = Design.Text(Session.Account.Name[..1].ToUpperInvariant(), 28, "Bold", Design.Forest);
        initial.HorizontalTextAlignment = TextAlignment.Center;
        initial.VerticalTextAlignment = TextAlignment.Center;
        var avatar = Design.SurfaceBox(initial, 99, new Thickness(0), Design.Sage, false);
        avatar.WidthRequest = avatar.HeightRequest = 64;
        var profile = Design.Columns(64, GridLength.Star);
        var info = new VerticalStackLayout { Spacing = 4, VerticalOptions = LayoutOptions.Center };
        info.Add(Design.Text(Session.Account.Name, 20, "SemiBold"));
        info.Add(Design.Text(Session.Account.Email, 12, color: Design.Muted));
        Design.Cell(profile, avatar, 0);
        Design.Cell(profile, info, 1);
        Body.Add(profile);
        var gestures = Journal.Period(Session.Account, DateTimeOffset.Now, false).Count;
        var challenges = Session.Account.ChallengeStartedAt is null ? 0 : 1;
        Body.Add(Design.Text($"{gestures} geste{(gestures == 1 ? "" : "s")} cette semaine · {challenges} défi rejoint", 12, color: Design.Forest));
        Body.Add(Shortcut("Mes rappels", "profile-reminders", "asset_bell.png", "reminders"));
        Body.Add(Shortcut("Explorer les gestes", "profile-catalogue", "asset_c8051.png", "catalogue"));
        Body.Add(Shortcut("Mon bilan", "profile-stats", "asset_b8f60.png", "stats"));
        Body.Add(Design.Text("Vos données", 20, "SemiBold"));
        Body.Add(Design.Text("Votre journal est personnel. Vous gardez le choix des gestes et des rappels.", color: Design.Muted));
        Body.Add(Action("Se déconnecter", "logout", async () =>
        { await Session.LogoutAsync(); Router.Show("login"); }, "asset_e16b4.png"));
    }

    private View Shortcut(string title, string id, string icon, string destination)
    {
        var row = Design.Columns(20, GridLength.Star, 20);
        row.HeightRequest = 44;
        Design.Cell(row, Design.Icon(icon), 0);
        var label = Design.Text(title, color: Design.Forest);
        label.VerticalOptions = LayoutOptions.Center;
        Design.Cell(row, label, 1);
        Design.Cell(row, Design.Icon("asset_00350.png"), 2);
        row.AutomationId = id;
        SemanticProperties.SetDescription(row, title);
        var tap = new TapGestureRecognizer();
        tap.Tapped += (_, _) => Router.Show(destination);
        row.GestureRecognizers.Add(tap);
        return row;
    }
}
