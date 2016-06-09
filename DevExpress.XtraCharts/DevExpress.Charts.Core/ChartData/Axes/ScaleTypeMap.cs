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
using DevExpress.Utils;
using System.Reflection;
using System.Collections;
namespace DevExpress.Charts.Native {
	public enum Scale {
		Qualitative,
		Numerical,
		DateTime,
		Auto
	}
	public enum ActualScaleType {
		Numerical = Scale.Numerical,
		DateTime = Scale.DateTime,
		Qualitative = Scale.Qualitative
	}
	public interface IPriorScaleMap {
		double NativeToInternal(object value);
		object InternalToNative(double value);
		void UpdateMin(double min);
	}
	public struct RangeValue {
		static RangeValue empty = new RangeValue(Double.NaN, Double.NaN);
		public static RangeValue Empty { get { return empty; } }
		double value1;
		double value2;
		public bool IsEmpty { get { return double.IsNaN(value1) && double.IsNaN(value2); } }
		public double Min { get { return Math.Min(value1, value2); } }
		public double Max { get { return Math.Max(value1, value2); } }
		public double Delta { get { return Max - Min; } }
		public double Value1 {
			get { return value1; }
			set { value1 = value; }
		}
		public double Value2 {
			get { return value2; }
			set { value2 = value; }
		}
		public RangeValue(double value1, double value2) {
			this.value1 = value1;
			this.value2 = value2;
		}
		public RangeValue(double value)
			: this(value, value) {
		}
		public bool Contains(double value) {
			return Min <= value && value <= Max;
		}
	}
	public struct RangeIndexes {
		static RangeIndexes empty = new RangeIndexes(-1, -1);
		public static RangeIndexes Empty { get { return empty; } }
		int index1;
		int index2;
		public int Index1 {
			get { return index1; }
			set { index1 = value; }
		}
		public int Index2 {
			get { return index2; }
			set { index2 = value; }
		}
		public int Min { get { return Math.Min(index1, index2); } }
		public int Max { get { return Math.Max(index1, index2); } }
		public RangeIndexes(int index1, int index2) {
			this.index1 = index1;
			this.index2 = index2;
		}
	}
	public abstract class AxisScaleTypeMap : IScaleMap {
		Transformation transformation;
		public static bool CheckArgumentScaleType(ISeriesPoint point, Scale scaleType) {
			if (scaleType != Scale.Auto && (scaleType == Scale.Numerical || scaleType == Scale.DateTime))
				return point.ArgumentScaleType == scaleType;
			return true;
		}
		public abstract ActualScaleType ScaleType { get; }
		public Transformation Transformation {
			get {
				if (transformation == null)
					transformation = new IdentityTransformation();
				return transformation;
			}
		}
		public AxisScaleTypeMap() {
		}
		protected virtual Transformation CreateTransformation(IAxisData axis) {
			return new IdentityTransformation();
		}
		public void BuildTransformation(IAxisData axis) {
			transformation = CreateTransformation(axis);
		}
		public abstract AxisScaleTypeMap Clone();
		public abstract double NativeToInternal(object value);
		public abstract double NativeToRefined(object value);
		public abstract bool TryNativeToInternal(object value, out double result);
		public abstract object InternalToNative(double value);
		public abstract double InternalToRefined(double value);
		public abstract double RefinedToInternal(double value);
		public abstract object ConvertValue(object value, CultureInfo culture);
		public abstract object TryParse(object value, CultureInfo culture);
		public abstract bool IsCompatible(object value);
		public abstract bool IsCompatibleType(Type type);
		public abstract object DefaultAxisValue { get; }
		public abstract double InternalToRefinedExact(double value);
		public abstract double RefinedToInternalExact(double value);
		public abstract object RefinedToNative(double value);
		public double NativeToFinalWithConvertion(object nativeValue, CultureInfo cultureInfo) {
			object convertedValue = ConvertValue(nativeValue, cultureInfo);
			return transformation.TransformForward(NativeToInternal(convertedValue));
		}
	}
	public class AxisQualitativeMap : AxisScaleTypeMap, IPriorScaleMap {
#region inner classes
		struct ValueItem {
			readonly string value;
			int index;
			public string Value { get { return value; } }
			public int Index { get { return index; } set { index = value; } }
			public ValueItem(string value) {
				this.value = value;
				this.index = -1;
			}
			public ValueItem(object value) : this (value.ToString()) {
			}
		}
		class ValueItemComparer : IComparer<ValueItem> {
#if DXPORTABLE
			StringComparer innerComparer = StringExtensions.ComparerInvariantCultureIgnoreCase;
#else
			StringComparer innerComparer = StringComparer.Create(CultureInfo.InvariantCulture, false);
#endif
			public int Compare(ValueItem x, ValueItem y) {
				return innerComparer.Compare(x.Value, y.Value);
			}
		}
#endregion
		class UserValueComparer : IComparer<string> {
			readonly IComparer userComparer;
			public UserValueComparer(IComparer userComparer) {
				this.userComparer = userComparer;
			}
			public int Compare(string x, string y) {
				return userComparer.Compare(x, y);
			}
		}
		readonly List<string> uniqueValues;
		readonly List<ValueItem> sortedValues;
		ValueItemComparer comparer = new ValueItemComparer();
		public override ActualScaleType ScaleType { get { return ActualScaleType.Qualitative; } }
		public override object DefaultAxisValue { get { return InternalToNative(0); } }
		public int UniqueValuesCount { get { return uniqueValues.Count; } }
		public AxisQualitativeMap(List<object> uniqueValues) {
			this.uniqueValues = new List<string>(uniqueValues.Count);
			this.sortedValues = new List<ValueItem>(uniqueValues.Count);
			foreach (object obj in uniqueValues)
				AddValue(obj);
		}
		public void AddValue(object value) {
			if (value != null) {
				ValueItem item = new ValueItem(value) { Index = uniqueValues.Count };
				int index = sortedValues.BinarySearch(item, comparer);
				if (index < 0) {
					index = ~index;
					sortedValues.Insert(index, item);
					uniqueValues.Add(item.Value);
				}
			}		
		}
		public void Insert(object value) {
			if (value != null) {
				ValueItem item = new ValueItem(value) { Index = 0 };
				int index = sortedValues.BinarySearch(item, comparer);
				if (index < 0) {
					index = ~index;
					for (int i = 0; i < sortedValues.Count; i++)
						sortedValues[i] = new ValueItem(sortedValues[i].Value) { Index = sortedValues[i].Index + 1 };
					sortedValues.Insert(index, item);
					uniqueValues.Insert(0, item.Value);
				}
			}
		}
		public void SortValues(IComparer comparer) {
			if (comparer != null) {
				UserValueComparer userComparer = new UserValueComparer(comparer);
				uniqueValues.Sort(userComparer);
				for (int uniqueIndex = 0; uniqueIndex < uniqueValues.Count; uniqueIndex++) {
					string value = uniqueValues[uniqueIndex];
					int sortedIndex = sortedValues.BinarySearch(new ValueItem(value), this.comparer);
					if (sortedIndex >= 0) {
						ValueItem valueItem = sortedValues[sortedIndex];
						valueItem.Index = uniqueIndex;
						sortedValues[sortedIndex] = valueItem;
					}
				}
			}
		}
		int IndexOf(object value) {
			int index = value != null ? sortedValues.BinarySearch(new ValueItem(value), comparer) : -1;
			return index < 0 ? -1 : sortedValues[index].Index;
		}
		public override double NativeToInternal(object value) {
			int index = IndexOf(value);
			return index < 0 ? double.NaN : index;
		}
		public override double NativeToRefined(object value) {
			return NativeToInternal(value);
		}
		public override bool TryNativeToInternal(object value, out double result) {
			result = IndexOf(value);
			return result >= 0;
		}
		public override object InternalToNative(double value) {
			if (value > int.MaxValue)
				value = int.MaxValue;
			if (value < int.MinValue)
				value = int.MinValue;
			int index = Convert.ToInt32(value);
			return index < uniqueValues.Count && index >= 0 ? uniqueValues[index] : String.Empty;
		}
		public override double RefinedToInternal(double value) {
			return value;
		}
		public override double InternalToRefined(double value) {
			return value;
		}
		public override object ConvertValue(object value, CultureInfo culture) {
			if (value == null)
				return null;
			try {
				return Convert.ToString(value, culture);
			}
			catch {
				return value;
			}
		}
		public override object TryParse(object value, CultureInfo culture) {
			if (value == null)
				return null;
			string str = value as string;
			if (str != null)
				return str;
			return null;
		}
		public override bool IsCompatible(object value) {
			return value != null;
		}
		public override bool IsCompatibleType(Type type) {
			return (type != null);
		}
		public override AxisScaleTypeMap Clone() {
			return new AxisQualitativeMap(new List<object>());
		}
		public IList<string> GetValues() {
			return uniqueValues;
		}
		public override double InternalToRefinedExact(double value) {
			return value;
		}
		public override double RefinedToInternalExact(double value) {
			return value;
		}
		public override object RefinedToNative(double value) {
			return InternalToNative(RefinedToInternalExact(value));
		}
#region IPriorScaleMap
		double IPriorScaleMap.NativeToInternal(object value) {
			return NativeToInternal(value);
		}
		object IPriorScaleMap.InternalToNative(double value) {
			return InternalToNative(value);
		}
		void IPriorScaleMap.UpdateMin(double min) {
		}
#endregion
	}
	public class AxisNumericalMap : AxisScaleTypeMap, IPriorScaleMap {
		const double doubleInaccuracy = 1e-10;
		readonly double? measureUnit;
		public double? MeasureUnit { get { return measureUnit; } }
		public override ActualScaleType ScaleType { get { return ActualScaleType.Numerical; } }
		public override object DefaultAxisValue { get { return 1.0; } }
		public AxisNumericalMap() {
			this.measureUnit = null;
		}
		public AxisNumericalMap(double measureUnit) {
			this.measureUnit = measureUnit;
		}
		protected override Transformation CreateTransformation(IAxisData axis) {
			ILogarithmic logarithmicAxis = axis as ILogarithmic;
			if (logarithmicAxis != null && logarithmicAxis.Enabled)
				return new LogarithmicTransformation(logarithmicAxis.Base);
			return base.CreateTransformation(axis);
		}
		public override double NativeToInternal(object value) {
			if (value != null && DataItemsHelper.IsNumericalType(value.GetType()))
				return RefinedToInternal(Convert.ToDouble(value));
			return 0;
		}
		public override double NativeToRefined(object value) {
			return value != null && DataItemsHelper.IsNumericalType(value.GetType()) ? Convert.ToDouble(value) : 0;
		}
		public override bool TryNativeToInternal(object value, out double result) {
			double? internalValue = DataItemsHelper.ParseNumerical(value);
			if (internalValue.HasValue) {
				result = internalValue.Value;
				return true;
			}
			result = 0;
			return false;
		}
		public override object InternalToNative(double value) {
			return measureUnit.HasValue ? value * measureUnit.Value : value;
		}
		public override double InternalToRefined(double value) {
			return measureUnit.HasValue ? value * measureUnit.Value : value;
		}
		public override double RefinedToInternal(double value) {
			if (measureUnit.HasValue) {
				double internalValue = value / measureUnit.Value;
				if (internalValue > 0)
					internalValue += doubleInaccuracy;
				return Math.Floor(internalValue);
			}
			return value;
		}
		public override object ConvertValue(object value, CultureInfo culture) {
			if (value != null) {
				string str = value as string;
				if (str == null)
					try {
						return Convert.ToDouble(value, culture);
					}
					catch {
					}
				else {
					double result;
					if (Double.TryParse(str,
							NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign |
							NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, culture, out result))
						return result;
				}
			}
			return value;
		}
		public override object TryParse(object value, CultureInfo culture) {
			if (value == null)
				return null;
			string str = value as string;
			if (str == null)
				return value;
			double result;
			if (Double.TryParse(str,
					NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign |
					NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, culture, out result))
				return result;
			return value;
		}
		public override bool IsCompatible(object value) {
			return value is double || value is int;
		}
		public override bool IsCompatibleType(Type type) {
			return (type != null) && (type.IsAssignableFrom(typeof(double)));
		}
		public override AxisScaleTypeMap Clone() {
			if (measureUnit.HasValue)
				return new AxisNumericalMap(measureUnit.Value);
			return new AxisNumericalMap();
		}
		public override double InternalToRefinedExact(double value) {
			return InternalToRefined(value);
		}
		public override double RefinedToInternalExact(double value) {
			return measureUnit.HasValue ? value / measureUnit.Value : value;
		}
		public override object RefinedToNative(double value) {
			return InternalToNative(RefinedToInternalExact(value));
		}
#region IPriorScaleMap
		double IPriorScaleMap.NativeToInternal(object value) {
			return NativeToInternal(value);
		}
		object IPriorScaleMap.InternalToNative(double value) {
			return InternalToNative(value);
		}
		void IPriorScaleMap.UpdateMin(double min) {
		}
#endregion
	}
	public class AxisDateTimeMap : AxisScaleTypeMap, IPriorScaleMap {
		readonly DateTimeMeasureUnitNative measureUnit;
		readonly IWorkdaysOptions workdaysOptions;
		DateTime minValue = DateTime.MinValue;
		double min = 0;
		public double Min {
			get {
				if (minValue == DateTime.MinValue) {
					minValue = DateTime.Now;
					this.min = NativeToInternal(minValue);
				}
				return min;
			}
		}
		public override ActualScaleType ScaleType { get { return ActualScaleType.DateTime; } }
		public override object DefaultAxisValue { get { return minValue; } }
		public DateTimeMeasureUnitNative MeasureUnit { get { return measureUnit; } }
		public AxisDateTimeMap() : this(DateTimeMeasureUnitNative.Millisecond, null) { }
		public AxisDateTimeMap(DateTimeMeasureUnitNative dateTimeMeasureUnit, IWorkdaysOptions workdaysOptions) {
			this.measureUnit = dateTimeMeasureUnit;
			this.workdaysOptions = workdaysOptions;
		}
		public override double NativeToInternal(object value) {
			if (value is DateTime)
				return DateTimeUtilsExt.Difference(DateTimeUtilsExt.MinDateTime, (DateTime)value, measureUnit, workdaysOptions) -
					   DateTimeUtilsExt.GetHolidaysCount((DateTime)value, workdaysOptions, this.measureUnit);
			return 0.0;
		}
		public override double NativeToRefined(object value) {
			if (value is DateTime)
				return (((DateTime)value) - DateTimeUtilsExt.MinDateTime).TotalMilliseconds;
			return 0.0;
		}
		public override object InternalToNative(double value) {
			return InternalToNative(value, this.measureUnit);
		}
		public override double InternalToRefined(double value) {
			return InternalToRefined(value, this.measureUnit);
		}
		public override double RefinedToInternal(double value) {
			return RefinedToInternal(value, this.measureUnit);
		}
		public override bool TryNativeToInternal(object value, out double result) {
			DateTime? dateTimeValue = DataItemsHelper.ParseDateTime(value);
			result = dateTimeValue.HasValue ? NativeToInternal(dateTimeValue.Value) : 0;
			return dateTimeValue.HasValue;
		}
		public override object ConvertValue(object value, CultureInfo culture) {
			if (value != null) {
				string str = value as string;
				if (str == null)
					try {
						return Convert.ToDateTime(value, culture);
					}
					catch {
					}
				else {
					DateTime dateTime;
					if (DateTime.TryParse(str, culture, DateTimeStyles.None, out dateTime))
						return dateTime;
				}
			}
			return value;
		}
		public override object TryParse(object value, CultureInfo culture) {
			if (value == null)
				return null;
			string str = value as string;
			if (str == null)
				return value;
			DateTime dateTime;
			if (DateTime.TryParse(str, culture, DateTimeStyles.None, out dateTime))
				return dateTime;
			return value;
		}
		public override bool IsCompatible(object value) {
			return value is DateTime;
		}
		public override bool IsCompatibleType(Type type) {
			return (type != null) && (type.IsAssignableFrom(typeof(DateTime)));
		}
		public override AxisScaleTypeMap Clone() {
			AxisDateTimeMap map = new AxisDateTimeMap(measureUnit, workdaysOptions);
			map.minValue = this.minValue;
			map.min = this.min;
			return map;
		}
		public object InternalToNative(double value, DateTimeMeasureUnitNative measureUnit) {
			return DateTimeUtilsExt.ReverseHolidays(value, workdaysOptions, measureUnit);
		}
		public double RefinedToInternal(double value, DateTimeMeasureUnitNative measureUnit) {
			return DateTimeUtilsExt.Difference(DateTimeUtilsExt.MinDateTime, DateTimeUtilsExt.MinDateTime.AddMilliseconds(value), measureUnit, workdaysOptions) -
				   DateTimeUtilsExt.GetHolidaysCount(DateTimeUtilsExt.MinDateTime.AddMilliseconds(value), workdaysOptions, measureUnit);
		}
		public double InternalToRefined(double value, DateTimeMeasureUnitNative measureUnit) {
			return NativeToRefined(InternalToNative(value, measureUnit));
		}
		public override double InternalToRefinedExact(double value) {
			double floorValueInternal = Math.Floor(value);
			double floorValue = InternalToRefined(floorValueInternal);
			double offsetInternal = value - floorValueInternal;
			double ceilValue = InternalToRefined(floorValueInternal + 1.0);
			return floorValue + (ceilValue - floorValue) * offsetInternal;
		}
		public override double RefinedToInternalExact(double value) {
			double floorValueInternal = RefinedToInternal(value);
			double floorValueRefined = InternalToRefined(floorValueInternal);
			double offsetRefined = value - floorValueRefined;
			double ceilValueRefined = InternalToRefined(floorValueInternal + 1.0);
			double stepRefined = ceilValueRefined - floorValueRefined;
			return floorValueInternal + (offsetRefined / stepRefined);
		}
		public override object RefinedToNative(double value) {
			return DateTime.MinValue.AddMilliseconds(value);
		}
#region IPriorScaleMap
		double IPriorScaleMap.NativeToInternal(object value) {
			return NativeToInternal(value) - Min;
		}
		object IPriorScaleMap.InternalToNative(double value) {
			return InternalToNative(Min + value);
		}
		void IPriorScaleMap.UpdateMin(double min) {
			if (double.IsNaN(min))
				return;
			minValue = (DateTime)InternalToNative(min);
			this.min = min;
		}
#endregion
	}
}
