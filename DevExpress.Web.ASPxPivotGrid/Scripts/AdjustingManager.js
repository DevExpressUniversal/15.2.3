(function() {
var PivotAdjustingManager = ASPx.CreateClass(null, {
    constructor: function(pivot, onCollapsed) {
        this.pivot = pivot;
        this.onCollapsed = onCollapsed;
        this.utils = this.createSizeUtils();
        this.opts = null;
    },

    // DOM elements
    getMainElement: function() { return ASPx.GetElementById(this.pivot.name); },
    getMainTable: function() { return this.pivot.GetChildElement("MT"); },
    getPivotTableContainer: function() { return this.pivot.GetChildElement("PTCDiv");},
    getDummyPivotTableContainer: function() { return this.pivot.GetChildElement("PTCDiv_Dummy");},
    getFilterAreaContainer: function() { return this.pivot.GetChildElement("FilterAreaContainer"); },
    getCallbackAnimationElement: function() { return this.pivot.GetChildElement("DCSCell_SCRootDiv")},

    HasAdjustingLogic: function() {
        return this.opts != null;
    },

    SetRenderOptions: function(options) {
        this.opts = options;
        var wrapper = this.pivotTableWrapper;
        if(wrapper && wrapper.state != "Initial")
            wrapper.toInitialState();
        this.invalidateWrapper();
    },

    UpdatePartial: function(pivotTableInnerHtml) {
        var that = this,
            dummyContainer = that.getDummyPivotTableContainer();
        ASPx.SetInnerHtml(dummyContainer, pivotTableInnerHtml);
    },

    OnNewMarkup: function() {
        this.invalidateWrapper();
    },

    Adjust: function() {
        var that = this,
            utils = that.utils,
            wrapper = that.getPivotTableWrapper(),
            pivotTable = that.getPivotTableFromContainer(that.getWorkingPivotTableContainer()),
            isPartialRender = that.isPartialRender();

        if(pivotTable.offsetParent == null) // pivotTable is not visible (T263106)
            return;

        that.setFilterAreaDivWidth(0);
        if(isPartialRender) {
            that.setPivotTableContainerVisibility(that.getDummyPivotTableContainer(), true);
            that.setPivotTableContainerVisibility(that.getPivotTableContainer(), false);
        }
        if(!!that.basePivotTableWrapper) {
            wrapper.MergeBeforeCollapse(that.basePivotTableWrapper);
        }


        wrapper.CalculateSizes(that.opts, function() {
            that.onCollapsed();
            var mainContainer = that.getMainElement(),
                containerWidth =  utils.getRect(mainContainer).width - ASPx.GetLeftRightBordersAndPaddingsSummaryValue(mainContainer),
                containerHeight = utils.getRect(mainContainer).height - ASPx.GetTopBottomBordersAndPaddingsSummaryValue(mainContainer),
                scrollableWidth = containerWidth - utils.getRect(pivotTable).width,
                scrollableHeight = containerHeight - utils.getRect(that.getMainTable()).height;
            return {
                scrollableWidth: scrollableWidth,
                scrollableHeight: scrollableHeight
            };
        });

        if(isPartialRender) {
            var newWrapper = wrapper;
            that.basePivotTableWrapper.Merge(newWrapper);
            that.pivotTableWrapper = that.basePivotTableWrapper;
            wrapper = that.pivotTableWrapper;

            that.setPivotTableContainerVisibility(that.getDummyPivotTableContainer(), false);
            that.setPivotTableContainerVisibility(that.getPivotTableContainer(), true);
            that.getDummyPivotTableContainer().innerHTML = '';
        }
        wrapper.Adjust(that.opts, isPartialRender);
        var mainContainer = that.getMainElement(),
            containerWidth =  utils.getRect(mainContainer).width - ASPx.GetLeftRightBordersAndPaddingsSummaryValue(mainContainer);
        that.setFilterAreaDivWidth(containerWidth);
    },

    ResetScrollPos: function() {
        var that = this;
        if(that.pivotTableWrapper) {
            that.pivotTableWrapper.ResetScrollPos(false, true);
        }
    },

    GetCallbackAnimationElement: function() {
        return this.getCallbackAnimationElement();
    },

    invalidateWrapper: function() {
        var that = this;
        if(!!that.pivotTableWrapper)
            that.basePivotTableWrapper = that.pivotTableWrapper;
        this.pivotTableWrapper = null;
    },

    isPartialRender: function() {
        var that = this,
            dummyContainer = that.getDummyPivotTableContainer();
        return !!that.getPivotTableFromContainer(dummyContainer);
    },

    getPivotTableFromContainer: function(container) {
        var childNodes = container.childNodes;
        for(var i = 0; i < childNodes.length; i++) {
            var el = childNodes[i];
            if(el.tagName == 'TABLE')
                return el;
        }
        return null;
    },

    onPagingRequest: function(vertPageIndex, vertPageSize, horzPageIndex, horzPageSize) {
        var that = this,
            pivot = that.pivot;
        pivot.PerformCallbackInternal(pivot.GetMainTable(),
                'VS|' + vertPageIndex + '|' + vertPageSize + '|' + horzPageIndex + '|' + horzPageSize);
    },

    setPivotTableContainerVisibility: function(container, visible) {
        var sizeStyle = visible ? '' : '0px';
        container.style.width = sizeStyle;
        container.style.height = sizeStyle;
    },

    getWorkingPivotTableContainer: function() {
        var that = this,
            container = that.isPartialRender() ? that.getDummyPivotTableContainer() : that.getPivotTableContainer();
        return container;
    },

    createSizeUtils: function() {
        return {
            getRect: function(el) {
                var rect = el.getBoundingClientRect();
                if(!ASPx.IsExists(rect.width)) {
                    rect = {
                        right: rect.right,
                        left: rect.left,
                        top: rect.top,
                        bottom: rect.bottom,
                        width: rect.right - rect.left,
                        height: rect.bottom - rect.top
                    };
                }
                return rect;
            }
        };
    },

    getPivotTableWrapper: function() {
        var that = this,
            wrapper = that.pivotTableWrapper;
        if(!wrapper) {
            var container = that.isPartialRender() ? that.getDummyPivotTableContainer() : that.getPivotTableContainer(),
                pivotTable = that.getPivotTableFromContainer(container);
            wrapper = new ASPx.PivotTableWrapper(that.utils);
            wrapper.Initialize(that.pivot, pivotTable, that.onPagingRequest, that.opts);
            that.pivotTableWrapper = wrapper;
        }
        return wrapper;
    },

    setFilterAreaDivWidth: function(width) {
        var filterAreaDiv = this.getFilterAreaContainer();
        if(filterAreaDiv != null) filterAreaDiv.style.width = width + 'px';
    }
});

ASPx.PivotAdjustingManager = PivotAdjustingManager;
})();