using System;
using System.Windows;
using System.Windows.Threading;
using IRO.XWebView.Core.Utils;

namespace IRO.XWebView.CefSharp.Wpf.Utils
{
    public class ThreadSyncInvoker:IThreadSyncInvoker
    {
        Dispatcher _dispatcher;

        public ThreadSyncInvoker(Dispatcher dispatcher=null)
        {
            _dispatcher = dispatcher ?? Application.Current.Dispatcher;
        }

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
