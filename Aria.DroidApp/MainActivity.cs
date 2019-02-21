using System;
using System.Globalization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

namespace Aria.DroidApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Button resetButton;
        private TimePicker timePicker;
        private string resetCallFwdAction;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            resetButton = FindViewById<Button>(Resource.Id.SaveResetTimeButton);
            timePicker  = FindViewById<TimePicker>(Resource.Id.ResetTimePicker);

            resetCallFwdAction = $"{ApplicationContext.PackageName}.reset-call-fwd";
        }

        protected override void OnResume()
        {
            resetButton.Click += OnResetButtonClick;
            base.OnResume();
        }

        protected override void OnPause()
        {
            resetButton.Click -= OnResetButtonClick;
            base.OnPause();
        }

        protected override void OnNewIntent(Intent intent)
        {
            if (StringComparer.InvariantCultureIgnoreCase.Equals(intent.Action, resetCallFwdAction))
                OnResetCallForward();
        }

        private void OnResetButtonClick(object sender, EventArgs e)
        {
            var testIntent = new Intent(this, typeof(MainActivity));
            testIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.FromBackground);
            testIntent.SetAction(resetCallFwdAction);

            var pendingIntent = PendingIntent.GetActivity(this, default(int), testIntent, PendingIntentFlags.UpdateCurrent);

            var alarmDate = timePicker.ToFutureDateTime();
            ShowToast(alarmDate.ToString("ddd dd MMM hh:mm tt", CultureInfo.CurrentUICulture));

            var alarmManager = (AlarmManager)GetSystemService(AlarmService);
            alarmManager.SetExact(AlarmType.RtcWakeup, alarmDate.ToUniversalTime().ToEpochMilliseconds(), pendingIntent);
        }

        private void OnResetCallForward()
        {
            ShowToast("Alarm Invoked");
        }

        private void ShowToast(string message)
        {
            Toast
                .MakeText(this, message, ToastLength.Long)
                .Show();
        }
    }
}

