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
using System.CodeDom.Compiler;
using System.Drawing;
using DevExpress.CodeParser;
using DevExpress.DataAccess.Native.Sql.ConnectionProviders;
using DevExpress.DataAccess.Native.Sql.SqlParser;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.SyntaxEdit;
using TokensList = DevExpress.DataAccess.Native.Sql.SqlParser.Tokens;
namespace DevExpress.DataAccess.UI.Native.Sql {
	class SyntaxHelper : ISyntaxHelper {
		readonly ISyntaxColors syntaxColors;
		readonly TokenCollection tokens = new TokenCollection();
		readonly SqlRichEditControl syntaxEdit;
		public SyntaxHelper(SqlRichEditControl syntaxEdit) {
			this.syntaxEdit = syntaxEdit;
			this.syntaxColors = syntaxEdit.SyntaxColors;
		}
		#region ISyntaxHelper Members
		public void DrawHighlightMatchingBrackets(Graphics graphics) {
		}
		public Color GetTextColor(int position, int startTokenIndex) {
			TokenCategory tokenCategory = GetCurrentTokenCategory(position, startTokenIndex);
			if(tokenCategory == TokenCategory.Comment)
				return this.syntaxColors.CommentColor;
			if(tokenCategory == TokenCategory.Keyword)
				return this.syntaxColors.KeywordColor;
			if(tokenCategory == TokenCategory.String)
				return this.syntaxColors.StringColor;
			if(tokenCategory == TokenCategory.XmlComment)
				return this.syntaxColors.XmlCommentColor;
			return this.syntaxColors.TextColor;
		}
		public void MarkErrors(CompilerErrorCollection errors) {
			throw new NotSupportedException();
		}
		public IAliasFormatter AliasFormatter { get; set; }
		public ITokenCollection Tokens { get { return this.tokens; } }
		#endregion
		#region ISyntaxCheckService Members
		public RunInfo[] Check(DocumentModelPosition start, DocumentModelPosition end) {
			return new RunInfo[] {};
		}
		#endregion
		TokenCategory GetCurrentTokenCategory(int position, int startTokenIndex) {
			for(int i = Math.Max(0, startTokenIndex); i < this.tokens.Count; i++)
				if(this.tokens[i].StartPosition <= position && this.tokens[i].EndPosition > position) {
					return GetTokenCategory(this.tokens[i]);
				}
			return TokenCategory.Text;
		}
		TokenCategory GetTokenCategory(Token token) {
			switch(token.Type) {
				case TokensList.SINGLESTRING:
					return AliasFormatter != null && AliasFormatter.SingleQuotedString ? TokenCategory.String : TokenCategory.Text;
				case TokensList.DOUBLESTRING:
					return AliasFormatter != null && AliasFormatter.SingleQuotedString ? TokenCategory.Text : TokenCategory.String;
				case TokensList.GRAVISSTRING:
				case TokensList.SQUAREBRACKETSTRING:
				case TokensList.MaxTokens:
				case TokensList.IDENT:
				case TokensList.COMMA:
				case TokensList.DOT:
				case TokensList.LPAR:
				case TokensList.RPAR:
				case TokensList.LBRACE:
				case TokensList.RBRACE:
				case TokensList.REALCON:
				case TokensList.INTCON:
					return TokenCategory.Text;
				case TokensList.COMMENT:
					return TokenCategory.Comment;
				default:
					return TokenCategory.Keyword;
			}
		}
		#region IContentChangedNotificationService Members
		public void NotifyContentChanged() {
			string text = this.syntaxEdit.Text.ToLower();
			using(SourceStringReader stringReader = new SourceStringReader(text)) {
				CommonSqlScanner sqlScanner = new CommonSqlScanner(stringReader);
				this.tokens.Clear();
				Token token = sqlScanner.Scan();
				while(token.Type != TokensList.EOF) {
					this.tokens.Add(token);
					token = sqlScanner.Scan();
				}
			}
		}
		#endregion
	}
}
