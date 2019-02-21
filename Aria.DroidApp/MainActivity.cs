using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Java.Lang;

namespace Aria.DroidApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Button resetButton;
        private string resetCallForwardAction;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            resetButton = FindViewById<Button>(Resource.Id.SaveResetTimeButton);
            resetCallForwardAction = $"{ApplicationContext.PackageName}.reset-call-fwd";
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
            if (StringComparer.InvariantCultureIgnoreCase.Equals(intent.Action, resetCallForwardAction))
                OnResetCallForward();
        }

        private void OnResetButtonClick(object sender, EventArgs e)
        {
            var testIntent = new Intent(this, typeof(MainActivity));
            testIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.FromBackground);
            testIntent.SetAction(resetCallForwardAction);

            var pendingIntent = PendingIntent.GetActivity(this, default(int), testIntent, PendingIntentFlags.UpdateCurrent);

            var alarmManager = (AlarmManager)GetSystemService(AlarmService);
            alarmManager.SetExact(AlarmType.RtcWakeup, JavaSystem.CurrentTimeMillis() + 5000, pendingIntent);
        }

        private void OnResetCallForward()
        {

        }
    }
}

