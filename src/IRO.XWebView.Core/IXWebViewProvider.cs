using System.Threading.Tasks;

namespace IRO.XWebView.Core
{
    public interface IXWebViewProvider
    {
        string BrowserType { get; }

        Task<IXWebView> Resolve(XWebViewVisibility visibility = XWebViewVisibility.Hidden);
    }
}