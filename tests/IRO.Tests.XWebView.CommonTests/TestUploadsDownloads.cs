using System.Threading.Tasks;
using IRO.XWebView.Core;

namespace IRO.Tests.XWebView.CommonTests
{
    public class TestUploadsDownloads : IWebViewTest
    {
        public async Task RunTest(IXWebView xwv, ITestingEnvironment env)
        {
            await xwv.WaitWhileBusy();
            var loadRes = await xwv.LoadUrl("https://dropmefiles.com");
        }
    }
}