(function() {
    ASPx.HtmlEditorClasses.Commands.Redo = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
	    Execute: function(cmdValue, wrapper) {
            wrapper.focus();
		    return this.owner.redo();
	    },
	    IsReversable: function() {
	        return false;
	    },
	    IsLocked: function(wrapper) {
	        return !this.owner.isRedoAvailable();
	    },
        canBeExecutedOnSelection: function(wrapper) {
            return true;
        }
    });

    ASPx.HtmlEditorClasses.Commands.HtmlView.Redo = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
	    Execute: function(cmdValue, wrapper) {
            wrapper.sourceEditor.redo();
	    },
	    IsReversable: function() {
	        return false;
	    },
	    IsLocked: function(wrapper) {
	        return wrapper.sourceEditor.historySize().redo < 1;
	    },
        canBeExecutedOnSelection: function(wrapper) {
            return true;
        }
    });
})();