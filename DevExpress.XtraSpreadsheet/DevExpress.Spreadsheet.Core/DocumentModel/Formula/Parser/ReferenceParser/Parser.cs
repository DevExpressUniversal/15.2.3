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

using System.Collections;
using System.IO;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
namespace DevExpress.XtraSpreadsheet.Model {
public partial class ReferenceParser : IDisposable {
	const int _EOF = 0;
	const int _ident = 1;
	const int _positiveinumber = 2;
	const int _fnumber = 3;
	const int _space = 4;
	const int _wideident = 5;
	const int _quotedOpenBracket = 6;
	const int _quotedSymbol = 7;
	const int _pathPart = 8;
	const int _trueConstant = 9;
	const int _falseConstant = 10;
	const int maxT = 39;
	const bool T = true;
	const bool x = false;
	const int minErrDist = 1;
	ReferenceScanner scanner;
	char listSeparator;
	Token t;	
	Token la;   
	int errDist = minErrDist;
	int bracketsCounter;
	int functionLevel;
	OperandDataType dataType;
	bool wasReferenceBinaryOperator = false;
	FormulaProperties formulaProperties;
	IExpressionParserContext parserContext;
	public ReferenceParser() {
		this.scanner = new ReferenceScanner();
	}
	#region Properties
	public ParserErrors Errors { get { return parserContext.ErrorList; } }
	#endregion 
	void SynErr (int n) {
		if (errDist >= minErrDist) Errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}
	public void SemErr (string msg) {
		if (errDist >= minErrDist) Errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }
			la = t;
		}
	}
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	void ReferenceParserGrammar() {
		ParsedExpression result = parserContext.Result; 
		Reference(dataType, ref result);
	}
	void Reference(OperandDataType dataType, ref ParsedExpression expression) {
		CommonCellReference(dataType, expression);
		if (formulaProperties != FormulaProperties.None)
		expression.Insert(0, new ParsedThingAttrSemi(formulaProperties));
		if (Errors.Count > 0) 
		return;
		CheckResultExpression(expression);
	}
	void CommonCellReference(OperandDataType dataType, ParsedExpression expression) {
		LogicalClause(dataType, expression);
	}
	void LogicalClause(OperandDataType dataType, ParsedExpression expression) {
		ConcatenateClause(dataType, expression);
		if (Errors.Count > 0)
		return;
		while (StartOf(1)) {
			ParsedThingList thing = new ParsedThingList();
			if(!CheckReferenceDataType(dataType))
			return;
					if (dataType == OperandDataType.Default)
						dataType = OperandDataType.Array;
			dataType = dataType & (OperandDataType.Value | OperandDataType.Array);
			CorrectLastThingDataType(expression, dataType);
			TryAddAttrSpaceBeforeBaseExpression(t.pos + t.val.Length, la.pos, thing, ParsedThingAttrSpaceType.SpaceBeforeBaseExpression);
			switch (la.kind) {
			case 11: {
				Get();
				thing.Add(ParsedThingLess.Instance);	
				break;
			}
			case 12: {
				Get();
				thing.Add(ParsedThingGreater.Instance);	
				break;
			}
			case 13: {
				Get();
				thing.Add(ParsedThingLessEqual.Instance);	
				break;
			}
			case 14: {
				Get();
				thing.Add(ParsedThingGreaterEqual.Instance);	
				break;
			}
			case 15: {
				Get();
				thing.Add(ParsedThingNotEqual.Instance);	
				break;
			}
			case 16: {
				Get();
				thing.Add(ParsedThingEqual.Instance);	
				break;
			}
			}
			ConcatenateClause(dataType, expression);
			if (Errors.Count > 0)
			return;
			expression.AddRange(thing);
		}
	}
	void ConcatenateClause(OperandDataType dataType, ParsedExpression expression) {
		AdditionClause(dataType, expression);
		if (Errors.Count > 0)
		return;
		while (la.kind == 17) {
			if(!CheckReferenceDataType(dataType))
			return;
					if (dataType == OperandDataType.Default)
						dataType = OperandDataType.Array;
			dataType = dataType & (OperandDataType.Value | OperandDataType.Array);
			CorrectLastThingDataType(expression, dataType);
			ParsedThingList attrThing = new ParsedThingList();
			TryAddAttrSpaceBeforeBaseExpression(t.pos + t.val.Length, la.pos, attrThing, ParsedThingAttrSpaceType.SpaceBeforeBaseExpression);
			Get();
			AdditionClause(dataType, expression);
			if (Errors.Count > 0)
			return;
			if(attrThing != null)
			expression.AddRange(attrThing);
			expression.Add(ParsedThingConcat.Instance);
		}
	}
	void AdditionClause(OperandDataType dataType, ParsedExpression expression) {
		MultiplicationClause(dataType, expression);
		if (Errors.Count > 0)
		return;
		while (la.kind == 18 || la.kind == 19) {
			ParsedThingList thing = new ParsedThingList();
			if(!CheckReferenceDataType(dataType))
			return;
					if (dataType == OperandDataType.Default)
						dataType = OperandDataType.Array;
			dataType = dataType & (OperandDataType.Value | OperandDataType.Array);
			CorrectLastThingDataType(expression, dataType);
			TryAddAttrSpaceBeforeBaseExpression(t.pos + t.val.Length, la.pos, thing, ParsedThingAttrSpaceType.SpaceBeforeBaseExpression);
			if (la.kind == 18) {
				Get();
				thing.Add(ParsedThingAdd.Instance);	
			} else {
				Get();
				thing.Add(ParsedThingSubtract.Instance);	
			}
			MultiplicationClause(dataType, expression);
			if (Errors.Count > 0)
			return;
			expression.AddRange(thing);
		}
	}
	void MultiplicationClause(OperandDataType dataType, ParsedExpression expression) {
		PowerClause(dataType, expression);
		if (Errors.Count > 0)
		return;
		while (la.kind == 20 || la.kind == 21) {
			ParsedThingList thing = new ParsedThingList();
			if(!CheckReferenceDataType(dataType))
			return;
					if (dataType == OperandDataType.Default)
						dataType = OperandDataType.Array;
			dataType = dataType & (OperandDataType.Value | OperandDataType.Array);
			CorrectLastThingDataType(expression, dataType);
			TryAddAttrSpaceBeforeBaseExpression(t.pos + t.val.Length, la.pos, thing, ParsedThingAttrSpaceType.SpaceBeforeBaseExpression);
			if (la.kind == 20) {
				Get();
				thing.Add(ParsedThingMultiply.Instance);	
			} else {
				Get();
				thing.Add(ParsedThingDivide.Instance);	
			}
			PowerClause(dataType, expression);
			if (Errors.Count > 0)
			return;
			expression.AddRange(thing);
		}
	}
	void PowerClause(OperandDataType dataType, ParsedExpression expression) {
		UnaryClause(dataType, expression);
		if (Errors.Count > 0)
		return;
		while (la.kind == 22) {
			if(!CheckReferenceDataType(dataType))
			return;
					if (dataType == OperandDataType.Default)
						dataType = OperandDataType.Array;
			dataType = dataType & (OperandDataType.Value | OperandDataType.Array);
			CorrectLastThingDataType(expression, dataType);
			ParsedThingList attrThing = new ParsedThingList();
			TryAddAttrSpaceBeforeBaseExpression(t.pos + t.val.Length, la.pos, attrThing, ParsedThingAttrSpaceType.SpaceBeforeBaseExpression);
			Get();
			UnaryClause(dataType, expression);
			if (Errors.Count > 0)
			return; 
			expression.AddRange(attrThing);
			expression.Add(ParsedThingPower.Instance);
		}
	}
	void UnaryClause(OperandDataType dataType, ParsedExpression expression) {
		if (StartOf(2)) {
			PercentClause(dataType, expression);
		} else if (la.kind == 18 || la.kind == 19) {
			ParsedThingList attrThing = new ParsedThingList();
			IParsedThing thing = null;
			if(!CheckReferenceDataType(dataType))
			return;
					if (dataType == OperandDataType.Default)
						dataType = OperandDataType.Array;
			dataType = dataType & (OperandDataType.Value | OperandDataType.Array);
			TryAddAttrSpaceBeforeBaseExpression(t.pos + t.val.Length, la.pos, attrThing, ParsedThingAttrSpaceType.SpaceBeforeBaseExpression);
			int sign = 1;
			int expressionStartPos = expression.Count;		
			if (la.kind == 18) {
				Get();
				thing = ParsedThingUnaryPlus.Instance;	
			} else {
				Get();
				thing = ParsedThingUnaryMinus.Instance;	
				sign = -1; 
			}
			UnaryClause(dataType, expression);
			if (Errors.Count > 0)
			return;
			if(expression.Count - expressionStartPos != 1){
			expression.AddRange(attrThing);
			expression.Add(thing);
			}
			else{
					 IParsedThing innerThing = expression[expressionStartPos];
						ParsedThingInteger intThing = innerThing as ParsedThingInteger;
						if (intThing != null) {
							if (sign < 0) {
								ParsedThingNumeric doubleThing = new ParsedThingNumeric() { Value = intThing.Value * sign };
								expression.RemoveAt(expressionStartPos);
					expression.AddRange(attrThing);
								expression.Add(doubleThing);
							}
							else{
								intThing.Value *= sign;
					expression.InsertRange(expressionStartPos, attrThing);
				}
						}
						else {
							ParsedThingNumeric doubleThing = innerThing as ParsedThingNumeric;
							if (doubleThing != null) {
								doubleThing.Value *= sign;
					expression.InsertRange(expressionStartPos, attrThing);
							}else{
					expression.AddRange(attrThing);
					expression.Add(thing);
				}
						}
					}
		} else SynErr(40);
	}
	void PercentClause(OperandDataType dataType, ParsedExpression expression) {
		RangeUnionClause(dataType, expression);
		while (la.kind == 23) {
			ParsedThingList attrThing = new ParsedThingList();
			TryAddAttrSpaceBeforeBaseExpression(t.pos + t.val.Length, la.pos, attrThing, ParsedThingAttrSpaceType.SpaceBeforeBaseExpression);
			Get();
			if(!CheckReferenceDataType(dataType))
			return;
					if (dataType == OperandDataType.Default)
						dataType = OperandDataType.Array;
			dataType = dataType & (OperandDataType.Value | OperandDataType.Array);
			CorrectLastThingDataType(expression, dataType);
			expression.AddRange(attrThing);
			expression.Add(ParsedThingPercent.Instance);	
		}
	}
	void RangeUnionClause(OperandDataType dataType, ParsedExpression expression) {
		bool wasUnion = false;
		int expressionStartPosition = expression.Count;
		RangeIntersectionClause(dataType, expression);
		if (Errors.Count > 0)
		return;
		while (la.val == listSeparator.ToString() && (functionLevel <= 0 || bracketsCounter > 0)) {
			ParsedThingList attrThing = new ParsedThingList();
			TryAddAttrSpaceBeforeBaseExpression(t.pos + t.val.Length, la.pos, attrThing, ParsedThingAttrSpaceType.SpaceBeforeBaseExpression);
			Expect(24);
			if(!wasUnion){
			if(!CanBeInReferenceExpression(expression[expression.Count - 1], true)){
				SemErr("Invalid range union");
				return;
			}
			CorrectLastThingDataType(expression, OperandDataType.Reference);
			}
			wasUnion = true;
			wasReferenceBinaryOperator = true;
			RangeIntersectionClause(OperandDataType.Reference, expression);
			if (Errors.Count > 0)
			return;
			if(!CanBeInReferenceExpression(expression[expression.Count - 1], true))
			SemErr("Invalid range union");
			expression.AddRange(attrThing);
			expression.Add(ParsedThingUnion.Instance);
		}
		if(wasReferenceBinaryOperator && bracketsCounter <= 0){
				 ParsedThingMemFunc memFuncThing = new ParsedThingMemFunc();
				 memFuncThing.InnerThingCount = expression.Count - expressionStartPosition;
				 memFuncThing.DataType = GetRefBinaryDataType(dataType);
		expression.Insert(expressionStartPosition, memFuncThing);
		wasReferenceBinaryOperator = false;
		}
	}
	void RangeIntersectionClause(OperandDataType dataType, ParsedExpression expression) {
		bool wasIntersection = false;
		CellRangeClause(dataType, expression);
		if (Errors.Count > 0)
		return;
		while (StartOf(2)) {
			ParsedThingList attrThing = new ParsedThingList();
			TryAddAttrSpaceBeforeBaseExpression(t.pos + t.val.Length, la.pos - 1, attrThing, ParsedThingAttrSpaceType.SpaceBeforeBaseExpression);
					if (la.pos - t.pos - t.val.Length == 0) {
						SemErr("Invalid range intersection");
						return;
					}
			t.pos = la.pos - 1;
			t.val = "@";
			if(!wasIntersection){
			if(!CanBeInReferenceExpression(expression[expression.Count - 1], true)){
				SemErr("Invalid range intersection");
				return;
			}
			CorrectLastThingDataType(expression, OperandDataType.Reference);
			}
			wasIntersection = true;
			wasReferenceBinaryOperator = true;
			CellRangeClause(OperandDataType.Reference, expression);
			if (Errors.Count > 0)
			return;
			if(!CanBeInReferenceExpression(expression[expression.Count - 1], true))
			SemErr("Invalid range intersection");
			expression.AddRange(attrThing);
			expression.Add(ParsedThingIntersect.Instance);
		}
	}
	void CellRangeClause(OperandDataType dataType, ParsedExpression expression) {
		CellReferenceParseHelper helper = new CellReferenceParseHelper();
		helper.PrevStartCharPosition = la.charPos;
		helper.CurStartCharPosition = helper.PrevStartCharPosition;
		int sheetDefinitionIndex;
		bool wasRange = false;
		int startExpressionCounter = expression.Count;
		ParsedThingList attrList = new ParsedThingList();
		CellReferenceWithSheet(dataType, expression, out sheetDefinitionIndex, attrList);
		if (Errors.Count > 0)
		return;
		helper.PrevSheetDefinitionIndex = sheetDefinitionIndex;
			 expression.InsertRange(startExpressionCounter, attrList);
			 startExpressionCounter += attrList.Count;
		IParsedThing prevPtg = expression[expression.Count - 1];
		if(prevPtg is ParsedThingInteger || prevPtg is ParsedThingNumeric || prevPtg is ParsedThingRef || prevPtg is ParsedThingName)
		helper.PrevThingIndex = expression.Count - 1;
		else
		helper.PrevThingIndex = -1;
		helper.PrevEndCharPosition = t.charPos + t.val.Length;
		helper.CurEndCharPosition = helper.PrevEndCharPosition;
		while (la.kind == 25) {
			if(!wasRange){
			if(!CanBeInReferenceExpression(prevPtg, false)){
				SemErr("Can not create union");
				return;
			}
			CorrectLastThingDataType(expression, OperandDataType.Reference);
			}
				 attrList.Clear();
			int currentExpressionCount = expression.Count;
			ParsedThingList beforeSignAttrs = new ParsedThingList();
			TryAddAttrSpaceBeforeBaseExpression(t.pos + t.val.Length, la.pos, beforeSignAttrs, ParsedThingAttrSpaceType.SpaceBeforeBaseExpression);
			Get();
			helper.CurStartCharPosition = la.charPos;	
			CellReferenceWithSheet(OperandDataType.Reference, expression, out sheetDefinitionIndex, attrList);
			helper.CurEndCharPosition = t.charPos + t.val.Length;
			helper.CurSheetDefinitionIndex = sheetDefinitionIndex;
			if (Errors.Count > 0){
						if(helper.PrevThingIndex >= 0){
							ParsedThingRef prevRefPtg = expression[expression.Count - 1] as ParsedThingRef;
							if (prevRefPtg != null) {
								ParsedThingRefBase refThing = CreateParsedThingRef(prevRefPtg.Position, helper, wasRange ? OperandDataType.Reference : dataType);
								expression.RemoveAt(helper.PrevThingIndex);
								expression.Insert(helper.PrevThingIndex, refThing);
							}
						}
						return;
			}
			if(!TryCreateRange(expression, helper, wasRange? OperandDataType.Reference : dataType))
			SemErr("Can not create union");
			if(expression.Count - startExpressionCounter > 1){
			wasReferenceBinaryOperator = true;
			wasRange = true;
			}else
			helper.PrevThingIndex = -1;
			helper.PrevSheetDefinitionIndex = helper.CurSheetDefinitionIndex;
			helper.PrevStartCharPosition = helper.CurStartCharPosition;
			helper.PrevEndCharPosition = helper.CurEndCharPosition;
			if(currentExpressionCount < expression.Count){
			expression.InsertRange(currentExpressionCount, attrList);
						if(helper.PrevThingIndex >= 0)
							helper.PrevThingIndex += attrList.Count;
			expression.InsertRange(expression.Count-1, beforeSignAttrs);
			}
		}
		if (Errors.Count > 0)
		   return;
		if(!wasRange){
		prevPtg = expression[expression.Count-1];
		if(prevPtg is ValueParsedThing){
		if(!CheckReferenceDataType(dataType))
		return;
		if(helper.PrevSheetDefinitionIndex >= 0){
		SemErr("Sheet definition with simple constant");
		return;
		}
		}
		}			
		if(helper.PrevThingIndex >= 0){
		prevPtg = expression[helper.PrevThingIndex];
		if(wasRange && (prevPtg is ParsedThingInteger || prevPtg is ParsedThingNumeric))
		SemErr("Invalid cell interval range definition");
		ParsedThingRef prevRefPtg = prevPtg as ParsedThingRef;
		if(prevRefPtg != null){
		CellPosition prevRefPtgPosition = prevRefPtg.Position;
		if(prevRefPtgPosition.IsColumnOrRow){
		if(!parserContext.UseR1C1ReferenceStyle)
		SemErr("Invalid cell interval range definition");
		else
		ReplaceR1C1RefToArea(expression, prevRefPtgPosition, helper, dataType);
		}
		else {
		ParsedThingRefBase refThing= CreateParsedThingRef(prevRefPtgPosition, helper, wasRange? OperandDataType.Reference : dataType);
		expression.RemoveAt(helper.PrevThingIndex);
		expression.Insert(helper.PrevThingIndex, refThing);
		}
		}
		}
	}
	void CellReferenceWithSheet(OperandDataType dataType, ParsedExpression expression, out int sheetDefinitionIndex, ParsedThingList attrList) {
		string sheetName = string.Empty;
		sheetDefinitionIndex = -1;
		int startPosition = la.charPos;
		if (IsSheetDefinition()) {
			SheetDefinitionParserContext sheetDefinitionContext = new SheetDefinitionParserContext();
			TryAddAttrSpaceBeforeBaseExpression(t.pos + t.val.Length, la.pos, attrList, ParsedThingAttrSpaceType.SpaceBeforeBaseExpression);
			if (la.kind == 6 || la.kind == 7 || la.kind == 33) {
				SheetNameQuoted(sheetDefinitionContext);
			} else if (StartOf(3)) {
				if (la.kind == 26) {
					Get();
					FileDefinitionSimple(sheetDefinitionContext);
				}
				if (la.kind == 1 || la.kind == 2 || la.kind == 3) {
					SheetNameSimple(out sheetName);
					sheetDefinitionContext.SheetNameStart = sheetName; 
					if (la.kind == 25 || la.kind == 27) {
						if (la.kind == 25) {
							Get();
							SheetNameSimple(out sheetName);
							sheetDefinitionContext.SheetNameEnd = sheetName; 
						} else {
							Get();
							string ddeTopic;	
							SingleQuotedIdent(out ddeTopic);
							sheetDefinitionContext.Clear();
							sheetDefinitionContext.ExternalName = sheetName + '|' + ddeTopic;
						}
					}
				}
			} else SynErr(41);
			Expect(28);
			sheetDefinitionIndex = sheetDefinitionContext.RegisterSheetDefinition(parserContext); 
			CellDefinition(dataType, sheetDefinitionIndex, expression, startPosition, attrList);
		} else if (StartOf(4)) {
			CellDefinitionWithBrackets(dataType, expression, startPosition, attrList);
		} else SynErr(42);
	}
	void SheetNameQuoted(SheetDefinitionParserContext sheetDefinitionContext) {
		int startPos = la.pos + 1;
		string wholeDefinition;
		if (la.kind == 7 || la.kind == 33) {
			if (la.kind == 33) {
				Get();
			} else {
				Get();
			}
		} else if (la.kind == 6) {
			Get();
			FileDefinition(sheetDefinitionContext);
		} else SynErr(43);
		int sheetNameStartPos = t.pos + 1;
		sheetDefinitionContext.SheetNameStart = scanner.Buffer.GetString(sheetNameStartPos, la.pos); 
		if (StartOf(5)) {
			Get();
			while (StartOf(5)) {
				Get();
			}
			sheetDefinitionContext.SheetNameStart = scanner.Buffer.GetString(sheetNameStartPos, la.pos); 
			if (la.kind == 25) {
				SheetEndWithFileDefinitionCommon(startPos, sheetDefinitionContext);
			} else if (la.kind == 26) {
				SheetsWithFileDefinition(startPos, sheetDefinitionContext);
			} else if (la.kind == 33) {
				Get();
			} else SynErr(44);
		} else if (la.kind == 25) {
			SheetEndWithFileDefinitionCommon(startPos, sheetDefinitionContext);
		} else if (la.kind == 26) {
			SheetsWithFileDefinition(startPos, sheetDefinitionContext);
		} else if (la.kind == 33) {
			Get();
		} else SynErr(45);
		if (Errors.Count > 0)
		return;
		wholeDefinition = scanner.Buffer.GetString(startPos, la.pos-1);	 
		if (la.kind == 27) {
			Get();
			string ddeTopic;	
			SingleQuotedIdent(out ddeTopic);
			sheetDefinitionContext.Clear();
			sheetDefinitionContext.ExternalName = wholeDefinition + '|' + ddeTopic;
		}
	}
	void FileDefinitionSimple(SheetDefinitionParserContext sheetDefinitionContext) {
		if (la.kind == 2) {
			Get();
			if(parserContext.ImportExportMode)
			sheetDefinitionContext.ExternalReferenceIndex = int.Parse(t.val);
			else
			sheetDefinitionContext.ExternalName = t.val;
		} else if (StartOf(6)) {
			string ident = string.Empty;
			DefinedIdent(out ident);
			sheetDefinitionContext.ExternalName = ident;
		} else SynErr(46);
		Expect(32);
	}
	void SheetNameSimple(out string sheetName) {
		int sheetNameStart = la.pos;
		if (la.kind == 2) {
			Get();
		} else if (la.kind == 3) {
			Get();
			if (la.pos - t.pos == t.val.Length) {
				Expect(1);
			}
		} else if (la.kind == 1) {
			Get();
		} else SynErr(47);
		sheetName = scanner.Buffer.GetString(sheetNameStart, la.pos);	
	}
	void SingleQuotedIdent(out string value) {
		value = string.Empty;	
		if (StartOf(6)) {
			DefinedIdent(out value);
		} else if (la.kind == 33) {
			Get();
			int nameStartPos = t.pos + 1;	
			while (StartOf(7)) {
				Get();
			}
			value = scanner.Buffer.GetString(nameStartPos, la.pos);
			Expect(33);
		} else SynErr(48);
	}
	void CellDefinition(OperandDataType dataType, int sheetDefinitionIndex, ParsedExpression expression, int startPosition, ParsedThingList attrList) {
		SheetDefinition sheetDefinition = null;
		if(sheetDefinitionIndex>0)
		sheetDefinition = parserContext.GetSheetDefinition(sheetDefinitionIndex);
		if (IsTableDefinition(sheetDefinition)) {
			TableReferenceExpressionClause(dataType, sheetDefinitionIndex, expression, startPosition);
		} else if (IsFunctionDefinition()) {
			FunctionClause(dataType, sheetDefinitionIndex, expression, attrList, startPosition);
		} else if (IsDefinedNameDefinition(sheetDefinition)) {
			DefinedNameExpressionClause(dataType, sheetDefinitionIndex, expression, startPosition);
		} else if (IsRCCellPosition()) {
			CellPositionRCClause(dataType, sheetDefinitionIndex, expression, startPosition);
		} else if (IsA1CellPosition()) {
			CellPositionA1Clause(dataType, sheetDefinitionIndex, expression, startPosition);
		} else if (la.kind == 35) {
			CellError(dataType, sheetDefinitionIndex, expression);
		} else if (la.kind == 2 || la.kind == 3) {
			TermNumber(expression);
		} else if (la.kind == 9 || la.kind == 10) {
			BoolConstant(expression, dataType);
		} else if (la.kind == 5) {
			Get();
			expression.Add(GetDefinedNamePtg(dataType, sheetDefinitionIndex, t.val, startPosition, t.charPos + t.val.Length - startPosition)); 
		} else SynErr(49);
	}
	void CellDefinitionWithBrackets(OperandDataType dataType, ParsedExpression expression, int startPosition, ParsedThingList attrList) {
		if (StartOf(8)) {
			TryAddAttrSpaceBeforeBaseExpression(t.pos + t.val.Length, la.pos, attrList, ParsedThingAttrSpaceType.SpaceBeforeBaseExpression);
			if (StartOf(9)) {
				CellDefinition(dataType, -1, expression, startPosition, attrList);
			} else if (la.kind == 36) {
				ArrayDefinition(dataType, expression);
			} else {
				StringConstant(expression, dataType);
			}
		} else if (la.kind == 29) {
			ParsedThingList parenAttrList = new ParsedThingList();
			TryAddAttrSpaceBeforeBaseExpression(t.pos + t.val.Length, la.pos, parenAttrList, ParsedThingAttrSpaceType.SpaceBeforeOpenParentheses);
			Get();
			bracketsCounter++; 
			CommonCellReference(dataType, expression);
			TryAddAttrSpaceBeforeBaseExpression(t.pos + t.val.Length, la.pos, parenAttrList, ParsedThingAttrSpaceType.SpaceBeforeCloseParentheses);
			expression.AddRange(parenAttrList);
			if(la.val != ")")
			parserContext.RegisterSuggestion(new CloseBracketSuggestion(la.pos));
			Expect(30);
			bracketsCounter--;
			expression.Add(ParsedThingParentheses.Instance);
		} else SynErr(50);
	}
	void ArrayDefinition(OperandDataType dataType, ParsedExpression expression) {
		if(!CheckReferenceDataType(dataType))
		return;
		string value = string.Empty;
		Expect(36);
		int nameStartPos = t.pos + 1;	
		while (StartOf(10)) {
			Get();
		}
		value = scanner.Buffer.GetString(nameStartPos, la.pos);
		Expect(37);
		if (String.IsNullOrEmpty(value))
		SemErr("Incorrect array definition.");
		else
		PrepareArrayThing(expression, value, dataType);
	}
	void StringConstant(ParsedExpression expression, OperandDataType dataType) {
		if(!CheckReferenceDataType(dataType))
		return;
		Expect(38);
		int nameStartPos = t.pos + 1;	
		while (StartOf(11)) {
			Get();
		}
		Expect(38);
		while (la.val == "\"" && la.pos - t.pos == 1) {
			Expect(38);
			while (StartOf(11)) {
				Get();
			}
			Expect(38);
		}
		if (Errors.Count > 0)
			return;
		string value = scanner.Buffer.GetString(nameStartPos, t.pos);
		value = value.Replace("\"\"", "\"");
		expression.Add(new ParsedThingStringValue(value));
	}
	void TableReferenceExpressionClause(OperandDataType dataType, int sheetDefinitionIndex, ParsedExpression expression, int startPosition) {
		string name = string.Empty;	
		string columnNameStart = string.Empty;
			 sheetDefinitionIndex = CorrectSheetDefinition(sheetDefinitionIndex, false);
		SheetDefinition sheetDefinition = parserContext.GetSheetDefinition(sheetDefinitionIndex);
		if(sheetDefinition != null && !sheetDefinition.IsExternalReference) 
		sheetDefinition = null;
		ParsedThingTable tableExpression;
		if(sheetDefinition != null){
		ParsedThingTableExt tableExt = new ParsedThingTableExt();
		tableExt.SheetDefinitionIndex = sheetDefinitionIndex;
		tableExpression = tableExt;
		}
		else
		tableExpression = new ParsedThingTable();
		Table table = null;
		tableExpression.DataType = GetCorrectedDataType(dataType, OperandDataType.Reference);
		if(tableExpression.DataType == OperandDataType.None)
		return;
		if (StartOf(6)) {
			DefinedIdent(out name);
			table = parserContext.GetTable(name);
			tableExpression.TableName = name;
			if (la.val == "[" && la.pos - t.pos == name.Length) {
				Expect(26);
				if (IsSingleColumnDefinition()) {
					TblColumnNameQuoted(out columnNameStart);
					tableExpression.SetIncludedColumns(columnNameStart, string.Empty);	
				} else if (StartOf(12)) {
					if (la.kind == 34) {
						Get();
						tableExpression.SetAllowedCol(TableRowEnum.ThisRow);	
						if (StartOf(13)) {
							TblReF_Inner(ref tableExpression);
						}
					} else {
						TblReF_Inner(ref tableExpression);
					}
					Expect(32);
				} else SynErr(51);
			}
		} else if (la.kind == 26) {
			Get();
			if (IsSingleColumnDefinition()) {
				TblColumnNameQuoted(out columnNameStart);
				tableExpression.SetIncludedColumns(columnNameStart, string.Empty);	
			} else if (StartOf(12)) {
				if (la.kind == 34) {
					Get();
					tableExpression.SetAllowedCol(TableRowEnum.ThisRow);	
					if (StartOf(13)) {
						TblReF_Inner(ref tableExpression);
					}
				} else {
					TblReF_Inner(ref tableExpression);
				}
				Expect(32);
			} else SynErr(52);
			table = parserContext.GetCurrentTable();
			if(table == null)
			SemErr("Can not find table on current cell location. Please use the full form of table reference expression.");
			else
			tableExpression.TableName = table.Name;
		} else SynErr(53);
		expression.Add(tableExpression);
		if (!CheckTableReference(tableExpression, table))
		SemErr("Invalid table reference definition");
		if (Errors.Count>0)
		return;
		parserContext.RegisterTableReference(tableExpression, startPosition, t.charPos + t.val.Length - startPosition);
	}
	void FunctionClause(OperandDataType dataType, int sheetDefinitionIndex, ParsedExpression expression, ParsedThingList attrList, int startPosition) {
		string functionName; 
		int paramCount = 0;
		OperandDataType parameterDataType;
		List<FunctionParameterParsedData> parameterPositions = new List<FunctionParameterParsedData>();
		int startParameterPosition;
		DefinedIdent(out functionName);
		Expect(29);
		functionLevel++;	
		int prevBracketsCounter = bracketsCounter;
		bracketsCounter = 0;
		bool prevWasReferenceBinaryOperator = wasReferenceBinaryOperator;
		wasReferenceBinaryOperator = false;
		bool wasArgument = false;
		ISpreadsheetFunction function = parserContext.GetFunctionByName(functionName);
		if (function is NotExistingFunction && functionName.StartsWith(WorksheetFunctionBase.FUTURE_FUNCTION_PREFIX)) {
		ISpreadsheetFunction futureFunction	= parserContext.GetFunctionByName(functionName.Remove(0, WorksheetFunctionBase.FUTURE_FUNCTION_PREFIX.Length));
		if(!(futureFunction is NotExistingFunction ))
			function = futureFunction;
		}
		FunctionParameterCollection parameters = function.Parameters;
		int parametersCount = parameters.Count;
		FunctionParameter lastParameter = null;
		if (parametersCount > 0)
		lastParameter = parameters[parametersCount - 1];
		bool arrayResultCorrectionMode = false;
		bool valueParameterToArrayMode = false;
		if(dataType != OperandDataType.None){
		valueParameterToArrayMode = (function.ReturnDataType == OperandDataType.Value) && ((dataType & OperandDataType.Array) > 0);
		dataType = GetFunctionDataType(function, dataType);
		if(dataType == OperandDataType.None)
			return;
		arrayResultCorrectionMode = dataType == OperandDataType.Array && function.ReturnDataType == OperandDataType.Value;
		}
		else 
		dataType = OperandDataType.Reference;
		if (StartOf(14)) {
			if (parametersCount <= 0){
			SemErr("Too many parameters");
			parameterDataType = OperandDataType.Value | OperandDataType.Reference | OperandDataType.Array;
			}
			else
			parameterDataType = GetFunctionParameterDataType(parameters[0].DataType, arrayResultCorrectionMode, valueParameterToArrayMode);
			startParameterPosition = la.pos;
			CommonCellReference(parameterDataType, expression);
			wasArgument = true;	
			paramCount++; 
			parameterPositions.Add(new FunctionParameterParsedData(startParameterPosition + 1, t.charPos + t.val.Length - startParameterPosition));
		}
		ParsedThingAttrIf attrIf = null;
		ParsedThingAttrChoose attrChoose = null;
		List<int> parametersEndings = new List<int>();
		if(function.Code == 0x0001){ 
		attrIf = new ParsedThingAttrIf();
		expression.Add(attrIf);
		}else  if(function.Code == 0x0064){ 
		attrChoose = new ParsedThingAttrChoose();
		expression.Add(attrChoose);
		}
		parametersEndings.Add(expression.Count);
		while (la.kind == 24) {
			if(paramCount < 1)
			paramCount = 1;
			if(!wasArgument){
			if (parametersCount <= 0){
				SemErr("Too many parameters");
				parameterDataType = OperandDataType.Value | OperandDataType.Reference | OperandDataType.Array;
			}
			else
				parameterDataType = GetFunctionParameterDataType(parameters[0].DataType, arrayResultCorrectionMode, valueParameterToArrayMode);
			CheckReferenceDataType(parameterDataType);
			if(attrIf == null && attrChoose == null)
				expression.Add(ParsedThingMissingArg.Instance);
			else
				expression.Insert(expression.Count - 1, ParsedThingMissingArg.Instance);
			parametersEndings.Clear();
			parametersEndings.Add(expression.Count);
			parameterPositions.Add(new FunctionParameterParsedData(la.pos + 1, 0));
			}
			wasArgument = false;
			bracketsCounter = 0;
			wasReferenceBinaryOperator = false;
			Get();
			OperandDataType currentParameterDataType;
			if (paramCount + 1 > parametersCount && (lastParameter == null || !lastParameter.Unlimited)){
			SemErr("Too many parameters");
			parameterDataType = OperandDataType.Value | OperandDataType.Reference | OperandDataType.Array;
			}
			else{
			if (paramCount + 1 < parametersCount)
				currentParameterDataType = parameters[paramCount].DataType;
			else
				currentParameterDataType = lastParameter.DataType;
			parameterDataType = GetFunctionParameterDataType(currentParameterDataType, arrayResultCorrectionMode, valueParameterToArrayMode);
			}
			startParameterPosition = la.pos;
			if (StartOf(14)) {
				CommonCellReference(parameterDataType, expression);
				wasArgument	= true;	
			}
			paramCount++; 
			if(!wasArgument){
			CheckReferenceDataType(parameterDataType);
			expression.Add(ParsedThingMissingArg.Instance);
			}
			parameterPositions.Add(new FunctionParameterParsedData(startParameterPosition + 1, t.charPos + t.val.Length - startParameterPosition));
			wasArgument = true;
			parametersEndings.Add(expression.Count);
		}
		int attrCount = TryAddAttrSpaceBeforeBaseExpression(t.pos + t.val.Length, la.pos, expression, ParsedThingAttrSpaceType.SpaceBeforeCloseParentheses);
		if(parametersEndings.Count > 0)
		parametersEndings[parametersEndings.Count - 1] += attrCount;
		if(la.val != ")")
		parserContext.RegisterSuggestion(new FunctionCloseBracketSuggestion(la.pos));
		Expect(30);
		functionLevel--;
		bracketsCounter = prevBracketsCounter;
		wasReferenceBinaryOperator = prevWasReferenceBinaryOperator;
		expression.AddRange(attrList);
		if(parametersEndings.Count > 0)
		parametersEndings[parametersEndings.Count - 1] += attrList.Count;
		attrList.Clear();
			 ParsedThingFunc functionPtg = GetFunctionPtg(function, paramCount, sheetDefinitionIndex, functionName, dataType);
		expression.Add(functionPtg);
		parserContext.RegisterFunctionCall(functionPtg, startPosition, t.charPos + t.val.Length - startPosition, parameterPositions);
		if (parametersCount > 0 && paramCount < parametersCount && parameters[paramCount].Required){
		SemErr("Not enough parameters.");
		return;
		}
		if(attrIf != null || attrChoose != null){
		if(attrIf != null)
			attrIf.Offset = parametersEndings[1] - parametersEndings[0] + 1; 
		else
			for(int i = 1; i < parametersEndings.Count; i++)
				attrChoose.Offsets.Add(parametersEndings[i] - parametersEndings[0] + i); 
		for(int i = parametersEndings.Count-1; i > 0; i--)
			expression.Insert(parametersEndings[i], new ParsedThingAttrGoto(expression.Count - parametersEndings[i]));
		}
		ApplyFormulaProperties(function);
	}
	void DefinedNameExpressionClause(OperandDataType dataType, int sheetDefinitionIndex, ParsedExpression expression, int startPosition) {
		string name = string.Empty;	 
		if (StartOf(6)) {
			DefinedIdent(out name);
		} else if (la.kind == 33) {
			Get();
			int nameStartPos = t.pos + 1;	
			while (StartOf(7)) {
				Get();
			}
			name = scanner.Buffer.GetString(nameStartPos, la.pos);
			Expect(33);
		} else SynErr(54);
		expression.Add(GetDefinedNamePtg(dataType, sheetDefinitionIndex, name, startPosition, t.charPos + t.val.Length - startPosition)); 
	}
	void CellPositionRCClause(OperandDataType dataType, int sheetDefinitionIndex, ParsedExpression expression, int startPosition) {
		int startPos = la.pos;
		bool wasBrackets = false;
		Expect(1);
		if (la.pos - t.pos - t.val.Length == 0 && (la.val == "[")) {
			Expect(26);
			if (la.kind == 2 || la.kind == 19) {
				if (la.kind == 19) {
					Get();
				}
				Expect(2);
			}
			Expect(32);
			wasBrackets = true;	
		}
		if (la.pos - t.pos - t.val.Length == 0 && la.val.ToLower().StartsWith("c")) {
			Expect(1);
			if (la.pos - t.pos - t.val.Length == 0 && (la.val == "[")) {
				Expect(26);
				if (la.kind == 2 || la.kind == 19) {
					if (la.kind == 19) {
						Get();
					}
					Expect(2);
				}
				Expect(32);
				wasBrackets = true;	
			}
		}
		if (Errors.Count>0)
		return;
		string reference = scanner.Buffer.GetString(startPos, la.pos); 
		reference = reference.Trim(); 
		CellPosition position = CellReferenceParser.TryParsePartRC(reference, parserContext.CurrentColumnIndex, parserContext.CurrentRowIndex, 'r', 'c');
		if (!position.IsValid && !position.IsColumnOrRow){
		if(wasBrackets)
			SemErr("Invalid cell position definition");
		else
			expression.Add(GetDefinedNamePtg(dataType, sheetDefinitionIndex, reference, startPosition, t.charPos + t.val.Length - startPosition));
		}
		else
		expression.Add(new ParsedThingRef(position));
	}
	void CellPositionA1Clause(OperandDataType dataType, int sheetDefinitionIndex, ParsedExpression expression, int startPosition) {
		int startPos = la.pos;
		if (la.kind == 31) {
			Get();
		}
		if (la.kind == 1) {
			Get();
		} else if (la.kind == 2) {
			Get();
		} else SynErr(55);
		if (la.pos - t.pos - t.val.Length == 0 && (la.kind == _positiveinumber || la.val == "$")) {
			if (la.kind == 31) {
				Get();
			}
			Expect(2);
		}
		string reference = scanner.Buffer.GetString(startPos, la.pos); 
			 reference = reference.Trim();
		CellPosition position = CellReferenceParser.TryParsePart(reference);
		if ((!position.IsValid && !position.IsColumnOrRow) || parserContext.UseR1C1ReferenceStyle || position.ToString().Length < reference.Length){
		int intValue;
		if(int.TryParse(reference, out intValue)){
			VariantValue value = intValue;
			if (value.CanBeStoredAsUInt16())
				expression.Add(new ParsedThingInteger() { Value = (int)value.NumericValue });
			else
				expression.Add(new ParsedThingNumeric() { Value = value.NumericValue });
		}
		else
			expression.Add(GetDefinedNamePtg(dataType, sheetDefinitionIndex, reference, startPosition, t.charPos + t.val.Length - startPosition));
		}
		else{
		if(position.IsColumn && position.ColumnType == PositionType.Relative)
			expression.Add(GetDefinedNamePtg(dataType, sheetDefinitionIndex, reference, startPosition, t.charPos + t.val.Length - startPosition));
		else
			expression.Add(new ParsedThingRef(position));
		}
	}
	void CellError(OperandDataType dataType, int sheetDefinitionIndex, ParsedExpression expression) {
		int startPos = la.pos; 
		Expect(35);
		if (la.kind == 1) {
			Get();
			if (la.kind == 28) {
				Get();
			} else if (la.kind == 21) {
				Get();
				if (la.kind == 1) {
					Get();
				} else if (la.kind == 2) {
					Get();
				} else SynErr(56);
				if(la.val == "!" && la.pos == t.pos + t.val.Length)
				Get();
			} else SynErr(57);
		} else if (la.kind == 5) {
			Get();
		} else SynErr(58);
		string errorText = scanner.Buffer.GetString(startPos, t.pos + t.val.Length);
		IParsedThing errorThing = CreateCellErrorInstance(errorText, dataType, sheetDefinitionIndex);
		if (errorThing == null)
		   SemErr("Incorrect error");
		else
		   expression.Add(errorThing);
	}
	void TermNumber(ParsedExpression expression) {
		if (la.kind == 2) {
			Get();
		} else if (la.kind == 3) {
			Get();
		} else SynErr(59);
		double value;
		if(!double.TryParse(t.val, NumberStyles.Float, parserContext.Culture, out value))
		SemErr("Incorrect double value");
		expression.Add(ValueParsedThing.CreateInstance(value));
	}
	void BoolConstant(ParsedExpression expression, OperandDataType dataType) {
		bool value = false;	
		if(!CheckReferenceDataType(dataType))
		return;
		if (la.kind == 9) {
			Get();
			value = true;	
		} else if (la.kind == 10) {
			Get();
		} else SynErr(60);
		expression.Add(new ParsedThingBoolean(value));
	}
	void FileDefinition(SheetDefinitionParserContext sheetDefinitionContext) {
		int startPos = la.pos;	
		while (StartOf(15)) {
			Get();
		}
		string readedValue = scanner.Buffer.GetString(startPos, la.pos);
		int index = -1;
		if(parserContext.ImportExportMode && int.TryParse(readedValue, out index))
		sheetDefinitionContext.ExternalReferenceIndex = index;
		else
		sheetDefinitionContext.ExternalName = readedValue; 
		sheetDefinitionContext.WasFileDefinition = true;
		Expect(32);
	}
	void SheetEndWithFileDefinitionCommon(int startPos, SheetDefinitionParserContext sheetDefinitionContext) {
		Expect(25);
		int sheetNameStartPos = t.pos + 1; 
		while (StartOf(16)) {
			Get();
		}
		sheetDefinitionContext.SheetNameEnd = scanner.Buffer.GetString(sheetNameStartPos, la.pos); 
		if (la.kind == 26) {
			SheetsWithFileDefinition(startPos, sheetDefinitionContext);
		} else if (la.kind == 33) {
			Get();
		} else SynErr(61);
	}
	void SheetsWithFileDefinition(int startPos, SheetDefinitionParserContext sheetDefinitionContext) {
		if(sheetDefinitionContext.WasFileDefinition)
		SemErr("File reference has already been defined");
		string filePath = scanner.Buffer.GetString(startPos, la.pos);
		Expect(26);
		FileDefinition(sheetDefinitionContext);
		sheetDefinitionContext.ExternalName = filePath + sheetDefinitionContext.ExternalName;
		int sheetNameStartPos = t.pos + 1; 
		sheetDefinitionContext.SheetNameStart = scanner.Buffer.GetString(sheetNameStartPos, la.pos); 
		if (StartOf(7)) {
			Get();
			while (StartOf(17)) {
				Get();
			}
			sheetDefinitionContext.SheetNameStart = scanner.Buffer.GetString(sheetNameStartPos, la.pos); 
			if (la.kind == 25) {
				SheetNameEnd(sheetDefinitionContext);
			} else if (la.kind == 33) {
				Get();
			} else SynErr(62);
		} else if (la.kind == 33) {
			Get();
		} else SynErr(63);
	}
	void SheetNameEnd(SheetDefinitionParserContext sheetDefinitionContext) {
		Expect(25);
		int sheetNameStartPos = t.pos + 1; 
		while (StartOf(7)) {
			Get();
		}
		sheetDefinitionContext.SheetNameEnd = scanner.Buffer.GetString(sheetNameStartPos, la.pos); 
		Expect(33);
	}
	void DefinedIdent(out string value) {
		if (la.kind == 1) {
			Get();
		} else if (la.kind == 5) {
			Get();
		} else if (la.kind == 9) {
			Get();
		} else if (la.kind == 10) {
			Get();
		} else SynErr(64);
		value = t.val;	
	}
	void TblColumnNameQuoted(out string columnName) {
		int columnNameStart = t.pos + 1;
		while (StartOf(15)) {
			Get();
		}
		columnName = scanner.Buffer.GetString(columnNameStart, la.pos);	
		Expect(32);
	}
	void TblReF_Inner(ref ParsedThingTable tableExpression) {
		TblReFInnerClause(ref tableExpression);
		while (la.kind == 24) {
			Get();
			TblReFInnerClause(ref tableExpression);
		}
	}
	void TblReFInnerClause(ref ParsedThingTable tableExpression) {
		string columnNameStart = string.Empty;
		string columnNameEnd = string.Empty;
		bool columnDefined = false;
		TableRowEnum mask = TableRowEnum.NotDefined;
		if (StartOf(18)) {
			if (la.kind == 35) {
				TableRowRangeDefinition(out mask);
			} else {
				if (tableExpression.ColumnsDefined)
				SemErr("Column range has already been defined");
				TblColumnNameSimple(out columnNameStart);
				columnDefined = true; 	
			}
		} else if (la.kind == 26) {
			Get();
			if (la.val=="#") {
				TableRowRangeDefinition(out mask);
				Expect(32);
			} else if (StartOf(19)) {
				if (tableExpression.ColumnsDefined)
				SemErr("Column range has already been defined");
				TblColumnNameQuoted(out columnNameStart);
				columnDefined = true; 	
			} else SynErr(65);
		} else SynErr(66);
		if (la.kind == 25) {
			Get();
			if (!columnDefined)
			SemErr("Incorrect column range definition"); 
			if (StartOf(20)) {
				TblColumnNameSimple(out columnNameEnd);
			} else if (la.kind == 26) {
				Get();
				TblColumnNameQuoted(out columnNameEnd);
			} else SynErr(67);
		}
		if(columnDefined)
		tableExpression.SetIncludedColumns(columnNameStart, columnNameEnd);
		else
		tableExpression.SetAllowedCol(mask);
	}
	void TableRowRangeDefinition(out TableRowEnum mask) {
		mask = TableRowEnum.NotDefined;	
		Expect(35);
		Expect(1);
		string maskText = t.val.ToLower();
		switch(maskText){
		case "all" : mask = TableRowEnum.All; break;
		case "data"	: mask = TableRowEnum.Data; break;
		case "headers" : mask = TableRowEnum.Headers; break;
		case "totals" : mask = TableRowEnum.Totals; break;
		case "this" : mask = TableRowEnum.ThisRow; break;
		default : SemErr("Incorrect table row restriction definition."); break;
		}
		if (mask == TableRowEnum.ThisRow) {
			Expect(1);
			if(StringExtensions.CompareInvariantCultureIgnoreCase(t.val, "row") != 0) 
			SemErr("Incorrect THIS ROW definition.");
		}
	}
	void TblColumnNameSimple(out string columnName) {
		int columnNameStart = la.pos;
		if (la.kind == 2) {
			Get();
			if (la.pos - t.pos == t.val.Length) {
				if (la.kind == 1) {
					Get();
				} else if (la.kind == 9) {
					Get();
				} else if (la.kind == 10) {
					Get();
				} else SynErr(68);
			}
		} else if (la.kind == 1 || la.kind == 9 || la.kind == 10) {
			if (la.kind == 1) {
				Get();
			} else if (la.kind == 9) {
				Get();
			} else {
				Get();
			}
		} else SynErr(69);
		columnName = scanner.Buffer.GetString(columnNameStart, la.pos);	
	}
	public void Parse(string reference, OperandDataType dataType, IExpressionParserContext parserContext) {
		this.parserContext = parserContext;
		bracketsCounter = 0;
		functionLevel = 0;
		wasReferenceBinaryOperator = false;
		formulaProperties = FormulaProperties.None;
		this.dataType = dataType;
		listSeparator = parserContext.ListSeparator;
		scanner.SetString(reference, parserContext.DecimalSymbol, listSeparator);
		la = new Token();
		la.val = "";		
		Get();
		ReferenceParserGrammar();
		Expect(0);
	}
	static readonly bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,T,T,T, x,T,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,T, x,T,x,T, T,x,T,x, x},
		{x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,T,T,T, x,T,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,T, x,T,x,T, T,x,T,x, x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x,T, T,T,T,T, T,x,T,T, T,T,T,T, x},
		{x,T,x,x, x,T,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, x},
		{x,T,T,T, x,T,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,x,T,x, x},
		{x,T,T,T, x,T,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, x,x,x,x, x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, x},
		{x,T,T,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,T,T, x,x,x,x, x},
		{x,T,T,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x},
		{x,T,T,T, x,T,T,T, x,T,T,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,T,x, T,T,x,T, x,T,x,T, T,x,T,x, x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,x,T,T, T,T,T,T, x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,x,T,T, T,T,T,T, x},
		{x,T,T,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x},
		{x,T,T,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x}
	};
	#region IDisposable Members
	public void Dispose() {
		scanner.Dispose();
		scanner = null;
	}
	#endregion
} 
public enum ErrorType
{
	Syntax,
	Semantic,
	Warning,
}
public class ErrorDescription
{
	#region Fields
	string msg;
	ErrorType type;
	int row;
	int col;
	#endregion
	public ErrorDescription(){
	}
	public ErrorDescription(string msg, ErrorType type, int row, int col)
	{
		this.msg = msg;
		this.type = type;
		this.row = row;
		this.col = col;
	}
	#region Properties
	public string Msg { get { return msg; } }
	public ErrorType Type { get { return type; } }
	public int Row { get { return row; } }
	public int Col { get { return col; } }
	#endregion
	public override string ToString() {
		return msg;
	}
}
public class ParserErrors
{
	#region Fields
	string errMsgFormat = "-- line {0} col {1}: {2}"; 
	List<ErrorDescription> errorCollection = new List<ErrorDescription>();
	#endregion
	#region Properties
	public int Count { get { return ErrorCollection.Count; } }
	public List<ErrorDescription> ErrorCollection { get { return errorCollection; } }
	#endregion
	public void SynErr(int line, int col, int n){
		string s;
		switch (n)
		{
			case 0: s = "EOF expected"; break;
			case 1: s = "ident expected"; break;
			case 2: s = "positiveinumber expected"; break;
			case 3: s = "fnumber expected"; break;
			case 4: s = "space expected"; break;
			case 5: s = "wideident expected"; break;
			case 6: s = "quotedOpenBracket expected"; break;
			case 7: s = "quotedSymbol expected"; break;
			case 8: s = "pathPart expected"; break;
			case 9: s = "trueConstant expected"; break;
			case 10: s = "falseConstant expected"; break;
			case 11: s = "\"<\" expected"; break;
			case 12: s = "\">\" expected"; break;
			case 13: s = "\"<=\" expected"; break;
			case 14: s = "\">=\" expected"; break;
			case 15: s = "\"<>\" expected"; break;
			case 16: s = "\"=\" expected"; break;
			case 17: s = "\"&\" expected"; break;
			case 18: s = "\"+\" expected"; break;
			case 19: s = "\"-\" expected"; break;
			case 20: s = "\"*\" expected"; break;
			case 21: s = "\"/\" expected"; break;
			case 22: s = "\"^\" expected"; break;
			case 23: s = "\"%\" expected"; break;
			case 24: s = "\",\" expected"; break;
			case 25: s = "\":\" expected"; break;
			case 26: s = "\"[\" expected"; break;
			case 27: s = "\"|\" expected"; break;
			case 28: s = "\"!\" expected"; break;
			case 29: s = "\"(\" expected"; break;
			case 30: s = "\")\" expected"; break;
			case 31: s = "\"$\" expected"; break;
			case 32: s = "\"]\" expected"; break;
			case 33: s = "\"\\\'\" expected"; break;
			case 34: s = "\"@\" expected"; break;
			case 35: s = "\"#\" expected"; break;
			case 36: s = "\"{\" expected"; break;
			case 37: s = "\"}\" expected"; break;
			case 38: s = "\"\"\" expected"; break;
			case 39: s = "??? expected"; break;
			case 40: s = "invalid UnaryClause"; break;
			case 41: s = "invalid CellReferenceWithSheet"; break;
			case 42: s = "invalid CellReferenceWithSheet"; break;
			case 43: s = "invalid SheetNameQuoted"; break;
			case 44: s = "invalid SheetNameQuoted"; break;
			case 45: s = "invalid SheetNameQuoted"; break;
			case 46: s = "invalid FileDefinitionSimple"; break;
			case 47: s = "invalid SheetNameSimple"; break;
			case 48: s = "invalid SingleQuotedIdent"; break;
			case 49: s = "invalid CellDefinition"; break;
			case 50: s = "invalid CellDefinitionWithBrackets"; break;
			case 51: s = "invalid TableReferenceExpressionClause"; break;
			case 52: s = "invalid TableReferenceExpressionClause"; break;
			case 53: s = "invalid TableReferenceExpressionClause"; break;
			case 54: s = "invalid DefinedNameExpressionClause"; break;
			case 55: s = "invalid CellPositionA1Clause"; break;
			case 56: s = "invalid CellError"; break;
			case 57: s = "invalid CellError"; break;
			case 58: s = "invalid CellError"; break;
			case 59: s = "invalid TermNumber"; break;
			case 60: s = "invalid BoolConstant"; break;
			case 61: s = "invalid SheetEndWithFileDefinitionCommon"; break;
			case 62: s = "invalid SheetsWithFileDefinition"; break;
			case 63: s = "invalid SheetsWithFileDefinition"; break;
			case 64: s = "invalid DefinedIdent"; break;
			case 65: s = "invalid TblReFInnerClause"; break;
			case 66: s = "invalid TblReFInnerClause"; break;
			case 67: s = "invalid TblReFInnerClause"; break;
			case 68: s = "invalid TblColumnNameSimple"; break;
			case 69: s = "invalid TblColumnNameSimple"; break;
			default: s = "error " + n; break;
		}
		this.ErrorCollection.Add(new ErrorDescription(string.Format(errMsgFormat, line, col, s), ErrorType.Syntax, line, col));
	}
	public void SemErr(int line, int col, string s) {
		this.ErrorCollection.Add(new ErrorDescription(string.Format(errMsgFormat, line, col, s), ErrorType.Semantic, line, col));
	}
	public void SemErr(string s) {
		this.ErrorCollection.Add(new ErrorDescription(s, ErrorType.Semantic, -1, -1));
	}
	public void Warning(int line, int col, string s) {
		this.ErrorCollection.Add(new ErrorDescription(string.Format(errMsgFormat, line, col, s), ErrorType.Warning, line, col));
	}
	public void Warning(string s) {
		this.ErrorCollection.Add(new ErrorDescription(s, ErrorType.Warning, -1, -1));
	}
	public void Clear() {
		ErrorCollection.Clear();
	}
}
public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
}
