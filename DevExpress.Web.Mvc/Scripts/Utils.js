

var MVCx = {};
(function() {
MVCx.CallbackHtmlContentPrefix = "/*DXHTML*/";
MVCx.CallbackHtmlContentPlaceholder = "<%html%>";
MVCx.EditorsValuesKey = "DXMVCEditorsValues";

MVCx.PerformControlCallback = function(name, url, arg, params, customParams) {
    if(ASPx.GetPostHandler().cancelPostProcessing)
        return;
    var data = {};
    data.DXCallbackName = name;
    data.DXCallbackArgument = arg;
        
    MVCx.MergeHashTables(data, params);
    MVCx.MergeHashTables(data, customParams);
    
    $.ajax({
        type: 'POST',
        url: url,
        dataType: 'html',
        data: data,

        error: function(response) {
            var ctrl = ASPx.GetControlCollection().Get(data.DXCallbackName);
            if (ctrl != null) {
                if(typeof(response.responseText) == "string" && response.responseText != "")
                    ctrl.DoCallbackError(response.responseText);
                else if(typeof(response) == "string" && response != "")
                    ctrl.DoCallbackError(response);
            }
        },
        success: function(response) {
            var ctrl = ASPx.GetControlCollection().Get(data.DXCallbackName);
            if (ctrl != null) ctrl.DoCallback(response);
        }
    });
}
MVCx.IsCustomCallback = function(command){
    return command.toUpperCase() == "CUSTOMCALLBACK";
}
MVCx.IsCustomDataCallback = function(command) {
    var commandInUpperCase = command.toUpperCase();
    return commandInUpperCase == "CUSTOMDATACALLBACK" || commandInUpperCase == "CUSTOMVALUES";
}
MVCx.GetCustomActionCallBackMethod = function(control){
    return control.customActionCallBack || control.callBack;
}
MVCx.MergeHashTables = function(target, object){
    if(!object || typeof(object) == "string")
        return target;

    if (!target )
        target = {};
    for(var key in object)
        if(key && !target[key])
            target[key] = object[key];
    return target;
}

MVCx.AddCallbackParam = function(params, input) {
    if(!params || !input || input.name == "undefined") return;

    if(!params[input.name]){
        switch(input.type){
            case "checkbox":
                params[input.name] = input.checked;
                break;
            case "radio":
                if(input.checked)
                    params[input.name] = input.value;
                break;
            case "select-multiple":
                var values = $(input).val() || [];
                params[input.name] = values.join ? values.join(',') : values;
                break;
            default:
                params[input.name] = input.value;
        }
    }
}
MVCx.AddCallbackParamsInContainer = function(params, container) {
    if(!params || !container) return;

    $(container).find("input,textarea,select").each(function() {
        MVCx.AddCallbackParam(params, this);
    });
}
MVCx.AddDXEditorValuesInContainer = function(params, container){
    if(!params || !container) return;

    var editorValues = {};
    ASPx.GetControlCollection().ForEachControl(function(control){
        if(!ASPx.Ident.IsASPxClientEdit(control)) return;

        var mainElement = control.GetMainElement();
        if(mainElement && ASPx.IsValidElement(mainElement)){
            if(ASPx.GetIsParent(container, mainElement))
                editorValues[control.name] = MVCx.GetEditorValueByControl(control);
        }
    });
    params[MVCx.EditorsValuesKey] = ASPx.Json.ToJson(editorValues);
}

MVCx.EditorValuesSynchronizer = {
    RefreshFormsCache: function(){
        ASPx.GetControlCollection().ForEachControl(function(control){ this.UpdateFormValue(control); }.aspxBind(this));
        this.CacheAllFormValues();
    },
    UpdateFormValue: function(control, isPushValueToCache){
        if(!control || !control.GetMainElement) return;
        var mainElement = control.GetMainElement();
        var form = ASPx.GetParentByTagName(mainElement, "FORM");
        if(!form || !MVCx.IsExistingClientEdit(control) || !mainElement || !ASPx.IsValidElement(mainElement)){
            this.RemoveFormCacheValue(form);
            return;
        }
        var newEditorValue = control.GetEnabled() ? MVCx.GetEditorValueByControl(control) : null;
        this.SetFormCacheValue(form, control.name, newEditorValue);
        if(isPushValueToCache)
            this.CacheFormValues(form);
    },
    CacheAllFormValues: function(){
        for(var i = 0; i < document.forms.length; i++){
            this.CacheFormValues(document.forms[i]);
        }
    },
    CacheFormValues: function(form){
        if(!form || !form.DXEditorValues || !form.dxRequireCache)
            return;

        if(!form.DXEditorValuesField){
            form.DXEditorValuesField = ASPx.CreateHiddenField(MVCx.EditorsValuesKey, "");
            form.appendChild(form.DXEditorValuesField);
        }
        if(form.DXEditorValuesField)
            form.DXEditorValuesField.value = ASPx.Json.ToJson(form.DXEditorValues);
    },
    SetFormCacheValue: function(form, controlName, value){
        if(!form.DXEditorValues)
            form.DXEditorValues = {};
        
        if(form.DXEditorValues[controlName] !== value){
            form.DXEditorValues[controlName] = value;
            form.dxRequireCache = true;
        }
    },
    RemoveFormCacheValue: function(form, controlName){
        if(!form || !form.DXEditorValues) return;
        
        if(ASPx.IsExistsElement(form.DXEditorValues[controlName])){
            delete form.DXEditorValues[controlName];
            form.dxRequireCache = true;
        }
    }
};

MVCx.IsExistingClientEdit = function(control){
    if(!ASPx.Ident.IsASPxClientEdit(control))
        return false;

    var getValueInputMethod = MVCx.GetValueInputMethod(control);
    return getValueInputMethod ? ASPx.IsExistsElement(getValueInputMethod.call(control)) : true;
},
MVCx.GetValueInputElement = function(control){
    var method = MVCx.GetValueInputMethod(control);
    return method ? method.call(control) : null;
},
MVCx.GetValueInputMethod = function(control){
    return control.FindStateInputElement || control.GetStateInput || control.GetInputElement;
},
MVCx.GetEditorValueByControl = function(control){
    if(IsMultipleValueOwner(control))
        return control.GetSelectedValues();
    if(IsTokenBox(control))
        return control.GetTokenValuesCollection();
    return control.GetValue();
}
MVCx.EditorValueChanged = function(s, e){
    MVCx.EditorValuesSynchronizer.UpdateFormValue(s, true);
    if(s.context) s.context.validate("blur");
}

MVCx.GetEditorByElements = function(elements){
    for(var i = 0; i < elements.length; i++){
        var control = MVCx.GetEditorByElement(elements[i]);
        if (control)
            return control;
    }
    return null;
}
MVCx.GetEditorByElement = function(element){
    var control = MVCx.GetControlByElement(element);
    return control && ASPx.Ident.IsASPxClientEdit(control) ? control : null;
}
MVCx.GetControlByElement = function(element){
    var valueInputPostfixes = [ "", "_I", "_S", "_VI", "_STATE", "_KBS" ];
    for(var j = 0; j < valueInputPostfixes.length; j++){
        var regexp = new RegExp("(\S*)" + valueInputPostfixes[j] + "$");
        var controlName = regexp.test(element.id) ? element.id.replace(regexp, function(a,b){ return b; }) : "";
        var control = ASPx.GetControlCollection().Get(controlName);
        if(control) 
            return control;
    }
    return null;
}

MVCx.JQueryValidation = {
    IsEnabled: function(control) {
        return !!($.validator && control.GetParentForm());
    },
    Validate:function(control){
        var valueInput = MVCx.GetValueInputElement(control);
        return $(valueInput).valid();
    },
    SetUVAttributes: function(editor, rules){
        if(!editor || !rules) return;
        $input = $(MVCx.GetValueInputElement(editor));
        $.each(rules, function(ruleName, ruleValue){
            $input.attr(ruleName, ruleValue);
        });
    },
    PrepareUVRules: function(control){
        var form = control.GetParentForm();
        if(form.dxUVExecuted)
            return;

        $(form).removeData("validator");
	    $.validator.unobtrusive.parse(form);
        form.dxUVExecuted = true;
    },
    ResetUVRules: function(control){
        var form = control.GetParentForm();
        if(form)
            delete form.dxUVExecuted;
    },
    HasPendingRequests: function(control){
        var validator = this.GetValidator(control);
        return validator && validator.pendingRequest
    },
    SetOnStopRequestHandler: function(control, handler){
        var validator = this.GetValidator(control);
        if(validator)
            validator.dxOnStopRequestHandler = handler;
    },
    GetValidator: function(control){
        var form = control.GetParentForm();
        return form ? $(form).data('validator') : null;
    },
    EnsureStopRequestHandling: function(validator){
        if(!validator || validator.pendingRequest || !validator.dxOnStopRequestHandler) return;
        validator.dxOnStopRequestHandler();
        delete validator.dxOnStopRequestHandler;
    }
}

if(typeof(Sys) != "undefined" && typeof(Sys.Mvc) != "undefined") {
    MVCx.RequiredValidator = function MVCx_RequiredValidator(){
        MVCx.RequiredValidator.initializeBase(this);
    }
    MVCx.RequiredValidator.prototype = {
        editor: null,
        getEditor: function(context){
            if(!this.editor)
                this.editor = MVCx.GetEditorByElements(context.fieldContext.elements);
            return this.editor;
        },
        validate: function(value, context){
            var editor = this.getEditor(context);
            if(editor)
                return value != null && String(value).length > 0;
            return MVCx.RequiredValidator.callBaseMethod(this, 'validate', [value, context]);
        }
    }
    MVCx.RequiredValidator.registerClass('MVCx.RequiredValidator', Sys.Mvc.RequiredValidator);
    MVCx.RequiredValidator.create = function (rule) {
        return Function.createDelegate(new MVCx.RequiredValidator(), new MVCx.RequiredValidator().validate);
    }
    Sys.Mvc.ValidatorRegistry.validators["required"] = MVCx.RequiredValidator.create;
    
    MVCx.FieldContext = function MVCx_FieldContext(formContext){
        MVCx.FieldContext.initializeBase(this);
    }
    MVCx.FieldContext.prototype = {
        editor: null,
        getEditor: function(){
            if(!this.editor)
                this.editor = MVCx.GetEditorByElements(this.elements);
            return this.editor;
        },
        getDisabled: function() {
            var editor = this.getEditor();
            return this.elements[0].disabled || editor && !editor.GetEnabled();
		},
        enableDynamicValidation: function(){
            MVCx.FieldContext.callBaseMethod(this, 'enableDynamicValidation');
            
            var editor = this.getEditor(this.elements);
            if(editor) editor.context = this;
        },
        _getStringValue: function(){
            var editor = this.getEditor();
            if(editor)
                return editor.GetValueString();
            return MVCx.FieldContext.callBaseMethod(this, '_getStringValue');
        },
        $12: function(){
            var editor = this.getEditor();
            if(editor)
                return editor.GetValueString();
            return MVCx.FieldContext.callBaseMethod(this, '$12');
        },
        validate: function(eventName) {
            var errors = MVCx.FieldContext.callBaseMethod(this, 'validate', [eventName]);
            var editor = this.getEditor();
            if(editor){
                var errorMessage = errors.length == 0 ? null : errors[0];
                MVCx.SetEditorValidationParameters(editor, errorMessage);
            }
            return errors;
        }
    }
    MVCx.FieldContext.registerClass('MVCx.FieldContext', Sys.Mvc.FieldContext);
    
    if(Sys.Mvc.FormContext){
        if(Sys.Mvc.FormContext._parseJsonOptions)
            eval("Sys.Mvc.FormContext._parseJsonOptions = " + Sys.Mvc.FormContext._parseJsonOptions.toString().replace("Sys.Mvc.FieldContext", "MVCx.FieldContext"));
        else if(Sys.Mvc.FormContext.$12)
            eval("Sys.Mvc.FormContext.$12 = " + Sys.Mvc.FormContext.$12.toString().replace("Sys.Mvc.FieldContext", "MVCx.FieldContext"));
        if(Sys.Mvc.FormContext.prototype.validate)
            eval("Sys.Mvc.FormContext.prototype.validate = " + Sys.Mvc.FormContext.prototype.validate.toString().replace("elements[0].disabled", "getDisabled()"));
    }

    if(typeof(ASPx.ClearProcessingProc) != "undefined"){
        MVCx.ClearProcessingProc = ASPx.ClearProcessingProc;
        ASPx.ClearProcessingProc = function(edit) {
            MVCx.ClearProcessingProc(edit);
            if(edit.context) edit.context.clearErrors();
        }
    }
    
    if(typeof(Sys.Mvc.MvcHelpers) != "undefined"){
        var _asyncRequestInternal = Sys.Mvc.MvcHelpers._asyncRequest || Sys.Mvc.MvcHelpers.$2;
        Sys.Mvc.MvcHelpers._asyncRequest = Sys.Mvc.MvcHelpers.$2 = function(url, verb, body, triggerElement, ajaxOptions){
            ASPx.ResourceManager.SynchronizeResources(
                function(name, resource) { 
                    if(body && body.length > 0 && !body.endsWith('&'))
                        body += '&';
                    body += (name + "=" + resource);
                }
            );
            
            var customMethodOnSuccess = ajaxOptions.onSuccess;
            ajaxOptions.onSuccess = function(ajaxContext){
                if(customMethodOnSuccess)
                    customMethodOnSuccess(ajaxContext);
                ASPx.ProcessScriptsAndLinks('', true);
            }
            
            _asyncRequestInternal(url, verb, body, triggerElement, ajaxOptions);
        }
    }
}

if (typeof(jQuery) != "undefined") {
    if (typeof(jQuery.validator) != "undefined"){
        MVCx.getValueOfEditor = function(element){
            var control = MVCx.GetEditorByElement(element);
            return control ? control.GetValueString() : element.value.replace(/\r/g, "");
        }
        function getMainElement(element){
            var control = MVCx.GetEditorByElement(element);
            if (!control)
                return element;
            return control.GetExternalTable() || control.GetMainElement();
        }

        var elementsInternal = $.validator.prototype.elements;
        $.validator.prototype.elements = function(){
            var $elements = elementsInternal.call(this);
            var $dxElements = $();
            ASPxClientControl.GetControlCollection().ProcessControlsInContainer(this.currentForm, function(control){
                if(!MVCx.IsExistingClientEdit(control))
                    return;

                var input = MVCx.GetValueInputElement(control);
                if(input && $.inArray(input, $elements) == -1)
                    $dxElements = $dxElements.add(input);
            });
            return $.merge($elements, $dxElements);
        };
        
        var validationTargetForBase = $.validator.prototype.validationTargetFor;
        $.validator.prototype.validationTargetFor = function(element){
            return MVCx.GetEditorByElement(element) ? element : validationTargetForBase.call(this, element);
        };

        var checkInternal = $.validator.prototype.check;
        $.validator.prototype.check = function(element){
            var control = MVCx.GetEditorByElement(element);
            if(control && (!control.GetEnabled() || !MVCx.validateInvisibleEditors && !control.IsVisible()))
                return;

            var isValid = checkInternal.call(this, element, $);
            if(control)
                MVCx.SetEditorValidationParameters(control, this.errorMap[element.name]);
            return isValid;
        };
        
        var elementValueBase = $.validator.prototype.elementValue;
        $.validator.prototype.elementValue = elementValueBase && function(element){
            var dxControl = MVCx.GetEditorByElement(element)
            return dxControl ? dxControl.GetValueString() : elementValueBase.call(this, element);
        }

        var showLabelInternal = $.validator.prototype.showLabel;
        $.validator.prototype.showLabel = function(element, message){
            var control = MVCx.GetEditorByElement(element);
            if(control)
                MVCx.SetEditorValidationParameters(control, message);
            if(control && !$.validator.unobtrusive)
                element = control.GetMainElement();
            if(!control || !control.GetErrorCell())
                showLabelInternal.call(this, element, message);
        }

        var optionalInternal = $.validator.prototype.optional;
        $.validator.prototype.optional = function(element){
            var dxControl = MVCx.GetEditorByElement(element);
            if(!this.elementValue && dxControl)
                return !$.validator.methods.required.call(this, dxControl.GetValueString(), element) && "dependency-mismatch";
            return optionalInternal.call(this, element);
        }

        if (!$.validator.prototype.elementValue){
            var requiredInternal = $.validator.methods.required;
            $.validator.methods.required = function(value, element, param){
                return requiredInternal.call(this, MVCx.getValueOfEditor(element), element);
            }

            var rangeInternal = $.validator.methods.range;
            $.validator.methods.range = function(value, element, param){
                return rangeInternal.call(this, MVCx.getValueOfEditor(element), element, param);
            }

            var minlengthInternal = $.validator.methods.minlength;
            $.validator.methods.minlength = function(value, element, param){
                return minlengthInternal.call(this, MVCx.getValueOfEditor(element), element, param);
            }

            var maxlengthInternal = $.validator.methods.maxlength;
            $.validator.methods.maxlength = function(value, element, param){
                return maxlengthInternal.call(this, MVCx.getValueOfEditor(element), element, param);
            }

            var rangelengthInternal = $.validator.methods.rangelength;
            $.validator.methods.rangelength = function(value, element, param){
                return rangelengthInternal.call(this, MVCx.getValueOfEditor(element), element, param);
            }

            var minInternal = $.validator.methods.min;
            $.validator.methods.min = function(value, element, param){
                return minInternal.call(this, MVCx.getValueOfEditor(element), element, param);
            }

            var maxInternal = $.validator.methods.max;
            $.validator.methods.max = function(value, element, param){
                return maxInternal.call(this, MVCx.getValueOfEditor(element), element, param);
            }

            var emailInternal = $.validator.methods.email;
            $.validator.methods.email = function(value, element){
                return emailInternal.call(this, MVCx.getValueOfEditor(element), element);
            }

            var urlInternal = $.validator.methods.url;
            $.validator.methods.url = function(value, element){
                return urlInternal.call(this, MVCx.getValueOfEditor(element), element);
            }

            var dateISOInternal = $.validator.methods.dateISO;
            $.validator.methods.dateISO = function(value, element){
                return dateISOInternal.call(this, MVCx.getValueOfEditor(element), element);
            }

            var numberInternal = $.validator.methods.number;
            $.validator.methods.number = function(value, element){
                return numberInternal.call(this, MVCx.getValueOfEditor(element), element);
            }

            var digitsInternal = $.validator.methods.digits;
            $.validator.methods.digits = function(value, element){
                return digitsInternal.call(this, MVCx.getValueOfEditor(element), element);
            }

            var creditcardInternal = $.validator.methods.creditcard;
            $.validator.methods.creditcard = function(value, element){
                return creditcardInternal.call(this, MVCx.getValueOfEditor(element), element);
            }

            var acceptInternal = $.validator.methods.accept;
            $.validator.methods.accept = function(value, element, param){
                return acceptInternal.call(this, MVCx.getValueOfEditor(element), element, param);
            }
        }

        var dateInternal = $.validator.methods.date;
        $.validator.methods.date = function(value, element){
            var editor = MVCx.GetEditorByElement(element);
            if(editor)
                value = editor.GetValue();
            return dateInternal.call(this, value, element);
        };

        var equalToInternal = $.validator.methods.equalTo;
        $.validator.methods.equalTo = function(value, element, param){
            var target = $(param).unbind(".validate-equalTo").bind("blur.validate-equalTo", function() {
				$(element).valid();
			});
            var dxEditor;
            if(target.length > 0)
                dxEditor = MVCx.GetEditorByElement(target[0]);
            return dxEditor ? value == dxEditor.GetValue() : equalToInternal.call(this, value, element, param);
        }
        
        var remoteInternal = $.validator.methods.remote;
        $.validator.methods.remote = function(value, element, param){
            if(param && param.data){
                $.each(param.data, function(fieldName){
                    var editorName = param.dxfieldsmap && param.dxfieldsmap[fieldName] || fieldName;
                    var dxeditor = ASPx.GetControlCollection().Get(editorName);
                    if(dxeditor)
                        param.data[fieldName] = dxeditor.GetValueString();
                });
            }
            return remoteInternal.call(this, value, element, param);
        };

        var stopRequestInternal = $.validator.prototype.stopRequest;
        $.validator.prototype.stopRequest = function(element, valid) {
            stopRequestInternal.call(this, element, valid);
            MVCx.JQueryValidation.EnsureStopRequestHandling(this);
        }
        
        $.validator.addMethod("dxmask", function (value, element, params) {
            var control = MVCx.GetEditorByElement(element);
            if(control && control.maskInfo)
                return control.maskInfo.IsValid();
            return true;
        });
        $.validator.addMethod("dxdaterange", function (value, element, params) {
            var control = MVCx.GetEditorByElement(element);
            if(!control || !control.GetStartDateEdit)
                return;
            if(control.MinOrMaxRangeExist && !control.MinOrMaxRangeExist())
                return true;
            var pattern = new ASPx.DateRangeValidationPattern(control.GetStartDateEdit(), control);
            return pattern && pattern.EvaluateIsValid();
        });
    }

    if(typeof(jQuery.validator) != "undefined" && typeof(jQuery.validator.unobtrusive) != "undefined"){
        jQuery.validator.unobtrusive.adapters.add("dxmask", [], function (options) {
            options.rules["dxmask"] = { };
            options.messages["dxmask"] = options.message;
        });
        jQuery.validator.unobtrusive.adapters.add("dxdaterange", [], function (options) {
            options.rules["dxdaterange"] = { };
            options.messages["dxdaterange"] = options.message;
        });
        $.each(jQuery.validator.unobtrusive.adapters, function (i, adapter) {
            if (adapter.name != "remote") return;
            adapter.params.push("dxfieldsmap");
            adapter.adaptCore = adapter.adapt;
            adapter.adapt = function (options) {
                this.adaptCore(options);
                if (options.params.dxfieldsmap)
                    $.extend(options.rules["remote"], { dxfieldsmap: options.params.dxfieldsmap });
            }
        });
        jQuery.validator.unobtrusive.prototype = {
            parseElement: jQuery.validator.unobtrusive.parseElement
        };
        jQuery.validator.unobtrusive.parseElement = function (element, skipAttach) {
            jQuery.validator.unobtrusive.prototype.parseElement.call(this, element, skipAttach);
            var editor = MVCx.GetEditorByElement(element);
            if (editor && ASPx.IsExists(editor.validationPatterns))
                editor.validationPatterns = [ ];
        };
    }
    if(jQuery.expr && jQuery.expr[":"] && jQuery.expr[":"].tabbable) {
       var tabbableCore = jQuery.expr[":"].tabbable;
       jQuery.expr[":"].tabbable = function(element) {
            return tabbableCore.call(this, element) || isInternalTabbableInput(element);
        }
        function isInternalTabbableInput(element) {
            if (!element || element.tagName != 'INPUT') return false;
        
            var tabIndex = ASPx.Attr.GetAttribute(element, ASPx.Attr.GetTabIndexAttributeName());
            return (!tabIndex || tabIndex >= 0) && IsVisibleControlInput(element);
        }
        function IsVisibleControlInput(element) {
            var control = MVCx.GetControlByElement(element);
            return control && control.IsVisible();
        }
    }

    jQuery.prototype.ajax = jQuery.ajax;
    jQuery.ajax = function(url, settings){
        if(typeof url === "object"){
            settings = url;
            url = undefined;
        }
        
        var baseBeforeSendMethod = settings.beforeSend;
        settings.beforeSend = function(jqXHR, options){
            var result;
            if(baseBeforeSendMethod)
                result = baseBeforeSendMethod.call(this, jqXHR, options);

            ASPx.ResourceManager.SynchronizeResources(
                function(name, resource) { 
                    jqXHR.setRequestHeader(name, resource); 
                }
            );
            return result;
        };
        
        var baseCompleteMethod = settings.complete;
        settings.complete = function(jqXHR, status) {
            $("[data-ajax=true]")
            .filter(function() { return (this.action || this.href) === settings.url; })
            .each(function() {
                var insertionMode = ($(this).attr("data-ajax-mode") || "").toUpperCase();
                if(insertionMode && insertionMode != "REPLACE" && MVCx.isDXScriptInitializedOnLoad)
                    ASPx.RunStartupScripts();
            });
            var result;
            if(baseCompleteMethod)
                result = baseCompleteMethod.call(this, jqXHR, status);
            return result;
        }
        
        var params = [ ];
        if(url)
            params.push(url);
        params.push(settings);
        return jQuery.prototype.ajax.apply(this, params);
    };

    jQuery.prototype.clean = jQuery.clean;
    jQuery.clean = function(elems, context, fragment, scripts){
        var result = jQuery.prototype.clean.call(this, elems, context, fragment, scripts);
        for(var i=0; scripts && i < scripts.length; i++){
            var script = scripts[i];
            if(isDXScriptElement(script)){
                fragment.appendChild(script);
                scripts.splice(i--, 1);
            }
        }
        return result;
    };
    
    var mapBase = jQuery.map;
    jQuery.map = function (elems, callback, arg){
        function calbackInternal(element, index, arg){
            if(isDXScriptElement(element))
                return;
            return callback(element, index, arg);
        }
        return mapBase.call(this, elems, calbackInternal, arg);
    }

    jQuery.fn.prototype = {
        domManip: jQuery.fn.domManip
    }
    jQuery.fn.domManip = function (args, table, callback) {
        var result = jQuery.fn.prototype.domManip.apply(this, arguments);
        if(MVCx.isDXScriptInitializedOnLoad && haveElementsDXScript(args))
            ASPx.RunStartupScripts();
        return result;
    };

    function isDXScriptElement(element){
        return element && element.id && element.id.indexOf && element.id.indexOf(ASPx.startupScriptPrefix) == 0;
    }
    function haveElementsDXScript(elements){
        if(!elements || !elements.length)
            return false;

        var isDXScriptFound = false;
        var dxScriptSelector = "script[id^=" + ASPx.startupScriptPrefix + "]";
        var isHtmlExpression = /<|&#?\w+;/;
        for(var i = 0; i < elements.length && !isDXScriptFound; i++){
            var element = elements[i];
            if ($.type(element) != "object" && !isHtmlExpression.test(element)) continue;

            var $element = element instanceof jQuery ? element : $("<div>" + element + "</div>");
            $element.each(function(){
                var $this = $(this);
                if($this.is(dxScriptSelector) || $this.find(dxScriptSelector).length){
                    isDXScriptFound = true;
                    return false;
                }
            });
        }
        return isDXScriptFound;
    }
}

MVCx.isDXScriptInitializedOnLoad = false;
MVCx.validateInvisibleEditors = false;
MVCx.EditorSetValue = function(value){
    if(this.oldSetValue)
        this.oldSetValue(value);
    MVCx.EditorValueChanged(this, new ASPxClientEventArgs());
}
MVCx.SetEditorValidationParameters = function(control, errorText){
    if(errorText)
        control.SetErrorText(errorText);
    
    if(typeof(ASPxClientValidationSummary) != "undefined")
        ASPx.GetClientValidationSummaryCollection().AllowNewErrorsAccepting();
    
    control.SetIsValid(!errorText);

    if(typeof(ASPxClientValidationSummary) != "undefined")
        ASPx.GetClientValidationSummaryCollection().ForbidNewErrorsAccepting();
}
MVCx.MultipleEditorSelectValues = function(values){
    if(this.oldSelectValues)
        this.oldSelectValues(values);
    MVCx.EditorValueChanged(this, new ASPxClientEventArgs());
}
MVCx.MultipleEditorSelectAll = function(){
    if(this.oldSelectAll)
        this.oldSelectAll();
    MVCx.EditorValueChanged(this, new ASPxClientEventArgs());
}
MVCx.MultipleEditorUnselectAll = function(){
    if(this.oldUnselectAll)
        this.oldUnselectAll();
    MVCx.EditorValueChanged(this, new ASPxClientEventArgs());
}
MVCx.ListBoxSetSelectedIndex = function(index){
    if(this.oldSetSelectedIndex)
        this.oldSetSelectedIndex(index);
    MVCx.EditorValueChanged(this, new ASPxClientEventArgs());
}
MVCx.ListBoxSetIndicesSelectionState = function(indices, selected){
    if(this.oldSetIndicesSelectionState){
        this.oldSetIndicesSelectionState(indices, selected);
    }
    MVCx.EditorValueChanged(this.listBoxControl, new ASPxClientEventArgs());
}
MVCx.ComboBoxSelectIndex = function(index, initialize){
    if(this.oldSelectIndex)
        this.oldSelectIndex(index, initialize);
    MVCx.EditorValueChanged(this, new ASPxClientEventArgs());
    MVCx.EditorValueChanged(this.GetListBoxControl(), new ASPxClientEventArgs());
}

function aspxMVCControlsInitialized(s, e){
    ASPx.GetControlCollection().ForEachControl(function(control){
        if(!MVCx.IsExistingClientEdit(control) || control.AreSyncHandlersAdded || !control.ValueChanged)
            return;

        if(IsTokenBox(control))
            control.TokensChanged.InsertFirstHandler(MVCx.EditorValueChanged);
        else
            control.ValueChanged.InsertFirstHandler(MVCx.EditorValueChanged);
        control.EnabledChanged.InsertFirstHandler(MVCx.EditorValueChanged);
        control.LostFocus.InsertFirstHandler(function(){ MVCx.EditorValuesSynchronizer.RefreshFormsCache(); });
        if (IsMultipleValueOwner(control))
            control.SelectedIndexChanged.InsertFirstHandler(MVCx.EditorValueChanged);

        if(!control.oldSetValue){
            control.oldSetValue = control.SetValue;
            control.SetValue = MVCx.EditorSetValue;
        }
        if(IsMultipleValueOwner(control) && !control.oldSelectValues){
            control.oldSelectValues = control.SelectValues;
            control.SelectValues = MVCx.MultipleEditorSelectValues;
        }
        if(IsMultipleValueOwner(control) && !control.oldSelectAll){
            control.oldSelectAll = control.SelectAll;
            control.SelectAll = MVCx.MultipleEditorSelectAll;
        }
        if(IsMultipleValueOwner(control) && !control.oldUnselectAll){
            control.oldUnselectAll = control.UnselectAll;
            control.UnselectAll = MVCx.MultipleEditorUnselectAll;
        }
        if(IsListBox(control)){
            if(!control.oldSetSelectedIndex){
                control.oldSetSelectedIndex = control.SetSelectedIndex;
                control.SetSelectedIndex = MVCx.ListBoxSetSelectedIndex;
            }
            var itemSelectionHelper = control.GetItemSelectionHelper();
            if(itemSelectionHelper && !itemSelectionHelper.oldSetIndicesSelectionState){
                itemSelectionHelper.oldSetIndicesSelectionState = itemSelectionHelper.SetIndicesSelectionState;
                itemSelectionHelper.SetIndicesSelectionState = MVCx.ListBoxSetIndicesSelectionState;
            }
        }
        if(IsComboBox(control) && !control.oldSelectIndex){
            control.oldSelectIndex = control.SelectIndex;
            control.SelectIndex = MVCx.ComboBoxSelectIndex;
        }

        if(jQuery.validator && (IsGridLookup(control) || IsSpinEdit(control))){ //B215249
            control.ValueChanged.AddHandler(function(s, e){
                if(MVCx.JQueryValidation.IsEnabled(s))
                    MVCx.JQueryValidation.Validate(s);
            });
        }

        control.AreSyncHandlersAdded = true;
    });
    ASPx.GetPostHandler().Update();
    MVCx.EditorValuesSynchronizer.RefreshFormsCache();
    MVCxClientGlobalEvents.OnControlsInitialized(e);
}
function IsMultipleValueOwner(control) {
    return IsListBox(control) || IsCheckBoxList(control);
}
function IsCheckBoxList(control) {
    return control && typeof(ASPxClientCheckBoxList) != "undefined" && control instanceof ASPxClientCheckBoxList;
}
function IsListBox(control) {
    return control && typeof(ASPxClientListBox) != "undefined" && control instanceof ASPxClientListBox;
}
function IsComboBox(control) {
    return control && typeof(ASPxClientComboBox) != "undefined" && control instanceof ASPxClientComboBox;
}
function IsTokenBox(control) {
    return control && typeof(ASPxClientTokenBox) != "undefined" && control instanceof ASPxClientTokenBox;
}
function IsGridLookup(control) {
    return control && typeof(MVCxClientGridLookup) != "undefined" && control instanceof MVCxClientGridLookup;
}
function IsSpinEdit(control){
    return control && typeof(ASPxClientSpinEdit) != "undefined" && control instanceof ASPxClientSpinEdit;
}
ASPx.GetControlCollection().ControlsInitialized.AddHandler(aspxMVCControlsInitialized);
ASPx.Evt.AttachEventToElement(window, "load", function (evt) { MVCx.isDXScriptInitializedOnLoad = true; });
var MVCxClientUtils = {};
MVCxClientUtils.FinalizeCallback = function(){
    ASPx.ProcessScriptsAndLinks('', true);
}
var MVCxClientBeginCallbackEventArgs = ASPx.CreateClass(ASPxClientBeginCallbackEventArgs, {
    constructor: function(command){
        this.constructor.prototype.constructor.call(this, command);
        this.customArgs = {};
    }
});
var MVCxClientGlobalEvents = {
    ControlsInitialized: new ASPxClientEvent(),
    BeginCallback: new ASPxClientEvent(),
    EndCallback: new ASPxClientEvent(),
    CallbackError: new ASPxClientEvent(),
    AddControlsInitializedEventHandler: function(handler) {
        this.ControlsInitialized.AddHandler(handler);
    },
    AddBeginCallbackEventHandler: function(handler) {
        this.BeginCallback.AddHandler(handler);
    },
    AddEndCallbackEventHandler: function(handler) {
        this.EndCallback.AddHandler(handler);
    },
    AddCallbackErrorHandler: function(handler) {
        this.CallbackError.AddHandler(handler);
    },
    OnControlsInitialized: function(args) {
        if(!this.ControlsInitialized.IsEmpty())
            this.ControlsInitialized.FireEvent(this, args);
    },
    OnBeginCallback: function(args) {
        if(!this.BeginCallback.IsEmpty())
            this.BeginCallback.FireEvent(this, args);
    },
    OnEndCallback: function() {
        if(!this.EndCallback.IsEmpty()) {
            var args = new ASPxClientEndCallbackEventArgs();
            this.EndCallback.FireEvent(this, args);
        }
    },
    OnCallbackError: function(args) {
        if(!this.CallbackError.IsEmpty())
            this.CallbackError.FireEvent(this, args);
    }
};

window.MVCxClientUtils = MVCxClientUtils;
window.MVCxClientGlobalEvents = MVCxClientGlobalEvents;
window.MVCxClientBeginCallbackEventArgs = MVCxClientBeginCallbackEventArgs;
})();