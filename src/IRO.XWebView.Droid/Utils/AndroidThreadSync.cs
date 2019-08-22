using System;
using System.Diagnostics;
using Android.App;
using IRO.XWebView.Core.Exceptions;
using IRO.XWebView.Droid.Utils;

namespace IRO.XWebView.Droid.Utils
{
    public static class AndroidThreadSync
    {
        /// <summary>
        /// Invoke in ui tread synchronously and return result or throw 
        /// exception to CURRENT (not ui) thread.
        /// </summary>
        public static TResult Invoke<TResult>(Func<TResult> func)
        {
            var res = default(TResult);
            Exception origException = null;
            Application.SynchronizationContext.Send((obj) =>
            {
                try
                {
                    res = func();
                }
                catch (Exception ex)
                {
                    origException = ex;
                }
            }, null);

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
        /// Invoke in ui tread synchronously and if exception - throw 
        /// exception to CURRENT (not ui) thread.
        /// </summary>
        public static void Invoke(Action act)
        {
            Exception origException = null;
            Application.SynchronizationContext.Send((obj) =>
            {
                try
                {
                    act();
                }
                catch (Exception ex)
                {
                    origException = ex;
                }
            }, null);

            if (origException != null)
            {
                throw new ThreadSyncException(origException);
            }
        }

        /// <summary>
        /// Invoke in ui tread asynchronously.
        /// </summary>
        public static void InvokeAsync(Action act)
        {
            Application.SynchronizationContext.Post((obj) => { act(); }, null);
        }

        /// <summary>
        /// Invoke in ui tread asynchronously with try/catch.
        /// </summary>
        public static void TryInvokeAsync(Action act)
        {
            Application.SynchronizationContext.Post((obj) =>
            {
                try
                {
                    act();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"ThreadSync error: {ex}");
                }
            }, null);
        }

        /// <summary>
        /// Invoke in ui tread synchronously with try/catch.
        /// </summary>
        public static void TryInvoke(Action act)
        {
            Application.SynchronizationContext.Send((obj) =>
            {
                try
                {
                    act();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"ThreadSync error: {ex}");
                }
            }, null);
        }
    }
}