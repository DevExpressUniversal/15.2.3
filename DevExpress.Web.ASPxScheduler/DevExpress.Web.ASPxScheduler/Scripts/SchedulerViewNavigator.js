(function() {
////////////////////////////////////////////////////////////////////////////////
// ASPxClientSchedulerViewNavigator
////////////////////////////////////////////////////////////////////////////////
var ASPxClientSchedulerViewNavigator = ASPx.CreateClass(ASPx.SchedulerRelatedControlBase, {
    NavigateBackward: function () {
        var schedulerControl = ASPx.GetControlCollection().Get(this.schedulerControlId);
        if (ASPx.IsExists(schedulerControl))
            schedulerControl.NavigateBackward();
    },
    NavigateForward: function () {
        var schedulerControl = ASPx.GetControlCollection().Get(this.schedulerControlId);
        if (ASPx.IsExists(schedulerControl))
            schedulerControl.NavigateForward();
    },
    GotoToday: function () {
        var schedulerControl = ASPx.GetControlCollection().Get(this.schedulerControlId);
        if (ASPx.IsExists(schedulerControl))
            schedulerControl.GotoToday();
    },
    ShowGotoDateCalendar: function () {
        if (this.calendarId) {
            var schedulerControl = ASPx.GetControlCollection().Get(this.schedulerControlId);
            var schedulerGotoDateCalendar = ASPx.GetControlCollection().Get(this.calendarId);
            var popupDiv = ASPx.GetElementById(this.calendarPopupDivId);
            if (schedulerControl && schedulerGotoDateCalendar && popupDiv) {
                var intervals = schedulerControl.GetVisibleIntervals();
                if (intervals.length > 0) {
                    schedulerGotoDateCalendar.SetValue(intervals[0].start);
                }
                var buttonElement = ASPx.GetElementById(this.gotoDateButtonId);
                var posX = ASPx.GetAbsolutePositionX(buttonElement);
                var posY = ASPx.GetAbsolutePositionY(buttonElement) + buttonElement.offsetHeight;
                schedulerControl.ShowPopupDiv(popupDiv);
                ASPx.SetAbsoluteX(popupDiv, posX);
                ASPx.SetAbsoluteY(popupDiv, posY);
            }
        }
    },
    GotoDate: function (calendar) {
        var schedulerControl = ASPx.GetControlCollection().Get(this.schedulerControlId);
        var popupDiv = ASPx.GetElementById(this.calendarPopupDivId);
        schedulerControl.HidePopupDiv(popupDiv);
        var date = calendar.GetValue();
        schedulerControl.GotoDate(date);
    }
});
////////////////////////////////////////////////////////////////////////////////

ASPx.SchedulerShowGotoDateCalendar = function(name) {
    var schedulerViewNavigator = ASPx.GetControlCollection().Get(name);
    if(schedulerViewNavigator)
        schedulerViewNavigator.ShowGotoDateCalendar();
}
ASPx.SchedulerGotoDate = function(calendar, name) {
    var schedulerViewNavigator = ASPx.GetControlCollection().Get(name);
    if(schedulerViewNavigator)
        schedulerViewNavigator.GotoDate(calendar);
}
ASPx.SchedulerNavigateViewBackward = function(name) {
    var schedulerViewNavigator = ASPx.GetControlCollection().Get(name);
    if (ASPx.IsExists(schedulerViewNavigator))
        schedulerViewNavigator.NavigateBackward();
}

ASPx.SchedulerNavigateViewForward = function(name) {
    var schedulerViewNavigator = ASPx.GetControlCollection().Get(name);
    if (ASPx.IsExists(schedulerViewNavigator))
        schedulerViewNavigator.NavigateForward();
}

ASPx.SchedulerGotoToday = function(name) {
    var schedulerViewNavigator = ASPx.GetControlCollection().Get(name);
    if (ASPx.IsExists(schedulerViewNavigator))
        schedulerViewNavigator.GotoToday();
}

window.ASPxClientSchedulerViewNavigator = ASPxClientSchedulerViewNavigator;
})();