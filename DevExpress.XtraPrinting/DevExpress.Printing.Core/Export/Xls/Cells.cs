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
using System.IO;
using System.Text;
using DevExpress.Utils;
using DevExpress.Export.Xl;
namespace DevExpress.XtraExport.Xls {
	using DevExpress.XtraExport.Implementation;
	public partial class XlsDataAwareExporter {
		readonly List<XlCell> cellsToExport = new List<XlCell>();
		XlCellFormatting inheritedFormatting;
		XlCell currentCell = null;
		XlPtgDataType paramType = XlPtgDataType.Value;
		public IXlCell BeginCell() {
			currentCell = new XlCell();
			currentCell.RowIndex = CurrentRowIndex;
			currentCell.ColumnIndex = currentColumnIndex;
			IXlColumn column;
			if (currentSheet.ColumnsTable.TryGetValue(currentColumnIndex, out column))
				inheritedFormatting = XlCellFormatting.Merge(XlFormatting.CopyObject(column.Formatting), currentRow.Formatting);
			else
				inheritedFormatting = XlFormatting.CopyObject(currentRow.Formatting);
			currentCell.Formatting = XlFormatting.CopyObject(inheritedFormatting);
			return currentCell;
		}
		public void EndCell() {
			if (currentCell == null)
				throw new InvalidOperationException("BeginCell/EndCell calls consistency.");
			if (currentCell.ColumnIndex >= options.MaxColumnCount)
				throw new ArgumentOutOfRangeException(string.Format("Cell column index out of range 0..{0}.", options.MaxColumnCount - 1));
			if (currentCell.ColumnIndex < currentColumnIndex)
				throw new InvalidOperationException("Cell column index consistency.");
			if (!currentCell.Value.IsEmpty || currentCell.HasFormula || !XlCellFormatting.Equals(inheritedFormatting, currentCell.Formatting)) {
				if (currentCell.Value.IsNumeric && XNumChecker.IsNegativeZero(currentCell.Value.NumericValue))
					currentCell.Value = 0.0;
				currentSheet.RegisterCellPosition(currentCell);
				if (currentRow.Cells.Count == 0)
					currentRow.FirstColumnIndex = currentCell.ColumnIndex;
				currentRow.LastColumnIndex = currentCell.ColumnIndex;
				currentRow.Cells.Add(currentCell);
				if (!XlCellFormatting.EqualFonts(inheritedFormatting, currentCell.Formatting))
					CalculateAutomaticHeight(currentCell.Formatting);
			}
			currentColumnIndex = currentCell.ColumnIndex + 1;
			currentCell = null;
		}
		public void SkipCells(int count) {
			Guard.ArgumentPositive(count, "count");
			if(currentCell != null)
				throw new InvalidOperationException("Operation cannot be executed inside BeginCell/EndCell scope.");
			if((currentColumnIndex + count) >= options.MaxColumnCount)
				throw new ArgumentOutOfRangeException(string.Format("Cell column index goes beyond range 0..{0}.", options.MaxColumnCount - 1));
			currentColumnIndex += count;
		}
		#region IXlFormulaEngine Members
		IXlFormulaParameter IXlFormulaEngine.Param(XlVariantValue value) {
			return new XlFormulaParameter(value);
		}
		IXlFormulaParameter IXlFormulaEngine.Subtotal(XlCellRange range, XlSummary summary, bool ignoreHidden) {
			return XlSubtotalFunction.Create(range, summary, ignoreHidden);
		}
		IXlFormulaParameter IXlFormulaEngine.Subtotal(IList<XlCellRange> ranges, XlSummary summary, bool ignoreHidden) {
			return XlSubtotalFunction.Create(ranges, summary, ignoreHidden);
		}
		IXlFormulaParameter IXlFormulaEngine.VLookup(XlVariantValue lookupValue, XlCellRange table, int columnIndex, bool rangeLookup) {
			return XlVLookupFunction.Create(lookupValue, table, columnIndex, rangeLookup);
		}
		IXlFormulaParameter IXlFormulaEngine.VLookup(IXlFormulaParameter lookupValue, XlCellRange table, int columnIndex, bool rangeLookup) {
			return XlVLookupFunction.Create(lookupValue, table, columnIndex, rangeLookup);
		}
		IXlFormulaParameter IXlFormulaEngine.Text(XlVariantValue value, string netFormatString, bool isDateTimeFormatString) {
			return XlTextFunction.Create(value, netFormatString, isDateTimeFormatString);
		}
		IXlFormulaParameter IXlFormulaEngine.Text(IXlFormulaParameter formula, string netFormatString, bool isDateTimeFormatString) {
			return XlTextFunction.Create(formula, netFormatString, isDateTimeFormatString);
		}
		IXlFormulaParameter IXlFormulaEngine.Concatenate(params IXlFormulaParameter[] parameters) {
			return new XlConcatenateFunction(parameters);
		}
		#endregion
		void WriteRowCells(XlsTableRow row) {
			this.cellsToExport.Clear();
			foreach (XlCell cell in row.Cells) {
				if (NeedToFlushCells(cell))
					FlushCells();
				this.cellsToExport.Add(cell);
			}
			if (this.cellsToExport.Count > 0)
				FlushCells();
		}
		bool NeedToFlushCells(XlCell cell) {
			int count = this.cellsToExport.Count;
			if (count == 0)
				return false;
			if (cell.HasFormula)
				return true;
			XlCell lastCell = this.cellsToExport[count - 1];
			if (lastCell.HasFormula)
				return true;
			if ((cell.ColumnIndex - lastCell.ColumnIndex) > 1) 
				return true;
			XlVariantValueType cellType = cell.Value.Type;
			if (cellType != XlVariantValueType.None && cellType != XlVariantValueType.Numeric)
				return true;
			if (lastCell.Value.Type != cellType)
				return true;
			if ((lastCell.Value.Type == XlVariantValueType.Numeric) && !XlsRkNumber.IsRkValue(lastCell.Value.NumericValue)) 
				return true;
			if ((cellType == XlVariantValueType.Numeric) && !XlsRkNumber.IsRkValue(cell.Value.NumericValue)) 
				return true;
			return (cellType == XlVariantValueType.None && cell.Formatting == null);
		}
		void FlushCells() {
			XlCell cell = this.cellsToExport[0];
			if (cell.HasFormula) {
				WriteFormulaCell();
			}
			else {
				switch (cell.Value.Type) {
					case XlVariantValueType.None:
						WriteBlankCells();
						break;
					case XlVariantValueType.Numeric:
					case XlVariantValueType.DateTime:
						WriteNumericCells();
						break;
					case XlVariantValueType.Boolean:
						WriteBooleanCell();
						break;
					case XlVariantValueType.Text:
						WriteSharedStringCell();
						break;
					case XlVariantValueType.Error:
						WriteErrorCell();
						break;
				}
			}
			this.cellsToExport.Clear();
		}
		void WriteFormulaCell() {
			XlCell cell = this.cellsToExport[0];
			XlsContentFormula content = new XlsContentFormula();
			InitializeContent(content, cell);
			SetFormulaValue(content, cell);
			if (cell.SharedFormulaPosition.IsValid) {
				if (!sharedFormulaHostCells.Contains(cell.SharedFormulaPosition))
					throw new AggregateException(string.Format("Position {0} refers to non existing shared formula", cell.SharedFormulaPosition.ToString()));
				content.PartOfSharedFormula = true;
				content.FormulaBytes = GetSharedFormulaRefBytes(cell.SharedFormulaPosition);
			}
			else if (cell.SharedFormulaRange != null) {
				XlCellPosition hostCell = new XlCellPosition(cell.ColumnIndex, cell.RowIndex);
				sharedFormulaHostCells.Add(hostCell);
				content.PartOfSharedFormula = true;
				content.FormulaBytes = GetSharedFormulaRefBytes(hostCell);
			}
			else {
				content.FormulaBytes = GetFormulaBytes(cell);
			}
			WriteContent(XlsRecordType.Formula, content);
			WriteSharedFormula(cell);
			if (content.Value.IsString)
				WriteStringFormulaValue(cell);
		}
		void WriteSharedFormula(XlCell cell) {
			if (cell.SharedFormulaRange == null)
				return;
			XlsContentSharedFormula content = new XlsContentSharedFormula();
			XlsRefU range = XlsRefU.FromRange(cell.SharedFormulaRange);
			if (range == null)
				throw new ArgumentException(string.Format("Shared formula range {0} out of XLS worksheet limits", cell.SharedFormulaRange.ToString()));
			content.Range = range;
			content.UseCount = (byte)Math.Max(255, content.Range.CellCount);
			content.FormulaBytes = GetSharedFormulaBytes(cell);
			WriteContent(XlsRecordType.SharedFormula, content);
		}
		byte[] GetSharedFormulaRefBytes(XlCellPosition hostCell) {
			XlExpression expression = new XlExpression();
			expression.Add(new XlPtgExp(hostCell));
			return expression.GetBytes(this);
		}
		void SetFormulaValue(XlsContentFormula content, XlCell cell) {
			switch (cell.Value.Type) {
				case XlVariantValueType.None:
					content.Value.IsBlankString = true;
					break;
				case XlVariantValueType.Numeric:
				case XlVariantValueType.DateTime:
					content.Value.NumericValue = cell.Value.NumericValue;
					break;
				case XlVariantValueType.Boolean:
					content.Value.BooleanValue = cell.Value.BooleanValue;
					break;
				case XlVariantValueType.Text:
					if (string.IsNullOrEmpty(cell.Value.TextValue))
						content.Value.IsBlankString = true;
					else
						content.Value.IsString = true;
					break;
			}
		}
		void WriteStringFormulaValue(IXlCell cell) {
			string value = cell.Value.TextValue;
			if (string.IsNullOrEmpty(value))
				return;
			XlsChunk firstChunk = new XlsChunk(XlsRecordType.String);
			XlsChunk nextChunk = new XlsChunk(XlsRecordType.Continue);
			using (XlsChunkWriter chunkWriter = new XlsChunkWriter(writer, firstChunk, nextChunk)) {
				XLUnicodeString str = new XLUnicodeString();
				str.Value = value;
				str.Write(chunkWriter);
			}
		}
		byte[] GetFormulaBytes(XlCell cell) {
			if (cell == null || !cell.HasFormula)
				return null;
			XlExpression formula;
			if (cell.Formula != null) {
				formula = new XlExpression();
				PrepareExpression(formula, cell.Formula);
			}
			else if (cell.Expression != null)
				formula = cell.Expression.ToXlsExpression();
			else {
				if (formulaParser == null)
					return null;
				expressionContext.CurrentCell = new XlCellPosition(cell.ColumnIndex, cell.RowIndex);
				expressionContext.ReferenceMode = XlCellReferenceMode.Reference;
				expressionContext.ExpressionStyle = XlExpressionStyle.Normal;
				formula = formulaParser.Parse(cell.FormulaString, expressionContext);
				if (formula == null || formula.Count == 0)
					return null;
				formula = formula.ToXlsExpression();
			}
			byte[] result = formula.GetBytes(this);
			if (result.Length <= XlsDefs.MaxFormulaBytesSize)
				return result;
			formula.Clear();
			formula.Add(new XlPtgErr(XlCellErrorType.Value));
			return formula.GetBytes(this);
		}
		byte[] GetSharedFormulaBytes(XlCell cell) {
			XlExpression formula;
			if (cell.Expression != null)
				formula = cell.Expression.ToXlsExpression();
			else {
				if (formulaParser == null)
					throw new InvalidOperationException("Formula parser required for this operation.");
				expressionContext.CurrentCell = new XlCellPosition(cell.ColumnIndex, cell.RowIndex);
				expressionContext.ReferenceMode = XlCellReferenceMode.Offset;
				expressionContext.ExpressionStyle = XlExpressionStyle.Shared;
				formula = formulaParser.Parse(cell.FormulaString, expressionContext);
				if (formula == null || formula.Count == 0)
					throw new InvalidOperationException(string.Format("Can't parse shared formula '{0}'.", cell.FormulaString));
				formula = formula.ToXlsExpression();
			}
			byte[] result = formula.GetBytes(this);
			if(result.Length <= XlsDefs.MaxFormulaBytesSize)
				return result;
			formula.Clear();
			formula.Add(new XlPtgErr(XlCellErrorType.Value));
			return formula.GetBytes(this);
		}
		bool PrepareExpression(XlExpression expression, XlSubtotalFunction function) {
			if(function == null)
				return false;
			int code = (int)function.Summary;
			if(function.IgnoreHidden)
				code += 100;
			int subtotalCount = 0;
			int count = 0;
			foreach(XlCellRange range in function.Ranges) {
				if(count == 0)
					expression.Add(new XlPtgInt(code));
				expression.Add(CreatePtg(range));
				count++;
				if(count >= 29) {
					expression.Add(new XlPtgFuncVar(0x0158, count + 1, XlPtgDataType.Value));
					subtotalCount++;
					count = 0;
				}
			}
			if(count > 0) {
				expression.Add(new XlPtgFuncVar(0x0158, count + 1, XlPtgDataType.Value));
				subtotalCount++;
			}
			if(subtotalCount > 1) {
				if(function.Summary == XlSummary.Average)
					expression.Add(new XlPtgFuncVar(0x0005, subtotalCount, XlPtgDataType.Value));
				else if(function.Summary == XlSummary.Min)
					expression.Add(new XlPtgFuncVar(0x0006, subtotalCount, XlPtgDataType.Value));
				else if(function.Summary == XlSummary.Max)
					expression.Add(new XlPtgFuncVar(0x0007, subtotalCount, XlPtgDataType.Value));
				else
					expression.Add(new XlPtgFuncVar(0x0004, subtotalCount, XlPtgDataType.Value)); 
			}
			return true;
		}
		bool PrepareExpression(XlExpression expression, XlVLookupFunction function) {
			if(function == null)
				return false;
			XlPtgDataType savedParamType = paramType;
			paramType = XlPtgDataType.Value;
			try {
				if(function.LookupValue != null)
					PrepareExpression(expression, function.LookupValue);
				else
					expression.Add(new XlPtgErr(XlCellErrorType.Value));
				expression.Add(CreatePtg(function.Table));
				expression.Add(new XlPtgInt(function.ColumnIndex));
				expression.Add(new XlPtgBool(function.RangeLookup));
				expression.Add(new XlPtgFuncVar(0x0066, 4, XlPtgDataType.Value));
			}
			finally {
				paramType = savedParamType;
			}
			return true;
		}
		bool PrepareExpression(XlExpression formula, XlTextFunction function) {
			if(function == null)
				return false;
			XlPtgDataType savedParamType = paramType;
			paramType = XlPtgDataType.Value;
			try {
				if(function.Value != null)
					PrepareExpression(formula, function.Value);
				else
					formula.Add(new XlPtgErr(XlCellErrorType.Value));
				if(function.NumberFormat != null)
					formula.Add(new XlPtgStr(function.NumberFormat.GetLocalizedFormatCode(CurrentCulture)));
				else {
					ExcelNumberFormat numberFormat = numberFormatConverter.Convert(function.NetFormatString, function.IsDateTimeFormatString, CurrentCulture);
					string localFormatString = function.IsDateTimeFormatString ?
						numberFormatConverter.GetLocalDateFormatString(numberFormat != null ? numberFormat.FormatString : string.Empty, CurrentCulture) :
						numberFormatConverter.GetLocalFormatString(numberFormat != null ? numberFormat.FormatString : string.Empty, CurrentCulture);
					formula.Add(new XlPtgStr(localFormatString));
				}
				formula.Add(new XlPtgFunc(0x0030, XlPtgDataType.Value)); 
			}
			finally {
				paramType = savedParamType;
			}
			return true;
		}
		void PrepareExpression(XlExpression expression, IXlFormulaParameter parameter) {
			if(PrepareExpression(expression, parameter as XlFormulaParameter))
				return;
			if(PrepareExpression(expression, parameter as XlSubtotalFunction))
				return;
			if(PrepareExpression(expression, parameter as XlVLookupFunction))
				return;
			if(PrepareExpression(expression, parameter as XlTextFunction))
				return;
			if(PrepareExpression(expression, parameter as XlFunctionBase))
				return;
			XlCellRange cellRange = parameter as XlCellRange;
			if(cellRange != null)
				expression.Add(CreatePtg(cellRange, paramType));
			else
				expression.Add(new XlPtgErr(XlCellErrorType.Value));
		}
		bool PrepareExpression(XlExpression expression, XlFormulaParameter parameter) {
			if(parameter == null)
				return false;
			expression.Add(CreatePtg(parameter.Value));
			return true;
		}
		bool PrepareExpression(XlExpression expression, XlFunctionBase function) {
			if(function == null)
				return false;
			XlPtgDataType savedParamType = paramType;
			paramType = function.ParamType;
			try {
				if(function.Parameters.Count == 0)
					expression.Add(new XlPtgErr(XlCellErrorType.Value));
				else {
					foreach(IXlFormulaParameter parameter in function.Parameters)
						PrepareExpression(expression, parameter);
					expression.Add(new XlPtgFuncVar(function.FunctionCode, function.Parameters.Count, XlPtgDataType.Value));
				}
			}
			finally {
				paramType = savedParamType;
			}
			return true;
		}
		void WriteBlankCells() {
			IXlCell cell = this.cellsToExport[0];
			int count = this.cellsToExport.Count;
			if(count > 1) {
				XlsContentMulBlank content = new XlsContentMulBlank();
				content.RowIndex = cell.RowIndex;
				content.FirstColumnIndex = cell.ColumnIndex;
				for(int i = 0; i < count; i++)
					content.FormatIndices.Add(GetFormatIndex(this.cellsToExport[i].Formatting));
				WriteContent(XlsRecordType.MulBlank, content);
			}
			else {
				XlsContentBlank content = new XlsContentBlank();
				InitializeContent(content, cell);
				WriteContent(XlsRecordType.Blank, content);
			}
		}
		void WriteNumericCells() {
			IXlCell cell = this.cellsToExport[0];
			int count = this.cellsToExport.Count;
			if(count > 1) {
				XlsContentMulRk content = new XlsContentMulRk();
				content.RowIndex = cell.RowIndex;
				content.FirstColumnIndex = cell.ColumnIndex;
				for(int i = 0; i < count; i++) {
					cell = this.cellsToExport[i];
					XlsRkRec item = new XlsRkRec();
					item.FormatIndex = GetFormatIndex(cell.Formatting);
					item.Rk.Value = cell.Value.NumericValue;
					content.RkRecords.Add(item);
				}
				WriteContent(XlsRecordType.MulRk, content);
			}
			else if(XlsRkNumber.IsRkValue(cell.Value.NumericValue)) {
				XlsContentRk content = new XlsContentRk();
				InitializeContent(content, cell);
				content.Value = (double)cell.Value.NumericValue;
				WriteContent(XlsRecordType.Rk, content);
			}
			else {
				XlsContentNumber content = new XlsContentNumber();
				InitializeContent(content, cell);
				content.Value = (double)cell.Value.NumericValue;
				WriteContent(XlsRecordType.Number, content);
			}
		}
		void WriteBooleanCell() {
			IXlCell cell = this.cellsToExport[0];
			XlsContentBoolErr content = new XlsContentBoolErr();
			InitializeContent(content, cell);
			content.Value = (byte)(cell.Value.BooleanValue ? 1 : 0);
			content.IsError = false;
			WriteContent(XlsRecordType.BoolErr, content);
		}
		void WriteSharedStringCell() {
			XlCell cell = this.cellsToExport[0];
			XlsContentLabelSst content = new XlsContentLabelSst();
			InitializeContent(content, cell);
			XlRichTextString richText = cell.RichTextValue;
			if(richText != null)
				content.StringIndex = RegisterString(richText);
			else
				content.StringIndex = RegisterString(cell.Value.TextValue);
			WriteContent(XlsRecordType.LabelSst, content);
		}
		void WriteErrorCell() {
			IXlCell cell = this.cellsToExport[0];
			XlsContentBoolErr content = new XlsContentBoolErr();
			InitializeContent(content, cell);
			content.Value = (byte)cell.Value.ErrorValue.Type;
			content.IsError = true;
			WriteContent(XlsRecordType.BoolErr, content);
		}
		int GetFormatIndex(XlCellFormatting formatting) {
			int formatIndex = RegisterFormatting(formatting);
			if(formatIndex < 0)
				formatIndex = XlsDefs.DefaultCellXFIndex;
			return formatIndex;
		}
		void InitializeContent(XlsContentCellBase content, IXlCell cell) {
			content.RowIndex = cell.RowIndex;
			content.ColumnIndex = cell.ColumnIndex;
			content.FormatIndex = GetFormatIndex(cell.Formatting);
		}
		XlPtgBase CreatePtg(XlCellRange range) {
			return CreatePtg(range, XlPtgDataType.Reference);
		}
		XlPtgBase CreatePtg(XlCellRange range, XlPtgDataType dataType) {
			if(string.IsNullOrEmpty(range.SheetName)) {
				if(range.TopLeft.Equals(range.BottomRight)) {
					return new XlPtgRef(range.TopLeft) { DataType = dataType };
				}
				return new XlPtgArea(range) { DataType = dataType };
			}
			if(range.TopLeft.Equals(range.BottomRight))
				return new XlPtgRef3d(range.TopLeft, range.SheetName) { DataType = dataType };
			return new XlPtgArea3d(range, range.SheetName) { DataType = dataType };
		}
		XlPtgBase CreatePtg(XlVariantValue value) {
			if(value.IsBoolean)
				return new XlPtgBool(value.BooleanValue);
			if(value.IsNumeric)
				return new XlPtgNum(value.NumericValue);
			if(value.IsText)
				return new XlPtgStr(value.TextValue);
			if (value.IsError) {
				if(value.ErrorValue.Type == XlCellErrorType.Reference)
					return new XlPtgRefErr(paramType);
				return new XlPtgErr(value.ErrorValue.Type);
			}
			return new XlPtgMissArg();
		}
	}
}
