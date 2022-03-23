using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using IRO.XWebView.Core.Events;
using IRO.XWebView.Core.Utils;
using IRO.XWebView.Droid.Utils;

namespace IRO.XWebView.Droid.Renderer
{
    /// <summary>
    /// Not freezing activity when webview initializing.
    /// </summary>
    public class WebViewRenderer : RelativeLayout
    {
        /// <summary>
        /// Progress bar style used when it must be visible.
        /// </summary>
        public ProgressBarStyle VisibleProgressBarStyle { get; set; } = ProgressBarStyle.Linear;


        readonly TaskCompletionSource<object> _finishedWhenWebViewInflated = new TaskCompletionSource<object>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );

        ProgressBar _circularProgressBar;
        ProgressBar _linearProgressBar;
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
                _linearProgressBar.ScaleY = 1.0f;
                _circularProgressBar = (ProgressBar)rootView.FindViewById(Resource.Id.CircularProgressbar);
                _swipeRefreshLayout = (SwipeRefreshLayout)rootView.FindViewById(Resource.Id.SwipeRefresh);
                _swipeRefreshLayout.SetColorSchemeColors(Android.Graphics.Color.LightBlue);

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

        void ToggleProgressBar(ProgressBarStyle progressBarStyle)
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
            CurrentWebView.EnableFullscreenViewSupport(this);
            _swipeRefreshLayout.Refresh += RefreshRequested;
            var ep=CurrentWebView.ProxyWebViewClient().EventsProxy;
            ep.OnPageStarted += OnPageStarted;
            ep.OnPageFinished += OnPageFinished;
            CurrentWebView.ViewDetachedFromWindow += delegate { OnViewDetachedFromWindow(); };
        }

        void OnViewDetachedFromWindow()
        {
            try
            {
                var ep = CurrentWebView.ProxyWebViewClient().EventsProxy;
                ep.OnPageStarted -= OnPageStarted;
                ep.OnPageFinished -= OnPageFinished;
            }
            catch { }
        }

        void OnPageFinished(WebView view, string url)
        {
            ToggleProgressBar(ProgressBarStyle.None);
            CurrentWebView.Visibility = ViewStates.Visible;
        }

        void OnPageStarted(WebView view, string url, Bitmap favicon)
        {
            //Hide webview if use linear progressbar.
            if (VisibleProgressBarStyle == ProgressBarStyle.Circular)
                CurrentWebView.Visibility = ViewStates.Invisible;
            ToggleProgressBar(VisibleProgressBarStyle);
        }

        void RefreshRequested(object sender, EventArgs eventArgs)
        {
            try
            {
                var client = CurrentWebView.ProxyWebViewClient();
                LoadFinishedDelegate_Sync handler = null;
                handler = new LoadFinishedDelegate_Sync((s, a) =>
                {
                    XWebViewThreadSync.Inst.TryInvoke(() =>
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
    }
}