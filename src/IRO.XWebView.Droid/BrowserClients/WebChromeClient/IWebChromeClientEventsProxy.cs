namespace IRO.XWebView.Droid.BrowserClients
{
    public interface IWebChromeClientEventsProxy
    {
        event OnCreateWindow OnCreateWindow;
        event OnExceededDatabaseQuota OnExceededDatabaseQuota;
        event OnGeolocationPermissionsHidePrompt OnGeolocationPermissionsHidePrompt;
        event OnGeolocationPermissionsShowPrompt OnGeolocationPermissionsShowPrompt;
        event OnHideCustomView OnHideCustomView;
        event OnJsAlert OnJsAlert;
        event OnJsBeforeUnload OnJsBeforeUnload;
        event OnJsConfirm OnJsConfirm;
        event OnJsPrompt OnJsPrompt;
        event OnJsTimeout OnJsTimeout;
        event OnPermissionRequest OnPermissionRequest;
        event OnPermissionRequestCanceled OnPermissionRequestCanceled;
        event OnProgressChanged OnProgressChanged;
        event OnReachedMaxAppCacheSize OnReachedMaxAppCacheSize;
        event OnReceivedIcon OnReceivedIcon;
        event OnReceivedTitle OnReceivedTitle;
        event OnReceivedTouchIconUrl OnReceivedTouchIconUrl;
        event OnRequestFocus OnRequestFocus;
        event OnShowCustomView OnShowCustomView;
        event OnShowCustomView2 OnShowCustomView2;
        event OnShowFileChooser OnShowFileChooser;
    }
}