using Android.Webkit;
using IRO.XWebView.Core.BindingJs;
using IRO.XWebView.Core.BindingJs.LowLevelBridge;
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
        public void OnJsCallNativeAsync(
            string jsObjectName,
            string functionName,
            string parametersJson,
            string resolveFunctionName,
            string rejectFunctionName
        )
        {
            _lowLevelBridge.OnJsCallNativeAsync(
                jsObjectName,
                functionName,
                parametersJson,
                resolveFunctionName,
                rejectFunctionName
            );
        }

        [Export]
        [JavascriptInterface]
        public string OnJsCallNativeSync(
            string jsObjectName,
            string functionName,
            string parametersJson
        )
        {
            return _lowLevelBridge.OnJsCallNativeSync(
                jsObjectName,
                functionName,
                parametersJson
            );
        }

        [Export]
        [JavascriptInterface]
        public void OnJsPromiseFinished(string taskCompletionSourceId, bool isError, string jsResultJson)
        {
            _lowLevelBridge.OnJsPromiseFinished(
                taskCompletionSourceId,
                isError,
                jsResultJson
            );
        }

        [Export]
        [JavascriptInterface]
        public string GetAttachBridgeScript()
        {
            return _lowLevelBridge.GetAttachBridgeScript();
        }
    }
}