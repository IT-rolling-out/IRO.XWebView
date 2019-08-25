using System;
using System.Threading.Tasks;
using IRO.XWebView.CefSharp.Utils;
using IRO.XWebView.Core.Utils;
using CefWpf = CefSharp.Wpf;

namespace IRO.XWebView.CefSharp.Wpf.Utils
{
    public static class WpfCefHelpers
    {
        public static async Task WaitInitialization(this CefWpf.ChromiumWebBrowser webBrowser)
        {
            var tcs = new TaskCompletionSource<object>(TaskContinuationOptions.RunContinuationsAsynchronously);
            bool isBrowserInit = false;
            ThreadSync.Inst.Invoke(() =>
            {
                if (webBrowser.IsBrowserInitialized)
                {
                    return;
                }

                
                EventHandler ev = null;
                ev = (s, a) =>
                {
                    webBrowser.Initialized -= ev;
                    tcs?.TrySetResult(null);
                    tcs = null;
                };
                webBrowser.Initialized += ev;
                isBrowserInit = webBrowser.IsBrowserInitialized;
                
            });
            if (isBrowserInit)
            {
                await tcs.Task;
            }
        }
    }
}
