//Native bridge. Must be registered on low level.
function OnJsCallAsync(jsObjectName, functionName, parametersJson, resolveFunctionName, rejectFunctionName) {
    var callArgumentsArr = Array.prototype.slice.call(arguments);
    //console.log(callArgumentsArr);
    //Emulate.
    return window[resolveFunctionName]('some args');
}
function OnJsCallSync(jsObjectName, functionName, parametersJson) {
    var callArgumentsArr = Array.prototype.slice.call(arguments);
    //console.log(callArgumentsArr);
    //Emulate.
    var res = { result: 'my res', isError: false };
    return JSON.stringify(res);
}


function InitNativeBridge() {
    var w = window;
    //if (window['NativeBridgeInitStarted'])
    //    return;
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

    //Register methods.
    ram("Math", "SumAsync");
    rsm("Math", "SumSync");
}
InitNativeBridge();

Math.SumSync(11, 22);
Math.SumAsync(33, 44);