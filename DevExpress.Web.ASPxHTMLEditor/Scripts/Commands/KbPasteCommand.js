(function() {
    ASPx.HtmlEditorClasses.Commands.KbPaste = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.TextType, { 
        canBeExecutedOnSelection: function(wrapper) {
            return true;
        }
    });
})();