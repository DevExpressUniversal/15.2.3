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
using System.IO;
using System.Text;
using DevExpress.Office.Services;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Export.Xls {
	public class XlsCellsExporter : XlsWorksheetExporterBase {
		#region Fields
		const int rowsInBlock = 32;
		List<Row> rowsToExport;
		List<ICell> cellsToExport;
		List<long> dbCellsPositions;
		XlsDbCellCalculator dbCellCalculator;
		Dictionary<SharedFormula, CellPosition> exportedSharedFormulas;
		bool calculated = false;
		CellRange dimensionsRange;
		CellRange customRowsRange;
		int rowsMaxOutlineLevel;
		#endregion
		public XlsCellsExporter(BinaryWriter streamWriter, DocumentModel documentModel, ExportXlsStyleSheet exportStyleSheet, Worksheet sheet)
			: base(streamWriter, documentModel, exportStyleSheet, sheet) {
			this.rowsToExport = new List<Row>();
			this.cellsToExport = new List<ICell>();
			this.dbCellsPositions = new List<long>();
			this.dbCellCalculator = new XlsDbCellCalculator();
			this.exportedSharedFormulas = new Dictionary<SharedFormula, CellPosition>(sheet.SharedFormulas.Count);
		}
		#region Properties
		public List<long> DbCellsPositions { get { return dbCellsPositions; } }
		public Dictionary<SharedFormula, CellPosition> ExportedSharedFormulas { get { return exportedSharedFormulas; } }
		public CellRange DimensionsRange {
			get {
				if (!calculated) {
					Calculate();
					calculated = true;
				}
				return dimensionsRange;
			}
		}
		public CellRange CustomRowsRange {
			get {
				if (!calculated) {
					Calculate();
					calculated = true;
				}
				return customRowsRange;
			}
		}
		public int RowsMaxOutlineLevel {
			get {
				if (!calculated) {
					Calculate();
					calculated = true;
				}
				return rowsMaxOutlineLevel;
			}
		}
		#endregion
		void Calculate() {
			int minRowIndex = XlsDefs.MaxRowCount;
			int maxRowIndex = 0;
			int minColIndex = XlsDefs.MaxColumnCount;
			int maxColIndex = 0;
			rowsMaxOutlineLevel = 0;
			foreach (Row row in Sheet.Rows.GetExistingRows(0, XlsDefs.MaxRowCount, false)) {
				int firstColumnIndex = row.FirstColumnIndex;
				if (firstColumnIndex < XlsDefs.MaxColumnCount) {
					if (row.OutlineLevel > rowsMaxOutlineLevel)
						rowsMaxOutlineLevel = row.OutlineLevel;
				}
				if (SkipRow(row))
					continue;
				if (row.Index < minRowIndex)
					minRowIndex = row.Index;
				if (row.Index > maxRowIndex)
					maxRowIndex = row.Index;
				if (firstColumnIndex != -1 && firstColumnIndex < minColIndex)
					minColIndex = firstColumnIndex;
				int lastColumnIndex = row.LastColumnIndex;
				if (lastColumnIndex != -1) {
					if (lastColumnIndex >= XlsDefs.MaxColumnCount)
						lastColumnIndex = XlsDefs.MaxColumnCount - 1;
					if (lastColumnIndex > maxColIndex)
						maxColIndex = lastColumnIndex;
				}
			}
			if (maxRowIndex < minRowIndex)
				customRowsRange = null;
			else
				customRowsRange = new CellRange(Sheet, new CellPosition(0, minRowIndex), new CellPosition(0, maxRowIndex));
			foreach (Comment comment in Sheet.Comments) {
				if (comment.Reference.OutOfLimits()) continue;
				if (comment.Reference.Row < minRowIndex)
					minRowIndex = comment.Reference.Row;
				if (comment.Reference.Row > maxRowIndex)
					maxRowIndex = comment.Reference.Row;
				if (comment.Reference.Column < minColIndex)
					minColIndex = comment.Reference.Column;
				if (comment.Reference.Column > maxColIndex)
					maxColIndex = comment.Reference.Column;
			}
			if (maxRowIndex < minRowIndex)
				dimensionsRange = null;
			else {
				if (maxColIndex < minColIndex)
					minColIndex = 0;
				dimensionsRange = new CellRange(Sheet, new CellPosition(minColIndex, minRowIndex), new CellPosition(maxColIndex, maxRowIndex));
			}
		}
		public int GetNumberOfRowBlocks() {
			int result = 0;
			CellRange rowsRange = CustomRowsRange;
			if (rowsRange != null) {
				int numberOfRows = (rowsRange.BottomRight.Row - rowsRange.TopLeft.Row) + 1;
				if (numberOfRows % rowsInBlock == 0)
					result = numberOfRows / rowsInBlock;
				else
					result = numberOfRows / rowsInBlock + 1;
			}
			return result;
		}
		public override void WriteContent() {
			CellRange rowsRange = CustomRowsRange;
			if (rowsRange == null)
				return; 
			this.rowsToExport.Clear();
			this.dbCellsPositions.Clear();
			this.dbCellCalculator.Reset();
			int minRowIndex = rowsRange.TopLeft.Row;
			int maxRowIndex = rowsRange.BottomRight.Row + 1;
			int rowCount = 0;
			for (int i = minRowIndex; i < maxRowIndex; i++) {
				Row row = Sheet.Rows.TryGetRow(i);
				if (!SkipRow(row)) {
					rowsToExport.Add(row);
					WriteRow(row);
				}
				else if (OutOfLimits(row)) {
					string message = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_RowFirstColumnOutOfXLSRange), row.Index + 1);
					LogMessage(LogCategory.Warning, message);
				}
				rowCount++;
				if (rowCount >= rowsInBlock) {
					WriteRowsContent();
					rowCount = 0;
				}
			}
			if (this.rowsToExport.Count > 0) {
				WriteRowsContent();
			}
		}
		#region IDisposable Members
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing) {
				if (this.rowsToExport != null) {
					this.rowsToExport.Clear();
					this.rowsToExport = null;
				}
				if (this.cellsToExport != null) {
					this.cellsToExport.Clear();
					this.cellsToExport = null;
				}
				this.dbCellsPositions = null;
				this.dbCellCalculator = null;
			}
		}
		#endregion
		bool SkipRow(Row row) {
			if (row == null)
				return true;
			return row.IsDefault() || row.OutOfXlsLimits();
		}
		bool OutOfLimits(Row row) {
			if (row == null)
				return false;
			return row.OutOfXlsLimits();
		}
		void WriteRow(Row row) {
			this.dbCellCalculator.RegisterRowPosition(GetCurrentPosition());
			XlsCommandRow command = new XlsCommandRow();
			if (row.CellsCount == 0) {
				command.FirstColumnIndex = 0;
				command.LastColumnIndex = 0;
			}
			else {
				command.FirstColumnIndex = row.FirstColumnIndex;
				int lastColumnIndex = row.LastColumnIndex + 1;
				if (lastColumnIndex > XlsDefs.MaxColumnCount) {
					lastColumnIndex = XlsDefs.MaxColumnCount;
					string message = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_RowLastColumnOutOfXLSRange), row.Index + 1);
					LogMessage(LogCategory.Warning, message);
				}
				command.LastColumnIndex = lastColumnIndex;
			}
			command.HasFormatting = row.ApplyStyle;
			command.HeightInTwips = ToTwips(RowExportHelper.GetRowHeight(row, Sheet));
			if (command.HeightInTwips == 0)
				command.HeightInTwips = XlsDefs.DefaultRowHeightInTwips;
			command.Index = row.Index;
			command.IsCollapsed = row.IsCollapsed;
			command.IsCustomHeight = RowExportHelper.GetIsCustomHeight(row, Sheet);
			command.IsHidden = row.IsHidden;
			command.OutlineLevel = row.OutlineLevel;
			command.HasThickBorder = row.IsThickTopBorder;
			command.HasMediumBorder = row.IsThickBottomBorder;
			command.FormatIndex = ExportStyleSheet.GetXFIndex(row.FormatIndex);
			command.Write(StreamWriter);
		}
		void WriteRowsContent() {
			int count = this.rowsToExport.Count;
			for (int i = 0; i < count; i++) {
				this.dbCellCalculator.RegisterFirstCellPosition(GetCurrentPosition());
				WriteRowCells(this.rowsToExport[i]);
			}
			WriteDbCell();
			this.rowsToExport.Clear();
			this.dbCellCalculator.Reset();
		}
		void WriteDbCell() {
			long position = GetCurrentPosition();
			this.dbCellsPositions.Add(position);
			this.dbCellCalculator.RegisterDbCellPosition(position);
			XlsCommandDbCell command = new XlsCommandDbCell();
			command.FirstRowOffset = this.dbCellCalculator.CalculateFirstRowOffset();
			command.StreamOffsets.AddRange(this.dbCellCalculator.CalculateStreamOffsets());
			command.Write(StreamWriter);
		}
		void WriteRowCells(Row row) {
			foreach (ICell cell in row.Cells) {
				if (cell.ColumnIndex >= XlsDefs.MaxColumnCount)
					break;
				if (NeedToFlushCells(cell))
					FlushCells();
				this.cellsToExport.Add(cell);
			}
			if (this.cellsToExport.Count > 0)
				FlushCells();
		}
		bool NeedToFlushCells(ICell cell) {
			int count = this.cellsToExport.Count;
			if (count == 0)
				return false;
			if (cell.HasFormula)
				return true;
			ICell lastCell = this.cellsToExport[count - 1];
			if (lastCell.HasFormula)
				return true;
			if ((cell.ColumnIndex - lastCell.ColumnIndex) > 1) 
				return true;
			VariantValueType cellType = cell.Value.Type;
			if (cellType != VariantValueType.None && cellType != VariantValueType.Numeric)
				return true;
			if (lastCell.Value.Type != cellType)
				return true;
			if ((lastCell.Value.Type == VariantValueType.Numeric) && !RkNumber.IsRkValue(lastCell.Value.NumericValue)) 
				return true;
			if ((cellType == VariantValueType.Numeric) && !RkNumber.IsRkValue(cell.Value.NumericValue)) 
				return true;
			return (cellType == VariantValueType.None && cell.FormatIndex == 0);
		}
		void FlushCells() {
			ICell cell = this.cellsToExport[0];
			if (cell.HasFormula) {
				WriteFormulaCell();
			}
			else {
				switch (cell.Value.Type) {
					case VariantValueType.None:
						WriteBlankCells();
						break;
					case VariantValueType.Numeric:
						WriteNumericCells();
						break;
					case VariantValueType.Boolean:
						WriteBooleanCell();
						break;
					case VariantValueType.Error:
						WriteErrorCell();
						break;
					case VariantValueType.SharedString:
						WriteSharedStringCell();
						break;
#if ((DEBUG || DEBUGTEST) && !DATA_SHEET)
					case VariantValueType.InlineText:
						throw new Exception("Inline text valued cell");
#endif
#if DATA_SHEET
					case VariantValueType.InlineText:
						WriteInlineStringCell();
						break;
#endif
				}
			}
			this.cellsToExport.Clear();
		}
		void WriteBlankCells() {
			ICell cell = this.cellsToExport[0];
			int count = this.cellsToExport.Count;
			if (count > 1) {
				XlsCommandMulBlank command = new XlsCommandMulBlank();
				command.RowIndex = cell.RowIndex;
				command.FirstColumnIndex = cell.ColumnIndex;
				for (int i = 0; i < count; i++) {
					int formatIndex = ExportStyleSheet.GetXFIndex(this.cellsToExport[i].FormatIndex);
					command.FormatIndices.Add(formatIndex);
				}
				command.Write(StreamWriter);
			}
			else {
				XlsCommandBlank command = new XlsCommandBlank();
				InitializeCommand(command, cell);
				command.Write(StreamWriter);
			}
		}
		void WriteNumericCells() {
			ICell cell = this.cellsToExport[0];
			int count = this.cellsToExport.Count;
			if (count > 1) {
				XlsCommandMulRk command = new XlsCommandMulRk();
				command.RowIndex = cell.RowIndex;
				command.FirstColumnIndex = cell.ColumnIndex;
				for (int i = 0; i < count; i++) {
					cell = this.cellsToExport[i];
					XlsRkRec item = new XlsRkRec();
					item.FormatIndex = ExportStyleSheet.GetXFIndex(cell.FormatIndex);
					item.Rk.Value = cell.Value.NumericValue;
					command.RkRecords.Add(item);
				}
				command.Write(StreamWriter);
			}
			else if (RkNumber.IsRkValue(cell.Value.NumericValue)) {
				XlsCommandRk command = new XlsCommandRk();
				InitializeCommand(command, cell);
				command.Value = (double)cell.Value.NumericValue;
				command.Write(StreamWriter);
			}
			else {
				XlsCommandNumber command = new XlsCommandNumber();
				InitializeCommand(command, cell);
				command.Value = (double)cell.Value.NumericValue;
				command.Write(StreamWriter);
			}
		}
		void WriteBooleanCell() {
			ICell cell = this.cellsToExport[0];
			XlsCommandBoolErr command = new XlsCommandBoolErr();
			InitializeCommand(command, cell);
			command.BoolValue = cell.Value.BooleanValue;
			command.Write(StreamWriter);
		}
		void WriteErrorCell() {
			ICell cell = this.cellsToExport[0];
			XlsCommandBoolErr command = new XlsCommandBoolErr();
			InitializeCommand(command, cell);
			command.ErrorValue = cell.Value.ErrorValue.Value;
			command.Write(StreamWriter);
		}
		void WriteSharedStringCell() {
			ICell cell = this.cellsToExport[0];
			XlsCommandLabelSst command = new XlsCommandLabelSst();
			InitializeCommand(command, cell);
			int sstIndex = cell.Value.SharedStringIndexValue.ToInt();
			command.StringIndex = ExportStyleSheet.SharedStringsTable[sstIndex];
			command.Write(StreamWriter);
		}
#if DATA_SHEET
		void WriteInlineStringCell() {
			ICell cell = this.cellsToExport[0];
			XlsCommandLabel command = new XlsCommandLabel();
			InitializeCommand(command, cell);
			string text = cell.Value.InlineTextValue;
			if (!string.IsNullOrEmpty(text) && text.Length > 255)
				text = text.Substring(0, 255);
			command.Value = text;
			command.Write(StreamWriter);
		}
#endif
		void WriteFormulaCell() {
			ICell cell = this.cellsToExport[0];
			XlsCommandFormula command = new XlsCommandFormula();
			InitializeCommand(command, cell);
			SetFormulaValue(command, cell);
			FormulaBase formula = ReplaceCustomFunctions(cell);
			SetFormulaExpression(command, cell, formula);
			command.Write(StreamWriter);
			if (command.PartOfArrayFormula)
				WriteArrayFormula(cell, formula);
			if (command.PartOfSharedFormula)
				WriteSharedFormula(cell, formula);
			if (command.Value.IsString)
				WriteStringFormulaValue(cell);
		}
		FormulaBase ReplaceCustomFunctions(ICell cell) {
			if (DocumentModel.DocumentExportOptions.CustomFunctionExportMode == CustomFunctionExportMode.CalculatedValue && cell.FormulaType != FormulaType.ArrayPart)
				return cell.GetFormulaWithoutCustomFunctions(false);
			return cell.Formula;
		}
		#region Utils
		long GetCurrentPosition() {
			return StreamWriter.BaseStream.Position;
		}
		protected int ToTwips(float value) {
			return (int)DocumentModel.UnitConverter.ModelUnitsToTwipsF(value);
		}
		void InitializeCommand(XlsCommandCellBase command, ICell cell) {
			command.RowIndex = cell.RowIndex;
			command.ColumnIndex = cell.ColumnIndex;
			command.FormatIndex = ExportStyleSheet.GetXFIndex(cell.FormatIndex);
		}
		void SetFormulaValue(XlsCommandFormula command, ICell cell) {
			switch (cell.Value.Type) {
				case VariantValueType.None:
					command.Value.IsBlankString = true;
					break;
				case VariantValueType.Numeric:
					command.Value.NumericValue = cell.Value.NumericValue;
					break;
				case VariantValueType.Boolean:
					command.Value.BooleanValue = cell.Value.BooleanValue;
					break;
				case VariantValueType.Error:
					command.Value.SetErrorValue(cell.Value.ErrorValue.Value);
					break;
				case VariantValueType.InlineText:
				case VariantValueType.SharedString:
					if (string.IsNullOrEmpty(GetStringValue(cell)))
						command.Value.IsBlankString = true;
					else
						command.Value.IsString = true;
					break;
			}
		}
		void SetFormulaExpression(XlsCommandFormula command, ICell cell, FormulaBase cellFormula) {
			ExportStyleSheet.RPNContext.WorkbookContext.PushCurrentCell(cell);
			ExportStyleSheet.RPNContext.PushCurrentSubject(cell.GetDescription());
			try {
				ArrayFormula arrayFormula = cellFormula as ArrayFormula;
				ArrayFormulaPart arrayFormulaPart = cellFormula as ArrayFormulaPart;
				SharedFormulaRef sharedFormulaRef = cellFormula as SharedFormulaRef;
				ParsedExpression expression = new ParsedExpression();
				if (arrayFormula != null) {
					ParsedThingExp ptg = new ParsedThingExp();
					ptg.Position = new CellPosition(cell.ColumnIndex, cell.RowIndex);
					expression.Add(ptg);
				}
				else if (arrayFormulaPart != null) {
					ParsedThingExp ptg = new ParsedThingExp();
					ptg.Position = new CellPosition(arrayFormulaPart.TopLeftCell.ColumnIndex, arrayFormulaPart.TopLeftCell.RowIndex);
					expression.Add(ptg);
				}
				else if (sharedFormulaRef != null) {
					SharedFormula hostSharedFormula = sharedFormulaRef.HostSharedFormula;
					ParsedExpression rpn = hostSharedFormula.Expression;
					if (rpn.IsXlsSharedFormulaCompliant()) {
						ParsedThingExp ptg = new ParsedThingExp();
						CellPosition hostCellPosition;
						if (ExportedSharedFormulas.TryGetValue(hostSharedFormula, out hostCellPosition))
							ptg.Position = ExportedSharedFormulas[hostSharedFormula];
						else
							ptg.Position = cell.Position;
						expression.Add(ptg);
						command.PartOfSharedFormula = true;
					}
					else {
						expression = sharedFormulaRef.GetNormalCellFormula(cell);
						expression = XlsParsedThingConverter.ToXlsExpression(expression, ExportStyleSheet.RPNContext);
					}
				}
				else {
					expression = cellFormula.Expression;
					expression = XlsParsedThingConverter.ToXlsExpression(expression, ExportStyleSheet.RPNContext);
				}
				command.SetParsedExpression(expression, ExportStyleSheet.RPNContext);
				command.AlwaysCalc = cellFormula.CalculateAlways || cellFormula.IsVolatile();
			}
			finally {
				ExportStyleSheet.RPNContext.WorkbookContext.PopCurrentCell();
				ExportStyleSheet.RPNContext.PopCurrentSubject();
			}
		}
		void WriteArrayFormula(ICell cell, FormulaBase formula) {
			ArrayFormula arrayFormula = formula as ArrayFormula;
			if (arrayFormula != null) {
				CellRange xlsMaxRange = new CellRange(cell.Sheet, new CellPosition(0, 0), new CellPosition(XlsDefs.MaxColumnCount - 1, XlsDefs.MaxRowCount - 1));
				VariantValue rangeIntersection = arrayFormula.Range.IntersectionWith(xlsMaxRange);
				if (rangeIntersection.Type != VariantValueType.CellRange)
					return;
				CellRangeBase range = rangeIntersection.CellRangeValue;
				XlsCommandArrayFormula command = new XlsCommandArrayFormula();
				command.Range = new CellRangeInfo(range.TopLeft, range.BottomRight);
				ExportStyleSheet.RPNContext.WorkbookContext.PushCurrentCell(cell);
				ExportStyleSheet.RPNContext.WorkbookContext.PushArrayFormulaProcessing(true);
				ExportStyleSheet.RPNContext.PushCurrentSubject(cell.GetDescription(arrayFormula));
				try {
					ParsedExpression expression = arrayFormula.Expression;
					expression = XlsParsedThingConverter.ToXlsExpression(expression, ExportStyleSheet.RPNContext);
					command.SetParsedExpression(expression, ExportStyleSheet.RPNContext);
				}
				finally {
					ExportStyleSheet.RPNContext.WorkbookContext.PopArrayFormulaProcessing();
					ExportStyleSheet.RPNContext.WorkbookContext.PopCurrentCell();
					ExportStyleSheet.RPNContext.PopCurrentSubject();
				}
				command.Write(StreamWriter);
			}
		}
		void WriteSharedFormula(ICell cell, FormulaBase formula) {
			SharedFormulaRef sharedFormulaRef = formula as SharedFormulaRef;
			if (sharedFormulaRef == null)
				return;
			SharedFormula sharedFormula = sharedFormulaRef.HostSharedFormula;
			if (!ExportedSharedFormulas.ContainsKey(sharedFormula)) {
				CellRange xlsMaxRange = new CellRange(cell.Sheet, new CellPosition(0, 0), new CellPosition(XlsDefs.MaxColumnCount - 1, XlsDefs.MaxRowCount - 1));
				VariantValue rangeIntersection = sharedFormula.Range.IntersectionWith(xlsMaxRange);
				if (rangeIntersection.Type != VariantValueType.CellRange)
					return;
				ExportedSharedFormulas.Add(sharedFormula, cell.Position);
				CellRangeBase range = rangeIntersection.CellRangeValue;
				XlsCommandSharedFormula command = new XlsCommandSharedFormula();
				command.Range = new CellRangeInfo(range.TopLeft, range.BottomRight);
				ExportStyleSheet.RPNContext.WorkbookContext.PushCurrentCell(cell);
				ExportStyleSheet.RPNContext.WorkbookContext.PushSharedFormulaProcessing(true);
				ExportStyleSheet.RPNContext.PushCurrentSubject(cell.GetDescription(sharedFormula));
				try {
					ParsedExpression expression = sharedFormula.Expression;
					expression = XlsParsedThingConverter.ToXlsExpression(expression, ExportStyleSheet.RPNContext);
					command.SetParsedExpression(expression, ExportStyleSheet.RPNContext);
				}
				finally {
					ExportStyleSheet.RPNContext.WorkbookContext.PopSharedFormulaProcessing();
					ExportStyleSheet.RPNContext.WorkbookContext.PopCurrentCell();
					ExportStyleSheet.RPNContext.PopCurrentSubject();
				}
				command.UseCount = CalcUseCount(sharedFormula, range);
				command.Write(StreamWriter);
			}
		}
		int CalcUseCount(SharedFormula formula, CellRangeBase range) {
			int result = 0;
			foreach (ICellBase info in range.GetExistingCellsEnumerable()) {
				ICell cell = info as ICell;
				if (cell == null || !cell.HasFormula)
					continue;
				SharedFormulaRef sharedFormulaRef = cell.GetFormula() as SharedFormulaRef;
				if (sharedFormulaRef == null)
					continue;
				if (object.ReferenceEquals(formula, sharedFormulaRef.HostSharedFormula))
					result++;
				if (result == byte.MaxValue)
					break;
			}
			return result;
		}
		void WriteStringFormulaValue(ICell cell) {
			string value = GetStringValue(cell);
			if (string.IsNullOrEmpty(value))
				return;
			XlsCommandString stringCommand = new XlsCommandString();
			XlsCommandContinue continueCommand = new XlsCommandContinue();
			using (XlsChunkWriter writer = new XlsChunkWriter(StreamWriter, stringCommand, continueCommand)) {
				XLUnicodeString str = new XLUnicodeString();
				str.Value = value;
				str.Write(writer);
			}
		}
		string GetStringValue(ICell cell) {
			if (cell.Value.Type == VariantValueType.InlineText)
				return cell.Value.InlineTextValue;
			if (cell.Value.Type == VariantValueType.SharedString)
				return cell.Value.GetTextValue(cell.Sheet.Workbook.SharedStringTable);
			return string.Empty;
		}
		#endregion
	}
}
