using System;
using System.Threading.Tasks;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;

namespace IRO.Tests.XWebView.Core.Tests
{
    public class TestBothCalls : BaseXWebViewTest
    {
        protected override async Task RunTest()
        {
            var xwv = await XWVProvider.Resolve(XWebViewVisibility.Hidden);
            xwv.Disposing += delegate { ShowMessage($"XWebView disposed."); };
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
            await xwv.ExJs<object>(jsIncScript);

            //In example we use js Inc method in c# and vice versa.
            //Don't forget to use 'return await', when want to get promise res.
            var value = 0;
            await xwv.AttachBridge();
            value = await xwv.ExJs<int>($"return JsInc({value});", true);

            value = await xwv.ExJs<int>($"return Native.Inc({value});", true);
            value = await xwv.ExJs<int>($"return JsInc({value});", true);
            value = await xwv.ExJs<int>($"return Native.Inc({value});", true);
            value = await xwv.ExJs<int>($"return JsInc({value});", true);
            value = await xwv.ExJs<int>($"return Native.Inc({value});", true);
            xwv.Dispose();
            ShowMessage($"JsResult: {value}. Must be 6.");

        }
    }
}