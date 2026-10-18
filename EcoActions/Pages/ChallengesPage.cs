using EcoActions.Core;
using EcoActions.UI;

namespace EcoActions.Pages;

public sealed class ChallengesPage : EcoPage
{
    public ChallengesPage(AppRouter router) : base(router, "challenges", "challenges", "home")
    {
        Header();
        Body.Add(Design.Title("Un défi à votre portée.", "À votre rythme, sans classement."));
        var card = new VerticalStackLayout { Spacing = 16 };
        card.Add(GestureViews.ThreePhotos());
        card.Add(Design.Text("7 gestes, un nouvel élan", 20, "SemiBold"));
        card.Add(Design.Text("Sept actions concrètes pour commencer.", color: Design.Muted));
        var count = Journal.ChallengeProgress(Session.Account);
        var progress = Design.Columns(GridLength.Star, GridLength.Auto);
        Design.Cell(progress, Design.Text("Votre progression"), 0);
        Design.Cell(progress, Design.Text($"{count} / 7", 14, "SemiBold", Design.Forest), 1);
        card.Add(progress);
        card.Add(new ProgressBar { Progress = count / 7d, ProgressColor = Design.Forest,
            BackgroundColor = Design.Line, HeightRequest = 8 });
        var circles = Design.Columns(Enumerable.Repeat(GridLength.Star, 7).ToArray());
        for (var i = 0; i < 7; i++)
        {
            View content = i < count ? Design.Icon("asset_3558d.png", 21) : new ContentView();
            var circle = Design.SurfaceBox(content, 99, new Thickness(6), i < count ? Design.Forest : Design.Line, false);
            circle.WidthRequest = circle.HeightRequest = 32;
            Design.Cell(circles, circle, i);
        }
        card.Add(circles);
        Body.Add(Design.SurfaceBox(card, padding: new Thickness(12)));
        if (Session.Account.ChallengeStartedAt is null)
            Body.Add(Action("Rejoindre le défi", "join-challenge", async () =>
            {
                await Session.UpdateAccountAsync(a => a.ChallengeStartedAt = DateTimeOffset.Now);
                Router.Show("challenges");
            }));
        else
        {
            Body.Add(Design.Text(count == 7 ? "Défi accompli. Bravo !" : "Votre prochain petit pas", 20, "SemiBold"));
            var gesture = GestureCatalog.All.MinBy(g => Session.Account.Journal.Count(e => e.GestureId == g.Id))!;
            Body.Add(GestureViews.Row(gesture, gesture.Title, false, async () =>
            { await Session.AddAsync(gesture, gesture.DefaultQuantity); Router.Show("challenges"); },
                () => Router.Show("detail", gesture), RunAsync));
            Body.Add(Design.Text("✓ Défi rejoint", color: Design.Forest));
            Body.Add(Action("Choisir un geste", "challenge-catalogue", () =>
            { Router.Show("catalogue"); return Task.CompletedTask; }));
        }
    }
}
