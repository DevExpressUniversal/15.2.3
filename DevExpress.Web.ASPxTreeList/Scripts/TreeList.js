// TreeList
(function() {
var ASPxClientTreeList = ASPx.CreateClass(ASPxClientControl, {

    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        
        this.rowIdPrefix = this.name + "_" + this.RowIDSuffix;
        this.focusedKey = null;
        this.dragHelper = new TreeListDragHelper(this);
        this.selectionDiff = { };
        this.syncLock = false;
        this.callbackHandlersPool = [ ];
        this.kbdHelper = null;
        this.selectionStartKey = null;
        this.internalCheckBoxCollection = null;
        this.supportGestures = true;
        this.sizingConfig.adjustControl = true;
        this.indentCount = -1;
        
        // set from outside
        this.enableFocusedNode = false;
        this.focusSendsCallback = false;
        this.selectionSendsCallback = false;
        this.recursiveSelection = false;
        this.columns = [ ];
        this.expandCollapseAction = this.ExpandCollapseAction.Button;
        this.focusOnExpandCollapse = true;
        this.confirmDeleteMsg = null;
        this.allowStylizeEditingNode = false;
        this.enableKeyboard = false;
        this.accessKey = null;
        this.checkBoxImageProperties = null;
        this.icbFocusedStyle = [];
        this.showRoot = false;
        this.columnResizeMode = ASPx.ColumnResizeMode.None;
        this.horzScroll = ASPx.ScrollBarMode.Hidden;
        this.vertScroll = ASPx.ScrollBarMode.Hidden;
        this.enableEllipsis = false;
         
        // updatable on callbacks
        this.maxVisibleLevel = -1;
        this.visibleColumnCount = 0;
        this.rowCount = 0;
        this.editingKey = null;
        this.isNewNodeEditing = false;
        this.pageIndex = -1;
        this.pageSize = -1;
        this.pageCount = 1;
        this.cButtonIDs = null;
                        
        this.FocusedNodeChanged = new ASPxClientEvent();
        this.SelectionChanged = new ASPxClientEvent();
        this.CustomizationWindowCloseUp = new ASPxClientEvent();
        this.CustomDataCallback = new ASPxClientEvent();		
        this.NodeClick = new ASPxClientEvent();
        this.NodeDblClick = new ASPxClientEvent();
        this.ContextMenu = new ASPxClientEvent();
		this.StartDragNode = new ASPxClientEvent();
		this.EndDragNode = new ASPxClientEvent();        
		this.CustomButtonClick = new ASPxClientEvent();
		this.NodeFocusing = new ASPxClientEvent();
        this.NodeExpanding = new ASPxClientEvent();
        this.NodeCollapsing = new ASPxClientEvent();
        this.ColumnResizing = new ASPxClientEvent();
        this.ColumnResized = new ASPxClientEvent();
    },
    
    Initialize: function() {
        ASPxClientControl.prototype.Initialize.call(this);
        this.AssignEllipsisToolTips();
    },

    InlineInitialize: function() {
        ASPxClientControl.prototype.InlineInitialize.call(this);

        this.RefreshArmatureRow();
        if(this.enableKeyboard) {
            this.kbdHelper = new TreeListKbdHelper(this);
            this.kbdHelper.Init();
            ASPx.KbdHelper.RegisterAccessKey(this);
        }
        this.PrepareCommandButtons();
        if(this.checkBoxImageProperties) 
            this.CreateInternalCheckBoxCollection();
        this.SetHeadersClientEvents();
        var resizingHelper = this.GetResizingHelper();
        if(resizingHelper)
            resizingHelper.ResetStretchedColumnWidth();
    },
    PrepareCommandButtons: function(){
        if(!this.cButtonIDs) return;
        
        for(var i = 0; i < this.cButtonIDs.length; i++){
            var clientID = this.cButtonIDs[i];
            if(!ASPx.GetElementById(clientID)) continue;

            var button = new ASPxClientButton(clientID);
            button.useSubmitBehavior = false;
            button.causesValidation = false;
            button.encodeHtml = !!eval(ASPx.Attr.GetAttribute(button.GetMainElement(), "data-encodeHtml"));
            button.isNative = !!eval(ASPx.Attr.GetAttribute(button.GetMainElement(), "data-isNative"));
            button.enabled = !ASPx.ElementContainsCssClass(button.GetMainElement(), "dxbDisabled");
            button.Click.AddHandler(this.OnCommandButtonClick.aspxBind(this));
            button.InlineInitialize();
        }
        delete this.cButtonIDs;
    },
    OnCommandButtonClick: function(button){
        var handler = this.GetCommandButtonHandler(button.name);
        if(!handler)
            return;
        var clickArgs = eval(ASPx.Attr.GetAttribute(button.GetMainElement(), "data-args"));
        clickArgs ? handler.apply(this, clickArgs) : handler.apply(this);
    },
    GetCommandButtonHandler: function(buttonName){
        var buttonType = parseInt(buttonName.split("_").pop());
        switch(buttonType){
            case this.CommandButtonType.Edit:
                return this.StartEdit;
            case this.CommandButtonType.New:
                return this.StartEditNewNode;
            case this.CommandButtonType.Delete:
                return this.OnNodeDeleting;
            case this.CommandButtonType.Update:
                return this.UpdateEdit;
            case this.CommandButtonType.Cancel:
                return this.CancelEdit;
        }
        if(buttonType < 0)
            return this.RaiseCustomButtonClick;
    },
    CreateInternalCheckBoxCollection: function() {
        if(!this.internalCheckBoxCollection)
            this.internalCheckBoxCollection = new ASPx.CheckBoxInternalCollection(this.checkBoxImageProperties, true, undefined, undefined, undefined, this.accessibilityCompliant);
        else
            this.internalCheckBoxCollection.SetImageProperties(this.checkBoxImageProperties);
        this.CompleteInternalCheckBoxCollection();
    },
    CompleteInternalCheckBoxCollection: function() {
        var container = this.GetUpdatableCell();
        var selectAllCheck = this.FindSelectAllCheckBox();
        var icbInputElements = this.FindSelectionCheckBoxes(container);
        this.internalCheckBoxCollection.Clear();
        for(var i = 0; i < icbInputElements.length; i++)
            this.AddInternalCheckBoxToCollection(icbInputElements[i]);
      
        if(selectAllCheck)
            this.AddSelectAllCheckBoxToCollection(selectAllCheck);
    },
    AddInternalCheckBoxToCollection: function(icbInputElement) {
        var instance = this;
        var row = this.FindDataRow(icbInputElement);
        var internalCheckBox = this.internalCheckBoxCollection.Add(row.id, icbInputElement);
        internalCheckBox.CreateFocusDecoration(this.icbFocusedStyle);
        internalCheckBox.SetEnabled(this.GetEnabled());
        internalCheckBox.readOnly = this.readOnly;
        internalCheckBox.CheckedChanged.AddHandler(
            function(s, e) {
                if(instance.IsNodeDragDropEnabled()) 
                    instance.OnNodeSelecting(ASPx.Evt.GetEventSource(e));
                else
                    instance.OnRowCheckBoxClick(e);
            }
        );
    },
    AddSelectAllCheckBoxToCollection: function(selectAllCheckInput) {
        var instance = this;
        var selectAllInternalCheckBox = this.internalCheckBoxCollection.Add(selectAllCheckInput.id, selectAllCheckInput);
        selectAllInternalCheckBox.CreateFocusDecoration(this.icbFocusedStyle);
        selectAllInternalCheckBox.SetEnabled(this.GetEnabled());
        selectAllInternalCheckBox.readOnly = this.readOnly;
        selectAllInternalCheckBox.CheckedChanged.AddHandler(
            function(s, e) {
                instance.OnSelectingAll(s.GetChecked());
            }
        );
    },
    IsNodeDragDropEnabled: function() {
        return !!this.GetDataTable().onmousedown;
    },

    // UI
    OnNodeSelecting: function(check) {    
		this.OnNodeSelectingCore(check);		
		if(this.RaiseSelectionChanged())
			this.SendDummyCommand();		
    },
    OnNodeSelectingCore: function(check) {
		var row = this.FindDataRow(check);		
		if(!ASPx.IsExistsElement(row)) return;
		var nodeKey = this.GetNodeKeyByRow(row);	        
		if(this.selectionDiff[nodeKey])
			delete this.selectionDiff[nodeKey];
		else
			this.selectionDiff[nodeKey] = 1;			
		this.PersistSelectionDiff();			 
		this.UpdateRowStyle(nodeKey);		
    },
    OnSelectingAll: function(state) {
		if(!ASPx.IsExists(this.stateObject.selectAllMark))
			return;
		this.stateObject.selectAllMark = state ? "A" : "N";		
		this.SendDummyCommand();
		this.RaiseSelectionChanged();
    },
    OnFocusingNode: function(key, htmlEvent) {		
		var prevKey = this.GetFocusedNodeKey();
		if(prevKey != key && this.RaiseNodeFocusing(key, htmlEvent)) {
			this.SetFocusedNodeKey(key);
			return this.RaiseFocusedNodeChanged();
		}
		return false;
    },
    OnDataTableMouseDown: function(e) {    
		if(this.syncLock) return;
		if(!ASPx.Evt.IsLeftButtonPressed(e)) return;
		e = ASPx.Evt.GetEvent(e);
        var src = ASPx.Evt.GetEventSource(e);
		var row = this.FindDataRow(src);
		if(!ASPx.IsExistsElement(row))
			return;
		var helper = this.dragHelper;		
		var canDrag = !this.IsEditing();		
		if(canDrag) {
		    var nodeKey = this.GetNodeKeyByRow(row);
		    var targets = this.GetAllDataRows();
		    var header = this.GetHeaderRow();
		    if(ASPx.IsExistsElement(header))
				targets.unshift(header);
		    canDrag = this.RaiseStartDragNode(nodeKey, e, targets);
		    if(canDrag)
			    helper.CreateNodeTargets(targets, row);
		}
        if(canDrag && this.HasHorzScroll()) {
            var extraCell = row.cells[row.cells.length - 1];
            if(extraCell === src || ASPx.GetIsParent(extraCell, src))
                return;
        }
		var drag = helper.CreateNodeDrag(row, e, canDrag);
    },
    OnDataTableClick: function(e) {
		if(this.syncLock) return;
		e = ASPx.Evt.GetEvent(e);
		var source = ASPx.Evt.GetEventSource(e);
		var sourceIsIndent = this.FindIndentCell(source) != null;
		var sourceIsCommandCell = this.FindCommandCell(source) != null;
		var row = this.FindDataRow(source);
		if(!ASPx.IsExistsElement(row))
			return;
		var nodeKey = this.GetNodeKeyByRow(row);
		if(!sourceIsIndent && !sourceIsCommandCell) {
			if(!this.RaiseNodeClick(nodeKey, e))		
				return;
		}
		var tag = source.tagName;
		var className = source.className;
		
		this.selectionStartKey = nodeKey;

		if(tag == "IMG") {
			if(className.indexOf(this.ExpandButtonClassName) > -1) {
				if(this.enableFocusedNode && this.focusOnExpandCollapse)
					this.OnFocusingNode(nodeKey, e);
                if(this.RaiseNodeExpanding(nodeKey, e))
				    this.ExpandNode(nodeKey);				
				return;
			}
			if(className.indexOf(this.CollapseButtonClassName) > -1) {
				if(this.enableFocusedNode && this.focusOnExpandCollapse)
					this.OnFocusingNode(nodeKey, e);
                if(this.RaiseNodeCollapsing(nodeKey, e))
				    this.CollapseNode(nodeKey);
				return;			
			}
		}
		
		if(!sourceIsIndent) {					
			var processOnServer = this.enableFocusedNode ? this.OnFocusingNode(nodeKey, e) : false;
			if(!sourceIsCommandCell 
				&& this.expandCollapseAction == this.ExpandCollapseAction.NodeClick 
				&& this.TryExpandCollapse(nodeKey, row))
				return;				
			if(processOnServer)
				this.SendDummyCommand();
		}
    },
    OnRowCheckBoxClick: function(e) {
        if(!this.IsNodeDragDropEnabled()) {
            var source = ASPx.Evt.GetEventSource(e);
            var row = this.FindDataRow(source);
		    if(!ASPx.IsExistsElement(row))
			    return;
		    var nodeKey = this.GetNodeKeyByRow(row);
		    if(this.enableFocusedNode)
			    this.OnFocusingNode(nodeKey, e);
			this.OnNodeSelecting(source);
		}
    },
    OnDataTableDblClick: function(e) {
		if(this.syncLock) return;
		e = ASPx.Evt.GetEvent(e);		
		var source = ASPx.Evt.GetEventSource(e);
		if(this.FindIndentCell(source) != null)
			return;		
		var row = this.FindDataRow(source);
		if(!ASPx.IsExistsElement(row))
			return;
		var nodeKey = this.GetNodeKeyByRow(row);
		if(!this.IsEditing())
			ASPx.Selection.Clear();				
		if(!this.RaiseNodeDblClick(nodeKey, e))
			return;
		if(this.expandCollapseAction == this.ExpandCollapseAction.NodeDblClick)
			this.TryExpandCollapse(nodeKey, row);
    },
    TryExpandCollapse: function(nodeKey, row) {
        if(nodeKey && nodeKey == this.editingKey)
            return;
		var state = this.GetNodeState(nodeKey, row);
		if(state == "Expanded") {
			this.CollapseNode(nodeKey);
			return true;
		} else if(state == "Collapsed") {
			this.ExpandNode(nodeKey);
			return true;
		}
		return false;
    },  
    OnHeaderMouseDown: function(element, e) {    
		if(this.syncLock) return;
		if(!ASPx.Evt.IsLeftButtonPressed(e)) return;
        if(this.TryStartColumnResizing(e, element))
            return;
		var canDrag = element.id.indexOf(this.DragAndDropTargetMark) > -1;
		var drag = this.dragHelper.CreateHeaderDrag(element, e, canDrag);
		this.dragHelper.CreateHeaderTargets(drag, e);
    },
    OnHeaderClick: function(element, shiftKey, ctrlKey) {
		if(this.syncLock) return;
		var index = this.GetLastNumberOfId(element);
		var column = this.GetColumnByIndex(index);
		if(column != null && column.canSort)
			this.SendSortCommand(index, ctrlKey ? "none" : "", !shiftKey && !ctrlKey);
    },
    OnColumnMoving: function(sourceIndex, targetIndex, before) {    			
		this.SendCommand("MoveColumn", [sourceIndex, targetIndex, before]);
    },
    OnNodeDeleting: function(nodeKey) {
		if(this.confirmDeleteMsg != null && !confirm(this.confirmDeleteMsg))
			return;
		this.DeleteNode(nodeKey);
    },
    OnContextMenu: function(objectType, objectKey, htmlEvent) {
		var e = new ASPxClientTreeListContextMenuEventArgs(objectType, objectKey, htmlEvent);
		if(this.RaiseContextMenu(e))
            ASPx.Evt.PreventEventAndBubble(htmlEvent);
		return e.cancel;
    },
    // Callbacks
    OnCallback: function(result) {
	    this.ProcessCallbackResult(result);
        this.RefreshArmatureRow();
        this.SetHeadersClientEvents();

        var resizingHelper = this.GetResizingHelper();
        if(resizingHelper)
            resizingHelper.ResetStretchedColumnWidth();

        this.UpdateScrollableControls();
        if(resizingHelper)
            resizingHelper.ValidateColumnWidths();

        this.UpdateIndentCellWidths();
        
        if(this.kbdHelper)
            this.kbdHelper.EnsureFocusedNodeVisible();
    },
    OnCallbackFinalized: function() {
        ASPxClientControl.prototype.OnCallbackFinalized.call(this);
        this.AssignEllipsisToolTips();
    },
    OnCallbackError: function(result, data) {
		if(result != "") {
		    this.ShowPopupEditForm();
		    var cell = this.GetErrorCell();
		    if(ASPx.IsExistsElement(cell)) {
			    ASPx.SetElementDisplay(cell.parentNode, true);
			    ASPx.SetInnerHtml(cell, result);
		    } 
		    else 
			    alert(result);
		}
    },
    DoEndCallback: function() {
        this.syncLock = false;
        ASPxClientControl.prototype.DoEndCallback.call(this);
        this.PrepareCommandButtons();
    },
    SendCommand: function(command, args) {
		if(this.syncLock)
			return;		    
		this.SerializeEditorValues();
		this.HidePopupEditForm();
		var id = this.CommandId[command];
		var monoArg = args ? ([id].concat(args)).join(this.SeparatorToken) : id;
		if(this.callBack && this.CanCreateCallback()) {
		    this.ShowLoadingElements();		
		    this.PurgeCaches();
		    this.syncLock = true;
			this.CreateCallback(monoArg, command);
		} else 
			this.SendPostBack(monoArg);		
    },
    SendDummyCommand: function(sync) {
		this.SendCommand("Dummy");
		if(!sync)
		    this.syncLock = false; // B37381
    },
    SendSortCommand: function(columnIndex, order, reset) {
		this.SendCommand("Sort", [columnIndex, order, reset]);
    },    
    SendPagerCommand: function(arg, fromKbd) {
        this.AssignSlideAnimationDirectionByPagerArgument(arg, this.pageIndex);

        var scrollHelper = this.GetScrollHelper();
        if(scrollHelper)
            scrollHelper.ResetScrollTop();

        var list = [ arg ];
        if(fromKbd) list.push("k");
		this.SendCommand("Pager", list);
    },

    CanHandleGesture: function(evt) {
        var source = ASPx.Evt.GetEventSource(evt);
        var table = this.GetDataTable();
        if(ASPx.GetIsParent(table, source))
            return !ASPx.GetIsParent(this.GetHeaderRow(), source);
        if(table.parentNode.tagName == "DIV" && ASPx.GetIsParent(table.parentNode, source))
            return ASPx.Browser.TouchUI || evt.offsetX < table.parentNode.clientWidth;
        return false;
    },
    AllowStartGesture: function() {
        return ASPxClientControl.prototype.AllowStartGesture.call(this) && 
            (this.AllowExecutePagerGesture(this.pageIndex, this.pageCount, 1) || this.AllowExecutePagerGesture(this.pageIndex, this.pageCount, -1));
    },
    AllowExecuteGesture: function(value) {
        return this.AllowExecutePagerGesture(this.pageIndex, this.pageCount, value);
    },
    ExecuteGesture: function(value, count) {
        this.ExecutePagerGesture(this.pageIndex, this.pageCount, value, count, function(arg) { this.SendPagerCommand(arg, false); }.aspxBind(this));
    },

    SendAsyncCommand: function(command, args) {
		if(!this.callBack) return;
		var monoArg = [this.CommandId[command], args].join(this.SeparatorToken);
		this.CreateCallback(monoArg, command);
    },
	SendGetNodeValuesCommand: function(handler, mode, nodeKey, fieldNames) {
		if(fieldNames === null) fieldNames = "";
		var index = this.GetCallbackHandlerIndex(handler);
		var monoArg = ([index, mode, this.EscapeNodeKey(nodeKey)].concat(fieldNames)).join(this.SeparatorToken);
		this.SendAsyncCommand("GetNodeValues", monoArg);		
	},
	HidePopupEditForm: function() {
	    var popup = this.GetPopupEditForm();
	    if(popup)
	        popup.Hide();
	},
	ShowPopupEditForm: function() { 
	    var popup = this.GetPopupEditForm();
	    if(popup && !popup.IsVisible())
	        popup.Show();
	},
    ShowLoadingPanel: function() {
		this.CreateLoadingPanelWithAbsolutePosition(this.GetUpdatableCell(), this.GetLoadingPanelOffsetElement(this.GetUpdatableCell()));
    },
    ShowLoadingDiv: function() {
        this.CreateLoadingDiv(this.GetUpdatableCell());
    },
    GetCallbackAnimationElement: function() {
        var table = this.GetDataTable();
        if(table && table.parentNode && table.parentNode.tagName == "DIV")
            return table.parentNode;
        return table;
    },
    NeedPreventTouchUIMouseScrolling: function(element) {
        return this.IsHeaderChild(element);
    },
    IsHeaderChild: function(source) {
        var headerRow = this.GetHeaderRow();
        if(headerRow && ASPx.GetIsParent(headerRow, source))
            return true;
        return false;
    },

    ProcessCallbackResult: function(resultObj) {
		if(resultObj.customData) {			
			if(ASPx.IsExists(resultObj.handler)) {
				var index = parseInt(resultObj.handler);
				var handler = this.callbackHandlersPool[index];
				this.callbackHandlersPool[index] = null;
				handler(resultObj.data);
			} else {
				this.RaiseCustomDataCallback(resultObj.arg, resultObj.data);
			}
			return;
		}
		this.PurgeCaches();
		// state
        this.UpdateStateObjectWithObject(resultObj.stateObject);
		// focused node
		if(ASPx.IsExists(resultObj.fkey))
			this.SetFocusedNodeKeyInternal(resultObj.fkey);
		// selection inputs
		if(ASPx.IsExists(this.stateObject.selection))
			this.stateObject.selection = "";
		if(ASPx.IsExists(this.stateObject.selectAllMark))
			this.stateObject.selectAllMark = "";
	    // pager
	    if(ASPx.IsExists(resultObj.pi))
	        this.pageIndex = resultObj.pi;
        if(ASPx.IsExists(resultObj.ps))
            this.pageSize = resultObj.ps;
	    if(ASPx.IsExists(resultObj.pc))
	        this.pageCount = resultObj.pc;
		// custom properties
		if(resultObj.cp) {			
			for(var name in resultObj.cp)
				this[name] = resultObj.cp[name];
		}
			
		if(ASPx.IsExists(resultObj.partial))
			this.ProcessPartialCallbackResult(resultObj);
		else
			this.ProcessFullCallbackResult(resultObj);
        if(this.checkBoxImageProperties)
            this.CreateInternalCheckBoxCollection();
				
		this.maxVisibleLevel = resultObj.level;
		this.visibleColumnCount = resultObj.visColCount;
		this.rowCount = resultObj.rows;
		this.editingKey = ASPx.IsExists(resultObj.editingKey) ? resultObj.editingKey : null;
		this.isNewNodeEditing = ASPx.IsExists(resultObj.newNode);
        this.cButtonIDs = resultObj.cButtonIDs;
    },    
    ProcessPartialCallbackResult: function(resultObj) {
		this.UpdateFirstDataCellSpans(resultObj.level);

		var row = this.GetRowByNodeKey(resultObj.partial);
		if(!ASPx.IsExistsElement(row))
			return;
		if(resultObj.remove)
			this.KillNextRows(row, resultObj.remove);
			
        var uid = "";
        var cell = this.GetUpdatableCell();
        var html = cell.innerHTML;
        do {
            uid = this.GenerateUID();
        } while(html.indexOf(uid) > -1);
        
        var placeholder = document.createTextNode(uid);        
        row.parentNode.insertBefore(placeholder, row);
        this.DestroyHtmlRow(row);        
       
        this.HideLoadingElements();
        var data = resultObj.data;
        data = data.replace('$\'', '$$$$\''); // Q439341

        html = cell.innerHTML.replace(/<script(.|\s)*?\/script>/ig, "");        
        html = html.replace(uid, data);
        ASPx.SetInnerHtml(cell, html);
    },
    ProcessFullCallbackResult: function(resultObj) {
		ASPx.SetInnerHtml(this.GetUpdatableCell(), resultObj.data);
    },
    
    // Elements
    GetDataTable:					function() { return this.GetChildElement("D"); },
    GetHeaderTable:                 function() { return this.GetChildElement("H"); },
    GetFooterTable:                 function() { return this.GetChildElement("F"); },
    GetUpdatableCell:				function() { return this.GetChildElement("U"); },        	
	GetStyleTable:					function() { return this.GetChildElement("ST"); },
    GetDragAndDropArrowDownImage:	function() { return this.GetChildElement("DAD"); },
    GetDragAndDropArrowUpImage:		function() { return this.GetChildElement("DAU"); },
    GetDragAndDropHideImage:		function() { return this.GetChildElement("DH"); },
    GetDragAndDropNodeImage:		function() { return this.GetChildElement("DN"); },
    GetErrorCell:                   function() { return this.GetChildElement(this.GetPopupEditForm() ? this.PopupEditFormSuffix + "_Error" : "Error"); },
    GetHeaderRow:					function() { return this.GetChildElement("HDR"); },

    GetHeaderCell: function(columnIndex) {
        var id = this.name + this.GetHeaderSuffix(true) + columnIndex;
        var header = ASPx.GetElementById(id);
        if(header) return header;
        id = this.name + this.GetHeaderSuffix(false) + columnIndex;
        return ASPx.GetElementById(id);
    },
    
    GetCustomizationWindow: function() { 
		return ASPx.GetControlCollection().Get(this.name + this.GetCustomizationWindowSuffix()); 
	},
	GetCustomizationWindowElement: function() {
		var win = this.GetCustomizationWindow();
		return win ? win.GetWindowElement(-1) : null;		
	},
	GetPopupEditForm: function() { 
	    return ASPx.GetControlCollection().Get(this.name + "_" + this.PopupEditFormSuffix); 
	},
	GetHeaderSuffix: function(allowDragDrop) {
        var suffix = "H-";
        if(allowDragDrop)
            suffix = this.DragAndDropTargetMark + suffix;
        else
            suffix = "_" + suffix;
        return suffix;
    },
	GetCustomizationWindowSuffix: function() { return this.DragAndDropTargetMark + "CW"; },
	GetEmptyHeaderSuffix: function() { return this.DragAndDropTargetMark + "EH"; },	
       
    // Columns
    CreateColumn: function(index, name, fieldName, canSort, showInCw, minWidth) {
		var column = new ASPxClientTreeListColumn(index, name, fieldName);
		column.canSort = canSort;
		column.showInCw = showInCw;
        column.minWidth = minWidth;
		this.columns.push(column);
    },
    FindColumn: function(id) {
		if(!ASPx.IsExists(id)) return null;
		if(id.__dxColumnObject)
			return id;
		if(typeof id == "number")
			return this.GetColumnByIndex(id);
		var result = this.GetColumnByName(id);
		if(result == null)
			result = this.GetColumnByFieldName(id);
		return result;
    },
    
    // Utils
    UpdateRowStyle: function(nodeKey) {
		if(!this.allowStylizeEditingNode && nodeKey == this.editingKey)
			return;
		var row = this.GetRowByNodeKey(nodeKey);
		if(!ASPx.IsExistsElement(row))
			return;
			
		var isFocused = this.focusedKey == nodeKey;
		var isSelected = this.IsRowSelected(row);
		
		if(isFocused) {
			this.ApplyRowStyle(row, 0);
		} else if(isSelected) {
			this.ApplyRowStyle(row, 1);
		} else {		
			var index = row.rowIndex;
			this.ApplyRowStyle(row, 2 + index);
		}
    },
    ApplyRowStyle: function(row, index) {
		var styledCells = this.GetStyleTable().rows[0].cells;
		var max = styledCells.length - 1;
		if(index > max)
			index = max; // the case of simple style-table
		row.className = styledCells[index].className;
		row.style.cssText = styledCells[index].innerHTML;
    },
    GetNodeKeyByRow: function(row) {
		return this.UnescapeNodeKey(row.id.substr(this.rowIdPrefix.length));
    },
    GetRowByNodeKey: function(nodeKey) { 		
		var id = this.RowIDSuffix + this.EscapeNodeKey(nodeKey);		
		return this.GetChildElement(id);		
    },
    EscapeNodeKey: function(value) {
		return String(value).replace(/[^A-Za-z0-9]/g, function(match) { 
			return "_" + match.charCodeAt(0) + "_";
		});
    },
    UnescapeNodeKey: function(value) {
		return value.replace(/_\d+_/g, function(match) { 
			return String.fromCharCode(match.substr(1, match.length - 2));
		});
    },
    GetAllDataRows: function() {
        var result = [ ];
		var rows = this.GetDataTable().rows;
		for(var i = 0; i < rows.length; i++) {
			var row = rows[i];			
			if(!this.IsElementDataRow(row)) continue;
			result.push(row);
		}
		return result;
    },        
    IsElementEmptyHeader: function(element) {
		return element.id == this.name + this.GetEmptyHeaderSuffix();
    },
    IsElementDataRow: function(element) {
		return element.tagName == "TR" && element.id.indexOf(this.rowIdPrefix) == 0;
    },    
    IsElementIndentCell: function(element) {
		return ASPx.ElementContainsCssClass(element, "dxtl__I");
    },    
    IsElementCommandCell: function(element) {
		return ASPx.ElementContainsCssClass(element, "dxtl__cc");
    },    
    FindDataRow:		function(element) { return this.FindElementUpwardsCore(element, this.IsElementDataRow); },
    FindIndentCell:		function(element) { return this.FindElementUpwardsCore(element, this.IsElementIndentCell); },
    FindCommandCell:	function(element) { return this.FindElementUpwardsCore(element, this.IsElementCommandCell); },
    FindElementUpwardsCore: function(startElement, matchEvaluator) {
		var dataTable = this.GetDataTable();
		var element = startElement;
		while(ASPx.IsExistsElement(element) && element != dataTable) {
			if(matchEvaluator.call(this, element))
				return element;
			element = element.parentNode;
		}
		return null;        
    },
    IsRowSelected: function(row) {
		var check = this.FindSelectionCheck(row);
		if(!ASPx.IsExistsElement(check))
			return null;
        var internalCheckBox = this.internalCheckBoxCollection.Get(row.id);
		return internalCheckBox.GetChecked();
    },
    FindSelectionCheck: function(row) {
		return this.FindSelectionCheckBoxes(row)[0];
    },
    FindSelectionCheckBoxes: function(container) {
        var elements = container.getElementsByTagName("INPUT");
		var result = [];
        for(var i = 0; i < elements.length; i++) {
			if(ASPx.GetParentByPartialClassName(elements[i], this.SelectionCellClassName))
				result.push(elements[i]);
		}
		return result;
    },
    FindSelectAllCheckBox: function() {
        return this.GetChildElement(this.SelectAllCheckID);
    },
    PersistSelectionDiff: function() {
		if(!ASPx.IsExists(this.stateObject.selection))
			return;
		var list = [ ];
		for(var key in this.selectionDiff)
			list.push(this.EscapeNodeKey(key));
		this.stateObject.selection = list.join(this.SeparatorToken);
    },
    UpdateFirstDataCellSpans: function(newMaxVisibleLevel) {
        var spanDiff = newMaxVisibleLevel - this.maxVisibleLevel;
		if(spanDiff == 0) 
            return;
        var tables = [ this.GetHeaderTable(), this.GetDataTable(), this.GetFooterTable() ];
        var hasArmRow = this.HasScrolling() || this.AllowResizing() || this.enableEllipsis;
        for(var i = 0; i < tables.length; i++) {
            var table = tables[i];
            if(!table) continue;
            this.UpdateFirstDataCellSpansCore(table, spanDiff, hasArmRow);
        }
        if(hasArmRow)
            this.GetTableHelper().UpdateArmCells(spanDiff);
    },
    UpdateFirstDataCellSpansCore: function(table, spanDiff, hasArmRow) {
        var extraCellCount = (this.HasHorzScroll() ? 1 : 0);
		var rows = table.rows;
		for(var i = hasArmRow ? 1 : 0; i < rows.length; i++) {
			var row = rows[i];
            if(row.id == this.name + this.HiddenEmptyRowID)
                continue;

			var tempCell;
			var firstDataCell;
			var tempCell = row.cells[row.cells.length - 1 - extraCellCount];
			if(tempCell.colSpan > 1)
				firstDataCell = tempCell; // only preview-row can have spanned last cell
			else 
				firstDataCell = row.cells[row.cells.length - this.GetColumnCellCount() - extraCellCount];
			var newSpan = firstDataCell.colSpan + spanDiff;
			if(newSpan > 0)
				firstDataCell.colSpan = newSpan;
		}
    },
    KillNextRows: function(row, count) {		
		while(count-- > 0) {
			var rowToKill = row.nextSibling;
			if(!ASPx.IsExistsElement(rowToKill))
				break;			
			if(rowToKill.nodeType != 1) {	// Gecko
				count++;
				ASPx.RemoveElement(rowToKill);
			} else {
				this.DestroyHtmlRow(rowToKill);
			}			
		}
    },
    DestroyHtmlRow: function(row) {
		for(var i = row.cells.length - 1; i >= 0; i--)
			row.cells[i].innerHTML = "";
		ASPx.RemoveElement(row);		
    },
    SetFocusedNodeKeyInternal: function(key) {
		this.focusedKey = key;
		this.stateObject.focusedKey = key;
    },    
    GetLastNumberOfId: function(element) {
        return this.GetLastNumberOfString(element.id);
    },
    GetLastNumberOfString: function(id) {
        var matches = id.match(/\d+/g);
		if(matches != null) {
			var count = matches.length;
			if(count > 0)
				return parseInt(matches[count - 1], 10);
		}
		return -1;
    },
    GenerateUID: function() {
        var result = "";
        for(var i = 0; i < 16; i++) {
            var num = Math.floor(10000 * (1 + Math.random()));
            result += num.toString(36);
        }
        return result;
    },
    PurgeCaches: function() {		
		this.focusedKey = null;
		this.selectionDiff = { };
        this.visibleColumnIndices = null;
        this.indentCount = -1;
    },
    GetColumnCellCount: function() {
		var count = this.visibleColumnCount;
		if(count < 1) count = 1; // when no columns visible
		return count;
    },
    GetCallbackHandlerIndex: function(handler) {
		for(var i = 0; i < this.callbackHandlersPool.length; i++) {
			if(this.callbackHandlersPool[i] == null) {
				this.callbackHandlersPool[i] = handler;
				return i;
			}
		}
		this.callbackHandlersPool.push(handler);
		return this.callbackHandlersPool.length - 1;
    },
    RefreshArmatureRow: function() {
        if(this.HasScrolling() || this.enableEllipsis)
            return;

        var id = this.name + "_ArmRow";
        
        var row = ASPx.GetElementById(id);
        if(row) 
            ASPx.RemoveElement(row);

        row = this.CreateArmatureRow();
        row.id = id;
      
        var dataTable = this.GetDataTable();
        if(dataTable.tBodies.length > 0)
            dataTable.tBodies[0].appendChild(row);
    },
    CreateArmatureRow: function() {
        var row = document.createElement("TR");        
        
        var colSpan = this.maxVisibleLevel - 1;
        if(this.showRoot)
            colSpan++;
        if(this.FindSelectionCheck(this.GetDataTable()))
            colSpan++;

        if(colSpan > 0) {
            var indentSpaning = document.createElement("TD");
            row.appendChild(indentSpaning);
        indentSpaning.colSpan = colSpan;
        indentSpaning.style.padding = "0 1px";
        }
        
        var strut;

        for(var i = 1; i < this.GetColumnCellCount() + 1; i++) {
            var cell = document.createElement("TD");
            row.appendChild(cell);
            cell.style.padding = "0 1px";
            cell.style.whiteSpace = "normal";
            if(!strut)
                strut = this.CreateArmatureRowStrut();
            cell.innerHTML = strut;
        }

        return row;
    },
    CreateArmatureRowStrut: function() {
        var sb = [ "<div style=\"overflow: hidden; height: 0;\">" ];
        var count = 1000 / (this.GetColumnCellCount() + 1);
        for(var i = 0; i < count; i++)
            sb.push("&nbsp; ");
        sb.push("</div>");
        return sb.join("");
    },
    
    // Editing
    GetEditorObjects: function() {
		var list = [ ];
		var clientObjects = ASPx.GetControlCollection().elements;		
		for(var name in clientObjects) {
			if(name.indexOf(this.name) != 0)
				continue;
			var pos = name.indexOf(this.EditorSuffix);
			if(pos < 0 || name.indexOf("_", pos) > -1)
				continue;
			var obj = clientObjects[name];
			if(!obj.GetMainElement || !ASPx.IsExistsElement(obj.GetMainElement()))
				continue;			
			list.push(obj);
		}
		return list;
    },
    SerializeEditorValues: function() {
		var editors = this.GetEditorObjects();
		var count = editors.length;
		var result = [ count ];						
		for(var i = 0; i < count; i++) {
			var columnIndex = this.GetEditorColumnIndex(editors[i]);
			var value = editors[i].GetValueString();
			var length = -1;
			if(!ASPx.IsExists(value)) {
				value = "";
			} else {
				value = value.toString();
				length = value.length;
			}
			result.push(columnIndex);
			result.push(length);
			result.push(value);
		}
		this.stateObject.editorValues = result.join(this.SeparatorToken);		
    },
    GetEditorColumnIndex: function(editorObject) {
		var name = editorObject.name;
		return name.substr(name.lastIndexOf(this.EditorSuffix) + this.EditorSuffix.length);
    },
    
    // Scrolling
    HasHorzScroll: function() { return this.horzScroll != ASPx.ScrollBarMode.Hidden; },
    HasVertScroll: function() { return this.vertScroll != ASPx.ScrollBarMode.Hidden; },
    HasScrolling: function() { return this.HasHorzScroll() || this.HasVertScroll(); },

    GetTableHelper: function() {
        if(!this.tableHelper && typeof(ASPx.TreeListTableHelper) != "undefined")
            this.tableHelper = new ASPx.TreeListTableHelper(this, "D", "H", "F", this.horzScroll, this.vertScroll);
        return this.tableHelper;
    },
    GetScrollHelper: function() {
        if(!this.HasScrolling()) return;
        if(!this.scrollHelper)
            this.scrollHelper = new ASPx.TreeListTableScrollHelper(this.GetTableHelper());
        return this.scrollHelper;
    },
    UpdateScrollableControls: function(onAdjust) {
        var scrollHelper = this.GetScrollHelper();
        if(scrollHelper)
            scrollHelper.Update();
    },
    UpdateIndentCellWidths: function() {
        if(this.HasScrolling() || this.AllowResizing() || this.enableEllipsis)
            this.GetTableHelper().UpdateIndentCellWidths();
    },
    AdjustControlCore: function() {
        ASPxClientControl.prototype.AdjustControlCore.call(this);
        this.UpdateScrollableControls();
        if(this.AllowResizing())
            this.GetResizingHelper().ValidateColumnWidths();
        this.UpdateIndentCellWidths();
        this.AdjustPagerControls();
    },
    SetWidth: function(width) {
        var scrollHelper = this.GetScrollHelper();
        if(scrollHelper)
            scrollHelper.OnSetWidth(width);
        this.ResetControlAdjustment();
        this.constructor.prototype.SetWidth.call(this, width);
        this.AssignEllipsisToolTips();
    },
    SetHeight: function(height) {
        var scrollHelper = this.GetScrollHelper();
        if(scrollHelper)
            scrollHelper.SetHeight(height);
    },

    // Resizing
    AllowResizing: function() { return this.columnResizeMode != ASPx.ColumnResizeMode.None; },

    GetResizingHelper: function() {
        if(!this.AllowResizing()) return null;
        if(!this.resizingHelper)
            this.resizingHelper = new ASPx.TreeListTableResizingHelper(this.GetTableHelper());
        return this.resizingHelper;
    },

    TryStartColumnResizing: function(e, headerCell) {
        var helper = this.GetResizingHelper();
        if(!helper || !helper.CanStartResizing(e, headerCell))
            return false;

        var column = this.GetColumnByIndex(helper.GetResizingColumnIndex(e, headerCell));
        if(this.RaiseColumnResizing(column))
            return false;

        helper.StartResizing(column.index);
        return true;
    },

    SetHeadersClientEvents: function() {
        if(!this.AllowResizing())
            return;
        var helper = this.GetResizingHelper();
        var attachMouseMove = function(headerCell) { 
            ASPx.Evt.AttachEventToElement(headerCell, "mousemove", function(e) { helper.UpdateCursor(e, headerCell); });
        };
        var indices = this.GetVisibleColumnIndices();
        for(var i = 0; i < indices.length; i++)
            attachMouseMove(this.GetHeaderCell(indices[i]));
    },

    GetVisibleColumnIndices: function() {
        if(!this.visibleColumnIndices)
            this.visibleColumnIndices = this.CreateVisibleColumnIndices();
        return this.visibleColumnIndices;
    },

    CreateVisibleColumnIndices: function() {
        var result = [ ];
        var headerRow = this.GetHeaderRow();
        if(!headerRow || headerRow.cells.length < 1) 
            return result;

        for(var i = 0; i < headerRow.cells.length; i++) {
            var cell = headerRow.cells[i];
            var index = this.GetLastNumberOfString(cell.id);
            if(index > -1)
                result.push(index);
        }
        return result;
    },
    GetIndentCount: function() {
        if(this.indentCount < 0)
            this.indentCount = this.CalculateIndentCount();
        return this.indentCount;
    },

    CalculateIndentCount: function() {
        var result = Math.max(0, this.maxVisibleLevel - 1);
        if(this.showRoot)
            result++;
        if(this.FindSelectAllCheckBox() || this.FindSelectionCheck(this.GetDataTable()))
            result++;
        return result;
    },

    OnBrowserWindowResize: function(e) {
        if(this.AllowResizing() && !this.HasScrolling())
            this.GetResizingHelper().ValidateColumnWidths();
    },

	// Consts
	SeparatorToken: " ",
	RowIDSuffix: "R-",
    SelectAllCheckID: "_SelAll",
	EditorSuffix: "DXEDITOR",
	DragAndDropTargetMark: "_DX-DnD-",		
    ExpandButtonClassName: "dxtl__Expand",
    CollapseButtonClassName: "dxtl__Collapse",
    SelectionCellClassName: "dxtlSelectionCell",
    PopupEditFormSuffix: "PEF",
    HiddenEmptyRowID: "_HER",
    CommandId: {
		Expand:				1,
		Collapse:			2,
		Pager:				3,
		CustomDataCallback:	4,
		MoveColumn:			5,
		Sort:				6,
		Dummy:				8,
		ExpandAll:			9,
		CollapseAll:		10,
		CustomCallback:		11,
		StartEdit:			12,
		UpdateEdit:			14,
		CancelEdit:			15,
		MoveNode:			16,
		DeleteNode:			17,
		StartEditNewNode:	18,
		GetNodeValues:		20
    },    
	ExpandCollapseAction: {
		Button: 0,
		NodeClick: 1,
		NodeDblClick: 2
	},
	GetNodeValuesCommandMode: {	
		ByKey:				0,
		Visible:			1,
		SelectedAll:		2,
		SelectedVisible:	3
	},
    CommandButtonType: { 
        Edit:   0, 
        New:    1, 
        Delete: 2, 
        Update: 3, 
        Cancel: 4 
    },
	// base
	RaiseContextMenu: function(e) {
        var hasContextMenu = !this.ContextMenu.IsEmpty();
		if(hasContextMenu)
			this.ContextMenu.FireEvent(this, e);
        return hasContextMenu;
	},
	RaiseCustomButtonClick: function(nodeKey, buttonIndex, buttonID) {
		var handler = this.CustomButtonClick;
		if(handler.IsEmpty()) return false;
		handler.FireEvent(this, new ASPxClientTreeListCustomButtonEventArgs(nodeKey, buttonIndex, buttonID));
	},
    Focus: function() {
        if(this.kbdHelper)
            this.kbdHelper.Focus();
    },
    
    /**************************************** FOCUSED NODE **************************************/
    GetFocusedNodeKey: function() {
		if(!this.enableFocusedNode)
			return "";
		if(this.focusedKey === null)
			this.focusedKey = this.stateObject.focusedKey;
		return this.focusedKey;
    },
    SetFocusedNodeKey: function(key) {
		if(!this.enableFocusedNode)
			return;
							
		var prevKey = this.GetFocusedNodeKey();		
		this.SetFocusedNodeKeyInternal(key);
		this.UpdateRowStyle(prevKey);	
		this.UpdateRowStyle(key);
    },
    RaiseNodeFocusing: function(nodeKey, htmlEvent) {
		if(this.NodeFocusing.IsEmpty()) return true;
		var args = new ASPxClientTreeListNodeEventArgs(nodeKey, htmlEvent);
		this.NodeFocusing.FireEvent(this, args);
		return !args.cancel;
    },    
    RaiseFocusedNodeChanged: function() {
		var processOnServer = false;
		if(!this.FocusedNodeChanged.IsEmpty()) {
			var args = new ASPxClientProcessingModeEventArgs(processOnServer);
			this.FocusedNodeChanged.FireEvent(this, args);
			processOnServer = args.processOnServer;			
		}
		return this.focusSendsCallback || processOnServer;
    },
    
    /*************************************** SELECTION *********************************************/
    GetNodeCheckState: function(nodeKey) {
        var row = this.GetRowByNodeKey(nodeKey);
		if(!ASPx.IsExistsElement(row))
			return null;
        if(this.FindSelectionCheck(row)) {
            var internalCheckBox = this.internalCheckBoxCollection.Get(row.id);
            if(internalCheckBox)
                return internalCheckBox.GetCurrentCheckState();
        }
        return null;
    },
    SetNodeCheckState: function(nodeKey, checkState) {
        var row = this.GetRowByNodeKey(nodeKey);
		var newInputKey = ASPx.CheckBoxInputKey[checkState];
        if(!ASPx.IsExistsElement(row) || !newInputKey)
			return;			
		var check = this.FindSelectionCheck(row);
		if(!ASPx.IsExistsElement(check))
			return;
        var internalCheckBox = this.internalCheckBoxCollection.Get(row.id);
        if(internalCheckBox.GetCurrentInputKey() == newInputKey)
            return;
        internalCheckBox.SetValue(newInputKey);
		this.OnNodeSelectingCore(check);
    },    
    RaiseSelectionChanged: function() {
		var processOnServer = false;
		if(!this.SelectionChanged.IsEmpty()) {
			var args = new ASPxClientProcessingModeEventArgs(processOnServer);
			this.SelectionChanged.FireEvent(this, args);
			processOnServer = args.processOnServer;
		}
		return this.selectionSendsCallback || processOnServer;
    },
    IsNodeSelected: function(nodeKey) {
        var row = this.GetRowByNodeKey(nodeKey);
        if(row) {
            var nodeCheckState = this.GetNodeCheckState(nodeKey);
            if(nodeCheckState)
                return nodeCheckState === ASPx.CheckBoxCheckState.Checked;
        }
		return null;
    },
    SelectNode: function(nodeKey, state) {    
	    if(!ASPx.IsExists(state)) 
            state = true;			
        this.SetNodeCheckState(nodeKey, state ? ASPx.CheckBoxCheckState.Checked : ASPx.CheckBoxCheckState.Unchecked);
    },
	GetVisibleSelectedNodeKeys: function() {
		var rows = this.GetAllDataRows();
		var result = [ ];
		for(var i = 0; i < rows.length; i++) {
			var key = this.GetNodeKeyByRow(rows[i]);
			if(this.IsNodeSelected(key))
				result.push(key);
		}
		return result;
	},    
                
    /********************************* CUSTOMIZATION WINDOW ****************************************/
    IsCustomizationWindowVisible: function() {
		var win = this.GetCustomizationWindow();
		return win != null && win.IsVisible();
    },
    ShowCustomizationWindow: function(htmlElement) {
		var win = this.GetCustomizationWindow();
		if(win == null)
			return;
		if(!ASPx.IsExistsElement(htmlElement))
			htmlElement = this.GetMainElement();
		win.ShowAtElement(htmlElement);
    },
    HideCustomizationWindow: function() {
		var win = this.GetCustomizationWindow();
		if(win != null)
			win.Hide();
    },
    RaiseCustomizationWindowCloseUp: function() {
		if(!this.CustomizationWindowCloseUp.IsEmpty())
			this.CustomizationWindowCloseUp.FireEvent(this, { } );
	},
	
	/************************************* CUSTOM CALLBACKS ****************************************/
	PerformCustomCallback: function(arg) {
		this.PerformCallback(arg);		
	},
	PerformCallback: function(args) {
		this.SendCommand("CustomCallback", [args]);
	},
	PerformCustomDataCallback: function(arg) {
		this.SendAsyncCommand("CustomDataCallback", arg);
	},
	RaiseCustomDataCallback: function(arg, result) {
		if(!this.CustomDataCallback.IsEmpty()) {
			var e = new ASPxClientTreeListCustomDataCallbackEventArgs(arg, result);
			this.CustomDataCallback.FireEvent(this, e);
		}		
	},
	GetNodeValues: function(nodeKey, fieldNames, onCallback) {
		this.SendGetNodeValuesCommand(onCallback, this.GetNodeValuesCommandMode.ByKey, nodeKey, fieldNames);
	},
	GetVisibleNodeValues: function(fieldNames, onCallback) {
		this.SendGetNodeValuesCommand(onCallback, this.GetNodeValuesCommandMode.Visible, "", fieldNames);
	},
	GetSelectedNodeValues: function(fieldNames, onCallback, visibleOnly) {
		var mode = visibleOnly 
			? this.GetNodeValuesCommandMode.SelectedVisible 
			: this.GetNodeValuesCommandMode.SelectedAll; 
		this.SendGetNodeValuesCommand(onCallback, mode, "", fieldNames);
	},		
	
	/************************************* PAGING *************************************************/
	GoToPage: function(index) {
		if(index < -1)
			return;
		this.SendPagerCommand(ASPx.PagerCommands.PageNumber + index);
	},
	PrevPage: function(fromKbd) {
		this.SendPagerCommand(ASPx.PagerCommands.Prev, fromKbd);
	},
	NextPage: function(fromKbd) {
		this.SendPagerCommand(ASPx.PagerCommands.Next, fromKbd);
	},
    GetPageIndex: function(){
        return this.pageIndex;
    },
    GetPageCount: function(){
        return this.pageCount;
    },

	/*****************************************  NODES  ******************************************/
	GetNodeState: function(nodeKey, row) {		
		if(!row)
			row = this.GetRowByNodeKey(nodeKey);
		if(!ASPx.IsExistsElement(row))
			return "NotFound";
		var children = row.getElementsByTagName("IMG");
		for(var i = 0; i < children.length; i++) {
			var name = children[i].className;
			if(name.indexOf(this.ExpandButtonClassName) > -1)
				return "Collapsed";
			if(name.indexOf(this.CollapseButtonClassName) > -1)
				return "Expanded";
		}
		return "Child";
	},
	ExpandAll: function() {
		this.SendCommand("ExpandAll");
	},
	CollapseAll: function() {
		this.SendCommand("CollapseAll");
	},
	ExpandNode: function(key) {
		this.SendCommand("Expand", [key]);
	},
	CollapseNode: function(key) {
		this.SendCommand("Collapse", [key]);
	},
	GetVisibleNodeKeys: function() {
	    var rows = this.GetAllDataRows();
	    var result = [ ];
	    for(var i = 0; i < rows.length; i++)
	        result.push(this.GetNodeKeyByRow(rows[i]));
	    return result;
	},
	GetNodeHtmlElement: function(nodeKey) {
	    return this.GetRowByNodeKey(nodeKey);
	},
	RaiseNodeClick: function(nodeKey, htmlEvent) {
		if(this.NodeClick.IsEmpty()) return true;
		var e = new ASPxClientTreeListNodeEventArgs(nodeKey, htmlEvent);
		this.NodeClick.FireEvent(this, e);
		return !e.cancel;		
	},
	RaiseNodeDblClick: function(nodeKey, htmlEvent) {
		if(this.NodeDblClick.IsEmpty()) return true;
		var e = new ASPxClientTreeListNodeEventArgs(nodeKey, htmlEvent);
		this.NodeDblClick.FireEvent(this, e);
		return !e.cancel;
	},
	RaiseNodeExpanding: function(nodeKey, htmlEvent) {
		if(!this.NodeExpanding.IsEmpty()) {
			var e = new ASPxClientTreeListNodeEventArgs(nodeKey, htmlEvent);
			this.NodeExpanding.FireEvent(this, e);
			return !e.cancel;
		}
		return true;
	},
	RaiseNodeCollapsing: function(nodeKey, htmlEvent) {
		if(!this.NodeCollapsing.IsEmpty()) {
			var e = new ASPxClientTreeListNodeEventArgs(nodeKey, htmlEvent);
			this.NodeCollapsing.FireEvent(this, e);
			return !e.cancel;
		}
		return true;
	},
	RaiseStartDragNode: function(nodeKey, htmlEvent, targets) {
	    if(this.StartDragNode.IsEmpty()) return true;
	    var e = new ASPxClientTreeListStartDragNodeEventArgs(nodeKey, htmlEvent, targets);
	    this.StartDragNode.FireEvent(this, e);
	    return !e.cancel;
	},
	RaiseEndDragNode: function(nodeKey, htmlEvent, targetElement) {
	    if(this.EndDragNode.IsEmpty()) return true;
	    var e = new ASPxClientTreeListEndDragNodeEventArgs(nodeKey, htmlEvent, targetElement);
	    this.EndDragNode.FireEvent(this, e);
	    return !e.cancel;
	},
		
	/******************************************** COLUMNS ****************************************/
	GetVisibleColumnCount: function() {
		return this.visibleColumnCount;
	},
	GetColumnCount: function() {
		return this.columns.length;
	},
    GetColumnByIndex: function(index) {
		for(var i = 0; i < this.columns.length; i++) {
			if(this.columns[i].index == index)
				return this.columns[i];
		}
		return null;
    },
    GetColumnByName: function(name) {
		if(name == "")
			return null;
		for(var i = 0; i < this.columns.length; i++) {
			if(this.columns[i].name == name)
				return this.columns[i];
		}
		return null;
    },
    GetColumnByFieldName: function(fieldName) {
		if(fieldName == "")
			return null;
		for(var i = 0; i < this.columns.length; i++) {
			if(this.columns[i].fieldName == fieldName)
				return this.columns[i];
		}
		return null;
    },
    SortBy: function(columnId, order, reset) {
		var column = this.FindColumn(columnId);
		if(column == null)
			return;
    
		if(!ASPx.IsExists(order)) order = "";
		if(!ASPx.IsExists(reset)) reset = true;
		this.SendSortCommand(column.index, order, reset);
    },
    
    /******************************************** EDITING ****************************************/
    StartEdit: function(nodeKey) {
		this.SendCommand("StartEdit", [nodeKey]);
    },
    UpdateEdit: function() {
		if(!this.IsEditing() || !this.ValidateEditors()) return;
		this.SendCommand("UpdateEdit");
    },
    ValidateEditors: function(){
        var editors = this.GetEditorObjects();		
		for(var i = 0; i < editors.length; i++) {
			var editor = editors[i];
            if(!editor.Validate) continue;
			editor.Validate();
			if(!editor.GetIsValid()) {
				if(editor.setFocusOnError)
					editor.Focus();
				return false;
			}
		}
        return true;
    },
    CancelEdit: function() {
		if(!this.IsEditing()) return;
		this.SendCommand("CancelEdit");
    },
    IsEditing: function() { 
		return this.editingKey != null || this.isNewNodeEditing;
	},
    GetEditingNodeKey: function() { 
		return this.editingKey; 
	},
	MoveNode: function(nodeKey, parentNodeKey) {
		this.SendCommand("MoveNode", [this.EscapeNodeKey(nodeKey), this.EscapeNodeKey(parentNodeKey)]);
	},
	DeleteNode: function(nodeKey) {
		this.SendCommand("DeleteNode", [nodeKey]);
	},
	StartEditNewNode: function(parentNodeKey) {
		if(!ASPx.IsExists(parentNodeKey))
			parentNodeKey = "";
		this.SendCommand("StartEditNewNode", [parentNodeKey]);
	},
	GetEditor: function(columnId) {
		var column = this.FindColumn(columnId);
		if(column == null) return null;
		var editors = this.GetEditorObjects();
		for(var i = 0; i < editors.length; i++) {
			if(column.index == this.GetEditorColumnIndex(editors[i]))
				return editors[i];
		}
		return null;
	},	
	GetEditValue: function(columnId) {
		var editor = this.GetEditor(columnId);
		if(editor == null) return null;
		return editor.GetValue();
	},		
	SetEditValue: function(columnId, value) {
		var editor = this.GetEditor(columnId);
		if(editor != null)
			editor.SetValue(value);
	},	
	FocusEditor: function(columnId) {
		var editor = this.GetEditor(columnId);
		if(editor && editor.SetFocus)
			editor.SetFocus();
	},

    /******************************************** SCROLLING ****************************************/
    MakeNodeVisible: function(nodeKey) {
        if(!this.HasVertScroll()) return;
        var row = this.GetRowByNodeKey(nodeKey);
        if(!row) return;
        this.GetScrollHelper().MakeRowVisible(row);
    },
    GetVerticalScrollPosition: function() {
        var scrollHelper = this.GetScrollHelper();
        if(scrollHelper)
            return scrollHelper.GetVertScrollPosition();
        return 0;
    },
    GetHorizontalScrollPosition: function() {
        var scrollHelper = this.GetScrollHelper();
        if(scrollHelper)
            return scrollHelper.GetHorzScrollPosition();
        return 0;
    },
    SetVerticalScrollPosition: function(value) {
        var scrollHelper = this.GetScrollHelper();
        if(scrollHelper)
            scrollHelper.SetVertScrollPosition(value);
    },
    SetHorizontalScrollPosition: function(value) {
        var scrollHelper = this.GetScrollHelper();
        if(scrollHelper)
            scrollHelper.SetHorzScrollPosition(value);
    },
    
    /******************************************** RESIZING ****************************************/
    RaiseColumnResizing: function(column) {
        if(!this.ColumnResizing.IsEmpty()) {
            var args = new ASPxClientTreeListColumnResizingEventArgs(column);
            this.ColumnResizing.FireEvent(this, args);
            return args.cancel;
        }
        return false;
    },
    RaiseColumnResized: function(column) {
        if(!this.ColumnResized.IsEmpty()) {
            var args = new ASPxClientTreeListColumnResizedEventArgs(column);
            this.ColumnResized.FireEvent(this, args);
            if(args.processOnServer)
                this.SendDummyCommand();
        }
    }
}); 
ASPxClientTreeList.Cast = ASPxClientControl.Cast;

var TreeListDragHelper = ASPx.CreateClass(null, {
	constructor: function(treeList)  {
		this.treeList = treeList;
        this.rect = null;
        this.nodeTargets = null;
	},
	  
	// Header dragging
    CreateHeaderDrag: function(element, e, canDrag) {		
		e = ASPx.Evt.GetEvent(e);
	    var drag = new ASPx.DragHelper(e, element, true);	    
        drag.centerClone = true;
	    drag.canDrag = canDrag;
	    drag.ctrl = e.ctrlKey;
	    drag.shift = e.shiftKey;
	    drag.treeListHelper = this;
        
        drag.onDoClick = this.OnHeaderClick;
        drag.onDragDivCreating = this.OnHeaderDragDivCreating;
        drag.onCloneCreating = this.OnHeaderCloneCreating;
        drag.onEndDrag = this.OnHeaderEndDrag;
        drag.onCancelDrag = this.OnHeaderCancelDrag;

        if(this.treeList.HasHorzScroll())
            this.SaveScrollContainerDimension();
        
        return drag;		
    },
	CreateHeaderTargets: function(drag, e) {
	    if(!drag.canDrag)
			return;
			
        var targets = new ASPx.CursorTargets();                
        targets.obj = drag.obj;
        targets.treeListHelper = this;

        targets.onTargetAdding = this.OnHeaderTargetAdding;
        targets.onTargetChanging = this.OnHeaderTargetChanging; 
        targets.onTargetChanged = this.OnHeaderTargetChanged;
        var tree = this.treeList;
        
        targets.RegisterTargets(tree.GetHeaderRow(),
			[ tree.GetEmptyHeaderSuffix(), tree.GetHeaderSuffix(true) ]);

		var cw = tree.GetCustomizationWindowElement();
		if(cw != null) {
			var index = this.treeList.GetLastNumberOfId(drag.obj);
			var columnObj = this.treeList.FindColumn(index);
			if(columnObj && columnObj.showInCw)
			    targets.RegisterTargets(cw, [ tree.GetCustomizationWindowSuffix() ]);
		}
	},
    SaveScrollContainerDimension: function() {
        var scrollHelper = this.treeList.GetScrollHelper();
        var scrollableControl = scrollHelper.GetHorzScrollableControl() || scrollHelper.GetVertScrollableControl();

        var scrollDivX = ASPx.GetAbsoluteX(scrollableControl);
        var scrollDivY = ASPx.GetAbsoluteY(scrollableControl);

        var header = this.treeList.GetHeaderRow();
        var headerY = ASPx.GetAbsoluteY(header);
        
        this.rect = { 
            left: scrollDivX, 
            right: scrollDivX + scrollableControl.offsetWidth - this.GetExtraCellWidth(),
            top: ASPx.IsExistsElement(header) ? headerY : scrollDivY,
            bottom: scrollDivY + scrollableControl.offsetHeight
        };
    },
    OnHeaderTargetAdding: function(targets, targetElement) {
        if(!targetElement) 
            return false;
        var helper = targets.treeListHelper;
        if(targetElement.id.indexOf(helper.treeList.GetCustomizationWindowSuffix()) > 0)
            return true;
        if(helper.rect) {
            var targetX = ASPx.GetAbsoluteX(targetElement);
            return targetX >= helper.rect.left && targetX + targetElement.offsetWidth <= helper.rect.right
        }
        return true;
    },
	OnHeaderClick: function(drag) {
		var treeList = drag.treeListHelper.treeList;
		treeList.OnHeaderClick(drag.obj, drag.shift, drag.ctrl);
	},
    OnHeaderDragDivCreating: function(drag, dragDiv) {
        drag.treeListHelper.AssignTreeListMainElementStyles(dragDiv);
	},
    OnHeaderCloneCreating: function(clone) {    
		clone.colSpan = 1;	// when we drag the first header		
		TreeListDragHelper.RestoreElementBorder(clone);		
		var table = document.createElement("table");
		table.cellSpacing = 0;
		var row = table.insertRow(-1);
		row.appendChild(clone);
		table.style.width = Math.min(200, this.obj.offsetWidth) + "px";
		table.style.opacity = 0.80;
		table.style.filter = "alpha(opacity=80)";    
        if(ASPx.IsElementRightToLeft(this.obj))
            table.dir = "rtl";
		return table;
    },
    OnHeaderEndDrag: function(drag) {
        if(drag.targetElement == null) 
			return;		
		var treeList = drag.treeListHelper.treeList;
		var sourceIndex = treeList.GetLastNumberOfId(drag.obj);
		var targetIndex;
		var cwElement = treeList.GetCustomizationWindowElement();
		if(cwElement && drag.targetElement.id == cwElement.id) {
			targetIndex = -1;
		} else if(treeList.IsElementEmptyHeader(drag.targetElement)) {
			targetIndex = 0;
		} else {
			targetIndex = treeList.GetLastNumberOfId(drag.targetElement);
			if(sourceIndex == targetIndex)
				return;
		}
		var before = drag.targetTag;
		if(treeList.rtl)
		    before = !before;
		treeList.OnColumnMoving(sourceIndex, targetIndex, before);
    },    	
    OnHeaderCancelDrag: function(drag) {
		if(drag.canDrag)
			drag.treeListHelper.HideHeaderDragImages();
    },
    
    OnHeaderTargetChanging: function(targets) {
		targets.targetTag = targets.isLeftPartOfElement();
    },        
    OnHeaderTargetChanged: function(targets) {    
		if(ASPx.currentDragHelper == null) 
			return;
							
		var helper = targets.treeListHelper;
		var element = targets.targetElement;
		helper.HideHeaderDragImages();
		if(element && element != ASPx.currentDragHelper.obj) {			
			ASPx.currentDragHelper.targetElement = element;
			ASPx.currentDragHelper.targetTag = targets.targetTag;			
			var left = ASPx.GetAbsoluteX(element);
			if(!targets.targetTag) {
                var brother = element;
                do {
                    var brother = helper.treeList.rtl ? brother.previousSibling : brother.nextSibling;
                } while(brother && brother.nodeType != 1);
			    if(brother)
			        left = ASPx.GetAbsoluteX(brother);
			    else
				    left += element.offsetWidth;
			}
				
			if(element == helper.treeList.GetCustomizationWindowElement()) {
				var hideImage = helper.treeList.GetDragAndDropHideImage();
				hideImage.style.top = "";
				ASPx.currentDragHelper.addElementToDragDiv(hideImage);
			} else {
				helper.SetHeaderDragImagesPosition(element, left);
			}
			
		} else {
			ASPx.currentDragHelper.targetElement = null;
		}
    },
    
    SetHeaderDragImagesPosition: function(element, left) {
        this.ShowHeaderDragImages();
        var arrowDown = this.treeList.GetDragAndDropArrowDownImage();
        var arrowUp = this.treeList.GetDragAndDropArrowUpImage();
        
        ASPx.SetAbsoluteX(arrowDown, Math.ceil(left - arrowDown.offsetWidth / 2));
        ASPx.SetAbsoluteX(arrowUp, Math.ceil(left - arrowUp.offsetWidth / 2));
        
        var top = ASPx.GetAbsoluteY(element);
        ASPx.SetAbsoluteY(arrowDown, top - arrowDown.offsetHeight);
        ASPx.SetAbsoluteY(arrowUp, top + element.offsetHeight);
    },
    HideHeaderDragImages: function() {
		this.SetHeaderDragImagesVisibility("hidden");
        if(ASPx.currentDragHelper != null)
            ASPx.currentDragHelper.removeElementFromDragDiv();		
    },
    ShowHeaderDragImages: function() {
		this.SetHeaderDragImagesVisibility("visible");
    },
    SetHeaderDragImagesVisibility: function(value) {
		this.treeList.GetDragAndDropArrowDownImage().style.visibility = value;
		this.treeList.GetDragAndDropArrowUpImage().style.visibility = value;
    },
    AssignTreeListMainElementStyles: function(target) {
        var mainElement = this.treeList.GetMainElement();
	    if(!target || !mainElement) return;
	    target.className = mainElement.className;
	    target.style.cssText = mainElement.style.cssText;
    },
        
    // Node dragging
    CreateNodeDrag: function(row, e, canDrag) {
		e = ASPx.Evt.GetEvent(e);
		var drag = new ASPx.DragHelper(e, row, true);
		drag.__treeList = this.treeList;
		drag.canDrag = canDrag;
        drag.onDragDivCreating = this.OnNodeDragDivCreating;
		drag.onCloneCreating = this.OnNodeCloneCreating;
		drag.onCancelDrag = this.OnNodeCancelDrag;
		drag.onEndDrag = this.OnNodeEndDrag;
		drag.onDoClick = this.OnNodeClick;

        if(this.treeList.HasScrolling()) {
            this.SaveScrollContainerDimension();
            if(this.treeList.HasVertScroll())
                this.ScrollControlManually();
        }
		return drag;
    },
    CreateNodeTargets: function(targetElements, sourceElement) {
		this.nodeTargets = new ASPx.CursorTargets();
		this.nodeTargets.__treeList = this.treeList;
		for(var i = 0; i < targetElements.length; i++) {
			if(targetElements[i] == sourceElement) continue;
		    this.nodeTargets.list.push(new ASPx.CursorTarget(targetElements[i]));
		}
        this.nodeTargets.onTargetAllowed = function(targetElement, x, y) { return this.OnNodeTargetAllowed(targetElement, x, y); }.aspxBind(this);
		this.nodeTargets.onTargetChanged = this.OnNodeTargetChanged;
    },
    OnNodeDragDivCreating: function(drag, dragDiv) {
        drag.__treeList.dragHelper.AssignTreeListMainElementStyles(dragDiv);
	},
    OnNodeCloneCreating: function(row) {
		var treeList = ASPx.currentDragHelper.__treeList;
        if(treeList.GetHorizontalScrollPosition() > 0)
           treeList.SetHorizontalScrollPosition(0); 
        var helper = treeList.dragHelper;
		var table = document.createElement("table");
		table.cellSpacing = 0;		
		var tbody = document.createElement("tbody");
		table.appendChild(tbody);
		tbody.appendChild(row);
		var list = [ ];
        var extraCellCount = helper.GetExtraCellCount();
		var thr = row.cells.length - treeList.GetColumnCellCount() - extraCellCount;
        var lastDataCellIndex = row.cells.length - extraCellCount - 1;
		var originalRow = ASPx.currentDragHelper.obj;
		var removedWidth = 0;
		for(var i = 0; i < row.cells.length; i++) {
			var cell = row.cells[i];
            var originalCell = originalRow.cells[i];
			if(i < thr || i > lastDataCellIndex) {
				list.push(cell);
				removedWidth += originalCell.offsetWidth;
			} else {
				TreeListDragHelper.RestoreElementBorder(cell, "top");
				TreeListDragHelper.RestoreElementBorder(cell, "bottom");
				if(i == thr)
					TreeListDragHelper.RestoreElementBorder(cell, "left");
				if(i == lastDataCellIndex)
					TreeListDragHelper.RestoreElementBorder(cell, "right");
                cell.style.width = originalCell.offsetWidth - 
                    ASPx.GetLeftRightBordersAndPaddingsSummaryValue(originalCell) + "px";
			}				
		}
		for(var i = 0; i < list.length; i++)
			row.removeChild(list[i]);
		table.width = (originalRow.offsetWidth - removedWidth).toString() + "px";
		table.style.marginLeft = removedWidth - helper.GetExtraCellWidth() + "px";
		table.style.opacity = 0.80;
		table.style.filter = "alpha(opacity=80)";
		return table;
    },
    OnNodeTargetAllowed: function(targetElement, x, y) {
        if(!this.rect)
            return true;
        if(this.treeList.HasScrolling() && !ASPx.GetIsParent(this.treeList.GetMainElement(), targetElement))
            return true;
        return x >= this.rect.left && x <= this.rect.right && y >= this.rect.top && y <= this.rect.bottom;
    },
    OnNodeTargetChanged: function(targets) {
		if(ASPx.currentDragHelper == null) return;
		var element = targets.targetElement;
		var hasTarget = ASPx.IsExistsElement(element);
		targets.__treeList.dragHelper.SetNodeImageVisibility(hasTarget, element);
		ASPx.currentDragHelper.targetElement = hasTarget ? targets.targetElement : null;
    },
    OnNodeCancelDrag: function(drag) {
        var helper = drag.__treeList.dragHelper;
		helper.SetNodeImageVisibility(false);

        helper.nodeTargets = null;
        if(helper.mouseMoveHandler)
            ASPx.Evt.DetachEventFromDocument("mousemove", helper.mouseMoveHandler);
        helper.CancelScrolling();
    },
    OnNodeEndDrag: function(drag, e) {
        if(drag.targetElement == null) 
			return;
		var sourceRow = drag.obj;
		var targetElement = drag.targetElement;
		if(sourceRow == targetElement)
			return;
		var treeList = drag.__treeList;
		var nodeKey = treeList.GetNodeKeyByRow(sourceRow);		
		if(!treeList.RaiseEndDragNode(nodeKey, e, targetElement))
		    return;				
		if(treeList.IsElementDataRow(targetElement)) {
		    var parentKey = treeList.GetNodeKeyByRow(targetElement);
		    treeList.MoveNode(nodeKey, parentKey);
		} else if(targetElement == treeList.GetHeaderRow()) {
			treeList.MoveNode(nodeKey, "");
		} else {
		    alert("Unprocessed custom target id=" + targetElement.id);
		}
    },
	OnNodeClick: function(drag, e) {
		drag.__treeList.OnDataTableClick(e);
	},    
    SetNodeImageVisibility: function(visible, element) {
        if(element == document.body)
            visible = false;
		var img = this.treeList.GetDragAndDropNodeImage();
		img.style.visibility = visible ? "visible" : "hidden";
		if(!visible) return;
		if(element.cells && element.cells.length > 0) {            
			for(var i = element.cells.length - 1; i >= 0; i--) {
				if(element.cells[i].className.indexOf("dxtl__I") > -1) {          
					element = element.cells[1 + i];
					break;
				}
			}
		}
		ASPx.SetAbsoluteX(img, ASPx.GetAbsoluteX(element) - img.offsetWidth + 4);
		ASPx.SetAbsoluteY(img, ASPx.GetAbsoluteY(element) + Math.floor(0.5 * (element.clientHeight - img.clientHeight)));
    },
    GetExtraCellCount: function() {
        var tableHelper = this.treeList.GetTableHelper();
        if(tableHelper)
            return tableHelper.GetExtraCellCount();
        return 0;
    },
    GetExtraCellWidth: function() {
        var tableHelper = this.treeList.GetTableHelper();
        if(tableHelper)
            return tableHelper.GetExtraCellWidth();
        return 0;
    },

    // Scrolling
    OnVerticalScroll: function() {
        if(!this.nodeTargets || this.nodeTargets.list.length == 0)
            return;
        this.updateTargetPositionTimerID = ASPx.Timer.ClearTimer(this.updateTargetPositionTimerID);
        this.updateTargetPositionTimerID = window.setTimeout(function() { this.UpdateTargetProsition(); }.aspxBind(this), 50);
    },
    UpdateTargetProsition: function() {
        if(!this.nodeTargets || this.nodeTargets.list.length == 0)
            return;
        for(var i = 0; i < this.nodeTargets.list.length; i++)
            this.nodeTargets.list[i].UpdatePosition();
    },
    ScrollControlManually: function() {
        if(!ASPx.Browser.IE && !ASPx.Browser.Firefox)
            return;
        this.mouseMoveHandler = function(e) { this.OnMouseMove(e); }.aspxBind(this);
        ASPx.Evt.AttachEventToDocument("mousemove", this.mouseMoveHandler);
    },
    OnMouseMove: function(e) {
        var y = ASPx.Evt.GetEventY(e);
        if(this.savedY === y) 
            return;
        this.savedY = y;
        
        if(y >= this.rect.top && y <= this.rect.bottom) {
            this.CancelScrolling();
            return;
        }

        var isTop = y < this.rect.top;
        var diff = isTop ? this.rect.top - y : y - this.rect.bottom;

        this.repeatScrollTimeout = diff < 30 ? 30 - diff : 1;
        this.scrollTopInc = isTop ? -1 : 1;
        
        this.StartScrolling(diff, isTop);
    },
    StartScrolling: function(diff, isTop) {
        if(this.scrollingProcessing)
            return;
        this.scrollingProcessing = true;
        this.scrollHandler = this.RepeatScrolling();
    },
    CancelScrolling: function() {
        this.scrollingProcessing = false;
        this.scrollHandler = ASPx.Timer.ClearTimer(this.scrollHandler);
    },
    RepeatScrolling: function(time, inc) {
        var scrollableControl = this.treeList.GetScrollHelper().GetVertScrollableControl();
        scrollableControl.scrollTop += this.scrollTopInc;
        return window.setTimeout(function() { this.scrollHandler = this.RepeatScrolling(); }.aspxBind(this), this.repeatScrollTimeout);
    }
});
TreeListDragHelper.RestoreElementBorder = function(element, borderPart) {
	var ruleName = borderPart ? "border-" + borderPart + "-style" : "border-style";
	element.style.cssText += ";" + ruleName + ": solid!important;";
};

var TreeListKbdHelper = ASPx.CreateClass(ASPx.KbdHelper, {

    HandleKeyDown: function(e) {
        var tree = this.control;
        var row = tree.GetRowByNodeKey(tree.GetFocusedNodeKey());
        var busy = tree.syncLock;
        var key = ASPx.Evt.GetKeyCode(e);
        
        if(tree.rtl) {
            if(key == ASPx.Key.Left)
                key = ASPx.Key.Right;
            else if(key == ASPx.Key.Right)
                key = ASPx.Key.Left;
        }
                
        switch(key) {
        
            case ASPx.Key.Down:
                if(!busy)
                    this.TryMoveFocusDown(row, e.shiftKey);
                return true;
                
            case ASPx.Key.Up:
                if(!busy)
                    this.TryMoveFocusUp(row, e.shiftKey);
                return true;
                
            case ASPx.Key.Right:
                if(!busy) {
                    if(!this.TryExpand(row, e))
                        this.TryMoveFocusDown(row, e.shiftKey);
                }
                return true;
            
            case ASPx.Key.Left:
                if(!busy) {
                    if(!this.TryCollapse(row, e))
                        this.TryMoveFocusUp(row, e.shiftKey);
                }
                return true;
                
            case ASPx.Key.PageDown:
                if(e.shiftKey) {
                    if(!busy && tree.pageIndex < tree.pageCount - 1)
                        tree.NextPage();
                    return true;    
                }
                break;
                
            case ASPx.Key.PageUp:
                if(e.shiftKey) {
                    if(!busy && tree.pageIndex > 0)
                        tree.PrevPage();
                    return true;    
                }
                break;                                    

        }
        
        return false;
    },
    
    HandleKeyPress: function(e) {
        var tree = this.control;
        var key = tree.GetFocusedNodeKey();
        
        var busy = tree.syncLock;
        
        switch(ASPx.Evt.GetKeyCode(e)) {
        
            case ASPx.Key.Space:            
                if(!busy) {
                    var state = tree.IsNodeSelected(key);
                    if(state != null) {
                        tree.SelectNode(key, !state);
                        if(tree.RaiseSelectionChanged())
			                tree.SendDummyCommand(true);
                    }                        
                }
                return true;
                
             case 43:      
                if(!busy)
                    this.TryExpand(tree.GetRowByNodeKey(key));
                return true;
                
             case 45:                
                if(!busy)
                    this.TryCollapse(tree.GetRowByNodeKey(key));
                return true;                
                
        }
            
        return false;
    },
    
    TryMoveFocusDown: function(row, select) {
        var tree = this.control;
        var nextRow = this.GetSiblingRow(row, 1);
        if(nextRow) {
            var processOnServer = tree.OnFocusingNode(tree.GetNodeKeyByRow(nextRow), {});
            if(select && !tree.recursiveSelection) {
                this.TrySelectNodes(row, nextRow);
            } else {
                tree.selectionStartKey = null;
            }
            if(processOnServer)
	            tree.SendDummyCommand(true);
            else
                this.EnsureFocusedNodeVisible();
        } else if(tree.pageIndex > -1 && tree.pageIndex < tree.pageCount - 1) {
            tree.NextPage(true);
        }
    },
    
    TryMoveFocusUp: function(row, select) {
        var tree = this.control;    
        var prevRow = this.GetSiblingRow(row, -1);
        if(prevRow) {
            var processOnServer = tree.OnFocusingNode(tree.GetNodeKeyByRow(prevRow), {});
            if(select && !tree.recursiveSelection) {
                this.TrySelectNodes(row, prevRow);
            } else {
                tree.selectionStartKey = null;
            }
            if(processOnServer)
                tree.SendDummyCommand(true);
            else
                this.EnsureFocusedNodeVisible();
        } else if(tree.pageIndex > 0) {
            tree.PrevPage(true);
        }    
    },
    
    TryExpand: function(row, e) {
        var tree = this.control;
        var nodeKey = tree.GetNodeKeyByRow(row);
        if(tree.GetNodeState(null, row) == "Collapsed" && tree.RaiseNodeExpanding(nodeKey, ASPx.Evt.GetEvent(e))){
            tree.ExpandNode(nodeKey);
            return true;
        }
        return false;
    },
    
    TryCollapse: function(row, e) {
        var tree = this.control;
        var nodeKey = tree.GetNodeKeyByRow(row);
        if(tree.GetNodeState(null, row) == "Expanded" && tree.RaiseNodeCollapsing(nodeKey, ASPx.Evt.GetEvent(e))) {
            tree.CollapseNode(nodeKey);
            return true;
        }
        return false;
    },
    
    GetSiblingRow: function(row, offset) {
        var i = 0;
        if(row == null) {
            var dataRows = this.control.GetAllDataRows();
            if(dataRows == null)
                return;
            return dataRows[offset >= 0 ? 0 : dataRows.length - 1];
        }
        while(i < Math.abs(offset)) {
            row = offset < 0 ? row.previousSibling : row.nextSibling;
            if(!row)
                return null;
            if(row.id == this.control.name + "_ArmRow")
                return null;
            if(row.nodeType != 1 || !this.control.IsElementDataRow(row))
                continue;
            i++;
        }
        return row;
    },
    
    TrySelectNodes: function(startRow, endRow) {
        var tree = this.control;
        if(tree.selectionStartKey != null)
            startRow = tree.GetRowByNodeKey(tree.selectionStartKey) || startRow;        
        tree.selectionStartKey = tree.GetNodeKeyByRow(startRow);

        if(!tree.FindSelectionCheck(tree.GetDataTable()))
            return ;

        var rows = tree.GetAllDataRows();
        var inside = false;
        var changed = false;
        for(var i = 0; i < rows.length; i++) {
            var hit = rows[i] == startRow || rows[i] == endRow;            
            if(hit && !inside) {
                inside = true;
                hit = false;
            }
            var key = tree.GetNodeKeyByRow(rows[i]);
            if(tree.IsNodeSelected(key) != inside)
                changed = true;
            tree.SelectNode(key, inside);
            if(inside && (hit || startRow == endRow))
                inside = false;
        }
        
        if(changed) {
            if(tree.RaiseSelectionChanged())
                tree.SendDummyCommand(true);
        }
    },

    EnsureFocusedNodeVisible: function() {
        var tree = this.control;
        if(!tree.HasVertScroll()) return;
        var row = tree.GetRowByNodeKey(tree.GetFocusedNodeKey());
        tree.GetScrollHelper().MakeRowVisible(row, true);
    }
    
});

// API classes
var ASPxClientTreeListColumn = ASPx.CreateClass(null, {	
	constructor: function(index, name, fieldName) {
		this.index = index;
		this.name = name;
		this.fieldName = fieldName;
						
		this.canSort = false;	// internal
		this.showInCw = true;   // internal
        this.minWidth = 0;      // internal
	},
	__dxColumnObject: true
});
var ASPxClientTreeListCustomDataCallbackEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
	constructor: function(arg, result) {
	    this.constructor.prototype.constructor.call(this);
        this.arg = arg;
        this.result = result;
    }
});
var ASPxClientTreeListNodeEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
	constructor: function(nodeKey, htmlEvent) {
		this.constructor.prototype.constructor.call(this);
		this.nodeKey = nodeKey;
		this.htmlEvent = htmlEvent;
		this.cancel = false;		
	}
});
var ASPxClientTreeListContextMenuEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
	constructor: function(objectType, objectKey, htmlEvent) {
		this.constructor.prototype.constructor.call(this);
		this.objectType = objectType;
		this.objectKey = objectKey;
		this.htmlEvent = htmlEvent;
		this.cancel = false;			
	}
});
var ASPxClientTreeListStartDragNodeEventArgs = ASPx.CreateClass(ASPxClientTreeListNodeEventArgs, {
    constructor: function(nodeKey, htmlEvent, targets) {
        this.constructor.prototype.constructor.call(this, nodeKey, htmlEvent);
        this.targets = targets;        
    }
});
var ASPxClientTreeListEndDragNodeEventArgs = ASPx.CreateClass(ASPxClientTreeListNodeEventArgs, {
    constructor: function(nodeKey, htmlEvent, targetElement) {
        this.constructor.prototype.constructor.call(this, nodeKey, htmlEvent);
        this.targetElement = targetElement;
    }    
});
var ASPxClientTreeListCustomButtonEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
	constructor: function(nodeKey, buttonIndex, buttonID) {
		this.constructor.prototype.constructor.call(this);
		this.nodeKey = nodeKey;
		this.buttonIndex = buttonIndex;
		this.buttonID = buttonID;
	}	
});
var ASPxClientTreeListColumnResizingEventArgs = ASPx.CreateClass(ASPxClientCancelEventArgs, {
	constructor: function(column) {
		this.constructor.prototype.constructor.call(this);
		this.column = column;
	}
});
var ASPxClientTreeListColumnResizedEventArgs = ASPx.CreateClass(ASPxClientProcessingModeEventArgs, {
	constructor: function(column){
	    this.constructor.prototype.constructor.call(this, false);
        this.column = column;
    }
});

ASPx.TLPager = function(name, arg) {
	var treeList = ASPx.GetControlCollection().Get(name);
	if(treeList != null)
		treeList.SendPagerCommand(arg);
}

ASPx.TLClick = function(name, e) {
	var treeList = ASPx.GetControlCollection().Get(name);
	if(treeList)
		treeList.OnDataTableClick(e);
}

ASPx.TLDblClick = function(name, e) {
	var treeList = ASPx.GetControlCollection().Get(name);
	if(treeList)
		treeList.OnDataTableDblClick(e);
}

ASPx.TLHeaderDown = function(name, element, e) {
	var treeList = ASPx.GetControlCollection().Get(name);
	if(treeList)
		treeList.OnHeaderMouseDown(element, e);
}

ASPx.TLCWCloseUp = function(name) {
	var treeList = ASPx.GetControlCollection().Get(name);
	if(treeList)
		treeList.RaiseCustomizationWindowCloseUp();
}

ASPx.TLMouseDown = function(name, e) {
	var treeList = ASPx.GetControlCollection().Get(name);
	if(treeList)
		treeList.OnDataTableMouseDown(e);
}

ASPx.TLStartEdit = function(name, key) {
	var treeList = ASPx.GetControlCollection().Get(name);
	if(treeList)
		treeList.StartEdit(key);	
}
ASPx.TLStartEditNewNode = function(name, parentKey) {
	var treeList = ASPx.GetControlCollection().Get(name);
	if(treeList)
		treeList.StartEditNewNode(parentKey);	
}
ASPx.TLDeleteNode = function(name, key) {
	var treeList = ASPx.GetControlCollection().Get(name);
	if(treeList)
		treeList.OnNodeDeleting(key);	
}
ASPx.TLUpdateEdit = function(name) {
	var treeList = ASPx.GetControlCollection().Get(name);
	if(treeList)
		treeList.UpdateEdit();	
}
ASPx.TLCancelEdit = function(name) {
	var treeList = ASPx.GetControlCollection().Get(name);
	if(treeList)
		treeList.CancelEdit();	
}
ASPx.TLCustomButton = function(name, nodeKey, index, id) {
	var treeList = ASPx.GetControlCollection().Get(name);
	if(treeList)
		treeList.RaiseCustomButtonClick(nodeKey, index, id);
}

ASPx.TLMenu = function(name, objectType, objectKey, htmlEvent) {
	var treeList = ASPx.GetControlCollection().Get(name);
	if(treeList)
		return treeList.OnContextMenu(objectType, objectKey, htmlEvent);
	return true;
}

ASPx.TLSort = function(name, columnIndex) {
	var treeList = ASPx.GetControlCollection().Get(name);
	if(treeList)
		treeList.SortBy(columnIndex);
}

ASPx.TLExpand = function(name, nodeKey) {
	var treeList = ASPx.GetControlCollection().Get(name);
	if(treeList) {
		if(treeList.enableFocusedNode && treeList.focusOnExpandCollapse)
			treeList.OnFocusingNode(nodeKey, null);
		treeList.ExpandNode(nodeKey);
	}
}

ASPx.TLCollapse = function(name, nodeKey) {
	var treeList = ASPx.GetControlCollection().Get(name);
	if(treeList) {
		if(treeList.enableFocusedNode && treeList.focusOnExpandCollapse)
			treeList.OnFocusingNode(nodeKey, null);	
		treeList.CollapseNode(nodeKey);
	}
}

window.ASPxClientTreeList = ASPxClientTreeList;
window.ASPxClientTreeListColumn = ASPxClientTreeListColumn;
window.ASPxClientTreeListCustomDataCallbackEventArgs = ASPxClientTreeListCustomDataCallbackEventArgs;
window.ASPxClientTreeListNodeEventArgs = ASPxClientTreeListNodeEventArgs;
window.ASPxClientTreeListContextMenuEventArgs = ASPxClientTreeListContextMenuEventArgs;
window.ASPxClientTreeListStartDragNodeEventArgs = ASPxClientTreeListStartDragNodeEventArgs;
window.ASPxClientTreeListEndDragNodeEventArgs = ASPxClientTreeListEndDragNodeEventArgs;
window.ASPxClientTreeListCustomButtonEventArgs = ASPxClientTreeListCustomButtonEventArgs;
window.ASPxClientTreeListColumnResizingEventArgs = ASPxClientTreeListColumnResizingEventArgs;
window.ASPxClientTreeListColumnResizedEventArgs = ASPxClientTreeListColumnResizedEventArgs;
})();