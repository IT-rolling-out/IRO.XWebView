using System.Threading.Tasks;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;

namespace IRO.Tests.XWebView.Core.Tests
{
    public class TestUploadsDownloads : BaseXWebViewTest
    {
        protected override async Task RunTest()
        {
            var xwv = await XWVProvider.Resolve(XWebViewVisibility.Visible);
            await xwv.WaitWhileNavigating();
            var loadRes = await xwv.LoadUrl("https://file.pizza/");
        }
    }
}