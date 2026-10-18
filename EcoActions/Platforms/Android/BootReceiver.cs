using Android.App;
using Android.Content;

namespace EcoActions.Platforms.Android;

[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter([Intent.ActionBootCompleted, Intent.ActionTimeChanged, Intent.ActionTimezoneChanged])]
public sealed class BootReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (intent?.Action is not (Intent.ActionBootCompleted or Intent.ActionTimeChanged or Intent.ActionTimezoneChanged)) return;
        var minute = Preferences.Get("reminder-minute", -1);
        if (minute >= 0 && minute < 1440) ReminderService.Schedule(TimeSpan.FromMinutes(minute));
    }
}
