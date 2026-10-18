using EcoActions.Pages;
using EcoActions.UI;

namespace EcoActions.Platforms.iOS;

public sealed class IosTabsPage : TabbedPage
{
    public IosTabsPage(AppRouter router, string active)
    {
        AutomationId = "main-tabs";
        SelectedTabColor = Design.Forest;
        UnselectedTabColor = Design.Muted;
        (string Route, string Label, string Image, Page Page)[] tabs =
        [
            ("home", "Accueil", "asset_07c94.png", new HomePage(router)),
            ("catalogue", "Gestes", "asset_a50ed.png", new CataloguePage(router)),
            ("challenges", "Défis", "asset_86c15.png", new ChallengesPage(router)),
            ("journal", "Journal", "asset_ef18b.png", new JournalPage(router)),
            ("stats", "Bilan", "asset_34c15.png", new StatsPage(router))
        ];
        foreach (var tab in tabs)
        {
            tab.Page.Title = tab.Label;
            tab.Page.IconImageSource = tab.Image;
            Children.Add(tab.Page);
            if (tab.Route == active) CurrentPage = tab.Page;
        }
    }
}
