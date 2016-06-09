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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.XtraGrid;
using DevExpress.Utils;
using DevExpress.Data.Filtering.Helpers;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Compatibility.System;
#if SL
using Comparer = System.Collections.Generic.Comparer<object>;
#endif
namespace DevExpress.XtraGrid {
	public enum ColumnFilterMode { Value, DisplayText }
	public enum ColumnSortMode { Default, Value, DisplayText, Custom }
	public enum ColumnGroupInterval { Default, Value, Date, DateMonth, DateYear, DateRange, Alphabetical, DisplayText }
}
namespace DevExpress.Data.Helpers {
	public enum OutlookInterval {
		Older,
		LastMonth,
		EarlierThisMonth,
		ThreeWeeksAgo,
		TwoWeeksAgo,
		LastWeek,
		PD0, PD1, PD2, PD3, PD4, PD5, PD6,
		Yesterday,
		Today,
		Tomorrow,
		D0, D1, D2, D3, D4, D5, D6,
		NextWeek,
		TwoWeeksAway,
		ThreeWeeksAway,
		LaterThisMonth,
		NextMonth,
		BeyondNextMonth,
	};
	public abstract class BaseFilterData : IDisposable {
		Dictionary<object, BaseGridColumnInfo> columns;
		string[] outlookNames;
		CultureInfo culture;
		DateTime sortStartTime, sortStartWeek, sortZeroTime;
		protected BaseFilterData() {
			this.columns = new Dictionary<object, BaseGridColumnInfo>();
			this.culture = CultureInfo.CurrentCulture;
			SortStartTime = DateTime.Now;
		}
		public virtual void Dispose() {
			if(this.columns == null) return;
			foreach(BaseGridColumnInfo column in Columns.Values) {
				column.Dispose();
			}
			Columns.Clear();
		}
		protected Dictionary<object, BaseGridColumnInfo> Columns { get { return columns; } }
		public void OnStart() {
			if (this.columns == null) this.columns = new Dictionary<object, BaseGridColumnInfo>();
			Columns.Clear();
			this.culture = CultureInfo.CurrentCulture;
			OnFillColumns();
			SortStartTime = DateTime.Now;
			if(Columns.Count == 0) this.columns = null;
		}
		public DateTime SortStartTime {
			get { return sortStartTime; }
			set {
				sortStartTime = value;
				this.sortStartWeek = OutlookDateHelper.GetWeekStart(value, DateTimeFormat);
				this.sortZeroTime = new DateTime(value.Year, value.Month, value.Day);
			}
		}
		public DateTime SortZeroTime { get { return sortZeroTime; } }
		public DateTime SortStartWeek { get { return sortStartWeek; } }
		public CultureInfo Culture { get { return culture; } }
		public DateTimeFormatInfo DateTimeFormat { get { return Culture.DateTimeFormat; } }
		protected abstract void OnFillColumns();
		public virtual bool IsRequired(DataColumnInfo column) {
			if(this.columns == null || Columns.Count == 0) return false;
			object key = GetKey(column);
			if(key == null) return false;
			return Columns.ContainsKey(key);
		}
		public virtual BaseGridColumnInfo GetInfo(DataColumnInfo info) {
			if(this.columns == null) return null;
			return GetInfoCore(GetKey(info));
		}
		protected virtual BaseGridColumnInfo GetInfoCore(object key) {
			if(this.columns == null || key == null) return null;
			BaseGridColumnInfo result;
			Columns.TryGetValue(key, out result);
			return result;
		}
		protected virtual object GetKey(DataColumnInfo column) {
			if(column == null) return null;
			return column.Name;
		}
		public string GetDisplayText(int listSourceIndex, DataColumnInfo info, object value) {
			BaseGridColumnInfo gi = GetInfo(info);
			if(gi == null) return string.Empty;
			return gi.GetDisplayText(listSourceIndex, value);
		}
		public string GetOutlookLocaizedString(int id) {
			if(this.outlookNames == null) {
				this.outlookNames = GetOutlookLocalizedStrings();
			}
			if(this.outlookNames.Length <= id) return "";
			return this.outlookNames[id];
		}
		protected virtual string[] GetOutlookLocalizedStrings() { return new string[] { }; }
		public abstract int SortCount { get; }
		public abstract int GroupCount { get; }
		public abstract int GetSortIndex(object column);
	}
	public abstract class BaseGridColumnInfo : IDisposable {
		FilterDataOutlookDateHelper outlookHelper;
		BaseFilterData data;
		bool required = false;
		object column;
		bool isLastSortColumn, isGrouped;
		protected BaseGridColumnInfo(BaseFilterData data, object column) {
			this.data = data;
			this.column = column;
			this.outlookHelper = new FilterDataOutlookDateHelper(data);
			int sortIndex = Data.GetSortIndex(column);
			this.isGrouped = sortIndex >= 0 && sortIndex < Data.GroupCount;
			this.isLastSortColumn = sortIndex >= 0 && sortIndex == Data.SortCount - 1;
		}
		public virtual void Dispose() { }
		public object Column { get { return column; } }
		public virtual bool Required { get { return required; } set { required = value; } }
		public BaseFilterData Data { get { return data; } }
		public abstract string GetDisplayText(int listSourceIndex, object val);
		protected bool IsGrouped { get { return isGrouped; } }
		protected bool IsLastSortColumn { get { return isLastSortColumn; } }
		public ColumnSortMode SortMode = ColumnSortMode.Default;
		public ColumnGroupInterval GroupInterval = ColumnGroupInterval.Default;
		public object UpdateGroupDisplayValue(object val) {
			if(SortMode == ColumnSortMode.DisplayText) return val;
			switch(GroupInterval) {
				case ColumnGroupInterval.Date:
					return GetDate(val);
				case ColumnGroupInterval.DateMonth:
					return GetDateMonth(val);
				case ColumnGroupInterval.DateYear:
					return GetDateYear(val);
			}
			return val;
		}
		public bool AllowImageGroup {
			get { return GroupInterval != ColumnGroupInterval.Alphabetical; }
		}
		public string GetGroupDisplayText(object val, string text) {
			if(GroupInterval == ColumnGroupInterval.Alphabetical) {
				if(text != null && text.Length > 0) return text.Substring(0, 1);
				return text;
			}
			if(SortMode == ColumnSortMode.DisplayText) return text;
			switch(GroupInterval) {
				case ColumnGroupInterval.DateRange :
					return GetOutlookDisplayText(val);
			}
			return text;
		}
		public string GetOutlookDisplayText(object val) {
			OutlookInterval? ninterval = GetOutlookInterval(val);
			if(!ninterval.HasValue)
				return string.Empty;
			OutlookInterval interval = ninterval.Value;
			int id = (int)interval;
			string text = Data.GetOutlookLocaizedString(id);
			if(interval >= OutlookInterval.D0 && interval <= OutlookInterval.D6) {
				if(String.IsNullOrEmpty(text))
					text = GetDayName(id - (int)OutlookInterval.D0);
			}
			if(interval >= OutlookInterval.PD0 && interval <= OutlookInterval.PD6) {
				if(String.IsNullOrEmpty(text))
					text = GetDayName(id - (int)OutlookInterval.PD0);
			}
			return text;
		}
		string GetDayName(int day) {
			int delta = (int)Data.DateTimeFormat.FirstDayOfWeek - (int)DayOfWeek.Sunday;
			day = (day + delta) % 7;
			return Data.DateTimeFormat.DayNames[day];
		}
		int? GetDateMonthInt(object val) {
			DateTime? ndt = GetDateTime(val);
			if(!ndt.HasValue)
				return null;
			DateTime dt = ndt.Value;
			return (dt.Year << 5) | dt.Month;
		}
		int? GetDateYearInt(object val) {
			DateTime? ndt = GetDateTime(val);
			if(!ndt.HasValue)
				return null;
			DateTime dt = ndt.Value;
			return dt.Year;
		}
		int? GetDateInt(object val) {
			DateTime? ndt = GetDateTime(val);
			if(!ndt.HasValue)
				return null;
			DateTime dt = ndt.Value;
			return (dt.Year << 10) | (dt.Month << 6) | (dt.Day);
		}
		public DateTime? GetDateMonth(object val) {
			DateTime? ndt = GetDateTime(val);
			if(!ndt.HasValue)
				return null;
			DateTime dt = ndt.Value;
			return new DateTime(dt.Year, dt.Month, 1);
		}
		public DateTime? GetDateYear(object val) {
			DateTime? ndt = GetDateTime(val);
			if(!ndt.HasValue)
				return null;
			DateTime dt = ndt.Value;
			return new DateTime(dt.Year, 1, 1);
		}
		public DateTime? GetDateTime(object val) {
			if(val is DateTime)
				return (DateTime)val;
			if(val == null || val is DBNull)
				return null;
			DateTime rv;
			if(DateTime.TryParse(val.ToString(), out rv))
				return rv;
			return null;
		}
		public DateTime? GetDate(object val) {
			DateTime? ndt = GetDateTime(val);
			if(!ndt.HasValue)
				return null;
			DateTime dt = ndt.Value;
			return new DateTime(dt.Year, dt.Month, dt.Day);
		}
		public DateTime? GetTime(object val) {
			return GetDateTime(val); 
		}
		public string GetAlpha(object val) {
			if(val == null || val == DBNull.Value) return null;
			string s = val.ToString();
			if(s.Length == 0) return s;
			return s.Substring(0, 1);
		}
		public OutlookInterval? GetOutlookInterval(object val) {
			return outlookHelper.GetOutlookInterval(GetDateTime(val));
		}
		public ExpressiveSortInfo.Cell GetCompareSortValuesInfo(Type basicExtractorType, ColumnSortOrder sortOrder) {
			if(SortMode == ColumnSortMode.Custom) {
				Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, object, object, int?> customSortRaiser = (r1, r2, value1, value2) => RaiseCustomSort(r1.SourceIndex, r2.SourceIndex, value1, value2, sortOrder);
				return new ExpressiveSortInfo.Cell(true, null, null
					, customSortRaiser
					, false, false);
			}
			if(SortMode == ColumnSortMode.DisplayText) {
				Comparer<string> stringComparer4Capture = Comparer<string>.Default;
				Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, object, string> transformer = (r, boxedValue) => GetDisplayText(r.SourceIndex, boxedValue);
				Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, string, string, int> comparer = (r1, r2, displayText1, displayText2)
					=> stringComparer4Capture.Compare(displayText1, displayText2);
				return new ExpressiveSortInfo.Cell(true, typeof(string), transformer, comparer, true, true);
			}
			if(IsGrouped && !IsLastSortColumn) {
				return GetCompareGroupValuesInfo(basicExtractorType);
			}
			switch(GroupInterval) {
				case ColumnGroupInterval.DateRange:
				case ColumnGroupInterval.Date:
				case ColumnGroupInterval.DateMonth:
				case ColumnGroupInterval.DateYear:
					Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, object, DateTime?> transformer = (r, boxedValue)
						=> GetDateTime(boxedValue);
					Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, DateTime?, DateTime?, int> comparer = (r1, r2, v1, v2)
						=> CompareNullableComparables(v1, v2);
					return new ExpressiveSortInfo.Cell(true, typeof(DateTime?), transformer, comparer, true, true);
			}
			return null;
		}
		static int CompareNullableComparables<T>(T? v1, T? v2) where T: struct, IComparable<T> {
			if(v1.HasValue) {
				if(v2.HasValue)
					return v1.Value.CompareTo(v2.Value);
				else
					return 1;
			} else {
				if(v2.HasValue)
					return -1;
				else
					return 0;
			}
		}
		public int? CompareSortValues(int listSourceRow1, int listSourceRow2, object value1, object value2, ColumnSortOrder sortOrder) {
			if(SortMode == ColumnSortMode.Custom) {
				return RaiseCustomSort(listSourceRow1, listSourceRow2, value1, value2, sortOrder);
			}
			if(IsGrouped && !IsLastSortColumn) {
				if(SortMode != ColumnSortMode.DisplayText)
					return CompareGroupValues(listSourceRow1, listSourceRow2, value1, value2);
			}
			if(SortMode == ColumnSortMode.DisplayText) {
				string text1, text2;
				text1 = GetDisplayText(listSourceRow1, value1);
				text2 = GetDisplayText(listSourceRow2, value2);
				return Comparer<string>.Default.Compare(text1, text2);
			}
			switch(GroupInterval) {
				case ColumnGroupInterval.DateRange:
				case ColumnGroupInterval.Date:
				case ColumnGroupInterval.DateMonth:
				case ColumnGroupInterval.DateYear: {
						DateTime? v1 = GetDateTime(value1);
						DateTime? v2 = GetDateTime(value2);
						return CompareNullableComparables(v1, v2);
					}
			}
			return null;
		}
		protected abstract int? RaiseCustomSort(int listSourceRow1, int listSourceRow2, object value1, object value2, ColumnSortOrder sortOrder);
		protected abstract int? RaiseCustomGroup(int listSourceRow1, int listSourceRow2, object value1, object value2, ColumnSortOrder columnSortOrder);
		public ExpressiveSortInfo.Cell GetCompareGroupValuesInfo(Type basicExtractorType) {
			if(SortMode == ColumnSortMode.Custom) {
				Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, object, object, int?> customGroupRaiser = (r1, r2, value1, value2) => RaiseCustomGroup(r1.SourceIndex, r2.SourceIndex, value1, value2, ColumnSortOrder.None);
				return new ExpressiveSortInfo.Cell(true, null, null, customGroupRaiser, false, false);
			}
			if(GroupInterval == ColumnGroupInterval.DisplayText) {
				Comparer<string> stringComparer4Capture = Comparer<string>.Default;
				Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, object, string> transformer = (r, boxedValue) => GetDisplayText(r.SourceIndex, boxedValue);
				Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, string, string, int> comparer = (r1, r2, displayText1, displayText2)
					=> stringComparer4Capture.Compare(displayText1, displayText2);
				return new ExpressiveSortInfo.Cell(true, typeof(string), transformer, comparer, true, true);
			}
			if(SortMode == ColumnSortMode.DisplayText) {
				if(GroupInterval == ColumnGroupInterval.Alphabetical) {
					Comparer<string> stringComparer4Capture = Comparer<string>.Default;
					Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, object, string> transformer = (r, boxedValue) => GetAlpha(GetDisplayText(r.SourceIndex, boxedValue));
					Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, string, string, int> comparer = (r1, r2, a1, a2)
						=> stringComparer4Capture.Compare(a1, a2);
					return new ExpressiveSortInfo.Cell(true, typeof(string), transformer, comparer, true, true);
				} else
					return null;
			}
			switch(GroupInterval){
				case ColumnGroupInterval.Alphabetical: {
						Comparer<string> stringComparer4Capture = Comparer<string>.Default;
						Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, object, string> transformer = (r, boxedValue) => GetAlpha(boxedValue);
						Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, string, string, int> comparer = (r1, r2, a1, a2)
							=> stringComparer4Capture.Compare(a1, a2);
						return new ExpressiveSortInfo.Cell(true, typeof(string), transformer, comparer, true, true);
					}
				case ColumnGroupInterval.DateMonth:
					return MakeDateRangeExtractorUndComparerInfo((r, v) => GetDateMonthInt(v));
				case ColumnGroupInterval.DateYear:
					return MakeDateRangeExtractorUndComparerInfo((r, v) => GetDateYearInt(v));
				case ColumnGroupInterval.Date:
					return MakeDateRangeExtractorUndComparerInfo((r, v) => GetDateInt(v));
				case ColumnGroupInterval.DateRange:
					return MakeDateRangeExtractorUndComparerInfo((r, v) => (int?)GetOutlookInterval(v));
			}
			return null;
		}
		ExpressiveSortInfo.Cell MakeDateRangeExtractorUndComparerInfo(Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, object, int?> transformator) {
			Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, int?, int?, int> comparer = (r1, r2, g1, g2)
				=> CompareNullableComparables(g1, g2);
			return new ExpressiveSortInfo.Cell(true, typeof(int?), transformator, comparer, true, true);
		}
		public int? CompareGroupValues(int listSourceRow1, int listSourceRow2, object value1, object value2) {
			if(SortMode == ColumnSortMode.Custom) {
				return RaiseCustomGroup(listSourceRow1, listSourceRow2, value1, value2, ColumnSortOrder.None);
			}
			if(GroupInterval == ColumnGroupInterval.DisplayText) {
				return Comparer<string>.Default.Compare(GetDisplayText(listSourceRow1, value1), GetDisplayText(listSourceRow2, value2));
			}
			if(SortMode == ColumnSortMode.DisplayText) {
				if(GroupInterval == ColumnGroupInterval.Alphabetical) {
					value1 = GetDisplayText(listSourceRow1, value1);
					value2 = GetDisplayText(listSourceRow2, value2);
				}
				else
					return null;
			}
			switch(GroupInterval) {
				case ColumnGroupInterval.Alphabetical :
					return Comparer<string>.Default.Compare(GetAlpha(value1), GetAlpha(value2));
				case ColumnGroupInterval.DateMonth :
					return CompareNullableComparables(GetDateMonthInt(value1), GetDateMonthInt(value2));
				case ColumnGroupInterval.DateYear :
					return CompareNullableComparables(GetDateYearInt(value1), GetDateYearInt(value2));
				case ColumnGroupInterval.Date :
					return CompareNullableComparables(GetDateInt(value1), GetDateInt(value2));
				case ColumnGroupInterval.DateRange :
					return CompareNullableComparables((int?)GetOutlookInterval(value1), (int?)GetOutlookInterval(value2));
			}
			return null;
		}
		static FormatInfo CreateFormat(FormatType type, string format) {
			FormatInfo res = new FormatInfo();
			res.FormatType = type;
			res.FormatString = format;
			return res;
		}
		static FormatInfo[] defaultFormats = new FormatInfo[] { 
				CreateFormat(FormatType.DateTime, "d"), CreateFormat(FormatType.DateTime, "y"), 
				CreateFormat(FormatType.DateTime, "yyyy")};
		public virtual FormatInfo GetColumnGroupFormat() { return GetDefaultFormat(); }
		public FormatInfo GetDefaultFormat() {
			if(SortMode != ColumnSortMode.Value) return null;
			switch(GroupInterval) {
				case ColumnGroupInterval.Date:
					return defaultFormats[0];
				case ColumnGroupInterval.DateMonth:
					return defaultFormats[1];
				case ColumnGroupInterval.DateYear:
					return defaultFormats[2];
			}
			return null;
		}
	}
	public class FilterDataOutlookDateHelper : OutlookDateHelper {
		BaseFilterData data;
		public FilterDataOutlookDateHelper(BaseFilterData data) {
			this.data = data;
		}
		protected override DateTime SortZeroTime { get { return this.data.SortZeroTime; } }
		protected override DateTime SortStartWeek { get { return this.data.SortStartWeek; } }
	}
	public abstract class OutlookDateHelper {
		protected abstract DateTime SortZeroTime { get ; }
		protected abstract DateTime SortStartWeek { get; }
		public OutlookInterval? GetOutlookInterval(DateTime? ndate) {
			if(!ndate.HasValue)
				return null;
			DateTime sortTime = SortZeroTime;
			DateTime weekStart = SortStartWeek;
			DateTime date = new DateTime(ndate.Value.Year, ndate.Value.Month, ndate.Value.Day);
			TimeSpan weekSpan = weekStart.Subtract(date);
			bool less = date < SortZeroTime;
			bool lessWeek = date < weekStart;
			int weekDays = weekSpan.Days;
			int weeks = GetWeekNumber(weekSpan, lessWeek);
			if(Math.Abs(weeks) < 2) {
				TimeSpan span = SortZeroTime.Subtract(date);
				int days = span.Days;
				if(Math.Abs(days) < 2) {
					if(days == 0) return OutlookInterval.Today;
					if(span.Ticks >= 0) {
						if(days == 1) return OutlookInterval.Yesterday;
					}
					else {
						if(days == -1) return OutlookInterval.Tomorrow;
					}
				}
			}
			if(weeks == 0) {
				if(less) return (OutlookInterval)(((int)OutlookInterval.PD0) + Math.Abs(weekDays));
				return (OutlookInterval)(((int)OutlookInterval.D0) + Math.Abs(weekDays));
			}
			if(weeks > 0) {
				if(GetMonthDelta(sortTime, date) > 1) return OutlookInterval.Older;
				if(weeks > 3) {
					return sortTime.Month != date.Month ? OutlookInterval.LastMonth : OutlookInterval.EarlierThisMonth;
				}
				if(weeks > 2) return OutlookInterval.ThreeWeeksAgo;
				if(weeks > 1) return OutlookInterval.TwoWeeksAgo;
				if(weeks > 0) return OutlookInterval.LastWeek;
			}
			else {
				if(GetMonthDelta(sortTime, date) > 1) return OutlookInterval.BeyondNextMonth;
				if(weeks < -3) {
					return sortTime.Month != date.Month ? OutlookInterval.NextMonth : OutlookInterval.LaterThisMonth;
				}
				if(weeks < -2) return OutlookInterval.ThreeWeeksAway;
				if(weeks < -1) return OutlookInterval.TwoWeeksAway;
				if(weeks < 0) return OutlookInterval.NextWeek;
			}
			return OutlookInterval.Older;
		}
		int GetMonthDelta(DateTime d1, DateTime d2) {
			int y1 = Math.Max(d1.Year, d2.Year), y2 = Math.Min(d1.Year, d2.Year);
			if(y1 - y2 > 1) return 12;
			if(y1 - y2 == 0) {
				int i1 = Math.Max(d1.Month, d2.Month), i2 = Math.Min(d1.Month, d2.Month);
				return i1 - i2;
			}
			int m1 = d1.Month, m2 = d2.Month;
			if(d1.Year > d2.Year) return m1 + 12 - m2;
			return m2 + 12 - m1;
		}
		public int GetWeekNumber(TimeSpan weekSpan, bool alwaysAdd) {
			int res = weekSpan.Days / 7;
			if((weekSpan.Days > 0 && (weekSpan.Days % 7) != 0) || (weekSpan.Days == 0 && alwaysAdd)) res++;
			return res;
		}
		public static DateTime GetWeekStart(DateTime sortTime) {
			return GetWeekStart(sortTime, CultureInfo.CurrentUICulture.DateTimeFormat);
		}
		public static DateTime GetWeekStart(DateTime sortTime, DateTimeFormatInfo dateFormatInfo) {
			while(sortTime.DayOfWeek != dateFormatInfo.FirstDayOfWeek) {
				sortTime = sortTime.AddDays(-1);
			}
			return new DateTime(sortTime.Year, sortTime.Month, sortTime.Day);
		}
	}
	public class ExpressiveSortInfo {
		public struct CondencedAndSourceIndicesPair {
			public readonly int CondencedIndex;
			public readonly int SourceIndex;
			public CondencedAndSourceIndicesPair(int _CondencedIndex, int _SourceIndex) {
				this.CondencedIndex = _CondencedIndex;
				this.SourceIndex = _SourceIndex;
			}
			public override string ToString() {
				return string.Format("[c:{0}, s:{1}]", CondencedIndex, SourceIndex);
			}
		}
		public class Cell {
			public readonly bool RequireBoxedInput;
			public readonly Type TransformedType;
			public readonly Delegate Transformator;	 
			public readonly Delegate ValuesComparer;	
			public readonly bool IsFinalComparer; 
			public readonly bool IsComparerThreadSafe;
			public Cell(bool requireBoxedInput, Type transformedType, Delegate transformator, Delegate valuesComparer, bool isValuesComparerFinalComparer, bool isComparerThreadSafe) {
				this.RequireBoxedInput = requireBoxedInput;
				this.TransformedType = transformedType;
				this.Transformator = transformator;
				this.ValuesComparer = valuesComparer;
				this.IsFinalComparer = isValuesComparerFinalComparer;
				this.IsComparerThreadSafe = isComparerThreadSafe;
			}
		}
		public class Row {
			public readonly Delegate RowsComparer;  
			public readonly bool IsFinalComparer;
			public readonly bool IsComparerThreadSafe;
			public Row(Delegate rowsComparer, bool isFinalComparer, bool isComparerThreadSafe) {
				this.RowsComparer = rowsComparer;
				this.IsFinalComparer = isFinalComparer;
				this.IsComparerThreadSafe = isComparerThreadSafe;
			}
		}
	}
	public static class ExpressiveSortHelpers {
		public static class GetCompareRowCellInfoTypedCompareHelper {
			static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, bool> threadsafeCache = new System.Collections.Concurrent.ConcurrentDictionary<Type, bool>();
			static bool _GuessIsThreadSafeTypeAndDefaultComparer_Core(Type t) {
				return t.IsPrimitive() || t.IsEnum() || (t.IsValueType() && typeof(int).GetAssembly() == t.GetAssembly()) || t == typeof(string);
			}
			static bool GuessIsThreadSafeTypeAndDefaultComparer(Type t) {
				return threadsafeCache.GetOrAdd(t, _GuessIsThreadSafeTypeAndDefaultComparer_Core);
			}
			public abstract class PlainCombarableMaker: GenericInvoker<Func<ExpressiveSortInfo.Cell, ExpressiveSortInfo.Cell>, PlainCombarableMaker.Impl<string>> {
				public class Impl<T>: PlainCombarableMaker where T: IComparable<T> {
					static ExpressiveSortInfo.Cell Make(ExpressiveSortInfo.Cell sortedByClientMethodInfo) {
						var capturedComparer = Comparer<T>.Default;
						bool isThreadSafe = (sortedByClientMethodInfo != null) ? sortedByClientMethodInfo.IsComparerThreadSafe : true;
						if(isThreadSafe)
							isThreadSafe = GuessIsThreadSafeTypeAndDefaultComparer(typeof(T));
						if(sortedByClientMethodInfo == null) {
							Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, T, T, int> cmp = (row1, row2, v1, v2) => capturedComparer.Compare(v1, v2);
							return new ExpressiveSortInfo.Cell(false, null, null, cmp, true, isThreadSafe);
						} else if(sortedByClientMethodInfo.RequireBoxedInput) {
							var clientComparer = (Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, object, object, int?>)sortedByClientMethodInfo.ValuesComparer;
							Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, object, object, int> finalComparer = (row1, row2, v1, v2) => clientComparer(row1, row2, v1, v2) ?? capturedComparer.Compare((T)v1, (T)v2);
							return new ExpressiveSortInfo.Cell(true, null, null, finalComparer, true, isThreadSafe);
						} else {
							var clientComparer = (Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, T, T, int?>)sortedByClientMethodInfo.ValuesComparer;
							Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, T, T, int> finalComparer = (row1, row2, v1, v2) => clientComparer(row1, row2, v1, v2) ?? capturedComparer.Compare(v1, v2);
							return new ExpressiveSortInfo.Cell(false, null, null, finalComparer, true, isThreadSafe);
						}
					}
					protected override Func<ExpressiveSortInfo.Cell, ExpressiveSortInfo.Cell> CreateInvoker() {
						return sortedByClientMethodInfo => Make(sortedByClientMethodInfo);
					}
				}
			}
			public abstract class NullableCombarableMaker: GenericInvoker<Func<ExpressiveSortInfo.Cell, ExpressiveSortInfo.Cell>, NullableCombarableMaker.Impl<int>> {
				public class Impl<T>: NullableCombarableMaker where T: struct, IComparable<T> {
					static int CompareNullableComparables(T? v1, T? v2) {
						if(v1.HasValue) {
							if(v2.HasValue)
								return v1.Value.CompareTo(v2.Value);
							else
								return 1;
						} else {
							if(v2.HasValue)
								return -1;
							else
								return 0;
						}
					}
					static ExpressiveSortInfo.Cell Make(ExpressiveSortInfo.Cell sortedByClientMethodInfo) {
						bool isThreadSafe = (sortedByClientMethodInfo != null) ? sortedByClientMethodInfo.IsComparerThreadSafe : true;
						if(isThreadSafe)
							isThreadSafe = GuessIsThreadSafeTypeAndDefaultComparer(typeof(T));
						if(sortedByClientMethodInfo == null) {
							Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, T?, T?, int> cmp = (row1, row2, v1, v2) => CompareNullableComparables(v1, v2);
							return new ExpressiveSortInfo.Cell(false, null, null, cmp, true, isThreadSafe);
						} else if(sortedByClientMethodInfo.RequireBoxedInput) {
							var clientComparer = (Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, object, object, int?>)sortedByClientMethodInfo.ValuesComparer;
							Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, object, object, int> finalComparer = (row1, row2, v1, v2) => clientComparer(row1, row2, v1, v2) ?? CompareNullableComparables((T?)v1, (T?)v2);
							return new ExpressiveSortInfo.Cell(true, null, null, finalComparer, true, isThreadSafe);
						} else {
							var clientComparer = (Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, T?, T?, int?>)sortedByClientMethodInfo.ValuesComparer;
							Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, T?, T?, int> finalComparer = (row1, row2, v1, v2) => clientComparer(row1, row2, v1, v2) ?? CompareNullableComparables(v1, v2);
							return new ExpressiveSortInfo.Cell(false, null, null, finalComparer, true, isThreadSafe);
						}
					}
					protected override Func<ExpressiveSortInfo.Cell, ExpressiveSortInfo.Cell> CreateInvoker() {
						return sortedByClientMethodInfo => Make(sortedByClientMethodInfo);
					}
				}
			}
			public abstract class FallbackCombarableMaker: GenericInvoker<Func<ExpressiveSortInfo.Cell, ExpressiveSortInfo.Cell>, FallbackCombarableMaker.Impl<object>> {
				public class Impl<T>: FallbackCombarableMaker {
					static ExpressiveSortInfo.Cell Make(ExpressiveSortInfo.Cell sortedByClientMethodInfo) {
						bool isThreadSafe = (sortedByClientMethodInfo != null) ? sortedByClientMethodInfo.IsComparerThreadSafe : true;
						if(isThreadSafe)
							isThreadSafe = GuessIsThreadSafeTypeAndDefaultComparer(typeof(T));
						if(sortedByClientMethodInfo == null) {
							var capturedComparer = Comparer.Default;
							Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, object, object, int> objComparer
								= (r1, r2, value1, value2)
									=> capturedComparer.Compare(value1, value2);
							return new ExpressiveSortInfo.Cell(true, null, null, objComparer, true, isThreadSafe);
						} else if(sortedByClientMethodInfo.RequireBoxedInput) {
							var capturedComparer = Comparer.Default;
							var clientComparer = (Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, object, object, int?>)sortedByClientMethodInfo.ValuesComparer;
							Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, object, object, int> objComparer
								= (r1, r2, value1, value2)
									=> clientComparer(r1, r2, value1, value2) ?? capturedComparer.Compare(value1, value2);
							return new ExpressiveSortInfo.Cell(true, null, null, objComparer, true, isThreadSafe);
						} else {
							var capturedComparer = Comparer<T>.Default;
							var clientComparer = (Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, T, T, int?>)sortedByClientMethodInfo.ValuesComparer;
							Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, T, T, int> objComparer
								= (listSourceRow1, listSourceRow2, value1, value2)
									=> clientComparer(listSourceRow1, listSourceRow2, value1, value2) ?? capturedComparer.Compare(value1, value2);
							return new ExpressiveSortInfo.Cell(false, null, null, objComparer, true, isThreadSafe);
						}
					}
					protected override Func<ExpressiveSortInfo.Cell, ExpressiveSortInfo.Cell> CreateInvoker() {
						return sortedByClientMethodInfo => Make(sortedByClientMethodInfo);
					}
				}
			}
			public static ExpressiveSortInfo.Cell MakeFinalCompare(ExpressiveSortInfo.Cell sortedByClientMethodInfo, Type baseExtractorType) {
				if(sortedByClientMethodInfo != null) {
					if(sortedByClientMethodInfo.IsFinalComparer)
						throw new InvalidOperationException("sortedByClientMethodInfo.IsFinalComparer");
					if(sortedByClientMethodInfo.TransformedType != null)
						throw new InvalidOperationException("sortedByClientMethodInfo.TransformedType != null");
				}
				Type boxedType = NullableHelpers.GetBoxedType(baseExtractorType);
				bool isTypedCmpType = typeof(IComparable<>).MakeGenericType(boxedType).IsAssignableFrom(boxedType);
				if(isTypedCmpType) {
					if(baseExtractorType != boxedType) {
						return NullableCombarableMaker.GetInvoker(boxedType)(sortedByClientMethodInfo);
					} else {
						return PlainCombarableMaker.GetInvoker(baseExtractorType)(sortedByClientMethodInfo);
					}
				} else {
					return FallbackCombarableMaker.GetInvoker(baseExtractorType)(sortedByClientMethodInfo);
				}
			}
		}
		public abstract class CellComparerBuilder: GenericInvoker<Func<Delegate, Delegate, bool, Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, int>>, CellComparerBuilder.Impl<object>> {
			public class Impl<T>: CellComparerBuilder {
				static Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, int> Build(Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, T> finalGetter, Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, T, T, int> comparer, bool isNegative) {
					Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, int> rv;
					if(isNegative) {
						rv = (r1, r2) =>
								comparer(r2, r1, finalGetter(r2), finalGetter(r1));
					} else {
						rv = (r1, r2) =>
								comparer(r1, r2, finalGetter(r1), finalGetter(r2));
					}
					return rv;
				}
				protected override Func<Delegate, Delegate, bool, Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, int>> CreateInvoker() {
					return (getter, comparer, isDesc) =>
						Build((Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, T>)getter, (Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, T, T, int>)comparer, isDesc);
				}
			}
		}
		static Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, int> MakeRowsCompare(IDataControllerSort expressiveSortClient, IEnumerable<DataColumnSortInfo> sortInfos, Func<DataColumnSortInfo, Type, Delegate> getterGetter, Func<Type, Delegate, bool, int, Delegate> finalCompareValueGetterCacher, ref bool isPlinqAble) {
			ExpressiveSortInfo.Row clientRowComparer = expressiveSortClient != null ? expressiveSortClient.GetCompareRowsMethodInfo() : null;
			if(clientRowComparer != null && clientRowComparer.IsFinalComparer) {
				isPlinqAble = isPlinqAble && clientRowComparer.IsComparerThreadSafe;
				return (Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, int>)clientRowComparer.RowsComparer;
			}
			bool cellsThreadSafe;
			var cellLarvae = PrepareCells(expressiveSortClient, sortInfos, getterGetter, out cellsThreadSafe);
			isPlinqAble = isPlinqAble && cellsThreadSafe && (clientRowComparer == null || clientRowComparer.IsComparerThreadSafe);
			bool isPlinq = isPlinqAble;
			var cellLarvaeStoraged = cellLarvae.Select((l, i) => new { Larvae = l, FinalGetter = finalCompareValueGetterCacher(l.ComparerArgumentType, l.CompareValuesExtractor, isPlinq, i) }).ToArray();
			bool isLastCellDescending = cellLarvaeStoraged.Length > 0 ? cellLarvaeStoraged[cellLarvaeStoraged.Length - 1].Larvae.IsDescending : false;
			Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, int> comp;
			if(isLastCellDescending)
				comp = (r1, r2) => r2.SourceIndex.CompareTo(r1.SourceIndex);
			else
				comp = (r1, r2) => r1.SourceIndex.CompareTo(r2.SourceIndex);
			for(int i = cellLarvaeStoraged.Length - 1; i >= 0; --i) {
				var info = cellLarvaeStoraged[i];
				Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, int> currentCellComp = CellComparerBuilder.GetInvoker(info.Larvae.ComparerArgumentType)(info.FinalGetter, info.Larvae.Comparer, info.Larvae.IsDescending);
				{
					var capturedPrevComp = comp;
					Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, int> nextComp =
						(r1, r2) => {
							var cmpRes = currentCellComp(r1, r2);
							if(cmpRes != 0)
								return cmpRes;
							return capturedPrevComp(r1, r2);
						};
					comp = nextComp;
				}
			}
			if(clientRowComparer != null) {
				var rowsComparer = (Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, int?>)clientRowComparer.RowsComparer;
				var capturedPrevComp = comp;
				Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, int> nextComp =
					(r1, r2) =>
						rowsComparer(r1, r2) ?? capturedPrevComp(r1, r2);
				comp = nextComp;
			}
			return comp;
		}
		public static Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, int> MakeRowsCompare(IDataControllerSort expressiveSortClient, DataColumnSortInfo[] sortInfos, BaseDataControllerHelper getValueSource, Func<Type, Delegate, bool, int, Delegate> finalCompareValueGetterCacher, ref bool isPlinqAble) {
			Func<DataColumnSortInfo, Type, Delegate> getterGetter
				= (dcsi, targetType) => {
					var fromSourceRowIndexExpression = getValueSource.GetGetRowValue(dcsi.ColumnInfo, targetType);
					Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, int> decondencer =
						c =>
							c.SourceIndex;
					return GenericDelegateHelper.ApplyChain(decondencer, fromSourceRowIndexExpression, targetType);
				};
			var rowsCompare = MakeRowsCompare(expressiveSortClient, sortInfos, getterGetter, finalCompareValueGetterCacher, ref isPlinqAble);
			return rowsCompare;
		}
		class CellCompareLarvae {
			public readonly Type ComparerArgumentType;
			public readonly Delegate CompareValuesExtractor;	
			public readonly Delegate Comparer;  
			public readonly bool IsDescending;
			public CellCompareLarvae(Type _ComparerArgumentType, Delegate _CompareValuesExtractor, Delegate _Comparer, bool _IsDescending) {
				this.ComparerArgumentType = _ComparerArgumentType;
				this.CompareValuesExtractor = _CompareValuesExtractor;
				this.Comparer = _Comparer;
				this.IsDescending = _IsDescending;
			}
		}
		public static Type GetCompareType(DataColumnInfo dci) {
			if(dci.Unbound)
				return typeof(object);
			return NullableHelpers.GetUnBoxedType(dci.Type);
		}
		static CellCompareLarvae[] PrepareCells(IDataControllerSort expressiveSortClient, IEnumerable<DataColumnSortInfo> sortInfos, Func<DataColumnSortInfo, Type, Delegate> getterGetter, out bool isThreadSafe) {
			var columnsData = sortInfos.Select(si => new { SortInfo = si, CellMethodInfo = GetCompareRowsCellInfo(expressiveSortClient, si.ColumnInfo, GetCompareType(si.ColumnInfo), si.SortOrder) }).ToList();
			isThreadSafe = columnsData.All(cd => cd.CellMethodInfo.IsComparerThreadSafe);
			var result = columnsData.Select(cd => Transform(cd.SortInfo, cd.CellMethodInfo, getterGetter)).ToArray();
			return result;
		}
		static CellCompareLarvae Transform(DataColumnSortInfo sortInfo, ExpressiveSortInfo.Cell cellMethodInfo, Func<DataColumnSortInfo, Type, Delegate> getterGetter) {
			Type getterType = cellMethodInfo.RequireBoxedInput ? typeof(object) : GetCompareType(sortInfo.ColumnInfo);
			var valueGetMethod = getterGetter(sortInfo, getterType);
			Type resultType;
			Delegate transformedData;
			if(cellMethodInfo.TransformedType != null) {
				resultType = cellMethodInfo.TransformedType;
				var rowparam = Expression.Parameter(typeof(ExpressiveSortInfo.CondencedAndSourceIndicesPair), "row");
				var lambda = Expression.Lambda(Expression.Invoke(Expression.Constant(cellMethodInfo.Transformator), rowparam, Expression.Invoke(Expression.Constant(valueGetMethod), rowparam)), rowparam);
				transformedData = lambda.Compile();
			} else {
				resultType = getterType;
				transformedData = valueGetMethod;
			}
			return new CellCompareLarvae(resultType, transformedData, cellMethodInfo.ValuesComparer, sortInfo.SortOrder != ColumnSortOrder.Ascending);
		}
		static ExpressiveSortInfo.Cell GetCompareRowsCellInfo(IDataControllerSort expressiveSortClient, DataColumnInfo dataColumnInfo, Type baseExtractorType, ColumnSortOrder order) {
			var sortedByClientMethodInfo = expressiveSortClient != null ? expressiveSortClient.GetSortCellMethodInfo(dataColumnInfo, baseExtractorType, order) : null;
			if(sortedByClientMethodInfo != null && sortedByClientMethodInfo.IsFinalComparer)
				return sortedByClientMethodInfo;
			if(sortedByClientMethodInfo != null && sortedByClientMethodInfo.TransformedType != null)
				throw new NotImplementedException("sortedByClientMethodInfo != null && sortedByClientMethodInfo.TransformedType != null");
			if(dataColumnInfo.CustomComparer != null) {
				if(sortedByClientMethodInfo != null)
					throw new NotImplementedException("dataColumnInfo.CustomComparer != null and sortedByClientMethodInfo != null");
				var capturedComparer = dataColumnInfo.CustomComparer;
				Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, object, object, int> customCompare = (r1, r2, value1, value2) => capturedComparer.Compare(value1, value2);
				return new ExpressiveSortInfo.Cell(true, null, null, customCompare, true, false);
			}
			if(baseExtractorType == typeof(string)) {
				CompareInfo capturedCi = CultureInfo.CurrentCulture.CompareInfo;
				if(sortedByClientMethodInfo == null) {
					Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, string, string, int> strComparer = (r1, r2, value1, value2) => capturedCi.Compare(value1, value2, CompareOptions.None);
					return new ExpressiveSortInfo.Cell(false, null, null, strComparer, true, true);
				} else if(sortedByClientMethodInfo.RequireBoxedInput) {
					var clientComparer = (Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, object, object, int?>)sortedByClientMethodInfo.ValuesComparer;
					Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, object, object, int> boxedStrComparer = (r1, r2, value1, value2) => clientComparer(r1, r2, value1, value2) ?? capturedCi.Compare((string)value1, (string)value2, CompareOptions.None);
					return new ExpressiveSortInfo.Cell(true, null, null, boxedStrComparer, true, sortedByClientMethodInfo.IsComparerThreadSafe);
				} else {
					var clientComparer = (Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, string, string, int?>)sortedByClientMethodInfo.ValuesComparer;
					Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, string, string, int> strComparer = (r1, r2, value1, value2) => clientComparer(r1, r2, value1, value2) ?? capturedCi.Compare(value1, value2, CompareOptions.None);
					return new ExpressiveSortInfo.Cell(false, null, null, strComparer, true, sortedByClientMethodInfo.IsComparerThreadSafe);
				}
			}
			return ExpressiveSortHelpers.GetCompareRowCellInfoTypedCompareHelper.MakeFinalCompare(sortedByClientMethodInfo, baseExtractorType);
		}
		public class ComparisonComparer<T>: IComparer<T> {
			readonly Func<T, T, int> Comparison;
			public ComparisonComparer(Func<T, T, int> _Comparison) {
				this.Comparison = _Comparison;
			}
			public int Compare(T x, T y) {
				return this.Comparison(x, y);
			}
		}
	}
	public static class SummaryValueExpressiveCalculator {
		public static object Calculate(SummaryItemType summaryItemType, IEnumerable valuesEnumerable, Type valuesType, bool ignoreNulls, IComparer customComparer, Func<string[]> exceptionAuxInfoGetter) {
			if(customComparer != null && (summaryItemType == SummaryItemType.Min || summaryItemType == SummaryItemType.Max)) {
				return ProcessUntypedCalculation(summaryItemType, valuesEnumerable, ignoreNulls, customComparer);
			}
			if(valuesType == typeof(object)) {
				if(summaryItemType == SummaryItemType.Count || summaryItemType == SummaryItemType.Min || summaryItemType == SummaryItemType.Max) {
					return ProcessUntypedCalculation(summaryItemType, valuesEnumerable, ignoreNulls, customComparer);
				} else {
					return ProcessUntypedCalculationWithTypedFallback(summaryItemType, valuesEnumerable, ignoreNulls, customComparer, exceptionAuxInfoGetter);
				}
			}
			if(valuesType == typeof(float?)) {
				valuesEnumerable = ((IEnumerable<float?>)valuesEnumerable).Select(v => (v.HasValue && float.IsNaN(v.Value) ? null : v));
			} else if(valuesType == typeof(double?)) {
				valuesEnumerable = ((IEnumerable<double?>)valuesEnumerable).Select(v => (v.HasValue && double.IsNaN(v.Value) ? null : v));
			}
			if(ignoreNulls)
				valuesEnumerable = valuesEnumerable.ApplyWhereNotNull(valuesType);
			switch(summaryItemType) {
				case SummaryItemType.Count:
					if(!ignoreNulls)
						throw new InvalidOperationException("!ignoreNulls is not expected here");
					return valuesEnumerable.ApplyCount(valuesType);
				case SummaryItemType.Sum:
					return DoTypedSum(valuesEnumerable, valuesType, exceptionAuxInfoGetter);
				case SummaryItemType.Max:
					return MinMaxComparableClassApplier.GetInvoker(valuesType)(valuesEnumerable, true);
				case SummaryItemType.Min:
					return MinMaxComparableClassApplier.GetInvoker(valuesType)(valuesEnumerable, false);
				case SummaryItemType.Average:
					return DoTypedAverage(valuesEnumerable, valuesType, exceptionAuxInfoGetter);
				default:
					throw new InvalidOperationException(summaryItemType.ToString() + " is not expected here");
			}
		}
		static object DoTypedAverage(IEnumerable valuesEnumerable, Type valuesType, Func<string[]> exceptionAuxInfoGetter) {
			if(!NullableHelpers.CanAcceptNull(valuesType))
				throw new InvalidOperationException("!NullableHelpers.CanAcceptNull(" + valuesType.FullName + ")");
			Type boxedType = NullableHelpers.GetBoxedType(valuesType);
			switch(DXTypeExtensions.GetTypeCode(boxedType)) {
				case TypeCode.Boolean:
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.Char:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Decimal: try {
						var avg = valuesEnumerable.ApplyCast<decimal?>(valuesType, exceptionAuxInfoGetter).Select(v => v ?? (decimal?)0m).Average();
						return avg;
					} catch(OverflowException) {
						return null;
					}
				case TypeCode.Double:
					return ((IEnumerable<double?>)valuesEnumerable).Select(v=>v??(double?)0.0).Average();
				case TypeCode.Single:
					return ((IEnumerable<float?>)valuesEnumerable).Select(v => v ?? (float?)0.0f).Average();
				case TypeCode.Object: {
						if(boxedType == typeof(TimeSpan)) {
							var totalMS = ((IEnumerable<TimeSpan?>)valuesEnumerable).Select(v => v.HasValue ? v.Value.TotalMilliseconds : (double?)0.0).Average();
							if(totalMS.HasValue)
								return TimeSpan.FromMilliseconds(totalMS.Value);
							else
								return null;
						}
					}
					break;
				case TypeCode.DateTime:
				case TypeCode.String:
				default:
					break;
			}
			return ProcessUntypedCalculation(SummaryItemType.Average, valuesEnumerable, false, null);
		}
		public abstract class MinMaxComparableClassApplier: GenericInvoker<Func<IEnumerable, bool, object>, MinMaxComparableClassApplier.Impl<object>> {
			public class Impl<T>: MinMaxComparableClassApplier {
				static object DoMax(IEnumerable<T> src) {
					Func<T, T, bool> isBetter;  
					if(typeof(IComparable).IsAssignableFrom(NullableHelpers.GetBoxedType(typeof(T)))) {
						var capturedComparer = Comparer<T>.Default;
						isBetter = (current, candidate) => {
							try {
								return capturedComparer.Compare(current, candidate) < 0;
							} catch {
								return false;
							}
						};
					} else if(Nullable.GetUnderlyingType(typeof(T)) == null) {
						var capturedComparer = Comparer<T>.Default;
						isBetter = (current, candidate) => {
							if(!(candidate is IComparable))
								return false;
							try {
								return capturedComparer.Compare(current, candidate) < 0;
							} catch {
								return false;
							}
						};
					} else {
						return null;
					}
					T best = default(T);
					if(best != null)
						throw new InvalidOperationException("default(T) != null");
					bool first = true;
					foreach(T t in src) {
						if(first) {
							best = t;
							first = false;
						} else if(isBetter(best, t)) {
							best = t;
						}
					}
					return best;
				}
				static object DoMin(IEnumerable<T> src) {
					Func<T, T, bool> isBetter;  
					if(typeof(IComparable).IsAssignableFrom(NullableHelpers.GetBoxedType(typeof(T)))) {
						var capturedComparer = Comparer<T>.Default;
						isBetter = (current, candidate) => {
							try {
								return capturedComparer.Compare(current, candidate) > 0;
							} catch {
								return false;
							}
						};
					} else if(Nullable.GetUnderlyingType(typeof(T)) == null) {
						var capturedComparer = Comparer<T>.Default;
						isBetter = (current, candidate) => {
							if(!(candidate is IComparable))
								return false;
							try {
								return capturedComparer.Compare(current, candidate) > 0;
							} catch {
								return false;
							}
						};
					} else {
						return null;
					}
					T best = default(T);
					if(best != null)
						throw new InvalidOperationException("default(T) != null");
					Func<T, bool> isNull = GenericDelegateHelper.MakeNullCheck<T>();
					bool first = true;
					foreach(T t in src) {
						if(isNull(t))
							return null;
						if(first) {
							best = t;
							first = false;
						} else if(isBetter(best, t)) {
							best = t;
						}
					}
					return best;
				}
				static object DoMinMax(IEnumerable<T> src, bool isMax) {
					if(isMax)
						return DoMax(src);
					else
						return DoMin(src);
				}
				protected override Func<IEnumerable, bool, object> CreateInvoker() {
					return (src, isMax) => DoMinMax((IEnumerable<T>)src, isMax);
				}
			}
		}
		static object DoTypedSum(IEnumerable valuesEnumerable, Type valuesType, Func<string[]> exceptionAuxInfoGetter) {
			if(!NullableHelpers.CanAcceptNull(valuesType))
				throw new InvalidOperationException("!NullableHelpers.CanAcceptNull(" + valuesType.FullName + ")");
			Type boxedType = NullableHelpers.GetBoxedType(valuesType);
			switch(DXTypeExtensions.GetTypeCode(boxedType)) {
				case TypeCode.Boolean:
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.Char:
				case TypeCode.Int16:
				case TypeCode.Int32: {
					var sum = valuesEnumerable.ApplyCast<long?>(valuesType, exceptionAuxInfoGetter).Sum();
						if(!sum.HasValue)
							return null;
						else if(sum.Value > int.MaxValue || sum.Value < int.MinValue)
							return sum.Value;
						else
							return (int)sum.Value;
					}
				case TypeCode.Int64: {
						using(var enumer = ((IEnumerable<long?>)valuesEnumerable).Where(v => v.HasValue).Select(v => v.Value).GetEnumerator()) {
							long? total = null;
							while(enumer.MoveNext()) {
								long current = enumer.Current;
								if(!total.HasValue) {
									total = current;
									continue;
								}
								try {
									total = checked(total.Value + current);
								} catch(OverflowException) {
									decimal overlownTotal = (decimal)total.Value + (decimal)current;
									while(enumer.MoveNext()) {
										overlownTotal += enumer.Current;
									}
									return overlownTotal;
								}
							}
							return total;
						}
					}
				case TypeCode.UInt16:
				case TypeCode.UInt32: {
						var uSum = valuesEnumerable.ApplyCast<ulong?>(valuesType, exceptionAuxInfoGetter).Where(v => v.HasValue).Select(v => v.Value);
						ulong? total = null;
						foreach(var v in uSum) {
							if(!total.HasValue) {
								total = v;
								continue;
							}
							total = total.Value + v;
						}
						if(!total.HasValue)
							return null;
						if(total.Value > uint.MaxValue)
							return total.Value;
						return (uint)total.Value;
					}
				case TypeCode.UInt64: {
						using(var enumer = ((IEnumerable<ulong?>)valuesEnumerable).Where(v => v.HasValue).Select(v => v.Value).GetEnumerator()) {
							ulong? total = null;
							while(enumer.MoveNext()) {
								ulong current = enumer.Current;
								if(!total.HasValue) {
									total = current;
									continue;
								}
								try {
									total = checked(total.Value + current);
								} catch(OverflowException) {
									decimal overlownTotal = (decimal)total.Value + (decimal)current;
									while(enumer.MoveNext()) {
										overlownTotal += enumer.Current;
									}
									return overlownTotal;
								}
							}
							return total;
						}
					}
				case TypeCode.Decimal:
					try {
						return ((IEnumerable<decimal?>)valuesEnumerable).Sum();
					} catch {
						return null;
					}
				case TypeCode.Double:
					return ((IEnumerable<double?>)valuesEnumerable).Sum();
				case TypeCode.Single:
					return ((IEnumerable<Single?>)valuesEnumerable).Sum();
				case TypeCode.Object: {
						if(boxedType == typeof(TimeSpan)) {
							var totalMS = ((IEnumerable<TimeSpan?>)valuesEnumerable).Select(v => v.HasValue ? v.Value.TotalMilliseconds : (double?)null).Sum();
							if(totalMS.HasValue)
								return TimeSpan.FromMilliseconds(totalMS.Value);
							else
								return null;
						}
					}
					break;
				case TypeCode.DateTime:
				case TypeCode.String:
				default:
					break;
			}
			return ProcessUntypedCalculation(SummaryItemType.Sum, valuesEnumerable, false, null);
		}
		static object ProcessUntypedCalculationWithTypedFallback(SummaryItemType summaryItemType, IEnumerable iEnumerable, bool ignoreNulls, IComparer customComparer, Func<string[]> exceptionAuxInfoGetter) {
			var values = iEnumerable.Cast<object>().ToArray();
			Type valuesType = TryDetermineUntypedType(values);
			if(valuesType == null)
				return null;
			if(valuesType != typeof(object)) {
				var typeCode = DXTypeExtensions.GetTypeCode(valuesType);
				if(typeCode != TypeCode.DateTime && typeCode != TypeCode.String && (typeCode != TypeCode.Object || valuesType == typeof(TimeSpan))) {
					var unboxedType = NullableHelpers.GetUnBoxedType(valuesType);
					return Calculate(summaryItemType, values.ApplyCast(unboxedType, exceptionAuxInfoGetter), unboxedType, ignoreNulls, customComparer, exceptionAuxInfoGetter);
				}
			}
			return ProcessUntypedCalculation(summaryItemType, values, ignoreNulls, customComparer);
		}
		static object ProcessUntypedCalculation(SummaryItemType summaryItemType, IEnumerable values, bool ignoreNulls, IComparer customComparer) {
			var typedEnumerable = values.Cast<object>();
			var nanFilteredEnumerable = typedEnumerable.Select(
				x => {
					if(x is double && double.IsNaN((double)x))
						return null;
					else if(x is float && float.IsNaN((float)x))
						return null;
					else
						return x;
				}
				);
			IEnumerable<object> preparedEnumerable;
			if(ignoreNulls)
				preparedEnumerable = nanFilteredEnumerable.Where(x => x != null);
			else
				preparedEnumerable = nanFilteredEnumerable;
			switch(summaryItemType){
				case SummaryItemType.Count:
					if(!ignoreNulls)
						throw new InvalidOperationException("!ignoreNulls is not expected here");
					return preparedEnumerable.Count();
				case SummaryItemType.Max:
					return DoUntypedMinMax(true, preparedEnumerable, customComparer);
				case SummaryItemType.Min:
					return DoUntypedMinMax(false, preparedEnumerable, customComparer);
				case SummaryItemType.Sum: {
						int cnt;
						return DoUntypedSumWithCount(preparedEnumerable, out cnt);
					}
				case SummaryItemType.Average: {
						int cnt;
						object acc = DoUntypedSumWithCount(preparedEnumerable, out cnt);
						if(acc == null)
							return null;
						if(acc is decimal)
							return ((decimal)acc) / cnt;
						if(acc is double)
							return ((double)acc) / cnt;
						if(acc is float)
							return ((float)acc) / cnt;
						throw new InvalidOperationException("unexpected acc type: " + acc.GetType().FullName);
					}
			}
			throw new NotSupportedException(summaryItemType.ToString());
		}
		static object DoUntypedSumWithCount(IEnumerable<object> preparedEnumerable, out int cnt) {
			cnt = 0;
			object acc = null;
			foreach(var o in preparedEnumerable) {
				++cnt;
				if(o == null)
					continue;
				object obj = o;
				if(obj is TimeSpan)
					obj = ((TimeSpan)obj).TotalMilliseconds;
				if(!(obj is IConvertible))
					continue;
				if(obj is string) {
					if(acc is double) {
						double dummy;
						if(double.TryParse((string)obj, out dummy))
							obj = dummy;
						else
							continue;
					} else if(acc is float) {
						float dummy;
						if(float.TryParse((string)obj, out dummy))
							obj = dummy;
						else
							continue;
					} else {
						decimal dummy;
						if(decimal.TryParse((string)obj, out dummy))
							obj = dummy;
						else
							continue;
					}
				}
				if(acc is decimal) {
					try {
						acc = (decimal)acc + Convert.ToDecimal(obj);
					} catch {
					}
				} else if(acc == null) {
					if(obj is double || obj is float || obj is decimal) {
						acc = obj;
					} else {
						try {
							acc = Convert.ToDecimal(obj);
						} catch {
						}
					}
				} else if(acc is double) {
					if(obj is decimal) {
						acc = (decimal)(double)acc + (decimal)obj;
					} else {
						try {
							acc = (double)acc + Convert.ToDouble(obj);
						} catch {
						}
					}
				} else if(acc is float) {
					if(obj is decimal) {
						acc = (decimal)(float)acc + (decimal)obj;
					} else if(obj is double) {
						acc = (double)(float)acc + (double)obj;
					} else {
						try {
							acc = (float)acc + Convert.ToSingle(obj);
						} catch {
						}
					}
				} else {
					throw new InvalidOperationException("unexpected acc type: " + acc.GetType().FullName);
				}
			}
			return acc;
		}
		static object DoUntypedMinMax(bool isMax, IEnumerable<object> preparedEnumerable, IComparer customComparer) {
			IComparer comparer = customComparer ?? Comparer.Default;
			object res = null;
			bool resSet = false;
			foreach(var o in preparedEnumerable) {
				if(resSet) {
					if(customComparer == null && !(o is IComparable)) {
						if(!isMax)
							return null;
						continue;
					}
					var cmp = comparer.Compare(res, o);
					if(isMax && cmp < 0 || !isMax && cmp > 0)
						res = o;
				} else {
					if(customComparer == null && !(o is IComparable)) {
						if(!isMax)
							return null;
					} else {
						try {
							comparer.Compare(o, o);
						} catch {
							continue;
						}
					}
					res = o;
					resSet = true;
				}
			}
			return res;
		}
		static Type TryDetermineUntypedType(object[] values) {
			Type accType = null;
			foreach(var type in values.Where(v => v != null).Select(v => v.GetType())) {
				if(accType == type) {
				} else if(accType == null || type.IsAssignableFrom(accType)) {
					accType = type;
					if(accType == typeof(object))
						break;
				} else if(accType.IsAssignableFrom(type)) {
				} else {
					accType = typeof(object);
					break;
				}
			}
			return accType;
		}
	}
}
namespace DevExpress.Data.Filtering {
	public sealed class CriteriaColumnAffinityResolver : IClientCriteriaVisitor<OperandProperty> {
		CriteriaColumnAffinityResolver() { }
		static CriteriaColumnAffinityResolver Instance = new CriteriaColumnAffinityResolver();
		OperandProperty IClientCriteriaVisitor<OperandProperty>.Visit(OperandProperty theOperand) {
			return theOperand;
		}
		OperandProperty IClientCriteriaVisitor<OperandProperty>.Visit(AggregateOperand theOperand) {
			return null;
		}
		OperandProperty IClientCriteriaVisitor<OperandProperty>.Visit(JoinOperand theOperand) {
			return null;
		}
		OperandProperty ICriteriaVisitor<OperandProperty>.Visit(FunctionOperator theOperator) {
			if (theOperator.Operands.Count == 1) {
				switch (theOperator.OperatorType) {
					case FunctionOperatorType.IsOutlookIntervalBeyondThisYear:
					case FunctionOperatorType.IsOutlookIntervalLaterThisYear:
					case FunctionOperatorType.IsOutlookIntervalLaterThisMonth:
					case FunctionOperatorType.IsOutlookIntervalLaterThisWeek:
					case FunctionOperatorType.IsOutlookIntervalNextWeek:
					case FunctionOperatorType.IsOutlookIntervalTomorrow:
					case FunctionOperatorType.IsOutlookIntervalToday:
					case FunctionOperatorType.IsOutlookIntervalYesterday:
					case FunctionOperatorType.IsOutlookIntervalEarlierThisWeek:
					case FunctionOperatorType.IsOutlookIntervalLastWeek:
					case FunctionOperatorType.IsOutlookIntervalEarlierThisMonth:
					case FunctionOperatorType.IsOutlookIntervalEarlierThisYear:
					case FunctionOperatorType.IsOutlookIntervalPriorThisYear:
					case FunctionOperatorType.IsNullOrEmpty:
						return Process(theOperator.Operands[0]);
				}
			}
			else if (theOperator.Operands.Count == 2) {
				switch (theOperator.OperatorType) {
					case FunctionOperatorType.StartsWith:
					case FunctionOperatorType.EndsWith:
					case FunctionOperatorType.Contains:
						return Process(theOperator.Operands[0]);
				}
			}
			else if (LikeCustomFunction.IsBinaryCompatibleLikeFunction(theOperator)) {
				return Process(theOperator.Operands[1]);
			}
			return null;
		}
		OperandProperty ICriteriaVisitor<OperandProperty>.Visit(OperandValue theOperand) {
			return null;
		}
		OperandProperty ICriteriaVisitor<OperandProperty>.Visit(GroupOperator theOperator) {
			OperandProperty prop = null;
			foreach (CriteriaOperator op in theOperator.Operands) {
				OperandProperty affProp = Process(op);
				if (ReferenceEquals(prop, null)) {
					prop = affProp;
				}
				else if (prop.PropertyName != affProp.PropertyName) {
					return null;
				}
			}
			return prop;
		}
		OperandProperty ICriteriaVisitor<OperandProperty>.Visit(InOperator theOperator) {
			return Process(theOperator.LeftOperand);
		}
		OperandProperty ICriteriaVisitor<OperandProperty>.Visit(UnaryOperator theOperator) {
			return Process(theOperator.Operand);
		}
		OperandProperty ICriteriaVisitor<OperandProperty>.Visit(BinaryOperator theOperator) {
			return Process(theOperator.LeftOperand);
		}
		OperandProperty ICriteriaVisitor<OperandProperty>.Visit(BetweenOperator theOperator) {
			return Process(theOperator.TestExpression);
		}
		OperandProperty Process(CriteriaOperator op) {
			if (ReferenceEquals(op, null))
				return new OperandProperty();
			OperandProperty result = op.Accept(this);
			if (ReferenceEquals(result, null))
				return new OperandProperty();
			return result;
		}
		public static OperandProperty GetAffinityColumn(CriteriaOperator op) {
			return Instance.Process(op);
		}
		public static IDictionary<OperandProperty, CriteriaOperator> SplitByColumns(CriteriaOperator op) {
			Dictionary<OperandProperty, CriteriaOperator> result = new Dictionary<OperandProperty, CriteriaOperator>();
			if (!ReferenceEquals(op, null)) {
				OperandProperty affine = GetAffinityColumn(op);
				if (affine.PropertyName != null && affine.PropertyName.Length > 0) {
					result.Add(affine, op);
				}
				else {
					GroupOperator group = op as GroupOperator;
					if (!ReferenceEquals(group, null) && group.OperatorType == GroupOperatorType.And) {
						foreach (CriteriaOperator lop in group.Operands) {
							OperandProperty laffine = GetAffinityColumn(lop);
							CriteriaOperator affineCriteria;
							if (result.TryGetValue(laffine, out affineCriteria)) {
								affineCriteria = GroupOperator.And(affineCriteria, lop);
							}
							else {
								affineCriteria = lop;
							}
							if (!ReferenceEquals(affineCriteria, null))
								result[laffine] = affineCriteria;
						}
					}
					else {
						result.Add(affine, op);
					}
				}
			}
			return result;
		}
	}
}
