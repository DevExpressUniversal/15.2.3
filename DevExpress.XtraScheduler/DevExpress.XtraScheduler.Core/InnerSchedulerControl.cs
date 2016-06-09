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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using DevExpress.Services.Internal;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraScheduler.Services.Implementation;
using DevExpress.XtraScheduler.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Schedule;
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.XtraScheduler.Internal;
#if !SL
using System.Windows.Forms;
#else
using System.Windows.Media;
using DevExpress.Data;
#endif
namespace DevExpress.XtraScheduler.Native {
	#region IInnerSchedulerControlOwner
	public interface IInnerSchedulerControlOwner : ICommandAwareControl<SchedulerCommandId> {
		SchedulerViewRepositoryBase CreateViewRepository();
		SchedulerOptionsBehaviorBase CreateOptionsBehavior();
		SchedulerOptionsViewBase CreateOptionsView();
		AppointmentSelectionController CreateAppointmentSelectionController();
		ISchedulerInplaceEditController CreateInplaceEditController();
		NormalKeyboardHandlerBase CreateKeyboardHandler();
		SchedulerMouseHandler CreateMouseHandler(InnerSchedulerControl control);
		AppointmentChangeHelper CreateAppointmentChangeHelper(InnerSchedulerControl innerSchedulerControl);
		ISupportAppointmentEdit SupportAppointmentEdit { get; }
		ISupportAppointmentDependencyEdit SupportAppointmentDependencyEdit { get; }
		bool QueryDeleteForEachRecurringAppointment { get; }
		void UpdatePaintStyle();
		bool IsDateTimeScrollbarVisibilityDependsOnClientSize();
		void RecalcClientBounds();
		bool ChangeResourceScrollBarOrientationIfNeeded();
		bool ChangeDateTimeScrollBarOrientationIfNeeded();
		bool ChangeResourceScrollBarVisibilityIfNeeded();
		bool ChangeDateTimeScrollBarVisibilityIfNeeded();
		void RecalcViewBounds();
		void RecalcScrollBarVisibility();
		void EnsureCalculationsAreFinished();
		void RecreateViewInfo();
		void RecalcFinalLayout();
		void RecalcPreliminaryLayout();
		void ClearPreliminaryAppointmentsAndCellContainers();
		void RecalcAppointmentsLayout();
		void RecalcDraggingAppointmentPosition();
		bool ObtainDateTimeScrollBarVisibility();
		void UpdateScrollBarsPosition();
		void UpdateDateTimeScrollBarValue();
		void UpdateResourceScrollBarValue();
		void RepaintView();
		void UpdateScrollMoreButtonsVisibility();
		ChangeActions PrepareChangeActions();
		RecurrentAppointmentAction ShowEditRecurrentAppointmentForm(Appointment apt, bool readOnly, CommandSourceType commandSourceType);
		void ShowGotoDateForm(DateTime date);
		RecurrentAppointmentAction ShowDeleteRecurrentAppointmentForm(Appointment apt);
		RecurrentAppointmentAction ShowDeleteRecurrentAppointmentsForm(AppointmentBaseCollection apts);
		event EventHandler StorageChanged;
		ISchedulerStorageBase Storage { get; }
#if SL
		bool LeftButtonPressed { get; }
#endif
		void ShowPrintPreview();
	}
	#endregion
	#region InnerSchedulerControl
	public class InnerSchedulerControl : ISchedulerControlChangeTarget, IBatchUpdateable, IBatchUpdateHandler, IDisposable, IServiceContainer, ISchedulerCommandTarget, IInnerSchedulerCommandTarget, INotifyPropertyChanged, ISupportAppointmentEdit, ISupportAppointmentDependencyEdit, ICommandAwareControl<SchedulerCommandId> {
		public static WorkDaysCollection CreateDefaultWorkDaysCollection() {
			WorkDaysCollection workDays = CreateWorkDays();
			workDays.Add(new WeekDaysWorkDay(WeekDays.WorkDays));
			return workDays;
		}
		protected internal static WorkDaysCollection CreateWorkDays() {
			return new WorkDaysCollection(); 
		}
		#region Fields
		bool isDisposed;
		BatchUpdateHelper batchUpdateHelper;
		SuspendEventTracker suspendEventTracker;
		IInnerSchedulerControlOwner owner;
		SchedulerControlDeferredChanges deferredChanges;
		ISchedulerStorageBase storage;
		SchedulerViewRepositoryBase views;
		InnerSchedulerViewBase activeView;
		ICollectionChangedListener resourceColorSchemasListener;
		ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> resourceColorSchemas;
		NotificationTimeInterval limitInterval;
		WorkDaysCollection workDays;
		NotificationCollectionChangedListener<WorkDay> workDaysCollectionListener;
		SchedulerOptionsCustomization optionsCustomization;
		SchedulerOptionsBehaviorBase optionsBehavior;
		SchedulerOptionsViewBase optionsView;
		SchedulerViewSelection selection;
		AppointmentSelectionController appointmentSelectionController;
		AppointmentDependencySelectionController appointmentDependencySelectionController;
		ServiceManager serviceManager;
		ISchedulerInplaceEditController inplaceEditController;
		AppointmentChangeHelper appointmentChangeHelper;
		Stack<KeyboardHandler> keyboardHandlers;
		SchedulerMouseHandler mouseHandler;
		int stateIdentity = Int32.MaxValue;
		bool forceQueryAppointments = false;
		bool deferredStateIdentityChanged = false;
		Stack<IAppointmentVisualStateCalculator> appointmentVisualStateCalculator;
		SchedulerTimeZoneHelperEventWrapper timeZoneEngineEventWrapper;
		#endregion
		public InnerSchedulerControl(IInnerSchedulerControlOwner owner) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
			this.appointmentVisualStateCalculator = new Stack<IAppointmentVisualStateCalculator>();
			this.appointmentVisualStateCalculator.Push(new DefaultAppointmentVisualStateCalculator(this));
		}
		#region Properties
		public TimeZoneHelper TimeZoneHelper { get; private set; }
		public IAppointmentVisualStateCalculator AppointmentVisualStateCalculator { get { return this.appointmentVisualStateCalculator.Peek(); } }
		internal bool IsEventsSuspended { get { return suspendEventTracker.IsEventsSuspended; } }
		internal bool IsDisposed { get { return isDisposed; } }
		public IInnerSchedulerControlOwner Owner { get { return owner; } }
		internal SchedulerControlDeferredChanges DeferredChanges { get { return deferredChanges; } }
		internal SuspendEventTracker SuspendEventTracker { get { return suspendEventTracker; } }
		#region ResourceColorSchemas
		[Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> ResourceColorSchemas { get { return resourceColorSchemas; } }
		internal bool ShouldSerializeResourceColorSchemas() {
			return !ResourceColorSchemas.HasDefaultContent();
		}
		internal void ResetResourceColorSchemas() {
			ResourceColorSchemas.LoadDefaults();
		}
		protected internal virtual object XtraCreateResourceColorSchemasItem(XtraItemEventArgs e) {
			return new SchedulerColorSchemaBase();
		}
		internal void XtraSetIndexResourceColorSchemasItem(XtraSetItemIndexEventArgs e) {
			SchedulerColorSchemaBase schema = e.Item.Value as SchedulerColorSchemaBase;
			if (schema == null)
				return;
			ResourceColorSchemas.Add(schema);
		}
		#endregion
		internal ICollectionChangedListener ResourceColorSchemasListener { get { return resourceColorSchemasListener; } }
		public SchedulerOptionsCustomization OptionsCustomization { get { return optionsCustomization; } }
		public SchedulerOptionsBehaviorBase OptionsBehavior { get { return optionsBehavior; } }
		public SchedulerOptionsViewBase OptionsView { get { return optionsView; } }
		public SchedulerViewSelection Selection { get { return selection; } set { selection = value; } }
		internal AppointmentSelectionController AppointmentSelectionController { get { return appointmentSelectionController; } }
		internal AppointmentDependencySelectionController AppointmentDependencySelectionController { get { return appointmentDependencySelectionController; } }
		internal AppointmentBaseCollection SelectedAppointments { get { return AppointmentSelectionController != null ? AppointmentSelectionController.SelectedAppointments : new AppointmentBaseCollection(); } }
		internal AppointmentDependencyBaseCollection SelectedDependencies { get { return AppointmentDependencySelectionController != null ? AppointmentDependencySelectionController.SelectedDependencies : new AppointmentDependencyBaseCollection(); } }
		#region Start
		public DateTime Start {
			get { return ActiveView.VisibleStart; }
			set {
				if (this.Start == value)
					return;
				ActiveView.SetStart(value, selection);
				ApplyChanges(SchedulerControlChangeType.ControlStartChanged);
				RaiseUpdateUI();
			}
		}
		#endregion
		#region LimitInterval
		public NotificationTimeInterval LimitInterval {
			get { return limitInterval; }
			set {
				NotificationTimeInterval newInterval = (value == null) ? NotificationTimeInterval.FullRange :
					new NotificationTimeInterval(value.Start, value.Duration);
				if (limitInterval.Equals(newInterval))
					return;
				UnsubscribeLimitIntervalEvents();
				limitInterval = newInterval;
				SubscribeLimitIntervalEvents();
				OnLimitIntervalChanged(this, EventArgs.Empty);
			}
		}
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WorkDaysCollection WorkDays { get { return workDays; } }
		internal NotificationCollectionChangedListener<WorkDay> WorkDaysCollectionListener { get { return workDaysCollectionListener; } }
		#region Storage
		[Category(SRCategoryNames.Data), DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public ISchedulerStorageBase Storage {
			get { return storage; }
			set {
				if (value != null && value.IsDisposed)
					value = null;
				if (storage == value)
					return;
				if (value != null) {
					IInternalSchedulerStorageBase internalStorage = value as IInternalSchedulerStorageBase;
					if (internalStorage != null)
						TimeZoneHelper.StorageTimeZoneEngine = internalStorage.TimeZoneEngine;
				}
				SetStorageCore(value);
				ApplyChanges(SchedulerControlChangeType.StorageChanged);
				OnStorageChanged();
			}
		}
		#endregion
		IInternalSchedulerStorageBase InternalStorageImpl { get { return Storage as IInternalSchedulerStorageBase; } }
		[Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public SchedulerViewRepositoryBase Views { get { return views; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public InnerSchedulerViewBase ActiveView { get { return activeView; } protected internal set { activeView = value; } }
		[Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public InnerDayView DayView { get { return (InnerDayView)Views.GetInnerView(SchedulerViewType.Day); } }
		[Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public InnerWorkWeekView WorkWeekView { get { return (InnerWorkWeekView)Views.GetInnerView(SchedulerViewType.WorkWeek); } }
		[Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public InnerWeekView WeekView { get { return (InnerWeekView)Views.GetInnerView(SchedulerViewType.Week); } }
		[Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public InnerMonthView MonthView { get { return (InnerMonthView)Views.GetInnerView(SchedulerViewType.Month); } }
		[Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public InnerTimelineView TimelineView { get { return (InnerTimelineView)Views.GetInnerView(SchedulerViewType.Timeline); } }
		[Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public InnerGanttView GanttView { get { return (InnerGanttView)Views.GetInnerView(SchedulerViewType.Gantt); } }
		[Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public InnerFullWeekView FullWeekView { get { return (InnerFullWeekView)Views.GetInnerView(SchedulerViewType.FullWeek); } }
		#region ActiveViewType
		[
		DefaultValue(SchedulerViewType.Day),
		Category(SRCategoryNames.View),
		XtraSerializableProperty(XtraSerializationFlags.DefaultValue)
		]
		public SchedulerViewType ActiveViewType {
			get {
				return activeView.Type;
			}
			set {
				if (ActiveViewType == value)
					return;
				InnerSchedulerViewBase newView = Views.GetInnerView(value);
				if (newView == null)
					return;
				if (OnActiveViewChanging(newView)) {
					UnsubscribeActiveViewEvents();
					RaiseBeforeActiveViewChange();
					DateTime previousViewStart = Selection.FirstSelectedInterval.Start;
					InnerSchedulerViewBase oldView = this.activeView;
					this.activeView = newView;
					newView.Enabled = true;
					RaiseAfterActiveViewChange();
					activeView.SynchronizeVisibleStart(previousViewStart, selection);
					SubscribeActiveViewEvents();
					RaisePropertyChanged("ActiveViewType");
					OnVisibleResourcesChanged(oldView, newView);
					ApplyChanges(SchedulerControlChangeType.ActiveViewChanged);
					RaiseUpdateUI();
					OnActiveViewChanged();
				}
			}
		}
		#endregion
		#region GroupType
		[DefaultValue(SchedulerGroupType.None), Category(SRCategoryNames.View), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public SchedulerGroupType GroupType {
			get {
				if (activeView != null)
					return activeView.GroupType;
				else
					return SchedulerGroupType.None;
			}
			set {
				if (views != null)
					views.SetGroupType(value);
			}
		}
		#endregion
		[Browsable(false)]
		public bool SupportsRecurrence { get { return Storage != null ? Storage.SupportsRecurrence : false; } }
		[Browsable(false)]
		public bool ResourceSharing { get { return Storage != null ? Storage.ResourceSharing : false; } }
		public DayOfWeek FirstDayOfWeek { get { return DateTimeHelper.ConvertFirstDayOfWeek(OptionsView.FirstDayOfWeek); } }
		protected internal ServiceManager ServiceManager { get { return serviceManager; } }
		internal ISchedulerInplaceEditController InplaceEditController { get { return inplaceEditController; } }
		internal AppointmentChangeHelper AppointmentChangeHelper { get { return appointmentChangeHelper; } }
		internal KeyboardHandler KeyboardHandler { get { return keyboardHandlers.Peek(); } }
		internal Stack<KeyboardHandler> KeyboardHandlers { get { return keyboardHandlers; } }
		internal SchedulerMouseHandler MouseHandler { get { return mouseHandler; } }
		internal int StateIdentity { get { return stateIdentity; } }
		internal bool ForceQueryAppointments { get { return forceQueryAppointments; } set { forceQueryAppointments = value; } }
		TimeZoneEngineSyncronizer TimeZoneEngineSyncronizer { get; set; }
		IInternalSchedulerStorageBase InternalStorage { get { return (IInternalSchedulerStorageBase)Storage; } }
		protected internal bool NestedQuery { get; set; }
		#endregion
		#region Events
		#region AllowAppointmentDrag
		internal AppointmentOperationEventHandler onAllowAppointmentDrag;
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentDrag {
			add { onAllowAppointmentDrag += value; }
			remove { onAllowAppointmentDrag -= value; }
		}
		#endregion
		#region AllowAppointmentDragBetweenResources
		internal AppointmentOperationEventHandler onAllowAppointmentDragBetweenResources;
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentDragBetweenResources {
			add { onAllowAppointmentDragBetweenResources += value; }
			remove { onAllowAppointmentDragBetweenResources -= value; }
		}
		#endregion
		#region AllowAppointmentResize
		internal AppointmentOperationEventHandler onAllowAppointmentResize;
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentResize {
			add { onAllowAppointmentResize += value; }
			remove { onAllowAppointmentResize -= value; }
		}
		#endregion
		#region AllowAppointmentCopy
		internal AppointmentOperationEventHandler onAllowAppointmentCopy;
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentCopy {
			add { onAllowAppointmentCopy += value; }
			remove { onAllowAppointmentCopy -= value; }
		}
		#endregion
		#region AllowAppointmentDelete
		internal AppointmentOperationEventHandler onAllowAppointmentDelete;
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentDelete {
			add { onAllowAppointmentDelete += value; }
			remove { onAllowAppointmentDelete -= value; }
		}
		#endregion
		#region AllowAppointmentCreate
		internal AppointmentOperationEventHandler onAllowAppointmentCreate;
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentCreate {
			add { onAllowAppointmentCreate += value; }
			remove { onAllowAppointmentCreate -= value; }
		}
		#endregion
		#region AllowAppointmentEdit
		internal AppointmentOperationEventHandler onAllowAppointmentEdit;
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentEdit {
			add { onAllowAppointmentEdit += value; }
			remove { onAllowAppointmentEdit -= value; }
		}
		#endregion
		#region AllowInplaceEditor
		internal AppointmentOperationEventHandler onAllowInplaceEditor;
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowInplaceEditor {
			add { onAllowInplaceEditor += value; }
			remove { onAllowInplaceEditor -= value; }
		}
		#endregion
		#region UnhandledException
		SchedulerUnhandledExceptionEventHandler onUnhandledException;
		public event SchedulerUnhandledExceptionEventHandler UnhandledException { add { onUnhandledException += value; } remove { onUnhandledException -= value; } }
		protected internal virtual bool RaiseUnhandledException(Exception e) {
			try {
				if (onUnhandledException != null) {
					SchedulerUnhandledExceptionEventArgs args = new SchedulerUnhandledExceptionEventArgs(e);
					onUnhandledException(Owner, args);
					return args.Handled;
				} else
					return false;
			} catch {
				return false;
			}
		}
		#endregion
		#region AllowAppointmentConflicts
		AppointmentConflictEventHandler onAllowAppointmentConflicts;
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentConflictEventHandler AllowAppointmentConflicts {
			add { onAllowAppointmentConflicts += value; }
			remove { onAllowAppointmentConflicts -= value; }
		}
		protected internal virtual void RaiseAllowAppointmentConflicts(AppointmentConflictEventArgs e) {
			if (onAllowAppointmentConflicts != null) {
				if (!IsEventsSuspended) {
					onAllowAppointmentConflicts(owner, e);
					RaiseUpdateUI();
				}
			}
		}
		#endregion
		#region AppointmentDrag
		AppointmentDragEventHandler onAppointmentDrag;
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentDragEventHandler AppointmentDrag {
			add { onAppointmentDrag += value; }
			remove { onAppointmentDrag -= value; }
		}
		protected internal virtual void RaiseAppointmentDrag(AppointmentDragEventArgs args) {
			if (onAppointmentDrag != null) {
				if (!IsEventsSuspended)
					onAppointmentDrag(owner, args);
			}
		}
		#endregion
		#region AppointmentDrop
		AppointmentDragEventHandler onAppointmentDrop;
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentDragEventHandler AppointmentDrop {
			add { onAppointmentDrop += value; }
			remove { onAppointmentDrop -= value; }
		}
		protected internal virtual void RaiseAppointmentDrop(AppointmentDragEventArgs args) {
			if (onAppointmentDrop != null) {
				if (!IsEventsSuspended)
					onAppointmentDrop(owner, args);
			}
		}
		#endregion
		#region AppointmentResizing
		AppointmentResizeEventHandler onAppointmentResizing;
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentResizeEventHandler AppointmentResizing {
			add { onAppointmentResizing += value; }
			remove { onAppointmentResizing -= value; }
		}
		protected internal virtual void RaiseAppointmentResizing(AppointmentResizeEventArgs args) {
			if (onAppointmentResizing != null) {
				if (!IsEventsSuspended)
					onAppointmentResizing(owner, args);
			}
		}
		#endregion
		#region AppointmentResized
		AppointmentResizeEventHandler onAppointmentResized;
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentResizeEventHandler AppointmentResized {
			add { onAppointmentResized += value; }
			remove { onAppointmentResized -= value; }
		}
		protected internal virtual void RaiseAppointmentResized(AppointmentResizeEventArgs args) {
			if (onAppointmentResized != null) {
				if (!IsEventsSuspended)
					onAppointmentResized(owner, args);
			}
		}
		#endregion
		#region AppointmentsSelectionChanged
		EventHandler onAppointmentsSelectionChanged;
		internal event EventHandler AppointmentsSelectionChanged {
			add { onAppointmentsSelectionChanged += value; }
			remove { onAppointmentsSelectionChanged -= value; }
		}
		protected internal virtual void RaiseAppointmentsSelectionChanged() {
			if (onAppointmentsSelectionChanged != null) {
				if (!IsEventsSuspended)
					onAppointmentsSelectionChanged(this, EventArgs.Empty);
			}
		}
		#endregion
		#region AppointmentDependenciesSelectionChanged
		EventHandler onAppointmentDependenciesSelectionChanged;
		internal event EventHandler AppointmentDependenciesSelectionChanged {
			add { onAppointmentDependenciesSelectionChanged += value; }
			remove { onAppointmentDependenciesSelectionChanged -= value; }
		}
		protected internal virtual void RaiseAppointmentDependenciesSelectionChanged() {
			if (onAppointmentDependenciesSelectionChanged != null) {
				if (!IsEventsSuspended)
					onAppointmentDependenciesSelectionChanged(this, EventArgs.Empty);
			}
		}
		#endregion
		#region StorageChanged
		EventHandler onStorageChanged;
		internal event EventHandler StorageChanged {
			add { onStorageChanged += value; }
			remove { onStorageChanged -= value; }
		}
		protected internal virtual void RaiseStorageChanged() {
			if (onStorageChanged != null) {
				if (!IsEventsSuspended)
					onStorageChanged(this, EventArgs.Empty);
			}
		}
		#endregion
		#region ReminderAlert
		ReminderEventHandler onReminderAlert;
		internal event ReminderEventHandler ReminderAlert {
			add { onReminderAlert += value; }
			remove { onReminderAlert -= value; }
		}
		protected internal virtual void RaiseReminderAlert(ReminderEventArgs args) {
			if (onReminderAlert != null) {
				if (!IsEventsSuspended)
					onReminderAlert(owner, args);
			}
		}
		#endregion
		#region BeforeActiveViewChange
		EventHandler onBeforeActiveViewChange;
		public event EventHandler BeforeActiveViewChange {
			add { onBeforeActiveViewChange += value; }
			remove { onBeforeActiveViewChange -= value; }
		}
		protected internal virtual void RaiseBeforeActiveViewChange() {
			if (onBeforeActiveViewChange != null)
				onBeforeActiveViewChange(this, EventArgs.Empty);
		}
		#endregion
		#region AfterActiveViewChange
		EventHandler onAfterActiveViewChange;
		public event EventHandler AfterActiveViewChange {
			add { onAfterActiveViewChange += value; }
			remove { onAfterActiveViewChange -= value; }
		}
		protected internal virtual void RaiseAfterActiveViewChange() {
			if (onAfterActiveViewChange != null)
				onAfterActiveViewChange(this, EventArgs.Empty);
		}
		#endregion
		#region ActiveViewChanging
		InnerActiveViewChangingEventHandler onActiveViewChanging;
		[Category(SRCategoryNames.Scheduler)]
		public event InnerActiveViewChangingEventHandler ActiveViewChanging {
			add { onActiveViewChanging += value; }
			remove { onActiveViewChanging -= value; }
		}
		protected internal virtual bool RaiseActiveViewChanging(InnerSchedulerViewBase newView) {
			if (onActiveViewChanging != null && !IsEventsSuspended) {
				InnerActiveViewChangingEventArgs args = new InnerActiveViewChangingEventArgs(newView, newView);
				onActiveViewChanging(this, args);
				return !args.Cancel;
			}
			return true;
		}
		#endregion
		#region ActiveViewChanged
		EventHandler onActiveViewChanged;
		[Category(SRCategoryNames.Scheduler)]
		public event EventHandler ActiveViewChanged {
			add { onActiveViewChanged += value; }
			remove { onActiveViewChanged -= value; }
		}
		protected internal virtual void RaiseActiveViewChanged() {
			if (onActiveViewChanged != null) {
				if (!IsEventsSuspended)
					onActiveViewChanged(owner, EventArgs.Empty);
			}
		}
		#endregion
		#region GroupTypeChanged
		EventHandler onGroupTypeChanged;
		internal event EventHandler GroupTypeChanged {
			add { onGroupTypeChanged += value; }
			remove { onGroupTypeChanged -= value; }
		}
		protected internal virtual void RaiseGroupTypeChanged() {
			if (onGroupTypeChanged != null)
				if (!IsEventsSuspended)
					onGroupTypeChanged(owner, EventArgs.Empty);
		}
		#endregion
		#region RaiseQueryWorkTime
		QueryWorkTimeEventHandler onRaiseQueryWorkTime;
		[Category(SRCategoryNames.Scheduler)]
		public event QueryWorkTimeEventHandler QueryWorkTime { add { onRaiseQueryWorkTime += value; } remove { onRaiseQueryWorkTime -= value; } }
		protected internal virtual void RaiseQueryWorkTime(object sender, QueryWorkTimeEventArgs e) {
			if (onRaiseQueryWorkTime != null) {
				if (!IsEventsSuspended)
					onRaiseQueryWorkTime(owner, e);
			}
		}
		#endregion
		#region RaiseMoreButtonClicked
		MoreButtonClickedEventHandler onRaiseMoreButtonClicked;
		[Category(SRCategoryNames.Scheduler)]
		public event MoreButtonClickedEventHandler MoreButtonClicked { add { onRaiseMoreButtonClicked += value; } remove { onRaiseMoreButtonClicked -= value; } }
		protected internal virtual bool RaiseMoreButtonClicked(object sender, MoreButtonClickedEventArgs e) {
			if (onRaiseMoreButtonClicked != null) {
				if (!IsEventsSuspended)
					onRaiseMoreButtonClicked(owner, e);
			}
			return e.Handled;
		}
		#endregion
		#region BeforeApplyChanges
		AfterApplyChangesEventHandler onBeforeApplyChanges;
		[Category(SRCategoryNames.Scheduler)]
		internal event AfterApplyChangesEventHandler BeforeApplyChanges {
			add { onBeforeApplyChanges += value; }
			remove { onBeforeApplyChanges -= value; }
		}
		protected internal virtual void RaiseBeforeApplyChanges(List<SchedulerControlChangeType> changeTypes, ChangeActions actions) {
			if (onBeforeApplyChanges != null) {
				AfterApplyChangesEventArgs args = new AfterApplyChangesEventArgs(changeTypes, actions);
				onBeforeApplyChanges(owner, args);
			}
		}
		#endregion
		#region AfterApplyChanges
		AfterApplyChangesEventHandler onAfterApplyChanges;
		[Category(SRCategoryNames.Scheduler)]
		internal event AfterApplyChangesEventHandler AfterApplyChanges {
			add { onAfterApplyChanges += value; }
			remove { onAfterApplyChanges -= value; }
		}
		protected internal virtual void RaiseAfterApplyChanges(List<SchedulerControlChangeType> changeTypes, ChangeActions actions) {
			if (onAfterApplyChanges != null) {
				AfterApplyChangesEventArgs args = new AfterApplyChangesEventArgs(changeTypes, actions);
				onAfterApplyChanges(owner, args);
			}
		}
		#endregion
		#region SelectionChanged
		EventHandler onSelectionChanged;
		[Category(SRCategoryNames.Scheduler)]
		public event EventHandler SelectionChanged {
			add { onSelectionChanged += value; }
			remove { onSelectionChanged -= value; }
		}
		protected internal virtual void RaiseSelectionChanged() {
			if (onSelectionChanged != null) {
				if (!IsEventsSuspended)
					onSelectionChanged(owner, EventArgs.Empty);
			}
		}
		#endregion
		#region VisibleIntervalChanged
		EventHandler onVisibleIntervalChanged;
		[Category(SRCategoryNames.Scheduler)]
		public event EventHandler VisibleIntervalChanged {
			add { onVisibleIntervalChanged += value; }
			remove { onVisibleIntervalChanged -= value; }
		}
		protected internal virtual ChangeActions RaiseVisibleIntervalChanged() {
			SchedulerStorageReloadListener listener = new SchedulerStorageReloadListener();
			if (Storage != null && InternalStorage != null) {
				InternalStorage.AfterFetchAppointments += listener.OnReload;
				((IInternalAppointmentStorage)Storage.Appointments).Reload += listener.OnReload;
			}
			try {
				RaiseInnerVisibleIntervalChanged();
			} finally {
				if (Storage != null && InternalStorage != null) {
					InternalStorage.AfterFetchAppointments -= listener.OnReload;
					((IInternalAppointmentStorage)Storage.Appointments).Reload -= listener.OnReload;
				}
			}
			if (onVisibleIntervalChanged != null) {
				if (!IsEventsSuspended)
					onVisibleIntervalChanged(owner, EventArgs.Empty);
			}
			if (listener.ShouldReload)
				return ChangeActions.ClearPreliminaryAppointmentsAndCellContainers | ChangeActions.RecalcViewLayout;
			else
				return ChangeActions.None;
		}
		#endregion
		#region InnerVisibleIntervalChanged
		EventHandler onInnerVisibleIntervalChanged;
		[Category(SRCategoryNames.Scheduler)]
		public event EventHandler InnerVisibleIntervalChanged {
			add { onInnerVisibleIntervalChanged += value; }
			remove { onInnerVisibleIntervalChanged -= value; }
		}
		protected internal virtual void RaiseInnerVisibleIntervalChanged() {
			if (onInnerVisibleIntervalChanged != null) {
				if (!IsEventsSuspended)
					onInnerVisibleIntervalChanged(owner, EventArgs.Empty);
			}
		}
		#endregion
		#region ViewUIChanged
		SchedulerViewUIChangedEventHandler onViewUIChanged;
		internal event SchedulerViewUIChangedEventHandler ViewUIChanged { add { onViewUIChanged += value; } remove { onViewUIChanged -= value; } }
		protected internal virtual void RaiseViewUIChanged(object sender, SchedulerViewUIChangedEventArgs args) {
			if (onViewUIChanged != null)
				onViewUIChanged(sender, args);
		}
		#endregion
		#region LimitIntervalChanged
		EventHandler onLimitIntervalChanged;
		public event EventHandler LimitIntervalChanged { add { onLimitIntervalChanged += value; } remove { onLimitIntervalChanged -= value; } }
		void RaiseLimitIntervalChanged() {
			EventHandler handler = onLimitIntervalChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#region RemindersFormDefaultAction
		RemindersFormDefaultActionEventHandler onRemindersFormDefaultAction;
		[Category(SRCategoryNames.Scheduler)]
		public event RemindersFormDefaultActionEventHandler RemindersFormDefaultAction {
			add { onRemindersFormDefaultAction += value; }
			remove { onRemindersFormDefaultAction -= value; }
		}
		protected internal virtual void RaiseRemindersFormDefaultAction(RemindersFormDefaultActionEventArgs e) {
			if (onRemindersFormDefaultAction != null)
				onRemindersFormDefaultAction(owner, e);
		}
		#endregion
		#region PropertyChanged (INotifyPropertyChanged implementation)
		PropertyChangedEventHandler onPropertyChanged;
		public event PropertyChangedEventHandler PropertyChanged { add { onPropertyChanged += value; } remove { onPropertyChanged -= value; } }
		protected internal virtual void RaisePropertyChanged(string propertyName) {
			if (onPropertyChanged != null) {
				PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
				onPropertyChanged(this, args);
			}
		}
		#endregion
		#region BeforeDispose
		EventHandler onBeforeDispose;
		event EventHandler ICommandAwareControl<SchedulerCommandId>.BeforeDispose { add { this.BeforeDispose += value; } remove { this.BeforeDispose -= value; } }
		internal event EventHandler BeforeDispose { add { onBeforeDispose += value; } remove { onBeforeDispose -= value; } }
		protected internal virtual void RaiseBeforeDispose() {
			if (onBeforeDispose != null)
				onBeforeDispose(this, EventArgs.Empty);
		}
		#endregion
		#region VisibleResourcesChanged
		VisibleResourcesChangedEventHandler onVisibleResourcesChanged;
		public event VisibleResourcesChangedEventHandler VisibleResourcesChanged {
			add { onVisibleResourcesChanged += value; }
			remove { onVisibleResourcesChanged -= value; }
		}
		void OnVisibleResourcesChanged() {
			if (GroupType == SchedulerGroupType.None)
				return;
			VisibleResourcesChangedEventArgs args = new VisibleResourcesChangedEventArgs(ActiveView.VisibleResources);
			args.OldFirstVisibleResourceIndex = ActiveView.FirstVisibleResourceIndex;
			args.OldResourcePerPage = ActiveView.ResourcesPerPage;
			args.NewFirstVisibleResourceIndex = ActiveView.FirstVisibleResourceIndex;
			args.NewResourcePerPage = ActiveView.ResourcesPerPage;
			RaiseVisibleResourcesChanged(args);
		}
		void OnVisibleResourcesChanged(InnerSchedulerViewBase oldView, InnerSchedulerViewBase newView) {
			if (GroupType == SchedulerGroupType.None)
				return;
			VisibleResourcesChangedEventArgs args = new VisibleResourcesChangedEventArgs(ActiveView.VisibleResources);
			args.OldFirstVisibleResourceIndex = oldView.FirstVisibleResourceIndex;
			args.OldResourcePerPage = oldView.ResourcesPerPage;
			args.NewFirstVisibleResourceIndex = newView.FirstVisibleResourceIndex;
			args.NewResourcePerPage = newView.ResourcesPerPage;
			RaiseVisibleResourcesChanged(args);
		}
		void OnVisibleResourcesChanged(SchedulerControlStateChangedEventArgs e) {
			VisibleResourcesChangedEventArgs args = new VisibleResourcesChangedEventArgs(ActiveView.VisibleResources);
			args.OldFirstVisibleResourceIndex = (e.Change == SchedulerControlChangeType.FirstVisibleResourceIndexChanged) ? (int)e.OldValue : activeView.ActualFirstVisibleResourceIndex;
			args.OldResourcePerPage = (e.Change == SchedulerControlChangeType.ResourcesPerPageChanged) ? (int)e.OldValue : activeView.ResourcesPerPage;
			args.NewFirstVisibleResourceIndex = ActiveView.ActualFirstVisibleResourceIndex;
			args.NewResourcePerPage = ActiveView.ResourcesPerPage;
			RaiseVisibleResourcesChanged(args);
		}
		protected internal virtual void RaiseVisibleResourcesChanged(VisibleResourcesChangedEventArgs args) {
			if (this.onVisibleResourcesChanged == null)
				return;
			this.onVisibleResourcesChanged(this, args);
		}
		#endregion
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (!IsDisposed)
					RaiseBeforeDispose();
				if (this.activeView != null && this.activeView.Owner != null) {
					IViewAsyncSupport viewAsync = this.activeView.Owner as IViewAsyncSupport;
					if (viewAsync != null)
						viewAsync.ThreadManager.WaitForAllThreads();
				}
				if (mouseHandler != null) {
					mouseHandler.Dispose();
					mouseHandler = null;
				}
				if (views != null) {
					UnsubscribeViewsUIChangedEvents();
					UnsubscribeActiveViewEvents();
					views.Dispose();
					views = null;
				}
				if (activeView != null) {
					activeView.Dispose();
					activeView = null;
				}
				if (storage != null) {
					UnsubscribeStorageEvents();
					DeleteAppointmentCache(owner);
					storage = null;
				}
				if (resourceColorSchemasListener != null) {
					UnsubscribeResourceColorSchemasListenerEvents();
					resourceColorSchemasListener.Dispose();
					resourceColorSchemasListener = null;
				}
				if (resourceColorSchemas != null) {
					resourceColorSchemas.Clear();
					resourceColorSchemas = null;
				}
				if (workDaysCollectionListener != null) {
					UnsubscribeWorkDaysEvents();
					workDaysCollectionListener.Dispose();
					workDaysCollectionListener = null;
				}
				if (this.timeZoneEngineEventWrapper != null) {
					this.timeZoneEngineEventWrapper.CleanUp();
					this.timeZoneEngineEventWrapper = null;
				}
				if (TimeZoneHelper != null)
					UnsubscribeTimeZoneHelperEvents();
				if (TimeZoneHelper != null) {
					TimeZoneHelper = null;
				}
				if (TimeZoneEngineSyncronizer != null) {
					TimeZoneEngineSyncronizer.Dispose();
					TimeZoneEngineSyncronizer = null;
				}
				workDays = null;
				if (limitInterval != null) {
					UnsubscribeLimitIntervalEvents();
					limitInterval = null;
				}
				if (inplaceEditController != null) {
					inplaceEditController.Dispose();
					inplaceEditController = null;
				}
				if (appointmentSelectionController != null) {
					UnsubscribeAppointmentSelectionControllerEvents();
					appointmentSelectionController.Dispose();
					appointmentSelectionController = null;
				}
				if (appointmentDependencySelectionController != null) {
					UnsubscribeAppointmentDependencySelectionControllerEvents();
					appointmentDependencySelectionController.Dispose();
					appointmentDependencySelectionController = null;
				}
				if (optionsCustomization != null) {
					UnsubscribeOptionsCustomizationEvents();
					optionsCustomization = null;
				}
				if (optionsBehavior != null) {
					UnsubscribeOptionsBehaviorEvents();
					optionsBehavior = null;
				}
				if (optionsView != null) {
					UnsubscribeOptionsViewEvents();
					optionsView = null;
				}
				if (this.serviceManager != null) {
					UnsubscibeServiceManagerEvents();
					this.serviceManager.Dispose();
					this.serviceManager = null;
				}
				if (keyboardHandlers != null) {
					keyboardHandlers.Clear();
					keyboardHandlers = null;
				}
				this.activeView = null;
				this.batchUpdateHelper = null;
				this.suspendEventTracker = null;
				this.selection = null;
				this.appointmentChangeHelper = null;
				this.owner = null;
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		public virtual void Initialize() {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.suspendEventTracker = new SuspendEventTracker();
			this.serviceManager = CreateServiceManager();
			SubscribeServiceManagerEvents();
			AddServices();
			this.resourceColorSchemas = CreateResourceColorSchemaCollection();
			this.resourceColorSchemasListener = CreateResourceColorSchemaChangedListener();
			SubscribeResourceColorSchemasListenerEvents();
			this.limitInterval = NotificationTimeInterval.FullRange;
			SubscribeLimitIntervalEvents();
			this.workDays = CreateDefaultWorkDaysCollection();
			this.workDaysCollectionListener = new NotificationCollectionChangedListener<WorkDay>(workDays);
			SubscribeWorkDaysEvents();
			this.optionsCustomization = new SchedulerOptionsCustomization();
			SubscribeOptionsCustomizationEvents();
			this.optionsBehavior = owner.CreateOptionsBehavior();
			SubscribeOptionsBehaviorEvents();
			InitializeTimeZoneEngine();
			this.optionsBehavior.TimeZoneHelper = TimeZoneHelper;
			SubscribeTimeZoneHelperEvents();
			this.optionsView = owner.CreateOptionsView();
			SubscribeOptionsViewEvents();
			this.selection = new SchedulerViewSelection();
			this.appointmentSelectionController = owner.CreateAppointmentSelectionController();
			SubscribeAppointmentSelectionControllerEvents();
			this.appointmentDependencySelectionController = CreateAppointmentDependencySelectionController();
			SubscribeAppointmentDependencySelectionControllerEvents();
			this.views = owner.CreateViewRepository();
			this.views.CreateViews(this);
			this.views.InitializeViews(this);
			this.activeView = views.GetInnerView(SchedulerViewType.Day);
			SubscribeActiveViewEvents();
			SubscribeViewsUIChangedEvents();
			this.inplaceEditController = owner.CreateInplaceEditController();
			((ISchedulerControlChangeTarget)this).AssignLimitInterval();
			InitializeSelection();
			InitializeKeyboardHandlers();
			this.appointmentChangeHelper = Owner.CreateAppointmentChangeHelper(this);
			this.mouseHandler = Owner.CreateMouseHandler(this);
			this.mouseHandler.Initialize();
		}
		protected internal virtual void InitializeTimeZoneEngine() {
			TimeZoneHelper = CreateTimeZoneEngine();
			TimeZoneEngineSyncronizer = new TimeZoneEngineSyncronizer(this, TimeZoneHelper);
		}
		protected virtual ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> CreateResourceColorSchemaCollection() {
			return new SchedulerColorSchemaCollectionBase<SchedulerColorSchemaBase>();
		}
		protected virtual ICollectionChangedListener CreateResourceColorSchemaChangedListener() {
			return new ResourceColorSchemasChangedListenerCore(ResourceColorSchemas);
		}
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			this.deferredChanges = new SchedulerControlDeferredChanges();
			this.AppointmentSelectionController.BeginUpdate();
			this.AppointmentDependencySelectionController.BeginUpdate();
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			if (this.deferredStateIdentityChanged)
				UpdateStateIdentityCore();
			this.deferredStateIdentityChanged = false;
			if (deferredChanges.ChangeActions != ChangeActions.None)
				ApplyChangesCore(deferredChanges.ChangeTypes, deferredChanges.ChangeActions);
			this.AppointmentSelectionController.EndUpdate();
			this.AppointmentDependencySelectionController.EndUpdate();
			this.deferredChanges = null;
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			this.AppointmentSelectionController.CancelUpdate();
			this.AppointmentDependencySelectionController.CancelUpdate();
			this.deferredChanges = null;
		}
		#endregion
		protected internal virtual void BeginInit() {
			ResourceColorSchemas.Clear();
			DayView.TimeRulers.Clear();
			DayView.TimeSlots.Clear();
			WorkWeekView.TimeRulers.Clear();
			WorkWeekView.TimeSlots.Clear();
			FullWeekView.TimeRulers.Clear();
			FullWeekView.TimeSlots.Clear();
			TimelineView.Scales.Clear();
		}
		protected internal virtual void EndInit() {
			if (ResourceColorSchemas.Count <= 0)
				ResourceColorSchemas.LoadDefaults();
			if (DayView.TimeSlots.Count <= 0)
				DayView.TimeSlots.LoadDefaults();
			if (WorkWeekView.TimeSlots.Count <= 0)
				WorkWeekView.TimeSlots.LoadDefaults();
			if (FullWeekView.TimeSlots.Count <= 0)
				FullWeekView.TimeSlots.LoadDefaults();
			if (TimelineView.Scales.Count <= 0)
				TimelineView.Scales.LoadDefaults();
			DayView.UpdateTimeRulersClientTimeZone(TimeZoneHelper.ClientTimeZone);
			WorkWeekView.UpdateTimeRulersClientTimeZone(TimeZoneHelper.ClientTimeZone);
			FullWeekView.UpdateTimeRulersClientTimeZone(TimeZoneHelper.ClientTimeZone);
		}
		protected internal virtual void RecreateAppointmentChangeHelper() {
			this.appointmentChangeHelper = Owner.CreateAppointmentChangeHelper(this);
		}
		internal void SuspendEvents() {
			suspendEventTracker.SuspendEvents();
		}
		internal void ResumeEvents() {
			suspendEventTracker.ResumeEvents();
		}
		protected internal virtual void RaiseAppointmentOperationEvent(AppointmentOperationEventHandler handler, AppointmentOperationEventArgs args) {
			if (handler != null) {
				if (!IsEventsSuspended)
					handler(owner, args);
			}
		}
		protected internal virtual ServiceManager CreateServiceManager() {
			return new ServiceManager();
		}
		protected internal virtual void AddServices() {
			AddService(typeof(IDateTimeNavigationService), new DateTimeNavigationService(this));
			AddService(typeof(IResourceNavigationService), new ResourceNavigationService(this));
			AddService(typeof(IAppointmentSelectionService), new AppointmentSelectionService(this));
			AddService(typeof(ITimeRulerFormatStringService), new TimeRulerFormatStringService());
			AddService(typeof(IAppointmentFormatStringService), new AppointmentFormatStringService());
			AddService(typeof(ISchedulerCommandFactoryService), CreateSchedulerCommandFactoryService());
			AddService(typeof(IAppointmentComparerProvider), new AppointmentComparerProvider(this));
		}
		protected virtual object CreateSchedulerCommandFactoryService() {
			return new SchedulerCommandFactoryService(this);
		}
		void SubscribeServiceManagerEvents() {
			ServiceManager.ServiceListChanged += new EventHandler(OnServiceManagerServiceListChanged);
		}
		void UnsubscibeServiceManagerEvents() {
			ServiceManager.ServiceListChanged -= new EventHandler(OnServiceManagerServiceListChanged);
		}
		void OnServiceManagerServiceListChanged(object sender, EventArgs e) {
			RaiseUpdateUI();
		}
		void SubscribeTimeZoneHelperEvents() {
			TimeZoneHelper.ClientTimeZoneChanged += OnTimeZoneHelperClientTimeZoneChanged;
		}
		void UnsubscribeTimeZoneHelperEvents() {
			TimeZoneHelper.ClientTimeZoneChanged -= OnTimeZoneHelperClientTimeZoneChanged;
		}
		void OnTimeZoneHelperClientTimeZoneChanged(object sender, EventArgs e) {
			UpdateRulersClientTimeZone();
		}
		#region SubscribeOptionsCustomizationEvents
		protected internal virtual void SubscribeOptionsCustomizationEvents() {
			optionsCustomization.Changed += new BaseOptionChangedEventHandler(OnOptionsCustomizationChanged);
		}
		#endregion
		#region UnsubscribeOptionsCustomizationEvents
		protected internal virtual void UnsubscribeOptionsCustomizationEvents() {
			optionsCustomization.Changed -= new BaseOptionChangedEventHandler(OnOptionsCustomizationChanged);
		}
		#endregion
		protected internal virtual void OnOptionsCustomizationChanged(object sender, BaseOptionChangedEventArgs e) {
			appointmentSelectionController.AllowAppointmentMultiSelect = optionsCustomization.AllowAppointmentMultiSelect;
			ApplyChanges(SchedulerControlChangeType.OptionsCustomizationChanged);
			RaiseUpdateUI();
		}
		#region SubscribeOptionsBehaviorEvents
		protected internal virtual void SubscribeOptionsBehaviorEvents() {
			optionsBehavior.Changed += new BaseOptionChangedEventHandler(OnOptionsBehaviorChanged);
		}
		#endregion
		#region UnsubscribeOptionsBehaviorEvents
		protected internal virtual void UnsubscribeOptionsBehaviorEvents() {
			optionsBehavior.Changed -= new BaseOptionChangedEventHandler(OnOptionsBehaviorChanged);
		}
		#endregion
		protected internal virtual void OnOptionsBehaviorChanged(object sender, BaseOptionChangedEventArgs e) {
			UpdateTimeZoneEngine();
			UpdateRulersClientTimeZone();
			if (e.Name == "ShowCurrentTime")
				UpdateTimeMarkerVisibilityFromOptionBehavior();
			ApplyChanges(SchedulerControlChangeType.OptionsBehaviorChanged);
		}
#pragma warning disable 618
		protected internal virtual void UpdateTimeMarkerVisibilityFromOptionBehavior() {
			TimeMarkerVisibility timeMarkerVisibility = ConvertToTimeMarkerVisibility(OptionsBehavior.ShowCurrentTime);
			UpdateTimeMarkerVisibility(DayView, timeMarkerVisibility);
			UpdateTimeMarkerVisibility(WorkWeekView, timeMarkerVisibility);
			UpdateTimeMarkerVisibility(FullWeekView, timeMarkerVisibility);
		}
		TimeMarkerVisibility ConvertToTimeMarkerVisibility(CurrentTimeVisibility visibility) {
			switch (visibility) {
				case CurrentTimeVisibility.Always:
					return TimeMarkerVisibility.Always;
				case CurrentTimeVisibility.Never:
					return TimeMarkerVisibility.Never;
				default:
					return TimeMarkerVisibility.TodayView;
			}
		}
		void UpdateTimeMarkerVisibility(InnerSchedulerViewBase view, TimeMarkerVisibility timeMarkerVisibility) {
			if (view == null)
				return;
			InnerDayView dayView = view as InnerDayView;
			if (dayView != null && dayView.TimeMarkerVisibility != timeMarkerVisibility)
				dayView.TimeMarkerVisibility = timeMarkerVisibility;
		}
#pragma warning restore 618
		protected internal virtual void UpdateRulersClientTimeZone() {
			DayView.UpdateTimeRulersClientTimeZone(TimeZoneHelper.ClientTimeZone);
			WorkWeekView.UpdateTimeRulersClientTimeZone(TimeZoneHelper.ClientTimeZone);
			FullWeekView.UpdateTimeRulersClientTimeZone(TimeZoneHelper.ClientTimeZone);
		}
		#region SubscribeOptionsViewEvents
		protected internal virtual void SubscribeOptionsViewEvents() {
			optionsView.Changed += new BaseOptionChangedEventHandler(OnOptionsViewChanged);
		}
		#endregion
		#region UnsubscribeOptionsViewEvents
		protected internal virtual void UnsubscribeOptionsViewEvents() {
			optionsView.Changed -= new BaseOptionChangedEventHandler(OnOptionsViewChanged);
		}
		#endregion
		protected internal virtual void OnOptionsViewChanged(object sender, BaseOptionChangedEventArgs e) {
			ApplyChanges(SchedulerControlChangeType.OptionsViewChanged);
		}
		#region SubscribeAppointmentSelectionControllerEvents
		protected internal virtual void SubscribeAppointmentSelectionControllerEvents() {
			appointmentSelectionController.SelectionChanged += new EventHandler(OnAppointmentsSelectionChanged);
		}
		#endregion
		#region UnsubscribeAppointmentSelectionControllerEvents
		protected internal virtual void UnsubscribeAppointmentSelectionControllerEvents() {
			appointmentSelectionController.SelectionChanged -= new EventHandler(OnAppointmentsSelectionChanged);
		}
		#endregion
		protected internal virtual void OnAppointmentsSelectionChanged(object sender, EventArgs e) {
			RaiseAppointmentsSelectionChanged();
			RaiseUpdateUI();
		}
		#region SubscribeAppointmentDependencySelectionControllerEvents
		protected internal virtual void SubscribeAppointmentDependencySelectionControllerEvents() {
			appointmentDependencySelectionController.SelectionChanged += new EventHandler(OnAppointmentDependenciesSelectionChanged);
		}
		#endregion
		#region UnsubscribeAppointmentDependencySelectionControllerEvents
		protected internal virtual void UnsubscribeAppointmentDependencySelectionControllerEvents() {
			appointmentDependencySelectionController.SelectionChanged -= new EventHandler(OnAppointmentDependenciesSelectionChanged);
		}
		#endregion
		protected internal virtual void OnAppointmentDependenciesSelectionChanged(object sender, EventArgs e) {
			RaiseAppointmentDependenciesSelectionChanged();
		}
		#region SubscribeResourceColorSchemasListenerEvents
		protected internal virtual void SubscribeResourceColorSchemasListenerEvents() {
			resourceColorSchemasListener.Changed += new EventHandler(OnResourceColorSchemasListenerChanged);
		}
		#endregion
		#region UnsubscribeResourceColorSchemasListenerEvents
		protected internal virtual void UnsubscribeResourceColorSchemasListenerEvents() {
			resourceColorSchemasListener.Changed -= new EventHandler(OnResourceColorSchemasListenerChanged);
		}
		#endregion
		protected internal virtual void OnResourceColorSchemasListenerChanged(object sender, EventArgs e) {
			ApplyChanges(SchedulerControlChangeType.ResourceColorSchemasChanged);
		}
		#region SubscribeLimitIntervalEvents
		protected internal virtual void SubscribeLimitIntervalEvents() {
			limitInterval.Changed += new EventHandler(OnLimitIntervalChanged);
		}
		#endregion
		#region UnsubscribeLimitIntervalEvents
		protected internal virtual void UnsubscribeLimitIntervalEvents() {
			limitInterval.Changed -= new EventHandler(OnLimitIntervalChanged);
		}
		#endregion
		protected internal virtual void OnLimitIntervalChanged(object sender, EventArgs e) {
			ApplyChanges(SchedulerControlChangeType.LimitIntervalChanged);
			RaiseUpdateUI();
			RaiseLimitIntervalChanged();
		}
		#region SubscribeWorkDaysEvents
		protected internal virtual void SubscribeWorkDaysEvents() {
			workDaysCollectionListener.Changed += new EventHandler(OnWorkDaysChanged);
		}
		#endregion
		#region UnsubscribeWorkDaysEvents
		protected internal virtual void UnsubscribeWorkDaysEvents() {
			workDaysCollectionListener.Changed -= new EventHandler(OnWorkDaysChanged);
		}
		#endregion
		protected internal virtual void OnWorkDaysChanged(object sender, EventArgs e) {
			ApplyChanges(SchedulerControlChangeType.WorkDaysChanged);
		}
		#region SubscribeActiveViewEvents
		protected internal virtual void SubscribeActiveViewEvents() {
			activeView.EnabledChanging += new CancelEventHandler(OnActiveViewEnabledChanging);
			activeView.Changed += new SchedulerControlStateChangedEventHandler(OnActiveViewChanged);
			activeView.LowLevelChanged += new SchedulerControlLowLevelStateChangedEventHandler(OnActiveViewLowLevelChanged);
			activeView.QueryWorkTime += new QueryWorkTimeEventHandler(OnActiveViewQueryWorkTime);
			activeView.GroupTypeChanged += new EventHandler(OnGroupTypeChanged);
		}
		#endregion
		#region UnsubscribeActiveViewEvents
		protected internal virtual void UnsubscribeActiveViewEvents() {
			activeView.EnabledChanging -= new CancelEventHandler(OnActiveViewEnabledChanging);
			activeView.Changed -= new SchedulerControlStateChangedEventHandler(OnActiveViewChanged);
			activeView.LowLevelChanged -= new SchedulerControlLowLevelStateChangedEventHandler(OnActiveViewLowLevelChanged);
			activeView.QueryWorkTime -= new QueryWorkTimeEventHandler(OnActiveViewQueryWorkTime);
			activeView.GroupTypeChanged -= new EventHandler(OnGroupTypeChanged);
		}
		#endregion
		protected internal virtual void SubscribeViewsUIChangedEvents() {
			int count = Views.Count;
			for (int i = 0; i < count; i++) {
				InnerSchedulerViewBase view = Views.GetInnerView(i);
				if (view != null)
					SubscribeViewUIChangedEvents(view);
			}
		}
		protected internal virtual void UnsubscribeViewsUIChangedEvents() {
			int count = Views.Count;
			for (int i = 0; i < count; i++) {
				InnerSchedulerViewBase view = Views.GetInnerView(i);
				if (view != null)
					UnsubscribeViewUIChangedEvents(view);
			}
		}
		protected internal virtual void SubscribeViewUIChangedEvents(InnerSchedulerViewBase view) {
			view.UIChanged += new SchedulerViewUIChangedEventHandler(OnViewUIChanged);
		}
		protected internal virtual void UnsubscribeViewUIChangedEvents(InnerSchedulerViewBase view) {
			view.UIChanged -= new SchedulerViewUIChangedEventHandler(OnViewUIChanged);
		}
		protected internal virtual void OnViewUIChanged(object sender, SchedulerViewUIChangedEventArgs e) {
			RaiseViewUIChanged(sender, e);
			RaiseUpdateUI();
		}
		protected internal virtual void OnActiveViewEnabledChanging(object sender, CancelEventArgs e) {
			e.Cancel = !IsUpdateLocked;
		}
		protected internal virtual void OnActiveViewChanged(object sender, SchedulerControlStateChangedEventArgs e) {
			if (e.IgnoreApplyChanges)
				return;
			ApplyChanges(e.Change);
			if (e.Change == SchedulerControlChangeType.FirstVisibleResourceIndexChanged || e.Change == SchedulerControlChangeType.ResourcesPerPageChanged || e.Change == SchedulerControlChangeType.ActiveViewChanged)
				OnVisibleResourcesChanged(e);
		}
		protected internal virtual void OnActiveViewLowLevelChanged(object sender, SchedulerControlLowLevelStateChangedEventArgs e) {
			ApplyChangesCore(e.ChangeType, e.Actions);
		}
		protected internal virtual bool OnActiveViewChanging(InnerSchedulerViewBase newView) {
			return RaiseActiveViewChanging(newView);
		}
		protected internal virtual void OnActiveViewChanged() {
			RaiseActiveViewChanged();
		}
		protected internal virtual void OnActiveViewQueryWorkTime(object sender, QueryWorkTimeEventArgs e) {
			RaiseQueryWorkTime(sender, e);
		}
		protected internal virtual void OnGroupTypeChanged(object sender, EventArgs e) {
			RaiseGroupTypeChanged();
			RaiseUpdateUI();
		}
		#region SubscribeStorageEvents
		protected internal virtual void SubscribeStorageEvents() {
			IInternalSchedulerStorageBase internalStorage = Storage as IInternalSchedulerStorageBase;
			if (internalStorage == null)
				return;
			internalStorage.InternalAppointmentCollectionCleared += new EventHandler(OnAppointmentCollectionCleared);
			internalStorage.InternalAppointmentCollectionLoaded += new EventHandler(OnAppointmentCollectionLoaded);
			internalStorage.InternalAppointmentsInserted += new PersistentObjectsEventHandler(OnAppointmentsInserted);
			internalStorage.InternalAppointmentsChanged += new PersistentObjectsEventHandler(OnAppointmentsChanged);
			internalStorage.InternalAppointmentsDeleted += new PersistentObjectsEventHandler(OnAppointmentsDeleted);
			internalStorage.InternalAppointmentMappingsChanged += new EventHandler(OnAppointmentMappingsChanged);
			internalStorage.InternalAppointmentUIObjectsChanged += new EventHandler(OnAppointmentUIObjectsChanged);
			internalStorage.InternalResourceCollectionCleared += new EventHandler(OnResourceCollectionCleared);
			internalStorage.InternalResourceCollectionLoaded += new EventHandler(OnResourceCollectionLoaded);
			internalStorage.InternalResourcesInserted += new PersistentObjectsEventHandler(OnResourcesInserted);
			internalStorage.InternalResourcesChanged += new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourcesDeleted += new PersistentObjectsEventHandler(OnResourcesDeleted);
			internalStorage.InternalResourceVisibilityChanged += new EventHandler(OnResourceVisibilityChanged);
			internalStorage.InternalResourceMappingsChanged += new EventHandler(OnResourceMappingsChanged);
			internalStorage.InternalResourceSortedColumnsChange += new EventHandler(OnResourceSortedColumnsChange);
			internalStorage.InternalDependencyCollectionCleared += new EventHandler(OnDependencyCollectionCleared);
			internalStorage.InternalDependencyCollectionLoaded += new EventHandler(OnDependencyCollectionLoaded);
			internalStorage.InternalDependenciesInserted += new PersistentObjectsEventHandler(OnDependenciesInserted);
			internalStorage.InternalDependenciesChanged += new PersistentObjectsEventHandler(OnDependenciesChanged);
			internalStorage.InternalDependenciesDeleted += new PersistentObjectsEventHandler(OnDependenciesDeleted);
			internalStorage.InternalDependencyMappingsChanged += new EventHandler(OnDependencyMappingsChanged);
			internalStorage.InternalDeferredNotifications += new EventHandler(OnStorageDeferredNotifications);
			internalStorage.InternalReminderAlert += new ReminderEventHandler(OnReminderAlert);
			internalStorage.BeforeDispose += new EventHandler(OnBeforeStorageDispose);
			internalStorage.UpdateUI += new EventHandler(OnStorageUpdateUI);
		}
		#endregion
		#region UnsubscribeStorageEvents
		protected internal virtual void UnsubscribeStorageEvents() {
			IInternalSchedulerStorageBase internalStorage = Storage as IInternalSchedulerStorageBase;
			if (internalStorage == null)
				return;
			internalStorage.InternalAppointmentCollectionCleared -= new EventHandler(OnAppointmentCollectionCleared);
			internalStorage.InternalAppointmentCollectionLoaded -= new EventHandler(OnAppointmentCollectionLoaded);
			internalStorage.InternalAppointmentsInserted -= new PersistentObjectsEventHandler(OnAppointmentsInserted);
			internalStorage.InternalAppointmentsChanged -= new PersistentObjectsEventHandler(OnAppointmentsChanged);
			internalStorage.InternalAppointmentsDeleted -= new PersistentObjectsEventHandler(OnAppointmentsDeleted);
			internalStorage.InternalAppointmentMappingsChanged -= new EventHandler(OnAppointmentMappingsChanged);
			internalStorage.InternalAppointmentUIObjectsChanged -= new EventHandler(OnAppointmentUIObjectsChanged);
			internalStorage.InternalResourceCollectionCleared -= new EventHandler(OnResourceCollectionCleared);
			internalStorage.InternalResourceCollectionLoaded -= new EventHandler(OnResourceCollectionLoaded);
			internalStorage.InternalResourcesInserted -= new PersistentObjectsEventHandler(OnResourcesInserted);
			internalStorage.InternalResourcesChanged -= new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourcesDeleted -= new PersistentObjectsEventHandler(OnResourcesDeleted);
			internalStorage.InternalResourceVisibilityChanged -= new EventHandler(OnResourceVisibilityChanged);
			internalStorage.InternalResourceMappingsChanged -= new EventHandler(OnResourceMappingsChanged);
			internalStorage.InternalResourceSortedColumnsChange -= new EventHandler(OnResourceSortedColumnsChange);
			internalStorage.InternalDependencyCollectionCleared -= new EventHandler(OnDependencyCollectionCleared);
			internalStorage.InternalDependencyCollectionLoaded -= new EventHandler(OnDependencyCollectionLoaded);
			internalStorage.InternalDependenciesInserted -= new PersistentObjectsEventHandler(OnDependenciesInserted);
			internalStorage.InternalDependenciesChanged -= new PersistentObjectsEventHandler(OnDependenciesChanged);
			internalStorage.InternalDependenciesDeleted -= new PersistentObjectsEventHandler(OnDependenciesDeleted);
			internalStorage.InternalDependencyMappingsChanged -= new EventHandler(OnDependencyMappingsChanged);
			internalStorage.InternalDeferredNotifications -= new EventHandler(OnStorageDeferredNotifications);
			internalStorage.InternalReminderAlert -= new ReminderEventHandler(OnReminderAlert);
			internalStorage.BeforeDispose -= new EventHandler(OnBeforeStorageDispose);
			internalStorage.UpdateUI -= new EventHandler(OnStorageUpdateUI);
		}
		#endregion
		#region GetLabelColor
		#endregion
		protected internal IAppointmentStatus GetStatus(int statusId) {
			return Storage.Appointments.Statuses.GetById(statusId);
		}
		void OnStorageUpdateUI(object sender, EventArgs e) {
			RaiseUpdateUI();
		}
		protected internal virtual void OnAppointmentCollectionCleared(object sender, EventArgs e) {
			UnsubscribeAppointmentSelectionControllerEvents();
			try {
				AppointmentSelectionController.OnAppointmentsCleared();
			} finally {
				SubscribeAppointmentSelectionControllerEvents();
			}
			OnAppointmentsChangedCore();
		}
		protected internal virtual void OnAppointmentCollectionLoaded(object sender, EventArgs e) {
			UnsubscribeAppointmentSelectionControllerEvents();
			try {
				AppointmentSelectionController.OnAppointmentsLoaded();
			} finally {
				SubscribeAppointmentSelectionControllerEvents();
			}
			OnAppointmentsChangedCore();
		}
		protected internal virtual void OnAppointmentsInserted(object sender, PersistentObjectsEventArgs e) {
			OnAppointmentsOperationCompleted(e, AppointmentSelectionController.OnAppointmentsInserted);
		}
		protected internal virtual void OnAppointmentsChanged(object sender, PersistentObjectsEventArgs e) {
			OnAppointmentsOperationCompleted(e, AppointmentSelectionController.OnAppointmentsChanged);
		}
		protected internal virtual void OnAppointmentsDeleted(object sender, PersistentObjectsEventArgs e) {
			OnAppointmentsOperationCompleted(e, AppointmentSelectionController.OnAppointmentsDeleted);
		}
		protected internal virtual void OnAppointmentMappingsChanged(object sender, EventArgs e) {
		}
		protected internal virtual void OnAppointmentUIObjectsChanged(object sender, EventArgs e) {
			RaiseUpdateUI();
		}
		protected internal delegate void AppointmentBaseCollectionHandler(AppointmentBaseCollection appointments);
		protected internal virtual void OnAppointmentsOperationCompleted(PersistentObjectsEventArgs e, AppointmentBaseCollectionHandler handler) {
			AppointmentBaseCollection appointments = new AppointmentBaseCollection();
			appointments.AddRange(e.Objects);
			UnsubscribeAppointmentSelectionControllerEvents();
			try {
				handler(appointments);
			} finally {
				SubscribeAppointmentSelectionControllerEvents();
			}
			OnAppointmentsChangedCore();
			RaiseUpdateUI();
		}
		protected internal virtual void OnAppointmentsChangedCore() {
			if (ActiveViewType == SchedulerViewType.Day || ActiveViewType == SchedulerViewType.WorkWeek || ActiveViewType == SchedulerViewType.FullWeek)
				ApplyChanges(SchedulerControlChangeType.DayViewActiveStorageAppointmentsChanged);
			else
				ApplyChanges(SchedulerControlChangeType.StorageAppointmentsChanged);
		}
		protected internal virtual void OnStorageDeferredNotifications(object sender, EventArgs e) {
			IInternalSchedulerStorageBase internalStorage = storage as IInternalSchedulerStorageBase;
			SchedulerStorageDeferredChanges deferredChanges = (internalStorage != null) ? internalStorage.DeferredChanges : null;
			if (deferredChanges != null) {
				AppointmentSelectionController.OnStorageDeferredNotifications(deferredChanges);
				AppointmentDependencySelectionController.OnStorageDeferredNotifications(deferredChanges);
			}
			OnStorageDeferredNotificationsCore();
		}
		protected internal virtual void OnStorageDeferredNotificationsCore() {
			ApplyChanges(SchedulerControlChangeType.StorageDeferredNotifications);
		}
		protected internal virtual void OnBeforeStorageDispose(object sender, EventArgs e) {
			this.Storage = null;
		}
		protected internal virtual void OnResourceCollectionCleared(object sender, EventArgs e) {
			OnResourcesChangedCore();
		}
		protected internal virtual void OnResourceCollectionLoaded(object sender, EventArgs e) {
			OnResourcesChangedCore();
		}
		protected internal virtual void OnResourcesInserted(object sender, PersistentObjectsEventArgs e) {
			OnResourcesChangedCore();
		}
		protected internal virtual void OnResourcesChanged(object sender, PersistentObjectsEventArgs e) {
			OnResourcesChangedCore();
		}
		protected internal virtual void OnResourcesDeleted(object sender, PersistentObjectsEventArgs e) {
			OnResourcesChangedCore();
		}
		protected internal virtual void OnResourceMappingsChanged(object sender, EventArgs e) {
		}
		protected internal virtual void OnResourceVisibilityChanged(object sender, EventArgs e) {
			OnResourcesChangedCore();
			OnVisibleResourcesChanged();
		}
		protected internal virtual void OnResourceSortedColumnsChange(object sender, EventArgs e) {
			OnResourcesChangedCore();
		}
		protected internal virtual void OnResourcesChangedCore() {
			ApplyChanges(SchedulerControlChangeType.StorageResourcesChanged);
		}
		protected internal virtual void OnReminderAlert(object sender, ReminderEventArgs e) {
			RaiseReminderAlert(e);
		}
		protected internal virtual void OnDependenciesChangedCore() {
			ApplyChanges(SchedulerControlChangeType.StorageAppointmentDependenciesChanged);
		}
		protected internal virtual void OnDependencyCollectionCleared(object sender, EventArgs e) {
			UnsubscribeAppointmentDependencySelectionControllerEvents();
			try {
				AppointmentDependencySelectionController.OnAppointmentDependenciesCleared();
			} finally {
				SubscribeAppointmentDependencySelectionControllerEvents();
			}
			OnDependenciesChangedCore();
		}
		protected internal virtual void OnDependencyCollectionLoaded(object sender, EventArgs e) {
			UnsubscribeAppointmentDependencySelectionControllerEvents();
			try {
				AppointmentDependencySelectionController.OnAppointmentDependenciesLoaded();
			} finally {
				SubscribeAppointmentDependencySelectionControllerEvents();
			}
			OnDependenciesChangedCore();
		}
		protected internal virtual void OnDependenciesInserted(object sender, PersistentObjectsEventArgs e) {
			OnAppointmentDependenciesOperationCompleted(e, AppointmentDependencySelectionController.OnAppointmentDependenciesInserted);
		}
		protected internal virtual void OnDependenciesChanged(object sender, PersistentObjectsEventArgs e) {
			OnAppointmentDependenciesOperationCompleted(e, AppointmentDependencySelectionController.OnAppointmentDependenciesChanged);
		}
		protected internal virtual void OnDependenciesDeleted(object sender, PersistentObjectsEventArgs e) {
			OnAppointmentDependenciesOperationCompleted(e, AppointmentDependencySelectionController.OnAppointmentDependenciesDeleted);
		}
		protected internal virtual void OnDependencyMappingsChanged(object sender, EventArgs e) {
		}
		protected internal delegate void AppointmentDependencyBaseCollectionHandler(AppointmentDependencyBaseCollection dependencies);
		protected internal virtual void OnAppointmentDependenciesOperationCompleted(PersistentObjectsEventArgs e, AppointmentDependencyBaseCollectionHandler handler) {
			AppointmentDependencyBaseCollection dependencies = new AppointmentDependencyBaseCollection();
			dependencies.AddRange(e.Objects);
			UnsubscribeAppointmentDependencySelectionControllerEvents();
			try {
				handler(dependencies);
			} finally {
				SubscribeAppointmentDependencySelectionControllerEvents();
			}
			OnDependenciesChangedCore();
		}
		protected internal virtual TimeZoneHelper CreateTimeZoneEngine() {
			IInternalSchedulerStorageBase internalStorage = Storage as IInternalSchedulerStorageBase;
			TimeZoneEngine storagaTimeZoneEngine = (internalStorage == null) ? null : internalStorage.TimeZoneEngine;
			return new TimeZoneHelper(storagaTimeZoneEngine);
		}
		protected internal virtual void UpdateTimeZoneEngine() {
			TimeZoneHelper.ClientTimeZone = TimeZoneInfo.FindSystemTimeZoneById(OptionsBehavior.ClientTimeZoneId);
		}
		protected internal virtual void CreateAppointmentCache(object caller) {
			storage.RegisterClient(caller);
		}
		protected internal virtual void DeleteAppointmentCache(object caller) {
			storage.UnregisterClient(caller);
		}
		protected internal virtual void SetStorageCore(ISchedulerStorageBase value) {
			if (storage != null) {
				UnsubscribeStorageEvents();
				DeleteAppointmentCache(owner);
			}
			this.storage = value;
			TimeZoneEngineSyncronizer.SetStorage(this.storage);
			if (storage != null) {
				SubscribeStorageEvents();
				CreateAppointmentCache(owner);
			}
		}
		protected internal virtual void OnStorageChanged() {
			AppointmentSelectionController.OnStorageChanged();
			AppointmentDependencySelectionController.OnStorageChanged();
			RaiseStorageChanged();
		}
		protected internal virtual bool IsAppointmentCurrentlyEdited(Appointment apt) {
			return AppointmentChangeHelper.GetEditedAppointment(apt) != null;
		}
		protected internal virtual bool IsInAppointmentChangingState() {
			return AppointmentChangeHelper.Active;
		}
		#region ISchedulerControlChangeTarget implementation
		void ISchedulerControlChangeTarget.UpdatePaintStyle() {
			owner.UpdatePaintStyle();
		}
		bool ISchedulerControlChangeTarget.IsDateTimeScrollbarVisibilityDependsOnClientSize() {
			return owner.IsDateTimeScrollbarVisibilityDependsOnClientSize();
		}
		void ISchedulerControlChangeTarget.RecalcClientBounds() {
			owner.RecalcClientBounds();
		}
		bool ISchedulerControlChangeTarget.ChangeResourceScrollBarOrientationIfNeeded() {
			return owner.ChangeResourceScrollBarOrientationIfNeeded();
		}
		bool ISchedulerControlChangeTarget.ChangeDateTimeScrollBarOrientationIfNeeded() {
			return owner.ChangeDateTimeScrollBarOrientationIfNeeded();
		}
		bool ISchedulerControlChangeTarget.ChangeResourceScrollBarVisibilityIfNeeded() {
			return owner.ChangeResourceScrollBarVisibilityIfNeeded();
		}
		bool ISchedulerControlChangeTarget.ChangeDateTimeScrollBarVisibilityIfNeeded() {
			return owner.ChangeDateTimeScrollBarVisibilityIfNeeded();
		}
		void ISchedulerControlChangeTarget.RecalcViewBounds() {
			owner.RecalcViewBounds();
		}
		void ISchedulerControlChangeTarget.RecalcScrollBarVisibility() {
			owner.RecalcScrollBarVisibility();
		}
		void ISchedulerControlChangeTarget.EnsureCalculationsAreFinished() {
			owner.EnsureCalculationsAreFinished();
		}
		void ISchedulerControlChangeTarget.RecreateViewInfo() {
			owner.RecreateViewInfo();
		}
		ChangeActions ISchedulerControlChangeTarget.PrepareChangeActions() {
			return owner.PrepareChangeActions();
		}
		ChangeActions ISchedulerControlChangeTarget.RecalcFinalLayout() {
			if (Storage == null)
				return ChangeActions.None;
			TimeInterval oldInterval = ActiveView.GetVisibleIntervals().Interval;
			owner.RecalcFinalLayout();
			TimeInterval newInterval = ActiveView.GetVisibleIntervals().Interval;
			return TimeInterval.Equals(oldInterval, newInterval) ? ChangeActions.None : ChangeActions.RaiseVisibleIntervalChanged;
		}
		void ISchedulerControlChangeTarget.RecalcPreliminaryLayout() {
			owner.RecalcPreliminaryLayout();
		}
		void ISchedulerControlChangeTarget.ClearPreliminaryAppointmentsAndCellContainers() {
			owner.ClearPreliminaryAppointmentsAndCellContainers();
		}
		void ISchedulerControlChangeTarget.RecalcAppointmentsLayout() {
			owner.RecalcAppointmentsLayout();
		}
		void ISchedulerControlChangeTarget.RecalcDraggingAppointmentPosition() {
			owner.RecalcDraggingAppointmentPosition();
		}
		bool ISchedulerControlChangeTarget.ObtainDateTimeScrollBarVisibility() {
			return owner.ObtainDateTimeScrollBarVisibility();
		}
		void ISchedulerControlChangeTarget.UpdateScrollBarsPosition() {
			owner.UpdateScrollBarsPosition();
		}
		void ISchedulerControlChangeTarget.UpdateDateTimeScrollBarValue() {
			owner.UpdateDateTimeScrollBarValue();
		}
		void ISchedulerControlChangeTarget.UpdateResourceScrollBarValue() {
			owner.UpdateResourceScrollBarValue();
		}
		ChangeActions ISchedulerControlChangeTarget.QueryResources() {
			if (!IsInAppointmentChangingState())
				return activeView.QueryResources();
			else {
				return activeView.QueryVisibleResources();
			}
		}
		ChangeActions ISchedulerControlChangeTarget.QueryAppointments() {
			if (ForceQueryAppointments || !IsInAppointmentChangingState())
				return activeView.QueryAppointments();
			return ChangeActions.None;
		}
		void ISchedulerControlChangeTarget.AssignLimitInterval() {
			activeView.LimitInterval = this.LimitInterval;
		}
		ChangeActions ISchedulerControlChangeTarget.UpdateVisibleIntervals() {
			return activeView.UpdateVisibleIntervals(Selection);
		}
		ChangeActions ISchedulerControlChangeTarget.SynchronizeSelectionInterval(bool activeViewChanged) {
			return activeView.SynchronizeSelectionInterval(Selection, activeViewChanged);
		}
		ChangeActions ISchedulerControlChangeTarget.SynchronizeOrResetSelectionInterval(bool activeViewChanged) {
			return activeView.SynchronizeOrResetSelectionInterval(Selection, activeViewChanged);
		}
		ChangeActions ISchedulerControlChangeTarget.SynchronizeSelectionResource() {
			return activeView.SynchronizeSelectionResource(Selection);
		}
		ChangeActions ISchedulerControlChangeTarget.ValidateSelectionResource() {
			return activeView.ValidateSelectionResource(Selection);
		}
		void ISchedulerControlChangeTarget.RaiseSelectionChanged() {
			RaiseSelectionChanged();
		}
		void ISchedulerControlChangeTarget.UpdateStateIdentity() {
			if (IsUpdateLocked)
				this.deferredStateIdentityChanged = true;
			else
				UpdateStateIdentityCore();
		}
		void UpdateStateIdentityCore() {
			this.stateIdentity++;
		}
		ChangeActions ISchedulerControlChangeTarget.RaiseVisibleIntervalChanged() {
			ChangeActions visibleIntervalChangedResult = RaiseVisibleIntervalChanged();
			AppointmentSelectionController.OnVisibleIntervalChanged();
			AppointmentDependencySelectionController.OnVisibleIntervalChanged();
			return visibleIntervalChangedResult;
		}
		void ISchedulerControlChangeTarget.RepaintView() {
			Owner.RepaintView();
		}
		void ISchedulerControlChangeTarget.UpdateScrollMoreButtonsVisibility() {
			Owner.UpdateScrollMoreButtonsVisibility();
		}
		#endregion
		#region InitializeKeyboardHandlers
		protected internal virtual void InitializeKeyboardHandlers() {
			this.keyboardHandlers = new Stack<KeyboardHandler>();
			NormalKeyboardHandlerBase defaultKeyboardHandler = Owner.CreateKeyboardHandler();
			InitializeKeyboardHandlerDefaults(defaultKeyboardHandler);
			SetNewKeyboardHandler(defaultKeyboardHandler);
		}
		#endregion
		protected internal virtual void InitializeKeyboardHandlerDefaults(NormalKeyboardHandlerBase keyboardHandler) {
			SchedulerViewRepositoryBase views = this.Views;
			keyboardHandler.RegisterKeyHandlerForAllViewsAndGroupTypes(views, Keys.O, Keys.Control, SchedulerCommandId.EditAppointmentQuery);
			keyboardHandler.RegisterKeyHandlerForAllViewsAndGroupTypes(views, Keys.N, Keys.Control, SchedulerCommandId.NewAppointment);
			keyboardHandler.RegisterKeyHandlerForAllViewsAndGroupTypes(views, Keys.G, Keys.Control, SchedulerCommandId.GotoDate);
			keyboardHandler.RegisterKeyHandlerForAllViewsAndGroupTypes(views, Keys.D, Keys.Control, SchedulerCommandId.DeleteAppointmentsQueryOrDependencies);
			keyboardHandler.RegisterKeyHandlerForAllViewsAndGroupTypes(views, Keys.Delete, Keys.None, SchedulerCommandId.DeleteAppointmentsQueryOrDependencies);
			keyboardHandler.RegisterKeyHandlerForAllViewsAndGroupTypes(views, Keys.Add, Keys.Control, SchedulerCommandId.ViewZoomIn);
			keyboardHandler.RegisterKeyHandlerForAllViewsAndGroupTypes(views, Keys.Subtract, Keys.Control, SchedulerCommandId.ViewZoomOut);
			keyboardHandler.RegisterKeyHandlerForAllViewsAndGroupTypes(views, Keys.F2, Keys.None, SchedulerCommandId.EditAppointmentViaInplaceEditor);
			keyboardHandler.RegisterKeyHandlerForAllViewsAndGroupTypes(views, Keys.Enter, Keys.None, SchedulerCommandId.EditAppointmentOrNewAppointmentViaInplaceEditor);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.Home, Keys.Alt, SchedulerCommandId.DayViewMoveFirstDayOfWeek);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.End, Keys.Alt, SchedulerCommandId.DayViewMoveLastDayOfWeek);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.PageUp, Keys.Alt, SchedulerCommandId.DayViewMoveStartOfMonth);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.PageDown, Keys.Alt, SchedulerCommandId.DayViewMoveEndOfMonth);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.Up, Keys.Alt, SchedulerCommandId.DayViewMovePrevWeek);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.Down, Keys.Alt, SchedulerCommandId.DayViewMoveNextWeek);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.Home, Keys.Alt | Keys.Shift, SchedulerCommandId.DayViewExtendFirstDayOfWeek);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.End, Keys.Alt | Keys.Shift, SchedulerCommandId.DayViewExtendLastDayOfWeek);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.PageUp, Keys.Alt | Keys.Shift, SchedulerCommandId.DayViewExtendStartOfMonth);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.PageDown, Keys.Alt | Keys.Shift, SchedulerCommandId.DayViewExtendEndOfMonth);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.Left, Keys.Shift, SchedulerCommandId.DayViewExtendPrevDay);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.Right, Keys.Shift, SchedulerCommandId.DayViewExtendNextDay);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.Up, Keys.None, SchedulerCommandId.DayViewMovePrevCell);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.Down, Keys.None, SchedulerCommandId.DayViewMoveNextCell);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.Home, Keys.None, SchedulerCommandId.DayViewMoveToStartOfWorkTime);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.End, Keys.None, SchedulerCommandId.DayViewMoveToEndOfWorkTime);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.Home, Keys.Control, SchedulerCommandId.DayViewMoveToStartOfVisibleTime);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.End, Keys.Control, SchedulerCommandId.DayViewMoveToEndOfVisibleTime);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.PageUp, Keys.None, SchedulerCommandId.DayViewMovePageUp);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.PageDown, Keys.None, SchedulerCommandId.DayViewMovePageDown);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.Up, Keys.Shift, SchedulerCommandId.DayViewExtendPrevCell);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.Down, Keys.Shift, SchedulerCommandId.DayViewExtendNextCell);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.Home, Keys.Shift, SchedulerCommandId.DayViewExtendToStartOfWorkTime);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.End, Keys.Shift, SchedulerCommandId.DayViewExtendToEndOfWorkTime);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.Home, Keys.Control | Keys.Shift, SchedulerCommandId.DayViewExtendToStartOfVisibleTime);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.End, Keys.Control | Keys.Shift, SchedulerCommandId.DayViewExtendToEndOfVisibleTime);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.PageUp, Keys.Shift, SchedulerCommandId.DayViewExtendPageUp);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(SchedulerViewType.Day, Keys.PageDown, Keys.Shift, SchedulerCommandId.DayViewExtendPageDown);
			IKeyHashProvider provider;
			provider = new SchedulerKeyHashProvider(SchedulerViewType.Day, SchedulerGroupType.None);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.None, SchedulerCommandId.DayViewMovePrevDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.None, SchedulerCommandId.DayViewMoveNextDay);
			provider = new SchedulerKeyHashProvider(SchedulerViewType.Day, SchedulerGroupType.Resource);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.None, SchedulerCommandId.DayViewGroupByResourceMovePrevDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.None, SchedulerCommandId.DayViewGroupByResourceMoveNextDay);
			provider = new SchedulerKeyHashProvider(SchedulerViewType.Day, SchedulerGroupType.Date);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.None, SchedulerCommandId.DayViewGroupByDateMovePrevDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.None, SchedulerCommandId.DayViewGroupByDateMoveNextDay);
			RegisterKeyboardHandlerForWorkWeekView(SchedulerViewType.WorkWeek, keyboardHandler);
			RegisterKeyboardHandlerForWorkWeekView(SchedulerViewType.FullWeek, keyboardHandler);
			provider = new SchedulerKeyHashProvider(SchedulerViewType.Week, SchedulerGroupType.None);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.None, SchedulerCommandId.WeekViewMoveLeft);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.None, SchedulerCommandId.WeekViewMoveRight);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.None, SchedulerCommandId.WeekViewMovePrevDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.None, SchedulerCommandId.WeekViewMoveNextDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.None, SchedulerCommandId.WeekViewMoveFirstDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.None, SchedulerCommandId.WeekViewMoveLastDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.None, SchedulerCommandId.WeekViewMovePageUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.None, SchedulerCommandId.WeekViewMovePageDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Alt, SchedulerCommandId.WeekViewMoveStartOfMonth);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Alt, SchedulerCommandId.WeekViewMoveEndOfMonth);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Shift, SchedulerCommandId.WeekViewExtendLeft);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Shift, SchedulerCommandId.WeekViewExtendRight);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.Shift, SchedulerCommandId.WeekViewExtendPrevDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.Shift, SchedulerCommandId.WeekViewExtendNextDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.Shift, SchedulerCommandId.WeekViewExtendFirstDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.Shift, SchedulerCommandId.WeekViewExtendLastDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Shift, SchedulerCommandId.WeekViewExtendPageUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Shift, SchedulerCommandId.WeekViewExtendPageDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Alt | Keys.Shift, SchedulerCommandId.WeekViewExtendStartOfMonth);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Alt | Keys.Shift, SchedulerCommandId.WeekViewExtendEndOfMonth);
			provider = new SchedulerKeyHashProvider(SchedulerViewType.Week, SchedulerGroupType.Resource);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.None, SchedulerCommandId.WeekViewGroupByResourceMoveLeft);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.None, SchedulerCommandId.WeekViewGroupByResourceMoveRight);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.None, SchedulerCommandId.WeekViewMovePrevDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.None, SchedulerCommandId.WeekViewMoveNextDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.None, SchedulerCommandId.WeekViewMoveFirstDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.None, SchedulerCommandId.WeekViewMoveLastDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.None, SchedulerCommandId.WeekViewMovePageUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.None, SchedulerCommandId.WeekViewMovePageDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Alt, SchedulerCommandId.WeekViewMoveStartOfMonth);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Alt, SchedulerCommandId.WeekViewMoveEndOfMonth);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Shift, SchedulerCommandId.WeekViewExtendLeft);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Shift, SchedulerCommandId.WeekViewExtendRight);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.Shift, SchedulerCommandId.WeekViewExtendPrevDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.Shift, SchedulerCommandId.WeekViewExtendNextDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.Shift, SchedulerCommandId.WeekViewExtendFirstDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.Shift, SchedulerCommandId.WeekViewExtendLastDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Shift, SchedulerCommandId.WeekViewExtendPageUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Shift, SchedulerCommandId.WeekViewExtendPageDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Alt | Keys.Shift, SchedulerCommandId.WeekViewExtendStartOfMonth);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Alt | Keys.Shift, SchedulerCommandId.WeekViewExtendEndOfMonth);
			provider = new SchedulerKeyHashProvider(SchedulerViewType.Week, SchedulerGroupType.Date);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.None, SchedulerCommandId.WeekViewMovePrevDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.None, SchedulerCommandId.WeekViewMoveNextDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.None, SchedulerCommandId.WeekViewGroupByDateMoveUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.None, SchedulerCommandId.WeekViewGroupByDateMoveDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.None, SchedulerCommandId.WeekViewMoveFirstDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.None, SchedulerCommandId.WeekViewMoveLastDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.None, SchedulerCommandId.WeekViewMovePageUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.None, SchedulerCommandId.WeekViewMovePageDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Alt, SchedulerCommandId.WeekViewMoveStartOfMonth);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Alt, SchedulerCommandId.WeekViewMoveEndOfMonth);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Shift, SchedulerCommandId.WeekViewExtendPrevDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Shift, SchedulerCommandId.WeekViewExtendNextDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.Shift, SchedulerCommandId.WeekViewGroupByDateExtendUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.Shift, SchedulerCommandId.WeekViewGroupByDateExtendDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.Shift, SchedulerCommandId.WeekViewExtendFirstDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.Shift, SchedulerCommandId.WeekViewExtendLastDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Shift, SchedulerCommandId.WeekViewExtendPageUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Shift, SchedulerCommandId.WeekViewExtendPageDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Alt | Keys.Shift, SchedulerCommandId.WeekViewExtendStartOfMonth);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Alt | Keys.Shift, SchedulerCommandId.WeekViewExtendEndOfMonth);
			provider = new SchedulerKeyHashProvider(SchedulerViewType.Month, SchedulerGroupType.None);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.None, SchedulerCommandId.MonthViewMovePrevDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.None, SchedulerCommandId.MonthViewMoveNextDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.None, SchedulerCommandId.MonthViewMoveUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.None, SchedulerCommandId.MonthViewMoveDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.None, SchedulerCommandId.MonthViewMoveFirstDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.None, SchedulerCommandId.MonthViewMoveLastDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.None, SchedulerCommandId.MonthViewMovePageUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.None, SchedulerCommandId.MonthViewMovePageDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Alt, SchedulerCommandId.MonthViewMoveStartOfMonth);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Alt, SchedulerCommandId.MonthViewMoveEndOfMonth);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Shift, SchedulerCommandId.MonthViewExtendPrevDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Shift, SchedulerCommandId.MonthViewExtendNextDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.Shift, SchedulerCommandId.MonthViewExtendUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.Shift, SchedulerCommandId.MonthViewExtendDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.Shift, SchedulerCommandId.MonthViewExtendFirstDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.Shift, SchedulerCommandId.MonthViewExtendLastDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Shift, SchedulerCommandId.MonthViewExtendPageUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Shift, SchedulerCommandId.MonthViewExtendPageDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Alt | Keys.Shift, SchedulerCommandId.MonthViewExtendStartOfMonth);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Alt | Keys.Shift, SchedulerCommandId.MonthViewExtendEndOfMonth);
			provider = new SchedulerKeyHashProvider(SchedulerViewType.Month, SchedulerGroupType.Resource);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.None, SchedulerCommandId.MonthViewGroupByResourceMoveLeft);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.None, SchedulerCommandId.MonthViewGroupByResourceMoveRight);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.None, SchedulerCommandId.MonthViewMoveUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.None, SchedulerCommandId.MonthViewMoveDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.None, SchedulerCommandId.MonthViewMoveFirstDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.None, SchedulerCommandId.MonthViewMoveLastDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.None, SchedulerCommandId.MonthViewMovePageUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.None, SchedulerCommandId.MonthViewMovePageDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Alt, SchedulerCommandId.MonthViewMoveStartOfMonth);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Alt, SchedulerCommandId.MonthViewMoveEndOfMonth);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Shift, SchedulerCommandId.MonthViewExtendPrevDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Shift, SchedulerCommandId.MonthViewExtendNextDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.Shift, SchedulerCommandId.MonthViewExtendUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.Shift, SchedulerCommandId.MonthViewExtendDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.Shift, SchedulerCommandId.MonthViewExtendFirstDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.Shift, SchedulerCommandId.MonthViewExtendLastDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Shift, SchedulerCommandId.MonthViewExtendPageUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Shift, SchedulerCommandId.MonthViewExtendPageDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Alt | Keys.Shift, SchedulerCommandId.MonthViewExtendStartOfMonth);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Alt | Keys.Shift, SchedulerCommandId.MonthViewExtendEndOfMonth);
			provider = new SchedulerKeyHashProvider(SchedulerViewType.Month, SchedulerGroupType.Date);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.None, SchedulerCommandId.MonthViewMovePrevDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.None, SchedulerCommandId.MonthViewMoveNextDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.None, SchedulerCommandId.MonthViewGroupByDateMoveUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.None, SchedulerCommandId.MonthViewGroupByDateMoveDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.None, SchedulerCommandId.MonthViewMoveFirstDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.None, SchedulerCommandId.MonthViewMoveLastDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.None, SchedulerCommandId.MonthViewMovePageUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.None, SchedulerCommandId.MonthViewMovePageDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Alt, SchedulerCommandId.MonthViewMoveStartOfMonth);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Alt, SchedulerCommandId.MonthViewMoveEndOfMonth);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Shift, SchedulerCommandId.MonthViewExtendPrevDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Shift, SchedulerCommandId.MonthViewExtendNextDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.Shift, SchedulerCommandId.MonthViewExtendUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.Shift, SchedulerCommandId.MonthViewExtendDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.Shift, SchedulerCommandId.MonthViewExtendFirstDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.Shift, SchedulerCommandId.MonthViewExtendLastDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Shift, SchedulerCommandId.MonthViewExtendPageUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Shift, SchedulerCommandId.MonthViewExtendPageDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Alt | Keys.Shift, SchedulerCommandId.MonthViewExtendStartOfMonth);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Alt | Keys.Shift, SchedulerCommandId.MonthViewExtendEndOfMonth);
			provider = new SchedulerKeyHashProvider(SchedulerViewType.Timeline, SchedulerGroupType.None);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.None, SchedulerCommandId.TimelineViewMovePrevDate);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.None, SchedulerCommandId.TimelineViewMoveNextDate);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.None, SchedulerCommandId.TimelineViewMoveToStartOfVisibleTime);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.None, SchedulerCommandId.TimelineViewMoveToEndOfVisibleTime);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Alt, SchedulerCommandId.TimelineViewMoveToMajorStart);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Alt, SchedulerCommandId.TimelineViewMoveToMajorEnd);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Shift, SchedulerCommandId.TimelineViewExtendPrevDate);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Shift, SchedulerCommandId.TimelineViewExtendNextDate);
			provider = new SchedulerKeyHashProvider(SchedulerViewType.Timeline, SchedulerGroupType.Date);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.None, SchedulerCommandId.TimelineViewMovePrevDate);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.None, SchedulerCommandId.TimelineViewMoveNextDate);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.None, SchedulerCommandId.TimelineViewGroupByDateMoveUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.None, SchedulerCommandId.TimelineViewGroupByDateMoveDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.None, SchedulerCommandId.TimelineViewMoveToStartOfVisibleTime);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.None, SchedulerCommandId.TimelineViewMoveToEndOfVisibleTime);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Alt, SchedulerCommandId.TimelineViewMoveToMajorStart);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Alt, SchedulerCommandId.TimelineViewMoveToMajorEnd);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Shift, SchedulerCommandId.TimelineViewExtendPrevDate);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Shift, SchedulerCommandId.TimelineViewExtendNextDate);
			provider = new SchedulerKeyHashProvider(SchedulerViewType.Timeline, SchedulerGroupType.Resource);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.None, SchedulerCommandId.TimelineViewMovePrevDate);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.None, SchedulerCommandId.TimelineViewMoveNextDate);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.None, SchedulerCommandId.TimelineViewGroupByDateMoveUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.None, SchedulerCommandId.TimelineViewGroupByDateMoveDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.None, SchedulerCommandId.TimelineViewMoveToStartOfVisibleTime);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.None, SchedulerCommandId.TimelineViewMoveToEndOfVisibleTime);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Alt, SchedulerCommandId.TimelineViewMoveToMajorStart);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Alt, SchedulerCommandId.TimelineViewMoveToMajorEnd);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Shift, SchedulerCommandId.TimelineViewExtendPrevDate);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Shift, SchedulerCommandId.TimelineViewExtendNextDate);
			provider = new SchedulerKeyHashProvider(SchedulerViewType.Gantt, SchedulerGroupType.None);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.None, SchedulerCommandId.TimelineViewMovePrevDate);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.None, SchedulerCommandId.TimelineViewMoveNextDate);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.None, SchedulerCommandId.TimelineViewMoveToStartOfVisibleTime);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.None, SchedulerCommandId.TimelineViewMoveToEndOfVisibleTime);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Alt, SchedulerCommandId.TimelineViewMoveToMajorStart);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Alt, SchedulerCommandId.TimelineViewMoveToMajorEnd);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Shift, SchedulerCommandId.TimelineViewExtendPrevDate);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Shift, SchedulerCommandId.TimelineViewExtendNextDate);
			provider = new SchedulerKeyHashProvider(SchedulerViewType.Gantt, SchedulerGroupType.Date);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.None, SchedulerCommandId.TimelineViewMovePrevDate);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.None, SchedulerCommandId.TimelineViewMoveNextDate);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.None, SchedulerCommandId.TimelineViewGroupByDateMoveUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.None, SchedulerCommandId.TimelineViewGroupByDateMoveDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.None, SchedulerCommandId.TimelineViewMoveToStartOfVisibleTime);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.None, SchedulerCommandId.TimelineViewMoveToEndOfVisibleTime);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Alt, SchedulerCommandId.TimelineViewMoveToMajorStart);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Alt, SchedulerCommandId.TimelineViewMoveToMajorEnd);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Shift, SchedulerCommandId.TimelineViewExtendPrevDate);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Shift, SchedulerCommandId.TimelineViewExtendNextDate);
			provider = new SchedulerKeyHashProvider(SchedulerViewType.Gantt, SchedulerGroupType.Resource);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.None, SchedulerCommandId.TimelineViewMovePrevDate);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.None, SchedulerCommandId.TimelineViewMoveNextDate);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.None, SchedulerCommandId.TimelineViewGroupByDateMoveUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.None, SchedulerCommandId.TimelineViewGroupByDateMoveDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.None, SchedulerCommandId.TimelineViewMoveToStartOfVisibleTime);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.None, SchedulerCommandId.TimelineViewMoveToEndOfVisibleTime);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Alt, SchedulerCommandId.TimelineViewMoveToMajorStart);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Alt, SchedulerCommandId.TimelineViewMoveToMajorEnd);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Shift, SchedulerCommandId.TimelineViewExtendPrevDate);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Shift, SchedulerCommandId.TimelineViewExtendNextDate);
		}
		void RegisterKeyboardHandlerForWorkWeekView(SchedulerViewType schedulerViewType, NormalKeyboardHandlerBase keyboardHandler) {
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(schedulerViewType, Keys.Home, Keys.Alt, SchedulerCommandId.WorkWeekViewMoveFirstDayOfWeek);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(schedulerViewType, Keys.End, Keys.Alt, SchedulerCommandId.WorkWeekViewMoveLastDayOfWeek);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(schedulerViewType, Keys.Up, Keys.Alt, SchedulerCommandId.WorkWeekViewMovePrevWeek);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(schedulerViewType, Keys.Down, Keys.Alt, SchedulerCommandId.WorkWeekViewMoveNextWeek);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(schedulerViewType, Keys.Home, Keys.Alt | Keys.Shift, SchedulerCommandId.WorkWeekViewExtendFirstDayOfWeek);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(schedulerViewType, Keys.End, Keys.Alt | Keys.Shift, SchedulerCommandId.WorkWeekViewExtendLastDayOfWeek);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(schedulerViewType, Keys.Left, Keys.Shift, SchedulerCommandId.WorkWeekViewExtendPrevDay);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(schedulerViewType, Keys.Right, Keys.Shift, SchedulerCommandId.WorkWeekViewExtendNextDay);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(schedulerViewType, Keys.Up, Keys.None, SchedulerCommandId.WorkWeekViewMovePrevCell);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(schedulerViewType, Keys.Down, Keys.None, SchedulerCommandId.WorkWeekViewMoveNextCell);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(schedulerViewType, Keys.Home, Keys.None, SchedulerCommandId.WorkWeekViewMoveToStartOfWorkTime);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(schedulerViewType, Keys.End, Keys.None, SchedulerCommandId.WorkWeekViewMoveToEndOfWorkTime);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(schedulerViewType, Keys.Home, Keys.Control, SchedulerCommandId.WorkWeekViewMoveToStartOfVisibleTime);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(schedulerViewType, Keys.End, Keys.Control, SchedulerCommandId.WorkWeekViewMoveToEndOfVisibleTime);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(schedulerViewType, Keys.PageUp, Keys.None, SchedulerCommandId.WorkWeekViewMovePageUp);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(schedulerViewType, Keys.PageDown, Keys.None, SchedulerCommandId.WorkWeekViewMovePageDown);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(schedulerViewType, Keys.Up, Keys.Shift, SchedulerCommandId.WorkWeekViewExtendPrevCell);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(schedulerViewType, Keys.Down, Keys.Shift, SchedulerCommandId.WorkWeekViewExtendNextCell);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(schedulerViewType, Keys.Home, Keys.Shift, SchedulerCommandId.WorkWeekViewExtendToStartOfWorkTime);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(schedulerViewType, Keys.End, Keys.Shift, SchedulerCommandId.WorkWeekViewExtendToEndOfWorkTime);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(schedulerViewType, Keys.Home, Keys.Control | Keys.Shift, SchedulerCommandId.WorkWeekViewExtendToStartOfVisibleTime);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(schedulerViewType, Keys.End, Keys.Control | Keys.Shift, SchedulerCommandId.WorkWeekViewExtendToEndOfVisibleTime);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(schedulerViewType, Keys.PageUp, Keys.Shift, SchedulerCommandId.WorkWeekViewExtendPageUp);
			keyboardHandler.RegisterKeyHandlerForAllGroupTypes(schedulerViewType, Keys.PageDown, Keys.Shift, SchedulerCommandId.WorkWeekViewExtendPageDown);
			SchedulerKeyHashProvider provider = new SchedulerKeyHashProvider(schedulerViewType, SchedulerGroupType.None);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.None, SchedulerCommandId.WorkWeekViewMovePrevDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.None, SchedulerCommandId.WorkWeekViewMoveNextDay);
			provider = new SchedulerKeyHashProvider(schedulerViewType, SchedulerGroupType.Resource);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.None, SchedulerCommandId.WorkWeekViewGroupByResourceMovePrevDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.None, SchedulerCommandId.WorkWeekViewGroupByResourceMoveNextDay);
			provider = new SchedulerKeyHashProvider(schedulerViewType, SchedulerGroupType.Date);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.None, SchedulerCommandId.WorkWeekViewGroupByDateMovePrevDay);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.None, SchedulerCommandId.WorkWeekViewGroupByDateMoveNextDay);
		}
		protected internal virtual void SetNewKeyboardHandler(KeyboardHandler keyboardHandler) {
			keyboardHandlers.Push(keyboardHandler);
		}
		protected internal virtual void RestoreKeyboardHandler() {
			keyboardHandlers.Pop();
		}
		protected internal virtual void InitializeSelection() {
			activeView.InitializeSelection(selection);
		}
		public virtual AppointmentBaseCollection GetNonFilteredAppointments() {
			if (Storage != null)
				return Storage.Appointments.Items;
			else
				return new AppointmentBaseCollection();
		}
		public virtual ResourceBaseCollection GetFilteredResources() {
			if (Storage != null)
				return InternalStorage.GetVisibleResources(true);
			else
				return new ResourceBaseCollection();
		}
		public virtual ResourceBaseCollection GetResourcesTree() {
			return GetResourcesTree(true);
		}
		public virtual ResourceBaseCollection GetResourcesTree(bool useResourcesTreeFilter) {
			if (Storage != null)
				return InternalStorage.GetResourcesTree(useResourcesTreeFilter);
			else
				return new ResourceBaseCollection();
		}
		protected internal virtual AppointmentBaseCollection GetNonRecurringAppointments() {
			if (Storage != null) {
				UnsubscribeStorageEvents();
				try {
					return Storage.GetNonRecurringAppointments();
				} finally {
					SubscribeStorageEvents();
				}
			} else
				return new AppointmentBaseCollection();
		}
		public virtual AppointmentBaseCollection GetFilteredAppointments(TimeInterval interval, ResourceBaseCollection resources, out bool appointmentsAreReloaded) {
			if (Storage != null) {
				UnsubscribeStorageEvents();
				SchedulerStorageReloadListener listener = new SchedulerStorageReloadListener();
				try {
					((IInternalAppointmentStorage)Storage.Appointments).Reload += listener.OnReload;
					((IInternalSchedulerStorageBase)Storage).AfterFetchAppointments += listener.OnReload;
					AppointmentBaseCollection result = GetFilteredAppointmentsCore(interval, resources, owner);
					appointmentsAreReloaded = listener.ShouldReload;
					return result;
				} finally {
					((IInternalAppointmentStorage)Storage.Appointments).Reload -= listener.OnReload;
					((IInternalSchedulerStorageBase)Storage).AfterFetchAppointments -= listener.OnReload;
					SubscribeStorageEvents();
				}
			} else {
				appointmentsAreReloaded = false;
				return GetFilteredAppointmentsCore(interval, resources, owner);
			}
		}
		protected virtual AppointmentResourcesMatchFilter CreateAppointmentResourcesMatchFilter(ResourceBaseCollection resources, TimeZoneHelper timeZoneEngine) {
			Guard.ArgumentNotNull(resources, "resources");
			AppointmentResourcesMatchFilter result = new AppointmentResourcesMatchFilter();
			result.AppointmentExternalFilter = Storage.CreateAppointmentExternalFilterPredicate();
			result.Resources = resources;
			result.ShowOnlyResourceAppointments = OptionsView.ShowOnlyResourceAppointments;
			result.TimeZoneHelper = timeZoneEngine;
			return result;
		}
		protected internal AppointmentResourcesMatchFilter CreateAppointmentResourcesMatchFilter(ResourcePredicatePairCollection predicatePairs) {
			Guard.ArgumentNotNull(predicatePairs, "predicatePairs");
			AppointmentResourcesMatchFilter result = new AppointmentResourcesMatchFilter();
			if (Storage != null)
				result.AppointmentExternalFilter = Storage.CreateAppointmentExternalFilterPredicate();
			result.ShowOnlyResourceAppointments = OptionsView.ShowOnlyResourceAppointments;
			result.TimeZoneHelper = TimeZoneHelper;
			ResourceBaseCollection resources = new ResourceBaseCollection();
			ResourceIdAppointmentPredicateDictionary predicateDictionary = new ResourceIdAppointmentPredicateDictionary();
			int count = predicatePairs.Count;
			for (int i = 0; i < count; i++) {
				ResourceAppointmentsPredicatePair predicatePair = predicatePairs[i];
				Resource resource = predicatePair.Resource;
				resources.Add(resource);
				if (resource.Id == null)
					continue;
				predicateDictionary.Add(resource.Id, predicatePair.Predicate);
			}
			result.Resources = resources;
			result.ResourceAppointmentPredicateDictionary = predicateDictionary;
			return result;
		}
		protected internal virtual Dictionary<TimeInterval, AppointmentBaseCollection> GetFilteredAppointmentsByIntervals(TimeIntervalCollection searchIntervals, ResourceBaseCollection resources, TimeZoneHelper timeZoneEngine, object callerObject) {
			if (Storage != null) {
				AppointmentResourcesMatchFilter filter = CreateAppointmentResourcesMatchFilter(resources, timeZoneEngine);
				return Storage.GetFilteredAppointmentByIntervals(searchIntervals, filter, callerObject);
			}
			return new Dictionary<TimeInterval, AppointmentBaseCollection>();
		}
		protected internal virtual IList<DateTime> GetAppointmentTreeFilteredAppointments(TimeInterval clientInterval, ResourceBaseCollection resources, TimeZoneHelper tzEngine, object callerObject) {
			if (Storage != null) {
				AppointmentResourcesMatchFilter filter = CreateAppointmentResourcesMatchFilter(resources, tzEngine);
				return Storage.GetAppointmentDates(clientInterval, callerObject, filter);
			}
			return new DateTime[0];
		}
		protected internal virtual AppointmentBaseCollection GetFilteredAppointmentsCore(TimeInterval clientInterval, ResourceBaseCollection resources, object callerObject) {
			if (Storage != null) {
				TimeInterval actualInterval = ApplyTimeZone(clientInterval);
				AppointmentBaseCollection appointments = Storage.GetAppointmentsCore(actualInterval, callerObject);
				ProcessorBase<Appointment> filter = CreateResourcesAppointmentsFilter(resources);
				return ApplyResourcesAppointmentsFilter(filter, appointments);
			}
			return new AppointmentBaseCollection();
		}
		internal virtual AppointmentDependencyBaseCollection GetFilteredDependencies(TimeInterval interval) {
			if (Storage != null) {
				UnsubscribeStorageEvents();
				try {
					return GetFilteredDependenciesCore(interval, owner);
				} finally {
					SubscribeStorageEvents();
				}
			} else
				return GetFilteredDependenciesCore(interval, owner);
		}
		protected internal virtual AppointmentDependencyBaseCollection GetFilteredDependenciesCore(TimeInterval clientInterval, object callerObject) {
			if (Storage != null) {
				TimeInterval actualInterval = ApplyTimeZone(clientInterval);
				return InternalStorageImpl.GetFilteredDependencies(actualInterval, callerObject);
			}
			return new AppointmentDependencyBaseCollection();
		}
		protected internal virtual ProcessorBase<Appointment> CreateResourcesAppointmentsFilter(ResourceBaseCollection resources) {
			Guard.ArgumentNotNull(resources, "resources");
			ResourcesAppointmentsFilter resourceFilter = new ResourcesAppointmentsFilter(resources);
			if (!OptionsView.ShowOnlyResourceAppointments)
				return resourceFilter;
			CompositeAppointmentsProcessor compositeFilter = new CompositeAppointmentsProcessor();
			compositeFilter.Items.Add(new NonEmptyResourceAppointmentsFilter());
			compositeFilter.Items.Add(resourceFilter);
			return compositeFilter;
		}
		protected internal virtual SchedulerPredicate<Appointment> CreateResourcesAppointmentPredicate(ResourceBaseCollection resources) {
			Guard.ArgumentNotNull(resources, "resources");
			ResourcesAppointmentPredicate resourceFilter = new ResourcesAppointmentPredicate(resources);
			if (!OptionsView.ShowOnlyResourceAppointments)
				return resourceFilter;
			CompositeAppointmentPredicate compositeFilter = new CompositeAppointmentPredicate();
			compositeFilter.Items.Add(new NonEmptyResourceAppointmentPredicate());
			compositeFilter.Items.Add(resourceFilter);
			return compositeFilter;
		}
		protected internal virtual AppointmentBaseCollection ApplyResourcesAppointmentsFilter(ProcessorBase<Appointment> filter, AppointmentBaseCollection appointments) {
			if (filter == null)
				return appointments;
			filter.Process(appointments);
			return (AppointmentBaseCollection)filter.DestinationCollection;
		}
		protected internal virtual TimeInterval ApplyTimeZone(TimeInterval interval) {
			if (interval.Start == DateTime.MinValue) {
				DateTime newEnd = TimeZoneHelper.FromClientTime(interval.End);
				return new TimeInterval(DateTime.MinValue, newEnd);
			}
			if (interval.End == DateTime.MaxValue) {
				DateTime newStart = TimeZoneHelper.FromClientTime(interval.Start);
				return new TimeInterval(newStart, DateTime.MaxValue);
			}
			DateTime date = TimeZoneHelper.FromClientTime(interval.Start);
			return new TimeInterval(date, interval.Duration);
		}
		protected internal virtual void ApplyChanges(SchedulerControlChangeType change) {
			if (change == SchedulerControlChangeType.GroupTypeChanged)
				RaisePropertyChanged("GroupType");
			if (change == SchedulerControlChangeType.ActiveViewChanged)
				RaisePropertyChanged("GroupType");
			ChangeActions actions = ChangeActionsCalculator.CalculateChangeActions(change);
			ApplyChangesCore(change, actions);
		}
		protected internal virtual void ApplyChangesCore(SchedulerControlChangeType change, ChangeActions actions) {
			List<SchedulerControlChangeType> changeTypes = new List<SchedulerControlChangeType>();
			changeTypes.Add(change);
			ApplyChangesCore(changeTypes, actions);
		}
		protected internal virtual void ApplyChangesCore(List<SchedulerControlChangeType> changeTypes, ChangeActions actions) {
			if (IsUpdateLocked)
				deferredChanges.RegisterChangeActions(changeTypes, actions);
			else {
				ISetSchedulerStateService service = GetService<ISetSchedulerStateService>();
				if (service != null) {
					if (service.IsApplyChanges)
						return;
					service.IsApplyChanges = true;
				}
				SchedulerControlChangeManager changeManager = new SchedulerControlChangeManager(actions);
				RaiseBeforeApplyChanges(changeTypes, changeManager.Actions);
				changeManager.ApplyChanges(this);
				RaiseAfterApplyChanges(changeTypes, changeManager.Actions);
				if (service != null)
					service.IsApplyChanges = false;
			}
		}
		protected internal virtual bool CalculateResourceNavigatorVisibility(ResourceNavigatorVisibility visibility) {
			switch (visibility) {
				case ResourceNavigatorVisibility.Never:
					return false;
				case ResourceNavigatorVisibility.Always:
					return GroupType != SchedulerGroupType.None;
				default:
					return GroupType != SchedulerGroupType.None && ActiveView.CanShowResources();
			}
		}
		CommandBasedKeyboardHandler<SchedulerCommandId> ICommandAwareControl<SchedulerCommandId>.KeyboardHandler { get { return this.KeyboardHandler as CommandBasedKeyboardHandler<SchedulerCommandId>; } }
		bool ICommandAwareControl<SchedulerCommandId>.HandleException(Exception e) {
			return false;
		}
		Command ICommandAwareControl<SchedulerCommandId>.CreateCommand(SchedulerCommandId commandId) {
			return this.CreateCommand(commandId);
		}
		protected internal virtual SchedulerCommand CreateCommand(SchedulerCommandId commandId) {
			ISchedulerCommandFactoryService service = (ISchedulerCommandFactoryService)GetService(typeof(ISchedulerCommandFactoryService));
			if (service == null)
				return null;
			return service.CreateCommand(commandId);
		}
		#region IServiceContainer Members
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			if (serviceManager != null)
				serviceManager.AddService(serviceType, callback, promote);
		}
		public void AddService(Type serviceType, ServiceCreatorCallback callback) {
			if (serviceManager != null)
				serviceManager.AddService(serviceType, callback);
		}
		public void AddService(Type serviceType, object serviceInstance, bool promote) {
			if (serviceManager != null)
				serviceManager.AddService(serviceType, serviceInstance, promote);
		}
		public void AddService(Type serviceType, object serviceInstance) {
			if (serviceManager != null)
				serviceManager.AddService(serviceType, serviceInstance);
		}
		public void RemoveService(Type serviceType, bool promote) {
			if (serviceManager != null)
				serviceManager.RemoveService(serviceType, promote);
		}
		public void RemoveService(Type serviceType) {
			if (serviceManager != null)
				serviceManager.RemoveService(serviceType);
		}
		#endregion
		#region IServiceProvider Members
		public virtual T GetService<T>() {
			return (T)this.GetService(typeof(T));
		}
		public virtual object GetService(Type serviceType) {
			if (serviceManager != null)
				return serviceManager.GetService(serviceType);
			else
				return null;
		}
		#endregion
		public T ReplaceService<T>(T newService) where T : class {
			T oldService = GetService<T>();
			if (oldService != null)
				RemoveService(typeof(T));
			if (newService != null)
				AddService(typeof(T), newService);
			return oldService;
		}
		#region IInnerSchedulerCommandTarget Members
		InnerSchedulerControl IInnerSchedulerCommandTarget.InnerSchedulerControl { get { return this; } }
		#endregion
		#region ISupportAppointmentEdit Members
		void ISupportAppointmentEdit.ShowAppointmentForm(Appointment apt, bool openRecurrenceForm, bool readOnly, CommandSourceType commandSourceType) {
			Owner.SupportAppointmentEdit.ShowAppointmentForm(apt, openRecurrenceForm, readOnly, commandSourceType);
		}
		void ISupportAppointmentEdit.SelectNewAppointment(Appointment apt) {
			Owner.SupportAppointmentEdit.SelectNewAppointment(apt);
		}
		void ISupportAppointmentEdit.BeginEditNewAppointment(Appointment apt) {
			Owner.SupportAppointmentEdit.BeginEditNewAppointment(apt);
		}
		void ISupportAppointmentEdit.RaiseInitNewAppointmentEvent(Appointment apt) {
			Owner.SupportAppointmentEdit.RaiseInitNewAppointmentEvent(apt);
		}
		#endregion
		#region ISupportAppointmentDependencyEdit Members
		void ISupportAppointmentDependencyEdit.ShowAppointmentDependencyForm(AppointmentDependency dependency, bool readOnly, CommandSourceType commandSourceType) {
			Owner.SupportAppointmentDependencyEdit.ShowAppointmentDependencyForm(dependency, readOnly, commandSourceType);
		}
		#endregion
		protected internal AppointmentDependencySelectionController CreateAppointmentDependencySelectionController() {
			return new AppointmentDependencySelectionController(true);
		}
		protected internal TimeScaleCollection GetViewScales() {
			if (ActiveView is InnerGanttView)
				return GanttView.Scales;
			else
				return TimelineView.Scales;
		}
		#region ICommandAwareControl<SchedulerCommandId> Members
		void ICommandAwareControl<SchedulerCommandId>.CommitImeContent() {
		}
		void ICommandAwareControl<SchedulerCommandId>.Focus() {
		}
		#region UpdateUI
		EventHandler onUpdateUI;
		event EventHandler ICommandAwareControl<SchedulerCommandId>.UpdateUI {
			add {
				onUpdateUI += value;
			}
			remove {
				onUpdateUI -= value;
			}
		}
		protected internal void RaiseUpdateUI() {
			if (Storage == null)
				return;
			if (this.onUpdateUI != null)
				this.onUpdateUI(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected internal void ShowPrintPreview() {
			Owner.ShowPrintPreview();
		}
		public void SetNewAppointmentVisualStateCalculator(IAppointmentVisualStateCalculator calculator) {
			this.appointmentVisualStateCalculator.Push(calculator);
		}
		public IAppointmentVisualStateCalculator RestoreAppointmentVisualStateFactory() {
			IAppointmentVisualStateCalculator result = this.appointmentVisualStateCalculator.Pop();
			System.Diagnostics.Debug.Assert(this.appointmentVisualStateCalculator.Count > 0);
			return result;
		}
	}
	#endregion
	public interface ICollectionChangedListener : IDisposable {
		event CancelEventHandler Changing;
		event EventHandler Changed;
	}
	public class ResourceColorSchemasChangedListenerCore : NotificationCollectionChangedListener<SchedulerColorSchemaBase>, ICollectionChangedListener {
		public ResourceColorSchemasChangedListenerCore(ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> collection)
			: base((NotificationCollection<SchedulerColorSchemaBase>)collection) {
		}
	}
	public class SchedulerControlDeferredChanges {
		ChangeActions changeActions = ChangeActions.None;
		List<SchedulerControlChangeType> changeTypes = new List<SchedulerControlChangeType>();
		public ChangeActions ChangeActions { get { return changeActions; } }
		public List<SchedulerControlChangeType> ChangeTypes { get { return changeTypes; } }
		public virtual void RegisterChangeActions(List<SchedulerControlChangeType> changeTypes, ChangeActions actions) {
			this.changeActions |= actions;
			this.changeTypes.AddRange(changeTypes);
		}
	}
	#region SuspendEventTracker
	public class SuspendEventTracker : IBatchUpdateable, IBatchUpdateHandler {
		BatchUpdateHelper batchUpdateHelper;
		public SuspendEventTracker() {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
		}
		public void SuspendEvents() {
			((IBatchUpdateable)this).BeginUpdate();
		}
		public void ResumeEvents() {
			((IBatchUpdateable)this).EndUpdate();
		}
		public bool IsEventsSuspended { get { return ((IBatchUpdateable)this).IsUpdateLocked; } }
		#region IBatchUpdateable implementation
		void IBatchUpdateable.BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		void IBatchUpdateable.EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		void IBatchUpdateable.CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		bool IBatchUpdateable.IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraScheduler {
	#region ResourceNavigatorVisibility
	public enum ResourceNavigatorVisibility {
		Always,
		Never,
		Auto
	}
	#endregion
	#region ToolTipVisibility
	public enum ToolTipVisibility {
		Never,
		Standard,
		Always
	}
	#endregion
}
