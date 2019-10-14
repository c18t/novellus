namespace Novellus.Droid
{
    using System;
    using Android.Webkit;
    using Java.Interop;

    public class JSBridge : Java.Lang.Object
    {
        private readonly WeakReference<HybridWebViewRenderer> hybridWebViewRenderer;

        public JSBridge(HybridWebViewRenderer hybridRenderer)
        {
            this.hybridWebViewRenderer = new WeakReference<HybridWebViewRenderer>(hybridRenderer);
        }

        [JavascriptInterface]
        [Export("invokeAction")]
        public void InvokeAction(string data)
        {
            if (!(this.hybridWebViewRenderer is null) && this.hybridWebViewRenderer.TryGetTarget(out HybridWebViewRenderer hybridRenderer))
            {
                hybridRenderer.Element.HandleScriptReceived(data);
            }
        }
    }
}
