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
using IRO.XWebView.Droid.Consts;

namespace IRO.XWebView.Droid.Renderer
{
    [Activity(Label = "TransparentXWebViewActivity",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        Theme = "@style/XWebViewTheme.Transparent"
    )]
    public class XWebViewTransparentActivity : Activity, IWebViewContainer
    {
        public WebView CurrentWebView { get; private set; }

        public virtual bool CanSetVisibility { get; } = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.XWebViewTransparentActivity);
            CurrentWebView = FindViewById<WebView>(Resource.Id.MyWebView);
        }

        public virtual void ToggleVisibilityState(XWebViewVisibility visibility)
        {
        }

        public virtual async Task WebViewWrapped(AndroidXWebView XWebView)
        {
            WebViewExtensions.ApplyDefaultSettings(CurrentWebView);
        }

        public async Task WaitWebViewInitialized()
        {
            if (CurrentWebView == null)
                throw new Exception("You can start resolving webview only after OnCreate invoked.");
        }

        #region Disposing.
        public bool IsDisposed { get; private set; }

        public event Action<object, EventArgs> Disposing;

        public new virtual void Dispose()
        {
            if (IsDisposed)
                return;
            IsDisposed = true;
            Finish();
            Disposing?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}