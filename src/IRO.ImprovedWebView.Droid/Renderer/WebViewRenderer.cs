using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace IRO.ImprovedWebView.Droid.Renderer
{
    /// <summary>
    /// Not freezing activity when webview initializing.
    /// </summary>
    public class WebViewRenderer : RelativeLayout
    {
        ProgressBar _linearProgressBar;

        ProgressBar _circularProgressBar;

        TaskCompletionSource<object> _finishedWhenWebViewInflated = new TaskCompletionSource<object>(
            TaskCreationOptions.RunContinuationsAsynchronously
            );

        public WebView CurrentWebView { get; private set; }

        public WebViewRenderer(Context context) : base(context)
        {
            Init();
        }

        public WebViewRenderer(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init();
        }

        public WebViewRenderer(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init();
        }

        void Init()
        {
            try
            {
                var rootView = Inflate(Context, Resource.Layout.WebViewRenderer, this);
                CurrentWebView = (WebView)rootView.FindViewById(Resource.Id.just_web_view);
                _linearProgressBar = (ProgressBar)rootView.FindViewById(Resource.Id.linear_progressbar);
                _circularProgressBar = (ProgressBar)rootView.FindViewById(Resource.Id.circular_progressbar);

                ToggleProgressBar(ProgressBarStyle.None);
                //CurrentWebView.LoadUrl("about:blank");
                _finishedWhenWebViewInflated.SetResult(new object());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ImprovedWebView error: {ex}");
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
        }
    }
}
