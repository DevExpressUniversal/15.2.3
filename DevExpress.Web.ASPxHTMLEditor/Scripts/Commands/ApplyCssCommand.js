(function() {
    ASPx.HtmlEditorClasses.Commands.ApplyCss = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.SelectionManipulationCommand, {
         // cmdValue = { tagName, cssClass };
        customCssValueHashTable: { },
        Execute: function(value, wrapper) {
	        var cmdValue;
            if(typeof(value) != "string")
                cmdValue = value;
            else if(!value) 
                cmdValue = { tagName: "", cssClass: ""};
            else {
                var valueArray = value.split("|");
                cmdValue = { tagName: valueArray[0], cssClass: valueArray[1]};
            }
            cmdValue.tagName = ASPx.Str.Trim(cmdValue.tagName).toUpperCase();
	        cmdValue.cssClass = ASPx.Str.Trim(cmdValue.cssClass);
            
            var tagIsFiltered = wrapper.tagIsFiltered(cmdValue.tagName);
            var attrIsFiltered = wrapper.attributeIsFiltered("class");
            if(tagIsFiltered && attrIsFiltered || this.IsExclusionTagName(cmdValue.tagName))
                return false;
            if(tagIsFiltered)
                cmdValue.tagName = "";
            else if(attrIsFiltered)
                cmdValue.cssClass = "";
            ASPx.HtmlEditorClasses.Commands.SelectionManipulationCommand.prototype.Execute.apply(this, arguments);

            var selection = wrapper.getSelection();
            var parentElement = selection.GetSelectedElement();
            if(parentElement.nodeType == 3)
                parentElement = parentElement.parentNode;
            var cleanMode = !cmdValue.cssClass && !cmdValue.tagName;
            var getNextSibling = function(element) { return element.nextSibling; };
            var getPreviousSibling = function(element) { return element.previousSibling; };
            var getSibling = function(element, func) { 
                element = func(element);
                while(element && element.nodeType == 3 && (!element.nodeValue || element.nodeValue.indexOf(" ") < 0 && !ASPx.Str.Trim(element.nodeValue)))
                    element = func(element);
                return element;
            };
            if(this.IsExclusionTagName(parentElement.tagName))
                this.SetClassName(parentElement, cmdValue.cssClass);
            else if(selection.GetHtml()) {
                var elements = selection.GetElements();
                if(cmdValue.tagName == "LI") {
                    if(!ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParentList(parentElement)) {
                        this.prepareArray(elements);
                        this.wrapElementToList(wrapper, elements);
                        elements = selection.GetElements();
                    }
                    this.appendClassNameToListElements(elements, cmdValue.cssClass);
                }
                else {
                    for(var i = 0, el; el = elements[i]; i++) {
                        if(el.nodeType == 3 && !el.nodeValue)
                            continue;
                        var el = this.getParentNode(el);
                        if(cleanMode)
                            this.CleanFormat(el.nodeType == 3 && !ASPx.HtmlEditorClasses.Commands.Utils.IsInlineTextElement(el.parentNode) && !getSibling(el, getPreviousSibling) && !getSibling(el, getNextSibling) ? el.parentNode : el);
                        else if(!this.GetEqualParentElement(el.nodeType == 3 ? el.parentNode : el, cmdValue.tagName, cmdValue.cssClass))
                            this.ApplyFormat(el, cmdValue.cssClass, cmdValue.tagName);
                    }
                }
            }
            setTimeout(function() {
                if(!wrapper.isSelectionRestored())
                    wrapper.restoreSelection();
            }.aspxBind(this), 300);
            return true;
        },
        GetValue: function(wrapper, selection, selectedElements) {
            var element;
            if(wrapper.isInFocus) {
                selection = !selection ? wrapper.getSelection() : selection;
                element = selection.GetSelectedElement();
            }
            var tagName = "";
	        var cssClass = "";

            if (element && element.nodeType == 1 && element.tagName != "BODY") {
			    tagName = element.tagName.toLowerCase();
			    cssClass = element.className;
		    }
	        var result = !tagName && !cssClass ? null : tagName + "|" + cssClass;
            return !this.customCssValueHashTable[result] ? this.GetDefaultValue(wrapper) : result;
	    },

        CleanFormat: function(el) {
            var parent = el.parentNode;
            if(el.nodeType == 3 && parent.className && ASPx.HtmlEditorClasses.Commands.Utils.IsInlineTextElement(parent))
                this.SeparateParent(el, "", true);
            else if(el.nodeType == 1) {
                this.CleanFormatInDescendants(el);
                var prevSibling = el.previousSibling;
                if(el.previousSibling && prevSibling.tagName == el.tagName && !prevSibling.className && !this.isBlockElement(prevSibling) && !this.isBlockElement(el)) {
                    for(var child = el.firstChild; child; child = el.firstChild) {
                        el.previousSibling.appendChild(child);
                    }
                    ASPx.RemoveElement(el);
                }   
            }
        },
        CleanFormatInDescendants: function(el) {
            if(el.nodeType == 1) {
                this.SetClassName(el, "");
                for(var child = el.firstChild; child; child = child.nextSibling) {
                    this.CleanFormatInDescendants(child);
                }
            }
        },
        ApplyFormat: function(el, className, tagName) {
            if(el.nodeType == 1 && (!tagName || el.tagName == tagName))
                this.AppendClassName(el, className);
            else
                this.ApplyWrapperFormat(el, !tagName ? "SPAN" : tagName, className);
        },
        ApplyWrapperFormat: function(el, tagName, className) {
            if(this.FindNotNestedNode(el) && this.IsCantBeNested(tagName)) {
                var _this = this;
                var predicate = function(target) { return !_this.IsCantBeNested(target); };
                var nnElement = this.SeparateParentByPredicate(el, predicate);
                this.WrapElement(nnElement, tagName, className);
                for(var child = nnElement.firstChild; child; child = nnElement.firstChild) {
                    nnElement.parentNode.insertBefore(child, nnElement);
                }
                ASPx.RemoveElement(nnElement);
            }
            else if(ASPx.HtmlEditorClasses.Commands.Utils.IsInlineTextElement(el.nodeType == 3 ? el.parentNode : el)) {
                var predicate = function(target) { return !ASPx.HtmlEditorClasses.Commands.Utils.IsInlineTextElement(target.parentNode); };
                el = ASPx.HtmlEditorClasses.Commands.Utils.IsInlineTextElement(tagName) ? el : this.SeparateParentByPredicate(el, predicate);
                this.WrapElement(el, tagName, className);
            }
            else if(el.nodeType == 3)
                this.WrapElement(el, tagName, className);
            else
                this.ApplyFormatToInlineChilds(el, tagName, className);
        },
        AppendClassName: function(target, className) {
            if(className && target.className.indexOf(className) == -1) {
                if(target.className.length > 0)
                    className = " " + className; 
                target.className += className;
            }
        },
        WrapElement: function(target, tagName, className) {
            var wrapper = target.ownerDocument.createElement(tagName);
            this.SetClassName(wrapper, className);
            this.WrapElementInternal(target, wrapper);
        },
        ApplyFormatToInlineChilds: function(el, tagName, className) {
            var child = el.firstChild;
            while(child) {
                var next = child.nextSibling;
                if(child.nodeType == 3)
                    this.WrapElement(child, tagName, className);
                else if(child.tagName == tagName && child.className.indexOf(className) < 0)
                    this.AppendClassName(child, className);
                else if(ASPx.HtmlEditorClasses.Commands.Utils.IsInlineTextElement(child))
                    this.WrapElement(child, tagName, className);
                else
                    this.ApplyFormatToInlineChilds(child, tagName, className);
                child = next;
            }
        },
        IsCantBeNested: function(el) {
            var expr = /^(H[1-6]|P|ADDRESS|BLOCKQUOTE|PRE|A|DIV)$/i;
            return expr.test(typeof(el) == "object" ? el.tagName : el);
        },
        IsExclusionTagName: function(tagName) {
	        tagName = tagName.toUpperCase();
	        return tagName == "IMG" || tagName == "TABLE" || tagName == "OBJECT" || tagName == "EMBED";
	    },
        GetEqualParentElement: function(elem, tagName, cssClass) {
            if (tagName && cssClass)
		        return ASPx.GetParentByTagNameAndAttributeValue(elem, tagName, "class", cssClass);
		    else if (cssClass)
		        return ASPx.GetParentByPartialClassName(elem, cssClass);
	    },
        FindNotNestedNode: function(el) {
            if(!el || el.tagName == "BODY")
                return null;
            if(el.nodeType == 3)
                el = el.parentNode;
            if(this.IsCantBeNested(el))
                return el;
            return this.FindNotNestedNode(el.parentNode);
        },
        SkipStateOnFirstLoad: function() {
            return true;
        },
	    IsLocked: function(wrapper) {
	        return false;
	    },
        setCustomCssValueHashTable: function(value) {
            this.customCssValueHashTable = value;
        },
        canBeExecutedOnSelection: function(wrapper, curSelection) {
            return true;
        },
        wrapElementToList: function(wrapper, elements) {
            var doc = wrapper.getDocument();
            var processingElements = ASPx.HtmlEditorClasses.Utils.getBeforeBlockParentElements(elements);
            for(var i = 0, element; element = processingElements[i]; i++) {
                if(!ASPx.HtmlEditorClasses.Commands.Utils.IsBlockElement(element)) {
                    var blockElement = doc.createElement("DIV");
                    element.parentNode.insertBefore(blockElement, element);
                    blockElement.appendChild(element);
                    _aspxArrayRemoveAt(processingElements, i);
                    _aspxArrayInsert(processingElements, blockElement, i);
                }
            }
            var firstElement = processingElements[0];
            var lastElement = processingElements[processingElements.length - 1];
            var bookmark = ASPx.HtmlEditorClasses.Selection.getBookmarkByElements(doc, firstElement.firstChild, lastElement.lastChild);
            wrapper.getSelection().clientSelection.SelectExtendedBookmark(bookmark);
            wrapper.commandManager.executeCommand(ASPxClientCommandConsts.INSERTORDEREDLIST_COMMAND);
        },
        appendClassNameToListElements: function(elements, className) {
            var predicate = function(element) { return element.nodeType == 1 && element.nodeName == "LI"; };
            for(var i = 0, element; element = elements[i]; i++) {
                var listElements = [];
                if(element.nodeType == 1 && element.nodeName == "LI")
                    listElements.push(element);
                else {
                    listElements = ASPx.HtmlEditorClasses.Utils.getChildsByPredicate(element, predicate);
                    if(listElements.length == 0) {
                        var parent = _aspxGetParentByTagName(element, "LI");
                        if(parent.nodeName == "LI")
                            listElements.push(parent);
                    }
                }
                for(var j = 0, listItem; listItem = listElements[j]; j++)
                    listItem.className = className;
            }
        },
        prepareArray: function(elements) {
            for(var i = 0, element; element = elements[i]; i++) {
                if(element.nodeType == 3 && ASPx.HtmlEditorClasses.Commands.Utils.IsBlockElement(element.parentNode)) {
                    var element = ASPx.HtmlEditorClasses.Utils.splitParentElement(element.parentNode, element, element);
                    _aspxArrayRemoveAt(elements, i);
                    _aspxArrayInsert(elements, element, i);
                }
            }
        },
        getParentNode: function(el) {
            var getNextSibling = function(element) { return element.nextSibling; };
            var getPreviousSibling = function(element) { return element.previousSibling; };
            var processingNodeValue = function(value) { 
                if(value) {
                    if(/\r|\n/g.test(value))
                        value = value.replace(/\s*(\r|\n)*\s*/ig, "");
                    else if(/\s/g.test(value) && value.length == 1)
                        value = "";
                }
                return value; 
            };
            var getSibling = function(element, func) { 
                element = func(element);
                while(element && (element.nodeType != 1 && !processingNodeValue(element.nodeValue) || element.nodeType == 1 && element.nodeName == "BR"))
                    element = func(element);
                return element;
            };
            var compare = function(el) { return el.parentNode.nodeName == "BODY" || getSibling(el, getNextSibling) || getSibling(el, getPreviousSibling); };
            return ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParent(el, compare);
        },
        canBeExecuted: function() {
            return true;
        }
    });
})();