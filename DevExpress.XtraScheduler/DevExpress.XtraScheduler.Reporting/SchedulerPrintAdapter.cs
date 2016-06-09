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
using System.Collections;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.Reporting {
	public class SchedulerPrintAdapterProperties : SchedulerPrintAdapterPropertiesBase {
		public new SchedulerColorSchemaCollection ResourceColorSchemas { get { return (SchedulerColorSchemaCollection)base.ResourceColorSchemas; } }
		protected override ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> CreateResourceColorSchemas() {
			return new SchedulerColorSchemaCollection();
		}
		protected override ICollectionChangedListener CreateResourceColorsListener() {
			return new ResourceColorSchemasChangedListener(ResourceColorSchemas);
		}
	}
	[DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabSchedulerReporting),
	ToolboxBitmap(typeof(XtraSchedulerReport), DevExpress.Utils.ControlConstants.BitmapPath + "controladapter.bmp"),
	Description("A component bound to the SchedulerControl used for retrieving data and scheduler settings.")]
	public class SchedulerControlPrintAdapter : SchedulerPrintAdapter {
		SchedulerControl schedulerControl;
		public SchedulerControlPrintAdapter() {
		}
		public SchedulerControlPrintAdapter(SchedulerControl control) {
			this.schedulerControl = control;
			UpdateTimeZoneEngine(GetClientTimeZoneId());
		}
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("SchedulerControlPrintAdapterSchedulerControl"),
#endif
		Category(SRCategoryNames.Scheduler), DefaultValue(null)]
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
				return SchedulerControl.ActualResourceColorSchemas;
			return base.GetResourceColorSchemasCore();
		}
		protected override ResourceBaseCollection GetResourcesCore() {
			if (SchedulerControl != null)
				return SchedulerControl.ActiveView.GetResources();
			return base.GetResourcesCore();
		}
		protected override AppointmentBaseCollection GetAppointmentsCore(TimeInterval timeInterval, ResourceBaseCollection resources) {
			if (SchedulerControl != null) {
				bool reloaded;
				return SchedulerControl.InnerControl.GetFilteredAppointments(timeInterval, resources, out reloaded);
			}
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
			else if (type == SchedulerViewType.FullWeek)
				result.Add(schedulerControl.FullWeekView.WorkTime);
			else if (type == SchedulerViewType.Timeline)
				result.Add(SchedulerControl.TimelineView.WorkTime);
			return result;
		}
		protected override ISchedulerPrintAdapterPropertiesBase CreateProperties() {
			return new SchedulerPrintAdapterPropertiesBase();
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
			createDocumentMethod(methodParam);
		}
	}
	[DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabSchedulerReporting),
	ToolboxBitmap(typeof(XtraSchedulerReport), DevExpress.Utils.ControlConstants.BitmapPath + "storageadapter.bmp"),
	Description("A component bound to the SchedulerStorage component used for retrieving data and scheduler settings.")
	]
	public class SchedulerStoragePrintAdapter : SchedulerStorageBasePrintAdapter {
		public SchedulerStoragePrintAdapter() {
			InitializeTimeInterval();
		}
		public SchedulerStoragePrintAdapter(ISchedulerStorageBase storage)
			: base(storage) {
			InitializeTimeInterval();
		}
		#region Properties
		[
		Category(SRCategoryNames.Scheduler), DefaultValue(null)]
		public ISchedulerStorageBase SchedulerStorage {
			get { return StorageBase; }
			set { StorageBase = value; }
		}
		public new SchedulerColorSchemaCollection ResourceColorSchemas {
			get { return (SchedulerColorSchemaCollection)base.ResourceColorSchemas; }
		}
		#endregion
		void InitializeTimeInterval() {
			TimeInterval = new TimeInterval(DateTime.Today, TimeSpan.FromDays(1));
		}
		protected override ISchedulerPrintAdapterPropertiesBase CreateProperties() {
			return new SchedulerPrintAdapterPropertiesBase();
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
		internal override void RunCreatingDocument(Action<bool> createDocumentMethod, bool methodParam) {
			createDocumentMethod(methodParam);
		}
	}
}
namespace DevExpress.XtraScheduler.Reporting.Native {
	[DXToolboxItem(false)]
	public class InnerPrintAdapter : SchedulerPrintAdapter {
		protected override ISchedulerPrintAdapterPropertiesBase CreateProperties() {
			return new SchedulerPrintAdapterProperties();
		}
		public override void SetSourceObject(object sourceObject) {
		}
		internal override HeaderCaptionFormatProviderBase GetHeaderCaptionFormatProvider() {
			IHeaderCaptionService service = GetService(typeof(IHeaderCaptionService)) as IHeaderCaptionService;
			return service != null ? new HeaderCaptionFormatProvider(service) : null;
		}
		internal override void RunCreatingDocument(Action<bool> createDocumentMethod, bool methodParam) {
			createDocumentMethod(methodParam);
		}
	}
	#region ResourceColorSchemasCache
	public class ResourceColorSchemasCache {
		SchedulerColorSchemaCollection schedulerColorSchemaCollection;
		Hashtable hashTable = new Hashtable();
		public ResourceColorSchemasCache() {
			schedulerColorSchemaCollection = new SchedulerColorSchemaCollection();
			schedulerColorSchemaCollection.LoadDefaults();
			HashTable.Add(ResourceBase.Empty.Id, schedulerColorSchemaCollection.DefaultSchema1);
		}
		internal Hashtable HashTable { get { return hashTable; } }
		SchedulerColorSchema ConvertToWinSchema(SchedulerColorSchemaBase baseSchema) {
			SchedulerColorSchema resultSchema = new SchedulerColorSchema(Color.FromArgb(baseSchema.BaseColorValue));
			resultSchema.CellColorValue = baseSchema.CellColorValue;
			resultSchema.CellBorderColorValue = baseSchema.CellBorderColorValue;
			resultSchema.CellBorderDarkColorValue = baseSchema.CellBorderDarkColorValue;
			resultSchema.CellLightColorValue = baseSchema.CellLightColorValue;
			resultSchema.CellLightBorderColorValue = baseSchema.CellLightBorderColorValue;
			resultSchema.CellLightBorderDarkColorValue = baseSchema.CellLightBorderDarkColorValue;
			return resultSchema;
		}
		public virtual void Update(ResourceBaseCollection resources, ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> schemas) {
			PopulateHashTable(resources, schemas);
		}
		protected internal virtual void PopulateHashTable(ResourceBaseCollection resources, ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> schemas) {
			hashTable.Clear();
			int count = resources.Count;
			for (int i = 0; i < count; i++) {
				Resource resource = resources[i];
				SchedulerColorSchemaBase schema = schemas.GetSchema(resource.GetColor().ToArgb(), i);
				AddColorSchema(resource.Id, schema);
			}
			AddEmptyResourceColorSchema(schemas);
		}
		protected internal virtual void AddEmptyResourceColorSchema(ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> schemas) {
			SchedulerColorSchemaBase emptyResourceSchema = schemas.Count > 0 ? schemas[0] : schedulerColorSchemaCollection.DefaultSchema1;
			AddColorSchema(ResourceBase.Empty.Id, emptyResourceSchema);
		}
		protected internal virtual void AddColorSchema(object resourceId, SchedulerColorSchemaBase schema) {
			if (hashTable[resourceId] == null)
				hashTable.Add(resourceId, schema);
		}
		protected internal virtual SchedulerColorSchema GetSchema(Resource resource) {
			SchedulerColorSchemaBase baseSchema = (SchedulerColorSchemaBase)HashTable[resource.Id];
			if (baseSchema == null)
				baseSchema = (SchedulerColorSchemaBase)HashTable[ResourceBase.Empty.Id];
			SchedulerColorSchema schema = baseSchema as SchedulerColorSchema;
			if (schema == null)
				schema = ConvertToWinSchema(baseSchema);
			return schema;
		}
	}
	#endregion
}
