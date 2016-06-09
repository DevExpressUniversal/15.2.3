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
using System.Globalization;
using System.IO;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Exceptions;
using System.Text;
namespace DevExpress.Data.Filtering.Helpers {
	public class CriteriaLexer : yyInput {
		protected readonly TextReader InputReader;
		public int CurrentToken = 0;
		public object CurrentValue = null;
		bool isAfterColumn = false;
		int _line = -1;
		int _col = -1;
		int _currentLine = 0;
		int _currentCol = 0;
		int _pos = 0;
		int _currentTokenPos = -1;
		public int Line { get { return _line; } }
		public int Col { get { return _col; } }
		public int Position { get { return _pos; } }
		public int CurrentTokenPosition { get { return _currentTokenPos; } }
		bool yyInput.advance() {
			return this.Advance();
		}
		int yyInput.token() {
			return CurrentToken;
		}
		object yyInput.value() {
			return CurrentValue;
		}
		public CriteriaLexer(TextReader inputReader) {
			this.InputReader = inputReader;
		}
		public bool Advance() {
			SkipBlanks();
			_line = _currentLine;
			_col = _currentCol;
			_currentTokenPos = _pos;
			CurrentToken = 0;
			CurrentValue = null;
			int nextInt = ReadNextChar();
			if(nextInt == -1) {
				return false;
			}
			char nextChar = (char)nextInt;
			switch(nextChar) {
				case '?':
					DoParam();
					break;
				case '^':
				case '+':
				case '*':
				case '/':
				case '%':
				case '(':
				case ')':
				case ']':
				case ',':
				case '~':
				case '-':
				case ';':
					this.CurrentToken = nextChar;
					break;
				case '.':
					DoDotOrNumber();
					break;
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					DoNumber(nextChar);
					break;
				case '!':
					if(PeekNextChar() == '=') {
						ReadNextChar();
						this.CurrentToken = Token.OP_NE;
					} else {
						this.CurrentToken = Token.NOT;
					}
					break;
				case '=':
					this.CurrentToken = Token.OP_EQ;
					if(PeekNextChar() == '=') {
						ReadNextChar();
					}
					break;
				case '<':
					if(PeekNextChar() == '>') {
						ReadNextChar();
						this.CurrentToken = Token.OP_NE;
					} else if(PeekNextChar() == '=') {
						ReadNextChar();
						this.CurrentToken = Token.OP_LE;
					} else {
						this.CurrentToken = Token.OP_LT;
					}
					break;
				case '>':
					if(PeekNextChar() == '=') {
						ReadNextChar();
						this.CurrentToken = Token.OP_GE;
					} else {
						this.CurrentToken = Token.OP_GT;
					}
					break;
				case '|':
					if(PeekNextChar() == '|') {
						ReadNextChar();
						this.CurrentToken = Token.OR;
					} else {
						this.CurrentToken = nextChar;
					}
					break;
				case '&':
					if(PeekNextChar() == '&') {
						ReadNextChar();
						this.CurrentToken = Token.AND;
					} else {
						this.CurrentToken = nextChar;
					}
					break;
				case '[':
					if(isAfterColumn) {
						this.CurrentToken = nextChar;
					} else {
						DoEnclosedColumn();
					}
					break;
				case '{':
					DoConstGuid();
					break;
				case '\'':
					DoString();
					break;
				case '@':
					DoAtColumn();
					break;
				case '#':
					if(PeekNextChar() == '#') {
						ReadNextChar();
						DoUserObject();
					} else {
						DoDateTimeConst();
					}
					break;
				default:
					CatchAll(nextChar);
					break;
			}
			isAfterColumn = this.CurrentToken == Token.COL;
			return true;
		}
		char wasChar = '\0';
		protected int ReadNextChar() {
			int nextInt = InputReader.Read();
			if(nextInt == -1) {
				wasChar = '\0';
			} else if(nextInt == '\n') {
				if(wasChar == '\r') {
					wasChar = '\0';
					++_pos;
				} else {
					wasChar = '\n';
					++_pos;
					++_currentLine;
					_currentCol = 0;
				}
			} else if(nextInt == '\r') {
				if(wasChar == '\n') {
					wasChar = '\0';
					++_pos;
				} else {
					wasChar = '\r';
					++_pos;
					++_currentLine;
					_currentCol = 0;
				}
			} else {
				++_pos;
				++_currentCol;
			}
			return nextInt;
		}
		protected int PeekNextChar() {
			return InputReader.Peek();
		}
		public void SkipBlanks() {
			for(; ; ) {
				int peeked = PeekNextChar();
				UnicodeCategory peekedCategory = CharUnicodeInfo.GetUnicodeCategory((char)peeked);
				if(peekedCategory != UnicodeCategory.SpaceSeparator && peekedCategory != UnicodeCategory.Control)
					return;
				ReadNextChar();
			}
		}
		void DoAtColumn() {
			string columnName = string.Empty;
			for(; ; ) {
				if(CanContinueColumn((char)PeekNextChar())) {
					columnName += (char)ReadNextChar();
				} else
					break;
			}
			this.CurrentToken = Token.COL;
			this.CurrentValue = new OperandProperty(columnName);
		}
		void DoParam() {
			string paramName = null;
			for(; ; ) {
				if(CanContinueColumn((char)PeekNextChar())) {
					paramName += (char)ReadNextChar();
				} else
					break;
			}
			this.CurrentToken = Token.PARAM;
			this.CurrentValue = paramName;
		}
		void DoEnclosedColumn() {
			string name = string.Empty;
			this.CurrentToken = Token.COL;
			try {
				for(; ; ) {
					int nextInt = ReadNextChar();
					if(nextInt == -1) {
						YYError(FilteringExceptionsText.LexerNonClosedElement, FilteringExceptionsText.LexerElementPropertyName, "]");
						return;
					}
					char nextChar = (char)nextInt;
					if(nextChar == ']') {
						return;
					}
					if(nextChar == '\\') {
						nextInt = ReadNextChar();
						if(nextInt == -1) {
							YYError(FilteringExceptionsText.LexerNonClosedElement, FilteringExceptionsText.LexerElementPropertyName, "]");
							return;
						}
						nextChar = (char)nextInt;
						switch(nextChar) {
							case 'n':
								name += '\n';
								break;
							case 'r':
								name += '\r';
								break;
							case 't':
								name += '\t';
								break;
							default:
								name += nextChar;
								break;
						}
					} else {
						name += nextChar;
					}
				}
			} finally {
				this.CurrentValue = new OperandProperty(name);
			}
		}
		void DoString() {
			this.CurrentToken = Token.CONST;
			string str = string.Empty;
			for(; ; ) {
				int nextInt = ReadNextChar();
				if(nextInt == -1) {
					this.CurrentValue = new ConstantValue(str);
					YYError(FilteringExceptionsText.LexerNonClosedElement, FilteringExceptionsText.LexerElementStringLiteral, "'");
					return;
				}
				char nextChar = (char)nextInt;
				if(nextChar == '\'') {
					if(PeekNextChar() != '\'') {
						this.CurrentValue = new ConstantValue(str);
						if(str.Length == 1) {
							int possibleSuffix = PeekNextChar();
							if(possibleSuffix == 'c' || possibleSuffix == 'C') {
								ReadNextChar();
								this.CurrentValue = new ConstantValue(str[0]);
							}
						}
						return;
					}
					ReadNextChar();
				}
				str += nextChar;
			}
		}
		string ReadToLoneSharp() {
			StringBuilder str = new StringBuilder();
			for(; ; ) {
				int nextInt = ReadNextChar();
				if(nextInt == -1) {
					this.CurrentValue = new ConstantValue(str);
					YYError(FilteringExceptionsText.LexerNonClosedElement, FilteringExceptionsText.LexerElementDateTimeOrUserTypeLiteral, "#");
					return str.ToString();
				}
				char nextChar = (char)nextInt;
				if(nextChar == '#') {
					int peek = PeekNextChar();
					if(peek == '#')
						ReadNextChar();
					else
						break;
				}
				str.Append(nextChar);
			}
			return str.ToString();
		}
		void DoUserObject() {
			this.CurrentToken = Token.CONST;
			this.CurrentValue = new ConstantValue();
			string tag = ReadToLoneSharp();
			string data = ReadToLoneSharp();
			try {
				this.CurrentValue = new ConstantValue(ExtractUserValue(tag, data));
			} catch(Exception e) {
				this.CurrentValue = new ConstantValue(tag.Replace("#", "##") + "#" + data.Replace("#", "##"));
				YYError("Can't restore user object. Tag '{0}', data '{1}', Exception: '{2}'", tag, data, e);
			}
		}
		static object ExtractUserValue(string tag, string data) {
			UserValueProcessingEventArgs e = CriteriaOperator.DoUserValueParse(tag, data);
			EnumProcessingHelper.ExtractEnumIfNeeded(e);
			if(e.Handled)
				return e.Value;
			if(e.Tag == CriteriaOperator.TagToString)
				return data;
			throw new InvalidOperationException("'" + tag + "' tag is not handled, data is '" + data + "'");
		}
		void DoDateTimeConst() {
			this.CurrentToken = Token.CONST;
			string str = ReadToLoneSharp();
#if DXWhidbey && !CF
			TimeSpan ts;
			if(TimeSpan.TryParse(str, out ts)) {
				this.CurrentValue = new ConstantValue(ts);
				return;
			}
			DateTime dt;
			if(DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dt)) {
				this.CurrentValue = new ConstantValue(dt);
				return;
			}
#else
			try {
				this.CurrentValue = new ConstantValue(TimeSpan.Parse(str));
				return;
			} catch { }
			try {
				this.CurrentValue = new ConstantValue(DateTime.Parse(str, CultureInfo.InvariantCulture));
				return;
			} catch { }
#endif
			this.CurrentValue = new ConstantValue(str);
			YYError(FilteringExceptionsText.LexerInvalidElement, FilteringExceptionsText.LexerElementDateTimeLiteral, str);
		}
		void DoConstGuid() {
			this.CurrentToken = Token.CONST;
			string str = string.Empty;
			for(; ; ) {
				int nextInt = ReadNextChar();
				if(nextInt == -1) {
					this.CurrentValue = new ConstantValue(str);
					YYError(FilteringExceptionsText.LexerNonClosedElement, FilteringExceptionsText.LexerElementGuidLiteral, "}");
					return;
				}
				char nextChar = (char)nextInt;
				if(nextChar == '}')
					break;
				str += nextChar;
			}
			try {
				this.CurrentValue = new ConstantValue(new Guid(str));
				return;
			} catch { }
			this.CurrentValue = new ConstantValue(str);
			YYError(FilteringExceptionsText.LexerInvalidElement, FilteringExceptionsText.LexerElementGuidLiteral, str);
		}
		void CatchAll(char firstChar) {
			string str = string.Empty;
			str += firstChar;
			if(!CanStartColumn(firstChar)) {
				this.CurrentToken = Token.yyErrorCode;
				this.CurrentValue = firstChar;
				YYError(FilteringExceptionsText.LexerInvalidInputCharacter, str);
				return;
			}
			for(; ; ) {
				int nextInt = PeekNextChar();
				if(nextInt == -1)
					break;
				char nextChar = (char)nextInt;
				if(!CanContinueColumn(nextChar))
					break;
				ReadNextChar();
				str += nextChar;
			}
			ToTokenAndValue(str, out this.CurrentToken, out this.CurrentValue, RecogniseSortings);
		}
		internal bool RecogniseSortings;
		static void ToTokenAndValue(string str, out int currentToken, out object currentValue) {
			ToTokenAndValue(str, out currentToken, out currentValue, false);
		}
		static void ToTokenAndValue(string str, out int currentToken, out object currentValue, bool allowSortings) {
			currentValue = null;
			switch(str.ToUpperInvariant()) {
				case "AND":
					currentToken = Token.AND;
					break;
				case "OR":
					currentToken = Token.OR;
					break;
				case "TRUE":
					currentToken = Token.CONST;
					currentValue = new ConstantValue(true);
					break;
				case "FALSE":
					currentToken = Token.CONST;
					currentValue = new ConstantValue(false);
					break;
				case "NOT":
					currentToken = Token.NOT;
					break;
				case "IS":
					currentToken = Token.IS;
					break;
				case "NULL":
					currentToken = Token.NULL;
					break;
				case "LIKE":
					currentToken = Token.OP_LIKE;
					break;
				case "ISNULL":
					currentToken = Token.FN_ISNULL;
					break;
				case "ISNULLOREMPTY":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.IsNullOrEmpty;
					break;
				case "TRIM":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Trim;
					break;
				case "LEN":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Len;
					break;
				case "SUBSTRING":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Substring;
					break;
				case "UPPER":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Upper;
					break;
				case "LOWER":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Lower;
					break;
				case "CUSTOM":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Custom;
					break;
				case "CUSTOMNONDETERMINISTIC":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.CustomNonDeterministic;
					break;
				case "CONCAT":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Concat;
					break;
				case "IIF":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Iif;
					break;
				case "ABS":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Abs;
					break;
				case "ACOS":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Acos;
					break;
				case "ADDDAYS":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.AddDays;
					break;
				case "ADDHOURS":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.AddHours;
					break;
				case "ADDMILLISECONDS":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.AddMilliSeconds;
					break;
				case "ADDMINUTES":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.AddMinutes;
					break;
				case "ADDMONTHS":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.AddMonths;
					break;
				case "ADDSECONDS":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.AddSeconds;
					break;
				case "ADDTICKS":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.AddTicks;
					break;
				case "ADDTIMESPAN":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.AddTimeSpan;
					break;
				case "ADDYEARS":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.AddYears;
					break;
				case "ASCII":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Ascii;
					break;
				case "ASIN":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Asin;
					break;
				case "ATN":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Atn;
					break;
				case "ATN2":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Atn2;
					break;
				case "BIGMUL":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.BigMul;
					break;
				case "CEILING":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Ceiling;
					break;
				case "CHAR":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Char;
					break;
				case "CHARINDEX":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.CharIndex;
					break;
				case "COS":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Cos;
					break;
				case "COSH":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Cosh;
					break;
				case "EXP":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Exp;
					break;
				case "FLOOR":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Floor;
					break;
				case "GETDATE":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.GetDate;
					break;
				case "GETDAY":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.GetDay;
					break;
				case "GETDAYOFWEEK":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.GetDayOfWeek;
					break;
				case "GETDAYOFYEAR":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.GetDayOfYear;
					break;
				case "GETHOUR":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.GetHour;
					break;
				case "GETMILLISECOND":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.GetMilliSecond;
					break;
				case "GETMINUTE":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.GetMinute;
					break;
				case "GETMONTH":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.GetMonth;
					break;
				case "GETSECOND":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.GetSecond;
					break;
				case "GETTIMEOFDAY":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.GetTimeOfDay;
					break;
				case "GETYEAR":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.GetYear;
					break;
				case "DATEDIFFDAY":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.DateDiffDay;
					break;
				case "DATEDIFFHOUR":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.DateDiffHour;
					break;
				case "DATEDIFFMILLISECOND":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.DateDiffMilliSecond;
					break;
				case "DATEDIFFMINUTE":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.DateDiffMinute;
					break;
				case "DATEDIFFMONTH":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.DateDiffMonth;
					break;
				case "DATEDIFFSECOND":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.DateDiffSecond;
					break;
				case "DATEDIFFTICK":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.DateDiffTick;
					break;
				case "DATEDIFFYEAR":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.DateDiffYear;
					break;
				case "LOG":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Log;
					break;
				case "LOG10":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Log10;
					break;
				case "NOW":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Now;
					break;
				case "UTCNOW":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.UtcNow;
					break;
				case "PADLEFT":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.PadLeft;
					break;
				case "PADRIGHT":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.PadRight;
					break;
				case "POWER":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Power;
					break;
				case "REMOVE":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Remove;
					break;
				case "REPLACE":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Replace;
					break;
				case "REVERSE":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Reverse;
					break;
				case "RND":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Rnd;
					break;
				case "ROUND":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Round;
					break;
				case "SIGN":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Sign;
					break;
				case "SIN":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Sin;
					break;
				case "SINH":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Sinh;
					break;
				case "SQR":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Sqr;
					break;
				case "TOSTR":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.ToStr;
					break;
				case "INSERT":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Insert;
					break;
				case "TAN":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Tan;
					break;
				case "TANH":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Tanh;
					break;
				case "TODAY":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Today;
					break;
				case "TOINT":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.ToInt;
					break;
				case "TOLONG":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.ToLong;
					break;
				case "TOFLOAT":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.ToFloat;
					break;
				case "TODOUBLE":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.ToDouble;
					break;
				case "TODECIMAL":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.ToDecimal;
					break;
				case "STARTSWITH":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.StartsWith;
					break;
				case "ENDSWITH":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.EndsWith;
					break;
				case "CONTAINS":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.Contains;
					break;			   
				case "BETWEEN":
					currentToken = Token.OP_BETWEEN;
					break;
				case "IN":
					currentToken = Token.OP_IN;
					break;
				case "EXISTS":
					currentToken = Token.AGG_EXISTS;
					break;
				case "COUNT":
					currentToken = Token.AGG_COUNT;
					break;
				case "MIN":
					currentToken = Token.AGG_MIN;
					break;
				case "MAX":
					currentToken = Token.AGG_MAX;
					break;
				case "SINGLE":
					currentToken = Token.AGG_SINGLE;
					break;
				case "AVG":
					currentToken = Token.AGG_AVG;
					break;
				case "SUM":
					currentToken = Token.AGG_SUM;
					break;
				case "LOCALDATETIMETHISYEAR":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.LocalDateTimeThisYear;
					break;
				case "LOCALDATETIMETHISMONTH":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.LocalDateTimeThisMonth;
					break;
				case "LOCALDATETIMELASTWEEK":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.LocalDateTimeLastWeek;
					break;
				case "LOCALDATETIMETHISWEEK":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.LocalDateTimeThisWeek;
					break;
				case "LOCALDATETIMEYESTERDAY":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.LocalDateTimeYesterday;
					break;
				case "LOCALDATETIMETODAY":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.LocalDateTimeToday;
					break;
				case "LOCALDATETIMENOW":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.LocalDateTimeNow;
					break;
				case "LOCALDATETIMETOMORROW":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.LocalDateTimeTomorrow;
					break;
				case "LOCALDATETIMEDAYAFTERTOMORROW":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.LocalDateTimeDayAfterTomorrow;
					break;
				case "LOCALDATETIMENEXTWEEK":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.LocalDateTimeNextWeek;
					break;
				case "LOCALDATETIMETWOWEEKSAWAY":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.LocalDateTimeTwoWeeksAway;
					break;
				case "LOCALDATETIMENEXTMONTH":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.LocalDateTimeNextMonth;
					break;
				case "LOCALDATETIMENEXTYEAR":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.LocalDateTimeNextYear;
					break;
				case "ISOUTLOOKINTERVALBEYONDTHISYEAR":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.IsOutlookIntervalBeyondThisYear;
					break;
				case "ISOUTLOOKINTERVALLATERTHISYEAR":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.IsOutlookIntervalLaterThisYear;
					break;
				case "ISOUTLOOKINTERVALLATERTHISMONTH":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.IsOutlookIntervalLaterThisMonth;
					break;
				case "ISOUTLOOKINTERVALNEXTWEEK":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.IsOutlookIntervalNextWeek;
					break;
				case "ISOUTLOOKINTERVALLATERTHISWEEK":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.IsOutlookIntervalLaterThisWeek;
					break;
				case "ISOUTLOOKINTERVALTOMORROW":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.IsOutlookIntervalTomorrow;
					break;
				case "ISOUTLOOKINTERVALTODAY":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.IsOutlookIntervalToday;
					break;
				case "ISOUTLOOKINTERVALYESTERDAY":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.IsOutlookIntervalYesterday;
					break;
				case "ISOUTLOOKINTERVALEARLIERTHISWEEK":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.IsOutlookIntervalEarlierThisWeek;
					break;
				case "ISOUTLOOKINTERVALLASTWEEK":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.IsOutlookIntervalLastWeek;
					break;
				case "ISOUTLOOKINTERVALEARLIERTHISMONTH":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.IsOutlookIntervalEarlierThisMonth;
					break;
				case "ISOUTLOOKINTERVALEARLIERTHISYEAR":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.IsOutlookIntervalEarlierThisYear;
					break;
				case "ISOUTLOOKINTERVALPRIORTHISYEAR":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.IsOutlookIntervalPriorThisYear;
					break;
				case "ISTHISWEEK":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.IsThisWeek;
					break;
				case "ISTHISMONTH":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.IsThisMonth;
					break;
				case "ISTHISYEAR":
					currentToken = Token.FUNCTION;
					currentValue = FunctionOperatorType.IsThisYear;
					break;
				case "ASC":
				case "ASCENDING":
					if(allowSortings) {
						currentToken = Token.SORT_ASC;
						break;
					} else
						goto default;
				case "DESC":
				case "DESCENDING":
					if(allowSortings) {
						currentToken = Token.SORT_DESC;
						break;
					} else
						goto default;
				default:
					currentToken = Token.COL;
					currentValue = new OperandProperty(str);
					break;
			}
		}
		void DoNumber(char firstSymbol) {
			string str = string.Empty;
			str += firstSymbol;
			for(; ; ) {
				int nextInt = PeekNextChar();
				char nextChar = (char)nextInt;
				switch(nextChar) {
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
					case '.':
						ReadNextChar();
						str += nextChar;
						break;
					case 'e':
					case 'E':
						ReadNextChar();
						str += nextChar;
						nextInt = ReadNextChar();
						if(nextInt == -1) {
							YYError(FilteringExceptionsText.LexerInvalidElement, FilteringExceptionsText.LexerElementNumberLiteral, str);
							break;
						}
						nextChar = (char)nextInt;
						str += nextChar;
						break;
					default:
						this.CurrentToken = Token.CONST;
						string numericCode = GetNumericCode();
						try {
							this.CurrentValue = new ConstantValue(ExtractNumericValue(str, numericCode));
						} catch {
							this.CurrentValue = new ConstantValue(str + numericCode);
							YYError(FilteringExceptionsText.LexerInvalidElement, FilteringExceptionsText.LexerElementNumberLiteral, str + numericCode);
						}
						return;
				}
			}
		}
		object ExtractNumericValue(string str, string numericCode) {
			switch(numericCode.ToLowerInvariant()) {
				case "m":
					return Convert.ToDecimal(str, CultureInfo.InvariantCulture);
				case "f":
					return Convert.ToSingle(str, CultureInfo.InvariantCulture);
				case "i":
					return Convert.ToInt32(str, CultureInfo.InvariantCulture);
				case "s":
					return Convert.ToInt16(str, CultureInfo.InvariantCulture);
				case "l":
					return Convert.ToInt64(str, CultureInfo.InvariantCulture);
				case "b":
					return Convert.ToByte(str, CultureInfo.InvariantCulture);
				case "u":
				case "ui":
				case "iu":
					return Convert.ToUInt32(str, CultureInfo.InvariantCulture);
				case "sb":
				case "bs":
					return Convert.ToSByte(str, CultureInfo.InvariantCulture);
				case "us":
				case "su":
					return Convert.ToUInt16(str, CultureInfo.InvariantCulture);
				case "ul":
				case "lu":
					return Convert.ToUInt64(str, CultureInfo.InvariantCulture);
				default:
					throw new InvalidOperationException("invalid type code");
				case "":
					if(str.IndexOfAny(new char[] { '.', 'e', 'E' }) >= 0)
						return Convert.ToDouble(str, CultureInfo.InvariantCulture);
					try {
						return Convert.ToInt32(str, CultureInfo.InvariantCulture);
					} catch { }
					try {
						return Convert.ToInt64(str, CultureInfo.InvariantCulture);
					} catch { }
					return Convert.ToDouble(str, CultureInfo.InvariantCulture);
			}
		}
		string GetNumericCode() {
			int peeked = PeekNextChar();
			if(peeked == -1)
				return string.Empty;
			char ch = (char)peeked;
			switch(ch) {
				default:
					return string.Empty;
				case 'm':
				case 'M':
				case 'f':
				case 'F':
					ReadNextChar();
					return ch.ToString();
				case 'b':
				case 's':
				case 'i':
				case 'l':
				case 'u':
				case 'B':
				case 'S':
				case 'I':
				case 'L':
				case 'U':
					break;
			}
			ReadNextChar();
			peeked = PeekNextChar();
			if(peeked != -1) {
				char ch2 = (char)peeked;
				switch(ch2) {
					case 'b':
					case 's':
					case 'i':
					case 'l':
					case 'u':
					case 'B':
					case 'S':
					case 'I':
					case 'L':
					case 'U':
						ReadNextChar();
						return ch.ToString() + ch2.ToString();
				}
			}
			return ch.ToString();
		}
		void DoDotOrNumber() {
			switch(PeekNextChar()) {
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					DoNumber('.');
					break;
				default:
					this.CurrentToken = '.';
					break;
			}
		}
		public static bool CanStartColumn(char value) {
			switch(CharUnicodeInfo.GetUnicodeCategory(value)) {
				case UnicodeCategory.UppercaseLetter:
				case UnicodeCategory.LowercaseLetter:
				case UnicodeCategory.TitlecaseLetter:
				case UnicodeCategory.ModifierLetter:
				case UnicodeCategory.OtherLetter:
				case UnicodeCategory.ConnectorPunctuation:
					return true;
				default:
					return false;
			}
		}
		public static bool CanContinueColumn(char value) {
			switch(CharUnicodeInfo.GetUnicodeCategory(value)) {
				case UnicodeCategory.UppercaseLetter:
				case UnicodeCategory.LowercaseLetter:
				case UnicodeCategory.TitlecaseLetter:
				case UnicodeCategory.ModifierLetter:
				case UnicodeCategory.OtherLetter:
				case UnicodeCategory.ConnectorPunctuation:
				case UnicodeCategory.DecimalDigitNumber:
				case UnicodeCategory.LetterNumber:
				case UnicodeCategory.OtherNumber:
					return true;
				default:
					return false;
			}
		}
		public static bool IsGoodUnescapedName(string fnName) {
			if(fnName == null)
				return false;
			if(fnName.Length < 1)
				return false;
			if(!CanStartColumn(fnName[0]))
				return false;
			for(int i = 1; i < fnName.Length; ++i)
				if(!CanContinueColumn(fnName[i]))
					return false;
			int token;
			object value;
			ToTokenAndValue(fnName, out token, out value);
			return token == Token.COL;
		}
		public virtual void YYError(string message, params object[] args) {
			string fullMessage = string.Format(CultureInfo.InvariantCulture, message, args);
			throw new CriteriaParserException(fullMessage);
		}
		public void CheckFunctionArgumentsCount(FunctionOperator theOperator) {
			FunctionOperatorType functionType = theOperator.OperatorType;
			int argumentsCount = theOperator.Operands.Count;
			if((functionType == FunctionOperatorType.Custom || functionType == FunctionOperatorType.CustomNonDeterministic)) {
				if((argumentsCount > 0) && FunctionOperatorHelper.IsValidCustomFunctionArgumentsCount(((OperandValue)theOperator.Operands[0]).Value.ToString(), argumentsCount - 1)) return;
		} else {
				int[] argumentsCountArray = FunctionOperatorHelper.GetFunctionArgumentsCount(functionType);
				if(argumentsCountArray == null) YYError("Wrong function - '{0}'.", functionType.ToString());
				for(int i = 0; i < argumentsCountArray.Length; i++) {
					if(argumentsCountArray[i] == argumentsCount) return;
					if(argumentsCountArray[i] < 0) {
						if((-argumentsCountArray[i]) <= argumentsCount) return;
					}
				}
			}
			YYError("Wrong arguments count ({0}). Function - '{1}'.", argumentsCount, functionType.ToString());
		}
	}
}
