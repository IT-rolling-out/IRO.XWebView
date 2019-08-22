using System;
using System.Threading.Tasks;
using CefSharp;
using IRO.XWebView.CefSharp.BrowserClients;
using IRO.XWebView.CefSharp.Containers;
using IRO.XWebView.CefSharp.Utils;
using C = CefSharp;
using IRO.XWebView.Core;
using IRO.XWebView.Core.BindingJs.LowLevelBridge;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Events;
using IRO.XWebView.Core.Exceptions;

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

        protected CefSharpXWebView(ICefSharpContainer container, CustomRequestHandler customRequestHandler)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            Browser = _container.CurrentBrowser;
            if (Browser == null)
            {
                throw new XWebViewException("Browser is null.");
            }
            
            //Register native bridge.
            _bridge = new LowLevelBridge(this.BindingJsSystem, this);

            CefThreadSync.Invoke(() =>
            {
                Browser.ThrowExceptionIfBrowserNotInitialized();
                Browser.RegisterJsObject(
                    Core.BindingJs.BindingJsSystem.JsBridgeObjectName,
                    this
                    );
                Browser.RequestHandler = customRequestHandler ?? new CustomRequestHandler();

                // ReSharper disable once VirtualMemberCallInConstructor
                RegisterEvents();
                InitCefSharpSpecial();
            });
        }

        public static async Task<CefSharpXWebView> Create(ICefSharpContainer container, CustomRequestHandler customRequestHandler = null)
        {
            await container.WaitWebViewInitialized();
            var xwv = new CefSharpXWebView(container, customRequestHandler);
            return xwv;
        }

        public override async Task<string> UnmanagedExecuteJavascriptWithResult(string script, int? timeoutMS = null)
        {
            ThrowIfDisposed();
            return await CefThreadSync.InvokeAsync(async () =>
            {
                if (!Browser.CanExecuteJavascriptInMainFrame)
                    throw new XWebViewException("Can't execute js in main frame.");

                TimeSpan? timeout = null;
                if (timeoutMS != null)
                {
                    timeout = TimeSpan.FromMilliseconds(timeoutMS.Value);
                }

                var jsResponse = await Browser.EvaluateScriptAsync(script, timeout);
                if (jsResponse.Success)
                {
                    var res = jsResponse.Result.ToString();
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
            CefThreadSync.Invoke(() =>
            {
                if (!Browser.CanExecuteJavascriptInMainFrame)
                    throw new XWebViewException("Can't execute js in main frame.");
                Browser.ExecuteScriptAsync(script);
            });
        }

        public override void Stop()
        {
            ThrowIfDisposed();
            CefThreadSync.TryInvoke(() =>
            {
                Browser.Stop();
            });
        }

        public override void ClearCookies()
        {
            ThrowIfDisposed();
            CefThreadSync.TryInvoke(() =>
            {
                var cookieManager = Browser.GetCookieManager();
                cookieManager.DeleteCookies();
            });
        }

        protected override void StartLoading(string url)
        {
            CefThreadSync.TryInvoke(() =>
            {
                Browser.Load(url);
            });
        }

        protected override void StartLoadingHtml(string data, string baseUrl)
        {
            CefThreadSync.TryInvoke(() =>
            {
                Browser.LoadHtml(data, baseUrl);
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
            return CefThreadSync.Invoke(() => Browser.CanGoForward);
        }

        public override bool CanGoBack()
        {
            return CefThreadSync.Invoke(() => Browser.CanGoBack);
        }

        public override object Native()
        {
            return Browser;
        }

        public override void Dispose()
        {
            try
            {
                Browser.Dispose();
                _container.Dispose();
                _bridge.Dispose();
                Browser = null;
                _container = null;
                _bridge = null;
            }
            catch { }
            base.Dispose();
        }

        protected virtual void RegisterEvents()
        {
            var reqHandler = (CustomRequestHandler)Browser.RequestHandler;
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
                if (a.Browser.IsDisposed)
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
                var b = (IWebBrowser)s;
                if (a.Frame.IsMain)
                {
                    b.SetZoomLevel(ZoomLevel);
                }
            };
        }
        #endregion
    }
}
