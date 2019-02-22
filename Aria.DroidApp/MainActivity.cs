using System;
using System.Linq;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Aria.DroidApp
{
    [Activity(
        Label = "@string/app_name", 
        Theme = "@style/AppTheme.NoActionBar",
        LaunchMode = LaunchMode.SingleInstance,
        MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private const string CallPhonePermission = Manifest.Permission.CallPhone;
        private const int CallPhonePermissionRequestCode = 123;

        private Button resetButton;
        private TimePicker timePicker;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AppCenter.Start("7d917ae7-5b08-427f-8ed7-5e6e1e6e20d1", typeof(Analytics), typeof(Crashes));

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
                OnCallForwardingResetAlarm();
        }

        private void OnResetButtonClick(object sender, EventArgs e)
        {
            if (CheckSelfPermission(CallPhonePermission) == Permission.Granted)
                ScheduleCallForwardingResetAlarm();
            else
                RequestPermissions(new[] {CallPhonePermission}, CallPhonePermissionRequestCode);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            var requestCodeMatched     = requestCode == CallPhonePermissionRequestCode;
            var permissionNameMatched  = permissions.Any() && permissions.First() == CallPhonePermission;
            var permissionGrantMatched = grantResults.Any() && grantResults.First() == Permission.Granted;
            
            if(requestCodeMatched && permissionNameMatched && permissionGrantMatched)
                ScheduleCallForwardingResetAlarm();
            else
                ShowToast("Permission to manage calls denied or failed. Contact the App developer, if this issue persists.");
        }

        private void ScheduleCallForwardingResetAlarm()
        {
            var alarmDate    = timePicker.ToFutureDateTime();
            var alarmManager = (AlarmManager)GetSystemService(AlarmService);

            alarmManager
                .SetExact(
                    AlarmType.RtcWakeup,
                    alarmDate.ToUniversalTime().ToEpochMilliseconds(),
                    IntentFactory.CreateWakeOnAlarmIntent(this));

            ShowToast($"Scheduled for {alarmDate.FormatDateTime()}");
            ShowToast($"{alarmDate.GetTimeRemaining()} remaining.");
        }

        private void OnCallForwardingResetAlarm()
        {
            StartActivity(IntentFactory.CreateResetCallForwardingIntent());
        }

        private void ShowToast(string message)
        {
            Toast
                .MakeText(this, message, ToastLength.Long)
                .Show();
        }
    }
}

