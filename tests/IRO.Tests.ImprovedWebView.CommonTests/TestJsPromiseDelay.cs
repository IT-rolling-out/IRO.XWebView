using System.Threading.Tasks;
using IRO.ImprovedWebView.Core;

namespace IRO.Tests.ImprovedWebView.CommonTests
{
    public class TestJsPromiseDelay : IWebViewTest
    {
        public async Task RunTest(IImprovedWebView iwv, ITestingEnvironment env)
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
            await iwv.ExJs<string>(delayScript);
            //Even if you wan't to await promise from js you must attach bridge (to init callbacks support script).
            await iwv.AttachBridge();
            var str = await iwv.ExJs<string>(
                "return delayPromise(5000);", 
                true
                );
            env.Message($"JsResult: '{str}'");
        }

    }
}