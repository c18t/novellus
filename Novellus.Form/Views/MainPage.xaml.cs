using Novellus.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Novellus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            this.hybridWebView.RegisterAction("invokeCSharpAction", data => {
                DisplayAlert("Alert", "Hello " + data, "OK");
                string js = "document.getElementById('book').innerHTML='C#!!!';";
                this.hybridWebView.InjectJavascriptAsync(js).ConfigureAwait(false);
            });
        }
    }
}