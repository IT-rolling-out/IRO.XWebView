using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using IRO.Tests.XWebView.CommonTests;
using IRO.Tests.XWebView.DroidApp.Tests;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Droid.OnFinestWebView;
using IRO.XWebView.Droid.Utils;
using TheFinestArtist.FinestWebViewLib;
using Xamarin.Essentials;
using Debug = System.Diagnostics.Debug;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace IRO.Tests.XWebView.DroidApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            var switchCtrl = FindViewById<Switch>(Resource.Id.UseFinestWebViewSwitch);
            switchCtrl.CheckedChange += (s, ev) =>
             {
                 TestXWebViewProvider.UseFinestWebView = ev.IsChecked;
             };

            //Used activities with overrided RunTest method to execute test on it's activity.
            var btn = FindViewById<Button>(Resource.Id.TestLoadingButton);
            btn.Click += async delegate { await RunXWebViewTest<TestLoading>(); };

            btn = FindViewById<Button>(Resource.Id.TestUploadsDownloadsButton);
            btn.Click += async delegate { await RunXWebViewTest<TestUploadsDownloads>(); };

            btn = FindViewById<Button>(Resource.Id.TestJsPromiseDelayButton);
            btn.Click += async delegate { await RunXWebViewTest<TestJsPromiseDelay>(); };

            btn = FindViewById<Button>(Resource.Id.TestJsAwaitDelayButton);
            btn.Click += async delegate { await RunXWebViewTest<TestJsAwaitDelay>(); };

            btn = FindViewById<Button>(Resource.Id.TestJsAwaitErrorButton);
            btn.Click += async delegate { await RunXWebViewTest<TestJsAwaitError>(); };

            btn = FindViewById<Button>(Resource.Id.TestJsCallNativeButton);
            btn.Click += async delegate { await RunXWebViewTest<TestJsCallNative>(); };

            btn = FindViewById<Button>(Resource.Id.TestBothCallsButton);
            btn.Click += async delegate { await RunXWebViewTest<TestBothCalls>(); };

            btn = FindViewById<Button>(Resource.Id.TestBothCallsSpeedButton);
            btn.Click += async delegate { await RunXWebViewTest<TestBothCallsSpeed>(); };

            btn = FindViewById<Button>(Resource.Id.TestTransparentActivityButton);
            btn.Click += async delegate { await RunXWebViewTest<TestTransparentView>(); };

            btn = FindViewById<Button>(Resource.Id.TestFullscreenViewsButton);
            btn.Click += async delegate { await RunXWebViewTest<TestFullscreenViews>(); };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        async Task RunXWebViewTest<TWebViewTest>()
            where TWebViewTest : IXWebViewTest
        {
            //In test we use overrided NewActivityXWebViewProvider to get link to last XWebView.
            //In your projects you can use NewActivityXWebViewProvider.
            //Another use case is to use XWebViewProvider by object, when you created XWebView on current activity.

            var provider = new TestXWebViewProvider();
            var test = Activator.CreateInstance<TWebViewTest>();
            var testEnv = new AndroidTestingEnvironment();
            try
            {
                await test.RunTest(provider, testEnv);
                if (provider.LastVisibility == XWebViewVisibility.Hidden)
                {
                    //Crunch to dispose transparent XWebView.
                    //You can do it saving link from IXWebViewProvider.
                    testEnv.Message("Hidden XWebView disposed.");
                    provider.LastResolved.Dispose();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR \n" + ex.ToString());
                testEnv.Error(ex.ToString());
            }
        }
    }
}