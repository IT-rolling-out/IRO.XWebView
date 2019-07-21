using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IRO.ImprovedWebView.Core.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IRO.ImprovedWebView.Core.BindingJs
{
    public class BindingJsSystem : IBindingJsSystem
    {
        public BindingJsSystemSettings Settings { get; }

        #region js2csharp
        const string FuncName_Async = "sa";

        const string FuncName_Sync = "ss";

        string _pageInitializationJs_Cached;

        bool _pageInitializationJs_CacheUpdated;

        /// <summary>
        /// Key is 'jsObjectName.actionName'.
        /// </summary>
        readonly IDictionary<string, BindedMethodData> _methods = new Dictionary<string, BindedMethodData>();

        /// <summary>
        /// If registered method return Task, it means  promises used.
        /// </summary>
        public void OnJsCallNativeAsync(
            IImprovedWebView sender,
            string jsObjectName,
            string functionName,
            string parametersJson,
            string resolveFunctionName,
            string rejectFunctionName
            )
        {
            var parameters = JToken.Parse(parametersJson);
            var key = jsObjectName + "." + functionName;
            var bindedMethodData = _methods[key];
            var paramsArr = JsonToParams(bindedMethodData.Parameters, parameters);

            Task.Run(async () =>
            {
                try
                {
                    object methodRes = bindedMethodData.Method.Invoke(bindedMethodData.InvokeOn, paramsArr);
                    var methodRealRes = new object();
                    if (methodRes is Task task)
                    {
                        await task;
                        if (task.IsFaulted || task.IsCanceled)
                        {
                            //!Reject
                            var ex = task.Exception;
                            await RejectPromise(sender, resolveFunctionName, ex);
                        }
                        else
                        {
                            var prop = task.GetType().GetProperty("Result");
                            methodRealRes = prop?.GetValue(task);
                        }
                    }
                    else
                    {
                        methodRealRes = methodRes;
                    }
                    //!Resolve
                    await ResolvePromise(sender, resolveFunctionName, methodRealRes);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"ImprovedWebView error: {ex}");
                    //!Reject
                    await RejectPromise(sender, resolveFunctionName, ex);
                }
            });
        }

        public ExecutionResult OnJsCallNativeSync(IImprovedWebView sender, string jsObjectName, string functionName, string parametersJson)
        {
            var parameters = JToken.Parse(parametersJson);
            var key = jsObjectName + "." + functionName;
            var bindedMethodData = _methods[key];
            var paramsArr = JsonToParams(bindedMethodData.Parameters, parameters);
            ExecutionResult jsSyncResult;
            try
            {
                object methodRes = bindedMethodData.Method.Invoke(bindedMethodData.InvokeOn, paramsArr);
                jsSyncResult = new ExecutionResult()
                {
                    Result = JToken.FromObject(methodRes),
                    IsError = false
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ImprovedWebView error: {ex}");
                jsSyncResult = new ExecutionResult()
                {
                    Result = ex.ToString(),
                    IsError = true
                };
            }
            return jsSyncResult;
        }

        public void BindToJs(MethodInfo methodInfo, object invokeOn, string functionName, string jsObjectName)
        {
            if (methodInfo == null) throw new ArgumentNullException(nameof(methodInfo));
            if (functionName == null) throw new ArgumentNullException(nameof(functionName));
            if (jsObjectName == null) throw new ArgumentNullException(nameof(jsObjectName));
            jsObjectName = jsObjectName.Trim();

            var bindedMethodData = new BindedMethodData()
            {
                JsObjectName = jsObjectName,
                FunctionName = functionName,
                Method = methodInfo,
                InvokeOn = invokeOn,
                Parameters = methodInfo.GetParameters()
            };
            var key = jsObjectName + "." + functionName;
            _methods[key] = bindedMethodData;
            //Force update page init script.
            _pageInitializationJs_CacheUpdated = false;
        }

        /// <summary>
        /// Return script used to add support of js2native calls.
        /// </summary>
        /// <returns></returns>
        public string GeneratePageInitializationJs()
        {
            if (_pageInitializationJs_CacheUpdated)
                return _pageInitializationJs_Cached;

            string initNativeBridgeScript_Start = @"
function InitNativeBridge() {
    var w = window;
    if (w['NativeBridgeInitStarted'])
        return;
    w['NativeBridgeInitStarted'] = true;

    //Js wrap to handle promises and exceptions.
    var ac = function AsyncCall(jsObjectName, functionName, callArguments) {
        var num = Math.floor(Math.random() * 10001);
        var resolveFunctionName = 'randomFunc_Resolve_' + num;
        var rejectFunctionName = 'randomFunc_Reject_' + num;
        var resPromise = new window.Promise(function (rs, rj) {
            w[resolveFunctionName] = rs;
            w[rejectFunctionName] = rj;
        });
        var callArgumentsArr = Array.prototype.slice.call(callArguments);
        var parametersJson = JSON.stringify(callArgumentsArr);
        " + Settings.OnJsCallNativeAsyncFunctionName + @"(jsObjectName, functionName, parametersJson, resolveFunctionName, rejectFunctionName);
        return resPromise;
    };
    var sc = function " + Settings.OnJsCallNativeSyncFunctionName + @"(jsObjectName, functionName, callArguments) {
        var callArgumentsArr = Array.prototype.slice.call(callArguments);
        var parametersJson = JSON.stringify(callArgumentsArr);
        var nativeMethodResJson = OnJsCallSync(jsObjectName, functionName, parametersJson);
        var nativeMethodRes = JSON.parse(nativeMethodResJson);
        if (nativeMethodRes.isError) {
            throw nativeMethodRes.result;
        } else {
            return nativeMethodRes.result;
        }
    };

    //Registration helpers.
    var ram = function RegisterAsyncMethod(objName, functionName) {
        w[objName] = w[objName] || {};
        w[objName][functionName] = function () { return ac(objName, functionName, arguments); };
    };
    var rsm = function RegisterAsyncMethod(objName, functionName) {
        w[objName] = w[objName] || {};
        w[objName][functionName] = function () { return sc(objName, functionName, arguments); };
    };
";
            const string initNativeBridgeScript_End = @"
}
InitNativeBridge();
";

            var methodsRegistrationScript = GenerateMethodsRegistrationScript();
            var script = initNativeBridgeScript_Start + methodsRegistrationScript + initNativeBridgeScript_End;
            _pageInitializationJs_Cached = script;
            _pageInitializationJs_CacheUpdated = true;
            return script;
        }

        object[] JsonToParams(ParameterInfo[] parameters, JToken jTokens)
        {
            var res = new object[parameters.Length];
            int i = 0;
            foreach (var param in parameters)
            {
                i++;
                var jToken = jTokens[i];
                if (jToken == null)
                {
                    res[i] = DefaultOf(param.ParameterType);
                }
                else
                {
                    var deserializedParameter = jToken.ToObject(param.ParameterType);
                    res[i] = deserializedParameter;
                }
            }
            return res;
        }

        object DefaultOf(Type t)
        {
            if (t.IsValueType)
            {
                return Activator.CreateInstance(t);
            }
            return null;
        }

        async Task RejectPromise(IImprovedWebView sender, string rejectFunctionName, Exception ex)
        {
            try
            {
                var serializedEx = JsonConvert.SerializeObject(ex.Message.ToString());
                await sender.ExJsDirect($"{rejectFunctionName}({serializedEx});");
            }
            catch(Exception newEx)
            {
                Debug.WriteLine($"ImprovedWebView error: {newEx}");
                //Ignore exceptions. It can be rised, for example, when we load new page.
            }
        }

        async Task ResolvePromise(IImprovedWebView sender, string resolveFunctionName, object res)
        {
            try
            {
                var serializedRes = JsonConvert.SerializeObject(res);
                await sender.ExJsDirect($"{resolveFunctionName}({serializedRes});");
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"ImprovedWebView error: {ex}");
                //Ignore exceptions. It can be rised, for example, when we load new page.
            }
        }

        string GenerateMethodsRegistrationScript()
        {
            var sb = new StringBuilder();
            foreach (var item in _methods)
            {
                var data = item.Value;
                var jsObjectName = JsonConvert.SerializeObject(data.JsObjectName);
                var functionName = JsonConvert.SerializeObject(data.FunctionName);
                var isAsync = typeof(Task).IsAssignableFrom(data.Method.ReturnType);
                var registerFunctionName = isAsync ? "ram" : "rsm";
                var line = $"{registerFunctionName}({jsObjectName}, {functionName});\n";
                sb.Append(line);
            }
            return sb.ToString();
        }
        #endregion


        #region csharp2js
        readonly IDictionary<string, TaskCompletionSource<JToken>> _pendingPromisesCallbacks =
            new Dictionary<string, TaskCompletionSource<JToken>>();

        readonly Random _random = new Random();

        public BindingJsSystem(BindingJsSystemSettings settings)
        {
            Settings = settings;
        }

        public void OnJsPromiseFinished(IImprovedWebView sender, string taskCompletionSourceId, ExecutionResult executionResult)
        {
            if (!_pendingPromisesCallbacks.TryGetValue(taskCompletionSourceId, out var tcs))
                return;
            try
            {
                _pendingPromisesCallbacks.Remove(taskCompletionSourceId);
                if (executionResult.IsError)
                {
                    tcs.TrySetException(new ImprovedWebViewException(executionResult.Result.ToString()));
                }
                else
                {
                    tcs.TrySetResult(executionResult.Result);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ImprovedWebView error: {ex}");
                tcs.TrySetException(new ImprovedWebViewException("", ex));
            }
        }

        public async Task<TResult> ExJs<TResult>(IImprovedWebView sender, string script, bool promiseSupport, int? timeoutMS)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            if (script == null) throw new ArgumentNullException(nameof(script));

            Task<JToken> task;
            if (promiseSupport)
            {
                task = ExJs_PromisesSupported(sender, script, timeoutMS);
            }
            else
            {
                task = ExJs_PromisesNotSupported(sender, script, timeoutMS);
            }

            //Wait with timeout. Needed, because browser timeout doesn't work for callbacks.
            if (timeoutMS != null)
            {
                await Task.WhenAny(
                    task,
                    Task.Delay(timeoutMS.Value)
                );
                if (!task.IsCompleted)
                {
                    Debug.WriteLine($"ImprovedWebView error: 'Js evaluation timeout'");
                    throw new ImprovedWebViewException($"Js evaluation timeout {timeoutMS}");
                }
            }
            var res = await task;
            if (res == null)
            {
                return default(TResult);
            }
            return res.ToObject<TResult>();
        }

        async Task<JToken> ExJs_PromisesSupported(IImprovedWebView sender, string script, int? timeoutMS)
        {
            var taskId = _random.Next(10000, 99999).ToString();
            var tcs = new TaskCompletionSource<JToken>(TaskCreationOptions.RunContinuationsAsynchronously);
            _pendingPromisesCallbacks[taskId] = tcs;

            var scriptSerialized = JsonConvert.SerializeObject(script);
            var taskIdSerialized = JsonConvert.SerializeObject(taskId);
            var allScript = @"
var Native = {};
Native.OnJsPromiseFinished = function () {
    var callArgumentsArr = Array.prototype.slice.call(arguments);
    console.log(callArgumentsArr);
};

(function () {
    var script = " + scriptSerialized + @";
    var numId = " + taskIdSerialized + @";
    try {
        var evalRes = eval('(async () => {' + script + '})()');
        evalRes.then(
            function (value) {
                " + Settings.OnJsPromiseFinishedFunctionName + @"(numId, false, JSON.stringify(value));
            },
            function (error) {
                var errorSerialized = JSON.stringify(error);
                if (!error || errorSerialized === '{}') {
                    errorSerialized = 'Empty exception in js promise.';
                }
                " + Settings.OnJsPromiseFinishedFunctionName + @"(numId, true, JSON.stringify(errorSerialized));
            }
        );

    } catch (e) {
        NativeBridge.OnJsPromiseFinished(numId, true, 'Evaluation error: ' + JSON.stringify(e));
    }
    return numId;
})();
";
            await sender.ExJsDirect(allScript, timeoutMS);
            //Wait callback.
            var res = await tcs.Task;
            return res;
        }

        /// <summary>
        /// Execute script with promise support.
        /// </summary>
        async Task<JToken> ExJs_PromisesNotSupported(IImprovedWebView sender, string script, int? timeoutMS)
        {
            var scriptSerialized = JsonConvert.SerializeObject(script);
            var allScript = @"
(function () {
    var script = " + scriptSerialized + @";
    var res = {};
    try {
        var evalRes = eval('(() => {' + script + '})()');
        res.isError = false;
        res.result = JSON.stringify(evalRes);
    } catch (ex) {
        res.isError = true;
        res.result = JSON.stringify(ex);
    }
    return res;
})();
";
            var jsResult = await sender.ExJsDirect(allScript, timeoutMS);
            var executionResult = ExecutionResult.FromJson(jsResult);
            if (executionResult.IsError)
            {
                Debug.WriteLine($"ImprovedWebView error: {executionResult.Result}");
                throw new ImprovedWebViewException($"Error in js: '{executionResult.Result}'");
            }
            else
            {
                return executionResult.Result;
            }
        }
        #endregion
    }
}