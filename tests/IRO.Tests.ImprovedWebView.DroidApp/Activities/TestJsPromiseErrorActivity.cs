using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using IRO.ImprovedWebView.Droid;

namespace IRO.Tests.ImprovedWebView.DroidApp.Activities
{
    [Activity(Label = "TestJsPromiseErrorActivity",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class TestJsPromiseErrorActivity : BaseTestActivity
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
}
";
            await iwv.ExJs<string>(delayScript);
            try
            {
                var str = await iwv.ExJs<string>("await delayPromiseError(5000);", true);
            }
            catch (System.Exception ex)
            {
                ShowMessage($"Test successful.\nCatched exception from promise: '{ex.ToString()}'");
            }


        }
    }
}