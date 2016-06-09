(function() {
    ASPx.HtmlEditorClasses.Commands.SaveAs = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
	    Execute: function(cmdValue, wrapper) {
            wrapper.raiseExport(cmdValue);
	    },
        IsDefaultAction: function(wrapper) {
            return true; 
        },
	    IsImmediateExecution: function() {
	        return true;
	    },
	    IsHtmlChangeable: function() {
	        return false;
	    },
        IsLocked: function(wrapper) {
            return false;
        },
	    IsReversable: function() {
	        return false;
	    },
        IsClientCommand: function() {
            return false;
        },
        canBeExecutedOnSelection: function(wrapper, curSelection) {
            return true;
        }
    });
})();