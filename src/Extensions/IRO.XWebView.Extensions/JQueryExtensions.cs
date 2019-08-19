using System;
using System.Reflection;
using System.Threading.Tasks;
using IRO.EmbeddedResources;
using IRO.XWebView.Core;

namespace IRO.XWebView.Extensions
{
    public static class JQueryExtensions
    {
        static string CachedSourceStr;

        /// <summary>
        /// Cached on first call.
        /// </summary>
        public static async Task<string> GetJQuerySource()
        {
            if (CachedSourceStr == null)
            {
                var name = "IRO.XWebView.Extensions.EmbeddedFiles.jquery_min.js";
                CachedSourceStr = await EmbeddedResourcesHelpers.ReadEmbeddedResourceText(
                    Assembly.GetExecutingAssembly(),
                    name
                    );
            }
            return CachedSourceStr;
        }

        /// <summary>
        /// Execute script to include jquery on page.
        /// </summary>
        /// <returns></returns>
        public static async Task IncludeJQuery(this IXWebView xwv)
        {
            var script=await GetJQuerySource();
            script += "\n\n\n;\nwindow.jQuery=jQuery;\nwindow.$=jQuery;";
            await xwv.UnmanagedExecuteJavascriptWithResult(script);
            if (!await IsJQueryIncluded(xwv))
            {
                throw new Exception("Include JQuery task finished, but it not included.");
            }
        }

        /// <summary>
        /// Check if jquery included on page.
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> IsJQueryIncluded(this IXWebView xwv)
        {
            var script = @"
(function(){
if(window.jQuery){
  return true;
}
return false;
})();
";
            var scriptRes = await xwv.UnmanagedExecuteJavascriptWithResult(script);
            return scriptRes =="true";
        }

        /// <summary>
        /// Check if jquery included on page and include it if not.
        /// </summary>
        /// <returns>True if was included before.</returns>
        public static async Task<bool> IncludeJQueryIfNotIncluded(this IXWebView xwv)
        {
            var isIncluded=await IsJQueryIncluded(xwv);
            if (!isIncluded)
            {
                await IncludeJQuery(xwv);
            }
            return isIncluded;
        }
       
    }
}
