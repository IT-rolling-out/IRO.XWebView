using System.Reflection;
using Newtonsoft.Json.Linq;

namespace IRO.ImprovedWebView.Core.BindingJs
{
    public interface IBindingJsSystem
    {
        /// <summary>
        /// Return script used to add support of js2native calls.
        /// </summary>
        /// <returns></returns>
        string GeneratePageInitializationJs();

        /// <summary>
        /// If registered method return Task, it means  promises used.
        /// </summary>
        void OnJsCallAsync(
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
        string OnJsCallSync(
            IImprovedWebView sender,
            string jsObjectName,
            string functionName,
            string parametersJson
            );

        void BindToJs(
            MethodInfo methodInfo,
            object invokeOn, 
            string functionName, 
            string jsObjectName
            );
    }


}