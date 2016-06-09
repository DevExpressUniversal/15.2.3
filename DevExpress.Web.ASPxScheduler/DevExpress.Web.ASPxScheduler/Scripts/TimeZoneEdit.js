(function() {
////////////////////////////////////////////////////////////////////////////////
// ASPxClientTimeZoneEdit
////////////////////////////////////////////////////////////////////////////////
var ASPxClientTimeZoneEdit = ASPx.CreateClass(ASPx.SchedulerRelatedControlBase, {
    ChangeTimeZoneId: function(tzId) {
        var schedulerControl = ASPx.GetControlCollection().Get(this.schedulerControlId);
        if (ASPx.IsExists(schedulerControl)) {
            schedulerControl.ChangeTimeZoneId(tzId);
        }
    }
});
////////////////////////////////////////////////////////////////////////////////

ASPx.TimeZoneEditComboSelectedIndexChanged = function(name, tzId) {
    var timeZoneEdit = ASPx.GetControlCollection().Get(name);
    if (ASPx.IsExists(timeZoneEdit))
        timeZoneEdit.ChangeTimeZoneId(tzId);
}

window.ASPxClientTimeZoneEdit = ASPxClientTimeZoneEdit;
})();