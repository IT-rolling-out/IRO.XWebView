using Android.Webkit;
using IRO.ImprovedWebView.Core;
using IRO.ImprovedWebView.Core.BindingJs;
using Java.Interop;

namespace IRO.ImprovedWebView.Droid.BrowserClients
{
    class LowLevelJsBridge : Java.Lang.Object
    {
        IBindingJsSystem _bindingJsSystem;

        IImprovedWebView _improvedWebView;

        [Export]
        [JavascriptInterface]
        public async void SendAsync(string messageName, string sendedObjectJson, string resolveFunctionName, string rejectFunctionName)
        {
            //if (_wvw.JsMessagingEnabledOnCurrentPage)
            //    await _jsMessagingSystem.SendJsMessageAsync(messageName, sendedObjectJson, resolveFunctionName, rejectFunctionName);
        }

        [Export]
        [JavascriptInterface]
        public string SendSync(string messageName, string sendedObjectJson)
        {
        //    if (_wvw.JsMessagingEnabledOnCurrentPage)
        //        return _jsMessagingSystem.SendJsMessageSync(messageName, sendedObjectJson);
            return "null";
        }
    }
}