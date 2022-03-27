using System;
using System.Threading.Tasks;
using Android.App;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;
using IRO.XWebView.Droid.OnFinestWebView.Providers;
using IRO.XWebView.Droid.Providers;
#pragma warning disable 618

namespace IRO.Tests.XWebView.DroidApp
{
    public class TestXWebViewProvider : BaseXWebViewProvider
    {
        public static bool UseFinestWebView { get; set; } = true;

        public NewActivityXWebViewProvider NewActivityProvider { get; } = new NewActivityXWebViewProvider();

        public FinestXWebViewProvider FinestProvider { get; } = new FinestXWebViewProvider(Application.Context);

        [Obsolete("Used in old tests, but not now.")]
        public IXWebView LastResolved { get; private set; }

        [Obsolete("Used in old tests, but not now.")]
        public XWebViewVisibility LastVisibility { get; private set; }

        protected override async Task<IXWebView> ProtectedResolve(XWebViewVisibility preferredVisibility)
        {
            LastVisibility = preferredVisibility;
            if (preferredVisibility == XWebViewVisibility.Hidden || !UseFinestWebView)
            {
                LastResolved = await NewActivityProvider.Resolve(preferredVisibility);
            }
            else
            {
                LastResolved = await FinestProvider.Resolve(preferredVisibility);
            }
            return LastResolved;
        }
    }
}