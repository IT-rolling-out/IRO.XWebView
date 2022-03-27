using Android.Webkit;
using IRO.XWebView.Core.BindingJs;
using Java.Interop;
using Java.Lang;

namespace IRO.XWebView.Droid.BrowserClients
{
    /// <summary>
    /// Proxy to <see cref="LowLevelBridge"/>.
    /// </summary>
    public class AndroidBridge : Object
    {
        readonly LowLevelBridge _lowLevelBridge;

        public AndroidBridge(IBindingJsSystem bindingJsSystem, AndroidXWebView XWebView)
        {
            //Automatically disposed when XWebView disposed.
            _lowLevelBridge = new LowLevelBridge(bindingJsSystem, XWebView);
        }

        [Export]
        [JavascriptInterface]
        public void OJC(
            string jsObjectName,
            string functionName,
            string parametersJson,
            string resolveFunctionName,
            string rejectFunctionName
        )
        {
            _lowLevelBridge.OJC(
                jsObjectName,
                functionName,
                parametersJson,
                resolveFunctionName,
                rejectFunctionName
            );
        }

        [Export]
        [JavascriptInterface]
        public void OJPF(string taskCompletionSourceId, bool isError, string jsResultJson)
        {
            _lowLevelBridge.OJPF(
                taskCompletionSourceId,
                isError,
                jsResultJson
            );
        }
    }
}