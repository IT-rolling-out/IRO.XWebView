namespace IRO.XWebView.Droid.BrowserClients
{
    public interface IWebViewClientEventsProxy
    {
        event OnPageFinished OnPageFinished;
        event OnPageStarted OnPageStarted;
        event ShouldOverrideUrlLoading ShouldOverrideUrlLoading;
        event ShouldOverrideUrlLoading2 ShouldOverrideUrlLoading2;
        event OnReceivedError OnReceivedError;
        event OnReceivedError2 OnReceivedError2;
        event OnLoadResource OnLoadResource;
        event OnPageCommitVisible OnPageCommitVisible;
        event DoUpdateVisitedHistory DoUpdateVisitedHistory;
        event OnFormResubmission OnFormResubmission;
        event OnReceivedClientCertRequest OnReceivedClientCertRequest;
        event OnReceivedHttpAuthRequest OnReceivedHttpAuthRequest;
        event OnReceivedHttpError OnReceivedHttpError;
        event OnReceivedLoginRequest OnReceivedLoginRequest;
        event OnReceivedSslError OnReceivedSslError;
        event OnRenderProcessGone OnRenderProcessGone;
        event OnSafeBrowsingHit OnSafeBrowsingHit;
        event OnScaleChanged OnScaleChanged;
        event OnTooManyRedirects OnTooManyRedirects;
        event OnUnhandledInputEvent OnUnhandledInputEvent;
        event OnUnhandledKeyEvent OnUnhandledKeyEvent;
        event ShouldInterceptRequest ShouldInterceptRequest;
        event ShouldInterceptRequest2 ShouldInterceptRequest2;
    }
}