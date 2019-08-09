using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.Views;
using Android.Webkit;

namespace IRO.XWebView.Droid.BrowserClients
{
    public class WebChromeClientEventsProxy : IWebChromeClientEventsProxy
    {
        public event OnCreateWindow OnCreateWindow;

        public event OnExceededDatabaseQuota OnExceededDatabaseQuota;

        public event OnGeolocationPermissionsHidePrompt OnGeolocationPermissionsHidePrompt;

        public event OnGeolocationPermissionsShowPrompt OnGeolocationPermissionsShowPrompt;

        public event OnHideCustomView OnHideCustomView;

        public event OnJsAlert OnJsAlert;

        public event OnJsBeforeUnload OnJsBeforeUnload;

        public event OnJsConfirm OnJsConfirm;

        public event OnJsPrompt OnJsPrompt;

        public event OnJsTimeout OnJsTimeout;

        public event OnPermissionRequest OnPermissionRequest;

        public event OnPermissionRequestCanceled OnPermissionRequestCanceled;

        public event OnProgressChanged OnProgressChanged;

        public event OnReachedMaxAppCacheSize OnReachedMaxAppCacheSize;

        public event OnReceivedIcon OnReceivedIcon;

        public event OnReceivedTitle OnReceivedTitle;

        public event OnReceivedTouchIconUrl OnReceivedTouchIconUrl;

        public event OnRequestFocus OnRequestFocus;

        public event OnShowCustomView OnShowCustomView;

        public event OnShowCustomView2 OnShowCustomView2;

        public event OnShowFileChooser OnShowFileChooser;

        internal bool? RiseOnCreateWindow(WebView view, bool isdialog, bool isusergesture, Android.OS.Message resultmsg)
        {
            return OnCreateWindow?.Invoke(view, isdialog, isusergesture, resultmsg);
        }

        internal void RiseOnExceededDatabaseQuota(string url, string databaseidentifier, long quota, long estimateddatabasesize, long totalquota, WebStorage.IQuotaUpdater quotaupdater)
        {
            OnExceededDatabaseQuota?.Invoke(url, databaseidentifier, quota, estimateddatabasesize, totalquota,
                quotaupdater);
        }

        internal void RiseOnGeolocationPermissionsHidePrompt()
        {
            OnGeolocationPermissionsHidePrompt?.Invoke();
        }

        internal void RiseOnGeolocationPermissionsShowPrompt(string origin, GeolocationPermissions.ICallback callback)
        {
            OnGeolocationPermissionsShowPrompt?.Invoke(origin, callback);
        }

        internal void RiseOnHideCustomView()
        {
            OnHideCustomView?.Invoke();
        }

        internal bool? RiseOnJsAlert(WebView view, string url, string message, JsResult result)
        {
            return OnJsAlert?.Invoke(view, url, message, result);
        }

        internal bool? RiseOnJsBeforeUnload(WebView view, string url, string message, JsResult result)
        {
            return OnJsBeforeUnload?.Invoke(view, url, message, result);
        }

        internal bool? RiseOnJsConfirm(WebView view, string url, string message, JsResult result)
        {
            return OnJsConfirm?.Invoke(view, url, message, result);
        }

        internal bool? RiseOnJsPrompt(WebView view, string url, string message, string defaultvalue, JsPromptResult result)
        {
            return OnJsPrompt?.Invoke(view, url, message, defaultvalue, result);
        }

        internal bool? RiseOnJsTimeout()
        {
            return OnJsTimeout?.Invoke();
        }

        internal void RiseOnPermissionRequest(PermissionRequest request)
        {
            OnPermissionRequest?.Invoke(request);
        }

        internal void RiseOnPermissionRequestCanceled(PermissionRequest request)
        {
            OnPermissionRequestCanceled?.Invoke(request);
        }

        internal void RiseOnProgressChanged(WebView view, int newprogress)
        {
            OnProgressChanged?.Invoke(view, newprogress);
        }

        internal void RiseOnReachedMaxAppCacheSize(long requiredstorage, long quota, WebStorage.IQuotaUpdater quotaupdater)
        {
            OnReachedMaxAppCacheSize?.Invoke(requiredstorage, quota, quotaupdater);
        }

        internal void RiseOnReceivedIcon(WebView view, Bitmap icon)
        {
            OnReceivedIcon?.Invoke(view, icon);
        }

        internal void RiseOnReceivedTitle(WebView view, string title)
        {
            OnReceivedTitle?.Invoke(view, title);
        }

        internal void RiseOnReceivedTouchIconUrl(WebView view, string url, bool precomposed)
        {
            OnReceivedTouchIconUrl?.Invoke(view, url, precomposed);
        }

        internal void RiseOnRequestFocus(WebView view)
        {
            OnRequestFocus?.Invoke(view);
        }

        internal void RiseOnShowCustomView(View view, Android.Webkit.WebChromeClient.ICustomViewCallback callback)
        {
            OnShowCustomView?.Invoke(view, callback);
        }

        internal void RiseOnShowCustomView2(View view, ScreenOrientation requestedorientation, Android.Webkit.WebChromeClient.ICustomViewCallback callback)
        {
            OnShowCustomView2?.Invoke(view, requestedorientation, callback);
        }

        internal bool? RiseOnShowFileChooser(WebView webview, IValueCallback filepathcallback, Android.Webkit.WebChromeClient.FileChooserParams filechooserparams)
        {
            return OnShowFileChooser?.Invoke(webview, filepathcallback, filechooserparams);
        }
    }
}