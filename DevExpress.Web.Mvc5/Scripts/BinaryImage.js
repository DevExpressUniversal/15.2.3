(function() {
var MVCxClientBinaryImage = ASPx.CreateClass(ASPxClientBinaryImage, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.callbackUrl = "";
    },
    InlineInitialize: function() {
        if(this.callbackUrl != "")
            this.callBack = function(arg){ MVCx.PerformControlCallback(this.name, this.callbackUrl, arg, this.GetCallbackParams(), null) }; 
        ASPxClientBinaryImage.prototype.InlineInitialize.call(this);
    },
    CreateCallbackByInfo: function(arg, command, callbackInfo) {
        this.CreateCallbackInternal(arg, command, true, callbackInfo);
    },
    GetCallbackMethod: function(command){
        return this.callBack;
    },
    GetCallbackParams: function(){
        var params = { };
        MVCx.AddCallbackParam(params, this.GetValueHiddenField());
        params.MVCxBinaryImageUniqueID = this.GetValueHiddenField().name;
        return params;
    }
});

window.MVCxClientBinaryImage = MVCxClientBinaryImage;
})();