using System;
using System.Threading.Tasks;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;
using IRO.XWebView.Extensions;

namespace IRO.Tests.XWebView.Core.Tests
{
    public class TestJQueryInclude : BaseXWebViewTest
    {
        protected override async Task RunTest()
        {
           ShowMessage($"Will include JQuery and than check if included.");
            var xwv = await XWVProvider.Resolve(XWebViewVisibility.Hidden);
            xwv.Disposing += delegate {ShowMessage($"XWebView disposed."); };
            await xwv.LoadUrl("about:blank");
            await xwv.AttachBridge();
            await xwv.IncludeJQueryIfNotIncluded();
            var isIncluded=await xwv.IsJQueryIncluded();
            xwv.Dispose();
            if (isIncluded)
            {
                ShowMessage("JQuery successfully included on page.");
            }
            else
            {
                throw new Exception("JQuery not included.");
            }
            
        }
    }
}