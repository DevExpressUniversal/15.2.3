(function () {
////////////////////////////////////////////////////////////////////////////////
// ASPxClientSchedulerCalendar
////////////////////////////////////////////////////////////////////////////////
var ASPxClientSchedulerCalendar = ASPx.CreateClass(ASPxClientCalendar, {
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);
    },
    OnTodayClick: function () {
        var todayDate = this.GetTodayDate();
        if (this.IsDateInRange(todayDate)) {
            if (!this.IsDateDisabled(todayDate))
                this.MoveSelection(todayDate);
        } else {
            var nearestDate = this.FindNearestDate(todayDate);
            if (nearestDate != null)
                this.MoveSelection(nearestDate);
        }
    },
    FindNearestDate: function (targetDate) {
        var result;
        if (targetDate >= this.maxDate) {
            result = ASPxClientCalendar.AddDays(this.maxDate, -1);
            while (this.IsDateDisabled(result) && result >= this.minDate)
                result = ASPxClientCalendar.AddDays(this.maxDate, -1);
        } else if (targetDate <= this.minDate) {
            result = this.minDate;
            while (this.IsDateDisabled(result) && result < this.maxDate)
                result = ASPxClientCalendar.AddDays(this.maxDate, 1);
        }

        if (result >= this.minDate && result < this.maxDate)
            return result;
        else
            return null;
    },
    MoveSelection: function (targetDate) {
        this.selectionTransaction.Start();
        this.selectionTransaction.selection.Add(targetDate);
        this.lastSelectedDate = ASPxClientCalendar.CloneDate(targetDate);
        this.OnSelectionChanging();

        if (!ASPxClientCalendar.AreDatesOfSameMonth(targetDate, this.visibleDate))
            this.OnVisibleMonthChanged(targetDate);
    }
});
////////////////////////////////////////////////////////////////////////////////

window.ASPxClientSchedulerCalendar = ASPxClientSchedulerCalendar;
})();