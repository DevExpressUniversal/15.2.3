

(function() {
var MVCxClientCallbackPanel = ASPx.CreateClass(ASPxClientCallbackPanel, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.callbackUrl = "";
        this.callbackCustomArgs = {};
    },
    PerformCallback: function(data) {
        MVCx.MergeHashTables(this.callbackCustomArgs, data);
        ASPxClientCallbackPanel.prototype.PerformCallback.call(this, data);
    },
    InlineInitialize: function(){
        if(this.callbackUrl != "")
            this.callBack = function(arg){ MVCx.PerformControlCallback(this.name, this.callbackUrl, arg, this.callbackCustomArgs, this.GetCallbackParams()) }; 

        ASPxClientCallbackPanel.prototype.InlineInitialize.call(this);
    },

    RaiseBeginCallbackInternal: function(command) {
        var args = new MVCxClientBeginCallbackEventArgs(command);
        if(!this.BeginCallback.IsEmpty())
            this.BeginCallback.FireEvent(this, args);
        MVCxClientGlobalEvents.OnBeginCallback(args);
        MVCx.MergeHashTables(this.callbackCustomArgs, args.customArgs);
    },
    RaiseEndCallback: function() {
        ASPxClientCallbackPanel.prototype.RaiseEndCallback.call(this);
        MVCxClientGlobalEvents.OnEndCallback();
    },
    RaiseCallbackError: function(message) {
        var result = ASPxClientCallbackPanel.prototype.RaiseCallbackError.call(this, message);
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
        var editorValues = $("[name=" + MVCx.EditorsValuesKey + "]").val();
        if(editorValues)
            params[MVCx.EditorsValuesKey] = editorValues;
        MVCx.AddCallbackParamsInContainer(params, this.GetMainElement());
        return params;
    },
    CreateCallbackCore: function(arg, command, callbackID) {
        if(this.callbackCustomArgs != {})
            window.setTimeout(function () { this.callbackCustomArgs = {}; }.aspxBind(this), 0);
        ASPxClientCallbackPanel.prototype.CreateCallbackCore.call(this, arg, command, callbackID);
    },
    EvalCallbackResult: function(resultString){
        var resultStringParts = resultString.split(MVCx.CallbackHtmlContentPrefix);
        if(resultStringParts.length == 2){
            var resultObj = ASPxClientCallbackPanel.prototype.EvalCallbackResult.call(this, resultStringParts[0]);
            resultObj.result = resultStringParts[1];
            return resultObj;
        }
        return ASPxClientCallbackPanel.prototype.EvalCallbackResult.call(this, resultString);
    }
});
MVCxClientCallbackPanel.Cast = ASPxClientControl.Cast;

window.MVCxClientCallbackPanel = MVCxClientCallbackPanel;
})();