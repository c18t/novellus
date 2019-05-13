using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.Content;
using Novellus;
using Novellus.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace Novellus.Droid
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, Android.Webkit.WebView>
    {
        JavascriptValueCallback _callback;
        Context _context;

        public HybridWebViewRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var webView = new Android.Webkit.WebView(_context);
                _callback = new JavascriptValueCallback(this);
                
                webView.Settings.JavaScriptEnabled = true;
                webView.AddJavascriptInterface(new JSBridge(this), "jsBridge");
                webView.SetWebViewClient(new JavascriptWebViewClient(this));

                HybridWebView.CallbackAdded += OnCallbackAdded;

                SetNativeControl(webView);
            }
            if (e.OldElement != null)
            {
                Control.RemoveJavascriptInterface("jsBridge");
                var hybridWebView = e.OldElement as HybridWebView;
                hybridWebView.RemoveAllActions();
                hybridWebView.OnJavascriptInjectionRequest -= OnJavascriptInjectionRequest;
            }
            if (e.NewElement != null)
            {
                Control.AddJavascriptInterface(new JSBridge(this), "jsBridge");
                var hybridWebView = e.NewElement as HybridWebView;
                hybridWebView.OnJavascriptInjectionRequest += OnJavascriptInjectionRequest;
                Control.LoadUrl(Element.Uri);
            }
        }

        async void OnCallbackAdded(object sender, string e)
        {
            if (Element == null || string.IsNullOrWhiteSpace(e)) return;

            if (sender != null)
            {
                await OnJavascriptInjectionRequest(HybridWebView.GenerateFunctionScript(e));
            }
        }

        internal async Task<string> OnJavascriptInjectionRequest(string js)
        {
            if (Element == null || Control == null) return string.Empty;

            // fire!
            _callback.Reset();

            var response = string.Empty;
            Device.BeginInvokeOnMainThread(() => Control.EvaluateJavascript(js, _callback));

            // wait!
            await Task.Run(() =>
            {
                while (_callback.Value == null) { }

                // Get the string and strip off the quotes
                if (_callback.Value is Java.Lang.String)
                {
                    // Unescape that damn Unicode Java bull.
                    response = Regex.Replace(_callback.Value.ToString(), @"\\[Uu]([0-9A-Fa-f]{4})", m => char.ToString((char)ushort.Parse(m.Groups[1].Value, NumberStyles.AllowHexSpecifier)));
                    response = Regex.Unescape(response);

                    if (response.Equals("\"null\""))
                        response = null;

                    else if (response.StartsWith("\"") && response.EndsWith("\""))
                    {
                        response = response.Substring(1, response.Length - 2);
                    }
                }

            });

            // return
            return response;
        }
    }
}