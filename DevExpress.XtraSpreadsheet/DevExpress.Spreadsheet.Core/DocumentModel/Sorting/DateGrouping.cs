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
using DevExpress.Office;
using DevExpress.Utils;
using System.Runtime.InteropServices;
using DevExpress.XtraSpreadsheet.Model.History;
namespace DevExpress.XtraSpreadsheet.Model {
	#region DateTimeGrouping
	public enum DateTimeGroupingType { 
		None = 0,
		Year = 1,
		Month = 2,
		Day = 3,
		Hour = 4,
		Minute = 5,
		Second = 6
	}
	#endregion
	#region DateGroupingInfo
	public class DateGroupingInfo : ICloneable<DateGroupingInfo>, ISupportsCopyFrom<DateGroupingInfo>, ISupportsSizeOf {
		#region Fields
		const uint MaskDateTimeGrouping = 0x00000007;   
		const uint MaskMonth			= 0x00000078;   
		const uint MaskDay			  = 0x00000F80;   
		const uint MaskHour			 = 0x0001F000;   
		const uint MaskMinute		   = 0x007E0000;   
		const uint MaskSecond		   = 0x1F800000;   
		uint packedValues;
		int year;
		#endregion
		#region Properties
		#region DateTimeGrouping
		public DateTimeGroupingType DateTimeGrouping {
			get { return (DateTimeGroupingType)(packedValues & MaskDateTimeGrouping); }
			set {
				packedValues &= ~MaskDateTimeGrouping;
				packedValues |= (uint)value & MaskDateTimeGrouping;
			}
		}
		#endregion
		#region Month
		public int Month {
			get { return (int)((packedValues & MaskMonth) >> 3); }
			set {
				packedValues &= ~MaskMonth;
				packedValues |= ((uint)value << 3) & MaskMonth;
			}
		}
		#endregion
		#region Day
		public int Day {
			get { return (int)((packedValues & MaskDay) >> 7); }
			set {
				packedValues &= ~MaskDay;
				packedValues |= ((uint)value << 7) & MaskDay;
			}
		}
		#endregion
		#region Hour
		public int Hour {
			get { return (int)((packedValues & MaskHour) >> 12); }
			set {
				packedValues &= ~MaskHour;
				packedValues |= ((uint)value << 12) & MaskHour;
			}
		}
		#endregion
		#region Minute
		public int Minute {
			get { return (int)((packedValues & MaskMinute) >> 17); }
			set {
				packedValues &= ~MaskMinute;
				packedValues |= ((uint)value << 17) & MaskMinute;
			}
		}
		#endregion
		#region Second
		public int Second {
			get { return (int)((packedValues & MaskSecond) >> 23); }
			set {
				packedValues &= ~MaskSecond;
				packedValues |= ((uint)value << 23) & MaskSecond;
			}
		}
		#endregion
		public int Year { get { return year; } set { year = value; } }
		#endregion
		protected internal bool HasMonth { get { return InRange(Month, 1, 12); } }
		protected internal bool HasDay { get { return InRange(Day, 1, 31); } }
		protected internal bool HasHour { get { return InRange(Hour, 0, 23); } }
		protected internal bool HasMinute { get { return InRange(Minute, 0, 59); } }
		protected internal bool HasSecond { get { return InRange(Second, 0, 59); } }
		bool InRange(int value, int minInclusive, int maxInclusive) {
			return value >= minInclusive && value <= maxInclusive;
		}
		#region ICloneable<DateGroupingInfo> Members
		public DateGroupingInfo Clone() {
			DateGroupingInfo clone = CreateEmptyClone();
			clone.CopyFrom(this);
			return clone;
		}
		protected virtual DateGroupingInfo CreateEmptyClone() {
			return new DateGroupingInfo();
		}
		#endregion
		#region ISupportsCopyFrom<DateGroupingInfo> Members
		public void CopyFrom(DateGroupingInfo value) {
			this.packedValues = value.packedValues;
			this.year = value.year;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			DateGroupingInfo info = obj as DateGroupingInfo;
			if (info == null)
				return false;
			return this.packedValues == info.packedValues &&
				this.year == info.year;
		}
		public override int GetHashCode() {
			return (int)(packedValues ^ year);
		}
	}
	#endregion
	#region DateGroupingInfoCache
	public class DateGroupingInfoCache : UniqueItemsCache<DateGroupingInfo> {
		public DateGroupingInfoCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override DateGroupingInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			DateGroupingInfo info = new DateGroupingInfo();
			info.DateTimeGrouping = DateTimeGroupingType.None;
			info.Year = -1;
			info.Month = 0;
			info.Day = 0;
			info.Hour = byte.MaxValue;
			info.Minute = byte.MaxValue;
			info.Second = byte.MaxValue;
			return info;
		}
	}
	#endregion
	#region DateGrouping
	public class DateGrouping : SpreadsheetUndoableIndexBasedObject<DateGroupingInfo> {
		#region Static Members
		public static DateGrouping Create(Worksheet sheet, DateTime date, DateTimeGroupingType groupingType) {
			DateGrouping result = new DateGrouping(sheet);
			result.BeginUpdate();
			try {
				result.DateTimeGrouping = groupingType;
				result.Year = date.Year;
				if (groupingType > DateTimeGroupingType.Year)
					result.Month = date.Month;
				if (groupingType > DateTimeGroupingType.Month)
					result.Day = date.Day;
				if (groupingType > DateTimeGroupingType.Day)
					result.Hour = date.Hour;
				if (groupingType > DateTimeGroupingType.Hour)
					result.Minute = date.Minute;
				if (groupingType > DateTimeGroupingType.Minute)
					result.Second = date.Second;
			}
			finally {
				result.EndUpdate();
			}
			return result;
		}
		#endregion
		public DateGrouping(Worksheet sheet)
			: base(sheet) {
		}		 
		#region Properties
		public Worksheet Worksheet { get { return (Worksheet)DocumentModelPart; } }
		#region DateTimeGrouping
		public DateTimeGroupingType DateTimeGrouping {
			get { return Info.DateTimeGrouping; }
			set {
				if (DateTimeGrouping == value)
					return;
				SetPropertyValue(SetDateTimeGroupingCore, value);
			}
		}
		DocumentModelChangeActions SetDateTimeGroupingCore(DateGroupingInfo info, DateTimeGroupingType value) {
			info.DateTimeGrouping = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Year
		public int Year {
			get { return Info.Year; }
			set {
				if (Year == value)
					return;
				SetPropertyValue(SetYearCore, value);
			}
		}
		DocumentModelChangeActions SetYearCore(DateGroupingInfo info, int value) {
			info.Year = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Month
		public int Month {
			get { return Info.Month; }
			set {
				if (Month == value)
					return;
				SetPropertyValue(SetMonthCore, value);
			}
		}
		DocumentModelChangeActions SetMonthCore(DateGroupingInfo info, int value) {
			info.Month = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Day
		public int Day {
			get { return Info.Day; }
			set {
				if (Day == value)
					return;
				SetPropertyValue(SetDayCore, value);
			}
		}
		DocumentModelChangeActions SetDayCore(DateGroupingInfo info, int value) {
			info.Day = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Hour
		public int Hour {
			get { return Info.Hour; }
			set {
				if (Hour == value)
					return;
				SetPropertyValue(SetHourCore, value);
			}
		}
		DocumentModelChangeActions SetHourCore(DateGroupingInfo info, int value) {
			info.Hour = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Minute
		public int Minute {
			get { return Info.Minute; }
			set {
				if (Minute == value)
					return;
				SetPropertyValue(SetMinuteCore, value);
			}
		}
		DocumentModelChangeActions SetMinuteCore(DateGroupingInfo info, int value) {
			info.Minute = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Second
		public int Second {
			get { return Info.Second; }
			set {
				if (Second == value)
					return;
				SetPropertyValue(SetSecondCore, value);
			}
		}
		DocumentModelChangeActions SetSecondCore(DateGroupingInfo info, int value) {
			info.Second = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		protected internal bool HasMonth { get { return InRange(Month, 1, 12); } }
		protected internal bool HasDay { get { return InRange(Day, 1, 31); } }
		protected internal bool HasHour { get { return InRange(Hour, 0, 23); } }
		protected internal bool HasMinute { get { return InRange(Minute, 0, 59); } }
		protected internal bool HasSecond { get { return InRange(Second, 0, 59); } }
		#endregion
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<DateGroupingInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.DateGroupingInfoCache;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			Worksheet.ApplyChanges(changeActions);
		}
		public bool IsDefault() {
			return Index == 0;
		}
		public DateTime CreateDateTime() {
			if (HasMonth && HasDay && HasHour && HasMinute && HasSecond)
				return new DateTime(Year, Month, Day, Hour, Minute, Second);
			else if (HasMonth && HasDay && HasHour && HasMinute)
				return new DateTime(Year, Month, Day, Hour, Minute, 0);
			else if (HasMonth && HasDay && HasHour)
				return new DateTime(Year, Month, Day, Hour, 0, 0);
			else if (HasMonth && HasDay)
				return new DateTime(Year, Month, Day);
			else if (HasMonth)
				return new DateTime(Year, Month, 1);
			return new DateTime(Year, 1, 1);
		}
		bool InRange(int value, int minInclusive, int maxInclusive) {
			return value >= minInclusive && value <= maxInclusive;
		}
		public bool IsVisible(DateTime date) {
			bool result = date.Year == Year;
			if (DateTimeGrouping >= DateTimeGroupingType.Month && HasMonth)
				result &= date.Month == Month;
			if (DateTimeGrouping >= DateTimeGroupingType.Day && HasDay)
				result &= date.Day == Day;
			if (DateTimeGrouping >= DateTimeGroupingType.Hour && HasHour)
				result &= date.Hour == Hour;
			if (DateTimeGrouping >= DateTimeGroupingType.Minute && HasMinute)
				result &= date.Minute == Minute;
			if (DateTimeGrouping >= DateTimeGroupingType.Second && HasSecond)
				result &= date.Second == Second;
			return result;
		}
	}
	#endregion
	#region DateGroupingCollection
	public class DateGroupingCollection : UndoableCollection<DateGrouping> {
		public DateGroupingCollection(Worksheet worksheet) : base(worksheet.Workbook) { }
		public bool ContainsInfo(DateGroupingInfo groupingInfo) {
			int count = InnerList.Count;
			if (count == 0)
				return false;
			for (int i = 0; i < count; i++) {
				DateGrouping current = InnerList[i];
				if (groupingInfo.Equals(current.Info))
					return true;
			}
			return false;
		}
	}
	#endregion
}
