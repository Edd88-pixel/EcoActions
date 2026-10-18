using EcoActions.Core;
using EcoActions.Pages;
using EcoActions.Services;

namespace EcoActions.UI;

public sealed class AppRouter(Window window, AppSession session)
{
    public AppSession Session { get; } = session;

    public void Show(string route, Gesture? gesture = null)
    {
#if IOS
        if (route is "home" or "catalogue" or "challenges" or "journal" or "stats")
        {
            window.Page = new Platforms.iOS.IosTabsPage(this, route);
            return;
        }
#endif
        window.Page = route switch
        {
            "login" => new LoginPage(this),
            "signup" => new SignupPage(this),
            "recover" => new RecoveryPage(this),
            "home" => new HomePage(this),
            "catalogue" => new CataloguePage(this),
            "detail" => new DetailPage(this, gesture ?? throw new ArgumentNullException(nameof(gesture))),
            "challenges" => new ChallengesPage(this),
            "journal" => new JournalPage(this),
            "stats" => new StatsPage(this),
            "profile" => new ProfilePage(this),
            "reminders" => new RemindersPage(this),
            "saved" => new ConfirmationPage(this, "Préférences enregistrées.",
                "À l’heure qui vous convient.", "Retour à mon profil", "profile"),
            _ => throw new ArgumentException("Destination inconnue.", nameof(route))
        };
    }

    public void Confirm(string title, string description, string action, string destination)
        => window.Page = new ConfirmationPage(this, title, description, action, destination);

    public void ShowStorageError(Func<Task> retry)
    {
        var content = new VerticalStackLayout { Spacing = 16, Padding = 24, VerticalOptions = LayoutOptions.Center };
        content.Add(Design.Text("Votre espace est indisponible.", 28, "Bold"));
        content.Add(Design.Text("Vos données n’ont pas pu être ouvertes. Réessayez sans supprimer l’application.", color: Design.Muted));
        content.Add(Design.Link("Réessayer", "retry-storage", async (_, _) => await retry()));
        window.Page = new ContentPage { BackgroundColor = Colors.White, Content = content };
    }
}
