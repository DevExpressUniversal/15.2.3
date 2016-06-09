(function() {
    var DesignTabName = "Design";
    var IDConstants = {
        Wrapper: "_TI",
        TagsContainer: "_TITC",
        ControlsWrapper: "_TICW",
        RemoveButton: "_TITRB",
        PropertiesButton: "_TITPB"
    };
    var CssClasses = {
        TagSeparator: "dxhe-tiSeparator",
        TagWrapper: "dxhe-tiTagWrapper"
    };

    function getTagListCurrentWidth(tagsContainer, controlsWrapper) {
        var tagListCurrentWidth = 0;
        for(var i = 0; i < tagsContainer.childNodes.length; i++) {
            var element = tagsContainer.childNodes[i];
            if(element != controlsWrapper && element.nodeType == 1)
                tagListCurrentWidth += element.offsetWidth;
        }
        return tagListCurrentWidth;
    };
    function getElementsToHideInternal(collection, maxSize, currentSize, leftIndex, rightIndex, useLeft, elementsToHide) {
        if(currentSize > maxSize) {
            var index = useLeft ? leftIndex-- : rightIndex++;
            var element = collection[index];
            if(element) {
                element.indexInArray = index;
                elementsToHide.push(element);
                currentSize -= element.offsetWidth;
                return getElementsToHideInternal(collection, maxSize, currentSize, leftIndex, rightIndex, !useLeft, elementsToHide);
            }
        }
        elementsToHide.sort(function(first, second) {
            return first.indexInArray < second.indexInArray ? -1 : 1;    
        });
        return elementsToHide;
    };
    function getElementsToHide(collection, maxSize, currentSize) {
        var middleIndex = (collection.length - 1) / 2;
        var leftIndex = Math.floor(middleIndex);
        var rightIndex = Math.ceil(middleIndex);
        if(leftIndex == rightIndex)
            leftIndex--;
        return getElementsToHideInternal(collection, maxSize, currentSize, leftIndex, rightIndex, false, []);
    };
    function reduceTagList(wrapper, tagsContainer, controlsWrapper, ellipsis) {
        var tagListCurrentWidth = getTagListCurrentWidth(tagsContainer, controlsWrapper);
        var tagListMaxWidth = getWidth(wrapper) - controlsWrapper.offsetWidth;
        if(tagListCurrentWidth <= tagListMaxWidth)
            return;
        
        tagsContainer.appendChild(ellipsis);
        var tagCollection = ASPx.GetChildNodes(tagsContainer, function(child) { 
            return child != controlsWrapper && child != ellipsis && child.nodeType == 1; 
        });
        var elementsToHide = getElementsToHide(tagCollection, tagListMaxWidth, tagListCurrentWidth + ellipsis.offsetWidth);
        if(elementsToHide.length) {
            var firstElement = elementsToHide[0];
            firstElement.parentNode.insertBefore(ellipsis, firstElement);
            ASPx.Evt.AttachEventToElement(ellipsis, "click", function() {
                var tagElement = ellipsis.nextSibling;
                ellipsis.parentNode.removeChild(ellipsis);
                while(tagElement.style.display == "none") {
                    tagElement.style.display = "";
                    tagElement = tagElement.nextSibling;    
                }
            });
            for(var i = 0; i < elementsToHide.length; i++)
                elementsToHide[i].style.display = "none";
        }
    };
    function iterateCollection(collection, startFromTail, callback) {
        var index = startFromTail ? collection.length - 1 : 0;
        while(index < collection.length && index >= 0) {
            var result = callback(collection[index]);
            if(result)
                return result;
            startFromTail? index-- : index++;
        }
    };
    function getChildForBookmark(parent, selectLast) {
        if(parent) {
            switch(parent.tagName) {
                case "TABLE":
                case "TBODY":
                case "THEAD":
                case "TFOOT":
                case "TR":
                    var childNodes = ASPx.GetNodesByTagName(parent, "TD");
                    return getChildForBookmark(childNodes[selectLast ? childNodes.length - 1 : 0], selectLast);
                default:
                    var childNodes = ASPx.GetChildNodes(parent);
                    return childNodes && (childNodes[selectLast ? childNodes.length - 1 : 0]);
            }
        }
    };
    function getElementToSelect(element) {
        if(element) {
            switch(element.tagName) {
                case "THEAD":
                case "TBODY":
                case "TFOOT":
                    return element.parentNode;
            }
        }
        return element;
    };

    function getWidth(element) {
        var style = ASPx.GetCurrentStyle(element);
        var width = element.offsetWidth;
        width -= ASPx.PxToInt(style.paddingLeft);
        width -= ASPx.PxToInt(style.paddingRight);
        return width;
    };
    
    function getChild(element, position) {
        return selectChild(element, position, ASPx.GetChildElementNodes);
    };
    function getChildByTagName(element, tagName, position) {
        return selectChild(element, position, function(el) { return ASPx.GetNodesByTagName(el, tagName); });
    };
    function selectChild(element, position, selectFunc) {
        if(!element) return null;
        var nodes = selectFunc(element);
        return !!nodes && !!nodes.length && nodes[ASPx.IsExists(position) ? Math.min(position, nodes.length -1) : 0];
    };

    ASPx.HtmlEditorClasses.Controls.TagInspector = ASPx.CreateClass(null, {
        constructor: function(htmlEditor) {
            this.htmlEditor = htmlEditor;
            this.styles = this.htmlEditor.tagInspectorStyles;
            this.hoverStyles = this.htmlEditor.tagInspectorStyles.selectionStyle;
            this.wrapper = null;
            this.currentElement = null;
            this.cache = {};
            this.isActive = false;
        },
        initialize: function() {
            ASPx.Selection.SetElementAsUnselectable(this.getInnerElement(IDConstants.Wrapper), true, true);
            this.adjustControl();
            this.assignHandlers();
        },
        setActive: function(value) {
            this.isActive = value;
        },
        assignHandlers: function() {
            this.htmlEditor.ActiveTabChanged.AddHandler(function(s, e) {
                this.adjustControl();
            }.aspxBind(this));
            this.htmlEditor.GotFocus.AddHandler(function(s, e) {
                this.setActive(true);
                this.updateElementTree();
            }.aspxBind(this));
            this.htmlEditor.LostFocus.AddHandler(function(s, e) {
                this.setActive(false);
            }.aspxBind(this));
            this.getInnerControl(IDConstants.RemoveButton).Click.AddHandler(function(s, e) {
                this.removeElement();
            }.aspxBind(this));
            this.getInnerControl(IDConstants.PropertiesButton).Click.AddHandler(function(s, e) {
                this.showPropertiesDialog();
            }.aspxBind(this));    
        },
        adjustControl: function() {
            if(this.htmlEditor.GetActiveTabName() != DesignTabName)
                return;
            this.updateElementTree();
        },
        showPropertiesDialog: function() {
            this.htmlEditor.ExecuteCommandInternal(ASPxClientCommandConsts.CHANGEELEMENTPROPERTIES_DIALOG_COMMAND, { selectedElement: this.currentElement });
        },
        removeElement: function() {
            var state = this.beforeRemoveElement(this.currentElement);
            this.htmlEditor.ExecuteCommandInternal(ASPxClientCommandConsts.DELETEELEMENT_COMMAND, this.currentElement);
            this.afterRemoveElement(state);
        },
        beforeRemoveElement: function(element) {
            var state = {
                parentId: ASPx.HtmlEditorClasses.Selection.CreateUniqueID(),
                tagName: element.tagName,
                getElement: function(id) {
                    var result = element.ownerDocument.getElementById(id);
                    if(result)
                        ASPx.Attr.RestoreAttribute(result, "id");
                    return result;
                }
            };
            if(element.parentNode)
                ASPx.Attr.ChangeAttribute(element.parentNode, "id", state.parentId);
            switch(element.tagName) {
                case "TD":
                    this.prepareTDState(element, state);
                    break;
                case "TR":
                    this.prepareTRState(element, state);
                    break;
                case "TABLE":
                    this.prepareTABLEState(element, state);
                    break;
            }
            return state;
        },
        prepareTDState: function(td, state) {
            state.rowId = ASPx.HtmlEditorClasses.Selection.CreateUniqueID();
            state.tdPosition = ASPx.Data.ArrayIndexOf(ASPx.GetChildElementNodes(td.parentNode), td);
            ASPx.Attr.ChangeAttribute(td.parentNode, "id", state.rowId);
            this.prepareTRState(td.parentNode, state);
        },
        prepareTRState: function(tr, state) {
            if(!ASPx.IsExists(state.tdPosition))
                this.tryToSetTDPosition(tr, state);
            state.trPosition = ASPx.Data.ArrayIndexOf(ASPx.HtmlEditorClasses.Utils.getTableRows(ASPx.GetParentByTagName(tr, "TABLE")), tr);
            this.prepareTABLEState(ASPx.GetParentByTagName(tr, "TABLE"), state);
        },
        tryToSetTDPosition: function(tr, state) {
            var td = ASPx.HtmlEditorClasses.Commands.Tables.Cell.Get(this.htmlEditor.getDesignViewWrapper());
            if(td)
                state.tdPosition = ASPx.Data.ArrayIndexOf(ASPx.GetChildNodesByTagName(tr, "TD"), td);
        },
        prepareTABLEState: function(table, state) {
            state.tableId = ASPx.HtmlEditorClasses.Selection.CreateUniqueID();
            state.siblingId = ASPx.HtmlEditorClasses.Selection.CreateUniqueID();
            state.containerId = ASPx.HtmlEditorClasses.Selection.CreateUniqueID();

            ASPx.Attr.ChangeAttribute(table, "id", state.tableId);
            ASPx.Attr.ChangeAttribute(table.parentNode, "id", state.containerId);
            var nextElementSibling = ASPx.HtmlEditorClasses.Utils.getNextElementSibling(table);
            if(nextElementSibling)
                ASPx.Attr.ChangeAttribute(nextElementSibling, "id", state.siblingId);
        },
        afterRemoveElement: function(state) {
            var elementToSelect = null;
            switch(state.tagName) {
                case "TD":
                    elementToSelect = this.processTDDelete(state);
                    break;
                case "TR":
                    elementToSelect = this.processTRDelete(state);
                    break;
                case "TABLE":
                    elementToSelect = this.processTABLEDelete(state);
                    break;
                default:
                    elementToSelect = state.getElement(state.parentId);
                    break;
            }
            this.buildElementTree(this.isBodyElement(elementToSelect) ? null : elementToSelect);
        },
        processTDDelete: function(state) {
            return this.processTABLEDelete(state, this.processTRDelete(state, 
                this.getElementToSelect(state, state.rowId, null, function(row) {
                    return getChild(row, state.tdPosition);
                })));
        },
        processTRDelete: function(state) {
            return this.processTABLEDelete(state, 
                this.getElementToSelect(state, state.tableId, null, function(table) {
                    return getChild(getChildByTagName(table, "TR", state.trPosition), state.tdPosition);
                }));
        },
        processTABLEDelete: function(state, elementToSelect) {
            return this.getElementToSelect(state, state.containerId,
                        this.getElementToSelect(state, state.siblingId, elementToSelect));
        },
        getElementToSelect: function(state, id, prevElementToSelect, customGetter) {
            var element = state.getElement(id);
            customGetter = customGetter || function(element) { return element; };
            if(element && !prevElementToSelect)
                prevElementToSelect = customGetter(element) || prevElementToSelect;
            return prevElementToSelect;
        },
        updateElementTree: function() {
            this.buildElementTree(this.getSelectedElement());
        },
        buildElementTree: function(selectedElement) {
            this.currentElement = selectedElement;
            this.clearTagInspector();
            if (selectedElement) {
                this.createSelectedTag(selectedElement);
                if(this.createTagHierarchy(selectedElement.parentNode, this.getControlWrapper()))
                    this.reduceTagList();
            }
            this.updateControls(!!selectedElement);
        },
        reduceTagList: function() {
            reduceTagList(this.htmlEditor.getTagInspectorWrapperElement(), this.getTagsContainer(), this.getControlWrapper(), this.createEllipsis());    
        },
        updateControls: function(showControls) {
            ASPx.SetElementVisibility(this.getControlWrapper(), showControls);
        },
        createEllipsis: function() {
            var ellipsis = document.createElement("DIV");
            ellipsis.innerHTML = "...";
            ASPx.SetStyles(ellipsis, this.styles.ellipsisStyle);
            ellipsis.appendChild(this.createSeparator());
            return ellipsis;
        },
        createSeparator: function() {
            var result = document.createElement("SPAN");
            ASPx.AddClassNameToElement(result, CssClasses.TagSeparator);
            result.innerHTML = this.htmlEditor.tagInspectorSeparatorImageHtml;
            return result;
        },
        createElement: function(source) {
            var result = document.createElement("DIV");
            var link = this.createTagLink(source, this.styles.tagStyle);
            ASPx.Evt.AttachEventToElement(result, "click", function() {
                this.removeHighlightFromElement(source);  
                this.setSelection(source);
            }.aspxBind(this));
            result.appendChild(this.createTagLink(source, this.styles.tagStyle));
            ASPx.AddClassNameToElement(result, CssClasses.TagWrapper);
            result.appendChild(this.createSeparator());
            return result;
        },
        createTagHierarchy: function(currentTag, prevElement, result) {
            if(!currentTag)
                return result;
            var flag = this.isSystemElement(currentTag) ? "SystemElement" : currentTag.tagName;
            switch (flag) {
                case "THEAD":
                case "TBODY":
                case "TFOOT":
                case "SystemElement":
                    return this.createTagHierarchy(currentTag.parentNode, prevElement, result);
                case "BODY":
                    return result;
                default:
                    var newChild = this.createElement(currentTag);
                    prevElement.parentNode.insertBefore(newChild, prevElement);
                    return this.createTagHierarchy(currentTag.parentNode, newChild, true);
            }
        },
        createSelectedTag: function(source) {   
            var link = this.createTagLink(source, this.styles.selectedTagStyle);
            var container = this.getSelectedTagContainer();
            container.innerHTML = "";
            container.appendChild(link);
        },
        createTagLink: function(source, style) {
            var link = document.createElement("A");
            link.href = ASPx.AccessibilityEmptyUrl;
            link.innerHTML = ASPx.HtmlEditorClasses.Utils.getTagName(source);
            ASPx.SetStyles(link, style);
            ASPx.Evt.AttachEventToElement(link, "mouseover", function() {
                ASPx.SetStyles(link, style.hoverStyle);
                addHighlightToElement(source, this.htmlEditor.getDesignViewWrapper(), this.hoverStyles);
            }.aspxBind(this));
            ASPx.Evt.AttachEventToElement(link, "mouseout", function() {
                ASPx.SetStyles(link, style); 
                this.removeHighlightFromElement(source);  
            }.aspxBind(this));
            return link;
        },
        setSelection: function (elementToSelect) {
            var wrapper = this.htmlEditor.core.getActiveWrapper();
            wrapper.focus();
            this.buildElementTree(elementToSelect);
            setTimeout(function () {
                this.htmlEditor.GetSelection().clientSelection.SelectExtendedBookmark(this.getBookmarks(elementToSelect));
                this.htmlEditor.RaiseSelectionChanged();
            }.aspxBind(this), 0);
        },
        getBookmarks: function(element) {
            var elementToSelect = getElementToSelect(element);
            var extremeLeft =  getChildForBookmark(elementToSelect);
            var extremeRight = getChildForBookmark(elementToSelect, true);
            var bookmarks = ASPx.HtmlEditorClasses.Selection.getBookmarkByElements(elementToSelect.ownerDocument, extremeLeft, extremeRight);
            return bookmarks;
        },
        removeHighlightFromElement: function(element) {
            if(element.overlay) {
                ASPx.RemoveElement(element.overlay);
                delete element.overlay;
            }
        },
        clearTagInspector: function() {
            var container = this.getTagsContainer();
            var wrapper = this.getControlWrapper();
            while(container.firstChild && container.firstChild != wrapper)
                container.removeChild(container.firstChild);
        },
        getSelectedElement: function() {
            if(this.isActive) {
                var selectedElement = this.htmlEditor.GetSelection().GetSelectedElement();
                if (this.isSystemElement(selectedElement) && selectedElement.parentNode) {
                    selectedElement = selectedElement.parentNode;
                }
                if (this.isEditableElement(selectedElement)) {
                    return selectedElement;
                }
            }
            return null;
        },
        isSystemElement: function(el) {
            var isSystemSearchElement = (el.tagName == "SPAN" &&
                (ASPx.ElementHasCssClass(el, ASPx.HtmlEditorClasses.Search.CssClasses.Highlight)
                || (ASPx.ElementHasCssClass(el, ASPx.HtmlEditorClasses.Search.CssClasses.Background))));
            return isSystemSearchElement;
        },
        isEditableElement: function (el) {
            return !this.isBodyElement(el) && !this.isSystemElement(el);
        },
        isBodyElement: function(el) {
            return el && el.tagName == "BODY";   
        },
        /* inner entities */
        getControlWrapper: function() {
            return this.getInnerElement(IDConstants.ControlsWrapper);        
        },
        getSelectedTagContainer: function() {
            return ASPx.GetChildElementNodes(this.getControlWrapper())[0];
        },
        getTagsContainer: function() {
            return this.getInnerElement(IDConstants.TagsContainer);    
        },
        getInnerElement: function(idSuffix) {
            return this.getInnerEntity(idSuffix, false);
        },
        getInnerControl: function(idSuffix) {
            return this.getInnerEntity(idSuffix, true);  
        },
        getInnerEntity: function(idSuffix, isControl) {
            var id = this.htmlEditor.name + idSuffix;
            if(!this.cache[id + isControl])
                this.cache[id + isControl] = isControl ? ASPx.GetControlCollection().Get(id) : document.getElementById(id);
            return this.cache[id + isControl];
        }
    });
    function addHighlightToElement(element, wrapper, overlayStyles) {
        var isIE9orLess = ASPx.Browser.IE && ASPx.Browser.Version <= 9;
        var iframeElement = wrapper.getElement();

        var contentElement = isIE9orLess ? iframeElement.contentDocument.documentElement : wrapper.getBody();

        var scrollX = iframeElement.offsetWidth - contentElement.clientWidth;
        var scrollY = iframeElement.offsetHeight - contentElement.clientHeight;
        var overlayCoord = getOverlayCoordinates(element, iframeElement, iframeElement.parentNode, scrollX, scrollY);
        if(overlayCoord) {
            overlayStyles.width = overlayCoord.right - overlayCoord.left;
            overlayStyles.height = overlayCoord.bottom - overlayCoord.top;
            overlayStyles.top = overlayCoord.top;
            overlayStyles.left = overlayCoord.left;
            
            var overlay = document.createElement("DIV");
            ASPx.InsertElementAfter(overlay, iframeElement);

            ASPx.SetStyles(overlay, overlayStyles);
            element.overlay = overlay;
        }
    };

    function getCoordinates(element) {
        var rect = element.getBoundingClientRect();
        return {
            top: rect.top,
            left: rect.left,
            bottom: rect.bottom,
            right: rect.right
        };
    };

    function getOverlayCoordinates(element, iframe, iframeParent, scrollX, scrollY) {
        var elementBox = getCoordinates(element);
        var iframeBox = getCoordinates(iframe);
        var iframeParentBox = getCoordinates(iframeParent);

        var offsetTop = iframeBox.top - iframeParentBox.top;
        var offsetLeft = iframeBox.left - iframeParentBox.left;
        var maxBottom = iframeBox.bottom - iframeBox.top - scrollY;
        var maxRight = iframeBox.right - iframeBox.left - scrollX;

        var resultBox = {
            top: Math.min(Math.max(elementBox.top, 0), maxBottom) + offsetTop,
            left: Math.min(Math.max(elementBox.left, 0), maxRight) + offsetLeft,
            right: Math.max(Math.min(elementBox.right, maxRight), 0) + offsetLeft,
            bottom: Math.max(Math.min(elementBox.bottom, maxBottom), 0) + offsetTop
        };
        
        if(((resultBox.bottom - resultBox.top) <= 0) || ((resultBox.right - resultBox.left) <= 0))
            return null;
        return resultBox;
    };

    ASPx.HtmlEditorClasses.Controls.TagInspector.getElementsToHide = getElementsToHide;
    ASPx.HtmlEditorClasses.Controls.TagInspector.getTagListCurrentWidth = getTagListCurrentWidth;
    ASPx.HtmlEditorClasses.Controls.TagInspector.reduceTagList = reduceTagList;
    ASPx.HtmlEditorClasses.Controls.TagInspector.getChildForBookmark = getChildForBookmark;
    ASPx.HtmlEditorClasses.Controls.TagInspector.getOverlayCoordinates = getOverlayCoordinates;
})();