

(function() {
var MVCxClientTreeList = ASPx.CreateClass(ASPxClientTreeList, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.callbackUrl = "";
        this.customActionUrl = "";
        this.customDataActionUrl = "";
        this.addNewNodeUrl = "";
        this.updateNodeUrl = "";
        this.moveNodeUrl = "";
        this.deleteNodeUrl = "";
        this.callbackCustomArgs = {};

        this.customActionCallBack;
        this.customDataActionCallback;
        this.addNewNodeCallback;
        this.moveNodeCallback;
        this.deleteNodeCallback;
        this.updateNodeCallback;

        this.keyName;
        this.parentKeyName;
        this.movingKey;
        this.newParentKey;
        this.deleteKey;
        this.editingParentKey;

        this.UnobtrusiveValidationRulesHiddenFieldPostfix = "_UVR";
    },
    PerformCallback: function(data) {
        MVCx.MergeHashTables(this.callbackCustomArgs, data);
        ASPxClientTreeList.prototype.PerformCallback.call(this, data);
    },
    PerformCustomDataCallback: function(data) {
        MVCx.MergeHashTables(this.callbackCustomArgs, data);
        ASPxClientTreeList.prototype.PerformCustomDataCallback.call(this, ASPx.Json.ToJson(this.callbackCustomArgs));
    },
    InlineInitialize: function() {
        if(this.callbackUrl != "")
            this.callBack = function(arg) { MVCx.PerformControlCallback(this.name, this.callbackUrl, arg, this.GetCallbackParams(arg), this.callbackCustomArgs) };
        if(this.customActionUrl != "")
            this.customActionCallBack = function(arg) { MVCx.PerformControlCallback(this.name, this.customActionUrl, arg, this.GetCallbackParams(arg), this.callbackCustomArgs) };
        if(this.customDataActionUrl != "")
            this.customDataActionCallback = function(arg) { MVCx.PerformControlCallback(this.name, this.customDataActionUrl, arg, this.GetCallbackParams(arg), this.callbackCustomArgs) };
        if(this.addNewNodeUrl != "")
            this.addNewNodeCallback = function(arg) { MVCx.PerformControlCallback(this.name, this.addNewNodeUrl, arg, this.GetCallbackParams(arg), this.callbackCustomArgs) };
        if(this.deleteNodeCallback != "")
            this.deleteNodeCallback = function(arg) { MVCx.PerformControlCallback(this.name, this.deleteNodeUrl, arg, this.GetCallbackParams(arg), this.callbackCustomArgs) };
        if(this.updateNodeUrl != "")
            this.updateNodeCallback = function(arg) { MVCx.PerformControlCallback(this.name, this.updateNodeUrl, arg, this.GetCallbackParams(arg), this.callbackCustomArgs) };
        if(this.moveNodeUrl != "")
            this.moveNodeCallback = function(arg) { MVCx.PerformControlCallback(this.name, this.moveNodeUrl, arg, this.GetCallbackParams(arg), this.callbackCustomArgs) };

        ASPxClientTreeList.prototype.InlineInitialize.call(this);
    },
    RaiseBeginCallbackInternal: function(command) {
        var args = new MVCxClientBeginCallbackEventArgs(command);
        if(!this.BeginCallback.IsEmpty())
            this.BeginCallback.FireEvent(this, args);
        MVCxClientGlobalEvents.OnBeginCallback(args);
        MVCx.MergeHashTables(this.callbackCustomArgs, args.customArgs);
    },
    RaiseEndCallback: function() {
        ASPxClientTreeList.prototype.RaiseEndCallback.call(this);
        MVCxClientGlobalEvents.OnEndCallback();
    },
    RaiseCallbackError: function(message) {
        var result = ASPxClientTreeList.prototype.RaiseCallbackError.call(this, message);
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
    EvalCallbackResult: function(resultString) {
        var resultStringParts = resultString.split(MVCx.CallbackHtmlContentPrefix);
        if(resultStringParts.length == 2) {
            var resultObj = ASPxClientTreeList.prototype.EvalCallbackResult.call(this, resultStringParts[0]);
            resultObj.result.data = resultStringParts[1];
            return resultObj;
        }
        return ASPxClientTreeList.prototype.EvalCallbackResult.call(this, resultString);
    },
    GetCallbackParams: function(arg) {
        var params = {};
        MVCx.AddCallbackParam(params, this.GetStateHiddenField());
        MVCx.AddCallbackParamsInContainer(params, this.GetMainElement());

        var editFields = [];
        var editors = this.GetEditorObjects();

        for(var i = 0; i < editors.length; i++) {
            var column = this.columns[this.GetEditorColumnIndex(editors[i])];
            var editorValue = ASPx.Json.ToJson(MVCx.GetEditorValueByControl(editors[i]));
            if (ASPx.IsExists(editorValue))
                params[column.fieldName] = editorValue;
            editFields.push(column.fieldName);
        }

        if(editFields.length > 0)
            params["DXMVCTreeListEditFields"] = ASPx.Json.ToJson(editFields);

        if(this.keyName != "") {
            if(this.deleteKey) {
                params[this.keyName] = this.deleteKey;
                this.deleteKey = null;
            }
            else if(this.IsEditing() && !this.isNewNodeEditing && !params[this.keyName])
                params[this.keyName] = this.GetEditingNodeKey();
            else if(this.movingKey) {
                params[this.keyName] = this.movingKey;
                this.movingKey = null;
                params[this.parentKeyName] = this.newParentKey;
                this.newParentKey = null;
            }
        }

        if(this.parentKeyName != "") {
            if(this.IsEditing() && !params[this.parentKeyName] && this.editingParentKey)
                params[this.parentKeyName] = this.editingParentKey;
        }

        MVCx.AddDXEditorValuesInContainer(params, this.GetMainElement());
        return params;
    },
    CreateCallbackCore: function(arg, command, callbackID) {
        if(this.callbackCustomArgs != {})
            window.setTimeout(function() { this.callbackCustomArgs = {}; } .aspxBind(this), 0);
        ASPxClientTreeList.prototype.CreateCallbackCore.call(this, arg, command, callbackID);
    },
    GetCallbackMethod: function(command) {
        if(MVCx.IsCustomCallback(command))
            return MVCx.GetCustomActionCallBackMethod(this);
        if(MVCx.IsCustomDataCallback(command))
            return this.customDataActionCallback;
        if(command == "DeleteNode")
            return this.deleteNodeCallback;
        if(command == "UpdateEdit")
            return this.isNewNodeEditing ? this.addNewNodeCallback : this.updateNodeCallback;
        if(command == "MoveNode")
            return this.moveNodeCallback;
        return this.callBack;
    },
    ProcessCallbackResult: function(resultObj) {
        ASPxClientTreeList.prototype.ProcessCallbackResult.call(this, resultObj);
        this.editingParentKey = resultObj.editingParentKey;
        this.errorText = resultObj.errorText;
    },
    DoEndCallback: function() {
        if(this.errorText) {
            var result = this.RaiseCallbackError(this.errorText);
            this.errorText = null;
            if(!result.isHandled)
                this.OnCallbackError(result.errorMessage);
        }
        ASPxClientTreeList.prototype.DoEndCallback.call(this);
    },
    MoveNode: function(nodeKey, parentNodeKey) {
        this.movingKey = nodeKey;
        this.newParentKey = parentNodeKey;
        ASPxClientTreeList.prototype.MoveNode.call(this, nodeKey, parentNodeKey);
    },
    DeleteNode: function(key) {
        this.deleteKey = key;
        ASPxClientTreeList.prototype.DeleteNode.call(this, key);
    },

    // Validation
    ValidateEditors: function(){
        var isValid = ASPxClientTreeList.prototype.ValidateEditors.call(this);
        var jQValidation = MVCx.JQueryValidation;
        if(jQValidation.IsEnabled(this)){
            this.AssignUnobtrusiveValidationRulesToEditors();
            jQValidation.PrepareUVRules(this);
            $.each(this.GetEditorObjects(), function(i, editor){
                isValid &= jQValidation.Validate(editor);
            });
            if(jQValidation.HasPendingRequests(this)){
                jQValidation.SetOnStopRequestHandler(this, this.UpdateEdit.aspxBind(this));
                return false;
            }
        }
        return isValid;
    },
    AssignUnobtrusiveValidationRulesToEditors: function(){
        var rules = this.unobtrusiveValidationRules;
        if(!ASPx.IsExists(rules))
            return;

        var editors = this.GetEditorObjects();
        var treeList = this;
        $.each(rules, function(fieldName, validationRule){
            var column = treeList.GetColumnByFieldName(fieldName);
            if(column && editors.length > column.index)
                MVCx.JQueryValidation.SetUVAttributes(editors[column.index], validationRule);
        });
    },
    OnCallbackFinalized: function() {
        MVCx.JQueryValidation.ResetUVRules(this);
        ASPxClientTreeList.prototype.OnCallbackFinalized.call(this);
    }
});
MVCxClientTreeList.Cast = ASPxClientControl.Cast;

window.MVCxClientTreeList = MVCxClientTreeList;
})();