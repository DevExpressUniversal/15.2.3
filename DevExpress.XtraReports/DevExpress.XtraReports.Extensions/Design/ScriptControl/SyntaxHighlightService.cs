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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using DevExpress.CodeParser;
using DevExpress.CodeParser.CSharp;
using DevExpress.CodeParser.JavaScript;
using DevExpress.CodeParser.VB;
using DevExpress.Utils;
using DevExpress.XtraReports.UI;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.SyntaxEdit;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraReports.Design {
	[CLSCompliant(false)]
	public class SyntaxHelper : ISyntaxHelper {
		#region static
		static readonly ParserVersion version = MethodsHelper.GetParserVersion();
		#if DEBUGTEST
		internal
		#endif
		static IList<Token> CreateErrorTokens(CompilerErrorCollection errors, TokenCollection tokens, int startParagraph, int endParagraph) {
			List<Token> errorTokens = new List<Token>();
			foreach(CompilerError error in errors) {
				if(error.Line < startParagraph)
					continue;
				if(error.Line > endParagraph)
					break;
				Token errorToken = new Token(error.Line, error.Column, error.Line, error.Column, 0, string.Empty);
				if(!errorTokens.Exists(delegate(Token token) { return errorToken.Range.Intersects(token.Range); })) {
					int index = ArrayList.Adapter(tokens).BinarySearch(errorToken, new TokenErrorsRangeComparer());
					if(index < 0)
						errorTokens.Add(errorToken);
					else
						errorTokens.Add(tokens[index]);
				}
			}
			errorTokens.Sort(new TokenErrorsRangeComparer());
			return errorTokens;
		}
		#endregion
		#region fields
		IScriptSource scriptSource;
		SyntaxEditor syntaxEdit;
		TokenCollection tokens = null;
		ISyntaxColors syntaxColors;
		CompilerErrorCollection errors;
		IServiceProvider servProvider;
		#endregion
		public SyntaxHelper(SyntaxEditor syntaxEdit, IScriptSource scriptSource, IServiceProvider servProvider) {
			this.scriptSource = scriptSource;
			this.syntaxEdit = syntaxEdit;
			this.syntaxColors = syntaxEdit.SyntaxColors;
			this.servProvider = servProvider;
		}
		#region ISyntaxHelper Members
		public ITokenCollection Tokens {
			get { return tokens; }
		}
		public Color GetTextColor(int position, int startTokenIndex) {
			TokenCategory tokenCategory = GetCurrentTokenCategory(position, startTokenIndex);
			if(tokenCategory == TokenCategory.Comment)
				return syntaxColors.CommentColor;
			else if(tokenCategory == TokenCategory.Keyword)
				return syntaxColors.KeywordColor;
			else if(tokenCategory == TokenCategory.String)
				return syntaxColors.StringColor;
			else if(tokenCategory == TokenCategory.XmlComment)
				return syntaxColors.XmlCommentColor;
			return syntaxColors.TextColor;
		}
		public void MarkErrors(CompilerErrorCollection errors) {
			this.errors = errors;
			new CheckSyntaxCommand(syntaxEdit).Execute();
		}
		public void DrawHighlightMatchingBrackets(Graphics graphics) {
			List<Rectangle> matchingBracketsRects = MatchingBracketsRects();
			if(matchingBracketsRects != null) {
				using (Brush brush = new SolidBrush(syntaxColors.BracketHighlightColor)) {
					graphics.FillRectangle(brush, matchingBracketsRects[0]);
					graphics.FillRectangle(brush, matchingBracketsRects[1]);
				}
			}
		}
		#endregion
		#region ISyntaxCheckService Members
		public RunInfo[] Check(DocumentModelPosition start, DocumentModelPosition end) {
			if(errors == null || errors.Count == 0)
				return new RunInfo[] { };
			int startParagraph = ((IConvertToInt<ParagraphIndex>)start.ParagraphIndex).ToInt();
			int endParagraph = ((IConvertToInt<ParagraphIndex>)end.ParagraphIndex).ToInt();
			List<RunInfo> errorsRunInfo = new List<RunInfo>();
			IList<Token> errorTokens = CreateErrorTokens(errors, tokens, startParagraph, endParagraph);
			foreach(Token errorToken in errorTokens) {
				RunInfo errorRunInfo = new RunInfo(start.PieceTable);
				DevExpress.XtraRichEdit.Model.Paragraph paragraph;
				if(!TryGetParagraph(start.PieceTable.Paragraphs, errorToken.Line - 1, out paragraph))
					continue;
				int paragraphPosition = ((IConvertToInt<DocumentLogPosition>)paragraph.LogPosition).ToInt();
				DocumentLogPosition startLogPosition = new DocumentLogPosition(paragraphPosition + Math.Max(0, errorToken.Column - 1));
				DocumentLogPosition endLogPosition = new DocumentLogPosition(paragraphPosition + Math.Max(0, errorToken.EndColumn - 1));
				if(startLogPosition >= start.PieceTable.DocumentEndLogPosition)
					continue;				
				DocumentModelPosition startPos = PositionConverter.ToDocumentModelPosition(start.PieceTable, startLogPosition);				
				DocumentModelPosition endPos = PositionConverter.ToDocumentModelPosition(start.PieceTable, Algorithms.Min(endLogPosition, start.PieceTable.DocumentEndLogPosition));
				errorRunInfo.Start.CopyFrom(startPos);
				errorRunInfo.End.CopyFrom(endPos);
				errorsRunInfo.Add(errorRunInfo);
			}
			return errorsRunInfo.ToArray();
		}
		static bool TryGetParagraph(DevExpress.XtraRichEdit.Model.ParagraphCollection paragraphs, int index, out DevExpress.XtraRichEdit.Model.Paragraph value) {
			if(index >= 0 && index < paragraphs.Count) {
				value = paragraphs[new ParagraphIndex(index)];
				return true;
			}
			value = null;
			return false;
		}
		static bool TryGetParagraph(DevExpress.XtraRichEdit.API.Native.ParagraphCollection paragraphs, int index, out DevExpress.XtraRichEdit.API.Native.Paragraph value) {
			if(index >= 0 && index < paragraphs.Count) {
				value = paragraphs[index];
				return true;
			}
			value = null;
			return false;
		}
		#endregion
		TokenCollection GetTokens() {
			TokenCollection result;
			try {
				if(scriptSource.ScriptLanguage == ScriptLanguage.VisualBasic)
					result = VBTokensHelper.GetTokens(syntaxEdit.Text);
				else if(scriptSource.ScriptLanguage == ScriptLanguage.JScript)
					result = JavaScriptTokensHelper.GetTokens(syntaxEdit.Text);
				else result = CSharpTokensHelper.GetTokens(syntaxEdit.Text);
			}
			catch {
				result = new TokenCollection();
			}
			return result;
		}
		List<Rectangle> MatchingBracketsRects() {
			if(string.IsNullOrEmpty(syntaxEdit.Text))
				return null;
			DocumentPosition caretPosition = syntaxEdit.Document.CaretPosition;
			DevExpress.XtraRichEdit.API.Native.Paragraph para = syntaxEdit.Document.Paragraphs.Get(caretPosition);
			int line = para.Index + 1;
			int column = caretPosition.ToInt() - para.Range.Start.ToInt() + 1;
			int index = ArrayList.Adapter(tokens).BinarySearch(new Token(line, column, line, column, 0, string.Empty), new TokenBracketComparer());
			if(index >= 0) {
				Token token = tokens[index];
				Token secondToken = FindSecondToken(token);
				if(secondToken != null) {
					List<Rectangle> rects = new List<Rectangle>();
					try {
						rects.Add(syntaxEdit.GetLayoutPhysicalBoundsFromPosition(syntaxEdit.Document.CreatePosition(GetTokenStartPosition(token))));
						rects.Add(syntaxEdit.GetLayoutPhysicalBoundsFromPosition(syntaxEdit.Document.CreatePosition(GetTokenStartPosition(secondToken))));
					}
					catch {
						return null;
					}
					return rects;
				}
			}
			return null;
		}
		int GetTokenStartPosition(Token token) {
			DevExpress.XtraRichEdit.API.Native.Paragraph paragraph;
			if(!TryGetParagraph(syntaxEdit.Document.Paragraphs, token.Range.Start.Line - 1, out paragraph))
				return 0;
			return DocumentHelper.GetParagraphStart(syntaxEdit.Document.Paragraphs[token.Range.Start.Line - 1]) + token.Range.Start.Offset - 1;
		}
		Token FindSecondToken(Token token) {
			return BracketSearcher.CreateInstance(token).FindClosingBracket(tokens);
		}
		TokenCategory GetCurrentTokenCategory(int position, int startTokenIndex) {
			for(int i = Math.Max(0, startTokenIndex); i < tokens.Count; i++)
				if(tokens[i].StartPosition <= position && tokens[i].EndPosition > position)
					return GetTokenCategory((CategorizedToken)tokens[i]);
			return TokenCategory.Text;
		}
		TokenCategory GetTokenCategory(CategorizedToken token) {
			if(scriptSource.ScriptLanguage == ScriptLanguage.CSharp) {
				if(token == null)
					return TokenCategory.Text;
				if(String.Compare(token.Value, "var", StringComparison.CurrentCulture) == 0)
					return TokenCategory.Keyword;
				if(version >= ParserVersion.VS2010)
					if(String.Compare(token.Value, "dynamic", StringComparison.CurrentCulture) == 0)
						return TokenCategory.Keyword;
			}
			return token.Category;
		}
		#region IContentChangedNotificationService Members
		public void NotifyContentChanged() {
			tokens = GetTokens();
			XtraReport report = ((ScriptSource)scriptSource).SourceReport;
			if(report.ScriptsSource == syntaxEdit.Text || servProvider.IsDebugging() || syntaxEdit.IsInitializing)
				return;
			IComponentChangeService changeService = ((IDesignerHost)report.Site.Container).GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			PropertyDescriptor property = DevExpress.XtraReports.Native.XRAccessor.GetPropertyDescriptor(report, "ScriptsSource");
			changeService.OnComponentChanging(report, property);
			report.ScriptsSource = syntaxEdit.Text;
			changeService.OnComponentChanged(report, property, null, null);
		}
		#endregion
	}
}
