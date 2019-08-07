﻿using System.Threading.Tasks;
using IRO.XWebView.Core;

namespace IRO.Tests.XWebView.CommonTests
{
    public class TestJsAwaitError : IWebViewTest
    {
        public async Task RunTest(IXWebView xwv, ITestingEnvironment env)
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
            await xwv.ExJs<string>(delayScript);
            await xwv.AttachBridge();

            //ES7, chromium 55+ required.
            string scriptWithError = @"
return (async function(){
  await delayPromiseError(5000); 
})();";
            try
            {
                await xwv.ExJs<string>(scriptWithError, true);
            }
            catch (System.Exception ex)
            {
                if (!ex.ToString().Contains("-----REJECT PASSED MESSAGE-----"))
                    throw;
                env.Message($"Test successful.\nCatched exception from promise: '{ex.ToString()}'");
            }


        }
    }
}