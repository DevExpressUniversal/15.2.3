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
using System.ComponentModel;
using DevExpress.Utils.Extensions.Helpers;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Utils;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System;
namespace DevExpress.XtraPivotGrid {
	public interface ISummaryFilterController : IFilterController, ISummaryFilter {
		FormatInfo GetFormatInfo(object value);
		PivotSummaryInterval GetSummaryInterval(bool visibleIntervalOnly);
		IList<PivotGridFieldBase> GetAreaFields(bool isColumn);
		PivotGridFieldBase GetLevelField(bool isColumn);
		void SetLevelFields(PivotGridFieldBase rowField, PivotGridFieldBase columnField);
		event EventHandler Updated;
		void BeginUpdate();
		void EndUpdate();
		void CancelUpdate();
	}
	public interface IFilterController {
		bool CanAccept { get; }
		void ApplyFilter();
	}
	public interface ISummaryFilter {
		object StartValue { get; set; }
		object EndValue { get; set; }
		PivotSummaryFilterMode Mode { get; set; }
		PivotSummaryFilterValidity Validity { get; }
		void Clear();
	}
	public class PivotSummaryFilterController : ISummaryFilterController {
		readonly PivotGridFieldBase field;
		readonly PivotSummaryFilterInternal filter;
		public event EventHandler Updated;
		int lockCount;
		public PivotSummaryFilterController(PivotGridFieldBase field) {
			this.field = field;
			this.filter = new PivotSummaryFilterInternal(field);
		}
		PivotGridData Data { get { return field.Data; } }
		PivotSummaryFilter Filter { get { return filter; } }
		PivotGridFieldBase GetFieldFromData(PivotGridFieldBase field) {
			if(Data == null || !Data.Fields.Contains(field)) return null;
			return field;
		}
		IList<PivotGridFieldBase> GetAreaFields(bool isColumn) {
			if(Data == null) return new List<PivotGridFieldBase>(0);
			return Data.GetFieldsByArea(isColumn ? PivotArea.ColumnArea : PivotArea.RowArea, false);
		}
		PivotGridFieldBase RowField {
			get { return GetFieldFromData(Filter.RowField); }
		}
		PivotGridFieldBase ColumnField {
			get { return GetFieldFromData(Filter.ColumnField); }
		}
		#region ISummaryFilterController Members
		FormatInfo ISummaryFilterController.GetFormatInfo(object value) {
			if(Data == null) return field.CellFormat;
			PivotFieldItemBase fieldItem = Data.GetFieldItem(field);
			bool isGrandTotal = IsGrandTotal,
				isTotal = !isGrandTotal && IsTotal;
			value = value ?? (Filter.StartValueInternal ?? Filter.EndValueInternal);
			FormatInfo fieldFormat = PivotGridCellItem.GetFormatInfo(() => value, fieldItem, null, isTotal, isGrandTotal);
			if(fieldFormat != null && !string.IsNullOrEmpty(fieldFormat.FormatString))
				return fieldFormat;
			FormatInfo roundedFormat = PivotDisplayValueFormatter.GetRoundedFormat(value);
			if(fieldFormat != null)
				roundedFormat.FormatType = fieldFormat.FormatType;
			return roundedFormat;
		}
		bool IsTotal {
			get {
				if(Filter.Mode == PivotSummaryFilterMode.LastLevel)
					return false;
				PivotGridFieldBase lastRowField = GetLastLevelField(false),
					lastColumnField = GetLastLevelField(true);
				return (Filter.Mode == PivotSummaryFilterMode.SpecificLevel) && (RowField != lastRowField || ColumnField != lastColumnField);
			}
		}
		bool IsGrandTotal {
			get {
				if(GetAreaFields(false).Count == 0 || GetAreaFields(true).Count == 0)
					return true;
				return (Filter.Mode == PivotSummaryFilterMode.SpecificLevel) && (RowField == null || ColumnField == null);
			}
		}
		PivotSummaryInterval ISummaryFilterController.GetSummaryInterval(bool visibleIntervalOnly) {
			if(Data == null) return null;
			return Data.GetSummaryInterval(field, visibleIntervalOnly, this.filter.Mode == PivotSummaryFilterMode.SpecificLevel, RowField, ColumnField);
		}
		IList<PivotGridFieldBase> ISummaryFilterController.GetAreaFields(bool isColumn) {
			return GetAreaFields(isColumn);
		}
		PivotGridFieldBase ISummaryFilterController.GetLevelField(bool isColumn) {
			if(Filter.Mode == PivotSummaryFilterMode.LastLevel)
				return GetLastLevelField(isColumn);
			PivotGridFieldBase field = Filter.GetLevelField(isColumn);
			return GetFieldFromData(field);
		}
		PivotGridFieldBase GetLastLevelField(bool isColumn) {
			IList<PivotGridFieldBase> fields = GetAreaFields(isColumn);
			return (fields.Count != 0) ? fields[fields.Count - 1] : null;
		}
		void ISummaryFilterController.SetLevelFields(PivotGridFieldBase rowField, PivotGridFieldBase columnField) {
			SetFilterField(false, rowField);
			SetFilterField(true, columnField);
			RaiseUpdated();
		}
		void SetFilterField(bool isColumn, PivotGridFieldBase field) {
			PivotGridFieldBase correctedField = GetFieldFromData(field);
			Filter.SetLevelField(isColumn, correctedField);
		}
		bool IFilterController.CanAccept {
			get {
				if(Filter.Mode == PivotSummaryFilterMode.SpecificLevel && RowField == null && ColumnField == null)
					return false;
				return Filter.Validity == PivotSummaryFilterValidity.Valid;
			}
		}
		void IFilterController.ApplyFilter() {
			this.field.SummaryFilter.Apply(Filter);
		}
		object ISummaryFilter.StartValue {
			get { return Filter.StartValue; }
			set {
				if(Filter.StartValue == value) return;
				Filter.StartValue = value;
				RaiseUpdated();
			}
		}
		object ISummaryFilter.EndValue {
			get { return Filter.EndValue; }
			set {
				if(Filter.EndValue == value) return;
				Filter.EndValue = value;
				RaiseUpdated();
			}
		}
		PivotSummaryFilterMode ISummaryFilter.Mode {
			get { return Filter.Mode; }
			set {
				if(Filter.Mode == value) return;
				Filter.Mode = value;
				ResetFilterFields();
				RaiseUpdated();
			}
		}
		PivotSummaryFilterValidity ISummaryFilter.Validity {
			get { return Filter.Validity; }
		}
		void ISummaryFilter.Clear() {
			Filter.Clear();
			RaiseUpdated();
		}
		void ISummaryFilterController.BeginUpdate() {
			this.lockCount++;
		}
		void ISummaryFilterController.EndUpdate() {
			this.lockCount--;
			if(this.lockCount < 0)
				throw new InvalidOperationException("EndUpdate has been called without a preceding call to BeginUpdate");
			RaiseUpdated();
		}
		void ISummaryFilterController.CancelUpdate() {
			this.lockCount--;
			if(this.lockCount < 0)
				throw new InvalidOperationException("CancelUpdate has been called without a preceding call to BeginUpdate");
		}
		void ResetFilterFields() {
			Filter.ColumnField = Filter.Mode == PivotSummaryFilterMode.SpecificLevel ? GetLastLevelField(true) : null;
			Filter.RowField = Filter.Mode == PivotSummaryFilterMode.SpecificLevel ? GetLastLevelField(false) : null;
		}
		void RaiseUpdated() {
			if(this.lockCount != 0) return;
			Updated.SafeRaise(this, EventArgs.Empty);
		}
		#endregion
	}
	public static class PivotDisplayValueFormatter {
		readonly static Dictionary<Type, string> masks = new Dictionary<Type, string>();
		readonly static List<Type> continuousTypes = new List<Type>(3) { typeof(double), typeof(float), typeof(decimal) };
		readonly static List<Type> discreteTypes = new List<Type>(8) { typeof(int), typeof(short), typeof(uint), typeof(ushort),
			typeof(byte), typeof(sbyte), typeof(long), typeof(ulong) };
		static PivotDisplayValueFormatter() {
			foreach(Type type in continuousTypes)
				masks[type] = "f2";
			foreach(Type type in discreteTypes)
				masks[type] = "n0";
		}
		public static FormatInfo GetRoundedFormat(object obj) {
			if(obj == null) return new FormatInfo();
			Type type = obj.GetType();
			FormatInfo roundFormatInfo = new FormatInfo() {
				FormatType = FormatType.Numeric,
				FormatString = "{0:" + GetEditMask(type) + "}"
			};
			return roundFormatInfo;
		}
		public static string GetEditMask(Type type) {
			string mask = string.Empty;
			masks.TryGetValue(type, out mask);
			return mask;
		}
		public static string Format(FormatInfo formatInfo, object obj) {
			return formatInfo.GetDisplayText(obj);
		}
		public static bool IsDiscrete(Type type) {
			return discreteTypes.Contains(type);
		}
	}
	public class PivotSummaryInterval {
		public static readonly PivotSummaryInterval Empty = new PivotSummaryInterval();
		readonly List<PivotCrossValue> values;
		IList<int> countDistribution;
		IList<PivotCrossValues> valuesDistribution;
		object startValue;
		object endValue;
		int valueCount;
		internal PivotSummaryInterval() {
			this.values = new List<PivotCrossValue>();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public IList<int> Distribution {
			get { return countDistribution; }
		}
		int ValueCount {
			get { return valueCount; }
			set { valueCount = value; }
		}
		IList<PivotCrossValues> ValuesDistribution {
			get { return valuesDistribution; }
		}
		public object StartValue {
			get { return startValue; }
			private set { startValue = value; }
		}
		public object EndValue {
			get { return endValue; }
			private set { endValue = value; }
		}
		public bool IsEmpty {
			get {
				return (StartValue == null && EndValue == null) || Distribution == null || Distribution.Count == 0;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsDiscrete { 
			get { return PivotDisplayValueFormatter.IsDiscrete(DataType); } 
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Type DataType {
			get { return StartValue.GetType(); }
		}
		internal void Calculate(PivotSummaryInterval parent, IComparer<object> comparer, int count) {
			if(ValueCount == 0) return;
			if(parent == null) {
				StartValue = EndValue = null;
				CalculateBounds(comparer);
				CalculateDistribution(count);
			} else {
				StartValue = parent.StartValue;
				EndValue = parent.EndValue;
				CalculateDistribution(parent);
			}
			this.values.Clear();
		}
		internal void AddValue(PivotCrossValue value) {
			this.values.Add(value);
			ValueCount++;
		}
#if DEBUGTEST
		internal void AddCrossValue(object value) {
			PivotCrossValue crossValue = new PivotCrossValue(null, null, value);
			AddValue(crossValue);
		}
#endif
		void CalculateBounds(IComparer<object> comparer) {
			foreach(PivotCrossValue crossValue in this.values) {
				object value = crossValue.Value;
				int resStart = comparer.Compare(StartValue, value),
					resEnd = comparer.Compare(EndValue, value);
				if(resStart > 0 || StartValue == null)
					StartValue = value;
				if(resEnd < 0 || EndValue == null)
					EndValue = value;
			}
		}
		void CalculateDistribution(PivotSummaryInterval parentInterval) {
			this.countDistribution = new int[parentInterval.ValuesDistribution.Count];
			if(ValueCount == 0) return;
			if(ValueCount == parentInterval.ValueCount) {
				for(int i = 0; i < this.countDistribution.Count; i++) {
					this.countDistribution[i] = parentInterval.Distribution[i];
				}
			} else {
				for(int i = 0; i < this.countDistribution.Count; i++) {
					if(parentInterval.ValuesDistribution[i] == null) continue;
					this.countDistribution[i] = parentInterval.ValuesDistribution[i].GetInnerCount(this.values);
				}
			}
		}
		void CalculateDistribution(int count) {
			decimal leftBound = Convert.ToDecimal(StartValue),
				rightBound = Convert.ToDecimal(EndValue);
			decimal interval = rightBound - leftBound;
			if(interval != 0 && interval < count && IsDiscrete)
				count = Convert.ToInt32(interval) * 2;
			CreateDistributions(count);
			if(interval == 0) {
				countDistribution[count - 1] = ValueCount;
				return;
			}
			foreach(PivotCrossValue crossValue in this.values) {
				decimal decimalValue = Convert.ToDecimal(crossValue.Value);
				double intervalCount = Convert.ToDouble((count * (decimalValue - leftBound)) / interval);
				int index = Convert.ToInt32(Math.Floor(intervalCount));
				if(index >= count) index = count - 1;
				AddToDistributions(index, crossValue);
			}
		}
		void CreateDistributions(int count) {
			this.countDistribution = new int[count];
			this.valuesDistribution = new PivotCrossValues[count];
		}
		void AddToDistributions(int index, PivotCrossValue crossValue) {
			this.countDistribution[index]++;
			if(this.valuesDistribution[index] == null)
				this.valuesDistribution[index] = new PivotCrossValues();
			this.valuesDistribution[index].Add(crossValue);
		}
	}
	internal class PivotCrossValues {
		IList<PivotCrossValue> values;
		public void Add(PivotCrossValue value) {
			Values.Add(value);
		}
		public int GetInnerCount(IList<PivotCrossValue> crossValues) {
			int count = 0;
			foreach(PivotCrossValue value in this.values) {
				foreach(PivotCrossValue crossValue in crossValues) {
					if(object.Equals(value, crossValue)) {
						count++;
						break;
					}
				}
			}
			return count;
		}
		IList<PivotCrossValue> Values {
			get {
				if(values == null)
					values = new List<PivotCrossValue>();
				return values; 
			}
		}
	}
	internal class PivotCrossValue : IEquatable<PivotCrossValue> {
		internal static PivotCrossValue Create(object[] rowBranch, object[] columnBranch, object value) {
			return new PivotCrossValue(rowBranch, columnBranch, value);
		}
		static bool BranchesEqual(object[] branch1, object[] branch2) {
			if(branch1 == null && branch2 == null)
				return true;
			if(branch1 == null || branch2 == null)
				return false;
			int count = branch1.Length;
			for(int i = 0; i < count; i++) {
				if(!object.Equals(branch1[i], branch2[i]))
					return false;
			}
			return true;
		}
		readonly object[] rowBranch;
		readonly object[] columnBranch;
		readonly int rowBranchHash;
		readonly int columnBranchHash;
		readonly object value;
		internal PivotCrossValue(object[] row, object[] column, object value) {
			this.rowBranch = row;
			this.columnBranch = column;
			this.value = value;
			this.rowBranchHash = ComputeArrayHash(this.rowBranch);
			this.columnBranchHash = ComputeArrayHash(this.columnBranch);
		}
		internal object Value { get { return value; } }
		static int ComputeArrayHash(object[] values) {
			if(values == null) return -1;
			int hash = values.Length;
			foreach(object value in values) {
				if(value == null) continue;
				hash = unchecked(hash * 17 + value.GetHashCode());
			}
			return hash;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			PivotCrossValue other = obj as PivotCrossValue;
			if(other == null) return false;
			if(this.rowBranchHash != other.rowBranchHash || this.columnBranchHash != other.columnBranchHash) return false;
			return BranchesEqual(rowBranch, other.rowBranch) && BranchesEqual(columnBranch, other.columnBranch);
		}
		bool IEquatable<PivotCrossValue>.Equals(PivotCrossValue other) {
			return this.Equals(other);
		}
	}
	internal class PivotCrossLevel : IEquatable<PivotCrossLevel>, ICloneable {
		internal static PivotCrossLevel Create(int rowLevel, int columnLevel) {
			return new PivotCrossLevel(rowLevel, columnLevel);
		}
		readonly int column, row;
		internal PivotCrossLevel(int rowIndex, int columnIndex) {
			this.row = rowIndex;
			this.column = columnIndex;
		}
		public override int GetHashCode() {
			return Column ^ Row;
		}
		internal int Column { get { return column; } }
		internal int Row { get { return row; } }
		internal PivotCrossLevel Clone() {
			return new PivotCrossLevel(Row, Column);
		}
		object ICloneable.Clone() {
			return Clone();
		}
		bool IEquatable<PivotCrossLevel>.Equals(PivotCrossLevel other) {
			return row == other.Row && column == other.Column;
		}
	}
	internal class PivotSummaryIntervalsCache {
		readonly PivotSummaryIntervalCache summaryIntervalCache;
		readonly PivotSummaryIntervalCache visibleSummaryIntervalCache;
		internal PivotSummaryIntervalsCache() {
			this.summaryIntervalCache = new PivotSummaryIntervalCache();
			this.visibleSummaryIntervalCache = new PivotSummaryIntervalCache();
		}
		internal void Clear() {
			this.summaryIntervalCache.Clear();
			this.visibleSummaryIntervalCache.Clear();
		}
		internal bool TryGetValue(bool visible, int rowLevel, int columnLevel, out PivotSummaryInterval interval) {
			return GetCache(visible).TryGetValue(rowLevel, columnLevel, out interval);
		}
		internal void AddValue(bool visible, object[] rowBranch, object[] columnBranch, object value) {
			GetCache(visible).AddValue(rowBranch, columnBranch, value);
		}
		internal void Calculate(bool visible, int count, IComparer<object> comparer) {
			PivotSummaryIntervalCache parent = visible ? summaryIntervalCache : null;
			GetCache(visible).CalculateIntervals(parent, count, comparer);
		}
		PivotSummaryIntervalCache GetCache(bool visible) {
			return visible ? visibleSummaryIntervalCache : summaryIntervalCache;
		}
	}
	internal class PivotSummaryIntervalCache {
		Dictionary<PivotCrossLevel, PivotSummaryInterval> summaryIntervals;
		internal void AddValue(object[] rowBranch, object[] columnBranch, object value) {
			int rowLevel = rowBranch != null ? rowBranch.Length - 1 : -1,
				columnLevel = columnBranch != null ? columnBranch.Length - 1 : -1;
			PivotCrossLevel crossLevel = PivotCrossLevel.Create(rowLevel, columnLevel);
			PivotCrossValue crossValue = PivotCrossValue.Create(rowBranch, columnBranch, value);
			this[crossLevel].AddValue(crossValue);
		}
		internal PivotSummaryInterval this[PivotCrossLevel level] {
			get {
				PivotSummaryInterval interval;
				if(!SummaryIntervals.TryGetValue(level, out interval)) {
					interval = new PivotSummaryInterval();
					SummaryIntervals.Add(level, interval);
				}
				return interval;
			}
		}
		internal void Clear() {
			if(summaryIntervals != null)
				summaryIntervals.Clear();
		}
		internal bool TryGetValue(int rowLevel, int columnLevel, out PivotSummaryInterval interval) {
			interval = null;
			if(this.summaryIntervals == null) return false;
			PivotCrossLevel level = PivotCrossLevel.Create(rowLevel, columnLevel);
			return SummaryIntervals.TryGetValue(level, out interval);
		}
		internal void CalculateIntervals(PivotSummaryIntervalCache parent, int count, IComparer<object> comparer) {
			foreach(KeyValuePair<PivotCrossLevel, PivotSummaryInterval> pair in SummaryIntervals) {
				PivotSummaryInterval parentInterval = (parent == null) ? null : parent[pair.Key];
				pair.Value.Calculate(parentInterval, comparer, count);
			}
		}
		Dictionary<PivotCrossLevel, PivotSummaryInterval> SummaryIntervals {
			get {
				if(summaryIntervals == null)
					summaryIntervals = new Dictionary<PivotCrossLevel, PivotSummaryInterval>();
				return summaryIntervals;
			}
		}
	}
}
