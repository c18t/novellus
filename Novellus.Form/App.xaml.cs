namespace Novellus
{
    using System.Net.Http;
    using Novellus.Views;
    using Xamarin.Forms;

    public partial class App : Application
    {
        public App()
        {
            DependencyService.Register<HttpClient>();

            this.InitializeComponent();

            this.MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
