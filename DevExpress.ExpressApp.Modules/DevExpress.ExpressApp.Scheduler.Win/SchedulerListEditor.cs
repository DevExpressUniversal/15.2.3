#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Printing;
using DevExpress.XtraScheduler.Services;
namespace DevExpress.ExpressApp.Scheduler.Win {
	public class SchedulerContextMenuTemplate : IContextMenuTemplate {
		private SchedulerControl scheduler;
		private IBarManagerHolder barManagerHolder;
		protected void OnBoundItemCreating() {
			if(BoundItemCreating != null) {
				BoundItemCreating(this, null);
			}
		}
		public SchedulerContextMenuTemplate(SchedulerControl scheduler) {
			this.scheduler = scheduler;
		}
		#region IContextMenuTemplate Members
		public void CreateActionItems(IFrameTemplate parent, ListView context, ICollection<IActionContainer> contextContainers) {
			if(barManagerHolder != null) {
				barManagerHolder.BarManagerChanged -= new EventHandler(SchedulerContextMenuTemplate_BarManagerChanged);
			}
			barManagerHolder = parent as IBarManagerHolder;
			if(barManagerHolder != null) {
				barManagerHolder.BarManagerChanged += new EventHandler(SchedulerContextMenuTemplate_BarManagerChanged);
				AssignBarManager(barManagerHolder);
			}
		}
		private void AssignBarManager(IBarManagerHolder barManagerHolder) {
			scheduler.MenuManager = barManagerHolder.BarManager;
		}
		private void SchedulerContextMenuTemplate_BarManagerChanged(object sender, EventArgs e) {
			AssignBarManager((IBarManagerHolder)sender);
		}
		public event EventHandler<BoundItemCreatingEventArgs> BoundItemCreating;
		#endregion
		#region IDisposable Members
		public void Dispose() {
			scheduler = null;
			if(barManagerHolder != null) {
				barManagerHolder.BarManagerChanged -= new EventHandler(SchedulerContextMenuTemplate_BarManagerChanged);
			}
			barManagerHolder = null;
		}
		#endregion
	}
	public class SchedulerListEditorPanelControl : PanelControl, IBasePrintableProvider {
		SchedulerListEditor schedulerListEditorCore;
		public SchedulerListEditorPanelControl(SchedulerListEditor sli) { schedulerListEditorCore = sli; }
		protected override void Dispose(bool disposing) {
			if(!disposing) {
				schedulerListEditorCore = null;
			}
			base.Dispose(disposing);
		}
		object IBasePrintableProvider.GetIPrintableImplementer() {
			return schedulerListEditorCore.SchedulerControl;
		}
	}
	public class SchedulerListEditor : SchedulerListEditorBase, IExportable {
		private SchedulerListEditorPanelControl panel;
		private SchedulerControl scheduler;
		private SchedulerStorage schedulerStorage;
		private DateNavigator dateNavigator;
		private IList resourcesDataSource;
		private SchedulerContextMenuTemplate contextMenu;
		private void AssignMainDataSource(Object dataSource) {
			if(schedulerStorage != null && schedulerStorage.Appointments.DataSource != dataSource) {
				try {
					BeginUpdate();
					schedulerStorage.Appointments.DataSource = dataSource;
				}
				finally {
					EndUpdate();
				}
			}
		}
		private void AssignResourcesDataSource() {
			if(schedulerStorage != null && schedulerStorage.Resources.DataSource != resourcesDataSource) {
				try {
					BeginUpdate();
					schedulerStorage.Resources.DataSource = resourcesDataSource;
				}
				finally {
					EndUpdate();
				}
			}
		}
		private void Scheduler_EditAppointmentFormShowing(object sender, AppointmentFormEventArgs e) {
			e.Handled = true;
			Object schedulerEvent = e.Appointment.GetSourceObject(schedulerStorage);
			if(schedulerEvent != null) {
				scheduler.SelectedAppointments.Clear();
				scheduler.SelectedAppointments.Add(e.Appointment);
				OnProcessSelectedItem();
			}
			else {
				RaiseNewAction(e.Appointment);
			}
		}
		private void scheduler_DeleteRecurrentAppointmentFormShowing(object sender, DeleteRecurrentAppointmentFormEventArgs e) {
			e.Handled = true;
		}
		private void Scheduler_PreparePopupMenu(object sender, PopupMenuShowingEventArgs e) {
			e.Menu.RemoveMenuItem(SchedulerMenuItemId.NewRecurringAppointment);
			if(e.Menu.Id == SchedulerMenuItemId.DefaultMenu) {
				e.Menu.RemoveMenuItem(SchedulerMenuItemId.SwitchToGanttView); 
				SchedulerMenuItem newAppointmentMenuItem = e.Menu.GetMenuItemById(SchedulerMenuItemId.NewAppointment, true);
				if(newAppointmentMenuItem != null) {
				}
			}
			if(e.Menu.Id == SchedulerMenuItemId.AppointmentMenu) {
				if(ObjectType == null || !DataManipulationRight.CanEdit(ObjectType, null, null, CollectionSource, ObjectSpace)) {
					e.Menu.RemoveMenuItem(SchedulerMenuItemId.StatusSubMenu);
					e.Menu.RemoveMenuItem(SchedulerMenuItemId.LabelSubMenu);
				}
			}
		}
		private void scheduler_AppointmentDrop(object sender, AppointmentDragEventArgs e) {
			if(e.SourceAppointment.Type == AppointmentType.Occurrence) {
				AllowSchedulerNativeDeleting = true;
			}
		}
		private void scheduler_HandleCreated(object sender, EventArgs e) {
			AssignDataSourceToControl(DataSource);
		}
		private void schedulerStorage_TryFetchAppointments(object sender, FetchAppointmentsEventArgs e) {
			ISchedulerStateService svc = (ISchedulerStateService)SchedulerControl.GetService<ISchedulerStateService>();
			if(svc != null && (svc.IsAppointmentResized || svc.AreAppointmentsDragged)) {  
				e.ForceReloadAppointments = false;
				return;
			}
			SchedulerStorage_FetchAppointments(sender, e);
		}
		private void SubscribeEvents() {
			scheduler.SelectedAppointments.CollectionChanged += new CollectionChangedEventHandler<Appointment>(SelectedAppointments_CollectionChanged);
			scheduler.EditAppointmentFormShowing += new AppointmentFormEventHandler(Scheduler_EditAppointmentFormShowing);
			scheduler.DeleteRecurrentAppointmentFormShowing += new DeleteRecurrentAppointmentFormEventHandler(scheduler_DeleteRecurrentAppointmentFormShowing);
			scheduler.PopupMenuShowing += new PopupMenuShowingEventHandler(Scheduler_PreparePopupMenu);
			scheduler.AppointmentDrop += new AppointmentDragEventHandler(scheduler_AppointmentDrop);
			scheduler.HandleCreated += new EventHandler(scheduler_HandleCreated);
			schedulerStorage.AppointmentsInserted += new PersistentObjectsEventHandler(SchedulerStorage_AppointmentsInserted);
			schedulerStorage.AppointmentsChanged += new PersistentObjectsEventHandler(SchedulerStorage_AppointmentsChanged);
			schedulerStorage.FetchAppointments += new FetchAppointmentsEventHandler(schedulerStorage_TryFetchAppointments);
			schedulerStorage.AppointmentDeleting += new PersistentObjectCancelEventHandler(SchedulerStorage_AppointmentDeleting);
			dateNavigator.HandleCreated += new EventHandler(scheduler_HandleCreated);
		}
		private void UnsubscribeEvents() {
			if(scheduler != null) {
				scheduler.SelectedAppointments.CollectionChanged -= new CollectionChangedEventHandler<Appointment>(SelectedAppointments_CollectionChanged);
				scheduler.EditAppointmentFormShowing -= new AppointmentFormEventHandler(Scheduler_EditAppointmentFormShowing);
				scheduler.DeleteRecurrentAppointmentFormShowing -= new DeleteRecurrentAppointmentFormEventHandler(scheduler_DeleteRecurrentAppointmentFormShowing);
				scheduler.PopupMenuShowing -= new PopupMenuShowingEventHandler(Scheduler_PreparePopupMenu);
				scheduler.AppointmentDrop -= new AppointmentDragEventHandler(scheduler_AppointmentDrop);
				scheduler.HandleCreated -= new EventHandler(scheduler_HandleCreated);
			}
			if(schedulerStorage != null) {
				schedulerStorage.AppointmentsInserted -= new PersistentObjectsEventHandler(SchedulerStorage_AppointmentsInserted);
				schedulerStorage.AppointmentsChanged -= new PersistentObjectsEventHandler(SchedulerStorage_AppointmentsChanged);
				schedulerStorage.FetchAppointments -= new FetchAppointmentsEventHandler(schedulerStorage_TryFetchAppointments);
				schedulerStorage.AppointmentDeleting -= new PersistentObjectCancelEventHandler(SchedulerStorage_AppointmentDeleting);
			}
			if(dateNavigator != null) {
				dateNavigator.HandleCreated -= new EventHandler(scheduler_HandleCreated);
			}
		}
		protected virtual SchedulerControl CreateSchedulerControl() {
			return new SchedulerControl();
		}
		protected virtual SchedulerStorage CreateSchedulerStorage() {
			return new SchedulerStorage();
		}
		protected virtual DateNavigator CreateDateNavigatorControl() {
#pragma warning disable 0618
			return new ExpressAppDateNavigator();
#pragma warning restore 0618
		}
		protected override object CreateControlsCore() {
			panel = new SchedulerListEditorPanelControl(this);
			panel.Dock = DockStyle.Fill;
			scheduler = CreateSchedulerControl();
			scheduler.BeginInit(); 
			scheduler.Parent = panel;
			scheduler.Dock = DockStyle.Fill;
			scheduler.GroupType = SchedulerGroupType.Date;
			scheduler.DayView.TimeRulers.Add(new TimeRuler());
			scheduler.WorkWeekView.TimeRulers.Add(new TimeRuler());
			scheduler.DayView.NavigationButtonVisibility = NavigationButtonVisibility.Never;
			scheduler.WorkWeekView.NavigationButtonVisibility = NavigationButtonVisibility.Never;
			scheduler.WeekView.NavigationButtonVisibility = NavigationButtonVisibility.Never;
			scheduler.MonthView.WeekCount = 5;
			scheduler.MonthView.NavigationButtonVisibility = NavigationButtonVisibility.Never;
			scheduler.TimelineView.NavigationButtonVisibility = NavigationButtonVisibility.Never;
			scheduler.TimelineView.Scales.LoadDefaults();
			schedulerStorage = CreateSchedulerStorage();
			schedulerStorage.EnableReminders = false;
			scheduler.DataStorage = schedulerStorage;
			dateNavigator = CreateDateNavigatorControl();
			dateNavigator.Dock = DockStyle.Right;
			dateNavigator.WeekNumberRule = DevExpress.XtraEditors.Controls.WeekNumberRule.Default;
			dateNavigator.Parent = panel;
			dateNavigator.Dock = DockStyle.Right;
			dateNavigator.SchedulerControl = scheduler;
			SetupMappings();
			contextMenu = new SchedulerContextMenuTemplate(scheduler);
			SubscribeEvents();
			ApplyModel();
			SetupSorting(scheduler as IServiceContainer);
			scheduler.OptionsCustomization.AllowAppointmentCreate = DataManipulationRight.HasPermissionTo(ObjectType, ObjectAccess.Create, null, null, CollectionSource) ? UsedAppointmentType.All : UsedAppointmentType.None;
			dateNavigator.BringToFront();
			scheduler.BringToFront();
			scheduler.Tag = EasyTestTagHelper.FormatTestTable("SchedulerControl");
			dateNavigator.Tag = EasyTestTagHelper.FormatTestField("DateNavigator");
			scheduler.EndInit();
			((DailyPrintStyle)scheduler.PrintStyles[SchedulerPrintStyleKind.Daily]).CalendarHeaderVisible = DailyPrintStyleCalendarHeaderVisible;
			OnPrintableChanged();
			return panel;
		}		
		protected override void SelectAppointment(Appointment appointment) {
			scheduler.ActiveView.SelectAppointment(appointment);
		}
		protected override void AssignDataSourceToControl(Object dataSource) {
			AssignMainDataSource(dataSource);
			AssignResourcesDataSource();
		}
		protected override SourceObjectHelperBase CreateSourceObjectHelper() {
			return new SourceObjectHelperWin(StorageBase, ObjectSpace);
		}
		protected override object CreateResourcesDataSourceCore(Type resourceType) {
			return ObjectSpace.CreateCollection(resourceType);
		}
		protected override SchedulerDeleteHelper CreateDefaultDeleteHelper() {
			return new SchedulerDeleteHelperWin(this);
		}
		protected void OnPrintableChanged() {
			if(PrintableChanged != null) {
				PrintableChanged(this, new PrintableChangedEventArgs(Printable));
			}
		}
		internal void RecreateResourcesDataSource() {
			object newResourcesDataSource = CreateResourcesDataSource();
			resourcesDataSource = (IList)newResourcesDataSource;
			AssignResourcesDataSource();
		}
		internal void TryRefreshResourcesDataSource() {
			bool resourceDataSourceNeedUpdate = false;
			IList newDataSource = CreateResourcesDataSource() as IList;
			if(newDataSource != null) {
				resourceDataSourceNeedUpdate = resourcesDataSource == null || newDataSource.Count != resourcesDataSource.Count;
				if(!resourceDataSourceNeedUpdate) {
					foreach(object obj in newDataSource) {
						if(!resourcesDataSource.Contains(obj)) {
							resourceDataSourceNeedUpdate = true;
							break;
						}
					}
				}
			}
			if(resourceDataSourceNeedUpdate) {
				resourcesDataSource = newDataSource;
				AssignResourcesDataSource();
			}
		}
		internal void RaiseNewAction(Appointment appointment) {
			OnNewAction(appointment);
		}
		public SchedulerListEditor(IModelListView model)
			: base(model) {
		}
		public override void ApplyModel() {
			if(Model != null) {
				CancelEventArgs args = new CancelEventArgs();
				OnModelApplying(args);
				if(!args.Cancel) {
					if(scheduler != null) {
						SchedulerControlModelSynchronizer.ApplyModel((IModelListViewScheduler)Model, scheduler);
					}
					base.ApplyModel();
					OnModelApplied();
				}
			}
		}
		public override void SaveModel() {
			if(Model != null) {
				CancelEventArgs args = new CancelEventArgs();
				OnModelSaving(args);
				if(!args.Cancel) {
					if(scheduler != null) {
						SchedulerControlModelSynchronizer.SaveModel((IModelListViewScheduler)Model, scheduler);
					}
					base.SaveModel();
					OnModelSaved();
				}
			}
		}
		public override void Dispose() {
			try {
				BreakLinksToControls();
			}
			finally {
				base.Dispose();
			}
		}
		public override void BreakLinksToControls() {
			base.BreakLinksToControls();
			UnsubscribeEvents();
			if(scheduler != null) {
				scheduler.Dispose();
				scheduler = null;
			}
			if(schedulerStorage != null) {
				schedulerStorage.Dispose();
				schedulerStorage = null;
			}
			if(dateNavigator != null) {
				dateNavigator.Dispose();
				dateNavigator = null;
			}
			if(panel != null) {
				panel.Dispose();
				panel = null;
			}
			if(contextMenu != null) {
				contextMenu.Dispose();
				contextMenu = null;
			}
			OnPrintableChanged();
		}
		public override void Refresh() {
			if(schedulerStorage != null) {
				schedulerStorage.RefreshData();
			}
		}
		public override void BeginUpdate() {
			if(scheduler != null) {
				scheduler.BeginUpdate();
			} 
			if(schedulerStorage != null) {
				schedulerStorage.BeginUpdate();
			}
			if(dateNavigator != null) {
				dateNavigator.BeginInit();
			}
		}
		public override void EndUpdate() {
			if(schedulerStorage != null) {
				schedulerStorage.EndUpdate();
			}
			if(scheduler != null) {
				scheduler.EndUpdate();
			}
			if(dateNavigator != null) {
				dateNavigator.EndInit();
			}
		}
		public override Boolean SupportsDataAccessMode(CollectionSourceDataAccessMode dataAccessMode) {
			return
				(dataAccessMode == CollectionSourceDataAccessMode.Client)
				||
				(dataAccessMode == CollectionSourceDataAccessMode.DataView);
		}
		public SchedulerControl SchedulerControl {
			get { return (SchedulerControl)scheduler; }
		}
		public override ISchedulerStorageBase StorageBase {
			get { return SchedulerControl.DataStorage; }
		}
		public override IAppointmentStorageBase Appointments {
			get { return SchedulerControl.DataStorage.Appointments; }
		}
		public override AppointmentMappingInfo AppointmentsMappings {
			get { return SchedulerControl.DataStorage.Appointments.Mappings; }
		}
		public override AppointmentCustomFieldMappingCollection AppointmentsCustomFieldMappings {
			get { return (AppointmentCustomFieldMappingCollection)SchedulerControl.DataStorage.Appointments.CustomFieldMappings; }
		}
		public override IResourceStorageBase Resources {
			get { return SchedulerControl.DataStorage.Resources; }
		}
		public override ResourceMappingInfo ResourcesMappings {
			get { return SchedulerControl.DataStorage.Resources.Mappings; }
		}
		public override SchedulerOptionsCustomization OptionsCustomization {
			get { return SchedulerControl.OptionsCustomization; }
		}
		public override AppointmentBaseCollection SelectedAppointments {
			get { return SchedulerControl.SelectedAppointments; }
		}
		public override DateTime Start {
			get { return SchedulerControl.Start; }
			set { SchedulerControl.Start = value; }
		}
		public override TimeInterval SelectedInterval {
			get { return SchedulerControl.SelectedInterval; }
		}
		public override Resource SelectedResource {
			get { return SchedulerControl.SelectedResource; }
		}
		public override IDateNavigatorControllerOwner DateNavigator {
			get { return dateNavigator; }
		}
		public override IContextMenuTemplate ContextMenuTemplate {
			get { return contextMenu; }
		}
		public IList ResourcesDataSource {
			get { return resourcesDataSource; }
			set {
				resourcesDataSource = value;
				AssignResourcesDataSource();
			}
		}
		public static bool DailyPrintStyleCalendarHeaderVisible = true;
		internal void ReloadObject(object obj) {
			obj = ObjectSpace.GetObject(obj);
			if(obj != null) {
				ObjectSpace.ReloadObject(obj);
			}
		}
		public List<ExportTarget> SupportedExportFormats {
			get {
				if(Printable == null) {
					return new List<ExportTarget>();
				}
				else {
					return new List<ExportTarget>(){
				ExportTarget.Csv,
				ExportTarget.Html,
				ExportTarget.Image,
				ExportTarget.Mht,
				ExportTarget.Pdf,
				ExportTarget.Rtf,
				ExportTarget.Text,
				ExportTarget.Xls,
				ExportTarget.Xlsx
				};
				}
			}
		}
		public IPrintable Printable { get { return scheduler; } }
		public void OnExporting() {}
		public event EventHandler<PrintableChangedEventArgs> PrintableChanged;
	}
	#region Obsolete since 15.1
	[ToolboxItem(false)]
	[Obsolete("Use the DevExpress.XtraScheduler.DateNavigator class instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ExpressAppDateNavigator : DateNavigator {
		public new DateTime StartDate {
			get { return base.StartDate; }
		}
		public DateTime EndDate {
			get { return base.GetEndDate(); }
		}
	}
	[Obsolete("Use SchedulerListEditor.ControlSettingsSaving instead.")]
	public class XafSchedulerControlModelSynchronizer : SchedulerControlModelSynchronizer<SchedulerControl> {
		public XafSchedulerControlModelSynchronizer(SchedulerControl control, IModelListView model)
			: base(control, model) {
		}
		protected override SchedulerViewType ViewType {
			get {
				return Control.ActiveViewType;
			}
			set {
				Control.ActiveViewType = value;
			}
		}
		protected override TimeSpan TimeScale {
			get {
				switch(Control.ActiveViewType) {
					case SchedulerViewType.Day:
						return Control.DayView.TimeScale;
					case SchedulerViewType.WorkWeek:
						return Control.WorkWeekView.TimeScale;
					default:
						return TimeSpan.Zero;
				}
			}
			set {
				if(value != TimeSpan.Zero) {
					Control.DayView.TimeScale = value;
					Control.WorkWeekView.TimeScale = value;
				}
			}
		}
		protected override int ResourcesPerPage {
			get {
				return Control.ActiveView.ResourcesPerPage;
			}
			set {
				Control.DayView.ResourcesPerPage = value;
				Control.WorkWeekView.ResourcesPerPage = value;
				Control.WeekView.ResourcesPerPage = value;
				Control.MonthView.ResourcesPerPage = value;
				Control.TimelineView.ResourcesPerPage = value;
			}
		}
		protected override TimeInterval VisibleInterval {
			get {
				return Control.ActiveView.GetVisibleIntervals().Interval;
			}
			set {
				if(value != null) {
					TimeIntervalCollection timeIntervalCollection;
					if(Control.ActiveViewType == SchedulerViewType.Day) {
						timeIntervalCollection = new DayIntervalCollection();
					}
					else {
						timeIntervalCollection = new TimeIntervalCollection();
					}
					timeIntervalCollection.Add(value);
					Control.ActiveView.SetVisibleIntervals(timeIntervalCollection);
				}
			}
		}
	}
	#endregion
}
