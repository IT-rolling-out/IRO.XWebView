using System.Threading.Tasks;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;
using IRO.XWebView.Extensions;
using Newtonsoft.Json;

namespace IRO.Tests.XWebView.Core.Tests
{
    public class TestGetHtmlViaJs : BaseXWebViewTest
    {
        protected override async Task RunTest()
        {
            var xwv = await XWVProvider.Resolve(XWebViewVisibility.Hidden);
            xwv.Disposing += delegate {ShowMessage($"XWebView disposed."); };
            await xwv.LoadUrl("http://google.com");
            var html = await xwv.GetHtml();
            xwv.Dispose();
            ShowMessage("HTML FROM google.com :\n\n" + html);
        }
    }
}