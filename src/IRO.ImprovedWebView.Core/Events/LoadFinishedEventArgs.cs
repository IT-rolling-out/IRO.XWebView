namespace IRO.ImprovedWebView.Core.EventsAndDelegates
{
    public class LoadFinishedEventArgs : LoadEventArgs
    {
        /// <summary>
        /// Только если ошибка подключения.
        /// </summary>
        public bool IsError { get; set; }

        public string ErrorDescription { get; set; }

        /// <summary>
        /// Throw exception on true.
        /// </summary>
        public bool WasCancelled { get; set; }
    }
}