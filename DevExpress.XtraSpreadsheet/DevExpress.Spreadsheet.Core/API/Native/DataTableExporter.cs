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
using System.Diagnostics;
using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Export;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Services.Implementation;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Model = DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	partial class NativeWorksheet {
		protected internal DataTableExporter CreateDataTableExporter(Range range, DataTable dataTable, bool rangeHasHeaders) {
			DataTableExporter result = new DataTableExporter(dataTable, range, rangeHasHeaders);
			result.Options = new DataTableExportOptions();
			result.Options.DefaultCellValueToColumnTypeConverter = new CellValueToColumnTypeConverter();
			result.Options.DefaultCellValueToStringConverter = new CellValueToStringConverter();
			result.Options.CustomConverters = new Dictionary<string, ICellValueToColumnTypeConverter>();
			return result;
		}
		protected internal DataTable CreateDataTable(Range range, bool rangeHasHeaders) {
			return CreateDataTable(range, rangeHasHeaders, false);
		}
		protected internal DataTable CreateDataTable(Range range, bool rangeHasHeaders, bool stringColumnType) {
			DataTable result = new DataTable();
			Model.CellRange headerRange;
			Model.CellRange dataRange;
			NativeWorksheet nativeWorksheet = (NativeWorksheet)range.Worksheet;
			Model.CellRange modelRange = nativeWorksheet.GetModelSingleRange(range);
			SplitRangeToDataAndHeaderRanges(modelRange, out headerRange, out dataRange, rangeHasHeaders);
			Range headerApiRange = new NativeRange(headerRange, nativeWorksheet);
			try {
				result.BeginInit();
				AllocateNecessaryDataRows( result, dataRange, true);
				result.Locale = Workbook.Options.Culture;
				int width = headerRange.Width;
				DataColumnCollection columns = result.Columns;
				if (dataRange == null || stringColumnType)
					CreateStringColumns(columns, width);
				else
					CreateTypedColumns(columns, width, dataRange);
				if (rangeHasHeaders) {
					IDataColumnNameCreationService service = headerApiRange.Worksheet.Workbook.GetService(typeof(IDataColumnNameCreationService)) as IDataColumnNameCreationService;
					if (service == null) {
						Debug.Assert(service != null, "IDataColumnNameCreationService is not initialized.", "");
						throw new InvalidOperationException();
					}
					service.ExtractDataTableColumnNamesFromHeaderRange(result, headerApiRange);
				}
			}
			finally {
				result.EndInit();
			}
			return result;
		}
		void CreateStringColumns(DataColumnCollection columns, int width) {
			for (int i = 0; i < width; i++)
				columns.Add(new DataColumn() { DataType = typeof(string) });
		}
		void CreateTypedColumns(DataColumnCollection columns, int width, Model.CellRange dataRange) {
			for (int i = 0; i < width; i++) {
				Type detectedType = DetectRangeColumnType(dataRange, i, typeof(string), 2);
				columns.Add(new DataColumn() { DataType = detectedType });
			}
		}
		protected internal void AllocateNecessaryDataRows(DataTable dataTable, Model.CellRange dataRange, bool skipEmptyRows) {
			const int defaultDataRowAllocationCount = 50;
			if (dataRange == null || dataRange.Height < defaultDataRowAllocationCount)
				return;
			if (!skipEmptyRows) {
				dataTable.MinimumCapacity = dataRange.Height;
				return;
			}
			int calculatedCapacity = 0;
			IEnumerator<Model.Row> existingRowsEnumerator = ModelWorksheet.Rows.GetExistingRowsEnumerator(dataRange.TopLeft.Row, dataRange.BottomRight.Row, false);
			while(existingRowsEnumerator.MoveNext())
				calculatedCapacity++;
			dataTable.MinimumCapacity = calculatedCapacity;
		}
		static Type DetectRangeColumnType(Model.CellRange dataRange, int columnIndexInRange, Type defaultType, int maxAttemptsToDetect) {
			Model.CellRange columnRange = dataRange.GetSubColumnRange(columnIndexInRange, columnIndexInRange);
			Model.ICell nonEmptyModelCell = null;
			foreach (Model.CellBase rangeCellInfo in columnRange.GetExistingCellsEnumerable()) {
				Model.ICell cell = rangeCellInfo as Model.ICell;
				bool cellIsNotEmpty = cell.HasFormula || !cell.Value.IsEmpty;
				if (!cellIsNotEmpty)
					continue;
				nonEmptyModelCell = cell;
				break;
			}
			if(nonEmptyModelCell == null)
				return defaultType;
			Model.VariantValue value = nonEmptyModelCell.Value;
			Model.NumberFormat numberFormat = nonEmptyModelCell.ActualFormat;
			bool numberFormatIsText = numberFormat.IsText;
			if (value.IsText || numberFormatIsText)
				return typeof(string);
			if(value.IsBoolean)
				return typeof(bool);
			if (value.IsNumeric) {
				if (value.NumericValue < 0)
					return typeof(double);
				bool displayedAsDateTime = numberFormat.IsDateTime && !numberFormatIsText;
				if (displayedAsDateTime)
					return typeof(DateTime);
				return typeof(double);
			}
			return defaultType;
		}
		internal static void SplitRangeToDataAndHeaderRanges(Model.CellRange range, out Model.CellRange headerRange, out Model.CellRange dataRange, bool rangeHasHeaders) {
			Model.CellPosition headerRangeTopLeft = new Model.CellPosition(range.LeftColumnIndex, range.TopRowIndex);
			Model.CellPosition headerRangeBottomRight = new Model.CellPosition(range.RightColumnIndex, range.TopRowIndex);
			headerRange = new Model.CellRange(range.Worksheet, headerRangeTopLeft, headerRangeBottomRight);
			dataRange = range;
			if (rangeHasHeaders) {
				if (range.Height <= 1)
					dataRange = null;
				else {
					Model.CellPosition dataRangeTopLeft = new Model.CellPosition(range.LeftColumnIndex, range.TopRowIndex + 1);
					Model.CellPosition dataRangeBottomRight = new Model.CellPosition(range.RightColumnIndex, range.BottomRowIndex);
					dataRange = new Model.CellRange(range.Worksheet, dataRangeTopLeft, dataRangeBottomRight);
				}
			}
		}
	}
}
namespace DevExpress.Spreadsheet.Export {
	public enum ConversionResult {
		Success,
		Overflow, 
		Error
	}
	public enum DataTableExporterAction {
		Continue = 1,
		SkipRow = 3,
		Stop = 2
	}
	public enum DataTableExporterOptimizationType {
		Speed = 0,
		Memory = 1 
	};
	public class DataTableExporter { 
		readonly Range range;
		readonly bool rangeHasHeaders;
		readonly DataTable dataTable;
		CellValueConversionErrorEventHandler onConversionError;
		ProcessEmptyRowEventHandler onProcessEmptyRow;
		CellValueConversionErrorEventArgs args;
		ProcessEmptyRowEventArgs processEmptyRowArgs;
		DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeCell cachedNativeCell;
		DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeCellContent emptyCellContentForEmptyCells;
		CellValue cachedCellValue = CellValue.Empty;
		bool[] useCustomConverterForColumnFlag;
		IDataValueSetter valueSettingStrategy = null;
		public DataTableExporter(DataTable dataTable, Range range, bool rangeHasHeaders) {
			DevExpress.Utils.Guard.ArgumentNotNull(dataTable, "dataTable");
			DevExpress.Utils.Guard.ArgumentNotNull(range, "range");
			if (dataTable.Columns.Count != range.ColumnCount || dataTable.Columns.Count == 0) {
				string str = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorRangeColumnCountIsNotTheSameAsColumnCountInDataTable);
				throw new ArgumentException(str);
			}
			this.dataTable = dataTable;
			this.range = range;
			this.rangeHasHeaders = rangeHasHeaders;
			Initialize();
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("DataTableExporterRange")]
#endif
		public Range Range { get { return range; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("DataTableExporterRangeHasHeaders")]
#endif
		public bool RangeHasHeaders { get { return rangeHasHeaders; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("DataTableExporterDataTable")]
#endif
		public DataTable DataTable { get { return dataTable; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("DataTableExporterOptions")]
#endif
		public DataTableExportOptions Options { get; set; }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("DataTableExporterCellValueConversionError")]
#endif
		public event CellValueConversionErrorEventHandler CellValueConversionError {
			add { onConversionError += value; }
			remove { onConversionError -= value; }
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("DataTableExporterProcessEmptyRow")]
#endif
		public event ProcessEmptyRowEventHandler ProcessEmptyRow {
			add { onProcessEmptyRow += value; }
			remove { onProcessEmptyRow -= value; }
		}
		void Initialize() {
			this.args = new CellValueConversionErrorEventArgs();
			this.processEmptyRowArgs = new ProcessEmptyRowEventArgs();
			this.useCustomConverterForColumnFlag = null;
		}
		protected internal void Export() {
			DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeWorksheet nativeWorksheet = Range.Worksheet as DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeWorksheet;
			Model.Worksheet modelWorksheet = nativeWorksheet.ModelWorksheet;
			int nextRowIndex = RangeHasHeaders ? Range.TopRowIndex + 1 : Range.TopRowIndex;
			if (nextRowIndex > Range.BottomRowIndex)
				return;
			Initialize();
			this.useCustomConverterForColumnFlag = CreateUseCustomConverterForColumnFlags(DataTable.Columns, Options.CustomConverters);
			this.cachedNativeCell = new DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeCell(nativeWorksheet, new Model.CellKey(nativeWorksheet.ModelWorksheet.SheetId, 0, 0));
			this.emptyCellContentForEmptyCells = new XtraSpreadsheet.API.Native.Implementation.NativeCellContent(new Model.CellKey(nativeWorksheet.ModelWorksheet.SheetId, 0, 0), nativeWorksheet);
			this.cachedCellValue = new CellValue(Model.VariantValue.Empty, modelWorksheet.DataContext);
			this.valueSettingStrategy = CreateValueSettingStrategy(dataTable.Columns.Count, Options.OptimizationType);
			try {
				this.dataTable.BeginLoadData(); 
				DataTableExporterAction exportFlowAction = DataTableExporterAction.Continue;
				IEnumerable<Model.Row> rowsExistingOrFake = GetRowsToExport(modelWorksheet,nextRowIndex, Range.BottomRowIndex);
				foreach (Model.Row modelRow in rowsExistingOrFake) {
					while (modelRow.Index > nextRowIndex) {
						if (!Options.SkipEmptyRows) {
							bool shouldAddAnEmptyRow = true;
							if (onProcessEmptyRow != null) {
								DataTableExporterAction action = RaiseProcessEmptyRow(modelRow.Index);
								shouldAddAnEmptyRow = action == DataTableExporterAction.Continue;
								if (action == DataTableExporterAction.Stop)
									return;
							}
							if(shouldAddAnEmptyRow)
								DataTable.Rows.Add(DataTable.NewRow());
						}
						nextRowIndex++;
					}
					if (modelRow.Index == nextRowIndex) {
						DataRow dataRow = DataTable.NewRow();
						dataRow.BeginEdit();
						try {
							valueSettingStrategy.BeforeExportNewRow(dataRow);
							ExportWorksheetRow(modelRow, nativeWorksheet, out exportFlowAction, valueSettingStrategy);
							if (exportFlowAction == DataTableExporterAction.Continue) {
								valueSettingStrategy.Flush(dataRow);
								dataRow.EndEdit();
								dataTable.Rows.Add(dataRow);	
							}
							if (exportFlowAction == DataTableExporterAction.Stop) {
								dataRow.CancelEdit();   
								return;
							}
						}
						catch {
							dataRow.CancelEdit();
							throw;
						}
						finally {
							dataRow.EndEdit();
						}
					}
					nextRowIndex++;
				}
			}
			catch {
				throw;
			}
			finally {
				this.dataTable.EndLoadData(); 
				this.dataTable.AcceptChanges();
			}
		}
		DataTableExporterAction RaiseProcessEmptyRow(int index) {
			this.processEmptyRowArgs.Initialize(index);
			onProcessEmptyRow(this, processEmptyRowArgs);
			return processEmptyRowArgs.Action;
		}
		IEnumerable<Model.Row> GetRowsToExport(Model.Worksheet modelWorksheet, int topRowIndex, int bottomRow) {
			IOrderedEnumerator<Model.Row> existingRows = modelWorksheet.Rows.GetExistingRowsEnumerator(topRowIndex, bottomRow, false);
			if (Options.SkipEmptyRows && !Options.ConvertEmptyCells)
				return new Enumerable<Model.Row>(existingRows);
			if (Options.ConvertEmptyCells) {
				DevExpress.XtraSpreadsheet.Model.ContinuousRowsEnumerator fakeRows = 
					new DevExpress.XtraSpreadsheet.Model.ContinuousRowsEnumerator(modelWorksheet, topRowIndex, bottomRow, false, null);
				JoinedOrderedEnumerator<Model.Row> joinded = new JoinedOrderedEnumerator<Model.Row>(existingRows, fakeRows);
				return new Enumerable<Model.Row>(joinded);
			}
			return new Enumerable<Model.Row>(existingRows);
		}
		IDataValueSetter CreateValueSettingStrategy(int columnCount, DataTableExporterOptimizationType dataTableExporterOptimizationType) {
			if (dataTableExporterOptimizationType == DataTableExporterOptimizationType.Memory)
				return new DataValueSetterToDataRow();
			return new DataValueSetterToValueArray(columnCount);
		}
		bool[] CreateUseCustomConverterForColumnFlags(DataColumnCollection columns, Dictionary<string, ICellValueToColumnTypeConverter> dictionary) {
			bool[] result = new bool[columns.Count];
			if (dictionary.Count > result.Length) {
				string str = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorNoColumnsInDataTable);
				throw new Exception(str);
			}
			foreach (KeyValuePair<string, ICellValueToColumnTypeConverter> item in dictionary) {
				int columnIndex = DataTable.Columns.IndexOf(item.Key);
				if (columnIndex < 0) {
					string str = String.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorNoColumnInDataTable), item.Key);
					throw new Exception(str);
				}
				result[columnIndex] = true;
			}
			return result;
		}
		void ExportWorksheetRow(Model.Row modelRow, DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeWorksheet nativeWorksheet, out DataTableExporterAction exporterAction, IDataValueSetter setter) {
			exporterAction = args.Action = DataTableExporterAction.Continue;
			Model.Worksheet modelWorksheet = nativeWorksheet.ModelWorksheet;
			IEnumerable<Model.ICell> cells = GetCellsInRowEnumerable(modelWorksheet, modelRow);
			IOrderedEnumerator<Model.ICell> orderedEnumerator = (cells.GetEnumerator() as IOrderedEnumerator<Model.ICell>);
			bool nonEmptyCellExists = false;
			foreach (Model.ICell modelCell in cells) {
				int cellColumnIndex = (modelCell == null) ? orderedEnumerator.CurrentValueOrder : modelCell.ColumnIndex;
				int indexInDataRange = cellColumnIndex - Range.LeftColumnIndex;
				int indexInMapping = indexInDataRange; 
				DataColumn dataTableColumn = DataTable.Columns[indexInMapping]; 
				ICellValueToColumnTypeConverter converter = GetConverter(dataTableColumn.DataType, dataTableColumn.ColumnName, indexInMapping);
				Model.VariantValue modelValue = modelCell != null ? modelCell.Value : Model.VariantValue.Missing;
				bool modelCellHasValue = modelCell != null && (modelCell.HasFormula || modelValue != Model.VariantValue.Empty);
				if (modelCellHasValue || Options.ConvertEmptyCells) {
					CellValue cellValue = CellValue.Empty;
					if (modelCellHasValue) {
						cachedNativeCell.SubstituteModelCell(modelCell);
						if (modelValue.IsError)
							cellValue = CellValue.GetNativeErrorValue(modelValue.ErrorValue.Type);
						else {
							bool isDateTime = false; 
							cachedCellValue.ChangeModelValue(modelValue, isDateTime);
							cellValue = cachedCellValue;
						}
					}
					else if (Options.ConvertEmptyCells) {
						int columnIndex = orderedEnumerator.CurrentValueOrder;
						Model.CellKey keyForEmptyCell = new Model.CellKey(modelWorksheet.SheetId, columnIndex, modelRow.Index);
						emptyCellContentForEmptyCells.SubstituteModelCell(keyForEmptyCell);
						cachedNativeCell.SubstituteModelCell(keyForEmptyCell, emptyCellContentForEmptyCells);
					}
					nonEmptyCellExists = true;
					object convertedValue;
					ConversionResult convertationResult = converter.Convert(cachedNativeCell, cellValue, dataTableColumn.DataType, out convertedValue);
					if (convertationResult == ConversionResult.Success)
						setter[indexInMapping] = convertedValue; 
					else if (onConversionError != null) {
						this.args.Initialize(cachedNativeCell, cellValue, dataTableColumn, convertationResult);
						onConversionError(this, args);
						if (args.Action != DataTableExporterAction.Continue)
							break;
						object resolvedValue = args.DataTableValue;
						if (resolvedValue != DBNull.Value && resolvedValue != null)
							setter[indexInMapping] = args.DataTableValue;
					}
					else {
						if (convertationResult == ConversionResult.Overflow) {
							string overflowText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorDataTableExporterOverflow);
							throw new OverflowException(overflowText);
						}
						else if (convertationResult == ConversionResult.Error) {
							string errorText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorDataTableExporterConversionError);
							throw new Exception(errorText);
						}
					}
				}
			}
			exporterAction = args.Action;
			if (!nonEmptyCellExists) {
				if (Options.SkipEmptyRows)
					exporterAction = DataTableExporterAction.SkipRow;
				else if (onProcessEmptyRow != null)
					exporterAction = RaiseProcessEmptyRow(modelRow.Index);
			}
		}
		IEnumerable<Model.ICell> GetCellsInRowEnumerable(Model.Worksheet modelWorksheet, Model.Row modelRow) {
			int near = Range.LeftColumnIndex;
			int far = Range.RightColumnIndex;
			if (Options.ConvertEmptyCells)
				return modelRow.Cells.GetAllCellsWithFakeCellsAsNullInRow(modelRow, near, far);
			return new Enumerable<Model.ICell>(modelRow.Cells.GetExistingCellsEnumerator(near, far, false));
		}
		public virtual ICellValueToColumnTypeConverter GetConverter(Type targetType, string columnName, int columnIndexInDataTable) {
			if (useCustomConverterForColumnFlag[columnIndexInDataTable])
				return Options.CustomConverters[columnName];
			if (targetType == typeof(string))
				return Options.DefaultCellValueToStringConverter;
			return Options.DefaultCellValueToColumnTypeConverter;
		}
		#region IDataValueSetter
		interface IDataValueSetter {
			object this[int index] { set; }
			void BeforeExportNewRow(DataRow dataRow);
			void Flush(DataRow dataRow);
		}
		class DataValueSetterToDataRow : IDataValueSetter {
			DataRow DataRow { get; set; }
			public object this[int columnIndex] {
				set {
					if (value == null)
						value = DBNull.Value;
					DataRow[columnIndex] = value; 
				}
			}
			public void BeforeExportNewRow(DataRow dataRow) {
				this.DataRow = dataRow;
			}
			public void Flush(DataRow dataRow) {
				this.DataRow = null;
			}
		}
		class DataValueSetterToValueArray : IDataValueSetter {
			readonly object[] valueArray;
			readonly int columnCount;
			internal DataValueSetterToValueArray(int columnCount) {
				this.columnCount = columnCount;
				this.valueArray = new object[columnCount];
			}
			public object this[int columnIndex] {
				set { this.valueArray[columnIndex] = value; }
			}
			public void BeforeExportNewRow(DataRow dataRow) {
				Array.Clear(this.valueArray, 0, columnCount);
			}
			public void Flush(DataRow dataRow) {
				dataRow.ItemArray = this.valueArray;
			}
		} 
		#endregion
	}
	public delegate void CellValueConversionErrorEventHandler(Object sender, CellValueConversionErrorEventArgs e);
	public delegate void ProcessEmptyRowEventHandler(Object sender, ProcessEmptyRowEventArgs e);
	#region ProcessEmptyRowEventArgs
	public class ProcessEmptyRowEventArgs : EventArgs {
		int rowIndex = Int32.MinValue;
		DataTableExporterAction action = DataTableExporterAction.Continue;
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ProcessEmptyRowEventArgsAction")]
#endif
		public DataTableExporterAction Action { get { return action; } set { action = value; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ProcessEmptyRowEventArgsRowIndex")]
#endif
		public int RowIndex { get { return rowIndex; } }
		public void Initialize(int rowIndex) {
			this.rowIndex = rowIndex;
			this.action = DataTableExporterAction.Continue;
		}
	}
	#endregion
	#region CellValueConversionErrorEventArgs
	public class CellValueConversionErrorEventArgs : EventArgs {
		#region Fields
		Cell cell;
		CellValue cellValue;
		DataColumn dataColumn;
		ConversionResult conversionResult;
		#endregion
		#region Properties
		public Cell Cell { get { return cell; } }
		public CellValue CellValue { get { return cellValue; } }
		public DataColumn DataColumn { get { return dataColumn; } }
		public ConversionResult ConversionResult { get { return conversionResult; } }
		public object DataTableValue { get; set; }
		public DataTableExporterAction Action { get; set; }
		#endregion
		public void Initialize(Cell cell, CellValue value, DataColumn dataColumn, ConversionResult prevResult) {
			this.cell = cell;
			this.cellValue = value;
			this.dataColumn = dataColumn;
			this.conversionResult = prevResult;
			DataTableValue = DBNull.Value;
			Action = DataTableExporterAction.Continue;
		}
	} 
	#endregion
	public class DataTableExportOptions {
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("DataTableExportOptionsDefaultCellValueToStringConverter")]
#endif
		public CellValueToStringConverter DefaultCellValueToStringConverter { get; set; }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("DataTableExportOptionsDefaultCellValueToColumnTypeConverter")]
#endif
		public CellValueToColumnTypeConverter DefaultCellValueToColumnTypeConverter { get; set; }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("DataTableExportOptionsCustomConverters")]
#endif
		public Dictionary<string, ICellValueToColumnTypeConverter> CustomConverters { get; set; }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("DataTableExportOptionsSkipEmptyRows")]
#endif
		public bool SkipEmptyRows { get; set; }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("DataTableExportOptionsConvertEmptyCells")]
#endif
		public bool ConvertEmptyCells { get; set; }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("DataTableExportOptionsOptimizationType")]
#endif
		public DataTableExporterOptimizationType OptimizationType { get;set;}
	}
}
#endif
