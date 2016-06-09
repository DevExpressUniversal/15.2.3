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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Xpf.Scheduler.Internal;
using DevExpress.Xpf.Scheduler.Reporting.Native;
using DevExpress.Xpf.Scheduler.Services;
using DevExpress.Xpf.Scheduler.Services.Internal;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Reporting;
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.Xpf.Scheduler.Reporting {
	[DXToolboxBrowsable(false)]
	public abstract class DXSchedulerPrintAdapter : DXDesignTimeControl, ISchedulerPrintAdapterPropertiesBase {
		SchedulerPrintAdapter schedulerAdapter;
		TimeZoneHelper timeZoneHelper;
		protected DXSchedulerPrintAdapter() {
			ResourceColorSchemas = new SchedulerColorSchemaCollection();
#if SILVERLIGHT
			SmartSyncOptions = new SchedulerSmartSyncOptions();
#endif
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SchedulerPrintAdapter SchedulerAdapter {
			get {
				if (schedulerAdapter == null) {
					schedulerAdapter = CreateInnerSchedulerPrintAdapter(this);
					SubscribeSchedulerAdapterEvents(schedulerAdapter);
				}
				return schedulerAdapter;
			}
		}
		protected abstract SchedulerPrintAdapter CreateInnerSchedulerPrintAdapter(ISchedulerPrintAdapterPropertiesBase properties);
		TimeZoneHelper ISchedulerPrintAdapterPropertiesBase.TimeZoneHelper {
			get {
				if (timeZoneHelper == null)
					timeZoneHelper = new TimeZoneHelper(new TimeZoneEngine());
				return timeZoneHelper;
			}
		}
		void ISchedulerPrintAdapterPropertiesBase.UpdateTimeZoneEngine(string tzId) {
			((ISchedulerPrintAdapterPropertiesBase)this).TimeZoneHelper.ClientTimeZone = TimeZoneInfo.FindSystemTimeZoneById(tzId);
		}
		#region Events
		#region ValidateTimeIntervals
		[Category(SRCategoryNames.Scheduler)]
		public event TimeIntervalsValidationEventHandler ValidateTimeIntervals;
		protected internal virtual void RaiseValidateTimeIntervals(object sender, TimeIntervalsValidationEventArgs e) {
			if (ValidateTimeIntervals == null)
				return;
			ValidateTimeIntervals(sender, e);
		}
		#endregion
		#region ValidateResources
		[Category(SRCategoryNames.Scheduler)]
		public event ResourcesValidationEventHandler ValidateResources;
		protected virtual void RaiseValidateResources(object sender, ResourcesValidationEventArgs e) {
			if (ValidateResources == null)
				return;
			ValidateResources(sender, e);
		}
		#endregion
		#region ValidateWorkTime
		[Category(SRCategoryNames.Scheduler)]
		public event WorkTimeValidationEventHandler ValidateWorkTime;
		protected virtual void RaiseValidateWorkTime(object sender, WorkTimeValidationEventArgs e) {
			if (ValidateWorkTime == null)
				return;
			ValidateWorkTime(sender, e);
		}
		#endregion
		#region ValidateAppointments
		[Category(SRCategoryNames.Scheduler)]
		public event AppointmentsValidationEventHandler ValidateAppointments;
		protected virtual void RaiseValidateAppointments(object sender, AppointmentsValidationEventArgs e) {
			if (ValidateAppointments == null)
				return;
			ValidateAppointments(sender, e);
		}
		#endregion
		#region SchedulerSourceChanged
		[Category(SRCategoryNames.Scheduler)]
		public event EventHandler SchedulerSourceChanged;
		protected virtual void RaiseSchedulerSourceChanged(object sender, EventArgs e) {
			if (SchedulerSourceChanged == null)
				return;
			SchedulerSourceChanged(sender, e);
		}
		#endregion
		#endregion
		#region Properties
		#region WorkTime
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public TimeOfDayInterval WorkTime {
			get { return (TimeOfDayInterval)GetValue(WorkTimeProperty); }
			set { SetValue(WorkTimeProperty, value); }
		}
		public static readonly DependencyProperty WorkTimeProperty = CreateWorkTimeProperty();
		static DependencyProperty CreateWorkTimeProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DXSchedulerPrintAdapter, TimeOfDayInterval>("WorkTime", WorkTimeInterval.WorkTime, (d, e) => d.OnWorkTimeChanged(e.OldValue, e.NewValue));
		}
		void OnWorkTimeChanged(TimeOfDayInterval oldValue, TimeOfDayInterval newValue) {
			RaisePropertyChanged("WorkTime", oldValue, newValue);
		}
		#endregion
		#region ClientTimeZoneId
		public string ClientTimeZoneId {
			get { return (string)GetValue(ClientTimeZoneIdProperty); }
			set { SetValue(ClientTimeZoneIdProperty, value); }
		}
		public static readonly DependencyProperty ClientTimeZoneIdProperty = CreateClientTimeZoneIdProperty();
		static DependencyProperty CreateClientTimeZoneIdProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DXSchedulerPrintAdapter, string>("ClientTimeZoneId", null, (d, e) => d.OnClientTimeZoneIdChanged(e.OldValue, e.NewValue), null);
		}
		private void OnClientTimeZoneIdChanged(string oldValue, string newValue) {
			RaisePropertyChanged("ClientTimeZoneId", oldValue, newValue);
		}
		#endregion
		#region FirstDayOfWeek
		public FirstDayOfWeek FirstDayOfWeek {
			get { return (FirstDayOfWeek)GetValue(FirstDayOfWeekProperty); }
			set { SetValue(FirstDayOfWeekProperty, value); }
		}
		public static readonly DependencyProperty FirstDayOfWeekProperty = CreateFirstDayOfWeekProperty();
		static DependencyProperty CreateFirstDayOfWeekProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DXSchedulerPrintAdapter, FirstDayOfWeek>("FirstDayOfWeek", FirstDayOfWeek.System, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnFirstDayOfWeekChanged(e.OldValue, e.NewValue));
		}
		private void OnFirstDayOfWeekChanged(FirstDayOfWeek oldValue, FirstDayOfWeek newValue) {
			RaisePropertyChanged("FirstDayOfWeek", oldValue, newValue);
		}
		#endregion
		#region TimeInterval
		public TimeInterval TimeInterval {
			get { return (TimeInterval)GetValue(TimeIntervalProperty); }
			set { SetValue(TimeIntervalProperty, value); }
		}
		public static readonly DependencyProperty TimeIntervalProperty = CreateTimeIntervalProperty();
		static DependencyProperty CreateTimeIntervalProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DXSchedulerPrintAdapter, TimeInterval>("TimeInterval", NotificationTimeInterval.Empty, (d, e) => d.OnTimeIntervalChanged(e.OldValue, e.NewValue), null);
		}
		private void OnTimeIntervalChanged(TimeInterval oldValue, TimeInterval newValue) {
			RaisePropertyChanged("TimeInterval", oldValue, newValue);
		}
		#endregion
		#region EnableSmartSync
		public bool EnableSmartSync {
			get { return (bool)GetValue(EnableSmartSyncProperty); }
			set { SetValue(EnableSmartSyncProperty, value); }
		}
		public static readonly DependencyProperty EnableSmartSyncProperty = CreateEnableSmartSyncProperty();
		static DependencyProperty CreateEnableSmartSyncProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DXSchedulerPrintAdapter, bool>("EnableSmartSync", false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnEnableSmartSyncChanged(e.OldValue, e.NewValue));
		}
		private void OnEnableSmartSyncChanged(bool oldValue, bool newValue) {
			RaisePropertyChanged("EnableSmartSync", oldValue, newValue);
		}
		#endregion
		#region SmartSyncOptions
		public ISmartSyncOptions SmartSyncOptions {
			get { return (ISmartSyncOptions)GetValue(SmartSyncOptionsProperty); }
			set { SetValue(SmartSyncOptionsProperty, value); }
		}
		public static readonly DependencyProperty SmartSyncOptionsProperty = CreateSmartSyncOptionsProperty();
		static DependencyProperty CreateSmartSyncOptionsProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DXSchedulerPrintAdapter, ISmartSyncOptions>("SmartSyncOptions", null, (d, e) => d.OnSmartSyncOptionsChanged(e.OldValue, e.NewValue), null);
		}
		private void OnSmartSyncOptionsChanged(ISmartSyncOptions oldValue, ISmartSyncOptions newValue) {
			RaisePropertyChanged("SmartSyncOptions", oldValue, newValue);
		}
		#endregion
		#region ResourceColorSchemas
		ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> ISchedulerPrintAdapterPropertiesBase.ResourceColorSchemas {
			get { return ResourceColorSchemas; }
		}
		public SchedulerColorSchemaCollection ResourceColorSchemas {
			get { return (SchedulerColorSchemaCollection)GetValue(ResourceColorSchemasProperty); }
			set { SetValue(ResourceColorSchemasProperty, value); }
		}
		public static readonly DependencyProperty ResourceColorSchemasProperty = CreateResourceColorSchemasProperty();
		static DependencyProperty CreateResourceColorSchemasProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DXSchedulerPrintAdapter, SchedulerColorSchemaCollection>("ResourceColorSchemas", null, (d, e) => d.OnResourceColorSchemasChanged(e.OldValue, e.NewValue), null);
		}
		private void OnResourceColorSchemasChanged(SchedulerColorSchemaCollection oldValue, SchedulerColorSchemaCollection newValue) {
			RaisePropertyChanged("ResourceColorSchemas", oldValue, newValue);
		}
		#endregion
		#endregion
		#region INotifyPropertyChanged members
		public event PropertyChangingEventHandler PropertyChanging;
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual bool RaisePropertyChanging(string propertyName) {
			if (PropertyChanging != null) {
				PropertyChangingEventArgsEx args = new PropertyChangingEventArgsEx(propertyName);
				PropertyChanging(this, args);
				return !args.Cancel;
			}
			return true;
		}
		protected internal virtual void RaisePropertyChanged<T>(string propertyName, T oldValue, T newValue) {
			if (PropertyChanged != null) {
				PropertyChangedEventArgsEx args = new PropertyChangedEventArgsEx(propertyName, oldValue, newValue);
				PropertyChanged(this, args);
			}
		}
		#endregion
		protected virtual void SubscribeSchedulerAdapterEvents(SchedulerPrintAdapter adapter) {
			adapter.ValidateTimeIntervals += OnSchedulerAdapterValidateTimeIntervals;
			adapter.ValidateResources += OnSchedulerAdapterValidateResources;
			adapter.ValidateWorkTime += OnSchedulerAdapterValidateWorkTime;
			adapter.ValidateAppointments += OnSchedulerAdapterValidateAppointments;
			adapter.SchedulerSourceChanged += OnSchedulerAdapterSchedulerSourceChanged;
		}
		protected virtual void UnsubscribeSchedulerAdapterEvents(SchedulerPrintAdapter adapter) {
			adapter.ValidateTimeIntervals -= OnSchedulerAdapterValidateTimeIntervals;
			adapter.ValidateResources -= OnSchedulerAdapterValidateResources;
			adapter.ValidateWorkTime -= OnSchedulerAdapterValidateWorkTime;
			adapter.ValidateAppointments -= OnSchedulerAdapterValidateAppointments;
			adapter.SchedulerSourceChanged -= OnSchedulerAdapterSchedulerSourceChanged;
		}
		void OnSchedulerAdapterValidateTimeIntervals(object sender, TimeIntervalsValidationEventArgs e) {
			RaiseValidateTimeIntervals(sender, e);
		}
		void OnSchedulerAdapterValidateResources(object sender, ResourcesValidationEventArgs e) {
			RaiseValidateResources(sender, e);
		}
		void OnSchedulerAdapterValidateWorkTime(object sender, WorkTimeValidationEventArgs e) {
			RaiseValidateWorkTime(sender, e);
		}
		void OnSchedulerAdapterValidateAppointments(object sender, AppointmentsValidationEventArgs e) {
			RaiseValidateAppointments(sender, e);
		}
		void OnSchedulerAdapterSchedulerSourceChanged(object sender, EventArgs e) {
			RaiseSchedulerSourceChanged(sender, e);
		}
		void ISupportInitialize.BeginInit() {
			ResourceColorSchemas.Clear();
		}
		void ISupportInitialize.EndInit() {
			if (ResourceColorSchemas.Count == 0)
				ResourceColorSchemas.LoadDefaults();
		}
#if !SILVERLIGHT
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			if (SmartSyncOptions == null)
				SmartSyncOptions = new SchedulerSmartSyncOptions(); 
		}
#endif
		void IDisposable.Dispose() {
			timeZoneHelper = null;
			if (this.schedulerAdapter != null) {
				UnsubscribeSchedulerAdapterEvents(SchedulerAdapter);
				IDisposable schedulerAdapterToDispose = this.schedulerAdapter;
				this.schedulerAdapter = null;
				schedulerAdapterToDispose.Dispose();
			}
		}
		string ISchedulerPrintAdapterPropertiesBase.ClientTimeZoneId {
			get {
				return ClientTimeZoneId ?? SchedulerPrintAdapter.GetDefaultTimeZoneId();
			}
			set {
				ClientTimeZoneId = value;
			}
		}
	}
#if !SILVERLIGHT
	[DXToolboxBrowsable, ToolboxTabName(AssemblyInfo.DXTabWpfScheduling)]
#endif
	public class DXSchedulerControlPrintAdapter : DXSchedulerPrintAdapter {
		public DXSchedulerControlPrintAdapter() {
		}
		public DXSchedulerControlPrintAdapter(SchedulerControl control) {
			SchedulerControl = control;
		}
		#region SchedulerControl
		public SchedulerControl SchedulerControl {
			get { return (SchedulerControl)GetValue(SchedulerControlProperty); }
			set { SetValue(SchedulerControlProperty, value); }
		}
		public static readonly DependencyProperty SchedulerControlProperty = CreateSchedulerControlProperty();
		static DependencyProperty CreateSchedulerControlProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DXSchedulerControlPrintAdapter, SchedulerControl>("SchedulerControl", null, (d, e) => d.OnSchedulerControlChanged(e.OldValue, e.NewValue), null);
		}
		protected virtual void OnSchedulerControlChanged(SchedulerControl oldValue, SchedulerControl newValue) {
			InnerSchedulerAdapter.SchedulerControl = newValue;
		}
		#endregion
		protected InnerSchedulerControlPrintAdapter InnerSchedulerAdapter {
			get { return (InnerSchedulerControlPrintAdapter)SchedulerAdapter; }
		}
		protected override SchedulerPrintAdapter CreateInnerSchedulerPrintAdapter(ISchedulerPrintAdapterPropertiesBase properties) {
			return new InnerSchedulerControlPrintAdapter(SchedulerControl, this);
		}
		protected override string GetDesignTimeImageName() {
			return "DevExpress.Xpf.Scheduler.Images.DXSchedulerControlPrintAdapter.png";
		}
	}
#if !SILVERLIGHT
	[DXToolboxBrowsable, ToolboxTabName(AssemblyInfo.DXTabWpfScheduling)]
#endif
	public class DXSchedulerStoragePrintAdapter : DXSchedulerPrintAdapter {
		public DXSchedulerStoragePrintAdapter() {
		}
		public DXSchedulerStoragePrintAdapter(SchedulerStorage storage) {
			SchedulerStorage = storage;
		}
		protected override SchedulerPrintAdapter CreateInnerSchedulerPrintAdapter(ISchedulerPrintAdapterPropertiesBase properties) {
			return new InnerSchedulerStoragePrintAdapter(SchedulerStorage, this);
		}
		protected override string GetDesignTimeImageName() {
			return "DevExpress.Xpf.Scheduler.Images.DXSchedulerStoragePrintAdapter.png";
		}
		#region SchedulerStorage
		public SchedulerStorage SchedulerStorage {
			get { return (SchedulerStorage)GetValue(SchedulerStorageProperty); }
			set { SetValue(SchedulerStorageProperty, value); }
		}
		public static readonly DependencyProperty SchedulerStorageProperty = CreateSchedulerStorageProperty();
		static DependencyProperty CreateSchedulerStorageProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DXSchedulerStoragePrintAdapter, SchedulerStorage>("SchedulerStorage", null, (d, e) => d.OnSchedulerStorageChanged(e.OldValue, e.NewValue), null);
		}
		protected virtual void OnSchedulerStorageChanged(SchedulerStorage oldValue, SchedulerStorage newValue) {
			InnerSchedulerAdapter.SchedulerStorage = newValue;
		}
		#endregion
		protected InnerSchedulerStoragePrintAdapter InnerSchedulerAdapter {
			get { return (InnerSchedulerStoragePrintAdapter)SchedulerAdapter; }
		}
	}
	public class SchedulerSmartSyncOptions : DependencyObject, ISmartSyncOptions {
		public SchedulerGroupType GroupType {
			get { return (SchedulerGroupType)GetValue(GroupTypeProperty); }
			set { SetValue(GroupTypeProperty, value); }
		}
		public static readonly DependencyProperty GroupTypeProperty = CreateGroupTypeProperty();
		static DependencyProperty CreateGroupTypeProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerSmartSyncOptions, SchedulerGroupType>("GroupType", SchedulerGroupType.Resource, (d, e) => d.OnGroupTypeChanged(e.OldValue, e.NewValue), null);
		}
		protected virtual void OnGroupTypeChanged(SchedulerGroupType oldValue, SchedulerGroupType newValue) {
			if (oldValue == newValue)
				return;
			RaisePropertyChanged("GroupType", oldValue, newValue);
		}
		#region PropertyChanged
		PropertyChangedEventHandler onPropertyChanged;
		public event PropertyChangedEventHandler PropertyChanged { add { onPropertyChanged += value; } remove { onPropertyChanged -= value; } }
		protected internal virtual void RaisePropertyChanged<T>(string propertyName, T oldValue, T newValue) {
			if (onPropertyChanged != null) {
				PropertyChangedEventArgsEx args = new PropertyChangedEventArgsEx(propertyName, oldValue, newValue);
				onPropertyChanged(this, args);
			}
		}
		#endregion
	}
}
namespace DevExpress.Xpf.Scheduler.Reporting.Native {
	public class SchedulerPrintAdapterProperties : SchedulerPrintAdapterPropertiesBase {
		public new SchedulerColorSchemaCollection ResourceColorSchemas { get { return (SchedulerColorSchemaCollection)base.ResourceColorSchemas; } }
		protected override ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> CreateResourceColorSchemas() {
			return new SchedulerColorSchemaCollection();
		}
		protected override ICollectionChangedListener CreateResourceColorsListener() {
			return new ResourceColorSchemasChangedListener(ResourceColorSchemas);
		}
	}
	public class InnerSchedulerControlPrintAdapter : SchedulerPrintAdapter {
		SchedulerControl schedulerControl;
		public InnerSchedulerControlPrintAdapter() {
		}
		public InnerSchedulerControlPrintAdapter(SchedulerControl control, ISchedulerPrintAdapterPropertiesBase properties)
			: base(properties) {
			this.schedulerControl = control;
			UpdateTimeZoneEngine(GetClientTimeZoneId());
		}
		[Category(SRCategoryNames.Scheduler), DefaultValue(null)]
		public SchedulerControl SchedulerControl {
			get { return schedulerControl; }
			set {
				if (schedulerControl == value)
					return;
				schedulerControl = value;
				OnSchedulerControlChanged();
			}
		}
		public new SchedulerColorSchemaCollection ResourceColorSchemas {
			get { return (SchedulerColorSchemaCollection)base.ResourceColorSchemas; }
		}
		protected void OnSchedulerControlChanged() {
			UpdateTimeZoneEngine(GetClientTimeZoneId());
			RaiseSchedulerSourceChanged();
		}
		protected override ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> GetResourceColorSchemasCore() {
			if (SchedulerControl != null)
				return SchedulerControl.InnerControl.ResourceColorSchemas; 
			return base.GetResourceColorSchemasCore();
		}
		protected override ResourceBaseCollection GetResourcesCore() {
			if (SchedulerControl != null)
				return SchedulerControl.InnerControl.ActiveView.GetResources();
			return base.GetResourcesCore();
		}
		protected override AppointmentBaseCollection GetAppointmentsCore(TimeInterval timeInterval, ResourceBaseCollection resources) {
			bool reloaded = false;
			if (SchedulerControl != null)
				return SchedulerControl.InnerControl.GetFilteredAppointments(timeInterval, resources, out reloaded);
			return base.GetAppointmentsCore(timeInterval, resources);
		}
		protected override IAppointmentStatus GetStatusCore(object statusId) {
			if (SchedulerControl != null)
				return SchedulerControl.GetStatus(statusId);
			return base.GetStatusCore(statusId);
		}
#if !SL
		protected override System.Drawing.Color GetLabelColorCore(object labelId) {
			if (SchedulerControl != null) {
				Color color = SchedulerControl.GetLabelColor(labelId);
				return DXColorConverter.ToDrawingColor(color);
			}
			return base.GetLabelColorCore(labelId);
		}
#endif
		protected override object GetServiceCore(Type serviceType) {
			if (SchedulerControl != null)
				return SchedulerControl.GetService(serviceType);
			return base.GetServiceCore(serviceType);
		}
		public override WorkDaysCollection GetWorkDays() {
			if (SchedulerControl != null) {
				WorkDaysCollection result = new WorkDaysCollection();
				result.AddRange(SchedulerControl.WorkDays);
				return result;
			}
			return base.GetWorkDays();
		}
		protected override TimeOfDayIntervalCollection GetWorkTimeCore(TimeInterval interval, Resource resource) {
			if (SchedulerControl == null)
				return base.GetWorkTimeCore(interval, resource);
			TimeOfDayIntervalCollection result = SchedulerControl.ActiveView.InnerView.CalcResourceWorkTimeInterval(interval, resource);
			if (result.Count > 0)
				return result;
			SchedulerViewType type = SchedulerControl.ActiveViewType;
			if (type == SchedulerViewType.Day)
				result.Add(SchedulerControl.DayView.WorkTime);
			else if (type == SchedulerViewType.WorkWeek)
				result.Add(SchedulerControl.WorkWeekView.WorkTime);
			else if (type == SchedulerViewType.Timeline)
				result.Add(SchedulerControl.TimelineView.WorkTime);
			return result;
		}
		protected override ISchedulerPrintAdapterPropertiesBase CreateProperties() {
			return new SchedulerPrintAdapterProperties();
		}
		protected internal override TimeIntervalCollection GetTimeIntervalsCore() {
			if (SchedulerControl != null) {
				return SchedulerControl.ActiveView.GetVisibleIntervals();
			}
			return base.GetTimeIntervalsCore();
		}
		protected internal override string GetClientTimeZoneIdCore() {
			if (SchedulerControl != null)
				return SchedulerControl.OptionsBehavior.ClientTimeZoneId;
			return base.GetClientTimeZoneIdCore();
		}
		protected override IStatusBrushAdapter CreateBrushAdapterImplementation() {
			return new WpfStatusBrushAdapter();
		}
		public override void SetSourceObject(object sourceObject) {
			if (sourceObject == null) {
				SchedulerControl = null;
				return;
			}
			SchedulerControl control = sourceObject as SchedulerControl;
			if (control != null) {
				SchedulerControl = control;
				return;
			}
			Exceptions.ThrowArgumentException("sourceObject", sourceObject);
		}
		internal override HeaderCaptionFormatProviderBase GetHeaderCaptionFormatProvider() {
			IHeaderCaptionService service = GetService(typeof(IHeaderCaptionService)) as IHeaderCaptionService;
			return service != null ? new HeaderCaptionFormatProvider(service) : null;
		}
		protected override void UpdateTimeZoneEngine(string clientTimeZoneId) {
			base.UpdateTimeZoneEngine(clientTimeZoneId);
			if (SchedulerControl != null)
				Properties.TimeZoneHelper.StorageTimeZoneEngine = SchedulerControl.TimeZoneHelper.StorageTimeZoneEngine;
		}
		internal override void RunCreatingDocument(Action<bool> createDocumentMethod, bool methodParam) {
			SchedulerControl.Dispatcher.BeginInvoke(createDocumentMethod, DispatcherPriority.Render, methodParam);
		}
	}
	public class InnerSchedulerStoragePrintAdapter : SchedulerStorageBasePrintAdapter {
		SchedulerStorage storage;
		public InnerSchedulerStoragePrintAdapter() {
		}
		public InnerSchedulerStoragePrintAdapter(SchedulerStorage storage, ISchedulerPrintAdapterPropertiesBase properties)
			: base(storage.InnerStorage, properties) {
			this.storage = storage;
		}
		#region Properties
		[Category(SRCategoryNames.Scheduler), DefaultValue(null)]
		public SchedulerStorage SchedulerStorage {
			get { return storage; }
			set {
				this.storage = value;
				base.StorageBase = storage != null ? storage.InnerStorage : null;
			}
		}
		#endregion
#if !SL
		protected override System.Drawing.Color GetLabelColorCore(object labelId) {
			if (SchedulerStorage != null) {
				Color color = SchedulerStorage.GetLabelColor(labelId);
				return DXColorConverter.ToDrawingColor(color);
			}
			return base.GetLabelColorCore(labelId);
		}
#endif
		protected override ISchedulerPrintAdapterPropertiesBase CreateProperties() {
			return new SchedulerPrintAdapterProperties();
		}
		public override void SetSourceObject(object sourceObject) {
			if (sourceObject == null) {
				SchedulerStorage = null;
				return;
			}
			SchedulerStorage storage = sourceObject as SchedulerStorage;
			if (storage != null) {
				SchedulerStorage = storage;
				return;
			}
			SchedulerControl control = sourceObject as SchedulerControl;
			if (control != null) {
				SchedulerStorage = control.Storage;
				return;
			}
			Exceptions.ThrowArgumentException("sourceObject", sourceObject);
		}
		internal override HeaderCaptionFormatProviderBase GetHeaderCaptionFormatProvider() {
			IHeaderCaptionService service = GetService(typeof(IHeaderCaptionService)) as IHeaderCaptionService;
			return service != null ? new HeaderCaptionFormatProvider(service) : null;
		}
		protected override void UpdateTimeZoneEngine(string tzId) {
			base.UpdateTimeZoneEngine(tzId);
			IInternalSchedulerStorageBase internalStorage = StorageBase as IInternalSchedulerStorageBase;
			if (internalStorage != null)
				Properties.TimeZoneHelper.StorageTimeZoneEngine = internalStorage.TimeZoneEngine;
		}
		protected override IStatusBrushAdapter CreateBrushAdapterImplementation() {
			return new WpfStatusBrushAdapter();
		}
		internal override void RunCreatingDocument(Action<bool> createDocumentMethod, bool methodParam) {
			Popup hiddenPopup = new Popup();
			hiddenPopup.Width = 0;
			hiddenPopup.Height = 0;
			hiddenPopup.DataContext = SchedulerStorage;
			StatusBrushVisualizer visualizer = new StatusBrushVisualizer();
			hiddenPopup.Child = visualizer;
			hiddenPopup.IsOpen = true;
			hiddenPopup.Dispatcher.BeginInvoke(new Action<Action<bool>, bool, Popup>((methodToInvoke, methodArg, popupElement) => {
				methodToInvoke(methodArg);
				popupElement.IsOpen = false;
				popupElement.Visibility = Visibility.Collapsed;
				popupElement = null;
			}), DispatcherPriority.Render, createDocumentMethod, methodParam, hiddenPopup);
		}
	}
	public class WpfStatusBrushAdapter : IStatusBrushAdapter {
		Dictionary<IAppointmentStatus, Dictionary<Size, System.Drawing.Image>> imageCache;
		public Dictionary<IAppointmentStatus, Dictionary<Size, System.Drawing.Image>> ImageCache {
			get { return this.imageCache ?? (this.imageCache = new Dictionary<IAppointmentStatus, Dictionary<Size, System.Drawing.Image>>()); }
		}
		System.Drawing.Image GetImageFromCache(IAppointmentStatus status, Size imageSize) {
			Dictionary<Size, System.Drawing.Image> sizeCache;
			if (!ImageCache.TryGetValue(status, out sizeCache)) {
				ImageCache[status] = new Dictionary<Size, System.Drawing.Image>();
				return null;
			}
			System.Drawing.Image img;
			if (!sizeCache.TryGetValue(imageSize, out img))
				return null;
			return img;
		}
		void PutImageToCache(IAppointmentStatus status, System.Drawing.Image img) {
			ImageCache[status][new Size(img.Width, img.Height)] = img;
		}
		public System.Drawing.Image CreateImage(System.Drawing.Rectangle bounds, IAppointmentStatus status) {
			Size imageSize = new Size(bounds.Width, bounds.Height);
			System.Drawing.Image image = GetImageFromCache(status, imageSize);
			if (image == null) {
				byte[] imageBytes = BrushToImageBytesConverter.Convert(status.GetBrush(), imageSize);
				image = SchedulerImageHelper.CreateImageFromBytes(imageBytes);
				if (image != null)
					PutImageToCache(status, image);
			}
			return image;
		}
		public void ClearCache() {
			if (this.imageCache == null || this.imageCache.Count == 0)
				return;
			foreach (KeyValuePair<IAppointmentStatus, Dictionary<Size, System.Drawing.Image>> imageCacheEntry in ImageCache) {
				foreach (KeyValuePair<Size, System.Drawing.Image> sizeCacheEntry in imageCacheEntry.Value)
					sizeCacheEntry.Value.Dispose();
			}
			ImageCache.Clear();
		}
	}
}
