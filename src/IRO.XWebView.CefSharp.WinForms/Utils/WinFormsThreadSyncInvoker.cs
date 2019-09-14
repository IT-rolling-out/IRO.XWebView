using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using IRO.Threading;

namespace IRO.XWebView.CefSharp.WinForms.Utils
{
    public class WinFormsThreadSyncInvoker : IThreadSyncInvoker
    {
        static Form _form;

        public WinFormsThreadSyncInvoker()
        {
            if (_form != null)
                return;
            var form = new Form();
            var thread = new Thread(() =>
            {

                form.FormBorderStyle = FormBorderStyle.None;
                form.ShowInTaskbar = false;
                form.Load += delegate { form.Size = new Size(0, 0); };
                Application.Run(form);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
            _form = form;
        }

        public void Invoke(Action act)
        {
            _form.Invoke(act);
        }

        public void InvokeAsync(Action act)
        {
            _form.BeginInvoke(act);
        }
    }
}
