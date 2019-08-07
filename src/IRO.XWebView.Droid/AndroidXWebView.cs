using System;
using System.Threading.Tasks;
using Android.Webkit;
using IRO.XWebView.Core;
using IRO.XWebView.Core.BindingJs;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Exceptions;
using IRO.XWebView.Droid.Activities;
using IRO.XWebView.Droid.Utils;

namespace IRO.XWebView.Droid
{
    public class AndroidXWebView : BaseXWebView
    {
        IWebViewActivity _webViewActivity;

        public IWebViewEventsProxy EventsProxy { get; private set; }

        public WebView CurrentWebView { get; private set; }

        public override bool CanSetVisibility => true;

        /// <summary>
        /// WebViewClient will be overrided.
        /// </summary>
        /// <param name="webViewActivity"></param>
        protected AndroidXWebView(IWebViewActivity webViewActivity)
        {
            _webViewActivity = webViewActivity ?? throw new ArgumentNullException(nameof(webViewActivity));
            CurrentWebView = _webViewActivity.CurrentWebView ??
                             throw new NullReferenceException(nameof(_webViewActivity.CurrentWebView));

            //Set events proxy webclient.
            var proxy = new WebViewEventsProxy();
            var webViewClient = new CustomWebViewClient(proxy);
            EventsProxy = proxy;
            CurrentWebView.SetWebViewClient(webViewClient);

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
            webViewActivity.Finishing += delegate
            {
                if (weakThis.TryGetTarget(out var thisReference))
                    thisReference.Dispose(true);
            };

            //Stop load because previouse loads will not be
            //detected (because IsBusy set event wasn't assigned from start).
            Stop();

            //Add js interface.
            CurrentWebView.AddJavascriptInterface(
                new AndroidBridge(BindingJsSystem, this),
                BindingJsSystem.JsBridgeObjectName
                );
        }

        public static async Task<AndroidXWebView> Create(IWebViewActivity webViewActivity)
        {
            if (webViewActivity == null)
                throw new ArgumentNullException(nameof(webViewActivity));
            await webViewActivity.WaitWebViewInitialized();
            var iwv = new AndroidXWebView(webViewActivity);
            await iwv.TryLoadUrl("about:blank");
            ThreadSync.TryInvoke(() => { iwv.CurrentWebView.ClearHistory(); });
            await webViewActivity.WebViewWrapped(iwv);
            return iwv;
        }

        /// <summary>
        /// Js result will be converted by JsonConvert.
        /// Note: Promises will be awaited like <see cref="Task" />.
        /// </summary>
        public override async Task<string> ExJsDirect(string script, int? timeoutMS = null)
        {
            ThrowIfDisposed();
            var jsResult = await CurrentWebView.ExJsWithResult(script, timeoutMS);
            var jsResultString = jsResult.ToString();
            return jsResultString;
        }

        public sealed override void Stop()
        {
            ThreadSync.Invoke(() => { CurrentWebView.StopLoading(); });
        }

        public override bool CanGoForward()
        {
            var res = ThreadSync.Invoke(() => CurrentWebView.CanGoForward());
            return res;
        }

        public override bool CanGoBack()
        {
            var res = ThreadSync.Invoke(() => CurrentWebView.CanGoBack());
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

        protected override void ToggleVisibilityState(XWebViewVisibility visibility)
        {
            _webViewActivity.ToggleVisibilityState(visibility);
        }

        protected override void StartLoading(string url)
        {
            ThreadSync.Invoke(() => { CurrentWebView.LoadUrl(url); });
        }

        protected override void StartLoadingHtml(string data, string baseUrl)
        {
            ThreadSync.Invoke(() =>
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
            ThreadSync.Invoke(() => { CurrentWebView.Reload(); });
        }

        protected override void StartGoForward()
        {
            ThreadSync.Invoke(() => { CurrentWebView.GoForward(); });
        }

        protected override void StartGoBack()
        {
            ThreadSync.Invoke(() => { CurrentWebView.GoBack(); });
        }

        public override void Dispose()
        {
            Dispose(false);
        }

        void Dispose(bool acrivityFinishing)
        {
            if (IsDisposed)
                return;
            OnDisposing();

            //Dispose webview.
            ThreadSync.Invoke(() =>
            {
                try
                {
                    CurrentWebView.Dispose();
                }
                catch { }
            });
            CurrentWebView = null;

            //Dispose activity.
            if (!acrivityFinishing)
            {
                ThreadSync.Invoke(() =>
                {
                    try
                    {
                        _webViewActivity.Finish();
                    }
                    catch { }
                });
            }
            _webViewActivity = null;
            EventsProxy = null;
            OnDisposed();
        }
    }
}