using System.Threading.Tasks;
using IRO.AndroidActivity;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;
using IRO.XWebView.Droid.Activities;
using IRO.XWebView.Droid.Renderer;

namespace IRO.XWebView.Droid.Providers
{
    public class NewActivityXWebViewProvider : IXWebViewProvider
    {
        public virtual async Task<IXWebView> Resolve(XWebViewVisibility prefferedVisibility = XWebViewVisibility.Hidden)
        {
            if (prefferedVisibility == XWebViewVisibility.Visible)
            {
                var webViewActivity = await ActivityExtensions.StartNewActivity<XWebViewActivity>();
                return await AndroidXWebView.Create(webViewActivity);
            }
            else
            {
                var webViewActivity = await ActivityExtensions.StartNewActivity<XWebViewTransparentActivity>();
                return await AndroidXWebView.Create(webViewActivity);
            }
        }
    }
}