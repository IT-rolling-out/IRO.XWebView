using System;
using Android.App;
using Android.Content;
using Android.Webkit;
using Android.Widget;
using IRO.AndroidActivity;

namespace IRO.ImprovedWebView.Droid.BrowserClients
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
                    Application.SynchronizationContext.Post((obj) =>
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
                    }, null);
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