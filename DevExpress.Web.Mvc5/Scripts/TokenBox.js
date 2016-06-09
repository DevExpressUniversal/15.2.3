

(function() {
var MVCxClientTokenBox = ASPx.CreateClass(ASPxClientTokenBox, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.callbackUrl = "";
        this.callbackCustomArgs = {};
    },
    PerformCallback: function(data) {
        MVCx.MergeHashTables(this.callbackCustomArgs, data);
        ASPxClientTokenBox.prototype.PerformCallback.call(this, data);
    },
    InlineInitialize: function() {
        if(this.callbackUrl != "") {
            this.callBack = function(arg) { MVCx.PerformControlCallback(this.name, this.callbackUrl, arg, null, this.callbackCustomArgs) };
            this.isCallbackMode = true;
        }

        ASPxClientTokenBox.prototype.InlineInitialize.call(this);
    },

    RaiseBeginCallbackInternal: function(command) {
        var args = new MVCxClientBeginCallbackEventArgs(command);
        if(!this.BeginCallback.IsEmpty())
            this.BeginCallback.FireEvent(this, args);
        MVCxClientGlobalEvents.OnBeginCallback(args);
        MVCx.MergeHashTables(this.callbackCustomArgs, args.customArgs);
    },
    RaiseEndCallback: function() {
        ASPxClientTokenBox.prototype.RaiseEndCallback.call(this);
        MVCxClientGlobalEvents.OnEndCallback();
    },
    RaiseCallbackError: function(message) {
        var result = ASPxClientTokenBox.prototype.RaiseCallbackError.call(this, message);
        if(!result.isHandled) {
            var args = new ASPxClientCallbackErrorEventArgs(message);
            MVCxClientGlobalEvents.OnCallbackError(args);
            result = { isHandled: args.handled, errorMessage: args.message };
        }
        return result;
    },
    CreateCallbackByInfo: function(arg, command, callbackInfo) {
        if(this.CanCreateCallback())
            this.CreateCallbackInternal(arg, command, true, callbackInfo);
    },
    CreateCallbackCore: function(arg, command, callbackID) {
        if(this.callbackCustomArgs != {})
            window.setTimeout(function() { this.callbackCustomArgs = {}; }.aspxBind(this), 0);
        ASPxClientTokenBox.prototype.CreateCallbackCore.call(this, arg, command, callbackID);
    }
});
MVCxClientTokenBox.Cast = ASPxClientControl.Cast;

window.MVCxClientTokenBox = MVCxClientTokenBox;
})();