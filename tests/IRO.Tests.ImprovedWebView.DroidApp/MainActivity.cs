using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using IRO.AndroidActivity;
using IRO.ImprovedWebView.Core;
using IRO.ImprovedWebView.Droid;
using IRO.ImprovedWebView.Droid.Renderer;

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
            testLoadingButton.Click += async delegate
            {
                var webViewActivity = await ActivityExtensions.StartNewActivity<WebViewRendererActivity>();
                var iwv = await AndroidImprovedWebView.Create(webViewActivity);
                //Choose website that can load long time.
                await iwv.WaitWhileBusy();
                var loadRes = await iwv.LoadUrl("https://www.youtube.com/");
                ShowMessage($"Loaded {loadRes.Url}");
                loadRes = await iwv.LoadUrl("https://www.google.com/");
                ShowMessage($"Loaded {loadRes.Url}");
            };


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

        void ShowMessage(string str)
        {
            Application.SynchronizationContext.Post((obj) =>
            {
                Toast.MakeText(Application.Context, str, ToastLength.Long).Show();
            }, null);
        }
    }
}

