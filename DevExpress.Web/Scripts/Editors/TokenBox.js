/// <reference path="..\_references.js"/>

(function() {
var tokensHiddenFieldSuffix = "TK";
var tokensValuesHiddenFieldSuffix = "TKV";
var tokenBoxTokenSuffix = "Token";
var tokenBoxTokenTextSuffix = "TokenT";
var tokenBoxTokenRemoveButtonSuffix = "TokenRB";
var tokenBoxInputMinWidth = 10;
var ASPxClientTokenBox = ASPx.CreateClass(ASPxClientComboBox, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.hoverTokenClasses = [""];
        this.hoverTokenCssArray = [""];
        this.hoverTokenRemoveButtonClasses = [""];
        this.hoverTokenRemoveButtonCssArray = [""];
        this.valueSeparator = ',';
        this.textSeparator = ',';
        this.showDropDownOnFocus = "Always";
        this.allowDuplicateTokens = false;
        this.editorTimer = null;
        this.encodeHtml = true;
        this.TokensChanged = new ASPxClientEvent();
        this.lastInputText = "";
        this.elementsInitialized = false;
        this.visibilityTimerID = -1;
        this.caseSensitiveTokens = true;
    },
    Initialize: function() {
        if(this.IsVisible()) {
            this.InternalInitialize();
        } else {
            this.elementsInitialized = false;
            this.SetVisibilityTimer();
        }
        ASPxClientComboBox.prototype.Initialize.call(this);
    },
    InternalInitialize: function() {
        this.AdjustSampleToken();
        this.SyncHtmlTokensElementsWithTokenCollection(); //T147690
        this.AdjustTokens(true);
        this.AdjustInput(false);
		this.generateHoverStateItems();
        this.elementsInitialized = true;
    },
    InlineInitialize: function() {
        this.initSelectedIndex = -1;
        this.initTextCorrectionRequired = false;
        ASPxClientComboBox.prototype.InlineInitialize.call(this);
    },
    SyncHtmlTokensElementsWithTokenCollection: function(){
        var tokens = this.GetTokenCollection();
        for(var i = 0; i < tokens.length; i++) {
            if(!this.GetTokenHtmlElement(i)){
                this.AddTokenHtmlElementInternal(i, tokens[i], true);
            }
        }
    },
    AdjustControl: function() {  // TODO: Refactor for resizing and adjusting
        ASPxClientComboBox.prototype.AdjustControl.call(this);
        if(this.IsVisible() && this.isInitialized) {
            this.AdjustSampleToken();
            this.AdjustTokens(false);
            this.AdjustInput(false);
        }
    },
    AdjustSampleToken: function() {
        var sizes = this.GetTokenAndTokenTextMaxWidth();
        var sampleToken = this.GetSampleTokenElement();
        this.SetTokenElementStylesInternal(sampleToken, sizes);
    },
    AdjustTokens: function(needTokenHover) {
        var sizes = this.GetTokenAndTokenTextMaxWidth();
        var tokens = this.GetTokenCollection();
        for(var i = 0; i < tokens.length; i++) {
            var tokenElement = this.GetTokenHtmlElement(i);
            this.SetTokenElementStylesInternal(tokenElement, sizes);
        }
    },
    SetTokenElementStylesInternal: function(tokenElement, sizes) {
        tokenElement.style.maxWidth = sizes.tokenMaxWidth + "px";
        ASPx.SetStyles(tokenElement.childNodes[0], {
            textOverflow: "ellipsis",
            whiteSpace: "nowrap",
            overflow: "hidden"
        });
        tokenElement.childNodes[0].style.maxWidth = sizes.tokenTextMaxWidth + "px";
        tokenElement.childNodes[0].style.maxHeight = "none";
    },
    generateHoverStateItems: function() {
        var tokens = this.GetTokenCollection();
        for(var i = 0; i < tokens.length; i++) {
            var tokenElement = this.GetTokenHtmlElement(i);
            this.SetTokenHover(tokenElement, true);
        }
    },
    SetTokenHover: function(tokenElement, state) {
        if(ASPx.Browser.WebKitTouchUI) return;
        var controller = ASPx.GetStateController();
        if(state) {
            controller.AddHoverItem(tokenElement.id, this.hoverTokenClasses, this.hoverTokenCssArray, "", null, null, true);
            controller.AddHoverItem(tokenElement.childNodes[1].id, this.hoverTokenRemoveButtonClasses, this.hoverTokenRemoveButtonCssArray, "", null, null, true);
        } else {
            controller.RemoveHoverItem(tokenElement.id);
            controller.RemoveHoverItem(tokenElement.childNodes[1].id);
        }
    },
    GetTokenHtmlElement: function(index) {
        return document.getElementById(this.name + "_" + tokenBoxTokenSuffix + index);
    },
    GetTokenTextHtmlElement: function(index) {
        return document.getElementById(this.name + "_" + tokenBoxTokenTextSuffix + index)
    },
    GetTokenRemoveButtonHtmlElement: function(index) {
        return document.getElementById(this.name + "_" + tokenBoxTokenRemoveButtonSuffix + index)
    },
    GetTokenAndTokenTextMaxWidth: function() {
        var sampleToken = this.GetSampleTokenElement();
        var input = this.GetInputElement();
        var inputParentWidth = ASPx.GetClearClientWidth(input.parentElement);
        if(inputParentWidth <= 0)
            this.SetVisibilityTimer();
        var testSampleToken = sampleToken.cloneNode(true);
        testSampleToken.style.position = "absolute";
        testSampleToken.style.top = ASPx.InvalidPosition + "px";
        testSampleToken.style.left = ASPx.InvalidPosition + "px";
        testSampleToken.childNodes[0].innerHTML = "";
        testSampleToken.id += "_testSampleToken";
        document.body.appendChild(testSampleToken);
        testSampleToken.style.display = "";
        var testSampleTokenFullNullWidth = ASPx.GetLeftRightBordersAndPaddingsSummaryValue(testSampleToken) + ASPx.GetLeftRightMargins(testSampleToken);
        var testSampleTokenTextSpanFullNullWidth = ASPx.GetLeftRightBordersAndPaddingsSummaryValue(testSampleToken.childNodes[0]) + ASPx.GetLeftRightMargins(testSampleToken.childNodes[0]);
        var testSampleTokenRBFullWidth = testSampleToken.childNodes[1].offsetWidth + ASPx.GetLeftRightMargins(testSampleToken.childNodes[1]);
        var tokenMaxWidth = inputParentWidth - testSampleTokenFullNullWidth;
        var tokenTextMaxWidth = tokenMaxWidth - (testSampleTokenTextSpanFullNullWidth + testSampleTokenRBFullWidth);
        document.body.removeChild(testSampleToken);
        return {"tokenMaxWidth": tokenMaxWidth, "tokenTextMaxWidth": tokenTextMaxWidth};
    },
    getInputElementHeight: function() {
        var inputElementHeight = ASPxClientComboBox.prototype.getInputElementHeight.call(this);

        var tokens = this.GetTokenCollection(),
            inputElement = this.GetInputElement(),
            inputStyle = ASPx.GetCurrentStyle(inputElement),
            inputRectTop = inputElement.getBoundingClientRect().top + ASPx.PxToFloat(inputStyle.marginTop);
        for(var i = 0; i < tokens.length; i++) {
            var tokenElement = this.GetTokenHtmlElement(i),
                tokenStyle = ASPx.GetCurrentStyle(tokenElement),
                tokenRect = tokenElement.getBoundingClientRect(),
                tokenRectBottom = tokenRect.bottom + ASPx.PxToFloat(tokenStyle.marginBottom);
            if(tokenRectBottom <= inputRectTop)
                inputElementHeight -= tokenRect.height + ASPx.GetTopBottomMargins(tokenElement, tokenStyle);
        }

        return inputElementHeight;
    },
    GetTokensHiddenField: function() {
        return document.getElementById(this.name + "_" + tokensHiddenFieldSuffix);
    },
    GetTokensValuesHiddenField: function() {
        return document.getElementById(this.name + "_" + tokensValuesHiddenFieldSuffix);
    },
    GetTokenCollection: function() {
        var tokensHiddenField = this.GetTokensHiddenField();
        return ASPx.Json.Eval(tokensHiddenField.value.toString(), this.name);
    },
    // INFO GetTokenValuesCollection method isn't public but it is used from the outside (xtrareports)
    GetTokenValuesCollection: function() {
        var tokensValuesHiddenField = this.GetTokensValuesHiddenField();
        return ASPx.Json.Eval(tokensValuesHiddenField.value.toString(), this.name);
    },
    SetTokenCollection: function(collection) {
        this.ClearTokenCollection();
        for(var i = 0; i < collection.length; i++) {
            this.AddTokenInternal(collection[i], true);
        }
    },
    SetTokenCollectionInternal: function(tokenCollection, tokenValueCollection, callByApi) {
        var tokenCollectionString = ASPx.Json.ToJson(ASPx.Data.GetDistinctArray(tokenCollection));
        var tokenValueCollectionString = ASPx.Json.ToJson(ASPx.Data.GetDistinctArray(tokenValueCollection));
        var tokensHiddenField = this.GetTokensHiddenField();
        var tokensValueHiddenField = this.GetTokensValuesHiddenField();
        tokensHiddenField.value = tokenCollectionString;
        tokensValueHiddenField.value = tokenValueCollectionString;
        if(!callByApi) {
            if(this.OnTokensChanged())
                this.SendPostBackInternal("");
        }
        if(this.HasTextDecorators() && this.CanApplyNullTextDecoration()) {
            this.ToggleTextDecoration();
            if(this.styleDecoration) {
                this.styleDecoration.Update();
            }
        }
        this.SyncVisibleItems();
    },
    SyncVisibleItems: function() {
        var lb = this.GetListBoxControl();
        if(lb) {
            var itemCount = lb.GetItemCount();
            for(var i = 0; i < itemCount; i++) {
                if(this.isContainedInTokens(lb.GetItem(i).text, false))
                    ASPx.Attr.ChangeStyleAttribute(lb.GetItemRow(i), "display", "none");
                else
                    ASPx.Attr.RestoreStyleAttribute(lb.GetItemRow(i), "display");
            }
            lb.OnScroll();
        }
    },
    VisibleCollectionChanged: function() {
        ASPxClientComboBox.prototype.VisibleCollectionChanged.call(this);
        this.SyncVisibleItems();
    },
    isContainedInTokens: function(text, caseSensitive) {
        var transformText = !caseSensitive ? text.toLowerCase() : text; 
        var transformTokenCollection = !caseSensitive ?
                this.GetTokenCollection().map(function(token) { return token.toLowerCase(); }) :
                this.GetTokenCollection(); 
        return ASPx.Data.ArrayIndexOf(transformTokenCollection, transformText) !== -1;
    },
    OnCallbackFinalized: function() {
        ASPxClientComboBox.prototype.OnCallbackFinalized.call(this);
        this.SyncVisibleItems();
    },
    ClearTokenCollection: function() {
        var count = this.GetTokenCollection().length;
        for(var i = 0; i < count; i++) {
            this.RemoveToken(0, true);
        }
    },
    
    AddToken: function(text) {
        this.AddTokenInternal(text, true);
    },
    AddTokenInternal: function(text, callByApi) {
        var item = this.FindItemByCaseSensitiveText(text);
        var value = item ? item.value : text;
        var input = this.GetTokensHiddenField();
        var tokens = this.GetTokenCollection();
        var tokensValue = this.GetTokenValuesCollection();
        var lb = this.GetListBoxControl();
        if(this.incrementalFilteringMode != "None" && !(text == null || text == "")) {
            if(lb && !callByApi) {
                var selectedItem = null;
                if(!this.AllowCustomToken()) {
		            selectedItem = lb.GetSelectedItem();
                } else if(!this.caseSensitiveTokens) {
                    var visibleItems = this.GetVisibleItems();
                    if(visibleItems.length == 1) {
                        if(visibleItems[0].text.toLowerCase() == text.toLowerCase())
                            selectedItem = visibleItems[0];
                    }
                }
				if(selectedItem != null && selectedItem.text.toLowerCase().indexOf(text.toLowerCase()) != -1) {
				    text = selectedItem.text;
				    value = selectedItem.value;
				}
            }
        }
        var allowAddToken = !this.IsCustomToken(text, true) || this.AllowCustomToken();
        if(this.incrementalFilteringMode != "None")
            this.ClearFilterInternal();
        if((allowAddToken || callByApi) && !this.isContainedInTokens(text, this.caseSensitiveTokens) && text != "") {
            this.AddTokenHtmlElementInternal(tokens.length, text, true);
            tokens.push(text);
            tokensValue.push(value);
            this.SetTokenCollectionInternal(tokens, tokensValue, !!callByApi);
        }
        if(lb)
            lb.SetSelectedIndex(-1);
        if(this.IsCanToDropDown()) {
            if(!this.droppedDown && this.IsFocusEventsLocked() && !callByApi) //Prevent additional showdropdown on lostfocus
                this.ShowDropDown();
        } else {
            if(this.droppedDown)
                this.HideDropDown();
		}
    },
    AddTokenHtmlElementInternal: function(index, text, needTokenHover) {
        var tokenText = this.encodeHtml ? ASPx.Str.EncodeHtml(text) : text;
        var tokenElement = this.GetNewHtmlTokenItem(index, tokenText);
        this.ResetInputSize();
        var element = index == 0 ? this.GetTokensHiddenField() : this.GetTokenHtmlElement(index - 1).nextSibling;
        element.parentNode.insertBefore(tokenElement, element);
        if(needTokenHover)
            this.SetTokenHover(tokenElement, true);
        return tokenElement;
    },
    IsCustomToken: function(text, caseSensitive) {
        if(caseSensitive)
            return !this.FindItemByCaseSensitiveText(text);
        else
            return !this.FindItemByText(text);
    },
    FindItemByCaseSensitiveText: function(text) {
        for(var i = 0; i < this.GetItemCount(); i ++) {
            var item = this.GetItem(i);
            if(item.text == text)
                return item;
        }
        return null;
    },
    AllowCustomToken: function() {
        return !this.isDropDownListStyle;
    },
    GetTokenIndexByText: function(text) {
        var tokens = this.GetTokenCollection();
        for(var i = 0; i < tokens.length; i++) {
            if(tokens[i].toString() == text)
                return i;
        }
        return -1;
    },
    GetVisibleItems: function() {
        var lb = this.GetListBoxControl();
        var visibleItems = [];
        if(!!lb) {
            var lbTable = lb.GetListTable();
            var count = lb.GetItemCount();
            for(var i = 0; i < count; i ++){
                if(ASPx.GetElementDisplay(lbTable.rows[i])) {
                    visibleItems.push(lb.GetItem(i));
                }
            }
        }
        return visibleItems;
    },
    OnTokensChanged: function() {
        this.RaiseValidationInternal();
        this.AdjustInput(false);
        var processOnServer = this.RaiseTokensChanged();
        processOnServer = ASPxClientTextEdit.prototype.RaiseValueChangedEvent.call(this);
        return processOnServer;
    },
    RaiseTokensChanged: function() {
        if(!this.TokensChanged.IsEmpty()){
            var args = new ASPxClientEventArgs();
            this.TokensChanged.FireEvent(this, args);
        }
        return this.autoPostBack;
    },
    OnListBoxItemMouseUp: function(evt) {
        var index = this.GetSelectedIndex();
        this.GetInputElement().value = "";
        this.AddTokenInternal(this.GetItem(index).text);
        ASPxClientComboBox.prototype.SetValue.call(this, null);
        return false;
    },
    ShouldCloseOnMCMouseDown: function(evt) {
        return false;
    },
    ShowDropDownArea: function(isRaiseEvent){
        ASPxClientComboBox.prototype.ShowDropDownArea.call(this, isRaiseEvent);
        this.SyncVisibleItems();
    },
    OnFocus: function() {
        if(this.CorrectFocusWhenDisabled())
            return;
        if(this.editorTimer)
            clearTimeout(this.editorTimer);
        this.editorTimer = setTimeout(function() { this.AdjustInput(true); }.aspxBind(this), 0);
        ASPxClientComboBox.prototype.OnFocus.call(this);
        if(this.IsCanToDropDown())
            this.ToggleDropDown();
    },
    IsCanToDropDown: function() {
        return this.showDropDownOnFocus == "Always" && ASPxClientComboBox.prototype.IsCanToDropDown.call(this);
    },
    IsFilterRollbackRequiredAfterApply: function() {
        return false;
    },
    ValidateWithPatterns: function() {
        if(this.validationPatterns.length > 0) {
            var validate = function(tokenBox, validator, value) {
                if(!validator.EvaluateIsValid(value)) {
                    tokenBox.SetIsValid(false, true /* validating */);
                    tokenBox.SetErrorText(validator.errorText, true /* validating */);
                    return;
                }
            };
            var coll = this.GetValue().split(this.valueSeparator);
            for(var i = 0; i < this.validationPatterns.length; i++) {
                var validator = this.validationPatterns[i];
                if(coll.length > 1) {
                    for(var vi = 0; vi < coll.length; vi++) {
                        validate(this, validator, coll[vi]);
                    }
                } else {
                    validate(this, validator, this.GetValue());
                }
            }
        }
    },
    RemoveTokenByText: function(text) {
        var index = this.GetTokenIndexByText(text);
        if(index != -1)
            this.RemoveToken(index, true);
    },
    RemoveToken: function(index, callByApi) {
        if(this.CanChange() || callByApi) {
            var tokens = this.GetTokenCollection();
            var tokensValue = this.GetTokenValuesCollection();
            if(tokens.length > 0) {
                tokens.splice(index, 1);
                tokensValue.splice(index, 1);
                var tokenElement = this.GetTokenHtmlElement(index);
                var tokensParent = tokenElement.parentNode;
                this.SetTokenHover(tokenElement, false);
                tokensParent.removeChild(tokenElement);
                this.RefreshHtmlTokenIndices();
                this.SetTokenCollectionInternal(tokens, tokensValue, !!callByApi);
            }
        }
    },
    CanChange: function() {
        return !this.readOnly && this.enabled && this.clientEnabled;
    },
    RefreshHtmlTokenIndices: function() {
        var parent = this.GetTokensHiddenField().parentNode;
        var ind = 0;
        for(var i = 0; i < parent.childNodes.length; i++) {
            var tokenElement = parent.childNodes[i];
            var indexOfTokenId = tokenElement.id.indexOf(this.name + "_" + tokenBoxTokenSuffix);
            if(indexOfTokenId >= 0) {
                this.SetTokenHover(tokenElement, false);
                tokenElement.id = this.name + "_" + tokenBoxTokenSuffix + ind.toString();
                this.SetTokenHover(tokenElement, true);
                ind++;
            }
        }
    },
    GetNewHtmlTokenItem: function(index, text) {
        var tokenElement = this.GetSampleTokenElement().cloneNode(true);
        tokenElement.childNodes[0].innerHTML = text;
        tokenElement.childNodes[0].id = this.name + "_" + tokenBoxTokenTextSuffix + index;
        tokenElement.childNodes[1].id = this.name + "_" + tokenBoxTokenRemoveButtonSuffix + index;
        tokenElement.id = this.name + "_" + tokenBoxTokenSuffix + index;
        tokenElement.style.display = "";
        return tokenElement;
    },
    GetSampleTokenElement: function() {
        return document.getElementById(this.name + "_" +  tokenBoxTokenSuffix + "-1");
    },
    GetText: function() {
        var tokens = this.GetTokenCollection();
        return tokens.join(this.textSeparator);
    },
    SetText: function(text) {
        this.ClearTokenCollection();
        if(text != null && text != "") {
            var tokens = text.split(this.textSeparator);
            if(tokens.length > 0) {
                for(var i = 0; i < tokens.length; i++) {
                    this.AddTokenInternal(tokens[i], true);
                }
            }
        }
    },
    GetValue: function() {
        var tokensValues = this.GetTokenValuesCollection();
        return tokensValues.join(this.valueSeparator);
    },
    SetValue: function(value) {
        this.ClearTokenCollection();
        if(value != null && value != "") {
            var values = value.split(this.valueSeparator);
            for(var i = 0; i < values.length; i++) {
                var item = this.FindItemByValue(values[i]);
                this.AddTokenInternal(item != null ? item.text : values[i].toString(), true);
            }
        }
    },
    OnTextChanged: function() { },
    OnValueChanged: function() { },
    OnApplyChangesInternal: function() { },
    ParseValue: function() { },
    CanApplyNullTextDecoration: function () { return this.GetTokenCollection().length == 0; },
    ToggleTextDecorationCore: function() {
        var input = this.GetInputElement();
        var value = this.CanApplyTextDecorators() && this.CanApplyNullTextDecoration() ? this.nullText : "";
        if(input.value != value)
            input.value = value;
        this.AdjustInput();
    },
    GetRawValue: function(value){
        return null;
    },
    ClearFilterInternal: function() {
        //this.GetInputElement().value = "";
        if(this.filterStrategy.filter != "")
            this.filterStrategy.FilteringBackspace();
        if(this.filterStrategy.filterInitialized){
            this.filterStrategy.ClearFilter();
            this.filterStrategy.isEnterLocked = false;
            this.filterStrategy.FilteringStopClient();
        }
    },
    OnEscape: function() {
        this.GetInputElement().value = "";
        if(this.filterStrategy)
            this.filterStrategy.Filtering();
        this.HideDropDownArea(true);
        return this.droppedDown;
    },
    OnEnter: function() {
        var inputValue = this.GetInputElement().value;
        if(inputValue){
            this.GetInputElement().value = "";
            this.AddTokenInternal(inputValue);
        }
        return true;
    },
    EnsureSelectedItemVisibleOnShow: function() {},
    RaiseValueChangedEvent: function() { return false; },
    OnSpecialKeyDown: function(evt){
        if(ASPx.Evt.GetKeyCode(evt) == ASPx.Key.Backspace && this.GetInputElement().value == "") {
            if(this.CanChange()) {
                var tokens = this.GetTokenCollection()
                if(tokens.length > 0) {
                    var index = tokens.length - 1;
                    var text = tokens[index];
                    this.RemoveToken(index, false);
                    if(this.AllowCustomToken() || this.IsFilterEnabled()) {
                        this.GetInputElement().value = text;
                        ASPx.Selection.Set(this.GetInputElement());
                    }
                }
            }
            return true;
        } else
            return ASPxClientComboBox.prototype.OnSpecialKeyDown.call(this, evt);
    },
    ContainsSeparator: function(text) {
        var delimiters = [',', ';', this.textSeparator];
        var delimiterPosition = -1;
        if(this.AllowCustomToken())
            delimiters.push(this.valueSeparator);
        delimiters = ASPx.Data.GetDistinctArray(delimiters);
        for(var i = 0; i < delimiters.length; i++) {
            if(text.indexOf(delimiters[i]) >= 0)
                delimiterPosition = delimiterPosition == -1 ? text.indexOf(delimiters[i]) : Math.min(text.indexOf(delimiters[i]), delimiterPosition);
        }
        return delimiterPosition;
    },
    OnLostFocus: function() {
        var input = this.GetInputElement();
        var focusLockedOrCorrected = this.IsFocusEventsLocked() || this.NeedFocusCorrectionWhenDisabled() && !this.GetEnabled();
        if(!focusLockedOrCorrected) {
            var lb = this.GetListBoxControl();
            if(lb)
                lb.SetSelectedIndex(-1);
            if(input.value != ""){
                this.AddTokenInternal(input.value, false);
                input.value = "";
            }
            if(this.droppedDown)
                this.HideDropDown();
        }
        ASPxClientComboBox.prototype.OnLostFocus.call(this);
        if(this.IsVisible())
            this.AdjustInput(false);
        clearTimeout(this.editorTimer);
        this.editorTimer = null;
        if(this.incrementalFilteringMode != "None" && !focusLockedOrCorrected) {
            if(this.InCallback() || this.filterStrategy.IsFilterTimerActive())
                this.filterStrategy.callbackProcessingStateManager.SetApply();
            this.ClearFilterInternal();
        }
    },
    ToggleDropDown: function() {
        this.OnApplyChanges();
        if(!this.droppedDown)
            this.ShowDropDownArea(true);
    },
    ResetInputSize: function() {
        this.GetInputElement().style.width = tokenBoxInputMinWidth + "px";
    },
    inputIsCollapsed: function() {
        var inputWidth = this.GetInputElement().style.width;
        return inputWidth == tokenBoxInputMinWidth + "px" || inputWidth == "";
    },
    AdjustInput: function(startTimer) {
        this.AdjustInputInternal(!startTimer);
        if(this.droppedDown)
            this.RelocationPopup();
        if(startTimer) {
            this.SplitInputTextAndAddTokenInternal();
            this.editorTimer = setTimeout(function() { this.AdjustInput(startTimer); }.aspxBind(this), 0);
        }
    },
    AdjustInputInternal: function(force) {
        var input = this.GetInputElement();
        if(force && ASPx.GetActiveElement() !== input && !(this.nullText && input.value === this.nullText))
            this.ResetInputSize();
        else if(this.lastInputText != input.value.toString() || this.inputIsCollapsed()) {
            this.lastInputText = input.value.toString();
            var lastToken = document.getElementById(this.name + "_" + tokenBoxTokenSuffix + (this.GetTokenCollection().length - 1));
            var tokensOffset = 0;
            if(lastToken) {
                var lastTokenCss = ASPx.GetCurrentStyle(lastToken);
                var mainElementCss = ASPx.GetCurrentStyle(this.GetMainElement());
                var lastTokenOffsetLeft = 0;
                if(!ASPx.Browser.Chrome) {//T255859,T286239
                    lastTokenOffsetLeft = lastToken.offsetLeft;
                } else {
                    lastTokenOffsetLeft = lastToken.offsetLeft + ASPx.PxToInt(mainElementCss.borderLeftWidth) + 1;
                }
                if(this.rtl)
                    tokensOffset = lastTokenOffsetLeft - ASPx.PxToInt(lastTokenCss.marginLeft);
                else
                    tokensOffset = lastTokenOffsetLeft + lastToken.offsetWidth + ASPx.PxToInt(lastTokenCss.marginRight);
            }
            var parentElementFW = Math.max(ASPx.GetLeftRightMargins(input.parentElement), 0) + input.parentElement.offsetWidth;
            var inputLRMBPSV = ASPx.GetLeftRightMargins(input) + ASPx.GetLeftRightBordersAndPaddingsSummaryValue(input);
            var maxInputWidthWT = this.rtl ? tokensOffset - inputLRMBPSV : (ASPx.GetClearClientWidth(input.parentElement) - tokensOffset) - inputLRMBPSV;
            var maxInputWidth = ASPx.GetClearClientWidth(input.parentElement) - inputLRMBPSV;
            var textWidth = ASPx.GetSizeOfText(input.value.toString(), ASPx.GetCurrentStyle(input)).width;
            var inputWidth = Math.max(tokenBoxInputMinWidth, textWidth) > maxInputWidthWT ? maxInputWidth : maxInputWidthWT;
            input.style.width = ((inputWidth / parentElementFW) * 100) + "%";
        }
    },
    SplitInputTextAndAddTokenInternal: function() {
        var input = this.GetInputElement();
        var text = input.value.toString();
        var delimiterPosition = this.ContainsSeparator(text);
        if(delimiterPosition >= 0) {
            var texts = [text.substr(0, delimiterPosition), text.substr(delimiterPosition + 1)];
            var tokenText = texts[0];
            var inputText = texts[1];
            input.value = inputText;
            if(texts[0] != "") {
                this.AddTokenInternal(tokenText);
                if(this.incrementalFilteringMode != "None" && this.InCallback()) {
                    this.filterStrategy.callbackProcessingStateManager.SetApply();
                }
            }
        }
    },
    RelocationPopup: function() {
        var pc = this.GetPopupControl();
        var element = pc.GetWindowElement(-1);
        var popupElement = this.GetMainElement();
        var horizontalPopupPosition = pc.GetClientPopupPos(element, popupElement, ASPx.InvalidPosition, true, false);
        var verticalPopupPosition = pc.GetClientPopupPos(element, popupElement, ASPx.InvalidPosition, false, false);
        var clientX = horizontalPopupPosition.position;
        var clientY = verticalPopupPosition.position;
        pc.SetWindowPos(-1, element, clientX, clientY); 
    },
    ForceRefocusEditor: function(evt, isNativeFocus) {
        if(ASPx.Browser.VirtualKeyboardSupported && !this.IsCanToDropDown() && !this.AllowCustomToken())
            isNativeFocus = true;
        ASPxClientComboBox.prototype.ForceRefocusEditor.call(this, evt, isNativeFocus);
    },
    SetVisibilityTimer: function() {
        if(this.visibilityTimerID === -1)
            this.visibilityTimerID = ASPx.Timer.SetControlBoundInterval(function () {
                if(!this.IsVisible()) return;

                if(!this.elementsInitialized) {
                    this.InternalInitialize();
                    this.ClearVisibilityTimer();
                } else {
                    var input = this.GetInputElement();
                    var inputParentWidth = ASPx.GetClearClientWidth(input.parentElement);
                    if(inputParentWidth > 0) {
                        this.AdjustControl();
                        this.ClearVisibilityTimer();
                    }
                }
            }, this, 50);
    },
    ClearVisibilityTimer: function () {
        this.visibilityTimerID = ASPx.Timer.ClearInterval(this.visibilityTimerID);
    },
    OnDropDown: function(evt) {
        var isFocused = this.focused;
    	var returnValue = ASPxClientComboBox.prototype.OnDropDown.call(this, evt);
        if(!this.AllowCustomToken()) {
            this.SpecialShowDropDown(evt, isFocused);
        }
        return returnValue;
    },
    OnMainCellMouseDown: function (evt) {
        ASPxClientComboBox.prototype.OnMainCellMouseDown.call(this, evt);
        if(this.AllowCustomToken()) {
            this.SpecialShowDropDown(evt, this.focused);
        }
    },
    SpecialShowDropDown: function(evt, isFocused) {
        if(!this.GetEnabled() || !isFocused) return;
        var isSilentMode =  this.incrementalFilteringMode == "None" && !this.IsCanToDropDown();
        var isRemoveButton = ASPx.Evt.GetEventSource(evt).id.indexOf(this.name + "_" + tokenBoxTokenRemoveButtonSuffix) > -1;
        if(isSilentMode && !isRemoveButton) {
            ASPxClientComboBox.prototype.ToggleDropDown.call(this);
        }
    }
});

// TokenBox
// TODO rename TRBClick
ASPx.TRBClick = function(name, evt) {
    var tokenElement = ASPx.Evt.GetEventSource(evt).parentElement;
    var tb = ASPx.GetControlCollection().Get(name);
    if(tb) {
        var index = tokenElement.id.substring((tb.name + "_" + tokenBoxTokenSuffix).length);
        tb.RemoveToken(index, false);
    }
    return ASPx.Evt.CancelBubble(evt);
}
// TODO rename ME_MD
ASPx.ME_MD = function(name, evt) {
    var tb = ASPx.GetControlCollection().Get(name);
    if(tb && !tb.focused) {
        if(ASPx.Browser.VirtualKeyboardSupported) {
            window.setTimeout(function() {  
                ASPx.VirtualKeyboardUI.smartFocusEditor(tb);
            }, 0);
        } else {
            tb.SetFocus(); 
        }
    }
}

window.ASPxClientTokenBox = ASPxClientTokenBox;

})();