using EcoActions.Core;

namespace EcoActions.UI;

public static class GestureViews
{
    public static View Category(Gesture gesture)
    {
        var row = new HorizontalStackLayout { Spacing = 6 };
        row.Add(Design.Icon(gesture.Icon, 14));
        row.Add(Design.Text(gesture.Category, 11, "Medium", Design.Forest));
        var pill = Design.SurfaceBox(row, 99, new Thickness(9, 6), Color.FromArgb(gesture.CategoryColor), false);
        pill.HorizontalOptions = LayoutOptions.Start;
        return pill;
    }

    public static View Command(Gesture gesture, bool recorded, Func<Task>? add, EcoPageCallback run,
        Func<Task>? remove = null)
    {
        var circle = Design.SurfaceBox(Design.Icon(recorded ? "asset_3558d.png" : "asset_cb601.png", 21),
            99, new Thickness(5), recorded ? Design.Forest : Colors.White);
        circle.Stroke = Design.Forest;
        circle.WidthRequest = circle.HeightRequest = 32;
        circle.HorizontalOptions = circle.VerticalOptions = LayoutOptions.Center;
        var target = new ContentView { Content = circle, WidthRequest = 44, HeightRequest = 44,
            AutomationId = "add-" + gesture.Id };
        SemanticProperties.SetDescription(target, recorded
            ? (remove is null ? "Geste enregistré : " : "Retirer du journal : ") + gesture.Title
            : "Enregistrer : " + gesture.Title);
        if (recorded && remove is null) return target;
        var action = recorded ? remove : add;
        ArgumentNullException.ThrowIfNull(action);
        var tap = new TapGestureRecognizer();
        tap.Tapped += async (_, _) => await run(action);
        target.GestureRecognizers.Add(tap);
        return target;
    }

    public delegate Task EcoPageCallback(Func<Task> action);

    public static View Row(Gesture gesture, string title, bool recorded, Func<Task>? add,
        Action open, EcoPageCallback run, Func<Task>? remove = null)
    {
        var row = Design.Columns(84, GridLength.Star, 44);
        row.ColumnSpacing = 10;
        var photo = Design.Photo(gesture.Image, 86, 84);
        var copy = new VerticalStackLayout { Spacing = 4 };
        copy.Add(Category(gesture));
        copy.Add(Design.Text(title, 15, "SemiBold"));
        copy.Add(Design.Text(gesture.Description, 14, color: Design.Muted));
        var details = new TapGestureRecognizer();
        details.Tapped += (_, _) => open();
        photo.GestureRecognizers.Add(details);
        var copyTap = new TapGestureRecognizer();
        copyTap.Tapped += (_, _) => open();
        copy.GestureRecognizers.Add(copyTap);
        Design.Cell(row, photo, 0);
        Design.Cell(row, copy, 1);
        Design.Cell(row, Command(gesture, recorded, add, run, remove), 2);
        var card = Design.SurfaceBox(row);
        card.AutomationId = "gesture-" + gesture.Id;
        SemanticProperties.SetDescription(photo, "Voir : " + gesture.Title);
        return card;
    }

    public static View Card(Gesture gesture, bool recorded, Func<Task>? add, Action open, EcoPageCallback run)
    {
        var stack = new VerticalStackLayout { Spacing = 6 };
        var photo = Design.Photo(gesture.Image, 92);
        var tap = new TapGestureRecognizer();
        tap.Tapped += (_, _) => open();
        photo.GestureRecognizers.Add(tap);
        SemanticProperties.SetDescription(photo, "Voir : " + gesture.Title);
        stack.Add(photo);
        stack.Add(Category(gesture));
        var title = Design.Text(gesture.ShortTitle, 15, "SemiBold");
        title.MinimumHeightRequest = 20;
        stack.Add(title);
        var lower = Design.Columns(GridLength.Star, 44);
        lower.ColumnSpacing = 4;
        Design.Cell(lower, Design.Text(gesture.Description, 14, color: Design.Muted), 0);
        Design.Cell(lower, Command(gesture, recorded, add, run), 1);
        stack.Add(lower);
        var card = Design.SurfaceBox(stack, 18);
        card.MinimumHeightRequest = 272;
        card.VerticalOptions = LayoutOptions.Start;
        card.Shadow = new Shadow { Brush = Colors.Black, Opacity = 0.06f, Radius = 4, Offset = new(0, 2) };
        card.AutomationId = "gesture-" + gesture.Id;
        return card;
    }

    public static View ThreePhotos(double height = 100)
    {
        var grid = Design.Columns(GridLength.Star, GridLength.Star, GridLength.Star);
        var photos = GestureCatalog.All.Take(3).ToArray();
        for (var i = 0; i < photos.Length; i++) Design.Cell(grid, Design.Photo(photos[i].Image, height), i);
        return grid;
    }
}
