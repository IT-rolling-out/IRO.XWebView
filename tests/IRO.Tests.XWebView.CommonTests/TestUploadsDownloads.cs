using System.Threading.Tasks;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;

namespace IRO.Tests.XWebView.CommonTests
{
    public class TestUploadsDownloads : IXWebViewTest
    {
        public async Task RunTest(IXWebViewProvider xwvProvider, ITestingEnvironment env)
        {
            var xwv = await xwvProvider.Resolve(XWebViewVisibility.Visible);
            await xwv.WaitWhileBusy();
            var loadRes = await xwv.LoadUrl("https://dropmefiles.com");
        }
    }
}