using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CefSharp.OffScreen;

namespace IRO.XWebView.CefSharp.OffScreen.Utils
{
    public static class OffScreenCefHelpers
    {
        public static async Task WaitInitialization(this ChromiumWebBrowser webBrowser)
        {
            if (webBrowser.IsBrowserInitialized)
            {
                return;
            }
            var tcs = new TaskCompletionSource<object>(TaskContinuationOptions.RunContinuationsAsynchronously);
            EventHandler ev = null;
            ev = (s, a) =>
            {
                webBrowser.BrowserInitialized -= ev;
                tcs?.TrySetResult(null);
                tcs = null;
            };
            webBrowser.BrowserInitialized += ev;
            if (!webBrowser.IsBrowserInitialized)
            {
                await tcs.Task;
            }
        }
    }
}
