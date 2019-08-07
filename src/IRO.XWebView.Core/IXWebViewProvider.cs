using System.Threading.Tasks;
using IRO.XWebView.Core.Consts;

namespace IRO.XWebView.Core
{
    public interface IXWebViewProvider
    {
        Task<IXWebView> Resolve(XWebViewVisibility visibility = XWebViewVisibility.Hidden);
    }
}