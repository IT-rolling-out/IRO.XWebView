using System.Threading.Tasks;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;

namespace IRO.Tests.XWebView.Core.Tests
{
    public class TestTransparentView : BaseXWebViewTest
    {
        protected override async Task RunTest()
        {
            ShowMessage("Will execute alert('Hello transparent!') in transparent webview.");
            var xwv = await XWVProvider.Resolve(XWebViewVisibility.Hidden);
            await xwv.ExJs<object>("alert('Hello from transparent!')");
            xwv.Dispose();
            ShowMessage("Hidden XWebView disposed automatically.");
        }
    }
}