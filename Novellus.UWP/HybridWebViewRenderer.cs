using Novellus;
using Novellus.UWP;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace Novellus.UWP
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, WebView>
    {
        protected override async void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var control = new WebView();

                SetNativeControl(control);

                HybridWebView.CallbackAdded += OnCallbackAdded;
                Control.DOMContentLoaded += OnDOMContentLoaded;
                Control.ScriptNotify += OnScriptNotify;
            }
            if (e.OldElement != null)
            {
                var hybridWebView = e.OldElement as HybridWebView;
                hybridWebView.OnJavascriptInjectionRequest -= OnJavascriptInjectionRequestAsync;
            }
            if (e.NewElement != null)
            {
                var hybridWebView = e.NewElement as HybridWebView;
                hybridWebView.OnJavascriptInjectionRequest += OnJavascriptInjectionRequestAsync;

                // Navigate
                await WebView.ClearTemporaryWebDataAsync();
                Control.NavigateWithHttpRequestMessage(new HttpRequestMessage(HttpMethod.Get, new Uri(hybridWebView.Uri)));
            }
        }

        async void OnDOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            if (Element == null) return;

            // Add Injection Function
            await Control.InvokeScriptAsync("eval", new[] { HybridWebView.InjectedFunction });

            // Add Callbacks
            foreach (var callback in Element.RegisteredCallbacks)
            {
                await Control.InvokeScriptAsync("eval", new[] { HybridWebView.GenerateFunctionScript(callback.Key) });
            }
        }

        async void OnCallbackAdded(object sender, string e)
        {
            if (Element == null || string.IsNullOrWhiteSpace(e)) return;

            if (sender != null)
            {
                await OnJavascriptInjectionRequestAsync(HybridWebView.GenerateFunctionScript(e));
            }
        }

        void OnScriptNotify(object sender, NotifyEventArgs e)
        {
            if (Element == null) return;
            Element.HandleScriptReceived(e.Value);
        }

        async Task<string> OnJavascriptInjectionRequestAsync(string js)
        {
            if (Control == null) return string.Empty;
            var result = await Control.InvokeScriptAsync("eval", new[] { js });
            return result;
        }
    }
}
