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
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraScheduler.Reporting {
	#region Provider interfaces
	public interface ISchedulerResourceProvider {
		ResourceBaseCollection GetResources();
		int ResourceCount { get; }
		ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> GetResourceColorSchemas();
		SchedulerGroupType GetGroupType();
	}
	public interface ISchedulerAppointmentProvider {
		AppointmentBaseCollection GetAppointments(TimeInterval timeInterval, ResourceBaseCollection resources);
		Color GetLabelColor(object labelId);
		IAppointmentStatus GetStatus(object statusId);
	}
	public interface ISchedulerTimeIntervalProvider {
		TimeIntervalCollection GetTimeIntervals();
		TimeOfDayIntervalCollection GetWorkTime(TimeInterval interval, Resource resource);
		WorkDaysCollection GetWorkDays();
		DayOfWeek GetFirstDayOfWeek();
		string GetClientTimeZoneId();
		TimeInterval GetAdapterInterval();
	}
	#endregion
	#region InnerTimeIntervalProvider
	public class InnerTimeIntervalProvider : ISchedulerTimeIntervalProvider {
		SchedulerPrintAdapter adapter;
		public InnerTimeIntervalProvider(SchedulerPrintAdapter adapter) {
			if (adapter == null)
				Exceptions.ThrowArgumentNullException("adapter");
			this.adapter = adapter;
		}
		protected SchedulerPrintAdapter Adapter { get { return adapter; } }
		#region ISchedulerTimeIntervalProvider Members
		public TimeIntervalCollection GetTimeIntervals() {
			return GetTimeIntervalsCore();
		}
		public TimeOfDayIntervalCollection GetWorkTime(TimeInterval interval, Resource resource) {
			return GetWorkTimeCore(interval, resource);
		}
		public WorkDaysCollection GetWorkDays() {
			return SchedulerPrintAdapter.GetDefaultWorkDays();
		}
		public string GetClientTimeZoneId() { return Adapter.ClientTimeZoneId; }
		public DayOfWeek GetFirstDayOfWeek() { return Adapter.ConvertFirstDayOfWeek(); }
		#endregion
		protected virtual TimeIntervalCollection GetTimeIntervalsCore() {
			return new TimeIntervalCollection();
		}
		protected virtual TimeOfDayIntervalCollection GetWorkTimeCore(TimeInterval interval, Resource resource) {
			TimeOfDayIntervalCollection result = new TimeOfDayIntervalCollection();
			result.Add(SchedulerPrintAdapter.DefaultWorkTime.Clone());
			return result;
		}
		TimeInterval ISchedulerTimeIntervalProvider.GetAdapterInterval() {			
			return Adapter != null ? Adapter.GetTimeIntervals().Interval : TimeInterval.Empty;
		}
	}
	#endregion
	#region InnerResourceProvider
	public class InnerResourceProvider : ISchedulerResourceProvider {
		public int ResourceCount { get { return 0; } }
		public ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> GetResourceColorSchemas() {
			return new SchedulerColorSchemaCollectionBase<SchedulerColorSchemaBase>();
		}
		public ResourceBaseCollection GetResources() {
			return new ResourceBaseCollection();
		}
		public SchedulerGroupType GetGroupType() {
			return SchedulerGroupType.Resource; 
		}
	}
	#endregion
	#region InnerAppointmentProvider
	public class InnerAppointmentProvider : ISchedulerAppointmentProvider {
		public AppointmentBaseCollection GetAppointments(TimeInterval timeInterval, ResourceBaseCollection resources) {
			return new AppointmentBaseCollection();
		}
		public Color GetLabelColor(object labelId) {
			return DXColor.White;
		}
		public IAppointmentStatus GetStatus(object statusId) {
			return null; 
		}
	}
	#endregion
}
