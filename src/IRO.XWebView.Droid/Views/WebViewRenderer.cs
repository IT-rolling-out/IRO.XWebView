using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Content.PM;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using IRO.XWebView.Core.Events;
using IRO.XWebView.Droid.Utils;

namespace IRO.XWebView.Droid.Renderer
{
    /// <summary>
    /// Not freezing activity when webview initializing.
    /// </summary>
    public class WebViewRenderer : RelativeLayout
    {
        readonly TaskCompletionSource<object> _finishedWhenWebViewInflated = new TaskCompletionSource<object>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );

        ProgressBar _circularProgressBar;
        ProgressBar _linearProgressBar;
        FrameLayout _fullscreenContainer;
        SwipeRefreshLayout _swipeRefreshLayout;

        public WebViewRenderer(Context context) : base(context)
        {
            Init();
        }

        public WebViewRenderer(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init();
        }

        public WebViewRenderer(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs,
            defStyleAttr)
        {
            Init();
        }

        public WebView CurrentWebView { get; private set; }

        void Init()
        {
            try
            {
                var rootView = Inflate(Context, Resource.Layout.WebViewRenderer, this);
                CurrentWebView = (WebView)rootView.FindViewById(Resource.Id.MyWebView);
                _linearProgressBar = (ProgressBar)rootView.FindViewById(Resource.Id.LinearProgressbar);
                _circularProgressBar = (ProgressBar)rootView.FindViewById(Resource.Id.CircularProgressbar);
                _fullscreenContainer = (FrameLayout)rootView.FindViewById(Resource.Id.FullscreenContainer);
                _swipeRefreshLayout = (SwipeRefreshLayout)rootView.FindViewById(Resource.Id.SwipeRefresh);
                _swipeRefreshLayout.SetColorSchemeColors(Android.Graphics.Color.LightBlue);
              

                //var toolbar = (Android.Support.V7.Widget.Toolbar)rootView.FindViewById(Resource.Id.MyToolbar);
                //toolbar.Menu.Add("aaad");

                //ndroid.support.design.widget.AppBarLayout
                ToggleProgressBar(ProgressBarStyle.None);
                //CurrentWebView.LoadUrl("about:blank");
                _finishedWhenWebViewInflated.SetResult(new object());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"XWebView error: {ex}");
                _finishedWhenWebViewInflated.SetException(ex);
            }
        }

        public void ToggleProgressBar(ProgressBarStyle progressBarStyle)
        {
            switch (progressBarStyle)
            {
                case ProgressBarStyle.Circular:
                    _linearProgressBar.Visibility = ViewStates.Gone;
                    _circularProgressBar.Visibility = ViewStates.Visible;
                    break;
                case ProgressBarStyle.None:
                    _linearProgressBar.Visibility = ViewStates.Gone;
                    _circularProgressBar.Visibility = ViewStates.Gone;
                    break;
                case ProgressBarStyle.Linear:
                    _linearProgressBar.Visibility = ViewStates.Visible;
                    _circularProgressBar.Visibility = ViewStates.Gone;
                    break;
            }
        }

        public async Task WaitWebViewInflated()
        {
            await _finishedWhenWebViewInflated.Task;
            WebViewExtensions.ApplyDefaultSettings(CurrentWebView);
            var ep = CurrentWebView.ProxyWebChromeClient().EventsProxy;
            ep.OnShowCustomView += OnShowCustomView;
            ep.OnShowCustomView2 += OnShowCustomView2;
            ep.OnHideCustomView += OnHideCustomView;
            _swipeRefreshLayout.Refresh += RefreshRequested;
        }

        void RefreshRequested(object sender, EventArgs eventArgs)
        {
            try
            {
                var client = CurrentWebView.ProxyWebViewClient();
                LoadFinishedDelegate handler = null;
                handler = new LoadFinishedDelegate((s, a) =>
                {
                    ThreadSync.TryInvoke(() =>
                    {
                        client.LoadFinished -= handler;
                        _swipeRefreshLayout.Refreshing = false;
                    });
                });
                client.LoadFinished += handler;
                CurrentWebView.Reload();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in WebViewRenderer.RefreshRequested {ex}.");
            }
        }

        #region Fullscreen video support.
        Android.Webkit.WebChromeClient.ICustomViewCallback _customViewCallback;

        View _customView;

        bool _newCustomViewWorks;

        void OnShowCustomView(View view, Android.Webkit.WebChromeClient.ICustomViewCallback callback)
        {
            try
            {
                _newCustomViewWorks = true;
                CurrentWebView.Visibility = ViewStates.Invisible;
                _fullscreenContainer.Visibility = ViewStates.Visible;
                _fullscreenContainer.AddView(view);
                _customViewCallback = callback;
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
                CurrentWebView.Visibility = ViewStates.Visible;
                _fullscreenContainer.Visibility = ViewStates.Gone;
                _fullscreenContainer.RemoveView(_customView);
                _customViewCallback?.OnCustomViewHidden();
                _customView = null;
                _customViewCallback = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in WebViewRenderer.OnHideCustomView {ex}.");
            }
        }
        #endregion
    }
}