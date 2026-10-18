#if ANDROID
using Android.Content.Res;
#endif
using Microsoft.Maui.Handlers;

namespace EcoActions;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder().UseMauiApp<App>();
#if IOS
        builder.ConfigureMauiHandlers(handlers => handlers.AddHandler<Platforms.iOS.GlassTimePicker,
            Platforms.iOS.GlassTimePickerHandler>());
#endif
        builder.ConfigureFonts(fonts =>
        {
            fonts.AddFont("Inter-Regular.ttf", "InterRegular");
            fonts.AddFont("Inter-Medium.ttf", "InterMedium");
            fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
            fonts.AddFont("Inter-Bold.ttf", "InterBold");
        });
#if ANDROID
        EntryHandler.Mapper.AppendToMapping("EcoEntry", (handler, _) =>
            handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent));
        ImageHandler.Mapper.AppendToMapping("EcoIcon", (handler, view) =>
        {
            if (view is UI.StateIcon icon)
                handler.PlatformView.ImageTintList = ColorStateList.ValueOf(Android.Graphics.Color.ParseColor(icon.StateColor.ToArgbHex()));
        });
#elif IOS
        ImageButtonHandler.Mapper.AppendToMapping("EcoGlass", Platforms.iOS.GlassControls.Apply);
        ButtonHandler.Mapper.AppendToMapping("EcoGlassAction", Platforms.iOS.GlassControls.ApplyAction);
        ButtonHandler.Mapper.AppendToMapping(nameof(Button.Text), Platforms.iOS.GlassControls.ApplyAction);
        ScrollViewHandler.Mapper.AppendToMapping("EcoScrollBounds", (handler, view) =>
        {
            if (view is ScrollView { Orientation: ScrollOrientation.Vertical })
            {
                handler.PlatformView.Bounces = false;
                handler.PlatformView.AlwaysBounceVertical = false;
            }
        });
        EntryHandler.Mapper.AppendToMapping("EcoEntry", (handler, _) =>
            handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None);
        ImageHandler.Mapper.AppendToMapping("EcoIcon", (handler, view) =>
        {
            if (view is UI.StateIcon icon)
            {
                handler.PlatformView.TintColor = UIKit.UIColor.FromRGBA(
                    (nfloat)icon.StateColor.Red, (nfloat)icon.StateColor.Green,
                    (nfloat)icon.StateColor.Blue, (nfloat)icon.StateColor.Alpha);
                if (handler.PlatformView.Image is { } image)
                    handler.PlatformView.Image = image.ImageWithRenderingMode(UIKit.UIImageRenderingMode.AlwaysTemplate);
            }
        });
#endif
        builder.Services.AddSingleton<Services.AppSession>();
        return builder.Build();
    }
}
