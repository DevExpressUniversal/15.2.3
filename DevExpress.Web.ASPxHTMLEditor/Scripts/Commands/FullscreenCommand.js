(function() {
    ASPx.HtmlEditorClasses.Commands.Fullscreen = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
	    Execute: function(cmdValue, wrapper) {
		    this.isFullscreenMode = !this.isFullscreenMode;
            wrapper.setFullscreenMode(wrapper, this.isFullscreenMode);
            return true;
	    },
	    IsLocked: function(wrapper) {
	        return false;
	    },
	    IsReversable: function() {
	        return false;
	    },
	    GetState: function(wrapper) {
            return this.isInFullscreen();
        },
        isInFullscreen: function() {
            return !!this.isFullscreenMode;
        },
        canBeExecutedOnSelection: function(wrapper) {
            return true;
        }
    });
})();