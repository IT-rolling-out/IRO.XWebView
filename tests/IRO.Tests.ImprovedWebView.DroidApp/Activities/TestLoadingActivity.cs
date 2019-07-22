using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using IRO.ImprovedWebView.Core;
using IRO.ImprovedWebView.Droid;

namespace IRO.Tests.ImprovedWebView.DroidApp.Activities
{
    [Activity(Label = "TestLoadingActivity", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class TestLoadingActivity : BaseTestActivity
    {
        protected override async Task RunTest(AndroidImprovedWebView iwv)
        {
            //Choose websites that can load long time.
            //This three must be aborted in test.
            iwv.TryLoadUrl("https://stackoverflow.com");
            Application.SynchronizationContext.Send((obj) =>
            {
                CurrentWebView.LoadUrl("https://twitter.com");
            }, null);
            iwv.TryLoadUrl("https://visualstudio.microsoft.com/ru/");

            var loadRes = await iwv.LoadUrl("https://www.youtube.com/");
            ShowMessage($"Loaded {loadRes.Url}");
            loadRes = await iwv.LoadUrl("https://www.google.com/");
            ShowMessage($"Loaded {loadRes.Url}");
            loadRes = await iwv.Reload();
            ShowMessage($"Reloaded {loadRes.Url}");
        }
    }
}