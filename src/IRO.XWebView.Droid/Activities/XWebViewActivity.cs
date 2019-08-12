using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Webkit;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Droid.BrowserClients;
using IRO.XWebView.Droid.Renderer;

namespace IRO.XWebView.Droid.Activities
{
    [Activity(Label = "XWebViewActivity", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class XWebViewActivity : Activity, IWebViewContainer
    {
        public AndroidXWebView XWebView{ get; private set; }

        public WebViewRenderer ViewRenderer{ get; private set; }

        public WebView CurrentWebView => ViewRenderer.CurrentWebView;

        public virtual bool CanSetVisibility { get; } = false;

        public virtual async Task WebViewWrapped(AndroidXWebView xwv)
        {
            XWebView = xwv;
            AndroidXWebViewExtensions.UseBackButtonCrunch(XWebView, CurrentWebView, Finish);
        }

        public void ToggleVisibilityState(XWebViewVisibility visibility)
        {
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
        #region Disposing.

        public bool IsDisposed { get; private set; }

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
            CurrentWebView.Destroy();
        }
        #endregion
    }
}