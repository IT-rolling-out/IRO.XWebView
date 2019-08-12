using System.Threading.Tasks;
using Android.App;
using Android.Webkit;
using Java.Lang;
using Exception = System.Exception;

namespace IRO.XWebView.Droid
{
    public static class WebViewJsExtensions { 

        #region Ex js.
        /// <summary>
        /// Расширение WebView для выполнения скрипта  и получения результата.
        /// </summary>
        public static Task<object> ExJsWithResult(this WebView wv, string script, int? timeoutMS = null)
        {
            //TODO Такой код лочит главный поток при вызове Wait в нем. Не знаю возможноно ли вообще это исправить, но желательно.
            var callback = new JsValueCallback();
            Application.SynchronizationContext.Send((obj) => { wv.EvaluateJavascript(script, callback); }, null);

            var taskCompletionSource = callback.GetTaskCompletionSource();
            var t = taskCompletionSource.Task;
            if (timeoutMS != null)
            {
                Task.Run(async () =>
                {
                    await Task.WhenAny(
                        t,
                        Task.Delay(timeoutMS.Value)
                    );
                    if (!t.IsCompleted)
                        taskCompletionSource.TrySetException(new Exception($"Js evaluation timeout {timeoutMS}"));
                });
            }

            return t;
        }

        class JsValueCallback : Object, IValueCallback
        {
            readonly TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>(
                TaskCreationOptions.RunContinuationsAsynchronously
            );

            public void OnReceiveValue(Object value)
            {
                taskCompletionSource.SetResult(value);
            }

            public TaskCompletionSource<object> GetTaskCompletionSource()
            {
                return taskCompletionSource;
            }
        }
        #endregion

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