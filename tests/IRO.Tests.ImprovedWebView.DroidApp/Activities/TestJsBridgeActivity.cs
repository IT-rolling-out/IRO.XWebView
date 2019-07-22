using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using IRO.ImprovedWebView.Core;
using IRO.ImprovedWebView.Droid;

namespace IRO.Tests.ImprovedWebView.DroidApp.Activities
{
    [Activity(Label = "TestLoadingActivity", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class TestJsBridgeActivity : BaseTestActivity
    {
        protected override async Task RunTest(AndroidImprovedWebView iwv)
        {
            //NOTE: all your js interfaces with Promises and Exceptions support
            //will not be available by default on each page, because it require initialization script
            //invokation.
            //If you wan't use those features on you page (for example, index.html from Assets)
            //you can init it before usage by calling 'window.eval(NativeBridge.GetAttachBridgeScript());'.
            //or use method below to inject it on page in c# code, AFTER page loaded.

            iwv.BindToJs(new JsToNativeBridge(), "Native");
            var script = @"
(async function(){
document.write('<br>await Native.Reverse(\'my message from js\');');
var res = await Native.Reverse('my message from js');
document.write('<br> -> '+JSON.stringify(res));


document.write('<br>Native.Inc(5);');
res = Native.Inc(5);
document.write('<br> -> '+JSON.stringify(res));


document.write('<br>Native.ErrorSync();');
try{
res = Native.ErrorSync();
}catch(ex){
  res = ex;
}
document.write('<br> -> exception '+JSON.stringify(res));


document.write('<br>await Native.ErrorAsync();');
try{
res = await Native.ErrorAsync();
}catch(ex){
  res = ex;
}
document.write('<br> -> exception '+JSON.stringify(res));
})();
";
            await iwv.AttachBridge();
            await iwv.ExJsDirect(script);
        }

        class JsToNativeBridge
        {
            public async Task<string> Reverse(string str)
            {
                await Task.Delay(500);
                var charArray = str.ToCharArray();
                Array.Reverse(charArray);
                return new string(charArray);
            }

            public int Inc(int num)
            {
                return num + 1;
            }

            public async Task ErrorAsync()
            {
                await Task.Delay(500);
                throw new Exception("Exception from c# sync.");
            }

            public void ErrorSync()
            {
                throw new Exception("Exception from c# async.");
            }
        }
    }
}