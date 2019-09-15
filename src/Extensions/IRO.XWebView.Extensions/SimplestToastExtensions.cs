using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IRO.EmbeddedResources;
using IRO.XWebView.Core;
using Newtonsoft.Json;

namespace IRO.XWebView.Extensions
{
    public static class SimplestToastExtensions
    {
        static string CachedSourceStr;

        /// <summary>
        /// Cached on first call.
        /// </summary>
        public static async Task<string> GetSimplestToastSource()
        {
            if (CachedSourceStr == null)
            {
                var name = "IRO.XWebView.Extensions.EmbeddedFiles.simplest_toast.js";
                CachedSourceStr = await EmbeddedResourcesHelpers.ReadEmbeddedResourceText(
                    Assembly.GetExecutingAssembly(),
                    name
                );
            }
            return CachedSourceStr;
        }

        public static async Task ShowToast(this IXWebView xwv, string msg, int timeoutMS=3000)
        {
            try
            {
                var source = await GetSimplestToastSource();
                await xwv.ExJs<object>(source);
                var msgJson = JsonConvert.SerializeObject(msg);
                var script = $@"
SimplestToast.Show({msgJson}, {timeoutMS});
";
                await xwv.ExJs<object>(script);
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"ShowToast exception '{ex}'.");
            }
        }
    }
}
