using System.Threading.Tasks;
using IRO.ImprovedWebView.Core;

namespace IRO.Tests.ImprovedWebView.CommonTests
{
    public class TestUploadsDownloads : IWebViewTest
    {
        public async Task RunTest(IImprovedWebView iwv, ITestingEnvironment env)
        {
            await iwv.WaitWhileBusy();
            var loadRes = await iwv.LoadUrl("https://dropmefiles.com");
        }
    }
}