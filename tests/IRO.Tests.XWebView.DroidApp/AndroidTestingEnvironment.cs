using Android.App;
using Android.Widget;
using IRO.Tests.XWebView.Core;
using IRO.XWebView.Droid.Utils;
using Plugin.CurrentActivity;

namespace IRO.Tests.XWebView.DroidApp
{
    public class AndroidTestingEnvironment : ITestingEnvironment
    {
        public void Message(string str)
        {
            ThreadSync.Invoke(() => { Toast.MakeText(Application.Context, str, ToastLength.Long).Show(); });
        }

        public void Error(string str)
        {
            ThreadSync.Invoke(() =>
            {
                var builder = new AlertDialog.Builder(CrossCurrentActivity.Current.Activity);
                builder.SetMessage(str);
                builder.SetPositiveButton("Ok", (s, a) => { });
                var alert = builder.Create();
                alert.Show();
            });
        }
    }
}