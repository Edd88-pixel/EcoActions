using EcoActions.Core;
using EcoActions.UI;

namespace EcoActions.Pages;

public sealed class RecoveryPage : EcoPage
{
    public RecoveryPage(AppRouter router) : base(router, "recover", back: "login")
    {
        Body.Add(new AuthHero(() => Router.Show("login")));
        Body.Add(Design.Title("Retrouver votre espace.", "Utilisez le code conservé lors de la création de votre compte."));
        var email = new FormField("Adresse e-mail", "recovery-email", keyboard: Keyboard.Email);
        var code = new FormField("Code de récupération", "recovery-code");
        var password = new FormField("Nouveau mot de passe", "recovery-password", true);
        AddField(email);
        AddField(code);
        AddField(password);
        Body.Add(Action("Retrouver mon compte", "recovery-submit", async () =>
        {
            await Session.ChangeAsync(data =>
            { EcoActions.Core.Accounts.ResetPassword(data, email.Value, code.Value, password.Value); return true; });
            Router.Confirm("Votre espace vous attend.", "Votre mot de passe a été changé. Vous pouvez vous reconnecter.",
                "Retour à la connexion", "login");
        }));
        Body.Add(Design.Notice("Le code est nécessaire pour protéger votre compte. Aucun e-mail ne sera envoyé."));
        Body.Add(Action("Retour à la connexion", "recovery-login", () =>
        { Router.Show("login"); return Task.CompletedTask; }, "asset_e16b4.png"));
    }
}
