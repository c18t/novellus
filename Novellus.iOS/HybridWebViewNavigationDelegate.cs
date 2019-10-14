namespace Novellus.iOS
{
    using System;
    using Foundation;
    using WebKit;

    public class HybridWebViewNavigationDelegate : WKNavigationDelegate
    {
        private readonly WeakReference<HybridWebViewRenderer> reference;

        public HybridWebViewNavigationDelegate(HybridWebViewRenderer renderer)
        {
            this.reference = new WeakReference<HybridWebViewRenderer>(renderer);
        }

        [Export("webView:didFinishNavigation:")]
        public async override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
        {
            if (this.reference is null || !this.reference.TryGetTarget(out HybridWebViewRenderer renderer))
            {
                return;
            }

            if (renderer.Element is null)
            {
                return;
            }

            await renderer.OnJavascriptInjectionRequest(HybridWebView.InjectedFunction);

            foreach (string actionName in renderer.Element.GetRegisteredActionNames())
            {
                await renderer.OnJavascriptInjectionRequest(HybridWebView.GenerateFunctionScript(actionName));
            }
        }
    }
}
