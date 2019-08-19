using System;
using System.Collections.Generic;
using System.Diagnostics;
using Chromely.Core.RestfulService;
using IRO.XWebView.Core.BindingJs;
using IRO.XWebView.Core.BindingJs.LowLevelBridge;
using IRO.XWebView.Core.Exceptions;

namespace IRO.XWebView.CefGlue.Controllers
{
    [ControllerProperty(Name = "TestSyncCallsController", Route = "js_calls")]
    public class JsCallsController : ChromelyController
    {
        public const string RequestBodyStartsWith = "JS2CSHARP_CALL_";

        public const string WebViewIdAndDataSeparator = "_XWEBVIEW_ID_AND_DATA_SEPARATOR_";

        static IDictionary<int, UnifiedLowLevelBridge> _bridges = new Dictionary<int, UnifiedLowLevelBridge>();

        public JsCallsController()
        {
            RegisterPostRequest("/js_calls/call", Call);
        }

        public static void RegisterJsLowLevelBridge(int xWebViewId, UnifiedLowLevelBridge lowLevelBridge)
        {
            if (_bridges.ContainsKey(xWebViewId))
                throw new XWebViewException("Such XWebView id was registered before.");
            if (lowLevelBridge is null)            
                throw new ArgumentNullException(nameof(lowLevelBridge));            
            _bridges.Add(xWebViewId, lowLevelBridge);
        }

        public static void UnregisterJsLowLevelBridge(int xWebViewId)
        {
            _bridges.Remove(xWebViewId);
        }

        public ChromelyResponse Call(ChromelyRequest req)
        {
            try
            {
                var data = (string)req.PostData;
                if (data.StartsWith(RequestBodyStartsWith))
                {
                    data = data.Substring(RequestBodyStartsWith.Length);
                }
                else
                {
                    throw new Exception("Wrong request.");
                }

                var index = data.IndexOf(WebViewIdAndDataSeparator);
                var idString = data.Remove(index);
                var id = Convert.ToInt32(idString);
                var jsonStr = data.Substring(index + WebViewIdAndDataSeparator.Length);

                var lowLevelBridge = _bridges[id];
                var resultStr = lowLevelBridge.OnJsCall(jsonStr);


                return new ChromelyResponse()
                {
                    Status = 200,
                    Data = resultStr
                };
            }
            catch (Exception ex)
            {
                var exStr = $"Exception in {GetType().FullName}, maybe wrong request. Exception '{ex}'.";
                Debug.WriteLine(exStr);
                var res = new ExecutionResult()
                {
                    IsError = true,
                    Result = exStr
                };
                return new ChromelyResponse()
                {
                    Status = 200,
                    Data = res.ToJson()
                };                
            }
        }
    }
}
