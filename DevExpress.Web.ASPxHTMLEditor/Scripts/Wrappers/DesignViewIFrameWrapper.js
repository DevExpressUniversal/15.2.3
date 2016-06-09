(function() {
    ASPx.HtmlEditorClasses.Wrappers.DesignIFrameWrapper = ASPx.CreateClass(ASPx.HtmlEditorClasses.Wrappers.IFrameWrapper, {
        constructor: function(id, settings, callbacks) {
            this.constructor.prototype.constructor.call(this, id, settings, callbacks);
            this.selectionManager = new ASPx.HtmlEditorClasses.Managers.SelectionManager(this);
            this.placeholderManager = new ASPx.HtmlEditorClasses.Managers.PlaceholderManager(this);
            this.isInFocus = false;
            this.settings.allowPlaceholderReplacing = true;
            this.resetScrollPosAfterInsertHtml = true;
        },
        initialize: function(inlineInit, bodyContentHtml) {
            ASPx.HtmlEditorClasses.Wrappers.IFrameWrapper.prototype.initialize.call(this, inlineInit, bodyContentHtml);
            this.setDesignModeAttributeValue(this.enabled);
            if (this.iframeStateController) {
                this.iframeStateController.state.bodyAttributes.push({ 'name': 'contentEditable', 'value': this.enabled });
            }
        },
        initializeManagers: function() {
            ASPx.HtmlEditorClasses.Wrappers.IFrameWrapper.prototype.initializeManagers.call(this);
            this.eventManager = new ASPx.HtmlEditorClasses.Managers.DesignEventManager(this);
        },
        createCommandManager: function() {
            return new ASPx.HtmlEditorClasses.Managers.DesignCommandManager(this);
        },
        createKeyboardManager: function (shortcuts, disabledShortcuts) {
            return new ASPx.HtmlEditorClasses.Managers.DesignViewKeyboardManager(shortcuts, disabledShortcuts);
        },
        getSearchSelectedElement: function() {
            return this.getSelection().GetSelectedElement();    
        },
        getSearchSelectedText: function() {
            return this.getSelection().GetText();    
        },
        onLockRelease: function() {
            this.commandManager.executeCommand(ASPxClientCommandConsts.SAVESTATEUNDOREDOSTACK_COMMAND, null, true);
            this.updateToolbar(true);
            this.eventManager.onHtmlChangedInternal(true);    
        },
        getName: function() {
            return ASPx.HtmlEditorClasses.View.Design;
        },
        focus: function() {
            var focusObj = ASPx.Browser.NetscapeFamily || ASPx.Browser.Edge || ASPx.Browser.WebKitFamily && !ASPx.Browser.WebKitTouchUI ? this.getBody() : this.getWindow();
            focusObj.focus();
            ASPx.HtmlEditorClasses.Wrappers.IFrameWrapper.prototype.focus.call(this);
        },
        removeBrElements: function(html, body) {
            var childNodes = body.childNodes;
            if(childNodes.length == 0 || childNodes.length == 1 && childNodes[0].nodeName == "BR")
                html = "";
            return html;
        },
        getViewAreaStyleCssText: function() {
            return ASPx.IsExists(this.settings.viewAreaStyleCssText) ? this.settings.viewAreaStyleCssText : "";
        },
        getDocumentClassName: function() {
            return "dxheDesignViewDoc";
        },
        setDesignModeAttributeValue: function(enabled) {
            var doc = this.getDocument();
            if(doc && doc.body)
                doc.body.contentEditable = enabled;
        },
        resetContentEditable: function() {
            this.setDesignModeAttributeValue(false);
            this.setDesignModeAttributeValue(true);
            if (this.isInFocus) {
                this.eventManager.setPreventFocusEventFlag();
                this.eventManager.setPreventLostFocusEventFlag();
                this.removeFocus();
                this.focus();
                this.isInFocus = true;
            }
        },
        getRawHtml: function() {
            return this.getBody().innerHTML;
        },
        getProcessedHtml: function(html) {
            html = ASPx.HtmlEditorClasses.Search.RemoveSearchInfo(html);
            if(this.settings.updateDeprecatedElements)
                html = this.replaceFontWithSpanTag(html);
            if(ASPx.Browser.NetscapeFamily)
                html = html.replace(new RegExp("<br\\s*id=\"" + ASPx.HtmlEditorClasses.preservedAttributeNamePrefix + "tempBr\"[^>]*>", "gi"), "");
            html = this.depreserveButtonTags(html);
            html = this.filterHtmlToGetHtml(html);
            html = this.removeEmptyBorderClassName(html);
            html = this.closeTags(html);
            html = this.depreserveAttribute(html, ASPx.HtmlEditorClasses.ContentEditableAttributeNameRegExp);
            if(ASPx.Browser.NetscapeFamily)
                html = this.removeMozUserSelectStyleAttribute(html);
            html = html.replace(new RegExp("\\s+(" + ASPx.HtmlEditorClasses.ListIdAttributeName + "|" + ASPx.HtmlEditorClasses.HeadListAttributeName + ")\\s*=\\s*['\"][^'\"]*['\"]", "gi"), "");
            html =  ASPx.HtmlEditorClasses.HtmlProcessor.restoreElementBySpecialImage(html);
            if(this.settings.allowPlaceholderReplacing && this.settings.placeholders && this.settings.placeholders.length > 0)
                html = this.placeholderManager.replacePlaceholderElementToTextLabel(html);
            html = ASPx.HtmlEditorClasses.HtmlProcessor.removeSelectedHiddenContainer(html);
            html = ASPx.HtmlEditorClasses.Wrappers.IFrameWrapper.prototype.getProcessedHtml.call(this, html);
            return html;
        },
        processHtmlBodyBeforeInsert: function(html) {
            html = ASPx.HtmlEditorClasses.Wrappers.IFrameWrapper.prototype.processHtmlBodyBeforeInsert.call(this, html);
            html = ASPx.HtmlEditorClasses.HtmlProcessor.replaceElementsToSpecialImage(this.getDocument(), html, ASPx.HtmlEditorClasses.getReplacingElementsOptionsArray());
            if (ASPx.Browser.NetscapeFamily)
                html = html.replace(/(<input[^>]*>)(?=<\/|$)/gi, "$1<br id=\"" + ASPx.HtmlEditorClasses.preservedAttributeNamePrefix + "tempBr\"/>");
            if(this.settings.updateDeprecatedElements)
                html = this.replaceFontWithSpanTag(html);
            html = ASPx.HtmlEditorClasses.HtmlProcessor.preserveAttribute(html, ASPx.HtmlEditorClasses.ContentEditableAttributeNameRegExp);
            if(ASPx.Browser.NetscapeFamily)
                html = this.setTextInputUnselectable(html);
            if(this.settings.allowPlaceholderReplacing && this.settings.placeholders && this.settings.placeholders.length > 0)
                html = this.placeholderManager.replaceTextLabelToPlaceholderElement(html);
            html = ASPx.HtmlEditorClasses.Commands.Browser.InsertList.UpdateListNumberingByHtml(html, this.getDocument());
            return html;
        },
        insertHtml: function(html) {
            this.setInnerHtmlToBody(html);
            ASPx.HtmlEditorClasses.HtmlProcessor.processInnerHtml(this.getBody());
            this.processingFormElements(this.getBody());
            window.setTimeout(function() {
                if(this.resetScrollPosAfterInsertHtml) {
                    var doc = this.getDocument();
                    if(doc && doc.body)
                        doc.body.scrollTop = 0;
                }
                this.resetScrollPosAfterInsertHtml = true;
            }.aspxBind(this), 0);
        },
        setHtml: function(html) {
            if(this.eventManager) {
                this.selectionManager.selection = null;
                this.eventManager.callSuspendedHtmlChangedEvt();
            }
            ASPx.HtmlEditorClasses.Wrappers.IFrameWrapper.prototype.setHtml.call(this, html);
            this.placeholderManager.updateFildsArray();
        },
        saveScrollPosition: function() {
            var doc = this.getDocument();
            this.savedScrollTop = doc.body.scrollTop;
            this.savedScrollLeft = doc.body.scrollLeft;
        },
        restoreScrollPosition: function() {
            this.getWindow().scrollTo(this.savedScrollLeft, this.savedScrollTop);
        },

        /* Selection API */
        getSelection: function() {
            return this.selectionManager.createSelection();
        },
        saveSelection: function() {
            this.selectionManager.saveSelection();
        },
        saveLastSelection: function() {
            return this.selectionManager.saveLastSelection();
        },
        restoreSelection: function() {
            this.selectionManager.restoreSelection();
        },
        restoreLastSelection: function(selectionObj) {
            this.selectionManager.restoreLastSelection(selectionObj);
        },
        restoreSelectionForPopup: function() {
            this.selectionManager.restoreSelectionForPopup();
        },
        saveSelectionForPopup: function() {
            this.selectionManager.saveSelectionForPopup();
        },
        restoreSelectionByTimer: function() {
            window.setTimeout(function() {
                this.restoreSelection();
            }.aspxBind(this), 0);
        },
        isSelectionRestored: function() {
            return this.selectionManager.isSelectionRestored;
        },
        getCachedElementsManager: function() {
            return this.selectionManager.cachedElementsManager;
        },
        getSelectedElement: function(name) {
            return this.getCachedElementsManager().GetSeletedElement(name);
        },
        setSelectedElement: function(name, element) {
            return this.getCachedElementsManager().SetSelectedElement(name, element);
        },
        clearCachedSeletedElements: function() {
            this.getCachedElementsManager().ClearSeletedElements();
        },
        needGetElementFromSelection: function (name) {
            return this.getCachedElementsManager().isNeedCachedSelectedElements[name] && !ASPx.IsExists(this.getCachedElementsManager().selectedElements[name]);
        },
        saveSelectionByBookmark: function() {
            return this.selectionManager.saveSelectionByBookmark();
        },
        restoreSelectionByBookmark: function() {
            this.selectionManager.restoreSelectionByBookmark(this.getDocument(), ASPx.HtmlEditorClasses.StartSelectionPosMarkID, ASPx.HtmlEditorClasses.EndSelectionPosMarkID);
        },
        restoreBookmark: function(savedSelectionByBookmarkObject, html) {
            return this.selectionManager.restoreBookmark(savedSelectionByBookmarkObject, html);
        }
    });

    ASPx.HtmlEditorClasses.Managers.PlaceholderManager = ASPx.CreateClass(null, {
        constructor: function(wrapper, valueList) {
            this.wrapper = wrapper;
            this.filds = [];
        },
        updateFildsArray: function() {
            var body = this.wrapper.getBody();
            var predicate = function(element) { return element.nodeName == "SPAN" && ASPx.ElementHasCssClass(element, ASPx.HtmlEditorClasses.PlaceholderCssClasseName); };
            this.filds = ASPx.HtmlEditorClasses.Utils.getChildsByPredicate(body, predicate);
        },
        updatePlaceholdersStyle: function() {
            this.updateFildsArray();
            for(var i = 0, fild; fild = this.filds[i]; i++)
                this.updateStyleToElement(fild);
        },
        getFildsCount: function() {
            return this.filds.length;
        },
        setAllowEditFilds: function(value) {
            if(this.needToUpdateFildsArray())
                this.updateFildsArray();
            for(var i = 0, fild; fild = this.filds[i]; i++)
                this.setAllowEditElement(fild, value);
        },
        setAllowEditElement: function(element, value) {
            if(value) {
                ASPx.Attr.RemoveAttribute(element, "contentEditable");
                if(ASPx.Browser.IE)
                    ASPx.Attr.RemoveAttribute(element.style, "display");
            }
            else {
                ASPx.Attr.SetAttribute(element, "contentEditable", false);
                if(ASPx.Browser.IE) {
                    ASPx.Attr.SetAttribute(element.style, "display", "inline-block");
                    this.updateStyleToElement(element);
                }
            }
        },
        updateStyleToElement: function(element) {
            if(!this.wrapper.commandManager) return;
            var commands = [ASPxClientCommandConsts.UNDERLINE_COMMAND, ASPxClientCommandConsts.STRIKETHROUGH_COMMAND];
            for(var i = 0, commandName; commandName = commands[i]; i++) {
                var comman = this.wrapper.commandManager.getCommand(commandName);
                var attrName = comman.GetStyleAttribute();
                ASPx.Attr.RemoveAttribute(element.style, attrName);
            }
            for(var i = 0, commandName; commandName = commands[i]; i++) {
                var comman = this.wrapper.commandManager.getCommand(commandName);
                var attrName = comman.GetStyleAttribute();
                var attrValue = comman.GetStyleAttributeConstValue();
                var state = comman.getStateToElement(element);
                if(state) {
                    var value = element.style[attrName];
                    element.style[attrName] = value ? value + " " +  attrValue : attrValue;
                }
            }
        },
        needToUpdateFildsArray: function() {
            var placeholderValueArray = this.getPlaceholderValueArray();
            if(placeholderValueArray.length == 0)
                return false;
            for(var i = 0, fild; fild = this.filds[i]; i++) {
                if(!ASPx.HtmlEditorClasses.Utils.getBodyByElement(fild))
                    return true;
            }
            return false;
        },
        replacePlaceholderElementToTextLabel: function(html) {
            var doc = this.wrapper.getDocument();
            var isPlaceholderElement = function(element) { return element.nodeType == 1 && ASPx.ElementHasCssClass(element, ASPx.HtmlEditorClasses.PlaceholderCssClasseName); };
            var element = doc.createElement("DIV");
            element.innerHTML = html;
            var placeholderElements = ASPx.HtmlEditorClasses.Utils.getChildsByPredicate(element, isPlaceholderElement);
            if(placeholderElements.length == 0)
                this.processingPlaceholderElement(element);
            else {
                for(var i = 0, placeholder; placeholder = placeholderElements[i]; i++) {
                    this.processingPlaceholderElement(placeholder);
                    this.processingElement(placeholder);
                }
            }
            return element.innerHTML;
        },
        processingPlaceholderElement: function(parent) {
            var isPlaceholderMark = function(element) { return element.nodeType == 1 && (element.className == ASPx.HtmlEditorClasses.PlaceholderStartMarkCssClasseName || element.className == ASPx.HtmlEditorClasses.PlaceholderEndMarkCssClasseName); };
            var isPlaceholderContentElement = function(element) { return element.nodeType == 1 && element.className == ASPx.HtmlEditorClasses.PlaceholderContentClasseName; };
            var placeholderContentElement = ASPx.HtmlEditorClasses.Utils.getChildsByPredicate(parent, isPlaceholderContentElement)[0];
            if(!placeholderContentElement)
                return;
            var placeholderMarks = ASPx.HtmlEditorClasses.Utils.getChildsByPredicate(parent, isPlaceholderMark);
            if(placeholderContentElement.firstChild.nodeType != 3) {
                var child = placeholderContentElement.firstChild;
                while(child.nodeType != 3)
                    child = child.firstChild;
                placeholderContentElement.parentNode.insertBefore(placeholderContentElement.firstChild, placeholderContentElement);
                child.parentNode.insertBefore(placeholderContentElement, child);
                placeholderContentElement.appendChild(child);
            }
            placeholderContentElement.innerHTML = ASPx.GetInnerText(placeholderMarks[0]) + placeholderContentElement.innerHTML + ASPx.GetInnerText(placeholderMarks[1]);
                
            for(var j = 0, mark; mark = placeholderMarks[j]; j++)
                mark.parentNode.removeChild(mark);

            this.processingElement(placeholderContentElement);
        },
        processingElement: function(element) {
            var cssText = this.processingCssText(element.style.cssText);
            if(cssText) {
                element.style.cssText = cssText;
                ASPx.Attr.RemoveAttribute(element, "id");
                ASPx.Attr.RemoveAttribute(element, "class");
            }
            else {
                var parentNode = element.parentNode;
                for(var j = 0, child; child = element.childNodes[j]; j++)
                    parentNode.insertBefore(child.cloneNode(true), element);
                element.parentNode.removeChild(element);
            }
        },
        processingCssText: function(cssText) {
            if(ASPx.Browser.IE)
                cssText = cssText.replace(/text-decoration:[^;]*;/, "");
            cssText = cssText.replace(/white-space:[^;]*;/, "");
            return cssText.replace(/display:[^;]*;/, "");
        },
        replaceTextLabelToPlaceholderElement: function(html) {
            html = this.replacePlaceholderElementToTextLabel(html);
            var placeholderValueArray = this.getPlaceholderValueArray();
            var styles = ASPx.Browser.IE ? " style='display: inline-block;'" : "";
            for(var i = 0, itemValue; itemValue = placeholderValueArray[i]; i++) {
                html = html.replace(new RegExp("{((&nbsp;)*|\\s*)(" + itemValue + ")((&nbsp;)*|\\s*)}", "g"), "<span class='" + ASPx.HtmlEditorClasses.PlaceholderCssClasseName +"'" + styles + " contenteditable='false'><span class='" + ASPx.HtmlEditorClasses.PlaceholderStartMarkCssClasseName + "'>{$1</span><span class='" + ASPx.HtmlEditorClasses.PlaceholderContentClasseName + "'>" + "$3" + "</span><span class='" + ASPx.HtmlEditorClasses.PlaceholderEndMarkCssClasseName + "'>$4}</span></span>");
                var regExp = "<span class='" + ASPx.HtmlEditorClasses.PlaceholderCssClasseName +"'" + styles + " contenteditable='false'><span class='" + ASPx.HtmlEditorClasses.PlaceholderStartMarkCssClasseName + "'>{((&nbsp;)*|\\s*)</span><span class='" + ASPx.HtmlEditorClasses.PlaceholderContentClasseName + "'>" + itemValue + "</span><span class='" + ASPx.HtmlEditorClasses.PlaceholderEndMarkCssClasseName + "'>((&nbsp;)*|\\s*)}</span></span>";
                html = html.replace(new RegExp("(<[^>]*)" + regExp + "([^>]*>)", "gi"), "$1{$2" + itemValue +"$4}$6");
            }
            return html;
        },
        getDefaultStartMark: function() {
            return "<span class='" + ASPx.HtmlEditorClasses.PlaceholderStartMarkCssClasseName + "'>{ </span>";
        },
        getDefaultEndMark: function() {
            return "<span class='" + ASPx.HtmlEditorClasses.PlaceholderEndMarkCssClasseName + "'> }</span>";
        },
        getPlaceholderContent: function(content) {
            return "<span class='" + ASPx.HtmlEditorClasses.PlaceholderContentClasseName + "'>" + content + "</span>";
        },
        getPlaceholderValueArray: function() {
            return this.wrapper.settings.placeholders ? this.wrapper.settings.placeholders : [];
        }
    });
})();