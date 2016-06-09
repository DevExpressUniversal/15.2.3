/// <reference path="_references.js"/>

(function() {

ASPx.dialogFormCallbackStatus = "DialogForm";
ASPx.currentControlNameInDialog = "";

var areKeyboardEventsInitialized = false;

function aspxAdjustControlsSizeInDialogWindow() {
    var control = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
    var curDialog = control != null ? Dialog.GetLastDialog(control) : null;
    if(curDialog != null) {
        var element = curDialog.GetDialogPopup().GetMainElement();
        ASPx.GetControlCollection().AdjustControls(element, true);
    }
}

var Dialog = ASPx.CreateClass(null, {
	constructor: function(name) {
	    this.name = name;
	    this.initInfo = null;
        this.editorKeyDownProccesed = false;
	    this.keyDownHandlers = {};
        this.InitializeKeyHandlers();
    },
    // Keyboard support
    AddKeyDownHandler: function(shortcut, handler) {
        this.keyDownHandlers[shortcut] = handler;
    },
    InitializeKeyHandlers: function() {
        this.AddKeyDownHandler(ASPx.Key.F11, this.toggleFullScreen.aspxBind(this));
    },
    // virtual
    DoCustomAction: function(result, params) {
    },
    GetDialogCaptionText: function() {
        return "";
    },
    GetInitInfoObject: function() {
        return null;
    },
    InitializeDialogFields: function(initInfo) {
    },
    SetFocusInField: function() {
    },
    toggleFullScreen: function() {
    },
    // Internal function
    Execute: function(ownerControl, popupElementID) {
	    this.ownerControl = ownerControl;
        ASPx.currentControlNameInDialog = this.ownerControl.name;
        Dialog.PushDialogToCollection(this.ownerControl, this);
	    
	    this.InitializePopupEvents();
        this.GetDialogPopup().SetHeaderText(this.GetDialogCaptionText());        
        if(popupElementID)
            this.GetDialogPopup().ShowAtElementByID(popupElementID);
        else
            this.GetDialogPopup().Show();
        
	    if(this.GetDialogContent(this.name) == null) {
            this.SendCallbackForDialogContent();
            this.ShowLoadingPanelInDialogPopup();
        }
        else {
            this.ExecuteInternal(this.GetDialogContent(this.name));
            ASPx.ProcessScriptsAndLinks(ASPx.currentControlNameInDialog);
            this.OnInitComplete();
        }
    },
    ExecuteInternal: function(result) {
        this.initInfo = this.GetInitInfoObject();
        this.GetDialogPopup().SetContentHtml(result);
        if(this.CanUpdatePopupPosition())
            this.UpdatePopupPosition();
    },
    CanUpdatePopupPosition: function() {
        return this.GetDialogPopup().IsVisible();     
    },
    UpdatePopupPosition: function() {
        this.GetDialogPopup().UpdatePosition();        
    },
    GetDialogPopup: function() {
        if(this.ownerControl.GetDialogPopupControl)
            return this.ownerControl.GetDialogPopupControl();
        return null;
    },
    
    AddDialogContentToHash: function(name, content) {
        this.GetDialogContentHashTable()[this.name] = content;
    },
    GetDialogContent: function(name) {    
        return this.GetDialogContentHashTable()[this.name] || null;
    },
    GetDialogContentHashTable: function() {
        return this.ownerControl.dialogContentHashTable || null;
    },
    InitializePopupEvents: function() {
	    var dialogPopup = this.GetDialogPopup();
        if(dialogPopup.CloseButtonClick.IsEmpty()) {
            var owner = this.ownerControl;
            dialogPopup.Closing.AddHandler(Dialog.GetOnClosingEventHandler(owner));
            dialogPopup.CloseButtonClick.AddHandler(Dialog.GetOnCloseButtonClickEventHandler(owner));
	        dialogPopup.CloseUp.AddHandler(Dialog.GetOnCloseEventHandler(owner));
            dialogPopup.Shown.AddHandler(Dialog.GetOnShownEventHandler(owner));
	    }
    },
    InitCustomKeyboardHandling: function() {
        if(!areKeyboardEventsInitialized) {
            areKeyboardEventsInitialized = true;                        
            ASPx.Evt.AttachEventToDocument("keydown", ASPx.DialogDocumentKeydown);
            if(ASPx.Browser.NetscapeFamily)
                this.ReplaceKBSIKeyDown();
        }
    },
    ReplaceKBSIKeyDown: function() { // hack for FireFox
        var original = ASPx.KBSIKeyDown;
        ASPx.KBSIKeyDown = function(name, evt) {
            var isProcessed = original(name, evt);
            var ownerControl = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
            var curDialog = ownerControl != null ? Dialog.GetLastDialog(ownerControl) : null;
	        if(curDialog != null) curDialog.OnInnerEditorKeyDown(evt, isProcessed);
            return isProcessed;
        };
    },
    SendCallbackForDialogContent: function () {
        if (this.ownerControl.useCallbackQueue())
            this.curCallbackToken = this.ownerControl.sendCallbackViaQueue(ASPx.dialogFormCallbackStatus, this.name, false, this);
        else {
            this.ownerControl.callbackOwner = this;
            this.ownerControl.SendCallback(ASPx.dialogFormCallbackStatus, this.name, false);
        }
    },
    ShowLoadingPanelInDialogPopup: function() {
        var dialogPopup = this.GetDialogPopup();
        dialogPopup.SetContentHtml("");
        var contentElement = dialogPopup.GetWindowContentElement(-1);
        this.ownerControl.CreateLoadingDiv(contentElement);
        this.ownerControl.CreateLoadingPanelInsideContainer(contentElement);
        this.ownerControl.RestoreLoadingDivOpacity();
    },
    ShowLoadingPanelOverDialogPopup: function() {
        var offsetElement = ASPx.GetParentByTagName(this.GetDialogPopup().GetWindowContentElement(-1), "table");
        this.ownerControl.CreateLoadingDiv(document.body, offsetElement);
        this.ownerControl.CreateLoadingPanelWithAbsolutePosition(document.body, offsetElement);
        this.ownerControl.RestoreLoadingDivOpacity();
    },
    HideLoadingPanelOverDialogPopup: function() {
        this.ownerControl.HideLoadingElements();
    },    
    HideDialog: function(evt, dontRaiseClosing) {
        if(dontRaiseClosing || !this.OnClosing()) {
            this.HideDialogPopup();
  	        // emulation PopupControl events sequence
            this.OnCloseButtonClick();
  	        this.OnClose();
        }
    },
    HideDialogPopup: function() {
        this.GetDialogPopup().DoHideWindow(-1, true, ASPxClientPopupControlCloseReason.API);
    },
    isPopupVisible: function() {
        return this.GetDialogPopup().InternalIsWindowVisible(-1);
    },
    //Events
    OnCallback: function(result) {
        this.ExecuteInternal(result);
        this.AddDialogContentToHash(this.name, result);
    },
    OnCallbackError: function(result, data) {
        this.ownerControl.callbackOwner = null;
    },
    OnEndCallback: function() {
        var popupClosedOnCallback = !this.isPopupVisible();
        if (popupClosedOnCallback)
            return;

        this.ownerControl.callbackOwner = null;
        this.OnInitComplete();
    },
    OnClosing: function(args) {
        return false;
    },
    OnCloseButtonClick: function () {

    },
    OnClose: function() {
        Dialog.RemoveLastDialog(this.ownerControl);
        if (this.curCallbackToken)
            this.curCallbackToken.cancel();
    },
  	OnComplete: function(result, params) {
  	    this.HideDialogPopup();
	    this.DoCustomAction(result, params);
  	},
    OnDocumentKeyDown: function(evt) {
        if(!this.editorKeyDownProccesed) {
            var handler = this.keyDownHandlers[ASPx.GetShortcutCode(evt.keyCode, evt.ctrlKey, evt.shiftKey, evt.altKey)];
            if(handler) {
                ASPx.Evt.PreventEvent(evt);
                handler(evt);
            }
        }
        this.editorKeyDownProccesed = false;
    },
    // hack for FireFox
    OnInnerEditorKeyDown: function(evt, isProcessed) {
        this.editorKeyDownProccesed = ASPx.IsExists(isProcessed) ? !isProcessed : false;
    },
    
    OnInitComplete: function() {
        this.InitCustomKeyboardHandling();
        this.InitializeDialogFields(this.initInfo);
        this.SetFocusInField();
    },
    OnShown: function(args) {

    }
});
Dialog.GetOnClosingEventHandler = function(owner) {
    return Dialog.GetPopupEventHandlerCore(owner, function(curDialog, args) {
        curDialog.OnClosing(args);
    });
};
Dialog.GetOnCloseButtonClickEventHandler = function(owner) {
    return Dialog.GetPopupEventHandlerCore(owner, function(curDialog, args) {
        curDialog.OnCloseButtonClick();
    });
};
Dialog.GetOnCloseEventHandler = function(owner) {
    return Dialog.GetPopupEventHandlerCore(owner, function(curDialog, args) {
        curDialog.OnClose();
    });
};
Dialog.GetOnShownEventHandler = function(owner) {
    return Dialog.GetPopupEventHandlerCore(owner, function(curDialog, args) {
        curDialog.OnShown(args);
    });
};
Dialog.GetPopupEventHandlerCore = function(owner, action) {
    return function(s, e) {
        var curDialog = Dialog.GetLastDialog(owner);
        if(curDialog)
            return action(curDialog, e);
    };
};
// DialogUtils
Dialog.PushDialogToCollection = function(ownerControl, dialog) {
    if(!ownerControl.dialogArray)
        ownerControl.dialogArray = [ ];
    ownerControl.dialogArray.push(dialog);
};
Dialog.GetLastDialog = function(ownerControl) {
    if(ownerControl.dialogArray) {
        var length = ownerControl.dialogArray.length;
        return length > 0 ? ownerControl.dialogArray[length - 1] : null;
    }
    return null;
};
Dialog.RemoveLastDialog = function(ownerControl) {
    var array = ownerControl.dialogArray;
    if(array && array.length > 0)
        ASPx.Data.ArrayRemoveAt(array, array.length - 1);
};
Dialog.GetOwnerControl = function(name) {
    return ASPx.GetControlCollection().Get(name ? name : ASPx.currentControlNameInDialog);
};
Dialog.GetCurrentDialog = function(name) {
    var ownerControl = Dialog.GetOwnerControl(name);
    return ownerControl ? Dialog.GetLastDialog(ownerControl) : null;
};
ASPx.Dialog = Dialog;

// ---------------------------------------------------------------------------------------------------
ASPx.DialogComplete = function(result, params) {
    var curDialog = Dialog.GetCurrentDialog();
	if(curDialog != null)
	    return curDialog.OnComplete(result, params);
}
ASPx.DialogDocumentKeydown = function (evt) {
    var curDialog = Dialog.GetCurrentDialog();
	if(curDialog != null)
	    curDialog.OnDocumentKeyDown(evt);
}
})();