(function() {
    ASPx.HtmlEditorClasses.Commands.Browser.WrappedCommand = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Browser.Command, {
        Execute: function(cmdValue, wrapper) {
            if(!wrapper.isSelectionRestored()) {
                if(!ASPx.Browser.Opera)
                    wrapper.restoreSelection();
                else
                    wrapper.focus();
            }
            if(ASPx.Browser.MobileUI && wrapper.getSelection().IsCollapsed())
                return ASPx.HtmlEditorClasses.Commands.Browser.Command.prototype.Execute.call(this, cmdValue, wrapper);
            var isSuccessfully = this.ExecuteCore(cmdValue, wrapper);
            if(isSuccessfully) {
                wrapper.saveSelection();
                setTimeout(function() {
                    if(!wrapper.isSelectionRestored())
                        wrapper.restoreSelection();
                }.aspxBind(this), 300);
            }
            return isSuccessfully;
        },
        ExecuteCore: function(cmdValue, wrapper) {
            return true;
        },
        ExecuteInternal: function(cmdValue, wrapper) {
            var doc = wrapper.getDocument();
            var selection = wrapper.getSelection();
            var selectedElements = selection.GetElements(true);
            if(selectedElements.length > 0) {
                selectedElements = this.replaceTableElements(selectedElements);
                var selectedUnwrappedElements = this.GetNotStylizedElements(wrapper, selectedElements, null, false);
                if(selectedUnwrappedElements.length == 0)
                    return true;
                else {
                    var selectionBookmarks = ASPx.HtmlEditorClasses.Commands.Utils.createBookmarkElements(doc, selectedElements[0], selectedElements[selectedElements.length - 1]);
                    this.RemoveStyle(wrapper, selectedUnwrappedElements, selectionBookmarks);
                    if(this.ContainsNotDOMElements(selectedUnwrappedElements)) {
                        selectedElements = ASPx.HtmlEditorClasses.Utils.ElementsSelectionHelper.GetSelectedElements(doc, selectionBookmarks.startMarkerID, selectionBookmarks.endMarkerID, true);
                        selectedUnwrappedElements = this.GetNotStylizedElements(wrapper, selectedElements, null, false);
                    }
                    this.StylizeElements(wrapper, selectedUnwrappedElements, selectionBookmarks);
                    wrapper.getSelection().clientSelection.SelectExtendedBookmark(selectionBookmarks);
                }
            }
            else
                this.ExecuteCommandLater(wrapper, selection);
            return true;
        },
        GetState: function(wrapper, selection, selectedElements) {
            if(!wrapper.isInFocus)
                return this.IsElementStylized(wrapper.getDocument().body);
            if(ASPx.Browser.WebKitTouchUI || ASPx.Browser.MSTouchUI)
                return ASPx.HtmlEditorClasses.Commands.Browser.Command.prototype.GetState.call(this, wrapper);
            selection = !selection ? wrapper.getSelection() : selection;
            var selectedParent = this.GetSelectedElement(wrapper, selection, selectedElements);
            if(selectedElements && selectedElements.length == 1 && selectedElements[0].nodeType != 3 && selectedElements[0].childNodes.length == 0)
                selectedElements = [];
            return this.GetStateCore(wrapper, selection, selectedParent, selectedElements);
        },
        GetStateCore: function(wrapper, selection, selectedParent, selectedElements) {
            return null;
        },
        GetStateInternal: function(wrapper, selection, selectedElements) {
            if(selection) {
                selectedElements = !selectedElements ? selection.GetElements(true) : selectedElements;
                return this.GetNotStylizedElements(wrapper, selectedElements, null, false) == 0;
            }
        },
        GetValue: function(wrapper, selection, selectedElements) {
            if(!wrapper.isInFocus)
                return this.GetValueCore(wrapper);
            if(ASPx.Browser.WebKitTouchUI || ASPx.Browser.MSTouchUI)
                return ASPx.HtmlEditorClasses.Commands.Browser.Command.prototype.GetValue.call(this, wrapper);
            selection = !selection ? wrapper.getSelection() : selection;
            var selectedParent = this.GetSelectedElement(wrapper, selection, selectedElements);
            if(selectedElements && selectedElements.length == 1 && selectedElements[0].nodeType != 3 && selectedElements[0].childNodes.length == 0)
                selectedElements = [];
            return this.GetValueCore(wrapper, selection, selectedParent, selectedElements);
        },
        GetValueCore: function(wrapper, selection, selectedParent, selectedElements) {
            return null;
        },
        IsCommandTextDecoration: function() {
            var cmdName = this.GetCommandName();
            return ASPxClientCommandConsts.UNDERLINE_COMMAND == cmdName || ASPxClientCommandConsts.STRIKETHROUGH_COMMAND == cmdName;
        },
        IsBookmark: function(bookmark, element) {
            return element && bookmark && element.nodeType == 1 && (bookmark.startMarkerID == element.id || bookmark.endMarkerID == element.id);
        },
        IsFontOrSpanElement: function(element) {
            return element.nodeName == "SPAN" || element.nodeName == "FONT";
        },
        IsExistAttributes: function(element) {
            return element.nodeType == 1 && (element.id || element.className || element.style.cssText);
        },
        IsParentElementStylized: function(element) {
            return (this.NeedFindFontStyleItalicParent() && this.FindFontStyleItalicParent(element)) || (this.IsCommandTextDecoration() && this.FindTextDecorationParent(element));
        },
        IsElementStylized: function(element) {
            var attr = this.GetAttribute();
            var styleAttrConstValue = this.GetStyleAttributeConstValue();
            var styleAttrValue = this.GetCurrentStyleAttributeValue(element);
            var result = styleAttrValue && styleAttrValue.indexOf(styleAttrConstValue) > -1;
            if(typeof(styleAttrValue) == "string" && /(%|px|pt|cm)$/gi.test(styleAttrValue))
                result = this.convertToPixel(styleAttrValue) == this.convertToPixel(styleAttrConstValue);
            return result || this.GetAttributeValue(element, attr) == styleAttrConstValue;
        },
        NeedFindFontStyleItalicParent: function() {
            return ASPx.Browser.IE && ASPx.Browser.MajorVersion < 9 && ASPxClientCommandConsts.ITALIC_COMMAND == this.GetCommandName();
        },
        StylizeElements: function(wrapper, elements, selectionBookmarks, wrapInFontStyleNormal) {
            if(wrapInFontStyleNormal && this.IsCommandTextDecoration())
                return;
            var elementContainer;
            var doc = wrapper.getDocument();
            for(var i = 0, element; element = elements[i]; i++) {
                if((element.nodeType == 3 && !element.nodeValue) || this.IsBookmark(selectionBookmarks, element) || !element.parentNode)
                    continue;
                var prevSibling = element.previousSibling;
                var isPreviousElementWrapper;
                if(prevSibling && prevSibling.nodeType == 1) {
                    var attrValue = this.GetStyleAttributeValue(prevSibling);
                    if(attrValue)
                        this.SetStyleAttribute(prevSibling, "");
                    var isNotSetStyleAttribute = !this.IsExistAttributes(prevSibling);
                    if(attrValue)
                        this.SetStyleAttribute(prevSibling, attrValue);
                    isPreviousElementWrapper = prevSibling !== elementContainer && isNotSetStyleAttribute && !this.IsBookmark(selectionBookmarks, prevSibling) &&
                                               (wrapInFontStyleNormal && this.IsStyleAttributeNormal(prevSibling) || this.IsTagFontStyle(prevSibling));
                }
                if(isPreviousElementWrapper && prevSibling && prevSibling.nodeName != "IMG")
                    elementContainer = prevSibling;
                if(elementContainer && elementContainer === prevSibling)
                    elementContainer.appendChild(element);
                else {
                    if(wrapInFontStyleNormal) {
                        elementContainer = doc.createElement("SPAN");
                        this.SetStyleAttribute(elementContainer, "normal");
                    }
                    else
                        elementContainer = this.CreateStylizedWrapper(doc, wrapper.settings.updateDeprecatedElements, wrapper.settings.updateBoldItalic, "SPAN");
                    element.parentNode.insertBefore(elementContainer, element);
                    elementContainer.appendChild(element);
                }
            }
        },
        UnwrapElement: function(wrapper, element, selectionBookmarks, removeFontStyleInHierarchy) {
            var childElements = [];
            for(var child = element.firstChild; child; child = element.firstChild) {
                element.parentNode.insertBefore(child, element);
                childElements.push(child);
                if(removeFontStyleInHierarchy && child.nodeType != 3 && this.FindFontStyleDescendant(child))
                    this.RemoveStyle(wrapper, [ child ], selectionBookmarks, removeFontStyleInHierarchy);
            }
            element.parentNode.removeChild(element);
            return childElements;
        },
        RemoveStyle: function(wrapper, elements) {
            for(var i = 0, element; element = elements[i]; i++) {
                var childElements = [];
                var attr = this.GetAttribute();
                if(element.nodeType == 1) {
                    var outerHTML = element.outerHTML;
                    var attr = this.GetAttribute();
                    if(this.GetAttributeValue(element, attr))
                        ASPx.Attr.RemoveAttribute(element, attr);
                    if(this.GetStyleAttributeValue(element))
                        this.SetStyleAttribute(element, "");
                    this.RemoveStyleInHierarchy(wrapper, element, outerHTML != element.outerHTML);
                }
            }
        },
        RemoveStyleInHierarchy: function(wrapper, element, isElementChanged) {
            if(!element || element.nodeType != 1)
                return;
            if(isElementChanged && !this.IsExistAttributes(element) && this.IsFontOrSpanElement(element)) {
                childElements = this.UnwrapElement(wrapper, element, null, false);
                this.RemoveStyle(wrapper, childElements);
            }
            else {
                for(var child = element.firstChild; child; child = child.nextSibling)
                    this.RemoveStyle(wrapper, [child]);
            }
        },
        ExecuteCommandLater: function(wrapper, selection) {
            var selectedParent = this.GetSelectedElement(wrapper, selection);
            if(this.IsElementStylized(selectedParent) || (this.IsCommandTextDecoration() && selectedParent.nodeName != "BODY" && this.FindTextDecorationParent(selectedParent))) {
                if(this.NeedChangeRemoveStyleCommandState())
                    this.owner.typeCommandProcessor.changeRemoveStyleCommand(this);
            }
            else
                this.owner.typeCommandProcessor.changeApplyStyleCommand(this);
            var selection = wrapper.getSelection();
            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 9 && !selection.GetHtml())
                wrapper.selectionManager.isSelectionRestored = true;
            wrapper.eventManager.setLockUpdate(true);
            return false;
        },
        NeedChangeRemoveStyleCommandState: function() {
            return false;
        },
        CanBeWrapped: function(element) {
            return element.nodeType == 3 || ASPx.HtmlEditorClasses.Commands.Utils.CanBePlacedInInlineParent(element);
        },
        CanNotBeRemove: function(element) {
            return element.nodeName == "SUB" || element.nodeName == "SUP" || 
                   (ASPx.HtmlEditorClasses.Commands.Utils.CanBePlacedInInlineParent(element) && !ASPx.HtmlEditorClasses.Commands.Utils.IsInlineTextElement(element)) ||
                   ASPx.HEBlockElements[element.nodeName.toLowerCase()];
        },
        FindDescendant: function(element, comparer, comparerParam) {
            if(comparer(element, comparerParam))
                return element;
            if(element.nodeType != 3) {
                for(var child = element.firstChild; child; child = child.nextSibling) {
                    var childNode = this.FindDescendant(child, comparer, comparerParam)
                    if(childNode)
                        return childNode;
                }
            }
            return null;
        },
        FindNotWrappedInlineDescendant: function(parent) {
            return this.FindDescendant(parent,
                function(element) {
                    return (ASPx.HtmlEditorClasses.Commands.Utils.CanBePlacedInInlineParent(element) && !ASPx.HtmlEditorClasses.Commands.Utils.IsInlineTextElement(element) || element.nodeType == 3)
                    && !this.IsElementStylized(element) && !this.IsParentElementStylized(element);
                }.aspxBind(this)
            );
        },
        FindNotWrappedDescendant: function(parent) {
            return this.FindDescendant(parent,
                function(element) {
                    return !this.IsElementStylized(element);
                }.aspxBind(this)
            );
        },
        FindFontStyleDescendant: function(parent) {
            return this.FindDescendant(parent,
                function(element) {
                    return this.IsElementStylized(element) || this.IsStyleAttributeNormal(element);
                }.aspxBind(this)
            );
        },
        FindBookmarkDescendant: function(parent, selectionBookmarks) {
            return this.FindDescendant(parent,
                function(element, selectionBookmarks) {
                    return element.id && this.IsBookmark(selectionBookmarks, element);
                }.aspxBind(this),
                selectionBookmarks
            );
        },
        FindAllTextNodes: function(element) {
            if(element && element.nodeType == 3)
                return element;
            var nodes = [];
            for(var child = element.firstChild; child; child = child.nextSibling)
                nodes = nodes.concat(this.FindAllTextNodes(child));
            return nodes;
        },
        FindTextDecorationParent: function(parent) {
            return this.FindParent(parent,
                function(attributeValue) {
                    return this.IsTextDecorationAttributeValue(attributeValue);
                }.aspxBind(this)
            );
        },
        FindFontStyleItalicParent: function(parent) {
            return this.FindParent(parent,
                function(attributeValue, element) {
                    return attributeValue == this.GetStyleAttributeConstValue() || this.IsTagFontStyle(element);
                }.aspxBind(this)
            );
        },
        FindParent: function(element, compare) {
            var result;
            var attr = this.GetStyleAttribute();
            for(var parent = element.parentNode; parent.nodeName != "BODY"; parent = parent.parentNode) {
                var attributeValue =  ASPx.GetCurrentStyle(parent)[attr];
                if(compare(attributeValue, parent))
                    result = parent;
            }
            return result;
        },
        SetStyleAttribute: function(element, value) {
            if(element.nodeType != 3) {
                var attr = this.GetStyleAttribute();
                if(value == "" && ASPx.Browser.IE)
                    ASPx.Attr.RemoveAttribute(element.style, attr);
                else
                    element.style[attr] = value;
                if(ASPx.Browser.NetscapeFamily) // force current style recalculating
                    var offsetHeight = element.offsetHeight;
            }
        },
        GetCurrentStyleAttributeValue: function(element) {
            var attr = this.GetStyleAttribute();
            return ASPx.GetCurrentStyle(element.nodeType == 1 ? element : element.parentNode)[attr];
        },
        GetAttributeValue: function(element, attr) {
            var attrValue = ASPx.Attr.GetAttribute(element.nodeType == 1 ? element : element.parentNode, attr);
            return attrValue && typeof(attrValue) == "string" ? attrValue.toLowerCase() : attrValue;
        },
        GetStyleAttribute: function() {
            return null;
        },
        GetAttribute: function() {
            return null;
        },
        GetStyleAttributeConstValue: function() {
            return null;
        },
        GetStyleAttributeValue: function(element) {
            if(element.nodeType != 3) {
                var attr = this.GetStyleAttribute();
                var attrValue = element.style[attr];
                return attrValue;
            }
        },
        GetCommonStyleAttributeValue: function(elements) {
            var textNodes = [];
            for(var i = 0, element; element = elements[i]; i++)
                textNodes = textNodes.concat(this.FindAllTextNodes(element));
            if(textNodes.length == 0)
                return "";
            var firstElement = textNodes[0];
            var styleAttrValue = this.GetCurrentStyleAttributeValue(firstElement);
            for(var i = 1, element; element = textNodes[i]; i++) {
                if(styleAttrValue != this.GetCurrentStyleAttributeValue(element))
                    return null;
            }
            return styleAttrValue;
        },
        GetNotStylizedElements: function(wrapper, elements, selectionBookmarks, removeFontStyleInHierarchy) {
            var resultList = [];
            for(var i = 0, element; element = elements[i]; i++) {
                if(this.skipElement(element))
                    continue;
                var elementStylized = this.IsElementStylized(element) || this.IsParentElementStylized(element);
                if(!this.CanBeWrapped(element) || elementStylized || this.GetStyleAttributeValue(element) == "normal") {
                    for(var child = element.firstChild; child; child = child.nextSibling)
                        resultList = resultList.concat(this.GetNotStylizedElements(wrapper, [child], selectionBookmarks, removeFontStyleInHierarchy));
                }
                else {
                    if(!this.FindNotWrappedInlineDescendant(element))
                        continue;
                    resultList = resultList.concat(removeFontStyleInHierarchy ? this.GetCanBeWrappedElements(wrapper, [element], selectionBookmarks) : [element]);
                }
            }
            return resultList;
        },
        GetCanBeWrappedElements: function(wrapper, elements, selectionBookmarks) {
            var resultList = [];
            for(var i = 0, element; element = elements[i]; i++) {
                if(this.CanBeWrapped(element) && !this.FindBookmarkDescendant(element, selectionBookmarks) && !this.IsElementStylized(element)) {
                    resultList.push(element);
                    if(this.FindFontStyleDescendant(element))
                        this.RemoveStyle(wrapper, [element], selectionBookmarks, true);
                } else {
                    var childElements = [];
                    for(var child = element.firstChild; child; child = child.nextSibling)
                        childElements.push(child);
                    this.RemoveStyle(wrapper, [element], selectionBookmarks, false);
                    resultList = resultList.concat(this.GetCanBeWrappedElements(wrapper, childElements, selectionBookmarks));
                }
            }
            return resultList;
        },
        GetSelectedElement: function(doc, selection) {
            if(selection) {
                var selectedElement = selection.GetSelectedElement();
                return selectedElement.nodeName == "HTML" ? doc.body : selectedElement;
            }
            return doc.body;
        },
        CreateStylizedWrapper: function(doc, updateDeprecatedElements, updateBoldItalic, tagName) {
            var newElement = doc.createElement(tagName);
            this.SetStyleAttribute(newElement, this.GetStyleAttributeConstValue());
            return newElement;
        },
        ChangeExecuteStyleCommandState: function(commandName, executeFontStyleCommandState) {
            var index = ASPx.Data.ArrayIndexOf(executeFontStyleCommandState, commandName);
            if(index > -1)
                executeFontStyleCommandState.splice(index, 1);
            executeFontStyleCommandState.push(commandName);
        },
        canBeRemoved: function() {
            return false;
        },
        canCheckSelectedElementStyle: function(selection, selectedElement, selectedElements) {
            return selection.IsCollapsed() || selection.GetHtml().length < 2 || selectedElements && selectedElements.length == 0 ||  ASPx.Browser.IE && selection.GetIsControlSelected() || ASPx.HtmlEditorClasses.Utils.needToUseSpecialSelection(selectedElement);
        },
	    IsLocked: function(wrapper) {
	        if(wrapper.isInFocus)
                return ASPx.HtmlEditorClasses.Commands.Browser.Command.prototype.IsLocked.call(this, wrapper);
	        return false;
	    },
        getStateToElement: function(element) {
            return false;
        },
        convertToPixel: function(value) {
            var DPI = 96;
            var result = parseFloat(value);
            if(/in/i.test(value))
                result = DPI * result;
            else if(/pt/i.test(value))
                result = result * DPI / 72;
            else if(/cm/i.test(value))
                result = value / (2.54 / DPI);
            return Math.round(result) + "px";
        },
        replaceTableElements: function(elements) {
            var predicate = function(el) { return /^(td|th)$/i.test(el.nodeName); };
            var result = [];
            for(var i = 0, element; element = elements[i]; i++) {
                if(element.nodeType == 3 && /^(table|tbody|tfoot|thead|tr)$/i.test(element.parentNode.nodeName) && !ASPx.Str.Trim(element.nodeValue))
                    continue;
                if(/^(table|tbody|tfoot|thead|tr|caption|col|colgroup)$/i.test(element.nodeName))
                    result = result.concat(ASPx.HtmlEditorClasses.Utils.getChildsByPredicate(element, predicate, false, true));
                else
                    result.push(element);
            }
            return result;
        },
        skipElement: function(element) {
            if(!element.parentNode)
                return true;
            if(element.nodeType == 3) {
                var nodeValue = ASPx.Str.Trim(element.nodeValue);
                return !nodeValue && /^(ol|ul)$/gi.test(element.parentNode.nodeName);
            }
            return false;
        }
    });

    ASPx.HtmlEditorClasses.Commands.Browser.ApplyStyle = function(commandManager) {
        var typeCommandProcessor = commandManager.typeCommandProcessor;
        var activeWrapper = commandManager.wrapper;
        if(!typeCommandProcessor.hasStyleCommand())
            return;
        var parentWrapper, elementContainer;
        var doc = activeWrapper.getDocument();
        if(typeCommandProcessor.applyStyleCommands.length > 0) {
            var currentWrapper = null;
            for(var i = 0; i < typeCommandProcessor.applyStyleCommands.length; i++) {
                elementContainer = commandManager.getCommand(typeCommandProcessor.applyStyleCommands[i]).CreateStylizedWrapper(doc, activeWrapper.settings.updateDeprecatedElements, activeWrapper.settings.updateBoldItalic, "SPAN");
                if(i == 0)
                    parentWrapper = elementContainer;
                if(currentWrapper)
                    currentWrapper.appendChild(elementContainer);
                currentWrapper = elementContainer;
            }
        }
        var selection = activeWrapper.getSelection();
        var tempSpan = doc.createElement("SPAN");
        tempSpan.id = "tempSpan";
        if(!selection.IsCollapsed()) {
            var range = selection.clientSelection.GetRange();
            range.collapse(false);
            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 11)
                range.select();
        }
        selection.InsertHtml(tempSpan.outerHTML);
        var tempElement = doc.getElementById("tempSpan");
        var previousNode = tempElement.previousSibling;
        var parentNode = tempElement.parentNode;
        if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 9) {
            //core.preventLostFocus = true;
            //core.getDesignViewCell().focus();
            if(!doc.getElementById("tempSpan")) {
                ASPx.InsertElementAfter(tempSpan, previousNode);
                tempElement = doc.getElementById("tempSpan");
            }
        }
        tempSpan = tempElement;

        if(!previousNode || previousNode.nodeType != 3) return;
        var imputText = previousNode.nodeValue.substring(previousNode.nodeValue.length - 1);
        previousNode.nodeValue = previousNode.nodeValue.substring(0, previousNode.nodeValue.length - 1);
        var imputTextNode =  doc.createTextNode(imputText);

        selectionBookmarks = { "startMarkerID": "sbm", "endMarkerID": "ebm"};
        var sbm = doc.createElement("span");
        sbm.id = "sbm";
        var ebm = doc.createElement("span");
        ebm.id = "ebm";

        if(!parentWrapper) {
            parentNode.insertBefore(imputTextNode, tempSpan);
            parentNode.insertBefore(sbm, tempSpan);
            parentNode.insertBefore(ebm, sbm);
        }
        else {
            elementContainer.appendChild(imputTextNode);
            parentNode.insertBefore(parentWrapper, tempSpan);
            elementContainer.appendChild(ebm);
            elementContainer.appendChild(sbm);
        }
        ASPx.RemoveElement(tempSpan);
        for(var i = 0, commandName; commandName = typeCommandProcessor.removeStyleCommands[i]; i++)
            commandManager.getCommand(commandName).Execute(imputTextNode, activeWrapper);

        var node = ASPx.Browser.NetscapeFamily || ASPx.Browser.Opera ? imputTextNode.parentNode : ebm.previousSibling;
        if(node && node.style && (node.style.fontWeight == "normal" || node.style.fontStyle == "normal")) {
            node.appendChild(ebm);
            node.appendChild(sbm);
        }
        activeWrapper.getSelection().clientSelection.SelectExtendedBookmark(selectionBookmarks);
        typeCommandProcessor.clearCommands();
    };
})();