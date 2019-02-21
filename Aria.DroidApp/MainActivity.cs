using System;
using System.Globalization;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

namespace Aria.DroidApp
{
    [Activity(
        Label = "@string/app_name", 
        Theme = "@style/AppTheme.NoActionBar",
        LaunchMode = LaunchMode.SingleInstance,
        MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Button resetButton;
        private TimePicker timePicker;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            resetButton = FindViewById<Button>(Resource.Id.SaveResetTimeButton);
            timePicker  = FindViewById<TimePicker>(Resource.Id.ResetTimePicker);

            OnNewIntent(Intent);
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
            base.OnNewIntent(intent);

            // Android retains the Intent that was used to create
            // an activity and if the app is minimised and relaunched from history,
            // android starts the activity with the same intent not MAIN_LAUNCHER causing 
            // this intent to be handled multiple times. This means the app will reset 
            // call forwarding every time the app is launced from history if the activity  
            // was started with the call forward reset pending intent.
            var launchedFromHistory  = intent.Flags.HasFlag(ActivityFlags.LaunchedFromHistory);
            var isCallFwdResetIntent = this.IsCallForwardResetIntent(intent);

            if (isCallFwdResetIntent && !launchedFromHistory)
                OnResetCallForward();
        }

        private void OnResetButtonClick(object sender, EventArgs e)
        {
            var testIntent = new Intent(this, typeof(MainActivity));
            testIntent.SetAction(this.GetResetCallForwardingActionName());

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

