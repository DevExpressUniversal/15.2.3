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

namespace DevExpress.Data.Mask {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	class DupSymbol {
		public readonly int MinMatches, MaxMatches;
		public DupSymbol(object minMatches, object maxMatches) {
			this.MinMatches = (int)minMatches;
			this.MaxMatches = (int)maxMatches;
		}
	}
	class RegExpParser {
		Dfa result;
		public Dfa Result { get { return result; } }
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
#pragma warning disable 162
	for(; ; ++yyTop) {
#pragma warning restore 162
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
  { result = (Dfa)yyVals[-1+yyTop]; }
  break;
case 2:
  { result = Dfa.Empty; }
  break;
case 3:
  { yyVal = yyVals[0+yyTop]; }
  break;
case 4:
  { yyVal = ((Dfa)yyVals[-2+yyTop] | (Dfa)yyVals[0+yyTop]); }
  break;
case 5:
  { yyVal = yyVals[0+yyTop]; }
  break;
case 6:
  {
													if(!reverseAutomate)
														yyVal = ((Dfa)yyVals[-1+yyTop] & (Dfa)yyVals[0+yyTop]);
													else
														yyVal = ((Dfa)yyVals[0+yyTop] & (Dfa)yyVals[-1+yyTop]);
												}
  break;
case 7:
  { yyVal = yyVals[0+yyTop]; }
  break;
case 8:
  { yyVal = yyVals[-1+yyTop]; }
  break;
case 9:
  { yyVal = Dfa.Power((Dfa)yyVals[-1+yyTop],((DupSymbol)yyVals[0+yyTop]).MinMatches,((DupSymbol)yyVals[0+yyTop]).MaxMatches); }
  break;
case 10:
  { yyVal = new Dfa(new OneSymbolTransition((char)yyVals[0+yyTop])); }
  break;
case 11:
  { yyVal = new Dfa(new AnySymbolTransition()); }
  break;
case 12:
  { yyVal = new Dfa((Transition)yyVals[0+yyTop]); }
  break;
case 13:
  { yyVal = new Dfa(new DecimalDigitTransition(false)); }
  break;
case 14:
  { yyVal = new Dfa(new DecimalDigitTransition(true)); }
  break;
case 15:
  { yyVal = new Dfa(new WhiteSpaceTransition(false)); }
  break;
case 16:
  { yyVal = new Dfa(new WhiteSpaceTransition(true)); }
  break;
case 17:
  { yyVal = new Dfa(new WordTransition(false)); }
  break;
case 18:
  { yyVal = new Dfa(new WordTransition(true)); }
  break;
case 19:
  { yyVal = new Dfa(new UnicodeCategoryTransition((string)yyVals[0+yyTop], false)); }
  break;
case 20:
  { yyVal = new Dfa(new UnicodeCategoryTransition((string)yyVals[0+yyTop], true)); }
  break;
case 21:
  { yyVal = new DupSymbol(0,-1); }
  break;
case 22:
  { yyVal = new DupSymbol(1,-1); }
  break;
case 23:
  { yyVal = new DupSymbol(0,1); }
  break;
case 24:
  { yyVal = new DupSymbol(yyVals[-1+yyTop],yyVals[-1+yyTop]); }
  break;
case 25:
  { yyVal = new DupSymbol(yyVals[-2+yyTop],-1); }
  break;
case 26:
  { yyVal = new DupSymbol(yyVals[-3+yyTop],yyVals[-1+yyTop]); }
  break;
case 27:
  { yyVal = new BracketTransition(false, ((List<BracketTransitionRange>)yyVals[-1+yyTop]).ToArray()); }
  break;
case 28:
  { yyVal = new BracketTransition(true, ((List<BracketTransitionRange>)yyVals[-1+yyTop]).ToArray()); }
  break;
case 29:
  { List<BracketTransitionRange> newList = new List<BracketTransitionRange>(); newList.Add((BracketTransitionRange)yyVals[0+yyTop]); yyVal = newList; }
  break;
case 30:
  { yyVal = yyVals[-1+yyTop]; ((List<BracketTransitionRange>)yyVal).Add((BracketTransitionRange)yyVals[0+yyTop]); }
  break;
case 31:
  { yyVal = new BracketTransitionRange((char)yyVals[0+yyTop], (char)yyVals[0+yyTop]); }
  break;
case 32:
  { yyVal = new BracketTransitionRange((char)yyVals[-2+yyTop], (char)yyVals[0+yyTop]); }
  break;
case 33:
  { yyVal = yyVals[0+yyTop]; }
  break;
case 34:
  { yyVal = yyVals[0+yyTop]; }
  break;
case 35:
  { yyVal = (char)yyVals[0+yyTop] - '0'; }
  break;
case 36:
  { yyVal = ((int)yyVals[-1+yyTop])*10 + (char)yyVals[0+yyTop] - '0'; }
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
	0,	0,	1,	1,	2,	2,	3,	3,	3,	4,
	4,	4,	4,	4,	4,	4,	4,	4,	4,	4,
	5,	5,	5,	5,	5,	5,	7,	7,	9,	9,
   10,   10,	6,	6,	8,	8,
  };
   static  short [] yyLen = {		   2,
	2,	1,	1,	3,	1,	2,	1,	3,	2,	1,
	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,
	1,	1,	1,	3,	4,	5,	3,	4,	1,	2,
	1,	3,	1,	1,	1,	2,
  };
   static  short [] yyDefRed = {			0,
   33,   34,   13,   14,   15,   16,   17,   18,   19,   20,
	2,	0,   11,	0,	0,	0,	0,	0,	7,   10,
   12,	0,	0,	0,	0,   29,	1,	0,	0,   21,
   22,   23,	0,	9,	8,	0,	0,   27,   30,	0,
   35,	0,   28,   32,   36,   24,	0,   25,	0,   26,
  };
  protected static  short [] yyDgoto  = {			15,
   16,   17,   18,   19,   34,   20,   21,   42,   25,   26,
  };
  protected static  int yyFinal = 15;
  protected static  short [] yySindex = {		   21,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,  -38,	0,  -84,	0,   12,  -38,  -26,	0,	0,
	0,  -28, -238,  -22,  -90,	0,	0,  -38,  -26,	0,
	0,	0, -234,	0,	0,  -88, -238,	0,	0,  -38,
	0,  -44,	0,	0,	0,	0, -121,	0, -119,	0,
  };
  protected static  short [] yyRindex = {			0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	9,	1,	0,	0,
	0,	0,	0,  -86,	0,	0,	0,	0,   11,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,   14,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
  };
  protected static  short [] yyGindex = {			0,
   13,   -1,   -2,	0,	0,	8,	0,  -21,	5,   -7,
  };
  protected static  short [] yyTable = {			47,
	5,   12,   38,   48,   43,   50,   31,   13,	3,   23,
	6,   27,   35,	4,   29,   30,   31,   39,	1,	2,
   11,   24,   37,   41,   22,   49,   40,   36,   39,	0,
   24,	0,   24,	0,	0,	0,   32,   29,	0,	0,
	5,	5,	0,   24,   44,	0,	5,	0,	0,	3,
	6,	6,   14,	0,	4,	0,	6,	0,	0,	0,
   12,	0,	0,	0,	0,	0,   13,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
   46,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	5,	0,	0,	0,   28,   33,	0,	0,	0,
	0,	6,	0,	0,	0,	0,	0,	0,	0,	0,
	0,   14,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	5,	0,	0,	0,	0,	0,
	0,	0,	3,	0,	6,   28,   41,	4,   45,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	1,	2,	1,	2,
   31,   31,	1,	2,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,   45,	0,	0,	0,	0,	1,	2,
	3,	4,	5,	6,	7,	8,	9,   10,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	5,	5,	5,
	5,	5,	5,	5,	5,	5,	5,	6,	6,	6,
	6,	6,	6,	6,	6,	6,	6,	1,	2,	3,
	4,	5,	6,	7,	8,	9,   10,
  };
  protected static  short [] yyCheck = {			44,
	0,   40,   93,  125,   93,  125,   93,   46,	0,   94,
	0,	0,   41,	0,   17,   42,   43,   25,  257,  258,
	0,   14,   45,  258,   12,   47,   28,   23,   36,   -1,
   23,   -1,   25,   -1,   -1,   -1,   63,   40,   -1,   -1,
   40,   41,   -1,   36,   37,   -1,   46,   -1,   -1,   41,
   40,   41,   91,   -1,   41,   -1,   46,   -1,   -1,   -1,
   40,   -1,   -1,   -1,   -1,   -1,   46,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   91,   -1,   -1,   -1,  124,  123,   -1,   -1,   -1,
   -1,   91,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   91,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  124,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  124,   -1,  124,  124,  258,  124,  258,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,  257,  258,
  257,  258,  257,  258,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  258,   -1,   -1,   -1,   -1,  257,  258,
  259,  260,  261,  262,  263,  264,  265,  266,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,
  260,  261,  262,  263,  264,  265,  266,  257,  258,  259,
  260,  261,  262,  263,  264,  265,  266,  257,  258,  259,
  260,  261,  262,  263,  264,  265,  266,
  };
	Yylex lexer;
	bool reverseAutomate;
	void yyerror (string message) {
		yyerror(message, null);
	}
	void yyerror (string message, string[] expected) {
		string buf = message;
		if ((expected != null) && (expected.Length  > 0)) {
			buf += message;
			buf += ", expecting\n";
			for (int n = 0; n < expected.Length; ++ n)
				buf += (" "+expected[n]);
			buf += "\n";
		}
		throw new ArgumentException(buf);
	}
	Dfa Parse(TextReader reader, bool reverseAutomate, CultureInfo parseCulture) {
		this.lexer = new Yylex(reader, parseCulture);
		this.reverseAutomate = reverseAutomate;
		yyparse(lexer);
		return Result;
	}
	public static Dfa Parse(string regExp, bool reverseAutomate, CultureInfo parseCulture) {
		return new RegExpParser().Parse(new StringReader(regExp), reverseAutomate, parseCulture);
	}
}
 class Token {
  public const int CHAR = 257;
  public const int DIGIT = 258;
  public const int CHARClassDecimalDigit = 259;
  public const int CHARClassNonDecimalDigit = 260;
  public const int CHARClassWhiteSpace = 261;
  public const int CHARClassNonWhiteSpace = 262;
  public const int CHARClassWord = 263;
  public const int CHARClassNonWord = 264;
  public const int CHARClassUnicodeCategory = 265;
  public const int CHARClassUnicodeCategoryNot = 266;
  public const int yyErrorCode = 256;
 }
 interface yyInput {
   bool advance ();
   int token ();
   Object value ();
 }
} 
