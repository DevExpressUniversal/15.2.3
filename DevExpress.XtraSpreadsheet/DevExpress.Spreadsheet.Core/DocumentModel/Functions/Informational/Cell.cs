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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Export.Xl;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionCell
	public class FunctionCell : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		#region formatCodeTable
		static readonly Dictionary<string, string> formatCodeTable = CreateFormatCodeTable();
		static Dictionary<string, string> CreateFormatCodeTable() {
			Dictionary<string, string> result = new Dictionary<string, string>();
			result.Add("0", "F0");
			result.Add("#,##0", ",0");
			result.Add("0.00", "F2");
			result.Add("#,##0.00", ",2");
			result.Add("$#,##0_);($#,##0)", "C0");
			result.Add("$#,##0_);[Red]($#,##0)", "C0-");
			result.Add("$#,##0.00_);($#,##0.00)", "C2");
			result.Add("$#,##0.00_);[Red]($#,##0.00)", "C2-");
			result.Add("0%", "P0");
			result.Add("0.00%", "P2");
			result.Add("0.00E+00", "S2");
			result.Add("# ?/?", "G");
			result.Add("# ??/??", "G");
			result.Add("m/d/yy", "D4");
			result.Add("m/d/yy h:mm", "D4");
			result.Add("mm/dd/yy", "D4");
			result.Add("d-mmm-yy", "D1");
			result.Add("dd-mmm-yy", "D1");
			result.Add("d-mmm", "D2");
			result.Add("dd-mmm", "D2");
			result.Add("mmm-yy", "D3");
			result.Add("mm/dd", "D5");
			result.Add("h:mm AM/PM", "D7");
			result.Add("h:mm:ss AM/PM", "D6");
			result.Add("h:mm", "D9");
			result.Add("h:mm:ss", "D8");
			return result;
		}
		#endregion
		public override string Name { get { return "CELL"; } }
		public override int Code { get { return 0x007D; } }
		public override bool IsVolatile { get { return true; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			int operandCount = arguments.Count;
			VariantValue argument = arguments[0].ToText(context);
			if (argument.IsError)
				return argument;
			ICellBase cell;
			if (operandCount == 2) {
				VariantValue reference = arguments[1];
				if (reference.IsError)
					return reference;
				CellRangeBase range = reference.CellRangeValue;
				CellPosition topLeft = range.TopLeft;
				cell = range.Worksheet.TryGetCell(topLeft.Column, range.TopLeft.Row);
				if (cell == null)
					cell = new FakeCell(new CellPosition(topLeft.Column, range.TopLeft.Row), (Worksheet)range.Worksheet);
			}
			else {
				CellPosition currentCellPosition = context.Workbook.ActiveSheet.Selection.ActiveCell;   
				cell = context.CurrentWorksheet.TryGetCell(currentCellPosition.Column, currentCellPosition.Row);
				if (cell == null)
					cell = new FakeCell(currentCellPosition, (Worksheet)context.CurrentWorksheet);
			}
			return GetResult(argument.GetTextValue(context.StringTable), (ICell)cell, context);
		}
		VariantValue GetResult(string argument, ICell cell, WorkbookDataContext context) {
			if (CompareParamWithPattern(argument, XtraSpreadsheetFunctionNameStringId.AddressInfo, "address", context))
				return GetAddress(cell, context);
			if (CompareParamWithPattern(argument, XtraSpreadsheetFunctionNameStringId.ColumnInfo, "col", context))
				return GetColumn(cell);
			if (CompareParamWithPattern(argument, XtraSpreadsheetFunctionNameStringId.ColorInfo, "color", context))
				return GetColor(cell);
			if (CompareParamWithPattern(argument, XtraSpreadsheetFunctionNameStringId.ContentsInfo, "contents", context))
				return GetContents(cell);
			if (CompareParamWithPattern(argument, XtraSpreadsheetFunctionNameStringId.FilenameInfo, "filename", context))
				return GetFileName(cell);
			if (CompareParamWithPattern(argument, XtraSpreadsheetFunctionNameStringId.FormatInfo, "format", context))
				return GetFormat(cell);
			if (CompareParamWithPattern(argument, XtraSpreadsheetFunctionNameStringId.ParenthesesInfo, "parentheses", context))
				return GetParentheses(cell);
			if (CompareParamWithPattern(argument, XtraSpreadsheetFunctionNameStringId.PrefixInfo, "prefix", context))
				return GetPrefix(cell);
			if (CompareParamWithPattern(argument, XtraSpreadsheetFunctionNameStringId.ProtectInfo, "protect", context))
				return GetProtect(cell);
			if (CompareParamWithPattern(argument, XtraSpreadsheetFunctionNameStringId.RowInfo, "row", context))
				return GetRow(cell);
			if (CompareParamWithPattern(argument, XtraSpreadsheetFunctionNameStringId.TypeInfo, "type", context))
				return GetType(cell);
			if (CompareParamWithPattern(argument, XtraSpreadsheetFunctionNameStringId.WidthInfo, "width", context))
				return GetWidth(cell);
			return VariantValue.ErrorInvalidValueInFunction;
		}
		bool CompareParamWithPattern(string argument, XtraSpreadsheetFunctionNameStringId stringId, string defaultValue, WorkbookDataContext context) {
			string paramPattern = FormulaCalculator.GetFunctionParameterName(stringId, defaultValue, context);
			if (StringExtensions.CompareInvariantCultureIgnoreCase(argument, paramPattern) == 0)
				return true;
			return StringExtensions.CompareInvariantCultureIgnoreCase(argument, defaultValue) == 0;
		}
		VariantValue GetWidth(ICell cell) {
			IColumnRange column = cell.Worksheet.Columns.GetReadonlyColumnRange(cell.ColumnIndex);
			float colWidthInChars = DevExpress.XtraSpreadsheet.Layout.Engine.ColumnWidthCalculationUtils.CalculateColumnWidthInCharsRound(column);
			return DXMath.Round(colWidthInChars);
		}
		VariantValue GetProtect(ICell cell) {
			return cell.Protection.Locked ? 1 : 0;
		}
		VariantValue GetPrefix(ICell cell) {
			VariantValue value = cell.Value;
			if (!value.IsText)
				return String.Empty;
			switch (cell.ActualAlignment.Horizontal) {
				case XlHorizontalAlignment.General:
					return "'";
				case XlHorizontalAlignment.Left:
					return "'";
				case XlHorizontalAlignment.Center:
					return "^";
				case XlHorizontalAlignment.Right:
					return "\"";
				case XlHorizontalAlignment.Fill:
					return "\\";
				case XlHorizontalAlignment.Justify:
					return "'";
				case XlHorizontalAlignment.CenterContinuous:
					return "^";
				case XlHorizontalAlignment.Distributed:
					return "'";
				default:
					return "'";
			}
		}
		VariantValue GetParentheses(ICell cell) {
			NumberFormat format = cell.Format;
			return format.EnclosedInParenthesesForPositive() ? 1 : 0;
		}
		VariantValue GetColor(ICell cell) {
			NumberFormat format = cell.Format;
			return format.HasColorForPositiveOrNegative() ? 1 : 0;
		}
		VariantValue GetFormat(ICell cell) {
			NumberFormat format = cell.Format;
			string result;
			if (formatCodeTable.TryGetValue(format.FormatCode, out result))
				return result;
			return "G";
		}
		VariantValue GetFileName(ICell cell) {
			string filePath = cell.Worksheet.Workbook.DocumentSaveOptions.CurrentFileName;
			if (String.IsNullOrEmpty(filePath))
				return filePath;
			string path = Path.GetDirectoryName(filePath);
			string fileName = "[" + Path.GetFileName(filePath) + "]" + cell.Worksheet.Name;
			return Path.Combine(path, fileName);
		}
		VariantValue GetAddress(ICell cell, WorkbookDataContext context) {
			FunctionAddress function = new FunctionAddress();
			return function.CreateReferenceText(context, cell.RowIndex, cell.ColumnIndex, PositionType.Absolute, PositionType.Absolute, context.UseR1C1ReferenceStyle, String.Empty);
		}
		VariantValue GetContents(ICell cell) {
			VariantValue result;
			cell.Worksheet.Workbook.CalculationChain.TryGetCalculatedValue(cell, out result);
			return result;
		}
		VariantValue GetType(ICell cell) {
			VariantValue value = cell.Value;
			if (value.IsEmpty)
				return "b";
			if (value.IsText)
				return "l";
			return "v";
		}
		VariantValue GetRow(ICell cell) {
			return cell.RowIndex + 1;
		}
		VariantValue GetColumn(ICell cell) {
			return cell.ColumnIndex + 1;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Reference, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion
}
