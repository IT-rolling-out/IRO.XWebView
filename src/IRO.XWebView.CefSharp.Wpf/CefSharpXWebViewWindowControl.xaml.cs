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
using IRO.XWebView.CefSharp.Containers;
using IRO.XWebView.CefSharp.Utils;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Utils;

namespace IRO.XWebView.CefSharp.Wpf
{
    /// <summary>
    /// Interaction logic for CefSharpXWebViewWindowControl.xaml
    /// </summary>
    public partial class CefSharpXWebViewWindowControl : UserControl, ICefSharpContainer
    {
        ChromiumWebBrowser _chromiumWebBrowser;

        protected CefSharpXWebView XWV { get; set; }

        public event EventHandler Disposed;

        public bool IsDisposed { get; private set; }

        public IWebBrowser CurrentBrowser => _chromiumWebBrowser;

        public bool CanSetVisibility => true;

        public CefSharpXWebViewWindowControl()
        {
            Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
            InitializeComponent();
            _chromiumWebBrowser = new ChromiumWebBrowser("about:blank");
            this.Content = _chromiumWebBrowser;
            Focus();
            this.Unloaded += delegate { Dispose(); };
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
            ThreadSync.Inst.TryInvoke(() =>
            {
                _chromiumWebBrowser.Dispose();
                _chromiumWebBrowser = null;
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
        public virtual async Task Wrapped(CefSharpXWebView xwv)
        {
        }

        /// <summary>
        /// Return <see cref="CefSharpXWebView"/> for those control or wait while it initialized.
        /// Note that inititalization can't be finished when control is on screen.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<CefSharpXWebView> GetXWebView()
        {
            if (XWV == null)
            {
                XWV = await CefSharpXWebView.Create(this);
                SetVisibilityState(XWebViewVisibility.Hidden);
            }
            await CurrentBrowser.WaitInitialization();
            return XWV;
        }
    }
}
