using System.Threading.Tasks;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Droid.Providers;

namespace IRO.Tests.XWebView.DroidApp
{
    public class TestXWebViewProvider : NewActivityXWebViewProvider
    {
        public IXWebView LastResolved { get; private set; }

        public XWebViewVisibility LastVisibility { get; private set; }

        public override async Task<IXWebView> Resolve(XWebViewVisibility prefferedVisibility = XWebViewVisibility.Hidden)
        {
            LastVisibility = prefferedVisibility;
            var xwv = await base.Resolve(prefferedVisibility);
            LastResolved = xwv;
            return xwv;
        }
    }
}