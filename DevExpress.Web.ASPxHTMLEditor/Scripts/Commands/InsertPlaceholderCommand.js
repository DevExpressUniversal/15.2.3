(function() {
    ASPx.HtmlEditorClasses.Commands.UpdateDocument = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
        Execute: function(cmdValue, wrapper) {
            var html = wrapper.getHtml();
            wrapper.setHtml(html);
            if(ASPx.Browser.NetscapeFamily)
                wrapper.resetContentEditable();
        }
    });

    ASPx.HtmlEditorClasses.Commands.InsertPlaceholder = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
        Execute: function(cmdValue, wrapper) {
            ASPx.HtmlEditorClasses.Commands.Command.prototype.Execute.apply(this, arguments);
            var doc = wrapper.getDocument();
            var selectedElement = wrapper.getSelection().GetSelectedElement();
            if(ASPx.ElementHasCssClass(selectedElement, ASPx.HtmlEditorClasses.PlaceholderCssClasseName))
                return this.owner.getCommand(ASPxClientCommandConsts.CHANGEPLACEHOLDER_COMMAND).Execute(cmdValue, wrapper);

            var element = doc.createElement("SPAN");
            element.innerHTML = wrapper.placeholderManager.getDefaultStartMark() + wrapper.placeholderManager.getPlaceholderContent(cmdValue) + wrapper.placeholderManager.getDefaultEndMark();
            element.contentEditable = false;
            element.className = ASPx.HtmlEditorClasses.PlaceholderCssClasseName;

            var tempElement = ASPx.HtmlEditorClasses.Selection.CreateElementWithUniqueID(doc);
            wrapper.commandManager.executeCommand(ASPxClientCommandConsts.PASTEHTML_COMMAND, element.outerHTML + tempElement.outerHTML);
            tempElement =  doc.getElementById(tempElement.id);
            element = tempElement.previousSibling;
            tempElement.parentNode.removeChild(tempElement);
            wrapper.getSelection().clientSelection.applySpecialSelectionToElement(element);
            wrapper.placeholderManager.updateFildsArray();
            return true;
        }
    });

    ASPx.HtmlEditorClasses.Commands.ChangePlaceholder = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
        Execute: function(cmdValue, wrapper) {
            ASPx.HtmlEditorClasses.Commands.Command.prototype.Execute.apply(this, arguments);
            var selectedElement = wrapper.getSelection().GetSelectedElement();
            if(ASPx.ElementHasCssClass(selectedElement, ASPx.HtmlEditorClasses.PlaceholderCssClasseName)) {
                var placeholderContent = ASPx.GetChildByClassName(selectedElement, ASPx.HtmlEditorClasses.PlaceholderContentClasseName);
                placeholderContent.innerHTML = cmdValue;
                return true;
            }
            return false;
        }
    });

    ASPx.HtmlEditorClasses.Commands.DeletePlaceholder = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
        Execute: function(cmdValue, wrapper) {
            ASPx.HtmlEditorClasses.Commands.Command.prototype.Execute.apply(this, arguments);
            var doc = wrapper.getDocument();
            var selection = wrapper.getSelection();
            var selectedElement = selection.GetSelectedElement();
            var bookmark = ASPx.HtmlEditorClasses.Selection.getBookmarkByElements(doc, selectedElement, selectedElement);
            selection.getSpecialSelection().removeHiddenContainer();
            selectedElement.parentNode.removeChild(selectedElement);
            wrapper.getSelection().clientSelection.SelectExtendedBookmark({ "startMarkerID": bookmark.endMarkerID, "endMarkerID": bookmark.startMarkerID});
            return true;
        }
    });
})();