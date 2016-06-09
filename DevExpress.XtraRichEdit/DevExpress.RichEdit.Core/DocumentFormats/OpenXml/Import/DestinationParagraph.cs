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
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Office.History;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using System.Globalization;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region ParagraphDestination
	public class ParagraphDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			FillElementHandlerTable(result);
			return result;
		}
		protected static void FillElementHandlerTable(ElementHandlerTable handlerTable) {
			handlerTable.Add("pPr", OnParagraphProperties);
			handlerTable.Add("r", OnRun);
			handlerTable.Add("pict", OnPicture);
			handlerTable.Add("fldSimple", OnFieldSimple);
			handlerTable.Add("hyperlink", OnHyperlink);
			handlerTable.Add("fldChar", OnComplexFieldMarker);
			handlerTable.Add("instrText", OnFieldInstruction); 
			handlerTable.Add("bookmarkStart", OnBookmarkStart);
			handlerTable.Add("bookmarkEnd", OnBookmarkEnd);
			handlerTable.Add("permStart", OnRangePermissionStart);
			handlerTable.Add("permEnd", OnRangePermissionEnd);
			handlerTable.Add("commentRangeStart", OnCommentStart);
			handlerTable.Add("commentRangeEnd", OnCommentEnd);
			handlerTable.Add("smartTag", OnSmartTag);
			handlerTable.Add("sdt", OnStructuredDocument);
			handlerTable.Add("customXml", OnCustomXml);
		}
		bool shouldInsertSection;
		int listLevelIndex = Int32.MinValue;
		int numberingId = Int32.MinValue;
		public ParagraphDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
			importer.Position.ParagraphFormatting.ReplaceInfo(importer.DocumentModel.Cache.ParagraphFormattingInfoCache.DefaultItem, new ParagraphFormattingOptions(ParagraphFormattingOptions.Mask.UseNone));
			importer.Position.ParagraphStyleIndex = 0;
			importer.Position.ParagraphMarkCharacterFormatting.ReplaceInfo(importer.DocumentModel.Cache.CharacterFormattingInfoCache.DefaultItem, new CharacterFormattingOptions(CharacterFormattingOptions.Mask.UseNone));
			importer.Position.ParagraphMarkCharacterStyleIndex = 0;
			importer.Position.ParagraphTabs.Clear();
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public bool ShouldInsertSection { get { return shouldInsertSection; } set { shouldInsertSection = value; } }
		public int ListLevelIndex { get { return listLevelIndex; } set { listLevelIndex = value; } }
		public int NumberingId { get { return numberingId; } set { numberingId = value; } }
		static ParagraphDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (ParagraphDestination)importer.PeekDestination();
		}
		protected internal virtual ParagraphPropertiesDestination CreateParagraphPropertiesDestination() {
			return new ParagraphPropertiesDestination(Importer, this, Importer.Position.ParagraphFormatting, Importer.Position.ParagraphTabs);
		}
		protected static Destination OnParagraphProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			importer.Position.ParagraphFormatting.ReplaceInfo(importer.DocumentModel.Cache.ParagraphFormattingInfoCache.DefaultItem, new ParagraphFormattingOptions(ParagraphFormattingOptions.Mask.UseNone));
			return GetThis(importer).CreateParagraphPropertiesDestination();
		}
		static Destination OnRun(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateRunDestination();
		}
		protected static Destination OnPicture(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new InlinePictureDestination(importer);
		}
		protected static Destination OnFieldSimple(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new FieldSimpleDestination(importer);
		}
		protected static Destination OnHyperlink(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new HyperlinkDestination(importer);
		}
		protected static Destination OnComplexFieldMarker(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new FieldCharDestination(importer);
		}
		protected static Destination OnFieldInstruction(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TextDestination(importer);
		}
		protected static Destination OnBookmarkStart(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateBookmarkStartElementDestination(reader);
		}
		protected static Destination OnBookmarkEnd(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateBookmarkEndElementDestination(reader);
		}
		protected static Destination OnRangePermissionStart(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new RangePermissionStartElementDestination(importer);
		}
		protected static Destination OnRangePermissionEnd(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new RangePermissionEndElementDestination(importer);
		}
		protected static Destination OnCommentStart(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new CommentStartElementDestination(importer);
		}
		protected static Destination OnCommentEnd(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new CommentEndElementDestination(importer);
		}
		protected static Destination OnSmartTag(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SmartTagDestination(importer);
		}
		static Destination OnStructuredDocument(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new StructuredDocumentDestination(importer);
		}
		static Destination OnCustomXml(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new CustomXmlDestination(importer);
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string val = reader.GetAttribute("paraId", Importer.W14NamespaceConst);
			Importer.ParaId = Importer.GetIntegerValue(val, NumberStyles.HexNumber, Int32.MinValue);
		}
		public override void ProcessElementClose(XmlReader reader) {
			Importer.Position.CharacterFormatting.CopyFrom(Importer.Position.ParagraphMarkCharacterFormatting);
			Importer.Position.CharacterStyleIndex = Importer.Position.ParagraphMarkCharacterStyleIndex;
			if (ShouldInsertSection)
				InsertSection();
			else {
				InsertParagraphCore();
			}
			Importer.Position.CharacterStyleIndex = 0;
		}
		void InsertParagraphCore() {
			if (SuppressInsertParagraph()) {
				PieceTable.InsertTextCore(Importer.Position, " ");
				return;
			}
			Paragraph paragraph = InsertParagraph();
			if (DocumentFormatsHelper.ShouldInsertNumbering(DocumentModel))
				AddNumbering(paragraph);
		}
		protected internal virtual void AddNumbering(Paragraph paragraph) {
			listLevelIndex = Math.Max(0, listLevelIndex);
			if (NumberingId != Int32.MinValue) {
				OpenXmlNumberingListInfo listInfo = Importer.ListInfos.FindById(NumberingId);
				if (!DocumentFormatsHelper.ShouldInsertMultiLevelNumbering(DocumentModel))
					ListLevelIndex = 0;
				if (listInfo != null)
					PieceTable.AddNumberingListToParagraph(paragraph, listInfo.ListIndex, ListLevelIndex);
				else if (new NumberingListIndex(NumberingId) == NumberingListIndex.NoNumberingList && paragraph.ParagraphStyle.GetNumberingListIndex() >= NumberingListIndex.MinValue) {
					PieceTable.AddNumberingListToParagraph(paragraph, NumberingListIndex.NoNumberingList, ListLevelIndex);
					if (!paragraph.ParagraphProperties.UseFirstLineIndentType)
						paragraph.FirstLineIndentType = ParagraphFirstLineIndent.None;
					if (!paragraph.ParagraphProperties.UseFirstLineIndent)
						paragraph.FirstLineIndent = 0;
					if (!paragraph.ParagraphProperties.UseLeftIndent)
						paragraph.LeftIndent = 0;
				}
			}
		}
		protected internal virtual bool SuppressInsertParagraph() {
			return !DocumentModel.DocumentCapabilities.ParagraphsAllowed;
		}
		protected internal virtual Paragraph InsertParagraph() {
			ParagraphIndex paragraphIndex = Importer.Position.ParagraphIndex;
			PieceTable.InsertParagraphCore(Importer.Position);
			ApplyParagraphProperties(paragraphIndex);
			return PieceTable.Paragraphs[paragraphIndex];
		}
		protected internal virtual void InsertSection() {
			Debug.Assert(PieceTable.IsMain);
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				ParagraphIndex paragraphIndex = Importer.Position.ParagraphIndex;
				PieceTable.InsertSectionParagraphCore(Importer.Position);
				ApplyParagraphProperties(paragraphIndex);
				DocumentModel.SafeEditor.PerformInsertSectionCore(paragraphIndex);
				Importer.CurrentSection = DocumentModel.Sections.Last;
				Importer.CurrentSection.Reset();
			}
		}
		protected internal virtual void ApplyParagraphProperties(ParagraphIndex paragraphIndex) {
			Paragraph paragraph = PieceTable.Paragraphs[paragraphIndex];
			paragraph.ParagraphStyleIndex = Importer.Position.ParagraphStyleIndex;
			paragraph.ParagraphProperties.CopyFrom(Importer.Position.ParagraphFormatting);
			if (Importer.Position.ParagraphFrameFormatting.Options.GetHashCode() != 0 && paragraph.FrameProperties == null && DocumentModel.DocumentCapabilities.ParagraphFramesAllowed) {
				paragraph.FrameProperties = new FrameProperties(paragraph);
				paragraph.FrameProperties.CopyFrom(Importer.Position.ParagraphFrameFormatting);
			}
			Importer.Position.ParagraphFrameFormatting.ReplaceInfo(Importer.DocumentModel.Cache.ParagraphFrameFormattingInfoCache.DefaultItem, new ParagraphFrameFormattingOptions(ParagraphFrameFormattingOptions.Mask.UseNone));
			paragraph.SetOwnTabs(Importer.Position.ParagraphTabs);
		}
	}
	#endregion
	#region SmartTagDestination
	public class SmartTagDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("smartTag", OnSmartTag);
			result.Add("r", OnRun);
			return result;
		}
		public SmartTagDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected static Destination OnSmartTag(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SmartTagDestination(importer);
		}
		protected static Destination OnRun(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateRunDestination();
		}
	}
	#endregion
}
