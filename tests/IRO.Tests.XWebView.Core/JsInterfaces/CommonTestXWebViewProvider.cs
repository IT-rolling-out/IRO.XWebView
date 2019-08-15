using System.Threading.Tasks;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;

namespace IRO.Tests.XWebView.Core.JsInterfaces
{
    public class CommonTestXWebViewProvider : IXWebViewProvider
    {
        public CommonTestXWebViewProvider(IXWebViewProvider origProvider)
        {
            OrigProvider = origProvider;
        }

        public IXWebViewProvider OrigProvider { get; }

        public IXWebView LastResolved { get; private set; }

        public XWebViewVisibility LastVisibility { get; private set; }

        public async Task<IXWebView> Resolve(XWebViewVisibility prefferedVisibility = XWebViewVisibility.Hidden)
        {
            LastVisibility = prefferedVisibility;
            LastResolved = await OrigProvider.Resolve(prefferedVisibility);
            return LastResolved;
        }
    }
}