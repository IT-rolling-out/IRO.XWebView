using System.Diagnostics;
using System.Threading.Tasks;
using Android.Webkit;
using IRO.ImprovedWebView.Core.BindingJs;
using Java.Interop;

namespace IRO.ImprovedWebView.Droid
{
    /// <summary>
    /// Proxy to <see cref="UnifiedLowLevelBridge"/>.
    /// Just example of how it simple to add support of all js callbacks.
    /// Not used now, because have faster analog.
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
            //Don't know why, but breakpoints doesn't work when they called in current event thread.
            var t = Task.Run(() =>
            {
                return _lowLevelBridge.OnJsCall(jsonParameters);
            });
            t.Wait();
            return t.Result;
        }
    }
}