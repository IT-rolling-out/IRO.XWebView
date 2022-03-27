using System.Threading.Tasks;
using IRO.AndroidActivity;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;
using IRO.XWebView.Droid.Activities;
using IRO.XWebView.Droid.Renderer;

namespace IRO.XWebView.Droid.Providers
{
    public class NewActivityXWebViewProvider : BaseXWebViewProvider
    {
        protected override async Task<IXWebView> ProtectedResolve(XWebViewVisibility preferredVisibility)
        {
            if (preferredVisibility == XWebViewVisibility.Visible)
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