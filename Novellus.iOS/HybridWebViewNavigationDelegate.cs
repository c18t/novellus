using System;
using Foundation;
using WebKit;

namespace Novellus.iOS
{
    public class HybridWebViewNavigationDelegate : WKNavigationDelegate
    {
        readonly WeakReference<HybridWebViewRenderer> Reference;

        public HybridWebViewNavigationDelegate(HybridWebViewRenderer renderer)
        {
            Reference = new WeakReference<HybridWebViewRenderer>(renderer);
        }

        [Export("webView:didFinishNavigation:")]
        public async override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
        {
            if (Reference == null || !Reference.TryGetTarget(out HybridWebViewRenderer renderer)) return;
            if (renderer.Element == null) return;

            await renderer.OnJavascriptInjectionRequest(HybridWebView.InjectedFunction);

            foreach (var function in renderer.Element.RegisteredCallbacks)
            {
                await renderer.OnJavascriptInjectionRequest(HybridWebView.GenerateFunctionScript(function.Key));
            }
        }
    }
}