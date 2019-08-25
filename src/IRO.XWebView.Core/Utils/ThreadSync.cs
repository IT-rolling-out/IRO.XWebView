using System;
using System.Linq;
using System.Threading.Tasks;
using IRO.XWebView.Core.Exceptions;

namespace IRO.XWebView.Core.Utils
{
    /// <summary>
    /// Class that help execute code on specific thread.
    /// Specially designed to pass all involed delegates results and exceptions
    /// to calling thread and make work with it more simple.
    /// </summary>
    public class ThreadSync
    {
        #region Static part.
        static ThreadSync _inst;

        public static ThreadSync Inst
        {
            get
            {
                if (_inst == null)
                {
                    var invoker = TryAutomaticallyFindInvoker();
                    if (invoker == null)
                    {
                        throw new Exception(
                            $"{nameof(ThreadSync)} is not initialized. " +
                            $"Auto init failed, can't find {nameof(IThreadSyncInvoker)} implementation.\n" +
                            $"You can manually initialize it by {nameof(ThreadSync)}{nameof(Init)}."
                            );
                    }
                    Init(invoker);
                }
                return _inst;
            }
        }

        public static void Init(IThreadSyncInvoker threadSyncInvoker)
        {
            _inst = new ThreadSync(threadSyncInvoker);
        }

        static IThreadSyncInvoker TryAutomaticallyFindInvoker()
        {
            var baseType = typeof(IThreadSyncInvoker);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.SelectMany(s => s.GetTypes())
                .Where(t => baseType.IsAssignableFrom(t) && t.IsPublic)
                .ToList();
            foreach (var t in types)
            {
                try
                {
                    var res = (IThreadSyncInvoker)Activator.CreateInstance(t);
                    return res;
                }
                catch { }
            }
            return null;
        }

        #endregion

        readonly IThreadSyncInvoker invoker;

        public ThreadSync(IThreadSyncInvoker threadSyncInvoker)
        {
            invoker = threadSyncInvoker ?? throw new ArgumentNullException(nameof(threadSyncInvoker));
        }

        /// <summary>
        /// Invoke in ui tread synchronously and return result or throw 
        /// exception to calling (not specific) thread.
        /// </summary>
        public TResult Invoke<TResult>(Func<TResult> func)
        {
            var res = default(TResult);
            Exception origException = null;
            invoker.Invoke(() =>
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
        /// Invoke in ui tread asynchronously.
        /// </summary>
        public async Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> func)
        {
            var tcs = new TaskCompletionSource<TResult>(TaskCreationOptions.RunContinuationsAsynchronously);
            invoker.InvokeAsync(async () =>
            {
                try
                {
                    var res = await func();
                    tcs.SetResult(res);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Invoke in ui tread synchronously and if exception - throw 
        /// exception to WAITER (not ui) thread.
        /// </summary>
        public void Invoke(Action act)
        {
            Invoke<object>(() =>
            {
                act();
                return null;
            });
        }

        /// <summary>
        /// Invoke in specific thread asynchronously and if exception - throw 
        /// exception to AWAITER thread.
        /// </summary>
        public async Task<TResult> InvokeAsync<TResult>(Func<TResult> func)
        {
            return await InvokeAsync(async () => func());
        }

        /// <summary>
        /// Invoke in specific thread asynchronously and if exception - throw 
        /// exception to AWAITER thread.
        /// </summary>
        public async Task InvokeAsync(Action act)
        {
            await InvokeAsync(async () => act());
        }

        /// <summary>
        /// Invoke in ui tread asynchronously with try/catch.
        /// </summary>
        public async Task TryInvokeAsync(Action act)
        {
            try
            {
                await InvokeAsync(act);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Invoke in ui tread synchronously with try/catch.
        /// </summary>
        public void TryInvoke(Action act)
        {
            try
            {
                Invoke(act);
            }
            catch
            {

            }
        }
    }
}
