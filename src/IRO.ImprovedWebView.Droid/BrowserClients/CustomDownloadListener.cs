using System;
using Android.App;
using Android.Content;
using Android.Webkit;
using Android.Widget;

namespace IRO.ImprovedWebView.Droid.BrowserClients
{
    public class CustomDownloadListener : Java.Lang.Object, IDownloadListener
    {
        /// <summary>
        /// Deafult is true.
        /// </summary>
        public bool DownloadsEnabled { get; set; } = true;

        public event Action<CustomDownloadListener, Exception> DownloadExceptionCatched;

        public void OnDownloadStart(string url, string userAgent, string contentDisposition, string mimetype, long contentLength)
        {

            if (!DownloadsEnabled)
                return;

            try
            {

                DownloadManager.Request request = new DownloadManager.Request(
                    Android.Net.Uri.Parse(url)
                    );

                request.AllowScanningByMediaScanner();
                request.SetNotificationVisibility(DownloadVisibility.VisibleNotifyCompleted);
                var fileName = new ContentDisposition(contentDisposition).FileName.Trim('\"');
                request.SetDestinationInExternalPublicDir(Android.OS.Environment.DirectoryDownloads, fileName);
                var dm = (DownloadManager)Application.Context.GetSystemService(Context.DownloadService);
                dm.Enqueue(request);
                Application.SynchronizationContext.Post((obj) =>
                {
                    Toast.MakeText(Application.Context, "Downloading File", ToastLength.Long).Show();
                }, null);
            }
            catch (Exception ex)
            {
                DownloadExceptionCatched?.Invoke(this, ex);
            }
        }
    }
}