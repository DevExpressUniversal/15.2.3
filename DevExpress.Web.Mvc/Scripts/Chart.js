

(function() {
var MVCxClientChart = ASPx.CreateClass(ASPxClientWebChartControl, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.callbackUrl = "";
        this.customActionUrl = "";
        this.callbackCustomArgs = {};

        this.customActionCallBack;
    },
    PerformCallback: function(data){
        MVCx.MergeHashTables(this.callbackCustomArgs, data);
        ASPxClientWebChartControl.prototype.PerformCallback.call(this, data);
    },
    InlineInitialize: function() {
        if (this.callbackUrl != "")
            this.callBack = function(arg) { MVCx.PerformControlCallback(this.name, this.callbackUrl, arg, null, this.callbackCustomArgs) };
        if (this.customActionUrl != "")
            this.customActionCallBack = function(arg){ MVCx.PerformControlCallback(this.name, this.customActionUrl, arg, null, this.callbackCustomArgs) };;
        ASPxClientWebChartControl.prototype.InlineInitialize.call(this);
    },
    RaiseBeginCallbackInternal: function(command) {
        var args = new MVCxClientBeginCallbackEventArgs(command);
        if(!this.BeginCallback.IsEmpty())
            this.BeginCallback.FireEvent(this, args);
        MVCxClientGlobalEvents.OnBeginCallback(args);
        MVCx.MergeHashTables(this.callbackCustomArgs, args.customArgs);
    },
    RaiseEndCallback: function() {
        ASPxClientWebChartControl.prototype.RaiseEndCallback.call(this);
        MVCxClientGlobalEvents.OnEndCallback();
    },
    RaiseCallbackError: function(message) {
        var result = ASPxClientWebChartControl.prototype.RaiseCallbackError.call(this, message);
        if(!result.isHandled) {
            var args = new ASPxClientCallbackErrorEventArgs(message);
            MVCxClientGlobalEvents.OnCallbackError(args);
            result = { isHandled: args.handled, errorMessage: args.message };
        }
        return result;
    },
    CreateCallbackByInfo: function(arg, command, callbackInfo) {
        this.CreateCallbackInternal(arg, command, true, callbackInfo);
    },
    CreateCallbackCore: function(arg, command, callbackID){
        if(this.callbackCustomArgs != {})
            window.setTimeout(function(){ this.callbackCustomArgs = {}; }.aspxBind(this), 0);
        ASPxClientWebChartControl.prototype.CreateCallbackCore.call(this, arg, command, callbackID);
    },
    GetCallbackMethod: function(command){
        return MVCx.IsCustomCallback(command) ? MVCx.GetCustomActionCallBackMethod(this) : this.callBack;
    },
    EvalCallbackResult: function(resultString) {
        var resultStringParts = resultString.split(MVCx.CallbackHtmlContentPrefix);
        if (resultStringParts.length == 2) {
            var resultObj = ASPxClientWebChartControl.prototype.EvalCallbackResult.call(this, resultStringParts[0]);
            resultObj.result.html = resultStringParts[1];
            return resultObj;
        }
        return ASPxClientWebChartControl.prototype.EvalCallbackResult.call(this, resultString);
    }
});
MVCxClientChart.Cast = ASPxClientControl.Cast;

window.MVCxClientChart = MVCxClientChart;
})();