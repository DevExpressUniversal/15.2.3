/// <reference path="_references.js"/>

(function () {
var ASPxClientDataView = ASPx.CreateClass(ASPxClientControl, {
    EndlessPagingMode: {
        Disabled: 0,
        OnClick: 1,
        OnScroll: 2
    },

    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);

        this.callbackPrefixes = { Custom: "c", PagerClick: "p" };
        this.supportGestures = true;
        this.endlessPagingHelper = null;

        ASPxClientDataViewCollection.add(this);
    },
    Initialize: function () {
        ASPxClientControl.prototype.Initialize.call(this);

        var helper = this.GetEndlessPagingHelper();
        if(helper) helper.Initialize();
    },
    InlineInitialize: function () {
        ASPxClientControl.prototype.InlineInitialize.call(this);
        this.ChangeEndlessPagingContainerVisiblity();
    },

    AdjustControlCore: function () {
        this.UpdateItemsScroller();
        this.AdjustPagerControls();
    },
    OnBrowserWindowResize: function (evt) {
        this.AdjustControl();
    },

    GetContentCell: function () {
        return this.GetChildElement("CCell");
    },
    GetItemsCell: function () {
        return this.GetChildElement("ICell");
    },
    GetItemsScroller: function () {
        return this.GetChildElement("IScroller");
    },
    GetEndlessPagingContainer: function () {
        return this.GetChildElement("EPContainer");
    },
    IsTableLayout: function () {
        return this.stateObject.layout === 0;
    },
    DoPagerClick: function (value) {
        this.AssignSlideAnimationDirectionByPagerArgument(value, this.GetPageIndex());

        var endlessPagingHelper = this.GetEndlessPagingHelper();
        if(endlessPagingHelper)
            endlessPagingHelper.OnDoPagerClick(value);

        var preparedArgs = this.PrepareCallbackArgs(this.GetPageIndex() + ASPx.CallbackSeparator + this.GetPageSize() + ASPx.CallbackSeparator + value, false);
        this.CreateCallback(preparedArgs, "PAGERCLICK");
    },

    CanHandleGesture: function (evt) {
        var source = ASPx.Evt.GetEventSource(evt);
        return ASPx.GetIsParent(this.GetItemsCell(), source);
    },
    AllowStartGesture: function () {
        return ASPxClientControl.prototype.AllowStartGesture.call(this) &&
            (this.AllowExecutePagerGesture(this.GetPageIndex(), this.GetPageCount(), 1) || this.AllowExecutePagerGesture(this.GetPageIndex(), this.GetPageCount(), -1));
    },
    AllowExecuteGesture: function (value) {
        return this.AllowExecutePagerGesture(this.GetPageIndex(), this.GetPageCount(), value);
    },
    ExecuteGesture: function (value, count) {
        this.ExecutePagerGesture(this.GetPageIndex(), this.GetPageCount(), value, count, function (arg) { this.DoPagerClick(arg); }.aspxBind(this));
    },
    AssignSlideAnimationDirectionByPagerArgument: function (arg, currentPageIndex) {
        if(this.UseEndlessPaging()) return;
        ASPxClientControl.prototype.AssignSlideAnimationDirectionByPagerArgument.call(this, arg, currentPageIndex);
    },

    CustomCallback: function (args) {
        var preparedArgs = this.PrepareCallbackArgs(args, true);
        if(!this.callBack) {
            if(this.isInitialized)
                this.SendPostBack(preparedArgs);
            return;
        }
        this.CreateCallback(preparedArgs, "CUSTOMCALLBACK");
    },
    OnCallback: function (result) {
        var areStatesEqual = this.GetPageIndex() == result.stateObject.pageIndex && this.GetPageSize() == result.stateObject.pageSize;
        this.UpdateStateObjectWithObject(result.stateObject);

        var isEndlessPagingCallback = ASPx.IsExists(result.epHtml);
        if(this.UseEndlessPaging() && isEndlessPagingCallback) {
            var helper = this.GetEndlessPagingHelper();
            if(helper && !areStatesEqual)
                helper.OnCallback(result.html, result.epHtml);
        } else {
            var element = this.GetContentCell();
            if(element != null)
                ASPx.SetInnerHtml(element, result.html);
        }
    },
    DoEndCallback: function () {
        ASPxClientControl.prototype.DoEndCallback.call(this);
            
        var helper = this.GetEndlessPagingHelper();
        if(helper) helper.DoEndCallback();

        this.UpdateItemsScroller();
    },

    CreateCallback: function (arg, command, callbackInfo) {
        this.ShowLoadingElements();
        ASPxClientControl.prototype.CreateCallback.call(this, arg, command);
    },
    PrepareCallbackArgs: function (args, isCustomCallback) {
        return (isCustomCallback ? this.callbackPrefixes.Custom : this.callbackPrefixes.PagerClick) + args;
    },
    ShowLoadingPanel: function () {
        var endlessPagingHelper = this.GetEndlessPagingHelper();
        var endlessContainer = this.GetEndlessPagingContainer();
        if(!endlessPagingHelper || !endlessPagingHelper.NeedShowLoadingPanelAtBottom() || !endlessContainer) {
            this.CreateLoadingPanelWithAbsolutePosition(this.GetContentCell(), this.GetLoadingPanelOffsetElement(this.GetContentCell()));
            return;
        }

        var nodes = endlessContainer.childNodes;
        for(var i = 0; i < nodes.length; i++)
            ASPx.RemoveElement(nodes[i]);
        this.CreateLoadingPanelWithoutBordersInsideContainer(endlessContainer);
    },
    ShowLoadingDiv: function () {
        this.CreateLoadingDiv(this.GetContentCell());
    },
    GetCallbackAnimationElement: function () {
        var itemsCell = this.GetItemsCell();
        if(itemsCell)
            return itemsCell.firstChild;
        return null;
    },

    GetEndlessPagingMode: function () {
        if(ASPx.IsExists(this.stateObject) && ASPx.IsExists(this.stateObject.endlessPagingMode))
            return this.stateObject.endlessPagingMode;
        return this.EndlessPagingMode.Disabled;
    },
    UseEndlessPaging: function () {
        return !!this.GetEndlessPagingMode();
    },
    GetEndlessPagingHelper: function () {
        if(!this.UseEndlessPaging())
            return;
        if(!this.endlessPagingHelper)
            this.endlessPagingHelper = new ASPxClientDataViewEndlessPagingHelper(this);
        return this.endlessPagingHelper;
    },
    ChangeEndlessPagingContainerVisiblity: function () {
        var epContainer = this.GetEndlessPagingContainer();
        if(epContainer && epContainer.parentNode) {
            var isEPContainerVisible = !!epContainer.innerHTML && !(new RegExp(/^\s+$/).test(epContainer.innerHTML));
            ASPx.SetElementDisplay(epContainer.parentNode, isEPContainerVisible);
        }
    },

    OnBrowserWindowResize: function(evt){
        ASPxClientControl.prototype.OnBrowserWindowResize.call(this, evt);

        var helper = this.GetEndlessPagingHelper();
        if(helper)  helper.LoadNextPageIfRequired();

        this.UpdateItemsScroller();
    },
    OnScroll: function(evt){
        var helper = this.GetEndlessPagingHelper();
        if(helper) helper.LoadNextPageIfRequired();
    },
    UpdateItemsScroller: function(){
        var scroller = this.GetItemsScroller();
        if(scroller){
            var scrollerParent = scroller.parentNode;
            scroller.style.height = "0px";
            scroller.style.height = (scrollerParent.offsetHeight - ASPx.GetTopBottomBordersAndPaddingsSummaryValue(scrollerParent)) + "px";
        }
    },
    SetHeight: function(height) {
        ASPxClientControl.prototype.SetHeight.call(this, height);
        this.UpdateItemsScroller();
    },
    GotoPage: function (pageIndex) {
        this.DoPagerClick(ASPx.PagerCommands.PageNumber + pageIndex);
    },
    GetPageIndex: function () {
        return this.stateObject.pageIndex;
    },
    GetPageSize: function () {
        return this.stateObject.pageSize;
    },
    SetPageSize: function (pageSize) {
        this.DoPagerClick(ASPx.PagerCommands.PageSize + pageSize);
    },
    GetPageCount: function () {
        return this.stateObject.pageCount;
    },
    NextPage: function () {
        this.DoPagerClick(ASPx.PagerCommands.Next);
    },
    PrevPage: function () {
        this.DoPagerClick(ASPx.PagerCommands.Prev);
    },
    FirstPage: function () {
        this.DoPagerClick(ASPx.PagerCommands.First);
    },
    LastPage: function (focusBottomRow) {
        this.DoPagerClick(ASPx.PagerCommands.Last);
    },
    PerformCallback: function (parameter) {
        this.CustomCallback(parameter);
    }
});
ASPxClientDataView.Cast = ASPxClientControl.Cast;

var ASPxClientDataViewEndlessPagingHelper = ASPx.CreateClass(null, {
    DefaultScrollDelta: 70,

    constructor: function (dataView) {
        this.dataView = dataView;
        this.needShowLoadingPanelAtBottom = false;
    },

    LoadNextPageIfRequired: function () {
        if(this.IsRequiredPageLoad())
            this.dataView.NextPage();
    },
    NeedShowLoadingPanelAtBottom: function () {
        return this.needShowLoadingPanelAtBottom;
    },
    GetEndlessPagingMode: function () {
        return this.dataView.GetEndlessPagingMode();
    },
    IsRequiredPageLoad: function () {
        var dataView = this.dataView;
        if(!dataView.UseEndlessPaging() || dataView.InCallback() || dataView.GetPageIndex() >= (dataView.GetPageCount() - 1))
            return false;
        if(this.GetEndlessPagingMode() == dataView.EndlessPagingMode.OnClick)
            return false;

        var windowHeight = ASPxClientUtils.iOSPlatform ? window.innerHeight : ASPxClientUtils.GetDocumentClientHeight();
        var mainElement = dataView.GetMainElement();
        var dataViewOffsetBottom = ASPx.GetAbsolutePositionY(mainElement) + mainElement.clientHeight;
        var scrollTop = ASPxClientUtils.GetDocumentScrollTop();

        var scrollDelta = dataViewOffsetBottom - windowHeight - scrollTop;
        return scrollDelta <= this.GetMinScrollDelta();
    },
    GetMinScrollDelta: function() {
        if (this.IsTableLayout())
            return this.DefaultScrollDelta;

        var container = this.GetUpdatableItemsContainer();
        var lastDataItem = container.lastChild;
        var containerRightX = ASPx.GetAbsoluteX(container) + container.offsetWidth;
        var lastElementRightX = ASPx.GetAbsoluteX(lastDataItem) + lastDataItem.offsetWidth;
        
        var canAddItemInLastLine = containerRightX - lastElementRightX >= lastDataItem.offsetWidth;
        return canAddItemInLastLine ? lastDataItem.offsetHeight + this.DefaultScrollDelta : this.DefaultScrollDelta;
    },
    IsTableLayout: function() {
        return this.dataView.IsTableLayout();
    },

    Initialize: function () {
        this.LoadNextPageIfRequired();
    },
    OnDoPagerClick: function (command) {
        this.needShowLoadingPanelAtBottom = command == ASPx.PagerCommands.Next;
    },
    OnCallback: function (html, epHtml) {
        this.dataView.HideLoadingElements();
        this.ClearEPContainer();
        this.needShowLoadingPanelAtBottom = false;

        var container = this.dataView.GetEndlessPagingContainer();
        if(container)
            ASPx.SetInnerHtml(container, epHtml);

        var itemsContainer = this.GetUpdatableItemsContainer();
        if(this.IsTableLayout())
            this.InsertRows(itemsContainer, html);
        else
            itemsContainer.insertAdjacentHTML("beforeEnd", html);

        this.dataView.ChangeEndlessPagingContainerVisiblity();
    },
    DoEndCallback: function () {
        this.LoadNextPageIfRequired();
    },

    ClearEPContainer: function () {
        var epContainer = this.dataView.GetEndlessPagingContainer();
        if(!epContainer)
            return;

        var nodes = epContainer.childNodes;
        for(var i = 0; i < nodes.length; i++)
            ASPx.RemoveElement(nodes[i]);
    },
    InsertRows: function (table, rowsHtml) {
        var row = document.createElement("TR");
        var cell = document.createElement("TD");
        cell.innerHTML = "<table><tbody>" + rowsHtml + "</tbody></table>";

        var tbody = table.tBodies[0];
        tbody.appendChild(row);
        row.appendChild(cell);

        var newTable = ASPx.GetNodeByTagName(cell, "TABLE", 0);
        while(newTable.rows.length > 0)
            tbody.appendChild(newTable.rows[0]);
        ASPx.RemoveElement(row);
    },
    GetUpdatableItemsContainer: function () {
        var contentTable = ASPx.GetNodesByTagName(this.dataView.GetItemsCell(), "TABLE")[0];
        if(this.IsTableLayout())
            return contentTable;
        return contentTable.rows[0].cells[0];
    }
});

ASPxClientDataViewCollection = {
    instances: { },
    add: function(dataView){
        ASPxClientDataViewCollection.instances[dataView.name] = dataView;
    },
    remove: function(name){
        delete ASPxClientDataViewCollection.instances[name];
    },
    get: function(name){
        return ASPxClientDataViewCollection.instances[name];
    },
    onScroll: function(evt){
        for(var name in ASPxClientDataViewCollection.instances){
            var dataView = ASPxClientDataViewCollection.get(name);
            if(!dataView || !ASPx.IsExists(dataView.GetMainElement()))
                ASPxClientDataViewCollection.remove(name);
            else
                dataView.OnScroll(evt);
        }
    }
}
ASPx.Evt.AttachEventToElement(window, 'scroll', function(evt){ ASPxClientDataViewCollection.onScroll(evt); });

ASPx.DVPagerClick = function(name, value) {
    var dv = ASPx.GetControlCollection().Get(name);
    if(dv != null)
        dv.DoPagerClick(value);
}
ASPx.DVEPClick = function(name) {
    var dataView = ASPx.GetControlCollection().Get(name);
    if(dataView)
        dataView.NextPage();
}

window.ASPxClientDataView = ASPxClientDataView;
})();