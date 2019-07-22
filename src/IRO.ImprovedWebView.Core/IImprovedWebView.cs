using System;
using System.Reflection;
using System.Threading.Tasks;
using IRO.ImprovedWebView.Core.EventsAndDelegates;

namespace IRO.ImprovedWebView.Core
{
    /// <summary>
    /// If you implement it, note that all browser crunches must be implemented on native level.
    /// Something like check when browser can't load site or just not respuding, all this cases must
    /// be identified and throw exception.
    /// </summary>
    public interface IImprovedWebView : IDisposable
    {
        string BrowserType { get; }

        string Url { get; }

        /// <summary>
        /// Mean that browser load page or execute js.
        /// </summary>
        bool IsBusy { get; }

        ImprovedWebViewVisibility Visibility { get; set; }

        #region Main.
        Task<LoadFinishedEventArgs> LoadUrl(string url);

        Task<LoadFinishedEventArgs> LoadHtml(string html, string baseUrl = "about:blank");

        /// <summary>
        /// Reload current page.
        /// </summary>
        Task<LoadFinishedEventArgs> Reload();

        Task WaitWhileBusy();

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
        /// Or you can call it from your html page by
        /// 'window.eval(NativeBridge.GetAttachBridgeScript());'.
        /// </summary>
        /// <returns></returns>
        Task AttachBridge();

        void UnbindFromJs(string functionName, string jsObjectName);

        /// <summary>
        /// Js result will be converted by JsonConvert.
        /// <para></para>
        /// Note: Promises will be awaited like <see cref="Task"/>.
        /// <para></para>
        /// It will be executed in js 'window.eval', so you must use 'return'
        /// in your script to get value.
        /// </summary>
        /// <param name="promiseSupport">
        /// If true - use callback to resolve value.
        /// Can support promises.
        /// </param>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="script"></param>
        /// <returns></returns>
        Task<TResult> ExJs<TResult>(string script, bool promiseSupport = false, int? timeoutMS = null);

        /// <summary>
        /// Execute your script in browser without any manipulations.
        /// Doesn't support promises.
        /// </summary>
        Task<string> ExJsDirect(string script, int? timeoutMS = null);

        void Stop();
        #endregion

        bool CanGoForward();

        Task<LoadFinishedEventArgs> GoForward();

        bool CanGoBack();

        Task<LoadFinishedEventArgs> GoBack();

        /// <summary>
        /// Return native WebView.
        /// </summary>
        /// <returns></returns>
        object Native();

        #region Events.
        event GoBackDelegate GoBackRequested;

        event GoForwardDelegate GoForwardRequested;

        event LoadStartedDelegate LoadStarted;

        event LoadFinishedDelegate LoadFinished;

        event Action<object, EventArgs> Disposing;

        event Action<object, EventArgs> Disposed;
        #endregion
    }
}
