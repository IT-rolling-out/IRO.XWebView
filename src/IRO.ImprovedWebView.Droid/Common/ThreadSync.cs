using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace IRO.ImprovedWebView.Droid.Common
{
    public static class ThreadSync
    {
        /// <summary>
        /// Invoke in ui tread synchronously and return result or throw 
        /// exception to CURRENT (not ui) thread.
        /// </summary>
        public static TResult Invoke<TResult>(Func<TResult> func)
        {
            TResult res = default(TResult);
            Exception origException = null;
            Application.SynchronizationContext.Send((obj) =>
            {
                try
                {
                    res = func();
                }
                catch (System.Exception ex)
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
                catch (System.Exception ex)
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
            Application.SynchronizationContext.Post((obj) =>
            {
                act();
            }, null);
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
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ImprovedWebView error: {ex}");
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
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ImprovedWebView error: {ex}");
                }
            }, null);
        }


    }
}