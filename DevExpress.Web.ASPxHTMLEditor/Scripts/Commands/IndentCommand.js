(function() {
    ASPx.HtmlEditorClasses.Commands.Browser.Indent = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Browser.WrappedCommand, {
        Execute: function(cmdValue, wrapper) {
            var doc = wrapper.getDocument();
            var selection = wrapper.getSelection();
            var selectedElement = selection.GetSelectedElement();
            var selectedList = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetMainParentList(selectedElement);
            if (selectedList)
                this.IndentList(selectedList, wrapper, selectedElement);
            else {
                var tempNodeId = "tempSpanID";
                var selectedElements = [];
                var bookmark = selection.clientSelection.GetExtendedBookmark();
                if(!selection.IsCollapsed())
                    selectedElements = selection.GetElements(true);
                if(selectedElements.length > 0)
                    this.SpliceBookmarks(selectedElements, bookmark);
                else
                    selectedElements.push(ASPx.HtmlEditorClasses.Commands.Utils.InsertNewElement(doc, selection, tempNodeId));
                selectedElements = this.GetCommandName() == ASPxClientCommandConsts.INDENT_COMMAND ? this.prepareElementsBeforeIndentCommand(selectedElements) : this.prepareElementsBeforeOutdentCommand(selectedElements);
                for(var i = 0, element; element = selectedElements[i]; i++)
                    this.StylizeElements(element);
                ASPx.RemoveElement(ASPx.GetElementByIdInDocument(doc, tempNodeId));
                wrapper.getSelection().clientSelection.SelectExtendedBookmark(bookmark);
            }
            return true;
        },
        GetState: function(wrapper, selection, selectedElements) {
            return false;
        },
        IsLocked: function(wrapper) {
            if(!wrapper.isInFocus)
                return false;
            var selection = wrapper.getSelection();
            var selectedList = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParentList(selection.GetSelectedElement());
            return selectedList ? false : ASPx.HtmlEditorClasses.Commands.Browser.Command.prototype.TryGetIsLocked.call(this, wrapper);
        },
        IndentList: function(listNode, wrapper, selectedElement) {
            if(ASPx.Browser.NetscapeFamily)
                selectedElement.normalize();
            var bookmark = wrapper.getSelection().clientSelection.GetExtendedBookmark();
            var selection = wrapper.getSelection();
            var startContainer, endContainer;
            if(!selection.IsCollapsed()) {
                var selectedElements = selection.GetElements(true);
                this.SpliceBookmarks(selectedElements, bookmark);
                if(selectedElements.length > 0) {
                    startContainer = selectedElements[0];
                    endContainer = selectedElements[selectedElements.length - 1];
                }
            }
            if(!startContainer && !endContainer) {
                startContainer = selection.GetSelectedElement();
                endContainer = startContainer;
            }
            var doc = wrapper.getDocument();
            var listItemArray = this.GetListItemArray(listNode, startContainer, endContainer, bookmark);
            if(listItemArray.length > 0) {
                var parentListItem = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParentListItem(startContainer);
                if(!parentListItem.previousSibling && (this.GetCommandName() == ASPxClientCommandConsts.INDENT_COMMAND || this.IsElementStylized(parentListItem.parentNode)))
                    this.StylizeElements(parentListItem.parentNode);
                else
                    this.ReplaceList(doc, listItemArray, listNode, bookmark);
                if(ASPx.Browser.IE && ASPx.Browser.MajorVersion > 8 && bookmark.endMarkerID) { // Q421012
                    var endBm = doc.getElementById(bookmark.endMarkerID);
                    if(endBm.parentNode.childNodes.length == 2 && endBm.parentNode.firstChild.id == bookmark.startMarkerID) {
                        var fakeElement = doc.createTextNode(" ");
                        endBm.parentNode.insertBefore(fakeElement, endBm.parentNode.firstChild);
                    }
                }
                selection.clientSelection.SelectExtendedBookmark(bookmark);
                if(fakeElement) // Q421012
                    fakeElement.parentNode.removeChild(fakeElement);
            }
            wrapper.focus();
        },
        GetIndent: function() {
            var indent = 0;
            switch (this.GetCommandName()) {
                case ASPxClientCommandConsts.INDENT_COMMAND:
                    indent = 1; break;
                case ASPxClientCommandConsts.OUTDENT_COMMAND:
                    indent = -1; break;
            }
            return indent;
        },
        GetListItemArray: function(listNode, startItemNode, endItemNode, bookmark) {
            var startNode = ASPx.HtmlEditorClasses.Commands.Browser.Indent.EnsureListItemNode(startItemNode);
            var endNode = ASPx.HtmlEditorClasses.Commands.Browser.Indent.EnsureListItemNode(endItemNode);
            var listItemArray = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetListItemArray(listNode, 0, null, null, bookmark);
            return ASPx.HtmlEditorClasses.Commands.Browser.Indent.IndentListItemsInItemArray(listItemArray, startNode, endNode, this.GetIndent());
        },
        SpliceBookmarks: function(elements, bookmark) {
            for(var i = 0, element; element = elements[i]; i++) {
                if(element.id ==  bookmark.startMarkerID || element.id ==  bookmark.endMarkerID) {
                    elements.splice(i, 1);
                    i--;
                }
            }
        },
        ReplaceList: function(doc, listItemArray, listNode, bookmark) {
            var structureArray = [];
            var tempArray = [];
            var listNode = listItemArray[0].node.parentNode;
            for(var i = 0, item; item = listItemArray[i]; i++) {
                if(item.level == -1) {
                    if(tempArray.length > 0) {
                        structureArray.push(ASPx.HtmlEditorClasses.Commands.Browser.Indent.GenerateListByItemArray(tempArray, listNode));
                        tempArray = [];
                    }
                    this.WrapInBlockElements(doc, item.node.childNodes);
                    for(var j = 0, child; child = item.node.childNodes[j]; j++) {
                        if(child.nodeName != "UL" && child.nodeName != "OL")
                            structureArray.push(child);
                    }
                }
                else
                    tempArray.push(item);
            }
            if(tempArray.length > 0)
                structureArray.push(ASPx.HtmlEditorClasses.Commands.Browser.Indent.GenerateListByItemArray(tempArray, listNode));
            var lastElement = structureArray.splice(structureArray.length - 1, 1)[0];
            listNode.parentNode.replaceChild(lastElement, listNode);
            for(var i = 0, item; item = structureArray[i]; i++)
                lastElement.parentNode.insertBefore(item, lastElement);
            if(this.GetCommandName() == ASPxClientCommandConsts.INDENT_COMMAND) {
                var startBm = ASPx.GetElementByIdInDocument(doc, bookmark.startMarkerID);
                var parentListItem = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParentListItem(startBm);
                if(!parentListItem.previousSibling)
                    this.SetStyleAttribute(parentListItem.parentNode, "");
            }
        },
        GetStyleIndentValue: function(styleAttrValue, value) {
            return (styleAttrValue + value) + "px";
        },
        WrapInBlockElements: function(doc, elements) {
            var wrapper;
            var currentPos = 0;
            while (elements.length > currentPos) {
                child = elements[currentPos];
                if((child.nodeType == 3 || (ASPx.GetCurrentStyle(child)["display"] == "inline") && child.nodeName != "UL" && child.nodeName != "OL")) {
                    if(!wrapper) {
                        wrapper = doc.createElement("DIV");
                        child.parentNode.insertBefore(wrapper, child);
                        currentPos++;
                    }
                    wrapper.appendChild(child);
                } 
                else {
                    wrapper = null;
                    currentPos++;
                }
            }
        },
        StylizeElements: function(element) {
            var currentStyle = ASPx.GetCurrentStyle(element);
            var styleAttrValue = this.GetCurrentStyleAttributeValue(element, currentStyle);
            var indentStep = this.GetIndentStep();
            if(this.GetCommandName() == ASPxClientCommandConsts.OUTDENT_COMMAND) {
                var value = styleAttrValue + this.getCurrentStyleAttributeValueByAttributeName(currentStyle, "textIndent");
                indentStep = value < 40 ? value : indentStep;
            }
            var newStyleAttrValue = this.GetIndent() > 0 ? this.GetStyleIndentValue(styleAttrValue, indentStep) : this.GetStyleIndentValue(styleAttrValue, indentStep * -1);
            if(parseInt(newStyleAttrValue) < 0)
                this.SetStyleAttribute(element, "");
            else
                this.SetStyleAttribute(element, newStyleAttrValue);
        },
        GetIndentStep: function() {
            return 40;
        },
        IsElementStylized: function(element) {
            var currentStyle = ASPx.GetCurrentStyle(element);
            return (this.GetCurrentStyleAttributeValue(element, currentStyle) + this.getCurrentStyleAttributeValueByAttributeName(currentStyle, "textIndent")) > 0;
        },
        GetCurrentStyleAttributeValue: function(element, currentStyle) {
            return this.getCurrentStyleAttributeValueByAttributeName(currentStyle, this.GetStyleAttribute());
        },
        SkipStateOnFirstLoad: function() {
            return true;
        },
        GetStyleAttribute: function() {
            return "marginLeft";
        },
        canBeExecutedOnSelection: function(wrapper, curSelection) {
            return true;
        },
        prepareElementsBeforeIndentCommand: function(elements) {
            var result = [];
            var regExp = /^(body|tr|td)/i;
            for(var i = 0, element; element = elements[i]; i++) {
                var isBlockElement = ASPx.HtmlEditorClasses.Commands.Utils.IsBlockElement(element);
                if(!isBlockElement) {
                    var inlineEl = ASPx.HtmlEditorClasses.Utils.findInlineElementBeforeBlockParent(element);
                    var blockParentElement = inlineEl.parentNode;
                    if(result.length > 0 && result[result.length - 1] == blockParentElement)
                        continue;
                    var inlineElements = ASPx.HtmlEditorClasses.Commands.Utils.GetBetweenBlocksInlineElements(inlineEl);
                    if(regExp.test(blockParentElement.nodeName) || inlineElements.length < blockParentElement.childNodes.length) {
                        var inlineElements = ASPx.HtmlEditorClasses.Commands.Utils.GetBetweenBlocksInlineElements(inlineEl);
                        var divElement = element.ownerDocument.createElement("DIV");
                        blockParentElement.insertBefore(divElement, inlineElements[0]);
                        for(var j = 0, child; child = inlineElements[j]; j++)
                            divElement.appendChild(child);
                        result.push(divElement);
                    }
                    else
                        result.push(blockParentElement);
                }
                else {
                    if(!regExp.test(element.nodeName))
                        result.push(element);
                    else {
                        var predicate = function(el) { return !regExp.test(el.nodeName); };
                        var childNodes = ASPx.HtmlEditorClasses.Utils.getChildsByPredicate(element, predicate, true);
                        for(var j = 0, child; child = childNodes[j]; j++) {
                            if(ASPx.HtmlEditorClasses.Commands.Utils.IsBlockElement(child))
                                result.push(child);
                            else {
                                var inlineElements = ASPx.HtmlEditorClasses.Commands.Utils.GetBetweenBlocksInlineElements(child);
                                if(result.length > 0 && result[result.length - 1] == child.parentNode)
                                    continue;
                                var divElement = element.ownerDocument.createElement("DIV");
                                element.insertBefore(divElement, child);
                                for(var k = 0, el; el = inlineElements[k]; k++)
                                    divElement.appendChild(el);
                                result.push(divElement);
                            }
                        }
                    }
                }
            }
            return result;
        },
        prepareElementsBeforeOutdentCommand: function(elements) {
            var result = [];
            for(var i = 0, element; element = elements[i]; i++) {
                var isBlockElement = ASPx.HtmlEditorClasses.Commands.Utils.IsBlockElement(element);
                var parent = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParent(element, function(el) { return el.nodeName == "BODY" || ASPx.HtmlEditorClasses.Commands.Utils.IsBlockElement(el); });
                var stylizedParent = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParent(parent,  function(el) { return el.nodeName == "BODY" || this.IsElementStylized(el); }.aspxBind(this));
                if(stylizedParent.nodeName == "BODY" && !isBlockElement || result.length > 0 && result[result.length - 1] == stylizedParent)
                    continue;
                if(stylizedParent && stylizedParent.nodeName != "BODY") {
                    if(parent == stylizedParent)
                        result.push(stylizedParent);
                    else
                        result.push(ASPx.HtmlEditorClasses.Utils.splitParentElement(stylizedParent, parent.firstChild, parent.lastChild));
                }
                else {
                    var predicate = function(el) { return ASPx.HtmlEditorClasses.Commands.Utils.IsBlockElement(el) && this.IsElementStylized(el); }.aspxBind(this);
                    var childNodes = ASPx.HtmlEditorClasses.Utils.getChildsByPredicate(parent, predicate);
                    if(childNodes.length > 0)
                        result = result.concat(childNodes);
                }
            }
            return result;
        },
        getCurrentStyleAttributeValueByAttributeName: function(currentStyle, attrName) {
            var result = currentStyle[attrName];
            if(parseFloat(result) != NaN && !/px/i.test(result))
                result = this.convertToPixel(result);
            result = parseInt(result);
            return result ? result : 0;
        }
    });

    // itemArray = { {level_1, node_1, parentItemIndex_1 }, ... , {item_N, node_N, parentItemIndex_N } }
    ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetListItemArray = function(listNode, currentLevel, itemArray, parentItemIndex, bookmark) {
        if (!itemArray)
            itemArray = [ ];
        
        if (!ASPx.IsExists(parentItemIndex))
            parentItemIndex = -1;
               
        var curListItem = ASPx.GetNodeByTagName(listNode, "LI" ,0);    
        while(curListItem) {
            if(!bookmark || (curListItem.id !=  bookmark.startMarkerID && curListItem.id !=  bookmark.endMarkerID))
                itemArray.push({ 'level': currentLevel, 'node' : curListItem, 'parentItemIndex' : parentItemIndex });
        
            var childListArray = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetChildListArray(curListItem);
            for(var i = 0; listNode = childListArray[i]; i++)
                itemArray = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetListItemArray(listNode, currentLevel + 1, itemArray, itemArray.length - 1, bookmark);
            curListItem = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetNextListItem(curListItem);
        }
        return itemArray;
    };
    // itemArray = { {level_1, node_1, parentItemIndex_1 }, ... , {item_N, node_N, parentItemIndex_N } }
    // indent = {-1, 1}
    ASPx.HtmlEditorClasses.Commands.Browser.Indent.IndentListItemsInItemArray = function(itemArray, startListItem, endListItem, indent) {
        var startItemIndex = -1;
        var endItemIndex = -1;
        // get index
        for (var i = 0; i < itemArray.length; i++) {
            if (itemArray[i].node == startListItem)
                startItemIndex = i;
            if (itemArray[i].node == endListItem)
                endItemIndex = i;
            if (startItemIndex != -1 && endItemIndex != -1)
                break;
        }
        if (startItemIndex == -1 || endItemIndex == -1)
            return null;
    
        for (var i = startItemIndex; i <= endItemIndex; i++)
            itemArray[i].level += indent;
     
        if (indent < 0) {
            for (var i = 0; i < itemArray.length; i++) {
                if (itemArray[i].parentItemIndex != -1) {
                    var parentLevel = itemArray[itemArray[i].parentItemIndex].level;
                    if (itemArray[i].level - parentLevel > 1)
                        itemArray[i].level += indent;
                }
            }
        }
        return itemArray;
    };
    // output = {listNode, startListItemNode, endtListItemNode}
    ASPx.HtmlEditorClasses.Commands.Browser.Indent.GenerateListByItemArray = function(itemArray, listNode, startAttrValue) {
        if (itemArray.length <= 0)
            return null;
        
        var doc = itemArray[0].node.document || itemArray[0].node.ownerDocument;
    
        var listNodeByLevel = [];
        var listItemNodeArray = [];
        var curLevel = 0;
        
        for (var i = 0; i < itemArray.length; i++) {
            var curItem = itemArray[i];
                    
            if (curLevel > curItem.level) {
                for (var j = curItem.level + 1; j <= curLevel; j++)
                    listNodeByLevel[j] = null;
            }
            
            // create new list
            if (!listNodeByLevel[curItem.level]) {
                var oldListNode = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParentList(curItem.node);
                var ownerDocument = doc.ownerDocument ? doc.ownerDocument : doc;
                var newListNode = curItem.parentListType ? ownerDocument.createElement(curItem.parentListType) : oldListNode.cloneNode(false);
                if(curItem.listStyleTypeValue)
                    newListNode.style.listStyleType = curItem.listStyleTypeValue;
            
                listNodeByLevel[curItem.level] = newListNode;
            
                if (curItem.level - 1 >= 0)
                    listItemNodeArray[curItem.level - 1].appendChild(newListNode);
            }
        
            var clonedListItem = ASPx.HtmlEditorClasses.Commands.Browser.Indent.CloneListItem(curItem.node, doc);
        
            listItemNodeArray[curItem.level] = clonedListItem;
            listNodeByLevel[curItem.level].appendChild(clonedListItem);
                
            curLevel = curItem.level;
        }
        var newListNode = listNodeByLevel[0];
        if(listNode)
            ASPx.Attr.CopyAllAttributes(listNode, newListNode);
        if(startAttrValue)
            ASPx.Attr.SetAttribute(newListNode, "start", startAttrValue);
        return newListNode;
    };
    ASPx.HtmlEditorClasses.Commands.Browser.Indent.EnsureListItemNode = function(node) {
	    while (node) {
	        var tagName = "";
	        var parentNode = node.parentNode;
	        if (parentNode && parentNode.nodeType == 1)
                tagName = parentNode.tagName.toUpperCase();
	        if (tagName == "OL" || tagName == "UL")
	            break;
		    node = parentNode;
	    }
        return node;
    };
    ASPx.HtmlEditorClasses.Commands.Browser.Indent.CloneListItem = function(item, doc) {
	    var newItem = item.cloneNode(true);
	    newItem.innerHTML = "";
	    for (var i = 0 ; i < item.childNodes.length; i++) {
	        var tagName = item.childNodes[i].tagName;
            tagName = tagName ? tagName.toUpperCase() : "";
		    if ((tagName != "UL") && (tagName != "OL"))
                newItem.appendChild(item.childNodes[i].cloneNode(true));
	    }
	    return newItem;
    };
    ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetChildListArray = function(elem) {
	    var result = [];
        for (var i = 0 ; i < elem.childNodes.length; i++) {
	        var tagName = elem.childNodes[i].tagName;
            tagName = tagName ? tagName.toUpperCase() : "";	
		    if ((tagName == "UL") || (tagName == "OL"))
		        result.push(elem.childNodes[i]);
	    }
	    return result;
    };
    ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetNextListItem = function(listItemNode) {
        var nextListItemNode = listItemNode.nextSibling;
        while (nextListItemNode != null && nextListItemNode.nodeType != 1)
            nextListItemNode = nextListItemNode.nextSibling;
        return nextListItemNode;
    };
    ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetMainParentList = function(elem) {
	    var lastListNode = null;
	    while (elem) {
	        if (elem.nodeType == 1) {
	            var tagName = elem.tagName.toUpperCase();
	            if (tagName == "UL" || tagName == "OL")
	                lastListNode = elem;
	        }
		    elem = elem.parentNode;
	    }
	    return lastListNode;
    };
    ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParentList = function(elem) {
        return ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParent(elem,
            function(elem) {
                return elem.tagName == "UL" || elem.tagName == "OL";
            }.aspxBind(this)
        );
    };
    ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParentListItem = function(elem) {
        return ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParent(elem,
            function(elem) {
                return elem.tagName == "LI";
            }.aspxBind(this)
        );
    };
    ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParentBlockquote = function(elem) {
        return ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParent(elem,
            function(elem) {
                return elem.tagName == "BLOCKQUOTE";
            }.aspxBind(this)
        );
    };
    ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParent = function(elem, compare) {
        while(elem) {
            if(compare(elem))
                return elem;
            elem = elem.parentNode;
        }
        return null;
    };
    ASPx.HtmlEditorClasses.Commands.Browser.Indent.SplitParentBlockquote = function(selectedElements) {
        for(var i = 0, element; element = selectedElements[i]; i++) {
            var parentElement = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParentBlockquote(element);
            if(parentElement) {
                var prevParentElement = i > 0 ? ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParentBlockquote(selectedElements[i - 1]) : null;
                var foundBlockOrBrElement = ASPx.HtmlEditorClasses.Commands.Browser.Indent.FindBlockOrBrElement(parentElement.childNodes);
                if(foundBlockOrBrElement && parentElement.childNodes.length > 1 && (!prevParentElement || prevParentElement != parentElement)) {
                    var blockElement = null;
                    for(var child = parentElement.firstChild; child; child = parentElement.firstChild) {
                        if(!blockElement || (child.nodeType == 1 && ASPx.GetCurrentStyle(child)["display"] == "block")) {
                            blockElement = parentElement.cloneNode(false);
                            parentElement.parentNode.insertBefore(blockElement, parentElement);
                        }
                        blockElement.appendChild(child);
                        if(child.nodeName == "BR")
                            blockElement = null;
                    }
                    parentElement.parentNode.removeChild(parentElement);
                }
            }
        }
    };
    ASPx.HtmlEditorClasses.Commands.Browser.Indent.FindBlockOrBrElement = function(elements) {
        for(var i = 0, elem; elem = elements[i]; i++) { 
            if(elem.nodeType == 1 && (elem.nodeName == "BR" || ASPx.GetCurrentStyle(elem)["display"] == "block"))
                return true;
        }
        return false;
    };
})();