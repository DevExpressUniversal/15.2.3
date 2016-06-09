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
using System.Text;
using DevExpress.XtraRichEdit.Model;
using System.Globalization;
using DevExpress.XtraRichEdit.Native;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Fields;
namespace DevExpress.XtraRichEdit.Fields {
	public class FieldScanner {
		static readonly string[] SimpleTokens = new string[] {
			"IF", "COMPARE", "CREATEDATE", "DATE", "EDITTIME", "PRINTDATE", "SAVEDATE", "TIME",
			"DOCVARIABLE", "GOTOBUTTON", "MACROBUTTON", "PRINT", "ADVANCE", "SYMBOL", "INDEX", "RD", "TA", "TC", "TOA", "TOC", "XE",
			"AUTOTEXT", "AUTOTEXTLIST", "BIBLIOGRAPHY", "CITATION", "HYPERLINK",
			"INCLUDEPICTURE", "INCLUDETEXT", "LINK", "NOTEREF", "PAGEREF", "QUOTE", "REF", "STYLEREF",
			"ADDRESSBLOCK", "ASK", "DATABASE", "FILLIN", "GREETINGLINE",
			"MERGEFIELD", "MERGEREC", "MERGESEQ", "NEXT", "SET", "SKIPIF",
			"NEXTIF",
			"AUTONUM", "AUTONUMLGL", "AUTONUMOUT", "BARCODE", "LISTNUM", "PAGE", "REVNUM",
			"SECTION", "SECTIONPAGES", "SEQ",
			"USERADDRESS", "USERINITIALS", "USERNAME",
			"FORMCHECKBOX", "FORMDROPDOWN", "FORMTEXT" 
		};
		static readonly string[] DocPropertyInfoCommonTokens = new string[] {
			"AUTHOR", "COMMENTS", "KEYWORDS", "LASTSAVEDBY", "SUBJECT", "TEMPLATE", "TITLE"
		};
		static readonly string[] DocPropertyCategoryTokens = new string[] {
			"BYTES", "CATEGORY", "CHARACTERS", "CHARACTERSWITHSPACES",
			"COMPANY", "CREATETIME", "HYPERLINKBASE", "LASTPRINTED", "LASTSAVEDTIME",
			"LINES", "MANAGER", "NAMEOFAPPLICATION", "ODMADOCID", "PAGES", "PARAGRAPHS",
			"REVISIONNUMBER", "SECURITY", "TOTALEDITINGTIME", "WORDS"
		};
		const string DocPropertyToken = "DOCPROPERTY";
		static readonly string[] DocumentInformationTokens = new string[] {
			"FILENAME", "FILESIZE", "INFO", "NUMCHARS", "NUMPAGES", "NUMWORDS"
		};
		const string EqToken = "EQ";
		static readonly Dictionary<String, TokenKind> TokenDictionary;
		static FieldScanner() {
			TokenDictionary = new Dictionary<string, TokenKind>();
			AddTokensToDictionary(SimpleTokens, TokenKind.Simple);
			AddTokensToDictionary(DocPropertyInfoCommonTokens, TokenKind.DocPropertyInfoCommon);
			AddTokensToDictionary(DocPropertyCategoryTokens, TokenKind.DocPropertyCategory);
			AddTokenToDictionary(DocPropertyToken, TokenKind.DocProperty);
			AddTokensToDictionary(DocumentInformationTokens, TokenKind.DocumentInformation);
			AddTokenToDictionary(EqToken, TokenKind.Eq);
		}
		static void AddTokensToDictionary(string[] tokens, TokenKind kind) {
			int count = tokens.Length;
			for (int i = 0; i < count; i++)
				AddTokenToDictionary(tokens[i], kind);
		}
		static void AddTokenToDictionary(string token, TokenKind kind) {
			TokenDictionary.Add(token, kind);
		}
		Token currentScanToken;
		Token currentPeekToken;
		char separatorChar;
		char decimalSeparator;
		IFieldIterator iterator;
		ScannerStateBase state;
		readonly int maxFieldSwitchLength;
		readonly bool enableFieldNames;
		readonly bool supportFieldCommonStringFormat;
		public FieldScanner(IFieldIterator iterator, PieceTable pieceTable)
			: this(iterator, pieceTable.DocumentModel.MaxFieldSwitchLength, pieceTable.DocumentModel.EnableFieldNames, pieceTable.SupportFieldCommonStringFormat) {
		}
		public FieldScanner(IFieldIterator iterator, int maxFieldSwitchLength, bool enableFieldNames, bool supportFieldCommonStringFormat) {
			decimalSeparator = GetDecimalSeparator();
			separatorChar = GetSeparatorChar();
			this.iterator = iterator;
			this.state = new ScannerStateSimpleToken(this);
			this.maxFieldSwitchLength = maxFieldSwitchLength;
			this.enableFieldNames = enableFieldNames;
			this.supportFieldCommonStringFormat = supportFieldCommonStringFormat;
		}
		public IFieldIterator Iterator { get { return iterator; } }
		public bool SupportFieldCommonStringFormat { get { return supportFieldCommonStringFormat; } }
		public virtual char SeparatorChar { get { return separatorChar; } }
		public virtual char DecimalSeparator { get { return decimalSeparator; } }
		public ScannerStateBase State { get { return state; } }
		public int MaxFieldSwitchLength { get { return maxFieldSwitchLength; } }
		public bool EnableFieldNames { get { return enableFieldNames; } }
		protected virtual char GetSeparatorChar() {
			if (DecimalSeparator == ',')
				return ';';
			else
				return ',';
		}
		protected virtual char GetDecimalSeparator() {
			return CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
		}
		public Token Scan() {
			if (currentScanToken != null && currentScanToken.Next != null)
				currentScanToken = currentScanToken.Next;
			else
				currentScanToken = Peek();
			ResetPeek();
			return currentScanToken;
		}
		public void SwitchState(ScannerStateType stateType) {
			switch (stateType) {
				case ScannerStateType.EqInstructions:
					state = new ScannerStateEqInstructions(this);
					break;
				case ScannerStateType.EqSwitches:
					state = new ScannerStateEqSwitches(this);
					break;
				case ScannerStateType.Expression:
					state = new ScannerStateExpression(this);
					break;
				case ScannerStateType.Instructions:
					state = new ScannerStateInstructions(this);
					break;
				case ScannerStateType.Simple:
					state = new ScannerStateSimpleToken(this);
					break;
			}
		}
		public Token Peek() {
			if (currentPeekToken == null || currentPeekToken.Next == null) {
				Token token = PeekCore();
				if (token.ActualKind != TokenKind.Eof)
					SwitchState(state.GetNextState(token));
				if (currentPeekToken == null) {
					currentPeekToken = token;
					return token;
				}
				currentPeekToken.Next = token;
			}
			currentPeekToken = currentPeekToken.Next;
			return currentPeekToken;
		}
		private Token PeekCore() {
			SkipWhitespaces(true);
			if (Iterator.IsEnd())
				return new Token(TokenKind.Eof, Iterator.Position, String.Empty);
			char ch = Iterator.PeekNextChar();
			if (ch != '"')
				return ReadToken();
			else {
				Iterator.AdvanceNextChar();
				return ReadQuotedText();
			}
		}
		Token ReadToken() {
			return state.ReadNextToken();
		}
		Token ReadQuotedText() {
			Token result = new Token(TokenKind.QuotedText, Iterator.Position, String.Empty);
			StringBuilder val = new StringBuilder();
			char ch;
			DocumentLogPosition lastCharPosition = Iterator.Position;
			while (!Iterator.IsEnd() && (ch = Iterator.ReadNextChar()) != '"' ) {
				if (ch == '\\')
					ch = Iterator.ReadNextChar();
				val.Append(ch);
				lastCharPosition = Iterator.Position;
			}
			result.Value = val.ToString();
			result.Length = lastCharPosition - result.Position;
			return result;
		}
		public void SkipWhitespaces(bool skipNestedFields) {
			while (!Iterator.IsEnd() && IsWhiteSpace(Iterator.PeekNextChar()))
				Iterator.AdvanceNextChar(skipNestedFields);
		}
		public bool IsFieldStart() {
			SkipWhitespaces(false);
			return Iterator.IsFieldCodeStart();
		}
		public Token ScanEntireFieldToken() {
			DocumentLogPosition start = Iterator.Position;
			DocumentLogPosition end = start;
			do {
				Iterator.AdvanceNextChar();
				if(Iterator.IsFieldCodeEnd())
					end = Iterator.Position;					
			} while (!Iterator.IsEnd() && !Iterator.IsFieldEnd());
			return new Token(TokenKind.Template, start + 1, String.Empty, end - start - 1);
		}
		public virtual bool IsWhiteSpace(char ch) {
			return Char.IsWhiteSpace(ch);
		}
		public void ResetPeek() {
			currentPeekToken = currentScanToken;
		}
		public TokenKind GetTokenKindByVal(string strVal) {
			TokenKind result;
			if (TokenDictionary.TryGetValue(strVal, out result))
				return result;
			else
				return TokenKind.Invalid;
		}
		public virtual bool IsSwitchToken(TokenKind kind) {
			return kind == TokenKind.FieldSwitchCharacter ||
				kind == TokenKind.DateAndTimeFormattingSwitchBegin ||
				kind == TokenKind.NumbericFormattingSwitchBegin ||
				kind == TokenKind.CommonStringFormatSwitchBegin ||
				kind == TokenKind.GeneralFormattingSwitchBegin;
		}
		public bool IsValidFirstToken(Token token) {
			return TokenDictionary.ContainsKey(token.Value);
		}
	}
	public class ExpressionScanner {
		FieldScanner scanner;
		public ExpressionScanner(FieldScanner scanner) {
			this.scanner = scanner;
		}
		public Token Scan() {
			Token result = scanner.Scan();
			if (scanner.IsSwitchToken(result.ActualKind))
				return new Token(TokenKind.Eof, result.Position, String.Empty);
			else
				return result;
		}
		public Token Peek() {
			Token result = scanner.Peek();
			if (scanner.IsSwitchToken(result.ActualKind))
				return new Token(TokenKind.Eof, result.Position, String.Empty);
			else
				return result;
		}
		public void ResetPeek() {
			scanner.ResetPeek();
		}
	}
	public enum ScannerStateType {
		Simple,
		Expression,
		Instructions,
		EqSwitches,
		EqInstructions
	}
	public abstract class ScannerStateBase {
		readonly FieldScanner scanner;
		protected ScannerStateBase(FieldScanner scanner) {
			this.scanner = scanner;
		}
		public FieldScanner Scanner { get { return scanner; } }
		public abstract Token ReadNextToken();
		public IFieldIterator Iterator { get { return Scanner.Iterator; } }
		public abstract ScannerStateType StateType { get; }
		public abstract ScannerStateType GetNextState(Token token);
	}
	public class ScannerStateSimpleToken : ScannerStateBase {
		public ScannerStateSimpleToken(FieldScanner scanner)
			: base(scanner) {
		}
		public override ScannerStateType StateType { get { return ScannerStateType.Simple; } }
		public override Token ReadNextToken() {
			DocumentLogPosition start = Iterator.Position;
			if (Iterator.PeekNextChar() == '=') {
				Iterator.AdvanceNextChar();
				return new Token(TokenKind.OpEQ, start, "=", Iterator.Position - start);
			}
			StringBuilder val = new StringBuilder();
			bool firstChar = true;
			char ch;
			while (true) {
				ch = Iterator.PeekNextChar();
				if ((!firstChar && IsEndOfToken(ch)) || Iterator.IsEnd())
					break;
				firstChar = false;
				Iterator.AdvanceNextChar();
				val.Append(Char.ToUpperInvariant(ch));
			}
			string strVal = val.ToString();
			TokenKind kind = Scanner.GetTokenKindByVal(strVal);
			return new Token(kind, start, strVal, Iterator.Position - start);
		}
		bool IsEndOfToken(char ch) {			
			   return Scanner.IsWhiteSpace(ch) || ch == '\\';
		}
		public override ScannerStateType GetNextState(Token token) {
			switch (token.ActualKind) {
				case TokenKind.OpEQ:
					return ScannerStateType.Expression;
				case TokenKind.Eq:
					return ScannerStateType.EqSwitches;
				default:
					return ScannerStateType.Instructions;
			}
		}
	}
	public class ScannerStateExpression : ScannerStateBase {
		public ScannerStateExpression(FieldScanner scanner)
			: base(scanner) {
		}
		public override ScannerStateType StateType { get { return ScannerStateType.Expression; } }
		public override Token ReadNextToken() {
			StringBuilder val = new StringBuilder();
			DocumentLogPosition start = Iterator.Position;
			char ch = Iterator.PeekNextChar();
			if (IsSpecialChar(ch))
				return ReadSpecialCharToken();
			if (IsConstantChar(ch))
				return ReadConstantToken();
			if (ch == '%') {
				Iterator.AdvanceNextChar();
				return new Token(TokenKind.Percent, start, "%", Iterator.Position - start);
			}
			if (IsSwitchStart(ch)) {
				Iterator.AdvanceNextChar();
				return ReadSwitchToken(start);
			}
			bool fieldNameStarted = false;
			int openBracketCount = 0;
			if (Scanner.EnableFieldNames) {
				fieldNameStarted = Iterator.PeekNextChar() == '[';
			}
			while (true) {
				ch = Iterator.PeekNextChar();
				if ((!fieldNameStarted && IsEndOfToken(ch)) || (fieldNameStarted && ch == '\0'))
					break;
				Iterator.AdvanceNextChar();
				val.Append(ch);
				if (fieldNameStarted) {
					if (ch == ']') {
						openBracketCount--;
						if (openBracketCount == 0)
							break;
					}
					if (ch == '[')
						openBracketCount++;
				}
			}
			string strVal = val.ToString();			
			int length = Iterator.Position - start;
			if (Scanner.IsWhiteSpace(ch)) {
				Scanner.SkipWhitespaces(true);
			}
			TokenKind tokenKind = (!Iterator.IsEnd() && Iterator.PeekNextChar() == '(') ? TokenKind.FunctionName : TokenKind.Text;
			return new Token(tokenKind, start, strVal, length);
		}
		public override ScannerStateType GetNextState(Token token) {
			if (Scanner.IsSwitchToken(token.ActualKind))
				return ScannerStateType.Instructions;
			else
				return StateType;
		}
		bool IsEndOfToken(char ch) {
			return IsSpecialChar(ch) || ch == '\0' || Scanner.IsWhiteSpace(ch);
		}
		bool IsSpecialChar(char ch) {
			return ch == '+' || ch == '-' || ch == '*' || ch == '/' || ch == '^' || ch == '(' || ch == ')'
				|| ch == '=' || ch == '<' || ch == '>' || ch == Scanner.SeparatorChar;
		}
		bool IsConstantChar(char ch) {
			return (ch >= '0' && ch <= '9') || ch == Scanner.DecimalSeparator;
		}
		Token ReadConstantToken() {
			DocumentLogPosition start = Iterator.Position;
			StringBuilder val = new StringBuilder();
			while (true) {
				char ch = Iterator.PeekNextChar();
				if (!IsConstantChar(ch))
					break;
				Iterator.AdvanceNextChar();
				val.Append(Char.ToUpperInvariant(ch));
			}
			string strVal = val.ToString();
			return new Token(TokenKind.Constant, start, strVal, Iterator.Position - start);
		}
		private Token ReadSpecialCharToken() {
			DocumentLogPosition start = Iterator.Position;
			char ch = Iterator.ReadNextChar();
			char nextChar = Iterator.PeekNextChar();
			switch (ch) {
				case '=':
					return new Token(TokenKind.OpEQ, start, "=", Iterator.Position - start);
				case '<':
					switch (nextChar) {
						case '>':
							Iterator.AdvanceNextChar();
							return new Token(TokenKind.OpNEQ, start, "<>", Iterator.Position - start);
						case '=':
							Iterator.AdvanceNextChar();
							return new Token(TokenKind.OpLOWEQ, start, "<=", Iterator.Position - start);
						default:
							return new Token(TokenKind.OpLOW, start, "<", Iterator.Position - start);
					}
				case '>':
					if (nextChar == '=') {
						Iterator.AdvanceNextChar();
						return new Token(TokenKind.OpHIEQ, start, ">=", Iterator.Position - start);
					}
					else
						return new Token(TokenKind.OpHI, start, ">", Iterator.Position - start);
				case '+':
					return new Token(TokenKind.OpPLUS, start, "+", Iterator.Position - start);
				case '-':
					return new Token(TokenKind.OpMINUS, start, "-", Iterator.Position - start);
				case '*':
					return new Token(TokenKind.OpMUL, start, "*", Iterator.Position - start);
				case '/':
					return new Token(TokenKind.OpDIV, start, "/", Iterator.Position - start);
				case '^':
					return new Token(TokenKind.OpPOW, start, "^", Iterator.Position - start);
				case '(':
					return new Token(TokenKind.OpenParenthesis, start, "(", Iterator.Position - start);
				case ')':
					return new Token(TokenKind.CloseParenthesis, start, ")", Iterator.Position - start);
				default:
					if (ch == Scanner.SeparatorChar)
						return new Token(TokenKind.SeparatorChar, start, new String(Scanner.SeparatorChar, 1), Iterator.Position - start);
					else
						return new Token(TokenKind.Invalid, start, String.Empty);
			}
		}
		bool IsSwitchStart(char ch) {
			return ch == '\\';
		}
		Token ReadSwitchToken(DocumentLogPosition start) {
			char ch = Iterator.PeekNextChar();			
			switch (ch) {
				case '@':
					Iterator.AdvanceNextChar();
					return new Token(TokenKind.DateAndTimeFormattingSwitchBegin, start, "\\@", Iterator.Position - start);
				case '#':
					Iterator.AdvanceNextChar();
					return new Token(TokenKind.NumbericFormattingSwitchBegin, start, "\\#", Iterator.Position - start);
				case '*':
					Iterator.AdvanceNextChar();
					return new Token(TokenKind.GeneralFormattingSwitchBegin, start, "\\*", Iterator.Position - start);
				case '!':
					Iterator.AdvanceNextChar();
					return new Token(TokenKind.FieldSwitchCharacter, start, "\\!", Iterator.Position - start);
				case '$':
					if (Scanner.SupportFieldCommonStringFormat) {
						Iterator.AdvanceNextChar();
						return new Token(TokenKind.CommonStringFormatSwitchBegin, start, "\\$", Iterator.Position - start);
					}
					else
						goto default;
				default:
					return ReadFieldSwitchCharacter(start);
			}
		}
		Token ReadFieldSwitchCharacter(DocumentLogPosition start) {
			string val = "\\";
			int maxResultLength = Scanner.MaxFieldSwitchLength + 1;
			while (val.Length < maxResultLength) {
				char ch = Char.ToLowerInvariant(Iterator.PeekNextChar());
				if (!IsLatinLetter(ch))
					break;
				val += ch;
				Iterator.AdvanceNextChar();
			}
			return new Token(TokenKind.FieldSwitchCharacter, start, val, Iterator.Position - start);
		}
		bool IsLatinLetter(char ch) {
			return ch >= 'a' && ch <= 'z';
		}
	}
	public class ScannerStateInstructions : ScannerStateBase {
		public ScannerStateInstructions(FieldScanner scanner)
			: base(scanner) {
		}
		public override ScannerStateType StateType { get { return ScannerStateType.Instructions; } }
		public override Token ReadNextToken() {
			DocumentLogPosition start = Iterator.Position;
			StringBuilder val = new StringBuilder();
			char ch = Iterator.PeekNextChar();
			if (IsSwitchStart(ch)) {
				Iterator.AdvanceNextChar();
				return ReadSwitchToken(start);
			}
			while (true) {
				ch = Iterator.PeekNextChar();
				if (Iterator.IsEnd() || Scanner.IsWhiteSpace(ch) || IsSwitchStart(ch))
					break;
				Iterator.AdvanceNextChar();
				val.Append(ch);
			}
			string strVal = val.ToString();
			return new Token(TokenKind.Text, start, strVal, Iterator.Position - start);
		}
		public override ScannerStateType GetNextState(Token token) {
			return this.StateType;
		}
		protected bool IsSwitchStart(char ch) {
			return ch == '\\';
		}
		protected Token ReadSwitchToken(DocumentLogPosition start) {
			char ch = Iterator.PeekNextChar();
			switch (ch) {
				case '@':
					Iterator.AdvanceNextChar();
					return new Token(TokenKind.DateAndTimeFormattingSwitchBegin, start, "\\@", Iterator.Position - start);
				case '#':
					Iterator.AdvanceNextChar();
					return new Token(TokenKind.NumbericFormattingSwitchBegin, start, "\\#", Iterator.Position - start);
				case '*':
					Iterator.AdvanceNextChar();
					return new Token(TokenKind.GeneralFormattingSwitchBegin, start, "\\*", Iterator.Position - start);
				case '!':
					Iterator.AdvanceNextChar();
					return new Token(TokenKind.FieldSwitchCharacter, start, "\\!", Iterator.Position - start);
				case '$':
					if (Scanner.SupportFieldCommonStringFormat) {
						Iterator.AdvanceNextChar();
						return new Token(TokenKind.CommonStringFormatSwitchBegin, start, "\\$", Iterator.Position - start);
					}
					else goto default;
				default:
					return ReadFieldSwitchCharacter(start);
			}
		}
		protected Token ReadFieldSwitchCharacter(DocumentLogPosition start) {			
			string val = "\\";	
			int maxResultLength = Scanner.MaxFieldSwitchLength + 1;
			while (val.Length < maxResultLength) {
				char ch = Char.ToLowerInvariant(Iterator.PeekNextChar());
				if(!IsLatinLetter(ch))
					break;
				val += ch;
				Iterator.AdvanceNextChar();
			}
			return new Token(TokenKind.FieldSwitchCharacter, start, val, Iterator.Position - start);
		}
		bool IsLatinLetter(char ch) {
			return ch >= 'a' && ch <= 'z';
		}
	}
	public class ScannerStateEqSwitches : ScannerStateInstructions {
		public ScannerStateEqSwitches(FieldScanner scanner)
			: base(scanner) {
		}
		public override ScannerStateType StateType { get { return ScannerStateType.EqSwitches; } }
		public override Token ReadNextToken() {
			DocumentLogPosition start = Iterator.Position;
			StringBuilder val = new StringBuilder();
			char ch = Iterator.PeekNextChar();
			if (ch == '(') {
				Iterator.AdvanceNextChar();
				return new Token(TokenKind.OpenParenthesis, start, "(", Iterator.Position - start);
			}
			if (IsSwitchStart(ch)) {
				Iterator.AdvanceNextChar();
				return ReadSwitchToken(start);
			}
			while (true) {
				ch = Iterator.PeekNextChar();
				if (Iterator.IsEnd() || Scanner.IsWhiteSpace(ch))
					break;
				Iterator.AdvanceNextChar();
				val.Append(ch);
			}
			string strVal = val.ToString();
			return new Token(TokenKind.Text, start, strVal, Iterator.Position - start);
		}
		public override ScannerStateType GetNextState(Token token) {
			if (token.ActualKind == TokenKind.OpenParenthesis)
				return ScannerStateType.EqInstructions;
			else
				return this.StateType;
		}
	}
	public class ScannerStateEqInstructions : ScannerStateBase {
		public ScannerStateEqInstructions(FieldScanner scanner)
			: base(scanner) {
		}
		public override ScannerStateType StateType { get { return ScannerStateType.EqInstructions; } }
		public override Token ReadNextToken() {
			DocumentLogPosition start = Iterator.Position;
			StringBuilder val = new StringBuilder();
			char ch = Iterator.PeekNextChar();
			if (ch == ')') {
				Iterator.AdvanceNextChar();
				return new Token(TokenKind.CloseParenthesis, start, ")", Iterator.Position - start);
			}
			if (ch == Scanner.SeparatorChar) {
				Iterator.AdvanceNextChar();
				return new Token(TokenKind.SeparatorChar, start, new String(ch, 1), Iterator.Position - start);
			}
			while (true) {
				ch = Iterator.PeekNextChar();
				if (Iterator.IsEnd() || IsEndOfToken(ch))
					break;
				Iterator.AdvanceNextChar();
				val.Append(ch);
			}
			string strVal = val.ToString();
			return new Token(TokenKind.Text, start, strVal, Iterator.Position - start);
		}
		public override ScannerStateType GetNextState(Token token) {
			if (token.ActualKind == TokenKind.CloseParenthesis)
				return ScannerStateType.EqSwitches;
			else
				return this.StateType;
		}
		bool IsEndOfToken(char ch) {
			return ch == ')' || ch == Scanner.SeparatorChar;
		}
	}
}
