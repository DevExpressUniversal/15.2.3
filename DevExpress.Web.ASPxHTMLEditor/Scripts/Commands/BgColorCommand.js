(function() {
    ASPx.HtmlEditorClasses.Commands.Browser.BgColor = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Browser.FontColor, {
        Execute: function(cmdValue, wrapper) {
            if(wrapper.attributeIsFiltered("style") || wrapper.styleAttributeIsFiltered("background-color"))
                return false;
            // hack - like Word behavior        
            if (!cmdValue)
                cmdValue = this.TryGetValue(wrapper);
            else
                this.setCurrentBgColor(cmdValue);
            return ASPx.HtmlEditorClasses.Commands.Browser.Command.prototype.Execute.call(this, cmdValue, wrapper);
        },
        GetCommandName: function() {
            return ASPx.Browser.IE ? ASPx.HtmlEditorClasses.Commands.Browser.Command.prototype.GetCommandName.call(this) : "HiliteColor";
        },
        NeedUseCss: function() {
            return true;
        },
        TryGetValue: function(wrapper) {
            return this.getCurrentBgColor();
        },
	    IsLocked: function(wrapper) {
	        if(wrapper.isInFocus)
                return ASPx.HtmlEditorClasses.Commands.Browser.Command.prototype.IsLocked.call(this, wrapper);
	        return false;
	    },
        getCurrentBgColor: function() {
            if(!this.currentBgColor)
                this.currentBgColor = "#ffffff";
            return this.currentBgColor;
        },
        setCurrentBgColor: function(value) {
            this.currentBgColor = value;
        }
    });
})();