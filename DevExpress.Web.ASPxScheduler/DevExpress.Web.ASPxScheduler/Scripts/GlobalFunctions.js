(function() {
///////// TODO  make helper classes ASPxSchedulerAptUtils.Delete()
var recycleDiv = document.createElement("DIV");

var SchedulerGlobals = {};
SchedulerGlobals.RemoveChildFromParent = function(parentNode, element) {		
	parentNode.removeChild(element);
	if(ASPx.Browser.IE) {		
		recycleDiv.appendChild(element);
		recycleDiv.innerHTML = "";
		element = null;
	}
	
}
SchedulerGlobals.RecycleNode = function(element) {
	var parentNode = element.parentNode;
	if(ASPx.IsExists(parentNode)) {
		parentNode.removeChild(element);
	}
	if(!ASPx.Browser.IE)
		return;
	recycleDiv.appendChild(element);
	recycleDiv.innerHTML = "";
	element = null;
}

////////////////////////// TO CLASSES ///////////////////////////////////////////
SchedulerGlobals.DateIncrease = function(date, spanInMilliseconds) {
    return new Date(date.valueOf() + spanInMilliseconds);        
}
SchedulerGlobals.DateIncreaseWithUtcOffset = function(date, spanInMilliseconds) {
    var result = SchedulerGlobals.DateIncrease(date, spanInMilliseconds);
    var utcDiff = (result.getTimezoneOffset() - date.getTimezoneOffset()) * 60000;
    return SchedulerGlobals.DateIncrease(result, utcDiff);
}
SchedulerGlobals.DateSubsWithTimezone = function(date1, date2) {
    //this function return the same result as .NET
    return date1 - date2 + (date2.getTimezoneOffset() - date1.getTimezoneOffset()) * 60000;
}
//TODO: test it!
SchedulerGlobals.DateTimeToMilliseconds = function(dateTime) {
    var result = dateTime.valueOf();
    result -= 60000 * dateTime.getTimezoneOffset();
    return result;
}
SchedulerGlobals.DateTimeMinValue = function(value1, value2) {
    return value1 < value2 ? value1 : value2;
}
SchedulerGlobals.DateTimeMaxValue = function(value1, value2) {
    return value1 > value2 ? value1 : value2;
}

SchedulerGlobals.GetOnMouseOutEventTarget = function(evt){
    evt = ASPx.Evt.GetEvent(evt);
    if(!ASPx.IsExists(evt)) return null; 
    return ASPx.Browser.IE ? evt.toElement : evt.relatedTarget;
}
SchedulerGlobals.SetTableCellOffsetWidth = function(tableCell, newOffsetWidth) {
    if (newOffsetWidth <=0 ) {
        return;
    }
    tableCell.style.width = newOffsetWidth + "px";
    SchedulerGlobals.RefreshTableCell(tableCell);// fix opera bug 
    var delta = tableCell.clientWidth - newOffsetWidth;
    var correctedWidth = newOffsetWidth - delta;
    if (correctedWidth > 0 ) 
        tableCell.style.width = correctedWidth + "px";
    else
        tableCell.style.width = "0.1px";
    SchedulerGlobals.RefreshTableCell(tableCell);// fix opera bug 
}
SchedulerGlobals.RefreshTableCell = function(tableCell) {
    if (ASPx.Browser.Opera) { // fix opera bug 
        tableCell.style.display = "none";
        tableCell.style.display = "table-cell";
    }
}
SchedulerGlobals.ChangeTableLayout = function(table, layout) {
        if (ASPx.Browser.IE) {
            table.style.tableLayout = layout;
            return;
        }    
        var parent = table.parentNode;
        var nextSibling = table.nextSibling;
        parent.removeChild(table);
        table.style.tableLayout = layout;
        if(ASPx.IsExists(nextSibling))        
            parent.insertBefore(table, nextSibling);
        else
            parent.appendChild(table);
}
SchedulerGlobals.GetItemByLocation = function(parentTable, location) {
    return parentTable.rows[location[0]].cells[location[1]];
}

////////////////////////// KEYMOUSE//////////////////////////////////////////////
ASPx.SchedulerNavBtnClick = function(name, startTime, duration, resourceId) {
    var scheduler = ASPx.GetControlCollection().Get(name);
    if(scheduler != null)
        scheduler.NavBtnClick(startTime, duration, resourceId);
}
ASPx.SchedulerMainDivMouseDown = function(name, evt) {
    var scheduler = ASPx.GetControlCollection().Get(name);
    if(scheduler != null)
        scheduler.MainDivMouseDown(evt);
}
ASPx.SchedulerMainDivMouseUp = function(name, evt) {
    var scheduler = ASPx.GetControlCollection().Get(name);
    if (scheduler != null)
        scheduler.MainDivMouseUp(evt);
}
ASPx.SchedulerMainDivMouseClick = function(name, evt)  {
    var scheduler = ASPx.GetControlCollection().Get(name);
    if(scheduler != null) {
        scheduler.MainDivMouseClick(evt);
    }
}
ASPx.SchedulerMainDivMouseDoubleClick = function(name, evt)  {
    var scheduler = ASPx.GetControlCollection().Get(name);
    if(scheduler != null) { 
        scheduler.MainDivMouseDoubleClick(evt);
    }
}


ASPx.SchedulerFuncCallbackHandler = function(name, result) {
    var scheduler = ASPx.GetControlCollection().Get(name);
    if(scheduler != null)
        scheduler.OnFuncCallback(unescape(result));
}
ASPx.SchedulerEmptyFuncCallbackHandler = function() {
    //Do nothing
}



////////////////////////// APPOINTMENT ////////////////////////////////////////////////
ASPx.AppointmentSave = function(controlId) {
    var control = ASPx.GetControlCollection().Get(controlId);
    if (ASPx.IsExists(control)) {
        //control.HideCurrentPopupContainer();
        if (ASPxClientEdit.ValidateEditorsInContainerById(controlId + "_formBlock_innerContent"))
            control.RaiseCallback("APTSAVE|");
    }
}
ASPx.AppointmentCancel = function(controlId) {
    var control = ASPx.GetControlCollection().Get(controlId);
    if (ASPx.IsExists(control)) {
        control.CancelFormChangesAndClose(control.aptFormVisibility, "APTCANCEL|");
    }
}
ASPx.AppointmentDelete = function(controlId) {
    var control = ASPx.GetControlCollection().Get(controlId);
    if (ASPx.IsExists(control)) {
        var aptId = aspxGetCoreAppointmentId(control.stateObject.editableAppointment);
        var aptIdCollection = [ aptId ];
        if (control.RaseAppointmentDeleting(aptIdCollection)) {
            ASPx.AppointmentCancel(controlId);
            return;
        }
        control.RaiseCallback("APTDEL|");
    }
}
function aspxGetCoreAppointmentId(aptId) {
    var result = aptId + "";
    var indxOfEndAptType = result.indexOf("|");
    var count = result.length;
    if (indxOfEndAptType > 0) 
        result = result.substr(indxOfEndAptType + 1, count);
    return result;
}
ASPx.InplaceEditorSave = function(controlId) {
    var control = ASPx.GetControlCollection().Get(controlId);
    if (ASPx.IsExists(control)) {
        control.RaiseCallback("INPLACESAVE|");
    }
}
ASPx.InplaceEditorEditForm = function(controlId) {
    var control = ASPx.GetControlCollection().Get(controlId);
    if (ASPx.IsExists(control)) {
        control.RaiseCallback("INPLACEFORM|");
    }
}

ASPx.ShowInplacePopupWindow = function(name, popupId, aptId) {
    var control = ASPx.GetControlCollection().Get(name);
    if (ASPx.IsExists(control)) {
        control.SaveCurrentPopupContainer(popupId);
        control.ShowInplacePopupWindow(popupId, aptId);
    }
}
ASPx.ShowFormPopupWindow = function(name, popupId) {
    var control = ASPx.GetControlCollection().Get(name);
    var mainElement = control.GetMainElement();
    if ((!ASPx.IsElementVisible(mainElement) && control.visibility != ASPx.SchedulerFormVisibility.FillControlArea) || control.formContainerChanger.IsActive()) {
        control.SaveCurrentPopupContainer(popupId);
        control.ShowFormPopupWindowDeferred(popupId);
        return;
    }
    
    if (ASPx.IsExists(control)) {
        control.SaveCurrentPopupContainer(popupId);
        control.ShowFormPopupWindow(popupId);
    }
}
ASPx.GotoDateApply = function(controlId) {
    var control = ASPx.GetControlCollection().Get(controlId);
    if (ASPx.IsExists(control)) {
        var activeViewType = control.GetActiveViewType();
        if (!control.RaiseActiveViewChanging(activeViewType))
            return;
        control.RaiseCallback("GOTODATEFORM|");
    }
}
ASPx.GotoDateCancel = function(controlId) {
    var control = ASPx.GetControlCollection().Get(controlId);
    if (ASPx.IsExists(control)) {
        control.CancelFormChangesAndClose(control.gotoDateFormVisibility, "GOTODATEFORMCANCEL|");
        //control.HideCurrentPopupContainer();
        //control.RaiseCallback("FORMCLOSE|");
    }
}
ASPx.RecurrentAptDeleteCancel = function(controlId) {
    var control = ASPx.GetControlCollection().Get(controlId);
    if (ASPx.IsExists(control)) 
        control.CancelFormChangesAndClose(control.recurrentAppointmentDeleteFormVisibility, "RECURAPTDELETECANCEL|");
}
ASPx.RecurrentAptDelete = function(controlId) {
    var control = ASPx.GetControlCollection().Get(controlId);
    if (ASPx.IsExists(control)) 
           return control.RaiseCallback("RECURAPTDELETE|");
}
ASPx.RecurrentAptEditCancel = function(controlId) {
    var control = ASPx.GetControlCollection().Get(controlId);
    if (ASPx.IsExists(control)) 
        control.CancelFormChangesAndClose(control.recurrentAppointmentDeleteFormVisibility, "RECURAPTEDITCANCEL|");
}
ASPx.RecurrentAptEdit = function(controlId) {
    var control = ASPx.GetControlCollection().Get(controlId);
    if (ASPx.IsExists(control)) 
           return control.RaiseCallback("RECURAPTEDIT|");
}

//todo: test it
ASPx.SchedulerDateTimeIndexComparer = function(array, index, dateTime2) {
    return ASPx.SchedulerDateTimeComparer(array[index], dateTime2);
}
ASPx.SchedulerDateTimeComparer = function(dateTime1, dateTime2) {
    var dif = dateTime2 - dateTime1;
    if (dif == 0)
        return 0;
    return dif < 0 ? 1 : -1;
}
////////////////////////// MOREBUTTONS //////////////////////////
//TODO: test it!
ASPx.MoreButtonClickEvent = function(evt) {    
    function isMoreButtonDiv(element) {
        return ASPx.IsExists(element.isMoreButton) ? element.isMoreButton : false;
    }
    var srcMoreButtonDiv = ASPx.GetParent(ASPx.Evt.GetEventSource(evt), isMoreButtonDiv);
    var control = srcMoreButtonDiv.schedulerControl;
    var args = new MoreButtonClickedEventArgs(srcMoreButtonDiv.targetDateTime, srcMoreButtonDiv.interval, srcMoreButtonDiv.resource);
    control.RaiseMoreButtonClickedEvent(args);
    var activeViewType = control.GetActiveViewType();
    if (!control.RaiseActiveViewChanging(activeViewType, ASPxSchedulerViewType.Day))
        return;
    if (args.handled)
        return;       
    var intervalArgs = ASPx.SchedulerGlobals.DateTimeToMilliseconds(srcMoreButtonDiv.interval.GetStart()) + "," + srcMoreButtonDiv.interval.GetDuration();
    var targetDateTimeArg = ASPx.SchedulerGlobals.DateTimeToMilliseconds(srcMoreButtonDiv.targetDateTime);
    control.RaiseCallback("MOREBTN|" + targetDateTimeArg + "," +  intervalArgs + "," + srcMoreButtonDiv.resource);
}

////////////////////////// REMINDERS ///////////////////////////

ASPx.CancelRemindersForm = function(controlId) {
    var control = ASPx.GetControlCollection().Get(controlId);
    if (ASPx.IsExists(control)) 
        control.RaiseCallback("CLSREMINDERSFRM|");
}
ASPx.DismissReminders = function(controlId) {
    var control = ASPx.GetControlCollection().Get(controlId);
    if (ASPx.IsExists(control)) 
           return control.RaiseCallback("DSMSREMINDER|");
}
ASPx.DismissAllReminders = function(controlId) {
    var control = ASPx.GetControlCollection().Get(controlId);
    if (ASPx.IsExists(control)) 
           return control.RaiseCallback("DSMSALLREMINDERS|");
}
ASPx.SnoozeReminders = function(controlId) {
    var control = ASPx.GetControlCollection().Get(controlId);
    if (ASPx.IsExists(control)) 
           return control.RaiseCallback("SNZREMINDERS|");
}

ASPx.SchedulerGlobals = SchedulerGlobals;
})();