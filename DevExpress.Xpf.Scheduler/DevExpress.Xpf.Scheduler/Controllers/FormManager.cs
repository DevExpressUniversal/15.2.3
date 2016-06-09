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
using System.Linq;
using System.Windows;
using DevExpress.Utils;
using System.Collections;
using DevExpress.Xpf.Core;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;
using DevExpress.XtraScheduler;
using DevExpress.Utils.Commands;
using DevExpress.Xpf.Core.Native;
using System.Collections.Generic;
using DevExpress.XtraScheduler.UI;
using DevExpress.Xpf.Scheduler.UI;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.XtraScheduler.Services.Internal;
#if WPF
using PlatformIndependentPropertyChangedCallback = System.Windows.PropertyChangedCallback;
using PlatformIndependentDependencyPropertyChangedEventArgs = System.Windows.DependencyPropertyChangedEventArgs;
using DevExpress.Xpf.Scheduler.Internal;
using DevExpress.Xpf.Scheduler.Native;
#else
using PlatformIndependentDependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PlatformIndependentPropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Xpf.Scheduler.Internal;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class SchedulerPopupBorderControl : PopupBorderControl {
		public SchedulerPopupBorderControl() {
			DefaultStyleKey = typeof(SchedulerPopupBorderControl);
		}
	}
	public class SchedulerPopup : PopupBase {
		public static readonly DependencyProperty ContainerProperty = DependencyProperty.RegisterAttached("Container", typeof(SchedulerPopup), typeof(SchedulerPopup), new PropertyMetadata(null));
		public static void SetContainer(DependencyObject content, SchedulerPopup container) {
			content.SetValue(ContainerProperty, container);
		}
		public static SchedulerPopup GetContainer(DependencyObject content) {
			return (SchedulerPopup)content.GetValue(ContainerProperty);
		}
		public static SchedulerPopup Show(DependencyObject content, Point location) {
			SchedulerPopup container = GetContainer(content);
			if (container != null && container.IsOpen)
				return container;
			container = new SchedulerPopup();
			container.PopupContent = content;
			SetContainer(content, container);
			container.Show(location, false);
			return container;
		}
		public static void Close(DependencyObject content) {
			SchedulerPopup container = GetContainer(content);
			if (container == null)
				return;
			SetContainer(content, null);
			if (container.IsOpen)
				container.IsOpen = false;
		}
		protected override PopupBorderControl CreateBorderControl() {
			return new SchedulerPopupBorderControl();
		}
		void Show(Point location, bool staysOpen) {
			StaysOpen = staysOpen;
			VerticalOffset = location.Y;
			HorizontalOffset = location.X;
#if !SL
			this.Placement = System.Windows.Controls.Primitives.PlacementMode.Absolute;
#endif
			IsOpen = true;
		}
	}
}
namespace DevExpress.Xpf.Scheduler.Native {
	public class SchedulerFormId {
		public const string Reminder = "Reminder";
		public const string EditAppointment = "EditAppointment";
		public const string EditRecurrence = "Recurrence";
		public const string GoToDate = "GoToDate";
		public const string DeleteRecurrentAppointment = "DeleteRecurrentAppointmentForm";
		public const string EditRecurrentAppointment = "EditRecurrentAppointment";
		public const string CustomizeTimeRuler = "CustomizeTimeRuler";
	}
	public class FormManager {
		#region Fields
		SchedulerControl control;
		FormStorage formsStorage;
		#endregion
		public FormManager(SchedulerControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.formsStorage = new FormStorage(control);
		}
		public FormStorage FormStorage { get { return formsStorage; } }
		internal SchedulerControl Control { get { return control; } set { control = value; } }
		public bool IsFormOpen { get { return FormStorage.IsFormOpen; } }
		public bool IsFormExceptReminderOpen { get { return FormStorage.IsFormExceptReminderOpen; } }
		protected internal IEnumerator GetActiveFormList() {
			return FormStorage.ActiveForms.GetEnumerator();
		}
		public virtual void ShowReminderForm(ReminderAlertNotificationCollection notifications) {
			RemindersFormEventArgs args = new RemindersFormEventArgs(notifications);
			Control.RaiseRemindersFormShowing(args);
			if (!args.Cancel)
				ShowReminderFormCore(args);
			else
				FormStorage.CloseFormByName(SchedulerFormId.Reminder);
		}
		internal virtual void ShowReminderFormCore(RemindersFormEventArgs args) {
			RemindersFormBase reminderForm = FormStorage.FindFormById(SchedulerFormId.Reminder) as RemindersFormBase;
			if (reminderForm == null) {
				reminderForm = args.Form != null ? args.Form : CreateRemindersForm(args, Control);
				SchedulerFormInfo formInfo = new SchedulerFormInfo(SchedulerFormId.Reminder, reminderForm);
				formInfo.AllowResize = args.AllowResize;
				formInfo.ShowModal = false;
				formInfo.OnCustomCloseAction = (form, dialogResult) => {
					RemindersFormBase rForm = form as RemindersFormBase;
					if (rForm != null)
						rForm.OnClose();
				};
				ShowForm(formInfo);
			}
			reminderForm.OnReminderAlert(args.AlertNotifications);
		}
		public virtual void ShowEditAppointmentForm(Appointment apt, bool readOnly, CommandSourceType commandSourceType) {
			ShowEditAppointmentForm(apt, readOnly, commandSourceType, false);
		}
		public virtual void ShowEditAppointmentForm(Appointment apt, bool readOnly, CommandSourceType commandSourceType, bool openRecurrenceForm) {
			EditAppointmentFormEventArgs args = new EditAppointmentFormEventArgs(apt, readOnly, commandSourceType);
			Control.RaiseEditAppointmentFormShowing(args);
			if (args.Cancel)
				return;
			UserControl form = args.Form != null ? args.Form : CreateAppointmentForm(apt, readOnly);
			SchedulerFormInfo formInfo = new SchedulerFormInfo(SchedulerFormId.EditAppointment, form);
			formInfo.AllowResize = args.AllowResize;
			RemindersFormBase reminderForm = FormStorage.FindFormById(SchedulerFormId.Reminder) as RemindersFormBase;
			RegisterFormOpenState(formInfo);
			if (reminderForm != null)
				ShowForm(formInfo, reminderForm);
			else
				ShowForm(formInfo, Control);
			if (openRecurrenceForm)
				Control.Dispatcher.BeginInvoke(new Action(() => ShowRecurrenceForm(form, readOnly)));
		}
		void RegisterFormOpenState(SchedulerFormInfo formInfo) {
			ISetSchedulerStateService setStateService = Control.GetService<ISetSchedulerStateService>();
			if (setStateService != null)
				setStateService.IsModalFormOpened = true;
			formInfo.OnCustomCloseAction = (formToClose, dialogResult) => {
				if (setStateService != null)
					setStateService.IsModalFormOpened = false;
			};
		}
		public virtual void ShowRecurrenceForm(UserControl parentForm, bool readOnly) {
			RecurrenceFormEventArgs args = new RecurrenceFormEventArgs(parentForm, readOnly);
			Control.RaiseRecurrenceFormShowing(args);
			if (args.Cancel)
				return;
			UserControl form = args.Form;
			if (form == null)
				form = CreateRecurrenceForm(parentForm, args.Controller, readOnly);
			if (form == null)
				return;
			SchedulerFormInfo formInfo = new SchedulerFormInfo(SchedulerFormId.EditRecurrence, form);
			FrameworkElement owner = parentForm;
			if (parentForm == null)
				owner = Control;
			ShowForm(formInfo, owner);
		}
		public virtual void ShowGotoDateForm(DateTime date) {
			GotoDateFormEventArgs args = new GotoDateFormEventArgs(Control.Views, date, Control.ActiveViewType);
			Control.RaiseGotoDateFormShowing(args);
			if (args.Cancel)
				return;
			UserControl form = CreateGotoDateForm(args);
			SchedulerFormInfo formInfo = new SchedulerFormInfo(SchedulerFormId.GoToDate, form);
			formInfo.AllowResize = args.AllowResize;
			ShowForm(formInfo);
		}
		void ShowFormCore(SchedulerFormInfo formInfo, FrameworkElement owner) {
			FloatingContainerParameters parameters = CreateFloatingContainerParameters(formInfo);
			UserControl form = formInfo.Form;
#if SL
			if (formInfo.ShowModal)
				SchedulerFormBehavior.ShowDialog(form, owner, parameters);
			else
				SchedulerFormBehavior.Show(form, owner, parameters);
#else
			SchedulerFormBehavior.ShowDialog(form, owner, parameters);
#endif
		}
		void EndFormLayout(UserControl form, SchedulerFormLayoutInfo formLayoutInfo) {
#if SL
			const int additionalWidth = 20; 
			const int additionalHeight = 70; 
			DXDialog formContainer = (DXDialog)form.Parent;
			if (form.MaxWidth == double.PositiveInfinity) {
				if (form.MinWidth != 0) {
					double actualWidth = form.MinWidth + additionalWidth;
					formContainer.MinWidth = actualWidth;
					formContainer.Width = actualWidth;
				}
			}
			if (form.MaxHeight == double.PositiveInfinity) {
				if (form.MinHeight != 0) {
					double actualHeight = form.MinHeight + additionalHeight;
					formContainer.MinHeight = actualHeight;
					formContainer.Height = actualHeight;
				}
			}
#else
			if (!formLayoutInfo.UseCustomMaxWidth)
				form.MaxWidth = double.PositiveInfinity;
			if (!formLayoutInfo.UseCustomMaxHeight)
				form.MaxHeight = double.PositiveInfinity;
#endif
		}
		SchedulerFormLayoutInfo BeginFormLayout(UserControl form) {
#if !SL
			SchedulerFormLayoutInfo result = new SchedulerFormLayoutInfo();
			result.UseCustomMaxWidth = false;
			if (form.MaxWidth == double.PositiveInfinity) {
				if (form.MinWidth != 0)
					form.MaxWidth = form.MinWidth;
			}
			else
				result.UseCustomMaxWidth = true;
			result.UseCustomMaxHeight = false;
			if (form.MaxHeight == double.PositiveInfinity) {
				if (form.MinHeight != 0)
					form.MaxHeight = form.MinHeight;
			}
			else
				result.UseCustomMaxHeight = true;
			return result;
#else
			return null;
#endif
		}
		protected void ShowForm(SchedulerFormInfo formInfo) {
			ShowForm(formInfo, Control);
		}
		protected void ShowForm(SchedulerFormInfo formInfo, FrameworkElement owner) {
			if (this.formsStorage.ActiveForms.Count > 0) 
				CloseInplaceForm();
			UserControl form = formInfo.Form;
			SetFormDataContext(form);
			SchedulerFormBehavior.SetSchedulerControl(form, Control);
			SchedulerFormLayoutInfo formLayoutInfo = BeginFormLayout(form);
			ShowFormCore(formInfo, owner);
			EndFormLayout(form, formLayoutInfo);
			FormStorage.RegisterForm(formInfo);
		}
		internal void ShowInplacementForm(UserControl form, Rect selection) {
			SchedulerFormInfo formInfo = new SchedulerFormInfo("InplaceEditor", form);
			formInfo.OnCustomCloseAction = (formToBeClose, dialogResult) => {
				Control.InplaceEditController.DoCommit();
			};
			FormStorage.RegisterForm(formInfo);
#if !SL           
			ThemeManager.SetThemeName(form, ObtainThemeName());
#endif
			SchedulerPopup popup = SchedulerPopup.Show(form, GetPopupLocation(selection));
			popup.Closed += (s,e) => { formInfo.OnClose(false); };
		}
#if !SL
		string ObtainThemeName() {
			SchedulerControl scheduler = FormStorage.SchedulerControl;
			if (scheduler == null)
				return String.Empty;
			return DevExpress.Xpf.Editors.Helpers.ThemeHelper.GetEditorThemeName(scheduler);
		}		
#endif
		internal Point GetPopupLocation(Rect bounds) {
			Point controlPoint =
#if !SL
			Control.MapPointFromScreen(new Point(0, 0));
#else
			Control.TransformToVisual(null).Inverse.Transform(new Point(0, 0));
#endif
			Point location = bounds.Location();
			location.Y -= controlPoint.Y;
			location.X -= controlPoint.X;
			if (Control.FlowDirection == FlowDirection.RightToLeft)
				location.X = -bounds.Width - location.X;
			return location;
		}
		public virtual void ShowDeleteRecurrentAppointmentForm(AppointmentBaseCollection appointments) {
			DeleteRecurrentAppointmentFormEventArgs args = new DeleteRecurrentAppointmentFormEventArgs(appointments);
			Control.RaiseDeleteRecurrentAppointmentFormShowing(args);
			if (args.Cancel)
				return;
			UserControl actualForm = args.Form;
			if (actualForm == null)
				actualForm = CreateDeleteRecurrentAppointmentForm(appointments);
			SchedulerFormInfo formInfo = new SchedulerFormInfo(SchedulerFormId.DeleteRecurrentAppointment, actualForm);
			formInfo.AllowResize = args.AllowResize;
			ShowForm(formInfo);
		}
		public virtual void ShowEditRecurrentAppointmentForm(Appointment appointment, bool readOnly, CommandSourceType commandSourceType) {
			if (appointment == null || appointment.IsBase)
				XtraScheduler.Native.Exceptions.ThrowArgumentException("appointment", appointment);
			EditRecurrentAppointmentFormEventArgs args = new EditRecurrentAppointmentFormEventArgs(appointment, readOnly, commandSourceType);
			Control.RaiseEditRecurrentAppointmentFormShowing(args);
			if (args.Cancel)
				return;
			UserControl actualForm = args.Form;
			if (actualForm == null)
				actualForm = CreateEditRecurrentAppointmentForm(args);
			SchedulerFormInfo formInfo = new SchedulerFormInfo(SchedulerFormId.EditRecurrentAppointment, actualForm);
			ShowForm(formInfo);
		}
		public virtual void ShowCustomizeTimeRulerForm(TimeRuler ruler) {
			CustomizeTimeRulerFormEventArgs args = new CustomizeTimeRulerFormEventArgs(ruler);
			args.AllowResize = false;
			Control.RaiseCustomizeTimeRulerFormShowing(args);
			if (args.Cancel)
				return;
			UserControl form = args.Form ?? CreateCustomizeTimeRulerForm(args);
			SchedulerFormInfo formInfo = new SchedulerFormInfo(SchedulerFormId.CustomizeTimeRuler, form);
			formInfo.AllowResize = args.AllowResize;
			ShowForm(formInfo);
		}
		protected internal virtual UserControl CreateGotoDateForm(GotoDateFormEventArgs args) {
			UserControl gotoDateForm = args.Form ?? new GotoDateForm(Control, args.Views, args.Date, args.SchedulerViewType);
			return gotoDateForm;
		}
		protected internal virtual AppointmentInplaceEditor CreateInplaceEditorForm(Appointment apt, Size size) {
			AppointmentInplaceEditor appointmentInplaceForm = new AppointmentInplaceEditor(Control, apt);
			appointmentInplaceForm.DataContext = appointmentInplaceForm;
			appointmentInplaceForm.SetSize(size);
			return appointmentInplaceForm;
		}
		protected internal virtual DeleteRecurrentAppointmentForm CreateDeleteRecurrentAppointmentForm(AppointmentBaseCollection appointments) {
			DeleteRecurrentAppointmentForm form = new DeleteRecurrentAppointmentForm(Control.Storage, appointments);
			form.DataContext = form;
			return form;
		}
		protected internal virtual EditRecurrentAppointmentForm CreateEditRecurrentAppointmentForm(EditAppointmentFormEventArgs args) {
			EditRecurrentAppointmentForm form = new EditRecurrentAppointmentForm(Control, args.Appointment, args.ReadOnly);
			return form;
		}
		protected internal virtual TimeRulerForm CreateCustomizeTimeRulerForm(CustomizeTimeRulerFormEventArgs args) {
			TimeRulerForm timeRulerForm = new TimeRulerForm(args.TimeRuler);
			return timeRulerForm;
		}
		protected virtual RemindersFormBase CreateRemindersForm(RemindersFormEventArgs args, FrameworkElement root) {
			return new RemindersForm(Control);
		}
		protected virtual UserControl CreateAppointmentForm(Appointment apt, bool readOnly) {
			return new AppointmentForm(Control, apt, readOnly);
		}
		protected virtual UserControl CreateRecurrenceForm(UserControl parentForm, AppointmentFormController controller, bool readOnly) {
			AppointmentFormController actualController = controller;
			if (actualController == null) {
				AppointmentForm appointmentForm = parentForm as AppointmentForm;
				if (appointmentForm != null)
					actualController = appointmentForm.Controller;
			}
			if (actualController == null)
				return null;
			return new RecurrenceForm(Control, actualController, readOnly);
		}
		protected internal virtual FloatingContainerParameters CreateFloatingContainerParameters(SchedulerFormInfo formInfo) {
			FloatingContainerParameters parameters = new FloatingContainerParameters();
			parameters.AllowSizing = formInfo.AllowResize;
			parameters.CloseOnEscape = true;
			parameters.AllowSizing = formInfo.AllowResize;
			parameters.ClosedDelegate = formInfo.OnClose;
#if !SL
			parameters.ShowModal = formInfo.ShowModal;
#endif
			return parameters;
		}
#if SL
		protected internal virtual void SubscribeFormEvents(FloatingWindowContainer form) {
			form.Hidden += new DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventHandler(FormHidden);
		}
		protected internal virtual void UnsubscribeFormEvents(FloatingWindowContainer form) {
			form.Hidden -= new DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventHandler(FormHidden);
		}
		internal void FormHidden(object sender, RoutedEventArgs e) {
			FloatingWindowContainer form = sender as FloatingWindowContainer;
			UnsubscribeFormEvents(form);
			CloseForm((UserControl)form.Content);
		}
#endif
		protected internal virtual void CloseInplaceForm() {
			this.Control.InplaceEditController.DoCommit();
			UserControl form = FormStorage.FindFormById("InplaceEditor");
			if (form != null)
				SchedulerFormBehavior.Close(form, false);
		}
		protected internal virtual void SetFormDataContext(UserControl form) {
			if (form.DataContext == null)
				form.DataContext = form;
		}
#if DEBUGTEST
		internal static UserControl GetActiveForm(SchedulerControl scheduler) {
			if (scheduler == null)
				return null;
			List<UserControl> activeForms = scheduler.FormManager.FormStorage.ActiveForms.Keys.ToList();
			int formCount = activeForms.Count;
			if (formCount < 1)
				return null;
			return activeForms[formCount - 1];
		}
		internal static void CloseAllForms(SchedulerControl scheduler) {
			if (scheduler == null)
				return;
			scheduler.FormManager.FormStorage.CloseAllForms();
		}
#endif
		internal void CloseForm(UserControl form, bool? dialogResult = false) {
			FormStorage.CloseForm(form, dialogResult);
		}
	}
	public class SchedulerFormLayoutInfo {
		public bool UseCustomMaxWidth { get; set; }
		public bool UseCustomMaxHeight { get; set; }
	}
	public class FormStorage {
		public FormStorage(SchedulerControl schedulerControl) {
			FormRegistrator = new FormRegistrator();
			ActiveForms = new Dictionary<UserControl, SchedulerFormInfo>();
			SchedulerControl = schedulerControl;
		}
		public FormRegistrator FormRegistrator { get; private set; }
		public Dictionary<UserControl, SchedulerFormInfo> ActiveForms { get; private set; }
		public SchedulerControl SchedulerControl { get; private set; }
		public bool IsFormOpen { get { return ActiveForms.Count > 0; } }
		public bool IsFormExceptReminderOpen {
			get {
				int openFormCount = ActiveForms.Count;
				if (FindFormById(SchedulerFormId.Reminder) != null)
					openFormCount--;
				return openFormCount > 0;
			}
		}
		public void RegisterForm(SchedulerFormInfo formInfo) {
			UserControl form = formInfo.Form;
			SchedulerFormBehavior.SetSchedulerControl(form, SchedulerControl);
			ActiveForms.Add(form, formInfo);
			FormRegistrator.RegisterForm(form);
			formInfo.FormStorage = this;
		}
		public void UnregisterForm(SchedulerFormInfo formInfo) {
			ActiveForms.Remove(formInfo.Form);
			FormRegistrator.UnregisterForm(formInfo.Form);
			try {
				((ILogicalOwner)SchedulerControl).RemoveChild(formInfo.Form);
			}
			catch { }
		}
		public UserControl FindFormById(string formId) {
			foreach (KeyValuePair<UserControl, SchedulerFormInfo> item in ActiveForms)
				if (item.Value.Id == formId)
					return item.Key;
			return null;
		}
		public void CloseFormByName(string formId) {
			UserControl form = FindFormById(formId);
			if (form == null)
				return;
			SchedulerFormBehavior.Close(form, false);
		}
		public void CloseAllForms() {
			List<UserControl> forms = ActiveForms.Keys.ToList();
			foreach (UserControl form in forms)
				SchedulerFormBehavior.Close(form, false);
		}
		public void CloseForm(UserControl form, bool? dialogResult) {
			if (ActiveForms.ContainsKey(form))
				SchedulerFormBehavior.Close(form, false);
		}
	}
	public class SchedulerFormInfo {
		public SchedulerFormInfo(String id, UserControl form) {
			Form = form;
			Id = id;
			ShowModal = true;
		}
		public string Id { get; set; }
		public UserControl Form { get; private set; }
		public FormStorage FormStorage { get; set; }
		public bool AllowResize { get; set; }
		public bool ShowModal { get; set; }
		public Action<UserControl, bool?> OnCustomCloseAction { get; set; }
		public void OnClose(bool? dialogResult) {
			if (!FormStorage.ActiveForms.ContainsKey(Form))
				return;
			if (OnCustomCloseAction != null)
				OnCustomCloseAction(Form, dialogResult);
			FormStorage.UnregisterForm(this);
		}
	}
}
namespace DevExpress.Xpf.Scheduler.UI {
	public class RecurrenceControlBase : UserControl, INotifyPropertyChanged, IValidatableControl {
		SchedulerRecurrenceValidator validator;
		public RecurrenceControlBase() {
			this.validator = CreateRecurrenceValidator();
		}
		protected internal SchedulerRecurrenceValidator Validator { get { return validator; } }
		#region IsReadOnly
		public bool IsReadOnly {
			get { return (bool)GetValue(IsReadOnlyProperty); }
			set { SetValue(IsReadOnlyProperty, value); }
		}
		public static readonly DependencyProperty IsReadOnlyProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<RecurrenceControlBase, bool>("IsReadOnly", false, (d, e) => d.OnIsReadOnlyChanged(e.OldValue, e.NewValue));
		protected virtual void OnIsReadOnlyChanged(bool oldValue, bool newValue) {
		}
		#endregion
		#region INotifyPropertyChanged Members
		PropertyChangedEventHandler onPropertyChanged;
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			add { onPropertyChanged += value; }
			remove { onPropertyChanged -= value; }
		}
		protected virtual void RaiseOnPropertyChange(string propertyName) {
			if (onPropertyChanged != null)
				onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		protected virtual SchedulerRecurrenceValidator CreateRecurrenceValidator() {
			return new SchedulerRecurrenceValidator();
		}
		#region IValidatableControl Members
		void IValidatableControl.ValidateValues(ValidationArgs args) {
			ValidateValues(args);
		}
		void IValidatableControl.CheckForWarnings(ValidationArgs args) {
			CheckForWarnings(args);
		}
		#endregion
		protected FrameworkElement FindElementByName(string name) {
			return LayoutHelper.FindElementByName(this, name);
		}
		protected virtual void ValidateValues(ValidationArgs args) {
		}
		protected virtual void CheckForWarnings(ValidationArgs args) {
		}
	}
	public class SchedulerFormHelper {
		public static void ValidateValues(FrameworkElement root, ValidationArgs validationArgs) {
			ValidateTree(root, validationArgs, (control, args) => control.ValidateValues(args));
		}
		public static void CheckForWarnings(FrameworkElement root, ValidationArgs validationArgs) {
			ValidateTree(root, validationArgs, (control, args) => control.CheckForWarnings(args));
		}
		static void ValidateTree(FrameworkElement root, ValidationArgs args, ValidateAction handler) {
			VisualTreeEnumerator vte = new VisualTreeEnumerator(root);
			while (vte.MoveNext()) {
				IValidatableControl control = vte.Current as IValidatableControl;
				if (control != null) {
					handler(control, args);
					if (!args.Valid)
						break;
				}
			}
		}
		delegate void ValidateAction(IValidatableControl control, ValidationArgs args);
	}
}
