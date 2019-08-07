using System;
using System.Diagnostics;
using System.Threading.Tasks;
using IRO.XWebView.Core.Exceptions;

namespace IRO.XWebView.Core.BindingJs
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

        public UnifiedLowLevelBridge(IBindingJsSystem bindingJsSystem, IXWebView XWebView)
        {
            _lowLevelBridge = new LowLevelBridge(bindingJsSystem, XWebView);
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

                if (data.MethodName.Trim() == "Dispose")
                {
                    throw new Exception("Dispose method not allowed in OnJsCall");
                }

                //Caching not implemented now.
                var method = _lowLevelBridge
                    .GetType()
                    .GetMethod(data.MethodName);

                if (method == null)
                    throw new Exception($"Can't find method with name {data.MethodName}");
                var parametersInfo = method.GetParameters();
                var parameters = BindingJsSystem.JsonToParams(parametersInfo, data.Parameters);
                var res = (string)method.Invoke(_lowLevelBridge, parameters);
                return res ?? "null";
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"XWebView OnJsCall error: {ex}");
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
    }
}