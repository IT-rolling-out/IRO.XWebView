using System.Threading.Tasks;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;
using IRO.XWebView.Extensions;
using Newtonsoft.Json;

namespace IRO.Tests.XWebView.Core.Tests
{
    public class TestGetHtmlViaJs : IXWebViewTest
    {
        public async Task RunTest(IXWebViewProvider xwvProvider, ITestingEnvironment env, TestAppSetupConfigs appConfigs)
        {
            var xwv = await xwvProvider.Resolve(XWebViewVisibility.Hidden);
            xwv.Disposing += delegate { env.Message($"XWebView disposed."); };
            await xwv.LoadUrl("http://google.com");
            var html = await xwv.GetHtml();
            xwv.Dispose();
            env.Message("HTML FROM google.com :\n\n" + html);
        }
    }
}