using System;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace IRO.XWebView.Droid
{
    public class FullscreenViewSupport : IDisposable
    {
        WebView _webView;

        ViewGroup _fullscreenContainer;

        Android.Webkit.WebChromeClient.ICustomViewCallback _customViewCallback;

        View _customView;

        bool _newCustomViewWorks;

        public FullscreenViewSupport(WebView webView, ViewGroup fullscreenContainer)
        {
            _webView = webView ?? throw new ArgumentNullException(nameof(webView));
            _fullscreenContainer = fullscreenContainer ?? throw new ArgumentNullException(nameof(fullscreenContainer));
            var ep = _webView.ProxyWebChromeClient().EventsProxy;
            ep.OnShowCustomView += OnShowCustomView;
            ep.OnShowCustomView2 += OnShowCustomView2;
            ep.OnHideCustomView += OnHideCustomView;
        }

        /// <summary>
        /// Unsubscripe OnShowCustomView events and dispose current object.
        /// </summary>
        public void Dispose()
        {
            var ep = _webView?.ProxyWebChromeClient()?.EventsProxy;
            if (ep != null)
            {
                ep.OnShowCustomView -= OnShowCustomView;
                ep.OnShowCustomView2 -= OnShowCustomView2;
                ep.OnHideCustomView -= OnHideCustomView;
            }
            _webView = null;
            _fullscreenContainer = null;
        }

        void OnShowCustomView(View view, Android.Webkit.WebChromeClient.ICustomViewCallback callback)
        {
            try
            {
                _newCustomViewWorks = true;
                var ownerFrameLayout = new FrameLayout(_webView.Context);
                ownerFrameLayout.AddView(view);
                ownerFrameLayout.Background = new ColorDrawable(Color.Black);
                ownerFrameLayout.LayoutParameters = new ViewGroup.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.MatchParent
                    );
                _fullscreenContainer.AddView(ownerFrameLayout);
                _customViewCallback = callback;
                _customView = ownerFrameLayout;
                _webView.Visibility = ViewStates.Invisible;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in WebViewRenderer.OnShowCustomView {ex}.");
            }
        }

        [Obsolete("deprecated")]
        void OnShowCustomView2(View view, ScreenOrientation requestedorientation, Android.Webkit.WebChromeClient.ICustomViewCallback callback)
        {
            if (_newCustomViewWorks)
                return;
            OnShowCustomView(view, callback);
        }

        void OnHideCustomView()
        {
            try
            {
                _fullscreenContainer.RemoveView(_customView);
                _customViewCallback?.OnCustomViewHidden();
                _customView = null;
                _customViewCallback = null;
                _webView.Visibility = ViewStates.Visible;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in WebViewRenderer.OnHideCustomView {ex}.");
            }
        }
    }
}