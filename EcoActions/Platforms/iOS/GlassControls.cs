using Foundation;
using Microsoft.Maui.Handlers;
using UIKit;

namespace EcoActions.Platforms.iOS;

public static class GlassControls
{
    public static void Apply(IImageButtonHandler handler, IImageButton view)
    {
        if (!OperatingSystem.IsIOSVersionAtLeast(26) ||
            view is not ImageButton { AutomationId: "auth-back" or "auth-info" }) return;
        var configuration = UIButtonConfiguration.ClearGlassButtonConfiguration;
        configuration.CornerStyle = UIButtonConfigurationCornerStyle.Capsule;
        configuration.BaseForegroundColor = UIColor.FromRGB(7, 77, 59);
        handler.PlatformView.Configuration = configuration;
    }

    public static void ApplyAction(IButtonHandler handler, IButton view)
    {
        if (view is not Button button) return;
        var id = button.AutomationId ?? "";
        var filter = id.StartsWith("filter-", StringComparison.Ordinal);
        var compact = filter
            || id.StartsWith("quantity-", StringComparison.Ordinal) || id == "favorite-gesture";
        if (!compact && id is not ("login-submit" or "signup-submit" or "recovery-submit"
            or "detail-record" or "save-reminders" or "join-challenge")) return;
        CoreFoundation.DispatchQueue.MainQueue.DispatchAsync(() => ConfigureAction(handler, button, id, compact, filter));
    }

    private static void ConfigureAction(IButtonHandler handler, Button button, string id, bool compact, bool filter)
    {
        if (!OperatingSystem.IsIOSVersionAtLeast(26) || handler.PlatformView is null) return;
        var selectedAll = id == "filter-all" && button.Text.Contains('✓');
        var configuration = UIButtonConfiguration.GlassButtonConfiguration;
        configuration.CornerStyle = UIButtonConfigurationCornerStyle.Capsule;
        configuration.TitleLineBreakMode = UILineBreakMode.Clip;
        configuration.ButtonSize = filter ? UIButtonConfigurationSize.Mini : UIButtonConfigurationSize.Small;
        var foreground = UIColor.FromRGB(7, 77, 59);
        if (filter) configuration.BaseBackgroundColor = id switch
        {
            "filter-all" when selectedAll => UIColor.FromRGB(207, 231, 214),
            "filter-consumption" => UIColor.FromRGB(229, 242, 255),
            "filter-food" => UIColor.FromRGB(255, 233, 221),
            "filter-energy" => UIColor.FromRGB(255, 245, 204),
            _ => UIColor.FromRGB(227, 242, 228)
        };
        configuration.BaseForegroundColor = foreground;
        configuration.ContentInsets = filter ? new NSDirectionalEdgeInsets(4, 10, 4, 10)
            : new NSDirectionalEdgeInsets(8, 16, 8, 16);
        if (filter)
        {
            var background = configuration.Background;
            background.BackgroundInsets = new NSDirectionalEdgeInsets(6, 0, 6, 0);
            var glass = UIGlassEffect.Create(UIGlassEffectStyle.Regular);
            glass.TintColor = configuration.BaseBackgroundColor;
            background.VisualEffect = glass;
            background.BackgroundColor = configuration.BaseBackgroundColor!.ColorWithAlpha(0.35f);
            configuration.Background = background;
        }
        var attributes = new UIStringAttributes
        {
            Font = UIFont.SystemFontOfSize((nfloat)(filter ? 13 : button.FontSize), UIFontWeight.Medium),
            ForegroundColor = foreground
        };
        configuration.TitleTextAttributesTransformer = _ => attributes.Dictionary;
        configuration.AttributedTitle = new NSAttributedString(button.Text, attributes.Dictionary);
        button.ImageSource = null;
        var symbol = id switch
        {
            "filter-favorites" => "heart",
            "filter-mobility" => "figure.walk",
            "filter-consumption" => "waterbottle",
            "filter-food" => "leaf",
            "filter-energy" => "lightbulb",
            _ => compact ? null : "arrow.right"
        };
        configuration.Image = symbol is null ? null : UIImage.GetSystemImage(symbol);
        configuration.PreferredSymbolConfigurationForImage = UIImageSymbolConfiguration.Create(14, UIImageSymbolWeight.Regular);
        configuration.ImagePlacement = filter ? NSDirectionalRectEdge.Leading : NSDirectionalRectEdge.Trailing;
        configuration.ImagePadding = filter ? 5 : compact ? 0 : 8;
        button.LineBreakMode = LineBreakMode.NoWrap;
        handler.PlatformView.TitleLabel.Lines = 1;
        handler.PlatformView.TitleLabel.LineBreakMode = UILineBreakMode.Clip;
        button.WidthRequest = Math.Ceiling(configuration.AttributedTitle.Size.Width
            + configuration.ContentInsets.Leading + configuration.ContentInsets.Trailing
            + (filter ? (symbol is null ? 24 : 43) : compact ? 16 : 46 + configuration.ImagePadding));
        handler.PlatformView.Configuration = configuration;
    }
}
