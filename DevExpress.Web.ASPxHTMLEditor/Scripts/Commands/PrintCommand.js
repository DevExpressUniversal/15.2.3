(function() {
    ASPx.HtmlEditorClasses.Commands.Print  = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
	    Execute: function(cmdValue, wrapper) {
            var window = wrapper.getWindow();
            if(window) {
                if(ASPx.Browser.IE)
                    window.focus();
                return window.print();
            }
            return false;
	    },
	    IsLocked: function(wrapper) {
	        return ASPx.IsEmptyHtml(wrapper.getHtml());
	    },
	    IsReversable: function() {
	        return false;
	    }
    });
})();