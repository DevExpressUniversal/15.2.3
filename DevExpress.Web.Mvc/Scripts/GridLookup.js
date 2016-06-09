(function() {
var MVCxClientGridLookup = ASPx.CreateClass(ASPxClientGridLookup, {
    GridViewMethods: {
        CreateCallbackCore: function (arg, command, callbackID) {
            if (this.IsFilterByTextCallback(arg, command)) {
                var callback = this.callbackMethods[ASPxClientGridViewCallbackCommand.ApplyFilter] || this.callBack;
                callback.call(this, this.GetSerializedCallbackInfoByID(callbackID) + arg);
                return;
            }

            MVCxClientGridView.prototype.CreateCallbackCore.call(this, arg, command, callbackID);
        },
        IsFilterByTextCallback: function (arg, command) {
            return command == ASPxClientGridViewCallbackCommand.CustomCallback && arg && arg.indexOf("GLP_F") > -1;
        },
        GetCallbackParams: function(arg) {
            var gridParams = MVCxClientGridView.prototype.GetCallbackParams.call(this, arg);
            var lookup = ASPx.GetControlCollection().Get(this.lookupName);
            return lookup ? MVCx.MergeHashTables(gridParams, lookup.GetCallbackParams()) : gridParams;
        }
    },

    SendGridViewCustomCallback: function(command, args) {
        var inputElement = this.GetInputElement();
        if(ASPx.IsExistsElement(inputElement)){
            var grid = this.GetGridView();
            grid.callbackCustomArgs[this.name] = inputElement.value;
        }
        ASPxClientGridLookup.prototype.SendGridViewCustomCallback.call(this, command, args);
    },
    InitializeGridViewInstance: function(){
        var isGridViewCreated = !!this.gridView;
        ASPxClientGridLookup.prototype.InitializeGridViewInstance.call(this);
        
        if(!isGridViewCreated){
            var inst = this;
            inst.gridView.lookupName = this.name
            $.each(this.GridViewMethods, function(key, value){
                inst.gridView[key] = value;
            });
        }
    },
    GetCallbackParams: function(){
        var params = {};
        MVCx.AddCallbackParam(params, this.GetStateHiddenField());
        MVCx.AddCallbackParamsInContainer(params, this.GetMainElement());
        return params;
    }
});

window.MVCxClientGridLookup = MVCxClientGridLookup;
})();