(function() {
    ASPx.HtmlEditorClasses.Commands.CheckSpellingCore = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
	    Execute: function(cmdValue, wrapper) {
		    wrapper.setHtml(cmdValue);
		    return true;
	    }
    });
})();