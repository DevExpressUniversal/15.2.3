

(function() {
var MVCxClientRoundPanel = ASPx.CreateClass(ASPxClientRoundPanel, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.callbackUrl = "";
        this.callbackCustomArgs = {};
    },
    PerformCallback: function(data) {
        MVCx.MergeHashTables(this.callbackCustomArgs, data);
        ASPxClientRoundPanel.prototype.PerformCallback.call(this, data);
    },
    InlineInitialize: function(){
        if(this.callbackUrl != "")
            this.callBack = function(arg){ MVCx.PerformControlCallback(this.name, this.callbackUrl, arg, null, this.callbackCustomArgs) }; 

        ASPxClientRoundPanel.prototype.InlineInitialize.call(this);
    },

    RaiseBeginCallbackInternal: function(command) {
        var args = new MVCxClientBeginCallbackEventArgs(command);
        if(!this.BeginCallback.IsEmpty())
            this.BeginCallback.FireEvent(this, args);
        MVCxClientGlobalEvents.OnBeginCallback(args);
        MVCx.MergeHashTables(this.callbackCustomArgs, args.customArgs);
    },
    RaiseEndCallback: function() {
        ASPxClientRoundPanel.prototype.RaiseEndCallback.call(this);
        MVCxClientGlobalEvents.OnEndCallback();
    },
    RaiseCallbackError: function(message) {
        var result = ASPxClientRoundPanel.prototype.RaiseCallbackError.call(this, message);
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
            window.setTimeout(function () { this.callbackCustomArgs = {}; } .aspxBind(this), 0);
        ASPxClientRoundPanel.prototype.CreateCallbackCore.call(this, arg, command, callbackID);
    },
    EvalCallbackResult: function(resultString){
        var resultStringParts = resultString.split(MVCx.CallbackHtmlContentPrefix);
        if(resultStringParts.length == 2){
            var resultObj = ASPxClientRoundPanel.prototype.EvalCallbackResult.call(this, resultStringParts[0]);
            resultObj.result = resultStringParts[1];
            return resultObj;
        }
        return ASPxClientRoundPanel.prototype.EvalCallbackResult.call(this, resultString);
    }
});
MVCxClientRoundPanel.Cast = ASPxClientControl.Cast;

window.MVCxClientRoundPanel = MVCxClientRoundPanel;
})();