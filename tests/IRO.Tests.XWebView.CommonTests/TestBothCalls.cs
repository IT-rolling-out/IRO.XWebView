using System;
using System.Threading.Tasks;
using IRO.XWebView.Core;

namespace IRO.Tests.XWebView.CommonTests
{
    public class TestBothCalls : IWebViewTest
    {
        public async Task RunTest(IXWebView xwv, ITestingEnvironment env)
        {
            //Register Inc method in js and c#.
            Func<int, Task<int>> inc = async (num) => num + 1;
            xwv.BindToJs(inc, "Inc", "Native");
            var jsIncScript = @"
window['JsInc'] = function(num){
  return new Promise(
    function(resolve,reject)
    {
      resolve(num+1);
    });
}
";
            await xwv.ExJsDirect(jsIncScript);

            //In example we use js Inc method in c# and vice versa.
            //Don't forget to use 'return await', when want to get promise res.
            int value = 0;
            await xwv.AttachBridge();
            value = await xwv.ExJs<int>($"return JsInc({value});", true);
            value = await xwv.ExJs<int>($"return Native.Inc({value});", true);
            value = await xwv.ExJs<int>($"return JsInc({value});", true);
            value = await xwv.ExJs<int>($"return Native.Inc({value});", true);
            value = await xwv.ExJs<int>($"return JsInc({value});", true);
            value = await xwv.ExJs<int>($"return Native.Inc({value});", true);
            env.Message($"JsResult: {value}. Must be 6.");
        }
    }
}