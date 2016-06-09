(function() {
    ASPx.HtmlEditorClasses.Commands.HtmlView = { };

    ASPx.HtmlEditorClasses.Commands.Command = ASPx.CreateClass(null, {
	    constructor: function(cmdID, owner) {
		    this.commandID = cmdID;
            this.owner = owner;
        },
        CanChangeSelection: function() {
            return true;    
        },
	    Execute: function(cmdValue, wrapper) {
	        return true;
	    },
	    GetCommandID: function() {
	        return this.commandID;
	    },	
	    GetState: function(wrapper, selection, selectedElements) {
	        return true;
	    },
	    GetValue: function(wrapper, selection, selectedElements) {
	        return null;
	    },
        IsDefaultAction: function(wrapper) {
            return false; 
            // true for all command, that don't require additional processing
        },
	    IsImmediateExecution: function() {
	        return false;
	    },
	    IsHtmlChangeable: function() {
	        return true;
	    },
        IsLocked: function(wrapper) {
            return false;
        },
        canBeExecutedOnSelection: function(wrapper, curSelection) {
            if(!curSelection)
                return true;
            var containerElement = curSelection.GetSelectedElement();
            var selectedElement = this.getSelectedElement(containerElement);
            return this.canBeExecutedOnSelectedElement(selectedElement);
        },
        getSelectedElement: function(containerElement) {
            return containerElement.tagName == "IMG" ? containerElement : ASPx.GetParentByTagName(containerElement, "IMG");
        },
        canBeExecutedOnSelectedElement: function(selectedElement) {
            return !selectedElement;
        },
        isEnabled: function(wrapper, curSelection) {
            return !this.IsLocked(wrapper) && this.canBeExecutedOnSelection(wrapper, curSelection);
        },
	    IsReversable: function() {
	        return true;
	    },
        IsClientCommand: function() {
            return true;
        },
        SkipStateOnFirstLoad: function() {
            return false;
        },
	    GetDefaultValue: function() {
	        return "";
	    },
        ContainsNotDOMElements: function(elements) {
            for(var i = 0, element; element = elements[i]; i++) {
                if(!element.parentNode || element.parentNode.nodeType == 11)
                    return true;
            }
            return false;
        }
    });

    ASPx.HtmlEditorClasses.Commands.Utils = {
        IsInlineTextElement: function(element) {
            var expr = /^(SPAN|B|STRONG|I|EM|U|S|SUP|SUB|FONT)$/i;
            return expr.test(typeof (element) == "object" ? element.tagName : element);
        },
        CanBePlacedInInlineParent: function(element) {
            if(this.IsInlineTextElement(element))
                return true;
            var expr = /^(IMG|A|BR|SMALL|BIG)$/i;
            return expr.test(typeof (element) == "object" ? element.tagName : element);
        },
        FindParent: function(element, comparer) {
            var parent = null;
            while(element && element.tagName != "BODY") {
                element = element.parentNode;
                if(comparer(element))
                    parent = element;
                else break;
            }
            return parent;
        },
        IsBlockElement: function(element) {
            return element && element.style && ASPx.GetCurrentStyle(element)["display"] != "inline" && element.nodeName != "IMG" || 
            (ASPx.HEBlockElements[element.nodeName.toLowerCase()] || ASPx.HEPathBlockLimitElements[element.nodeName.toLowerCase()]);
        },
        FindNextBlockElement: function(element) {
            return this.FindElement(element, this.IsBlockElement,
                function(element) {
                    return element.nextSibling;
                }.aspxBind(this)
            );
        },
        FindPreviousBlockElement: function(element) {
            return this.FindElement(element, this.IsBlockElement,
                function(element) {
                    return element.previousSibling;
                }.aspxBind(this)
            );
        },
        FindElement: function(element, comparer, func) {
            var result;
            for(var el = element; el; el = func(el)) {
                if(comparer(el)) {
                    result = el;
                    break;
                }
            }
            return result;
        },
        GetBetweenBlocksInlineElements: function(element) {
            var firstElement, lastElement;
            var resultList = [];
            firstElement = this.FindPreviousBlockElement(element);
            lastElement = this.FindNextBlockElement(element);
            firstElement = firstElement ? firstElement.nextSibling : element.parentNode.firstChild;
            lastElement = lastElement ? lastElement.previousSibling : element.parentNode.lastChild;
            for(var el = firstElement; el !== lastElement.nextSibling; el = el.nextSibling)
                resultList.push(el);
            return resultList;
        },
        InsertNewElement: function(doc, selection, nodeId) {
            var tempNode = doc.createElement("SPAN");
            tempNode.id = nodeId;
            selection.InsertHtml(tempNode.outerHTML);
            return ASPx.GetElementByIdInDocument(doc, nodeId);
        },
        createBookmarkElements: function(doc, firstElement, lastElement, isDirectOrder) {
            isDirectOrder = isDirectOrder === undefined || isDirectOrder;
            var selectionBookmarks = { "startMarkerID": "sbm", "endMarkerID": "ebm"};
            var sbm = doc.createElement("span");
            sbm.id = "sbm";
            var ebm = doc.createElement("span");
            ebm.id = "ebm";
            firstElement.parentNode.insertBefore(isDirectOrder ? sbm : ebm, firstElement);
            ASPx.InsertElementAfter(isDirectOrder ? ebm : sbm, lastElement);
            return selectionBookmarks;
        },
        correctSelectionForDeleteCommand: function(wrapper) {
            if(ASPx.Browser.NetscapeFamily) { // B204211
                var clientSelection = ASPxClientHtmlEditorSelection.Create(wrapper.getWindow());
                var parent = clientSelection.GetParentElement();
                if(parent.nodeName == "BODY" && parent.firstChild.nodeName == "TABLE") {
                    var tableElement = parent.firstChild;
                    var bm = this.createBookmarkElements(wrapper.getDocument(), parent.firstChild, parent.lastChild);
                    parent.removeChild(tableElement);
                    clientSelection.SelectExtendedBookmark(bm);
                }
                else if(parent.tagName == "A") {
                    var isNeedDeleteParentNode = false;
                    var selectionText = clientSelection.GetText();
                    if(selectionText != "") {
                        var parentInnerText = ASPx.GetInnerText(parent);
                        isNeedDeleteParentNode = selectionText == parentInnerText;
                    }
                    else {
                        var bm = clientSelection.GetExtendedBookmark();
                        var startBmElement = ASPx.GetElementByIdInDocument(clientSelection.contentDocument, bm.startMarkerID);
                        var endBmElement = ASPx.GetElementByIdInDocument(clientSelection.contentDocument, bm.endMarkerID);

                        var parts = ASPx.HtmlEditorClasses.Commands.Utils.SplitNode(parent, [endBmElement, startBmElement]);
                        var prevSymbolsCount = ASPx.GetInnerText(parts[0]).length,
                            nextSymbolsCount = ASPx.GetInnerText(parts[parts.length - 1]).length;
                        var lastKeyDownInfo = wrapper.getLastKeyDownInfo();
                        if(lastKeyDownInfo && lastKeyDownInfo.isBackSpaceKey)
                            isNeedDeleteParentNode = prevSymbolsCount < 2 && nextSymbolsCount == 0;
                        else
                            isNeedDeleteParentNode = nextSymbolsCount < 2 && prevSymbolsCount == 0;
                        clientSelection.SelectExtendedBookmark(bm);
                    }
                    if(isNeedDeleteParentNode && parent.tagName != "BODY")
                        clientSelection.SelectElement(parent);
                }
            }
        },
        SplitNode: function(parent, params) {
            var separators = [];
            var parts = [];

            if(this.IsArray(params))
                separators = separators.concat(params);
            else if(params instanceof Object)
                separators.push(params);

            var fullPathSeparators = this.GetFullPathSeparators(parent, separators);
            if(fullPathSeparators.length > 0) {
                var count = fullPathSeparators.length;
                parts.push(this.GetPart(parent, null, fullPathSeparators[0]));
                for(var i = 0; i < count - 1; i++) {
                    parts.push(this.GetPart(parent, fullPathSeparators[i], fullPathSeparators[i+1]));
                }
                parts.push(this.GetPart(parent, fullPathSeparators[count - 1], null));
                return parts;
            }
            return [ parent ];
        },
        GetFullPathSeparators: function(parent, separators) {
            var paths = [];
            var path = [];
            for(var i = 0; i < separators.length; i++) {
                var element = separators[i];
                if(ASPx.GetIsParent(parent, element)) {
                    while(element != null && element != parent.parentNode) {
                        path.push(element);
                        element = element.parentNode;
                    }
                    path = path.reverse();
                    paths.push(path);
                }
                path = [];
            }
            return paths;
        },
        GetPart: function(parent, prevBound, nextBound) {
            var dest = parent.cloneNode(false);
            this.CreateBranch(parent, dest, 1, prevBound, nextBound);
            return dest;
        },
        CreateBranch: function(srcNode, dstNode, nestingIndex, prevBound, nextBound) {
            var prevLimitElement = (prevBound && nestingIndex < prevBound.length) ? prevBound[nestingIndex] : null;
            var nextLimitElement = (nextBound && nestingIndex < nextBound.length) ? nextBound[nestingIndex] : null;

            var nodes = ASPx.GetChildNodes(srcNode, function(element) { return element == prevLimitElement; });
            var child = (nodes.length > 0) ? nodes[0] : srcNode.firstChild;

            var isNeedBreak = (child == nextLimitElement);
            while(child) {
                dstNode.appendChild(child.cloneNode(false));

                if(child.hasChildNodes())
                    this.CreateBranch(child, dstNode.lastChild, nestingIndex + 1, prevBound, nextBound);

                if(isNeedBreak)
                    break;
                child = child.nextSibling;
                isNeedBreak = (child == nextLimitElement);
            }
        },
        IsArray: function(object) {
            return (typeof object == "object") && (object instanceof Array);
        },
        processingElementsBeforeCope: function(elements) {
            var processedInputElements = [];
            var processedElements = [];
            var getElement = function(element) {
                if(element.nodeType == 1 && /img|a|iframe/gi.test(element.nodeName)) {
                    if(/img|iframe/gi.test(element.nodeName))
                        ASPx.Attr.SaveAttribute(element, "src", element, ASPx.HtmlEditorClasses.SavedSrcAttributeName);
                    else
                        ASPx.Attr.SaveAttribute(element, "href", element, ASPx.HtmlEditorClasses.SavedSrcAttributeName);
                    return element;
                }
            };
            var getInputElement = function(el) {
                if(el.nodeName == "INPUT" && ASPx.Attr.GetAttribute(el.style, "-moz-user-select")) {
                    ASPx.Attr.RemoveAttribute(el.style, "-moz-user-select");
                    return el;
                }
            }
            for(var i = 0, el; el = elements[i]; i++) {
                if(ASPx.Browser.NetscapeFamily)
                    processedInputElements = processedInputElements.concat(ASPx.HtmlEditorClasses.Utils.getChildsByPredicate(el, getInputElement));
                processedElements = processedElements.concat(ASPx.HtmlEditorClasses.Utils.getChildsByPredicate(el, getElement));
            }
            window.setTimeout(function() {
                for(var i = 0, el; el = processedInputElements[i]; i++)
                    ASPx.Attr.SetAttribute(el.style, "-moz-user-select", "none");
                for(var i = 0, el; el = processedElements[i]; i++)
                    ASPx.Attr.RestoreAttributeExtended(el, /img|iframe/gi.test(el.nodeName) ? "src" : "href", el, ASPx.HtmlEditorClasses.SavedSrcAttributeName);
            }.aspxBind(this), 0);
        }
    };
})();