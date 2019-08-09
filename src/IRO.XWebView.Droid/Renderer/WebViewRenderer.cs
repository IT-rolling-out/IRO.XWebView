using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace IRO.XWebView.Droid.Renderer
{
    /// <summary>
    /// Not freezing activity when webview initializing.
    /// </summary>
    public class WebViewRenderer : RelativeLayout
    {
        ProgressBar _circularProgressBar;

        readonly TaskCompletionSource<object> _finishedWhenWebViewInflated = new TaskCompletionSource<object>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );

        ProgressBar _linearProgressBar;

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
                CurrentWebView = (WebView) rootView.FindViewById(Resource.Id.MyWebView);
                _linearProgressBar = (ProgressBar) rootView.FindViewById(Resource.Id.LinearProgressbar);
                _circularProgressBar = (ProgressBar) rootView.FindViewById(Resource.Id.CircularProgressbar);

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
        }
    }
}