using System;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using IRO.AndroidActivity;
using IRO.ImprovedWebView.Core;
using IRO.ImprovedWebView.Droid;
using IRO.ImprovedWebView.Droid.Common;
using IRO.ImprovedWebView.Droid.Renderer;
using IRO.Tests.ImprovedWebView.DroidApp.Activities;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace IRO.Tests.ImprovedWebView.DroidApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            var testLoadingButton = FindViewById<Button>(Resource.Id.TestLoadingButton);
            testLoadingButton.Click += async delegate { await CreateWebViewRendererActivity<TestLoadingActivity>(); };

            var testUploadsDownloadsButton = FindViewById<Button>(Resource.Id.TestUploadsDownloadsButton);
            testUploadsDownloadsButton.Click += async delegate { await CreateWebViewRendererActivity<TestUploadsDownloadsActivity>(); };

            var testJsPromiseDelayButton = FindViewById<Button>(Resource.Id.TestJsPromiseDelayButton);
            testJsPromiseDelayButton.Click += async delegate { await CreateWebViewRendererActivity<TestJsPromiseDelayActivity>(); };

            var testJsBridgeButton = FindViewById<Button>(Resource.Id.TestJsBridgeButton);
            testJsBridgeButton.Click += async delegate { await CreateWebViewRendererActivity<TestJsBridgeActivity>(); };
        }
        
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        async Task CreateWebViewRendererActivity<TWebViewRendererActivity>()
            where TWebViewRendererActivity:WebViewRendererActivity
        {
            var webViewActivity = await ActivityExtensions.StartNewActivity<TWebViewRendererActivity>();
            var iwv = await AndroidImprovedWebView.Create(webViewActivity);
        }
    }

}

