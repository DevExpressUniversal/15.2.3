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
using System.Text;
using System.Xml;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Export.OpenXml;
using System.Collections.Generic;
using System.Globalization;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.Office.NumberConverters;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Export.WordML {
	public class WordMLExporter : WordProcessingMLBaseExporter, IWordMLExporter {
		public const string WordProcessingNamespaceConst = "http://schemas.microsoft.com/office/word/2003/wordml";
		public const string AMLNamespaceConst = "http://schemas.microsoft.com/aml/2001/core";
		#region NumberingFormatTable
		static readonly Dictionary<NumberingFormat, int> listNumberingFormatTable = CreateListNumberingFormatTable();
		protected static Dictionary<NumberingFormat, int> CreateListNumberingFormatTable() {
			Dictionary<NumberingFormat, int> result = new Dictionary<NumberingFormat, int>();
			result.Add(NumberingFormat.Decimal, 0);
			result.Add(NumberingFormat.AIUEOHiragana, 12);
			result.Add(NumberingFormat.AIUEOFullWidthHiragana, 20);
			result.Add(NumberingFormat.ArabicAbjad, 48);
			result.Add(NumberingFormat.ArabicAlpha, 46);
			result.Add(NumberingFormat.Bullet, 23);
			result.Add(NumberingFormat.CardinalText, 6);
			result.Add(NumberingFormat.Chicago, 9);
			result.Add(NumberingFormat.ChineseCounting, 37);
			result.Add(NumberingFormat.ChineseCountingThousand, 39);
			result.Add(NumberingFormat.ChineseLegalSimplified, 38);
			result.Add(NumberingFormat.Chosung, 25);
			result.Add(NumberingFormat.DecimalEnclosedCircle, 18);
			result.Add(NumberingFormat.DecimalEnclosedCircleChinese, 28);
			result.Add(NumberingFormat.DecimalEnclosedFullstop, 26);
			result.Add(NumberingFormat.DecimalEnclosedParenthses, 27);
			result.Add(NumberingFormat.DecimalFullWidth, 14);
			result.Add(NumberingFormat.DecimalFullWidth2, 19);
			result.Add(NumberingFormat.DecimalHalfWidth, 15);
			result.Add(NumberingFormat.DecimalZero, 22);
			result.Add(NumberingFormat.Ganada, 24);
			result.Add(NumberingFormat.Hebrew1, 45);
			result.Add(NumberingFormat.Hebrew2, 47);
			result.Add(NumberingFormat.Hex, 8);
			result.Add(NumberingFormat.HindiConsonants, 50);
			result.Add(NumberingFormat.HindiDescriptive, 52);
			result.Add(NumberingFormat.HindiNumbers, 51);
			result.Add(NumberingFormat.HindiVowels, 49);
			result.Add(NumberingFormat.IdeographDigital, 10);
			result.Add(NumberingFormat.IdeographEnclosedCircle, 29);
			result.Add(NumberingFormat.IdeographLegalTraditional, 34);
			result.Add(NumberingFormat.IdeographTraditional, 30);
			result.Add(NumberingFormat.IdeographZodiac, 31);
			result.Add(NumberingFormat.IdeographZodiacTraditional, 32);
			result.Add(NumberingFormat.Iroha, 13);
			result.Add(NumberingFormat.IrohaFullWidth, 21);
			result.Add(NumberingFormat.JapaneseCounting, 11);
			result.Add(NumberingFormat.JapaneseDigitalTenThousand, 17);
			result.Add(NumberingFormat.JapaneseLegal, 16);
			result.Add(NumberingFormat.KoreanCounting, 42);
			result.Add(NumberingFormat.KoreanDigital, 41);
			result.Add(NumberingFormat.KoreanDigital2, 44);
			result.Add(NumberingFormat.KoreanLegal, 43);
			result.Add(NumberingFormat.LowerLetter, 4);
			result.Add(NumberingFormat.LowerRoman, 2);
			result.Add(NumberingFormat.NumberInDash, 57);
			result.Add(NumberingFormat.Ordinal, 5);
			result.Add(NumberingFormat.OrdinalText, 7);
			result.Add(NumberingFormat.RussianLower, 58);
			result.Add(NumberingFormat.RussianUpper, 59);
			result.Add(NumberingFormat.TaiwaneseCounting, 33);
			result.Add(NumberingFormat.TaiwaneseCountingThousand, 35);
			result.Add(NumberingFormat.TaiwaneseDigital, 36);
			result.Add(NumberingFormat.ThaiDescriptive, 55);
			result.Add(NumberingFormat.ThaiLetters, 53);
			result.Add(NumberingFormat.ThaiNumbers, 54);
			result.Add(NumberingFormat.UpperLetter, 3);
			result.Add(NumberingFormat.UpperRoman, 1);
			result.Add(NumberingFormat.VietnameseDescriptive, 56);
			return result;
		}
		#endregion
		readonly Dictionary<OfficeImage, string> exportedImageTable = new Dictionary<OfficeImage, string>();
		public WordMLExporter(DocumentModel documentModel, DocumentExporterOptions options)
			: base(documentModel, options) {
		}
		#region Properties
		public new WordMLDocumentExporterOptions Options { get { return (WordMLDocumentExporterOptions)base.Options; } }
		protected override string WordProcessingNamespace { get { return WordProcessingNamespaceConst; } }
		protected override string WordProcessingPrefix { get { return "w"; } }
		protected internal string AMLNamespace { get { return AMLNamespaceConst; } }
		protected internal string AMLPrefix { get { return "aml"; } }
		protected internal override Dictionary<OfficeImage, string> ExportedImageTable { get { return exportedImageTable; } }
		protected override Dictionary<FloatingObjectHorizontalPositionType, WordProcessingMLValue> HorizontalPositionTypeAttributeTable { get { return DevExpress.XtraRichEdit.Import.WordML.InlinePictureDestination.WordMLHorizontalPositionTypeAttributeTable; } }
		protected override Dictionary<FloatingObjectVerticalPositionType, WordProcessingMLValue> VerticalPositionTypeAttributeTable { get { return DevExpress.XtraRichEdit.Import.WordML.InlinePictureDestination.WordMLVerticalPositionTypeAttributeTable; } }
		#endregion
		public new virtual string Export() {
			ChunkedStringBuilder result = new ChunkedStringBuilder();
			using (ChunkedStringBuilderWriter writer = new ChunkedStringBuilderWriter(result)) {
				writer.SetEncoding(DXEncoding.UTF8NoByteOrderMarks);
				Export(writer);
				writer.Flush();
			}
			return result.ToString();
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
		protected internal void GenerateDocumentContent() {
			ExportedImageTable.Clear();
			DocumentContentWriter.WriteProcessingInstruction("mso-application", @"progid=""Word.Document""");
			DocumentContentWriter.WriteStartElement(WordProcessingPrefix, "wordDocument", WordProcessingNamespace);
			try {
				DocumentContentWriter.WriteAttributeString("xml", "space", string.Empty, "preserve");				
				ExportNumbering();
				ExportStyles();
				ExportSettings();
				ExportBackgroundPictureInformation();
				DocumentContentWriter.WriteStartElement(WordProcessingPrefix, "body", WordProcessingNamespace);
				try {
					base.Export();
				}
				finally {
					WriteWpEndElement();
				}
			}
			finally {
				WriteWpEndElement();
			}
		}
		#region Settings
		private void ExportSettings() {
			ExportSettingsCore();
		}	   
		protected internal virtual void ExportSettingsCore() {			
			WordProcessingMLValue val = new WordProcessingMLValue("settings", "docPr");
			WriteWpStartElement(GetWordProcessingMLValue(val));
			try {
				WriteWpStringValue("view", "print");
				WriteSettingsCore();
			} finally {
				WriteWpEndElement();
			}
		}
		#endregion
		protected internal virtual void ExportBackgroundPictureInformation() {
			DocumentProperties documentProperties = DocumentModel.DocumentProperties;
			if (!ShouldExportBackgroundPictureInformation(documentProperties))
				return;
			WriteWpStartElement("bgPict");
			try {
				ExportDocumentBackground(documentProperties.PageBackColor);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual bool ShouldExportBackgroundPictureInformation(DocumentProperties properties) {
			return properties.PageBackColor != DXColor.Empty;
		}
		protected internal virtual void ExportDocumentBackground(Color pageBackColor) {
			if (pageBackColor == DXColor.Empty)
				return;
			WriteWpStartElement("background");
			try {
				WriteWpStringAttr("bgcolor", ConvertColorToString(pageBackColor));
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal override void ExportDocumentProtectionSettingsCore() {
			DocumentProtectionProperties properties = DocumentModel.ProtectionProperties;
			byte[] hash = properties.Word2003PasswordHash;
			string value;
			if (hash == null)
				value = "00000000";
			else {
				value = String.Format("{0:X2}", hash[0]);
				value += String.Format("{0:X2}", hash[1]);
				value += String.Format("{0:X2}", hash[2]);
				value += String.Format("{0:X2}", hash[3]);
			}
			WriteWpStringAttr("unprotectPassword", value);
		}
		private void ExportStyles() {
			ExportStylesCore();
		}
		private void ExportNumbering() {
			ExportNumberingCore();
		}
		protected internal override void ExportFieldCodeEndRun(FieldCodeEndRun run) {
			base.ExportFieldCodeEndRun(run);
		}
		protected internal override void ExportFieldResultEndRun(FieldResultEndRun run) {
			base.ExportFieldResultEndRun(run);
		} 
		protected internal override void ExportFieldCodeStartRun(FieldCodeStartRun run) {
			base.ExportFieldCodeStartRun(run);
		}
		protected internal override void ExportInlineObjectRun(InlineObjectRun run) {
			base.ExportInlineObjectRun(run);
		}
		protected internal override void ExportParagraphListReference(Paragraph paragraph) {
			WriteWpStartElement("listPr");
			try {
				WriteWpIntValue("ilvl", paragraph.GetListLevelIndex());
				WriteWpIntValue("ilfo", GetNumberingListIndexForExport(paragraph.GetOwnNumberingListIndex()));
			}
			finally {
				WriteWpEndElement();
			}
		}
		#region Section
		protected internal override void ExportSection(Section section) {
			DocumentContentWriter.WriteStartElement("wx", "sect", "http://schemas.microsoft.com/office/word/2003/auxHint");
			try {
				base.ExportSection(section);
				if (section.LastParagraphIndex == new ParagraphIndex(PieceTable.Paragraphs.Count - 1)) 
					ExportSectionProperties(CurrentSection); 
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		#endregion
		#region Styles
		protected internal override  void ExportDocumentDefaults() {
		}
		protected internal override void ExportDocumentCharacterDefaults() {
		}
		protected internal override void ExportDocumentParagraphDefaults() {
		}
		protected internal override void ExportParagraphStyleListReference(NumberingListIndex numberingListIndex, int listLevelIndex) {
			WriteWpStartElement("listPr");
			try {
				if(numberingListIndex >= NumberingListIndex.MinValue || numberingListIndex == NumberingListIndex.NoNumberingList)
					WriteWpIntValue("ilfo", GetNumberingListIndexForExport(numberingListIndex));
				if (listLevelIndex > 0)
					WriteWpIntValue("ilvl", listLevelIndex);
			}
			finally {
				WriteWpEndElement();
			}
		}
		#endregion
		#region Numbering
		protected internal override void ExportAbstractNumberingList(AbstractNumberingList list, int id) {
			WriteWpStartElement("listDef");
			try {
				WriteWpIntAttr("listDefId", id);
				WriteWpStringValue("lsid", ConvertToHexString(list.Id));
				WriteWpStringValue("plt", ConvertNumberingListType(NumberingListHelper.GetListType(list)));
				ExportLevels(list.Levels);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal override void ExportNumberingList(NumberingList list, int id) {
			WriteWpStartElement("list");
			try {
				WriteWpIntAttr("ilfo", id + 1);
				WriteWpIntValue("ilst", ((IConvertToInt<AbstractNumberingListIndex>)list.AbstractNumberingListIndex).ToInt());
				ExportOverrideLevels(list.Levels);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal override void ExportNumberFormatValue(ListLevelProperties properties) {
			if (properties.Format != NumberingFormat.None)
				WriteWpIntValue("nfc", ConvertListNumberFormat(properties.Format));
		}
		#endregion
		#region FloatingObjectProperties
		protected internal override void ExportFloatingObjectAnchorRun(FloatingObjectAnchorRun run) {
			base.ExportFloatingObjectAnchorRun(run);
		}
		protected internal override void ExportImageReference(FloatingObjectAnchorRun run) {
			PictureFloatingObjectContent pictureContent = run.Content as PictureFloatingObjectContent;
			TextBoxFloatingObjectContent textBoxContent = run.Content as TextBoxFloatingObjectContent;
			FloatingObjectProperties floatingObjectProperties = run.FloatingObjectProperties;
			if (pictureContent == null && textBoxContent == null)
				return;
			int id = DrawingElementId;
			string namePrefix = pictureContent != null ? "Picture" : "Text Box";
			string name = GenerateFloatingObjectName(run.Name, namePrefix, id);
			IncrementDrawingElementId();
			WriteFloatingObjectPict(floatingObjectProperties, textBoxContent, pictureContent, run.Shape, name);
		}
		#endregion
		#region Pictures
		protected override void ExportImageReference(InlinePictureRun run) {
			WriteWpStartElement("pict");
			try {
				string imagePath = ExportBinData(run.Image);
				DocumentContentWriter.WriteStartElement(VMLPrefix, "shape", VMLNamespace);
				try {
					float finalWidth = UnitConverter.ModelUnitsToPointsF(run.ActualSizeF.Width);
					float finalHeight = UnitConverter.ModelUnitsToPointsF(run.ActualSizeF.Height);
					string imageStyle = String.Format(CultureInfo.InvariantCulture, "width:{0}pt;height:{1}pt", finalWidth, finalHeight);
					DocumentContentWriter.WriteAttributeString("style", imageStyle);
					ExportImageData(imagePath);
				}
				finally {
					DocumentContentWriter.WriteEndElement();
				}
			}
			finally {
				WriteWpEndElement();
			}
		}
		#endregion
		#region Bookmarks
		protected internal override void ExportBookmarkStart(Bookmark bookmark) {
			DocumentContentWriter.WriteStartElement(AMLPrefix, "annotation", AMLNamespace);
			try {
				DocumentContentWriter.WriteAttributeString(AMLPrefix, "id", AMLNamespace, GenerateBookmarkId(bookmark));
				WriteWpStringAttr("type", "Word.Bookmark.Start");
				WriteWpStringAttr("name", bookmark.Name);
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		protected internal override void ExportBookmarkEnd(Bookmark bookmark) {
			DocumentContentWriter.WriteStartElement(AMLPrefix, "annotation", AMLNamespace);
			try {
				DocumentContentWriter.WriteAttributeString(AMLPrefix, "id", AMLNamespace, GenerateBookmarkId(bookmark));
				WriteWpStringAttr("type", "Word.Bookmark.End");
				WriteWpStringAttr("name", bookmark.Name);
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		string GenerateBookmarkId(Bookmark bookmark) {
			return "B" + PieceTable.Bookmarks.IndexOf(bookmark);
		}
		#endregion
		#region Comments
		protected internal override void ExportCommentStart(Comment comment) {
			if (comment.Start == comment.End)
				return;
			DocumentContentWriter.WriteStartElement(AMLPrefix, "annotation", AMLNamespace);
			try {
				DocumentContentWriter.WriteAttributeString(AMLPrefix, "id", AMLNamespace, Convert.ToString(comment.Index));
				WriteWpStringAttr("type", "Word.Comment.Start");
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		protected internal override void ExportCommentEnd(Comment comment) {
			if (comment.Start != comment.End) {
				DocumentContentWriter.WriteStartElement(AMLPrefix, "annotation", AMLNamespace);
				try {
					DocumentContentWriter.WriteAttributeString(AMLPrefix, "id", AMLNamespace, Convert.ToString(comment.Index));
					WriteWpStringAttr("type", "Word.Comment.End");
				}
				finally {
					DocumentContentWriter.WriteEndElement();
				}
			}
			ExportHeaderCommentContent(comment);
		}
		void ExportHeaderCommentContent(Comment comment) {
			WriteWpStartElement("r");
			try {
				DocumentContentWriter.WriteStartElement(AMLPrefix, "annotation", AMLNamespace);
				try {
					DocumentContentWriter.WriteAttributeString(AMLPrefix, "id", AMLNamespace, Convert.ToString(comment.Index));
					WriteWpStringAttr("type", "Word.Comment");
					DocumentContentWriter.WriteAttributeString(AMLPrefix, "author", AMLNamespace, comment.Author);
						DocumentContentWriter.WriteAttributeString(AMLPrefix, "createdate", AMLNamespace, DateTimeUtils.ToDateTimeISO8601(comment.Date));
					WriteWpStringAttr("initials", comment.Name);
					ExportCommentContent(comment);
				}
				finally {
					DocumentContentWriter.WriteEndElement();
				}
			}
			finally {
				WriteWpEndElement();
			}
		}
		void ExportCommentContent(Comment comment) {
			DocumentContentWriter.WriteStartElement(AMLPrefix, "content", AMLNamespace);
			try {
				PerformExportPieceTable(comment.Content.PieceTable, ExportPieceTable);
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		#endregion
		#region Headers/Footers
		protected internal override void ExportFirstPageHeader(SectionHeader sectionHeader, bool linkedToPrevious) {
			if (linkedToPrevious)
				return;
			WriteWpStartElement("hdr");
			try {
				WriteWpStringAttr("type", "first");
				base.ExportFirstPageHeader(sectionHeader, linkedToPrevious);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal override void ExportOddPageHeader(SectionHeader sectionHeader, bool linkedToPrevious) {
			if (linkedToPrevious)
				return;
			WriteWpStartElement("hdr");
			try {
				WriteWpStringAttr("type", "odd");
				base.ExportOddPageHeader(sectionHeader, linkedToPrevious);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal override void ExportEvenPageHeader(SectionHeader sectionHeader, bool linkedToPrevious) {
			if (linkedToPrevious)
				return;
			WriteWpStartElement("hdr");
			try {
				WriteWpStringAttr("type", "even");
				base.ExportEvenPageHeader(sectionHeader, linkedToPrevious);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal override void ExportFirstPageFooter(SectionFooter sectionFooter, bool linkedToPrevious) {
			if (linkedToPrevious)
				return;
			WriteWpStartElement("ftr");
			try {
				WriteWpStringAttr("type", "first");
				base.ExportFirstPageFooter(sectionFooter, linkedToPrevious);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal override void ExportOddPageFooter(SectionFooter sectionFooter, bool linkedToPrevious) {
			if (linkedToPrevious)
				return;
			WriteWpStartElement("ftr");
			try {
				WriteWpStringAttr("type", "odd");
				base.ExportOddPageFooter(sectionFooter, linkedToPrevious);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal override void ExportEvenPageFooter(SectionFooter sectionFooter, bool linkedToPrevious) {
			if (linkedToPrevious)
				return;
			WriteWpStartElement("ftr");
			try {
				WriteWpStringAttr("type", "even");
				base.ExportEvenPageFooter(sectionFooter, linkedToPrevious);
			}
			finally {
				WriteWpEndElement();
			}
		}
		#endregion
		protected internal override void ExportTableCellPropertiesVerticalMerging(MergingState verticalMerging) {
			WriteWpStringValue("vmerge", ConvertMergingState(verticalMerging));
		}
		protected internal override string ConvertBoolToString(bool value) {
			return value ? "on" : "off";
		}
		private int ConvertListNumberFormat(NumberingFormat value) {
			int result;
			if (listNumberingFormatTable.TryGetValue(value, out result))
				return result;
			else
				return listNumberingFormatTable[NumberingFormat.Decimal];
		}
		protected internal override string GetWordProcessingMLValue(WordProcessingMLValue value) {
			return value.WordMLValue;
		}	   
	}
}
