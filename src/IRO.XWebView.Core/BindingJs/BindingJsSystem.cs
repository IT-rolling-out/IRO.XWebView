using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IRO.XWebView.Core.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IRO.XWebView.Core.BindingJs
{
    public class BindingJsSystem : IBindingJsSystem
    {
        public const string JsBridgeObjectName = "NativeBridge";

        #region Static.

        public static object[] JsonToParams(ICollection<ParameterInfo> parameters, JToken jTokens)
        {
            var res = new object[parameters.Count];
            var i = 0;
            foreach (var param in parameters)
            {
                try
                {
                    var jToken = jTokens[i];
                    var deserializedParameter = jToken.ToObject(param.ParameterType);
                    res[i] = deserializedParameter;
                }
                catch
                {
                    res[i] = DefaultOf(param.ParameterType);
                }

                i++;
            }

            return res;
        }

        static object DefaultOf(Type t)
        {
            if (t.IsValueType)
            {
                return Activator.CreateInstance(t);
            }

            return null;
        }

        #endregion

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
            IXWebView sender,
            string jsObjectName,
            string functionName,
            string parametersJson,
            string resolveFunctionName,
            string rejectFunctionName
        )
        {
            Task.Run(async () =>
            {
                var parameters = JToken.Parse(parametersJson);
                var key = jsObjectName + "." + functionName;
                var bindedMethodData = _methods[key];
                var paramsArr = JsonToParams(bindedMethodData.Parameters, parameters);
                try
                {
                    var methodRes = bindedMethodData.Method.Invoke(bindedMethodData.InvokeOn, paramsArr);
                    var methodRealRes = new object();
                    if (methodRes is Task task)
                    {
                        await task;
                        if (task.IsFaulted || task.IsCanceled)
                        {
                            //!Reject
                            var ex = task.Exception;
                            RejectPromise(sender, rejectFunctionName, ex);
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
                    ResolvePromise(sender, resolveFunctionName, methodRealRes);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"XWebView error: {ex}");
                    //!Reject
                    RejectPromise(sender, rejectFunctionName, ex);
                }
            });
        }

        public ExecutionResult OnJsCallNativeSync(IXWebView sender, string jsObjectName, string functionName,
            string parametersJson)
        {
            var parameters = JToken.Parse(parametersJson);
            var key = jsObjectName + "." + functionName;
            var bindedMethodData = _methods[key];
            var paramsArr = JsonToParams(bindedMethodData.Parameters, parameters);
            ExecutionResult jsSyncResult;
            try
            {
                var methodRes = bindedMethodData.Method.Invoke(bindedMethodData.InvokeOn, paramsArr);
                jsSyncResult = new ExecutionResult()
                {
                    Result = JToken.FromObject(methodRes),
                    IsError = false
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"XWebView error: {ex}");
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

        public void UnbindFromJs(string functionName, string jsObjectName)
        {
            var key = jsObjectName + "." + functionName;
            _methods.Remove(key);
            _pageInitializationJs_CacheUpdated = false;
        }

        public void UnbindAllFromJs()
        {
            _methods.Clear();
            _pageInitializationJs_CacheUpdated = false;
        }

        public string GetIsBridgeAttachedScript()
        {
            return $@"
(function(){{
  if(window.NativeBridgeInitialized){{
    return true;
  }}
  return false;
}})();
";
        }

        /// <summary>
        /// Return script used to add support of js2native calls.
        /// </summary>
        /// <returns></returns>
        public string GetAttachBridgeScript()
        {
            if (_pageInitializationJs_CacheUpdated)
                return _pageInitializationJs_Cached;

            var checkLowLevelNativeBridgeScript = GetCheckLowLevelNativeBridgeScript();
            var initNativeBridgeScript_Start = @"
function FullBridgeInit() {
" + checkLowLevelNativeBridgeScript + @"

    var w = window;
    if (w['NativeBridgeInitStarted']){
        console.warn('Native bridge was initialized before.');
        return;
    }
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
        " + $"{JsBridgeObjectName}.{nameof(OnJsCallNativeAsync)}" +
                                               @"(jsObjectName, functionName, parametersJson, resolveFunctionName, rejectFunctionName);
        return resPromise;
    };
    var sc = function SyncCall(jsObjectName, functionName, callArguments) {
        var callArgumentsArr = Array.prototype.slice.call(callArguments);
        var parametersJson = JSON.stringify(callArgumentsArr);
        var nativeMethodResJson = " + $"{JsBridgeObjectName}.{nameof(OnJsCallNativeSync)}" +
                                               @"(jsObjectName, functionName, parametersJson);
        var nativeMethodRes = JSON.parse(nativeMethodResJson);
        if (nativeMethodRes.IsError) {
            throw nativeMethodRes.Result;
        } else {
            return nativeMethodRes.Result;
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
  window.NativeBridgeInitialized = true;
  console.log('Native bridge initialized.');
}
FullBridgeInit();
";

            var methodsRegistrationScript = GenerateMethodsRegistrationScript();
            var script = initNativeBridgeScript_Start + methodsRegistrationScript + initNativeBridgeScript_End;
            _pageInitializationJs_Cached = script;
            _pageInitializationJs_CacheUpdated = true;
            return script;
        }

        string GetCheckLowLevelNativeBridgeScript()
        {
            var checkLowLevelNativeBridgeScript = @"
if(!window['" + JsBridgeObjectName + @"'])
    window['" + JsBridgeObjectName + @"'] = {};
var jsBridge = window['" + JsBridgeObjectName + @"']
";


            var methodNames = new string[]
            {
                nameof(OnJsCallNativeAsync),
                nameof(OnJsCallNativeSync),
                nameof(OnJsPromiseFinished)
            };
            foreach (var methodName in methodNames)
            {
                var line = $@"
    if(!jsBridge['{methodName}']){{
        jsBridge.{methodName}" + @" = function(){
            var obj = {};
            obj.methodName = '" + methodName + $@"';
            obj.parameters = Array.prototype.slice.call(arguments);
            return jsBridge.OnJsCall(JSON.stringify(obj));             
        }}
    }}
";
                checkLowLevelNativeBridgeScript += line;
            }

            //Log all calls if NativeBridge not registered. 
            //Useful in debug.
            checkLowLevelNativeBridgeScript += $@"
if(!jsBridge['OnJsCall']){{
    console.warn('OnJsCall work in log only mode. Native method wasn`t inplemented.');
    jsBridge.OnJsCall = function(jsonParameters){{
        console.log('OnJsCall invoked with params: ');
        console.log(jsonParameters);
        return {{""Result"":""empty""}};
    }}
}}";
            return checkLowLevelNativeBridgeScript;
        }

        void RejectPromise(IXWebView sender, string rejectFunctionName, Exception ex)
        {
            try
            {
                var serializedEx = JsonConvert.SerializeObject(ex.ToString());
                sender.UnmanagedExecuteJavascriptAsync($"{rejectFunctionName}({serializedEx});");
            }
            catch (Exception newEx)
            {
                Debug.WriteLine($"XWebView error: {newEx}");
                //Ignore exceptions. It can be rised, for example, when we load new page.
            }
        }

        void ResolvePromise(IXWebView sender, string resolveFunctionName, object res)
        {
            try
            {
                var serializedRes = JsonConvert.SerializeObject(res);
                sender.UnmanagedExecuteJavascriptAsync($"{resolveFunctionName}({serializedRes});");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"XWebView error: {ex}");
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
        /// <inheridoc></inheridoc>
        public bool UnsafeEval { get; set; }

        readonly IDictionary<string, TaskCompletionSource<JToken>> _pendingPromisesCallbacks =
            new ConcurrentDictionary<string, TaskCompletionSource<JToken>>();

        readonly Random _random = new Random();

        public void OnJsPromiseFinished(IXWebView sender, string taskCompletionSourceId,
            ExecutionResult executionResult)
        {
            Task.Run(() =>
            {
                if (!_pendingPromisesCallbacks.TryGetValue(taskCompletionSourceId, out var tcs))
                    return;
                try
                {
                    _pendingPromisesCallbacks.Remove(taskCompletionSourceId);
                    if (executionResult.IsError)
                    {
                        tcs.TrySetException(new XWebViewException(executionResult.Result.ToString()));
                    }
                    else
                    {
                        tcs.TrySetResult(executionResult.Result);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"XWebView error: {ex}");
                    tcs.TrySetException(new XWebViewException("", ex));
                }
            });
        }

        public async Task<TResult> ExJs<TResult>(IXWebView sender, string script, bool promiseResultSupport,
            int? timeoutMS)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            if (script == null) throw new ArgumentNullException(nameof(script));

            try
            {
                Task<JToken> task;
                if (promiseResultSupport)
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
                        Debug.WriteLine($"XWebView error: 'Js evaluation timeout'");
                        throw new XWebViewException($"Js evaluation timeout {timeoutMS}");
                    }
                }

                var res = await task;
                if (res == null)
                {
                    return default(TResult);
                }

                return res.ToObject<TResult>();
            }
            catch(Exception ex)
            {
                throw new XWebViewException($"{nameof(ExJs)} exception.", ex);
            }
        }

        async Task<JToken> ExJs_PromisesSupported(IXWebView sender, string script, int? timeoutMS)
        {
            var taskId = _random.Next(10000, 99999).ToString();
            var tcs = new TaskCompletionSource<JToken>(TaskCreationOptions.RunContinuationsAsynchronously);
            _pendingPromisesCallbacks[taskId] = tcs;

            var scriptToInvoke = GetEvalScript(script);
            var taskIdSerialized = JsonConvert.SerializeObject(taskId);
            var allScript = @"
(function () {
    var numId = " + taskIdSerialized + @";
    try {
        var evalRes = " + scriptToInvoke + @";
        if(!evalRes){
          /*Without result.*/
          " + $"{JsBridgeObjectName}.{nameof(OnJsPromiseFinished)}" + @"(numId, false, 'null');
          return;
        }
        if((!evalRes.then) || (typeof evalRes.then != 'function')){
          /*Sync function.*/
          " + $"{JsBridgeObjectName}.{nameof(OnJsPromiseFinished)}" + @"(numId, false, JSON.stringify(evalRes));
          return;
        }
        evalRes.then(
            function (value) {
                " + $"{JsBridgeObjectName}.{nameof(OnJsPromiseFinished)}" + @"(numId, false, JSON.stringify(value));
            },
            function (e) {
                " + $"{JsBridgeObjectName}.{nameof(OnJsPromiseFinished)}" + @"(numId, true, JSON.stringify(e) + ' : ' + e);
            }
        );

    } catch (e) {
        " + $"{JsBridgeObjectName}.{nameof(OnJsPromiseFinished)}" +
                            @"(numId, true, 'Evaluation error: ' + JSON.stringify(e) + ' : ' + e);
    }
})();
";
            sender.UnmanagedExecuteJavascriptAsync(allScript, timeoutMS);
            //Wait callback.
            var res = await tcs.Task;
            return res;
        }

        /// <summary>
        /// Execute script with promise support.
        /// </summary>
        async Task<JToken> ExJs_PromisesNotSupported(IXWebView sender, string script, int? timeoutMS)
        {
            var scriptToInvoke = GetEvalScript(script);
            var allScript = @"
(function () {
    var res = {};
    try {
        var evalRes = " + scriptToInvoke + @";
        res.IsError = false;
        res.Result = JSON.stringify(evalRes);
    } catch (ex) {
        res.IsError = true;
        res.Result = JSON.stringify(ex + '');
    }
    return res;
})();
";

            var jsResult = await sender.UnmanagedExecuteJavascriptWithResult(allScript, timeoutMS);
            var executionResult = ExecutionResult.FromJson(jsResult);
            if (executionResult.IsError)
            {
                Debug.WriteLine($"XWebView error: {executionResult.Result}");
                throw new XWebViewException($"Error in js: '{executionResult.Result}'");
            }
            else
            {
                return executionResult.Result;
            }
        }

        string GetEvalScript(string passedScript)
        {
            if (UnsafeEval)
            {
                var script = @"(function(){ " + passedScript + @" })()";
                return script;
            }
            else 
            {
                var scriptSerialized = JsonConvert.SerializeObject(passedScript);
                //Remove brackets.
                scriptSerialized = scriptSerialized.Substring(1);
                scriptSerialized=scriptSerialized.Remove(scriptSerialized.Length - 1);
                var script = @"window.eval(""(function(){ " + scriptSerialized + @" })();"")";
                return script;
            }
        }

        #endregion
    }
}