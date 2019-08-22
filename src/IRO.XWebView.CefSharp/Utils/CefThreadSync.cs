using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using IRO.XWebView.CefSharp.Utils;
using IRO.XWebView.Core.Exceptions;

namespace IRO.XWebView.CefSharp.Utils
{
    public static class CefThreadSync
    {
        public static void Init(Action<Action> syncInvoker, Action<Action> asyncInvoker)
        {

        }

        /// <summary>
        /// Invoke in specific thread synchronously and return result or throw 
        /// exception to CURRENT thread.
        /// </summary>
        public static TResult Invoke<TResult>(Func<TResult> func)
        {
            return InvokeAsync(func).Result;
        }

        /// <summary>
        /// Invoke in specific tread synchronously and if exception - throw 
        /// exception to CURRENT thread.
        /// </summary>
        public static void Invoke(Action act)
        {
            InvokeAsync(act).Wait();
        }

        /// <summary>
        /// Invoke in specific thread asynchronously and if exception - throw 
        /// exception to AWAITER thread.
        /// </summary>
        public static async Task InvokeAsync(Action act)
        {
            await InvokeAsync<object>(() =>
            {
                act();
                return null;
            });
        }

        /// <summary>
        /// Invoke in specific thread asynchronously and if exception - throw 
        /// exception to AWAITER thread.
        /// </summary>
        public static async Task<TResult> InvokeAsync<TResult>(Func<TResult> func)
        {
            return await InvokeAsync(async () => func());
        }

        /// <summary>
        /// Invoke in specific thread asynchronously and if exception - throw 
        /// exception to AWAITER thread.
        /// </summary>
        public static async Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> func)
        {
            var res = default(TResult);
            Exception origException = null;
            await Cef.UIThreadTaskFactory.StartNew(async () =>
            {
                try
                {
                    res = await func();
                }
                catch (Exception ex)
                {
                    origException = ex;
                }
            }).ConfigureAwait(false);

            if (origException == null)
            {
                return res;
            }
            else
            {
                throw new ThreadSyncException(origException);
            }
        }

        /// <summary>
        /// Invoke in specific thread asynchronously with try/catch.
        /// </summary>
        public static async Task TryInvokeAsync(Action act)
        {
            await InvokeAsync(() =>
            {
                try
                {
                    act();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"ThreadSync error: {ex}");
                }
            });
        }

        /// <summary>
        /// Invoke in specific thread synchronously with try/catch.
        /// </summary>
        public static void TryInvoke(Action act)
        {
            TryInvokeAsync(act).Wait();
        }
    }
}

