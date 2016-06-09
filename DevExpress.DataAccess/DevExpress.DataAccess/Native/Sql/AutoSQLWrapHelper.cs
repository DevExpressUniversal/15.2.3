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

using System.Collections.Generic;
using System.Text;
using DevExpress.CodeParser;
using DevExpress.DataAccess.Native.Sql.SqlParser;
namespace DevExpress.DataAccess.Native.Sql {
	public class AutoSqlWrapHelper {
		static bool CanSplit(Token previous, Token current) {
			switch(previous.Type) {
				case Tokens.SELECT:
				case Tokens.FROMKW:
				case Tokens.AS:
				case Tokens.ON:
				case Tokens.AND:
				case Tokens.OR:
				case Tokens.JOIN:
				case Tokens.BY:
				case Tokens.GREATER:
				case Tokens.EQUAL:
				case Tokens.LESS:
					return false;
				case Tokens.INNER:
				case Tokens.OUTER:
					if(current.Type == Tokens.JOIN)
						return false;
					break;
				case Tokens.FULL:
				case Tokens.LEFT:
				case Tokens.RIGHT:
					if(current.Type == Tokens.OUTER)
						return false;
					break;
				case Tokens.GROUP:
					if(current.Type == Tokens.BY)
						return false;
					break;
			}
			switch(current.Type) {
				case Tokens.GREATER:
				case Tokens.EQUAL:
				case Tokens.LESS:
				case Tokens.AS:
					return false;
			}
			return NeedSpacesBetween(previous, current);
		}
		static bool NeedSpacesBetween(Token previous, Token current) {
			if(previous.Type == Tokens.LESS || previous.Type == Tokens.GREATER || previous.Type == Tokens.EQUAL)
				if(current.Type == Tokens.LESS || current.Type == Tokens.GREATER || current.Type == Tokens.EQUAL)
					return false;
			switch(previous.Type) {
				case Tokens.DOT:
				case Tokens.LBRACE:
				case Tokens.LPAR:
				case Tokens.GRAVIS:
					return false;
				case Tokens.COUNT:
				case Tokens.MAX:
				case Tokens.MIN:
				case Tokens.AVG:
				case Tokens.SUM:
					if(current.Type == Tokens.LPAR)
						return false;
					break;
			}
			switch(current.Type) {
				case Tokens.DOT:
				case Tokens.RBRACE:
				case Tokens.RPAR:
				case Tokens.GRAVIS:
				case Tokens.COMMA:
					return false;
			}
			return true;
		}
		static bool NeedNewLine(Token token) {
			switch(token.Type) {
				case Tokens.FROMKW:
				case Tokens.JOIN:
				case Tokens.INNER:
				case Tokens.OUTER:
				case Tokens.LEFT:
				case Tokens.RIGHT:
				case Tokens.FULL:
				case Tokens.WHERE:
				case Tokens.GROUP:
				case Tokens.ORDER:
					return true;
			}
			return false;
		}
		static int GetAdditionalSpaceCount(Token token) {
			switch(token.Type) {
				case Tokens.SELECT:
				case Tokens.GROUP:
				case Tokens.ORDER:
					return 0;
				case Tokens.WHERE:
					return 1;
			}
			return NeedNewLine(token) ? 2 : 7;
		}
		class TokensGroup : List<Token> {
			public bool NewLine { get { return NeedNewLine(this[0]); } }
			public int AdditionalSpacesCount { get { return GetAdditionalSpaceCount(this[0]); } }
			public override string ToString() {
				StringBuilder output = new StringBuilder();
				Token previous = null;
				foreach(Token token in this) {
					if(previous != null && NeedSpacesBetween(previous, token))
						output.Append(' ');
					output.Append(token.Value);
					previous = token;
				}
				return output.ToString();
			}
		}
		public static string AutoSqlTextWrap(string query, int maxLineLength) {
			if(query == null)
				return null;
			Token previousToken = null;
			TokensGroup currentGroup = new TokensGroup();
			List<TokensGroup> groups = new List<TokensGroup> {currentGroup};
			using(SourceStringReader stringReader = new SourceStringReader(query)) {
				CommonSqlScanner sqlScanner = new CommonSqlScanner(stringReader);
				for(;;) {
					Token token = sqlScanner.Scan();
					if(token.Type == Tokens.EOF)
						break;
					if(previousToken != null && CanSplit(previousToken, token)) {
						currentGroup = new TokensGroup();
						groups.Add(currentGroup);
					}
					currentGroup.Add(token);
					previousToken = token;
				}
			}
			int currentLineLength = 0;
			StringBuilder output = new StringBuilder();
			foreach(TokensGroup tokensGroup in groups) {
				string value = tokensGroup.ToString();
				if(output.Length > 0) {
					if(tokensGroup.NewLine || (currentLineLength + 1 + value.Length > maxLineLength)) {
						output.Append('\n').Append(new string(' ', tokensGroup.AdditionalSpacesCount));
						currentLineLength = tokensGroup.AdditionalSpacesCount;
					} else {
						output.Append(' ');
						currentLineLength++;
					}
				}
				output.Append(value);
				currentLineLength += value.Length;
			}
			return output.ToString();
		}
	}
}
