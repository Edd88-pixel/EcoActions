using EcoActions.Core;
using EcoActions.UI;

namespace EcoActions.Pages;

public sealed class JournalPage : EcoPage
{
    public JournalPage(AppRouter router) : base(router, "journal", "journal", "home")
    {
        Header();
        Body.Add(Design.Title("Votre journal.", "Chaque petit geste compte."));
        var heading = Design.Columns(GridLength.Star, GridLength.Auto);
        Design.Cell(heading, Design.Text("Vos gestes", 20, "SemiBold"), 0);
        Design.Cell(heading, Action("Ajouter", "journal-add", () =>
        { Router.Show("catalogue"); return Task.CompletedTask; }), 1);
        Body.Add(heading);
        Body.Add(Design.Text($"{Session.Account.Journal.Count} geste{(Session.Account.Journal.Count == 1 ? " enregistré" : "s enregistrés")}", 12, color: Design.Muted));
        var removed = Session.Account.LastRemoved is not null;
        if (removed) Body.Add(Design.Notice("Le dernier geste a été retiré de votre journal.", true));
        var entries = Session.Account.Journal.OrderByDescending(e => e.RecordedAt).ToArray();
        if (entries.Length == 0)
        {
            Body.Add(Design.Text("Votre premier geste vous attend.", 20, "SemiBold"));
            Body.Add(Design.Text("Choisissez un petit geste pour commencer votre journal.", color: Design.Muted));
            Body.Add(Action("Choisir un geste", "empty-journal-action", () =>
            { Router.Show("catalogue"); return Task.CompletedTask; }));
        }
        foreach (var entry in entries)
        {
            var gesture = GestureCatalog.Get(entry.GestureId);
            Body.Add(GestureViews.Row(gesture, Journal.Summary(gesture, entry.Quantity), true,
                null, () => Router.Show("detail", gesture), RunAsync, remove: async () =>
                {
                    await Session.UpdateAccountAsync(account => Journal.Remove(account, entry.Id));
                    Router.Show("journal");
                }));
            Body.Add(Design.Text(entry.RecordedAt.LocalDateTime.ToString("dd/MM · HH:mm"), 11, color: Design.Muted));
        }
        if (removed)
            Body.Add(Action("Rétablir le geste", "journal-restore", async () =>
            { await Session.UpdateAccountAsync(Journal.Restore); Router.Show("journal"); }, "asset_e16b4.png"));
        else if (entries.Length > 0)
            Body.Add(Action("Annuler le dernier geste", "journal-undo", async () =>
            { await Session.UpdateAccountAsync(Journal.Undo); Router.Show("journal"); }, "asset_e16b4.png"));
        Body.Add(Design.Notice("Votre historique reste personnel. Aucune comparaison avec d’autres utilisateurs."));
    }
}
