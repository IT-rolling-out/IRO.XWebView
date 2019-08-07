using System.Threading.Tasks;
using IRO.ImprovedWebView.Core;

namespace IRO.Tests.ImprovedWebView.CommonTests
{
    public class TestJsAwaitDelay : IWebViewTest
    {
        public async Task RunTest(IImprovedWebView iwv, ITestingEnvironment env)
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
            await iwv.AttachBridge();

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
            env.Message($"JsResult: '{str}'");
        }

    }
}