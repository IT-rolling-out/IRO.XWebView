using System;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;
using IRO.XWebView.Core.Consts;

namespace IRO.XWebView.CefSharp.Containers
{
    public class SelfCefSharpContainer : ICefSharpContainer
    {
        XWebViewVisibility _alwaysReturnVisibility;

        public SelfCefSharpContainer(IWebBrowser currentBrowser, XWebViewVisibility alwaysReturnVisibility = XWebViewVisibility.Hidden)
        {
            _alwaysReturnVisibility = alwaysReturnVisibility;
            CurrentBrowser = currentBrowser ?? throw new ArgumentNullException(nameof(currentBrowser));
        }

        public bool IsDisposed { get; private set; }

        public IWebBrowser CurrentBrowser { get; private set; }

        public bool CanSetVisibility { get; } = false;

        public void SetVisibilityState(XWebViewVisibility visibility)
        {
            throw new NotImplementedException();
        }

        public XWebViewVisibility GetVisibilityState() => _alwaysReturnVisibility;

        public async Task WaitWebViewInitialized()
        {
            while (!CurrentBrowser.IsBrowserInitialized)
            {
                await Task.Delay(10).ConfigureAwait(false);
            }
        }

        public async Task Wrapped(CefSharpXWebView xwv)
        {
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;
            IsDisposed = true;
            try
            {
                if (!CurrentBrowser.IsDisposed)
                {
                    CurrentBrowser.Dispose();
                }
            }
            catch { }
            CurrentBrowser = null;
        }
    }
}