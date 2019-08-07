namespace IRO.XWebView.Core.EventsAndDelegates
{
    public class LoadFinishedEventArgs : LoadEventArgs
    {
        /// <summary>
        /// Только если ошибка подключения.
        /// </summary>
        public bool IsError { get; set; }

        public string ErrorDescription { get; set; }

        /// <summary>
        /// Short desciption of error. Can be special for webview on each platform.
        /// </summary>
        public string ErrorType { get; set; }

        /// <summary>
        /// Throw exception on true.
        /// </summary>
        public bool WasCancelled { get; set; }
    }
}