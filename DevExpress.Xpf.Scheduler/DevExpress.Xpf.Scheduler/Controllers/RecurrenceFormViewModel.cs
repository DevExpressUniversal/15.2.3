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
using System.Windows.Controls;
using System.Windows;
using DevExpress.XtraScheduler;
using System.ComponentModel;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.UI;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Xpf.Scheduler.Native;
using System.Windows.Data;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.XtraScheduler.Native;
using System.Collections.ObjectModel;
using System.Reflection;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.Xpf.Scheduler.UI {
	public class RecurrenceDialogViewModel : ViewModelBase, IDocumentContent {
		IMessageBoxService actualMessageBoxService;
		internal static RecurrenceTypeElement RecurrenceNone = new RecurrenceTypeElement(null);
		internal static readonly RecurrenceType[] RecurrenceTypeOrder = new RecurrenceType[] {XtraScheduler.RecurrenceType.Minutely, 
																			XtraScheduler.RecurrenceType.Hourly, 
																			XtraScheduler.RecurrenceType.Daily, 
																			XtraScheduler.RecurrenceType.Weekly,
																			XtraScheduler.RecurrenceType.Monthly,
																			XtraScheduler.RecurrenceType.Yearly
																			};
		internal static RecurrenceType[] AvailableRecurrenceTypesByDefault = new RecurrenceType[] { XtraScheduler.RecurrenceType.Daily, 
																							XtraScheduler.RecurrenceType.Weekly,
																							XtraScheduler.RecurrenceType.Monthly,
																							XtraScheduler.RecurrenceType.Yearly};
		public static RecurrenceDialogViewModel Create(AppointmentFormViewModel viewModel) {
			return RecurrenceDialogViewModel.Create(viewModel, false);
		}
		public static RecurrenceDialogViewModel Create(AppointmentFormViewModel viewModel, bool readOnly) {
			return ViewModelSource.Create(() => new RecurrenceDialogViewModel(viewModel, readOnly));
		}
		public static RecurrenceDialogViewModel Create(AppointmentFormViewModel viewModel, XtraScheduler.RecurrenceType[] availableRecurrenceTypes) {
			return RecurrenceDialogViewModel.Create(viewModel, availableRecurrenceTypes, false);
		}
		public static RecurrenceDialogViewModel Create(AppointmentFormViewModel viewModel, XtraScheduler.RecurrenceType[] availableRecurrenceTypes, bool readOnly) {
			return ViewModelSource.Create(() => new RecurrenceDialogViewModel(viewModel, availableRecurrenceTypes, readOnly));
		}
		public RecurrenceDialogViewModel(AppointmentFormViewModel viewModel) 
			: this(viewModel, AvailableRecurrenceTypesByDefault, false) { 
		}
		public RecurrenceDialogViewModel(AppointmentFormViewModel viewModel, bool readOnly) 
			: this(viewModel, AvailableRecurrenceTypesByDefault, readOnly) { 
		}
		public RecurrenceDialogViewModel(AppointmentFormViewModel viewModel, XtraScheduler.RecurrenceType[] availableRecurrenceTypes)
			: this(viewModel, availableRecurrenceTypes, false) { 
		}
		public RecurrenceDialogViewModel(AppointmentFormViewModel viewModel, XtraScheduler.RecurrenceType[] availableRecurrenceTypes, bool readOnly) 
			: base() {
			AvailableRecurrenceTypes = new ObservableCollection<RecurrenceType>();
			foreach (var recurrenceType in availableRecurrenceTypes)
				AvailableRecurrenceTypes.Add(recurrenceType);
			AvailableRecurrenceTypes.CollectionChanged += OnAvailableRecurrenceTypesCollectionChanged;
			AppointmentViewModel = viewModel;
			IsReadOnly = readOnly;
			RecreateSources();
			FirstDayOfWeek = GetActualFirstDayOfWeek(AppointmentViewModel.Control);
			InitializeRecurrenceType(AppointmentViewModel);
			AppointmentViewModel.PropertyChanged += OnViewModelPropertyChanged;
			CommandsCollection = new List<UICommand>();
			if(this is IPOCOViewModel) {
				UICommand saveCommand = new UICommand() {
					Caption = SchedulerControlLocalizer.GetString(SchedulerControlStringId.ButtonCaption_OK),
					Command = this.GetCommand(x => x.SaveRecurrence()),
					IsCancel = true,
					Id = 1
				};
				CommandsCollection.Add(saveCommand);
				UICommand cancelCommand = new UICommand() {
					Caption = SchedulerControlLocalizer.GetString(SchedulerControlStringId.ButtonCaption_Cancel),
					Command = this.GetCommand(x => x.CancelEditing()),
					IsCancel = true,
					Id = 1
				};
				CommandsCollection.Add(cancelCommand);
				UICommand deleteCommand = new UICommand() {
					Caption = SchedulerControlLocalizer.GetString(SchedulerControlStringId.ButtonCaption_Delete),
					Command = this.GetCommand(x => x.DeleteRecurrence()),
					IsCancel = true,
					Id = 1
				};
				CommandsCollection.Add(deleteCommand);
			}
		}
		#region Properties
		public TimeZoneHelper TimeZoneHelper { get { return AppointmentViewModel.TimeZoneHelper; } }
		public List<UICommand> CommandsCollection { get; protected set; }
		public bool IsReadOnly { get; set; }
		public Appointment PatternCopy { get { return AppointmentViewModel.PatternCopy; } }
		public object Title {
			get { return SchedulerControlLocalizer.GetString(SchedulerControlStringId.Form_Recurrence); }
		}
		#region FirstDayOfWeek
		DayOfWeek firstDayOfWeek;
		public DayOfWeek FirstDayOfWeek {
			get { return firstDayOfWeek; }
			set {
				if (firstDayOfWeek != value) {
					firstDayOfWeek = value;
					RaisePropertyChanged("FirstDayOfWeek");
				}
			}
		}
		#endregion
		#region RecurrenceElement
		RecurrenceTypeElement recurrenceElement;
		public RecurrenceTypeElement RecurrenceElement {
			get { return recurrenceElement; }
			set {
				if (recurrenceElement != value) {
					recurrenceElement = value;
					RaisePropertyChanged("RecurrenceElement");
					OnRecurrenceElementChanged(recurrenceElement);
				}
			}
		}
		#endregion
		public bool ShouldShowRecurrence { get { return !AppointmentViewModel.SourceAppointment.IsOccurrence && AppointmentViewModel.ShouldShowRecurrenceButton; } }
		#region EnableRecurrence
		bool enableRecurrence = false;
		public bool EnableRecurrence {
			get { return enableRecurrence; }
			set {
				if (enableRecurrence == value)
					return;
				BeginInternalUpdate();
				enableRecurrence = value;
				if (value == true) {
					if (!RecurrenceType.HasValue) {
						foreach (RecurrenceType type in RecurrenceTypeOrder)
							if (AvailableRecurrenceTypes.Contains(type)) {
								RecurrenceType = type;
								break;
							}
					}
				}
				SyncRecurrenceTypeWithOtherProperties();
				RaisePropertiesUpdated();
				EndInternalUpdate();
			}
		}
		#endregion
		public AppointmentFormViewModel AppointmentViewModel { get; protected set; }
		public ObservableCollection<RecurrenceType> AvailableRecurrenceTypes { get; set; }
		public ObservableCollection<RecurrenceTypeElement> RecurrenceElements { get; protected set; }
		public List<IRecurrenceInfo> RecurrenceInfos { get; protected set; }
		public IRecurrenceInfo RecurrenceInfo {
			get {
				if (!RecurrenceType.HasValue || !EnableRecurrence)
					return null;
				return RecurrenceInfos.FirstOrDefault(ri => ri.Type == RecurrenceType.Value);
			}
		}
		#region RecurrenceType
		RecurrenceType? recurrenceType;
		public RecurrenceType? RecurrenceType {
			get { return recurrenceType; }
			set {
				recurrenceType = value;
				SyncRecurrenceTypeWithOtherProperties();
				RaisePropertiesUpdated();
			}
		}
		#endregion
		public bool IsDailyRecurrence { get { return RecurrenceType == XtraScheduler.RecurrenceType.Daily; } }
		public bool IsWeeklyRecurrence { get { return RecurrenceType == XtraScheduler.RecurrenceType.Weekly; } }
		public bool IsMonthlyRecurrence { get { return RecurrenceType == XtraScheduler.RecurrenceType.Monthly; } }
		public bool IsYearlyRecurrence { get { return RecurrenceType == XtraScheduler.RecurrenceType.Yearly; } }
		public bool IsHourlyRecurrence { get { return RecurrenceType == XtraScheduler.RecurrenceType.Hourly; } }
		public bool IsMinutelyRecurrence { get { return RecurrenceType == XtraScheduler.RecurrenceType.Minutely; } }
		public bool IsRecurrence { get { return RecurrenceType.HasValue; } }
		public bool EnableDailyRecurrence {
			get { return AvailableRecurrenceTypes.Contains(XtraScheduler.RecurrenceType.Daily); }
			set { EnableRecurrenceType(XtraScheduler.RecurrenceType.Daily, value); }
		}
		public bool EnableWeeklyRecurrence {
			get { return AvailableRecurrenceTypes.Contains(XtraScheduler.RecurrenceType.Weekly); }
			set { EnableRecurrenceType(XtraScheduler.RecurrenceType.Weekly, value); }
		}
		public bool EnableMonthlyRecurrence {
			get { return AvailableRecurrenceTypes.Contains(XtraScheduler.RecurrenceType.Monthly); }
			set { EnableRecurrenceType(XtraScheduler.RecurrenceType.Monthly, value); }
		}
		public bool EnableYearlyRecurrence {
			get { return AvailableRecurrenceTypes.Contains(XtraScheduler.RecurrenceType.Yearly); }
			set { EnableRecurrenceType(XtraScheduler.RecurrenceType.Yearly, value); }
		}
		public bool EnableHourlyRecurrence {
			get { return AvailableRecurrenceTypes.Contains(XtraScheduler.RecurrenceType.Hourly); }
			set { EnableRecurrenceType(XtraScheduler.RecurrenceType.Hourly, value); }
		}
		public bool EnableMinutelyRecurrence {
			get { return AvailableRecurrenceTypes.Contains(XtraScheduler.RecurrenceType.Minutely); }
			set { EnableRecurrenceType(XtraScheduler.RecurrenceType.Minutely, value); }
		}
		#region EnableNoneRecurrence
		bool enableNoneRecurrence = true;
		public bool EnableNoneRecurrence {
			get { return enableNoneRecurrence; }
			set {
				if (EnableNoneRecurrence == value)
					return;
				BeginInternalUpdate();
				enableNoneRecurrence = value;
				RecreateSources();
				SyncRecurrenceTypeWithOtherProperties();
				RaisePropertiesUpdated();
				EndInternalUpdate();
			}
		}
		#endregion
		#region InternalUpdate
		bool internalUpdate = false;
		bool InternalUpdate {
			get { return internalUpdate; }
		}
		#endregion
		protected IDocumentOwner DocumentOwner { get; private set; }
		public virtual IMessageBoxService MessageBoxService { get { return null; } }
		protected IMessageBoxService ActualMessageBoxService {
			get {
				if (MessageBoxService != null)
					return MessageBoxService;
				if (this.actualMessageBoxService == null)
					this.actualMessageBoxService = new DevExpress.Xpf.Core.DXMessageBoxService();
				return this.actualMessageBoxService;
			}
		}
		#endregion
		#region Commands
		public void SaveRecurrence() {
			ApplyRecurrence();
		}
		public bool CanSaveRecurrence() {
			return !IsReadOnly;
		}
		public void CancelEditing() {
		}
		public void DeleteRecurrence() {
			AppointmentViewModel.RemoveRecurrence();
		}
		public bool CanDeleteRecurrence() {
			return !IsReadOnly;
		}
		#endregion
		public bool Close() {
			return true;
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
		void OnAvailableRecurrenceTypesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			if (!RecurrenceType.HasValue )
				return;
			if (AvailableRecurrenceTypes.Contains(RecurrenceType.Value) || AvailableRecurrenceTypes.Count == 0)
				return;
			RecurrenceType = AvailableRecurrenceTypes[0];
		}
		void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "Start") {
				SynchronizeRecurrenceInfoStart(AppointmentViewModel.TimeZoneHelper.FromClientTime(AppointmentViewModel.Start, AppointmentViewModel.TimeZoneId));
			}
		}
		protected virtual void SynchronizeRecurrenceInfoStart(DateTime start) {
			if (RecurrenceInfo != null && RecurrenceInfo.Start != start) {
				RecurrenceInfo.Start = start;
			}
		}
		DayOfWeek GetActualFirstDayOfWeek(SchedulerControl schedulerControl) {
			if (schedulerControl.OptionsView.FirstDayOfWeek == XtraScheduler.FirstDayOfWeek.System)
				return DevExpress.XtraScheduler.Native.DateTimeHelper.FirstDayOfWeek;
			return DevExpress.XtraScheduler.Native.DateTimeHelper.ConvertFirstDayOfWeek(schedulerControl.OptionsView.FirstDayOfWeek);
		}
		void OnRecurrenceElementChanged(RecurrenceTypeElement newValue) {
			if (InternalUpdate)
				return;
			if (newValue != null && newValue.RecurrenceType.HasValue && AvailableRecurrenceTypes.Contains(newValue.RecurrenceType.Value)) {
				this.enableRecurrence = true;
				RecurrenceType = newValue.RecurrenceType.Value;
			} else {
				this.enableRecurrence = false;
				RecurrenceType = null;
			}
			SynchronizeRecurrenceInfoStart(AppointmentViewModel.TimeZoneHelper.FromClientTime(AppointmentViewModel.Start, AppointmentViewModel.TimeZoneId));
			ApplyRecurrence();
			RaisePropertiesUpdated();
		}
		protected virtual void InitializeRecurrenceType(AppointmentFormViewModel controller) {
			EnableRecurrence = true;
			RecurrenceType = controller.PatternCopy.RecurrenceInfo.Type;
		}
		void BeginInternalUpdate() {
			this.internalUpdate = true;
		}
		void EndInternalUpdate() {
			this.internalUpdate = false;
		}
		void EnableRecurrenceType(RecurrenceType recurrenceType, bool value) {
			BeginInternalUpdate();
			if(AvailableRecurrenceTypes.Contains(recurrenceType) == value)
				return;
			if(value == true)
				AvailableRecurrenceTypes.Add(recurrenceType);
			else
				AvailableRecurrenceTypes.Remove(recurrenceType);
			RecreateSources();
			SyncRecurrenceTypeWithOtherProperties();
			RaisePropertiesUpdated();
			EndInternalUpdate();
		}
		void RecreateSources() {
			RecurrenceElements = CreateRecurrenceElements();
			RecurrenceInfos = CreateRecurrenceInfos(CreateNewEditedAppointmentPattern(AppointmentViewModel.EditedAppointmentCopy));
			RaiseSourcePropertiesUpdated();
		}
		Appointment CreateNewEditedAppointmentPattern(Appointment apt) {
			Appointment patternCopy = apt.Copy();
			((IInternalAppointment)patternCopy).SetTypeCore(AppointmentType.Pattern);
			patternCopy.RecurrenceInfo.OccurrenceCount = 10;
			patternCopy.RecurrenceInfo.FirstDayOfWeek = FirstDayOfWeek;
			return patternCopy;
		}
		void SyncRecurrenceTypeWithOtherProperties() {
			RecurrenceTypeElement result = RecurrenceNone;
			if(!EnableNoneRecurrence)
				result = RecurrenceElements[0];
			if(RecurrenceType.HasValue && EnableRecurrence) {
				foreach(RecurrenceTypeElement item in RecurrenceElements) {
					if(item.RecurrenceType == RecurrenceType.Value) {
						result = item;
						break;
					}
				}
			}
			RecurrenceElement = result;
		}
		void RaisePropertiesUpdated() {
			RaisePropertyChanged("IsDailyRecurrence");
			RaisePropertyChanged("IsWeeklyRecurrence");
			RaisePropertyChanged("IsMonthlyRecurrence");
			RaisePropertyChanged("IsYearlyRecurrence");
			RaisePropertyChanged("IsRecurrence");
			RaisePropertyChanged("RecurrenceInfo");
			RaisePropertyChanged("EnableRecurrence");
		}
		void RaiseSourcePropertiesUpdated() {
			RaisePropertyChanged("RecurrenceElements");
			RaisePropertyChanged("RecurrenceInfos");
		}
		ObservableCollection<RecurrenceTypeElement> CreateRecurrenceElements() {
			ObservableCollection<RecurrenceTypeElement> elements = new ObservableCollection<RecurrenceTypeElement>();
			if(EnableNoneRecurrence)
				elements.Add(RecurrenceNone);
			int count = RecurrenceTypeOrder.Length;
			for(int i = 0; i < count; i++) {
				RecurrenceType currentType = RecurrenceTypeOrder[i];
				if(AvailableRecurrenceTypes.Contains(currentType))
					elements.Add(new RecurrenceTypeElement(currentType));
			}
			return elements;
		}
		List<IRecurrenceInfo> CreateRecurrenceInfos(Appointment patternCopy) {
			List<IRecurrenceInfo> result = new List<IRecurrenceInfo>();
			int count = RecurrenceTypeOrder.Length;
			for(int i = 0; i < count; i++) {
				RecurrenceType currentType = RecurrenceTypeOrder[i];
				if(AvailableRecurrenceTypes.Contains(currentType))
					result.Add(CreateRecurrenceInfo(patternCopy, currentType));
			}
			return result;
		}
		protected IRecurrenceInfo CreateRecurrenceInfo(Appointment patternCopy, RecurrenceType type) {
			IRecurrenceInfo patternRecurrenceInfo = patternCopy.RecurrenceInfo;
			IRecurrenceInfo ri = new RecurrenceInfo();
			if(patternRecurrenceInfo != null)
				ri.Assign(patternRecurrenceInfo);
			ri.Type = type;
			((IInternalRecurrenceInfo)ri).UpdateRange(ri.Start, ri.End, ri.Range, ri.OccurrenceCount, patternCopy);
			return ri;
		}
		public void ApplyRecurrence() {
			if (AppointmentViewModel.AreExceptionsPresent()) {
				MessageResult result = ActualMessageBoxService.ShowMessage(SchedulerLocalizer.GetString(SchedulerStringId.Msg_RecurrenceExceptionsWillBeLost), Assembly.GetEntryAssembly().GetName().Name, MessageButton.YesNo, MessageIcon.Exclamation);
				if (result == MessageResult.No)
					return;
			}
			AppointmentViewModel.ApplyRecurrenceInfo(RecurrenceInfo);			
		}
		bool CalcCanChangeAllDay() {
			return !RecurrenceType.HasValue;
		}		
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
