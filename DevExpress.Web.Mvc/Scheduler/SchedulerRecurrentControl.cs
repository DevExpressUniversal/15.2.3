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
using System.Collections;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.ASPxScheduler.Controls;
	using DevExpress.XtraScheduler;
	[ToolboxItem(false)]
	public class MVCxAppointmentRecurrenceControl: AppointmentRecurrenceControl {
		Hashtable clientData;
		protected internal MVCxAppointmentRecurrenceControl()
			: this(null) {
		}
		protected internal MVCxAppointmentRecurrenceControl(Hashtable clientData)
			: base() {
			this.clientData = clientData != null ? clientData : new Hashtable();
		}
		protected new MVCxHourlyRecurrenceControl HourlyRecurrenceControl { get { return (MVCxHourlyRecurrenceControl)base.HourlyRecurrenceControl; } }
		protected new MVCxDailyRecurrenceControl DailyRecurrenceControl { get { return (MVCxDailyRecurrenceControl)base.DailyRecurrenceControl; } }
		protected new MVCxWeeklyRecurrenceControl WeeklyRecurrenceControl { get { return (MVCxWeeklyRecurrenceControl)base.WeeklyRecurrenceControl; } }
		protected new MVCxRecurrenceRangeControl RecurrenceRangeControl { get { return (MVCxRecurrenceRangeControl)base.RecurrenceRangeControl; } }
		protected new MVCxMonthlyRecurrenceControl MonthlyRecurrenceControl { get { return (MVCxMonthlyRecurrenceControl)base.MonthlyRecurrenceControl; } }
		protected new MVCxYearlyRecurrenceControl YearlyRecurrenceControl { get { return (MVCxYearlyRecurrenceControl)base.YearlyRecurrenceControl; } }
		protected Hashtable ClientData { get { return clientData; } }
		protected internal new void EnsureChildControls() {
			base.EnsureChildControls();
		}
		protected override HourlyRecurrenceControl CreateHourlyRecurrenceControl() {
			return new MVCxHourlyRecurrenceControl((Hashtable)this.clientData["Hourly"]);
		}
		protected override DailyRecurrenceControl CreateDailyRecurrenceControl() {
			return new MVCxDailyRecurrenceControl((Hashtable)this.clientData["Daily"]);
		}
		protected override WeeklyRecurrenceControl CreateWeeklyRecurrenceControl() {
			return new MVCxWeeklyRecurrenceControl((Hashtable)this.clientData["Weekly"]);
		}
		protected override RecurrenceRangeControl CreateRecurrenceRangeControl() {
			return new MVCxRecurrenceRangeControl((Hashtable)this.clientData["Range"]);
		}
		protected override MonthlyRecurrenceControl CreateMonthlyRecurrenceControl() {
			return new MVCxMonthlyRecurrenceControl((Hashtable)this.clientData["Monthly"]);
		}
		protected override YearlyRecurrenceControl CreateYearlyRecurrenceControl() {
			return new MVCxYearlyRecurrenceControl((Hashtable)this.clientData["Yearly"]);
		}
		protected override void CreateChildWebControls() {
			base.CreateChildWebControls();
			RecurrenceTypeEdit.Value = (RecurrenceType)this.clientData["Type"];
		}
	}
	[ToolboxItem(false)]
	public class MVCxHourlyRecurrenceControl : HourlyRecurrenceControl { 
		Hashtable clientData;
		protected internal MVCxHourlyRecurrenceControl()
			: this(null) {
		}
		protected internal MVCxHourlyRecurrenceControl(Hashtable clientData)
			: base() {
			this.clientData = clientData != null ? clientData : new Hashtable();
		}
		protected override int CalculateClientPeriodicity() {
			return Convert.ToInt32(this.clientData["Periodicity"]);
		}
	}
	[ToolboxItem(false)]
	public class MVCxDailyRecurrenceControl: DailyRecurrenceControl {
		Hashtable clientData;
		protected internal MVCxDailyRecurrenceControl()
			: this(null) {
		}
		protected internal MVCxDailyRecurrenceControl(Hashtable clientData)
			: base() {
			this.clientData = clientData != null ? clientData : new Hashtable();
		}
		protected override int CalculateClientPeriodicity() {
			return Convert.ToInt32(this.clientData["Periodicity"]);
		}
		protected override WeekDays CalculateClientWeekDays() {
			return (WeekDays)this.clientData["WeekDays"];
		}
	}
	[ToolboxItem(false)]
	public class MVCxWeeklyRecurrenceControl : WeeklyRecurrenceControl{
		Hashtable clientData;
		protected internal MVCxWeeklyRecurrenceControl()
			: this(null) {
		}
		protected internal MVCxWeeklyRecurrenceControl(Hashtable clientData)
			: base() {
			this.clientData = clientData != null ? clientData : new Hashtable();
		}
		protected override int CalculateClientPeriodicity() {
			return Convert.ToInt32(this.clientData["Periodicity"]);
		}
		protected override WeekDays CalculateClientWeekDays() {
			return (WeekDays)this.clientData["WeekDays"];
		}
	}
	[ToolboxItem(false)]
	public class MVCxRecurrenceRangeControl: RecurrenceRangeControl {
		Hashtable clientData;
		protected internal MVCxRecurrenceRangeControl()
			: this(null) {
		}
		protected internal MVCxRecurrenceRangeControl(Hashtable clientData)
			: base() {
			this.clientData = clientData != null ? clientData : new Hashtable();
		}
		protected override RecurrenceRange CalculateClientRange() {
			return (RecurrenceRange)Enum.Parse(typeof(RecurrenceRange), (string)this.clientData["Range"]);
		}
		protected override int CalculateClientOccurrenceCount() {
			return Convert.ToInt32(this.clientData["OccurrenceCount"]);
		}
		protected override DateTime CalculateClientEnd() {
			return Convert.ToDateTime(this.clientData["EndDate"]);
		}
	}
	[ToolboxItem(false)]
	public class MVCxMonthlyRecurrenceControl: MonthlyRecurrenceControl {
		Hashtable clientData;
		protected internal MVCxMonthlyRecurrenceControl()
			: this(null) {
		}
		protected internal MVCxMonthlyRecurrenceControl(Hashtable clientData)
			: base() {
			this.clientData = clientData != null ? clientData : new Hashtable();
		}
		protected override int CalculateClientDayNumber() {
			return Convert.ToInt32(this.clientData["DayNumber"]);
		}
		protected override int CalculateClientPeriodicity() {
			return Convert.ToInt32(this.clientData["Periodicity"]);
		}
		protected override WeekDays CalculateClientWeekDays() {
			return (WeekDays)this.clientData["WeekDays"];
		}
		protected override WeekOfMonth CalculateClientWeekOfMonth() {
			return (WeekOfMonth)this.clientData["WeekOfMonth"];
		}
	}
	[ToolboxItem(false)]
	public class MVCxYearlyRecurrenceControl: YearlyRecurrenceControl {
		Hashtable clientData;
		protected internal MVCxYearlyRecurrenceControl()
			: this(null) {
		}
		protected internal MVCxYearlyRecurrenceControl(Hashtable clientData)
			: base() {
			this.clientData = clientData != null ? clientData : new Hashtable();
		}
		protected override int CalculateClientDayNumber() {
			return Convert.ToInt32(this.clientData["DayNumber"]);
		}
		protected override int CalculateClientMonth() {
			return Convert.ToInt32(this.clientData["Month"]);
		}
		protected override WeekDays CalculateClientWeekDays() {
			return (WeekDays)this.clientData["WeekDays"];
		}
		protected override WeekOfMonth CalculateClientWeekOfMonth() {
			return (WeekOfMonth)this.clientData["WeekOfMonth"];
		}
	}
}
