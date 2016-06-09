(function() {
    ASPx.HtmlEditorClasses.Commands.Browser.Clipboard = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Browser.Command, {
	    Execute: function(cmdValue, wrapper) {
            var result = false;
            if(ASPx.HtmlEditorClasses.Commands.Browser.Clipboard.IsAllowed[this.commandID])
                result = this.ExecuteInternal(wrapper);
            else
                alert(ASPx.HtmlEditorClasses.Commands.Browser.Clipboard.NotAllowedMessage[this.commandID]);
            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion > 8)
                wrapper.saveSelection();
            wrapper.focus();
            return result;
	    },
	    ExecuteInternal: function(wrapper) {
		    var result = false;
            var isEnabled = true;
		    if(ASPx.Browser.IE) {
		        try {
			        wrapper.getDocument().queryCommandEnabled(this.commandID); 
	            }
		        catch (e) { 
		            isEnabled = false;
		        }
		    }
		    if(isEnabled) {
		        if(ASPx.Browser.IE) {
                    var selection = wrapper.getSelection();
                    if(this.GetCommandID() == ASPxClientCommandConsts.PASTE_COMMAND) {
                        if(wrapper.settings.enablePasteOptions || wrapper.settings.pasteMode == ASPx.HtmlEditorClasses.PasteMode.MergeFormatting)
                            wrapper.commandManager.getCommand(ASPxClientCommandConsts.PASTEHTMLMERGEFORMATTING_COMMAND).initStyleStateArray(wrapper);
		                ASPx.HtmlEditorClasses.Utils.createPasteContainer(wrapper);
                        if(wrapper.commandManager.getLastPasteFormattingHtml())
                            wrapper.commandManager.clearPasteOptions(false);
                    }
                    else {
                        ASPx.HtmlEditorClasses.Commands.Utils.processingElementsBeforeCope(wrapper.getSelection().GetElements());
                        if(this.GetCommandID() == ASPxClientCommandConsts.CUT_COMMAND) {
                            selection.getSpecialSelection().removeHiddenContainer();
                            ASPx.HtmlEditorClasses.Commands.Browser.InsertList.UpdateListNumbering(wrapper.getBody());
                        }
                        else {
                            var selectedElement = selection.GetSelectedElement();
                            if(ASPx.ElementHasCssClass(selectedElement, ASPx.HtmlEditorClasses.SelectedContainerCssClasseName))
                                selection.getSpecialSelection().applySpecialSelectionToElement(selectedElement, true);
                        }
                    }
                }
		        result = wrapper.getDocument().execCommand(this.commandID, false, null);
                if(wrapper.settings.placeholders && wrapper.settings.placeholders.length > 0 &&  wrapper.placeholderManager.getFildsCount() > 0) {
                    wrapper.placeholderManager.setAllowEditFilds(false);
                    var selectedElement = wrapper.getSelection().GetSelectedElement();
                    if(ASPx.HtmlEditorClasses.Utils.needToUseSpecialSelection(selectedElement))
                        wrapper.getSelection().getSpecialSelection().applySpecialSelectionToElement(selectedElement);
                }
		    }
            return result;
	    },
        TryGetIsLocked: function(wrapper) {
	        if(!ASPx.Browser.IE && (this.commandID == ASPxClientCommandConsts.COPY_COMMAND || this.commandID == ASPxClientCommandConsts.CUT_COMMAND || this.commandID == ASPxClientCommandConsts.PASTE_COMMAND))
                return true;
            if(ASPx.Browser.Safari)
                return false;
            if(ASPx.Browser.IE && this.commandID == ASPxClientCommandConsts.PASTE_COMMAND)
                return false;
            return ASPx.HtmlEditorClasses.Commands.Browser.Command.prototype.TryGetIsLocked.call(this, wrapper);
	    },
	    IsHtmlChangeable: function() {
	        return this.GetCommandID() != ASPxClientCommandConsts.COPY_COMMAND;
	    },
        canBeExecutedOnSelection: function(wrapper) {
            return true;
        }
    });
    ASPx.HtmlEditorClasses.Commands.Browser.Clipboard.NotAllowedMessage = {};
    ASPx.HtmlEditorClasses.Commands.Browser.Clipboard.NotAllowedMessage[ASPxClientCommandConsts.CUT_COMMAND] = "This command cannot be executed. Please use Ctrl+X to Cut to the clipboard.";
    ASPx.HtmlEditorClasses.Commands.Browser.Clipboard.NotAllowedMessage[ASPxClientCommandConsts.PASTE_COMMAND] = "This command cannot be executed. Please use Ctrl+V to Paste from the clipboard.";
    ASPx.HtmlEditorClasses.Commands.Browser.Clipboard.NotAllowedMessage[ASPxClientCommandConsts.COPY_COMMAND] = "This command cannot be executed. Please use Ctrl+C to Copy to the clipboard.";

    // This commands doesn't work in all browsers due to security reasons
    // We should check this limitations from time to time
    ASPx.HtmlEditorClasses.Commands.Browser.Clipboard.IsAllowed = {};
    ASPx.HtmlEditorClasses.Commands.Browser.Clipboard.IsAllowed[ASPxClientCommandConsts.CUT_COMMAND] = ASPx.Browser.IE || ASPx.Browser.Chrome;
    ASPx.HtmlEditorClasses.Commands.Browser.Clipboard.IsAllowed[ASPxClientCommandConsts.PASTE_COMMAND] = ASPx.Browser.IE;
    ASPx.HtmlEditorClasses.Commands.Browser.Clipboard.IsAllowed[ASPxClientCommandConsts.COPY_COMMAND] = ASPx.Browser.IE || ASPx.Browser.Chrome;
})();