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

        void ToggleVisibilityState(XWebViewVisibility visibility);

        /// <summary>
        /// Wait while native WebView controll initializing.
        /// </summary>
        /// <returns></returns>
        Task WaitWebViewInitialized();

        /// <summary>
        /// Used for initializations that require <see cref="AndroidXWebView"/>.
        /// Sometimes your visual container need access to events or some methods of XWebView.
        /// </summary>
        /// <param name="xwv"></param>
        /// <returns></returns>
        Task WebViewWrapped(AndroidXWebView xwv);
    }
}