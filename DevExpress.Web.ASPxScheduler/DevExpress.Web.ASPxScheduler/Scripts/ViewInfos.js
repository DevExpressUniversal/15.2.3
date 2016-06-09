(function() {
ASPx.SchedulerMoreButtonId = "moreButton";

var ClientCellDurationIterator = ASPx.CreateClass(null, {
    constructor: function(durations) {
        this.durations = durations;
        this.start();
    },
    start: function() {
        this.currentDurationIndex = -1;
        this.restDurationCount = 0;
    },
    getNextDuration: function() {
        if(this.restDurationCount == 0) {
            this.currentDurationIndex++;
            this.restDurationCount = this.durations[2 * this.currentDurationIndex];
            this.currentDuration = this.durations[2 * this.currentDurationIndex + 1];
        }
        this.restDurationCount--;
        return this.currentDuration;
    }
});
////////////////////////////////////////////////////////////////////////////////
// SchedulerContainer
////////////////////////////////////////////////////////////////////////////////
var SchedulerContainer = ASPx.CreateClass(null, {
    constructor: function (containerIndex, index, startTime, cellCount, cellDurationInfos, resource, isVertical, cellsLocation, middleCompressedCellsHeaderLocation) {
        this.index = index;
        this.cellCount = cellCount;
        this.cellDurationInfos = cellDurationInfos;
        this.containerIndex = containerIndex;
        this.resource = resource;
        this.interval = new ASPxClientTimeInterval(startTime, this.GetTotalContainerDuration(cellDurationInfos));
        this.isVertical = isVertical;
        this.privateCellsLocation = cellsLocation;
        this.cellsLocation = this.ExpandCellsLocation(cellsLocation);
        if (middleCompressedCellsHeaderLocation)
            this.middleCompressedCellsHeaderLocation = this.ExpandCellLocation(middleCompressedCellsHeaderLocation);
        this.initializedCells = [];
    },
    GetTotalContainerDuration: function (cellDurationInfos) {
        this.cellTimes = [];
        var count = cellDurationInfos.length;
        var totalDuration = 0;
        for (var i = 0; i < count; i += 2) {
            this.cellTimes.push(totalDuration);
            totalDuration += cellDurationInfos[i] * cellDurationInfos[i + 1];
        }
        this.cellTimes.push(totalDuration);
        return totalDuration;
    },
    ExpandCellsLocation: function (cellsLocation) {
        var result = [];
        var count = cellsLocation.length;
        for (var i = 0; i < count; i++) {
            this.ExpandCellsLocationCore(cellsLocation[i], result);
        }
        return result;
    },
    ExpandCellsLocationCore: function (cellsLocation, result) {
        var count = cellsLocation[0];
        for (var i = 0; i < count; i++) {
            var position = this.ExpandCellLocation(cellsLocation);
            if (this.isVertical)
                position.row += i;
            else
                position.column += i;
            result.push(position);
        }
    },
    ExpandCellLocation: function (cellLocation) {
        var row = cellLocation[1];
        var column = cellLocation[2];

        var position = {};
        position.row = cellLocation[1];
        position.column = cellLocation[2]
        position.isCompressed = (cellLocation[3]) ? true : false;
        return position;
    },
    GetCellLocation: function (cellIndex) {
        return this.cellsLocation[cellIndex];
    },
    GetMiddleCompressedCellsHeaderLocation: function () {
        return this.middleCompressedCellsHeaderLocation;
    },
    InitializeCell: function (cell, cellIndex) {
        var cellDurations = this.cellDurationInfos;
        var time = this.interval.GetStart();
        var durationIndex = 0;
        var duration;
        while (cellIndex >= cellDurations[durationIndex]) {
            var cellCount = cellDurations[durationIndex];
            if (cellCount > 0) {
                cellIndex -= cellCount;
                duration = cellDurations[durationIndex] * cellDurations[durationIndex + 1];
            }
            else
                duration = -cellDurations[durationIndex + 1];
            time = ASPx.SchedulerGlobals.DateIncreaseWithUtcOffset(time, duration);

            durationIndex += 2;
        }
        duration = cellDurations[durationIndex + 1];
        var cellStart = ASPx.SchedulerGlobals.DateIncreaseWithUtcOffset(time, cellIndex * duration);
        var cellEnd = ASPx.SchedulerGlobals.DateIncreaseWithUtcOffset(cellStart, duration);
        cell.interval = new ASPxClientTimeInterval(cellStart, duration);

        cell.resource = this.resource;
        cell.container = this;
        this.initializedCells.push(cell);
    },
    ClearInitializedCells: function () {
        var count = this.initializedCells.length;
        for (var i = 0; i < count; i++) {
            var cell = this.initializedCells[i];
            cell.interval = null;
            cell.resource = null;
            cell.container = null;
        }
    }
});
////////////////////////////////////////////////////////////////////////////////
// SchedulerViewInfo 
////////////////////////////////////////////////////////////////////////////////
var SchedulerViewInfo = ASPx.CreateClass(null, {
    constructor: function (scheduler, prevViewInfo, usePrevCache) {
        this.scheduler = scheduler;
        this.appointmentViewInfos = [];
        this.cellContainers = [];
        this.isVertical = null;
        if (ASPx.IsExists(prevViewInfo) && usePrevCache) {
            this.ClearPrevContainers(prevViewInfo.cellContainers);
            this.cachedCells = prevViewInfo.cachedCells;
        }
        else {
            this.cachedCells = null;
        }
    },
    ClearPrevContainers: function (containers) {
        var count = containers.length;
        for (var i = 0; i < count; i++) {
            containers[i].ClearInitializedCells();
        }
    },
    Initialize: function (table) {
        this.table = table;
        if (!this.cachedCells)
            this.cachedCells = {};
    },
    Dispose: function () {
        var count = this.appointmentViewInfos.length;
        for (var i = 0; i < count; i++)
            this.DisposeAppointmentViewInfo(this.appointmentViewInfos[i]);

    },
    DisposeAppointmentViewInfo: function (appointmentViewInfo) {
        var appointmentDiv = appointmentViewInfo.contentDiv;
        if (!ASPx.IsExists(appointmentDiv))
            return;
        appointmentViewInfo.Dispose();
        appointmentDiv.viewInfo = null;
    },
    Prepare: function (parent) {
        this.parent = parent;
        var count = this.appointmentViewInfos.length;
        parent.PrepareAppointmentLayer(count);
        for (var i = 0; i < count; i++) {
            var viewInfo = this.appointmentViewInfos[i];

            var div = this.scheduler.GetAppointmentDivById(viewInfo.divId);
            if (!ASPx.IsExists(div))
                continue;
            viewInfo.contentDiv = div;
            viewInfo.contentDiv.appointmentId = viewInfo.appointmentId;
            div.appointmentViewInfo = viewInfo;
            if (div.parentNode != parent.appointmentLayer)
                parent.AppendChildToAppointmentLayer(div); //!!!!!
        }
        this.selectionViewInfo = this.CreateSelectionViewInfoCore();
    },
    GetAdaptedContainerTime: function (cellContainer, dateTime) {
        var containerStart = cellContainer.interval.GetStart();
        var dayTimeDelta = ASPx.SchedulerGlobals.DateSubsWithTimezone(dateTime, containerStart);
        var dateTimeDuration = Math.abs(dayTimeDelta);
        var dayTime = dateTimeDuration % (24 * 60 * 60 * 1000); //
        if (dayTimeDelta < 0)
            dayTime = 24 * 60 * 60 * 1000 - dayTime;
        var containerTime = ASPx.SchedulerGlobals.DateIncreaseWithUtcOffset(containerStart, dayTime);
        return containerTime;
    },
    HideAppointments: function () {
        var count = this.appointmentViewInfos.length;
        for (var i = 0; i < count; i++) {
            if (this.appointmentViewInfos[i].contentDiv != null)
                ASPx.SetSchedulerDivDisplay(this.appointmentViewInfos[i].contentDiv, false);
        }
    },
    ShowCellSelection: function (interval, resource, highlightPartiallySelectedCell, selectionVisible) {
        if (ASPx.IsExists(interval) && ASPx.IsExists(resource))
            this.selectionViewInfo.HighlightCells(interval, resource, highlightPartiallySelectedCell, selectionVisible);
    },
    GetHightlightElementFromCell: function (cell) {
        return this.selectionViewInfo.GetHightlightElementFromCell(cell);
    },
    AddCellContainer: function (containerIndex, cellCount, containerStartTime, cellDurationInfos, resource, cellsLocations, middleCompressedCellsHeaderLocation) {
        var container = new SchedulerContainer(containerIndex, this.cellContainers.length, containerStartTime, cellCount, cellDurationInfos, resource, this.isVertical, cellsLocations, middleCompressedCellsHeaderLocation);
        this.cellContainers.push(container);
    },
    InitializeCell: function (cell, containerIndex, cellIndex) {
        this.cellContainers[containerIndex].InitializeCell(cell, cellIndex);
    },
    //GetAppointmentDivById: function(divId) {
    //    return GetAppointmentDivById(divId);
    //return this.scheduler.appointmentDivCache[divId];
    //return this.scheduler.GetAppointmentBlockElementById(divId);
    //},
    GetCellById: function (id) {
        //return this.scheduler.GetElementById(id);
        return this.cachedCells[id];
        /*var result = this.cachedCells[id];
        if(!ASPx.IsExists(result)) {
        result = this.scheduler.GetContainerElementById(id);
        this.cachedCells[id] = result;                   
        return result;
        }
        else
        return result;*/
    },
    GetCell: function (containerIndex, index) {
        //var loc = this.cellContainers[containerIndex].GetCellLocation(index);
        //return result = this.table.rows[loc.row].cells[loc.column];       

        var hashCode = (containerIndex << 16) + index;
        var result = this.cachedCells[hashCode];
        if (!ASPx.IsExists(result)) {
            var container = this.cellContainers[containerIndex];
            if (!ASPx.IsExists(container))
                return null;
            var loc = container.GetCellLocation(index);
            if (ASPx.IsExists(loc)) {
                result = this.table.rows[loc.row].cells[loc.column];
                //result = this.GetCellById(this._constCellPrefix() + containerIndex + "_" + index);
                this.cachedCells[hashCode] = result;
            }
        }
        return result;
    },
    GetMiddleCompressedCellsHeader: function (containerIndex) {
        var container = this.cellContainers[containerIndex];
        var loc = container.GetMiddleCompressedCellsHeaderLocation();
        if (!loc)
            return null;
        return this.table.rows[loc.row].cells[loc.column];
    },
    FindCellContainerByTimeInterval: function (start, end) {
        //TODO: for group by resource it works incorrect 
        var interval = new ASPxClientTimeInterval(start, ASPx.SchedulerGlobals.DateSubsWithTimezone(end, start));
        var index = ASPx.Data.ArrayBinarySearch(this.cellContainers, interval, this.ContainerWithIntervalComparer);
        if (index < 0)
            return null;
        else
            return this.cellContainers[index];
    },
    FindCellContainersByTimeInterval: function (start, end) {
        var interval = new ASPxClientTimeInterval(start, ASPx.SchedulerGlobals.DateSubsWithTimezone(end, start));
        var result = [];
        var count = this.cellContainers.length;
        for (var i = 0; i < count; i++) {
            var container = this.cellContainers[i];
            if (this.ContainerWithIntervalComparer(this.cellContainers, i, interval) == 0)
                result.push(container);
        }
        return result;
    },
    FindStartCellIndexByTime: function (cellContainer, time) {
        //TODO: rewrite
        var start = cellContainer.interval.GetStart();
        var durations = cellContainer.cellDurationInfos;
        var count = durations.length;
        var cellCount = 0;
        /*if(time.getDay() == 2)
        debugger_;*/
        /*if(count > 2)
        debugger_;*/
        var i = 0;
        for (; i < count - 2; i += 2) {
            var duration = durations[i] * durations[i + 1];
            if (duration > 0) {
                if ((start - time) + duration > 0) {//start+duration > time
                    break;
                }
                cellCount += durations[i];
            }
            else {
                duration = -durations[i + 1];
                if (start - time + duration > 0)
                    return cellCount;
            }
            start = ASPx.SchedulerGlobals.DateIncrease(start, duration);
        }
        var dif = ASPx.SchedulerGlobals.DateSubsWithTimezone(time, start); /* + (start.getTimezoneOffset() - time.getTimezoneOffset()) * 60000;*/
        var rem = dif % durations[i + 1];
        //alert("start" + time + " " + (dif - rem) / durations[i + 1] + cellCount);
        return (dif - rem) / durations[i + 1] + cellCount;
    },
    FindStartCellByTime: function (cellContainer, time) {
        return this.GetCell(cellContainer.index, this.FindStartCellIndexByTime(cellContainer, time));
    },
    FindEndCellIndexByTime: function (cellContainer, time) {//TODO: rewrite
        var start = cellContainer.interval.GetStart();
        var durations = cellContainer.cellDurationInfos;
        var count = durations.length;
        var cellCount = 0;
        /*if(count > 2)
        debugger_;*/
        var i = 0;
        for (; i < count - 2; i += 2) {
            var duration = durations[i] * durations[i + 1];
            if (duration > 0) {
                if ((start - time) + duration > 0) {//start+duration > time
                    break;
                }
                cellCount += durations[i];
            }
            else {
                duration = -durations[i + 1];
                if (start - time + duration > 0)
                    return cellCount - 1;
            }
            start = ASPx.SchedulerGlobals.DateIncrease(start, duration);
        }
        var dif = ASPx.SchedulerGlobals.DateSubsWithTimezone(time, start); /*+ (start.getTimezoneOffset() - time.getTimezoneOffset()) * 60000;*/
        var rem = dif % durations[i + 1];
        var cellIndex = (dif - rem) / durations[i + 1] + cellCount;

        if (rem == 0 && cellIndex > 0)
            cellIndex--;
        return Math.min(cellIndex, cellContainer.cellCount - 1);
    },
    FindEndCellByTime: function (cellContainer, time) {
        return this.GetCell(cellContainer.index, this.FindEndCellIndexByTime(cellContainer, time));
    },
    ContainerWithIntervalComparer: function (array, index, interval) {
        var container = array[index];
        var conteinerInterval = container.interval;
        if (conteinerInterval.GetStart() > interval.GetEnd())
            return 1;
        if (conteinerInterval.GetEnd() < interval.GetStart())
            return -1;
        return 0;
    },
    AddViewInfo: function (viewInfo) {
        this.appointmentViewInfos.push(viewInfo);
    },
    RemoveViewInfo: function (viewInfo) {
        ASPx.Data.ArrayRemove(this.appointmentViewInfos, viewInfo);
    },
    FindAppointmentViewInfoByPosition: function (x, y) {
        var count = this.appointmentViewInfos.length;
        for (var i = 0; i < count; i++) {
            var viewInfo = this.appointmentViewInfos[i];
            var aptDiv = viewInfo.contentDiv;
            var aptX = ASPx.GetAbsoluteX(aptDiv);
            var aptY = ASPx.GetAbsoluteY(aptDiv);
            var aptHeight = aptDiv.clientHeight;
            var aptWidth = aptDiv.clientWidth;
            if ( (aptX < x && x < (aptX + aptWidth)) && (aptY < y && y < (aptY + aptHeight)) )
                return viewInfo;
        }
        return null;
    }, 
    FindCellByPosition: function (containerIndex, firstCellIndex, lastCellIndex, x, y) {
        if (ASPx.IsExists(containerIndex))
            return this.FindCellByPositionCore(containerIndex, firstCellIndex, lastCellIndex, x, y);
        var count = this.cellContainers.length;
        for (var i = 0; i < count; i++) {
            var container = this.cellContainers[i];
            var lastCellIndex = container.cellCount - 1;
            if (!this.IsPointInsideContainer(i, 0, lastCellIndex, x, y))
                continue;
            var result = this.FindCellByPositionCore(i, 0, lastCellIndex, x, y);
            if (ASPx.IsExists(result))
                return result;
        }
        return null;
    },
    FindCellByPositionSlow: function (x, y) {
        var count = this.cellContainers.length;
        for (var i = 0; i < count; i++) {
            var container = this.cellContainers[i];
            var lastCellIndex = container.cellCount - 1;
            if (!this.IsPointInsideContainer(i, 0, lastCellIndex, x, y))
                continue;
            var result = this.FindCellByPositionCore(i, 0, lastCellIndex, x, y);
            if (ASPx.IsExists(result))
                return result;
        }
        return null;
    },
    IsPointInsideContainer: function (containerIndex, firstCellIndex, lastCellIndex, x, y) {
        var firstCell = this.GetCell(containerIndex, firstCellIndex);
        var lastCell = this.GetCell(containerIndex, lastCellIndex);
        if (!ASPx.IsExists(firstCell) || !ASPx.IsExists(lastCell))
            return false;
        var dxtop = this.parent.CalcRelativeElementTop(firstCell);
        if (y < dxtop)
            return false;
        var bottom = this.parent.CalcRelativeElementBottom(lastCell);
        if (y > bottom)
            return false;
        var left = this.parent.CalcRelativeElementLeft(firstCell);
        if (x < left)
            return false;
        var right = this.parent.CalcRelativeElementRight(lastCell);
        if (x > right)
            return false;
        return true;
    },
    FindCellByPositionCore: function (containerIndex, firstCellIndex, lastCellIndex, x, y) {
        var firstCell = this.GetCell(containerIndex, firstCellIndex);
        var lastCell = this.GetCell(containerIndex, lastCellIndex);
        if (!ASPx.IsExists(firstCell) || !ASPx.IsExists(lastCell))
            return null;
        if (this.CompareCellAndPosition(x, y, firstCell) < 0) //position before first cell
            return null;
        if (this.CompareCellAndPosition(x, y, lastCell) > 0) //position after last cell
            return null;

        while (firstCellIndex <= lastCellIndex) {
            var cellIndex = (firstCellIndex + lastCellIndex) >> 1;
            var cell = this.GetCell(containerIndex, cellIndex);
            var result = this.CompareCellAndPosition(x, y, cell);
            if (result == 0)
                return cell;
            if (result < 0)
                lastCellIndex = cellIndex - 1;
            else
                firstCellIndex = cellIndex + 1;
        }
        return null;
    },
    FindViewInfosByAppointmentId: function (appointmentId) {
        var result = [];
        var count = this.appointmentViewInfos.length;
        for (var i = 0; i < count; i++) {
            var viewInfo = this.appointmentViewInfos[i];
            if (viewInfo.appointmentId == appointmentId)
                result.push(viewInfo);
        }
        return result;
    },
    RemoveViewInfosByAppointmentId: function (appointmentId) {
        var viewInfos = this.FindViewInfosByAppointmentId(appointmentId);
        var count = viewInfos.length;
        for (var i = 0; i < count; i++) {
            var viewInfo = viewInfos[i];
            this.RemoveViewInfo(viewInfo);
            ASPx.SchedulerGlobals.RecycleNode(viewInfo.contentDiv);
            viewInfo.contentDiv = null;
        }
    }    
});

var MoreButtonHelper = ASPx.CreateClass(null, {
    constructor: function (schedulerViewInfo) {
        this.schedulerViewInfo = schedulerViewInfo;
        this.scheduler = schedulerViewInfo.scheduler;
        this.moreButtonParent = this.scheduler.containerCell;
        this.scrollContainer = this.schedulerViewInfo.parent.parentElement.parentNode;
        this.moreButtonContainers = this.CreateMoreButtonContainers(this.scrollContainer.scrollHeight);
        this.horizontalOffset = 2;
        this.verticalOffset = 2;
        this.subscribedElements = [];
    },
    Dispose: function () {
        var count = this.subscribedElements.length;
        for (var i = 0; i < count; i++) {
            var elem = this.subscribedElements[i];
            ASPx.Evt.DetachEventFromElement(elem[0], elem[1], elem[2]);
        }        
        this.subscribedElements = [];
    },
    CalculateMoreButtons: function () {

        var visibleTop = this.scrollContainer.scrollTop;
        var visibleBottom = visibleTop + this.scrollContainer.offsetHeight;
        var appointmentViewInfos = this.schedulerViewInfo.appointmentViewInfos;
        var count = appointmentViewInfos.length;
        this.ResetMoreButtonContainers();
        for (var i = 0; i < count; i++) {
            var viewInfo = appointmentViewInfos[i];
            var div = viewInfo.contentDiv;
            if (div == null)
                continue;
            var dxtop = div.offsetTop;
            var bottom = dxtop + div.offsetHeight;

            var moreButtonContainerObject = this.GetMoreButtonContainer(viewInfo);
            var delta = 1;
            if (ASPx.Browser.Firefox)
                delta = 5;
            if ((bottom - delta) <= visibleTop) {
                if (dxtop >= moreButtonContainerObject.nearestTopAppointmentPosition) {
                    moreButtonContainerObject.nearestTopAppointmentPosition = dxtop;
                    moreButtonContainerObject.nearestTopAppointmentViewInfo = viewInfo;
                }
                continue;
            }
            if ((dxtop + delta) >= visibleBottom) {
                if (bottom <= moreButtonContainerObject.nearestBottomAppointmentPosition) {
                    moreButtonContainerObject.nearestBottomAppointmentPosition = bottom;
                    moreButtonContainerObject.nearestBottomAppointmentViewInfo = viewInfo;
                }
                continue;
            }
        }
        this.DisplayMoreButtons();
    },
    DisplayMoreButtons: function () {
        var count = this.moreButtonContainers.length;
        for (var i = 0; i < count; i++) {
            this.DisplayMoreButtonsCore(this.moreButtonContainers[i]);
        }
    },
    ResetMoreButtonContainers: function () {
        var count = this.moreButtonContainers.length;
        for (var i = 0; i < count; i++)
            this.moreButtonContainers[i].Reset(this.scrollContainer.scrollHeight);
    },
    DisplayMoreButtonsCore: function (moreButtonContainer) {
        var scrollContainer = this.scrollContainer;
        var moreButtonParent = this.moreButtonParent;
        if (ASPx.IsExists(moreButtonContainer.nearestTopAppointmentViewInfo)) {
            var div = this.GetDivForContainer(moreButtonContainer, 0);
            if (!ASPx.IsExists(div))
                return;
            div.targetAppointmentViewInfo = moreButtonContainer.nearestTopAppointmentViewInfo;
            var appointment = this.scheduler.GetAppointment(div.targetAppointmentViewInfo.appointmentId);
            div.targetDateTime = appointment.interval.GetStart();
            //debugger_;
            // div.interval = null; new ASPxClientTimeInterval(new Date(0), 0);
            var top = ASPx.GetAbsoluteY(scrollContainer) - ASPx.GetAbsoluteY(moreButtonParent);
            div.style.top = top + this.verticalOffset + "px";
        }
        else
            this.HideDivForContainer(moreButtonContainer, 0);
        if (ASPx.IsExists(moreButtonContainer.nearestBottomAppointmentViewInfo)) {
            var div = this.GetDivForContainer(moreButtonContainer, 1);
            if (!ASPx.IsExists(div))
                return;
            div.targetAppointmentViewInfo = moreButtonContainer.nearestBottomAppointmentViewInfo;
            var appointment = this.scheduler.GetAppointment(div.targetAppointmentViewInfo.appointmentId);
            div.targetDateTime = appointment.interval.GetEnd();
            //            debugger_;
            // div.interval = null;//new ASPxClientTimeInterval(new Date(0), 0);
            var top = ASPx.GetAbsoluteY(scrollContainer) - ASPx.GetAbsoluteY(moreButtonParent);
            var bottom = top + scrollContainer.clientHeight;
            div.style.top = bottom - this.verticalOffset - div.offsetHeight + "px";
        }
        else
            this.HideDivForContainer(moreButtonContainer, 1);
    },
    GetDivForContainer: function (moreButtonContainer, buttonIndex) {
        var div = moreButtonContainer.moreButtons[buttonIndex];
        if (!ASPx.IsExists(div)) {
            var isTop = buttonIndex == 0;
            div = this.scheduler.GetContainerElementById("MoreButtons_" + (isTop ? "Top_" : "Bottom_") + moreButtonContainer.index);
            if (!ASPx.IsExists(div))
                return null;
            moreButtonContainer.moreButtons[buttonIndex] = div;
            var clickDelegate = ASPx.CreateDelegate(this.schedulerViewInfo.OnMoreButtonClick, this.schedulerViewInfo, [div, isTop]);
            ASPx.Evt.AttachEventToElement(div, "click", clickDelegate);
            this.subscribedElements.push([div, "click", clickDelegate]);
        }
        ASPx.SetElementDisplay(div, true);
        div.style.left = this.GetDivLeftPosition(moreButtonContainer, div) + "px";
        return div;
    },
    HideMoreButtons: function () {
        var count = this.moreButtonContainers.length;
        for (var i = 0; i < count; i++) {
            var container = this.moreButtonContainers[i];
            this.HideDivForContainer(container, 0);
            this.HideDivForContainer(container, 1);
        }
    },
    HideDivForContainer: function (moreButtonContainer, buttonIndex) {
        var div = moreButtonContainer.moreButtons[buttonIndex];
        if (ASPx.IsExists(div))
            ASPx.SetElementDisplay(div, false);
    }
});
var MoreButtonContainer = ASPx.CreateClass(null, {
    constructor: function(scrollHeight, index) {
        this.Reset(scrollHeight);
        this.moreButtons = [null, null];
        this.index = index;
    },
    Reset: function(scrollHeight) {
        //doesn't reset moreButtons array
        this.nearestTopAppointmentViewInfo = null;
        this.nearestBottomAppointmentViewInfo = null;
        this.nearestTopAppointmentPosition = -1;
        this.nearestBottomAppointmentPosition = scrollHeight + 1;
    }    
});
var SingleMoreButtonHelper = ASPx.CreateClass(MoreButtonHelper, {
    constructor: function(schedulerViewInfo) {
        this.constructor.prototype.constructor.call(this, schedulerViewInfo);
    },
    CreateMoreButtonContainers: function(scrollHeight) {
        return [new MoreButtonContainer(scrollHeight, 0)];
    },
    GetMoreButtonContainer: function(viewInfo) {
        return this.moreButtonContainers[0];
    },
    GetDivLeftPosition: function(moreButtonContainer, div) {
        return this.schedulerViewInfo.parent.CalcRelativeElementLeft(this.schedulerViewInfo.GetCell(0, 0)) - div.offsetWidth - this.horizontalOffset;
    }
});
var MoreButtonInEachColumnHelper = ASPx.CreateClass(MoreButtonHelper, {
    constructor: function(schedulerViewInfo) {
        this.constructor.prototype.constructor.call(this, schedulerViewInfo);
    },
    CreateMoreButtonContainers: function(scrollHeight) {
        var count = this.schedulerViewInfo.cellContainers.length;
        var result = [];
        for(var i = 0; i < count; i++) {
            var moreButtonContainer = new MoreButtonContainer(scrollHeight, i);
            result.push(moreButtonContainer);
        }
        return result;
    },
    GetMoreButtonContainer: function(viewInfo) {
        return this.moreButtonContainers[viewInfo.containerIndex];
    },
    GetDivLeftPosition: function(moreButtonContainer) {
        return this.schedulerViewInfo.parent.CalcRelativeElementLeft(this.schedulerViewInfo.GetCell(moreButtonContainer.index, 0)) + this.horizontalOffset;
    }

});
////////////////////////////////////////////////////////////////////////////////
// VerticalSchedulerViewInfo
////////////////////////////////////////////////////////////////////////////////
var VerticalSchedulerViewInfo = ASPx.CreateClass(SchedulerViewInfo, {
    constructor: function (scheduler, prevViewInfo, usePrevCache, appointmentsSnapToCells) {
        this.constructor.prototype.constructor.call(this, scheduler, prevViewInfo, usePrevCache);
        this.appointmentsSnapToCells = appointmentsSnapToCells;
        this.isVertical = true;
        if(ASPx.IsExists(prevViewInfo) && ASPx.IsExists(prevViewInfo.moreButtonHelper)) {
            prevViewInfo.moreButtonHelper.HideMoreButtons();
        }
    },
    _constCellPrefix: function () { return "DXCntv"; },
    CreateSelectionViewInfoCore: function () {
        return new ASPx.VerticalCellHighlightViewInfo(this.scheduler, this, this.scheduler.selectionDiv, this.parent.selectionLayer);
    },
    CompareCellAndPosition: function (x, y, cell) {
        var dxtop = this.parent.CalcRelativeElementTop(cell);
        if(y < dxtop)
            return -1;
        var bottom = this.parent.CalcRelativeElementBottom(cell);
        if(y > bottom)
            return 1;
        return 0;
    },
    Dispose: function () {
        this.constructor.prototype.Dispose.call(this);
        if(this.containerScrollDelegate && this.scrollContainer) {
            ASPx.Evt.DetachEventFromElement(this.scrollContainer, "scroll", this.containerScrollDelegate);
            this.containerScrollDelegate = null;
        }
        this.moreButtonHelper.Dispose();
        if(this.timeMarkerViewInfo)
            this.timeMarkerViewInfo.Dispose();
    },
    Prepare: function (parent) {
        this.constructor.prototype.Prepare.call(this, parent);
        if(ASPx.IsExists(this.scheduler.privateShowMoreButtonsOnEachColumn) && this.scheduler.privateShowMoreButtonsOnEachColumn)
            this.moreButtonHelper = new MoreButtonInEachColumnHelper(this);
        else
            this.moreButtonHelper = new SingleMoreButtonHelper(this);
        this.timeMarkerViewInfo = new ASPx.HorizontalCurrentTimeContainerViewInfo(this.scheduler, this, this.scheduler.timeMarkerImage, this.scheduler.timeMarkerLine, this.parent.timeMarkerLayer);
    },
    ShowMoreButton: function () {
        var scrollContainer = this.GetScrollContainer();
        if(scrollContainer.scrollHeight == scrollContainer.offsetHeight)
            return;
        this.ShowMoreButtonCore();
        if(this.containerScrollDelegate) {//B182532
            ASPx.Evt.DetachEventFromElement(this.scrollContainer, "scroll", this.containerScrollDelegate);
            this.containerScrollDelegate = null;
        }
        this.containerScrollDelegate = ASPx.CreateDelegate(this.OnContainerScroll, this);
        this.scrollContainer = scrollContainer;
        ASPx.Evt.AttachEventToElement(scrollContainer, "scroll", this.containerScrollDelegate);
    },
    OnContainerScroll: function () {
        window.setTimeout(ASPx.CreateDelegate(this.ShowMoreButtonCore, this), 0);
        this.scheduler.topRowTimeManager.SaveActiveViewTopRowTime();
    },
    ShowMoreButtonCore: function () {
        this.moreButtonHelper.CalculateMoreButtons();
    },
    OnMoreButtonClick: function (e, arguments) {
        var moreButtonDiv = arguments[0];
        var targetDateTime = moreButtonDiv.targetDateTime;
        var args = new MoreButtonClickedEventArgs(targetDateTime, moreButtonDiv.interval, moreButtonDiv.resource /*"null"*/);
        this.scheduler.RaiseMoreButtonClickedEvent(args);
        if(args.handled)
            return;
        if(args.processOnServer) {
            if(ASPx.IsExists(moreButtonDiv.interval))
                var intervalArgs = ASPx.SchedulerGlobals.DateTimeToMilliseconds(moreButtonDiv.interval.GetStart()) + "," + moreButtonDiv.interval.GetDuration();
            else
                var intervalArgs = "0, 0";
            var targetDateTimeArg = ASPx.SchedulerGlobals.DateTimeToMilliseconds(targetDateTime);
            this.scheduler.RaiseCallback("RAISEMOREBTN|" + targetDateTimeArg + "," + intervalArgs + "," + moreButtonDiv.resource /*",null"*/);
            return;
        }

        var toTop = arguments[1];
        var appointmentDiv = moreButtonDiv.targetAppointmentViewInfo.contentDiv;
        var scrollContainer = this.GetScrollContainer();
        if(toTop)
            scrollContainer.scrollTop = appointmentDiv.offsetTop;
        else {
            if(scrollContainer.clientHeight > appointmentDiv.offsetHeight)
                scrollContainer.scrollTop = appointmentDiv.offsetTop - scrollContainer.clientHeight + appointmentDiv.offsetHeight;
            else
                scrollContainer.scrollTop = appointmentDiv.offsetTop;
        }
    },
    GetScrollContainer: function () {
        return this.parent.parentElement.parentNode;
    },
    SetScrollOffset: function (scrollOffset) {
        this.GetScrollContainer().scrollTop = scrollOffset;
    },
    FindStartCellByTimeOfDay: function (cellContainer, dateTime) { // ignore year, month and day information in date
        var containerStart = cellContainer.interval.GetStart();
        var dateTimeBeginOfDay = ASPxSchedulerDateTimeHelper.TruncToDate(dateTime);//B196170
        var dayTimeDelta = ASPx.SchedulerGlobals.DateSubsWithTimezone(dateTime, dateTimeBeginOfDay);
        var datTimeDuration = Math.abs(dayTimeDelta);
        var dayTime = datTimeDuration % (24 * 60 * 60 * 1000); //
        var containerAbsStart = ASPxSchedulerDateTimeHelper.TruncToDate(containerStart);//B196170
        var containerTime = ASPx.SchedulerGlobals.DateIncreaseWithUtcOffset(containerAbsStart, dayTime);//B196170
        return this.GetCell(cellContainer.index, this.FindStartCellIndexByTime(cellContainer, containerTime));
    },
    LayoutTimeMarker: function () {
        this.timeMarkerViewInfo.Recalc();
    }
});

////////////////////////////////////////////////////////////////////////////////
// HorizontalSchedulerViewInfo
////////////////////////////////////////////////////////////////////////////////
var HorizontalSchedulerViewInfo = ASPx.CreateClass(SchedulerViewInfo, {        
    constructor: function(scheduler, prevViewInfo, usePrevCache) { 
        this.constructor.prototype.constructor.call(this, scheduler, prevViewInfo, usePrevCache);
        this.moreButtonDiv = this.GetMoreButtonDiv();        
        this.isVertical = false;
        this.moreButtonLeftMargin = 3;
        this.moreButtonRightMargin = 3;
        this.moreButtonBottomMargin = 3;
    },    
    _constCellPrefix: function() { return "DXCnth"; },
    GetMoreButtonDiv: function() {         
        return this.scheduler.GetContainerElementById(ASPx.SchedulerMoreButtonId);
    },
    GetMoreButtonSize: function() {
        if(!ASPx.IsExists(this.moreButtonSize) || this.moreButtonSize <= 0)
            this.moreButtonSize = this.CalculateMoreButtonSize();
        return this.moreButtonSize;
    },
    CalculateMoreButtonSize: function() {           
        var result;
        if (ASPx.IsExists(this.moreButtonDiv)) {
            this.moreButtonDiv.style.display = "block";
            result = this.moreButtonDiv.offsetHeight;
            this.moreButtonDiv.style.display = "none";            
        }
        else
            result = 0;
        return result;
    },
    CreateSelectionViewInfoCore: function() {
        return new ASPx.HorizontalCellHighlightViewInfo(this.scheduler, this, this.scheduler.selectionDiv, this.parent.selectionLayer);
    },
    ShowMoreButton: function(cell, viewInfo) {
        if(ASPx.IsExists(this.moreButtonDiv) &&(!ASPx.IsExists(cell.hasMoreButton) || !cell.hasMoreButton)) {
            var newDiv = this.moreButtonDiv.cloneNode(true);
            newDiv.id = cell.id + "_moreButton";
            newDiv.style.left = 0;
            newDiv.style.top = 0;
            newDiv.style.display = "block";
            newDiv.schedulerControl = this.scheduler;            
            newDiv.interval = this.scheduler.GetCellInterval(cell);
            newDiv.targetDateTime = this.CalculateMoreButtonTargetDateTime(cell, viewInfo);
            newDiv.resource = this.scheduler.GetCellResource(cell);
            newDiv.isMoreButton = true;
            //cell.appendChild(newDiv);
            this.parent.AppendChildToMoreButtonLayer(newDiv);
            cell.hasMoreButton = true;
            ASPx.SetElementDisplay(newDiv, true);
            ASPx.Evt.AttachEventToElement(newDiv, "click", ASPx.MoreButtonClickEvent);
            // there is another way to add onlick event handler - we can use anonymous function: 
            // newDiv.onclick = new Function("MoreBtnClick(" + dateTime + "); ");
            // in this case, no necessary to keep dateTime and schedulerControl at the inner fields of the mainDiv. 
            // But anonymous function obstruct browser's memory because of  a great number of morebuttons which are re-created frequently. 
            // Belomytsev
            var newDivWidth = this.moreButtonWidth;//newDiv.scrollWidth;
            var cellLeft = this.parent.CalcRelativeElementLeft(cell) + this.moreButtonLeftMargin;
            var cellRight = this.parent.CalcRelativeElementRight(cell) - this.moreButtonRightMargin;
            newDiv.style.left = Math.max(cellLeft, cellRight - newDivWidth) + "px";
            newDiv.style.width = Math.max(0,Math.min(newDivWidth, cellRight - cellLeft)) + "px";
            newDiv.style.top = this.parent.CalcRelativeElementBottom(cell) - this.moreButtonSize - this.moreButtonBottomMargin + "px";
        }   
    },
    //TODO: test it!
    //TODO
    CalculateMoreButtonTargetDateTime: function(cell, aptViewInfo) {  
        var appointment = this.scheduler.GetAppointment(aptViewInfo.appointmentId);
        var cellInterval = this.scheduler.GetCellInterval(cell);
        var cellStart = cellInterval.GetStart();
        var aptStart = appointment.interval.GetStart();        
        var cellStartMSec = parseInt(ASPx.SchedulerGlobals.DateTimeToMilliseconds(cellStart));
        var aptStartMSec = parseInt(ASPx.SchedulerGlobals.DateTimeToMilliseconds(aptStart));
        if (cellStartMSec > aptStartMSec)
            return cellStart;
        else
            return aptStart;
    },
    CompareCellAndPosition: function(x, y, cell) {
        var left = this.parent.CalcRelativeElementLeft(cell);
        if(x < left)
            return -1;
        var right = this.parent.CalcRelativeElementRight(cell);
        if(x > right)
            return 1;
        var dxtop = this.parent.CalcRelativeElementTop(cell);
        if(y < dxtop)
            return -1;
        var bottom = this.parent.CalcRelativeElementBottom(cell);
        if(y > bottom)
            return 1;
        return 0;
    },
    Prepare: function(parent) {
        this.constructor.prototype.Prepare.call(this, parent);
        this.timeMarkerViewInfo = new ASPx.VerticalCurrentTimeContainerViewInfo(this.scheduler, this, this.scheduler.timeMarkerLine, this.parent.timeMarkerLayer);
    },
    LayoutTimeMarker: function() {
        this.timeMarkerViewInfo.Recalc();
    }
});

////////////////////////////////////////////////////////////////////////////////
// TimelineHeaderLevelViewInfo
////////////////////////////////////////////////////////////////////////////////
var TimelineHeaderLevelViewInfo = ASPx.CreateClass(null, {
    constructor: function() {
        this.timelineHeaders = [];
    },
    Add: function(cellLocation, offset, baseCellLocation) {
        var header = new TimelineHeaderCellViewInfo(cellLocation, offset, baseCellLocation);
        this.timelineHeaders.push(header);
    }
});   
var TimelineHeaderCellViewInfo = ASPx.CreateClass(null, {
    constructor: function(cellLocation, offset, baseCellLocation) {
        this.cellLocation = cellLocation;
        this.offset = offset;
        this.baseCellLocation = baseCellLocation;
    }
});   
var TimelineHeaderLayoutCalculator = ASPx.CreateClass(null, {
    constructor: function(schedulerControl) {
        this.schedulerControl = schedulerControl;
        this.headerCellFinder = new TimelineHeaderCellFinder(this.schedulerControl.horzTable);
        this.coordinatesCalculator = schedulerControl.horizontalViewInfo.parent;
    },
    CalculateLayout: function(headerLevels) {
        var levelCount = headerLevels.length;
        for(levelIndex = 0; levelIndex < levelCount; levelIndex++) {
            if (headerLevels[levelIndex] != null) {
                var headerLevel = headerLevels[levelIndex];
                this.CalculateLevelLayout(headerLevel.timelineHeaders);
            }
        }
    },
    CalculateLevelLayout: function(headers) {
        var count = headers.length;
        var header = headers[0];
        var start = this.GetStartPosition(header);
        var end = 0;
        for (var headerIndex = 1; headerIndex < count - 1; headerIndex++) {
            var nextHeader = headers[headerIndex];
            end = this.GetStartPosition(nextHeader);
            this.SetHeaderPosition(header, start, end);
            header = nextHeader;
            start = end;
        }
        this.SetLastHeaderPosition(headers[count-2], end);
    },
    GetStartPosition: function(header) {
        //var cell = this.schedulerControl.GetContainerElementById(header.baseCellId);
        var cell = this.headerCellFinder.GetCellByPathLocation(header.baseCellLocation);
        var position = this.coordinatesCalculator.CalcRelativeElementLeft(cell) + header.offset * cell.offsetWidth / 100;//!!!~~~
        return Math.floor(position);
        // bug in ASPxRelativeCoordinatesCalculator, so doesn't use it.
        //return this.schedulerControl.GetCellLeft(cell) + header.offset * cell.offsetWidth / 100;//!!!~~~
    },
    SetHeaderPosition: function(header, start, end) {
        //var headerCell = this.schedulerControl.GetContainerElementById(header.cellId);
        var headerCell = this.headerCellFinder.GetCellByPathLocation(header.cellLocation);
        var totalBorderWidth = headerCell.offsetWidth - headerCell.clientWidth;
        var totalHorizontalPadding = 0;
        headerCell.style.left = start + "px";
        var expectedOffsetWidth = end - start;
        if (expectedOffsetWidth > 0)
            ASPx.SchedulerGlobals.SetTableCellOffsetWidth(headerCell, expectedOffsetWidth - totalBorderWidth);
    },
    SetLastHeaderPosition: function(header, start) {
        //var headerCell = this.schedulerControl.GetContainerElementById(header.cellId);
        var headerCell = this.headerCellFinder.GetCellByPathLocation(header.cellLocation);
        headerCell.style.width = "100%";
        headerCell.style.left = start + "px";
    }
}); 

var TimelineHeaderCellFinder = ASPx.CreateClass(null, {
    constructor: function(parentTable) {
        this.parentTable = parentTable;
    },
    GetCellByPathLocation: function(path) {
        var count = path.length;
        var table = this.parentTable;
        var result = null;
        for (var i = 0; i < count; i++) {
            var location = path[i];
            var rowIndex = location[0];
            var cellIndex = location[1];
            result = table.rows[rowIndex].cells[cellIndex];
            table = this.GetNextTable(table.rows[rowIndex].cells[cellIndex]);
        }
        return result;
    },
    GetNextTable: function(cell) {
        var children = cell.childNodes;
        var count = children.length;
        for(var i = 0; i < count; i++) {
            var child = children[i];
            if(ASPx.IsExists(child.tagName) && child.tagName.toUpperCase() == "TABLE") 
                return child;
        }  
        return null;
    }
}); 

//////////////////////////////////////////////////////////////////////////////// 
var NavigationButton = ASPx.CreateClass(null, {
    constructor: function(divId, resourceId, anchorType) {
        this.divId = divId;
        this.anchorType = anchorType;
        this.resourceId = resourceId;
    }
});

ASPx.CreateDelegate = function(method, object, args) {
    function handler(e) {
        if(ASPx.IsExists(object)) {
            var delegateArgs = [];
            var count = arguments.length;
            for(var i=0; i<count; i++) 
                delegateArgs.push(arguments[i]);
            if (ASPx.IsExists(args)) 
                delegateArgs.push(args);
            return method.apply(object, delegateArgs);
        }
        else
            return method(e, args);
    }
    return handler;
}

ASPx.SchedulerViewInfo = SchedulerViewInfo;
ASPx.VerticalSchedulerViewInfo = VerticalSchedulerViewInfo;
ASPx.HorizontalSchedulerViewInfo = HorizontalSchedulerViewInfo;
ASPx.TimelineHeaderLevelViewInfo = TimelineHeaderLevelViewInfo;
ASPx.TimelineHeaderCellViewInfo = TimelineHeaderCellViewInfo;
ASPx.TimelineHeaderLayoutCalculator = TimelineHeaderLayoutCalculator;
ASPx.NavigationButton = NavigationButton;
})();