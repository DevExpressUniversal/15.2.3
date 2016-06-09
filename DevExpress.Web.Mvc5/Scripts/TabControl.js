

(function() {
var MVCxClientPageControl = ASPx.CreateClass(ASPxClientPageControl, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.callbackUrl = "";
        this.callbackCustomArgs = {};
    },
    PerformCallback: function(data) {
        MVCx.MergeHashTables(this.callbackCustomArgs, data);
        ASPxClientPageControl.prototype.PerformCallback.call(this, data);
    },
    InlineInitialize: function(){
        if(this.callbackUrl != "")
            this.callBack = function(arg){ MVCx.PerformControlCallback(this.name, this.callbackUrl, arg, this.GetCallbackParams(arg), this.callbackCustomArgs) }; 
        
        ASPxClientPageControl.prototype.InlineInitialize.call(this);
    },

    RaiseBeginCallbackInternal: function(command) {
        var args = new MVCxClientBeginCallbackEventArgs(command);
        if(!this.BeginCallback.IsEmpty())
            this.BeginCallback.FireEvent(this, args);
        MVCxClientGlobalEvents.OnBeginCallback(args);
        MVCx.MergeHashTables(this.callbackCustomArgs, args.customArgs);
    },
    RaiseEndCallback: function() {
        ASPxClientPageControl.prototype.RaiseEndCallback.call(this);
        MVCxClientGlobalEvents.OnEndCallback();
    },
    RaiseCallbackError: function(message) {
        var result = ASPxClientPageControl.prototype.RaiseCallbackError.call(this, message);
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
    CreateCallbackCore: function(arg, command, callbackID) {
        if(this.callbackCustomArgs != {})
            window.setTimeout(function() { this.callbackCustomArgs = {}; }.aspxBind(this), 0);
        ASPxClientPageControl.prototype.CreateCallbackCore.call(this, arg, command, callbackID);
    },
    GetCallbackParams: function(arg){
        var params = {};
        MVCx.AddCallbackParam(params, this.GetStateHiddenField());
        MVCx.AddCallbackParamsInContainer(params, this.GetMainElement());
        return params;
    },
    EvalCallbackResult: function(resultString){
        var resultStringParts = resultString.split(MVCx.CallbackHtmlContentPrefix);
        if(resultStringParts.length == 2){
            var resultObj = ASPxClientPageControl.prototype.EvalCallbackResult.call(this, resultStringParts[0]);
            resultObj.result.html = resultStringParts[1];
            return resultObj;
        }
        return ASPxClientPageControl.prototype.EvalCallbackResult.call(this, resultString);
    }
});
MVCxClientPageControl.Cast = ASPxClientControl.Cast;

window.MVCxClientPageControl = MVCxClientPageControl;
})();