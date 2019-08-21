using System;
using System.Collections.Generic;
using System.Text;
using CefSharp;
using CefSharp.Handler;

namespace IRO.XWebView.CefSharp.BrowserClients
{
    public class CustomRequestHandler:RequestHandler
    {
        /// <summary>
        /// Used in Cancel load implemention.
        /// </summary>
        public event OnBeforeBrowse BeforeBrowse;

        protected override bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            return base.OnBeforeBrowse(chromiumWebBrowser, browser, frame, request, userGesture, isRedirect);
        }
    }

    
}
