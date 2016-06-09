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
using System.Drawing;
using DevExpress.CodeParser;
using DevExpress.Utils;
using DevExpress.Office.Drawing;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.SyntaxEdit;
using DevExpress.XtraRichEdit.Utils;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Layout.Export {
	#region SyntaxEditDocumentLayoutExporter
	public class SyntaxEditDocumentLayoutExporter : ScreenOptimizedGraphicsDocumentLayoutExporter {
		#region Fields
		readonly ISyntaxEditControl syntaxEdit;
		readonly BoxMeasurer measurer;
		int startTokenIndex;
		#endregion
		public SyntaxEditDocumentLayoutExporter(ISyntaxEditControl syntaxEdit, BoxMeasurer measurer, DocumentModel documentModel, Painter painter, GraphicsDocumentLayoutExporterAdapter adapter, Rectangle bounds)
			: base(documentModel, painter, adapter, bounds, TextColors.Defaults) {
			this.syntaxEdit = syntaxEdit;
			this.measurer = measurer;
		}
		ITokenCollection Tokens { get { return syntaxEdit.SyntaxHelper.Tokens; } }
		public override void ExportTextBox(TextBox box) {
			foreach (TextBox currentBox in GetBoxes(box)) {
				this.Painter.TextForeColor = GetTextColor(currentBox);
				bool allow = Painter.AllowChangeTextForeColor;
				Painter.AllowChangeTextForeColor = false;
				try {
					base.ExportTextBox(currentBox);
				}
				finally {
					Painter.AllowChangeTextForeColor = allow;
				}
			}
		}
		public override void ExportLineNumberBox(LineNumberBox box) {
			this.Painter.TextForeColor = syntaxEdit.SyntaxColors.LineNumbersColor;
			bool allow = Painter.AllowChangeTextForeColor;
			Painter.AllowChangeTextForeColor = false;
			try {
				base.ExportLineNumberBox(box);
			}
			finally {
				Painter.AllowChangeTextForeColor = allow;
			}
		}
		static bool IsStartToken(IToken token, int position) {
			return token.StartPosition <= position && token.EndPosition > position;
		}
		static bool IsEndToken(IToken token, int endPosition) {
			return token.EndPosition >= endPosition;
		}
		List<TextBox> GetBoxes(TextBox box) {
			string text = box.GetText(PieceTable);
			int position = GetBoxPosition(box);
			List<TextBox> textBoxes = new List<TextBox>();
			bool addTextBox = false;
			for (int i = Math.Max(0, startTokenIndex); i < Tokens.Count - 1; i++) {
				bool isEndToken = IsEndToken(Tokens[i], position + text.Length);
				if (IsStartToken(Tokens[i], position)) {
					startTokenIndex = i;
					if (isEndToken)
						break;
					addTextBox = true;
				}
				if (addTextBox) {
					BoxInfo boxInfo = GetBoxInfo(Tokens[i], box, text, position);
					CreateTextBox(box, boxInfo, textBoxes);
				}
				if (isEndToken)
					return textBoxes;
			}
			textBoxes = new List<TextBox>();
			textBoxes.Add(box);
			return textBoxes;
		}
		void CreateTextBox(TextBox box, BoxInfo boxInfo, List<TextBox> textBoxes) {
			TextBox textBox = new TextBox();
			textBox.StartPos = boxInfo.StartPos;
			textBox.EndPos = boxInfo.EndPos;
			Point location = box.Bounds.Location;
			if (textBoxes.Count > 0) {
				TextBox lastTextBox = textBoxes[textBoxes.Count - 1];
				location = lastTextBox.Bounds.Location;
				location.Offset(lastTextBox.Bounds.Width, 0);
			}
			textBox.Bounds = new Rectangle(location, boxInfo.Size);
			textBoxes.Add(textBox);
		}
		BoxInfo GetBoxInfo(IToken token, TextBox box, string text, int position) {
			int actualStartPos = token.StartPosition - position;
			int startPos = Math.Max(0, Math.Min(text.Length, actualStartPos));
			int endPos = Math.Max(startPos, Math.Min(text.Length, actualStartPos + token.Length));
			string resultText = text.Substring(startPos, endPos - startPos);
			BoxInfo boxInfo = new BoxInfo();
			boxInfo.StartPos = new FormatterPosition(box.StartPos.RunIndex, box.StartPos.Offset + startPos, 0);
			boxInfo.EndPos = new FormatterPosition(box.EndPos.RunIndex, Math.Min(box.EndPos.Offset, box.StartPos.Offset + endPos), 0);
			measurer.MeasureText(boxInfo, resultText, box.GetFontInfo(PieceTable));
			return boxInfo;
		}
		Color GetTextColor(TextBox box) {
			return syntaxEdit.SyntaxHelper.GetTextColor(GetBoxPosition(box), startTokenIndex);
		}
		int GetBoxPosition(TextBox box) {
			DocumentLogPosition pos = this.PieceTable.GetRunLogPosition(box.StartPos.RunIndex);
			int paragraphCountBefore = ((IConvertToInt<ParagraphIndex>)(PieceTable.Runs[box.StartPos.RunIndex].Paragraph.Index)).ToInt();
			return ((IConvertToInt<DocumentLogPosition>)pos).ToInt() + box.StartPos.Offset + paragraphCountBefore;
		}
	}
	#endregion
}
