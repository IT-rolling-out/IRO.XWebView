using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using IRO.XWebView.Core.Exceptions;

namespace IRO.XWebView.CefSharp.Wpf.Utils
{
    public static class WpfThreadSync
    {
        /// <summary>
        /// Invoke in ui tread synchronously and return result or throw 
        /// exception to CURRENT (not ui) thread.
        /// </summary>
        public static TResult Invoke<TResult>(Func<TResult> func, Dispatcher dispatcher = null)
        {
            var res = default(TResult);
            Exception origException = null;
            dispatcher ??= Application.Current.Dispatcher;
            dispatcher.Invoke(() =>
            {
                try
                {
                    res = func();
                }
                catch (Exception ex)
                {
                    origException = ex;
                }
            });

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
        public static void Invoke(Action act, Dispatcher dispatcher = null)
        {
            Invoke<object>(() =>
            {
                act();
                return null;
            }, dispatcher);
        }

        /// <summary>
        /// Invoke in ui tread asynchronously.
        /// </summary>
        public static void InvokeAsync(Action act, Dispatcher dispatcher = null)
        {
            dispatcher ??= Application.Current.Dispatcher;
            dispatcher.InvokeAsync(act);
        }

        /// <summary>
        /// Invoke in ui tread asynchronously with try/catch.
        /// </summary>
        public static void TryInvokeAsync(Action act, Dispatcher dispatcher = null)
        {
            dispatcher ??= Application.Current.Dispatcher;
            dispatcher.InvokeAsync(() =>
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
        /// Invoke in ui tread synchronously with try/catch.
        /// </summary>
        public static void TryInvoke(Action act, Dispatcher dispatcher = null)
        {
            dispatcher ??= Application.Current.Dispatcher;
            dispatcher.Invoke(() =>
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
    }
}
