(function() {
    ASPx.HtmlEditorClasses.Commands.TableToolsState = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
	    GetState: function(wrapper, selection) {
            if(!wrapper.isInFocus)
                return false;
            if(!selection)
                selection = wrapper.getSelection();
            var compare = function(element) { 
                return element && element.nodeType == 1 && /^(table|tbody|tfoot|thead|tr|td|caption|col|colgroup)$/gi.test(element.nodeName) ? element : null;
            }
            var selectedElement = selection.GetSelectedElement();
            return selectedElement.nodeType == 1 && selectedElement.nodeName != "IMG" ? !!ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParent(selection.GetSelectedElement(), compare) : false;
        }
    });
})();