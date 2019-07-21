//Async method call.
var NativeBridge = {};
NativeBridge.OnJsPromiseFinished = function () {
    var callArgumentsArr = Array.prototype.slice.call(arguments);
    console.log(callArgumentsArr);
};

(function () {
    var script = "function delayPromise(delayMS) {\n  return new Promise(function(resolve, reject){\n    setTimeout(function(){\n      resolve('msg');\n    }, delayMS)\n  });\n}\nreturn await delayPromise(4000);";
    var numId = "31579";
    try {
        var evalRes = eval('(async () => {' + script + '})()');
        evalRes.then(
            function (value) {
                NativeBridge.OnJsPromiseFinished(numId, false, JSON.stringify(value));
            },
            function (error) {
                var errorSerialized = JSON.stringify(error);
                if (!error || errorSerialized === '{}') {
                    errorSerialized = 'Empty exception in js promise.';
                }
                NativeBridge.OnJsPromiseFinished(numId, true, JSON.stringify(errorSerialized));
            }
        );

    } catch (e) {
        NativeBridge.OnJsPromiseFinished(numId, true, "Evaluation error: " + JSON.stringify(e));
    }
    return numId;
})();


//Sync.
(function () {
    var script = 'var msg=\'qqqq\'; return msg;';
    var res = {};
    try {
        var evalRes = eval('(() => {' + script + '})()');
        res.result = evalRes;
    } catch (ex) {
        res.isError = true;
        res.result = JSON.stringify(ex);
    }
    return JSON.stringify(res);
})();
