using Android.Graphics;
using Android.Net.Http;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;

namespace IRO.XWebView.Droid.BrowserClients
{
    public class WebViewClientEventsProxy : IWebViewClientEventsProxy
    {
        public event OnPageFinished OnPageFinished;

        public event OnPageStarted OnPageStarted;

        public event ShouldOverrideUrlLoading ShouldOverrideUrlLoading;

        public event ShouldOverrideUrlLoading2 ShouldOverrideUrlLoading2;

        public event OnReceivedError OnReceivedError;

        public event OnReceivedError2 OnReceivedError2;

        public event OnLoadResource OnLoadResource;

        public event OnPageCommitVisible OnPageCommitVisible;


        public event DoUpdateVisitedHistory DoUpdateVisitedHistory;

        public event OnFormResubmission OnFormResubmission;

        public event OnReceivedClientCertRequest OnReceivedClientCertRequest;

        public event OnReceivedHttpAuthRequest OnReceivedHttpAuthRequest;

        public event OnReceivedHttpError OnReceivedHttpError;

        public event OnReceivedLoginRequest OnReceivedLoginRequest;

        public event OnReceivedSslError OnReceivedSslError;

        public event OnRenderProcessGone OnRenderProcessGone;

        public event OnSafeBrowsingHit OnSafeBrowsingHit;

        public event OnScaleChanged OnScaleChanged;

        public event OnTooManyRedirects OnTooManyRedirects;

        public event OnUnhandledInputEvent OnUnhandledInputEvent;

        public event OnUnhandledKeyEvent OnUnhandledKeyEvent;

        public event ShouldInterceptRequest ShouldInterceptRequest;

        public event ShouldInterceptRequest2 ShouldInterceptRequest2;

        public ShouldOverrideKeyEvent ShouldOverrideKeyEvent;


        internal void RiseOnPageFinished(WebView view, string url)
        {
            OnPageFinished?.Invoke(view, url);
        }

        internal void RiseOnPageStarted(WebView view, string url, Bitmap favicon)
        {
            OnPageStarted?.Invoke(view, url, favicon);
        }

        internal void RiseShouldOverrideUrlLoading(WebView view, string url)
        {
            ShouldOverrideUrlLoading?.Invoke(view, url);
        }

        internal void RiseShouldOverrideUrlLoading2(WebView view, IWebResourceRequest request)
        {
            ShouldOverrideUrlLoading2?.Invoke(view, request);
        }

        internal void RiseOnReceivedError(WebView view, ClientError errorcode, string description, string failingurl)
        {
            OnReceivedError?.Invoke(view, errorcode, description, failingurl);
        }

        internal void RiseOnReceivedError2(WebView view, IWebResourceRequest request, WebResourceError error)
        {
            OnReceivedError2?.Invoke(view, request, error);
        }

        internal void RiseOnLoadResource(WebView view, string url)
        {
            OnLoadResource?.Invoke(view, url);
        }

        internal void RiseOnPageCommitVisible(WebView view, string url)
        {
            OnPageCommitVisible?.Invoke(view, url);
        }

        internal void RiseDoUpdateVisitedHistory(WebView view, string url, bool isReload)
        {
            DoUpdateVisitedHistory?.Invoke(view, url, isReload);
        }

        internal void RiseOnFormResubmission(WebView view, Message dontResend, Message resend)
        {
            OnFormResubmission?.Invoke(view, dontResend, resend);
        }

        internal void RiseOnReceivedClientCertRequest(WebView view, ClientCertRequest request)
        {
            OnReceivedClientCertRequest?.Invoke(view, request);
        }

        internal void RiseOnReceivedHttpAuthRequest(WebView view, HttpAuthHandler handler, string host, string realm)
        {
            OnReceivedHttpAuthRequest?.Invoke(view, handler, host, realm);
        }

        internal void RiseOnReceivedHttpError(WebView view, IWebResourceRequest request, WebResourceResponse errorResponse)
        {
            OnReceivedHttpError?.Invoke(view, request, errorResponse);
        }

        internal void RiseOnReceivedLoginRequest(WebView view, string realm, string account, string args)
        {
            OnReceivedLoginRequest?.Invoke(view, realm, account, args);
        }

        internal void RiseOnReceivedSslError(WebView view, SslErrorHandler handler, SslError error)
        {
            OnReceivedSslError?.Invoke(view, handler, error);
        }

        internal bool? RiseOnRenderProcessGone(WebView view, RenderProcessGoneDetail detail)
        {
            return OnRenderProcessGone?.Invoke(view, detail);
        }

        internal void RiseOnSafeBrowsingHit(WebView view, IWebResourceRequest request,
            [GeneratedEnum] SafeBrowsingThreat threatType, SafeBrowsingResponse callback)
        {
            OnSafeBrowsingHit?.Invoke(view, request,threatType, callback);
        }

        internal void RiseOnScaleChanged(WebView view, float oldScale, float newScale)
        {
            OnScaleChanged?.Invoke(view, oldScale, newScale);
        }

        internal void RiseOnTooManyRedirects(WebView view, Message cancelMsg, Message continueMsg)
        {
            OnTooManyRedirects?.Invoke(view, cancelMsg, continueMsg);
        }

        internal void RiseOnUnhandledInputEvent(WebView view, InputEvent e)
        {
            OnUnhandledInputEvent?.Invoke(view,e);
        }

        internal void RiseOnUnhandledKeyEvent(WebView view, KeyEvent e)
        {
            OnUnhandledInputEvent?.Invoke(view, e);
        }

        internal WebResourceResponse RiseShouldInterceptRequest(WebView view, IWebResourceRequest request)
        {
            return ShouldInterceptRequest?.Invoke(view, request);
        }

        internal WebResourceResponse RiseShouldInterceptRequest2(WebView view, string url)
        {
            return ShouldInterceptRequest2?.Invoke(view, url);
        }

        internal bool? RiseShouldOverrideKeyEvent(WebView view, KeyEvent e)
        {
            return ShouldOverrideKeyEvent?.Invoke(view, e);
        }
    }
}