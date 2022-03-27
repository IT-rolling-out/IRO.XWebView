using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using IRO.Threading;
using IRO.XWebView.Core.BindingJs;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Events;
using IRO.XWebView.Core.Models;

namespace IRO.XWebView.Core
{
    /// <summary>
    /// If you implement it, note that all browser crunches must be implemented on native level.
    /// Something like check when browser can't load site or just not respuding, all this cases must
    /// be identified and throw exception.
    /// </summary>
    public interface IXWebView : IDisposable
    {
        /// <summary>
        /// Random id.
        /// </summary>
        int Id { get; }

        string BrowserName { get; }

        string Url { get; }

        /// <summary>
        /// Mean that browser load page.
        /// </summary>
        bool IsNavigating { get; }

        /// <summary>
        /// False by default. If true - will add your native methods support on all pages after they uploaded.
        /// </summary>
        bool AutomaticallyAttachBridge { get; set; }

        /// <summary>
        /// Default is false. If true - all passed js scripts will be escaped and
        /// executed in 'window.eval' which help to handle syntax exceptions.
        /// <para></para>
        /// If false - script will be directly invoked in webview, without escaping.
        /// Syntax errors can broke your code, but with it you can do things supported by 'unsafe-eval'
        /// security flag. Use on your own risk.
        /// </summary>
        bool UnsafeEval { get; set; }

        /// <summary>
        /// If true - you can change visibility after creation.
        /// If false - <see cref="Visibility"/> assignment will throw exception.
        /// </summary>
        bool CanSetVisibility { get; }

        XWebViewVisibility Visibility { get; set; }

        /// <summary>
        /// Use it for extensions.
        /// </summary>
        IDictionary<string, object> Data { get; }

        /// <summary>
        /// Synchroniztion context. Used to invoke calls to native webview control thread.
        /// </summary>
        ThreadSyncContext ThreadSync { get; }

        /// <summary>
        /// Usually you get fully initialized xwv from providers.
        /// This property usefull with some browser engines when you develop your own controls.
        /// </summary>
        bool IsInitialized { get; }

        event EventHandler Initialized;

        bool CanGoForward();

        Task<LoadResult> GoForward();

        bool CanGoBack();

        Task<LoadResult> GoBack();

        Task<string> GetHtml();

        /// <summary>
        /// Return native WebView.
        /// </summary>
        /// <returns></returns>
        object Native();

        #region Main.

        Task<LoadResult> LoadUrl(string url);

        Task<LoadResult> LoadHtml(string html, string baseUrl = "about:blank");

        /// <summary>
        /// Reload current page.
        /// </summary>
        Task<LoadResult> Reload();

        Task WaitWhileNavigating();

        /// <summary>
        /// You can call passed method from js.
        /// All its exceptions will be passed to js.
        /// If method return Task - it will be converted to promise.
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="invokeOn">
        /// Instance of object for which the method will be invoked.
        /// Can be null for static.
        /// </param>
        /// <param name="functionName"></param>
        /// <param name="jsObjectName"></param>
        void BindToJs(MethodInfo methodInfo, object invokeOn, string functionName, string jsObjectName);

        /// <summary>
        /// Execute script to init native methods support.
        /// Or you can use <see cref="AutomaticallyAttachBridge"/>.
        /// </summary>
        /// <returns></returns>
        Task AttachBridge();

        void UnbindFromJs(string functionName, string jsObjectName);

        void UnbindAllFromJs();

        /// <summary>
        /// Js result will be converted by JsonConvert.
        /// <para></para>
        /// Note: Promises will be awaited like <see cref="Task"/>.
        /// <para></para>
        /// It will be executed in js 'window.eval', so you must use 'return'
        /// in your script to get value.
        /// </summary>
        /// <param name="promiseResultSupport">
        /// If true - use callback to resolve value.
        /// Can support promises.
        /// </param>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="script"></param>
        /// <returns></returns>
        Task<TResult> ExJs<TResult>(string script, bool promiseResultSupport = true, int? timeoutMS = null);

        /// <summary>
        /// Execute your script in browser without any manipulations.
        /// Doesn't support promises.
        /// <para></para>
        /// Used as base method to build javascript wrapper for sync scripts.
        /// <para></para>
        /// It is recommended to use this method only when absolutely necessary
        /// (better only in <see cref="IXWebView"/> implementions) .
        /// Better to use <see cref="ExJs{TResult}(string, bool, int?)"/>.
        /// </summary>
        Task<string> UnmanagedExecuteJavascriptWithResult(string script, int? timeoutMS = null);

        /// <summary>
        /// Execute your script in browser without any manipulations.
        /// Doesn't support promises.
        /// <para></para>
        /// Used as base method to build javascript wrapper for async scripts (callbacks).
        /// <para></para>
        /// It is recommended to use this method only when absolutely necessary
        /// (better only in <see cref="IXWebView"/> implementions) .
        /// Better to use <see cref="ExJs{TResult}(string, bool, int?)"/>.
        /// </summary>
        void UnmanagedExecuteJavascriptAsync(string script, int? timeoutMS = null);

        void Stop();

        void ClearCookies();
        #endregion

        #region Events.

        event GoBackDelegate GoBackRequested;

        event GoForwardDelegate GoForwardRequested;

        event LoadStartedDelegate LoadStarted;

        event LoadFinishedDelegate LoadFinished;

        #endregion

        #region Disposing.

        /// <summary>
        /// Set to true when start disposing.
        /// </summary>
        bool IsDisposed { get; }

        event Action<object, EventArgs> Disposing;

        #endregion
    }
}