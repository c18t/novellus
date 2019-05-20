using Newtonsoft.Json;
using Novellus.Models.DeviceEvents;
using Novellus.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
            this.hybridWebView.RegisterAction("Device_Alert", data => {
                AlertRequest req = JsonConvert.DeserializeObject<AlertRequest>(data);
                string res = JsonConvert.SerializeObject(new AlertResponse() { UUID = req.UUID });
                var js = string.Format("window.SetDeviceResult('{0}', '{1}')", req.UUID, res);
                this.hybridWebView.InjectJavascriptAsync(js).ConfigureAwait(false);
                DisplayAlert("Alert", req.Message, "OK");
            });

            this.hybridWebView.RegisterAction("Device_Time", data => {
                TimeRequest req = JsonConvert.DeserializeObject<TimeRequest>(data);
                string res = JsonConvert.SerializeObject(new TimeResponse() { UUID = req.UUID, Time = DateTime.Now });
                var js = string.Format("window.SetDeviceResult('{0}', '{1}')", req.UUID, res);
                this.hybridWebView.InjectJavascriptAsync(js).ConfigureAwait(false);
            });

            this.hybridWebView.RegisterAction("Device_Fetch", async data => {
                FetchRequest req = JsonConvert.DeserializeObject<FetchRequest>(data);

                string content = string.Empty;
                using (var client = new HttpClient())
                {
                    content = Convert.ToBase64String(await client.GetByteArrayAsync(req.Url));
                }
                string res = JsonConvert.SerializeObject(new FetchResponse() { UUID = req.UUID, Content = content });
                var js = string.Format("window.SetDeviceResult('{0}', '{1}')", req.UUID, res);
                await this.hybridWebView.InjectJavascriptAsync(js);
            });
        }
    }
}
