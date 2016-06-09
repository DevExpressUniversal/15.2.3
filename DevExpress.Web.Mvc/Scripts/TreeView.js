

(function() {
var MVCxClientTreeView = ASPx.CreateClass(ASPxClientTreeView, {
    NodesInfoHiddenInputIDPostfix: "_NIHF",

    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.callbackUrl = "";
        this.callbackCustomArgs = {};
    },
    InlineInitialize: function(){
        if(this.callbackUrl != "")
            this.callBack = function(arg){ MVCx.PerformControlCallback(this.name, this.callbackUrl, arg, this.GetCallbackParams(), this.callbackCustomArgs) }; 

        ASPxClientTreeView.prototype.InlineInitialize.call(this);
    },

    RaiseBeginCallbackInternal: function(command) {
        var args = new MVCxClientBeginCallbackEventArgs(command);
        if(!this.BeginCallback.IsEmpty())
            this.BeginCallback.FireEvent(this, args);
        MVCxClientGlobalEvents.OnBeginCallback(args);
        this.callbackCustomArgs = args.customArgs;
    },
    RaiseEndCallback: function() {
        ASPxClientTreeView.prototype.RaiseEndCallback.call(this);
        MVCxClientGlobalEvents.OnEndCallback();
    },
    RaiseCallbackError: function(message) {
        var result = ASPxClientTreeView.prototype.RaiseCallbackError.call(this, message);
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
    EvalCallbackResult: function(resultString){
        var resultStringParts = resultString.split(MVCx.CallbackHtmlContentPrefix);
        if(resultStringParts.length == 2){
            var resultObj = ASPxClientTreeView.prototype.EvalCallbackResult.call(this, resultStringParts[0]);
            resultObj.result[2] = resultObj.result[2].replace(MVCx.CallbackHtmlContentPlaceholder, resultStringParts[1]);
            return resultObj;
        }
        return ASPxClientTreeView.prototype.EvalCallbackResult.call(this, resultString);
    },
    GetCallbackParams: function(){
        var params = {};
        MVCx.AddCallbackParam(params, this.GetStateHiddenField());
        MVCx.AddCallbackParamsInContainer(params, this.GetMainElement());
        return params;
    },
    OnCallback: function (resultObj) {
        ASPxClientTreeView.prototype.OnCallback.call(this, resultObj);
        if(resultObj && resultObj[0] == this.ExpandNodeCommand)
            this.stateObject.nodesInfo = resultObj[resultObj.length - 1];
    }
});
MVCxClientTreeView.Cast = ASPxClientControl.Cast;

window.MVCxClientTreeView = MVCxClientTreeView;
})();