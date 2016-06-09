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

using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
namespace DevExpress.Xpf.Scheduler.UI {
	public partial class AppointmentForm : UserControl {
		readonly AppointmentFormController controller;
		string timeEditMask = CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;
		readonly SchedulerStorage storage;
		readonly SchedulerControl control;
		Appointment currentAppointment;
		bool canCloseFormOnDelete = true;
		[EditorBrowsable(EditorBrowsableState.Never)]
		public AppointmentForm() {
		}
		public AppointmentForm(SchedulerControl control, Appointment apt, bool readOnly) {
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(control.Storage, "control.Storage");
			Guard.ArgumentNotNull(apt, "apt");
			this.controller = CreateController(control, apt);
			this.currentAppointment = apt;
			CanChangeAllDay = CalcCanChangeAllDay();
			this.control = control;
			this.storage = control.Storage;
			this.IsReadOnly = readOnly;
			SubscribeStorageEvents();
			InitializeComponent();
			brdResources.DataContext = this;
		}
		#region Properties
		#region Title
		public String Title {
			get { return (String)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}
		public static readonly DependencyProperty TitleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentForm, String>("Title", String.Empty);
		#endregion
		public AppointmentFormController Controller { get { return controller; } }
		public string TimeEditMask { get { return timeEditMask; } }
		protected internal SchedulerControl Control { get { return control; } }
		protected internal SchedulerStorage Storage { get { return storage; } }
		protected internal bool IsNewAppointment { get { return controller != null ? controller.IsNewAppointment : true; } }
		public bool ShouldShowRecurrence {
			get {
				return Controller.ShouldShowRecurrence;
			}
		}
		#region IsReadOnly
		public bool IsReadOnly {
			get { return (bool)GetValue(IsReadOnlyProperty); }
			set { SetValue(IsReadOnlyProperty, value); }
		}
		public static readonly DependencyProperty IsReadOnlyProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentForm, bool>("IsReadOnly", false, (d, e) => d.OnIsReadOnlyChanged(e.OldValue, e.NewValue));
		protected virtual void OnIsReadOnlyChanged(bool oldValue, bool newValue) {
		}
		#endregion
		#region CanChangeAllDay
		public bool CanChangeAllDay {
			get { return (bool)GetValue(CanChangeAllDayProperty); }
			set { SetValue(CanChangeAllDayProperty, value); }
		}
		public static readonly DependencyProperty CanChangeAllDayProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentForm, bool>("CanChangeAllDay", true);
		#endregion
		#endregion
		void SubscribeStorageEvents() {
			Control.Storage.AppointmentsDeleted += OnStorageAppointmentsDeleted;
		}
		void UnsubscribeStorageEvents() {
			Control.Storage.AppointmentsDeleted -= OnStorageAppointmentsDeleted;
		}
		void OnStorageAppointmentsDeleted(object sender, PersistentObjectsEventArgs e) {
			if (!this.canCloseFormOnDelete)
				return;
			int count = e.Objects.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = e.Objects[i] as Appointment;
				if (apt != null && apt == this.currentAppointment)
					Dispatcher.BeginInvoke(new Action(() => CloseForm(false)));
			}
		}
		protected virtual AppointmentFormController CreateController(SchedulerControl control, Appointment apt) {
			return new AppointmentFormController(control, apt);
		}
		bool CalcCanChangeAllDay() {
			if (this.currentAppointment.IsRecurring) {
				Appointment apt = currentAppointment;
				if (this.currentAppointment.RecurrencePattern != null)
					apt = this.currentAppointment.RecurrencePattern;
				return !((RecurrenceType?)apt.RecurrenceInfo.Type).HasValue;
			}
			else
				return !currentAppointment.IsRecurring;
		}
		void OnOKButtonClick(object sender, RoutedEventArgs e) {
			if (edtEndDate.HasValidationError || edtEndTime.HasValidationError)
				return;
			ValidationArgs args = new ValidationArgs();
			SchedulerFormHelper.ValidateValues(this, args);
			if (!args.Valid) {
#if !SL
				DXMessageBox.Show(args.ErrorMessage, System.Windows.Forms.Application.ProductName, System.Windows.MessageBoxButton.OK, MessageBoxImage.Exclamation);
#else
				MessageBox.Show(args.ErrorMessage, String.Empty, System.Windows.MessageBoxButton.OK);
#endif
				FocusInvalidControl(args);
				return;
			}
			SchedulerFormHelper.CheckForWarnings(this, args);
			if (!args.Valid) {
#if !SL
				MessageBoxResult answer = DXMessageBox.Show(args.ErrorMessage, System.Windows.Forms.Application.ProductName, System.Windows.MessageBoxButton.OKCancel, MessageBoxImage.Question);
#else
				MessageBoxResult answer = MessageBox.Show(args.ErrorMessage, String.Empty, System.Windows.MessageBoxButton.OKCancel);
#endif
				if (answer == MessageBoxResult.Cancel) {
					FocusInvalidControl(args);
					return;
				}
			}
			if (!controller.IsConflictResolved()) {
#if !SL
				DXMessageBox.Show(SchedulerLocalizer.GetString(SchedulerStringId.Msg_Conflict), System.Windows.Forms.Application.ProductName, System.Windows.MessageBoxButton.OK, MessageBoxImage.Exclamation);
#else
				MessageBox.Show(SchedulerLocalizer.GetString(SchedulerStringId.Msg_Conflict), String.Empty, System.Windows.MessageBoxButton.OK);
#endif
				return;
			}
			this.canCloseFormOnDelete = false;
			if (Controller.EditedPattern != null)
				SynchronizeRecurrenceInfoStart(Controller.TimeZoneHelper.FromClientTime(Controller.Start));
			Controller.ApplyChanges();
			CloseForm(true);
		}
		void SynchronizeRecurrenceInfoStart(DateTime start) {
			Controller.EditedPattern.RecurrenceInfo.Start = start;
		}
		protected virtual bool CanApplyChanges() {
			return Controller.IsNewAppointment || controller.IsAppointmentChanged();
		}
		void FocusInvalidControl(ValidationArgs args) {
			Control control = args.Control as Control;
			if (control != null)
				control.Focus();
		}
		void OnCancelButtonClick(object sender, RoutedEventArgs e) {
			CloseForm(false);
		}
		void OnDeleteButtonClick(object sender, RoutedEventArgs e) {
			if (IsNewAppointment)
				return;
			this.canCloseFormOnDelete = false;
			Controller.DeleteAppointment();
			CloseForm(false);
		}
		private void CloseForm(bool dialogResult) {
			UnsubscribeStorageEvents();
			SchedulerFormBehavior.Close(this, dialogResult);
		}
		void OnEdtEndTimeValidate(object sender, ValidationEventArgs e) {
			if (e.Value == null)
				return;
			e.IsValid = IsValidInterval(Controller.Start.Date, Controller.Start.TimeOfDay, Controller.End.Date, ((DateTime)e.Value).TimeOfDay);
			e.ErrorContent = SchedulerLocalizer.GetString(SchedulerStringId.Msg_InvalidEndDate);
		}
		void OnEdtEndDateValidate(object sender, ValidationEventArgs e) {
			if (e.Value == null)
				return;
			e.IsValid = IsValidInterval(Controller.Start.Date, Controller.Start.TimeOfDay, (DateTime)e.Value, Controller.End.TimeOfDay);
			e.ErrorContent = SchedulerLocalizer.GetString(SchedulerStringId.Msg_InvalidEndDate);
		}
		protected internal virtual bool IsValidInterval(DateTime startDate, TimeSpan startTime, DateTime endDate, TimeSpan endTime) {
			return AppointmentFormControllerBase.ValidateInterval(startDate, startTime, endDate, endTime);
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			LayoutUpdated += new EventHandler(OnLayoutUpdated);
			Controller.PropertyChanged += new PropertyChangedEventHandler(Controller_PropertyChanged);
			UpdateTitle();
		}
		void Controller_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			UpdateTitle();
		}
		void UpdateTitle() {
			string newTitle = SchedulerUtils.FormatAppointmentFormCaption(Controller.AllDay, Controller.Subject, false);
			SchedulerFormBehavior.SetTitle(this, newTitle);
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			LayoutUpdated -= new EventHandler(OnLayoutUpdated);
			subjectEdit.Focus();
		}
		void OnRecurrenceButtonClick(object sender, RoutedEventArgs e) {
			if (!controller.ShouldShowRecurrenceButton)
				return;
			Control.ShowRecurrenceForm(this, IsReadOnly);
		}
	}
}
