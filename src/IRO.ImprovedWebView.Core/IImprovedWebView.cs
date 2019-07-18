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
    public interface IImprovedWebView
    {
        string BrowserType { get; }

        string Url { get; }

        /// <summary>
        /// Mean that browser load page or execute js.
        /// </summary>
        bool IsBusy { get; }

        ImprovedWebViewVisibility Visibility { get; set; }

        #region Main.
        Task LoadUrl(string url);

        Task LoadHtml(string html, string baseUrl="about:blank");

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
        /// Js result will be converted by JsonConvert.
        /// Note: Promises will be awaited like <see cref="Task"/>.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="script"></param>
        /// <returns></returns>
        Task<TResult> ExJs<TResult>(string script);

        /// <summary>
        /// Not js function call.
        /// Just dynamic function call, that must be processed on browser level.
        /// Works like messaging system.
        /// For example, 'InjectJQuery' cmd.
        /// </summary>
        Task<TResult> CallCmd<TResult>(string cmdName, object[] parameters = null);
        
        #endregion
        Task Finish();

        Task<bool> CanGoForward();

        Task GoForward();

        Task<bool> CanGoBack();

        Task GoBack();

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

        event Action<object, EventArgs> Finishing;

        event Action<object, EventArgs> Finished;
        #endregion
    }
}
