using System.Threading.Tasks;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;
using IRO.XWebView.Extensions;

namespace IRO.Tests.XWebView.Core.Tests
{
    public class TestToast : IXWebViewTest
    {
        public async Task RunTest(IXWebViewProvider xwvProvider, ITestingEnvironment env, TestAppSetupConfigs appConfigs)
        {
            var xwv = await xwvProvider.Resolve(XWebViewVisibility.Visible);
            xwv.Disposing += delegate { env.Message($"XWebView disposed."); };
            await Task.Delay(1000);
            xwv.ShowToast("Crossplatform toast in webview.", 5000);
        }
    }
}