(function() {
ASPx.schedulerSelectionHelper = null;

//ASPx.Evt.AttachEventToDocument(ASPx.TouchUIHelper.touchMouseUpEventName, function (e) {
//    if(ASPx.schedulerSelectionHelper != null) {
//        ASPx.schedulerSelectionHelper.EndSelection(e);
//        return true;
//    }
    //});

//ASPx.Evt.AttachEventToDocument(ASPx.TouchUIHelper.touchMouseMoveEventName, function(e) {
//    if(ASPx.schedulerSelectionHelper != null) {
//        ASPx.schedulerSelectionHelper.MouseMove(e);
//        return false;
//    }
//    return true;
//});
ASPx.Evt.AttachEventToDocument("selectstart", function(e) {
    if (ASPx.schedulerGlobalMouseHandler != null) {
        ASPx.Selection.Clear();//B149978
        return false;
    }
    return true;
});

var SchedulerSelection = new ASPx.CreateClass(null, {
    constructor: function(interval, resource, firstSelectedInterval) {
        this.interval = interval;
        this.resource = resource;
        this.firstSelectedInterval = ASPx.IsExists(firstSelectedInterval) ? firstSelectedInterval : interval;
    }
});

var SchedulerSelectionHelper = ASPx.CreateClass(null, {
    constructor: function(scheduler, firstCell, continueSelection) {
        if(ASPx.schedulerSelectionHelper != null) 
            ASPx.schedulerSelectionHelper.CancelSelection();

        this.scheduler = scheduler;
        if(!continueSelection || !ASPx.IsExists(this.scheduler.selection)) {
            var resource = scheduler.GetCellResource(firstCell);
            var interval = scheduler.GetCellInterval(firstCell);
            this.scheduler.SetSelectionCore(new SchedulerSelection(interval, resource));
        }
        else
            this.ContinueSelection(firstCell);
        ASPx.schedulerSelectionHelper = this;
    },
    MouseMove: function(e) {
        var cell = this.scheduler.CalcHitTest(e).cell;
        if(ASPx.IsExists(cell)) {
            this.ContinueSelection(cell);
        }        
    },
    ContinueSelection: function(cell) {
        var interval = this.scheduler.GetCellInterval(cell);
        if(!ASPx.IsExists(interval))
            return;
        var newSelectionInterval = this.CalculateSelectionInterval(interval);
        if(!newSelectionInterval.Equals(this.scheduler.selection.interval)) {
            this.scheduler.SetSelectionInterval(newSelectionInterval);
        }
    },
    EndSelection: function(e) {
        this.CancelSelection();
        this.scheduler.OnSelectionChanged(e);
    },
    CancelSelection: function(e) {
        ASPx.schedulerSelectionHelper = null;
    },
    CalculateSelectionInterval: function(hitInterval) {
        var firstInterval = this.scheduler.selection.firstSelectedInterval;
        if(firstInterval.IntersectsWith(hitInterval))
            return new ASPxClientTimeInterval(firstInterval.GetStart(), ASPx.SchedulerGlobals.DateSubsWithTimezone(hitInterval.GetEnd(), firstInterval.GetStart()));
        if(hitInterval.GetStart() - firstInterval.GetEnd() >= 0)
            return new ASPxClientTimeInterval(firstInterval.GetStart(), ASPx.SchedulerGlobals.DateSubsWithTimezone(hitInterval.GetEnd(), firstInterval.GetStart()));
        else
            return new ASPxClientTimeInterval(hitInterval.GetStart(), ASPx.SchedulerGlobals.DateSubsWithTimezone(firstInterval.GetEnd(), hitInterval.GetStart()));
    }
});

var AppointmentSelection = ASPx.CreateClass(null, {
    constructor: function (scheduler) {
        this.selectedAppointmentIds = [];
        this.selectedAppointmentViewInfos = [];
        this.scheduler = scheduler;
        this.lockCount = 0;
        this.deferredOnAppointmentSelection = false;
        this.internalSelection = false;
    },
    BeginUpdate: function (internalSelection) {
        this.internalSelection = internalSelection;
        this.lockCount++;
    },
    EndUpdate: function () {
        if (this.lockCount > 0) {
            this.lockCount--;
            if (this.lockCount == 0) {
                if (!this.internalSelection)
                    this.ValidateViewInfos();
                if (this.deferredOnAppointmentSelection)
                    this.OnAppointmentSelectionChanged();
                this.deferredOnAppointmentSelection = false;
            }
        }
    },
    ValidateViewInfos: function () {
        ASPx.Data.ArrayClear(this.selectedAppointmentViewInfos);
        var count = this.selectedAppointmentIds.length;
        for (var i = count - 1; i >= 0; i--) {
            if (!this.AddAppointmentCore(this.selectedAppointmentIds[i]))
                ASPx.Data.ArrayRemoveAt(this.selectedAppointmentIds, i);
        }
    },
    AddAppointmentCore: function (appointmentId) {
        var viewInfos = this.scheduler.FindViewInfosByAppointmentId(appointmentId);
        var count = viewInfos.length;
        if (count <= 0)
            return false;
        for (var i = 0; i < count; i++) {
            if (this.IsAppointmentVisible(viewInfos[i]))
                this.SelectAppointmentViewInfo(viewInfos[i]);
        }
        return true;
    },
    IsAppointmentVisible: function (viewInfo) {
        var appointmentDiv = viewInfo.contentDiv;
        return appointmentDiv.style.display != "none";
    },
    Prepare: function () {
        this.aptAdorner = this.scheduler.aptAdorner;
    },
    OnAppointmentSelectionChanged: function () {
        if (this.lockCount > 0)
            this.deferredOnAppointmentSelection = true;
        else
            this.scheduler.OnAppointmentSelectionChanged(this.selectedAppointmentIds);
        //this.scheduler.SaveAppointmentSelectionState();
    },
    IsSingleAppointmentSelected: function() {
        return this.selectedAppointmentIds.length == 1;
    },
    AddAppointmentToSelection: function (appointmentId) {
        if (!this.scheduler.privateAllowAppointmentMultiSelect && this.selectedAppointmentIds.length >= 1)
            return;
        var contains = ASPx.Data.ArrayIndexOf(this.selectedAppointmentIds, appointmentId) >= 0;
        if (contains)
            return;
        if (this.lockCount > 0 && !this.internalSelection) {
            this.selectedAppointmentIds.push(appointmentId);
            this.OnAppointmentSelectionChanged();

        }
        else {
            if (this.AddAppointmentCore(appointmentId)) {
                this.selectedAppointmentIds.push(appointmentId);
                this.OnAppointmentSelectionChanged();
            }
        }
    },
    ClearSelection: function () {
        var count = this.selectedAppointmentIds.length;
        if (count <= 0)
            return;

        ASPx.Data.ArrayClear(this.selectedAppointmentIds);
        if (this.lockCount <= 0 || this.internalSelection) {
            count = this.selectedAppointmentViewInfos.length;
            for (var i = count - 1; i >= 0; i--)
                this.UnselectAppointmentViewInfoByIndex(i);
        }
        this.OnAppointmentSelectionChanged();
    },
    RemoveAppointmentFromSelection: function (appointmentId) {
        ASPx.Data.ArrayRemove(this.selectedAppointmentIds, appointmentId);
        if (this.lockCount <= 0) {
            var count = this.selectedAppointmentViewInfos.length;
            for (var i = count - 1; i >= 0; i--) {
                var viewInfo = this.selectedAppointmentViewInfos[i];
                if (viewInfo.appointmentId == appointmentId)
                    this.UnselectAppointmentViewInfoByIndex(i);
            }
        }
        this.OnAppointmentSelectionChanged();
    },
    ChangeAppointmentSelection: function (appointmentId) {
        var contains = ASPx.Data.ArrayIndexOf(this.selectedAppointmentIds, appointmentId) >= 0;
        if (contains && this.selectedAppointmentIds.length > 1)
            this.RemoveAppointmentFromSelection(appointmentId);
        else
            if (this.scheduler.privateAllowAppointmentMultiSelect || this.selectedAppointmentIds.length < 1)
                this.AddAppointmentToSelection(appointmentId);
    },
    SelectSingleAppointment: function (appointmentId) {
        if (this.selectedAppointmentIds.length == 1 && this.selectedAppointmentIds[0] == appointmentId)
            return;
        this.BeginUpdate(true);
        this.ClearSelection();
        this.AddAppointmentToSelection(appointmentId);
        this.EndUpdate();
    },
    IsAppointmentSelected: function (appointmentId) {
        return ASPx.Data.ArrayIndexOf(this.selectedAppointmentIds, appointmentId) >= 0;
    },
    UnselectAppointmentViewInfoByIndex: function (index) {
        var viewInfo = this.selectedAppointmentViewInfos[index];
        this.UnselectAppointmentViewInfoCore(viewInfo);
        ASPx.Data.ArrayRemoveAt(this.selectedAppointmentViewInfos, index);
    },
    UnselectAppointmentViewInfoCore: function (viewInfo) {
        viewInfo.contentDiv.style.zIndex = 1;
        var adornerDiv = viewInfo.adornerDiv;
        adornerDiv.oncontextmenu = null;
        adornerDiv.appointmentViewInfo = null;
        ASPx.SchedulerGlobals.RecycleNode(viewInfo.adornerDiv);
        viewInfo.adornerDiv = null;
    },
    SelectAppointmentViewInfo: function (viewInfo) {
        var adornerDiv = this.SelectAppointmentViewInfoCore(viewInfo);
        ASPx.ClearCurrentMouseEventDataObject();
        this.selectedAppointmentViewInfos.push(viewInfo);
    },
    SelectAppointmentViewInfoCore: function (viewInfo) {
        var adornerDiv = this.aptAdorner.cloneNode(true);
        adornerDiv.oncontextmenu = this.aptAdorner.oncontextmenu;
        var appointmentDiv = viewInfo.contentDiv;
        var parent = appointmentDiv.parentNode;
        parent.appendChild(adornerDiv);
        adornerDiv.appointmentViewInfo = viewInfo;
        viewInfo.adornerDiv = adornerDiv;
        this.SetAdornerDivPosition(viewInfo);
        adornerDiv.style.zIndex = 2;
        appointmentDiv.style.zIndex = 3;
        return adornerDiv;
    },
    RecalcSelection: function () {
        var count = this.selectedAppointmentViewInfos.length;
        for (var i = 0; i < count; i++)
            this.SetAdornerDivPosition(this.selectedAppointmentViewInfos[i]);
    },
    SetAdornerDivPosition: function (viewInfo) {
        var adornerDiv = viewInfo.adornerDiv;
        var appointmentDiv = viewInfo.contentDiv;
        adornerDiv.style.left = 0;
        adornerDiv.style.top = 0;
        ASPx.SetSchedulerDivDisplay(adornerDiv, true);
        var borderWidth = Math.max(adornerDiv.offsetWidth - adornerDiv.clientWidth, 0) >> 1;
        var borderHeight = Math.max(adornerDiv.offsetHeight - adornerDiv.clientHeight, 0) >> 1;

        var offsetLeft = appointmentDiv.offsetLeft;
        var offsetTop = appointmentDiv.offsetTop;
        if (ASPx.Browser.IE) {//B38290
            var offsetParent = ASPx.FindOffsetParent(appointmentDiv);
            offsetLeft = ASPx.GetAbsolutePositionX(appointmentDiv) - ASPx.GetAbsolutePositionX(offsetParent);
            offsetTop = ASPx.GetAbsolutePositionY(appointmentDiv) - ASPx.GetAbsolutePositionY(offsetParent);
            //B149871 fixed, but it is exist one scroll position in wich we can't fix this bug 
            if (offsetParent.getBoundingClientRect().top < 0)
                offsetTop++;
            if (offsetParent.getBoundingClientRect().left < 0)
                offsetLeft++;
        }

        adornerDiv.style.left = offsetLeft - borderWidth + "px"; //appointmentDiv.style.left
        adornerDiv.style.top = offsetTop - borderHeight + "px"; //appointmentDiv.style.top        

        adornerDiv.style.width = appointmentDiv.offsetWidth + "px";
        adornerDiv.style.height = appointmentDiv.offsetHeight + "px";

        //adornerDiv.style.width = appointmentDiv.offsetWidth + (ASPx.Browser.IE ? borderWidth : 0) + "px";
        //adornerDiv.style.height = appointmentDiv.offsetHeight + (ASPx.Browser.IE ? borderHeight : 0) + "px";

        /*adornerDiv.style.left -= (adornerDiv.offsetWidth - adornerDiv.clientWidth) / 2 + "px";
        adornerDiv.style.top -= (adornerDiv.offsetHeight - adornerDiv.clientHeight) / 2 + "px";*/
        adornerDiv.appointmentDiv = appointmentDiv;
        this.CalculateContentTableHeight(adornerDiv);
    },
    CalculateContentTableHeight: function (adornerDiv) {
        var children = adornerDiv.childNodes;
        var count = children.length;
        for (var i = 0; i < count; i++) {
            var child = children[i];
            if (ASPx.IsExists(child.tagName) && child.tagName.toUpperCase() == "TABLE") {
                child.style.height = adornerDiv.offsetHeight + "px";
                break;
            }
        }
    }
});

var CellHighlightElement = ASPx.CreateClass(null, {
    constructor: function(container, left, dxtop, width, height, interval) {
        this.left = left;
        this.top = dxtop;
        this.width = width;
        this.height = height;
        this.interval = interval;
        this.container = container;
    }
});

var CurrentTimeContainerViewInfoBase = ASPx.CreateClass(null, { // abstract class
    constructor: function (scheduler, viewInfo, timeMarkerLineSource, layer) {
        this.scheduler = scheduler;
        this.viewInfo = viewInfo;
        this.layer = layer;
        this.timeMarkerLineSource = timeMarkerLineSource;
    },
    Dispose: function () {
        if(this.timeMarkerLineCollection) {
            this.timeMarkerLineCollection = null;
        }
    },
    Recalc: function () {
        var time = new Date();
        var newLocalTime = time.getTime() + time.getTimezoneOffset() * 60000 + this.scheduler.clientUtcOffset;//B156956
        time = new Date(newLocalTime);
        if(!this.LayoutLayer(time)) {//B196170
            ASPx.SetElementDisplay(this.layer, false)
            return;
        }
        this.LayoutTimeIndicatorLines(time);
        this.LayoutExtraItems(time);
    },
    LayoutLayer: function (time) {
        var cellContainers = this.viewInfo.cellContainers;
        if(cellContainers.length <= 0)
            return false;
        var container = cellContainers[0];
        var cell = this.FindBaseCellForLayer(container, time);
        if(!cell)
            return false;

        var x = this.CalculateLayerX(time, cell, container);
        var y = this.CalculateLayerY(time, cell, container);

        var timeMarkerLayer = this.layer;
        timeMarkerLayer.style.position = "absolute";
        timeMarkerLayer.style.left = x + "px";
        timeMarkerLayer.style.top = y + "px";
        timeMarkerLayer.style.zIndex = "10";
        return true;
    },
    FindBaseCellForLayer: function (container, time) { //abstract member
    },
    CalculateLayerX: function (cell, time) {
        return 0;
    },
    CalculateLayerY: function (cell, time) {
        return 0;
    },
    LayoutTimeIndicatorLines: function (time) {
        if (this.scheduler.timeIndicatorVisibility == ASPxClientTimeIndicatorVisibility.Never)
            return;
        var cellContainers = this.viewInfo.cellContainers;
        var containerCount = cellContainers.length;
        if (containerCount <= 0)
            return;

        var todayContainers = this.viewInfo.FindCellContainersByTimeInterval(time, time);
        var todayContainerCount = todayContainers.length;
        if (todayContainerCount <= 0 && this.scheduler.timeIndicatorVisibility != ASPxClientTimeIndicatorVisibility.Always)
            return;
        var timeIndicatorViewInfos = [];
        if (this.scheduler.timeIndicatorVisibility == ASPxClientTimeIndicatorVisibility.Always || (todayContainerCount > 0 && this.scheduler.timeIndicatorVisibility == ASPxClientTimeIndicatorVisibility.TodayView)) {
            var timeIndicatorViewInfo = new TimeIndicatorViewInfo(this.viewInfo);
            timeIndicatorViewInfo.Add(cellContainers[0]);
            timeIndicatorViewInfo.Add(cellContainers[containerCount - 1]);
            timeIndicatorViewInfos.push(timeIndicatorViewInfo);
        } else if (this.scheduler.timeIndicatorVisibility == ASPxClientTimeIndicatorVisibility.CurrentDate) {
            for (var i = 0; i < todayContainerCount; i++) {
                var todayContainer = todayContainers[i];
                var timeIndicatorViewInfo = new TimeIndicatorViewInfo(this.viewInfo);
                timeIndicatorViewInfo.Add(todayContainer);
                timeIndicatorViewInfos.push(timeIndicatorViewInfo);
            }
        }
        
        var count = timeIndicatorViewInfos.length;
        if (this.timeMarkerLineCollection == null) {
            this.timeMarkerLineCollection = [];
            for (var i = 0; i < count; i++) {
                this.timeMarkerLineCollection.push(this.CreateTodayTimeMarkerLine());
            }
        }
        for (var i = 0; i < count; i++) {
            var timeMarkerLine = this.timeMarkerLineCollection[i];
            timeMarkerLine.style.position = "absolute";
            this.LayoutTimeIndicatorLine(timeMarkerLine, timeIndicatorViewInfos[i]);
            ASPx.SetElementDisplay(timeMarkerLine, true)
        }
    },
    LayoutTimeIndicatorLine: function (timeMarkerLine, todayCell) {
        // do nothing
    },
    LayoutExtraItems: function (time) {
        // do nothing
    },
    CreateTodayTimeMarkerLine: function () {
        var timeMarkerLine = this.timeMarkerLineSource.cloneNode(true);
        ASPx.SetElementDisplay(timeMarkerLine, false);
        this.viewInfo.parent.AppendChildToLayer(timeMarkerLine, this.layer);
        return timeMarkerLine;
    }
});
var VerticalCurrentTimeContainerViewInfo = ASPx.CreateClass(CurrentTimeContainerViewInfoBase, {
    CalculateLayerX: function(containerTime, cell) {
        var cellInterval = this.scheduler.GetCellInterval(cell);
        var cellDuration = cellInterval.duration;
        var cellWidth = cell.offsetWidth;
        var remainderDuration = ASPx.SchedulerGlobals.DateSubsWithTimezone(containerTime, cellInterval.GetStart());
        return cell.offsetLeft + cellWidth * remainderDuration / cellDuration;       
    },
    LayoutTimeIndicatorLine: function (timeMarkerLine, timeIndicatorViewInfo) {
        var firstCell = timeIndicatorViewInfo.GetFirstCell();
        var lastCell = timeIndicatorViewInfo.GetLastCell();
        timeMarkerLine.style.top = firstCell.offsetTop + "px";
        timeMarkerLine.style.height = lastCell.offsetTop - firstCell.offsetTop + lastCell.offsetHeight + "px";
    },
    FindBaseCellForLayer: function(container, time) {
        return this.viewInfo.FindStartCellByTime(container, time);
    }
});
var HorizontalCurrentTimeContainerViewInfo = ASPx.CreateClass(CurrentTimeContainerViewInfoBase, {
    constructor: function (scheduler, viewInfo, timeMarkerImageSource, timeMarkerLineSource, layer) {
        this.constructor.prototype.constructor.call(this, scheduler, viewInfo, timeMarkerLineSource, layer);
        this.timeMarkerImageSource = timeMarkerImageSource;
    },
    Dispose: function () {
        this.constructor.prototype.Dispose(this);
        if (!this.timeMarkerImage) {
            var timeRulers = this.scheduler.GetTimeRulers();
            if (timeRulers && timeRulers.length < 1) {
                var timeRulerCount = timeRulers.length;
                for (var i = 0; i < timeRulerCount; i++)
                    ASPx.SchedulerGlobals.RecycleNode(this.timeMarkerImage[i]);
            }
            this.timeMarkerImage = null;
        }
    },
    CalculateLayerY: function (time, cell, container) {
        var adaptedContainerTime = this.viewInfo.GetAdaptedContainerTime(container, time);
        var cellInterval = this.scheduler.GetCellInterval(cell);
        var cellDuration = cellInterval.duration;
        var cellHeight = cell.offsetHeight;
        var remainderDuration = ASPx.SchedulerGlobals.DateSubsWithTimezone(adaptedContainerTime, cellInterval.GetStart());
        return cell.offsetTop + cellHeight * remainderDuration / cellDuration;
    },
    LayoutTimeIndicatorLine: function (timeMarkerLine, timeIndicatorViewInfo) {
        var firstCell = timeIndicatorViewInfo.GetFirstCell();
        var lastCell = timeIndicatorViewInfo.GetLastCell();
        timeMarkerLine.style.left = firstCell.offsetLeft + "px";
        timeMarkerLine.style.width = lastCell.offsetLeft - firstCell.offsetLeft + lastCell.offsetWidth + "px";
    },
    LayoutExtraItems: function (time) {
        this.LayoutTimeMarkerImage(time);
    },
    LayoutTimeMarkerImage: function (time) {
        var timeRulers = this.scheduler.GetTimeRulers();
        if (!timeRulers || timeRulers.length < 1)
            return;
        var timeRulerCount = timeRulers.length;
        if (!this.timeMarkerImage) {
            this.timeMarkerImage = [];
            for (var i = 0; i < timeRulerCount; i++)
                this.timeMarkerImage.push(this.CreateTimeMarkerImage());
        }
        var timeMarkerLayer = this.scheduler.verticalParent.timeMarkerLayer;
        for (var i = 0; i < timeRulerCount; i++)
            this.timeMarkerImage[i].style.position = "absolute";
        for (var i = 0; i < timeRulerCount; i++) {
            var timeRuler = timeRulers[i];
            var rulerCell = timeRuler.GetAnchorCell(this.scheduler);
            var posX = rulerCell.offsetLeft;
            this.timeMarkerImage[i].style.left = posX + "px";
            ASPx.SetElementDisplay(this.timeMarkerImage[i], timeRuler.IsVisible());
        }
    },
    CreateTimeMarkerImage: function () {
        var timeMarkerImage = this.timeMarkerImageSource.cloneNode(true);
        timeMarkerImage.id = this.timeMarkerImageSource.id + "_clone";
        ASPx.SetElementDisplay(timeMarkerImage, false);
        this.viewInfo.parent.AppendChildToLayer(timeMarkerImage, this.layer);
        return timeMarkerImage;
    },
    FindBaseCellForLayer: function (container, time) {
        return this.viewInfo.FindStartCellByTimeOfDay(container, time);
    }
});

var TimeIndicatorViewInfo = ASPx.CreateClass(null, {
    constructor: function (viewInfo) {
        this.containers = [];
        this.viewInfo = viewInfo;
    },
    Add: function (container) {
        this.containers.push(container);
    },
    GetFirstCell: function () {
        return this.viewInfo.GetCell(this.containers[0].index, 0);
    },
    GetLastCell: function () {
        var count = this.containers.length;
        return this.viewInfo.GetCell(this.containers[count - 1].index, 0);
    }
});

var CellHighlightViewInfo = ASPx.CreateClass(null, {//abstract class
    constructor: function (scheduler, viewInfo, divTemplate, layer) {
        this.scheduler = scheduler;
        this.viewInfo = viewInfo;
        this.parent = viewInfo.parent;
        this.divTemplate = divTemplate;
        this.highlightDivs = [];

        this.highlightElements = [];
        this.highlightDivCache = [];
        this.layer = layer;
    },
    GetHightlightElementFromCell: function(cell) {
        var count = this.highlightElements.length;
        var startTime = this.scheduler.GetCellStartTime(cell);
        var endTime = this.scheduler.GetCellEndTime(cell);
        var container = this.scheduler.GetCellContainer(cell);
        if (!ASPx.IsExists(startTime) || !ASPx.IsExists(endTime) || !ASPx.IsExists(container))
            return null;
        for (var i = 0; i < count; i++) {
            var highlightElement = this.highlightElements[i];
            if (highlightElement.container != container)
                continue;
            var highlightInterval = highlightElement.interval;
            if (startTime.valueOf() == highlightInterval.GetStart().valueOf() && endTime.valueOf() == highlightInterval.GetEnd().valueOf()) {
                return this.highlightDivs[i];
            }
        }
        return null;
    },
    HighlightCells: function (interval, resource, highlightPartiallySelectedCell, selectionVisible) {
        if (!ASPx.IsExists(interval) || !ASPx.IsExists(resource))
            return;
        this.RemovePrevCellHighlight();
        if (selectionVisible) {
            this.CreateHighlightElements(interval, resource, highlightPartiallySelectedCell);
            this.CreateHighlightDivs();
        }
    },
    RemovePrevCellHighlight: function () {
        var count = this.highlightDivs.length;
        for (var i = 0; i < count; i++) {
            this.ReleaseHighlightDiv(this.highlightDivs[i]);
        }
        ASPx.Data.ArrayClear(this.highlightDivs);
        ASPx.Data.ArrayClear(this.highlightElements);
    },
    CreateHighlightElements: function (interval, resource, highlightPartiallySelectedCell) {
        var cellContainers = this.viewInfo.cellContainers;
        var count = cellContainers.length;
        var viewInfo = this.viewInfo;
        for (var i = 0; i < count; i++) {
            var container = cellContainers[i];
            if (container.resource != resource || !container.interval.IntersectsWithExcludingBounds(interval))
                continue;
            if (interval.Contains(container.interval)) {
                this.HighlightContainer(container);
                continue;
            }
            var cellCount = container.cellCount;
            for (var j = 0; j < cellCount; j++) {
                var cell = this.viewInfo.GetCell(i, j);
                if (ASPx.IsExists(cell)) {
                    var cellInterval = this.scheduler.GetCellInterval(cell);
                    var highlightCell;
                    if (highlightPartiallySelectedCell)
                        highlightCell = cellInterval.IntersectsWithExcludingBounds(interval);
                    else
                        highlightCell = interval.Contains(cellInterval);
                    if (highlightCell)
                        this.HighlightCell(cell);
                }
            }
        }
    },
    CreateHighlightDivs: function () {
        var count = this.highlightElements.length;
        for (var i = 0; i < count; i++) {
            var element = this.highlightElements[i];
            var highlightDiv = this.CreateHighlightDiv(element);
            if (ASPx.IsExists(this.scheduler.selectionToolTip)) {
                //ASPx.SubscribeSchedulerMouseEvents(highlightDiv, highlightDiv);
                //highlightDiv.mouseEvents = new ASPx.SchedulerMouseEvents();
                highlightDiv.toolTip = this.scheduler.selectionToolTip;
                highlightDiv.viewInfo = highlightDiv;
                //var enterLeaveHelper = new ASPx.MouseEnterLeaveHelper(appointmentDiv);
                //enterLeaveHelper.Attach(this.OnMouseEnter.aspxBind(this), this.OnMouseLeave.aspxBind(this));
                //ASPx.AddToolTip(highlightDiv, this.scheduler.selectionToolTip);
            }
            this.highlightDivs.push(highlightDiv);
        }
    },
    CreateHighlightDiv: function (highlightElement) {
        var highlightDiv = this.CreateHightlightDiv();
        highlightDiv.style.height = "0px";
        highlightDiv.container = highlightElement.container;
        highlightDiv.style.left = highlightElement.left + "px";
        highlightDiv.style.top = highlightElement.top + "px";
        highlightDiv.style.width = highlightElement.width + "px";
        highlightDiv.style.height = highlightElement.height + "px";
        ASPx.SetElementDisplay(highlightDiv, true);
        highlightDiv.style.display = "inline";

        return highlightDiv;
    },
    CreateHightlightDiv: function () {
        if (this.highlightDivCache.length > 0)
            return this.highlightDivCache.pop();
        else {
            var highlightDiv = this.divTemplate.cloneNode(true);
            highlightDiv.id = this.divTemplate.id + "_clone";
            highlightDiv.oncontextmenu = highlightDiv;
            ASPx.SetElementDisplay(highlightDiv, false);
            this.viewInfo.parent.AppendChildToLayer(highlightDiv, this.layer); //!!!!!
            return highlightDiv;
        }
    },
    ReleaseHighlightDiv: function (div) {
        this.highlightDivCache.push(div);
        ASPx.SetElementDisplay(div, false);
        //ASPx.UnsubscribeSchedulerMouseEvents(div, div) {
    },
    HighlightContainer: function (container) {
        if (!this.IsValidContainer(container))
            return;
        var bounds = this.parent.CalcRelativeContainerBounds(this.viewInfo, container);
        if (!ASPx.IsExists(bounds))
            return;
        var left = bounds.left;
        var dxtop = bounds.top;
        var width = bounds.width;
        var height = bounds.height;
        var newHighlightElement = new CellHighlightElement(container, left, dxtop, width, height, container.interval);
        this.highlightElements.push(newHighlightElement);
    },
    HighlightCell: function (cell) {
        var findResult = this.FindAdjacentHighlightElement(cell);
        if (ASPx.IsExists(findResult)) {
            var highlightElement = findResult.highlightElement;
            if (ASPx.IsExists(findResult.newLeft))
                highlightElement.left = findResult.newLeft;
            if (ASPx.IsExists(findResult.newTop))
                highlightElement.top = findResult.newTop;
            if (ASPx.IsExists(findResult.newRight))
                highlightElement.width = findResult.newRight - highlightElement.left;
            if (ASPx.IsExists(findResult.newBottom))
                highlightElement.height = findResult.newBottom - highlightElement.top;
            if (ASPx.IsExists(findResult.newStartTime))
                highlightElement.interval.SetStart(findResult.newStartTime);
            if (ASPx.IsExists(findResult.newEndTime))
                highlightElement.interval.SetEnd(findResult.newEndTime);
        }
        else {
            var container = this.scheduler.GetCellContainer(cell);
            if (!this.IsValidContainer(container))
                return;
            var left = this.parent.CalcRelativeElementLeft(cell);
            var dxtop = this.parent.CalcRelativeElementTop(cell);
            var width = cell.offsetWidth;
            var height = this.parent.CalcCellHeight(cell);
            var startTime = this.scheduler.GetCellStartTime(cell);
            var endTime = this.scheduler.GetCellEndTime(cell);
            if (!ASPx.IsExists(container) || !ASPx.IsExists(left) || !ASPx.IsExists(dxtop) || !ASPx.IsExists(width) || !ASPx.IsExists(height) || !ASPx.IsExists(startTime) || !ASPx.IsExists(endTime))
                return;
            var interval = new ASPxClientTimeInterval(startTime, ASPx.SchedulerGlobals.DateSubsWithTimezone(endTime, startTime));
            var newHighlightElement = new CellHighlightElement(container, left, dxtop, width, height, interval);
            this.highlightElements.push(newHighlightElement);
        }
    },
    FindAdjacentHighlightElement: function (cell) {
        var count = this.highlightElements.length;
        var startTime = this.scheduler.GetCellStartTime(cell);
        var endTime = this.scheduler.GetCellEndTime(cell);
        var container = this.scheduler.GetCellContainer(cell);
        if (!ASPx.IsExists(startTime) || !ASPx.IsExists(endTime) || !ASPx.IsExists(container))
            return null;
        for (var i = 0; i < count; i++) {
            var highlightElement = this.highlightElements[i];
            if (highlightElement.container != container)
                continue;
            var highlightInterval = highlightElement.interval;
            var cellBeforeHighlight = (highlightInterval.GetStart() - endTime) == 0;
            var cellAfterHighlight = (highlightInterval.GetEnd() - startTime) == 0;
            if (!cellBeforeHighlight && !cellAfterHighlight)
                continue;
            var result = {};
            if (cellBeforeHighlight)
                result.newStartTime = startTime;
            if (cellAfterHighlight)
                result.newEndTime = endTime;


            if (this.IsVisuallyAdjacent(highlightElement, cell, result)) {
                result.highlightElement = highlightElement;
                return result;
            }
        }
        return null;
    }
});

var HorizontalCellHighlightViewInfo = ASPx.CreateClass(CellHighlightViewInfo, {
    constructor: function(scheduler, viewInfo, divTemplate, layer) {
        this.constructor.prototype.constructor.call(this, scheduler, viewInfo, divTemplate, layer);
    },
    IsValidContainer: function(container) {
        return ASPx.IsExists(container.isVertical) && (!container.isVertical);
    },
    IsVisuallyAdjacent: function (highlightElement, cell, result) {
        
        var cellTop = this.parent.CalcRelativeElementTop(cell);
        var cellHeight = this.parent.CalcCellHeight(cell) - 1;
        if(cellTop == highlightElement.top && cellHeight == highlightElement.height) {
            var cellLeft = this.parent.CalcRelativeElementLeft(cell);
            result.newLeft = Math.min(cellLeft, highlightElement.left);
            result.newRight = Math.max(this.parent.CalcRelativeElementRight(cell), highlightElement.left + highlightElement.width);                
            return true;
        }
    }
});
var VerticalCellHighlightViewInfo = ASPx.CreateClass(CellHighlightViewInfo, {
    constructor: function(scheduler, viewInfo, divTemplate, layer) {
        this.constructor.prototype.constructor.call(this, scheduler, viewInfo, divTemplate, layer);
    },
    IsValidContainer: function(container) {
        return ASPx.IsExists(container.isVertical) && container.isVertical;
    },
    IsVisuallyAdjacent: function(highlightElement, cell, result) {
        var cellLeft = this.parent.CalcRelativeElementLeft(cell);
        if(cellLeft == highlightElement.left && cell.offsetWidth == highlightElement.width) {
            var cellTop = this.parent.CalcRelativeElementTop(cell);
            result.newTop = Math.min(cellTop, highlightElement.top);
            result.newBottom = Math.max(this.parent.CalcRelativeElementBottom(cell), highlightElement.top + highlightElement.height);                
            return true;
        }
    }    
});

ASPx.SchedulerSelection = SchedulerSelection;
ASPx.SchedulerSelectionHelper = SchedulerSelectionHelper;
ASPx.AppointmentSelection = AppointmentSelection;
ASPx.VerticalCurrentTimeContainerViewInfo = VerticalCurrentTimeContainerViewInfo;
ASPx.HorizontalCurrentTimeContainerViewInfo = HorizontalCurrentTimeContainerViewInfo;
ASPx.HorizontalCellHighlightViewInfo = HorizontalCellHighlightViewInfo;
ASPx.VerticalCellHighlightViewInfo = VerticalCellHighlightViewInfo;
})();