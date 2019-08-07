using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using IRO.AndroidActivity;
using IRO.XWebView.Core;
using IRO.XWebView.Droid;
using IRO.XWebView.Droid.Renderer;
using AlertDialog = Android.App.AlertDialog;

namespace IRO.Tests.XWebView.DroidApp.Activities
{
    public abstract class BaseTestActivity : WebViewRendererActivity
    {
        protected readonly AndroidTestingEnvironment TestingEnvironment;

        protected BaseTestActivity()
        {
            TestingEnvironment = new AndroidTestingEnvironment(this);
        }

        public override async Task WebViewWrapped(AndroidXWebView XWebView)
        {
            await base.WebViewWrapped(XWebView);
            try
            {
                await RunTest(XWebView);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR \n" + ex.ToString());
                TestingEnvironment.Error(ex.ToString());
            }


        }

        protected abstract Task RunTest(AndroidXWebView iwv);
    }
}