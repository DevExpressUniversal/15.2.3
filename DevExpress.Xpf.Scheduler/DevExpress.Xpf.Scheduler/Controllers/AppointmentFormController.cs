#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraScheduler.UI;
using DevExpress.XtraScheduler;
using DevExpress.Utils;
using System.ComponentModel;
using System.Collections;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Localization;
using System.Globalization;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.XtraScheduler.Services.Internal;
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using System.Windows.Controls;
using DevExpress.Xpf.Scheduler.Commands;
using DevExpress.XtraScheduler.Internal.Implementations;
using System.Reflection;
namespace DevExpress.Xpf.Scheduler.UI {
	public class AppointmentFormViewModel : AppointmentFormControllerBase, IDocumentContent {
		bool canCloseFormOnDelete = true;
		Appointment patternCopy;
		IMessageBoxService actualMessageBoxService;
		public AppointmentFormViewModel(SchedulerControl control, Appointment apt)
			: this(control, apt, false, false) {
		}
		public AppointmentFormViewModel(SchedulerControl control, Appointment apt, bool readOnly)
			: this(control, apt, readOnly, false) {
		}
		public AppointmentFormViewModel(SchedulerControl control, Appointment apt, bool readOnly, bool showRecurrenceDialog)
			: base(control.InnerControl, apt) {
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(control.Storage, "control.Storage");
			Guard.ArgumentNotNull(apt, "apt");
			IsReadOnly = readOnly;
			CanChangeAllDay = CalcCanChangeAllDay();
			Control = control;
			Resources = control.Storage.ResourceStorage.InnerResources;
			this.patternCopy = PrepareToRecurrenceEdit();
			PropertyChanged += new PropertyChangedEventHandler(OnAppointmentFormControllerPropertyChanged);
			UpdateTitle();
			SubscribeStorageEvents();
			CustomFields = new Dictionary<string, object>();
			FillCustomFields();
			if (showRecurrenceDialog) {
				Control.Dispatcher.BeginInvoke(new Action(delegate { Control.ShowRecurrenceForm(this, IsReadOnly); }));
			}
		}
		public static AppointmentFormViewModel Create(SchedulerControl control, Appointment apt) {
			return AppointmentFormViewModel.Create(control, apt, false, false);
		}
		public static AppointmentFormViewModel Create(SchedulerControl control, Appointment apt, bool readOnly) {
			return AppointmentFormViewModel.Create(control, apt, readOnly, false);
		}
		public static AppointmentFormViewModel Create(SchedulerControl control, Appointment apt, bool readOnly, bool showRecurrenceDialog) {
			return ViewModelSource.Create(() => new AppointmentFormViewModel(control, apt, readOnly, showRecurrenceDialog));
		}
		public IDocument ActiveDocument { get; set; }
		public virtual IMessageBoxService MessageBoxService { get { return null; } }
		public Dictionary<string, object> CustomFields { get; protected set; }
		public SchedulerControl Control { get; protected set; }
		public SchedulerStorage Storage { get { return Control.Storage; } }
		public TimeZoneHelper TimeZoneHelper { get { return base.InnerControl.TimeZoneHelper; } }
		public bool ShouldShowRecurrence { get { return !SourceAppointment.IsOccurrence && ShouldShowRecurrenceButton; } }
		protected IAppointmentStorage Appointments { get { return (IAppointmentStorage)InnerAppointments; } }
		protected IResourceStorageBase Resources { get; set; }
		protected AppointmentLabelCollection Labels { get { return Appointments.Labels; } }
		protected AppointmentStatusCollection Statuses { get { return Appointments.Statuses; } }
		public AppointmentStatus Status { get { return Statuses.GetById(StatusKey); } set { OnStatusChanged(value); } }
		public AppointmentLabel Label { get { return Labels.GetById(LabelKey); } set { OnLabelChanged(value); } }
		public Resource AppointmentResource { get { return Resources.GetResourceById(ResourceId); } set { OnResourceChanged(value); } }
		protected string NoneString { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_NoneReminder); } }
		public string TimeEditMask { get { return GetTimeEditMask(); } }
		#region Title
		object title = null;
		public object Title {
			get { return title; }
			set {
				if (title != value) {
					title = value;
					NotifyPropertyChanged("Title");
				}
			}
		}
		#endregion
		#region IsReadOnly
		bool isReadOnly = false;
		public bool IsReadOnly {
			get { return isReadOnly; }
			set {
				if (isReadOnly != value) {
					isReadOnly = value;
					NotifyPropertyChanged("IsReadOnly");
				}
			}
		}
		#endregion
		#region CanChangeAllDay
		bool canChangeAllDay = false;
		public bool CanChangeAllDay {
			get { return canChangeAllDay; }
			set {
				if (canChangeAllDay != value) {
					canChangeAllDay = value;
					NotifyPropertyChanged("CanChangeAllDay");
				}
			}
		}
		#endregion
		#region IsRecurrenceEditable
		public bool IsRecurrenceEditable {
			get { return CanEditRecurrence(); }
		}
		#endregion
		public DateTime DisplayStartDate {
			get { return DisplayStart.Date; }
			set {
				SetDisplayStartDate(value);
				NotifyPropertyChanged("DisplayStartDate");
				NotifyPropertyChanged("DisplayEndDate");
			}
		}
		public TimeSpan DisplayStartTime {
			get { return new DateTime(DisplayStart.TimeOfDay.Ticks).TimeOfDay; }
			set {
				SetDisplayStartTime(value);
				NotifyPropertyChanged("DisplayStartTime");
				NotifyPropertyChanged("DisplayEndTime");
			}
		}
		public DateTime DisplayEndDate {
			get { return DisplayEnd.Date; }
			set {
				if (!ValidateInterval(DisplayStartDate, DisplayStartTime, value, DisplayEndTime)) {
					throw new ArgumentException(SchedulerLocalizer.GetString(SchedulerStringId.Msg_InvalidEndDate));
				}
				SetDisplayEndDate(value);
				NotifyPropertyChanged("DisplayEndDate");
				NotifyPropertyChanged("DisplayEndTime");
			}
		}
		public TimeSpan DisplayEndTime {
			get {
				return new DateTime(DisplayEnd.TimeOfDay.Ticks).TimeOfDay;
			}
			set {
				if (!ValidateInterval(DisplayStartDate, DisplayStartTime, DisplayEndDate, value)) {
					throw new ArgumentException(SchedulerLocalizer.GetString(SchedulerStringId.Msg_InvalidEndDate));
				}
				SetDisplayEndTime(value);
				NotifyPropertyChanged("DisplayEndTime");
				NotifyPropertyChanged("DisplayEndDate");
			}
		}
		public Appointment PatternCopy {
			get {
				if (patternCopy == null)
					patternCopy = PrepareToRecurrenceEdit();
				return patternCopy;
			}
			set { }
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentFormControllerAppointmentResources")]
#endif
		public ResourceBaseCollection AppointmentResources { get { return GetAppointmentResources(); } set { } }
		public RecurrenceInfo PatternRecurrenceInfo {
			get { return SourceAppointment.IsRecurring ? (RecurrenceInfo)PatternCopy.RecurrenceInfo : null; }
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentFormControllerAppointmentResourceIds")]
#endif
		public IList AppointmentResourceIds { get { return EditedAppointmentCopy.ResourceIds; } set { SetAppointmentResourceIds(value); } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentFormControllerReminderSpans")]
#endif
		public IList ReminderSpans { get { return GetReminderSpans(); } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentFormControllerReminderSpan")]
#endif
		public string ReminderSpan {
			get { return GetReminderTimeBeforeStart(); }
			set { SetReminderTimeBeforeStart(value); }
		}
		public override bool IsTimeEnabled {
			get { return !AllDay; }
		}
		public override bool IsDateTimeEditable {
			get { return true; }
		}
		protected IMessageBoxService ActualMessageBoxService {
			get {
				if (MessageBoxService != null)
					return MessageBoxService;
				if (this.actualMessageBoxService == null)
					this.actualMessageBoxService = new DevExpress.Xpf.Core.DXMessageBoxService();
				return this.actualMessageBoxService;
			}
		}
		protected IDocumentOwner DocumentOwner { get; private set; }
		#region Commands
		public void SaveAndCloseAppointment() {
			SaveAppointmentCore();
			CloseForm();
		}
		private void SaveAppointmentCore() {
			if (!IsConflictResolved()) {
#if SL
				ActualMessageBoxService.Show(SchedulerLocalizer.GetString(SchedulerStringId.Msg_Conflict));
#else
				ActualMessageBoxService.Show(SchedulerLocalizer.GetString(SchedulerStringId.Msg_Conflict), System.Windows.Forms.Application.ProductName, System.Windows.MessageBoxButton.OK, MessageBoxImage.Exclamation);
#endif
				return;
			}
			this.canCloseFormOnDelete = false;
			if (EditedPattern != null)
				SynchronizeRecurrenceInfoStart(TimeZoneHelper.FromClientTime(Start));
			if (IsAppointmentChanged())
				ApplyChanges();
		}
		public bool CanSaveAndCloseAppointment() {
			return !IsReadOnly;
		}
		public void SaveAppointment() {
			SaveAppointmentCore();
		}
		public bool CanSaveAppointemnt() {
			return !IsReadOnly;
		}
		public void CancelEditing() {
			CloseForm();
		}
		public void RemoveAppointment() {
			canCloseFormOnDelete = false;
			DeleteAppointment();
			CloseForm();
		}
		public bool CanRemoveAppointment() {
			return !IsNewAppointment && !IsReadOnly;
		}
		public void EditRecurrence() {
			if (!ShouldShowRecurrenceButton)
				return;
			Control.ShowRecurrenceForm(this, IsReadOnly);
		}
		public bool CanEditRecurrence() {
			return ShouldShowRecurrence && !IsReadOnly;
		}
		public void EditTimeZone() {
			TimeZoneVisible = !TimeZoneVisible;
		}
		public bool CanEditTimeZone() {
			return TimeZonesEnabled && !IsReadOnly;
		}
		public void Undo() {
			TextBox focusedControl = FocusManager.GetFocusedElement(GetActiveWindow()) as TextBox;
			if (focusedControl != null)
				focusedControl.Undo();
		}
		public bool CanUndo() {
			Window focusedWindow = GetActiveWindow();
			if (focusedWindow == null)
				return false;
			TextBox textBox = FocusManager.GetFocusedElement(focusedWindow) as TextBox;
			return textBox != null && textBox.CanUndo && !IsReadOnly;
		}
		public void Redo() {
			TextBox focusedControl = FocusManager.GetFocusedElement(GetActiveWindow()) as TextBox;
			if (focusedControl != null)
				focusedControl.Redo();
		}
		public bool CanRedo() {
			Window focusedWindow = GetActiveWindow();
			if (focusedWindow == null)
				return false;
			TextBox textBox = FocusManager.GetFocusedElement(focusedWindow) as TextBox;
			return textBox != null && textBox.CanRedo && !IsReadOnly;
		}
		private Window GetActiveWindow() {
			return Application.Current == null ? null : Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
		}
		public void Next() {
			SelectNextAppointmentCommand command = new SelectNextAppointmentCommand(this.Control);
			command.Execute();
			OpenSelectedAppointmentForm();
		}
		public bool CanNext() {
			return !IsReadOnly;
		}
		public void Previous() {
			SelectPrevAppointmentCommand command = new SelectPrevAppointmentCommand(this.Control);
			command.Execute();
			OpenSelectedAppointmentForm();
		}
		public bool CanPrevious() {
			return !IsReadOnly;
		}
		private void OpenSelectedAppointmentForm() {
			if (this.Control.SelectedAppointments.Count == 1 && this.Control.SelectedAppointments[0] != SourceAppointment) {
				this.Control.ShowEditAppointmentForm(this.Control.SelectedAppointments[0]);
				CloseForm();
			}
		}
		public void CloseForm() {
#if DEBUGTEST
			if (DevExpress.Xpf.Scheduler.Tests.TestFormHelper.ActiveViewModels.ContainsKey(this))
				DevExpress.Xpf.Scheduler.Tests.TestFormHelper.ActiveViewModels.Remove(this);
#endif
			UnsubscribeStorageEvents();
			if (ActiveDocument == null)
				return;
			ActiveDocument.Close(false);
		}
		#endregion
		public bool Close() {
			if (Control == null)
				return true;
			RestoreControlFocus();
			ISetSchedulerStateService setStateService = Control.GetService<ISetSchedulerStateService>();
			if (setStateService != null)
				setStateService.IsModalFormOpened = false;
			return true;
		}
		public override bool IsAppointmentChanged() {
			if (this.IsNewAppointment) {
				return true;
			}
			if (base.IsAppointmentChanged()) {
				return true;
			}
			foreach (CustomField field in SourceAppointment.CustomFields) {
				if (CustomFields[field.Name] != field.Value) {
					return true;
				}
			}
			return false;
		}
		public override void ApplyChanges() {
			foreach (CustomField field in EditedAppointmentCopy.CustomFields) {
				field.Value = CustomFields[field.Name];
			}
			base.ApplyChanges();
		}
		public void ApplyRecurrenceInfo(IRecurrenceInfo recurrenceInfo) {
			if (recurrenceInfo == null) {
				RemoveRecurrence();
				return;
			}
			PatternCopy.Start = EditedAppointmentCopy.Start;
			PatternCopy.End = EditedAppointmentCopy.End;
			PatternCopy.AllDay = AllDay;
			PatternCopy.RecurrenceInfo.Assign(recurrenceInfo);
		}
		protected override void NotifyPropertyChanged(string info) {
			base.NotifyPropertyChanged(info);
			if (info == "Subject" || info == "AllDay") {
				UpdateTitle();
			}
		}
		protected void RaisePropertyChanged(string propertyName) {
			NotifyPropertyChanged(propertyName);
		}
		protected void SubscribeStorageEvents() {
			Control.Storage.AppointmentsDeleted += OnStorageAppointmentsChanged;
			Control.Storage.AppointmentsChanged += OnStorageAppointmentsChanged;
		}
		protected void UnsubscribeStorageEvents() {
			Control.Storage.AppointmentsDeleted -= OnStorageAppointmentsChanged;
			Control.Storage.AppointmentsChanged -= OnStorageAppointmentsChanged;
		}
		protected virtual void OnStorageAppointmentsChanged(object sender, PersistentObjectsEventArgs e) {
			if (!this.canCloseFormOnDelete)
				return;
			int count = e.Objects.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = e.Objects[i] as Appointment;
				if (apt != null && apt == this.SourceAppointment) {
					CloseForm();
				}
			}
		}
		protected virtual void FillCustomFields() {
			foreach (CustomField field in SourceAppointment.CustomFields) {
				CustomFields[field.Name] = field.Value;
			}
		}
		protected virtual bool ValidateInterval(DateTime displayStartDate, TimeSpan displayStartTime, DateTime displayEndDate, DateTime displayEndTime) {
			return AppointmentFormControllerBase.ValidateInterval(DisplayStartDate, DisplayStartTime, displayEndDate, DisplayEndTime);
		}
		protected internal virtual void SetDisplayStartDate(DateTime value) {
			UpdateStart(value.Date, DisplayStartTime);
		}
		protected internal virtual void SetDisplayStartTime(TimeSpan value) {
			UpdateStart(DisplayStartDate, value);
		}
		protected internal virtual void UpdateStart(DateTime date, TimeSpan timeOfDay) {
			DisplayStart = date + timeOfDay;
		}
		protected internal virtual void SetDisplayEndDate(DateTime value) {
			UpdateEnd(value.Date, DisplayEndTime);
		}
		protected internal virtual void SetDisplayEndTime(TimeSpan value) {
			UpdateEnd(DisplayEndDate, value);
		}
		protected internal virtual void UpdateEnd(DateTime date, TimeSpan timeOfDay) {
			DisplayEnd = date + timeOfDay;
		}
		protected override void OnStatusIdChanged() {
			base.OnStatusIdChanged();
			NotifyPropertyChanged("Status");
		}
		protected override void OnLabelIdChanged() {
			base.OnLabelIdChanged();
			NotifyPropertyChanged("Label");
		}
		protected virtual string GetTimeEditMask() {
			return CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;
		}
		void RestoreControlFocus() {
			Window schedulerWindow = LayoutHelper.FindRoot(Control) as Window;
			if (schedulerWindow != null)
				schedulerWindow.Activate();
		}
		void SynchronizeRecurrenceInfoStart(DateTime start) {
			EditedPattern.RecurrenceInfo.Start = start;
		}
		void UpdateTitle() {
			Title = SchedulerUtils.FormatAppointmentFormCaption(AllDay, Subject, false);
		}
		void OnAppointmentFormControllerPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "AllDay") {
				NotifyPropertyChanged("DisplayStartTime");
				NotifyPropertyChanged("DisplayEndTime");
				NotifyPropertyChanged("DisplayStartDate");
				NotifyPropertyChanged("DisplayEndDate");
			} else if (e.PropertyName == "DisplayStartTime") {
				NotifyPropertyChanged("DisplayEndTime");
				NotifyPropertyChanged("DisplayEndDate");
			}
		}
		ResourceBaseCollection GetAppointmentResources() {
			ResourceBaseCollection result = new ResourceBaseCollection();
			result.Add(ResourceBase.Empty);
			result.AddRange(Resources.Items);
			return result;
		}
		string GetReminderTimeBeforeStart() {
			return HasReminder ? HumanReadableTimeSpanHelper.ToString(ReminderTimeBeforeStart) : NoneString;
		}
		void SetReminderTimeBeforeStart(string value) {
			if (value == NoneString) {
				HasReminder = false;
				return;
			}
			HasReminder = true;
			ReminderTimeBeforeStart = HumanReadableTimeSpanHelper.Parse(value);
		}
		bool CalcCanChangeAllDay() {
			if (SourceAppointment.IsRecurring) {
				Appointment apt = SourceAppointment;
				if (SourceAppointment.RecurrencePattern != null)
					apt = SourceAppointment.RecurrencePattern;
				return !((RecurrenceType?)apt.RecurrenceInfo.Type).HasValue;
			} else
				return !SourceAppointment.IsRecurring;
		}
		void SetAppointmentResourceIds(IList value) {
			EditedAppointmentCopy.BeginUpdate();
			try {
				AppointmentResourceIdCollection resIds = EditedAppointmentCopy.ResourceIds;
				resIds.BeginUpdate();
				try {
					resIds.Clear();
					resIds.AddRange(value);
				} finally {
					resIds.EndUpdate();
				}
			} finally {
				EditedAppointmentCopy.EndUpdate();
			}
		}
		void OnStatusChanged(IAppointmentStatus status) {
			StatusKey = status.Id;
		}
		void OnLabelChanged(IAppointmentLabel label) {
			LabelKey = label.Id;
		}
		void OnResourceChanged(Resource resource) {
			ResourceId = resource.Id;
		}
		IList GetReminderSpans() {
			List<string> result = new List<string>();
			TimeSpan maxDuration = TimeSpan.MaxValue;
			result.Add(NoneString);
			int count = ReminderTimeSpans.ReminderTimeSpanValues.Length;
			for (int i = 0; i < count; i++) {
				TimeSpan timeSpan = ReminderTimeSpans.ReminderTimeSpanValues[i];
				if (timeSpan <= maxDuration)
					result.Add(HumanReadableTimeSpanHelper.ToString(timeSpan));
			}
			return result;
		}
		protected virtual bool CloseChangedAppointment() {
			bool result = true;
#if !DEBUGTEST
			if (IsAppointmentChanged() && !IsNewAppointment && !SourceAppointment.IsDisposed) {
				MessageResult messageResult = ActualMessageBoxService.ShowMessage(SchedulerLocalizer.GetString(SchedulerStringId.Msg_SaveBeforeClose), Assembly.GetEntryAssembly().GetName().Name, MessageButton.YesNoCancel, MessageIcon.Warning);
				if (messageResult == MessageResult.Yes)
					SaveAndCloseAppointment();
				if (messageResult == MessageResult.Cancel)
					result = false;
			}
#endif
			return result;
		}
		#region IDocumentContent
		void IDocumentContent.OnClose(CancelEventArgs e) {
			e.Cancel = !CloseChangedAppointment() || !Close();
		}
		void IDocumentContent.OnDestroy() { }
		IDocumentOwner IDocumentContent.DocumentOwner {
			get { return DocumentOwner; }
			set { DocumentOwner = value; }
		}
		#endregion
	}
}
