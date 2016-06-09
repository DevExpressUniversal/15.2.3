// BaseDialog
(function() {
var SCDialog = ASPx.CreateClass(ASPx.Dialog, {
	constructor: function(name, spellChecker) {
        this.constructor.prototype.constructor.call(this, name);
	    this.initExecuting = false;
	    this.spellChecker = spellChecker;
	    this.ownerControl = spellChecker;
    },
        
    ShowLoadingPanelInDialogPopup: function() {
    },
    Show: function(popupElement) {
        ASPx.Dialog.PushDialogToCollection(this.ownerControl, this);
        
        this.InitializePopupEvents();
        this.GetDialogPopup().SetHeaderText(this.GetDialogCaptionText());
        this.GetDialogPopup().ShowAtElement(popupElement);
    },
    InitializePopupEvents: function() {
	    this.GetDialogPopup().CloseUp.AddHandler(function(s ,e) { ASPx.SCDialogComplete(false); });
    },
    SetDialogContent: function(dialogContent) {
        var popupControl = this.GetDialogPopup();
        popupControl.SetContentHtml(dialogContent);
        if(ASPx.Browser.Chrome)
            popupControl.AdjustSize();      // Q488371
        this.AddDialogContentToHash(this.name, dialogContent);
        this.GetDialogPopup().UpdatePosition();
    },
    HideDialog: function() {
        ASPx.SCDialogComplete(false);
    },    

    GetDialogPopup: function() {
        return this.spellChecker.GetDialogPopupControl(this.name);    
    },
    IsVisible: function(){
        return this.GetDialogPopup().IsVisible();
    },
    OnInitComplete: function() {
        ASPx.Dialog.prototype.OnInitComplete.call(this);
        this.GetDialogPopup().UpdatePosition();
    }
});

// SpellCheckForm
var SpellCheckForm = ASPx.CreateClass(SCDialog , {
    // virtual
    DoCustomAction: function(result) {
    },
    GetDialogCaptionText: function() {
        return this.spellChecker.spellCheckFormCaption;
    },
    SendCallbackForDialogContent: function() {
    },
    SetFocusInField: function() {
        if(this.spellChecker.formHandler.GetSCFormChangeTextBox())
            ASPx.SetFocusToTextEditWithDelay(_dxeSCTxtChangeTo.name);
    },
    
    HideLoadingPanelOverDialogPopup: function() {
        SCDialog.prototype.HideLoadingPanelOverDialogPopup.call(this);
        if(this.spellChecker.formHandler.GetSCFormChangeButton())
            _dxeSCBtnChange.SetEnabled(true);
    },
    ShowLoadingPanelOverDialogPopup: function() {
        SCDialog.prototype.ShowLoadingPanelOverDialogPopup.call(this);
        if(this.spellChecker.formHandler.GetSCFormChangeButton())
            _dxeSCBtnChange.SetEnabled(false);
    }
});

// SpellCheckOptionsForm
var SpellCheckOptionsForm = ASPx.CreateClass(SCDialog , {    
    // virtual
    InitializePopupEvents: function() {
        SCDialog.prototype.InitializePopupEvents.call(this);
	    this.GetDialogPopup().AddKeyDownHandler("ESC", function() { ASPx.SCDialogComplete(false); });
    },
    DoCustomAction: function(result, params) {
    },      
    GetDialogCaptionText: function() {
        return this.spellChecker.optionsFormCaption;
    },
    SetFocusInField: function() {
    }
});

ASPx.SCDialogComplete = function(result) {
    if(ASPx.activeSpellChecker)
        ASPx.activeSpellChecker.HideDialog(result);
}

ASPx.SpellCheckForm = SpellCheckForm;
ASPx.SpellCheckOptionsForm = SpellCheckOptionsForm;
})();