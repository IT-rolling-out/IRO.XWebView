using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace IRO.XWebView.Core.BindingJs.LowLevelBridge
{
    /// <summary>
    /// One of the options of low level bridges to js (object that directly connected
    /// to native WebView control). Represent how main methods will look in js.
    /// Just proxy to <see cref="IBindingJsSystem"/>.
    /// </summary>
    public class LowLevelBridge
    {
        IBindingJsSystem _bindingJsSystem;

        IXWebView _XWebView;

        public LowLevelBridge(IBindingJsSystem bindingJsSystem, IXWebView XWebView)
        {
            _bindingJsSystem = bindingJsSystem;
            _XWebView = XWebView;
            _XWebView.Disposed += delegate { Dispose(); };
        }

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
                    _XWebView,
                    jsObjectName,
                    functionName,
                    parametersJson,
                    resolveFunctionName,
                    rejectFunctionName
                );
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"XWebView error: {ex}");
            }
        }

        public string OnJsCallNativeSync(
            string jsObjectName,
            string functionName,
            string parametersJson
        )
        {
            try
            {
                var res = _bindingJsSystem.OnJsCallNativeSync(
                    _XWebView,
                    jsObjectName,
                    functionName,
                    parametersJson
                );
                return res.ToJson();
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"XWebView error: {ex}");
                var res = new ExecutionResult()
                {
                    IsError = true,
                    Result = ex.ToString()
                };
                return res.ToJson();
            }
        }

        public void OnJsPromiseFinished(string taskCompletionSourceId, bool isError, string jsResultJson)
        {
            try
            {
                var executionResult = new ExecutionResult()
                {
                    IsError = isError
                };
                try
                {
                    executionResult.Result = JToken.Parse(jsResultJson);
                }
                catch
                {
                    executionResult.Result = JToken.FromObject(jsResultJson);
                }

                _bindingJsSystem.OnJsPromiseFinished(
                    _XWebView,
                    taskCompletionSourceId,
                    executionResult
                );
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"XWebView error: {ex}");
            }
        }

        public string GetAttachBridgeScript()
        {
            try
            {
                return _bindingJsSystem.GetAttachBridgeScript();
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"XWebView error: {ex}");
                return "console.log('AttachJsBridgeScript error.');";
            }
        }

        public void Dispose()
        {
            _bindingJsSystem = null;
            _XWebView = null;
        }
    }
}