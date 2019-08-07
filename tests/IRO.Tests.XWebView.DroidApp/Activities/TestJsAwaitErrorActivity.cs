using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using IRO.XWebView.Droid;
using IRO.Tests.XWebView.CommonTests;

namespace IRO.Tests.XWebView.DroidApp.Activities
{
    [Activity(Label = "TestJsAwaitErrorActivity",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class TestJsAwaitErrorActivity : BaseTestActivity
    {
        protected override async Task RunTest(AndroidXWebView iwv)
        {
            var test = new TestJsAwaitError();
            await test.RunTest(iwv, TestingEnvironment);
        }
    }
}