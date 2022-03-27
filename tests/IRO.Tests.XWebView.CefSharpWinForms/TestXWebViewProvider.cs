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
    public class TestXWebViewProvider : BaseXWebViewProvider
    {
        public static bool UseOffScreenProvider { get; set; } = true;

        public WinFormsCefSharpXWebViewProvider WpfProvider { get; } = new WinFormsCefSharpXWebViewProvider();

        public OffScreenCefSharpXWebViewProvider OffScreenProvider { get; } = new OffScreenCefSharpXWebViewProvider();

        public IXWebView LastResolved { get; private set; }

        public XWebViewVisibility LastVisibility { get; private set; }

        protected override async Task<IXWebView> ProtectedResolve(XWebViewVisibility preferredVisibility)
        {
            LastVisibility = preferredVisibility;
            if (preferredVisibility == XWebViewVisibility.Hidden && UseOffScreenProvider)
            {
                //LastResolved = await WpfProvider.Resolve(preferredVisibility);
                LastResolved = await OffScreenProvider.Resolve(preferredVisibility);
            }
            else
            {
                LastResolved = await WpfProvider.Resolve(preferredVisibility);
            }

            var xwv = (CefSharpXWebView)LastResolved;
            return LastResolved;
        }
    }

}
