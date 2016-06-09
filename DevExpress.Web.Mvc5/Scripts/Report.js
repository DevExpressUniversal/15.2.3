

(function(window) {
    var MVCxClientDocumentViewer = ASPx.CreateClass(ASPxClientDocumentViewer, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.callbackUrl = "";
            this.exportUrl = "";
            this.callbackCustomArgs = {};
            this.BeforeExportRequest = new ASPxClientEvent();
        },
        Initialize: function() {
            ASPxClientDocumentViewer.prototype.Initialize.call(this);
            this.viewer.setFormHelper(this.createFormHelper());
        },
        InlineInitialize: function() {
            if(this.callbackUrl != "") {
                this.callBack = function(arg) {
                    MVCx.PerformControlCallback(this.name, this.callbackUrl, arg, this.GetCallbackParams(), this.callbackCustomArgs);
                };
            }
            ASPxClientDocumentViewer.prototype.InlineInitialize.call(this);
        },
        GetCallbackParams: function() {
            var result = {};
            MVCx.AddCallbackParam(result, this.viewer.GetStateHiddenField());
            MVCx.MergeHashTables(result, this.collectParameterPanelEditorsParams());
            return result;
        },
        getHiddenFields: function() {
            return ASPxClientDocumentViewerReportViewer.prototype.getHiddenFields.call(this.viewer);
        },
        CreateCallbackByInfo: function(arg, command, callbackInfo) {
            this.CreateCallbackInternal(arg, command, true, callbackInfo);
        },
        CreateCallbackCore: function(arg, command, callbackID) {
            if(this.callbackCustomArgs != {})
                window.setTimeout(function() { this.callbackCustomArgs = {}; }.aspxBind(this), 0);
            ASPxClientDocumentViewer.prototype.CreateCallbackCore.call(this, arg, command, callbackID);
        },
        GetCallbackMethod: function(command) {
            return MVCx.IsCustomCallback(command) ? MVCx.GetCustomActionCallBackMethod(this) : this.callBack;
        },
        createFormHelper: function() {
            var helper = ASPxClientReportViewer.__useMobileSpecificExport
                ? new FormHelperMobile(this.exportUrl, this.getHiddenFields(), this.viewer.uniqueID)
                : new FormHelper(this.exportUrl, this.getHiddenFields(), this.viewer.uniqueID);
            helper.BeforeSendPostback.AddHandler(function(_, e) {
                var args = new MVCxClientBeforeExportRequestEventArgs();
                this.BeforeExportRequest.FireEvent(this, args);
                helper.customArgs = args.customArgs;
            }.aspxBind(this));
            return helper;
        },
        RaiseBeginCallbackInternal: function(command) {
            var args = new MVCxClientBeginCallbackEventArgs(command);
            if(!this.BeginCallback.IsEmpty())
                this.BeginCallback.FireEvent(this, args);
            MVCxClientGlobalEvents.OnBeginCallback(args);
            this.callbackCustomArgs = args.customArgs;
        },
        RaiseEndCallback: function() {
            ASPxClientDocumentViewer.prototype.RaiseEndCallback.call(this);
            MVCxClientGlobalEvents.OnEndCallback();
        },
        RaiseCallbackError: function(message) {
            var result = ASPxClientReportViewer.prototype.RaiseCallbackError.call(this, message);
            if(!result.isHandled) {
                var args = new ASPxClientCallbackErrorEventArgs(message);
                MVCxClientGlobalEvents.OnCallbackError(args);
                result = { isHandled: args.handled, errorMessage: args.message };
            }
            return result;
        },
        collectParameterPanelEditorsParams: function() {
            if(!ASPx.IsExists(this.parametersPanel)) {
                return {};
            }
            var result = {};
            var editors = this.parametersPanel.collectEditors();
            $(editors).each(function(i, v) {
                if(v["parameterName"]) {
                    result[v["parameterName"]] = v["value"];
                } else {
                    $(v.GetMainElement()).find("input,textarea,select").each(function() {
                        result[this.name] = this.value;
                    });
                }
            });
            return result;
        }
    });

    MVCxClientDocumentViewer.arrayAny = function(arr, condition) {
        for(var i = 0; i < arr.length; i++) {
            if(condition(arr[i])) {
                return true;
            }
        }
        return false;
    };
    var MVCxClientReportViewer = ASPx.CreateClass(ASPxClientReportViewer, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.callbackUrl = "";
            this.exportUrl = "";
            this.callbackCustomArgs = {};
            this.BeforeExportRequest = new ASPxClientEvent();
        },

        InlineInitialize: function() {
            if(this.callbackUrl != "") {
                this.callBack = function(arg) { MVCx.PerformControlCallback(this.name, this.callbackUrl, arg, this.GetCallbackParams(), this.callbackCustomArgs) };
            }
            ASPxClientReportViewer.prototype.InlineInitialize.call(this);
        },
        GetCallbackParams: function() {
            var params = {};
            MVCx.AddCallbackParam(params, this.GetStateHiddenField());
            return params;
        },
        CreateCallbackByInfo: function(arg, command, callbackInfo) {
            this.CreateCallbackInternal(arg, command, true, callbackInfo);
        },
        createFormHelper: function() {
            return ASPxClientReportViewer.__useMobileSpecificExport
                ? new FormHelperMobile(this.exportUrl, this.getHiddenFields(), this.name)
                : new FormHelper(this.exportUrl, this.getHiddenFields(), this.name);
        },
        RaiseBeginCallbackInternal: function(command) {
            var args = new MVCxClientBeginCallbackEventArgs(command);
            if(!this.BeginCallback.IsEmpty())
                this.BeginCallback.FireEvent(this, args);
            MVCxClientGlobalEvents.OnBeginCallback(args);
            this.callbackCustomArgs = args.customArgs;
        },
        RaiseEndCallback: function() {
            ASPxClientReportViewer.prototype.RaiseEndCallback.call(this);
            MVCxClientGlobalEvents.OnEndCallback();
        },
        RaiseCallbackError: function(message) {
            var result = ASPxClientReportViewer.prototype.RaiseCallbackError.call(this, message);
            if(!result.isHandled) {
                var args = new ASPxClientCallbackErrorEventArgs(message);
                MVCxClientGlobalEvents.OnCallbackError(args);
                result = { isHandled: args.handled, errorMessage: args.message };
            }
            return result;
        },
        RaiseBeforeExportRequestEvent: function() {
            if(!this.BeforeExportRequest.IsEmpty()) {
                var args = new MVCxClientBeforeExportRequestEventArgs();
                this.BeforeExportRequest.FireEvent(this, args);
                this.formHelper.customArgs = args.customArgs;
            }
        },
        execExport: function(exportKind, params, win) {
            this.RaiseBeforeExportRequestEvent();
            ASPxClientReportViewer.prototype.execExport.call(this, exportKind, params, win);
        },
        subscribeToAspForm: function() {
            // pass
        }
    });
    var MVCxClientBeforeExportRequestEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function() {
            this.constructor.prototype.constructor.call(this);
            this.customArgs = {};
        }
    });

    var FormHelper = ASPx.CreateClass(ASPx.dx_FormHelper, {
        constructor: function(action, hiddenFields, viewerName) {
            this.action = action;
            this.viewerName = viewerName;
            this.argumentFieldName = "DXArgument";
            this.stateFieldName = "DXRVState";
            this.hiddenFields = hiddenFields;
            this.hiddenFields.push({ tagName: "input", type: "hidden", id: this.argumentFieldName, name: this.argumentFieldName });
            this.customArgs = {};
            this.BeforeSendPostback = new ASPxClientEvent();
        },
        getTheForm: function() {
            return {
                method: "POST",
                action: this.action,
                id: "form1",
                elements: this.getElements(),
                name: ""
            };
        },
        getElements: function(originalName) {
            var elements = [];
            for(var i = 0; i < this.hiddenFields.length; i++) {
                elements.push(this.hiddenFields[i]);
            }
            for(var arg in this.customArgs) {
                if(this.customArgs.hasOwnProperty(arg)) {
                    elements.push({ tagName: 'input', type: 'hidden', id: arg, name: arg, value: this.customArgs[arg] });
                }
            }
            return elements;
        },
        getElementName: function(originalName) {
            if(originalName === this.viewerName) {
                return this.stateFieldName;
            }
            return originalName;
        },
        sendPostbackInWindow: function(win, eventTarget, eventArgument) {
            this.BeforeSendPostback.FireEvent(this);
            ASPx.dx_FormHelper.prototype.sendPostbackInWindow.call(this, win, eventTarget, eventArgument);
        }
    });

    var FormHelperMobile = ASPx.CreateClass(FormHelper, {
        constructor: function(action, hiddenFields, viewerName) {
            this.constructor.prototype.constructor.call(this, action, hiddenFields, viewerName);
        },
        sendPostbackInWindow: function(_, eventTarget, eventArgument) {
            this.BeforeSendPostback.FireEvent(this);
            this.eventTarget = eventTarget;
            this.eventArgument = eventArgument;
            this.inputValues = {};
            this.inputUniqueIdCounter = 1;

            var form = this.forceRecreateForm();
            this.applyInputValues(form);
            form.submit();
        },
        forceRecreateForm: function() {
            var theForm = this.getTheForm();
            var id = "DXXRMobileSpecificExportForm";

            var prevForm = document.getElementById(id);
            if(prevForm) {
                var $parentElement = $(prevForm.parentElement);
                $parentElement.remove();
            }

            var formAttributes = {
                id: id,
                method: theForm.method,
                action: this.getCorectedAction(this.action)
            };
            var container = document.createElement('div');
            container.style.overflow = 'hidden';
            container.style.width = '0px';
            container.style.height = '0px';
            container.innerHTML = this.buildTag('form', formAttributes, this.getInputsAndInitInputValues() + "<input type='submit' value='submit'/>");
            document.body.appendChild(container);
            return document.getElementById(id);
        },
        applyInputValues: function(form) {
            for(var inputId in this.inputValues) {
                try {
                    var input = form.querySelector("#" + inputId);
                    input && (input.value = this.inputValues[inputId]);
                } catch(e) {
                }
            }
        }
    });

    window.MVCxClientDocumentViewer = MVCxClientDocumentViewer;
    window.MVCxClientReportViewer = MVCxClientReportViewer;
    window.MVCxClientBeforeExportRequestEventArgs = MVCxClientBeforeExportRequestEventArgs;
})(window);