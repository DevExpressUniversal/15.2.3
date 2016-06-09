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
using System.Globalization;
using DevExpress.XtraScheduler;
using DevExpress.Web.Internal;
using System.Collections;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.Web.ASPxScheduler {
	public static class ClientSideAppointmentFieldNames {
		public const string Id = "appointmentId";
		public const string Start = "start";
		public const string End = "end";
		public const string ResourceIds = "resources";
		public const string AppointmentType = "appointmentType";
		public const string StatusId = "statusIndex";
		public const string LabelId = "labelIndex";
		public const string Subject = "subject";
		public const string Description = "description";
		public const string Location = "location";
		public const string AllDay = "allDay";
		public const string RecurrenceInfo = "recurrenceInfo";
		public const string Interval = "interval";
		public const string PatternType = "pattern";
	}
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	public static class ClientAppointmentPropertiesNames {
		public const string Id = "Id";
		public const string Start = "Start";
		public const string End = "End";
		public const string ResourceIds = "ResourceIds";
		public const string Type = "Type";
		public const string StatusId = "StatusId";
		public const string LabelId = "LabelId";
		public const string Subject = "Subject";
		public const string Description = "Description";
		public const string Location = "Location";
		public const string AllDay = "AllDay";
		public const string RecurrenceInfo = "RecurrenceInfo";
		public const string Pattern = "Pattern";
	}
	public class ClientAppointmentProperties {
		[Obsolete("You should use the 'ClientAppointmentPropertiesNames.Id' instead")]
		public const string IdPropertyName = "appointmentId";
		[Obsolete("You should use the 'ClientAppointmentPropertiesNames.Start' instead")]
		public const string StartPropertyName = "start";
		[Obsolete("You should use the 'ClientAppointmentPropertiesNames.End' instead")]
		public const string EndPropertyName = "end";
		[Obsolete("You should use the 'ClientAppointmentPropertiesNames.ResourceIds' instead")]
		public const string ResourceIdsPropertyName = "resources";
		[Obsolete("You should use the 'ClientAppointmentPropertiesNames.AppointmentType' instead")]
		public const string AppointmentTypePropertyName = "appointmentType";
		[Obsolete("You should use the 'ClientAppointmentPropertiesNames.StatusId' instead")]
		public const string StatusIdPropertyName = "statusIndex";
		[Obsolete("You should use the 'ClientAppointmentPropertiesNames.LabelId' instead")]
		public const string LabelIdPropertyName = "labelIndex";
		[Obsolete("You should use the 'ClientAppointmentPropertiesNames.Subject' instead")]
		public const string SubjectPropertyName = "subject";
		[Obsolete("You should use the 'ClientAppointmentPropertiesNames.Description' instead")]
		public const string DescriptionPropertyName = "description";
		[Obsolete("You should use the 'ClientAppointmentPropertiesNames.Location' instead")]
		public const string LocationPropertyName = "location";
		[Obsolete("You should use the 'ClientAppointmentPropertiesNames.AllDay' instead")]
		public const string AllDayPropertyName = "allDay";
		[Obsolete("You should use the 'ClientAppointmentPropertiesNames.RecurrenceInfo' instead")]
		public const string RecurrenceInfoPropertyName = "recurrenceInfo";
		[Obsolete("You should use the 'ClientAppointmentPropertiesNames.PatternType' instead")]
		public const string PatternTypePropertyName = "pattern";
		public const string IntervalPropertyName = "interval";
		Dictionary<string, Object> properties;
		public ClientAppointmentProperties() {
			this.properties = new Dictionary<string, Object>();
		}
		public Dictionary<string, Object> Properties { get { return properties; } }
		public String Id {
			get { return (String)GetProperty(ClientSideAppointmentFieldNames.Id); }
			set { SetProperty(ClientSideAppointmentFieldNames.Id, value, ClientSideAppointmentFieldNames.Id); }
		}
		public DateTime? Start {
			get {
				return (DateTime?)GetProperty(ClientSideAppointmentFieldNames.Start);
			}
			set { SetProperty(ClientSideAppointmentFieldNames.Start, value, Start); }
		}
		public DateTime? End {
			get { return (DateTime?)GetProperty(ClientSideAppointmentFieldNames.End); }
			set { SetProperty(ClientSideAppointmentFieldNames.End, value, End); }
		}
		public List<String> ResourceIds {
			get { return (List<String>)GetProperty(ClientSideAppointmentFieldNames.ResourceIds); }
			set { SetProperty(ClientSideAppointmentFieldNames.ResourceIds, value, ResourceIds); }
		}
		public AppointmentType? AppointmentType {
			get { return (AppointmentType?)GetProperty(ClientSideAppointmentFieldNames.AppointmentType); }
			set { SetProperty(ClientSideAppointmentFieldNames.AppointmentType, value, AppointmentType); }
		}
		public int? StatusId {
			get { return (int?)GetProperty(ClientSideAppointmentFieldNames.StatusId); }
			set { SetProperty(ClientSideAppointmentFieldNames.StatusId, value, StatusId); }
		}
		public int? LabelId {
			get { return (int?)GetProperty(ClientSideAppointmentFieldNames.LabelId); }
			set { SetProperty(ClientSideAppointmentFieldNames.LabelId, value, LabelId); }
		}
		public String Subject {
			get { return (String)GetProperty(ClientSideAppointmentFieldNames.Subject); }
			set { SetProperty(ClientSideAppointmentFieldNames.Subject, value, Subject); }
		}
		public String Description {
			get { return (String)GetProperty(ClientSideAppointmentFieldNames.Description); }
			set { SetProperty(ClientSideAppointmentFieldNames.Description, value, Description); }
		}
		public String Location {
			get { return (String)GetProperty(ClientSideAppointmentFieldNames.Location); }
			set { SetProperty(ClientSideAppointmentFieldNames.Location, value, Location); }
		}
		public bool? AllDay {
			get { return (bool?)GetProperty(ClientSideAppointmentFieldNames.AllDay); }
			set { SetProperty(ClientSideAppointmentFieldNames.AllDay, value, AllDay); }
		}
		public IRecurrenceInfo RecurrenceInfo {
			get { return (IRecurrenceInfo)GetProperty(ClientSideAppointmentFieldNames.RecurrenceInfo); }
			set { SetProperty(ClientSideAppointmentFieldNames.RecurrenceInfo, value, RecurrenceInfo); }
		}
		internal void SetProperty(string propertyName, object newValue, object oldValue) {
			if(newValue == oldValue)
				return;
			if(newValue == null) {
				Properties.Remove(propertyName);
				return;
			}
			if(Properties.ContainsKey(propertyName))
				Properties[propertyName] = newValue;
			else
				Properties.Add(propertyName, newValue);
		}
		internal Object GetProperty(string propertyName) {
			if(!Properties.ContainsKey(propertyName))
				return null;
			return Properties[propertyName];
		}
	}
	public class ClientRecurrenceInfoProperties {
		public const string TypePropertyName = "type";
		public const string IntervalPropertyName = "interval";
		public const string IdPropertyName = "id";
		public const string MonthPropertyName = "month";
		public const string WeekOfMonthPropertyName = "weekOfMonth";
		public const string WeekDaysPropertyName = "weekDays";
		public const string DayNumberPropertyName = "dayNumber";
		public const string PeriodicityPropertyName = "periodicity";
		public const string OccurrenceCountPropertyName = "occurrenceCount";
		public const string RangePropertyName = "range";
		IRecurrenceInfo recurrenceInfo;
		public ClientRecurrenceInfoProperties(IRecurrenceInfo recurrenceInfo, string timeZoneId) {
			this.recurrenceInfo = recurrenceInfo;
			TimeZoneId = timeZoneId;
		}
		public IRecurrenceInfo RecurrenceInfo { get { return recurrenceInfo; } }
		public string TimeZoneId { get; set; }
		public Dictionary<String, Object> ToPropertiesDictionary(TimeZoneHelper timeZoneEngine) {
			Dictionary<String, Object> result = new Dictionary<string, object>();
			result.Add(TypePropertyName, RecurrenceInfo.Type);
			ClientIntervalProperties clientInterval = new ClientIntervalProperties(RecurrenceInfo.Start, RecurrenceInfo.End);
			result.Add(IntervalPropertyName, clientInterval.ToPropertiesDictionary(timeZoneEngine, TimeZoneId));
			result.Add(IdPropertyName, RecurrenceInfo.Id.ToString());
			result.Add(MonthPropertyName, RecurrenceInfo.Month);
			result.Add(WeekOfMonthPropertyName, RecurrenceInfo.WeekOfMonth);
			result.Add(WeekDaysPropertyName, (int)RecurrenceInfo.WeekDays);
			result.Add(DayNumberPropertyName, RecurrenceInfo.DayNumber);
			result.Add(PeriodicityPropertyName, RecurrenceInfo.Periodicity);
			result.Add(OccurrenceCountPropertyName, RecurrenceInfo.OccurrenceCount);
			result.Add(RangePropertyName, RecurrenceInfo.Range);
			return result;
		}
	}
	public class ClientIntervalProperties {
		public const string StartPropertyName = "start";
		public const string DurationPropertyName = "duration";
		TimeInterval interval;
		public ClientIntervalProperties(TimeInterval interval) {
			this.interval = interval;
		}
		public ClientIntervalProperties(DateTime start, DateTime end) {
			this.interval = new TimeInterval(start, end);
		}
		public TimeInterval Interval { get { return interval; } }
		public Dictionary<String, Object> ToPropertiesDictionary(TimeZoneHelper helper, string timeZoneId) {
			Dictionary<String, Object> result = new Dictionary<string, object>();
			DateTime actualStart = (helper == null) ? Interval.Start : helper.ToClientTime(Interval.Start, timeZoneId);
			result.Add(StartPropertyName, actualStart);
			result.Add(DurationPropertyName, SchedulerWebUtils.ToJavaScriptDuration(Interval.Duration));
			return result;
		}
	}
	public class RecurrenceInfoJSONDeserializer {
		class Context {
			TimeZoneHelper timeZoneEngine;
			public Context(TimeZoneHelper timeZoneEngine) {
				this.timeZoneEngine = timeZoneEngine;
			}
			public TimeZoneHelper TimeZoneHelper { get { return timeZoneEngine; } }
		}
		delegate void DeserializePropertyHandler(object value, RecurrenceInfo info, Context context);
		static Dictionary<String, DeserializePropertyHandler> deserializeManager;
		static RecurrenceInfoJSONDeserializer() {
			deserializeManager = new Dictionary<string, DeserializePropertyHandler>();
			deserializeManager.Add(ClientRecurrenceInfoProperties.TypePropertyName, DeserializeType);
			deserializeManager.Add(ClientRecurrenceInfoProperties.IntervalPropertyName, DeserializeInterval);
			deserializeManager.Add(ClientRecurrenceInfoProperties.IdPropertyName, DeserializeId);
			deserializeManager.Add(ClientRecurrenceInfoProperties.MonthPropertyName, DeserializeMonth);
			deserializeManager.Add(ClientRecurrenceInfoProperties.WeekOfMonthPropertyName, DeserializeWeekOfMonth);
			deserializeManager.Add(ClientRecurrenceInfoProperties.WeekDaysPropertyName, DeserializeWeekDays);
			deserializeManager.Add(ClientRecurrenceInfoProperties.DayNumberPropertyName, DeserializeDayNumber);
			deserializeManager.Add(ClientRecurrenceInfoProperties.PeriodicityPropertyName, DeserializePeriodicity);
			deserializeManager.Add(ClientRecurrenceInfoProperties.OccurrenceCountPropertyName, DeserializeOccurrenceCount);
			deserializeManager.Add(ClientRecurrenceInfoProperties.RangePropertyName, DeserializeRange);
		}
		static void DeserializeType(object value, RecurrenceInfo info, Context context) {
			info.Type = (RecurrenceType)Enum.Parse(typeof(RecurrenceType), value.ToString());
		}
		static void DeserializeInterval(object value, RecurrenceInfo info, Context context) {
			TimeInterval interval = JSONSchedulerHelper.DeserializeInterval(value, context.TimeZoneHelper);
			info.Start = interval.Start;
			info.End = interval.End;
		}
		static void DeserializeId(object value, IRecurrenceInfo info, Context context) {
			Guid guid = new Guid(value.ToString());
			((IIdProvider)info).SetId(guid);
		}
		static void DeserializeMonth(object value, RecurrenceInfo info, Context context) {
			string stringValue = value.ToString();
			info.Month = int.Parse(stringValue);
		}
		static void DeserializeWeekOfMonth(object value, RecurrenceInfo info, Context context) {
			string stringValue = value.ToString();
			info.WeekOfMonth = (WeekOfMonth)Enum.Parse(typeof(WeekOfMonth), stringValue);
		}
		static void DeserializeWeekDays(object value, RecurrenceInfo info, Context context) {
			string stringValue = value.ToString();
			info.WeekDays = (WeekDays)(int.Parse(stringValue));
		}
		static void DeserializeDayNumber(object value, RecurrenceInfo info, Context context) {
			string stringValue = value.ToString();
			info.DayNumber = int.Parse(stringValue);
		}
		static void DeserializePeriodicity(object value, RecurrenceInfo info, Context context) {
			string stringValue = value.ToString();
			info.Periodicity = int.Parse(stringValue);
		}
		static void DeserializeOccurrenceCount(object value, RecurrenceInfo info, Context context) {
			string stringValue = value.ToString();
			info.OccurrenceCount = int.Parse(stringValue);
		}
		static void DeserializeRange(object value, RecurrenceInfo info, Context context) {
			info.Range = (RecurrenceRange)Enum.Parse(typeof(RecurrenceRange), value.ToString());
		}
		TimeZoneHelper timeZoneEngine;
		public RecurrenceInfoJSONDeserializer(TimeZoneHelper timeZoneEngine) {
			this.timeZoneEngine = timeZoneEngine;
		}
		public TimeZoneHelper TimeZoneHelper { get { return timeZoneEngine; } }
		public RecurrenceInfo DeserializeObject(string jsonString) {
			IDictionary infoAsDictionary = (IDictionary)HtmlConvertor.FromJSON(jsonString);
			return DeserializeObject(infoAsDictionary);
		}
		public RecurrenceInfo DeserializeObject(IDictionary infoAsDictionary) {
			RecurrenceInfo info = new RecurrenceInfo();
			Context context = new Context(TimeZoneHelper);
			foreach (KeyValuePair<string, DeserializePropertyHandler> handler in deserializeManager) {
				if (infoAsDictionary.Contains(handler.Key))
					handler.Value(infoAsDictionary[handler.Key], info, context);
			}
			return info;
		}
	 }
	public class ClientAppointmentJSONDeserializer {
		class Context {
			TimeZoneHelper timeZoneEngine;
			public Context(TimeZoneHelper timeZoneEngine) {
				this.timeZoneEngine = timeZoneEngine;
			}
			public TimeZoneHelper TimeZoneHelper { get { return timeZoneEngine; } }
		}
		delegate void DeserializePropertyHandler(object value, ClientAppointmentProperties apt, Context context);
		static Dictionary<String, DeserializePropertyHandler> deserializeManager;
		static ClientAppointmentJSONDeserializer() {
			deserializeManager = new Dictionary<string, DeserializePropertyHandler>();
			deserializeManager.Add(ClientSideAppointmentFieldNames.Id, DeserializeAppointmentId);
			deserializeManager.Add(ClientAppointmentProperties.IntervalPropertyName, DeserializeInterval);
			deserializeManager.Add(ClientSideAppointmentFieldNames.ResourceIds, DeserializeResourceIds);
			deserializeManager.Add(ClientSideAppointmentFieldNames.AppointmentType, DeserializeAppointmentType);
			deserializeManager.Add(ClientSideAppointmentFieldNames.StatusId, DeserializeStatusIndex);
			deserializeManager.Add(ClientSideAppointmentFieldNames.LabelId, DeserializeLabelIndex);
			deserializeManager.Add(ClientSideAppointmentFieldNames.Subject, DeserializeSubject);
			deserializeManager.Add(ClientSideAppointmentFieldNames.Description, DeserializeDescription);
			deserializeManager.Add(ClientSideAppointmentFieldNames.Location, DeserializeLocation);
			deserializeManager.Add(ClientSideAppointmentFieldNames.AllDay, DeserializeAllDay);
			deserializeManager.Add(ClientSideAppointmentFieldNames.RecurrenceInfo, DeserializeRecurrenceInfo);
		}
		static void DeserializeInterval(object value, ClientAppointmentProperties apt, Context context) {
			TimeInterval interval = JSONSchedulerHelper.DeserializeInterval(value, context.TimeZoneHelper);
			apt.Start = interval.Start;
			apt.End = interval.End;
		}
		static void DeserializeResourceIds(object value, ClientAppointmentProperties apt, Context context) {
			IList resourceIds = (IList)value;
			List<String> ids = ConvertToGenericList(resourceIds, new Converter<Object, String>(Convert.ToString));
			apt.ResourceIds = ids;
		}
		static List<T> ConvertToGenericList<T>(IList items, Converter<Object, T> converter) {
			List<T> result = new List<T>();
			int count = items.Count;
			for(int i = 0; i < count; i++) {
				T convertedItem = converter.Invoke(items[i]);
				result.Add(convertedItem);
			}
			return result;
		}
		static void DeserializeAppointmentType(object value, ClientAppointmentProperties apt, Context context) {
			apt.AppointmentType = (AppointmentType)Enum.Parse(typeof(AppointmentType), (string)value);
		}
		static void DeserializeAppointmentId(object value, ClientAppointmentProperties apt, Context context) {
			apt.Id = value.ToString();
		}
		static void DeserializeStatusIndex(object value, ClientAppointmentProperties apt, Context context) {
			string stringValue = value.ToString();
			apt.StatusId = int.Parse(stringValue);
		}
		static void DeserializeLabelIndex(object value, ClientAppointmentProperties apt, Context context) {
			string stringValue = value.ToString();
			apt.LabelId = int.Parse(stringValue);
		}
		static void DeserializeSubject(object value, ClientAppointmentProperties apt, Context context) {
			apt.Subject = value.ToString();
		}
		static void DeserializeDescription(object value, ClientAppointmentProperties apt, Context context) {
			apt.Description = value.ToString();
		}
		static void DeserializeLocation(object value, ClientAppointmentProperties apt, Context context) {
			apt.Location = value.ToString();
		}
		static void DeserializeAllDay(object value, ClientAppointmentProperties apt, Context context) {
			apt.AllDay = (bool)value;
		}
		static void DeserializeRecurrenceInfo(object value, ClientAppointmentProperties apt, Context context) { 
			RecurrenceInfoJSONDeserializer recurrenceInfoDeserializer = new RecurrenceInfoJSONDeserializer(context.TimeZoneHelper);
			apt.RecurrenceInfo = recurrenceInfoDeserializer.DeserializeObject((IDictionary)value);
		}
		static void DeserializeUnknowProperty(string propertyName, object value, ClientAppointmentProperties apt) {
			apt.Properties[propertyName] = value;
		}
		TimeZoneHelper timeZoneEngine;
		public ClientAppointmentJSONDeserializer(TimeZoneHelper timeZoneEngine) {
			this.timeZoneEngine = timeZoneEngine;
		}
		public TimeZoneHelper TimeZoneHelper { get { return timeZoneEngine; } }
		public ClientAppointmentProperties DeserializeObject(string jsonString) {
			Context context = new Context(TimeZoneHelper);
			IDictionary aptAsDictionary = (IDictionary)HtmlConvertor.FromJSON(jsonString);
			ClientAppointmentProperties apt = new ClientAppointmentProperties();
			foreach (DictionaryEntry entry in aptAsDictionary) {
				string propertyName = (String)entry.Key;
				DeserializeProperty(propertyName, entry.Value, apt, context);
			}
			return apt;
		}
		void DeserializeProperty(string propertyName, object value, ClientAppointmentProperties apt, Context context) {
			if(!deserializeManager.ContainsKey(propertyName) || value == null) {
				DeserializeUnknowProperty(propertyName, value, apt);
				return;
			}
			DeserializePropertyHandler handler = deserializeManager[propertyName];
			handler(value, apt, context);
		}
	}
	public static class JSONSchedulerHelper {
		public static TimeInterval DeserializeInterval(object value, TimeZoneHelper timeZoneEngine) {
			IDictionary intervalDictionary = (IDictionary)value;
			object dateTimeObject = intervalDictionary[ClientIntervalProperties.StartPropertyName];
			DateTime startDate = DateTime.MinValue;
			if (dateTimeObject is DateTime)
				startDate = (DateTime)dateTimeObject;
			else {
				string startDateString = intervalDictionary[ClientIntervalProperties.StartPropertyName].ToString();
				startDate = DateTime.Parse(startDateString, CultureInfo.InvariantCulture);
			}
			string durationString = intervalDictionary[ClientIntervalProperties.DurationPropertyName].ToString();
			TimeSpan duration = SchedulerWebUtils.ToTimeSpan(durationString);
			return new TimeInterval(timeZoneEngine.FromClientTime(startDate), duration);
		}
		public static string SerializeInterval(TimeInterval interval, TimeZoneHelper timeZoneEngine) {
			string startString = SchedulerWebUtils.ToJavaScriptDate(interval.Start);
			string durationString = SchedulerWebUtils.ToJavaScriptDurationString(interval.Duration);
			return String.Format("'start':{0},'duration':{1}", startString, durationString);
		}
	}
}
