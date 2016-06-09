/// <reference path="_references.js"/>
/// <reference path="HtmlEditorConstants.js"/>
/// <reference path="HtmlEditorCore.js"/>
/// <reference path="EventManager.js"/>
(function() {
    var pasteOptionsBarControlIDSuffix = "_POB";
    ASPx.HtmlEditorClasses.Controls.PasteOptionsBarManager = ASPx.CreateClass(null, {
	    constructor: function(htmlEditor) {
	        this.htmlEditor = htmlEditor;
            this.core = htmlEditor.core;
            this.pasteOptionsBarControl = this.getPasteOptionsBarControl();
            if(this.pasteOptionsBarControl)
                this.hideBarImmediately();
        },
        getPasteOptionsBarControl: function() {
            return ASPx.GetControlCollection().Get(this.htmlEditor.name + pasteOptionsBarControlIDSuffix);
        },
        showBar: function() {
            if(this.pasteOptionsBarControl && !this.isBarVisible()) {
                this.pasteOptionsBarControl.SetVisible(true);
                ASPx.AnimationHelper.fadeIn(this.pasteOptionsBarControl.GetMainElement(), null, 200);
            }
        },
        hideBar: function() {
            if(this.pasteOptionsBarControl && this.isBarVisible()) {
                var func = function() {
                    this.pasteOptionsBarControl.SetVisible(false);
                }.aspxBind(this);
                ASPx.AnimationHelper.fadeOut(this.pasteOptionsBarControl.GetMainElement(), func, 50);
            }
        },
        hideBarImmediately: function() {
            if(this.pasteOptionsBarControl) {
                this.pasteOptionsBarControl.SetVisible(false);
                ASPx.AnimationHelper.setOpacity(this.pasteOptionsBarControl.GetMainElement(), 0);
            }
        },
        isBarVisible: function() {
            return this.pasteOptionsBarControl && ASPx.GetElementOpacity(this.pasteOptionsBarControl.GetMainElement()) > 0;
        },
        updatePasteFormattingItemsState: function() {
            if(!this.pasteOptionsBarControl)
                return;
            var wrapper = this.core.getActiveWrapper();
            var selection = wrapper.getSelection();

            for(var i = 0; i < this.pasteOptionsBarControl.GetItemCount(); i++) {
                var item = this.pasteOptionsBarControl.GetItem(i);
                var command = this.htmlEditor.getCommandManager().getCommand(item.name);
                if(command)
                    item.SetChecked(command.GetState(wrapper, selection));
            }
        }
    });
})();