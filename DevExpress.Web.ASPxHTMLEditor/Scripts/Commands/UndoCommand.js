(function() {
    ASPx.HtmlEditorClasses.Commands.Undo = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
	    Execute: function(cmdValue, wrapper) {
            wrapper.focus();
		    return this.owner.undo();
	    },
	    IsReversable: function() {
	        return false;
	    },
	    IsLocked: function(wrapper) {
	        return !this.owner.isUndoAvailable();
	    },
        canBeExecutedOnSelection: function(wrapper) {
            return true;
        }
    });

    ASPx.HtmlEditorClasses.Commands.HtmlView.Undo = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
	    Execute: function(cmdValue, wrapper) {
            wrapper.sourceEditor.undo();
	    },
	    IsReversable: function() {
	        return false;
	    },
	    IsLocked: function(wrapper) {
	        return wrapper.sourceEditor.historySize().undo < 1;
	    },
        canBeExecutedOnSelection: function(wrapper) {
            return true;
        }
    });
})();