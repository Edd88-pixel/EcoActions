using EcoActions.UI;
using EcoActions.Services;

namespace EcoActions.Pages;

public sealed class RemindersPage : EcoPage
{
    public RemindersPage(AppRouter router) : base(router, "reminders", "home", "profile")
    {
        Body.Add(Action("Votre profil", "reminders-back", () =>
        { Router.Show("profile"); return Task.CompletedTask; }, "asset_e16b4.png"));
        Body.Add(Design.Title("Un petit rappel.", "À l’heure qui vous convient."));
        Body.Add(Design.Photo("asset_6d16b.png", 120));
        var enabled = new Switch
        {
            IsToggled = Session.Account.RemindersEnabled,
            OnColor = Design.Forest,
            AutomationId = "reminders-toggle"
        };
        SemanticProperties.SetDescription(enabled, "Recevoir un rappel");
        var row = Design.Columns(GridLength.Star, GridLength.Auto);
        Design.Cell(row, Design.Text("Recevoir un rappel", 15, "SemiBold"), 0);
        Design.Cell(row, enabled, 1);
        Body.Add(row);
        Body.Add(Design.Text("Heure du rappel", 11, "Medium"));
#if IOS
        var time = new Platforms.iOS.GlassTimePicker { Time = Session.Account.ReminderTime,
            HeightRequest = 48, WidthRequest = 170, HorizontalOptions = LayoutOptions.Start,
            AutomationId = "reminder-time", IsEnabled = enabled.IsToggled };
        Body.Add(time);
#else
        var time = new TimePicker
        {
            Time = Session.Account.ReminderTime,
            Format = "HH:mm",
            FontFamily = "InterRegular",
            FontSize = 14,
            AutomationId = "reminder-time",
            IsEnabled = enabled.IsToggled
        };
        Body.Add(Design.SurfaceBox(time, 10, new Thickness(12, 0)));
#endif
        SemanticProperties.SetDescription(time, "Heure du rappel");
        enabled.Toggled += (_, e) => time.IsEnabled = e.Value;
        Body.Add(Design.Text("Un rappel par jour, sans pression.", 12, color: Design.Muted));
        Body.Add(Action("Enregistrer", "save-reminders", async () =>
        {
            if (enabled.IsToggled && !await ReminderService.RequestPermissionAsync())
                throw new InvalidOperationException("Les notifications sont désactivées. Autorisez-les dans les réglages pour recevoir vos rappels.");
            var chosen = time.Time ?? throw new ArgumentException("Choisissez une heure pour votre rappel.");
            var previous = Session.Account.RemindersEnabled;
            var previousTime = Session.Account.ReminderTime;
            if (enabled.IsToggled) await ReminderService.ScheduleAsync(chosen); else ReminderService.Cancel();
            try
            {
                await Session.UpdateAccountAsync(account =>
                { account.RemindersEnabled = enabled.IsToggled; account.ReminderTime = chosen; });
            }
            catch (Exception saveError)
            {
                try
                {
                    if (previous) await ReminderService.ScheduleAsync(previousTime); else ReminderService.Cancel();
                }
                catch (Exception rollbackError)
                {
                    throw new IOException("Les préférences et le rappel n’ont pas pu être rétablis.",
                        new AggregateException(saveError, rollbackError));
                }
                throw;
            }
            Router.Show("saved");
        }));
        Body.Add(Design.Notice("Choisissez le moment qui vous convient. Vous pouvez arrêter les rappels à tout moment."));
    }
}
