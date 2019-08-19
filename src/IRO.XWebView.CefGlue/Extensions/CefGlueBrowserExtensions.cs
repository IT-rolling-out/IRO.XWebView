using System;
using System.Threading.Tasks;
using Chromely.CefGlue.Browser;
using Chromely.CefGlue.Browser.EventParams;

namespace IRO.XWebView.CefGlue.Extensions
{
    public static class CefGlueBrowserExtensions
    {
        public const string SimpleCallbackFuncName = "OnJsSimpleCallback";

        /// <summary>
        /// Used here to get some values when bridge not attached.
        /// NOTE:You must call <see cref="SimpleCallbackFuncName" /> to return value.
        /// </summary>
        /// Сan cause deadlock, so it's better to use a timeout.
        /// <returns></returns>
        public static async Task<string> ExecuteJavascriptSimpleCallback(this CefGlueBrowser cefGlueBrowser, string script, int? timeoutMS=null)
        {
            const string ResPrefix = "SIMPLE_CALLBACK_RES_";
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            EventHandler<ConsoleMessageEventArgs> eventHandler = null;
            eventHandler = (s, e) =>
            {
                if (e.Message.StartsWith(ResPrefix))
                {
                    e.Handled = true;
                    cefGlueBrowser.ConsoleMessage -= eventHandler;
                    var res = e.Message.Substring(ResPrefix.Length);
                    tcs?.TrySetResult(res);
                }

            };
            cefGlueBrowser.ConsoleMessage += eventHandler;

            var scriptStart = $"function {SimpleCallbackFuncName}(val){{console.log('{ResPrefix}'+val);}};";
            script = scriptStart + script;
            cefGlueBrowser.CefBrowser.GetMainFrame().ExecuteJavaScript(script, "", 0);

            if (timeoutMS == null)
            {
                await tcs.Task;
            }
            else
            {
                await Task.WhenAny(
                    tcs.Task,
                    Task.Delay(timeoutMS.Value)
                );
            }

            if (tcs.Task.IsCompleted)
                return tcs.Task.Result;
            else
                throw new Exception($"Js evaluation timeout {timeoutMS}");
        }
    }
}
