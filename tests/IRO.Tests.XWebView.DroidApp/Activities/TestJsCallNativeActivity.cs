using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using IRO.XWebView.Core;
using IRO.XWebView.Droid;
using IRO.Tests.XWebView.CommonTests;

namespace IRO.Tests.XWebView.DroidApp.Activities
{
    [Activity(Label = "TestJsCallNativeActivity", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class TestJsCallNativeActivity : BaseTestActivity
    {
        protected override async Task RunTest(AndroidXWebView iwv)
        {
            var test = new TestJsCallNative();
            await test.RunTest(iwv, TestingEnvironment);
        }
    }
}