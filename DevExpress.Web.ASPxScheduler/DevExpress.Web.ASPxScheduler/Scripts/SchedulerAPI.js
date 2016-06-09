

(function() {
var ASPxClientSchedulerRefreshAction = ASPx.CreateClass(null, {
});
ASPxClientSchedulerRefreshAction.None = 0;
ASPxClientSchedulerRefreshAction.VisibleIntervalChanged = 1;
ASPxClientSchedulerRefreshAction.ActiveViewTypeChanged = 2;
var ASPxClientAppointmentOperation = ASPx.CreateClass(null, {
    constructor: function(helper, callbackParameters) {
        this.helper = helper;
        this.callbackParameters = callbackParameters;
    },
    Apply: function() {
        this.helper.Apply(this.callbackParameters);
    },
    Cancel: function() {
        this.helper.Cancel();        
    }
}); 
ASPxClientScheduler.Cast = ASPxClientControl.Cast;
ASPxClientScheduler.prototype.GetActiveViewType = function() {
    return this.privateActiveViewType;
}
ASPxClientScheduler.prototype.SetActiveViewType = function(newViewType) {
    var activeViewType = this.GetActiveViewType();
    if (activeViewType == newViewType)
        return;

    if (this.RaiseActiveViewChanging(activeViewType, newViewType))
        this.RaiseCallback("SAVT|" + newViewType);
}
ASPxClientScheduler.prototype.PerformCallback = function(parameter) {
        this.RaiseCallback("CUSTOMCALLBACK|" + parameter);
}
ASPxClientScheduler.prototype.Refresh = function (refreshAction) {
    var args = "";
    if (refreshAction == ASPxClientSchedulerRefreshAction.ActiveViewTypeChanged)
        this.SetCheckSums("");
    if (ASPx.IsExists(refreshAction))
        args = refreshAction;
    this.RaiseCallback("REFRESH|" + args);
}
ASPxClientScheduler.prototype.GetGroupType = function() {
    return this.privateGroupType;
}
ASPxClientScheduler.prototype.SetGroupType = function(groupType) {
    this.RaiseCallback("SVGT|" + groupType);
}
ASPxClientScheduler.prototype.GotoToday = function() {
    this.AssignSlideAnimationDirectionByDate(new Date());
    this.RaiseCallback("GOTODAY|");
}
ASPxClientScheduler.prototype.GotoDate = function(date) {
    this.AssignSlideAnimationDirectionByDate(date);
    var ms = ASPx.SchedulerGlobals.DateTimeToMilliseconds(date);
    this.RaiseCallback("GOTODATE|" + ms);
}
ASPxClientScheduler.prototype.NavigateBackward = function() {
    this.slideAnimationDirection = this.IsCallbackAnimationEnabled() ? ASPx.AnimationHelper.SLIDE_RIGHT_DIRECTION : null;
    this.RaiseCallback("BACK|");
}
ASPxClientScheduler.prototype.NavigateForward = function() {
    this.slideAnimationDirection = this.IsCallbackAnimationEnabled() ? ASPx.AnimationHelper.SLIDE_LEFT_DIRECTION : null;
    this.RaiseCallback("FORWARD|");
}

ASPxClientScheduler.prototype.ShiftVisibleIntervals = function (offset, firstDate, lastDate, formattedDates) {
    this.RaiseCallback("OFFSETVISI|" + offset + ',' + firstDate + ',' + lastDate + ',' + formattedDates);
}

ASPxClientScheduler.prototype.SetVisibleDays = function(days) {
    this.RaiseCallback("SETVISDAYS|" + days);
}
ASPxClientScheduler.prototype.ChangeTimeZoneId = function(tzi) {
    this.RaiseCallback("TZI|" + tzi);
}
ASPxClientScheduler.prototype.ShowSelectionToolTip = function(x, y) {
    this.ShowSelectionToolTipInernal(x, y);
}
ASPxClientScheduler.prototype.GetSelectedInterval = function() {
    return new ASPxClientTimeInterval(this.selection.interval.start, this.selection.interval.duration);
}
ASPxClientScheduler.prototype.GetSelectedResource = function() {
    return this.selection.resource;
}
ASPxClientScheduler.prototype.GetAppointmentById = function(id) {
    return this.GetAppointment(id);
}
ASPxClientScheduler.prototype.GetSelectedAppointmentIds = function() {
    return this.appointmentSelection.selectedAppointmentIds;
}
ASPxClientScheduler.prototype.DeselectAppointmentById = function(aptId) {
    this.appointmentSelection.RemoveAppointmentFromSelection(aptId);
}
ASPxClientScheduler.prototype.SelectAppointmentById = function(aptId) {
    this.appointmentSelection.SelectSingleAppointment(aptId);
}
ASPxClientScheduler.prototype.GetAppointmentProperties = function(aptId, propertyNames, onCallBack) {
     this.RaiseFuncCallback("APTDATA|", aptId + "," + propertyNames, onCallBack);
 }
ASPxClientScheduler.prototype.RefreshClientAppointmentProperties = function(clientAppointment, propertyNames, onCallBack) {
    if (ASPx.IsExists(onCallBack)) {
        this.RefreshClientAppointmentPropertiesUserCallbackFunction = onCallBack;
    }
    else
        this.RefreshClientAppointmentPropertiesUserCallbackFunction = null;
    var callbackDelegate = ASPx.CreateDelegate(this.RefreshClientAppointmentPropertiesCore, this);
    this.RaiseFuncCallback("APTDATAEX|", clientAppointment.appointmentId + "," + propertyNames, callbackDelegate);
}
ASPxClientScheduler.prototype.ShowAppointmentFormByClientId = function(aptClientId) {
    this.RaiseCallback("EDTFRMSHOW|" + aptClientId);
}
ASPxClientScheduler.prototype.ShowAppointmentFormByServerId = function(aptServerId) {
    this.RaiseCallback("EDTFRMSHOWSID|" + aptServerId);
}
ASPxClientScheduler.prototype.SetTopRowTime = function(duration, viewType) {
    var actualViewType = this.privateActiveViewType;
    if (ASPx.IsExists(viewType)) 
        actualViewType = viewType;
    var state = this.topRowTimeManager.CreateTopRowTimeState(duration, -1);
    this.topRowTimeManager.SetTopRowTimeState(state, actualViewType);
}
ASPxClientScheduler.prototype.GetTopRowTime = function(viewType) {
    var actualViewType = this.privateActiveViewType;
    if (ASPx.IsExists(viewType)) 
        actualViewType = viewType;
    var state = this.topRowTimeManager.GetTopRowTimeState(actualViewType);
    return state.duration;
}
ASPxClientScheduler.prototype.ShowInplaceEditor = function(startDate, endDate, resourceId) {
    var start = ASPx.SchedulerGlobals.DateTimeToMilliseconds(startDate);
    var end = ASPx.SchedulerGlobals.DateTimeToMilliseconds(endDate);
    var args = start + "," + end;
    if (ASPx.IsExists(resourceId))
        args += "," + resourceId;
    this.RaiseCallback("NEWAPTVIAINPLFRM|" + args);
}
ASPxClientScheduler.prototype.InsertAppointment = function(clientAppointment) {
    var args  = ASPx.Json.ToJson(clientAppointment);
    this.RaiseCallback("INSRTAPT|" + args);
}
ASPxClientScheduler.prototype.UpdateAppointment = function(clientAppointment) {
    var args  = ASPx.Json.ToJson(clientAppointment);
    this.RaiseCallback("UPDTAPT|" + args);
}
ASPxClientScheduler.prototype.DeleteAppointment = function(clientAppointment) {
    var aptIdCollection = [ clientAppointment.appointmentId ];
    if (this.RaseAppointmentDeleting(aptIdCollection))
        return;
    var args = ASPx.Json.ToJson(clientAppointment);
    this.RaiseCallback("DLTAPT|" + args);
}
ASPxClientScheduler.prototype.GetVisibleIntervals = function() {
    var result = [];
    var count = this.visibleIntervals.length;
    for (var i = 0; i < count; i++) 
    	result.push(this.visibleIntervals[i].Clone());
    return result;
}
ASPxClientScheduler.prototype.ChangeToolTipContainer = function (container, attachEvents) {
    this.toolTipContainerChanger.ChangeContainer(container, attachEvents);
}
ASPxClientScheduler.prototype.ChangePopupMenuContainer = function (container, attachEvents) {
    this.menuContainerChanger.ChangeContainer(container, attachEvents);
}
ASPxClientScheduler.prototype.ChangeFormContainer = function (container, attachEvents) {
    this.formContainerChanger.ChangeContainer(container, attachEvents);
}
ASPxClientScheduler.prototype.AppointmentFormSave = function () {
    ASPx.AppointmentSave(this.name);
}
ASPxClientScheduler.prototype.AppointmentFormDelete = function () {
    ASPx.AppointmentDelete(this.name);
}
ASPxClientScheduler.prototype.AppointmentFormCancel = function () {
    ASPx.AppointmentCancel(this.name);
}
ASPxClientScheduler.prototype.GoToDateFormApply = function () {
    ASPx.GotoDateApply(this.name);
}
ASPxClientScheduler.prototype.GoToDateFormCancel = function () {
    ASPx.GotoDateCancel(this.name);
}
ASPxClientScheduler.prototype.InplaceEditFormSave = function () {
    ASPx.InplaceEditorSave(this.name);
}
ASPxClientScheduler.prototype.InplaceEditFormCancel = function () {
    ASPx.AppointmentCancel(this.name);
}
ASPxClientScheduler.prototype.InplaceEditFormShowMore = function () {
    ASPx.InplaceEditorEditForm(this.name);
}
ASPxClientScheduler.prototype.ReminderFormCancel = function () {
    ASPx.CancelRemindersForm(this.name);
}
ASPxClientScheduler.prototype.ReminderFormDismiss = function () {
    ASPx.DismissReminders(this.name);
}
ASPxClientScheduler.prototype.ReminderFormDismissAll = function () {
    ASPx.DismissAllReminders(this.name);
}
ASPxClientScheduler.prototype.ReminderFormSnooze = function () {
    ASPx.SnoozeReminders(this.name);
}

///////////////////////////////////// EVENTS ///////////////////////////////////////////
ASPxClientScheduler.prototype.RaiseActiveViewChanging = function(oldView, newView) {
    if (!this.ActiveViewChanging.IsEmpty()) {
        var args = new ActiveViewChangingEventArgs(oldView, newView);
        this.ActiveViewChanging.FireEvent(this, args);
        return !args.cancel;
    }
    else
        return true;
}
ASPxClientScheduler.prototype.RaiseActiveViewChanged = function() {
    if (!this.ActiveViewChanged.IsEmpty()) {
        var args = new ASPxClientEventArgs();
        this.ActiveViewChanged.FireEvent(this, args);
    }
}
ASPxClientScheduler.prototype.RaiseAppointmentClick = function(appointmentId, evt) {
    if (!this.AppointmentClick.IsEmpty()) {
        var args = new AppointmentClickEventArgs(appointmentId, evt);
        this.AppointmentClick.FireEvent(this, args);
        return args.handled;
    }
    return false;
}
ASPxClientScheduler.prototype.RaiseAppointmentDoubleClick = function(appointmentId, evt) {
    if (!this.AppointmentDoubleClick.IsEmpty()) {
        var args = new AppointmentClickEventArgs(appointmentId, evt);
        this.AppointmentDoubleClick.FireEvent(this, args);
        return args.handled;
    }
    return false;
}

ASPxClientScheduler.prototype.RaiseMouseUp = function () {
    if (!this.MouseUp.IsEmpty()) {
        var args = new ASPxClientEventArgs();
        this.MouseUp.FireEvent(this, args);        
    }
}
ASPxClientScheduler.prototype.RaiseAppointmentsSelectionChanged = function(selectedAppointmentIds) {
    if (!this.AppointmentsSelectionChanged.IsEmpty()) {
        var args = new AppointmentsSelectionEventArgs(selectedAppointmentIds);
        this.AppointmentsSelectionChanged.FireEvent(this, args);
    }
}
ASPxClientScheduler.prototype.RaiseSelectionChanged = function() {
    if (!this.SelectionChanged.IsEmpty()) {
        var args = new ASPxClientEventArgs();
        this.SelectionChanged.FireEvent(this, args);
    }
}
ASPxClientScheduler.prototype.RaiseSelectionChanging = function() {
    if (!this.SelectionChanging.IsEmpty()) {
        var args = new ASPxClientEventArgs();
        this.SelectionChanging.FireEvent(this, args);
    }
}
ASPxClientScheduler.prototype.RaiseVisibleIntervalChanged = function() {
    if (!this.VisibleIntervalChanged.IsEmpty()) {
        var args = new ASPxClientEventArgs();
        this.VisibleIntervalChanged.FireEvent(this, args);
    }
}
ASPxClientScheduler.prototype.RaiseMoreButtonClickedEvent = function(args) {
    if (!this.MoreButtonClicked.IsEmpty())
        this.MoreButtonClicked.FireEvent(this, args);
}
ASPxClientScheduler.prototype.RaiseMenuItemClicked = function(itemName) {
    if (!this.MenuItemClicked.IsEmpty()) {
        var args = new MenuItemClickedEventArgs(itemName);
        this.MenuItemClicked.FireEvent(this, args);
        return args.handled;
    }
    return false;
}
ASPxClientScheduler.prototype.RaiseAppointmentDrop = function(operation) {
    if (!this.AppointmentDrop.IsEmpty()) {
        var args = new ASPxClientAppointmentDragEventArgs(operation);
        this.AppointmentDrop.FireEvent(this, args);
        return args.handled;
    }
    return false;
}
ASPxClientScheduler.prototype.RaiseAppointmentResize = function(operation) {
    if (!this.AppointmentResize.IsEmpty()) {
        var args = new ASPxClientAppointmentResizeEventArgs(operation);
        this.AppointmentResize.FireEvent(this, args);
        return args.handled;
    }
    return false;
}
ASPxClientScheduler.prototype.RaseAppointmentDeleting = function(clientAppointments) {
    if (!this.AppointmentDeleting.IsEmpty()) {
        var args = new ASPxClientAppointmentDeletingEventArgs(clientAppointments);
        this.AppointmentDeleting.FireEvent(this, args);
        return args.cancel;
    }
    return false;
}
////////////////////////////////////////////////////////////////////////////////
var ASPxSchedulerViewType = ASPx.CreateClass(null, {
});
ASPxSchedulerViewType.Day = "Day";
ASPxSchedulerViewType.WorkWeek = "WorkWeek";
ASPxSchedulerViewType.Week = "Week";
ASPxSchedulerViewType.Month = "Month";
ASPxSchedulerViewType.Timeline = "Timeline";
ASPxSchedulerViewType.FullWeek = "FullWeek";
var ASPxSchedulerGroupType = ASPx.CreateClass(null, {
});
ASPxSchedulerGroupType.None = "None";
ASPxSchedulerGroupType.Date = "Date";
ASPxSchedulerGroupType.Resource = "Resource";


////////////////////////////////////////////////////////////////////////////////
// ASPxAppointmentType  (similar to AppointmentType enum declaration)
////////////////////////////////////////////////////////////////////////////////
var ASPxAppointmentType = ASPx.CreateClass(null, {
});
ASPxAppointmentType.Normal = "Normal";
ASPxAppointmentType.Pattern = "Pattern";
ASPxAppointmentType.Occurrence = "Occurrence",
ASPxAppointmentType.ChangedOccurrence = "ChangedOccurrence",
ASPxAppointmentType.DeletedOccurrence = "DeletedOccurrence";
var ASPxClientAppointmentDeletingEventArgs = ASPx.CreateClass(ASPxClientCancelEventArgs, {
    constructor: function(appointmentIds) {
        this.constructor.prototype.constructor.call(this);
        this.appointmentIds = appointmentIds;
    }
});
var AppointmentClickEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(appointmentId, sourceEvent) {
        this.appointmentId = appointmentId;
        this.htmlElement = sourceEvent;
        this.handled = false;
    }
});
var AppointmentsSelectionEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(appointmentIds){
        this.constructor.prototype.constructor.call(this);
        this.appointmentIds = appointmentIds;
    }
});
var ActiveViewChangingEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(oldView, newView){
        this.constructor.prototype.constructor.call(this);
        this.oldView = oldView;
        this.newView = newView;
        this.cancel = false;
    }
});
var MoreButtonClickedEventArgs = ASPx.CreateClass(ASPxClientProcessingModeEventArgs, {
    constructor: function(targetDateTime, interval, resource) {
        this.targetDateTime = targetDateTime;
        this.interval = interval;
        this.resource = resource;
        this.handled = false;
    }
});
var MenuItemClickedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(itemName) {
        this.itemName = itemName;
        this.handled = false;
    }
});
var ASPxClientAppointmentDragEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(operation) {
        this.handled = false;
        this.operation = operation;
    }
});
var ASPxClientAppointmentResizeEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(operation) {
        this.handled = false;
        this.operation = operation;
    }
});

window.ASPxClientSchedulerRefreshAction = ASPxClientSchedulerRefreshAction;
window.ASPxClientAppointmentOperation = ASPxClientAppointmentOperation;
window.ASPxSchedulerViewType = ASPxSchedulerViewType;
window.ASPxSchedulerGroupType = ASPxSchedulerGroupType;
window.ASPxAppointmentType = ASPxAppointmentType;
window.ASPxClientAppointmentDeletingEventArgs = ASPxClientAppointmentDeletingEventArgs;
window.AppointmentClickEventArgs = AppointmentClickEventArgs;
window.AppointmentsSelectionEventArgs = AppointmentsSelectionEventArgs;
window.ActiveViewChangingEventArgs = ActiveViewChangingEventArgs;
window.MoreButtonClickedEventArgs = MoreButtonClickedEventArgs;
window.MenuItemClickedEventArgs = MenuItemClickedEventArgs;
window.ASPxClientAppointmentDragEventArgs = ASPxClientAppointmentDragEventArgs;
window.ASPxClientAppointmentResizeEventArgs = ASPxClientAppointmentResizeEventArgs;
})();