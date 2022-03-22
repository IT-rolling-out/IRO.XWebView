using IRO.EmbeddedResources;
using IRO.XWebView.Core;
using System.Reflection;
using System.Threading.Tasks;

namespace IRO.XWebView.Extensions
{
    public static class PolyfillExtensions
    {
        private static string CachedPolyfillStr;

        /// <summary>
        /// Cached on first call.
        /// </summary>
        public static async Task<string> GetPolyfillSource()
        {
            if (CachedPolyfillStr == null)
            {
                var name = $"{typeof(JQueryExtensions)}.EmbeddedFiles.polyfill_min.js";
                CachedPolyfillStr = await EmbeddedResourcesHelpers.ReadEmbeddedResourceText(
                    Assembly.GetExecutingAssembly(),
                    name
                );
            }
            return CachedPolyfillStr;
        }

        /// <summary>
        /// Used pollyfill from
        /// https://polyfill.io/v3/polyfill.min.js?flags=gated%2Calways&features=blissfuljs
        /// <para></para>
        /// More info on https://blissfuljs.com/
        /// <para></para>
        /// Each feature will be added only if it not implemented in browser.
        /// <para></para>
        /// NOTE: Polyfill can be injected without awaiting, because IXWebView callbacks
        /// can't work without Promises (if browser not support them), that added by pollyfill.
        /// </summary>
        public static async Task IncludePolyfill(this IXWebView xwv)
        {
            var script = await GetPolyfillSource();
            try
            {
                //Try await while invoked.
                await xwv.UnmanagedExecuteJavascriptWithResult(script);
            }
            catch
            {
                xwv.UnmanagedExecuteJavascriptAsync(script);
            }
        }
    }
}