using System;

namespace IRO.ImprovedWebView.Core.EventsAndDelegates
{
    public class GoBackEventArgs : EventArgs
    {
        /// <summary>
        /// Set it to true, to disable default handling.
        /// </summary>
        public bool Cancel { get; set; }

        public bool CanGoBack { get; set; }
    }
}