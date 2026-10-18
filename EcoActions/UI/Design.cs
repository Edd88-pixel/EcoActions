using Microsoft.Maui.Controls.Shapes;

namespace EcoActions.UI;

public static class Design
{
    public static Color Forest => Color.FromArgb("#074D3B");
    public static Color Ink => Color.FromArgb("#071610");
    public static Color Muted => Color.FromArgb("#59656B");
    public static Color Line => Color.FromArgb("#E7EAE8");
    public static Color Surface => Color.FromArgb("#F7F8F7");
    public static Color Sage => Color.FromArgb("#E3F2E4");

    public static Label Text(string text, double size = 14, string weight = "Regular", Color? color = null)
        => new() { Text = text, FontFamily = "Inter" + weight, FontSize = size,
            TextColor = color ?? Ink, LineHeight = 1.35, LineBreakMode = LineBreakMode.WordWrap };

    public static Border SurfaceBox(View content, double radius = 14, Thickness? padding = null,
        Color? fill = null, bool outline = true) => new()
    {
        StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(radius) },
        Stroke = outline ? Line : Colors.Transparent, StrokeThickness = outline ? 1 : 0,
        BackgroundColor = fill ?? Colors.White, Padding = padding ?? new Thickness(8), Content = content
    };

    public static Border Photo(string image, double height, double? width = null, double radius = 10)
    {
        var photo = new Image { Source = image, Aspect = Aspect.AspectFill, HeightRequest = height };
        var box = SurfaceBox(photo, radius, new Thickness(0), outline: false);
        box.HeightRequest = height;
        if (width is { } value) box.WidthRequest = value;
        return box;
    }

    public static Image Icon(string image, double size = 20)
        => new() { Source = image, WidthRequest = size, HeightRequest = size, Aspect = Aspect.AspectFit };

    public static Button Link(string text, string id, EventHandler clicked, string arrow = "asset_00350.png")
    {
        var button = new Button { Text = text, AutomationId = id, ImageSource = arrow,
            ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Right, 8),
            TextColor = Forest, BackgroundColor = Colors.Transparent, BorderWidth = 0, Padding = 0,
            FontFamily = "InterRegular", FontSize = 14, HeightRequest = 44,
            HorizontalOptions = LayoutOptions.Start, TextTransform = TextTransform.None };
        SemanticProperties.SetDescription(button, text);
        button.Clicked += clicked;
        return button;
    }

    public static Grid Columns(params GridLength[] columns)
    {
        var grid = new Grid { ColumnSpacing = 8 };
        foreach (var width in columns) grid.ColumnDefinitions.Add(new ColumnDefinition { Width = width });
        return grid;
    }

    public static void Cell(Grid grid, View child, int column)
    {
        grid.Add(child);
        Grid.SetColumn(child, column);
    }

    public static View Title(string title, string subtitle)
    {
        var stack = new VerticalStackLayout { Spacing = 4 };
        stack.Add(Text(title, 28, "Bold"));
        stack.Add(Text(subtitle, 14, color: Muted));
        return stack;
    }

    public static View Notice(string text, bool success = false)
    {
        var row = Columns(18, GridLength.Star);
        Cell(row, Icon("asset_d2dcf.png", 18), 0);
        Cell(row, Text(text, 12, color: Muted), 1);
        return SurfaceBox(row, 10, new Thickness(10), success ? Sage : Surface, false);
    }
}
