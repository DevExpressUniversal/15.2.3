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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.API.Native.Implementation;
using ModelCellRange = DevExpress.XtraSpreadsheet.Model.CellRange;
using ModelVariantValue = DevExpress.XtraSpreadsheet.Model.VariantValue;
using ModelWorkbookDataContext = DevExpress.XtraSpreadsheet.Model.WorkbookDataContext;
using ModelVariantValueType = DevExpress.XtraSpreadsheet.Model.VariantValueType;
using ModelCellErrorType = DevExpress.XtraSpreadsheet.Model.ModelCellErrorType;
using ModelIVariantArray = DevExpress.XtraSpreadsheet.Model.IVariantArray;
using ModelVariantArray = DevExpress.XtraSpreadsheet.Model.VariantArray;
using DateSystem = DevExpress.XtraSpreadsheet.Model.DateSystem;
using Model = DevExpress.XtraSpreadsheet.Model;
using System.Diagnostics;
using System.ComponentModel;
using System.Globalization;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Spreadsheet {
	#region CellValueType
	public enum CellValueType {
		None = ModelVariantValueType.None,
		Error = ModelVariantValueType.Error,
		Boolean = ModelVariantValueType.Boolean,
		Text = ModelVariantValueType.InlineText,
		Numeric = ModelVariantValueType.Numeric,
		DateTime = ModelVariantValueType.Missing + 1,
		Unknown = -1
	};
	#endregion
	#region ErrorType
	public enum ErrorType {
		None = 0,
		Value = ModelCellErrorType.Value,
		DivisionByZero = ModelCellErrorType.DivisionByZero,
		Number = ModelCellErrorType.Number,
		Name = ModelCellErrorType.Name,
		Reference = ModelCellErrorType.Reference,
		NotAvailable = ModelCellErrorType.NotAvailable,
		Null = ModelCellErrorType.Null,
	}
	#endregion
	public interface ErrorValueInfo {
		ErrorType Type { get; }
		string Name { get; }
		string Description { get; }
	}
	#region CellValue
	public class CellValue {
		ModelVariantValue modelValue;
		ModelWorkbookDataContext modelContext;
		bool isDateTime;
		static readonly CellValue empty = new CellValue(ModelVariantValue.Empty, false);
		static readonly ICellValueConverter defaultConverter = new DefaultCellValueConverter();
		static readonly DateTime date1900Zero = new DateTime(1899, 12, 31);
		#region Errors
		public static CellValue ErrorInvalidValueInFunction = new CellValue(ModelVariantValue.ErrorInvalidValueInFunction, false);
		public static CellValue ErrorDivisionByZero = new CellValue(ModelVariantValue.ErrorDivisionByZero, false);
		public static CellValue ErrorNumber = new CellValue(ModelVariantValue.ErrorNumber, false);
		public static CellValue ErrorReference = new CellValue(ModelVariantValue.ErrorReference, false);
		public static CellValue ErrorValueNotAvailable = new CellValue(ModelVariantValue.ErrorValueNotAvailable, false);
		public static CellValue ErrorNullIntersection = new CellValue(ModelVariantValue.ErrorNullIntersection, false);
		public static CellValue ErrorName = new CellValue(ModelVariantValue.ErrorName, false);
		protected internal static Dictionary<ModelCellErrorType, CellValue> modelErrorTypeToApiErrorTable = CreateErrorTable();
		static Dictionary<ModelCellErrorType, CellValue> CreateErrorTable() {
			Dictionary<ModelCellErrorType, CellValue> result = new Dictionary<ModelCellErrorType, CellValue>();
			result.Add(ModelCellErrorType.Value, ErrorInvalidValueInFunction);
			result.Add(ModelCellErrorType.DivisionByZero, ErrorDivisionByZero);
			result.Add(ModelCellErrorType.Number, ErrorNumber);
			result.Add(ModelCellErrorType.Reference, ErrorReference);
			result.Add(ModelCellErrorType.NotAvailable, ErrorValueNotAvailable);
			result.Add(ModelCellErrorType.Null, ErrorNullIntersection);
			result.Add(ModelCellErrorType.Name, ErrorName);
			return result;
		}
		#endregion
		internal CellValue(Cell owner) {
			NativeCell cell = owner as NativeCell;
			if (cell != null) {
				DevExpress.XtraSpreadsheet.Model.ICell modelCell = cell.ReadOnlyModelCell;
				this.ModelVariantValue = modelCell.Value;
				this.isDateTime = cell.IsDisplayedAsDateTime;
				if (ModelVariantValue.IsSharedString)
					ModelVariantValue = ModelVariantValue.GetTextValue(modelCell.Sheet.Workbook.SharedStringTable);
				modelContext = modelCell.Sheet.Workbook.DataContext;
			}
		}
		internal CellValue(DevExpress.XtraSpreadsheet.Model.VariantValue value, ModelWorkbookDataContext modelContext) { 
			this.modelValue = value;
			this.modelContext = modelContext;
		}
		internal CellValue(DevExpress.XtraSpreadsheet.Model.VariantValue value, bool isDateTime) {
			ChangeModelValue(value, isDateTime);
		}
		[Browsable(false)]
		internal void ChangeModelValue(DevExpress.XtraSpreadsheet.Model.VariantValue value, bool isDateTime) {
			this.modelValue = value;
			this.isDateTime = isDateTime && value.IsNumeric;
		}
		#region Properties
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueDefaultConverter")]
#endif
		public static ICellValueConverter DefaultConverter { get { return defaultConverter; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueEmpty")]
#endif
		public static CellValue Empty { get { return CellValue.empty; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueDateTimeValue")]
#endif
		public DateTime DateTimeValue {
			get {
				try {
					if (!IsNumeric)
						return DateTime.MinValue;
					DateSystem dateSystem = modelContext == null ? DateSystem.Date1900 : modelContext.DateSystem;
					if (ModelWorkbookDataContext.IsErrorDateTimeSerial(ModelVariantValue.NumericValue, dateSystem))
						return DateTime.MinValue;
					return ModelVariantValue.ToDateTime(dateSystem);
				}
				catch (ArgumentOutOfRangeException) {
					return DateTime.MinValue;
				}
				catch (OverflowException) {
					return DateTime.MaxValue;
				}
			}
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueBooleanValue")]
#endif
		public bool BooleanValue { get { return ModelVariantValue.BooleanValue; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueNumericValue")]
#endif
		public double NumericValue { get { return ModelVariantValue.NumericValue; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueTextValue")]
#endif
		public string TextValue {
			get {
				if (modelContext != null && IsSharedString)
					return ModelVariantValue.GetTextValue(modelContext.Workbook.SharedStringTable);
				return ModelVariantValue.InlineTextValue;
			}
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueErrorValue")]
#endif
		public ErrorValueInfo ErrorValue {
			get {
				if (!IsError)
					return null;
				return new NativeErrorInfo(modelValue.ErrorValue);
			}
		}
		internal ModelVariantValue ModelVariantValue { get { return modelValue; } set { modelValue = value; } }
		internal ModelWorkbookDataContext ModelDataContext { get { return modelContext; } set { modelContext = value; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueIsEmpty")]
#endif
		public bool IsEmpty { [DebuggerStepThrough] get { return ModelVariantValue.IsEmpty; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueIsNumeric")]
#endif
		public bool IsNumeric { [DebuggerStepThrough] get { return ModelVariantValue.IsNumeric; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueIsBoolean")]
#endif
		public bool IsBoolean { [DebuggerStepThrough] get { return ModelVariantValue.IsBoolean; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueIsError")]
#endif
		public bool IsError { [DebuggerStepThrough] get { return ModelVariantValue.IsError; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueIsText")]
#endif
		public bool IsText { [DebuggerStepThrough] get { return IsInlineText || IsSharedString; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueIsDateTime")]
#endif
		public bool IsDateTime { [DebuggerStepThrough] get { return isDateTime; } internal set { isDateTime = value; } }
		bool IsInlineText { [DebuggerStepThrough] get { return ModelVariantValue.IsInlineText; } }
		bool IsSharedString { [DebuggerStepThrough] get { return ModelVariantValue.IsSharedString; } }
		#region Type
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueType")]
#endif
		public CellValueType Type {
			get {
				if (isDateTime)
					return CellValueType.DateTime;
				switch (ModelVariantValue.Type) {
					case DevExpress.XtraSpreadsheet.Model.VariantValueType.None:
						return CellValueType.None;
					case DevExpress.XtraSpreadsheet.Model.VariantValueType.Numeric:
						return CellValueType.Numeric;
					case DevExpress.XtraSpreadsheet.Model.VariantValueType.Error:
						return CellValueType.Error;
					case DevExpress.XtraSpreadsheet.Model.VariantValueType.Boolean:
						return CellValueType.Boolean;
					case DevExpress.XtraSpreadsheet.Model.VariantValueType.InlineText:
						return CellValueType.Text;
					case DevExpress.XtraSpreadsheet.Model.VariantValueType.SharedString:
						return CellValueType.Text;
					case DevExpress.XtraSpreadsheet.Model.VariantValueType.CellRange:
						return CellValueType.Unknown;
					case DevExpress.XtraSpreadsheet.Model.VariantValueType.Array:
						return CellValueType.Unknown;
					default:
						return CellValueType.Unknown;
				}
			}
		}
		#endregion
		#endregion
		internal CellValue Clone() {
			return new CellValue(ModelVariantValue, IsDateTime);
		}
		#region Equality
		public override int GetHashCode() {
			return modelValue.GetHashCode();
		}
		public override bool Equals(object obj) {
			CellValue nativeValue = obj as CellValue;
			if (nativeValue != null)
				return AreEqual(this, nativeValue);
			nativeValue = TryCreateFromObject(obj);
			if (nativeValue != null)
				return AreEqual(this, nativeValue);
			return false;
		}
		public static bool operator ==(CellValue value, CellValue other) {
			return AreEqual(value, other);
		}
		public static bool operator !=(CellValue value, CellValue other) {
			return !AreEqual(value, other);
		}
		#endregion
		#region implicit conversion to CellValue
		public static implicit operator CellValue(int value) {
			return new CellValue(value, false);
		}
		[DebuggerStepThrough]
		public static implicit operator CellValue(double value) {
			if (double.IsInfinity(value) || double.IsNaN(value))
				return new CellValue(ushort.MaxValue, false);
			if (DevExpress.XtraExport.Xls.XNumChecker.IsNegativeZero(value))
				value = 0;
			return new CellValue(value, false);
		}
		[DebuggerStepThrough]
		public static implicit operator CellValue(float value) {
			if (float.IsInfinity(value) || float.IsNaN(value))
				return new CellValue(ushort.MaxValue, false);
			if (DevExpress.XtraExport.Xls.XNumChecker.IsNegativeZero(value))
				value = 0;
			return new CellValue(ConvertFloat(value), false);
		}
		[DebuggerStepThrough]
		public static implicit operator CellValue(char value) {
			return new CellValue(value, false);
		}
		[DebuggerStepThrough]
		public static implicit operator CellValue(string value) {
			return new CellValue(value, false);
		}
		[DebuggerStepThrough]
		public static implicit operator CellValue(DateTime value) {
			return FromDateTime(value, false);
		}
		[DebuggerStepThrough]
		public static implicit operator CellValue(bool value) {
			return new CellValue(value, false);
		}
		[DebuggerStepThrough]
		public static implicit operator CellValue(ErrorType value) {
			if (value == ErrorType.None)
				return CellValue.Empty;
			else
				return GetNativeErrorValue((ModelCellErrorType)value);
		}
		[DebuggerStepThrough]
		public static implicit operator CellValue(TimeSpan value) {
			return FromDateTime(date1900Zero.Add(value), false);
		}
		#endregion
		[DebuggerStepThrough]
		public static CellValue FromDateTime(DateTime value, bool use1904DateSystem) {
			return new CellValue(CreateModelDateTime(value, use1904DateSystem), true);
		}
		internal static CellValue GetNativeErrorValue(ModelCellErrorType errorType) {
			return modelErrorTypeToApiErrorTable[errorType];
		}
		static double ConvertFloat(float value) {
			if (value >= (float)decimal.MaxValue)
				return (double)value;
			if (value <= (float)decimal.MinValue)
				return (double)value;
			return (double)((decimal)value);
		}
		protected static bool AreEqual(CellValue value, CellValue other) {
			if (Object.ReferenceEquals(value, other))
				return true;
			if (Object.ReferenceEquals(value, null) || Object.ReferenceEquals(other, null))
				return false;
			return value.ModelVariantValue.Equals(other.ModelVariantValue);
		}
		protected static ModelVariantValue CreateModelDateTime(DateTime value, bool use1904DateSystem) {
			ModelVariantValue result = new ModelVariantValue();
			if (!use1904DateSystem && (value == date1900Zero))
				value = DateTime.MinValue;
			result.SetDateTime(value, use1904DateSystem ? DevExpress.XtraSpreadsheet.Model.DateSystem.Date1904 : DevExpress.XtraSpreadsheet.Model.DateSystem.Date1900);
			return result;
		}
		public override string ToString() {
			if (modelContext != null)
				return ToString(modelContext.Culture);
			else
				return ToString(CultureInfo.CurrentCulture);
		}
		public string ToString(IFormatProvider formatProvider) {
			switch (this.Type) {
				default:
					return String.Empty;
				case CellValueType.Unknown:
					return String.Empty;
				case CellValueType.None:
					return String.Empty;
				case CellValueType.Boolean:
					return BooleanValue.ToString(formatProvider);
				case CellValueType.DateTime:
					return DateTimeValue.ToString(formatProvider);
				case CellValueType.Numeric:
					return NumericValue.ToString(formatProvider);
				case CellValueType.Error:
					return ErrorValue.Name;
				case CellValueType.Text:
					return TextValue;
			}
		}
		public object ToObject() {
			return ToObject(DefaultConverter);
		}
		public object ToObject(ICellValueConverter converter) {
			return converter.ConvertToObject(this);
		}
		public static CellValue FromObject(object value) {
			return FromObject(value, DefaultConverter);
		}
		public static CellValue FromObject(object value, ICellValueConverter converter) {
			CellValue result = TryCreateFromObject(value, converter);
			if (result == null)
				throw new InvalidCastException("Can not convert '" + value.ToString() + "' value to CellValue using " + converter.GetType().Name + " converter."); 
			else
				return result;
		}
		public static CellValue TryCreateFromObject(object value) {
			return TryCreateFromObject(value, DefaultConverter);
		}
		public static CellValue TryCreateFromObject(object value, ICellValueConverter converter) {
			return converter.TryConvertFromObject(value);
		}
		internal static CellValue CreateForVirtualCell(object objectValue, int columnIndex, IDataValueConverter converter) {
			CellValue result;
			if (converter != null && converter.TryConvert(objectValue, columnIndex, out result)) {
				if (result == null)
					return CellValue.Empty;
			}
			else
				result = FromObject(objectValue);
			if (result.IsInlineText && String.IsNullOrEmpty(result.TextValue))
				return CellValue.Empty;
			return result;
		}
	}
	#endregion
	public interface ICellValueConverter {
		CellValue TryConvertFromObject(object value);
		object ConvertToObject(CellValue value);
	}
}
namespace DevExpress.Spreadsheet.Functions {
	public class ParameterValue {
		static readonly ParameterValue empty = new ParameterValue(ModelVariantValue.Empty);
		#region Errors
		public static ParameterValue ErrorInvalidValueInFunction = new ParameterValue(ModelVariantValue.ErrorInvalidValueInFunction);
		public static ParameterValue ErrorDivisionByZero = new ParameterValue(ModelVariantValue.ErrorDivisionByZero);
		public static ParameterValue ErrorNumber = new ParameterValue(ModelVariantValue.ErrorNumber);
		public static ParameterValue ErrorReference = new ParameterValue(ModelVariantValue.ErrorReference);
		public static ParameterValue ErrorValueNotAvailable = new ParameterValue(ModelVariantValue.ErrorValueNotAvailable);
		public static ParameterValue ErrorNullIntersection = new ParameterValue(ModelVariantValue.ErrorNullIntersection);
		public static ParameterValue ErrorName = new ParameterValue(ModelVariantValue.ErrorName);
		protected internal static Dictionary<ModelCellErrorType, ParameterValue> modelErrorTypeToApiErrorTable = CreateErrorTable();
		static Dictionary<ModelCellErrorType, ParameterValue> CreateErrorTable() {
			Dictionary<ModelCellErrorType, ParameterValue> result = new Dictionary<ModelCellErrorType, ParameterValue>();
			result.Add(ModelCellErrorType.Value, ErrorInvalidValueInFunction);
			result.Add(ModelCellErrorType.DivisionByZero, ErrorDivisionByZero);
			result.Add(ModelCellErrorType.Number, ErrorNumber);
			result.Add(ModelCellErrorType.Reference, ErrorReference);
			result.Add(ModelCellErrorType.NotAvailable, ErrorValueNotAvailable);
			result.Add(ModelCellErrorType.Null, ErrorNullIntersection);
			result.Add(ModelCellErrorType.Name, ErrorName);
			return result;
		}
		#endregion
		#region Fields
		ModelVariantValue modelValue;
		IWorkbook workbook;
		#endregion
		public ParameterValue() {
			modelValue = ModelVariantValue.Empty;
		}
		internal ParameterValue(ModelVariantValue modelValue, ModelWorkbookDataContext context) {
			Debug.Assert(!modelValue.IsCellRange);
			if (modelValue.IsSharedString)
				this.modelValue = modelValue.GetTextValue(context.Workbook.SharedStringTable);
			else
				this.modelValue = modelValue;
		}
		internal ParameterValue(ModelVariantValue modelValue, IWorkbook workbook) {
			Debug.Assert(!modelValue.IsSharedString);
			this.modelValue = modelValue;
			this.workbook = workbook;
		}
		internal ParameterValue(ModelVariantValue modelValue) {
			Debug.Assert(!modelValue.IsSharedString && !modelValue.IsCellRange);
			this.modelValue = modelValue;
		}
		#region Properties
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ParameterValueEmpty")]
#endif
		public static ParameterValue Empty { get { return ParameterValue.empty; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ParameterValueErrorValue")]
#endif
		public ErrorValueInfo ErrorValue { get { return !IsError ? null : new NativeErrorInfo(modelValue.ErrorValue); } }
		protected internal ModelVariantValue ModelVariantValue { get { return modelValue; } protected set { modelValue = value; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ParameterValueIsEmpty")]
#endif
		public bool IsEmpty { [DebuggerStepThrough] get { return ModelVariantValue.IsEmpty; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ParameterValueIsNumeric")]
#endif
		public bool IsNumeric { [DebuggerStepThrough] get { return ModelVariantValue.IsNumeric; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ParameterValueIsBoolean")]
#endif
		public bool IsBoolean { [DebuggerStepThrough] get { return ModelVariantValue.IsBoolean; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ParameterValueIsError")]
#endif
		public bool IsError { [DebuggerStepThrough] get { return ModelVariantValue.IsError; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ParameterValueIsText")]
#endif
		public bool IsText { [DebuggerStepThrough] get { return IsInlineText || IsSharedString; } }
		bool IsInlineText { [DebuggerStepThrough] get { return ModelVariantValue.IsInlineText; } }
		bool IsSharedString { [DebuggerStepThrough] get { return ModelVariantValue.IsSharedString; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ParameterValueIsArray")]
#endif
		public bool IsArray { [DebuggerStepThrough] get { return ModelVariantValue.IsArray; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ParameterValueIsRange")]
#endif
		public bool IsRange { [DebuggerStepThrough] get { return ModelVariantValue.IsCellRange; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ParameterValueBooleanValue")]
#endif
		public bool BooleanValue { get { return ModelVariantValue.BooleanValue; } set { ModelVariantValue = value; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ParameterValueNumericValue")]
#endif
		public double NumericValue { get { return ModelVariantValue.NumericValue; } set { ModelVariantValue = value; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ParameterValueTextValue")]
#endif
		public string TextValue { get { return ModelVariantValue.InlineTextValue; } set { ModelVariantValue = value; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ParameterValueRangeValue")]
#endif
		public Range RangeValue { get { return GetRangeValue(); } set { SetRangeValue(value); } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ParameterValueRangeAreas")]
#endif
		public List<Range> RangeAreas { get { return GetRangeAreas(); } set { SetRangeValue(value); } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ParameterValueArrayValue")]
#endif
		public CellValue[,] ArrayValue { get { return ToArrayValue(); } }
		#endregion
		#region Equality
		public override int GetHashCode() {
			return modelValue.GetHashCode();
		}
		public override bool Equals(object obj) {
			ParameterValue nativeValue = obj as ParameterValue;
			if (nativeValue == null)
				return false;
			return AreEqual(this, nativeValue);
		}
		public static bool operator ==(ParameterValue value, ParameterValue other) {
			return AreEqual(value, other);
		}
		public static bool operator !=(ParameterValue value, ParameterValue other) {
			return !AreEqual(value, other);
		}
		#endregion
		#region implicit conversion to CellValue
		public static implicit operator ParameterValue(int value) {
			return new ParameterValue(value);
		}
		[DebuggerStepThrough]
		public static implicit operator ParameterValue(double value) {
			return new ParameterValue(value);
		}
		[DebuggerStepThrough]
		public static implicit operator ParameterValue(char value) {
			return new ParameterValue(value);
		}
		[DebuggerStepThrough]
		public static implicit operator ParameterValue(string value) {
			return new ParameterValue(value);
		}
		[DebuggerStepThrough]
		public static implicit operator ParameterValue(DateTime value) {
			return FromDateTime(value, false);
		}
		[DebuggerStepThrough]
		public static implicit operator ParameterValue(bool value) {
			return new ParameterValue(value);
		}
		[DebuggerStepThrough]
		public static implicit operator ParameterValue(CellValue value) {
			if (value != null)
				return new ParameterValue(value.ModelVariantValue);
			return new ParameterValue();
		}
		public static implicit operator ParameterValue(CellValue[,] values) {
			int width = values.GetLength(1);
			int height = values.GetLength(0);
			ModelVariantValue[] modelValues = new ModelVariantValue[height * width];
			for (int j = 0; j < height; j++)
				for (int i = 0; i < width; i++) {
					CellValue value = values[j, i];
					if (value == null)
						modelValues[j * width + i] = 0;
					else
						modelValues[j * width + i] = value.ModelVariantValue;
				}
			ModelVariantArray variantArray = new ModelVariantArray();
			variantArray.SetValues(modelValues, width, height);
			return new ParameterValue(ModelVariantValue.FromArray(variantArray));
		}
		static Model.CellRangeBase GetModelCellRange(Range range) {
			return ((NativeWorksheet)(range.Worksheet)).GetModelRange(range);
		}
		[DebuggerStepThrough]
		public static ParameterValue FromRange(Range value) {
			ParameterValue result = new ParameterValue();
			result.SetRangeValue(value);
			return result;
		}
		[DebuggerStepThrough]
		public static ParameterValue FromRange(List<Range> rangeAreas) {
			ParameterValue result = new ParameterValue();
			result.SetRangeValue(rangeAreas);
			return result;
		}
		#endregion
		NativeRange GetRangeValue() {
			if (workbook == null)
				return null;
			Model.CellRangeBase modelCellRangeBase = ModelVariantValue.CellRangeValue;
			Model.CellRange modelCellRange = modelCellRangeBase.GetFirstInnerCellRange();
			string sheetName = string.Empty;
			if (modelCellRange.Worksheet == null) {
				if (modelCellRangeBase.Worksheet == null)
					return null;
				sheetName = modelCellRangeBase.Worksheet.Name;
			}
			else
				sheetName = modelCellRange.Worksheet.Name;
			NativeWorksheet sheet = workbook.Worksheets[sheetName] as NativeWorksheet;
			if (sheet == null)
				return null;
			return new NativeRange(modelCellRange, sheet);
		}
		void SetRangeValue(Range value) {
			if (value == null)
				throw new ArgumentNullException();
			ModelVariantValue = ModelVariantValue.Create(GetModelCellRange(value));
			workbook = value.Worksheet.Workbook;
		}
		void SetRangeValue(List<Range> value) {
			if (value == null || value.Count <= 0)
				ModelVariantValue = Model.VariantValue.Empty;
			List<Model.CellRangeBase> modelRanges = new List<Model.CellRangeBase>();
			foreach (Range range in value)
				if (range != null) {
					modelRanges.Add(GetModelCellRange(range));
					workbook = range.Worksheet.Workbook;
				}
			if (modelRanges.Count <= 0)
				ModelVariantValue = Model.VariantValue.Empty;
			else {
				if (modelRanges.Count == 1)
					ModelVariantValue = ModelVariantValue.Create(modelRanges[0]);
				else
					ModelVariantValue = ModelVariantValue.Create(new Model.CellUnion(modelRanges));
			}
		}
		List<Range> GetRangeAreas() {
			List<Range> result = new List<Range>();
			if (!ModelVariantValue.IsCellRange || workbook == null)
				return result;
			Model.CellRangeBase modelCellRangeBase = ModelVariantValue.CellRangeValue;
			List<ModelCellRange> innerRanges = new List<ModelCellRange>();
			modelCellRangeBase.AddRanges(innerRanges);
			Model.ICellTable modelSheet = modelCellRangeBase.Worksheet;
			foreach (Model.CellRange innerRange in innerRanges) {
				Model.ICellTable rangeModelSheet = innerRange.Worksheet;
				if (rangeModelSheet == null) {
					if (modelSheet == null)
						continue;
					rangeModelSheet = modelSheet;
				}
				NativeWorksheet sheet = workbook.Worksheets[rangeModelSheet.Name] as NativeWorksheet;
				if (sheet != null)
					result.Add(new NativeRange(innerRange, sheet));
			}
			return result;
		}
		[DebuggerStepThrough]
		public static ParameterValue FromDateTime(DateTime value, bool use1904DateSystem) {
			return new ParameterValue(CreateModelDateTime(value, use1904DateSystem));
		}
		CellValue[,] ToArrayValue() {
			if (!ModelVariantValue.IsArray)
				return null;
			ModelIVariantArray modelArray = ModelVariantValue.ArrayValue;
			if (modelArray == null)
				return null;
			int width = modelArray.Width;
			int height = modelArray.Height;
			CellValue[,] result = new CellValue[height, width];
			for (int j = 0; j < height; j++)
				for (int i = 0; i < width; i++)
					result[j, i] = new CellValue(modelArray.GetValue(j, i), false);
			return result;
		}
		static bool AreEqual(ParameterValue value, ParameterValue other) {
			if (Object.ReferenceEquals(value, other))
				return true;
			if (Object.ReferenceEquals(value, null) || Object.ReferenceEquals(other, null))
				return false;
			return value.ModelVariantValue.Equals(other.ModelVariantValue);
		}
		static ModelVariantValue CreateModelDateTime(DateTime value, bool use1904DateSystem) {
			ModelVariantValue result = new ModelVariantValue();
			result.SetDateTime(value, use1904DateSystem ? DevExpress.XtraSpreadsheet.Model.DateSystem.Date1904 : DevExpress.XtraSpreadsheet.Model.DateSystem.Date1900);
			return result;
		}
		#region ToString
		public override string ToString() {
			IFormatProvider formatProvider = workbook != null ? workbook.Options.Culture : CultureInfo.CurrentCulture;
			return ToString(formatProvider);
		}
		public string ToString(IFormatProvider formatProvider) {
			switch (modelValue.Type) {
				default:
					return String.Empty;
				case ModelVariantValueType.Missing:
				case ModelVariantValueType.None:
					return String.Empty;
				case ModelVariantValueType.Boolean:
					return BooleanValue.ToString(formatProvider);
				case ModelVariantValueType.Numeric:
					return NumericValue.ToString(formatProvider);
				case ModelVariantValueType.Error:
					return ErrorValue.Name;
				case ModelVariantValueType.InlineText:
					return TextValue;
				case ModelVariantValueType.CellRange:
					return RangeToString(ModelVariantValue.CellRangeValue, formatProvider);
				case ModelVariantValueType.Array:
					return string.Format("<array>[{0}, {1}]", modelValue.ArrayValue.Height, modelValue.ArrayValue.Width);
			}
		}
		string RangeToString(Model.CellRangeBase cellRangeBase, IFormatProvider formatProvider) {
			if (cellRangeBase.RangeType == Model.CellRangeType.UnionRange) {
				string listSeparator = TryGetListSeparatorFromFormatProvider(formatProvider);
				return ((Model.CellUnion)cellRangeBase).ToString(listSeparator[0], false);
			}
			return cellRangeBase.ToString();
		}
		string TryGetListSeparatorFromFormatProvider(IFormatProvider formatProvider) {
			CultureInfo cultureInfo = formatProvider as CultureInfo;
			if (cultureInfo != null)
				return cultureInfo.TextInfo.ListSeparator;
			if (formatProvider != null) {
				TextInfo info = formatProvider.GetFormat(typeof(TextInfo)) as TextInfo;
				if (info != null)
					return info.ListSeparator;
			}
			return ",";
		}
		#endregion
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Utils;
	using System.Diagnostics;
	using DevExpress.Spreadsheet;
	using DevExpress.XtraSpreadsheet.Utils;
	using DevExpress.XtraSpreadsheet.Localization;
	using DevExpress.XtraSpreadsheet.API.Internal;
	using ModelCell = DevExpress.XtraSpreadsheet.Model.ICell;
	using ModelCellKey = DevExpress.XtraSpreadsheet.Model.CellKey;
	using ModelCellPosition = DevExpress.XtraSpreadsheet.Model.CellPosition;
	using ModelHyperlink = DevExpress.XtraSpreadsheet.Model.ModelHyperlink;
	using ModelMargins = DevExpress.XtraSpreadsheet.Model.Margins;
	using ModelWorkbook = DevExpress.XtraSpreadsheet.Model.DocumentModel;
	using ModelWorksheet = DevExpress.XtraSpreadsheet.Model.Worksheet;
	using ModelPrintSetup = DevExpress.XtraSpreadsheet.Model.PrintSetup;
	using ModelDefinedName = DevExpress.XtraSpreadsheet.Model.DefinedName;
	#region NativeErrorInfo
	partial class NativeErrorInfo : ErrorValueInfo {
		readonly Model.ICellError innerError;
		public NativeErrorInfo(Model.ICellError error) {
			this.innerError = error;
		}
		public ErrorType Type { get { return (ErrorType)innerError.Type; } }
		public string Name { get { return innerError.Name; } }
		public string Description { get { return innerError.Description; } }
		public override string ToString() {
			return Type.ToString();
		}
	}
	#endregion
	#region DefaultCellValueConverter
	public class DefaultCellValueConverter : ICellValueConverter {
		#region ICellValueConverter implementation
		public object ConvertToObject(CellValue value) {
			switch (value.Type) {
				default:
					return null;
				case CellValueType.Unknown:
					return null;
				case CellValueType.None:
					return null;
				case CellValueType.Boolean:
					return value.BooleanValue;
				case CellValueType.DateTime:
					return value.DateTimeValue;
				case CellValueType.Numeric:
					return value.NumericValue;
				case CellValueType.Error:
					return value.ErrorValue.Type;
				case CellValueType.Text:
					return value.TextValue;
			}
		}
		public CellValue TryConvertFromObject(object value) {
			if (value == null)
				return CellValue.Empty;
			if (DXConvert.IsDBNull(value))
				return CellValue.Empty;
			Type type = value.GetType();
			if (type == typeof(string))
				return (string)value;
			if (type == typeof(DateTime))
				return (DateTime)value;
			if (type == typeof(TimeSpan))
				return (TimeSpan)value;
			if (type == typeof(bool))
				return (bool)value;
			if (type == typeof(double))
				return Convert.ToDouble(value);
			if (type == typeof(int))
				return Convert.ToDouble(value);
			if (type == typeof(long))
				return Convert.ToDouble(value);
			if (type == typeof(decimal))
				return Convert.ToDouble(value);
			if (type == typeof(float))
				return Convert.ToSingle(value);
			if (type == typeof(ErrorType))
				return (ErrorType)value;
			if (type == typeof(short))
				return Convert.ToDouble(value);
			if (type == typeof(byte))
				return Convert.ToDouble(value);
			if (type == typeof(ushort))
				return Convert.ToDouble(value);
			if (type == typeof(uint))
				return Convert.ToDouble(value);
			return null;
		}
		#endregion
	}
	#endregion
}
