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
using IRO.ImprovedWebView.Core;
using IRO.ImprovedWebView.Droid;
using IRO.ImprovedWebView.Droid.Common;
using IRO.ImprovedWebView.Droid.Renderer;
using AlertDialog = Android.App.AlertDialog;

namespace IRO.Tests.ImprovedWebView.DroidApp.Activities
{
    public abstract class BaseTestActivity : WebViewRendererActivity
    {
        protected void ShowMessage(string str)
        {
            ThreadSync.Invoke(() =>
            {
                Toast.MakeText(Application.Context, str, ToastLength.Long).Show();
            });
        }

        protected void Alert(string str)
        {
            Android.App.Application.SynchronizationContext.Send((obj) =>
            {
                var builder = new AlertDialog.Builder(this);
                builder.SetMessage(str);
                builder.SetPositiveButton("Ok", (s, a) => { });
                var alert = builder.Create();
                alert.Show();
            }, null);
        }

        public override async Task WebViewWrapped(AndroidImprovedWebView improvedWebView)
        {
            await base.WebViewWrapped(improvedWebView);
            try
            {
                await RunTest(improvedWebView);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR \n" + ex.ToString());
                Alert(ex.ToString());
            }

        }

        protected abstract Task RunTest(AndroidImprovedWebView iwv);
    }
}