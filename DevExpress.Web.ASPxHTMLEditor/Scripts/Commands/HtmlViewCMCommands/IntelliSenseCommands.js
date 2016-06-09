(function() {
    ASPx.HtmlEditorClasses.Commands.HtmlView.ShowIntelliSenseWindowCommand  = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
        Execute: function(cmdValue, wrapper) {
            wrapper.showIntelliSenseWindow();
	    }
    });
})();