﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;

namespace IRO.Tests.XWebView.Core.Tests
{
    public class TestBothCallsSpeed : IXWebViewTest
    {
        public async Task RunTest(IXWebViewProvider xwvProvider, ITestingEnvironment env, TestAppSetupConfigs appConfigs)
        {
            const int countTo = 500;
            var xwv = await xwvProvider.Resolve(XWebViewVisibility.Hidden);
            xwv.Disposing += delegate { env.Message($"XWebView disposed."); };

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
            env.Message($"Will count to {countTo}.");

            //In example we use js Inc method in c# and vice versa.
            //Don't forget to use 'return await', when want to get promise res.
            var value = 0;
            await xwv.AttachBridge();
            var sw = new Stopwatch();
            sw.Start();
            do
            {
                value = await xwv.ExJs<int>($"return JsInc({value});", true);
                value = await xwv.ExJs<int>($"return Native.Inc({value});", true);
            } while (value < countTo);

            sw.Stop();
            xwv.Dispose();
            env.Message($"JsResult: {value}. Must be {countTo}.\nExecution total time {sw.ElapsedMilliseconds} ms.");
        }
    }
}