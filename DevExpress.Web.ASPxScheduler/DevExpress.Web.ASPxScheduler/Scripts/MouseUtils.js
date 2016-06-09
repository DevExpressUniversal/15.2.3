(function() {
mouseEventDataObject = null;

// event
var SchedulerMouseEvents = ASPx.CreateClass(null, {
    constructor: function(){
        this.mouseOverHandlerList = [];
        this.mouseMoveHandlerList = [];
        this.mouseOutHandlerList = [];
    },
    CreateHandler: function(handlerObject, param) {
        var result = {};
        result.handlerObject = handlerObject;
        result.param = param;
        return result;
    },
    AddMouseOverHandler: function (handlerObject, param) {
        var handler = this.CreateHandler(handlerObject, param);
        this.mouseOverHandlerList.push(handler);
    },
    AddMouseMoveHandler: function (handlerObject, param) {
        var handler = this.CreateHandler(handlerObject, param);
        this.mouseMoveHandlerList.push(handler);
    },
    AddMouseOutHandler: function (handlerObject, param) {
        var handler = this.CreateHandler(handlerObject, param);
        this.mouseOutHandlerList.push(handler);
    },	
    FireMouseOverEvent: function (evt, dataObject) {
        function fireMouseOver(handlerObject, evt, dataObject, param) {
            return handlerObject.OnMouseOver(evt, dataObject, param);
        }
        this.FireMouseEvent(this.mouseOverHandlerList, fireMouseOver, evt, dataObject );

    },
    FireMouseOutEvent: function (evt, dataObject) {
        function fireMouseOut(handlerObject, evt, dataObject, param) {
            return handlerObject.OnMouseOut(evt, dataObject, param);
        }
        this.FireMouseEvent(this.mouseOutHandlerList, fireMouseOut, evt, dataObject );
    },
    FireMouseMoveEvent: function (evt, dataObject) {
        function fireMouseMove(handlerObject, evt, dataObject, param) {
            return handlerObject.OnMouseMove(evt, dataObject, param);
        }
        this.FireMouseEvent(this.mouseMoveHandlerList, fireMouseMove, evt, dataObject );
    },
    FireMouseEvent: function(handlerList, fireEventFunc, evt, dataObject) {
        for(var i = 0; i < handlerList.length; i++) {
            var handler = handlerList[i];
            fireEventFunc(handler.handlerObject, evt, dataObject, handler.param);
        }
    }
});

function _aspxSchedulerTestIsSupportMouseEvents(element) {
    return ASPx.IsExists(element.viewInfo) && ASPx.IsExists(element.viewInfo.mouseEvents);
}
function _aspxSchedulerTestIsSupportViewInfo(element) {
    return ASPx.IsExists(element.viewInfo);
}
function _aspxClearCurrentMouseEventDataObject() {
    if(ASPx.IsExists(mouseEventDataObject))
        mouseEventDataObject.mouseEvents.FireMouseOutEvent(null, mouseEventDataObject);        
    mouseEventDataObject = null;
}

var SchedulerToolTipHelper = ASPx.CreateClass(null, {
    constructor: function () {
        //this.timeBeforeShow = ASPx.Browser.TouchUI ? 0 : 500;
        this.timeBeforeShow = 500;
        this.timerId = -1;
        this.lastX = null;
        this.lastY = null;
        this.viewInfo = null;
        this.toolTip = null;
        this.activeToolTip = null;
        this.toolTipOffsetX = -15; //5;
        this.toolTipOffsetY = -20; //5;
        this.canCloseByMouseOut = true;
    },
    OnMouseOver: function (evt, dataObject, toolTip) {
        if (this.activeToolTip != null)
            return;
        this.RestartTimerAndInitToolTip(evt, dataObject, toolTip);
        this.lastX = ASPx.Evt.GetEventX(evt);
        this.lastY = ASPx.Evt.GetEventY(evt);
    },
    OnMouseMove: function (evt, dataObject, toolTip) {
        if (this.CanResetTimer())
            this.RestartTimerAndInitToolTip(evt, dataObject, toolTip);
        this.lastX = ASPx.Evt.GetEventX(evt);
        this.lastY = ASPx.Evt.GetEventY(evt);
    },
    CanResetTimer: function () {
        if (this.activeToolTip == null)
            return true;
        return ASPx.IsExists(this.toolTip) && this.toolTip.ShouldResetPositionByTimer();
    },
    OnMouseOut: function (evt, dataObject, toolTip) {
        ASPx.Timer.ClearTimer(this.timerId);
        if (ASPx.IsExists(this.toolTip) && this.canCloseByMouseOut)
            this.HideToolTip();
    },
    Reset: function () {
        ASPx.Timer.ClearTimer(this.timerId);
    },
    RestartTimerAndInitToolTip: function (evt, viewInfo, toolTip) {
        ASPx.Timer.ClearTimer(this.timerId);
        var toolTipSource = ASPx.FindToolTipInParentElements(ASPx.Evt.GetEventSource(evt));
        if (ASPx.IsExists(toolTipSource)) {
            return;
        }
        this.toolTip = ASPx.GetControlCollection().Get(toolTip);
        this.viewInfo = viewInfo;
        //this.timerId = 
        this.timerId = window.setTimeout(this.OnToolTipTimer.aspxBind(this), this.timeBeforeShow);
        this.canCloseByMouseOut = !this.toolTip.CanCloseByMouseClick();
    },
    OnToolTipTimer: function() {
        this.ShowToolTip();
    },
    ShowToolTipInstantly: function (x, y, toolTipInstance, viewInfo) {
        ASPx.Timer.ClearTimer(this.timerId);
        this.lastX = x;
        this.lastY = y;
        this.toolTip = toolTipInstance;
        this.viewInfo = null;
        if (!this.CanShowToolTip())
            return;
        if (!ASPx.IsExists(viewInfo))
            viewInfo = null;
        this.canCloseByMouseOut = false;
        var scheduler = this.toolTip.GetSchedulerControl();
        if (scheduler)
            scheduler.BeginDeferredFuncCallbackArea();
        this.ShowToolTipCore(viewInfo);
        if (scheduler)
            scheduler.EndDeferredFuncCallbackArea();
    },
    ShowToolTip: function () {
        if (!this.CanShowToolTip())
            return;
        if (!ASPx.IsExists(this.viewInfo))
            return;
        var toolTip = this.toolTip;
        toolTip.viewInfo = this.viewInfo;
        var scheduler = toolTip.GetSchedulerControl();
        if (scheduler)
            scheduler.BeginDeferredFuncCallbackArea();

        if (this.ShowToolTipCore(toolTip.viewInfo)) {
            if (this.enterLeaveHelper)
                this.enterLeaveHelper.Detach();
            this.enterLeaveHelper = new ASPx.MouseEnterLeaveHelper(this.activeToolTip.mainDiv);
            this.enterLeaveHelper.Attach(this.OnMouseOverHandler.aspxBind(this), this.OnMouseOutHandler.aspxBind(this));
        }
        if (scheduler)
            scheduler.EndDeferredFuncCallbackArea();
    },
    ShowToolTipInstantly: function (evt, container) {
        var eventSource = ASPx.Evt.GetEventSource(evt);
        eventSource = ASPx.GetParent(eventSource, ASPx.SchedulerTestIsSupportViewInfo);
        if (!ASPx.IsExists(eventSource)) {
            if (!container)
                return;
            eventSource = container;
        }
        this.Reset();
        var viewInfo = eventSource.viewInfo;
        if (!viewInfo)
            return;
        this.lastX = ASPx.Evt.GetEventX(evt);
        this.lastY = ASPx.Evt.GetEventY(evt);        
        this.toolTip = ASPx.GetControlCollection().Get(viewInfo.toolTip);
        this.viewInfo = viewInfo;
        this.canCloseByMouseOut = !this.toolTip.CanCloseByMouseClick();
        this.ShowToolTip();
    },
    OnMouseOverHandler: function (element) {
    },
    OnMouseOutHandler: function (element) {
        ASPx.Timer.ClearTimer(this.timerId);
        if (ASPx.IsExists(this.toolTip) && this.canCloseByMouseOut)
            this.HideToolTip();
        if (this.enterLeaveHelper) {
            this.enterLeaveHelper.Detach();
            this.enterLeaveHelper = null;
        }
    },
    ShowToolTipCore: function (viewInfo) {
        this.HideActiveToolTip();
        var toolTipData = this.CreateToolTipData(viewInfo);
        if (!toolTipData) //appointment deleted before been shown
            return false;
        var toolTip = this.toolTip;
        toolTip.SetContent(toolTipData);
        toolTip.ShowToolTip(this.lastX, this.lastY);
        this.activeToolTip = toolTip;
        toolTip.FinalizeUpdate(toolTipData);
        return true;
    },
    CreateToolTipData: function (dataObject) {
        var scheduler = this.toolTip.aspxParentControl;
        if (ASPx.IsExists(dataObject) && ASPx.IsExists(dataObject.appointmentId)) {
            var apt = scheduler.GetAppointment(dataObject.appointmentId);
            if (!apt) //appointment deleted before been shown
                return null;
            return new ASPxClientSchedulerToolTipData(apt, apt.interval, apt.GetResources());
        } else
            return new ASPxClientSchedulerToolTipData(null, scheduler.selection.interval, [scheduler.selection.resource]);
    },
    CanShowToolTip: function () {
        if (!ASPx.IsExists(this.toolTip))
            return false;
        var scheduler = this.toolTip.aspxParentControl;
        if (scheduler.containerCell.parentNode == null)
            return false;
        if (ASPx.IsExists(scheduler.currentPopupContainer))
            return false;
        if (!scheduler.toolTipsEnable)
            return false;
        var isSelectionOperationActive = scheduler.IsOperationSelectionActive();
        var toolTipData = (this.viewInfo) ? this.CreateToolTipData(this.viewInfo) : null;
        return this.toolTip.CanShowToolTip(toolTipData) && !this.IsAnyMenuVisible() && !isSelectionOperationActive;
    },
    IsAnyMenuVisible: function () {
        if (typeof ASPxClientMenuBase == 'undefined')
            return false;
        var menuCollection = ASPxClientMenuBase.GetMenuCollection();
        return menuCollection.IsAnyMenuVisible();
    },
    HideActiveToolTip: function () {
        this.HideToolTipCore();
    },
    HideToolTip: function () {
        this.HideToolTipCore();
        this.toolTip = null;
        this.viewInfo = null;
    },
    HideToolTipCore: function () {
        if (!ASPx.IsExists(this.activeToolTip))
            return;
        this.activeToolTip.HideToolTip();
        this.activeToolTip = null;
    }
});

ASPx.SchedulerLeftResizeDivMouseDown = function(name, element, evt) {
    evt = ASPx.Evt.GetEvent(evt);
    var scheduler = ASPx.GetControlCollection().Get(name);
    if(!ASPx.IsExists(scheduler))   
        return;
    var appointmentDiv = element.parentNode;
    var appointmentViewInfo = appointmentDiv.appointmentViewInfo;
    if(ASPx.IsExists(appointmentViewInfo))
        scheduler.BeginAppointmentResizeAtLeft(appointmentViewInfo, aspxSchedulerGetMousePosition(evt));
}
ASPx.SchedulerRightResizeDivMouseDown = function(name, element, evt) {
    evt = ASPx.Evt.GetEvent(evt);
    var scheduler = ASPx.GetControlCollection().Get(name);
    if(!ASPx.IsExists(scheduler))   
        return;
    var appointmentDiv = element.parentNode;
    var appointmentViewInfo = appointmentDiv.appointmentViewInfo;
    if (ASPx.IsExists(appointmentViewInfo))        
        scheduler.BeginAppointmentResizeAtRight(appointmentViewInfo, aspxSchedulerGetMousePosition(evt));
}
ASPx.SchedulerTopResizeDivMouseDown = function(name, element, evt) {
    evt = ASPx.Evt.GetEvent(evt);
    var scheduler = ASPx.GetControlCollection().Get(name);
    if(!ASPx.IsExists(scheduler))   
        return;    
    var appointmentDiv = element.parentNode;
    var appointmentViewInfo = appointmentDiv.appointmentViewInfo;
    
    if(ASPx.IsExists(appointmentViewInfo))
        scheduler.BeginAppointmentResizeAtTop(appointmentViewInfo, aspxSchedulerGetMousePosition(evt));
}
ASPx.SchedulerBottomResizeDivMouseDown = function(name, element, evt) {
    evt = ASPx.Evt.GetEvent(evt);
    var scheduler = ASPx.GetControlCollection().Get(name);
    if(!ASPx.IsExists(scheduler))   
        return;    
    var appointmentDiv = element.parentNode;
    var appointmentViewInfo = appointmentDiv.appointmentViewInfo;
    if(ASPx.IsExists(appointmentViewInfo))
        scheduler.BeginAppointmentResizeAtBottom(appointmentViewInfo, aspxSchedulerGetMousePosition(evt));
}
function aspxSchedulerGetMousePosition(e) {
    return {
        x: ASPx.Evt.GetEventX(e),
        y: ASPx.Evt.GetEventY(e)
    };
}

ASPx.AppointmentResizeLeft = function(scheduler, appointmentDiv, cell) {
    var cellTime = scheduler.GetCellStartTime(cell);
    var appointment = scheduler.GetAppointment(appointmentDiv.appointmentViewInfo.appointmentId);;
    var newStart = ASPx.SchedulerGlobals.DateTimeMinValue(cellTime, appointment.interval.GetEnd());
    var newEnd = appointment.interval.GetEnd();
    var newInterval = new ASPxClientTimeInterval(newStart, ASPx.SchedulerGlobals.DateSubsWithTimezone(newEnd, newStart));
    if(!newInterval.Equals(appointment.operationInterval)) {
        appointment.operationInterval = newInterval;
        return true;
    }
    else
        return false;
}
ASPx.AppointmentResizeTop = function(scheduler, appointmentDiv, cell) {
    var cellTime = scheduler.GetCellStartTime(cell);
    var appointment = scheduler.GetAppointment(appointmentDiv.appointmentViewInfo.appointmentId);;
    var newStart = ASPx.SchedulerGlobals.DateTimeMinValue(cellTime, appointment.interval.GetEnd());
    var newEnd = appointment.interval.GetEnd();
    var newInterval = new ASPxClientTimeInterval(newStart, ASPx.SchedulerGlobals.DateSubsWithTimezone(newEnd, newStart));
    if(!newInterval.Equals(appointment.operationInterval)) {
        appointment.operationInterval = newInterval;
        return true;
    }
    else
        return false;
}
ASPx.AppointmentResizeRight = function(scheduler, appointmentDiv, cell) {    
    var cellTime = scheduler.GetCellEndTime(cell);
    var appointment = scheduler.GetAppointment(appointmentDiv.appointmentViewInfo.appointmentId);;
    var newStart = appointment.interval.GetStart();
    var newEnd = ASPx.SchedulerGlobals.DateTimeMaxValue(cellTime, appointment.interval.GetStart());
    var newInterval = new ASPxClientTimeInterval(newStart, ASPx.SchedulerGlobals.DateSubsWithTimezone(newEnd, newStart));
    if(!newInterval.Equals(appointment.operationInterval)) {
        appointment.operationInterval = newInterval;
        return true;
    }
    else
        return false;
}
ASPx.AppointmentResizeBottom = function(scheduler, appointmentDiv, cell) {    
    var cellTime = scheduler.GetCellEndTime(cell);        
    var appointment = scheduler.GetAppointment(appointmentDiv.appointmentViewInfo.appointmentId);;
    var newStart = appointment.interval.GetStart();
    var newEnd = ASPx.SchedulerGlobals.DateTimeMaxValue(cellTime, appointment.interval.GetStart());
    var newInterval = new ASPxClientTimeInterval(newStart, ASPx.SchedulerGlobals.DateSubsWithTimezone(newEnd, newStart));
    if(!newInterval.Equals(appointment.operationInterval)) {
        appointment.operationInterval = newInterval;
        return true;
    }
    else
        return false;
}

var resizeHelper = null;

var ResizeViewHelper = ASPx.CreateClass(null, {
    constructor: function(scheduler, appointmentDiv) {
        this.scheduler = scheduler;
        this.horizontalResizePresenter = new ASPx.HorizontalAppointmentOperationPresenter(scheduler, appointmentDiv);
        this.verticalResizePresenter = new ASPx.VerticalAppointmentOperationPresenter(scheduler, appointmentDiv);
    },
    ShowResizedAppointmentPosition: function(e, resizedAppointment) {
        this.horizontalResizePresenter.ShowAppointment(resizedAppointment, false);        
        this.verticalResizePresenter.ShowAppointment(resizedAppointment, false);
        var toolTip = this.scheduler.GetAppointmentDragTooltip();
        if(ASPx.IsExists(toolTip) && toolTip.CanShowToolTip()) {       
            var toolTipData = new ASPxClientSchedulerToolTipData(resizedAppointment, resizedAppointment.operationInterval, resizedAppointment.operationResources) 
            toolTip.SetContent(toolTipData);
            var shouldResetToolTip = this.scheduler.activeToolTip != toolTip || toolTip.ShouldResetPositionByTimer();
            if (shouldResetToolTip) {
                this.scheduler.HideAllToolTips();
                toolTip.ShowToolTip(ASPx.Evt.GetEventX(e), ASPx.Evt.GetEventY(e) - 5);//B141819
            }
            toolTip.FinalizeUpdate(toolTipData);
        }

    },
    HideResizedAppointmentPosition: function(aptId, restoreAptPos) {
        this.horizontalResizePresenter.HideAppointment(aptId, restoreAptPos);
        this.verticalResizePresenter.HideAppointment(aptId, restoreAptPos);
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

var SchedulerResizeHelper = ASPx.CreateClass(null, {//TODO: inherit from ASPxClientDragHelper
    constructor: function (scheduler, appointmentDiv, handler, startResizePosition) {
        if(resizeHelper != null)
           resizeHelper.cancelResize(true);
        scheduler.DisableReminderTimer();
        this.lastCell = null;
        this.scheduler = scheduler;
        this.appointmentDiv = appointmentDiv;
        this.resizeHandler = handler;
        this.resizeViewHelper = new ResizeViewHelper(scheduler, appointmentDiv);
        this.startResizePosition = startResizePosition;
        resizeHelper = this;
    },
    resize: function (e) {
        var position = aspxSchedulerGetMousePosition(e);
        if (this.startResizePosition && Math.abs(this.startResizePosition.x - position.x) < 3 && Math.abs(this.startResizePosition.y - position.y) < 3)
            return;
        this.startResizePosition = null;

        if(ASPx.Browser.IE)//FIX IE Bug with ESC button
            this.scheduler.innerContentElement.setActive();
        var hitTestResult = this.scheduler.CalcHitTest(e);
        var cell = hitTestResult.cell;
        if(ASPx.IsExists(cell) && cell != this.lastCell) {
            if(this.resizeHandler(this.scheduler, this.appointmentDiv, cell)) {
                var appointment = this.scheduler.GetAppointment(this.appointmentDiv.appointmentViewInfo.appointmentId);
                this.resizeViewHelper.ShowResizedAppointmentPosition(e, appointment);
            }
            this.lastCell = cell;
        }
    },
    endResize: function() {
        var wasCallback = this.ApplyChanges();
        if(!wasCallback)
            this.cancelResize(true);
        else {
            this.resizeViewHelper.HideToolTip();
            //this.cancelResize(true);
            resizeHelper = null;
            this.scheduler.onCallback = ASPx.CreateDelegate(this.OnCallback, this);
            this.scheduler.onCallbackError = ASPx.CreateDelegate(this.OnCallbackError, this);
        }
    },
    OnCallback: function() {
        this.cancelResize(false);
    },
    OnCallbackError: function() {
        this.cancelResize(true);
    },
    ApplyChanges: function() {
        var id = this.appointmentDiv.appointmentViewInfo.appointmentId;
        var appointment = this.scheduler.GetAppointment(id);
        if(!ASPx.IsExists(appointment) || !ASPx.IsExists(appointment.operationInterval))
            return false;            
        var wasResized = (appointment.operationInterval.GetStart() - appointment.interval.GetStart() != 0) || (appointment.operationInterval.GetEnd() - appointment.interval.GetEnd() != 0);
        if(wasResized) {
            var params = "APTSCHANGE|" + id;
            params += "?START="+  ASPx.SchedulerGlobals.DateTimeToMilliseconds(appointment.operationInterval.GetStart());
            params += "?DURATION="+  appointment.operationInterval.GetDuration();
            
            var operation = new ASPxClientAppointmentOperation(this, params);
            var isHandled = this.scheduler.RaiseAppointmentResize(operation);
            if (isHandled)
                return true;
            this.Apply(params);
            return true;
        }
        else
            return false;
    },
    Apply: function(params) {
        this.scheduler.RaiseCallback(params);
    },
    Cancel: function() {
        this.cancelResize(true, true);
    },
    cancelResize: function(restoreAptPos, restoreTimer) {
        if(!ASPx.IsExists(restoreAptPos))
            restoreAptPos = true;
        this.resizeViewHelper.HideResizedAppointmentPosition(this.appointmentDiv.appointmentViewInfo.appointmentId, restoreAptPos);            
        this.scheduler.onCallback = null;
        this.scheduler.onCallbackError = null;
        resizeHelper = null;
        if(restoreTimer)
            this.scheduler.EnableReminderTimer();
        
    }
});

var MouseEnterLeaveHelper = new ASPx.CreateClass(null, {
    constructor: function (element) {
        this.element = element;
    },
    Attach: function (onMouseOverHandler, onMouseOutHandler) {
        this.mouseOverHandler = onMouseOverHandler;
        this.mouseOutHandler = onMouseOutHandler;
        this.mouseEnterHandlerWithScope = this.MouseEnterHandler.aspxBind(this);
        ASPx.Evt.AttachEventToElement(this.element, "mouseover", this.mouseEnterHandlerWithScope);
        ASPx.Evt.AttachEventToElement(this.element, "mouseout", this.mouseEnterHandlerWithScope);
        
    },
    Detach: function () {
        if (!this.mouseEnterHandlerWithScope || !this.element)
            return;
        ASPx.Evt.DetachEventFromElement(this.element, "mouseover", this.mouseEnterHandlerWithScope);
        ASPx.Evt.DetachEventFromElement(this.element, "mouseout", this.mouseEnterHandlerWithScope);
        this.mouseEnterHandlerWithScope = null;
        this.mouseOverHandler = null;
        this.mouseOutHandler = null;
        this.element = null;
    },
    MouseEnterHandler: function (evt) {
        var isMouseOverExecuted = !!this.element.dxMouseOverExecuted;
        var isMouseOverEvent = (evt.type == "mouseover");
        if (isMouseOverEvent && isMouseOverExecuted || !isMouseOverEvent && !isMouseOverExecuted)
            return;
        var source = ASPx.Evt.GetEventRelatedTarget(evt, isMouseOverEvent);
        if (!ASPx.GetIsParent(this.element, source)) {
            this.element.dxMouseOverExecuted = isMouseOverEvent;
            if (isMouseOverEvent)
                this.mouseOverHandler(this.element);
            else
                this.mouseOutHandler(this.element);
        }
        else if (isMouseOverEvent && !isMouseOverExecuted) {
            this.element.dxMouseOverExecuted = true;
            this.mouseOverHandler(this.element);
        }
    }
});

ASPx.MouseEnterLeaveHelper = MouseEnterLeaveHelper;
ASPx.SchedulerMouseEvents = SchedulerMouseEvents;
ASPx.ClearCurrentMouseEventDataObject = _aspxClearCurrentMouseEventDataObject;
ASPx.GetMousePosition = aspxSchedulerGetMousePosition;
ASPx.SchedulerResizeHelper = SchedulerResizeHelper;
ASPx.SchedulerToolTipHelper = SchedulerToolTipHelper;
ASPx.SchedulerTestIsSupportMouseEvents = _aspxSchedulerTestIsSupportMouseEvents;
ASPx.SchedulerTestIsSupportViewInfo = _aspxSchedulerTestIsSupportViewInfo;
})();