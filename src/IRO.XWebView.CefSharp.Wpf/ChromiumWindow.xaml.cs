using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;
using IRO.XWebView.CefSharp.Containers;
using IRO.XWebView.CefSharp.Utils;
using IRO.XWebView.CefSharp.Wpf.Utils;
using IRO.XWebView.Core.Consts;

namespace IRO.XWebView.CefSharp.Wpf
{
    /// <summary>
    /// Interaction logic for ChromiumWindow.xaml
    /// </summary>
    public partial class ChromiumWindow : Window, ICefSharpContainer
    {
        readonly ChromiumWebBrowser _chromiumWebBrowser;

        public bool IsDisposed { get; private set; }

        public IWebBrowser CurrentBrowser => _chromiumWebBrowser;

        public bool CanSetVisibility => true;
        
        public ChromiumWindow()
        {
            InitializeComponent();
            ChromiumContainer.Content = _chromiumWebBrowser = new ChromiumWebBrowser("about:blank");
            Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
        }

        public void SetVisibilityState(XWebViewVisibility visibility)
        {
            ThreadSync.Inst.Invoke(() =>
            {
                if (visibility == XWebViewVisibility.Visible)
                {
                    Visibility = Visibility.Visible;
                }
                Visibility = Visibility.Hidden;
            });

        }

        public XWebViewVisibility GetVisibilityState()
        {
            return ThreadSync.Inst.Invoke(() =>
            {
                if (Visibility == Visibility.Visible)
                {
                    return XWebViewVisibility.Visible;
                }
                return XWebViewVisibility.Hidden;
            }, Dispatcher);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            if (IsDisposed)
                return;
            ThreadSync.Inst.TryInvoke(CurrentBrowser.Dispose);
            ThreadSync.Inst.TryInvoke(Close, Dispatcher);
            IsDisposed = true;
        }


        /// <summary>
        /// Wait while native WebView controll initializing.
        /// </summary>
        /// <returns></returns>
        public async Task WaitWebViewInitialized()
        {
            await WpfCefHelpers.WaitInitialization(_chromiumWebBrowser);
        }

        /// <summary>
        /// Used for initializations that require <see cref="CefSharpXWebView"/>.
        /// Sometimes your visual container need access to events or some methods of XWebView.
        /// </summary>
        /// <param name="xwv"></param>
        /// <returns></returns>
        public async Task Wrapped(CefSharpXWebView xwv)
        {
        }
    }
}
