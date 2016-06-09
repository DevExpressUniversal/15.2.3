

(function() {
var MVCxClientGridView = ASPx.CreateClass(ASPxClientGridView, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.callbackUrl = "";
        this.customActionUrl = "";
        this.addNewItemUrl = "";
        this.updateItemUrl = "";
        this.deleteItemUrl = "";
        this.batchUpdateUrl = "";
        this.callbackCustomArgs = {};
        this.callbackActionUrlCollection = {};

        this.customActionCallBack;
        this.addNewItemCallBack;
        this.updateItemCallBack;
        this.deleteItemCallBack;
        this.batchUpdateCallBack;
        this.callbackMethods = {};
        this.funcCallbackMethod;

        this.deleteKeyValue;

        this.gridAdapter = new MVCx.GridAdapter(this);
    },
    PerformCallback: function(data){
        MVCx.MergeHashTables(this.callbackCustomArgs, data);
        ASPxClientGridView.prototype.PerformCallback.call(this, data);
    },
    GetValuesOnCustomCallback: function(data, onCallback) {
        this.funcCallbackMethod = this.callbackMethods[ASPxClientGridViewCallbackCommand.CustomValues];
        MVCx.MergeHashTables(this.callbackCustomArgs, data);
        ASPxClientGridView.prototype.GetValuesOnCustomCallback.call(this, data, onCallback);
    },
    InlineInitialize: function(){
        this.gridAdapter.InlineInitialize();
        ASPxClientGridView.prototype.InlineInitialize.call(this);
    },
    RaiseBeginCallbackInternal: function(command){
        var args = new MVCxClientBeginCallbackEventArgs(command);
        if(!this.BeginCallback.IsEmpty())
            this.BeginCallback.FireEvent(this, args);
        MVCxClientGlobalEvents.OnBeginCallback(args);
        MVCx.MergeHashTables(this.callbackCustomArgs, args.customArgs);
    },
    RaiseEndCallback: function() {
        ASPxClientGridView.prototype.RaiseEndCallback.call(this);
        MVCxClientGlobalEvents.OnEndCallback();
    },
    RaiseCallbackError: function(message) {
        var result = ASPxClientGridView.prototype.RaiseCallbackError.call(this, message);
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
    GetSortingCallbackParamsMethod: function(arg){
        var param = this.GetCallbackParams(arg);
        return param;
    },
    GetCallbackParams: function(arg) {
        return this.gridAdapter.GetCallbackParams();
    },
    EvalCallbackResult: function(resultString){
        var resultStringParts = resultString.split(MVCx.CallbackHtmlContentPrefix);
        if(resultStringParts.length == 2){
            var resultObj = ASPxClientGridView.prototype.EvalCallbackResult.call(this, resultStringParts[0]);
            resultObj.result.html = resultStringParts[1];
            return resultObj;
        }
        return ASPxClientGridView.prototype.EvalCallbackResult.call(this, resultString);
    },
    CreateCallbackCore: function(arg, command, callbackID){
        if(this.callbackCustomArgs != {})
            window.setTimeout(function(){ this.callbackCustomArgs = {}; }.aspxBind(this), 0);
        if(this.funcCallbackMethod)
            window.setTimeout(function() { this.funcCallbackMethod = null; }.aspxBind(this), 0);
        ASPxClientGridView.prototype.CreateCallbackCore.call(this, arg, command, callbackID);
    },
    GetCallbackMethod: function(command) {
        return this.gridAdapter.GetCallbackMethod(command);
    },
    DeleteItemByKey: function(key) {
        this.deleteKeyValue = key;
        ASPxClientGridView.prototype.DeleteItemByKey.call(this, key);
    },
    gridCallBack: function(args) {
        this.gridAdapter.GridCallBack(args);
        ASPxClientGridView.prototype.gridCallBack.call(this, args);
    },
    OnAfterCallback: function() {
        this.gridAdapter.OnAfterCallback();
        ASPxClientGridView.prototype.OnAfterCallback.call(this);
    },
    // Validation
    _validateEditors: function () {
        return ASPxClientGridView.prototype._validateEditors.call(this) && this.gridAdapter.ValidateEditorsByJQuery();
    },

    CreateBatchEditHelper: function(){
        this.InitializeBatchEditHelperClass();
        return new MVCxClientGridViewBatchEditHelper(this, new MVCx.GridBatchEditHelperAdapter());
    },
    InitializeBatchEditHelperClass: function(){
        if(typeof(MVCxClientGridViewBatchEditHelper) != 'undefined') return;

        MVCxClientGridViewBatchEditHelper = ASPx.CreateClass(ASPx.GridViewBatchEditHelper, {
            constructor: function(grid, batchEditHelperAdapter) { 
                this.constructor.prototype.constructor.call(this, grid);
                this.batchEditHelperAdapter = batchEditHelperAdapter;
                this.batchEditHelperAdapter.Init(this);
            },

            Init: function(){
                ASPx.GridViewBatchEditHelper.prototype.Init.call(this);
                this.batchEditHelperAdapter.PrepareEditorsValidation();
            },
            OnAfterCallback: function(){
                ASPx.GridViewBatchEditHelper.prototype.OnAfterCallback.call(this);
                this.batchEditHelperAdapter.PrepareEditorsValidation();
            },
            
            GetEndEditValidationInfo: function(itemValues, skipValidation) {
                var info = ASPx.GridViewBatchEditHelper.prototype.GetEndEditValidationInfo.call(this, itemValues, skipValidation);
                if(!this.batchEditHelperAdapter.CanEndEdit())
                    info.allowEndEdit = false;
                return info;
            },
            EndEdit: function(skipValidation) {
                return this.batchEditHelperAdapter.SafeCallEndEdit(
                    function(skipValidation) { return ASPx.GridViewBatchEditHelper.prototype.EndEdit.call(this, skipValidation); }.aspxBind(this)
                );
            },
            ValidateEditor: function(editor){
                ASPx.GridViewBatchEditHelper.prototype.ValidateEditor.call(this, editor);
                this.batchEditHelperAdapter.ValidateEditor(editor);
            }
        });
    }
});
MVCxClientGridView.Cast = ASPxClientControl.Cast;
window.MVCxClientGridView = MVCxClientGridView;
})();