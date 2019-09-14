using System;
using System.ComponentModel;
using System.Windows.Forms;
using CefSharp;
using CefSharp.Web;
using CefSharp.WinForms;
using IRO.XWebView.CefSharp.Containers;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Utils;

namespace IRO.XWebView.CefSharp.WinForms
{
    public partial class CefSharpXWebViewControl : UserControl, ICefSharpContainer
    {
        bool _thisDisposed;

        ChromiumWebBrowser _chromiumWebBrowser;

        public IWebBrowser CurrentBrowser => _chromiumWebBrowser;

        public bool CanSetVisibility => true;

        public CefSharpXWebViewControl()
        {
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }
            _chromiumWebBrowser = new ChromiumWebBrowser(new HtmlString("about:blank"));
            Controls.Add(_chromiumWebBrowser);
            Focus();
            base.Disposed += delegate { this.Dispose(); };
        }

        public virtual void SetVisibilityState(XWebViewVisibility visibility)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
            ThreadSync.Inst.Invoke(() =>
            {
                if (visibility == XWebViewVisibility.Visible)
                {
                    Visible = true;
                }
                else
                {
                    Visible = false;
                }
            });
        }

        public virtual XWebViewVisibility GetVisibilityState()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
            return ThreadSync.Inst.Invoke(() =>
            {
                if (Visible)
                {
                    return XWebViewVisibility.Visible;
                }
                return XWebViewVisibility.Hidden;
            });
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public new virtual void Dispose()
        {
            if (_thisDisposed)
                return;
            ThreadSync.Inst.TryInvoke(() =>
            {
                _chromiumWebBrowser.Dispose();
                Controls.Clear();
            });
            _thisDisposed = true;
            if (!IsDisposed)
                base.Dispose();
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
