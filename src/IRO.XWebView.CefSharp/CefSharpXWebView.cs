using System;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;
using IRO.XWebView.CefSharp.BrowserClients;
using C = CefSharp;
using IRO.XWebView.Core;
using IRO.XWebView.Core.BindingJs.LowLevelBridge;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Events;
using IRO.XWebView.Core.Exceptions;
using IRO.XWebView.Droid.Containers;

namespace IRO.XWebView.CefSharp
{
    public class CefSharpXWebView : BaseXWebView
    {
        readonly LowLevelBridge _bridge;

        readonly ICefSharpContainer _container;

        public ChromiumWebBrowser Browser { get; }


        /// <summary>
        /// If true - you can change visibility after creation.
        /// If false - <see cref="P:IRO.XWebView.Core.IXWebView.Visibility" /> assignment will throw exception.
        /// </summary>
        public override bool CanSetVisibility { get; } = true;

        public override string BrowserName => nameof(CefSharpXWebView);

        public CefGlueXWebView(ICefSharpContainer container, CustomRequestHandler customRequestHandler = null)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            Browser = _container.CurrentBrowser;
            if (Browser == null)
            {
                throw new XWebViewException("Browser is null.");
            }

            Browser.RequestHandler = customRequestHandler ?? new CustomRequestHandler();

            //Register native bridge.
            _bridge = new LowLevelBridge(this.BindingJsSystem, this);
            Browser.RegisterJsObject(
                Core.BindingJs.BindingJsSystem.JsBridgeObjectName,
                this
                );

            // ReSharper disable once VirtualMemberCallInConstructor
            RegisterEvents();
            InitCefSharpSpecial();
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

        public override async Task<TResult> ExJs<TResult>(string script, bool promiseResultSupport = false, int? timeoutMS = null)
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

        public override async Task WaitWhileBusy()
        {
            await WaitJsCanBeExecuted();
            await base.WaitWhileBusy();
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

            if (timeoutMS <= 0)
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
            base.StartLoadingHtml(data, baseUrl);
            CefGlueBrowser.CefBrowser.GetMainFrame().LoadString(data, baseUrl);
        }

        protected override void ToggleVisibilityState(XWebViewVisibility visibility)
        {
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
            if (Browser.RequestHandler is CustomRequestHandler reqHandler)
            {
                //Cancel implemented if used cutom CustomRequestHandler.
                reqHandler.BeforeBrowse += (chromiumWebBrowser, browser, frame, request, userGesture, isRedirect) =>
                {
                    if (frame.IsMain && !isRedirect)
                    {
                        var loadStartArgs = new LoadStartedEventArgs()
                        {
                            Url = request.Url
                        };
                        OnLoadStarted(loadStartArgs);
                        return !loadStartArgs.Cancel;
                    }
                    return true;
                };
            }
            else
            {
                //!Load cancel not implemented.
                string newAddress = "about:blank";
                Browser.AddressChanged += (s, a) =>
                {
                    newAddress = a.Browser.MainFrame.Url;
                };
                Browser.FrameLoadStart += (s, a) =>
                {
                    if (!a.Frame.IsMain)
                        return;
                    var loadStartArgs = new LoadStartedEventArgs()
                    {
                        Url = newAddress
                    };
                    OnLoadStarted(loadStartArgs);
                };
            }

            Browser.FrameLoadEnd += (s, a) =>
            {
                if (!a.Frame.IsMain)
                    return;
                var loadStartArgs = new LoadFinishedEventArgs()
                {
                    Url = a.Frame.Url
                };
                OnLoadFinished(loadStartArgs);
            };
            Browser.LoadError += (s, a) =>
            {
                if (!a.Frame.IsMain)
                    return;
                var loadStartArgs = new LoadFinishedEventArgs()
                {
                    Url = a.FailedUrl,
                    ErrorType = a.ErrorCode.ToString(),
                    ErrorDescription = a.ErrorText,
                    IsError = true
                };
                OnLoadFinished(loadStartArgs);
            };


            //Auto disposing.
            Browser.StatusMessage += (s, a) =>
            {
                if (a.Browser.IsDisposed && this.IsDisposed)
                {
                    try
                    {
                        Dispose();
                    }
                    catch { }
                }
            };
        }

        #region CefSharp sprcial.
        public double ZoomLevel { get; set; } = 1;

        void InitCefSharpSpecial()
        {
            Browser.FrameLoadStart += (s, a) =>
            {
                var b = (ChromiumWebBrowser)s;
                if (a.Frame.IsMain)
                {
                    b.SetZoomLevel(ZoomLevel);
                }
            };
        }
        #endregion
    }
}
