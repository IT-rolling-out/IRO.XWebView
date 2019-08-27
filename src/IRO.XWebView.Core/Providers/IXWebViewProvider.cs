using System.Threading.Tasks;
using IRO.XWebView.Core.Consts;

namespace IRO.XWebView.Core.Providers
{
    public interface IXWebViewProvider
    {
        Task<IXWebView> Resolve(XWebViewVisibility preferredVisibility = XWebViewVisibility.Hidden);
    }
}