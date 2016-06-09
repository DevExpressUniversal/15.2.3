(function() {
    ASPx.HtmlEditorClasses.Commands.KbCopy = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.TextType, {
        Execute: function(cmdValue, wrapper) {
            if(wrapper.settings.placeholders && wrapper.settings.placeholders.length > 0 &&  wrapper.placeholderManager.getFildsCount() > 0) {
                wrapper.placeholderManager.setAllowEditFilds(true);
                setTimeout(function() {
                    wrapper.placeholderManager.setAllowEditFilds(false);
                    var selectedElement = wrapper.getSelection().GetSelectedElement();
                    if(ASPx.HtmlEditorClasses.Utils.needToUseSpecialSelection(selectedElement)) {
                        wrapper.getSelection().clientSelection.applySpecialSelectionToElement(selectedElement);
                        if(ASPx.Browser.IE && wrapper.isInFocus)
                            wrapper.eventManager.setPreventEventFlag();
                    }
                }.aspxBind(this), 0);
            }
        },
        IsHtmlChangeable: function() {
	        return false;
	    },
        canBeExecutedOnSelection: function(wrapper) {
            return true;
        }
    });
})();