using System.Threading.Tasks;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;
using IRO.XWebView.Extensions;

namespace IRO.Tests.XWebView.Core.Tests
{
    public class TestToast : BaseXWebViewTest
    {
        protected override async Task RunTest()
        {
            var xwv = await XWVProvider.Resolve(XWebViewVisibility.Visible);
            xwv.Disposing += delegate {ShowMessage($"XWebView disposed."); };
            await Task.Delay(1000);
            await xwv.ShowToast("Crossplatform toast in webview.", 5000);
        }
    }
}