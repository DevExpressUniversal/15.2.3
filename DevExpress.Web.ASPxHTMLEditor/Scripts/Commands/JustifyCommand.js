(function() {
    ASPx.HtmlEditorClasses.Commands.Browser.Justify = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Browser.WrappedCommand, {
        ExecuteCore: function(cmdValue, wrapper) {
            var doc = wrapper.getDocument();
            var selection = wrapper.getSelection();

            var tempNodeId = "tempSpanID";
            var elements = [];
            var isCollapsed = selection.IsCollapsed();
            if(!isCollapsed)
                elements = selection.GetElements(true);
            if(elements.length == 0 || isCollapsed)
                elements = [ ASPx.HtmlEditorClasses.Commands.Utils.InsertNewElement(doc, selection, tempNodeId) ];
            var firstElement = elements[0];

            var notStylizedElements;
            var isBrElement = function(el) { return el && el.nodeName == "BR"; };
            if(elements.length == 1 && firstElement.nodeName == "IMG") {
                if(isBrElement(firstElement.nextSibling))
                    firstElement.nextSibling.parentNode.removeChild(firstElement.nextSibling);
                notStylizedElements = elements;
            }

            var selectionBookmarks;
            if(isCollapsed) {
                if(firstElement.id == tempNodeId && !firstElement.previousSibling && !firstElement.nextSibling) {
                    var newElement = doc.createElement("DIV");
                    firstElement.parentNode.insertBefore(newElement, firstElement);
                    newElement.appendChild(firstElement);
                    newElement.appendChild(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 11 ? doc.createTextNode("\xA0") : doc.createElement("BR"));
                }
                selectionBookmarks = ASPx.HtmlEditorClasses.Commands.Utils.createBookmarkElements(doc, firstElement, firstElement, false);
            }
            else
                selectionBookmarks = ASPx.HtmlEditorClasses.Commands.Utils.createBookmarkElements(doc, firstElement, elements[elements.length - 1]);
            
            elements = this.replaceTableElements(elements);
            if(!notStylizedElements)
                notStylizedElements = this.GetNotStylizedElements(wrapper, elements);
            if(notStylizedElements.length > 0) {
                this.RemoveStyle(wrapper, notStylizedElements);
                this.StylizeElements(doc, notStylizedElements);
            }
            ASPx.RemoveElement(ASPx.GetElementByIdInDocument(doc, tempNodeId));
            wrapper.getSelection().clientSelection.SelectExtendedBookmark(selectionBookmarks);
            return true;
        },
        GetStateCore: function(wrapper, selection, selectedParent, selectedElements) {
            if(this.canCheckSelectedElementStyle(selection, selectedParent, selectedElements))
                return this.IsElementStylized(selectedParent);
            return this.GetStateInternal(wrapper, selection, selectedElements);
	    },
        GetNotStylizedElements: function(wrapper, elements) {
            var resultList = [];
            for(var i = 0, element; element = elements[i]; i++) {
                if(this.skipElement(element) || this.IsElementStylized(element))
                    continue;
                if(ASPx.HtmlEditorClasses.Commands.Utils.IsBlockElement(element))
                    resultList.push(element);
                else {
                    var inlineEl = this.FindInlineElementBeforeBlockParent(element);
                    var inlineElements = ASPx.HtmlEditorClasses.Commands.Utils.GetBetweenBlocksInlineElements(inlineEl);
                    var blockElement = inlineEl.parentNode;
                    if(inlineElements.length == blockElement.childNodes.length && blockElement.nodeName != "BODY")
                        resultList.push(blockElement);
                    else
                        resultList = resultList.concat(inlineElements);
                }
            }
            return resultList;
        },
        StylizeElements: function(doc, elements) {
            var wraper;
            var attr = this.GetStyleAttributeConstValue();
            for(var i = 0, element; element = elements[i]; i++) {
                if(ASPx.HtmlEditorClasses.Commands.Utils.IsBlockElement(element)) {
                    this.SetStyleAttribute(element, attr);
                    if(wraper)
                        wraper = null;
                } 
                else {
                    if(!wraper)
                        wraper = this.CreateStylizedWrapper(doc, false, false, "DIV");
                    if(!wraper.parentNode)    
                        element.parentNode.insertBefore(wraper, element);
                    wraper.appendChild(element);
                }
            }
        },
        RemoveStyleInHierarchy: function(wrapper, element) {
            if(element) {
                for(var child = element.firstChild; child; child = child.nextSibling)
                    this.RemoveStyle(wrapper, [child]);
            }
        },
        FindInlineElementBeforeBlockParent: function(element) {
            if(ASPx.HtmlEditorClasses.Commands.Utils.IsBlockElement(element.parentNode) || element.parentNode.nodeName == "BODY")
                return element;
            else
                return this.FindInlineElementBeforeBlockParent(element.parentNode);
        },
        GetStyleAttribute: function() {
            return "textAlign";
        },
        GetAttribute: function() {
            return "align";
        },
        GetStyleAttributeConstValue: function() {
            switch (this.GetCommandName()) {
                case ASPxClientCommandConsts.JUSTIFYCENTER_COMMAND:
                    return "center";
                case ASPxClientCommandConsts.JUSTIFYLEFT_COMMAND:
                    return "left";
                case ASPxClientCommandConsts.JUSTIFYRIGHT_COMMAND:
                    return "right";
                case ASPxClientCommandConsts.JUSTIFYFULL_COMMAND:
                    return "justify";
            }
        },
        canBeExecutedOnSelection: function(wrapper) {
            return true;
        }
    });
})();