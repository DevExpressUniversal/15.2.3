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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.IO;
using DevExpress.Data.Storage;
using DevExpress.PivotGrid;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.IO;
#if DXRESTRICTED
using PD = DevExpress.Compatibility.System.ComponentModel;
#else
using PD = System.ComponentModel;
#endif
namespace DevExpress.Data.PivotGrid {
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum PivotSummaryType { Count = 0, Sum = 1, Min = 2, Max = 3, Average = 4, StdDev = 5, StdDevp = 6, Var = 7, Varp = 8, Custom = 9 };
	public enum PivotSummaryVariation { None, Absolute, Percent };
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum PivotSummaryDisplayType {
		Default = 0,
		AbsoluteVariation = 1,
		PercentVariation = 2,
		PercentOfColumn = 3,
		PercentOfRow = 4,
		PercentOfColumnGrandTotal = 5,
		PercentOfRowGrandTotal = 6,
		PercentOfGrandTotal = 7,
		RankInColumnSmallestToLargest = 8,
		RankInRowSmallestToLargest = 9,
		RankInColumnLargestToSmallest = 10,
		RankInRowLargestToSmallest = 11,
		Index = 12
	};
	public class PivotSummaryDisplayTypeConverter {
		static public PivotSummaryVariation DisplayTypeToVariation(PivotSummaryDisplayType displayType) {
			switch(displayType) {
				case PivotSummaryDisplayType.AbsoluteVariation:
					return PivotSummaryVariation.Absolute;
				case PivotSummaryDisplayType.PercentVariation:
					return PivotSummaryVariation.Percent;
				default:
					return PivotSummaryVariation.None;
			}
		}
		static public PivotSummaryDisplayType VariationToDisplayType(PivotSummaryVariation variation) {
			switch(variation) {
				case PivotSummaryVariation.Absolute:
					return PivotSummaryDisplayType.AbsoluteVariation;
				case PivotSummaryVariation.Percent:
					return PivotSummaryDisplayType.PercentVariation;
				default:
					return PivotSummaryDisplayType.Default;
			}
		}
		static public bool IsVariation(PivotGridData data, PivotGridFieldBase field) {
			Guard.ArgumentNotNull(field, "field");
			if(data != null)
				return data.CellValuesProvider.GetLastCalculation(field) is PivotVariationCalculationBase;
			return false;
		}
	}
	[TypeConverter(typeof(PivotErrorValue.PivotErrorValueTypeConverter))]
	public class PivotErrorValue {
		public class PivotErrorValueTypeConverter : TypeConverter {
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
				return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
			}
			public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
				string str = value as string;
				if(str != null && str == "PEV")
					return PivotSummaryValue.ErrorValue;
				return base.ConvertFrom(context, culture, value);
			}
			public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
				if(destinationType == typeof(string))
					return "PEV";
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
		public PivotErrorValue(int errorId) {
			this.errorId = errorId;
		}
		int errorId;
		internal int ErrorId { get { return errorId; } }
		public override string ToString() {
			return PivotGridLocalizer.GetString(PivotGridStringId.ValueError);
			;
		}
	}
	public class PivotSummaryValue {
		static object errorValue;
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotSummaryValueErrorValue")]
#endif
		public static object ErrorValue {
			get {
				if(errorValue == null)
					errorValue = new PivotErrorValue(0);
				return errorValue;
			}
		}
		static bool IsDecimalConversionAllowed(double value) {
			double decimalValue = (double)((decimal)value);
			return (value <= (double)decimal.MaxValue) &&
				(value >= (double)decimal.MinValue) &&
				((value > 0 && (value - decimalValue) < 0) || (value < 0 && (value - decimalValue) > 0));
		}
		public static Type GetValueType(PivotSummaryType summaryType, Type sourceType) {
			switch(summaryType) {
				case PivotSummaryType.Average:
					return typeof(decimal);
				case PivotSummaryType.Max:
					return sourceType;
				case PivotSummaryType.Min:
					return sourceType;
				case PivotSummaryType.Sum:
					return typeof(decimal);
				case PivotSummaryType.StdDev:
					return typeof(double);
				case PivotSummaryType.StdDevp:
					return typeof(double);
				case PivotSummaryType.Var:
					return typeof(double);
				case PivotSummaryType.Varp:
					return typeof(double);
				case PivotSummaryType.Custom:
					return sourceType;
				case PivotSummaryType.Count:
					return typeof(int);
			}
			return sourceType;
		}
		object tag;
		object min;
		object max;
		object customValue;
		decimal summary;
		double squareSummary;
		int count;
		bool isSummaryNull;
		PivotValueComparerBase valueComparer;
		bool compareError,
			summaryError;
		internal PivotValueComparerBase Comparer { get { return valueComparer; } }
		public PivotSummaryValue(PivotValueComparerBase valueComparer) {
			this.valueComparer = valueComparer;
			Clear();
		}
		internal void Clear() {
			this.customValue = null;
			this.min = null;
			this.max = null;
			this.count = 0;
			this.summary = 0;
			this.squareSummary = 0;
			this.isSummaryNull = true;
			this.compareError = false;
			this.summaryError = false;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void AddValue(object value, decimal numericValue) {
			if(value == null || value is DBNull)
				return;
			if(value is PivotErrorValue) {
				compareError = true;
				summaryError = true;
			}
			if(count == 0) {
				min = max = value;
			} else {
				AddMaxValue(value);
				AddMinValue(value);
			}
			count++;
			if(isSummaryNull)
				isSummaryNull = false;
			try {
				if(!summaryError)
					summary += numericValue;
			} catch(System.OverflowException) {
				summaryError = true;
			}
			squareSummary += ((double)numericValue * (double)numericValue);
		}
		virtual internal void AddValue(object value) {
			decimal numericValue = Convert.ToDecimal(value);
			AddValue(value, numericValue);
		}
		internal void AddValue(PivotSummaryValue summaryValue) {
			if(summaryValue == null || summaryValue.Count == 0)
				return;
			if(summaryValue.CompareError) {
				compareError = true;
				summaryError = true;
			}
			if(summaryValue.summaryError) {
				summaryError = true;
			}
			if(count == 0) {
				min = summaryValue.Min;
				max = summaryValue.Max;
			} else {
				AddMaxValue(summaryValue.Max);
				AddMinValue(summaryValue.Min);
			}
			count += summaryValue.Count;
			if(!summaryValue.IsSummaryNull) {
				isSummaryNull = false;
				try {
					summary += summaryValue.SummaryCore;
				} catch(System.OverflowException) {
					summaryError = true;
				}
				squareSummary += summaryValue.SquareSummary;
			}
		}
		void AddMaxValue(object maxValue) {
			if(CompareError)
				return;
			int res = Comparer.UnsafeCompare(max, maxValue);
			if(res == PivotValueComparer.CompareError)
				compareError = true;
			else if(res < 0)
				max = maxValue;
		}
		void AddMinValue(object minValue) {
			if(CompareError)
				return;
			int res = Comparer.UnsafeCompare(min, minValue);
			if(res == PivotValueComparer.CompareError)
				compareError = true;
			else if(res > 0)
				min = minValue;
		}
		public virtual object GetValue(PivotSummaryType summaryType) {
			switch(summaryType) {
				case PivotSummaryType.Average:
					return Average;
				case PivotSummaryType.Max:
					return Max;
				case PivotSummaryType.Min:
					return Min;
				case PivotSummaryType.Sum:
					return Summary;
				case PivotSummaryType.StdDev:
					return StdDev;
				case PivotSummaryType.StdDevp:
					return StdDevp;
				case PivotSummaryType.Var:
					return Var;
				case PivotSummaryType.Varp:
					return Varp;
				case PivotSummaryType.Custom:
					return CustomValue;
				case PivotSummaryType.Count:
					return Count;
			}
			return null;
		}
		internal object GetCustomValue(PivotGridFieldBase field) {
			PivotGridCustomValues dic = CustomValue as PivotGridCustomValues;
			if(dic == null)
				return CustomValue;
			if(!dic.Contains(field))
				return dic.Contains(null) ? dic[null] : null;
			return dic[field];
		}
		virtual internal void SetErrorValue() {
			compareError = true;
			summaryError = true;
		}
		internal void SetSummaryError() {
			summaryError = true;
		}
		internal bool CompareError { get { return compareError; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotSummaryValueCount")]
#endif
		public int Count { get { return count; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotSummaryValueMin")]
#endif
		public object Min { get { return compareError ? ErrorValue : min; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotSummaryValueMax")]
#endif
		public object Max { get { return compareError ? ErrorValue : max; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotSummaryValueCustomValue")]
#endif
		public object CustomValue { get { return customValue; } set { customValue = value; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotSummaryValueTag")]
#endif
		public object Tag { get { return tag; } set { tag = value; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotSummaryValueSummary")]
#endif
		public object Summary { get { return summaryError ? ErrorValue : summary; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotSummaryValueAverage")]
#endif
		public object Average {
			get {
				if(summaryError)
					return ErrorValue;
				if(IsSummaryNull)
					return null;
				else
					return summary / count;
			}
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotSummaryValueStdDev")]
#endif
		public object StdDev {
			get {
				if(IsStdDevIncorrect)
					return null;
				else
					return Math.Sqrt(VarCore);
			}
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotSummaryValueStdDevp")]
#endif
		public object StdDevp {
			get {
				if(IsStdDevpIncorrect)
					return null;
				else
					return Math.Sqrt(VarpCore);
			}
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotSummaryValueVar")]
#endif
		public object Var {
			get {
				if(IsStdDevIncorrect)
					return null;
				else
					return VarCore;
			}
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotSummaryValueVarp")]
#endif
		public object Varp {
			get {
				if(IsStdDevpIncorrect)
					return null;
				else
					return VarpCore;
			}
		}
		protected bool IsSummaryNull { get { return count == 0 || isSummaryNull || summaryError; } }
		protected bool IsStdDevIncorrect { get { return count < 2 || isSummaryNull || summaryError; } }
		protected bool IsStdDevpIncorrect { get { return count < 1 || isSummaryNull || summaryError; } }
		protected decimal SummaryCore { get { return summary; } }
		protected double SquareSummary { get { return squareSummary; } }
		protected double VarCore {
			get {
				if(IsDecimalConversionAllowed(squareSummary)) {
					return (double)(((decimal)squareSummary - (summary * summary) / count) / (count - 1));
				} else {
					return (squareSummary - ((double)summary * (double)summary) / count) / (count - 1);
				}
			}
		}
		protected double VarpCore {
			get {
				if(IsDecimalConversionAllowed(squareSummary)) {
					return (double)(((decimal)squareSummary - (summary * summary) / count) / count);
				} else {
					return (squareSummary - ((double)summary * (double)summary) / count) / count;
				}
			}
		}
	}
	public class NumericPivotSummaryValue : PivotSummaryValue {
		public NumericPivotSummaryValue(PivotValueComparerBase valueComparer)
			: base(valueComparer) {
		}
		public override void AddValue(object value, decimal numericValue) {
			base.AddValue((value == null || value == DBNull.Value) ? value : numericValue, numericValue);
		}
	}
	public class PivotExpressionSummaryValue : PivotSummaryValue {
		public PivotExpressionSummaryValue(PivotValueComparerBase valueComparer)
			: base(valueComparer) {
			errorValue = false;
		}
		bool errorValue;
		object value;
		object Value {
			get {
				if(errorValue)
					return ErrorValue;
				return this.value;
			}
			set { this.value = value; }
		}
		internal override void AddValue(object value) {
			Value = value;
		}
		public override void AddValue(object value, decimal numericValue) {
			throw new Exception("Unexpected operation with the expression summary");
		}
		public override object GetValue(PivotSummaryType summaryType) {
			return Value;
		}
		internal override void SetErrorValue() {
			errorValue = true;
		}
	}
	public class NumericPivotExpressionSummaryValue : PivotExpressionSummaryValue {
		public NumericPivotExpressionSummaryValue(PivotValueComparerBase valueComparer)
			: base(valueComparer) {
		}
		public override void AddValue(object value, decimal numericValue) {
			base.AddValue(numericValue, numericValue);
		}
	}
	class PivotGridCustomValues {
		Dictionary<PivotGridFieldBase, object> customValues;
		Dictionary<string, PivotGridFieldBase> fieldsByUniqueName;
		object nullFieldValue;
		bool isNullAsigned;
		public PivotGridCustomValues() {
			customValues = new Dictionary<PivotGridFieldBase, object>();
			fieldsByUniqueName = new Dictionary<string, PivotGridFieldBase>();
			isNullAsigned = false;
		}
		public bool Contains(PivotGridFieldBase field) {
			return (field == null && isNullAsigned) || (field != null && customValues.ContainsKey(field));
		}
		public bool TryGetValue(string uniqueColumnName, out object value) {
			value = null;
			PivotGridFieldBase keyField;
			if(!fieldsByUniqueName.TryGetValue(uniqueColumnName, out keyField))
				return false;
			value = customValues[keyField];
			return true;
		}
		public object this[PivotGridFieldBase field] {
			get {
				if(field == null)
					return nullFieldValue;
				return customValues[field];
			}
			set {
				if(field == null)
					SetNullValue(value);
				else
					AddCustomValue(field, value);
			}
		}
		void SetNullValue(object value) {
			nullFieldValue = value;
			isNullAsigned = true;
		}
		void AddCustomValue(PivotGridFieldBase field, object value) {
			customValues[field] = value;
			fieldsByUniqueName[field.ExpressionFieldName] = field;
		}
	}
	public class PivotSortByCondition {
		DataColumnInfo column;
		object value;
		int level;
		public PivotSortByCondition(DataColumnInfo column, object value, int level) {
			if(column == null)
				throw new ArgumentNullException("column");
			if(level < 0)
				throw new ArgumentException("level must be greater that zero");
			this.column = column;
			this.value = value;
			this.level = level;
		}
		public DataColumnInfo Column { get { return column; } }
		public object Value { get { return value; } }
		public int Level { get { return level; } }
	}
	enum SummaryItemCalculationMode {
		Traditional,
		Expression,
		AggregateExpression
	}
	public class PivotSummaryItem : SummaryItemBase {
		PivotGridFieldBase field;
		readonly string name;
		PivotSummaryType summaryType;
		PivotSummaryIntervalsCache intervalsCache;
		public PivotSummaryItem(DataColumnInfo columnInfo, PivotSummaryType summaryType)
			: base(columnInfo) {
			this.name = string.Empty;
			this.summaryType = summaryType;
		}
		public PivotSummaryItem(DataColumnInfo columnInfo, PivotGridFieldBase field, string expressionFieldName, PivotSummaryType summaryType)
			: base(columnInfo) {
			this.name = expressionFieldName;
			this.field = field;
			ChangeSummaryType(summaryType);
		}
		public PivotGridFieldBase Field { get { return field; } }
		public string Name { get { return name; } }
		public PivotSummaryType SummaryType { get { return summaryType; } }
		internal virtual SummaryItemCalculationMode CalculationMode { get { return SummaryItemCalculationMode.Traditional; } }
		internal void ChangeSummaryType(PivotSummaryType summaryType) {
			this.summaryType = summaryType;
			if(intervalsCache != null)
				intervalsCache.Clear();
		}
		protected bool UseDecimalValuesForMaxMinSummary {
			get {
				if(ColumnInfo.Tag == null)
					return false;
				return ((PivotGridFieldBase)ColumnInfo.Tag).UseDecimalValuesForMaxMinSummary;
			}
		}
		public virtual PivotSummaryValue CreateSummaryValue(PivotValueComparerBase valueComparer) {
			if(UseDecimalValuesForMaxMinSummary)
				return new NumericPivotSummaryValue(valueComparer);
			return new PivotSummaryValue(valueComparer);
		}
		public static PivotSummaryItem CreateSummaryItem(DataColumnInfo columnInfo, PivotSummaryType summaryType) {
			return new PivotSummaryItem(columnInfo, summaryType);
		}
		public static PivotSummaryItem CreateSummaryItem(DataColumnInfo columnInfo, PivotGridFieldBase field) {
			return new PivotSummaryItem(columnInfo, field, field.ExpressionFieldName, field.SummaryType); 
		}
		public static PivotSummaryExpressionItem CreateSummaryExpressionItem(DataColumnInfo columnInfo, PivotGridFieldBase field, int maxColumnGroupLevel, int maxRowGroupLevel) {
			return new PivotSummaryExpressionItem(columnInfo, field, maxColumnGroupLevel, maxRowGroupLevel);
		}
		internal PivotSummaryIntervalsCache IntervalsCache {
			get {
				if(intervalsCache == null)
					intervalsCache = new PivotSummaryIntervalsCache();
				return intervalsCache;
			}
		}
		bool RequireValueConvert {
			get {
				if(ColumnInfo == null)
					return false;
				Type type = ColumnInfo.Type;
				if(type == typeof(string) || type == typeof(DateTime) || type == typeof(DateTime?))
					return false;
				return true; 
			}
		}
		static object ConvertValue(object val, out decimal numericVal, bool requireValueConvert, Func<decimal, object> errorValueConverter) {
			numericVal = 0;
			if(!requireValueConvert)
				return val;
			try {
				if(val != null && !object.ReferenceEquals(val, DBNull.Value)) {
					IConvertible convertible = val as IConvertible;
					if(convertible != null) {
						double doubleVal = 0;
						if(val is string) {
							if(!double.TryParse(val.ToString(), out doubleVal)) {
								return null;
							}
						}
						doubleVal = convertible.ToDouble(null);
						if(double.IsNaN(doubleVal))
							return 0;
						if(double.IsInfinity(doubleVal))
							return 0;
						numericVal = convertible.ToDecimal(null);
					} else
						numericVal = ValueToDecimal(val);
				}
			} catch {
				numericVal = 0;
				return errorValueConverter(numericVal);
			}
			return numericVal;
		}
		static decimal ValueToDecimal(object val) {
			if(val is TimeSpan) {
				return ((TimeSpan)val).Ticks;
			}
			return 0;
		}
		internal object ConvertValue(object val, out decimal numericVal) {
			return ConvertValue(val, out numericVal, RequireValueConvert, GetConvertErrorValue);
		}
		object GetConvertErrorValue(decimal numericVal) {
			return PivotSummaryValue.ErrorValue;
		}
	}
	class PivotCustomAggregateSummaryItem : PivotSummaryItem {
		IPivotCustomSummaryAggregate agg;
		public PivotCustomAggregateSummaryItem(DataColumnInfo columnInfo, PivotGridFieldBase field, string expressionFieldName, PivotSummaryType summaryType, IPivotCustomSummaryAggregate agg)
			: base(columnInfo, field, expressionFieldName, summaryType) {
			this.agg = agg;
		}
		internal override SummaryItemCalculationMode CalculationMode { get { return SummaryItemCalculationMode.AggregateExpression; } }
		internal object Calculate(IEnumerable<object> enumerable) {
			return agg.Calculate(enumerable);
		}
		public override PivotSummaryValue CreateSummaryValue(PivotValueComparerBase valueComparer) {
			return new PivotExpressionSummaryValue(valueComparer);
		}
	}
	public class PivotSummaryExpressionItem : PivotSummaryItem {
		List<PivotSummaryItem> summaryRelations;
		bool hasBadRelations;
		int maxRowGroupLevel;
		int maxColumnGroupLevel;
		public PivotSummaryExpressionItem(DataColumnInfo columnInfo, PivotGridFieldBase field, int maxColumnGroupLevel, int maxRowGroupLevel, string expressionFieldName, PivotSummaryType summaryType)
			: base(columnInfo, field, expressionFieldName, summaryType) {
			this.summaryRelations = null;
			this.hasBadRelations = false;
			this.maxColumnGroupLevel = maxColumnGroupLevel;
			this.maxRowGroupLevel = maxRowGroupLevel;
		}
		public PivotSummaryExpressionItem(DataColumnInfo columnInfo, PivotGridFieldBase field, int maxColumnGroupLevel, int maxRowGroupLevel)
			: this(columnInfo, field, maxColumnGroupLevel, maxRowGroupLevel, field.ExpressionFieldName, field.SummaryType) {
		}
		internal override SummaryItemCalculationMode CalculationMode { get { return SummaryItemCalculationMode.Expression; } }
		public int MaxRowGroupLevel {
			get { return maxRowGroupLevel; }
		}
		public int MaxColumnGroupLevel {
			get { return maxColumnGroupLevel; }
		}
		public bool HasSummaryRelations { get { return SummaryRelations != null; } }
		public List<PivotSummaryItem> SummaryRelations {
			get { return summaryRelations; }
		}
		public void AddRelatedSummary(PivotSummaryItem summary) {
			if(summaryRelations == null)
				summaryRelations = new List<PivotSummaryItem>();
			summaryRelations.Add(summary);
		}
		public bool HasBadRelations {
			get { return hasBadRelations; }
			set { hasBadRelations = value; }
		}
		public override PivotSummaryValue CreateSummaryValue(PivotValueComparerBase valueComparer) {
			if(UseDecimalValuesForMaxMinSummary)
				return new NumericPivotExpressionSummaryValue(valueComparer);
			return new PivotExpressionSummaryValue(valueComparer);
		}
		public object ConvertExpressionValue(object value) {
			switch(Field.UnboundType) {
				case UnboundColumnType.Object:
					return value;
				case UnboundColumnType.Boolean:
					return Convert.ToBoolean(value);
				case UnboundColumnType.DateTime:
					return Convert.ToDateTime(value);
				case UnboundColumnType.Decimal:
					return Convert.ToDecimal(value);
				case UnboundColumnType.Integer:
					return Convert.ToInt32(value);
				case UnboundColumnType.String:
					return Convert.ToString(value);
				default:
					throw new Exception("Incorrect summary expression type conversion");
			}
		}
	}
	public class PivotSummaryExpressionEvaluator {
		PivotDataController controller;
		CriteriaOperator expOperator;
		ExpressionEvaluator evaluator;
		public PivotSummaryExpressionEvaluator(PivotDataController controller, CriteriaOperator expOperator) {
			this.controller = controller;
			this.expOperator = expOperator;
		}
		public ExpressionEvaluator Evaluator {
			get {
				if(evaluator == null) {
					evaluator = new ExpressionEvaluator(controller.GetDescriptorCollection(), expOperator, controller.CaseSensitive);
				}
				return evaluator;
			}
		}
	}
	public class PivotSummaryExpressionEvaluators {
		Hashtable evaluators;
		public PivotSummaryExpressionEvaluators() {
			evaluators = new Hashtable();
		}
		protected Hashtable Evaluators {
			get { return evaluators; }
			set { evaluators = value; }
		}
		public void Clear() { Evaluators.Clear(); }
		public bool Contains(PivotSummaryExpressionItem summaryItem) {
			return Evaluators.ContainsKey(summaryItem);
		}
		public ExpressionEvaluator this[PivotSummaryExpressionItem summaryItem] {
			get { return ((PivotSummaryExpressionEvaluator)Evaluators[summaryItem]).Evaluator; }
		}
		public void Add(PivotSummaryExpressionItem summaryItem, PivotSummaryExpressionEvaluator evaluator) {
			if(!Contains(summaryItem)) {
				Evaluators.Add(summaryItem, evaluator);
			}
		}
	}
	public class PivotSummaryItemCollection : ColumnInfoNotificationCollection {
		readonly Dictionary<string, int> indexByName;
		public PivotSummaryItemCollection(DataControllerBase controller, CollectionChangeEventHandler collectionChanged)
			: base(controller, collectionChanged) {
			indexByName = new Dictionary<string, int>();
		}
		public PivotSummaryItem this[int index] { get { return (PivotSummaryItem)List[index]; } }
		public bool Contains(PivotSummaryItem item) { return List.Contains(item); }
		public bool Contains(DataColumnInfo columnInfo) {
			return indexByName.ContainsKey(columnInfo.Name);
		}
		protected override DataColumnInfo GetColumnInfo(int index) { return this[index].ColumnInfo; }
		public PivotSummaryItem TryGetValue(DataColumnInfo columnInfo) {
			if(!indexByName.ContainsKey(columnInfo.Name))
				return null;
			return this[indexByName[columnInfo.Name]];
		}
		public PivotSummaryItem TryGetValue(string name) {
			if(!indexByName.ContainsKey(name))
				return null;
			return this[indexByName[name]];
		}
		void AddItemIndexToHash(PivotSummaryItem item, int index) {
			indexByName[item.ColumnInfo.Name] = index;
			if(item.Name != item.ColumnInfo.Name)
				indexByName[item.Name] = index;
		}
		public virtual PivotSummaryItem Add(PivotSummaryItem item) {
			int index = List.Add(item);
			AddItemIndexToHash(item, index);
			return item;
		}
		public void ClearAndAddRange(PivotSummaryItemCollection collection) {
			ClearAndAddRange((PivotSummaryItem[])collection.InnerList.ToArray(typeof(PivotSummaryItem)));
		}
		public void ClearAndAddRange(PivotSummaryItem[] summaryItems) {
			BeginUpdate();
			try {
				Clear();
				AddRange(summaryItems);
			} finally {
				EndUpdate();
			}
		}
		public void AddRange(PivotSummaryItem[] summaryItems) {
			BeginUpdate();
			try {
				foreach(PivotSummaryItem summaryItem in summaryItems) {
					Add(summaryItem);
				}
			} finally {
				EndUpdate();
			}
		}
		protected internal virtual void OnSummaryItemChanged(PivotSummaryItem item) {
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, item));
		}
		public new void Clear() {
			indexByName.Clear();
			base.Clear();
		}
		internal int IndexOf(PivotSummaryItem item) {
			return List.IndexOf(item);
		}
		void RebuildNameByIndexHash() {
			indexByName.Clear();
			for(int index = 0; index < List.Count; index++) {
				AddItemIndexToHash((PivotSummaryItem)List[index], index);
			}
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			RebuildNameByIndexHash();
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			RebuildNameByIndexHash();
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			RebuildNameByIndexHash();
		}
	}
	public class PivotGroupRowKeeper {
		object[] values;
		public PivotGroupRowKeeper(object[] values) {
			this.values = values;
		}
		public object[] Values { get { return values; } }
		public int Level { get { return Values.Length - 1; } }
		public string RowHashCode { get { return GetRowHashCode(Level); } }
		public string GetRowHashCode(int level) {
			StringBuilder sb = new StringBuilder();
			sb.Append(level);
			for(int i = 0; i <= level; i++) {
				sb.Append('-');
				if(values[i] != null) {
					sb.Append(values[i].GetHashCode());
				}
			}
			return sb.ToString();
		}
	}
	public class PivotGroupRowKeeperCollection : CollectionBase {
		public PivotGroupRowKeeper this[int index] { get { return (PivotGroupRowKeeper)InnerList[index]; } }
		public void Add(PivotGroupRowKeeper rowKeeper) {
			InnerList.Add(rowKeeper);
		}
		public void Add(object[] values) {
			PivotGroupRowKeeper rowKeeper = new PivotGroupRowKeeper(values);
			InnerList.Add(rowKeeper);
		}
	}
	public class PivotGroupRowsKeeperColumnInfo {
		Type columnType,
			actualColumnType = null;
		string columnName;
		bool isRowsExpanded;
		bool hasOthers;
		public PivotGroupRowsKeeperColumnInfo() {
			Init(null, null, string.Empty, false);
		}
		public PivotGroupRowsKeeperColumnInfo(DataColumnInfo columnInfo, bool hasOthers) {
			Type actualColumnType = columnInfo.Type == typeof(object) ? IdentifyActualType(columnInfo) : columnInfo.Type;
			Init(columnInfo.Type, actualColumnType, columnInfo.Name, hasOthers);
		}
		protected void Init(Type columnType, Type actualColumnType, string columnName, bool hasOthers) {
			this.columnType = columnType;
			this.columnName = columnName;
			this.actualColumnType = actualColumnType;
			this.isRowsExpanded = true;
			this.hasOthers = hasOthers;
		}
		public bool HasOthers { get { return hasOthers; } set { hasOthers = value; } }
		public Type ColumnType { get { return columnType; } }
		public Type ActualColumnType { get { return actualColumnType; } set { actualColumnType = value; } }
		public string ColumnName { get { return columnName; } }
		public bool IsRowsExpanded { get { return isRowsExpanded; } set { isRowsExpanded = value; } }
		bool IsObjectType { get { return ColumnType == typeof(object); } }
		public bool WriteValueType {
			get { return ActualColumnType.IsInterface() || ActualColumnType.IsAbstract() || HasOthers; }
		}
		protected Type IdentifyActualType(DataColumnInfo columnInfo) {
			if(!columnInfo.GetStorageComparer().IsStorageEmpty) {
				for(int i = 0; i < columnInfo.GetStorageComparer().RecordCount; i++) {
					object val = columnInfo.GetStorageComparer().GetNullableRecordValue(i);
					if(object.ReferenceEquals(val, null))
						continue;
					return val.GetType();
				}
			}
			return columnInfo.Type;
		}
		public void Write(BinaryWriter writer) {
			writer.Write(ColumnName);
			writer.Write(ColumnType.AssemblyQualifiedName);
			if(IsObjectType)
				writer.Write(ActualColumnType.AssemblyQualifiedName);
			Byte byt = HasOthers ? (Byte)1 : (Byte)0;
			byt *= 10;
			byt += IsRowsExpanded ? (Byte)1 : (Byte)0;
			writer.Write(byt);
		}
		public void Read(BinaryReader reader) {
			this.columnName = reader.ReadString();
			this.columnType = Type.GetType(reader.ReadString());
			ActualColumnType = IsObjectType ? Type.GetType(reader.ReadString()) : columnType;
			Byte byt = reader.ReadByte();
			this.isRowsExpanded = byt % 10 == 1;
			this.HasOthers = byt > 1;
		}
	}
	public class PivotGroupRowsKeeper {
		PivotDataControllerArea area;
		PivotGroupRowsKeeperColumnInfo[] savedColumns;
		PivotGroupRowKeeperCollection rows;
		Hashtable foundRows;
		PivotCollapsedStateSerializerV1 serializerv1, serializerv2;
		public PivotGroupRowsKeeper(PivotDataControllerArea area) {
			this.area = area;
			this.rows = new PivotGroupRowKeeperCollection();
			this.savedColumns = new PivotGroupRowsKeeperColumnInfo[] { };
			this.foundRows = new Hashtable();
		}
		public PivotDataControllerArea Area { get { return area; } }
		protected GroupRowInfoCollection GroupInfo { get { return Area.GroupInfo; } }
		public PivotGroupRowKeeperCollection Rows { get { return rows; } }
		public PivotCollapsedStateSerializerV1 Serializerv1 {
			get {
				if(serializerv1 == null)
					serializerv1 = new PivotCollapsedStateSerializerV1();
				return serializerv1;
			}
		}
		public PivotCollapsedStateSerializerV1 Serializerv2 {
			get {
				if(serializerv2 == null)
					serializerv2 = new PivotCollapsedStateSerializerV2();
				return serializerv2;
			}
		}
		public void Clear() {
			this.savedColumns = new PivotGroupRowsKeeperColumnInfo[] { };
			Rows.Clear();
			foundRows.Clear();
		}
		public void Restore() {
			int restoredLevel = GetRestoredLevel(false);
			RestoreRows(restoredLevel);
			if(restoredLevel < Area.Columns.Count - 1)
				SaveColumns();
		}
		public void WebWriteToStream(Stream stream) {
			List<bool> expandValues = WebSaveExpandValues();
			WebWriteExpandValues(stream, expandValues);
		}
		List<bool> WebSaveExpandValues() {
			int lastLevel = GetRestoredLevel(true);
			List<bool> expandValues = new List<bool>(GroupInfo.Count);
			for(int i = 0; i < GroupInfo.Count; i++) {
				GroupRowInfo groupRow = GroupInfo[i];
				if(!IsRowLevelBefore(groupRow, lastLevel))
					continue;
				expandValues.Add(groupRow.Expanded);
			}
			return expandValues;
		}
		void WebWriteExpandValues(Stream stream, List<bool> expandValues) {
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(expandValues.Count);
			byte c = 0;
			for(int i = 0; i < expandValues.Count; i++) {
				c |= (byte)((expandValues[i] ? 1 : 0) << (i % 8));
				if(i % 8 == 7) {
					writer.Write(c);
					c = 0;
				}
			}
			if(expandValues.Count % 8 != 0)
				writer.Write(c);
		}
		bool IsRowLevelBefore(GroupRowInfo groupRow, int restoredLevel) {
			return groupRow.Level < restoredLevel;
		}
		public void WebReadFromStream(Stream stream) {
			List<bool> expandValues = WebReadExpandValues(stream);
			RestoreExpandValues(expandValues);
		}
		List<bool> WebReadExpandValues(Stream stream) {
			BinaryReader reader = new BinaryReader(stream);
			int count = reader.ReadInt32();
			List<bool> expandValues = new List<bool>(count);
			byte c = 0;
			for(int i = 0; i < count; i++) {
				if(i % 8 == 0)
					c = reader.ReadByte();
				expandValues.Add((c & 1) == 1);
				c >>= 1;
			}
			return expandValues;
		}
		void RestoreExpandValues(List<bool> expandValues) {
			int lastLevel = GetRestoredLevel(false);
			for(int i = 0, j = 0; i < GroupInfo.Count && j < expandValues.Count; i++) {
				GroupRowInfo groupRow = GroupInfo[i];
				if(!IsRowLevelBefore(groupRow, lastLevel))
					continue;
				groupRow.Expanded = expandValues[j++];
			}
		}
		public void WriteToStream(Stream stream) {
			TypedBinaryWriter writer = TypedBinaryWriter.CreateWriter(stream, Area.Controller.CustomObjectConverter);
			SaveColumns();
			SaveRows();
			Serializerv2.WriteToStream(writer, Rows, SavedColumns);
		}
		public void ReadFromStream(Stream stream) {
			TypedBinaryReader reader = TypedBinaryReader.CreateReader(stream, this.Area.Controller.CustomObjectConverter);
			int firstInt = reader.ReadInt32();
			if(firstInt >= 0)
				this.savedColumns = Serializerv1.ReadFromStream(stream, reader, Rows, firstInt);
			else
				this.savedColumns = Serializerv2.ReadFromStream(stream, reader, Rows, firstInt);
		}
		protected internal int GetRestoredLevel(bool isWebSaving) {
			int columnCount = Area.Columns.Count - 1;
			if(isWebSaving)
				return columnCount;
			int count = Math.Min(columnCount, savedColumns.Length);
			for(int i = 0; i < count; i++)
				if(!IsColumnEquals(SavedColumns[i], Area.Columns[i].ColumnInfo))
					return i;
			return count;
		}
		protected bool IsColumnEquals(PivotGroupRowsKeeperColumnInfo keeperColumnInfo, DataColumnInfo columnInfo) {
			return keeperColumnInfo.ColumnType == columnInfo.Type && keeperColumnInfo.ColumnName == columnInfo.Name;
		}
		protected PivotGroupRowsKeeperColumnInfo[] SavedColumns { get { return savedColumns; } }
		public void SaveColumns() {
			Clear();
			int count = Area.Columns.Count - 1;
			if(count > 1) {
				if(GroupInfo.LastExpandableLevel >= 0 && GroupInfo.LastExpandableLevel < count)
					count = GroupInfo.LastExpandableLevel;
			}
			if(count < 0)
				count = 0;
			this.savedColumns = new PivotGroupRowsKeeperColumnInfo[count];
			for(int i = 0; i < SavedColumns.Length; i++) {
				SavedColumns[i] = new PivotGroupRowsKeeperColumnInfo(Area.Columns[i].ColumnInfo, Area.Columns[i].ShowOthersValue);
			}
		}
		protected void SaveDefaultExpandedCollasped() {
			int[] expandedCount = new int[SavedColumns.Length];
			for(int i = 0; i < expandedCount.Length; i++)
				expandedCount[i] = GroupInfo.AutoExpandAllGroups ? 0 : -1;
			for(int i = 0; i < GroupInfo.Count; i++) {
				GroupRowInfo groupRow = GroupInfo[i];
				if(!IsRowLevelBefore(groupRow, expandedCount.Length))
					continue;
				if(groupRow.Expanded)
					expandedCount[groupRow.Level]++;
				else
					expandedCount[groupRow.Level]--;
			}
			for(int i = 0; i < expandedCount.Length; i++)
				this.savedColumns[i].IsRowsExpanded = expandedCount[i] >= 0;
		}
		public void SaveRows() {
			Rows.Clear();
			foundRows.Clear();
			SaveDefaultExpandedCollasped();
			int lastLevel = GetRestoredLevel(false);
			for(int i = 0; i < GroupInfo.Count; i++) {
				GroupRowInfo groupRow = GroupInfo[i];
				if(!IsRowLevelBefore(groupRow, lastLevel))
					continue;
				if(groupRow.Expanded != this.savedColumns[groupRow.Level].IsRowsExpanded) {
					SaveRow(groupRow);
				}
			}
		}
		protected void SaveRow(GroupRowInfo groupRow) {
			object[] values = new object[groupRow.Level + 1];
			while(groupRow != null) {
				values[groupRow.Level] = Area.GetValue(groupRow);
				groupRow = groupRow.ParentGroup;
			}
			Rows.Add(values);
		}
		protected void RestoreRows(int restoredLevel) {
			if(restoredLevel == 0)
				return;
			RestoreDefaultExpandedCollapsed(restoredLevel);
			for(int i = 0; i < Rows.Count; i++) {
				if(Rows[i].Level < restoredLevel) {
					GroupRowInfo groupRow = FindRestoredRow(Rows[i]);
					if(groupRow != null) {
						groupRow.Expanded = !SavedColumns[groupRow.Level].IsRowsExpanded;
					}
				}
			}
		}
		protected void RestoreDefaultExpandedCollapsed(int restoredLevel) {
			for(int i = 0; i < GroupInfo.Count; i++) {
				GroupRowInfo groupRow = GroupInfo[i];
				if(!IsRowLevelBefore(groupRow, restoredLevel))
					continue;
				groupRow.Expanded = this.savedColumns[groupRow.Level].IsRowsExpanded;
			}
		}
		protected GroupRowInfo FindRestoredRow(PivotGroupRowKeeper rowKeeper) {
			GroupRowInfo groupRow = FindRestoredRow(rowKeeper, rowKeeper.Level);
			if(groupRow == null)
				return null;
			while(groupRow != null && groupRow.Level < rowKeeper.Level) {
				groupRow = FindRestoredRow(groupRow, rowKeeper);
			}
			return groupRow;
		}
		protected GroupRowInfo FindRestoredRow(PivotGroupRowKeeper rowKeeper, int level) {
			GroupRowInfo groupRow = foundRows[rowKeeper.GetRowHashCode(level)] as GroupRowInfo;
			if(groupRow != null)
				return groupRow;
			if(level > 0)
				return FindRestoredRow(rowKeeper, level - 1);
			for(int i = 0; i < GroupInfo.Count; i++) {
				if(GroupInfo[i].Level != 0)
					continue;
				if(IsEqual(GroupInfo[i], rowKeeper, 0)) {
					return GroupInfo[i];
				}
			}
			return null;
		}
		protected GroupRowInfo FindRestoredRow(GroupRowInfo parentGroupRow, PivotGroupRowKeeper rowKeeper) {
			for(int i = parentGroupRow.Index + 1; i < GroupInfo.Count; i++) {
				GroupRowInfo groupRow = GroupInfo[i];
				if(groupRow.Level <= parentGroupRow.Level)
					break;
				if(groupRow.Level > parentGroupRow.Level + 1)
					continue;
				if(IsEqual(groupRow, rowKeeper, parentGroupRow.Level + 1))
					return groupRow;
			}
			return null;
		}
		protected bool IsEqual(GroupRowInfo groupRow, PivotGroupRowKeeper rowKeeper, int level) {
			if(IsEqual(groupRow, rowKeeper.Values[level])) {
				foundRows[rowKeeper.GetRowHashCode(level)] = groupRow;
				return true;
			}
			return false;
		}
		protected bool IsEqual(GroupRowInfo groupRow, object value) {
			object groupRowValue = Area.GetValue(groupRow);
			bool isGroupRowValueOthersValue = object.ReferenceEquals(groupRowValue, DataControllerGroupHelperBase.OthersValue),
				isValueOthersValue = object.ReferenceEquals(value, DataControllerGroupHelperBase.OthersValue);
			if(isGroupRowValueOthersValue && isValueOthersValue)
				return true;
			if(isGroupRowValueOthersValue || isValueOthersValue)
				return false;
			return Comparer.Default.Compare(groupRowValue, value) == 0;
		}
	}
	public class PivotCollapsedStateSerializerV1 {
		protected PivotGroupRowsKeeperColumnInfo[] SavedColumns;
		protected PivotGroupRowKeeperCollection Rows;
		public PivotCollapsedStateSerializerV1() { }
		void Init(PivotGroupRowKeeperCollection rows, PivotGroupRowsKeeperColumnInfo[] savedColumns) {
			Rows = rows;
			SavedColumns = savedColumns;
		}
		public virtual PivotGroupRowsKeeperColumnInfo[] ReadFromStream(Stream stream, TypedBinaryReader reader, PivotGroupRowKeeperCollection rows, int firstInt) {
			Init(rows, ReadColumns(reader, firstInt));
			ReadRows(reader);
			return SavedColumns;
		}
		public void WriteToStream(TypedBinaryWriter writer, PivotGroupRowKeeperCollection rows, PivotGroupRowsKeeperColumnInfo[] savedColumns) {
			Init(rows, savedColumns);
			WriteVersion(writer);
			WriteColumns(writer);
			WriteRows(writer);
			writer.Flush();
		}
		protected virtual void WriteVersion(TypedBinaryWriter writer) { }
		protected void WriteColumns(TypedBinaryWriter writer) {
			writer.Write(SavedColumns.Length);
			for(int i = 0; i < SavedColumns.Length; i++) {
				SavedColumns[i].Write(writer);
			}
		}
		protected PivotGroupRowsKeeperColumnInfo[] ReadColumns(TypedBinaryReader reader, int count) {
			PivotGroupRowsKeeperColumnInfo[] savedColumns = new PivotGroupRowsKeeperColumnInfo[count];
			for(int i = 0; i < savedColumns.Length; i++) {
				savedColumns[i] = new PivotGroupRowsKeeperColumnInfo();
				savedColumns[i].Read(reader);
			}
			return savedColumns;
		}
		protected virtual void WriteRows(TypedBinaryWriter writer) {
			writer.Write(Rows.Count);
			for(int i = 0; i < Rows.Count; i++) {
				WriteRow(writer, Rows[i]);
			}
		}
		protected virtual void ReadRows(TypedBinaryReader reader) {
			Rows.Clear();
			int count = reader.ReadInt32();
			for(int i = 0; i < count; i++) {
				ReadRow(reader);
			}
		}
		protected void WriteRow(TypedBinaryWriter writer, PivotGroupRowKeeper rowKeeper) {
			writer.Write(rowKeeper.Values.Length);
			for(int i = 0; i < rowKeeper.Values.Length; i++) {
				WriteValue(writer, rowKeeper.Values[i], i);
			}
		}
		protected void ReadRow(TypedBinaryReader reader) {
			int count = reader.ReadInt32();
			object[] values = new object[count];
			for(int i = 0; i < values.Length; i++) {
				values[i] = ReadValue(reader, i);
				if(SavedColumns[i].HasOthers && DataControllerGroupHelperBase.OthersValue.Equals(values[i]))
					values[i] = DataControllerGroupHelperBase.OthersValue;
			}
			Rows.Add(values);
		}
		protected virtual void WriteValue(TypedBinaryWriter writer, object obj, int i) {
			WriteValueCore(writer, obj, i);
		}
		protected virtual object ReadValue(TypedBinaryReader reader, int i) {
			return ReadValueCore(reader, SavedColumns[i]);
		}
		protected void WriteValueCore(TypedBinaryWriter writer, object obj, int i) {
			if(!SavedColumns[i].WriteValueType)
				writer.WriteObject(obj);
			else
				writer.WriteTypedObject(obj);
		}
		protected object ReadValueCore(TypedBinaryReader reader, PivotGroupRowsKeeperColumnInfo column) {
			if(!column.WriteValueType)
				return reader.ReadObject(column.ActualColumnType);
			else
				return reader.ReadTypedObject();
		}
	}
	public class PivotCollapsedStateSerializerV2 : PivotCollapsedStateSerializerV1 {
		NullableDictionary<object, int>[] uniqueValuesDictionary;
		List<object>[] uniqueValuesList;
		public PivotCollapsedStateSerializerV2() { }
		public override PivotGroupRowsKeeperColumnInfo[] ReadFromStream(Stream stream, TypedBinaryReader reader, PivotGroupRowKeeperCollection rows, int firstInt) {
			return base.ReadFromStream(stream, reader, rows, reader.ReadInt32());
		}
		protected override void WriteVersion(TypedBinaryWriter writer) {
			writer.Write(-2);
		}
		protected override void WriteRows(TypedBinaryWriter writer) {
			FillUniqueColumnValues();
			WriteUniqueColumnValues(writer);
			base.WriteRows(writer);
			uniqueValuesDictionary = null;
		}
		protected override void ReadRows(TypedBinaryReader reader) {
			ReadUniqueValues(reader);
			base.ReadRows(reader);
			uniqueValuesList = null;
		}
		void WriteUniqueColumnValues(TypedBinaryWriter writer) {
			writer.Write(uniqueValuesDictionary.Length);
			for(int i = 0; i < uniqueValuesDictionary.Length; i++) {
				writer.Write(uniqueValuesDictionary[i].Count);
				foreach(KeyValuePair<object, int> val in uniqueValuesDictionary[i])
					WriteValueCore(writer, val.Key, i);
			}
		}
		void FillUniqueColumnValues() {
			uniqueValuesDictionary = new NullableDictionary<object, int>[SavedColumns.Length];
			for(int i = 0; i < SavedColumns.Length; i++)
				uniqueValuesDictionary[i] = new NullableDictionary<object, int>();
			for(int i = 0; i < Rows.Count; i++) {
				object[] vals = Rows[i].Values;
				for(int j = 0; j < vals.Length; j++)
					if(!uniqueValuesDictionary[j].Contains(vals[j]))
						uniqueValuesDictionary[j].Add(vals[j], uniqueValuesDictionary[j].Count);
			}
		}
		void ReadUniqueValues(TypedBinaryReader reader) {
			int len = reader.ReadInt32();
			uniqueValuesList = new List<object>[len];
			for(int i = 0; i < len; i++) {
				uniqueValuesList[i] = new List<object>();
				int valCount = reader.ReadInt32();
				for(int j = 0; j < valCount; j++)
					uniqueValuesList[i].Add(ReadValueCore(reader, SavedColumns[i]));
			}
		}
		protected override void WriteValue(TypedBinaryWriter writer, object obj, int i) {
			writer.Write(uniqueValuesDictionary[i][obj]);
		}
		protected override object ReadValue(TypedBinaryReader reader, int i) {
			return uniqueValuesList[i][reader.ReadInt32()];
		}
	}
	public class NullableHashtable {
		static object NullKey = new object();
		Hashtable innerHashtable;
		public NullableHashtable()
			: this(0, null) {
		}
		public NullableHashtable(int capacity)
			: this(capacity, null) {
		}
		public NullableHashtable(int capacity, IEqualityComparer comparer) {
#if !SL
			this.innerHashtable = new Hashtable(capacity, comparer);
#else
			this.innerHashtable = new Hashtable(comparer ?? EqualityComparer<object>.Default);
#endif
		}
		protected Hashtable InnerHashtable {
			get { return innerHashtable; }
		}
		public object this[object key] {
			get {
				if(key == null)
					key = NullKey;
				return InnerHashtable[key];
			}
			set {
				if(key == null)
					key = NullKey;
				InnerHashtable[key] = value;
			}
		}
		public int Count {
			get { return InnerHashtable.Count; }
		}
		public bool ContainsKey(object key) {
			if(key == null)
				key = NullKey;
			return InnerHashtable.ContainsKey(key);
		}
		public bool Contains(object key) {
			return ContainsKey(key);
		}
		public void Add(object key, object value) {
			if(key == null)
				key = NullKey;
			InnerHashtable.Add(key, value);
		}
		public void Add(object key) {
			Add(key, null);
		}
		public void Remove(object key) {
			if(key == null)
				key = NullKey;
			InnerHashtable.Remove(key);
		}
		public void Clear() {
			InnerHashtable.Clear();
		}
		public void CopyKeysTo(Array array, int index) {
			object[] tempArray = new object[Count];
			InnerHashtable.Keys.CopyTo(tempArray, 0);
			int tempIndex = tempArray.Length,
				arrayIndex = index;
			while(--tempIndex >= 0) {
				object key = tempArray[tempIndex];
				if(key == NullKey)
					key = null;
				array.SetValue(key, arrayIndex++);
			}
		}
		public void SwichComparer(IEqualityComparer newComparer) {
#if !SL
			Hashtable newHashtable = new Hashtable(innerHashtable.Count, newComparer);
#else
			Hashtable newHashtable = new Hashtable(newComparer);
#endif
			foreach(DictionaryEntry entry in innerHashtable)
				newHashtable.Add(entry.Key, entry.Value);
			innerHashtable = newHashtable;
		}
		public IEnumerable<object> EnumerateKeys() {
			foreach(object key in InnerHashtable.Keys)
				if(key == NullKey)
					yield return null;
				else
					yield return key;
		}
		public void CopyValuesTo(Array array, int index) {
			InnerHashtable.Values.CopyTo(array, index);
		}
	}
	public class NullableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>> where TKey : class {
		Dictionary<TKey, TValue> innerDic;
		bool hasNullValue;
		TValue nullValue;
		int modificationsCount;
		public NullableDictionary() {
			this.innerDic = new Dictionary<TKey, TValue>();
		}
		public NullableDictionary(int capacity) {
			this.innerDic = new Dictionary<TKey, TValue>(capacity);
		}
		public bool HasNullValue { get { return hasNullValue; } }
		public TValue NullValue { get { return nullValue; } }
		public int Count {
			get {
				return hasNullValue ? innerDic.Count + 1 : innerDic.Count;
			}
		}
		public TValue this[TKey index] {
			get {
				if(index == null) {
					if(hasNullValue)
						return nullValue;
					throw new ArgumentException("not found");
				}
				return innerDic[index];
			}
			set {
				if(index == null) {
					nullValue = value;
					hasNullValue = true;
				} else {
					innerDic[index] = value;
				}
				OnChanged();
			}
		}
		public IEnumerable<TKey> Keys {
			get {
				if(hasNullValue)
					return new KeysCollection(this);
				else
					return innerDic.Keys;
			}
		}
		public IEnumerable<TValue> Values {
			get {
				if(hasNullValue)
					return new ValuesCollection(this);
				else
					return innerDic.Values;
			}
		}
		public IEnumerable<TValue> NotNullValues {
			get { return innerDic.Values; }
		}
		public IEnumerable<TKey> NotNullKeys {
			get { return innerDic.Keys; }
		}
		public void Add(TKey key, TValue value) {
			if(key != null) {
				innerDic.Add(key, value);
				OnChanged();
				return;
			}
			if(hasNullValue)
				throw new ArgumentException("duplicate");
			nullValue = value;
			hasNullValue = true;
			OnChanged();
		}
		public bool Remove(TKey key) {
			bool changed = false;
			if(key == null) {
				changed = hasNullValue;
				hasNullValue = false;
			} else
				changed = innerDic.Remove(key);
			OnChanged();
			return changed;
		}
		public void Clear() {
			innerDic.Clear();
			hasNullValue = false;
			OnChanged();
		}
		public bool Contains(TKey key) {
			return key == null ? hasNullValue : innerDic.ContainsKey(key);
		}
		public bool ContainsKey(TKey key) {
			return Contains(key);
		}
		public bool TryGetValue(TKey key, out TValue value) {
			if(key == null) {
				value = hasNullValue ? nullValue : default(TValue);
				return hasNullValue;
			}
			return innerDic.TryGetValue(key, out value);
		}
		void OnChanged() {
			modificationsCount++;
		}
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
			return new PairEnumerator(this);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		abstract class BaseEnumerator : IEnumerator, IDisposable {
			protected enum State { None, Null, InnerDic };
			NullableDictionary<TKey, TValue> owner;
			int modificationsCount;
			IEnumerator<KeyValuePair<TKey, TValue>> innerEnum;
			State state;
			public BaseEnumerator(NullableDictionary<TKey, TValue> owner) {
				this.owner = owner;
				Reset();
			}
			protected abstract object CurrentImpl { get; }
			protected State CurrentState { get { return state; } }
			protected NullableDictionary<TKey, TValue> Owner { get { return owner; } }
			protected IEnumerator<KeyValuePair<TKey, TValue>> InnerEnum { get { return innerEnum; } }
#region IDisposable Members
			void IDisposable.Dispose() {
				Reset();
			}
#endregion
#region IEnumerator Members
			object IEnumerator.Current { get { return CurrentImpl; } }
			bool IEnumerator.MoveNext() {
				if(modificationsCount != owner.modificationsCount)
					throw new InvalidOperationException("collection modified");
				switch(state) {
					case State.None:
						if(owner.hasNullValue) {
							state = State.Null;
							return true;
						} else {
							state = State.InnerDic;
							innerEnum = owner.innerDic.GetEnumerator();
							return innerEnum.MoveNext();
						}
					case State.Null:
						state = State.InnerDic;
						innerEnum = owner.innerDic.GetEnumerator();
						return innerEnum.MoveNext();
					case State.InnerDic:
						return innerEnum.MoveNext();
				}
				throw new InvalidOperationException("invalid state");
			}
			void IEnumerator.Reset() {
				Reset();
			}
#endregion
			void Reset() {
				state = State.None;
				if(innerEnum != null)
					innerEnum.Dispose();
				innerEnum = null;
				modificationsCount = owner.modificationsCount;
			}
		}
		class PairEnumerator : BaseEnumerator, IEnumerator<KeyValuePair<TKey, TValue>> {
			public PairEnumerator(NullableDictionary<TKey, TValue> owner)
				: base(owner) {
			}
			public KeyValuePair<TKey, TValue> Current {
				get {
					switch(CurrentState) {
						case State.Null:
							return new KeyValuePair<TKey, TValue>(null, Owner.nullValue);
						case State.InnerDic:
							return InnerEnum.Current;
					}
					throw new InvalidOperationException("invalid state");
				}
			}
			protected override object CurrentImpl {
				get { return Current; }
			}
		}
		class KeyEnumerator : BaseEnumerator, IEnumerator<TKey> {
			public KeyEnumerator(NullableDictionary<TKey, TValue> owner)
				: base(owner) {
			}
			public TKey Current {
				get {
					switch(CurrentState) {
						case State.Null:
							return null;
						case State.InnerDic:
							return InnerEnum.Current.Key;
					}
					throw new InvalidOperationException("invalid state");
				}
			}
			protected override object CurrentImpl {
				get { return Current; }
			}
		}
		class KeysCollection : IEnumerable<TKey> {
			readonly NullableDictionary<TKey, TValue> owner;
			public KeysCollection(NullableDictionary<TKey, TValue> owner) {
				this.owner = owner;
			}
			IEnumerator<TKey> GetEnumeratorCore() {
				return new KeyEnumerator(owner);
			}
#region IEnumerable<TKey> Members
			IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() {
				return GetEnumeratorCore();
			}
#endregion
#region IEnumerable Members
			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumeratorCore();
			}
#endregion
		}
		class ValueEnumerator : BaseEnumerator, IEnumerator<TValue> {
			public ValueEnumerator(NullableDictionary<TKey, TValue> owner)
				: base(owner) {
			}
			public TValue Current {
				get {
					switch(CurrentState) {
						case State.Null:
							return Owner.nullValue;
						case State.InnerDic:
							return InnerEnum.Current.Value;
					}
					throw new InvalidOperationException("invalid state");
				}
			}
			protected override object CurrentImpl {
				get { return Current; }
			}
		}
		class ValuesCollection : IEnumerable<TValue> {
			readonly NullableDictionary<TKey, TValue> owner;
			public ValuesCollection(NullableDictionary<TKey, TValue> owner) {
				this.owner = owner;
			}
			IEnumerator<TValue> GetEnumeratorCore() {
				return new ValueEnumerator(owner);
			}
#region IEnumerable<TValue> Members
			IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() {
				return GetEnumeratorCore();
			}
#endregion
#region IEnumerable Members
			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumeratorCore();
			}
#endregion
		}
		ICollection<TKey> IDictionary<TKey, TValue>.Keys {
			get { return hasNullValue ? innerDic.Keys.Concat(new TKey[] { null }).ToList() : (ICollection<TKey>)innerDic.Keys; }
		}
		ICollection<TValue> IDictionary<TKey, TValue>.Values {
			get { return hasNullValue ? innerDic.Values.Concat(new TValue[] { nullValue }).ToList() : (ICollection<TValue>)innerDic.Values; }
		}
		public void Add(KeyValuePair<TKey, TValue> item) {
			Add(item.Key, item.Value);
		}
		public bool Contains(KeyValuePair<TKey, TValue> item) {
			return Contains(item.Key) && object.Equals(this[item.Key], item.Value);
		}
		public void CopyToStartNullable(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
			if(hasNullValue) {
				array[arrayIndex] = new KeyValuePair<TKey, TValue>(null, nullValue);
				((IDictionary<TKey, TValue>)innerDic).CopyTo(array, arrayIndex + 1);
			} else {
				((IDictionary<TKey, TValue>)innerDic).CopyTo(array, arrayIndex);
			}
		}
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
			((IDictionary<TKey, TValue>)innerDic).CopyTo(array, arrayIndex);
			if(hasNullValue)
				array[arrayIndex + Count - 1] = new KeyValuePair<TKey, TValue>(null, nullValue);
		}
		public bool IsReadOnly {
			get { return false; }
		}
		public bool Remove(KeyValuePair<TKey, TValue> item) {
			if(Contains(item))
				return Remove(item.Key);
			return false;
		}
		public void CopyValuesTo(TValue[] list, int startIndex) {
			innerDic.Values.CopyTo(list, startIndex);
			if(HasNullValue)
				list[startIndex + Count - 1] = nullValue;
		}
		public void CopyValuesTo(TValue[] list) {
			innerDic.Values.CopyTo(list, 0);
			if(HasNullValue)
				list[Count - 1] = nullValue;
		}
	}
	public static class ListComparer {
		public delegate bool ObjectCompareDelegate(object x, object y);
		public static bool ListsEqual(IList list1, IList list2) {
			if(list1.Count != list2.Count)
				return false;
			for(int i = 0; i < list1.Count; i++) {
				if(list1[i] != list2[i])
					return false;
			}
			return true;
		}
		public static bool IsOnlyLastItemsAdded(IList source, IList newList) {
			if(source.Count >= newList.Count)
				return false;
			for(int i = 0; i < source.Count; i++) {
				if(source[i] != newList[i])
					return false;
			}
			return true;
		}
		public static bool IsOnlyLastItemsRemoved(IList source, IList newList) {
			if(source.Count <= newList.Count)
				return false;
			for(int i = 0; i < newList.Count; i++) {
				if(source[i] != newList[i])
					return false;
			}
			return true;
		}
		public static bool ListsEqual(IList list1, IList list2, ObjectCompareDelegate compare) {
			if(list1.Count != list2.Count)
				return false;
			for(int i = 0; i < list1.Count; i++) {
				if(!compare(list1[i], list2[i]))
					return false;
			}
			return true;
		}
	}
	public class ExpNamePropertyDescriptor : PD.PropertyDescriptor {
		PD.PropertyDescriptor pd;
		string expFieldName;
		public ExpNamePropertyDescriptor(PD.PropertyDescriptor pd, string expName)
			: base(expName, null) {
			this.pd = pd;
			this.expFieldName = expName;
		}
		protected PD.PropertyDescriptor PD { get { return pd; } }
		public override bool IsBrowsable { get { return PD.IsBrowsable; } }
		public override bool IsReadOnly { get { return PD.IsReadOnly; } }
		public override string Category { get { return string.Empty; } }
		public override string Name { get { return expFieldName; } }
		public override Type PropertyType { get { return PD.PropertyType; } }
		public override Type ComponentType { get { return PD.PropertyType; } }
		public override void ResetValue(object component) { }
		public override bool CanResetValue(object component) { return false; }
		public override object GetValue(object component) {
			return PD.GetValue(component);
		}
		public override void SetValue(object component, object value) {
			PD.SetValue(component, value);
		}
		public override bool ShouldSerializeValue(object component) { return false; }
		public override string ToString() {
			return Name;
		}
	}
	public class PivotDataColumnInfoCollection : DataColumnInfoCollection, IEnumerable<KeyValuePair<string, DataColumnInfo>> {
		Dictionary<string, DataColumnInfo> expNameHash;
		protected Dictionary<string, DataColumnInfo> ExpNameHash {
			get { return expNameHash; }
		}
		public PivotDataColumnInfoCollection()
			: base() {
			expNameHash = new Dictionary<string, DataColumnInfo>();
		}
		public void AddExpNameHash(string expName, DataColumnInfo column) {
			ExpNameHash[expName] = column;
			NameHash[expName] = column;
		}
		public DataColumnInfo GetColumnByExpName(string expName) {
			if(ExpNameHash.ContainsKey(expName))
				return ExpNameHash[expName];
			return null;
		}
		protected override void OnRemoveComplete(int position, object item) {
			base.OnRemoveComplete(position, item);
			DataColumnInfo col = item as DataColumnInfo;
			if(col.Tag != null) {
				string expName = ((PivotGridFieldBase)(col.Tag)).ExpressionFieldName;
				if(ExpNameHash.ContainsKey(expName))
					ExpNameHash.Remove(expName);
				if(NameHash.ContainsKey(expName))
					NameHash.Remove(expName);
			}
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			ExpNameHash.Clear();
		}
		IEnumerator<KeyValuePair<string, DataColumnInfo>> IEnumerable<KeyValuePair<string, DataColumnInfo>>.GetEnumerator() {
			return expNameHash.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return expNameHash.GetEnumerator();
		}
	}
	public static class PivotGridSerializeHelper {
		public static string ToBase64String(Action<TypedBinaryWriter> action, ICustomObjectConverter converter) {
			using(MemoryStream stream = new MemoryStream()) {
				using(TypedBinaryWriter writer = TypedBinaryWriter.CreateWriter(stream, converter)) {
					action(writer);
					return Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length);
				}
			}
		}
		public static string ToBase64StringDeflateBuffered(Action<TypedBinaryWriter> action) {
			using(MemoryStream stream = new MemoryStream()) {
				using(DeflateStream compressor = new DeflateStream(stream, CompressionMode.Compress, true)) {
					using(BufferedStream buffered = new BufferedStream(compressor)) {
						using(TypedBinaryWriter writer = new TypedBinaryWriter(buffered)) {
							action(writer);
						}
					}
				}
				try {
					return Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length);
				} catch {
					return Convert.ToBase64String(stream.ToArray());
				}
			}
		}
		public static string ToBase64StringDeflateBuffered(Action<TypedBinaryWriter> action, ICustomObjectConverter converter) {
			using(MemoryStream stream = new MemoryStream()) {
				using(DeflateStream compressor = new DeflateStream(stream, CompressionMode.Compress, true)) {
					using(BufferedStream buffered = new BufferedStream(compressor)) {
						using(TypedBinaryWriter writer = TypedBinaryWriter.CreateWriter(buffered, converter)) {
							action(writer);
						}
					}
				}
				try {
					return Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length);
				} catch {
					return Convert.ToBase64String(stream.ToArray());
				}
			}
		}	
	}
	public interface IPivotCustomSummaryAggregate : ICustomFunctionOperatorBrowsable {
		object Calculate(IEnumerable<object> enumerable);
	}
}
