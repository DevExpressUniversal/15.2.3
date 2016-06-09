
(function() {
    var MVCxClientFileManager = ASPx.CreateClass(ASPxClientFileManager, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.callbackUrl = "";
            this.downloadUrl = "";
            this.customActionUrl = "";
            this.callbackCustomArgs = {};
        },
        PerformCallback: function(data){
            MVCx.MergeHashTables(this.callbackCustomArgs, data);
            ASPxClientFileManager.prototype.PerformCallback.call(this, data);
        },

        InlineInitialize: function() {
            if(this.callbackUrl != "")
                this.callBack = function(arg) { MVCx.PerformControlCallback(this.name, this.callbackUrl, arg, this.GetCallbackParams(arg), this.callbackCustomArgs) };
            if (this.customActionUrl != "")
                this.customActionCallBack = function(arg){ MVCx.PerformControlCallback(this.name, this.customActionUrl, arg, this.GetCallbackParams(arg), this.callbackCustomArgs) };
            ASPxClientFileManager.prototype.InlineInitialize.call(this);
        },

        RaiseBeginCallbackInternal: function(command) {
            var args = new MVCxClientBeginCallbackEventArgs(command);
            if(!this.BeginCallback.IsEmpty())
                this.BeginCallback.FireEvent(this, args);
            MVCxClientGlobalEvents.OnBeginCallback(args);
            MVCx.MergeHashTables(this.callbackCustomArgs, args.customArgs);
        },
        RaiseEndCallback: function() {
            ASPxClientFileManager.prototype.RaiseEndCallback.call(this);
            MVCxClientGlobalEvents.OnEndCallback();
        },
        RaiseCallbackError: function(message) {
            var result = ASPxClientFileManager.prototype.RaiseCallbackError.call(this, message);
            if(!result.isHandled) {
                var args = new ASPxClientCallbackErrorEventArgs(message);
                MVCxClientGlobalEvents.OnCallbackError(args);
                result = { isHandled: args.handled, errorMessage: args.message };
            }
            return result;
        },
        GetCallbackParams: function(arg) {
            var params = {};
            MVCx.AddCallbackParam(params, this.GetStateHiddenField());
            MVCx.AddCallbackParamsInContainer(params, this.GetMainElement());

            var treeView = this.GetTreeView();
            if(treeView)
                MVCx.AddCallbackParam(params, treeView.GetStateHiddenField());

            var folderBrowser = this.GetFolderBrowserTreeView();
            if(folderBrowser && folderBrowser.GetStateHiddenField()) 
                MVCx.AddCallbackParam(params, folderBrowser.GetStateHiddenField());

            if(this.viewMode == ASPx.FileManagerConsts.ViewMode.Grid) 
                MVCx.AddCallbackParam(params, this.GetFilesGridView().GetStateHiddenField());

            return params;
        },
        CreateCallbackByInfo: function(arg, command, callbackInfo) {
            this.CreateCallbackInternal(arg, command, true, callbackInfo);
        },
        Download: function() {
            if(!this.downloadUrl)
                return;
            ASPxClientFileManager.prototype.Download.call(this);
        },
        SendPostBack: function(postBackArg) {
            this.OnPost();
            var form = ASPx.GetParentByTagName(this.GetMainElement(), "form");
            if(form) {
                var sourceFormAction = form.action;
                var prefix = this.downloadUrl.indexOf("?") >= 0 ? "&" : "?";
                form.action = this.downloadUrl + prefix + "DXMVCFileManagerDownloadArgument=" + encodeURIComponent(postBackArg);
                form.submit();
                form.action = sourceFormAction;
            }
        },
        EvalCallbackResult: function (resultString) {
            var resultStringParts = resultString.split(MVCx.CallbackHtmlContentPrefix);
            if(resultStringParts.length == 2) {
                var resultObj = ASPxClientFileManager.prototype.EvalCallbackResult.call(this, resultStringParts[0]);
                resultObj.result.itemsRender = resultStringParts[1];
                return resultObj;
            }
            return ASPxClientFileManager.prototype.EvalCallbackResult.call(this, resultString);
        },
        CreateCallbackCore: function(arg, command, callbackID){
            if(this.callbackCustomArgs != {})
                window.setTimeout(function(){ this.callbackCustomArgs = {}; }.aspxBind(this), 0);
            ASPxClientFileManager.prototype.CreateCallbackCore.call(this, arg, command, callbackID);
        },
        GetCallbackMethod: function(command){
            return MVCx.IsCustomCallback(command) ? MVCx.GetCustomActionCallBackMethod(this) : this.callBack;
        }
    });
    MVCxClientFileManager.Cast = ASPxClientControl.Cast;

    var MVCxLegacyUploadManager = ASPx.CreateClass(ASPxClientUploadControl.UploadManagerClass, {
        GetUploadFormAction: function(form) {
            if(this.options.callbackUrl != "")
                form.action = this.options.callbackUrl;
            return ASPxClientUploadControl.UploadManagerClass.prototype.GetUploadFormAction.call(this, form);
        }
    });

    var FileManagerUploadControl = ASPx.CreateClass(ASPx.FileManagerUploadControl, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.callbackUrl = "";
        },
        createUploadManager: function() {
            this.options.callbackUrl = this.callbackUrl;
            return new MVCxLegacyUploadManager(this.options);
        }
    });

    window.MVCxClientFileManager = MVCxClientFileManager;

    MVCx.FileManagerUploadControl = FileManagerUploadControl;
})();