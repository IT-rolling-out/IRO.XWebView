﻿namespace IRO.XWebView.Core.Events
{
    public class LoadStartedEventArgs : LoadEventArgs
    {
        /// <summary>
        /// If you set true - content will not be loaded by browser.
        /// LoadUrl task will be cancelled.
        /// </summary>
        public bool Cancel { get; set; }
    }
}