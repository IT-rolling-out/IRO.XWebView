using System;
using System.Threading.Tasks;
using Android.App;
using Android.Webkit;
using IRO.ImprovedWebView.Core;
using IRO.ImprovedWebView.Core.EventsAndDelegates;
using IRO.ImprovedWebView.Droid.Activities;
using IRO.ImprovedWebView.Droid.EventsProxy;
using Newtonsoft.Json;

namespace IRO.ImprovedWebView.Droid
{
    public class AndroidImprovedWebView : BaseImprovedWebView
    {
        readonly WebView _webView;

        readonly IWebViewActivity _webViewActivity;

        public IWebViewEventsProxy EventsProxy { get; }

        public override string BrowserType { get; } = "AndroidWebView";

        #region Visibility.
        ImprovedWebViewVisibility _visibility;

        public override ImprovedWebViewVisibility Visibility
        {
            get => _visibility;
            set
            {
                _visibility = value;
                _webViewActivity.ToggleVisibilityState(Visibility);
            }
        }
        #endregion

        public override bool IsBusy { get; protected set; }

        public override string Url { get; protected set; }

        /// <summary>
        /// WebViewClient will be overrided.
        /// </summary>
        /// <param name="webViewActivity"></param>
        protected AndroidImprovedWebView(IWebViewActivity webViewActivity)
        {
            _webViewActivity = webViewActivity ?? throw new ArgumentNullException(nameof(webViewActivity));
            _webView = _webViewActivity.CurrentWebView ?? throw new NullReferenceException(nameof(_webViewActivity.CurrentWebView));
            var proxy = new WebViewEventsProxy();
            var webViewClient = new ProxyWebViewClient(proxy);

            var weakThis = new WeakReference<AndroidImprovedWebView>(this);
            webViewClient.LoadStarted += (s, a) =>
            {
                if (weakThis.TryGetTarget(out var thisReference))
                {
                    thisReference.OnLoadStarted(a);
                }
            };
            webViewClient.LoadFinished += (s, a) =>
            {
                if (weakThis.TryGetTarget(out var thisReference))
                {
                    thisReference.OnLoadFinished(a);
                }
            };

            _webView.SetWebViewClient(webViewClient);
            EventsProxy = proxy;
            webViewActivity.WebViewWrapped(this);

            LoadStarted += (s, a) =>
            {
                IsBusy = true;
            };
            LoadFinished += (s, a) =>
            {
                Url = a.Url;
                IsBusy = false;
            };

            //Stop load because previouse loads will not be
            //detected (because IsBusy set event wasn't assigned from start).
            Stop();
        }

        public static async Task<AndroidImprovedWebView> Create(IWebViewActivity webViewActivity)
        {
            if (webViewActivity == null) throw new ArgumentNullException(nameof(webViewActivity));
            await webViewActivity.WaitWebViewInitialized();
            return new AndroidImprovedWebView(webViewActivity);
        }

        /// <summary>
        /// Js result will be converted by JsonConvert.
        /// Note: Promises will be awaited like <see cref="Task"/>.
        /// </summary>
        public override async Task<TResult> ExJs<TResult>(string script, int? timeoutMS = null)
        {
            var scriptUpd = "JSON.stringify((function(){" + script + "})());";
            var jsResult = await _webView.ExJsWithResult(script, timeoutMS);
            var jsResultString = jsResult.ToString();
            return JsonConvert.DeserializeObject<TResult>(jsResultString);
        }

        public sealed override void Stop()
        {
            Invoke(() =>
            {
                _webView.StopLoading();
            });
        }

        public override void Finish()
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> CanGoForward()
        {
            bool res = false;
            Invoke(() =>
            {
                res = _webView.CanGoForward();
            });
            return res;
        }

        public override async Task GoForward()
        {
            Invoke(() =>
            {
                var canGoForward = _webView.CanGoBack();
                var args = new GoForwardEventArgs();
                args.CanGoForward = canGoForward;
                OnGoForwardRequested(args);
                if (args.Cancel)
                    return;
                _webView.GoForward();
            });
        }

        public override async Task<bool> CanGoBack()
        {
            bool res = false;
            Invoke(() =>
            {
                res = _webView.CanGoBack();
            });
            return res;
        }

        public override async Task GoBack()
        {
            Invoke(() =>
            {
                var canGoBack = _webView.CanGoBack();
                var args = new GoBackEventArgs();
                args.CanGoBack = canGoBack;
                OnGoBackRequested(args);
                if (args.Cancel)
                    return;
                _webView.GoBack();
            });

        }

        /// <summary>
        /// Return native WebView.
        /// </summary>
        /// <returns></returns>
        public override object Native()
        {
            return _webView;
        }

        public override void StartLoading(string url)
        {
            Invoke(() =>
            {
                _webView.LoadUrl(url);
            });
        }

        public override void StartLoadingHtml(string data, string baseUrl)
        {
            Invoke(() =>
            {
                _webView.LoadDataWithBaseURL(
                    baseUrl,
                    data,
                    "text/html",
                    "UTF-8",
                    Url
                );
            });
        }

        /// <summary>
        /// Invoke to main thread.
        /// </summary>
        /// <param name="act"></param>
        void Invoke(Action act)
        {
            Application.SynchronizationContext.Send((obj) => { act(); }, null);
        }
    }
}