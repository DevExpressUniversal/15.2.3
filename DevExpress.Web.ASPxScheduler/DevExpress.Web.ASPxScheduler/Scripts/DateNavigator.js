(function () {
ASPx.VisibleMonthChanged = function(dateNavigatorId, offset) {
    //debugger;
    var dateNavigator = ASPx.GetControlCollection().Get(dateNavigatorId);
    if (ASPx.IsExists(dateNavigator))
        dateNavigator.OnVisibleMonthChanged(offset);
}
ASPx.DateNavigatorSelectionChanged = function(dateNavigatorId) {
    var dateNavigator = ASPx.GetControlCollection().Get(dateNavigatorId);
    if (ASPx.IsExists(dateNavigator))
        dateNavigator.OnSelectionChanged();
}
////////////////////////////////////////////////////////////////////////////////
// ASPxClientDateNavigator
////////////////////////////////////////////////////////////////////////////////
var ASPxClientDateNavigator = ASPx.CreateClass(ASPx.SchedulerRelatedControlBase, {
	constructor: function(name) {
		this.constructor.prototype.constructor.call(this, name);		
		this.calendarId = "";
    },
	OnVisibleMonthChanged: function (offset) {
        var calendar = ASPx.GetControlCollection().Get(this.calendarId);
        if (!ASPx.IsExists(calendar)) 
            return;        
        this.UpdateCalendarViews();
        var schedulerControl = ASPx.GetControlCollection().Get(this.schedulerControlId);
        if (!ASPx.IsExists(schedulerControl))
            return;
        var formattedDates = calendar.FormatDates(calendar.selection.GetDates(), ",");
        var formattedFirstDate = ASPx.DateUtils.GetInvariantDateString(this.GetFirstDate());
        var formattedLastDate = ASPx.DateUtils.GetInvariantDateString(this.GetLastDate());
        schedulerControl.ShiftVisibleIntervals(offset, formattedFirstDate, formattedLastDate, formattedDates);
    },
    OnSelectionChanged: function() {
        var calendar = ASPx.GetControlCollection().Get(this.calendarId);
        if (ASPx.IsExists(calendar)) {
            var schedulerControl = ASPx.GetControlCollection().Get(this.schedulerControlId);
            if (ASPx.IsExists(schedulerControl))
                var formattedDates = calendar.FormatDates(calendar.selection.GetDates(), ",");
                schedulerControl.SetVisibleDays(formattedDates);
        }
    },
    GetFirstDate: function () {
        var calendar = ASPx.GetControlCollection().Get(this.calendarId);
        if (!ASPx.IsExists(calendar))
            return "";
        var startDate = ASPxClientCalendar.CloneDate(calendar.GetView(0, 0).visibleDate);
        startDate.setDate(1);
        return startDate;
    },
    GetLastDate: function () {
        var calendar = ASPx.GetControlCollection().Get(this.calendarId);
        if (!ASPx.IsExists(calendar))
            return "";
        var endDate = ASPxClientCalendar.CloneDate(calendar.GetView(calendar.rows - 1, calendar.columns - 1).visibleDate);
        endDate.setDate(ASPxClientCalendar.GetDaysInMonth(endDate.getMonth(), endDate.getFullYear()));
        return endDate;
    },
    UpdateCalendarViews: function () {
        var calendar = ASPx.GetControlCollection().Get(this.calendarId);
        if (!ASPx.IsExists(calendar))
            return;
        for (var key in calendar.views) {
            var view = calendar.views[key];
            if (!view.UpdateDate) continue;
            view.UpdateDate();
        }
    }
});
////////////////////////////////////////////////////////////////////////////////

window.ASPxClientDateNavigator = ASPxClientDateNavigator;
})();