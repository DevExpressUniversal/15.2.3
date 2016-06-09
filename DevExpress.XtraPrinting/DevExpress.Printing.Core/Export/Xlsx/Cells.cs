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
using System.Text;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Implementation;
using DevExpress.Utils;
namespace DevExpress.XtraExport.Xlsx {
	partial class XlsxDataAwareExporter {
		StringBuilder cellRefBuilder = new StringBuilder(10);
		XlCell currentCell = new XlCell();
		XlCellFormatting inheritedFormatting = null;
		readonly XlExpressionContext expressionContext = new XlExpressionContext();
		readonly Dictionary<XlCellPosition, int> sharedFormulaTable = new Dictionary<XlCellPosition, int>();
		bool insideCellScope = false;
		public int CurrentColumnIndex { get { return columnIndex; } }
		public IXlCell BeginCell() {
			ExportPendingRow();
			currentCell.Value = XlVariantValue.Empty;
			currentCell.Formula = null;
			currentCell.Expression = null;
			currentCell.FormulaString = null;
			currentCell.SharedFormulaPosition = XlCellPosition.InvalidValue;
			currentCell.SharedFormulaRange = null;
			currentCell.RowIndex = rowIndex;
			currentCell.ColumnIndex = columnIndex;
			IXlColumn column;
			if(columns.TryGetValue(columnIndex, out column))
				inheritedFormatting = XlCellFormatting.Merge(XlFormatting.CopyObject(column.Formatting), currentRow.Formatting);
			else
				inheritedFormatting = XlFormatting.CopyObject(currentRow.Formatting);
			currentCell.Formatting = XlFormatting.CopyObject(inheritedFormatting);
			insideCellScope = true;
			return currentCell;
		}
		public void EndCell() {
			if(currentCell.ColumnIndex < 0 || currentCell.ColumnIndex >= options.MaxColumnCount)
				throw new ArgumentOutOfRangeException(string.Format("Cell column index out of range 0..{0}.", options.MaxColumnCount - 1));
			try {
				XlVariantValue cellValue = currentCell.Value;
				bool hasFormula = currentCell.HasFormula;
				if(!cellValue.IsEmpty || hasFormula || !XlCellFormatting.Equals(inheritedFormatting, currentCell.Formatting)) {
					WriteShStartElement("c");
					ExportCellProperties(currentCell);
					if(hasFormula)
						ExportCellFormula(currentCell);
					if(!cellValue.IsEmpty)
						ExportCellValue(cellValue, hasFormula);
					WriteShEndElement(); 
					currentSheet.RegisterCellPosition(currentCell);
					calculationOptions.FullCalculationOnLoad |= currentCell.HasFormulaWithoutValue;
				}
				columnIndex = currentCell.ColumnIndex + 1;
			}
			finally {
				insideCellScope = false;
			}
		}
		public void SkipCells(int count) {
			Guard.ArgumentPositive(count, "count");
			ExportPendingRow();
			if(insideCellScope)
				throw new InvalidOperationException("Operation cannot be executed inside BeginCell/EndCell scope.");
			if((columnIndex + count) >= options.MaxColumnCount)
				throw new ArgumentOutOfRangeException(string.Format("Cell column index goes beyond range 0..{0}.", options.MaxColumnCount - 1));
			columnIndex += count;
		}
		void ExportCellProperties(XlCell cell) {
			ExportCellDataType(cell.Value, cell.HasFormula);
			WriteShStringValue("r", GetCellReference(cell));
			int index = RegisterFormatting(cell.Formatting);
			if (index > 0)
				WriteShIntValue("s", index);
		}
		string GetCellReference(IXlCell cell) {
			cellRefBuilder.Clear();
			int columnIndex = cell.ColumnIndex + 1;
			int lastPart = columnIndex % 26;
			if(lastPart == 0)
				lastPart = 26;
			if(columnIndex > 702) {
				int middlePart = (columnIndex - lastPart) % 676;
				if(middlePart == 0)
					middlePart = 676;
				cellRefBuilder.Append((char)('@' + (columnIndex - middlePart - lastPart) / 676));
				cellRefBuilder.Append((char)('@' + middlePart / 26));
			}
			else if(columnIndex > 26) {
				cellRefBuilder.Append((char)('@' + (columnIndex - lastPart) / 26));
			}
			cellRefBuilder.Append((char)('@' + lastPart));
			cellRefBuilder.Append(cell.RowIndex + 1);
			return cellRefBuilder.ToString();
		}
		void ExportCellFormula(XlCell cell) {
			if(cell.SharedFormulaPosition.IsValid) {
				if(!sharedFormulaTable.ContainsKey(cell.SharedFormulaPosition))
					return;
				WriteShStartElement("f");
				try {
					WriteStringValue("t", "shared");
					WriteStringValue("si", sharedFormulaTable[cell.SharedFormulaPosition].ToString());
				}
				finally {
					WriteShEndElement();
				}
			}
			else {
				bool isSharedFormula = cell.SharedFormulaRange != null;
				string formula = GetFormulaString(cell, isSharedFormula);
				if(String.IsNullOrEmpty(formula))
					return;
				WriteShStartElement("f");
				try {
					if(isSharedFormula) {
						int sharedFormulaIndex = sharedFormulaTable.Count;
						XlCellPosition hostCell = new XlCellPosition(cell.ColumnIndex, cell.RowIndex);
						sharedFormulaTable.Add(hostCell, sharedFormulaIndex);
						XlCellRange range = cell.SharedFormulaRange.AsRelative();
						range.SheetName = string.Empty;
						WriteStringValue("t", "shared");
						WriteStringValue("ref", range.ToString());
						WriteStringValue("si", sharedFormulaIndex.ToString());
					}
					WriteShString(EncodeXmlCharsNoCrLf(formula));
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		string GetFormulaString(XlCell cell, bool isSharedFormula) {
			if(cell.Formula != null) {
				string result = cell.Formula.ToString(CurrentCulture);
				if(result.Length <= 8192)
					return result;
			}
			else if(cell.Expression != null) {
				expressionContext.CurrentCell = new XlCellPosition(cell.ColumnIndex, cell.RowIndex);
				expressionContext.ReferenceMode = isSharedFormula ? XlCellReferenceMode.Offset : XlCellReferenceMode.Reference;
				expressionContext.ExpressionStyle = isSharedFormula ? XlExpressionStyle.Shared : XlExpressionStyle.Normal;
				string result = cell.Expression.ToString(expressionContext);
				if(result.Length <= 8192)
					return result;
			}
			else if(!string.IsNullOrEmpty(cell.FormulaString)) {
				string result = cell.FormulaString;
				if(formulaParser != null) {
					expressionContext.CurrentCell = new XlCellPosition(cell.ColumnIndex, cell.RowIndex);
					expressionContext.ReferenceMode = isSharedFormula ? XlCellReferenceMode.Offset : XlCellReferenceMode.Reference;
					expressionContext.ExpressionStyle = isSharedFormula ? XlExpressionStyle.Shared : XlExpressionStyle.Normal;
					XlExpression expression = formulaParser.Parse(result, expressionContext);
					if(expression == null || expression.Count == 0)
						return string.Empty;
				}
				if(result.Length <= 8192)
					return result;
			}
			return "#VALUE!";
		}
		void ExportCellDataType(XlVariantValue value, bool cellHasFormula) {
			if (value.IsText)
				WriteShStringValue("t", cellHasFormula ? "str" : "s");
			else if (value.IsBoolean)
				WriteShStringValue("t", "b");
			else if (value.IsError)
				WriteShStringValue("t", "e");
		}
		void ExportCellValue(XlVariantValue value, bool hasFormula) {
			if(value.IsNumeric) {
				double numericValue = value.NumericValue;
				if(DevExpress.XtraExport.Xls.XNumChecker.IsNegativeZero(numericValue))
					numericValue = 0.0;
				WriteShString("v", numericValue.ToString("G17", CultureInfo.InvariantCulture));
			}
			else if(value.IsText) {
				if(!hasFormula) {
					XlRichTextString richText = currentCell.RichTextValue;
					int index = richText == null ? sharedStringTable.RegisterString(value.TextValue) : sharedStringTable.RegisterString(richText);
					WriteShString("v", index.ToString());
				}
				else
					WriteShString("v", EncodeXmlChars(value.TextValue), true);
			}
			else if(value.IsBoolean)
				WriteShString("v", value.BooleanValue ? "1" : "0");
			else if(value.IsError)
				WriteShString("v", value.ErrorValue.Name);
		}
	}
}
