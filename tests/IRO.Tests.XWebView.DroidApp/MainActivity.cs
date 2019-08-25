using System;
using System.IO;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using IRO.Tests.XWebView.Core;
using IRO.Tests.XWebView.DroidApp.JsInterfaces;
using IRO.XWebView.Droid;
using IRO.XWebView.Droid.Containers;
using IRO.XWebView.Droid.Renderer;
using Xamarin.Essentials;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Utils;
using IRO.XWebView.Droid.Utils;
using Newtonsoft.Json;
using Environment = System.Environment;

namespace IRO.Tests.XWebView.DroidApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            var testEnv = new AndroidTestingEnvironment();
            try
            {
                base.OnCreate(savedInstanceState);
                Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.activity_main);
                ThreadSync.Init(new AndroidThreadSyncInvoker());
                var viewRenderer = FindViewById<WebViewRenderer>(Resource.Id.MyWebViewRenderer);

                await viewRenderer.WaitWebViewInflated();
                var webViewContainer = new SelfWebViewContainer();
                webViewContainer.SetWebView(viewRenderer.CurrentWebView);

                var mainXWV = await AndroidXWebView.Create(webViewContainer);
                mainXWV.BindToJs(new AndroidNativeJsInterface(), "AndroidNative");
                var provider = new TestXWebViewProvider();


                var appConfigs = new TestAppSetupConfigs
                {
                    MainXWebView = mainXWV,
                    Provider = provider,
                    TestingEnvironment = testEnv,
                    ContentPath = Application.Context.GetExternalFilesDir("data").CanonicalPath
                };

                var app = new TestApp();
                await app.Setup(appConfigs);

            }
            catch (Exception ex)
            {
                testEnv.Error("Init exception " + ex.ToString());
            }
        }
    }
}