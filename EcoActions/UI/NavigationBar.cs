namespace EcoActions.UI;

public sealed class NavigationBar : Border
{
    public NavigationBar(AppRouter router, string active)
    {
        Stroke = Design.Line;
        StrokeThickness = 1;
        BackgroundColor = Colors.White;
        HeightRequest = 66;
        Padding = new Thickness(24, 6);
        var row = Design.Columns(GridLength.Star, GridLength.Star, GridLength.Star, GridLength.Star, GridLength.Star);
        row.ColumnSpacing = 0;
        (string Route, string Label, string Image)[] tabs =
        [ ("home", "Accueil", active == "home" ? "asset_9d881.png" : "asset_07c94.png"),
          ("catalogue", "Gestes", active == "catalogue" ? "asset_df5d3.png" : "asset_a50ed.png"),
          ("challenges", "Défis", "asset_86c15.png"), ("journal", "Journal", "asset_ef18b.png"),
          ("stats", "Bilan", active == "stats" ? "asset_b8f60.png" : "asset_34c15.png") ];
        for (var i = 0; i < tabs.Length; i++)
        {
            var tab = tabs[i];
            var content = new VerticalStackLayout { Spacing = 3, VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center };
            content.Add(new StateIcon(tab.Image, active == tab.Route ? Design.Forest : Design.Muted));
            content.Add(Design.Text(tab.Label, 11, active == tab.Route ? "Bold" : "Medium",
                active == tab.Route ? Design.Forest : Design.Muted));
            var target = new ContentView { Content = content, AutomationId = "tab-" + tab.Route,
                MinimumHeightRequest = 52, MinimumWidthRequest = 44 };
            SemanticProperties.SetDescription(target, tab.Label + (active == tab.Route ? ", sélectionné" : ""));
            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) => router.Show(tab.Route);
            target.GestureRecognizers.Add(tap);
            Design.Cell(row, target, i);
        }
        Content = row;
    }
}
