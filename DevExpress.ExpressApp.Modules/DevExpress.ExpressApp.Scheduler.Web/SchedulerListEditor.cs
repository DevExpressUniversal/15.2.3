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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Persistent.Base.General;
using DevExpress.Utils;
using DevExpress.Web.ASPxScheduler;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Web.ASPxScheduler.Localization;
using DevExpress.Web.ASPxScheduler.Services.Internal;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.ExpressApp.Scheduler.Web {
	public class ShowHideDateNavigatorSchedulerScriptService : ISchedulerScriptService {
		private string dateNavigatorClientID;
		public static string GetShowHideDateNavigatorScript(string dateNavigatorClientID) {
			return "var dateNavigatorId = '" + dateNavigatorClientID + "'; SetControlVisibility(dateNavigatorId, !GetControlVisible(dateNavigatorId)); ";
		}
		public ShowHideDateNavigatorSchedulerScriptService(string dateNavigatorClientID) {
			this.dateNavigatorClientID = dateNavigatorClientID;
		}
		public string GetShowGotoDateCalendarAction() {
			return "function() {{ " + GetShowHideDateNavigatorScript(dateNavigatorClientID) + "}}";
		}
	}
	public class ASPxSchedulerListEditor : SchedulerListEditorBase,  ITestable, ISupportModelSaving, ISupportInplaceEdit {
		private bool isBound = false;
		private object defaultAppointmentId;
		private Locker refreshLocker;
		private Table containerTable;
		private ASPxScheduler scheduler;
		private ASPxSchedulerStorage SchedulerStorage {
			get { return scheduler != null ? scheduler.Storage : null; }
		}
		private ASPxDateNavigator dateNavigator;
		private CollectionSourceBase collectionSource;
		private ASPxSchedulerContextMenu contextMenu;
		private ASPxSchedulerDeleteRecurrenceControl deleteRecurrenceControl;
		private bool isSchedulerControlLoaded;
		private void scheduler_BeforeExecuteCallbackCommand(object sender, SchedulerCallbackCommandEventArgs e) {
			if(contextMenu != null) {
				if(e.CommandId == SelectionChangedFunctionalCallbackCommand.CommandId) {
					string updateCallbackScript = ((ICallbackManagerHolder)scheduler.Page).CallbackManager.GetScript(scheduler.UniqueID, string.Format("'{0}'", ASPxSchedulerContextMenu.SelectionChangedCommandId));
					e.Command = new SelectionChangedFunctionalCallbackCommand(scheduler, string.Format("{0} {1}", contextMenu.GetMenuUpdateScript(), updateCallbackScript));
				}
			}
			refreshLocker.Lock();
		}
		private void scheduler_AfterExecuteCallbackCommand(object sender, SchedulerCallbackCommandEventArgs e) {
			refreshLocker.Unlock();
			OnISupportModelSaving();
		}
		private void scheduler_AppointmentRowInserted(object sender, ASPxSchedulerDataInsertedEventArgs e) {
			e.KeyFieldValue = defaultAppointmentId;
			WebDataSource dataSource = scheduler.AppointmentDataSource as WebDataSource;
			if(dataSource != null) {
				WebPersistentDataSourceView dataSourceView = dataSource.View as WebPersistentDataSourceView;
				if(dataSourceView != null) {
					e.KeyFieldValue = dataSourceView.LastCreatedObjectKey;
				}
			}
			else {
			}
		}
		private string GetSchedulerId(ASPxScheduler scheduler) {
			return string.IsNullOrEmpty(scheduler.ClientInstanceName) ? scheduler.ClientID : scheduler.ClientInstanceName;
		}
		private void scheduler_Init(object sender, EventArgs e) {
			if(WebApplicationStyleManager.IsNewStyle) {
				ISchedulerScriptService gotoDateScriptService = scheduler.GetService(typeof(ISchedulerScriptService)) as ISchedulerScriptService;
				if(gotoDateScriptService != null) {
					scheduler.RemoveService(typeof(ISchedulerScriptService));
				}
				scheduler.AddService(typeof(ISchedulerScriptService), new ShowHideDateNavigatorSchedulerScriptService(dateNavigator.ClientID));
			}
		}
		private void scheduler_Load(object sender, EventArgs e) {
			scheduler.ClientSideEvents.AppointmentsSelectionChanged = string.Format(@"function(s, e){{ {0}.RaiseFuncCallback('{1}|', '1', xafEvalFunc); }}", GetSchedulerId(scheduler), SelectionChangedFunctionalCallbackCommand.CommandId);
			isSchedulerControlLoaded = true;
			DataBind();
		}
		private void schedulerControl_Unload(object sender, EventArgs e) {
			isSchedulerControlLoaded = false;
			if(SchedulerStorage != null) {
				SchedulerStorage.FetchAppointments -= new FetchAppointmentsEventHandler(SchedulerStorage_FetchAppointments);
			}
		}
		private void refreshLocker_LockedChanged(object sender, LockedChangedEventArgs e) {
			if(!e.Locked && e.PendingCalls.Contains("Refresh")) {
				Refresh();
			}
		}
		private void InitLocalization() {
			if(SchedulerControlCoreLocalizer.Active != null) {
				SchedulerLocalizer.SetActiveLocalizerProvider(new SchedulerControlCoreLocalizerProvider());
				SchedulerLocalizer.Active = SchedulerControlCoreLocalizer.Active;
			}
			if(ASPxSchedulerControlLocalizer.Active != null) {
				ASPxSchedulerLocalizer.SetActiveLocalizerProvider(new ASPxSchedulerControlLocalizerProvider());
				ASPxSchedulerLocalizer.Active = ASPxSchedulerControlLocalizer.Active;
			}
		}
		private void View_ObjectUpdated(object sender, EventArgs e) {
			OnCommitChanges();
		}
		private void scheduler_UnhandledException(object sender, SchedulerUnhandledExceptionEventArgs e) {
			if(ErrorHandling.IsIgnoredException(e.Exception)) {
				e.Handled = true;
			}
		}
		private void Control_Unload(object sender, EventArgs e) {
			OnControlInitialized(sender as Control);
		}
		internal void RaiseNewAction(Appointment appointment) {
			OnNewAction(appointment);
		}
		internal void RaiseSelectionChanged() {
			OnSelectionChanged();
			OnFocusedObjectChanged();
		}
		protected virtual ASPxScheduler CreateSchedulerControl() {
			return new ASPxScheduler();
		}
		protected virtual ASPxDateNavigator CreateDateNavigatorControl() {
			return new ASPxDateNavigator();
		}
		protected override SourceObjectHelperBase CreateSourceObjectHelper() {
			return new SourceObjectHelperWeb(ObjectSpace, CollectionSource);
		}
		protected override object CreateResourcesDataSourceCore(Type resourceType) {
			WebDataSource dataSource = new WebDataSource(ObjectSpace, XafTypesInfo.Instance.FindTypeInfo(resourceType), ObjectSpace.CreateCollection(resourceType));
			dataSource.View.ObjectUpdated += new EventHandler<EventArgs>(View_ObjectUpdated);
			return dataSource;
		}
		protected virtual void AssignResourcesDataSource() {
			if(scheduler != null) {
				scheduler.ResourceDataSource = CreateResourcesDataSource();
			}
		}
		protected override SchedulerDeleteHelper CreateDefaultDeleteHelper() {
			return new SchedulerDeleteHelperWeb(this);
		}
		protected override object CreateControlsCore() {
			InitLocalization();
			isBound = false;
			containerTable = RenderHelper.CreateTable();
			containerTable.Width = Unit.Percentage(100);
			scheduler = CreateSchedulerControl();
			scheduler.GroupType = SchedulerGroupType.Date;
			scheduler.DayView.NavigationButtonVisibility = NavigationButtonVisibility.Never;
			scheduler.WorkWeekView.NavigationButtonVisibility = NavigationButtonVisibility.Never;
			scheduler.WeekView.NavigationButtonVisibility = NavigationButtonVisibility.Never;
			scheduler.MonthView.WeekCount = 5;
			scheduler.MonthView.NavigationButtonVisibility = NavigationButtonVisibility.Never;
			scheduler.TimelineView.NavigationButtonVisibility = NavigationButtonVisibility.Never;
			scheduler.TimelineView.Scales.LoadDefaults();
			scheduler.OptionsCustomization.AllowInplaceEditor = UsedAppointmentType.None;
			scheduler.Services.RemoveService(typeof(ISupportCollectionDataSourceService));
			scheduler.Services.AddService(typeof(ISupportCollectionDataSourceService), new SupportCollectionService());
			scheduler.Storage.EnableReminders = false;
			SetupMappings();
			SubscribeSchedulerEvents();
			deleteRecurrenceControl = new ASPxSchedulerDeleteRecurrenceControl(scheduler);
			deleteRecurrenceControl.ID = "DRC";
			dateNavigator = CreateDateNavigatorControl();
			FillTable(containerTable);
			((SchedulerDeleteHelperWeb)SchedulerDeleteHelper).SetupControl(deleteRecurrenceControl, collectionSource);
			ApplyModel();
			SetupSorting(scheduler as IServiceContainer);
			dateNavigator.MasterControlID = scheduler.ID;
			dateNavigator.ID = scheduler.ID + "_DateNavigator";
			dateNavigator.Properties.ShowTodayButton = false;
			if(WebApplicationStyleManager.IsNewStyle) {
				dateNavigator.Width = Unit.Percentage(100);
			}
			containerTable.Unload += new EventHandler(Control_Unload);
			return containerTable;
		}
		private void FillTable(Table containerTable) {
			if(WebApplicationStyleManager.IsNewStyle) {
				TableRow row1 = new TableRow();
				containerTable.Rows.Add(row1);
				TableCell cell = new TableCell();
				row1.Cells.Add(cell);
				dateNavigator.CssClass += " dateNavigatorIndent";
				cell.Controls.Add(dateNavigator);
				TableRow row2 = new TableRow();
				containerTable.Rows.Add(row2);
				TableCell cell2 = new TableCell();
				row2.Cells.Add(cell2);
				cell2.Controls.Add(deleteRecurrenceControl);
				cell2.Controls.Add(scheduler);
			}
			else {
				TableRow row1 = new TableRow();
				containerTable.Rows.Add(row1);
				TableCell cell1 = new TableCell();
				TableCell cell2 = new TableCell();
				row1.Cells.Add(cell1);
				row1.Cells.Add(cell2);
				cell1.Style["vertical-align"] = "top";
				cell1.Style["padding-right"] = "5px";
				cell2.Style["vertical-align"] = "top";
				cell1.Controls.Add(deleteRecurrenceControl);
				cell1.Controls.Add(scheduler);
				cell2.Controls.Add(dateNavigator);
			}
		}
		protected void OnControlInitialized(Control control) {
			if(ControlInitialized != null) {
				ControlInitialized(this, new ControlInitializedEventArgs(control));
			}
		}
		protected override void SelectAppointment(Appointment appointment) {
			scheduler.ActiveView.SelectAppointment(appointment);
		}
		protected override void SetupEditModeDependentOptions() {
			base.SetupEditModeDependentOptions();
			if(AllowEdit) {
				OptionsCustomization.AllowInplaceEditor = UsedAppointmentType.None;
				OptionsCustomization.AllowAppointmentEdit = UsedAppointmentType.All;
			}
			else {
				OptionsCustomization.AllowAppointmentEdit = UsedAppointmentType.None;
			}
		}
		protected override void OnAppointmentsInserted(PersistentObjectsEventArgs e) {
			AppointmentBaseCollection appointments = (AppointmentBaseCollection)e.Objects;
			if(appointments.Count != 1) {
				Exceptions.ThrowArgumentException("appointments.Count", appointments.Count);
			}
			base.OnAppointmentsInserted(e);
		}
		protected override bool IsEqual(IEvent schedulerEvent, Appointment exception) {
			object appointmentId = AppointmentIdHelper.GetAppointmentId(exception);
			return schedulerEvent.AppointmentId.Equals(appointmentId) ||
				ObjectSpace.IsNewObject(schedulerEvent) && appointmentId.Equals(defaultAppointmentId) ||
				base.IsEqual(schedulerEvent, exception);
		}
		protected virtual void OnISupportModelSaving() {
			if(supportModelSavingEvent != null) {
				supportModelSavingEvent(this, EventArgs.Empty);
			}
		}
		protected virtual void OnCommitChanges() {
			if(CommitChanges != null) {
				CommitChanges(this, EventArgs.Empty);
			}
		}
		protected virtual void AssignMainDataSource(Object dataSource) {
			if(SchedulerStorage != null && SchedulerStorage.Appointments != null && isBound) {
				WebDataSource webDataSource = new WebDataSource(ObjectSpace, ObjectTypeInfo, collectionSource.Collection);
				webDataSource.View.ObjectUpdated += new EventHandler<EventArgs>(View_ObjectUpdated);
				scheduler.AppointmentDataSource = webDataSource;
			}
		}
		protected virtual ASPxSchedulerContextMenu CreateCustomContextMenu() {
			return new ASPxSchedulerContextMenu(this);
		}
		protected void DataBind() {
			if(scheduler != null && scheduler.Page != null) {
				refreshLocker.Lock();
				isBound = true;
				dateNavigator.DataBind();
				AssignResourcesDataSource();
				AssignMainDataSource(DataSource);
				scheduler.DataBind();
				refreshLocker.ClearPendingCall("Refresh");
				refreshLocker.Unlock();
			}
		}
		protected override void AssignDataSourceToControl(Object dataSource) {
			DataBind();
		}
		internal new IObjectSpace ObjectSpace {
			get { return base.ObjectSpace; }
		}
		public ASPxSchedulerListEditor(IModelListView model)
			: base(model) {
			refreshLocker = new Locker();
			refreshLocker.LockedChanged += new EventHandler<LockedChangedEventArgs>(refreshLocker_LockedChanged);
			contextMenu = CreateCustomContextMenu();
		}
		public override void ApplyModel() {
			if(Model != null) {
				CancelEventArgs args = new CancelEventArgs();
				OnModelApplying(args);
				if(!args.Cancel) {
					IModelListViewSchedulerWeb modelListViewSchedulerWeb = (IModelListViewSchedulerWeb)Model;
					IModelListViewScheduler modelListView = (IModelListViewScheduler)Model;
					if(scheduler != null) {
						ASPxSchedulerModelSynchronizer.ApplyModel(Model, scheduler);
					}
					if(dateNavigator != null) {
						ASPxDateNavigatorModelSynchronizer.ApplyModel(Model, dateNavigator);
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
						ASPxSchedulerModelSynchronizer.SaveModel(Model, scheduler);
					}
					base.SaveModel();
					OnModelSaved();
				}
			}
		}
		public void SelectEvent(IEvent objectToSelect) {
			Appointment appointment = GetAppointment(objectToSelect);
			if(appointment != null && (SelectedAppointments.Count != 1 || SelectedAppointments[0] != appointment)) {
				SelectedAppointments.Clear();
				SelectedAppointments.Add(appointment);
			}
		}
		public override void BeginUpdate() {
			if(scheduler != null) {
				scheduler.BeginUpdate();
			}
			refreshLocker.Lock();
		}
		public override void EndUpdate() {
			if(scheduler != null && scheduler.IsUpdateLocked) {
				scheduler.EndUpdate();
			}
			refreshLocker.Unlock();
		}
		private void SubscribeSchedulerEvents() {
			scheduler.Init += new EventHandler(scheduler_Init);
			scheduler.Load += new EventHandler(scheduler_Load);
			scheduler.SelectedAppointments.CollectionChanged += new CollectionChangedEventHandler<Appointment>(SelectedAppointments_CollectionChanged);
			scheduler.BeforeExecuteCallbackCommand += new SchedulerCallbackCommandEventHandler(scheduler_BeforeExecuteCallbackCommand);
			scheduler.AfterExecuteCallbackCommand += new SchedulerCallbackCommandEventHandler(scheduler_AfterExecuteCallbackCommand);
			scheduler.AppointmentRowInserted += new ASPxSchedulerDataInsertedEventHandler(scheduler_AppointmentRowInserted);
			scheduler.Unload += new EventHandler(schedulerControl_Unload);
			scheduler.Storage.AppointmentsInserted += new PersistentObjectsEventHandler(SchedulerStorage_AppointmentsInserted);
			scheduler.Storage.AppointmentsChanged += new PersistentObjectsEventHandler(SchedulerStorage_AppointmentsChanged);
			scheduler.Storage.AppointmentDeleting += new PersistentObjectCancelEventHandler(SchedulerStorage_AppointmentDeleting);
			scheduler.Storage.FetchAppointments += new FetchAppointmentsEventHandler(SchedulerStorage_FetchAppointments);
			scheduler.UnhandledException += new SchedulerUnhandledExceptionEventHandler(scheduler_UnhandledException);
		}
		private void UnsubscibeEvents() {
			if(scheduler != null) {
				scheduler.Init -= new EventHandler(scheduler_Init);
				scheduler.Load -= new EventHandler(scheduler_Load);
				scheduler.SelectedAppointments.CollectionChanged -= new CollectionChangedEventHandler<Appointment>(SelectedAppointments_CollectionChanged);
				scheduler.BeforeExecuteCallbackCommand -= new SchedulerCallbackCommandEventHandler(scheduler_BeforeExecuteCallbackCommand);
				scheduler.AfterExecuteCallbackCommand -= new SchedulerCallbackCommandEventHandler(scheduler_AfterExecuteCallbackCommand);
				scheduler.AppointmentRowInserted -= new ASPxSchedulerDataInsertedEventHandler(scheduler_AppointmentRowInserted);
				scheduler.Unload -= new EventHandler(schedulerControl_Unload);
				if(scheduler.Storage != null) {
					scheduler.Storage.AppointmentsInserted -= new PersistentObjectsEventHandler(SchedulerStorage_AppointmentsInserted);
					scheduler.Storage.AppointmentsChanged -= new PersistentObjectsEventHandler(SchedulerStorage_AppointmentsChanged);
					scheduler.Storage.AppointmentDeleting -= new PersistentObjectCancelEventHandler(SchedulerStorage_AppointmentDeleting);
				}
				scheduler.UnhandledException -= new SchedulerUnhandledExceptionEventHandler(scheduler_UnhandledException);
			}
		}
		public override void BreakLinksToControls() {
			base.BreakLinksToControls();
			UnsubscibeEvents();
			if(scheduler != null) {
				if(scheduler.Storage != null) {
					scheduler.Storage.Dispose();
				}
				if(!scheduler.Parent.Controls.IsReadOnly) {
					scheduler.Parent.Controls.Remove(scheduler);
				}
				scheduler.Dispose();
				scheduler = null;
			}
			if(dateNavigator != null) {
				dateNavigator.Dispose();
				dateNavigator = null;
			}
			if(containerTable != null) {
				containerTable.Dispose();
				containerTable = null;
			}
			if(deleteRecurrenceControl != null) {
				deleteRecurrenceControl.Dispose();
				deleteRecurrenceControl = null;
			}
		}
		public override void Dispose() {
			try {
				if(contextMenu != null) {
					contextMenu.Dispose();
					contextMenu = null;
				}
			}
			finally {
				base.Dispose();
			}
		}
		public override void Refresh() {
			if(refreshLocker.Locked) {
				refreshLocker.Call("Refresh");
			}
			else {
				if(!FetchAppointments) {
					DataBind();
				}
			}
		}
		public override Boolean SupportsDataAccessMode(CollectionSourceDataAccessMode dataAccessMode) {
			return
				(dataAccessMode == CollectionSourceDataAccessMode.Client)
				||
				(dataAccessMode == CollectionSourceDataAccessMode.DataView);
		}
		public override IDateNavigatorControllerOwner DateNavigator {
			get { return dateNavigator; }
		}
		public ASPxScheduler SchedulerControl {
			get { return scheduler; }
		}
		public override ISchedulerStorageBase StorageBase {
			get { return SchedulerControl.Storage; }
		}
		public override IAppointmentStorageBase Appointments {
			get { return SchedulerControl.Storage.Appointments; }
		}
		public override AppointmentMappingInfo AppointmentsMappings {
			get { return SchedulerControl.Storage.Appointments.Mappings; }
		}
		public override AppointmentCustomFieldMappingCollection AppointmentsCustomFieldMappings {
			get { return SchedulerControl.Storage.Appointments.CustomFieldMappings; }
		}
		public override IResourceStorageBase Resources {
			get { return SchedulerControl.Storage.Resources; }
		}
		public override ResourceMappingInfo ResourcesMappings {
			get { return SchedulerControl.Storage.Resources.Mappings; }
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
		public override DevExpress.XtraScheduler.Resource SelectedResource {
			get { return SchedulerControl.SelectedResource; }
		}
		public override IContextMenuTemplate ContextMenuTemplate {
			get { return contextMenu; } 
		}
		public override bool IsSchedulerControlLoaded {
			get { return isSchedulerControlLoaded; }
		}
		#region IComplexListEditor Members
		public override void Setup(CollectionSourceBase collectionSource, XafApplication application) {
			base.Setup(collectionSource, application);
			ITypeInfo typeInfo = Model.ModelClass.TypeInfo;
			Type keyPropertyType = typeInfo.IsPersistent ? ObjectSpace.GetKeyPropertyType(typeInfo.Type) : null;
			if(keyPropertyType != null && keyPropertyType.IsValueType) {
				defaultAppointmentId = TypeHelper.CreateInstance(keyPropertyType);
			}
			else {
				defaultAppointmentId = null;
			}
			this.collectionSource = collectionSource;
		}
		#endregion
		#region ITestable Members
		string ITestable.TestCaption {
			get { return Name; }
		}
		string ITestable.ClientId {
			get { return scheduler == null ? "" : scheduler.ClientID; }
		}
		IJScriptTestControl ITestable.TestControl {
			get { return new JSSchedulerEditorTestControl(); }
		}
		public event EventHandler<ControlInitializedEventArgs> ControlInitialized;
		public virtual TestControlType TestControlType {
			get { return TestControlType.Table; }
		}
		#endregion
		#region ISupportModelSaving Members
		private EventHandler<EventArgs> supportModelSavingEvent;
		event EventHandler<EventArgs> ISupportModelSaving.ModelSaving { add { supportModelSavingEvent += value; } remove { supportModelSavingEvent -= value; } }
		#endregion
		#region ISupportInplaceEdit Members
		public event EventHandler<EventArgs> CommitChanges;
		#endregion
		public void OnExporting() { }
		protected override void SetupMappings() {
			base.SetupMappings();
			IPrivateFieldOwner privateFieldOwner = this.AppointmentsMappings as IPrivateFieldOwner;
			if(privateFieldOwner == null)
				return;
			IMemberInfo keyMemberInfo = ObjectTypeInfo.KeyMember;
			if(keyMemberInfo == null)
				return;
			privateFieldOwner.PrivateFields.Add(keyMemberInfo.Name);
		}
#if DebugTest
		public object DebugTest_DefaultAppointmentId {
			get { return defaultAppointmentId; }
		}
#endif
	}
	public class SupportCollectionService : ISupportCollectionDataSourceService {
		public Boolean IsSupported(IEnumerable data) {
			return true;
		}
	}
	public class ASPxSchedulerModelSynchronizer {
		public static void ApplyModel(IModelListView model, ASPxScheduler scheduler) {
			IModelListViewSchedulerWeb modelListViewSchedulerWeb = (IModelListViewSchedulerWeb)model;
			IModelListViewScheduler modelListView = (IModelListViewScheduler)model;
			scheduler.ID = WebIdHelper.GetListEditorControlId(model.Id); 
			scheduler.ActiveViewType = modelListView.SchedulerViewType;
			scheduler.DayView.ResourcesPerPage = modelListView.VisibleResourcesCount;
			scheduler.WorkWeekView.ResourcesPerPage = modelListView.VisibleResourcesCount;
			scheduler.WeekView.ResourcesPerPage = modelListView.VisibleResourcesCount;
			scheduler.MonthView.ResourcesPerPage = modelListView.VisibleResourcesCount;
			scheduler.TimelineView.ResourcesPerPage = modelListView.VisibleResourcesCount;
			scheduler.DayView.TimeScale = modelListView.TimeScale;
			scheduler.WorkWeekView.TimeScale = modelListView.TimeScale;
			TimeInterval visibleInterval = modelListView.SelectedIntervalStart < modelListView.SelectedIntervalEnd ? new TimeInterval(modelListView.SelectedIntervalStart, modelListView.SelectedIntervalEnd) : null;
			if(visibleInterval != null) {
				TimeIntervalCollection timeIntervalCollection = new TimeIntervalCollection();
				timeIntervalCollection.Add(visibleInterval);
				scheduler.ActiveView.SetVisibleIntervals(timeIntervalCollection);
			}
			int scrollAreaHeight = modelListViewSchedulerWeb.ScrollAreaHeight;
			if(scrollAreaHeight != -1) {
				scheduler.DayView.Styles.ScrollAreaHeight = scrollAreaHeight;
				scheduler.WorkWeekView.Styles.ScrollAreaHeight = scrollAreaHeight;
			}
		}
		public static void SaveModel(IModelListView model, ASPxScheduler scheduler) {
			IModelListViewScheduler modelListView = (IModelListViewScheduler)model;
			modelListView.SchedulerViewType = scheduler.ActiveViewType;
			modelListView.VisibleResourcesCount = scheduler.ActiveView.ResourcesPerPage;
			if(scheduler.ActiveViewType == SchedulerViewType.Day) {
				modelListView.TimeScale = scheduler.DayView.TimeScale;
			}
			else if(scheduler.ActiveViewType == SchedulerViewType.WorkWeek) {
				modelListView.TimeScale = scheduler.WorkWeekView.TimeScale;
			}
			else {
				modelListView.TimeScale = TimeSpan.Zero;
			}
			TimeInterval visibleInterval = scheduler.ActiveView.GetVisibleIntervals().Interval;
			if(visibleInterval != null) {
				modelListView.SelectedIntervalStart = visibleInterval.Start;
				modelListView.SelectedIntervalEnd = visibleInterval.End;
			}
			else {
				modelListView.SelectedIntervalStart = default(DateTime);
				modelListView.SelectedIntervalEnd = default(DateTime);
			}
		}
	}
	public class ASPxDateNavigatorModelSynchronizer {
		public static void ApplyModel(IModelListView model, ASPxDateNavigator dateNavigator) {
			IModelListViewSchedulerWeb modelListViewSchedulerWeb = (IModelListViewSchedulerWeb)model;
			if(WebApplicationStyleManager.IsNewStyle) {
				dateNavigator.Properties.Columns = modelListViewSchedulerWeb.DateNavigatorRowCount;
			}
			else {
				dateNavigator.Properties.Rows = modelListViewSchedulerWeb.DateNavigatorRowCount;
			}
		}
	}
	#region Obsolete since 15.1
	[Obsolete("Use ASPxSchedulerListEditor.ControlSettingsSaving instead.")]
	public class ASPxSchedulerControlModelSynchronizer : SchedulerControlModelSynchronizer<ASPxScheduler> {
		public ASPxSchedulerControlModelSynchronizer(ASPxScheduler control, IModelListView model)
			: base(control, model) {
		}
		protected override int ResourcesPerPage {
			get { return Control.ActiveView.ResourcesPerPage; }
			set {
				Control.DayView.ResourcesPerPage = value;
				Control.WorkWeekView.ResourcesPerPage = value;
				Control.WeekView.ResourcesPerPage = value;
				Control.MonthView.ResourcesPerPage = value;
				Control.TimelineView.ResourcesPerPage = value;
			}
		}
		protected override SchedulerViewType ViewType {
			get { return Control.ActiveViewType; }
			set { Control.ActiveViewType = value; }
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
		protected override TimeInterval VisibleInterval {
			get { return Control.ActiveView.GetVisibleIntervals().Interval; }
			set {
				if(value != null) {
					TimeIntervalCollection timeIntervalCollection = new TimeIntervalCollection();
					timeIntervalCollection.Add(value);
					Control.ActiveView.SetVisibleIntervals(timeIntervalCollection);
				}
			}
		}
	}
	#endregion
}
