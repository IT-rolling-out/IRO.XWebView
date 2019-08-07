using System;
using System.Diagnostics;
using IRO.ImprovedWebView.Core.Exceptions;

namespace IRO.ImprovedWebView.Core.BindingJs
{
    /// <summary>
    /// Another the options of low level bridges to js (object that directly connected
    /// to native WebView control). Represent how main methods will look in js.
    /// Just proxy to <see cref="IBindingJsSystem"/>.
    /// <para></para>
    /// Use it when you want add support of all <see cref="IBindingJsSystem"/> features, but you can pass all data only through one string parameter
    /// and one method. Faster to implement, but lower performance.
    /// </summary>
    public class UnifiedLowLevelBridge
    {
        LowLevelBridge _lowLevelBridge;

        public UnifiedLowLevelBridge(IBindingJsSystem bindingJsSystem, IImprovedWebView improvedWebView)
        {
            _lowLevelBridge = new LowLevelBridge(bindingJsSystem, improvedWebView);
        }

        /// <summary>
        /// Call methods of <see cref="LowLevelBridge"/> via one method.
        /// Just register it in your WebView and get all callbacks support.
        /// <para></para>
        /// Not to fast, but unified.
        /// </summary>
        public string OnJsCall(
            string jsonParameters
        )
        {
            try
            {
                var data = UnifiedJsCallData.FromJson(jsonParameters);
                var res = OnJsCall(data);
                if (res != null)
                    return res.ToJson();
                return "null";
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

        public void Dispose()
        {
            _lowLevelBridge?.Dispose();
            _lowLevelBridge = null;
        }

        ExecutionResult OnJsCall(
            UnifiedJsCallData data
        )
        {
            try
            {
                if (data == null) throw new ArgumentNullException(nameof(data));
                //Caching not implemented now.
                var method = _lowLevelBridge
                    .GetType()
                    .GetMethod(data.MethodName);
                if (method == null)
                    throw new Exception($"Can't find method with name {data.MethodName}");
                var parametersInfo = method.GetParameters();
                var parameters = BindingJsSystem.JsonToParams(parametersInfo, data.Parameters);
                var res = method.Invoke(this, parameters);
                return (ExecutionResult)res;
            }
            catch (Exception ex)
            {
                throw new ImprovedWebViewException($"OnJsCall exception {ex} .");
            }
        }
    }
}