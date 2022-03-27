using System.Threading.Tasks;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;

namespace IRO.Tests.XWebView.Core.Tests
{
    public class TestJsPromiseDelay : BaseXWebViewTest
    {
        protected override async Task RunTest()
        {
            var xwv = await XWVProvider.Resolve(XWebViewVisibility.Hidden);
            xwv.Disposing += delegate { ShowMessage($"XWebView disposed."); };
            var delayScript = @"
window['delayPromise'] = function(delayMS) {
  return new Promise(function(resolve, reject){
    setTimeout(function(){
      resolve('Promise message from js');
    }, delayMS)
  });
}
";
            await xwv.ExJs<string>(delayScript);
            //Even if you wan't to await promise from js you must attach bridge (to init callbacks support script).
            await xwv.AttachBridge();
            ShowMessage("Start waiting.");
            var str = await xwv.ExJs<string>(
                "return delayPromise(5000);",
                true
                );
            xwv.Dispose();
            ShowMessage($"JsResult: '{str}'");
        }
    }
}