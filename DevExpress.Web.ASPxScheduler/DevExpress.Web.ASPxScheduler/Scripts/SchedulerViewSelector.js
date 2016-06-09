(function() {
////////////////////////////////////////////////////////////////////////////////
// ASPxClientSchedulerViewSelector
////////////////////////////////////////////////////////////////////////////////
var ASPxClientSchedulerViewSelector = ASPx.CreateClass(ASPx.SchedulerRelatedControlBase, {
    SelectView: function(view) {
        var schedulerControl = ASPx.GetControlCollection().Get(this.schedulerControlId);
        if (ASPx.IsExists(schedulerControl))
            schedulerControl.SetActiveViewType(view);
    }
});
////////////////////////////////////////////////////////////////////////////////

ASPx.SchedulerSelectView = function(name, view) {
    var schedulerViewSelector = ASPx.GetControlCollection().Get(name);
    if (ASPx.IsExists(schedulerViewSelector))
        schedulerViewSelector.SelectView(view);
}

window.ASPxClientSchedulerViewSelector = ASPxClientSchedulerViewSelector;
})();