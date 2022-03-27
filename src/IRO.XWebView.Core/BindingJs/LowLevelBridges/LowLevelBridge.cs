using System;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace IRO.XWebView.Core.BindingJs.LowLevelBridges
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
            _XWebView.Disposing += delegate { Dispose(); };
        }

        public void OnJsCall(
            string jsObjectName,
            string functionName,
            string parametersJson,
            string resolveFunctionName,
            string rejectFunctionName
        )
        {
            try
            {
                _bindingJsSystem.OnJsCall(
                    _XWebView,
                    jsObjectName,
                    functionName,
                    parametersJson,
                    resolveFunctionName,
                    rejectFunctionName
                );
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"XWebView error: {ex}");
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
            catch (Exception ex)
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
            catch (Exception ex)
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