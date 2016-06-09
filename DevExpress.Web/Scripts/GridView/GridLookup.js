/// <reference path="..\_references.js"/>

(function() {
var GLPCallbackCommand = {
    CancelChanges       : "GLP_CC",
    ApplyChanges        : "GLP_AC",
    ApplyInputChanges   : "GLP_AIC",
    Filter              : "GLP_F"
};
var ASPxClientGridLookup = ASPx.CreateClass(ASPxClientDropDownEditBase, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        
        this.gridView = null;
        this.gridViewWrapper = null;
        this.allowFocusedRow = false;
        this.allowSelectSingleRowOnly = false;
        this.incrementalFilteringMode = "Contains";
        this.keyboardHelper = null;
        this.fieldNames = null;
        this.inputLockCount = 0;
        this.inputEventLockCount = 0;
        this.gridSetValueLockCount = 0;
        this.keyFieldName = "";
        this.multiTextSeparator = ";";
        this.selectionChangedViaAPI = false;
        this.isAccessibilityComplianceEnabled = false;
        this.applyCallbackResult = false;
        this.filterTimer = 500;
        this.adroidSamsungBugTimeout = 70; //T170541
        this.lockSelectionChanged = false;
        this.gridViewSelected = false;    
        this.RowClick  = new ASPxClientEvent();
    },
    Initialize: function() {
        ASPxClientDropDownEditBase.prototype.Initialize.call(this);
        this.InitializeGridViewInstance();
    },
    InitializeEvents: function() {
        ASPxClientDropDownEditBase.prototype.InitializeEvents.call(this);
        ASPx.Evt.AttachEventToElement(this.GetInputElement(), "paste", this.OnKeyUp.aspxBind(this));
    },
    AfterInitialize: function(){
        this.CreateStrategiesAndHelpers();
        this.constructor.prototype.AfterInitialize.call(this);
    },
    InitializeGridViewInstance: function(){
		if(!this.gridView) {
            var name = this.name + "_DDD_gv";
            this.gridView = ASPx.GetControlCollection().Get(name);
            if(this.gridView){
                this.gridView.lookupBehavior = true;
                this.allowFocusedRow = this.gridView.allowFocusedRow;
                this.allowSelectSingleRowOnly = this.gridView.allowSelectSingleRowOnly;
                this.gridViewWrapper = new ASPxClientLookupGridViewWrapper(this.gridView);
                this.InitializeColumnFieldNames(this.gridView)
                this.SubscribeToGVEvents(this.gridView);
            }
        }
    },
    InitializeColumnFieldNames: function(gridView){
        var fieldNames = [];
        var columnCount = gridView.GetColumnsCount();
        for(var i = 0; i < columnCount; i++){
            var column = gridView.GetColumn(i);
            if(column.fieldName != null)
                fieldNames.push(column.fieldName);
        }
        this.fieldNames = fieldNames.join(";");
    },
    SubscribeToGVEvents: function(grid) {
        if(grid) {
            grid.BeginCallback.InsertFirstHandler(this.CreateGridViewEventHandler("OnGridViewBeginCallback"));
            grid.EndCallback.InsertFirstHandler(this.CreateGridViewEventHandler("OnGridViewEndCallback"));
            grid.EndCallbackAnimationStart.InsertFirstHandler(this.CreateGridViewEventHandler("OnGridViewEndCallbackAnimationStart"));
            grid.RowClick.InsertFirstHandler(this.CreateGridViewEventHandler("OnGridViewRowClick"));
            grid.SelectionChanged.InsertFirstHandler(this.CreateGridViewEventHandler("OnGridViewSelectionChanged"));
            grid.FocusedRowChanged.InsertFirstHandler(this.CreateGridViewEventHandler("OnGridViewFocusedRowChanged"));
            grid.ColumnResized.InsertFirstHandler(this.CreateGridViewEventHandler("OnGridViewColumnResized"));
            grid.InternalCheckBoxClick.InsertFirstHandler(this.CreateGridViewEventHandler("OnGridViewInternalCheckBoxClick"));
        }
    },
    CreateGridViewEventHandler: function(eventHandlerName) {
        return function(sender, args) {
            var gridLookup = ASPx.GetControlCollection().Get(this.name);
	        if(gridLookup)
		        return gridLookup[eventHandlerName](sender, args);
        }.aspxBind(this);
    },
    CreateStrategiesAndHelpers: function(){
        this.CreateFilterStrategy();
        this.CreateSelectionStrategy();
        this.CreateKBHelper();
    },
    CreateFilterStrategy: function(){
        if(this.incrementalFilteringMode == "Contains")
            this.filterStrategy = new ASPxGridLookupContainsFilteringStrategy(this);
        else if(this.incrementalFilteringMode == "StartsWith")
            this.filterStrategy = new ASPxGridLookupStartsWithFilteringStrategy(this);
        else
            this.filterStrategy = new ASPxGridLookupDisabledFilteringStrategy(this);
        this.filterStrategy.Initialize();
    },
    CreateSelectionStrategy: function(){
        this.selectionStrategy = null;
        if(this.MultiSelectionPossible())
            this.selectionStrategy = new ASPxGridLookupMultipleSelectionStrategy(this);
        else
            this.selectionStrategy = new ASPxGridLookupSingleSelectionStrategy(this);
    },
    CreateKBHelper: function(){
        var gridView = this.GetGridView();
        this.keyboardHelper = new ASPxGridLookupKeyboardHelper(this, gridView, this.filterStrategy, this.selectionStrategy);
        this.keyboardHelper.Initialize();
    },
        
    RefocusOnClickRequired: function(){
        return ASPx.Browser.IE;
    },
    
    IsAndroidKeyEventsLocked: function() {
        return ASPx.Browser.AndroidMobilePlatform && this.androidKeyEventsLocked;
    },
    LockAndroidKeyEvents: function() {
        this.androidKeyEventsLocked = true;
    },
    UnlockAndroidKeyEvents: function() {
        this.androidKeyEventsLocked = false;
    },
    LockInputEvent: function(){
        this.inputEventLockCount ++;
    },
    UnlockInputEvent: function(){
        this.inputEventLockCount --;
    },
    InputEventLocked: function(){
        return this.inputEventLockCount > 0;
    },
    LockInput: function(){
        this.inputLockCount ++;
    },
    UnlockInput: function(){
        this.inputLockCount --;
    },
    InputLocked: function(){
        return this.inputLockCount > 0;
    },
    SetSelectionChangedViaAPI: function(value){
        this.selectionChangedViaAPI = value;
    },
    SelectionChangedViaAPI: function(){
        return this.selectionChangedViaAPI;
    },

    OnCancelChanges: function() {
        ASPxClientDropDownEditBase.prototype.OnCancelChanges.call(this);
        this.selectionStrategy.OnCancelChanges();
    },
    OnApplyChanges: function() {
		if(!this.readOnly){
            if(this.InCallback() || this.GetGridView().InCallback() || this.filterStrategy.IsFilterTimerActive()){
                this.applyCallbackResult = true;
                if (this.droppedDown)
                    this.HideDropDownArea(true);
            } else {
        	    ASPxClientDropDownEditBase.prototype.OnApplyChanges.call(this);
                this.selectionStrategy.OnApplyChanges();
                this.filterStrategy.OnApplyChanges();
            }
        } else {
            this.ClearFocusedRowIndexInput();
			this.RollbackSelection();
        }
    },
    ClearFocusedRowIndexInput: function() {
        if(ASPx.IsExists(this.gridView.stateObject.focusedRow))
            this.gridView.stateObject.focusedRow = "";
    },
    OnApplyChangesAndHideDropDown: function(){
        this.OnApplyChanges();
        if(this.droppedDown)
            this.HideDropDownArea(true);
    },
    CanApplyNullTextDecoration: function () {
		var nothingIsFocusedInGrid = !(this.GetGridView().GetFocusedRowIndex() + 1);
        var nothingIsSelectedInGrid = !(this.GetGridView().GetSelectedRowCount());
        return this.MultiSelectionPossible() ? nothingIsSelectedInGrid : nothingIsFocusedInGrid;
    },
    AdjustDropDownWindow: function() {
        var pc = this.GetPopupControl();
        if(pc && pc.IsWindowVisible()) 
            ASPxClientDropDownEditBase.prototype.AdjustDropDownWindow.call(this);
        else {
            this.ResetDropDownSizeCache();
        }
    },  
    AdjustPagerToFixItMinWidthBeforeGridAdjust: function() {
        var topPager = ASPx.GetControlCollection().Get(this.GetGridView().name + "_" + this.GetGridView().PagerTopID);
        if(topPager) topPager.AdjustControl();
        var bottomPager = ASPx.GetControlCollection().Get(this.GetGridView().name + "_" + this.GetGridView().PagerBottomID);
        if(bottomPager) bottomPager.AdjustControl();
    },
    ShowDropDownArea: function(isRaiseEvent){
        ASPxClientDropDownEditBase.prototype.ShowDropDownArea.call(this, isRaiseEvent);
        this.AdjustPagerToFixItMinWidthBeforeGridAdjust();
        this.GetGridView().AdjustControl();
        var instance = this;
        window.setTimeout(function(){instance.AdjustDropDownWindow();}, 0);
    },
    ClearEditorValueByClearButtonCore: function() {
        if(this.gridSetValueLockCount === 0) {
            var gridView = this.GetGridView();
            if(!gridView.InCallback()) {
                this.SetSelectionChangedViaAPI(false);
                gridView.UnselectRows();
                if(this.filterStrategy && this.droppedDown) {
                    this.GetInputElement().value = '';
                    this.filterStrategy.Filtering();
                }
            }
        } else 
            ASPxClientDropDownEditBase.prototype.ClearEditorValueByClearButtonCore.call(this);
    },
    // GridView Utils
    GetGridViewWrapper: function(){
        if(!this.gridViewWrapper)
            this.InitializeGridViewInstance();
        return this.gridViewWrapper;
    },
    // GridView API // TODO Create client wrapper and apply this code there
    OnGridViewRowClick: function(sender, args){
        var retValue = this.RaiseRowClick(sender, args);
        this.selectionStrategy.OnGridViewRowClick(args);
        return retValue;
    },
    OnGridViewFocusedRowChanged: function(s, e){
        if(!this.readOnly) 
            this.selectionStrategy.OnGridViewFocusedRowChanged(e);
    },
    OnGridViewColumnResized: function(s, e){
        this.OnGridViewSizeChanged();
    },
    OnGridViewSizeChanged: function(){
        this.AdjustDropDownWindow();
    },
    OnGridViewInternalCheckBoxClick: function(sender, args){
        this.selectionStrategy.OnGridViewInternalCheckBoxClick(args);
    },
    RaiseRowClick: function(sender, args) {
        if(!this.RowClick.IsEmpty()){
            this.RowClick.FireEvent(sender, args);
            return args.cancel;
        }
        return false; 
    },
    
    CloseDropDownByDocumentOrWindowEvent: function(causedByWindowResizing) {
        this.OnApplyChanges();
        ASPxClientDropDownEditBase.prototype.CloseDropDownByDocumentOrWindowEvent.call(this, causedByWindowResizing); 
    },
    // Keyboard support
    OnKeyDown: function(evt){
        ASPxClientDropDownEditBase.prototype.OnKeyDown.call(this, evt);
        if(this.keyboardHelper)
            this.keyboardHelper.OnKeyDown(evt, true);
    },
    OnKeyUp: function(evt){
        ASPxClientDropDownEditBase.prototype.OnKeyUp.call(this, evt);
        if(this.keyboardHelper)
            this.keyboardHelper.OnKeyUp(evt, true);
    },
    OnEscape: function() {
        ASPxClientDropDownEditBase.prototype.OnEscape.call(this);               
    },
    OnEnter: function() {
        this.OnApplyChangesAndHideDropDown();
    },
    OnTab: function() {
        if(this.isAccessibilityComplianceEnabled && this.droppedDown) //B181376
            return;
        this.OnApplyChanges();
        if(this.droppedDown)
            this.HideDropDownArea(true);
        if(ASPx.Browser.Firefox) //T164190
            var dummy = this.GetGridView().GetMainElement().offsetWidth;
    },
    OnArrowUp: function(evt) {
        ASPxClientDropDownEditBase.prototype.OnArrowUp.call(this, evt);     
        this.ForceRefocusEditor();
    },
    OnArrowDown: function(evt) {
        ASPxClientDropDownEditBase.prototype.OnArrowDown.call(this, evt);   
        this.ForceRefocusEditor();
    },
    OnHome: function() {
        var grid = this.GetGridView();
        grid.SetFocusedRowIndex(grid.GetTopVisibleIndex());       
    },
    OnEnd: function() {
        var grid = this.GetGridView();
        grid.SetFocusedRowIndex(grid.GetTopVisibleIndex() + grid.GetVisibleRowsOnPage() - 1);
    },

    ShouldCloseOnMCMouseDown: function () {
        return this.GetInputElement().readOnly;
    },
    
    MultiSelectionPossible: function(){
        return !this.allowSelectSingleRowOnly;
    },
    
    OnTextChanged: function() {
        if(!this.droppedDown)
            this.OnApplyChanges();
    },
    OnSelectionChanged: function(){
        if(!this.InputEventLocked() && !this.SelectionChangedViaAPI() && !this.lockSelectionChanged)
            this.OnValueChanged();
    },
    SetTemporaryTextOfFocusedRow: function(){ // TODO is it real temporary?
        var rowText = this.gridViewWrapper.GetFocusedRowText();
        this.SetTextWithoutApply(rowText);
    },
    LockGridSetValue: function(){
        this.gridSetValueLockCount++;
    },
    UnlockGridSetValue: function(){
        this.gridSetValueLockCount--;
    },
    SetText: function (text) {
        this.SetTextWithoutApply(text);
        this.selectionStrategy.InputChanged();
        this.OnApplyChanges();
    },
    SetTextWithoutApply: function(text) {
        this.LockGridSetValue();
        if(!this.InputLocked())
            ASPxClientDropDownEditBase.prototype.SetText.call(this, text);
        this.UnlockGridSetValue();
    },
    GetText: function() {
        var textValue = ASPxClientTextEdit.prototype.GetValue.call(this);
	    return textValue != null ? textValue : "";
    },
    GetValue: function() {
        var keyValues = [];
        var gridView = this.GetGridView();
        if(!!gridView) {
            var selectedRowTexts = ASPx.CloneObject(gridView.currentSelectionState.SelectedRowTexts);
            for(var key in selectedRowTexts)
                keyValues.push(key);
        }
        return keyValues;
    },
    SetValue: function(value){
        if(this.gridSetValueLockCount == 0){
            var gridView = this.GetGridView();
            if(!gridView.InCallback()){
                this.SetSelectionChangedViaAPI(true);
                if(ASPx.IsExists(value))
                    gridView.SelectRowsByKey(value);
                else
                    gridView.UnselectRows();
            }
        } else 
            ASPxClientDropDownEditBase.prototype.SetValue.call(this, value);
    },
    // Callback
    GetGridViewCallbackArgs: function(command, args) {
        var grid = this.GetGridView();
        var arg = (ASPx.Ident.IsArray(args)) ? grid.SerializeCallbackArgs(args) : args;
        return ASPx.FilteringUtils.FormatCallbackArg(command, arg);
    },
    SendGridViewCustomCallback: function(command, args) {
        var grid = this.GetGridView();
        var gvPerformCallbackArgs = this.GetGridViewCallbackArgs(command, args);
        grid.PerformCallback(gvPerformCallbackArgs);
    },
    OnGridViewBeginCallback: function(sender, args){
        this.GetGridView().callbackCommand = "";
        this.RaiseBeginCallback(args.command);
    },
    OnGridViewEndCallbackAnimationStart: function() {
        this.OnGridViewSizeChanged();
    },
    OnGridViewEndCallbackSizeChanged: function(){
        var gridFocused = ASPx.KbdHelper.active == this.GetGridView().kbdHelper;
        this.OnGridViewSizeChanged();
        if(ASPx.Browser.IE && gridFocused)
            this.GetGridView().kbdHelper.Focus();
    },
    OnGridViewEndCallback: function() {
        this.OnGridViewEndCallbackSizeChanged();
        
        this.EnsureFocusedRowOnEndCallback();

        this.selectionStrategy.OnGridViewEndCallback();
        this.filterStrategy.OnEndCallback(!this.applyCallbackResult);
        
        this.SetSelectionChangedViaAPI(false);
        this.RaiseEndCallback();

        if(this.applyCallbackResult){
            this.applyCallbackResult = false;
            var _this = this;
            window.setTimeout(function(){_this.OnApplyChanges();}, 0);
        }
    },
    EnsureFocusedRowOnEndCallback: function(){
        this.gridViewWrapper.FlushFocusedRowIndex();
        if(this.IsFocusChangingCallback() && this.selectionStrategy.ChangeTextOnFocusRowChangedCallbackAllowed()) {
            if(!this.readOnly)
                this.SetTemporaryTextOfFocusedRow();
        }
    },
    IsFocusChangingCallback: function(){
        var grid = this.GetGridView();
        return grid.callbackCommand == "FOCUSEDROW" || grid.callbackCommand == "PAGERONCLICK" || grid.callbackCommand == "NEXTPAGE" || grid.callbackCommand == "PREVPAGE" || grid.callbackCommand == "SELECTROWSKEY" || grid.callbackCommand == "SELECTROWS";
    },
    CallbackResultCanBeApplied: function(){
        return !this.filterStrategy.FilteringCallback();
    },
    // Selection
    RollbackSelection: function() {
		this.OnCancelChanges();
	},
	OnGridViewSelectionChanged: function(s, e){
		if(!this.readOnly) {
	        if(this.gridViewWrapper.IsSelectionChangeEventLocked())
	            return;
	        if(this.selectionStrategy)
	            this.selectionStrategy.OnGridViewSelectionChanged(e);
		}
    },
    OnGridViewTextChanged: function(newText, silent){
        if(!this.InputLocked()){
            if(silent)
                this.LockInputEvent();
            this.SetTextWithoutApply(newText);
            this.OnSelectionChanged();
            if(silent)
                this.UnlockInputEvent();
        }
        this.lockSelectionChanged = false;
    },
    GetFieldNames: function(){
        return this.fieldNames;
    },
    IsElementBelongToInputElement: function(element) {
        var isInputElement = ASPxClientDropDownEditBase.prototype.IsElementBelongToInputElement.call(this, element);
        this.gridViewSelected = !isInputElement && ASPx.GetIsParent(this.gridView.GetMainElement(), element);
        return isInputElement || this.gridViewSelected;
    },
    OnLostFocusCore: function() {
        ASPxClientDropDownEditBase.prototype.OnLostFocusCore.call(this);
        if(!this.IsFocusEventsLocked() && !this.gridViewSelected)
            this.OnApplyChanges();
        this.gridViewSelected = false;
    },
    // API
    GetGridView: function(){
        if(!this.gridView)
            this.InitializeGridViewInstance();
        return this.gridView;
    },
    ConfirmCurrentSelection: function() {
        this.OnApplyChanges();
    },
    RollbackToLastConfirmedSelection: function() {
        this.OnCancelChanges();
    }
}); 
ASPxClientGridLookup.Cast = ASPxClientControl.Cast;

//==================================== GridViewWrapper ====================================
var ASPxClientLookupGridViewWrapper = ASPx.CreateClass(null, {
    constructor: function(gridView) {
        this.gridView = gridView;
        this.selectionChangeEventLockCount = 0;

        this.FlushFocusedRowIndex();
    },
    FlushFocusedRowIndex: function(){
        this.gridView._setFocusedItemInputValue();
        this.updateFocusedRowKeyHiddenInputValue();
    },
    updateFocusedRowKeyHiddenInputValue: function() {
        this.gridView.UpdateStateObjectWithObject({ focusedKey: this.gridView.GetRowKey(this.gridView.focusedRowIndex) });
    },
    GetIsRowOnPage: function(rowVisibleIndex){
        return this.gridView.GetTopVisibleIndex() <= rowVisibleIndex && rowVisibleIndex <= this.GetBottomVisibleIndex();
    },
    GetBottomVisibleIndex: function(){
        var pageRowCount = this.gridView.GetVisibleRowsOnPage();
        var topVisibleIndex = this.gridView.GetTopVisibleIndex();
        return topVisibleIndex + pageRowCount - 1;
    },
    GetSelectedIndicesOnPage: function(){
        var selectedIndices = [];
        var topVisibleIndex = this.gridView.GetTopVisibleIndex();
        var bottomVisibleIndex = this.GetBottomVisibleIndex();
        for(var i = topVisibleIndex; i <= bottomVisibleIndex; i++) {
            if(this.gridView.IsRowSelectedOnPage(i))
                selectedIndices.push(i);
        }
        return selectedIndices;
    },
    GetVisibleRowText: function(visibleRowIndex){
        var rowIndexOnPage = this.ConvertVisibleRowIndexToRowIndexOnPage(visibleRowIndex);
        return this.GetVisibleRowTextOnPage(rowIndexOnPage);
    },
    GetVisibleRowTextOnPage: function(rowIndexOnPage){
        return this.gridView.itemTexts[rowIndexOnPage];
    },
    GetSelectedRowTexts: function(){
        return this.gridView.currentSelectionState.SelectedRowTexts;
    },
    HighlightVisibleRow: function(visibleRowIndex){
        if(this.gridView.allowFocusedRow)
            this.gridView.SetFocusedRowIndex(visibleRowIndex);
        else{
            this.LockSelectionChangeEvent();
            this.gridView.SelectRowOnPage(visibleRowIndex);
            this.UnlockSelectionChangeEvent();
        }
    },
    LockSelectionChangeEvent: function(){ this.selectionChangeEventLockCount++;},
    UnlockSelectionChangeEvent: function(){ this.selectionChangeEventLockCount--;},
    IsSelectionChangeEventLocked: function() { return this.selectionChangeEventLockCount > 0;},

    GetFocusedRowText: function(){
        var focusedRowIndex = this.GetFocusedRowIndexOnPage();
        return this.GetVisibleRowTextOnPage(focusedRowIndex);
    },
    GetFocusedRowIndexOnPage: function(){
        var visibleIndex =  this.gridView.GetFocusedRowIndex();
        return this.ConvertVisibleRowIndexToRowIndexOnPage(visibleIndex);
    },
    ConvertVisibleRowIndexToRowIndexOnPage: function(visibleIndex){
        return visibleIndex - this.gridView.GetTopVisibleIndex();
    },
    ConvertRowIndexOnPageToVisibleRowIndex: function(indexOnPage){
        return indexOnPage + this.gridView.GetTopVisibleIndex();
    },
    GetPreviousSingleSelectedRowIndexOnPage: function(){ // TODO Find the way to do that without interferring with the grid's guts
        if(ASPx.IsExists(this.gridView.stateObject.selection)) {
	        var checkList = this.gridView.stateObject.selection;
	        return checkList.indexOf("T");
        }
        return -1;
    },
	EnsureFocusedRadioButtonChecked: function(){
        var focusedRowVisibleIndex = this.gridView.GetFocusedRowIndex();
        this.gridView._selectAllSelBtn(false);
        var rowCheckBox = this.gridView.GetDataRowSelBtn(focusedRowVisibleIndex);
        if (rowCheckBox != null){
            var internalCheckBox = this.gridView.internalCheckBoxCollection.Get(rowCheckBox.id);
            internalCheckBox.SetValue(ASPx.CheckBoxInputKey.Checked);
        }
	}
});

//==================================== KBS ====================================
var GridLookupKeyboardSupportHelper =  ASPx.CreateClass(ASPx.GridViewKbdHelper, {
    constructor: function(gridView) {
        this.gridView = gridView;
        this.keyboardHelper = null;
        ASPx.GridViewKbdHelper.prototype.constructor.call(this, gridView);
    },
    SetGridLookupKBHelper: function(keyboardHelper) {
        this.keyboardHelper = keyboardHelper;
    },
    SetGridLookup: function(gridLookup){},
    // General handlers
    HandleKeyDown: function(evt) {
        if(this.keyboardHelper)
            return this.keyboardHelper.OnKeyDown(evt, false);
		return false;
    },
    HandleKeyPress: function(evt) {
        if(this.keyboardHelper)
            return this.keyboardHelper.OnKeyPress(evt, false);
        return false;
    },
    HandleKeyUp: function(evt) {
        if(this.keyboardHelper)
            return this.keyboardHelper.OnKeyUp(evt, false);
		return false;
    },
    HandleKeyDownCore: function(evt){
        return ASPx.GridViewKbdHelper.prototype.HandleKeyDown.call(this, evt);
    },
    HandleKeyPressCore: function(evt){
        return ASPx.GridViewKbdHelper.prototype.HandleKeyPress.call(this, evt);
    },
    HandleKeyUpCore: function(evt){
        return ASPx.GridViewKbdHelper.prototype.HandleKeyUp.call(this, evt);
    }
});

var ASPxGridLookupKeyboardHelper = ASPx.CreateClass(null, {
    constructor: function(gridLookup, gridView, filterStrategy, selectionStrategy) {
        this.gridLookup = gridLookup;
        this.filterStrategy = filterStrategy;
        this.selectionStrategy = selectionStrategy;
        this.gridView = gridView;
        this.gridViewKbdHelper = null;
    },
    Initialize: function(){
        if(this.gridLookup.enabled) {
        	this.gridViewKbdHelper = this.gridView.kbdHelper;
        	this.gridViewKbdHelper.SetGridLookupKBHelper(this);
        	this.gridViewKbdHelper.SetGridLookup(this.gridLookup);
		}
    },
    
    IsKeyAllowedForGridView: function(evt, fromLookup) {
        var keyCode = ASPx.Evt.GetKeyCode(evt); 
        if((keyCode == ASPx.Key.Right || keyCode == ASPx.Key.Left) && fromLookup)
            return false;
        if(evt.altKey && (keyCode == ASPx.Key.Up || keyCode == ASPx.Key.Down))
            return false;
        var onlyFilteringKey = fromLookup && ASPx.FilteringUtils.EventKeyCodeChangesTheInput(evt);
        return !onlyFilteringKey;
    },
    IsKeyAllowedForLookup: function(evt, fromLookup) {
        var keyCode = ASPx.Evt.GetKeyCode(evt); 
        if(evt.shiftKey && (keyCode == ASPx.Key.Home || keyCode == ASPx.Key.End))
            return !fromLookup;
        if(evt.shiftKey && (keyCode == ASPx.Key.PageUp || keyCode == ASPx.Key.PageDown || keyCode == ASPx.Key.Home || keyCode == ASPx.Key.End))
            return true;
        if(evt.altKey && (keyCode == ASPx.Key.Up || keyCode == ASPx.Key.Down))
            return true;
        var canHarmInput = !fromLookup && ASPx.FilteringUtils.EventKeyCodeChangesTheInput(evt);
        return !canHarmInput;
    },
    OnGLPKeyDown: function(evt) {
        if(this.gridLookup.IsAndroidKeyEventsLocked())
            return ASPx.Evt.PreventEventAndBubble(evt);
        this.filterStrategy.OnKeyDown(evt);
        this.OnGLPSpecialKeyDown(evt);
    },
    OnGLPSpecialKeyDown: function(evt){
        var keyCode = ASPx.Evt.GetKeyCode(evt);   
        switch(keyCode) {
            case ASPx.Key.Esc:
                this.gridLookup.OnEscape();
                break;
            case ASPx.Key.Enter:
                this.gridLookup.OnEnter();
                break;
            case ASPx.Key.Tab:
                this.gridLookup.OnTab();
                break;
            case ASPx.Key.Home:
                if(evt.shiftKey)
                    this.gridLookup.OnHome();
                break;
            case ASPx.Key.End:
                if(evt.shiftKey)
                    this.gridLookup.OnEnd();
                break;
            case ASPx.Key.Up:
                if(evt.altKey)
                    this.gridLookup.OnArrowUp(evt);
                else 
                    ASPx.Evt.PreventEvent(evt);
                break;
            case ASPx.Key.Down:
                if(evt.altKey)
                    this.gridLookup.OnArrowDown(evt);
                else 
                    ASPx.Evt.PreventEvent(evt);
                break;
        }
    },
    OnGLPKeyUp:function(evt) {
        if(this.gridLookup && this.filterStrategy) {
            if(this.gridLookup.IsAndroidKeyEventsLocked())
                return ASPx.Evt.PreventEventAndBubble(evt);
            this.filterStrategy.OnKeyUp(evt);
        }
    },
    // General handlers
    OnKeyDown: function(evt, fromLookup) {
		var result = false;
		this.selectionStrategy.OnKeyDown(evt);
        if(this.IsKeyAllowedForGridView(evt, fromLookup))
            result = this.gridViewKbdHelper.HandleKeyDownCore(evt);
        if(this.IsKeyAllowedForLookup(evt, fromLookup))
            this.OnGLPKeyDown(evt);
		return result;
    },
    OnKeyPress: function(evt, fromLookup) {
        if(this.IsKeyAllowedForGridView(evt, fromLookup))
            return this.gridViewKbdHelper.HandleKeyPressCore(evt);
		return false;
    },
    OnKeyUp: function(evt, fromLookup) {
		var result = false;
        if(this.IsKeyAllowedForGridView(evt, fromLookup))
            result = this.gridViewKbdHelper.HandleKeyUpCore(evt);
        if(this.IsKeyAllowedForLookup(evt, fromLookup))
            this.OnGLPKeyUp(evt);
		return result;
    },
    // Filtering
    FilteringByTimer: function(){
        if(this.filterStrategy)
            this.filterStrategy.Filtering();
    }
});

//==================================== F I L T E R I N G ====================================
var ASPxGridLookupDisabledFilteringStrategy = ASPx.CreateClass(null, {
    Initialize: function(){},
    OnEndCallback: function(allowTextHighlighting){},
    OnKeyDown: function(evt){},
    OnKeyUp: function(evt){},
    OnApplyChanges: function(evt){},
    Filtering: function(){},
    RestoreFocusAterCallbackAllowed: function(){},
    FilteringCallback: function(){ return false; },
    IsFilterTimerActive: function() { return false;}
});
var ASPxGridLookupEnabledFilteringStrategyBase = ASPx.CreateClass(ASPxGridLookupDisabledFilteringStrategy, {
    constructor: function(gridLookup) {
        this.gridLookup = gridLookup;
        this.gridViewWrapper = gridLookup.gridViewWrapper;
        this.filterTimer = gridLookup.filterTimer;
        this.filterTimerId = -1;
        this.filteringCallback = false;
        this.refiltrationRequired = false;
        this.filterInProcess;
        this.filterTyped;
    },
    OnKeyDown: function(evt){
        if(!ASPx.FilteringUtils.EventKeyCodeChangesTheInput(evt))
            return;
        setTimeout(function() { this.filterTyped = this.GetInputElement().value; }.aspxBind(this), 0);
    },
    FilteringCallback: function(){ 
        return this.filteringCallback; 
    },
    RestoreFocusAterCallbackAllowed: function(){
        return !this.FilteringCallback();
    },
    IsDefualtRowHighlitingEnabled: function() {
        return this.GetInputElement().value != "";
    },
    GetInputElement: function() {
        return this.gridLookup.GetInputElement();
    },
    EnsureShowDropDownArea: function(){
        if(!this.gridLookup.droppedDown)
            this.gridLookup.ShowDropDownArea(true);
    },
    
    Filtering: function(){
        this.FilterStopTimer();
        if(this.gridLookup.focused)
            this.EnsureShowDropDownArea();
        
        var currentFilter = this.GetInputElement().value;
        if(this.filterInProcess !== currentFilter){
            this.filterInProcess = currentFilter;
            this.FilteringCore(this.filterInProcess);
        }
    },
    FilteringCore: function(filter){
        this.PerformFilteringCallback(filter);
    },
    PerformFilteringCallback: function(args){
        if(this.FilteringCallback())
            this.refiltrationRequired = true;
        else {
            this.gridLookup.LockInput();
            this.refiltrationRequired = false;
            this.filteringCallback = true;
            this.gridLookup.SendGridViewCustomCallback(GLPCallbackCommand.Filter, args);
        }
    },
    OnEndCallback: function(allowTextHighlighting){
        this.FilterStopTimer();
        if(this.filteringCallback){
            this.EnsureRefiltrationRequiredIsCorrect();
            if(this.refiltrationRequired){
                this.refiltrationRequired = false;
                this.FinalizeFilteringCallback();
                
				this.filterInProcess = this.filterTyped;
                window.setTimeout(function() { this.FilteringCore(this.filterInProcess); }.aspxBind(this), 0);
            } else
                this.OnEndFilteringCallbackCore(allowTextHighlighting);
        } else
            this.ResetFilter();
    },
    EnsureRefiltrationRequiredIsCorrect: function(){
        this.refiltrationRequired = this.refiltrationRequired || this.filterTyped !== this.filterInProcess;
    },
    OnEndFilteringCallbackCore: function(){
        this.FinalizeFilteringCallback();
    },
    FinalizeFilteringCallback: function(){
        this.gridLookup.UnlockInput();
        this.filteringCallback = false;
    },
    OnApplyChanges: function(){
        this.FilterStopTimer();
        this.ResetFilter();
    },
    ResetFilter: function() {
        this.filterInProcess = null;
        this.filterTyped = null;
    },

    //Filter timer
    IsFilterTimerActive: function() {
        return (this.filterTimerId != -1);
    },
    FilterStopTimer: function() {
        this.filterTimerId = ASPx.Timer.ClearTimer(this.filterTimerId);
    },
    FilterStartTimer: function(){
        this.isEnterLocked = true;
        this.filterTimerId = window.setTimeout(function() {
            if(this.gridLookup.keyboardHelper)
                this.gridLookup.keyboardHelper.FilteringByTimer();
        }.aspxBind(this), this.filterTimer);
    }
});
var ASPxGridLookupContainsFilteringStrategy = ASPx.CreateClass(ASPxGridLookupEnabledFilteringStrategyBase, {
    constructor: function(gridLookup) {
        this.filterChanged = false;
        this.constructor.prototype.constructor.call(this, gridLookup);
    },
    OnKeyDown: function(evt){
        ASPxGridLookupEnabledFilteringStrategyBase.prototype.OnKeyDown.call(this, evt);
        if(evt.keyCode === ASPx.Key.Tab){
            if(this.filterChanged){
                this.FilterStopTimer();
                this.Filtering();
            }
        } else if(ASPx.FilteringUtils.EventKeyCodeChangesTheInput(evt))
            this.filterChanged = true;
    },
    OnKeyUp: function(evt) {
        if(ASPx.FilteringUtils.EventKeyCodeChangesTheInput(evt)){
            this.filterChanged = true;
            this.FilterStopTimer();
            this.FilterStartTimer();
        }
    },
    OnEndFilteringCallbackCore: function(allowTextHighlighting){
        this.OnFilterCallbackHighlightAndSelect(allowTextHighlighting);
        ASPxGridLookupEnabledFilteringStrategyBase.prototype.OnEndFilteringCallbackCore.call(this);
        this.filterChanged = false;
    },
    OnFilterCallbackHighlightAndSelect: function(){
        if(!this.IsDefualtRowHighlitingEnabled()) return;
        var grid = this.gridLookup.GetGridView();
        var highlightRowIndex = grid.GetTopVisibleIndex();
        if(!grid.IsDataRow(highlightRowIndex)) 
            highlightRowIndex++;
        this.HighlightVisibleRow(highlightRowIndex);
    },
    HighlightVisibleRow: function(visibleRowIndex){
        this.gridLookup.LockInput();
        this.gridViewWrapper.HighlightVisibleRow(visibleRowIndex);
        this.gridLookup.UnlockInput();
    }
});
var ASPxGridLookupStartsWithFilteringStrategy = ASPx.CreateClass(ASPxGridLookupContainsFilteringStrategy, {
    constructor: function(gridLookup) {
        this.constructor.prototype.constructor.call(this, gridLookup);
    },
    OnKeyUp: function(evt) {
        if(ASPx.FilteringUtils.EventKeyCodeChangesTheInput(evt)){
            var input = this.GetInputElement();
            var newFilter = input.value.toLowerCase();
            
            var filterChanged = !this.filterInProcess || newFilter !== this.filterInProcess.toLowerCase();
            var filteringBackSpace = evt.keyCode == ASPx.Key.Backspace && !filterChanged;
            
            if(filterChanged || filteringBackSpace){
                this.FilterStopTimer();
                if(filteringBackSpace)
                    this.FilteringBackspace();
                else
                    this.FilterStartTimer();
            }
        }
    },
    FilteringBackspace: function(){
        var input = this.GetInputElement();
        ASPx.StartWithFilteringUtils.RollbackOneSuggestedChar(input);
        this.filterTyped = input.value;
        this.FilterStartTimer();
    },
    OnFilterCallbackHighlightAndSelect: function(allowTextHighlighting){
        if(!this.IsDefualtRowHighlitingEnabled()) return;
        var grid = this.gridLookup.GetGridView();
        var visibleRowsOnPage = grid.GetVisibleRowsOnPage();
        if(visibleRowsOnPage > 0){
            var highlightRowIndex = grid.GetTopVisibleIndex();
            if(!grid.IsDataRow(highlightRowIndex)) 
                highlightRowIndex++;
            var firstItemText = this.gridViewWrapper.GetVisibleRowText(highlightRowIndex);
            var isTextClearing = this.filterInProcess == "" && this.filterInProcess != firstItemText;
            if(!isTextClearing){
                this.HighlightVisibleRow(highlightRowIndex);
                if(allowTextHighlighting)
                    this.HighlightSuggestedText(firstItemText);
            } else 
                this.HighlightVisibleRow(-1);
        }
    },
    HighlightSuggestedText: function(suggestedText){
        var input = this.GetInputElement();
        ASPx.StartWithFilteringUtils.HighlightSuggestedText(input, suggestedText, this.gridLookup);
    }
});

//==================================== S E L E C T I O N ====================================
var ASPxGridLookupSelectionStrategyBase = ASPx.CreateClass(null, {
    constructor: function(gridLookup) {
        this.gridLookup = gridLookup;
        this.gridViewWrapper = gridLookup.gridViewWrapper;
        this.allowFocusedRow = gridLookup.allowFocusedRow;
        this.isSelectionChanged = false;
        this.lastSuccessfulInputText = gridLookup.GetGridView() ? gridLookup.gridView.currentSelectionState.InputText : "";
        this.lastSuccessfulTextValues = gridLookup.gridView ? ASPx.CloneObject(gridLookup.gridView.currentSelectionState.SelectedRowTexts) : { };
        this.lastChangeWasInInput = false;
    },
    RestoreFocusAterCallbackAllowed: function(){ return true; },
    ChangeTextOnFocusRowChangedCallbackAllowed: function(){ return true; },
    OnGridViewRowClick: function(args){ },
    OnGridViewFocusedRowChanged: function(e){ },
    OnGridViewInternalCheckBoxClick: function(args){ },
    OnGridViewSelectionChanged: function(e){
        var grid = this.gridLookup.GetGridView();
        var selectedRowIndex = ASPx.IsExists(e.visibleIndex) ? e.visibleIndex : grid.GetFocusedRowIndex();
        
        if(e.isSelected === true)
            this.OnGridViewRowSelected(selectedRowIndex, e.isAllRecordsOnPage);
        else if(e.isSelected === false)
            this.OnGridViewRowUnselected(selectedRowIndex, e.isAllRecordsOnPage);
    },
    OnGridViewRowSelected: function (selectedRowIndex, isAllRecordsOnPage) {
        this.SelectRowInLookup(selectedRowIndex, isAllRecordsOnPage);
        this.GridSelectionChanged();
    },
    OnGridViewRowUnselected: function(selectedRowIndex, isAllRecordsOnPage){
        this.GridSelectionChanged();
    },
    
    ShouldApplyAfterCallback: function(callbackCommand){
        return this.gridLookup.CallbackResultCanBeApplied() && 
            (callbackCommand == GLPCallbackCommand.ApplyChanges || callbackCommand == GLPCallbackCommand.ApplyInputChanges || 
            callbackCommand == "CUSTOMCALLBACK" || callbackCommand == "SELECTROWSKEY" || callbackCommand == "SELECTROWS");
    },
    CallbackChangedGrid: function(callbackCommand){
        return callbackCommand != "";
    },
    ShouldApplySilent: function(callbackCommand){
        return this.gridLookup.CallbackResultCanBeApplied() && callbackCommand == "CUSTOMCALLBACK";
    },
    OnGridViewEndCallback: function() {
        var grid = this.gridLookup.GetGridView();
        if(!this.gridLookup.readOnly && this.CallbackChangedGrid(grid.callbackCommand)){
            if(this.ShouldApplyAfterCallback(grid.callbackCommand))
                this.OnChangesApplied(this.ShouldApplySilent(grid.callbackCommand));
            else if(grid.callbackCommand != GLPCallbackCommand.CancelChanges) 
                this.GridSelectionChanged();
        }
    },
    OnKeyDown: function(evt) {
        if(ASPx.FilteringUtils.EventKeyCodeChangesTheInput(evt))
            this.InputChanged();
    },
    
    InputChanged: function(){
        this.SelectionChanged();
        this.lastChangeWasInInput = true;
    },
    GridSelectionChanged: function(){
        this.lastChangeWasInInput = false;
        this.SelectionChanged();
    },
    SelectionChanged: function() {
        this.isSelectionChanged = true;
    },
    ResetSelectionChanged: function(){
        this.lastChangeWasInInput = false;
        this.isSelectionChanged = false;
    },
    
    SelectRowInLookup: function (visibleRowIndex, isAllRecordsOnPage) {
    },
    OnApplyChanges: function(){
        if(this.isSelectionChanged){
            var applyCallbackParams = this.GetApplyCallbackParams();
            this.gridLookup.SendGridViewCustomCallback(applyCallbackParams.Command, applyCallbackParams.args);
            this.ResetSelectionChanged();
        }
    },
    OnCancelChanges: function(){
        this.RestoreLastSuccessfulControlState();
    },
    GetApplyCallbackParams: function(){
        if(this.lastChangeWasInInput)
            return this.GetApplyInputCallbackParams();
        return this.GetApplyGridSelectionCallbackParams();
    },
    GetApplyInputCallbackParams: function(){
        return { Command: GLPCallbackCommand.ApplyInputChanges, args: "" };
    },
    GetApplyGridSelectionCallbackParams: function(){
        var grid = this.gridLookup.GetGridView();
        var appliedRowIndex = grid.GetFocusedRowIndex();
        var appliedRowKey = grid.GetRowKey(appliedRowIndex);
        if(appliedRowKey === null)
            appliedRowKey = "";
        
        return { Command: GLPCallbackCommand.ApplyChanges, args: appliedRowKey };
    },
    RestoreLastSuccessfulControlState: function() {
        if(!this.isSelectionChanged) return;
        this.ResetSelectionChanged();
        this.gridLookup.SetTextWithoutApply(this.lastSuccessfulInputText);
        this.gridLookup.SendGridViewCustomCallback(GLPCallbackCommand.CancelChanges, this.GetLastSuccessfulKeyValues());
    },
    GetLastSuccessfulKeyValues: function() {
        var lastSuccessfulKeyValues = [];
        for(var key in this.lastSuccessfulTextValues)
            lastSuccessfulKeyValues.push(key);
        return lastSuccessfulKeyValues;
    },
    OnChangesApplied: function(silent) {
        var grid = this.gridLookup.GetGridView();
        var selectionChanged = this.lastSuccessfulInputText !== grid.currentSelectionState.InputText
            || JSON.stringify(this.lastSuccessfulTextValues) !== JSON.stringify(grid.currentSelectionState.SelectedRowTexts);
        this.gridLookup.lockSelectionChanged = !selectionChanged;
        this.lastSuccessfulInputText = grid.currentSelectionState.InputText;
        this.lastSuccessfulTextValues = ASPx.CloneObject(grid.currentSelectionState.SelectedRowTexts);
        this.gridLookup.OnGridViewTextChanged(this.lastSuccessfulInputText, silent);
    }
});
var ASPxGridLookupSingleSelectionStrategy = ASPx.CreateClass(ASPxGridLookupSelectionStrategyBase, {
    constructor: function(gridLookup) {
        this.constructor.prototype.constructor.call(this, gridLookup);
        this.nextSelectionOrFocusingRequiresApplying = false; // rename ...Selection or Focus...
		this.EnsureFocusedRadioButtonChecked();
    },
    OnGridViewInternalCheckBoxClick: function(args){ 
        args.cancel = true;
        var grid = this.gridLookup.GetGridView();
        var rowAlreadySelected = args.visibleIndex == grid.GetFocusedRowIndex();
        grid.SetFocusedRowIndex(rowAlreadySelected ? -1 : args.visibleIndex);
        var instance = this;
	    window.setTimeout(function(){instance.OnGridViewSelectedChangedCore();}, 0);
    },
    SelectRowInLookup: function(visibleRowIndex){
        var grid = this.gridLookup.GetGridView();
        if(grid.IsDataRow(visibleRowIndex) && this.gridViewWrapper.GetIsRowOnPage(visibleRowIndex)) {
            var pageVisibleRowIndex = this.gridViewWrapper.ConvertVisibleRowIndexToRowIndexOnPage(visibleRowIndex);
            var newText = this.gridViewWrapper.GetVisibleRowTextOnPage(pageVisibleRowIndex);
            this.gridLookup.OnGridViewTextChanged(newText, true);
        }
    },
    IsRadioButtonEvt: function(args) {
        var grid = this.gridLookup.GetGridView();
        var radioButtonInputElement = grid.GetDataRowSelBtn(args.visibleIndex);
        if(radioButtonInputElement) {
            var radioButton = grid.internalCheckBoxCollection.Get(radioButtonInputElement.id);
            return ASPx.Evt.GetEventSource(args.htmlEvent) == radioButton.mainElement;
        }
        return false;
    },
    OnGridViewRowClick: function(args){
        if(this.IsRadioButtonEvt(args))
            return;
        ASPxGridLookupSelectionStrategyBase.prototype.OnGridViewRowClick.call(this, args);
        var grid = this.gridLookup.GetGridView();
        var rowAlreadySelected = args.visibleIndex == grid.GetFocusedRowIndex();
        if(rowAlreadySelected)
            this.OnGridViewSelectedChangedCore();
        else if(grid.IsDataRow(args.visibleIndex))
            this.nextSelectionOrFocusingRequiresApplying = true;
    },
    OnGridViewRowSelected: function(selectedRowIndex){ // TODO rename selectedRowIndex->rowVisibleIndex
        ASPxGridLookupSelectionStrategyBase.prototype.OnGridViewRowSelected.call(this, selectedRowIndex);
        if(this.nextSelectionOrFocusingRequiresApplying)
            this.OnGridViewSelectedChangedCore();
    },
    OnGridViewFocusedRowChanged: function(e){
        ASPxGridLookupSelectionStrategyBase.prototype.OnGridViewFocusedRowChanged.call(this, e);
        this.EnsureFocusedRadioButtonChecked();
        var grid = this.gridLookup.GetGridView();
        var focusedRowVisibleIndex = grid.GetFocusedRowIndex();
        var dataRow = grid.IsDataRow(focusedRowVisibleIndex);
        var newText = dataRow ? this.gridViewWrapper.GetFocusedRowText() : "";
        this.gridLookup.OnGridViewTextChanged(newText, true);
        this.GridSelectionChanged();
        if(this.nextSelectionOrFocusingRequiresApplying)
            this.OnGridViewSelectedChangedCore();
    },
    OnGridViewSelectedChangedCore: function(){
        this.nextSelectionOrFocusingRequiresApplying = false;
        this.gridLookup.OnApplyChangesAndHideDropDown();
    },
    OnGridViewEndCallback: function() {
        ASPxGridLookupSelectionStrategyBase.prototype.OnGridViewEndCallback.call(this);
        var instance = this;
        window.setTimeout(function(){
            instance.EnsureFocusedRadioButtonChecked();
        },0);
    },
    EnsureFocusedRadioButtonChecked: function() {
		this.gridViewWrapper.EnsureFocusedRadioButtonChecked();
    }
});
var ASPxGridLookupMultipleSelectionStrategy = ASPx.CreateClass(ASPxGridLookupSelectionStrategyBase, {
    constructor: function(gridLookup) {
        this.constructor.prototype.constructor.call(this, gridLookup);
    },
    OnGridViewRowUnselected: function(selectedRowIndex, isAllRecordsOnPage){
        ASPxGridLookupSelectionStrategyBase.prototype.OnGridViewRowUnselected.call(this, selectedRowIndex, isAllRecordsOnPage);
        this.UnselectRowInLookup(selectedRowIndex, isAllRecordsOnPage);
    },
    OnGridViewFocusedRowChanged: function(e){
        ASPxGridLookupSelectionStrategyBase.prototype.OnGridViewFocusedRowChanged.call(this, e);
        var grid = this.gridLookup.GetGridView();
        ASPx.SetFocus(grid.GetMainElement());
    },
    SelectRowInLookup: function (visibleRowIndex, isAllRecordsOnPage) {
        if(isAllRecordsOnPage){
            var grid = this.gridLookup.GetGridView();
            var bottomVisibleIndex =  this.gridViewWrapper.GetBottomVisibleIndex();
            for (var i = grid.GetTopVisibleIndex(); i <= bottomVisibleIndex; i++)
                this.SelectRowInLookupCore(i);
        } else 
            this.SelectRowInLookupCore(visibleRowIndex);
        var newText = this.GetSelectedRowsText();
        this.gridLookup.OnGridViewTextChanged(newText, true);
    },
    SelectRowInLookupCore: function (visibleRowIndex) {
        var grid = this.gridLookup.GetGridView();
        if (grid.IsDataRow(visibleRowIndex) && this.gridViewWrapper.GetIsRowOnPage(visibleRowIndex)) {
            var pageVisibleRowIndex = this.gridViewWrapper.ConvertVisibleRowIndexToRowIndexOnPage(visibleRowIndex);
            var selectedRowText = this.gridViewWrapper.GetVisibleRowTextOnPage(pageVisibleRowIndex)
            var selectedRowTexts = this.gridViewWrapper.GetSelectedRowTexts();
            var rowKeyValue = grid.GetRowKey(visibleRowIndex);
            selectedRowTexts[rowKeyValue] = selectedRowText;
        }
    },
    UnselectRowInLookup: function(visibleRowIndex, isAllRecordsOnPage){
        var grid = this.gridLookup.GetGridView();
        if(isAllRecordsOnPage){
            var topVisibleIndex = grid.GetTopVisibleIndex();
            var bottomVisibleIndex = this.gridViewWrapper.GetBottomVisibleIndex();
            for(var i = topVisibleIndex; i <= bottomVisibleIndex; i++)
                this.UnselectRowInLookupCore(i);
        } else
            this.UnselectRowInLookupCore(visibleRowIndex);
        
        var newText = this.GetSelectedRowsText();
        this.gridLookup.OnGridViewTextChanged(newText, true);
    },
    UnselectRowInLookupCore: function(visibleRowIndex){
        var grid = this.gridLookup.GetGridView();
        if(grid.IsDataRow(visibleRowIndex) && this.gridViewWrapper.GetIsRowOnPage(visibleRowIndex)){
            var rowKeyValue = grid.GetRowKey(visibleRowIndex);
            var selectedRowTexts = this.gridViewWrapper.GetSelectedRowTexts();
            delete selectedRowTexts[rowKeyValue];
        }
    },
    GetSelectedRowsText: function(){
        var sb = [];
        var selectedRowTexts = this.gridViewWrapper.GetSelectedRowTexts();
        for(var key in selectedRowTexts)
            sb.push(selectedRowTexts[key]);
        return sb.join(this.gridLookup.multiTextSeparator);
    },
    RestoreFocusAterCallbackAllowed: function(){
        return false;
    },
    ChangeTextOnFocusRowChangedCallbackAllowed: function(){
        return false;
    }
});

window.ASPxClientGridLookup = ASPxClientGridLookup;

ASPx.GridLookupKeyboardSupportHelper = GridLookupKeyboardSupportHelper;
})();