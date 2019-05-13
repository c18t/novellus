using System;
using Android.Webkit;
using Novellus;

namespace Novellus.Droid
{
    public class JavascriptWebViewClient : WebViewClient
    {
        readonly WeakReference<HybridWebViewRenderer> Reference;

        public JavascriptWebViewClient(HybridWebViewRenderer　renderer)
        {
            Reference = new WeakReference<HybridWebViewRenderer>(renderer); ;
        }

        public async override void OnPageFinished(WebView view, string url)
        {
            if (Reference == null || !Reference.TryGetTarget(out HybridWebViewRenderer renderer)) return;
            if (renderer.Element == null) return;

            // Add Injection Function
            await renderer.OnJavascriptInjectionRequest(HybridWebView.InjectedFunction);
            // Add Callbacks
            foreach (var callback in renderer.Element.RegisteredCallbacks)
            {
                await renderer.OnJavascriptInjectionRequest(HybridWebView.GenerateFunctionScript(callback.Key));
            }

            base.OnPageFinished(view, url);
        }
    }
}
