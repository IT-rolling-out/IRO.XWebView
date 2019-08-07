using System;

namespace IRO.XWebView.Core.EventsAndDelegates
{
    public class GoForwardEventArgs : EventArgs
    {
        /// <summary>
        /// Set it to true, to disable default handling.
        /// </summary>
        public bool Cancel { get; set; }

        public bool CanGoForward { get; set; }
    }
}