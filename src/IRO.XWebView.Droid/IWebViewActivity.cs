using System;
using System.Threading.Tasks;
using Android.Webkit;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;

namespace IRO.XWebView.Droid.Activities
{
    public interface IWebViewActivity
    {
        void ToggleVisibilityState(XWebViewVisibility visibility);

        event Action<IWebViewActivity> Finishing;

        WebView CurrentWebView { get; }

        Task WaitWebViewInitialized();

        Task WebViewWrapped(AndroidXWebView XWebView);

        void Finish();
    }
}