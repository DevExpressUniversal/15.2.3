(function() {
var SchedulerFormType = { };
SchedulerFormType.None = "None",
SchedulerFormType.Appointment = "Appointment",
SchedulerFormType.AppointmentInplace = "AppointmentInplace", 
SchedulerFormType.GotoDate = "GotoDate";

var SchedulerFormVisibility = { };
SchedulerFormVisibility.None = "None",
SchedulerFormVisibility.PopupWindow = "PopupWindow",
SchedulerFormVisibility.FillControlArea = "FillControlArea";
var ASPxClientRecurrenceRange = { };
ASPxClientRecurrenceRange.NoEndDate = "NoEndDate";
ASPxClientRecurrenceRange.OccurrenceCount = "OccurrenceCount";
ASPxClientRecurrenceRange.EndByDate = "EndByDate";
var ASPxClientRecurrenceType = { };
ASPxClientRecurrenceType.Daily = "Daily";
ASPxClientRecurrenceType.Weekly = "Weekly";
ASPxClientRecurrenceType.Monthly = "Monthly";
ASPxClientRecurrenceType.Yearly = "Yearly";
ASPxClientRecurrenceType.Hourly = "Hourly";
var ASPxClientWeekDays = { };
ASPxClientWeekDays.Sunday = 1;
ASPxClientWeekDays.Monday = 2;
ASPxClientWeekDays.Tuesday = 4;
ASPxClientWeekDays.Wednesday = 8;
ASPxClientWeekDays.Thursday = 16;
ASPxClientWeekDays.Friday = 32;
ASPxClientWeekDays.Saturday = 64;
ASPxClientWeekDays.WeekendDays = ASPxClientWeekDays.Sunday | ASPxClientWeekDays.Saturday;
ASPxClientWeekDays.WorkDays = ASPxClientWeekDays.Monday | ASPxClientWeekDays.Tuesday | ASPxClientWeekDays.Wednesday | ASPxClientWeekDays.Thursday | ASPxClientWeekDays.Friday;
ASPxClientWeekDays.EveryDay = ASPxClientWeekDays.WeekendDays | ASPxClientWeekDays.WorkDays;
var ASPxClientWeekOfMonth = { };
ASPxClientWeekOfMonth.None = 0;
ASPxClientWeekOfMonth.First = 1;
ASPxClientWeekOfMonth.Second = 2;
ASPxClientWeekOfMonth.Third = 3;
ASPxClientWeekOfMonth.Fourth = 4;
ASPxClientWeekOfMonth.Last = 5;

ASPxClientTimeIndicatorVisibility = {};
ASPxClientTimeIndicatorVisibility.Never = 0;
ASPxClientTimeIndicatorVisibility.Always = 1;
ASPxClientTimeIndicatorVisibility.TodayView = 2;
ASPxClientTimeIndicatorVisibility.CurrentDate = 3;
var ASPxSchedulerDateTimeHelper = {};
ASPxSchedulerDateTimeHelper.DaySpan = 24 * 60 * 60 * 1000; //duration in ms
ASPxSchedulerDateTimeHelper.TruncToDate = function (dateTime) {
    return new Date(dateTime.getFullYear(), dateTime.getMonth(), dateTime.getDate());    
}
ASPxSchedulerDateTimeHelper.ToDayTime = function (dateTime) {
    return dateTime.valueOf() - ASPxSchedulerDateTimeHelper.TruncToDate(dateTime).valueOf();
}
ASPxSchedulerDateTimeHelper.AddDays = function (date, dayCount) {
    var dayCountDelta = dayCount * ASPxSchedulerDateTimeHelper.DaySpan;//24 * 60 * 60 * 1000
    return ASPx.SchedulerGlobals.DateIncreaseWithUtcOffset(date, dayCountDelta);
}
ASPxSchedulerDateTimeHelper.AddTimeSpan = function (date, timeSpan) {
    return new Date(date.valueOf() + timeSpan);
}
ASPxSchedulerDateTimeHelper.CeilDateTime = function (dateTime, spanInMs) {
    return new Date(ASPxSchedulerDateTimeHelper.Ceil(dateTime.valueOf(), spanInMs));
}
ASPxSchedulerDateTimeHelper.Ceil = function (dateInMs, spanInMs) {
    var remainder = dateInMs % spanInMs;
    if (remainder != 0) {
        var delta = spanInMs - remainder;
        dateInMs += delta;
    }
    return dateInMs;
}
var SchedulerUtils = { };
SchedulerUtils.IsAppointmentResourcesEmpty = function(appointmentResources) {
    if(appointmentResources.length == 0)
        return true;
    if(appointmentResources.length == 1 && appointmentResources[0] == "null")    
        return true;
    else
        return false;
}


var SchedulerRelatedControlBase = ASPx.CreateClass(ASPxClientControl, {
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);
        this.schedulerControlId = "";
        this.standalone = true;
    },
    GetMainElement: function () {
        if (!ASPx.IsExistsElement(this.mainElement))
            this.mainElement = ASPx.GetElementById(this.name + "_mainCell");
        return this.mainElement;
    },
    CreateLoadingDiv: function (parentNode, offsetNode) {
        if (this.standalone)
            return this.constructor.prototype.CreateLoadingDiv.call(this, parentNode, offsetNode);
        return null;
    },
    GetLoadingDiv: function () {
        return ASPx.GetElementById(this.schedulerControlId + "_LD");
    }
});

ASPx.SchedulerSetRecurrenceControlsVisibility = function(controlsIds, recurrenceControlContainerId, visibleIndex) {
    var count = controlsIds.length;
    for(var i = 0; i < count; i++) {
        var ctl = ASPx.GetElementById(controlsIds[i]);
        if(ASPx.IsExistsElement(ctl)) {
            //ASPx.SetElementVisibility(ctl, visibleIndex == i);
            //ctl.style.position = "absolute"            
            ASPx.SetElementDisplay(ctl, visibleIndex == i);
        }
    }
    var container = ASPx.GetElementById(recurrenceControlContainerId);
    
    if (ASPx.IsExists(container.rbcControlsSizeCorrected)) {
        if (container.rbcControlsSizeCorrected[visibleIndex])
            return;
    } else {
        container.rbcControlsSizeCorrected = [ ];
    }
    container.rbcControlsSizeCorrected[visibleIndex] = true;
        
    if (ASPx.IsExistsElement(container))
        ASPx.GetControlCollection().AdjustControls(container);
}
ASPx.SchedulerChangeElementVisibility = function(id){
    var ctl = ASPx.GetElementById(id);
    if(ASPx.IsExistsElement(ctl))
        ASPx.SetElementDisplay(ctl, !ASPx.GetElementDisplay(ctl));        
}

////////////////////////////////////////////////////////////////////////////////
// Measurer
////////////////////////////////////////////////////////////////////////////////

var SchedulerMeasurer = {
    SetCustomDateHeaderContent: function(headers, dates, headerCaptions, headerToolTips) {
        var count = headers.length;
        if (count <= 0)
            return;
            
        this.EnsureFormatter();
        this.formatter.SetFormatString("d MMMM yyyy");

        var toolTipHelper = new ASPxSchedulerSimpleToolTipHelper(headerToolTips);
        for (var i = 0; i < count; i++) {
            var header = headers[i];
            var toolTip = toolTipHelper.GetToolTip(i, this.formatter.Format(dates[i]));
            header.setAttribute("title", toolTip);
            header.innerHTML = headerCaptions[i];
        }
    },
    SetCustomDayOfWeekHeaderContent: function(headers, days, headerCaptions, headerToolTips) {
        var count = headers.length;
        if (count <= 0)
            return;
            
        this.EnsureFormatter();
        this.formatter.SetFormatString("d MMMM yyyy");

        var toolTipHelper = new ASPxSchedulerSimpleToolTipHelper(headerToolTips);
        for (var i = 0; i < count; i++) {
            var header = headers[i];
            var toolTip = toolTipHelper.GetToolTip(i, this.GetDayName(days[i]));
            header.setAttribute("title", toolTip);
            header.innerHTML = headerCaptions[i];
        }
    },
    SetOptimalDayNumberHeaderContent: function(headers, dates, formats, headerToolTips) {
        var count = headers.length;
        if (count <= 0)
            return;

        this.EnsureFormatter();
        
        var toolTipHelper = new ASPxSchedulerSimpleToolTipHelper(headerToolTips);
        this.formatter.SetFormatString("d MMMM yyyy");
        for(var i=0; i < count; i++) {
            var header = headers[i];
            var toolTip = toolTipHelper.GetToolTip(i, this.formatter.Format(dates[i]));
            header.setAttribute("title", toolTip);
            header.innerHTML = dates[i].getDate();
        }
    },
    SetOptimalDayOfWeekHeadersContent: function(headers, days, headerToolTips) {
        if (headers.length <= 0)
            return;

        if (count <= 0)
            return;

        var toolTipHelper = new ASPxSchedulerSimpleToolTipHelper(headerToolTips);
        this.EnsureFormatter();
        
        var measureDiv = this.CreateMeasureDiv(headers[0]);

        var headerTexts = [];
        var headerContentWidths = [];
        
        var count = headers.length;
        for (var i = 0; i <count; i++) {
            var day = days[i];
            var toolTip = toolTipHelper.GetToolTip(i, this.GetDayName(day));
            this.SetOptimalDayOfWeekHeader(headers[i], day, measureDiv, toolTip);
        }

        ASPx.SchedulerGlobals.RemoveChildFromParent(document.body, measureDiv);                               
    },
    SetOptimalDayOfWeekHeader: function(header, day, measureDiv, toolTip) {
        var fullDayName = this.GetDayName(day);
        measureDiv.innerHTML = fullDayName;
        var width = measureDiv.clientWidth;
        var desiredWidth = header.clientWidth;
        if (width <= desiredWidth)
            header.innerHTML = fullDayName;
        else
            header.innerHTML = this.GetAbbrDayName(day);
        header.setAttribute("title", toolTip);
    },
    GetDayName: function(day) {
        var dayNames = ASPx.CultureInfo.dayNames;
        return (day == 8) ? dayNames[6] + "/" + dayNames[0] : dayNames[day];
    },
    GetAbbrDayName: function(day) {
        var dayNames = ASPx.CultureInfo.abbrDayNames;
        return (day == 8) ? dayNames[6] + "/" + dayNames[0] : dayNames[day];
    },
    SetOptimalHeadersContent: function(headers, dates, formats, headerToolTips) {
        if (headers.length <= 0)
            return;
            
        this.EnsureFormatter();
                
        var measureDiv = this.CreateMeasureDiv(headers[0]);
        
        var headerTexts = [];
        var headerContentWidths = [];
        this.InitializeHeadersIntermediateInfo(headers, headerTexts, headerContentWidths);
        
        var count = formats.length;
        for (var i = 0; i < count; i++) { // starting from long formats to short
            this.formatter.SetFormatString(formats[i]);
            this.SetOptimalHeadersContentCore(headers, dates, headerTexts, headerContentWidths, measureDiv);
        }
        this.SetNewHeadersContent(headers, headerTexts, dates, headerToolTips);
                        
        ASPx.SchedulerGlobals.RemoveChildFromParent(document.body, measureDiv);
    },
    InitializeHeadersIntermediateInfo: function(headers, headerTexts, headerContentWidths) {
        var count = headers.length;
        for (var i = 0; i < count; i++) {
            headerTexts.push("&hellip;");
            headerContentWidths.push(-1);
        }
    },
    SetNewHeadersContent: function(headers, headerTexts, dates, headerToolTips) {
        var toolTipHelper = new ASPxSchedulerSimpleToolTipHelper(headerToolTips);
        var count = headers.length;
        this.formatter.SetFormatString("d MMMM yyyy");
        for (var i = 0; i < count; i++) {
            var header = headers[i];
            var toolTip = toolTipHelper.GetToolTip(i, this.formatter.Format(dates[i]));
            header.setAttribute("title", toolTip);
            header.innerHTML = headerTexts[i];
        }
    },
    SetOptimalHeadersContentCore: function(headers, dates, headerTexts, headerContentWidths, measureDiv) {
        var count = headers.length;
        for (var i = count - 1; i >= 0; i--) {
            var text = this.formatter.Format(dates[i]);
            measureDiv.innerHTML = text;
            var width = measureDiv.clientWidth;
            var desiredWidth = headers[i].clientWidth;
            if (width <= desiredWidth && headerContentWidths[i] == -1) {
                //B138417
                //if (width >= headerContentWidths[i]) { //B37091
                    headerContentWidths[i] = width;
                    headerTexts[i] = text;
                //}
            }
            //else
            //    break;
        }
    },
    CreateMeasureDiv: function(element) {
        var result = document.createElement("div");            
        document.body.appendChild(result);
        result.style.cssText = element.style.cssText;
        result.style.position = "absolute";
        result.style.top = "-100px";
        result.style.width = "";
        result.className = element.className;
        return result;
    },
    EnsureFormatter: function() {
        if (!ASPx.IsExists(this.formatter))
            this.formatter = new ASPx.DateFormatter();
    }
}

ASPxSchedulerSimpleToolTipHelper = ASPx.CreateClass(null, {
    constructor: function(toolTips) {
        this.toolTips = toolTips;
        this.useToolTipFromServer = ASPx.IsExists(toolTips);
    },
    GetToolTip: function(toolTipIndex, defaultToolTip) {
        if (this.useToolTipFromServer)
            return this.toolTips[toolTipIndex];
        return defaultToolTip;
    }
});

ASPx.SetSchedulerDivDisplay = function(appointmentDiv, visible) {
    if (appointmentDiv.style.top == "")
        appointmentDiv.style.top = 0;
    if (appointmentDiv.style.left == "")
        appointmentDiv.style.left = 0;
    appointmentDiv.style.display = visible ? "block" : "none";
}

///////////////////////////////////////////////////////////////////////////////
/////////////// SchedulerPropertyApplyController //////////////////////////
///////////////////////////////////////////////////////////////////////////////
var SchedulerPropertyApplyController = ASPx.CreateClass(null, {
    constructor: function(scheduler) {
        this.scheduler = scheduler;
        this.createPropertyValueHandlers = {};
        this.createPropertyValueHandlers["interval"] =  ASPx.CreateDelegate(this.CreateIntervalPropertyValue, this);
    },
    CreateIntervalPropertyValue: function(property, value) {
        return new ASPxClientTimeInterval(value.start, value.duration);
    },
    ApplyProperties: function(obj, dictionary) {
        for (var property in dictionary) {
            var propertyValue = dictionary[property];
            var createValueHandler = this.createPropertyValueHandlers[property];
            if (createValueHandler) 
                propertyValue = createValueHandler(property, propertyValue);
            obj[property] = propertyValue;
        }
    }    
});
///////////////////////////////////////////////////////////////////////////////
/////////// AppointmentPropertyApplyController //////////////////////
///////////////////////////////////////////////////////////////////////////////
var AppointmentPropertyApplyController = ASPx.CreateClass(SchedulerPropertyApplyController, {
    constructor: function(scheduler) {
        this.constructor.prototype.constructor.call(this, scheduler);
        this.createPropertyValueHandlers["pattern"] = ASPx.CreateDelegate(this.CreatePatternPropertyValue, this);
        this.createPropertyValueHandlers["recurrenceInfo"] = ASPx.CreateDelegate(this.CreateRecurrenceInfoPropertyValue, this);
    },
    CreatePatternPropertyValue: function(property, value) {
        var pattern = this.scheduler.GetAppointment(value.appointmentId);
        if (pattern == null) 
            pattern = new ASPxClientAppointment();
        var propertyController = new AppointmentPropertyApplyController(this.scheduler);
        propertyController.ApplyProperties(pattern, value);
        this.scheduler.AddAppointmentPattern(pattern);
        return pattern;
    },
    CreateRecurrenceInfoPropertyValue: function(property, value) {
        var recurrenceInfo = new ASPxClientRecurrenceInfo();
        var propertyController = new RecurrenceInfoPropertyApplyController(this.scheduler);
        propertyController.ApplyProperties(recurrenceInfo, value);
        return recurrenceInfo;
    }
});
///////////////////////////////////////////////////////////////////////////////
/////////// RecurrenceInfoPropertyApplyController ///////////////////
///////////////////////////////////////////////////////////////////////////////
var RecurrenceInfoPropertyApplyController = ASPx.CreateClass(SchedulerPropertyApplyController, {
    constructor: function(scheduler) {
        this.constructor.prototype.constructor.call(this, scheduler);
    }
});

///////////////////////////////////////////////////////////////////////////////
//////////////////////////////// ToolTip helpers //////////////////////////////
///////////////////////////////////////////////////////////////////////////////
function aspxFindToolTipInParentElements(someItem) {
    function isToolTipSource(element) {
        return ASPx.IsExists(element.isToolTip) ? element.isToolTip : false;
    }
    var toolTipSource = ASPx.GetParent(someItem, isToolTipSource);
    if(ASPx.IsExists(toolTipSource)) {
        return toolTipSource;
    }
    return null;
}
function aspxIsElementPartOfToolTip(element) {
    var toolTip = aspxFindToolTipInParentElements(element);
    return toolTip != null;
}
///////////////////////////////////////////////////////////////////////////////
/////////////////////// SchedulerElementContainerChangerBase //////////////////
///////////////////////////////////////////////////////////////////////////////
var SchedulerElementContainerChangerBase = ASPx.CreateClass(null, {
    constructor: function (scheduler) {
        this.scheduler = scheduler;
        this.isActive = false;
    },
    ChangeContainer: function (container, attachEvents) {
        if (!ASPx.IsExists(container) || (container == this)) {
            // Restore original popup menus position
            if (this.movedElements) {
                this.RestoreElementContainer();
                this.movedElements = null;
            }
            this.UnsubscribeSchedulerEvents(container, attachEvents);
        }
        else {
            // Move popup menus to container
            if (this.CanChangeContainer()) {
                // If menus was already moved before
                if (this.movedElements) {
                    this.ChangeElementContainer(container);
                }
                else
                    this.movedElements = [];

                // Attach events
                this.SubscribeSchedulerEvents(container, attachEvents);
                this.ChangeContainerCore(container);
            }
        }
    },
    CanChangeContainer: function () {
        return true;
    },
    SubscribeSchedulerEvents: function (container, attachEvents) {
        if (!this.changeContainerBeginCallbackHandler && (!ASPx.IsExists(attachEvents) || attachEvents)) {
            this.isActive = true;
            this.changeContainerBeginCallbackHandler = function (s) { this.ChangeContainer(this, false); };
            this.scheduler.BeginCallback.AddHandler(ASPx.CreateDelegate(this.changeContainerBeginCallbackHandler, this));
            this.changeContainerEndCallbackHandler = function (s) { this.ChangeContainer(container, false); };
            this.scheduler.EndCallback.AddHandler(ASPx.CreateDelegate(this.changeContainerEndCallbackHandler, this));
        }
    },
    UnsubscribeSchedulerEvents: function (container, attachEvents) {
        // Detach events
        if (this.changeContainerBeginCallbackHandler && (!ASPx.IsExists(attachEvents) || attachEvents)) {
            this.isActive = false;
            this.scheduler.BeginCallback.RemoveHandler(this.changeContainerBeginCallbackHandler);
            this.changeContainerBeginCallbackHandler = null;
            this.scheduler.EndCallback.RemoveHandler(this.changeContainerEndCallbackHandler);
            this.changeContainerEndCallbackHandler = null;
        }
    },
    RestoreElementContainer: function () {
        //TODO override
    },
    ChangeElementContainer: function (container) {
        //TODO override
    },
    ChangeContainerCore: function (container) {
        //TODO override
    },
    IsActive: function () {
        return this.isActive;
    }
});
///////////////////////////////////////////////////////////////////////////////
/////////////////////////// SchedulerMenuContainerChanger /////////////////////
///////////////////////////////////////////////////////////////////////////////
var SchedulerMenuContainerChanger = ASPx.CreateClass(SchedulerElementContainerChangerBase, {
    constructor: function (scheduler) {
        this.constructor.prototype.constructor.call(this, scheduler);
    },
    CanChangeContainer: function () {
        return ASPx.IsExists(ASPx.GetMenuCollection);
    },
    RestoreElementContainer: function () {
        for (var i = 0; i < this.movedElements.length; i++)
            for (var j = 0; j < this.movedElements[i].length; j++)
                ASPx.RestoreElementContainer(this.movedElements[i][j]);
    },
    ChangeElementContainer: function (container) {
        for (var i = 0; i < this.movedElements.length; i++)
            for (var j = 0; j < this.movedElements[i].length; j++)
                ASPx.ChangeElementContainer(this.movedElements[i][j], container, false);
    },
    ChangeContainerCore: function (container) {
        var movedElements = this.movedElements; // Closure
        ASPx.GetMenuCollection().ProcessControlsInContainer(this.scheduler.GetMainElement(), function (control) {
            if (control.isPopupMenu) {
                var menuMainElement = control.GetMainElement()
                if (menuMainElement.parentNode.className.indexOf("dxmLite") != -1)
                    menuMainElement = menuMainElement.parentNode;
                var menuElements = [menuMainElement];

                // Searching for sub menus and border correctors
                var idPrefix = menuElements[0].id;
                var currentElement = menuElements[0].nextSibling;
                while (ASPx.IsExists(currentElement)) {
                    if (ASPx.IsExists(currentElement.tagName) && (currentElement.id.indexOf(idPrefix) == 0)) {
                        if (currentElement.tagName == "DIV" || currentElement.tagName == "TABLE")
                            menuElements.push(currentElement);
                    }
                    currentElement = currentElement.nextSibling;
                }

                for (var i = 0; i < menuElements.length; i++)
                    ASPx.ChangeElementContainer(menuElements[i], container, true);

                movedElements.push(menuElements);
            }
        });
    }
});

///////////////////////////////////////////////////////////////////////////////
/////////////////////// SchedulerToolTipContainerChanger //////////////////////
///////////////////////////////////////////////////////////////////////////////
var SchedulerToolTipContainerChanger = ASPx.CreateClass(SchedulerElementContainerChangerBase, {
    constructor: function (scheduler) {
        this.constructor.prototype.constructor.call(this, scheduler);
    },
    RestoreElementContainer: function () {
        var count = this.movedElements.length;
        for (var i = 0; i < count; i++)
            ASPx.RestoreElementContainer(this.movedElements[i]);
    },
    ChangeElementContainer: function (container) {
        var count = this.movedElements.length;
        for (var i = 0; i < count; i++)
            ASPx.ChangeElementContainer(this.movedElements[i], container, false);
    },
    ChangeContainerCore: function (container) {
        var toolTips = [this.scheduler.GetAppointmentDragTooltip(), this.scheduler.GetSelectionToolTip(), this.scheduler.GetAppointmentToolTip()];
        for (var i in toolTips) {
            var toolTip = toolTips[i];
            if (!toolTip)
                continue;
            var toolTipDiv = toolTip.GetMainDiv();
            this.movedElements.push(toolTipDiv);
            ASPx.ChangeElementContainer(toolTipDiv, container, true);
            toolTip.UpdateMainDiv();
        }
    }
});

///////////////////////////////////////////////////////////////////////////////
/////////////////////// SchedulerFormContainerChanger //////////////////////
///////////////////////////////////////////////////////////////////////////////
var SchedulerFormContainerChanger = ASPx.CreateClass(SchedulerElementContainerChangerBase, {
    constructor: function (scheduler) {
        this.constructor.prototype.constructor.call(this, scheduler);
    },
    RestoreElementContainer: function () {
        var count = this.movedElements.length;
        for (var i = 0; i < count; i++)
            ASPx.RestoreElementContainer(this.movedElements[i]);
    },
    ChangeElementContainer: function (container) {
        var count = this.movedElements.length;
        for (var i = 0; i < count; i++)
            ASPx.ChangeElementContainer(this.movedElements[i], container, false);
    },
    ChangeContainerCore: function (container) {
        var formBlockDiv = ASPx.GetElementById(this.scheduler.GetBlockElementId("formBlock", "innerContent"));
        if (!formBlockDiv)
            return;
        this.movedElements.push(formBlockDiv);
        ASPx.ChangeElementContainer(formBlockDiv, container, true);
        this.scheduler.ProcessShowFormPopupWindowDeferred();
    }
});

var SchedulerFuncCallback = ASPx.CreateClass(null, {
    constructor: function (callbackName, args, handler) {
        this.callbackName = callbackName;
        this.args = args;
        this.handler = handler;
    },
    Raise: function (scheduler) {
        scheduler.RaiseFuncCallbackCore(this.callbackName, this.args, this.handler);
    }
})

var SchedulerBlockPropertiesInfo = ASPx.CreateClass(null, {
    constructor: function (blockId, params) {
        this.blockId = blockId;
        this.params = params;
    }
});

var TimeRuler = ASPx.CreateClass(null, {
    constructor: function (info) {
        this.cellLocation = info.p;
        this.isVisible = info.v;
    },
    GetAnchorCell: function (scheduler) {
        if (!this.timeRulerAnchorCell) {
            var rowIndex = this.cellLocation[0];
            var cellIndex = this.cellLocation[1];
            this.timeRulerAnchorCell = scheduler.vertTable.rows[rowIndex].cells[cellIndex]
        }
        return this.timeRulerAnchorCell;
    },
    IsVisible: function () {
        return this.isVisible;
    }
});

var SchedulerBrowserHelper = ASPx.CreateClass(null, {});
SchedulerBrowserHelper.IsIE9CompatibilityView = function () {
    //http://msdn.microsoft.com/en-us/library/ms537503(v=vs.85).aspx
    //http://social.msdn.microsoft.com/Forums/en/netfxjscript/thread/ae715fd2-1ddd-46f7-8c26-9aed6b2103f1
    if (!ASPx.Browser.IE)
        return false;
    var userAgent = navigator.userAgent;
    return userAgent.indexOf("Trident/5.0") > -1 && userAgent.indexOf("MSIE 7.0") > -1;
}

var SchedulerLoadImageChecker = ASPx.CreateClass(null, {
    constructor: function (scheduler) {
        this.scheduler = scheduler;
        this.roots = [];
        this.imageCount = 0;
        this.disabled = true;
    },
    Reset: function () {
        if (this.disabled)
            return;
        this.roots = [];
        this.imageCount = 0;
    },
    Add: function (root) {
        if (this.disabled || this.root == null)
            return;
        var images = ASPx.GetNodesByTagName(root, "img");
        var count = images.length;
        for (var i = 0; i < count; i++) {
            var image = images[i];
            if (this.IsImageLoaded(image))
                break;
            this.SubscribeEvent(image);
        }
    },
    SubscribeEvent: function (image) {
        this.imageCount++;
        ASPx.Evt.AttachEventToElement(image, "load", ASPx.CreateDelegate(this.OnLoad, this));
        ASPx.Evt.AttachEventToElement(image, "error", ASPx.CreateDelegate(this.OnLoad, this));
    },
    IsImageLoaded: function (image) {
        if (image.src == "")
            return false;
        return image.complete;
    },
    OnLoad: function () {
        this.imageCount--;
        if (this.imageCount == 0)
            this.RaiseRefreshLayout();
    },
    RaiseRefreshLayout: function () {
        this.Reset();
        this.scheduler.RecalcLayout();
    }
});

var SchedulerTimer = ASPx.CreateClass(null, {
    constructor: function (time, callback) {
        this.time = time;
        this.callback = callback;
        this.isDeferred = false;
        this.defferedCallbackOccurred = false
    },
    Start: function () {
        if (!this.time)
            return;
        this.startTime = new Date();
        this.timerId = window.setTimeout(ASPx.CreateDelegate(this.OnCallback, this), this.time);
    },
    Pause: function () {
        if (!this.time || !this.startTime)
            return;
        ASPx.Timer.ClearTimer(this.timerId);
        this.timerId = null;
        this.time = this.time - (new Date().valueOf() - this.startTime.valueOf());
        if (this.time <= 0)
            this.time = null;
    },
    BeginDeferredAlert: function () {
        this.isDeferred = true;
    },
    EndDeferredAlert: function () {
        if (this.isDeferred && this.defferedCallbackOccurred)
            this.callback();
        else
            this.defferedCallbackOccurred = false;
        this.isDeferred = false;
    },
    OnCallback: function () {
        if (!this.time || !this.callback)
            return;        
        if (this.isDeferred) 
            this.defferedCallbackOccurred = true;
        else
            this.callback();
    }
});

var ASPxClientSchedulerValidationCompletedArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function (isValid) {
        this.isValid = isValid;
    }
});



ASPx.SchedulerFormType = SchedulerFormType;
ASPx.SchedulerFormVisibility = SchedulerFormVisibility;

ASPx.SchedulerUtils = SchedulerUtils;
ASPx.SchedulerRelatedControlBase = SchedulerRelatedControlBase;

ASPx.SchedulerMeasurer = SchedulerMeasurer;
ASPx.SchedulerPropertyApplyController = SchedulerPropertyApplyController;
ASPx.AppointmentPropertyApplyController = AppointmentPropertyApplyController;

ASPx.FindToolTipInParentElements = aspxFindToolTipInParentElements;
ASPx.IsElementPartOfToolTip = aspxIsElementPartOfToolTip;

ASPx.SchedulerMenuContainerChanger = SchedulerMenuContainerChanger;
ASPx.SchedulerToolTipContainerChanger = SchedulerToolTipContainerChanger;
ASPx.SchedulerFormContainerChanger = SchedulerFormContainerChanger;
ASPx.SchedulerFuncCallback = SchedulerFuncCallback;
ASPx.SchedulerBlockPropertiesInfo = SchedulerBlockPropertiesInfo;
ASPx.TimeRuler = TimeRuler;

ASPx.SchedulerBrowserHelper = SchedulerBrowserHelper;
ASPx.SchedulerLoadImageChecker = SchedulerLoadImageChecker;
ASPx.SchedulerTimer = SchedulerTimer;

window.ASPxClientRecurrenceRange = ASPxClientRecurrenceRange;
window.ASPxClientRecurrenceType = ASPxClientRecurrenceType;
window.ASPxClientWeekDays = ASPxClientWeekDays;
window.ASPxClientWeekOfMonth = ASPxClientWeekOfMonth;
window.ASPxClientTimeIndicatorVisibility = ASPxClientTimeIndicatorVisibility;
window.ASPxSchedulerDateTimeHelper = ASPxSchedulerDateTimeHelper;
window.ASPxClientSchedulerValidationCompletedArgs = ASPxClientSchedulerValidationCompletedArgs;
})();