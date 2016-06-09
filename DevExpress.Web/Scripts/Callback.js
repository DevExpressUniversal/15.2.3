/// <reference path="_references.js"/>

(function () {
var ASPxClientCallback = ASPx.CreateClass(ASPxClientComponent, {
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);
        this.CallbackComplete = new ASPxClientEvent();
    },
    SendCallback: function (parameter) {
        this.PerformCallback(parameter);
    },
    PerformCallback: function (parameter) {
        if (!ASPx.IsExists(parameter)) parameter = "";
        this.CreateCallback(parameter);
    },
    OnCallback: function (result) {
        var args = new ASPxClientCallbackCompleteEventArgs(result.parameter, result.data);
        this.CallbackComplete.FireEvent(this, args);
    }
});
ASPxClientCallback.Cast = ASPxClientControl.Cast;
var ASPxClientCallbackCompleteEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function (parameter, result) {
        this.constructor.prototype.constructor.call(this);
        this.parameter = parameter;
        this.result = result;
    }
});

window.ASPxClientCallback = ASPxClientCallback;
window.ASPxClientCallbackCompleteEventArgs = ASPxClientCallbackCompleteEventArgs;
})();