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
using System.Text;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraScheduler.Reporting;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.Web;
using DevExpress.Web.ASPxScheduler.Reporting.Native;
using DevExpress.Services.Internal;
using System.ComponentModel.Design;
using DevExpress.Web.Internal;
using System.Web.UI;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Web.ASPxScheduler.Services;
using DevExpress.Web.ASPxScheduler.Services.Internal;
using DevExpress.Web.ASPxScheduler.Internal;
namespace DevExpress.Web.ASPxScheduler.Reporting {
	#region ASPxSchedulerPrintAdapterBase (abstract class)
	public abstract class ASPxSchedulerPrintAdapterBase : ASPxWebComponent {
		SchedulerPrintAdapter schedulerAdapter;
		protected ASPxSchedulerPrintAdapterBase() {
		}
		#region Properties
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerPrintAdapterBaseSchedulerControlID"),
#endif
Category(SRCategoryNames.Scheduler), DefaultValue(""), Themeable(false), AutoFormatDisable(),
		IDReferenceProperty(typeof(ASPxScheduler)),
		TypeConverter("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerIDConverter, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull)]
		public virtual string SchedulerControlID {
			get { return GetStringProperty("SchedulerCtrlId", String.Empty); }
			set {
				SetStringProperty("SchedulerCtrlId", String.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerPrintAdapterBaseWorkTime"),
#endif
Category(SRCategoryNames.Scheduler),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		]
		public TimeOfDayInterval WorkTime {
			get { return SchedulerAdapter.WorkTime; }
			set {
				SchedulerAdapter.WorkTime = value;
			}
		}
		internal bool ShouldSerializeWorkTime() {
			return SchedulerAdapter.ShouldSerializeWorkTime();
		}
		internal void ResetWorkTime() {
			SchedulerAdapter.ResetWorkTime();
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerPrintAdapterBaseFirstDayOfWeek"),
#endif
Category(SRCategoryNames.Scheduler), DefaultValue(FirstDayOfWeek.System)]
		public FirstDayOfWeek FirstDayOfWeek {
			get { return SchedulerAdapter.FirstDayOfWeek; }
			set { SchedulerAdapter.FirstDayOfWeek = value; }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerPrintAdapterBaseClientTimeZoneId"),
#endif
Category(SRCategoryNames.Scheduler)]
		public string ClientTimeZoneId {
			get { return SchedulerAdapter.ClientTimeZoneId; }
			set { SchedulerAdapter.ClientTimeZoneId = value; }
		}
		internal bool ShouldSerializeClientTimeZoneId() {
			return SchedulerAdapter.ShouldSerializeClientTimeZoneId();
		}
		internal void ResetClientTimeZoneId() {
			SchedulerAdapter.ResetClientTimeZoneId();
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerPrintAdapterBaseTimeInterval"),
#endif
		Category(SRCategoryNames.Scheduler), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public TimeInterval TimeInterval {
			get { return SchedulerAdapter.TimeInterval; }
			set { SchedulerAdapter.TimeInterval = value; }
		}
		internal bool ShouldSerializeTimeInterval() {
			return SchedulerAdapter.ShouldSerializeTimeInterval();
		}
		internal void ResetTimeInterval() {
			SchedulerAdapter.ResetTimeInterval();
		}
		[
Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SchedulerColorSchemaCollection ResourceColorSchemas { get { return (SchedulerColorSchemaCollection)SchedulerAdapter.ResourceColorSchemas; } }
		internal bool ShouldSerializeResourceColorSchemas() {
			return SchedulerAdapter.ShouldSerializeResourceColorSchemas();
		}
		internal void ResetResourceColorSchemas() {
			SchedulerAdapter.ResetResourceColorSchemas();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SchedulerPrintAdapter SchedulerAdapter {
			get {
				if (schedulerAdapter == null) {
					schedulerAdapter = CreateInnerSchedulerPrintAdapter();
					InitInnerSchedulerPrintAdapter();
				}
				return schedulerAdapter;
			}
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerPrintAdapterBaseEnableSmartSync"),
#endif
Category(SRCategoryNames.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		]
		public bool EnableSmartSync {
			get { return SchedulerAdapter.EnableSmartSync; }
			set {
				SchedulerAdapter.EnableSmartSync = value;
			}
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerPrintAdapterBaseSmartSyncOptions"),
#endif
Category(SRCategoryNames.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		]
		public ISmartSyncOptions SmartSyncOptions { get { return SchedulerAdapter.SmartSyncOptions; } }
		#endregion
		#region Events
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerPrintAdapterBaseValidateTimeIntervals"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event TimeIntervalsValidationEventHandler ValidateTimeIntervals
		{
			add { SchedulerAdapter.ValidateTimeIntervals += value; }
			remove { SchedulerAdapter.ValidateTimeIntervals -= value; }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerPrintAdapterBaseValidateWorkTime"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event WorkTimeValidationEventHandler ValidateWorkTime
		{
			add { SchedulerAdapter.ValidateWorkTime += value; }
			remove { SchedulerAdapter.ValidateWorkTime -= value; }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerPrintAdapterBaseValidateAppointments"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event AppointmentsValidationEventHandler ValidateAppointments
		{
			add { SchedulerAdapter.ValidateAppointments += value; }
			remove { SchedulerAdapter.ValidateAppointments -= value; }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerPrintAdapterBaseValidateResources"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event ResourcesValidationEventHandler ValidateResources
		{
			add { SchedulerAdapter.ValidateResources += value; }
			remove { SchedulerAdapter.ValidateResources -= value; }
		}
		#endregion
		protected abstract SchedulerPrintAdapter CreateInnerSchedulerPrintAdapter();
		protected abstract void InitInnerSchedulerPrintAdapter();
		protected ASPxScheduler TryGetSchedulerControl() {
			return FindControlHelper.LookupControl(this, SchedulerControlID) as ASPxScheduler;
		}
	}
	#endregion
	#region ASPxSchedulerStoragePrintAdapter
	[DXWebToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(ToolboxBitmapAccess), 
	ToolboxBitmapAccess.BitmapPath + "ASPxStorageAdapter.bmp"),
	Designer("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerStoragePrintAdapterDesigner, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull)
	]
	public class ASPxSchedulerStoragePrintAdapter : ASPxSchedulerPrintAdapterBase {
		public ASPxSchedulerStoragePrintAdapter() {
		}
		public ASPxSchedulerStoragePrintAdapter(ASPxSchedulerStorage storage) {
			if (storage == null)
				Exceptions.ThrowArgumentNullException("storage");
			ClientIDHelper.SetClientIDModeToAutoID(this);
			SchedulerStorage = storage;
		}
		protected internal WebSchedulerStoragePrintAdapter InnerStorageAdapter {
			get { return (WebSchedulerStoragePrintAdapter)base.SchedulerAdapter; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ISchedulerStorageBase SchedulerStorage {
			get { return InnerStorageAdapter.SchedulerStorage; }
			set { InnerStorageAdapter.SchedulerStorage = value; }
		}
		protected override SchedulerPrintAdapter CreateInnerSchedulerPrintAdapter() {
			return new WebSchedulerStoragePrintAdapter();
		}
		protected override void InitInnerSchedulerPrintAdapter() {
			ASPxScheduler scheduler = TryGetSchedulerControl();
			if (scheduler != null)
				InnerStorageAdapter.SchedulerStorage = scheduler.Storage;
		}
	}
	#endregion
	#region ASPxSchedulerControlPrintAdapter
	[DXWebToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	 ToolboxBitmap(typeof(ToolboxBitmapAccess), 
	 ToolboxBitmapAccess.BitmapPath + "ASPxSchedulerAdapter.bmp"),
	 Designer("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerControlPrintAdapterDesigner, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull)
	]
	public class ASPxSchedulerControlPrintAdapter : ASPxSchedulerPrintAdapterBase {
		public ASPxSchedulerControlPrintAdapter() {
			ClientIDHelper.SetClientIDModeToAutoID(this);
		}
		public ASPxSchedulerControlPrintAdapter(ASPxScheduler control) {
			if (control == null)
				Exceptions.ThrowArgumentNullException("control");
			SchedulerControl = control;
		}
		protected WebSchedulerControlPrintAdapter InnerSchedulerAdapter {
			get { return (WebSchedulerControlPrintAdapter)base.SchedulerAdapter; } 
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ASPxScheduler SchedulerControl {
			get { 
				return InnerSchedulerAdapter.SchedulerControl; 
			}
			set { InnerSchedulerAdapter.SchedulerControl = value; }
		}
		protected override SchedulerPrintAdapter CreateInnerSchedulerPrintAdapter() {
			return new WebSchedulerControlPrintAdapter();
		}
		protected override void InitInnerSchedulerPrintAdapter() {
			InnerSchedulerAdapter.SchedulerControl = TryGetSchedulerControl();
		}
	}
	#endregion
}
namespace DevExpress.Web.ASPxScheduler.Reporting.Native {
	public class SchedulerPrintAdapterProperties : SchedulerPrintAdapterPropertiesBase {
		public new SchedulerColorSchemaCollection ResourceColorSchemas { get { return (SchedulerColorSchemaCollection)base.ResourceColorSchemas; } }
		protected override ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> CreateResourceColorSchemas() {
			return new SchedulerColorSchemaCollection();
		}
		protected override ICollectionChangedListener CreateResourceColorsListener() {
			return new ResourceColorSchemasChangedListener(ResourceColorSchemas);
		}
	}
	#region WebSchedulerStoragePrintAdapter
	public class WebSchedulerStoragePrintAdapter : SchedulerStorageBasePrintAdapter {
		public WebSchedulerStoragePrintAdapter() {
			InitializeTimeInterval();
		}
		public WebSchedulerStoragePrintAdapter(ASPxSchedulerStorage storage)
			: base(storage) {
			InitializeTimeInterval();
		}
		#region Properties
		public ISchedulerStorageBase SchedulerStorage {
			get { return base.StorageBase; }
			set { base.StorageBase = value; }
		}
		#endregion
		protected override ISchedulerPrintAdapterPropertiesBase CreateProperties() {
			return new SchedulerPrintAdapterProperties();
		}
		public override void SetSourceObject(object sourceObject) {
			if (sourceObject == null) {
				SchedulerStorage = null;
				return;
			}
			ISchedulerStorageBase storage = sourceObject as ISchedulerStorageBase;
			if (storage != null) {
				SchedulerStorage = storage;
				return;
			}
			ASPxScheduler control = sourceObject as ASPxScheduler;
			if (control != null) {
				SchedulerStorage = control.Storage;
				return;
			}
			Exceptions.ThrowArgumentException("sourceObject", sourceObject);
		}
		internal override DevExpress.XtraScheduler.Services.Internal.HeaderCaptionFormatProviderBase GetHeaderCaptionFormatProvider() {
			IHeaderCaptionService service = GetService(typeof(IHeaderCaptionService)) as IHeaderCaptionService;
			return service != null ? new HeaderCaptionFormatProvider(service) : null;			
		}
		internal override void RunCreatingDocument(Action<bool> createDocumentMethod, bool methodParam) {
			createDocumentMethod(methodParam);
		}
		void InitializeTimeInterval() {
			TimeInterval = new TimeInterval(DateTime.Today, TimeSpan.FromDays(1));
		}
	}
	#endregion
	#region WebSchedulerControlPrintAdapter
	public class WebSchedulerControlPrintAdapter : SchedulerPrintAdapter {
		ASPxScheduler schedulerControl;
		public WebSchedulerControlPrintAdapter() {
		}
		public WebSchedulerControlPrintAdapter(ASPxScheduler control) {
			if (schedulerControl == null)
				Exceptions.ThrowArgumentNullException("control");
			this.schedulerControl = control;
		}
		public ASPxScheduler SchedulerControl {
			get { return schedulerControl; }
			set {
				if (schedulerControl == value)
					return;
				schedulerControl = value;
				OnSchedulerControlChanged();
			}
		}
		protected void OnSchedulerControlChanged() {
			UpdateTimeZoneEngine(GetClientTimeZoneId());
			RaiseSchedulerSourceChanged();
		}
		protected override ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> GetResourceColorSchemasCore() {
			if (SchedulerControl != null)
				return SchedulerControl.ResourceColorSchemas;
			return (SchedulerColorSchemaCollection)base.GetResourceColorSchemasCore();
		}
		protected override ResourceBaseCollection GetResourcesCore() {
			if (SchedulerControl != null)
				return SchedulerControl.ActiveView.GetResources();
			return base.GetResourcesCore();
		}
		protected override AppointmentBaseCollection GetAppointmentsCore(TimeInterval timeInterval, ResourceBaseCollection resources) {
			bool reloaded;
			if (SchedulerControl != null)
				return SchedulerControl.InnerControl.GetFilteredAppointments(timeInterval, resources, out reloaded);
			return base.GetAppointmentsCore(timeInterval, resources);
		}
		protected override IAppointmentStatus GetStatusCore(object statusId) {
			if (SchedulerControl != null)
				return SchedulerControl.GetStatus(statusId);
			return base.GetStatusCore(statusId);
		}
		protected override Color GetLabelColorCore(object labelId) {
			if (SchedulerControl != null)
				return SchedulerControl.GetLabelColor(labelId);
			return base.GetLabelColorCore(labelId);
		}
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
		protected override TimeOfDayIntervalCollection GetWorkTimeCore(TimeInterval interval, XtraScheduler.Resource resource) {
			if (SchedulerControl != null) {
				SchedulerViewType type = SchedulerControl.ActiveViewType;
				if (type == SchedulerViewType.Day) {
					TimeOfDayIntervalCollection result = new TimeOfDayIntervalCollection();
					result.Add(SchedulerControl.DayView.WorkTime);
					return result;
				}
				if (type == SchedulerViewType.Timeline) {
					TimeOfDayIntervalCollection result = new TimeOfDayIntervalCollection();
					result.Add(SchedulerControl.TimelineView.WorkTime);
					return result;
				}
			}
			return base.GetWorkTimeCore(interval, resource);
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
		public override void SetSourceObject(object sourceObject) {
			if (sourceObject == null) {
				SchedulerControl = null;
				return;
			}
			ASPxScheduler control = sourceObject as ASPxScheduler;
			if (control != null) {
				SchedulerControl = control;
				return;
			}
			Exceptions.ThrowArgumentException("sourceObject", sourceObject);
		}
		internal override DevExpress.XtraScheduler.Services.Internal.HeaderCaptionFormatProviderBase GetHeaderCaptionFormatProvider() {
			IHeaderCaptionService service = GetService(typeof(IHeaderCaptionService)) as IHeaderCaptionService;
			return service != null ? new HeaderCaptionFormatProvider(service) : null;
		}
		internal override void RunCreatingDocument(Action<bool> createDocumentMethod, bool methodParam) {
			createDocumentMethod(methodParam);
		}
	}
	#endregion
}
