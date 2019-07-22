using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using IRO.ImprovedWebView.Droid;

namespace IRO.Tests.ImprovedWebView.DroidApp.Activities
{
     [Activity(Label = "TestJsPromiseDelayActivity", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class TestJsPromiseDelayActivity : BaseTestActivity
    {
        protected override async Task RunTest(AndroidImprovedWebView iwv)
        {
            var delayScript = @"
window['delayPromise'] = function(delayMS) {
  return new Promise(function(resolve, reject){
    setTimeout(function(){
      resolve({});
    }, delayMS)
  });
}
";
            await iwv.ExJs<string>(delayScript);
            var str = await iwv.ExJs<string>("await delayPromise(5000); return 'Awaited message from js';", true);
            ShowMessage($"JsResult: '{str}'");
        }
    }
}