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
using System.Text;
using DevExpress.Mvvm.POCO;
using DevExpress.XtraScheduler;
using DevExpress.Mvvm;
using System.Windows.Media.Imaging;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.XtraScheduler.Services;
using System.Windows.Threading;
using DevExpress.XtraScheduler.Native;
using System.Globalization;
using System.ComponentModel;
namespace DevExpress.Xpf.Scheduler.UI {
	public class ItemContainer {
		public RecurrentAppointmentAction Action { get; set; }
		public string Name { get; set; }
	}
	public abstract class ManageRecurrentAppointmentDialogViewModelBase : ViewModelBase, IDocumentContent {
		public List<UICommand> CommandsCollection { get; protected set; }
		public BitmapImage ImageSource { get; set; }
		public List<ItemContainer> ActionsItemsSource { get; set; }
		public object Title { get; protected set; }
		protected bool CanClose { get; set; }
		protected ManageRecurrentAppointmentDialogViewModelBase()
			: base() {
			ActionsItemsSource = new List<ItemContainer>();
			ActionsItemsSource.Add(new ItemContainer() { Action = RecurrentAppointmentAction.Series, Name = SchedulerControlLocalizer.GetString(GetTopCheckEditStringId()) });
			ActionsItemsSource.Add(new ItemContainer() { Action = RecurrentAppointmentAction.Occurrence, Name = SchedulerControlLocalizer.GetString(GetBottomCheckEditStringId()) });
			SelectedAction = ActionsItemsSource[1];
			CanClose = true;
			ImageSource = GetImageSource(GetImageName());
			Title = SchedulerControlLocalizer.GetString(GetTitleStringId());
			CommandsCollection = new List<UICommand>();
			if (this is IPOCOViewModel) {
				UICommand saveCommand = new UICommand() {
					Caption = SchedulerControlLocalizer.GetString(SchedulerControlStringId.ButtonCaption_OK),
					Command = this.GetCommand(x => x.Ok()),
					IsDefault = true,
					IsCancel = false,
					Id = 1
				};
				CommandsCollection.Add(saveCommand);
				UICommand cancelCommand = new UICommand() {
					Caption = SchedulerControlLocalizer.GetString(SchedulerControlStringId.ButtonCaption_Cancel),
					Command = this.GetCommand(x => x.Cancel()),
					IsCancel = true,
					Id = 1
				};
				CommandsCollection.Add(cancelCommand);
			}
		}
		string message = string.Empty;
		public string Message {
			get { return message; }
			set {
				if (message != value) {
					message = value;
					RaisePropertyChanged("Message");
				}
			}
		}
		ItemContainer selectedAction = null;
		public ItemContainer SelectedAction {
			get { return selectedAction; }
			set {
				if (selectedAction != value) {
					selectedAction = value;
					RaisePropertyChanged("SelectedAction");
				}
			}
		}
		public RecurrentAppointmentAction Action { get { return SelectedAction == null ? RecurrentAppointmentAction.Occurrence : SelectedAction.Action; } }
		Appointment currentAppointment;
		public Appointment CurrentAppointment {
			get { return currentAppointment; }
			set {
				if (currentAppointment != value) {
					currentAppointment = value;
					if (currentAppointment != null)
						Message = String.Format(SchedulerControlLocalizer.GetString(GetMessageStringId()), CurrentAppointment.Subject);
				}
			}
		}
		public bool Close() {
			if (!CanClose) {
				CanClose = true;
				return false;
			}
			return true;
		}
		protected abstract string GetImageName();
		protected abstract SchedulerControlStringId GetMessageStringId();
		protected abstract SchedulerControlStringId GetTitleStringId();
		protected abstract SchedulerControlStringId GetTopCheckEditStringId();
		protected abstract SchedulerControlStringId GetBottomCheckEditStringId();
		protected virtual void OkActions() { }
		protected virtual void CancelActions() { }
		BitmapImage GetImageSource(string name) {
			return DevExpress.Xpf.Core.Native.ImageHelper.CreateImageFromEmbeddedResource(System.Reflection.Assembly.GetExecutingAssembly(), "DevExpress.Xpf.Scheduler.Images." + name);
		}
		public void Ok() {
			OkActions();
		}
		public void Cancel() {
			CancelActions();
		}
#if DEBUGTEST && !SL
		public void CloseForm() {
			ICurrentWindowService service = GetService<ICurrentWindowService>();
			if (service != null) {
#if DEBUGTEST
				if (DevExpress.Xpf.Scheduler.Tests.TestFormHelper.ActiveViewModels.ContainsKey(this))
					DevExpress.Xpf.Scheduler.Tests.TestFormHelper.ActiveViewModels.Remove(this);
#endif
				service.Close();
			}
		}
#endif
		protected IDocumentOwner DocumentOwner { get; private set; }
		#region IDocumentContent
		void IDocumentContent.OnClose(CancelEventArgs e) {
			e.Cancel = !Close();
		}
		void IDocumentContent.OnDestroy() { }
		IDocumentOwner IDocumentContent.DocumentOwner {
			get { return DocumentOwner; }
			set { DocumentOwner = value; }
		}
		#endregion
	}
	public class DeleteRecurrentAppointmentDialogViewModel : ManageRecurrentAppointmentDialogViewModelBase {
		protected List<Appointment> Appointments { get; set; }
		protected SchedulerStorage Storage { get; set; }
		public DeleteRecurrentAppointmentDialogViewModel(SchedulerStorage storage, AppointmentBaseCollection apts)
			: base() {
			Storage = storage;
			Appointments = new List<Appointment>();
			Appointments.AddRange(apts);
			FetchAppointmentForProcessing();
		}
		public static DeleteRecurrentAppointmentDialogViewModel Create(SchedulerStorage storage, AppointmentBaseCollection apts) {
			return ViewModelSource.Create(() => new DeleteRecurrentAppointmentDialogViewModel(storage, apts));
		}
		bool FetchAppointmentForProcessing() {
			while (Appointments.Count > 0) {
				Appointment appointment = Appointments[0];
				Appointments.Remove(appointment);
				if (appointment.RecurrencePattern != null) {
					CurrentAppointment = appointment;
					return true;
				}
			}
			return false;
		}
		void DeleteAppointments() {
			if (Action == RecurrentAppointmentAction.Occurrence)
				CurrentAppointment.Delete();
			else if (Action == RecurrentAppointmentAction.Series) {
				Appointment pattern = CurrentAppointment.RecurrencePattern;
				pattern.Delete();
				RemoveAppointmentsRelatedToPattern(pattern);
			}
		}
		protected virtual void RemoveAppointmentsRelatedToPattern(Appointment pattern) {
			int count = Appointments.Count;
			for (int i = count - 1; i >= 0; i--) {
				Appointment apt = Appointments[i];
				if (apt.RecurrencePattern == pattern)
					Appointments.RemoveAt(i);
			}
		}
		protected override string GetImageName() {
			return "Warning.png";
		}
		protected override SchedulerControlStringId GetMessageStringId() {
			return SchedulerControlStringId.Form_DeleteRecurrentAppointmentFormMessage;
		}
		protected override SchedulerControlStringId GetTitleStringId() {
			return SchedulerControlStringId.Caption_DeleteRecurrentApt;
		}
		protected override SchedulerControlStringId GetTopCheckEditStringId() {
			return SchedulerControlStringId.Form_RecurrentAppointmentAction_DeleteSeries;
		}
		protected override SchedulerControlStringId GetBottomCheckEditStringId() {
			return SchedulerControlStringId.Form_RecurrentAppointmentAction_DeleteOccurrence;
		}
		protected override void OkActions() {
			if (!Storage.IsUpdateLocked)
				Storage.BeginUpdate();
			DeleteAppointments();
			if (!FetchAppointmentForProcessing()) {
				if (Storage.IsUpdateLocked)
					Storage.EndUpdate();
			} else {
				CanClose = false;
			}
		}
		protected override void CancelActions() {
			if (FetchAppointmentForProcessing())
				CanClose = false;
		}
	}
	public class EditRecurrentAppointmentDialogViewModel : ManageRecurrentAppointmentDialogViewModelBase {
		protected SchedulerControl Control { get; set; }
		protected bool ReadOnly { get; set; }
		public EditRecurrentAppointmentDialogViewModel(SchedulerControl schedulerControl, Appointment apt, bool readOnly)
			: base() {
			Guard.ArgumentNotNull(schedulerControl, "control");
			Guard.ArgumentNotNull(apt, "appointment");
			Control = schedulerControl;
			CurrentAppointment = apt;
			ReadOnly = readOnly;
		}
		public static EditRecurrentAppointmentDialogViewModel Create(SchedulerControl schedulerControl, Appointment apt, bool readOnly) {
			return ViewModelSource.Create(() => new EditRecurrentAppointmentDialogViewModel(schedulerControl, apt, readOnly));
		}
		protected override string GetImageName() {
			return "Question.png";
		}
		protected override SchedulerControlStringId GetMessageStringId() {
			return SchedulerControlStringId.Form_EditRecurrentAppointmentFormMessage;
		}
		protected override SchedulerControlStringId GetTitleStringId() {
			return SchedulerControlStringId.Caption_OpenRecurrentApt;
		}
		protected override SchedulerControlStringId GetTopCheckEditStringId() {
			return SchedulerControlStringId.Form_RecurrentAppointmentAction_EditSeries;
		}
		protected override SchedulerControlStringId GetBottomCheckEditStringId() {
			return SchedulerControlStringId.Form_RecurrentAppointmentAction_EditOccurrence;
		}
		protected override void OkActions() {
			Appointment actualAppointment = Action == RecurrentAppointmentAction.Series ? CurrentAppointment.RecurrencePattern : CurrentAppointment;
			if (Action == RecurrentAppointmentAction.Series)
				Control.ShowEditAppointmentForm(actualAppointment, ReadOnly, CommandSourceType.Unknown, true);
			else
				Control.ShowEditAppointmentForm(actualAppointment, ReadOnly, CommandSourceType.Unknown);
		}
	}
	public class GotoDateViewModel : ViewModelBase, IDocumentContent {
		public List<UICommand> CommandsCollection { get; protected set; }
		public object Title { get; protected set; }
		protected SchedulerControl Control { get; set; }
		public NamedElementList ActualViews { get; set; }
		public GotoDateViewModel(SchedulerControl control, SchedulerViewRepository views, DateTime date, SchedulerViewType viewType)
			: base() {
			Guard.ArgumentNotNull(control, "control");
			Title = SchedulerControlLocalizer.GetString(SchedulerControlStringId.Caption_GotoDate);
			this.Control = control;
			this.Date = date;
			TargetViewType = viewType;
			PopulateActiveViews(views);
			CommandsCollection = new List<UICommand>();
			if (this is IPOCOViewModel) {
				UICommand saveCommand = new UICommand() {
					Caption = SchedulerControlLocalizer.GetString(SchedulerControlStringId.ButtonCaption_OK),
					Command = this.GetCommand(x => x.Ok()),
					IsCancel = false,
					IsDefault = true,
					Id = 1
				};
				CommandsCollection.Add(saveCommand);
				UICommand cancelCommand = new UICommand() {
					Caption = SchedulerControlLocalizer.GetString(SchedulerControlStringId.ButtonCaption_Cancel),
					Command = this.GetCommand(x => x.Cancel()),
					IsCancel = true,
					IsDefault = false,
					Id = 1
				};
				CommandsCollection.Add(cancelCommand);
			}
		}
		public static GotoDateViewModel Create(SchedulerControl control, SchedulerViewRepository views, DateTime date, SchedulerViewType viewType) {
			return ViewModelSource.Create(() => new GotoDateViewModel(control, views, date, viewType));
		}
		DateTime date;
		public DateTime Date {
			get { return date; }
			set {
				if (date != value) {
					date = value;
					RaisePropertyChanged("Date");
				}
			}
		}
		SchedulerViewType targetViewType;
		public SchedulerViewType TargetViewType {
			get { return targetViewType; }
			set {
				if (targetViewType != value) {
					targetViewType = value;
					RaisePropertyChanged("TargetViewType");
				}
			}
		}
		public void PopulateActiveViews(SchedulerViewRepository views) {
			ActualViews = new NamedElementList();
			int count = views.Count;
			for (int i = 0; i < count; i++) {
				SchedulerViewBase view = views[i];
				if (view.Enabled)
					ActualViews.Add(new NamedElement(view.Type, view.DisplayName));
			}
		}
		public void GoToDate(DateTime date, SchedulerViewType viewType) {
			IDateTimeNavigationService service = (IDateTimeNavigationService)Control.GetService(typeof(IDateTimeNavigationService));
			if (service != null)
				service.GoToDate(date, viewType);
		}
		public bool Close() {
			return true;
		}
		public void Ok() {
			GoToDate(Date, TargetViewType);
		}
		public void Cancel() {
		}
#if DEBUGTEST && !SL
		public void CloseForm() {
			ICurrentWindowService service = GetService<ICurrentWindowService>();
			if (service != null) {
#if DEBUGTEST
				if (DevExpress.Xpf.Scheduler.Tests.TestFormHelper.ActiveViewModels.ContainsKey(this))
					DevExpress.Xpf.Scheduler.Tests.TestFormHelper.ActiveViewModels.Remove(this);
#endif
				service.Close();
			}
		}
#endif
		protected IDocumentOwner DocumentOwner { get; private set; }
		#region IDocumentContent
		void IDocumentContent.OnClose(CancelEventArgs e) {
			e.Cancel = !Close();
		}
		void IDocumentContent.OnDestroy() { }
		IDocumentOwner IDocumentContent.DocumentOwner {
			get { return DocumentOwner; }
			set { DocumentOwner = value; }
		}
		#endregion
	}
	public class TimeRulerViewModel : ViewModelBase, IDocumentContent {
		public static TimeRulerViewModel Create(TimeRuler timeRuler) {
			return ViewModelSource.Create(() => new TimeRulerViewModel(timeRuler));
		}
		public TimeRulerViewModel(TimeRuler timeRuler)
			: base() {
			TimeRuler = timeRuler;
			TimeZoneId = timeRuler.TimeZoneId;
			Caption = timeRuler.Caption;
			AdjustForDaylightSavingTime = TimeRuler.AdjustForDaylightSavingTime;
			Title = SchedulerControlLocalizer.GetString(SchedulerControlStringId.Caption_TimeRuler);
			InitializeTimer();
			UpdateCurrentTime();
			CommandsCollection = new List<UICommand>();
			if (this is IPOCOViewModel) {
				UICommand saveCommand = new UICommand() {
					Caption = SchedulerControlLocalizer.GetString(SchedulerControlStringId.ButtonCaption_OK),
					Command = this.GetCommand(x => x.Ok()),
					IsDefault = true,
					IsCancel = false,
					Id = 1
				};
				CommandsCollection.Add(saveCommand);
				UICommand cancelCommand = new UICommand() {
					Caption = SchedulerControlLocalizer.GetString(SchedulerControlStringId.ButtonCaption_Cancel),
					Command = this.GetCommand(x => x.Cancel()),
					IsCancel = true,
					Id = 2
				};
				CommandsCollection.Add(cancelCommand);
			}
		}
		public TimeRuler TimeRuler { get; protected set; }
		public List<UICommand> CommandsCollection { get; protected set; }
		public object Title { get; protected set; }
		protected DispatcherTimer Timer { get; set; }
		#region Caption
		string caption = string.Empty;
		public string Caption {
			get { return caption; }
			set {
				if (caption != value) {
					caption = value;
					RaisePropertiesChanged("Caption");
				}
			}
		}
		#endregion
		#region CurrentTime
		string currentTime = string.Empty;
		public string CurrentTime {
			get { return currentTime; }
			set {
				if (currentTime != value) {
					currentTime = value;
					RaisePropertiesChanged("CurrentTime");
				}
			}
		}
		#endregion
		#region TimeZoneId
		string timeZoneId = string.Empty;
		public string TimeZoneId {
			get { return timeZoneId; }
			set {
				if (timeZoneId != value) {
					timeZoneId = value;
					RaisePropertiesChanged("TimeZoneId");
					UpdateCurrentTime();
				}
			}
		}
		#endregion
		#region AdjustForDaylightSavingTime
		bool adjustForDaylightSavingTime;
		public bool AdjustForDaylightSavingTime {
			get { return adjustForDaylightSavingTime; }
			set {
				if (adjustForDaylightSavingTime != value) {
					adjustForDaylightSavingTime = value;
					RaisePropertiesChanged("AdjustForDaylightSavingTime");
					UpdateCurrentTime();
				}
			}
		}
		#endregion
		void InitializeTimer() {
			Timer = new DispatcherTimer();
			Timer.Interval = TimeSpan.FromMinutes(1);
			Timer.Tick += new EventHandler(OnTimerTick);
			Timer.Start();
		}
		void OnTimerTick(object sender, EventArgs e) {
			UpdateCurrentTime();
		}
		void UpdateCurrentTime() {
			if (TimeRuler == null)
				return;
			TimeZoneInfo tz = GetSelectedTimeZone();
			if (tz == null)
				return;
			DateTime now = DateTime.Now;
			DateTime convertedTime = TimeZoneInfo.ConvertTime(now, TimeZoneInfo.Local, TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId));
			CurrentTime = String.Format("{0} {1}", convertedTime.ToShortDateString(), DateTimeFormatHelper.DateToShortTimeString(convertedTime));
		}
		TimeZoneInfo GetSelectedTimeZone() {
			TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
			if (tz == null)
				tz = TimeZoneInfo.FindSystemTimeZoneById(TimeRuler.TimeZoneId);
			return tz;
		}
		public bool Close() {
			return true;
		}
		public void Ok() {
			if (TimeRuler != null) {				
				TimeRuler.Caption = Caption;
				TimeRuler.AdjustForDaylightSavingTime = AdjustForDaylightSavingTime;
				TimeRuler.TimeZoneId = TimeZoneId;
			}
		}
		public void Cancel() { }
#if DEBUGTEST && !SL
		public void CloseForm() {
			ICurrentWindowService service = GetService<ICurrentWindowService>();
			if (service != null) {
#if DEBUGTEST
				if (DevExpress.Xpf.Scheduler.Tests.TestFormHelper.ActiveViewModels.ContainsKey(this))
					DevExpress.Xpf.Scheduler.Tests.TestFormHelper.ActiveViewModels.Remove(this);
#endif
				service.Close();
			}
		}
#endif
		protected IDocumentOwner DocumentOwner { get; private set; }
		#region IDocumentContent
		void IDocumentContent.OnClose(CancelEventArgs e) {
			e.Cancel = !Close();
		}
		void IDocumentContent.OnDestroy() { }
		IDocumentOwner IDocumentContent.DocumentOwner {
			get { return DocumentOwner; }
			set { DocumentOwner = value; }
		}
		#endregion
	}
}
