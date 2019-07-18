using System.Threading.Tasks;

namespace IRO.ImprovedWebView.Core
{
    public interface IImprovedWebViewProvider
    {
        string BrowserType { get; }

        Task<IImprovedWebView> Resolve(ImprovedWebViewVisibility visibility = ImprovedWebViewVisibility.Hidden);
    }
}