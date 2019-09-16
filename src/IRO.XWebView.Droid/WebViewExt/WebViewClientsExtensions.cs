using System;
using Android.Webkit;
using IRO.XWebView.Core.Utils;
using IRO.XWebView.Droid.BrowserClients;
using IRO.XWebView.Droid.Utils;

namespace IRO.XWebView.Droid
{
    public static class WebViewClientsExtensions
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
            _webChromeClientProvider = webChromeClientProvider ??
                                       throw new ArgumentNullException(nameof(webChromeClientProvider));
            _webViewClientProvider =
                webViewClientProvider ?? throw new ArgumentNullException(nameof(webViewClientProvider));
        }

        /// <summary>
        /// Get WebChromeClient of WebView and cast it to <see cref="CustomWebChromeClient"/>
        /// and reset it if failed. Recommended to use <see cref="CustomWebChromeClient"/> to get access via events.
        /// <para></para>
        /// NOTE: Invoked to main thread.
        /// </summary>
        public static CustomWebChromeClient ProxyWebChromeClient(this WebView wv)
        {
            return XWebViewThreadSync.Inst.Invoke(() =>
            {
                var сustomWebChromeClient = wv.WebChromeClient as CustomWebChromeClient;
                if (сustomWebChromeClient == null)
                {
                    сustomWebChromeClient = _webChromeClientProvider();
                    if (сustomWebChromeClient == null) throw new NullReferenceException(nameof(сustomWebChromeClient));
                    wv.SetWebChromeClient(сustomWebChromeClient);
                }

                return сustomWebChromeClient;
            });
        }

        /// <summary>
        /// Get WebViewClient of WebView and cast it to <see cref="CustomWebViewClient"/>
        /// and reset it if failed. Recommended to use <see cref="CustomWebViewClient"/> to get access via events.
        /// <para></para>
        /// NOTE: Invoked to main thread.
        /// </summary>
        public static CustomWebViewClient ProxyWebViewClient(this WebView wv)
        {
            return XWebViewThreadSync.Inst.Invoke(() =>
            {
                var сustomWebViewClient = wv.WebViewClient as CustomWebViewClient;
                if (сustomWebViewClient == null)
                {
                    сustomWebViewClient = _webViewClientProvider();
                    if (сustomWebViewClient == null) throw new NullReferenceException(nameof(сustomWebViewClient));
                    wv.SetWebViewClient(сustomWebViewClient);
                }
                return сustomWebViewClient;
            });
            
        }

        #endregion
    }
}