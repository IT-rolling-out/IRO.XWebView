using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace IRO.ImprovedWebView.Droid
{
    [Activity(Label = "WebViewRendererActivity", Icon = "@mipmap/icon", Theme = "@style/MainTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class WebViewRendererActivity : Activity
    {
        WebViewRenderer _webViewRenderer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.WebViewRendererActivity);
            _webViewRenderer = FindViewById<WebViewRenderer>(Resource.Layout.WebViewRenderer);
        }

        public async Task<WebView> ResolveInflatedWebView()
        {
            if (_webViewRenderer == null)
                throw new Exception("You can start resolving webview only after OnCreate invoked.");
            await _webViewRenderer.WaitWebViewInflated();
            return _webViewRenderer.CurrentWebView;
        }
    }

    
}