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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Export.Xls {
	#region XlsCFExportHelper
	public static class XlsCFExportHelper {
		#region Function codes
		static int funcAnd = FormulaCalculator.GetFunctionByInvariantName("AND").Code;
		static int funcAverage = FormulaCalculator.GetFunctionByInvariantName("AVERAGE").Code;
		static int funcCount = FormulaCalculator.GetFunctionByInvariantName("COUNT").Code;
		static int funcCountIf = FormulaCalculator.GetFunctionByInvariantName("COUNTIF").Code;
		static int funcIf = FormulaCalculator.GetFunctionByInvariantName("IF").Code;
		static int funcIsBlank = FormulaCalculator.GetFunctionByInvariantName("ISBLANK").Code;
		static int funcIsError = FormulaCalculator.GetFunctionByInvariantName("ISERROR").Code;
		static int funcLarge = FormulaCalculator.GetFunctionByInvariantName("LARGE").Code;
		static int funcLeft = FormulaCalculator.GetFunctionByInvariantName("LEFT").Code;
		static int funcLen = FormulaCalculator.GetFunctionByInvariantName("LEN").Code;
		static int funcMax = FormulaCalculator.GetFunctionByInvariantName("MAX").Code;
		static int funcMin = FormulaCalculator.GetFunctionByInvariantName("MIN").Code;
		static int funcNot = FormulaCalculator.GetFunctionByInvariantName("NOT").Code;
		static int funcRight = FormulaCalculator.GetFunctionByInvariantName("RIGHT").Code;
		static int funcRoundDown = FormulaCalculator.GetFunctionByInvariantName("ROUNDDOWN").Code;
		static int funcSearch = FormulaCalculator.GetFunctionByInvariantName("SEARCH").Code;
		static int funcSmall = FormulaCalculator.GetFunctionByInvariantName("SMALL").Code;
		static int funcStDevP = FormulaCalculator.GetFunctionByInvariantName("STDEVP").Code;
		static int funcTrim = FormulaCalculator.GetFunctionByInvariantName("TRIM").Code;
		static int funcInt = FormulaCalculator.GetFunctionByInvariantName("INT").Code;
		static int funcFloor = FormulaCalculator.GetFunctionByInvariantName("FLOOR").Code;
		static int funcToday = FormulaCalculator.GetFunctionByInvariantName("TODAY").Code;
		static int funcWeekday = FormulaCalculator.GetFunctionByInvariantName("WEEKDAY").Code;
		static int funcMonth = FormulaCalculator.GetFunctionByInvariantName("MONTH").Code;
		static int funcYear = FormulaCalculator.GetFunctionByInvariantName("YEAR").Code;
		#endregion
		static ParsedThingRefRel ptgRefN = new ParsedThingRefRel(new CellOffset(0, 0, CellOffsetType.Offset, CellOffsetType.Offset)) { DataType = OperandDataType.Value };
		static ParsedThingInteger ptgZero = new ParsedThingInteger() { Value = 0 };
		static ParsedThingInteger ptgOne = new ParsedThingInteger() { Value = 1 };
		static ParsedThingAttrSpace ptgAttrSpace = new ParsedThingAttrSpace(ParsedThingAttrSpaceType.SpaceBeforeBaseExpression, 1);
		public static bool IsNotCF12(ConditionalFormatting cf) {
			FormulaConditionalFormatting cfRule = cf as FormulaConditionalFormatting;
			if (cfRule == null)
				return false;
			DifferentialFormat format = cfRule.DifferentialFormatInfo;
			if (format.MultiOptionsInfo.ApplyNumberFormat)
				return false;
			if ((cf is RankFormulaConditionalFormatting) && (cf.CellRange.RangeType == CellRangeType.UnionRange))
				return false;
			AverageFormulaConditionalFormatting avgConditionalFormatting = cf as AverageFormulaConditionalFormatting;
			if ((avgConditionalFormatting != null) && (avgConditionalFormatting.StdDev != 0) && (cf.CellRange.RangeType == CellRangeType.UnionRange))
				return false;
			return true;
		}
		public static bool HasFormulaInCF12(ConditionalFormatting cf) {
			RankFormulaConditionalFormatting cfRule = cf as RankFormulaConditionalFormatting;
			if (cfRule != null )
				return false;
			return true;
		}
		#region GetRuleFormula methods
		public static ParsedExpression GetRuleFormula(this TextFormulaConditionalFormatting cf) {
			ParsedExpression result = new ParsedExpression();
			ParsedThingStringValue ptgStr = new ParsedThingStringValue(cf.Value);
			switch (cf.Condition) {
				case ConditionalFormattingTextCondition.BeginsWith:
					result.Add(ptgRefN);
					result.Add(ptgStr);
					result.Add(new ParsedThingFunc(funcLen, OperandDataType.Value));
					result.Add(new ParsedThingFuncVar(funcLeft, 2, OperandDataType.Value));
					result.Add(ptgStr);
					result.Add(ParsedThingEqual.Instance);
					break;
				case ConditionalFormattingTextCondition.Contains:
					result.Add(ptgStr);
					result.Add(ptgRefN);
					result.Add(new ParsedThingFuncVar(funcSearch, 2, OperandDataType.Value));
					result.Add(new ParsedThingFunc(funcIsError, OperandDataType.Value));
					result.Add(new ParsedThingFunc(funcNot, OperandDataType.Value));
					break;
				case ConditionalFormattingTextCondition.EndsWith:
					result.Add(ptgRefN);
					result.Add(ptgStr);
					result.Add(new ParsedThingFunc(funcLen, OperandDataType.Value));
					result.Add(new ParsedThingFuncVar(funcRight, 2, OperandDataType.Value));
					result.Add(ptgStr);
					result.Add(ParsedThingEqual.Instance);
					break;
				case ConditionalFormattingTextCondition.NotContains:
					result.Add(ptgStr);
					result.Add(ptgRefN);
					result.Add(new ParsedThingFuncVar(funcSearch, 2, OperandDataType.Value));
					result.Add(new ParsedThingFunc(funcIsError, OperandDataType.Value));
					break;
			}
			return result;
		}
		public static ParsedExpression GetRuleFormula(this SpecialFormulaConditionalFormatting cf) {
			ParsedExpression result = new ParsedExpression();
			switch(cf.Condition) {
				case ConditionalFormattingSpecialCondition.ContainBlanks:
					result.Add(ptgRefN);
					result.Add(new ParsedThingFunc(funcTrim, OperandDataType.Value));
					result.Add(new ParsedThingFunc(funcLen, OperandDataType.Value));
					result.Add(ptgZero);
					result.Add(ParsedThingEqual.Instance);
					break;
				case ConditionalFormattingSpecialCondition.ContainNonBlanks:
					result.Add(ptgRefN);
					result.Add(new ParsedThingFunc(funcTrim, OperandDataType.Value));
					result.Add(new ParsedThingFunc(funcLen, OperandDataType.Value));
					result.Add(ptgZero);
					result.Add(ParsedThingGreater.Instance);
					break;
				case ConditionalFormattingSpecialCondition.ContainError:
					result.Add(ptgRefN);
					result.Add(new ParsedThingFunc(funcIsError, OperandDataType.Value));
					break;
				case ConditionalFormattingSpecialCondition.NotContainError:
					result.Add(ptgRefN);
					result.Add(new ParsedThingFunc(funcIsError, OperandDataType.Value));
					result.Add(new ParsedThingFunc(funcNot, OperandDataType.Value));
					break;
				case ConditionalFormattingSpecialCondition.ContainUniqueValue:
					CreateUniqueDuplicatesExpression(result, cf.CellRange, true);
					break;
				case ConditionalFormattingSpecialCondition.ContainDuplicateValue:
					CreateUniqueDuplicatesExpression(result, cf.CellRange, false);
					break;
			}
			return result;
		}
		public static ParsedExpression GetRuleFormula(this RankFormulaConditionalFormatting cf) {
			ParsedExpression result = new ParsedExpression();
			if(cf.CellRange.RangeType == CellRangeType.UnionRange)
				return result;
			ParsedThingArea ptgArea = new ParsedThingArea(cf.CellRange.GetWithModifiedPositionType(PositionType.Absolute) as CellRange);
			switch(cf.Condition) {
				case ConditionalFormattingRankCondition.TopByRank:
					result.Add(ptgArea);
					result.Add(ParsedThingParentheses.Instance);
					result.Add(ptgAttrSpace);
					result.Add(new ParsedThingInteger() { Value = cf.Rank });
					result.Add(ptgArea);
					result.Add(new ParsedThingFuncVar(funcCount, 1, OperandDataType.Value));
					result.Add(new ParsedThingFuncVar(funcMin, 2, OperandDataType.Value));
					result.Add(new ParsedThingFunc(funcLarge, OperandDataType.Value));
					result.Add(ptgRefN);
					result.Add(ParsedThingLessEqual.Instance);
					break;
				case ConditionalFormattingRankCondition.BottomByRank:
					result.Add(ptgArea);
					result.Add(ParsedThingParentheses.Instance);
					result.Add(ptgAttrSpace);
					result.Add(new ParsedThingInteger() { Value = cf.Rank });
					result.Add(ptgArea);
					result.Add(new ParsedThingFuncVar(funcCount, 1, OperandDataType.Value));
					result.Add(new ParsedThingFuncVar(funcMin, 2, OperandDataType.Value));
					result.Add(new ParsedThingFunc(funcSmall, OperandDataType.Value));
					result.Add(ptgRefN);
					result.Add(ParsedThingGreaterEqual.Instance);
					break;
				case ConditionalFormattingRankCondition.TopByPercent:
					result.Add(ptgArea);
					result.Add(new ParsedThingFuncVar(funcCount, 1, OperandDataType.Value));
					result.Add(new ParsedThingInteger() { Value = cf.Rank });
					result.Add(ParsedThingPercent.Instance);
					result.Add(ParsedThingMultiply.Instance);
					result.Add(new ParsedThingFunc(funcInt, OperandDataType.Value));
					result.Add(ptgZero);
					result.Add(ParsedThingGreater.Instance);
					result.Add(new ParsedThingAttrIf() { Offset = 9 });
					result.Add(ptgArea);
					result.Add(ptgArea);
					result.Add(new ParsedThingFuncVar(funcCount, 1, OperandDataType.Value));
					result.Add(new ParsedThingInteger() { Value = cf.Rank });
					result.Add(ParsedThingPercent.Instance);
					result.Add(ParsedThingMultiply.Instance);
					result.Add(new ParsedThingFunc(funcInt, OperandDataType.Value));
					result.Add(new ParsedThingFunc(funcLarge, OperandDataType.Value));
					result.Add(new ParsedThingAttrGoto(5));
					result.Add(ptgAttrSpace);
					result.Add(ptgArea);
					result.Add(new ParsedThingFuncVar(funcMax, 1, OperandDataType.Value));
					result.Add(new ParsedThingAttrGoto(1));
					result.Add(new ParsedThingFuncVar(funcIf, 3, OperandDataType.Value));
					result.Add(ptgRefN);
					result.Add(ParsedThingLessEqual.Instance);
					break;
				case ConditionalFormattingRankCondition.BottomByPercent:
					result.Add(ptgArea);
					result.Add(new ParsedThingFuncVar(funcCount, 1, OperandDataType.Value));
					result.Add(new ParsedThingInteger() { Value = cf.Rank });
					result.Add(ParsedThingPercent.Instance);
					result.Add(ParsedThingMultiply.Instance);
					result.Add(new ParsedThingFunc(funcInt, OperandDataType.Value));
					result.Add(ptgZero);
					result.Add(ParsedThingGreater.Instance);
					result.Add(new ParsedThingAttrIf() { Offset = 9 });
					result.Add(ptgArea);
					result.Add(ptgArea);
					result.Add(new ParsedThingFuncVar(funcCount, 1, OperandDataType.Value));
					result.Add(new ParsedThingInteger() { Value = cf.Rank });
					result.Add(ParsedThingPercent.Instance);
					result.Add(ParsedThingMultiply.Instance);
					result.Add(new ParsedThingFunc(funcInt, OperandDataType.Value));
					result.Add(new ParsedThingFunc(funcSmall, OperandDataType.Value));
					result.Add(new ParsedThingAttrGoto(5));
					result.Add(ptgAttrSpace);
					result.Add(ptgArea);
					result.Add(new ParsedThingFuncVar(funcMin, 1, OperandDataType.Value));
					result.Add(new ParsedThingAttrGoto(1));
					result.Add(new ParsedThingFuncVar(funcIf, 3, OperandDataType.Value));
					result.Add(ptgRefN);
					result.Add(ParsedThingGreaterEqual.Instance);
					break;
			}
			return result;
		}
		public static ParsedExpression GetRuleFormula(this AverageFormulaConditionalFormatting cf) {
			ParsedExpression result = new ParsedExpression();
			result.Add(ptgRefN);
			CellRangeBase cellRange = cf.CellRange;
			if (cf.StdDev == 0) {
				if (cellRange.RangeType == CellRangeType.UnionRange) {
					CellUnion union = cellRange as CellUnion;
					int count = union.InnerCellRanges.Count;
					for (int i = 0; i < count; i++) {
						CellRangeBase part = union.InnerCellRanges[i];
						CreateAverageExpression(result, part as CellRange);
					}
					result.Add(new ParsedThingFuncVar(funcAverage, count, OperandDataType.Value));
				}
				else {
					CreateAverageExpression(result, cellRange as CellRange);
					result.Add(new ParsedThingFuncVar(funcAverage, 1, OperandDataType.Value));
				}
			}
			else {
				BasicExpressionCreator.CreateCellRangeExpression(result, cellRange, BasicExpressionCreatorParameter.None, OperandDataType.Default, cf.DocumentModel.DataContext);
				result.Add(new ParsedThingFuncVar(funcAverage, 1, OperandDataType.Value));
				result.Add(ParsedThingSubtract.Instance);
				result.Add(ParsedThingParentheses.Instance);
				BasicExpressionCreator.CreateCellRangeExpression(result, cellRange, BasicExpressionCreatorParameter.None, OperandDataType.Default, cf.DocumentModel.DataContext);
				result.Add(new ParsedThingFuncVar(funcStDevP, 1, OperandDataType.Value));
				if (cf.Condition == ConditionalFormattingAverageCondition.Below || cf.Condition == ConditionalFormattingAverageCondition.BelowOrEqual)
					result.Add(new ParsedThingNumeric() { Value = -cf.StdDev });
				else
					result.Add(new ParsedThingInteger() { Value = cf.StdDev });
				result.Add(ParsedThingParentheses.Instance);
				result.Add(ParsedThingMultiply.Instance);
			}
			CreateAverageComparison(result, cf.Condition, cf.StdDev);
			return result;
		}
		public static ParsedExpression GetRuleFormula(this TimePeriodFormulaConditionalFormatting cf) {
			ParsedExpression result = new ParsedExpression();
			switch(cf.TimePeriod) {
				case ConditionalFormattingTimePeriod.Today:
				case ConditionalFormattingTimePeriod.Yesterday:
				case ConditionalFormattingTimePeriod.Tomorrow:
					CreateDayExpression(result, cf.TimePeriod);
					break;
				case ConditionalFormattingTimePeriod.Last7Days:
					CreateLast7DaysExpression(result);
					break;
				case ConditionalFormattingTimePeriod.LastWeek:
					CreateLastWeekExpression(result);
					break;
				case ConditionalFormattingTimePeriod.ThisWeek:
					CreateThisWeekExpression(result);
					break;
				case ConditionalFormattingTimePeriod.NextWeek:
					CreateNextWeekExpression(result);
					break;
				case ConditionalFormattingTimePeriod.LastMonth:
					CreateLastMonthExpression(result);
					break;
				case ConditionalFormattingTimePeriod.ThisMonth:
					CreateThisMonthExpression(result);
					break;
				case ConditionalFormattingTimePeriod.NextMonth:
					CreateNextMonthExpression(result);
					break;
			}
			return result;
		}
		#endregion
		#region GetRuleTemplate methods
		public static XlsCFRuleTemplate GetRuleTemplate(this SpecialFormulaConditionalFormatting comparer) {
			switch(comparer.Condition) {
				case ConditionalFormattingSpecialCondition.ContainBlanks:
					return XlsCFRuleTemplate.ContainsBlanks;
				case ConditionalFormattingSpecialCondition.ContainNonBlanks:
					return XlsCFRuleTemplate.ContainsNoBlanks;
				case ConditionalFormattingSpecialCondition.ContainError:
					return XlsCFRuleTemplate.ContainsErrors;
				case ConditionalFormattingSpecialCondition.NotContainError:
					return XlsCFRuleTemplate.ContainsNoError;
				case ConditionalFormattingSpecialCondition.ContainDuplicateValue:
					return XlsCFRuleTemplate.DuplicateValues;
				case ConditionalFormattingSpecialCondition.ContainUniqueValue:
					return XlsCFRuleTemplate.UniqueValues;
			}
			return XlsCFRuleTemplate.Formula;
		}
		public static XlsCFRuleTemplate GetRuleTemplate(this AverageFormulaConditionalFormatting comparer) {
			switch(comparer.Condition) {
				case ConditionalFormattingAverageCondition.Above:
					return XlsCFRuleTemplate.AboveAverage;
				case ConditionalFormattingAverageCondition.AboveOrEqual:
					return XlsCFRuleTemplate.AboveOrEqualToAverage;
				case ConditionalFormattingAverageCondition.Below:
					return XlsCFRuleTemplate.BelowAverage;
				case ConditionalFormattingAverageCondition.BelowOrEqual:
					return XlsCFRuleTemplate.BelowOrEqualToAverage;
			}
			return XlsCFRuleTemplate.Formula;
		}
		public static XlsCFRuleTemplate GetRuleTemplate(this TimePeriodFormulaConditionalFormatting comparer) {
			switch(comparer.TimePeriod) {
				case ConditionalFormattingTimePeriod.Last7Days:
					return XlsCFRuleTemplate.Last7Days;
				case ConditionalFormattingTimePeriod.LastMonth:
					return XlsCFRuleTemplate.LastMonth;
				case ConditionalFormattingTimePeriod.LastWeek:
					return XlsCFRuleTemplate.LastWeek;
				case ConditionalFormattingTimePeriod.NextMonth:
					return XlsCFRuleTemplate.NextMonth;
				case ConditionalFormattingTimePeriod.NextWeek:
					return XlsCFRuleTemplate.NextWeek;
				case ConditionalFormattingTimePeriod.ThisMonth:
					return XlsCFRuleTemplate.ThisMonth;
				case ConditionalFormattingTimePeriod.ThisWeek:
					return XlsCFRuleTemplate.ThisWeek;
				case ConditionalFormattingTimePeriod.Today:
					return XlsCFRuleTemplate.Today;
				case ConditionalFormattingTimePeriod.Tomorrow:
					return XlsCFRuleTemplate.Tomorrow;
				case ConditionalFormattingTimePeriod.Yesterday:
					return XlsCFRuleTemplate.Yesterday;
			}
			return XlsCFRuleTemplate.Formula;
		}
		#endregion
		#region Internals
		static void CreateUniqueDuplicatesExpression(ParsedExpression result, CellRangeBase cellRange, bool isUnique) {
			if(cellRange.RangeType == CellRangeType.UnionRange) {
				CellUnion union = cellRange as CellUnion;
				int count = union.InnerCellRanges.Count;
				for(int i = 0; i < count; i++) {
					CellRangeBase part = union.InnerCellRanges[i];
					result.Add(new ParsedThingArea(part.GetWithModifiedPositionType(PositionType.Absolute) as CellRange));
					result.Add(ptgAttrSpace);
					result.Add(ptgRefN);
					result.Add(new ParsedThingFunc(funcCountIf, OperandDataType.Value));
				}
				for(int i = 0; i < (count - 1); i++)
					result.Add(ParsedThingAdd.Instance);
			}
			else {
				result.Add(new ParsedThingArea(cellRange.GetWithModifiedPositionType(PositionType.Absolute) as CellRange));
				result.Add(ptgAttrSpace);
				result.Add(ptgRefN);
				result.Add(new ParsedThingFunc(funcCountIf, OperandDataType.Value));
			}
			result.Add(ptgOne);
			if(isUnique)
				result.Add(ParsedThingEqual.Instance);
			else
				result.Add(ParsedThingGreater.Instance);
			result.Add(ptgRefN);
			result.Add(new ParsedThingFunc(funcIsBlank, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcNot, OperandDataType.Value));
			result.Add(new ParsedThingFuncVar(funcAnd, 2, OperandDataType.Value));
		}
		static void CreateAverageExpression(ParsedExpression result, CellRange cellRange) {
			ParsedThingArea ptgArea = new ParsedThingArea(cellRange) { DataType = OperandDataType.Array };
			result.Add(ptgArea);
			result.Add(new ParsedThingFunc(funcIsError, OperandDataType.Array));
			result.Add(new ParsedThingAttrIf() { Offset = 3 });
			result.Add(ptgAttrSpace);
			result.Add(new ParsedThingStringValue(string.Empty));
			result.Add(new ParsedThingAttrGoto(14));
			result.Add(ptgArea);
			result.Add(new ParsedThingFunc(funcIsBlank, OperandDataType.Array));
			result.Add(new ParsedThingAttrIf() { Offset = 3 });
			result.Add(ptgAttrSpace);
			result.Add(new ParsedThingStringValue(string.Empty));
			result.Add(new ParsedThingAttrGoto(5));
			result.Add(ptgAttrSpace);
			result.Add(ptgArea);
			result.Add(ptgAttrSpace);
			result.Add(new ParsedThingAttrGoto(1));
			result.Add(new ParsedThingFuncVar(funcIf, 3, OperandDataType.Reference));
			result.Add(ptgAttrSpace);
			result.Add(new ParsedThingAttrGoto(1));
			result.Add(new ParsedThingFuncVar(funcIf, 3, OperandDataType.Reference));
		}
		static void CreateAverageComparison(ParsedExpression result, ConditionalFormattingAverageCondition condition, int stdDev) {
			switch(condition) {
				case ConditionalFormattingAverageCondition.Above:
					result.Add(stdDev == 0 ? (IParsedThing)ParsedThingGreater.Instance : (IParsedThing)ParsedThingGreaterEqual.Instance);
					break;
				case ConditionalFormattingAverageCondition.AboveOrEqual:
					result.Add(ParsedThingGreaterEqual.Instance);
					break;
				case ConditionalFormattingAverageCondition.Below:
					result.Add(stdDev == 0 ? (IParsedThing)ParsedThingLess.Instance : (IParsedThing)ParsedThingLessEqual.Instance);
					break;
				case ConditionalFormattingAverageCondition.BelowOrEqual:
					result.Add(ParsedThingLessEqual.Instance);
					break;
			}
		}
		static void CreateNextMonthExpression(ParsedExpression result) {
			result.Add(new ParsedThingAttrSemi());
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcMonth, OperandDataType.Value));
			result.Add(new ParsedThingInteger() { Value = 12 });
			result.Add(ParsedThingEqual.Instance);
			result.Add(new ParsedThingAttrIf() { Offset = 14 });
			result.Add(ptgRefN);
			result.Add(new ParsedThingFunc(funcMonth, OperandDataType.Value));
			result.Add(ptgOne);
			result.Add(ParsedThingEqual.Instance);
			result.Add(ptgRefN);
			result.Add(new ParsedThingFunc(funcYear, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcYear, OperandDataType.Value));
			result.Add(ptgOne);
			result.Add(ParsedThingAdd.Instance);
			result.Add(ParsedThingParentheses.Instance);
			result.Add(ParsedThingEqual.Instance);
			result.Add(new ParsedThingFuncVar(funcAnd, 2, OperandDataType.Value));
			result.Add(new ParsedThingAttrGoto(16));
			result.Add(ptgRefN);
			result.Add(new ParsedThingFunc(funcMonth, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcMonth, OperandDataType.Value));
			result.Add(ptgOne);
			result.Add(ParsedThingAdd.Instance);
			result.Add(ParsedThingParentheses.Instance);
			result.Add(ParsedThingEqual.Instance);
			result.Add(ptgRefN);
			result.Add(new ParsedThingFunc(funcYear, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcYear, OperandDataType.Value));
			result.Add(ParsedThingEqual.Instance);
			result.Add(new ParsedThingFuncVar(funcAnd, 2, OperandDataType.Value));
			result.Add(new ParsedThingAttrGoto(1));
			result.Add(new ParsedThingFuncVar(funcIf, 3, OperandDataType.Value));
		}
		static void CreateLastMonthExpression(ParsedExpression result) {
			result.Add(new ParsedThingAttrSemi());
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcMonth, OperandDataType.Value));
			result.Add(ptgOne);
			result.Add(ParsedThingEqual.Instance);
			result.Add(new ParsedThingAttrIf() { Offset = 14 });
			result.Add(ptgRefN);
			result.Add(new ParsedThingFunc(funcMonth, OperandDataType.Value));
			result.Add(new ParsedThingInteger() { Value = 12 });
			result.Add(ParsedThingEqual.Instance);
			result.Add(ptgRefN);
			result.Add(new ParsedThingFunc(funcYear, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcYear, OperandDataType.Value));
			result.Add(ptgOne);
			result.Add(ParsedThingSubtract.Instance);
			result.Add(ParsedThingParentheses.Instance);
			result.Add(ParsedThingEqual.Instance);
			result.Add(new ParsedThingFuncVar(funcAnd, 2, OperandDataType.Value));
			result.Add(new ParsedThingAttrGoto(16));
			result.Add(ptgRefN);
			result.Add(new ParsedThingFunc(funcMonth, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcMonth, OperandDataType.Value));
			result.Add(ptgOne);
			result.Add(ParsedThingSubtract.Instance);
			result.Add(ParsedThingParentheses.Instance);
			result.Add(ParsedThingEqual.Instance);
			result.Add(ptgRefN);
			result.Add(new ParsedThingFunc(funcYear, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcYear, OperandDataType.Value));
			result.Add(ParsedThingEqual.Instance);
			result.Add(new ParsedThingFuncVar(funcAnd, 2, OperandDataType.Value));
			result.Add(new ParsedThingAttrGoto(1));
			result.Add(new ParsedThingFuncVar(funcIf, 3, OperandDataType.Value));
		}
		static void CreateThisMonthExpression(ParsedExpression result) {
			result.Add(new ParsedThingAttrSemi());
			result.Add(ptgRefN);
			result.Add(new ParsedThingFunc(funcMonth, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcMonth, OperandDataType.Value));
			result.Add(ParsedThingEqual.Instance);
			result.Add(ptgRefN);
			result.Add(new ParsedThingFunc(funcYear, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcYear, OperandDataType.Value));
			result.Add(ParsedThingEqual.Instance);
			result.Add(new ParsedThingFuncVar(funcAnd, 2, OperandDataType.Value));
		}
		static void CreateNextWeekExpression(ParsedExpression result) {
			result.Add(new ParsedThingAttrSemi());
			result.Add(ptgRefN);
			result.Add(ptgZero);
			result.Add(new ParsedThingFunc(funcRoundDown, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			result.Add(ParsedThingSubtract.Instance);
			result.Add(new ParsedThingInteger() { Value = 7 });
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			result.Add(new ParsedThingFuncVar(funcWeekday, 1, OperandDataType.Reference));
			result.Add(ParsedThingSubtract.Instance);
			result.Add(ParsedThingParentheses.Instance);
			result.Add(ParsedThingGreater.Instance);
			result.Add(ptgRefN);
			result.Add(ptgZero);
			result.Add(new ParsedThingFunc(funcRoundDown, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			result.Add(ParsedThingSubtract.Instance);
			result.Add(new ParsedThingInteger() { Value = 15 });
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			result.Add(new ParsedThingFuncVar(funcWeekday, 1, OperandDataType.Reference));
			result.Add(ParsedThingSubtract.Instance);
			result.Add(ParsedThingParentheses.Instance);
			result.Add(ParsedThingLess.Instance);
			result.Add(new ParsedThingFuncVar(funcAnd, 2, OperandDataType.Value));
		}
		static void CreateThisWeekExpression(ParsedExpression result) {
			result.Add(new ParsedThingAttrSemi());
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			result.Add(ptgRefN);
			result.Add(ptgZero);
			result.Add(new ParsedThingFunc(funcRoundDown, OperandDataType.Value));
			result.Add(ParsedThingSubtract.Instance);
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			result.Add(new ParsedThingFuncVar(funcWeekday, 1, OperandDataType.Reference));
			result.Add(ptgOne);
			result.Add(ParsedThingSubtract.Instance);
			result.Add(ParsedThingLessEqual.Instance);
			result.Add(ptgRefN);
			result.Add(ptgZero);
			result.Add(new ParsedThingFunc(funcRoundDown, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			result.Add(ParsedThingSubtract.Instance);
			result.Add(new ParsedThingInteger() { Value = 7 });
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			result.Add(new ParsedThingFuncVar(funcWeekday, 1, OperandDataType.Reference));
			result.Add(ParsedThingSubtract.Instance);
			result.Add(ParsedThingLessEqual.Instance);
			result.Add(new ParsedThingFuncVar(funcAnd, 2, OperandDataType.Value));
		}
		static void CreateLastWeekExpression(ParsedExpression result) {
			result.Add(new ParsedThingAttrSemi());
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			result.Add(ptgRefN);
			result.Add(ptgZero);
			result.Add(new ParsedThingFunc(funcRoundDown, OperandDataType.Value));
			result.Add(ParsedThingSubtract.Instance);
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			result.Add(new ParsedThingFuncVar(funcWeekday, 1, OperandDataType.Reference));
			result.Add(ParsedThingParentheses.Instance);
			result.Add(ParsedThingGreaterEqual.Instance);
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			result.Add(ptgRefN);
			result.Add(ptgZero);
			result.Add(new ParsedThingFunc(funcRoundDown, OperandDataType.Value));
			result.Add(ParsedThingSubtract.Instance);
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			result.Add(new ParsedThingFuncVar(funcWeekday, 1, OperandDataType.Reference));
			result.Add(new ParsedThingInteger() { Value = 7 });
			result.Add(ParsedThingAdd.Instance);
			result.Add(ParsedThingParentheses.Instance);
			result.Add(ParsedThingLess.Instance);
			result.Add(new ParsedThingFuncVar(funcAnd, 2, OperandDataType.Value));
		}
		static void CreateLast7DaysExpression(ParsedExpression result) {
			result.Add(new ParsedThingAttrSemi());
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			result.Add(ptgRefN);
			result.Add(ptgOne);
			result.Add(new ParsedThingFunc(funcFloor, OperandDataType.Value));
			result.Add(ParsedThingSubtract.Instance);
			result.Add(new ParsedThingInteger() { Value = 6 });
			result.Add(ParsedThingLessEqual.Instance);
			result.Add(ptgRefN);
			result.Add(ptgOne);
			result.Add(new ParsedThingFunc(funcFloor, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			result.Add(ParsedThingLessEqual.Instance);
			result.Add(new ParsedThingFuncVar(funcAnd, 2, OperandDataType.Value));
		}
		static void CreateDayExpression(ParsedExpression result, ConditionalFormattingTimePeriod period) {
			result.Add(new ParsedThingAttrSemi());
			result.Add(ptgRefN);
			result.Add(ptgOne);
			result.Add(new ParsedThingFunc(funcFloor, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcToday, OperandDataType.Value));
			if(period != ConditionalFormattingTimePeriod.Today) {
				result.Add(ptgOne);
				if(period == ConditionalFormattingTimePeriod.Tomorrow)
					result.Add(ParsedThingAdd.Instance);
				else
					result.Add(ParsedThingSubtract.Instance);
			}
			result.Add(ParsedThingEqual.Instance);
		}
		#endregion
	}
	#endregion
	#region XlsCFExpressionsHelper
	public static class XlsCFExpressionsHelper {
		#region Expressions collector
		public static List<ParsedExpression> GetExpressions(this ConditionalFormatting cf, XlsRPNContext context) {
			List<ParsedExpression> result = new List<ParsedExpression>();
			result.AddExpressions(cf as FormulaConditionalFormatting, context);
			result.AddExpressions(cf as ColorScaleConditionalFormatting, context);
			result.AddExpressions(cf as DataBarConditionalFormatting, context);
			result.AddExpressions(cf as IconSetConditionalFormatting, context);
			return result;
		}
		static void AddExpressions(this List<ParsedExpression> result, FormulaConditionalFormatting cf, XlsRPNContext context) {
			if(cf == null) return;
			result.AddExpressions(cf as ExpressionFormulaConditionalFormatting, context);
			result.AddExpressions(cf as RangeFormulaConditionalFormatting, context);
		}
		static void AddExpressions(this List<ParsedExpression> result, ExpressionFormulaConditionalFormatting cf, XlsRPNContext context) {
			if(cf == null) return;
			ParsedExpression expression = GetParsedExpression(cf.Value, context);
			if(expression != null)
				result.Add(expression);
		}
		static void AddExpressions(this List<ParsedExpression> result, RangeFormulaConditionalFormatting cf, XlsRPNContext context) {
			if(cf == null) return;
			ParsedExpression expression = GetParsedExpression(cf.Value, context);
			if(expression != null)
				result.Add(expression);
			expression = GetParsedExpression(cf.Value2, context);
			if(expression != null)
				result.Add(expression);
		}
		static void AddExpressions(this List<ParsedExpression> result, ColorScaleConditionalFormatting cf, XlsRPNContext context) {
			if(cf == null) return;
			result.AddExpression(cf.LowPointValue, context);
			if(cf.ScaleType == ColorScaleType.Color3)
				result.AddExpression(cf.MiddlePointValue, context);
			result.AddExpression(cf.HighPointValue, context);
		}
		static void AddExpressions(this List<ParsedExpression> result, DataBarConditionalFormatting cf, XlsRPNContext context) {
			if(cf == null) return;
			result.AddExpression(cf.LowBound, context);
			result.AddExpression(cf.HighBound, context);
		}
		static void AddExpressions(this List<ParsedExpression> result, IconSetConditionalFormatting cf, XlsRPNContext context) {
			if(cf == null) return;
			int count = cf.ExpectedPointsNumber;
			for(int i = 0; i < count; i++) {
				ConditionalFormattingValueObject pointValue = cf.GetPointValue(i);
				result.AddExpression(pointValue, context);
			}
		}
		static void AddExpression(this List<ParsedExpression> result, ConditionalFormattingValueObject value, XlsRPNContext context) {
			ParsedExpression expression = null;
			switch(value.ValueType) {
				case ConditionalFormattingValueObjectType.Formula:
				case ConditionalFormattingValueObjectType.Num:
				case ConditionalFormattingValueObjectType.Percent:
				case ConditionalFormattingValueObjectType.Percentile:
					expression = value.ValueExpression;
					break;
			}
			if(expression != null)
				result.Add(expression);
		}
		static ParsedExpression GetParsedExpression(string formula, XlsRPNContext context) {
			if (!formula.StartsWith("=", StringComparison.Ordinal))
				formula = "=" + formula;
			return context.WorkbookContext.ParseExpression(formula, OperandDataType.Value, false);
		}
		#endregion
	}
	#endregion
}
