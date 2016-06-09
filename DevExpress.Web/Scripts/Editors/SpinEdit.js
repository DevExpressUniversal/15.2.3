/// <reference path="..\_references.js"/>

(function() {
ASPx.NumberDecimalSeparator = ",";

var spindButtonIdPostfix = "_B";
var repeatBtnMinIntervalDelay = 5;
var repeatBtnMaxIntervalDelay = 300;
var ASPxClientSpinEditBase = ASPx.CreateClass(ASPxClientButtonEditBase, {

    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);

        this.largeDecButtonIndex = -1;
        this.incButtonIndex = -2;
        this.decButtonIndex = -3;
        this.largeIncButtonIndex = -4;

        this.valueChangedDelay = 0;
        this.valueChangedTimerID = -1;
        this.repeatButtonTimerID = -1;
        this.isValidating = false;

        aspxGetSpinEditCollection().Add(this);
    },
    /*OnButtonClick: function(number){
        this.ClearButtonRepeatClickTimer();
    },*/
    Initialize: function() {
        this.AssignButtonAttributes();
        ASPxClientButtonEditBase.prototype.Initialize.call(this);
    },
    AdjustControlCore: function() {
        ASPxClientButtonEditBase.prototype.AdjustControlCore.call(this);
        if(this.GetIncDecButtonsContainer()) {
            this.CorrectVerticalAlignment(ASPx.AdjustHeightInContainer, this.GetIncDecButtonsContainer, "Btns");
            this.CorrectVerticalAlignment(ASPx.AdjustVerticalMargins, this.GetIncDecButtonsImages, "Imgs");
        }
    },

    AssignButtonAttributes: function() {
        var buttons = [
			this.GetIncrementButton(),
			this.GetDecrementButton(),
			this.GetLargeIncrementButton(),
			this.GetLargeDecrementButton()
		];
        for(var i = 0; i < buttons.length; i++) {
            var button = buttons[i];
            if(!ASPx.IsExistsElement(button)) continue;
            ASPx.Evt.PreventElementDragAndSelect(button, true);
            button.needClearSelection = true;
            if(!ASPx.Browser.NetscapeFamily)
                button.unselectable = "on";
            var img = ASPx.GetNodesByTagName(button, "img")[0];
            if(ASPx.IsExistsElement(img))
                ASPx.Evt.PreventImageDragging(img);
            this.AttachButtonEvents(button);
        }
    },
    AttachButtonEvents: function(button){
        ASPx.Evt.AttachEventToElement(button, "mouseout", function () { this.ClearButtonRepeatClickTimer(); }.aspxBind(this));
        ASPx.Evt.AttachEventToElement(button, ASPx.TouchUIHelper.touchMouseDownEventName, function (evt) { this.OnButtonMouseDown(evt, button.id); }.aspxBind(this));
        ASPx.Evt.AttachEventToElement(button, ASPx.TouchUIHelper.touchMouseUpEventName, function () { this.OnButtonMouseUp(button.id); }.aspxBind(this));
    },

    GetIncDecButtonsContainer: function() { 
        if(this.GetIncrementButton())
            return this.GetIncrementButton().parentNode;
        if(this.GetDecrementButton())
            return this.GetDecrementButton().parentNode;
        return null;
    },
    GetIncDecButtonsImages: function() { 
        var elements = [];
        if(this.GetIncrementButton())
            elements.push(ASPx.GetNodeByTagName(this.GetIncrementButton(), "IMG", 0));
        if(this.GetDecrementButton())
            elements.push(ASPx.GetNodeByTagName(this.GetDecrementButton(), "IMG", 0));
        return elements;
    },
    GetIncrementButton: function() { 
        return this.GetButton(this.incButtonIndex); 
    },
    GetDecrementButton: function() { 
        return this.GetButton(this.decButtonIndex); 
    },
    GetLargeIncrementButton: function() { 
        return this.GetButton(this.largeIncButtonIndex); 
    },
    GetLargeDecrementButton: function() { 
        return this.GetButton(this.largeDecButtonIndex); 
    },

    GetButtonNumber: function(id) {
        var pos = id.lastIndexOf(spindButtonIdPostfix);
        if(pos > -1)
            return id.substring(pos + spindButtonIdPostfix.length, id.length);
        return null;
    },

    GetNextTimerInterval: function(iterationIndex) {
        var coef = 1 / iterationIndex;
        if(coef < 0.13)
            coef = 0.13;
        return coef * repeatBtnMaxIntervalDelay;
    },
    DoRepeatButtonClick: function(num, iterationIndex) {
        this.ProcessInternalButtonClick(num);

        var timerInterval = this.GetNextTimerInterval(iterationIndex);
        
        if(iterationIndex < 50)
            iterationIndex++;

        this.SetButtonRepeatClickTimer(num, timerInterval, iterationIndex);
    },
    StartButtonRepeatClickTimer: function(num, timerInterval, iterationIndex) {
        this.repeatButtonTimerIDLocked = false;
        this.SetButtonRepeatClickTimer(num, timerInterval, iterationIndex);
    },
    SetButtonRepeatClickTimer: function(num, timerInterval, iterationIndex) {
        if(this.repeatButtonTimerIDLocked)
            return;
        var repeatButtonClickHandler = function() {
            this.DoRepeatButtonClick(num, iterationIndex);
        };
        this.repeatButtonTimerID = ASPx.Timer.SetControlBoundTimeout(repeatButtonClickHandler, this, timerInterval);
    },
    ClearButtonRepeatClickTimer: function() {
        this.repeatButtonTimerIDLocked = true;
        this.repeatButtonTimerID = ASPx.Timer.ClearTimer(this.repeatButtonTimerID);
    },

    ProcessInternalButtonClick: function(buttonIndex) {
    },

    OnButtonMouseDown: function(evt, id) {
        var num = this.GetButtonNumber(id);
        if(num != null)
            this.StartButtonRepeatClickTimer(num, 300, 1);

        
        if(ASPx.Browser.NetscapeFamily || ASPx.TouchUIHelper.isTouchEvent(evt)) // forNSFamilly browsers hack
            evt.preventDefault();
    },
    OnButtonMouseUp: function(id) {
        this.ClearButtonRepeatClickTimer();
    },
    OnValueChanged: function() {
        if(this.valueChangedDelay == 0)
            this.OnValueChangedTimer();
        else {
            if(this.valueChangedTimerID > -1) {
                window.clearTimeout(this.valueChangedTimerID);
                this.valueChangedTimerID = -1;
            }
            this.valueChangedTimerID = ASPx.Timer.SetControlBoundTimeout(this.OnValueChangedTimer, this, this.valueChangedDelay);
        }
    },
    OnValueChangedTimer: function() {
        this.valueChangedTimerID = ASPx.Timer.ClearTimer(this.valueChangedTimerID);
        this.RaisePersonalStandardValidation();
        this.InvokeActualOnValueChanged();        
    },
    InvokeActualOnValueChanged: function() {
        ASPxClientButtonEditBase.prototype.OnValueChanged.call(this);
    },

    ChangeEnabledAttributes: function(enabled) {
        ASPxClientButtonEditBase.prototype.ChangeEnabledAttributes.call(this, enabled);
        var btnElement = this.GetIncrementButton();
        if(btnElement)
            this.ChangeButtonEnabledAttributes(btnElement, ASPx.Attr.ChangeAttributesMethod(enabled));
        btnElement = this.GetDecrementButton();
        if(btnElement)
            this.ChangeButtonEnabledAttributes(btnElement, ASPx.Attr.ChangeAttributesMethod(enabled));
        btnElement = this.GetLargeIncrementButton();
        if(btnElement)
            this.ChangeButtonEnabledAttributes(btnElement, ASPx.Attr.ChangeAttributesMethod(enabled));
        btnElement = this.GetLargeDecrementButton();
        if(btnElement)
            this.ChangeButtonEnabledAttributes(btnElement, ASPx.Attr.ChangeAttributesMethod(enabled));
    },
    ChangeEnabledStateItems: function(enabled) {
        this.ClearButtonRepeatClickTimer();

        ASPxClientButtonEditBase.prototype.ChangeEnabledStateItems.call(this, enabled);
        var btnElement = this.GetIncrementButton();
        if(btnElement)
            ASPx.GetStateController().SetElementEnabled(btnElement, enabled);
        btnElement = this.GetDecrementButton();
        if(btnElement)
            ASPx.GetStateController().SetElementEnabled(btnElement, enabled);
        btnElement = this.GetLargeIncrementButton();
        if(btnElement)
            ASPx.GetStateController().SetElementEnabled(btnElement, enabled);
        btnElement = this.GetLargeDecrementButton();
        if(btnElement)
            ASPx.GetStateController().SetElementEnabled(btnElement, enabled);
    },
    OnMouseOver: function(evt) {
        ASPxClientButtonEditBase.prototype.OnMouseOver.call(this);
    },
    Validate: function() {
        this.isValidating = true;
        ASPxClientButtonEditBase.prototype.Validate.call(this);
        this.isValidating = false;
    }
});
var ASPxClientSpinEdit = ASPx.CreateClass(ASPxClientSpinEditBase, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.NumberChanged = new ASPxClientEvent();

        this.decimalPlaces = 0;
        this.number = 0;
        this.inc = 1;
        this.largeInc = 10;
        this.minValue = 0;
        this.maxValue = 0;
        this.showOutOfRangeWarning = true;

        this.allowNull = true;
        this.numberType = "f";
        this.maxLength = 0;

        this.inputValueBeforeFocus = null;
        this.valueChangedProcsCalledBeforeLostFocus = false;
        this.lockValueChanged = false;

        // for paste operations        
        this.pasteTimerID = -1;
        this.keyUpProcessing = false;
        this.isChangingCheckProcessed = false;
    },
    Initialize: function() {
        ASPxClientSpinEditBase.prototype.Initialize.call(this);
        this.filedsInitialize();
        this.GenerateValidationRegExp();
    },
    InlineInitialize: function() {
        this.UpdateLastCorrectValueString();
        ASPxClientSpinEditBase.prototype.InlineInitialize.call(this);
        if (this.UseRestrictions() && this.showOutOfRangeWarning)
            this.EnsureOutOfRangeWarningManager();
    },

    filedsInitialize: function() {
        if(!this.UseRestrictions()) {
            this.minValue = this.GetDefaultMinValue();
            this.maxValue = this.GetDefaultMaxValue();
        }
    },

    GetDefaultMinValue: function () {
        var isDecimal = this.numberType == "f";
        return isDecimal ? ASPxClientSpinEditConsts.DECIMAL_MIN_VALUE :
                ASPxClientSpinEditConsts.INT_32_MIN_VALUE;
    },

    GetDefaultMaxValue: function () {
        var isDecimal = this.numberType == "f";
        return isDecimal ? ASPxClientSpinEditConsts.DECIMAL_MAX_VALUE :
            ASPxClientSpinEditConsts.INT_32_MAX_VALUE;
    },

    GenerateValidationRegExp: function() {
        var decimalSeparator = "";
        var allowDecimalSeparatorSymbols = "";

        if(this.IsFloatNumber()) {
            decimalSeparator = this.GetDecimalSeparatorRegExpString(ASPx.NumberDecimalSeparator);
            for(var i = 0; i < ASPx.PossibleNumberDecimalSeparators.length; i++)
                allowDecimalSeparatorSymbols += this.GetDecimalSeparatorRegExpString(ASPx.PossibleNumberDecimalSeparators[i]);
        }

        this.allowSymbolRegExp = new RegExp("[-0-9" + allowDecimalSeparatorSymbols + "]");

        this.validNumberRegExp = new RegExp("^[-]?[0-9]*[" + decimalSeparator + "]?[0-9]*([0-9]+)?$");

        var validFormattedNumberRegExpString = "^[-]?[0-9]*[" + decimalSeparator + "]?[0-9]";
        if(this.decimalPlaces != 0)
            validFormattedNumberRegExpString += "{0," + this.decimalPlaces + "}$";
        else
            validFormattedNumberRegExpString += "*([0-9]+)?$";

        this.validFormattedNumberRegExp = new RegExp(validFormattedNumberRegExpString);
    },
    GetDecimalSeparatorRegExpString: function(decimalSeparator) {
        return decimalSeparator == "." ? "\\." : decimalSeparator;
    },


    ChangeNumber: function(offset) {
        if(!this.readOnly) {
            var newNumber = this.GetValidNumber(ASPx.CorrectRounding(this.number, offset), this.number);
            if(newNumber != this.number) {
                this.SetNumberInternal(newNumber);
                this.OnValueChanged();
            }
            ASPx.Selection.Set(this.GetInputElement());
        }
    },

    ProcessInternalButtonClick: function(buttonIndex) {
        var ret = false;
        this.ParseValueAfterPaste();
        if(this.largeDecButtonIndex == buttonIndex) {
            this.ChangeNumber(-this.largeInc);
            ret = true;
        } else if(this.incButtonIndex == buttonIndex) {
            this.ChangeNumber(this.inc);
            ret = true;
        } else if(this.decButtonIndex == buttonIndex) {
            this.ChangeNumber(-this.inc);
            ret = true;
        } else if(this.largeIncButtonIndex == buttonIndex) {
            this.ChangeNumber(this.largeInc);
            ret = true;
        }
        return ret;
    },
    GetCorrectNumberInRange: function(number) {
        if(this.UseRestrictions() && number > this.maxValue)
            number = this.maxValue;
        if(this.UseRestrictions() && number < this.minValue)
            number = this.minValue;
        return number;
    },
    GetValidNumber: function(number, oldNumber) {
        var validNumber = 0;
        if(this.UseRestrictions() && number < this.minValue && (this.number == null || number > this.number))
            validNumber = this.minValue;
        else if(this.UseRestrictions() && number > this.maxValue && (this.number == null || number < this.number))
            validNumber = this.maxValue;
        else if((!this.UseRestrictions() || number <= this.maxValue) &&
            (!this.UseRestrictions() || number >= this.minValue))
            validNumber = number;
        else
            validNumber = this.number;
        if(!this.IsFloatNumber())
            validNumber = Math.round(validNumber);
        if((this.maxLength > 0) && (validNumber.toString().length > this.maxLength))
            validNumber = oldNumber;

        return validNumber;
    },
    GetValueType: function() {
        return this.IsFloatNumber() ? "Decimal" : "Int";
    },
    GetFormattedNumber: function(number) {
        if(number == null)
            return "";

        var value = String(number);
        if(ASPx.NumberDecimalSeparator != ".") {
            if(value.indexOf(".") != -1)
                value = value.replace(".", ASPx.NumberDecimalSeparator);
        }
        value = this.GetCorrectFormattedNumberString(value);
        return value;
    },
    IsFloatNumber: function() {
        return this.numberType == "f";
    },

    ConvertToStringWithCorrectSeparator: function(string) {
        for(var i = 0; i < ASPx.PossibleNumberDecimalSeparators.length; i++)
            if(ASPx.PossibleNumberDecimalSeparators[i] !== ASPx.NumberDecimalSeparator)
                string = string.replace(ASPx.PossibleNumberDecimalSeparators[i], ASPx.NumberDecimalSeparator)
        return string;
    },

    ParseValue: function(withSelection) {
        if(!ASPx.IsExists(withSelection))
            withSelection = true;
        if(this.isValidating) //B158934
            withSelection = false;

        var valueString = this.GetInputNumber();
        var newNumber = (valueString != "") ? this.ParseValueInternal(valueString) : null;
        if((newNumber != null) && !isNaN(newNumber)) {
            if(newNumber != this.number) {
                newNumber = this.GetCorrectNumberInRange(newNumber);
                var isEqual = newNumber == this.number;
                this.SetNumberInternal(newNumber, withSelection);
                if(!isEqual)
                    this.OnValueChanged();
            } else
                this.SetFormattedNumberInInput(newNumber, withSelection)
        }
        else {
            if(this.allowNull)
                this.SetNumberInternal(null, withSelection);
            else
                this.SetNumberInternal(this.GetCorrectNumberInRange(0), withSelection);
            this.OnValueChanged();
        }
    },
    ParseValueAfterPaste: function() {
        if(this.isChangingCheckProcessed) {
            this.ParseValue();
            this.isChangingCheckProcessed = false;
        }
    },
    ParseValueOnPaste: function() {
        var newNumber = null;
        var inputElement = this.GetInputElement();
        if(ASPx.IsExistsElement(inputElement)) {
            var valueString = inputElement.value;

            if(valueString != "") {
                valueString = this.ConvertToStringWithCorrectSeparator(valueString);
                if(!this.IsValidNumberString(valueString)) {
                    valueString = this.lastValue;
                    inputElement.value = this.lastValue;
                }
                else
                    this.lastValue = valueString;
            }

            newNumber = (valueString != "") ? this.ParseValueInternal(valueString) : null;
            if(newNumber != null)
                this.SetFormattedNumberInInput(newNumber);
        }
        return newNumber;
    },
    ParseValueInternal: function(value) {
        if(value == null || value.toString() == "")
            return null;

        // correct DecimalSeparator in number string
        if(ASPx.NumberDecimalSeparator != ".") {
            if(value.indexOf(ASPx.NumberDecimalSeparator) != -1)
                value = value.replace(ASPx.NumberDecimalSeparator, ".");
        }
        if(typeof (value) == "number")
            return value;
        return this.ParseNumber(value.toString(), this.GetValueType());
    },
    ParseNumber: function(value, type) {
        if(type == "Decimal")
            return parseFloat(value, 10);
        return parseInt(value, 10);
    },
    RaiseValueChangedEvent: function() {
        return this.OnNumberChanged();
    },
    SetNumberInternal: function(value, withSelection) {
        if(!ASPx.IsExists(withSelection))
            withSelection = true;

        this.number = this.RoundNumber(value);
        this.SetFormattedNumberInInput(this.number, withSelection);
        if(this.HasTextDecorators())
            this.SyncRawValue();
    },
    RoundNumber: function(number) {
        if(this.decimalPlaces <= 0 || number == null)
            return number;
        
        return parseFloat(number.toFixed(this.decimalPlaces));
    },
   
    SetFormattedNumberInInput: function(number, withSelection) {
        if(!ASPx.IsExists(withSelection))
            withSelection = true;

        var inputElement = this.GetInputElement();
        if(inputElement != null) {
            if(withSelection)
                this.UpdateSelectionStartAndEndPosition(inputElement); // for paste
            var formattedNumber = this.GetFormattedNumber(number);
            if(formattedNumber.toString() != inputElement.value) {
                inputElement.value = formattedNumber;
                if(this.HasTextDecorators()) {
                    this.SyncRawValue();
                    if(this.CanApplyTextDecorators() && this.isValidating)
                        inputElement.value = this.GetDecoratedText(formattedNumber);
                }
            }
            if(withSelection) {
                if(ASPx.Browser.VirtualKeyboardSupported && (!this.focused || ASPx.VirtualKeyboardUI.focusableInputElementIsActive(this)))
                    ASPx.VirtualKeyboardUI.smartFocusEditor(this);
                else
                    ASPx.Selection.Set(inputElement, inputElement.selectionStart, inputElement.selectionEnd); // for paste
            }
            this.UpdateLastCorrectValueString();
        }
    },
    UseRestrictions: function() {
        return (this.maxValue != 0 || this.minValue != 0);
    },
    UpdateLastCorrectValueString: function() {
        this.lastValue = this.GetInputNumber();
    },
    GetValue: function() {
        var number = this.GetInputNumber();
        if(!ASPx.IsExists(number))
            return null;
        if(number == "")
            return null;
        else
            return this.number;
    },
    SetValue: function(number) {
        if(typeof(number) === "string" && number !== "")
            number = parseFloat(number);

        this.number = this.RoundNumber(number);
        // Update
        ASPxClientSpinEditBase.prototype.SetValue.call(this, this.GetFormattedNumber(this.number));
    },
    ClearEditorValueByClearButtonCore: function() {
        if(this.allowNull === false) 
            this.SetNumber(0);
        else
            ASPxClientSpinEditBase.prototype.ClearEditorValueByClearButtonCore.call(this);
    },

    OnKeyPress: function(evt) {
        ASPxClientSpinEditBase.prototype.OnKeyPress.call(this, evt);
        if(!this.IsValueChangeAllowed()) return;
        if(this.valueChangedProcsCalledBeforeLostFocus)
            this.valueChangedProcsCalledBeforeLostFocus = false;
        var inputElement = this.GetInputElement();
        if(!inputElement) {
            if(this.pasteTimerID != -1)
                this.ClearTextChangingTimer();
            return;
        }

        this.keyUpProcessing = true;
        if(!ASPx.Browser.IE && ASPx.IsPasteShortcut(evt)) {
            this.keyUpProcessing = false;
            return true;
        }

        if(evt.altKey || evt.ctrlKey)
            return true;

        if(this.IsSpecialKey(evt, false)) {
            this.keyUpProcessing = true;
            return true;
        }
        
        var keyCode = ASPx.Evt.GetKeyCode(evt);

        this.UpdateSelectionStartAndEndPosition(inputElement);
        var selectionStart = inputElement.selectionStart;
        var selectionEnd = inputElement.selectionEnd;

        var pressed = String.fromCharCode(keyCode);
        if(!this.IsAllowableSymbol(pressed) && keyCode !== ASPx.Key.Enter) {
            this.keyUpProcessing = false;
            return ASPx.Evt.PreventEvent(evt); // false
        }

        if(this.IsSignSymbol(pressed)) {
            var isAllowTypeNumberSignSymbol = this.IsAllowTypeNumberSignSymbol(selectionStart, selectionEnd);
            this.keyUpProcessing = isAllowTypeNumberSignSymbol;
            return isAllowTypeNumberSignSymbol ? true : ASPx.Evt.PreventEvent(evt);  // false
        }
        if(this.IsDecimalSeparatorSymbol(pressed)) {
            var isAllowTypeDecimalSeparator = this.IsAllowTypeDecimalSeparatorSymbol(selectionStart, selectionEnd);
            if(isAllowTypeDecimalSeparator)
                this.TypeDecimalSeparator(selectionStart, selectionEnd);

            this.keyUpProcessing = isAllowTypeDecimalSeparator;
            return ASPx.Evt.PreventEvent(evt);
        }
        // Digit symbol
        if(!this.IsAllowTypeDigitToCurrentPosition(selectionStart, selectionEnd, pressed)) {
            this.keyUpProcessing = false;
            return ASPx.Evt.PreventEvent(evt); // false
        }
        return true;
    },
    OnKeyUp: function(evt) {
        ASPxClientSpinEditBase.prototype.OnKeyUp.call(this, evt);
        this.isKeyPressed = false;
        if(this.keyUpProcessing) {
            this.UpdateLastCorrectValueString();
            this.keyUpProcessing = false;
        }
        if(this.IsPageOrArrowKey(evt))
            this.OnPageOrArrowKeyUp();
        if(this.UseRestrictions() && this.outOfRangeWarningManager) {
            var valueString = this.GetInputElement().value;
            var currentNumber = this.ParseValueInternal(valueString);
            this.outOfRangeWarningManager.UpdateOutOfRangeWarningElementVisibility(!isNaN(currentNumber) ? currentNumber : null);
        }
    },
    OnKeyDown: function(evt) {
        this.isKeyPressed = true;
        if(evt.keyCode == ASPx.Key.Enter)
            this.OnTextChanged();

        ASPxClientSpinEditBase.prototype.OnKeyDown.call(this, evt);

        if(this.IsPageOrArrowKey(evt))
            this.OnPageOrArrowKeyDown(evt);

        // ifspecial key is pressed, event OnKeyPress isnt' fired
        if((ASPx.Browser.IE || ASPx.Browser.WebKitFamily) && this.IsSpecialKey(evt, true))
            this.keyUpProcessing = true;
    },
    OnPageOrArrowKeyDown: function(evt) {
        var btnIndex = this.GetButtonIndexByKeyCode(ASPx.Evt.GetKeyCode(evt), evt.ctrlKey);
        if(ASPx.Browser.Opera)
            this.StartButtonRepeatClickTimer(btnIndex, 60, 1);
        else {
            this.ProcessInternalButtonClick(btnIndex);
            ASPx.Evt.PreventEvent(evt);
        }
    },
    OnPageOrArrowKeyUp: function(evt) {
        if(ASPx.Browser.Opera)
            this.ClearButtonRepeatClickTimer();
    },
    IsValueChangeAllowed: function() {
        return !this.readOnly && this.clientEnabled; //B190001
    },
    OnFocus: function() {
        
        if(this.IsValueChangeAllowed()) {
            this.SaveIEOnValueChangedEmulationData();
        }
        ASPxClientSpinEditBase.prototype.OnFocus.call(this);
        
        if(this.IsValueChangeAllowed() && !ASPx.GetControlCollection().InCallback())
            this.SetTextChangingTimer();
    },
    OnLostFocus: function() {
        if(this.isInitialized && this.IsValueChangeAllowed()) {
            this.ClearTextChangingTimer();
            this.EmulateOnValueChanged();
        }
        ASPxClientSpinEditBase.prototype.OnLostFocus.call(this);
        if (this.outOfRangeWarningManager)
            this.outOfRangeWarningManager.HideOutOfRangeWarningElement();
    },
    OnLostFocusCore: function() {
        if(this.lockValueChanged && !this.IsFocusEventsLocked())
            ASPxClientSpinEditBase.prototype.OnValueChanged.call(this);
        ASPxClientSpinEditBase.prototype.OnLostFocusCore.call(this);
    },
    OnNumberChanged: function() {
        var processOnServer = ASPxClientSpinEditBase.prototype.RaiseValueChangedEvent.call(this);
        processOnServer = this.RaiseNumberChanged(processOnServer);

        if(this.focused)
            this.valueChangedProcsCalledBeforeLostFocus = true;

        return processOnServer;
    },
    OnValueChanged: function() {
        if(!this.lockValueChanged)
            ASPxClientSpinEditBase.prototype.OnValueChanged.call(this);
    },

    OnMouseOver: function(evt) {
        ASPxClientSpinEditBase.prototype.OnMouseOver.call(this);
        if(ASPx.Evt.IsLeftButtonPressed(evt) && !this.HasTextDecorators())
            this.OnTextChangingCheck();
    },
    OnMouseWheel: function(evt) {
        if(!this.allowMouseWheel || !this.GetEnabled())
            return;

        this.ParseValue();
        var wheelDelta = ASPx.Evt.GetWheelDelta(evt);
        if(wheelDelta > 0)
            this.ChangeNumber(this.inc);
        else if(wheelDelta < 0)
            this.ChangeNumber(-this.inc);
        if(this.UseRestrictions() && this.outOfRangeWarningManager)
            this.outOfRangeWarningManager.UpdateOutOfRangeWarningElementVisibility(this.GetValue());
        ASPx.Evt.PreventEvent(evt);
    },
    OnTextChangingCheck: function(evt) {
        var curValueString = this.GetInputNumber();
        this.isChangingCheckProcessed = true;

        if((this.lastValue != curValueString) && !this.keyUpProcessing) {
            if(this.isKeyPressed) { // T155125
                this.lastValue = curValueString;
                return;
            }
            var isChromeOnAndroid = ASPx.Browser.AndroidMobilePlatform && ASPx.Browser.Chrome;
            var difIsLastDS = (this.lastValue + ASPx.NumberDecimalSeparator) == curValueString;
            if(!(isChromeOnAndroid && difIsLastDS))
                this.OnPaste();
        }
    },
    OnPaste: function() {
        var newNumber = this.ParseValueOnPaste();
        if (this.UseRestrictions() && this.outOfRangeWarningManager)
            this.outOfRangeWarningManager.UpdateOutOfRangeWarningElementVisibility(newNumber);
    },
    OnTextChanged: function() {
        this.ParseValue(false);// false - B195593
    },

    GetButtonIndexByKeyCode: function(keyCode, ctrl) {
        var ret = 0;
        switch (keyCode) {
            case ASPx.Key.Up:
                ret = ctrl ? this.largeIncButtonIndex : this.incButtonIndex;
                break;
            case ASPx.Key.Down:
                ret = ctrl ? this.largeDecButtonIndex : this.decButtonIndex;
                break;
            case ASPx.Key.PageUp:
                ret = this.largeIncButtonIndex;
                break;
            case ASPx.Key.PageDown:
                ret = this.largeDecButtonIndex;
                break;
        }
        return ret;
    },

    // IE ValueChanged emulation
    SaveIEOnValueChangedEmulationData: function() {
        this.valueChangedProcsCalledBeforeLostFocus = false;
        this.inputValueBeforeFocus = this.GetInputNumber();
    },
    EmulateOnValueChanged: function() {
        if(!this.valueChangedProcsCalledBeforeLostFocus) {
            if(this.GetInputNumber() != this.inputValueBeforeFocus) {
                this.OnTextChanged();
                this.RaiseValidationInternal();
                this.RaisePersonalStandardValidation();
            }
        }
    },

    // Timer utils
    SetTextChangingTimer: function() {
        if(this.pasteTimerID === -1)
            this.pasteTimerID = ASPx.Timer.SetControlBoundInterval(this.OnTextChangingCheck, this, ASPx.PasteCheckInterval);
    },
    ClearTextChangingTimer: function() {
        this.pasteTimerID = ASPx.Timer.ClearInterval(this.pasteTimerID);
    },

    // Input correction utils
    IsAllowableSymbol: function(symbol) {
        return this.allowSymbolRegExp.test(symbol);
    },
    IsAllowTypeNumberSignSymbol: function(selectionStart, selectionEnd) {
        var curValueString = this.GetInputElement().value.toString();
        if((curValueString != null) && this.IsSignSymbol(curValueString.charAt(0)))
            return (selectionStart == 0) && (selectionEnd > 0);
        else
            return (selectionStart == 0);
    },
    IsAllowTypeDecimalSeparatorSymbol: function(selectionStart, selectionEnd) {
        if(this.maxLength > 0 && selectionStart >= this.maxLength)
            return false;
        var unselectedText = this.GetUnselectedText(selectionStart, selectionEnd);
        var decimalSepIndex = unselectedText.indexOf(ASPx.NumberDecimalSeparator);
        if((this.decimalPlaces != 0) && (decimalSepIndex == -1)) {
            var possibleValueString = this.GetPossibleValueString(selectionStart, selectionEnd, ASPx.NumberDecimalSeparator);
            return this.IsValidFormattedNumberString(possibleValueString);
        }
        return decimalSepIndex == -1;
    },
    IsAllowTypeDigitToCurrentPosition: function(selectionStart, selectionEnd, pressedSymbol) {
        var possibleValueString = this.GetPossibleValueString(selectionStart, selectionEnd, pressedSymbol);
        var decimalSepIndex = possibleValueString.indexOf(ASPx.NumberDecimalSeparator);

        if((this.decimalPlaces != 0) && (decimalSepIndex != -1))
            return this.IsValidFormattedNumberString(possibleValueString);

        return true;
    },
    IsValidNumberString: function(numString) {
        return this.validNumberRegExp.test(numString);
    },
    IsValidFormattedNumberString: function(numString) {
        return this.validFormattedNumberRegExp.test(numString);
    },
    GetCorrectFormattedNumberString: function(numString) {
        var ret = numString;
        if(!this.IsValidFormattedNumberString(numString)) {
            // number with exponent
            if(numString.toLowerCase().indexOf("e") > -1)
                numString = ASPxClientSpinEdit.RemoveExponentialNotation(numString);

            var decimalSepIndex = numString.indexOf(ASPx.NumberDecimalSeparator);
            if(decimalSepIndex > -1) {
                ret = numString.substring(0, decimalSepIndex);
                if(this.IsFloatNumber()) {
                    if(this.decimalPlaces > 0)
                        ret += numString.substr(decimalSepIndex, this.decimalPlaces + 1);
                    else
                        ret += numString.substr(decimalSepIndex);
                }
            }
            else
                ret = numString;

        }
        return ret;
    },

    GetPossibleValueString: function(selectionStart, selectionEnd, pressedSymbol) {
        var curValueString = this.GetInputElement().value.toString();

        var newValueString = curValueString.substring(0, selectionStart);
        newValueString += pressedSymbol;

        var selectionLength = selectionEnd - selectionStart;
        newValueString += curValueString.substr(selectionEnd, curValueString.length - selectionLength);

        return newValueString;
    },
    GetUnselectedText: function (selectionStart, selectionEnd) {
        return this.GetPossibleValueString(selectionStart, selectionEnd, "");
    },
    IsDecimalSeparatorSymbol: function(symbol) {
        for(var i = 0; i < ASPx.PossibleNumberDecimalSeparators.length; i++)
            if(ASPx.PossibleNumberDecimalSeparators[i] == symbol)
                return true;
        return false;
    },
    IsValidMinMaxValue: function(minValue, maxValue) {
        if(typeof (maxValue) != "number")
            maxValue = this.ParseValueInternal(maxValue.toString());
        if(typeof (minValue) != "number")
            minValue = this.ParseValueInternal(minValue.toString());
        return (isNaN(maxValue) || isNaN(minValue)) ? false : (maxValue >= minValue);
    },
    // Keyboard utils
    IsSpecialKey: function(evt, inKeyDown) {
        var keyCode = ASPx.Evt.GetKeyCode(evt);
        return keyCode == 0 || keyCode == ASPx.Key.Backspace || keyCode == ASPx.Key.Tab ||
            (inKeyDown && keyCode == ASPx.Key.Delete) || keyCode > 60000 /*Saffari*/;
    },
    IsPageOrArrowKey: function(evt) {
        var keyCode = ASPx.Evt.GetKeyCode(evt);
        // hack in opera
        if(ASPx.Browser.Opera && evt.ctrlKey && (keyCode == ASPx.Key.Up || keyCode == ASPx.Key.Down))
            return false;
        else
            return keyCode == ASPx.Key.Up || keyCode == ASPx.Key.Down ||
                    keyCode == ASPx.Key.PageUp || keyCode == ASPx.Key.PageDown;
    },
    IsSignSymbol: function(symbol) {
        return symbol == "-";
    },

    TypeDecimalSeparator: function(selectionStart, selectionEnd) {
        var possibleValueString = this.GetPossibleValueString(selectionStart, selectionEnd, ASPx.NumberDecimalSeparator);
        var inputElement = this.GetInputElement();
        inputElement.value = possibleValueString;
        var decimalSepIndex = possibleValueString.indexOf(ASPx.NumberDecimalSeparator);
        ASPx.Selection.SetCaretPosition(inputElement, decimalSepIndex + 1); // for paste
    },
    UpdateSelectionStartAndEndPosition: function(inputElement) {
        if(ASPx.Browser.IE && ASPx.Browser.Version < 9 && document.selection) {
            inputElement.selectionStart = 0;
            inputElement.selectionEnd = 0;

            var curRange = document.selection.createRange();
            var copyRange = curRange.duplicate();

            curRange.move('character', -inputElement.value.length);
            curRange.setEndPoint('EndToStart', copyRange);
            inputElement.selectionStart = curRange.text.length;
            inputElement.selectionEnd = inputElement.selectionStart + copyRange.text.length;
        }
    },

    GetInputNumber: function() {
        if(!this.focused && this.HasTextDecorators())
            return this.GetRawValue();

        var element = this.GetInputElement();
        return ASPx.IsExistsElement(element) ? element.value : null;
    },
    GetDecoratedText: function(value) {
        if(typeof value == "string" && !!this.displayFormat)
            value = this.ParseValueInternal(value);
        var result = ASPxClientSpinEditBase.prototype.GetDecoratedText.call(this, value).toString();
        if(!this.displayFormat && ASPx.NumberDecimalSeparator !== ".") 
            result = result.replace('.', ASPx.NumberDecimalSeparator);
        return result;
    },
    EnsureOutOfRangeWarningManager: function() {
        this.ensureOutOfRangeWarningManager(this.minValue, this.maxValue, this.GetDefaultMinValue(), this.GetDefaultMaxValue());
    },

    // API
    RaiseNumberChanged: function(processOnServer) {
        if(!this.NumberChanged.IsEmpty()) {
            var args = new ASPxClientProcessingModeEventArgs(processOnServer);
            this.NumberChanged.FireEvent(this, args);
            processOnServer = args.processOnServer;
        }
        return processOnServer;
    },
    SetNumber: function(number) {
        this.SetValue(number);
    },
    GetNumber: function() {
        return this.number;
    },
    GetText: function () {
        if(this.maskInfo != null)
            return this.maskInfo.GetText();
        else {
            var isNullText = this.number === null && this.nullText !== "";
            var inputElementText = this.GetInputNumber();
            return isNullText ? "" : this.GetDecoratedText(inputElementText).toString();
        }
    },
    SetText: function(text) {
        ASPxClientSpinEditBase.prototype.SetValue.call(this, text);
        this.ParseValue(false);
    },
    SetMinValue: function(value) {
        if(this.IsValidMinMaxValue(value, this.maxValue)) {
            this.minValue = this.ParseValueInternal(value.toString());
            this.EnsureCurrentNumberInBoundries();
            if (this.showOutOfRangeWarning) {
                this.EnsureOutOfRangeWarningManager();
                this.outOfRangeWarningManager.SetMinValue(this.minValue);
            }
        }
    },
    GetMinValue: function() {
        return this.minValue;
    },
    SetMaxValue: function(value) {
        if(this.IsValidMinMaxValue(this.minValue, value)) {
            this.maxValue = this.ParseValueInternal(value.toString());
            this.EnsureCurrentNumberInBoundries();
            if (this.showOutOfRangeWarning) {
                this.EnsureOutOfRangeWarningManager();
                this.outOfRangeWarningManager.SetMaxValue(this.maxValue);
            }
        }
    },
    GetMaxValue: function() {
        return this.maxValue;
    },
    EnsureCurrentNumberInBoundries: function() {
        var value = this.GetValue();
        if(value != null)
            this.SetNumber(this.GetCorrectNumberInRange(value));
    },
    // ** Only for ASPxHtmlEditor **
    GetParsedNumber: function() {
        var inputElement = this.GetInputElement();
        var valueString = inputElement.value;
        var newNumber = valueString != "" ? this.ParseValueInternal(valueString) : null;
        if((newNumber != null) && !isNaN(newNumber)) {
            if(newNumber != this.number)
                newNumber = this.GetCorrectNumberInRange(newNumber);
        } else
            newNumber = this.GetCorrectNumberInRange(0);
        return newNumber;
    },
    RestoreSelectionStartAndEndPosition: function() {
        var inputElement = this.GetInputElement();
        ASPx.Selection.Set(inputElement, inputElement.selectionStart, inputElement.selectionEnd);
    },
    SaveSelectionStartAndEndPosition: function() {
        this.UpdateSelectionStartAndEndPosition(this.GetInputElement());
    }
});
ASPxClientSpinEdit.Cast = ASPxClientControl.Cast;

var ASPxClientSpinEditConsts = {
    INT_32_MAX_VALUE: 2147483647,
    INT_32_MIN_VALUE: -2147483648,
    DECIMAL_MIN_VALUE: -79228162514264337593543950335,
    DECIMAL_MAX_VALUE: 79228162514264337593543950335
};

ASPxClientSpinEdit.RemoveExponentialNotation = function(numString) {
    var mantissaPossition = numString.toLowerCase().indexOf("e");
    var ret = numString;
    if(mantissaPossition > -1) {
        var isNegative = numString.indexOf("-") == 0;
        var isNegativeMantissa = numString.lastIndexOf("-") > 0;

        var mantissa = numString.replace(new RegExp('^[+-]?([0-9]*\.?[0-9]+|[0-9]+\.?[0-9]*)([eE][+-]?)'), "");
        var numberBase = numString.replace(new RegExp('([eE][+-]?[0-9]+)'), "");
        numberBase = numberBase.replace("+", "");
        numberBase = numberBase.replace("-", "");

        var decimalDecimalSeparator = ".";
        if(numberBase.indexOf(decimalDecimalSeparator) == -1) {
            numberBase = numberBase.replace(decimalDecimalSeparator, ASPx.NumberDecimalSeparator);
            decimalDecimalSeparator = ASPx.NumberDecimalSeparator;
        }

        var numberParts = numberBase.split(decimalDecimalSeparator);
        if(numberParts.length == 1)
            numberParts[1] = "";

        var zeroNumbers = parseInt(mantissa, 10);

        if(isNegativeMantissa) {
            zeroNumbers -= 1;
            ret = "0" + ASPx.NumberDecimalSeparator + ASPxClientSpinEdit.GetZeros(zeroNumbers) +
                numberParts[0] + numberParts[1];
        }
        else {
            zeroNumbers -= numberParts[0].length + numberParts[1].length - 1;
            ret = numberParts[0] + numberParts[1] + ASPxClientSpinEdit.GetZeros(zeroNumbers);
        }
        if(isNegative)
            ret = ASPx.Str.Insert(ret, "-", 0);
    }
    return ret;
}
ASPxClientSpinEdit.GetZeros = function(length) {
    var zeros = [];
    for(var i = 0; i < length; i++)
        zeros.push('0');
    return zeros.join("");
}
var ASPxClientTimeEdit = ASPx.CreateClass(ASPxClientSpinEditBase, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.date = null;
        this.DateChanged = new ASPxClientEvent();
        this.InternalValueChanging = new ASPxClientEvent();
        this.OwnerDateEdit = null;
        this.allowNull = true;
        this.deleteAllProcessing = false;
        this.isTemporaryNull = false;
        this.isASPxClientTimeEdit = true;
    },
    
    Initialize: function() {
        ASPxClientSpinEditBase.prototype.Initialize.call(this);
        this.SubscribeInputElementClick();
    },

    SetNullStateValue: function(value) {
        this.isTemporaryNull = value;
        this.ApplyNullTextClassName(value);
        this.EnsureClearButtonVisibility();
    },

    ApplyNullTextClassName: function (value) {
        if(this.styleDecoration)
            this.styleDecoration.ApplyNullTextClassName(value);
    },
    
    SubscribeInputElementClick: function() {
        var editor = this;
        ASPx.Evt.AttachEventToElement(this.GetInputElement().parentNode, "click", function(){
            editor.FillMaskInfo();
			if(editor.maskInfo.ApplyFixes(null))
				editor.ApplyMaskInfo(false);
        });
    },

    ApplyMaskInfo: function(applyCaret) { 
        var input = this.GetInputElement();
        var prev =  input.value;
        ASPxClientSpinEditBase.prototype.ApplyMaskInfo.call(this, applyCaret);
        if(prev != input.value)
            this.RaiseInternalValueChanging(ASPx.MaskDateTimeHelper.GetDate(this.maskInfo));
	},
    SetDate: function(date) {
        this.SetValue(date);
    },
    GetDate: function() {
        return this.date ? new Date(this.date.valueOf()) : null;
    },
    
    SetValue: function(date) {
        this.date = date;
		ASPx.MaskDateTimeHelper.SetDate(this.maskInfo, date);
		this.ApplyMaskInfo(false);
		this.SavePrevMaskValue();
		if (this.styleDecoration)
		    this.styleDecoration.Update();
    },
    
    GetValue: function() {
        return this.date;
    },
    GetLastSuccesfullValue: function() {
        return this.GetValue();
    },
    
    OnClear: function() {
        this.SetNullStateValue(true);
        ASPxClientButtonEditBase.prototype.OnClear.call(this);
        this.lastInputValue = this.GetEmptyValueDisplayText();
    },

    ClearEditorValueByClearButtonCore: function() {
        if(this.allowNull === false) {
            this.GetInputElement().value = '';
            this.ParseValue();
        } else
            ASPxClientSpinEditBase.prototype.ClearEditorValueByClearButtonCore.call(this);
    },

    RequireShowClearButtonCore: function() {
        return !this.isTemporaryNull || ASPxClientButtonEditBase.prototype.RequireShowClearButtonCore.call(this);
    },

    GetValueString: function() {
        return this.date != null ? ASPx.DateUtils.GetInvariantDateTimeString(this.date) : null;
    },
    
    ProcessInternalButtonClick: function(buttonIndex) {
        var delta;
        var result = false;
        if(buttonIndex == this.incButtonIndex) {
            delta = 1;
            result = true;
        } else if(buttonIndex == this.decButtonIndex) {
            delta = -1;
            result = true;
        }
        if(result && !this.focused && !this.IsFocusEventsLocked()) // TODO check
            this.EnsureNullState();
        if(result && !this.readOnly) {
            ASPx.MaskManager.OnMouseWheel(this.maskInfo, delta);
            this.ApplyMaskInfo(true);
            this.SetNullStateValue(false);
        }
        if(ASPx.Browser.VirtualKeyboardSupported) {
            if(!this.focused || ASPx.VirtualKeyboardUI.focusableInputElementIsActive(this)) // for TimeEdit belongs to DateEdit
                ASPx.VirtualKeyboardUI.smartFocusEditor(this);
        } else {
            this.SetFocus();
        }
        return result;
    },
    
    ParseValue: function () {
        var newDate = null;
        var needRollbackValue = !this.allowNull && this.isTemporaryNull;
        if(needRollbackValue)
            newDate = this.date;
        else {
            if(!this.focused && this.isValidating && this.allowNull)
                this.LoadNullState();
            newDate = this.isTemporaryNull ? null : ASPx.MaskDateTimeHelper.GetDate(this.maskInfo, this.date, true);
        }
        var changed = !ASPx.DateUtils.AreDatesEqualExact(this.date, newDate);
        this.SetValue(newDate);
        var forceValueChanged = this.IsValueChangeForced() && !this.IsValueChanging();
        if(changed || forceValueChanged) {
            this.StartValueChanging();
            this.RaisePersonalStandardValidation();
            this.OnValueChanged();
            this.EndValueChanging();
        }
    },
    
    OnTextChanged: function() {
        this.ParseValue();
    },
    
	RaiseInternalValueChanging: function(date) {
        if(!this.InternalValueChanging.IsEmpty())
            this.InternalValueChanging.FireEvent(this, date); 
	},

    OnMouseWheel: function(evt) {
        if(!this.allowMouseWheel || !this.GetEnabled())
            return;
        ASPxClientTextEdit.prototype.OnMouseWheel.call(this, evt);
    },
       
    InvokeActualOnValueChanged: function() {
        // we skip text-edit implementation as ASPxDateEdit does
        ASPxClientEdit.prototype.OnValueChanged.call(this);
    },
    
    RaiseValueChangedEvent: function() {
        if(!this.isInitialized) return false;
        var processOnServer = ASPxClientEdit.prototype.RaiseValueChangedEvent.call(this);
        processOnServer = this.RaiseDateChanged(processOnServer);
        return processOnServer;
    },
    
    RaiseDateChanged: function(processOnServer) {
        if(!this.DateChanged.IsEmpty()) {
            var args = new ASPxClientProcessingModeEventArgs(processOnServer);
            this.DateChanged.FireEvent(this, args);
            processOnServer = args.processOnServer;
        }
        return processOnServer;
    },
    
    GetMaskDisplayText: function() {
		if(!this.focused) {
		    if(this.date == null)
				return this.nullText;
			if(this.HasTextDecorators())
				return this.GetDecoratedText(this.date);
		}
		return this.maskInfo.GetText();
    },
    
    HasTextDecorators: function() {
		return this.date == null || ASPxClientSpinEditBase.prototype.HasTextDecorators.call(this);
    },
    
    SyncRawValue: function() {
		this.SetRawValue(this.date == null ? "N" : ASPx.DateUtils.ToLocalTime(this.date).valueOf());
    },
    
    DecodeRawInputValue: function(value) {
		if(value == "N") return null;
		var date = new Date();
		date.setTime(Number(value));
		var result = ASPx.DateUtils.ToUtcTime(date);
        var offsetDiff = result.getTimezoneOffset() - date.getTimezoneOffset();
        if(offsetDiff !== 0)
            result.setTime(result.valueOf() + offsetDiff * 60000);
		return result;
    },
    
    BeginShowMaskHint: function() {
    },

    RequireTrackNullState: function() {
        return !this.readOnly;
    },

    LoadNullState: function() {
        this.SetNullStateValue(this.GetValue() === null);
    },

    EnsureNullState: function () {
        if(this.nullStatePasteTimer > -1)
            return;
        this.LoadNullState();
        if(!this.RequireTrackNullState())
            return;
        this.CreateNullStatePasteTimer();
    },

    CreateNullStatePasteTimer: function() {
        this.nullStatePasteTimer = ASPx.Timer.SetControlBoundInterval(this.OnTextChangingCheck, this, ASPx.PasteCheckInterval);
        this.lastInputValue = this.GetInputElement().value;
    },
    ClearNullStatePasteTimer: function() {
        this.nullStatePasteTimer = ASPx.Timer.ClearInterval(this.nullStatePasteTimer);
    },
    OnTextChangingCheck: function () {
        if(this.GetInputElement().value === this.lastInputValue)
            return;
        if(this.deleteAllProcessing)
            return;
        this.SetNullStateValue(false);
        this.lastInputValue = this.GetInputElement().value;
    },

    OnFocus: function () {
        var forceEnsureNullState = !this.IsFocusEventsLocked()

        ASPxClientSpinEditBase.prototype.OnFocus.call(this);

        if(forceEnsureNullState)
            this.EnsureNullState();
    },
    
    OnLostFocusCore: function() {        
        if(!this.IsFocusEventsLocked())
            this.RaiseStandardOnChange();
        ASPxClientEdit.prototype.OnLostFocusCore.call(this);
	    this.EndShowMaskHint();
	    this.HideMaskHint();	    
	    if(!this.IsFocusEventsLocked()) {
            if(this.maskInfo.ApplyFixes(null))
                this.ApplyMaskInfo(false);
            this.ClearNullStatePasteTimer();
            this.ApplyNullTextClassName(false);
	    }
	    this.ToggleTextDecoration();
        this.EnsureClearButtonVisibility();
        this.EnsureHidePopupHelpText();
    },
    
    IsDigit: function(symbol) {
        return new RegExp("[0-9]").test(symbol);
    },
    GetInputSelectionInfo: function() {
        var input = this.GetInputElement();
        return ASPx.Selection.GetExtInfo(input);
    },
    IsAllTextSelected: function () {
        var inputSelectionInfo = this.GetInputSelectionInfo();
        var inputSelectionLength = inputSelectionInfo.endPos - inputSelectionInfo.startPos;
        return inputSelectionLength === this.maskInfo.GetSize();
    },
    
    ProcessKeyInputToEnsureNotNullValue: function(keyEvent) {
        if(!this.RequireTrackNullState() || !this.isTemporaryNull)
            return;
        if(this.NeedResetNullStateOnKeyEvent(keyEvent))
            this.SetNullStateValue(false);
    },
    NeedResetNullStateOnKeyEvent: function(keyEvt) {
        if(keyEvt.type === "keydown" && ASPx.IsPasteShortcut(keyEvt))
            return true;
        if(keyEvt.type === "keypress") {
            var inputValue = this.GetInputElement().value;
            var selInfo = this.GetInputSelectionInfo();
            var startPosSymbol = inputValue.charAt(selInfo.startPos);
            var keySymbol = String.fromCharCode(ASPx.Evt.GetKeyCode(keyEvt));
            return this.IsDigit(keySymbol) && keySymbol === startPosSymbol;
        }
        return false;
    },    
    OnKeyDown: function (evt) {
        this.ProcessKeyInputToEnsureNotNullValue(evt);
        this.ProcessDeleteAllOnKeyDown(evt);
        ASPxClientSpinEditBase.prototype.OnKeyDown.call(this, evt);
    },
    OnKeyUp: function (evt) {
        ASPxClientSpinEditBase.prototype.OnKeyUp.call(this, evt);
        this.ProcessDeleteAllOnKeyUp(evt);
    },
    OnKeyPress: function (evt) {
        this.ProcessKeyInputToEnsureNotNullValue(evt);
        ASPxClientSpinEditBase.prototype.OnKeyPress.call(this, evt);
    },
    ProcessDeleteAllOnKeyDown: function(keyEvent) {
        if(!this.RequireTrackNullState())
            return;
        if(!this.IsAllTextSelected())
            return;
        var keyCode = ASPx.Evt.GetKeyCode(keyEvent);
        if(keyCode === ASPx.Key.Delete || keyCode === ASPx.Key.Backspace)
            this.deleteAllProcessing = true;
    },
    ProcessDeleteAllOnKeyUp: function(keyEvent) {
        if(!this.RequireTrackNullState())
            return;
        if(!this.deleteAllProcessing)
            return;
        var keyCode = ASPx.Evt.GetKeyCode(keyEvent);
        if(keyCode === ASPx.Key.Delete || keyCode === ASPx.Key.Backspace) {
            this.SetNullStateValue(true);
            this.lastInputValue = this.GetInputElement().value;
            this.deleteAllProcessing = false;
        }
    }
});
ASPxClientTimeEdit.Cast = ASPxClientControl.Cast;

ASPx.Ident.IsASPxClientTimeEdit = function(obj) {
    return !!obj.isASPxClientTimeEdit;
};

// SpinEdit collection
var spinEditCollection = null;
function aspxGetSpinEditCollection() {
    if(spinEditCollection == null)
        spinEditCollection = new ASPxClientSpinEditCollection();
    return spinEditCollection;
}

var ASPxClientSpinEditCollection = ASPx.CreateClass(ASPxClientControlCollection, {
    constructor: function() {
        this.constructor.prototype.constructor.call(this);
    },
    GetCollectionType: function(){
        return "SpinEdit";
    },
    GetSpinEdit: function(id) {
        return this.Get(this.GetSpinEditName(id));
    },
    GetSpinEditName: function(id) {
        var pos = id.lastIndexOf(spindButtonIdPostfix);
        if(pos > -1)
            return id.substring(0, pos);

        pos = id.lastIndexOf(ASPx.TEInputSuffix);
        if(pos > -1)
            return id.substring(0, pos);

        return id;
    }
});

ASPx.SEMouseOver = function(name, evt) {
    var edit = ASPx.GetControlCollection().Get(name);
    if(edit != null) edit.OnMouseOver(evt);
}

window.ASPxClientSpinEditBase = ASPxClientSpinEditBase;
window.ASPxClientSpinEdit = ASPxClientSpinEdit;
window.ASPxClientTimeEdit = ASPxClientTimeEdit;
})();