using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace IRO.ImprovedWebView.Core.BindingJs
{
    /// <summary>
    /// One of the options of low level bridges to js (object that directly connected
    /// to native WebView control). Represent how main methods will look in js.
    /// Just proxy to <see cref="IBindingJsSystem"/>.
    /// </summary>
    public class LowLevelBridge
    {
        IBindingJsSystem _bindingJsSystem;

        IImprovedWebView _improvedWebView;

        public LowLevelBridge(IBindingJsSystem bindingJsSystem, IImprovedWebView improvedWebView)
        {
            _bindingJsSystem = bindingJsSystem;
            _improvedWebView = improvedWebView;
            _improvedWebView.Disposed += delegate { Dispose(); };
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

        public void Dispose()
        {
            _bindingJsSystem = null;
            _improvedWebView = null;
        }
    }
}