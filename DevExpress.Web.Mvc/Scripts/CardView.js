

(function () {
    var MVCxClientCardView = ASPx.CreateClass(ASPxClientCardView, {
        constructor: function (name) {
            this.constructor.prototype.constructor.call(this, name);
            this.callbackUrl = "";
            this.customActionUrl = "";
            this.callbackCustomArgs = {};
            this.addNewItemUrl = "";
            this.updateItemUrl = "";
            this.deleteItemUrl = "";
            this.callbackActionUrlCollection = {};

            this.addNewItemCallBack;
            this.updateItemCallBack;
            this.deleteItemCallBack;
            this.callbackMethods = {};
            this.deleteKeyValue;
            this.funcCallbackMethod;

            this.gridAdapter = new MVCx.GridAdapter(this);
        },
        PerformCallback: function (data) {
            MVCx.MergeHashTables(this.callbackCustomArgs, data);
            ASPxClientCardView.prototype.PerformCallback.call(this, data);
        },
        GetValuesOnCustomCallback: function(data, onCallback) {
            this.funcCallbackMethod = this.callbackMethods[ASPxClientGridViewCallbackCommand.CustomValues];
            MVCx.MergeHashTables(this.callbackCustomArgs, data);
            ASPxClientCardView.prototype.GetValuesOnCustomCallback.call(this, data, onCallback);
        },
        InlineInitialize: function(){
            this.gridAdapter.InlineInitialize();
            ASPxClientCardView.prototype.InlineInitialize.call(this);
        },
        RaiseBeginCallbackInternal: function (command) {
            var args = new MVCxClientBeginCallbackEventArgs(command);
            if (!this.BeginCallback.IsEmpty())
                this.BeginCallback.FireEvent(this, args);
            MVCxClientGlobalEvents.OnBeginCallback(args);
            MVCx.MergeHashTables(this.callbackCustomArgs, args.customArgs);
        },
        RaiseEndCallback: function () {
            ASPxClientCardView.prototype.RaiseEndCallback.call(this);
            MVCxClientGlobalEvents.OnEndCallback();
        },
        RaiseCallbackError: function (message) {
            var result = ASPxClientCardView.prototype.RaiseCallbackError.call(this, message);
            if (!result.isHandled) {
                var args = new ASPxClientCallbackErrorEventArgs(message);
                MVCxClientGlobalEvents.OnCallbackError(args);
                result = { isHandled: args.handled, errorMessage: args.message };
            }
            return result;
        },
        CreateCallbackByInfo: function (arg, command, callbackInfo) {
            this.CreateCallbackInternal(arg, command, true, callbackInfo);
        },
        GetCallbackParams: function (arg) {
            return this.gridAdapter.GetCallbackParams();
        },
        CreateCallbackCore: function (arg, command, callbackID) {
            if (this.callbackCustomArgs != {})
                window.setTimeout(function () { this.callbackCustomArgs = {}; } .aspxBind(this), 0);
            if (this.funcCallbackMethod)
                window.setTimeout(function () { this.funcCallbackMethod = null; } .aspxBind(this), 0);
            ASPxClientCardView.prototype.CreateCallbackCore.call(this, arg, command, callbackID);
        },
        GetCallbackMethod: function(command) {
            return this.gridAdapter.GetCallbackMethod(command);
        },
        EvalCallbackResult: function (resultString) {
            var resultStringParts = resultString.split(MVCx.CallbackHtmlContentPrefix);
            if (resultStringParts.length == 2) {
                var resultObj = ASPxClientCardView.prototype.EvalCallbackResult.call(this, resultStringParts[0]);
                resultObj.result.html = resultStringParts[1];
                return resultObj;
            }
            return ASPxClientCardView.prototype.EvalCallbackResult.call(this, resultString);
        },
        DeleteItemByKey: function(key) {
            this.deleteKeyValue = key;
            ASPxClientCardView.prototype.DeleteItemByKey.call(this, key);
        },
        gridCallBack: function(args){
            this.gridAdapter.GridCallBack(args);
            ASPxClientCardView.prototype.gridCallBack.call(this, args);
        },
        OnAfterCallback: function() {
            this.gridAdapter.OnAfterCallback();
            ASPxClientCardView.prototype.OnAfterCallback.call(this);
        },
        _validateEditors: function () {
            return ASPxClientCardView.prototype._validateEditors.call(this) && this.gridAdapter.ValidateEditorsByJQuery();
        },
        CreateBatchEditHelper: function(){
            this.InitializeBatchEditHelperClass();
            return new MVCxClientCardViewBatchEditHelper(this, new MVCx.GridBatchEditHelperAdapter());
        },
        InitializeBatchEditHelperClass: function(){
            if(typeof(MVCxClientCardViewBatchEditHelper) != 'undefined') return;

            MVCxClientCardViewBatchEditHelper = ASPx.CreateClass(ASPx.CardViewBatchEditHelper, {
                constructor: function(grid, batchEditHelperAdapter) { 
                    this.constructor.prototype.constructor.call(this, grid);
                    this.batchEditHelperAdapter = batchEditHelperAdapter;
                    this.batchEditHelperAdapter.Init(this);
                },

                Init: function(){
                    ASPx.CardViewBatchEditHelper.prototype.Init.call(this);
                    this.batchEditHelperAdapter.PrepareEditorsValidation();
                },
                OnAfterCallback: function(){
                    ASPx.CardViewBatchEditHelper.prototype.OnAfterCallback.call(this);
                    this.batchEditHelperAdapter.PrepareEditorsValidation();
                },
            
                GetEndEditValidationInfo: function(itemValues, skipValidation) {
                    var info = ASPx.CardViewBatchEditHelper.prototype.GetEndEditValidationInfo.call(this, itemValues, skipValidation);
                    if(!this.batchEditHelperAdapter.CanEndEdit())
                        info.allowEndEdit = false;
                    return info;
                },

                EndEdit: function(skipValidation) {
                    return this.batchEditHelperAdapter.SafeCallEndEdit(
                        function(skipValidation) { return ASPx.CardViewBatchEditHelper.prototype.EndEdit.call(this, skipValidation); }.aspxBind(this)
                    );
                },
                ValidateEditor: function(editor){
                    ASPx.CardViewBatchEditHelper.prototype.ValidateEditor.call(this, editor);
                    this.batchEditHelperAdapter.ValidateEditor(editor);
                }
            });
        }
    });
    MVCxClientCardView.Cast = ASPxClientControl.Cast;
    window.MVCxClientCardView = MVCxClientCardView;
})();