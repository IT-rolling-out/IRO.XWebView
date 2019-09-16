using System;
using Android.App;
using Android.Content;
using Android.Webkit;
using Android.Widget;
using IRO.XWebView.Core.Utils;
using IRO.XWebView.Droid.Utils;
using Environment = Android.OS.Environment;
using Exception = System.Exception;
using Object = Java.Lang.Object;
using Uri = Android.Net.Uri;

namespace IRO.XWebView.Droid.BrowserClients
{
    public class CustomDownloadListener : Object, IDownloadListener
    {
        /// <summary>
        /// Deafult is true.
        /// </summary>
        public bool DownloadsEnabled { get; set; } = true;

        public async void OnDownloadStart(string url, string userAgent, string contentDisposition, string mimetype,
            long contentLength)
        {
            if (!DownloadsEnabled)
                return;

            try
            {
                var request = new DownloadManager.Request(
                    Uri.Parse(url)
                );

                request.AllowScanningByMediaScanner();
                request.SetNotificationVisibility(DownloadVisibility.VisibleNotifyCompleted);
                var fileName = new ContentDisposition(contentDisposition).FileName.Trim('\"');
                request.SetDestinationInExternalPublicDir(Environment.DirectoryDownloads, fileName);
                var dm = (DownloadManager) Application.Context.GetSystemService(Context.DownloadService);
                dm.Enqueue(request);
                await XWebViewThreadSync.Inst.TryInvokeAsync(() =>
                {
                    Toast.MakeText(Application.Context, "Downloading File", ToastLength.Long).Show();
                });
            }
            catch (Exception ex)
            {
                DownloadExceptionCatched?.Invoke(this, ex);
            }
        }

        public event Action<CustomDownloadListener, Exception> DownloadExceptionCatched;
    }
}