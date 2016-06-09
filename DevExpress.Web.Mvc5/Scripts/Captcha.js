(function() {
var MVCxClientCaptcha = ASPx.CreateClass(ASPxClientCaptcha, {
    constructor: function(name){
        this.constructor.prototype.constructor.call(this, name);
        this.callbackUrl = "";
    },
    InlineInitialize: function(){
        if(this.callbackUrl != "")
            this.callBack = function(arg){ MVCx.PerformControlCallback(this.name, this.callbackUrl, arg, null, null) };

        ASPxClientCaptcha.prototype.InlineInitialize.call(this);
    },
    CreateCallbackByInfo: function(arg, command, callbackInfo){
        this.CreateCallbackInternal(arg, command, true, callbackInfo);
    }
});

window.MVCxClientCaptcha = MVCxClientCaptcha;
})();