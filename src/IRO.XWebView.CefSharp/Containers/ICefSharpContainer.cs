using System;
using System.Threading.Tasks;
using CefSharp;
using IRO.XWebView.Core.Consts;

namespace IRO.XWebView.CefSharp.Containers
{
    public interface ICefSharpContainer : IDisposable
    {
        bool IsDisposed { get; }

        IWebBrowser CurrentBrowser { get; }

        bool CanSetVisibility { get; }

        void SetVisibilityState(XWebViewVisibility visibility);

        XWebViewVisibility GetVisibilityState();

        /// <summary>
        /// Used for initializations that require <see cref="CefSharpXWebView"/>.
        /// Sometimes your visual container need access to events or some methods of XWebView.
        /// </summary>
        /// <param name="xwv"></param>
        /// <returns></returns>
        Task Wrapped(CefSharpXWebView xwv);
    }
}