(function() {
    ASPx.HtmlEditorClasses.Commands.HtmlView.FormatDocumentCommand  = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
	    Execute: function(cmdValue, wrapper) {
            wrapper.insertHtml(html_beautify(wrapper.getRawHtml()));
            return true;
	    },
        GetState: function(wrapper, selection, selectedElements) {
            return false;
        },
	    IsLocked: function(wrapper) {
            return false;
	    },
        canBeExecutedOnSelection: function(wrapper, curSelection) {
            return true;
        }
    });
})();