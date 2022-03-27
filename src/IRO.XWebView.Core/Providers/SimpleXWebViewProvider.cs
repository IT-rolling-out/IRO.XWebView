using System;
using System.Threading.Tasks;
using IRO.XWebView.Core.Consts;

namespace IRO.XWebView.Core.Providers
{
    public class SimpleXWebViewProvider : BaseXWebViewProvider
    {
        readonly Func<XWebViewVisibility, Task<IXWebView>> _providerDelegate;

        /// <summary>
        /// Provider from instance.
        /// </summary>
        public SimpleXWebViewProvider(IXWebView xWebView)
        {
            if (xWebView == null) throw new ArgumentNullException(nameof(xWebView));
            _providerDelegate = async (v) => xWebView;
        }

        /// <summary>
        /// Provider from delegate.
        /// </summary>
        public SimpleXWebViewProvider(Func<XWebViewVisibility, Task<IXWebView>> providerDelegate)
        {
            _providerDelegate = providerDelegate ?? throw new ArgumentNullException(nameof(providerDelegate));
        }

        protected override async Task<IXWebView> ProtectedResolve(XWebViewVisibility preferredVisibility)
        {
            var xwv = await _providerDelegate(preferredVisibility);
            if (xwv.Visibility != preferredVisibility)
            {
                xwv.Visibility = preferredVisibility;
            }
            return xwv;
        }
    }
}