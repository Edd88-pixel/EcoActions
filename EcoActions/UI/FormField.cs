namespace EcoActions.UI;

public sealed class FormField : VerticalStackLayout
{
    public Entry Input { get; }
    public string Value => Input.Text ?? "";
    public string Field { get; }
    private readonly Border _box;
    private readonly Label _error = Design.Text("", 12, color: Color.FromArgb("#A84040"));

    public FormField(string label, string id, bool password = false, Keyboard? keyboard = null)
    {
        Spacing = 6;
        Field = id.Split('-')[^1];
        Add(Design.Text(label, 11, "Medium"));
        Input = new Entry
        {
            AutomationId = id,
            IsPassword = password,
            Keyboard = keyboard ?? Keyboard.Default,
            FontFamily = "InterRegular",
            FontSize = 14,
            TextColor = Design.Ink,
            BackgroundColor = Colors.Transparent,
            ClearButtonVisibility = password ? ClearButtonVisibility.Never : ClearButtonVisibility.WhileEditing,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center
        };
        SemanticProperties.SetHint(Input, label);
        _box = Design.SurfaceBox(Input, 10, new Thickness(12, 0));
        _box.HeightRequest = 46;
        Add(_box);
        _error.IsVisible = false;
        Add(_error);
        Input.TextChanged += (_, _) => ClearError();
    }

    public void ShowError(string message)
    {
        _box.Stroke = Color.FromArgb("#A84040");
        _error.Text = message;
        _error.IsVisible = true;
    }

    public void ClearError()
    {
        _box.Stroke = Design.Line;
        _error.IsVisible = false;
    }
}
