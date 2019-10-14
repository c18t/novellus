namespace Novellus.Droid
{
    using System;
    using Android.Webkit;
    using Novellus;

    public class JavascriptWebViewClient : WebViewClient
    {
        private readonly WeakReference<HybridWebViewRenderer> reference;

        public JavascriptWebViewClient(HybridWebViewRenderer　renderer)
        {
            this.reference = new WeakReference<HybridWebViewRenderer>(renderer);
        }

        public async override void OnPageFinished(WebView view, string url)
        {
            if (this.reference is null || !this.reference.TryGetTarget(out HybridWebViewRenderer renderer))
            {
                return;
            }

            if (renderer.Element is null)
            {
                return;
            }

            // Add Injection Function
            await renderer.OnJavascriptInjectionRequest(HybridWebView.InjectedFunction);

            // Add Callbacks
            foreach (string actionName in renderer.Element.GetRegisteredActionNames())
            {
                await renderer.OnJavascriptInjectionRequest(HybridWebView.GenerateFunctionScript(actionName));
            }

            base.OnPageFinished(view, url);
        }
    }
}
