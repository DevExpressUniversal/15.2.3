

(function() {
var MVCxClientPopupControl = ASPx.CreateClass(ASPxClientPopupControl, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.callbackUrl = "";
        this.callbackCustomArgs = {};
    },
    PerformCallback: function(data){
        MVCx.MergeHashTables(this.callbackCustomArgs, data);
        ASPxClientPopupControl.prototype.PerformCallback.call(this, data);
    },
    PerformWindowCallback: function(window, data) {
        MVCx.MergeHashTables(this.callbackCustomArgs, data);
        this.callbackCustomArgs["windowIndex"] = window ? window.index : -1;
        ASPxClientPopupControl.prototype.PerformWindowCallback.call(this, window, data);
    },
    InlineInitialize: function(){
        if(this.callbackUrl != "")
            this.callBack = function(arg){ MVCx.PerformControlCallback(this.name, this.callbackUrl, arg, null, this.callbackCustomArgs) }; 

        ASPxClientPopupControl.prototype.InlineInitialize.call(this);
    },
    RaiseBeginCallbackInternal: function(command){
        var args = new MVCxClientBeginCallbackEventArgs(command);
        if(!this.BeginCallback.IsEmpty())
            this.BeginCallback.FireEvent(this, args);
        MVCxClientGlobalEvents.OnBeginCallback(args);
        MVCx.MergeHashTables(this.callbackCustomArgs, args.customArgs);
    },
    RaiseEndCallback: function() {
        ASPxClientPopupControl.prototype.RaiseEndCallback.call(this);
        MVCxClientGlobalEvents.OnEndCallback();
    },
    RaiseCallbackError: function(message) {
        var result = ASPxClientPopupControl.prototype.RaiseCallbackError.call(this, message);
        if(!result.isHandled) {
            var args = new ASPxClientCallbackErrorEventArgs(message);
            MVCxClientGlobalEvents.OnCallbackError(args);
            result = { isHandled: args.handled, errorMessage: args.message };
            if(result.isHandled)
                this.HideAllLoadingPanels();
        }
        return result;
    },
    CreateCallbackByInfo: function(arg, command, callbackInfo) {
        this.CreateCallbackInternal(arg, command, true, callbackInfo);
    },
    CreateCallbackCore: function(arg, command, callbackID){
        if(this.callbackCustomArgs != {})
            window.setTimeout(function(){ this.callbackCustomArgs = {}; }.aspxBind(this), 0);
        ASPxClientPopupControl.prototype.CreateCallbackCore.call(this, arg, command, callbackID);
    },
    EvalCallbackResult: function(resultString){
        var resultStringParts = resultString.split(MVCx.CallbackHtmlContentPrefix);
        if(resultStringParts.length == 2){
            var resultObj = ASPxClientPopupControl.prototype.EvalCallbackResult.call(this, resultStringParts[0]);
            resultObj.result.html = resultStringParts[1];
            return resultObj;
        }
        return ASPxClientPopupControl.prototype.EvalCallbackResult.call(this, resultString);
    }
});
MVCxClientPopupControl.Cast = ASPxClientControl.Cast;

window.MVCxClientPopupControl = MVCxClientPopupControl;
})();