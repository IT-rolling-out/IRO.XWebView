using System;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;

namespace IRO.XWebView.CefSharp.Providers
{
    public class OffScreenCefSharpXWebViewProvider : IXWebViewProvider
    {
        Action<BrowserSettings, RequestContextSettings> _offScreenConfigAct;

        public Task<IXWebView> Resolve(XWebViewVisibility prefferedVisibility = XWebViewVisibility.Hidden)
        {
            throw new NotImplementedException();
        }

        public void ConfigOffScreen(Action<BrowserSettings, RequestContextSettings> action)
        {
            _offScreenConfigAct = action;
        }

        public async Task<ChromiumWebBrowser> CreateOffScreen()
        {
            var browserSettings = new BrowserSettings();
            //Reduce rendering speed to one frame per second so it's easier to take screen shots
            browserSettings.WindowlessFrameRate = 5;
            var requestContextSettings = new RequestContextSettings();
            _offScreenConfigAct?.Invoke(browserSettings, requestContextSettings);
            var requestContext = new RequestContext(requestContextSettings);
            var browser = new ChromiumWebBrowser("about:blank", browserSettings, requestContext);
            var onUi = Cef.CurrentlyOnThread(CefThreadIds.TID_UI);

            // For Google.com pre-pupulate the search text box
            await browser.EvaluateScriptAsync("document.getElementById('lst-ib').value = 'CefSharp Was Here!'");


            //Gets a wrapper around the underlying CefBrowser instance
            var cefBrowser = browser.GetBrowser();
            // Gets a warpper around the CefBrowserHost instance
            // You can perform a lot of low level browser operations using this interface
            var cefHost = cefBrowser.GetHost();

            //You can call Invalidate to redraw/refresh the image
            cefHost.Invalidate(PaintElementType.View);
            return browser;

        }
    }
}
