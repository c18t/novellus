[assembly: Xamarin.Forms.ExportRenderer(typeof(Novellus.HybridWebView), typeof(Novellus.Droid.HybridWebViewRenderer))]

namespace Novellus.Droid
{
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Android.Content;
    using Xamarin.Forms;
    using Xamarin.Forms.Platform.Android;

    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, Android.Webkit.WebView>
    {
        private JavascriptValueCallback callback;
        private Context context;

        public HybridWebViewRenderer(Context context) : base(context)
        {
            this.context = context;
        }

        internal async Task<string> OnJavascriptInjectionRequest(string js)
        {
            if (this.Element is null || this.Control is null)
            {
                return string.Empty;
            }

            // fire!
            this.callback.Reset();

            string response = string.Empty;
            Device.BeginInvokeOnMainThread(() => this.Control.EvaluateJavascript(js, this.callback));

            // wait!
            await Task.Run(() =>
            {
                while (this.callback.Value is null)
                {
                    // continue
                }

                // Get the string and strip off the quotes
                if (this.callback.Value is Java.Lang.String)
                {
                    // Unescape that damn Unicode Java bull.
                    response = Regex.Replace(this.callback.Value.ToString(), @"\\[Uu]([0-9A-Fa-f]{4})", m => char.ToString((char)ushort.Parse(m.Groups[1].Value, NumberStyles.AllowHexSpecifier)));
                    response = Regex.Unescape(response);

                    if (response.Equals("\"null\""))
                    {
                        response = null;
                    }
                    else if (response.StartsWith("\"") && response.EndsWith("\""))
                    {
                        response = response.Substring(1, response.Length - 2);
                    }
                }
            });

            // return
            return response;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);

            if (this.Control is null)
            {
                var webView = new Android.Webkit.WebView(this.context);
                this.callback = new JavascriptValueCallback(this);

                webView.Settings.JavaScriptEnabled = true;
                webView.AddJavascriptInterface(new JSBridge(this), "jsBridge");
                webView.SetWebViewClient(new JavascriptWebViewClient(this));

                HybridWebView.CallbackAdded += this.OnCallbackAdded;

                this.SetNativeControl(webView);
            }

            if (!(e.OldElement is null))
            {
                this.Control.RemoveJavascriptInterface("jsBridge");
                var hybridWebView = e.OldElement as HybridWebView;
                hybridWebView.RemoveAllActions();
                hybridWebView.OnJavascriptInjectionRequest -= this.OnJavascriptInjectionRequest;
            }

            if (!(e.NewElement is null))
            {
                this.Control.AddJavascriptInterface(new JSBridge(this), "jsBridge");
                var hybridWebView = e.NewElement as HybridWebView;
                hybridWebView.OnJavascriptInjectionRequest += this.OnJavascriptInjectionRequest;
                this.Control.LoadUrl(Element.Uri);
            }
        }

        private async void OnCallbackAdded(object sender, string e)
        {
            if (this.Element is null || string.IsNullOrWhiteSpace(e))
            {
                return;
            }

            if (!(sender is null))
            {
                await this.OnJavascriptInjectionRequest(HybridWebView.GenerateFunctionScript(e));
            }
        }
    }
}