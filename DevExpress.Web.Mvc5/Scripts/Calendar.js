

(function() {
var MVCxClientCalendar = ASPx.CreateClass(ASPxClientCalendar, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.callbackUrl = "";
        this.callbackCustomArgs = {};
    },
    InlineInitialize: function(){
        if(this.callbackUrl != "")
            this.callBack = function(arg){ MVCx.PerformControlCallback(this.name, this.callbackUrl, arg, this.GetCallbackParams(), this.callbackCustomArgs) }; 

        ASPxClientCalendar.prototype.InlineInitialize.call(this);
    },
    RaiseBeginCallbackInternal: function (command) {
        var args = new MVCxClientBeginCallbackEventArgs(command);
        if (!this.BeginCallback.IsEmpty())
            this.BeginCallback.FireEvent(this, args);
        MVCxClientGlobalEvents.OnBeginCallback(args);
        MVCx.MergeHashTables(this.callbackCustomArgs, args.customArgs);
    },
    RaiseEndCallback: function () {
        ASPxClientCalendar.prototype.RaiseEndCallback.call(this);
        MVCxClientGlobalEvents.OnEndCallback();
    },
    RaiseCallbackError: function (message) {
        var result = ASPxClientCalendar.prototype.RaiseCallbackError.call(this, message);
        if (!result.isHandled) {
            var args = new ASPxClientCallbackErrorEventArgs(message);
            MVCxClientGlobalEvents.OnCallbackError(args);
            result = { isHandled: args.handled, errorMessage: args.message };
        }
        return result;
    },
    CreateCallbackByInfo: function(arg, command, callbackInfo) {
        this.CreateCallbackInternal(arg, command, true, callbackInfo);
    },
    GetCallbackParams: function() {
        var params = {};
        MVCx.AddCallbackParam(params, this.GetStateHiddenField());
        MVCx.AddCallbackParamsInContainer(params, this.GetMainElement());
        return params;
    },
    EvalCallbackResult: function(resultString){
        var resultStringParts = resultString.split(MVCx.CallbackHtmlContentPrefix);
        if(resultStringParts.length == 2){
            var resultObj = ASPxClientCalendar.prototype.EvalCallbackResult.call(this, resultStringParts[0]);
            var resultHtml = eval(resultStringParts[1]);
            for(var index = 0; index < resultHtml.length; index++){
                resultObj.result[index] = resultHtml[index];
            }
            return resultObj;
        }
        return ASPxClientCalendar.prototype.EvalCallbackResult.call(this, resultString);
    }
});
MVCxClientCalendar.Cast = ASPxClientControl.Cast;

window.MVCxClientCalendar = MVCxClientCalendar;
})();