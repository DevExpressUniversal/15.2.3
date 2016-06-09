/// <reference path="..\_references.js"/>

(function() {
var ColorPickerIDSuffix = {
    ColorArea: "_CA",
    HueArea: "_HA",
    CurrentColor: "_CC",
    SavedColor: "_SC",
    ColorParameterRed: "_R",
    ColorParameterGreen: "_G",
    ColorParameterBlue: "_B",
    RedInput: "_RI",
    GreenInput: "_GI",
    BlueInput: "_BI",
    WebColor: "_WC"
};

var ColorPicker = ASPx.CreateClass(ASPxClientControl, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);

        this.rgbDefault = { red: ASPxColorUtils.maxRgb.red, green: 0, blue: 0 };
        this.hsbCurrent;
        this.colorAreaWidth;
        this.colorAreaHeight;
        this.hueAreaHeight;
        this.InputEsc = new ASPxClientEvent();
        this.InputEnter = new ASPxClientEvent();
    },
    Initialize: function() {
        this.constructor.prototype.Initialize.call(this);
        this.InitializeSize();
        this.InitializeEvents();
        this.InitializeDefaultState();
    },
    InitializeDefaultState: function() {
        this.SetDefaultState();
    },
    InitializeSize: function() {
        var colorArea = this.GetColorAreaElement();
        var hueArea = this.GetHueAreaElement();
        var colorIndicator = this.GetColorIndicatorElement();
        var hueIndicator = this.GetHueIndicatorElement();

        this.colorAreaWidth = colorArea.offsetWidth;
        this.colorAreaHeight = colorArea.offsetHeight;
        this.hueAreaHeight = hueArea.offsetHeight;
        this.colorIndicatorWidth = colorIndicator.offsetWidth;
        this.colorIndicatorHeight = colorIndicator.offsetHeight;
        this.hueIndicatorHeight = hueIndicator.offsetHeight;
    },
    InitializeEvents: function(listTable, method) {
        var colorArea = this.GetColorAreaElement();
        var hueArea = this.GetHueAreaElement();
        var instance = this;

        this.OnColorAreaMouseMove = function(e) {
            if(ASPx.Evt.IsLeftButtonPressed(e)){
                instance.MoveColorIndicator(e);
                if(ASPx.Browser.WebKitTouchUI)
                    e.preventDefault();
            }
        }
        this.OnHueAreaMouseMove = function(e) {
            if(ASPx.Evt.IsLeftButtonPressed(e)) {
                instance.MoveHueIndicator(e);
                if(ASPx.Browser.WebKitTouchUI)
                    e.preventDefault();
            }
        }
        this.OnColorAreaMouseUp = function(e) {
            ASPx.Evt.DetachEventFromElement(document, ASPx.TouchUIHelper.touchMouseMoveEventName, instance.OnColorAreaMouseMove);
            ASPx.Evt.DetachEventFromElement(document, ASPx.TouchUIHelper.touchMouseUpEventName, instance.OnColorAreaMouseUp);
            ASPx.Selection.SetElementSelectionEnabled(document.body, true);
        }
        this.OnHueAreaMouseUp = function(e) {
            ASPx.Evt.DetachEventFromElement(document, ASPx.TouchUIHelper.touchMouseMoveEventName, instance.OnHueAreaMouseMove);
            ASPx.Evt.DetachEventFromElement(document, ASPx.TouchUIHelper.touchMouseUpEventName, instance.OnHueAreaMouseUp);
            ASPx.Selection.SetElementSelectionEnabled(document.body, true);
        }
        
        ASPx.Evt.AttachEventToElement(this.GetColorAreaElement(), ASPx.TouchUIHelper.touchMouseDownEventName, this.OnColorAreaMouseDown.aspxBind(this));
        ASPx.Evt.AttachEventToElement(this.GetColorIndicatorElement(), ASPx.TouchUIHelper.touchMouseDownEventName, this.OnColorAreaMouseDown.aspxBind(this));
        ASPx.Evt.AttachEventToElement(this.GetHueAreaElement(), ASPx.TouchUIHelper.touchMouseDownEventName, this.OnHueAreaMouseDown.aspxBind(this));
        ASPx.Evt.AttachEventToElement(this.GetHueIndicatorElement(), ASPx.TouchUIHelper.touchMouseDownEventName, this.OnHueAreaMouseDown.aspxBind(this));

        ASPx.Evt.AttachEventToElement(this.GetCurrentColorElement(), ASPx.TouchUIHelper.touchMouseDownEventName, this.OnCurrentColorClick.aspxBind(this));
        ASPx.Evt.AttachEventToElement(this.GetSavedColorElement(), ASPx.TouchUIHelper.touchMouseDownEventName, this.OnSavedColorClick.aspxBind(this));
        ASPx.Evt.AttachEventToElement(this.GetWebColorInputElement(), "keyup", this.OnWebColorInputChange.aspxBind(this));
        ASPx.Evt.AttachEventToElement(this.GetWebColorInputElement(), "keypress", this.OnWebColorKeyPress.aspxBind(this));
        ASPx.Evt.AttachEventToElement(this.GetWebColorInputElement(), "blur", this.OnWebColorInputBlur.aspxBind(this));

        this.SetEventsOnInputElement(this.GetRedInputElement(), ASPxColorUtils.maxRgb.red);
        this.SetEventsOnInputElement(this.GetGreenInputElement(), ASPxColorUtils.maxRgb.green);
        this.SetEventsOnInputElement(this.GetBlueInputElement(), ASPxColorUtils.maxRgb.blue);
    },
    SetDefaultState: function() {
        var defaultWebColor = ASPxColorUtils.RgbToWebColor(this.rgbDefault);
        this.SetColorByHexColor(defaultWebColor);
        this.SetSavedColor(defaultWebColor);
    },
    OnCurrentColorClick: function(e) {
        this.SetSavedColorToCurrentColor();
    },
    SetSavedColorToCurrentColor: function() {
        this.SetSavedColor(this.GetCurrentColor());
    },
    OnSavedColorClick: function(e) {
        var colorStyle = this.GetSavedColor();
        this.SetCurrentColor(colorStyle);
        var rgb = ASPxColorUtils.RgbStyleToObject(colorStyle);
        if(!ASPxColorUtils.IsRgbEquals(rgb, this.GetRgbInput())) {
            this.SetRgbInput(rgb);
            this.SetWebColorInput(ASPxColorUtils.RgbToWebColor(rgb));
            var hsb = ASPxColorUtils.RgbToHsb(rgb);
            this.SetHsbCore(hsb);
            this.ChangePositionHsb(hsb);
            this.SetCurrentColor(ASPxColorUtils.RgbObjectToStyle(rgb));
        }
    },
    OnColorAreaMouseDown: function(e) {
        ASPx.Evt.AttachEventToElement(document, ASPx.TouchUIHelper.touchMouseMoveEventName, this.OnColorAreaMouseMove);
        ASPx.Evt.AttachEventToElement(document, ASPx.TouchUIHelper.touchMouseUpEventName, this.OnColorAreaMouseUp);
        ASPx.Selection.SetElementSelectionEnabled(document.body, false);
        this.MoveColorIndicator(e);
    },
    OnHueAreaMouseDown: function(e) {
        ASPx.Evt.AttachEventToElement(document, ASPx.TouchUIHelper.touchMouseMoveEventName, this.OnHueAreaMouseMove);
        ASPx.Evt.AttachEventToElement(document, ASPx.TouchUIHelper.touchMouseUpEventName, this.OnHueAreaMouseUp);
        ASPx.Selection.SetElementSelectionEnabled(document.body, false);
        this.MoveHueIndicator(e);
    },
    SetEventsOnInputElement: function(input, maxValue) {
        this.OnInputChange = function(e) {
            var key = ASPx.Evt.GetKeyCode(e);
            var inputElement = ASPx.Evt.GetEventSource(e);
            var newValue = inputElement.value;
            if(this.ProcessServiceKeys(key))
                return;
            
            if(newValue === "" || inputElement.oldvalue === newValue)
                return ASPx.Evt.PreventEvent(e);

            if(!ASPxColorUtils.IsRgbComponentValid(newValue)) {
                inputElement.value = inputElement.oldvalue;
                return ASPx.Evt.PreventEvent(e);
            }

            inputElement.oldvalue = newValue;

            var hsb = ASPxColorUtils.RgbToHsb(this.GetRgbInput());
            this.SetHsbCore(hsb);
            this.ChangePositionHsb(hsb);
            this.SetCurrentColor(ASPxColorUtils.RgbObjectToStyle(this.GetRgbInput()));
            this.SetWebColorInput(ASPxColorUtils.RgbToWebColor(this.GetRgbInput()));
        }

        this.OnInputFocus = function(e) {
            var inputElement = ASPx.Evt.GetEventSource(e);
            inputElement.oldvalue = inputElement.value;
        }

        this.OnInputBlur = function(e) {
            var inputElement = ASPx.Evt.GetEventSource(e);
			var oldValue = inputElement.oldvalue;
            var isValidValue = inputElement.value && ASPxColorUtils.IsRgbComponentValid(inputElement.value);
            if(!isValidValue)
                inputElement.value = oldValue;

            if(inputElement.value != oldValue)
                this.SetColorByHexColor(ASPxColorUtils.RgbToWebColor(this.GetRgbInput()));
        }

        this.OnRgbInputKeyPress = function(e) {
            var regex = /[0-9]/;
            return this.OnInputKeyPress(e, regex);
        }

        ASPx.Evt.AttachEventToElement(input, "focus", this.OnInputFocus);
        ASPx.Evt.AttachEventToElement(input, "blur", this.OnInputBlur.aspxBind(this));
        ASPx.Evt.AttachEventToElement(input, "keyup", this.OnInputChange.aspxBind(this));
        ASPx.Evt.AttachEventToElement(input, "keypress", this.OnRgbInputKeyPress.aspxBind(this));
    },
    OnInputKeyPress: function(e, regex) {
        var key = ASPx.Evt.GetKeyCode(e);
        key = String.fromCharCode(key);
        if(!regex.test(key) && !this.ServiceKeyInFirefox(e))
            return ASPx.Evt.PreventEvent(e);
    },
    ServiceKeyInFirefox: function(e) {
        var keyCode = e.keyCode;
        return ASPx.Browser.Firefox && (keyCode == ASPx.Key.Left || keyCode == ASPx.Key.Right ||
                keyCode == ASPx.Key.Backspace || keyCode == ASPx.Key.Delete || keyCode == ASPx.Key.Home || 
                keyCode == ASPx.Key.End || keyCode == ASPx.Key.Up || keyCode == ASPx.Key.Down || 
                keyCode == ASPx.Key.Tab || keyCode == ASPx.Key.Insert || e.ctrlKey);
    },
    OnWebColorInputChange: function(e) {
        var key = ASPx.Evt.GetKeyCode(e);
        var newValue = ASPx.Evt.GetEventSource(e).value;
        var rgb = ASPxColorUtils.WebColorToRgb(newValue);
        if(rgb != null) {            
            this.SetCurrentColor(newValue);
            this.SetRgbInput(rgb);
            var hsb = ASPxColorUtils.RgbToHsb(rgb);
            this.SetHsbCore(hsb);
            this.ChangePositionHsb(hsb);
            this.SetCurrentColor(ASPxColorUtils.RgbObjectToStyle(rgb));
        }

        this.ProcessServiceKeys(key);
    },
    OnWebColorKeyPress: function(e) {
        var regex = /#|[a-f]|[A-F]|[0-9]/;
        return this.OnInputKeyPress(e, regex);
    },
    OnWebColorInputBlur: function(e) {
        var webColorElement = ASPx.Evt.GetEventSource(e);
        var rgb = ASPxColorUtils.WebColorToRgb(webColorElement.value);
        var rgbInputsValue = this.GetRgbInput();

        if(rgb == null)
            webColorElement.value = ASPxColorUtils.RgbToWebColor(rgbInputsValue);
        
        if(rgb != null && !ASPxColorUtils.IsRgbEquals(rgb, rgbInputsValue))
            this.SetColorByHexColor(ASPxColorUtils.RgbToWebColor(rgb));
    },
    ProcessServiceKeys: function(key) {
        switch(key) {
            case ASPx.Key.Esc:
                this.RaiseInputEsc();
                return true;
            case ASPx.Key.Enter:
                this.RaiseInputEnter();
                return true;
        }
        return false;
    },
    RaiseInputEsc: function() {
        if(!this.InputEsc.IsEmpty()) {
            var args = new ASPxClientEventArgs();
            this.InputEsc.FireEvent(this, args);
        }
    },
    RaiseInputEnter: function() {
        if(!this.InputEnter.IsEmpty()) {
            var args = new ASPxClientEventArgs();
            this.InputEnter.FireEvent(this, args);
        }
    },
    GetColorAreaElement: function() {
        return ASPx.GetElementById(this.name + ColorPickerIDSuffix.ColorArea);
    },
    GetHueAreaElement: function() {
        return ASPx.GetElementById(this.name + ColorPickerIDSuffix.HueArea);
    },
    GetCurrentColorElement: function() {
        return ASPx.GetElementById(this.name + ColorPickerIDSuffix.CurrentColor);
    },
    GetSavedColorElement: function() {
        return ASPx.GetElementById(this.name + ColorPickerIDSuffix.SavedColor);
    },
    GetRedInputElement: function() {
        return ASPx.GetElementById(this.name + ColorPickerIDSuffix.ColorParameterRed + ColorPickerIDSuffix.RedInput);
    },
    GetGreenInputElement: function() {
        return ASPx.GetElementById(this.name + ColorPickerIDSuffix.ColorParameterGreen + ColorPickerIDSuffix.GreenInput);
    },
    GetBlueInputElement: function() {
        return ASPx.GetElementById(this.name + ColorPickerIDSuffix.ColorParameterBlue + ColorPickerIDSuffix.BlueInput);
    },
    GetWebColorInputElement: function() {
        return ASPx.GetElementById(this.name + ColorPickerIDSuffix.WebColor);
    },
    GetColorIndicatorElement: function() {
        var colorArea = this.GetColorAreaElement();
        return ASPx.GetChildByTagName(colorArea, "DIV");
    },
    GetHueIndicatorElement: function() {
        var hueArea = this.GetHueAreaElement();
        return ASPx.GetChildByTagName(hueArea, "DIV");
    },
    SetColorAreaBackgroundColor: function(rgb) {
        this.GetColorAreaElement().style.backgroundColor = ASPxColorUtils.RgbObjectToStyle(rgb);
    },
    GetHsb: function() {
        return this.hsbCurrent;
    },
    GetRgbInput: function() {
        return { red: ASPxColorUtils.ToInt(this.GetRedInputElement().value),
                    green: ASPxColorUtils.ToInt(this.GetGreenInputElement().value),
                    blue: ASPxColorUtils.ToInt(this.GetBlueInputElement().value) };
    },
    GetWebColor: function() {
        return this.GetWebColorInputElement().value;
    },
    SetHsbCore: function(hsb) {
        this.hsbCurrent = { hue:hsb.hue, saturation:hsb.saturation, brightness:hsb.brightness };
    },
    SetRgbInput: function(rgb) {
        this.GetRedInputElement().value = rgb.red;
        this.GetGreenInputElement().value = rgb.green;
        this.GetBlueInputElement().value = rgb.blue;
    },
    SetWebColorInput: function(webColor) {
        this.GetWebColorInputElement().value = webColor;
    },
    SetCurrentColor: function(color) {
        this.GetCurrentColorElement().style.backgroundColor = color;
    },
    SetSavedColor: function(color) {
        this.GetSavedColorElement().style.backgroundColor = color;
    },
    GetCurrentColor: function() {
        return this.GetCurrentColorElement().style.backgroundColor;
    },
    GetSavedColor: function() {
        return this.GetSavedColorElement().style.backgroundColor;
    },
    SetColorByHexColor: function(webColor) {
		this.SetCurrentColor(webColor);
	    
        var rgb = ASPxColorUtils.WebColorToRgb(webColor);
        this.SetRgbInput(rgb);
        this.SetWebColorInput(ASPxColorUtils.RgbToWebColor(rgb));

        var hsb = ASPxColorUtils.RgbToHsb(rgb);
        this.SetHsbCore(hsb);
        this.ChangePositionHsb(hsb);
        this.SetCurrentColor(ASPxColorUtils.RgbObjectToStyle(rgb));
    },
    SetColorByHsbCore: function(hsb) {
        var rgb = ASPxColorUtils.HsbToRgb(hsb);
        this.SetCurrentColor(ASPxColorUtils.RgbObjectToStyle(rgb));
        this.SetRgbInput(rgb);
        this.SetHsbCore(hsb);
        this.SetWebColorInput(ASPxColorUtils.RgbToWebColor(rgb));
    },
    MoveColorIndicator: function(e) {
        var colorIndicator = this.GetColorIndicatorElement();
        var x = ASPx.PrepareClientPosForElement(ASPx.Evt.GetEventX(e), colorIndicator, true);
        var y = ASPx.PrepareClientPosForElement(ASPx.Evt.GetEventY(e), colorIndicator, false);
        
        x = Math.min(this.colorAreaWidth, Math.max(0, x)) - this.colorIndicatorWidth / 2;
        y = Math.min(this.colorAreaHeight, Math.max(0, y)) - this.colorIndicatorHeight / 2;

        ASPx.SetStyles(colorIndicator, { left: x, top: y });

        var hsb = this.GetHsb();
        hsb.saturation = this.ColorIndicatorPositionToSaturationValue(x);
        hsb.brightness = this.ColorIndicatorPositionToBrightnessValue(y);
        this.SetColorByHsbCore(hsb);
    },
    MoveHueIndicator: function(e) {
        var hueIndicator = this.GetHueIndicatorElement();
        var y = ASPx.PrepareClientPosForElement(ASPx.Evt.GetEventY(e), hueIndicator, false);
        y = Math.min(this.hueAreaHeight - Math.round(this.hueIndicatorHeight / 2), Math.max(0 - this.hueIndicatorHeight / 2, y));

        ASPx.SetStyles(hueIndicator, { top: y });

        var hsb = this.GetHsb();
        hsb.hue = this.HueIndicatorPositionToHueValue(y);
        var rgb_color_area = ASPxColorUtils.HsbToRgb({ hue: hsb.hue, saturation: ASPxColorUtils.maxHsb.saturation, brightness: ASPxColorUtils.maxHsb.brightness });
        this.SetColorAreaBackgroundColor(rgb_color_area);

        this.SetColorByHsbCore(hsb);
    },
    ChangePositionHsb: function(hsb) {
        var hueIndicator = this.GetHueIndicatorElement();
        var yHue = this.CalculateHueIndicatorPosition(hsb.hue);
        ASPx.SetStyles(hueIndicator, { top: yHue });

        var colorIndicator = this.GetColorIndicatorElement();
        var xSaturation = this.SaturationToColorIndicatorPosition(hsb.saturation);
        var yBrightness = this.BrightnessToColorIndicatorPosition(hsb.brightness);
        ASPx.SetStyles(colorIndicator, { left: xSaturation, top: yBrightness });

        var rgb_color_area = ASPxColorUtils.HsbToRgb({ hue: hsb.hue, saturation: ASPxColorUtils.maxHsb.saturation, brightness: ASPxColorUtils.maxHsb.brightness });
        this.SetColorAreaBackgroundColor(rgb_color_area);
    },
    CalculateHueIndicatorPosition: function(hue) {
        var hueIndicator = this.GetHueIndicatorElement();
        var hueArea = this.GetHueAreaElement();
        var y = hueArea.offsetHeight - hueArea.offsetHeight * hue / ASPxColorUtils.maxHsb.hue;
        var hueIndicatorOffset = Math.round(this.hueIndicatorHeight / 2);
        y = Math.min(this.hueAreaHeight - hueIndicatorOffset, Math.max(0 - this.hueIndicatorHeight / 2, y - hueIndicatorOffset));
        return Math.round(y);
    },
    SaturationToColorIndicatorPosition: function(saturation) {
        var colorArea = this.GetColorAreaElement();
        var colorIndicator = this.GetColorIndicatorElement();
        var x = colorArea.offsetWidth * saturation / ASPxColorUtils.maxHsb.saturation - this.colorIndicatorWidth / 2;
        return Math.floor(x);
    },
    BrightnessToColorIndicatorPosition: function(brightness) {
        var colorArea = this.GetColorAreaElement();
        var colorIndicator = this.GetColorIndicatorElement();
        var y = colorArea.offsetHeight - colorArea.offsetHeight * brightness / ASPxColorUtils.maxHsb.brightness - this.colorIndicatorHeight / 2;
        return Math.floor(y);
    },
    HueIndicatorPositionToHueValue: function(y) {
        var hueIndicator = this.GetHueIndicatorElement();
        var hueArea = this.GetHueAreaElement();

        var correction = y < 0 ? this.hueIndicatorHeight / 2 : Math.round(this.hueIndicatorHeight / 2);
        y = y + correction;
        var hue = y * ASPxColorUtils.maxHsb.hue / hueArea.offsetHeight;
        hue = ASPxColorUtils.maxHsb.hue - hue;

        return Math.round(hue);
    },
    ColorIndicatorPositionToSaturationValue: function(x) {
        var colorArea = this.GetColorAreaElement();
        var colorIndicator = this.GetColorIndicatorElement();

        var saturation = (x + this.colorIndicatorWidth / 2) * ASPxColorUtils.maxHsb.saturation / colorArea.offsetWidth;
        return Math.floor(saturation);
    },
    ColorIndicatorPositionToBrightnessValue: function(y) {
        var colorArea = this.GetColorAreaElement();
        var colorIndicator = this.GetColorIndicatorElement();

        var brightness = ASPxColorUtils.maxHsb.brightness - (y + this.colorIndicatorHeight / 2) * ASPxColorUtils.maxHsb.brightness / colorArea.offsetHeight;
        return Math.floor(brightness);
    },
    GetColor: function() {
        return ASPxColorUtils.RgbToWebColor(this.GetRgbInput());
    },
    GetColorUtils: function() {
        return ASPxColorUtils;
    }

});

var ASPxColorUtils = {
    maxHsb: { hue: 359, saturation: 100, brightness: 100 },
    maxRgb: { red: 255, green: 255, blue: 255 },
    regExpRgbValid: new RegExp('^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$'),
    IsNormalInteger: function(str) {
        return !isNaN(parseFloat(str)) && isFinite(str);
    },
    ToInt: function(str) {
        return str == "" ? 0 : parseInt(str, 10);
    },
    RgbStyleToObject: function(style) {
        var colorsOnly = style.substring(style.indexOf('(') + 1, style.lastIndexOf(')')).split(/,\s*/);
        return { red: ASPxColorUtils.ToInt(colorsOnly[0]), green: ASPxColorUtils.ToInt(colorsOnly[1]), blue: ASPxColorUtils.ToInt(colorsOnly[2]) };
    },
    RgbObjectToStyle: function(rgb) {
        return "rgb(" + rgb.red + ", " + rgb.green + ", " + rgb.blue + ")";
    },
    IsRgbEquals: function(rgb1, rgb2) {
        return rgb1.red == rgb2.red && rgb1.green == rgb2.green && rgb1.blue == rgb2.blue;
    },
    IsRgbComponentValid: function(component) {
        return ASPxColorUtils.regExpRgbValid.test(component);
    },
    HsbToRgb: function(hsb) {
        var Hi = Math.floor(hsb.hue / 60);
        var Bmin = (ASPxColorUtils.maxHsb.saturation - hsb.saturation) * hsb.brightness / ASPxColorUtils.maxHsb.brightness;
        var a = (hsb.brightness - Bmin) * (hsb.hue % 60) / 60;
        Binc = Bmin + a;
        Bdec = hsb.brightness - a;

        var rgb;
        switch (Hi) {
            case 0:
                rgb = { red: hsb.brightness, green: Binc, blue: Bmin };
                break;
            case 1:
                rgb = { red: Bdec, green: hsb.brightness, blue: Bmin };
                break;
            case 2:
                rgb = { red: Bmin, green: hsb.brightness, blue: Binc };
                break;
            case 3:
                rgb = { red: Bmin, green: Bdec, blue: hsb.brightness };
                break;
            case 4:
                rgb = { red: Binc, green: Bmin, blue: hsb.brightness };
                break;
            case 5:
                rgb = { red: hsb.brightness, green: Bmin, blue: Bdec };
                break;
        }
        rgb.red = Math.round(rgb.red * 2.55);
        rgb.green = Math.round(rgb.green * 2.55);
        rgb.blue = Math.round(rgb.blue * 2.55);

        return rgb;
    },
    RgbToHsb: function(rgb) {
        var rgb = { red: rgb.red / ASPxColorUtils.maxRgb.red, green: rgb.green / ASPxColorUtils.maxRgb.green, blue: rgb.blue / ASPxColorUtils.maxRgb.blue };
        var hsb = {};
        var max = Math.max(rgb.red, rgb.green, rgb.blue);
        var min = Math.min(rgb.red, rgb.green, rgb.blue);

        if(max == min) {
            hsb.hue = 0;
        } else if(max == rgb.red && rgb.green >= rgb.blue) {
            hsb.hue = 60 * (rgb.green - rgb.blue) / (max - min);
        } else if(max == rgb.red && rgb.green < rgb.blue) {
            hsb.hue = 60 * (rgb.green - rgb.blue) / (max - min) + ASPxColorUtils.maxHsb.hue;
        } else if(max == rgb.green) {
            hsb.hue = 60 * (rgb.blue - rgb.red) / (max - min) + 120;
        } else if(max == rgb.blue) {
            hsb.hue = 60 * (rgb.red - rgb.green) / (max - min) + 240;
        }
        hsb.saturation = max == 0 ? 0 : (1 - min / max) * ASPxColorUtils.maxHsb.saturation;
        hsb.brightness = max * ASPxColorUtils.maxHsb.brightness;
        hsb.hue = Math.round(hsb.hue);
        hsb.saturation = Math.round(hsb.saturation);
        hsb.brightness = Math.round(hsb.brightness);
        return hsb;
    },
    RgbToWebColor: function(rgb) {
        return "#" + this.RgbComponentToHex(rgb.red) + this.RgbComponentToHex(rgb.green) + this.RgbComponentToHex(rgb.blue);
    },
    HsbToWebColor: function(hsb) {
        return ASPxColorUtils.RgbToWebColor(ASPxColorUtils.HsbToRgb(hsb));
    },
    RgbComponentToHex: function(component) {

        return ('0' + component.toString(16)).slice(-2).toUpperCase();
    },
    WebColorToRgb: function (webColor) {
        var shorthandRegex = /^#?([a-f\d])([a-f\d])([a-f\d])$/i;
        webColor = webColor.replace(shorthandRegex, function(m, r, g, b) {
            return r + r + g + g + b + b;
        });

        var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(webColor);
        return result ? {
            red: parseInt(result[1], 16),
            green: parseInt(result[2], 16),
            blue: parseInt(result[3], 16)
        } : null;
    }
};

ASPx.ColorPicker = ColorPicker;
})();