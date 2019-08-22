using System;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.Views;
using Android.Webkit;

namespace IRO.XWebView.Droid.BrowserClients
{
    public class CustomWebChromeClient : WebChromeClient
    {
        public WebChromeClientEventsProxy EventsProxy { get; } = new WebChromeClientEventsProxy();

        public override void GetVisitedHistory(IValueCallback callback)
        {
            base.GetVisitedHistory(callback);
            EventsProxy.RiseGetVisitedHistory(callback);
        }

        public override bool OnCreateWindow(WebView view, bool isdialog, bool isusergesture, Android.OS.Message resultmsg)
        {
            var resBase = base.OnCreateWindow(view, isdialog, isusergesture, resultmsg);
            var res = EventsProxy.RiseOnCreateWindow(view, isdialog, isusergesture, resultmsg);
            return res ?? resBase;
        }

        [Obsolete("deprecated")]
        public override void OnExceededDatabaseQuota(string url, string databaseidentifier, long quota, long estimateddatabasesize, long totalquota, WebStorage.IQuotaUpdater quotaupdater)
        {
            base.OnExceededDatabaseQuota(url, databaseidentifier, quota, estimateddatabasesize, totalquota,
                quotaupdater);
            EventsProxy.RiseOnExceededDatabaseQuota(url, databaseidentifier, quota, estimateddatabasesize, totalquota,
                quotaupdater);
        }

        public override void OnGeolocationPermissionsHidePrompt()
        {
            base.OnGeolocationPermissionsHidePrompt();
            EventsProxy.RiseOnGeolocationPermissionsHidePrompt();
        }

        public override void OnGeolocationPermissionsShowPrompt(string origin, GeolocationPermissions.ICallback callback)
        {
            base.OnGeolocationPermissionsShowPrompt(origin, callback);
            EventsProxy.RiseOnGeolocationPermissionsShowPrompt(origin, callback);
        }

        public override void OnHideCustomView()
        {
            base.OnHideCustomView();
            EventsProxy.RiseOnHideCustomView();
        }

        public override bool OnJsAlert(WebView view, string url, string message, JsResult result)
        {
            var resBase =  base.OnJsAlert(view, url, message, result);
            var res = EventsProxy.RiseOnJsAlert(view, url, message, result);
            return res ?? resBase;
        }

        public override bool OnJsBeforeUnload(WebView view, string url, string message, JsResult result)
        {
            var resBase =  base.OnJsBeforeUnload(view, url, message, result);
            var res = EventsProxy.RiseOnJsBeforeUnload(view, url, message, result);
            return res ?? resBase;
        }

        public override bool OnJsConfirm(WebView view, string url, string message, JsResult result)
        {
            var resBase =  base.OnJsConfirm(view, url, message, result);
            var res = EventsProxy.RiseOnJsConfirm(view, url, message, result);
            return res ?? resBase;
        }

        public override bool OnJsPrompt(WebView view, string url, string message, string defaultvalue, JsPromptResult result)
        {
            var resBase =  base.OnJsPrompt(view, url, message, defaultvalue, result);
            var res = EventsProxy.RiseOnJsPrompt(view, url, message, defaultvalue, result);
            return res ?? resBase;
        }

        [Obsolete("deprecated")]
        public override bool OnJsTimeout()
        {
            var resBase =  base.OnJsTimeout();
            var res = EventsProxy.RiseOnJsTimeout();
            return res ?? resBase;
        }

        public override void OnPermissionRequest(PermissionRequest request)
        {
            base.OnPermissionRequest(request);
            EventsProxy.RiseOnPermissionRequest(request);
        }

        public override void OnPermissionRequestCanceled(PermissionRequest request)
        {
            base.OnPermissionRequestCanceled(request);
            EventsProxy.RiseOnPermissionRequestCanceled(request);
        }

        public override void OnProgressChanged(WebView view, int newprogress)
        {
            base.OnProgressChanged(view, newprogress);
            EventsProxy.RiseOnProgressChanged(view, newprogress);
        }

        [Obsolete("deprecated")]
        public override void OnReachedMaxAppCacheSize(long requiredstorage, long quota, WebStorage.IQuotaUpdater quotaupdater)
        {
            base.OnReachedMaxAppCacheSize(requiredstorage, quota, quotaupdater);
            EventsProxy.RiseOnReachedMaxAppCacheSize(requiredstorage, quota, quotaupdater);
        }

        public override void OnReceivedIcon(WebView view, Bitmap icon)
        {
            base.OnReceivedIcon(view, icon);
            EventsProxy.RiseOnReceivedIcon(view, icon);
        }

        public override void OnReceivedTitle(WebView view, string title)
        {
            base.OnReceivedTitle(view, title);
            EventsProxy.RiseOnReceivedTitle(view, title);
        }

        public override void OnReceivedTouchIconUrl(WebView view, string url, bool precomposed)
        {
            base.OnReceivedTouchIconUrl(view, url, precomposed);
            EventsProxy.RiseOnReceivedTouchIconUrl(view, url, precomposed);
        }

        public override void OnRequestFocus(WebView view)
        {
            base.OnRequestFocus(view);
            EventsProxy.RiseOnRequestFocus(view);
        }

        public override void OnShowCustomView(View view, Android.Webkit.WebChromeClient.ICustomViewCallback callback)
        {
            base.OnShowCustomView(view, callback);
            EventsProxy.RiseOnShowCustomView(view, callback);
        }

        [Obsolete("deprecated")]
        public override void OnShowCustomView(View view, ScreenOrientation requestedorientation, Android.Webkit.WebChromeClient.ICustomViewCallback callback)
        {
            base.OnShowCustomView(view, requestedorientation, callback);
            EventsProxy.RiseOnShowCustomView2(view, requestedorientation, callback);
        }

        public override bool OnShowFileChooser(WebView webview, IValueCallback filepathcallback, Android.Webkit.WebChromeClient.FileChooserParams filechooserparams)
        {
            var resBase = base.OnShowFileChooser(webview, filepathcallback, filechooserparams);
            var res = EventsProxy.RiseOnShowFileChooser(webview, filepathcallback, filechooserparams);
            return res ?? resBase;
        }
    }
}