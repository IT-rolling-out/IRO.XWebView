using System;
using System.Threading.Tasks;
using Android.Webkit;
using IRO.XWebView.Core.Consts;

namespace IRO.XWebView.Droid
{
    public interface IWebViewContainer : IDisposable
    {
        bool IsDisposed { get; }

        WebView CurrentWebView { get; }

        bool CanSetVisibility { get; }

        event Action<object, EventArgs> Disposing;

        void ToggleVisibilityState(XWebViewVisibility visibility);

        Task WaitWebViewInitialized();

        Task WebViewWrapped(AndroidXWebView xwv);
    }
}