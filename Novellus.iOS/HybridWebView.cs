﻿[assembly: Xamarin.Forms.ExportRenderer(typeof(Novellus.HybridWebView), typeof(Novellus.iOS.HybridWebViewRenderer))]

namespace Novellus.iOS
{
    using System;
    using System.Threading.Tasks;
    using Foundation;
    using Novellus;
    using WebKit;
    using Xamarin.Forms.Platform.iOS;

    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, WKWebView>, IWKScriptMessageHandler
    {
        private WKUserContentController userController;
        private HybridWebViewNavigationDelegate navigationDelegate;
        private WKWebViewConfiguration configuration;

        public async Task<string> OnJavascriptInjectionRequest(string js)
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
                this.userController.AddScriptMessageHandler(this, "invokeAction");
                this.navigationDelegate = new HybridWebViewNavigationDelegate(this);
                this.configuration = new WKWebViewConfiguration { UserContentController = this.userController };

                var webView = new WKWebView(this.Frame, this.configuration) { NavigationDelegate = this.navigationDelegate };

                HybridWebView.CallbackAdded += this.OnCallbackAdded;
                this.SetNativeControl(webView);
            }

            if (!(e.OldElement is null))
            {
                this.userController.RemoveAllUserScripts();
                this.userController.RemoveScriptMessageHandler("invokeAction");
                var hybridWebView = e.OldElement as HybridWebView;
                hybridWebView.OnJavascriptInjectionRequest -= this.OnJavascriptInjectionRequest;
                hybridWebView.RemoveAllActions();
            }

            if (!(e.NewElement is null))
            {
                var hybridWebView = e.NewElement as HybridWebView;
                hybridWebView.OnJavascriptInjectionRequest += this.OnJavascriptInjectionRequest;
                this.Control.LoadRequest(new NSUrlRequest(new NSUrl(this.Element.Uri), NSUrlRequestCachePolicy.ReloadIgnoringCacheData, 10));
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