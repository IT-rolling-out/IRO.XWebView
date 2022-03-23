using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IRO.XWebView.Core.Utils
{
    public static class DelegatesWithTaskResultInvoker
    {
        /// <summary>
        /// Run sub-delegates from GetInvocationList() one by one and if result is <see cref="Task"/> it will await
        /// and only after this will exit method.
        /// <para></para>
        /// Use this to wait all async events execute one by one.
        /// <para></para>
        /// Null safe for better usage. If <paramref name="del"/> will be null - just return empty list.
        /// </summary>
        /// <param name="del">delegate to invoke</param>
        /// <param name="suppressExceptions">if true, when error in one of sub-delegates - it will be added as result to result list.</param>
        /// <param name="args">Arguments to be passed in delegate</param>
        /// <returns>Results of all sub-delegates.</returns>
        public static async Task<IEnumerable<object>> InvokeOneByOne(this Delegate del, bool suppressExceptions, params object[] args)
        {
            if (del == null)
            {
                return new object[0];
            }

            var delegatesList = del.GetInvocationList();
            var results = new object[delegatesList.Length];
            for (int i = 0; i < delegatesList.Length; i++)
            {
                var subDelegate = delegatesList[i];
                try
                {
                    var subResult = subDelegate.DynamicInvoke(args: args);
                    var subTask = subResult as Task;
                    if (subTask != null)
                    {
                        await subTask;
                    }
                    results[i] = subResult;
                }
                catch (Exception ex)
                {
                    if (!suppressExceptions)
                        throw;
                    results[i] = ex;
                }
            }
            return results;
        }

        /// <summary>
        /// Run sub-delegates from GetInvocationList() parallel and if result is <see cref="Task"/> it will await
        /// and only after this will exit method.
        /// <para></para>
        /// Use this to wait all async events execute in parallel.
        /// <para></para>
        /// Null safe for better usage. If <paramref name="del"/> will be null - just return empty list.
        /// </summary>
        /// <param name="del">delegate to invoke</param>
        /// <param name="suppressExceptions"></param>
        /// <param name="args">Arguments to be passed in delegate</param>
        public static async Task InvokeParallel(this Delegate del, bool suppressExceptions, params object[] args)
        {
            if (del == null)
            {
                return;
            }

            var allTasks = new List<Task>();
            var delegatesList = del.GetInvocationList();

            //Start
            for (int i = 0; i < delegatesList.Length; i++)
            {
                var subDelegate = delegatesList[i];
                try
                {
                    var subResult = subDelegate.DynamicInvoke(args: args);
                    var subTask = subResult as Task;
                    if (subTask != null)
                    {
                        allTasks.Add(subTask);
                    }
                }
                catch
                {
                    if (!suppressExceptions)
                        throw;
                }
            }

            //Wait
            foreach (var subTask in allTasks)
            {
                try
                {
                    await subTask;
                }
                catch
                {
                    if (!suppressExceptions)
                        throw;
                }
            }
        }
    }
}