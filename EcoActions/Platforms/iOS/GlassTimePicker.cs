using Foundation;
using Microsoft.Maui.Handlers;
using UIKit;

namespace EcoActions.Platforms.iOS;

public sealed class GlassTimePicker : View
{
    public static readonly BindableProperty TimeProperty = BindableProperty.Create(
        nameof(Time), typeof(TimeSpan?), typeof(GlassTimePicker), null);

    public TimeSpan? Time
    {
        get => (TimeSpan?)GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }
}

public sealed class GlassTimePickerHandler : ViewHandler<GlassTimePicker, UIVisualEffectView>
{
    private readonly UIDatePicker _picker = new()
    {
        Mode = UIDatePickerMode.Time,
        PreferredDatePickerStyle = UIDatePickerStyle.Compact,
        Locale = NSLocale.FromLocaleIdentifier("fr_FR"),
        BackgroundColor = UIColor.Clear,
        TintColor = UIColor.FromRGB(7, 77, 59),
        TranslatesAutoresizingMaskIntoConstraints = false
    };

    private static readonly IPropertyMapper<GlassTimePicker, GlassTimePickerHandler> Properties =
        new PropertyMapper<GlassTimePicker, GlassTimePickerHandler>(ViewMapper)
        {
            [nameof(GlassTimePicker.Time)] = (handler, _) => handler.UpdateTime()
        };

    public GlassTimePickerHandler() : base(Properties) { }

    protected override UIVisualEffectView CreatePlatformView()
    {
        var effect = OperatingSystem.IsIOSVersionAtLeast(26)
            ? UIGlassEffect.Create(UIGlassEffectStyle.Regular)
            : (UIVisualEffect)UIBlurEffect.FromStyle(UIBlurEffectStyle.SystemThinMaterial);
        var surface = new UIVisualEffectView(effect);
        surface.Layer.CornerRadius = 24;
        surface.ClipsToBounds = true;
        surface.ContentView.AddSubview(_picker);
        NSLayoutConstraint.ActivateConstraints([
            _picker.LeadingAnchor.ConstraintEqualTo(surface.ContentView.LeadingAnchor, 12),
            _picker.TrailingAnchor.ConstraintEqualTo(surface.ContentView.TrailingAnchor, -12),
            _picker.CenterYAnchor.ConstraintEqualTo(surface.ContentView.CenterYAnchor),
            _picker.HeightAnchor.ConstraintEqualTo(36)
        ]);
        return surface;
    }

    protected override void ConnectHandler(UIVisualEffectView platformView)
    {
        base.ConnectHandler(platformView);
        _picker.ValueChanged += TimeChanged;
        UpdateTime();
    }

    protected override void DisconnectHandler(UIVisualEffectView platformView)
    {
        _picker.ValueChanged -= TimeChanged;
        base.DisconnectHandler(platformView);
    }

    private void UpdateTime()
    {
        if (VirtualView.Time is not { } time) return;
        var date = NSCalendar.CurrentCalendar.Components(
            NSCalendarUnit.Year | NSCalendarUnit.Month | NSCalendarUnit.Day, NSDate.Now);
        date.Hour = time.Hours;
        date.Minute = time.Minutes;
        date.Second = 0;
        _picker.SetDate(NSCalendar.CurrentCalendar.DateFromComponents(date), false);
    }

    private void TimeChanged(object? sender, EventArgs args)
    {
        var time = NSCalendar.CurrentCalendar.Components(NSCalendarUnit.Hour | NSCalendarUnit.Minute, _picker.Date);
        VirtualView.Time = new TimeSpan((int)time.Hour, (int)time.Minute, 0);
    }
}
