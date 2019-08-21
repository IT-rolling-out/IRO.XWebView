using System;
using System.Collections.Generic;
using System.Text;
using CefSharp;

namespace IRO.XWebView.CefSharp.BrowserClients
{
    public delegate bool? OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request,
        bool userGesture, bool isRedirect);
}
