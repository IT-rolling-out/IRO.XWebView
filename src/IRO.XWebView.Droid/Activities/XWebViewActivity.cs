using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Webkit;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Droid.Activities;

namespace IRO.XWebView.Droid.Renderer
{
    [Activity(Label = "XWebViewActivity", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class XWebViewActivity : Activity, IWebViewContainer
    {
        protected IWebViewEventsProxy EventsProxy;
        protected WebViewRenderer ViewRenderer;

        /// <summary>
        /// Progress bar style used when it must be visible.
        /// </summary>
        public ProgressBarStyle VisibleProgressBarStyle { get; set; } = ProgressBarStyle.Linear;

        public WebView CurrentWebView => ViewRenderer.CurrentWebView;

        public virtual bool CanSetVisibility { get; } = false;

        public virtual async Task WebViewWrapped(AndroidXWebView XWebView)
        {
            WebViewExtensions.ApplyDefaultSettings(CurrentWebView);

            EventsProxy = XWebView.EventsProxy;
            EventsProxy.PageStartedEvent += OnPageStarted;
            EventsProxy.PageFinishedEvent += OnPageFinished;
            AndroidXWebViewExtensions.UseBackButtonCrunch(XWebView, CurrentWebView, Finish);
        }

        public void ToggleVisibilityState(XWebViewVisibility visibility)
        {
            throw new NotImplementedException();
        }

        public async Task WaitWebViewInitialized()
        {
            if (ViewRenderer == null)
                throw new Exception("You can start resolving webview only after OnCreate invoked.");
            await ViewRenderer.WaitWebViewInflated();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.XWebViewActivity);
            ViewRenderer = FindViewById<WebViewRenderer>(Resource.Id.MyWebViewRenderer);
        }

        protected virtual void OnPageFinished(WebView view, string url)
        {
            ViewRenderer.ToggleProgressBar(ProgressBarStyle.None);
            CurrentWebView.Visibility = ViewStates.Visible;
        }

        protected virtual void OnPageStarted(WebView view, string url, Bitmap favicon)
        {
            //Hide webview if use linear progressbar.
            if (VisibleProgressBarStyle == ProgressBarStyle.Circular)
                CurrentWebView.Visibility = ViewStates.Invisible;
            ViewRenderer.ToggleProgressBar(VisibleProgressBarStyle);
        }

        #region Disposing.

        public bool IsDisposed { get; private set; }

        public event Action<object, EventArgs> Disposing;

        public override void Finish()
        {
            if (IsFinishing)
                return;
            base.Finish();
            Dispose();
        }

        /// <summary>
        /// If we use activity as <see cref="IWebViewContainer"/>
        /// Dispose must do same work to Finish.
        /// </summary>
        public new virtual void Dispose()
        {
            if (IsDisposed)
                return;
            IsDisposed = true;
            Finish();
            if (EventsProxy != null)
            {
                EventsProxy.PageStartedEvent -= OnPageStarted;
                EventsProxy.PageFinishedEvent -= OnPageFinished;
            }
            Disposing?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}