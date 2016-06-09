

(function(window) {
    var MVCxClientReportDesigner = ASPx.CreateClass(ASPxClientReportDesigner, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);

            this.callbackUrl = "";
            this.downloadUrl = "";
            this.callbackCustomArgs = {};
            this.SaveCommandExecuted = new ASPxClientEvent();
        },
        InlineInitialize: function() {
            if(this.callbackUrl != "")
                this.callBack = function(arg) { MVCx.PerformControlCallback(this.name, this.callbackUrl, arg, this.GetCallbackParams(arg), this.callbackCustomArgs) };
            ASPxClientReportDesigner.prototype.InlineInitialize.call(this);
        },
        PerformCallback: function(args) {
            if(args && typeof args !== 'object') {
                args = { args: args };
            }
            this.callbackCustomArgs = args || {};
            this.performCallbackCore();
        },
        RaiseBeginCallbackInternal: function(command) {
            var args = new MVCxClientBeginCallbackEventArgs(command);
            if(!this.BeginCallback.IsEmpty())
                this.BeginCallback.FireEvent(this, args);
            MVCxClientGlobalEvents.OnBeginCallback(args);
            MVCx.MergeHashTables(this.callbackCustomArgs, args.customArgs);
        },
        RaiseEndCallback: function() {
            ASPxClientReportDesigner.prototype.RaiseEndCallback.call(this);
            MVCxClientGlobalEvents.OnEndCallback();
        },
        RaiseCallbackError: function(message) {
            var result = ASPxClientReportDesigner.prototype.RaiseCallbackError.call(this, message);
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
        CreateCallbackCore: function(arg, command, callbackID) {
            window.setTimeout(function() { this.callbackCustomArgs = {}; }.aspxBind(this), 0);
            ASPxClientReportDesigner.prototype.CreateCallbackCore.call(this, arg, command, callbackID);
        },
        GetCallbackMethod: function(command) {
            return MVCx.IsCustomCallback(command) ? MVCx.GetCustomActionCallBackMethod(this) : this.callBack;
        },
        GetCallbackParams: function(arg) {
            var params = {};
            params[this.name + "_DXReportDesigner"] = arg;
            return params;
        },
        DoCallback: function(result) {
            if(result.indexOf(ASPx.CallbackResultPrefix) === 0) {
                ASPxClientReportDesigner.prototype.DoCallback.call(this, result);
                return;
            }
            this.DoLoadCallbackScripts();
            var args = new MVCxClientReportDesignerSaveCommandExecutedEventArgs(result);
            this.SaveCommandExecuted.FireEvent(this, args);
        },
        canSaveExecuteCore: function(arg) {
            return this.callBack && ASPxClientReportDesigner.prototype.canSaveExecuteCore(arg);
        }
    });
    var MVCxClientReportDesignerSaveCommandExecutedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(result) {
            this.constructor.prototype.constructor.call(this);
            this.Result = result;
        }
    });

    window.MVCxClientReportDesigner = MVCxClientReportDesigner;
    window.MVCxClientReportDesignerSaveCommandExecutedEventArgs = MVCxClientReportDesignerSaveCommandExecutedEventArgs;
})(window);