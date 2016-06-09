/// <reference path="..\_references.js"/>

(function() {
var ASPxClientCheckBox = ASPx.CreateClass(ASPxClientEdit, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);

        this.isASPxClientCheckEdit = true;
        this.valueChecked = true;
        this.valueUnchecked = false;
        this.valueGrayed = null;
        this.allowGrayed = false;
        this.isNative = false;
        this.stateController = null;
        this.imageProperties = null;
        this.allowGrayedByClick = true;
        this.internalCheckBox = null;
        this.icbFocusedStyle = [];

        this.sizingConfig.allowSetWidth = false;
        this.sizingConfig.allowSetHeight = false;
        this.CheckedChanged = new ASPxClientEvent();
    },
    InlineInitialize: function() {
		this.isNative = !this.imageProperties;
        this.allowGrayed = this.IsGrayedAllowed();
        this.stateController = ASPx.CheckableElementStateController.Create(this.imageProperties, this.valueChecked,  this.valueUnchecked, this.valueGrayed, this.allowGrayed);
        if(!this.isNative)
            this.CreateInternalCheckBox();

        ASPxClientEdit.prototype.InlineInitialize.call(this);
    },

    Initialize: function() {
        ASPxClientEdit.prototype.Initialize.call(this);

        if(!ASPx.IsExistsElement(this.GetMainElement()))
            return;

        if(!this.isNative) {
            this.previousValue = this.GetStateInput().value;
            this.SetValue(this.GetValue());
        }
        else
            this.previousValue = this.GetInputElement().checked;
    },
    GetInputElement: function() {
        if(this.isNative)
            return ASPxClientEdit.prototype.GetInputElement.call(this);
        else
            return this.GetStateInput();
    },
    CreateInternalCheckBox: function() {
        var instance = this;
        this.internalCheckBox = new ASPx.CheckBoxInternal(this.GetStateInput(), this.stateController, this.allowGrayed, this.allowGrayedByClick, 
            ASPx.CheckEditElementHelper.Instance, this.GetMainElement(), true, undefined, undefined, this.accessibilityCompliant);
        this.internalCheckBox.CreateFocusDecoration(this.icbFocusedStyle);
        this.internalCheckBox.SetEnabled(this.GetEnabled());
        this.internalCheckBox.readOnly = this.readOnly; 
        this.internalCheckBox.CheckedChanged.AddHandler(function(s, e) { instance.OnClick(e); });
        this.internalCheckBox.Focus.AddHandler(function() { instance.OnFocus(); });
        this.internalCheckBox.LostFocus.AddHandler(function() { instance.OnLostFocus(); });
        if(ASPx.Browser.WebKitFamily) //B186511
            ASPx.Evt.AttachEventToElement(this.internalCheckBox.mainElement.parentNode, "selectstart", ASPx.Evt.PreventEventAndBubble);
    },
	FindInputElement: function() {
        var element = this.GetMainElement();
        if(ASPx.IsExistsElement(element) && element.tagName != "INPUT")
            element = this.GetChildElement("I");
        return element;
    },
    GetFocusableInputElement: function() {
        return this.accessibilityCompliant ? this.GetICBMainElement() : this.GetInputElement();
    },
    IsGrayedAllowed: function() {
        for(var propertyKey in this.imageProperties)
            if(this.imageProperties[propertyKey].length === 3)
                return true;
        return false;
    },
    RaiseValueChangedEvent: function() {
        var processOnServer = ASPxClientEdit.prototype.RaiseValueChangedEvent.call(this);
        processOnServer = this.RaiseCheckedChanged(processOnServer);
        return processOnServer;
    },    
    OnClick: function() {
        if(this.ToogleCheckState() && !this.focused)
            this.SetFocus();
    },  
    ToogleCheckState: function() {
        if(!this.GetEnabled() || this.readOnly)
            return false;
        var value = this.GetCorrectValue(this.previousValue);
        value = this.stateController.GetNextCheckBoxValue(value, this.allowGrayedByClick && this.allowGrayed);       
        this.SetValue(value);
        this.OnValueChanged();
        return true;
    },
    IsElementBelongToInputElement: function(element) {
        return false; //B197442
    },
    ForceRefocusEditor: function() {
        if(this.isNative)
            ASPxClientEdit.prototype.ForceRefocusEditor.call(this);
    },
    // value
    GetValue: function() {
        var value = this.stateController.GetValueByInputKey(this.GetStateInput().value);
        if(value === "" && this.convertEmptyStringToNull)
            value = null;                    
        return value;
    },        
    SetValue: function(value) {
        this.previousValue = value;
        value = this.GetCorrectValue(value);
        if(this.isNative)
            this.GetInputElement().checked = (this.previousValue == this.valueChecked);
        else
            this.internalCheckBox.SetValue(value);
        this.GetStateInput().value = this.stateController.GetInputKeyByValue(value);
        if(this.accessibilityCompliant) {
            var state = this.GetCheckState();
            var mainElement = this.GetICBMainElement();
            var value = this.stateController.GetAriaCheckedValue(state);
            if(mainElement.attributes["aria-checked"] !== undefined)
                mainElement.setAttribute("aria-checked", value); 
            if(mainElement.attributes["aria-selected"] !== undefined)
                mainElement.setAttribute("aria-selected", value);
        }
        this.OnValueSet();
    },
    GetICBMainElement: function() {
        return ASPx.CheckableElementHelper.Instance.GetICBMainElementByInput(this.GetStateInput());
    },
    GetLabelElement: function() {
        return ASPx.CheckEditElementHelper.Instance.GetLabelElement(this.GetMainElement());
    },
    GetStateInput: function() {
        return this.GetChildElement("S");
    },
    GetCorrectValue: function(value) {
        var stateIsExist = typeof(this.stateController.GetInputKeyByValue(value)) != "undefined";
        if(!stateIsExist)
            return this.allowGrayed ? this.valueGrayed : this.valueUnchecked; 
        return value;
    },
    OnValueSet: function() {},
    // API
    RaiseCheckedChanged: function(processOnServer) {
        if(!this.CheckedChanged.IsEmpty()) {
            var args = new ASPxClientProcessingModeEventArgs(processOnServer);
            this.CheckedChanged.FireEvent(this, args);
            processOnServer = args.processOnServer;
        }
        return processOnServer;
    },
    SetEnabled: function(enabled) {
        ASPxClientEdit.prototype.SetEnabled.call(this, enabled);
        if(!this.isNative) 
                this.internalCheckBox.SetEnabled(enabled);
    },
    GetChecked: function() {
        return this.stateController.GetCheckStateByInputKey(this.GetStateInput().value) == ASPx.CheckBoxCheckState.Checked;
    },
    SetChecked: function(isChecked) {
        this.SetCheckState(isChecked ? ASPx.CheckBoxCheckState.Checked : ASPx.CheckBoxCheckState.Unchecked);
    },
    GetText: function() {
        var labelElement = this.GetLabelElement();
        return (labelElement != null) ? labelElement.innerHTML : "";
    },
    GetCheckState: function() {
        var internalCheckeState = this.stateController.GetCheckStateByInputKey(this.GetStateInput().value);
        if(internalCheckeState != ASPx.CheckBoxCheckState.Indeterminate || this.allowGrayed)
            return internalCheckeState;
        else 
            return ASPx.CheckBoxCheckState.Unchecked;
    },
    SetCheckState: function(checkState) {
        if(checkState != ASPx.CheckBoxCheckState.Indeterminate || this.allowGrayed) {
            var value = this.stateController.GetValueByCheckState(checkState);
            this.SetValue(value);
        }
    },
    SetText: function(text) {
        var labelElement = this.GetLabelElement();
        if(labelElement != null) 
            ASPx.SetInnerHtml(labelElement, text);
    },
    
    ChangeEnabledAttributes: function(enabled){
        if(this.isNative) {
            this.ChangeInputEnabledAttributes(this.GetInputElement(), ASPx.Attr.ChangeAttributesMethod(enabled));
            this.GetInputElement().disabled = !enabled;
        } else
            this.internalCheckBox.SetEnabled(enabled);
    },
    ChangeEnabledStateItems: function(enabled){
        ASPx.GetStateController().SetElementEnabled(this.GetMainElement(), enabled);
        this.GetStateInput().disabled = !enabled;
    },
	ChangeInputEnabledAttributes: function(element, method){
        method(element, "onclick");
	}
});
ASPxClientCheckBox.Cast = ASPxClientControl.Cast;
var ASPxClientRadioButton = ASPx.CreateClass(ASPxClientCheckBox, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.isASPxClientRadioButton = true;
        this.groupName = "";
    },
    OnValueSet: function() {
        if(this.previousValue === true) 
            this.UncheckOtherGroupMembers(true /* suppressEvents */);
    },
    OnClick: function() {
        if(!this.previousValue) {
            this.UncheckOtherGroupMembers();
            ASPxClientCheckBox.prototype.OnClick.call(this);
        } else {
            if(!this.focused)
                this.SetFocus();
        }
    },
    CreateInternalCheckBox: function() {
        ASPxClientCheckBox.prototype.CreateInternalCheckBox.call(this);
        this.internalCheckBox.autoSwitchEnabled = false;
    },
    UncheckOtherGroupMembers: function(suppressEvents) {
        var members = this.GetGroupMembers();
        for(var i = 0; i < members.length; i++) {
            var radioButton = members[i];
            if(!ASPx.IsExistsElement(radioButton.GetMainElement()))
                members[i] = null;
            else {
                if(radioButton != this && radioButton.GetValue()){
                    radioButton.SetValue(false);
                    if(!suppressEvents)
                        radioButton.RaiseValueChangedEvent();
                }
            }
        }
    },
    OnReadonlyClick: function() {
        if(!this.previousValue) {            
            var members = this.GetGroupMembers();            
            for(var i = 0; i < members.length; i++) {
                var radioButton = members[i];
                radioButton.SetValue(radioButton.GetValue());
            }
        }
    },
    GetGroupName: function() {
        if(!this.isNative)
            return this.groupName;
        
        var inputElement = this.GetInputElement();
        if (!ASPx.IsExistsElement(inputElement))
            return null;
        var name = inputElement.name;
        if(!name.length)
            name = "";
        return name;
    },
    GetGroupMembers: function() {
        var result = [ ];
        var groupName = this.GetGroupName();
        if(groupName.length > 0) {
            ASPx.GetControlCollection().ForEachControl(function(control) {
                if(ASPx.Ident.IsASPxClientRadioButton(control)) {
                    var controlGroupName = control.GetGroupName();
                    if (controlGroupName != null && controlGroupName == groupName)
                        result.push(control);
                }
            });
        } else {
            result.push(this);
        }
        return result;        
    },
    // API
    GetChecked: function() {
        return this.GetValue() == true;
    },
    SetChecked: function(isChecked) {
        this.SetValue(isChecked);
    }
});
ASPxClientRadioButton.Cast = ASPxClientControl.Cast;

ASPx.Ident.IsASPxClientCheckEdit = function(obj) {
    return !!obj.isASPxClientCheckEdit;
};
ASPx.Ident.IsASPxClientRadioButton = function(obj) {
    return !!obj.isASPxClientRadioButton;
};
ASPx.Ident.IsASPxClientCheckBox = function(obj) {
    return ASPx.Ident.IsASPxClientCheckEdit(obj) && !ASPx.Ident.IsASPxClientRadioButton(obj);
};

ASPx.ChkOnClick = function(name) {
    var edit = ASPx.GetControlCollection().Get(name);
    if(edit)
        edit.OnClick();
}
ASPx.ERBOnReadonlyClick = function(name) {
    var rb = ASPx.GetControlCollection().Get(name);
    if(rb)
        rb.OnReadonlyClick();
}

window.ASPxClientCheckBox = ASPxClientCheckBox;
window.ASPxClientRadioButton = ASPxClientRadioButton;
})();