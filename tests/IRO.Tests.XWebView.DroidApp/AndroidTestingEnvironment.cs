using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using IRO.XWebView.Droid.Common;
using IRO.Tests.XWebView.CommonTests;

namespace IRO.Tests.XWebView.DroidApp
{
    public class AndroidTestingEnvironment:ITestingEnvironment
    {
        readonly Context _context;

        public AndroidTestingEnvironment(Context context)
        {
            _context = context;
        }

        public void Message(string str)
        {
            ThreadSync.Invoke(() =>
            {
                Toast.MakeText(Application.Context, str, ToastLength.Long).Show();
            });
        }

        public void Error(string str)
        {
            Android.App.Application.SynchronizationContext.Send((obj) =>
            {
                var builder = new AlertDialog.Builder(_context);
                builder.SetMessage(str);
                builder.SetPositiveButton("Ok", (s, a) => { });
                var alert = builder.Create();
                alert.Show();
            }, null);
        }
    }
}