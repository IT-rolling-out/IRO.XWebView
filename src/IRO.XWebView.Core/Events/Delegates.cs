namespace IRO.XWebView.Core.Events
{
    public delegate void LoadStartedDelegate(object sender, LoadStartedEventArgs args);

    public delegate void LoadFinishedDelegate(object sender, LoadFinishedEventArgs args);

    public delegate void GoBackDelegate(object sender, GoBackEventArgs args);

    public delegate void GoForwardDelegate(object sender, GoForwardEventArgs args);
}