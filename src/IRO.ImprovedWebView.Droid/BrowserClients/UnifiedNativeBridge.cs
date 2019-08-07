using System.Diagnostics;
using System.Threading.Tasks;
using Android.Webkit;
using IRO.ImprovedWebView.Core.BindingJs;
using Java.Interop;

namespace IRO.ImprovedWebView.Droid
{
    /// <summary>
    /// Proxy to <see cref="UnifiedLowLevelBridge"/>.
    /// </summary>
    public class UnifiedAndroidBridge : Java.Lang.Object
    {
        readonly UnifiedLowLevelBridge _lowLevelBridge;

        public UnifiedAndroidBridge(IBindingJsSystem bindingJsSystem, AndroidImprovedWebView improvedWebView)
        {
            //Automatically disposed when improvedWebView disposed.
            _lowLevelBridge = new UnifiedLowLevelBridge(bindingJsSystem, improvedWebView);
        }

        [Export]
        [JavascriptInterface]
        public string OnJsCall(
            string jsonParameters
        )
        {
            return _lowLevelBridge.OnJsCall(jsonParameters);
            //Don't now why, but if run in event thread in android - even breakpoints doesn't work.
            //So you can start it in Task if needed.
        }
    }
}