

(function() {
var MVCxClientHtmlEditor = ASPx.CreateClass(ASPxClientHtmlEditor, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.callbackUrl = "";
        this.customDataActionUrl = "";
        this.callbackCustomArgs = {};

        this.customDataActionCallback;
    },
    PerformDataCallback: function(data, onCallback) {
        MVCx.MergeHashTables(this.callbackCustomArgs, data);
        ASPxClientHtmlEditor.prototype.PerformDataCallback.call(this, ASPx.Json.ToJson(this.callbackCustomArgs), onCallback);
    },
    CreateCustomDataCallback: function(arg, command, handler) {
        ASPxClientHtmlEditor.prototype.CreateCustomDataCallback.call(this, arg, "CUSTOMDATACALLBACK", handler);
    },
    InlineInitialize: function(){
        if(this.callbackUrl != "")
            this.callBack = function(arg){ MVCx.PerformControlCallback(this.name, this.callbackUrl, arg, this.GetCallbackParams(), this.callbackCustomArgs) };
        if(this.customDataActionUrl != "")
            this.customDataActionCallback = function(arg) { MVCx.PerformControlCallback(this.name, this.customDataActionUrl, arg, this.GetCallbackParams(), this.callbackCustomArgs) };

        var element = this.GetMainElement();
        if(element)
            element.appendChild(ASPx.CreateHiddenField(this.name + "_DXMVCHtmlEditor"));

        ASPxClientHtmlEditor.prototype.InlineInitialize.call(this);
    },

    RaiseBeginCallbackInternal: function(command){
        var args = new MVCxClientBeginCallbackEventArgs(command);
        if(!this.BeginCallback.IsEmpty())
            this.BeginCallback.FireEvent(this, args);
        MVCxClientGlobalEvents.OnBeginCallback(args);
        MVCx.MergeHashTables(this.callbackCustomArgs, args.customArgs);
    },
    RaiseEndCallback: function() {
        ASPxClientHtmlEditor.prototype.RaiseEndCallback.call(this);
        MVCxClientGlobalEvents.OnEndCallback();
    },
    RaiseCallbackError: function(message) {
        var result = ASPxClientHtmlEditor.prototype.RaiseCallbackError.call(this, message);
        if(!result.isHandled) {
            var args = new ASPxClientCallbackErrorEventArgs(message);
            MVCxClientGlobalEvents.OnCallbackError(args);
            result = { isHandled: args.handled, errorMessage: args.message };
        }
        return result;
    },
    CreateCallbackByInfo: function(arg, command, callbackInfo) {
        var callbackId = this.CreateCallbackInternal(arg, command, true, callbackInfo);
        return callbackId;
    },
    GetCallbackParams: function() {
        var params = {};
        MVCx.AddCallbackParam(params, this.GetStateHiddenField());
        MVCx.AddCallbackParamsInContainer(params, this.GetMainElement());
        return params;
    },
    CreateCallbackCore: function(arg, command, callbackID) {
        if(this.callbackCustomArgs != {})
            window.setTimeout(function() { this.callbackCustomArgs = {}; } .aspxBind(this), 0);
        ASPxClientHtmlEditor.prototype.CreateCallbackCore.call(this, arg, command, callbackID);
    },
    GetCallbackMethod: function(command) {
        if(MVCx.IsCustomDataCallback(command) && this.customDataActionCallback)
            return this.customDataActionCallback;
        return this.callBack;
    },
    EvalCallbackResult: function(resultString){
        var resultStringParts = resultString.split(MVCx.CallbackHtmlContentPrefix);
        if(resultStringParts.length == 2 && resultStringParts[1] != ""){
            var resultObj = ASPxClientHtmlEditor.prototype.EvalCallbackResult.call(this, resultStringParts[0]);
            resultObj.result = resultObj.result.replace(MVCx.CallbackHtmlContentPlaceholder, resultStringParts[1]);
            return resultObj;
        }
        return ASPxClientHtmlEditor.prototype.EvalCallbackResult.call(this, resultString);
    },
    Export: function(format) {
        var form = ASPx.CreateHtmlElementFromString("<form method='post' action='" + this.exportUrl + (this.exportUrl.indexOf("?") > -1 ? "&" : "?") + "format=" + format + "'> </form>");
        var heMainEl = this.GetMainElement();
        this.OnPost();
        $(heMainEl).parent().find("input").each(function() {
            form.appendChild(this.cloneNode(true));
        });
        document.body.appendChild(form);
        form.submit();
        document.body.removeChild(form);
    }
});
MVCxClientHtmlEditor.Cast = ASPxClientControl.Cast;

window.MVCxClientHtmlEditor = MVCxClientHtmlEditor;
})();