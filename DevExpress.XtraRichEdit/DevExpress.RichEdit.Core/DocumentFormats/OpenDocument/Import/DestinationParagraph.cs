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
using System.Drawing;
using System.Xml;
using DevExpress.XtraRichEdit.Export.OpenDocument;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Import.OpenDocument {
	#region ParagraphDestination
	public class ParagraphDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("span", OnSpanDestination);
			result.Add("note", OnNoteDestination);
			result.Add("index-entry-span", OnSpanDestination);
			result.Add("line-break", OnLineBreakDestination);
			result.Add("a", OnHyperlinkDestination);
			result.Add("frame", OnFrameDestination);
			result.Add("rect", OnFrameDestination);
			result.Add("custom-shape", OnFrameDestination);
			result.Add("tab", OnTabDestination);
			result.Add("bookmark", OnBookmarkDestination);
			result.Add("bookmark-start", OnBookmarkStartDestination);
			result.Add("bookmark-end", OnBookmarkEndDestination);
			result.Add("annotation", OnAnnotationDestination);
			result.Add("s", OnSpacesDestination);
			FieldHandlers.AddFieldHandlers(result);
			return result;
		}
		public ParagraphDestination(OpenDocumentTextImporter importer)
			: base(importer) {
			Importer.InputPosition.CharacterFormatting.ReplaceInfo(importer.DocumentModel.Cache.CharacterFormattingInfoCache.DefaultItem, new CharacterFormattingOptions(CharacterFormattingOptions.Mask.UseNone));
			Importer.InputPosition.CharacterStyleIndex = 0;
			Importer.InputPosition.CurrentParagraphInfo.Reset(importer.DocumentModel);
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		internal ParagraphInfo CurrentParagraphInfo { get { return Importer.InputPosition.CurrentParagraphInfo; } }
		internal SectionInfo CurrentSectionInfo { get { return Importer.InputPosition.CurrentSectionInfo; } }
		internal NumberingListReference CurrentListReference { get { return Importer.InputPosition.CurrentListReference; } }
		static Destination OnFrameDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new FrameDestination(importer);
		}
		static Destination OnSpanDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new SpanDestination(importer);
		}
		static Destination OnNoteDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new NoteDestination(importer);
		}
		static Destination OnTabDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new TabMarkDestination(importer);
		}
		static Destination OnHyperlinkDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new HyperlinkDestination(importer);
		}
		static Destination OnLineBreakDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new LineBreakDestination(importer);
		}
		static Destination OnBookmarkDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new BookmarkElementDestination(importer);
		}
		static Destination OnBookmarkStartDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new BookmarkStartElementDestination(importer);
		}
		static Destination OnBookmarkEndDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new BookmarkEndElementDestination(importer);
		}
		static Destination OnAnnotationDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new AnnotationDestination(importer);
		}
		static Destination OnSpacesDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new SpacesDestination(importer);
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ApplyParagraphStyle(reader);
			if (ParagraphDestination.ShouldInsertSection(DocumentModel, Importer))
				InsertSection();
			if (ShouldApplySectionProperties())
				ApplyCurrentSectionProperties();
			InsertBreakBeforeParagraph();
			CurrentSectionInfo.SectionTagOpened = false;
			CurrentSectionInfo.SectionTagClosed = false;
		}
		protected internal virtual void ApplyParagraphStyle(XmlReader reader) {
			string styleName = ImportHelper.GetTextStringAttribute(reader, "style-name");
			ParagraphAutoStyleInfo paragraphAutoStyleInfo;
			if (Importer.ParagraphAutoStyles.TryGetValue(styleName, out paragraphAutoStyleInfo))
				ApplyParagraphAutoStyle(paragraphAutoStyleInfo);
			else
				ApplyParagraphCommonStyle(styleName);
		}
		internal virtual void InsertBreakBeforeParagraph() {
			ParagraphBreakType breakType = CurrentParagraphInfo.Breaks.BreakBefore;
			if (ShouldInsertBreakBeforeParagraph(breakType))
				InsertBreak(breakType);
		}
		internal virtual bool ShouldInsertBreakBeforeParagraph(ParagraphBreakType breakType) {
			switch (breakType) {
				case ParagraphBreakType.Column:
					return CanInsertColumnBreak();
				case ParagraphBreakType.Page:
					return CanInsertPageBreak();
				default:
					return false;
			}
		}
		internal virtual void InsertBreakAfterParagraph() {
			ParagraphBreakType breakType = CurrentParagraphInfo.Breaks.BreakAfter;
			InsertBreak(breakType);
		}
		internal virtual void InsertBreak(ParagraphBreakType breakType) {
			switch (breakType) {
				case ParagraphBreakType.Column:
					InsertBreak(Characters.ColumnBreak);
					break;
				case ParagraphBreakType.Page:
					InsertBreak(Characters.PageBreak);
					break;
			}
		}
		internal virtual bool CanInsertPageBreak() {
			return !CurrentParagraphInfo.HasMasterPage && !CurrentParagraphInfo.IsFirstAtDocument;
		}
		internal virtual bool CanInsertColumnBreak() {
			return CanInsertPageBreak();
		}
		public static bool ShouldInsertSection(DocumentModel documentModel, OpenDocumentTextImporter importer) {
			if (!documentModel.DocumentCapabilities.SectionsAllowed)
				return false;
			if (!importer.PieceTable.IsMain)
				return false;
			OpenDocumentInputPosition pos = importer.InputPosition;
			bool shouldInsertNextSection = importer.DocumentModel.IsDocumentProtectionEnabled && pos.CurrentSectionInfo.SectionTagClosed;
			bool shouldStartNewSection = (pos.CurrentSectionInfo.SectionTagOpened && !pos.CurrentSectionInfo.IsFirstAtDocument)
				|| shouldInsertNextSection;
			bool shouldStartNewPage = pos.CurrentParagraphInfo.HasMasterPage && !pos.CurrentParagraphInfo.IsFirstAtDocument;
			ITableCellDestination cellDestination = importer.InputPosition.CurrentTableCellReference.CurrentCell;
			return (shouldStartNewSection || shouldStartNewPage) && cellDestination== null;
		}
		internal virtual bool ShouldApplySectionProperties() {
			if (!DocumentModel.DocumentCapabilities.SectionsAllowed)
				return false;
			if (!Importer.PieceTable.IsMain)
				return false;			
			bool applySectionProperties = CurrentSectionInfo.SectionTagOpened && !CurrentSectionInfo.IsFirstAtDocument;
			if (applySectionProperties)
				return true;
			bool applyMasterPageProperties = GetHasMasterPage();
			if (applyMasterPageProperties)
				return true;
			return false;			
		}
		private bool GetHasMasterPage() {
			if (Importer.InputPosition.ParagraphIndex != ParagraphIndex.Zero)
				return CurrentParagraphInfo.HasMasterPage;
			OdtImportedTableInfo tableInfo = Importer.InputPosition.TablesImportHelper.TopLevelTableInfo;
			if (tableInfo != null && tableInfo.HasMasterPage)
				return true;
			return CurrentParagraphInfo.HasMasterPage;
		}
		internal virtual void InsertBreak(char symbol) {
			Importer.PieceTable.InsertTextCore(Importer.InputPosition, new String(symbol, 1));
		}
		protected internal virtual void ApplyParagraphAutoStyle(ParagraphAutoStyleInfo info) {
			if (DocumentFormatsHelper.ShouldApplyParagraphStyle(Importer.DocumentModel))
				CurrentParagraphInfo.StyleIndex = GetParagraphStyleIndexByName(info.ParentStyleName);
			CurrentParagraphInfo.ParagraphFormatting.CopyFrom(info.ParagraphFormatting);
			CurrentParagraphInfo.Tabs.CopyFrom(info.Tabs);
			Importer.InputPosition.CharacterFormatting.CopyFrom(info.CharacterFormatting);
			CurrentParagraphInfo.Breaks.CopyFrom(info.Breaks);
			OdtImportedTableInfo topLevelTableInfo = Importer.InputPosition.TablesImportHelper.TopLevelTableInfo;
			if (Importer.InputPosition.ParagraphIndex != ParagraphIndex.Zero || topLevelTableInfo == null || !topLevelTableInfo.HasMasterPage)
				CurrentParagraphInfo.MasterPageName = info.MasterPageName;
			else
				CurrentParagraphInfo.MasterPageName = topLevelTableInfo.MasterPageName;
		}
		protected internal virtual void ApplyParagraphCommonStyle(string styleName) {
			if (!DocumentFormatsHelper.ShouldApplyParagraphStyle(Importer.DocumentModel))
				return;
			int styleIndex = GetParagraphStyleIndexByName(styleName);
			if (styleIndex == -1) {
				Debug.Assert(false, "paragraphStyle not founded", styleName);
			}
			CurrentParagraphInfo.StyleIndex = styleIndex;
		}
		protected internal virtual int GetParagraphStyleIndexByName(string name) {
			return Math.Max(0, Importer.DocumentModel.ParagraphStyles.GetStyleIndexByName(name));
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (!CurrentSectionInfo.SectionTagOpened)	
				InsertParagraphCore();
			InsertBreakAfterParagraph();
			CurrentParagraphInfo.IsFirstAtDocument = false;
			if (Importer.DocumentModel.IsDocumentProtectionEnabled)
				CurrentSectionInfo.IsFirstAtDocument = false;
			CurrentSectionInfo.SectionTagOpened = false;
		}
		void InsertParagraphCore() {
			ParagraphIndex paragraphIndex = Importer.InputPosition.ParagraphIndex;
			Importer.InsertParagraph();
			ApplyParagraphProperties(paragraphIndex);
		}
		protected internal virtual void InsertSection() {
			DocumentModel documentModel = Importer.DocumentModel;
			Importer.InsertSection();
			ApplyCurrentSectionProperties();
			ApplyParagraphProperties(Importer.InputPosition.ParagraphIndex);
			documentModel.CheckIntegrity();
		}
		internal virtual void ApplyCurrentSectionProperties() {
			string masterPageName = CurrentParagraphInfo.MasterPageName;
			Section section = Importer.DocumentModel.Sections.Last;
			PageLayoutStyleInfo pageStyle = Importer.GetPageLayoutStyle(masterPageName);
			SectionStyleInfo sectionStyle = Importer.GetSectionStyle(CurrentSectionInfo.StyleName);
			Importer.ApplySectionProperties(section, pageStyle, sectionStyle);
			Importer.ApplyHeaderAndFooterToSection(masterPageName, section, CurrentSectionInfo.DifferentFirstPage);
		}
		protected internal virtual void ApplyParagraphProperties(ParagraphIndex paragraphIndex) {
			DocumentModel documentModel = Importer.DocumentModel;
			Paragraph paragraph = Importer.PieceTable.Paragraphs[paragraphIndex];
			paragraph.ParagraphStyleIndex = CurrentParagraphInfo.StyleIndex;
			paragraph.ParagraphProperties.CopyFrom(CurrentParagraphInfo.ParagraphFormatting);
			paragraph.Tabs.CopyFrom(CurrentParagraphInfo.Tabs);
			if (!CurrentSectionInfo.SectionTagOpened
				&& DocumentFormatsHelper.ShouldInsertNumbering(documentModel)) 
				AddNumberingList(documentModel, paragraph);
		}
		protected internal virtual void AddNumberingList(DocumentModel documentModel, Paragraph paragraph) {
			NumberingListReference listReference = CurrentListReference;
			if (listReference.IsListActive && listReference.CanAddNumberingListToParagraph) {
				NumberingListIndex listIndex = new NumberingListIndex(listReference.ListIndex);
				int levelIndex = DocumentFormatsHelper.ShouldInsertMultiLevelNumbering(documentModel) ? listReference.LevelIndex : 0;
				Importer.PieceTable.AddNumberingListToParagraph(paragraph, listIndex, levelIndex);
				listReference.CanAddNumberingListToParagraph = false;
			}
		}
		public override bool ProcessText(XmlReader reader) {
			string textContent = reader.Value;
			textContent = ImportHelper.RemoveRedundantSpaces(textContent, false);
			if (!String.IsNullOrEmpty(textContent))
				Importer.PieceTable.InsertTextCore(Importer.InputPosition, textContent);
			return true;
		}
	}
	#endregion
	#region HeadingDestination
	public class HeadingDestination : ParagraphDestination {
		public HeadingDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			int level = ImportHelper.GetTextIntegerAttribute(reader, "outline-level", -1);
			if (level > 0 && level < 10)
				CurrentParagraphInfo.ParagraphFormatting.OutlineLevel = level;
		}
	}
	#endregion
}
