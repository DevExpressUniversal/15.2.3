(function() {
    var DeleteElement = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
        // cmdValue - element to delete
        Execute: function(cmdValue, wrapper) {
            var selection = wrapper.getSelection();
	        var elementToDelete = cmdValue || selection.GetSelectedElement();
	        if (!elementToDelete)
	            return false;
            
	        var externalCmdName = this.getDeleteCommandName(elementToDelete);
	        if (externalCmdName)
	            return wrapper.commandManager.getCommand(externalCmdName).Execute({ selectedElement: elementToDelete }, wrapper);
	        else {
                if(ASPx.HtmlEditorClasses.Utils.needToUseSpecialSelection(elementToDelete))
                    this.deleteElement(wrapper, elementToDelete, true);
                else {
                    var clientSelection = selection.clientSelection;
                    var bookmark = clientSelection.GetExtendedBookmark();
	                switch (elementToDelete.tagName) {
	                    case "LI":
	                        this.deleteLICommand(elementToDelete);
	                        break;
	                    case "CAPTION":
	                        this.deleteCaptionCommand(elementToDelete);
	                        break;
	                    default:
	                        this.deleteSimpleElement(elementToDelete);
                            break;
	                }
                    clientSelection.SelectExtendedBookmark(bookmark);
                }
            }
	        return true;
	    },
        deleteSimpleElement: function(element) {
            var parent = element.parentNode;
            while(element.firstChild) 
                parent.insertBefore(element.firstChild, element);
            parent.removeChild(element);
        },
        deleteElement: function(wrapper, element, saveSelection) {
            var doc = wrapper.getDocument();
            var selection = wrapper.getSelection();
            element = element ? element : selection.GetSelectedElement();
            var bookmark;
            if(saveSelection)
                bookmark = ASPx.HtmlEditorClasses.Selection.getBookmarkByElements(doc, element, element);
            selection.getSpecialSelection().removeHiddenContainer();
            element.parentNode.removeChild(element);
            if(bookmark)
                wrapper.getSelection().clientSelection.SelectExtendedBookmark({ "startMarkerID": bookmark.endMarkerID, "endMarkerID": bookmark.startMarkerID});
        },
        getDeleteCommandName: function(elementToDelete) {
            switch(elementToDelete.tagName) {
                case "TABLE":
                    return this.getTableDeleteCommandName();
                case "TR":
                    return this.getTRDeleteCommandName(elementToDelete);
                case "TD":
                    return this.getTDDeleteCommandName(elementToDelete);
                case "UL":
                    return this.getULDeleteCommandName(elementToDelete);
                case "OL":
                    return this.getOLDeleteCommandName(elementToDelete);
            }
            return null;
        },
        deleteCaptionCommand: function(caption) {
            var text = document.createTextNode(caption.innerHTML);
            var table = caption.parentElement;
            table.removeChild(caption);
            table.parentElement.insertBefore(text, table);
            return null;
        },
        deleteLICommand: function (li) {
            var parentNode = li.parentNode;
            ASPx.RemoveElement(li);
            for (var i = li.childNodes.length - 1 ; i >= 0 ; i--) {
                ASPx.InsertElementAfter(li.childNodes[i], parentNode);
            }
            return null;
        },
        getTableDeleteCommandName: function(ul) {
            return ASPxClientCommandConsts.DELETETABLE_COMMAND;
        },
        getULDeleteCommandName: function(ul) { 
            if(ul && ul.childNodes.length > 0)
                return ASPxClientCommandConsts.INSERTUNORDEREDLIST_COMMAND;
            return null;
        },
        getOLDeleteCommandName: function(ol) {
            if(ol && ol.childNodes.length > 0)
                return ASPxClientCommandConsts.INSERTORDEREDLIST_COMMAND;
            return null;
        },
        getTRDeleteCommandName: function(tr) {
            var table = ASPx.GetParentByTagName(tr, "TABLE");
            var rows = ASPx.HtmlEditorClasses.Utils.getTableRows(table);
            if(rows && rows.length > 1)
                return ASPxClientCommandConsts.DELETETABLEROW_COMMAND;
            return this.getTableDeleteCommandName();
        },
        getTDDeleteCommandName: function(td) {
            if (ASPx.HtmlEditorClasses.Utils.getPreviousElementSibling(td))
                return ASPxClientCommandConsts.MERGETABLECELLLEFT_COMMAND;
            else if (ASPx.HtmlEditorClasses.Utils.getNextElementSibling(td))
                return ASPxClientCommandConsts.MERGETABLECELLRIGHT_COMMAND;
            return this.getTRDeleteCommandName(td.parentNode);
        }
    });
    ASPx.HtmlEditorClasses.Commands.DeleteElement = DeleteElement;
})();