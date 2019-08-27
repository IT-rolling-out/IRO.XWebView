using System;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Internals;
using CefSharp.Wpf;
using IRO.XWebView.CefSharp.Containers;
using IRO.XWebView.CefSharp.Utils;
using IRO.XWebView.CefSharp.Wpf.Utils;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Exceptions;
using IRO.XWebView.Core.Providers;
using IRO.XWebView.Core.Utils;

namespace IRO.XWebView.CefSharp.Wpf.Providers
{
    public class WpfCefSharpXWebViewProvider : IXWebViewProvider
    {
        Action<IBrowserSettings, RequestContextSettings> _configAct;

        public virtual async Task<IXWebView> Resolve(XWebViewVisibility preferredVisibility = XWebViewVisibility.Hidden)
        {
            var chromiumWindow = CreateWpfWindow();
            var xwv = await CefSharpXWebView.Create(chromiumWindow);
            chromiumWindow.SetVisibilityState(preferredVisibility);
            ThreadSync.Inst.Invoke(() =>
            {
                chromiumWindow.Show();
            });
            await chromiumWindow.CurrentBrowser.WaitInitialization();
            return xwv;
        }

        public void Configure(Action<IBrowserSettings, RequestContextSettings> action)
        {
            _configAct = action;
        }

        public virtual ChromiumWindow CreateWpfWindow()
        {
            CefHelpers.InitializeCefIfNot(new CefSettings());
            return ThreadSync.Inst.Invoke(() =>
            {
                var chromiumWindow = new ChromiumWindow();
                var br = (ChromiumWebBrowser)chromiumWindow.CurrentBrowser;
                br.BrowserSettings ??= new BrowserSettings();
                var requestContextSettings = new RequestContextSettings();
                _configAct?.Invoke(br.BrowserSettings, requestContextSettings);
                br.RequestContext = new RequestContext(requestContextSettings);
                return chromiumWindow;
            });

        }
    }
}
