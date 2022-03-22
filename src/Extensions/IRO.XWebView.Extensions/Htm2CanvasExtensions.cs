using IRO.EmbeddedResources;
using IRO.XWebView.Core;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace IRO.XWebView.Extensions
{
    public static class Htm2CanvasExtensions
    {
        private static string CachedSourceStr;

        /// <summary>
        /// Cached on first call.
        /// </summary>
        public static async Task<string> GetHtml2CanvasSource()
        {
            if (CachedSourceStr == null)
            {
                var name = $"{typeof(JQueryExtensions)}.EmbeddedFiles.html2canvas_min.js";
                CachedSourceStr = await EmbeddedResourcesHelpers.ReadEmbeddedResourceText(
                    Assembly.GetExecutingAssembly(),
                    name
                );
            }
            return CachedSourceStr;
        }

        /// <summary>
        /// Use html2canvas.js to make screenshot and return base64 string image.
        /// <para></para>
        /// Unsafe eval must be enabled.
        /// </summary>
        public static async Task<string> ScreenshotViaJs(this IXWebView xwv, string jquerySelector = "body")
        {
            await xwv.IncludeJQueryIfNotIncluded();
            await IncludeHtml2CanvasIfNotIncluded(xwv);

            var screenshotScript = @"
var promise = new Promise(function(resolve, reject) {
  html2canvas($('body')[0]).then(
    function(canvas){
      var data = canvas.toDataURL();
      resolve(data);   
    },
    function(error){ reject(error); }
  );
});
return promise;
";
            //Disable security to use unsafe-eval. Better use only in debug.
            var prevUnsafeEvalValue = xwv.UnsafeEval;
            xwv.UnsafeEval = true;
            try
            {
                var res = await xwv.ExJs<string>(screenshotScript, true);
                return res;
            }
            finally
            {
                xwv.UnsafeEval = prevUnsafeEvalValue;
            }
        }


        /// <summary>
        /// Execute script to include html2canvas on page.
        /// </summary>
        /// <returns></returns>
        public static async Task IncludeHtml2Canvas(this IXWebView xwv)
        {
            var script = await GetHtml2CanvasSource();
            script += "\n\n\n;\nwindow.html2canvas=html2canvas;";
            await xwv.UnmanagedExecuteJavascriptWithResult(script);
            if (!await IsHtml2CanvasIncluded(xwv))
            {
                throw new Exception("Include Html2Canvas task finished, but it not included.");
            }
        }

        /// <summary>
        /// Check if html2canvas included on page.
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> IsHtml2CanvasIncluded(this IXWebView xwv)
        {
            var script = @"
(function(){
if(window.html2canvas){
  return true;
}
return false;
})();
";
            var scriptRes = await xwv.UnmanagedExecuteJavascriptWithResult(script);
            return scriptRes == "true";
        }

        /// <summary>
        /// Check if html2canvas included on page and include it if not.
        /// </summary>
        /// <returns>True if was included before.</returns>
        public static async Task<bool> IncludeHtml2CanvasIfNotIncluded(this IXWebView xwv)
        {
            var isIncluded = await IsHtml2CanvasIncluded(xwv);
            if (!isIncluded)
            {
                await xwv.IncludeHtml2Canvas();
            }
            return isIncluded;
        }

        public static Image Base64StringToBitmap(string base64String)
        {
            base64String = FixBase64ForImage(base64String);
            byte[] byteBuffer = Convert.FromBase64String(base64String);
            MemoryStream memoryStream = new MemoryStream(byteBuffer)
            {
                Position = 0
            };
            var res = Image.FromStream(memoryStream);
            memoryStream.Close();
            memoryStream = null;
            byteBuffer = null;
            return res;
        }

        private static string FixBase64ForImage(string str)
        {
            const string keyword = "base64,";
            var index = str.IndexOf(keyword);
            if (index > 0)
            {
                str = str.Substring(index + keyword.Length);
            }
            return FixBase64ForImage2(str);
        }

        private static string FixBase64ForImage2(string Image)
        {
            System.Text.StringBuilder sbText = new System.Text.StringBuilder(Image, Image.Length);
            sbText.Replace("\r\n", String.Empty); sbText.Replace(" ", String.Empty);
            return sbText.ToString();
        }
    }
}