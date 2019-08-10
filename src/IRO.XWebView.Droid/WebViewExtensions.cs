using System;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Webkit;
using IRO.XWebView.Droid.BrowserClients;
using IRO.XWebView.Droid.Consts;
using Debug = System.Diagnostics.Debug;
using Exception = System.Exception;
using Object = Java.Lang.Object;

namespace IRO.XWebView.Droid
{
    public static class WebViewExtensions
    {
        #region Clients with events.
        static Func<CustomWebChromeClient> _webChromeClientProvider = () => new CustomWebChromeClient();

        static Func<CustomWebViewClient> _webViewClientProvider = () => new CustomWebViewClient();

        /// <summary>
        /// Use it to set your own overrided browser clients that will be compatible with <see cref="AndroidXWebView"/>
        /// and <see cref="WebViewExtensions"/>.
        /// </summary>
        /// <param name="wv"></param>
        /// <param name="webChromeClientProvider"></param>
        /// <param name="webViewClientProvider"></param>
        public static void SetWebViewClients(
            this WebView wv,
            Func<CustomWebChromeClient> webChromeClientProvider,
            Func<CustomWebViewClient> webViewClientProvider
            )
        {
            _webChromeClientProvider = webChromeClientProvider ?? throw new ArgumentNullException(nameof(webChromeClientProvider));
            _webViewClientProvider = webViewClientProvider ?? throw new ArgumentNullException(nameof(webViewClientProvider));
        }

        /// <summary>
        /// Get WebChromeClient of WebView and cast it to <see cref="CustomWebChromeClient"/>
        /// and reset it if failed. Recommended to use <see cref="CustomWebChromeClient"/> to get access via events.
        /// </summary>
        public static CustomWebChromeClient ProxyWebChromeClient(this WebView wv)
        {
            var сustomWebChromeClient = wv.WebChromeClient as CustomWebChromeClient;
            if (сustomWebChromeClient == null)
            {
                сustomWebChromeClient = _webChromeClientProvider();
                if (сustomWebChromeClient==null) throw new NullReferenceException(nameof(сustomWebChromeClient));
                wv.SetWebChromeClient(сustomWebChromeClient);
            }
            return сustomWebChromeClient;
        }

        /// <summary>
        /// Get WebViewClient of WebView and cast it to <see cref="CustomWebViewClient"/>
        /// and reset it if failed. Recommended to use <see cref="CustomWebViewClient"/> to get access via events.
        /// </summary>
        public static CustomWebViewClient ProxyWebViewClient(this WebView wv)
        {
            var сustomWebViewClient = wv.WebViewClient as CustomWebViewClient;
            if (сustomWebViewClient == null)
            {
                сustomWebViewClient = _webViewClientProvider();
                if (сustomWebViewClient==null) throw new NullReferenceException(nameof(сustomWebViewClient));
                wv.SetWebViewClient(сustomWebViewClient);
            }
            return сustomWebViewClient;
        }
        #endregion

        public static void ApplyDefaultSettings(WebView wv)
        {
            EnableDefaultOptions(wv);
            SetPermissionsMode(wv, PermissionsMode.AllowedAll);
            AddDownloadsSupport(wv);
            AddUploadsSupport(wv);
            var dataDirectory = Application.Context.GetExternalFilesDir("data").CanonicalPath;
            var cachePath = Path.Combine(dataDirectory, "webview_cache");
            InitWebViewCaching(wv, cachePath);
        }

        public static void SetPermissionsMode(WebView wv, PermissionsMode permissionsMode)
        {
            if (permissionsMode == PermissionsMode.AllowedAll)
            {
                wv.Settings.AllowFileAccess = true;
                wv.Settings.AllowFileAccessFromFileURLs = true;
                wv.Settings.AllowUniversalAccessFromFileURLs = true;
            }
            else
            {
                wv.Settings.AllowFileAccess = false;
                wv.Settings.AllowFileAccessFromFileURLs = false;
                wv.Settings.AllowUniversalAccessFromFileURLs = false;
            }
        }

        public static CustomDownloadListener AddDownloadsSupport(WebView wv)
        {
            var downloadListener = new CustomDownloadListener();
            wv.SetDownloadListener(downloadListener);
            return downloadListener;
        }

        public static void AddUploadsSupport(WebView wv)
        {
            var webChromeClient = wv.ProxyWebChromeClient();
            webChromeClient.EventsProxy.OnShowFileChooser -= DefaultUploadsClient.OnShowFileChooser;
            webChromeClient.EventsProxy.OnShowFileChooser += DefaultUploadsClient.OnShowFileChooser;
        }

        public static void InitWebViewCaching(WebView wv, string cacheDirectory)
        {
            wv.Settings.CacheMode = CacheModes.Normal;
            wv.Settings.SetAppCacheMaxSize(100 * 1024 * 1024);
            wv.Settings.SetAppCacheEnabled(true);
            try
            {
                if (!Directory.Exists(cacheDirectory))
                {
                    Directory.CreateDirectory(cacheDirectory);
                }

                wv.Settings.SetAppCachePath(cacheDirectory);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"XWebView warning: {ex}");
            }
        }

        /// <summary>
        /// Config WebView, that make it look like real browser.
        /// </summary>
        public static void EnableDefaultOptions(WebView wv)
        {
            wv.Settings.BuiltInZoomControls = true;
            wv.Settings.JavaScriptEnabled = true;

#if DEBUG
            WebView.SetWebContentsDebuggingEnabled(true);
#endif

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                CookieManager.Instance.SetAcceptThirdPartyCookies(wv, true);
            }
            else
            {
                CookieManager.Instance.SetAcceptCookie(true);
            }

            wv.Settings.SetPluginState(WebSettings.PluginState.On);
            try
            {
                wv.Settings.PluginsEnabled = true;
            }
            catch
            {
            }

            wv.Settings.LoadWithOverviewMode = true;
            //wv.Settings.UseWideViewPort = true;
            wv.Settings.DefaultZoom = WebSettings.ZoomDensity.Far;
            wv.Settings.DisplayZoomControls = false;
            wv.Settings.AllowContentAccess = true;
            wv.Settings.DomStorageEnabled = true;
            wv.Settings.JavaScriptCanOpenWindowsAutomatically = true;
            wv.Settings.MixedContentMode = MixedContentHandling.AlwaysAllow;
            wv.Settings.SavePassword = true;
            wv.Settings.MediaPlaybackRequiresUserGesture = false;
            try
            {
                //Обычно не работает, нужно задать в манифесте.
                wv.Settings.SafeBrowsingEnabled = false;
            }
            catch
            {
            }

            wv.Settings.DatabaseEnabled = true;


            //Подмена user-agent нужна чтоб избежать ограничений от некоторых сайтов.
            var androidVersion = Build.VERSION.Release;
            wv.Settings.UserAgentString =
                $"Mozilla/5.0 (Linux; Android {androidVersion}; m2 Build/LMY47D) AppleWebKit/537.36 (KHTML, like Gecko) " +
                "Chrome/66.0.3359.158 Mobile Safari/537.36";
        }

        #region Ex js.
        /// <summary>
        /// Расширение WebView для выполнения скрипта  и получения результата.
        /// </summary>
        public static Task<object> ExJsWithResult(this WebView wv, string script, int? timeoutMS = null)
        {
            //TODO Такой код лочит главный поток при вызове Wait в нем. Не знаю возможноно ли вообще это исправить, но желательно.
            var callback = new JsValueCallback();
            Application.SynchronizationContext.Send((obj) => { wv.EvaluateJavascript(script, callback); }, null);

            var taskCompletionSource = callback.GetTaskCompletionSource();
            var t = taskCompletionSource.Task;
            if (timeoutMS != null)
            {
                Task.Run(async () =>
                {
                    await Task.WhenAny(
                        t,
                        Task.Delay(timeoutMS.Value)
                    );
                    if (!t.IsCompleted)
                        taskCompletionSource.TrySetException(new Exception($"Js evaluation timeout {timeoutMS}"));
                });
            }

            return t;
        }

        class JsValueCallback : Object, IValueCallback
        {
            readonly TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>(
                TaskCreationOptions.RunContinuationsAsynchronously
            );

            public void OnReceiveValue(Object value)
            {
                taskCompletionSource.SetResult(value);
            }

            public TaskCompletionSource<object> GetTaskCompletionSource()
            {
                return taskCompletionSource;
            }
        }
        #endregion

        #region Disable alerts.
        /// <summary>
        /// Enable js alerts (if was disabled).
        /// </summary>
        public static void DisableAlerts(this WebView wv)
        {
            var ep = wv.ProxyWebChromeClient().EventsProxy;
            ep.OnJsAlert += OnJsAlert;
            ep.OnJsPrompt += OnJsPrompt;
            ep.OnJsConfirm += OnJsConfirm;
        }

        /// <summary>
        /// Disable js alerts.
        /// </summary>
        public static void EnableAlerts(this WebView wv)
        {
            var ep = wv.ProxyWebChromeClient().EventsProxy;
            ep.OnJsAlert -= OnJsAlert;
            ep.OnJsPrompt -= OnJsPrompt;
            ep.OnJsConfirm -= OnJsConfirm;
        }

        static bool? OnJsAlert(WebView view, string url, string message, JsResult result)
        {
            result.Cancel();
            return true;
        }

        static bool? OnJsPrompt(WebView view, string url, string message, string defaultvalue, JsPromptResult result)
        {
            result.Cancel();
            return true;
        }

        static bool? OnJsConfirm(WebView view, string url, string message, JsResult result)
        {
            result.Cancel();
            return true;
        }
        #endregion
    }
}