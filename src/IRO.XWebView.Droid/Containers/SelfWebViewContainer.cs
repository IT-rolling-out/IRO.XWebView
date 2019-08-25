using System;
using System.Threading.Tasks;
using Android.App;
using Android.Views;
using Android.Webkit;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Droid.Utils;

namespace IRO.XWebView.Droid.Containers
{
    /// <summary>
    /// Use WebView as it's container.
    /// Handle visibility and lifecycle of webview.
    /// <para></para>
    /// Another practice is to implement <see cref="IWebViewContainer"/> by activity.
    /// </summary>
    public class SelfWebViewContainer : IWebViewContainer
    {
        public TaskCompletionSource<object> _waitTaskCompletionSource = new TaskCompletionSource<object>(
            TaskCreationOptions.RunContinuationsAsynchronously
            );

        public AndroidXWebView XWebView { get; private set; }

        public Activity CurrentActivity { get; private set; }

        public WebView CurrentWebView { get; private set; }

        public bool CanSetVisibility { get; set; } = true;

        /// <summary>
        /// Invoked when WebView initialized.
        /// </summary>
        public virtual void SetWebView(WebView currentWebView, Activity activity = null)
        {
            if (_waitTaskCompletionSource.Task.IsCompleted)
                throw new Exception($"You can init {nameof(SelfWebViewContainer)} one time.");
            CurrentWebView = currentWebView ?? throw new ArgumentNullException(nameof(currentWebView));
            CurrentWebView.ViewDetachedFromWindow += delegate
            {
                Dispose();
            };
            CurrentActivity = activity;
            _waitTaskCompletionSource.SetResult(null);
        }

        public virtual void SetVisibilityState(XWebViewVisibility visibility)
        {
            AndroidThreadSync.Inst.Invoke(() =>
            {
                if (visibility == XWebViewVisibility.Visible)
                {
                    CurrentWebView.Visibility = ViewStates.Visible;
                }
                else
                {
                    CurrentWebView.Visibility = ViewStates.Invisible;
                }
            });
        }

        public virtual XWebViewVisibility GetVisibilityState()
        {
            return AndroidThreadSync.Inst.Invoke(() =>
            {
                if (CurrentWebView.Visibility == ViewStates.Visible)
                {
                    return XWebViewVisibility.Visible;
                }

                return XWebViewVisibility.Hidden;
            });
        }

        public virtual async Task WaitWebViewInitialized()
        {
            await _waitTaskCompletionSource.Task;
        }

        public virtual async Task WebViewWrapped(AndroidXWebView xwv)
        {
            XWebView = xwv;
        }

        #region Disposing.
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// If we use activity as <see cref="IWebViewContainer"/>
        /// Dispose must do same work to Finish.
        /// </summary>
        public virtual void Dispose()
        {
            if (IsDisposed)
                return;
            IsDisposed = true;
            XWebView = null;
            CurrentWebView.Destroy();
            if (CurrentActivity != null && !CurrentActivity.IsFinishing)
                CurrentActivity.Finish();
            //XWebView handle all WebView clearing work.
        }
        #endregion
    }
}