(function() {
    // Delete without selection. Needn't do anything. Browser must delete content himself.
    ASPx.HtmlEditorClasses.Commands.DeleteWithoutSelection = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
	    Execute: function(cmdValue, wrapper) {
            var selectedElement = wrapper.getSelection().GetSelectedElement();
            if(ASPx.HtmlEditorClasses.Utils.needToUseSpecialSelection(selectedElement))
                wrapper.commandManager.executeCommand(ASPxClientCommandConsts.DELETEPLACEHOLDER_COMMAND, null, true);
            else {
                ASPx.HtmlEditorClasses.Commands.Utils.correctSelectionForDeleteCommand(wrapper);
	            if(ASPx.Browser.WebKitFamily && selectedElement.parentNode.nodeName == "BODY" && selectedElement.parentNode.childNodes.length == 1 && (selectedElement.childNodes.length == 1 && selectedElement.childNodes[0].nodeName == "BR" || selectedElement.childNodes.length == 0)) {
                    selectedElement.parentNode.removeChild(selectedElement);
                    wrapper.getSelection().SetFocusToDocumentStart();
                }
                if(ASPx.Browser.NetscapeFamily) {
	                var IFrameBody = wrapper.getBody();
		            window.setTimeout(function() {
		                ASPx.HtmlEditorClasses.Commands.Enter.RemoveBogusNodes(IFrameBody);
		            }, 0);
		        }
            }
	        return true;
	    },
        IsImmediateExecution: function() {
            return true;
        }
    });
})();