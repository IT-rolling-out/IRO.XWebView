using Android.Graphics;
using Android.Webkit;

namespace IRO.ImprovedWebView.Droid.EventsProxy
{
    public class WebViewEventsProxy:IWebViewEventsProxy
    {
        public event OnPageFinishedDelegate PageFinishedEvent;

        public event OnPageStartedDelegate PageStartedEvent;

        public event ShouldOverrideUrlLoadingDelegate ShouldOverrideUrlLoadingEvent;

        public event ShouldOverrideUrlLoading2Delegate ShouldOverrideUrlLoading2Event;

        public event OnReceivedErrorDelegate ReceivedErrorEvent;

        public event OnReceivedError2Delegate ReceivedError2Event;

        public event OnLoadResourceDelegate LoadResourceEvent;

        public event OnPageCommitVisible PageCommitVisible;


        public virtual void OnPageFinished(WebView view, string url)
        {
            PageFinishedEvent?.Invoke(view, url);
        }

        public virtual void OnPageStarted(WebView view, string url, Bitmap favicon)
        {
            PageStartedEvent?.Invoke(view, url, favicon);
        }

        public virtual void ShouldOverrideUrlLoading(WebView view, string url)
        {
            ShouldOverrideUrlLoadingEvent?.Invoke(view, url);
        }

        public virtual void ShouldOverrideUrlLoading2(WebView view, IWebResourceRequest request)
        {
            ShouldOverrideUrlLoading2Event?.Invoke(view, request);
        }

        public virtual void OnReceivedError(WebView view, ClientError errorcode, string description, string failingurl)
        {
            ReceivedErrorEvent?.Invoke(view, errorcode, description, failingurl);
        }

        public virtual void OnReceivedError2(WebView view, IWebResourceRequest request, WebResourceError error)
        {
            ReceivedError2Event?.Invoke(view, request, error);
        }

        public virtual void OnLoadResource(WebView view, string url)
        {
            LoadResourceEvent?.Invoke(view, url);
        }

        public virtual void OnPageCommitVisible(WebView view, string url)
        {
            PageCommitVisible?.Invoke(view, url);
        }
    }
}