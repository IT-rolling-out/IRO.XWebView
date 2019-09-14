using System;
using System.Threading.Tasks;
using Chromely.CefGlue.Browser;
using Chromely.Core.Host;
using IRO.XWebView.CefGlue.Controllers;
using IRO.XWebView.CefGlue.Extensions;
using IRO.XWebView.Core;
using IRO.XWebView.Core.BindingJs.LowLevelBridge;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Events;
using IRO.XWebView.Core.Exceptions;

namespace IRO.XWebView.CefGlue
{
    public class CefGlueXWebView : BaseXWebView
    {
        readonly UnifiedLowLevelBridge _bridge;

        public CefGlueBrowser CefGlueBrowser { get; private set; }

        public IChromelyWindow Window { get; private set; }

        /// <summary>
        /// If true - you can change visibility after creation.
        /// If false - <see cref="P:IRO.XWebView.Core.IXWebView.Visibility" /> assignment will throw exception.
        /// </summary>
        public override bool CanSetVisibility { get; } = true;

        public override string BrowserName => nameof(CefGlueXWebView);

        public CefGlueXWebView(IChromelyWindow window)
        {
            Window = window ?? throw new ArgumentNullException(nameof(window));
            CefGlueBrowser = (CefGlueBrowser)window.Browser;
            if (CefGlueBrowser == null)
            {
                throw new XWebViewException("Window browser is null.");
            }

            //Register native bridge.
            _bridge = new UnifiedLowLevelBridge(this.BindingJsSystem, this);
            JsCallsController.RegisterJsLowLevelBridge(Id, _bridge);

            // ReSharper disable once VirtualMemberCallInConstructor
            RegisterEvents();

            CefGlueBrowser.RenderProcessTerminated += delegate
            {
                try
                {
                    Dispose();
                }
                catch { }
            };
            SetInitialized();
        }

        #region Js.
        /// <summary>
        /// Execute your script in browser without any manipulations.
        /// Doesn't support promises.
        /// </summary>
        public override Task<string> UnmanagedExecuteJavascriptWithResult(string script, int? timeoutMS = null)
        {
            //!This what you must do if your browser doesn't support executing js with result.
            throw new XWebViewException(
                $"{BrowserName} doesn't support {nameof(UnmanagedExecuteJavascriptWithResult)}" +
                $"because of CefGlue limitations. Use {nameof(ExJs)}, NOTE: in {BrowserName}" +
                $"this method will always use callbacks.");
        }

        public override void UnmanagedExecuteJavascriptAsync(string script, int? timeoutMS = null)
        {
            CefGlueBrowser.CefBrowser.GetMainFrame().ExecuteJavaScript(script, "", 0);
        }

        public override async Task<TResult> ExJs<TResult>(string script, bool promiseResultSupport = true, int? timeoutMS = null)
        {
            //!This what you must do if your browser doesn't support executing js with result.
            var jsObjName = Core.BindingJs.BindingJsSystem.JsBridgeObjectName;
            var simpleCallback = Extensions.CefGlueBrowserExtensions.SimpleCallbackFuncName;

            //!Check if js is work.
            var isJsWorks = await CheckIfJsWorksOnPage();
            if (!isJsWorks)
            {
                //!Execute test javascript to checkout if it executed. If not - throw exception.
                //It make debug really simpler.
                throw new XWebViewException("Javascript can't be executed at the moment. Maybe page not loaded.");
            }


            //!Check if bridge attached, because ExJs can't work without it.
            var checkBridgeAttached = $@"if(window.{jsObjName}){{{simpleCallback}('ATTACHED');}}else{{{simpleCallback}('NOTATTACHED');}}";
            //Used 'simple callback' based on console.log().
            var isAttached = await CefGlueBrowser.ExecuteJavascriptSimpleCallback(checkBridgeAttached, 1000) == "ATTACHED";

            if (!isAttached)
            {
                //!Here we attaching bridge if it is not.
                await AttachBridge();
            }

            //!Here we specify ExJs to always use callbacks.
            return await base.ExJs<TResult>(script, true, timeoutMS);
        }

        async Task<bool> CheckIfJsWorksOnPage(int timeoutMS = 500)
        {
            var simpleCallback = Extensions.CefGlueBrowserExtensions.SimpleCallbackFuncName;
            var isJsWorks = "";
            try
            {
                isJsWorks = await CefGlueBrowser.ExecuteJavascriptSimpleCallback($"(function(){{{simpleCallback}('true');}})();", timeoutMS);
            }
            catch { }
            return isJsWorks == "true";
        }
        #endregion

        public override async Task WaitWhileNavigating()
        {
            await WaitJsCanBeExecuted();
            await base.WaitWhileNavigating();
        }

        public async Task WaitJsCanBeExecuted(int timeoutMS = 5000)
        {
            //Really sorry for this code. Now i hate this CefGlue.
            //Here we wait when there can be executed js (when page loaded).
            while (timeoutMS >= 0)
            {
                var jsCheckTimeout = 200;
                if (await CheckIfJsWorksOnPage(jsCheckTimeout))
                {
                    break;
                }
                timeoutMS -= jsCheckTimeout;
            }

            if (timeoutMS<=0)
            {
                throw new XWebViewException("Waiting while js ca be executed canceled cause of timeout. " +
                                            "Js can't be executed. Maybe page not loaded.");
            }
        }

        public override void Stop()
        {
            CefGlueBrowser.CefBrowser.StopLoad();
        }

        public override void ClearCookies()
        {
            throw new NotImplementedException();
        }

        protected override void StartLoading(string url)
        {
            CefGlueBrowser.CefBrowser.GetMainFrame().LoadUrl(url);
        }

        protected override void StartLoadingHtml(string data, string baseUrl)
        {
            CefGlueBrowser.CefBrowser.GetMainFrame().LoadString(data, baseUrl);
        }

        protected override void SetVisibilityState(XWebViewVisibility visibility)
        {
        }

        protected override XWebViewVisibility GetVisibilityState()
        {
            return XWebViewVisibility.Visible;
        }

        public override bool CanGoForward()
        {
            return CefGlueBrowser.CefBrowser.CanGoForward;
        }

        public override bool CanGoBack()
        {
            return CefGlueBrowser.CefBrowser.CanGoBack;
        }

        /// <summary>
        /// Return <see cref="CefGlueBrowser"/>.
        /// </summary>
        /// <returns></returns>
        public override object Native()
        {
            return CefGlueBrowser;
        }

        public override void Dispose()
        {
            try
            {
                CefGlueBrowser.Dispose();
                CefGlueBrowser = null;
                Window.Close();
                Window.Dispose();
                Window = null;
            }
            catch { }
            try
            {
                JsCallsController.UnregisterJsLowLevelBridge(Id);
            }
            catch { }

            base.Dispose();
        }

        #region Js bridge.
        public override async Task AttachBridge()
        {
            await base.AttachBridge();
            //Registering unified OnJsCall to add callbacks  support. Can't find better solution for CefGlue.
            var jsObjName = Core.BindingJs.BindingJsSystem.JsBridgeObjectName;
            var callbackSupportScript = $@"
  window.{jsObjName} = window.{jsObjName} || {{}};
  window.{jsObjName}.{nameof(UnifiedLowLevelBridge.OnJsCall)} = function(jsonStr){{
    var http = new XMLHttpRequest();
    var url = 'http://chromely.com/js_calls/call';
    var canBeAsync = jsonStr.indexOf('{nameof(LowLevelBridge.OnJsCallNativeAsync)}') !== -1;
    if(!canBeAsync){{ 
      var canBeAsync = jsonStr.indexOf('{nameof(LowLevelBridge.OnJsPromiseFinished)}') !== -1; 
    }}
    http.open('POST', url, canBeAsync);    
    var reqBody = '{JsCallsController.RequestBodyStartsWith}' + '{Id}' + '{JsCallsController.WebViewIdAndDataSeparator}' + jsonStr;
    http.send(reqBody);
    return http.responseText;
  }}
";
            UnmanagedExecuteJavascriptAsync(callbackSupportScript);
        }
        #endregion

        protected virtual void RegisterEvents()
        {
            string newAddress = "about:blank";
            CefGlueBrowser.AddressChanged += (s, a) =>
            {
                if (!a.Frame.IsMain)
                    return;
                newAddress = a.Address;
            };
            CefGlueBrowser.FrameLoadStart += (s, a) =>
            {
                if (!a.Frame.IsMain)
                    return;
                var args = new LoadStartedEventArgs()
                {
                    Url = newAddress
                };
                OnLoadStarted(args);
                //!Load cancel not implemented.
            };
            CefGlueBrowser.FrameLoadEnd += (s, a) =>
            {
                if (!a.Frame.IsMain)
                    return;
                var args = new LoadFinishedEventArgs()
                {
                    Url = a.Frame.Url
                };
                OnLoadFinished(args);
            };
            CefGlueBrowser.LoadError += (s, a) =>
            {
                if (!a.Frame.IsMain)
                    return;
                var args = new LoadFinishedEventArgs()
                {
                    Url = a.FailedUrl,
                    ErrorType = a.ErrorCode.ToString(),
                    ErrorDescription = a.ErrorText,
                    IsError = true
                };
                OnLoadFinished(args);
            };
            CefGlueBrowser.RenderProcessTerminated += delegate
            {
                Dispose();
            };
        }
    }
}
