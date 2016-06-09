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
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler {
	public class TimeZoneHelper : TimeZoneEngineBase {
		TimeZoneInfo clientTimeZone;
		internal TimeZoneHelper(TimeZoneEngine storageTimeZoneEngine) {
			StorageTimeZoneEngine = storageTimeZoneEngine;
		}
		public TimeZoneHelper(string clientTimeZoneId) {
			ClientTimeZone = TimeZoneInfo.FindSystemTimeZoneById(clientTimeZoneId);
		}
		#region Properties
		internal TimeZoneEngine StorageTimeZoneEngine { get; set; }
		#region OperationTimeZone
		public override TimeZoneInfo OperationTimeZone {
			get {
				if (StorageTimeZoneEngine == null)
					return base.OperationTimeZone;
				else
					return StorageTimeZoneEngine.OperationTimeZone;
			}
			internal set {
				if (StorageTimeZoneEngine == null)
					base.OperationTimeZone = value;
				else
					StorageTimeZoneEngine.OperationTimeZone = value;
			}
		}
		#endregion
		public TimeZoneInfo ClientTimeZone {
			get {
				return clientTimeZone ?? TimeZoneEngine.Local;
			}
			internal set {
				if (clientTimeZone == value)
					return;
				clientTimeZone = value;
				RaiseClientTimeZoneChanged();
			}
		}
		#endregion
		#region Events
		public event EventHandler ClientTimeZoneChanged;
		void RaiseClientTimeZoneChanged() {
			if (ClientTimeZoneChanged == null)
				return;
			ClientTimeZoneChanged(this, EventArgs.Empty);
		}
		#endregion
		public override DateTime ToOperationTime(DateTime dateTime, string tzId) {
			if (StorageTimeZoneEngine == null)
				return base.ToOperationTime(dateTime, tzId);
			return StorageTimeZoneEngine.ToOperationTime(dateTime, tzId);
		}
		public override DateTime FromOperationTime(DateTime dateTime, string tzId) {
			if (StorageTimeZoneEngine == null)
				return base.FromOperationTime(dateTime, tzId);
			dateTime = NormalizeDate(dateTime);
			return StorageTimeZoneEngine.FromOperationTime(dateTime, tzId);
		}
		#region FromClientTime
		public virtual DateTime FromClientTime(DateTime dateTime) {
			if (Object.Equals(ClientTimeZone, OperationTimeZone))
				return dateTime;
			DateTime dateToConvert = ValidateDateTime(dateTime, ClientTimeZone);
			return TimeZoneInfo.ConvertTime(dateToConvert, ClientTimeZone, OperationTimeZone);
		}		
		public DateTime FromClientTime(DateTime dateTime, string tartgetTzId) {
			TimeZoneInfo targetTimeZone = (String.IsNullOrEmpty(tartgetTzId)) ? OperationTimeZone : TimeZoneInfo.FindSystemTimeZoneById(tartgetTzId);
			dateTime = ValidateDateTime(dateTime, ClientTimeZone);			
			return TimeZoneInfo.ConvertTime(DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified), ClientTimeZone, targetTimeZone);
		}
		public virtual TimeInterval FromClientTime(TimeInterval interval) {
			interval = ValidateInterval(interval, ClientTimeZone);
			return new TimeInterval(FromClientTime(interval.Start), FromClientTime(interval.End));
		}
		public virtual TimeInterval FromClientTime(TimeInterval interval, bool treatAllDayAsFloatInterval) {
			if (treatAllDayAsFloatInterval && interval.AllDay)
				return interval;
			return FromClientTime(interval);
		}		
		public TimeInterval FromClientTime(TimeInterval sourceTimeInterval, string tartgetTzId, bool treatAllDayAsFloatInterval) {
			if (treatAllDayAsFloatInterval && sourceTimeInterval.AllDay)
				return sourceTimeInterval;
			return FromClientTime(sourceTimeInterval, tartgetTzId);
		}
		public TimeInterval FromClientTime(TimeInterval sourceTimeInterval, string tartgetTzId) {
			sourceTimeInterval = ValidateInterval(sourceTimeInterval, ClientTimeZone);
			DateTime start = FromClientTime(sourceTimeInterval.Start, tartgetTzId);
			DateTime end = FromClientTime(sourceTimeInterval.End, tartgetTzId);
			return new TimeInterval(start, end);			
		}
		#endregion
		#region ToClientTime
		public DateTime ToClientTime(DateTime sourceDateTime, string sourceTimeZoneId) {
			TimeZoneInfo sourceTz = OperationTimeZone;
			if (!String.IsNullOrEmpty(sourceTimeZoneId))
				sourceTz = TimeZoneInfo.FindSystemTimeZoneById(sourceTimeZoneId);
			sourceDateTime = ValidateDateTime(sourceDateTime, sourceTz);
			if (sourceTz == ClientTimeZone)
				return sourceDateTime;
			return TimeZoneInfo.ConvertTime(DateTime.SpecifyKind(sourceDateTime, DateTimeKind.Unspecified), sourceTz, ClientTimeZone);
		}
		public DateTime ToClientTime(DateTime sourceDateTime, string sourceTimeZoneId, string clientTimeZoneId) {
			TimeZoneInfo sourceTz = OperationTimeZone;
			if (!String.IsNullOrEmpty(sourceTimeZoneId))
				sourceTz = TimeZoneInfo.FindSystemTimeZoneById(sourceTimeZoneId);
			sourceDateTime = ValidateDateTime(sourceDateTime, sourceTz);
			TimeZoneInfo clientTz = ClientTimeZone;
			if (!String.IsNullOrEmpty(clientTimeZoneId))
				clientTz = TimeZoneInfo.FindSystemTimeZoneById(clientTimeZoneId);
			if (sourceTz == clientTz)
				return sourceDateTime;
			return TimeZoneInfo.ConvertTime(DateTime.SpecifyKind(sourceDateTime, DateTimeKind.Unspecified), sourceTz, clientTz);
		}
		public TimeInterval ToClientTime(TimeInterval sourceInterval, string sourceTimeZoneId, bool treatAllDayAsFloatInterval) {
			if (treatAllDayAsFloatInterval && sourceInterval.AllDay)
				return sourceInterval;
			return ToClientTime(sourceInterval, sourceTimeZoneId);
		}
		public TimeInterval ToClientTime(TimeInterval sourceInterval, bool treatAllDayAsFloatInterval) {
			if (treatAllDayAsFloatInterval && sourceInterval.AllDay)
				return sourceInterval;
			return ToClientTime(sourceInterval);
		}
		public TimeInterval ToClientTime(TimeInterval sourceInterval) {
			return ToClientTime(sourceInterval, OperationTimeZone.Id);
		}
		public TimeInterval ToClientTime(TimeInterval sourceInterval, string sourceTimeZoneId) {
			DateTime start = NormalizeDate(sourceInterval.Start);
			TimeZoneInfo sourceTz = OperationTimeZone;
			if (!String.IsNullOrEmpty(sourceTimeZoneId))
				sourceTz = TimeZoneInfo.FindSystemTimeZoneById(sourceTimeZoneId);
			if (sourceTz.IsInvalidTime(start))
				return new TimeInterval(FromClientTime(start), sourceInterval.Duration);
			start = ToClientTime(start, sourceTimeZoneId);
			DateTime end = ToClientTime(NormalizeDate(sourceInterval.End), sourceTimeZoneId);
			return new TimeInterval(start, end);
		}
		public DateTime ToClientTime(DateTime dateTime) {
			return ToClientTime(DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified), OperationTimeZone.Id);
		}
		public DateTime ToAppointmentTime(DateTime dateTime, string appointmentTimeZoneId) {
			return ToClientTime(dateTime, string.Empty, appointmentTimeZoneId);
		}
		#endregion        
	}
}
