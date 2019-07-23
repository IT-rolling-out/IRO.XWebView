using System.Diagnostics;
using Android.Webkit;
using IRO.ImprovedWebView.Core;
using IRO.ImprovedWebView.Core.BindingJs;
using Java.Interop;
using Java.Lang;
using Newtonsoft.Json.Linq;

namespace IRO.ImprovedWebView.Droid.BrowserClients
{
    class NativeBridge : Java.Lang.Object
    {
        readonly IBindingJsSystem _bindingJsSystem;

        readonly AndroidImprovedWebView _improvedWebView;

        public NativeBridge(IBindingJsSystem bindingJsSystem, AndroidImprovedWebView improvedWebView)
        {
            _bindingJsSystem = bindingJsSystem;
            _improvedWebView = improvedWebView;
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
            try
            {
                _bindingJsSystem.OnJsCallNativeAsync(
                    _improvedWebView,
                    jsObjectName,
                    functionName,
                    parametersJson,
                    resolveFunctionName,
                    rejectFunctionName
                    );
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"ImprovedWebView error: {ex}");
            }
        }

        [Export]
        [JavascriptInterface]
        public string OnJsCallNativeSync(
            string jsObjectName,
            string functionName,
            string parametersJson
            )
        {
            try
            {
                var res = _bindingJsSystem.OnJsCallNativeSync(
                    _improvedWebView,
                    jsObjectName,
                    functionName,
                    parametersJson
                    );
                return res.ToJson();
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"ImprovedWebView error: {ex}");
                var res = new ExecutionResult()
                {
                    IsError = true,
                    Result = ex.ToString()
                };
                return res.ToJson();
            }
        }

        [Export]
        [JavascriptInterface]
        public void OnJsPromiseFinished(string taskCompletionSourceId, bool isError, string jsResultJson)
        {
            try
            {
                var executionResult = new ExecutionResult()
                {
                    IsError = isError,
                    Result = JToken.Parse(jsResultJson)
                };
                _bindingJsSystem.OnJsPromiseFinished(
                    _improvedWebView,
                    taskCompletionSourceId,
                    executionResult
                    );
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"ImprovedWebView error: {ex}");
            }
        }

        [Export]
        [JavascriptInterface]
        public string GetAttachBridgeScript()
        {
            try
            {
                return _bindingJsSystem.GetAttachBridgeScript();
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"ImprovedWebView error: {ex}");
                return "console.log('AttachJsBridgeScript error.');";
            }
        }
    }
}