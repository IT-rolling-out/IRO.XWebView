using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp;
using CefSharp.Wpf;
using IRO.XWebView.CefSharp.BrowserClients;
using IRO.XWebView.CefSharp.Containers;
using IRO.XWebView.CefSharp.Utils;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Utils;

namespace IRO.XWebView.CefSharp.Wpf
{
    /// <summary>
    /// Interaction logic for CefSharpXWebViewControl.xaml
    /// </summary>
    public partial class CefSharpXWebViewControl : UserControl, ICefSharpContainer
    {
        ChromiumWebBrowser _chromiumWebBrowser;

        public event EventHandler Disposed;

        public bool IsDisposed { get; private set; }

        public IWebBrowser CurrentBrowser => _chromiumWebBrowser;

        public bool CanSetVisibility => true;

        public CefSharpXWebViewControl()
        {
            Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
            InitializeComponent();
            _chromiumWebBrowser = new ChromiumWebBrowser("about:blank");
            Content = _chromiumWebBrowser;
            Focus();
            Unloaded += delegate { Dispose(); };
        }

        public virtual void SetVisibilityState(XWebViewVisibility visibility)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
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
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
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
            ThreadSync.Inst.TryInvoke(() =>
            {
                _chromiumWebBrowser.Dispose();
                this.Content = null;
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
        }
    }
}
