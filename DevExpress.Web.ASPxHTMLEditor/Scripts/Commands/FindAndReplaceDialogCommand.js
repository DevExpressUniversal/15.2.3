(function () {
    var SearchCommand = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Browser.Command, {
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
    
    ASPx.HtmlEditorClasses.Commands.Browser.QuickSearch = ASPx.CreateClass(SearchCommand, {
        //{selectedText :''}
        Execute: function(cmdValue, wrapper) {
            wrapper.showQuickSearch(cmdValue);
            return true;
        }
    });

    ASPx.HtmlEditorClasses.Commands.Browser.FindAndReplaceDialog = ASPx.CreateClass(SearchCommand, {
        //{selectedText :''}
        Execute: function (cmdValue, wrapper) {
            wrapper.showAdvancedSearch(cmdValue);
            return true;
        }
    });

})();