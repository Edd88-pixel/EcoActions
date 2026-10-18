namespace EcoActions.UI;

public sealed class GestureFilters : ScrollView
{
    private readonly Dictionary<string, Button> _buttons = [];
    public string Selected { get; private set; } = "Tous";

    public GestureFilters(Action<string> changed)
    {
        Orientation = ScrollOrientation.Horizontal;
        HorizontalScrollBarVisibility = ScrollBarVisibility.Never;
        HeightRequest = 44;
        var row = new HorizontalStackLayout { Spacing = 8 };
        foreach (var (label, id) in new[]
        {
            ("Tous", "all"), ("Favoris", "favorites"), ("Mobilité", "mobility"),
            ("Consommation", "consumption"), ("Alimentation", "food"), ("Énergie", "energy")
        })
        {
            var button = Design.Link(label, "filter-" + id, (_, _) =>
            {
                Select(label);
                changed(label);
            }, "");
            _buttons.Add(label, button);
            row.Add(button);
        }
        Content = row;
        Select("Tous");
    }

    public void Select(string selected)
    {
        Selected = selected;
        foreach (var (label, button) in _buttons)
        {
            button.Text = label == selected ? label + " ✓" : label;
            SemanticProperties.SetDescription(button, label == selected ? label + ", sélectionné" : label);
        }
    }
}
