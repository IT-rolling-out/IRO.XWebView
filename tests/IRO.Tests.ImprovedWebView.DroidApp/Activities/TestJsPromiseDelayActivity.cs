using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using IRO.ImprovedWebView.Droid;
using IRO.Tests.ImprovedWebView.CommonTests;

namespace IRO.Tests.ImprovedWebView.DroidApp.Activities
{
    [Activity(Label = "TestJsPromiseDelayActivity",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class TestJsPromiseDelayActivity : BaseTestActivity
    {
        protected override async Task RunTest(AndroidImprovedWebView iwv)
        {
            var test = new TestJsPromiseDelay();
            await test.RunTest(iwv, TestingEnvironment);
        }

    }
}