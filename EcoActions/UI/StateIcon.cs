namespace EcoActions.UI;

public sealed class StateIcon : Image
{
    public Color StateColor { get; }

    public StateIcon(string source, Color color, double size = 23)
    {
        Source = source;
        StateColor = color;
        WidthRequest = HeightRequest = size;
        Aspect = Aspect.AspectFit;
    }
}
