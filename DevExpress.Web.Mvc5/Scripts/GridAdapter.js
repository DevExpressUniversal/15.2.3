(function () {
var MVCxGridAdapter = ASPx.CreateClass(null, {
    constructor: function(grid){
        this.grid = grid;
    },
    InlineInitialize: function(){
        var grid = this.grid;

        if(grid.callbackUrl != "")
            grid.callBack = function(arg){ MVCx.PerformControlCallback(grid.name, grid.callbackUrl, arg, grid.GetCallbackParams(arg), grid.callbackCustomArgs) };
        if(grid.addNewItemUrl != "")
            grid.addNewItemCallBack = function (arg) { MVCx.PerformControlCallback(grid.name, grid.addNewItemUrl, arg, grid.GetCallbackParams(arg), grid.callbackCustomArgs) };
        if(grid.updateItemUrl != "")
            grid.updateItemCallBack = function (arg) { MVCx.PerformControlCallback(grid.name, grid.updateItemUrl, arg, grid.GetCallbackParams(arg), grid.callbackCustomArgs) };
        if(grid.deleteItemUrl != "")
            grid.deleteItemCallBack = function (arg) { MVCx.PerformControlCallback(grid.name, grid.deleteItemUrl, arg, grid.GetCallbackParams(arg), grid.callbackCustomArgs) };
        if(grid.allowBatchEditing && grid.batchUpdateUrl != "")
            grid.batchUpdateCallBack = function(arg) { MVCx.PerformControlCallback(grid.name, grid.batchUpdateUrl, arg, grid.GetCallbackParams(arg), grid.callbackCustomArgs) }; 

        for(var command in grid.callbackActionUrlCollection){
            (function(command){
                grid.callbackMethods[command] = function(arg){
                    MVCx.PerformControlCallback(grid.name, grid.callbackActionUrlCollection[command], arg, grid.GetCallbackParams(arg), grid.callbackCustomArgs) 
                };
            }).call(this, command);
        }
    },
    ValidateEditorsByJQuery: function(){
        var isValid = true;
        var jQValidation = MVCx.JQueryValidation;
        if(jQValidation.IsEnabled(this.grid)){
            jQValidation.PrepareUVRules(this.grid);
            $.each(this.GetEditorsToValidate(), function(i, editor){
                isValid &= jQValidation.Validate(editor);
            });
            if(jQValidation.HasPendingRequests(this.grid)){
                jQValidation.SetOnStopRequestHandler(this.grid, this.grid.UpdateEdit.aspxBind(this));
                isValid = false;
            }
        }
        return isValid;
    },
    GetEditorsToValidate: function(){
        var editors = this.grid._getEditors() || [];
        $.each(this.grid.columns, function(i, column){
            var control = ASPx.GetControlCollection().Get(column.fieldName);
            if(control && ASPx.Ident.IsASPxClientEdit(control))
                editors.push(control);
        });
        return editors;
    },
    GetCallbackMethod: function(command){
        var grid = this.grid;
        if(command == "FUNCTION")
            return grid.funcCallbackMethod || grid.callBack;
        if(command == ASPxClientGridViewCallbackCommand.DeleteRow)
            return grid.deleteItemCallBack;
        if(command == ASPxClientGridViewCallbackCommand.UpdateEdit && grid.allowBatchEditing)
            return grid.batchUpdateCallBack;
        if(command == ASPxClientGridViewCallbackCommand.UpdateEdit)
            return grid.IsNewItemEditing() ? grid.addNewItemCallBack : grid.updateItemCallBack;
        
        return grid.callbackMethods[command] || grid.callBack;
    },
    GetCallbackParams: function(){
        var params = {};
        MVCx.AddCallbackParam(params, this.grid.GetStateHiddenField());
        MVCx.AddCallbackParamsInContainer(params, this.grid.GetMainElement());

        this.AddEditingCallbackParams(params);
        return params;
    },

    AddEditingCallbackParams: function(params){
        this.AddEditorsCallbackParams(params);

        var keyName = this.grid.keyName;
        if(keyName != "" && !params[keyName])
            params[keyName] = this.GetKeyValue();

        if(this.grid.allowBatchEditing)
            this.AddBatchEditingCallbackParams(params);
    },
    AddEditorsCallbackParams: function(params){
        var grid = this.grid;
        var editFields = [];
        var editors = grid._getEditors();
        for(var i = 0; i < editors.length; i++){
            var editorIndex = grid.GetEditorIndex(editors[i].name);
            var column = grid.columns[editorIndex];
            params[column.fieldName] = ASPx.Json.ToJson(MVCx.GetEditorValueByControl(editors[i]));
            editFields.push(column.fieldName);
        }
        if(editFields.length > 0)
            params["DXMVCGridEditFields"] = ASPx.Json.ToJson(editFields);
        MVCx.AddDXEditorValuesInContainer(params, grid.GetMainElement());
    },
    GetKeyValue: function(){
        var keyValue = null;
        var grid = this.grid;
        
        if(grid.deleteKeyValue){
            keyValue = grid.deleteKeyValue;
            grid.deleteKeyValue = null;
        }
        else if(!grid.IsNewItemEditing() && grid.editingItemVisibleIndex > -1)
            keyValue = grid.GetItemKey(grid.editingItemVisibleIndex);
        
        return keyValue;
    },
    AddBatchEditingCallbackParams: function(params){
        params["DXMVCBatchEditingValuesRequestKey"] = this.grid.uniqueID;
        params["DXMVCBatchEditingKeyFieldName"] = this.grid.keyName;
    },
    GridCallBack: function(args) {
        if(args[0] === ASPxClientGridViewCallbackCommand.Sort)
            this.grid.callbackCustomArgs["reset"] = args[4];
    },
    OnAfterCallback: function() {
        MVCx.JQueryValidation.ResetUVRules(this.grid);
    }
});

var GridBatchEditHelperAdapter = ASPx.CreateClass(null, {
    constructor: function() {
        this.batchEditHelper = null;
        this.grid = null;
    },

    Init: function(batchEditHelper) {
        this.batchEditHelper = batchEditHelper;
        this.grid = this.batchEditHelper.grid;
    },
    
    PrepareEditorsValidation: function(){
        if(!MVCx.JQueryValidation.IsEnabled(this.grid))
            return;

        MVCx.JQueryValidation.PrepareUVRules(this.grid);
        var columnIndices = this.batchEditHelper.GetEditColumnIndices();
        for(var i = 0; i < columnIndices.length; i++) {
            var editor = this.grid.GetEditor(columnIndices[i]);
            if(editor){
                editor.GotFocus.AddHandler(function(s){ $.validator.prototype.lastElement = s.GetInputElement(); });
                editor.LostFocus.AddHandler(function(s){ $.validator.lastElement = null; });
            }
        }
    },

    CanEndEdit: function() {
        var jQValidation = MVCx.JQueryValidation;
        return !jQValidation.IsEnabled(this.grid) || !jQValidation.HasPendingRequests(this.grid);
    },

    SafeCallEndEdit: function(endEditFunc) {
        var oldValidateInvisibleEditors = MVCx.validateInvisibleEditors;
        try {
            MVCx.validateInvisibleEditors = true;
            return endEditFunc();
        } finally{
            MVCx.validateInvisibleEditors = oldValidateInvisibleEditors;
        }
    },
    
    ValidateEditor: function(editor){
        if(MVCx.JQueryValidation.IsEnabled(this.grid))
            MVCx.JQueryValidation.Validate(editor);
    }
});

function InitUnobtrusiveRules(name, rules){
    var editor = ASPx.GetControlCollection().Get(name);
    MVCx.JQueryValidation.SetUVAttributes(editor, rules);
}

MVCx.GridAdapter = MVCxGridAdapter;
MVCx.GridBatchEditHelperAdapter = GridBatchEditHelperAdapter;
MVCx.InitUnobtrusiveRules = InitUnobtrusiveRules;
})();