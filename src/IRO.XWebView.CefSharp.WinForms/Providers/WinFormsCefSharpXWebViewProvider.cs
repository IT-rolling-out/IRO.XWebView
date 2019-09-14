using System;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.WinForms;
using IRO.XWebView.CefSharp.Utils;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;
using IRO.XWebView.Core.Utils;

namespace IRO.XWebView.CefSharp.WinForms.Providers
{
    public class WinFormsCefSharpXWebViewProvider : IXWebViewProvider
    {
        Action<IBrowserSettings, RequestContextSettings> _configAct;

        public virtual async Task<IXWebView> Resolve(XWebViewVisibility preferredVisibility = XWebViewVisibility.Hidden)
        {
            var chromiumWindow = CreateForm();
            var xwv = new CefSharpXWebView(chromiumWindow);
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

        protected virtual CefSharpXWebViewForm CreateForm()
        {
            CefHelpers.InitializeCefIfNot(new CefSettings());
            return ThreadSync.Inst.Invoke(() =>
            {
                var chromiumWindow = new CefSharpXWebViewForm();
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
