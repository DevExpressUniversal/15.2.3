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
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
namespace DevExpress.XtraScheduler.Reporting {
	#region TimeIntervalsValidationEventHandler
	public delegate void TimeIntervalsValidationEventHandler(object sender, TimeIntervalsValidationEventArgs e);
	#endregion
	#region TimeIntervalsValidationEventArgs
	public class TimeIntervalsValidationEventArgs : EventArgs {
		TimeIntervalCollection intervals;
		public TimeIntervalsValidationEventArgs(TimeIntervalCollection intervals) {
			if (intervals == null)
				Exceptions.ThrowArgumentNullException("intervals");
			this.intervals = intervals;
		}
		public TimeIntervalCollection Intervals { get { return intervals; } }
	}
	#endregion
	#region WorkTimeValidationEventHandler
	public delegate void WorkTimeValidationEventHandler(object sender, WorkTimeValidationEventArgs e);
	#endregion
	#region WorkTimeValidationEventArgs
	public class WorkTimeValidationEventArgs : EventArgs {
		TimeOfDayIntervalCollection workTimes;
		TimeInterval timeInterval;
		Resource resource;
		public WorkTimeValidationEventArgs(TimeOfDayIntervalCollection workTimes, TimeInterval timeInterval, Resource resource) {
			Guard.ArgumentNotNull(workTimes, "workTimes");
			Guard.ArgumentNotNull(timeInterval, "timeInterval");
			Guard.ArgumentNotNull(resource, "resource");
			this.workTimes = workTimes;
			this.timeInterval = timeInterval;
			this.resource = resource;
		}
		public TimeInterval TimeInterval { get { return timeInterval; } }
		public Resource Resource { get { return resource; } }
		public TimeOfDayInterval WorkTime {
			get {
				if (workTimes.Count <= 0)
					return TimeOfDayInterval.Empty;
				else
					return workTimes[0];
			}
			set {
				workTimes.Clear();
				if (value != null)
					workTimes.Add(value);
			}
		}
		public TimeOfDayIntervalCollection WorkTimes { get { return workTimes; } }
	}
	#endregion
	#region ResourcesValidationEventHandler
	public delegate void ResourcesValidationEventHandler(object sender, ResourcesValidationEventArgs e);
	#endregion
	#region ResourcesValidationEventArgs
	public class ResourcesValidationEventArgs : EventArgs {
		ResourceBaseCollection resources;
		public ResourcesValidationEventArgs(ResourceBaseCollection resources) {
			if (resources == null)
				Exceptions.ThrowArgumentNullException("resources");
			this.resources = resources;
		}
		public ResourceBaseCollection Resources { get { return resources; } }
	}
	#endregion
	#region AppointmentsValidationEventHandler
	public delegate void AppointmentsValidationEventHandler(object sender, AppointmentsValidationEventArgs e);
	#endregion
	#region AppointmentsValidationEventArgs
	public class AppointmentsValidationEventArgs : EventArgs {
		AppointmentBaseCollection appointments;
		TimeInterval interval;
		ResourceBaseCollection resources;
		public AppointmentsValidationEventArgs(AppointmentBaseCollection appointments, TimeInterval interval, ResourceBaseCollection resources) {
			if (appointments == null)
				Exceptions.ThrowArgumentNullException("appointments");
			if (interval == null)
				Exceptions.ThrowArgumentNullException("interval");
			if (resources == null)
				Exceptions.ThrowArgumentNullException("resources");
			this.appointments = appointments;
			this.resources = resources;
			this.interval = interval;
		}
		public AppointmentBaseCollection Appointments { get { return appointments; } }
		public TimeInterval Interval { get { return interval; } }
		public ResourceBaseCollection Resources { get { return resources; } }
	}
	#endregion
}
