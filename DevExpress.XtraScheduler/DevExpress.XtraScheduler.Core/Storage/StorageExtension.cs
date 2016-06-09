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

using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Internal {
	public static class StorageBaseExtension {
		public static Dictionary<TimeInterval, AppointmentBaseCollection> GetFilteredAppointmentByIntervals(this ISchedulerStorageBase storage, TimeIntervalCollection searchIntervals, AppointmentResourcesMatchFilter filter, object callerObject) {
			return ((IInternalSchedulerStorageBase)storage).GetFilteredAppointmentByIntervals(searchIntervals, filter, callerObject);
		}
		public static Dictionary<object, Appointment> FindNearestAppointmentInterval(this ISchedulerStorageBase storage, TimeInterval searchInterval, AppointmentResourcesMatchFilter filter, bool findPrevApt) {
			return ((IInternalSchedulerStorageBase)storage).FindNearestAppointmentInterval(searchInterval, filter, findPrevApt);
		}
		public static IPredicate<Appointment> CreateAppointmentExternalFilterPredicate(this ISchedulerStorageBase storage) {
			return ((IInternalSchedulerStorageBase)storage).CreateAppointmentExternalFilterPredicate();
		}
		public static IPredicate<Appointment> CreateAppointmentExternalFilter(this ISchedulerStorageBase storage) {
			return ((IInternalSchedulerStorageBase)storage).CreateAppointmentExternalFilter();
		}
		public static IPredicate<OccurrenceInfo> CreateOccurrenceInfoExternalFilter(this ISchedulerStorageBase storage) {
			return ((IInternalSchedulerStorageBase)storage).CreateOccurrenceInfoExternalFilter();
		}
		public static AppointmentBaseCollection GetNonRecurringAppointments(this ISchedulerStorageBase storage) {
			return ((IInternalSchedulerStorageBase)storage).GetNonRecurringAppointments();
		}
		public static IList<DateTime> GetAppointmentDates(this ISchedulerStorageBase storage, TimeInterval interval, object callerObject, AppointmentResourcesMatchFilter filter) {
			return ((IInternalSchedulerStorageBase)storage).GetAppointmentDates(interval, callerObject, filter);
		}
	}
}
