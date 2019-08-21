using System;
using System.Threading.Tasks;
using CefSharp.OffScreen;
using IRO.XWebView.CefSharp;
using IRO.XWebView.Core.Consts;

namespace IRO.XWebView.Droid.Containers
{
    public interface ICefSharpContainer : IDisposable
    {
        bool IsDisposed { get; }

        ChromiumWebBrowser CurrentBrowser { get; }

        bool CanSetVisibility { get; }

        void ToggleVisibilityState(XWebViewVisibility visibility);

        /// <summary>
        /// Wait while native WebView controll initializing.
        /// </summary>
        /// <returns></returns>
        Task WaitWebViewInitialized();

        /// <summary>
        /// Used for initializations that require <see cref="CefSharpXWebView"/>.
        /// Sometimes your visual container need access to events or some methods of XWebView.
        /// </summary>
        /// <param name="xwv"></param>
        /// <returns></returns>
        Task Wrapped(CefSharpXWebView xwv);
    }
}