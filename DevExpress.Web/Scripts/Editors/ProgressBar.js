/// <reference path="..\_references.js"/>

(function() {
var ASPxClientProgressBar = ASPx.CreateClass(ASPxClientEditBase, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        
        this.sizingConfig.adjustControl = true;
    },
    InlineInitialize: function() {
        var progressBar = this.GetProgressBar();

        progressBar.mainElement = this.GetMainElement();
        progressBar.InlineInitialize(true /* calledByOwner */);

        ASPxClientEditBase.prototype.InlineInitialize.call(this);
    },
    AdjustControlCore: function() {
        ASPxClientEditBase.prototype.AdjustControlCore.call(this);    
        this.GetProgressBar().AdjustControlCore();
    },
    GetProgressBar: function() {
        return window[this.name + "_MC"];
    },
    ChangeEnabledStateItems: function(enabled){
        ASPx.GetStateController().SetElementEnabled(this.GetMainElement(), enabled);
        var valueIndicatorCell = this.GetProgressBar().GetValueIndicatorCell();
        if(valueIndicatorCell)
            ASPx.GetStateController().SetElementEnabled(valueIndicatorCell, enabled);
    },

    // API
    SetPosition: function(position) {
        this.GetProgressBar().SetPosition(position);
    },
    GetPosition: function() {
        return this.GetProgressBar().GetPosition();
    },
    SetCustomDisplayFormat: function(text) {
        this.GetProgressBar().SetCustomDisplayFormat(text);
    },
    GetDisplayText: function() {
        return this.GetProgressBar().GetDisplayText();
    },
    GetPercent: function() {
        return this.GetProgressBar().GetPercent();
    },
    SetMinimum: function(min) {
        this.SetMinMaxValues(min, this.GetMaximum());
    },
    SetMaximum: function(max) {
        this.SetMinMaxValues(this.GetMinimum(), max);
    },
    GetMinimum: function() {
        return this.GetProgressBar().minimum;
    },
    GetMaximum: function() {
        return this.GetProgressBar().maximum;
    },
    SetMinMaxValues: function(min, max) {
        this.GetProgressBar().SetMinMaxValues(min, max);
    },
    SetHeight: function(height) {
        if(height < this.GetHeight())
            this.GetProgressBar().ResetIndicatorHeight();
        ASPxClientControl.prototype.SetHeight.call(this, height);
    },
    SetValue: function(value) {
        this.SetPosition(value);
    },
    GetValue: function() {
        return this.GetPosition();
    }
});
ASPxClientProgressBar.Cast = ASPxClientControl.Cast;

window.ASPxClientProgressBar = ASPxClientProgressBar;
})();