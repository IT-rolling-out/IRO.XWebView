using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using IRO.Threading;

namespace IRO.XWebView.CefSharp.WinForms.Utils
{
    public class WinFormsThreadSyncInvoker : IThreadSyncInvoker
    {
        readonly Form _form;

        public WinFormsThreadSyncInvoker(Form mainForm)
        {
            _form = mainForm;
        }

        public void Invoke(Action act)
        {
            if (!_form.InvokeRequired)
            {
                act();
                return;
            }
            _form.Invoke(act);
        }

        public void InvokeAsync(Action act)
        {
            if (!_form.InvokeRequired)
            {
                act();
                return;
            }
            //var thread = new Thread((obj) =>
            //{
            //    _form.Invoke(act);
            //});
            //thread.Start();
            _form.BeginInvoke(act);
        }
    }
}
