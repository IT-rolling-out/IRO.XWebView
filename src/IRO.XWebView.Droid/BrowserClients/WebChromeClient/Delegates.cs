using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Webkit;

namespace IRO.XWebView.Droid.BrowserClients
{
    public delegate void GetVisitedHistory(IValueCallback callback);

    public delegate void OnCloseWindow(WebView window);


    public delegate bool OnConsoleMessage(ConsoleMessage consoleMessage);

    public delegate void OnConsoleMessage2(string message, int lineNumber, string sourceID);

    public delegate bool OnCreateWindow(
      WebView view,
      bool isDialog,
      bool isUserGesture,
      Android.OS.Message resultMsg);

    public delegate void OnExceededDatabaseQuota(
          string url,
          string databaseIdentifier,
          long quota,
          long estimatedDatabaseSize,
          long totalQuota,
          WebStorage.IQuotaUpdater quotaUpdater);

    public delegate void OnGeolocationPermissionsHidePrompt();

    public delegate void OnGeolocationPermissionsShowPrompt(
          string origin,
          GeolocationPermissions.ICallback callback);

    public delegate void OnHideCustomView();

    public delegate bool? OnJsAlert(WebView view, string url, string message, JsResult result);

    public delegate bool? OnJsBeforeUnload(
      WebView view,
      string url,
      string message,
      JsResult result);

    public delegate bool? OnJsConfirm(WebView view, string url, string message, JsResult result);

    public delegate bool? OnJsPrompt(
      WebView view,
      string url,
      string message,
      string defaultValue,
      JsPromptResult result);

    public delegate bool? OnJsTimeout();

    public delegate void OnPermissionRequest(PermissionRequest request);

    public delegate void OnPermissionRequestCanceled(PermissionRequest request);

    public delegate void OnProgressChanged(WebView view, int newProgress);

    public delegate void OnReachedMaxAppCacheSize(
      long requiredStorage,
      long quota,
      WebStorage.IQuotaUpdater quotaUpdater);

    public delegate void OnReceivedIcon(WebView view, Bitmap icon);

    public delegate void OnReceivedTitle(WebView view, string title);

    public delegate void OnReceivedTouchIconUrl(WebView view, string url, bool precomposed);

    public delegate void OnRequestFocus(WebView view);

    public delegate void OnShowCustomView(View view, Android.Webkit.WebChromeClient.ICustomViewCallback callback);

    public delegate void OnShowCustomView2(
      View view,
      [GeneratedEnum] ScreenOrientation requestedOrientation,
      Android.Webkit.WebChromeClient.ICustomViewCallback callback);

    public delegate bool? OnShowFileChooser(
         WebView webView,
         IValueCallback filePathCallback,
         Android.Webkit.WebChromeClient.FileChooserParams fileChooserParams);
}