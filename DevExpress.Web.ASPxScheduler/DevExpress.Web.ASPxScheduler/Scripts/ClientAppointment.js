

(function () {

var AppointmentResizeSideType = { };
AppointmentResizeSideType.Left = 0,
AppointmentResizeSideType.Top = 1,
AppointmentResizeSideType.Bottom = 2,
AppointmentResizeSideType.Right = 4;

////////////////////////////////////////////////////////////////////////////////
// AppointmentViewInfo 
////////////////////////////////////////////////////////////////////////////////
var AppointmentStatusViewInfo = ASPx.CreateClass(null, {
    constructor: function(statusBackDivId, statusForeDivId, statusStartOffset, statusEndOffset) {    
        this.backDivId = statusBackDivId;
        this.foreDivId = statusForeDivId;
        this.startOffset = statusStartOffset;
        this.endOffset = statusEndOffset;
    }
});
var AppointmentViewInfo = ASPx.CreateClass(null, {
    constructor: function(schedulerViewInfo, containerIndex, firstCellIndex, lastCellIndex, startTime, duration, startRelativeIndent, endRelativeIndent, divId, appointmentId, statusBackDivId, statusForeDivId, statusStartOffset, statusEndOffset) {
        this.schedulerViewInfo = schedulerViewInfo;
        this.containerIndex = containerIndex;
        this.firstCellIndex = firstCellIndex;
        this.lastCellIndex = lastCellIndex;
        this.startRelativeIndent = ASPx.IsExists(startRelativeIndent) ? startRelativeIndent : 0;
        this.endRelativeIndent = ASPx.IsExists(endRelativeIndent) ? endRelativeIndent : 0;
        this.initialStartRelativeIndent = this.startRelativeIndent;
        this.initialEndRelativeIndent = this.endRelativeIndent;        
        //TODO to ASPxClientAppointment ref
        if(startTime != null && duration != null)
            this.appointmentInterval = new ASPxClientTimeInterval(startTime, duration);
        else
            this.appointmentInterval = null;
        this.initialAppointmentInterval = this.appointmentInterval != null ? this.appointmentInterval.Clone() : null;
            
        this.appointmentId = appointmentId;

        this.visibleStartTime = startTime;
        this.visibleDuration = duration;        
        this.visibleFirstCellIndex = firstCellIndex;
        this.visibleLastCellIndex = lastCellIndex;
        this.divId = divId;
        this.contentDiv = null;
        this.visibleFirstCell = null;
        this.visibleLastCell = null;
        this.mouseEvents = new ASPx.SchedulerMouseEvents();
        this.mouseEvents.AddMouseOverHandler(this);
        this.mouseEvents.AddMouseOutHandler(this);
        this.statusViewInfo = new AppointmentStatusViewInfo(statusBackDivId, statusForeDivId, statusStartOffset, statusEndOffset);
    },
    getStartTime: function() {
        return this.appointmentInterval.GetStart();
    },
    getEndTime: function() {        
        return this.appointmentInterval.GetEnd();
    },
    getDuration: function() {        
        return this.appointmentInterval.GetDuration();
    },
    CanResizeAtLeft: function() {
        return false;
    },
    CanResizeAtRight: function() {
        return false;
    },
    CanResizeAtTop: function() {
        return false;
    },
    CanResizeAtBottom: function() {
        return false;
    },
    ShowResizeDivs: function () {
        var div = this.contentDiv;
        var resizeDivs = [];
        var scheduler = this.schedulerViewInfo.scheduler;
        if (this.CanResizeAtTop())
            resizeDivs.push(this.ShowResizeDiv(scheduler.topResizeDiv, 0, 0, div.clientWidth, null, AppointmentResizeSideType.Top));
        if (this.CanResizeAtLeft())
            resizeDivs.push(this.ShowResizeDiv(scheduler.leftResizeDiv, 0, 0, null, div.clientHeight, AppointmentResizeSideType.Left));
        if (this.CanResizeAtBottom())
            resizeDivs.push(this.ShowResizeDiv(scheduler.bottomResizeDiv, 0, null, div.clientWidth, null, AppointmentResizeSideType.Bottom));
        if (this.CanResizeAtRight())
            resizeDivs.push(this.ShowResizeDiv(scheduler.rightResizeDiv, null, 0, null, div.clientHeight, AppointmentResizeSideType.Right));
        if (resizeDivs.length > 0)
            this.contentDiv.resizeDivs = resizeDivs;
    },
    SubscribeMouseEvents: function () {
        var appointmentDiv = this.contentDiv;
        this.enterLeaveHelper = new ASPx.MouseEnterLeaveHelper(appointmentDiv);
        this.enterLeaveHelper.Attach(this.OnMouseEnter.aspxBind(this), this.OnMouseLeave.aspxBind(this));
    },
    UnsubscribeMouseEvents: function () {
        if (this.enterLeaveHelper) {
            this.enterLeaveHelper.Detach();
            this.enterLeaveHelper = null;
        }
    },
    OnMouseEnter: function (element) {
        this.ShowResizeDivs();
    },
    OnMouseLeave: function (element) {
        this.HideResizeDivs();
    },
    CanResize: function() {
        var appointment = this.schedulerViewInfo.scheduler.GetAppointment(this.appointmentId);        
        return appointment.flags.allowResize;
    },
    GetToolTip: function(source) {
        while(source != null) {
            if(ASPx.IsExists(source.isToolTip) && source.isToolTip)
                return source;
            source = source.parentNode;
        }
        return null;
    },
    ResetRelativeIndentAndTime: function() {
        this.startRelativeIndent = this.initialStartRelativeIndent;
        this.endRelativeIndent = this.initialEndRelativeIndent;
        this.appointmentInterval = this.initialAppointmentInterval != null ? this.initialAppointmentInterval.Clone() : null;
    },        
    ShowResizeDiv: function (resizeDiv, left, dxtop, width, height, resizeDivType) {//name dxtop (not top) to fix FireFox Bug
        var appointmentDiv = this.contentDiv;
        var offsetX = 0;
        var offsetY = 0;
        var targetDiv = appointmentDiv;
        var clone = resizeDiv.cloneNode(true);

        ASPx.SetElementDisplay(clone, true);
        targetDiv.appendChild(clone);
        if (!ASPx.IsExists(width))
            width = clone.offsetWidth;
        if (!ASPx.IsExists(height))
            height = clone.offsetHeight;
        if (!ASPx.IsExists(left))
            left = appointmentDiv.clientWidth - width;
        if (!ASPx.IsExists(dxtop))
            dxtop = appointmentDiv.clientHeight - height;
        clone.style.left = left + offsetX + "px";
        clone.style.top = dxtop + offsetY + "px";
        clone.style.width = width + "px";
        clone.style.height = height + "px";
        clone.dxResizeDivType = resizeDivType;
        return clone;
    },
    HideResizeDivs: function () {
        var appointmentDiv = this.contentDiv;
        var targetDiv = /*ASPx.IsExists(viewInfo.adornerDiv) ? viewInfo.adornerDiv :*/appointmentDiv;
        if (!ASPx.IsExists(targetDiv.resizeDivs))
            return;
        var count = appointmentDiv.resizeDivs.length;
        for (var i = 0; i < count; i++) {
            var resizeDiv = appointmentDiv.resizeDivs[i];
            if (ASPx.IsExists(resizeDiv) && resizeDiv.parentNode == targetDiv)
                ASPx.SchedulerGlobals.RemoveChildFromParent(targetDiv, resizeDiv);
        }
        appointmentDiv.resizeDivs = null;
    },
    Dispose: function () {
        this.UnsubscribeMouseEvents();
        this.contentDiv = null;
    }
});
var HorizontalAppointmentViewInfo = ASPx.CreateClass(AppointmentViewInfo, {
    constructor: function(schedulerViewInfo, containerIndex, firstCellIndex, lastCellIndex, startTime, duration, startRelativeIndent, endRelativeIndent, divId, appointmentId, hasLeftBorder, hasRightBorder, statusBackDivId, statusForeDivId, statusStartOffset, statusEndOffset) {
        this.constructor.prototype.constructor.call(this, schedulerViewInfo, containerIndex, firstCellIndex, lastCellIndex, startTime, duration, startRelativeIndent, endRelativeIndent, divId, appointmentId, statusBackDivId, statusForeDivId, statusStartOffset, statusEndOffset);
        this.height = 0;
        this.relativePosition = 0;
        this.hasLeftBorder = hasLeftBorder;
        this.hasRightBorder = hasRightBorder;
    },
    clone: function() {
        return new HorizontalAppointmentViewInfo(this.schedulerViewInfo, this.containerIndex, this.firstCellIndex, this.lastCellIndex, this.getStartTime(), this.getDuration(),
            this.startRelativeIndent, this.endRelativeIndent, this.divId, this.appointmentId);
    },
    CanResizeAtLeft: function() {
        return this.CanResize() && this.hasLeftBorder;//TODO
    },
    CanResizeAtRight: function() {
        return this.CanResize() && this.hasRightBorder;//TODO
    },
    IsHorizontal: function() {
        return true;
    }
});
var VerticalAppointmentViewInfo = ASPx.CreateClass(AppointmentViewInfo, {
    constructor: function(schedulerViewInfo, containerIndex, firstCellIndex, lastCellIndex, startTime, duration, divId, startHorizontalIndex, endHorizontalIndex, maxIndexInGroup, startRelativeIndent, endRelativeIndent, appointmentId, hasTopBorder, hasBottomBorder, statusBackDivId, statusForeDivId, statusStartOffset, statusEndOffset) {	    
        this.constructor.prototype.constructor.call(this, schedulerViewInfo, containerIndex, firstCellIndex, lastCellIndex, startTime, duration, startRelativeIndent, endRelativeIndent, divId, appointmentId, statusBackDivId, statusForeDivId, statusStartOffset, statusEndOffset);
        this.startHorizontalIndex = startHorizontalIndex;
        this.endHorizontalIndex = endHorizontalIndex;
        this.maxIndexInGroup = maxIndexInGroup;
        this.hasTopBorder = hasTopBorder;
        this.hasBottomBorder = hasBottomBorder;        
    },
    clone: function(a) {
        return new VerticalAppointmentViewInfo(this.schedulerViewInfo, this.containerIndex, this.firstCellIndex,
            this.lastCellIndex, this.getStartTime(), this.getDuration(), this.divId, this.startHorizontalIndex, this.endHorizontalIndex, this.maxIndexInGroup, this.startRelativeIndent, this.endRelativeIndent, this.appointmentId, this.hasTopBorder, this.hasBottomBorder, statusBackDivId, statusForeDivId, statusStartOffset, statusEndOffset);
    },
    CanResizeAtTop: function() {
        return this.CanResize() && this.hasTopBorder;//TODO
    },
    CanResizeAtBottom: function() {
        return this.CanResize() && this.hasBottomBorder;//TODO
    },
    IsHorizontal: function() {
        return false;
    }
});

//////////////////////////////////////////////////////////////
// AppointmentOperationPresenter
//////////////////////////////////////////////////////////////
var AppointmentOperationPresenter = ASPx.CreateClass(null, {
    constructor: function (scheduler, appointmentDiv) {
        this.scheduler = scheduler;
        this.calculator = this.CreateCalculator();
        this.appointmentDiv = appointmentDiv;
        this.parent = this.GetParent();
        this.parentElement = this.GetParentElement();
        this.schedulerViewInfo = this.GetViewInfo();
        this.isSourceAppointmentVisible = {};
        this.sourceViewInfos = {};
        this.templateViewInfos = {};
        this.viewInfos = {};
    },
    ShowAppointment: function (appointment, showSourceAppointment) {
        var aptId = appointment.appointmentId;
        if (!ASPx.IsExists(this.sourceViewInfos[aptId]) && !ASPx.IsExists(this.templateViewInfos[aptId])) {
            this.sourceViewInfos[aptId] = this.FindSourceViewInfos(aptId, this.schedulerViewInfo);
            this.templateViewInfos[aptId] = this.GetTemplateViewInfos(aptId);
            this.SaveAppointmentVisibleState(aptId);
        }
        var sourceViewInfos = this.sourceViewInfos[aptId];
        var templateViewInfos = this.templateViewInfos[aptId];
        this.ShowSourceAppointment(showSourceAppointment, aptId);
        this.HidePrevViewInfos(aptId);
        if (templateViewInfos.length == 0)
            return;
        if (!this.CanShowAppointment(appointment))
            return;
        var containers = this.GetCellContainers();
        var count = containers.length;
        var viewInfos = [];
        this.viewInfos[aptId] = viewInfos;

        var operationInterval = appointment.operationInterval;
        var operationIntervalStartTime = operationInterval.GetStart();
        var operationIntervalEndTime = operationInterval.GetEnd();

        for (var i = 0; i < count; i++) {
            var container = containers[i];
            var containerInterval = container.interval;
            var containerStartTime = containerInterval.GetStart();
            var containerEndTime = containerInterval.GetEnd();

            var isAppointmentOutsideCell = operationIntervalStartTime >= containerEndTime || operationIntervalEndTime < containerStartTime || ((operationIntervalEndTime - containerStartTime) == 0 && !operationInterval.IsZerroDurationInterval());
            if (isAppointmentOutsideCell || !this.ShowAppointmentForResource(appointment, container.resource))
                continue;
            var intervalStart = ASPx.SchedulerGlobals.DateTimeMaxValue(containerStartTime, operationIntervalStartTime);
            var intervalEnd = ASPx.SchedulerGlobals.DateTimeMinValue(containerEndTime, operationIntervalEndTime);
            this.AddViewInfo(viewInfos, container, intervalStart, intervalEnd);
        }
        count = viewInfos.length;
        for (var i = 0; i < count; i++) {
            var sourceDiv = this.appointmentDiv;
            var isPrimaryAppointment = this.appointmentDiv.appointmentViewInfo.appointmentId == appointment.appointmentId;
            if (count > 1 || !isPrimaryAppointment) {
                if (i == count - 1 && count > 1) {
                    var index = templateViewInfos.length - 1;
                    sourceDiv = templateViewInfos[index].contentDiv;
                }
                else
                    if (i == 0 || !isPrimaryAppointment)
                        sourceDiv = templateViewInfos[0].contentDiv;
            }
            var sourceViewInfo = sourceDiv.appointmentViewInfo;
            var newDiv = sourceDiv.cloneNode(true);
            
            var targetViewInfo = viewInfos[i];

            targetViewInfo.maxIndexInGroup = ASPx.IsExists(sourceViewInfo.maxIndexInGroup) ? sourceViewInfo.maxIndexInGroup : 1;
            targetViewInfo.startHorizontalIndex = ASPx.IsExists(sourceViewInfo.startHorizontalIndex) ? sourceViewInfo.startHorizontalIndex : 0;
            targetViewInfo.endHorizontalIndex = ASPx.IsExists(sourceViewInfo.endHorizontalIndex) ? sourceViewInfo.endHorizontalIndex : 1;
            var sourceStatusInfo = sourceViewInfo.statusViewInfo;
            if (ASPx.IsExists(sourceStatusInfo))
                targetViewInfo.statusViewInfo = new AppointmentStatusViewInfo(sourceStatusInfo.backDivId, sourceStatusInfo.foreDivId, 0, 0);
            newDiv.style.width = "";
            newDiv.style.height = ""; //TODO
            targetViewInfo.contentDiv = newDiv;
            targetViewInfo.startRelativeIndent = sourceViewInfo.startRelativeIndent;
            targetViewInfo.endRelativeIndent = sourceViewInfo.endRelativeIndent;
            targetViewInfo.initialStartRelativeIndent = sourceViewInfo.initialStartRelativeIndent;
            targetViewInfo.initialEndRelativeIndent = sourceViewInfo.initialEndRelativeIndent;
            newDiv.appointmentViewInfo = targetViewInfo;
            this.parentElement.appendChild(newDiv);
            this.CalculateAppointmentLayout(targetViewInfo);
            this.CalculateFinalContentLayout(targetViewInfo);
            this.scheduler.appointmentSelection.SelectAppointmentViewInfoCore(targetViewInfo);
        }
    },
    ShowAppointmentForResource: function (appointment, containerResource) {
        var appointmentResources = appointment.operationResources;
        if (!ASPx.IsExists(appointmentResources))
            appointmentResources = appointment.resources;
        if (!ASPx.IsExists(appointmentResources))
            return false;
        if (ASPx.SchedulerUtils.IsAppointmentResourcesEmpty(appointmentResources) || ASPx.Data.ArrayIndexOf(appointmentResources, containerResource) >= 0 || containerResource == "null")
            return true;
        return false;
    },
    GetTemplateViewInfos: function (appointmentId) {
        if (this.sourceViewInfos[appointmentId].length != 0)
            return this.sourceViewInfos[appointmentId];
        else {
            var horizontalAppointmentsViewInfos = this.FindSourceViewInfos(appointmentId, this.scheduler.horizontalViewInfo);
            var verticalAppointmentsViewInfos = this.FindSourceViewInfos(appointmentId, this.scheduler.verticalViewInfo);
            if (horizontalAppointmentsViewInfos.length > 0)
                return horizontalAppointmentsViewInfos;
            else
                return verticalAppointmentsViewInfos;
        }
    },
    ShowSourceAppointment: function (showSourceAppointment, aptId) {
        if (this.isSourceAppointmentVisible[aptId] != showSourceAppointment) {
            ASPx.Selection.Clear(); //IE BUG
            var sourceViewInfos = this.sourceViewInfos[aptId];
            var count = sourceViewInfos.length;
            for (var i = 0; i < count; i++) {
                var viewInfo = sourceViewInfos[i];
                this.ShowSourceAppointmentViewInfo(viewInfo, showSourceAppointment);
            }
            this.isSourceAppointmentVisible[aptId] = showSourceAppointment;
        }
    },
    ShowSourceAppointmentViewInfo: function (viewInfo, isVisible) {
        ASPx.SetSchedulerDivDisplay(viewInfo.contentDiv, isVisible);
        if (ASPx.IsExists(viewInfo.adornerDiv))
            ASPx.SetSchedulerDivDisplay(viewInfo.adornerDiv, isVisible);
    },
    SaveAppointmentVisibleState: function (aptId) {
        if (!this.sourceViewInfos)
            return;
        var sourceViewInfos = this.sourceViewInfos[aptId];
        var count = sourceViewInfos.length;
        for (var i = 0; i < count; i++) {
            var viewInfo = sourceViewInfos[i];
            viewInfo.lastIsVisibile = ASPx.GetElementDisplay(viewInfo.contentDiv);
        }
    },
    RestoreAppointmentVisibleState: function (aptId) {
        if (!this.sourceViewInfos)
            return;
        var sourceViewInfos = this.sourceViewInfos[aptId];
        var count = sourceViewInfos.length;
        for (var i = 0; i < count; i++) {
            var viewInfo = sourceViewInfos[i];
            this.ShowSourceAppointmentViewInfo(viewInfo, viewInfo.lastIsVisibile);
            delete viewInfo.lastIsVisibile;
        }
        this.isSourceAppointmentVisible[aptId] = true;
    },
    HideAppointment: function (aptId, showSourceAppointment) {
        this.HidePrevViewInfos(aptId);
        if (!ASPx.IsExists(this.sourceViewInfos[aptId]) || this.sourceViewInfos[aptId].length == 0)
            return;
        if (showSourceAppointment) {
            this.RestoreAppointmentVisibleState(aptId); //B189430
            //this.ShowSourceAppointment(true, aptId);
        }
    },
    FindSourceViewInfos: function (appointmentId, viewInfo) {
        var result = [];
        var viewInfos = viewInfo.appointmentViewInfos;
        var count = viewInfos.length;
        for (var i = 0; i < count; i++) {
            if (viewInfos[i].appointmentId == appointmentId)
                result.push(viewInfos[i]);
        }
        return result;
    },
    HidePrevViewInfos: function (aptId) {
        var viewInfos = this.viewInfos[aptId];
        if (!ASPx.IsExists(viewInfos))
            return;
        var count = viewInfos.length;
        for (var i = 0; i < count; i++) {
            //ASPx.RemoveElement(viewInfos[i].contentDiv);
            this.scheduler.appointmentSelection.UnselectAppointmentViewInfoCore(viewInfos[i]);
            ASPx.SchedulerGlobals.RecycleNode(viewInfos[i].contentDiv);
            viewInfos[i].contentDiv = null;

        }
        this.viewInfos[aptId] = null;
    },
    CalculateFinalContentLayout: function (viewInfo) {
        var innerContentDiv = this.calculator.FindInnerContentDiv(viewInfo);
        if (ASPx.IsExists(innerContentDiv)) {
            this.calculator.PrepareSetInnerContentDivSize(innerContentDiv, viewInfo);
            this.calculator.CalculateSetInnerContentDivSizeParameters1(innerContentDiv, viewInfo);
            this.calculator.CalculateSetInnerContentDivSizeParameters2(innerContentDiv, viewInfo);

            this.calculator.CalculateSetInnerContentDivWidth(innerContentDiv, viewInfo);
            this.calculator.SetInnerContentDivWidth(innerContentDiv, viewInfo);
            this.calculator.CalculateSetInnerContentDivHeight(innerContentDiv, viewInfo);
            this.calculator.SetInnerContentDivHeight(innerContentDiv, viewInfo);
        }
    }
});
/////////////////////////////////////////////////////////////////////////////////
//    VerticalAppointmentOperationPresenter
/////////////////////////////////////////////////////////////////////////////////
var VerticalAppointmentOperationPresenter = ASPx.CreateClass(AppointmentOperationPresenter, {
    constructor: function(scheduler, appointmentDiv) {
        this.constructor.prototype.constructor.call(this, scheduler, appointmentDiv);
        this.isHorizontal = true;
    },
    CreateCalculator: function() {
        return this.scheduler.CreateVerticalAppointmentsCalculator();
    },
    GetCellContainers: function() {
        return this.scheduler.verticalViewInfo.cellContainers;
    },
    GetParent: function() {
        return this.scheduler.verticalViewInfo.parent;
    },
    GetParentElement: function() {
        return this.scheduler.verticalViewInfo.parent.parentElement;
    },
    GetViewInfo: function() {
        return this.scheduler.verticalViewInfo;
    },
    CanShowAppointment: function(appointment) {
        if (this.scheduler.privateShowAllAppointmentsOnTimeCells)
            return true;
        else
            return appointment.operationInterval.IsSmallerThanDay();        
    },
    AddViewInfo: function(aptViewInfos, cellContainer, startTime, endTime) {
    //constructor: function(schedulerViewInfo, containerIndex, firstCellIndex, lastCellIndex, startTime, duration, divId, startHorizontalIndex, endHorizontalIndex, maxIndexInGroup, startRelativeIndent, endRelativeIndent, appointmentId, statusItemId) {
        var viewInfo = new VerticalAppointmentViewInfo(this.schedulerViewInfo, cellContainer.containerIndex,
            null, null, startTime, ASPx.SchedulerGlobals.DateSubsWithTimezone(endTime, startTime), null, 0, 1, 1, 0, 0, null);/*this.appointmentDiv.appointmentViewInfo.clone();*/
        /*viewInfo.maxIndexInGroup = 1;
        viewInfo.startHorizontalIndex = 0;
        viewInfo.endHorizontalIndex = 1;*/        
        viewInfo.containerIndex = cellContainer.containerIndex;
        var startCellIndex = this.scheduler.verticalViewInfo.FindStartCellIndexByTime(cellContainer, startTime);
        var endCellIndex = this.scheduler.verticalViewInfo.FindEndCellIndexByTime(cellContainer, endTime);
        if(endCellIndex < startCellIndex)
            endCellIndex = startCellIndex;
        viewInfo.visibleFirstCellIndex = startCellIndex;
        viewInfo.visibleLastCellIndex = endCellIndex;
        var startCell = this.scheduler.verticalViewInfo.GetCell(cellContainer.index, startCellIndex);
        var endCell = this.scheduler.verticalViewInfo.GetCell(cellContainer.index, endCellIndex);
        
        if(!ASPx.IsExists(startCell) || !ASPx.IsExists(endCell))
            return null;
        var relativeTop = 0;
        var relativeBottom = 0;
        if(!this.scheduler.verticalViewInfo.appointmentsSnapToCells && startCell != endCell) {
            var startCellStartTime = this.scheduler.GetCellStartTime(startCell);
            var endCellEndTime = this.scheduler.GetCellEndTime(endCell);
            var startCellDuration = this.scheduler.GetCellDuration(startCell);
            var endCellDuration = this.scheduler.GetCellDuration(endCell);
            relativeTop = ASPx.SchedulerGlobals.DateSubsWithTimezone(startTime, startCellStartTime) * startCell.offsetHeight / startCellDuration;
            //if(endCellIndex != startCellIndex)
                relativeBottom = ASPx.SchedulerGlobals.DateSubsWithTimezone(endCellEndTime, endTime) * endCell.offsetHeight / endCellDuration;
            /*else
                relativeBottom = 0;*/
        }
        viewInfo.startCell = startCell;
        viewInfo.endCell = endCell;
        viewInfo.startRelativeIndent = relativeTop;
        viewInfo.endRelativeIndent = relativeBottom;
        viewInfo.initialStartRelativeIndent = relativeTop;
        viewInfo.initialEndRelativeIndent = relativeBottom;
        aptViewInfos.push(viewInfo);
    },
    CalculateAppointmentLayout: function(viewInfo) {
        this.calculator.CalculateAppointmentLayout(viewInfo);
    }
});
/////////////////////////////////////////////////////////////////////////////////
//    HorizontalAppointmentOperationPresenter
/////////////////////////////////////////////////////////////////////////////////
var HorizontalAppointmentOperationPresenter = ASPx.CreateClass(AppointmentOperationPresenter, {
    constructor: function(scheduler, appointmentDiv) {
        this.constructor.prototype.constructor.call(this, scheduler, appointmentDiv);
        this.isHorizontal = false;
    },
    CreateCalculator: function() {
        return this.scheduler.CreateHorizontalAppointmentsCalculator();
    },
    GetCellContainers: function() {
        return this.scheduler.horizontalViewInfo.cellContainers;
    },
    GetParent: function() {
        return this.scheduler.horizontalViewInfo.parent;
    },
    GetParentElement: function() {
        return this.scheduler.horizontalViewInfo.parent.innerParentElement;
            
    },
    GetViewInfo: function() {
        return this.scheduler.horizontalViewInfo;
    },
    CanShowAppointment: function(appointment) {
        var isDayView = this.scheduler.privateActiveViewType == ASPxSchedulerViewType.Day || this.scheduler.privateActiveViewType == ASPxSchedulerViewType.WorkWeek || this.scheduler.privateActiveViewType == ASPxSchedulerViewType.FullWeek;
        var isAptLongerThanDay = !appointment.operationInterval.IsSmallerThanDay();              
        var showAllAppointmentsOnTimeCells = this.scheduler.privateShowAllAppointmentsOnTimeCells;
        if (isDayView)             
            return (isAptLongerThanDay && !showAllAppointmentsOnTimeCells);
        else
            return true;           
        //return isNotDayView || isAptLongerThanDay;      
    },
    AddViewInfo: function(aptViewInfos, cellContainer, startTime, endTime) {
        
        var startCellIndex = this.scheduler.verticalViewInfo.FindStartCellIndexByTime(cellContainer, startTime);
        var endCellIndex = this.scheduler.verticalViewInfo.FindEndCellIndexByTime(cellContainer, endTime);
        if(startCellIndex < 0 || endCellIndex < 0)
            return null;
        var lastCellTop = null;
        for(var i = startCellIndex; i <= endCellIndex; i++) {
            var cell = this.schedulerViewInfo.GetCell(cellContainer.containerIndex, i);
            var cellTop = this.parent.CalcRelativeElementTop(cell);
            if(cellTop != lastCellTop && lastCellTop != null) {
                var viewInfo = this.CreateSingleViewInfo(cellContainer.containerIndex, startCellIndex, i > startCellIndex ? i - 1 : startCellIndex);
                aptViewInfos.push(viewInfo);
                startCellIndex = i;
            }
            lastCellTop = cellTop;
        }
        if(endCellIndex < startCellIndex)
            endCellIndex = startCellIndex;
        var viewInfo = this.CreateSingleViewInfo(cellContainer.containerIndex, startCellIndex, endCellIndex);
        aptViewInfos.push(viewInfo);
    },
    CreateSingleViewInfo: function(containerIndex, startCellIndex, endCellIndex) {
        var viewInfo = new HorizontalAppointmentViewInfo(this.schedulerViewInfo, containerIndex, startCellIndex, endCellIndex, null, null, 0, 0, null, this.appointmentDiv.appointmentViewInfo.appointmentId);/*this.appointmentDiv.appointmentViewInfo.clone();*/
        viewInfo.visibleFirstCellIndex = startCellIndex;
        viewInfo.visibleLastCellIndex = endCellIndex;
        viewInfo.containerIndex = containerIndex;
        return viewInfo;
    },
    CalculateAppointmentLayout: function(viewInfo) {
        ASPx.SetSchedulerDivDisplay(viewInfo.contentDiv, true);
        this.calculator.CalculateAppointmentLayoutAtOnce(viewInfo);
    }
});
var DragViewHelper = ASPx.CreateClass(null, {
    constructor: function(scheduler, primaryAppointmentId, appointmentDiv) {
        this.scheduler = scheduler;
        this.primaryAppointmentId = primaryAppointmentId;
        this.horizontalDragPresenter = new HorizontalAppointmentOperationPresenter(scheduler, appointmentDiv);
        this.verticalDragPresenter = new VerticalAppointmentOperationPresenter(scheduler, appointmentDiv);
    },
    ShowDraggedAppointmentPosition: function(e, draggedAppointments, copy) {
        var count = draggedAppointments.length;
        if(count <= 0)
            return;
        for(var i = 0; i < count; i++) {
            var appointment = draggedAppointments[i];
            copy &= appointment.flags.allowCopy;
            this.horizontalDragPresenter.ShowAppointment(appointment, copy != 0);
            this.verticalDragPresenter.ShowAppointment(appointment, copy != 0);
            if(appointment.appointmentId == this.primaryAppointmentId) {
                //var content = appointment.GetToolTipContent(this.scheduler) + "<br />" + this.scheduler.operationToolTipCaption;
                var toolTip = this.scheduler.GetAppointmentDragTooltip();
                if(ASPx.IsExists(toolTip) && toolTip.CanShowToolTip()) {
                    var toolTipData = new ASPxClientSchedulerToolTipData(appointment, appointment.operationInterval, appointment.operationResources)
                    toolTip.SetContent(toolTipData);
                    if (this.scheduler.activeToolTip == toolTip && !toolTip.ShouldResetPositionByTimer()) {
                        continue;
                    } 
                    else
                        this.scheduler.HideAllToolTips();
                    
                    toolTip.ShowToolTip(ASPx.Evt.GetEventX(e), ASPx.Evt.GetEventY(e));
                    toolTip.FinalizeUpdate(toolTipData);
                }
            }
       }
    },
    HideDraggedAppointmentPosition: function(draggedAppointments, restoreAptPos) {
        var count = draggedAppointments.length;
        for(var i = 0; i < count; i++) {
            var appointmentId = draggedAppointments[i].appointmentId;
            this.horizontalDragPresenter.HideAppointment(appointmentId, restoreAptPos);
            this.verticalDragPresenter.HideAppointment(appointmentId, restoreAptPos);
        }
        var toolTip = this.scheduler.GetAppointmentDragTooltip();
        if(ASPx.IsExists(toolTip)) {
            toolTip.HideToolTip();
        }
    },
    HideToolTip: function() {
        var toolTip = this.scheduler.GetAppointmentDragTooltip();
        if(ASPx.IsExists(toolTip)) {        
            toolTip.HideToolTip();
        }
    }
});
var AppointmentDragHelper = ASPx.CreateClass(null, {//TODO: inherit from ASPx.DragHelper
    constructor: function (scheduler, appointmentDiv, cell, e, onDoClickHandler, owner) {
        if (ASPx.currentDragHelper != null)
            ASPx.currentDragHelper.cancelDrag();

        scheduler.DisableReminderTimer();
        this.dragArea = 5;
        this.lastX = ASPx.Evt.GetEventX(e);
        this.lastY = ASPx.Evt.GetEventY(e);

        this.scheduler = scheduler;
        //this.canDrag = this.scheduler.appointmentSelection.selectedAppointmentIds.length == 1; 
        this.canDrag = true;
        this.startCell = cell;
        this.lastCell = null;
        this.lastCtrlState = null;
        this.scheduler = scheduler;
        this.appointmentSelection = scheduler.appointmentSelection;
        this.appointmentDiv = appointmentDiv;
        this.onDoClickHandler = onDoClickHandler;
        this.onEndDrag = this.OnApplyDrag;
        this.dragViewHelper = new DragViewHelper(scheduler, appointmentDiv.appointmentViewInfo.appointmentId, appointmentDiv);
        this.owner = owner;
        ASPx.currentDragHelper = null;
    },
    drag: function (e) {
        if (!this.canDrag) return;
        if (!this.isDragging()) {
            if (!this.isOutOfDragArea(ASPx.Evt.GetEventX(e), ASPx.Evt.GetEventY(e)))
                return;
            this.startDragCore(e);
        }
        if (this.isDragging())
            this.dragCore(e);
    },

    startDragCore: function (e) {
        if (ASPx.Browser.IE) {//FIX IE Bug with ESC button
            this.scheduler.innerContentElement.setActive();

        }
        this.startCellTime = this.scheduler.GetCellStartTime(this.startCell);
        this.startCellResource = this.scheduler.GetCellResource(this.startCell);
        var selectedAppointmentIds = this.appointmentSelection.selectedAppointmentIds;
        this.selectedAppointments = [];
        var count = selectedAppointmentIds.length;
        for (var i = 0; i < count; i++) {
            var apt = this.scheduler.GetAppointment(selectedAppointmentIds[i]);
            if (apt.flags.allowDrag)
                this.selectedAppointments.push(apt);
        }
        if (this.selectedAppointments.length == 0)
            return;

        var hitTestResult = this.scheduler.CalcHitTest(e);
        var cell = hitTestResult.cell;
        if (ASPx.IsExists(cell)) {
            var cellTime = this.scheduler.GetCellStartTime(cell);
            var activeViewType = this.scheduler.GetActiveViewType();
            if (activeViewType == ASPxSchedulerViewType.Day || activeViewType == ASPxSchedulerViewType.WorkWeek || activeViewType == ASPxSchedulerViewType.FullWeek) {
                this.dragState = new DragDayViewAppointmentState(this.selectedAppointments, this.selectedAppointments[0], ASPx.SchedulerGlobals.DateSubsWithTimezone(this.startCellTime, this.selectedAppointments[0].interval.GetStart()), this.scheduler.privateShowAllAppointmentsOnTimeCells);
            }
            else
                this.dragState = new DragAppointmentState(this.selectedAppointments, this.selectedAppointments[0], ASPx.SchedulerGlobals.DateSubsWithTimezone(this.startCellTime, this.selectedAppointments[0].interval.GetStart()));
        }

    },
    dragCore: function (e) {
        var hitTestResult = this.scheduler.CalcHitTest(e);
        var cell = hitTestResult.cell;
        if (ASPx.IsExists(cell) && (cell != this.lastCell || this.lastCtrlState != ctrlPressed)) {
            var cellTime = this.scheduler.GetCellInterval(cell);
            var cellResource = this.scheduler.GetCellResource(cell);
            if (ASPx.IsExists(cellTime) && ASPx.IsExists(cellResource)) {
                this.dragState.DragTo(e, cellTime, cellResource);
                var ctrlPressed = e.ctrlKey;
                this.dragViewHelper.ShowDraggedAppointmentPosition(e, this.selectedAppointments, ctrlPressed);
                this.lastCell = cell;
                this.lastCtrlState = ctrlPressed;
            }
        }
    },
    isDragging: function () {
        return ASPx.IsExists(this.dragState);
    },
    OnCallback: function () {
        //this.cancelDrag(false);
        this.cancelDrag(false, false);
    },
    OnCallbackError: function () {
        this.cancelDrag(true, false);
    },
    cancelDrag: function (restoreAptPos, restoreTimer) {
        if (!ASPx.IsExists(restoreAptPos))
            restoreAptPos = true;
        if (!ASPx.IsExists(restoreTimer))
            restoreTimer = true;
        if (this.isDragging())
            this.dragViewHelper.HideDraggedAppointmentPosition(this.selectedAppointments, restoreAptPos);
        this.scheduler.onCallback = null;
        this.scheduler.onCallbackError = null;

        ASPx.currentDragHelper = null;
        if (restoreTimer)
            this.scheduler.EnableReminderTimer();
    },
    endDrag: function (e) {
        var restoreAptPos = true;
        var wasCallback = false;
        if (!this.isDragging() && !this.isOutOfDragArea(ASPx.Evt.GetEventX(e), ASPx.Evt.GetEventY(e))) {
            if (ASPx.IsExists(this.onDoClickHandler))
                this.onDoClickHandler.call(this.owner, e);
        }
        else {
            var ctrlPressed = e.ctrlKey;
            wasCallback = this.onEndDrag(ctrlPressed);
        }
        if (!wasCallback)
            this.cancelDrag(true, false);
        else {
            this.dragViewHelper.HideToolTip();
            ASPx.currentDragHelper = null;
            this.scheduler.onCallback = ASPx.CreateDelegate(this.OnCallback, this);
            this.scheduler.onCallbackError = ASPx.CreateDelegate(this.OnCallbackError, this);
        }
    },
    isOutOfDragArea: function (newX, newY) {
        return Math.max(Math.abs(newX - this.lastX), Math.abs(newY - this.lastY)) >= this.dragArea;
    },
    OnApplyDrag: function (copy) {
        var selectedAppointmentIds = this.appointmentSelection.selectedAppointmentIds;
        var count = selectedAppointmentIds.length;
        var params = "APTSCHANGE|";
        var wasChanges = false;
        for (var i = 0; i < count; i++) {
            var id = selectedAppointmentIds[i];
            var appointment = this.scheduler.GetAppointment(id);
            if (!ASPx.IsExists(appointment) || !ASPx.IsExists(appointment.operationInterval) || !ASPx.IsExists(appointment.operationResources))
                continue;
            var canCopy = copy & appointment.flags.allowCopy;
            var wasDragged = !appointment.operationInterval.Equals(appointment.interval) || !ASPx.Data.ArrayEqual(appointment.resources, appointment.operationResources);
            var wasMovedToAllDayArea = appointment.operationAllDay && !appointment.allDay;
            if (!wasDragged && !canCopy)
                continue;
            if (wasChanges)
                params += "!";
            params += id + "?START=" + ASPx.SchedulerGlobals.DateTimeToMilliseconds(appointment.operationInterval.GetStart());
            if (wasMovedToAllDayArea)
                params += "?MADA=true";
            if (appointment.operationInterval.GetDuration() != appointment.interval.GetDuration())
                params += "?DURATION=" + appointment.operationInterval.GetDuration();
            if (!ASPx.Data.ArrayEqual(appointment.resources, appointment.operationResources))
                params += "?RESOURCES=" + this.ResourcesToString(appointment.operationResources);
            if (canCopy)
                params += "?COPY=true";            
            wasChanges = true;
        }
        if (wasChanges) {
            var operation = new ASPxClientAppointmentOperation(this, params);
            var isHandled = this.scheduler.RaiseAppointmentDrop(operation);
            if (isHandled)
                return true;
            this.Apply(params);
            return true;
        }
        else
            return false;

    },
    Apply: function (params) {
        this.scheduler.RaiseCallback(params);
    },
    Cancel: function () {
        this.cancelDrag(true, true);
    },
    ResourcesToString: function (resources) {
        var result = "";
        var count = resources.length;
        for (var i = 0; i < count; i++) {
            if (i > 0)
                result += ",";
            result += resources[i];
        }
        return result;
    }
});
var DragAppointmentState = ASPx.CreateClass(null, {
    constructor: function(sourceAppointments, primaryAppointment, appointmentDragOffset) {
        this.appointmentDragOffset = appointmentDragOffset;
        this.primaryAppointment = primaryAppointment;
        this.lastHitResource = "null";
        this.sourceAppointments = sourceAppointments;
    },
    DragTo: function(evt, layoutInterval, layoutResource) {
        var appointmentsOffset = this.CalculateAppointmentsOffset(layoutInterval);
        this.lastHitResource = layoutResource;
        this.DragAppointments(evt, layoutInterval, layoutResource, appointmentsOffset);
    },
    DragAppointments: function(evt, layoutInterval, layoutResource, appointmentsOffset) {    
        this.DragAppointmentsCore(evt, layoutInterval, layoutResource, appointmentsOffset, this.DragAppointment);
    },
    CalculateAppointmentsOffset: function(layoutInterval) {
        var newPrimaryAppointmentStart = ASPx.SchedulerGlobals.DateIncreaseWithUtcOffset(layoutInterval.GetStart(), -this.appointmentDragOffset);/*new Date(layoutInterval.start - this.appointmentDragOffset);*/
        //return newPrimaryAppointmentStart - this.primaryAppointment.interval.start;
        return ASPx.SchedulerGlobals.DateSubsWithTimezone(newPrimaryAppointmentStart, this.primaryAppointment.interval.GetStart());
    },
    DragAppointmentsCore: function(evt, layoutInterval, layoutResource, appointmentsOffset, dragAppointmentHandler) {
        var sourceAppointments = this.sourceAppointments;
        var count = sourceAppointments.length;
        for(var i = 0; i < count; i++) {
            var sourceAppointment = sourceAppointments[i];
            if(sourceAppointment.flags.allowDrag) {
                this.ChangeResource(evt, sourceAppointment, layoutResource);    
                dragAppointmentHandler.call(this, sourceAppointment, layoutInterval, appointmentsOffset);
            //RaiseAppointmentDragEvent(new AppointmentDragEventArgs(sourceAppointment, editedAppointment, layoutInterval, layoutResource));
            }
        }            
    },
    DragAppointment: function(appointment, layoutInterval, appointmentsOffset) {
        var startTime = ASPx.SchedulerGlobals.DateIncreaseWithUtcOffset(appointment.interval.GetStart(), appointmentsOffset);
        var endTime = ASPx.SchedulerGlobals.DateIncreaseWithUtcOffset(appointment.interval.GetEnd(), appointmentsOffset);
        var duration = ASPx.SchedulerGlobals.DateSubsWithTimezone(endTime, startTime);/*endTime - startTime;*/
        if(duration < 0)
            duration = 0;
        appointment.operationInterval = new ASPxClientTimeInterval(startTime, duration);
    },
    ChangeResource: function(evt, sourceAppointment, layoutResource) {
        //TODO: test it
        if(this.ShouldChangeResource(sourceAppointment, layoutResource))
            sourceAppointment.operationResources = [layoutResource];
        else
            sourceAppointment.operationResources = sourceAppointment.resources;
    },

    ShouldChangeResource: function(sourceAppointment, layoutResource) {
        if(sourceAppointment.appointmentType != ASPxAppointmentType.Normal || !sourceAppointment.flags.allowDragBetweenResources)
            return false;
        if(layoutResource == "null" || ASPx.SchedulerUtils.IsAppointmentResourcesEmpty(sourceAppointment.resources) || ASPx.Data.ArrayContains(sourceAppointment.resources, layoutResource))
            return false;
        return true;
    }
});

/////////////////////////////////////////////////////////////
// DragDayViewAppointmentState
/////////////////////////////////////////////////////////////
var DragDayViewAppointmentState = ASPx.CreateClass(DragAppointmentState, {
    constructor: function (sourceAppointments, primaryAppointment, appointmentDragOffset, showAllAppointmentsOnTimeCells) {
        this.constructor.prototype.constructor.call(this, sourceAppointments, primaryAppointment, appointmentDragOffset);
        this.showAllAppointmentsOnTimeCells = showAllAppointmentsOnTimeCells;
    },
    DragAppointments: function (evt, layoutInterval, layoutResource, appointmentsOffset) {
        if (layoutInterval.IsSmallerThanDay())
            this.DragAppointmentsCore(evt, layoutInterval, layoutResource, appointmentsOffset, this.DragToCell);
        else
            this.DragAppointmentsCore(evt, layoutInterval, layoutResource, appointmentsOffset, this.DragToAllDayArea);
    },
    DragToAllDayArea: function (sourceAppointment, layoutInterval, appointmentsOffset) {
        if (sourceAppointment.interval.IsSmallerThanDay())
            this.DragShortAppointmentToAllDayArea(sourceAppointment, appointmentsOffset);
        else
            this.DragLongAppointmentToAllDayArea(sourceAppointment, appointmentsOffset, layoutInterval);
    },
    DragShortAppointmentToAllDayArea: function (sourceAppointment, appointmentsOffset) {
        var start;
        start = ASPx.SchedulerGlobals.DateIncreaseWithUtcOffset(ASPxSchedulerDateTimeHelper.TruncToDate(sourceAppointment.interval.GetStart()), appointmentsOffset);
        sourceAppointment.operationInterval = new ASPxClientTimeInterval(start, ASPxSchedulerDateTimeHelper.DaySpan);
        sourceAppointment.operationAllDay = true;
    },
    DragLongAppointmentToAllDayArea: function (sourceAppointment, appointmentsOffset, layoutInterval) {
        var start;
        if (this.showAllAppointmentsOnTimeCells) {
            var offset = this.CalculateLongAppointmentOffset(layoutInterval, appointmentsOffset);
            var duration = this.ExtendToDay(sourceAppointment);
            start = ASPx.SchedulerGlobals.DateIncreaseWithUtcOffset(ASPxSchedulerDateTimeHelper.TruncToDate(sourceAppointment.interval.GetStart()), offset);
            sourceAppointment.operationInterval = new ASPxClientTimeInterval(start, duration);
        }
        else {
            start = ASPx.SchedulerGlobals.DateIncreaseWithUtcOffset(sourceAppointment.interval.GetStart(), appointmentsOffset);
            sourceAppointment.operationInterval = new ASPxClientTimeInterval(start, sourceAppointment.interval.GetDuration());
        }
    },
    CalculateLongAppointmentOffset: function (layoutInterval, appointmentsOffset) {
        var dragStart = ASPx.SchedulerGlobals.DateIncreaseWithUtcOffset(layoutInterval.GetStart(), -appointmentsOffset);
        var daysDifference = this.CalculateDaysDifference(dragStart, layoutInterval.GetStart());
        return ASPxSchedulerDateTimeHelper.DaySpan * daysDifference;
    },
    CalculateDaysDifference: function (date1, date2) {
        snappedDate1 = ASPxSchedulerDateTimeHelper.TruncToDate(date1);
        snappedDate2 = ASPxSchedulerDateTimeHelper.TruncToDate(date2);
        return ASPx.SchedulerGlobals.DateSubsWithTimezone(snappedDate2, snappedDate1) / ASPxSchedulerDateTimeHelper.DaySpan;
    },
    ExtendToDay: function (sourceAppointment) {
        var duration = sourceAppointment.interval.GetDuration(); ;
        var remainder = duration % ASPxSchedulerDateTimeHelper.DaySpan;
        if (remainder != 0)
            return (duration - remainder + ASPxSchedulerDateTimeHelper.DaySpan);
        else
            return duration;
    },
    DragToCell: function (sourceAppointment, layoutInterval, appointmentsOffset) {
        if (sourceAppointment.interval.IsSmallerThanDay())
            this.DragShortAppointmentToCell(sourceAppointment, layoutInterval, appointmentsOffset);
        else
            this.DragLongAppointmentToCell(sourceAppointment, layoutInterval, appointmentsOffset);
    },
    DragLongAppointmentToCell: function (sourceAppointment, layoutInterval, appointmentsOffset) {
        if (this.showAllAppointmentsOnTimeCells) {
            var start = ASPx.SchedulerGlobals.DateIncreaseWithUtcOffset(sourceAppointment.interval.GetStart(), appointmentsOffset);
            sourceAppointment.operationInterval = new ASPxClientTimeInterval(start, sourceAppointment.interval.GetDuration());
        }
        else
            sourceAppointment.operationInterval = new ASPxClientTimeInterval(layoutInterval.GetStart(), layoutInterval.GetDuration());
    },
    DragShortAppointmentToCell: function (sourceAppointment, layoutInterval, appointmentsOffset) {
        var start = ASPx.SchedulerGlobals.DateIncreaseWithUtcOffset(sourceAppointment.interval.GetStart(), appointmentsOffset);
        sourceAppointment.operationInterval = new ASPxClientTimeInterval(start, sourceAppointment.interval.GetDuration());
        sourceAppointment.operationAllDay = false;
    },
    CalculateAppointmentsOffset: function (layoutInterval) {
        if (this.ShouldTruncToDate(layoutInterval))
            return ASPx.SchedulerGlobals.DateSubsWithTimezone(layoutInterval.GetStart(), ASPxSchedulerDateTimeHelper.TruncToDate(this.primaryAppointment.interval.GetStart()));
        else
            return this.constructor.prototype.CalculateAppointmentsOffset.call(this, layoutInterval);
    },
    ShouldTruncToDate: function (layoutInterval) {
        var dragToAllDayArea = !layoutInterval.IsSmallerThanDay();
        var isShortAppointment = this.primaryAppointment.interval.IsSmallerThanDay();
        return dragToAllDayArea && isShortAppointment;
    }
});

////////////////////////////////////////////////////////////////////////////////
// AppointmentOperationFlags
////////////////////////////////////////////////////////////////////////////////
var ASPxClientAppointmentFlags = ASPx.CreateClass(null, {
    constructor: function(){
        this.constructor.prototype.constructor.call(this);
        this.allowDelete = true;
        this.allowEdit = true;
        this.allowResize = true;
        this.allowCopy = true;
        this.allowDrag = true;
        this.allowDragBetweenResources = true;
        this.allowInplaceEditor = true;
        this.allowConflicts = true;

        //todo allowMultiSelect AllowDisplayAptForm
    }
});

////////////////////////////////////////////////////////////////////////////////
// ASPxClientAppointment
////////////////////////////////////////////////////////////////////////////////
var ASPxClientAppointment = ASPx.CreateClass(null, {
    constructor: function(interval, resources, flags, appointmentId, appointmentType, statusIndex, labelIndex){
        this.interval = interval;
        this.resources = resources;
        this.flags = flags;
        this.appointmentId = appointmentId;
        this.appointmentType = ( appointmentType ) ? appointmentType : ASPxAppointmentType.Normal;
        this.statusIndex = statusIndex;
        this.labelIndex = labelIndex;
        //all necessary properties here  like subject etc.
//	    this.subject = '';
//
//	    this.description = '';
//
//	    this.location = '';
//
//	    this.allDay = false;
    },
    __toJsonExceptKeys: ["flags"],
    GetToolTipContent: function(scheduler) {	
        var formatter = new ASPx.DateFormatter();
        var interval = ASPx.IsExists(this.operationInterval) ? this.operationInterval : this.interval;
        var startTimeFormat = this.SelectStartTimeFormat(scheduler, interval);
        var endTimeFormat = this.SelectEndTimeFormat(scheduler, interval);
        //formatter.SetFormatString("dd.MM.yyyy HH:mm");
        formatter.SetFormatString(startTimeFormat);
        var result = formatter.Format(interval.GetStart());
        if(ASPx.IsExists(endTimeFormat)) {
            formatter.SetFormatString(endTimeFormat);
            result += " - " + formatter.Format(interval.GetEnd());
        }
        return result;	    
    },
    SelectStartTimeFormat: function(scheduler, interval) {
        var intervalStart = interval.GetStart();
        var intervalEnd = interval.GetEnd();
        var startDate = intervalStart.getDate();
        var startYear = intervalStart.getYear();
        var startMonth = intervalStart.getMonth();
        var endDate = intervalEnd.getDate();
        var endYear = intervalEnd.getYear();
        var endMonth = intervalEnd.getMonth();
        var truncStartDate = new Date(startYear, startMonth, startDate);
        var truncEndDate = new Date(endYear, endMonth, endDate);
        var datesEquals = startDate == endDate && startMonth == endMonth && startYear == endYear;
        if(datesEquals) {
            if(interval.IsSmallerThanDay())
                return scheduler.formatsTimeWithMonthDay[0];
            else
                return scheduler.formatsWithoutYearAndWeekDay[0];
        }
        else {
            if(truncStartDate - interval.GetStart() == 0&& truncEndDate - interval.GetEnd() == 0) {	        
                if(startYear == endYear || interval.IsDurationEqualToDay())
                    return scheduler.formatsWithoutYearAndWeekDay[0];    	        
                else
                    return scheduler.formatsDateWithYear[0];    	        
            }
            else {
                if(startYear == endYear)
                    return scheduler.formatsTimeWithMonthDay[0];    	        
                else
                    return scheduler.formatsDateTimeWithYear[0];    	        
            }
       }
    },
    SelectEndTimeFormat: function(scheduler, interval) {
        var intervalStart = interval.GetStart();
        var intervalEnd = interval.GetEnd();
        var startDate = intervalStart.getDate();
        var startYear = intervalStart.getYear();
        var startMonth = intervalStart.getMonth();
        var endDate = intervalEnd.getDate();
        var endYear = intervalEnd.getYear();
        var endMonth = intervalEnd.getMonth();
        var truncStartDate = new Date(startYear, startMonth, startDate);
        var truncEndDate = new Date(endYear, endMonth, endDate);

        var datesEquals = startDate == endDate && startMonth == endMonth && startYear == endYear;
        if(datesEquals) {
            if(interval.IsSmallerThanDay())
                return scheduler.formatsTimeOnly[0];
            else
                return null;
        }
        else {//startDate != endDate
            if(truncStartDate - interval.GetStart() == 0&& truncEndDate - interval.GetEnd() == 0) {	        
                if(startYear == endYear || interval.IsDurationEqualToDay())
                    return (interval.IsDurationEqualToDay()) ? null : scheduler.formatsWithoutYearAndWeekDay[0];    	        
                else
                    return scheduler.formatsDateWithYear[0];    	        
            }
            else {
                if(startYear == endYear)
                    return scheduler.formatsTimeWithMonthDay[0];    	        
                else
                    return scheduler.formatsDateTimeWithYear[0];    	        
            }
       }
    },
    AddResource: function(resourceId) {
        if (!this.resources)
            this.resources = [];
        this.resources.push(resourceId);
    },
    GetResource: function(index) {
        var resources = this.GetResources();
        if (index >= 0 && index < resources.length)
            return resources[index];
        return null;
    },
    GetResources: function() {
        if (!this.resources)
            this.resources = [];
        return this.resources;
    },
    CreateDefaultTimeInterval: function() {
        return new ASPxClientTimeInterval(new Date(), 30 * 60 * 1000);
    },
    SetStart: function(start) {
        if (!this.interval) 
            this.interval = this.CreateDefaultTimeInterval();
        this.interval.SetStart(start);
    },
    GetStart: function() {
        if (!this.interval) 
            this.interval = this.CreateDefaultTimeInterval();
        return this.interval.GetStart();
    },
    SetEnd: function(end) {
        if (!this.interval) 
            this.interval = this.CreateDefaultTimeInterval();
        this.interval.SetEnd(end);
    },
    GetEnd: function() {
        if (!this.interval) 
            this.interval = this.CreateDefaultTimeInterval();
        return this.interval.GetEnd();
    },
    SetDuration: function(duration) {
        if (!this.interval)
            this.interval = this.CreateDefaultInterval();
        this.interval.SetDuration(duration);
    },
    GetDuration: function() {
        if (!this.interval)
            this.interval = this.CreateDefaultInterval();
        return this.interval.GetDuration();
    },
    SetId: function(id) {
        this.appointmentId = id;
    },
    GetId: function() {
        return this.appointmentId;
    },
    SetAppointmentType: function(type) {
        this.appointmentType = type;
    },
    GetAppointmentType: function() {
        return this.appointmentType;
    },
    SetStatusId: function(statusId) {
        this.statusIndex = statusId;
    },
    GetStatusId: function() {
        return this.statusIndex;
    },
    SetLabelId: function(labelId) {
        this.labelIndex = labelId;
    },
    GetLabelId: function() {
        return this.labelIndex;
    },
    SetSubject: function(subject) {
        this.subject = subject;
    },
    GetSubject: function() {
        if (!this.subject)
            return "";
        return this.subject;
    },
    SetDescription: function(description) {
        this.description = description;
    },
    GetDescription: function() {
        if (!this.description)
            return "";
        return this.description;
    },
    SetLocation: function(location) {
        this.location = location;
    },
    GetLocation: function() {
        if (!this.location)
            return "";
        return this.location;
    },
    SetAllDay: function(allDay) {
        this.allDay = allDay;
    },
    GetAllDay: function() {
        if (!this.allDay)
            return false;
        return this.allDay;
    },
    SetRecurrencePattern: function(recurrencePattern) {
        this.recurrencePattern = recurrencePattern;
    },
    GetRecurrencePattern: function() {
        return this.recurrencePattern;
    },
    SetRecurrenceInfo: function(recurrenceInfo) {
        this.recurrenceInfo = recurrenceInfo;
    },
    GetRecurrenceInfo: function() {
        return this.recurrenceInfo;
    },
    
    GetOperationInterval: function() {
        if (ASPx.IsExists(this.operationInterval))
            return this.operationInterval;
        return this.interval;
    },
    GetOperationResources: function() {
        if (ASPx.IsExists(this.operationResources))
            return this.operationResources;
        return this.GetResources();
    }
});
////////////////////////////////////////////////////////////////////////////////

/*
//----------------------------------------------
ASPxSchedulerCellInfo = new ASPx.CreateClass(null, {
    constructor: function(containerIndex, cellIndex) {
        this.containerIndex = containerIndex;
        this.cellIndex = cellIndex;
    }    
});

*/
////////////////////////////////////////////////////////////////////////////////
/// ASPxClientRectangle 
////////////////////////////////////////////////////////////////////////////////
var ASPxClientPoint = ASPx.CreateClass( null, {
    constructor: function(x, y) {
        this.x = x;
        this.y = y;
    },
    GetX: function() {
        return this.x;
    },
    GetY: function() {
        return this.y;
    }
});
var ASPxClientRect = ASPx.CreateClass( null, {
    constructor: function(x, y, width, height) {    
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    },
    GetLeft: function() {
        return this.x;
    },
    GetRight: function() {
        return this.x + this.width;
    },
    GetTop: function() {
        return this.y;
    },
    GetBottom: function() {
        return this.y + this.height;
    },
    GetWidth: function() {
        return this.width;
    },
    GetHeight: function() {
        return this.height;
    }
});

////////////////////////////////////////////////////////////////////////////////
/// AppointmentBounds
////////////////////////////////////////////////////////////////////////////////
var AppointmentBounds = ASPx.CreateClass(null, {
   constructor: function(x, y, width, height) {    
       this.x = x;
       this.y = y;
       this.width = width;
       this.height = height;
   },
   GetLeftBound: function() {
        return this.x;
   },
   GetRightBound: function() {
        return this.x + this.width;
   },
   GetTopBound: function() {
        return this.y;
   },
   GetBottomBound: function() {
        return this.y + this.height;
   },
   GetWidth: function() {
        return this.width;
   },   
   GetHeight: function() {
        return this.height;
   }
});

////////////////////////////////////////////////////////////////////////////////
// AppointmentLayoutCalculator
////////////////////////////////////////////////////////////////////////////////
var AppointmentLayoutCalculator = ASPx.CreateClass(null, {
    constructor: function(schedulerViewInfo, parent) {
        this.schedulerViewInfo = schedulerViewInfo;
        this.minAppointmentWidth = 7;
        this.minAppointmentHeight = 7;        
    },
    RecalculateInnerContentDivSize: function(appointmentViewInfos) {//B180241
        var count = appointmentViewInfos.length;
       
        this.divs = [];
        this.vi = [];
        for (var i = 0; i < count; i++) {
            var innerContentDiv = this.FindInnerContentDiv(appointmentViewInfos[i]);
            if (ASPx.IsExists(innerContentDiv)) {
                this.divs.push(innerContentDiv);
                this.vi.push(appointmentViewInfos[i]);
                this.PrepareSetInnerContentDivSize(innerContentDiv, appointmentViewInfos[i]);
            }
        }
        
        for (var i = this.divs.length - 1; i >= 0; i--) {
            this.CalculateSetInnerContentDivSizeParameters1(this.divs[i], this.vi[i]);
        }
        for (var i = this.divs.length - 1; i >= 0; i--) {
            this.CalculateSetInnerContentDivSizeParameters2(this.divs[i], this.vi[i]);
        }
        for (var i = this.divs.length - 1; i >= 0; i--) {
            this.CalculateSetInnerContentDivWidth(this.divs[i], this.vi[i]);
        }
        for (var i = this.divs.length - 1; i >= 0; i--) {
            this.SetInnerContentDivWidth(this.divs[i], this.vi[i]);
        }
        for (var i = this.divs.length - 1; i >= 0; i--) {
            this.CalculateSetInnerContentDivHeight(this.divs[i], this.vi[i]);
        }
        for (var i = this.divs.length - 1; i >= 0; i--) {
            this.SetInnerContentDivHeight(this.divs[i], this.vi[i]);
        }
    },
    AfterCalculateAppointments: function(appointmentViewInfos) {
        var count = appointmentViewInfos.length;
        for(var i = 0; i < count; i++)
            this.AfterCalculateAppointment(appointmentViewInfos[i]);
    },
    AfterCalculateAppointment: function(appointmentViewInfo) {
        var appointmentDiv = appointmentViewInfo.contentDiv;
        if(!ASPx.IsExists(appointmentDiv))
            return;
        //ASPx.SubscribeSchedulerMouseEvents(appointmentDiv, appointmentViewInfo);        
        appointmentDiv.viewInfo = appointmentViewInfo;
        appointmentViewInfo.SubscribeMouseEvents();
        appointmentViewInfo.toolTip = this.schedulerViewInfo.scheduler.appointmentToolTip;
        this.CalculateFinalContentLayout(appointmentViewInfo);
    },
    CalculateFinalContentLayout: function(appointmentViewInfo) {
        this.LayoutAppointmentStatus(appointmentViewInfo);	    
    },	
    LayoutAppointmentStatus : function(appointmentViewInfo) {	   

        var statusBackDivId = appointmentViewInfo.statusViewInfo.backDivId;
        var statusForeDivId = appointmentViewInfo.statusViewInfo.foreDivId;
        if (ASPx.IsExists(statusBackDivId) && ASPx.IsExists(statusForeDivId)) {
            var statusBackDiv = this.schedulerViewInfo.scheduler.GetAppointmentBlockElementById(statusBackDivId);
            if (!ASPx.IsExists(statusBackDiv))
                return;
            var statusForeDiv = this.schedulerViewInfo.scheduler.GetAppointmentBlockElementById(statusForeDivId);
            if (!ASPx.IsExists(statusForeDiv))
                return;
                var innerContentDiv = this.FindInnerContentDiv(appointmentViewInfo);
                if (!ASPx.IsExists(innerContentDiv))
                    innerContentDiv = appointmentViewInfo.contentDiv;	             
                statusBackDiv.appointmentDiv = innerContentDiv;
                statusForeDiv.appointmentDiv = innerContentDiv;	            
                this.LayoutAppointmentStatusCore(statusForeDiv, statusBackDiv, appointmentViewInfo);	            
            }
    },
    ResetContentDivSize : function(appointmentViewInfo) {
        var innerContentDiv = this.FindInnerContentDiv(appointmentViewInfo);
        if(ASPx.IsExists(innerContentDiv))
            innerContentDiv.style.height = "";		
    },

    FindInnerContentDiv : function(appointmentViewInfo) {
        var appointmentDiv = appointmentViewInfo.contentDiv; 
        if(!ASPx.IsExists(appointmentDiv))
            return null;
        var children = appointmentDiv.childNodes;
        var count = children.length;
        for(var i = 0; i < count; i++) {
            var child = children[i];
            if(this.IsInnerContentDiv(child))		        
                return child;            
        }
        return null;
    },
    IsInnerContentDiv : function(child) {	    
        if (!ASPx.IsExists(child.tagName))
            return false;
        else {
            var tagName = child.tagName.toUpperCase();
            return ((tagName == "TABLE") || (tagName == "DIV"));
        }
    },
    PrepareSetInnerContentDivSize: function(contentDiv, aptViewInfo) {
        if(ASPx.Browser.IE) { //Q152414
            if (contentDiv.clientHeight <= 0)
                contentDiv.style.height = "0";
            if (contentDiv.clientWidth <= 0)
                contentDiv.style.width = "0";
        }
    },
    CalculateSetInnerContentDivSizeParameters1: function(contentDiv, aptViewInfo) {
        var bordersHeight = contentDiv.offsetHeight - contentDiv.clientHeight;   // borders + paddings!
        var bordersWidth = contentDiv.offsetWidth - contentDiv.clientWidth;   // borders + paddings!
        aptViewInfo.bordersHeight = bordersHeight;
        aptViewInfo.bordersWidth = bordersWidth;

    },
    CalculateSetInnerContentDivSizeParameters2: function(contentDiv, aptViewInfo) {
        contentDiv.style.height = "";
    },
    CalculateSetInnerContentDivWidth: function(contentDiv, aptViewInfo) {
        var bordersWidth = aptViewInfo.bordersWidth;   // borders + paddings!    
        aptViewInfo.innerStyleWidth = Math.max(0, aptViewInfo.contentDiv.offsetWidth - bordersWidth) + "px";
    },
    CalculateSetInnerContentDivHeight: function(contentDiv, aptViewInfo) {
        var bordersHeight = aptViewInfo.bordersHeight;   // borders + paddings!
        aptViewInfo.innerStyleHeight = Math.max(aptViewInfo.contentDiv.offsetHeight - bordersHeight, this.minAppointmentHeight) + "px";
    },
    SetInnerContentDivWidth: function(contentDiv, aptViewInfo) {
        contentDiv.style.width = aptViewInfo.innerStyleWidth;
    },
    SetInnerContentDivHeight: function(contentDiv, aptViewInfo) {
        contentDiv.style.height = aptViewInfo.innerStyleHeight;
    },
    RecalcAppointmentIntervalAndOffset: function(viewInfo, aptBounds, firstCell, lastCell) {
        var firstCellInterval = this.schedulerViewInfo.scheduler.GetCellInterval(firstCell);
        var lastCellInterval = this.schedulerViewInfo.scheduler.GetCellInterval(lastCell);
        viewInfo.startRelativeIndent = this.RecalcStartOffset(aptBounds, firstCell);
        viewInfo.endRelativeIndent = this.RecalcEndOffset(aptBounds, lastCell);		
        
    
        var start = this.CalculateStartTimeByOffset(firstCellInterval, viewInfo.startRelativeIndent);
        var end = this.CalculateEndTimeByOffset(lastCellInterval, viewInfo.endRelativeIndent);				
        viewInfo.appointmentInterval.SetStart(start);
        viewInfo.appointmentInterval.SetDuration(end - start);		
    },
    CalculateStartTimeByOffset: function(baseInterval, startOffset) {
        var offset =  baseInterval.GetDuration() * startOffset / 100;
        return ASPx.SchedulerGlobals.DateIncrease(baseInterval.GetStart(), offset);
    },
    CalculateEndTimeByOffset: function(baseInterval, endOffset) {
        var offset =  - baseInterval.GetDuration() * endOffset / 100;
        return ASPx.SchedulerGlobals.DateIncrease(baseInterval.GetEnd(), offset);		
    },
    RecalcStartOffset: function(aptBounds, firstCell) {
        //abstract function
    },
    RecalcEndOffset: function(aptBounds, lastCell) {
        //abstract function
    }	
});

///////////////////////////////////////////////////////////////////////////////
// VerticalAppointmentLayoutCalculator
////////////////////////////////////////////////////////////////////////////////
var VerticalAppointmentLayoutCalculator = ASPx.CreateClass(AppointmentLayoutCalculator, {
    constructor: function(schedulerViewInfo, parent, disableSnapToCells) {
        this.constructor.prototype.constructor.call(this, schedulerViewInfo);
        this.gapBetweenAppointment = 4;
        this.leftAppointmentIndent = 2;
        this.rightAppointmentIndent = 2;	    	    
        this.parent = parent;
        this.disableSnapToCells = disableSnapToCells;
        
    },
    CalculateLayout: function(appointmentViewInfos) {
        if(!ASPx.IsExists(appointmentViewInfos) || !ASPx.IsExists(appointmentViewInfos.length))    
            return;
        var count = appointmentViewInfos.length;
        if(count <= 0)
            return;
        var count = appointmentViewInfos.length;
        for(var i = 0; i < count; i ++)
            this.CalculateAppointmentLayout(appointmentViewInfos[i]);
        this.RecalculateInnerContentDivSize(appointmentViewInfos);
        this.AfterCalculateAppointments(appointmentViewInfos); 
   },
   CalculateAppointmentLayout: function(viewInfo){
        viewInfo.ResetRelativeIndentAndTime();
        this.ResetContentDivSize(viewInfo);
        var schedulerViewInfo = this.schedulerViewInfo;
        //var div = schedulerViewInfo.GetAppointmentDivById(viewInfo.divId);
        var div = viewInfo.contentDiv;
        
        if(ASPx.IsExists(div)){
            //viewInfo.contentDiv = div;
            //viewInfo.contentDiv.appointmentId = viewInfo.appointmentId;
            //div.appointmentViewInfo = viewInfo;
            var startCell = ASPx.IsExists(viewInfo.startCell) ? viewInfo.startCell : schedulerViewInfo.GetCell(viewInfo.containerIndex, viewInfo.firstCellIndex) ;
            var endCell = ASPx.IsExists(viewInfo.endCell) ? viewInfo.endCell : schedulerViewInfo.GetCell(viewInfo.containerIndex, viewInfo.lastCellIndex);
            var leftColumnPadding = this.schedulerViewInfo.scheduler.leftColumnPadding;
            var rightColumnPadding = this.schedulerViewInfo.scheduler.rightColumnPadding;
            if(ASPx.IsExists(startCell) && ASPx.IsExists(endCell)){                
                ASPx.SetSchedulerDivDisplay(div, true);
                viewInfo.startCell = startCell;
                viewInfo.endCell = endCell;
                var paddingBeforeAppointment = this.GetPaddingBeforeAppointment(viewInfo);
                var paddingAfterAppointment = this.GetPaddingAfterAppointment(viewInfo);
                var factor = (startCell.offsetWidth - (leftColumnPadding + rightColumnPadding)) / viewInfo.maxIndexInGroup;
                
                /*var innertDiv = this.FindInnerContentDiv(viewInfo);                
                var borderWidth = innertDiv.offsetWidth - innertDiv.clientWidth;                
                var borderHeight = innertDiv.offsetHeight - innertDiv.clientHeight;*/
                
                var topIndent = this.GetIndent(startCell, viewInfo.startRelativeIndent);
                var bottomIndent = this.GetIndent(endCell, viewInfo.endRelativeIndent);
                var aptLeft = Math.floor(leftColumnPadding + schedulerViewInfo.parent.CalcRelativeElementLeft(startCell) + viewInfo.startHorizontalIndex * factor  + paddingBeforeAppointment);               
                var aptWidth = Math.floor((viewInfo.endHorizontalIndex - viewInfo.startHorizontalIndex) * factor - (paddingAfterAppointment + paddingBeforeAppointment)/* - borderWidth*/);

                var aptTop = Math.round(schedulerViewInfo.parent.CalcRelativeElementTop(startCell) + topIndent); //!!!~~~
                var aptBottom = Math.round(schedulerViewInfo.parent.CalcRelativeElementBottom(endCell) - bottomIndent); //!!!~~~
                if (ASPx.Browser.Firefox)
                    aptTop = aptTop - 1;
                
                var aptHeight = aptBottom - aptTop /*- borderHeight */;               
                               
                    
                var aptBounds = new AppointmentBounds(aptLeft, aptTop, aptWidth, aptHeight);
                if(this.ShouldExtendAppointmentBounds(aptHeight)) {
                    this.ExtendAppointmentBounds(aptBounds, startCell, endCell);
                    this.RecalcAppointmentIntervalAndOffset(viewInfo, aptBounds, startCell, endCell);
                }
                
                div.style.left = aptLeft + "px";//!!!!!
                div.style.width = Math.max(aptWidth, 0)  + "px";
                div.style.top = aptBounds.GetTopBound() + "px";
                div.style.height = aptBounds.GetHeight() + "px";
            }
        }
    },
    ShouldExtendAppointmentBounds: function(aptHeight) {
        if (aptHeight >= this.minAppointmentHeight)
            return false;    
        if(ASPx.IsExists(this.disableSnapToCells) && this.disableSnapToCells)
            return false;
        return true;
    },
    ExtendAppointmentBounds: function(aptBounds, firstCell, lastCell) {
        var firstCellTopBound = this.schedulerViewInfo.parent.CalcRelativeElementTop(firstCell);
        var lastCellBottomBound = this.schedulerViewInfo.parent.CalcRelativeElementBottom(lastCell);	    
        //Extending BottomBound
        aptBounds.height = Math.min(this.minAppointmentHeight, lastCellBottomBound - aptBounds.GetTopBound());	   			
        
        //Extending TopBound
        if (aptBounds.height < this.minAppointmentHeight) 			
            aptBounds.y = Math.max(firstCellTopBound, aptBounds.GetBottomBound() - this.minAppointmentHeight);		
    },
    RecalcStartOffset: function(aptBounds, firstCell) {
        var cellTopBound = this.schedulerViewInfo.parent.CalcRelativeElementTop(firstCell);
        var cellHeight = this.schedulerViewInfo.parent.CalcRelativeElementBottom(firstCell) - cellTopBound;
        return Math.floor((aptBounds.GetTopBound() - cellTopBound) / cellHeight * 100);
    },
    RecalcEndOffset: function(aptBounds, lastCell) {
        var cellBottomBound = this.schedulerViewInfo.parent.CalcRelativeElementBottom(lastCell);
        var cellHeight = cellBottomBound - this.schedulerViewInfo.parent.CalcRelativeElementTop(lastCell);
        return Math.floor((cellBottomBound - aptBounds.GetBottomBound()) / cellHeight * 100);		
    },
    GetPaddingBeforeAppointment: function(viewInfo) {
        if (this.IsFirstAppointment(viewInfo))
            return this.leftAppointmentIndent;
        else
            return this.gapBetweenAppointment / 2;
    },
    GetPaddingAfterAppointment: function(viewInfo) {
        if (this.IsLastAppointment(viewInfo))
            return this.rightAppointmentIndent;
        else
            return this.gapBetweenAppointment / 2;
    },
    IsLastAppointment: function(viewInfo) {
        return viewInfo.endHorizontalIndex == viewInfo.maxIndexInGroup;
    },
    IsFirstAppointment: function(viewInfo) {
        return viewInfo.startHorizontalIndex == 0;
    },
    GetIndent: function(cell, relativeIndent) {
        if(!ASPx.IsExists(relativeIndent))
            return 0;
        else
            return relativeIndent * cell.offsetHeight / 100;
    },		
    LayoutAppointmentStatusCore : function(statusForeDiv, statusBackDiv, appointmentViewInfo) {	  
        var statusViewInfo = appointmentViewInfo.statusViewInfo;
        var aptDiv = statusBackDiv.appointmentDiv;	    
        var height = aptDiv.clientHeight;	    	    
        var topBorderHeight = this.CalculateTopBorderHeight(appointmentViewInfo, aptDiv);
        
        //BackGround
        statusBackDiv.style.height = height + "px";	    
        statusBackDiv.style.top =  topBorderHeight  +  "px";	    
        
        //ForeGround
        var topIndent = Math.floor(statusViewInfo.startOffset * height / 100);
        var bottomIndent = Math.floor(statusViewInfo.endOffset * height / 100);
        statusForeDiv.style.top =  topIndent  + topBorderHeight + "px";	        
        statusForeDiv.style.height =  height - topIndent - bottomIndent + "px";
    },
    CalculateTopBorderHeight: function(aptViewInfo, aptDiv) {
        // Suggesdted that appointment's top and bottom borders has the same height, and there is no top and bottom paddings.
        var aptBordersHeight = aptDiv.offsetHeight - aptDiv.clientHeight;
        if (!aptViewInfo.hasTopBorder)
            return 0;
        if (aptViewInfo.hasBottomBorder)
            return Math.ceil(aptBordersHeight / 2);
        else
            return aptBordersHeight;
    }
});
////////////////////////////////////////////////////////////////////////////////

var BusyInterval = ASPx.CreateClass(null, {
    constructor: function(start, end) {
        this.start = start;
        this.end = end;
    },
    ContainsExcludeEndBound: function(value) {
        return this.start <= value && this.end > value;
    }
});



////////////////////////////////////////////////////////////////////////////////
///////// HorizontalAppointmentLayoutCalculator            ///////////
////////////////////////////////////////////////////////////////////////////////
var HorizontalAppointmentLayoutCalculator = ASPx.CreateClass(AppointmentLayoutCalculator, {
    constructor: function (schedulerViewInfo, disableSnapToCells) {
        this.constructor.prototype.constructor.call(this, schedulerViewInfo);
        this.gapBetweenAppointments = 1 + schedulerViewInfo.scheduler.appointmentVerticalInterspacing;
        this.leftAppointmentIndent = 2;
        this.rightAppointmentIndent = 2;
        this.topAppointmentIndent = 2;
        this.bottomLastAppointmentIndent = 2;
        this.schedulerViewInfo = schedulerViewInfo;
        this.maxCellsInWeek = 7;
        this.appointmentNotFitted = 0;
        this.appointmentPartialFitted = 1;
        this.appointmentFitted = 2;
        this.disableSnapToCells = disableSnapToCells;
        this.containerSizeCache = [];
    },
    //public
    CalculateLayout: function (appointmentViewInfos) {
        this.RecalculateContainersCellCache(this.schedulerViewInfo.cellContainers);
        if (!ASPx.IsExists(appointmentViewInfos) || !ASPx.IsExists(appointmentViewInfos.length))
            return;
        var count = appointmentViewInfos.length;
        if (count <= 0)
            return;
        this.PrepareViewInfos(appointmentViewInfos);
        var containerCount = this.schedulerViewInfo.cellContainers.length;
        this.apts = [];
        this.cellWithMoreButtons = [];
        this.moreButtonViewInfos = [];
        this.aptPositions = []; ;

        for (var i = 0; i < containerCount; i++) {
            var containerViewInfos = this.SelectViewInfosForContainer(appointmentViewInfos, this.schedulerViewInfo.cellContainers[i].containerIndex);
            if (containerViewInfos.length > 0)
                this.CalculateLayoutCore(containerViewInfos);
        }

        this.RecalculateCellsHeight();

        for (var i = this.apts.length - 1; i >= 0; i--) {
            var posInfo = this.aptPositions[i];
            if (posInfo != null)
                this.apts[i].style.top = this.CalculateAppointmentTopFromPositionInfo(posInfo);
            else
                ASPx.SetSchedulerDivDisplay(this.apts[i], false);
        }
        if (this.schedulerViewInfo.moreButtonDiv)
            this.schedulerViewInfo.moreButtonDiv.style.display = "none";
        for (var i = this.cellWithMoreButtons.length - 1; i >= 0; i--) {
            this.schedulerViewInfo.ShowMoreButton(this.cellWithMoreButtons[i], this.moreButtonViewInfos[i]); //TODO: optimize, reflow on each button
        }

        this.AfterCalculateAppointments(appointmentViewInfos);
    },
    //public
    CalculateAppointmentLayoutAtOnce: function (viewInfo) {
        viewInfo.height = viewInfo.contentDiv.offsetHeight;
        this.CalculateAppointmentSize(viewInfo);
        this.ApplyPrepare(viewInfo);
        this.apts = [];
        this.cellWithMoreButtons = [];
        this.aptPositions = [];
        this.CalculateAppointmentCellsHeight(viewInfo);
        this.CalculateAppointmentPosition(viewInfo, true);
        viewInfo.contentDiv.style.height = viewInfo.height + "px";
        var count = this.apts.length;
        for (var i = 0; i < count; i++) {
            if (this.aptPositions[i])
                this.apts[i].style.top = this.CalculateAppointmentTopFromPositionInfo(this.aptPositions[i]);
            else
                ASPx.SetSchedulerDivDisplay(this.apts[i], false);
        }
    },
    //all other private
    RecalculateCellsHeight: function() {
    },
    CalculateAppointmentTopFromPositionInfo: function (posInfo) {
        var cell = this.schedulerViewInfo.GetCell(posInfo.containerIndex, posInfo.cellIndex);
        return posInfo.relativePosition + this.schedulerViewInfo.parent.CalcRelativeElementTop(cell) + "px";
    },
    CalculateAppointmentCellsHeight: function (viewInfo) {
        var containerIndex = viewInfo.containerIndex;
        if (!this.containerSizeCache[containerIndex])
            this.containerSizeCache[containerIndex] = [];
        for (var i = viewInfo.firstCellIndex; i <= viewInfo.lastCellIndex; i++)
            this.UpdateCellContainerSizeCache(containerIndex, i);
    },
    RecalculateContainersCellCache: function (cellContainers) {
        this.containerSizeCache = [];
        var containerCount = cellContainers.length;
        for (var i = 0; i < containerCount; i++)
            this.RecalculateContainerCellCache(cellContainers[i]);
    },
    RecalculateContainerCellCache: function (container) {
        //var container = this.schedulerViewInfo.cellContainers[containerIndex];
        var cellCount = container.cellCount;
        var containerIndex = container.containerIndex;
        this.containerSizeCache[containerIndex] = [];
        for (var i = 0; i < cellCount; i++)
            this.UpdateCellContainerSizeCache(containerIndex, i);
    },
    UpdateCellContainerSizeCache: function (containerIndex, cellIndex) {
        var cell = this.schedulerViewInfo.GetCell(containerIndex, cellIndex);
        this.containerSizeCache[containerIndex][cellIndex] = cell.clientHeight;
    },
    SetCellContainerSizeCache: function (containerIndex, cellIndex, cellHeight) {
        this.containerSizeCache[containerIndex][cellIndex] = cellHeight;
    },
    GetCellHeight: function (containerIndex, cellIndex) {
        return this.containerSizeCache[containerIndex][cellIndex];
    },
    SelectViewInfosForContainer: function (viewInfos, containerIndex) {
        var count = viewInfos.length;
        var result = [];
        for (var i = 0; i < count; i++) {
            if (viewInfos[i].containerIndex == containerIndex)
                result.push(viewInfos[i]);
        }
        return result;
    },
    CalculateLayoutCore: function (viewInfos) {
        var count = viewInfos.length;
        var relativePositionCalculator = this.CreateRelativePositionCalculator();
        var index = 0;
        do {
            relativePositionCalculator.CalculateAppointmentRelativePositions(viewInfos, index);
            index = this.CalculateAppointmentsPosition(viewInfos, index);
        } while (index < count);
    },
    CreateRelativePositionCalculator: function () {
        return new HorizontalAppointmentRelativePositionCalculator(this.gapBetweenAppointments);
    },
    PrepareViewInfos: function (viewInfos) {
        var count = viewInfos.length;
        var moreButtonDiv = this.schedulerViewInfo.moreButtonDiv;
        if (moreButtonDiv) {
            moreButtonDiv.style.top = 0;
            moreButtonDiv.style.left = 0;
            moreButtonDiv.style.display = "block";
        }
        for (var i = 0; i < count; i++) {
            var div = viewInfos[i].contentDiv;
            if (div) {
                ASPx.SetSchedulerDivDisplay(div, true);
                div.style.width = "";
            }
            this.ResetContentDivSize(viewInfos[i]);
            this.CalculateAppointmentHeight(viewInfos[i]);

        }
        for (var i = 0; i < count; i++) {
            this.PrepareViewInfo(viewInfos[i]);
        }
        for (var i = 0; i < count; i++) {
            this.ApplyPrepare(viewInfos[i]);
        }
        this.RecalculateInnerContentDivSize(viewInfos); //B180241
        for (var i = 0; i < count; i++) {
            viewInfos[i].height = viewInfos[i].contentDiv.offsetHeight;
        }
        if (moreButtonDiv) {
            this.schedulerViewInfo.moreButtonSize = moreButtonDiv.offsetHeight;
            this.schedulerViewInfo.moreButtonWidth = moreButtonDiv.offsetWidth;
        }

    },
    PrepareViewInfo: function (viewInfo) {
        viewInfo.ResetRelativeIndentAndTime();

        var div = viewInfo.contentDiv;
        viewInfo.visibleFirstCellIndex = viewInfo.firstCellIndex;
        viewInfo.visibleLastCellIndex = viewInfo.lastCellIndex;
        if (ASPx.IsExists(div)) {
            //ASPx.SetSchedulerDivDisplay(div, true);
            this.CalculateAppointmentSize(viewInfo);
        }
    },
    ApplyPrepare: function (viewInfo) {
        var div = viewInfo.contentDiv;
        div.style.left = viewInfo.styleLeft;
        div.style.width = viewInfo.styleWidth;
    },
    CalculateAppointmentSize: function (viewInfo) {
        var containerIndex = viewInfo.containerIndex;
        var firstCell = this.schedulerViewInfo.GetCell(containerIndex, viewInfo.visibleFirstCellIndex);
        var lastCell = this.schedulerViewInfo.GetCell(containerIndex, viewInfo.visibleLastCellIndex);
        if (ASPx.IsExists(firstCell) && ASPx.IsExists(lastCell)) {
            this.CalculateAppointmentWidthAndPosition(viewInfo, firstCell, lastCell);
            //viewInfo.height = viewInfo.contentDiv.offsetHeight;
            viewInfo.visibleFirstCell = firstCell;
            viewInfo.visibleLastCell = lastCell;
        }
    },
    CalculateAppointmentHeight: function (viewInfo) {
        var div = viewInfo.contentDiv;
        this.ResetInnerContentDivHeight(viewInfo);
        var aptHeight = this.schedulerViewInfo.scheduler.privateAppointmentHeight;
        if (aptHeight != 0)
            div.style.height = aptHeight + "px";
        else
            div.style.height = "";
    },
    CalculateAppointmentWidthAndPosition: function (viewInfo, firstCell, lastCell) {
        var div = viewInfo.contentDiv;
        var borderWidth = div.offsetWidth - div.clientWidth;
        var leftIndent = Math.floor(this.GetIndent(firstCell, viewInfo.startRelativeIndent) + this.leftAppointmentIndent);
        var rightIndent = Math.floor(this.GetIndent(lastCell, viewInfo.endRelativeIndent) + this.rightAppointmentIndent);
        var left = Math.floor(this.schedulerViewInfo.parent.CalcRelativeElementLeft(firstCell) + leftIndent); //!!!!!
        var width = Math.floor(this.schedulerViewInfo.parent.CalcRelativeElementRight(lastCell) - left - rightIndent); //!!!~~~
        var aptBounds = new AppointmentBounds(left, 0, width, div.offsetHeight);
        if (this.ShouldExtendAppointmentBounds(width)) {
            this.ExtendAppointmentBounds(aptBounds, firstCell, lastCell);
            this.RecalcAppointmentIntervalAndOffset(viewInfo, aptBounds, firstCell, lastCell);
        }
        viewInfo.styleLeft = aptBounds.GetLeftBound() + "px";
        viewInfo.styleWidth = Math.max(aptBounds.GetWidth() - borderWidth, 1) + "px";
        //div.style.left = aptBounds.GetLeftBound() + "px";
        //div.style.width = Math.max(aptBounds.GetWidth() - borderWidth, 0) + "px";
    },
    ShouldExtendAppointmentBounds: function (width) {
        if (width >= this.minAppointmentWidth)
            return false;
        if (ASPx.IsExists(this.disableSnapToCells) && this.disableSnapToCells)
            return false;
        return true;
    },
    ResetInnerContentDivHeight: function (aptViewInfo) {
        var innerDiv = this.FindInnerContentDiv(aptViewInfo);
        if (ASPx.IsExists(innerDiv)) {
            innerDiv.style.height = "";
            innerDiv.style.width = "";
        }
    },
    ExtendAppointmentBounds: function (aptBounds, firstCell, lastCell) {
        var firstCellLeftBound = this.schedulerViewInfo.parent.CalcRelativeElementLeft(firstCell);
        var lastCellRightBound = this.schedulerViewInfo.parent.CalcRelativeElementRight(lastCell);
        //Extending RightBound
        aptBounds.width = Math.min(this.minAppointmentWidth, lastCellRightBound - aptBounds.GetLeftBound());

        //Extending LeftBound
        if (aptBounds.width < this.minAppointmentWidth)
            aptBounds.x = Math.max(firstCellLeftBound, aptBounds.GetRightBound() - this.minAppointmentWidth);
    },
    RecalcStartOffset: function (aptBounds, firstCell) {
        var cellLeftBound = this.schedulerViewInfo.parent.CalcRelativeElementLeft(firstCell);
        var cellWidth = this.schedulerViewInfo.parent.CalcRelativeElementRight(firstCell) - cellLeftBound;
        return Math.floor((aptBounds.GetLeftBound() - cellLeftBound) / cellWidth * 100);
    },
    RecalcEndOffset: function (aptBounds, lastCell) {
        var cellRightBound = this.schedulerViewInfo.parent.CalcRelativeElementRight(lastCell);
        var cellWidth = cellRightBound - this.schedulerViewInfo.parent.CalcRelativeElementLeft(lastCell);
        return Math.floor((cellRightBound - aptBounds.GetRightBound()) / cellWidth * 100);
    },
    CalculateAppointmentsPosition: function (viewInfos, startIndex) {
        var count = viewInfos.length;
        for (var i = startIndex; i < count; i++) {
            var viewInfo = viewInfos[i];
            var result = this.CalculateAppointmentPosition(viewInfo, false);
            if (result == this.appointmentPartialFitted) {
                this.CalculateAppointmentSize(viewInfo);
                //this.PrepareViewInfo(viewInfo);
                this.ApplyPrepare(viewInfo);
                return i;
            }
        }
        return count;
    },
    CalculateAppointmentPosition: function (viewInfo, dragMode) {
        var firstCell = viewInfo.visibleFirstCell;
        var dxtop = viewInfo.relativePosition + this.topAppointmentIndent; //!!!~~~
        var bottom = dxtop + viewInfo.height;
        var fitCount = 0;

        var container = this.schedulerViewInfo.cellContainers[viewInfo.containerIndex];
        for (var i = viewInfo.visibleFirstCellIndex; i <= viewInfo.visibleLastCellIndex; i++) {
            var cell = this.schedulerViewInfo.GetCell(viewInfo.containerIndex, i);
            var isFitted = this.CalculateIsFitted(viewInfo.containerIndex, i, bottom);
            if (!isFitted && !dragMode) {//!!!~~~
                if (this.TryExpandCellSize(viewInfo.containerIndex, i, bottom + this.bottomLastAppointmentIndent)) {
                    fitCount++;
                    continue;
                }
                this.cellWithMoreButtons.push(cell);
                this.moreButtonViewInfos.push(viewInfo);
                if (i > viewInfo.visibleFirstCellIndex && i < viewInfo.visibleLastCellIndex && fitCount > 0) {
                    //Cell don't fit in the middle cells. This is internal error.
                    throw new Error("internal scheduler error");
                }
                var prevCellsFitted = fitCount > 0;
                if (viewInfo.visibleFirstCellIndex < viewInfo.visibleLastCellIndex && i == viewInfo.visibleLastCellIndex && prevCellsFitted) {
                    viewInfo.visibleLastCellIndex--;
                    return this.appointmentPartialFitted;
                }
            }
            else
                fitCount++;

        }
        this.apts.push(viewInfo.contentDiv);
        if (fitCount == 0) {
            this.aptPositions.push(null);
            return this.appointmentNotFitted;
        }
        else {
            var positionInfo = { relativePosition: viewInfo.relativePosition + this.topAppointmentIndent,
                cellIndex: viewInfo.visibleFirstCellIndex,
                containerIndex: container.containerIndex
            };
            this.aptPositions.push(positionInfo);
            return this.appointmentFitted;
        }
    },
    GetIndent: function (cell, relativeIndent) {
        if (!ASPx.IsExists(relativeIndent))
            return 0;
        else
            return relativeIndent * (cell.offsetWidth - this.leftAppointmentIndent - this.rightAppointmentIndent) / 100;
    },
    CalculateIsFitted: function (containerIndex, cellIndex, bottom) {
        var cellHeight = this.GetCellHeight(containerIndex, cellIndex);
        return (bottom + this.schedulerViewInfo.GetMoreButtonSize()) < cellHeight;
    },
    LayoutAppointmentStatusCore: function (statusForeDiv, statusBackDiv, appointmentViewInfo) {
        var statusViewInfo = appointmentViewInfo.statusViewInfo;
        statusBackDiv.style.width = statusBackDiv.appointmentDiv.clientWidth + "px";
        var leftIndent = Math.ceil(statusViewInfo.startOffset * statusBackDiv.offsetWidth / 100);
        var rightIndent = Math.ceil(statusViewInfo.endOffset * statusBackDiv.offsetWidth / 100);
        statusForeDiv.style.top = "1px";
        statusForeDiv.style.left = leftIndent + 1 + "px";
        statusForeDiv.style.width = Math.max(statusBackDiv.offsetWidth - leftIndent - rightIndent, 1) + "px";
    },
    TryExpandCellSize: function (containerIndex, cellIndex, bottom) {
        return false;
    }
});

////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////
var CellsAutoHeightClientHorizontalAppointmentLayoutCalculator = ASPx.CreateClass(HorizontalAppointmentLayoutCalculator, {
    constructor: function (schedulerViewInfo, disableSnapToCells) {
        this.constructor.prototype.constructor.call(this, schedulerViewInfo, disableSnapToCells);
    },
    TryExpandCellSize: function (containerIndex, cellIndex, bottom) {
        var container = this.schedulerViewInfo.cellContainers[containerIndex];
        var cellConstrant = container.cellConstraint[cellIndex];
        var currentCellHeight = this.GetCellHeight(containerIndex, cellIndex);

        var moreButtonSize = this.schedulerViewInfo.GetMoreButtonSize();

        if (cellConstrant.maxHeight >= 0)
            bottom += moreButtonSize;
        var desiredHeight = Math.max(currentCellHeight, bottom);
        var canExpandCell = cellConstrant.maxHeight <= 0 || desiredHeight < cellConstrant.maxHeight

        var cell = this.schedulerViewInfo.GetCell(containerIndex, cellIndex);
        if (!cell.reduceSize)
            cell.reduceSize = moreButtonSize;

        if (!canExpandCell)
            cell.reduceSize = 0;
        else
            this.SetCellContainerSizeCache(containerIndex, cellIndex, desiredHeight);
        return canExpandCell;
    },
    RecalculateCellsHeight: function () {
        var containers = this.schedulerViewInfo.cellContainers
        var containerCount = containers.length;
        //recalc cells
        var compressedCells = [];
        var compressedCellsHeaderHeight = [];
        for (var i = 0; i < containerCount; i++) {
            var cellContainer = containers[i];
            compressedCells[i] = [];
            var headerCell = this.schedulerViewInfo.GetMiddleCompressedCellsHeader(i);
            compressedCellsHeaderHeight[i] = (headerCell) ? headerCell.clientHeight : 0;
            var cellHeights = cellContainer.maxCellHeights;
            for (var cellIndex = 0; cellIndex < cellContainer.cellCount; cellIndex++) {
                var cell = this.schedulerViewInfo.GetCell(i, cellIndex);
                var cellHeight = this.GetCellHeight(i, cellIndex);
                if (cell.reduceSize) {
                    cellHeight -= cell.reduceSize;
                    cell.reduceSize = null;
                }
                cellHeight = Math.max(cellContainer.cellConstraint[cellIndex].minHeight, cellHeight);
                if (cellContainer.GetCellLocation(cellIndex).isCompressed) {
                    cell.style.height = cellHeight + "px";
                    compressedCells[i].push(cellIndex);
                }
                else
                    cell.style.height = cellHeight + "px";
                cell.isCalculated = false;
            }
        }

        this.schedulerViewInfo.parent.ResetCache();
        this.CorrectCompressedCells(containers, compressedCells, compressedCellsHeaderHeight);        
    },
    CorrectCompressedCells: function (containers, compressedCells, compressedCellsHeaderHeight) {
        var containerCount = containers.length;
        for (var i = 0; i < containerCount; i++) {
            var cellContainer = containers[i];
            var headerCell = this.schedulerViewInfo.GetMiddleCompressedCellsHeader(i);
            if (headerCell == null)
                continue;
            var desiredHeaderHeight = compressedCellsHeaderHeight[i];
            if (headerCell.clientHeight > desiredHeaderHeight) { // todo: refact this code - potential visual bug in layout compressed cells
                var bugOffset = (headerCell.clientHeight - desiredHeaderHeight);
                var totalSize = 0;
                //assume, that only 2 compressed cells
                var firstCellCalculatedHeight = this.GetCellHeight(i, compressedCells[i][0]);
                var secondCellCalculatedHeight = this.GetCellHeight(i, compressedCells[i][1]);
                var firstCell = this.schedulerViewInfo.GetCell(i, compressedCells[i][0]);
                var secondCell = this.schedulerViewInfo.GetCell(i, compressedCells[i][1]);
                var firstCellClientHeight = firstCell.clientHeight;
                var secondCellClientHeight = secondCell.clientHeight;
                //debugger;
                var totalDesiredClientCellHeight = (firstCellClientHeight + secondCellClientHeight + bugOffset) / 2;
                var totalCorrection = bugOffset;
                if (firstCellCalculatedHeight > totalDesiredClientCellHeight) {
                    //totalCorrection -= firstCellCalculatedHeight - totalDesiredClientCellHeight;
                    firstCell.style.height = firstCellClientHeight + "px";
                    secondCell.style.height = secondCellClientHeight + totalCorrection + "px";
                } else if (secondCellCalculatedHeight > totalDesiredClientCellHeight) {
                    //totalCorrection -= secondCellCalculatedHeight - totalDesiredClientCellHeight;
                    secondCell.style.height = secondCellClientHeight + "px";
                    firstCell.style.height = firstCellClientHeight + totalCorrection + "px";
                } else {
                    firstCell.style.height = totalDesiredClientCellHeight + "px";
                    secondCell.style.height = totalDesiredClientCellHeight + "px";
                }
            }
        }
    },
    UpdateCellContainerSizeCache: function (containerIndex, cellIndex) {
        var container = this.schedulerViewInfo.cellContainers[containerIndex];
        this.containerSizeCache[containerIndex][cellIndex] = container.cellConstraint[cellIndex].minHeight;
    }
})
////////////////////////////////////////////////////////////////////////////////

var HorizontalAppointmentLayoutCalculatorInfinityHeight = ASPx.CreateClass(HorizontalAppointmentLayoutCalculator, {
    constructor: function(schedulerViewInfo, disableSnapToCells) {
        this.constructor.prototype.constructor.call(this, schedulerViewInfo, disableSnapToCells);
        this.maxBottom = 0;
    },
    CalculateIsFitted: function(containerIndex, cellIndex, bottom) {
        this.maxBottom = Math.max(bottom, this.maxBottom);
        return true;
    }
});

//todo: test it
var AppointmentCellIndexes = ASPx.CreateClass(null, {
    constructor: function(firstCellIndex, lastCellIndex) {
        this.firstCellIndex = firstCellIndex;
        this.lastCellIndex = lastCellIndex;
    }
 });
 
var HorizontalAppointmentRelativePositionCalculator = ASPx.CreateClass(null, {
    constructor: function(gapBetweenAppointments) {
        this.gapBetweenAppointments = gapBetweenAppointments;
    },
    //todo: test it
    CreateAppointmentCellIndexesCollection : function(viewInfos) {
        var result = [];
        var count = viewInfos.length;
        for (var i = 0; i < count; i++) {
            var viewInfo = viewInfos[i];
            var cellIndexes = new AppointmentCellIndexes(viewInfo.visibleFirstCellIndex, viewInfo.visibleLastCellIndex);
            result.push(cellIndexes);
        }
        return result;
    },
    //todo: test it
    RestoreCellIndexes : function(viewInfos, appointmentsCellIndexes) {
        var count = viewInfos.length;
        if (count == appointmentsCellIndexes.length) 
            for (var i = 0; i < count; i++) {
                var indexes = appointmentsCellIndexes[i];
                var viewInfo = viewInfos[i];
                viewInfo.visibleFirstCellIndex = indexes.firstCellIndex;
                viewInfo.visibleLastCellIndex = indexes.lastCellIndex;
            }
    },
    //todo: test it
    //todo: rewrite test (cellsCount not used)
    CalculateAppointmentRelativePositions: function(viewInfos, startIndex) {	 	        
        var previousCellIndexes = this.CreateAppointmentCellIndexesCollection(viewInfos);        
        this.AdjustAppointmentCellIndexes(viewInfos);
        this.CalculateAppointmentRelativePositionsCore(viewInfos, startIndex);
        this.RestoreCellIndexes(viewInfos, previousCellIndexes);
    },	
    //todo: test it
    AdjustAppointmentCellIndexes : function(viewInfos) {
        var dateTimes = this.CreateViewInfosDateTimeCollection(viewInfos);
        this.CalculateAdjustedCellIndexes(viewInfos, dateTimes);
    },
    //todo: test it
    CalculateAdjustedCellIndexes : function (viewInfos, dateTimes) {
        var count = viewInfos.length;
        for (var i = 0; i < count; i++)
            this.CalculateAdjustedCellIndexesCore(viewInfos[i], dateTimes);            
    },
    //todo: test it
    CalculateAdjustedCellIndexesCore : function (viewInfo, dateTimes) {                
        var firstCellIndex = ASPx.Data.ArrayBinarySearch(dateTimes, viewInfo.getStartTime(), ASPx.SchedulerDateTimeIndexComparer);
        var lastCellIndex = ASPx.Data.ArrayBinarySearch(dateTimes, viewInfo.getEndTime(), ASPx.SchedulerDateTimeIndexComparer) - 1;
            viewInfo.visibleFirstCellIndex = firstCellIndex;
            viewInfo.visibleLastCellIndex = lastCellIndex;        
    },     
    //todo: test it
    CreateViewInfosDateTimeCollection : function(viewInfos) {
        var count = viewInfos.length;
        var dateTimeCollection = [];
        for (var i = 0; i < count; i++) {
            var viewInfo = viewInfos[i];
            this.AddDateTime(dateTimeCollection, viewInfo.getStartTime());            
            this.AddDateTime(dateTimeCollection, viewInfo.getEndTime());              
        }
        dateTimeCollection.sort(ASPx.SchedulerDateTimeComparer);
        return dateTimeCollection;
    },	
    //todo: test it
    AddDateTime : function(dateTimeCollection, dateTime) {
        if (!this.IsAlreadyAdded(dateTimeCollection, dateTime))
            dateTimeCollection.push(dateTime);
    },	
    //todo: test it
    IsAlreadyAdded : function(dateTimeCollection, dateTime) {
          var count = dateTimeCollection.length;
          for (var i = 0; i < count; i++) 
                if (dateTimeCollection[i].valueOf() == dateTime.valueOf())
                    return true;
          return false;
    },	
    //todo: rewrite test (cellsCount not used)
    CalculateAppointmentRelativePositionsCore: function(viewInfos, startIndex) {	 	
        var count = viewInfos.length;
        var busyIntervals = this.CreateBusyIntervals(2*count);		
        var i = 0;
        while (i < startIndex) {
            this.MakeIntervalBusy(viewInfos[i], busyIntervals);
            i++;
        }
        while (i < count) {
            var viewInfo = viewInfos[i];
            var relativePosition = this.FindAvailableRelativePosition(viewInfo, busyIntervals);
            viewInfo.relativePosition = relativePosition;
            this.MakeIntervalBusy(viewInfo, busyIntervals);
            i++;
        }
    },
    CreateBusyIntervals: function(cellsCount) {	
        var result = new Array(cellsCount);
        for (var i = 0; i < cellsCount; i++)
            result[i] = [];
        return result;
    },
    FindAvailableRelativePosition: function(viewInfo, cellsBusyIntervals){
        viewInfo.relativePosition = 0;
        var relativePosition = 0;		
        var from = viewInfo.visibleFirstCellIndex;
        var to = viewInfo.visibleLastCellIndex;
        var i = from;
        while (i <= to) {
            var busyIntervals = cellsBusyIntervals[i];
            var interval = this.FindPossibleIntersectionInterval(busyIntervals, relativePosition);
            if ((interval == null) || (interval.start >= relativePosition + viewInfo.height))
                i++;
            else {
                relativePosition = interval.end;
                i = from;
            }
        }
        return relativePosition;
    },
    FindPossibleIntersectionInterval: function(busyIntervals, value) {
        for (var i = 0; i < busyIntervals.length; i++) {
            var interval = busyIntervals[i];
            if ((interval.ContainsExcludeEndBound(value)) || (interval.start > value))
                return new BusyInterval(interval.start, interval.end);
        }
        return null;
    },
    MakeIntervalBusy: function(info, busyIntervals) {
        for (var i = info.visibleFirstCellIndex; i <= info.visibleLastCellIndex; i++)
            this.AddBusyInterval(busyIntervals[i], new BusyInterval(info.relativePosition, info.relativePosition + info.height + this.gapBetweenAppointments));
    },
    AddBusyInterval: function(busyIntervals, busyInterval) {
        var count = busyIntervals.length;
        var i = 0;
        while (i < count) {
            if (busyIntervals[i].start > busyInterval.start)
                break;
            i++;
        }
        ASPx.Data.ArrayInsert(busyIntervals, busyInterval, i);
    }	
});
////////////////////////////////////////////////////////////////////////////////


////////////////////////////////////////////////////////////////////////////////
///////// TimelineAppointmentLayoutCalculator  ///////////
////////////////////////////////////////////////////////////////////////////////
var TimelineAppointmentLayoutCalculator = ASPx.CreateClass(HorizontalAppointmentLayoutCalculator, {
    constructor: function(schedulerViewInfo, disableSnapToCells) {
        this.constructor.prototype.constructor.call(this, schedulerViewInfo, disableSnapToCells);
        this.leftAppointmentIndent = 0;
        this.rightAppointmentIndent = 0;
    }
    
});

////////////////////////////////////////////////////////////////////////////////
///////////////////////////// ASPxClientRecurrenceInfo /////////////////////////
////////////////////////////////////////////////////////////////////////////////
var ASPxClientRecurrenceInfo = ASPx.CreateClass(null, {
    constructor: function(){
        this.interval = new ASPxClientTimeInterval(new Date(), 0);
        this.type = ASPxClientRecurrenceInfo.DefaultRecurrenceType;
        this.range = ASPxClientRecurrenceInfo.DefaultRecurrenceRange;
        this.weekDays = ASPxClientRecurrenceInfo.DefaultWeekDays;
        this.occurrenceCount = 1;
        this.periodicity = ASPxClientRecurrenceInfo.DefaultPeriodicity;
        this.dayNumber = 1; //???
        this.weekOfMonth = ASPxClientRecurrenceInfo.DefaultWeekOfMonth;
        this.month = 1; //???
        //this.id //???
    },
    CreateDefaultInterval: function() {
        return new ASPxClientTimeInterval(new Date(), 0);
    },
    SetStart: function(start) {
        if (!this.interval)
            this.interval = this.CreateDefaultInterval();
        this.interval.SetStart(start);
    },
    GetStart: function() {
        if (!this.interval)
            this.interval = this.CreateDefaultInterval();
        return this.interval.GetStart();
    },
    SetEnd: function(end) {
        if (!this.interval)
            this.interval = this.CreateDefaultInterval();
        this.interval.SetEnd(end);
    },
    GetEnd: function() {
        if (!this.interval)
            this.interval = this.CreateDefaultInterval();
        return this.interval.GetEnd();
    },
    SetDuration: function(duration) {
        if (!this.interval)
            this.interval = this.CreateDefaultInterval();
        this.interval.SetDuration(duration);
    },
    GetDuration: function() {
        if (!this.interval)
            this.interval = this.CreateDefaultInterval();
        return this.interval.GetDuration();
    },
    SetRecurrenceType: function(type) {
        this.type = type;
    },
    GetRecurrenceType: function() {
        if (!this.type)
            this.type = ASPxClientRecurrenceInfo.DefaultRecurrenceType;
        return this.type;
    },
    SetWeekDays: function(weekDays) {
        this.weekDays = weekDays;
    },
    GetWeekDays: function() {
        if (!this.weekDays)
            this.weekDays = ASPxClientRecurrenceInfo.DefaultWeekDays;
        return this.weekDays;
    },
    SetOccurrenceCount: function(occurrenceCount) {
        this.occurrenceCount = occurrenceCount;
    },
    GetOccurrenceCount: function() {
        if (!this.occurrenceCount)
            this.occurrenceCount = 1;
        return this.occurrenceCount = 1;
    },
    SetPeriodicity: function(periodicity) {
        this.periodicity = periodicity;
    },
    GetPeriodicity: function() {
        if (!this.periodicity)
            this.periodicity = ASPxClientRecurrenceInfo.DefaultPeriodicity;
        return this.periodicity;
    },
    SetDayNumber: function(dayNumber) {
        this.dayNumber = dayNumber;
    },
    GetDayNumber: function() {
        if (!this.dayNumber)
            this.dayNumber = 1;
        return this.dayNumber;
    },
    SetWeekOfMonth: function(weekOfMonth ) {
        this.weekOfMonth = weekOfMonth;
    },
    GetWeekOfMonth: function() {
        if(!this.weekOfMonth)
            this.weekOfMonth = ASPxClientRecurrenceInfo.DefaultWeekOfMonth;
        return this.weekOfMonth;
    },
    SetMonth: function(month) {
        this.month = month;
    },
    GetMonth: function() {
        if (!this.month)
            this.month = 1;
        return this.month;
    },
    GetRange: function() {
        return this.range;
    },
    SetRange: function(range) {
        this.range = range;
    }
});
ASPxClientRecurrenceInfo.DefaultPeriodicity = 1;
ASPxClientRecurrenceInfo.DefaultRecurrenceRange = ASPxClientRecurrenceRange.NoEndDate;
ASPxClientRecurrenceInfo.DefaultRecurrenceType = ASPxClientRecurrenceType.Daily;      
ASPxClientRecurrenceInfo.DefaultWeekDays = 127;
ASPxClientRecurrenceInfo.DefaultWeekOfMonth = ASPxClientWeekOfMonth.First;

ASPx.HorizontalAppointmentViewInfo = HorizontalAppointmentViewInfo;
ASPx.VerticalAppointmentViewInfo = VerticalAppointmentViewInfo;
ASPx.VerticalAppointmentOperationPresenter = VerticalAppointmentOperationPresenter;
ASPx.HorizontalAppointmentOperationPresenter = HorizontalAppointmentOperationPresenter;
ASPx.AppointmentDragHelper = AppointmentDragHelper;
ASPx.VerticalAppointmentLayoutCalculator = VerticalAppointmentLayoutCalculator;
ASPx.BusyInterval = BusyInterval;
ASPx.HorizontalAppointmentLayoutCalculator = HorizontalAppointmentLayoutCalculator;
ASPx.CellsAutoHeightClientHorizontalAppointmentLayoutCalculator = CellsAutoHeightClientHorizontalAppointmentLayoutCalculator;
ASPx.HorizontalAppointmentLayoutCalculatorInfinityHeight = HorizontalAppointmentLayoutCalculatorInfinityHeight;
ASPx.HorizontalAppointmentRelativePositionCalculator = HorizontalAppointmentRelativePositionCalculator;

ASPx.AppointmentResizeSideType = AppointmentResizeSideType;

window.ASPxClientAppointmentFlags = ASPxClientAppointmentFlags;
window.ASPxClientAppointment = ASPxClientAppointment;
window.ASPxClientPoint = ASPxClientPoint;
window.ASPxClientRect = ASPxClientRect;
window.ASPxClientRecurrenceInfo = ASPxClientRecurrenceInfo;
})();