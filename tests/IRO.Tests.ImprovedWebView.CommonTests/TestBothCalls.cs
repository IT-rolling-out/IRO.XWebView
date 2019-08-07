using System;
using System.Threading.Tasks;
using IRO.ImprovedWebView.Core;

namespace IRO.Tests.ImprovedWebView.CommonTests
{
    public class TestBothCalls : IWebViewTest
    {
        public async Task RunTest(IImprovedWebView iwv, ITestingEnvironment env)
        {
            //Register Inc method in js and c#.
            Func<int, Task<int>> inc = async (num) => num + 1;
            iwv.BindToJs(inc, "Inc", "Native");
            var jsIncScript = @"
window['JsInc'] = function(num){
  return new Promise(
    function(resolve,reject)
    {
      resolve(num+1);
    });
}
";
            await iwv.ExJsDirect(jsIncScript);

            //In example we use js Inc method in c# and vice versa.
            //Don't forget to use 'return await', when want to get promise res.
            int value = 0;
            await iwv.AttachBridge();
            value = await iwv.ExJs<int>($"return JsInc({value});", true);
            value = await iwv.ExJs<int>($"return Native.Inc({value});", true);
            value = await iwv.ExJs<int>($"return JsInc({value});", true);
            value = await iwv.ExJs<int>($"return Native.Inc({value});", true);
            value = await iwv.ExJs<int>($"return JsInc({value});", true);
            value = await iwv.ExJs<int>($"return Native.Inc({value});", true);
            env.Message($"JsResult: {value}. Must be 6.");
        }
    }
}