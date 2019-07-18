using System;
using System.Reflection;
using System.Threading.Tasks;
using Android.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using IRO.ImprovedWebView.Core;
using IRO.ImprovedWebView.Core.EventsAndDelegates;
using IRO.ImprovedWebView.Droid.EventsProxy;

namespace IRO.ImprovedWebView.Droid
{
    public class AndroidImprovedWebView : IImprovedWebView
    {
        readonly WebView _webView;

        readonly IWebViewEventsProxy _webViewEventsProxy;

        public IWebViewEventsProxy WebViewEventsProxy { get; }

        public string BrowserType { get; } = "AndroidWebView";

        public string Url { get; } = "about:blank";

        /// <summary>
        /// Mean that browser load page or execute js.
        /// </summary>
        public bool IsBusy { get; }

        public ImprovedWebViewVisibility Visibility { get; set; }

        public AndroidImprovedWebView(WebView webView, Activity webViewActivity)
        {
            _webView = webView;
            var proxy = new WebViewEventsProxy();
            var webViewClient = new ProxyWebViewClient(proxy);
            _webView.SetWebViewClient(webViewClient);
        }

        public static AndroidImprovedWebView Create<TActivity>(WebView webView, TActivity webViewActivity)
            where TActivity:Activity, IWebViewEventsProxy
        {
          
        }

        public Task LoadUrl(string url)
        {
            throw new NotImplementedException();
        }

        public Task LoadHtml(string html, string baseUrl = "about:blank")
        {
            throw new NotImplementedException();
        }

        public Task WaitWhileBusy()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// You can call passed method from js.
        /// All its exceptions will be passed to js.
        /// If method return Task - it will be converted to promise.
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="invokeOn">
        /// Instance of object for which the method will be invoked.
        /// Can be null for static.
        /// </param>
        /// <param name="functionName"></param>
        /// <param name="jsObjectName"></param>
        public void BindToJs(MethodInfo methodInfo, object invokeOn, string functionName, string jsObjectName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Js result will be converted by JsonConvert.
        /// Note: Promises will be awaited like <see cref="Task"/>.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="script"></param>
        /// <returns></returns>
        public Task<TResult> ExJs<TResult>(string script)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Override and add any your code.
        /// <para></para>
        /// Not js function call.
        /// Just dynamic function call, that must be processed on browser level.
        /// Works like messaging system.
        /// For example, 'InjectJQuery' cmd.
        /// </summary>
        public virtual async Task<TResult> CallCmd<TResult>(string cmdName, object[] parameters = null)
        {
            throw new NotImplementedException();
        }

        public Task Finish()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CanGoForward()
        {
            bool? res = null;
            Invoke(() =>
            {
                res = _webView.CanGoForward();
            });
            return res.Value;
        }

        public async Task GoForward()
        {
            Invoke(() =>
            {
                var canGoForward = _webView.CanGoBack();
                var args = new GoForwardEventArgs();
                args.CanGoForward = canGoForward;
                GoForwardRequested?.Invoke(this, args);
                if (args.Cancel)
                    return;
                _webView.GoForward();
            });
        }

        public async Task<bool> CanGoBack()
        {
            bool? res = null;
            Invoke(() =>
            {
                res = _webView.CanGoBack();
            });
            return res.Value;
        }

        public async Task GoBack()
        {
            Invoke(() =>
            {
                var canGoBack = _webView.CanGoBack();
                var args = new GoBackEventArgs();
                args.CanGoBack = canGoBack;
                GoBackRequested?.Invoke(this, args);
                if (args.Cancel)
                    return;
                _webView.GoBack();
            });

        }

        /// <summary>
        /// Return native WebView.
        /// </summary>
        /// <returns></returns>
        public object Native()
        {
            return _webView;
        }

        #region Events.
        public event GoBackDelegate GoBackRequested;
        public event GoForwardDelegate GoForwardRequested;
        public event LoadStartedDelegate LoadStarted;
        public event LoadFinishedDelegate LoadFinished;
        public event Action<object, EventArgs> Finishing;
        public event Action<object, EventArgs> Finished;
        #endregion

        public void StartLoading(string url)
        {
            _webView.LoadUrl(url);
        }

        public void StartLoadingHtml(string data, string baseUrl)
        {
            _webView.LoadDataWithBaseURL(
                baseUrl,
                data,
                "text/html",
                "UTF-8",
                Url
                );
        }

        /// <summary>
        /// Invoke to main thread.
        /// </summary>
        /// <param name="act"></param>
        void Invoke(Action act)
        {
            Application.SynchronizationContext.Post((obj) => { act(); }, null);
        }
    }

    public class AndroidImprovedWebViewExtensions
    {
        /// <summary>
        /// Extension.
        /// <para></para>
        /// Add event to android back button tap, it will invoke GoBack() method.
        /// </summary>
        public void UseBackButtonCrunch(AndroidImprovedWebView androidImprovedWebView)
        {
            int backTaps = 0;
            int wantToQuitApp = 0;
            var ev = new EventHandler<View.KeyEventArgs>(async (s, e) =>
            {
                if (e.KeyCode == Keycode.Back)
                {
                    e.Handled = true;
                    if (backTaps > 0)
                    {
                        //wantToQuitApp используется для двух попыток нажать назад перед оконсчательной установкой, что нельзя идти назад.
                        //Просто баг в WebView.
                        var canGoBack = await androidImprovedWebView.CanGoBack() && wantToQuitApp > 0;
                        if (canGoBack)
                        {
                            wantToQuitApp = 0;
                            backTaps = 0;
                            await androidImprovedWebView.GoBack();
                        }
                        else
                        {
                            wantToQuitApp++;
                        }
                    }
                    else
                    {
                        backTaps++;
                    }
                }

            });
            wv.KeyPress += ev;
        }
    }
}