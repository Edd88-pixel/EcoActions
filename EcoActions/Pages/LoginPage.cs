using EcoActions.Core;
using EcoActions.UI;

namespace EcoActions.Pages;

public sealed class LoginPage : EcoPage
{
    public LoginPage(AppRouter router) : base(router, "login")
    {
        Header();
        Body.Add(GestureViews.ThreePhotos());
        Body.Add(Design.Title("Un geste à la fois.", "Retrouvez votre espace EcoActions."));
        var email = new FormField("Adresse e-mail", "login-email", keyboard: Keyboard.Email);
        var password = new FormField("Mot de passe", "login-password", true);
        AddField(email);
        AddField(password);
        Body.Add(Action("Se connecter", "login-submit", async () =>
        {
            var account = await Session.ChangeAsync(data => EcoActions.Core.Accounts.Login(data, email.Value, password.Value));
            if (account.RemindersEnabled) await EcoActions.Services.ReminderService.ScheduleAsync(account.ReminderTime);
            Router.Show("home");
        }));
        Body.Add(Action("Mot de passe oublié ?", "open-recovery", () =>
        { Router.Show("recover"); return Task.CompletedTask; }, "asset_e16b4.png"));
        Body.Add(Action("Créer mon compte", "open-signup", () =>
        { Router.Show("signup"); return Task.CompletedTask; }));
        Body.Add(Design.Notice("Des petits gestes, à votre rythme."));
    }
}
