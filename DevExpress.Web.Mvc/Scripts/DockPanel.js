

(function() {
var MVCxClientDockPanel = ASPx.CreateClass(ASPxClientDockPanel, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.callbackUrl = "";
        this.callbackCustomArgs = {};
    },
    PerformCallback: function(data) {
        MVCx.MergeHashTables(this.callbackCustomArgs, data);
        ASPxClientDockPanel.prototype.PerformCallback.call(this, data);
    },
    InlineInitialize: function() {
        if(this.callbackUrl != "")
            this.callBack = function(arg) { MVCx.PerformControlCallback(this.name, this.callbackUrl, arg, null, this.callbackCustomArgs) };

        ASPxClientDockPanel.prototype.InlineInitialize.call(this);
    },
    InitializeWindow: function(index) {
        this.TryToLoadStateFromDockManager();
        ASPxClientDockPanel.prototype.InitializeWindow.call(this, index);
    },
    TryToLoadStateFromDockManager: function() {
        if(typeof(ASPxClientDockManager) != "undefined" && ASPxClientDockManager.Get()) {
            var panelLayoutState = ASPxClientDockManager.Get().clientLayoutState[this.panelUID];
            if(!panelLayoutState) return;
            
            this.showOnPageLoad = !!panelLayoutState[0];
            this.mode = panelLayoutState[1];
            this.SetZoneUID(panelLayoutState[2]);
            var width = parseInt(panelLayoutState[3].replace('px', '')),
                height = parseInt(panelLayoutState[4].replace('px', ''));
            if(width && this.widthFixed)
                this.floatingStateDimensions.width = width;
            if(height && this.heightFixed)
                this.floatingStateDimensions.height = height;
            this.left = panelLayoutState[5];
            this.top = panelLayoutState[6];
            this.SetVisibleIndexCore(panelLayoutState[7]);
        }
    },
    AfterInitialize: function() {
        ASPxClientDockPanel.prototype.AfterInitialize.call(this);
        if(this.restoredFloatingStateDimensions) {
            this.StoreFloatingStateDimensionsCore(this.restoredFloatingStateDimensions.width, this.restoredFloatingStateDimensions.height);
            this.restoredFloatingStateDimensions = null;
        }
    },
    DoHideWindowCore: function(index) {
        if(!this.GetWindowCachedSize(index))
            this.SetWindowCachedSize(index, this.GetWidth(), this.GetHeight());
        ASPxClientDockPanel.prototype.DoHideWindowCore.call(this, index);
    },
    GetLayoutStateObject: function() {
        var state = ASPxClientDockPanel.prototype.GetLayoutStateObject.call(this);
        if(!parseInt(state[3]))
            state[3] = this.GetStoredDimensionValue(true);
        if(!parseInt(state[4]))
            state[4] = this.GetStoredDimensionValue(false);
        return state;
    },
    GetStoredDimensionValue: function(isWidth) {
        var cachedSize = this.GetWindowCachedSize();
        var dimensionValue = cachedSize ? cachedSize[isWidth ? "width" : "height"] : this[isWidth ? "GetWidth" : "GetHeight"]();
        return dimensionValue + '';
    },
    RaiseBeginCallbackInternal: function(command) {
        var args = new MVCxClientBeginCallbackEventArgs(command);
        if(!this.BeginCallback.IsEmpty())
            this.BeginCallback.FireEvent(this, args);
        MVCxClientGlobalEvents.OnBeginCallback(args);
        MVCx.MergeHashTables(this.callbackCustomArgs, args.customArgs);
    },
    RaiseEndCallback: function() {
        ASPxClientDockPanel.prototype.RaiseEndCallback.call(this);
        MVCxClientGlobalEvents.OnEndCallback();
    },
    RaiseCallbackError: function(message) {
        var result = ASPxClientDockPanel.prototype.RaiseCallbackError.call(this, message);
        if(!result.isHandled) {
            var args = new ASPxClientCallbackErrorEventArgs(message);
            MVCxClientGlobalEvents.OnCallbackError(args);
            result = { isHandled: args.handled, errorMessage: args.message };
            if(result.isHandled)
                this.HideAllLoadingPanels();
        }
        return result;
    },
    CreateCallbackByInfo: function(arg, command, callbackInfo) {
        this.CreateCallbackInternal(arg, command, true, callbackInfo);
    },
    CreateCallbackCore: function(arg, command, callbackID){
        if(this.callbackCustomArgs != {})
            window.setTimeout(function(){ this.callbackCustomArgs = {}; }.aspxBind(this), 0);
        ASPxClientDockPanel.prototype.CreateCallbackCore.call(this, arg, command, callbackID);
    },
    EvalCallbackResult: function(resultString) {
        var resultStringParts = resultString.split(MVCx.CallbackHtmlContentPrefix);
        if(resultStringParts.length == 2) {
            var resultObj = ASPxClientDockPanel.prototype.EvalCallbackResult.call(this, resultStringParts[0]);
            resultObj.result.html = resultStringParts[1];
            return resultObj;
        }
        return ASPxClientDockPanel.prototype.EvalCallbackResult.call(this, resultString);
    }
});
MVCxClientDockPanel.Cast = ASPxClientControl.Cast;

window.MVCxClientDockPanel = MVCxClientDockPanel;
})();