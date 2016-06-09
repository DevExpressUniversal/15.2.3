(function() {
var PivotDragManager = ASPx.CreateClass(null, {
    constructor: function(pivot, drag) {
        this.DragTargets = new ASPx.CursorTargets();
        this.Configure(pivot, drag, this.DragTargets);
    },
    Configure: function(pivot, drag, targets) {
        this.configureDrag(pivot, drag, targets);
        this.configureTargets(pivot, targets, drag);
        this.registerTargets(targets, drag.obj);
    },
    configureDrag: function(pivot, drag, targets) {
        drag.pGrid = pivot;
        drag.targets = targets;
        drag.onCloneCreating = this.cloneCreating;
        drag.onCloneCreated = this.cloneCreated;
        drag.onDragDivCreating = this.onDragDivCreating;
        drag.onEndDrag = this.endDrag;
        drag.onCancelDrag = function() {
            if(drag.pGrid)
                drag.pGrid.DragDropManager.DragTargets = null;
        };
    },
    configureTargets: function(pivot, targets, drag) {
        targets.pGrid = pivot;
        targets.drag = drag;
        targets.onTargetChanging = this.targetChanging;
        targets.onTargetChanged = this.targetChanged;
        targets.onTargetAllowed = this.targetAllowed;
    },
    cloneCreating: function(clone) {
        var pGrid = ASPx.currentDragHelper.pGrid;
        var groups = pGrid.getGroups();
        if(groups == null) return clone;
        for(var i = 0; i < groups.length; i++) {
            if(groups[i].ContainsField(clone.id)) {
                ASPx.currentDragHelper.obj = pGrid.getField(groups[i], 0);
//              ASPx.currentDragHelper.dragDiv.style.left = ASPx.GetAbsoluteX(ASPx.currentDragHelper.obj) + "px";
//              ASPx.currentDragHelper.dragDiv.style.top = ASPx.GetAbsoluteY(ASPx.currentDragHelper.obj) + "px";
                clone = pGrid.cloneGroup(groups[i]);
                break;
            }
        }
        if(clone.clientLeft == 0 && clone.offsetTop == 0)
            this.centerClone = true;
        return clone;
    },
    cloneCreated: function(clone) {
        var dragHelper = this;
        var list = [];
        var width = dragHelper.obj.offsetWidth + "px";
        clone.style.width = width;
        clone.style.opacity = 0.5;
        clone.style.filter = 'alpha(opacity=50)';
        if(clone.addEventListener) {
            clone.addEventListener('DOMMouseScroll', dragHelper.pGrid.ScrollFieldList, false);
        }
        clone.onmousewheel = dragHelper.pGrid.ScrollFieldList;
        clone.DragDropHelper = dragHelper;
        list.push(clone);
        for(var i = 0; i < clone.childNodes.length; i++) {
            if(ASPx.IsExists(clone.childNodes[i].style))
                clone.childNodes[i].style.width = width;
            list.push(clone.childNodes[i]);
        }
        for(var i = 0; i < list.length; i++) {
            if(!ASPx.IsExists(list[i].style)) continue;
            width = new String(list[i].style.width);
            if(width.indexOf("%") > -1) {
                list[i].style.width = "";
            }
        }
    },
    onDragDivCreating: function(drag, dragDiv) {
	    var rootElement = drag.pGrid.GetMainElement();
        if(!dragDiv || !rootElement) return;
	    dragDiv.className = rootElement.className;
	    dragDiv.style.cssText = rootElement.style.cssText;
    },
    endDrag: function(drag) {
        var pGrid = drag.pGrid;
        pGrid.targetImagesChangeVisibility(false);
        var targets = drag.targets;
        var manager = pGrid.DragDropManager;
        var targetElement = targets.targetElement;
        if(targetElement == drag.obj || !manager.isTargetAllowed(targets)) 
            return;
        if(typeof (targetElement.pgddCustomDrop) != "undefined") {
            targetElement.pgddCustomDrop(drag.obj, targets.targetTag);
            return;
        }
        if(pGrid.isDeferUpdatesChecked() && pGrid.GetCustomizationFieldsWindowContentDiv().className != "TopPanelOnly" && pGrid.isFieldListElement(targetElement)){
            manager.endDragClientSide(drag, pGrid, targets, targetElement);
            pGrid.forcePerformDeferUpdatesCallback = true;
        }
        else{
            pGrid.PerformCallbackInternal(targetElement, 'D' + '|' + drag.obj.id + '|' + targetElement.id + '|' + targets.targetTag);
        }
    },
    endDragClientSide: function(drag, pGrid, targets, targetElement) {
        var targetTableBody;
        var movedRow;
        if(pGrid.isFieldListElement(drag.obj))
            movedRow = ASPx.GetParentByTagName(drag.obj, "tr");
        else {
            var groupIndex;
            var movedRowId;
            var lastIndex = drag.obj.id.lastIndexOf("_");
            var groups = pGrid.getGroups();
            for(var i = 0; i < groups.length; i++)
                if(groups[i].ContainsField(drag.obj.id))
                    groupIndex = i;
            if(groupIndex == null)
                movedRowId = drag.obj.id.substr(0, lastIndex).replace("_DHP", "") + "_dxpgCustFields" + drag.obj.id.substr(lastIndex, drag.obj.id.length);
            else
                movedRowId = drag.obj.id.substr(0, lastIndex).replace("_DHP", "") + "_dxpgCustFields_pgGroupHeader" + groupIndex;
            movedRow = ASPx.GetParentByTagName(document.getElementById(movedRowId), "tr");
        }
        var treeView = pGrid.GetCustomizationTreeView();
        var sourceTable = ASPx.GetParentByTagName(movedRow, "table");
        if(sourceTable.id.indexOf("listCF") != -1 && treeView)
            treeView.HandleDragFromCompleted(drag);
        if(targetElement.id.indexOf("pgHeader") != -1 || targetElement.id.indexOf("pgGroupHeader") != -1) {
            targetTableBody = ASPx.GetParentByTagName(targetElement.parentNode, "tbody");
            targetRow = targetElement.parentNode.parentNode;
            if(targetRow != movedRow) {
                movedRow.parentNode.removeChild(movedRow);
                if(targets.targetTag)
                    targetTableBody.insertBefore(movedRow, targetRow);
                else
                    ASPx.InsertElementAfter(movedRow, targetRow);
            }
        } else {
            movedRow.parentNode.removeChild(movedRow);
            targetTableBody = ASPx.GetNodeByTagName(targetElement, "tbody", 0);
            if(targetTableBody.parentNode.style["display"] == "none" && treeView)
                treeView.HandleDragToCompleted(drag);
            if(targets.targetTag && targetTableBody.firstChild)
                targetTableBody.insertBefore(movedRow, targetTableBody.firstChild);
            else
                targetTableBody.appendChild(movedRow);
        }
        pGrid.resetDragOverFieldList(pGrid.LastHoverFieldList);
        pGrid.FixIEFieldListScrollbar();
    },
    targetAllowed: function(targetElement, x, y) {
        if(ASPx.IsExists(targetElement.pgddIsAllowed))
            return targetElement.pgddIsAllowed(x, y);
        return true;
    },
    targetChanging: function(targets) {
        var pGrid = targets.pGrid;
        if(pGrid.IsTargetElementArea(targets))
            targets.targetTag = true;
        if(pGrid.isVerticalElement(targets.targetElement)) {
            if(targets.targetElement.id.indexOf("pgArea") > -1)
                targets.targetTag = targets.y < ASPx.GetAbsoluteY(ASPx.GetNodesByPartialClassName(targets.targetElement, "dxpgFLListDiv")[0]) + 2;
            else
                targets.targetTag = targets.isTopPartOfElement();
        }
        else
            targets.targetTag = targets.isLeftPartOfElement();
        if(pGrid.isInnerGroupTarget(targets.targetElement, targets.targetTag)) {
            targets.targetElement = null;
            return;
        }

        if(targets.dataHeadersPopup != null) {
            var manager = pGrid.DragDropManager;
            manager.targetChangingDataHeadersPopup(targets);
        }
    },
    targetChangingDataHeadersPopup: function(targets) {
        var popupControl = targets.dataHeadersPopup;
        if(targets.dataHeadersPopupCellRect.Contains(targets.x, targets.y)) {
            if(!popupControl.IsVisible()) {
                popupControl.Show();
                var windowElem = popupControl.GetWindowElement(-1);
                popupControl.bounds = ASPx.GetBounds(windowElem);

                var list = [];
                ASPx.GetNodesByPartialId(windowElem, 'pgHeader', list);
                ASPx.GetNodesByPartialId(windowElem, 'pgGroupHeader', list);
                for(var i = 0; i < list.length; i++)
                    for(var j = 0; j < targets.list.length; j++) {
                        if(targets.list[j].targetElement == list[i]) {
                            targets.list[j].absoluteX = ASPx.GetAbsoluteX(list[i]);
                            targets.list[j].absoluteY = ASPx.GetAbsoluteY(list[i]);
                            break;
                        }
                    }
            }
        } else {
            if(ASPx.IsExists(popupControl.bounds) && popupControl.bounds != null && popupControl.IsVisible()) {
                if(popupControl.bounds.Contains(targets.x, targets.y)) {
                    if(ASPx.IsExists(targets.targetElement) && targets.targetElement.id.indexOf("DHP") < 0)
                        targets.cancelChanging();
                } else {
                    popupControl.Hide();
                    popupControl.bounds = null;
                }
            }
        }
    },
    targetChanged: function(targets) {
        var pGrid = targets.pGrid;
        var manager = pGrid.DragDropManager;
        var isHeadersTable = pGrid.IsHeadersTable(targets.targetElement);
        pGrid.resetDragOverFieldList(pGrid.LastHoverFieldList);
        if(ASPx.currentDragHelper != null && targets.targetElement != ASPx.currentDragHelper.obj && manager.isTargetAllowed(targets)) {//ASPx.currentDragHelper.obj==targets.drag.obj
            if(isHeadersTable && manager.changeTarget(targets))
                targets.targetTag = false;
            pGrid.setDragImagesPosition(targets.targetElement, targets.targetTag);
        } else {
            pGrid.targetImagesChangeVisibility(false);
        }
    },
    registerTargets: function(targets, dragObj) {
        var pGrid = targets.pGrid;
        pGrid.ListTargets = [];
        var targetIds = [];
        targetIds.push("pgGroupHeader", "pgHeader");

        if(pGrid.GetCustomizationFieldsWindowContentDiv() != null) {
            targets.instantAddElement = targets.addElement;
            targets.addElement = this.fieldListAddTarget;
            targets.onTargetAdding = this.fieldListTargetAdding;

            var fieldList = pGrid.getFieldListFields();
            var filterArea = pGrid.getFilterAreaFieldList();
            var columnArea = pGrid.getColumnAreaFieldList();
            var rowArea = pGrid.getRowAreaFieldList();
            var dataArea = pGrid.getDataAreaFieldList();

            targets.RegisterTargets(columnArea, targetIds);
            targets.RegisterTargets(dataArea, targetIds);
            targets.RegisterTargets(filterArea, targetIds);
            targets.RegisterTargets(rowArea, targetIds);

            targets.addElement = targets.instantAddElement;
            targets.instantAddElement = null;
            targets.onTargetAdding = null;

            targets.addElement(fieldList);
            targets.addElement(columnArea);
            targets.addElement(dataArea);
            targets.addElement(filterArea);
            targets.addElement(rowArea);
        }

        targets.onTargetAdding = this.targetAdding;

        targets.RegisterTargets(pGrid.GetDataHeadersPopupWindowElement(), targetIds);
        targets.RegisterTargets(pGrid.GetHeadersTable("DataArea"), targetIds);
        targets.RegisterTargets(pGrid.GetHeadersTable("RowArea"), targetIds);
        targets.RegisterTargets(pGrid.GetHeadersTable("ColumnArea"), targetIds);
        targets.RegisterTargets(pGrid.GetHeadersTable("FilterArea"), targetIds);
        targets.UnregisterTargets(dragObj, targetIds);
        targets.onTargetAdding = null;

        targets.addElement(pGrid.GetHeadersTable("DataArea"));
        targets.addElement(pGrid.GetHeadersTable("ColumnArea"));
        targets.addElement(pGrid.GetHeadersTable("FilterArea"));

        targets.dataHeadersPopup = pGrid.IsDataHeadersPopupExists() ? pGrid.GetDataHeadersPopup() : null;
        if(targets.dataHeadersPopup != null) {
            targets.dataHeadersPopupCell = pGrid.GetDataHeadersPopupCell();
            targets.dataHeadersPopupCellRect = ASPx.GetBounds(targets.dataHeadersPopupCell);
        }
        for(var i = 0; i <= 3; i++) {
            targets.addElement(pGrid.GetEmptyAreaCell(i));
        }
        pGrid.RaiseCustomTargets(targets);
    },
    fieldListAddTarget: function(element) {
        var targets = this;
        var element = targets.instantAddElement(element);
        if(element != null) {
            element.element = element.targetElement.parentNode;
            element.absoluteX = ASPx.GetAbsoluteX(element.element);
            element.absoluteY = ASPx.GetAbsoluteY(element.element);
        }
    },
    fieldListTargetAdding: function(sender, element) {
        var id = sender.pGrid.getLastIdPart(element.id);
        if(id.length == 0) return false;
        var lastChar = id.charAt(id.length - 1);
        if(!(lastChar >= '0' && lastChar <= '9'))
            return false;
        if(!sender.pGrid.IsFieldListTargetAllowed(element)) {
            sender.pGrid.ListTargets.push(element);
            return false;
        }
        return true;
    },
    targetAdding: function(sender, element) {
        return sender.pGrid.IsValidDragDropTarget(element);
    },
    isTargetAllowed: function(targets) {
        if(targets.targetElement == null) return false;
        var pGrid = targets.pGrid;
        if(ASPx.ElementContainsCssClass(targets.targetElement, "dxpgCustFields") && !ASPx.ElementContainsCssClass(targets.targetElement, "dxpgCustFieldsFieldList")) {
            var listTable = ASPx.GetNodeByTagName(targets.targetElement, "table", 0);
            var listTop = ASPx.GetAbsolutePositionY(listTable);
            if(targets.y > listTop && listTop + listTable.offsetHeight > targets.y)
                return false;
        }
        if(pGrid.IsHeadersTable(targets.targetElement) && !pGrid.DragDropManager.isNewTargetArea(targets.drag.obj, targets.targetElement))
            return false;
        var allAllowedIds = pGrid.pivotGrid_AllowedAreaIds[pGrid.name];
        if(allAllowedIds == null) return true;
        var allowedIds = allAllowedIds[targets.drag.obj.id];
        if(!allowedIds)
            allowedIds = allAllowedIds[targets.drag.obj.id.replace("_dxpgCustFields", "").replace("_DHP", "")];
        var isAllowed;
        if(allowedIds != null) {
            var checkTarget;
            if(pGrid.isFieldListHeader(targets.targetElement) && pGrid.isDeferUpdatesChecked())
                checkTarget = ASPx.GetParentByPartialClassName(targets.targetElement.parentNode, "dxpgCustFields");
            else
                checkTarget = targets.targetElement;
            isAllowed = ASPx.Data.ArrayIndexOf(allowedIds, checkTarget.id.replace("_dxpgCustFields", "").replace("_DHP", "")) > -1;
            isAllowed |= ASPx.Data.ArrayIndexOf(allowedIds, checkTarget.id) > -1;
            isAllowed |= pGrid.getFieldListFields().id == targets.targetElement.id && targets.pGrid.getLastIdPart(targets.drag.obj.id).indexOf("pgdthdr") == -1;
            isAllowed &= !this.isTargetDraggedBackToCustForm(targets);
        }
        else {
            isAllowed = !this.isTargetDraggedBackToCustForm(targets);
        }
        return isAllowed;
    },
    isNewTargetArea: function(element, parent) {
        while(element) {
            if(element.parentNode == parent)
                return false;
            element = element.parentNode;
        }
        return true;
    },
    isTargetDraggedBackToCustForm: function(targets) {
    	return targets.targetElement.className.indexOf("dxpgCustFieldsFieldList") >= 0 && ASPx.GetParentByPartialClassName(targets.drag.obj, "dxpgCustFieldsFieldList") != null;
    },
    changeTarget: function(targets) {
        var pGrid = targets.pGrid;
        var targetElement = targets.targetElement;
        var list = [];
        ASPx.GetNodesByPartialId(targetElement, "pgHeader", list);
        ASPx.GetNodesByPartialId(targetElement, "pgGroupHeader", list);
        for(var i = list.length - 1; i >= 0; i--) {
            if(pGrid.IsValidDragDropTarget(list[i])) {
                targets.targetElement = list[i];
                return true;
            }
        }
        return false;
    }
});

ASPx.PivotDragManager = PivotDragManager;
})();