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
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.Utils.Controls;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraScheduler {
	public class SchedulerStorageRangeControlClient : ScaleBasedRangeControlClient {
		public SchedulerStorageRangeControlClient(SchedulerStorage storage) {
			Guard.ArgumentNotNull(storage, "storage");
			Storage = storage;
			Initialize(CreateDataProvider());
		}
		public SchedulerStorage Storage { get; private set; }
		protected SchedulerStorageClientDataProvider CreateDataProvider() {
			return new SchedulerStorageClientDataProvider(Storage);
		}
		[Category(SRCategoryNames.Options),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)
		]
		public new SchedulerStorageRangeControlClientOptions Options { get { return base.Options as SchedulerStorageRangeControlClientOptions; } }
		protected override void ApplyHitInterval(TimeInterval interval) {
			base.ApplyHitInterval(interval);
			RefreshRangeControlCore(false);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (Storage != null) {
					Storage = null;
				}
			}
			base.Dispose(disposing);
		}
	}
	public class SchedulerStorageRangeControlClientOptions : ScaleBasedRangeControlClientOptions {
	}
}
namespace DevExpress.XtraScheduler.Native {
	public class SchedulerStorageClientDataProvider : SchedulerStorageBaseClientDataProvider {
		SchedulerStorageRangeControlClientOptions optionsInstance;
		public SchedulerStorageClientDataProvider(SchedulerStorage storage)
			: base(storage) {
			Guard.ArgumentNotNull(storage, "storage");
		}
		public SchedulerStorage SchedulerStorage { get { return Storage as SchedulerStorage; } }
		protected internal TimeInterval SelectedInterval { get; private set; }
		protected override IScaleBasedRangeControlClientOptions GetOptionsCore() {
			if (optionsInstance == null)
				optionsInstance = new SchedulerStorageRangeControlClientOptions();
			return optionsInstance;
		}
		protected override Dictionary<TimeInterval, AppointmentBaseCollection> GetFilteredAppointmentByIntervals(TimeIntervalCollection intervals) {
			if (SchedulerStorage == null)
				return new Dictionary<TimeInterval, AppointmentBaseCollection>();
			ResourceBaseCollection resources = SchedulerStorage.GetFilteredResources(true);
			TimeZoneHelper tzEngine = new TimeZoneHelper(SchedulerStorage.TimeZoneEngine);
			AppointmentResourcesMatchFilter filter = CreateAppointmentResourcesMatchFilter(resources, tzEngine);
			return SchedulerStorage.GetFilteredAppointmentByIntervals(intervals, filter, this);
		}
		protected virtual AppointmentResourcesMatchFilter CreateAppointmentResourcesMatchFilter(ResourceBaseCollection resources, TimeZoneHelper tzEngine) {
			if (resources == null)
				Exceptions.ThrowArgumentNullException("resources");
			AppointmentResourcesMatchFilter result = new AppointmentResourcesMatchFilter();
			result.AppointmentExternalFilter = Storage.CreateAppointmentExternalFilterPredicate();
			result.Resources = resources;
			result.TimeZoneHelper = tzEngine;
			return result;
		}
		protected override DateTime GetSelectedRangeStart() {
			return SelectedInterval != null ? SelectedInterval.Start : DateTime.MinValue;
		}
		protected override DateTime GetSelectedRangeEnd() {
			return SelectedInterval != null ? SelectedInterval.End : DateTime.MinValue;
		}
		protected override IDataItemThumbnail CreateDataItemThumbnailItem(Appointment apt) {
			Color color = CalculateAppointmentColor(apt);
			return new DataItemThumbnail(color, apt);
		}
		protected virtual Color CalculateAppointmentColor(Appointment appointment) {
			return SchedulerStorage.Appointments.Labels.GetById(appointment.LabelKey).GetColor();
		}
		protected override void OnSelectedRangeChangedCore(DateTime rangeMinimum, DateTime rangeMaximum) {
			SelectedInterval = new TimeInterval(rangeMinimum, rangeMaximum);
		}
	}
}
