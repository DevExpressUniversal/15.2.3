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
using System.Text;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Internal {
	static class XlsLimits {
		public static CellRange GetBoundRange(IWorksheet sheet) {
			return new CellRange(sheet, new CellPosition(0, 0), new CellPosition(XlsDefs.MaxColumnCount - 1, XlsDefs.MaxRowCount - 1));
		}
		public static CellRange GetBoundRange(CellRange range) {
			return new CellRange(range.Worksheet,
				new CellPosition(0, 0, range.TopLeft.ColumnType, range.TopLeft.RowType),
				new CellPosition(XlsDefs.MaxColumnCount - 1, XlsDefs.MaxRowCount - 1, range.BottomRight.ColumnType, range.BottomRight.RowType));
		}
		public static bool OutOfLimits(this CellPosition position) {
			return position.Column >= XlsDefs.MaxColumnCount || position.Row >= XlsDefs.MaxRowCount;
		}
		public static bool OutOfLimits(this CellOffset offset) {
			return (offset.ColumnType == CellOffsetType.Position && offset.Column >= XlsDefs.MaxColumnCount) ||
				(offset.RowType == CellOffsetType.Position && offset.Row >= XlsDefs.MaxRowCount);
		}
		public static bool ShouldBeTruncated(this CellOffset offset) {
			return (offset.ColumnType == CellOffsetType.Offset && (offset.Column & (XlsDefs.MaxColumnCount - 1)) != 0) ||
				(offset.RowType == CellOffsetType.Offset && (offset.Row & (XlsDefs.MaxRowCount - 1)) != 0);
		}
		public static CellOffset GetTruncated(this CellOffset offset) {
			CellOffsetType columnType = offset.ColumnType;
			CellOffsetType rowType = offset.RowType;
			int column = columnType != CellOffsetType.Position ? offset.Column & (XlsDefs.MaxColumnCount - 1) : Math.Min(offset.Column, XlsDefs.MaxColumnCount - 1);
			int row = rowType != CellOffsetType.Position ? offset.Row & (XlsDefs.MaxRowCount - 1) : Math.Min(offset.Row, XlsDefs.MaxRowCount - 1);
			return new CellOffset(column, row, columnType, rowType);
		}
	}
	static class XlsXmlMapsHelper {
		const string xmlnsSpreadsheetML = "xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" ";
		public static string InsertSpreadsheetMLNameSpace(string content) {
			int pos = content.IndexOf('>');
			if(content.IndexOf(xmlnsSpreadsheetML, 0, pos) == -1)
				content = content.Insert(9, xmlnsSpreadsheetML);
			return content;
		}
		public static string RemoveSpreadsheetMLNameSpace(string content) {
			int pos = content.IndexOf('>');
			int xmlnsPos = content.IndexOf(xmlnsSpreadsheetML, 0, pos);
			if(xmlnsPos != -1)
				content = content.Remove(xmlnsPos, xmlnsSpreadsheetML.Length);
			return content;
		}
	}
	static class XlsRangeHelper {
		public static int ValueInRange(int value, int minValue, int maxValue) {
			if(value < minValue)
				value = minValue;
			if(value > maxValue)
				value = maxValue;
			return value;
		}
		public static CellRangeInfo GetCellRangeInfo(CellRangeBase range) {
			CellPosition topLeft = new CellPosition(range.TopLeft.Column, range.TopLeft.Row);
			CellPosition bottomRight = new CellPosition(Math.Min(range.BottomRight.Column, XlsDefs.MaxColumnCount - 1), Math.Min(range.BottomRight.Row, XlsDefs.MaxRowCount - 1));
			return new CellRangeInfo(topLeft, bottomRight);
		}
		public static XlsRef8 GetRef8(CellRangeBase range) {
			XlsRef8 result = new XlsRef8();
			result.FirstColumnIndex = range.TopLeft.Column;
			result.FirstRowIndex = range.TopLeft.Row;
			result.LastColumnIndex = Math.Min(range.BottomRight.Column, XlsDefs.MaxColumnCount - 1);
			result.LastRowIndex = Math.Min(range.BottomRight.Row, XlsDefs.MaxRowCount - 1);
			return result;
		}
	}
	static class XlsCommentsHelper {
		public static int CountInXlsCellRange(this CommentCollection comments) {
			int count = 0;
			foreach(Comment comment in comments) {
				if(comment.Reference.OutOfLimits()) continue;
				count++;
			}
			return count;
		}
	}
	static class XlsDefinedNameHelper {
		public static string ReplaceInvalidChars(string value) {
			if(string.IsNullOrEmpty(value))
				return string.Empty;
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < value.Length; i++) {
				char curChar = value[i];
				if(WorkbookDataContext.IsValidIndentifierChar(curChar, i, value))
					sb.Append(curChar);
				else
					sb.Append('_');
			}
			return sb.ToString();
		}
	}
	#region Xls parsed things compliance
	public static class XlsParsedThingsExtensions {
		#region NamedParsedFormula incompliant
		static HashSet<Type> namedParsedFormulaIncompliant = CreateNamedParsedFormulaIncompliant();
		static HashSet<Type> CreateNamedParsedFormulaIncompliant() {
			HashSet<Type> result = new HashSet<Type>();
			result.Add(typeof(ParsedThingExp));
			result.Add(typeof(ParsedThingDataTable));
			result.Add(typeof(ParsedThingElfLel));
			result.Add(typeof(ParsedThingElfRw));
			result.Add(typeof(ParsedThingElfCol));
			result.Add(typeof(ParsedThingElfRwV));
			result.Add(typeof(ParsedThingElfColV));
			result.Add(typeof(ParsedThingElfRadical));
			result.Add(typeof(ParsedThingElfRadicalS));
			result.Add(typeof(ParsedThingElfColS));
			result.Add(typeof(ParsedThingElfColSV));
			result.Add(typeof(ParsedThingElfRadicalLel));
			result.Add(typeof(ParsedThingSxName));
			result.Add(typeof(ParsedThingRef));
			result.Add(typeof(ParsedThingRefRel));
			result.Add(typeof(ParsedThingRefErr));
			result.Add(typeof(ParsedThingArea));
			result.Add(typeof(ParsedThingAreaN));
			result.Add(typeof(ParsedThingAreaErr));
			return result;
		}
		#endregion
		#region CellParsedFormula incompliant
		static HashSet<Type> cellParsedFormulaIncompliant = CreateCellParsedFormulaIncompliant();
		static HashSet<Type> CreateCellParsedFormulaIncompliant() {
			HashSet<Type> result = new HashSet<Type>();
			result.Add(typeof(ParsedThingSxName));
			result.Add(typeof(ParsedThingRefRel));
			result.Add(typeof(ParsedThingAreaN));
			return result;
		}
		#endregion
		#region ArrayParsedFormula incompliant
		static HashSet<Type> arrayParsedFormulaIncompliant = CreateArrayParsedFormulaIncompliant();
		static HashSet<Type> CreateArrayParsedFormulaIncompliant() {
			HashSet<Type> result = new HashSet<Type>();
			result.Add(typeof(ParsedThingExp));
			result.Add(typeof(ParsedThingDataTable));
			result.Add(typeof(ParsedThingSxName));
			result.Add(typeof(ParsedThingRefRel));
			result.Add(typeof(ParsedThingAreaN));
			return result;
		}
		#endregion
		#region SharedParsedFormula incompliant
		static HashSet<Type> sharedParsedFormulaIncompliant = CreateSharedParsedFormulaIncompliant();
		static HashSet<Type> CreateSharedParsedFormulaIncompliant() {
			HashSet<Type> result = new HashSet<Type>();
			result.Add(typeof(ParsedThingExp));
			result.Add(typeof(ParsedThingDataTable));
			result.Add(typeof(ParsedThingSxName));
			result.Add(typeof(ParsedThingIntersect));
			result.Add(typeof(ParsedThingUnion));
			result.Add(typeof(ParsedThingRange));
			result.Add(typeof(ParsedThingArray));
			result.Add(typeof(ParsedThingElfLel));
			result.Add(typeof(ParsedThingElfRw));
			result.Add(typeof(ParsedThingElfCol));
			result.Add(typeof(ParsedThingElfRwV));
			result.Add(typeof(ParsedThingElfColV));
			result.Add(typeof(ParsedThingElfRadical));
			result.Add(typeof(ParsedThingElfRadicalS));
			result.Add(typeof(ParsedThingElfColS));
			result.Add(typeof(ParsedThingElfColSV));
			result.Add(typeof(ParsedThingElfRadicalLel));
			result.Add(typeof(ParsedThingRefErr));
			result.Add(typeof(ParsedThingAreaErr));
			result.Add(typeof(ParsedThingRef3d));
			result.Add(typeof(ParsedThingArea3d));
			result.Add(typeof(ParsedThingRef3dRel));
			result.Add(typeof(ParsedThingArea3dRel));
			result.Add(typeof(ParsedThingErr3d));
			result.Add(typeof(ParsedThingAreaErr3d));
			result.Add(typeof(ParsedThingNameX));
			result.Add(typeof(ParsedThingMemArea));
			result.Add(typeof(ParsedThingMemErr));
			result.Add(typeof(ParsedThingMemNoMem));
			result.Add(typeof(ParsedThingMemFunc));
			return result;
		}
		#endregion
		#region ChartParsedFormula compliant
		static HashSet<Type> chartParsedFormulaCompliant = CreateChartParsedFormulaCompliant();
		static HashSet<Type> CreateChartParsedFormulaCompliant() {
			HashSet<Type> result = new HashSet<Type>();
			result.Add(typeof(ParsedThingParentheses));
			result.Add(typeof(ParsedThingUnion));
			result.Add(typeof(ParsedThingRef3d));
			result.Add(typeof(ParsedThingErr3d));
			result.Add(typeof(ParsedThingArea3d));
			result.Add(typeof(ParsedThingAreaErr3d));
			result.Add(typeof(ParsedThingNameX));
			result.Add(typeof(ParsedThingMemFunc));
			return result;
		}
		#endregion
		#region CFParsedFormula incompliant
		static HashSet<Type> cfParsedFormulaIncompliant = CreateCFParsedFormulaIncompliant();
		static HashSet<Type> CreateCFParsedFormulaIncompliant() {
			HashSet<Type> result = new HashSet<Type>();
			result.Add(typeof(ParsedThingExp));
			result.Add(typeof(ParsedThingDataTable));
			result.Add(typeof(ParsedThingSxName));
			result.Add(typeof(ParsedThingIntersect));
			result.Add(typeof(ParsedThingUnion));
			result.Add(typeof(ParsedThingArray));
			result.Add(typeof(ParsedThingElfLel));
			result.Add(typeof(ParsedThingElfRw));
			result.Add(typeof(ParsedThingElfCol));
			result.Add(typeof(ParsedThingElfRwV));
			result.Add(typeof(ParsedThingElfColV));
			result.Add(typeof(ParsedThingElfRadical));
			result.Add(typeof(ParsedThingElfRadicalS));
			result.Add(typeof(ParsedThingElfColS));
			result.Add(typeof(ParsedThingElfColSV));
			result.Add(typeof(ParsedThingElfRadicalLel));
			result.Add(typeof(ParsedThingRef3d));
			result.Add(typeof(ParsedThingRef3dRel));
			result.Add(typeof(ParsedThingArea3d));
			result.Add(typeof(ParsedThingArea3dRel));
			result.Add(typeof(ParsedThingErr3d));
			result.Add(typeof(ParsedThingAreaErr3d));
			result.Add(typeof(ParsedThingNameX));
			result.Add(typeof(ParsedThingMemArea));
			result.Add(typeof(ParsedThingMemNoMem));
			return result;
		}
		#endregion
		#region DVParsedFormula incompliant
		static HashSet<Type> dvParsedFormulaIncompliant = CreateDVParsedFormulaIncompliant();
		static HashSet<Type> CreateDVParsedFormulaIncompliant() {
			HashSet<Type> result = new HashSet<Type>();
			result.Add(typeof(ParsedThingExp));
			result.Add(typeof(ParsedThingDataTable));
			result.Add(typeof(ParsedThingSxName));
			result.Add(typeof(ParsedThingIntersect));
			result.Add(typeof(ParsedThingUnion));
			result.Add(typeof(ParsedThingArray));
			result.Add(typeof(ParsedThingElfLel));
			result.Add(typeof(ParsedThingElfRw));
			result.Add(typeof(ParsedThingElfCol));
			result.Add(typeof(ParsedThingElfRwV));
			result.Add(typeof(ParsedThingElfColV));
			result.Add(typeof(ParsedThingElfRadical));
			result.Add(typeof(ParsedThingElfRadicalS));
			result.Add(typeof(ParsedThingElfColS));
			result.Add(typeof(ParsedThingElfColSV));
			result.Add(typeof(ParsedThingElfRadicalLel));
			result.Add(typeof(ParsedThingRef3d));
			result.Add(typeof(ParsedThingRef3dRel));
			result.Add(typeof(ParsedThingArea3d));
			result.Add(typeof(ParsedThingArea3dRel));
			result.Add(typeof(ParsedThingErr3d));
			result.Add(typeof(ParsedThingAreaErr3d));
			result.Add(typeof(ParsedThingNameX));
			result.Add(typeof(ParsedThingMemArea));
			result.Add(typeof(ParsedThingMemNoMem));
			return result;
		}
		#endregion
		public static bool IsXlsSharedFormulaCompliant(this ParsedExpression things) {
			for(int i = 0; i < things.Count; i++) {
				IParsedThing ptg = things[i];
				Type ptgType = ptg.GetType();
				if(sharedParsedFormulaIncompliant.Contains(ptgType))
					return false;
				if(Object.ReferenceEquals(ptgType, typeof(ParsedThingRef))) {
					ParsedThingRef ptgRef = ptg as ParsedThingRef;
					if(ptgRef.Position.ColumnType == PositionType.Relative) return false;
					if(ptgRef.Position.RowType == PositionType.Relative) return false;
				}
				if(Object.ReferenceEquals(ptgType, typeof(ParsedThingArea))) {
					ParsedThingArea ptgArea = ptg as ParsedThingArea;
					if(ptgArea.TopLeft.ColumnType == PositionType.Relative) return false;
					if(ptgArea.TopLeft.RowType == PositionType.Relative) return false;
					if(ptgArea.BottomRight.ColumnType == PositionType.Relative) return false;
					if(ptgArea.BottomRight.RowType == PositionType.Relative) return false;
				}
				if(Object.ReferenceEquals(ptgType, typeof(ParsedThingFuncVar))) {
					ParsedThingFuncVar ptgFuncVar = ptg as ParsedThingFuncVar;
					if(ptgFuncVar.FuncCode == 0x017b) return false;
				}
			}
			return true;
		}
		public static bool IsXlsArrayFormulaCompliant(this ParsedExpression things) {
			return IsCompliant(things, arrayParsedFormulaIncompliant);
		}
		public static bool IsXlsNamedFormulaCompliant(this ParsedExpression things) {
			return IsCompliant(things, namedParsedFormulaIncompliant);
		}
		public static bool IsXlsCellFormulaCompliant(this ParsedExpression things) {
			return IsCompliant(things, cellParsedFormulaIncompliant);
		}
		public static bool IsXlsChartFormulaCompliant(this ParsedExpression things) {
			return IsNotCompliant(things, chartParsedFormulaCompliant);
		}
		public static bool IsXlsCFFormulaCompliant(this ParsedExpression things) {
			return IsCompliant(things, cfParsedFormulaIncompliant);
		}
		public static bool IsXlsDVFormulaCompliant(this ParsedExpression things) {
			return IsCompliant(things, dvParsedFormulaIncompliant);
		}
		static bool IsCompliant(this ParsedExpression things, HashSet<Type> incompliant) {
			for(int i = 0; i < things.Count; i++) {
				Type ptgType = things[i].GetType();
				if(incompliant.Contains(ptgType))
					return false;
			}
			return true;
		}
		static bool IsNotCompliant(this ParsedExpression things, HashSet<Type> compliant) {
			for (int i = 0; i < things.Count; i++) {
				Type ptgType = things[i].GetType();
				if (!compliant.Contains(ptgType))
					return false;
			}
			return true;
		}
	}
	public static class XlsSharedFormulaExtensions {
		public static bool IsCompliant(this SharedFormula sharedFormula, WorkbookDataContext context) {
			ParsedExpression expression = sharedFormula.Expression;
			return expression.IsXlsSharedFormulaCompliant();
		}
	}
	#endregion
	#region Xls parsed expression validation
	public static class XlsParsedExpressionValidator {
		public static bool IsValidExpression(this ParsedExpression things, XlsContentBuilder contentBuilder) {
			return IsValidExpression(things, contentBuilder, contentBuilder.Options.ValidateFormula);
		}
		public static bool IsValidExpression(this ParsedExpression things, XlsContentBuilder contentBuilder, bool validate) {
			if (!validate)
				return true;
			try {
				WorkbookDataContext context = contentBuilder.RPNContext.WorkbookContext;
				string formula = things.BuildExpressionString(context);
				ParsedExpression expression = context.ParseExpression(formula, context.DefinedNameProcessing ? OperandDataType.Default : OperandDataType.Value, false);
				return expression != null;
			}
			catch { }
			return false;
		}
		public static bool IsValidExpression(this SharedFormula sharedFormula, XlsContentBuilder contentBuilder) {
			if(!contentBuilder.Options.ValidateFormula)
				return true;
			try {
				WorkbookDataContext context = contentBuilder.RPNContext.WorkbookContext;
				ParsedExpression expression = sharedFormula.Expression;
				string formula = expression.BuildExpressionString(context);
				expression = context.ParseExpression(formula, context.DefinedNameProcessing ? OperandDataType.Default : OperandDataType.Value, false);
				return expression != null;
			}
			catch { }
			return false;
		}
	}
	#endregion
}
