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
        ChromiumWebBrowser _chromiumWebBrowser;

        public IWebBrowser CurrentBrowser => _chromiumWebBrowser;

        public bool CanSetVisibility => true;

        public new EventHandler Disposed;

        public new bool IsDisposed { get; private set; }

        public CefSharpXWebViewControl()
        {
            Dock = DockStyle.Fill;
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }
            _chromiumWebBrowser = new ChromiumWebBrowser("about:blank");
            _chromiumWebBrowser.Dock = DockStyle.Fill;
            Controls.Add(_chromiumWebBrowser);
            Focus();

            base.Disposed += (s, e) =>
            {
                IsDisposed = true;
                Disposed?.Invoke(s, e);
            };

        }

        public virtual void SetVisibilityState(XWebViewVisibility visibility)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
            XWebViewThreadSync.Inst.Invoke(() =>
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
            return XWebViewThreadSync.Inst.Invoke(() =>
            {
                if (Visible)
                {
                    return XWebViewVisibility.Visible;
                }
                return XWebViewVisibility.Hidden;
            });
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
