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
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Model.External;
namespace DevExpress.XtraSpreadsheet.Model {
	public partial class ReferenceParser {
		static readonly HashSet<TableRowEnum> tableAllowedRowsTable = InitializeTableAllowedRowsTable();
		static HashSet<TableRowEnum> InitializeTableAllowedRowsTable() {
			HashSet<TableRowEnum> result = new HashSet<TableRowEnum>();
			result.Add(TableRowEnum.NotDefined);
			result.Add(TableRowEnum.Headers);
			result.Add(TableRowEnum.Totals);
			result.Add(TableRowEnum.Data);
			result.Add(TableRowEnum.All);
			result.Add(TableRowEnum.ThisRow);
			result.Add(TableRowEnum.Data | TableRowEnum.Totals);
			result.Add(TableRowEnum.Data | TableRowEnum.Headers);
			return result;
		}
		bool IsSheetDefinition() {
			scanner.ResetPeek();
			if (la.val == "'" || la.val == "'[" || la.kind == _quotedSymbol || (la.val == "!" && parserContext.DefinedNameProcessing))
				return true;
			if (la.val == "\"" || la.val == "(")
				return false;
			Token next;
			if (la.val == "[") {
				next = scanner.Peek();
				if (next.kind != _ident && next.kind != _wideident && next.kind != _positiveinumber)
					return false;
				next = scanner.Peek();
				if (next.val != "]")
					return false;
				Token sheetName = scanner.Peek();
				if (sheetName.val == "!")
					return true;
				if (sheetName.kind == _EOF || (sheetName.kind != _ident && sheetName.kind != _wideident && sheetName.kind != _positiveinumber))
					return false;
				if (sheetName.pos - next.pos == next.val.Length)
					return true;
				return false;
			}
			next = scanner.Peek();
			if (next.val == listSeparator.ToString())
				return false;
			if (next.val == "!" || next.val == "|")
				return true;
			if (next.val != ":")
				return false;
			if (la.kind == _ident && WorkbookDataContext.StartsFromCellPosition(la.val))
				return false;
			next = scanner.Peek();
			if (next.kind != _ident)
				return false;
			next = scanner.Peek();
			return next.val == "!";
		}
		bool IsTableDefinition(SheetDefinition sheetDefinition) {
			if (sheetDefinition != null && sheetDefinition.IsExternalReference)
				return false;
			if (la.val == "[") 
				return true;
			if (la.kind != _ident && la.kind != _wideident)
				return false;
			if (parserContext.GetTable(la.val) != null)
				return true;
			return false;
		}
		bool IsDefinedNameDefinition(SheetDefinition sheetDefinition) {
			if (la.val == "'")
				return true; 
			if ((la.kind != _ident && la.kind != _wideident) || !parserContext.HasDefinedName(la.val, sheetDefinition))
				return false;
			if (la.kind == _wideident)
				return true;
			CellPosition position;
			if (!parserContext.UseR1C1ReferenceStyle)
				position = CellReferenceParser.TryParsePart(la.val);
			else
				position = CellReferenceParser.TryParsePartRC(la.val, parserContext.CurrentColumnIndex, parserContext.CurrentRowIndex, 'r', 'c');
			if ((position.IsValid || position.IsColumnOrRow) && position.ToString().Length >= la.val.Length)
				return false;
			return true;
		}
		bool IsRCCellPosition() {
			scanner.ResetPeek();
			if (!parserContext.UseR1C1ReferenceStyle)
				return false;
			string lowerVal = la.val.ToLower();
			if (lowerVal.StartsWith("r") || lowerVal.StartsWith("c"))
				return true;
			return false;
		}
		bool IsA1CellPosition() {
			scanner.ResetPeek();
			if (parserContext.UseR1C1ReferenceStyle)
				return false;
			if (la.val == "$" || la.kind == _ident)
				return true;
			return false;
		}
		bool IsFunctionDefinition() {
			scanner.ResetPeek();
			if (la.kind != _ident && la.kind != _wideident && la.kind != _trueConstant && la.kind != _falseConstant)
				return false;
			Token next = scanner.Peek();
			if (next.val != "(")
				return false;
			CellPosition position = CellPosition.TryCreate(la.val);
			if (position.IsValid && next.pos > la.pos + la.val.Length) 
				return false;
			return true;
		}
		bool IsSingleColumnDefinition() {
			if (la.val == "@" || la.val == "[" || la.val == "#")
				return false;
			return true;
		}
		int CorrectSheetDefinition(int sheetDefinitionIndex, bool isRef) {
			if (sheetDefinitionIndex < 0)
				return sheetDefinitionIndex;
			SheetDefinition sheetDefinition = parserContext.GetSheetDefinition(sheetDefinitionIndex);
			if (sheetDefinition.ExternalReferenceIndex >= 0)
				return sheetDefinitionIndex;
			string sheetNameStart = sheetDefinition.SheetNameStart;
			if (!string.IsNullOrEmpty(sheetNameStart) && string.IsNullOrEmpty(sheetDefinition.SheetNameEnd))
				if (!parserContext.IsCurrentWorkbookContainsSheet(sheetNameStart)) {
					sheetDefinition = sheetDefinition.Clone();
					ExternalLinkInfo externalLinkInfo = parserContext.GetExternalLinkByName(sheetNameStart);
					if (externalLinkInfo != null) {
						if (isRef && externalLinkInfo.ExternalLink.Workbook.SheetCount > 0) {
							ExternalWorksheet externalSheet = externalLinkInfo.ExternalLink.Workbook.Sheets[0];
							sheetDefinition.SheetNameStart = externalSheet.Name;
						}
						else
							sheetDefinition.SheetNameStart = string.Empty;
						sheetDefinition.ExternalReferenceIndex = externalLinkInfo.Index + 1;
						return parserContext.RegisterSheetDefinition(sheetDefinition);
					}
					if (parserContext.AllowsToCreateExternal) {
						sheetDefinition.ExternalReferenceIndex = parserContext.RegisterExternalLink(sheetNameStart, sheetDefinition.SheetNameEnd, -1, sheetNameStart);
						return parserContext.RegisterSheetDefinition(sheetDefinition);
					}
				}
			return sheetDefinitionIndex;
		}
		ParsedThingName GetDefinedNamePtg(OperandDataType dataType, int sheetDefinitionIndex, string name, int startPosition, int length) {
			ParsedThingName result;
			if (!parserContext.ImportExportMode && parserContext.DefinedNameProcessing && sheetDefinitionIndex < 0) {
				if(parserContext.HasSheetScopedDefinedName(name))
					sheetDefinitionIndex = GetSheetDefinitionForDefinedName();
			}
			OperandDataType correctedDataType = GetCorrectedDataType(dataType, OperandDataType.Reference);
			if (sheetDefinitionIndex < 0)
				result = new ParsedThingName(name, correctedDataType);
			else {
				sheetDefinitionIndex = CorrectSheetDefinition(sheetDefinitionIndex, false);
				ParsedThingNameX externalName = new ParsedThingNameX(name, sheetDefinitionIndex, correctedDataType);
				result = externalName;
			}
			parserContext.RegisterDefinedName(result, startPosition, length);
			return result;
		}
		protected internal List<string> SplitArray(string expression, char separator) {
			List<string> result = new List<string>();
			bool isQuotationMarksClose = true;
			int startIndex = 0;
			int count = expression.Length;
			for (int i = 0; i < count; i++) {
				char currentSimbol = expression[i];
				if (currentSimbol == '"')
					isQuotationMarksClose = !isQuotationMarksClose;
				else if (currentSimbol == separator && isQuotationMarksClose) {
					result.Add(expression.Substring(startIndex, i - startIndex).Trim());
					startIndex = i + 1;
				}
			}
			result.Add(expression.Substring(startIndex, count - startIndex).Trim());
			return result;
		}
		VariantValue GetArrayElement(string elementToParse) {
			if (elementToParse.Length > 1 && elementToParse.StartsWith("\"") && elementToParse.EndsWith("\""))
				return elementToParse.Substring(1, elementToParse.Length - 2);
			ICellError error = parserContext.CreateError(elementToParse);
			if (error != null)
				return error.Value;
			double doubleResult;
			if (double.TryParse(elementToParse, NumberStyles.Float, parserContext.Culture, out doubleResult))
				return doubleResult;
			bool boolResult;
			if (parserContext.TryParseBoolean(elementToParse, out boolResult))
				return boolResult;
			return elementToParse;
		}
		bool CanBeInReferenceExpression(IParsedThing thing, bool checkNumbers) {
			if (thing is ParsedThingStringValue || thing is ParsedThingArray || thing is ParsedThingError)
				return false;
			if (checkNumbers && (thing is ParsedThingInteger || thing is ParsedThingNumeric))
				return false;
			return true;
		}
		int TryGetRowFromInteger(IParsedThing thing) {
			int value = -1;
			ParsedThingInteger intThing = thing as ParsedThingInteger;
			if (intThing != null)
				value = intThing.Value;
			else {
				ParsedThingNumeric numericThing = thing as ParsedThingNumeric;
				if (numericThing != null) {
					double dValue = numericThing.Value;
					double truncated = WorksheetFunctionBase.Truncate(dValue);
					if (truncated == dValue)
						value = (int)truncated;
				}
			}
			if (value > 0 && value <= IndicesChecker.MaxRowCount)
				return value - 1;
			return -1;
		}
		CellPosition CreateColumnFromDefinedName(IParsedThing thing) {
			ParsedThingName nameThing = thing as ParsedThingName;
			if (nameThing != null) {
				CellPosition position = CellReferenceParser.TryParsePart(nameThing.DefinedName);
				if (position.IsColumn)
					return position;
			}
			return CellPosition.InvalidValue;
		}
		bool TryCreateRange(ParsedExpression expression, CellReferenceParseHelper helper, OperandDataType dataType) {
			IParsedThing lastThing = expression[expression.Count - 1];
			if (!CanBeInReferenceExpression(lastThing, false))
				return false;
			if (helper.PrevThingIndex >= 0) {
				IParsedThing prevThing = expression[helper.PrevThingIndex];
				ParsedThingRef refThing = prevThing as ParsedThingRef;
				if (refThing != null)
					return TryCreateRangeRef(expression, helper, dataType);
				int intValue = TryGetRowFromInteger(prevThing);
				if (intValue >= 0)
					return TryCreateRangeInteger(expression, intValue, helper, dataType);
				CellPosition position = CreateColumnFromDefinedName(prevThing);
				if (position.IsColumn)
					return TryCreateRangeColumnPosition(expression, helper, position, dataType);
			}
			if (lastThing is ParsedThingInteger || lastThing is ParsedThingNumeric || lastThing is ParsedThingRef || lastThing is ParsedThingName)
				helper.PrevThingIndex = expression.Count - 1;
			expression.Add(ParsedThingRange.Instance);
			return true;
		}
		bool TryCreateRangeColumnPosition(ParsedExpression expression, CellReferenceParseHelper helper, CellPosition position, OperandDataType dataType) {
			IParsedThing lastThing = expression[expression.Count - 1];
			ParsedThingRef lastRefThing = lastThing as ParsedThingRef;
			if (helper.CurSheetDefinitionIndex < 0) {
				CellPosition lastPosition;
				if (lastRefThing != null)
					lastPosition = lastRefThing.Position;
				else
					lastPosition = CreateColumnFromDefinedName(lastThing);
				if (lastPosition.IsColumn) {
					CellRange range = CellIntervalRange.CreateColumnInterval(null, position.Column, position.ColumnType, lastPosition.Column, lastPosition.ColumnType);
					ParsedThingBase areaThing = CreateParsedThingArea(range, helper, dataType);
					ParsedThingName nameLastThing = lastThing as ParsedThingName;
					if (nameLastThing != null)
						parserContext.UnRegisterDefinedName(nameLastThing);
					expression.RemoveAt(expression.Count - 1);
					nameLastThing = expression[helper.PrevThingIndex] as ParsedThingName;
					if (nameLastThing != null)
						parserContext.UnRegisterDefinedName(nameLastThing);
					expression.RemoveAt(helper.PrevThingIndex);
					expression.Insert(helper.PrevThingIndex, areaThing);
					helper.PrevThingIndex = -1;
				}
				else {
					helper.PrevThingIndex = expression.Count - 1;
					expression.Add(ParsedThingRange.Instance);
				}
			}
			else {
				helper.PrevThingIndex = expression.Count - 1;
				expression.Add(ParsedThingRange.Instance);
			}
			return true;
		}
		void ReplaceR1C1RefToArea(ParsedExpression expression, CellPosition refThingPosition, CellReferenceParseHelper helper, OperandDataType dataType) {
			CellIntervalRange range;
			if (refThingPosition.IsRow)
				range = CellIntervalRange.CreateRowInterval(null, refThingPosition.Row, refThingPosition.RowType, refThingPosition.Row, refThingPosition.RowType);
			else
				range = CellIntervalRange.CreateColumnInterval(null, refThingPosition.Column, refThingPosition.ColumnType, refThingPosition.Column, refThingPosition.ColumnType);
			ParsedThingWithDataType areaThing = CreateParsedThingArea(range, helper, dataType);
			expression.RemoveAt(helper.PrevThingIndex);
			expression.Insert(helper.PrevThingIndex, areaThing);
		}
		bool TryCreateRangeRef(ParsedExpression expression, CellReferenceParseHelper helper, OperandDataType dataType) {
			int expressionCount = expression.Count;
			ParsedThingRef refThing = expression[helper.PrevThingIndex] as ParsedThingRef;
			CellPosition refThingPosition = refThing.Position;
			IParsedThing lastThing = expression[expressionCount - 1];
			ParsedThingRef lastRefThing = lastThing as ParsedThingRef;
			if (lastRefThing != null)
				return TryCreateAreaFromRefs(refThingPosition, lastRefThing.Position, expression, helper, dataType);
			if (refThingPosition.IsColumnOrRow) {
				if (!parserContext.UseR1C1ReferenceStyle) {
					if (refThingPosition.IsRow) {
						int intValue = TryGetRowFromInteger(lastThing);
						if (intValue >= 0) {
							CellPosition positionFromInt = new CellPosition(-1, intValue, PositionType.Relative, PositionType.Relative);
							return TryCreateAreaFromRefs(refThingPosition, positionFromInt, expression, helper, dataType);
						}
					}
					else {
						CellPosition positionFromName = CreateColumnFromDefinedName(lastThing);
						if (positionFromName.IsColumn)
							return TryCreateAreaFromRefs(refThingPosition, positionFromName, expression, helper, dataType);
					}
					return false;
				}
				else {
					ReplaceR1C1RefToArea(expression, refThingPosition, helper, dataType);
					return true;
				}
			}
			ParsedThingRefBase resultRefThing = CreateParsedThingRef(refThing.Position, helper, OperandDataType.Reference);
			expression.RemoveAt(helper.PrevThingIndex);
			expression.Insert(helper.PrevThingIndex, resultRefThing);
			if (lastThing is ParsedThingInteger || lastThing is ParsedThingNumeric || lastThing is ParsedThingName)
				helper.PrevThingIndex = expressionCount - 1;
			else
				helper.PrevThingIndex = -1;
			expression.Add(ParsedThingRange.Instance);
			return true;
		}
		bool TryCreateAreaFromRefs(CellPosition firstPosition, CellPosition lastPosition, ParsedExpression expression, CellReferenceParseHelper helper, OperandDataType dataType) {
			if (helper.CurSheetDefinitionIndex < 0) {
				CellRange range = null;
				if (firstPosition.IsColumnOrRow) {
					if (!lastPosition.IsColumnOrRow || firstPosition.IsRow != lastPosition.IsRow) {
						if (!parserContext.UseR1C1ReferenceStyle)
							return false;
						else
							ReplaceR1C1RefToArea(expression, firstPosition, helper, dataType);
					}
					else
						if (firstPosition.IsColumn)
							range = CellIntervalRange.CreateColumnInterval(null, firstPosition.Column, firstPosition.ColumnType, lastPosition.Column, lastPosition.ColumnType);
						else
							range = CellIntervalRange.CreateRowInterval(null, firstPosition.Row, firstPosition.RowType, lastPosition.Row, lastPosition.RowType);
				}
				else {
					if (!lastPosition.IsColumnOrRow)
						range = new CellRange(null, firstPosition, lastPosition);
				}
				if (range != null) {
					ParsedThingBase areaThing = CreateParsedThingArea(range, helper, dataType);
					expression.RemoveAt(expression.Count - 1);
					expression.RemoveAt(helper.PrevThingIndex);
					expression.Insert(helper.PrevThingIndex, areaThing);
					helper.PrevThingIndex = -1;
				}
				else {
					helper.PrevThingIndex = expression.Count - 1;
					expression.Add(ParsedThingRange.Instance);
				}
			}
			else {
				ParsedThingBase refThing = CreateParsedThingRef(firstPosition, helper, OperandDataType.Reference);
				expression.RemoveAt(helper.PrevThingIndex);
				expression.Insert(helper.PrevThingIndex, refThing);
				helper.PrevThingIndex = expression.Count - 1;
				expression.Add(ParsedThingRange.Instance);
			}
			return true;
		}
		bool TryCreateRangeInteger(ParsedExpression expression, int prevThingValue, CellReferenceParseHelper helper, OperandDataType dataType) {
			IParsedThing lastThing = expression[expression.Count - 1];
			int intValue = TryGetRowFromInteger(lastThing);
			CellIntervalRange range;
			ParsedThingRef lastRefThing = lastThing as ParsedThingRef;
			if (intValue >= 0)
				range = CellIntervalRange.CreateRowInterval(null, prevThingValue, PositionType.Relative, intValue, PositionType.Relative);
			else if (lastRefThing != null && lastRefThing.Position.IsRow)
				range = CellIntervalRange.CreateRowInterval(null, prevThingValue, PositionType.Relative, lastRefThing.Position.Row, lastRefThing.Position.RowType);
			else
				return false;
			ParsedThingBase areaThing = CreateParsedThingArea(range, helper, dataType);
			expression.RemoveAt(expression.Count - 1);
			expression.RemoveAt(helper.PrevThingIndex);
			expression.Insert(helper.PrevThingIndex, areaThing);
			helper.PrevThingIndex = -1;
			return true;
		}
		ParsedThingRefBase CreateParsedThingRef(CellPosition position, CellReferenceParseHelper helper, OperandDataType dataType) {
			ParsedThingRefBase result;
			int sheetDefinitionIndex = helper.PrevSheetDefinitionIndex;
			if (parserContext.DefinedNameProcessing && sheetDefinitionIndex < 0 && parserContext.IsLocalWorkbookProcessing())
				sheetDefinitionIndex = GetSheetDefinitionForDefinedName();
			sheetDefinitionIndex = CorrectSheetDefinition(sheetDefinitionIndex, true);
			OperandDataType correctedDataType = GetRefDataType(dataType);
			if (parserContext.DefinedNameProcessing || parserContext.RelativeToCurrentCell) {
				CellOffset offset = position.ToCellOffset(parserContext.CurrentColumnIndex, parserContext.CurrentRowIndex);
				if (sheetDefinitionIndex >= 0)
					result = new ParsedThingRef3dRel(offset, sheetDefinitionIndex, correctedDataType);
				else
					result = new ParsedThingRefRel(offset, correctedDataType);
			}
			else {
				if (sheetDefinitionIndex >= 0)
					result = new ParsedThingRef3d(position, sheetDefinitionIndex, correctedDataType);
				else
					result = new ParsedThingRef(position, correctedDataType);
			}
			parserContext.RegisterCellRange(new CellRange(null, position, position), helper.PrevSheetDefinitionIndex, helper.PrevStartCharPosition, helper.PrevEndCharPosition - helper.PrevStartCharPosition);
			return result;
		}
		ParsedThingWithDataType CreateParsedThingArea(CellRange range, CellReferenceParseHelper helper, OperandDataType dataType) {
			ParsedThingWithDataType result;
			int sheetDefinitionIndex = helper.PrevSheetDefinitionIndex;
			if (parserContext.DefinedNameProcessing && sheetDefinitionIndex < 0 && parserContext.IsLocalWorkbookProcessing())
				sheetDefinitionIndex = GetSheetDefinitionForDefinedName();
			sheetDefinitionIndex = CorrectSheetDefinition(sheetDefinitionIndex, true);
			if (parserContext.DefinedNameProcessing || parserContext.RelativeToCurrentCell) {
				CellOffset offset1 = range.TopLeft.ToCellOffset(parserContext.CurrentColumnIndex, parserContext.CurrentRowIndex);
				CellOffset offset2 = range.BottomRight.ToCellOffset(parserContext.CurrentColumnIndex, parserContext.CurrentRowIndex);
				if (sheetDefinitionIndex >= 0)
					result = new ParsedThingArea3dRel(offset1, offset2, sheetDefinitionIndex);
				else
					result = new ParsedThingAreaN(offset1, offset2);
			}
			else {
				if (sheetDefinitionIndex >= 0)
					result = new ParsedThingArea3d(range, sheetDefinitionIndex);
				else
					result = new ParsedThingArea(range);
			}
			result.DataType = GetCorrectedDataType(dataType, OperandDataType.Reference);
			parserContext.RegisterCellRange(range, sheetDefinitionIndex, helper.PrevStartCharPosition, helper.CurEndCharPosition - helper.PrevStartCharPosition);
			return result;
		}
		ParsedThingFunc GetFunctionPtg(ISpreadsheetFunction function, int paramCount, int sheetDefinitionIndex, string functionName, OperandDataType dataType) {
			sheetDefinitionIndex = CorrectSheetDefinition(sheetDefinitionIndex, false);
			if (sheetDefinitionIndex >= 0)
				return new ParsedThingUnknownFuncExt(function.Name, paramCount, sheetDefinitionIndex, dataType);
			if (function is NotExistingFunction) {
				if (functionName.StartsWith(ParsedThingAddinFunc.ADDIN_PREFIX)) {
					string name = functionName.Remove(0, ParsedThingAddinFunc.ADDIN_PREFIX.Length);
					return new ParsedThingAddinFunc(name, paramCount, dataType);
				}
				return new ParsedThingUnknownFunc(functionName, paramCount, dataType);
			}
			if (function is CustomFunction)
				return new ParsedThingCustomFunc(function, paramCount, dataType);
			if (function.HasFixedParametersCount)
				return new ParsedThingFunc(function.Code, dataType);
			return new ParsedThingFuncVar(function.Code, paramCount, dataType);
		}
		bool CheckReferenceDataType(OperandDataType dataType) {
			if (dataType == OperandDataType.Reference) {
				SemErr("Operator does not match reference data type");
				return false;
			}
			return true;
		}
		OperandDataType GetCorrectedDataType(OperandDataType dataType, OperandDataType defaultDataType) {
			if (dataType == OperandDataType.Default)
				return defaultDataType;
			if (dataType == OperandDataType.None || (dataType & OperandDataType.Reference) > 0)
				return OperandDataType.Reference;
			return GetCorrectedDataTypeCore(dataType);
		}
		OperandDataType GetCorrectedDataTypeCore(OperandDataType dataType) {
			if (!parserContext.ArrayFormulaProcessing && !parserContext.DefinedNameProcessing) {
				if ((dataType & OperandDataType.Value) > 0)
					return OperandDataType.Value;
				if ((dataType & OperandDataType.Reference) > 0)
					return OperandDataType.Reference;
				return OperandDataType.Array;
			}
			else {
				if ((dataType & OperandDataType.Array) > 0)
					return OperandDataType.Array;
				if ((dataType & OperandDataType.Reference) > 0)
					return OperandDataType.Reference;
				return OperandDataType.Value;
			}
		}
		OperandDataType GetFunctionParameterDataType(OperandDataType parameterDataType, bool arrayResultCorrectionMode, bool valueParameterToArrayMode) {
			if (arrayResultCorrectionMode && (parameterDataType == OperandDataType.Value || (parameterDataType & OperandDataType.Array) > 0))
				return OperandDataType.Array | (parameterDataType & OperandDataType.Reference);
			if (valueParameterToArrayMode && parameterDataType == OperandDataType.Value)
				parameterDataType |= OperandDataType.Array;
			return parameterDataType;
		}
		OperandDataType GetArrayDataType(OperandDataType dataType) {
			if (dataType == OperandDataType.Default)
				return OperandDataType.Array;
			if (dataType == OperandDataType.None)
				return OperandDataType.Reference;
			if ((dataType & OperandDataType.Array) > 0)
				return OperandDataType.Array;
			if ((dataType & OperandDataType.Value) > 0)
				return OperandDataType.Value;
			SemErr("Can not use array as reference data type.");
			return OperandDataType.None;
		}
		OperandDataType GetRefDataType(OperandDataType dataType) {
			if (dataType == OperandDataType.Default)
				return OperandDataType.Reference;
			if (dataType == OperandDataType.None || (dataType & OperandDataType.Reference) > 0)
				return OperandDataType.Reference;
			if ((dataType & OperandDataType.Value) > 0 && !parserContext.ArrayFormulaProcessing && !parserContext.DefinedNameProcessing)
				return OperandDataType.Value;
			return GetCorrectedDataTypeCore(dataType);
		}
		OperandDataType GetRefBinaryDataType(OperandDataType dataType) {
			if (dataType == OperandDataType.Default || dataType == OperandDataType.None)
				return OperandDataType.Reference;
			if (!parserContext.ArrayFormulaProcessing && !parserContext.DefinedNameProcessing) {
				if ((dataType & OperandDataType.Reference) > 0)
					return OperandDataType.Reference;
				if ((dataType & OperandDataType.Value) > 0)
					return OperandDataType.Value;
				return OperandDataType.Array;
			}
			else {
				if ((dataType & OperandDataType.Array) > 0)
					return OperandDataType.Array;
				if ((dataType & OperandDataType.Reference) > 0)
					return OperandDataType.Reference;
				return OperandDataType.Value;
			}
		}
		OperandDataType GetFunctionDataType(ISpreadsheetFunction function, OperandDataType dataType) {
			if (dataType == OperandDataType.Default)
				dataType = function.GetDefaultDataType();
			if (((function.ReturnDataType & OperandDataType.Reference) <= 0)) {
				if (dataType == OperandDataType.Reference) {
					SemErr("Function can not return reference data type");
					return OperandDataType.None;
				}
				else
					dataType = dataType & ~OperandDataType.Reference;
			}
			if ((dataType & OperandDataType.Array) > 0) {
				if (function.ReturnDataType == OperandDataType.Value && parserContext.ArrayFormulaProcessing)
					dataType = OperandDataType.Array;
				else if (function.ReturnDataType == OperandDataType.Array)
					dataType = OperandDataType.Array;
			}
			return GetCorrectedDataType(dataType, OperandDataType.Reference);
		}
		void CorrectLastThingDataType(ParsedExpression expression, OperandDataType dataType) {
			IParsedThing thing;
			int i = expression.Count - 1;
			do {
				thing = expression[i];
				i--;
			} while (thing is ParsedThingParentheses && i >= 0);
			ParsedThingFunc funcThing = thing as ParsedThingFunc;
			if (funcThing != null)
				thing.DataType = GetFunctionDataType(funcThing.Function, dataType);
			else if (thing is BinaryParsedThing || thing is UnaryParsedThing) {
				if (!(thing is BinaryReferenceParsedThing) && !CheckReferenceDataType(dataType))
					return;
			}
			else if (thing is ParsedThingMemBase)
				thing.DataType = GetRefBinaryDataType(dataType);
			else if (thing is ParsedThingRefBase)
				thing.DataType = GetRefDataType(dataType);
			else if (thing is ParsedThingArea || thing is ParsedThingAreaN || thing is ParsedThingName || thing is ParsedThingTable)
				thing.DataType = GetCorrectedDataType(dataType, OperandDataType.Reference);
			else if (thing is ParsedThingArray)
				thing.DataType = GetArrayDataType(dataType);
		}
		int TryAddAttrSpaceBeforeBaseExpression(int startPos, int endPos, ParsedThingList expression, ParsedThingAttrSpaceType spacePosition) {
			if (endPos > startPos) {
				string attrString = scanner.Buffer.GetString(startPos, endPos);
				string[] lines = attrString.Split(new string[] { Environment.NewLine, "\r\n", "\r", "\n" }, StringSplitOptions.None);
				int linesCount = lines.Length;
				int result = 0;
				if (linesCount > 1) {
					expression.Add(new ParsedThingAttrSpace((ParsedThingAttrSpaceType)(spacePosition + 1), linesCount - 1));
					result++;
				}
				if (lines[linesCount - 1].Length > 0) {
					expression.Add(new ParsedThingAttrSpace(spacePosition, lines[linesCount - 1].Length));
					result++;
				}
				return result;
			}
			return 0;
		}
		int GetSheetDefinitionForDefinedName() {
			SheetDefinition sheetDefinition = new SheetDefinition();
			string currentSheetName = parserContext.GetCurrentSheetName();
			if (!string.IsNullOrEmpty(currentSheetName) && !parserContext.ImportExportMode)
				sheetDefinition.SheetNameStart = currentSheetName;
			else {
				sheetDefinition.ExternalReferenceIndex = 0;
				sheetDefinition.ValidReference = false;
			}
			return parserContext.RegisterSheetDefinition(sheetDefinition);
		}
		bool CheckResultExpression(ParsedExpression expression) {
			if (expression == null)
				return false;
			for (int i = 0; i < expression.Count; i++) {
				ParsedThingName nameThing = expression[i] as ParsedThingName;
				if (nameThing != null && !CheckNameThing(nameThing))
					return false;
			}
			return true;
		}
		bool CheckNameThing(ParsedThingName nameThing) {
			string name = nameThing.DefinedName;
			if (!WorkbookDataContext.IsIdent(name)) {
				ParsedThingNameX nameXthing = nameThing as ParsedThingNameX;
				if (nameXthing != null)
					if (parserContext.SheetDefinitionRefersToDdeWorkbook(nameXthing.SheetDefinitionIndex))
						return true;
				SemErr("Invalid defined name");
				return false;
			}
			return true;
		}
		void ApplyFormulaProperties(ISpreadsheetFunction function) {
			if (function.IsVolatile)
				formulaProperties |= FormulaProperties.HasVolatileFunction;
			if (function is CustomFunction)
				formulaProperties |= FormulaProperties.HasCustomFunction;
			if ((function.Code & 0x4100) == 0x4100)
				formulaProperties |= FormulaProperties.HasInternalFunction;
			if (function.Code == FormulaCalculator.FuncRTDCode)
				formulaProperties |= FormulaProperties.HasFunctionRTD;
		}
		void PrepareArrayThing(ParsedExpression expression, string value, OperandDataType dataType) {
			List<string> rows = SplitArray(value, ';');
			IList<VariantValue> elements = new List<VariantValue>();
			int width = Int32.MinValue;
			int height = rows.Count;
			char elementDelimiter = listSeparator == ',' ? listSeparator : '\\';
			foreach (string row in rows) {
				List<string> elementsInRow = SplitArray(row, elementDelimiter);
				int currentElementCount = elementsInRow.Count;
				if (width != currentElementCount && width != Int32.MinValue)
					SemErr("Incorrect array definition");
				width = currentElementCount;
				for (int elementIndex = 0; elementIndex < currentElementCount; elementIndex++) {
					string elementToParse = elementsInRow[elementIndex];
					VariantValue elementExpression = GetArrayElement(elementToParse);
					elements.Add(elementExpression);
				}
			}
			ParsedThingArray arrayThing = new ParsedThingArray();
			VariantArray array = new VariantArray();
			array.SetValues(elements, width, height);
			arrayThing.ArrayValue = array;
			arrayThing.DataType = GetArrayDataType(dataType);
			expression.Add(arrayThing);
		}
		public bool CheckTableReference(ParsedThingTable thing, Table table) {
			if (table == null)
				return false;
			if (!tableAllowedRowsTable.Contains(thing.IncludedRows))
				return false;
			if (table == null)
				return false;
			if (!string.IsNullOrEmpty(thing.ColumnStart))
				if (table.Columns[thing.ColumnStart] == null)
					return false;
			if (!string.IsNullOrEmpty(thing.ColumnEnd))
				if (table.Columns[thing.ColumnEnd] == null)
					return false;
			return true;
		}
		IParsedThing CreateCellErrorInstance(string errorText, OperandDataType dataType, int sheetDefinitionIndex) {
			ICellError cellError = parserContext.CreateError(errorText);
			if (cellError == null)
				return null;
			IParsedThing result = null;
			if (cellError != ReferenceError.Instance) {
				if (CheckReferenceDataType(dataType) && sheetDefinitionIndex < 0)
					result = new ParsedThingError(cellError);
			}
			else {
				bool isLocalWorkbookProcessing = parserContext.IsLocalWorkbookProcessing();
				bool definedNameProcessing = parserContext.DefinedNameProcessing;
				if (definedNameProcessing && sheetDefinitionIndex < 0 && isLocalWorkbookProcessing)
					sheetDefinitionIndex = GetSheetDefinitionForDefinedName();
				OperandDataType preparedDataType = GetCorrectedDataType(dataType, OperandDataType.Reference);
				if (sheetDefinitionIndex >= 0) {
					sheetDefinitionIndex = CorrectSheetDefinition(sheetDefinitionIndex, true);
					result = new ParsedThingErr3d(sheetDefinitionIndex, preparedDataType);
				}
				else {
					if (definedNameProcessing && !isLocalWorkbookProcessing)
						result = new ParsedThingError(VariantValue.ErrorReference.ErrorValue);
					else
						result = new ParsedThingRefErr(preparedDataType);
				}
			}
			return result;
		}
	}
}
