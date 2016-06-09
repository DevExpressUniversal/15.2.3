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
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.CodeParser;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.Office.Internal;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Import;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.API.Native;
namespace DevExpress.DXperience.Demos.CodeDemo {
	public class CustomRichEditCommandFactoryService :IRichEditCommandFactoryService {
		private readonly IRichEditCommandFactoryService service;
		public CustomRichEditCommandFactoryService(IRichEditCommandFactoryService service) {
			Guard.ArgumentNotNull(service, "service");
			this.service = service;
		}
		RichEditCommand IRichEditCommandFactoryService.CreateCommand(RichEditCommandId id) {
			if(id.Equals(RichEditCommandId.InsertColumnBreak) || id.Equals(RichEditCommandId.InsertLineBreak) || id.Equals(RichEditCommandId.InsertPageBreak)) {
				return service.CreateCommand(RichEditCommandId.InsertParagraph);
			}
			return service.CreateCommand(id);
		}
	}
	public static class SourceCodeDocumentFormat {
		public static readonly DocumentFormat Id = new DocumentFormat(1325);
	}
	public class SourcesCodeDocumentExporter :PlainTextDocumentExporter {
		public override FileDialogFilter Filter {
			get {
				return SourcesCodeDocumentImporter.filter;
			}
		}
		public override DocumentFormat Format {
			get {
				return SourceCodeDocumentFormat.Id;
			}
		}
	}
	public class SourcesCodeDocumentImporter :PlainTextDocumentImporter {
		internal static readonly FileDialogFilter filter = new FileDialogFilter("Source Files", new string[] { "cs", "vb", "html", "htm", "js", "xml", "css" });
		public override FileDialogFilter Filter {
			get {
				return filter;
			}
		}
		public override DocumentFormat Format {
			get {
				return SourceCodeDocumentFormat.Id;
			}
		}
	}
	public class SyntaxHighlightInfo {
		private readonly Dictionary<TokenCategory, SyntaxHighlightProperties> properties;
		private readonly LookAndFeel.UserLookAndFeel userLookAndFeel;
		public SyntaxHighlightInfo(LookAndFeel.UserLookAndFeel userLookAndFeel) {
			this.userLookAndFeel = userLookAndFeel;
			properties = new Dictionary<TokenCategory, SyntaxHighlightProperties>();
			Reset();
		}
		private void Add(TokenCategory category, Color foreColor) {
			var item = new SyntaxHighlightProperties();
			item.ForeColor = foreColor;
			properties.Add(category, item);
		}
		public SyntaxHighlightProperties CalculateTokenCategoryHighlight(TokenCategory category) {
			var result = (SyntaxHighlightProperties)null;
			if(properties.TryGetValue(category, out result)) {
				return result;
			} else {
				return properties[TokenCategory.Text];
			}
		}
		public void Reset() {
			properties.Clear();
			Add(TokenCategory.Text, DXColor.Empty);
			Add(TokenCategory.Keyword, GetColor(DXColor.Blue));
			Add(TokenCategory.String, GetColor(DXColor.Brown));
			Add(TokenCategory.Comment, GetColor(DXColor.Green));
			Add(TokenCategory.Identifier, DXColor.Empty);
			Add(TokenCategory.PreprocessorKeyword, GetColor(DXColor.Blue));
			Add(TokenCategory.Number, GetColor(DXColor.Red));
			Add(TokenCategory.Operator, DXColor.Empty);
			Add(TokenCategory.Unknown, DXColor.Empty);
			Add(TokenCategory.XmlComment, GetColor(DXColor.Gray));
			Add(TokenCategory.CssComment, GetColor(DXColor.Green));
			Add(TokenCategory.CssKeyword, GetColor(DXColor.Brown));
			Add(TokenCategory.CssPropertyName, GetColor(DXColor.Red));
			Add(TokenCategory.CssPropertyValue, GetColor(DXColor.Blue));
			Add(TokenCategory.CssSelector, GetColor(DXColor.Blue));
			Add(TokenCategory.CssStringValue, GetColor(DXColor.Blue));
			Add(TokenCategory.HtmlAttributeName, GetColor(DXColor.Red));
			Add(TokenCategory.HtmlAttributeValue, GetColor(DXColor.Blue));
			Add(TokenCategory.HtmlComment, GetColor(DXColor.Green));
			Add(TokenCategory.HtmlElementName, GetColor(DXColor.Brown));
			Add(TokenCategory.HtmlEntity, GetColor(DXColor.Gray));
			Add(TokenCategory.HtmlOperator, DXColor.Empty);
			Add(TokenCategory.HtmlServerSideScript, DXColor.Empty);
			Add(TokenCategory.HtmlString, GetColor(DXColor.Blue));
			Add(TokenCategory.HtmlTagDelimiter, GetColor(DXColor.Blue));
		}
		private Color GetColor(Color color) {
			if(DevExpress.Utils.Frames.FrameHelper.IsDarkSkin(userLookAndFeel) &&
				userLookAndFeel.ActiveSkinName != "Dark Side" &&
				userLookAndFeel.ActiveSkinName != "Office 2010 Black" &&
				userLookAndFeel.ActiveSkinName != "Sharp" &&
				userLookAndFeel.ActiveSkinName != "Office 2016 Dark") {
				if(color == Color.Blue) return Color.DeepSkyBlue;
				if(color == Color.Brown) return Color.LightCoral;
				if(color == Color.Green) return Color.LightGreen;
				if(color == Color.Red) return Color.LightPink;
				if(color == Color.Gray) return Color.Silver;
			}
			return color;
		}
	}
	public class SyntaxHighlightService :ISyntaxHighlightService {
		private readonly InnerRichEditControl editor;
		private readonly string fileExtensionToHightlight;
		private SyntaxHighlightInfo syntaxHighlightInfo;
		public SyntaxHighlightService(InnerRichEditControl editor, string extension) {
			this.editor = editor;
			((RichEditControl)(editor.Owner)).LookAndFeel.StyleChanged += LookAndFeel_StyleChanged;
			syntaxHighlightInfo = new SyntaxHighlightInfo(((RichEditControl)(editor.Owner)).LookAndFeel);
			fileExtensionToHightlight = extension;
		}
		void LookAndFeel_StyleChanged(object sender, EventArgs e) {
			syntaxHighlightInfo = new SyntaxHighlightInfo(((RichEditControl)(editor.Owner)).LookAndFeel);
			ExecuteCore();
		}
		private ITokenCategoryHelper CreateTokenizer() {
			var fileName = editor.Options.DocumentSaveOptions.CurrentFileName;
			string extenstion;
			if(String.IsNullOrEmpty(fileName)) {
				extenstion = fileExtensionToHightlight;
			} else {
				extenstion = Path.GetExtension(fileName);
			}
			var result = TokenCategoryHelperFactory.CreateHelperForFileExtensions(extenstion);
			if(result != null) {
				return result;
			} else {
				return null;
			}
		}
		private void ExecuteCore() {
			var tokens = Parse(editor.Text);
			HighlightSyntax(tokens);
		}
		private void HighlightCategorizedToken(CategorizedToken token, List<SyntaxHighlightToken> syntaxTokens) {
			var highlightProperties = syntaxHighlightInfo.CalculateTokenCategoryHighlight(token.Category);
			var syntaxToken = SetTokenColor(token, highlightProperties);
			if(syntaxToken != null) {
				syntaxTokens.Add(syntaxToken);
			}
		}
		private void HighlightSyntax(TokenCollection tokens) {
			if(tokens == null || tokens.Count == 0) {
				return;
			}
			var document = editor.Document;
			var cp = document.BeginUpdateCharacters(0, 1);
			var syntaxTokens = new List<SyntaxHighlightToken>(tokens.Count);
			foreach(Token token in tokens) {
				HighlightCategorizedToken((CategorizedToken)token, syntaxTokens);
			}
			document.ApplySyntaxHighlight(syntaxTokens);
			document.EndUpdateCharacters(cp);
		}
		void ISyntaxHighlightService.Execute() {
			ExecuteCore();
		}
		void ISyntaxHighlightService.ForceExecute() {
			ExecuteCore();
		}
		private TokenCollection Parse(string code) {
			if(string.IsNullOrEmpty(code)) {
				return null;
			}
			var tokenizer = CreateTokenizer();
			if(tokenizer == null) {
				return new TokenCollection();
			}
			return tokenizer.GetTokens(code);
		}
		private SyntaxHighlightToken SetTokenColor(Token token, SyntaxHighlightProperties foreColor) {
			if(editor.Document.Paragraphs.Count < token.Range.Start.Line) {
				return null;
			}
			var paragraphStart = DocumentHelper.GetParagraphStart(editor.Document.Paragraphs[token.Range.Start.Line - 1]);
			var tokenStart = paragraphStart + token.Range.Start.Offset - 1;
			if(token.Range.End.Line != token.Range.Start.Line) {
				paragraphStart = DocumentHelper.GetParagraphStart(editor.Document.Paragraphs[token.Range.End.Line - 1]);
			}
			var tokenEnd = paragraphStart + token.Range.End.Offset - 1;
			System.Diagnostics.Debug.Assert(tokenEnd > tokenStart);
			return new SyntaxHighlightToken(tokenStart, tokenEnd - tokenStart, foreColor);
		}
	}
	public static class SyntaxHightlightInitializeHelper {
		public static void Initialize(IRichEditControl richEditControl, string codeExamplesFileExtension) {
			var innerControl = richEditControl.InnerControl;
			var commandFactory = innerControl.GetService<IRichEditCommandFactoryService>();
			if(commandFactory == null) {
				return;
			}
			innerControl.ReplaceService<ISyntaxHighlightService>(new SyntaxHighlightService(innerControl, codeExamplesFileExtension));
			var newCommandFactory = new CustomRichEditCommandFactoryService(commandFactory);
			innerControl.RemoveService(typeof(IRichEditCommandFactoryService));
			innerControl.AddService(typeof(IRichEditCommandFactoryService), newCommandFactory);
			var importManager = innerControl.GetService<IDocumentImportManagerService>();
			importManager.UnregisterAllImporters();
			importManager.RegisterImporter(new PlainTextDocumentImporter());
			importManager.RegisterImporter(new SourcesCodeDocumentImporter());
			var exportManager = innerControl.GetService<IDocumentExportManagerService>();
			exportManager.UnregisterAllExporters();
			exportManager.RegisterExporter(new PlainTextDocumentExporter());
			exportManager.RegisterExporter(new SourcesCodeDocumentExporter());
			var document = innerControl.Document;
			document.BeginUpdate();
			try {
				document.DefaultCharacterProperties.FontName = "Consolas";
				document.DefaultCharacterProperties.FontSize = 10;
				document.Sections[0].Page.Width = Units.InchesToDocumentsF(100);
				document.Sections[0].LineNumbering.CountBy = 1;
				document.Sections[0].LineNumbering.RestartType = LineNumberingRestart.Continuous;
			} finally {
				document.EndUpdate();
			}
		}
	}
}
