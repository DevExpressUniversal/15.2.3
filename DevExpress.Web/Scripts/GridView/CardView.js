/// <reference path="..\_references.js"/>
(function(){
var ASPxClientCardView = ASPx.CreateClass(ASPxClientGridBase, {
    CardID: "DXDataCard",
    EmptyCardID: "DXEmptyCard",
    CardLayoutID: "DXCardLayout",
    EndlessPagingMoreButtonContainer: "EPMBC",
    EndlessPagingMoreButtonDivID: "EPMBD",
    DefaultEndlessPagingScrollDelta: 70,

    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.BatchEditCardValidating = new ASPxClientEvent();
        this.CardClick = new ASPxClientEvent();
        this.CardDblClick = new ASPxClientEvent();
        this.FocusedCardChanged = new ASPxClientEvent();
    },

    CreateBatchEditApi: function() { return new ASPxClientCardViewBatchEditApi(this); },

    Initialize: function() {
        ASPxClientGridBase.prototype.Initialize.call(this);
        ASPx.Evt.AttachEventToElement(window, "scroll", function() { this.CheckEndlessPagingLoadNextPage(); }.aspxBind(this));

        this.CheckEndlessPagingLoadNextPage();
    },
    IsOnClickEndlessPagingMode: function() { return this.useEndlessPaging && this.endlessPagingMode === 0; },
    IsFlowLayoutMode: function() { return this.layoutMode === 1; },

    GetCardLayoutControl: function(visibleIndex) { return ASPx.GetControlCollection().Get(this.name + "_" + this.CardLayoutID + visibleIndex); },
    GetItem: function(visibleIndex){
        return this.GetChildElement(this.CardID + visibleIndex);
    },
    GetDataItemIDPrefix: function() { return this.CardID; },
    GetEmptyDataItemIDPostfix: function() { return this.EmptyCardID; },
    IsEditRowHasDisplayedDataRow: function() { return false; }, // TODO
    GetCssClassNamePrefix: function() { return "dxcv"; },
    GetDataRowSelBtn: function(index) { return this.GetChildElement(this.CardLayoutID + index + "_DXSelBtn" + index); },
    //GetEditingRow: function(obj) { return ASPx.GetElementById(obj.name + "_EditingCard"); },
    getItemByHtmlEvent: function(evt) {
        return this.getItemByHtmlEventCore(evt, this.CardID);
    },
    GetEndlessPagingMoreButtonContainer: function() { return this.GetChildElement(this.EndlessPagingMoreButtonContainer); },
    GetEndlessPagingMoreButtonDiv: function() { return this.GetChildElement(this.EndlessPagingMoreButtonDivID); },
    GetScrollHelper: function() {
        if(!this.HasScrolling()) return null;
        if(!this.scrollableHelper)
            this.scrollableHelper = new CardViewTableScrollHelper(this);
        return this.scrollableHelper;
    },
    CreateEndlessPagingHelper: function() { return new ASPx.CardViewEndlessPagingHelper(this); },
    CreateBatchEditHelper: function() { return new ASPx.CardViewBatchEditHelper(this); },

    GetItemVisibleIndexRegExp: function(dataAndGroupOnly) {
        return this.GetItemVisibleIndexRegExpByIdParts([ this.CardID ]);
    },

    CreateEditingErrorItem: function() {
        var wrapper = document.createElement("div");
        wrapper.innerHTML = this.styleInfo[ASPxClientGridItemStyle.ErrorItemHtml];
        return ASPx.GetChildByTagName(wrapper, "DIV", 0);
    },

    RestoreCallbackSettings: function(){
        this.UpdateScrollableControls();
        ASPxClientGridBase.prototype.RestoreCallbackSettings.call(this);
    },

    OnBrowserWindowResize: function(e) {
        ASPxClientGridBase.prototype.OnBrowserWindowResize.call(this);
        this.CheckEndlessPagingLoadNextPage();
    },

    CheckEndlessPagingLoadNextPage: function() {
        if(!this.useEndlessPaging || this.IsOnClickEndlessPagingMode() || this.HasScrolling()) 
            return;
        if(this.pageIndex + 1 === this.pageCount || this.pageCount === 0 || this.InCallback())
            return;

        var windowHeight = ASPxClientUtils.iOSPlatform ? window.innerHeight : ASPxClientUtils.GetDocumentClientHeight(); // TODO move to Utils
        var mainTable = this.GetMainTable();

        var mainTableBottomY = ASPx.GetAbsolutePositionY(mainTable) + mainTable.clientHeight;
        var scrollTop = ASPxClientUtils.GetDocumentScrollTop();
        var scrollDelta = mainTableBottomY - windowHeight - scrollTop;

        if(scrollDelta <= this.GetEndlessPagingMinScrollDelta())
            this.NextPage();
    },
    HasEmptySpaceInLastCardLine: function() {
        if(!this.IsFlowLayoutMode())
            return false;
        var cards = this.GetEndlessPagingHelper().GetItemElements();
        if(cards.length === 0)
            return false;

        var mainTable = this.GetMainTable();
        var lastCard = cards[cards.length - 1];
        
        var mainTableRightPos = ASPx.GetAbsoluteX(mainTable) + mainTable.offsetWidth;
        var lastCardRightPos = ASPx.GetAbsoluteX(lastCard) + lastCard.offsetWidth;

        return mainTableRightPos - lastCardRightPos >= lastCard.offsetWidth;

    },
    GetEndlessPagingMinScrollDelta: function() {
        var result = this.DefaultEndlessPagingScrollDelta;
        if(this.HasEmptySpaceInLastCardLine()) {
            var card = this.GetEndlessPagingHelper().GetItemElements()[0];
            result += card.offsetHeight;
        }
        return result;
    },

    OnAfterCallback: function() {
        ASPxClientGridBase.prototype.OnAfterCallback.call(this);
        this.CheckEndlessPagingMoreButtonVisibility();
    },
    CheckEndlessPagingMoreButtonVisibility: function() {
        if(this.IsOnClickEndlessPagingMode()) {
            var container = this.GetEndlessPagingMoreButtonContainer();
            var show = this.pageCount > -1 && this.pageIndex < this.pageCount - 1;
            ASPx.SetElementDisplay(container, show);
            if(this.GetEndlessPagingLPContainer()) {            
                ASPx.Attr.RestoreStyleAttribute(container, "min-height");
                ASPx.Attr.RestoreStyleAttribute(container, "padding");
                ASPx.SetElementDisplay(this.GetEndlessPagingMoreButtonDiv(), true);
            }
        }
    },
    ShowLoadingPanel: function() {
        if(this.IsOnClickEndlessPagingMode() && this.NeedShowLoadingPanelInsideEndlessPagingContainer()){
            var container = this.GetEndlessPagingMoreButtonContainer();
            ASPx.Attr.ChangeStyleAttribute(container, "min-height",container.offsetHeight + "px");    
            ASPx.Attr.ChangeStyleAttribute(container, "padding","0px");
            ASPx.SetElementDisplay(this.GetEndlessPagingMoreButtonDiv(), false);
        }
        ASPxClientGridBase.prototype.ShowLoadingPanel.call(this);
    },
    SortBy: function(column, sortOrder, reset, sortIndex){
       ASPxClientGridBase.prototype.SortBy.call(this, column, sortOrder, reset, sortIndex);
    },
    MoveColumn: function(column, columnMoveTo, moveBefore){
        ASPxClientGridBase.prototype.MoveColumn.call(this, column, columnMoveTo, moveBefore);
    },
    GetCardKey: function(visibleIndex) {
        return this.GetItemKey(visibleIndex);
    },
    StartEditCard: function(visibleIndex) {
       this.StartEditItem(visibleIndex);
    },
    StartEditCardByKey: function(key) {
        this.StartEditItemByKey(key);
    },
    IsNewCardEditing: function() {
        return this.IsNewItemEditing();
    },
    AddNewCard: function(){
       this.AddNewItem();
    },
    DeleteCard: function(visibleIndex){
        this.DeleteItem(visibleIndex);
    },
    DeleteCardByKey: function(key) {
        this.DeleteItemByKey(key);
    },
    GetFocusedCardIndex: function() {
        return this._getFocusedItemIndex();
    },
    SetFocusedCardIndex: function(visibleIndex) {
        return this._setFocusedItemIndex(visibleIndex);
    },
    SelectCards: function(visibleIndices, selected){
        this.SelectItemsCore(visibleIndices, selected, false);
    },
    SelectCardsByKey: function(keys, selected){
        this.SelectItemsByKey(keys, selected);
    },
    UnselectCardsByKey: function(keys){
        this.SelectCardsByKey(keys, false);
    },
    UnselectCards: function(visibleIndices){
        this.SelectCards(visibleIndices, false);
    },
    UnselectFilteredCards: function() {
        this.UnselectFilteredItemsCore();
    },
    SelectCardOnPage: function(visibleIndex, selected){
        if(!ASPx.IsExists(selected)) selected = true;
        this.SelectItem(visibleIndex, selected);
    },
    UnselectCardOnPage: function(visibleIndex){
        this.SelectCardOnPage(visibleIndex, false);
    },
    SelectAllCardsOnPage: function(selected){
        if(!ASPx.IsExists(selected)) selected = true;
        this._selectAllRowsOnPage(selected);
    },
    UnselectAllCardsOnPage: function(){
        this._selectAllRowsOnPage(false);
    },
    GetSelectedCardCount: function() {
        return this._getSelectedRowCount();
    },
    IsCardSelectedOnPage: function(visibleIndex) {
		return this._isRowSelected(visibleIndex);
    },

    RaiseFocusedItemChanged: function(){
        if(!this.FocusedCardChanged.IsEmpty()){
            var args = new ASPxClientProcessingModeEventArgs(false);
            this.FocusedCardChanged.FireEvent(this, args);
            if(args.processOnServer)
                this.gridCallBack([ASPxClientGridViewCallbackCommand.FocusedRow]);
        }
        return false; 
    },
    RaiseItemClick: function(visibleIndex, htmlEvent) {
        if(!this.CardClick.IsEmpty()){
            var args = new ASPxClientCardViewCardClickEventArgs(visibleIndex, htmlEvent);
            this.CardClick.FireEvent(this, args);
            return args.cancel;
        }
        return false; 
    },
    RaiseItemDblClick: function(visibleIndex, htmlEvent) {
        if(!this.CardDblClick.IsEmpty()){
            ASPx.Selection.Clear(); // B141112
            var args = new ASPxClientCardViewCardClickEventArgs(visibleIndex, htmlEvent);
            this.CardDblClick.FireEvent(this, args);
            return args.cancel;
        }
        return false; 
    },
    CreateCommandCustomButtonEventArgs: function(index, id){
        return new ASPxClientCardViewCustomButtonEventArgs(index, id);
    },
    CreateSelectionEventArgs: function(visibleIndex, isSelected, isAllRecordsOnPage, isChangedOnServer){
        return new ASPxClientCardViewSelectionEventArgs(visibleIndex, isSelected, isAllRecordsOnPage, isChangedOnServer);
    },
    CreateColumnCancelEventArgs: function(column){
        return new ASPxClientCardViewColumnCancelEventArgs(column);
    },
    UA_EndlessShowMore: function() {
        this.NextPage();
    },

    GetCardValues: function(visibleIndex, fieldNames, onCallBack) {
        this.GetItemValues(visibleIndex, fieldNames, onCallBack);
    },
    GetPageCardValues: function(fieldNames, onCallBack) {
        this.GetPageItemValues(fieldNames, onCallBack);
    },
    GetVisibleCardsOnPage: function() {
        return this.GetVisibleItemsOnPage();
    },

    RaiseBatchEditStartEditing: function(visibleIndex, column, rowValues) {
        var args = new ASPxClientCardViewBatchEditStartEditingEventArgs(visibleIndex, column, rowValues);
        if(!this.BatchEditStartEditing.IsEmpty())
            this.BatchEditStartEditing.FireEvent(this, args);
        return args;
    },
    RaiseBatchEditEndEditing: function(visibleIndex, rowValues) {
        var args = new ASPxClientCardViewBatchEditEndEditingEventArgs(visibleIndex, rowValues);
        if(!this.BatchEditEndEditing.IsEmpty())
            this.BatchEditEndEditing.FireEvent(this, args);
        return args;
    },
    RaiseBatchEditItemValidating: function(visibleIndex, validationInfo) {
        var args = new ASPxClientCardViewBatchEditCardValidatingEventArgs(visibleIndex, validationInfo);
        if(!this.BatchEditCardValidating.IsEmpty())
            this.BatchEditCardValidating.FireEvent(this, args);
        return args.validationInfo;
    },
    RaiseBatchEditConfirmShowing: function(requestTriggerID) {
        if(!this.BatchEditConfirmShowing.IsEmpty()) {
            var args = new ASPxClientCardViewBatchEditConfirmShowingEventArgs(requestTriggerID);
            this.BatchEditConfirmShowing.FireEvent(this, args);
            return args.cancel;
        }
        return false;
    },
    RaiseBatchEditTemplateCellFocused: function(columnIndex) {
        var column = this._getColumn(columnIndex);
        if(!column) return false;
        var args = new ASPxClientCardViewBatchEditTemplateCellFocusedEventArgs(column);
        if(!this.BatchEditTemplateCellFocused.IsEmpty())
            this.BatchEditTemplateCellFocused.FireEvent(this, args);
        return args.handled;
    },




    // ASPxClientGridBase methods/events declarations for help

}); 
ASPxClientCardView.Cast = ASPxClientGridBase.Cast;
var ASPxClientCardViewColumn = ASPx.CreateClass(ASPxClientGridColumnBase, {
});
var ASPxClientCardViewColumnCancelEventArgs = ASPx.CreateClass(ASPxClientCancelEventArgs, {
	constructor: function(column){
	    this.constructor.prototype.constructor.call(this);
        this.column = column;
    }
});
var ASPxClientCardViewCardClickEventArgs = ASPx.CreateClass(ASPxClientCancelEventArgs, {
	constructor: function(visibleIndex, htmlEvent){
	    this.constructor.prototype.constructor.call(this, visibleIndex);
        this.visibleIndex = visibleIndex;
        this.htmlEvent = htmlEvent;
    }
});
var ASPxClientCardViewCustomButtonEventArgs = ASPx.CreateClass(ASPxClientProcessingModeEventArgs, {
	constructor: function(visibleIndex, buttonID) {
		this.constructor.prototype.constructor.call(this, false);
		this.visibleIndex = visibleIndex;
		this.buttonID = buttonID;
	}	
});
var ASPxClientCardViewSelectionEventArgs = ASPx.CreateClass(ASPxClientProcessingModeEventArgs, {
	constructor: function(visibleIndex, isSelected, isAllRecordsOnPage, isChangedOnServer){
	    this.constructor.prototype.constructor.call(this, false);
        this.visibleIndex = visibleIndex;
        this.isSelected = isSelected;
        this.isAllRecordsOnPage = isAllRecordsOnPage;
        this.isChangedOnServer = isChangedOnServer;
    }
});
var ASPxClientCardViewBatchEditStartEditingEventArgs = ASPx.CreateClass(ASPxClientGridBatchEditStartEditingEventArgs, {
	constructor: function(visibleIndex, focusedColumn, itemValues) {
	    this.constructor.prototype.constructor.call(this, visibleIndex, focusedColumn, itemValues);
        this.cardValues = this.itemValues;
    }
});
var ASPxClientCardViewBatchEditEndEditingEventArgs = ASPx.CreateClass(ASPxClientGridBatchEditEndEditingEventArgs, {
	constructor: function(visibleIndex, itemValues) {
	    this.constructor.prototype.constructor.call(this, visibleIndex, itemValues);
        this.cardValues = this.itemValues;
    }
});
var ASPxClientCardViewBatchEditCardValidatingEventArgs = ASPx.CreateClass(ASPxClientGridBatchEditItemValidatingEventArgs, {
	constructor: function(visibleIndex, validationInfo) {
	    this.constructor.prototype.constructor.call(this, visibleIndex, validationInfo);
    }
});
var ASPxClientCardViewBatchEditConfirmShowingEventArgs = ASPx.CreateClass(ASPxClientGridBatchEditConfirmShowingEventArgs, {
	constructor: function(requestTriggerID) {
	    this.constructor.prototype.constructor.call(this, requestTriggerID);
    }
});
var ASPxClientCardViewBatchEditTemplateCellFocusedEventArgs = ASPx.CreateClass(ASPxClientGridBatchEditTemplateCellFocusedEventArgs, {
	constructor: function(column) {
	    this.constructor.prototype.constructor.call(this, column);
    }
});

ASPx.CVToogleCustWindow = function(name) {
    var gv = ASPx.GetControlCollection().Get(name);
    if(gv != null)
        gv.ShowCustomizationWindow();
};

var CardViewTableScrollHelper = ASPx.CreateClass(null, {
    constructor: function(grid){
        this.control = grid;
        this.useEndlessPaging = this.control.useEndlessPaging;
        this.scrollableControlHeight = -1;
        this.savedScrollTop = 0;
    },

    ApplyScrollPosition: function() {
        this.SetVertScrollPosition(this.savedScrollTop);
    },

    AssignPosLoggerHandler: function() {
        var scrollContainer = this.GetScrollableContrainer();
        if (!scrollContainer) return;

        if(!this.posLoggerHandler)
            this.posLoggerHandler = function() { this.LogScrollPosition(); }.aspxBind(this);

        ASPx.Evt.DetachEventFromElement(scrollContainer, "scroll", this.posLoggerHandler);
        ASPx.Evt.AttachEventToElement(scrollContainer, "scroll", this.posLoggerHandler);
    },

    GetScrollState: function() { return this.control.stateObject.scrollState; },
    SetScrollState: function(position) { this.control.stateObject.scrollState = [ position ]; },
    
    HasVertScroll: function() { return this.control.HasVertScroll(); },

    LoadScrollPosition: function() {
        var savedState = this.GetScrollState();
        if(savedState && savedState.length > 0) 
            this.savedScrollTop = savedState[0];
    },
    SaveScrollPosition: function() {
        this.SetScrollState(this.savedScrollTop);
    },

    LogScrollPosition: function() {
        var scrollContainer = this.GetScrollableContrainer();
        if(this.HasVertScroll() && scrollContainer)
            this.savedScrollTop = scrollContainer.scrollTop;
        this.SaveScrollPosition();
    },

    SetHeight: function(height) {
        if(this.useEndlessPaging)
            this.CheckEndlessPagingLoadNextPage();
        if(this.scrollableControlHeight && this.scrollableControlHeight === height)
            return;
        this.scrollableControlHeight = height;
        this.Update();
    },

    SetHeightCore: function(height) {
        var mainElement = this.control.GetMainElement();
        var div = this.GetScrollableContrainer();
        var mainElementHeight = mainElement.offsetHeight;

        height = ASPx.PxToFloat(div.style.height) + (height - mainElementHeight);
        div.style.height = Math.max(height, 0) + "px";
    },

    
    SetVertScrollPosition: function(pos) {
        var scrollContainer = this.GetScrollableContrainer();
        if(!this.HasVertScroll() || !scrollContainer)
            return;
        
        if(scrollContainer.scrollTop != this.savedScrollTop)
            scrollContainer.scrollTop = this.savedScrollTop;
    },
    
    Update: function(){
        this.AssignPosLoggerHandler();
        this.useEndlessPaging && this.UpdateEndlessPaging();

        if(this.scrollableControlHeight > -1)
            this.SetHeightCore(this.scrollableControlHeight);
        
        this.LoadScrollPosition();
        this.ApplyScrollPosition();
    },
    
    UpdateEndlessPaging: function() {
        var container = this.GetScrollableContrainer();
        if(container.dxEndlessPaging)
            return;
        ASPx.Evt.AttachEventToElement(container, "scroll", function(e) { this.OnEndlessPagingScroll(e); }.aspxBind(this)); 
        ASPx.Evt.AttachEventToElement(container, ASPx.Evt.GetMouseWheelEventName(), function(e) { this.OnEndlessPagingMouseWheel(e); }.aspxBind(this));
        container.dxEndlessPaging = true;
    },
    GetScrollableContrainer: function(){
        var mainTable = this.GetContentTable();
        return mainTable ? mainTable.parentNode : null;
    },
    GetContentTable: function(){
        return this.control.GetMainTable();
    },
    OnEndlessPagingMouseWheel: function(e) {
        if(!this.control.InCallback()) return;
        var canScrollBottom = this.CalculateBottomExcess(true) > 0;
        var isScrollingToBottom = ASPx.Evt.GetWheelDelta(e) < 0;
        if(!canScrollBottom && isScrollingToBottom)
            return ASPx.Evt.PreventEvent(e);
    },
    OnEndlessPagingScroll: function(e) {
        this.CheckEndlessPagingLoadNextPage();
    },
    CheckEndlessPagingLoadNextPage: function(){
        var grid = this.control;
        if(grid.IsOnClickEndlessPagingMode())
            return;
        if(grid.pageIndex + 1 === grid.pageCount || grid.pageCount === 0 || grid.InCallback())
            return;

        var cards = grid.GetEndlessPagingHelper().GetItemElements();
        var lastCardHeight = cards.length > 0 ? cards[cards.length - 1].offsetHeight : -1;

        if(lastCardHeight > 0 && this.CalculateBottomExcess() - lastCardHeight < 0)
            grid.NextPage();
    },
    CalculateBottomExcess: function(withLoadingPanel) {
        var table = this.GetContentTable();
        var scrollDiv = this.GetScrollableContrainer(table);
        var contentHeight = table.offsetHeight;
        var result = contentHeight - scrollDiv.clientHeight - scrollDiv.scrollTop;
        if(withLoadingPanel)
            result += this.control.GetEndlessPagingLPContainer().clientHeight;
        return result;
    }
});
var ASPxClientCardViewBatchEditApi = ASPx.CreateClass(ASPxClientGridBatchEditApi, {
    constructor: function(grid) {
        this.constructor.prototype.constructor.call(this, grid);
    },
    ValidateCards: function() {
        return this.ValidateItems();
    },
    ValidateCard: function(visibleIndex) {
        return this.ValidateItem(visibleIndex);
    }
});

window.ASPxClientCardView = ASPxClientCardView;
window.ASPxClientCardViewColumn = ASPxClientCardViewColumn;
window.ASPxClientCardViewColumnCancelEventArgs = ASPxClientCardViewColumnCancelEventArgs;
window.ASPxClientCardViewCardClickEventArgs = ASPxClientCardViewCardClickEventArgs;
window.ASPxClientCardViewCustomButtonEventArgs = ASPxClientCardViewCustomButtonEventArgs;
window.ASPxClientCardViewSelectionEventArgs = ASPxClientCardViewSelectionEventArgs;
window.ASPxClientCardViewBatchEditStartEditingEventArgs = ASPxClientCardViewBatchEditStartEditingEventArgs;
window.ASPxClientCardViewBatchEditEndEditingEventArgs = ASPxClientCardViewBatchEditEndEditingEventArgs;
window.ASPxClientCardViewBatchEditCardValidatingEventArgs = ASPxClientCardViewBatchEditCardValidatingEventArgs;
window.ASPxClientCardViewBatchEditConfirmShowingEventArgs = ASPxClientCardViewBatchEditConfirmShowingEventArgs;
window.ASPxClientCardViewBatchEditTemplateCellFocusedEventArgs = ASPxClientCardViewBatchEditTemplateCellFocusedEventArgs;
window.ASPxClientCardViewBatchEditApi = ASPxClientCardViewBatchEditApi;
})();