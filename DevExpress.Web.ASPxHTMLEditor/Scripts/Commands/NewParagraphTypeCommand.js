(function() {
    ASPx.HtmlEditorClasses.Commands.NewParagraphType = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
	    Execute: function(cmdValue, wrapper) {
            return wrapper.getDocument().execCommand("InsertParagraph", false, null);
            // todo process if selection in LI
	    }
    });
})();