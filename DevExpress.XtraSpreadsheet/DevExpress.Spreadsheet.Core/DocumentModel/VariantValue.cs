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
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public enum DateSystem {
		Date1900,
		Date1904
	}
	#region VariantValueType
	public enum VariantValueType {
		None,
		Error,
		Boolean,
		InlineText,
		SharedString,
		Numeric,
		Missing,
		Array,
		CellRange,
	}
	#endregion
	#region VariantValue
	public struct VariantValue {
		const string trueConstant = "TRUE";
		const string falseConstant = "FALSE";
		static readonly DateTime baseDate = new DateTime(1899, 12, 30);
		static readonly DateTime baseDate1904 = new DateTime(1904, 1, 1);
		static readonly TimeSpan day29Feb1900 = TimeSpan.FromDays(60);
		public static DateTime BaseDate { get { return baseDate; } }
		public static DateTime BaseDate1904 { get { return baseDate1904; } }
		public static TimeSpan Day29Feb1900 { get { return day29Feb1900; } }
		public static string TrueConstant { get { return trueConstant; } }
		public static string FalseConstant { get { return falseConstant; } }
		public static readonly VariantValue Empty = new VariantValue();
		public static readonly VariantValue Missing = CreateMissing();
		public static readonly VariantValue ErrorInvalidValueInFunction = VariantValue.CreateError(InvalidValueInFunctionError.Instance);
		public static readonly VariantValue ErrorDivisionByZero = VariantValue.CreateError(DivisionByZeroError.Instance);
		public static readonly VariantValue ErrorNumber = VariantValue.CreateError(NumberError.Instance);
		public static readonly VariantValue ErrorReference = VariantValue.CreateError(ReferenceError.Instance);
		public static readonly VariantValue ErrorValueNotAvailable = VariantValue.CreateError(ValueNotAvailableError.Instance);
		public static readonly VariantValue ErrorNullIntersection = VariantValue.CreateError(NullIntersectionError.Instance);
		public static readonly VariantValue ErrorName = VariantValue.CreateError(NameError.Instance);
		public static readonly VariantValue ErrorGettingData = VariantValue.CreateError(GettingDataError.Instance);
		static readonly VariantValue[] errors = new VariantValue[] {
			ErrorValueNotAvailable,
			ErrorNumber,
			ErrorName,
			ErrorReference,
			ErrorInvalidValueInFunction,
			ErrorDivisionByZero,
			ErrorNullIntersection,
			ErrorGettingData
		};
		public static VariantValue[] Errors { get { return errors; } }
		double numericValue; 
		object referenceValue;
		VariantValueType type; 
		[DebuggerStepThrough]
		static VariantValue CreateError(ICellError value) {
			VariantValue result = new VariantValue();
			result.type = VariantValueType.Error;
			result.referenceValue = value;
			return result;
		}
		[DebuggerStepThrough]
		static VariantValue CreateMissing() {
			VariantValue result = new VariantValue();
			result.type = VariantValueType.Missing;
			return result;
		}
		internal VariantValueType InversedSortType {
			get {
				if (type == VariantValueType.SharedString)
					return VariantValueType.InlineText;
				else
					return type;
			}
		}
		public VariantValueType Type { [DebuggerStepThrough] get { return type; } }
		public bool IsEmpty { [DebuggerStepThrough] get { return type == VariantValueType.None; } }
		public bool IsMissing { [DebuggerStepThrough] get { return type == VariantValueType.Missing; } }
		public bool IsNumeric { [DebuggerStepThrough] get { return type == VariantValueType.Numeric; } }
		public bool IsBoolean { [DebuggerStepThrough] get { return type == VariantValueType.Boolean; } }
		public bool IsError { [DebuggerStepThrough] get { return type == VariantValueType.Error; } }
		public bool IsArray { [DebuggerStepThrough] get { return type == VariantValueType.Array; } }
		public bool IsCellRange { [DebuggerStepThrough] get { return type == VariantValueType.CellRange; } }
		public bool IsText { [DebuggerStepThrough] get { return IsInlineText || IsSharedString; } }
		public bool IsInlineText { [DebuggerStepThrough] get { return type == VariantValueType.InlineText; } }
		public bool IsSharedString { [DebuggerStepThrough] get { return type == VariantValueType.SharedString; } }
		public double NumericValue {
			[DebuggerStepThrough]
			get { return numericValue; }
			[DebuggerStepThrough]
			set {
				this.type = VariantValueType.Numeric;
				this.numericValue = value;
				this.referenceValue = null;
			}
		}
		public SharedStringIndex SharedStringIndexValue {
			[DebuggerStepThrough]
			get { return new SharedStringIndex((int)numericValue); }
		}
		public void SetSharedString(SharedStringTable sharedStringTable, SharedStringIndex value) {
			this.type = VariantValueType.SharedString;
			this.numericValue = value.ToInt();
		}
		public void SetSharedString(SharedStringTable sharedStringTable, string value) {
			SharedStringIndex index = sharedStringTable.RegisterString(value);
			SetSharedString(sharedStringTable, index);
		}
		public static VariantValue FromSharedString(int index) {
			VariantValue result = new VariantValue();
			result.numericValue = index;
			result.type = VariantValueType.SharedString;
			return result;
		}
		public static VariantValue FromArray(IVariantArray array) {
			VariantValue result = new VariantValue();
			result.ArrayValue = array;
			return result;
		}
		public bool BooleanValue {
			[DebuggerStepThrough]
			get { return this.numericValue != 0; }
			[DebuggerStepThrough]
			set {
				this.type = VariantValueType.Boolean;
				this.numericValue = value ? 1 : 0;
				this.referenceValue = null;
			}
		}
		public DateTime ToDateTime(WorkbookDataContext context) {
			return context.FromDateTimeSerial(this.numericValue);
		}
		public DateTime ToDateTime(DateSystem dateSystem) {
			return WorkbookDataContext.FromDateTimeSerial(this.numericValue, dateSystem);
		}
		public void SetDateTime(DateTime value, WorkbookDataContext context) {
			SetDateTime(value, context.DateSystem);
		}
		public void SetDateTime(DateTime value, DateSystem dateSystem) {
			bool onlyTimeSpecified = value.Year == 1 && value.Month == 1 && value.Day == 1;
			if (onlyTimeSpecified) {
				if (dateSystem == DateSystem.Date1900)
					value = new DateTime(1899, 12, 31).AddTicks(value.Ticks);
				else
					value = new DateTime(1904, 1, 1).AddTicks(value.Ticks);
			}
			else {
				if (dateSystem == DateSystem.Date1900) {
					if (value <= VariantValue.baseDate.AddDays(1)) {
						this.ErrorValue = InvalidValueInFunctionError.Instance;
						return;
					}
				}
				else {
					if (value < VariantValue.baseDate1904) {
						this.ErrorValue = InvalidValueInFunctionError.Instance;
						return;
					}
				}
			}
			this.type = VariantValueType.Numeric;
			this.numericValue = WorkbookDataContext.ToDateTimeSerialDouble(value, dateSystem);
			this.referenceValue = null;
		}
		public IVariantArray ArrayValue {
			[DebuggerStepThrough]
			get { return this.referenceValue as IVariantArray; }
			[DebuggerStepThrough]
			set {
				this.type = VariantValueType.Array;
				this.numericValue = 0;
				this.referenceValue = value;
			}
		}
		public CellRangeBase CellRangeValue {
			[DebuggerStepThrough]
			get { return this.referenceValue as CellRangeBase; }
			[DebuggerStepThrough]
			set {
				this.type = VariantValueType.CellRange;
				this.numericValue = 0;
				this.referenceValue = value;
			}
		}
		public string GetTextValue(SharedStringTable stringTable) {
			if (IsSharedString) {
				ISharedStringItem item = stringTable[SharedStringIndexValue];
				FormattedStringItem formattedContent = item as FormattedStringItem;
				if (formattedContent != null)
					return formattedContent.GetPlainText();
				else
					return item.Content;
			}
			else
				return referenceValue as string;
		}
		public string InlineTextValue {
			[DebuggerStepThrough]
			get { return referenceValue as string; }
			set {
				if (value == null)
					value = String.Empty;
				this.type = VariantValueType.InlineText;
				this.numericValue = 0;
				this.referenceValue = value;
			}
		}
		public ICellError ErrorValue {
			[DebuggerStepThrough]
			get { return this.referenceValue as ICellError; }
			[DebuggerStepThrough]
			set {
				this.type = VariantValueType.Error;
				this.numericValue = 0;
				this.referenceValue = value;
			}
		}
		[DebuggerStepThrough]
		public override int GetHashCode() {
			if (this.referenceValue == null)
				return ((int)type << 29) ^ this.numericValue.GetHashCode();
			else
				return ((int)type << 29) ^ this.numericValue.GetHashCode() ^ this.referenceValue.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!(obj is VariantValue))
				return false;
			VariantValue value = (VariantValue)obj;
			if (value.type == type && value.numericValue == this.numericValue) {
				if (type == VariantValueType.InlineText)
					return Object.Equals(this.referenceValue as string, value.referenceValue as string);
				return Object.ReferenceEquals(value.referenceValue, this.referenceValue);
			}
			else
				return false;
		}
		public bool IsEqual(VariantValue value, StringComparison stringComparison, SharedStringTable stringTable) {
			if (value.type == type && value.numericValue == this.numericValue) {
				if (IsText)
					return String.Compare(GetTextValue(stringTable), value.GetTextValue(stringTable), stringComparison) == 0;
				return Object.ReferenceEquals(value.referenceValue, this.referenceValue);
			}
			else {
				if (IsText && value.IsText)
					return String.Compare(GetTextValue(stringTable), value.GetTextValue(stringTable), stringComparison) == 0;
				return false;
			}
		}
		[DebuggerStepThrough]
		public static bool operator ==(VariantValue first, VariantValue second) {
			if (first.type == second.type && first.numericValue == second.numericValue) {
				if (first.Type == VariantValueType.InlineText)
					return (first.referenceValue as string) == (second.referenceValue as string);
				return Object.ReferenceEquals(first.referenceValue, second.referenceValue);
			}
			else
				return false;
		}
		[DebuggerStepThrough]
		public static bool operator !=(VariantValue first, VariantValue second) {
			if (first.type != second.type || first.numericValue != second.numericValue)
				return true;
			if (first.Type == VariantValueType.InlineText)
				return (first.referenceValue as string) != (second.referenceValue as string);
			return !Object.ReferenceEquals(first.referenceValue, second.referenceValue);
		}
		[DebuggerStepThrough]
		public static implicit operator VariantValue(double value) {
			VariantValue result = new VariantValue();
			result.NumericValue = value;
			return result;
		}
		[DebuggerStepThrough]
		public static implicit operator VariantValue(char value) {
			VariantValue result = new VariantValue();
			result.InlineTextValue = char.ToString(value);
			return result;
		}
		[DebuggerStepThrough]
		public static implicit operator VariantValue(string value) {
			if (value == null)
				return VariantValue.Empty;
			VariantValue result = new VariantValue();
			result.InlineTextValue = value;
			return result;
		}
		[DebuggerStepThrough]
		public static implicit operator VariantValue(bool value) {
			VariantValue result = new VariantValue();
			result.BooleanValue = value;
			return result;
		}
		[DebuggerStepThrough]
		public static implicit operator VariantValue(CellRangeBase value) {
			if (value == null)
				return VariantValue.Empty;
			VariantValue result = new VariantValue();
			result.CellRangeValue = value;
			return result;
		}
		[DebuggerStepThrough]
		public static implicit operator VariantValue(VariantValue[,] value) {
			VariantValue result = new VariantValue();
			VariantArray array = new VariantArray();
			array.SetValues(value);
			result.ArrayValue = array;
			return result;
		}
		public VariantValue ToBoolean(WorkbookDataContext context) {
			Guard.ArgumentNotNull(context, "context");
			switch (type) {
				case VariantValueType.Missing:
				case VariantValueType.None:
					return false;
				case VariantValueType.InlineText:
				case VariantValueType.SharedString: {
						Worksheet sheet = context.CurrentWorksheet as Worksheet;
						if (sheet != null && sheet.Properties.TransitionOptions.TransitionFormulaEvaluation)
							return false;
						string stringValue = GetTextValue(context.StringTable);
						bool boolValue;
						if (context.TryParseBoolean(stringValue, out boolValue))
							return boolValue;
						return VariantValue.ErrorInvalidValueInFunction;
					}
				case VariantValueType.Numeric:
					return NumericValue != 0;
				case VariantValueType.Boolean:
					return this;
				case VariantValueType.Array:
					if (ArrayValue.Count >= 1)
						return ArrayValue[0].ToBoolean(context);
					return VariantValue.ErrorInvalidValueInFunction;
				case VariantValueType.CellRange:
					if (CellRangeValue.RangeType == CellRangeType.UnionRange)
						return VariantValue.ErrorInvalidValueInFunction;
					if (CellRangeValue.CellCount > 1)
						return context.FindCrossing(CellRangeValue).ToBoolean(context);
					ICellBase cell = CellRangeValue.GetFirstCellUnsafe();
					if (cell == null)
						return false;
					VariantValue result;
					cell.Sheet.Workbook.CalculationChain.TryGetCalculatedValue(cell, out result);
					return result.ToBoolean(context);
				default:
				case VariantValueType.Error:
					return this;
			}
		}
		public VariantValue ToNumeric(WorkbookDataContext context) {
			return ToNumeric(context, false);
		}
		public VariantValue ToNumeric(WorkbookDataContext context, bool ignoreTransitionOptions) {
			Guard.ArgumentNotNull(context, "context");
			switch (type) {
				case VariantValueType.None:
				case VariantValueType.Missing:
					return 0;
				case VariantValueType.InlineText:
				case VariantValueType.SharedString: {
						Worksheet sheet = context.CurrentWorksheet as Worksheet;
						if (!ignoreTransitionOptions && sheet != null && sheet.Properties.TransitionOptions.TransitionFormulaEvaluation)
							return 0;
						string textValue = GetTextValue(context.StringTable);
						return context.ConvertTextToNumericWithCaching(textValue);
					}
				case VariantValueType.Numeric:
					return this;
				case VariantValueType.Boolean:
					return BooleanValue ? 1 : 0;
				case VariantValueType.Array:
					if (ArrayValue.Count >= 1)
						return ArrayValue[0].ToNumeric(context);
					return VariantValue.ErrorInvalidValueInFunction;
				case VariantValueType.CellRange:
					if (CellRangeValue.RangeType == CellRangeType.UnionRange)
						return VariantValue.ErrorInvalidValueInFunction;
					if (CellRangeValue.CellCount > 1)
						return context.FindCrossing(CellRangeValue).ToNumeric(context);
					ICellBase cell = CellRangeValue.GetFirstCellUnsafe();
					if (cell == null)
						return 0;
					VariantValue result;
					cell.Sheet.Workbook.CalculationChain.TryGetCalculatedValue(cell, out result);
					return result.ToNumeric(context);
				default:
				case VariantValueType.Error:
					return this;
			}
		}
		#region StringToDouble
		public static VariantValue ConvertStringToDouble(string textValue, WorkbookDataContext context) {
			if (String.IsNullOrEmpty(textValue))
				return VariantValue.ErrorInvalidValueInFunction;
			VariantValue value = ConvertStringToDoubleCore(textValue, context);
			if (value.IsError) {
				textValue = textValue.Trim();
				int length = textValue.Length;
				if (length > 0 && textValue[length - 1] == '%') {
					textValue = textValue.TrimEnd('%');
					value = ConvertStringToDoubleCore(textValue, context);
					if (value.IsError)
						return value;
					int numerOfPercents = length - textValue.Length;
					int divisor = 1;
					for (int i = 0; i < numerOfPercents; i++)
						divisor *= 100;
					return value.NumericValue / divisor;
				}
			}
			return value;
		}
		public static VariantValue ConvertStringToDoubleCore(string textValue, WorkbookDataContext context) {
			if (String.IsNullOrEmpty(textValue))
				return VariantValue.ErrorInvalidValueInFunction;
			const NumberStyles noThousandsStyle = NumberStyles.Any & (~NumberStyles.AllowThousands);
			double value;
			string doubleTextValue = DeleteLeadingSpaces(textValue, context);
			if (double.TryParse(doubleTextValue, noThousandsStyle, context.Culture, out value))
				return value;
			if (double.TryParse(doubleTextValue, NumberStyles.Any, context.Culture, out value)) {
				if (ValidateThousandSeparatorPositions(textValue, context.Culture))
					return value;
				else
					return VariantValue.ErrorInvalidValueInFunction;
			}
			FormattedVariantValue formattedFromDateTimeValue = context.TryConvertStringToDateTimeValue(textValue, false);
			VariantValue fromDateTimeValue = formattedFromDateTimeValue.Value;
			if (!fromDateTimeValue.IsError && !fromDateTimeValue.IsEmpty)
				return fromDateTimeValue;
			return VariantValue.ErrorInvalidValueInFunction;
		}
		static string DeleteLeadingSpaces(string textValue, WorkbookDataContext context) {
			textValue = textValue.Trim();
			if (!string.IsNullOrEmpty(textValue)) {
				int i = 0;
				if (textValue.StartsWith(context.Culture.NumberFormat.PositiveSign))
					i = context.Culture.NumberFormat.PositiveSign.Length;
				else
					if (textValue.StartsWith(context.Culture.NumberFormat.NegativeSign))
						i = context.Culture.NumberFormat.NegativeSign.Length;
				if (i != 0) {
					int signEndPosition = i;
					while (i < textValue.Length && Char.IsWhiteSpace(textValue[i])) {
						i++;
					}
					textValue = textValue.Remove(signEndPosition, i - signEndPosition);
				}
			}
			return textValue;
		}
		static bool ValidateThousandSeparatorPositions(string textValue, CultureInfo cultureInfo) {
			NumberFormatInfo info = cultureInfo.NumberFormat;
			string groupSeparator = info.NumberGroupSeparator;
			if (String.IsNullOrEmpty(groupSeparator))
				return true;
			int[] groupSizes = info.NumberGroupSizes;
			if (groupSizes == null || groupSizes.Length <= 0)
				return true;
			string decimalSeparator = info.NumberDecimalSeparator;
			int startIndex = -1;
			if (!String.IsNullOrEmpty(decimalSeparator))
				startIndex = textValue.IndexOf(decimalSeparator[0]) - 1;
			if (startIndex < 0)
				startIndex = textValue.Length - 1;
			if (startIndex < 0)
				return false;
			int groupIndex = 0;
			int groupSeparatorIndex = startIndex - groupSizes[groupIndex % groupSizes.Length];
			for (int i = startIndex; i >= 0; i--) {
				if (textValue[i] == groupSeparator[0]) {
					if (i > groupSeparatorIndex)
						return false;
					groupIndex++;
					groupSeparatorIndex -= groupSizes[groupIndex % groupSizes.Length] + 1;
				}
			}
			return true;
		}
		#endregion
		public VariantValue ToText(WorkbookDataContext context) {
			Guard.ArgumentNotNull(context, "context");
			switch (type) {
				case VariantValueType.None:
				case VariantValueType.Missing:
					return String.Empty;
				case VariantValueType.InlineText:
				case VariantValueType.SharedString:
					return GetTextValue(context.StringTable);
				case VariantValueType.Numeric:
					return context.ConvertNumberToText(NumericValue);
				case VariantValueType.Boolean:
					return BooleanValue ? trueConstant : falseConstant;
				case VariantValueType.Array:
					if (ArrayValue.Count >= 1)
						return ArrayValue[0].ToText(context);
					return VariantValue.ErrorInvalidValueInFunction;
				case VariantValueType.CellRange:
					return context.DereferenceValue(this, false).ToText(context);
				default:
				case VariantValueType.Error:
					return this;
			}
		}
		[DebuggerStepThrough]
		public override string ToString() {
			return ToString(null);
		}
		[DebuggerStepThrough]
		public string ToString(SharedStringTable stringTable) {
			switch (type) {
				case VariantValueType.None:
					return "Value=None, Type=None";
				case VariantValueType.Missing:
					return "Value=Missing, Type=Missing";
				case VariantValueType.SharedString:
					if (stringTable == null)
						return "Value='" + this.SharedStringIndexValue.ToInt().ToString(CultureInfo.InvariantCulture) + "', Type=SharedString";
					else
						return "Value='" + stringTable[this.SharedStringIndexValue].Content + "' (" + this.SharedStringIndexValue.ToInt().ToString(CultureInfo.InvariantCulture) + "), Type=SharedString";
				case VariantValueType.InlineText:
					return "Value='" + (this.referenceValue as string) + "', Type=InlineText";
				case VariantValueType.Numeric:
					return "Value=" + NumericValue.ToString(CultureInfo.InvariantCulture) + ", Type=Numeric, ExactValue=" + DevExpress.XtraSpreadsheet.Utils.DoubleConverter.ToExactString(NumericValue);
				case VariantValueType.Boolean:
					return "Value=" + BooleanValue.ToString(CultureInfo.InvariantCulture) + ", Type=Boolean";
				case VariantValueType.Array:
					string result = "Value=<array>, Type=Array, Values=<not available>";
					return result;
				case VariantValueType.CellRange:
					return "Value=" + CellRangeValue.ToString() + ", Type=CellRange";
				default:
				case VariantValueType.Error:
					return "Value='" + ErrorValue.Name + "', Value=Error";
			}
		}
		public static VariantValue Create(CellRangeBase cellRange) {
			VariantValue result = new VariantValue();
			result.CellRangeValue = cellRange;
			return result;
		}
		public void ChangeType(VariantValueType type) {
			System.Diagnostics.Debug.Assert(IsEmpty);
			System.Diagnostics.Debug.Assert(type != VariantValueType.CellRange);
			System.Diagnostics.Debug.Assert(type != VariantValueType.Error);
			if (type == VariantValueType.SharedString)
				type = VariantValueType.InlineText;
			if (type == VariantValueType.InlineText)
				this.InlineTextValue = String.Empty;
			else if (type == VariantValueType.Array) {
				VariantArray array = new VariantArray();
				this.ArrayValue = array;
				array.Values = new VariantValue[] { };
			}
			else {
				this.type = type;
				this.numericValue = 0;
			}
		}
		public bool CanBeStoredAsUInt16() {
			if (!this.IsNumeric)
				return false;
			double numericValue = this.NumericValue;
			double truncated = WorksheetFunctionBase.Truncate(numericValue);
			return truncated == numericValue && truncated >= 0 && truncated <= UInt16.MaxValue;
		}
	}
	#endregion
	public struct FormattedVariantValue {
		readonly static FormattedVariantValue empty = new FormattedVariantValue(VariantValue.Empty, 0);
		public static FormattedVariantValue Empty { get { return empty; } }
		int numberFormatId;
		VariantValue value;
		public FormattedVariantValue(VariantValue value, int numberFormatId) {
			this.value = value;
			this.numberFormatId = numberFormatId;
		}
		public bool IsEmpty { get { return value.IsEmpty; } }
		public int NumberFormatId { get { return numberFormatId; } }
		public VariantValue Value { get { return value; } set { this.value = value; } }
	}
	public interface ISupportsExistingValuesEnumeration {
		IEnumerator<VariantValue> GetExistingValuesEnumerator();
	}
	public interface IVariantArray :  ICloneable<IVariantArray> {
		long Count { get; }
		int Width { get; }
		int Height { get; }
		VariantValue this[int index] { get; }
		bool IsHorizontal { get; }
		bool IsVertical { get; }
		VariantValue GetValue(int y, int x);
	}
	#region VariantArrayValuesEnumerator
	public class VariantArrayValuesEnumerator : IEnumerator<VariantValue> {
		readonly VariantArray array;
		long index = -1;
		long count;
		public VariantArrayValuesEnumerator(VariantArray array) {
			Guard.ArgumentNotNull(array, "array");
			this.array = array;
			this.count = array.Count;
		}
		VariantValue GetCurrent() {
			return array[(int)index];
		}
		#region IEnumerator<VariantValue> Members
		public VariantValue Current { get { return GetCurrent(); } }
		#endregion
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
		#region IEnumerator Members
		object IEnumerator.Current { get { return GetCurrent(); } }
		public bool MoveNext() {
			index++;
			return index < count;
		}
		public void Reset() {
			index = -1;
		}
		#endregion
	}
	#endregion
	#region VariantArray
	public class VariantArray : IVariantArray {
		int width;
		int height;
		IList<VariantValue> values;
		public IList<VariantValue> Values { get { return values; } set { values = value; } }
		public long Count { get { return values.Count; } }
		public int Width { get { return width; } set { width = value; } }
		public int Height { get { return height; } set { height = value; } }
		public VariantValue this[int index] { get { return values[index]; } set { values[index] = value; } }
		public bool IsHorizontal { get { return this.height == 1 && this.width != 1; } }
		public bool IsVertical { get { return this.width == 1 && this.height != 1; } }
		public static VariantArray Create(int width, int height) {
			VariantArray result = new VariantArray();
			result.Width = width;
			result.Height = height;
			result.Values = new List<VariantValue>(new VariantValue[width * height]);
			return result;
		}
		public VariantValue GetValue(int y, int x) {
			if (y < 0)
				return VariantValue.ErrorValueNotAvailable;
			if (y >= Height) {
				if (Height != 1)
					return VariantValue.ErrorValueNotAvailable;
				y = 0;
			}
			if (x < 0)
				return VariantValue.ErrorValueNotAvailable;
			if (x >= Width) {
				if (Width != 1)
					return VariantValue.ErrorValueNotAvailable;
				x = 0;
			}
			return values[y * width + x];
		}
		public void SetValue(int y, int x, VariantValue value) {
			values[y * width + x] = value;
		}
		public void SetValues(VariantValue[,] valuesArray) {
			Width = valuesArray.GetLength(1);
			Height = valuesArray.GetLength(0);
			values = new VariantValue[width * height];
			for (int j = 0; j < height; j++)
				for (int i = 0; i < width; i++)
					SetValue(j, i, valuesArray[j, i]);
		}
		public void SetValues(IList<VariantValue> valuesArray, int width, int height) {
			Width = width;
			Height = height;
			values = valuesArray;
		}
		public static VariantValue GetValueConsiderRelativeOffset(IVariantArray array, CellPositionOffset cellOffset) {
			int offset1 = 0;
			if (array.IsHorizontal) {
				if (cellOffset.ColumnOffset >= array.Width)
					return VariantValue.ErrorValueNotAvailable;
				offset1 = cellOffset.ColumnOffset;
			}
			if (array.IsVertical) {
				if (cellOffset.RowOffset >= array.Height)
					return VariantValue.ErrorValueNotAvailable;
				offset1 = cellOffset.RowOffset;
			}
			if (!array.IsVertical && !array.IsHorizontal) {
				if (cellOffset.ColumnOffset >= array.Width || cellOffset.RowOffset >= array.Height)
					return VariantValue.ErrorValueNotAvailable;
				offset1 = array.Width * cellOffset.RowOffset + cellOffset.ColumnOffset;
			}
			return array[offset1];
		}
		IVariantArray ICloneable<IVariantArray>.Clone() {
			return this.Clone();
		}
		public VariantArray Clone() {
			VariantArray clone = new VariantArray();
			clone.SetValues(values, width, height);
			return clone;
		}
		public override bool Equals(object obj) {
			if (obj == null)
				return false;
			VariantArray anotherArray = obj as VariantArray;
			if (anotherArray == null)
				return false;
			if (this.width != anotherArray.width || this.height != anotherArray.height)
				return false;
			for (int i = 0; i < Count; i++)
				if (!values[i].Equals(anotherArray.values[i]))
					return false;
			return true;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	#endregion
	#region RangeVariantArray
	public class RangeVariantArray : IVariantArray, IEquatable<RangeVariantArray> {
		readonly CellRange range;
		public RangeVariantArray(CellRange range) {
			Guard.ArgumentNotNull(range, "range");
			this.range = range;
		}
		#region IVariantArray Members
		public long Count { get { return range.CellCount; } }
		public int Width { get { return range.Width; } }
		public int Height { get { return range.Height; } }
		public VariantValue this[int index] {
			get {
				int y = index / Width;
				int x = index % Width;
				return GetValue(y, x);
			}
		}
		public bool IsHorizontal { get { return this.Height == 1 && this.Width != 1; } }
		public bool IsVertical { get { return this.Width == 1 && this.Height != 1; } }
		public VariantValue GetValue(int y, int x) {
			return range.GetCellValueRelative(x, y);
		}
		#endregion
		IVariantArray ICloneable<IVariantArray>.Clone() {
			return new RangeVariantArray((CellRange)range.Clone());
		}
		public override bool Equals(object obj) {
			RangeVariantArray other = obj as RangeVariantArray;
			if (other == null)
				return false;
			return Equals(other);
		}
		public bool Equals(RangeVariantArray other) {
			return object.ReferenceEquals(range.Worksheet, other.range.Worksheet) && range.EqualsPosition(other.range);
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(range.Worksheet.GetHashCode(), range.TopLeft.GetHashCode(), range.BottomRight.GetHashCode());
		}
	}
	#endregion
	#region TransposedVariantArray
	public class TransposedVariantArray : IVariantArray, IEquatable<TransposedVariantArray> {
		readonly IVariantArray array;
		public TransposedVariantArray(IVariantArray array) {
			Guard.ArgumentNotNull(array, "array");
			this.array = array;
		}
		#region IVariantArray Members
		public long Count { get { return array.Count; } }
		public int Width { get { return array.Height; } }
		public int Height { get { return array.Width; } }
		public VariantValue this[int index] { get { return array[index]; } }
		public bool IsHorizontal { get { return Height == 1 && Width != 1; } }
		public bool IsVertical { get { return Width == 1 && Height != 1; } }
		public VariantValue GetValue(int y, int x) {
			return array.GetValue(x, y);
		}
		#endregion
		IVariantArray ICloneable<IVariantArray>.Clone() {
			return new TransposedVariantArray(array.Clone());
		}
		public override bool Equals(object obj) {
			TransposedVariantArray other = obj as TransposedVariantArray;
			if (other == null)
				return false;
			return Equals(other);
		}
		public bool Equals(TransposedVariantArray other) {
			return array.Equals(other.array);
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(array.GetHashCode(), GetType().GetHashCode());
		}
	}
	#endregion
	public interface IVariantArrayItemCalculator {
		VariantValue Calculate(VariantValue value);
	}
	#region CalculatedVariantArray
	public class CalculatedVariantArray : IVariantArray {
		readonly IVariantArray array;
		readonly IVariantArrayItemCalculator itemCalculator;
		public CalculatedVariantArray(IVariantArray array, IVariantArrayItemCalculator itemCalculator) {
			Guard.ArgumentNotNull(array, "array");
			Guard.ArgumentNotNull(itemCalculator, "itemCalculator");
			this.array = array;
			this.itemCalculator = itemCalculator;
		}
		#region IVariantArray Members
		public long Count { get { return array.Count; } }
		public int Width { get { return array.Width; } }
		public int Height { get { return array.Height; } }
		public VariantValue this[int index] { get { return CalculateValue(array[index]); } }
		public bool IsHorizontal { get { return Height == 1 && Width != 1; } }
		public bool IsVertical { get { return Width == 1 && Height != 1; } }
		public VariantValue GetValue(int y, int x) {
			return CalculateValue(array.GetValue(y, x));
		}
		#endregion
		VariantValue CalculateValue(VariantValue value) {
			return itemCalculator.Calculate(value);
		}
		IVariantArray ICloneable<IVariantArray>.Clone() {
			return new CalculatedVariantArray(array.Clone(), itemCalculator);
		}
	}
	#endregion
	public interface ICombinedVariantArrayItemCalculator {
		VariantValue Calculate(VariantValue firstValue, VariantValue secondValue);
	}
	#region CombinedVariantArray
	public class CombinedVariantArray : IVariantArray {
		readonly IVariantArray firstArray;
		readonly IVariantArray secondArray;
		readonly ICombinedVariantArrayItemCalculator itemCalculator;
		public CombinedVariantArray(IVariantArray firstArray, IVariantArray secondArray, ICombinedVariantArrayItemCalculator itemCalculator) {
			Guard.ArgumentNotNull(firstArray, "firstArray");
			Guard.ArgumentNotNull(secondArray, "secondArray");
			Guard.ArgumentNotNull(itemCalculator, "itemCalculator");
			this.firstArray = firstArray;
			this.secondArray = secondArray;
			this.itemCalculator = itemCalculator;
		}
		#region IVariantArray Members
		public long Count { get { return Width * Height; } }
		public int Width { get { return Math.Max(firstArray.Width, secondArray.Width); } }
		public int Height { get { return Math.Max(firstArray.Height, secondArray.Height); } }
		public VariantValue this[int index] {
			get {
#if !SL && !DXPORTABLE
				int x;
				int y = Math.DivRem(index, Width, out x);
				return GetValue(y, x);
#else
				return GetValue(index / Width, index % Width);
#endif
			}
		}
		public bool IsHorizontal { get { return Height == 1 && Width != 1; } }
		public bool IsVertical { get { return Width == 1 && Height != 1; } }
		public VariantValue GetValue(int y, int x) {
			return CalculateValue(firstArray.GetValue(y, x), secondArray.GetValue(y, x));
		}
		#endregion
		VariantValue CalculateValue(VariantValue firstValue, VariantValue secondValue) {
			return itemCalculator.Calculate(firstValue, secondValue);
		}
		IVariantArray ICloneable<IVariantArray>.Clone() {
			return new CombinedVariantArray(firstArray.Clone(), secondArray.Clone(), itemCalculator);
		}
	}
	#endregion
	#region DefaultVariantValueComparer
	public class DefaultVariantValueComparer : IComparer<VariantValue> {
		readonly SharedStringTable stringTable;
		public DefaultVariantValueComparer(SharedStringTable stringTable) {
			Guard.ArgumentNotNull(stringTable, "stringTable");
			this.stringTable = stringTable;
		}
		#region IComparer<VariantValue> Members
		public virtual int Compare(VariantValue x, VariantValue y) {
			VariantValueType sortType = GetSortType(x);
			int result = Comparer<int>.Default.Compare(VariantValueType.Numeric - sortType, VariantValueType.Numeric - GetSortType(y));
			if (result != 0)
				return result;
			if (sortType == VariantValueType.Numeric || sortType == VariantValueType.Boolean)
				return DoubleComparer.Compare(x.NumericValue, y.NumericValue);
			if (sortType == VariantValueType.InlineText)
				return String.Compare(x.GetTextValue(stringTable), y.GetTextValue(stringTable), StringComparison.CurrentCultureIgnoreCase);
			if (sortType == VariantValueType.Error)
				return CompareErrors(x.ErrorValue, y.ErrorValue);
			return 0;
		}
		#endregion
		protected virtual VariantValueType GetSortType(VariantValue value) {
			return value.InversedSortType;
		}
		protected virtual int CompareErrors(ICellError x, ICellError y) {
			return String.Compare(x.Name, y.Name, StringComparison.CurrentCultureIgnoreCase);
		}
	}
	#endregion
	#region DefaultVariantValueComparable
	public class DefaultVariantValueComparable : IComparable<VariantValue> {
		readonly VariantValue value;
		readonly SharedStringTable stringTable;
		public DefaultVariantValueComparable(VariantValue value, SharedStringTable stringTable) {
			Guard.ArgumentNotNull(stringTable, "stringTable");
			this.value = value;
			this.stringTable = stringTable;
		}
		protected SharedStringTable StringTable { get { return stringTable; } }
		#region IComparable<VariantValue> Members
		public virtual int CompareTo(VariantValue other) {
			VariantValueType sortType = value.InversedSortType;
			int result = Comparer<int>.Default.Compare(VariantValueType.Numeric - sortType, VariantValueType.Numeric - other.InversedSortType);
			if (result != 0)
				return -result;
			if (sortType == VariantValueType.Numeric || sortType == VariantValueType.Boolean)
				return -DoubleComparer.Compare(value.NumericValue, other.NumericValue);
			if (sortType == VariantValueType.InlineText)
				return -String.Compare(value.GetTextValue(stringTable), other.GetTextValue(stringTable), StringComparison.CurrentCultureIgnoreCase);
			if (sortType == VariantValueType.Error)
				return -String.Compare(value.ErrorValue.Name, other.ErrorValue.Name, StringComparison.CurrentCultureIgnoreCase);
			return 0;
		}
		#endregion
	}
	#endregion
	#region InversedDefaultVariantValueComparable
	public class InversedDefaultVariantValueComparable : DefaultVariantValueComparable {
		public InversedDefaultVariantValueComparable(VariantValue value, SharedStringTable stringTable)
			: base(value, stringTable) {
		}
		#region IComparable<VariantValue> Members
		public override int CompareTo(VariantValue other) {
			if (other.IsEmpty || (other.IsText && string.IsNullOrEmpty(other.GetTextValue(StringTable))))
				other = double.MinValue;
			return -base.CompareTo(other);
		}
		#endregion
	}
	#endregion
	#region ErrorConverter
	public static class ErrorConverter {
		public static VariantValue ErrorCodeToValue(int code) {
			switch (code) {
				case 0x00:
					return VariantValue.ErrorNullIntersection;
				case 0x07:
					return VariantValue.ErrorDivisionByZero;
				case 0x0f:
					return VariantValue.ErrorInvalidValueInFunction;
				case 0x17:
					return VariantValue.ErrorReference;
				case 0x1d:
					return VariantValue.ErrorName;
				case 0x24:
					return VariantValue.ErrorNumber;
				case 0x2a:
					return VariantValue.ErrorValueNotAvailable;
			}
			return VariantValue.Empty;
		}
		public static int ValueToErrorCode(VariantValue value) {
			if (value.Type == VariantValueType.Error) {
				if (value == VariantValue.ErrorNullIntersection) return 0x00;
				if (value == VariantValue.ErrorDivisionByZero) return 0x07;
				if (value == VariantValue.ErrorInvalidValueInFunction) return 0x0f;
				if (value == VariantValue.ErrorReference) return 0x17;
				if (value == VariantValue.ErrorName) return 0x1d;
				if (value == VariantValue.ErrorNumber) return 0x24;
				if (value == VariantValue.ErrorValueNotAvailable) return 0x2a;
			}
			return -1;
		}
	}
	#endregion
}
