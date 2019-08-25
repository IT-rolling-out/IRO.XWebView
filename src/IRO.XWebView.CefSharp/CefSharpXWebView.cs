using System;
using System.Threading.Tasks;
using CefSharp;
using IRO.XWebView.CefSharp.BrowserClients;
using IRO.XWebView.CefSharp.Containers;
using IRO.XWebView.Core;
using IRO.XWebView.Core.BindingJs.LowLevelBridge;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Events;
using IRO.XWebView.Core.Exceptions;
using IRO.XWebView.Core.Utils;

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

        public override bool IsNavigating
        {
            get
            {
                return ThreadSync.Inst.Invoke(() => Browser.IsLoading);
            }
        }

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

            ThreadSync.Inst.Invoke(() =>
            {
                var bindingOpt = new BindingOptions();
                bindingOpt.CamelCaseJavascriptNames = false;
                Browser.RegisterJsObject(
                    Core.BindingJs.BindingJsSystem.JsBridgeObjectName,
                    _bridge,
                    bindingOpt
                    );
                Browser.RequestHandler = customRequestHandler ?? new CustomRequestHandler();

                // ReSharper disable once VirtualMemberCallInConstructor
                RegisterEvents();
            });
        }

        public override async Task<string> UnmanagedExecuteJavascriptWithResult(string script, int? timeoutMS = null)
        {
            ThrowIfDisposed();
            await WaitCanExecuteJs();
            return await ThreadSync.Inst.InvokeAsync(async () =>
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

        public override async void UnmanagedExecuteJavascriptAsync(string script, int? timeoutMS = null)
        {
            ThrowIfDisposed();
            await WaitCanExecuteJs();
            ThreadSync.Inst.Invoke(() =>
            {
                if (!Browser.CanExecuteJavascriptInMainFrame)
                    throw new XWebViewException("Can't execute js in main frame.");
                Browser.ExecuteScriptAsync(script);
            });
        }

        public override void Stop()
        {
            ThrowIfDisposed();
            WaitWhileNavigating().Wait();
            ThreadSync.Inst.TryInvoke(() =>
            {
                Browser.Stop();
            });
            WaitWhileNavigating().Wait();
        }

        public override void ClearCookies()
        {
            ThrowIfDisposed();
            ThreadSync.Inst.TryInvoke(() =>
            {
                var cookieManager = Browser.GetCookieManager();
                cookieManager.DeleteCookies();
            });
        }

        protected override void StartLoading(string url)
        {
            ThreadSync.Inst.TryInvoke(() =>
            {
                Browser.Load(url);
            });
        }

        protected override void StartLoadingHtml(string data, string baseUrl)
        {
            ThreadSync.Inst.TryInvoke(() =>
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
            return ThreadSync.Inst.Invoke(() => Browser.CanGoForward);
        }

        public override bool CanGoBack()
        {
            return ThreadSync.Inst.Invoke(() => Browser.CanGoBack);
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
                    var args = new LoadStartedEventArgs()
                    {
                        Url = request.Url
                    };
                    OnLoadStarted(args);
                    return !args.Cancel;
                }
                return true;
            };
            Browser.FrameLoadEnd += (s, a) =>
            {
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

        #region CefSharp special.
        /// <summary>
        /// Return true if can execute.
        /// </summary>
        async Task<bool> WaitCanExecuteJs(int timeoutMS = 3000)
        {
            while (true)
            {
                var canExecute = ThreadSync.Inst.Invoke(
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

        /// <summary>
        /// Return true if can execute.
        /// </summary>
        async Task<bool> WaitLoadingPage(int timeoutMS = 5000)
        {
            while (true)
            {
                var isLoading = ThreadSync.Inst.Invoke(
                    () => Browser.IsLoading
                    );
                if (!isLoading)
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
