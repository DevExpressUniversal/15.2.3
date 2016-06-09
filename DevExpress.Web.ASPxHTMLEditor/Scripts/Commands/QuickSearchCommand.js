(function () {
    ASPx.HtmlEditorClasses.Commands.Browser.QuickSearch = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Browser.Command, {
        Execute: function(cmdValue, wrapper) {
            return true;
        },
        CanChangeSelection: function() {
            return false;    
        },
        IsHtmlChangeable: function() {
            return false;
        },
        GetState: function(core, selection, selectedElements) {
            return false;
        },
        IsLocked: function(core) {
            return false;
        }
    });
})();