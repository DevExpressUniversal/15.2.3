ASPxStandartTestControl_JS = function EasyTestJScripts_ASPxStandartTestControl_JS(id, caption) {
    this.inherit = ASPxStandartTestControl_JS.inherit;
    this.inherit(id, caption);
    this.SetText = function(value) {
        if (this.control.inputElement.readOnly) {
            this.LogOperationError('The ""' + this.caption + '"" editor is readonly.');
            return;
        }
        this.control.SetValue(value);
        if (this.autoPostBack) {
            xafDoPostBack(this.control.name, '');
        }
    }
    this.InitControl = function() {
        this.control = window[this.id.replace(/\$/g, '_')];
        if (this.control) {
            return;
        }
        var f = this.inherit.prototype.baseInitControl;
        f.call(this);
        if (this.error) {
            return;
        }
    }
    this.IsEnabled = function() {
        hasMainElementMethod = false;
        isMainElementEnabled = false;
        if (this.control.GetMainElement) {
            hasMainElementMethod = true;
            isMainElementEnabled = !this.control.GetMainElement().isDisabled;
        }
        hasGetEnabledMethod = false;
        isEnabled = false;
        if (this.control.GetEnabled) {
            hasGetEnabledMethod = true;
            isEnabled = this.control.GetEnabled();
        }
        //B150245
        if ((hasGetEnabledMethod && hasMainElementMethod) && (isEnabled && !isMainElementEnabled)) {
            return isMainElementEnabled;
        }
        if (hasGetEnabledMethod) {
            return isEnabled;
        }
        if (this.control.enabled != true && this.control.enabled != false) {
            if (this.control.GetInputElement) {
                return !this.control.GetInputElement().isDisabled;
            } else {
                return false;
            }
        }
        return this.control.enabled;
    }
    this.GetClientControlById = function(id) {
        return window[id];
    }
}