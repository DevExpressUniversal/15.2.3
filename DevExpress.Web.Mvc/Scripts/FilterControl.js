(function() {
var MVCxClientFilterControl = ASPx.CreateClass(ASPxClientFilterControl, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.callbackUrl = "";
        this.callbackCustomArgs = { };
        this.callbackActionUrlCollection = { };
        this.callbackMethods = { };
    },
    InlineInitialize: function() {
        if(this.callbackUrl != "")
            this.callBack = function(arg){ MVCx.PerformControlCallback(this.name, this.callbackUrl, arg, this.GetCallbackParams(), this.callbackCustomArgs) }; 

        for(var command in this.callbackActionUrlCollection){
            (function(command){
                this.callbackMethods[command] = function(arg){
                    MVCx.PerformControlCallback(this.name, this.callbackActionUrlCollection[command], arg, this.GetCallbackParams(), this.callbackCustomArgs) 
                };
            }).call(this, command);
        }

        ASPxClientFilterControl.prototype.InlineInitialize.call(this);
    },
    CreateCallbackByInfo: function(arg, command, callbackInfo) {
        this.CreateCallbackInternal(arg, command, true, callbackInfo);
    },
    GetCallbackMethod: function(command){
        return this.callbackMethods[command] || this.callBack;
    },
    GetCallbackParams: function() {
        var params = {};
        MVCx.AddCallbackParam(params, this.GetStateHiddenField());
        MVCx.AddCallbackParamsInContainer(params, this.GetMainElement());

        var owner = this.GetOwnerControl();
        if(owner) {
            var ownerParams = owner.GetCallbackParams();
            for(var key in ownerParams)
                params[key] = ownerParams[key];
        }
        return params;
    },
    EvalCallbackResult: function(resultString){
        var resultStringParts = resultString.split(MVCx.CallbackHtmlContentPrefix);
        if(resultStringParts.length == 2){
            var resultObj = ASPxClientFilterControl.prototype.EvalCallbackResult.call(this, resultStringParts[0]);
            resultObj.result.html = resultStringParts[1];
            return resultObj;
        }
        return ASPxClientFilterControl.prototype.EvalCallbackResult.call(this, resultString);
    },
    GetOwnerControl: function(){
        var FilterControlPrefix = "_DXPFCForm_DXPFC";
        if(this.name.indexOf(FilterControlPrefix) < 0)
            return null;
        return ASPx.GetControlCollection().Get(this.name.replace(FilterControlPrefix, ""));
    }
});

window.MVCxClientFilterControl = MVCxClientFilterControl;
})();