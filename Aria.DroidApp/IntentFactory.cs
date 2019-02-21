using Android.App;
using Android.Content;

namespace Aria.DroidApp
{
    public static class IntentFactory
    {
        public static PendingIntent CreateWakeOnAlarmIntent(Context context)
        {
            var resetIntent = new Intent(context, typeof(MainActivity));
            resetIntent.SetAction(context.GetResetCallForwardingActionName());

            return PendingIntent.GetActivity(
                context,
                default(int),
                resetIntent,
                PendingIntentFlags.UpdateCurrent);
        }

        public static Intent CreateResetCallForwardingIntent()
        {
            Intent intentCallForward = new Intent(Intent.ActionCall);
            Android.Net.Uri mmiCode = Android.Net.Uri.FromParts("tel", "##21#", "#");
            intentCallForward.SetData(mmiCode);
            return intentCallForward;
        }
    }
}