/// <reference path="..\_references.js"/>

(function() {
var captchaRefreshCommand = "R";
var captchaImageIDPostfix = "_IMG";
var captchaTextBoxIDPostfix = "_TB";
var captchaRefreshButtonIDPostfix = "_RB";
var captchaRefreshButtonTextSpanIDPostfix = "_RTS";
var ASPxClientCaptcha = ASPx.CreateClass(ASPxClientControl, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.refreshButtonOnClick = function() { this.Refresh(); }.aspxBind(this);
        var image = this.GetImage();
        ASPx.Evt.AttachEventToElement(image, "load", function() {
            this.HideLoadingElements();
            this.GetImage().style.visibility = "";
        }.aspxBind(this));
        var refreshButton = this.GetRefreshButton();
        if(refreshButton != null)
            ASPx.Evt.AttachEventToElement(refreshButton, "click", this.refreshButtonOnClick); 
        this.allowMultipleCallbacks = false;
    },
    
    GetImage: function() {
        return ASPx.GetElementById(this.name + captchaImageIDPostfix);
    },
    
    GetRefreshButton: function() {
        return ASPx.GetElementById(this.name + captchaRefreshButtonIDPostfix);
    },
    
    GetRefreshButtonTextSpan: function() {
        return ASPx.GetElementById(this.name + captchaRefreshButtonTextSpanIDPostfix);
    },
    
    GetEditor: function() {
        return ASPx.GetControlCollection().Get(this.name + captchaTextBoxIDPostfix);
    },
    
    HideLoadingPanelOnCallback: function() {
        return false;
    },
    
    CreateCallback: function(arg, command, callbackInfo) {
        this.ShowLoadingElements();
        ASPxClientControl.prototype.CreateCallback.call(this, arg, command);
    },
    DoBeginCallback: function(command){
        var editor = this.GetEditor();
        if(editor != null) editor.SetIsValid(true);

        ASPxClientControl.prototype.DoBeginCallback.call(this, command);
    },
    ShowLoadingPanel: function(){
        var image = this.GetImage();
        image.style.visibility = "hidden";
        this.CreateLoadingPanelWithAbsolutePosition(this.GetMainElement(), image);
    },
    ShowLoadingDiv: function () {
        this.CreateLoadingDiv(this.GetMainElement(), this.GetImage());
    },
    GetCallbackAnimationElement: function() {
        return this.GetImage();
    },
    
    OnCallback: function(result) {
        var image = this.GetImage();
        image.src = result;
    },
    
    SetEnabled: function(enabled) {
        if(enabled == this.clientEnabled)
            return;
        ASPxClientControl.prototype.SetEnabled.call(this, enabled);
        var refreshButton = this.GetRefreshButton();
        if(refreshButton == null)
            return;
        ASPx.GetStateController().SetElementEnabled(this.GetRefreshButton(), enabled);
        ASPx.GetStateController().SetElementEnabled(this.GetRefreshButtonTextSpan(), enabled);
        var eventsMethod = ASPx.Attr.ChangeEventsMethod(enabled);
        eventsMethod(refreshButton, "click", this.refreshButtonOnClick);
        refreshButton.style.cursor = enabled ? "pointer" : "default";
    },
    Focus: function () {
        var editor = this.GetEditor();
        if(editor)
            editor.SetFocus();
    },
    Refresh: function() {
        if(!this.callBack) {
            if(this.isInitialized)
                this.SendPostBack(captchaRefreshCommand);
            return;
        }
        this.CreateCallback(captchaRefreshCommand);
    }
});
ASPxClientCaptcha.Cast = ASPxClientControl.Cast;

window.ASPxClientCaptcha = ASPxClientCaptcha;
})();