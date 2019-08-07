using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using IRO.XWebView.Core;
using IRO.XWebView.Droid;
using IRO.Tests.XWebView.CommonTests;

namespace IRO.Tests.XWebView.DroidApp.Activities
{
    [Activity(Label = "TestBothCallsSpeedTestActivity", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class TestBothCallsSpeedActivity : BaseTestActivity
    {
        protected override async Task RunTest(AndroidXWebView iwv)
        {
            var test = new TestBothCallsSpeed();
            await test.RunTest(iwv, TestingEnvironment);
        }
    }
}