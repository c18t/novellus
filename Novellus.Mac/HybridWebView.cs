using System;
using System.IO;
using System.Threading.Tasks;
using Foundation;
using Novellus;
using Novellus.Mac;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.MacOS;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace Novellus.Mac
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, WKWebView>, IWKScriptMessageHandler
    {
        WKUserContentController userController;
        HybridWebViewNavigationDelegate navigationDelegate;
        WKWebViewConfiguration configuration;

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                userController = new WKUserContentController();
                userController.AddScriptMessageHandler(this, "invokeAction");
                navigationDelegate = new HybridWebViewNavigationDelegate(this);
                configuration = new WKWebViewConfiguration { UserContentController = userController };

                var webView = new WKWebView(Frame, configuration) { NavigationDelegate = navigationDelegate };

                HybridWebView.CallbackAdded += OnCallbackAdded;
                SetNativeControl(webView);
            }
            if (e.OldElement != null)
            {
                userController.RemoveAllUserScripts();
                userController.RemoveScriptMessageHandler("invokeAction");
                var hybridWebView = e.OldElement as HybridWebView;
                hybridWebView.OnJavascriptInjectionRequest -= OnJavascriptInjectionRequest;
                hybridWebView.RemoveAllActions();
            }
            if (e.NewElement != null)
            {
                //WKWebsiteDataStore.DefaultDataStore.RemoveDataOfTypes(WKWebsiteDataStore.AllWebsiteDataTypes, new NSDate(), () => { });
                var hybridWebView = e.NewElement as HybridWebView;
                hybridWebView.OnJavascriptInjectionRequest += OnJavascriptInjectionRequest;
                Control.LoadRequest(new NSUrlRequest(new NSUrl(Element.Uri),
                                                     NSUrlRequestCachePolicy.ReloadIgnoringCacheData, 10));
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

        public async Task<string> OnJavascriptInjectionRequest(string js)
        {
            if (Control == null || Element == null) return string.Empty;

            var response = string.Empty;

            try
            {
                var obj = await Control.EvaluateJavaScriptAsync(js).ConfigureAwait(true);
                if (obj != null)
                {
                    response = obj.ToString();
                }

                //var script = new WKUserScript(new NSString(js), WKUserScriptInjectionTime.AtDocumentStart, false);
                //var hasScript = false;
                //foreach (var item in userController.UserScripts)
                //{
                //    if (item.Source == script.Source)
                //    {
                //        hasScript = true; break;
                //    }
                //}
                //if (!hasScript)
                //{
                //    userController.AddUserScript(script);
                //    configuration.UserContentController = userController;
                //}
            }

            catch (Exception) { /* The Webview might not be ready... */ }
            return response;
        }

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            if (Element == null || message == null || message.Body == null) return;
            Element.HandleScriptReceived(message.Body.ToString());
        }
    }
}