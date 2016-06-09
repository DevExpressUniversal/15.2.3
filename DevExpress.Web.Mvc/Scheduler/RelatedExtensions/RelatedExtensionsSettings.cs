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
namespace DevExpress.Web.Mvc {
	using DevExpress.Web;
	using DevExpress.XtraScheduler;
	using DevExpress.Web.ASPxScheduler;
	using System.ComponentModel;
	public class TimeZoneEditSettings: SettingsBase {
		public TimeZoneEditSettings() {
		}
		public string SchedulerName { get; set; }
		protected override ImagesBase CreateImages() {
			return null;
		}
		protected override StylesBase CreateStyles() {
			return null;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return null;
		}
	}
	public class AppointmentRecurrenceFormSettings: SettingsBase {
		public AppointmentRecurrenceFormSettings() {
			DayNumber = 1;
			Month = 1;
			Periodicity = 1;
			OccurrenceCount = 1;
			EnableHourlyRecurrence = false;
			EnableScriptSupport = true;
		}
		public ASPxSchedulerImages Images { get { return (ASPxSchedulerImages)ImagesInternal; } }
		public ASPxSchedulerStyles Styles { get { return (ASPxSchedulerStyles)StylesInternal; } }
		public bool EnableScriptSupport { get; set; }
		public WeekDays WeekDays { get; set; }
		public WeekOfMonth WeekOfMonth { get; set; }
		public int DayNumber { get; set; }
		public int Month { get; set; }
		public int OccurrenceCount { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
		public RecurrenceRange RecurrenceRange { get; set; }
		public RecurrenceType RecurrenceType { get; set; }
		public bool IsRecurring { get; set; }
		public bool IsFormRecreated { get; set; }
		public int Periodicity { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool EnableHourlyRecurrence { get; set; }
		protected override ImagesBase CreateImages() {
			return new ASPxSchedulerImages(null);
		}
		protected override StylesBase CreateStyles() {
			return new ASPxSchedulerStyles(null);
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return null;
		}
	}
	public class SchedulerStatusInfoSettings: SettingsBase {
		public SchedulerStatusInfoSettings()
			: base() {
		}
		public int Priority { get; set; }
		public string SchedulerName { get; set; }
		protected override ImagesBase CreateImages() {
			return null;
		}
		protected override StylesBase CreateStyles() {
			return null;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return null;
		}
	}
	public class DateNavigatorSettings: SettingsBase {
		DateNavigatorProperties properties;
		public DateNavigatorSettings()
			: base() {
			this.properties = new DateNavigatorProperties();
		}
		public DateNavigatorProperties Properties { get { return properties; } }
		public string SchedulerName { get; set; }
		protected override ImagesBase CreateImages() {
			return null;
		}
		protected override StylesBase CreateStyles() {
			return null;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return null;
		}
	}
}
