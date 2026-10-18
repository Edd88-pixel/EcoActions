using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace EcoActions.Platforms.Android;

public static class ReminderService
{
    public static async Task<bool> RequestPermissionAsync()
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(33)) return true;
        return await Permissions.RequestAsync<Permissions.PostNotifications>() == PermissionStatus.Granted;
    }

    public static void Schedule(TimeSpan time)
    {
        var context = global::Android.App.Application.Context;
        var intent = new Intent(context, typeof(ReminderReceiver));
        intent.PutExtra("hour", time.Hours);
        intent.PutExtra("minute", time.Minutes);
        var pending = PendingIntent.GetBroadcast(context, 1, intent,
            PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable)!;
        var alarm = (AlarmManager)context.GetSystemService(Context.AlarmService)!;
        var next = DateTime.Today.Add(time);
        if (next <= DateTime.Now) next = next.AddDays(1);
        alarm.SetAndAllowWhileIdle(AlarmType.RtcWakeup, new DateTimeOffset(next).ToUnixTimeMilliseconds(), pending);
        Preferences.Set("reminder-minute", time.Hours * 60 + time.Minutes);
    }

    public static void Cancel()
    {
        var context = global::Android.App.Application.Context;
        var pending = PendingIntent.GetBroadcast(context, 1, new Intent(context, typeof(ReminderReceiver)),
            PendingIntentFlags.NoCreate | PendingIntentFlags.Immutable);
        if (pending is not null)
            ((AlarmManager)context.GetSystemService(Context.AlarmService)!).Cancel(pending);
        Preferences.Remove("reminder-minute");
    }

}

[BroadcastReceiver(Enabled = true, Exported = false)]
public sealed class ReminderReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context is null || intent is null) return;
        if (OperatingSystem.IsAndroidVersionAtLeast(33) &&
            context.CheckSelfPermission(Manifest.Permission.PostNotifications) != Permission.Granted) return;
        var manager = (NotificationManager)context.GetSystemService(Context.NotificationService)!;
        manager.CreateNotificationChannel(new NotificationChannel("ecoactions", "Petits rappels", NotificationImportance.Default));
        var open = PendingIntent.GetActivity(context, 0, new Intent(context, typeof(MainActivity)), PendingIntentFlags.Immutable);
        var notification = new Notification.Builder(context, "ecoactions")
            .SetContentTitle("Un petit geste aujourd’hui ?")
            .SetContentText("À votre rythme, un geste à la fois.")
            .SetSmallIcon(Resource.Drawable.asset_c8051)
            .SetContentIntent(open).SetAutoCancel(true).Build();
        manager.Notify(1, notification);
        if (intent.HasExtra("hour") && intent.HasExtra("minute"))
            ReminderService.Schedule(new TimeSpan(intent.GetIntExtra("hour", -1), intent.GetIntExtra("minute", -1), 0));
    }
}
