using System;
using System.Threading.Tasks;
using Android.Webkit;
using IRO.ImprovedWebView.Core;

namespace IRO.ImprovedWebView.Droid.Activities
{
    public interface IWebViewActivity
    {
        void ToggleVisibilityState(ImprovedWebViewVisibility visibility);

        event Action<IWebViewActivity> Finishing;

        WebView CurrentWebView { get; }

        Task WaitWebViewInitialized();

        Task WebViewWrapped(AndroidImprovedWebView improvedWebView);

        void Finish();
    }
}