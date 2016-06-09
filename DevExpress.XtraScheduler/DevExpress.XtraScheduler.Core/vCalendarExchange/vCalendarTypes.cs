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
using DevExpress.XtraScheduler;
using DevExpress.Utils;
namespace DevExpress.XtraScheduler.VCalendar {
	public enum VCalendarEncoding { Default, Base64, QuotedPrintable, Bit8 }
	public enum VCalendarSpecification { Unknown = 0, vCalendar10 = 1, iCalendar = 2 };
	public abstract class VObjectBase {
		public const VCalendarSpecification DefaultCalendarSpecification = VCalendarSpecification.vCalendar10;
		VCalendarSpecification specification = DefaultCalendarSpecification;
		public VCalendarSpecification Specification {
			get {
				return specification;
			}
			set {
				if (value == VCalendarSpecification.Unknown)
					return;
				this.specification = value;
			}
		}
		public abstract VObjectEncoder GetEncoder();
	}
	public class VCalendarObject : VObjectBase {
		string productId;
		readonly List<VEvent> eventList;
		string version;
		public VCalendarObject()
			: this(VCalendarConsts.DefaultCalendarProductID) {
		}
		public VCalendarObject(string productId) {
			this.eventList = new List<VEvent>();
			this.productId = productId;
			this.version = VCalendarConsts.DefaultCalendarVersion;
		}
		public List<VEvent> EventList { get { return eventList; } }
		public string ProductId { get { return productId; } set { productId = value; } }
		public string Version { get { return version; } set { version = value; } }
		public override VObjectEncoder GetEncoder() {
			return new VCalendarEncoder(this);
		}
	}
	public class VEvent : VObjectBase {
		DateTime dtStart;
		DateTime dtEnd;
		string summary;
		string location;
		string description;
		int timeTransparency = 0;
		int priority = 3;
		int rNum = -1;
		VRecurrenceRuleCollection recurrenceRules;
		DateTimeCollection recurrenceDateTimes;
		VRecurrenceRuleCollection exceptionRules;
		DateTimeCollection exceptionDateTimes;
		VEventExtensionCollection extensions;
		public VEvent() {
		}
		public DateTime DTStart { get { return dtStart; } set { dtStart = value; } }
		public DateTime DTEnd { get { return dtEnd; } set { dtEnd = value; } }
		public string Summary { get { return summary; } set { summary = value; } }
		public string Location { get { return location; } set { location = value; } }
		public string Description { get { return description; } set { description = value; } }
		public int TimeTransparency { get { return timeTransparency; } set { timeTransparency = value; } }
		public int Priority { get { return priority; } set { priority = value; } }
		public int NumberRecurrences { get { return rNum; } set { rNum = value; } }
		public VRecurrenceRuleCollection RecurrenceRules {
			get {
				if (recurrenceRules == null)
					recurrenceRules = new VRecurrenceRuleCollection();
				return recurrenceRules;
			}
		}
		public DateTimeCollection RecurrenceDateTimes {
			get {
				if (recurrenceDateTimes == null)
					recurrenceDateTimes = new DateTimeCollection();
				return recurrenceDateTimes;
			}
		}
		public VRecurrenceRuleCollection ExceptionRules {
			get {
				if (exceptionRules == null)
					exceptionRules = new VRecurrenceRuleCollection();
				return exceptionRules;
			}
		}
		public DateTimeCollection ExceptionDateTimes {
			get {
				if (exceptionDateTimes == null)
					exceptionDateTimes = new DateTimeCollection();
				return exceptionDateTimes;
			}
		}
		public VEventExtensionCollection Extensions {
			get {
				if (extensions == null)
					extensions = new VEventExtensionCollection();
				return extensions;
			}
		}
		public override VObjectEncoder GetEncoder() {
			return new VEventEncoder(this);
		}
		public virtual void FromAppointment(Appointment apt) {
			if (apt == null)
				return;
			this.dtStart = apt.Start;
			this.dtEnd = apt.End;
			this.summary = apt.Subject;
			this.description = apt.Description;
			this.location = apt.Location;
			this.timeTransparency = apt.AllDay ? 1 : 0;
			if (apt.Type == AppointmentType.Pattern) {
				VRecurrenceConvert conv = new VRecurrenceConvert();
				VRecurrenceRule rule = conv.FromRecurrenceInfo(apt.RecurrenceInfo);
				if (rule != null)
					RecurrenceRules.Add(rule);
			}
		}
		internal AppointmentType GetAppointmentType() {
			return RecurrenceRules.Count > 0 ? AppointmentType.Pattern : AppointmentType.Normal;
		}
		public virtual void ToAppointment(Appointment apt) {
			if (apt == null) return;
			apt.Start = DTStart;
			if (DTEnd != DateTime.MinValue)
				apt.End = DTEnd;
			apt.Subject = Summary;
			apt.Location = Location;
			apt.Description = Description;
			apt.AllDay = TimeTransparency == 1; 
			if (RecurrenceRules.Count > 0) {
				VRecurrenceConvert conv = new VRecurrenceConvert();
				conv.AssignRecurrenceInfo(RecurrenceRules[0], apt.RecurrenceInfo);
			}
		}
	}
	public class VEventExtension {
		string name = string.Empty;
		string val = string.Empty;
		VCalendarEncoding encoding = VCalendarEncoding.Default;
		readonly VCalendarParameterCollection parameters;
		public VEventExtension(string name, string val) {
			this.name = name;
			this.val = val;
			parameters = new VCalendarParameterCollection();
		}
		public string Name { get { return name; } }
		public string Value { get { return val; } }
		public VCalendarEncoding Encoding { get { return encoding; } set { encoding = value; } }
		public VCalendarParameterCollection Parameters { get { return parameters; } }
	}
	public class VEventExtensionCollection : DXNamedItemCollection<VEventExtension> {
		public int Add(string name, string val) {
			VEventExtension item = new VEventExtension(name, val);
			return Add(item);
		}
		protected override string GetItemName(VEventExtension item) {
			return item.Name;
		}
	}
}
