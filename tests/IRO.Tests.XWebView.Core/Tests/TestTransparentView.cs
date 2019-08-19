using System.Threading.Tasks;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;

namespace IRO.Tests.XWebView.Core.Tests
{
    public class TestTransparentView : IXWebViewTest
    {
        public async Task RunTest(IXWebViewProvider xwvProvider, ITestingEnvironment env, TestAppSetupConfigs appConfigs)
        {
            env.Message("Will execute alert('Hello transparent!') in transparent webview.");
            var xwv = await xwvProvider.Resolve(XWebViewVisibility.Hidden);
            await xwv.ExJs<object>("alert('Hello from transparent!')");
            xwv.Dispose();
            env.Message("Hidden XWebView disposed automatically.");
        }
    }
}