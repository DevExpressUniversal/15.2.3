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

namespace DevExpress.Data.Filtering.Helpers {
	using System;
	using System.Globalization;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using DevExpress.Data.Filtering;
	using DevExpress.Data.Filtering.Exceptions;
	public class CriteriaParser {
		CriteriaOperator[] result;
		public CriteriaOperator[] Result { get { return result; } }
		List<OperandValue> resultParameters = new List<OperandValue>();
		public List<OperandValue> ResultParameters { get { return resultParameters; } }
  int yyMax;
  Object yyparse (yyInput yyLex) {
	if (yyMax <= 0) yyMax = 256;			
	int yyState = 0;								   
	int [] yyStates = new int[yyMax];					
	Object yyVal = null;							   
	Object [] yyVals = new Object[yyMax];			
	int yyToken = -1;					
	int yyErrorFlag = 0;				
	int yyTop = 0;
	goto skip;
	yyLoop:
	yyTop++;
	skip:
	for(;;) {  
	  if(yyTop >= yyStates.Length) {			
		int[] i = new int[yyStates.Length + yyMax];
		yyStates.CopyTo(i, 0);
		yyStates = i;
		Object[] o = new Object[yyVals.Length + yyMax];
		yyVals.CopyTo(o, 0);
		yyVals = o;
	  }
	  yyStates[yyTop] = yyState;
	  yyVals[yyTop] = yyVal;
	  yyDiscarded:	
	  for(;;) {
		int yyN;
		if ((yyN = yyDefRed[yyState]) == 0) {	
		  if(yyToken < 0)
			yyToken = yyLex.advance() ? yyLex.token() : 0;
		  if((yyN = yySindex[yyState]) != 0 && ((yyN += yyToken) >= 0)
			  && (yyN < yyTable.Length) && (yyCheck[yyN] == yyToken)) {
			yyState = yyTable[yyN];		
			yyVal = yyLex.value();
			yyToken = -1;
			if (yyErrorFlag > 0) -- yyErrorFlag;
			goto yyLoop;
		  }
		  if((yyN = yyRindex[yyState]) != 0 && (yyN += yyToken) >= 0
			  && yyN < yyTable.Length && yyCheck[yyN] == yyToken)
			yyN = yyTable[yyN];			
		  else
			switch(yyErrorFlag) {
			case 0:
			  yyerror("syntax error");
			  goto case 1;
			case 1: case 2:
			  yyErrorFlag = 3;
			  do {
				if((yyN = yySindex[yyStates[yyTop]]) != 0
					&& (yyN += Token.yyErrorCode) >= 0 && yyN < yyTable.Length
					&& yyCheck[yyN] == Token.yyErrorCode) {
				  yyState = yyTable[yyN];
				  yyVal = yyLex.value();
				  goto yyLoop;
				}
			  } while (--yyTop >= 0);
			  yyerror("irrecoverable syntax error");
			  goto yyDiscarded;
			case 3:
			  if (yyToken == 0)
				yyerror("irrecoverable syntax error at end-of-file");
			  yyToken = -1;
			  goto yyDiscarded;		
			}
		}
		int yyV = yyTop + 1 - yyLen[yyN];
		yyVal = yyV > yyTop ? null : yyVals[yyV];
		switch(yyN) {
case 1:
  { result = new CriteriaOperator[0]; }
  break;
case 2:
  { result = ((List<CriteriaOperator>)yyVals[-1+yyTop]).ToArray(); }
  break;
case 3:
  { yyVal = new List<CriteriaOperator>(new CriteriaOperator[] {(CriteriaOperator)yyVals[0+yyTop]}); }
  break;
case 4:
  { yyVal = yyVals[-2+yyTop]; ((List<CriteriaOperator>)yyVal).Add((CriteriaOperator)yyVals[0+yyTop]); }
  break;
case 5:
  { yyVal = yyVals[-2+yyTop]; ((List<CriteriaOperator>)yyVal).Add((CriteriaOperator)yyVals[0+yyTop]); }
  break;
case 6:
  { yyVal = yyVals[0+yyTop]; }
  break;
case 7:
  { yyVal = yyVals[-1+yyTop]; }
  break;
case 8:
  {
#if CF
					throw new NotSupportedException();
#else
					yyVal = new FunctionOperator(DevExpress.Data.Helpers.ServerModeCore.OrderDescToken, (CriteriaOperator)yyVals[-1+yyTop]);
#endif
					}
  break;
case 9:
  { yyVal = yyVals[0+yyTop]; }
  break;
case 10:
  {
		OperandProperty prop1 = (OperandProperty)yyVals[-2+yyTop];
		OperandProperty prop3 = (OperandProperty)yyVals[0+yyTop];
		yyVal = new OperandProperty(prop1.PropertyName + '.' + prop3.PropertyName);
	}
  break;
case 11:
  {
		OperandProperty prop1 = (OperandProperty)yyVals[-2+yyTop];
		OperandProperty prop3 = (OperandProperty)yyVals[0+yyTop];
		yyVal = new OperandProperty(prop1.PropertyName + '+' + prop3.PropertyName);
	}
  break;
case 12:
  {
		OperandProperty prop2 = (OperandProperty)yyVals[-2+yyTop];
		OperandProperty prop4 = (OperandProperty)yyVals[0+yyTop];
		yyVal = new OperandProperty('<' + prop2.PropertyName + '>' + prop4.PropertyName);
	}
  break;
case 13:
  { yyVal = yyVals[0+yyTop]; }
  break;
case 14:
  { yyVal = yyVals[0+yyTop]; }
  break;
case 15:
  { yyVal = new OperandProperty("^"); }
  break;
case 16:
  { yyVal = yyVals[0+yyTop]; }
  break;
case 17:
  {
		OperandProperty prop1 = (OperandProperty)yyVals[-2+yyTop];
		OperandProperty prop3 = (OperandProperty)yyVals[0+yyTop];
		prop1.PropertyName += '.' + prop3.PropertyName;
		yyVal = prop1;
	}
  break;
case 18:
  {
		AggregateOperand agg = (AggregateOperand)yyVals[0+yyTop];
		yyVal = JoinOperand.JoinOrAggreagate((OperandProperty)yyVals[-2+yyTop], null, agg.AggregateType, agg.AggregatedExpression);
	}
  break;
case 19:
  {
		AggregateOperand agg = (AggregateOperand)yyVals[0+yyTop];
		yyVal = JoinOperand.JoinOrAggreagate((OperandProperty)yyVals[-5+yyTop], (CriteriaOperator)yyVals[-3+yyTop], agg.AggregateType, agg.AggregatedExpression);
	}
  break;
case 20:
  {
		AggregateOperand agg = (AggregateOperand)yyVals[0+yyTop];
		yyVal = JoinOperand.JoinOrAggreagate((OperandProperty)yyVals[-4+yyTop], null, agg.AggregateType, agg.AggregatedExpression);
	}
  break;
case 21:
  { yyVal = JoinOperand.JoinOrAggreagate((OperandProperty)yyVals[-3+yyTop], (CriteriaOperator)yyVals[-1+yyTop], Aggregate.Exists, null); }
  break;
case 22:
  { yyVal = JoinOperand.JoinOrAggreagate((OperandProperty)yyVals[-2+yyTop], null, Aggregate.Exists, null); }
  break;
case 25:
  { yyVal = new AggregateOperand((OperandProperty)null, (CriteriaOperator)null, Aggregate.Count, null); }
  break;
case 26:
  { yyVal = new AggregateOperand((OperandProperty)null, (CriteriaOperator)null, Aggregate.Exists, null); }
  break;
case 27:
  { yyVal = new AggregateOperand((OperandProperty)null, (CriteriaOperator)null, Aggregate.Count, null); }
  break;
case 28:
  { yyVal = new AggregateOperand((OperandProperty)null, (CriteriaOperator)null, Aggregate.Exists, null); }
  break;
case 29:
  { yyVal = new AggregateOperand((OperandProperty)null, (CriteriaOperator)yyVals[-1+yyTop], Aggregate.Avg, null); }
  break;
case 30:
  { yyVal = new AggregateOperand((OperandProperty)null, (CriteriaOperator)yyVals[-1+yyTop], Aggregate.Sum, null); }
  break;
case 31:
  { yyVal = new AggregateOperand((OperandProperty)null, (CriteriaOperator)new OperandProperty("This"), Aggregate.Single, null); }
  break;
case 32:
  { yyVal = new AggregateOperand((OperandProperty)null, (CriteriaOperator)yyVals[-1+yyTop], Aggregate.Single, null); }
  break;
case 33:
  { yyVal = yyVals[-1+yyTop]; }
  break;
case 34:
  { yyVal = yyVals[-1+yyTop]; }
  break;
case 35:
  { yyVal = new AggregateOperand((OperandProperty)null, (CriteriaOperator)yyVals[0+yyTop], Aggregate.Min, null); }
  break;
case 36:
  { yyVal = new AggregateOperand((OperandProperty)null, (CriteriaOperator)yyVals[0+yyTop], Aggregate.Max, null); }
  break;
case 37:
  { yyVal = yyVals[0+yyTop]; }
  break;
case 38:
  { yyVal = new ConstantValue(null); }
  break;
case 39:
  {
						  string paramName = (string)yyVals[0+yyTop];
						  if(string.IsNullOrEmpty(paramName)) {
							OperandValue param = new OperandValue();
							resultParameters.Add(param);
							yyVal = param;
						  } else {
							bool paramNotFound = true;
							foreach(OperandValue v in resultParameters) {
							  OperandParameter p = v as OperandParameter;
							  if(ReferenceEquals(p, null))
								continue;
							  if(p.ParameterName != paramName)
								continue;
							  paramNotFound = false;
							  resultParameters.Add(p);
							  yyVal = p;
							  break;
							}
							if(paramNotFound) {
							  OperandParameter param = new OperandParameter(paramName);
							  resultParameters.Add(param);
							  yyVal = param;
							}
						  }
						}
  break;
case 40:
  { yyVal = yyVals[0+yyTop]; }
  break;
case 41:
  { yyVal = yyVals[0+yyTop]; }
  break;
case 42:
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.Multiply ); }
  break;
case 43:
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.Divide ); }
  break;
case 44:
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.Plus ); }
  break;
case 45:
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.Minus ); }
  break;
case 46:
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.Modulo ); }
  break;
case 47:
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.BitwiseOr ); }
  break;
case 48:
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.BitwiseAnd ); }
  break;
case 49:
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.BitwiseXor ); }
  break;
case 50:
  {
								yyVal = new UnaryOperator( UnaryOperatorType.Minus, (CriteriaOperator)yyVals[0+yyTop] );
								try {
									if(yyVals[0+yyTop] is OperandValue) {
										OperandValue operand = (OperandValue)yyVals[0+yyTop];
										if(operand.Value is Int32) {
											operand.Value = -(Int32)operand.Value;
											yyVal = operand;
											break;
										} else if(operand.Value is Int64) {
											operand.Value = -(Int64)operand.Value;
											yyVal = operand;
											break;
										} else if(operand.Value is Double) {
											operand.Value = -(Double)operand.Value;
											yyVal = operand;
											break;
										} else if(operand.Value is Single) {
											operand.Value = -(Single)operand.Value;
											yyVal = operand;
											break;
										} else if(operand.Value is Decimal) {
											operand.Value = -(Decimal)operand.Value;
											yyVal = operand;
											break;
										}  else if(operand.Value is Int16) {
											operand.Value = -(Int16)operand.Value;
											yyVal = operand;
											break;
										}  else if(operand.Value is SByte) {
											operand.Value = -(SByte)operand.Value;
											yyVal = operand;
											break;
										}
									}
								} catch {}
							}
  break;
case 51:
  { yyVal = new UnaryOperator( UnaryOperatorType.Plus, (CriteriaOperator)yyVals[0+yyTop] ); }
  break;
case 52:
  { yyVal = new UnaryOperator( UnaryOperatorType.BitwiseNot, (CriteriaOperator)yyVals[0+yyTop] ); }
  break;
case 53:
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.Equal); }
  break;
case 54:
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.NotEqual); }
  break;
case 55:
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.Greater); }
  break;
case 56:
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.Less); }
  break;
case 57:
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.GreaterOrEqual); }
  break;
case 58:
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.LessOrEqual); }
  break;
case 59:
  { yyVal = LikeCustomFunction.Create( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop]); }
  break;
case 60:
  { yyVal = !LikeCustomFunction.Create( (CriteriaOperator)yyVals[-3+yyTop], (CriteriaOperator)yyVals[0+yyTop]); }
  break;
case 61:
  { yyVal = new UnaryOperator(UnaryOperatorType.Not, (CriteriaOperator)yyVals[0+yyTop]); }
  break;
case 62:
  { yyVal = GroupOperator.And((CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop]); }
  break;
case 63:
  { yyVal = GroupOperator.Or((CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop]); }
  break;
case 64:
  { yyVal = yyVals[-1+yyTop]; }
  break;
case 65:
  { yyVal = new UnaryOperator(UnaryOperatorType.IsNull, (CriteriaOperator)yyVals[-2+yyTop]); }
  break;
case 66:
  { yyVal = new UnaryOperator(UnaryOperatorType.Not, new UnaryOperator(UnaryOperatorType.IsNull, (CriteriaOperator)yyVals[-3+yyTop])); }
  break;
case 67:
  { yyVal = new InOperator((CriteriaOperator)yyVals[-2+yyTop], (IEnumerable<CriteriaOperator>)yyVals[0+yyTop]); }
  break;
case 68:
  { yyVal = new BetweenOperator((CriteriaOperator)yyVals[-6+yyTop], (CriteriaOperator)yyVals[-3+yyTop], (CriteriaOperator)yyVals[-1+yyTop]); }
  break;
case 69:
  { yyVal = new UnaryOperator(UnaryOperatorType.IsNull, (CriteriaOperator)yyVals[-1+yyTop]); }
  break;
case 70:
  { yyVal = new FunctionOperator(FunctionOperatorType.IsNull, (CriteriaOperator)yyVals[-3+yyTop], (CriteriaOperator)yyVals[-1+yyTop]); }
  break;
case 71:
  {  FunctionOperator fo = new FunctionOperator((FunctionOperatorType)yyVals[-1+yyTop], (IEnumerable<CriteriaOperator>)yyVals[0+yyTop]); lexer.CheckFunctionArgumentsCount(fo); yyVal = fo; }
  break;
case 72:
  {  FunctionOperator fo = new FunctionOperator(((OperandProperty)yyVals[-1+yyTop]).PropertyName, (IEnumerable<CriteriaOperator>)yyVals[0+yyTop]); lexer.CheckFunctionArgumentsCount(fo); yyVal = fo; }
  break;
case 73:
  { yyVal = null; }
  break;
case 74:
  { yyVal = new FunctionOperator(FunctionOperatorType.Min, ((AggregateOperand)yyVals[-3+yyTop]).AggregatedExpression, (CriteriaOperator)yyVals[-1+yyTop]); }
  break;
case 75:
  { yyVal = new FunctionOperator(FunctionOperatorType.Max, ((AggregateOperand)yyVals[-3+yyTop]).AggregatedExpression, (CriteriaOperator)yyVals[-1+yyTop]); }
  break;
case 76:
  { yyVal = yyVals[-1+yyTop]; }
  break;
case 77:
  { yyVal = new List<CriteriaOperator>(); }
  break;
case 78:
  {
							List<CriteriaOperator> lst = new List<CriteriaOperator>();
							lst.Add((CriteriaOperator)yyVals[0+yyTop]);
							yyVal = lst;
						}
  break;
case 79:
  {
							List<CriteriaOperator> lst = (List<CriteriaOperator>)yyVals[-2+yyTop];
							lst.Add((CriteriaOperator)yyVals[0+yyTop]);
							yyVal = lst;
						}
  break;
		}
		yyTop -= yyLen[yyN];
		yyState = yyStates[yyTop];
		int yyM = yyLhs[yyN];
		if(yyState == 0 && yyM == 0) {
		  yyState = yyFinal;
		  if(yyToken < 0)
			yyToken = yyLex.advance() ? yyLex.token() : 0;
		  if(yyToken == 0)
			return yyVal;
		  goto yyLoop;
		}
		if(((yyN = yyGindex[yyM]) != 0) && ((yyN += yyState) >= 0)
			&& (yyN < yyTable.Length) && (yyCheck[yyN] == yyState))
		  yyState = yyTable[yyN];
		else
		  yyState = yyDgoto[yyM];
	 goto yyLoop;
	  }
	}
  }
   static  short [] yyLhs  = {			  -1,
	0,	0,	1,	1,	1,	2,	2,	2,	4,	4,
	4,	5,	6,	6,	6,	7,	7,	8,	8,	8,
	8,	8,	8,   10,	9,	9,	9,	9,	9,	9,
	9,	9,	9,	9,   11,   12,	3,	3,	3,	3,
	3,	3,	3,	3,	3,	3,	3,	3,	3,	3,
	3,	3,	3,	3,	3,	3,	3,	3,	3,	3,
	3,	3,	3,	3,	3,	3,	3,	3,	3,	3,
	3,	3,	3,	3,	3,   13,   13,   14,   14,
  };
   static  short [] yyLen = {		   2,
	1,	2,	1,	3,	3,	1,	2,	2,	1,	3,
	3,	4,	1,	1,	1,	1,	3,	3,	6,	5,
	4,	3,	1,	1,	1,	1,	3,	3,	4,	4,
	3,	4,	2,	2,	3,	3,	1,	1,	1,	1,
	1,	3,	3,	3,	3,	3,	3,	3,	3,	2,
	2,	2,	3,	3,	3,	3,	3,	3,	3,	4,
	2,	3,	3,	3,	3,	4,	3,	7,	4,	6,
	2,	2,	2,	4,	4,	3,	2,	1,	3,
  };
   static  short [] yyDefRed = {			0,
   37,	0,	0,	0,	0,	0,	0,	0,   39,	0,
	0,	0,	0,	0,   38,	0,   15,	0,	0,	0,
	1,	0,	0,	3,	0,   14,   16,	0,   41,   24,
   23,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,   72,	0,   71,   73,	0,	0,	9,	0,	0,
   50,   51,	2,	0,	0,	7,	8,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
   33,	0,   34,	0,   28,   27,	0,	0,	0,	0,
   31,	0,   77,	0,	0,	0,   64,	0,	0,	0,
	4,	5,	0,	0,	0,	0,   65,	0,	0,	0,
	0,	0,	0,	0,   67,	0,	0,	0,	0,	0,
	0,   42,   43,   46,   13,   17,   18,	0,	0,	0,
	0,	0,	0,   29,   30,   32,   76,	0,   69,	0,
   10,   12,   11,	0,   66,	0,	0,	0,   74,   75,
	0,	0,	0,   20,	0,   70,	0,   19,   68,
  };
  protected static  short [] yyDgoto  = {			22,
   23,   24,   25,   49,   26,   27,   28,   29,   30,   31,
   32,   33,   42,   95,
  };
  protected static  int yyFinal = 22;
  protected static  short [] yySindex = {		  799,
	0,  -34,  -18,  -14,  -12,   -9,   -5,   -3,	0,   15,
   16,   15, 1303, 1557,	0, -239,	0, 1557, 1557, 1557,
	0,	0,	9,	0,  635,	0,	0,  -41,	0,	0,
	0,  -33,   -8,	8,   25, 1557, 1557, 1557, 1557, 1328,
 1370,	0, 1557,	0,	0,  840, 1260,	0,  -43,   79,
	0,	0,	0, 1557, 1557,	0,	0, 1557, 1557, -237,
 -258, 1557, 1557, 1557, 1557, 1557, 1557, 1557,   15,   51,
 1557, 1557, 1557, 1557, 1557, 1557, 1557, 1557,  257, 1513,
	0, 1557,	0, 1557,	0,	0, 1235, 1235,  854,  885,
	0,  899,	0, 1235,   30,  826,	0, -157, -155, -154,
	0,	0, 1248, 1260, 1557, -160,	0,  221,  221,  221,
 1271, 1271, 1271, 1271,	0, 1557,  -24,  -13,   79,  -35,
  -35,	0,	0,	0,	0,	0,	0,   82,   90,   91,
  914,  928,  945,	0,	0,	0,	0, 1557,	0, 1557,
	0,	0,	0,  221,	0,  959,  -47,   92,	0,	0,
 1235, 1207, 1557,	0,  -47,	0, 1221,	0,	0,
  };
  protected static  short [] yyRindex = {			0,
	0,   20,   40,	0,	0,	0,	0,	0,	0,	1,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,   10,	0,	0,   59,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,  194,	0,	0,  437,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,   45,   49,	0,	0,
	0,	0,	0,   64,	0,	0,	0,	0,	0,	0,
	0,	0,  202,  566,	0,	0,	0,   88,  117,  149,
  510,  524,  538,  552,	0,	0,  491,  470,  454,  162,
  415,	0,	0,	0,	0,	0,	0,	0,	0,   98,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,  577,	0,	0,	0,  130,	0,	0,
   66,	0,	0,	0,	0,	0,	0,	0,	0,
  };
  protected static  short [] yyGindex = {			0,
	0,   21, 1649,	0,	0,   67,	0,	0,  -75,	0,
  -28,  -27,	4,	0,
  };
  protected static  short [] yyTable = {		   100,
   13,   78,   98,  127,   79,   34,   76,   81,   53,	6,
   82,   77,   78,   73,  106,   44,  107,   76,   75,   26,
   74,   35,   77,   78,   73,   36,   48,   37,   76,   75,
   38,   74,   83,   77,   39,   84,   40,   13,   13,   25,
  105,   13,   13,   13,   13,   13,   13,   13,   85,   80,
  128,  129,   55,	6,   41,   43,   26,   26,   40,   13,
   26,   26,   26,   26,   26,   86,   26,   54,	6,   72,
  137,  154,  115,  138,  101,  102,   25,   25,   26,  158,
   25,   25,   25,   25,   25,   35,   25,   53,   35,   36,
  116,   13,   36,   13,   13,   40,   40,   22,   25,   40,
   40,   40,   40,   40,   78,   40,   79,   78,  141,   79,
  142,  143,   26,   26,  145,   78,   54,   40,  128,  129,
   76,   75,   81,   74,   13,   77,  128,  129,   53,   21,
   83,   53,   25,   25,   22,   22,  147,  155,   22,   22,
   22,   22,   22,   26,   22,  126,   53,	0,   59,	0,
	0,   40,   40,	0,	0,	0,   22,   54,	0,	0,
   54,   45,	0,   25,	0,	0,   21,   21,	0,	0,
   21,   21,   21,   21,   21,   54,   21,	0,	0,	0,
   53,	0,   40,	0,	0,	0,	0,	0,   21,   59,
   22,   22,   59,   61,	0,	0,	0,	0,	0,   45,
	0,   63,   45,	0,   45,   45,   45,   59,	0,   54,
	2,	3,	4,	5,	6,	7,	8,	0,	0,	0,
   45,   22,   21,   21,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,   61,   99,	0,   61,	0,	0,
	0,   59,   63,	0,	0,   63,	0,	0,	0,	0,
	0,	0,   61,   21,   45,   45,	0,   78,   73,	0,
   63,	0,   76,   75,	0,   74,	0,   77,	0,   13,
   13,   13,   13,   13,   13,	0,   13,   13,   13,   13,
   13,   13,   13,   13,   13,   45,   61,	0,   26,   26,
   26,   26,   26,   26,   63,   26,   26,   26,   26,   26,
   26,   26,   26,   26,	0,	0,	0,	0,   25,   25,
   25,   25,   25,   25,   72,   25,   25,   25,   25,   25,
   25,   25,   25,   25,	0,	0,	0,   40,   40,   40,
   40,   40,   40,	0,   40,   40,   40,   40,   40,   40,
   40,   40,   40,	0,   71,	0,	0,	0,	0,	0,
   17,	0,	0,	0,	0,	0,   53,   53,   53,   53,
   53,   53,	0,   53,   53,   53,   22,   22,   22,   22,
   22,   22,	0,   22,   22,   22,   22,   22,   22,   22,
   22,   22,	0,	0,	0,   54,   54,   54,   54,   54,
   54,	0,   54,   54,   54,	0,	0,	0,   21,   21,
   21,   21,   21,   21,	0,   21,   21,   21,   21,   21,
   21,   21,   21,   21,   44,	0,	0,   59,   59,   59,
   59,   59,   59,	0,   59,   59,   59,	0,	0,	0,
   45,   45,   45,   45,   45,   45,   52,   45,   45,   45,
   45,   45,   45,   45,   45,   45,	0,	0,	0,	0,
	0,	0,   44,   48,	0,   44,	0,   44,   44,   44,
	0,	0,   61,   61,   61,   61,	0,	0,	0,   49,
   63,   63,   63,   44,   52,	0,	0,   52,	0,	0,
   52,	0,	0,	0,	0,	0,	0,	0,	0,	0,
   47,   48,	0,	0,   48,   52,	0,   48,	0,   65,
   66,   67,   68,   69,   70,	0,	0,   44,   44,   55,
   49,	0,   48,   49,	2,	3,	4,	5,	6,	7,
	8,	0,  125,   56,	0,	0,	0,	0,   49,   52,
   52,   47,	0,	0,   47,	0,   16,   57,   44,	0,
	0,	0,	0,	0,	0,	0,   48,   48,	0,   47,
   55,   58,	0,   55,	0,	0,	0,	0,	0,	0,
   52,	0,   49,   49,   56,   62,	0,   56,   55,	0,
	0,	0,	0,	0,	0,	0,   60,   48,   57,	0,
	0,   57,   56,   47,	0,	0,	0,	0,	0,	0,
	0,	0,   58,   49,	0,   58,   57,	0,	0,	0,
	0,	0,   55,	0,	0,	0,   62,	0,	0,   62,
   58,	0,	0,	0,   47,	0,   56,   60,	0,	0,
   60,	0,	0,	0,   62,	0,	0,	0,	0,	0,
   57,	0,	0,	0,	0,   60,	0,	0,	0,	0,
	0,	0,	0,	0,   58,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,   62,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,   60,
	0,   78,   73,	0,	0,	0,   76,   75,	0,   74,
	0,   77,	0,   44,   44,   44,   44,   44,   44,	0,
   44,   44,   44,   44,   44,   44,   44,   44,   44,	0,
	0,	0,	0,	0,	0,   52,   52,   52,   52,   52,
   52,	0,   52,   52,   52,   52,   52,   52,   52,   52,
   52,	0,   48,   48,   48,   48,   48,   48,   72,   48,
   48,   48,   48,   48,   48,   48,   48,   48,   49,   49,
   49,   49,   49,   49,	0,   49,   49,   49,   49,   49,
   49,   49,   49,   49,	0,	0,	0,	0,   71,   47,
   47,   47,   47,   47,   47,	0,   47,   47,   47,   47,
   47,   47,   47,   47,   47,	0,	0,	0,   55,   55,
   55,   55,   55,   55,	0,   55,   55,   55,   55,   55,
   55,   55,   56,   56,   56,   56,   56,   56,   21,   56,
   56,   56,   56,   56,   56,   56,   57,   57,   57,   57,
   57,   57,	0,   57,   57,   57,   57,   57,   57,   57,
   58,   58,   58,   58,   58,   58,	0,   58,   58,   58,
   58,   58,   58,   58,   62,   62,   62,   62,   13,	0,
	0,   20,	0,   19,	0,   60,   60,   60,   60,   60,
   60,	0,   60,   60,   60,	0,	0,	0,	0,	0,
	0,	0,   78,   73,	0,	0,  139,   76,   75,  140,
   74,	0,   77,	0,	0,	0,   78,   73,	0,	0,
   97,   76,   75,	0,   74,	0,   77,	0,	0,	0,
   78,   73,   17,	0,  134,   76,   75,	0,   74,	0,
   77,	0,	0,   56,   57,   58,   59,   60,   61,	0,
   62,   63,   64,   65,   66,   67,   68,   69,   70,   72,
	0,   78,   73,	0,   18,  135,   76,   75,	0,   74,
	0,   77,	0,   72,	0,   78,   73,	0,	0,  136,
   76,   75,	0,   74,	0,   77,	0,   72,	0,   71,
   78,   73,	0,	0,	0,   76,   75,	0,   74,	0,
   77,	0,	0,   71,   78,   73,	0,	0,  149,   76,
   75,	0,   74,	0,   77,	0,	0,   71,   72,	0,
	0,   78,   73,	0,	0,  150,   76,   75,	0,   74,
	0,   77,   72,	0,	0,   78,   73,	0,	0,	0,
   76,   75,  153,   74,	0,   77,  148,   72,   71,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,   72,   71,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,   71,   72,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,   71,   72,	0,	0,	1,	2,	3,	4,	5,
	6,	7,	8,	9,   10,   11,   12,	0,   71,	0,
	0,   14,	0,   15,	0,	0,	0,	0,   16,	0,
	0,	0,   71,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,   58,   59,   60,   61,
	0,   62,   63,   64,   65,   66,   67,   68,   69,   70,
   58,   59,   60,   61,	0,   62,   63,   64,   65,   66,
   67,   68,   69,   70,   58,   59,   60,   61,	0,   62,
   63,   64,   65,   66,   67,   68,   69,   70,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,   58,   59,   60,   61,	0,
   62,   63,   64,   65,   66,   67,   68,   69,   70,   58,
   59,   60,   61,	0,   62,   63,   64,   65,   66,   67,
   68,   69,   70,	0,   58,   59,   60,   61,	0,   62,
   63,   64,   65,   66,   67,   68,   69,   70,   58,   59,
   60,   61,	0,   62,   63,   64,   65,   66,   67,   68,
   69,   70,	0,	0,	0,   58,   59,   60,   61,	0,
   62,   63,   64,   65,   66,   67,   68,   69,   70,   58,
   59,   60,   61,	0,   62,   63,   64,   65,   66,   67,
   68,   69,   70,   78,   73,	0,	0,  156,   76,   75,
	0,   74,	0,   77,	0,	0,	0,   78,   73,	0,
	0,  159,   76,   75,	0,   74,	0,   77,	0,	0,
	0,   78,   73,	0,	0,	0,   76,   75,	0,   74,
	0,   77,	0,	0,   78,   73,	0,	0,	0,   76,
   75,	0,   74,	0,   77,	0,   78,   73,	0,	0,
   72,   76,   75,	0,   74,	0,   77,   78,   73,	0,
	0,	0,   76,   75,   72,   74,	0,   77,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,   72,	0,
   71,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,   72,   13,   45,   71,   20,	0,   19,	0,	0,
	0,	0,	0,   72,	0,	0,	0,	0,   71,	0,
	0,	0,	0,	0,   72,	0,	0,   13,   91,	0,
   20,   71,   19,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,   71,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,   71,	0,   17,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,   13,
   93,	0,   20,	0,   19,	0,	0,	0,	0,	0,
	0,   17,	0,	0,	0,	0,	0,	0,   18,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,   18,	0,	0,	0,	0,	0,	0,
	0,	0,	0,   17,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,   58,   59,   60,
   61,	0,   62,   63,   64,   65,   66,   67,   68,   69,
   70,   58,   59,   60,   61,   18,   62,   63,   64,   65,
   66,   67,   68,   69,   70,   58,   59,   60,   61,	0,
   62,   63,   64,   65,   66,   67,   68,   69,   70,   59,
   60,   61,	0,   62,   63,   64,   65,   66,   67,   68,
   69,   70,   60,   61,	0,   62,   63,   64,   65,   66,
   67,   68,   69,   70,	0,	0,	0,	0,	0,	0,
	0,	0,   13,   69,   70,   20,	0,   19,	0,	1,
	2,	3,	4,	5,	6,	7,	8,	9,   10,   11,
   12,	0,	0,	0,	0,   14,	0,   15,	0,	0,
	0,	0,   16,	0,	1,	2,	3,	4,	5,	6,
	7,	8,	9,   10,   11,   12,   13,	0,	0,   20,
   14,   19,   15,	0,	0,  130,   17,   16,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	1,	2,	3,	4,
	5,	6,	7,	8,	9,   10,   11,   12,   18,	0,
	0,	0,   14,	0,   15,	0,	0,	0,	0,   16,
   17,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,   46,   47,	0,	0,	0,   50,   51,   52,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,   18,	0,   87,   88,   89,   90,   92,   94,
	0,   96,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,  103,  104,	0,	0,
  108,  109,  110,  111,  112,  113,  114,	0,	0,  117,
  118,  119,  120,  121,  122,  123,  124,	0,  131,	0,
  132,	0,  133,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,  144,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,  146,	0,	0,	0,	0,	1,
	2,	3,	4,	5,	6,	7,	8,	9,   10,   11,
   12,	0,	0,	0,	0,   14,  151,   15,  152,	0,
	0,	0,   16,	0,	0,	0,	0,	0,	0,	0,
	0,  157,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	1,	2,	3,	4,	5,	6,	7,
	8,	9,   10,   11,   12,	0,	0,	0,	0,   14,
	0,   15,	0,	0,	0,	0,   16,
  };
  protected static  short [] yyCheck = {			43,
	0,   37,   46,   79,   46,   40,   42,   41,	0,	0,
   44,   47,   37,   38,  273,   12,  275,   42,   43,	0,
   45,   40,   47,   37,   38,   40,  266,   40,   42,   43,
   40,   45,   41,   47,   40,   44,   40,   37,   38,	0,
  278,   41,   42,   43,   44,   45,   46,   47,   41,   91,
   79,   79,   44,   44,   40,   40,   37,   38,	0,   59,
   41,   42,   43,   44,   45,   41,   47,   59,   59,   94,
   41,  147,   69,   44,   54,   55,   37,   38,   59,  155,
   41,   42,   43,   44,   45,   41,   47,	0,   44,   41,
   40,   91,   44,   93,   94,   37,   38,	0,   59,   41,
   42,   43,   44,   45,   41,   47,   41,   44,  266,   44,
  266,  266,   93,   94,  275,   37,	0,   59,  147,  147,
   42,   43,   41,   45,  124,   47,  155,  155,   41,	0,
   41,   44,   93,   94,   37,   38,   46,   46,   41,   42,
   43,   44,   45,  124,   47,   79,   59,   -1,	0,   -1,
   -1,   93,   94,   -1,   -1,   -1,   59,   41,   -1,   -1,
   44,	0,   -1,  124,   -1,   -1,   37,   38,   -1,   -1,
   41,   42,   43,   44,   45,   59,   47,   -1,   -1,   -1,
   93,   -1,  124,   -1,   -1,   -1,   -1,   -1,   59,   41,
   93,   94,   44,	0,   -1,   -1,   -1,   -1,   -1,   38,
   -1,	0,   41,   -1,   43,   44,   45,   59,   -1,   93,
  258,  259,  260,  261,  262,  263,  264,   -1,   -1,   -1,
   59,  124,   93,   94,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   41,  279,   -1,   44,   -1,   -1,
   -1,   93,   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,
   -1,   -1,   59,  124,   93,   94,   -1,   37,   38,   -1,
   59,   -1,   42,   43,   -1,   45,   -1,   47,   -1,  269,
  270,  271,  272,  273,  274,   -1,  276,  277,  278,  279,
  280,  281,  282,  283,  284,  124,   93,   -1,  269,  270,
  271,  272,  273,  274,   93,  276,  277,  278,  279,  280,
  281,  282,  283,  284,   -1,   -1,   -1,   -1,  269,  270,
  271,  272,  273,  274,   94,  276,  277,  278,  279,  280,
  281,  282,  283,  284,   -1,   -1,   -1,  269,  270,  271,
  272,  273,  274,   -1,  276,  277,  278,  279,  280,  281,
  282,  283,  284,   -1,  124,   -1,   -1,   -1,   -1,   -1,
   94,   -1,   -1,   -1,   -1,   -1,  269,  270,  271,  272,
  273,  274,   -1,  276,  277,  278,  269,  270,  271,  272,
  273,  274,   -1,  276,  277,  278,  279,  280,  281,  282,
  283,  284,   -1,   -1,   -1,  269,  270,  271,  272,  273,
  274,   -1,  276,  277,  278,   -1,   -1,   -1,  269,  270,
  271,  272,  273,  274,   -1,  276,  277,  278,  279,  280,
  281,  282,  283,  284,	0,   -1,   -1,  269,  270,  271,
  272,  273,  274,   -1,  276,  277,  278,   -1,   -1,   -1,
  269,  270,  271,  272,  273,  274,	0,  276,  277,  278,
  279,  280,  281,  282,  283,  284,   -1,   -1,   -1,   -1,
   -1,   -1,   38,	0,   -1,   41,   -1,   43,   44,   45,
   -1,   -1,  269,  270,  271,  272,   -1,   -1,   -1,	0,
  269,  270,  271,   59,   38,   -1,   -1,   41,   -1,   -1,
   44,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
	0,   38,   -1,   -1,   41,   59,   -1,   44,   -1,  279,
  280,  281,  282,  283,  284,   -1,   -1,   93,   94,	0,
   41,   -1,   59,   44,  258,  259,  260,  261,  262,  263,
  264,   -1,  266,	0,   -1,   -1,   -1,   -1,   59,   93,
   94,   41,   -1,   -1,   44,   -1,  280,	0,  124,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   93,   94,   -1,   59,
   41,	0,   -1,   44,   -1,   -1,   -1,   -1,   -1,   -1,
  124,   -1,   93,   94,   41,	0,   -1,   44,   59,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,	0,  124,   41,   -1,
   -1,   44,   59,   93,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   41,  124,   -1,   44,   59,   -1,   -1,   -1,
   -1,   -1,   93,   -1,   -1,   -1,   41,   -1,   -1,   44,
   59,   -1,   -1,   -1,  124,   -1,   93,   41,   -1,   -1,
   44,   -1,   -1,   -1,   59,   -1,   -1,   -1,   -1,   -1,
   93,   -1,   -1,   -1,   -1,   59,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,
   -1,   37,   38,   -1,   -1,   -1,   42,   43,   -1,   45,
   -1,   47,   -1,  269,  270,  271,  272,  273,  274,   -1,
  276,  277,  278,  279,  280,  281,  282,  283,  284,   -1,
   -1,   -1,   -1,   -1,   -1,  269,  270,  271,  272,  273,
  274,   -1,  276,  277,  278,  279,  280,  281,  282,  283,
  284,   -1,  269,  270,  271,  272,  273,  274,   94,  276,
  277,  278,  279,  280,  281,  282,  283,  284,  269,  270,
  271,  272,  273,  274,   -1,  276,  277,  278,  279,  280,
  281,  282,  283,  284,   -1,   -1,   -1,   -1,  124,  269,
  270,  271,  272,  273,  274,   -1,  276,  277,  278,  279,
  280,  281,  282,  283,  284,   -1,   -1,   -1,  269,  270,
  271,  272,  273,  274,   -1,  276,  277,  278,  279,  280,
  281,  282,  269,  270,  271,  272,  273,  274,	0,  276,
  277,  278,  279,  280,  281,  282,  269,  270,  271,  272,
  273,  274,   -1,  276,  277,  278,  279,  280,  281,  282,
  269,  270,  271,  272,  273,  274,   -1,  276,  277,  278,
  279,  280,  281,  282,  269,  270,  271,  272,   40,   -1,
   -1,   43,   -1,   45,   -1,  269,  270,  271,  272,  273,
  274,   -1,  276,  277,  278,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   37,   38,   -1,   -1,   41,   42,   43,   44,
   45,   -1,   47,   -1,   -1,   -1,   37,   38,   -1,   -1,
   41,   42,   43,   -1,   45,   -1,   47,   -1,   -1,   -1,
   37,   38,   94,   -1,   41,   42,   43,   -1,   45,   -1,
   47,   -1,   -1,  269,  270,  271,  272,  273,  274,   -1,
  276,  277,  278,  279,  280,  281,  282,  283,  284,   94,
   -1,   37,   38,   -1,  126,   41,   42,   43,   -1,   45,
   -1,   47,   -1,   94,   -1,   37,   38,   -1,   -1,   41,
   42,   43,   -1,   45,   -1,   47,   -1,   94,   -1,  124,
   37,   38,   -1,   -1,   -1,   42,   43,   -1,   45,   -1,
   47,   -1,   -1,  124,   37,   38,   -1,   -1,   41,   42,
   43,   -1,   45,   -1,   47,   -1,   -1,  124,   94,   -1,
   -1,   37,   38,   -1,   -1,   41,   42,   43,   -1,   45,
   -1,   47,   94,   -1,   -1,   37,   38,   -1,   -1,   -1,
   42,   43,   44,   45,   -1,   47,   93,   94,  124,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   94,  124,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,   94,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  124,   94,   -1,   -1,  257,  258,  259,  260,  261,
  262,  263,  264,  265,  266,  267,  268,   -1,  124,   -1,
   -1,  273,   -1,  275,   -1,   -1,   -1,   -1,  280,   -1,
   -1,   -1,  124,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  271,  272,  273,  274,
   -1,  276,  277,  278,  279,  280,  281,  282,  283,  284,
  271,  272,  273,  274,   -1,  276,  277,  278,  279,  280,
  281,  282,  283,  284,  271,  272,  273,  274,   -1,  276,
  277,  278,  279,  280,  281,  282,  283,  284,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  271,  272,  273,  274,   -1,
  276,  277,  278,  279,  280,  281,  282,  283,  284,  271,
  272,  273,  274,   -1,  276,  277,  278,  279,  280,  281,
  282,  283,  284,   -1,  271,  272,  273,  274,   -1,  276,
  277,  278,  279,  280,  281,  282,  283,  284,  271,  272,
  273,  274,   -1,  276,  277,  278,  279,  280,  281,  282,
  283,  284,   -1,   -1,   -1,  271,  272,  273,  274,   -1,
  276,  277,  278,  279,  280,  281,  282,  283,  284,  271,
  272,  273,  274,   -1,  276,  277,  278,  279,  280,  281,
  282,  283,  284,   37,   38,   -1,   -1,   41,   42,   43,
   -1,   45,   -1,   47,   -1,   -1,   -1,   37,   38,   -1,
   -1,   41,   42,   43,   -1,   45,   -1,   47,   -1,   -1,
   -1,   37,   38,   -1,   -1,   -1,   42,   43,   -1,   45,
   -1,   47,   -1,   -1,   37,   38,   -1,   -1,   -1,   42,
   43,   -1,   45,   -1,   47,   -1,   37,   38,   -1,   -1,
   94,   42,   43,   -1,   45,   -1,   47,   37,   38,   -1,
   -1,   -1,   42,   43,   94,   45,   -1,   47,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   94,   -1,
  124,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   94,   40,   41,  124,   43,   -1,   45,   -1,   -1,
   -1,   -1,   -1,   94,   -1,   -1,   -1,   -1,  124,   -1,
   -1,   -1,   -1,   -1,   94,   -1,   -1,   40,   41,   -1,
   43,  124,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  124,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  124,   -1,   94,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   40,
   41,   -1,   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,
   -1,   94,   -1,   -1,   -1,   -1,   -1,   -1,  126,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  126,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   94,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  271,  272,  273,
  274,   -1,  276,  277,  278,  279,  280,  281,  282,  283,
  284,  271,  272,  273,  274,  126,  276,  277,  278,  279,
  280,  281,  282,  283,  284,  271,  272,  273,  274,   -1,
  276,  277,  278,  279,  280,  281,  282,  283,  284,  272,
  273,  274,   -1,  276,  277,  278,  279,  280,  281,  282,
  283,  284,  273,  274,   -1,  276,  277,  278,  279,  280,
  281,  282,  283,  284,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   40,  283,  284,   43,   -1,   45,   -1,  257,
  258,  259,  260,  261,  262,  263,  264,  265,  266,  267,
  268,   -1,   -1,   -1,   -1,  273,   -1,  275,   -1,   -1,
   -1,   -1,  280,   -1,  257,  258,  259,  260,  261,  262,
  263,  264,  265,  266,  267,  268,   40,   -1,   -1,   43,
  273,   45,  275,   -1,   -1,   93,   94,  280,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,  260,
  261,  262,  263,  264,  265,  266,  267,  268,  126,   -1,
   -1,   -1,  273,   -1,  275,   -1,   -1,   -1,   -1,  280,
   94,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   13,   14,   -1,   -1,   -1,   18,   19,   20,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  126,   -1,   36,   37,   38,   39,   40,   41,
   -1,   43,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   58,   59,   -1,   -1,
   62,   63,   64,   65,   66,   67,   68,   -1,   -1,   71,
   72,   73,   74,   75,   76,   77,   78,   -1,   80,   -1,
   82,   -1,   84,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  105,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  116,   -1,   -1,   -1,   -1,  257,
  258,  259,  260,  261,  262,  263,  264,  265,  266,  267,
  268,   -1,   -1,   -1,   -1,  273,  138,  275,  140,   -1,
   -1,   -1,  280,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  153,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  257,  258,  259,  260,  261,  262,  263,
  264,  265,  266,  267,  268,   -1,   -1,   -1,   -1,  273,
   -1,  275,   -1,   -1,   -1,   -1,  280,
  };
	CriteriaLexer lexer;
	public void yyerror (string message) {
		yyerror(message, null);
	}
	public void yyerror (string message, string[] expected) {
		string buf = message;
		if ((expected != null) && (expected.Length  > 0)) {
			buf += message;
			buf += ", expecting\n";
			for (int n = 0; n < expected.Length; ++ n)
				buf += (" "+expected[n]);
			buf += "\n";
		}
		throw new CriteriaParserException(buf);
	}
	void Parse(String query) {
		Parse(query, false);
	}
	void Parse(String query, bool allowSort) {
		StringReader sr = new System.IO.StringReader(query);
		lexer = new CriteriaLexer(sr);
		lexer.RecogniseSortings = allowSort;
		try {
			yyparse(lexer);
		} catch(CriteriaParserException e) {
			string malformedQuery = query;
			if(lexer.Line == 0) {
				try {
					malformedQuery = malformedQuery.Substring(0, lexer.Col) + FilteringExceptionsText.ErrorPointer + malformedQuery.Substring(lexer.Col);
				} catch { }
			}
			throw new CriteriaParserException(String.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.GrammarCatchAllErrorMessage, lexer.Line, lexer.Col, e.Message, malformedQuery), lexer.Line, lexer.Col);
		}
	}
	public static CriteriaOperator[] ParseList(string criteriaList, out OperandValue[] criteriaParametersList, bool allowSorting) {
		CriteriaParser parser = new CriteriaParser();
		parser.Parse(criteriaList, allowSorting);
		criteriaParametersList = parser.ResultParameters.ToArray();
		return parser.Result;
	}
	public static CriteriaOperator Parse(string stringCriteria, out OperandValue[] criteriaParametersList) {
		if(stringCriteria == null) {
			criteriaParametersList = null;
			return null;
		}
		CriteriaParser parser = new CriteriaParser();
		parser.Parse(stringCriteria);
		criteriaParametersList = parser.ResultParameters.ToArray();
		switch(parser.Result.Length) {
			case 0:
				return null;
			case 1:
				return parser.Result[0];
			default:
				throw new ArgumentException("single criterion expected", "stringCriteria");	
		}
	}
}
 class Token {
  public const int CONST = 257;
  public const int AGG_EXISTS = 258;
  public const int AGG_COUNT = 259;
  public const int AGG_MIN = 260;
  public const int AGG_MAX = 261;
  public const int AGG_AVG = 262;
  public const int AGG_SUM = 263;
  public const int AGG_SINGLE = 264;
  public const int PARAM = 265;
  public const int COL = 266;
  public const int FN_ISNULL = 267;
  public const int FUNCTION = 268;
  public const int SORT_ASC = 269;
  public const int SORT_DESC = 270;
  public const int OR = 271;
  public const int AND = 272;
  public const int NOT = 273;
  public const int IS = 274;
  public const int NULL = 275;
  public const int OP_EQ = 276;
  public const int OP_NE = 277;
  public const int OP_LIKE = 278;
  public const int OP_GT = 279;
  public const int OP_LT = 280;
  public const int OP_GE = 281;
  public const int OP_LE = 282;
  public const int OP_IN = 283;
  public const int OP_BETWEEN = 284;
  public const int NEG = 285;
  public const int yyErrorCode = 256;
 }
 interface yyInput {
   bool advance ();
   int token ();
   Object value ();
 }
} 
