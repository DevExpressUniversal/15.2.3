

(function () {
    var ASPxClientHtmlEditorSelection = ASPx.CreateClass(null, {
        constructor: function (iframeWrapper, selectionManager) {
            this.iframeWrapper = iframeWrapper;
            this.selectionManager = selectionManager;
            this.clientSelection = ASPxClientHtmlEditorSelection.Create(iframeWrapper.getWindow());
            this.clientSelection.Save();
        },
        Apply: function () {
            this.clientSelection.Restore();
        },
        GetIsControlSelected: function () {
            if(this.isControlSelected == undefined)
                this.isControlSelected = this.clientSelection.IsControl();
            return this.isControlSelected;
        },
        GetSelectedElement: function () {
            if(this.selectedElement == undefined)
                this.selectedElement = this.clientSelection.GetParentElement();
            return this.selectedElement;
        },
        GetHtml: function () {
            if(this.html == undefined)
                this.html = this.clientSelection.GetHtmlText();
            return this.html;
        },
        GetText: function () {
            if(this.text == undefined)
                this.text = this.clientSelection.GetText();
            return this.text;
        },

        IsCollapsed: function () {
            return this.GetHtml().length == 0 || (ASPx.Browser.IE && (this.GetHtml().indexOf("&nbsp;") > -1) && this.GetText().length == 0);
        },
        GetElements: function (removeEmptyElements) {
            var selection = this.iframeWrapper.selectionManager.createSelection();
            var elements = [];
            if(selection.GetHtml()) {
                var selectedElement = selection.GetSelectedElement();
                if(/^(img|input|textarea|select|option|button)$/gi.test(selectedElement.tagName) || ASPx.ElementHasCssClass(selectedElement, ASPx.HtmlEditorClasses.SelectedContainerCssClasseName))
                    elements.push(selectedElement);
                else {
                    var bm = selection.clientSelection.GetExtendedBookmark();
                    elements = this.GetElementsCore(bm, removeEmptyElements);
                    selection.clientSelection.SelectExtendedBookmark(bm);
                }
            }
            if(elements.length == 1 && elements[0].tagName == "BODY")
                elements = ASPx.Data.CollectionToArray(elements[0].childNodes);
            elements = this.TryUnionElements(elements);
            return elements;
        },
        GetElementsCore: function (bm, removeEmptyElements) {
            var bmElements = ASPx.HtmlEditorClasses.Selection.GetBookmarkElements(bm, this.clientSelection.contentDocument);
            if(!ASPx.Browser.IE)
                this.PrepareBookmarks(bmElements.startMarker, bmElements.endMarker);
            return ASPx.HtmlEditorClasses.Utils.ElementsSelectionHelper.GetSelectedElements(this.iframeWrapper.getDocument(), 
                bmElements.startMarker, bmElements.endMarker, removeEmptyElements);
        },
        TryUnionElements: function (elements) {
            if(elements.length > 1) {
                var firstEl = elements[0];
                var lastEl = elements[elements.length - 1];
                if(firstEl.parentNode == lastEl.parentNode && firstEl.parentNode.tagName != "BODY" && !firstEl.previousSibling && !lastEl.nextSibling)
                    elements = [firstEl.parentNode];
            }
            return elements;
        },
        PrepareBookmarks: function (startBm, endBm) {
            if(startBm.previousSibling && startBm.previousSibling.nodeType == 3 && startBm.previousSibling.nodeValue == "")
                ASPx.RemoveElement(startBm.previousSibling);
            if(endBm.nextSibling && endBm.nextSibling.nodeType == 3 && endBm.nextSibling.nodeValue == "")
                ASPx.RemoveElement(endBm.nextSibling);
        },
        SetHtml: function (html, addToHistory) {
            addToHistory = ASPx.IsExists(addToHistory) ? addToHistory : true;
            this.iframeWrapper.raiseExecuteCommand(ASPxClientCommandConsts.PASTEHTML_COMMAND, html, addToHistory);
        },
        SetHtmlInternal: function (html) {
            if(ASPx.ElementHasCssClass(this.selectedElement, ASPx.HtmlEditorClasses.SelectedContainerCssClasseName))
                ASPx.HtmlEditorClasses.Utils.removePlaceholderElement(this.selectedElement, this.iframeWrapper);
            html = ASPx.HtmlEditorClasses.HtmlProcessor.protectUrlsInHTML(html);

            var processingContainer = document.createElement("DIV");
            processingContainer.innerHTML = html;
            ASPx.HtmlEditorClasses.HtmlProcessor.processInnerHtml(processingContainer);
            html = processingContainer.innerHTML;

            if(this.selectedElement.tagName == "IMG") {
                if(!ASPx.Browser.WebKitFamily)
                    this.selectedElement.parentNode.removeChild(this.selectedElement);
                this.InsertHtml(html);
                this.iframeWrapper.selectionManager.saveSelection();
            }
            else {
                var elements = this.GetElements();
                if(ASPx.Browser.IE && this.selectedElement.tagName == "DIV" && elements.length == 1 && elements[0] == this.selectedElement) {
                    var doc = this.iframeWrapper.getDocument();
                    var wrapper = doc.createElement("DIV");
                    wrapper.innerHTML = html;

                    var selNode = doc.createElement("SPAN");
                    this.selectedElement.parentNode.insertBefore(selNode, this.selectedElement);

                    ASPx.RemoveElement(this.selectedElement);

                    for(var child = wrapper.firstChild; child; child = wrapper.firstChild)
                        selNode.parentNode.insertBefore(child, selNode);

                    setTimeout(function () {
                        this.clientSelection.SelectElement(selNode, false);
                        this.iframeWrapper.selectionManager.saveSelection();
                        ASPx.RemoveElement(selNode);
                    }.aspxBind(this), 0);
                    return;
                }
                else if(elements.length > 0) {
                    var replaceElement = elements[0];
                    // Remove elements
                    for(var i = 1, element = elements[i]; element; element = elements[++i]) {
                        if(this.IsElementCanDelete(element))
                            element.parentNode.removeChild(element);
                        else if(element.nodeType == 3) {
                            if(this.IsTableCell(element.parentNode) && element.parentNode.childNodes.length == 1)
                                element.parentNode.innerHTML = "&nbsp;";
                            else
                                element.nodeValue = "";
                        }
                        else if(element.nodeType == 1)
                            element.innerHTML = this.IsTableCell(element) ? "&nbsp;" : "";
                    }
                    // Replace first element
                    if(replaceElement.nodeType == 3) {
                        var selectNode = this.CreateNode("font", "&nbsp;");
                        replaceElement.parentNode.replaceChild(selectNode, replaceElement);
                    }
                    else {
                        var nonContentElementExpr = /^(IMG|BR|TABLE)$/i;
                        var needRemoveFirstChild = !nonContentElementExpr.test(replaceElement.tagName);
                        selectNode = needRemoveFirstChild ? this.ReplaceNodeContent(replaceElement) : replaceElement;
                    }
                    this.clientSelection.SelectElement(selectNode, true);
                }
                if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 11)
                    html += "<span id='selectNode'></span>";

                var array = html.match(/<(div|p|table|ol|ul|dl|h[1-6]|address|blockquote|center|pre)(?![^>]*(display\s*:\s*inline|data-aspx-elementcode))[^>]*/gi) || html.match(/<img[^>]*data-aspx-elementcode[^>]*/gi);
                var haveBlockElement = array && array.length > 0;
                var tampElementID = ASPx.HtmlEditorClasses.Selection.CreateUniqueID();
                this.InsertHtml(haveBlockElement ? "<span id='" + tampElementID + "'>&nbsp;</span>" : html, selectNode);

                var doc = this.iframeWrapper.getDocument();
                var parentBlockElement;
                var previousCloneBlockElement;
                if(haveBlockElement) {
                    var tampElement = doc.getElementById(tampElementID);
                    parentBlockElement = tampElement.parentNode;
                    var prevElement = tampElement;
                    while(!/body|div|td|li/gi.test(parentBlockElement.nodeName)) {
                        prevElement = parentBlockElement;
                        parentBlockElement = parentBlockElement.parentNode;
                    }
                    parentBlockElement = prevElement;

                    if(parentBlockElement != tampElement) {
                        var array = ASPx.HtmlEditorClasses.Utils.splitElementByChildNodeID(parentBlockElement, tampElementID);
                        previousCloneBlockElement = array[0];
                        parentBlockElement = array[1];
                    }
                    var tempDiv = doc.createElement("DIV");
                    tempDiv.innerHTML = html;
                    for(var i = 0, child; child = tempDiv.childNodes[i]; i++)
                        parentBlockElement.parentNode.insertBefore(child.cloneNode(true), parentBlockElement);
                    var startMarkElement = ASPx.HtmlEditorClasses.Selection.CreateElementWithUniqueID(doc);
                    var endMarkElement = ASPx.HtmlEditorClasses.Selection.CreateElementWithUniqueID(doc);
                    if(parentBlockElement != tampElement) {
                        parentBlockElement.insertBefore(startMarkElement, parentBlockElement.firstChild);
                        parentBlockElement.insertBefore(endMarkElement, parentBlockElement.firstChild);
                    }
                    else {
                        parentBlockElement.parentNode.insertBefore(startMarkElement, parentBlockElement);
                        parentBlockElement.parentNode.insertBefore(endMarkElement, parentBlockElement);
                    }
                    if(tampElement.parentNode)
                        tampElement.parentNode.removeChild(tampElement);
                    this.clientSelection.SelectExtendedBookmark({ "startMarkerID": endMarkElement.id, "endMarkerID": startMarkElement.id });
                }

                if(ASPx.Browser.IE) {
                    selectNode = doc.getElementById('selectNode');
                    if(selectNode) {
                        this.clientSelection.SelectElement(selectNode);
                        ASPx.RemoveElement(selectNode);
                    }
                    var fstNode = doc.getElementById('replaceBr');
                    if(fstNode)
                        ASPx.RemoveElement(fstNode);
                }
                else {
                    if(ASPx.IsExistsElement(selectNode) && selectNode.childNodes.length > 0)
                        ASPx.RemoveOuterTags(selectNode);
                    var fstNode = doc.getElementById('replaceBr');
                    if(fstNode)
                        ASPx.RemoveElement(fstNode);
                    if(needRemoveFirstChild && replaceElement) {
                        if(replaceElement.childNodes.length == 0)
                            ASPx.RemoveElement(replaceElement); // remove saved element on ReplaceNodeContent execute
                    }
                }
                if(parentBlockElement && parentBlockElement.parentNode && !ASPx.GetInnerText(parentBlockElement))
                    parentBlockElement.parentNode.removeChild(parentBlockElement);
                if(previousCloneBlockElement && !ASPx.GetInnerText(previousCloneBlockElement))
                    previousCloneBlockElement.parentNode.removeChild(previousCloneBlockElement);
                this.selectionManager.saveSelection();
            }
        },
        CreateNode: function (tagName, innerHtml) {
            if(!ASPx.Browser.Opera) {
                var node = this.iframeWrapper.getDocument().createElement(tagName);
                node.innerHTML = innerHtml;
            }
            else
                var node = ASPx.CreateHtmlElementFromString("<" + tagName + ">" + innerHtml + "</" + tagName + ">");
            return node;
        },
        IsElementCanDelete: function (element) {
            var nonDeleteElementExpr = /^(TR|TD|TH|THEAD|TBODY|TFOOT)$/i;
            return (element.nodeType == 3 && element.parentNode.tagName != "TD") ||
                   (element.nodeType == 1 && !nonDeleteElementExpr.test(element.tagName));
        },
        IsTableCell: function (element) {
            return element.tagName == "TD" || element.tagName == "TH";
        },
        ReplaceNodeContent: function (node) {
            if(node.nodeType != 1) return;
            var nodeContent = ASPx.Browser.IE ? "<br id='replaceBr' />&#xA;" : "<br id='replaceBr' />"; // for saving outer tag on PasteHtml execute
            node.innerHTML = nodeContent;
            var replaceNode = this.iframeWrapper.getDocument().createElement('font');
            replaceNode.innerHTML = "&nbsp";
            node.appendChild(replaceNode);
            return replaceNode;
        },
        InsertHtml: function (html, selectNode) {
            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 11)
                this.InsertHtmlIECore(html);
            else
                this.InsertHtmlCore(html, selectNode);

        },
        InsertHtmlIECore: function (html) {
            if(ASPx.Browser.MajorVersion > 8) { // B186293
                var obj = ASPx.HtmlEditorClasses.Commands.PasteHtml.safePreLineBreaks(html);
                html = obj.html.replace(/[\s]{2,}/g, " "); // remove twice and more spaces
                html = html.replace(/\s(<[^>]+>)\s/g, " $1"); // remove space after tag
                html = ASPx.HtmlEditorClasses.Commands.PasteHtml.restorePreLineBreaks(html, obj.safePreHtmlElements);
            }

            var doc = this.iframeWrapper.getDocument();
            if(doc.selection.type.toLowerCase() != "none")
                doc.selection.createRange().execCommand("Delete");
            if(doc.selection.type.toLowerCase() != "none") // hack: In IE, after above command is executed, <P>&nbsp;</P> appeares
                doc.execCommand("Delete");

            html = ASPx.HtmlEditorClasses.HtmlProcessor.protectUrlsInHTML(html);
            doc.body.setActive();
            var selRange = doc.selection.createRange();
            if(selRange && selRange.length) {
                var elem = selRange.item(0);
                if(elem && elem.tagName.toUpperCase() == "BODY") {
                    var formElement = elem.getElementsByTagName("FORM")[0];
                    if(formElement)
                        ASPx.SetInnerHtml(formElement, formElement.innerHTML + html);
                }
            }
            else {
                var rngStart = selRange.duplicate();
                rngStart.collapse(true);
                selRange.pasteHTML(html);
                rngStart.setEndPoint("StartToEnd", selRange);
                rngStart.select();
            }
            ASPx.HtmlEditorClasses.HtmlProcessor.restoreUrlsInDOM(this.iframeWrapper.getDocument());
            return true;
        },
        InsertHtmlCore: function (html, selectNode) {
            var doc = this.iframeWrapper.getDocument();
            var range = this.iframeWrapper.getWindow().getSelection().getRangeAt(0);

            if(selectNode)
                selectNode.parentNode.removeChild(selectNode);
            html += "<span id='sbm'></span> <span id='ebm'></span>";
            var bookmark = { "startMarkerID": "sbm", "endMarkerID": "ebm" };

            var tmpNode = doc.createElement("SPAN");
            ASPx.SetInnerHtml(tmpNode, html);
            if(this.selectedElement.tagName == "IMG")
                range.deleteContents();
            range.insertNode(tmpNode);
            this.selectedElement.normalize();
            var length = tmpNode.childNodes.length;
            for(var i = 0; i < length; i++)
                tmpNode.parentNode.insertBefore(tmpNode.childNodes[0], tmpNode);
            tmpNode.parentNode.removeChild(tmpNode);

            var startMarkerNode = doc.getElementById(bookmark.startMarkerID);
            var endMarkerNode = doc.getElementById(bookmark.endMarkerID);

            var previousSibling = startMarkerNode.previousSibling;
            if(!ASPx.Browser.IE && !endMarkerNode.nextSibling && previousSibling && previousSibling.nodeName != "#text" && previousSibling.nodeName != "IMG" && previousSibling.offsetTop != startMarkerNode.offsetTop) {
                var textNode = document.createTextNode("\xA0");
                startMarkerNode.parentNode.insertBefore(textNode, startMarkerNode);
            }

            var selection = this.selectionManager.createSelection();
            selection.clientSelection.SelectExtendedBookmark(bookmark);
            selection.clientSelection.RemoveExtendedBookmark(bookmark);

            var range = this.iframeWrapper.getWindow().getSelection().getRangeAt(0);
            range.deleteContents();

            this.iframeWrapper.focus();
            return true;
        },
        SetFocusToDocumentStart: function () {
            var doc = this.iframeWrapper.getDocument();

            var sbm = document.createElement("SPAN");
            var ebm = document.createElement("SPAN");
            sbm.id = "sbm";
            ebm.id = "ebm";
            doc.body.insertBefore(sbm, doc.body.firstChild)
            doc.body.insertBefore(ebm, sbm)

            var bookmark = { "startMarkerID": "sbm", "endMarkerID": "ebm" };
            this.selectionManager.createSelection().clientSelection.SelectExtendedBookmark(bookmark);
        },
        getSpecialSelection: function() {
            return this.clientSelection.specialSelection;
        }
    });
    ASPx.HtmlEditorClasses.Utils.ElementsSelectionHelper = {
        // New realization of GetEditorSelectionInfo()
        GetSelectedElements: function (doc, startBm, endBm, removeEmptyElements) {
            var elements = [];

            startBm = typeof (startBm) == "string" ? ASPx.GetElementByIdInDocument(doc, startBm) : startBm;
            endBm = typeof (endBm) == "string" ? ASPx.GetElementByIdInDocument(doc, endBm) : endBm;

            var chain1 = this.GetAncestorsChain(startBm);
            var chain2 = this.GetAncestorsChain(endBm);
            var commonAncestor = this.FindCommonAncestor(chain1, chain2);
            if(this.IsNeedExchangeBmPlace(chain1, chain2)) {
                var tmp = chain1;
                chain1 = chain2;
                chain2 = tmp;

                tmp = startBm;
                startBm = endBm;
                endBm = tmp;
            }

            var curAncestor = startBm.parentNode;
            var curElement = startBm;

            // Process left tree
            while(curAncestor != commonAncestor) {
                while(curElement.nextSibling) {
                    elements.push(curElement.nextSibling);
                    curElement = curElement.nextSibling;
                }
                curElement = curAncestor;
                curAncestor = curAncestor.parentNode;
            }

            // Process nodes between trees
            var rightLeftTreeNode = chain2[ASPx.Data.ArrayIndexOf(chain2, commonAncestor) + 1];
            rightLeftTreeNode = rightLeftTreeNode ? rightLeftTreeNode : endBm;
            while(this.GetNextSibling(curElement) != rightLeftTreeNode) {
                elements.push(this.GetNextSibling(curElement));
                curElement = this.GetNextSibling(curElement);
            }

            // Process right tree
            curAncestor = endBm.parentNode;
            curElement = endBm;
            var rightElements = [];
            var saveAllRightTree = true;
            while(curAncestor != commonAncestor) {
                if(saveAllRightTree && this.GetNextSibling(curElement) == null && curAncestor.parentNode == commonAncestor)
                    rightElements.push(curAncestor);
                else {
                    saveAllRightTree = false;
                    while(curElement.previousSibling) {
                        rightElements.push(curElement.previousSibling);
                        curElement = curElement.previousSibling;
                    }
                }
                curElement = curAncestor;
                curAncestor = curAncestor.parentNode;
            }
            elements = elements.concat(rightElements.reverse());

            // Process outer nodes (exclude bookmarks, round child nodes to parent nodes)
            if(elements.length > 0)
                elements[0] = this.PrepareOuterSelectNode(elements[0], startBm, endBm);
            if(elements.length > 1)
                elements[elements.length - 1] = this.PrepareOuterSelectNode(elements[elements.length - 1], endBm, startBm);
            elements = this.MergeTextNodes(elements);
            if(elements.length == 1 && elements[0].tagName == "BODY")
                elements = ASPx.Data.CollectionToArray(elements[0].childNodes);
            if(removeEmptyElements)
                this.RemoveEmptyElements(elements);
            return elements;
        },
        MergeTextNodes: function (elements) {
            for(var i = 0; el = elements[i]; i++) {
                var nextEl = elements[i + 1];
                if(el.nodeType == 3 && nextEl && nextEl.nodeType == 3 && nextEl.parentNode == el.parentNode) {
                    el.nodeValue += nextEl.nodeValue;
                    elements.splice(i + 1, 1);
                    ASPx.RemoveElement(nextEl);
                    elements[i] = this.FindParentNode(el);
                    i++;
                }
            }
            return elements;
        },
        RemoveEmptyElements: function (elements) {
            for(var i = 0, element; element = elements[i]; i++) {
                if(element.nodeType == 3 && (!element.nodeValue || (element.length == 1 && (element.nodeValue == "\n" || element.nodeValue == "\r")))) {
                    elements.splice(i, 1);
                    i--;
                }
            }
        },
        FindParentNode: function (el) {
            if(el.parentNode.childNodes.length == 1 && el.parentNode.tagName != "BODY")
                return this.FindParentNode(el.parentNode);
            return el;
        },
        FindCommonAncestor: function (chain1, chain2) {
            for(var i = 0; i < Math.max(chain1.length, chain2.length) ; i++) {
                if(chain1[i] != chain2[i])
                    return chain1[i - 1];
            }
            return chain1[chain1.length - 1];
        },
        GetAncestorsChain: function (element) {
            var parent = element.parentNode;
            var chain = [];
            while(parent.tagName != "BODY") {
                chain.push(parent);
                parent = parent.parentNode;
            }
            chain.push(parent);
            var result = chain.reverse();
            result.push(element);
            return result;
        },
        PrepareOuterSelectNode: function (node, bmElement1, bmElement2) {
            var parent = node.parentNode;
            var parentChildCount = parent.childNodes.length;
            if(parent.tagName != "BODY") {
                var canMoveToParent =
                    parentChildCount == 1 ||
                    (parentChildCount == 2 && (bmElement1.parentNode == parent || bmElement2.parentNode == parent)) ||
                    (parentChildCount == 3 && bmElement1.parentNode == parent && bmElement2.parentNode == parent);
                if(canMoveToParent)
                    return this.PrepareOuterSelectNode(parent, bmElement1, bmElement2);
            }
            return node;
        },
        IsNeedExchangeBmPlace: function (chain1, chain2) {
            var chainLength = Math.max(chain1.length, chain2.length);
            for(var i = 0; i < chainLength; i++) {
                if(i > 0 && chain1[i] != chain2[i])
                    return ASPx.Data.ArrayIndexOf(chain1[i - 1].childNodes, chain1[i]) > ASPx.Data.ArrayIndexOf(chain1[i - 1].childNodes, chain2[i]);
            }
            return false;
        },
        GetNextSibling: function (node) {
            if(!ASPx.Browser.IE || node.nextSibling)
                return node.nextSibling;
            var parent = node.parentNode;
            if(parent) {
                var index = ASPx.Data.ArrayIndexOf(parent.childNodes, node);
                if(index < parent.childNodes.length - 1)
                    return parent.childNodes[index + 1];
            }
            return null;
        }
    };

    ASPxClientHtmlEditorSelection.Create = function (window) {
        return ASPx.HtmlEditorClasses.Selection.Create(window);
    }
    ASPxClientHtmlEditorSelection.SelectElement = function (element, iframeWindow, selectAllContent) {
        if(!ASPx.IsExists(selectAllContent))
            selectAllContent = true;
        return ASPx.Browser.IE && ASPx.Browser.MajorVersion < 11 ? ASPx.HtmlEditorClasses.SelectionIE.SelectElementInDocument(element, iframeWindow.document, selectAllContent) :
                            ASPx.HtmlEditorClasses.SelectionNSOpera.SelectElementInDocument(element, iframeWindow.document, iframeWindow, selectAllContent);
    }

    // internal
    ASPx.HtmlEditorClasses.Selection = ASPx.CreateClass(null, {
        constructor: function (contentWindow) {
            this.contentWindow = contentWindow;
            this.contentDocument = contentWindow.document;

            this.specialSelection = new ASPx.HtmlEditorClasses.SpecialSelection(this);

            this.startContainer = null;
            this.endContainer = null;
            this.startOffset = 0;
            this.endOffset = 0;
        },
        canUseMarkers: function(parentElement) {
            return parentElement && 
                (parentElement.tagName == "INPUT" || 
                 ASPx.ElementHasCssClass(parentElement, ASPx.HtmlEditorClasses.SelectedContainerCssClasseName) || 
                 parentElement.childNodes.length == 1 && ASPx.ElementHasCssClass(parentElement.firstChild, ASPx.HtmlEditorClasses.PlaceholderCssClasseName));    
        },
        Save: function () {
        },
        Restore: function () {
        },

        GetText: function () {
        },
        GetHtmlText: function () {
        },
        GetStartContainer: function () {
            this.FindStartEndContainerInternal();
            return this.startContainer;
        },
        GetEndContainer: function () {
            this.FindStartEndContainerInternal();
            return this.endContainer;
        },
        GetStartOffset: function () {
            this.FindStartEndContainerInternal();
            return this.startOffset;
        },
        GetEndOffset: function () {
            this.FindStartEndContainerInternal();
            return this.endOffset;
        },
        IsControl: function () {
            return false;
        },
        IsTextOnly: function () {
            return false;
        },
        // ********* Utils ***********
        GetRange: function () {
        },
        GetExtendedBookmark: function () {
            return null;
        },
        GetSelectionMarkerElement: function (isStart) {
            return null;
        },
        SelectExtendedBookmark: function (bookmark) {
            var bmElements = ASPx.HtmlEditorClasses.Selection.GetBookmarkElements(bookmark, this.contentDocument);
            if(!bmElements.startMarker || !bmElements.endMarker)
                return;
            var getSpecialElement = function(startMarker, endMarker) {
                return startMarker.nextSibling && startMarker.nextSibling.className && ASPx.HtmlEditorClasses.Utils.needToUseSpecialSelection(startMarker.nextSibling) && startMarker.nextSibling === endMarker.previousSibling ? startMarker.nextSibling : null;
            }
            var element = getSpecialElement(bmElements.startMarker, bmElements.endMarker) || getSpecialElement(bmElements.endMarker, bmElements.startMarker);
            if(element) {
                ASPx.RemoveElement(bmElements.startMarker);
                ASPx.RemoveElement(bmElements.endMarker);
                this.applySpecialSelectionToElement(element);
                return true;
            }
            var needSelectChildNodes = function(startMarker, endMarker) {
                return startMarker.nextSibling && startMarker.nextSibling.childNodes.length > 0 && startMarker.nextSibling === endMarker.previousSibling && !ASPx.HtmlEditorClasses.Utils.needToUseSpecialSelection(startMarker.nextSibling);
            }
            var moveToChildElements = function(startMarker, endMarker) {
                startMarker.nextSibling.insertBefore(startMarker, startMarker.nextSibling.firstChild);
                endMarker.previousSibling.appendChild(endMarker);
            }
            if(needSelectChildNodes(bmElements.startMarker, bmElements.endMarker))
                moveToChildElements(bmElements.startMarker, bmElements.endMarker);
            else if(needSelectChildNodes(bmElements.endMarker, bmElements.startMarker))
                moveToChildElements(bmElements.endMarker, bmElements.startMarker);
            return false;
        },
        RemoveExtendedBookmark: function (bookmark) {
            if(bookmark) {
                var bmElements = ASPx.HtmlEditorClasses.Selection.GetBookmarkElements(bookmark, this.contentDocument);
                ASPx.RemoveElement(bmElements.startMarker);
                ASPx.RemoveElement(bmElements.endMarker);
            }
            return null;
        },

        FindStartEndContainer: function () {
        },
        FindStartEndContainerInternal: function () {
            if(this.startContainer || this.endContainer)
                return;
            this.FindStartEndContainer();
        },
        GetParentElement: function () {
            return this.specialSelection.getSelectedElement();
        },
        applySpecialSelectionToElement: function (element, useBrowserSelection) {
            return this.specialSelection.applySpecialSelectionToElement(element, useBrowserSelection);
        }
    });
    ASPx.HtmlEditorClasses.SpecialSelection = ASPx.CreateClass(null, {
        constructor: function(clientSelection) {
            this.clientSelection = clientSelection;
            this.selectedElement = null;
        },
        getBody: function() {
            return this.clientSelection.contentDocument.body;
        },
        getSelectedElement: function() {
            if(this.needToUpdateCachedElement())
                this.updateCachedElement();
            return this.selectedElement;
        },
        setSelectedElement: function(value) {
            return this.selectedElement = value;
        },
        needToUpdateCachedElement: function() {
            var hiddenElement = this.clientSelection.contentDocument.getElementById(ASPx.HtmlEditorClasses.SelectedHiddenContainerID);
            if(!this.selectedElement && !hiddenElement || !hiddenElement && this.selectedElement) {
                if(!hiddenElement && this.selectedElement)
                    this.selectedElement = null;
                return false;
            }
            return !this.selectedElement || !ASPx.HtmlEditorClasses.Utils.getBodyByElement(this.selectedElement);
        },
        updateCachedElement: function() {
            var predicate = function(element) { return !!ASPx.Attr.GetAttribute(element, ASPx.HtmlEditorClasses.SelectedFormElementAttrName) || ASPx.ElementHasCssClass(element, ASPx.HtmlEditorClasses.SelectedContainerCssClasseName); };
            var childs = ASPx.HtmlEditorClasses.Utils.getChildsByPredicate(this.getBody(), predicate);
            if(childs.length > 0)
                this.setSelectedElement(childs[0]);
        },
        applySpecialSelectionToElement: function (element, useBrowserSelection) {
            if(element.nodeType == 1) {
                var compare = function(el) { return ASPx.ElementHasCssClass(el, ASPx.HtmlEditorClasses.PlaceholderCssClasseName); };
                var parentElement = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParent(element, compare);
                element = parentElement ? parentElement : element;
            }
            var doc = element.ownerDocument;
            var startSpan = ASPx.HtmlEditorClasses.Selection.CreateElementWithUniqueID(doc);
            var endSpan = ASPx.HtmlEditorClasses.Selection.CreateElementWithUniqueID(doc);
            if(!useBrowserSelection) {
                var useSpecialSelection = ASPx.HtmlEditorClasses.Utils.needToUseSpecialSelection(element);
                var hiddenElement = doc.getElementById(ASPx.HtmlEditorClasses.SelectedHiddenContainerID);
                if(hiddenElement) {
                    if(this.selectedElement != element || !useSpecialSelection)
                        this.clearClassNameAttr(this.selectedElement);
                }
                if(useSpecialSelection) {
                    this.addClassName(element);
                    if(!hiddenElement) {
                        hiddenElement = ASPx.HtmlEditorClasses.Utils.createHiddenContainer(doc, "DIV", ASPx.HtmlEditorClasses.SelectedHiddenContainerID, "&nbsp;");
                        doc.body.appendChild(hiddenElement);
                    }
                    this.setSelectedElement(element);
                    hiddenElement.insertBefore(startSpan, hiddenElement.firstChild);
                    hiddenElement.appendChild(endSpan);
                    ASPx.HtmlEditorClasses.Utils.UnforcedFunctionCall(function() { }, "PreventLostFocus", 200, true);
                }
                else { 
                    if(hiddenElement)
                        this.removeHiddenContainer();
                    element.parentNode.insertBefore(startSpan, element);
                    ASPx.InsertElementAfter(endSpan, element);
                }
            }
            else {
                element.insertBefore(startSpan, element.firstChild);
                element.appendChild(endSpan);
            }
            return this.clientSelection.SelectExtendedBookmark({ "startMarkerID": startSpan.id, "endMarkerID": endSpan.id});
        },
        removeHiddenContainer: function() {
            var element = this.clientSelection.contentDocument.getElementById(ASPx.HtmlEditorClasses.SelectedHiddenContainerID);
            if(element) {
                var selectedElement = this.getSelectedElement();
                if(selectedElement)
                    this.clearClassNameAttr(selectedElement);
                element.parentNode.removeChild(element);
            }
        },
        clearClassNameAttr: function(element) {
            ASPx.RemoveClassNameFromElement(element, ASPx.HtmlEditorClasses.SelectedContainerCssClasseName);
            ASPx.Attr.RemoveAttribute(element, ASPx.HtmlEditorClasses.SelectedFormElementAttrName);
        },
        addClassName: function(element) {
            if(/form|input|textarea|select|option|button/gi.test(element.nodeName))
                ASPx.Attr.SetAttribute(element, ASPx.HtmlEditorClasses.SelectedFormElementAttrName, true);
            else
                ASPx.AddClassNameToElement(element, ASPx.HtmlEditorClasses.SelectedContainerCssClasseName);
        },
        restoreSpecialSelection: function() {
            var selectedElement = this.getSelectedElement();
            this.applySpecialSelectionToElement(selectedElement);
        }
    });

    ASPx.HtmlEditorClasses.Selection.IsHtml = function (text) {
        if(!text || !text.match) return text;
        return text.match(/</);
    }
    ASPx.HtmlEditorClasses.Selection.Create = function (contentWindow) {
        if(ASPx.Browser.Edge)
            return new ASPx.HtmlEditorClasses.SelectionEdge(contentWindow);
        else if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 11)
            return new ASPx.HtmlEditorClasses.SelectionIE(contentWindow);
        else
            return new ASPx.HtmlEditorClasses.SelectionNSOpera(contentWindow);
    }
    ASPx.HtmlEditorClasses.Selection.UniqCounter = 0;
    ASPx.HtmlEditorClasses.Selection.CreateUniqueID = function () {
        ASPx.HtmlEditorClasses.Selection.UniqCounter++;
        return 'dx_temp_' + (new Date()).valueOf() + '_' + ASPx.HtmlEditorClasses.Selection.UniqCounter;
    }
    ASPx.HtmlEditorClasses.Selection.CreateElementWithUniqueID = function (doc) {
        var element = doc.createElement("span");
        element.id = ASPx.HtmlEditorClasses.Selection.CreateUniqueID();
        return element;
    }
    ASPx.HtmlEditorClasses.Selection.IsControlElement = function (element) {
        if(element.nodeType == 1) {
            var tagName = element.tagName.toUpperCase();
            return tagName == "TABLE" || tagName == "IMG";
        }
        return false;
    };
    ASPx.HtmlEditorClasses.Selection.GetBookmarkElements = function (bookmark, document) {
        return {
            startMarker: ASPx.GetElementByIdInDocument(document, bookmark.startMarkerID),
            endMarker: ASPx.GetElementByIdInDocument(document, bookmark.endMarkerID)
        };
    };
    ASPx.HtmlEditorClasses.Selection.getBookmarkByElements = function(doc, firstElement, lastElement) {
        var startMarkElement = ASPx.HtmlEditorClasses.Selection.CreateElementWithUniqueID(doc);
        var endMarkElement = ASPx.HtmlEditorClasses.Selection.CreateElementWithUniqueID(doc);
        firstElement.parentNode.insertBefore(startMarkElement, firstElement);
        ASPx.InsertElementAfter(endMarkElement, lastElement);
        return { "startMarkerID": startMarkElement.id, "endMarkerID": endMarkElement.id };
    };

    // TODO Refactor - see similar function in commands.js
    ASPx.HtmlEditorClasses.Selection.InsertNodeAtPosition = function (containerNode, insertNode, containeNodeOffset, doc) {
        var newTextNode = null;
        if((containerNode.nodeType == 3) && (insertNode.nodeType == 3))
            containerNode.insertData(containeNodeOffset, insertNode.nodeValue);
        else {
            var afterNode;
            if(containerNode.nodeType == 3) {
                var textNode = containerNode;
                containerNode = textNode.parentNode;
                var nodeText = textNode.nodeValue;

                var textBefore = nodeText.substr(0, containeNodeOffset);
                var textAfter = nodeText.substr(containeNodeOffset);

                var beforeNode = doc.createTextNode(textBefore);
                afterNode = doc.createTextNode(textAfter);

                containerNode.insertBefore(afterNode, textNode);
                containerNode.insertBefore(insertNode, afterNode);

                newTextNode = beforeNode;

                try {
                    containerNode.insertBefore(beforeNode, insertNode);
                }
                catch (exc) { }
                containerNode.removeChild(textNode);
            }
            else {
                if(containerNode.childNodes.length > 0) {
                    afterNode = containerNode.childNodes[containeNodeOffset];
                    containerNode.insertBefore(insertNode, afterNode);
                }
                else {
                    if(containerNode.tagName != "BODY")
                        containerNode = containerNode.parentNode;
                    containerNode.appendChild(insertNode);
                }
            }
            return newTextNode;
        }
    }

    ASPx.HtmlEditorClasses.SelectionIE = ASPx.CreateClass(ASPx.HtmlEditorClasses.Selection, {
        constructor: function (contentWindow) {
            this.constructor.prototype.constructor.call(this, contentWindow);
        },
        Save: function () {
            var curSelection = this.contentDocument.selection;
            var selRange = curSelection.createRange();
            if(selRange.length)
                this.sourceIndex = selRange.item(0).sourceIndex;
            else {
                this.startBookmark = this.GetBookmark(selRange);
                this.selectedRange = selRange;
                this.isCollapsed = selRange.boundingWidth == 0;
            }
        },
        Restore: function () {
            var useBookmark = false;
            var selRange;
            if(this.selectedRange)
                useBookmark = this.GetHtmlText() != this.selectedRange.htmlText;

            if(ASPx.IsExists(this.sourceIndex))
                this.selectElementBySourceIndex(this.sourceIndex);
            else if(ASPx.IsExists(this.selectedRange)) {
                if(useBookmark) {
                    selRange = this.contentDocument.body.createTextRange();
                    selRange.moveToBookmark(this.startBookmark);
                    selRange.select();
                    selRange.collapse();
                } else {
                    try {
                        if(this.isCollapsed)
                            this.selectedRange.collapse();
                        this.selectedRange.select();
                    }
                    catch (e) { }
                }
            }
        },
        GetBookmark: function (range) {
            return range.getBookmark(); // faster than GetExtendedBookmark
        },
        GetSelectionMarkerElement: function (isStart) {
            var rng = this.GetRange();
            var markerID = ASPx.HtmlEditorClasses.Selection.CreateUniqueID();
            var makerElem = null;

            if(this.IsControl()) {
                makerElem = this.contentDocument.createElement("span");
                makerElem.id = markerID;

                var controlElem = rng.item(0);
                if(isStart)
                    controlElem.parentNode.insertBefore(makerElem, controlElem);
                else
                    controlElem.parentNode.insertBefore(makerElem, controlElem.nextSibling);
            }
            else {
                if(rng.parentElement) {
                    var document = rng.parentElement().document || rng.parentElement().ownerDocument;
                    if(document != this.contentDocument)
                        return null;
                }

                if(rng.collapse)
                    rng.collapse(isStart);

                // Paste a marker element at the collapsed range and get it from the DOM.
                try {
                    rng.pasteHTML('<span id="' + markerID + '"></span>');
                    makerElem = ASPx.GetElementByIdInDocument(this.contentDocument, markerID);
                } catch (e) { };
            }
            return makerElem;
        },
        GetExtendedBookmark: function () {
            var parentElement = this.GetParentElement();
            if(this.canUseMarkers(parentElement)) {
                var startMarker = ASPx.HtmlEditorClasses.Selection.CreateElementWithUniqueID(this.contentDocument);
                var endMarker = ASPx.HtmlEditorClasses.Selection.CreateElementWithUniqueID(this.contentDocument);
                parentElement.parentNode.insertBefore(startMarker, parentElement);
                ASPx.InsertElementAfter(endMarker, parentElement);
                return { "startMarkerID": startMarker.id, "endMarkerID": endMarker.id };
            }
            var controlElementID = null;
            var bookmark = null;

            if(this.IsControl()) {
                var selRange = this.contentDocument.selection.createRange();
                controlElementID = ASPx.HtmlEditorClasses.Selection.CreateUniqueID();
                ASPx.Attr.ChangeAttribute(selRange.item(0), "id", controlElementID);

                bookmark = { "controlElementID": controlElementID };
            }
            else {
                var startMarker = this.GetSelectionMarkerElement(true);
                var endMarker = this.GetSelectionMarkerElement(false);
                if(startMarker != null && endMarker != null)
                    bookmark = { "startMarkerID": startMarker.id, "endMarkerID": endMarker.id };
            }
            return bookmark;
        },
        SelectExtendedBookmark: function (bookmark) {
            if(ASPx.HtmlEditorClasses.Selection.prototype.SelectExtendedBookmark.call(this, bookmark))
                return;
            var bmElements = ASPx.HtmlEditorClasses.Selection.GetBookmarkElements(bookmark, this.contentDocument);
            var rng1 = null;
            if(bookmark.controlElementID) {
                rng1 = this.contentDocument.body.createControlRange();
                var controlElement = ASPx.GetElementByIdInDocument(this.contentDocument, bookmark.controlElementID);
                ASPx.Attr.RestoreAttribute(controlElement, "id");
                rng1.addElement(controlElement);
            }
            else {
                rng1 = this.contentDocument.body.createTextRange();
                if(ASPx.IsExists(bmElements.startMarker))
                    rng1.moveToElementText(bmElements.startMarker);
                rng1.moveStart('character', 0);

                var rng2 = this.contentDocument.body.createTextRange();
                if(ASPx.IsExists(bmElements.endMarker))
                    rng2.moveToElementText(bmElements.endMarker);

                rng1.setEndPoint('EndToEnd', rng2);
                rng1.moveEnd('character', 0);

                ASPx.RemoveElement(bmElements.startMarker);
                ASPx.RemoveElement(bmElements.endMarker);
            }

            rng1.select();
        },
        SelectElement: function (elem, selectAllContent) {
            ASPx.HtmlEditorClasses.SelectionIE.SelectElementInDocument(elem, this.contentDocument, selectAllContent);
        },
        GetRange: function () {
            return this.contentDocument.selection.createRange();
        },
        GetParentElement: function () {
            var element = ASPx.HtmlEditorClasses.Selection.prototype.GetParentElement.call(this);
            if(element)
                return element;
            var ret = null;
            var rng = this.GetRange();
            if(!rng) return ret;

            if(rng.length)
                ret = rng.item(0);
            else if(rng.parentElement)
                ret = rng.parentElement();
            return ret;
        },
        GetText: function () {
            var ret = "";
            var rng = this.contentDocument.selection.createRange();
            if(ASPx.IsExists(rng.text))
                ret = rng.text;
            return ret;
        },
        GetHtmlText: function () {
            var ret = "";
            var rng = this.GetRange();
            if(rng.length)
                ret = rng.item(0).outerHTML;
            else if(rng.htmlText)
                ret = rng.htmlText;
            return ret;
        },
        IsControl: function () {
            return this.contentDocument.selection.type == 'Control';
        },
        IsTextOnly: function () {
            var rng = this.contentDocument.selection.createRange();
            if(ASPx.IsExists(rng.text) && ASPx.IsExists(rng.htmlText))
                return (rng.text == rng.htmlText);
            return false;
        },
        FindStartEndContainer: function () {
            var startMarker = this.GetSelectionMarkerElement(true);
            var endMarker = this.GetSelectionMarkerElement(false);

            if(!startMarker && !endMarker) {
                this.startContainer = this.contentDocument.body;
                return;
            }
            this.startContainer = startMarker.parentNode;
            this.startOffset = ASPx.Data.ArrayIndexOf(this.startContainer.childNodes, startMarker);
            ASPx.RemoveElement(startMarker);

            this.endContainer = endMarker.parentNode;
            this.endOffset = ASPx.Data.ArrayIndexOf(this.endContainer.childNodes, endMarker);
            ASPx.RemoveElement(endMarker);

            // correction
            if(this.startContainer != this.endContainer) {
                if(this.endOffset == 0) {
                    while(this.endContainer && !this.endContainer.previousSibling)
                        this.endContainer = this.endContainer.parentNode;

                    if(this.endContainer)
                        this.endContainer = this.endContainer.previousSibling;
                }
                else if(this.endContainer.nodeType == 1)
                    this.endContainer = this.endContainer.childNodes[this.endOffset - 1];
            }
        },
        selectElementBySourceIndex: function(sourceIndex) {
            var selRange = this.contentDocument.body.createControlRange();
            if(this.sourceIndex >= this.contentDocument.all.length) {
                for(var i = 0, el; el = this.contentDocument.all[i]; i++) {
                    if(ASPx.IsExists(el.sourceIndex) && el.sourceIndex == sourceIndex) {
                        selRange.addElement(el);
                        break;
                    }
                }
            }
            else
                selRange.addElement(this.contentDocument.all(sourceIndex));
            selRange.select();
        }
    });
    ASPx.HtmlEditorClasses.SelectionIE.SelectElementInDocument = function (elem, docObj, selectAllContent) {
        if(!ASPx.IsExists(selectAllContent))
            selectAllContent = true;

        var rng;
        if(ASPx.HtmlEditorClasses.Selection.IsControlElement(elem)) {
            rng = docObj.body.createControlRange();
            rng.addElement(elem);
        }
        else {
            try { // B183821
                rng = docObj.selection.createRange();
            }
            catch (e) {
                window.focus();
                docObj.parentWindow.focus();
                rng = docObj.selection.createRange();
            }
            try { // B39184
                rng.moveToElementText(elem);
            }
            catch (e) { }
        }

        if(!selectAllContent) {
            try { // B39184
                rng.collapse(false);
            }
            catch (e) { }
        }

        rng.select();
    };

    ASPx.HtmlEditorClasses.DialogSelectionIE = ASPx.CreateClass(ASPx.HtmlEditorClasses.SelectionIE, {
        Save: function () {
            if(!this.IsControl())
                this.contentWindow.focus();
            var curSelection = this.contentDocument.selection;
            if(curSelection) {
                this.selectionRange = curSelection.createRange();
                if(curSelection.type.toLowerCase() == "control")
                    this.sourceIndex = this.selectionRange.item(0).sourceIndex;
            }
        },
        Restore: function () {
            try {
                if(ASPx.IsExists(this.sourceIndex))
                    this.selectElementBySourceIndex(this.sourceIndex);
                else if(this.selectionRange)
                    this.selectionRange.select();

                if(!this.IsControl())
                    this.contentWindow.focus();
            } catch (e) { }
        }
    });

    ASPx.HtmlEditorClasses.SelectionNSOpera = ASPx.CreateClass(ASPx.HtmlEditorClasses.Selection, {
        constructor: function (contentWindow) {
            this.constructor.prototype.constructor.call(this, contentWindow);
        },
        Save: function () {
            var curSelection = this.contentWindow.getSelection();
            if(curSelection) {
                this.isCollapsedSelection = curSelection.isCollapsed;
                this.startNodePath = this.GetNodePath(this.contentDocument.documentElement, curSelection.anchorNode);
                this.startNodeOffset = curSelection.anchorOffset;

                this.endNodePath = this.GetNodePath(this.contentDocument.documentElement, curSelection.focusNode);
                this.endNodeOffset = curSelection.focusOffset;
                if(ASPx.Browser.Opera && this.NeedExchangeStartAndEndNode(this.startNodePath.path, this.endNodePath.path, this.startNodeOffset, this.endNodeOffset)) {
                    var tmp = this.startNodePath;
                    this.startNodePath = this.endNodePath;
                    this.endNodePath = tmp;

                    tmp = this.startNodeOffset;
                    this.startNodeOffset = this.endNodeOffset;
                    this.endNodeOffset = tmp;
                }
            }
            else {
                this.isCollapsedSelection = null;
                this.startNodePath = null;
                this.startNodeOffset = null;
                this.endNodePath = null;
                this.endNodeOffset = null;
            }
        },
        Restore: function () {
            try {
                var curSelection = this.contentWindow.getSelection();
                var startNode = this.GetNodeByPath(this.contentDocument.documentElement, this.startNodePath, true);
                var endNode = this.GetNodeByPath(this.contentDocument.documentElement, this.endNodePath, false);
                var isSelectionEqual = (curSelection.anchorNode == startNode) && (curSelection.focusNode == endNode) &&
                                     (curSelection.anchorOffset == this.startNodeOffset) && (curSelection.focusOffset == this.endNodeOffset) &&
                                     (curSelection.isCollapsed == this.isCollapsedSelection);
                if(!isSelectionEqual) { //B210043
                    ASPx.HtmlEditorClasses.SelectionNSOpera.RestoreByStartAndEndNodeCore(startNode, endNode, this.startNodeOffset,
                                                                        this.endNodeOffset,
                                                                        this.isCollapsedSelection,
                                                                        this.contentDocument,
                                                                        this.contentWindow);
                }
            } catch (e) { }
        },
        GetParentElement: function () {
            var element = ASPx.HtmlEditorClasses.Selection.prototype.GetParentElement.call(this);
            if(element)
                return element;
            var ret = null;
            var rng = this.GetRange();
            if(!rng) return ret;

            var selection = this.contentWindow.getSelection();

            var startContainer = this.GetStartContainer(rng, selection);
            var endContainer = this.GetEndContainer(rng, selection);
            var startOffset = this.GetStartOffset(rng, selection);
            var endOffset = this.GetEndOffset(rng, selection);

            if(this.IsControl())
                ret = selection.anchorNode.nodeType == 3 && ASPx.Browser.IE ? startContainer.nextSibling : selection.anchorNode.childNodes[selection.anchorOffset];
            else {
                if(!rng.commonAncestorContainer.tagName) {
                    if(ASPx.Browser.IE && ASPx.Browser.MajorVersion >= 11 && this.contentWindow.document == rng.commonAncestorContainer &&
                            this.contentWindow.document == rng.startContainer && rng.startContainer == rng.endContainer)
                        return this.contentWindow.document.body;

                    if(this.contentWindow.document == rng.commonAncestorContainer &&
                        selection.baseNode) //SAFARI
                        ret = selection.baseNode.parentNode;
                    else
                        ret = rng.commonAncestorContainer.parentNode;
                }
                else {
                    if(ASPx.Browser.WebKitFamily) {
                        if((rng.startContainer.nodeType == 3 && rng.startOffset != rng.startContainer.length) || (rng.endContainer.nodeType == 3 && rng.endOffset > 0))
                            return rng.commonAncestorContainer;
                        var getSiblingElement = function (container) { return container.nextElementSibling; };
                        var startImageElement = this.GetSelectedImageElement(rng, rng.startContainer, getSiblingElement, rng.startOffset);
                        if(startImageElement && startImageElement.nodeName == "IMG") {
                            getSiblingElement = function (element) { return element.previousElementSibling; };
                            var endImageElement = this.GetSelectedImageElement(rng, rng.endContainer, getSiblingElement, rng.endOffset);
                            if(startImageElement === endImageElement)
                                return startImageElement;
                        }
                    }
                    ret = rng.commonAncestorContainer;
                }
            }
            return ret;
        },
        GetSelectedImageElement: function (rng, container, getSiblingElement, offsetValue) {
            var isImageElement = function (element) { return element && element.nodeName == "IMG" && (element.style["float"] || ASPx.GetCurrentStyle(element)["float"]); };
            var getImageOrNotEmptySiblingElement = function (container) {
                for(var element = container; element; element = getSiblingElement(element)) {
                    if(isImageElement(element) || element.childNodes.length > 0)
                        return element;
                }
            };
            if(container.childNodes.length > 0 && isImageElement(container.childNodes[offsetValue]))
                return container.childNodes[offsetValue];
            var result = getImageOrNotEmptySiblingElement(getSiblingElement(container));
            if(isImageElement(result))
                return result;
            for(var parent = container.parentNode; parent && parent.nodeName != "BODY"; parent = parent.parentNode) {
                result = getImageOrNotEmptySiblingElement(getSiblingElement(parent));
                if(isImageElement(result))
                    return result;
                else if(result)
                    break;
            }
            for(var child = getSiblingElement(container) ; child; child = child.firstChild) {
                if(isImageElement(child))
                    return child;
            }
            return null;
        },
        GetRange: function () {
            var selection = this.contentWindow.getSelection();
            if(!selection)
                return null;
            if(selection.rangeCount < 1)
                return ASPx.Browser.IE && ASPx.Browser.MajorVersion >= 11 ? this.contentDocument.createRange() : null;

            return selection.getRangeAt ? selection.getRangeAt(0) : this.contentWindow.createRange();//SAFARI
        },
        GetText: function () {
            var ret = "";
            if(this.contentWindow.getSelection)
                ret = this.contentWindow.getSelection().toString();
            return ret;
        },
        GetHtmlText: function () {
            var ret = "";
            var selection = this.contentWindow.getSelection();
            var rng = null;
            if(selection.getRangeAt && selection.rangeCount > 0) {
                rng = ASPx.Browser.Opera ? selection.getRangeAt(0).cloneRange() : selection.getRangeAt(0);
                var tempDiv = this.contentWindow.document.createElement("div");
                var clonedFragment = rng.cloneContents();
                if(ASPx.IsExists(clonedFragment)) {
                    tempDiv.appendChild(clonedFragment);
                    ret = tempDiv.innerHTML;
                }
            }
            return ret;
        },
        GetExtendedBookmark: function () {
            var startSpan = ASPx.HtmlEditorClasses.Selection.CreateElementWithUniqueID(this.contentDocument);
            var endSpan = ASPx.HtmlEditorClasses.Selection.CreateElementWithUniqueID(this.contentDocument);
            var parentElement = this.GetParentElement();
            if(this.canUseMarkers(parentElement)) {
                parentElement.parentNode.insertBefore(startSpan, parentElement);
                ASPx.InsertElementAfter(endSpan, parentElement);
                return { "startMarkerID": startSpan.id, "endMarkerID": endSpan.id};
            }
            var isDirectOrder = !this.NeedExchangeStartAndEndNode((this.startNodePath ? this.startNodePath.path : null), (this.endNodePath ? this.endNodePath.path : null), this.startNodeOffset, this.endNodeOffset);
            var isEmptyTextSelected = false;

            if(this.startContainer == this.endContainer) {
                isEmptyTextSelected = this.endOffset == this.startOffset;
                if(this.endContainer.tagName == "BR") {
                    if(!isDirectOrder)
                        ASPx.InsertElementAfter(endSpan, this.endContainer);
                    else
                        this.endContainer.parentNode.insertBefore(endSpan, this.endContainer);
                }
                else {
                    var newContainer = ASPx.HtmlEditorClasses.Selection.InsertNodeAtPosition(this.endContainer, endSpan, this.endOffset, this.contentDocument);
                    if(ASPx.IsExists(newContainer))
                        this.startContainer = newContainer;
                }
                if(this.startContainer.tagName == "BR") {
                    if(!isDirectOrder)
                        this.startContainer.parentNode.insertBefore(startSpan, this.startContainer);
                    else
                        ASPx.InsertElementAfter(startSpan, this.startContainer);
                }
                else
                    ASPx.HtmlEditorClasses.Selection.InsertNodeAtPosition(this.startContainer, startSpan, this.startOffset, this.contentDocument);
            }
            else {
                if(this.endContainer.tagName == "BR") {
                    if(!isDirectOrder)
                        ASPx.InsertElementAfter(endSpan, this.endContainer);
                    else
                        this.endContainer.parentNode.insertBefore(endSpan, this.endContainer);
                }
                else
                    ASPx.HtmlEditorClasses.Selection.InsertNodeAtPosition(this.endContainer, endSpan, this.endOffset, this.contentDocument);

                if(this.startContainer.tagName == "BR") {
                    if(!isDirectOrder)
                        this.startContainer.parentNode.insertBefore(startSpan, this.startContainer);
                    else
                        ASPx.InsertElementAfter(startSpan, this.startContainer);
                }
                else
                    ASPx.HtmlEditorClasses.Selection.InsertNodeAtPosition(this.startContainer, startSpan, this.startOffset, this.contentDocument);
            }

            var bookmark = { "startMarkerID": startSpan.id, "endMarkerID": endSpan.id, "isEmptyTextSelected": isEmptyTextSelected, "isDirectOrder": isDirectOrder };
            this.SelectExtendedBookmark(bookmark, false);
            return bookmark;
        },
        SelectExtendedBookmark: function (bookmark, withDeleteMarker) {
            if(ASPx.HtmlEditorClasses.Selection.prototype.SelectExtendedBookmark.call(this, bookmark))
                return;
            if(!ASPx.IsExists(withDeleteMarker))
                withDeleteMarker = true;

            var bmElements = ASPx.HtmlEditorClasses.Selection.GetBookmarkElements(bookmark, this.contentDocument);
            var rng1 = this.contentDocument.createRange();

            var startMarker = ASPx.Browser.IE && ASPx.Browser.MajorVersion > 10 ? bmElements.startMarker : (bmElements.startMarker.nextSibling || bmElements.startMarker);
            rng1.setStart(startMarker, 0);
            if(bmElements.startMarker.nextSibling != bmElements.endMarker) {
                var nextSibling = bmElements.endMarker.nextSibling || bmElements.endMarker;
                rng1.setEnd(nextSibling, 0);
            }

            var sel = this.contentWindow.getSelection();
            if(!bookmark.isDirectOrder) {
                if(sel.extend) {
                    sel.removeAllRanges();
                    sel.addRange(rng1);
                    sel.collapseToEnd();
                    sel.extend(bmElements.startMarker, 0);
                }
                else {
                    rng1.setStart(bmElements.startMarker, 0);
                    rng1.setEnd(bmElements.endMarker, 0);
                    sel.removeAllRanges();
                    sel.addRange(rng1);
                }
            }
            else {
                sel.removeAllRanges();
                sel.addRange(rng1);
            }

            if(bookmark.isEmptyTextSelected) {
                var index = ASPx.Data.ArrayIndexOf(bmElements.endMarker.parentNode.childNodes, bmElements.startMarker);
                if(ASPx.Browser.Opera)
                    bmElements.startMarker.innerHTML = "\xA0";
                sel.collapse(bmElements.endMarker.parentNode, index);
                if(ASPx.Browser.Opera)
                    bmElements.startMarker.innerHTML = "";
            }

            if(withDeleteMarker) {
                ASPx.RemoveElement(bmElements.startMarker);
                ASPx.RemoveElement(bmElements.endMarker);
            }
        },

        FindStartEndContainer: function () {
            var rng = this.GetRange();
            if(!rng)
                rng = {};
            var selection = this.contentWindow.getSelection();
            if(!selection)
                selection = {};

            this.startContainer = rng.startContainer ? rng.startContainer : selection.baseNode;
            this.endContainer = rng.endContainer ? rng.endContainer : selection.extentNode;
            this.startOffset = rng.startOffset != null ? rng.startOffset : selection.baseOffset;
            this.endOffset = rng.endOffset != null ? rng.endOffset : selection.extentOffset;
        },
        IsControl: function () {
            var selection = this.contentWindow.getSelection();
            if((!ASPx.Browser.IE && !ASPx.Browser.NetscapeFamily && selection.toString() != "") || !selection.focusNode)
                return false;

            var focusNode = selection.focusNode;
            var rng = this.GetRange();
            var startContainer = this.GetStartContainer();
            var endContainer = this.GetEndContainer();
            var startOffset = this.GetStartOffset();
            var endOffset = this.GetEndOffset();

            if(ASPx.Browser.IE) {
                var startNode, endNode;
                if(startContainer.nodeType == 3 && startOffset == startContainer.nodeValue.length && startContainer.nextSibling && startContainer.nextSibling.nodeType == 1)
                    startNode = startContainer.nextSibling;
                else if(startContainer.nodeType == 1 && startContainer.childNodes.length > 0)
                    startNode = startContainer.childNodes[startOffset];
                if(endContainer.nodeType == 3 && endOffset == 0 && endContainer.previousSibling && endContainer.previousSibling.nodeType == 1)
                    endNode = endContainer.previousSibling;
                else if(endContainer.nodeType == 1 && endContainer.childNodes.length > 0)
                    endNode = endContainer.childNodes[endOffset - 1];
                if(startNode && startNode == endNode)
                    focusNode = startNode;
            } else if(focusNode.nodeType == 1) {
                if(startContainer == endContainer && (endOffset - startOffset) == 1 &&
                        (selection.anchorNode.childNodes.length > selection.anchorOffset))
                    focusNode = selection.anchorNode.childNodes[selection.anchorOffset];
            }
            return (focusNode.nodeName == 'IMG' || ASPx.Browser.IE && (focusNode.nodeName == "TABLE" || focusNode.nodeName == "INPUT" || focusNode.nodeName == "P" && focusNode.style["width"]));
        },
        IsTextOnly: function () {
            var selection = this.contentWindow.getSelection();
            return selection.focusNode && selection.anchorNode &&
                    ((selection.focusNode == selection.anchorNode) ||
                    ((selection.focusNode.nodeType == 3) && (selection.anchorNode.nodeType == 3))); // text node
        },
        SelectElement: function (elem, selectAllContent) {
            if(!ASPx.IsExists(selectAllContent))
                selectAllContent = true;
            ASPx.HtmlEditorClasses.SelectionNSOpera.SelectElementInDocument(elem, this.contentDocument, this.contentWindow, selectAllContent);
        },
        NeedExchangeStartAndEndNode: function (startNodePath, endNodePath, startNodeOffset, endNodeOffset) {
            if(startNodePath != endNodePath) {
                var startNodePathArray = startNodePath.split("-");
                var endNodePathArray = endNodePath.split("-");

                var length = Math.min(startNodePathArray.length, endNodePathArray.length);
                for(var i = 0; i < length; i++) {
                    var i1 = parseInt(startNodePathArray[i], 10);
                    var i2 = parseInt(endNodePathArray[i], 10);
                    if(i1 > i2)
                        return true;
                    else if(i1 < i2)
                        return false;
                }
            }
            return startNodeOffset > endNodeOffset;
        },
        GetNodePath: function (rootNode, node) {
            var path = "";
            var curParentNode = node;
            var curNode = node;
            var lastParentNodeChilds = node ? this.getChildInfo(node.parentNode) : null;
            if(curParentNode) {
                while(rootNode != curParentNode) {
                    curParentNode = curParentNode.parentNode;

                    if(curParentNode.childNodes) {
                        var index = ASPx.Data.ArrayIndexOf(curParentNode.childNodes, curNode);
                        path = index.toString() + "-" + path;
                    }
                    curNode = curParentNode;
                }
            }
            return { path: path.substr(0, path.length - 1), lastParentNodeChilds: lastParentNodeChilds };
        },
        getChildInfo: function(parent) {
            var res = [];
            for(var i = 0, child; child = parent.childNodes[i]; i++)
                res.push({ nodeType: child.nodeType, nodeValue: (child.nodeType == 3 ? child.nodeValue : null) });
            return res;
        },
        spliteTextNode: function(parent, index, start, end) {
            var child = parent.childNodes[index];
            var firstChild = this.contentDocument.createTextNode(child.nodeValue.substring(start, end));
            var secondChild = this.contentDocument.createTextNode(child.nodeValue.substring(end, child.nodeValue.length));
            child.parentNode.insertBefore(firstChild, child);
            child.parentNode.insertBefore(secondChild, child);
            child.parentNode.removeChild(child);
        },
        GetNodeByPath: function (rootNode, path, isFirstNode) {
            if(path.path != "") {
                var pathNodeArray = path.path.split("-");
                var curNode = rootNode;
                for(var i = 0; i < pathNodeArray.length; i++) {
                    var index = parseInt(pathNodeArray[i], 10);
                    if(i == pathNodeArray.length - 1 && path.lastParentNodeChilds.length != curNode.childNodes.length) {
                        for(var j = 0, childInfo; childInfo = path.lastParentNodeChilds[j]; j++) {
                            var curChild = curNode.childNodes[j];
                            if(childInfo.nodeType == 3 && curChild.nodeType == 3 && curChild.nodeValue != childInfo.nodeValue)
                                this.spliteTextNode(curNode, j, 0, childInfo.nodeValue.length);
                        }
                    }
                    if(index >= curNode.childNodes.length) {
                        if(isFirstNode)
                            this.startNodeOffset = 0;
                        else
                            this.endNodeOffset = 0;
                        return curNode; // ifon undo/redo operations was deleted empty nodes and path can't will be restored
                    }
                    curNode = curNode.childNodes[index];
                }
                return curNode;
            }
            else
                return null;
        }
    });
    ASPx.HtmlEditorClasses.SelectionNSOpera.RestoreByStartAndEndNodeCore = function (startNode,
                                                                     endNode,
                                                                     startNodeOffset,
                                                                     endNodeOffset, isCollapsed,
                                                                     docObj, windowObj) {
        var curSelection = windowObj.getSelection();
        ASPx.HtmlEditorClasses.SelectionNSOpera.SelectElementInDocument(startNode, docObj, windowObj);
        ASPx.HtmlEditorClasses.SelectionNSOpera.SelectElementInDocument(endNode, docObj, windowObj);

        if(startNode) {
            if(startNode.nodeType == 1 && startNodeOffset > startNode.childNodes.length)
                startNodeOffset = startNode.childNodes.length > 0 ? startNode.childNodes.length - 1 : 0;
            curSelection.collapse(startNode, startNodeOffset);
        }
        if(endNode && !isCollapsed) {
            if(endNode.nodeType == 1 && endNodeOffset > endNode.childNodes.length)
                endNodeOffset = endNode.childNodes.length > 0 ? endNode.childNodes.length - 1 : 0;
            if(ASPx.Browser.WebKitFamily)
                curSelection.setBaseAndExtent(startNode, startNodeOffset, endNode, endNodeOffset);
            else {
                if(curSelection.extend)
                    curSelection.extend(endNode, endNodeOffset);
                else {
                    var rng = docObj.createRange();
                    rng.setStart(startNode, startNodeOffset);
                    rng.setEnd(endNode, endNodeOffset);
                    curSelection.removeAllRanges();
                    curSelection.addRange(rng);
                }
            }
        }
    }
    ASPx.HtmlEditorClasses.SelectionNSOpera.SelectElementInDocument = function (elem, docObj, windowObj, selectAllContent) {
        if(elem) {

            if(!ASPx.IsExists(selectAllContent))
                selectAllContent = true;

            var range = docObj.createRange();
            range.selectNode(elem);

            if(ASPx.Browser.Opera)
                range.selectNodeContents(elem);

            if(!selectAllContent)
                range.collapse(false);

            var selection = windowObj.getSelection();
            if(ASPx.Browser.WebKitFamily)
                selection.setBaseAndExtent(range.startContainer, range.startOffset, range.endContainer, range.endOffset);
            else {
                selection.removeAllRanges();
                selection.addRange(range);
            }
        }
    }

    
    ASPx.HtmlEditorClasses.SelectionEdge = ASPx.CreateClass(ASPx.HtmlEditorClasses.SelectionNSOpera, {
        GetParentElement: function () {
            var element = ASPx.HtmlEditorClasses.Selection.prototype.GetParentElement.call(this);
            if(element)
                return element;
            var ret = null;
            var rng = this.GetRange();
            if(!rng) return ret;

            var selection = this.contentWindow.getSelection();

            var startContainer = this.GetStartContainer(rng, selection);
            var endContainer = this.GetEndContainer(rng, selection);
            var startOffset = this.GetStartOffset(rng, selection);
            var endOffset = this.GetEndOffset(rng, selection);

            if(this.IsControl())
                ret = selection.anchorNode.nodeType == 3 ? startContainer.nextSibling : selection.anchorNode.childNodes[selection.anchorOffset];
            else {
                if(!rng.commonAncestorContainer.tagName) {
                    if(this.contentWindow.document == rng.commonAncestorContainer && 
                        rng.commonAncestorContainer == rng.startContainer && rng.startContainer == rng.endContainer)
                            return this.contentWindow.document.body;
                    ret = rng.commonAncestorContainer.parentNode
                }
                else
                    ret = rng.commonAncestorContainer;
            }
            return ret;
        },
        GetRange: function () {
            var selection = this.contentWindow.getSelection();
            if(selection)
                return selection.rangeCount < 1 ? this.contentDocument.createRange() : selection.getRangeAt(0);
            return null;
        },
        IsControl: function () {
            var selection = this.contentWindow.getSelection();
            if(!selection.focusNode)
                return false;

            var focusNode = selection.focusNode;
            var rng = this.GetRange();
            var startContainer = this.GetStartContainer();
            var endContainer = this.GetEndContainer();
            var startOffset = this.GetStartOffset();
            var endOffset = this.GetEndOffset();

            var startNode, endNode;
            if(startContainer.nodeType == 3 && startOffset == startContainer.nodeValue.length && startContainer.nextSibling && startContainer.nextSibling.nodeType == 1)
                startNode = startContainer.nextSibling;
            else if(startContainer.nodeType == 1 && startContainer.childNodes.length > 0)
                startNode = startContainer.childNodes[startOffset];
            if(endContainer.nodeType == 3 && endOffset == 0 && endContainer.previousSibling && endContainer.previousSibling.nodeType == 1)
                endNode = endContainer.previousSibling;
            else if(endContainer.nodeType == 1 && endContainer.childNodes.length > 0)
                endNode = endContainer.childNodes[endOffset - 1];
            if(startNode && startNode == endNode)
                focusNode = startNode;
            
            return (focusNode.nodeName == 'IMG' || ASPx.Browser.IE && (focusNode.nodeName == "TABLE" || focusNode.nodeName == "INPUT" || focusNode.nodeName == "P" && focusNode.style["width"]));
        }
    });

    ASPx.HtmlEditorClasses.Managers.CachedElementsManager = ASPx.CreateClass(null, {
        constructor: function () {
            this.selectedElements = { table: null, tableRow: null, tableCell: null };
            this.isNeedCachedSelectedElements = { table: true, tableRow: true, tableCell: true };
        },
        ClearSeletedElements: function () {
            for(var key in this.selectedElements) {
                this.selectedElements[key] = null;
                this.isNeedCachedSelectedElements[key] = true;
            }
        },
        GetSeletedElement: function (name) {
            return this.selectedElements[name];
        },
        SetSelectedElement: function (name, element) {
            this.selectedElements[name] = element;
            this.isNeedCachedSelectedElements[name] = false;
        }
    });

    window.ASPxClientHtmlEditorSelection = ASPxClientHtmlEditorSelection;
})();