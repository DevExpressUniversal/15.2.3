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
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Export.OpenXml;
using DevExpress.XtraRichEdit.Import.Xaml;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Export.Xaml {
	public class XamlExporter : XmlBasedDocumentModelExporter, IXamlExporter {
		readonly XamlDocumentExporterOptions options;
		XmlWriter documentContentWriter;
		readonly Stack<CharacterFormattingInfo> characterFormattingInfoStack;
		readonly Stack<ParagraphFormattingInfo> paragraphFormattingInfoStack;
		public XamlExporter(DocumentModel documentModel, XamlDocumentExporterOptions options)
			: base(documentModel) {
			Guard.ArgumentNotNull(options, "options");
			this.options = options;
			this.characterFormattingInfoStack = new Stack<CharacterFormattingInfo>();
			this.paragraphFormattingInfoStack = new Stack<ParagraphFormattingInfo>();
		}
		protected internal XmlWriter DocumentContentWriter { get { return documentContentWriter; } set { documentContentWriter = value; } }
		protected internal override InternalZipArchive Package { get { return null; } }
		public new virtual string Export() {
			using (MemoryStream stream = new MemoryStream()) {
				Export(stream);
				return DXEncoding.UTF8NoByteOrderMarks.GetString(stream.GetBuffer(), 0, (int)stream.Length);
			}
		}
		public virtual void Export(Stream stream) {
			StreamWriter writer = new StreamWriter(stream, DXEncoding.UTF8NoByteOrderMarks);
			Export(writer);
		}
		public virtual void Export(TextWriter textWriter) {
			using (XmlWriter writer = XmlWriter.Create(textWriter, CreateXmlWriterSettings())) {
				Export(writer);
			}
		}
		void Export(XmlWriter writer) {
			DocumentContentWriter = writer;
			GenerateDocumentContent();
			writer.Flush();
		}
		protected internal override XmlWriterSettings CreateXmlWriterSettings() {
			XmlWriterSettings settings = base.CreateXmlWriterSettings();
			settings.OmitXmlDeclaration = true;
			return settings;
		}
		protected internal void GenerateDocumentContent() {
			DocumentContentWriter.WriteStartElement("Section", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
			DocumentContentWriter.WriteAttributeString("xml", "space", string.Empty, "preserve");
			ExportDefaultStyle();
			try {
				base.Export();
			}
			finally {
				characterFormattingInfoStack.Pop();
				paragraphFormattingInfoStack.Pop();
				DocumentContentWriter.WriteEndElement();
			}
		}
		protected internal virtual void ExportDefaultStyle() {
			CharacterFormattingInfo characterFormattingInfo = DocumentModel.DefaultCharacterProperties.Info.Info.Clone();
			RemoveDecorations(characterFormattingInfo);
			characterFormattingInfoStack.Push(CreateFakeCharacterFormattingInfo(characterFormattingInfo));
			if (options.ExportDefaultStyle)
				ExportCharacterFormattingInfo(characterFormattingInfo, characterFormattingInfoStack.Peek(), false);
			characterFormattingInfoStack.Push(characterFormattingInfo);
			ParagraphFormattingInfo paragraphFormattingInfo = DocumentModel.DefaultParagraphProperties.Info.Info.Clone();
			paragraphFormattingInfoStack.Push(CreateFakeParagraphFormattingInfo(paragraphFormattingInfo));
			if (options.ExportDefaultStyle)
				ExportParagraphFormattingInfo(paragraphFormattingInfo, paragraphFormattingInfoStack.Peek());
			paragraphFormattingInfoStack.Push(paragraphFormattingInfo);
		}
		protected internal virtual CharacterFormattingInfo CreateFakeCharacterFormattingInfo(CharacterFormattingInfo info) {
			CharacterFormattingInfo result = info.Clone();
			result.FontName += "-";
			result.DoubleFontSize++;
			result.FontBold = !info.FontBold;
			result.FontItalic = !info.FontItalic;
			result.ForeColor = DXColor.FromArgb(DXColor.ToArgb(info.ForeColor) + 1);
			result.BackColor = DXColor.FromArgb(DXColor.ToArgb(info.BackColor) + 1);
			return result;
		}
		protected internal virtual ParagraphFormattingInfo CreateFakeParagraphFormattingInfo(ParagraphFormattingInfo info) {
			ParagraphFormattingInfo result = info.Clone();
			result.LeftIndent++;
			result.LineSpacing++;
			result.SpacingAfter++;
			result.SpacingBefore++;
			result.SuppressHyphenation = !info.SuppressHyphenation;
			result.Alignment = (ParagraphAlignment)(((int)info.Alignment + 1) % 4);
			result.FirstLineIndentType = (ParagraphFirstLineIndent)(((int)info.FirstLineIndentType + 1) % 3);
			result.LineSpacingType = (ParagraphLineSpacing)(((int)info.LineSpacingType + 1) % 6);
			return result;
		}
		protected internal virtual void RemoveDecorations(CharacterFormattingInfo info) {
			info.Script = CharacterFormattingScript.Normal;
			info.FontUnderlineType = UnderlineType.None;
			info.FontStrikeoutType = StrikeoutType.None;
		}
		#region Paragraph
		Paragraph currentParagraph;
		ParagraphBreakBefore currentParagraphBreakBefore;
		protected internal override ParagraphIndex ExportParagraph(Paragraph paragraph) {
			this.currentParagraph = paragraph;
			if (currentParagraphBreakBefore == ParagraphBreakBefore.None && paragraph.Length > 1) {
				TextRun firstRun = PieceTable.Runs[paragraph.FirstRunIndex] as TextRun;
				if (firstRun != null) {
					string text = firstRun.GetPlainText(PieceTable.TextBuffer, 0, 0);
					if (!String.IsNullOrEmpty(text)) { 
						char firstRunCharacter = text[0];
						if (firstRunCharacter == Characters.PageBreak)
							currentParagraphBreakBefore = ParagraphBreakBefore.Page;
						else if (firstRunCharacter == Characters.ColumnBreak)
							currentParagraphBreakBefore = ParagraphBreakBefore.Column;
					}
				}
			}
			WriteCurrentParagraphStartElement(currentParagraphBreakBefore);
			currentParagraphBreakBefore = ParagraphBreakBefore.None;
			try {
				return base.ExportParagraph(paragraph);
			}
			finally {
				WriteCurrentParagraphEndElement();
			}
		}
		protected internal virtual void WriteCurrentParagraphStartElement(ParagraphBreakBefore breakBefore) {
			DocumentContentWriter.WriteStartElement("Paragraph");
			WriteParagraphAttributes(breakBefore);
			CharacterFormattingInfo characterFormattingInfo = currentParagraph.GetMergedCharacterProperties().Info;
			RemoveDecorations(characterFormattingInfo);
			ExportCharacterFormattingInfo(characterFormattingInfo, characterFormattingInfoStack.Peek(), false);
			characterFormattingInfoStack.Push(characterFormattingInfo);
		}
		protected internal virtual void WriteCurrentParagraphEndElement() {
			characterFormattingInfoStack.Pop();
			paragraphFormattingInfoStack.Pop();
			DocumentContentWriter.WriteEndElement();
		}
		protected internal virtual void WriteParagraphAttributes(ParagraphBreakBefore breakBefore) {
			if (!options.SilverlightCompatible) {
				if (breakBefore != ParagraphBreakBefore.None) {
					if (breakBefore == ParagraphBreakBefore.Page)
						DocumentContentWriter.WriteAttributeString("BreakPageBefore", "True");
					else
						DocumentContentWriter.WriteAttributeString("BreakColumnBefore", "True");
				}
			}
			ParagraphFormattingInfo paragraphFormattingInfo = currentParagraph.GetMergedParagraphProperties().Info;
			ExportParagraphFormattingInfo(paragraphFormattingInfo, paragraphFormattingInfoStack.Peek());
			ExportFirstLineIndent(paragraphFormattingInfo, paragraphFormattingInfoStack.Peek());
			paragraphFormattingInfoStack.Push(paragraphFormattingInfo);
		}
		protected internal virtual void ExportParagraphFormattingInfo(ParagraphFormattingInfo info, ParagraphFormattingInfo parentInfo) {
			if (options.SilverlightCompatible)
				return;
			if (info.Alignment != parentInfo.Alignment)
				DocumentContentWriter.WriteAttributeString("TextAlignment", XamlImporter.textAlignmentTable[info.Alignment]);
			if (info.SuppressHyphenation != parentInfo.SuppressHyphenation)
				DocumentContentWriter.WriteAttributeString("IsHyphenationEnabled", info.SuppressHyphenation ? "False" : "True");
			if (info.SpacingBefore != parentInfo.SpacingBefore || info.SpacingAfter != parentInfo.SpacingAfter || info.LeftIndent != parentInfo.LeftIndent || info.RightIndent != parentInfo.RightIndent)
				DocumentContentWriter.WriteAttributeString("Margin", ConvertToThickness(info.LeftIndent, info.SpacingBefore, info.RightIndent, info.SpacingAfter));
			if (info.LineSpacingType != parentInfo.LineSpacingType || info.LineSpacing != parentInfo.LineSpacing) {
				if (info.LineSpacingType == ParagraphLineSpacing.Exactly) {
					DocumentContentWriter.WriteAttributeString("LineHeight", ConvertModelUnitsToMetricString(info.LineSpacing));
					DocumentContentWriter.WriteAttributeString("LineStackingStrategy", "BlockLineHeight");
				}
				else if (info.LineSpacingType == ParagraphLineSpacing.AtLeast) {
					DocumentContentWriter.WriteAttributeString("LineHeight", ConvertModelUnitsToMetricString(info.LineSpacing));
					DocumentContentWriter.WriteAttributeString("LineStackingStrategy", "MaxHeight");
				}
				else {
					DocumentContentWriter.WriteAttributeString("LineHeight", "Auto");
					DocumentContentWriter.WriteAttributeString("LineStackingStrategy", "MaxHeight");
				}
			}
		}
		protected internal virtual void ExportFirstLineIndent(ParagraphFormattingInfo info, ParagraphFormattingInfo parentInfo) {
			if (options.SilverlightCompatible)
				return;
			if (info.FirstLineIndentType != parentInfo.FirstLineIndentType || info.FirstLineIndent != parentInfo.FirstLineIndent) {
				int value = info.FirstLineIndent;
				if (info.FirstLineIndentType == ParagraphFirstLineIndent.Hanging)
					value = -value;
				DocumentContentWriter.WriteAttributeString("TextIndent", ConvertModelUnitsToMetricString(value));
			}
		}
		protected internal virtual string ConvertToThickness(int left, int top, int right, int bottom) {
			if (left == top && top == right && right == bottom)
				return ConvertModelUnitsToMetricString(left);
			if (left == right && top == bottom)
				return ConvertModelUnitsToMetricString(left) + "," + ConvertModelUnitsToMetricString(top);
			return ConvertModelUnitsToMetricString(left) + "," + ConvertModelUnitsToMetricString(top) + "," + ConvertModelUnitsToMetricString(right) + "," + ConvertModelUnitsToMetricString(bottom);
		}
		protected internal virtual string ConvertModelUnitsToMetricString(float value) {
			float metricValue = DocumentModel.UnitConverter.ModelUnitsToPointsF(value);
			return metricValue.ToString(CultureInfo.InvariantCulture) + "pt";
		}
		#endregion
		#region Run
		protected internal override void ExportTextRun(TextRun run) {
			string runText = run.GetPlainText(PieceTable.TextBuffer);
			ExportTextRun(run, runText);
		}
		protected internal virtual void ExportTextRunCore(TextRun run, string runText) {
			if (String.IsNullOrEmpty(runText))
				return;
			DocumentContentWriter.WriteStartElement("Run");
			try {
				if (ShouldPreserveSpace(runText))
					documentContentWriter.WriteAttributeString("xml", "space", null, "preserve");
				ExportCharacterFormattingInfo(run.MergedCharacterFormatting, characterFormattingInfoStack.Peek(), true);
				DocumentContentWriter.WriteString(runText);
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		protected internal virtual bool ShouldPreserveSpace(string text) {
			return text.StartsWith(" ") || text.EndsWith(" ") || text.Contains("  ");
		}
		protected virtual void ExportTextRun(TextRun run, string runText) {
			StringBuilder sb = new StringBuilder();
			int count = runText.Length;
			for (int i = 0; i < count; i++) {
				char character = runText[i];
				if (character == Characters.LineBreak) {
					ExportTextRunCore(run, sb.ToString());
					sb.Length = 0; 
					DocumentContentWriter.WriteStartElement("LineBreak");
					DocumentContentWriter.WriteEndElement();
				}
				else {
					if (character == Characters.PageBreak)
						ExportBreak(ParagraphBreakBefore.Page, run, sb, i == 0, i + 1 < count);
					else if (character == Characters.ColumnBreak)
						ExportBreak(ParagraphBreakBefore.Column, run, sb, i == 0, i + 1 < count);
					else
						sb.Append(character);
				}
			}
			ExportTextRunCore(run, sb.ToString());
		}
		protected internal virtual void ExportBreak(ParagraphBreakBefore breakBefore, TextRun run, StringBuilder sb, bool firstRunCharacter, bool lastRunCharacter) {
			if (options.SilverlightCompatible)
				return;
			if (firstRunCharacter && IsFirstParagraphRun(run))
				return;
			ExportTextRunCore(run, sb.ToString());
			sb.Length = 0; 
			currentParagraphBreakBefore = breakBefore; 
			if (lastRunCharacter || !IsLastParagraphRun(run)) {
				WriteCurrentParagraphEndElement();
				WriteCurrentParagraphStartElement(currentParagraphBreakBefore);
			}
		}
		protected internal virtual bool IsFirstParagraphRun(TextRun run) {
			return Object.ReferenceEquals(run, PieceTable.Runs[currentParagraph.FirstRunIndex]);
		}
		protected internal virtual bool IsLastParagraphRun(TextRun run) {
			if (currentParagraph.Length <= 1)
				return false;
			return Object.ReferenceEquals(run, PieceTable.Runs[currentParagraph.LastRunIndex - 1]);
		}
		protected internal virtual void ExportCharacterFormattingInfo(CharacterFormattingInfo info, CharacterFormattingInfo parentInfo, bool exportTextDecorations) {
			if (info.FontName != parentInfo.FontName)
				DocumentContentWriter.WriteAttributeString("FontFamily", info.FontName);
			if (info.DoubleFontSize != parentInfo.DoubleFontSize)
				DocumentContentWriter.WriteAttributeString("FontSize", (info.DoubleFontSize / 2f).ToString(CultureInfo.InvariantCulture));
			if (info.FontBold != parentInfo.FontBold) {
				if (info.FontBold)
					DocumentContentWriter.WriteAttributeString("FontWeight", "Bold");
				else
					DocumentContentWriter.WriteAttributeString("FontWeight", "Normal");
			}
			if (info.FontItalic != parentInfo.FontItalic) {
				if (info.FontItalic)
					DocumentContentWriter.WriteAttributeString("FontStyle", "Italic");
				else
					DocumentContentWriter.WriteAttributeString("FontStyle", "Normal");
			}
			if (info.ForeColor != parentInfo.ForeColor && !DXColor.IsTransparentOrEmpty(info.ForeColor))
				DocumentContentWriter.WriteAttributeString("Foreground", ToHtml(info.ForeColor));
			if (!options.SilverlightCompatible) {
				if (info.BackColor != parentInfo.BackColor && !DXColor.IsTransparentOrEmpty(info.BackColor))
					DocumentContentWriter.WriteAttributeString("Background", ToHtml(info.ForeColor));
			}
			if (exportTextDecorations)
				ExportCharacterFormattingInfoDecorations(info, parentInfo);
		}
		static string ToHtml(Color c) {
			return String.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B);
		}
		protected internal virtual void ExportCharacterFormattingInfoDecorations(CharacterFormattingInfo info, CharacterFormattingInfo parentInfo) {
			string decorations = String.Empty;
			if (info.FontUnderlineType != parentInfo.FontUnderlineType) {
				System.Diagnostics.Debug.Assert(info.FontUnderlineType != UnderlineType.None);
				decorations += "Underline";
			}
			if (!options.SilverlightCompatible) {
				if (info.FontStrikeoutType != parentInfo.FontStrikeoutType) {
					System.Diagnostics.Debug.Assert(info.FontStrikeoutType != StrikeoutType.None);
					if (!String.IsNullOrEmpty(decorations))
						decorations += ",";
					decorations += "Strikethrough";
				}
			}
			if (!String.IsNullOrEmpty(decorations))
				DocumentContentWriter.WriteAttributeString("TextDecorations", decorations);
			if (!options.SilverlightCompatible) {
				if (info.Script != parentInfo.Script) {
					switch (info.Script) {
						case CharacterFormattingScript.Normal:
							DocumentContentWriter.WriteAttributeString("BaselineAlignment", "Normal");
							break;
						case CharacterFormattingScript.Superscript:
							DocumentContentWriter.WriteAttributeString("BaselineAlignment", "Superscript");
							break;
						case CharacterFormattingScript.Subscript:
							DocumentContentWriter.WriteAttributeString("BaselineAlignment", "Subscript");
							break;
					}
				}
			}
		}
		#endregion
		#region Hyperlink
		protected internal override void ExportFieldCodeStartRun(FieldCodeStartRun run) {
			base.ExportFieldCodeStartRun(run);
		}
		protected internal override void ExportFieldCodeEndRun(FieldCodeEndRun run) {
			base.ExportFieldCodeEndRun(run);
			HyperlinkInfo hyperlinkInfo = GetHyperlinkInfo(run);
			if (hyperlinkInfo == null)
				return;
			DocumentContentWriter.WriteStartElement("Hyperlink");
			if (!String.IsNullOrEmpty(hyperlinkInfo.NavigateUri))
				DocumentContentWriter.WriteAttributeString("NavigateUri", hyperlinkInfo.NavigateUri);
			if (!String.IsNullOrEmpty(hyperlinkInfo.Target))
				DocumentContentWriter.WriteAttributeString("TargetName", hyperlinkInfo.Target);
		}
		protected internal override void ExportFieldResultEndRun(FieldResultEndRun run) {
			if (GetHyperlinkInfo(run) == null)
				return;
			DocumentContentWriter.WriteEndElement();
		}
		HyperlinkInfo GetHyperlinkInfo(TextRun run) {
			RunIndex runIndex = run.GetRunIndex();
			Field field = PieceTable.FindFieldByRunIndex(runIndex);
			Debug.Assert(field != null);
			HyperlinkInfo hyperlinkInfo = null;
			if (PieceTable.HyperlinkInfos.TryGetHyperlinkInfo(field.Index, out hyperlinkInfo))
				return hyperlinkInfo;
			return null;
		}
		#endregion
	}
	public enum ParagraphBreakBefore {
		None,
		Page,
		Column
	}
}
