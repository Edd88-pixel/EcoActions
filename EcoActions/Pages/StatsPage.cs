using EcoActions.Core;
using EcoActions.UI;

namespace EcoActions.Pages;

public sealed class StatsPage : EcoPage
{
    private readonly VerticalStackLayout _summary = new() { Spacing = 16 };
    private readonly Button _week;
    private readonly Button _month;
    private bool _monthly;

    public StatsPage(AppRouter router) : base(router, "stats", "stats", "home")
    {
        Header();
        Body.Add(Design.Title("Vos progrès.", "Du concret, à votre rythme."));
        _week = Action("Semaine", "stats-week", () => { SetPeriod(false); return Task.CompletedTask; }, "");
        _month = Action("Mois", "stats-month", () => { SetPeriod(true); return Task.CompletedTask; }, "");
        Body.Add(new HorizontalStackLayout { Spacing = 24, Children = { _week, _month } });
        Body.Add(_summary);
        SetPeriod(false);
    }

    private void SetPeriod(bool month)
    {
        _monthly = month;
        _week.FontFamily = month ? "InterRegular" : "InterBold";
        _month.FontFamily = month ? "InterBold" : "InterRegular";
        _week.TextColor = month ? Design.Muted : Design.Forest;
        _month.TextColor = month ? Design.Forest : Design.Muted;
        _summary.Clear();
        var now = DateTimeOffset.Now;
        var entries = Journal.Period(Session.Account, now, month);
        var chart = new VerticalStackLayout { Spacing = 12 };
        chart.Add(Design.Text($"{entries.Count} geste{(entries.Count == 1 ? " enregistré" : "s enregistrés")}", 20, "SemiBold"));
        chart.Add(Design.Text(month ? "Ce mois-ci" : "Ces sept derniers jours", 12, color: Design.Muted));
        chart.Add(Chart(entries, now));
        _summary.Add(Design.SurfaceBox(chart, padding: new Thickness(12)));
        if (entries.Count == 0)
        {
            _summary.Add(Design.Text("Chaque petit geste compte.", 20, "SemiBold"));
            _summary.Add(Design.Text("Enregistrez une action pour voir vos progrès.", color: Design.Muted));
            _summary.Add(Action("Choisir un geste", "stats-empty-action", () =>
            { Router.Show("catalogue"); return Task.CompletedTask; }));
        }
        foreach (var gesture in GestureCatalog.All)
        {
            var quantity = entries.Where(e => e.GestureId == gesture.Id).Sum(e => e.Quantity);
            if (quantity == 0) continue;
            _summary.Add(GestureViews.Row(gesture, Journal.Summary(gesture, quantity), true,
                null, () => Router.Show("detail", gesture), RunAsync));
        }
        _summary.Add(Design.Text("Vos catégories", 20, "SemiBold"));
        var categories = new VerticalStackLayout { Spacing = 12 };
        foreach (var category in GestureCatalog.All.GroupBy(g => g.Category))
        {
            var gesture = category.First();
            var card = new VerticalStackLayout { Spacing = 4 };

            card.Add(Design.Text(gesture.Category, 11, "Medium", Design.Forest));
            var ids = category.Select(g => g.Id).ToHashSet();
            var count = entries.Count(e => ids.Contains(e.GestureId));
            card.Add(Design.Text($"{count} geste{(count == 1 ? "" : "s")}", 12, color: Design.Muted));
            var row = Design.Columns(64, GridLength.Star);
            Design.Cell(row, Design.Photo(gesture.Image, 64, 64), 0);
            Design.Cell(row, card, 1);
            categories.Add(row);
        }
        _summary.Add(categories);
        _summary.Add(Design.Notice("Chaque geste compte, à sa manière."));
    }

    private View Chart(IReadOnlyList<JournalEntry> entries, DateTimeOffset now)
    {
        var bins = _monthly ? (DateTime.DaysInMonth(now.Year, now.Month) + 6) / 7 : 7;
        var counts = new int[bins];
        foreach (var entry in entries)
        {
            var bin = _monthly ? (entry.RecordedAt.LocalDateTime.Day - 1) / 7
                : (now.LocalDateTime.Date - entry.RecordedAt.LocalDateTime.Date).Days;
            counts[bin]++;
        }
        var grid = Design.Columns(Enumerable.Repeat(GridLength.Star, bins).ToArray());
        var maximum = Math.Max(1, counts.Max());
        for (var i = 0; i < bins; i++)
        {
            var bar = new VerticalStackLayout { Spacing = 4, VerticalOptions = LayoutOptions.End };
            var count = Design.Text(counts[i].ToString(), 11, color: i == 0 ? Design.Forest : Design.Muted);
            count.HorizontalTextAlignment = TextAlignment.Center;
            bar.Add(count);
            var area = new Grid { HeightRequest = 70 };
            if (counts[i] > 0)
            {
                var shape = Design.SurfaceBox(new ContentView(), 6, new Thickness(0),
                    i == 0 ? Design.Forest : Design.Sage, false);
                shape.HeightRequest = 70d * counts[i] / maximum;
                shape.VerticalOptions = LayoutOptions.End;
                area.Add(shape);
            }
            bar.Add(area);
            var label = Design.Text(_monthly ? $"S{i + 1}" : i == 0 ? "Auj." : $"J-{i}", 11, color: Design.Muted);
            label.HorizontalTextAlignment = TextAlignment.Center;
            bar.Add(label);
            Design.Cell(grid, bar, i);
        }
        return grid;
    }
}
