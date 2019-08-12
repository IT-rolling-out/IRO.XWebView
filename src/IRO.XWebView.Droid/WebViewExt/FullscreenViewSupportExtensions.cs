using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using IRO.XWebView.Droid.Renderer;

namespace IRO.XWebView.Droid
{
    public static class FullscreenViewSupportExtensions
    {
        /// <summary>
        /// <see cref="WebViewRenderer"/> use it by default.
        /// </summary>
        /// <param name="wv"></param>
        /// <param name="fullscreenContainer"></param>
        /// <returns></returns>
        public static FullscreenViewSupport EnableFullscreenViewSupport(this WebView wv, ViewGroup fullscreenContainer)
        {
            return new FullscreenViewSupport(wv, fullscreenContainer);
        }

    }
}