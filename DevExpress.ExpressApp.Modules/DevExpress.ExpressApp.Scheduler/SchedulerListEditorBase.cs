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
using System.Diagnostics.CodeAnalysis;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base.General;
using DevExpress.Utils;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services.Internal;
namespace DevExpress.ExpressApp.Scheduler {
	public class NewActionEventArgs : EventArgs {
		public NewActionEventArgs(Appointment appointment)
			: base() {
			Appointment = appointment;
		}
		public Appointment Appointment;
	}
	public class FilterDataSourceEventArgs : EventArgs {
		private DateTime startDate;
		private DateTime endDate;
		public FilterDataSourceEventArgs(DateTime startDate, DateTime endDate)
			: base() {
			this.startDate = startDate;
			this.endDate = endDate;
		}
		public DateTime StartDate {
			get { return startDate; }
		}
		public DateTime EndDate {
			get { return endDate; }
		}
	}
	public class ResourceDataSourceCreatingEventArgs : System.ComponentModel.HandledEventArgs {
		private Type resourceType;
		private object dataSource;
		public Type ResourceType {
			get { return resourceType; }
		}
		public object DataSource {
			get { return dataSource; }
			set { dataSource = value; }
		}
		public ResourceDataSourceCreatingEventArgs(Type resourceType) {
			this.resourceType = resourceType;
		}
	}
	public class ResourceDataSourceCreatedEventArgs : EventArgs {
		private object dataSource;
		public object DataSource {
			get { return dataSource; }
			set { dataSource = value; }
		}
		public ResourceDataSourceCreatedEventArgs(object dataSource) {
			this.dataSource = dataSource;
		}
		#region Obsolete 15.2
		[Obsolete("Use the ResourceDataSourceCreatedEventArgs(object dataSource) constructor instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public ResourceDataSourceCreatedEventArgs(Type resourceType, object dataSource) : this(dataSource) { }
		#endregion
	}
	public class ExceptionEventCreatedEventArgs : EventArgs {
		private IEvent patternEvent;
		private IEvent exceptionEvent;
		public ExceptionEventCreatedEventArgs(IEvent patternEvent, IEvent exceptionEvent) {
			this.patternEvent = patternEvent;
			this.exceptionEvent = exceptionEvent;
		}
		public IEvent PatternEvent {
			get {
				return patternEvent;
			}
		}
		public IEvent ExceptionEvent {
			get {
				return exceptionEvent;
			}
		}
	}
	internal class EventStartOnComparer : IComparer {
		public int Compare(object x, object y) {
			IEvent eventX = (IEvent)x;
			IEvent eventY = (IEvent)y;
			return Comparer.Default.Compare(eventX.StartOn, eventY.StartOn);
		}
	}
	public class CreateDeleteHelperEventArgs : EventArgs {
		private SchedulerDeleteHelper deleteHelper;
		public CreateDeleteHelperEventArgs(SchedulerDeleteHelper deleteHelper) {
			this.deleteHelper = deleteHelper;
		}
		public SchedulerDeleteHelper DeleteHelper {
			get {
				return deleteHelper;
			}
			set {
				deleteHelper = value;
			}
		}
	}
	public class AppointmentCompareService : IExternalAppointmentCompareService {
		SubjectAppointmentComparer comparer;
		public AppointmentCompareService() {
			this.comparer = new SubjectAppointmentComparer();
		}
		#region IExternalAppointmentCompareService Members
		IComparer<Appointment> IExternalAppointmentCompareService.Comparer { get { return comparer; } }
		#endregion
	}
	public class SubjectAppointmentComparer : IComparer<Appointment> {
		#region IComparer<Appointment> Members
		int IComparer<Appointment>.Compare(Appointment aptX, Appointment aptY) {
			if(aptX == null) {
				return (aptY == null) ? 0 : 1;
			}
			else {
				if(aptY == null) {
					return -1;
				}
				else {
					return CompareAppointmentSubjects(aptX, aptY);
				}
			}
		}
		#endregion
		private int CompareAppointmentSubjects(Appointment aptX, Appointment aptY) {
			return string.Compare(aptX.Subject, aptY.Subject);
		}
	}
	public abstract class SchedulerListEditorBase : ListEditor, IControlOrderProvider, IComplexListEditor, ISupportUpdate {
		public const string SchedulerViewTypeAttribute = "SchedulerViewType";
		private ArrayList sortedEvents;
		private bool fetchAppointments = false;
		private SourceObjectHelperBase sourceObjectHelper;
		private EventAssigner eventAssigner;
		private SchedulerDeleteHelper deleteHelper;
		private bool allowSchedulerNativeDeleting;
		private void EnsureSourceObjectHelper() {
			if(sourceObjectHelper == null) {
				sourceObjectHelper = CreateSourceObjectHelper();
			}
		}
		private void AddRequiredProperties(string value, List<string> requiredProperties){
			if(!requiredProperties.Contains(value)) {
				requiredProperties.Add(value);
			}
		}
		protected void RaiseFilterDataSource(DateTime startDate, DateTime endDate) {
			if(FilterDataSource != null) {
				FilterDataSource(this, new FilterDataSourceEventArgs(startDate, endDate));
			}
		}
		protected virtual void OnResourceDataSourceCreating(ResourceDataSourceCreatingEventArgs e) {
			if(ResourceDataSourceCreating != null) {
				ResourceDataSourceCreating(this, e);
			}
		}
		protected virtual void OnResourceDataSourceCreated(ResourceDataSourceCreatedEventArgs e) {
			if(ResourceDataSourceCreated != null) {
				ResourceDataSourceCreated(this, e);
			}
		}
		private void OnCreateDeleteHelper(CreateDeleteHelperEventArgs args) {
			if(CreateDeleteHelper != null) {
				CreateDeleteHelper(this, args);
			}
		}
		protected virtual void SetupEditModeDependentOptions() {
			if(AllowEdit) {
				OptionsCustomization.AllowInplaceEditor = UsedAppointmentType.All;
				OptionsCustomization.AllowAppointmentDrag = UsedAppointmentType.All;
				OptionsCustomization.AllowAppointmentDragBetweenResources = UsedAppointmentType.All;
				OptionsCustomization.AllowAppointmentResize = UsedAppointmentType.All;
			}
			else {
				OptionsCustomization.AllowInplaceEditor = UsedAppointmentType.None;
				OptionsCustomization.AllowAppointmentDrag = UsedAppointmentType.None;
				OptionsCustomization.AllowAppointmentDragBetweenResources = UsedAppointmentType.None;
				OptionsCustomization.AllowAppointmentResize = UsedAppointmentType.None;
			}
		}
		protected void SelectedAppointments_CollectionChanged(object sender, CollectionChangedEventArgs<Appointment> e) {
			if(!fetchAppointments && IsSchedulerControlLoaded) {
				OnSelectionChanged();
				OnFocusedObjectChanged();
			}
		}
		protected void SchedulerStorage_FetchAppointments(object sender, FetchAppointmentsEventArgs e) {
			fetchAppointments = true;
			try {
				RaiseFilterDataSource(e.Interval.Start, e.Interval.End);
			}
			finally {
				fetchAppointments = false;
			}
		}		
		protected abstract object CreateResourcesDataSourceCore(Type resourceType);
		protected abstract SchedulerDeleteHelper CreateDefaultDeleteHelper();
		protected object CreateResourcesDataSource() {
			object result = null;
			Type resourceType = GetResourceType();
			if(resourceType != null) {
				ResourceDataSourceCreatingEventArgs args = new ResourceDataSourceCreatingEventArgs(resourceType);
				OnResourceDataSourceCreating(args);
				if(args.Handled) {
					result = args.DataSource;
				}
				else {
					result = CreateResourcesDataSourceCore(resourceType);
				}
				ResourceDataSourceCreatedEventArgs resourceDataSourceCreatedEventArgs = new ResourceDataSourceCreatedEventArgs(result);
				OnResourceDataSourceCreated(resourceDataSourceCreatedEventArgs);
				result = resourceDataSourceCreatedEventArgs.DataSource;
			}
			return result;
		}
		protected virtual void OnAppointmentsInserted(PersistentObjectsEventArgs e) {
			AssignCustomProperties((Appointment)e.Objects[0]);
			OnObjectChanged();
		}
		protected void SchedulerStorage_AppointmentsInserted(object sender, PersistentObjectsEventArgs e) {
			OnAppointmentsInserted(e);
		}
		protected void SchedulerStorage_AppointmentsChanged(object sender, PersistentObjectsEventArgs e) {
			OnObjectChanged();
		}
		protected void SchedulerStorage_AppointmentDeleting(object sender, PersistentObjectCancelEventArgs e) {
			if(!AllowSchedulerNativeDeleting) {
				e.Cancel = true;
			}
			AllowSchedulerNativeDeleting = false;
		}
		protected void SetupSorting(IServiceContainer serviceContainer) {
			if(serviceContainer != null) {
				serviceContainer.RemoveService(typeof(IExternalAppointmentCompareService));
				serviceContainer.AddService(typeof(IExternalAppointmentCompareService), new AppointmentCompareService());
			}
		}
		protected override void OnSelectionChanged() {
			base.OnSelectionChanged();
			AllowSchedulerNativeDeleting = false;
		}
		protected override void OnAllowEditChanged() {
			base.OnAllowEditChanged();
			if(Control != null) {
				SetupEditModeDependentOptions();
			}
		}
		protected override void OnControlsCreated() {
			base.OnControlsCreated();
			SetupEditModeDependentOptions();
		}
		protected bool AreRecurrencesSupported {
			get {
				return typeof(IRecurrentEvent).IsAssignableFrom(Model.ModelClass.TypeInfo.Type);
			}
		}
		protected bool AreRemindersSupported {
			get {
				return typeof(IReminderEvent).IsAssignableFrom(Model.ModelClass.TypeInfo.Type);
			}
		}
		protected bool FetchAppointments { get { return fetchAppointments; } }
		protected virtual void SetupMappings() {
			Appointments.ResourceSharing = true;
			AppointmentMappingInfo appointmentMappingInfo = AppointmentsMappings;
			appointmentMappingInfo.Subject = "Subject";
			appointmentMappingInfo.AllDay = "AllDay";
			appointmentMappingInfo.Start = "StartOn";
			appointmentMappingInfo.End = "EndOn";
			appointmentMappingInfo.Location = "Location";
			appointmentMappingInfo.Label = "Label";
			appointmentMappingInfo.Status = "Status";
			appointmentMappingInfo.Description = "Description";
			appointmentMappingInfo.Type = "Type";
			appointmentMappingInfo.ResourceId = "ResourceId";
			appointmentMappingInfo.AppointmentId = "AppointmentId";
			if(AreRecurrencesSupported) {
				appointmentMappingInfo.RecurrenceInfo = "RecurrenceInfoXml";
			}
			if(AreRemindersSupported) {
				appointmentMappingInfo.ReminderInfo = "ReminderInfoXml";
			}
			ResourceMappingInfo resourceMappingInfo = ResourcesMappings;
			resourceMappingInfo.Id = "Id";
			resourceMappingInfo.Caption = "Caption";
			resourceMappingInfo.Color = "OleColor";
		}
		public override String[] RequiredProperties {
			get {
				List<String> result = new List<String>();
				result.AddRange(base.RequiredProperties);
				AddRequiredProperties("Subject", result);
				AddRequiredProperties("AllDay", result);
				AddRequiredProperties("StartOn", result);
				AddRequiredProperties("EndOn", result);
				AddRequiredProperties("Location", result);
				AddRequiredProperties("Label", result);
				AddRequiredProperties("Status", result);
				AddRequiredProperties("Description", result);
				AddRequiredProperties("Type", result);
				AddRequiredProperties("ResourceId", result);
				AddRequiredProperties("AppointmentId", result);
				AddRequiredProperties("RecurrenceInfoXml", result);
				AddRequiredProperties("ReminderInfoXml", result);
				foreach(IMemberInfo keyMemberInfo in Model.ModelClass.TypeInfo.KeyMembers) {
					result.Add(keyMemberInfo.Name);
				}
				return result.ToArray();
			}
		}
		public Appointment GetAppointment(IEvent schedulerEvent) {
			return GetAppointment(schedulerEvent, null);
		}
		protected virtual bool IsEqual(IEvent schedulerEvent, Appointment exception) {
			return schedulerEvent == SourceObjectHelper.GetSourceObject(exception);
		}
		public Appointment GetAppointment(IEvent schedulerEvent, IObjectSpace objectSpace) {
			Appointment result = null;
			if(schedulerEvent != null) {
				foreach(Appointment appointment in Appointments.Items) {
					IEvent appointmentEvent = SourceObjectHelper.GetSourceObject(appointment);
					if(objectSpace != null) {
						appointmentEvent = objectSpace.GetObject(appointmentEvent);
					}
					if(schedulerEvent == appointmentEvent) {
						return appointment;
					}
					if(appointment.Type == AppointmentType.Pattern) {
						foreach(Appointment exception in appointment.GetExceptions()) {
							if(IsEqual(schedulerEvent, exception)) {
								return exception;
							}
						}
					}
				}
			}
			return result;
		}
		protected abstract void SelectAppointment(Appointment appointment);
		public abstract ISchedulerStorageBase StorageBase { get; }
		public abstract IAppointmentStorageBase Appointments { get; }
		public abstract AppointmentMappingInfo AppointmentsMappings { get; }
		public abstract AppointmentCustomFieldMappingCollection AppointmentsCustomFieldMappings { get; }
		public abstract IResourceStorageBase Resources { get; }
		public abstract ResourceMappingInfo ResourcesMappings { get; }
		public abstract SchedulerOptionsCustomization OptionsCustomization { get; }
		public abstract AppointmentBaseCollection SelectedAppointments { get; }
		public abstract DateTime Start { get; set; }
		public abstract TimeInterval SelectedInterval { get; }
		public abstract DevExpress.XtraScheduler.Resource SelectedResource { get; }
		public SourceObjectHelperBase SourceObjectHelper {
			get {
				EnsureSourceObjectHelper();
				return sourceObjectHelper;
			}
		}
		public EventAssigner EventAssigner {
			get {
				if(eventAssigner == null) {
					eventAssigner = new EventAssigner(SourceObjectHelper, this);
				}
				return eventAssigner;
			}
		}
		public abstract IDateNavigatorControllerOwner DateNavigator {
			get;
		}
		protected virtual void OnNewAction(Appointment appointment) {
			if(NewAction != null) {
				NewAction(this, new NewActionEventArgs(appointment));
			}
		}
		public void AssignCustomProperties(Appointment appointment){
		 IRecurrentEvent schedulerEvent = SourceObjectHelper.GetSourceObject(appointment) as IRecurrentEvent;
			if(schedulerEvent != null && SchedulerUtils.IsExceptionType((AppointmentType)schedulerEvent.Type)) {
				IRecurrentEvent patternEvent = EventAssigner.GetRecurrencePattern(appointment, ObjectSpace);
				if(schedulerEvent.RecurrencePattern != patternEvent) {
					schedulerEvent.RecurrencePattern = patternEvent;
					RaiseExceptionEventCreated(patternEvent, schedulerEvent);
				}
			}
		}
		internal void RaiseExceptionEventCreated(IEvent sourceEvent, IEvent destinationEvent) {
			if(ExceptionEventCreated != null) {
				ExceptionEventCreated(this, new ExceptionEventCreatedEventArgs(sourceEvent, destinationEvent));
			}
		}
		public SchedulerListEditorBase(IModelListView model)
			: base(model) {
			sortedEvents = new ArrayList();
		}
		protected SchedulerDeleteHelper CreateSchedulerDeleteHelper() {
			CreateDeleteHelperEventArgs args = new CreateDeleteHelperEventArgs(CreateDefaultDeleteHelper());
			OnCreateDeleteHelper(args);
			return args.DeleteHelper;
		}
		public Type GetResourceType() {
			IModelClass resourceClass = ((IModelListViewScheduler)Model).ResourceClass;
			return resourceClass != null ? resourceClass.TypeInfo.Type : null;
		}
		public override IList GetSelectedObjects() {
			ArrayList selectedObjects = new ArrayList();
			if(Control != null) {
				foreach(Appointment appointment in SelectedAppointments) {
					Object schedulerEvent;
					if(appointment.IsOccurrence && !appointment.IsException) {
						schedulerEvent = appointment.RecurrencePattern.GetSourceObject(StorageBase);
					}
					else {
						schedulerEvent = appointment.GetSourceObject(StorageBase);
					}
					Object selectedEvent = SourceObjectHelper.GetObjectInCollectionSource(schedulerEvent);
					if(!selectedObjects.Contains(selectedEvent)) {
						selectedObjects.Add(selectedEvent);
					}
				}
			}
			return (object[])selectedObjects.ToArray(typeof(Object));
		}
		public override object FocusedObject {
			get {
				object result = null;
				if(Control != null && SelectedAppointments != null && SelectedAppointments.Count == 1) {
					result = GetSelectedObjects()[0];
				}
				return result;
			}
			set {
				if(Control != null) {
					Appointment currentSelectedAppointment = null;
					if(SelectedAppointments.Count == 1) {
						currentSelectedAppointment = SelectedAppointments[0];
					}
					if(!fetchAppointments) {
						Appointment appointment = GetAppointment(value as IEvent);
						if(appointment == null) {
							SelectedAppointments.Clear();
						}
						if(appointment != null && (appointment != currentSelectedAppointment) && (appointment.RecurrenceInfo == null)) {
							SelectAppointment(appointment);
						}
					}
				}
			}
		}
		public SchedulerDeleteHelper SchedulerDeleteHelper {
			get {
				if(deleteHelper == null) {
					deleteHelper = CreateSchedulerDeleteHelper();
				}
				return deleteHelper;
			}
		}
		public override SelectionType SelectionType {
			get { return SelectionType.Full; }
		}
		public virtual bool IsSchedulerControlLoaded {
			get {
				return true;
			}
		}
		public bool IsOccurrenceFocused {
			get {
				if (FocusedObject is XafDataViewRecord){
					return FocusedObject != null && (int)((XafDataViewRecord)FocusedObject)["Type"] != (int)AppointmentType.Normal;
				}
				return FocusedObject != null && ((IEvent)FocusedObject).Type != (int)AppointmentType.Normal;
			}
		}
		public bool IsChangedOccurrenceFocused {
			get {
				if(FocusedObject is XafDataViewRecord) {
					return FocusedObject != null && (int)((XafDataViewRecord)FocusedObject)["Type"] == (int)AppointmentType.ChangedOccurrence;
				}
				return FocusedObject != null && ((IEvent)FocusedObject).Type == (int)AppointmentType.ChangedOccurrence;
			}
		}
		public bool IsPatternFocused {
			get {
				if(FocusedObject is XafDataViewRecord) {
					return FocusedObject != null && (int)((XafDataViewRecord)FocusedObject)["Type"] == (int)AppointmentType.Pattern;
				}
				return FocusedObject != null && ((IEvent)FocusedObject).Type == (int)AppointmentType.Pattern;
			}
		}
		public event EventHandler<NewActionEventArgs> NewAction;
		public event EventHandler<FilterDataSourceEventArgs> FilterDataSource;
		public event EventHandler<ResourceDataSourceCreatingEventArgs> ResourceDataSourceCreating;
		public event EventHandler<ResourceDataSourceCreatedEventArgs> ResourceDataSourceCreated;
		public event EventHandler<ExceptionEventCreatedEventArgs> ExceptionEventCreated;
		public event EventHandler<CreateDeleteHelperEventArgs> CreateDeleteHelper;
		#region IControlOrderProvider Members
		public int GetIndexByObject(Object obj) {
			IList objects = GetOrderedObjects();
			return objects.IndexOf(obj);
		}
		public Object GetObjectByIndex(int index) {
			IList objects = GetOrderedObjects();
			if((index >= 0) && (index < objects.Count)) {
				return objects[index];
			}
			return null;
		}
		public IList GetOrderedObjects() {
			sortedEvents.Clear();
			if(DataSource != null && (collectionSource == null || !(collectionSource is CollectionSource) || !((CollectionSource)collectionSource).IsServerMode)) {
				foreach(Object obj in List) {
					if(obj != null  && (obj is IEvent) && ((IEvent)obj).Type == (int)AppointmentType.Normal) {
						sortedEvents.Add(obj);
					}
				}
			}
			sortedEvents.Sort(new EventStartOnComparer());
			return sortedEvents;
		}
		#endregion
		#region IComplexListEditor Members
		private CollectionSourceBase collectionSource;
		protected internal CollectionSourceBase CollectionSource {
			get { return collectionSource; }
		}
		private IObjectSpace objectSpace;
		protected internal IObjectSpace ObjectSpace {
			get { return objectSpace; }
		}
		public virtual void Setup(CollectionSourceBase collectionSource, XafApplication application) {
			this.collectionSource = collectionSource;
			objectSpace = collectionSource.ObjectSpace;
		}
		protected abstract SourceObjectHelperBase CreateSourceObjectHelper();
		#endregion
		#region ISupportUpdate Members
		public abstract void BeginUpdate();
		public abstract void EndUpdate();
		#endregion
		protected internal bool AllowSchedulerNativeDeleting {
			get { return allowSchedulerNativeDeleting; }
			set { allowSchedulerNativeDeleting = value; }
		}
	}
	#region Obsolete since 15.1
	[Obsolete("Use SchedulerListEditor.ControlSettingsSaving/ASPxSchedulerListEditor.ControlSettingsSaving instead.")]
	public abstract class SchedulerControlModelSynchronizer<T> : ModelSynchronizer<T, IModelListView> {
		public const string SelectedIntervalStartAttribute = "SelectedIntervalStart";
		private const string SelectedIntervalEndAttribute = "SelectedIntervalEnd";
		public SchedulerControlModelSynchronizer(T control, IModelListView model)
			: base(control, model) {
		}
		protected override void ApplyModelCore() {
			IModelListViewScheduler modelListView = (IModelListViewScheduler)Model;
			ViewType = modelListView.SchedulerViewType;
			ResourcesPerPage = modelListView.VisibleResourcesCount;
			TimeScale = modelListView.TimeScale;
			VisibleInterval = modelListView.SelectedIntervalStart < modelListView.SelectedIntervalEnd ? new TimeInterval(modelListView.SelectedIntervalStart, modelListView.SelectedIntervalEnd) : null;
		}
		protected abstract SchedulerViewType ViewType { get; set; }
		protected abstract int ResourcesPerPage { get; set; }
		protected abstract TimeInterval VisibleInterval { get; set; }
		protected abstract TimeSpan TimeScale { get; set; }
		public override void SynchronizeModel() {
			IModelListViewScheduler modelListView = (IModelListViewScheduler)Model;
			modelListView.SchedulerViewType = ViewType;
			modelListView.VisibleResourcesCount = ResourcesPerPage;
			modelListView.TimeScale = TimeScale;
			if(VisibleInterval != null) {
				modelListView.SelectedIntervalStart = VisibleInterval.Start;
				modelListView.SelectedIntervalEnd = VisibleInterval.End;
			}
			else {
				modelListView.SelectedIntervalStart = default(DateTime);
				modelListView.SelectedIntervalEnd = default(DateTime);
			}
		}
	}
	#endregion
}
