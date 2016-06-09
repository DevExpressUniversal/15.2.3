/// <reference path="ReportViewer.js"/>

(function (window) {
    function getEditorValue(editor) {
        var value = editor.cpReportParametersPanel_MultiValue
            ? editor.GetTokenValuesCollection()
            : editor.GetValue();
        if (editor.cpReportParametersPanel_DateTimeLookup) { // waiting for T130963
            if (editor.cpReportParametersPanel_MultiValue) {
                var valueLength = value.length;
                for (var i = 0; i < valueLength; i++) {
                    value[i] = eval(value[i]);
                }
            } else {
                value = eval(value);
            }
        }
        return value;
    }

    function setEditorValue(editor, value) {
        var supportUpdate = editor.BeginUpdate && editor.EndUpdate;
        if (supportUpdate)
            editor.BeginUpdate();
        if(editor.cpReportParametersPanel_MultiValue) {
            ASPxClientReportParametersPanel.resetMultiValueEditor(editor, value);
        } else {
            editor.SetValue(value);
        }
        if (supportUpdate)
            editor.EndUpdate();
    }
    var ASPxClientReportParametersPanel = ASPx.CreateClass(ASPxClientControl, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.parameters = {};
            this.editorValueChangedHandler = null;
            this.resetingCascadeParameters = false;
            this.cascadeLookupValuesRequest = new ASPxClientEvent();
        },
        InlineInitialize: function() {
            ASPxClientControl.prototype.InlineInitialize.call(this);

            var controlCollection = ASPx.GetControlCollection();
            var self = this;

            this.resetButton = controlCollection.Get(this.name + "_Reset");
            this.resetButton.Click.AddHandler(function() { self.resetButtonClick(); });

            this.submitButton = controlCollection.Get(this.name + "_Submit");
            this.submitButton.Click.AddHandler(function() { self.submitButtonClick(); });

            this.editorValueChangedHandler = function(s) {
                self.editorValueChanged(s);
            }
            var parameters = this.parameters;
            for(var i in parameters) {
                if(!parameters.hasOwnProperty(i)) {
                    continue;
                }
                var parameter = parameters[i];
                if(!parameter.editorId) {
                    continue;
                }
                var editor = controlCollection.Get(parameter.editorId);
                editor.ValueChanged.AddHandler(this.editorValueChangedHandler);
                if(parameter.useCascadeLookup) {
                    this.patchAutoCompleteBoxForCascadeLookupSupport(editor);
                }
                if(editor.cpReportParametersPanel_MultiValue) {
                    editor.TokensChanged.AddHandler(function(s, e) { self.onTokenParameterChanged(s); });
                }
                parameter.editor = editor;
            }
        },
        Initialize: function() {
            this.constructor.prototype.Initialize.call(this);

            if(this.reportViewerId) {
                this.reportViewer = ASPx.GetControlCollection().Get(this.reportViewerId);
            }
        },
        AssignParameters: function(parametersInfo) {
            var len = parametersInfo.length;
            for(var i = 0; i < len; i++) {
                var parameterInfo = parametersInfo[i];
                var path = parameterInfo.Path || parameterInfo.path;
                var value = parameterInfo.Value || parameterInfo.value;
                this.assignParameterCore(path, value);
            }
            this.resetButtonClick();
        },
        AssignParameter: function(path, value) {
            this.assignParameterCore(path, value);
            this.resetButtonClick();
        },
        GetParameterNames: function() {
            var result = [];
            var parameters = this.parameters;
            for(var i in parameters) {
                if(parameters.hasOwnProperty(i)) {
                    result.push(i);
                }
            }
            return result;
        },
        GetEditorByParameterName: function(parameterName) {
            var parameters = this.parameters;
            return parameters.hasOwnProperty(parameterName) && parameters[parameterName].editor;
        },
        editorValueChanged: function(currentEditor) {
            !this.resetingCascadeParameters && this.resetButton.SetEnabled(true);
            var cascadeLookupValues = this.collectCascadeLookupValues(currentEditor);
            if(cascadeLookupValues) {
                this.cascadeLookupValuesRequest.FireEvent(this, cascadeLookupValues);
            }
        },
        assignParameterCore: function(path, value) {
            var parameters = this.parameters;
            if(!parameters[path]) {
                throw new Error("there is no such parameter");
            }
            parameters[path].value = value;
        },
        resetButtonClick: function() {
            if(!this.reportViewer) {
                return;
            }
            var parameters = this.parameters;
            var firstEditor;
            for(var path in parameters) {
                var parameter = parameters[path];
                if (!ASPx.IsExists(parameter)) {
                    continue;
                }
                var editor = parameter.editor;
                if(editor) {
                    !firstEditor && (firstEditor = editor);
                    setEditorValue(editor, parameter.value);
                }
            }
            var cascadeParamsWithLookupValues = this.collectCascadeLookupValues(firstEditor);
            if(cascadeParamsWithLookupValues) {
                this.resetingCascadeParameters = true;
                this.cascadeLookupValuesRequest.FireEvent(this, cascadeParamsWithLookupValues);
            }
            this.resetButton.SetEnabled(false);
            ASPxClientEdit.ValidateEditorsInContainer(this.GetMainElement());
        },
        submitButtonClick: function() {
            if(!this.reportViewer || !ASPxClientEdit.ValidateEditorsInContainer(this.GetMainElement())) {
                return;
            }
            var reportViewerParameters = {};
            var parameters = this.parameters;
            for(var path in parameters) {
                var parameter = parameters[path];
                if (!ASPx.IsExists(parameter)) {
                    continue;
                }
                var value = !parameter.editor ? parameter.value : getEditorValue(parameter.editor);
                parameter.value = value;
                reportViewerParameters[path] = value;
            }
            this.resetButton.SetEnabled(false);
            this.reportViewer.SubmitParameters(reportViewerParameters);
        },
        patchAutoCompleteBoxForCascadeLookupSupport: function(autoCompleteBox) {
            autoCompleteBox.dxxrdv_callbackEvent = new ASPxClientEvent();
            autoCompleteBox.CreateCallback = function(arg) {
                autoCompleteBox.dxxrdv_callbackEvent.FireEvent(autoCompleteBox, arg);
            };
        },
        collectCascadeLookupValues: function(currentEditor) {
            var currentEditorFounded = false;
            var cascadeLookupValues = {};
            var hasCallbackEventArguments = false;
            for(var i in this.parameters) {
                var parameter = this.parameters[i];
                if(!ASPx.IsExists(parameter)) {
                    continue;
                }
                var editor = parameter.editor;
                var value = !editor ? parameter.value : getEditorValue(editor);
                var cascadeLookupValue = {
                    value: value
                };
                if(!currentEditorFounded && editor === currentEditor) {
                    currentEditorFounded = true;
                } else if(currentEditorFounded && parameter.useCascadeLookup) {
                    var onCallbackEvent = function(_, callbackEventArgument) {
                        cascadeLookupValue.callbackEventArgument = callbackEventArgument;
                    };
                    editor.dxxrdv_callbackEvent.AddHandler(onCallbackEvent);
                    editor.PerformCallback();
                    editor.dxxrdv_callbackEvent.RemoveHandler(onCallbackEvent);
                    hasCallbackEventArguments = true;
                }
                cascadeLookupValues[i] = cascadeLookupValue;
            }
            return hasCallbackEventArguments ? cascadeLookupValues : null;
        },
        onCascadeLookupsCallbackCore: function(responses) {
            for(var i in responses) {
                var response = responses[i];
                var parameter = this.parameters[i];
                if(!ASPx.IsExists(response) || !ASPx.IsExists(parameter)) {
                    continue;
                }
                var editor = parameter.editor;
                if(!editor) {
                    continue;
                }
                editor.ValueChanged.RemoveHandler(this.editorValueChangedHandler);
                editor.DoCallback(response);
                editor.ValueChanged.AddHandler(this.editorValueChangedHandler);
            }
            this.resetingCascadeParameters = false;
        },
        collectEditors: function() {
            var result = [];
            var parameters = this.parameters;
            for(var i in parameters) {
                if(!parameters.hasOwnProperty(i)) {
                    continue;
                }
                var parameter = parameters[i];
                if(!parameter.editor) {
                    result.push({ parameterName: i, value: parameter.value });
                } else {
                    result.push(parameter.editor);
                }
            }
            return result;
        },
        onTokenParameterChanged: function(tokenBoxEditor) {
            var tokens = tokenBoxEditor.GetTokenCollection();
            for(var i = 0; i < tokens.length; i++) {
                var tokenText = tokens[i];
                var isFound = !!tokenBoxEditor.FindItemByText(tokenText);
                if(!isFound)
                    tokenBoxEditor.AddItem(tokenText, tokenText);
            }
        }
    });
    ASPxClientReportParametersPanel.resetMultiValueEditor = function(tokenbox, values) {
        tokenbox.ClearTokenCollection();
        var valuesLength = values.length;
        for(var i = 0; i < valuesLength; i++) {
            var value = values[i];
            var item = tokenbox.FindItemByValue(value);
            if(item) {
                tokenbox.AddToken(item.text);
            }
        }
    };
    var ASPxClientReportParameterInfo = ASPx.CreateClass(null, {
        constructor: function(path, value) {
            this.Path = path;
            this.Value = value;
        }
    });

    window.ASPxClientReportParametersPanel = ASPxClientReportParametersPanel;
    window.ASPxClientReportParameterInfo = ASPxClientReportParameterInfo;
})(window);