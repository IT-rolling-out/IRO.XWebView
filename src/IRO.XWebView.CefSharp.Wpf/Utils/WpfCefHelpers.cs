using System;
using System.Threading.Tasks;
using CefWpf = CefSharp.Wpf;

namespace IRO.XWebView.CefSharp.Wpf.Utils
{
    public static class WpfCefHelpers
    {
        public static async Task WaitInitialization(this CefWpf.ChromiumWebBrowser webBrowser)
        {
            if (webBrowser.IsBrowserInitialized)
            {
                return;
            }
            var tcs =new TaskCompletionSource<object>(TaskContinuationOptions.RunContinuationsAsynchronously);
            EventHandler ev = null;
            ev = (s, a) =>
            {
                webBrowser.Initialized -= ev;
                tcs?.TrySetResult(null);
                tcs = null;
            };
            webBrowser.Initialized += ev;
            if (!webBrowser.IsBrowserInitialized)
            {
                await tcs.Task;
            }
        }
    }
}
