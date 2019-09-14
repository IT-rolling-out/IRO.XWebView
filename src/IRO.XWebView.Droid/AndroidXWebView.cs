using System;
using System.Threading.Tasks;
using Android.OS;
using Android.Webkit;
using IRO.XWebView.Core;
using IRO.XWebView.Core.BindingJs;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Utils;
using IRO.XWebView.Droid.BrowserClients;
using IRO.XWebView.Droid.Containers;
using IRO.XWebView.Droid.Utils;

namespace IRO.XWebView.Droid
{
    public class AndroidXWebView : BaseXWebView
    {
        IWebViewContainer _webViewContainer;

        protected AndroidXWebView(IWebViewContainer webViewContainer)
        {
            _webViewContainer = webViewContainer ?? throw new ArgumentNullException(nameof(webViewContainer));
            CurrentWebView = _webViewContainer.CurrentWebView ??
                             throw new NullReferenceException(nameof(_webViewContainer.CurrentWebView));

            //Set events proxy webviewclient.
            var webViewClient = CurrentWebView.ProxyWebViewClient();
            WebViewClientEvents = webViewClient.EventsProxy;

            //Set events proxy webchromeclient.
            var webChromeClient = CurrentWebView.ProxyWebChromeClient();
            WebChromeClientEvents = webChromeClient.EventsProxy;

            //Register main events.
            var weakThis = new WeakReference<AndroidXWebView>(this);
            webViewClient.LoadStarted += (s, a) =>
            {
                if (weakThis.TryGetTarget(out var thisReference))
                    thisReference.OnLoadStarted(a);
            };
            webViewClient.LoadFinished += (s, a) =>
            {
                if (weakThis.TryGetTarget(out var thisReference))
                    thisReference.OnLoadFinished(a);
            };
            CurrentWebView.ViewDetachedFromWindow += delegate
            {
                if (weakThis.TryGetTarget(out var thisReference))
                    thisReference.Dispose(true);
            };

            //Stop load because previouse loads will not be
            //detected (because IsBusy set event wasn't assigned from start).
            Stop();

            //Add js interface.
            ThreadSync.Inst.Invoke(() =>
            {
                CurrentWebView.AddJavascriptInterface(
                    new AndroidBridge(BindingJsSystem, this),
                    Core.BindingJs.BindingJsSystem.JsBridgeObjectName
                );
            });
        }

        public IWebViewClientEventsProxy WebViewClientEvents { get; private set; }

        public WebChromeClientEventsProxy WebChromeClientEvents { get; private set; }

        public WebView CurrentWebView { get; private set; }

        public override string BrowserName => nameof(AndroidXWebView);

        public override bool CanSetVisibility => _webViewContainer.CanSetVisibility;

        public static async Task<AndroidXWebView> Create(IWebViewContainer webViewContainer)
        {
            if (webViewContainer == null)
                throw new ArgumentNullException(nameof(webViewContainer));
            await webViewContainer.WaitWebViewInitialized();
            var xwv = new AndroidXWebView(webViewContainer);
            await xwv.TryLoadUrl("about:blank");
            xwv.Stop();
            ThreadSync.Inst.TryInvoke(() =>
            {
                xwv.CurrentWebView.ClearHistory();
            });
            await webViewContainer.WebViewWrapped(xwv);
            xwv.SetInitialized();
            return xwv;
        }

        public override async Task AttachBridge()
        {
            var script = this.BindingJsSystem.GetIsBridgeAttachedScript();
            var scriptRes = await UnmanagedExecuteJavascriptWithResult(script);
            var isAttached = scriptRes == "true";
            if (!isAttached)
            {
                await base.AttachBridge();
            }
        }

        /// <inheritdoc />
        public override async Task<string> UnmanagedExecuteJavascriptWithResult(string script, int? timeoutMS = null)
        {
            ThrowIfDisposed();
            var jsResult = await CurrentWebView.ExJsWithResult(script, timeoutMS);
            var jsResultString = jsResult.ToString();
            return jsResultString;
        }

        public override void UnmanagedExecuteJavascriptAsync(string script, int? timeoutMS = null)
        {
            ThrowIfDisposed();
            ThreadSync.Inst.Invoke(() =>
            {
                CurrentWebView.EvaluateJavascript(script, null);
            });
        }

        public sealed override void Stop()
        {
            ThrowIfDisposed();
            ThreadSync.Inst.Invoke(() => { CurrentWebView.StopLoading(); });
        }

        public override bool CanGoForward()
        {
            ThrowIfDisposed();
            var res = ThreadSync.Inst.Invoke(() => CurrentWebView.CanGoForward());
            return res;
        }

        public override bool CanGoBack()
        {
            ThrowIfDisposed();
            var res = ThreadSync.Inst.Invoke(() => CurrentWebView.CanGoBack());
            return res;
        }

        /// <summary>
        /// Return native WebView.
        /// </summary>
        /// <returns></returns>
        public override object Native()
        {
            return CurrentWebView;
        }

        public override void ClearCookies()
        {
            ThrowIfDisposed();
            ThreadSync.Inst.Invoke(() =>
            {
                CurrentWebView.ClearCache(true);
                if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.LollipopMr1)
                {
                    CookieManager.Instance.RemoveAllCookies(null);
                    CookieManager.Instance.Flush();
                }
                else
                {
                    var cookieSyncMngr = CookieSyncManager.CreateInstance(Android.App.Application.Context);
                    cookieSyncMngr.StartSync();
                    var cookieManager = CookieManager.Instance;
                    cookieManager.RemoveAllCookies(null);
                    cookieManager.RemoveSessionCookie();
                    cookieSyncMngr.StopSync();
                    cookieSyncMngr.Sync();
                }
            });
        }

        protected override void SetVisibilityState(XWebViewVisibility visibility)
        {
            _webViewContainer.SetVisibilityState(visibility);
        }

        protected override XWebViewVisibility GetVisibilityState()
            => _webViewContainer.GetVisibilityState();
        

        protected override void StartLoading(string url)
        {
            ThreadSync.Inst.Invoke(() => { CurrentWebView.LoadUrl(url); });
        }

        protected override void StartLoadingHtml(string data, string baseUrl)
        {
            ThreadSync.Inst.Invoke(() =>
            {
                CurrentWebView.LoadDataWithBaseURL(
                    baseUrl,
                    data,
                    "text/html",
                    "UTF-8",
                    Url
                );
            });
        }

        protected override void StartReloading()
        {
            ThreadSync.Inst.Invoke(() => { CurrentWebView.Reload(); });
        }

        protected override void StartGoForward()
        {
            ThreadSync.Inst.Invoke(() => { CurrentWebView.GoForward(); });
        }

        protected override void StartGoBack()
        {
            ThreadSync.Inst.Invoke(() => { CurrentWebView.GoBack(); });
        }

        #region Disposing.
        bool _isDisposing;

        public override void Dispose()
        {
            base.Dispose();
            Dispose(false);
        }

        void Dispose(bool containerDisposed)
        {
            //In order not to call the method again from Activity.Finish().
            if (_isDisposing)
                return;
            _isDisposing = true;

            //Dispose webview.
            ThreadSync.Inst.Invoke(() =>
            {
                try
                {
                    CurrentWebView.Destroy();
                    CurrentWebView.Dispose();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"WebView disposing exception {ex}.");
                }
            });
            CurrentWebView = null;

            //Dispose activity.
            if (!containerDisposed)
            {
                ThreadSync.Inst.Invoke(() =>
                {
                    try
                    {
                        if (!_webViewContainer.IsDisposed)
                            _webViewContainer.Dispose();
                    }
                    catch
                    {
                    }
                });
            }

            _webViewContainer = null;
            WebViewClientEvents = null;
        }
        #endregion
    }
}