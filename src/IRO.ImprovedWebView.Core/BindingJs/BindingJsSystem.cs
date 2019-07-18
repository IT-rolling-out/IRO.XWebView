using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IRO.ImprovedWebView.Core.BindingJs
{
    public class BindingJsSystem : IBindingJsSystem
    {
        const string FuncName_Async = "sa";

        const string FuncName_Sync = "ss";

        /// <summary>
        /// Key is 'jsObjectName.actionName'.
        /// </summary>
        readonly IDictionary<string, BindedMethodData> _methods = new Dictionary<string, BindedMethodData>();

        /// <summary>
        /// If registered method return Task, it means  promises used.
        /// </summary>
        public void OnJsCallAsync(
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
                    //!Reject
                    await RejectPromise(sender, resolveFunctionName, ex);
                }
            });


        }

        public string OnJsCallSync(IImprovedWebView sender, string jsObjectName, string functionName, string parametersJson)
        {
            var parameters = JToken.Parse(parametersJson);
            var key = jsObjectName + "." + functionName;
            var bindedMethodData = _methods[key];
            var paramsArr = JsonToParams(bindedMethodData.Parameters, parameters);
            BindingJsSyncResult jsSyncResult;
            try
            {
                object methodRes = bindedMethodData.Method.Invoke(bindedMethodData.InvokeOn, paramsArr);
                jsSyncResult = new BindingJsSyncResult()
                {
                    Result = methodRes,
                    IsError = false
                };
            }
            catch (Exception ex)
            {
                jsSyncResult = new BindingJsSyncResult()
                {
                    Result = ex.ToString(),
                    IsError = true
                };
            }
            var res = JsonConvert.SerializeObject(jsSyncResult);
            return res;
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
        }

        object[] JsonToParams(ParameterInfo[] parameters, JToken jToken)
        {
            var res = new object[parameters.Length];
            int i = 0;
            foreach (var param in parameters)
            {
                i++;
                var deserializedParameter = jToken[i].ToObject(param.ParameterType);
                res[i] = deserializedParameter;
            }
            return res;
        }

        async Task RejectPromise(IImprovedWebView sender, string rejectFunctionName, Exception ex)
        {
            try
            {
                var serializedEx = JsonConvert.SerializeObject(ex.Message.ToString());
                await sender.ExJs<object>($"{rejectFunctionName}({serializedEx});");
            }
            catch
            {
                //Ignore exceptions. It can be rised, for example, when we load new page.
            }
        }

        async Task ResolvePromise(IImprovedWebView sender, string resolveFunctionName, object res)
        {
            try
            {
                var serializedRes = JsonConvert.SerializeObject(res);
                await sender.ExJs<object>($"{resolveFunctionName}({serializedRes});");
            }
            catch
            {
                //Ignore exceptions. It can be rised, for example, when we load new page.
            }
        }

        /// <summary>
        /// Return script used to add support of js2native calls.
        /// </summary>
        /// <returns></returns>
        public string GeneratePageInitializationJs()
        {
            const string initNativeBridgeScript_Start = @"
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
        OnJsCallAsync(jsObjectName, functionName, parametersJson, resolveFunctionName, rejectFunctionName);
        return resPromise;
    };
    var sc = function SyncCall(jsObjectName, functionName, callArguments) {
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

            var methodsRegistrationScript=GenerateMethodsRegistrationScript();
            var script = initNativeBridgeScript_Start + methodsRegistrationScript + initNativeBridgeScript_End;
            return script;
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
    }
}