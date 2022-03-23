using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IRO.Threading;
using IRO.XWebView.Core.Exceptions;

namespace IRO.XWebView.Core.Utils
{
    public static class EventsWithTaskResultInvoker
    {

    }

    /// <summary>
    /// Class that help execute code on specific thread.
    /// Specially designed to pass all involed delegates results and exceptions
    /// to calling thread and make work with it more simple.
    /// </summary>
    public class XWebViewThreadSync
    {
        static ThreadSyncContext _inst;

        /// <summary>
        /// If you develop project based on IRO.XWebView (not new implemention of IXWebView) -
        /// it's strongly recommended to use <see cref="IXWebView.ThreadSync"/> instead of this singleton.
        /// </summary>
        public static ThreadSyncContext Inst
        {
            get
            {
                if (_inst == null)
                {
                    var invoker = TryAutomaticallyFindInvoker();
                    if (invoker == null)
                    {
                        throw new Exception(
                            $"{nameof(XWebViewThreadSync)} is not initialized. " +
                            $"Auto init failed, can't find {nameof(IThreadSyncInvoker)} implementation.\n" +
                            $"You can manually initialize it by {nameof(XWebViewThreadSync)}{nameof(Init)}."
                            );
                    }
                    Init(invoker);
                }
                return _inst;
            }
        }

        public static void Init(IThreadSyncInvoker threadSyncInvoker)
        {
            if (_inst != null)
            {
                throw new Exception($"{nameof(XWebViewThreadSync)} was initialized before.");
            }
            _inst = new ThreadSyncContext(threadSyncInvoker);
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

    }
}
