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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DevExpress.Utils;
using DevExpress.Compatibility.System;
namespace DevExpress.Schedule {
	public enum WorkDayType { WeekDay, Holiday, ExactWorkDay }
	public enum DateCheckResult { Unknown, Holiday, WorkDay }
	#region WorkDay (abstract class)
	public abstract class WorkDay : ICloneable {
		public abstract WorkDayType Type { get; }
		public abstract bool IsWorkDay(DateTime date);
		public abstract DateCheckResult CheckDate(DateTime date);
		#region ICloneable implementation
		object ICloneable.Clone() {
			return CloneCore();
		}
		protected abstract object CloneCore();
		#endregion
	}
	#endregion
	#region KnownDateDay
	public abstract class KnownDateDay : WorkDay {
		readonly DateTime date;
		string displayName;
		protected KnownDateDay(DateTime date, string displayName) {
			this.date = date;
			this.displayName = displayName;
		}
		public string DisplayName { get { return displayName; } set { displayName = value; } }
		public DateTime Date { get { return date; } }
		public override bool Equals(object obj) {
			KnownDateDay val = obj as KnownDateDay;
			if (val == null)
				return false;
			return Date == val.Date && DisplayName == val.DisplayName;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	#endregion
	#region Holiday
	public class Holiday : KnownDateDay {
		string location = string.Empty;
		public Holiday(DateTime date, string displayName, string location)
			: base(date, displayName) {
			this.location = location;
		}
		public Holiday(DateTime date, string displayName)
			: base(date, displayName) {
		}
		public override WorkDayType Type { get { return WorkDayType.Holiday; } }
		public string Location { get { return location; } set { location = value; } }
		public override bool IsWorkDay(DateTime date) { return CheckDate(date) == DateCheckResult.Unknown; }
		public override DateCheckResult CheckDate(DateTime date) {
			if (Date.Date == date.Date) {
				return DateCheckResult.Holiday;
			}
			return DateCheckResult.Unknown;
		}
		#region ICloneable implementation
		protected override object CloneCore() {
			return new Holiday(Date, DisplayName, Location);
		}
		public Holiday Clone() {
			return (Holiday)CloneCore();
		}
		#endregion
		public override bool Equals(object obj) {
			Holiday val = obj as Holiday;
			if (val == null)
				return false;
			return Location == val.Location && base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	#endregion
	#region ExactWorkDay
	public class ExactWorkDay : KnownDateDay {
		public ExactWorkDay(DateTime date, string displayName)
			: base(date, displayName) {
		}
		public override WorkDayType Type { get { return WorkDayType.ExactWorkDay; } }
		public override bool IsWorkDay(DateTime date) { return CheckDate(date) == DateCheckResult.WorkDay; }
		public override DateCheckResult CheckDate(DateTime date) {
			if (Date.Date == date.Date) {
				return DateCheckResult.WorkDay;
			}
			return DateCheckResult.Unknown;
		}
		#region ICloneable implementation
		protected override object CloneCore() {
			return new ExactWorkDay(Date, DisplayName);
		}
		public ExactWorkDay Clone() {
			return (ExactWorkDay)CloneCore();
		}
		#endregion
		public override bool Equals(object obj) {
			ExactWorkDay val = obj as ExactWorkDay;
			if (val == null)
				return false;
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	#endregion
	#region HolidayBaseCollection
	public class HolidayBaseCollection : DXCollection<Holiday> {
	}
	#endregion
	#region OutlookHolidaysLoader
	public class OutlookHolidaysLoader {
		#region CalendarTypes consts
		internal const int CAL_GREGORIAN = 1;
		internal const int CAL_GREGORIAN_ARABIC = 10;
		internal const int CAL_GREGORIAN_ME_FRENCH = 9;
		internal const int CAL_GREGORIAN_US = 2;
		internal const int CAL_GREGORIAN_XLIT_ENGLISH = 11;
		internal const int CAL_GREGORIAN_XLIT_FRENCH = 12;
		internal const int CAL_HEBREW = 8;
		internal const int CAL_HIJRI = 6;
		internal const int CAL_JAPAN = 3;
		internal const int CAL_JULIAN = 13;
		internal const int CAL_KOREA = 5;
		internal const int CAL_TAIWAN = 4;
		internal const int CAL_THAI = 7;
		#endregion
		internal const int DefaultCalendarType = CAL_GREGORIAN;
		Dictionary<int, System.Globalization.Calendar> calendarHash;
		public OutlookHolidaysLoader() {
			this.calendarHash = new Dictionary<int, System.Globalization.Calendar>();
		}
		~OutlookHolidaysLoader() {
			calendarHash.Clear();
			calendarHash = null;
		}
		protected virtual Char HolidayLineSeparator { get { return ','; } }
		protected virtual Char HolidayDateSeparator { get { return '/'; } }
		protected internal System.Globalization.Calendar QueryCalendar(int calendarType) {
			System.Globalization.Calendar cal;
			if (!calendarHash.TryGetValue(calendarType, out cal)) {
				cal = CreateCalendarInstance(calendarType);
				if (cal != null)
					calendarHash.Add(calendarType, cal);
			}
			return cal;
		}
		#region CreateCalendarInstance
		static System.Globalization.Calendar CreateCalendarInstance(int calendarType) {
			switch (calendarType) {
				case CAL_GREGORIAN:
					return new GregorianCalendar();
				case CAL_GREGORIAN_ARABIC:
					return new GregorianCalendar(GregorianCalendarTypes.Arabic);
				case CAL_GREGORIAN_ME_FRENCH:
					return new GregorianCalendar(GregorianCalendarTypes.MiddleEastFrench);
				case CAL_GREGORIAN_US:
					return new GregorianCalendar(GregorianCalendarTypes.USEnglish);
				case CAL_GREGORIAN_XLIT_ENGLISH:
					return new GregorianCalendar(GregorianCalendarTypes.TransliteratedEnglish);
				case CAL_GREGORIAN_XLIT_FRENCH:
					return new GregorianCalendar(GregorianCalendarTypes.TransliteratedFrench);
				case CAL_HEBREW:
					return new HebrewCalendar();
				case CAL_HIJRI:
					return new HijriCalendar();
				case CAL_JAPAN:
					return new JapaneseCalendar();
#if !SL
				case CAL_JULIAN:
					return new JulianCalendar();
#endif
				case CAL_KOREA:
					return new KoreanCalendar();
				case CAL_TAIWAN:
					return new TaiwanCalendar();
				case CAL_THAI:
					return new ThaiBuddhistCalendar();
				default:
					return null;
			}
		}
		#endregion
		public string[] ExtractLocations(string fileName) {
			if (!File.Exists(fileName))
				return new string[0];
			using (FileStream fs = File.OpenRead(fileName)) {
				return ExtractLocations(fs);
			}
		}
		public string[] ExtractLocations(Stream stream) {
			using (StreamReader sr = new StreamReader(stream)) {
				return ReadLocations(sr).ToArray();
			}
		}
		protected List<string> ReadLocations(StreamReader sr) {
			List<string> result = new List<string>();
			string line = null;
			while ((line = sr.ReadLine()) != null) {
				if (IsLocation(line)) result.Add(ExtractLocationName(line));
			}
			return result;
		}
		public HolidayBaseCollection FromFile(string fileName) {
			return FromFile(fileName, new string[0]);
		}
		public HolidayBaseCollection FromFile(string fileName, string[] locations) {
			return FromFile(fileName, null, locations);
		}
		public HolidayBaseCollection FromFile(string fileName, Encoding encoding, string[] locations) {
			if (!File.Exists(fileName))
				return new HolidayBaseCollection();
			using (FileStream fs = File.OpenRead(fileName)) {
				return FromStream(fs, encoding, locations);
			}
		}
		public HolidayBaseCollection FromStream(Stream stream) {
			return FromStream(stream, new string[0]);
		}
		public HolidayBaseCollection FromStream(Stream stream, string[] locations) {
			return FromStream(stream, null, locations);
		}
		public HolidayBaseCollection FromStream(Stream stream, Encoding encoding, string[] locations) {
			HolidayBaseCollection result = new HolidayBaseCollection();
			if (locations == null || stream == null || stream.Length == 0)
				return result;
			using (StreamReader sr = CreateStreamReader(stream, encoding)) {
				FillHolidays(result, sr, new List<string>(locations));
			}
			return result;
		}
		protected virtual StreamReader CreateStreamReader(Stream stream, Encoding encoding) {
			return encoding != null ? new StreamReader(stream, encoding) : new StreamReader(stream);
		}
		protected void FillHolidays(HolidayBaseCollection target, StreamReader sr, List<string> locations) {
			bool checkLocations = locations.Count > 0;
			bool isValidLocation = checkLocations ? false : true;
			string line = null;
			string currentLocation = string.Empty;
			while ((line = sr.ReadLine()) != null) {
				if (IsLocation(line)) {
					currentLocation = ExtractLocationName(line);
					if (checkLocations)
						isValidLocation = MatchLocation(locations, currentLocation);
				} else {
					if (isValidLocation) {
						Holiday item = CreateHoliday(line, currentLocation);
						if (item != null) target.Add(item);
					}
				}
			}
		}
		protected internal bool MatchLocation(List<string> locations, string currentLocation) {
			int count = locations.Count;
			for (int i = 0; i < count; i++) {
				if (String.Compare(locations[i], currentLocation, StringComparison.CurrentCultureIgnoreCase) == 0)
					return true;
			}
			return false;
		}
		protected internal bool IsLocation(string line) {
			if (String.IsNullOrEmpty(line))
				return false;
			return line.StartsWith("[") && line.IndexOf("]") > 0;
		}
		protected internal string ExtractLocationName(string line) {
			int start = line.IndexOf("[");
			int end = line.LastIndexOf("]");
			return (start >= 0 && end > 0) ? line.Substring(start + 1, end - start - 1) : string.Empty;
		}
		protected internal virtual Holiday CreateHoliday(string holidayInfo, string location) {
			if (String.IsNullOrEmpty(holidayInfo))
				return null;
			if (location == null) location = string.Empty;
			string[] parts = holidayInfo.Split(HolidayLineSeparator);
			int len = parts.Length;
			if (len < 2 || len > 3)
				return null;
			try {
				int type = (len == 3) ? Convert.ToInt32(parts[2]) : DefaultCalendarType;
				System.Globalization.Calendar cal = QueryCalendar(type);
				if (cal == null) return null;
				string[] dateParts = parts[1].Split(HolidayDateSeparator);
				if (dateParts.Length != 3) return null;
				DateTime date = cal.ToDateTime(Convert.ToInt32(dateParts[0]), Convert.ToInt32(dateParts[1]), Convert.ToInt32(dateParts[2]), 0, 0, 0, 0);
				return new Holiday(date, parts[0], location);
			} catch {
				return null;
			}
		}
	}
	#endregion
}
namespace DevExpress.Schedule.Serializing {
	using DevExpress.Utils.Serializing;
	using System.Xml;
	public sealed class HolidaySR {
		public const string XmlCollectionName = "Holidays";
		public const string XmlElementName = "Holiday";
		public const string Date = "Date";
		public const string DisplayName = "DisplayName";
		public const string Location = "Location";
	}
	public class HolidayContextElement : XmlContextItem {
		public HolidayContextElement(Holiday holiday)
			: base(HolidaySR.XmlElementName, holiday, null) {
		}
		protected Holiday Holiday { get { return (Holiday)Value; } }
		public override string ValueToString() {
			return new HolidayXmlPersistenceHelper(Holiday).ToXml();
		}
	}
	public class HolidayXmlPersistenceHelper : XmlPersistenceHelper {
		Holiday holiday;
		public HolidayXmlPersistenceHelper(Holiday holiday) {
			this.holiday = holiday;
		}
		protected override IXmlContext GetXmlContext() {
			XmlContext context = new XmlContext(HolidaySR.XmlElementName);
			context.Attributes.Add(new DateTimeContextAttribute(HolidaySR.Date, holiday.Date, DateTime.MinValue));
			context.Attributes.Add(new StringContextAttribute(HolidaySR.DisplayName, holiday.DisplayName, string.Empty));
			context.Attributes.Add(new StringContextAttribute(HolidaySR.Location, holiday.Location, string.Empty));
			return context;
		}
		public override ObjectXmlLoader CreateObjectXmlLoader(XmlNode root) {
			return new HolidayXmlLoader(root);
		}
		public static Holiday ObjectFromXml(string xml) {
			return ObjectFromXml(GetRootElement(xml));
		}
		public static Holiday ObjectFromXml(XmlNode root) {
			HolidayXmlPersistenceHelper helper = new HolidayXmlPersistenceHelper(null);
			return (Holiday)helper.FromXmlNode(root);
		}
	}
	public class HolidayXmlLoader : ObjectXmlLoader {
		public HolidayXmlLoader(XmlNode root)
			: base(root) {
		}
		public override object ObjectFromXml() {
			DateTime date = ReadAttributeAsDateTime(HolidaySR.Date, DateTime.MinValue);
			string displayName = ReadAttributeAsString(HolidaySR.DisplayName, string.Empty);
			string location = ReadAttributeAsString(HolidaySR.Location, string.Empty);
			return new Holiday(date, displayName, location);
		}
	}
	public class HolidayCollectionContextElement : XmlContextItem {
		public HolidayCollectionContextElement(HolidayBaseCollection holidays)
			: base(HolidaySR.XmlCollectionName, holidays, null) {
		}
		protected HolidayBaseCollection Holidays { get { return (HolidayBaseCollection)Value; } }
		public override string ValueToString() {
			return new HolidayCollectionXmlPersistenceHelper(Holidays).ToXml();
		}
	}
	public class HolidayCollectionXmlPersistenceHelper : CollectionXmlPersistenceHelper {
		public HolidayCollectionXmlPersistenceHelper(HolidayBaseCollection holidays)
			: base(holidays) {
		}
		protected override string XmlCollectionName { get { return HolidaySR.XmlCollectionName; } }
		public static HolidayBaseCollection ObjectFromXml(string xml) {
			return ObjectFromXml(GetRootElement(xml));
		}
		public static HolidayBaseCollection ObjectFromXml(XmlNode root) {
			HolidayCollectionXmlPersistenceHelper helper = new HolidayCollectionXmlPersistenceHelper(new HolidayBaseCollection());
			return (HolidayBaseCollection)helper.FromXmlNode(root);
		}
		protected override ObjectCollectionXmlLoader CreateObjectCollectionXmlLoader(XmlNode root) {
			return new HolidayCollectionXmlLoader(root, (HolidayBaseCollection)Collection);
		}
		protected override IXmlContextItem CreateXmlContextItem(object obj) {
			return new HolidayContextElement((Holiday)obj);
		}
	}
	public class HolidayCollectionXmlLoader : ObjectCollectionXmlLoader {
		HolidayBaseCollection holidays;
		public HolidayCollectionXmlLoader(XmlNode root, HolidayBaseCollection holidays)
			: base(root) {
			this.holidays = holidays;
		}
		protected override ICollection Collection { get { return holidays; } }
		protected override string XmlCollectionName { get { return HolidaySR.XmlCollectionName; } }
		protected override object LoadObject(XmlNode root) {
			return HolidayXmlPersistenceHelper.ObjectFromXml(root);
		}
		protected override void AddObjectToCollection(object obj) {
			holidays.Add((Holiday)obj);
		}
		protected override void ClearCollectionObjects() {
			holidays.Clear();
		}
	}
}
