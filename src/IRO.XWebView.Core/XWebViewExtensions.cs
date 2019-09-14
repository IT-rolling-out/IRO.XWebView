using System;
using System.Reflection;
using System.Threading.Tasks;
using IRO.XWebView.Core.Models;

namespace IRO.XWebView.Core
{
    public static class XWebViewExtensions
    {
        /// <summary>
        /// Wait for initialization if not init.
        /// </summary>
        public static async Task WaitInitialization(
            this IXWebView xwv
        )
        {
            if (xwv.IsInitialized)
                return;
            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            EventHandler ev = null;
            ev= delegate
            {
                xwv.Initialized -= ev;
                tcs.SetResult(null);
            };
            xwv.Initialized += ev;
            if (!xwv.IsInitialized)
            {
                await tcs.Task;
            }
        }

        /// <summary>
        /// Return null on error.
        /// </summary>
        public static async Task<LoadResult> TryLoadUrl(
            this IXWebView @this,
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
            this IXWebView @this,
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

        public static async Task<LoadResult> TryReload(
            this IXWebView @this
        )
        {
            try
            {
                return await @this.Reload();
            }
            catch
            {
                return null;
            }
        }

        public static async Task<LoadResult> TryGoForward(
            this IXWebView @this
        )
        {
            try
            {
                return await @this.GoForward();
            }
            catch
            {
                return null;
            }
        }

        public static async Task<LoadResult> TryGoBack(
            this IXWebView @this
        )
        {
            try
            {
                return await @this.GoBack();
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
            this IXWebView @this,
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
            this IXWebView @this,
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
                throw new Exception(
                    $"Passed type '{t}' not assignable with js proxyObject of type '{proxyObject.GetType()}'.");
            }

            var methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var mi in methods)
            {
                @this.BindToJs(mi, proxyObject, mi.Name, jsObjectName);
            }
        }
    }
}