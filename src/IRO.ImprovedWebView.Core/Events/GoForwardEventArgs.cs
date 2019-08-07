using System;

namespace IRO.ImprovedWebView.Core.EventsAndDelegates
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