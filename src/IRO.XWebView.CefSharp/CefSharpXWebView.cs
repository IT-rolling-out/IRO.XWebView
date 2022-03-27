using System;
using System.Text;
using System.Threading.Tasks;
using IRO.XWebView.CefSharp.BrowserClients;
using IRO.XWebView.CefSharp.Containers;
using IRO.XWebView.CefSharp.Utils;
using IRO.XWebView.Core;
using IRO.XWebView.Core.BindingJs.LowLevelBridges;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Events;
using IRO.XWebView.Core.Exceptions;
using IRO.XWebView.Core.Utils;
using CefSh = CefSharp;
using CefSharp;
using CefSharp.JavascriptBinding;

namespace IRO.XWebView.CefSharp
{
    public class CefSharpXWebView : BaseXWebView
    {
        LowLevelBridge _bridge;

        ICefSharpContainer _container;

        public IWebBrowser Browser { get; private set; }

        /// <summary>
        /// If true - you can change visibility after creation.
        /// If false - <see cref="P:IRO.XWebView.Core.IXWebView.Visibility" /> assignment will throw exception.
        /// </summary>
        public override bool CanSetVisibility => _container.CanSetVisibility;

        public override string BrowserName => nameof(CefSharpXWebView);

        public CefSharpXWebView(ICefSharpContainer container, CustomRequestHandler customRequestHandler = null)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            Browser = _container.CurrentBrowser;
            if (Browser == null)
            {
                throw new XWebViewException("Browser is null.");
            }

            //Register native bridge.
            _bridge = new LowLevelBridge(this.BindingJsSystem, this);

            // ReSharper disable once VirtualMemberCallInConstructor
            ThreadSync.Invoke(() =>
            {
                /*WAS_IN_OLD_VERSION            
             Browser.RegisterJsObject(
                 Core.BindingJs.BindingJsSystem.JsBridgeObjectName,
                 _bridge,
                 bindingOpt
                 );
             */
                //REPLACED_WITH

                var bindingOpt = new BindingOptions();
                var nameConverter = new PascalCaseJavascriptNameConverter();
                var cefModelBinder = new CefSh.ModelBinding.DefaultBinder(nameConverter);
                bindingOpt.Binder = cefModelBinder;              
                Browser.JavascriptObjectRepository.NameConverter = nameConverter;
                Browser.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
                Browser.JavascriptObjectRepository.Settings.JavascriptBindingApiEnabled = true;
                Browser.JavascriptObjectRepository.Settings.AlwaysInterceptAsynchronously = false;

                Browser.JavascriptObjectRepository.Register(
                    Core.BindingJs.JsConsts.BridgeObj,
                    _bridge,
                    options: bindingOpt
                    );

                Browser.JavascriptObjectRepository.Register(
                    "TestChr_Native",
                    new TestChr(),
                options: bindingOpt
                );

                this.BindToJs(
                    new TestChr(),
                    "TestChr_BindingSystem"
                    );

                var jsObjRep = Browser.JavascriptObjectRepository;


                Browser.RequestHandler = customRequestHandler ?? new CustomRequestHandler();
                // ReSharper disable once VirtualMemberCallInConstructor
                RegisterEvents();
            });
            container.Wrapped(this);

            Browser.WaitInitialization().ContinueWith((t) =>
            {
                SetInitialized();
            });
        }

        public override async Task<string> UnmanagedExecuteJavascriptWithResult(string script, int? timeoutMS = null)
        {
            ThrowIfDisposed();
            await WaitCanExecuteJs(2000);
            return await ThreadSync.InvokeAsync(async () =>
            {
                if (!Browser.CanExecuteJavascriptInMainFrame)
                    throw new XWebViewException($"Can't execute js in main frame. " +
                                                $"Use '{nameof(UnmanagedExecuteJavascriptAsync)}' to get around this limitation.");

                TimeSpan? timeout = null;
                if (timeoutMS != null)
                {
                    timeout = TimeSpan.FromMilliseconds(timeoutMS.Value);
                }
                //Use JSON.stringify to make it compatible with other browsers.
                var allScript = $@"
var result = {script} ;
JSON.stringify(result);
";
                var jsResponse = await Browser.EvaluateScriptAsync(allScript, timeout);
                if (jsResponse.Success)
                {
                    var res = jsResponse.Result?.ToString();
                    return res;
                }
                else
                {
                    throw new XWebViewException(
                        $"{nameof(UnmanagedExecuteJavascriptWithResult)} error '{jsResponse.Message}'.");
                }
            });
        }

        public override void UnmanagedExecuteJavascriptAsync(string script, int? timeoutMS = null)
        {
            ThrowIfDisposed();
            ThreadSync.Invoke(() =>
            {
                //?Why not Browser.ExecuteScriptAsync(script); ?
                //Method above will throw exceptions if V8Context of frame is not created.
                //This can happen when page load aborted or page doesn't contains javascript.
                //Opposite, Frame.ExecuteJavaScriptAsync will ignore this and always execute js,
                //because it will create context if it not exists.
                var mainFrame = Browser.GetMainFrame();
                mainFrame.ExecuteJavaScriptAsync(script, mainFrame.Url);
            });
        }

        public override void Stop()
        {
            ThrowIfDisposed();
            ThreadSync.TryInvoke(() =>
            {
                Browser.Stop();
            });
        }

        public override void ClearCookies()
        {
            ThrowIfDisposed();
            ThreadSync.TryInvoke(() =>
            {
                var cookieManager = Browser.GetCookieManager();
                cookieManager.DeleteCookies();
            });
        }

        protected override void StartLoading(string url)
        {
            ThreadSync.TryInvoke(() =>
            {
                Browser.Load(url);
            });
        }

        protected override void StartLoadingHtml(string data, string baseUrl)
        {
            ThreadSync.TryInvoke(() =>
            {
                if (baseUrl == "about:blank")
                {
                    //Must be https scheme.
                    baseUrl = "https://local.domain";
                }
                Browser.LoadHtml(data, baseUrl, encoding: Encoding.UTF8);
            });
        }

        protected override void SetVisibilityState(XWebViewVisibility visibility)
        {
            _container.SetVisibilityState(visibility);
        }

        protected override XWebViewVisibility GetVisibilityState()
            => _container.GetVisibilityState();

        public override bool CanGoForward()
        {
            return ThreadSync.Invoke(() => Browser.CanGoForward);
        }

        public override bool CanGoBack()
        {
            return ThreadSync.Invoke(() => Browser.CanGoBack);
        }

        public override object Native()
        {
            return Browser;
        }

        protected override void ProtectedDispose()
        {
            if (IsDisposed)
                return;
            try
            {
                if (_bridge == null)
                    return;
                _bridge.Dispose();
                _bridge = null;
            }
            catch { }
            ThreadSync.TryInvoke(() =>
            {
                if (Browser == null)
                    return;
                if (!Browser.IsDisposed)
                    Browser.Dispose();
                Browser = null;
            });
            ThreadSync.TryInvoke(() =>
            {
                if (_container == null)
                    return;
                if (!_container.IsDisposed)
                    _container.Dispose();
                _container = null;
            });
            base.Dispose();

        }

        protected virtual void RegisterEvents()
        {
            var reqHandler = (CustomRequestHandler)Browser.RequestHandler;
            reqHandler.BeforeBrowse += (chromiumWebBrowser, browser, frame, request, userGesture, isRedirect) =>
            {
                if (frame.IsMain && !isRedirect)
                {
                    var args = new LoadStartedEventArgs()
                    {
                        Url = request.Url
                    };
                    OnLoadStarted(args);
                    return !args.Cancel;
                }
                return true;
            };
            bool isFirstLoad = true;
            Browser.FrameLoadEnd += (s, a) =>
            {
                if (isFirstLoad)
                {
                    //!Ignore first load (load of initial page).
                    //It's easiest way to handle FrameLoadEnd of first page i found,
                    //because it will be rised without rising BeforeBrowse.
                    isFirstLoad = false;
                    return;
                }
                if (!a.Frame.IsMain)
                    return;
                var args = new LoadFinishedEventArgs()
                {
                    Url = a.Frame.Url
                };
                OnLoadFinished(args);
            };
            Browser.LoadError += (s, a) =>
            {
                if (!a.Frame.IsMain)
                    return;
                //if (a.ErrorCode == CefErrorCode.Aborted)
                //    return;
                var args = new LoadFinishedEventArgs()
                {
                    Url = a.FailedUrl,
                    ErrorType = a.ErrorCode.ToString(),
                    ErrorDescription = a.ErrorText,
                    IsError = true
                };
                OnLoadFinished(args);
            };

            //Auto disposing.
            _container.Disposed += (s, a) =>
             {
                 Dispose();
             };
        }

        #region CefSharp special.
        /// <summary>
        /// Return true if can execute.
        /// </summary>
        public async Task<bool> WaitCanExecuteJs(int timeoutMS = 3000)
        {
            while (true)
            {
                var canExecute = ThreadSync.Invoke(
                    () => Browser.CanExecuteJavascriptInMainFrame
                    );
                if (canExecute)
                {
                    return true;
                }
                await Task.Delay(20).ConfigureAwait(false);
                timeoutMS -= 20;
                if (timeoutMS <= 0)
                {
                    return false;
                }
            }
        }
        #endregion
    }
}
