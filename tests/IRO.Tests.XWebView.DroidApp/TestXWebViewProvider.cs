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
            LastVisibility = visibility;
            var xwv = await base.Resolve(visibility);
            LastResolved = xwv;
            return xwv;
        }
    }
}