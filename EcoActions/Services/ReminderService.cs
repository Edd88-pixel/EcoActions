namespace EcoActions.Services;

public static partial class ReminderService
{
    public static partial Task<bool> RequestPermissionAsync();
    public static partial Task ScheduleAsync(TimeSpan time);
    public static partial void Cancel();
}
