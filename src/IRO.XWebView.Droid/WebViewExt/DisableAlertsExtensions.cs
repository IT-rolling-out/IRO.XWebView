using Android.Webkit;

namespace IRO.XWebView.Droid
{
    public static class DisableAlertsExtensions
    {
        #region Disable alerts.
        /// <summary>
        /// Enable js alerts (if was disabled).
        /// </summary>
        public static void DisableAlerts(this WebView wv)
        {
            EnableAlerts(wv);
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