#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DataAccess.Native;
namespace DevExpress.DashboardCommon {
	public enum YearFormat { Default, Full, Abbreviated }
	public enum QuarterFormat { Default, Numeric, Full }
	public enum MonthFormat { Default, Full, Abbreviated, Numeric }
	public enum DayOfWeekFormat { Default, Full, Abbreviated, Numeric }
	public enum DateTimeFormat { Default, Long, Short, TimeOnly }
	public enum DateFormat { Default, Long, Short }
	public enum HourFormat { Default, Short, Long }
	public enum ExactDateFormat { Year, Quarter, Month, Day, Hour, Minute, Second }
	public class DataItemDateTimeFormat {
		internal const string XmlDateTimeFormat = "DateTimeFormat";
		const string xmlYearFormat = "YearFormat";
		const string xmlQuarterFormat = "QuarterFormat";
		const string xmlMonthFormat = "MonthFormat";
		const string xmlDayOfWeekFormat = "DayOfWeekFormat";
		const string xmlDateFormat = "DateFormat";
		const string xmlDateHourFormat = "DateHourFormat";
		const string xmlDateHourMinuteFormat = "DateHourMinuteFormat";
		const string xmlDateTimeFormat = "DateTimeFormat";
		const string xmlHourFormat = "HourFormat";
		const string xmlExactDateFormat = "ExactDateFormat";
		const YearFormat DefaultYearFormat = YearFormat.Default;
		const QuarterFormat DefaultQuarterFormat = QuarterFormat.Default;
		const MonthFormat DefaultMonthFormat = MonthFormat.Default;
		const DayOfWeekFormat DefaultDayOfWeekFormat = DayOfWeekFormat.Default;
		const DateTimeFormat DefaultDateTimeFormat = DateTimeFormat.Default;
		const DateFormat DefaultDateFormat = DateFormat.Default;
		const HourFormat DefaultHourFormat = HourFormat.Default;
		const ExactDateFormat DefaultExactDateFormat = ExactDateFormat.Day;
		readonly DataItem dataItem;
		readonly Locker locker = new Locker();
		YearFormat yearFormat = DefaultYearFormat;
		QuarterFormat quarterFormat = DefaultQuarterFormat;
		MonthFormat monthFormat = DefaultMonthFormat;
		DayOfWeekFormat dayOfWeekFormat = DefaultDayOfWeekFormat;
		DateFormat dateFormat = DefaultDateFormat;
		DateTimeFormat dateHourFormat = DefaultDateTimeFormat;
		DateTimeFormat dateHourMinuteFormat = DefaultDateTimeFormat;
		DateTimeFormat dateTimeFormat = DefaultDateTimeFormat;
		HourFormat hourFormat = DefaultHourFormat;
		ExactDateFormat exactDateFormat = DefaultExactDateFormat;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataItemDateTimeFormatYearFormat"),
#endif
		DefaultValue(DefaultYearFormat)
		]
		public YearFormat YearFormat {
			get { return yearFormat; }
			set {
				if (value != yearFormat) {
					yearFormat = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataItemDateTimeFormatQuarterFormat"),
#endif
		DefaultValue(DefaultQuarterFormat)
		]
		public QuarterFormat QuarterFormat {
			get { return quarterFormat; }
			set {
				if (value != quarterFormat) {
					quarterFormat = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataItemDateTimeFormatMonthFormat"),
#endif
		DefaultValue(DefaultMonthFormat)
		]
		public MonthFormat MonthFormat {
			get { return monthFormat; }
			set {
				if (value != monthFormat) {
					monthFormat = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataItemDateTimeFormatDayOfWeekFormat"),
#endif
		DefaultValue(DefaultDayOfWeekFormat)
		]
		public DayOfWeekFormat DayOfWeekFormat {
			get { return dayOfWeekFormat; }
			set {
				if (value != dayOfWeekFormat) {
					dayOfWeekFormat = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataItemDateTimeFormatDateFormat"),
#endif
		DefaultValue(DefaultDateFormat)
		]
		public DateFormat DateFormat {
			get { return dateFormat; }
			set {
				if (value != dateFormat) {
					dateFormat = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataItemDateTimeFormatDateHourFormat"),
#endif
		DefaultValue(DefaultDateTimeFormat)
		]
		public DateTimeFormat DateHourFormat {
			get { return dateHourFormat; }
			set {
				if (value != dateHourFormat) {
					dateHourFormat = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataItemDateTimeFormatDateHourMinuteFormat"),
#endif
		DefaultValue(DefaultDateTimeFormat)
		]
		public DateTimeFormat DateHourMinuteFormat {
			get { return dateHourMinuteFormat; }
			set {
				if (value != dateHourMinuteFormat) {
					dateHourMinuteFormat = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataItemDateTimeFormatDateTimeFormat"),
#endif
		DefaultValue(DefaultDateTimeFormat)
		]
		public DateTimeFormat DateTimeFormat {
			get { return dateTimeFormat; }
			set {
				if (value != dateTimeFormat) {
					dateTimeFormat = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataItemDateTimeFormatHourFormat"),
#endif
		DefaultValue(DefaultHourFormat)
		]
		public HourFormat HourFormat { 
			get { return hourFormat; }
			set {
				if (value != hourFormat) {
					hourFormat = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataItemDateTimeFormatExactDateFormat"),
#endif
		DefaultValue(DefaultExactDateFormat)
		]
		public ExactDateFormat ExactDateFormat {
			get { return exactDateFormat; }
			set {
				if(value != exactDateFormat) {
					exactDateFormat = value;
					OnChanged();
				}
			}
		}
		internal DateTimeGroupInterval GroupInterval {
			get {
				Dimension dimension = dataItem as Dimension;
				return dimension == null ? DateTimeGroupInterval.DayMonthYear : dimension.DateTimeGroupInterval;
			}
		}
		internal DataItemDateTimeFormat(DataItem dataItem) {
			this.dataItem = dataItem;
		}
		public void BeginUpdate() {
			locker.Lock();
		}
		public void EndUpdate() {
			locker.Unlock();
			OnChanged();
		}
		internal XElement SaveToXml() {
			XElement element = new XElement(XmlDateTimeFormat);
			if(YearFormat != DefaultYearFormat)
				element.Add(new XAttribute(xmlYearFormat, yearFormat));
			if(QuarterFormat != DefaultQuarterFormat)
				element.Add(new XAttribute(xmlQuarterFormat, quarterFormat));
			if(MonthFormat != DefaultMonthFormat)
				element.Add(new XAttribute(xmlMonthFormat, monthFormat));
			if(DayOfWeekFormat != DefaultDayOfWeekFormat)
				element.Add(new XAttribute(xmlDayOfWeekFormat, dayOfWeekFormat));
			if(DateFormat != DefaultDateFormat)
				element.Add(new XAttribute(xmlDateFormat, dateFormat));
			if(DateHourFormat != DefaultDateTimeFormat)
				element.Add(new XAttribute(xmlDateHourFormat, dateHourFormat));
			if(DateHourMinuteFormat != DefaultDateTimeFormat)
				element.Add(new XAttribute(xmlDateHourMinuteFormat, dateHourMinuteFormat));
			if(DateTimeFormat != DefaultDateTimeFormat)
				element.Add(new XAttribute(xmlDateTimeFormat, dateTimeFormat));
			if(HourFormat != DefaultHourFormat)
				element.Add(new XAttribute(xmlHourFormat, hourFormat));
			if(ExactDateFormat != DefaultExactDateFormat)
				element.Add(new XAttribute(xmlExactDateFormat, exactDateFormat));
			return element;
		}
		internal void LoadFromXml(XElement element) {
			string attribute = XmlHelper.GetAttributeValue(element, xmlYearFormat);
			if (!String.IsNullOrEmpty(attribute))
				yearFormat = XmlHelper.FromString<YearFormat>(attribute);
			attribute = XmlHelper.GetAttributeValue(element, xmlQuarterFormat);
			if (!String.IsNullOrEmpty(attribute))
				quarterFormat = XmlHelper.FromString<QuarterFormat>(attribute);
			attribute = XmlHelper.GetAttributeValue(element, xmlMonthFormat);
			if (!String.IsNullOrEmpty(attribute))
				monthFormat = XmlHelper.FromString<MonthFormat>(attribute);
			attribute = XmlHelper.GetAttributeValue(element, xmlDayOfWeekFormat);
			if (!String.IsNullOrEmpty(attribute))
				dayOfWeekFormat = XmlHelper.FromString<DayOfWeekFormat>(attribute);
			attribute = XmlHelper.GetAttributeValue(element, xmlDateFormat);
			if (!String.IsNullOrEmpty(attribute))
				dateFormat = XmlHelper.FromString<DateFormat>(attribute);
			attribute = XmlHelper.GetAttributeValue(element, xmlDateHourFormat);
			if (!String.IsNullOrEmpty(attribute))
				dateHourFormat = XmlHelper.FromString<DateTimeFormat>(attribute);
			attribute = XmlHelper.GetAttributeValue(element, xmlDateHourMinuteFormat);
			if (!String.IsNullOrEmpty(attribute))
				dateHourMinuteFormat = XmlHelper.FromString<DateTimeFormat>(attribute);
			attribute = XmlHelper.GetAttributeValue(element, xmlDateTimeFormat);
			if (!String.IsNullOrEmpty(attribute))
				dateTimeFormat = XmlHelper.FromString<DateTimeFormat>(attribute);
			attribute = XmlHelper.GetAttributeValue(element, xmlHourFormat);
			if (!String.IsNullOrEmpty(attribute))
				hourFormat = XmlHelper.FromString<HourFormat>(attribute);
			attribute = XmlHelper.GetAttributeValue(element, xmlExactDateFormat);
			if(!string.IsNullOrEmpty(attribute))
				exactDateFormat = XmlHelper.EnumFromString<ExactDateFormat>(attribute);
		}
		internal bool ShouldSerialize() {
			return
				YearFormat != DefaultYearFormat ||
				QuarterFormat != DefaultQuarterFormat ||
				MonthFormat != DefaultMonthFormat ||
				DayOfWeekFormat != DefaultDayOfWeekFormat ||
				DateFormat != DefaultDateFormat ||
				DateHourFormat != DefaultDateTimeFormat ||
				DateHourMinuteFormat != DefaultDateTimeFormat ||
				DateTimeFormat != DefaultDateTimeFormat ||
				HourFormat != DefaultHourFormat ||
				ExactDateFormat != DefaultExactDateFormat;
		}
		void OnChanged() {
			if(dataItem != null && !locker.IsLocked)
				dataItem.OnChanged();
		}
		internal void Assign(DataItemDateTimeFormat format) {
			YearFormat = format.YearFormat;
			QuarterFormat = format.QuarterFormat;
			MonthFormat = format.MonthFormat;
			DayOfWeekFormat = format.DayOfWeekFormat;
			DateFormat = format.DateFormat;
			DateHourFormat = format.DateHourFormat;
			DateHourMinuteFormat = format.DateHourMinuteFormat;
			DateTimeFormat = format.DateTimeFormat;
			HourFormat = format.HourFormat;
			ExactDateFormat = format.ExactDateFormat;
		}
	}
}
