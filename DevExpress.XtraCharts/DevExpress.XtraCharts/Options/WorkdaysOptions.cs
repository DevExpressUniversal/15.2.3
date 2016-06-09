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
using System.Globalization;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using DevExpress.Schedule;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	Flags,
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile)
	]
	public enum Weekday {
		Sunday = WeekdayCore.Sunday,
		Monday = WeekdayCore.Monday,
		Tuesday = WeekdayCore.Tuesday,
		Wednesday = WeekdayCore.Wednesdey,
		Thursday = WeekdayCore.Thursday,
		Friday = WeekdayCore.Friday,
		Saturday = WeekdayCore.Saturday
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(KnownDate.KnownDateTypeConverter))
	]
	public class KnownDate : ChartElementNamed, IKnownDate, IComparable {
		#region Nesed class: KnownDateTypeConverter
		internal class KnownDateTypeConverter : TypeConverter {
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
				return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
			}
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
				if (destinationType == typeof(InstanceDescriptor)) {
					KnownDate knownDate = (KnownDate)value;
					return new InstanceDescriptor(typeof(KnownDate).GetConstructor(new Type[] { typeof(string), typeof(DateTime) }),
						 new object[] { knownDate.Name, knownDate.Date }, true);
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
		#endregion
		DateTime date;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("KnownDateDate"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.KnownDate.Date"),
		XtraSerializableProperty
		]
		public DateTime Date {
			get { return date; }
			set {
				if (value != date) {
					SendNotification(new ElementWillChangeNotification(this));
					date = value;
					RaiseControlChanged();
				}
			}
		}
		public KnownDate(Holiday holiday)
			: this(holiday.DisplayName, holiday.Date) {
		}
		public KnownDate()
			: base(String.Empty) {
		}
		public KnownDate(string name, DateTime date)
			: base(name) {
			this.date = date;
		}
		#region IComparable
		int IComparable.CompareTo(object obj) {
			KnownDate otherDate = obj as KnownDate;
			if (otherDate == null)
				return 0;
			int result = date.CompareTo(otherDate.date);
			return result == 0 ? Name.CompareTo(otherDate.Name) : result;
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeDate() {
			return date != new DateTime();
		}
		void ResetDate() {
			Date = new DateTime();
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new KnownDate();
		}
		protected internal override bool ShouldSerialize() {
			return true;
		}
		protected override bool XtraShouldSerialize(string propertyName) {
			return propertyName == "Date" || base.XtraShouldSerialize(propertyName);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			KnownDate knownDate = obj as KnownDate;
			if (date != null)
				date = knownDate.Date;
		}
		public override bool Equals(object obj) {
			KnownDate knownDate = obj as KnownDate;
			return knownDate != null && base.Equals(knownDate) && date == knownDate.date;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override string ToString() {
			return date.ToShortDateString() + " - " + Name;
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartCollectionSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class KnownDateCollection : ChartCollectionBase {
		List<DateTime> sortedList;
		public KnownDate this[int index] {
			get { return (KnownDate)List[index]; }
		}
		internal KnownDateCollection(WorkdaysOptions workdaysOptions) {
			base.Owner = workdaysOptions;
		}
		internal IEnumerable<DateTime> GetDateTimeEnumerable() {
			if (sortedList == null) {
				sortedList = new List<DateTime>(Count);
				foreach (IKnownDate date in List) {
					DateTime day = date.Date;
					if (!sortedList.Contains(day))
						sortedList.Add(day);
				}
				sortedList.Sort();
			}
			return sortedList;
		}
		protected internal override void ProcessChanged(ChartUpdateInfoBase changeInfo) {
			sortedList = null;
			((WorkdaysOptions)Owner).RaiseCollectionChange(this);
		}
		public int Add(KnownDate date) {
			SendControlChanging();
			int res = base.Add(date);
			RaiseControlChanged();
			return res;
		}
		public void Insert(int index, KnownDate date) {
			SendControlChanging();
			base.Insert(index, date);
			RaiseControlChanged();
		}
		public void AddRange(KnownDate[] coll) {
			SendControlChanging();
			base.AddRange(coll);
			RaiseControlChanged();
		}
		public void Remove(KnownDate date) {
			SendControlChanging();
			base.Remove(date);
			RaiseControlChanged();
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(ExpandableObjectConverter))
	]
	public class WorkdaysOptions : ChartElement, IWorkdaysOptions, IXtraSupportDeserializeCollectionItem {
		const Weekday DefaultWorkdays = Weekday.Monday | Weekday.Tuesday | Weekday.Wednesday | Weekday.Thursday | Weekday.Friday;
		readonly KnownDateCollection holidays;
		readonly KnownDateCollection exactWorksdays;
		readonly CustomDateCollection customDays;
		Weekday workdays = DefaultWorkdays;
		internal CustomDateCollection CustomDays { get { return customDays; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("WorkdaysOptionsHolidays"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.WorkdaysOptions.Holidays"),
		TypeConverter(typeof(CollectionTypeConverter)),
		Editor("DevExpress.XtraCharts.Design.HolidayCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		Localizable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, 0, XtraSerializationFlags.DeserializeCollectionItemBeforeCallSetIndex)
		]
		public KnownDateCollection Holidays { get { return holidays; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("WorkdaysOptionsExactWorkdays"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.WorkdaysOptions.ExactWorkdays"),
		TypeConverter(typeof(CollectionTypeConverter)),
		Editor("DevExpress.XtraCharts.Design.HolidayCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		Localizable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, 0, XtraSerializationFlags.DeserializeCollectionItemBeforeCallSetIndex)
		]
		public KnownDateCollection ExactWorkdays { get { return exactWorksdays; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("WorkdaysOptionsWorkdays"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.WorkdaysOptions.Workdays"),
		Editor("DevExpress.XtraCharts.Design.WorkdaysEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		Localizable(true),
		XtraSerializableProperty
		]
		public Weekday Workdays {
			get { return workdays; }
			set {
				if (value != workdays) {
					if (value == 0)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgUnsupportedWorkdaysForWorkdaysOptions));
					SendNotification(new ElementWillChangeNotification(this));
					workdays = value;
					RaiseControlChanged(new PropertyUpdateInfo((ScaleOptions != null) ? (object)ScaleOptions.Axis : this, "Workdays"));
				}
			}
		}
		DateTimeScaleOptions ScaleOptions {
			get {
				return (DateTimeScaleOptions)Owner;
			}
		}
		internal WorkdaysOptions(DateTimeScaleOptions owner)
			: base(owner) {
			this.holidays = new KnownDateCollection(this);
			this.exactWorksdays = new KnownDateCollection(this);
			this.customDays = new CustomDateCollection();
		}
		#region IWorkdaysOptions
		WeekdayCore IWorkdaysOptions.Workdays { get { return (WeekdayCore)workdays; } }
		IEnumerable<CustomDate> IWorkdaysOptions.CustomDates { get { return customDays.Dates; } }
		IEnumerable<DateTime> IWorkdaysOptions.Holidays { get { return holidays.GetDateTimeEnumerable(); } }
		IEnumerable<DateTime> IWorkdaysOptions.ExactWorkdays { get { return exactWorksdays.GetDateTimeEnumerable(); } }
		bool IWorkdaysOptions.WorkdaysOnly { get { return ScaleOptions.WorkdaysOnly; } }
		DayOfWeek IWorkdaysOptions.FirstDayOfWeek { get { return System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek; } }
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeWorkdays() {
			return workdays != DefaultWorkdays;
		}
		void ResetWorkdays() {
			Workdays = DefaultWorkdays;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || holidays.Count > 0 || exactWorksdays.Count > 0 || ShouldSerializeWorkdays();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			return propertyName == "Workdays" ? ShouldSerializeWorkdays() : base.XtraShouldSerialize(propertyName);
		}
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			return (propertyName == "Holidays" || propertyName == "ExactWorkdays") ? new KnownDate() : null;
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			switch (propertyName) {
				case "Holidays":
					holidays.Add((KnownDate)e.Item.Value);
					break;
				case "ExactWorkdays":
					exactWorksdays.Add((KnownDate)e.Item.Value);
					break;
			}
		}
		#endregion
		void SyncCustomDates() {
			customDays.Clear();
			foreach (KnownDate date in holidays)
				customDays.SetDate(date.Date, true);
			foreach (KnownDate date in exactWorksdays)
				customDays.SetDate(date.Date, false);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing) {
				if (holidays != null)
					holidays.Dispose();
				if (exactWorksdays != null)
					exactWorksdays.Dispose();
			}
		}
		protected override ChartElement CreateObjectForClone() {
			return new WorkdaysOptions(null);
		}
		internal void RaiseCollectionChange(KnownDateCollection collection) {
			SyncCustomDates();
			if (collection == holidays)
				RaiseControlChanged(new PropertyUpdateInfo((ScaleOptions != null) ? (object)ScaleOptions.Axis : this, "Holidays"));
			else if (collection == exactWorksdays)
				RaiseControlChanged(new PropertyUpdateInfo((ScaleOptions != null) ? (object)ScaleOptions.Axis : this, "ExactWorkdays"));
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			WorkdaysOptions options = obj as WorkdaysOptions;
			if (options != null) {
				holidays.Assign(options.holidays);
				exactWorksdays.Assign(options.exactWorksdays);
				workdays = options.workdays;
				SyncCustomDates();
			}
		}
		public void LoadHolidays(string fileName, string location) {
			HolidayBaseCollection holidaysCollection = HolidaysLoader.LoadHolidaysCollection(fileName);
			holidays.BeginUpdate();
			try {
				holidays.Clear();
				if (String.IsNullOrEmpty(location))
					foreach (Holiday holiday in holidaysCollection)
						holidays.Add(new KnownDate(holiday));
				else
					foreach (Holiday holiday in holidaysCollection)
						if (holiday.Location == location)
							holidays.Add(new KnownDate(holiday));
			}
			finally {
				holidays.EndUpdate();
				SyncCustomDates();
			}
		}
		public void LoadHolidays(string fileName) {
			LoadHolidays(fileName, String.Empty);
		}
	}
}
