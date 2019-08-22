using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IRO.XWebView.CefSharp;
using IRO.XWebView.CefSharp.Wpf.Providers;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;

namespace IRO.Tests.XWebView.CefSharpWpf
{
    public class TestXWebViewProvider:IXWebViewProvider
    {
        public static bool UseWpfProvider { get; set; } = true;

        public WpfCefSharpXWebViewProvider WpfProvider { get; } = new WpfCefSharpXWebViewProvider();

        //public OffScreenCefSharpXWebViewProvider OffScreenProvider { get; } = new OffScreenCefSharpXWebViewProvider();

        [Obsolete("Used in old tests, but not now.")]
        public IXWebView LastResolved { get; private set; }

        [Obsolete("Used in old tests, but not now.")]
        public XWebViewVisibility LastVisibility { get; private set; }

        public async Task<IXWebView> Resolve(XWebViewVisibility prefferedVisibility = XWebViewVisibility.Hidden)
        {
            LastVisibility = prefferedVisibility;
            if (prefferedVisibility == XWebViewVisibility.Hidden && !UseWpfProvider)
            {
                LastResolved = await WpfProvider.Resolve(prefferedVisibility);
                //LastResolved = await OffScreenProvider.Resolve(prefferedVisibility);
            }
            else
            {
                LastResolved = await WpfProvider.Resolve(prefferedVisibility);
            }
            return LastResolved;
        }
    }
}
