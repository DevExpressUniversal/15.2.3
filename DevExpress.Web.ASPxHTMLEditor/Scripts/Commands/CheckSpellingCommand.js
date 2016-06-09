(function() {
    ASPx.HtmlEditorClasses.Commands.CheckSpelling  = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
	    Execute: function(cmdValue, wrapper) {
		    return wrapper.checkSpelling();
	    },
	    IsLocked: function(wrapper) {
	        return ASPx.IsEmptyHtml(wrapper.getHtml());
	    },
        canBeExecutedOnSelection: function(wrapper, curSelection) {
            return true;
        }
    });
})();