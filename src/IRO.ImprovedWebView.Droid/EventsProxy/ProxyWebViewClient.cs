using System;
using Android.Graphics;
using Android.Runtime;
using Android.Webkit;
using IRO.ImprovedWebView.Core.EventsAndDelegates;
using IRO.ImprovedWebView.Droid.EventsProxy;

namespace IRO.ImprovedWebView.Droid
{
    /// <summary>
    /// Настройка браузера.
    /// </summary>
    public class ProxyWebViewClient : WebViewClient
    {
        readonly WebViewEventsProxy _proxy;

        public ProxyWebViewClient(WebViewEventsProxy proxy)
        {
            _proxy = proxy;
        }

        public override void OnPageFinished(WebView view, string url)
        {
            _proxy.OnPageFinished(view, url);
            base.OnPageFinished(view, url);
        }

        public override void OnPageStarted(WebView view, string url, Bitmap favicon)
        {
            _proxy.OnPageFinished(view, url);
            base.OnPageStarted(view, url, favicon);
        }

        [Obsolete]
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            _proxy.ShouldOverrideUrlLoading(view, url);
            return base.ShouldOverrideUrlLoading(view, url);
        }

        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            _proxy.ShouldOverrideUrlLoading2(view, request);
            return base.ShouldOverrideUrlLoading(view, request);
        }

        [Obsolete]
        public override void OnReceivedError(WebView view, [GeneratedEnum] ClientError errorCode, string description, string failingUrl)
        {
            _proxy.OnReceivedError(view, errorCode, description, failingUrl);
            base.OnReceivedError(view, errorCode, description, failingUrl);
        }

        public override void OnReceivedError(WebView view, IWebResourceRequest request, WebResourceError error)
        {
            _proxy.OnReceivedError2(view, request, error);
            base.OnReceivedError(view, request, error);
        }

        public override void OnLoadResource(WebView view, string url)
        {
            _proxy.OnLoadResource(view, url);
            base.OnLoadResource(view, url);
        }

    }
}