using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IRO.XWebView.Core;

namespace IRO.XWebView.Extensions
{
    public static class CommonExtensions
    {
        /// <summary>
        /// Add meta page to line, that will disable it's security and allow unsafe-eval.
        ///<para></para>
        /// More info here https://stackoverflow.com/questions/38277526/webview-content-security-policy .
        /// </summary>
        /// <returns></returns>
        public static async Task DisablePageSecurity(this IXWebView xwv)
        {
            var script = @"
var meta = document.createElement('meta');
meta.httpEquiv = 'Content-Security-Policy';
meta.content = ""default-src * gap:; script-src * 'unsafe-inline' 'unsafe-eval'; connect-src *; img-src * data: blob: android-webview-video-poster:; style-src * 'unsafe-inline';"";
document.getElementsByTagName('head')[0].appendChild(meta);
";
            await xwv.UnmanagedExecuteJavascriptWithResult(script);

        }
    }
}
