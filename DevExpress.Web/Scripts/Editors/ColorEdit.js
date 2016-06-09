/// <reference path="..\_references.js"/>

(function() {
var colorIndicatorIdPostfix = "_CI";
var colorNestedControlIdPostfix = "_CNC";

var colorNameArray = {
    aliceblue: 'f0f8ff',
    antiquewhite: 'faebd7',
    aqua: '00ffff',
    aquamarine: '7fffd4',
    azure: 'f0ffff',
    beige: 'f5f5dc',
    bisque: 'ffe4c4',
    black: '000000',
    blanchedalmond: 'ffebcd',
    blue: '0000ff',
    blueviolet: '8a2be2',
    brown: 'a52a2a',
    burlywood: 'deb887',
    cadetblue: '5f9ea0',
    chartreuse: '7fff00',
    chocolate: 'd2691e',
    coral: 'ff7f50',
    cornflowerblue: '6495ed',
    cornsilk: 'fff8dc',
    crimson: 'dc143c',
    cyan: '00ffff',
    darkblue: '00008b',
    darkcyan: '008b8b',
    darkgoldenrod: 'b8860b',
    darkgray: 'a9a9a9',
    darkgreen: '006400',
    darkkhaki: 'bdb76b',
    darkmagenta: '8b008b',
    darkolivegreen: '556b2f',
    darkorange: 'ff8c00',
    darkorchid: '9932cc',
    darkred: '8b0000',
    darksalmon: 'e9967a',
    darkseagreen: '8fbc8f',
    darkslateblue: '483d8b',
    darkslategray: '2f4f4f',
    darkturquoise: '00ced1',
    darkviolet: '9400d3',
    deeppink: 'ff1493',
    deepskyblue: '00bfff',
    dimgray: '696969',
    dodgerblue: '1e90ff',
    feldspar: 'd19275',
    firebrick: 'b22222',
    floralwhite: 'fffaf0',
    forestgreen: '228b22',
    fuchsia: 'ff00ff',
    gainsboro: 'dcdcdc',
    ghostwhite: 'f8f8ff',
    gold: 'ffd700',
    goldenrod: 'daa520',
    gray: '808080',
    green: '008000',
    greenyellow: 'adff2f',
    honeydew: 'f0fff0',
    hotpink: 'ff69b4',
    indianred : 'cd5c5c',
    indigo : '4b0082',
    ivory: 'fffff0',
    khaki: 'f0e68c',
    lavender: 'e6e6fa',
    lavenderblush: 'fff0f5',
    lawngreen: '7cfc00',
    lemonchiffon: 'fffacd',
    lightblue: 'add8e6',
    lightcoral: 'f08080',
    lightcyan: 'e0ffff',
    lightgoldenrodyellow: 'fafad2',
    lightgrey: 'd3d3d3',
    lightgreen: '90ee90',
    lightpink: 'ffb6c1',
    lightsalmon: 'ffa07a',
    lightseagreen: '20b2aa',
    lightskyblue: '87cefa',
    lightslateblue: '8470ff',
    lightslategray: '778899',
    lightsteelblue: 'b0c4de',
    lightyellow: 'ffffe0',
    lime: '00ff00',
    limegreen: '32cd32',
    linen: 'faf0e6',
    magenta: 'ff00ff',
    maroon: '800000',
    mediumaquamarine: '66cdaa',
    mediumblue: '0000cd',
    mediumorchid: 'ba55d3',
    mediumpurple: '9370d8',
    mediumseagreen: '3cb371',
    mediumslateblue: '7b68ee',
    mediumspringgreen: '00fa9a',
    mediumturquoise: '48d1cc',
    mediumvioletred: 'c71585',
    midnightblue: '191970',
    mintcream: 'f5fffa',
    mistyrose: 'ffe4e1',
    moccasin: 'ffe4b5',
    navajowhite: 'ffdead',
    navy: '000080',
    oldlace: 'fdf5e6',
    olive: '808000',
    olivedrab: '6b8e23',
    orange: 'ffa500',
    orangered: 'ff4500',
    orchid: 'da70d6',
    palegoldenrod: 'eee8aa',
    palegreen: '98fb98',
    paleturquoise: 'afeeee',
    palevioletred: 'd87093',
    papayawhip: 'ffefd5',
    peachpuff: 'ffdab9',
    peru: 'cd853f',
    pink: 'ffc0cb',
    plum: 'dda0dd',
    powderblue: 'b0e0e6',
    purple: '800080',
    red: 'ff0000',
    rosybrown: 'bc8f8f',
    royalblue: '4169e1',
    saddlebrown: '8b4513',
    salmon: 'fa8072',
    sandybrown: 'f4a460',
    seagreen: '2e8b57',
    seashell: 'fff5ee',
    sienna: 'a0522d',
    silver: 'c0c0c0',
    skyblue: '87ceeb',
    slateblue: '6a5acd',
    slategray: '708090',
    snow: 'fffafa',
    springgreen: '00ff7f',
    steelblue: '4682b4',
    tan: 'd2b48c',
    teal: '008080',
    thistle: 'd8bfd8',
    tomato: 'ff6347',
    turquoise: '40e0d0',
    violet: 'ee82ee',
    violetred: 'd02090',
    wheat: 'f5deb3',
    white: 'ffffff',
    whitesmoke: 'f5f5f5',
    yellow: 'ffff00',
    yellowgreen: '9acd32'
};
var ASPxClientColorEdit = ASPx.CreateClass(ASPxClientDropDownEditBase, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);

        this.color = null;
        this.allowNull = true;
        this.colorOnError = "u";
        this.mouseDownElement = null;
        this.isTabbingColorPickerInputs = false;
        this.isAutomaticColorSelected = false;
        this.ColorChanged = new ASPxClientEvent();
    },
    InlineInitialize: function() {
        this.InitSpecialKeyboardHandling();
        ASPxClientDropDownEditBase.prototype.InlineInitialize.call(this);
    },
    Initialize: function() {
        var colorNestedControl = this.GetColorNestedControl();
        if(colorNestedControl) {
            if(colorNestedControl.enableCustomColors) {
                ASPx.Evt.AttachEventToElement(this.GetColorNestedControl().GetColorSelectorElement(), ASPx.TouchUIHelper.touchMouseDownEventName, this.OnMouseDownColorSelector.aspxBind(this));
			    this.AttachLostFocusEventToColorPickerInputs();
		    }
            colorNestedControl.ColorChanged.AddHandler(this.OnColorNestedControlColorChanged.aspxBind(this));
            colorNestedControl.ShouldBeClosed.AddHandler(this.HideDropDown.aspxBind(this));
            colorNestedControl.CustomColorTableUpdated.AddHandler(this.UpdateState.aspxBind(this));
            colorNestedControl.SetOwner(this.GetPopupControl());
            colorNestedControl.SetOwnerElementId(this.mainElement.id);
        }
        ASPxClientDropDownEditBase.prototype.Initialize.call(this);
    },
    AttachLostFocusEventToColorPickerInputs: function() {
        var colorInputs = [ ];
        var colorPickerControl = this.GetColorNestedControl().GetColorPickerControl();
        colorInputs.push(colorPickerControl.GetWebColorInputElement());
        colorInputs.push(colorPickerControl.GetRedInputElement());
        colorInputs.push(colorPickerControl.GetGreenInputElement());
        colorInputs.push(colorPickerControl.GetBlueInputElement());
        for(var i = 0; i < colorInputs.length; i++) {
            ASPx.Evt.AttachEventToElement(colorInputs[i], "focus", function(evt) {
                if(this.isTabbingColorPickerInputs)
                    this.mouseDownElement = ASPx.Evt.GetEventSource(evt);
                this.isTabbingColorPickerInputs = false;
            }.aspxBind(this));

            ASPx.Evt.AttachEventToElement(colorInputs[i], "keydown", function(evt) {
                this.isTabbingColorPickerInputs = evt.keyCode == ASPx.Key.Tab;
            }.aspxBind(this));

            ASPx.Evt.AttachEventToElement(colorInputs[i], "blur", this.OnColorPickerInputsBlur.aspxBind(this));
        }
    },
    ForceRefocusEditor: function(evt, isNativeFocus) {
        if(evt && this.GetColorNestedControl().enableCustomColors) {
            var evtSource = ASPx.Evt.GetEventSource(evt);
            var colorPickerMainElement = this.GetColorNestedControl().GetColorPickerControl().GetMainElement();
            if(colorPickerMainElement !== evtSource && !ASPx.GetIsParent(colorPickerMainElement, evtSource))
                this.mouseDownElement = null;
        }
        ASPxClientEdit.prototype.ForceRefocusEditor.call(this, evt, isNativeFocus);
    },
    OnColorPickerInputsBlur: function(evt) {
        var inputElement = null;
        var evtSource = ASPx.Evt.GetEventSource(evt);
        var isInput = this.mouseDownElement ? this.mouseDownElement.tagName == "INPUT" : false;
        var isLabel = this.mouseDownElement ? this.mouseDownElement.tagName == "LABEL" : false;
                
        if(isInput || isLabel)
            inputElement = isLabel ? ASPx.GetElementById(this.mouseDownElement.htmlFor) : this.mouseDownElement;
                
        if(this.isTabbingColorPickerInputs) {
            var webColorInput = this.GetColorNestedControl().GetColorPickerControl().GetWebColorInputElement();
            if(evtSource === webColorInput) {
                this.mouseDownElement = null;
                this.isTabbingColorPickerInputs = false;
                this.ForceRefocusEditor();
            }
        } else if(this.focusEventsLocked &&  ASPx.IsExists(inputElement) && inputElement === evtSource) {
            this.mouseDownElement = null;
            this.isTabbingColorPickerInputs = false;
            this.UnlockFocusEvents();
            this.OnLostFocus();
            this.OnSpecialLostFocus();
        }
    },
    GetColorIndicatorCell: function() {
        return this.GetChildElement(colorIndicatorIdPostfix);
    },
    GetColorNestedControl: function() {
        var pc = this.GetPopupControl();
        return pc ? ASPx.GetControlCollection().Get(pc.name + colorNestedControlIdPostfix) : null;
    },
    ShowDropDownArea: function(isRaiseEvent) {
        this.GetColorNestedControl().SetColorSelectorDisplay(false);
        this.GetColorNestedControl().SetColorTablesDisplay(true);
        ASPxClientDropDownEditBase.prototype.ShowDropDownArea.call(this, isRaiseEvent);
    },
    OnMouseDownColorSelector: function(e) {
        this.mouseDownElement = ASPx.Evt.GetEventSource(e);
    },
    OnApplyChanges: function() {
        this.OnTextChanged();
    },
    OnTab: function() {
        this.HideDropDown();
    },
    OnTextChanged: function() {
        this.UpdateColor();
    },
    OnColorNestedControlColorChanged: function(s, e) {
        this.SetTextInternal(s.GetValue());
        this.UpdateColor();
    },
    OnEnter: function() {
        if (!this.isInitialized) return true;

        if (this.droppedDown && this.GetColorNestedControl().IsColorSelectorDisplayed())
            this.GetColorNestedControl().OnOkButtonClick();
        else
            this.OnApplyChanges();
        return ASPxClientDropDownEditBase.prototype.OnEnter.call(this);
    },
    SetFocus: function() {
        if(this.mouseDownElement) {
            var isInput = this.mouseDownElement.tagName == "INPUT";
            var isButton = isInput && this.mouseDownElement.getAttribute('type') == "button";
            var isLabel = this.mouseDownElement.tagName == "LABEL";
            if (isInput && !isButton || isLabel) {
                ASPx.SetFocus(this.mouseDownElement);
                return;
            }
        }
        ASPxClientDropDownEditBase.prototype.SetFocus.call(this);
    },
    UpdateColor: function() {
        var inputElement = this.GetInputElement();
        if(!ASPx.IsExistsElement(inputElement))
            return;
        var inputValue = inputElement.value;
        var isAutomaticColor = this.IsAutomaticColorItemValue(inputValue);
        var color = isAutomaticColor ? this.GetAutomaticColor() : this.GetColorByValue(inputValue);
        this.ChangeColor(color, isAutomaticColor);
    },
    IsAutomaticColorItemValue: function(value) {
        return value != "" && value == this.GetAutomaticColorItemValue();
    },
    GetColorByValue: function(inputValue) {
        var color = inputValue != "" ? ASPxClientColorEdit.ParseColor(inputValue) : "";
        if(color === false) {
            switch (this.colorOnError) {
                case "u":
                    color = this.color;
                    break;
                default:
                    color = null;
                    break;
            }
        }
        if(!this.allowNull && (color == null || color == ""))
            color = this.color;
        return color;
    },
    ChangeColor: function(newColor, isAutomaticColor) {
        var isNew = this.IsNew(newColor, isAutomaticColor);
        this.isAutomaticColorSelected = isAutomaticColor;
        this.SetColorInternal(newColor);
        var forceValueChanged = this.IsValueChangeForced() && !this.IsValueChanging();
        if(isNew || forceValueChanged) {
            this.StartValueChanging();
            this.OnValueChanged();
            this.EndValueChanging();
        }
    },
    IsNew: function(newColor, isAutomaticColor) {
        var isChangedItemType = isAutomaticColor != this.IsAutomaticColorSelected();
        var isChangedColor = newColor ? this.color != newColor : !!this.color
        return isChangedItemType || isChangedColor;
    },
    SetColorInternal: function(color) {
        this.color = color;
        
        var value = this.IsAutomaticColorSelected() ? this.GetAutomaticColorItemValue() : this.GetFormattedColorString(this.color);
        ASPxClientDropDownEditBase.prototype.SetValue.call(this, value);

        this.GetColorNestedControl().SetColor(this.color, this.IsAutomaticColorSelected());
        this.ChangeIndicatorColor(this.color);
    },
    ChangeIndicatorColor: function(newColor) {
        var colorIndicator = this.GetColorIndicatorCell();
        colorIndicator.style.backgroundColor = newColor != null ? newColor : "";
    },
    GetFormattedColorString: function(color) {
        if (color == null)
            return "";
        return color.toUpperCase();
    },
    RaiseValueChangedEvent: function() {
        if (!this.isInitialized) return false;
        var processOnServer = ASPxClientEdit.prototype.RaiseValueChangedEvent.call(this);
        processOnServer = this.RaiseColorChanged(processOnServer);
        return processOnServer;
    },

    UpdateState: function() {
        var state = this.GetColorNestedControl().GetState();

        this.UpdateCookie(state);
        this.UpdateStateObjectWithObject({ colors: state });
    },
    UpdateCookie: function(state) {
        if(this.cookieName == '') return;

        ASPx.Cookie.DelCookie(this.cookieName);
        ASPx.Cookie.SetCookie(this.cookieName, this.GetColorNestedControl().GetState());
    },
    GetStateHiddenFieldName: function() {
        return this.uniqueID + "$State";
    },
    GetAutomaticColorItemValue: function() {
        return this.GetColorNestedControl().GetAutomaticColorItemValue();
    },
    GetAutomaticColor: function() {
        return this.GetColorNestedControl().GetAutomaticColor();
    },

    // API
    RaiseColorChanged: function(processOnServer) {
        if (!this.ColorChanged.IsEmpty()) {
            var args = new ASPxClientProcessingModeEventArgs(processOnServer);
            this.ColorChanged.FireEvent(this, args);
            processOnServer = args.processOnServer;
        }
        return processOnServer;
    },
    GetValue: function() {
        return this.IsAutomaticColorSelected() ? this.GetAutomaticColorItemValue() : this.color;
    },
    SetValue: function(value) {
        this.isAutomaticColorSelected = this.IsAutomaticColorItemValue(value);
        this.SetColorInternal(this.IsAutomaticColorSelected() ? this.GetAutomaticColor() : value);
    },
    GetText: function() {
        return this.IsAutomaticColorSelected() ? this.GetAutomaticColorItemValue() : this.GetFormattedColorString(this.color);
    },
    SetText: function(value) {
        ASPxClientDropDownEditBase.prototype.SetValue.call(this, value);
        this.UpdateColor();
    },
    GetColor: function() {
        if (this.color != null)
            return this.color.toUpperCase();
        return null;
    },
    SetColor: function(color) {
        this.isAutomaticColorSelected = false;
        this.SetColorInternal(color);
    },
    ClearEditorValueByClearButtonCore: function() {
        if(this.allowNull === false) {
            var lastSuccesfullValue = this.IsAutomaticColorSelected() ? this.GetAutomaticColor() : this.GetLastSuccesfullValue();
            this.SetColorInternal(lastSuccesfullValue);
        } else
            ASPxClientDropDownEditBase.prototype.ClearEditorValueByClearButtonCore.call(this);
    },
    IsAutomaticColorSelected: function() {
        return this.isAutomaticColorSelected;
    }
});
ASPxClientColorEdit.Cast = ASPxClientControl.Cast;

ASPxClientColorEdit.ParseColor = function(colorString) {
    var regExp = new RegExp("^#?([a-f]|[A-F]|[0-9]){3}(([a-f]|[A-F]|[0-9]){3})?$");
    var color = colorNameArray[colorString.toLowerCase()];
    if (!color) {
        if (regExp.test(colorString))
            colorString = _aspxGetFullHexColor(colorString);
        color = ASPx.Color.ColorToHexadecimal(colorString);
    }
    else 
        color = "#" + color;
    return regExp.test(color) ? color : false;
}

// #XXXXXX
function _aspxGetFullHexColor(colorString) {
    if (colorString == "")
        return null;
    var color = colorString.replace("#", "");
    if (color.length == 3) {
        var newColor = "";    
        for (var i = 0 ; i < 3; i++)
            newColor += color.charAt(i) + color.charAt(i);
            
            color = newColor;
    }
    return "#" + color;
}

window.ASPxClientColorEdit = ASPxClientColorEdit;
})();