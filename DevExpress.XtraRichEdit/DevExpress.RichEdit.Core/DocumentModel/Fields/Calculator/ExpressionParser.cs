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

using System.IO;
using System;
namespace DevExpress.XtraRichEdit.Fields.Expression {
	public class RichEditFieldExpressionParser {
	const int _EOF = 0;
	const int _OpEQ = 1;
	const int _OpNEQ = 2;
	const int _OpLOW = 3;
	const int _OpLOWEQ = 4;
	const int _OpHI = 5;
	const int _OpHIEQ = 6;
	const int _OpPLUS = 7;
	const int _OpMINUS = 8;
	const int _OpMUL = 9;
	const int _OpDIV = 10;
	const int _OpPOW = 11;
	const int _OpenParenthesis = 12;
	const int _CloseParenthesis = 13;
	const int _SimpleToken = 14;
	const int _DocPropertyInfoCommonToken = 15;
	const int _DocPropertyCategoryToken = 16;
	const int _DocPropertyToken = 17;
	const int _DocumentInformationToken = 18;
	const int _EqToken = 19;
	const int _DateAndTimeFormattingSwitchBegin = 20;
	const int _GeneralFormattingSwitchBegin = 21;
	const int _NumbericFormattingSwitchBegin = 22;
	const int _CommonStringFormatSwitchBegin = 23;
	const int _Text = 24;
	const int _QuotedText = 25;
	const int _FieldSwitchCharacter = 26;
	const int _Constant = 27;
	const int _Percent = 28;
	const int _SeparatorChar = 29;
	const int _FunctionName = 30;
	const int maxT = 31;
		const bool T = true;
		const bool x = false;
		const int minErrDist = 2;
		ExpressionScanner scanner;
		Errors errors;
		Token t;	
		Token la;   
		int errDist = minErrDist;
		ExpressionFieldBase result;
	public ExpressionFieldBase GetResult()
	{
	  return result;
	}
		public RichEditFieldExpressionParser(ExpressionScanner scanner) {
			this.scanner = scanner;
			errors = new Errors();
		}
		void SynErr(int n) {
			if (errDist >= minErrDist)
				errors.SynErr(0, 0, n);
			errDist = 0;
		}
		public void SemErr(string msg) {
			if (errDist >= minErrDist)
				errors.SemErr(0, 0, msg);
			errDist = 0;
		}
		void Get() {
			for (; ; ) {
				t = la;
				la = scanner.Scan();
				if (la.Kind <= maxT) { ++errDist; break; }
				la = t;
			}
		}
		void Expect(int n) {
			if (la.Kind == n)
				Get();
			else { SynErr(n); }
		}
		bool StartOf(int s) {
			return set[s, la.Kind];
		}
		void ExpectWeak(int n, int follow) {
			if (la.Kind == n)
				Get();
			else {
				SynErr(n);
				while (!StartOf(follow))
					Get();
			}
		}
		bool WeakSeparator(int n, int syFol, int repFol) {
			int kind = la.Kind;
			if (kind == n) { Get(); return true; }
			else if (StartOf(repFol)) { return false; }
			else {
				SynErr(n);
				while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
					Get();
					kind = la.Kind;
				}
				return StartOf(syFol);
			}
		}
			void ExpressionField() {
		ExpressionFieldCore(out result);
	}
	void ExpressionFieldCore(out ExpressionFieldBase field) {
		field = null; 
		field = new ExpressionFieldBase(); 
		EquationsAndFormulas(field.ExpressionTree);
	}
	void EquationsAndFormulas(ExpressionTree expressionTree) {
		FormulaNodeBase rootNode; 
		Expression(out rootNode);
		expressionTree.Root = rootNode; 
	}
	void Expression(out FormulaNodeBase obj) {
		CompareExpression(out obj);
	}
	void CompareExpression(out FormulaNodeBase result) {
		FormulaNodeBase leftNode; 
		AdditiveExpression(out leftNode);
		RestCompare(leftNode, out result);
	}
	void AdditiveExpression(out FormulaNodeBase result) {
		FormulaNodeBase leftNode; 
		MultiplyExpression(out leftNode);
		RestAdditive(leftNode, out result);
	}
	void RestCompare(FormulaNodeBase leftNode, out FormulaNodeBase result) {
		result = null;
		if (StartOf(1)) {
			FormulaNodeBase innerExpression;
			string op;		
			switch (la.Kind) {
			case 1: {
				Get();
				break;
			}
			case 2: {
				Get();
				break;
			}
			case 3: {
				Get();
				break;
			}
			case 4: {
				Get();
				break;
			}
			case 5: {
				Get();
				break;
			}
			case 6: {
				Get();
				break;
			}
			}
			op = t.Value; 
			AdditiveExpression(out innerExpression);
			leftNode = FormulaNodeBase.GetFormulaNode(leftNode, innerExpression, op); 
			RestCompare(leftNode, out result);
		} else if (la.Kind == 0 || la.Kind == 13 || la.Kind == 29) {
			result = leftNode; 
		} else SynErr(32);
	}
	void MultiplyExpression(out FormulaNodeBase result) {
		FormulaNodeBase leftNode; 
		PowExpression(out leftNode);
		RestMultiply(leftNode, out result);
	}
	void RestAdditive(FormulaNodeBase leftNode, out FormulaNodeBase result) {
		result = null;
		if (la.Kind == 7 || la.Kind == 8) {
			FormulaNodeBase innerExpression;
			string op;		
			if (la.Kind == 7) {
				Get();
			} else {
				Get();
			}
			op = t.Value; 
			MultiplyExpression(out innerExpression);
			leftNode = FormulaNodeBase.GetFormulaNode(leftNode, innerExpression, op); 
			RestAdditive(leftNode, out result);
		} else if (StartOf(2)) {
			result = leftNode; 
		} else SynErr(33);
	}
	void PowExpression(out FormulaNodeBase result) {
		FormulaNodeBase leftResult; 
		FormulaNodeBase rightResult = null; 
		string op = String.Empty; 
		UnaryExpression(out leftResult);
		if (la.Kind == 11) {
			Get();
			op = t.Value; 
			PowExpression(out rightResult);
		}
		result = FormulaNodeBase.GetFormulaNode(leftResult, rightResult, op); 
	}
	void RestMultiply(FormulaNodeBase leftNode, out FormulaNodeBase result) {
		result = null;
		if (la.Kind == 9 || la.Kind == 10) {
			FormulaNodeBase innerExpression;
			string op;		
			if (la.Kind == 9) {
				Get();
			} else {
				Get();
			}
			op = t.Value; 
			PowExpression(out innerExpression);
			leftNode = FormulaNodeBase.GetFormulaNode(leftNode, innerExpression, op); 
			RestMultiply(leftNode, out result);
		} else if (StartOf(3)) {
			result = leftNode; 
		} else SynErr(34);
	}
	void UnaryExpression(out FormulaNodeBase result) {
		result = null; 
		int unaryMinusCount = 0; 
		while (la.Kind == 8) {
			Get();
			unaryMinusCount++; 
		}
		if (la.Kind == 24 || la.Kind == 27) {
			PrimitiveExpression(out result);
		} else if (la.Kind == 12) {
			Get();
			Expression(out result);
			Expect(13);
		} else if (la.Kind == 30) {
			Get();
			string functionName = t.Value; 
			ArgumentList arguments = new ArgumentList();
			Expect(12);
			if (StartOf(4)) {
				FunctionArguments(arguments);
			}
			Expect(13);
			result = FormulaNodeBase.GetFunctionCallNode(functionName, arguments); 
		} else SynErr(35);
		if((unaryMinusCount % 2) != 0)result = new UnaryMinus(result); 
	}
	void PrimitiveExpression(out FormulaNodeBase result) {
		Primitive(out result);
		if (la.Kind == 28) {
			Get();
			result = new Percent(result); 
		}
	}
	void FunctionArguments(ArgumentList arguments) {
		FormulaNodeBase argument; 
		Expression(out argument);
		arguments.Add(argument); 
		while (la.Kind == 29) {
			Get();
			Expression(out argument);
			arguments.Add(argument); 
		}
	}
	void Primitive(out FormulaNodeBase result) {
		result = null; 
		if (la.Kind == 27) {
			Get();
			result = new PrimitiveFormulaNode(t.Value); 
		} else if (la.Kind == 24) {
			Get();
			result = new ReferenceFormulaNode(t.Value); 
		} else SynErr(36);
	}
		public void Parse() {
			la = new Token();
			la.Value = "";
			Get();
					ExpressionField();
			Expect(0);
		}
		bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x},
		{T,T,T,T, T,T,T,T, T,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x},
		{x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,T,x, x}
					  };
	} 
	public class Errors {
		#region Fields
		int count = 0;									
		TextWriter errorStream = Console.Out;   
		string errMsgFormat = "-- line {0} col {1}: {2}"; 
		#endregion
		#region Properties
		public int Count { get { return count; } set { count = value; } }
		public TextWriter ErrorStream { get { return errorStream; } set { errorStream = value; } }
		public string ErrMsgFormat { get { return errMsgFormat; } set { errMsgFormat = value; } }
		#endregion
		public void SynErr(int line, int col, int n) {
			string s;
			switch (n) {
							case 0: s = "EOF expected"; break;
			case 1: s = "OpEQ expected"; break;
			case 2: s = "OpNEQ expected"; break;
			case 3: s = "OpLOW expected"; break;
			case 4: s = "OpLOWEQ expected"; break;
			case 5: s = "OpHI expected"; break;
			case 6: s = "OpHIEQ expected"; break;
			case 7: s = "OpPLUS expected"; break;
			case 8: s = "OpMINUS expected"; break;
			case 9: s = "OpMUL expected"; break;
			case 10: s = "OpDIV expected"; break;
			case 11: s = "OpPOW expected"; break;
			case 12: s = "OpenParenthesis expected"; break;
			case 13: s = "CloseParenthesis expected"; break;
			case 14: s = "SimpleToken expected"; break;
			case 15: s = "DocPropertyInfoCommonToken expected"; break;
			case 16: s = "DocPropertyCategoryToken expected"; break;
			case 17: s = "DocPropertyToken expected"; break;
			case 18: s = "DocumentInformationToken expected"; break;
			case 19: s = "EqToken expected"; break;
			case 20: s = "DateAndTimeFormattingSwitchBegin expected"; break;
			case 21: s = "GeneralFormattingSwitchBegin expected"; break;
			case 22: s = "NumbericFormattingSwitchBegin expected"; break;
			case 23: s = "CommonStringFormatSwitchBegin expected"; break;
			case 24: s = "Text expected"; break;
			case 25: s = "QuotedText expected"; break;
			case 26: s = "FieldSwitchCharacter expected"; break;
			case 27: s = "Constant expected"; break;
			case 28: s = "Percent expected"; break;
			case 29: s = "SeparatorChar expected"; break;
			case 30: s = "FunctionName expected"; break;
			case 31: s = "??? expected"; break;
			case 32: s = "invalid RestCompare"; break;
			case 33: s = "invalid RestAdditive"; break;
			case 34: s = "invalid RestMultiply"; break;
			case 35: s = "invalid UnaryExpression"; break;
			case 36: s = "invalid Primitive"; break;
				default: s = "error " + n; break;
			}
			ErrorStream.WriteLine(ErrMsgFormat, line, col, s);
			Count++;
		}
		public void SemErr(int line, int col, string s) {
			ErrorStream.WriteLine(ErrMsgFormat, line, col, s);
			Count++;
		}
		public void SemErr(string s) {
			ErrorStream.WriteLine(s);
			Count++;
		}
		public void Warning(int line, int col, string s) {
			ErrorStream.WriteLine(ErrMsgFormat, line, col, s);
		}
		public void Warning(string s) {
			ErrorStream.WriteLine(s);
		}
	} 
	public class FatalError : Exception {
		public FatalError(string m) : base(m) { }
	}
}
