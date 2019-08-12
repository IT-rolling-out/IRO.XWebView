﻿using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Webkit;
using IRO.XWebView.Droid.BrowserClients;
using TheFinestArtist.FinestWebViewLib;

namespace IRO.XWebView.Droid.OnFinestWebView
{
    [Activity(
        Label = "OverriddenFinestWebViewActivity", 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        Name ="IRO.XWebView.Droid.OverriddenFinestWebViewActivity"
        )]
    public class OverriddenFinestWebViewActivity : FinestWebViewActivity
    {
        public WebView PublicWebView => this.WebView;

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            WebView.EnableFullscreenViewSupport(CoordinatorLayout);
        }

        public virtual void OnClientsEventsProxySetup()
        {
            WebView.EnableFullscreenViewSupport(CoordinatorLayout);
        }
    }
}