namespace Novellus.Views
{
    using System;
    using System.Net.Http;
    using System.Text;
    using Newtonsoft.Json;
    using Novellus.Models.DeviceEvents;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient httpClient;

        public MainPage()
        {
            this.httpClient = new HttpClient();
            this.InitializeComponent();
        }

        protected override void OnAppearing()
        {
            this.hybridWebView.RegisterAction(
                "Device_Alert",
                data =>
                {
                    AlertRequest req = JsonConvert.DeserializeObject<AlertRequest>(data);
                    string res = JsonConvert.SerializeObject(new AlertResponse() { UUID = req.UUID });
                    string js = string.Format("window.SetDeviceResult('{0}', '{1}')", req.UUID, res);
                    this.hybridWebView.InjectJavaScriptAsync(js).ConfigureAwait(false);
                    DisplayAlert("Alert", req.Message, "OK");
                });

            this.hybridWebView.RegisterAction(
                "Device_Time",
                data =>
                {
                    TimeRequest req = JsonConvert.DeserializeObject<TimeRequest>(data);
                    string res = JsonConvert.SerializeObject(new TimeResponse() { UUID = req.UUID, Time = DateTime.Now });
                    string js = string.Format("window.SetDeviceResult('{0}', '{1}')", req.UUID, res);
                    this.hybridWebView.InjectJavaScriptAsync(js).ConfigureAwait(false);
                });

            this.hybridWebView.RegisterAction(
                "Device_Fetch",
                async data =>
                {
                    FetchRequest req = JsonConvert.DeserializeObject<FetchRequest>(data);

                    byte[] response = { };
                    try
                    {
                        response = await this.httpClient.GetByteArrayAsync(req.Url);
                    }
                    catch (HttpRequestException ex)
                    {
                        response = Encoding.Unicode.GetBytes(ex.Message);
                    }

                    string content = Convert.ToBase64String(response);
                    string res = JsonConvert.SerializeObject(new FetchResponse() { UUID = req.UUID, Content = content });
                    string js = string.Format("window.SetDeviceResult('{0}', '{1}')", req.UUID, res);
                    await this.hybridWebView.InjectJavaScriptAsync(js);
                });
        }
    }
}
