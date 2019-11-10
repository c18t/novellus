namespace Novellus.Droid
{
    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.OS;
    using Android.Runtime;

    [Activity(Label = "小説を開く", Icon = "@mipmap/icon", Theme = "@style/MainTheme")]
    [IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataMimeType = "text/plain")]
    public class OpenNovelActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.StartActivity(new Intent(this, typeof(MainActivity)).PutExtras(this.Intent));
            this.Finish();
        }
    }
}