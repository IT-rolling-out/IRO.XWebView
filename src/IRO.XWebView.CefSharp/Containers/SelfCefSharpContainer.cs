using System;
using System.Threading.Tasks;
using CefSharp.OffScreen;
using IRO.XWebView.CefSharp;
using IRO.XWebView.Core.Consts;

namespace IRO.XWebView.Droid.Containers
{
    public class SelfCefSharpContainer : ICefSharpContainer
    {
        public SelfCefSharpContainer(ChromiumWebBrowser currentBrowser)
        {
            CurrentBrowser = currentBrowser ?? throw new ArgumentNullException(nameof(currentBrowser));
        }

        public bool IsDisposed { get; private set; }

        public ChromiumWebBrowser CurrentBrowser { get; private set; }

        public bool CanSetVisibility { get; } = false;

        public void ToggleVisibilityState(XWebViewVisibility visibility)
        {
            throw new NotImplementedException();
        }

        public Task WaitWebViewInitialized()
        {
            throw new NotImplementedException();
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