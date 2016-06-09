(function() {
    ASPx.HtmlEditorClasses.Commands.HtmlView.IndentLine = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
        Execute: function(cmdValue, wrapper) {
            var sourceEditor = wrapper.getSourceEditor();
            sourceEditor.execCommand(this.GetCommandID() == ASPxClientCommandConsts.INDENTLINE_COMMAND ? "indentMore" : "indentLess");
            return true;
        },
        GetState: function(wrapper, selection, selectedElements) {
            return false;
        },
        canBeExecutedOnSelection: function(wrapper, curSelection) {
            return true;
        }
    });
})();