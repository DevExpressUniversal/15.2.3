%{
// ATTENTION ATTENTION ATTENTION ATTENTION
// this .CS file is a tool generated file from grammar.y and lexer.l
// DO NOT CHANGE BY HAND!!!!
// YOU HAVE BEEN WARNED !!!!

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
%}

%token	CHAR DIGIT
%token	CHARClassDecimalDigit CHARClassNonDecimalDigit CHARClassWhiteSpace CHARClassNonWhiteSpace CHARClassWord CHARClassNonWord
%token	CHARClassUnicodeCategory CHARClassUnicodeCategoryNot

/* Grammar follows */
%start  Dfa
%%

/* --------------------------------------------
   Extended Regular Expression
   --------------------------------------------
*/
Dfa	: extended_reg_exp '\0'	{ result = (Dfa)$1; }
	| '\0'	{ result = Dfa.Empty; }
	;

extended_reg_exp   :                      ERE_branch	{ $$ = $1; }
                   | extended_reg_exp '|' ERE_branch	{ $$ = ((Dfa)$1 | (Dfa)$3); }
                   ;
ERE_branch         :            ERE_expression	{ $$ = $1; }
                   | ERE_branch ERE_expression	{
													if(!reverseAutomate)
														$$ = ((Dfa)$1 & (Dfa)$2);
													else
														$$ = ((Dfa)$2 & (Dfa)$1);
												}
                   ;
ERE_expression     : one_character_ERE			{ $$ = $1; }
                   | '(' extended_reg_exp ')'	{ $$ = $2; }
                   | ERE_expression ERE_dupl_symbol { $$ = Dfa.Power((Dfa)$1,((DupSymbol)$2).MinMatches,((DupSymbol)$2).MaxMatches); }
                   ;
one_character_ERE  : symbol		{ $$ = new Dfa(new OneSymbolTransition((char)$1)); }
                   | '.'		{ $$ = new Dfa(new AnySymbolTransition()); }
                   | bracket_expression			{ $$ = new Dfa((Transition)$1); }
                   | CHARClassDecimalDigit		{ $$ = new Dfa(new DecimalDigitTransition(false)); }
                   | CHARClassNonDecimalDigit	{ $$ = new Dfa(new DecimalDigitTransition(true)); }
                   | CHARClassWhiteSpace		{ $$ = new Dfa(new WhiteSpaceTransition(false)); }
                   | CHARClassNonWhiteSpace		{ $$ = new Dfa(new WhiteSpaceTransition(true)); }
                   | CHARClassWord				{ $$ = new Dfa(new WordTransition(false)); }
                   | CHARClassNonWord			{ $$ = new Dfa(new WordTransition(true)); }
                   | CHARClassUnicodeCategory	{ $$ = new Dfa(new UnicodeCategoryTransition((string)$1, false)); }
                   | CHARClassUnicodeCategoryNot	{ $$ = new Dfa(new UnicodeCategoryTransition((string)$1, true)); }
                   ;
ERE_dupl_symbol    : '*'						{ $$ = new DupSymbol(0,-1); }
                   | '+'						{ $$ = new DupSymbol(1,-1); }
                   | '?'						{ $$ = new DupSymbol(0,1); }
                   | '{' number '}'				{ $$ = new DupSymbol($2,$2); }
                   | '{' number ',' '}'			{ $$ = new DupSymbol($2,-1); }
                   | '{' number ',' number '}'	{ $$ = new DupSymbol($2,$4); }
                   ;

/* --------------------------------------------
   Bracket Expression
   -------------------------------------------
*/
bracket_expression : '[' bracket_list ']'	{ $$ = new BracketTransition(false, ((List<BracketTransitionRange>)$2).ToArray()); }
				| '[' '^' bracket_list ']'	{ $$ = new BracketTransition(true, ((List<BracketTransitionRange>)$3).ToArray()); }
				;
bracket_list	:              expression_term	{ List<BracketTransitionRange> newList = new List<BracketTransitionRange>(); newList.Add((BracketTransitionRange)$1); $$ = newList; }
				| bracket_list expression_term	{ $$ = $1; ((List<BracketTransitionRange>)$$).Add((BracketTransitionRange)$2); }
				;
expression_term	: symbol			{ $$ = new BracketTransitionRange((char)$1, (char)$1); }
				| symbol '-' symbol	{ $$ = new BracketTransitionRange((char)$1, (char)$3); }
				;

/* symbol and number */
symbol	:	CHAR	{ $$ = $1; }
		|	DIGIT	{ $$ = $1; }
		;
		
number	:	DIGIT			{ $$ = (char)$1 - '0'; }
		|	number DIGIT	{ $$ = ((int)$1)*10 + (char)$2 - '0'; }
		;

%%

	Yylex lexer;
	bool reverseAutomate;
//	CultureInfo parseCulture;

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
//		this.parseCulture = parseCulture;
		yyparse(lexer);
		return Result;
	}
	public static Dfa Parse(string regExp, bool reverseAutomate, CultureInfo parseCulture) {
		return new RegExpParser().Parse(new StringReader(regExp), reverseAutomate, parseCulture);
	}
}