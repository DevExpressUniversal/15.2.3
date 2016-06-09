(function() {
    ASPx.HtmlEditorClasses.Commands.Browser.SelectAll  = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Browser.Command, {
        Execute: function(cmdValue, wrapper) {
            var selection = wrapper.getSelection();
            var selectedElement = selection.GetSelectedElement();
            if(selectedElement && ASPx.ElementHasCssClass(selectedElement, ASPx.HtmlEditorClasses.PlaceholderCssClasseName))
                selection.getSpecialSelection().removeHiddenContainer();
            return ASPx.HtmlEditorClasses.Commands.Browser.Command.prototype.Execute.call(this, cmdValue, wrapper);
        },
	    IsHtmlChangeable: function() {
	        return false;
	    },
	    IsLocked: function(wrapper) {
            return ASPx.HtmlEditorClasses.Commands.Browser.Command.prototype.IsLocked.call(this, wrapper);
	    },
        canBeExecutedOnSelection: function(wrapper, curSelection) {
            return true;
        }
    });

    ASPx.HtmlEditorClasses.Commands.HtmlView.SelectAll  = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
        Execute: function(cmdValue, wrapper) {
            var sourceEditor = wrapper.getSourceEditor();
            sourceEditor.execCommand("selectAll");
        },
	    IsHtmlChangeable: function() {
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