namespace IRO.ImprovedWebView.Core.EventsAndDelegates
{
    public class LoadStartedEventArgs : LoadEventArgs
    {
        /// <summary>
        /// If you set true - content will not be loaded by browser.
        /// </summary>
        public bool Handled { get; set; }
    }
}