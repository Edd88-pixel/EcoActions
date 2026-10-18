using Foundation;
using UserNotifications;

namespace EcoActions.Services;

public static partial class ReminderService
{
    private const string ReminderId = "ecoactions-daily";

    public static partial async Task<bool> RequestPermissionAsync()
    {
        var center = UNUserNotificationCenter.Current;
        var settings = await center.GetNotificationSettingsAsync();
        if (settings.AuthorizationStatus == UNAuthorizationStatus.NotDetermined)
        {
            var result = await center.RequestAuthorizationAsync(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound);
            if (result.Item2 is { } error) throw new IOException("Les notifications n’ont pas pu être activées.", new NSErrorException(error));
            return result.Item1;
        }
        return settings.AuthorizationStatus is UNAuthorizationStatus.Authorized or UNAuthorizationStatus.Provisional;
    }

    public static partial async Task ScheduleAsync(TimeSpan time)
    {
        var content = new UNMutableNotificationContent
        {
            Title = "Un petit geste aujourd’hui ?",
            Body = "À votre rythme, un geste à la fois.",
            Sound = UNNotificationSound.Default
        };
        var date = new NSDateComponents { Hour = time.Hours, Minute = time.Minutes };
        var trigger = UNCalendarNotificationTrigger.CreateTrigger(date, true);
        var request = UNNotificationRequest.FromIdentifier(ReminderId, content, trigger);
        await UNUserNotificationCenter.Current.AddNotificationRequestAsync(request);
    }

    public static partial void Cancel()
    {
        UNUserNotificationCenter.Current.RemovePendingNotificationRequests([ReminderId]);
        UNUserNotificationCenter.Current.RemoveDeliveredNotifications([ReminderId]);
    }
}
