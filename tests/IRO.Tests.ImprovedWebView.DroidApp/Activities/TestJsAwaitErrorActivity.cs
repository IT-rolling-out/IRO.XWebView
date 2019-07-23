using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using IRO.ImprovedWebView.Droid;

namespace IRO.Tests.ImprovedWebView.DroidApp.Activities
{
    [Activity(Label = "TestJsAwaitErrorActivity",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class TestJsAwaitErrorActivity : BaseTestActivity
    {
        protected override async Task RunTest(AndroidImprovedWebView iwv)
        {
            //Rejected after delay.
            var delayScript = @"
window['delayPromiseError'] = function(delayMS) {
  return new Promise(function(resolve, reject){
    setTimeout(function(){
      reject('-----REJECT PASSED MESSAGE-----');
    }, delayMS)
  });
}";
            await iwv.ExJs<string>(delayScript);

            //ES7, chromium 55+ required.
            string scriptWithError = @"
return (async function(){
  await delayPromiseError(5000); 
})();";
            try
            {
                await iwv.ExJs<string>(scriptWithError, true);
            }
            catch (System.Exception ex)
            {
                if (!ex.ToString().Contains("-----REJECT PASSED MESSAGE-----"))
                    throw;
                ShowMessage($"Test successful.\nCatched exception from promise: '{ex.ToString()}'");
            }


        }
    }
}