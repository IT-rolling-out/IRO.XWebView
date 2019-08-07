using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Webkit;
using IRO.ImprovedWebView.Core;
using IRO.ImprovedWebView.Droid.Activities;
using IRO.ImprovedWebView.Droid.Consts;

namespace IRO.ImprovedWebView.Droid.Renderer
{
    [Activity(Label = "WebViewRendererActivity", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class WebViewRendererActivity : Activity, IWebViewActivity
    {
        protected WebViewRenderer ViewRenderer;

        protected IWebViewEventsProxy EventsProxy;

        public WebView CurrentWebView => ViewRenderer.CurrentWebView;

        /// <summary>
        /// Progress bar style used when it must be visible.
        /// </summary>
        public ProgressBarStyle VisibleProgressBarStyle { get; set; } = ProgressBarStyle.Linear;

        public event Action<IWebViewActivity> Finishing;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.WebViewRendererActivity);
            ViewRenderer = FindViewById<WebViewRenderer>(Resource.Id.MyWebViewRenderer);
            WebViewExtensions.ApplyDefaultSettings(CurrentWebView);
            WebViewExtensions.SetPermissionsMode(CurrentWebView, PermissionsMode.AllowedAll);
            WebViewExtensions.AddDownloadsSupport(CurrentWebView);
            WebViewExtensions.AddUploadsSupport(CurrentWebView);
            var dataDirectory = Android.App.Application.Context.GetExternalFilesDir("data").CanonicalPath;
            var cachePath = System.IO.Path.Combine(dataDirectory, "webview_cache");
            WebViewExtensions.InitWebViewCaching(CurrentWebView, cachePath);

        }

        public virtual void ToggleVisibilityState(ImprovedWebViewVisibility visibility)
        {
        }

        public virtual async Task WebViewWrapped(AndroidImprovedWebView improvedWebView)
        {
            RegisterEvents(improvedWebView.EventsProxy);
            AndroidImprovedWebViewExtensions.UseBackButtonCrunch(improvedWebView, CurrentWebView, Finish);
        }

        public override void Finish()
        {
            Finishing?.Invoke(this);
            if (EventsProxy != null)
            {
                EventsProxy.PageStartedEvent -= OnPageStarted;
                EventsProxy.PageFinishedEvent -= OnPageFinished;
            }

            base.Finish();
        }

        public async Task WaitWebViewInitialized()
        {
            if (ViewRenderer == null)
                throw new Exception("You can start resolving webview only after OnCreate invoked.");
            await ViewRenderer.WaitWebViewInflated();
        }

        protected virtual void OnPageFinished(WebView view, string url)
        {
            ViewRenderer.ToggleProgressBar(ProgressBarStyle.None);
            CurrentWebView.Visibility = Android.Views.ViewStates.Visible;
        }

        protected virtual void OnPageStarted(WebView view, string url, Bitmap favicon)
        {
            //Hide webview if use linear progressbar.
            if (VisibleProgressBarStyle == ProgressBarStyle.Circular)
                CurrentWebView.Visibility = Android.Views.ViewStates.Invisible;
            ViewRenderer.ToggleProgressBar(VisibleProgressBarStyle);
        }

        void RegisterEvents(IWebViewEventsProxy eventsProxy)
        {
            EventsProxy = eventsProxy;
            EventsProxy.PageStartedEvent += OnPageStarted;
            EventsProxy.PageFinishedEvent += OnPageFinished;
        }
    }


}