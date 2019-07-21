using System;
using System.Threading.Tasks;
using Android.App;
using Android.Webkit;
using IRO.ImprovedWebView.Core;
using IRO.ImprovedWebView.Core.BindingJs;
using IRO.ImprovedWebView.Core.EventsAndDelegates;
using IRO.ImprovedWebView.Core.Exceptions;
using IRO.ImprovedWebView.Droid.Activities;
using IRO.ImprovedWebView.Droid.BrowserClients;
using IRO.ImprovedWebView.Droid.EventsProxy;
using Newtonsoft.Json;

namespace IRO.ImprovedWebView.Droid
{
    public class AndroidImprovedWebView : BaseImprovedWebView
    {
        const string JsBridgeObjectName = "NativeBridge";

        static readonly BindingJsSystemSettings AndroidBindingJsSystemSettings = new BindingJsSystemSettings()
        {
            OnJsCallNativeAsyncFunctionName = JsBridgeObjectName + "." + nameof(NativeBridge.OnJsCallNativeAsync),
            OnJsCallNativeSyncFunctionName = JsBridgeObjectName + "." + nameof(NativeBridge.OnJsCallNativeSync),
            OnJsPromiseFinishedFunctionName = JsBridgeObjectName + "." + nameof(NativeBridge.OnJsPromiseFinished),
        };

        readonly IWebViewActivity _webViewActivity;

        public IWebViewEventsProxy EventsProxy { get; }

        public WebView CurrentWebView { get; }

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
        protected AndroidImprovedWebView(IWebViewActivity webViewActivity) : base(AndroidBindingJsSystemSettings)
        {
            _webViewActivity = webViewActivity ?? throw new ArgumentNullException(nameof(webViewActivity));
            CurrentWebView = _webViewActivity.CurrentWebView ?? throw new NullReferenceException(nameof(_webViewActivity.CurrentWebView));
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

            CurrentWebView.SetWebViewClient(webViewClient);
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

            //Add js interface.
            CurrentWebView.AddJavascriptInterface(
                new NativeBridge(this.BindingJsSystem, this),
                JsBridgeObjectName
                );
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
        public override async Task<string> ExJsDirect(string script, int? timeoutMS = null)
        {
            var jsResult = await CurrentWebView.ExJsWithResult(script, timeoutMS);
            var jsResultString = jsResult.ToString();
            return jsResultString;
        }

        public sealed override void Stop()
        {
            Invoke(() =>
            {
                CurrentWebView.StopLoading();
            });
        }

        public override async Task<bool> CanGoForward()
        {
            bool res = false;
            Invoke(() =>
            {
                res = CurrentWebView.CanGoForward();
            });
            return res;
        }

        public override async Task GoForward()
        {
            Invoke(() =>
            {
                var canGoForward = CurrentWebView.CanGoBack();
                var args = new GoForwardEventArgs();
                args.CanGoForward = canGoForward;
                OnGoForwardRequested(args);
                if (args.Cancel)
                    return;
                CurrentWebView.GoForward();
            });
        }

        public override async Task<bool> CanGoBack()
        {
            bool res = false;
            Invoke(() =>
            {
                res = CurrentWebView.CanGoBack();
            });
            return res;
        }

        public override async Task GoBack()
        {
            Invoke(() =>
            {
                var canGoBack = CurrentWebView.CanGoBack();
                var args = new GoBackEventArgs();
                args.CanGoBack = canGoBack;
                OnGoBackRequested(args);
                if (args.Cancel)
                    return;
                CurrentWebView.GoBack();
            });

        }

        /// <summary>
        /// Return native WebView.
        /// </summary>
        /// <returns></returns>
        public override object Native()
        {
            return CurrentWebView;
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override void StartLoading(string url)
        {
            Invoke(() =>
            {
                CurrentWebView.LoadUrl(url);
            });
        }

        public override void StartLoadingHtml(string data, string baseUrl)
        {
            Invoke(() =>
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