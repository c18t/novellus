[assembly: Xamarin.Forms.Platform.UWP.ExportRenderer(typeof(Novellus.HybridWebView), typeof(Novellus.UWP.HybridWebViewRenderer))]

namespace Novellus.UWP
{
    using System;
    using System.Threading.Tasks;
    using Novellus;
    using Windows.UI.Xaml.Controls;
    using Windows.Web.Http;
    using Xamarin.Forms.Platform.UWP;

    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, WebView>
    {
        protected override async void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);

            if (this.Control is null)
            {
                var control = new WebView();

                this.SetNativeControl(control);

                HybridWebView.CallbackAdded += this.OnCallbackAdded;
                this.Control.DOMContentLoaded += this.OnDOMContentLoaded;
                this.Control.ScriptNotify += this.OnScriptNotify;
            }

            if (!(e.OldElement is null))
            {
                var hybridWebView = e.OldElement as HybridWebView;
                hybridWebView.OnJavascriptInjectionRequest -= this.OnJavascriptInjectionRequestAsync;
            }

            if (!(e.NewElement is null))
            {
                var hybridWebView = e.NewElement as HybridWebView;
                hybridWebView.OnJavascriptInjectionRequest += this.OnJavascriptInjectionRequestAsync;

                // Navigate
                await WebView.ClearTemporaryWebDataAsync();
                this.Control.NavigateWithHttpRequestMessage(new HttpRequestMessage(HttpMethod.Get, new Uri(hybridWebView.Uri)));
            }
        }

        private async void OnDOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            if (this.Element is null)
            {
                return;
            }

            // Add Injection Function
            await this.Control.InvokeScriptAsync("eval", new[] { HybridWebView.InjectedFunction });

            // Add Callbacks
            foreach (string actionName in this.Element.GetRegisteredActionNames())
            {
                await this.Control.InvokeScriptAsync("eval", new[] { HybridWebView.GenerateFunctionScript(actionName) });
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
                await this.OnJavascriptInjectionRequestAsync(HybridWebView.GenerateFunctionScript(e));
            }
        }

        private void OnScriptNotify(object sender, NotifyEventArgs e)
        {
            if (this.Element is null)
            {
                return;
            }

            this.Element.HandleScriptReceived(e.Value);
        }

        private async Task<string> OnJavascriptInjectionRequestAsync(string js)
        {
            if (this.Control is null)
            {
                return string.Empty;
            }

            var result = await this.Control.InvokeScriptAsync("eval", new[] { js });
            return result;
        }
    }
}
