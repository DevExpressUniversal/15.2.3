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

#if !SL
using DevExpress.Office.Utils;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Model = DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.Spreadsheet.Export {
	public interface ICellValueToColumnTypeConverter {
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007")]
		ConversionResult Convert(Cell readOnlyCell, CellValue cellValue, Type dataColumnType, out object result);
		CellValue EmptyCellValue { get; set; }
	}
	#region CellValueToStringConverter
	public class CellValueToStringConverter : ICellValueToColumnTypeConverter {
		Model.NumberFormat numberFormat = null;
		Model.WorkbookDataContext dataContext = null;
		public CellValueToStringConverter() {
			this.PreferredCulture = CultureInfo.InvariantCulture;
			this.EmptyCellValue = CellValue.Empty;
			this.SkipErrorValues = false;
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueToStringConverterPreferredNumberFormat")]
#endif
		public string PreferredNumberFormat {
			get { return numberFormat != null ? numberFormat.FormatCode : String.Empty; }
		}
		public void SetPreferredNumberFormat(IWorkbook workbook, string excelNumberFormat) {
			DevExpress.XtraSpreadsheet.Model.DocumentModel documentModel = workbook.Model.DocumentModel;
			this.dataContext = documentModel.DataContext;
			DevExpress.XtraSpreadsheet.Model.FormatBase defaultFormatCopy = documentModel.Cache.CellFormatCache.DefaultItem.Clone();
			try {
				defaultFormatCopy.BeginUpdate();
				defaultFormatCopy.ForceSetFormatString(excelNumberFormat);
			}
			catch {
				throw;
			}
			finally {
				defaultFormatCopy.EndUpdate();
				this.numberFormat = documentModel.Cache.NumberFormatCache[defaultFormatCopy.NumberFormatIndex];
			}
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueToStringConverterPreferredCulture")]
#endif
		public CultureInfo PreferredCulture { get; set; } 
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueToStringConverterEmptyCellValue")]
#endif
		public CellValue EmptyCellValue { get; set; }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueToStringConverterSkipErrorValues")]
#endif
		public bool SkipErrorValues { get; set; }
		public virtual ConversionResult Convert(Cell cell, CellValue cellValue, Type dataColumnType, out object result) {
			result = String.Empty;
			bool cellValueIsEmpty = cellValue.IsEmpty;
			if (cellValueIsEmpty)
				cellValue = EmptyCellValue;
			if (cellValue.IsError)
				return SkipErrorValues ? ConversionResult.Success
					: ConversionResult.Error;
			if (cellValue.IsText)
				result = cellValue.TextValue;
			else {
				bool cellValueNotEmpty = !cellValueIsEmpty;
				bool noCustomNumberformat = (this.numberFormat == null || dataContext == null);
				if (noCustomNumberformat) {
					if (cellValueNotEmpty)
						result = (cell as DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeCell).ReadOnlyModelCell.Text;
					else
						result = cellValue.TextValue;
				}
				else {
					try {
						result = ConvertCellValueWithNumberFormat(cellValue);
					}
					catch {
						return ConversionResult.Error;
					}
				}
			}
			return ConversionResult.Success;
		}
		private string ConvertCellValueWithNumberFormat(CellValue cellValue) {
			if (dataContext == null || numberFormat == null) 
				return String.Empty;
			dataContext.PushCulture(PreferredCulture);
			try {
				DevExpress.XtraSpreadsheet.Model.VariantValue modelValue = cellValue.ModelVariantValue;
				return this.numberFormat.Format(modelValue, dataContext).Text;
			}
			catch {
				throw;
			}
			finally {
				dataContext.PopCulture();
			}
		}
	}
	#endregion
	#region CellValueToColumnTypeConverter
	public class CellValueToColumnTypeConverter : ICellValueToColumnTypeConverter { 
		public CellValueToColumnTypeConverter() {
			this.ConvertStringValues = false;
			this.CheckNarrowingConversion = false;
			this.EmptyCellValue = CellValue.Empty;
			this.PreferredCulture = CultureInfo.InvariantCulture;
			this.PreferredNumberStyles = NumberStyles.Float | NumberStyles.AllowThousands;
			this.TreatStringAsZero = false;
			this.SkipErrorValues = false;
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueToColumnTypeConverterCheckNarrowingConversion")]
#endif
		public bool CheckNarrowingConversion { get; set; }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueToColumnTypeConverterPreferredCulture")]
#endif
		public CultureInfo PreferredCulture { get; set; } 
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueToColumnTypeConverterPreferredNumberStyles")]
#endif
		public NumberStyles PreferredNumberStyles { get; set; }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueToColumnTypeConverterEmptyCellValue")]
#endif
		public CellValue EmptyCellValue { get; set; }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueToColumnTypeConverterSkipErrorValues")]
#endif
		public bool SkipErrorValues { get; set; }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueToColumnTypeConverterTreatStringAsZero")]
#endif
		public bool TreatStringAsZero { get; set; }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("CellValueToColumnTypeConverterConvertStringValues")]
#endif
		public bool ConvertStringValues { get; set; }
		object DefaultResult { get { return DBNull.Value; } }
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007")]
		public virtual ConversionResult Convert(Cell cell, CellValue value, Type destinationType, out object result) {
			result = DefaultResult;
			ConversionResult converted = ConversionResult.Success;
			if (value.IsEmpty)
				value = EmptyCellValue;
			if (value.IsError)
				return SkipErrorValues ? ConversionResult.Success
					: ConversionResult.Error;
			double cellNumericValue = value.NumericValue;
			bool cellBoolean = value.BooleanValue;
			bool needConvertCellTextValueToType = false;
			if (value.IsText && destinationType != typeof(string)) {
				if (ConvertStringValues) {
					needConvertCellTextValueToType = true;
				}
				else {
					if (!TreatStringAsZero)
						return ConversionResult.Error;
					cellNumericValue = 0;
				}
			}
			if (destinationType == typeof(DateTime))
				return ConvertToDateTime(cell, value, cellNumericValue, needConvertCellTextValueToType, out result);
			else if (destinationType == typeof(Decimal))
				return ConvertToDecimal(cell, cellNumericValue, needConvertCellTextValueToType, out result);
			else if (destinationType == typeof(Double))
				return ConvertToDouble(cell, cellNumericValue, needConvertCellTextValueToType, out result);
			else if (destinationType == typeof(Single))
				return ConverToSingle(cell, cellNumericValue, needConvertCellTextValueToType, out result);
			else if (destinationType == typeof(Int64))
				return ConvertToInt64(cell, cellNumericValue, needConvertCellTextValueToType, out result);
			else if (destinationType == typeof(UInt64))
				return ConvertToUInt64(cell, cellNumericValue, needConvertCellTextValueToType, out result);
			else if (destinationType == typeof(Int32))
				return ConvertToInt32(cell, cellNumericValue, needConvertCellTextValueToType, out result);
			else if (destinationType == typeof(UInt32))
				return ConvertToUInt32(cell, cellNumericValue, needConvertCellTextValueToType, out result);
			else if (destinationType == typeof(Int16))
				return ConvertToInt16(cell, cellNumericValue, needConvertCellTextValueToType, out result);
			else if (destinationType == typeof(UInt16))
				return ConvertToUInt16(cell, cellNumericValue, needConvertCellTextValueToType, out result);
			else if (destinationType == typeof(Byte))
				return ConvertToByte(cell, cellNumericValue, needConvertCellTextValueToType, out result);
			else if (destinationType == typeof(SByte))
				return ConvertToSByte(cell, cellNumericValue, needConvertCellTextValueToType, out result);
			else if (destinationType == typeof(Char))
				return ConvertToChar(cell, cellNumericValue, needConvertCellTextValueToType, out result);
			else if (destinationType == typeof(Boolean))
				return ConvertToBoolean(cell, cellBoolean, cellNumericValue, needConvertCellTextValueToType, out result);
			else
				converted = ConversionResult.Error;
			return converted;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007")]
		ConversionResult ConvertToDateTime(Cell cell, CellValue value, double cellNumericValue, bool needConvertCellTextValueToType, out object result) {
			result = DefaultResult;
			DateTime cellDateTimeValue = value.DateTimeValue;
			if (needConvertCellTextValueToType) {
				string cellStringValue = GetCellDisplayText(cell);
				DateTime dateTimeValue = DateTime.MinValue;
				if (!TryConvertCellTextValueToDateTime(cell, cellStringValue, ref dateTimeValue)) {
					return ConversionResult.Error;
				}
				cellDateTimeValue = dateTimeValue;
			}
			if (cellDateTimeValue != DateTime.MinValue
				&& cellDateTimeValue != DateTime.MaxValue)
				result = (DateTime)cellDateTimeValue;
			else {
				if (value.IsEmpty) {
					return ConversionResult.Success;
				}
				return ConversionResult.Error;
			}
			return ConversionResult.Success;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007")]
		ConversionResult ConvertToBoolean(Cell cell, bool cellBoolean, double cellNumericValue, bool needConvertCellTextValueToType, out object result) {
			if (needConvertCellTextValueToType) {
				string cellStringValue = GetCellDisplayText(cell);
				if (!TryConvertCellTextValueToBoolean(cell, cellStringValue, out cellBoolean)) {
					result = DefaultResult;
					return ConversionResult.Error;
				}
			}
			else if (CheckNarrowingConversion) {
				if (IsNarrowingConversion(typeof(Boolean), cellNumericValue, out result))
					return ConversionResult.Overflow;
				return ConversionResult.Success;
			}
			checked { result = (bool)cellBoolean; }
			return ConversionResult.Success;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007")]
		ConversionResult ConvertToChar(Cell cell, double cellNumericValue, bool needConvertCellTextValueToType, out object result) {
			if (needConvertCellTextValueToType) {
				string cellStringValue = GetCellDisplayText(cell);
				Char charValue = Char.MinValue;
				if (!TryConvertCellTextValueToChar(cell, cellStringValue, out charValue)) {
					result = DefaultResult;
					return ConversionResult.Error;
				}
				cellNumericValue = charValue;
			}
			else if (CheckNarrowingConversion) {
				if (IsNarrowingConversion(typeof(Char), cellNumericValue, out result))
					return ConversionResult.Overflow;
				return ConversionResult.Success;
			}
			checked { result = (Char)cellNumericValue; }
			return ConversionResult.Success;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007")]
		ConversionResult ConvertToSByte(Cell cell, double cellNumericValue, bool needConvertCellTextValueToType, out object result) {
			if (needConvertCellTextValueToType) {
				string cellStringValue = GetCellDisplayText(cell);
				SByte sbyteValue = 0;
				if (!TryConvertCellTextValueToSByte(cell, cellStringValue, out sbyteValue)) {
					result = DefaultResult;
					return ConversionResult.Error;
				}
				cellNumericValue = sbyteValue;
			}
			else if (CheckNarrowingConversion) {
				if (IsNarrowingConversion(typeof(SByte), cellNumericValue, out result))
					return ConversionResult.Overflow;
				return ConversionResult.Success;
			}
			checked { result = (SByte)cellNumericValue; }
			return ConversionResult.Success;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007")]
		ConversionResult ConvertToByte(Cell cell, double cellNumericValue, bool needConvertCellTextValueToType, out object result) {
			if (needConvertCellTextValueToType) {
				string cellStringValue = GetCellDisplayText(cell);
				byte byteValue = 0;
				if (!TryConvertCellTextValueToByte(cell, cellStringValue, out byteValue)) {
					result = DefaultResult;
					return ConversionResult.Error;
				}
				cellNumericValue = byteValue;
			}
			else if (CheckNarrowingConversion) {
				if (IsNarrowingConversion(typeof(Byte), cellNumericValue, out result))
					return ConversionResult.Overflow;
				return ConversionResult.Success;
			}
			checked { result = (byte)cellNumericValue; }
			return ConversionResult.Success;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007")]
		ConversionResult ConvertToUInt16(Cell cell, double cellNumericValue, bool needConvertCellTextValueToType, out object result) {
			if (needConvertCellTextValueToType) {
				string cellStringValue = GetCellDisplayText(cell);
				ushort ushortValue = 0;
				if (!TryConvertCellTextValueToUShort(cell, cellStringValue, out ushortValue)) {
					result = DefaultResult;
					return ConversionResult.Error;
				}
				cellNumericValue = ushortValue;
			}
			else if (CheckNarrowingConversion) {
				if (IsNarrowingConversion(typeof(UInt16), cellNumericValue, out result))
					return ConversionResult.Overflow;
				return ConversionResult.Success;
			}
			checked { result = (ushort)cellNumericValue; }
			return ConversionResult.Success;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007")]
		ConversionResult ConvertToInt16(Cell cell, double cellNumericValue, bool needConvertCellTextValueToType, out object result) {
			if (needConvertCellTextValueToType) {
				string cellStringValue = GetCellDisplayText(cell);
				short shortValue = 0;
				if (!TryConvertCellTextValueToShort(cell, cellStringValue, out shortValue)) {
					result = DefaultResult;
					return ConversionResult.Error;
				}
				cellNumericValue = shortValue;
			}
			else if (CheckNarrowingConversion) {
				if (IsNarrowingConversion(typeof(Int16), cellNumericValue, out result))
					return ConversionResult.Overflow;
				return ConversionResult.Success;
			}
			checked { result = (short)cellNumericValue; }
			return ConversionResult.Success;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007")]
		ConversionResult ConvertToUInt32(Cell cell, double cellNumericValue, bool needConvertCellTextValueToType, out object result) {
			if (needConvertCellTextValueToType) {
				string cellStringValue = GetCellDisplayText(cell);
				uint uintValue = 0;
				if (!TryConvertCellTextValueToUInt(cell, cellStringValue, out uintValue)) {
					result = DefaultResult;
					return ConversionResult.Error;
				}
				cellNumericValue = uintValue;
			}
			else if (CheckNarrowingConversion) {
				if (IsNarrowingConversion(typeof(UInt32), cellNumericValue, out result))
					return ConversionResult.Overflow;
				return ConversionResult.Success;
			}
			checked { result = (uint)cellNumericValue; }
			return ConversionResult.Success;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007")]
		ConversionResult ConvertToDecimal(Cell cell, double cellNumericValue, bool needConvertCellTextValueToType, out object result) {
			Decimal decimalValue = (Decimal)cellNumericValue;
			if (needConvertCellTextValueToType) {
				string cellStringValue = GetCellDisplayText(cell);
				if (!TryConvertCellTextValueToDecimal(cell, cellStringValue, out decimalValue)) {
					result = DefaultResult;
					return ConversionResult.Error;
				}
			}
			checked { result = decimalValue; }
			return ConversionResult.Success;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007")]
		ConversionResult ConvertToInt32(Cell cell, double cellNumericValue, bool needConvertCellTextValueToType, out object result) {
			if (needConvertCellTextValueToType) {
				string cellStringValue = GetCellDisplayText(cell);
				int intValue = 0;
				if (!TryConvertCellTextValueToInt32(cell, cellStringValue, out intValue)) {
					result = DefaultResult;
					return ConversionResult.Error;
				}
				cellNumericValue = intValue;
			}
			else if (CheckNarrowingConversion) {
				if (IsNarrowingConversion(typeof(Int32), cellNumericValue, out result))
					return ConversionResult.Overflow;
				return ConversionResult.Success;
			}
			checked { result = (int)cellNumericValue; }
			return ConversionResult.Success;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007")]
		ConversionResult ConvertToUInt64(Cell cell, double cellNumericValue, bool needConvertCellTextValueToType, out object result) {
			if (needConvertCellTextValueToType) {
				string cellStringValue = GetCellDisplayText(cell);
				UInt64 uInt64Value = 0;
				if (!TryConvertCellTextValueToUInt64(cell, cellStringValue, out uInt64Value)) {
					result = DefaultResult;
					return ConversionResult.Error;
				}
				cellNumericValue = uInt64Value;
			}
			else if (CheckNarrowingConversion) {
				if (IsNarrowingConversion(typeof(UInt64), cellNumericValue, out result))
					return ConversionResult.Overflow;
				return ConversionResult.Success;
			}
			checked { result = (float)cellNumericValue; }
			return ConversionResult.Success;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007")]
		ConversionResult ConvertToInt64(Cell cell, double cellNumericValue, bool needConvertCellTextValueToType, out object result) {
			if (needConvertCellTextValueToType) {
				string cellStringValue = GetCellDisplayText(cell);
				long longValue = 0;
				if (!TryConvertCellTextValueToLong(cell, cellStringValue, out longValue)) {
					result = DefaultResult;
					return ConversionResult.Error;
				}
				cellNumericValue = longValue;
			}
			else if (CheckNarrowingConversion) {
				if (IsNarrowingConversion(typeof(Int64), cellNumericValue, out result))
					return ConversionResult.Overflow;
				return ConversionResult.Success;
			}
			checked { result = (long)cellNumericValue; }
			return ConversionResult.Success;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007")]
		ConversionResult ConverToSingle(Cell cell, double cellNumericValue, bool needConvertCellTextValueToType, out object result) {
			if (needConvertCellTextValueToType) {
				string cellStringValue = GetCellDisplayText(cell);
				float floatValue = 0;
				if (!TryConvertCellTextValueToFloat(cell, cellStringValue, out floatValue)) {
					result = DefaultResult;
					return ConversionResult.Error;
				}
				cellNumericValue = floatValue;
			}
			else if (CheckNarrowingConversion) {
				if (IsNarrowingConversion(typeof(Single), cellNumericValue, out result))
					return ConversionResult.Overflow;
				return ConversionResult.Success;
			}
			checked { result = (float)cellNumericValue; }
			return ConversionResult.Success;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007")]
		ConversionResult ConvertToDouble(Cell cell, double cellNumericValue, bool needConvertCellTextValueToType, out object result) {
			if (needConvertCellTextValueToType) {
				string cellStringValue = GetCellDisplayText(cell);
				double doubleValue = 0;
				if (!TryConvertCellTextValueToDouble(cell, cellStringValue, out doubleValue)) {
					result = DefaultResult;
					return ConversionResult.Error;
				}
				cellNumericValue = doubleValue;
			}
			else if (CheckNarrowingConversion) {
				if (IsNarrowingConversion(typeof(Double), cellNumericValue, out result))
					return ConversionResult.Overflow;
				return ConversionResult.Success;
			}
			result = cellNumericValue;
			return ConversionResult.Success;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007")]
		protected virtual bool IsNarrowingConversion(Type type, double cellNumericValue, out object result) {
			try {
				result = System.Convert.ChangeType(cellNumericValue, type, PreferredCulture);
			}
			catch (OverflowException) {
				result = cellNumericValue;
				return true;
			}
			return false;
		}
		#region TryConvertCellTextValueTo methods
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045")]
		protected virtual bool TryConvertCellTextValueToDateTime(Cell cell, string cellStringValue, ref DateTime dateTimeValue) {
			bool result = false;
			return result;
		}
		[CLSCompliant(false)]
		protected virtual bool TryConvertCellTextValueToUInt(Cell cell, string cellStringValue, out uint uintValue) {
			return uint.TryParse(cellStringValue, PreferredNumberStyles, PreferredCulture, out uintValue);
		}
		[CLSCompliant(false)]
		protected virtual bool TryConvertCellTextValueToUInt64(Cell cell, string cellStringValue, out ulong uInt64Value) {
			return UInt64.TryParse(cellStringValue, PreferredNumberStyles, PreferredCulture, out uInt64Value);
		}
		[CLSCompliant(false)]
		protected virtual bool TryConvertCellTextValueToUShort(Cell cell, string cellStringValue, out ushort ushortValue) {
			return ushort.TryParse(cellStringValue, PreferredNumberStyles, PreferredCulture, out ushortValue);
		}
		protected virtual bool TryConvertCellTextValueToByte(Cell cell, string cellStringValue, out byte byteValue) {
			return Byte.TryParse(cellStringValue, PreferredNumberStyles, PreferredCulture, out byteValue);
		}
		[CLSCompliant(false)]
		protected virtual bool TryConvertCellTextValueToSByte(Cell cell, string cellStringValue, out sbyte sbyteValue) {
			return SByte.TryParse(cellStringValue, PreferredNumberStyles, PreferredCulture, out sbyteValue);
		}
		protected virtual bool TryConvertCellTextValueToChar(Cell cell, string cellStringValue, out char charValue) {
			return Char.TryParse(cellStringValue, out charValue);
		}
		protected virtual bool TryConvertCellTextValueToShort(Cell cell, string cellStringValue, out short shortValue) {
			return short.TryParse(cellStringValue, PreferredNumberStyles, PreferredCulture, out shortValue);
		}
		protected virtual bool TryConvertCellTextValueToFloat(Cell cell, string cellStringValue, out float floatValue) {
			return float.TryParse(cellStringValue, PreferredNumberStyles, PreferredCulture, out floatValue);
		}
		protected virtual bool TryConvertCellTextValueToDecimal(Cell cell, string cellStringValue, out Decimal DecimalValue) {
			return Decimal.TryParse(cellStringValue, PreferredNumberStyles, PreferredCulture, out DecimalValue);
		}
		protected virtual bool TryConvertCellTextValueToLong(Cell cell, string cellStringValue, out long longValue) {
			return long.TryParse(cellStringValue, PreferredNumberStyles, PreferredCulture, out longValue);
		}
		protected virtual bool TryConvertCellTextValueToDouble(Cell cell, string cellStringValue, out double cellNumericValue) {
			return Double.TryParse(cellStringValue, PreferredNumberStyles, PreferredCulture, out cellNumericValue);
		}
		protected virtual bool TryConvertCellTextValueToInt32(Cell cell, string cellStringValue, out int intValue) {
			return Int32.TryParse(cellStringValue, PreferredNumberStyles, PreferredCulture, out intValue);
		}
		protected virtual bool TryConvertCellTextValueToBoolean(Cell cell, string cellStringValue, out bool booleanValue) {
			return Boolean.TryParse(cellStringValue, out booleanValue);
		}
		public virtual string GetCellDisplayText(Cell cell) {
			return (cell as DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeCell).ReadOnlyModelCell.Text;
		}
		#endregion
	}
	#endregion
}
#endif
