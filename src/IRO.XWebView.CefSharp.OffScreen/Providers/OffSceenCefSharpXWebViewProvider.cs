using System;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;
using IRO.XWebView.CefSharp.Containers;
using IRO.XWebView.CefSharp.Utils;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Exceptions;
using IRO.XWebView.Core.Providers;
using IRO.XWebView.Core.Utils;

namespace IRO.XWebView.CefSharp.OffScreen.Providers
{
    public class OffScreenCefSharpXWebViewProvider : BaseXWebViewProvider
    {
        Action<IBrowserSettings, RequestContextSettings> _configAct;

        protected override async Task<IXWebView> ProtectedResolve(XWebViewVisibility preferredVisibility)
        {
            if (preferredVisibility == XWebViewVisibility.Visible)
            {
                throw new XWebViewException($"Can't create visible offscreen {nameof(CefSharpXWebView)}.");
            }
            var chromiumWebBrowser = CreateOffScreen();
            var container = new SelfCefSharpContainer(chromiumWebBrowser);
            var xwv = new CefSharpXWebView(container);
            XWebViewThreadSync.Inst.Invoke(() =>
            {
                chromiumWebBrowser.CreateBrowser(); 
            });
            await chromiumWebBrowser.WaitInitialization();
            return xwv;
        }

        public void Configure(Action<IBrowserSettings, RequestContextSettings> action)
        {
            _configAct = action;
        }

        public virtual ChromiumWebBrowser CreateOffScreen()
        {
            CefHelpers.InitializeCefIfNot(new CefSettings());
            return XWebViewThreadSync.Inst.Invoke(() =>
            {
                var browserSettings = new BrowserSettings();
                browserSettings.WindowlessFrameRate = 3;
                var requestContextSettings = new RequestContextSettings();
                _configAct?.Invoke(browserSettings, requestContextSettings);
                var requestContext = new RequestContext(requestContextSettings);
                var browser = new ChromiumWebBrowser(
                    "about:blank",
                    browserSettings,
                    requestContext,
                    automaticallyCreateBrowser: false
                    );
                return browser;
            });

        }
    }
}
