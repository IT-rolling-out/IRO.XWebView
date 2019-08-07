using System;
using System.Reflection;
using System.Threading.Tasks;
using IRO.ImprovedWebView.Core.EventsAndDelegates;
using IRO.ImprovedWebView.Core.Models;

namespace IRO.ImprovedWebView.Core
{
    public static class ImprovedWebViewExtensions
    {
        /// <summary>
        /// Return null on error.
        /// </summary>
        public static async Task<LoadResult> TryLoadUrl(
            this IImprovedWebView @this,
            string url
        )
        {
            try
            {
                return await @this.LoadUrl(url);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Return null on error.
        /// </summary>
        public static async Task<LoadResult> TryLoadHtml(
            this IImprovedWebView @this,
            string html,
            string baseUrl = "about:blank"
        )
        {
            try
            {
                return await @this.LoadHtml(html, baseUrl);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// You can call passed delegate from js.
        /// All its exceptions will be passed to js.
        /// If delegate return Task - it will be converted to promise.
        /// </summary>
        /// <returns></returns>
        public static void BindToJs(
            this IImprovedWebView @this,
            Delegate delegateObg,
            string functionName,
            string jsObjectName = "Native"
        )
        {
            if (delegateObg == null)
                throw new ArgumentNullException(nameof(delegateObg));
            @this.BindToJs(delegateObg.Method, delegateObg.Target, functionName, jsObjectName);
        }

        /// <summary>
        /// Add all public methods of current object to js.
        /// Add methods signature too.
        /// </summary>
        /// <param name="typeOfObject">If null - will use GetType().</param>
        public static void BindToJs(
            this IImprovedWebView @this,
            object proxyObject,
            string jsObjectName,
            Type typeOfObject = null
        )
        {
            if (proxyObject == null)
                throw new ArgumentNullException(nameof(proxyObject));
            var t = typeOfObject ?? proxyObject.GetType();
            if (!t.IsAssignableFrom(proxyObject.GetType()))
            {
                throw new Exception($"Passed type '{t}' not assignable with js proxyObject of type '{proxyObject.GetType()}'.");
            }
            
            var methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var mi in methods)
            {
                @this.BindToJs(mi, proxyObject, mi.Name, jsObjectName);
            }
        }
    }
}