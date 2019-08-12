using Android.Webkit;
using IRO.XWebView.Droid.BrowserClients;

namespace IRO.XWebView.Droid
{
    public static class DownloadsAndUploadExtensions
    {
        public static CustomDownloadListener AddDownloadsSupport(this WebView wv)
        {
            var downloadListener = new CustomDownloadListener();
            wv.SetDownloadListener(downloadListener);
            return downloadListener;
        }

        public static void AddUploadsSupport(this WebView wv)
        {
            var webChromeClient = wv.ProxyWebChromeClient();
            webChromeClient.EventsProxy.OnShowFileChooser -= DefaultUploadsClient.OnShowFileChooser;
            webChromeClient.EventsProxy.OnShowFileChooser += DefaultUploadsClient.OnShowFileChooser;
        }
    }
}