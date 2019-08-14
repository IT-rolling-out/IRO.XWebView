using System;
using System.Threading.Tasks;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;

namespace IRO.Tests.XWebView.Core
{
    public class TestJsCallNative : IXWebViewTest
    {
        public async Task RunTest(IXWebViewProvider xwvProvider, ITestingEnvironment env)
        {
            var xwv = await xwvProvider.Resolve(XWebViewVisibility.Visible);
            //NOTE: all your js interfaces with Promises and Exceptions support
            //will not be available by default on each page, because it require initialization script
            //invokation.
            //If you wan't use those features on you page (for example, index.html from Assets)
            //you can init it before usage by calling 'window.eval(NativeBridge.GetAttachBridgeScript());'.
            //or use method below to inject it on page in c# code, AFTER page loaded.

            xwv.BindToJs(new JsToNativeBridgeTestObject(), "Native");
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
            await xwv.AttachBridge();
            await xwv.ExJs<object>(script);
        }

        class JsToNativeBridgeTestObject
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