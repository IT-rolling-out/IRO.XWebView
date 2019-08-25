using System;
using System.Windows;
using System.Windows.Threading;
using IRO.XWebView.Core.Utils;

namespace IRO.XWebView.CefSharp.Wpf.Utils
{
    public class WpfThreadSyncInvoker:IThreadSyncInvoker
    {
        Dispatcher _dispatcher;

        public WpfThreadSyncInvoker(Dispatcher dispatcher = null)
        {
            _dispatcher = dispatcher ?? Application.Current.Dispatcher;
        }

        public WpfThreadSyncInvoker() : this(null) {}

        public void Invoke(Action act)
        {
            _dispatcher.Invoke(act);
        }

        public void InvokeAsync(Action act)
        {
            _dispatcher.InvokeAsync(act);
        }
    }
}
