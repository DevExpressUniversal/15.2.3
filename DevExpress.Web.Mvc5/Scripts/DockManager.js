

(function() {
var MVCxClientDockManager = ASPx.CreateClass(ASPxClientDockManager, {
    constructor: function(name) {
        var dockManagerInstance = ASPxClientDockManager.Get();
        if(dockManagerInstance && dockManagerInstance.name == name)
            ASPxClientDockManager.instance = this;
        this.callbackUrl = "";
        this.callbackCustomArgs = {};
        this.constructor.prototype.constructor.call(this, name);
    },
    PerformCallback: function(data) {
        MVCx.MergeHashTables(this.callbackCustomArgs, data);
        ASPxClientDockManager.prototype.PerformCallback.call(this, data);
    },
    InlineInitialize: function() {
        if(this.callbackUrl != "")
            this.callBack = function(arg) { MVCx.PerformControlCallback(this.name, this.callbackUrl, arg, this.GetCallbackParams(), this.callbackCustomArgs) };

        ASPxClientDockManager.prototype.InlineInitialize.call(this);
    },
    CreateCallbackByInfo: function(arg, command, callbackInfo) {
        this.CreateCallbackInternal(arg, command, true, callbackInfo);
    },
    CreateCallbackCore: function(arg, command, callbackID) {
        if(this.callbackCustomArgs != {})
            window.setTimeout(function() { this.callbackCustomArgs = {}; }.aspxBind(this), 0);
        ASPxClientDockManager.prototype.CreateCallbackCore.call(this, arg, command, callbackID);
    },
    GetCallbackParams: function() {
        this.UpdatePanelsLayoutState();
        var params = {};
        params["ClientLayoutState"] = ASPx.Json.ToJson(this.clientLayoutState);
        return params;
    },
    //Layout state
    UpdatePanelLayoutState: function(panel) {
        if(panel.firstShowProcessed)
            ASPxClientDockManager.prototype.UpdatePanelLayoutState.call(this, panel);
    }
});
MVCxClientDockManager.Cast = ASPxClientControl.Cast;

window.MVCxClientDockManager = MVCxClientDockManager;
})();