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
