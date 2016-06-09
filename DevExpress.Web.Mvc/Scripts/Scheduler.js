

(function() {
var MVCxClientScheduler = ASPx.CreateClass(ASPxClientScheduler, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.start = null;
        this.callbackUrl = "";
        this.customActionUrl = "";
        this.editAppointmentsUrl = "";
        this.appointmentFormUrl = "";
        this.appointmentInplaceEditorFormUrl = "";
        this.gotoDateFormUrl = "";
        this.recurrentAppointmentDeleteFormUrl = "";
        this.remindersFormUrl = "";
        this.callbackCustomArgs = {};
        this.editableAppoinmentEditors = null;
        this.recurrenceFormName = "";
        this.resourceSharing = false;
        this.ToolTipDisplaying = new ASPxClientEvent();
    },
    PerformCallback: function(data) {
        MVCx.MergeHashTables(this.callbackCustomArgs, data);
        ASPxClientScheduler.prototype.PerformCallback.call(this, ASPx.Json.ToJson(data));
    },
    InlineInitialize: function(){
        if(this.callbackUrl != "")
            this.callBack = function (arg) { MVCx.PerformControlCallback(this.name, this.callbackUrl, arg, this.GetCallbackParams(), this.callbackCustomArgs) };
        if(this.customActionUrl != "")
            this.customActionCallBack = function(arg) { MVCx.PerformControlCallback(this.name, this.customActionUrl, arg, this.GetCallbackParams(), this.callbackCustomArgs) }; 
        if(this.editAppointmentsUrl != "")
            this.editAppointmentsCallBack = function(arg){ MVCx.PerformControlCallback(this.name, this.editAppointmentsUrl, arg, this.GetCallbackParams(), this.callbackCustomArgs) }; 
        if(this.appointmentFormUrl != "")
            this.appointmentFormCallBack = function(arg){ MVCx.PerformControlCallback(this.name, this.appointmentFormUrl, arg, this.GetCallbackParams(), this.callbackCustomArgs) }; 
        if(this.appointmentInplaceEditorFormUrl != "")
            this.appointmentInplaceEditorFormCallback = function(arg){ MVCx.PerformControlCallback(this.name, this.appointmentInplaceEditorFormUrl, arg, this.GetCallbackParams(), this.callbackCustomArgs) }; 
        if(this.gotoDateFormUrl != "")
            this.gotoDateFormUrl = function(arg){ MVCx.PerformControlCallback(this.name, this.gotoDateFormUrl, arg, this.GetCallbackParams(), this.callbackCustomArgs) };
        if(this.recurrentAppointmentDeleteFormUrl != "")
            this.recurrentAppointmentDeleteFormUrl = function(arg){ MVCx.PerformControlCallback(this.name, this.recurrentAppointmentDeleteFormUrl, arg, this.GetCallbackParams(), this.callbackCustomArgs) };
        
        ASPxClientScheduler.prototype.InlineInitialize.call(this);
    },
    RaiseBeginCallbackInternal: function(command) {
        var args = new MVCxClientBeginCallbackEventArgs(command);
        if(!this.BeginCallback.IsEmpty())
            this.BeginCallback.FireEvent(this, args);
        MVCxClientGlobalEvents.OnBeginCallback(args);
        MVCx.MergeHashTables(this.callbackCustomArgs, args.customArgs);
    },
    RaiseEndCallback: function() {
        ASPxClientScheduler.prototype.RaiseEndCallback.call(this);
        MVCxClientGlobalEvents.OnEndCallback();
    },
    RaiseCallbackError: function(message) {
        var result = ASPxClientScheduler.prototype.RaiseCallbackError.call(this, message);
        if(!result.isHandled) {
            var args = new ASPxClientCallbackErrorEventArgs(message);
            MVCxClientGlobalEvents.OnCallbackError(args);
            result = { isHandled: args.handled, errorMessage: args.message };
        }
        return result;
    },
    CreateCallbackByInfo: function(arg, command, callbackInfo) {
        this.CreateCallbackInternal(arg, command, true, callbackInfo);
    },
    GetCallbackParams: function() {
        var params = {};
        MVCx.AddCallbackParam(params, this.GetStateHiddenField());
        MVCx.AddCallbackParamsInContainer(params, this.GetMainElement());
        this.FillParamsForDateNavigator(params);
        this.FillParamsForForm(params);
        params[this.name + "_start"] = ASPx.Json.ToJson(this.start);
        params[this.name + "_activeViewType"] = this.GetActiveViewType();
        return params;
    },
    FillParamsForDateNavigator: function(params) {
        if(!this.dateNavigatorId) return;
        var dateNavigator = ASPx.GetControlCollection().Get(this.dateNavigatorId);
        if(dateNavigator && dateNavigator.GetMainElement)
            MVCx.AddCallbackParamsInContainer(params, dateNavigator.GetMainElement());
    },
    FillParamsForForm: function(params) {
        if(!this.currentPopupContainer) return;

        var formValues = this.activeFormType == "Appointment" && this.editableAppoinmentEditors ? this.GetParamsForAppointmentForm() : this.GetParamsForStandardForm();
        if (this.IsVisibleAppointmentRecurrenceForm())
            formValues["RecurrenceFormValue"] = ASPx.GetControlCollection().Get(this.recurrenceFormName).GetValue();

        params["DXMVCSchedulerAptFormValues"] = ASPx.Json.ToJson(formValues);
        params[this.name + "_enabledAppointmentFormTemplateControl"] = !!this.editableAppoinmentEditors;
        params[this.name + "_resourceSharing"] = this.resourceSharing;
    },
    GetParamsForStandardForm: function() {
        function addValuesToPostData(name, value, formPostData){
            var lastSeparatorIndex = name.lastIndexOf("_");
            var editorName = lastSeparatorIndex > -1 ? name.substr(lastSeparatorIndex + 1) : name;
            if(!ASPx.IsExists(formPostData[editorName]))
                formPostData[editorName] = value;
        };
        var formValues = {};
        var formContainer = this.currentPopupContainer.GetContentContainer(-1);
        ASPx.GetControlCollection().ProcessControlsInContainer(formContainer, function(control) {
            if(!control.GetValue || !((ASPx.GetElementVisibility(control.GetMainElement()) && !ASPx.IsElementVisible(control.GetMainElement())) || ASPx.IsElementVisible(control.GetMainElement())))
                return;
            addValuesToPostData(control.name, MVCx.GetEditorValueByControl(control), formValues);
        });
        $(formContainer).children("input:hidden").each(function(){
            addValuesToPostData(this.id, this.value, formValues);
        });
        return formValues;
    },
    GetParamsForAppointmentForm: function() {
        var formValues = {};
        if (this.editableAppoinmentEditors) {
            for(var index in this.editableAppoinmentEditors){
                var fieldName = this.editableAppoinmentEditors[index];
                var control = ASPx.GetControlCollection().Get(fieldName);
                if (control && control.GetValue)
                    formValues[fieldName] = MVCx.GetEditorValueByControl(control);
                else if($("#" + fieldName).length)
                    formValues[fieldName] = $("#" + fieldName).val();
            }
        };
        return formValues;
    },
    IsVisibleAppointmentRecurrenceForm: function(){
        return this.activeFormType == "Appointment" && this.recurrenceFormName && ASPx.GetElementById(this.recurrenceFormName + "_mainDiv");
    },
    CreateClientAppointmentDragHelper: function(hitTestResult, appointmentClickHandler, e) {
        return new MVCxClientAppointmentDragHelper(this, hitTestResult.appointmentDiv, hitTestResult.cell, e, appointmentClickHandler, this);
    },
    CreateCallbackCore: function (arg, command, callbackID) {
        if(this.callbackCustomArgs != {})
            window.setTimeout(function () { this.callbackCustomArgs = {}; } .aspxBind(this), 0);
        ASPxClientScheduler.prototype.CreateCallbackCore.call(this, arg, this.GetCommandName(arg), callbackID);
    },
    GetCallbackMethod: function(command) {
        switch(command) {
            case "CUSTOMCALLBACK":
                return MVCx.GetCustomActionCallBackMethod(this);
            case "INPLACESAVE":
                return this.appointmentInplaceEditorFormCallback || this.editAppointmentsCallBack;
            case "APTDEL": case "APTSAVE":
                return this.appointmentFormCallBack || this.editAppointmentsCallBack;
            case "APTSCHANGE": case "LabelSubMenu": case "StatusSubMenu": case "RestoreOccurrence":
                return this.editAppointmentsCallBack;
            case "RECURAPTDELETE":
                return this.recurrentAppointmentDeleteFormUrl || this.editAppointmentsCallBack;
            case "DeleteAppointment":
                return this.editAppointmentsCallBack;
            case "INSRTAPT": case "UPDTAPT": case "DLTAPT":
                return this.editAppointmentsCallBack;
            case "GOTODATEFORM":
                return this.gotoDateFormUrl || this.callBack;
            case "SNZREMINDERS": case "DSMSREMINDER": case "DSMSALLREMINDERS": case "CLSREMINDERSFRM":
                return this.remindersFormUrl || this.editAppointmentsCallBack;
        }
        return this.callBack;
    },
    GetCommandName: function(arg){
        var data = arg.split("|");
        return data[0] == "MNUAPT" ? data[1].split("!")[0] : data[0];
    },
    DoCallback: function(response) {
        ASPxClientScheduler.prototype.DoCallback.call(this, response);

        if(this.editableAppoinmentEditors)
            this.editableAppoinmentEditors = eval(this.editableAppoinmentEditors);
    },
    RaiseToolTipDisplaying: function(arg) {
        if(!this.ToolTipDisplaying.IsEmpty())
            this.ToolTipDisplaying.FireEvent(this, arg);
    }
});
var MVCxClientSchedulerTemplateToolTip = ASPx.CreateClass(ASPxClientToolTipBase, {
    constructor: function(type) {
        this.constructor.prototype.constructor.call(this);
        this.type = type;
    },
    Update: function(toolTipData) {
        var args = new MVCxClientSchedulerToolTipDisplayingEventArgs(this, toolTipData);
        this.scheduler.RaiseToolTipDisplaying(args);
    }
});
var MVCxClientSchedulerAppointmentDragTemplateToolTip = ASPx.CreateClass(MVCxClientSchedulerTemplateToolTip, {
    constructor: function() {
        this.constructor.prototype.constructor.call(this, MVCxSchedulerToolTipType.AppointmentDrag);
        this.resetPositionByTimer = true;
    },
    CalculatePosition: function(bounds) {
        return new ASPxClientPoint(bounds.GetLeft(), bounds.GetTop() - bounds.GetHeight());
    }
});
var MVCxClientSchedulerToolTipDisplayingEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(toolTip, data) {
        this.toolTip = toolTip;
        this.data = data;
    }
});
var MVCxSchedulerToolTipType = {
    Appointment: 0,
    AppointmentDrag: 1,
    Selection: 2
};

var MVCxClientTimeZoneEdit = ASPx.CreateClass(ASPxClientTimeZoneEdit, {
    Initialize: function() {
        this.constructor.prototype.Initialize.call(this);
        MVCxClientGlobalEvents.ControlsInitialized.AddHandler(this.OnControlsInitialized, this);
    },
    OnControlsInitialized: function(s, e) {
        this.InitializeTimeZoneId();
        MVCxClientGlobalEvents.ControlsInitialized.RemoveHandler(this.OnControlsInitialized);
    },
    GetComboBoxBox: function() { return ASPx.GetControlCollection().Get(this.name + "_cmd"); },
    InitializeTimeZoneId: function() {
        var schedulerControl = ASPx.GetControlCollection().Get(this.schedulerControlId);
        var comboBox = this.GetComboBoxBox();
        if(schedulerControl && comboBox)
            comboBox.SetValue(schedulerControl.initClientTimeZoneId);
    }
});
MVCxClientScheduler.Cast = ASPxClientControl.Cast;

window.MVCxClientSchedulerTemplateToolTip = MVCxClientSchedulerTemplateToolTip;
window.MVCxClientSchedulerAppointmentDragTemplateToolTip = MVCxClientSchedulerAppointmentDragTemplateToolTip;
window.MVCxClientSchedulerToolTipDisplayingEventArgs = MVCxClientSchedulerToolTipDisplayingEventArgs;
window.MVCxSchedulerToolTipType = MVCxSchedulerToolTipType;
window.MVCxClientScheduler = MVCxClientScheduler;
MVCx.ClientTimeZoneEdit = MVCxClientTimeZoneEdit;
})();