

(function() {
var MVCxClientPivotGrid = ASPx.CreateClass(ASPxClientPivotGrid, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.callbackUrl = "";
        this.customActionUrl = "";
        this.callbackCustomArgs = {};
        this.pivotCustomizationExtensionName = "";

        this.customActionCallBack;
    },
    PerformCallback: function(data){
        MVCx.MergeHashTables(this.callbackCustomArgs, data);
        ASPxClientPivotGrid.prototype.PerformCallback.call(this, data);
    },
    InlineInitialize: function(){
        if(this.callbackUrl != "")
            this.callBack = function(arg){ MVCx.PerformControlCallback(this.name, this.callbackUrl, arg, this.GetCallbackParams(), this.callbackCustomArgs) }; 
        if (this.customActionUrl != "")
            this.customActionCallBack = function(arg){ MVCx.PerformControlCallback(this.name, this.customActionUrl, arg, this.GetCallbackParams(arg), this.callbackCustomArgs) };
        
        ASPxClientPivotGrid.prototype.InlineInitialize.call(this);
    },
    RaiseBeginCallbackInternal: function(command){
        var args = new MVCxClientBeginCallbackEventArgs(command);
        if(!this.BeginCallback.IsEmpty())
            this.BeginCallback.FireEvent(this, args);
        MVCxClientGlobalEvents.OnBeginCallback(args);
        MVCx.MergeHashTables(this.callbackCustomArgs, args.customArgs);
    },
    CreateCallbackByInfo: function(arg, command, callbackInfo) {
        this.CreateCallbackInternal(arg, command, true, callbackInfo);
    },
    CreateCallbackCore: function(arg, command, callbackID){
        if(this.callbackCustomArgs != {})
            window.setTimeout(function(){ this.callbackCustomArgs = {}; }.aspxBind(this), 0);
        ASPxClientPivotGrid.prototype.CreateCallbackCore.call(this, arg, command, callbackID);
    },
    GetCallbackMethod: function(command){
        return MVCx.IsCustomCallback(command) ? MVCx.GetCustomActionCallBackMethod(this) : this.callBack;
    },
    GetCallbackParams: function() {
        var params = {};
        this.FillCallbackParamsInternal(this.name, params, true);
        if(this.pivotCustomizationExtensionName)
            this.FillCallbackParamsInternal(this.pivotCustomizationExtensionName, params, false);
        return params;
    },
    FillCallbackParamsInternal: function(name, params, includeTreeView){
        var control = ASPx.GetControlCollection().Get(name);
        MVCx.AddCallbackParam(params, control.GetStateHiddenField());
        MVCx.AddCallbackParamsInContainer(params, control.GetMainElement());

		if(includeTreeView) {
			var treeView = control.GetTreeView();
			treeView && MVCx.AddCallbackParam(params, treeView.GetStateHiddenField());
		}
    },
    FillStateObject: function(obj) {
        if (!obj) 
            obj = {};
        this.OnPost();
        var params = this.GetCallbackParams();
        for(var key in params) {
            obj[key] = params[key];
        }
    }
});
MVCxClientPivotGrid.Cast = ASPxClientControl.Cast;

window.MVCxClientPivotGrid = MVCxClientPivotGrid;
})();