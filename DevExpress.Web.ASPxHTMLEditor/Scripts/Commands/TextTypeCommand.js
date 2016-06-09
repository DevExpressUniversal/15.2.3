(function() {
    // ** Text typing **
    ASPx.HtmlEditorClasses.Commands.TextType = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
        IsDefaultAction: function(wrapper) {
            return true;
        },
        IsImmediateExecution: function() {
            return true;
        }
    });
})();