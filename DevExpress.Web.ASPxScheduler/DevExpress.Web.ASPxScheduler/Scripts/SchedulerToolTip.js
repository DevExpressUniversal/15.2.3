

(function () {
////////////////////////////////////////////////////////////////////////////////
// ASPxClientScheduler
////////////////////////////////////////////////////////////////////////////////
var SchedulerToolTip = ASPx.CreateClass(ASPxClientControl, {
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);
        this.canShowToolTip = true;
    },
    Initialize: function () {
        this.constructor.prototype.Initialize.call(this);
        this.mainDiv = this.GetMainDiv();
        this.toolTipParent = this.mainDiv.parentNode;
        this.mainDiv.isToolTip = true;
        if (this.isVisible)
            this.ShowToolTip();
    },
    HideToolTip: function () {
        if (!this.isVisible)
            return;
        this.isVisible = false;
        ASPx.SetElementDisplay(this.mainDiv, false);
        this.aspxParentControl.activeToolTip = null;
    },
    ShouldResetPositionByTimer: function () {
        if (!ASPx.IsExists(this.templatedToolTip))
            return false;
        return this.templatedToolTip.ShouldResetPositionByTimer();
    },
    CanCloseByMouseClick: function () {
        if (!ASPx.IsExists(this.templatedToolTip))
            return false;
        return this.templatedToolTip.CanCloseByMouseClick();
    },
    ShowToolTip: function (documentX, documentY) {
        if (ASPx.IsExists(this.aspxParentControl.activeToolTip) && this.aspxParentControl.activeToolTip != this) {
            return;
        }
        this.aspxParentControl.activeToolTip = this;
        this.isVisible = true;
        this.mainDiv.viewInfo = this.viewInfo;
        ASPx.SetElementVisibility(this.mainDiv, false);
        ASPx.SetElementDisplay(this.mainDiv, true);
        var position = this.CalculateToolTipPosition(documentX, documentY);
        var xPos = position.GetX();
        var yPos = position.GetY();
        if (!ASPxClientScheduler.DisableSmartToolTipLayout && !(ASPx.Browser.MacOSMobilePlatform || ASPx.Browser.AndroidMobilePlatform)) {
            var clientWidth = ASPx.Browser.WebKitFamily ? document.documentElement.clientWidth : ASPx.GetDocumentClientWidth();
            xPos += this.CalculateToolTipOffset(ASPx.GetDocumentScrollLeft(), clientWidth, position.GetX(), this.mainDiv.offsetWidth);
            yPos += this.CalculateToolTipOffset(ASPx.GetDocumentScrollTop(), ASPx.GetDocumentClientHeight(), position.GetY(), this.mainDiv.offsetHeight);
        }
        this.SetDivPosition(this.mainDiv, xPos - ASPx.GetPositionElementOffset(this.mainDiv, true), yPos - ASPx.GetPositionElementOffset(this.mainDiv, false));
        ASPx.SetElementVisibility(this.mainDiv, true);
    },
    CalculateToolTipOffset: function (constraintStart, constraintLength, start, length) {
        var constraintEnd = constraintStart + constraintLength;
        var end = start + length;
        var offset = constraintEnd - end;
        if (offset < 0)
            return offset;
        offset = constraintStart - start;
        if (offset > 0)
            return offset;
        return 0;
    },
    CalculateToolTipPosition: function (documentX, documentY) {
        var positionX = documentX;
        var positionY = documentY;
        var position = new ASPxClientPoint(positionX, positionY);
        if (ASPx.IsExists(this.templatedToolTip)) {
            var bounds = new ASPxClientRect(positionX, positionY, this.mainDiv.offsetWidth, this.mainDiv.offsetHeight);
            position = this.templatedToolTip.CalculatePosition(bounds);
        }
        return position;
    },
    SetDivPosition: function (element, left, dxtop) {
        element.style.left = left + "px";
        element.style.top = dxtop + "px";
    },
    CanShowToolTip: function (toolTipData) {
        if (!this.canShowToolTip)
            return false;
        if (!ASPx.IsExists(this.templatedToolTip) || !toolTipData)
            return this.canShowToolTip;
        return this.templatedToolTip.CanShowToolTip(toolTipData);
    },
    GetAppointment: function (dataObject) {
        if (!dataObject || !dataObject.appointmentId)
            return null;
        var scheduler = this.GetSchedulerControl();
        if (!scheduler)
            return null;
        return scheduler.GetAppointment(dataObject.appointmentId);
    },
    AdjustImageWidth: function (element) {
        /*if(!ASPx.IsExists(element))
        return;
        var parentNode = _aspxGetParentNode(element);
        element.style.height = element.offsetHeight + "px";
        parentNode.style.width = "100%";
        element.style.width = parentNode.offsetWidth + "px";*/
    },
    AdjustCellSize: function (cell, image) {
        /*if(!ASPx.IsExists(cell) || !ASPx.IsExists(image))
        return;
        cell.style.width = image.offsetWidth + "px";
        cell.style.height = image.offsetHeight + "px";*/
    },
    GetElementById: function (id) {
        return ASPx.GetElementById(this.name + "_" + id);
    },
    SetContent: function (toolTipData) {
        //this.contentDiv.innerHTML = content;
        if (!ASPx.IsExists(this.templatedToolTip))
            return;
        this.templatedToolTip.Update(toolTipData);
    },
    FinalizeUpdate: function (toolTipData) {
        //this.contentDiv.innerHTML = content;
        if (!ASPx.IsExists(this.templatedToolTip))
            return;
        this.templatedToolTip.FinalizeUpdate(toolTipData);
    },
    UpdateMainDiv: function () {
        this.mainDiv = this.GetMainDiv();
        this.toolTipParent = this.mainDiv.parentNode;
    },
    GetMainDiv: function () {
        return this.GetElementById("mainDiv");
    },
    GetSchedulerControl: function () {
        if (!ASPx.IsExists(this.templatedToolTip))
            return null;
        return this.templatedToolTip.scheduler;
    },
    IsDOMDisposed: function () {
        return !ASPx.IsExistsElement(this.GetMainDiv());
    }
});
var ASPxClientSchedulerToolTipData = ASPx.CreateClass(null, {
    constructor: function(appointment, interval, resources) {
        this.appointment = ASPx.IsExists(appointment) ? appointment : null;
        this.interval = ASPx.IsExists(interval) ? interval : null;
        this.resources = ASPx.IsExists(resources) ? resources : null;
    },
    GetAppointment: function() {
        return this.appointment;
    },
    GetInterval: function() {
        return this.interval;
    },
    GetResources: function() {
        return this.resoruces;
    }
    
});

////////////////////////////////////////////////////////////////////////////////
// ASPxClientToolTipBase
////////////////////////////////////////////////////////////////////////////////
var ASPxClientToolTipBase = ASPx.CreateClass(null, {
    constructor: function (scheduler) {
        this.constructor.prototype.constructor.call(this);
        this.scheduler = scheduler;
        this.controls = new Object;
        this.canCloseByClick = false;
    },
    Initialize: function () {
    },
    CanShowToolTip: function (toolTipData) {
        return true;
    },
    FinalizeUpdate: function (toolTipData) {
    },
    Update: function (toolTipData) {
    },
    Close: function () {
        if (ASPx.IsExists(this.scheduler)) {
            this.scheduler.HideAllToolTips();
        }
    },
    CalculatePosition: function (bounds) {
        return new ASPxClientPoint(bounds.GetLeft() - bounds.GetWidth() / 2, bounds.GetTop() - bounds.GetHeight());
    },
    ShowAppointmentMenu: function (s) {
        var evt = ASPx.Evt.GetEvent(s);
        if (!ASPx.IsExists(evt)) return null;
        var sender = ASPx.Browser.IE ? evt.srcElement : evt.target
        if (!ASPx.IsExists(sender)) return null;
        this.scheduler.OnAppointmentToolTipClick(sender, evt);
    },
    ShowViewMenu: function (s) {
        var evt = ASPx.Evt.GetEvent(s);
        if (!ASPx.IsExists(evt)) return null;
        var sender = ASPx.Browser.IE ? evt.srcElement : evt.target
        if (!ASPx.IsExists(sender)) return null;
        this.scheduler.menuManager.ShowViewMenu(sender, evt);
    },
    ShouldResetPositionByTimer: function () {
        if (ASPx.IsExists(this.resetPositionByTimer))
            return this.resetPositionByTimer;
        return false;
    },
    CanCloseByMouseClick: function () {
        return this.canCloseByClick;
    },
    ConvertIntervalToString: function (interval) {
        var formatter = new ASPx.DateFormatter();
        var startTimeFormat = this.SelectStartTimeFormat(this.scheduler, interval);
        var endTimeFormat = this.SelectEndTimeFormat(this.scheduler, interval);
        formatter.SetFormatString(startTimeFormat);
        var result = formatter.Format(interval.GetStart());
        if (ASPx.IsExists(endTimeFormat)) {
            formatter.SetFormatString(endTimeFormat);
            result += " - " + formatter.Format(interval.GetEnd());
        }
        return result;
    },
    SelectStartTimeFormat: function (scheduler, interval) {
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
        if (datesEquals) {
            if (interval.IsSmallerThanDay())
                return scheduler.formatsTimeWithMonthDay[0];
            else
                return scheduler.formatsWithoutYearAndWeekDay[0];
        }
        else {
            if (truncStartDate - interval.GetStart() == 0 && truncEndDate - interval.GetEnd() == 0) {
                if (startYear == endYear || interval.IsDurationEqualToDay())
                    return scheduler.formatsWithoutYearAndWeekDay[0];
                else
                    return scheduler.formatsDateWithYear[0];
            }
            else {
                if (startYear == endYear)
                    return scheduler.formatsTimeWithMonthDay[0];
                else
                    return scheduler.formatsDateTimeWithYear[0];
            }
        }
    },
    SelectEndTimeFormat: function (scheduler, interval) {
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
        if (datesEquals) {
            if (interval.IsSmallerThanDay())
                return scheduler.formatsTimeOnly[0];
            else
                return null;
        }
        else {//startDate != endDate
            if (truncStartDate - interval.GetStart() == 0 && truncEndDate - interval.GetEnd() == 0) {
                if (startYear == endYear || interval.IsDurationEqualToDay())
                    return (interval.IsDurationEqualToDay()) ? null : scheduler.formatsWithoutYearAndWeekDay[0];
                else
                    return scheduler.formatsDateWithYear[0];
            }
            else {
                if (startYear == endYear)
                    return scheduler.formatsTimeWithMonthDay[0];
                else
                    return scheduler.formatsDateTimeWithYear[0];
            }
        }
    }
});
////////////////////////////////////////////////////////////////////////////////
// ASPxClientSelectionToolTip
////////////////////////////////////////////////////////////////////////////////
var ASPxClientSelectionToolTip = ASPx.CreateClass(ASPxClientToolTipBase, {
    Initialize: function() {
        ASPxClientUtils.AttachEventToElement(this.controls.buttonDiv, "click", ASPx.CreateDelegate(this.OnButtonDivClick, this));
    },
    OnButtonDivClick: function(s,e) {
        this.ShowViewMenu(s);
    }
});

ASPx.SchedulerToolTip = SchedulerToolTip;

window.ASPxClientSchedulerToolTipData = ASPxClientSchedulerToolTipData;
window.ASPxClientToolTipBase = ASPxClientToolTipBase;
window.ASPxClientSelectionToolTip = ASPxClientSelectionToolTip;
})();