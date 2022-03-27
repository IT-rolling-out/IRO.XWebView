using System.Threading.Tasks;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;

namespace IRO.Tests.XWebView.Core.Tests
{
    public class TestJsAwaitDelay : BaseXWebViewTest
    {
        protected override async Task RunTest()
        {
            var xwv = await XWVProvider.Resolve(XWebViewVisibility.Hidden);
            xwv.Disposing += delegate { ShowMessage($"XWebView disposed."); };
            var delayScript = @"
window['delayPromise'] = function(delayMS) {
  return new Promise(function(resolve, reject){
    setTimeout(function(){
      resolve('');
    }, delayMS)
  });
}
";
            await xwv.ExJs<string>(delayScript);
            await xwv.AttachBridge();

            //ES7, chromium 55+ required.
            //Easy way to create chain of callbacks, unfortunately not supported on old devices.
            var scriptWithAwaits = @"
return (async function(){
  await delayPromise(2500); 
  await delayPromise(2500); 
  return 'Awaited message from js, that use ES7 async/await';
})();
";
            var str = await xwv.ExJs<string>(scriptWithAwaits, true);
            xwv.Dispose();
            ShowMessage($"JsResult: '{str}'");
        }
    }
}