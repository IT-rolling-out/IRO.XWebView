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
using IRO.ImprovedWebView.Droid.Renderer;
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
            testLoadingButton.Click += WrapTryCatch(OnTestLoadingButtonClick);

            var testUploadsDownloadsButton = FindViewById<Button>(Resource.Id.TestUploadsDownloadsButton);
            testUploadsDownloadsButton.Click +=  WrapTryCatch(OnTestUploadsDownloadsButtonClick);

            var testJsPromiseDelayButton = FindViewById<Button>(Resource.Id.TestJsPromiseDelayButton);
            testJsPromiseDelayButton.Click +=  WrapTryCatch(OnTestJsPromiseDelayButtonClick);
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

        async Task OnTestUploadsDownloadsButtonClick()
        {
            var webViewActivity = await ActivityExtensions.StartNewActivity<WebViewRendererActivity>();
            var iwv = await AndroidImprovedWebView.Create(webViewActivity);
            await iwv.WaitWhileBusy();
            var loadRes = await iwv.LoadUrl("https://gofile.io/?t=uploadFiles");
        }

        async Task OnTestLoadingButtonClick()
        {
            var webViewActivity = await ActivityExtensions.StartNewActivity<WebViewRendererActivity>();
            var iwv = await AndroidImprovedWebView.Create(webViewActivity);
            //Choose websites that can load long time.

            //This three must be aborted in test.
            iwv.TryLoadUrl("https://stackoverflow.com");
            webViewActivity.CurrentWebView.LoadUrl("https://twitter.com");
            iwv.TryLoadUrl("https://visualstudio.microsoft.com/ru/");

            var loadRes = await iwv.LoadUrl("https://www.youtube.com/");
            ShowMessage($"Loaded {loadRes.Url}");
            loadRes = await iwv.LoadUrl("https://www.google.com/");
            ShowMessage($"Loaded {loadRes.Url}");
        }

        async Task OnTestJsPromiseDelayButtonClick()
        {
            var delayScript = @"
window['delayPromise'] = function(delayMS) {
  return new Promise(function(resolve, reject){
    setTimeout(function(){
      resolve({});
    }, delayMS)
  });
}
";
            var webViewActivity = await ActivityExtensions.StartNewActivity<WebViewRendererActivity>();
            var iwv = await AndroidImprovedWebView.Create(webViewActivity);
            await iwv.ExJs<string>(delayScript);
            var str=await iwv.ExJs<string>("await delayPromise(5000); return 'Awaited message from js';", true);
            ShowMessage($"JsResult: '{str}'");
        }


        void ShowMessage(string str)
        {
            Application.SynchronizationContext.Post((obj) =>
            {
                Toast.MakeText(Application.Context, str, ToastLength.Long).Show();
            }, null);
        }

        void Alert(string str)
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetMessage(str);
            builder.SetPositiveButton("Ok",(s,a)=>{});
            var alert=builder.Create();
            alert.Show();
        }

        EventHandler WrapTryCatch(Func<Task> func)
        {
            EventHandler res = async (s, a) =>
             {
                 try
                 {
                     await func.Invoke();
                 }
                 catch (Exception ex)
                 {
                     System.Diagnostics.Debug.WriteLine("ERROR \n"+ex.ToString());
                     Alert(ex.ToString());
                 }
             };
            return res;
        }
    }

}

