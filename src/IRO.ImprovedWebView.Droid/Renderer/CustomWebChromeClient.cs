using System;
using Android.App;
using Android.Content;
using Android.Webkit;
using Android.Widget;
using IRO.AndroidActivity;

namespace IRO.ImprovedWebView.Droid
{
    public class CustomWebChromeClient : WebChromeClient
    {
        public const int RequestSelectFile = 3412;

        int fileCounter = 0;

        public bool UploadsEnabled { get; set; }

        /// <summary>
        /// Обработчик отправки файлов в браузере.
        /// </summary>
        public override bool OnShowFileChooser(WebView webView, IValueCallback filePathCallback, FileChooserParams fileChooserParams)
        {
            if (UploadsEnabled)
                return false;

            try
            {
                Intent intent = fileChooserParams.CreateIntent();
                ActivityExtensions.StartActivityAndReturnResult(
                    intent
                    )
                .ContinueWith((t) =>
                {
                    try
                    {
                        var resultArgs = t.Result;
                        var uriArr = FileChooserParams.ParseResult(
                            Convert.ToInt32(resultArgs.ResultCode),
                            resultArgs.Data
                            );
                        filePathCallback?.OnReceiveValue(uriArr);
                    }
                    catch (Exception ex)
                    {
                    }
                });
                return true;
            }
            catch (Exception ex)
            {
                Application.SynchronizationContext.Post((obj) =>
                {
                    Toast.MakeText(Application.Context, "Cannot open file chooser.", ToastLength.Long).Show();
                }, null);
                return false;
            }
        }
    }
}