using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;
using IRO.XWebView.CefSharp.BrowserClients;
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
        CefSharpXWebViewControl _cefSharpXWebViewControl;

        public event EventHandler Disposed;

        public bool IsDisposed { get; private set; }

        public IWebBrowser CurrentBrowser => _cefSharpXWebViewControl.CurrentBrowser;

        public bool CanSetVisibility => true;

        public CefSharpXWebViewWindow()
        {
            Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
            InitializeComponent();
            Focus();
            _cefSharpXWebViewControl = new CefSharpXWebViewControl();
            Content = _cefSharpXWebViewControl;
            Closed += delegate { Dispose(); };
        }

        public virtual void SetVisibilityState(XWebViewVisibility visibility)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
            XWebViewThreadSync.Inst.Invoke(() =>
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
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
            return XWebViewThreadSync.Inst.Invoke(() =>
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
            XWebViewThreadSync.Inst.TryInvoke(() =>
            {
                _cefSharpXWebViewControl.Dispose();
                Content = null;
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
        public virtual void Wrapped(CefSharpXWebView xwv)
        {
            _cefSharpXWebViewControl.Wrapped(xwv);
        }
    }
}
