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
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Office.Services;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Model.External;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsParsedThingConversionResult
	public class XlsParsedThingConversionResult {
		public bool RefToError { get; set; }
		public bool RefTruncated { get; set; }
		public bool TableRefToRef { get; set; }
		public bool TableRefToError { get; set; }
		public bool ElfToRef { get; set; }
		public bool ElfLelToError { get; set; }
		public bool FuncVarArgumentCount { get; set; }
		public bool FuncNestingLevel { get; set; }
		public bool TotalFormulaSize { get; set; }
		public bool NoChanges {
			get { return !RefToError && !RefTruncated && !TableRefToRef && !TableRefToError && !ElfLelToError && !ElfToRef && !FuncVarArgumentCount && !FuncNestingLevel && !TotalFormulaSize; }
		}
		public bool OutOfXlsLimits {
			get { return RefToError || RefTruncated; }
		}
	}
	#region XlsParsedThingConverter
	public static class XlsParsedThingConverter {
		static void LogConversionResult(XlsParsedThingConversionResult result, IXlsRPNContext context) {
			if (context == null || result.NoChanges)
				return;
			string subject = context.CurrentSubject;
			if (string.IsNullOrEmpty(subject))
				return;
			ILogService logService = context.WorkbookContext.Workbook.GetService<ILogService>();
			if (logService != null) {
				subject += ": ";
				if (result.FuncVarArgumentCount) {
					logService.LogMessage(LogCategory.Warning, subject + XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_FuncVarExceedMaxArgCount));
				}
				else if (result.FuncNestingLevel) {
					logService.LogMessage(LogCategory.Warning, subject + XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_FuncExceedMaxNestingLevel));
				}
				else if (result.TotalFormulaSize) {
					logService.LogMessage(LogCategory.Warning, subject + XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_FormulaExceedMaxSize));
				}
				else {
					if (result.TableRefToRef)
						logService.LogMessage(LogCategory.Warning, subject + XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_TableRefToRef));
					if (result.TableRefToError)
						logService.LogMessage(LogCategory.Warning, subject + XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_TableRefToError));
					if (result.RefToError)
						logService.LogMessage(LogCategory.Warning, subject + XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_RefToError));
					if (result.RefTruncated)
						logService.LogMessage(LogCategory.Warning, subject + XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_RefTruncated));
					if (result.ElfToRef)
						logService.LogMessage(LogCategory.Warning, subject + XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ElfToRef));
					if (result.ElfLelToError)
						logService.LogMessage(LogCategory.Warning, subject + XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ElfLelToError));
				}
			}
		}
		#endregion
		#region Offset things utils
		interface IOffcetThing {
			bool CountDown();
			void Move(int offset);
		}
		abstract class OffsetThingBase : IOffcetThing {
			int offset;
			protected OffsetThingBase(int offset) {
				this.offset = offset;
			}
			public bool CountDown() {
				offset--;
				return offset <= 0;
			}
			public abstract void Move(int offset);
		}
		class MemOffsetThing : OffsetThingBase {
			ParsedThingMemBase thing;
			public MemOffsetThing(ParsedThingMemBase thing)
				: base(thing.InnerThingCount) {
				this.thing = thing;
			}
			public override void Move(int offset) {
				thing.InnerThingCount += offset;
			}
		}
		class AttrIfOffsetThing : OffsetThingBase {
			ParsedThingAttrIf thing;
			public AttrIfOffsetThing(ParsedThingAttrIf thing)
				: base(thing.Offset) {
				this.thing = thing;
			}
			public override void Move(int offset) {
				thing.Offset += offset;
			}
		}
		class AttrGotoOffsetThing : OffsetThingBase {
			ParsedThingAttrGoto thing;
			public AttrGotoOffsetThing(ParsedThingAttrGoto thing)
				: base(thing.Offset) {
				this.thing = thing;
			}
			public override void Move(int offset) {
				thing.Offset += offset;
			}
		}
		class AttrChoosePartOffsetThing : OffsetThingBase {
			ParsedThingAttrChoose thing;
			int partIndex;
			public AttrChoosePartOffsetThing(ParsedThingAttrChoose thing, int partIndex)
				: base(thing.Offsets[partIndex]) {
				this.partIndex = partIndex;
				this.thing = thing;
			}
			public override void Move(int offset) {
				thing.Offsets[partIndex] += offset;
			}
		}
		static List<IOffcetThing> offsetThings = new List<IOffcetThing>();
		static void CountDownOffsetThings() {
			int i = 0;
			while (i < offsetThings.Count) {
				if (offsetThings[i].CountDown())
					offsetThings.RemoveAt(i);
				else
					i++;
			}
		}
		static void MoveOffsetThings(int offset) {
			for (int i = 0; i < offsetThings.Count; i++)
				offsetThings[i].Move(offset);
		}
		#endregion
		public static ParsedExpression ToXlsExpression(ParsedExpression expression, IXlsRPNContext context) {
			XlsParsedThingConversionResult conversionResult = new XlsParsedThingConversionResult();
			return ToXlsExpression(expression, context, conversionResult);
		}
		public static ParsedExpression ToXlsExpression(ParsedExpression expression, IXlsRPNContext context, XlsParsedThingConversionResult conversionResult) {
			ParsedExpression result = new ParsedExpression();
			offsetThings.Clear();
			if (expression != null) {
				ParsedThingAttrSemi attrSemi = expression.Count > 0 ? expression[0] as ParsedThingAttrSemi : null;
				if (attrSemi != null && !attrSemi.HasProperty(FormulaProperties.HasVolatileFunction))
					expression.RemoveAt(0);
				for (int i = 0; i < expression.Count; i++) {
					IParsedThing thing = expression[i];
					if (thing is ParsedThingTable)
						ConvertTable(result, thing, context, conversionResult);
					else if (thing is ParsedThingAddinFunc)
						ConvertAddinFunc(result, thing);
					else if (thing is ParsedThingUnknownFuncExt)
						ConvertUnkownFuncExt(result, thing);
					else if (thing is ParsedThingUnknownFunc)
						ConvertUnkownFunc(result, thing);
					else if (thing is ParsedThingCustomFunc)
						ConvertCustomFunc(result, thing);
					else if (IsFutureFunction(thing))
						ConvertFutureFunc(result, thing);
					else if (IsPredefinedFunction(thing))
						ConvertPredefinedFunc(result, thing);
					else if (IsInternalFunction(thing))
						ConvertInternalFunc(result, thing);
					else if (thing is ParsedThingFuncVar)
						ConvertFunctionSum(result, thing);
					else if (thing is ParsedThingRef)
						CheckRefForXlsLimits(result, thing, conversionResult);
					else if (thing is ParsedThingRefRel)
						CheckRefNForXlsLimits(result, thing, conversionResult);
					else if (thing is ParsedThingArea)
						CheckAreaForXlsLimits(result, thing, conversionResult);
					else if (thing is ParsedThingAreaN)
						CheckAreaNForXlsLimits(result, thing, conversionResult);
					else
						result.Add(thing);
					CountDownOffsetThings();
					if (IsControlToken(thing) || IsMemToken(thing))
						AddOffsetToken(thing);
				}
				if (!CheckFuncVarArgumentCount(result)) {
					conversionResult.FuncVarArgumentCount = true;
					result.Clear();
					ParsedThingError error = new ParsedThingError();
					error.Value = VariantValue.ErrorInvalidValueInFunction;
					result.Add(error);
				}
				else if (!CheckFuncNestedLevel(result)) {
					conversionResult.FuncNestingLevel = true;
					result.Clear();
					ParsedThingError error = new ParsedThingError();
					error.Value = VariantValue.ErrorInvalidValueInFunction;
					result.Add(error);
				}
				else if (!CheckFormulaSize(result, context) || !CheckFormulaLength(expression, context)) {
					conversionResult.TotalFormulaSize = true;
					result.Clear();
					ParsedThingError error = new ParsedThingError();
					error.Value = VariantValue.ErrorInvalidValueInFunction;
					result.Add(error);
				}
			}
			else {
				ParsedThingError error = new ParsedThingError();
				error.Value = VariantValue.ErrorValueNotAvailable;
				result.Add(error);
			}
			LogConversionResult(conversionResult, context);
			return result;
		}
		public static ParsedExpression ToModelExpression(ParsedExpression expression, IXlsRPNContext context) {
			FormulaProperties properties = FormulaProperties.None;
			XlsParsedThingConversionResult conversionResult = new XlsParsedThingConversionResult();
			ParsedThingElfRadical radical = null;
			bool radicalLelWasHere = false;
			offsetThings.Clear();
			ParsedExpression result = new ParsedExpression();
			for (int i = 0; i < expression.Count; i++) {
				IParsedThing thing = expression[i];
				if (IsUnknownFunction(thing)) {
					ParsedThingFunc convertedFuncThing = ConvertUnknownFuncVar(result, thing, context);
					properties |= ValidateFormulaPropertiesCore(convertedFuncThing);
				}
				else if (IsElf(thing)) {
					ConvertElf(result, thing, context);
					conversionResult.ElfToRef = true;
				}
				else if (thing is ParsedThingRefRel)
					ConvertRefN(result, thing, context);
				else if (thing is ParsedThingAreaN)
					ConvertAreaN(result, thing, context);
				else if (thing is ParsedThingAttrSum)
					ConvertAttrSum(result, thing);
				else if (thing is ParsedThingMemBase)
					ConvertMemBased(result, thing);
				else if (Object.ReferenceEquals(thing.GetType(), typeof(ParsedThingArea))) {
					if (radicalLelWasHere) {
						result.Add(new ParsedThingError(VariantValue.ErrorName.ErrorValue));
						conversionResult.ElfLelToError = true;
					}
					else if (radical != null) {
						ParsedThingArea areaThing = thing.Clone() as ParsedThingArea;
						CellPosition position = areaThing.TopLeft;
						areaThing.TopLeft = new CellPosition(position.Column, position.Row, radical.Position.ColumnType, position.RowType);
						position = areaThing.BottomRight;
						areaThing.BottomRight = new CellPosition(position.Column, position.Row, radical.Position.ColumnType, position.RowType);
						result.Add(areaThing);
						conversionResult.ElfToRef = true;
					}
					else
						result.Add(thing);
				}
				else if (thing is ParsedThingAreaErr) {
					if (radicalLelWasHere) {
						result.Add(new ParsedThingError() { Value = VariantValue.ErrorName });
						conversionResult.ElfLelToError = true;
					}
					else
						result.Add(thing);
				}
				else {
					result.Add(thing);
					properties |= ValidateFormulaProperties(thing);
				}
				radical = thing as ParsedThingElfRadical;
				radicalLelWasHere = thing is ParsedThingElfRadicalLel;
				CountDownOffsetThings();
				if (IsControlToken(thing) || IsMemToken(thing))
					AddOffsetToken(thing);
			}
			if (properties != FormulaProperties.None) {
				ParsedThingAttrSemi attrSemi = result[0] as ParsedThingAttrSemi;
				if (attrSemi == null)
					result.Insert(0, new ParsedThingAttrSemi(properties));
				else
					attrSemi.AddProperty(properties);
			}
			LogConversionResult(conversionResult, context);
			return result;
		}
		static FormulaProperties ValidateFormulaProperties(IParsedThing thing) {
			ParsedThingFuncVar function = thing as ParsedThingFuncVar;
			if (function == null)
				return FormulaProperties.None;
			return ValidateFormulaPropertiesCore(function);
		}
		static FormulaProperties ValidateFormulaPropertiesCore(ParsedThingFunc function) {
					FormulaProperties result = FormulaProperties.None;
			if (function is ParsedThingCustomFunc)
				result |= FormulaProperties.HasCustomFunction;
			if (function.Function.IsVolatile)
				result |= FormulaProperties.HasVolatileFunction;
			if ((function.Function.Code & 0x4100) == 0x4100)
				result |= FormulaProperties.HasInternalFunction;
			else if (function.Function.Code == FormulaCalculator.FuncRTDCode)
				result |= FormulaProperties.HasFunctionRTD;
			return result;
		}
		public static bool IsFutureFunction(int funcCode) {
			ISpreadsheetFunction function = FormulaCalculator.GetFunctionByCode(funcCode);
			if (function != null)
				return PredefinedFunctions.IsExcel2003FutureFunction(function.Name);
			return false;
		}
		public static bool IsInternalFunction(int funcCode) {
			return (funcCode & 0x4100) == 0x4100;
		}
		public static bool IsPredefinedFunction(int funcCode) {
			ISpreadsheetFunction function = FormulaCalculator.GetFunctionByCode(funcCode);
			if (function != null)
				return PredefinedFunctions.IsExcel2010PredefinedNonFutureFunction(function.Name);
			return false;
		}
		public static ParsedExpression ToCFExpression(ParsedExpression expression, IXlsRPNContext context) {
			ParsedExpression result = new ParsedExpression();
			for (int i = 0; i < expression.Count; i++) {
				IParsedThing thing = expression[i];
				if (thing is ParsedThingRef3d)
					ConvertCFRef3d(result, thing, context);
				else if (thing is ParsedThingErr3d)
					ConvertCFErr3d(result, thing, context);
				else if (thing is ParsedThingArea3d)
					ConvertCFArea3d(result, thing, context);
				else if (thing is ParsedThingAreaErr3d)
					ConvertCFAreaErr3d(result, thing, context);
				else
					result.Add(thing);
			}
			return result;
		}
		#region ToCFExpression internals
		static void ConvertCFRef3d(ParsedExpression expression, IParsedThing ptg, IXlsRPNContext context) {
			ParsedThingRef3d thing = ptg as ParsedThingRef3d;
			SheetDefinition sheetDefinition = context.WorkbookContext.GetSheetDefinition(thing.SheetDefinitionIndex);
			if (sheetDefinition.IsCurrentSheetReference || (!sheetDefinition.IsExternalReference && !sheetDefinition.Is3DReference &&
				sheetDefinition.SheetDefinied && sheetDefinition.SheetNameStart == context.WorkbookContext.CurrentWorksheet.Name)) {
				ParsedThingRef ptgRef = new ParsedThingRef(thing.Position);
				ptgRef.DataType = thing.DataType;
				expression.Add(ptgRef);
			}
			else {
				ParsedThingRefErr ptgRefErr = new ParsedThingRefErr();
				ptgRefErr.DataType = thing.DataType;
				expression.Add(ptgRefErr);
			}
		}
		static void ConvertCFErr3d(ParsedExpression expression, IParsedThing ptg, IXlsRPNContext context) {
			ParsedThingRefErr ptgRefErr = new ParsedThingRefErr();
			ptgRefErr.DataType = ptg.DataType;
			expression.Add(ptgRefErr);
		}
		static void ConvertCFArea3d(ParsedExpression expression, IParsedThing ptg, IXlsRPNContext context) {
			ParsedThingArea3d thing = ptg as ParsedThingArea3d;
			SheetDefinition sheetDefinition = context.WorkbookContext.GetSheetDefinition(thing.SheetDefinitionIndex);
			if (sheetDefinition.IsCurrentSheetReference || (!sheetDefinition.IsExternalReference && !sheetDefinition.Is3DReference &&
				sheetDefinition.SheetDefinied && sheetDefinition.SheetNameStart == context.WorkbookContext.CurrentWorksheet.Name)) {
				ParsedThingArea ptgArea = new ParsedThingArea(thing.CellRange);
				ptgArea.DataType = thing.DataType;
				expression.Add(ptgArea);
			}
			else {
				ParsedThingAreaErr ptgAreaErr = new ParsedThingAreaErr();
				ptgAreaErr.DataType = thing.DataType;
				expression.Add(ptgAreaErr);
			}
		}
		static void ConvertCFAreaErr3d(ParsedExpression expression, IParsedThing ptg, IXlsRPNContext context) {
			ParsedThingAreaErr ptgAreaErr = new ParsedThingAreaErr();
			ptgAreaErr.DataType = ptg.DataType;
			expression.Add(ptgAreaErr);
		}
		#endregion
		#region ToModelExpression internals
		static ParsedThingFunc ConvertUnknownFuncVar(ParsedExpression expression, IParsedThing ptg, IXlsRPNContext context) {
			ParsedThingFunc func = ptg as ParsedThingFuncVar;
			Stack<IParsedThing> funcArgs = new Stack<IParsedThing>();
			int count = func.ParamCount;
			while (expression.Count > 0) {
				int index = expression.Count - 1;
				IParsedThing thing = expression[index];
				expression.RemoveAt(index);
				count += GetCountDelta(thing);
				if (count <= 0) {
					ParsedThingNameX ptgNameX = thing as ParsedThingNameX;
					if (ptgNameX != null) {
						expression.AddRange(funcArgs);
						if (ptgNameX.SheetDefinitionIndex < 0) { 
							if (PredefinedFunctions.IsExcel2010PredefinedFunction(ptgNameX.DefinedName) || PredefinedFunctions.IsInternalFunction(ptgNameX.DefinedName))
								func = CreateFunc(expression, func, ptgNameX.DefinedName, context);
							else {
								ParsedThingAddinFunc proxy = new ParsedThingAddinFunc();
								proxy.DataType = func.DataType;
								proxy.FuncCode = func.FuncCode;
								proxy.Name = ptgNameX.DefinedName;
								proxy.ParamCount = func.ParamCount - 1;
								expression.Add(proxy);
								func = proxy;
							}
						}
						else {
							ParsedThingUnknownFuncExt proxy = new ParsedThingUnknownFuncExt();
							proxy.DataType = func.DataType;
							proxy.FuncCode = func.FuncCode;
							proxy.Name = ptgNameX.DefinedName;
							proxy.ParamCount = func.ParamCount - 1;
							proxy.SheetDefinitionIndex = ptgNameX.SheetDefinitionIndex;
							expression.Add(proxy);
							func = proxy;
						}
						MoveOffsetThings(-1);
						return func;
					}
					ParsedThingName ptgName = thing as ParsedThingName;
					if (ptgName != null) {
						expression.AddRange(funcArgs);
						func = CreateFunc(expression, func, ptgName.DefinedName, context);
						MoveOffsetThings(-1);
						return func;
					}
				}
				funcArgs.Push(thing);
			}
			expression.AddRange(funcArgs);
			expression.Add(func);
			return func;
		}
		static ParsedThingFunc CreateFunc(ParsedExpression expression, ParsedThingFunc func, string name, IXlsRPNContext context) {
			string funcName = name.StartsWith("_xlfn.") ? name.Substring(6) : name;
			ISpreadsheetFunction function = FormulaCalculator.GetFunctionByInvariantName(funcName, context.WorkbookContext);
			ParsedThingFunc proxy = null;
			if (function is NotExistingFunction) {
				proxy = new ParsedThingUnknownFunc(name, func.ParamCount - 1, func.DataType);
				proxy.FuncCode = func.FuncCode;
			}
			else if (function is CustomFunction)
				proxy = new ParsedThingCustomFunc(function, func.ParamCount - 1, func.DataType);
			else if (function.HasFixedParametersCount)
				proxy = new ParsedThingFunc(function.Code, func.DataType);
			else
				proxy = new ParsedThingFuncVar(function.Code, func.ParamCount - 1, func.DataType);
			expression.Add(proxy);
			return proxy;
		}
		static void ConvertElf(ParsedExpression expression, IParsedThing ptg, IRPNContext context) {
			IParsedThingElf thing = ptg as IParsedThingElf;
			if (context != null && thing != null) {
				IParsedThing translated = thing.Translate(context.WorkbookContext);
				if (translated != null)
					expression.Add(translated);
				else
					MoveOffsetThings(-1);
			}
			else
				expression.Add(ptg);
		}
		static void ConvertRefN(ParsedExpression expression, IParsedThing ptg, IRPNContext context) {
			ParsedThingRefRel thing = ptg as ParsedThingRefRel;
			if (context != null && thing != null) {
				CellPosition position = thing.Location.ToCellPosition(context.WorkbookContext);
				thing.Location = position.ToCellOffset(context.WorkbookContext);
				expression.Add(thing);
			}
			else
				expression.Add(ptg);
		}
		static void ConvertAreaN(ParsedExpression expression, IParsedThing ptg, IRPNContext context) {
			ParsedThingAreaN thing = ptg as ParsedThingAreaN;
			if (context != null && thing != null) {
				CellPosition position = thing.First.ToCellPosition(context.WorkbookContext);
				thing.First = position.ToCellOffset(context.WorkbookContext);
				position = thing.Last.ToCellPosition(context.WorkbookContext);
				thing.Last = position.ToCellOffset(context.WorkbookContext);
				expression.Add(thing);
			}
			else
				expression.Add(ptg);
		}
		static void ConvertAttrSum(ParsedExpression expression, IParsedThing ptg) {
			ParsedThingAttrSum thing = ptg as ParsedThingAttrSum;
			if (thing != null) {
				ParsedThingFuncVar funcVar = new ParsedThingFuncVar();
				funcVar.DataType = OperandDataType.Value;
				funcVar.FuncCode = FormulaCalculator.FuncSumCode;
				funcVar.ParamCount = 1;
				expression.Add(funcVar);
			}
			else
				expression.Add(ptg);
		}
		static void ConvertMemBased(ParsedExpression expression, IParsedThing ptg) {
			ParsedThingMemBase thing = ptg as ParsedThingMemBase;
			ParsedThingMemFunc memFunc = new ParsedThingMemFunc();
			memFunc.DataType = thing.DataType;
			memFunc.InnerThingCount = thing.InnerThingCount;
			expression.Add(memFunc);
			offsetThings.Add(new MemOffsetThing(memFunc));
		}
		#endregion
		#region ToXlsExpression internals
		static void ConvertTable(ParsedExpression expression, IParsedThing ptg, IXlsRPNContext context, XlsParsedThingConversionResult conversionResult) {
			ParsedThingTable tableThing = ptg as ParsedThingTable;
			ParsedThingTableExt tableExtThing = ptg as ParsedThingTableExt;
			WorkbookDataContext workbookContext = context.WorkbookContext;
			if (tableExtThing != null) {
			}
			else if (tableThing != null) {
				SheetDefinition sheetDefinition = tableThing.GetSheetDefinition(workbookContext);
				VariantValue result = tableThing.PreEvaluate(workbookContext);
				if (result.IsCellRange) {
					conversionResult.TableRefToRef = true;
					CellRange range = result.CellRangeValue.GetFirstInnerCellRange();
					Table table = workbookContext.GetDefinedTableRange(tableThing.TableName);
					if (table.HasTotalsRow) {
						if (workbookContext.CurrentRowIndex == table.Range.BottomRight.Row) {
							range.TopLeft = new CellPosition(range.TopLeft.Column, range.TopLeft.Row);
							range.BottomRight = new CellPosition(range.BottomRight.Column, range.BottomRight.Row);
						}
					}
					if (range.CellCount == 1) {
						if (sheetDefinition.IsCurrentSheetReference) {
							ParsedThingRef ptgRef = new ParsedThingRef();
							ptgRef.DataType = tableThing.DataType;
							ptgRef.Position = range.TopLeft;
							CheckRefForXlsLimits(expression, ptgRef, conversionResult);
						}
						else if (context.WorkbookContext.DefinedNameProcessing) {
							ParsedThingRef3dRel ptgRef = new ParsedThingRef3dRel();
							ptgRef.DataType = tableThing.DataType;
							ptgRef.Location = range.TopLeft.ToCellOffset();
							ptgRef.SheetDefinitionIndex = workbookContext.RegisterSheetDefinition(sheetDefinition);
							CheckRefNForXlsLimits(expression, ptgRef, conversionResult);
						}
						else {
							ParsedThingRef3d ptgRef = new ParsedThingRef3d();
							ptgRef.DataType = tableThing.DataType;
							ptgRef.Position = range.TopLeft;
							ptgRef.SheetDefinitionIndex = workbookContext.RegisterSheetDefinition(sheetDefinition);
							CheckRefForXlsLimits(expression, ptgRef, conversionResult);
						}
					}
					else {
						if (sheetDefinition.IsCurrentSheetReference) {
							ParsedThingArea area = new ParsedThingArea();
							area.DataType = tableThing.DataType;
							area.CellRange = range;
							CheckAreaForXlsLimits(expression, area, conversionResult);
						}
						else if (context.WorkbookContext.DefinedNameProcessing) {
							ParsedThingArea3dRel area = new ParsedThingArea3dRel();
							area.DataType = tableThing.DataType;
							area.First = range.TopLeft.ToCellOffset();
							area.Last = range.BottomRight.ToCellOffset();
							area.SheetDefinitionIndex = workbookContext.RegisterSheetDefinition(sheetDefinition);
							CheckAreaNForXlsLimits(expression, area, conversionResult);
						}
						else {
							ParsedThingArea3d area = new ParsedThingArea3d();
							area.DataType = tableThing.DataType;
							area.CellRange = range;
							area.SheetDefinitionIndex = workbookContext.RegisterSheetDefinition(sheetDefinition);
							CheckAreaForXlsLimits(expression, area, conversionResult);
						}
					}
					return;
				}
				else if (result.IsError) {
					conversionResult.TableRefToError = true;
					ParsedThingAreaErr3d areaErr = new ParsedThingAreaErr3d(workbookContext.RegisterSheetDefinition(sheetDefinition));
					areaErr.DataType = tableThing.DataType;
					expression.Add(areaErr);
					return;
				}
			}
			expression.Add(ptg);
		}
		static void ConvertAddinFunc(ParsedExpression expression, IParsedThing ptg) {
			ParsedThingAddinFunc func = ptg as ParsedThingAddinFunc;
			Stack<IParsedThing> funcArgs = new Stack<IParsedThing>();
			TakeArguments(expression, funcArgs, func.ParamCount);
			ParsedThingNameX ptgNameX = new ParsedThingNameX();
			ptgNameX.DefinedName = func.Name;
			ptgNameX.SheetDefinitionIndex = -1;
			expression.Add(ptgNameX);
			expression.AddRange(funcArgs);
			ParsedThingFuncVar funcVar = new ParsedThingFuncVar();
			funcVar.DataType = func.DataType;
			funcVar.FuncCode = func.FuncCode;
			funcVar.ParamCount = func.ParamCount + 1;
			expression.Add(funcVar);
			MoveOffsetThings(1);
		}
		static void ConvertUnkownFuncExt(ParsedExpression expression, IParsedThing ptg) {
			ParsedThingUnknownFuncExt func = ptg as ParsedThingUnknownFuncExt;
			Stack<IParsedThing> funcArgs = new Stack<IParsedThing>();
			TakeArguments(expression, funcArgs, func.ParamCount);
			ParsedThingNameX ptgNameX = new ParsedThingNameX();
			ptgNameX.DataType = OperandDataType.Reference;
			ptgNameX.DefinedName = func.Name;
			ptgNameX.SheetDefinitionIndex = func.SheetDefinitionIndex;
			expression.Add(ptgNameX);
			expression.AddRange(funcArgs);
			ParsedThingFuncVar funcVar = new ParsedThingFuncVar();
			funcVar.DataType = func.DataType;
			funcVar.FuncCode = func.FuncCode;
			funcVar.ParamCount = func.ParamCount + 1;
			expression.Add(funcVar);
			MoveOffsetThings(1);
		}
		static void ConvertUnkownFunc(ParsedExpression expression, IParsedThing ptg) {
			ParsedThingUnknownFunc func = ptg as ParsedThingUnknownFunc;
			ConvertUnkownFuncCore(expression, func, func.Name);
		}
		static void ConvertCustomFunc(ParsedExpression expression, IParsedThing ptg) {
			ParsedThingCustomFunc func = ptg as ParsedThingCustomFunc;
			ConvertUnkownFuncCore(expression, func, func.Name);
		}
		static void ConvertUnkownFuncCore(ParsedExpression expression, ParsedThingFuncVar func, string functionName) {
			Stack<IParsedThing> funcArgs = new Stack<IParsedThing>();
			TakeArguments(expression, funcArgs, func.ParamCount);
			if (PredefinedFunctions.IsExcel2010PredefinedNonFutureFunction(functionName)) {
				ParsedThingNameX ptgNameX = new ParsedThingNameX();
				ptgNameX.DefinedName = functionName;
				ptgNameX.SheetDefinitionIndex = -1;
				expression.Add(ptgNameX);
			}
			else {
				ParsedThingName ptgName = new ParsedThingName();
				ptgName.DataType = OperandDataType.Reference;
				ptgName.DefinedName = functionName;
				expression.Add(ptgName);
			}
			expression.AddRange(funcArgs);
			ParsedThingFuncVar funcVar = new ParsedThingFuncVar();
			funcVar.DataType = func.DataType;
			funcVar.FuncCode = func.FuncCode;
			funcVar.ParamCount = func.ParamCount + 1;
			expression.Add(funcVar);
			MoveOffsetThings(1);
		}
		static void ConvertFutureFuncVar(ParsedExpression expression, IParsedThing ptg) {
			ParsedThingFuncVar func = ptg as ParsedThingFuncVar;
			Stack<IParsedThing> funcArgs = new Stack<IParsedThing>();
			TakeArguments(expression, funcArgs, func.ParamCount);
			ParsedThingName ptgName = new ParsedThingName();
			ISpreadsheetFunction function = func.Function;
			ptgName.DefinedName = WorksheetFunctionBase.FUTURE_FUNCTION_PREFIX + function.Name;
			expression.Add(ptgName);
			expression.AddRange(funcArgs);
			ParsedThingFuncVar funcVar = new ParsedThingFuncVar();
			funcVar.DataType = func.DataType;
			funcVar.FuncCode = 0x00ff;
			funcVar.ParamCount = func.ParamCount + 1;
			expression.Add(funcVar);
		}
		static void ConvertFutureFunc(ParsedExpression expression, IParsedThing ptg) {
			if (ptg is ParsedThingFuncVar)
				ConvertFutureFuncVar(expression, ptg);
			else {
				ParsedThingFunc func = ptg as ParsedThingFunc;
				Stack<IParsedThing> funcArgs = new Stack<IParsedThing>();
				ISpreadsheetFunction function = func.Function;
				int paramCount = function.Parameters.Count;
				TakeArguments(expression, funcArgs, paramCount);
				ParsedThingName ptgName = new ParsedThingName();
				ptgName.DefinedName = WorksheetFunctionBase.FUTURE_FUNCTION_PREFIX + function.Name;
				expression.Add(ptgName);
				expression.AddRange(funcArgs);
				ParsedThingFuncVar funcVar = new ParsedThingFuncVar();
				funcVar.DataType = func.DataType;
				funcVar.FuncCode = 0x00ff;
				funcVar.ParamCount = paramCount + 1;
				expression.Add(funcVar);
			}
			MoveOffsetThings(1);
		}
		#region ConvertPredefinedFuncVar
		static void ConvertPredefinedFuncVar(ParsedExpression expression, IParsedThing ptg) {
			ParsedThingFuncVar func = ptg as ParsedThingFuncVar;
			Stack<IParsedThing> funcArgs = new Stack<IParsedThing>();
			TakeArguments(expression, funcArgs, func.ParamCount);
			ParsedThingNameX ptgNameX = new ParsedThingNameX();
			ptgNameX.DefinedName = func.Function.Name;
			ptgNameX.SheetDefinitionIndex = -1;
			expression.Add(ptgNameX);
			expression.AddRange(funcArgs);
			ParsedThingFuncVar funcVar = new ParsedThingFuncVar();
			funcVar.DataType = func.DataType;
			funcVar.FuncCode = 0x00ff;
			funcVar.ParamCount = func.ParamCount + 1;
			expression.Add(funcVar);
		}
		static void ConvertPredefinedFunc(ParsedExpression expression, IParsedThing ptg) {
			if (ptg is ParsedThingFuncVar)
				ConvertPredefinedFuncVar(expression, ptg);
			else {
				ParsedThingFunc func = ptg as ParsedThingFunc;
				Stack<IParsedThing> funcArgs = new Stack<IParsedThing>();
				ISpreadsheetFunction function = func.Function;
				int paramCount = function.Parameters.Count;
				TakeArguments(expression, funcArgs, paramCount);
				ParsedThingNameX ptgNameX = new ParsedThingNameX();
				ptgNameX.DefinedName = func.Function.Name;
				ptgNameX.SheetDefinitionIndex = -1;
				expression.Add(ptgNameX);
				expression.AddRange(funcArgs);
				ParsedThingFuncVar funcVar = new ParsedThingFuncVar();
				funcVar.DataType = func.DataType;
				funcVar.FuncCode = 0x00ff;
				funcVar.ParamCount = paramCount + 1;
				expression.Add(funcVar);
			}
			MoveOffsetThings(1);
		}
		#endregion
		#region ConvertInternalFuncVar
		static void ConvertInternalFuncVar(ParsedExpression expression, IParsedThing ptg) {
			ParsedThingFuncVar func = ptg as ParsedThingFuncVar;
			Stack<IParsedThing> funcArgs = new Stack<IParsedThing>();
			TakeArguments(expression, funcArgs, func.ParamCount);
			ParsedThingName ptgNameX = new ParsedThingName();
			ptgNameX.DefinedName = func.Function.Name;
			expression.Add(ptgNameX);
			expression.AddRange(funcArgs);
			ParsedThingFuncVar funcVar = new ParsedThingFuncVar();
			funcVar.DataType = func.DataType;
			funcVar.FuncCode = 0x00ff;
			funcVar.ParamCount = func.ParamCount + 1;
			expression.Add(funcVar);
		}
		static void ConvertInternalFunc(ParsedExpression expression, IParsedThing ptg) {
			if (ptg is ParsedThingFuncVar)
				ConvertInternalFuncVar(expression, ptg);
			else {
				ParsedThingFunc func = ptg as ParsedThingFunc;
				Stack<IParsedThing> funcArgs = new Stack<IParsedThing>();
				ISpreadsheetFunction function = func.Function;
				int paramCount = function.Parameters.Count;
				TakeArguments(expression, funcArgs, paramCount);
				ParsedThingName ptgNameX = new ParsedThingName();
				ptgNameX.DefinedName = func.Function.Name;
				expression.Add(ptgNameX);
				expression.AddRange(funcArgs);
				ParsedThingFuncVar funcVar = new ParsedThingFuncVar();
				funcVar.DataType = func.DataType;
				funcVar.FuncCode = 0x00ff;
				funcVar.ParamCount = paramCount + 1;
				expression.Add(funcVar);
			}
			MoveOffsetThings(1);
		}
		#endregion
		static void ConvertFunctionSum(ParsedExpression expression, IParsedThing ptg) {
			ParsedThingFuncVar funcVar = ptg as ParsedThingFuncVar;
			if (funcVar.ParamCount == 1 && funcVar.DataType == OperandDataType.Value && funcVar.FuncCode == FormulaCalculator.FuncSumCode) {
				ParsedThingAttrSum attrSum = new ParsedThingAttrSum();
				expression.Add(attrSum);
			}
			else
				expression.Add(ptg);
		}
		static void TakeArguments(ParsedExpression expression, Stack<IParsedThing> funcArgs, int paramCount) {
			int count = paramCount;
			while (count > 0 && expression.Count > 0) {
				int index = expression.Count - 1;
				IParsedThing thing = expression[index];
				expression.RemoveAt(index);
				funcArgs.Push(thing);
				count += GetCountDelta(thing);
			}
			while (expression.Count > 0) {
				int index = expression.Count - 1;
				IParsedThing thing = expression[index];
				if (IsControlToken(thing) || (!IsAttribute(thing) && !IsMemToken(thing) && !IsDisplayToken(thing)))
					break;
				expression.RemoveAt(index);
				funcArgs.Push(thing);
			}
		}
		static void CheckRefForXlsLimits(ParsedExpression expression, IParsedThing ptg, XlsParsedThingConversionResult conversionResult) {
			ParsedThingRef thingRef = ptg as ParsedThingRef;
			ParsedThingRef3d thingRef3d = ptg as ParsedThingRef3d;
			if (thingRef3d != null && thingRef3d.Position.OutOfLimits()) {
				conversionResult.RefToError = true;
				ParsedThingErr3d refErr = new ParsedThingErr3d(thingRef3d.SheetDefinitionIndex);
				refErr.DataType = thingRef3d.DataType;
				expression.Add(refErr);
				return;
			}
			else if (thingRef != null && thingRef.Position.OutOfLimits()) {
				conversionResult.RefToError = true;
				ParsedThingRefErr refErr = new ParsedThingRefErr();
				refErr.DataType = thingRef.DataType;
				expression.Add(refErr);
				return;
			}
			expression.Add(ptg);
		}
		static void CheckRefNForXlsLimits(ParsedExpression expression, IParsedThing ptg, XlsParsedThingConversionResult conversionResult) {
			ParsedThingRefRel thingRef = ptg as ParsedThingRefRel;
			ParsedThingRef3dRel thingRef3d = ptg as ParsedThingRef3dRel;
			if (thingRef3d != null) {
				if (thingRef3d.Location.OutOfLimits()) {
					conversionResult.RefToError = true;
					ParsedThingErr3d refErr = new ParsedThingErr3d(thingRef3d.SheetDefinitionIndex);
					refErr.DataType = thingRef3d.DataType;
					expression.Add(refErr);
					return;
				}
				else if (thingRef3d.Location.ShouldBeTruncated()) {
					ParsedThingRef3dRel thingRef3dTruncated = new ParsedThingRef3dRel();
					thingRef3dTruncated.CopyFrom(thingRef3d);
					thingRef3dTruncated.Location = thingRef3d.Location.GetTruncated();
					expression.Add(thingRef3dTruncated);
					return;
				}
			}
			else if (thingRef != null) {
				if (thingRef.Location.OutOfLimits()) {
					conversionResult.RefToError = true;
					ParsedThingRefErr refErr = new ParsedThingRefErr();
					refErr.DataType = thingRef.DataType;
					expression.Add(refErr);
					return;
				}
				else if (thingRef.Location.ShouldBeTruncated()) {
					ParsedThingRefRel thingRefTruncated = new ParsedThingRefRel();
					thingRefTruncated.CopyFrom(thingRef);
					thingRefTruncated.Location = thingRef.Location.GetTruncated();
					expression.Add(thingRef);
					return;
				}
			}
			expression.Add(ptg);
		}
		static void CheckAreaForXlsLimits(ParsedExpression expression, IParsedThing ptg, XlsParsedThingConversionResult conversionResult) {
			ParsedThingArea area = ptg as ParsedThingArea;
			ParsedThingArea3d area3d = ptg as ParsedThingArea3d;
			if (area3d != null) {
				CellRange range = area3d.CellRange;
				CellRange maxRange = XlsLimits.GetBoundRange(range);
				if (!maxRange.ContainsRange(range)) {
					if (maxRange.Intersects(range)) {
						conversionResult.RefTruncated = true;
						ParsedThingArea3d truncatedArea = new ParsedThingArea3d();
						truncatedArea.DataType = area3d.DataType;
						truncatedArea.SheetDefinitionIndex = area3d.SheetDefinitionIndex;
						truncatedArea.CellRange = range.Intersection(maxRange);
						expression.Add(truncatedArea);
					}
					else {
						conversionResult.RefToError = true;
						ParsedThingAreaErr3d areaErr = new ParsedThingAreaErr3d(area3d.SheetDefinitionIndex);
						areaErr.DataType = area3d.DataType;
						expression.Add(areaErr);
					}
					return;
				}
			}
			else if (area != null) {
				CellRange range = area.CellRange;
				CellRange maxRange = XlsLimits.GetBoundRange(range);
				if (!maxRange.ContainsRange(range)) {
					if (maxRange.Intersects(range)) {
						conversionResult.RefTruncated = true;
						ParsedThingArea truncatedArea = new ParsedThingArea();
						truncatedArea.DataType = area.DataType;
						truncatedArea.CellRange = range.Intersection(maxRange);
						expression.Add(truncatedArea);
					}
					else {
						conversionResult.RefToError = true;
						ParsedThingAreaErr areaErr = new ParsedThingAreaErr();
						areaErr.DataType = area.DataType;
						expression.Add(areaErr);
					}
					return;
				}
			}
			expression.Add(ptg);
		}
		static void CheckAreaNForXlsLimits(ParsedExpression expression, IParsedThing ptg, XlsParsedThingConversionResult conversionResult) {
			ParsedThingAreaN area = ptg as ParsedThingAreaN;
			ParsedThingArea3dRel area3d = ptg as ParsedThingArea3dRel;
			if (area3d != null) {
				if ((area3d.First.OutOfLimits())) {
					conversionResult.RefToError = true;
					ParsedThingAreaErr3d areaErr = new ParsedThingAreaErr3d(area3d.SheetDefinitionIndex);
					areaErr.DataType = area3d.DataType;
					expression.Add(areaErr);
					return;
				}
				if (area3d.First.ShouldBeTruncated() || area3d.Last.OutOfLimits() || area3d.Last.ShouldBeTruncated()) {
					conversionResult.RefTruncated = true;
					ParsedThingArea3dRel truncatedArea = area3d.Clone() as ParsedThingArea3dRel;
					truncatedArea.First = truncatedArea.First.GetTruncated();
					truncatedArea.Last = truncatedArea.Last.GetTruncated();
					expression.Add(truncatedArea);
					return;
				}
			}
			else if (area != null) {
				if ((area.First.OutOfLimits())) {
					conversionResult.RefToError = true;
					ParsedThingAreaErr areaErr = new ParsedThingAreaErr();
					areaErr.DataType = area.DataType;
					expression.Add(areaErr);
					return;
				}
				if (area.First.ShouldBeTruncated() || area.Last.OutOfLimits() || area.Last.ShouldBeTruncated()) {
					conversionResult.RefTruncated = true;
					ParsedThingAreaN truncatedArea = area.Clone() as ParsedThingAreaN;
					truncatedArea.First = truncatedArea.First.GetTruncated();
					truncatedArea.Last = truncatedArea.Last.GetTruncated();
					expression.Add(truncatedArea);
					return;
				}
			}
			expression.Add(ptg);
		}
		static void AddOffsetToken(IParsedThing thing) {
			ParsedThingAttrIf attrIf = thing as ParsedThingAttrIf;
			ParsedThingAttrGoto attrGoto = thing as ParsedThingAttrGoto;
			ParsedThingAttrChoose attrChoose = thing as ParsedThingAttrChoose;
			ParsedThingMemBase memThing = thing as ParsedThingMemBase;
			if (attrIf != null)
				offsetThings.Add(new AttrIfOffsetThing(attrIf));
			else if (attrGoto != null)
				offsetThings.Add(new AttrGotoOffsetThing(attrGoto));
			else if (memThing != null)
				offsetThings.Add(new MemOffsetThing(memThing));
			else if (attrChoose != null) {
				for (int i = 0; i < attrChoose.Offsets.Count; i++)
					offsetThings.Add(new AttrChoosePartOffsetThing(attrChoose, i));
			}
		}
		static bool CheckFuncVarArgumentCount(ParsedExpression expression) {
			for (int i = 0; i < expression.Count; i++) {
				ParsedThingFuncVar thing = expression[i] as ParsedThingFuncVar;
				if (thing != null && thing.ParamCount > 30)
					return false;
			}
			return true;
		}
		public static bool CheckFuncNestedLevel(ParsedExpression expression) {
			FunctionsNestedLevelVisitor visitor = new FunctionsNestedLevelVisitor();
			return visitor.Calculate(expression) <= 8;
		}
		static bool CheckFormulaSize(ParsedExpression expression, IXlsRPNContext context) {
			byte[] formulaBytes = context.ExpressionToBinary(expression);
			return formulaBytes.Length <= XlsDefs.MaxFormulaBytesSize;
		}
		static bool CheckFormulaLength(ParsedExpression expression, IXlsRPNContext context) {
			if (context.SuppressFormulaLengthCheck)
				return true;
			string formula = expression.BuildExpressionString(context.WorkbookContext);
			return formula.Length <= XlsDefs.MaxFormulaStringLength;
		}
		#endregion
		#region Utils
		static bool IsElf(IParsedThing thing) {
			return thing is IParsedThingElf;
		}
		static bool IsUnknownFunction(IParsedThing thing) {
			ParsedThingFuncVar func = thing as ParsedThingFuncVar;
			if (func == null)
				return false;
			return func.FuncCode == 0x00ff;
		}
		static bool IsFunction(IParsedThing thing) {
			return thing is ParsedThingFunc;
		}
		static bool IsFutureFunction(IParsedThing thing) {
			ParsedThingFunc func = thing as ParsedThingFunc;
			if (func == null)
				return false;
			string funcName = func.Function.Name;
			return PredefinedFunctions.IsExcel2003FutureFunction(funcName);
		}
		static bool IsInternalFunction(IParsedThing thing) {
			ParsedThingFunc func = thing as ParsedThingFunc;
			if (func == null)
				return false;
			string funcName = func.Function.Name;
			return PredefinedFunctions.IsInternalFunction(funcName);
		}
		static bool IsPredefinedFunction(IParsedThing thing) {
			ParsedThingFunc func = thing as ParsedThingFunc;
			if (func == null)
				return false;
			string funcName = func.Function.Name;
			return PredefinedFunctions.IsExcel2010PredefinedNonFutureFunction(funcName);
		}
		static bool IsBinaryOperator(IParsedThing thing) {
			return thing is BinaryParsedThing;
		}
		static bool IsUnaryOperator(IParsedThing thing) {
			return thing is UnaryParsedThing;
		}
		static bool IsControlToken(IParsedThing thing) {
			return thing is ParsedThingAttrIf ||
				thing is ParsedThingAttrChoose ||
				thing is ParsedThingAttrGoto;
		}
		static bool IsAttribute(IParsedThing thing) {
			return thing is ParsedThingAttrBase;
		}
		static bool IsDisplayToken(IParsedThing thing) {
			return thing is ParsedThingAttrSpaceBase || thing is ParsedThingParentheses;
		}
		static bool IsMemToken(IParsedThing thing) {
			return thing is ParsedThingMemBase;
		}
		static int GetFuncParamCount(IParsedThing thing) {
			ParsedThingFuncVar funcVar = thing as ParsedThingFuncVar;
			if (funcVar != null)
				return funcVar.ParamCount;
			ParsedThingFunc func = thing as ParsedThingFunc;
			return func.Function.Parameters.Count;
		}
		static int GetCountDelta(IParsedThing thing) {
			if (IsFunction(thing))
				return GetFuncParamCount(thing) - 1;
			if (IsBinaryOperator(thing))
				return 1;
			if (IsUnaryOperator(thing) || IsAttribute(thing) || IsMemToken(thing) || IsDisplayToken(thing) || IsControlToken(thing))
				return 0;
			return -1;
		}
		#endregion
	}
	#endregion
}
