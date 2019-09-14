using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IRO.XWebView.Core;
using Newtonsoft.Json;

namespace IRO.XWebView.Extensions
{
    public static class CommonExtensions
    {
        public static async Task<string> GetHtml(this IXWebView xwv)
        {
            var script = @"
return document.documentElement.outerHTML;
";
            return await xwv.ExJs<string>(script);
        }

        /// <summary>
        /// Set zoom level on current page via js.
        /// </summary>
        public static void SetZoomLevel(this IXWebView xwv, double value)
        {
            var serializedValue = JsonConvert.SerializeObject(value);
            var script = $@"
(function(){{
  document.body.style.zoom = '{serializedValue}';
}})();
";
            xwv.UnmanagedExecuteJavascriptAsync(script);
        }

        
    }
}
