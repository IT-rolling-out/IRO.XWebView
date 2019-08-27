using System;
using System.Collections.Generic;
using System.Text;
using CefSharp;
using IRO.Threading;
using IRO.XWebView.Core.Utils;

namespace IRO.XWebView.CefSharp.OffScreen.Utils
{
    /// <summary>
    /// Please not use it for wpf projects.
    /// </summary>
    public class CefSharpThreadSyncInvoker : IThreadSyncInvoker
    {
        public void Invoke(Action act)
        {
            Cef.UIThreadTaskFactory.StartNew(act).Wait();
        }

        public void InvokeAsync(Action act)
        {
            Cef.UIThreadTaskFactory.StartNew(act);
        }
    }
}
