(function() {
    ASPx.HtmlEditorClasses.Commands.Browser.FontStyle = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Browser.WrappedCommand, {
        ExecuteCore: function(cmdValue, wrapper) {
            var doc = wrapper.getDocument();
            var selection = wrapper.getSelection();
            var selectedElements = cmdValue ? [ cmdValue ] : selection.GetElements(true);
            if(selectedElements.length > 0) {
                selectedElements = this.replaceTableElements(selectedElements);
                var firstElement = selectedElements[0];
                var lastElement = selectedElements[selectedElements.length - 1];
                var selectionBookmarks = ASPx.HtmlEditorClasses.Commands.Utils.createBookmarkElements(doc, firstElement, lastElement);

                var selectedUnwrappedElements = this.GetNotStylizedElements(wrapper, selectedElements, selectionBookmarks, true);
                var unwrappedElements = this.RemoveFontStyleParent(wrapper, firstElement, selectionBookmarks);

                if(selectedUnwrappedElements.length > 0) {
                    unwrappedElements = unwrappedElements.concat(selectedUnwrappedElements);
                    unwrappedElements = unwrappedElements.concat(this.RemoveFontStyleParent(wrapper, lastElement, selectionBookmarks));
                    this.StylizeElements(wrapper, unwrappedElements, selectionBookmarks, false);
                } 
                else {
                    unwrappedElements = unwrappedElements.concat(this.RemoveFontStyleParent(wrapper, lastElement, selectionBookmarks));
                    if(unwrappedElements.length > 0) {
                        this.StylizeElements(wrapper, unwrappedElements, selectionBookmarks, false);
                        selectedElements = this.UpdateSelectionElements(selectedElements);
                    }
                    if(this.ContainsNotDOMElements(selectedElements))
                        selectedElements = ASPx.HtmlEditorClasses.Utils.ElementsSelectionHelper.GetSelectedElements(doc, selectionBookmarks.startMarkerID, selectionBookmarks.endMarkerID, true);
                    this.RemoveStyle(wrapper, selectedElements, selectionBookmarks, true);
                }
                selection.clientSelection.SelectExtendedBookmark(selectionBookmarks);
            }
            else
                this.ExecuteCommandLater(wrapper, selection);
            return true;
        },
        GetStateCore: function(wrapper, selection, selectedParent, selectedElements) {
            var typeCommandProcessor = this.owner.typeCommandProcessor;
            if(typeCommandProcessor.hasApplyStyleCommand(this.GetCommandName()))
                return true;
            else if(typeCommandProcessor.hasRemoveStyleCommand(this.GetCommandName()))
                return false;
            if(this.canCheckSelectedElementStyle(selection, selectedParent, selectedElements))
                return this.IsElementStylized(selectedParent) || (selectedParent.nodeName != "BODY" && this.IsParentElementStylized(selectedParent));
            return this.GetStateInternal(wrapper, selection, selectedElements);
        },
        FindParentFontStyleElement: function(element) {
            if(this.NeedFindFontStyleItalicParent())
                return this.FindFontStyleItalicParent(element);
            if(this.IsCommandTextDecoration())
                return this.FindTextDecorationParent(element);
            return ASPx.HtmlEditorClasses.Commands.Utils.FindParent(element, function(parent) {
                return this.IsElementStylized(parent);
            }.aspxBind(this));
        },
        NeedChangeRemoveStyleCommandState: function() {
            return true;
        },
        UpdateSelectionElements: function(elements) {
            var newElements = [];
            for(var i = 0, element; element = elements[i]; i++) {
                if((!element.id || ASPx.HtmlEditorClasses.Utils.needToUseSpecialSelection(element)) && this.IsElementStylized(element.parentNode) && this.CanBeWrapped(element.parentNode)) {
                    if(newElements.length > 0 && newElements[newElements.length - 1] == element.parentNode)
                        continue;
                    newElements.push(element.parentNode);
                } 
                else
                    newElements.push(element);
            }
            return newElements;
        },
        RemoveFontStyleParent: function(wrapper, element, selectionBookmarks) {
            if(!element.parentNode)
                return childElements;
            var childElements = [];
            var parentFontStyleElement = this.FindParentFontStyleElement(element);
            if(parentFontStyleElement && parentFontStyleElement.nodeName != "BODY") {
                for(var child = parentFontStyleElement.firstChild; child; child = child.nextSibling)
                    childElements.push(child);
                if(this.GetStyleAttributeValue(parentFontStyleElement) == this.GetStyleAttributeConstValue()) {
                    this.SetStyleAttribute(parentFontStyleElement, "");
                    if(!this.IsFontOrSpanElement(parentFontStyleElement) && !this.IsElementStylized(parentFontStyleElement))
                        return this.GetCanBeWrappedElements(wrapper, childElements, selectionBookmarks);
                }
                if(ASPx.HEBlockElements[parentFontStyleElement.nodeName.toLowerCase()] && this.IsElementStylized(parentFontStyleElement))
                    return [];
                if(this.IsTagFontStyle(parentFontStyleElement) || (!this.IsExistAttributes(parentFontStyleElement) && !this.CanNotBeRemove(parentFontStyleElement))) {
                    if(this.IsExistAttributes(parentFontStyleElement)) {
                        var doc = wrapper.getDocument();
                        ASPx.ReplaceTagName(parentFontStyleElement, "SPAN", false);
                    }
                    else {
                        for(var i = 0, child; child = childElements[i]; i++)
                            parentFontStyleElement.parentNode.insertBefore(child, parentFontStyleElement);
                        parentFontStyleElement.parentNode.removeChild(parentFontStyleElement);
                    }
                }
            }
            return this.GetCanBeWrappedElements(wrapper, childElements, selectionBookmarks);
        },
        RemoveStyle: function(wrapper, elements, selectionBookmarks, removeFontStyleInHierarchy) {
            var doc = wrapper.getDocument();
            for(var i = 0, element; element = elements[i]; i++) {
                var childElements = [];
                if(element.nodeType == 1) {
                    if(this.IsTagFontStyle(element)) {
                        if(!this.IsExistAttributes(element))
                            childElements = this.UnwrapElement(wrapper, element, selectionBookmarks, removeFontStyleInHierarchy);
                        else
                            element = ASPx.ReplaceTagName(element, "SPAN", false);
                    }
                    if(this.GetStyleAttributeValue(element) == this.GetStyleAttributeConstValue() || this.IsStyleAttributeNormal(element)) {
                        this.SetStyleAttribute(element, "");
                        if(!this.IsExistAttributes(element) && this.IsFontOrSpanElement(element))
                            childElements = this.UnwrapElement(wrapper, element, selectionBookmarks, removeFontStyleInHierarchy);
                    }
                }
                if(element.parentNode) {
                    if(this.IsElementStylized(element) && this.CanBeWrapped(element))
                        this.StylizeElements(wrapper, [element], selectionBookmarks, true);
                    if(removeFontStyleInHierarchy) {
                        childElements = [];
                        for(var child = element.firstChild; child; child = child.nextSibling)
                            childElements.push(child);
                        this.RemoveStyle(wrapper, childElements, selectionBookmarks, removeFontStyleInHierarchy);
                    }
                } 
                else if(childElements.length > 0) {
                    for(var j = 0, child; child = childElements[j]; j++) {
                        if(child.parentNode && this.IsElementStylized(child.parentNode)) {
                            this.StylizeElements(wrapper, childElements, selectionBookmarks, true);
                            break;
                        }
                    }
                }
            }
        },
        GetFontStyleTagName: function (updateBoldItalic) {
            switch(this.GetCommandName()) {
                case ASPxClientCommandConsts.BOLD_COMMAND:
                    return updateBoldItalic ? "STRONG" : "B";
                case ASPxClientCommandConsts.ITALIC_COMMAND:
                    return updateBoldItalic ? "EM" : "I";
                case ASPxClientCommandConsts.UNDERLINE_COMMAND:
                    return "U";
                case ASPxClientCommandConsts.STRIKETHROUGH_COMMAND:
                    return "S";
            }
        },
        IsTextDecorationAttributeValue: function(attributeValue) {
            return attributeValue.indexOf(this.GetStyleAttributeConstValue()) > -1;
        },
        IsStyleAttributeNormal: function(element) {
            return this.IsFontOrSpanElement(element) && this.GetStyleAttributeValue(element) == "normal";
        },
        IsTagFontStyle: function(element) {
            if(this.IsCommandTextDecoration()) {
                var att = this.GetStyleAttributeValue(element);
                return (this.IsFontOrSpanElement(element) && this.IsTextDecorationAttributeValue(att) == this.GetStyleAttributeConstValue()) ||
                       element.nodeName == this.GetFontStyleTagName();
            }
            else
                return element.nodeName == this.GetFontStyleTagName(true) || element.nodeName == this.GetFontStyleTagName(false);
        },
        IsElementStylized: function(element) {
            if(!element.parentNode)
                return;
            element = element.nodeType == 1 ? element : element.parentNode;
            if(this.IsTagFontStyle(element))
                return true;
            if(this.GetCommandName() == ASPxClientCommandConsts.BOLD_COMMAND && (element.className == ASPx.HtmlEditorClasses.PlaceholderStartMarkCssClasseName || element.className == ASPx.HtmlEditorClasses.PlaceholderEndMarkCssClasseName))
                element = element.parentNode;
            var attr = this.GetStyleAttribute();
            var attributeValue =  ASPx.GetCurrentStyle(element)[attr];
            if(this.GetCommandName() == ASPxClientCommandConsts.BOLD_COMMAND && (ASPx.Browser.NetscapeFamily || ASPx.Browser.IE || ASPx.Browser.Opera))
                return attributeValue != 400; // 400: normal font-weight value; 700: bold font-weight
            if(this.IsCommandTextDecoration())
                return this.IsTextDecorationAttributeValue(attributeValue);
            return attributeValue == this.GetStyleAttributeConstValue();
        },
        CreateStylizedWrapper: function(doc, updateDeprecatedElements, updateBoldItalic, tagName) {
            var newElement;
            if(this.IsCommandTextDecoration() && updateDeprecatedElements) {
                newElement = doc.createElement(tagName);
                this.SetStyleAttribute(newElement, this.GetStyleAttributeConstValue());
            }
            else
                newElement = doc.createElement(this.GetFontStyleTagName(updateBoldItalic));
            return newElement;
        },
        ChangeExecuteStyleCommandState: function(commandName, executeFontStyleCommandState) {
            var index = ASPx.Data.ArrayIndexOf(executeFontStyleCommandState, commandName);
            if(index > -1)
                executeFontStyleCommandState.splice(index, 1);
            else
                executeFontStyleCommandState.push(commandName);
        },
        canBeRemoved: function() {
            return true;
        },
        GetStyleAttribute: function() {
            if(ASPxClientCommandConsts.BOLD_COMMAND == this.GetCommandName())
                return "fontWeight";
            else if(ASPxClientCommandConsts.ITALIC_COMMAND == this.GetCommandName())
                return "fontStyle";
            else if(this.IsCommandTextDecoration())
                return "textDecoration";
        },
        GetStyleAttributeConstValue: function() {
            switch (this.GetCommandName()) {
                case ASPxClientCommandConsts.BOLD_COMMAND:
                    return "bold";
                case ASPxClientCommandConsts.ITALIC_COMMAND:
                    return "italic";
                case ASPxClientCommandConsts.UNDERLINE_COMMAND:
                    return "underline";
                case ASPxClientCommandConsts.STRIKETHROUGH_COMMAND:
                    return "line-through";
            }
        },
        getStateToElement: function(element) {
            return this.IsElementStylized(element) || this.IsParentElementStylized(element);
        }
    });
})();