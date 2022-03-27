using System.Reflection;
using System.Threading.Tasks;

namespace IRO.XWebView.Core.BindingJs
{
    public interface IBindingJsSystem
    {
        /// <summary>
        /// Default is false. If true - all passed js scripts will be escaped and
        /// executed in 'window.eval' which help to handle syntax exceptions.
        /// <para></para>
        /// If false - script will be directly invoked in webview, without escaping.
        /// Syntax errors can broke your code, but with it you can do things supported by 'unsage-eval'
        /// security flag. Use on your own risk.
        /// </summary>
        bool UnsafeEval { get; set; }

        /// <summary>
        /// Return script used to add support of js2native calls.
        /// <para></para>
        /// Such algorithm used because of limitations of some browser controls (like android WebView)
        /// that do not allow to insert a script before the page is loaded.
        /// </summary>
        string GetAttachBridgeScript();

        /// <summary>
        /// Return script to check if bridge attached. Useful when your browser support csharp2js calls with result from box.
        /// </summary>
        /// <returns></returns>
        string GetIsBridgeAttachedScript();

        /// <summary>
        /// If registered method return Task, it means  promises used.
        /// </summary>
        void OnJsCall(
            IXWebView sender,
            string jsObjectName,
            string functionName,
            string parametersJson,
            string resolveFunctionName,
            string rejectFunctionName
        );

        /// <summary>
        /// Invoked through native js bridge from js.
        /// Norify that promise was finished.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="taskCompletionSourceId"></param>
        /// <param name="executionResult"></param>
        void OnJsPromiseFinished(
            IXWebView sender,
            string taskCompletionSourceId,
            ExecutionResult executionResult
        );

        /// <summary>
        /// Execute js with promise and exception support.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sender"></param>
        /// <param name="script"></param>
        /// <param name="promiseResultSupport">
        /// If true - use callback to resolve value.
        /// Can support promises.
        /// </param>
        /// <param name="timeoutMS"></param>
        /// <returns></returns>
        Task<TResult> ExJs<TResult>(
            IXWebView sender,
            string script,
            bool promiseResultSupport,
            int? timeoutMS
        );

        void BindToJs(
            MethodInfo methodInfo,
            object invokeOn,
            string functionName,
            string jsObjectName
        );

        void UnbindFromJs(
            string functionName,
            string jsObjectName
        );

        void UnbindAllFromJs();
    }
}