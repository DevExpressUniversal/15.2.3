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
using System.Text;
using DevExpress.Utils;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public class ParsedThingTable : ParsedThingWithDataType {
		#region Fields
		string tableName;
		TableRowEnum includedRows = TableRowEnum.NotDefined;
		string columnStart;
		string columnEnd;
		static readonly Dictionary<TableRowEnum, string> rowDescriptionsTable = CreateRowDescriptionsTable();
		static char[] quotedColumnSymbols = { '\'', '\\', '[', ']', '@', '#' };
		#endregion
		static Dictionary<TableRowEnum, string> CreateRowDescriptionsTable() {
			Dictionary<TableRowEnum, string> result = new Dictionary<TableRowEnum, string>();
			result.Add(TableRowEnum.All, "All");
			result.Add(TableRowEnum.Data, "Data");
			result.Add(TableRowEnum.Headers, "Headers");
			result.Add(TableRowEnum.ThisRow, "This Row");
			result.Add(TableRowEnum.Totals, "Totals");
			return result;
		}
		#region Properties
		public string TableName { get { return tableName; } set { tableName = value; } }
		public bool ColumnsDefined { get { return !string.IsNullOrEmpty(columnStart); } }
		public string ColumnStart { get { return columnStart; } set { columnStart = value; } }
		public string ColumnEnd { get { return columnEnd; } set { columnEnd = value; } }
		public TableRowEnum IncludedRows { get { return includedRows; } set { includedRows = value; } }
		#endregion
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		#region BuildExpressionString
		public override void BuildExpressionString(Stack<int> stack, System.Text.StringBuilder builder, System.Text.StringBuilder spacesBuilder, WorkbookDataContext context) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			BuildExpressionStringCore(builder, context);
		}
		internal void BuildExpressionStringCore(StringBuilder result, WorkbookDataContext context) {
			string separator = context.GetListSeparator() + " ";
			result.Append(tableName);
			bool columnsDefined = ColumnsDefined;
			bool rowsDefined = includedRows != TableRowEnum.NotDefined;
			if (rowsDefined || columnsDefined || context.ImportExportMode) {
				result.Append("[");
				if (rowsDefined)
					BuildRowConditions(result, columnsDefined, separator);
				if (columnsDefined) {
					if (rowsDefined)
						result.Append(separator);
					BuildColumnConditions(result, rowsDefined);
				}
				result.Append("]");
			}
		}
		void BuildRowConditions(StringBuilder result, bool hasColumnConditions, string separator) {
			bool needBrackets = hasColumnConditions;
			if (includedRows == TableRowEnum.All)
				result.Append(RowConditionToString(TableRowEnum.All, needBrackets, separator));
			else {
				if (GetAllowedCol(TableRowEnum.Headers)) {
					result.Append(RowConditionToString(TableRowEnum.Headers, needBrackets, separator));
					needBrackets = true;
				}
				if (GetAllowedCol(TableRowEnum.Data)) {
					result.Append(RowConditionToString(TableRowEnum.Data, needBrackets, separator));
					needBrackets = true;
				}
				if (GetAllowedCol(TableRowEnum.Totals))
					result.Append(RowConditionToString(TableRowEnum.Totals, needBrackets, separator));
				if (GetAllowedCol(TableRowEnum.ThisRow))
					result.Append(RowConditionToString(TableRowEnum.ThisRow, needBrackets, separator));
			}
			result.Remove(result.Length - 2, 2);
		}
		bool GetAllowedCol(TableRowEnum mask) {
			return (includedRows & mask) != 0;
		}
		public void SetAllowedCol(TableRowEnum mask) {
			includedRows |= mask;
		}
		string RowConditionToString(TableRowEnum value, bool needBrackets, string separator) {
			string format = needBrackets ? "[#{0}]" : "#{0}";
			format += separator;
			return String.Format(format, rowDescriptionsTable[value]);
		}
		void BuildColumnConditions(StringBuilder result, bool hasRowConditions) {
			bool hasIncludedColumnEnd = !string.IsNullOrEmpty(columnEnd);
			bool needBrackets = hasRowConditions || hasIncludedColumnEnd;
			result.Append(ColConditionToString(columnStart, needBrackets));
			if (hasIncludedColumnEnd) {
				result.Append(':');
				result.Append(ColConditionToString(columnEnd, needBrackets));
			}
		}
		string ColConditionToString(string columnName, bool needBrackets) {
			return QuoteColumnName(columnName, needBrackets);
		}
		string UnQuoteColumnName(string s) {
			int curPos = 0;
			char[] quotedSymbols = quotedColumnSymbols;
			string result = s;
			while (curPos < result.Length - 1) {
				if (result[curPos] == '\'' && (Array.IndexOf(quotedSymbols, result[curPos + 1]) >= 0))
					result = result.Remove(curPos, 1);
				curPos++;
			}
			return result;
		}
		string QuoteColumnName(string s, bool needBrackets) {
			int curPos = 0;
			char[] quotedSymbols = quotedColumnSymbols;
			string result = s;
			while (curPos < result.Length) {
				if (Array.IndexOf(quotedSymbols, result[curPos]) >= 0)
					result = result.Insert(curPos++, "'");
				curPos++;
			}
			string format = needBrackets || result.Trim().Length != result.Length ? "[{0}]" : "{0}";
			return string.Format(format, result);
		}
		#endregion
		public VariantValue PreEvaluate(WorkbookDataContext context) {
			VariantValue result;
			Table table = context.GetDefinedTableRange(tableName);
			if (table == null)
				result = VariantValue.ErrorName;
			else
				result = table.GetRangeByCondition(includedRows, columnStart, columnEnd, context.CurrentRowIndex);
			return result;
		}
		public override void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
			stack.Push(EvaluateToValue(context));
		}
		public virtual VariantValue EvaluateToValue(WorkbookDataContext context) {
			VariantValue result = PreEvaluate(context);
			if (!result.IsError) {
				if (DataType == OperandDataType.Value) {
					if (context.Workbook.CalculationChain.Enabled || !context.ArrayFormulaProcessing)
						result = context.DereferenceValue(result, false);
				}
				else if (DataType == OperandDataType.Array) {
					if (result.IsCellRange) {
						CellRange range = result.CellRangeValue.GetFirstInnerCellRange();
						RangeVariantArray array = new RangeVariantArray(range);
						result = VariantValue.FromArray(array);
					}
				}
			}
			return result;
		}
		public void SetIncludedColumns(string nameStart, string nameEnd) {
			columnStart = UnQuoteColumnName(nameStart);
			columnEnd = UnQuoteColumnName(nameEnd);
		}
		public override void GetInvolvedCellRanges(Stack<CellRangeList> stack, WorkbookDataContext context) {
			CellRangeList result = new CellRangeList();
			VariantValue value = PreEvaluate(context);
			if (!value.IsError && value.IsCellRange) {
				CellRangeBase range = value.CellRangeValue;
				if (DataType == OperandDataType.Value && !context.ArrayFormulaProcessing) {
					CellPosition position = context.DereferenceToCellPosition(range);
					if (position.IsValid)
						result.Add(new CellRange(range.Worksheet, position, position));
				}
				else
					result.Add(range);
			}
			stack.Push(result);
		}
		public override IParsedThing Clone() {
			ParsedThingTable clone = new ParsedThingTable();
			clone.DataType = DataType;
			clone.TableName = TableName;
			clone.IncludedRows = IncludedRows;
			clone.columnStart = columnStart;
			clone.columnEnd = columnEnd;
			return clone;
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			ParsedThingTable anotherPtg = (ParsedThingTable)obj;
			return this.includedRows == anotherPtg.includedRows &&
				StringExtensions.CompareInvariantCultureIgnoreCase(this.TableName, anotherPtg.TableName) == 0 &&
				StringExtensions.CompareInvariantCultureIgnoreCase(this.ColumnStart, anotherPtg.ColumnStart) == 0 &&
				StringExtensions.CompareInvariantCultureIgnoreCase(this.ColumnEnd, anotherPtg.ColumnEnd) == 0;
		}
		protected internal SheetDefinition GetSheetDefinition(WorkbookDataContext context) {
			Table table = context.GetDefinedTableRange(tableName);
			if (table != null) {
				SheetDefinition sheetDefinition = new SheetDefinition();
				if (context.DefinedNameProcessing || (context.CurrentWorksheet.SheetId != table.Worksheet.SheetId)) {
					sheetDefinition.SheetNameStart = table.Worksheet.Name;
				}
				return sheetDefinition;
			}
			return null;
		}
	}
	public class ParsedThingTableExt : ParsedThingTable, IParsedThingWithSheetDefinition {
		#region Fields
		int sheetDefinitionIndex;
		#endregion
		#region Propertries
		public int SheetDefinitionIndex { get { return sheetDefinitionIndex; } set { sheetDefinitionIndex = value; } }
		#endregion
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override void BuildExpressionString(Stack<int> stack, System.Text.StringBuilder builder, System.Text.StringBuilder spacesBuilder, WorkbookDataContext context) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			SheetDefinition sheetDefinition = context.GetSheetDefinition(sheetDefinitionIndex);
			sheetDefinition.BuildExpressionString(builder, context);
			BuildExpressionStringCore(builder, context);
		}
		public override VariantValue EvaluateToValue(WorkbookDataContext context) {
			return VariantValue.ErrorValueNotAvailable;
		}
		public override void GetInvolvedCellRanges(Stack<CellRangeList> stack, WorkbookDataContext context) {
			stack.Push(ParsedThingBase.EmptyCellRangeList);
		}
		public override IParsedThing Clone() {
			ParsedThingTableExt clone = new ParsedThingTableExt();
			clone.DataType = DataType;
			clone.TableName = TableName;
			clone.IncludedRows = IncludedRows;
			clone.ColumnStart = ColumnStart;
			clone.ColumnEnd = ColumnEnd;
			clone.sheetDefinitionIndex = sheetDefinitionIndex;
			return clone;
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			ParsedThingTableExt anotherPtg = (ParsedThingTableExt)obj;
			return this.sheetDefinitionIndex == anotherPtg.sheetDefinitionIndex;
		}
	}
}
