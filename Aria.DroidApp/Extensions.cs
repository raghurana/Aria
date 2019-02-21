using System;
using System.Globalization;
using Android.Content;
using Android.Widget;

namespace Aria.DroidApp
{
    public static class Extensions
    {
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
        
        public static long ToEpochMilliseconds(this DateTime utcDateTime)
        {
            if (utcDateTime.Kind != DateTimeKind.Utc)
                throw new ArgumentException($"{nameof(utcDateTime)} must be of kind utc.");

            if (utcDateTime < DateTime.UtcNow)
                throw new ArgumentException($"{nameof(utcDateTime)} must be in the future.");

            var utcEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((utcDateTime - utcEpoch).TotalMilliseconds);
        }

        public static string FormatDateTime(this DateTime date)
        {
            return date.ToString("ddd dd MMM hh:mm tt", CultureInfo.CurrentUICulture);
        }

        public static string GetTimeRemaining(this DateTime date)
        {
            var timeDiff = date.Subtract(DateTime.Now);

            if (timeDiff.Hours <= 0 && timeDiff.Minutes <= 0)
                return "Less than a minute";

            if (timeDiff.Hours <= 0 && timeDiff.Minutes > 0)
                return $"{timeDiff.Minutes} Minutes";

            return $"{timeDiff.Hours} Hours, {timeDiff.Minutes} Minutes";
        }

        public static DateTime ToFutureDateTime(this TimePicker timePicker)
        {
            timePicker.ClearFocus();

            var newDateLocal = 
                DateTime
                    .Today
                    .AddHours(timePicker.Hour)
                    .AddMinutes(timePicker.Minute);

            if (newDateLocal <= DateTime.Now)
                newDateLocal = newDateLocal.AddDays(1);

            return newDateLocal;
        }

    }
}