using EcoActions.Core;
using EcoActions.UI;

namespace EcoActions.Pages;

public sealed class CataloguePage : EcoPage
{
    private readonly VerticalStackLayout _results = new() { Spacing = 12 };
    private readonly Entry _search = new()
    {
        AutomationId = "search-gestures",
        Placeholder = "Rechercher un geste",
        FontFamily = "InterRegular",
        FontSize = 16,
        PlaceholderColor = Design.Muted,
        TextColor = Design.Ink,
        HeightRequest = 44,
        VerticalOptions = LayoutOptions.Center,
        ClearButtonVisibility = ClearButtonVisibility.WhileEditing
    };
    private readonly Label _heading = Design.Text("À explorer", 20, "SemiBold");
    private readonly Label _count = Design.Text("", 12, color: Design.Forest);
    private readonly GestureFilters _filters;
    private string? _category;

    public CataloguePage(AppRouter router) : base(router, "catalogue", "catalogue", "home")
    {
        Header();
        Body.Add(Design.Title("Les petits gestes.", "Choisissez selon votre quotidien."));
        var searchRow = Design.Columns(20, GridLength.Star);
        var searchIcon = Design.Icon("asset_db97d.png");
        searchIcon.VerticalOptions = LayoutOptions.Center;
        Design.Cell(searchRow, searchIcon, 0);
        Design.Cell(searchRow, _search, 1);
        SemanticProperties.SetDescription(_search, "Rechercher un geste");
        var searchSurface = Design.SurfaceBox(searchRow, 14, new Thickness(16, 0), Design.Surface);
        searchSurface.HeightRequest = 52;
        Body.Add(searchSurface);
        _filters = new GestureFilters(selected =>
        {
            _category = selected is "Tous" or "Favoris" ? null : selected;
            Refresh();
        });
        Body.Add(_filters);
        var heading = Design.Columns(GridLength.Star, GridLength.Auto);
        Design.Cell(heading, _heading, 0);
        Design.Cell(heading, _count, 1);
        Body.Add(heading);
        Body.Add(_results);
        _search.TextChanged += (_, _) => Refresh();
        Refresh();
    }

    private void Refresh()
    {
        _results.Clear();
        var gestures = GestureCatalog.Search(_search.Text ?? "", _category)
            .Where(g => _filters.Selected != "Favoris" || Session.Account.FavoriteGestureIds.Contains(g.Id)).ToArray();
        _heading.Text = gestures.Length == 0 ? "Aucun résultat" : _filters.Selected == "Favoris" ? "Vos favoris" : _category ?? "À explorer";
        _count.Text = $"{gestures.Length} geste{(gestures.Length == 1 ? "" : "s")}";
        if (gestures.Length == 0)
        {
            _results.Add(Design.Text(_filters.Selected == "Favoris" ? "Ajoutez un geste à vos favoris depuis sa fiche pour le retrouver ici."
                : "Essayez un autre mot ou revenez à tous les gestes.", color: Design.Muted));
            _results.Add(Action("Effacer", "clear-search", () =>
            { _category = null; _filters.Select("Tous"); _search.Text = ""; Refresh(); return Task.CompletedTask; }));
        }
        foreach (var gesture in gestures)
            _results.Add(GestureViews.Row(gesture, gesture.Title, false, async () =>
            { await Session.AddAsync(gesture, gesture.DefaultQuantity); Router.Show("journal"); },
                () => Router.Show("detail", gesture), RunAsync));
    }
}
