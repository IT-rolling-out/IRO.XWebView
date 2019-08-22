using System.IO;
using Android.App;
using Android.OS;
using Android.Webkit;
using IRO.XWebView.Droid.Consts;
using IRO.XWebView.Droid.Utils;
using Debug = System.Diagnostics.Debug;
using Exception = System.Exception;

namespace IRO.XWebView.Droid
{
    public static class WebViewExtensions
    {
        public static void ApplyDefaultSettings(this WebView wv)
        {
            EnableDefaultOptions(wv);
            SetPermissionsMode(wv, PermissionsMode.AllowedAll);
            wv.AddDownloadsSupport();
            wv.AddUploadsSupport();
            var dataDirectory = Application.Context.GetExternalFilesDir("data").CanonicalPath;
            var cachePath = Path.Combine(dataDirectory, "webview_cache");
            InitWebViewCaching(wv, cachePath);
        }

        public static void SetPermissionsMode(this WebView wv, PermissionsMode permissionsMode)
        {
            AndroidThreadSync.Invoke(() =>
            {
                if (permissionsMode == PermissionsMode.AllowedAll)
                {
                    wv.Settings.AllowFileAccess = true;
                    wv.Settings.AllowFileAccessFromFileURLs = true;
                    wv.Settings.AllowUniversalAccessFromFileURLs = true;
                    wv.Settings.MixedContentMode = MixedContentHandling.AlwaysAllow;
                }
                else
                {
                    wv.Settings.AllowFileAccess = false;
                    wv.Settings.AllowFileAccessFromFileURLs = false;
                    wv.Settings.AllowUniversalAccessFromFileURLs = false;
                    wv.Settings.MixedContentMode = MixedContentHandling.NeverAllow;
                }
            });
        }

        public static void InitWebViewCaching(this WebView wv, string cacheDirectory)
        {
            AndroidThreadSync.Invoke(() =>
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
            });
        }

        /// <summary>
        /// Config WebView, that make it look like real browser.
        /// </summary>
        public static void EnableDefaultOptions(this WebView wv)
        {
            AndroidThreadSync.Invoke(() =>
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
            });
        }
    }
}