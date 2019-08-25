using System;

namespace IRO.XWebView.Core.Utils
{
    public interface IThreadSyncInvoker
    {
        void Invoke(Action act);

        void InvokeAsync(Action act);
    }
}