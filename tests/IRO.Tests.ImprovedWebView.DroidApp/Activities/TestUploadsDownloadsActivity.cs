using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using IRO.ImprovedWebView.Droid;
using IRO.Tests.ImprovedWebView.CommonTests;

namespace IRO.Tests.ImprovedWebView.DroidApp.Activities
{
    [Activity(Label = "TestUploadsDownloadsActivity", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class TestUploadsDownloadsActivity : BaseTestActivity
    {
        protected override async Task RunTest(AndroidImprovedWebView iwv)
        {
            var test = new TestUploadsDownloads();
            await test.RunTest(iwv, TestingEnvironment);
        }
    }
}