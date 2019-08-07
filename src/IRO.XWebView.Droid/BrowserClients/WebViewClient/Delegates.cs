using Android.Graphics;
using Android.Runtime;
using Android.Webkit;

namespace IRO.XWebView.Droid
{
    public delegate void OnPageFinishedDelegate(WebView view, string url);

    public delegate void OnPageStartedDelegate(WebView view, string url, Bitmap favicon);

    public delegate void ShouldOverrideUrlLoadingDelegate(WebView view, string url);

    public delegate void ShouldOverrideUrlLoading2Delegate(WebView view, IWebResourceRequest request);

    public delegate void OnReceivedErrorDelegate(WebView view, [GeneratedEnum] ClientError errorCode, string description,
        string failingUrl);

    public delegate void OnReceivedError2Delegate(WebView view, IWebResourceRequest request, WebResourceError error);

    public delegate void OnLoadResourceDelegate(WebView view, string url);

    public delegate void OnPageCommitVisible(WebView view, string url);
}