using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace IRO.ImprovedWebView.Core.BindingJs
{
    public interface IBindingJsSystem
    {
        /// <summary>
        /// Return script used to add support of js2native calls.
        /// <para></para>
        /// Such algorithm used because of limitations of some browser controls (like android WebView)
        /// that do not allow to insert a script before the page is loaded.
        /// </summary>
        string GetAttachBridgeScript();

        /// <summary>
        /// If registered method return Task, it means  promises used.
        /// </summary>
        void OnJsCallNativeAsync(
            IImprovedWebView sender,
            string jsObjectName,
            string functionName,
            string parametersJson,
            string resolveFunctionName,
            string rejectFunctionName
            );

        /// <summary>
        /// If registered method not return task (synchronous).
        /// Just return serialized json to javascript part and it make all work.
        /// </summary>
        ExecutionResult OnJsCallNativeSync(
            IImprovedWebView sender,
            string jsObjectName,
            string functionName,
            string parametersJson
            );

        /// <summary>
        /// Invoked through native js bridge from js.
        /// Norify that promise was finished.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="taskCompletionSourceId"></param>
        /// <param name="executionResult"></param>
        void OnJsPromiseFinished(
            IImprovedWebView sender,
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
            IImprovedWebView sender,
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
    }
}