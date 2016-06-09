(function() {
    ASPx.HtmlEditorClasses.Commands.Browser.FormatBlock = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Browser.Command, {
        Execute: function(cmdValue, wrapper) {
            if(wrapper.tagIsFiltered(cmdValue))
                return false;
            cmdValue = cmdValue.toUpperCase();
            if(ASPx.HtmlEditorClasses.Commands.Utils.IsInlineTextElement(cmdValue))
                return;
            var selection = wrapper.getSelection();
            var doc = wrapper.getDocument();
            var tempNodeId = "tempSpan";
            var elements = [];
            var isCollapsed = selection.IsCollapsed();
            if(!isCollapsed)
                elements = selection.GetElements(true);
            var selectionBookmarks = elements.length == 0 ? wrapper.getSelection().clientSelection.GetExtendedBookmark() : ASPx.HtmlEditorClasses.Commands.Utils.createBookmarkElements(doc, elements[0], elements[elements.length - 1]);
            if(elements.length == 0 || isCollapsed)
                elements = [ ASPx.HtmlEditorClasses.Commands.Utils.InsertNewElement(doc, selection, tempNodeId) ];

            var newElements = [];
            for(var i = 0, element; element = elements[i]; i++) {
                if(!element.parentNode)
                    continue;
                var inlineNodes = [];
                var isTextNode = element.nodeType == 3;
                if(isTextNode)
                    inlineNodes.push(element);
                else if(!selection.IsCollapsed())
                    inlineNodes = this.FindAllTextNodes(element);
                else
                    inlineNodes.push(element);
                var headerExpr = /H[1-6]/;
                for(var j = 0, node; node = inlineNodes[j]; j++) {
                    var node = this.FindInlineTextParent(node) || node;
                    if(ASPx.Data.ArrayIndexOf(newElements, node.parentNode) > -1)
                        continue;
                    var parent = node.parentNode;
                    if(parent.tagName !== cmdValue) {
                        var elementContainer = doc.createElement(cmdValue);
                        var inlineElements = ASPx.HtmlEditorClasses.Commands.Utils.GetBetweenBlocksInlineElements(node);
                        var nextSibling = inlineElements[inlineElements.length - 1].nextSibling;
                        for(var k = 0, elem; elem = inlineElements[k]; k++)
                            elementContainer.appendChild(elem);
                        if(parent.childNodes.length > 0) {
                            if(!nextSibling)
                                parent.appendChild(elementContainer);
                            else
                                parent.insertBefore(elementContainer, nextSibling);
                        }
                        else if(parent.nodeName == "BODY" || parent.nodeName == "TD" || parent.nodeName == "TH" || parent.nodeName == "LI")
                            parent.appendChild(elementContainer);
                        else {
                            parent.parentNode.insertBefore(elementContainer, parent);
                            ASPx.Attr.CopyAllAttributes(parent, elementContainer);
                            parent.parentNode.removeChild(parent);
                        }
                        newElements.push(elementContainer);
                        if(headerExpr.test(cmdValue))
                            this.RemoveWeightAndSizeStyle(wrapper, inlineElements);
                    }
                }
            }
            ASPx.RemoveElement(ASPx.GetElementByIdInDocument(doc, tempNodeId));
            selection.clientSelection.SelectExtendedBookmark(selectionBookmarks);
            return true;
        },
        FindInlineTextParent: function(element) {
            return ASPx.HtmlEditorClasses.Commands.Utils.FindParent(element, ASPx.HtmlEditorClasses.Commands.Utils.IsInlineTextElement);
        },
        FindAllTextNodes: function(element) {
            var nodes = [];
            for(var child = element.firstChild; child; child = child.nextSibling) {
                if(child.nodeType == 3)
                    nodes.push(child);
                else
                    nodes = nodes.concat(this.FindAllTextNodes(child));
            }
            return nodes;
        },
	    GetCorrectedValue: function(value) {
	        return ASPx.Browser.IE ? "<" + value + ">" : value;
	    },
        IsFormatBlockElement: function(element) {
	        var expr = /H[1-6]|DIV|ADDRESS|BODY/;
            return expr.test(element.nodeName);
	    },
	    GetValue: function(wrapper, selection, selectedElements) {
	        if(!wrapper.isInFocus)
                return this.GetDefaultValue(wrapper);
            selection = !selection ? wrapper.getSelection() : selection;
            var selectedElement = selectedElements && selectedElements.length == 1 ? selectedElements[0] : selection.GetSelectedElement();
            var getParentNode = function(node) { return node.parentNode; };
            var parent = ASPx.HtmlEditorClasses.Commands.Utils.FindElement(selectedElement, this.IsFormatBlockElement, getParentNode);
            var result = !parent ? "" : parent.nodeName.toLowerCase();
            return ASPx.Data.ArrayIndexOf(this.getDefaultFormatBlockValues(), result) < 0 ? this.GetDefaultValue(wrapper) : result;
	    },
        RemoveWeightAndSizeStyle: function(wrapper, elements) {
            var parent = elements[0].parentNode;
            if(parent.nodeName != "BODY") {
                parent.style["fontWeight"] = "normal";
                if(ASPx.Browser.NetscapeFamily) // force current style recalculating
                    var offsetHeight = parent.offsetHeight;
            }
            this.owner.getCommand(ASPxClientCommandConsts.BOLD_COMMAND).RemoveStyle(wrapper, elements, null, true);
            this.owner.getCommand(ASPxClientCommandConsts.FONTSIZE_COMMAND).RemoveStyle(wrapper, elements);
            if(parent.nodeName != "BODY") {
                parent.style["fontWeight"] = "";
                parent.style["fontSize"] = "";
            }
        },
	    GetDefaultValue: function(wrapper) {
            var defaultTags = ["p", "span"];
            for(var i = 0, tag; tag = defaultTags[i]; i++) {
                if(ASPx.Data.ArrayIndexOf(this.getDefaultFormatBlockValues(), tag) > -1)
                    return tag;
            }
            return "";
	    },
	    IsLocked: function(wrapper) {
	        if(wrapper.isInFocus)
                return ASPx.HtmlEditorClasses.Commands.Browser.Command.prototype.IsLocked.call(this, wrapper);
	        return false;
	    },
        setDefaultFormatBlockValues: function(values) {
            this.defaultFormatBlockValues = values;
        },
        getDefaultFormatBlockValues: function() {
            return this.defaultFormatBlockValues;
        }
    });
})();