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
        CefSharpXWebViewControl _cefSharpXWebViewControl;

        public IWebBrowser CurrentBrowser => _cefSharpXWebViewControl.CurrentBrowser;

        public bool CanSetVisibility => true;

        public new EventHandler Disposed;

        public new bool IsDisposed { get; private set; }

        public CefSharpXWebViewForm()
        {
            InitializeComponent();
            _cefSharpXWebViewControl = new CefSharpXWebViewControl();
            Controls.Add(_cefSharpXWebViewControl);
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
