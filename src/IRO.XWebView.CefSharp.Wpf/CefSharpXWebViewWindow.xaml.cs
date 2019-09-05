using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;
using IRO.XWebView.CefSharp.Containers;
using IRO.XWebView.CefSharp.Utils;
using IRO.XWebView.CefSharp.Wpf.Utils;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Utils;

namespace IRO.XWebView.CefSharp.Wpf
{
    /// <summary>
    /// Interaction logic for CefSharpXWebViewWindow.xaml
    /// </summary>
    public partial class CefSharpXWebViewWindow : Window, ICefSharpContainer
    {
        ChromiumWebBrowser _chromiumWebBrowser;

        public event EventHandler Disposed;

        public bool IsDisposed { get; private set; }

        public IWebBrowser CurrentBrowser => _chromiumWebBrowser;

        public bool CanSetVisibility => true;

        public CefSharpXWebViewWindow()
        {
            Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
            InitializeComponent();
            
            _chromiumWebBrowser = new ChromiumWebBrowser("about:blank");
            ChromiumContainer.Content = _chromiumWebBrowser;
            Focus();
            Closed += delegate { Dispose(); };
        }

        public virtual void SetVisibilityState(XWebViewVisibility visibility)
        {
            ThreadSync.Inst.Invoke(() =>
            {
                if (visibility == XWebViewVisibility.Visible)
                {
                    Visibility = Visibility.Visible;
                }
                else
                {
                    Visibility = Visibility.Hidden;
                }
            });

        }

        public virtual XWebViewVisibility GetVisibilityState()
        {
            return ThreadSync.Inst.Invoke(() =>
            {
                if (Visibility == Visibility.Visible)
                {
                    return XWebViewVisibility.Visible;
                }
                return XWebViewVisibility.Hidden;
            });
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public virtual void Dispose()
        {
            if (IsDisposed)
                return;
            ThreadSync.Inst.TryInvoke(()=>
            {
                _chromiumWebBrowser.Dispose();
                _chromiumWebBrowser = null;
                ChromiumContainer.Content = null;
                Close();
            });
            IsDisposed = true;
            Disposed?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Used for initializations that require <see cref="CefSharpXWebView"/>.
        /// Sometimes your visual container need access to events or some methods of XWebView.
        /// </summary>
        /// <param name="xwv"></param>
        /// <returns></returns>
        public virtual async Task Wrapped(CefSharpXWebView xwv)
        {
        }

        public void Configure(Action<IBrowserSettings, RequestContextSettings> action)
        {
            _configAct = action;
        }

        public static CefSharpXWebViewWindow Create(Action<IBrowserSettings, RequestContextSettings> configAction=null)
        {
            CefHelpers.InitializeCefIfNot(new CefSettings());
            return ThreadSync.Inst.Invoke(() =>
            {
                var chromiumWindow = new CefSharpXWebViewWindow();
                var br = (ChromiumWebBrowser)chromiumWindow.CurrentBrowser;
                br.BrowserSettings ??= new BrowserSettings();
                var requestContextSettings = new RequestContextSettings();
                configAction?.Invoke(br.BrowserSettings, requestContextSettings);
                br.RequestContext = new RequestContext(requestContextSettings);
                return chromiumWindow;
            });

        }
    }
}
