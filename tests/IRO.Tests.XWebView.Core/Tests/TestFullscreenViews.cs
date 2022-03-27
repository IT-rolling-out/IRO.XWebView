using System.Threading.Tasks;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;

namespace IRO.Tests.XWebView.Core.Tests
{
    public class TestFullscreenViews : BaseXWebViewTest
    {
        protected override async Task RunTest()
        {
            ShowMessage("Try to open video in fullscreen.");
            var xwv = await XWVProvider.Resolve(XWebViewVisibility.Visible);
            await xwv.LoadUrl("https://www.youtube.com/watch?v=_Z1VzsE1GVg");
        }
    }
}