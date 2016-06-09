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
using System.Collections;
using System.Collections.Specialized;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.CSharp.Preprocessor
#else
namespace DevExpress.CodeParser.CSharp.Preprocessor
#endif
{
	public  class Tokens
	{
		public const int LINE = 143;
		public const int SBYTE = 60;
		public const int LINETERMINATOR = 7;
		public const int EQ = 96;
		public const int SCOLON = 118;
		public const int REGION = 146;
		public const int ENDREG = 147;
		public const int STRUCT = 66;
		public const int BASE = 10;
		public const int CASE = 14;
		public const int RAZORCOMMENT = 126;
		public const int RBRACE = 115;
		public const int FINALLY = 32;
		public const int NEW = 48;
		public const int LBRACE = 100;
		public const int ARGLIST = 83;
		public const int GTEQ = 98;
		public const int ELIF = 140;
		public const int USINGKW = 78;
		public const int MINUSASSGN = 107;
		public const int LOCK = 45;
		public const int ENDIF = 142;
		public const int RBRACK = 116;
		public const int TYPEOF = 72;
		public const int EXPLICIT = 29;
		public const int IN = 40;
		public const int IDENT = 1;
		public const int LONG = 46;
		public const int REF = 58;
		public const int COMMA = 91;
		public const int STRINGCON = 5;
		public const int LBRACK = 101;
		public const int FLOAT = 34;
		public const int UNSAFE = 76;
		public const int REFVALUE = 84;
		public const int PRAGMADIR = 148;
		public const int IFCLAUSE = 38;
		public const int OUT = 51;
		public const int DO = 24;
		public const int ULONG = 74;
		public const int CHARCON = 4;
		public const int WHILE = 82;
		public const int DEC = 92;
		public const int PLUSASSGN = 113;
		public const int MODASSGN = 108;
		public const int TRY = 71;
		public const int ORASSGN = 111;
		public const int DBLQUEST = 127;
		public const int LAMBDA = 136;
		public const int THIS = 68;
		public const int SWITCH = 67;
		public const int EXTERN = 30;
		public const int CONST = 19;
		public const int DELEGATE = 23;
		public const int MINUS = 106;
		public const int WARNING = 145;
		public const int TRUE = 70;
		public const int CHAR = 16;
		public const int TILDE = 119;
		public const int VOID = 80;
		public const int TIMESASSGN = 121;
		public const int EVENT = 28;
		public const int DEFINE = 137;
		public const int BREAK = 12;
		public const int UNDEF = 138;
		public const int DBLAND = 129;
		public const int PRIVATE = 54;
		public const int ATCOLON = 88;
		public const int BYTE = 13;
		public const int READONLY = 57;
		public const int DOUBLE = 25;
		public const int RETURN = 59;
		public const int TIMES = 120;
		public const int SEALED = 61;
		public const int LOWOREQ = 132;
		public const int LTLT = 105;
		public const int UNCHECKED = 75;
		public const int OR = 130;
		public const int MULTILINECOMMENT = 125;
		public const int DEFAULT = 22;
		public const int DBLCOLON = 95;
		public const int SINGLELINECOMMENT = 124;
		public const int XOR = 131;
		public const int NAMESPACE = 47;
		public const int CONTINUE = 20;
		public const int SIZEOF = 63;
		public const int ANDASSGN = 86;
		public const int THROW = 69;
		public const int CATCH = 15;
		public const int LT = 104;
		public const int ENUM = 27;
		public const int FOR = 35;
		public const int AT = 89;
		public const int PROTECTED = 55;
		public const int AND = 85;
		public const int COLON = 90;
		public const int LSHASSGN = 103;
		public const int OPERATOR = 50;
		public const int INTCON = 2;
		public const int GT = 97;
		public const int ERROR = 144;
		public const int SHORT = 62;
		public const int DIV = 133;
		public const int MOD = 134;
		public const int IMPLICIT = 39;
		public const int ABSTRACT = 8;
		public const int CLASS = 18;
		public const int UINT = 73;
		public const int FALSE = 31;
		public const int STACKALLOC = 64;
		public const int INTERFACE = 42;
		public const int ASSGN = 87;
		public const int INT = 41;
		public const int CHECKED = 17;
		public const int ELSE = 26;
		public const int USHORT = 77;
		public const int XORASSGN = 122;
		public const int GOTO = 37;
		public const int POINT = 135;
		public const int REALCON = 3;
		public const int NOT = 110;
		public const int NEQ = 109;
		public const int PLUS = 112;
		public const int DOT = 94;
		public const int FIXED = 33;
		public const int NULL = 49;
		public const int AS = 9;
		public const int VOLATILE = 81;
		public const int DIVASSGN = 93;
		public const int INC = 99;
		public const int ELSEDIR = 141;
		public const int QUESTION = 114;
		public const int BOOL = 11;
		public const int RPAR = 117;
		public const int OVERRIDE = 52;
		public const int EOF = 0;
		public const int STATIC = 65;
		public const int LPAR = 102;
		public const int IFDIR = 139;
		public const int VIRTUAL = 79;
		public const int FOREACH = 36;
		public const int IS = 44;
		public const int DECIMAL = 21;
		public const int PUBLIC = 56;
		public const int DBLOR = 128;
		public const int INTERNAL = 43;
		public const int PARAMS = 53;
		public const int MaxTokens = 156;
		public static int[] Keywords = {
		};
	}
	public class CSharpPreprocessor : PreprocessorBase
	{
	public CSharpPreprocessor(CSharpScanner scanner, SourceFile rootNode)
		: base(scanner, rootNode)
	{
		errors = new PreprocessorErrors();
		set = CreateSetArray();
		maxTokens = Tokens.MaxTokens;
	}
	public CSharpPreprocessor(CSharpScanner scanner, FormattingParserBase parser, SourceFile rootNode)
		: this(scanner, rootNode)
	{
		Parser = parser;
	if (Parser != null && Parser.SetTokensCategory)
	  PreprocessMode = PreprocessMode.None;
	}
		protected override void Get()
		{
			for (;;)
			{
				ProcessFormattingToken();
				tToken = la;				
				la = Scanner.Scan();				
				if (la.Type <= Tokens.MaxTokens)
				{
					++errDist;
					tToken.Next = la;
					break;
				}
				la = tToken;
			}
			AddTokenToCategoryCollectionIfNeeded();
		}
			void PreprocessorRoot()
	{
		if (la.Type == Tokens.DEFINE  || la.Type == Tokens.UNDEF )
		{
			PpDeclaration();
		}
		else if (StartOf(1))
		{
			PpConditional();
		}
		else
			SynErr(157);
	}
	void PpDeclaration()
	{
		PreprocessorDirective directive = null;
		SourceRange range = la.Range;
		if (la.Type == Tokens.DEFINE )
		{
			Get();
			IdnetifierOrKeyword();
			string macroName = tToken.Value;
			DefineMacro(macroName);
			DefineDirective defDirective = new DefineDirective();
			defDirective.Expression = macroName;
				defDirective.Name = macroName;
			directive = defDirective;
		}
		else if (la.Type == Tokens.UNDEF )
		{
			Get();
			IdnetifierOrKeyword();
			string macroName = tToken.Value;
			UndefMacro(macroName);
			UndefDirective undefDirective = new UndefDirective();
			undefDirective.Symbol = macroName;
			directive = undefDirective;
		}
		else
			SynErr(158);
		SetDirectiveRange(directive, range);
		AddPreprocessorDirective(directive);
	}
	void PpConditional()
	{
		if (la.Type == Tokens.IFDIR )
		{
			PpIfSection();
		}
		else if (la.Type == Tokens.ELIF )
		{
			PpElIfSection();
		}
		else if (la.Type == Tokens.ELSEDIR )
		{
			PpElseSection();
		}
		else if (la.Type == Tokens.ENDIF )
		{
			PpEndIf();
		}
		else
			SynErr(159);
	}
	void IdnetifierOrKeyword()
	{
		if (la.Type == Tokens.IDENT )
		{
			Get();
		}
		else if (StartOf(2))
		{
			Keyword();
		}
		else
			SynErr(160);
	}
	void PpIfSection()
	{
		bool result = false;
		SourceRange range = la.Range;
		TurnOnScannerPreprocessMode();
		  bool OldSaveFormat = Parser.SaveFormat;
		  Parser.SaveFormat = true;
		Expect(Tokens.IFDIR );
		Token startExpressionToken = la;
		PpExpression(out result);
		Parser.SaveFormat = OldSaveFormat;
		IfDirective directive =  new IfDirective();
		directive.Expression = GetDirectiveExpression(startExpressionToken, tToken);
		   directive.ExpressionValue = result;
		SetDirectiveRange(directive, range);
		AddPreprocessorDirective(directive);
		CoditionDirectiveTail(WillSkipInIf(result));
		ProcessIFDirectiveCondition(result);
	}
	void PpElIfSection()
	{
		bool result = false;
		SourceRange range = la.Range;
		TurnOnScannerPreprocessMode();
		  bool OldSaveFormat = Parser.SaveFormat;
		  Parser.SaveFormat = true;
		Expect(Tokens.ELIF );
		Token expressionStartToken = la;
		PpExpression(out result);
		Parser.SaveFormat = OldSaveFormat;
		ElifDirective directive =  new ElifDirective();
		directive.Expression = GetDirectiveExpression(expressionStartToken, tToken);
		   directive.ExpressionValue = result;
		SetDirectiveRange(directive, range);
		AddPreprocessorDirective(directive);
		CoditionDirectiveTail(WillSkip(result));
		ProcessDirectiveCondition(result);
	}
	void PpElseSection()
	{
		TurnOnScannerPreprocessMode();
		Expect(Tokens.ELSEDIR );
		ElseDirective directive =  new ElseDirective();
		  directive.IsSatisfied = !ConditionWasTrue;
		SetDirectiveRange(directive, tToken.Range);
		   bool willSkip = WillSkip(true);
		AddPreprocessorDirective(directive);
		CoditionDirectiveTail(willSkip);
		ProcessDirectiveCondition(true);
	}
	void PpEndIf()
	{
		Expect(Tokens.ENDIF );
		EndIfDirective directive =  new EndIfDirective();
		SetDirectiveRange(directive, tToken.Range);
		AddPreprocessorDirective(directive);
		   ProcessNonNewLineToken(tToken as FormattingToken);
		ProcessEndIf();
	}
	void PpExpression(out bool result)
	{
		result = false;
		AddFormattingTokens = false;
		PpOrExpression(out result);
		AddFormattingTokens = true; 
	}
	void CoditionDirectiveTail(bool willSkip)
	{
		ProcessSingleLineComment(la as FormattingToken);
		TurnOffScannerPreprocessMode();
		if (la.Type == Tokens.LINETERMINATOR)
		  AddLineTerminatorToFormattingTokens();
		if (willSkip)
		  return;
		ProcessLineTerminator(la as FormattingToken);
		Expect(Tokens.LINETERMINATOR );
	}
	void PpOrExpression(out bool result)
	{
		result = false;
		bool rightPart = false;
		PpAndExpression(out result);
		while (la.Type == Tokens.DBLOR )
		{
			Get();
			PpAndExpression(out rightPart);
			result = result || rightPart;
		}
	}
	void PpAndExpression(out bool result)
	{
		result = false;
		bool rightPart = true;
		PpEqualityExpression(out result);
		while (la.Type == Tokens.DBLAND )
		{
			Get();
			PpEqualityExpression(out rightPart);
			result = result && rightPart;
		}
	}
	void PpEqualityExpression(out bool result)
	{
		result = false;
		bool rightPart = false;
		bool isEqualOp = false;
		PpUnaryExpression(out result);
		while (la.Type == Tokens.EQ  || la.Type == Tokens.NEQ )
		{
			if (la.Type == Tokens.EQ )
			{
				Get();
				isEqualOp = true;
			}
			else
			{
				Get();
				isEqualOp = false; 
			}
			PpUnaryExpression(out rightPart);
			if (isEqualOp)
			{
				result = result == rightPart;
			}
			else
			{
				result = result != rightPart;
			}
		}
	}
	void PpUnaryExpression(out bool result)
	{
		result = false;
		if (StartOf(3))
		{
			PpPrimaryExpression(out result);
		}
		else if (la.Type == Tokens.NOT )
		{
			Get();
			PpUnaryExpression(out result);
			result = !result; 
		}
		else
			SynErr(161);
	}
	void PpPrimaryExpression(out bool result)
	{
		result = false;
		if (la.Type == Tokens.TRUE )
		{
			Get();
			result = true; 
		}
		else if (la.Type == Tokens.FALSE )
		{
			Get();
			result = false; 
		}
		else if (StartOf(4))
		{
			IdnetifierOrKeyword();
			result = IsDefineMacro(tToken.Value); 
		}
		else if (la.Type == Tokens.LPAR )
		{
			ExprInParens(out result);
		}
		else
			SynErr(162);
	}
	void Keyword()
	{
		switch (la.Type)
		{
		case Tokens.ABSTRACT : 
		{
			Get();
			break;
		}
		case Tokens.AS : 
		{
			Get();
			break;
		}
		case Tokens.BASE : 
		{
			Get();
			break;
		}
		case Tokens.BOOL : 
		{
			Get();
			break;
		}
		case Tokens.BREAK : 
		{
			Get();
			break;
		}
		case Tokens.BYTE : 
		{
			Get();
			break;
		}
		case Tokens.CASE : 
		{
			Get();
			break;
		}
		case Tokens.CATCH : 
		{
			Get();
			break;
		}
		case Tokens.CHAR : 
		{
			Get();
			break;
		}
		case Tokens.CHECKED : 
		{
			Get();
			break;
		}
		case Tokens.CLASS : 
		{
			Get();
			break;
		}
		case Tokens.CONST : 
		{
			Get();
			break;
		}
		case Tokens.CONTINUE : 
		{
			Get();
			break;
		}
		case Tokens.DECIMAL : 
		{
			Get();
			break;
		}
		case Tokens.DEFAULT : 
		{
			Get();
			break;
		}
		case Tokens.DELEGATE : 
		{
			Get();
			break;
		}
		case Tokens.DO : 
		{
			Get();
			break;
		}
		case Tokens.DOUBLE : 
		{
			Get();
			break;
		}
		case Tokens.ELSE : 
		{
			Get();
			break;
		}
		case Tokens.ENUM : 
		{
			Get();
			break;
		}
		case Tokens.EVENT : 
		{
			Get();
			break;
		}
		case Tokens.EXPLICIT : 
		{
			Get();
			break;
		}
		case Tokens.EXTERN : 
		{
			Get();
			break;
		}
		case Tokens.FALSE : 
		{
			Get();
			break;
		}
		case Tokens.FINALLY : 
		{
			Get();
			break;
		}
		case Tokens.FIXED : 
		{
			Get();
			break;
		}
		case Tokens.FLOAT : 
		{
			Get();
			break;
		}
		case Tokens.FOR : 
		{
			Get();
			break;
		}
		case Tokens.FOREACH : 
		{
			Get();
			break;
		}
		case Tokens.GOTO : 
		{
			Get();
			break;
		}
		case Tokens.IFCLAUSE : 
		{
			Get();
			break;
		}
		case Tokens.IMPLICIT : 
		{
			Get();
			break;
		}
		case Tokens.IN : 
		{
			Get();
			break;
		}
		case Tokens.INT : 
		{
			Get();
			break;
		}
		case Tokens.INTERFACE : 
		{
			Get();
			break;
		}
		case Tokens.INTERNAL : 
		{
			Get();
			break;
		}
		case Tokens.IS : 
		{
			Get();
			break;
		}
		case Tokens.LOCK : 
		{
			Get();
			break;
		}
		case Tokens.LONG : 
		{
			Get();
			break;
		}
		case Tokens.NAMESPACE : 
		{
			Get();
			break;
		}
		case Tokens.NEW : 
		{
			Get();
			break;
		}
		case Tokens.NULL : 
		{
			Get();
			break;
		}
		case Tokens.OPERATOR : 
		{
			Get();
			break;
		}
		case Tokens.OUT : 
		{
			Get();
			break;
		}
		case Tokens.OVERRIDE : 
		{
			Get();
			break;
		}
		case Tokens.PARAMS : 
		{
			Get();
			break;
		}
		case Tokens.PRIVATE : 
		{
			Get();
			break;
		}
		case Tokens.PROTECTED : 
		{
			Get();
			break;
		}
		case Tokens.PUBLIC : 
		{
			Get();
			break;
		}
		case Tokens.READONLY : 
		{
			Get();
			break;
		}
		case Tokens.REF : 
		{
			Get();
			break;
		}
		case Tokens.RETURN : 
		{
			Get();
			break;
		}
		case Tokens.SBYTE : 
		{
			Get();
			break;
		}
		case Tokens.SEALED : 
		{
			Get();
			break;
		}
		case Tokens.SHORT : 
		{
			Get();
			break;
		}
		case Tokens.SIZEOF : 
		{
			Get();
			break;
		}
		case Tokens.STACKALLOC : 
		{
			Get();
			break;
		}
		case Tokens.STATIC : 
		{
			Get();
			break;
		}
		case Tokens.STRUCT : 
		{
			Get();
			break;
		}
		case Tokens.SWITCH : 
		{
			Get();
			break;
		}
		case Tokens.THIS : 
		{
			Get();
			break;
		}
		case Tokens.THROW : 
		{
			Get();
			break;
		}
		case Tokens.TRUE : 
		{
			Get();
			break;
		}
		case Tokens.TRY : 
		{
			Get();
			break;
		}
		case Tokens.TYPEOF : 
		{
			Get();
			break;
		}
		case Tokens.UINT : 
		{
			Get();
			break;
		}
		case Tokens.ULONG : 
		{
			Get();
			break;
		}
		case Tokens.UNCHECKED : 
		{
			Get();
			break;
		}
		case Tokens.UNSAFE : 
		{
			Get();
			break;
		}
		case Tokens.USHORT : 
		{
			Get();
			break;
		}
		case Tokens.USINGKW : 
		{
			Get();
			break;
		}
		case Tokens.VIRTUAL : 
		{
			Get();
			break;
		}
		case Tokens.VOID : 
		{
			Get();
			break;
		}
		case Tokens.VOLATILE : 
		{
			Get();
			break;
		}
		case Tokens.WHILE : 
		{
			Get();
			break;
		}
		default: SynErr(163); break;
		}
	}
	void ExprInParens(out bool result)
	{
		result = false;
		Expect(Tokens.LPAR );
		PpExpression(out result);
		Expect(Tokens.RPAR );
	}
		protected override void StartRule()
		{
					PreprocessorRoot();
		}
		protected override bool[,] CreateSetArray()
		{
			bool[,] set =
			{
				{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x}
			};
			return set;
		}
	} 
	public class PreprocessorErrors : ParserErrorsBase
	{
		protected override string GetSyntaxErrorText(int n)
		{
			string s;
			switch (n)
			{
				case 0: s = "EOF expected"; break;
			case 1: s = "IDENT expected"; break;
			case 2: s = "INTCON expected"; break;
			case 3: s = "REALCON expected"; break;
			case 4: s = "CHARCON expected"; break;
			case 5: s = "STRINGCON expected"; break;
			case 6: s = "SHARPCOCODIRECTIVE expected"; break;
			case 7: s = "LINETERMINATOR expected"; break;
			case 8: s = "ABSTRACT expected"; break;
			case 9: s = "AS expected"; break;
			case 10: s = "BASE expected"; break;
			case 11: s = "BOOL expected"; break;
			case 12: s = "BREAK expected"; break;
			case 13: s = "BYTE expected"; break;
			case 14: s = "CASE expected"; break;
			case 15: s = "CATCH expected"; break;
			case 16: s = "CHAR expected"; break;
			case 17: s = "CHECKED expected"; break;
			case 18: s = "CLASS expected"; break;
			case 19: s = "CONST expected"; break;
			case 20: s = "CONTINUE expected"; break;
			case 21: s = "DECIMAL expected"; break;
			case 22: s = "DEFAULT expected"; break;
			case 23: s = "DELEGATE expected"; break;
			case 24: s = "DO expected"; break;
			case 25: s = "DOUBLE expected"; break;
			case 26: s = "ELSE expected"; break;
			case 27: s = "ENUM expected"; break;
			case 28: s = "EVENT expected"; break;
			case 29: s = "EXPLICIT expected"; break;
			case 30: s = "EXTERN expected"; break;
			case 31: s = "FALSE expected"; break;
			case 32: s = "FINALLY expected"; break;
			case 33: s = "FIXED expected"; break;
			case 34: s = "FLOAT expected"; break;
			case 35: s = "FOR expected"; break;
			case 36: s = "FOREACH expected"; break;
			case 37: s = "GOTO expected"; break;
			case 38: s = "IFCLAUSE expected"; break;
			case 39: s = "IMPLICIT expected"; break;
			case 40: s = "IN expected"; break;
			case 41: s = "INT expected"; break;
			case 42: s = "INTERFACE expected"; break;
			case 43: s = "INTERNAL expected"; break;
			case 44: s = "IS expected"; break;
			case 45: s = "LOCK expected"; break;
			case 46: s = "LONG expected"; break;
			case 47: s = "NAMESPACE expected"; break;
			case 48: s = "NEW expected"; break;
			case 49: s = "NULL expected"; break;
			case 50: s = "OPERATOR expected"; break;
			case 51: s = "OUT expected"; break;
			case 52: s = "OVERRIDE expected"; break;
			case 53: s = "PARAMS expected"; break;
			case 54: s = "PRIVATE expected"; break;
			case 55: s = "PROTECTED expected"; break;
			case 56: s = "PUBLIC expected"; break;
			case 57: s = "READONLY expected"; break;
			case 58: s = "REF expected"; break;
			case 59: s = "RETURN expected"; break;
			case 60: s = "SBYTE expected"; break;
			case 61: s = "SEALED expected"; break;
			case 62: s = "SHORT expected"; break;
			case 63: s = "SIZEOF expected"; break;
			case 64: s = "STACKALLOC expected"; break;
			case 65: s = "STATIC expected"; break;
			case 66: s = "STRUCT expected"; break;
			case 67: s = "SWITCH expected"; break;
			case 68: s = "THIS expected"; break;
			case 69: s = "THROW expected"; break;
			case 70: s = "TRUE expected"; break;
			case 71: s = "TRY expected"; break;
			case 72: s = "TYPEOF expected"; break;
			case 73: s = "UINT expected"; break;
			case 74: s = "ULONG expected"; break;
			case 75: s = "UNCHECKED expected"; break;
			case 76: s = "UNSAFE expected"; break;
			case 77: s = "USHORT expected"; break;
			case 78: s = "USINGKW expected"; break;
			case 79: s = "VIRTUAL expected"; break;
			case 80: s = "VOID expected"; break;
			case 81: s = "VOLATILE expected"; break;
			case 82: s = "WHILE expected"; break;
			case 83: s = "ARGLIST expected"; break;
			case 84: s = "REFVALUE expected"; break;
			case 85: s = "AND expected"; break;
			case 86: s = "ANDASSGN expected"; break;
			case 87: s = "ASSGN expected"; break;
			case 88: s = "ATCOLON expected"; break;
			case 89: s = "AT expected"; break;
			case 90: s = "COLON expected"; break;
			case 91: s = "COMMA expected"; break;
			case 92: s = "DEC expected"; break;
			case 93: s = "DIVASSGN expected"; break;
			case 94: s = "DOT expected"; break;
			case 95: s = "DBLCOLON expected"; break;
			case 96: s = "EQ expected"; break;
			case 97: s = "GT expected"; break;
			case 98: s = "GTEQ expected"; break;
			case 99: s = "INC expected"; break;
			case 100: s = "LBRACE expected"; break;
			case 101: s = "LBRACK expected"; break;
			case 102: s = "LPAR expected"; break;
			case 103: s = "LSHASSGN expected"; break;
			case 104: s = "LT expected"; break;
			case 105: s = "LTLT expected"; break;
			case 106: s = "MINUS expected"; break;
			case 107: s = "MINUSASSGN expected"; break;
			case 108: s = "MODASSGN expected"; break;
			case 109: s = "NEQ expected"; break;
			case 110: s = "NOT expected"; break;
			case 111: s = "ORASSGN expected"; break;
			case 112: s = "PLUS expected"; break;
			case 113: s = "PLUSASSGN expected"; break;
			case 114: s = "QUESTION expected"; break;
			case 115: s = "RBRACE expected"; break;
			case 116: s = "RBRACK expected"; break;
			case 117: s = "RPAR expected"; break;
			case 118: s = "SCOLON expected"; break;
			case 119: s = "TILDE expected"; break;
			case 120: s = "TIMES expected"; break;
			case 121: s = "TIMESASSGN expected"; break;
			case 122: s = "XORASSGN expected"; break;
			case 123: s = "POINTERTOMEMBER expected"; break;
			case 124: s = "SINGLELINECOMMENT expected"; break;
			case 125: s = "MULTILINECOMMENT expected"; break;
			case 126: s = "RAZORCOMMENT expected"; break;
			case 127: s = "DBLQUEST expected"; break;
			case 128: s = "DBLOR expected"; break;
			case 129: s = "DBLAND expected"; break;
			case 130: s = "OR expected"; break;
			case 131: s = "XOR expected"; break;
			case 132: s = "LOWOREQ expected"; break;
			case 133: s = "DIV expected"; break;
			case 134: s = "MOD expected"; break;
			case 135: s = "POINT expected"; break;
			case 136: s = "LAMBDA expected"; break;
			case 137: s = "DEFINE expected"; break;
			case 138: s = "UNDEF expected"; break;
			case 139: s = "IFDIR expected"; break;
			case 140: s = "ELIF expected"; break;
			case 141: s = "ELSEDIR expected"; break;
			case 142: s = "ENDIF expected"; break;
			case 143: s = "LINE expected"; break;
			case 144: s = "ERROR expected"; break;
			case 145: s = "WARNING expected"; break;
			case 146: s = "REGION expected"; break;
			case 147: s = "ENDREG expected"; break;
			case 148: s = "PRAGMADIR expected"; break;
			case 149: s = "SINGLELINEXML expected"; break;
			case 150: s = "MULTILINEXML expected"; break;
			case 151: s = "GTGT expected"; break;
			case 152: s = "RSHASSGN expected"; break;
			case 153: s = "ASPBLOCKSTART expected"; break;
			case 154: s = "ASPBLOCKEND expected"; break;
			case 155: s = "ASPCOMMENT expected"; break;
			case 156: s = "??? expected"; break;
			case 157: s = "invalid PreprocessorRoot"; break;
			case 158: s = "invalid PpDeclaration"; break;
			case 159: s = "invalid PpConditional"; break;
			case 160: s = "invalid IdnetifierOrKeyword"; break;
			case 161: s = "invalid PpUnaryExpression"; break;
			case 162: s = "invalid PpPrimaryExpression"; break;
			case 163: s = "invalid Keyword"; break;
				default:
					s = "error " + n;
					break;
			}
			return s;
		}
	}	
}
