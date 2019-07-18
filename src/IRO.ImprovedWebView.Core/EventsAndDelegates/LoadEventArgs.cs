using System;

namespace IRO.ImprovedWebView.Core.EventsAndDelegates
{
    public class LoadEventArgs:EventArgs
    {
        public string Url { get; set; }
    }
}