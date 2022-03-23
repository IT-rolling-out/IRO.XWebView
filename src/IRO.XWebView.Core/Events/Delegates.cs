using System.Threading.Tasks;

namespace IRO.XWebView.Core.Events
{
    #region With async support
    public delegate Task LoadStartedDelegate(object sender, LoadStartedEventArgs args);

    public delegate Task LoadFinishedDelegate(object sender, LoadFinishedEventArgs args);

    public delegate Task GoBackDelegate(object sender, GoBackEventArgs args);

    public delegate Task GoForwardDelegate(object sender, GoForwardEventArgs args);
    #endregion

    #region Only sync
    public delegate void LoadStartedDelegate_Sync(object sender, LoadStartedEventArgs args);

    public delegate void LoadFinishedDelegate_Sync(object sender, LoadFinishedEventArgs args);

    public delegate void GoBackDelegate_Sync(object sender, GoBackEventArgs args);

    public delegate void GoForwardDelegate_Sync(object sender, GoForwardEventArgs args);
    #endregion
}