using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Webkit;
using IRO.XWebView.Core.Consts;

namespace IRO.XWebView.Droid.Activities
{
    [Activity(Label = "TransparentXWebViewActivity",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        Theme = "@style/XWebViewTheme.Transparent"
    )]
    public class XWebViewTransparentActivity : Activity, IWebViewContainer
    {
        public AndroidXWebView XWebView;

        public WebView CurrentWebView { get; private set; }

        public virtual bool CanSetVisibility { get; } = false;

        public virtual void ToggleVisibilityState(XWebViewVisibility visibility)
        {
        }

        public virtual async Task WebViewWrapped(AndroidXWebView xwv)
        {
            XWebView = xwv;
        }

        public async Task WaitWebViewInitialized()
        {
            if (CurrentWebView == null)
                throw new Exception("You can start resolving webview only after OnCreate invoked.");
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.XWebViewTransparentActivity);
            CurrentWebView = FindViewById<WebView>(Resource.Id.MyWebView);
            WebViewExtensions.ApplyDefaultSettings(CurrentWebView);
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
            XWebView = null;
            Disposing?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}