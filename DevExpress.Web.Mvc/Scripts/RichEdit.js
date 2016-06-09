

(function() {
var MVCxClientRichEdit = ASPx.CreateClass(ASPxClientRichEdit, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.callbackUrl = "";
        this.callbackCustomArgs = {};
        this.customActionUrl = "";

        this.customActionCallBack;
    },
    InlineInitialize: function(){
        if(this.callbackUrl != "")
            this.callBack = function(arg){ MVCx.PerformControlCallback(this.name, this.callbackUrl, arg, this.GetCallbackParams(arg), this.callbackCustomArgs) };
        if(this.customActionUrl != "") {
            this.customActionCallBack = function(arg) { 
                MVCx.PerformControlCallback(this.name, this.customActionUrl, arg, this.GetCallbackParams(arg), this.callbackCustomArgs) 
            };
        }

        ASPxClientRichEdit.prototype.InlineInitialize.call(this);
    },
    PerformCallback: function(data) {
        MVCx.MergeHashTables(this.callbackCustomArgs, data);
        ASPxClientRichEdit.prototype.PerformCallback.call(this, ASPx.Json.ToJson(this.callbackCustomArgs));
    },
    RaiseBeginCallbackInternal: function(command){
        var args = new MVCxClientBeginCallbackEventArgs(command);
        if(!this.BeginCallback.IsEmpty())
            this.BeginCallback.FireEvent(this, args);
        MVCxClientGlobalEvents.OnBeginCallback(args);
        MVCx.MergeHashTables(this.callbackCustomArgs, args.customArgs);
    },
    RaiseEndCallback: function() {
        ASPxClientRichEdit.prototype.RaiseEndCallback.call(this);
        MVCxClientGlobalEvents.OnEndCallback();
    },
    RaiseCallbackError: function(message) {
        var result = ASPxClientRichEdit.prototype.RaiseCallbackError.call(this, message);
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
    GetCallbackParams: function(arg) {
        var params = {};
        MVCx.AddCallbackParam(params, this.GetStateHiddenField());
        MVCx.AddCallbackParamsInContainer(params, this.GetMainElement());
        return params;
    },
    CreateCallback: function(arg, command) {
        var argParts = arg.split('|');
        if(argParts.length > 1 && argParts[0] === ASPxClientRichEdit.Constants.PerformCallbackPrefix)
            command = "CUSTOMCALLBACK";
        ASPxClientRichEdit.prototype.CreateCallback.call(this, arg, command);
    },
    CreateCallbackCore: function(arg, command, callbackID) {
        if(this.callbackCustomArgs != {})
            window.setTimeout(function() { this.callbackCustomArgs = {}; } .aspxBind(this), 0);
        ASPxClientRichEdit.prototype.CreateCallbackCore.call(this, arg, command, callbackID);
    },
    GetCallbackMethod: function(command) {
        if(MVCx.IsCustomCallback(command))
            return MVCx.GetCustomActionCallBackMethod(this);
        return this.callBack;
    }
});
MVCxClientRichEdit.Cast = ASPxClientControl.Cast;

window.MVCxClientRichEdit = MVCxClientRichEdit;
})();