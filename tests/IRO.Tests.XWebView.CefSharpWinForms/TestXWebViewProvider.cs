using System;
using System.Threading.Tasks;
using IRO.XWebView.CefSharp;
using IRO.XWebView.CefSharp.OffScreen.Providers;
using IRO.XWebView.CefSharp.WinForms.Providers;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;

namespace IRO.Tests.XWebView.CefSharpWinForms
{
    public class TestXWebViewProvider:IXWebViewProvider
    {
        public static bool UseOffScreenProvider { get; set; } = true;

        public WinFormsCefSharpXWebViewProvider VisibleProvider { get; } = new WinFormsCefSharpXWebViewProvider();

        public OffScreenCefSharpXWebViewProvider OffScreenProvider { get; } = new OffScreenCefSharpXWebViewProvider();

        [Obsolete("Used in old tests, but not now.")]
        public IXWebView LastResolved { get; private set; }

        [Obsolete("Used in old tests, but not now.")]
        public XWebViewVisibility LastVisibility { get; private set; }

        public async Task<IXWebView> Resolve(XWebViewVisibility preferredVisibility = XWebViewVisibility.Hidden)
        {
            LastVisibility = preferredVisibility;
            if (preferredVisibility == XWebViewVisibility.Hidden && UseOffScreenProvider)
            {
                //LastResolved = await WpfProvider.Resolve(preferredVisibility);
                LastResolved = await OffScreenProvider.Resolve(preferredVisibility);
            }
            else
            {
                LastResolved = await VisibleProvider.Resolve(preferredVisibility);
            }

            var xwv = (CefSharpXWebView) LastResolved;
            return LastResolved;
        }
    }
}
