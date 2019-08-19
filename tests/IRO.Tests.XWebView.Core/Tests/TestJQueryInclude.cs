using System;
using System.Threading.Tasks;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;
using IRO.XWebView.Extensions;

namespace IRO.Tests.XWebView.Core.Tests
{
    public class TestJQueryInclude : IXWebViewTest
    {
        public async Task RunTest(IXWebViewProvider xwvProvider, ITestingEnvironment env, TestAppSetupConfigs appConfigs)
        {
            env.Message($"Will include JQuery and than check if included.");
            var xwv = await xwvProvider.Resolve(XWebViewVisibility.Visible);
            await xwv.LoadUrl("about:blank");
            await xwv.AttachBridge();
            await xwv.IncludeJQueryIfNotIncluded();
            var isIncluded=await xwv.IsJQueryIncluded();
            if (isIncluded)
            {
                env.Message("JQuery successfully included on page.");
            }
            else
            {
                throw new Exception("JQuery not included.");
            }
        }
    }
}