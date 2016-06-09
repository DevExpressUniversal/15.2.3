(function() {
    ASPx.HtmlEditorClasses.Commands.Browser.FontColor = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Browser.Command, {
        Execute: function(cmdValue, wrapper) {
            if(wrapper.attributeIsFiltered("style") || wrapper.styleAttributeIsFiltered("color"))
                return false;
            // hack - like Word behavior
            if (!cmdValue)
                cmdValue = this.getCurrentFontColor();
            else
                this.setCurrentFontColor(cmdValue);

            var hyperlinks;
            if(ASPx.Browser.IE || ASPx.Browser.WebKitFamily)
                hyperlinks = this.SaveHyperlinks(wrapper);
        
            if(ASPx.Browser.Firefox && ASPx.Browser.Version >= 4) { //B187553
                var contentAreaDoc = wrapper.getDocument();
                contentAreaDoc.execCommand("useCSS", false, false);
                var commandExecResult = ASPx.HtmlEditorClasses.Commands.Browser.Command.prototype.Execute.call(this, cmdValue, wrapper);
                contentAreaDoc.execCommand("useCSS", false, true);
            }
            else
                var commandExecResult = ASPx.HtmlEditorClasses.Commands.Browser.Command.prototype.Execute.call(this, cmdValue, wrapper);
        
            if(hyperlinks)
                this.RestoreHyperlinks(hyperlinks);

            return commandExecResult;
        },
        GetValue: function(wrapper, selection, selectedElements) {
            return ASPx.Color.ColorToHexadecimal(ASPx.HtmlEditorClasses.Commands.Browser.Command.prototype.GetValue.call(this, wrapper));
        },
        NeedUseCss: function() {
            return !ASPx.Browser.NetscapeFamily;
        },
        TryGetValue: function(wrapper) {
            return this.getCurrentFontColor();
        },
        SaveHyperlinks: function(wrapper) {
            var hyperlinks = [ ];

            var selection = ASPxClientHtmlEditorSelection.Create(wrapper.getWindow());
            var selectionContainer = selection.GetParentElement();

            var anchors = ASPx.GetNodesByTagName(selectionContainer, "A");
            for(var i = 0; i < anchors.length; i++) {
                var anchor = anchors[i];
                if(this.IsHyperlink(anchor)) {
                    hyperlinks.push(
                        {
                            element: anchor,
                            innerHTML: anchor.innerHTML
                        }
                    );
                }
            }
        
            return hyperlinks;
        },
        RestoreHyperlinks: function(hyperlinks) {
            for(var i = 0; i < hyperlinks.length; i++) {
                var hyperlink = hyperlinks[i];
                hyperlink.element.innerHTML = hyperlink.innerHTML;
            }
        },
        IsHyperlink: function(anchor) {
            return typeof(anchor.href) == "string" && anchor.href.length > 0;
        },
	    IsLocked: function(wrapper) {
	        if(wrapper.isInFocus)
                return ASPx.HtmlEditorClasses.Commands.Browser.Command.prototype.IsLocked.call(this, wrapper);
	        return false;
	    },
        getCurrentFontColor: function() {
            if(!this.currentFontColor)
                this.currentFontColor = "#ffffff";
            return this.currentFontColor;
        },
        setCurrentFontColor: function(value) {
            this.currentFontColor = value;
        }
    });
})();