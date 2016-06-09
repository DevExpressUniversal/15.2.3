/// <reference path="_references.js"/>

(function () {
var nonProcessingSymbols = ["^", "\\", "\'"];
var wildCards = ["*", "?"];
var defaultFilterIntervalDelay = 200;
var ASPxClientTitleIndex = ASPx.CreateClass(ASPxClientControl, {
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);

        this.autoFocus = false;
        this.columnCount = 1;
        this.filterDelay = 0;
        this.rowCount = -1;
        this.showBackToTop = false;
        this.softFiltering = false;

        this.groupSpacing = 0;
        this.groupContentPaddingBottom = 0;
        this.groupContentPaddingTop = 0;

        this.lastFilterMask = "";
        this.pasteTimerID = -1;
        this.filterTimerID = -1;
        this.mainCellWidth = 0;

        this.columnElements = {};
        this.groupElements = {};
        this.itemElements = {};
        this.ItemClick = new ASPxClientEvent();

        this.InitializeAutoComplete();
    },

    Initialize: function () {
        if(this.GetFilterInputElement()) {
            this.CleanWhitespaceInControl();

            if(this.filterDelay > defaultFilterIntervalDelay)
                defaultFilterIntervalDelay = this.filterDelay;
        }
        if(this.autoFocus && this.GetFilterInputElement() && ASPx.IsFocusable(this.GetFilterInputElement()))
            this.GetFilterInputElement().focus();

        this.constructor.prototype.Initialize.call(this);
    },
    InitializeAutoComplete: function () {
        var element = this.GetFilterInputElement();
        if(element) element.autocomplete = "off";
    },

    GetCategoryHeaderElement: function (rowIndex) {
        return this.GetChildElement("CH" + rowIndex);
    },
    GetColumnElement: function (index, rowIndex) {
        var columnId = (this.rowCount > 0) ? "_C" + index + "_" + rowIndex : "_C" + index;
        if(!ASPx.IsExistsElement(this.columnElements[columnId]))
            this.columnElements[columnId] = this.GetChildElement(columnId);
        return this.columnElements[columnId];
    },
    GetContentTDWidth: function () {
        return ASPx.GetParentByTagName(this.GetCategoryHeaderElement(0), 'td').clientWidth;
    },
    GetEmptyResultCaptionElement: function () {
        return this.GetChildElement("TI_E");
    },
    GetFilterInputElement: function () {
        return this.GetChildElement("FI");
    },
    GetGroupElements: function (columnElement) {
        if(!ASPx.IsValidElements(this.groupElements[columnElement.id]))
            this.groupElements[columnElement.id] = ASPx.GetNodesByPartialClassName(columnElement, "dxti-g");
        return this.groupElements[columnElement.id];
    },
    GetItemText: function (nodeElement) {
        var textElem = this.GetItemTextElement(nodeElement);
        if(textElem && textElem.nodeValue)
            return ASPx.Str.Trim(textElem.nodeValue);
        else
            return "";
    },
    GetItemElements: function (groupElement) {
        var groupText = this.GetItemText(groupElement);
        if(!ASPx.IsValidElements(this.itemElements[groupText]))
            this.itemElements[groupText] = ASPx.GetNodesByPartialClassName(groupElement, "dxti-i");
        return this.itemElements[groupText];
    },
    GetItemTextElement: function (nodeElement) {
        var textElement = ASPx.GetNodesByPartialClassName(nodeElement, "dxti-link")[0];
        return textElement ? ASPx.GetTextNode(textElement) : null;
    },
    GetContentCell: function () {
        return this.GetChildElement("CCell");
    },
    GetTreeViewCell: function () {
        return this.GetChildElement("ICell");
    },
    SetPrevFilterMask: function (filterMask) {
        this.lastFilterMask = filterMask || "";
    },
    // Timer
    SetFilterTimer: function (timeout) {
        if(timeout == 0)
            this.FilterTimer();
        else
            this.filterTimerID = window.setTimeout(function () { this.FilterTimer(); }.aspxBind(this), timeout);
    },
    FilterTimer: function () {
        this.DoFilter(this.GetFilterInputElement().value);
        this.ClearFilterTimer();
    },
    SetPasteTimer: function () {
        this.pasteTimerID = window.setInterval(function () {
            this.DoFilterInternal();
        }.aspxBind(this), defaultFilterIntervalDelay);
    },
    ClearFilterTimer: function () {
        this.filterTimerID = ASPx.Timer.ClearTimer(this.filterTimerID);
    },
    ClearPasteTimer: function () {
        this.pasteTimerID = ASPx.Timer.ClearInterval(this.pasteTimerID);
    },

    CleanWhitespaceInControl: function () {
        if(this.rowCount > 0)
            this.CleanWhitespace(ASPx.GetParentByTagName(this.GetCategoryHeaderElement(0), "table"));
        else {
            for(var j = 0; j < this.columnCount; j++)
                this.CleanWhitespace(this.GetColumnElement(j, -1));
        }
    },
    CleanWhitespace: function (element) {
        if(element.hasChildNodes()) {
            var i = 0;
            while(i < element.childNodes.length) {
                var node = element.childNodes[i];
                if(node.nodeType == 3 && !/\S/.test(node.nodeValue))
                    node.parentNode.removeChild(node);
                else {
                    i++;
                    if(node.nodeType != 3)
                        this.CleanWhitespace(node);
                }
            }
        }
    },
    CreateFilterRegEx: function (filterMask) {
        var regExString = (this.softFiltering) ? ".*" : "^";
        for(var i = 0; i < filterMask.length; i++) {
            var index = ASPx.Data.ArrayIndexOf(wildCards, filterMask.charAt(i));
            if(index > -1) {
                switch (index) {
                    case 0: regExString += ".*"; break; // * - wildCard
                    case 1: regExString += ".{1}"; break; // ? - wildCard
                }
            }
            else {
                var index = ASPx.Data.ArrayIndexOf(nonProcessingSymbols, filterMask.charAt(i));
                if(index == -1)
                    regExString += "[" + filterMask.charAt(i) + "]";
            }
        }
        regExString += ".*";
        return new RegExp(regExString, 'i'); // todo optional
    },
    OnCallback: function (result) {
        this.stateObject = { index: result.index };

        var element = this.GetContentCell();
        if(element != null)
            ASPx.SetInnerHtml(element, result.html);
        this.columnCount = result.columnCount;
    },
    IsAllowableKeyCode: function (keyCode) {
        return (((keyCode >= 48) && (keyCode <= 57)) ||
                ((keyCode >= 186) && (keyCode <= 192)) ||
                ((keyCode >= 219) && (keyCode <= 226)) ||
                ((keyCode >= 65) && (keyCode <= 90)) ||
                ((keyCode >= 65) && (keyCode <= 90)) ||
                ((keyCode >= 96) && (keyCode <= 107)) ||
                ((keyCode >= 109) && (keyCode <= 111)) ||
                    keyCode == 8 || keyCode == 45 || keyCode == 46);
    },
    IsFirstCategoryTR: function (categoryElem) {
        var spacingElem = categoryElem.parentNode.previousSibling;
        return !spacingElem;
    },
    IsFirstCategory: function (categoryElem, index) {
        var i = index - 1;
        while(i >= 0) {
            if(ASPx.GetElementDisplay(this.GetCategoryHeaderElement(i).parentNode))
                return false;
            i--;
        }
        return true;
    },
    IsFirstGroupInColumn: function (groupIndex, columnElem) {
        var groupElements = this.GetGroupElements(columnElem);
        var i = groupIndex - 1;

        while(i >= 0) {
            if(ASPx.GetElementDisplay(groupElements[i]))
                return false;
            i--;
        }
        return true;
    },
    IsFilterTimerActive: function () {
        return this.filterTimerID > -1;
    },
    IsFilterMaskChanged: function (filterMask) {
        if(!this.lastFilterMask)
            this.lastFilterMask = "";
        return this.lastFilterMask.toUpperCase() != filterMask.toUpperCase();
    },
    CorrectCategorySpacing: function (categoryElem, index) {
        var headerTR = categoryElem.parentNode;
        if(!this.IsFirstCategoryTR(categoryElem) &&
            (this.groupSpacing != 0) && this.IsFirstCategory(categoryElem, index))
            ASPx.SetElementDisplay(headerTR.previousSibling, false);
    },
    CorrectNodesPaddings: function (columnIndex, rowIndex) {
        var columnElem = this.GetColumnElement(columnIndex, rowIndex);
        var groupElements = this.GetGroupElements(columnElem);

        for(var i = 0; i < groupElements.length; i++) {
            if(ASPx.GetElementDisplay(groupElements[i])) {
                if(this.IsFirstGroupInColumn(i, columnElem)) // first group
                    ASPx.Attr.ChangeStyleAttribute(groupElements[i], "paddingTop", 0);
                else
                    ASPx.Attr.RestoreStyleAttribute(groupElements[i], "paddingTop");
                this.CorrectGroupContentPadding(groupElements[i]);
            }
        }
    },
    CorrectGroupContentPadding: function (groupElement) {
        var itemElements = this.GetItemElements(groupElement);

        for(var i = 0; i < itemElements.length; i++) {
            if(ASPx.GetElementDisplay(itemElements[i])) {
                if(i == 0)  // First Item
                    ASPx.Attr.ChangeStyleAttribute(itemElements[i], "paddingTop", 0);
                else
                    ASPx.Attr.RestoreStyleAttribute(itemElements[i], "paddingTop");
            }
        }
    },
    DoIndexPanelItemClick: function (value) {
        if(this.GetFilterInputElement())
            this.GetFilterInputElement().disabled = true;
        this.CreateCallback(value + ASPx.CallbackSeparator + value);
    },
    CreateCallback: function (arg, command, callbackInfo) {
        this.ShowLoadingElements();
        ASPxClientControl.prototype.CreateCallback.call(this, arg, command);
    },
    ShowLoadingPanel: function () {
        this.CreateLoadingPanelWithAbsolutePosition(this.GetContentCell(), this.GetLoadingPanelOffsetElement(this.GetMainElement()));
    },
    ShowLoadingDiv: function () {
        this.CreateLoadingDiv(this.GetContentCell(), this.GetMainElement());
    },
    GetCallbackAnimationElement: function () {
        return this.GetTreeViewCell();
    },

    OnControlClick: function (clickedElement, htmlEvent) {
        var itemElement = ASPx.GetParentByPartialClassName(clickedElement, "dxti-link");
        if(itemElement) {
            var processOnServer = this.RaiseItemClick(itemElement, htmlEvent);

            var hasItemLink = this.GetLinkElement(itemElement) != null;
            if(processOnServer && !hasItemLink) {
                var name = this.GetItemElementName(itemElement);
                this.SendPostBack("CLICK:" + name);
            }
        }
    },
    RaiseItemClick: function (itemElement, htmlEvent) {
        var processOnServer = this.autoPostBack || this.IsServerEventAssigned("ItemClick");
        if(!this.ItemClick.IsEmpty()) {
            var name = this.GetItemElementName(itemElement);
            var args = new ASPxClientTitleIndexItemEventArgs(processOnServer, name, itemElement, htmlEvent);
            this.ItemClick.FireEvent(this, args);
            processOnServer = args.processOnServer;
        }
        return processOnServer;
    },
    DoFilter: function (filterMask) {
        filterMask = ASPx.Str.Trim(filterMask);
        
        if(this.IsFilterMaskChanged(filterMask)) {
            this.SetPrevFilterMask(filterMask);
            var filterRegEx = this.CreateFilterRegEx(filterMask);
            var isData = false;

            if(this.rowCount > 0) { // Categorized = true;
                var categoryCount = 0;

                if(this.GetContentTDWidth() != 0)
                    this.mainCellWidth = this.GetContentTDWidth();

                for(var i = 0; i < this.rowCount; i++) {
                    var categoryElem = this.GetCategoryHeaderElement(i);
                    // Columns
                    var visibleColCount = 0;
                    for(var j = 0; j < this.columnCount; j++) {
                        var visibleItemCount = this.FilterColumnInCategory(j, i, filterMask, filterRegEx);
                        if(visibleItemCount != 0)
                            visibleColCount++;
                    }
                    if(visibleColCount != 0) {
                        this.ShowCategory(categoryElem);
                        this.CorrectCategorySpacing(categoryElem, i);

                        categoryCount++;
                    }
                    else
                        this.HideCategory(categoryElem);
                }
                isData = categoryCount != 0;
            }
            else { // Categorized = false;
                var visibleColCount = 0;
                for(var i = 0; i < this.columnCount; i++) {
                    var visibleCategoryNodeCount = this.FilterColumn(i, -1, filterMask, filterRegEx);
                    this.CorrectNodesPaddings(i, -1);

                    if(visibleCategoryNodeCount != 0)
                        visibleColCount++;
                }
                isData = visibleColCount != 0;
            }
            // NoDataPanel
            if(isData)
                this.HideEmptyResultCaption();
            else
                this.ShowEmptyResultCaption();
        }
    },
    DoFilterInternal: function () {
        this.DoFilter(this.GetFilterInputElement().value);
    },
    FilterColumn: function (columnIndex, rowIndex, filterMask, filterRegEx) {
        var columnElem = this.GetColumnElement(columnIndex, rowIndex);

        var groupElements = this.GetGroupElements(columnElem);
        var visibleGroupCount = 0;

        // Category Node
        for(var i = 0; i < groupElements.length; i++) {
            // Items
            visibleItemsCount = this.FilterItems(groupElements[i], filterRegEx);
            if(visibleItemsCount > 0) {
                visibleGroupCount++;
                this.ShowGroup(groupElements[i]);
            }
            else
                this.HideGroup(groupElements[i]);
        }
        // Column
        if(visibleGroupCount != 0) {
            ASPx.SetElementVisibility(columnElem, true);
            this.SetColumnSeparatorDisplay(columnElem, true, columnIndex, rowIndex);
        }
        else {
            ASPx.SetElementVisibility(columnElem, false);
            this.SetColumnSeparatorDisplay(columnElem, false, columnIndex, rowIndex);
        }
        return visibleGroupCount;
    },
    FilterColumnInCategory: function (columnIndex, rowIndex, filterMask, filterRegEx) {
        var columnElem = this.GetColumnElement(columnIndex, rowIndex);
        var visibleItemCount = 0;

        if(columnElem) {
            var itemElements = this.GetItemElements(columnElem);

            for(var i = 0; i < itemElements.length; i++) {
                var itemText = this.GetItemText(itemElements[i]);
                if(!filterRegEx.test(itemText))
                    ASPx.SetElementDisplay(itemElements[i], false);
                else {
                    ASPx.SetElementDisplay(itemElements[i], true);
                    visibleItemCount++;
                }
            }

            if(visibleItemCount != 0) {
                this.SetColumnSeparatorDisplay(columnElem, true, columnIndex, rowIndex);
            }
            else {
                this.SetColumnSeparatorDisplay(columnElem, false, columnIndex, rowIndex);
            }
        }
        return visibleItemCount;
    },
    FilterItems: function (parentNodeElem, filterRegEx) {
        var itemElements = this.GetItemElements(parentNodeElem);
        var visibleNodeCount = 0;

        for(var i = 0; i < itemElements.length; i++) {
            var itemText = this.GetItemText(itemElements[i]);
            if(filterRegEx.test(itemText)) {
                ASPx.SetElementDisplay(itemElements[i], true);
                visibleNodeCount++;
            }
            else
                ASPx.SetElementDisplay(itemElements[i], false);
        }
        return visibleNodeCount;
    },
    ShowCategory: function (categoryElement) {
        var headerTR = categoryElement.parentNode;
        ASPx.SetElementDisplay(headerTR, true);
        // GroupSpacing
        if((this.groupSpacing != 0) && !this.IsFirstCategoryTR(categoryElement))
            ASPx.SetElementDisplay(headerTR.previousSibling, true);

        var categoryContentElem = null;
        if(this.groupContentPaddingTop == 0) {// GroupContentPadding.PaddingTop == 0
            categoryContentElem = headerTR.nextSibling;
            ASPx.SetElementDisplay(headerTR.nextSibling, true);
        }
        else {
            // GroupContentPadding.PaddingTop TR
            ASPx.SetElementDisplay(headerTR.nextSibling, true);
            // CategoryContent TR
            categoryContentElem = headerTR.nextSibling.nextSibling;
            ASPx.SetElementDisplay(headerTR.nextSibling.nextSibling, true);
        }

        // GroupContentPadding.PaddingBottom TR
        if(this.groupContentPaddingBottom != 0)
            ASPx.SetElementDisplay(categoryContentElem.nextSibling, true);
        if(this.showBackToTop) {
            if(this.groupContentPaddingBottom != 0)
                ASPx.SetElementDisplay(categoryContentElem.nextSibling.nextSibling, true);
            else
                ASPx.SetElementDisplay(categoryContentElem.nextSibling, true);
        }
    },
    HideCategory: function (categoryElement) {
        var headerTR = categoryElement.parentNode;
        ASPx.SetElementDisplay(headerTR, false);
        // GroupSpacing
        if((this.groupSpacing != 0) && !this.IsFirstCategoryTR(categoryElement))
            ASPx.SetElementDisplay(headerTR.previousSibling, false);

        var categoryContentElem = null;
        if(this.groupContentPaddingTop == 0) { // GroupContentPadding.PaddingTop == 0
            categoryContentElem = headerTR.nextSibling;
            ASPx.SetElementDisplay(headerTR.nextSibling, false);
        }
        else {
            // GroupContentPadding.PaddingTop TR
            ASPx.SetElementDisplay(headerTR.nextSibling, false);
            // CategoryContent TR
            categoryContentElem = headerTR.nextSibling.nextSibling;
            ASPx.SetElementDisplay(headerTR.nextSibling.nextSibling, false);
        }
        // GroupContentPadding.PaddingBottom TR
        if(this.groupContentPaddingBottom != 0)
            ASPx.SetElementDisplay(categoryContentElem.nextSibling, false);
        if(this.showBackToTop) {
            if(this.groupContentPaddingBottom != 0)
                ASPx.SetElementDisplay(categoryContentElem.nextSibling.nextSibling, false);
            else
                ASPx.SetElementDisplay(categoryContentElem.nextSibling, false);
        }
    },
    ShowEmptyResultCaption: function () {
        var elem = this.GetEmptyResultCaptionElement();
        if(this.rowCount > 0 && (this.mainCellWidth > 0))
            elem.style.width = this.mainCellWidth + "px";
        ASPx.SetElementDisplay(elem, true);
    },
    HideEmptyResultCaption: function () {
        var elem = this.GetEmptyResultCaptionElement();
        ASPx.SetElementDisplay(elem, false);
    },
    ShowGroup: function (nodeElement) {
        if(!ASPx.GetElementDisplay(nodeElement))
            ASPx.SetElementDisplay(nodeElement, true);
    },
    HideGroup: function (nodeElement) {
        ASPx.SetElementDisplay(nodeElement, false);
    },

    SetColumnSeparatorDisplay: function (columnElem, value, columnIndex, rowIndex) {
        if(columnIndex != this.columnCount - 1)
            this.SetRightColumnSeparatorDisplay(columnElem, value);
        else { // Last column
            var prevColumn = this.GetColumnElement(columnIndex - 1, rowIndex);
            if((prevColumn != null) && (ASPx.GetElementDisplay(prevColumn))) {
                this.SetLeftColumnSeparatorDisplay(columnElem, value);
            }
        }
    },
    SetLeftColumnSeparatorDisplay: function (columnElem, value) {
        var curElem = columnElem.previousSibling;
        while((curElem != null) && (curElem.id == "")) {
            ASPx.SetElementVisibility(curElem, value);
            curElem = curElem.previousSibling;
        }
    },
    SetRightColumnSeparatorDisplay: function (columnElem, value) {
        var curElem = columnElem.nextSibling;
        while((curElem != null) && (curElem.id == "")) {
            ASPx.SetElementVisibility(curElem, value);
            curElem = curElem.nextSibling;
        }
    },

    // Events
    OnFilterInputBlur: function (evt) {
        this.ClearPasteTimer();
    },
    OnFilterInputChange: function (evt) {
        if(this.GetFilterInputElement().value != "") {
            if(this.IsFilterTimerActive())
                this.ClearFilterTimer();
            this.DoFilter(this.GetFilterInputElement().value); // problem with OnBlur
        }
    },
    OnFilterInputFocus: function () {
        this.SetPasteTimer();
    },
    OnFilterInputKeyUp: function (evt) {
        if(this.IsAllowableKeyCode(evt.keyCode)) {
            if(this.IsFilterTimerActive())
                this.ClearFilterTimer();
            this.SetFilterTimer(this.filterDelay);
        }
    },
    OnFilterInputKeyPress: function (evt) {
        if(evt.keyCode == ASPx.Key.Enter) // Disable Enter
            return false;
    }
});
ASPxClientTitleIndex.Cast = ASPxClientControl.Cast;
var ASPxClientTitleIndexItemEventArgs = ASPx.CreateClass(ASPxClientProcessingModeEventArgs, {
    constructor: function (processOnServer, name, htmlElement, htmlEvent) {
        this.constructor.prototype.constructor.call(this, processOnServer);
        this.name = name;
        this.htmlElement = htmlElement;
        this.htmlEvent = htmlEvent;
    }
});

ASPx.SIFBlur = function(name) {
    var si = ASPx.GetControlCollection().Get(name);
    if(si != null) si.OnFilterInputBlur();
    return true;
}
ASPx.SIFChange = function(evt, name) {
    var si = ASPx.GetControlCollection().Get(name);
    if(si != null) si.OnFilterInputChange(evt);
    return true;
}
ASPx.SIFFocus = function(name) {
    var si = ASPx.GetControlCollection().Get(name);
    if(si != null) si.OnFilterInputFocus();
    return true;
}
ASPx.SIFKeyUp = function(evt, name) {
    var si = ASPx.GetControlCollection().Get(name);
    if(si != null) si.OnFilterInputKeyUp(evt);
    return true;
}
ASPx.SIFKeyPress = function(evt, name) {
    var si = ASPx.GetControlCollection().Get(name);
    if(si != null) return si.OnFilterInputKeyPress(evt);
    return true;
}
// IndexPanel
ASPx.IPItemClick = function(name, value) {
    var ti = ASPx.GetControlCollection().Get(name);
    if(ti != null) ti.DoIndexPanelItemClick(value);
}


window.ASPxClientTitleIndex = ASPxClientTitleIndex;
window.ASPxClientTitleIndexItemEventArgs = ASPxClientTitleIndexItemEventArgs;
})();