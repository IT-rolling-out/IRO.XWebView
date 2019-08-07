using System;
using Android.App;
using Android.Content;
using Android.Webkit;
using Android.Widget;
using IRO.AndroidActivity;
using IRO.ImprovedWebView.Droid.Common;

namespace IRO.ImprovedWebView.Droid
{
    public class CustomWebChromeClient : WebChromeClient
    {
        /// <summary>
        /// Обработчик отправки файлов в браузере.
        /// </summary>
        public override bool OnShowFileChooser(WebView webView, IValueCallback filePathCallback, FileChooserParams fileChooserParams)
        {

            try
            {
                Intent intent = fileChooserParams.CreateIntent();
                ActivityExtensions.StartActivityAndReturnResult(
                    intent
                    )
                .ContinueWith((t) =>
                {
                    ThreadSync.TryInvoke(() =>
                    {
                        var resultArgs = t.Result;
                        var uriArr = FileChooserParams.ParseResult(
                            Convert.ToInt32(resultArgs.ResultCode),
                            resultArgs.Data
                            );
                        filePathCallback?.OnReceiveValue(uriArr);
                    });
                });
                return true;
            }
            catch (System.Exception ex)
            {
                ThreadSync.TryInvoke(() =>
                {
                    Toast.MakeText(Application.Context, "Cannot open file chooser.", ToastLength.Long).Show();
                });
                return false;
            }
        }
    }
}