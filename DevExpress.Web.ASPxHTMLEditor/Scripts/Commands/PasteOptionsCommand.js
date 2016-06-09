(function() {
    ASPx.HtmlEditorClasses.Commands.PasteOptionsCommand = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
        Execute: function(cmdValue, wrapper) {
            if(!ASPx.IsExists(this.pasteContent))
                return false;
            ASPx.HtmlEditorClasses.Commands.Command.prototype.Execute.apply(this, arguments);

            var startID = this.getStartBookmarkID();
            var endID = this.getEndBookmarkID();

            var doc = wrapper.getDocument();
            var content = wrapper.raiseBeforePaste(this.GetCommandID(), this.pasteContent);
            if(wrapper.settings.enablePasteOptions) {
                this.owner.lastPasteFormattingCommandObject.commandName = this.GetCommandID();
                if(this.owner.getLastPasteFormattingHtml())
                    wrapper.setInnerHtml(wrapper.getBody(), this.owner.getLastPasteFormattingHtml());
                else
                    wrapper.showPasteOptionsBar();
                
                content = this.setBookmark(content);
                
                var element = doc.getElementById(endID);
                if(element) {
                    element.parentNode.insertBefore(doc.createTextNode("\xA0"), element);
                    var element = doc.getElementById(startID);
                    ASPx.InsertElementAfter(doc.createTextNode("\xA0"), element);

                    var selection = wrapper.getSelection();
                    selection.clientSelection.SelectExtendedBookmark(this.getBookmark(this.getStartBookmarkID(), this.getEndBookmarkID(), true), ASPx.Browser.Safari ? false : true);
                    var elements = selection.GetElements();
                    for(var i = 0, element; element = elements[i]; i++) {
                        if(!element.id || (element.id != startID && element.id != endID))
                            element.parentNode.removeChild(element);
                    }
                    if(ASPx.Browser.Safari)
                        selection.clientSelection.SelectExtendedBookmark(this.getBookmark(this.getStartBookmarkID(), this.getEndBookmarkID(), true));
                }
            }
            
            wrapper.commandManager.executeCommand(ASPxClientCommandConsts.PASTEHTML_COMMAND, content);

            if(wrapper.settings.enablePasteOptions) {
                if(ASPx.Browser.IE) {
                    this.removeEmptyParentElement(wrapper.getDocument(), endID);
                    this.removeEmptyParentElement(wrapper.getDocument(), startID);
                }
                this.owner.lastPasteFormattingCommandObject.html = wrapper.getRawHtml();

                var element = doc.getElementById(this.getStartBookmarkID());
                if(element)
                    element.parentNode.removeChild(element);
                element = doc.getElementById(this.getEndBookmarkID());
                if(element)
                    this.removeElement(doc, element, wrapper.getSelection().clientSelection, true);
            }
            return true;
        },
        setBookmark: function(content) {
            return "<span id=\"" + this.getStartBookmarkID() +  "\"></span>" + content + "<span id=\"" + this.getEndBookmarkID() + "\"></span>";
        },
        getBookmark: function(startID, endID, isDirectOrder) {
            return { "startMarkerID": startID , "endMarkerID": endID, isDirectOrder: isDirectOrder };
        },
        getStartBookmarkID: function() {
            return "dx_temp_startID";
        },
        getEndBookmarkID: function() {
            return "dx_temp_endID";
        },
        initPasteContent: function(wrapper, pasteContent) {
            this.pasteContent = pasteContent;
        },
	    IsLocked: function(wrapper) {
	        return false;
	    },
        IsHtmlChangeable: function() {
            return false;
        },
        removeEmptyParentElement: function(doc, id) {
            var element = doc.getElementById(id);
            if(element && element.parentNode.nodeName != "BODY" && !ASPx.Str.Trim(ASPx.GetInnerText(element.parentNode))) {
                var parent = element.parentNode;
                parent.parentNode.insertBefore(element, parent);
                parent.parentNode.removeChild(parent);
            }
        },
        removeElement: function(doc, element, clientSelection, setFocusOnTimeout) {
            var tempStartElement = doc.createElement("SPAN");
            tempStartElement.id = ASPx.HtmlEditorClasses.Selection.CreateUniqueID();
            var tempEndElement = doc.createElement("SPAN");
            tempEndElement.id = ASPx.HtmlEditorClasses.Selection.CreateUniqueID();

            element.parentNode.insertBefore(tempStartElement, element);
            element.parentNode.insertBefore(tempEndElement, element);
                    
            element.parentNode.removeChild(element);
            if(setFocusOnTimeout) {
                setTimeout(function() {
                    clientSelection.SelectExtendedBookmark(this.getBookmark(tempEndElement.id, tempStartElement.id, false));
                }.aspxBind(this), 0);
            }
            else
                clientSelection.SelectExtendedBookmark(this.getBookmark(tempEndElement.id, tempStartElement.id, false));
        }
    });
    ASPx.HtmlEditorClasses.Commands.PasteHtmlSourceFormatting = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.PasteOptionsCommand, {
        Execute: function(cmdValue, wrapper) {
            ASPx.HtmlEditorClasses.Commands.PasteOptionsCommand.prototype.Execute.apply(this, arguments);
            return true;
        },
	    GetState: function(wrapper) {
            return this.owner.getLastPasteFormattingCommandName() == this.GetCommandID();
        },
        initPasteContent: function(wrapper, pasteContent) {
            ASPx.HtmlEditorClasses.Commands.PasteOptionsCommand.prototype.initPasteContent.call(this, wrapper, pasteContent);
        }
    });
    ASPx.HtmlEditorClasses.Commands.PasteHtmlPlainText = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.PasteOptionsCommand, {
        Execute: function(cmdValue, wrapper) {
            ASPx.HtmlEditorClasses.Commands.PasteOptionsCommand.prototype.Execute.apply(this, arguments);
            return true;
        },
	    GetState: function(wrapper) {
            return this.owner.getLastPasteFormattingCommandName() == this.GetCommandID();
        },
        initPasteContent: function(wrapper, pasteContent) {
            if(pasteContent)
                pasteContent = pasteContent.replace(/\n/gi, '<br/>');
            ASPx.HtmlEditorClasses.Commands.PasteOptionsCommand.prototype.initPasteContent.call(this, wrapper, pasteContent);
        }
    });
    ASPx.HtmlEditorClasses.Commands.PasteHtmlMergeFormatting = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.PasteOptionsCommand, {
        Execute: function(cmdValue, wrapper) {
            ASPx.HtmlEditorClasses.Commands.PasteOptionsCommand.prototype.Execute.apply(this, arguments);
            return true;
        },
	    GetState: function(wrapper) {
            return this.owner.getLastPasteFormattingCommandName() == this.GetCommandID();
        },
        initPasteContent: function(wrapper, pasteContent) {
            if(pasteContent) {
                var tampElementID = ASPx.HtmlEditorClasses.Selection.CreateUniqueID();
                var doc = wrapper.getDocument();
                var obj = ASPx.HtmlEditorClasses.Utils.insetContentInHiddenContainer(wrapper, pasteContent, true);
                var selection = wrapper.getSelection();
                var selectedElements = selection.IsCollapsed() ? [] : selection.GetElements(true);
                var selectedElement = this.owner.getCommand(ASPxClientCommandConsts.FONTNAME_COMMAND).GetSelectedElement(wrapper, selection, selectedElements);

                for(var i = 0, commandState; commandState = this.styleStateBeforePaste[i]; i++) {
                    if(selectedElements.length > 0 && this.ContainsNotDOMElements(selectedElements))
                        selectedElements = selection.GetElements(true);
                    if(ASPx.IsExists(commandState.state) && commandState.state && this.owner.getCommand(commandState.commandName).GetState(wrapper, selection, selectedElements) != commandState.state) {
                        this.owner.executeCommand(commandState.commandName);
                        if(commandState.repeat)
                            this.owner.executeCommand(commandState.commandName);
                    }
                    else if(ASPx.IsExists(commandState.value)) {
                        if(ASPxClientCommandConsts.FONTSIZE_COMMAND == commandState.commandName && this.owner.getCommand(ASPxClientCommandConsts.FONTSIZE_COMMAND).getFontSizeValue(wrapper, selection, selectedElements) != commandState.value)
                            this.owner.executeCommand(commandState.commandName, commandState.value);
                        else if(ASPxClientCommandConsts.FONTNAME_COMMAND == commandState.commandName && this.owner.getCommand(ASPxClientCommandConsts.FONTNAME_COMMAND).getFontNameValue(wrapper, selection, selectedElement, selectedElements) != commandState.value)
                            this.owner.executeCommand(commandState.commandName, commandState.value);
                        else if(this.owner.getCommand(commandState.commandName).GetValue(wrapper, selection, selectedElements) != commandState.value)
                            this.owner.executeCommand(commandState.commandName, commandState.value);
                    }
                }
                var element = obj.hiddenChild;
                element.id = "";
                ASPx.HtmlEditorClasses.Utils.clearPasteContainerStyle(element);

                pasteContent = element.style.cssText ? element.outerHTML : element.innerHTML;
                obj.hiddenParent.parentNode.removeChild(obj.hiddenParent);
                selection.clientSelection.SelectExtendedBookmark(obj.bookmark);
            }
            this.styleStateBeforePaste = [];
            ASPx.HtmlEditorClasses.Commands.PasteOptionsCommand.prototype.initPasteContent.call(this, wrapper, pasteContent);
        },
        initStyleStateArray: function(wrapper) {
            this.styleStateBeforePaste = [];
            var selection = wrapper.getSelection();
            var selectedElements = selection.IsCollapsed() ? [] : selection.GetElements(true);
            
            this.styleStateBeforePaste.push({ commandName: ASPxClientCommandConsts.JUSTIFYCENTER_COMMAND, state: this.owner.getCommand(ASPxClientCommandConsts.JUSTIFYCENTER_COMMAND).GetState(wrapper, selection, selectedElements) });
            this.styleStateBeforePaste.push({ commandName: ASPxClientCommandConsts.JUSTIFYLEFT_COMMAND, state: this.owner.getCommand(ASPxClientCommandConsts.JUSTIFYLEFT_COMMAND).GetState(wrapper, selection, selectedElements) });
            this.styleStateBeforePaste.push({ commandName: ASPxClientCommandConsts.JUSTIFYRIGHT_COMMAND, state: this.owner.getCommand(ASPxClientCommandConsts.JUSTIFYRIGHT_COMMAND).GetState(wrapper, selection, selectedElements) });
            this.styleStateBeforePaste.push({ commandName: ASPxClientCommandConsts.JUSTIFYFULL_COMMAND, state: this.owner.getCommand(ASPxClientCommandConsts.JUSTIFYFULL_COMMAND).GetState(wrapper, selection, selectedElements) });

            this.styleStateBeforePaste.push({ commandName: ASPxClientCommandConsts.SUPERSCRIPT_COMMAND, state: this.owner.getCommand(ASPxClientCommandConsts.SUPERSCRIPT_COMMAND).GetState(wrapper, selection, selectedElements) });
            this.styleStateBeforePaste.push({ commandName: ASPxClientCommandConsts.SUBSCRIPT_COMMAND, state: this.owner.getCommand(ASPxClientCommandConsts.SUBSCRIPT_COMMAND).GetState(wrapper, selection, selectedElements) });

            this.styleStateBeforePaste.push({ commandName: ASPxClientCommandConsts.FONTSIZE_COMMAND, value: this.owner.getCommand(ASPxClientCommandConsts.FONTSIZE_COMMAND).getFontSizeValue(wrapper, selection, selectedElements) });
            var selectedElement = this.owner.getCommand(ASPxClientCommandConsts.FONTNAME_COMMAND).GetSelectedElement(wrapper, selection, selectedElements);
            this.styleStateBeforePaste.push({ commandName: ASPxClientCommandConsts.FONTNAME_COMMAND, value: this.owner.getCommand(ASPxClientCommandConsts.FONTNAME_COMMAND).getFontNameValue(wrapper, selection, selectedElement, selectedElements) });

            this.styleStateBeforePaste.push({ commandName: ASPxClientCommandConsts.BOLD_COMMAND, state: this.owner.getCommand(ASPxClientCommandConsts.BOLD_COMMAND).GetState(wrapper, selection, selectedElements)});
            this.styleStateBeforePaste.push({ commandName: ASPxClientCommandConsts.ITALIC_COMMAND, state: this.owner.getCommand(ASPxClientCommandConsts.ITALIC_COMMAND).GetState(wrapper, selection, selectedElements)});
            this.styleStateBeforePaste.push({ commandName: ASPxClientCommandConsts.UNDERLINE_COMMAND, state: this.owner.getCommand(ASPxClientCommandConsts.UNDERLINE_COMMAND).GetState(wrapper, selection, selectedElements)});
            this.styleStateBeforePaste.push({ commandName: ASPxClientCommandConsts.STRIKETHROUGH_COMMAND, state: this.owner.getCommand(ASPxClientCommandConsts.STRIKETHROUGH_COMMAND).GetState(wrapper, selection, selectedElements)});
        },
        getStyleStateBeforePaste: function() {
            return this.styleStateBeforePaste;
        }
    });
})();