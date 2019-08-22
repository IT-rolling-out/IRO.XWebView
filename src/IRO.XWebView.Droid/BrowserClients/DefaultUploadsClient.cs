using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using IRO.AndroidActivity;
using IRO.XWebView.Droid.Utils;

namespace IRO.XWebView.Droid.BrowserClients
{
    public static class DefaultUploadsClient 
    {
        /// <summary>
        /// Обработчик отправки файлов в браузере.
        /// </summary>
        public static bool? OnShowFileChooser(WebView webView, IValueCallback filePathCallback,
            WebChromeClient.FileChooserParams fileChooserParams)
        {
            try
            {
                var intent = fileChooserParams.CreateIntent();
                var task=ActivityExtensions.StartActivityAndReturnResult(intent);
                    task.ContinueWith((t) =>
                    {
                        AndroidThreadSync.TryInvoke(() =>
                        {
                            var resultArgs = t.Result;
                            var uriArr = WebChromeClient.FileChooserParams.ParseResult(
                                Convert.ToInt32(resultArgs.ResultCode),
                                resultArgs.Data
                            );
                            filePathCallback?.OnReceiveValue(uriArr);
                        });
                    });
                return true;
            }
            catch (Exception ex)
            {
                AndroidThreadSync.TryInvoke(() =>
                {
                    Toast.MakeText(Application.Context, "Cannot open file chooser.", ToastLength.Long).Show();
                });
                return false;
            }
        }
    }
}
