using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using IRO.XWebView.CefSharp.Containers;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Utils;

namespace IRO.XWebView.CefSharp.WinForms
{
    public partial class CefSharpXWebViewForm : Form, ICefSharpContainer
    {
        bool _thisDisposed;

        CefSharpXWebViewControl _cefSharpXWebViewControl;

        public IWebBrowser CurrentBrowser => _cefSharpXWebViewControl.CurrentBrowser;

        public bool CanSetVisibility => true;

        public CefSharpXWebViewForm()
        {
            InitializeComponent();
            _cefSharpXWebViewControl = new CefSharpXWebViewControl();
            Controls.Add(_cefSharpXWebViewControl);
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
                _cefSharpXWebViewControl.Dispose();
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
            _cefSharpXWebViewControl.Wrapped(xwv);
        }
    }
}
