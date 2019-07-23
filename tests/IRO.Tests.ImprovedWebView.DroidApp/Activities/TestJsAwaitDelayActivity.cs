using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using IRO.ImprovedWebView.Droid;

namespace IRO.Tests.ImprovedWebView.DroidApp.Activities
{
    [Activity(Label = "TestJsAwaitDelayActivity",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class TestJsAwaitDelayActivity : BaseTestActivity
    {
        protected override async Task RunTest(AndroidImprovedWebView iwv)
        {
            var delayScript = @"
window['delayPromise'] = function(delayMS) {
  return new Promise(function(resolve, reject){
    setTimeout(function(){
      resolve('');
    }, delayMS)
  });
}
";
            await iwv.ExJs<string>(delayScript);

            //ES7, chromium 55+ required.
            //Easy way to create chain of callbacks, unfortunately not supported on old devices.
            string scriptWithAwaits = @"
return (async function(){
  await delayPromise(2500); 
  await delayPromise(2500); 
  return 'Awaited message from js, that use ES7 async/await';
})();
";
            var str = await iwv.ExJs<string>(scriptWithAwaits, true);
            ShowMessage($"JsResult: '{str}'");
        }

    }
}