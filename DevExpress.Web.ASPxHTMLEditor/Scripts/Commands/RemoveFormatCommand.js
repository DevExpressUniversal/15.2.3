(function() {
    ASPx.HtmlEditorClasses.Commands.Browser.RemoveFormat = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Browser.Command, {
	    Execute: function(cmdValue, wrapper) {
            var bookmark;
            var sel = wrapper.getSelection();
            var selectedElement = sel.GetSelectedElement();
            if(wrapper.placeholderManager.getFildsCount() > 0) {
                bookmark = sel.clientSelection.GetExtendedBookmark();
                var html = wrapper.getRawHtml();
                html = wrapper.placeholderManager.replacePlaceholderElementToTextLabel(html);
                ASPx.SetInnerHtml(wrapper.getBody(), html);
                wrapper.getSelection().clientSelection.SelectExtendedBookmark(bookmark);
                this.executeCore(cmdValue, wrapper);

                var elements = wrapper.getSelection().GetElements();
                bookmark = ASPx.HtmlEditorClasses.Selection.getBookmarkByElements(wrapper.getDocument(), elements[0], elements[elements.length - 1]);
                html = wrapper.getHtml();
                wrapper.setHtml(html);
                wrapper.getSelection().clientSelection.SelectExtendedBookmark(bookmark);
            }
            else
                this.executeCore(cmdValue, wrapper);
		    return true;
	    },
        executeCore: function(cmdValue, wrapper) {
	        ASPx.HtmlEditorClasses.Commands.Browser.Command.prototype.Execute.call(this, cmdValue, wrapper);
            var sel = wrapper.getSelection();
            var processingElements = ASPx.HtmlEditorClasses.Utils.getBeforeBlockParentElements(sel.GetElements());
            if(processingElements.length > 0) {
                bookmark = ASPx.HtmlEditorClasses.Selection.getBookmarkByElements(wrapper.getDocument(), processingElements[0], processingElements[processingElements.length - 1]);
                for(var i = 0, el; el = processingElements[i]; i++)
                    this.CleanElement(el);
                for(var i = 0, element; element = processingElements[i]; i++)
                    this.removeEmptySpans(element);
                wrapper.getSelection().clientSelection.SelectExtendedBookmark(bookmark);
            }
        },
	    CleanElement: function(element) {
            for (var i = 0; i < element.childNodes.length ; i++)
                this.CleanElement(element.childNodes[i]);
            if (element.nodeType == 1 && element.tagName.toUpperCase() != "BODY") {
                ASPx.Attr.RemoveAllAttributes(element, ["href", "src", "alt", "target", "id", "title", "value"]);
                ASPx.Attr.RemoveAllStyles(element);
            }
	    },
	    IsLocked: function(wrapper) {
	        if(wrapper.isInFocus)
                return ASPx.HtmlEditorClasses.Commands.Browser.Command.prototype.IsLocked.call(this, wrapper);
	        return true;
	    },
        removeEmptySpans: function(node) {
            var spanElements = [];
            if(node.nodeName == "SPAN")
                spanElements.push(node);
            spanElements = spanElements.concat(ASPx.GetChildNodesByTagName(node, "SPAN"));
            for (var i = spanElements.length - 1; i >= 0 ; i--) {
                var target = spanElements[i];
                if (!ASPx.HtmlEditorClasses.Utils.elementAttributesContains(target)) {
                    var content = target.innerHTML,
                    parent = target.parentNode;
                    target.insertAdjacentHTML('beforeBegin', content)
                    parent.removeChild(target);
                }
            }
        }
    });
})();