(function() {
var MVCxClientSpellChecker = ASPx.CreateClass(ASPxClientSpellChecker, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.callbackUrl = "";
    },
    InlineInitialize: function(){
        if(this.callbackUrl != "")
            this.callBack = function (arg) { MVCx.PerformControlCallback(this.name, this.callbackUrl, arg, this.GetCallbackParams(), null) };
        ASPxClientSpellChecker.prototype.InlineInitialize.call(this);
    },
    CreateCallbackByInfo: function (arg, command, callbackInfo) {
        this.CreateCallbackInternal(arg, command, true, callbackInfo);
    },
    GetCallbackParams: function() {
        var params = {};
        MVCx.AddCallbackParam(params, this.GetStateHiddenField());
        MVCx.AddCallbackParamsInContainer(params, this.GetMainElement());
        return params;
    },
    EvalCallbackResult: function(resultString) {
        var resultStringParts = resultString.split(MVCx.CallbackHtmlContentPrefix);
        if(resultStringParts.length == 2 && resultStringParts[1] != "") {
            var resultObj = ASPxClientSpellChecker.prototype.EvalCallbackResult.call(this, resultStringParts[0]);
            resultObj.result.dialogContent = resultObj.result.dialogContent.replace(MVCx.CallbackHtmlContentPlaceholder, resultStringParts[1]);
            return resultObj;
        }
        return ASPxClientSpellChecker.prototype.EvalCallbackResult.call(this, resultString);
    }
});

window.MVCxClientSpellChecker = MVCxClientSpellChecker;
})();