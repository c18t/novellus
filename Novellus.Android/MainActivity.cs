namespace Novellus.Droid
{
    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.Content.Res;
    using Android.Graphics;
    using Android.OS;
    using Android.Runtime;
    using Android.Views;
    using Firebase.Analytics;
    using Java.Lang;
    using Xamarin.Forms.Platform.Android;

    [Activity(Label = "Novellus", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleTask)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private App app;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity.TabLayoutResource = Resource.Layout.Tabbar;

            global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity.ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            this.LoadApplication(this.app = new App());

            // TODO: 起動時にURL共有を受けた際の処理。画面のロードが完了してからOpenUrlする
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            this.Intent = intent;
            Views.MainPage mainPage = this.app.MainPage as Views.MainPage;
            string url = this.Intent?.Extras?.Get(Intent.ExtraText)?.ToString();
            if (!(mainPage is null) && !(url is null))
            {
                mainPage.OpenUrl(url);
            }
        }

        private void ProcessIntent()
        {
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
        }
        protected void OnCreate(ActivationOptions options)
        {
            base.OnCreate(options);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
        protected override void OnPause()
        {
            base.OnPause();
        }
        protected override void OnRestart()
        {
            base.OnRestart();
        }
        protected override void OnResume()
        {
            base.OnResume();
        }
        protected override void OnStart()
        {
            base.OnStart();
        }
        protected override void OnStop()
        {
            base.OnStop();
        }

        protected void OnChildTitleChanged(Activity childActivity, string title)
        {
            base.OnChildTitleChanged(childActivity, title);
        }
        protected override void OnChildTitleChanged(Activity childActivity, ICharSequence title)
        {
            base.OnChildTitleChanged(childActivity, title);
        }
        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
        }
        protected override void OnPostResume()
        {
            base.OnPostResume();
        }
        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
        }
        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
        }
        protected void OnTitleChanged(string title, Color color)
        {
            ;
        }
        protected override void OnTitleChanged(ICharSequence title, Color color)
        {
            base.OnTitleChanged(title, color);
        }
        protected override void OnUserLeaveHint()
        {
            base.OnUserLeaveHint();
        }
    }
}
