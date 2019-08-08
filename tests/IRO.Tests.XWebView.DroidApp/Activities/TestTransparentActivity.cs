using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using IRO.XWebView.Droid;

namespace IRO.Tests.XWebView.DroidApp.Activities
{
    //!Real difference between XWebViewActivity and XWebViewTransparentActivity is in this attribute.
    [Activity(Label = "TransparentXWebViewActivity",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        Theme = "@style/XWebViewTheme.Transparent"
    )]
    public class TestTransparentActivity:BaseTestTransparentActivity
    {
        protected override async Task RunTest(AndroidXWebView iwv)
        {
            TestingEnvironment.Message("Will execute alert('Hello transparent!') in transparent webview.");
            await iwv.ExJsDirect("alert('Hello transparent!')");
        }
    }
}