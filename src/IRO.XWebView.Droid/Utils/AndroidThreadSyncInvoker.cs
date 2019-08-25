using System;
using Android.App;
using IRO.XWebView.Core.Utils;

namespace IRO.XWebView.Droid.Utils
{
    public class AndroidThreadSyncInvoker : IThreadSyncInvoker
    {
        public void Invoke(Action act)
        {
            Application.SynchronizationContext.Send((obj) => { act(); }, null);
        }

        public void InvokeAsync(Action act)
        {
            Application.SynchronizationContext.Post((obj) => { act(); }, null);
        }
    }
}