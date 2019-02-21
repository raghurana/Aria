using System;
using Android.Content;
using Android.Widget;

namespace Aria.DroidApp
{
    public static class Extensions
    {
        public static DateTime ToFutureDateTime(this TimePicker androidTimePicker)
        {
            var newDateLocal = DateTime.Today.AddHours(androidTimePicker.Hour).AddMinutes(androidTimePicker.Minute);

            if (newDateLocal <= DateTime.Now)
                newDateLocal = newDateLocal.AddDays(1);

            return newDateLocal;
        }

        public static long ToEpochMilliseconds(this DateTime utcDateTime)
        {
            if(utcDateTime.Kind != DateTimeKind.Utc)
                throw new ArgumentException($"{nameof(utcDateTime)} must be of kind utc.");

            if(utcDateTime < DateTime.UtcNow)
                throw new ArgumentException($"{nameof(utcDateTime)} must be in the future.");

            var utcEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((utcDateTime - utcEpoch).TotalMilliseconds);
        }

        public static string GetResetCallForwardingActionName(this Context context)
        {
            return $"{context.PackageName}.reset-call-fwd";
        }

        public static bool IsCallForwardResetIntent(this Context context, Intent intentToCheck)
        {
            var intentAction       = intentToCheck.Action;
            var resetCallFwdAction = context.GetResetCallForwardingActionName();

            return StringComparer.InvariantCultureIgnoreCase.Equals(intentAction, resetCallFwdAction);
        }
    }
}