using EcoActions.Services;
using EcoActions.UI;

namespace EcoActions;

public sealed class App(AppSession session) : Application
{
    protected override Window CreateWindow(IActivationState? activationState)
    {
        UserAppTheme = AppTheme.Light;
        var loading = new ContentPage { BackgroundColor = Colors.White,
            Content = new ActivityIndicator { IsRunning = true, Color = Design.Forest,
                HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center } };
        var window = new Window(loading);
        var router = new AppRouter(window, session);
        loading.Loaded += async (_, _) => await StartAsync(router);
        return window;
    }

    private async Task StartAsync(AppRouter router)
    {
        try
        {
            await session.OpenAsync();
            if (session.Active is { RemindersEnabled: true } account)
                await ReminderService.ScheduleAsync(account.ReminderTime);
            router.Show(session.Active is null ? "login" : "home");
        }
        catch (Exception error) when (error is IOException or System.Security.Cryptography.CryptographicException
            or System.Text.Json.JsonException or InvalidOperationException)
        {
            System.Diagnostics.Debug.WriteLine(error.GetType().FullName);
            router.ShowStorageError(() => StartAsync(router));
        }
    }
}
