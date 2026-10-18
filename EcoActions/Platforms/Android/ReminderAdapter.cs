namespace EcoActions.Services;

public static partial class ReminderService
{
    public static partial Task<bool> RequestPermissionAsync()
        => Platforms.Android.ReminderService.RequestPermissionAsync();

    public static partial Task ScheduleAsync(TimeSpan time)
    {
        Platforms.Android.ReminderService.Schedule(time);
        return Task.CompletedTask;
    }

    public static partial void Cancel() => Platforms.Android.ReminderService.Cancel();
}
