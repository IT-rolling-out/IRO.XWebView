using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using IRO.ImprovedWebView.Droid;

namespace IRO.ImprovedWebView.Droid
{
    /// <summary>
    /// Not freezing activity when webview initializing.
    /// </summary>
    public class WebViewRenderer : RelativeLayout
    {
        readonly ProgressBar _linearProgressBar;

        readonly ProgressBar _circularProgressBar;

        readonly TaskCompletionSource<object> _finishedWhenWebViewInflated = new TaskCompletionSource<object>();

        public WebView CurrentWebView { get; }

        public WebViewRenderer(Context context) : this(context, null)
        {
        }

        public WebViewRenderer(Context context, IAttributeSet attrs) : this(context, attrs, default(int))
        {
        }

        public WebViewRenderer(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs,
            defStyleAttr)
        {
            try
            {
                var rootView = Inflate(Context, Resource.Layout.WebViewRenderer, this);
                CurrentWebView = (WebView) rootView.FindViewById(Resource.Id.just_web_view);
                _linearProgressBar = (ProgressBar) rootView.FindViewById(Resource.Id.linear_progressbar);
                _circularProgressBar = (ProgressBar) rootView.FindViewById(Resource.Id.circular_progressbar);

                ToogleProgressBar(ProgressBarStyle.None);

                WebViewExtensions.ApplyDefaultSettings(CurrentWebView);
                CurrentWebView.LoadUrl("about:blank");
                _finishedWhenWebViewInflated.SetResult(new object());
            }
            catch(Exception ex)
            {
                _finishedWhenWebViewInflated.SetException(ex);
            }
        }

        public void ToogleProgressBar(ProgressBarStyle progressBarStyle)
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
