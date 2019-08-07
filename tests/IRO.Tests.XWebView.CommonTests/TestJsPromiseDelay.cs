using System.Threading.Tasks;
using IRO.XWebView.Core;

namespace IRO.Tests.XWebView.CommonTests
{
    public class TestJsPromiseDelay : IWebViewTest
    {
        public async Task RunTest(IXWebView xwv, ITestingEnvironment env)
        {
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
            var str = await xwv.ExJs<string>(
                "return delayPromise(5000);", 
                true
                );
            env.Message($"JsResult: '{str}'");
        }

    }
}