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
using System.Diagnostics;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
#if BTREE
		int currentRowIndex = -1;
#endif
		protected internal virtual void ExportCell(ICell cell) {
#if BTREE
			int cellRowIndex = cell.RowIndex;
			if (currentRowIndex != cellRowIndex) {
				if (currentRowIndex >= 0) {
					WriteShEndElement();
				}
				WriteShStartElement("row");
				currentRowIndex = cellRowIndex;
				ExportRowProperties(new Row(cellRowIndex, ActiveSheet));
			}
#endif
			WriteShStartElement("c");
			try {
				FormulaBase formula = null;
				bool cellHasFormula = false;
				if (cell.HasFormula) {
					formula = PrepareFormula(cell);
					cellHasFormula = formula != null;
					shouldExportCalculationChain &= cellHasFormula;  
				}
				ExportCellProperties(cell, cellHasFormula);
				if (cellHasFormula)
					ExportFormula(cell, formula);
				ExportCellValue(cell.Value, cellHasFormula);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void ExportExternalCellValue(VariantValue value, bool cellHasFormula) {
			System.Diagnostics.Debug.Assert(value.Type != VariantValueType.SharedString);
			if (value.IsEmpty)
				return;
			ExportCellValueCore(value);
		}
		protected internal virtual void ExportCellValue(VariantValue value, bool cellHasFormula) {
			if (value.IsEmpty)
				return;
			if (value.IsSharedString && cellHasFormula)
				value = value.GetTextValue(Workbook.SharedStringTable);
			if (value.IsInlineText && !cellHasFormula) {
				WriteShStartElement("is");
				try {
					WriteShString("t", EncodeXmlChars(value.InlineTextValue), true);
				}
				finally {
					WriteShEndElement();
				}
			}
			else if (value.IsSharedString && !cellHasFormula) {
				ExportSharedStringValue(value.SharedStringIndexValue);
			}
			else {
				if (value.IsSharedString)
					ExportSharedStringValue(value.SharedStringIndexValue);
				else
					ExportCellValueCore(value);
			}
		}
		void ExportSharedStringValue(SharedStringIndex index) {
			int sstIndex = index.ToInt();
			WriteShString("v", ExportStyleSheet.SharedStringsTable[sstIndex].ToString(), true);
		}
		protected internal virtual void ExportCellValueCore(VariantValue value) {
			System.Diagnostics.Debug.Assert(value.Type != VariantValueType.SharedString);
			string cellValue = String.Empty;
			if (value.IsText)
				cellValue = value.GetTextValue(Workbook.SharedStringTable);
			else if (value.IsBoolean)
				cellValue = value.BooleanValue ? "1" : "0";
			else if (value.IsNumeric)
				cellValue = value.NumericValue.ToString("G17", CultureInfo.InvariantCulture);
			else if (value.IsError)
				cellValue = value.ErrorValue.Name;
			else
				Exceptions.ThrowInternalException();
				WriteShString("v", EncodeXmlChars(cellValue), true);
		}
		protected internal virtual void ExportCellProperties(ICell cell, bool cellHasFormula) {
			ExportCellReference(cell);
			int index;
			if (!ExportStyleSheet.CellFormatTable.TryGetValue(cell.FormatIndex, out index))
				Exceptions.ThrowInternalException();
			if (index > 0)
				WriteShIntValue("s", index);
			ExportCellDataType(cell.Value, cellHasFormula);
		}
		protected internal virtual void ExportCellReference(ICellBase cell) {
			CellPosition position = new CellPosition(cell.ColumnIndex, cell.RowIndex);
			WriteShStringValue("r", CellReferenceParser.ToString(position));
		}
		protected internal virtual void ExportCellDataType(VariantValue value, bool cellHasFormula) {
			string type = String.Empty;
			if (cellHasFormula && value.IsText)
				type = "str";
			else if (value.IsBoolean)
				type = "b";
			else if (value.IsError)
				type = "e";
			else if (value.IsSharedString)
				type = "s";
			else if (value.IsInlineText)
				type = "inlineStr";
			else if (value.IsNumeric) {
			}
			if (!String.IsNullOrEmpty(type))
				WriteShStringValue("t", type);
		}
		protected internal virtual void ExportExternalCellDataType(VariantValue value, bool cellHasFormula) {
			System.Diagnostics.Debug.Assert(value.Type != VariantValueType.SharedString);
			string type = String.Empty;
			if (value.IsText)
				type = "str";
			else if (value.IsBoolean)
				type = "b";
			else if (value.IsError)
				type = "e";
			else if (value.IsSharedString)
				type = "s";
			else if (value.IsNumeric) {
			}
			if (!String.IsNullOrEmpty(type))
				WriteShStringValue("t", type);
		}
	}
}
