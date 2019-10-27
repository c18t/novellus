namespace Novellus.Droid
{
    using System;
    using Android.Webkit;
    using Novellus;

    public class JavaScriptWebViewClient : WebViewClient
    {
        private readonly WeakReference<HybridWebViewRenderer> reference;

        public JavaScriptWebViewClient(HybridWebViewRenderer renderer)
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
            await renderer.OnJavaScriptInjectionRequest(HybridWebView.InvokerFunctionScript);

            // Add Callbacks
            foreach (string actionName in renderer.Element.GetRegisteredActionNames())
            {
                await renderer.OnJavaScriptInjectionRequest(HybridWebView.GenerateFunctionScript(actionName));
            }

            base.OnPageFinished(view, url);
        }
    }
}
