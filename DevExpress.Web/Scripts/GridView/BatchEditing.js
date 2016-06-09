/// <reference path="..\_references.js"/>

(function() {
var GridBatchEditFocusHelper = ASPx.CreateClass(null, {
    constructor: function(batchEditHelper) {
        this.batchEditHelper = batchEditHelper;
        this.grid = this.batchEditHelper.grid;
        this.activeElementsHash = { };
        this.editTemplateActiveElementsHash = { };

        this.lockEdit = false;
        this.forceRefocusEditor = false;
        this.focusedEditorColumnIndex = -1;
        this.focusedColumnIndex = -1;
    },
    GetDataItemVisibleIndices: function(doNotIncludeDeleted) { return this.batchEditHelper.GetDataItemVisibleIndices(doNotIncludeDeleted); },
    GetDataItemElement: function(visibleIndex) { return this.batchEditHelper.GetDataItem(visibleIndex); },
    CanEditColumn: function(columnIndex) { return this.batchEditHelper.CanEditColumn(columnIndex); },
    IsEditing: function() { return this.batchEditHelper.IsEditing(); },
    GetTemplateColumnIndices: function() { return this.batchEditHelper.GetTemplateColumnIndices(); },
    IsSingleEditMode: function() { return this.batchEditHelper.IsSingleEditMode(); },
    IsEditingDataItem: function(visibleIndex) { return this.batchEditHelper.IsEditingDataItem(visibleIndex); },
    IsColumnEditing: function(columnIndex) { return this.batchEditHelper.IsColumnEditing(columnIndex); },
    GetEditor: function(columnIndex) { return this.batchEditHelper.GetEditor(columnIndex); },
    IsEditTemplateColumn: function(columnIndex) { return this.batchEditHelper.IsEditTemplateColumn(columnIndex); },
    IsNewItemOnTop: function() { return this.batchEditHelper.IsNewItemOnTop(); },
    GetVisibleColumnIndices: function() { return this.batchEditHelper.GetVisibleColumnIndices(); },
    GetEditColumnIndices: function() { return this.batchEditHelper.GetEditColumnIndices(); },
    GetEditItemVisibleIndex: function() { return this.batchEditHelper.editItemVisibleIndex; },
    GetEditingDataItemElement: function() { return this.GetDataItemElement(this.GetEditItemVisibleIndex()); },
    GetDataCell: function(dataItem, columnIndex) { return this.batchEditHelper.GetDataCell(dataItem, columnIndex); },
    GetColumnIndexByChild: function(dataItem, element) { return this.batchEditHelper.GetColumnIndexByChild(dataItem, element); },

    StartEdit: function(visibleIndex, columnIndex) { this.batchEditHelper.StartEdit(visibleIndex, columnIndex); },
    EndEdit: function() { return this.batchEditHelper.EndEdit(); },

    Update: function() {
        this.AttachEvents();
        this.LoadActiveElements();
        this.lockEdit = false;
    },
    
    CanStartEdit: function() { return !this.lockEdit; },
    OnDataItemAdded: function(visibleIndex) {
        this.LoadDataItemActiveElements(visibleIndex);
    },
    OnEditStarted: function(visibleIndex, columnIndices, focusedColumn) {
        if(focusedColumn)
            this.FocusColumnEditor(focusedColumn.index);

        this.lockEndEditOnLostFocus = true;
        window.setTimeout(function() { this.lockEndEditOnLostFocus = false; }.aspxBind(this), 0);
    },
    OnEditEnded: function() {
        this.BlurFocusedEditor();
    },

    LoadActiveElements: function() {
        var visibleIndices = this.GetDataItemVisibleIndices();
        for(var i = 0; i < visibleIndices.length; i++)
            this.LoadDataItemActiveElements(visibleIndices[i]);
        var columnIndices = this.GetTemplateColumnIndices();
        for(var i = 0; i < columnIndices.length; i++)
            this.LoadEditTemplateActiveElements(columnIndices[i]);
    },
    LoadDataItemActiveElements: function(visibleIndex) {
        var elementsInfo = this.GetDataItemActiveElementsInfo(visibleIndex);
        var dataItemInfo = { };
        for(var i = 0; i < elementsInfo.length; i++) {
            var element = elementsInfo[i].element;
            var columnIndex = elementsInfo[i].columnIndex;

            var cellActiveElements = dataItemInfo[columnIndex];
            if(!cellActiveElements)
                dataItemInfo[columnIndex] = cellActiveElements = [ ];
            cellActiveElements.push(element);

            element.dxgvColumnIndex = columnIndex;
            element.dxgvVisibleIndex = visibleIndex;
            ASPx.Evt.AttachEventToElement(element, "keydown", function(e) { this.OnActiveElementKeyDown(e); }.aspxBind(this));
        }
        this.SortDataItemActiveElements(dataItemInfo);
        this.activeElementsHash[visibleIndex] = dataItemInfo;
    },
    LoadEditTemplateActiveElements: function(columnIndex) {
        var elements = [ ];
        var editorContainer = this.batchEditHelper.GetEditorContainer(columnIndex);//TODO
        var inputs = ASPx.GetNodesByTagName(editorContainer, "INPUT");
        for(var i = 0; i < inputs.length; i++) {
            var input = inputs[i];
            if(input.type !== "hidden")
                elements.push(input);
        }
        var links = ASPx.GetNodesByTagName(editorContainer, "A");
        for(var i = 0; i < links.length; i++)
            elements.push(links[i]);
        this.SortEditTemplateActiveElements(elements);
        this.editTemplateActiveElementsHash[columnIndex] = elements;
    },
    SortDataItemActiveElements: function(rowInfo) { },
    SortEditTemplateActiveElements: function(elements) { },

    GetDataItemActiveElementsInfo: function(visibleIndex) {
        var dataItem = this.GetDataItemElement(visibleIndex);
        if(!dataItem) 
            return [ ];
        var inputs = this.GetDataItemActiveInputs(dataItem);
        var links = this.GetDataItemActiveLinks(dataItem);
        return inputs.concat(links);
    },
    GetDataItemActiveInputs: function(dataItem) {
        var result = [ ];
        var inputs = ASPx.GetNodesByTagName(dataItem, "INPUT");
        for(var i = 0; i < inputs.length; i++) {
            var input = inputs[i];
            var columnIndex = this.GetColumnIndexByChild(dataItem, input);
            if(input.type !== "hidden" && columnIndex >= 0 && !this.CanEditColumn(columnIndex))
                result.push({ element: input, columnIndex: columnIndex });
        }
        return result;
    },
    GetDataItemActiveLinks: function(dataItem) {
        var result = [ ];
        var links = ASPx.GetNodesByTagName(dataItem, "A");
        for(var i = 0; i < links.length; i++) {
            var link = links[i];
            var columnIndex = this.GetColumnIndexByChild(dataItem, link);
            if(columnIndex >= 0 && !this.CanEditColumn(columnIndex))
                result.push({ element: link, columnIndex: columnIndex });
        }
        return result;
    },

    OnActiveElementKeyDown: function(e) {
        var keyCode = ASPx.Evt.GetKeyCode(e);
        if(keyCode !== ASPx.Key.Tab)
            return;
        var src = ASPx.Evt.GetEventSource(e);
        if(this.MoveFocusFromActiveElement(src, e.shiftKey))
            ASPx.Evt.PreventEventAndBubble(e);
    },

    MoveFocusFromActiveElement: function(activeElement, isLeft) {
        var visibleIndex = activeElement.dxgvVisibleIndex;
        var columnIndex = activeElement.dxgvColumnIndex;
        var dataItemInfo = this.activeElementsHash[visibleIndex];
        if(!dataItemInfo) return;
        var cellActiveElements = dataItemInfo[columnIndex];
        if(!cellActiveElements) return;
        if(cellActiveElements.length > 1) {
            var elementIndex = ASPx.Data.ArrayIndexOf(cellActiveElements, activeElement);
            var inc = isLeft ? -1 : 1;
            var focusElementIndex = elementIndex + inc;
            if(focusElementIndex > -1 && focusElementIndex < cellActiveElements.length)
                return;
        }
        return this.MoveFocus(visibleIndex, columnIndex, isLeft);
    },
    MoveFocusFromEditor: function(editor, isLeft) {
        return this.MoveFocus(this.GetEditItemVisibleIndex(), editor.dxgvColumnIndex, isLeft, true);
    },

    MoveFocusNext: function() {
        if(!this.IsEditing()) return;
        return this.MoveFocus(this.GetEditItemVisibleIndex(), this.GetFocusedColumnIndex())
    },
    MoveFocusPrev: function() {
        if(!this.IsEditing()) return;
        return this.MoveFocus(this.GetEditItemVisibleIndex(), this.GetFocusedColumnIndex(), true);
    },
    
    MoveFocus: function(visibleIndex, columnIndex, isLeft, fromEditor) {
        var endEdit = function() { window.setTimeout(function() { this.EndEdit(); }.aspxBind(this), 0); }.aspxBind(this);
        var cellInfo = this.GetNewFocusedCellInfo(visibleIndex, columnIndex, isLeft);
        if(!cellInfo || cellInfo.addNewRow) {
            endEdit();
            return false;
        }
        var newVisibleIndex = cellInfo.visibleIndex;
        var newColumnIndex = cellInfo.columnIndex;
        this.focusedColumnIndex = newColumnIndex;

        if(this.CanFocusEditColumn(newColumnIndex)) {
            this.MoveFocusToEditCell(newVisibleIndex, newColumnIndex, visibleIndex, columnIndex, isLeft, fromEditor);
            return true;
        } 
        var isRowEditMode = !this.IsSingleEditMode();
        this.lockEndEditOnLostFocus = isRowEditMode && this.IsEditingDataItem(newVisibleIndex) && fromEditor;

        var activeElementFocused = this.TryFocusCellActiveElement(newVisibleIndex, newColumnIndex, isLeft);
        if(isRowEditMode && !this.IsEditingDataItem(newVisibleIndex) && !fromEditor)
            endEdit();
        return activeElementFocused;
    },
    MoveFocusToEditCell: function(visibleIndex, columnIndex, prevVisibleIndex, prevColumnIndex, isLeft, fromEditor) {
        if(this.IsSingleEditMode() || !this.IsEditing() || visibleIndex !== prevVisibleIndex)
            window.setTimeout(function() { 
                this.MoveFocusByStartEditing(visibleIndex, columnIndex, isLeft, fromEditor); 
            }.aspxBind(this), 0);
        else
            this.MoveFocusToSiblingEditCell(visibleIndex, columnIndex, isLeft);

        this.TryShowColumn(columnIndex, true);
    },
    MoveFocusByStartEditing: function(visibleIndex, columnIndex, isLeft, fromEditor) {
        this.StartEdit(visibleIndex, columnIndex); 
        if(!this.IsEditing())
            this.MoveFocus(visibleIndex, columnIndex, isLeft, fromEditor);
    },
    TryFocusCellActiveElement: function(visibleIndex, columnIndex, isLeft) {
        var dataItemInfo = this.activeElementsHash[visibleIndex];
        if(!dataItemInfo || !dataItemInfo[columnIndex]) 
            return false;
        var activeElements = dataItemInfo[columnIndex];
        var index = isLeft ? activeElements.length - 1 : 0;
        var element = activeElements[index];
        ASPx.SetFocus(element);
        return true;
    },
    CanFocusEditColumn: function(columnIndex) {
        if(!this.IsSingleEditMode() && this.IsEditing())
            return this.IsColumnEditing(columnIndex);// TODO
        return this.CanEditColumn(columnIndex);
    },
    MoveFocusToSiblingEditCell: function(visibleIndex, newColumnIndex, isLeft) { // only row edit mode
        var prevFocusedColumnIndex = this.focusedEditorColumnIndex;
        if(this.CanEditColumn(prevFocusedColumnIndex))
            this.lockEndEditOnLostFocus = true;
        window.setTimeout(function() {
            this.BlurEditor(prevFocusedColumnIndex);
            this.FocusColumnEditor(newColumnIndex, isLeft);
        }.aspxBind(this), 0);
    },
    FocusColumnEditor: function(columnIndex, isLeft) {
        if(this.IsEditTemplateColumn(columnIndex))
            this.FocusEditTemplateEditor(columnIndex, isLeft);
        else
            this.FocusDefaultEditor(columnIndex);
        this.focusedColumnIndex = columnIndex;
    },
    FocusDefaultEditor: function(columnIndex) {
        var editor = this.GetEditor(columnIndex);
        if(!editor) return;
        editor.Focus();
    },
    FocusEditTemplateEditor: function(columnIndex, isLeft) {
        var processed = this.grid.RaiseBatchEditTemplateCellFocused(columnIndex);
        if(processed) return;
        var activeElements = this.editTemplateActiveElementsHash[columnIndex];
        if(!activeElements || activeElements.length === 0)
            return;
        var element = !isLeft ? activeElements[0] : activeElements[activeElements.length - 1];
        ASPx.SetFocus(element);
    },
    BlurEditor: function(columnIndex) {
        var editor = this.GetEditor(columnIndex);
        if(!editor || editor !== ASPxClientEdit.GetFocusedEditor()) 
            return;
        var input = editor.GetInputElement();
        if(input)
            input.blur();
    },
    BlurFocusedEditor: function() {
        this.BlurEditor(this.focusedEditorColumnIndex);
    },

    GetNewFocusedCellInfo: function(visibleIndex, columnIndex, isLeft) {
        var inc = isLeft ? -1 : 1;
        columnIndex = this.GetNewFocusedColumnIndex(visibleIndex, columnIndex, inc);
        if(columnIndex >= 0)
            return { visibleIndex: visibleIndex, columnIndex: columnIndex };
        var addNewRow = false;
        var visibleIndices = this.GetDataItemVisibleIndices(true);
        var index = ASPx.Data.ArrayIndexOf(visibleIndices, visibleIndex) + inc;
        if(index < 0 && this.IsNewItemOnTop() || index == visibleIndices.length && !this.IsNewItemOnTop()) {
            addNewRow = true;
            columnIndex = -1;
            visibleIndex = null;
        } else if(index >= 0 && index < visibleIndices.length) {
            visibleIndex = visibleIndices[index];
        } else
            return null;
        columnIndex = this.GetNewFocusedColumnIndex(visibleIndex, columnIndex, inc);
        return { visibleIndex: visibleIndex, columnIndex: columnIndex, addNewRow: addNewRow };
    },

    GetNewFocusedColumnIndex: function(visibleIndex, columnIndex, inc) {
        var dataItemInfo = this.activeElementsHash[visibleIndex];
        var columnIndices = this.GetVisibleColumnIndices();

        var i = ASPx.Data.ArrayIndexOf(columnIndices, columnIndex);
        if(i < 0) 
            i = inc < 0 ? columnIndices.length : - 1; 
        do {
            i = i + inc;
            columnIndex = columnIndices[i];
            if(this.CanFocusEditColumn(columnIndex) || dataItemInfo && dataItemInfo[columnIndex])
                return columnIndex;
        } while(i >= 0 && i < columnIndices.length)
        return -1;
    },

    AttachEvents: function() {
        var grid = this.grid;
        var columnIndices = this.GetEditColumnIndices();
        for(var i = 0; i < columnIndices.length; i++) {
            var columnIndex = columnIndices[i];
            grid.AttachEventToEditor(columnIndex, "GotFocus", function(s, e) { this.OnEditorGotFocus(s, e); }.aspxBind(this));
            grid.AttachEventToEditor(columnIndex, "LostFocus", function(s, e) { this.OnEditorLostFocus(s, e); }.aspxBind(this));
            grid.AttachEventToEditor(columnIndex, "KeyDown", function(s, e) { this.OnEditorKeyDown(s, e); }.aspxBind(this));
            grid.AttachEventToEditor(columnIndex, "ValueChanged", function(s, e) { this.OnEditorValueChanged(s, e); }.aspxBind(this));
        }
        ASPx.Evt.AttachEventToElement(grid.GetMainTable(), "mousedown", function(e) { this.OnMainTableMouseDown(e); }.aspxBind(this));
    },

    OnEditorGotFocus: function(s, e) {
        s.dxgvSkipValueChanged = false;
        this.lockEdit = false;
        this.focusedEditorColumnIndex = s.dxgvColumnIndex;
        this.TryShowColumn(s.dxgvColumnIndex);
    },

    OnEditorLostFocus: function(s, e) {
        if(this.forceRefocusEditor || !this.lockEndEditOnLostFocus && !this.EndEdit()) {
            this.RefocusEditor(s);
            return;
        }
        this.lockEndEditOnLostFocus = false;
        this.focusedEditorColumnIndex = -1;
    },

    RefocusEditor: function(editor) {
        this.lockEdit = true;
        ASPx.Timer.ClearTimer(this.removeLockEditTimerID);
        this.removeLockEditTimerID = window.setTimeout(function() { this.lockEdit = false; }.aspxBind(this), 50);
        this.forceRefocusEditor = false;
        editor.Focus();
    },

    OnEditorValueChanged: function(s, e) {
        if(!s.dxgvSkipValueChanged) 
            this.batchEditHelper.OnEditorValueChanged(s, s.dxgvColumnIndex);
    },

    OnEditorKeyDown: function(s, e) {
        var keyCode = ASPx.Evt.GetKeyCode(e.htmlEvent);
        if(keyCode === ASPx.Key.Esc) {
            s.dxgvSkipValueChanged = true;
            // TODO reset changes in this editor
            this.lockEndEditOnLostFocus = false;
            this.BlurEditor(s.dxgvColumnIndex);
            return;
        }

        if(keyCode !== ASPx.Key.Tab && keyCode !== ASPx.Key.Enter)
            return;
        if(keyCode === ASPx.Key.Enter && this.batchEditHelper.IsMemoEdit(s))// TODO
		    return;
        if(this.MoveFocusFromEditor(s, e.htmlEvent.shiftKey))
            ASPx.Evt.PreventEventAndBubble(e.htmlEvent);
    },
    
    OnMainTableMouseDown: function(e) { // TODO refactor
        this.forceRefocusEditor = false;
        this.lockEndEditOnLostFocus = false;
        this.lockUserEndEdit = false;
        var src = ASPx.Evt.GetEventSource(e);
        var dataItem = this.GetEditingDataItemElement();
        if(!dataItem || !this.IsEditing()) 
            return;
        if(!this.IsSingleEditMode()) {
            if(!ASPx.GetIsParent(dataItem, src))
                return;
            this.lockEndEditOnLostFocus = true;

            var sourceColumnIndex = this.GetFocusedColumnIndex();
            var targetColumnIndex = this.GetColumnIndexByChild(dataItem, src);
            if(this.CanEditColumn(sourceColumnIndex) && this.CanEditColumn(targetColumnIndex) && sourceColumnIndex !== targetColumnIndex)
                this.lockUserEndEdit = true;
            this.focusedColumnIndex = targetColumnIndex;
        }

        if(this.focusedEditorColumnIndex < 0) {
            this.lockEndEditOnLostFocus = false;
            return;
        }
        var dataCell = this.batchEditHelper.GetActualDataCell(this.GetEditItemVisibleIndex(), this.focusedEditorColumnIndex);//TODO
        var editor = this.GetEditor(this.focusedEditorColumnIndex);
        if(!editor || !dataCell) return;

        if(!ASPx.GetIsParent(dataCell, src))
            return;

        var editorMainElement = ASPx.IsFunction(editor.GetICBMainElement) ? editor.GetICBMainElement() : editor.GetMainElement(); // Internal CheckBox
        var editorElements = [ editorMainElement ];
        if(ASPx.IsFunction(editor.GetPopupControl)) // DropDownEditBase
            editorElements.push(editor.GetPopupControl().GetWindowElement(-1));
        for(var i = 0; i < editorElements.length; i++) {
            if(ASPx.GetIsParent(editorElements[i], src)) {
                this.lockEndEditOnLostFocus = false;
                return;
            }
        }

        this.forceRefocusEditor = true;
    },

    GetFocusedColumnIndex: function() {
        if(this.focusedEditorColumnIndex >= 0)
            return this.focusedEditorColumnIndex;
        return this.focusedColumnIndex;
    },
    
    TryShowColumn: function(columnIndex, showFullRect) {
        var fixedColumnHelper = this.grid.GetFixedColumnsHelper && this.grid.GetFixedColumnsHelper(); // TODO
        if(fixedColumnHelper)
            fixedColumnHelper.TryShowColumn(columnIndex, showFullRect);
    },

});


var GridBatchEditHelper = ASPx.CreateClass(null, {
    EditType_Cell: 0,
    EditType_Row: 1,

    StartEditAction_Click: 0,
    StartEditAction_DblClick: 1,

    ValidationMode_OnEndEdit: 0,
    ValidationMode_OnSaveChanges: 1,

    InvalidItemVisibleIndex: -100000,
    NewItemInitValuesKey: "NIV",
    ColorPlaceholder: "||dxcolor||",

    constructor: function(grid) {
        this.grid = grid;
        this.pageServerValues = { };

        this.editItemVisibleIndex = this.InvalidItemVisibleIndex;
        this.editedColumnIndices = [ ];
        this.invalidItemIndices = [ ];
        
        this.insertedItemIndices = [ ];
        this.insertedValues = { };
        this.updatedValues = { };
        this.deletedItemKeys = [ ];
        this.serverCellTexts = { };

        this.focusHelper = new GridBatchEditFocusHelper(this);
        this.updateWatcherHelper = new GridViewBatchEditUpdateWatcherHelper(this);

        this.updating = false;
    },

    IsSingleEditMode: function() { return this.grid.batchEditClientState.editMode === this.EditType_Cell; },
    IsStartEditOnDblClick: function() { return this.grid.batchEditClientState.startEditAction === this.StartEditAction_DblClick; },
    IsNewItemOnTop: function() { return !!this.grid.batchEditClientState.isNewRowOnTop; },
    RequireValidateOnEndEdit: function() { return !!this.grid.batchEditClientState.validateOnEndEdit; },
    AllowEndEditOnError: function() { return !!this.grid.batchEditClientState.allowEndEditOnError; },
    GetEditColumnIndices: function() { return this.grid.batchEditClientState.editColumnIndices; },
    GetUpdateInfo: function() { return this.grid.batchEditClientState.updateInfo; },
    GetValidationInfo: function() { return this.grid.batchEditClientState.validationInfo; },
    GetTemplateColumnIndices: function() { return this.grid.batchEditClientState.templateColumnIndices },
    GetBinaryImageColumnsDisplayHtml: function() { return this.grid.batchEditClientState.binaryImageColumnsDisplayHtml; },
    GetCheckColumnsDisplayHtml: function() { return this.grid.batchEditClientState.checkColumnsDisplayHtml; },
    GetColorEditColumnsDisplayHtml: function() { return this.grid.batchEditClientState.colorColumnsDisplayHtml; },

    IsBinaryImageColumn: function(columnIndex) { return !!this.GetBinaryImageColumnsDisplayHtml()[columnIndex]; },
    IsCheckColumn: function(columnIndex) { return !!this.GetCheckColumnsDisplayHtml()[columnIndex]; },
    IsColorEditColumn: function(columnIndex) { return !!this.GetColorEditColumnsDisplayHtml()[columnIndex]; },
    IsEditTemplateColumn: function(columnIndex) { return ASPx.Data.ArrayIndexOf(this.GetTemplateColumnIndices(), columnIndex) >= 0; },

    IsMemoEdit: function(editor) { return ASPx.Ident.IsASPxClientMemo && ASPx.Ident.IsASPxClientMemo(editor); },

    IsNewItem: function(key) { return ASPx.Data.ArrayIndexOf(this.insertedItemIndices, key) >= 0; },
    IsDeletedItem: function(key) { return ASPx.Data.ArrayIndexOf(this.deletedItemKeys, key) >= 0; },

    IsEditing: function() { return this.editItemVisibleIndex !== this.InvalidItemVisibleIndex && this.editedColumnIndices.length > 0; },
    IsColumnEditing: function(columnIndex) { return this.IsEditingCell(this.editItemVisibleIndex, columnIndex); },
    IsEditingDataItem: function(visibleIndex) { return this.IsEditing() && this.editItemVisibleIndex === visibleIndex; },
    IsEditingCell: function(visibleIndex, columnIndex) { return this.IsEditingDataItem(visibleIndex) && ASPx.Data.ArrayIndexOf(this.editedColumnIndices, columnIndex) >= 0; },
    
    IsUpdating: function() { return this.updating; },
    CanEditColumn: function(columnIndex) { return ASPx.Data.ArrayIndexOf(this.GetEditColumnIndices(), columnIndex) >= 0; },
    GetEditorsContainer: function() { return this.grid.GetBatchEditorsContainer(); },
    GetEditorContainer: function(columnIndex) { return this.grid.GetBatchEditorContainer(columnIndex); },
    GetEditor: function(columnIndex) { return this.grid.GetEditor(columnIndex); },
    GetDataItemIndex: function(item) { return this.grid.tryGetNumberFromEndOfString(item.id).value; },
    GetCellErrorTable: function() { return this.grid.GetBatchEditCellErrorTable(); },
    GetVisibleColumnIndices: function() { return this.grid.GetVisibleColumnIndices(); },
    GetItemErrorID: function(visibleIndex) { return this.grid.name + "_" + this.grid.EditingErrorItemID + visibleIndex; },
    GetItemError: function(visibleIndex) { return ASPx.GetElementById(this.GetItemErrorID(visibleIndex)); },
    GetCellErrorTableID: function(visibleIndex, columnIndex) { return this.grid.BatchEditCellErrorTableID + visibleIndex + "_" + columnIndex; },
    GetEtalonNewItem: function() { return this.grid.GetChildElement(this.grid.GetDataItemIDPrefix() + "new"); },

    GetColumn: function(columnIndex) { return this.grid._getColumn(columnIndex); },
    IsHiddenColumn: function(columnIndex) { return ASPx.Data.ArrayIndexOf(this.GetHiddenColumns(), columnIndex) >= 0; },
    GetHiddenColumns: function() {
        var helper = this.grid.GetFixedColumnsHelper && this.grid.GetFixedColumnsHelper(); // TODO
        return helper ? helper.GetHiddenColumns() : [ ];
    },
    IsDateEdit: function(editor) { return ASPx.Ident.IsASPxClientDateEdit && ASPx.Ident.IsASPxClientDateEdit(editor);},
    GetObjectSize: function(obj) { return ASPx.GetObjectKeys(obj).length; },
    

    GetClientValuesBag: function(visibleIndex) {
        return visibleIndex >= 0 ? this.updatedValues : this.insertedValues;
    },
    GetItemKey: function(visibleIndex) {
        return visibleIndex >= 0 ? this.grid.GetItemKey(visibleIndex) : visibleIndex;
    },
    IsDirtyCell: function(visibleIndex, columnIndex) {
        var hash = this.GetClientValuesBag(visibleIndex);
        var key = this.GetItemKey(visibleIndex); // for new rows generate unique id and use it when find row
        var rowValues = hash[key];
        return rowValues && rowValues.hasOwnProperty(columnIndex);
    },
    GetCellValue: function(visibleIndex, columnIndex) {
        if(this.IsDirtyCell(visibleIndex, columnIndex))
            return this.GetClientValue(visibleIndex, columnIndex);
        return this.GetServerValue(visibleIndex, columnIndex);
    },
    GetClientValue: function(visibleIndex, columnIndex) {
        var hash = this.GetClientValuesBag(visibleIndex);
        var key = this.GetItemKey(visibleIndex);
        var rowValues = hash[key];
        return rowValues[columnIndex][0];
    },
    GetServerValue: function(visibleIndex, columnIndex) {
        var key = visibleIndex >= 0 ? this.GetItemKey(visibleIndex) : this.NewItemInitValuesKey;
        var rowValues = this.pageServerValues[key];
        if(rowValues && rowValues.hasOwnProperty(columnIndex))
            return rowValues[columnIndex];
        return null;
    },
    SetCellValue: function(visibleIndex, columnIndex, value, text) {
        if(this.IsEquals(this.GetServerValue(visibleIndex, columnIndex), value))
            this.ResetClientValue(visibleIndex, columnIndex);
        else
            this.SetCellValueCore(visibleIndex, columnIndex, value, text);
    },
    SetCellValueCore: function(visibleIndex, columnIndex, value, text) {
        var hash = this.GetClientValuesBag(visibleIndex);
        var key = this.GetItemKey(visibleIndex);
        var rowValues = hash[key];
        if(!rowValues)
            hash[key] = rowValues = { };
        rowValues[columnIndex] = [ value, text ];
        this.EnsureCellServerHtml(visibleIndex, columnIndex);
    },
    GetCellText: function(visibleIndex, columnIndex) {
        if(this.IsDirtyCell(visibleIndex, columnIndex))
            return this.GetClientValueText(visibleIndex, columnIndex);
        return this.GetCellServerText(visibleIndex, columnIndex);
    },
    GetClientValueText: function(visibleIndex, columnIndex) {
        var hash = this.GetClientValuesBag(visibleIndex);
        var key = this.GetItemKey(visibleIndex);
        var rowValues = hash[key];
        return rowValues[columnIndex][1];
    },
    GetCellServerText: function(visibleIndex, columnIndex) {
        var key = this.GetItemKey(visibleIndex);
        var rowTexts = this.serverCellTexts[key];
        if(!rowTexts)
            this.serverCellTexts[key] = rowTexts = { };
        if(!rowTexts.hasOwnProperty(columnIndex))
            rowTexts[columnIndex] = this.GetCellServerHtml(visibleIndex, columnIndex);
        return rowTexts[columnIndex];
    },
    GetCellServerHtml: function(visibleIndex, columnIndex) {
        var textContainer = this.GetCellTextContainer(visibleIndex, columnIndex);
        if(!textContainer) return null;
        if(!ASPx.IsExists(textContainer.dxgvSavedHtml))
            textContainer.dxgvSavedHtml = textContainer.innerHTML;
        return textContainer.dxgvSavedHtml;
    },
    EnsureCellServerHtml: function(visibleIndex, columnIndex) {
        this.GetCellServerHtml(visibleIndex, columnIndex);
    },
    ResetClientValue: function(visibleIndex, columnIndex) {
        this.ResetClientValueByKey(this.GetItemKey(visibleIndex), columnIndex);
    },
    ResetClientValueByKey: function(key, columnIndex) {
        this.ResetClientValueCore(key, columnIndex, this.insertedValues);
        this.ResetClientValueCore(key, columnIndex, this.updatedValues);
    },
    ResetClientValueCore: function(key, columnIndex, hash) {
        var rowValues = hash[key];
        if(rowValues)
            delete rowValues[columnIndex];
        if(this.GetObjectSize(rowValues) === 0)
            delete hash[key];
    },
    GetItemValues: function(visibleIndex) {
        var result = { };
        var columnIndices = this.GetEditColumnIndices();
        for(var i = 0; i < columnIndices.length; i++) {
            var columnIndex = columnIndices[i];
            result[columnIndex] = this.GetCellValueInfo(visibleIndex, columnIndex);
        }
        return result;
    },
    GetCellValueInfo: function(visibleIndex, columnIndex) {
        return { 
            value: this.GetCellValue(visibleIndex, columnIndex), 
            text: this.GetCellText(visibleIndex, columnIndex) 
        };
    },

    HasChanges: function(visibleIndex, columnIndex) {
        if(!ASPx.IsExists(visibleIndex))
            return this.insertedItemIndices.length > 0 || this.deletedItemKeys.length > 0 || this.GetObjectSize(this.updatedValues) > 0;
        if(this.IsNewItem(visibleIndex))
            return true;
        var columnIndices = !ASPx.IsExists(columnIndex) ? this.GetEditColumnIndices() : [ columnIndex ];
        for(var i = 0; i < columnIndices.length; i++) {
            if(this.IsDirtyCell(visibleIndex, columnIndices[i]))
                return true;
        }
        return false;
    },
    

    

    Init: function() {
        this.LoadPageServerValues();
        this.LoadSavedClientValues();
        this.ApplyClientChanges();
        this.ProcessServerChanges(this.GetUpdateInfo());
        this.ProcessValidationErrors(this.GetValidationInfo());

        this.focusHelper.Update();
    },

    OnAfterCallback: function() {
        this.ResetEditState();

        this.LoadPageServerValues();
        
        this.ProcessServerChanges(this.GetUpdateInfo());
        this.ProcessValidationErrors(this.GetValidationInfo());

        this.focusHelper.Update();
        
        this.updating = false;
        this.UpdateSyncInput();
    },

    ShowCallbackError: function(errorText, errorData) {
        if(!errorData) return;

        this.ProcessServerChanges(errorData.updateInfo);
        this.ProcessValidationErrors(errorData.validationInfo);

        this.grid.batchEditClientState.updateInfo = { };

        this.RemoveItemErrors();
        var item = this.GetDataItemByKey(errorData.rowKey);
        return this.ShowItemValidationError(item, errorText);
    },

    LoadPageServerValues: function() {
        this.pageServerValues = this.grid.batchEditPageValues; //TODO
        this.serverCellTexts = { };
    },
    LoadSavedClientValues: function() {
        var savedClientState = this.grid.stateObject.batchEditClientModifiedValues;
        if(!savedClientState) return;

        this.insertedItemIndices = savedClientState.InsertedIndices || [ ];
        this.insertedValues = savedClientState.InsertedValues || { };
        this.updatedValues = savedClientState.UpdatedValues || { };
        this.deletedItemKeys = savedClientState.DeletedKeys || [ ];
    },
    ResetEditState: function() {
        this.editItemVisibleIndex = this.InvalidItemVisibleIndex;
        this.editedColumnIndices = [ ];
        this.invalidItemIndices = [ ];
    },

    ProcessServerChanges: function(updateInfo) {
        if(!updateInfo) return;
        this.ProcessServerInsertedItems(updateInfo.inserted);
        this.ProcessServerUpdatedItems(updateInfo.updated);
        this.ProcessServerDeletedItems(updateInfo.deleted);
        this.ApplyClientChanges();
    },
    ProcessServerInsertedItems: function(visibleIndices) {
        visibleIndices = visibleIndices || [ ];
        for(var i = 0; i < visibleIndices.length; i++)
            this.ResetChanges(visibleIndices[i]);
    },
    ProcessServerUpdatedItems: function(keys) {
        keys = keys || [ ];
        for(var i = 0; i < keys.length; i++)
            this.ResetChangesByKey(keys[i]);
    },
    ProcessServerDeletedItems: function(deletedKeys) {
        deletedKeys = deletedKeys || [ ];
        var keys = this.deletedItemKeys.slice();
        for(var i = 0; i < keys.length; i++) {
            var key = keys[i];
            this.ResetChangesByKey(key);
            if(ASPx.Data.ArrayIndexOf(deletedKeys, key) >= 0)
                this.SafeRemoveHtmlItem(this.FindItemVisibleIndexByKey(key));
        }
    },
    ApplyClientChanges: function() {
        this.ApplyClientInsertChanges();
        this.ApplyClientUpdateChanges();
        this.ApplyClientDeleteChanges();
        this.UpdateSyncInput();
    },
    ApplyClientInsertChanges: function() {
        var editColumnIndices = this.GetEditColumnIndices();
        var indices = this.insertedItemIndices.slice();
        for(var i = 0; i < indices.length; i++) {
            var visibleIndex = indices[i];
            var newItem = this.AddNewItemCore(visibleIndex);
            if(newItem){
                for(var columnIndex = 0; columnIndex < editColumnIndices.length; columnIndex++)
                    this.EnsureCellServerHtml(visibleIndex, editColumnIndices[columnIndex]);
                this.UpdateItem(visibleIndex, editColumnIndices);
            }
        }
    },
    ApplyClientUpdateChanges: function() {
        var editColumnIndices = this.GetEditColumnIndices();
        for(var key in this.updatedValues) { 
            var visibleIndex = this.FindItemVisibleIndexByKey(key);
            for(var i = 0; i < editColumnIndices.length; i++)
                this.EnsureCellServerHtml(visibleIndex, editColumnIndices[i]);
            this.UpdateItem(visibleIndex, editColumnIndices);
        }
    },
    ApplyClientDeleteChanges: function() {
        var keys = this.deletedItemKeys;
        this.deletedItemKeys = [ ];
        for(var i = 0; i < keys.length; i++)
            this.DeleteItemByKey(keys[i]);
    },

    ResetAllChanges: function() {
        var indices = this.insertedItemIndices.slice();
        for(var i = 0; i < indices.length; i++)
            this.ResetChanges(indices[i]);

        var keys = this.deletedItemKeys.slice();
        for(var i = 0; i < keys.length; i++)
            this.ResetChangesByKey(keys[i]);

        keys = ASPx.GetObjectKeys(this.updatedValues);
        for(var i = 0; i < keys.length; i++)
            this.ResetChangesByKey(keys[i]);
    },
    ResetChanges: function(visibleIndex, columnIndex) {
        this.ResetChangesCore(this.GetItemKey(visibleIndex), visibleIndex, columnIndex);
    },
    ResetChangesByKey: function(key, columnIndex) {
        this.ResetChangesCore(key, null, columnIndex);
    },
    ResetChangesCore: function(key, visibleIndex, columnIndex) {
        if(!ASPx.IsExists(visibleIndex))
            visibleIndex = this.FindItemVisibleIndexByKey(key);

        var columnIndices = !ASPx.IsExists(columnIndex) ? this.GetEditColumnIndices() : [ columnIndex ];
        var entire = !ASPx.IsExists(columnIndex);

        for(var i = 0; i < columnIndices.length; i++)
            this.ResetClientValueByKey(key, columnIndices[i]);

        if(entire && this.serverCellTexts.hasOwnProperty(key))
            delete this.serverCellTexts[key];
        
        var isNew = this.IsNewItem(key);
        var isDeleted = !isNew && this.IsDeletedItem(key);
        var isUpdated = !isNew && !isDeleted;

        if(isNew && entire) {
            this.SafeRemoveHtmlItem(visibleIndex);
            ASPx.Data.ArrayRemove(this.insertedItemIndices, key);
        }
        if(isDeleted && entire) {
            ASPx.Data.ArrayRemove(this.deletedItemKeys, key);
            this.ChangeDeletedItemVisibility(visibleIndex, true);
        }
        if(isUpdated)
            this.UpdateItem(visibleIndex, columnIndices, false, true);

        this.EnsureEmptyDataItemVisibility();
        this.RemoveItemError(visibleIndex);

        this.ResetItemValidationState(visibleIndex);

        if(this.grid.CalculateAdaptivity)
            this.grid.CalculateAdaptivity();

        this.UpdateSyncInput();
        
    },
    ResetItemValidationState: function(visibleIndex) {
        ASPx.Data.ArrayRemove(this.invalidItemIndices, visibleIndex);
    },

    ProcessValidationErrors: function(validationInfo) {
        if(!validationInfo) return;
        for(var itemKey in validationInfo) {
            var itemValidationInfo = validationInfo[itemKey];
            var dataItem = this.GetDataItemByKey(itemKey);
            if(!dataItem) continue;
            if(itemValidationInfo.row)
                this.ShowItemValidationError(dataItem, itemValidationInfo.row);
            var editorErrors = itemValidationInfo.editors;
            for(var columnIndex in editorErrors)
                this.ShowCellValidationError(this.GetDataItemIndex(dataItem), columnIndex, editorErrors[columnIndex]);
        }
    },

    // Validation

    ValidateItems: function() {
        var pageValidationInfo = { };
        var isValid = true;
        var visibleIndices = this.GetDataItemVisibleIndices();
        for(var i = 0; i < visibleIndices.length; i++) {
            var visibleIndex = visibleIndices[i];
            var itemInfo = this.ValidateItem(visibleIndex);
            pageValidationInfo[visibleIndex] = itemInfo;
            isValid &= itemInfo.isValid;
        }
        return { isValid: isValid, dict: pageValidationInfo };
    },
    ValidateItem: function(visibleIndex, itemValues) {
        var isValid = true;
        var validationInfo = this.GetItemValidationInfo(visibleIndex, itemValues);
        if(validationInfo) {
            for(var columnIndex in validationInfo) {
                if(this.CanEditColumn(columnIndex))
                    isValid &= validationInfo[columnIndex].isValid;
            }
        }
        this.ResetItemValidationState(visibleIndex); // TODO save all validation state by visibleIndex and columnIndex (to correct ResetChanges)
        if(!isValid)
            this.invalidItemIndices.push(visibleIndex);

        return { isValid: isValid, dict: validationInfo };
    },
    GetItemValidationInfo: function(visibleIndex, itemValues) {
        if(!this.HasChanges(visibleIndex) && !this.IsEditingDataItem(visibleIndex)) 
            return;
        var result = { };
        var columnIndices = this.GetEditColumnIndices();
        for(var i = 0; i < columnIndices.length; i++) {
            var columnIndex = columnIndices[i];
            var value = this.GetCellValue(visibleIndex, columnIndex);
            if(itemValues && itemValues.hasOwnProperty(columnIndex))
                value = itemValues[columnIndex].value;
            result[columnIndex] = this.GetCellValidationInfo(visibleIndex, columnIndex, value);
        }
        result = this.grid.RaiseBatchEditItemValidating(visibleIndex, result);
        return result;
    },
    GetCellValidationInfo: function(visibleIndex, columnIndex, value) {
        var isValid = true;
        var errorText = "";

        var editor = this.GetEditor(columnIndex);
        if(editor) {
            if(this.IsEditingCell(visibleIndex, columnIndex))
                value = editor.GetValue();
            else
                editor.SetValue(value);

            this.ValidateEditor(editor);
            isValid = editor.GetIsValid();
            errorText = editor.GetErrorText();
        }
        return { value: value, isValid: isValid, errorText: errorText };
    },
    ValidateEditor: function(editor) {
        editor.Validate();
    },
    ApplyValidationInfo: function(visibleIndex, validationInfo) {
        var dataItem = this.GetDataItem(visibleIndex);
        if(!dataItem || !validationInfo || !validationInfo.dict) 
            return;
        var validColumnIndices = [ ];
        for(var columnIndex in validationInfo.dict) {
            if(!this.CanEditColumn(columnIndex)) continue;
            var cellInfo = validationInfo.dict[columnIndex];
            if(!cellInfo.isValid)
                this.ShowCellValidationError(visibleIndex, columnIndex, cellInfo.errorText);
            else
                validColumnIndices.push(columnIndex);
        }
        for(var i = 0; i < validColumnIndices.length; i++) {
            var textContainer = this.GetCellTextContainer(visibleIndex, validColumnIndices[i]);
            if(textContainer && textContainer.dxgvHasError) {
                this.UpdateItem(visibleIndex, validColumnIndices);
                break;
            }
        }
    },

    
    // CRUD

    AddNewItem: function() {
        var visibleIndex = this.CalcNewItemVisibleIndex();
        var newItem = this.AddNewItemCore(visibleIndex);
        if(!newItem) 
            return;

        this.insertedItemIndices.push(visibleIndex);
        var columnIndex = this.GetFirstVisibleColumnIndex(this.GetEditColumnIndices());
        this.StartEdit(visibleIndex, columnIndex);
        this.EnsureEmptyDataItemVisibility();
    },
    AddNewItemCore: function(visibleIndex) { 
        if(this.GetDataItem(visibleIndex))
            return;
        
        var newItem = this.CreateNewItemElement(visibleIndex);
        ASPx.SetElementDisplay(newItem, true);

        var refRow;
        for(var i = this.insertedItemIndices.length - 1; i >= 0; i--) {
            refRow = this.GetDataItem(this.insertedItemIndices[i]);
            if(refRow) break;
        }
        if(!refRow)
            refRow = this.GetEtalonNewItem();

        if(this.IsNewItemOnTop())
            refRow.parentNode.insertBefore(newItem, refRow);
        else
            ASPx.InsertElementAfter(newItem, refRow);
        return newItem;
    },

    CreateNewItemElement: function(visibleIndex) {
        var newItem = this.CreateNewItemElementCore(visibleIndex);
        newItem.id = this.GetDataItemID(visibleIndex);
        this.PrepareCommandButtonsIDList(newItem);
        return newItem;
    },
    CreateNewItemElementCore: function(visibleIndex) {
        var etalonItem = this.GetEtalonNewItem();
        return etalonItem.cloneNode(true);
    },

    
    DeleteItem: function(visibleIndex) {
        var key = visibleIndex >= 0 ? this.GetItemKey(visibleIndex) : visibleIndex;
        this.DeleteItemByKey(key);
    },
    DeleteItemByKey: function(key) {
        var item = this.GetDataItemByKey(key);
        if(!item) return;
        var visibleIndex = this.GetDataItemIndex(item);
        if(this.editItemVisibleIndex === visibleIndex && !this.EndEdit())
            return;
        if(this.grid._getFocusedItemIndex() === visibleIndex)
            this.UpdateFocusedItemIndex(visibleIndex);

        if(visibleIndex >= 0) {
            this.ChangeDeletedItemVisibility(visibleIndex, false);
            this.deletedItemKeys.push(key);
            if(ASPx.IsExists(this.updatedValues[key]))
                delete this.updatedValues[key];
        } else {
            this.ResetChanges(visibleIndex);
        }
        this.EnsureEmptyDataItemVisibility();
        this.RemoveItemError(visibleIndex);
        this.ResetItemValidationState(visibleIndex);
        this.UpdateSyncInput();
    },
    UpdateFocusedItemIndex: function(oldVisibleIndex) {
        var index = this.FindNewFocusedItemIndex(oldVisibleIndex, true);
        if(index === null)
            index = this.FindNewFocusedItemIndex(oldVisibleIndex, false);
        if(index === null)
            index = -1;
        this.grid._setFocusedItemIndex(index);
    },
    
    OnEditorValueChanged: function(editor, columnIndex) {
        if(this.IsCheckColumn(columnIndex)) {
            this.toogleCheckBoxColumnIndex = columnIndex;
            this.UpdateItem(this.editItemVisibleIndex, [ columnIndex ], true);
            this.toogleCheckBoxColumnIndex = -1;
        }
        editor.Validate();
    },


    // Html
    EnsureEmptyDataItemVisibility: function() {
        var dataItem = this.grid.GetEmptyDataItem();
        if(!dataItem) return;
        var show = !this.IsEditing() && this.RequireShowEmptyDataItem();
        ASPx.SetElementDisplay(dataItem, show);
    },
    ChangeDeletedItemVisibility: function(visibleIndex, show) { },
    SafeRemoveHtmlItem: function(visibleIndex) {
        var item = this.GetDataItem(visibleIndex)
        if(item) {
            this.MoveEditorsFromDeletingItem(visibleIndex);
            ASPx.RemoveElement(item);
        }
    },
    MoveEditorsFromDeletingItem: function(visibleIndex) {
        var item = this.GetDataItem(visibleIndex)
        if(!item) return;
        var editColumnIndices = this.GetEditColumnIndices();
        var editorsContainer = this.GetEditorsContainer();
        for(var i = 0; i < editColumnIndices.length; i++) {
            var columnIndex = editColumnIndices[i];
            var editorContainer = this.GetEditorContainer(columnIndex);
            if(ASPx.GetIsParent(item, editorContainer))
                editorsContainer.appendChild(editorContainer);
        }
    },
    RemoveItemErrors: function() {
        var visibleIndices = this.GetDataItemVisibleIndices();
        for(var i = 0; i < visibleIndices.length; i++)
            this.RemoveItemError(visibleIndices[i]);
    },
    RemoveItemError: function(visibleIndex) {
        ASPx.RemoveElement(this.GetItemError(visibleIndex));
    },
    CreateItemError: function(visibleIndex) {
        var item = this.grid.CreateEditingErrorItem();
        item.id = this.GetItemErrorID(visibleIndex);
        return item;
    },
    
    
    ShowItemValidationError: function(dataItem, errorText) {
        if(!dataItem) return false;
        var visibleIndex = this.GetDataItemIndex(dataItem);
        var itemError = this.GetItemError(visibleIndex);
        if(!itemError)
            itemError = this.CreateItemError(visibleIndex);
        this.InsertItemValidationError(dataItem, itemError, errorText);
        return true;
    },
    InsertItemValidationError: function(dataItem, itemError, errorText) { },

    ShowCellValidationError: function(visibleIndex, columnIndex, errorText) {
        var dataCell = this.GetActualDataCell(visibleIndex, columnIndex);
        var textContainer = this.GetCellTextContainer(visibleIndex, columnIndex);
        if(!dataCell || !textContainer || textContainer.dxgvHasError) 
            return;
        var html = textContainer.innerHTML;
        if(!ASPx.Str.Trim(html))
            html = "&nbsp;";
        textContainer.innerHTML = "";
        var errorTable = this.GetCellErrorTable().cloneNode(true);
        errorTable.id = this.GetCellErrorTableID(visibleIndex, columnIndex);
        var textCell = errorTable.rows[0].cells[0];
        var imageCell = errorTable.rows[0].cells[1];

        textCell.innerHTML = html;
        this.LoadCellAlign(textCell, columnIndex);

        var img = ASPx.GetNodeByTagName(imageCell, "IMG", 0);
        errorText = ASPx.IsExists(errorText) ? errorText : "";
        ASPx.Attr.SetAttribute(img, "alt", errorText);
        ASPx.Attr.SetAttribute(img, "title", errorText);
        textContainer.appendChild(errorTable);
        textContainer.dxgvHasError = true;
    },


    CreateTextContainer: function(dataCell) {
        var container = document.createElement("DIV");
        for(var i = dataCell.childNodes.length - 1; i >= 0; i--) {
            var child = dataCell.childNodes[i];
            if(container.childNodes.length > 0)
                container.insertBefore(child, container.childNodes[0]);
            else
                container.appendChild(child);
        }
        dataCell.appendChild(container);
        return container;
    },

    GetCheckBoxDisplayText: function(editor, columnIndex) {
        return this.GetCheckColumnDisplayText(editor.GetValue(), columnIndex);
    },
    GetCheckColumnDisplayText: function(value, columnIndex) {
        var columnDisplayHtml = this.GetCheckColumnsDisplayHtml();
        var state = columnDisplayHtml[columnIndex];
        for(var i = 0; i < state.length; i++) {
            var item = state[i];
            if(item && item[0] === value) // TODO use a simplified structure
                return item[1].toString();
        }
        return ASPx.IsExists(value) ? value.toString() : "";
    },
    GetColorEditDisplayText: function(editor, columnIndex) {
        return this.GetColorEditColumnDisplayText(editor.GetValue(), columnIndex);
    },
    GetColorEditColumnDisplayText: function(value, columnIndex) {
        if(!ASPx.IsExists(value))
            return "";
        value = ASPxClientColorEdit.ParseColor(value);
        if(!value) 
            return "";
        var displayText = this.GetColorEditColumnsDisplayHtml()[columnIndex];
        while(displayText.indexOf(this.ColorPlaceholder) > -1)
            displayText = displayText.replace(this.ColorPlaceholder, value.toUpperCase());
        return displayText;
    },
    GetBinaryImageDisplayText: function(editor, columnIndex) {
        var img = document.createElement('IMG');
        img.src =  ASPx.ImageUtils.GetImageSrc(editor.GetImageElement());
        return img.outerHTML;
    },




    // Processing
    ProcessTableClick: function(row, evt, isDblClick) {
        var src = ASPx.Evt.GetEventSource(evt);
        if(src.tagName === "INPUT" || src.tagName === "A" || src.tagName === "TEXTAREA")
            return;
        if(this.IsStartEditOnDblClick() ^ isDblClick || !this.IsActualDataItem(row))
            return;
        var visibleIndex = this.GetDataItemIndex(row);
        var columnIndex = this.GetColumnIndexByChild(row, src);
        if(this.IsSingleEditMode() && !this.CanEditColumn(columnIndex))
            return;
        var toogleCheckBox = !this.IsEditing() && this.IsCheckBoxDisplayControlClicked(columnIndex, src);
        
        if(this.CanStartEditOnTableClick(src, columnIndex))
            this.StartEdit(visibleIndex, columnIndex);

        if(this.IsEditing() && toogleCheckBox) {
            var editor = this.GetEditor(columnIndex);
            editor && editor.ToogleCheckState()
        }
    },
    CanStartEditOnTableClick: function(src, columnIndex) {
        var editor = this.GetEditor(columnIndex);
        if(editor && this.IsDateEdit(editor) && editor.GetShowTimeSection() && ASPx.GetIsParent(editor.GetTimeEdit().GetMainElement(), src))
            return false;
        return true;
    },

    IsActualDataItem: function(dataItem) { return this.IsDataItem(dataItem); },
    IsDataItem: function(item) {
        if(!item || !item.id) return false;
        var grid = this.grid;
        var indexInfo = grid.tryGetNumberFromEndOfString(item.id);
        if(!indexInfo.success) return false;
        return item.id === this.GetDataItemID(indexInfo.value);
    },
    GetDataItemID: function(index) { return this.grid.name + "_" + this.GetDataItemIDPrefix() + index.toString(); },
    GetDataItemIDPrefix: function() { },

    GetDataItem: function(visibleIndex) { },
    GetDataCell: function(dataItem, columnIndex) { },
    GetActualDataCell: function(visibleIndex, columnIndex) { 
        var dataItem = this.GetActualDataItem(visibleIndex, columnIndex);
        return this.GetDataCell(dataItem, columnIndex); 
    },
    GetActualDataItem: function(visibleIndex, columnIndex) { return this.GetDataItem(visibleIndex); },

    GetColumnIndex: function(dataCell) { },
    GetDataCellByChild: function(dataItem, element) { },
    GetColumnIndexByChild: function(dataItem, element) { 
        var dataCell = this.GetDataCellByChild(dataItem, element);
        return this.GetColumnIndex(dataCell);
    },
    GetCellTextContainer: function(visibleIndex, columnIndex) {
        var dataCell = this.GetActualDataCell(visibleIndex, columnIndex);
        if(!dataCell) return;
        var container = ASPx.GetChildByClassName(dataCell, "dxgvBCTC");
        if(!container) {
            container = this.CreateTextContainer(dataCell);
            container.className = "dxgvBCTC";
        }
        return container;
    },


    // Editing
    StartEditCell: function(visibleIndex, columnIndex) {
        var columnIndices = this.GetEditColumnIndices();
        if(!ASPx.IsExists(columnIndex) || columnIndex < 0 || !ASPx.Data.ArrayIndexOf(columnIndices, columnIndex))
            columnIndex = this.GetFirstVisibleColumnIndex(columnIndices);
        this.StartEdit(visibleIndex, columnIndex);
    },
    StartEditItemByKey: function(key) {
        var columnIndex = this.GetFirstVisibleColumnIndex(this.GetEditColumnIndices());
        var visibleIndex = this.FindItemVisibleIndexByKey(key);
        this.StartEdit(visibleIndex, columnIndex);
    },
    StartEdit: function(visibleIndex, columnIndex) { 
        if(!this.focusHelper.CanStartEdit()) 
            return;
        if(this.IsSingleEditMode() && this.IsEditingCell(visibleIndex, columnIndex))
            return;
        if(!this.IsSingleEditMode() && this.IsEditingDataItem(visibleIndex) && this.CanEditColumn(columnIndex)) {
            this.focusHelper.FocusColumnEditor(columnIndex);
            return;
        }
        if(this.IsEditing() && !this.EndEdit())
            return;
        var indices = this.IsSingleEditMode() ? [ columnIndex ] : this.GetEditColumnIndices();
        this.StartEditCore(visibleIndex, indices, columnIndex);
    },
    StartEditCore: function(visibleIndex, columnIndices, focusedColumnIndex) {
        var item = this.GetDataItem(visibleIndex);
        if(!item || !columnIndices || columnIndices.length === 0) 
            return;
        columnIndices = this.IntersectColumnIndices(this.GetEditColumnIndices(), columnIndices);
        if(columnIndices.length === 0)
            return;
        if(!ASPx.IsExists(focusedColumnIndex) || !this.CanEditColumn(focusedColumnIndex))
            focusedColumnIndex = this.GetFirstVisibleColumnIndex(columnIndices);
        var startEditArgs = this.grid.RaiseBatchEditStartEditing(visibleIndex, this.GetColumn(focusedColumnIndex), this.GetItemValues(visibleIndex));
        if(!startEditArgs || startEditArgs.cancel || this.GetObjectSize(startEditArgs.itemValues) === 0) 
            return;
        var itemValues = startEditArgs.itemValues;
        columnIndices = this.IntersectColumnIndices(ASPx.GetObjectKeys(itemValues), columnIndices);
        if(columnIndices.length === 0)
            return;
        
        ASPx.Selection.Clear();
        this.MoveEditors(visibleIndex, columnIndices);
        this.SetEditorValues(itemValues, columnIndices);
        this.UpdateItem(visibleIndex, columnIndices, true);

        this.editItemVisibleIndex = visibleIndex;
        this.editedColumnIndices = columnIndices;

        this.focusHelper.OnEditStarted(visibleIndex, columnIndices, startEditArgs.focusedColumn);
    },
    SetEditorValues: function(itemValues, columnIndices) {
        for(var i = 0; i < columnIndices.length; i++) {
            var columnIndex = columnIndices[i];
            var editor = this.GetEditor(columnIndex);
            if(editor && itemValues.hasOwnProperty(columnIndex)) {
                editor.SetIsValid(true);
                editor.SetValue(itemValues[columnIndex].value);
            }
        }
    },
    MoveEditors: function(visibleIndex, columnIndices) {
        for(var i = 0; i < columnIndices.length; i++) {
            var columnIndex = columnIndices[i];
            var editorContainer = this.GetEditorContainer(columnIndex);
            var textContainer = this.GetCellTextContainer(visibleIndex, columnIndex);
            if(!editorContainer || !textContainer)
                continue;
            ASPx.SetElementDisplay(editorContainer, false);
            ASPx.SetElementDisplay(textContainer, true);

            var dataCell = textContainer.parentNode;
            if(editorContainer.parentNode !== dataCell)
                dataCell.appendChild(editorContainer);
        }
    },
    EndEdit: function(skipValidation) {
        var dataItem = this.GetDataItem(this.editItemVisibleIndex);
        if(!this.IsEditing() || !dataItem) 
            return true;

        var itemValues = this.GetItemValues(this.editItemVisibleIndex);
        this.LoadEditorValues(itemValues, this.editItemVisibleIndex, this.editedColumnIndices);

        var endEditArgs = this.grid.RaiseBatchEditEndEditing(this.editItemVisibleIndex, itemValues);
        if(!endEditArgs || endEditArgs.cancel) 
            return false;
        var endEditValidationInfo = this.GetEndEditValidationInfo(endEditArgs.itemValues, skipValidation);
        if(!endEditValidationInfo.allowEndEdit)
            return false;

        var validationInfo = endEditValidationInfo.info;
        itemValues = this.FilterItemValues(endEditArgs.itemValues, this.editedColumnIndices);
        this.ApplyChanges(this.editItemVisibleIndex, itemValues);
        this.UpdateItem(this.editItemVisibleIndex, this.editedColumnIndices);
        this.ApplyValidationInfo(this.editItemVisibleIndex, validationInfo);
        this.UpdateSyncInput();

        this.editItemVisibleIndex = this.InvalidItemVisibleIndex;
        this.editedColumnIndices = [ ];
        this.focusHelper.OnEditEnded();
        return true;
    },
    GetEndEditValidationInfo: function(itemValues, skipValidation) {
        var info = null;
        var allowEndEdit = true;
        if(!skipValidation && this.RequireValidateOnEndEdit()) {
            info = this.ValidateItem(this.editItemVisibleIndex, itemValues);
            var isValid = this.IsSingleEditMode() ? info.dict[this.editedColumnIndices[0]].isValid : info.isValid;
            if(!isValid && !this.AllowEndEditOnError())
                allowEndEdit = false;
        }
        return { info: info, allowEndEdit: allowEndEdit };
    },
    LoadEditorValues: function(itemValues, visibleIndex, columnIndices) {
        for(var i = 0; i < columnIndices.length; i++) {
            var columnIndex = columnIndices[i];
            var editor = this.GetEditor(columnIndex);
            if(!editor) continue;
            if(ASPx.IsFunction(editor.OnTextChanged))
                editor.OnTextChanged();

            var value = editor.GetValue();
            if(value === null && editor.convertEmptyStringToNull && this.GetServerValue(visibleIndex, columnIndex) === "")
                value = "";

            var text = this.GetEditorDisplayText(editor, columnIndex);
            itemValues[columnIndex] = { value: value, text: text };
        }
    },
    ApplyChanges: function(visibleIndex, itemValues) {
        for(var columnIndex in itemValues) {
            var info = itemValues[columnIndex];
            this.SetCellValue(visibleIndex, columnIndex, info.value, info.text);
        }
    },
    IsEquals: function(val1, val2) { // TODO create internal event
        if(val1 === val2)
            return true;
        if(ASPx.IsExists(val1) && ASPx.IsExists(val2) && val1.toString() === val2.toString())
            return true;
        return false;
    },
    UpdateItem: function(visibleIndex, columnIndices, showEditors, reset, fromAPI) { // TODO refactor "fromAPI"
        var dataItem = this.GetDataItem(visibleIndex);
        if(!dataItem) return;
        var cellSizes = showEditors ? this.GetCellSizes(visibleIndex, columnIndices) : null;
        for(var i = 0; i < columnIndices.length; i++) {
            var columnIndex = columnIndices[i];
            var cellSize = cellSizes ? cellSizes[columnIndex] : null;
            this.UpdateCell(visibleIndex, columnIndex, showEditors, reset, cellSize, fromAPI);
        }
    },
    UpdateCell: function(visibleIndex, columnIndex, showEditor, reset, cellSize, fromAPI) {
        if(reset) showEditor = false;
        var dataCell = this.GetActualDataCell(visibleIndex, columnIndex);
        var editorContainer = this.GetEditorContainer(columnIndex);
        var textContainer = this.GetCellTextContainer(visibleIndex, columnIndex);
        if(!dataCell || !editorContainer || !textContainer) 
            return;

        ASPx.SetElementDisplay(textContainer, !showEditor);
        if(!showEditor)
            ASPx.SetInnerHtml(textContainer, this.GetCellText(visibleIndex, columnIndex));

        var styleType = this.GetStyleType(visibleIndex, columnIndex, showEditor, reset);
        this.ChangeCellStyle(visibleIndex, columnIndex, styleType);

        if(showEditor && cellSize) {
            var width = showEditor ? cellSize.width - ASPx.GetLeftRightBordersAndPaddingsSummaryValue(dataCell) : "";
            var height = showEditor ? cellSize.height - ASPx.GetTopBottomBordersAndPaddingsSummaryValue(dataCell) : "";
            if(ASPx.IsNumber(width) && width < 0)
                width = 0;
            if(ASPx.IsNumber(height) && height < 0)
                height = 0;
            ASPx.SetStyles(dataCell, { width: width, height: height });
        }

        var display = showEditor ? "" : "none";
        if(showEditor && this.IsCheckColumn(columnIndex))
            display = "inline-block";
        if(!fromAPI)
            ASPx.SetStyles(editorContainer, { display: display });
        
        if(ASPx.Browser.Firefox) {
            var editor = this.GetEditor(columnIndex);
            var input = editor && editor.GetInputElement();
            try {
                if(input)
                    input.setSelectionRange(0, 0, "backward");
            } catch(e) { }
        }

        if(this.IsHiddenColumn(columnIndex))
            ASPx.SetElementDisplay(dataCell, false);

        textContainer.dxgvHasError = false;
    },
    GetCellSizes: function(visibleIndex, columnIndices) {
        var result = { };
        for(var i = 0; i < columnIndices.length; i++) { 
            var columnIndex = columnIndices[i];
            var dataCell = this.GetActualDataCell(visibleIndex, columnIndex);
            if(dataCell)
                result[columnIndex] = { width: dataCell.offsetWidth, height: dataCell.offsetHeight };
        }
        return result;
    },
    
    GetEditorDisplayText: function(editor, columnIndex) {
        if(this.IsCheckColumn(columnIndex))
            return this.GetCheckBoxDisplayText(editor, columnIndex);
        if(this.IsColorEditColumn(columnIndex))
            return this.GetColorEditDisplayText(editor, columnIndex);
        if(this.IsBinaryImageColumn(columnIndex))
            return this.GetBinaryImageDisplayText(editor, columnIndex);
        var text = ASPx.IsFunction(editor.GetFormattedText) ? editor.GetFormattedText() : editor.GetText();
        text = editor.HtmlEncode(text);
        var input = editor.GetInputElement();
        if(ASPx.IsExists(text) && text.length > 0 && input && ASPx.Attr.GetAttribute(input, "type") == "password") {
            var bag = [ ];
            bag[text.length] = "";
            text = bag.join("*");
        }
        return text;
    },

    // Styles
    GetStyleType: function(visibleIndex, columnIndex, showEditor, reset) {
        if(reset) return ASPxClientGridItemStyle.Item;
        if(this.HasModifiedStyle(visibleIndex, columnIndex))
            return showEditor ? ASPxClientGridItemStyle.BatchEditMergedModifiedCell : ASPxClientGridItemStyle.BatchEditModifiedCell;
        return showEditor ? ASPxClientGridItemStyle.BatchEditCell : ASPxClientGridItemStyle.Item;
    },
    HasModifiedStyle: function(visibleIndex, columnIndex) {
        if(this.toogleCheckBoxColumnIndex === columnIndex)
            return !this.IsEquals(this.GetEditor(columnIndex).GetValue(), this.GetServerValue(visibleIndex, columnIndex));
        return this.IsDirtyCell(visibleIndex, columnIndex);
    },
    ChangeCellStyle: function(visibleIndex, columnIndex, styleType) {
        columnIndex = parseInt(columnIndex.toString(), 10);
        var cell = this.GetActualDataCell(visibleIndex, columnIndex);
        if(cell && styleType)
            this.ChangeCellStyleCore(cell, visibleIndex, columnIndex, styleType);
    },
    ChangeCellStyleCore: function(cell, visibleIndex, columnIndex, styleType) {
        if(styleType === ASPxClientGridItemStyle.Item) {
            if(ASPx.IsExists(cell.dxgvSavedClassName))
                cell.className = cell.dxgvSavedClassName;
            if(ASPx.IsExists(cell.dxgvSavedCssText))
                cell.style.cssText = cell.dxgvSavedCssText;
            return;
        }

        if(!ASPx.IsExists(cell.dxgvSavedClassName))
            cell.dxgvSavedClassName = cell.className;
        if(!ASPx.IsExists(cell.dxgvSavedCssText))
            cell.dxgvSavedCssText = cell.style.cssText;
        var styleInfo = this.grid.getItemStyleInfo(styleType);
        
        cell.className = cell.dxgvSavedClassName + " " + styleInfo.css; 
        cell.style.cssText = cell.dxgvSavedCssText + ";" + styleInfo.style;
    },
    ChangeFormLayoutItemStyle: function(element, styleType) {
        if(!ASPx.FormLayoutConsts) return;
        
        var cell = ASPx.GetParentByPartialClassName(element, ASPx.FormLayoutConsts.GROUP_CELL_PARTIAL_CLASS_NAME);
        if(cell && cell.tagName === "DIV") // Flow mode
            cell = cell.parentNode;
        
        var itemElement = ASPx.GetNodeByClassName(cell, ASPx.FormLayoutConsts.ITEM_SYSTEM_CLASS_NAME);
        if(itemElement) 
            ASPxClientFormLayout.UpdateNestedControlTypeClassName(itemElement, 
                styleType === ASPxClientGridItemStyle.BatchEditCell || styleType === ASPxClientGridItemStyle.BatchEditMergedModifiedCell);
    },
    LoadCellAlign: function(target, columnIndex) {
        return; // TODO
        var sourceCell = this.GetDataCell(this.GetEtalonNewItem(), columnIndex);
        if(!sourceCell) return;

        var cellAlign = ASPx.Attr.GetAttribute(sourceCell, "align");
        if(cellAlign) {
            ASPx.Attr.SetAttribute(textCell, "align", cellAlign);
            return;
        }

        if(sourceCell.className.indexOf("dx-al") > -1)
            target.className += " dx-al";
        else if(sourceCell.className.indexOf("dx-ar") > -1)
            target.className += " dx-ar";
        else if(sourceCell.className.indexOf("dx-ac") > -1)
            target.className += " dx-ac";
    },

    // API
    GetVisibleItemsOnPageCount: function() {
        return this.grid.pageRowCount - this.deletedItemKeys.length + this.insertedItemIndices.length;
    },
    IsValidVisibleIndex: function(visibleIndex) {
        var key = this.GetItemKey(visibleIndex);
        var startIndex = this.grid.GetTopVisibleIndex();
        var endIndex = startIndex + this.grid.pageRowCount - 1;
        return startIndex <= visibleIndex && visibleIndex <= endIndex && !this.IsDeletedItem(key) || this.IsNewItem(key);
    },
    CanUpdate: function() {
        if(this.IsEditing() && !this.EndEdit())
            return false;
        if(!this.RequireValidateOnEndEdit()) {
            var pageValidationInfo = this.ValidateItems();
            if(pageValidationInfo.isValid)
                return true;
            for(var visibleIndex in pageValidationInfo.dict)
                this.ApplyValidationInfo(visibleIndex, pageValidationInfo.dict[visibleIndex]);
        }
        return this.invalidItemIndices.length === 0 && this.HasChanges();
    },
    CancelEdit: function() {
        if(this.EndEdit(true)) {
            this.ResetAllChanges();
            this.RemoveItemErrors();
        }
    },
    OnUpdate: function() {
        this.updating = true;
        this.UpdateSyncInput();
    },
    GetColumnDisplayTextByEditor: function(value, columnIndex) {
        var editor = this.GetEditor(columnIndex);
        if(!editor)
            return ASPx.IsExists(value) ? this.GetValueString(value) : "";

        var prevValue = editor.GetValue();
        editor.SetValue(value);
        var processedText = this.GetEditorDisplayText(editor, columnIndex);
        editor.SetValue(prevValue);
        return processedText;
    },
    OnItemStyleChanged: function(visibleIndex) { },


    // server sync
    UpdateSyncInput: function() { 
        this.grid.stateObject.batchEditClientModifiedValues = { ClientState: this.GetClientState(), EditState: this.GetEditState() }; 
    },
    GetClientState: function() { 
        return { InsertedIndices: this.insertedItemIndices, InsertedValues: this.insertedValues, UpdatedValues: this.updatedValues, DeletedKeys: this.deletedItemKeys }; 
    },
    GetEditState: function() {
        var modifiedValues = { };
        var insertedRowValues = { };
        var editColumnIndices = this.GetEditColumnIndices();
        for(var rowKey in this.updatedValues) {
            var visibleIndex = this.FindItemVisibleIndexByKey(rowKey);
            modifiedValues[rowKey] = this.CreateItemEditState(visibleIndex, editColumnIndices);
        }
        for(var i = 0; i < this.insertedItemIndices.length; i++) {
            var visibleIndex = this.insertedItemIndices[i];
            insertedRowValues[visibleIndex] = this.CreateItemEditState(visibleIndex, editColumnIndices);
        }
        return { insertedRowValues : insertedRowValues, modifiedRowValues : modifiedValues, deletedRowKeys : this.deletedItemKeys };
    },
    CreateItemEditState: function(visibleIndex, columnIndices) {
        var grid = this.grid;
        var result = { };
        for(var i = 0; i < columnIndices.length; i++) {
            var columnIndex = columnIndices[i];
            var fieldName = grid.GetColumn(columnIndex).fieldName;
            var value = this.GetCellValue(visibleIndex, columnIndex);
            result[fieldName] = this.GetValueString(value);
        }
        return result;
    },
    



    // Utils

    GetDataItemVisibleIndices: function(doNotIncludeDeleted) {
        var insertIndices = this.insertedItemIndices.slice();
        if(this.IsNewItemOnTop())
            insertIndices.reverse();

        var dataIndices = [ ];
        var startIndex = this.grid.GetTopVisibleIndex();
        var endIndex = startIndex + this.grid.pageRowCount;
        for(var i = startIndex; i < endIndex; i++) {
            var dataItem = this.GetDataItem(i);
            if(!dataItem || doNotIncludeDeleted && this.IsDeletedItem(this.GetItemKey(i)))
                continue;
            dataIndices.push(i);
        }

        var parts = [ insertIndices, dataIndices ];
        if(!this.IsNewItemOnTop())
            parts.reverse();

        return parts[0].concat(parts[1]);
    },

    FindItemVisibleIndexByKey: function(key) { // TODO refactor
        var grid = this.grid;
        var startIndex = grid.GetTopVisibleIndex();
        var endIndex = startIndex + grid.pageRowCount;
        for(var i = startIndex; i < endIndex; i++) {
            if(grid.GetItemKey(i) === key)
                return i;
        }
        return null;
    },
    GetValueString: function(value) {
        if(value == null) return null;
        if(ASPx.Ident.IsDate(value))
            return ASPx.DateUtils.GetInvariantDateTimeString(value);
        return value.toString();
    },

    RequireShowEmptyDataItem: function() {
        var indices = this.GetDataItemVisibleIndices();
        for(var i = 0; i < indices.length; i++) {
            var dataItem = this.GetDataItem(indices[i]);
            if(dataItem && ASPx.GetElementDisplay(dataItem))
                return false;
        }
        return true;
    },

    GetFirstVisibleColumnIndex: function(columnIndices) {
        if(columnIndices.length === 1)
            return columnIndices[0];
        var visibleColumnIndices = this.GetVisibleColumnIndices();
        for(var i = 0; i < columnIndices.length; i++) {
            var columnIndex = visibleColumnIndices[i];
            if(ASPx.Data.ArrayIndexOf(columnIndices, columnIndex) >= 0)
                return columnIndex;
        }
        return -1;
    },

    CalcNewItemVisibleIndex: function() {
        var indices = this.insertedItemIndices;
        var lastIndex = indices.length > 0 ? indices[indices.length - 1] : 0;
        return lastIndex - 1;
    },
    FindNewFocusedItemIndex: function(oldVisibleIndex, direction) {
        var inc = direction ? 1 : -1;
        var visibleIndices = this.GetDataItemVisibleIndices();
        var index = ASPx.Data.ArrayIndexOf(visibleIndices, oldVisibleIndex);
        index += inc;
        var bounds = {
            left: direction ? index : visibleIndices[0],
            right: direction ? visibleIndices[visibleIndices.length - 1] : index
        };
        while(index >= bounds.left && index <= bounds.right) {
            var dataItem = this.GetDataItem(index);
            if(dataItem && ASPx.GetElementDisplay(dataItem))
                return index;
            index += inc;
        }
        return null;
    },
    GetDataItemByKey: function(key) {
        var visibleIndex = this.FindItemVisibleIndexByKey(key);
        if(visibleIndex === null) {
            visibleIndex = parseInt(key, 10);
            if(isNaN(visibleIndex) || visibleIndex >= 0)
                return null;
        }
        return this.GetDataItem(visibleIndex);
    },
    PrepareCommandButtonsIDList: function(item) {
        if(!this.grid.cButtonIDs)
            this.grid.cButtonIDs = [ ];
        var initButtonIndex = this.GetLastCommandButtonIndex() + 1;
        var elementList = item.getElementsByTagName('*');
        for(var i = 0; i < elementList.length; i++) {
            var element = elementList[i];
            var comandButtonIDPattern = /%DXItemIndex\d+%$/g; // T284481
            if(element.id && comandButtonIDPattern.test(element.id)) {
                var patternID = element.id;
                var buttonID = patternID.replace(comandButtonIDPattern, initButtonIndex++);
                ASPx.SetInnerHtml(element.parentNode, element.parentNode.innerHTML.replace(new RegExp(patternID, "g"), buttonID));
                this.grid.cButtonIDs.push(buttonID);
            }
        }
    },
    GetLastCommandButtonIndex: function() {
        var index = -1;
        ASPx.GetControlCollection().ForEachControl(function(control) {
            if(control.cpGVName !== this.grid.name)
                return;
            if(!control.GetMainElement()) { // TODO
                ASPx.GetControlCollection().Remove(control);
                return;
            }
            var matches = control.name.match(/_DXCBtn(\d+)/);
            if(matches && matches.length > 1) {
                var buttonIndex = parseInt(matches[1]);
                index = Math.max(index, buttonIndex);
            }
        }, this);
        return index;
    },
    FilterItemValues: function(rowValues, columnIndices) {
        var result = { };
        if(!rowValues || columnIndices.length === 0)
            return result;
        for(var i = 0; i < columnIndices.length; i++) {
            var columnIndex = columnIndices[i];
            if(rowValues[columnIndex])
                result[columnIndex] = rowValues[columnIndex];
        }
        return result;
    },
    IntersectColumnIndices: function(indices1, indices2) {
        var hash = { };
        for(var i = 0; i < indices1.length; i++)
            hash[indices1[i]] = true;
        var result = [ ];
        for(var i = 0; i < indices2.length; i++) {
            var index = indices2[i];
            if(hash[index])
                result.push(index);
        }
        return result;
    },
    IsCheckBoxDisplayControlClicked: function(columnIndex, element) {
        if(!this.IsCheckColumn(columnIndex)) return false;
        var className = element.className;
        if(!className) return false;
        var state = this.GetCheckColumnsDisplayHtml()[columnIndex];
        for(var i = 0; i < state.length; i++) {
            var item = state[i];
            if(!item || item.length !== 2) continue;
            var displayControlHtml = item[1];
            if(displayControlHtml.indexOf(className) >= 0)
                return true;
        }
        return false;
    },
});


var GridViewBatchEditHelper = ASPx.CreateClass(GridBatchEditHelper, {
    LastVisibleRowClassName: "dxgvBELVR",

    constructor: function(grid) { 
        this.constructor.prototype.constructor.call(this, grid);
    },
    
    GetDataItem: function(visibleIndex) { return this.grid.GetDataRow(visibleIndex); },


    Init: function() {
        this.constructor.prototype.Init.call(this);
        this.UpdateLastVisibleDataRow();
    },
    OnAfterCallback: function() {
        this.constructor.prototype.OnAfterCallback.call(this);
        this.UpdateLastVisibleDataRow();
        this.EnsureEmptyDataItemVisibility();
    },
    
    ResetChangesCore: function(key, visibleIndex, columnIndex) {
        this.constructor.prototype.ResetChangesCore.call(this, key, visibleIndex, columnIndex);
        this.UpdateLastVisibleDataRow();
    },

    AddNewItem: function() {
        this.constructor.prototype.AddNewItem.call(this);
        this.grid.CalculateAdaptivity();
    },
    AddNewItemCore: function(visibleIndex) {
        var newItem = this.constructor.prototype.AddNewItemCore.call(this, visibleIndex);

        this.grid.PrepareCommandButtons(); // TODO move to base class
        this.UpdateLastVisibleDataRow();
        this.focusHelper.OnDataItemAdded(visibleIndex); // TODO move to base class

        return newItem;
    },
    

    DeleteItemByKey: function(key) {
        this.constructor.prototype.DeleteItemByKey.call(this, key);
        this.UpdateLastVisibleDataRow();
    },

    OnItemStyleChanged: function(visibleIndex) { 
        if(this.IsDeletedItem(this.GetItemKey(visibleIndex)))
            ASPx.SetElementDisplay(this.GetDataItem(visibleIndex), false);
        this.UpdateLastVisibleDataRow();
    },

    UpdateLastVisibleDataRow: function() {
        var tBody = ASPx.GetNodeByTagName(this.grid.GetMainTable(), "TBODY", 0);
        var prevLastRows = ASPx.GetChildNodesByClassName(tBody, this.LastVisibleRowClassName);
        if(prevLastRows.length > 0)
            ASPx.RemoveClassNameFromElement(prevLastRows[0], this.LastVisibleRowClassName);
        var lastRow = this.FindLastVisibleDataRow();
        if(lastRow)
            ASPx.AddClassNameToElement(lastRow, this.LastVisibleRowClassName);
    },
    FindLastVisibleDataRow: function() {
        var dataGroupRowRegEx = this.grid.GetItemVisibleIndexRegExp(true);
        var rows = this.grid.GetMainTable().rows;
        for(var i = rows.length - 1; i >= 0; i--) {
            var row = rows[i];
            if(dataGroupRowRegEx.test(row.id) && ASPx.GetElementDisplay(row))
                return row;
        }
        return this.grid.GetEmptyDataItem();
    },


    ChangeDeletedItemVisibility: function(visibleIndex, show) {
        var grid = this.grid;
        var rows = [ this.GetDataItem(visibleIndex), grid.GetDetailRow(visibleIndex), grid.GetPreviewRow(visibleIndex) ];
        if(!show && grid.hasAdaptiveElements)
            rows.push(grid.GetAdaptiveDetailRow(visibleIndex));
        for(var i = 0; i < rows.length; i++) {
            var row = rows[i];
            if(row)
                ASPx.SetElementDisplay(row, show);
        }
    },

    GetVisibleRowsIndicesForAdaptivity: function() {
        var groupRowIndices = [ ];
        var start = this.grid.GetTopVisibleIndex();
        var end = start + this.grid.pageRowCount;
        for(var i = start; i < end; i++) {
            if(this.grid.IsGroupRow(i))
                groupRowIndices.push(i);
        }
        var indices = this.GetDataItemVisibleIndices(true);
        return indices.concat(groupRowIndices);
    },



    ////////////////////
    GetDataItemIDPrefix: function() { return this.grid.DataRowID },
    IsAdaptiveDetailRow: function(row) {
        if(!row || !row.id) return false;
        var grid = this.grid;
        var indexInfo = grid.tryGetNumberFromEndOfString(row.id);
        if(!indexInfo.success) return false;
        return row.id === this.GetAdaptiveDetailRowID(indexInfo.value);
    },
    GetAdaptiveDetailRowID: function(index) { return this.grid.name + "_" + this.grid.AdaptiveDetailRowID + index.toString(); },
    IsActualDataItem: function(dataItem) { return this.IsDataItem(dataItem) || this.IsAdaptiveDetailRow(dataItem); },

    GetColumnIndex: function(dataCell) {
        if(!dataCell) return -1;
        if(dataCell.columnIndex) return dataCell.columnIndex;

        var cellIndex = dataCell.cellIndex - this.grid.indentColumnCount;
        var columnIndices = this.GetVisibleColumnIndices();
        if(cellIndex < 0 || cellIndex >= columnIndices.length) 
            return -1;
        return columnIndices[cellIndex];
    },
    GetDataCellByChild: function(dataItem, element) {
        if(this.IsAdaptiveDetailRow(dataItem))
            return this.GetAdaptiveDataCellByChild(dataItem, element);
        while(element && element.parentNode !== dataItem)
            element = element.parentNode;
        return element;
    },
    GetAdaptiveDataCellByChild: function(dataItem, element) {
        while(element && element.tagName !== "BODY") {
            if(ASPx.ElementHasCssClass(element, this.grid.AdaptiveDetailDataCellCssClass))
                return element;
            element = element.parentNode;
        }
        return null;
    },
    GetDataCell: function(row, columnIndex) {
        var column = this.GetColumn(columnIndex)
        if(column.adaptiveHidden) {
            var visibleIndex = this.GetDataItemIndex(row);
            var adaptiveDetailsCell = this.grid.GetAdaptiveDetailCell(visibleIndex, false);
            if(adaptiveDetailsCell && adaptiveDetailsCell.adaptiveDetailsCells)
                return adaptiveDetailsCell.adaptiveDetailsCells[columnIndex];
        }
        return this.grid.GetDataCell(row, columnIndex);
    },
    GetActualDataItem: function(visibleIndex, columnIndex) {
        var column = this.GetColumn(columnIndex);
        var adaptiveRow = this.grid.GetAdaptiveDetailRow(visibleIndex);
        return column.adaptiveHidden && adaptiveRow ? adaptiveRow : this.GetDataItem(visibleIndex);
    },
    
    


    // Validation errors
    
    
    InsertItemValidationError: function(dataItem, itemError, errorText) {
        var cell = this.grid.GetLastNonAdaptiveIndentCell(itemError);
        cell.innerHTML = errorText;
        ASPx.InsertElementAfter(itemError, dataItem);
    },

    
    
    ChangeCellStyleCore: function(cell, visibleIndex, columnIndex, styleType) {
        this.constructor.prototype.ChangeCellStyleCore.call(this, cell, visibleIndex, columnIndex, styleType);
        if(!cell) 
            return;
        this.LoadCellAlign(cell, columnIndex);
        var indices = this.GetVisibleColumnIndices();
        if(indices.length > 0 && columnIndex === indices[indices.length - 1] && !this.grid.HasScrolling())
            ASPx.SetStyles(cell, { borderRightWidth: 0 });

        this.ChangeFormLayoutItemStyle(cell, styleType);
    },
    
});


var CardViewBatchEditHelper = ASPx.CreateClass(GridBatchEditHelper, {
    EmptyHiddenCardClassName: "dxcvEmptyHiddenCard",

    constructor: function(grid) { 
        this.constructor.prototype.constructor.call(this, grid);
    },
    
    GetDataItemIDPrefix: function() { return this.grid.CardID; },
    GetLayoutControl: function(visibleIndex) { return this.grid.GetCardLayoutControl(visibleIndex); },
    GetLayoutItemPaths: function() { return this.grid.batchEditClientState.layoutItemPaths },
    GetDataItem: function(visibleIndex) { return this.grid.GetItem(visibleIndex); },
    GetVisibleColumnIndices: function() { return [ ]; }, // for test purposes


    CreateNewItemElementCore: function(visibleIndex) {
        var etalonItem = this.GetEtalonNewItem();
        var wrapper = document.createElement("DIV");
        var outerHTML = etalonItem.outerHTML;

        var etalonIDPostfix = this.grid.CardLayoutID + "Etalon";
        var newIDPostfix = this.grid.CardLayoutID + visibleIndex;
        outerHTML = outerHTML.replace(new RegExp(etalonIDPostfix, "g"), newIDPostfix); // patch elements ID
        outerHTML = outerHTML.replace(new RegExp("dxss_(\\d+)", "g"), function(str) { return str + Math.floor(10000 * (1 + Math.random())) + "g"; }); // patch script ID

        if(!this.grid.IsFlowLayoutMode())
            outerHTML = "<table><tbody>" + outerHTML + "</tbody></table>"


        wrapper.innerHTML = outerHTML;
        var newItem = ASPx.GetNodesByPartialClassName(wrapper, this.grid.GetCssClassNamePrefix())[0];
        return newItem;
    },

    AddNewItemCore: function(visibleIndex) {
        var newItem = !this.grid.IsFlowLayoutMode() 
            ? this.AddNewTableLayoutItem(visibleIndex) : this.constructor.prototype.AddNewItemCore.call(this, visibleIndex);
        

        this.grid.PrepareCommandButtons();
        this.focusHelper.OnDataItemAdded(visibleIndex);

        ASPx.RunStartupScripts();

        return newItem;
    },

    AddNewTableLayoutItem: function(visibleIndex) {
        var itemToReplace = this.FindEmptyItemToReplaceByNew();

        var newItem = this.CreateNewItemElement(visibleIndex);
        ASPx.InsertElementAfter(newItem, itemToReplace);
        ASPx.RemoveElement(itemToReplace);

        return newItem;
    },

    FindEmptyItemToReplaceByNew: function() {
        var colCount = this.grid.colCount;

        var mainTable = this.grid.GetMainTable();
        var etalonItem = this.GetEtalonNewItem();
        var etalonRow = etalonItem.parentNode;
        var etalonRowIndex = etalonRow.sectionRowIndex;

        var createNewRow = this.insertedItemIndices.length === 0 || this.insertedItemIndices.length % colCount === 0;
        if(!createNewRow) {
            var index = this.IsNewItemOnTop() ? 0 : mainTable.rows.length - 1;
            return ASPx.GetChildNodesByPartialClassName(mainTable.rows[index], this.EmptyHiddenCardClassName)[0];
        }

        var step = this.IsNewItemOnTop() ? 1 : -1;
        var etalonSeparatorRow = mainTable.rows[etalonRowIndex + step];

        var newDataRow = etalonRow.cloneNode(true);
        var newSeparatorRow = etalonSeparatorRow.cloneNode(true);

        ASPx.SetElementDisplay(newDataRow, true);
        ASPx.SetElementDisplay(newSeparatorRow, true);

        if(this.IsNewItemOnTop()) {
            etalonRow.parentNode.insertBefore(newSeparatorRow, mainTable.rows[0]);
            etalonRow.parentNode.insertBefore(newDataRow, mainTable.rows[0]);
        } else {
            ASPx.InsertElementAfter(newSeparatorRow, mainTable.rows[mainTable.rows.length - 1]);
            ASPx.InsertElementAfter(newDataRow, mainTable.rows[mainTable.rows.length - 1]);
        }

        return newDataRow.cells[0];
    },

    SafeRemoveHtmlItem: function(visibleIndex) {
        var item = this.GetDataItem(visibleIndex);
        if(!item) return;

        this.MoveEditorsFromDeletingItem(visibleIndex);

        if(this.grid.IsFlowLayoutMode()) {
            ASPx.RemoveElement(item);
            return;
        }
        if(this.IsNewItem(visibleIndex)) {
            this.RemoveInsertedCard(visibleIndex);
            return;
        }

        var emptyCard = this.CreateEmptyCard();
        if(emptyCard) {
            ASPx.InsertElementAfter(emptyCard, item);
            ASPx.RemoveElement(item);
        }
    },


    RemoveInsertedCard: function(visibleIndex) {
        var item = this.GetDataItem(visibleIndex);
        var replaceInfo = this.GetRemovingItemReplaceInfo(visibleIndex);
        
        ASPx.RemoveElement(item);
        for(var i = 0; i < replaceInfo.length; i++) {
            var info = replaceInfo[i];
            ASPx.RemoveElement(info.element);
            if(info.insertAfter)
                ASPx.InsertElementAfter(info.element, info.refElement);
            else
                info.refElement.parentNode.insertBefore(info.element, info.refElement);
        }

        var mainTable = this.grid.GetMainTable();
        var lastRow = mainTable.rows[this.IsNewItemOnTop() ? 0 : mainTable.rows.length - 1];
        var lastSeparator = mainTable.rows[this.IsNewItemOnTop() ? 1 : mainTable.rows.length - 2];
        var dataCards = [ ];
        ASPx.GetNodesByPartialId(lastRow, this.GetDataItemIDPrefix(), dataCards)
        if(dataCards.length === 0) {
            ASPx.RemoveElement(lastRow);
            ASPx.RemoveElement(lastSeparator);
        }
    },

    GetRemovingItemReplaceInfo: function(visibleIndex) {
        var indices = this.insertedItemIndices.slice(this.insertedItemIndices.indexOf(visibleIndex));

        var cardsToMove = [ ];
        for(var i = 0; i < indices.length; i++)
            cardsToMove.push(this.GetDataItem(indices[i]));
        cardsToMove.push(this.CreateEmptyCard()); // to replace last card

        var info = [ ];
        for(var i = 1; i < cardsToMove.length; i++) {
            var prevItem = cardsToMove[i - 1];
            var currentItem = cardsToMove[i];

            var prevItemIndex = prevItem.cellIndex;
            var isLast = prevItemIndex === prevItem.parentNode.cells.length - 1;
            var refElementIndex = prevItemIndex + (isLast ? -1 : 1);
            var refElement = prevItem.parentNode.cells[refElementIndex];

            info.push({ element: currentItem, refElement: refElement, insertAfter: isLast });
        }
        return info;
    },

    CreateEmptyCard: function() {
        var etalonItem = this.GetEtalonNewItem();
        var etalonRow = etalonItem.parentNode;
        var emptyCards = ASPx.GetNodesByPartialClassName(etalonRow, this.EmptyHiddenCardClassName);
        return emptyCards.length > 0 && emptyCards[0].cloneNode(true);
    },

    GetColumnIndex: function(dataCell) {
        if(!dataCell) return -1;
        if(!ASPx.IsExists(dataCell.dxgvColumnIndex)) {
            var visibleIndex = this.GetDataItemIndex(this.GetDataItemByChild(dataCell));
            this.EnsureDataItemClientInfo(visibleIndex);
        }
        return dataCell.dxgvColumnIndex;
    },
    EnsureDataItemClientInfo: function(visibleIndex) {
        var layoutControl = this.GetLayoutControl(visibleIndex);
        if(layoutControl && layoutControl.GetMainElement())
            this.EnsureLayoutControlItems(layoutControl);
    },
    EnsureLayoutControlItems: function(layoutControl) {
        if(layoutControl.GetMainElement().dxgvItemsProcessed) 
            return;
        var itemPaths = this.GetLayoutItemPaths();
        for(var columnIndex in itemPaths) {
            var item = layoutControl.GetItemByPath(itemPaths[columnIndex]);
            var itemElement = layoutControl.GetHTMLElementByItem(item);
            if(item && itemElement) {
                itemElement.layoutItem = item;
                itemElement.dxgvColumnIndex = parseInt(columnIndex, 10);
            }
        }
        layoutControl.GetMainElement().dxgvItemsProcessed = true;
    },

    GetDataItemByChild: function(element) { return ASPx.GetParent(element, this.IsDataItem.aspxBind(this)); },

    GetActualDataCell: function(visibleIndex, columnIndex) { 
        this.EnsureDataItemClientInfo(visibleIndex);

        var dataItem = this.GetActualDataItem(visibleIndex, columnIndex);
        var containerCell = this.GetDataCell(dataItem, columnIndex); 
        var layoutItem = containerCell.layoutItem;
        return layoutItem && layoutItem.formLayout.GetNestedControlCell(layoutItem); // NestedControlCell
    },
    

    GetDataCell: function(dataItem, columnIndex) { 
        var itemPath = this.GetLayoutItemPaths()[columnIndex];
        var layoutControl = this.GetLayoutControlByDataItem(dataItem);
        if(!layoutControl || !layoutControl.GetMainElement() || !ASPx.IsExists(itemPath))
            return;
        var item = layoutControl.GetItemByPath(itemPath);
        return layoutControl.GetHTMLElementByItem(item);
    },
    GetDataCellByChild: function(dataItem, element) { 
        var cell = ASPx.GetParentByPartialClassName(element, ASPx.FormLayoutConsts.GROUP_CELL_PARTIAL_CLASS_NAME);
        if(cell && cell.tagName === "DIV") // Flow mode
            cell = cell.parentNode;
        return cell;
    },
    GetLayoutControlByDataItem: function(dataItem) { 
        if(!dataItem) return;
        var visibleIndex = this.GetDataItemIndex(dataItem);
        return this.GetLayoutControl(visibleIndex);
    },

    ChangeDeletedItemVisibility: function(visibleIndex, show) {
        var layoutControl = this.GetLayoutControl(visibleIndex);
        if(!layoutControl) 
            return;
        layoutControl.SetVisible(show);

        var deleteMessageContainer = this.GetDataItemDeleteMessageContainer(visibleIndex);
        ASPx.SetElementDisplay(deleteMessageContainer, !show);

        // TODO move to css
        var dataItem = this.GetDataItem(visibleIndex);
        dataItem.style.opacity = show ? 1 : 0.3;
        dataItem.style.textAlign = "center";
        dataItem.style.verticalAlign = "middle";
    },
    GetDataItemDeleteMessageContainer: function(visibleIndex) {
        var dataItem = this.GetDataItem(visibleIndex);
        if(!dataItem.delMessageContainer) {
            var container = document.createElement("SPAN");
            container.innerHTML = "DELETED";
            dataItem.appendChild(container);
            dataItem.delMessageContainer = container;
        }
        return dataItem.delMessageContainer;
    },






    InsertItemValidationError: function(dataItem, itemError, errorText) {
        itemError.innerHTML = errorText;
        var layoutControl = this.GetLayoutControlByDataItem(dataItem);
        ASPx.InsertElementAfter(itemError, layoutControl.GetMainElement());
    },

    ChangeCellStyleCore: function(cell, visibleIndex, columnIndex, styleType) {
        this.constructor.prototype.ChangeCellStyleCore.call(this, cell, visibleIndex, columnIndex, styleType);

        this.ChangeFormLayoutItemStyle(cell, styleType);
    },

});

var GridViewBatchEditUpdateWatcherHelper = ASPx.CreateClass(ASPx.UpdateWatcherHelper, {
    GridFuncCallbackWaitingTimeout: 15,
    constructor: function(owner) {
        this.grid = owner.grid;
        this.constructor.prototype.constructor.call(this, owner);
    },
    GetName: function() {
        return this.grid.name;
    },
    GetControlMainElement: function() {
        return this.grid.GetMainElement();
    },
    CanShowConfirm: function(requestOwnerID) {
        return !this.grid.RaiseBatchEditConfirmShowing(requestOwnerID);
    },
    HasChanges: function() {
        return this.owner.HasChanges() && this.grid.GetMainElement() && !this.owner.IsUpdating();
    },
    GetConfirmUpdateText: function() {
        return this.grid.batchEditClientState.confirmUpdate;
    },
    NeedConfirmOnCallback: function(dxCallbackOwner) {
        var updateOnCallback = ASPx.GetIsParent(dxCallbackOwner.GetMainElement(), this.GetControlMainElement());
        return updateOnCallback && !this.IsGridFuncCallback(dxCallbackOwner);
    },
    ResetClientChanges: function() {
        this.owner.CancelEdit();
        this.grid.UpdateStateHiddenField();
    },

    IsGridFuncCallback: function(dxCallbackOwner) {
        if(dxCallbackOwner !== this.grid)
            return false;
        var date = new Date();
        for(var i = 0; i < this.grid.funcCallbacks.length; i++) {
            var callbackItem = this.grid.funcCallbacks[i];
            if(callbackItem && (date - callbackItem.date) < this.GridFuncCallbackWaitingTimeout)
                return true;
        }
        return false;
    }
});

ASPx.GridViewBatchEditHelper = GridViewBatchEditHelper;
ASPx.CardViewBatchEditHelper = CardViewBatchEditHelper;
    
})();