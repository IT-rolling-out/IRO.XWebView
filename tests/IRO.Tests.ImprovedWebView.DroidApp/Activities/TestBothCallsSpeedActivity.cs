using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using IRO.ImprovedWebView.Core;
using IRO.ImprovedWebView.Droid;
using IRO.Tests.ImprovedWebView.CommonTests;

namespace IRO.Tests.ImprovedWebView.DroidApp.Activities
{
    [Activity(Label = "TestBothCallsSpeedTestActivity", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class TestBothCallsSpeedActivity : BaseTestActivity
    {
        protected override async Task RunTest(AndroidImprovedWebView iwv)
        {
            var test = new TestBothCallsSpeed();
            await test.RunTest(iwv, TestingEnvironment);
        }
    }
}