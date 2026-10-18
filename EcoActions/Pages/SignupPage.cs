using EcoActions.Core;
using EcoActions.UI;

namespace EcoActions.Pages;

public sealed class SignupPage : EcoPage
{
    public SignupPage(AppRouter router) : base(router, "signup", back: "login")
    {
        Body.Add(new AuthHero(() => Router.Show("login")));
        Body.Add(Design.Title("Un premier pas.", "Quelques informations pour commencer."));
        var name = new FormField("Prénom", "signup-name");
        var email = new FormField("Adresse e-mail", "signup-email", keyboard: Keyboard.Email);
        var password = new FormField("Mot de passe", "signup-password", true);
        var confirmation = new FormField("Confirmer le mot de passe", "signup-confirmation", true);
        AddField(name);
        AddField(email);
        AddField(password);
        AddField(confirmation);
        Body.Add(Design.Text("Choisissez un mot de passe d’au moins 10 caractères.", 12, color: Design.Muted));
        Body.Add(Action("Créer mon compte", "signup-submit", async () =>
        {
            var result = await Session.ChangeAsync(data => EcoActions.Core.Accounts.Register(data, name.Value, email.Value,
                password.Value, confirmation.Value));
            await DisplayAlertAsync("Conservez votre code", result.RecoveryCode +
                "\n\nCe code permet de retrouver votre compte si vous oubliez votre mot de passe. Gardez-le dans un endroit sûr.", "J’ai noté le code");
            Router.Show("home");
        }));
        Body.Add(Action("Déjà un compte ? Se connecter", "signup-login", () =>
        { Router.Show("login"); return Task.CompletedTask; }));
    }
}
