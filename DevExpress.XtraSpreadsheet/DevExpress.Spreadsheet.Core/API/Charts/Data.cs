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
using System.Diagnostics;
using System.Globalization;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.API.Native.Implementation;
using ModelCellErrorType = DevExpress.XtraSpreadsheet.Model.ModelCellErrorType;
using ModelCellRange = DevExpress.XtraSpreadsheet.Model.CellRange;
using ModelIVariantArray = DevExpress.XtraSpreadsheet.Model.IVariantArray;
using ModelVariantArray = DevExpress.XtraSpreadsheet.Model.VariantArray;
using ModelVariantValue = DevExpress.XtraSpreadsheet.Model.VariantValue;
using ModelVariantValueType = DevExpress.XtraSpreadsheet.Model.VariantValueType;
using ModelWorkbookDataContext = DevExpress.XtraSpreadsheet.Model.WorkbookDataContext;
using Model = DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.Spreadsheet.Charts {
	#region ChartDataDirection
	public enum ChartDataDirection {
		Row,
		Column
	}
	#endregion
	#region ChartData
	public class ChartData {
		static readonly ChartData empty = new ChartData(ModelVariantValue.Empty);
		#region Errors
		public static ChartData ErrorInvalidValueInFunction = new ChartData(ModelVariantValue.ErrorInvalidValueInFunction);
		public static ChartData ErrorDivisionByZero = new ChartData(ModelVariantValue.ErrorDivisionByZero);
		public static ChartData ErrorNumber = new ChartData(ModelVariantValue.ErrorNumber);
		public static ChartData ErrorReference = new ChartData(ModelVariantValue.ErrorReference);
		public static ChartData ErrorValueNotAvailable = new ChartData(ModelVariantValue.ErrorValueNotAvailable);
		public static ChartData ErrorNullIntersection = new ChartData(ModelVariantValue.ErrorNullIntersection);
		public static ChartData ErrorName = new ChartData(ModelVariantValue.ErrorName);
		protected internal static Dictionary<ModelCellErrorType, ChartData> modelErrorTypeToApiErrorTable = CreateErrorTable();
		static Dictionary<ModelCellErrorType, ChartData> CreateErrorTable() {
			Dictionary<ModelCellErrorType, ChartData> result = new Dictionary<ModelCellErrorType, ChartData>();
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
		NativeWorksheet rangeSheet;
		#endregion
		public ChartData() {
			modelValue = ModelVariantValue.Empty;
		}
		internal ChartData(ModelVariantValue modelValue, ModelWorkbookDataContext context) {
			Debug.Assert(!modelValue.IsCellRange);
			if (modelValue.IsSharedString)
				this.modelValue = modelValue.GetTextValue(context.Workbook.SharedStringTable);
			else
				this.modelValue = modelValue;
		}
		internal ChartData(ModelVariantValue modelValue, NativeWorksheet rangeSheet) {
			Debug.Assert(!modelValue.IsSharedString);
			this.modelValue = modelValue;
			this.rangeSheet = rangeSheet;
		}
		internal ChartData(ModelVariantValue modelValue) {
			Debug.Assert(!modelValue.IsSharedString && !modelValue.IsCellRange);
			this.modelValue = modelValue;
		}
		#region Properties
		public static ChartData Empty { get { return ChartData.empty; } }
		public ErrorValueInfo ErrorValue { get { return !IsError ? null : new NativeErrorInfo(modelValue.ErrorValue); } }
		protected internal ModelVariantValue ModelVariantValue { get { return modelValue; } protected set { modelValue = value; } }
		public bool IsEmpty { [DebuggerStepThrough] get { return ModelVariantValue.IsEmpty; } }
		public bool IsNumeric { [DebuggerStepThrough] get { return ModelVariantValue.IsNumeric; } }
		public bool IsBoolean { [DebuggerStepThrough] get { return ModelVariantValue.IsBoolean; } }
		public bool IsError { [DebuggerStepThrough] get { return ModelVariantValue.IsError; } }
		public bool IsText { [DebuggerStepThrough] get { return IsInlineText || IsSharedString; } }
		bool IsInlineText { [DebuggerStepThrough] get { return ModelVariantValue.IsInlineText; } }
		bool IsSharedString { [DebuggerStepThrough] get { return ModelVariantValue.IsSharedString; } }
		public bool IsArray { [DebuggerStepThrough] get { return ModelVariantValue.IsArray; } }
		public bool IsRange { [DebuggerStepThrough] get { return ModelVariantValue.IsCellRange; } }
		public bool BooleanValue { get { return ModelVariantValue.BooleanValue; } set { ModelVariantValue = value; } }
		public double NumericValue { get { return ModelVariantValue.NumericValue; } set { ModelVariantValue = value; } }
		public string TextValue { get { return ModelVariantValue.InlineTextValue; } set { ModelVariantValue = value; } }
		public Range RangeValue { get { return new NativeRange(ModelVariantValue.CellRangeValue, rangeSheet); } set { SetRangeValue(value); } }
		public CellValue[] ArrayValue { get { return ToArrayValue(); } }
		#endregion
		#region Equality
		public override int GetHashCode() {
			return modelValue.GetHashCode();
		}
		public override bool Equals(object obj) {
			ChartData nativeValue = obj as ChartData;
			if (nativeValue == null)
				return false;
			return AreEqual(this, nativeValue);
		}
		public static bool operator ==(ChartData value, ChartData other) {
			return AreEqual(value, other);
		}
		public static bool operator !=(ChartData value, ChartData other) {
			return !AreEqual(value, other);
		}
		#endregion
		#region implicit conversion to CellValue
		public static implicit operator ChartData(int value) {
			return new ChartData(value);
		}
		[DebuggerStepThrough]
		public static implicit operator ChartData(double value) {
			return new ChartData(value);
		}
		[DebuggerStepThrough]
		public static implicit operator ChartData(char value) {
			return new ChartData(value);
		}
		[DebuggerStepThrough]
		public static implicit operator ChartData(string value) {
			return new ChartData(value);
		}
		[DebuggerStepThrough]
		public static implicit operator ChartData(DateTime value) {
			return FromDateTime(value, false);
		}
		[DebuggerStepThrough]
		public static implicit operator ChartData(bool value) {
			return new ChartData(value);
		}
		[DebuggerStepThrough]
		public static implicit operator ChartData(CellValue value) {
			if (value != null)
				return new ChartData(value.ModelVariantValue);
			return new ChartData();
		}
		[DebuggerStepThrough]
		public static implicit operator ChartData(CellValue[] values) {
			return ChartData.FromArray(values);
		}
		#endregion
		[DebuggerStepThrough]
		public static ChartData FromRange(Range value) {
			ModelVariantValue modelValue = new ModelVariantValue();
			modelValue.CellRangeValue = GetModelCellRange(value);
			return new ChartData(modelValue, (NativeWorksheet)value.Worksheet);
		}
		[DebuggerStepThrough]
		public static ChartData FromDateTime(DateTime value, bool use1904DateSystem) {
			return new ChartData(CreateModelDateTime(value, use1904DateSystem));
		}
		[DebuggerStepThrough]
		public static ChartData FromArray(CellValue[] values) {
			int length = values.Length;
			ModelVariantValue[] modelValues = new ModelVariantValue[length];
			for (int i = 0; i < length; i++) {
				CellValue value = values[i];
				if (value == null)
					modelValues[i] = 0;
				else
					modelValues[i] = value.ModelVariantValue;
			}
			ModelVariantArray variantArray = new ModelVariantArray();
			variantArray.SetValues(modelValues, length, 1);
			return new ChartData(ModelVariantValue.FromArray(variantArray));
		}
		static ModelCellRange GetModelCellRange(Range range) {
			NativeWorksheet nativeWorksheet = range.Worksheet as NativeWorksheet;
			if (nativeWorksheet == null)
				return null;
			return nativeWorksheet.GetModelSingleRange(range);
		}
		static bool AreEqual(ChartData value, ChartData other) {
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
		void SetRangeValue(Range value) {
			ModelVariantValue = ModelVariantValue.Create(GetModelCellRange(value));
			rangeSheet = (NativeWorksheet)value.Worksheet;
		}
		CellValue[] ToArrayValue() {
			if (!ModelVariantValue.IsArray)
				return null;
			ModelIVariantArray modelArray = ModelVariantValue.ArrayValue;
			if (modelArray == null)
				return null;
			int width = modelArray.Width;
			int height = modelArray.Height;
			CellValue[] result = new CellValue[height * width];
			for (int j = 0; j < height; j++)
				for (int i = 0; i < width; i++)
					result[j * width + i] = new CellValue(modelArray.GetValue(j, i), false);
			return result;
		}
		#region ToString
		public override string ToString() {
			return ToString(CultureInfo.CurrentCulture);
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
			return RangeValue.GetReferenceA1();
			case ModelVariantValueType.Array:
			return string.Format("<array>[{0}, {1}]", modelValue.ArrayValue.Height, modelValue.ArrayValue.Width);
			}
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Charts;
	using DevExpress.Office.API.Internal;
	static class ChartDataConverter {
		public static Model.IDataReference ToDataReference(ChartData data, Model.DocumentModel documentModel, Model.ChartViewSeriesDirection seriesDirection, bool isNumber) {
			if (data.IsEmpty)
				return Model.DataReference.Empty;
			Model.ChartDataReference result = new Model.ChartDataReference(documentModel, seriesDirection, isNumber);
			result.SetVariantValueWithoutHistory(data.ModelVariantValue);
			return result;
		}
		public static ChartData ToChartData(Model.IDataReference reference, NativeWorkbook workbook) {
			Model.ChartDataReference modelChartDataReference = reference as Model.ChartDataReference;
			if (modelChartDataReference == null)
				return ChartData.Empty;
			Model.VariantValue value = modelChartDataReference.FormulaData.CachedValue;
			Model.WorkbookDataContext modelContext = workbook.ModelWorkbook.DataContext;
			if (value.IsCellRange) {
				if (value.CellRangeValue.Worksheet is Model.ChartDataCache) {
					modelChartDataReference.FormulaData.CachedValue = ModelVariantValue.Empty;
					value = modelChartDataReference.FormulaData.CachedValue;
					if(!value.IsCellRange)
						return CreateChartDataFromValue(value, modelContext);
				}
				Model.ICellTable modelSheet = value.CellRangeValue.GetFirstInnerCellRange().Worksheet;
				string sheetName = modelSheet == null ? modelContext.CurrentWorksheet.Name : modelSheet.Name;
				NativeWorksheet sheet = (NativeWorksheet)workbook.Worksheets[sheetName];
				return new ChartData(value, sheet);
			} else
				return CreateChartDataFromValue(value, modelContext);
		}
		static ChartData CreateChartDataFromValue(ModelVariantValue value, Model.WorkbookDataContext modelContext) {
			return new ChartData(value, modelContext);
		}
		public static bool IsNumber(ChartData data) {
			if (data.IsNumeric)
				return true;
			if (data.IsArray) {
				ModelIVariantArray modelArray = data.ModelVariantValue.ArrayValue;
				if (modelArray == null)
					return false;
				for (int i = 0; i < modelArray.Count; i++)
					if (!modelArray[i].IsNumeric)
						return false;
				return true;
			}
			if (data.IsRange) {
				ModelCellRange range = data.ModelVariantValue.CellRangeValue as ModelCellRange;
				if (range == null)
					return false;
				Model.ICellTable sheet = range.Worksheet;
				for (int i = range.TopLeft.Row; i <= range.BottomRight.Row; i++) 
					for (int j = range.TopLeft.Column; j <= range.BottomRight.Column; j++) {
					Model.ICellBase cell = sheet.TryGetCell(j, i);
					if (cell == null || !cell.Value.IsNumeric)
						return false;
				}
				return true;
			}
			return false;
		}
	}
}
