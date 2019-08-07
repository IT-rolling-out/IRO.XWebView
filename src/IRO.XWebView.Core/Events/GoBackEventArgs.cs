using System;
using System.Threading.Tasks;

namespace IRO.XWebView.Core.EventsAndDelegates
{
    public class GoBackEventArgs : EventArgs
    {
        /// <summary>
        /// Set it to true, to disable default handling.
        /// Will throw <see cref="TaskCanceledException"/> to GoForward call if true.
        /// </summary>
        public bool Cancel { get; set; }

        public bool CanGoBack { get; set; }
    }
}