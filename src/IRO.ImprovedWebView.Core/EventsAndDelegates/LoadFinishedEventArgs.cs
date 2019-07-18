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
        /// If true - content was not loaded by browser automatically.
        /// </summary>
        public bool WasHandled { get; set; }
    }
}