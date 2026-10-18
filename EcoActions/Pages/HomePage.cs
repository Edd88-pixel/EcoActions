using EcoActions.Core;
using EcoActions.UI;

namespace EcoActions.Pages;

public sealed class HomePage : EcoPage
{
    public HomePage(AppRouter router) : base(router, "home", "home")
    {
        Header();
        Body.Add(Design.Title($"Bonjour {Session.Account.Name}.", "Un geste à la fois."));
        var count = Journal.Period(Session.Account, DateTimeOffset.Now, false).Count;
        var marks = string.Join(" ", Enumerable.Range(0, 3).Select(i => i < count ? "●" : "○"));
        Body.Add(Design.Text($"{count} geste{(count == 1 ? "" : "s")} cette semaine   {marks}", color: Design.Muted));
        var heading = Design.Columns(GridLength.Star, GridLength.Auto);
        Design.Cell(heading, Design.Text("Aujourd’hui", 20, "SemiBold"), 0);
        Design.Cell(heading, Action("Voir tout", "home-catalogue", () =>
        { Router.Show("catalogue"); return Task.CompletedTask; }), 1);
        Body.Add(heading);
        for (var i = 0; i < 4; i += 2)
        {
            var row = Design.Columns(GridLength.Star, GridLength.Star);
            row.ColumnSpacing = 12;
            for (var column = 0; column < 2; column++)
            {
                var gesture = GestureCatalog.All[i + column];
                var recorded = Session.Account.Journal.Any(e => e.GestureId == gesture.Id && e.RecordedAt.LocalDateTime.Date == DateTime.Today);
                Design.Cell(row, GestureViews.Card(gesture, recorded, async () =>
                { await Session.AddAsync(gesture, gesture.DefaultQuantity); Router.Show("journal"); },
                    () => Router.Show("detail", gesture), RunAsync), column);
            }
            Body.Add(row);
        }
        var challenge = new VerticalStackLayout { Spacing = 4 };
        challenge.Add(Design.Text("MON DÉFI", 11, "Medium", Design.Muted));
        challenge.Add(Design.Text("7 gestes, un nouvel élan", 15, "SemiBold"));
        challenge.Add(Design.Text($"{Journal.ChallengeProgress(Session.Account)} / 7 gestes réalisés", 12, color: Design.Forest));
        var box = Design.Columns(64, GridLength.Star);
        Design.Cell(box, Design.Photo("asset_89395.png", 64, 64), 0);
        Design.Cell(box, challenge, 1);
        var card = Design.SurfaceBox(box, padding: new Thickness(10));
        card.AutomationId = "home-challenge";
        var tap = new TapGestureRecognizer();
        tap.Tapped += (_, _) => Router.Show("challenges");
        card.GestureRecognizers.Add(tap);
        SemanticProperties.SetDescription(card, "Voir mon défi : 7 gestes, un nouvel élan");
        Body.Add(card);
    }
}
