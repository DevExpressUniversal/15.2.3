(function() {
    ASPx.HtmlEditorClasses.Commands.DeleteSelectedListItems = function(wrapper, startListItem, endListItem) {
        var elements = wrapper.getSelection().GetElements(true);
        var skipFirstListItem = !!startListItem;
        var firstElement = elements[0];
        if(firstElement.nodeName == "LI")
            firstElement = firstElement.firstChild;
        var lastElement = elements[elements.length - 1];
        if(lastElement.nodeName == "LI")
            lastElement = lastElement.lastChild;
        var selection = wrapper.getSelection();
    
        var doc = wrapper.getDocument();
        selectionBookmarks = { "startMarkerID": "sbm", "endMarkerID": "ebm"};
        var sbm = doc.createElement("span");
        sbm.id = "sbm";
        var ebm = doc.createElement("span");
        ebm.id = "ebm";
        firstElement.parentNode.insertBefore(ebm, firstElement);
        firstElement.parentNode.insertBefore(sbm, firstElement);
        ASPx.RemoveElement(firstElement);
        var getNextSibling = function(element) { return element.nextSibling; };
        var getPreviousSibling = function(element) { return element.previousSibling; };
        var getSibling = function(element, func) { 
            element = func(element);
            while(element && element.nodeType != 1 && !element.nodeValue)
                element = func(element);
            return element;
        };
        var nextSibling = getSibling(lastElement, getNextSibling);
        if(nextSibling && nextSibling.nodeName != "LI" && nextSibling.nodeName != "UL" && nextSibling.nodeName != "OL") {
            for(var nextElement = nextSibling; nextElement; nextElement = nextElement.nextSibling) {
                if(nextElement.nodeName != "LI" && nextElement.nodeName != "UL" && nextElement.nodeName != "OL")
                    ASPx.InsertElementAfter(nextElement.cloneNode(true), sbm)
                else break;
            }
        }
        else { 
            nextSibling = getSibling(sbm, getNextSibling);
            if(!nextSibling || nextSibling.nodeName == "LI" || nextSibling.nodeName == "UL" || nextSibling.nodeName == "OL")
                ASPx.InsertElementAfter(doc.createTextNode("\xA0"), sbm);
        }
        if(!startListItem) {
            for(var i = 1; i < elements.length - 1; i++) {
                var listItem = ASPx.GetParentByTagName(elements[i], "LI");
                if(!listItem) 
                    ASPx.RemoveElement(elements[i]);
                else {
                    startListItem = listItem;
                    break;
                }
            }
        }
        var parentList = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetMainParentList(endListItem);
        var listItemArray = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetListItemArray(parentList, 0 ,null, null, selectionBookmarks);
        if(lastElement.nodeName == "UL" || lastElement.nodeName == "OL")
            endListItem = listItemArray[listItemArray.length - 1].node;
        var startListItemIndex, endListItemIndex;
        for(var i = 0, listItem; listItem = listItemArray[i]; i++) {
            if(startListItem == listItem.node)
                startListItemIndex = skipFirstListItem ? i + 1 : i;
            else if(endListItem == listItem.node) {
                endListItemIndex = i;
                break;
            }
        }
        ASPx.HtmlEditorClasses.Commands.Browser.InsertList.RemoveListItems(startListItemIndex === undefined ? endListItemIndex : startListItemIndex, endListItemIndex, listItemArray);
        var list = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GenerateListByItemArray(listItemArray);
        if(list)
            parentList.parentNode.insertBefore(list, parentList);
        ASPx.RemoveElement(parentList);
        selection.clientSelection.SelectExtendedBookmark(selectionBookmarks);
    };

    // Delete with selection
    ASPx.HtmlEditorClasses.Commands.Delete = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
	    Execute: function(cmdValue, wrapper) {
            var isSuccessfully = true;
            var selectedElement = wrapper.getSelection().GetSelectedElement();
            if(ASPx.ElementHasCssClass(selectedElement, ASPx.HtmlEditorClasses.SelectedContainerCssClasseName))
                wrapper.commandManager.executeCommand(ASPxClientCommandConsts.DELETEPLACEHOLDER_COMMAND, null, true);
            else {
                var clientSelection = wrapper.getSelection().clientSelection;
                var startContainer = clientSelection.GetStartContainer();
                var endContainer = clientSelection.GetEndContainer();
                var startListItem = ASPx.GetParentByTagName(startContainer, "LI");
                var endListItem = ASPx.GetParentByTagName(endContainer, "LI");
                if(endListItem && startListItem != endListItem)
                    ASPx.HtmlEditorClasses.Commands.DeleteSelectedListItems(wrapper, startListItem, endListItem)
                else {
                    ASPx.HtmlEditorClasses.Commands.Utils.correctSelectionForDeleteCommand(wrapper);
                    var designViewIFrameDocument = wrapper.getDocument();
  	                isSuccessfully = designViewIFrameDocument.execCommand(this.commandID, false, null);
  	                if (ASPx.Browser.Opera) { // hack Opera, because when SelectAll is executed, Delete doesn't work
		                designViewIFrameDocument.contentEditable = true;
		                designViewIFrameDocument.body.focus();
		            }
                }
            }
            ASPx.HtmlEditorClasses.Commands.Browser.InsertList.UpdateListNumbering(wrapper.getBody());
            return isSuccessfully;
	    }
    });
})();