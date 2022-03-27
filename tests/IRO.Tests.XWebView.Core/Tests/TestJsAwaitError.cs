using System;
using System.Threading.Tasks;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;

namespace IRO.Tests.XWebView.Core.Tests
{
    public class TestJsAwaitError : BaseXWebViewTest
    {
        protected override async Task RunTest()
        {
            var xwv = await XWVProvider.Resolve(XWebViewVisibility.Hidden);
            xwv.Disposing += delegate { ShowMessage($"XWebView disposed."); };
            //Rejected after delay.
            var delayScript = @"
window['delayPromiseError'] = function(delayMS) {
  return new Promise(function(resolve, reject){
    setTimeout(function(){
      reject('-----REJECT PASSED MESSAGE-----');
    }, delayMS)
  });
}";
            await xwv.ExJs<string>(delayScript);
            await xwv.AttachBridge();

            //ES7, chromium 55+ required.
            var scriptWithError = @"
return (async function(){
  await delayPromiseError(5000); 
})();";
            try
            {
                await xwv.ExJs<string>(scriptWithError, true);
            }
            catch (Exception ex)
            {
                if (!ex.ToString().Contains("-----REJECT PASSED MESSAGE-----"))
                    throw;
                ShowMessage($"Test successful.\nCatched exception from promise: '{ex.ToString()}'");
            }
            finally
            {
                xwv.Dispose();
            }
        }
    }
}