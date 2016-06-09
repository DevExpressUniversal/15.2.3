(function() {
    ASPx.HtmlEditorClasses.Commands.Browser.InsertList = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Browser.Command, {
        GetState: function(wrapper, selection, selectedElements) {
            if(!wrapper.isInFocus)
                return false;
            selection = !selection ? wrapper.getSelection() : selection;
            var curSelection = selection.clientSelection;
            var tagName = "";
            var parent = curSelection.GetParentElement();
            switch (this.GetCommandName()) {
                case ASPxClientCommandConsts.INSERTORDEREDLIST_COMMAND:
                    tagName = "OL";break;
                case ASPxClientCommandConsts.INSERTUNORDEREDLIST_COMMAND:
                    tagName = "UL";break;
            }
            var parentList = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParentList(parent);
            return tagName && parentList && parentList.tagName == tagName;
        },
        Execute: function(cmdValue, wrapper) {
            var tagName = (this.GetCommandName() == ASPxClientCommandConsts.INSERTORDEREDLIST_COMMAND) ? "OL" : "UL",
                oppositeTagName = (tagName == "OL") ? "UL" : "OL";
            if(wrapper.tagIsFiltered(tagName) || wrapper.tagIsFiltered("li"))
                return false;

            var doc = wrapper.getDocument(),
                selection = wrapper.getSelection(),
                selectedElement = selection.GetSelectedElement(),
                clientSelection = selection.clientSelection,
                bookmark = clientSelection.GetExtendedBookmark(),
                result = false,
                listItem,
                startListItem,
                endListItem;
            if(ASPx.Browser.IE) {
                var startNode = ASPx.GetElementByIdInDocument(doc, bookmark.startMarkerID);
                if(!startNode.nextSibling)
                    this.InsertBeforeNextTextNode(doc, startNode);
            }
            var parentList = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParentList(selectedElement);
            if(parentList) {
                if(selection.IsCollapsed()) {
                    if(parentList.nodeName == tagName)
                        this.UnwrapSelectedListItems(wrapper, bookmark, selectedElement);
                    else {
                        listItem = ASPx.GetParentByTagName(selectedElement, "LI");
                        this.ChangeListType([listItem], tagName, doc);
                    }
                }
                else {
                    var needUnwrapListItems = true;
                    var listItemsList = [];
                    var elements = selection.GetElements();
                    var endNode = ASPx.GetElementByIdInDocument(doc, bookmark.endMarkerID);
                    if(!endNode.nextSibling && endNode.previousSibling.nodeName == "LI")
                        endNode.previousSibling.appendChild(endNode);
                    for(var el, i = 0; el = elements[i]; i++) {
                        listItem = ASPx.GetParentByTagName(el, "LI");
                        if(listItem) {
                            var childLists = ASPx.GetChildNodesByTagName(listItem, oppositeTagName);
                            for(var cl, j = 0; cl = childLists[j]; j++) {
                                ASPx.ReplaceTagName(cl, tagName, false);
                                needUnwrapListItems = false;
                            }
                        }
                        if(listItem && listItem.parentNode.tagName == oppositeTagName) {
                            if(listItemsList.length == 0 || listItemsList[listItemsList.length - 1] != listItem) {
                                listItemsList.push(listItem);
                                continue;
                            }
                            else if(listItemsList[listItemsList.length - 1].parentNode == listItem.parentNode)
                                continue;
                        }
                        needUnwrapListItems = needUnwrapListItems && listItemsList.length == 0;
                        this.ChangeListType(listItemsList, tagName, doc);
                        listItemsList = [];
                    }
                    if(needUnwrapListItems && listItemsList.length == 0)
                        this.UnwrapSelectedListItems(wrapper, bookmark, selectedElement);
                    else
                        this.ChangeListType(listItemsList, tagName, doc);
                }
                result = true;
            }
            else {
                selectedElement = this.GetBlockSelectedElement(selectedElement);
                var lineInfos = this.GetLineInfos(selectedElement);
                var selectedLineInfos = this.GetSelectedLineInfos(selectedElement, lineInfos, bookmark);
                result = this.CreateBulletList(doc, tagName, lineInfos, selectedLineInfos.startLineIndex, selectedLineInfos.endLineIndex);
                startListItem = ASPx.GetElementByIdInDocument(doc, bookmark.startMarkerID).parentNode;
                endListItem =ASPx.GetElementByIdInDocument(doc, bookmark.endMarkerID).parentNode;
                if(startListItem == endListItem) {
                    if(ASPx.Browser.IE && ASPx.Browser.Version < 11) {
                        if(startListItem.innerText == " ") {
                            for(var i = 0, childNode; childNode = startListItem.childNodes[i]; i++) {
                                if(childNode.nodeType == 3)
                                    childNode.nodeValue = "";
                            }
                        }
                    } 
                    else if(startListItem.childNodes.length == 2)
                        startListItem.appendChild(doc.createElement("BR"));
                }
            }
            this.mergerParentLists(doc, bookmark, tagName);
            ASPx.HtmlEditorClasses.Commands.Browser.InsertList.UpdateListNumbering(wrapper.getBody());
            wrapper.getSelection().clientSelection.SelectExtendedBookmark(bookmark);
            return result;
        },
        mergerParentLists: function(doc, bookmark, tagName) {
            var listItem = ASPx.GetParentByTagName(ASPx.GetElementByIdInDocument(doc, bookmark.startMarkerID), "LI");
            if(listItem && !listItem.previousSibling && listItem.parentNode.previousSibling && listItem.parentNode.previousSibling.nodeName == tagName) {
                var prevList = listItem.parentNode.previousSibling;
                var parentList = listItem.parentNode;
                for(var i = 0, child; child = parentList.childNodes[i]; i++)
                    prevList.appendChild(child.cloneNode(true));
                parentList.parentNode.removeChild(parentList);
            }
            listItem = ASPx.GetParentByTagName(ASPx.GetElementByIdInDocument(doc, bookmark.endMarkerID), "LI");
            if(listItem && !listItem.nextSibling && listItem.parentNode.nextSibling && listItem.parentNode.nextSibling.nodeName == tagName) {
                var nextList = listItem.parentNode.nextSibling;
                var parentList = listItem.parentNode;
                var firstChild = nextList.firstChild;
                for(var i = 0, child; child = parentList.childNodes[i]; i++)
                    nextList.insertBefore(child.cloneNode(true), firstChild);
                parentList.parentNode.removeChild(parentList);
            }
        },
        InsertBeforeNextTextNode: function(doc, element) {
            var getNextSibling = function(elem) { return elem.nextSibling; }.aspxBind(this);
            var parentNode = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParent(element, getNextSibling);
            if(parentNode) {
                var nextSibling = parentNode.nextSibling;
                if(nextSibling.nodeType == 1) {
                    var textNode = ASPx.GetTextNode(nextSibling);
                    var node = !textNode ? nextSibling : textNode.parentNode;
                    node.appendChild(element);
                    element.parentNode.insertBefore(element, node.firstChild);
                }
                else
                    nextSibling.parentNode.insertBefore(element, nextSibling);
            }
        },
        RemoveBookmark: function(el) {
            if(el.parentNode && el.parentNode.childNodes.length == 1 && (el.parentNode.tagName == "UL" || el.parentNode.tagName == "OL"))
                ASPx.RemoveElement(el.parentNode);
            else
                ASPx.RemoveElement(el);
        },
        ChangeListType: function(listItems, tagName, doc) {
            if(listItems.length == 0)
                return;
            var curList = listItems[0].parentNode;
            var newList = doc.createElement(tagName);
            curList.parentNode.insertBefore(newList, curList);
            if(listItems[0].previousSibling && listItems[0].previousSibling.tagName == "LI") {
                var newPrevList = curList.cloneNode(false);
                var curNode = curList.firstChild;
                do {
                    var nextNode = curNode.nextSibling;
                    newPrevList.appendChild(curNode);
                    curNode = nextNode;
                } while(curNode && nextNode != listItems[0]);
                newList.parentNode.insertBefore(newPrevList, newList);
            }
            for(var listItem, i = 0; listItem = listItems[i]; i++)
                newList.appendChild(listItem);
            if(curList.childNodes.length == 0)
                ASPx.RemoveElement(curList);
        },
        UnwrapSelectedListItems: function(iframeWrapper, bookmark, selectedElement) {
            var doc = iframeWrapper.getDocument();
            var wrapperTagName = iframeWrapper.settings.enterMode != ASPx.HtmlEditorClasses.EnterMode.BR ? "P" : "DIV";
            var selectedList = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetMainParentList(selectedElement);
            var listItemArray = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetListItemArray(selectedList, 0, null, null, bookmark);
        
            var startNode = ASPx.GetElementByIdInDocument(doc, bookmark.startMarkerID);
            var endNode =ASPx.GetElementByIdInDocument(doc, bookmark.endMarkerID);
            var startListItem = ASPx.GetParentByTagName(startNode, "LI");
            var endListItem = ASPx.GetParentByTagName(endNode, "LI");

            if(!startListItem && !endListItem) {
                selectedElement.appendChild(endNode);
                selectedElement.appendChild(startNode);
                startListItem = selectedElement;
                endListItem = selectedElement;
            }
            var tempArray = [];
            var structureArray = [];

            var index = 0;
            for(var listItem; listItem = listItemArray[index]; index++) {
                if(startListItem === listItem.node)
                    break;
                tempArray.push(listItem);
            }
            if(tempArray.length > 0)
                structureArray.push(ASPx.HtmlEditorClasses.Commands.Browser.Indent.GenerateListByItemArray(tempArray));
            var wrapper;
            for(var listItem; listItem = listItemArray[index]; index++) {
                wrapper = doc.createElement(wrapperTagName);
                var childNodes = listItem.node.childNodes;
                for(var child, i = 0; child = childNodes[i]; i++) {
                    if(child.nodeName != "OL" && child.nodeName != "UL") {
                        if(wrapperTagName == "P" && ASPx.HtmlEditorClasses.Commands.Utils.IsBlockElement(child)) {
                            if(wrapper.childNodes.length > 0) {
                                structureArray.push(wrapper);
                                wrapper = doc.createElement(wrapperTagName);
                            }
                            structureArray.push(child);
                        }
                        else if(child.nodeType != 3 || child.nodeValue)
                            wrapper.appendChild(child.cloneNode(true));
                    }
                }
                if(wrapper.childNodes.length > 0)
                    structureArray.push(wrapper);
                if(endListItem === listItem.node)
                    break;
            }
            index++;
            tempArray = [];
            for(var listItem; listItem = listItemArray[index]; index++)
                tempArray.push(listItem);
            if(tempArray.length > 0) {
                this.UpdateListItemsLevels(tempArray);
                structureArray.push(ASPx.HtmlEditorClasses.Commands.Browser.Indent.GenerateListByItemArray(tempArray));
            }
            for(var item, i = 0; item = structureArray[i]; i++) {
                if(iframeWrapper.settings.enterMode != ASPx.HtmlEditorClasses.EnterMode.P && item.nodeName == "DIV") {
                    for(var j = 0, child; child = item.childNodes[j]; j++)
                        selectedList.parentNode.insertBefore(child.cloneNode(true), selectedList);
                    if(item.lastChild.nodeName != "BR")
                        selectedList.parentNode.insertBefore(doc.createElement("BR"), selectedList);
                }
                else
                    selectedList.parentNode.insertBefore(item, selectedList);
            }
            ASPx.RemoveElement(selectedList);
            startNode = ASPx.GetElementByIdInDocument(doc, bookmark.startMarkerID);
            endNode =ASPx.GetElementByIdInDocument(doc, bookmark.endMarkerID);
            if(endNode.parentNode == wrapper && startNode.parentNode == wrapper && wrapper.childNodes.length == 2) {
                if(ASPx.Browser.IE && ASPx.Browser.Version < 11) {
                    ASPx.InsertElementAfter(endNode, wrapper);
                    ASPx.InsertElementAfter(startNode, endNode);
                    wrapper.innerHTML = "&nbsp;";
                }
                else
                    wrapper.appendChild(doc.createElement("BR"));
            }
        },
        UpdateListItemsLevels: function(listItemArray) {
            var firstItem = listItemArray[0];
            var firstItemLevelValue = firstItem.level;
            if(firstItemLevelValue > 0) {
                firstItem.level = 0;
                firstItem.parentItemIndex = -1;
                for(var listItem, i = 1; listItem = listItemArray[i]; i++) {
                    if(listItem.level == 0)
                        break;
                    var newLevelValue = listItem.level - firstItemLevelValue;
                    listItem.level = newLevelValue;
                    listItem.parentItemIndex = newLevelValue == 0 ? -1 : newLevelValue;
                }
            }
        },

        CreateBulletList: function(doc, tagName, lineInfos, startLineIndex, endLineIndex) {
            if(startLineIndex == -1 || endLineIndex == -1)
                return false;

            var commonBlockParent;
            for(var i = startLineIndex, line = lineInfos[i]; i <= endLineIndex, line; i++, line = lineInfos[i]) {
                var blockEl = this.GetBlockSelectedElement(line[0]);
                if(!commonBlockParent)
                    commonBlockParent = blockEl;
                if(blockEl != commonBlockParent || blockEl.nodeName != "DIV") {
                    commonBlockParent = null;
                    break;
                }
            }

            var lineIndex = startLineIndex;
            var lineCount = lineInfos.length;
            // TODO reduce it
            while(lineIndex <= endLineIndex) {
                var line = lineInfos[lineIndex];
                var firstLineNode = line[0];
                var lastLineNode = line[line.length - 1];

                var firstLineElement = this.GetLineSelectedElement(firstLineNode);
                var lastLineElement = this.GetLineSelectedElement(lastLineNode);

                var firstBlockNode = this.GetBlockSelectedElement(firstLineNode);

                var hasBlockSibling = ASPx.GetChildNodes(firstBlockNode, function(element) {
                    return ASPx.HtmlEditorClasses.Commands.Enter.IsBlockNode(element);
                }).length > 0;

                var parent = null;
                var replacedElement = null;

                //var isParentBlockLimit = this.IsBlockLimitElement(this.GetBlockSelectedElement(firstLineNode.parentNode));
                var isParentBlockLimit = this.IsBlockLimitElement(firstBlockNode);
                var isMoveOutTheBlock = commonBlockParent ? false : !hasBlockSibling && !isParentBlockLimit;

                // move out the block unselected lines
                if(lineIndex == startLineIndex) {
                    var isList = false;
                    var sBlock = firstBlockNode;
                    if(sBlock.tagName == "LI") {
                        sBlock = sBlock.parentNode;
                        isList = true;
                        isMoveOutTheBlock = true;
                    }

                    var target = sBlock.firstChild;
                    for(var i = 0; i < lineIndex; i++) {
                        var uBlock = this.GetBlockSelectedElement(lineInfos[i][0]);
                        if(uBlock.tagName == "LI")
                            uBlock = uBlock.parentNode;

                        if(uBlock == sBlock && uBlock.tagName != tagName) {
                            var tFirst = lineInfos[i];
                            var tLast = lineInfos[lineIndex - 1];

                            var tFirstLineNode = tFirst[0];
                            var tLastLineNode = tLast[tLast.length - 1];

                            var textNodes = ASPx.HtmlEditorClasses.Commands.Utils.SplitNode(uBlock, [tFirstLineNode, tLastLineNode]);
                            if(isMoveOutTheBlock)
                                sBlock.parentNode.insertBefore(textNodes[1].cloneNode(true), sBlock);
                            else {
                                var childs = textNodes[1].childNodes;
                                for(var j = 0; j < childs.length; j++)
                                    sBlock.insertBefore(childs[j].cloneNode(true), target);
                            }

                            tLastLineNodeParent = tLastLineNode.parentNode;
                            this.RemoveLineBreakElement(uBlock, tFirstLineNode, tLastLineNode);
                            this.RemoveLineElement(uBlock, tFirstLineNode, tLastLineNode);

                            var tLastLineNodeParent = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParent(tLastLineNodeParent, function(el) { return el.nodeName == "BODY" || el.nextElementSibling || el.nextElementSibling; });
                            if(tLastLineNodeParent.nodeName != "BODY" && !/br/i.test(tLastLineNodeParent.innerHTML) && !ASPx.GetInnerText(tLastLineNodeParent))
                                ASPx.RemoveElement(tLastLineNodeParent);

                            if(isList) {
                                while(ASPx.GetInnerText(uBlock.firstChild) == "")
                                    uBlock.removeChild(uBlock.firstChild);
                            }
                            break;
                        }
                        else if(target)
                            target = target.nextSibling;
                    }
                }

                if(isParentBlockLimit && !this.IsAcceptableElement(firstBlockNode)) {
                    lineIndex++;
                    continue;
                }

                if(firstBlockNode.tagName == "LI") {
                    var parentLI = ASPx.GetParentByTagName(firstLineNode, "LI");
                    var parentList = parentLI.parentNode;
                    var prev = parentList.previousSibling;

                    if(parentList.tagName != tagName) {
                        if(!prev || prev.nodeType != 1 || prev.tagName != tagName) {
                            prev = document.createElement(tagName);
                            parentList.parentNode.insertBefore(prev, parentList);
                        }
                        prev.appendChild(parentLI.cloneNode(true));
                        ASPx.RemoveElement(parentLI);
                    }
                    else if(prev && prev.nodeType == 1 && prev.tagName == tagName) {
                        prev.appendChild(parentLI.cloneNode(true));
                        ASPx.RemoveElement(parentLI);
                    }

                    if(ASPx.GetInnerText(parentList) == "")
                        parentList.parentNode.removeChild(parentList);
                }
                else {
                    // create BulletList object
                    var list = doc.createElement(tagName);
                    var addInBegin = false;

                    if(isMoveOutTheBlock) {
                        replacedElement = firstBlockNode;
                        parent = replacedElement.parentNode;

                        var node = replacedElement.previousSibling;
                        if(node && node.nodeType == 1 && node.tagName == tagName)
                            list = node;
                        else
                            parent.insertBefore(list, replacedElement);

                        parent = replacedElement;
                    }
                    else {
                        parent = firstLineElement.parentNode;
                        var prevNode = firstLineElement.previousSibling;
                        var nextNode = lastLineElement.nextSibling;
                        if(prevNode && prevNode.nodeType == 1 && prevNode.tagName == tagName)
                            list = prevNode;
                        else if(nextNode && nextNode.nodeType == 1 && nextNode.tagName == tagName) {
                            list = nextNode;
                            addInBegin = true;
                        }
                        else {
                            parent.insertBefore(list, firstLineElement);
                            if(this.IsBoundaryElement(list.previousSibling))
                                parent.removeChild(list.previousSibling);
                        }
                    }

                    // create ListItem object and copy line nodes
                    var li = doc.createElement("LI");
                    if(addInBegin)
                        list.insertBefore(li, list.firstChild);
                    else
                        list.appendChild(li);

                    var textNodes = ASPx.HtmlEditorClasses.Commands.Utils.SplitNode(parent, [firstLineNode, lastLineNode]);
                    if(isMoveOutTheBlock)
                        this.AppendLineElement(li, replacedElement, textNodes[1].cloneNode(true));
                    else {
                        var childs = textNodes[1].childNodes;
                        for(var j = 0; j < childs.length; j++)
                            this.AppendLineElement(li, null, childs[j].cloneNode(true));
                    }

                    this.RemoveLineBreakElement(parent, firstLineNode, lastLineNode);
                    this.RemoveLineElement(parent, firstLineNode, lastLineNode);

                    if(replacedElement && ASPx.GetInnerText(replacedElement) == "")
                        replacedElement.parentNode.removeChild(replacedElement);
                }

                lineIndex++;
            }
            return true;
        },
        AppendLineElement: function(li, lineBlock, node) {
            if(lineBlock && {p:1, div:1}[node.tagName.toLowerCase()]) {
                ASPx.Attr.CopyAllAttributes(lineBlock, li);
                li.innerHTML = node.innerHTML;
            }
            else
                li.appendChild(node);
        },
        RemoveLineBreakElement: function(lineBlock, firstLineNode, lastLineNode) {
            var isRemoved = false;
            var first = firstLineNode;
            var last = lastLineNode;
            var parent = first.parentNode;
            do {
                if(last && this.IsBoundaryElement(last.nextSibling)) {
                    last.parentNode.removeChild(last.nextSibling);
                    isRemoved = true;
                }
                else if(first && this.IsBoundaryElement(first.previousSibling)) {
                    first.parentNode.removeChild(first.previousSibling);
                    isRemoved = true;
                }
                else {
                    first = first ? first.previousSibling : null;
                    last = last ? last.nextSibling : null;
                    if(!(first || last)) {
                        first = parent.previousSibling ? parent.previousSibling : null;
                        last = parent.nextSibling ? parent.nextSibling : null;
                        if(parent != lineBlock)
                            parent = parent.parentNode;
                    }
                }
            } while(!isRemoved && parent != lineBlock);
        },
        RemoveLineElement: function(lineBlock, firstLineNode, lastLineNode) {
            if(firstLineNode == lastLineNode) {
                firstLineNode.parentNode.removeChild(firstLineNode);
                return;
            }
            var isNeedRemoveElement = (arguments[3] == true);
            if(lineBlock.hasChildNodes()) {
                var childs = lineBlock.childNodes;
                var index = 0;
                while(childs[index]) {

                    if(childs[index].hasChildNodes())
                        isNeedRemoveElement = this.RemoveLineElement(childs[index], firstLineNode, lastLineNode, isNeedRemoveElement);

                    if(childs[index] == firstLineNode)
                        isNeedRemoveElement = true;

                    if(isNeedRemoveElement) {
                        if(childs[index] == lastLineNode)
                            isNeedRemoveElement = false;
                        lineBlock.removeChild(childs[index]);
                    }
                    else
                        index++;
                }
            }
            return isNeedRemoveElement;
        },
        GetBlockSelectedElement: function(element) {
            while(element && !(this.IsBlockLimitElement(element) || this.IsBlockElement(element)))
                element = element.parentNode;
            return element;
        },
        GetLineSelectedElement: function(element) {
            var parent = element.parentNode;
            while(parent && !this.IsBlockLimitElement(parent) && !this.IsBlockElement(parent)) {
                element = parent;
                parent = parent.parentNode;
            }
            return element;
        },
        GetLineInfos: function(selectedElement) {
            var properties = {
                lineInfos: [],
                lineElements: []
            };
            properties = this.SearchHtmlLines(selectedElement, properties);
            return properties.lineInfos;
        },
        GetSelectedLineInfos: function(parentElement, lineInfos, bookmark) {
            var startLineIndex = -1,
                endLineIndex = -1,
                i = 0,
                lineCount = lineInfos.length;

            while(i < lineCount) {
                var line = lineInfos[i];

                // Search markers in currentLine
                var markers;
                if(line.length == 1 && line[0].nodeType == 1)
                    markers = this.SearchMarkers(line[0], bookmark);
                else if(line.length > 1) {
                    params = [line[0], line[line.length - 1]];
                    parts = ASPx.HtmlEditorClasses.Commands.Utils.SplitNode(parentElement, params);
                    markers = this.SearchMarkers(parts[1], bookmark);
                }
                if(markers) {
                    if(startLineIndex == -1 && markers.startMarkerIsFound)
                        startLineIndex = i;
                    if(endLineIndex == -1 && markers.endMarkerIsFound) {
                        endLineIndex = i;
                        break;
                    }
                }
                i++;
            }

            return {
                startLineIndex: startLineIndex,
                endLineIndex: endLineIndex
            };
        },
        SearchHtmlLines: function(element, properties) {
            var nodes = element.childNodes;
            for(var i = 0; i < nodes.length; i++) {
                var isLineBreak = this.IsBoundaryElement(nodes[i]) || this.IsBlockElement(nodes[i]) || this.IsBlockLimitElement(nodes[i]);
                if( !isLineBreak ) {
                    if(nodes[i].hasChildNodes())
                        properties = this.SearchHtmlLines(nodes[i], properties);
                    else
                        properties.lineElements.push(nodes[i]);
                }
                else {
                    if(properties.lineElements.length > 0) {
                        properties.lineInfos.push( properties.lineElements );
                        properties.lineElements = [];
                    }
                    if(nodes[i].hasChildNodes())
                        properties = this.SearchHtmlLines(nodes[i], properties);
                }
            }

            if(this.IsBlockElement(element) || this.IsBlockLimitElement(element)) {
                if(properties.lineElements.length > 0) {
                    properties.lineInfos.push( properties.lineElements );
                    properties.lineElements = [];
                }
            }
            return properties;
        },
        SearchMarkers: function(element, bookmark) {
            var start = false;
            var end = false;

            if(element.nodeType == 1) {
                if(element.id == bookmark.startMarkerID)
                    start = true;
                if(element.id == bookmark.endMarkerID)
                    end = true;
                if(element.hasChildNodes()) {
                    var childs = element.childNodes;
                    for(var i = 0; i < childs.length; i++) {
                        var markers = this.SearchMarkers(childs[i], bookmark);
                        start = start || markers.startMarkerIsFound;
                        end = end || markers.endMarkerIsFound;
                    }
                }
            }

            return {
                startMarkerIsFound: start,
                endMarkerIsFound: end
            };
        },
        IsAcceptableElement: function(element) {
            var unacceptableElements = {colgroup: 1, table: 1, tbody: 1, thead:1, tfoot:1, tr: 1 };
            if(element.nodeType == 1)
                return !unacceptableElements[element.tagName.toLowerCase()];
            return true;
        },
        IsBlockElement: function(element) {
            return ASPx.HtmlEditorClasses.Commands.Enter.IsBlockNode(element);
        },
        IsBlockLimitElement: function(element) {
            if(!this.limiters) {
                this.limiters = {};
                for(var e in ASPx.HEPathBlockLimitElements) {
                    if(!{div:1}[e])
                        this.limiters[e] = 1;
                }
            }
            return element.nodeType == 1
                && !!this.limiters[element.tagName.toLowerCase()];
        },
        IsBoundaryElement: function(element) {
            return element != null && element.nodeType == 1 && element.tagName.toUpperCase() == "BR";
        },
        SkipStateOnFirstLoad: function() {
            return true;
        },
        canBeExecutedOnSelection: function(wrapper, curSelection) {
            return true;
        }
    });
    ASPx.HtmlEditorClasses.Commands.Browser.InsertList.RemoveListItems = function(startIndex, endIndex, listItemsArray) {
        var removeListItemsIndexArray = [];
        for(var i = startIndex; i <= endIndex; i++)
            removeListItemsIndexArray.push(i);
        ASPx.HtmlEditorClasses.Commands.Browser.InsertList.OutdentListItemsByIndex(removeListItemsIndexArray, listItemsArray);
        var listItemsIndexArray = [];
        for(var listItem, j = 0; listItem = listItemsArray[j]; j++) {
            if(ASPx.Data.ArrayIndexOf(removeListItemsIndexArray, listItem.parentItemIndex) > -1)
                listItemsIndexArray.push(j);
        }
        var parentItemIndex = startIndex == 0 ? -1 : startIndex - 1;
        for(var listItem, j = endIndex + 1; listItem = listItemsArray[j]; j++) {
            if(ASPx.Data.ArrayIndexOf(listItemsIndexArray, j) < 0) {
                if(listItem.parentItemIndex > endIndex)
                    listItem.parentItemIndex -= removeListItemsIndexArray.length;
            }
            else
                listItem.parentItemIndex = parentItemIndex;
        }
        listItemsArray.splice(startIndex, removeListItemsIndexArray.length);
    }
    ASPx.HtmlEditorClasses.Commands.Browser.InsertList.OutdentListItemsByIndex = function(indexArray, listItemsArray) {
        for(var i = 0; i < indexArray.length; i++) {  
            var index = indexArray[i];
            var listItemsIndexArray = [];
            for(var listItem, j = 0; listItem = listItemsArray[j]; j++) {
                if(listItem.parentItemIndex == index)
                    listItemsIndexArray.push(j);
            }
            listItemsArray[index].level -= 1;
            if(listItemsArray[index].level > 0) {
                for(var listItem, j = index - 1; listItem = listItemsArray[j]; j--) {
                    if((listItemsArray[index].level - listItem.level) > 0) {
                        listItemsArray[index].parentItemIndex = j;
                        break;
                    }
                }
            }
            else if(listItemsArray[index].level == 0)
                listItemsArray[index].parentItemIndex = -1;
            ASPx.HtmlEditorClasses.Commands.Browser.InsertList.OutdentListItemsByIndex(listItemsIndexArray, listItemsArray);
        }
    }

    ASPx.HtmlEditorClasses.Commands.Browser.InsertList.UpdateListNumberingByHtml = function(html, doc) {
        var tempElement = doc.createElement("DIV");
        tempElement.innerHTML = html;
        ASPx.HtmlEditorClasses.Commands.Browser.InsertList.UpdateListNumbering(tempElement);
        return tempElement.innerHTML;
    }
    ASPx.HtmlEditorClasses.Commands.Browser.InsertList.UpdateListNumbering = function(element, getListIdAttrValue) {
        var array = element.innerHTML.match(/<ol[^>]*start\s*=\s*[^>]*>/gi);
        if(array && array.length < 1)
            return;
        var parentListArray = ASPx.HtmlEditorClasses.Utils.getChildsByPredicate(element, function(el) { return el.nodeName == "OL"; });
        for(var i = 0, parentList; parentList = parentListArray[i]; i++) {
            if(ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetMainParentList(parentList.parentNode)) {
                ASPx.Data.ArrayRemove(parentListArray, parentList);
                i--;
            }
        }
        var needInitHeadParentList, newHeadListIdAttrValue;
        if(!getListIdAttrValue)
            getListIdAttrValue = function(el) { return ASPx.Attr.GetAttribute(el, ASPx.HtmlEditorClasses.ListIdAttributeName); };
        for(var i = 0, parentList; parentList = parentListArray[i]; i++) {
            var headListIdAttrValue = getListIdAttrValue(parentList);
            var headListType = ASPx.Attr.GetAttribute(parentList.style, "listStyleType");

            var listItems = ASPx.GetChildNodesByTagName(parentList, "LI");
            var needRestartListNumbering = headListIdAttrValue && !ASPx.Attr.GetAttribute(parentList, ASPx.HtmlEditorClasses.HeadListAttributeName);
            if(needRestartListNumbering)
                ASPx.Attr.RemoveAttribute(parentList, "start");
            var startAttrValue = needRestartListNumbering ? 0 : parseInt(ASPx.Attr.GetAttribute(parentList, "start"));
            var startValue = startAttrValue && startAttrValue != NaN ? startAttrValue + listItems.length : listItems.length + 1;

            if(!headListIdAttrValue)
                newHeadListIdAttrValue = ASPx.HtmlEditorClasses.Selection.CreateUniqueID();

            needInitHeadParentList = false;
            for(var j = i + 1, nextParentList; nextParentList = parentListArray[j]; j++) {
                var nextParentListIdAttrValue = getListIdAttrValue(nextParentList);
                if(headListType == ASPx.Attr.GetAttribute(nextParentList.style, "listStyleType") && (!headListIdAttrValue && !nextParentListIdAttrValue && startValue == ASPx.Attr.GetAttribute(nextParentList, "start") || nextParentListIdAttrValue && nextParentListIdAttrValue == headListIdAttrValue)) {
                    if(nextParentListIdAttrValue)
                        ASPx.Attr.SetAttribute(nextParentList, "start", startValue);
                    ASPx.Attr.SetAttribute(nextParentList, ASPx.HtmlEditorClasses.ListIdAttributeName, headListIdAttrValue ? headListIdAttrValue : newHeadListIdAttrValue);
                    ASPx.Data.ArrayRemove(parentListArray, nextParentList);
                    startValue += ASPx.GetChildNodesByTagName(nextParentList, "LI").length;
                    needInitHeadParentList = true;
                    j--;
                }
            }
            if(needInitHeadParentList) {
                ASPx.Attr.SetAttribute(parentList, ASPx.HtmlEditorClasses.ListIdAttributeName, headListIdAttrValue ? headListIdAttrValue : newHeadListIdAttrValue);
                ASPx.Attr.SetAttribute(parentList, ASPx.HtmlEditorClasses.HeadListAttributeName, "true");
            }
            needInitHeadParentList = false;
        }
    }
    ASPx.HtmlEditorClasses.Commands.Browser.InsertList.MergeLists = function(element) {
        var getListStyleTypeAttrValue = function(el) {
            var attrValue = ASPx.Attr.GetAttribute(el.style, "listStyleType");
            return el.nodeName == "UL" && attrValue == "disc" ? "" : attrValue;
        };
        var parentListArray = ASPx.HtmlEditorClasses.Utils.getChildsByPredicate(element, function(el) { return el.nodeName == "OL" || el.nodeName == "UL"; });
        for(var i = 0, parentList, nextParentList; i < parentListArray.length - 1, parentList = parentListArray[i], nextParentList = parentListArray[i + 1]; i++) {
            if(parentList.nextElementSibling == nextParentList && parentList.nodeName == nextParentList.nodeName && getListStyleTypeAttrValue(parentList) == getListStyleTypeAttrValue(nextParentList)) {
                for(var j = 0, child; child = nextParentList.childNodes[j]; j++)
                    parentList.appendChild(child.cloneNode(true));
                nextParentList.parentNode.removeChild(nextParentList);
                ASPx.Data.ArrayRemove(parentListArray, nextParentList);
                i--;
            }
        }
    }

    ASPx.HtmlEditorClasses.Commands.Browser.RestartOrderedList = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
        Execute: function(cmdValue, wrapper) {
            if(!this.canBeExecutedOnSelection(wrapper))
                return false;
            var selectedListItem = ASPx.GetParentByTagName(wrapper.getSelection().GetSelectedElement(), "LI");
            var selectedParentList = selectedListItem.parentNode;
            var selectedParentListId = ASPx.Attr.GetAttribute(selectedParentList, ASPx.HtmlEditorClasses.ListIdAttributeName);
            if(selectedListItem.previousSibling) {
                var newListItemArray = [];
                var listItemArray = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetListItemArray(selectedParentList, 0);
                for(var i = 0, listItem; listItem = listItemArray[i]; i++) {
                    if(listItem.node == selectedListItem)
                        break;
                    else {
                        newListItemArray.push(listItem);
                        ASPx.Data.ArrayRemove(listItemArray, listItem);
                        i--;
                    }
                }
                var firstParentList = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GenerateListByItemArray(newListItemArray);
                var secondParentList = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GenerateListByItemArray(listItemArray);
                var listType = ASPx.Attr.GetAttribute(selectedParentList.style, "listStyleType");
                ASPx.Attr.SetAttribute(firstParentList.style, "listStyleType", listType);
                ASPx.Attr.SetAttribute(secondParentList.style, "listStyleType", listType);
                var listId = ASPx.Attr.GetAttribute(selectedParentList, ASPx.HtmlEditorClasses.ListIdAttributeName);
                if(ASPx.Attr.IsExistsAttribute(selectedParentList, ASPx.HtmlEditorClasses.HeadListAttributeName)) {
                    ASPx.Attr.RemoveAttribute(firstParentList, ASPx.HtmlEditorClasses.ListIdAttributeName);
                    ASPx.Attr.RemoveAttribute(firstParentList, ASPx.HtmlEditorClasses.HeadListAttributeName);
                }
                selectedParentList.parentNode.insertBefore(firstParentList, selectedParentList);
                selectedParentList.parentNode.insertBefore(secondParentList, selectedParentList);
                selectedParentList.parentElement.removeChild(selectedParentList);
                selectedParentList = secondParentList;
            }
            var parentListArray = ASPx.HtmlEditorClasses.Utils.getChildsByPredicate(wrapper.getBody(), function(el) { return ASPx.Attr.IsExistsAttribute(el, ASPx.HtmlEditorClasses.ListIdAttributeName) && ASPx.Attr.GetAttribute(el, ASPx.HtmlEditorClasses.ListIdAttributeName) == selectedParentListId; }.aspxBind(this));
            for(var i = 0, list; list = parentListArray[i]; i++) {
                if(list == selectedParentList)
                    break;
                else {
                    ASPx.Data.ArrayRemove(parentListArray, list);
                    i--;
                }
            }
            if(parentListArray.length > 1) {
                var listId = ASPx.HtmlEditorClasses.Selection.CreateUniqueID();
                ASPx.Attr.SetAttribute(selectedParentList, ASPx.HtmlEditorClasses.HeadListAttributeName, "true");
                ASPx.Attr.RemoveAttribute(selectedParentList, "start");
                for(var i = 0, list; list = parentListArray[i]; i++)
                    ASPx.Attr.SetAttribute(list, ASPx.HtmlEditorClasses.ListIdAttributeName, listId);
            }
            else {
                ASPx.Attr.RemoveAttribute(selectedParentList, ASPx.HtmlEditorClasses.ListIdAttributeName);
                ASPx.Attr.RemoveAttribute(selectedParentList, ASPx.HtmlEditorClasses.HeadListAttributeName);
                ASPx.Attr.RemoveAttribute(selectedParentList, "start");
            }
            ASPx.HtmlEditorClasses.Commands.Browser.InsertList.UpdateListNumbering(wrapper.getBody());
            return true;
        },
        canBeExecutedOnSelection: function(wrapper, curSelection) {
            var selectedListItem = ASPx.GetParentByTagName(wrapper.getSelection().GetSelectedElement(), "LI");
            if(!selectedListItem)
                return false;
            var selectedParentList = selectedListItem.parentNode;
            if(selectedParentList.tagName == "UL" || ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetMainParentList(selectedParentList.parentNode))
                return false;
            return true;
        },
        SkipStateOnFirstLoad: function() {
            return true;
        }
    });

    ASPx.HtmlEditorClasses.Commands.Browser.ContinueOrderedList = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
        Execute: function(cmdValue, wrapper) {
            if(!this.canBeExecutedOnSelection(wrapper))
                return false;
            var selectedParentList = ASPx.GetParentByTagName(wrapper.getSelection().GetSelectedElement(), "LI").parentNode;
            var parentListArray = ASPx.HtmlEditorClasses.Utils.getChildsByPredicate(wrapper.getBody(), function(el) { return el.nodeName == "OL"; });
            for(var i = 0, parentList; parentList = parentListArray[i]; i++) {
                if(ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetMainParentList(parentList.parentNode)) {
                    ASPx.Data.ArrayRemove(parentListArray, parentList);
                    i--;
                }
            }
            var prevParentList;
            for(var i = 0, parentList; parentList = parentListArray[i]; i++) {
                if(parentList == selectedParentList) {
                    if(i > 0)
                        prevParentList = parentListArray[i - 1];
                    break;
                }
            }
            if(prevParentList) {
                if(!ASPx.Attr.IsExistsAttribute(prevParentList, ASPx.HtmlEditorClasses.ListIdAttributeName)) {
                    ASPx.Attr.SetAttribute(prevParentList, ASPx.HtmlEditorClasses.HeadListAttributeName, "true");
                    if(ASPx.Attr.IsExistsAttribute(selectedParentList, ASPx.HtmlEditorClasses.ListIdAttributeName))
                        ASPx.Attr.SetAttribute(prevParentList, ASPx.HtmlEditorClasses.ListIdAttributeName, ASPx.Attr.GetAttribute(selectedParentList, ASPx.HtmlEditorClasses.ListIdAttributeName));
                    else {
                        var newParentListId = ASPx.HtmlEditorClasses.Selection.CreateUniqueID();
                        ASPx.Attr.SetAttribute(prevParentList, ASPx.HtmlEditorClasses.ListIdAttributeName, newParentListId);
                        ASPx.Attr.SetAttribute(selectedParentList, ASPx.HtmlEditorClasses.ListIdAttributeName, newParentListId);
                    }
                }
                else {
                    var parentListId = ASPx.Attr.GetAttribute(selectedParentList, ASPx.HtmlEditorClasses.ListIdAttributeName);
                    var prevParentListId = ASPx.Attr.GetAttribute(prevParentList, ASPx.HtmlEditorClasses.ListIdAttributeName);
                    if(!parentListId) 
                        ASPx.Attr.SetAttribute(selectedParentList, ASPx.HtmlEditorClasses.ListIdAttributeName, prevParentListId);
                    else {
                        var parentListArray = ASPx.HtmlEditorClasses.Utils.getChildsByPredicate(wrapper.getBody(), function(el) { return ASPx.Attr.IsExistsAttribute(el, ASPx.HtmlEditorClasses.ListIdAttributeName) && ASPx.Attr.GetAttribute(el, ASPx.HtmlEditorClasses.ListIdAttributeName) == parentListId; }.aspxBind(this));
                        for(var i = 0, parentList; parentList = parentListArray[i]; i++) {
                            if(parentList == selectedParentList)
                                break;
                            else {
                                ASPx.Data.ArrayRemove(parentListArray, parentList);
                                i--;
                            }
                        }
                        for(var i = 0, parentList; parentList = parentListArray[i]; i++)
                            ASPx.Attr.SetAttribute(parentList, ASPx.HtmlEditorClasses.ListIdAttributeName, prevParentListId);
                    }
                }
                ASPx.Attr.RemoveAttribute(selectedParentList, ASPx.HtmlEditorClasses.HeadListAttributeName);
                ASPx.Attr.RemoveAttribute(selectedParentList, "start");
                ASPx.HtmlEditorClasses.Commands.Browser.InsertList.UpdateListNumbering(wrapper.getBody());
                ASPx.HtmlEditorClasses.Commands.Browser.InsertList.MergeLists(wrapper.getBody());
            }
            return true;
        },
        canBeExecutedOnSelection: function(wrapper, curSelection) {
            var selectedListItem = ASPx.GetParentByTagName(wrapper.getSelection().GetSelectedElement(), "LI");
            if(!selectedListItem || selectedListItem.previousSibling)
                return false;
            var selectedParentList = selectedListItem.parentNode;
            if(selectedParentList.tagName == "UL" || ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetMainParentList(selectedParentList.parentNode))
                return false;
            return true;
        },
        SkipStateOnFirstLoad: function() {
            return true;
        }
    });
})();