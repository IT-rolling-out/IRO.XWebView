using System.Collections.Generic;
using System.Threading.Tasks;
using IRO.XWebView.Core.Consts;

namespace IRO.XWebView.Core.Providers
{
    public interface IXWebViewProvider
    {
        /// <summary>
        /// List of all xwv resolved by this provider. When xwv is disposing - it will be removed from here.
        /// </summary>
        IReadOnlyCollection<IXWebView> ProvidedXWebView { get; }

        Task<IXWebView> Resolve(XWebViewVisibility preferredVisibility = XWebViewVisibility.Hidden);

        /// <summary>
        /// Disposing all XWebView instances resolved by current provided.
        /// </summary>
        void DisposeProvidedXWebView();
    }
}