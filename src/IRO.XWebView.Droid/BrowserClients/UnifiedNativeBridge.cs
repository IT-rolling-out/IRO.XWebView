using System.Threading.Tasks;
using Android.Webkit;
using IRO.XWebView.Core.BindingJs;
using IRO.XWebView.Core.BindingJs.LowLevelBridge;
using Java.Interop;
using Java.Lang;

namespace IRO.XWebView.Droid.BrowserClients
{
    /// <summary>
    /// Proxy to <see cref="UnifiedLowLevelBridge"/>.
    /// Just example of how it simple to add support of all js callbacks.
    /// Not used now, because have faster analog.
    /// </summary>
    public class UnifiedAndroidBridge : Object
    {
        readonly UnifiedLowLevelBridge _lowLevelBridge;

        public UnifiedAndroidBridge(IBindingJsSystem bindingJsSystem, AndroidXWebView XWebView)
        {
            //Automatically disposed when XWebView disposed.
            _lowLevelBridge = new UnifiedLowLevelBridge(bindingJsSystem, XWebView);
        }

        [Export]
        [JavascriptInterface]
        public string OnJsCall(
            string jsonParameters
        )
        {
            //Don't know why, but breakpoints doesn't work when they called in current event thread.
            var t = Task.Run(() => { return _lowLevelBridge.OnJsCall(jsonParameters); });
            t.Wait();
            return t.Result;
        }
    }
}