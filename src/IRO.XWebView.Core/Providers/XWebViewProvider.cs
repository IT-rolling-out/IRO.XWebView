using System;
using System.Threading.Tasks;
using IRO.XWebView.Core.Consts;

namespace IRO.XWebView.Core.Providers
{
    public class XWebViewProvider : IXWebViewProvider
    {
        readonly Func<XWebViewVisibility, Task<IXWebView>> _providerDelegate;

        /// <summary>
        /// Provider from instance.
        /// </summary>
        public XWebViewProvider(IXWebView xWebView)
        {
            if (xWebView == null) throw new ArgumentNullException(nameof(xWebView));
            _providerDelegate = async (v) => xWebView;
        }

        /// <summary>
        /// Provider from instance.
        /// </summary>
        public XWebViewProvider(Func<XWebViewVisibility, Task<IXWebView>> providerDelegate)
        {
            _providerDelegate = providerDelegate ?? throw new ArgumentNullException(nameof(providerDelegate));
        }

        public async Task<IXWebView> Resolve(XWebViewVisibility prefferedVisibility = XWebViewVisibility.Hidden)
        {
            var xwv = await _providerDelegate(prefferedVisibility);
            if (xwv.Visibility != prefferedVisibility)
            {
                xwv.Visibility = prefferedVisibility;
            }
            return xwv;
        }
    }
}