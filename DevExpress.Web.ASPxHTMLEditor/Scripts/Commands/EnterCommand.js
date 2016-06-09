(function() {
    ASPx.HtmlEditorClasses.Commands.Enter = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
        constructor: function(cmdID) {
            ASPx.HtmlEditorClasses.Commands.Command.prototype.constructor.call(this, cmdID);

            this.passCommandToBrowser;
        },
        IsDefaultAction: function(wrapper) {
            return Boolean(this.passCommandToBrowser);
        },
        ReplaceWithNewLine: function(controlElement, lineFeedNodeTagName) {
            var newLineElement = controlElement.document.createElement(lineFeedNodeTagName);
            if(newLineElement.tagName != "BR") {
                newLineElement = this.EnsureEmptySymbolIs(newLineElement);
            }

            controlElement.parentNode.replaceChild(newLineElement, controlElement);
        },
        Execute: function(cmdValue, wrapper) {
            this.passCommandToBrowser = false;
            
            var enterMode = wrapper.settings.enterMode;
            if(enterMode == ASPx.HtmlEditorClasses.EnterMode.Default) {
                this.passCommandToBrowser = true;
                return false;
            }

            var selection = ASPxClientHtmlEditorSelection.Create(wrapper.getWindow());

            // TODO: Do something in case if selection.GetStartContainer() != selection.GetEndContainer()
            //       or let the browser sort our consequences?

            var doc = wrapper.getDocument();
            var win = wrapper.getWindow();

            var startContainer = selection.GetStartContainer();
            var startParentLI = ASPx.GetParentByTagName(startContainer, "LI");
            if(startParentLI) {
                var endParentLI = ASPx.GetParentByTagName(selection.GetEndContainer(), "LI");
                if(enterMode == ASPx.HtmlEditorClasses.EnterMode.P && this.TryMoveToParagraphFromLI(startParentLI, endParentLI, doc, selection))
                    return true;
                if(ASPx.Browser.IE && ASPx.Browser.MajorVersion > 10 && startParentLI == endParentLI && endParentLI.childNodes.length == 1 && endParentLI.firstChild.nodeName == "BR")
                    endParentLI.innerHTML = "";
                setTimeout(function() {
                    ASPx.HtmlEditorClasses.Commands.Browser.InsertList.UpdateListNumbering(wrapper.getBody());
                    wrapper.commandManager.updateLastRestoreSelectionAndHTML();
                }.aspxBind(this), 0);
                this.passCommandToBrowser = true;
                return false;
            }

            if(this.NodeOrOneOfItsParentsIsPre(selection.GetParentElement()))
                enterMode = ASPx.HtmlEditorClasses.EnterMode.BR;

            if(enterMode == ASPx.HtmlEditorClasses.EnterMode.BR) {
                if(wrapper.tagIsFiltered("BR"))
                    return false;
                var lineFeedNode;
                var lineFeedNodeTagName = "br";

                var range = selection.GetRange();

                if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 11) {
                    if(selection.IsControl()) {
                        var controlElement = range.item(0);
                        this.ReplaceWithNewLine(controlElement, lineFeedNodeTagName);
                        return true;
                    }

                    var uniqueID = ASPx.HtmlEditorClasses.Selection.CreateUniqueID();
                    range.pasteHTML("<" + lineFeedNodeTagName + " id=\"" + uniqueID + "\" />");
                    lineFeedNode = ASPx.GetElementByIdInDocument(doc, uniqueID);
                    ASPx.Attr.RemoveAttribute(lineFeedNode, "id");

                    if(ASPx.HtmlEditorClasses.Commands.Enter.IsNeedBogusNode(lineFeedNode)) {
                        range.collapse(false);
                        ASPx.HtmlEditorClasses.Commands.Enter.AppendBogusNode(lineFeedNode);
                    }

                    range.collapse(true);

                    // Updating selection
                    range.select();

                } else if(ASPx.Browser.NetscapeFamily || (ASPx.Browser.IE && ASPx.Browser.MajorVersion > 10)) {

                    range.deleteContents();

                    lineFeedNode = doc.createElement(lineFeedNodeTagName);
                    range.insertNode(lineFeedNode);
                    range.selectNode(lineFeedNode);
                    range.collapse(false);

                    if(ASPx.HtmlEditorClasses.Commands.Enter.IsNeedBogusNode(lineFeedNode))
                        ASPx.HtmlEditorClasses.Commands.Enter.AppendBogusNode(lineFeedNode);

                    // Updating selection
                    var nativeSelection = win.getSelection();
                    if(nativeSelection.rangeCount > 0)
                        nativeSelection.removeAllRanges();
                    nativeSelection.addRange(range);

                } else if(ASPx.Browser.Opera) {

                    range.deleteContents();

                    lineFeedNode = doc.createElement(lineFeedNodeTagName);
                    range.insertNode(lineFeedNode);

                    // B155410
                    var parentElement = selection.GetParentElement();
                    if(!ASPx.HtmlEditorClasses.Commands.Enter.IsBlockNode(parentElement) && ASPx.HtmlEditorClasses.Commands.Enter.NextNode(lineFeedNode) == null) {
                        var savedDisplayStyle = parentElement.style.display;
                        parentElement.style.display = "block";
                    }

                    // Updating selection
                    range = doc.createRange();
                    range.selectNode(lineFeedNode);
                    range.collapse(false);
                    win.getSelection().addRange(range);

                    if(ASPx.IsExists(savedDisplayStyle))
                        parentElement.style.display = savedDisplayStyle;

                } else if(ASPx.Browser.WebKitFamily) {

                    range.deleteContents();

                    lineFeedNode = doc.createElement(lineFeedNodeTagName);
                    range.insertNode(lineFeedNode);
                    range.selectNode(lineFeedNode);
                    range.collapse(false);

                    if(ASPx.HtmlEditorClasses.Commands.Enter.IsNeedBogusNode(lineFeedNode))
                        ASPx.HtmlEditorClasses.Commands.Enter.AppendBogusNode(lineFeedNode);

                    // Updating selection
                    selection = win.getSelection();
                    selection.setBaseAndExtent(range.startContainer, range.startOffset, range.endContainer, range.endOffset);
                }

                ASPx.HtmlEditorClasses.Commands.Enter.RemoveBogusNodes(doc.body);

                if(!ASPx.Browser.IE || ASPx.Browser.MajorVersion > 10) {
                    var elementToView = lineFeedNode.nextSibling ? lineFeedNode.nextSibling : lineFeedNode;
                    this.ScrollToElement(elementToView, doc);
                }

            } else if(enterMode == ASPx.HtmlEditorClasses.EnterMode.P) {
                if(wrapper.tagIsFiltered("p"))
                    return false;
                var bookmark = selection.GetExtendedBookmark();

                if(ASPx.Browser.IE && selection.IsControl()) {
                    this.ReplaceWithNewLine(doc.getElementById(bookmark.controlElementID), "P");
                    return true;
                }

                var startMarkerNode = doc.getElementById(bookmark.startMarkerID);
                var endMarkerNode = doc.getElementById(bookmark.endMarkerID);
                this.TryMoveBookmarkFromParent(startMarkerNode);

                var firstSplitBlockItem = this.GetSplitBlockItem("P", startMarkerNode, true);
                var lastSplitBlockItem = this.GetSplitBlockItem("P", endMarkerNode, false);

                var firstContainer = firstSplitBlockItem.block,
                    firstLimitContainer = firstSplitBlockItem.blockLimit,
                    lastContainer = lastSplitBlockItem.block,
                    lastLimitContainer = lastSplitBlockItem.blockLimit;
                var elements = firstSplitBlockItem.elements.concat(lastSplitBlockItem.elements);
            
                var paragraphElement = elements[elements.length-1];
                var deleteBrElement = false;
                if(!ASPx.Browser.IE) {
                    if(paragraphElement.nodeType == 1 && !paragraphElement.innerHTML.replace(/<span[^>]*id\s*=\s*['\"]dx_temp[^'\"]*['\"][^>]*><\/span>\n\s*$/, ""))
                        paragraphElement.innerHTML = paragraphElement.innerHTML.replace(/\n\s*$/, "");
                    else {
                        deleteBrElement = paragraphElement.childNodes.length > 1;
                        if(!ASPx.Browser.WebKitFamily && deleteBrElement) {
                            var node = paragraphElement.childNodes[1];
                            deleteBrElement = (node.nodeName != "BR" && node.nextSibling) || node.nodeName == "#text";
                        }
                    }
                }

                if(firstContainer && (firstContainer == lastContainer || !lastContainer && firstLimitContainer.nodeName == "DIV" && paragraphElement.nodeName == "P")) {
                    var container = firstContainer;
                    for(var i = 0; i < elements.length; i++) {
                        if(i == 0)
                            container.parentNode.replaceChild(elements[i], container);
                        else
                            container.parentNode.insertBefore(elements[i], container.nextSibling);
                        container = elements[i];
                    }
                }
                else if(firstLimitContainer && firstLimitContainer == lastLimitContainer) {
                    ASPx.SetInnerHtml(firstLimitContainer, "");
                    for(var i = 0; i < elements.length; i++)
                        firstLimitContainer.appendChild(elements[i]);
                }
                else {
                    selection.RemoveExtendedBookmark(bookmark);
                    this.passCommandToBrowser = true;
                    return false;
                }

                var startSplitNode = doc.getElementById(bookmark.startMarkerID).parentNode;
                var endSplitNode = doc.getElementById(bookmark.endMarkerID).parentNode;

                selection.RemoveExtendedBookmark(bookmark);

                startSplitNode = this.EnsureEmptySymbolIs(startSplitNode);
                endSplitNode = this.EnsureEmptySymbolIs(endSplitNode);

                // Change caret position
                var selectElem = endSplitNode;
                var range = selection.GetRange();

                if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 11) {
                    if(ASPx.Browser.Version == 9) {
                        var fakeElem = doc.createElement("P");
                        selectElem.insertBefore(fakeElem, selectElem.firstChild);
                        selectElem = fakeElem;
                    }
                    range.moveToElementText(selectElem);
                }
                else {
                    if(selectElem.hasChildNodes())
                        selectElem = selectElem.firstChild;
                    selection.SelectElement(selectElem, true);
                    range = selection.GetRange();
                }

                var isStartSelection = !(ASPx.Browser.IE && ASPx.Browser.Version == 9);
                range.collapse(isStartSelection);

                if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 11) {
                    range.select();
                    if (ASPx.Browser.Version == 9)
                        selectElem.parentNode.removeChild(selectElem);
                } else if(ASPx.Browser.NetscapeFamily || (ASPx.Browser.IE && ASPx.Browser.MajorVersion > 10)) {

                    var emptyNode = doc.createTextNode("");
                    selectElem.parentNode.insertBefore(emptyNode, selectElem);
                    range.selectNode(emptyNode);
                    range.collapse(false);

                    // Updating selection
                    var nativeSelection = win.getSelection();
                    if(nativeSelection.rangeCount > 0)
                        nativeSelection.removeAllRanges();
                    nativeSelection.addRange(range);

                } else if(ASPx.Browser.Opera) {

                    doc.body.focus();

                    // Updating selection
                    range = doc.createRange();
                    range.selectNode(selectElem);
                    range.collapse(true);
                
                    var nativeSelection = win.getSelection();
                    if(ASPx.Browser.Version >= 10.5 && nativeSelection.rangeCount > 0)
                        nativeSelection.removeAllRanges();
                    nativeSelection.addRange(range);
                    var parentNode = selectElem.parentNode;
                    if(selectElem.innerHTML == "<br>" && parentNode && parentNode.previousSibling && parentNode.previousSibling.nodeName != selectElem.nodeName)
                        selectElem.parentNode.removeChild(selectElem);
                    else if(deleteBrElement && selectElem.nodeName == "BR" && selectElem.parentNode.nodeName == startSplitNode.nodeName)
                        endSplitNode.parentNode.removeChild(endSplitNode);

                } else if(ASPx.Browser.WebKitFamily) {
                    winSelection = win.getSelection();
                    winSelection.setBaseAndExtent(range.startContainer, range.startOffset, range.endContainer, range.endOffset);
                }

                if(!ASPx.Browser.IE || ASPx.Browser.MajorVersion > 10)
                    this.ScrollToElement(endSplitNode.firstChild, doc);
            }

            return true;
        },
        TryMoveToParagraphFromLI: function(startParentLI, endParentLI, doc, selection) {
            if(ASPx.Browser.WebKitFamily && startParentLI == endParentLI) {
                if(startParentLI.childNodes.length == 1 && startParentLI.firstChild.tagName == "BR" && startParentLI.parentNode.lastChild == endParentLI) {
                    var newNode = doc.createElement("P");
                    ASPx.InsertElementAfter(newNode, startParentLI.parentNode);
                    newNode.innerHTML = "<br />";
                    startParentLI.parentNode.removeChild(startParentLI);
                    selection.SelectElement(newNode.firstChild, true);
                    return true;
                }
            }
        },
        TryMoveBookmarkFromParent: function (bm) {
            var parent = bm.parentNode;
            parent.normalize();
            if (parent.tagName == "A" && parent.firstChild == bm) {
                parent.parentNode.insertBefore(bm, parent);
                this.TryMoveBookmarkFromParent(bm);
            }
        },
        NodeOrOneOfItsParentsIsPre: function(node) {
            while(node && node.tagName != "BODY") {
                if(node.tagName == "PRE")
                    return true;
                else
                    node = node.parentNode;
            }
            return false;
        },
        EnsureEmptySymbolIs: function(node) {
            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 11 && node.nodeType == 1 && ASPx.Str.Trim(node.innerHTML) == "") {
                node.innerHTML = "&nbsp;";
                node.innerText = "";
            }
            else if(node.nodeType == 3 && ASPx.Str.Trim(node.nodeValue) == "")
                node.nodeValue = "\xA0";
            else if(!node.hasChildNodes() || node.childNodes.length == 1 && node.firstChild.nodeType == 3 && (!node.firstChild.nodeValue || !node.innerHTML.replace(/\n\s*$/, ""))) {
                if(/\n\s*$/.test(node.innerHTML))
                    node.innerHTML = node.innerHTML.replace(/\n\s*$/, "");
                node.appendChild(node.ownerDocument.createElement("BR"));
            }
            return node;
        },
        GetSplitInfo: function(splitNode) {
            var block = null;
            var blockLimit = null;
            var elements = [];

            var node = splitNode;
            while(node && node.tagName != "BODY") {
                elements.push(node);
                node = node.parentNode;

                if(this.IsPathBlockElement(node)) {
                    block = node;
                    break;
                }
                if(this.IsPathBlockLimitElement(node)) {
                    blockLimit = node;
                    break;
                }
            }
            elements = elements.reverse();

            return {
                block: block,
                blockLimit: blockLimit,
                elements: elements
            };
        },
        GetSplitBlockItem: function(blockTag, splitNode, isFirst) {
            var doc = ASPx.GetElementDocument(splitNode);
            var splitInfo = this.GetSplitInfo(splitNode);
            var elements = splitInfo.elements;
            var getNextSibling = function(element) { return element.nextSibling; };
            var getSibling = function(element, func) { 
                element = func(element);
                while(element && element.nodeType == 3 && (!element.nodeValue || element.nodeValue.indexOf(" ") < 0 && !ASPx.Str.Trim(element.nodeValue)))
                    element = func(element);
                return element;
            };
            var nextNode = getSibling(splitNode, getNextSibling);
            if(splitInfo.blockLimit && splitInfo.blockLimit.tagName == "DIV" && nextNode && (nextNode.nodeName != "BR" || nextNode.nextSibling)) {
                blockTag = "DIV";
                splitInfo.block = splitInfo.blockLimit;
            }
            var srcNode = null;
            var destNode = splitInfo.block && (!/H[1-6]/.test(splitInfo.block.nodeName) || getSibling(splitNode, getNextSibling) || isFirst) ? splitInfo.block.cloneNode(false) : doc.createElement(blockTag);
            var splitBlock = destNode;

            for(var i = 0; i < elements.length; i++) {
                srcNode = elements[i];
                destNode = destNode.appendChild(srcNode.cloneNode(false));

                while(srcNode = isFirst ? srcNode.previousSibling : srcNode.nextSibling) {
                    var parent = destNode.parentNode;
                    if(isFirst)
                        parent.insertBefore(srcNode.cloneNode(true), parent.firstChild);
                    else
                        parent.appendChild(srcNode.cloneNode(true));
                }
            }
            return {
                block: splitInfo.block,
                blockLimit: splitInfo.blockLimit,
                elements: this.ValidateElements(splitBlock, isFirst)
            };
        },
        IsPathBlockElement: function(element) {
            return element.nodeType == 1
                && ASPx.HEPathBlockElements[element.tagName.toLowerCase()];
        },
        IsPathBlockLimitElement: function(element) {
            return element.nodeType == 1
                && (ASPx.HEPathBlockLimitElements[element.tagName.toLowerCase()] || /ol|ul/gi.test(element.tagName));
        },
        IsValidBlock: function(parentBlock, childBlock) {
            if(this.IsPathBlockElement(parentBlock) && 
                (this.IsPathBlockElement(childBlock) || this.IsPathBlockLimitElement(childBlock))) {
                return false;
            }
            return true;
        },
        ValidateElements: function(splitBlock, isFirst) {
            var elements = [];
            var isNeedToWrap = true;

            if(splitBlock.hasChildNodes()) {
                var count = splitBlock.childNodes.length;
                for(var i = isFirst ? count-1 : 0; isFirst ? i >= 0 : i < count; isFirst ? i-- : i++ ) {

                    var node = splitBlock.childNodes[i].cloneNode(true);

                    if(!this.IsValidBlock(splitBlock, node) || !isNeedToWrap) {
                        isNeedToWrap = false;
                        elements.push(node);
                    }
                    else {
                        if(elements.length == 0)
                            elements.push(splitBlock.cloneNode(false));
                        if(isFirst)
                            elements[0].insertBefore(node, elements[0].firstChild);
                        else
                            elements[0].appendChild(node);
                    }
                }
            }
            if(isFirst)
                elements = elements.reverse();
            return elements;
        },
        ScrollToElement: function(element, doc) {
            var elementPosY = 0;
            var elementHeight = 0;

            var parentNode = element.parentNode;

            var fakeElem = doc.createElement("DIV");
            fakeElem.innerHTML = "&nbsp;";
            parentNode.insertBefore(fakeElem, element);

            elementPosY = fakeElem.offsetTop;
            elementHeight = fakeElem.offsetHeight;

            parentNode.removeChild(fakeElem);

            var newScrollTop = (elementPosY + elementHeight) - doc.body.clientHeight;
            if(newScrollTop > doc.body.scrollTop)
                doc.body.scrollTop = Math.max(newScrollTop, 0);
        }
    });

    ASPx.HtmlEditorClasses.Commands.Enter.NextNode = function(node) {
        var next = node.nextSibling;
        if(next && next.nodeType == 3 && next.nodeValue == "")
            next = ASPx.HtmlEditorClasses.Commands.Enter.NextNode(next);
        return next;
    };
    ASPx.HtmlEditorClasses.Commands.Enter.PreviousNode = function(node) {
        var prev = node.previousSibling;
        if(prev && prev.nodeType == 3 && prev.nodeValue == "")
            prev = ASPx.HtmlEditorClasses.Commands.Enter.PreviousNode(prev);
        return prev;
    };
    ASPx.HtmlEditorClasses.Commands.Enter.IsBogusNode = function(node) {
        if(node && node.nodeType == 3) {
            if(ASPx.Browser.IE && node.parentNode && node.nodeValue == ASPx.HEBogusSymbol)
                return true;
            else if(ASPx.Browser.NetscapeFamily || ASPx.Browser.WebKitFamily) {
                if(node.nodeValue == ASPx.HEBogusSymbol)
                    return true;
            }
        }
        return false;
    };
    ASPx.HtmlEditorClasses.Commands.Enter.IsBlockNode = function(node) {
        if(node && node.nodeType == 1) {
            if(ASPx.HEBlockElements[node.nodeName.toLowerCase()]) {
                if(node.style.display != "inline")
                    return true;
            }
            else if(node.style.display == "block")
                return true;
        }
        return false;
    };
    ASPx.HtmlEditorClasses.Commands.Enter.IsNeedBogusNode = function(node) {
        var prev = ASPx.HtmlEditorClasses.Commands.Enter.PreviousNode(node);
        var next = ASPx.HtmlEditorClasses.Commands.Enter.NextNode(node);

        if(ASPx.Browser.NetscapeFamily && prev && prev.nodeName == "BR") {
            if(ASPx.Attr.GetAttribute(prev, "type") == "_moz")
                return false;
        }

        if(!ASPx.HtmlEditorClasses.Commands.Enter.IsBlockNode(prev) && (ASPx.HtmlEditorClasses.Commands.Enter.IsBlockNode(next) || next == null))
            return true;
        return false;
    };
    ASPx.HtmlEditorClasses.Commands.Enter.AppendBogusNode = function(node) {
        var doc = ASPx.GetElementDocument(node);
        return node.parentNode.insertBefore(doc.createTextNode(ASPx.HEBogusSymbol), node.nextSibling);
    };
    ASPx.HtmlEditorClasses.Commands.Enter.RemoveBogusNodes = function(node) {
        var textNodes = [ ];
        ASPx.GetTextNodes(node, textNodes);
        for(var i = textNodes.length - 1; i > 0; i--)
            if(ASPx.HtmlEditorClasses.Commands.Enter.IsBogusNode(textNodes[i]) && !ASPx.HtmlEditorClasses.Commands.Enter.IsNeedBogusNode(textNodes[i]))
                textNodes[i].parentNode.removeChild(textNodes[i]);
    };
})();