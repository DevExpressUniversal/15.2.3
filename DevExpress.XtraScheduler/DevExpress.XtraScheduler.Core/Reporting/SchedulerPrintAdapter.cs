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
using System.ComponentModel;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using System.ComponentModel.Design;
using DevExpress.Services.Internal;
using DevExpress.Utils.Controls;
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.XtraScheduler.Internal.Implementations;
#if !SL
using System.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraScheduler.Internal;
#else
	using DevExpress.Xpf.ComponentModel;
	using System.Windows.Media;
#endif
namespace DevExpress.XtraScheduler.Reporting {
#if !SL
	[DXToolboxItem(false),
	ToolboxItemFilter(DevExpress.XtraReports.Design.AttributeSR.SchedulerToolboxItem, ToolboxItemFilterType.Require),
	]
#endif
	public abstract class SchedulerPrintAdapter : Component, ISupportInitialize, ISchedulerAppointmentProvider, ISchedulerTimeIntervalProvider, ISchedulerResourceProvider, IServiceProvider , IStatusBrushAdapter {
		#region static and const
		internal const FirstDayOfWeek DefaultFirstDayOfWeek = FirstDayOfWeek.System;
		internal const WeekDays DefaultWeekDays = WeekDays.EveryDay;
		internal static readonly NotificationTimeInterval DefaultTimeInterval = NotificationTimeInterval.Empty;
		internal static readonly WorkTimeInterval DefaultWorkTime = WorkTimeInterval.WorkTime;
		internal static DayOfWeek GetDefaultFirstDayOfWeek() {
			return DateTimeHelper.ConvertFirstDayOfWeek(DefaultFirstDayOfWeek);
		}
		internal static string GetDefaultTimeZoneId() {
			return TimeZoneEngine.Local.Id;
		}
		#endregion
		#region Fields
		ISchedulerPrintAdapterPropertiesBase properties;
		ISchedulerTimeIntervalProvider timeIntervalProvider;
		ISchedulerAppointmentProvider appointmentProvider;
		ISchedulerResourceProvider resourceProvider;
		#endregion
		protected SchedulerPrintAdapter(ISchedulerPrintAdapterPropertiesBase properties) {
			Guard.ArgumentNotNull(properties, "properties");
			CreateInnerProviders();
			this.properties = properties;
			SubscribePropertiesEvents();
		}
		protected SchedulerPrintAdapter() {
			CreateInnerProviders();
			this.properties = CreateProperties();
			SubscribePropertiesEvents();
		}
		protected ISchedulerPrintAdapterPropertiesBase Properties { get { return properties; } }
		IStatusBrushAdapter BrushAdapterInstance { get; set; }
		protected abstract ISchedulerPrintAdapterPropertiesBase CreateProperties();
		protected internal virtual void SubscribePropertiesEvents() {
			Properties.PropertyChanging += OnPropertiesChanging;
			Properties.PropertyChanged += OnPropertiesChanged;
		}
		protected internal virtual void UnsubscribePropertiesEvents() {
			Properties.PropertyChanging -= OnPropertiesChanging;
			Properties.PropertyChanged -= OnPropertiesChanged;
		}
		protected internal virtual void OnPropertiesChanging(object sender, PropertyChangingEventArgs e) {
		}
		protected internal virtual void OnPropertiesChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "TimeInterval") {
				RaiseTimeIntervalChanged();
			}
			if (e.PropertyName == "WorkTime") {
				RaiseWorkTimeChanged();
			}
			if (e.PropertyName == "FirstDayOfWeek") {
				RaiseFirstDayOfWeekChanged();
			}
			if (e.PropertyName == "ClientTimeZoneId") {
				RaiseClientTimeZoneIdChanged();
			}
			if (e.PropertyName == "EnableSmartSync") {
				OnEnableSmartSyncChanged();
			}
			if (e.PropertyName == "SmartSyncOptions") {
				OnSmartSyncOptionsChanged();
			}
			if (e.PropertyName == "ResourceColorSchemas") {
				OnResourceColorsChanged();
			}
		}
		#region Properties
		[Category(SRCategoryNames.Scheduler),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerPrintAdapterWorkTime")
#else
	Description("")
#endif
		]
		public TimeOfDayInterval WorkTime {
			get { return  Properties.WorkTime; }
			set {
				Properties.WorkTime = value;
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerPrintAdapterFirstDayOfWeek"),
#endif
		Category(SRCategoryNames.Scheduler), DefaultValue(FirstDayOfWeek.System)]
		public FirstDayOfWeek FirstDayOfWeek {
			get { return  Properties.FirstDayOfWeek; }
			set {
				Properties.FirstDayOfWeek = value;
			}
		}
		protected internal TimeZoneHelper TimeZoneHelper { get { return Properties.TimeZoneHelper; } }
		[Category(SRCategoryNames.Scheduler), 
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerPrintAdapterClientTimeZoneId")
#else
	Description("")
#endif
]
		public string ClientTimeZoneId {
			get { return Properties.ClientTimeZoneId; }
			set {
				Properties.ClientTimeZoneId = value;
			}
		}
		protected internal virtual bool ShouldSerializeClientTimeZoneId() {
			return ClientTimeZoneId != GetDefaultTimeZoneId();
		}
		protected internal virtual void ResetClientTimeZoneId() {
			ClientTimeZoneId = GetDefaultTimeZoneId();
		}
		internal bool ShouldSerializeTimeInterval() {
			return !DefaultTimeInterval.Equals(TimeInterval);
		}
		internal void ResetTimeInterval() {
			TimeInterval = NotificationTimeInterval.Empty;
		}
		internal bool ShouldSerializeWorkTime() {
			return !WorkTime.IsEqual(DefaultWorkTime);
		}
		internal void ResetWorkTime() {
			WorkTime = DefaultWorkTime;
		}
		protected internal virtual bool IsDesignMode { get { return DesignMode; } }
		#region TimeInterval
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerPrintAdapterTimeInterval"),
#endif
		Category(SRCategoryNames.Scheduler), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TimeInterval TimeInterval {
			get { return  Properties.TimeInterval; }
			set {
				Properties.TimeInterval = value;
			}
		}
		#region ResourceColorSchemas
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerPrintAdapterResourceColorSchemas"),
#endif
		Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#if !SL
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)]
#endif
		public ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> ResourceColorSchemas { get { return Properties.ResourceColorSchemas; } }
		internal bool ShouldSerializeResourceColorSchemas() {
			return !ResourceColorSchemas.HasDefaultContent();
		}
		internal void ResetResourceColorSchemas() {
			ResourceColorSchemas.LoadDefaults();
		}
		#endregion
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerPrintAdapterSmartSyncOptions"),
#endif
Category(SRCategoryNames.Behavior), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ISmartSyncOptions SmartSyncOptions {
			get { return Properties.SmartSyncOptions; }
		}
		#endregion
		protected virtual void OnEnableSmartSyncChanged() {
		}
		protected virtual void OnSmartSyncOptionsChanged() {
		}
		protected internal virtual void OnResourceColorsChanged() {
			RaiseResourceColorsChanged();
		}
		protected internal ISchedulerTimeIntervalProvider TimeIntervalProvider { get { return timeIntervalProvider; } }
		protected internal ISchedulerAppointmentProvider AppointmentProvider { get { return appointmentProvider; } }
		protected internal ISchedulerResourceProvider ResourceProvider { get { return resourceProvider; } }
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerPrintAdapterEnableSmartSync"),
#endif
Category(SRCategoryNames.Behavior), DefaultValue(false)]
		public bool EnableSmartSync {
			get { return  Properties.EnableSmartSync; }
			set {
				Properties.EnableSmartSync = value;
			}
		}
		#endregion
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (timeIntervalProvider != null) {
						timeIntervalProvider = null;
					}
					if (resourceProvider != null) {
						resourceProvider = null;
					}
					if (appointmentProvider != null) {
						appointmentProvider = null;
					}
					if (properties != null) {
						UnsubscribePropertiesEvents();
						properties.Dispose();
						properties = null;
					}
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		#region Events
		static readonly object SchedulerSourceChangedEvent = new object();
		static readonly object TimeIntervalChangedEvent = new object();
		static readonly object WorkTimeChangedEvent = new object();
		static readonly object FirstDayOfWeekChangedEvent = new object();
		static readonly object ClientTimeZoneIdChangedEvent = new object();
		static readonly object ResourceColorsChangedEvent = new object();
		static readonly object ValidateTimeIntervalsEvent = new object();
		static readonly object ValidateWorkTimeEvent = new object();
		static readonly object ValidateAppointmentsEvent = new object();
		static readonly object ValidateResourcesEvent = new object();
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerPrintAdapterValidateTimeIntervals")]
#endif
		public event TimeIntervalsValidationEventHandler ValidateTimeIntervals {
			add { Events.AddHandler(ValidateTimeIntervalsEvent, value); }
			remove { Events.RemoveHandler(ValidateTimeIntervalsEvent, value); }
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerPrintAdapterValidateWorkTime")]
#endif
		public event WorkTimeValidationEventHandler ValidateWorkTime {
			add { Events.AddHandler(ValidateWorkTimeEvent, value); }
			remove { Events.RemoveHandler(ValidateWorkTimeEvent, value); }
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerPrintAdapterValidateAppointments")]
#endif
		public event AppointmentsValidationEventHandler ValidateAppointments {
			add { Events.AddHandler(ValidateAppointmentsEvent, value); }
			remove { Events.RemoveHandler(ValidateAppointmentsEvent, value); }
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerPrintAdapterValidateResources")]
#endif
		public event ResourcesValidationEventHandler ValidateResources {
			add { Events.AddHandler(ValidateResourcesEvent, value); }
			remove { Events.RemoveHandler(ValidateResourcesEvent, value); }
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerPrintAdapterSchedulerSourceChanged")]
#endif
		public event EventHandler SchedulerSourceChanged {
			add { Events.AddHandler(SchedulerSourceChangedEvent, value); }
			remove { Events.RemoveHandler(SchedulerSourceChangedEvent, value); }
		}
		protected void RaiseSchedulerSourceChanged() {
			EventHandler handler = (EventHandler)Events[SchedulerSourceChangedEvent];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerPrintAdapterResourceColorsChanged")]
#endif
		public event EventHandler ResourceColorsChanged {
			add { Events.AddHandler(ResourceColorsChangedEvent, value); }
			remove { Events.RemoveHandler(ResourceColorsChangedEvent, value); }
		}
		protected void RaiseResourceColorsChanged() {
			EventHandler handler = (EventHandler)Events[ResourceColorsChangedEvent];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerPrintAdapterTimeIntervalChanged")]
#endif
		public event EventHandler TimeIntervalChanged {
			add { Events.AddHandler(TimeIntervalChangedEvent, value); }
			remove { Events.RemoveHandler(TimeIntervalChangedEvent, value); }
		}
		protected virtual void RaiseTimeIntervalChanged() {
			EventHandler handler = (EventHandler)Events[TimeIntervalChangedEvent];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerPrintAdapterWorkTimeChanged")]
#endif
		public event EventHandler WorkTimeChanged {
			add { Events.AddHandler(WorkTimeChangedEvent, value); }
			remove { Events.RemoveHandler(WorkTimeChangedEvent, value); }
		}
		protected virtual void RaiseWorkTimeChanged() {
			EventHandler handler = (EventHandler)Events[WorkTimeChangedEvent];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerPrintAdapterFirstDayOfWeekChanged")]
#endif
		public event EventHandler FirstDayOfWeekChanged {
			add { Events.AddHandler(FirstDayOfWeekChangedEvent, value); }
			remove { Events.RemoveHandler(FirstDayOfWeekChangedEvent, value); }
		}
		protected virtual void RaiseFirstDayOfWeekChanged() {
			EventHandler handler = (EventHandler)Events[FirstDayOfWeekChangedEvent];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerPrintAdapterClientTimeZoneIdChanged")]
#endif
		public event EventHandler ClientTimeZoneIdChanged {
			add { Events.AddHandler(ClientTimeZoneIdChangedEvent, value); }
			remove { Events.RemoveHandler(ClientTimeZoneIdChangedEvent, value); }
		}
		protected virtual void RaiseClientTimeZoneIdChanged() {
			EventHandler handler = (EventHandler)Events[ClientTimeZoneIdChangedEvent];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseValidateResources(ResourcesValidationEventArgs args) {
			ResourcesValidationEventHandler handler = (ResourcesValidationEventHandler)Events[ValidateResourcesEvent];
			if (handler != null) {
				handler(this, args);
			}
		}
		protected internal virtual void RaiseValidateTimeIntervals(TimeIntervalsValidationEventArgs args) {
			TimeIntervalsValidationEventHandler handler = (TimeIntervalsValidationEventHandler)Events[ValidateTimeIntervalsEvent];
			if (handler != null) {
				handler(this, args);
			}
		}
		protected internal virtual void RaiseValidateWorkTime(WorkTimeValidationEventArgs args) {
			WorkTimeValidationEventHandler handler = (WorkTimeValidationEventHandler)Events[ValidateWorkTimeEvent];
			if (handler != null) {
				handler(this, args);
			}
		}
		protected internal virtual void RaiseValidateAppointments(AppointmentsValidationEventArgs args) {
			AppointmentsValidationEventHandler handler = (AppointmentsValidationEventHandler)Events[ValidateAppointmentsEvent];
			if (handler != null) {
				handler(this, args);
			}
		}
		protected virtual TimeIntervalsValidationEventArgs CreateTimeIntervalsValidationEventArgs(TimeIntervalCollection intervals) {
			TimeIntervalCollection sourceIntervals = new TimeIntervalCollection();
			sourceIntervals.AddRange(intervals);
			return new TimeIntervalsValidationEventArgs(sourceIntervals);
		}
		protected virtual WorkTimeValidationEventArgs CreateWorkTimeValidationEventArgs(TimeOfDayIntervalCollection workTimes, TimeInterval interval, Resource resource) {
			return new WorkTimeValidationEventArgs(workTimes, interval, resource);
		}
		protected virtual AppointmentsValidationEventArgs CreateAppointmentsValidationEventArgs(AppointmentBaseCollection appointments, TimeInterval interval, ResourceBaseCollection resources) {
			return new AppointmentsValidationEventArgs(appointments, interval, resources);
		}
		protected virtual ResourcesValidationEventArgs CreateResourcesValidationEventArgs(ResourceBaseCollection resources) {
			return new ResourcesValidationEventArgs(resources);
		}
		#endregion
		#region InnerProviders creation
		protected virtual ISchedulerTimeIntervalProvider CreateInnerTimeIntervalProvider() {
			return new InnerTimeIntervalProvider(this);
		}
		protected virtual ISchedulerResourceProvider CreateInnerResourceProvider() {
			return new InnerResourceProvider();
		}
		protected virtual ISchedulerAppointmentProvider CreateInnerAppointmentProvider() {
			return new InnerAppointmentProvider();
		}
		#endregion
		#region IServiceProvider Members
		public new object GetService(Type serviceType) {
			return GetServiceCore(serviceType);
		}
		protected virtual object GetServiceCore(Type serviceType) {
			return null;
		}
		internal abstract HeaderCaptionFormatProviderBase GetHeaderCaptionFormatProvider();
		#endregion
		#region ISchedulerAppointmentProvider Members
		public AppointmentBaseCollection GetAppointments(TimeInterval timeInterval, ResourceBaseCollection resources) {
			AppointmentBaseCollection appointments = GetAppointmentsCore(timeInterval, resources);
			AppointmentsValidationEventArgs args = CreateAppointmentsValidationEventArgs(appointments, timeInterval, resources);
			OnValidateAppointments(args);
			return args.Appointments;
		}
		public IAppointmentStatus GetStatus(object statusId) {
			return GetStatusCore(statusId);
		}
		public Color GetLabelColor(object labelId) {
			return GetLabelColorCore(labelId);
		}
		#endregion
		#region ISchedulerResourceProvider Members
		public ResourceBaseCollection GetResources() {
			ResourceBaseCollection resources = GetResourcesCore();
			ResourcesValidationEventArgs args = CreateResourcesValidationEventArgs(resources);
			OnValidateResources(args);
			return args.Resources;
		}
		public ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> GetResourceColorSchemas() {
			if (ShouldUseSpecificColorSchemas())
				return ResourceColorSchemas;
			return GetResourceColorSchemasCore();
		}
		[Browsable(false)]
		public virtual int ResourceCount {
			get { return GetResources().Count; }
		}
		SchedulerGroupType ISchedulerResourceProvider.GetGroupType() {
			return EnableSmartSync ? SmartSyncOptions.GroupType : SchedulerGroupType.None;
		}
		#endregion
		#region ISchedulerTimeIntervalProvider Members
		public TimeIntervalCollection GetTimeIntervals() {
			TimeIntervalCollection intervals;
			if (ShouldUseSpecificTimeInterval())
				intervals = CalculateTimeIntervals();
			else
				intervals = GetTimeIntervalsCore();
			TimeIntervalsValidationEventArgs args = CreateTimeIntervalsValidationEventArgs(intervals);
			OnValidateTimeIntervals(args);
			return args.Intervals;
		}
		public TimeOfDayIntervalCollection GetWorkTime(TimeInterval interval, Resource resource) {
			Resource res = resource != null ? resource : ResourceBase.Empty;
			TimeOfDayIntervalCollection workTimeIntervals;
			if (ShouldUseSpecificWorkTime()) {
				workTimeIntervals = new TimeOfDayIntervalCollection();
				workTimeIntervals.Add((TimeOfDayInterval)WorkTime.Clone());
			} else
				workTimeIntervals = GetWorkTimeCore(interval, res);
			WorkTimeValidationEventArgs args = CreateWorkTimeValidationEventArgs(workTimeIntervals, interval, res);
			OnValidateWorkTime(args);
			return args.WorkTimes;
		}
		public virtual WorkDaysCollection GetWorkDays() {
			return TimeIntervalProvider.GetWorkDays();
		}
		internal static WorkDaysCollection GetDefaultWorkDays() {
			WorkDaysCollection result = new WorkDaysCollection();
			result.Add(WeekDays.WorkDays);
			return result;
		}
		public virtual DayOfWeek GetFirstDayOfWeek() {
			if (FirstDayOfWeek != DefaultFirstDayOfWeek)
				return ConvertFirstDayOfWeek();
			return TimeIntervalProvider.GetFirstDayOfWeek();
		}
		public virtual string GetClientTimeZoneId() {
			if (ClientTimeZoneId != GetDefaultTimeZoneId())
				return ClientTimeZoneId;
			return GetClientTimeZoneIdCore();
		}
		TimeInterval ISchedulerTimeIntervalProvider.GetAdapterInterval() {
			TimeIntervalCollection intervals = GetTimeIntervals();
			return intervals.Interval;
		}
		protected internal virtual string GetClientTimeZoneIdCore() {
			return TimeIntervalProvider.GetClientTimeZoneId();
		}
		#endregion
		public abstract void SetSourceObject(object sourceObject);
		protected internal virtual void OnValidateTimeIntervals(TimeIntervalsValidationEventArgs args) {
			RaiseValidateTimeIntervals(args);
		}
		protected internal virtual void OnValidateWorkTime(WorkTimeValidationEventArgs args) {
			RaiseValidateWorkTime(args);
		}
		protected internal virtual void OnValidateAppointments(AppointmentsValidationEventArgs args) {
			RaiseValidateAppointments(args);
		}
		protected internal virtual void OnValidateResources(ResourcesValidationEventArgs args) {
			RaiseValidateResources(args);
		}
		protected virtual ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> GetResourceColorSchemasCore() {
			return ResourceProvider.GetResourceColorSchemas();
		}
		protected virtual bool ShouldUseSpecificColorSchemas() {
			return !ResourceColorSchemas.HasDefaultContent();
		}
		protected virtual ResourceBaseCollection GetResourcesCore() {
			return ResourceProvider.GetResources();
		}
		protected virtual AppointmentBaseCollection GetAppointmentsCore(TimeInterval timeInterval, ResourceBaseCollection resources) {
			return AppointmentProvider.GetAppointments(timeInterval, resources);
		}
		protected virtual IAppointmentStatus GetStatusCore(object statusId) {
			return AppointmentProvider.GetStatus(statusId);
		}
		protected virtual Color GetLabelColorCore(object labelId) {
			return AppointmentProvider.GetLabelColor(labelId);
		}
		protected virtual bool ShouldUseSpecificTimeInterval() {
			return !DefaultTimeInterval.Equals(TimeInterval);
		}
		protected virtual bool ShouldUseSpecificWorkTime() {
			return !DefaultWorkTime.IsEqual(WorkTime);
		}
		protected internal virtual TimeIntervalCollection GetTimeIntervalsCore() {
			return TimeIntervalProvider.GetTimeIntervals();
		}
		protected virtual TimeOfDayIntervalCollection GetWorkTimeCore(TimeInterval interval, Resource resource) {
			return TimeIntervalProvider.GetWorkTime(interval, resource);
		}
		protected internal virtual TimeIntervalCollection CalculateTimeIntervals() {
			TimeIntervalCollection result = new TimeIntervalCollection();
			result.Add(TimeInterval);
			return result;
		}
		protected internal DayOfWeek ConvertFirstDayOfWeek() {
			return DateTimeHelper.ConvertFirstDayOfWeek(FirstDayOfWeek);
		}
		protected internal virtual void CreateInnerProviders() {
			BrushAdapterInstance = CreateBrushAdapterImplementation();
			this.timeIntervalProvider = CreateInnerTimeIntervalProvider();
			this.appointmentProvider = CreateInnerAppointmentProvider();
			this.resourceProvider = CreateInnerResourceProvider();
		}
		protected virtual void UpdateTimeZoneEngine(string clientTimeZoneId) {
			Properties.UpdateTimeZoneEngine(clientTimeZoneId);
		}
		#region ISupportInitialize Members
		void ISupportInitialize.BeginInit() {
			BeginInit();
		}
		void ISupportInitialize.EndInit() {
			EndInit();
		}
		#endregion
		protected internal virtual void BeginInit() {
			Properties.BeginInit();
		}
		protected internal virtual void EndInit() {
			Properties.EndInit();
		}		
		protected virtual IStatusBrushAdapter CreateBrushAdapterImplementation() {
			return null;
		}
		Image IStatusBrushAdapter.CreateImage(Rectangle bounds, IAppointmentStatus status) {
			if (BrushAdapterInstance == null)
				return null;
			return BrushAdapterInstance.CreateImage(bounds, status);
		}
		void IStatusBrushAdapter.ClearCache() {
			if (BrushAdapterInstance == null)
				return;
			BrushAdapterInstance.ClearCache();
		}
		internal abstract void RunCreatingDocument(Action<bool> createDocumentMethod, bool methodParam);
	}
	public abstract class SchedulerStorageBasePrintAdapter : SchedulerPrintAdapter {
		ISchedulerStorageBase storage;
		ServiceManager serviceManager;
		protected SchedulerStorageBasePrintAdapter() {
			Initialize();
		}
		protected SchedulerStorageBasePrintAdapter(ISchedulerStorageBase storage, ISchedulerPrintAdapterPropertiesBase properties)
			: base(properties) {
			this.storage = storage;
			Initialize();
		}
		protected SchedulerStorageBasePrintAdapter(ISchedulerStorageBase storage) {
			this.storage = storage;
			Initialize();
		}
		#region Properties
		protected internal ISchedulerStorageBase StorageBase {
			get { return storage; }
			set {
				if (storage == value)
					return;
				storage = value;
				OnSchedulerStorageChanged();
			}
		}
		protected ServiceManager ServiceManager { get { return serviceManager; } }
		IInternalSchedulerStorageBase InternalStorage { get { return (IInternalSchedulerStorageBase)StorageBase; } }
		#endregion
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (serviceManager != null) {
					serviceManager.Dispose();
					serviceManager = null;
				}
			}
			base.Dispose(disposing);
		}
		#region IServiceContainer Members
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			serviceManager.AddService(serviceType, callback, promote);
		}
		public void AddService(Type serviceType, ServiceCreatorCallback callback) {
			serviceManager.AddService(serviceType, callback);
		}
		public void AddService(Type serviceType, object serviceInstance, bool promote) {
			serviceManager.AddService(serviceType, serviceInstance, promote);
		}
		public void AddService(Type serviceType, object serviceInstance) {
			serviceManager.AddService(serviceType, serviceInstance);
		}
		public void RemoveService(Type serviceType, bool promote) {
			serviceManager.RemoveService(serviceType, promote);
		}
		public void RemoveService(Type serviceType) {
			serviceManager.RemoveService(serviceType);
		}
		#endregion
		protected override object GetServiceCore(Type serviceType) {
			return serviceManager.GetService(serviceType);
		}
		protected internal virtual void Initialize() {
			this.serviceManager = CreateServiceManager();
		}
		protected internal virtual ServiceManager CreateServiceManager() {
			return new ServiceManager();
		}
		protected void OnSchedulerStorageChanged() {
			RaiseSchedulerSourceChanged();
		}
		protected override ResourceBaseCollection GetResourcesCore() {
			if (StorageBase != null)
				return InternalStorage.GetFilteredResources(true);
			return base.GetResourcesCore();
		}
		protected override ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> GetResourceColorSchemasCore() {
			return ResourceColorSchemas;
		}
		protected override IAppointmentStatus GetStatusCore(object statusId) {
			if (StorageBase != null)
				return StorageBase.Appointments.Statuses.GetById(statusId);
			return base.GetStatusCore(statusId);
		}
		protected override Color GetLabelColorCore(object labelId) { 
			if (StorageBase == null)
				return base.GetLabelColorCore(labelId);
			IAppointmentLabel label = StorageBase.Appointments.Labels.GetById(labelId);
			return Color.FromArgb(label.ColorValue);
		}
		protected override AppointmentBaseCollection GetAppointmentsCore(TimeInterval timeInterval, ResourceBaseCollection resources) {
			if (StorageBase != null)
				return GetFilteredAppointments(timeInterval, resources);
			return base.GetAppointmentsCore(timeInterval, resources);
		}
		protected internal virtual AppointmentBaseCollection GetFilteredAppointments(TimeInterval interval, ResourceBaseCollection resources) {
			if (StorageBase != null) {
				AppointmentBaseCollection appointments = StorageBase.GetAppointments(interval);
				return FilterAppointmentsByResource(appointments, resources);
			}
			return new AppointmentBaseCollection();
		}
		protected internal virtual AppointmentBaseCollection FilterAppointmentsByResource(AppointmentBaseCollection appointments, ResourceBaseCollection resources) {
			if (resources == null)
				Exceptions.ThrowArgumentNullException("resources");
			ResourcesAppointmentsFilter resourceFilter = new ResourcesAppointmentsFilter(resources);
			resourceFilter.Process(appointments);
			return (AppointmentBaseCollection)resourceFilter.DestinationCollection;
		}
	}
}
