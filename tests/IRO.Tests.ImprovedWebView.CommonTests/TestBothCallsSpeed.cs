using System;
using System.Diagnostics;
using System.Threading.Tasks;
using IRO.ImprovedWebView.Core;

namespace IRO.Tests.ImprovedWebView.CommonTests
{
    public class TestBothCallsSpeed : IWebViewTest
    {
        public async Task RunTest(IImprovedWebView iwv, ITestingEnvironment env)
        {
            const int countTo = 500;

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
            env.Message($"Will count to {countTo}.");

            //In example we use js Inc method in c# and vice versa.
            //Don't forget to use 'return await', when want to get promise res.
            int value = 0;
            await iwv.AttachBridge();
            var sw = new Stopwatch();
            sw.Start();
            do
            {
                value = await iwv.ExJs<int>($"return JsInc({value});", true);
                value = await iwv.ExJs<int>($"return Native.Inc({value});", true);
            } while (value < countTo);
            sw.Stop();
            env.Message($"JsResult: {value}. Must be {countTo}.\nExecution total time {sw.ElapsedMilliseconds} ms.");
        }
    }
}