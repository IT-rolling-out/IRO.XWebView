using System.Threading.Tasks;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;

namespace IRO.Tests.XWebView.Core.Tests
{
    public class TestFullscreenViews : IXWebViewTest
    {
        public async Task RunTest(IXWebViewProvider xwvProvider, ITestingEnvironment env)
        {
            env.Message("Try to open video in fullscreen.");
            var xwv = await xwvProvider.Resolve(XWebViewVisibility.Visible);
            await xwv.LoadUrl("https://www.youtube.com/watch?v=z0Zd1Nsyrq4");
        }
    }
}