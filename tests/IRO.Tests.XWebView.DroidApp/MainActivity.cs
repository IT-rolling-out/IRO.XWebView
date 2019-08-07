using System;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using IRO.AndroidActivity;
using IRO.XWebView.Core;
using IRO.XWebView.Droid;
using IRO.XWebView.Droid.Renderer;
using IRO.Tests.XWebView.DroidApp.Activities;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace IRO.Tests.XWebView.DroidApp
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

            var btn = FindViewById<Button>(Resource.Id.TestLoadingButton);
            btn.Click += async delegate { await CreateWebViewRendererActivity<TestLoadingActivity>(); };

            btn = FindViewById<Button>(Resource.Id.TestUploadsDownloadsButton);
            btn.Click += async delegate { await CreateWebViewRendererActivity<TestUploadsDownloadsActivity>(); };

            btn = FindViewById<Button>(Resource.Id.TestJsPromiseDelayButton);
            btn.Click += async delegate { await CreateWebViewRendererActivity<TestJsPromiseDelayActivity>(); };

            btn = FindViewById<Button>(Resource.Id.TestJsAwaitDelayButton);
            btn.Click += async delegate { await CreateWebViewRendererActivity<TestJsAwaitDelayActivity>(); };

            btn = FindViewById<Button>(Resource.Id.TestJsAwaitErrorButton);
            btn.Click += async delegate { await CreateWebViewRendererActivity<TestJsAwaitErrorActivity>(); };

            btn = FindViewById<Button>(Resource.Id.TestJsCallNativeButton);
            btn.Click += async delegate { await CreateWebViewRendererActivity<TestJsCallNativeActivity>(); };

            btn = FindViewById<Button>(Resource.Id.TestBothCallsButton);
            btn.Click += async delegate { await CreateWebViewRendererActivity<TestBothCallsActivity>(); };

            btn = FindViewById<Button>(Resource.Id.TestBothCallsSpeedButton);
            btn.Click += async delegate { await CreateWebViewRendererActivity<TestBothCallsSpeedActivity>(); };
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
            var iwv = await AndroidXWebView.Create(webViewActivity);
        }
    }

}

