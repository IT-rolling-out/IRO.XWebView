using System;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;
using IRO.XWebView.CefSharp.Containers;
using IRO.XWebView.CefSharp.OffScreen.Utils;
using IRO.XWebView.CefSharp.Utils;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Exceptions;
using IRO.XWebView.Core.Providers;

namespace IRO.XWebView.CefSharp.OffScreen.Providers
{
    public class OffScreenCefSharpXWebViewProvider : IXWebViewProvider
    {
        Action<IBrowserSettings, RequestContextSettings> _configAct;

        public virtual async Task<IXWebView> Resolve(XWebViewVisibility prefferedVisibility = XWebViewVisibility.Hidden)
        {
            if (prefferedVisibility == XWebViewVisibility.Visible)
            {
                throw new XWebViewException("Can't create visible offscreen xwebview.");
            }
            var chromiumWebBrowser= CreateOffScreen();
            await chromiumWebBrowser.WaitInitialization();
            var container = new SelfCefSharpContainer(chromiumWebBrowser);
            var xwv = await CefSharpXWebView.Create(container);
            return xwv;
        }

        public void Configure(Action<IBrowserSettings, RequestContextSettings> action)
        {
            _configAct = action;
        }

        public virtual ChromiumWebBrowser CreateOffScreen()
        {
            CefHelpers.InitializeCefIfNot(new CefSettings());
            return ThreadSync.Inst.Invoke(() =>
            {
                var browserSettings = new BrowserSettings();
                //Reduce rendering speed to one frame per second so it's easier to take screen shots.
                browserSettings.WindowlessFrameRate = 5;
                var requestContextSettings = new RequestContextSettings();
                _configAct?.Invoke(browserSettings, requestContextSettings);
                var requestContext = new RequestContext(requestContextSettings);
                var browser = new ChromiumWebBrowser("about:blank", browserSettings, requestContext);

                var cefBrowser = browser.GetBrowser();
                var cefHost = cefBrowser.GetHost();
                //You can call Invalidate to redraw/refresh the image.
                cefHost.Invalidate(PaintElementType.View);
                return browser;
            });

        }
    }
}
