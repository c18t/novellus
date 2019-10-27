[assembly: Xamarin.Forms.ExportRenderer(typeof(Novellus.HybridWebView), typeof(Novellus.Mac.HybridWebViewRenderer))]

namespace Novellus.Mac
{
    using System;
    using System.Threading.Tasks;
    using Foundation;
    using Novellus;
    using WebKit;
    using Xamarin.Forms.Platform.MacOS;

    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, WKWebView>, IWKScriptMessageHandler
    {
        private WKUserContentController userController;
        private HybridWebViewNavigationDelegate navigationDelegate;
        private WKWebViewConfiguration configuration;

        public async Task<string> OnJavaScriptInjectionRequest(string js)
        {
            if (this.Control is null || this.Element is null)
            {
                return string.Empty;
            }

            string response = string.Empty;
            try
            {
                NSObject obj = await this.Control.EvaluateJavaScriptAsync(js).ConfigureAwait(true);
                if (!(obj is null))
                {
                    response = obj.ToString();
                }
            }
            catch
            {
                // do nothing
            }

            return response;
        }

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            if (this.Element is null || message is null || message.Body is null)
            {
                return;
            }

            this.Element.HandleScriptReceived(message.Body.ToString());
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);

            if (this.Control is null)
            {
                this.userController = new WKUserContentController();
                this.userController.AddScriptMessageHandler(this, HybridWebView.JavaScriptMessageHandlerName);
                this.navigationDelegate = new HybridWebViewNavigationDelegate(this);
                this.configuration = new WKWebViewConfiguration { UserContentController = this.userController };

                var webView = new WKWebView(this.Frame, this.configuration) { NavigationDelegate = this.navigationDelegate };

                HybridWebView.CallbackAdded += this.OnCallbackAdded;
                this.SetNativeControl(webView);
            }

            if (!(e.OldElement is null))
            {
                this.userController.RemoveAllUserScripts();
                this.userController.RemoveScriptMessageHandler(HybridWebView.JavaScriptMessageHandlerName);
                var hybridWebView = e.OldElement as HybridWebView;
                hybridWebView.OnJavaScriptInjectionRequest -= this.OnJavaScriptInjectionRequest;
                hybridWebView.RemoveAllActions();
            }

            if (!(e.NewElement is null))
            {
                WKWebsiteDataStore.DefaultDataStore.RemoveDataOfTypes(
                    WKWebsiteDataStore.AllWebsiteDataTypes,
                    new NSDate(),
                    () => { });
                var hybridWebView = e.NewElement as HybridWebView;
                hybridWebView.OnJavaScriptInjectionRequest += this.OnJavaScriptInjectionRequest;
                this.Control.LoadRequest(new NSUrlRequest(new NSUrl(Element.Uri), NSUrlRequestCachePolicy.ReloadIgnoringCacheData, 10));
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
                await this.OnJavaScriptInjectionRequest(HybridWebView.GenerateFunctionScript(e));
            }
        }
    }
}
