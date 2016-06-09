

(function () {
    ////////////////////////////////////////////////////////////////////////////////
    /////////////////////// ASPxClientWeekDaysCheckEdit ////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////
    var ASPxClientWeekDaysCheckEdit = ASPx.CreateClass(ASPxClientControl, {
        constructor: function (name) {
            this.constructor.prototype.constructor.call(this, name);
        },
        Initialize: function () {
            this.constructor.prototype.Initialize.call(this);
            for (var day in this.itemsControl) {
                var checkEdit = this.itemsControl[day];
                checkEdit.CheckedChanged.AddHandler(ASPx.CreateDelegate(this.OnChkCheckedChanged, this));
            }
        },
        GetValue: function () {
            var result = 0;
            for (var day in this.itemsControl) {
                var checkEdit = this.itemsControl[day];
                if (checkEdit.GetChecked())
                    result |= day;
            }
            return result;
        },
        SetValue: function (value) {
            for (var day in this.itemsControl) {
                var checkEdit = this.itemsControl[day];
                checkEdit.SetChecked((value & day) == day);
            }
        },
        GetMainElement: function () {
            if (!ASPx.IsExistsElement(this.mainElement))
                this.mainElement = ASPx.GetElementById(this.name + "_Items");
            return this.mainElement;
        },
        OnChkCheckedChanged: function (s, e) {
            var result = this.GetValue();
            if (result != 0)
                return;
            s.SetChecked(true);
        }
    });
    ////////////////////////////////////////////////////////////////////////////////
    //////////////////// ASPxClientRecurrenceRangeControl //////////////////////////
    ////////////////////////////////////////////////////////////////////////////////
    var ASPxClientRecurrenceRangeControl = ASPx.CreateClass(ASPxClientControl, {
        constructor: function (name) {
            this.constructor.prototype.constructor.call(this, name);
            this.ValidationCompleted = new ASPxClientEvent();
        },
        GetMainElement: function () {
            if (!ASPx.IsExistsElement(this.mainElement))
                this.mainElement = ASPx.GetElementById(this.name + "_mainDiv");
            return this.mainElement;
        },
        UpdateControl: function (s, e) {
            var range = this.GetRange();
            this.UpdateChildControlsControls(range);
        },
        SubscribeEvents: function () {
            this.rbEndAfterNumberOfOccurrences.CheckedChanged.AddHandler(ASPx.CreateDelegate(this.OnCheckedChanged, this));
            this.rbEndByDate.CheckedChanged.AddHandler(ASPx.CreateDelegate(this.OnCheckedChanged, this));
            this.rbNoEndDate.CheckedChanged.AddHandler(ASPx.CreateDelegate(this.OnCheckedChanged, this));
            this.deRangeEnd.ValueChanged.AddHandler(ASPx.CreateDelegate(this.OnDeRangeEndValueChanged, this));
            this.deRangeEnd.Validation.AddHandler(ASPx.CreateDelegate(this.OnDeRangeEndValidation, this));
        },
        OnDeRangeEndValueChanged: function (s, e) {
            this.ValidateEndDate();
        },
        ValidateEndDate: function () {
            var isValid = this.IsValid();
            this.deRangeEnd.SetIsValid(isValid);
            this.RaiseValidationCompleted(isValid);
        },
        RaiseValidationCompleted: function (isValid) {
            if (!this.ValidationCompleted.IsEmpty()) {
                var args = new ASPxClientSchedulerValidationCompletedArgs(isValid);
                this.ValidationCompleted.FireEvent(this, args);
            }
        },
        OnDeRangeEndValidation: function (s, e) {
            var isValid = this.IsValid();
            this.deRangeEnd.SetIsValid(isValid);
            this.RaiseValidationCompleted(isValid);
        },
        OnCheckedChanged: function (s, e) {
            var range = this.GetRange();
            this.UpdateChildControlsControls(range);
        },
        UpdateChildControlsControls: function (range) {
            if (range == ASPxClientRecurrenceRange.NoEndDate) {
                this.deRangeEnd.SetEnabled(false);
                this.spinRangeOccurrencesCount.SetEnabled(false);
            }
            else if (range == ASPxClientRecurrenceRange.OccurrenceCount) {
                this.deRangeEnd.SetEnabled(false);
                this.spinRangeOccurrencesCount.SetEnabled(true);
            }
            else if (range == ASPxClientRecurrenceRange.EndByDate) {
                this.deRangeEnd.SetEnabled(true);
                this.spinRangeOccurrencesCount.SetEnabled(false);
                this.EnsureEndDateValid();
            }
            this.ValidateEndDate();
        },
        IsValid: function () {
            if (!this.start)
                return true;
            var range = this.GetRange();
            if (range != ASPxClientRecurrenceRange.EndByDate)
                return true;
            var endDate = this.deRangeEnd.GetDate();
            return endDate > this.start;
        },
        SetStart: function (start) {
            if (!start)
                return;
            this.start = start;
            this.EnsureEndDateValid();
        },
        EnsureEndDateValid: function () {
            if (!this.start)
                return;
            if (!this.IsValid()) {
                var newStart = ASPxSchedulerDateTimeHelper.AddDays(this.start, 1);
                this.deRangeEnd.SetDate(newStart);
            }
        },
        GetRange: function () {
            if (this.rbNoEndDate.GetChecked())
                return ASPxClientRecurrenceRange.NoEndDate;
            if (this.rbEndAfterNumberOfOccurrences.GetChecked())
                return ASPxClientRecurrenceRange.OccurrenceCount;
            if (this.rbEndByDate.GetChecked())
                return ASPxClientRecurrenceRange.EndByDate;
            return ASPxClientRecurrenceRange.NoEndDate;
        },
        GetOccurrenceCount: function () {
            return this.spinRangeOccurrencesCount.GetValue();
        },
        GetEndDate: function () {
            return this.deRangeEnd.GetValue();
        },
        SetRange: function (range) {
            switch (range) {
                case ASPxClientRecurrenceRange.NoEndDate:
                    this.rbNoEndDate.SetChecked(true);
                    break;
                case ASPxClientRecurrenceRange.OccurrenceCount:
                    this.rbEndAfterNumberOfOccurrences.SetChecked(true);
                    break;
                case ASPxClientRecurrenceRange.EndByDate:
                    this.rbEndByDate.SetChecked(true);
                    break;
            }
            this.UpdateChildControlsControls(range);
        },
        SetOccurrenceCount: function (occurrenceCount) {
            this.spinRangeOccurrencesCount.SetValue(occurrenceCount);
        },
        SetEndDate: function (date) {
            this.deRangeEnd.SetValue(date);
        },
        AfterInitialize: function () {
            this.constructor.prototype.AfterInitialize.call(this);
            this.UpdateControl();
        },
        GetValue: function () {
            var value = {};
            value["Range"] = this.GetRange();
            value["OccurrenceCount"] = this.GetOccurrenceCount();
            value["EndDate"] = this.GetEndDate();
            return value;
        }
    });
    ///////////////////////////////////////////////////////////////////////////////
    ///////////////////// ASPxClientRecurrenceControlBase /////////////////////////
    ///////////////////////////////////////////////////////////////////////////////
    var ASPxClientRecurrenceControlBase = ASPx.CreateClass(ASPxClientControl, {
        constructor: function (name) {
            this.constructor.prototype.constructor.call(this, name);
            this.valueAccessor = this.CreateValueAccessor(this);
        },
        Initialize: function () {
            this.constructor.prototype.Initialize.call(this);
        },
        GetMainElement: function () {
            if (!ASPx.IsExistsElement(this.mainElement))
                this.mainElement = ASPx.GetElementById(this.name + "_mainDiv");
            return this.mainElement;
        },
        CreateValueAccessor: function (recurrenceControl) {
            return new DefaultRecurrenceRuleValuesAccessor(recurrenceControl);
        },
        Update: function (recurrenceInfo) {
            //do nothing
        }
    });

    var ASPxClientHourlyRecurrenceControl = ASPx.CreateClass(ASPxClientRecurrenceControlBase, {
        constructor: function (name) {
            this.constructor.prototype.constructor.call(this, name);
        },
        Initialize: function () {
            this.constructor.prototype.Initialize.call(this);
        },
        CreateValueAccessor: function (recurrenceControl) {
            return new HourlyRecurrenceValuesAccessor(recurrenceControl);
        },
        Update: function (recurrenceInfo) {
            this.SpinHourCount.SetValue(recurrenceInfo.periodicity);
        },
        GetValue: function () {
            value = {};
            value["Periodicity"] = this.valueAccessor.GetPeriodicity();
            return value;
        }
    });

    ///////////////////////////////////////////////////////////////////////////////
    //////////////////// ASPxClientDailyRecurrenceControl /////////////////////////
    ///////////////////////////////////////////////////////////////////////////////
    var ASPxClientDailyRecurrenceControl = ASPx.CreateClass(ASPxClientRecurrenceControlBase, {
        constructor: function (name) {
            this.constructor.prototype.constructor.call(this, name);
        },
        Initialize: function () {
            this.constructor.prototype.Initialize.call(this);
        },
        CreateValueAccessor: function (recurrenceControl) {
            return new DailyRecurrenceValuesAccessor(recurrenceControl);
        },
        Update: function (recurrenceInfo) {
            this.SpinDailyDaysCount.SetValue(recurrenceInfo.periodicity);
            if ((recurrenceInfo.weekDays & ASPxClientWeekDays.EveryDay) == ASPxClientWeekDays.EveryDay)
                this.RbDay.SetChecked(true);
            else if ((recurrenceInfo.weekDays & ASPxClientWeekDays.WorkDays) == ASPxClientWeekDays.WorkDays)
                this.RbEveryWeekDay.SetChecked(true);
        },
        GetValue: function () {
            value = {};
            value["Periodicity"] = this.valueAccessor.GetPeriodicity();
            value["WeekDays"] = this.valueAccessor.GetWeekDays();
            return value;
        }
    });
    ////////////////////////////////////////////////////////////////////////////////
    //////////////////// ASPxClientWeeklyRecurrenceControl /////////////////////////
    ////////////////////////////////////////////////////////////////////////////////
    var ASPxClientWeeklyRecurrenceControl = ASPx.CreateClass(ASPxClientRecurrenceControlBase, {
        constructor: function (name) {
            this.constructor.prototype.constructor.call(this, name);
        },
        Initialize: function () {
            this.constructor.prototype.Initialize.call(this);
        },
        CreateValueAccessor: function (recurrenceControl) {
            return new WeeklyRecurrenceValuesAccessor(recurrenceControl);
        },
        Update: function (recurrenceInfo) {
            this.SpinWeeklyWeeksCount.SetValue(recurrenceInfo.periodicity);
            this.WeekDaysCheckEdit.SetValue(recurrenceInfo.weekDays);
        },
        GetValue: function () {
            value = {};
            value["Periodicity"] = this.valueAccessor.GetPeriodicity();
            value["WeekDays"] = this.valueAccessor.GetWeekDays();
            return value;
        }
    });
    ////////////////////////////////////////////////////////////////////////////////
    //////////////////// ASPxClientMonthlyRecurrenceControl /////////////////////////
    ////////////////////////////////////////////////////////////////////////////////
    var ASPxClientMonthlyRecurrenceControl = ASPx.CreateClass(ASPxClientRecurrenceControlBase, {
        constructor: function (name) {
            this.constructor.prototype.constructor.call(this, name);
        },
        Initialize: function () {
            this.constructor.prototype.Initialize.call(this);
        },
        CreateValueAccessor: function (recurrenceControl) {
            return new MonthlyRecurrenceValuesAccessor(recurrenceControl);
        },
        Update: function (recurrenceInfo) {
            this.spinMonthlyDayMonthCount.SetValue(recurrenceInfo.periodicity);
            this.spinMonthlyWeekDaysMonthCount.SetValue(recurrenceInfo.periodicity);
            this.wdeMonthlyWeekDays.SetValue(recurrenceInfo.weekDays);
            this.rbDay.SetChecked(false);
            this.rbWeekDays.SetChecked(false);
            var start = (recurrenceInfo.interval) ? recurrenceInfo.interval.start : new Date();
            if (recurrenceInfo.weekOfMonth == ASPxClientWeekOfMonth.None) {
                this.rbDay.SetChecked(true);
                var weekOfMonth = RecurrenceFormHelper.CalcWeekOfMonth(start, recurrenceInfo.dayOfWeek);
                this.wmeMonthlyWeekOfMonth.SetValue(weekOfMonth);
                this.spinMonthlyDay.SetValue(recurrenceInfo.DayNumber);
            }
            else {
                this.rbWeekDays.SetChecked(true);
                this.wmeMonthlyWeekOfMonth.SetValue(recurrenceInfo.weekOfMonth);
                this.spinMonthlyDay.SetValue(start.getDate());
            }
        },
        GetValue: function () {
            var value = {};
            value["DayNumber"] = this.valueAccessor.GetDayNumber();
            value["Periodicity"] = this.valueAccessor.GetPeriodicity();
            value["WeekDays"] = this.valueAccessor.GetWeekDays();
            value["WeekOfMonth"] = this.valueAccessor.GetWeekOfMonth();
            return value;
        }
    });
    ////////////////////////////////////////////////////////////////////////////////
    //////////////////// ASPxClientYearlyRecurrenceControl /////////////////////////
    ////////////////////////////////////////////////////////////////////////////////
    var ASPxClientYearlyRecurrenceControl = ASPx.CreateClass(ASPxClientRecurrenceControlBase, {
        constructor: function (name) {
            this.constructor.prototype.constructor.call(this, name);
        },
        Initialize: function () {
            this.constructor.prototype.Initialize.call(this);
        },
        CreateValueAccessor: function (recurrenceControl) {
            return new YearlyRecurrenceValuesAccessor(recurrenceControl);
        },
        Update: function (recurrenceInfo) {
            this.meYearlyDayMonth.SetValue(recurrenceInfo.month);
            this.meYearlyWeekDaysMonth.SetValue(recurrenceInfo.month);
            this.wdeYearlyWeekDays.SetValue(recurrenceInfo.weekDays);
            this.spinYearlyDayNumber.SetValue(recurrenceInfo.dayNumber);
            this.rbDay.SetChecked(false);
            this.rbWeekOfMonth.SetChecked(false);
            var start = (recurrenceInfo.interval) ? recurrenceInfo.interval.start : new Date();
            if (recurrenceInfo.weekOfMonth == ASPxClientWeekOfMonth.None) {
                this.rbDay.SetChecked(true);
                var weekOfMonth = RecurrenceFormHelper.CalcWeekOfMonth(start, recurrenceInfo.dayOfWeek);
                this.wmeYearlyWeekOfMonth.SetValue(weekOfMonth);
            }
            else {
                this.rbWeekOfMonth.SetChecked(true);
                this.wmeYearlyWeekOfMonth.SetValue(recurrenceInfo.weekOfMonth);
            }
        },
        GetValue: function () {
            var value = {};
            value["DayNumber"] = this.valueAccessor.GetDayNumber();
            value["Month"] = this.valueAccessor.GetMonth();
            value["WeekDays"] = this.valueAccessor.GetWeekDays();
            value["WeekOfMonth"] = this.valueAccessor.GetWeekOfMonth();
            return value;
        }
    });
    ///////////////////////////////////////////////////////////////////////////////
    /////////////////////////// RecurrenceFormHelper //////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////
    var RecurrenceFormHelper = ASPx.CreateClass(null, {});
    RecurrenceFormHelper.CalcWeekOfMonth = function (start, dayOfWeek) {
        var count = 0;
        var date = new Date(start.getFullYear(), start.getMonth(), 1);
        count = 0;
        while (date < start) {
            if (dayOfWeek == date.getDay())
                count++;
            date.setDate(1 + date.getDate());
        }
        if (count <= 1)
            return ASPxClientWeekOfMonth.First;
        else if (count == 2)
            return ASPxClientWeekOfMonth.Second;
        else if (count == 3)
            return ASPxClientWeekOfMonth.Third;
        else if (count == 4)
            return ASPxClientWeekOfMonth.Fourth;
        else if (count >= 5)
            return ASPxClientWeekOfMonth.Last;
    }
    ////////////////////////////////////////////////////////////////////////////////
    /////////////////// DefaultRecurrenceRuleValuesAccessor/////////////////////////
    ////////////////////////////////////////////////////////////////////////////////
    var DefaultRecurrenceRuleValuesAccessor = ASPx.CreateClass(null, {
        constructor: function (recurrenceControl) {
            this.recurrenceControl = recurrenceControl;
        },
        GetPeriodicity: function () {
            return ASPxClientRecurrenceInfo.DefaultPeriodicity;
        },
        GetDayNumber: function () {
            return 1;
        },
        GetMonth: function () {
            return 1;
        },
        GetWeekDays: function () {
            return ASPxClientRecurrenceInfo.DefaultWeekDays;
        },
        GetWeekOfMonth: function () {
            return ASPxClientRecurrenceInfo.DefaultWeekOfMonth;
        }
    });
    var HourlyRecurrenceValuesAccessor = ASPx.CreateClass(DefaultRecurrenceRuleValuesAccessor, {
        constructor: function (recurrenceControl) {
            this.constructor.prototype.constructor.call(this, recurrenceControl);
        },
        GetPeriodicity: function () {
            return this.recurrenceControl.SpinHourCount.GetValue();
        }
    });
    ////////////////////////////////////////////////////////////////////////////////
    /////////////////////// DailyRecurrenceValuesAccessor //////////////////////////
    ////////////////////////////////////////////////////////////////////////////////
    var DailyRecurrenceValuesAccessor = ASPx.CreateClass(DefaultRecurrenceRuleValuesAccessor, {
        constructor: function (recurrenceControl) {
            this.constructor.prototype.constructor.call(this, recurrenceControl);
        },
        GetPeriodicity: function () {
            return this.recurrenceControl.SpinDailyDaysCount.GetValue();
        },
        GetWeekDays: function () {
            return (this.recurrenceControl.RbDay.GetChecked()) ? ASPxClientWeekDays.EveryDay : ASPxClientWeekDays.WorkDays;
        }
    });
    ////////////////////////////////////////////////////////////////////////////////
    /////////////////////// WeeklyRecurrenceValuesAccessor /////////////////////////
    ////////////////////////////////////////////////////////////////////////////////
    var WeeklyRecurrenceValuesAccessor = ASPx.CreateClass(DefaultRecurrenceRuleValuesAccessor, {
        constructor: function (recurrenceControl) {
            this.constructor.prototype.constructor.call(this, recurrenceControl);
        },
        GetPeriodicity: function () {
            return this.recurrenceControl.SpinWeeklyWeeksCount.GetValue();
        },
        GetWeekDays: function () {
            return this.recurrenceControl.WeekDaysCheckEdit.GetValue();
        }
    });
    ////////////////////////////////////////////////////////////////////////////////
    /////////////////////// WeeklyRecurrenceValuesAccessor /////////////////////////
    ////////////////////////////////////////////////////////////////////////////////
    var MonthlyRecurrenceValuesAccessor = ASPx.CreateClass(DefaultRecurrenceRuleValuesAccessor, {
        constructor: function (recurrenceControl) {
            this.constructor.prototype.constructor.call(this, recurrenceControl);
        },
        GetDayNumber: function () {
            return this.recurrenceControl.spinMonthlyDay.GetValue();
        },
        GetPeriodicity: function () {
            result = (this.recurrenceControl.rbDay.GetChecked()) ? this.recurrenceControl.spinMonthlyDayMonthCount.GetValue() : this.recurrenceControl.spinMonthlyWeekDaysMonthCount.GetValue();
            return result;
        },
        GetWeekDays: function () {
            return this.recurrenceControl.wdeMonthlyWeekDays.GetValue();
        },
        GetWeekOfMonth: function () {
            return (this.recurrenceControl.rbDay.GetChecked()) ? ASPxClientWeekOfMonth.None : this.recurrenceControl.wmeMonthlyWeekOfMonth.GetValue();
        }
    });
    ////////////////////////////////////////////////////////////////////////////////
    /////////////////////// YearlyRecurrenceValuesAccessor /////////////////////////
    ////////////////////////////////////////////////////////////////////////////////
    var YearlyRecurrenceValuesAccessor = ASPx.CreateClass(DefaultRecurrenceRuleValuesAccessor, {
        constructor: function (recurrenceControl) {
            this.constructor.prototype.constructor.call(this, recurrenceControl);
        },
        GetDayNumber: function () {
            return this.recurrenceControl.spinYearlyDayNumber.GetValue();
        },
        GetMonth: function () {
            return (this.recurrenceControl.rbDay.GetChecked()) ? this.recurrenceControl.meYearlyDayMonth.GetValue() : this.recurrenceControl.meYearlyWeekDaysMonth.GetValue();
        },
        GetWeekDays: function () {
            return this.recurrenceControl.wdeYearlyWeekDays.GetValue();
        },
        GetWeekOfMonth: function () {
            return (this.recurrenceControl.rbDay.GetChecked()) ? ASPxClientWeekOfMonth.None : this.recurrenceControl.wmeYearlyWeekOfMonth.GetValue();
        }
    });
    ////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////// ASPxClientFormBase //////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////
    var ASPxClientFormBase = ASPx.CreateClass(ASPxClientControl, {
        constructor: function (name) {
            this.constructor.prototype.constructor.call(this, name);
            this.FormClosed = new ASPxClientEvent();
            this.name = name;
            this.uniqueID = name;
        },
        Close: function () {
            this.RaiseFormClosed();
        },
        RaiseFormClosed: function () {
            var args = new ASPxClientEventArgs();
            this.FormClosed.FireEvent(this, args);
        },
        SetVisibleCore: function (element, isVisible) {
            if (!element)
                return;
            if (isVisible) {
                ASPx.SetElementDisplay(element, true);
                ASPx.GetControlCollection().AdjustControls(element);
            }
            else
                ASPx.SetElementDisplay(element, false);
        },
        IsDOMDisposed: function () {
            if (this.linkToDOM) {
                var itemInDOM = ASPx.GetElementById(this.linkToDOM.id);
                return !ASPx.IsExistsElement(itemInDOM);
            }
            return false;
        },
        OnDispose: function () {
            ASPxClientControl.prototype.OnDispose.call(this);
            this.linkToDOM = null;
        },
        GetFormOwner: function () {
            if (!this.formOwnerId)
                return;
            return ASPx.GetControlCollection().Get(this.formOwnerId);
        }
    });
    var ASPxClientAppointmentRecurrenceControl = ASPx.CreateClass(ASPxClientRecurrenceControlBase, {
        constructor: function (name) {
            this.constructor.prototype.constructor.call(this, name);
            this.ValidationCompleted = new ASPxClientEvent();
        },
        Initialize: function () {
            this.constructor.prototype.Initialize.call(this);
            this.RecurrenceRangeControl.ValidationCompleted.AddHandler(ASPx.CreateDelegate(this.OnRecurrenceRangeControlValidationCompleted, this));
        },
        IsValid: function () {
            return this.RecurrenceRangeControl.IsValid();
        },
        SetStart: function (start) {
            this.RecurrenceRangeControl.SetStart(start);
        },
        OnRecurrenceRangeControlValidationCompleted: function (s, e) {
            this.RaiseValidationCompleted(e.isValid);
        },
        RaiseValidationCompleted: function (isValid) {
            if (!this.ValidationCompleted.IsEmpty()) {
                var args = new ASPxClientSchedulerValidationCompletedArgs(isValid);
                this.ValidationCompleted.FireEvent(this, args);
            }
        }
    });
    var ASPxClientAppointmentRecurrenceForm = ASPx.CreateClass(ASPxClientRecurrenceControlBase, {
        constructor: function (name) {
            this.constructor.prototype.constructor.call(this, name);
            this.isASPxClientEdit = true;
            this.ValidationCompleted = new ASPxClientEvent();
        },
        Initialize: function () {
            this.constructor.prototype.Initialize.call(this);
            this.RecurrenceControl.ValidationCompleted.AddHandler(ASPx.CreateDelegate(this.OnRecurrenceRangeControlValidationCompleted, this));
            this.ChkRecurrence.CheckedChanged.AddHandler(ASPx.CreateDelegate(this.OnChkRecurrenceCheckedChanged, this));
            this.primaryValue = this.GetValue();
        },
        OnChkRecurrenceCheckedChanged: function (s, e) {
            var recFormMainDiv = this.RecurrenceControl.GetMainElement();
            var display = ASPx.GetElementDisplay(recFormMainDiv);
            ASPx.SetElementDisplay(recFormMainDiv, !display);
            if (ASPx.IsExists(recFormMainDiv.chkControlsSizeCorrected)) {
                recFormMainDiv.chkControlsSizeCorrected = true;
                if (!display)
                    if (ASPx.Browser.Opera)
                        window.setTimeout(ASPx.CreateDelegate(this.RecalculateLayoutDeferred, this), 0);
                    else
                        ASPx.GetControlCollection().AdjustControls(recFormMainDiv);
            }
            this.RaiseValidationCompleted(this.IsValid());
        },
        RecalculateLayoutDeferred: function () {
            ASPx.GetControlCollection().AdjustControls(this.RecurrenceControl);
        },
        IsValid: function () {
            var isChecked = this.ChkRecurrence.GetValue();
            if (!isChecked)
                return true;
            return this.RecurrenceControl.IsValid();
        },
        IsChanged: function () {
            var currentValue = this.GetValue();
            return ASPx.Json.ToJson(this.primaryValue) != ASPx.Json.ToJson(currentValue);
        },
        OnRecurrenceRangeControlValidationCompleted: function (s, e) {
            this.RaiseValidationCompleted(e.isValid);
        },
        RaiseValidationCompleted: function (isValid) {
            if (!this.ValidationCompleted.IsEmpty()) {
                var args = new ASPxClientSchedulerValidationCompletedArgs(isValid);
                this.ValidationCompleted.FireEvent(this, args);
            }
        },
        GetValue: function () {
            var value = {};
            value["Reccurence"] = this.GetChildControlValueByName("ChkRecurrence");
            value["Type"] = this.GetChildControlValueByName("AptRecCtl_TypeEdt");
            value["Hourly"] = this.GetChildControlValueByName("AptRecCtl_Hourly");
            value["Daily"] = this.GetChildControlValueByName("AptRecCtl_Daily");
            value["Weekly"] = this.GetChildControlValueByName("AptRecCtl_Weekly");
            value["Monthly"] = this.GetChildControlValueByName("AptRecCtl_Monthly");
            value["Yearly"] = this.GetChildControlValueByName("AptRecCtl_Yearly");
            value["Range"] = this.GetChildControlValueByName("AptRecCtl_RangeCtl");
            return value;
        },
        SetStart: function (start) {
            this.RecurrenceControl.SetStart(start);
        },
        GetChildControlValueByName: function (childControlName) {
            var childControlClientId = this.name + "_" + childControlName;
            return ASPx.GetControlCollection().Get(childControlClientId).GetValue();
        },
        IsEditorElement: function (element) {
            return false;
        }
    });

    var ASPxSchedulerMessageBoxLink = [];

    var ASPxSchedulerMessageBoxBase = ASPx.CreateClass(ASPxClientFormBase, {
        constructor: function (name) {
            this.constructor.prototype.constructor.call(this, name);
        },
        Show: function (caption, message, okHandler, cancelHandler) {
            this.okHandler = okHandler;
            this.cancelHandler = cancelHandler;
            var viewPortWidth = window.innerWidth;
            var viewPortHeight = window.innerHeight;
            var scrollX = ASPx.GetDocumentScrollLeft();
            var scrollY = ASPx.GetDocumentScrollTop();

            this.popup.SetWidth(0);
            this.popup.SetHeight(0);

            this.UpdateMessage(message);
            var desiredWidth = this.CalculateDesiredWidth();
            this.popup.SetWidth(desiredWidth);

            var popupWidth = this.popup.GetWidth();
            var popupHeight = this.popup.GetHeight();
            var x = scrollX + viewPortWidth / 2 - popupWidth / 2;
            var y = scrollY + viewPortHeight / 2 - popupHeight / 2;
            this.popup.SetHeaderText(caption);
            this.popup.ShowAtPos(x, y);
        },
        Ok: function () {
            if (!this.popup)
                return;
            this.popup.Hide();
            if (!this.okHandler)
                return;
            this.okHandler();
        },
        Cancel: function () {
            if (!this.popup)
                return;
            this.popup.Hide();
            if (!this.cancelHandler)
                return;
            this.cancelHandler();
        },
        UpdateMessage: function (message) {
        },
        AdjustControl: function () {
        },
        CalculateDesiredWidth: function () {
            return 0;
        },
        RegisterMessageBox: function (formOwnerId) {
            if (formOwnerId)
                ASPxSchedulerMessageBoxLink[formOwnerId] = this;
        },
        LinkToPopup: function (popup) {
            this.popup = popup;
        },
        OnDispose: function () {
            ASPxClientFormBase.prototype.OnDispose.call(this);
            this.popup = null;
        }
    });

    ASPxSchedulerMessageBoxBase.SchedulerLink = ASPxSchedulerMessageBoxLink;

    window.ASPxClientWeekDaysCheckEdit = ASPxClientWeekDaysCheckEdit;
    window.ASPxClientRecurrenceRangeControl = ASPxClientRecurrenceRangeControl;
    window.ASPxClientRecurrenceControlBase = ASPxClientRecurrenceControlBase;
    window.ASPxClientHourlyRecurrenceControl = ASPxClientHourlyRecurrenceControl;
    window.ASPxClientDailyRecurrenceControl = ASPxClientDailyRecurrenceControl;
    window.ASPxClientWeeklyRecurrenceControl = ASPxClientWeeklyRecurrenceControl;
    window.ASPxClientMonthlyRecurrenceControl = ASPxClientMonthlyRecurrenceControl;
    window.ASPxClientYearlyRecurrenceControl = ASPxClientYearlyRecurrenceControl;
    window.ASPxSchedulerMessageBoxBase = ASPxSchedulerMessageBoxBase;
    window.DefaultRecurrenceRuleValuesAccessor = DefaultRecurrenceRuleValuesAccessor;
    window.HourlyRecurrenceValuesAccessor = HourlyRecurrenceValuesAccessor;
    window.DailyRecurrenceValuesAccessor = DailyRecurrenceValuesAccessor;
    window.WeeklyRecurrenceValuesAccessor = WeeklyRecurrenceValuesAccessor;
    window.MonthlyRecurrenceValuesAccessor = MonthlyRecurrenceValuesAccessor;
    window.YearlyRecurrenceValuesAccessor = YearlyRecurrenceValuesAccessor;
    window.ASPxClientFormBase = ASPxClientFormBase;
    window.ASPxClientAppointmentRecurrenceForm = ASPxClientAppointmentRecurrenceForm;
    window.ASPxClientAppointmentRecurrenceControl = ASPxClientAppointmentRecurrenceControl;
})();