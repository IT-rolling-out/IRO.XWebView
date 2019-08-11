using Android.Graphics;
using Android.Net.Http;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;

namespace IRO.XWebView.Droid.BrowserClients
{
    public delegate void OnPageFinished(WebView view, string url);

    public delegate void OnPageStarted(WebView view, string url, Bitmap favicon);

    public delegate void ShouldOverrideUrlLoading(WebView view, string url);

    public delegate void ShouldOverrideUrlLoading2(WebView view, IWebResourceRequest request);

    public delegate void OnReceivedError(WebView view, [GeneratedEnum] ClientError errorCode,
        string description,
        string failingUrl);

    public delegate void OnReceivedError2(WebView view, IWebResourceRequest request, WebResourceError error);

    public delegate void OnLoadResource(WebView view, string url);

    public delegate void OnPageCommitVisible(WebView view, string url);


    public delegate void DoUpdateVisitedHistory(WebView view, string url, bool isReload);

    public delegate void OnFormResubmission(WebView view, Message dontResend, Message resend);

    public delegate void OnReceivedClientCertRequest(WebView view, ClientCertRequest request);

    public delegate void OnReceivedHttpAuthRequest(WebView view, HttpAuthHandler handler, string host, string realm);

    public delegate void OnReceivedHttpError(WebView view, IWebResourceRequest request, WebResourceResponse errorResponse);


    public delegate void OnReceivedLoginRequest(WebView view, string realm, string account, string args);

    public delegate void OnReceivedSslError(WebView view, SslErrorHandler handler, SslError error);

    public delegate bool OnRenderProcessGone(WebView view, RenderProcessGoneDetail detail);

    public delegate void OnSafeBrowsingHit(WebView view, IWebResourceRequest request,
        [GeneratedEnum] SafeBrowsingThreat threatType, SafeBrowsingResponse callback);

    public delegate void OnScaleChanged(WebView view, float oldScale, float newScale);

    public delegate void OnTooManyRedirects(WebView view, Message cancelMsg, Message continueMsg);

    public delegate void OnUnhandledInputEvent(WebView view, InputEvent e);

    public delegate void OnUnhandledKeyEvent(WebView view, KeyEvent e);

    public delegate WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request);

    public delegate WebResourceResponse ShouldInterceptRequest2(WebView view, string url);

    public delegate bool ShouldOverrideKeyEvent(WebView view, KeyEvent e);
}