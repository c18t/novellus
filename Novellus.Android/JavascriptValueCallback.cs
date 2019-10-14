namespace Novellus.Droid
{
    using System;
    using Android.Webkit;

    public class JavascriptValueCallback : Java.Lang.Object, IValueCallback
    {
        private readonly WeakReference<HybridWebViewRenderer> reference;

        public JavascriptValueCallback(HybridWebViewRenderer renderer)
        {
            this.reference = new WeakReference<HybridWebViewRenderer>(renderer);
        }

        public Java.Lang.Object Value { get; private set; }

        public void OnReceiveValue(Java.Lang.Object value)
        {
            if (this.reference is null || !this.reference.TryGetTarget(out HybridWebViewRenderer renderer))
            {
                return;
            }

            this.Value = value;
        }

        public void Reset()
        {
            this.Value = null;
        }
    }
}
