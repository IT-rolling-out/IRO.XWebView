using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace IRO.ImprovedWebView.Droid.EventsProxy
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