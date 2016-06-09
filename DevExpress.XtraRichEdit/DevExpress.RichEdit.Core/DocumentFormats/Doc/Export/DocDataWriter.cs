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
using System.IO;
using DevExpress.XtraRichEdit.Import.Doc;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Tables.Native;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Export.Doc {
	#region DocDataWriter
	public class DocDataWriter : IDisposable {
		#region Fields
		MemoryStream fkpMemoryStream;
		MemoryStream sepxMemoryStream;
		FKPWriter fkpWriter;
		SectionPropertiesWriter sepxWriter;
		DocStylesExporter stylesExporter;
		DocFieldsExporter fieldsExporter;
		DocListsExporter listsExporter;
		DocTablesExporter tablesExporter;
		DocCommentsExporter commentsExporter;
		DocNotesExporter notesExporter;
		DocFloatingObjectsExporter floatingObjectsExporter;
		DocBookmarkIterator bookmarkIterator;
		DocRangeEditPermissionIterator permissionIterator;
		#endregion
		public DocDataWriter(BinaryWriter dataStreamWriter, DocDocumentExporterOptions options, DocumentModel documentModel) {
			Guard.ArgumentNotNull(dataStreamWriter, "dataStreamWriter");
			this.fkpMemoryStream = new MemoryStream();
			this.sepxMemoryStream = new MemoryStream();
			this.fkpWriter = new FKPWriter(FileInformationBlock.FIBSize, fkpMemoryStream, dataStreamWriter);
			this.sepxWriter = new SectionPropertiesWriter(sepxMemoryStream);
			this.stylesExporter = new DocStylesExporter(documentModel);
			this.listsExporter = new DocListsExporter(documentModel, options, this.stylesExporter);
			this.tablesExporter = new DocTablesExporter();
			this.fieldsExporter = new DocFieldsExporter();
			this.commentsExporter = new DocCommentsExporter();
			this.notesExporter = new DocNotesExporter();
			this.floatingObjectsExporter = new DocFloatingObjectsExporter(documentModel.UnitConverter);
			this.bookmarkIterator = new DocBookmarkIterator();
			this.permissionIterator = new DocRangeEditPermissionIterator();
		}
		#region Properties
		protected FKPWriter FormattedDiskPageWriter { get { return this.fkpWriter; } }
		protected SectionPropertiesWriter SectionPropertiesWriter { get { return sepxWriter; } }
		public DocStylesExporter StylesExporter { get { return stylesExporter; } }
		public DocListsExporter ListsExporter { get { return listsExporter; } }
		public DocTablesExporter TablesExporter { get { return tablesExporter; } }
		public DocFieldsExporter FieldsExporter { get { return fieldsExporter; } }
		public DocBookmarkIterator BookmarkIterator { get { return bookmarkIterator; } }
		public DocRangeEditPermissionIterator PermissionIterator { get { return permissionIterator; } }
		public DocFieldTable FieldTable { get { return fieldsExporter.GetFieldTable(); } }
		public DocCommentsExporter CommentsExporter { get { return commentsExporter; } }
		public DocNotesExporter NotesExporter { get { return notesExporter; } }
		public DocFloatingObjectsExporter FloatingObjectsExporter { get { return floatingObjectsExporter; } }
		#endregion
		public void WriteSection(int characterPosition, Section section) {
			SectionPropertiesWriter.WriteSection(characterPosition, section);
		}
		public void WriteSectionPositions(BinaryWriter writer) {
			SectionPropertiesWriter.SectionHelper.Write(writer);
		}
		public void UpdateSectionsOffsets(int offset) {
			SectionPropertiesWriter.SectionHelper.UpdateOffsets(offset);
		}
		public void WriteCharactersBinTable(BinaryWriter writer) {
			FormattedDiskPageWriter.CharactersBinTable.Write(writer);
		}
		public void WriteParagraphsBinTable(BinaryWriter writer) {
			FormattedDiskPageWriter.ParagraphsBinTable.Write(writer);
		}
		public void WriteParagraph(int characterPosition, int paragraphStyleIndex) {
			FormattedDiskPageWriter.WriteParagraph(characterPosition, paragraphStyleIndex);
		}
		public void WriteParagraph(int characterPosition, int paragraphStyleIndex, Paragraph paragraph) {
			byte[] grpprl = TablesExporter.InTable ? StylesExporter.GetTableParagraphPropertyModifiers(paragraph, TablesExporter.TableDepth)
				: StylesExporter.GetParagraphGroupPropertyModifiers(paragraph);
			FormattedDiskPageWriter.WriteParagraph(characterPosition, paragraphStyleIndex, grpprl);
		}
		public void WriteInTableParagraph(int characterPosition, int paragraphStyleIndex, Paragraph paragraph) {
			byte[] grpprl = TablesExporter.GetTableCellPropertyModifiers(paragraph);
			FormattedDiskPageWriter.WriteParagraph(characterPosition, paragraphStyleIndex, grpprl);
		}
		public void WriteParagraph(int characterPosition, int paragraphStyleIndex, TableRow row) {
			Guard.ArgumentNotNull(row, "row");
			int tableStyleIndex = StylesExporter.GetStyleIndexByName(row.Table.TableStyle.StyleName);
			byte[] grpprl = TablesExporter.GetTableRowPropertyModifiers(row, tableStyleIndex);
			FormattedDiskPageWriter.WriteTableProperties(characterPosition, paragraphStyleIndex, grpprl);
		}
		public void WriteTextRun(int characterPosition) {
			FormattedDiskPageWriter.WriteTextRun(characterPosition);
		}
		public void WriteTextRun(int characterPosition, TextRunBase textRun, bool specialSymbol) {
			byte[] grpprl = StylesExporter.GetCharacterGroupPropertyModifiers(textRun, specialSymbol);
			FormattedDiskPageWriter.WriteTextRun(characterPosition, grpprl);
		}
		public void WriteInlinePictureRun(int characterPosition, int characterStyleIndex, int dataStreamOffset, CharacterProperties characterProperties) {
			byte[] grpprl = StylesExporter.GetInlinePicturePropertyModifiers(characterProperties, characterStyleIndex, dataStreamOffset);
			FormattedDiskPageWriter.WriteTextRun(characterPosition, grpprl);
		}
		public void Finish(int lastCharacterPosition) {
			FormattedDiskPageWriter.Finish(lastCharacterPosition);
			SectionPropertiesWriter.Finish(lastCharacterPosition);
			FieldsExporter.Finish(lastCharacterPosition);
			CommentsExporter.Finish(lastCharacterPosition);
			NotesExporter.FinishFootNoteReferences(lastCharacterPosition);
			NotesExporter.FinishEndNoteReferences(lastCharacterPosition);
			FloatingObjectsExporter.Finish(lastCharacterPosition);
			BookmarkIterator.Finish(lastCharacterPosition);
			PermissionIterator.Finish(lastCharacterPosition);
		}
		public void ExportFieldTables(FileInformationBlock fib, BinaryWriter writer) {
			this.fieldsExporter.ExportFieldTables(fib, writer);
		}
		public byte[] GetFormattedDiskPages() {
			return this.fkpMemoryStream.GetBuffer();
		}
		public byte[] GetSectionProperties() {
			return this.sepxMemoryStream.ToArray();
		}
		protected internal virtual void SetState(DocContentState state) {
			FieldsExporter.State = state;
			FloatingObjectsExporter.State = state;
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (this.fkpWriter != null) {
					this.fkpWriter.Dispose();
					this.fkpWriter = null;
				}
				if (this.sepxWriter != null) {
					this.sepxWriter.Dispose();
					this.sepxWriter = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~DocDataWriter() {
			Dispose(false);
		}
		#endregion
	}
	#endregion
	#region DocStyleIndexes
	public static class DocStyleIndexes {
		public const int DefaultParagraphStyleIndex = 0x0000;
		public const int DefaultCharacterStyleIndex = 0x000a;
		public const int DefaultTableStyleIndex = 0x000b;
		public const int DefaultListStyleIndex = 0x000c;
	}
	#endregion
	#region DocStylesExporter
	public class DocStylesExporter {
		#region static
		static DocStylesExporter() {
			headingStyles = new Dictionary<ushort, string>(9);
			headingStyles.Add(0x0001, "Heading 1");
			headingStyles.Add(0x0002, "Heading 2");
			headingStyles.Add(0x0003, "Heading 3");
			headingStyles.Add(0x0004, "Heading 4");
			headingStyles.Add(0x0005, "Heading 5");
			headingStyles.Add(0x0006, "Heading 6");
			headingStyles.Add(0x0007, "Heading 7");
			headingStyles.Add(0x0008, "Heading 8");
			headingStyles.Add(0x0009, "Heading 9");
		}
		#endregion
		#region Fields
		const int defaultParagraphStyleIdentifier = 0x0000;
		const int defaultCharacterStyleIdentifier = 0x0041;
		const int defaultTableStyleIdentifier = 0x0069;
		const int defaultListStyleIdentifier = 0x006b;
		const string defaultParagraphStyleName = "Normal";
		const string defaultCharacterStyleName = "Default Paragraph Font";
		const string defaultListStyleName = "No List";
		const string defaultTableStyleName = "Table Normal";
		static Dictionary<ushort, string> headingStyles;
		FontsExportHelper fontsExportHelper;
		DocumentModel documentModel;
		Dictionary<string, int> styleNamesCollection;
		DocStyleSheet styleSheet;
		#endregion
		public DocStylesExporter(DocumentModel documentModel) {
			this.documentModel = documentModel;
			this.styleNamesCollection = new Dictionary<string, int>();
			this.fontsExportHelper = new FontsExportHelper();
			fontsExportHelper.FontNamesCollection.Clear();
		}
		#region Properties
		protected DocumentModel DocumentModel { get { return documentModel; } }
		public FontsExportHelper FontsExportHelper { get { return fontsExportHelper; } }
		public Dictionary<string, int> StyleNamesCollection { get { return styleNamesCollection; } }
		public int FontsCount { get { return FontsExportHelper.FontNamesCollection.Count; } }
		protected DocStyleSheet StyleSheet { get { return styleSheet; } }
		#endregion
		public int GetParagraphStyleIndex(int documentModelParagraphIndex) {
			int index;
			if (StyleNamesCollection.TryGetValue(DocumentModel.ParagraphStyles[documentModelParagraphIndex].StyleName, out index))
				return index;
			return 0;
		}
		public int GetCharacterStyleIndex(int documentModelCharacterStyleIndex) {
			int index;
			if (StyleNamesCollection.TryGetValue(DocumentModel.CharacterStyles[documentModelCharacterStyleIndex].StyleName, out index))
				return index;
			return DocStyleIndexes.DefaultCharacterStyleIndex;
		}
		public int GetNumberingStyleIndex(int documentModelNumberingStyleIndex) {
			int index;
			if (StyleNamesCollection.TryGetValue(DocumentModel.NumberingListStyles[documentModelNumberingStyleIndex].StyleName, out index))
				return index;
			return -1;
		}
		public int GetStyleIndexByName(string styleName) {
			int styleIndex;
			if (StyleNamesCollection.TryGetValue(styleName, out styleIndex))
				return styleIndex;
			return -1;
		}
		public int GetFontIndexByName(string fontName) {
			return FontsExportHelper.GetFontIndexByName(fontName);
		}
		public string GetFontNameByIndex(int fontNameIndex) {
			return FontsExportHelper.GetFontNameByIndex(fontNameIndex);
		}
		public byte[] GetCharacterGroupPropertyModifiers(ListLevel listLevel, int characterStyleIndex) {
			int fontNameIndex = GetFontIndexByName(listLevel.CharacterProperties.FontName);
			using (MemoryStream output = new MemoryStream()) {
				DocCharacterPropertiesActions actions = new DocListCharacterPropertiesActions(output, listLevel, fontNameIndex);
				actions.CreateCharacterPropertiesModifiers(characterStyleIndex, DocStyleIndexes.DefaultParagraphStyleIndex, false);
				return output.ToArray();
			}
		}
		public byte[] GetCharacterGroupPropertyModifiers(TextRunBase run, bool specialSymbol) {
			int characterStyleIndex = (run != null) ? GetCharacterStyleIndex(run.CharacterStyleIndex) : DocStyleIndexes.DefaultCharacterStyleIndex;
			int fontNameIndex = (run != null) ? GetFontIndexByName(run.CharacterProperties.FontName) : GetFontIndexByName(DocCharacterFormattingInfo.DefaultFontName);
			using (MemoryStream output = new MemoryStream()) {
				CharacterProperties characterProperties = (run != null) ? run.CharacterProperties : null;
				int paragraphStyleIndex = (run != null) ? GetParagraphStyleIndex(run.Paragraph.ParagraphStyleIndex) : DocStyleIndexes.DefaultParagraphStyleIndex;
				DocCharacterPropertiesActions actions = new DocCharacterPropertiesActions(output, characterProperties, fontNameIndex);
				actions.CreateCharacterPropertiesModifiers(characterStyleIndex, paragraphStyleIndex, specialSymbol);
				return output.ToArray();
			}
		}
		public byte[] GetInlinePicturePropertyModifiers(CharacterProperties characterProperties, int characterStyleIndex, int dataStreamOffset) {
			int fontNameIndex = GetFontIndexByName(characterProperties.FontName);
			using (MemoryStream output = new MemoryStream()) {
				DocCharacterPropertiesActions actions = new DocCharacterPropertiesActions(output, characterProperties, fontNameIndex);
				actions.CreateInlinePicturePropertiesModifiers(characterStyleIndex, dataStreamOffset);
				return output.ToArray();
			}
		}
		public byte[] GetParagraphGroupPropertyModifiers(Paragraph paragraph) {
			using (MemoryStream output = new MemoryStream()) {
				DocParagraphPropertiesActions actions = new DocParagraphPropertiesActions(output, paragraph);
				actions.CreateParagarphPropertyModifiers();
				return output.ToArray();
			}
		}
		public byte[] GetParagraphGroupPropertyModifiers(ListLevel listLevel) {
			using (MemoryStream output = new MemoryStream()) {
				DocParagraphPropertiesActions actions = new DocParagraphPropertiesActions(output, listLevel);
				actions.CreateParagarphPropertyModifiers();
				return output.ToArray();
			}
		}
		public byte[] GetTableParagraphPropertyModifiers(Paragraph paragraph, int tableDepth) {
			using (MemoryStream output = new MemoryStream()) {
				DocParagraphPropertiesActions actions = new DocParagraphPropertiesActions(output, paragraph);
				actions.CreateTableParagraphPropertyModifiers(tableDepth);
				return output.ToArray();
			}
		}
		public void CreateStyleSheet() {
			this.styleSheet = DocStyleSheet.CreateDefault();
			SetStyleSheetInformation(styleSheet);
			AddDefaultStyles(this.styleSheet);
			AddParagraphStyles(this.styleSheet);
			AddCharacterStyles(this.styleSheet);
			AddListStyles(this.styleSheet);
			AddTableStyles(this.styleSheet);
			styleSheet.StylesInformation.StylesCount = (short)this.styleSheet.Styles.Count;
		}
		public void WriteStyleSheet(BinaryWriter writer) {
			this.styleSheet.Write(writer);
		}
		void SetStyleSheetInformation(DocStyleSheet styleSheet) {
			if (documentModel.DefaultCharacterProperties.UseFontName)
				 styleSheet.StylesInformation.DefaultASCIIFont = (short)FontsExportHelper.GetFontIndexByName(this.DocumentModel.DefaultCharacterProperties.FontName);
		}
		void AddDefaultStyles(DocStyleSheet styleSheet) {
			AddDefaultParagraphStyle(styleSheet);
			AddHeadingStyles(styleSheet);
			AddDefaultCharacterStyle(styleSheet);
			AddDefaultTableStyle(styleSheet);
			AddDefaultListStyle(styleSheet);
			styleSheet.Styles.Add(null);
			styleSheet.Styles.Add(null);
			styleSheet.StylesInformation.FixedIndexStylesCount = 0x000f;
		}
		void AddDefaultParagraphStyle(DocStyleSheet styleSheet) {
			ParagraphStyle defaultParagraphStyle = DocumentModel.ParagraphStyles.GetStyleByName(defaultParagraphStyleName);
			if (defaultParagraphStyle == null)
				defaultParagraphStyle = DocumentModel.ParagraphStyles.DefaultItem;
			int fontNameIndex = FontsExportHelper.GetFontIndexByName(defaultParagraphStyle.CharacterProperties.FontName);
			ParagraphStyleDescription defaultParagraphStyleDescription = new ParagraphStyleDescription(defaultParagraphStyle, fontNameIndex);
			defaultParagraphStyleDescription.StyleIdentifier = defaultParagraphStyleIdentifier;
			defaultParagraphStyleDescription.StyleName = defaultParagraphStyle.StyleName;
			styleSheet.Styles.Add(defaultParagraphStyleDescription);
			StyleNamesCollection.Add(defaultParagraphStyle.StyleName, 0);
		}
		void AddHeadingStyles(DocStyleSheet styleSheet) {
			for (ushort defaultHeadingStyleIndex = 1; defaultHeadingStyleIndex < 10; defaultHeadingStyleIndex++) {
				string headingStyleName = headingStyles[defaultHeadingStyleIndex];
				ParagraphStyle heading = DocumentModel.ParagraphStyles.GetStyleByName(headingStyleName);
				if (heading != null) {
					int fontNameIndex = FontsExportHelper.GetFontIndexByName(heading.CharacterProperties.FontName);
					ParagraphStyleDescription headingStyleDescription = new ParagraphStyleDescription(heading, fontNameIndex);
					headingStyleDescription.StyleIdentifier = 0x0001;
					headingStyleDescription.StyleName = headingStyleName;
					headingStyleDescription.BaseStyleIndex = defaultParagraphStyleIdentifier;
					headingStyleDescription.NextStyleIndex = defaultParagraphStyleIdentifier;
					styleSheet.Styles.Add(headingStyleDescription);
					StyleNamesCollection.Add(headingStyleName, defaultHeadingStyleIndex);
				}
				else
					styleSheet.Styles.Add(null);
			}
		}
		void AddDefaultCharacterStyle(DocStyleSheet styleSheet) {
			CharacterStyle defaultCharacterStyle = DocumentModel.CharacterStyles.GetStyleByName(defaultCharacterStyleName);
			int fontNameIndex = FontsExportHelper.GetFontIndexByName(defaultCharacterStyle.CharacterProperties.FontName);
			CharacterStyleDescription defaultCharacterStyleDescription = new CharacterStyleDescription(defaultCharacterStyle, fontNameIndex);
			defaultCharacterStyleDescription.StyleIdentifier = defaultCharacterStyleIdentifier;
			defaultCharacterStyleDescription.StyleName = defaultCharacterStyleName;
			styleSheet.Styles.Add(defaultCharacterStyleDescription);
			StyleNamesCollection.Add(defaultCharacterStyleName, DocStyleIndexes.DefaultCharacterStyleIndex);
		}
		void AddDefaultTableStyle(DocStyleSheet styleSheet) {
			TableStyle defaultTableStyle = DocumentModel.TableStyles.GetStyleByName(defaultTableStyleName);
			if (defaultTableStyle != null) {
				int fontNameIndex = FontsExportHelper.GetFontIndexByName(defaultTableStyle.CharacterProperties.FontName);
				TableStyleDescription defaultTableStyleDescription = new TableStyleDescription(defaultTableStyle, fontNameIndex);
				defaultTableStyleDescription.StyleIdentifier = defaultTableStyleIdentifier;
				defaultTableStyleDescription.StyleName = defaultTableStyleName;
				defaultTableStyleDescription.StyleIndex = DocStyleIndexes.DefaultTableStyleIndex;
				StyleNamesCollection.Add(defaultTableStyleName, DocStyleIndexes.DefaultTableStyleIndex);
				styleSheet.Styles.Add(defaultTableStyleDescription);
			}
			else
				styleSheet.Styles.Add(null);
		}
		void AddDefaultListStyle(DocStyleSheet styleSheet) {
			ListStyleDescription defaultListStyleDescription = new ListStyleDescription();
			defaultListStyleDescription.StyleIdentifier = defaultListStyleIdentifier;
			defaultListStyleDescription.StyleName = defaultListStyleName;
			defaultListStyleDescription.StyleIndex = DocStyleIndexes.DefaultListStyleIndex;
			StyleNamesCollection.Add(defaultListStyleName, DocStyleIndexes.DefaultListStyleIndex);
			styleSheet.Styles.Add(defaultListStyleDescription);
		}
		void AddParagraphStyles(DocStyleSheet styleSheet) {
			int count = DocumentModel.ParagraphStyles.Count;
			for (int paragraphStyleIndex = 0; paragraphStyleIndex < count; paragraphStyleIndex++) {
				ParagraphStyle currentStyle = DocumentModel.ParagraphStyles[paragraphStyleIndex];
				if (GetStyleIndexByName(currentStyle.StyleName) < 0) {
					int fontNameIndex = GetFontIndexByName(currentStyle.CharacterProperties.FontName);
					ParagraphStyleDescription styleDescription = new ParagraphStyleDescription(currentStyle, fontNameIndex);
					styleDescription.StyleName = currentStyle.StyleName;
					styleDescription.Hidden = currentStyle.Hidden;
					styleDescription.QFormat = currentStyle.Primary;
					styleDescription.StyleIndex = styleSheet.Styles.Count;
					if (currentStyle.LinkedStyle != null)
						styleDescription.LinkedStyleIndex = (short)GetStyleIndexByName(currentStyle.LinkedStyle.StyleName);
					if (currentStyle.Parent != null)
						styleDescription.BaseStyleIndex = (short)GetStyleIndexByName(currentStyle.Parent.StyleName);
					StyleNamesCollection.Add(styleDescription.StyleName, styleDescription.StyleIndex);
					StyleSheet.Styles.Add(styleDescription);
				}
			}
			SetNextParagraphStyles();
		}
		void SetNextParagraphStyles() {
			int count = StyleSheet.Styles.Count;
			for (int i = 0; i < count; i++) {
				StyleDescriptionBase description = StyleSheet.Styles[i];
				if (description is ParagraphStyleDescription) {
					ParagraphStyle style = DocumentModel.ParagraphStyles.GetStyleByName(description.StyleName);
					if (style.NextParagraphStyle != null)
						description.NextStyleIndex = (short)GetStyleIndexByName(style.NextParagraphStyle.StyleName);
				}
			}
		}
		void AddCharacterStyles(DocStyleSheet styleSheet) {
			int count = DocumentModel.CharacterStyles.Count;
			for (int characterStyleIndex = 0; characterStyleIndex < count; characterStyleIndex++) {
				CharacterStyle currentStyle = DocumentModel.CharacterStyles[characterStyleIndex];
				if (GetStyleIndexByName(currentStyle.StyleName) < 0) {
					int fontNameIndex = GetFontIndexByName(currentStyle.CharacterProperties.FontName);
					CharacterStyleDescription styleDescription = new CharacterStyleDescription(currentStyle, fontNameIndex);
					styleDescription.StyleName = currentStyle.StyleName;
					styleDescription.Hidden = currentStyle.Hidden;
					styleDescription.QFormat = currentStyle.Primary;
					styleDescription.StyleIndex = styleSheet.Styles.Count;
					if (currentStyle.Parent != null)
						styleDescription.BaseStyleIndex = (short)GetStyleIndexByName(currentStyle.Parent.StyleName);
					if (currentStyle.LinkedStyle != null)
						styleDescription.LinkedStyleIndex = (short)GetStyleIndexByName(currentStyle.LinkedStyle.StyleName);
					styleDescription.NextStyleIndex = (short)styleDescription.StyleIndex;
					StyleNamesCollection.Add(styleDescription.StyleName, styleDescription.StyleIndex);
					styleSheet.Styles.Add(styleDescription);
				}
			}
		}
		void AddListStyles(DocStyleSheet styleSheet) {
			int count = DocumentModel.NumberingListStyles.Count;
			for (int i = 0; i < count; i++) {
				NumberingListStyle currentStyle = DocumentModel.NumberingListStyles[i];
				if (currentStyle.NumberingListIndex < NumberingListIndex.MinValue)
					continue;
				if (GetStyleIndexByName(currentStyle.StyleName) < 0) {
					ListStyleDescription styleDescription = new ListStyleDescription(currentStyle);
					styleDescription.StyleName = currentStyle.StyleName;
					styleDescription.Hidden = currentStyle.Hidden;
					styleDescription.QFormat = currentStyle.Primary;
					styleDescription.StyleIndex = styleSheet.Styles.Count;
					if (currentStyle.Parent != null)
						styleDescription.BaseStyleIndex = (short)GetStyleIndexByName(currentStyle.Parent.StyleName);
					styleDescription.NextStyleIndex = (short)styleDescription.StyleIndex;
					StyleNamesCollection.Add(styleDescription.StyleName, styleDescription.StyleIndex);
					styleSheet.Styles.Add(styleDescription);
				}
			}
		}
		void AddTableStyles(DocStyleSheet styleSheet) {
			int count = DocumentModel.TableStyles.Count;
			for (int tableStyleIndex = 0; tableStyleIndex < count; tableStyleIndex++) {
				TableStyle currentStyle = DocumentModel.TableStyles[tableStyleIndex];
				if (GetStyleIndexByName(currentStyle.StyleName) < 0) {
					int fontNameIndex = GetFontIndexByName(currentStyle.CharacterProperties.FontName);
					TableStyleDescription styleDescription = new TableStyleDescription(currentStyle, fontNameIndex);
					styleDescription.StyleName = currentStyle.StyleName;
					styleDescription.Hidden = currentStyle.Hidden;
					styleDescription.QFormat = currentStyle.Primary;
					styleDescription.StyleIndex = styleSheet.Styles.Count;
					styleDescription.NextStyleIndex = (short)styleDescription.StyleIndex;
					if (currentStyle.Parent != null)
						styleDescription.BaseStyleIndex = (short)GetStyleIndexByName(currentStyle.Parent.StyleName);
					StyleNamesCollection.Add(styleDescription.StyleName, styleDescription.StyleIndex);
					styleSheet.Styles.Add(styleDescription);
				}
			}
		}
	}
	#endregion
	#region DocListsExporter
	public class DocListsExporter {
		#region Fields
		const int emptyStyleIdentifier = 0x0fff;
		public const int EmptyCharacterPosition = unchecked((int)0xffffffff);
		DocumentModel documentModel;
		DocDocumentExporterOptions options;
		DocStylesExporter stylesExportHelper;
		DocListFormatInformation listInfo;
		DocListOverrideFormatInformation listOverrideInfo;
		List<ListStylesRecordItem> listStyles;
		#endregion
		public DocListsExporter(DocumentModel documentModel, DocDocumentExporterOptions options, DocStylesExporter stylesExportHelper) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			Guard.ArgumentNotNull(options, "options");
			this.documentModel = documentModel;
			this.options = options;
			this.stylesExportHelper = stylesExportHelper;
		}
		#region Properties
		protected DocumentModel DocumentModel { get { return documentModel; } }
		protected DocDocumentExporterOptions Options { get { return options; } }
		protected DocStylesExporter StylesExportHelper { get { return stylesExportHelper; } }
		protected internal List<ListStylesRecordItem> ListStyles { get { return listStyles; } }
		protected internal DocListFormatInformation ListInfo { get { return listInfo; } }
		protected internal DocListOverrideFormatInformation ListOverrideInfo { get { return listOverrideInfo; } }
		#endregion
		public void WriteListFormatInformation(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			this.listInfo.Write(writer);
		}
		public void WriteListOverrideFormatInformation(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			this.listOverrideInfo.Write(writer, Options.Compatibility.AllowNonLinkedListDefinitions);
		}
		public void CreateLists(DocStylesExporter stylesExporter) {
			this.listInfo = CreateListInfo(stylesExporter);
			this.listOverrideInfo = CreateListOverrideInfo();
		}
		DocListFormatInformation CreateListInfo(DocStylesExporter stylesExporter) {
			DocListFormatInformation result = new DocListFormatInformation();
			this.listStyles = new List<ListStylesRecordItem>();
			int count = DocumentModel.AbstractNumberingLists.Count;
			AbstractNumberingListIndex[] indexesSortedById = new AbstractNumberingListIndex[count];
			for (int i = 0; i < count; i++)
				indexesSortedById[i] = new AbstractNumberingListIndex(i);
			Comparison<AbstractNumberingListIndex> comparison = delegate(AbstractNumberingListIndex index1, AbstractNumberingListIndex index2) {
				AbstractNumberingList list1 = DocumentModel.AbstractNumberingLists[index1];
				AbstractNumberingList list2 = DocumentModel.AbstractNumberingLists[index2];
				return list1.Id - list2.Id;
			};
			Array.Sort(indexesSortedById, comparison);
			for (int i = 0; i < count; i++) {
				AbstractNumberingList list = DocumentModel.AbstractNumberingLists[indexesSortedById[i]];
				result.ListData.Add(CreateDocListData(list));
				if (list.StyleLinkIndex >= 0) {
					int docStyleIndex = stylesExporter.GetNumberingStyleIndex(list.StyleLinkIndex);
					if(docStyleIndex >= 0)
						listStyles.Add(new ListStylesRecordItem(i, docStyleIndex, true));
				}
				else if (list.NumberingStyleReferenceIndex >= 0) {
					int docStyleIndex = stylesExporter.GetNumberingStyleIndex(list.NumberingStyleReferenceIndex);
					if (docStyleIndex >= 0)
						listStyles.Add(new ListStylesRecordItem(i, docStyleIndex, false));
				}
			}
			return result;
		}
		DocListData CreateDocListData(AbstractNumberingList list) {
			DocListData result = new DocListData();
			result.ListFormatting = CreateListFormatting(list);
			for (int i = 0; i < DocListFormatting.MaxLevelCount; i++)
				result.LevelsFormatting.Add(CreateDocListLevel(list.Levels[i]));
			return result;
		}
		DocListFormatting CreateListFormatting(AbstractNumberingList list) {
			DocListFormatting listFormatting = new DocListFormatting();
			listFormatting.ListIdentifier = list.Id;
			if(list.StyleLinkIndex >= 0 || list.NumberingStyleReferenceIndex >=0)
				listFormatting.TemplateCode =  list.TemplateCode;
			else
				listFormatting.TemplateCode =  list.GetHashCode();
			listFormatting.SimpleList = false;
			for (int i = 0; i < DocListFormatting.MaxLevelCount; i++)
				if (list.Levels[i].ParagraphStyle == null)
					listFormatting.LevelStyleIdentifiers[i] = emptyStyleIdentifier;
				else
					listFormatting.LevelStyleIdentifiers[i] = (short)StylesExportHelper.GetParagraphStyleIndex(list.Levels[i].ParagraphStyleIndex);
			return listFormatting;
		}
		DocListLevel CreateDocListLevel(ListLevel listLevel) {
			DocListLevel result = new DocListLevel();
			result.ListLevelProperties = CreateListLevelProperties(listLevel.ListLevelProperties);
			result.CharacterUPX = StylesExportHelper.GetCharacterGroupPropertyModifiers(listLevel, DocStyleIndexes.DefaultCharacterStyleIndex);
			result.ListLevelProperties.CharacterUPXSize = (byte)result.CharacterUPX.Length;
			result.ParagraphUPX = StylesExportHelper.GetParagraphGroupPropertyModifiers(listLevel);
			result.ListLevelProperties.ParagraphUPXSize = (byte)result.ParagraphUPX.Length;
			result.SetDisplayFormatString(listLevel.ListLevelProperties.DisplayFormatString);
			return result;
		}
		DocListLevelProperties CreateListLevelProperties(ListLevelProperties listLevelProperties) {
			DocListLevelProperties result = new DocListLevelProperties();
			result.Alignment = listLevelProperties.Alignment;
			result.ConvertPreviousLevelNumberingToDecimal = listLevelProperties.ConvertPreviousLevelNumberingToDecimal;
			result.Legacy = listLevelProperties.Legacy;
			result.LegacyIndent = listLevelProperties.LegacyIndent;
			result.LegacySpace = listLevelProperties.LegacySpace;
			result.NumberingFormat = listLevelProperties.Format;
			result.RelativeRestartLevel = (byte)listLevelProperties.RelativeRestartLevel;
			result.Separator = listLevelProperties.Separator;
			result.Start = listLevelProperties.Start;
			result.SuppressRestart = listLevelProperties.SuppressRestart;
			return result;
		}
		DocListOverrideFormatInformation CreateListOverrideInfo() {
			DocListOverrideFormatInformation result = new DocListOverrideFormatInformation();
			int count = DocumentModel.NumberingLists.Count;
			for (int i = 0; i < count; i++) {
				NumberingList list = DocumentModel.NumberingLists[new NumberingListIndex(i)];
				DocListOverrideLevelInformation docListOverrideData = CreateDocListOverrideData(list);
				result.FormatOverrideData.Add(docListOverrideData);
				result.FormatOverride.Add(CreateDocListOverride(list, docListOverrideData.LevelFormatOverrideData.Count));
			}
			return result;
		}
		DocListOverrideFormat CreateDocListOverride(NumberingList list, int overrideLevelCount) {
			DocListOverrideFormat result = new DocListOverrideFormat();
			result.ListIdentifier = list.AbstractNumberingList.Id;
			result.LevelsCount = (byte)overrideLevelCount;
			return result;
		}
		DocListOverrideLevelInformation CreateDocListOverrideData(NumberingList list) {
			DocListOverrideLevelInformation result = new DocListOverrideLevelInformation();
			result.CharacterPosition = EmptyCharacterPosition;
			int count = list.Levels.Count;
			for (int i = 0; i < count; i++) {
				DocListOverrideLevelFormat overriddenLevel = CreateOverrideLevel(list.Levels[i], i);
				if (overriddenLevel != null)
					result.LevelFormatOverrideData.Add(overriddenLevel);
			}
			return result;
		}
		DocListOverrideLevelFormat CreateOverrideLevel(IOverrideListLevel listLevel, int levelIndex) {
			DocListOverrideLevelFormat result = new DocListOverrideLevelFormat();
			result.StartAt = listLevel.NewStart;
			result.OverriddenLevel = levelIndex;
			result.OverrideStart = listLevel.OverrideStart;
			OverrideListLevel overrideListLevel = listLevel as OverrideListLevel;
			if (overrideListLevel != null) {
				result.OverrideFormatting = true;
				result.OverrideLevelFormatting = CreateDocListLevel(overrideListLevel);
			}
			if (result.OverrideStart || result.OverrideFormatting)
				return result;
			else
				return null;
		}
	}
	#endregion
	#region DocFieldsExporter
	public class DocFieldsExporter {
		#region Fields
		DocFieldTable mainDocumentFieldTable;
		DocFieldTable footNotesFieldTable;
		DocFieldTable headersFootersFieldTable;
		DocFieldTable commentsFieldTable;
		DocFieldTable endNotesFieldTable;
		DocFieldTable textBoxesFieldTable;
		DocFieldTable headerTextBoxesFieldTable;
		DocContentState state;
		#endregion
		#region Properties
		 protected internal DocContentState State { get { return state; } set { state = value; } }
		#endregion
		public DocFieldTable GetFieldTable() {
			switch (State) {
				case DocContentState.MainDocument:
					if (this.mainDocumentFieldTable == null)
						this.mainDocumentFieldTable = new DocFieldTable();
					return this.mainDocumentFieldTable;
				case DocContentState.Footnotes:
					if (this.footNotesFieldTable == null)
						this.footNotesFieldTable = new DocFieldTable();
					return this.footNotesFieldTable;
				case DocContentState.HeadersFootersStory:
					if (this.headersFootersFieldTable == null)
						this.headersFootersFieldTable = new DocFieldTable();
					return this.headersFootersFieldTable;
				case DocContentState.Comments:
					if (this.commentsFieldTable == null)
						this.commentsFieldTable = new DocFieldTable();
					return this.commentsFieldTable;
				case DocContentState.Endnotes:
					if (this.endNotesFieldTable == null)
						this.endNotesFieldTable = new DocFieldTable();
					return this.endNotesFieldTable;
				case DocContentState.TextBoxes:
					if (this.textBoxesFieldTable == null)
						this.textBoxesFieldTable = new DocFieldTable();
					return this.textBoxesFieldTable;
				case DocContentState.HeaderTextBoxes:
					if (this.headerTextBoxesFieldTable == null)
						this.headerTextBoxesFieldTable = new DocFieldTable();
					return this.headerTextBoxesFieldTable;
				default:
					return null;
			}
		}
		public void Finish(int lastCharacterPosition) {
			if (this.mainDocumentFieldTable != null)
				this.mainDocumentFieldTable.Finish(lastCharacterPosition);
			if (this.headersFootersFieldTable != null)
				this.headersFootersFieldTable.Finish(lastCharacterPosition);
			if (this.textBoxesFieldTable != null)				
				this.textBoxesFieldTable.Finish(lastCharacterPosition);
			if (this.headerTextBoxesFieldTable != null)
				this.headerTextBoxesFieldTable.Finish(lastCharacterPosition);
			if (this.footNotesFieldTable != null)
				this.footNotesFieldTable.Finish(lastCharacterPosition);
			if (this.endNotesFieldTable != null)
				this.endNotesFieldTable.Finish(lastCharacterPosition);
			if (this.commentsFieldTable != null)
				this.commentsFieldTable.Finish(lastCharacterPosition);
		}
		public void ExportFieldTables(FileInformationBlock fib, BinaryWriter writer) {
			ExportMainDocumentFieldTable(fib, writer);
			ExportHeadersFootersFieldTable(fib, writer);
			ExportFootNotesFieldTable(fib, writer);
			ExportCommentsFieldTable(fib, writer);
			ExportEndNotesFieldTable(fib, writer);
			ExportTextBoxesFieldTable(fib, writer);
			ExportHeaderTextBoxesFieldTable(fib, writer);
		}
		void ExportMainDocumentFieldTable(FileInformationBlock fib, BinaryWriter writer) {
			if (this.mainDocumentFieldTable != null) {
				fib.MainDocumentFieldTableOffset = (int)writer.BaseStream.Position;
				this.mainDocumentFieldTable.Write(writer);
				fib.MainDocumentFieldTableSize = (int)(writer.BaseStream.Position - fib.MainDocumentFieldTableOffset);
			}
		}
		void ExportHeadersFootersFieldTable(FileInformationBlock fib, BinaryWriter writer) {
			if (this.headersFootersFieldTable != null) {
				fib.HeadersFootersFieldTableOffset = (int)writer.BaseStream.Position;
				this.headersFootersFieldTable.Write(writer);
				fib.HeadersFootersFieldTableSize = (int)(writer.BaseStream.Position - fib.HeadersFootersFieldTableOffset);
			}
		}
		void ExportFootNotesFieldTable(FileInformationBlock fib, BinaryWriter writer) {
			if (this.footNotesFieldTable != null) {
				fib.FootNotesFieldTableOffset = (int)writer.BaseStream.Position;
				this.footNotesFieldTable.Write(writer);
				fib.FootNotesFieldTableSize = (int)(writer.BaseStream.Position - fib.FootNotesFieldTableOffset);
			}
		}
		void ExportCommentsFieldTable(FileInformationBlock fib, BinaryWriter writer) {
			if (this.commentsFieldTable != null) {
				fib.CommentsFieldTableOffset = (int)writer.BaseStream.Position;
				this.commentsFieldTable.Write(writer);
				fib.CommentsFieldTableSize = (int)(writer.BaseStream.Position - fib.CommentsFieldTableOffset);
			}
		}
		void ExportEndNotesFieldTable(FileInformationBlock fib, BinaryWriter writer) {
			if (this.endNotesFieldTable != null) {
				fib.EndNotesFieldTableOffset = (int)writer.BaseStream.Position;
				this.endNotesFieldTable.Write(writer);
				fib.EndNotesFieldTableSize = (int)(writer.BaseStream.Position - fib.EndNotesFieldTableOffset);
			}
		}
		void ExportTextBoxesFieldTable(FileInformationBlock fib, BinaryWriter writer) {
			if (this.textBoxesFieldTable != null) {
				fib.MainDocumentTextBoxesFieldTableOffset = (int)writer.BaseStream.Position;
				this.textBoxesFieldTable.Write(writer);
				fib.MainDocumentTextBoxesFieldTableSize = (int)(writer.BaseStream.Position - fib.MainDocumentTextBoxesFieldTableOffset);
			}
		}
		void ExportHeaderTextBoxesFieldTable(FileInformationBlock fib, BinaryWriter writer) {
			if (this.headerTextBoxesFieldTable != null) {
				fib.HeaderTextBoxesFieldTableOffset = (int)(writer.BaseStream.Position);
				this.headerTextBoxesFieldTable.Write(writer);
				fib.HeaderTextBoxesFieldTableSize = (int)(writer.BaseStream.Position - fib.HeaderTextBoxesFieldTableOffset);
			}
		}
	}
	#endregion
	#region DocTablesExporter
	public class DocTablesExporter {
		#region Fields
		int currentTableDepth;
		Stack<TableGrid> grids;
		#endregion
		public DocTablesExporter() {
			this.grids = new Stack<TableGrid>();
		}
		#region Properties
		public bool InTable { get { return this.currentTableDepth > 0; } }
		public int TableDepth { get { return this.currentTableDepth; } }
		protected internal Stack<TableGrid> Grids { get { return grids; } }
		#endregion
		public void AdvanceNext(DevExpress.XtraRichEdit.Internal.TableInfo info) {
			this.currentTableDepth++;
			DocumentModelUnitToLayoutUnitConverter converter = info.Table.DocumentModel.UnitConverter.CreateConverterToLayoutUnits(DevExpress.Office.DocumentLayoutUnit.Twip);
			DocTableWidthsCalculator tableWidthsCalculator = new DocTableWidthsCalculator(converter);
			TableGridCalculator gridCalculator = new TableGridCalculator(info.Table.DocumentModel, tableWidthsCalculator, Int32.MaxValue);
			int parentSectionWidth = GetParentSectionWidthInModelUnits(info.Table);
			Grids.Push(gridCalculator.CalculateTableGrid(info.Table, parentSectionWidth));
		}
		public void FinishTable() {
			Debug.Assert(this.currentTableDepth > 0);
			this.currentTableDepth--;
			Grids.Pop();
		}
		public string GetTableUnitMark() {
			return (TableDepth == 1) ? new string(TextCodes.TableUnitMark, 1) : new string(TextCodes.ParagraphMark, 1);
		}
		public byte[] GetTableCellPropertyModifiers(Paragraph paragraph) {
			using (MemoryStream output = new MemoryStream()) {
				DocParagraphPropertiesActions paragraphActions = new DocParagraphPropertiesActions(output, paragraph);
				paragraphActions.CreateParagarphPropertyModifiers();
				TableCell cell = paragraph.GetCell();
				DocTableCellActions cellActions = new DocTableCellActions(output, cell);
				cellActions.CreateTableCellPropertyModifiers(TableDepth, cell != null && cell.EndParagraphIndex == paragraph.Index);
				return output.ToArray();
			}
		}
		public byte[] GetTableRowPropertyModifiers(TableRow row, int tableStyleIndex) {
			using (MemoryStream output = new MemoryStream()) {
				DocTableRowActions rowActions = new DocTableRowActions(output, row, Grids.Peek());
				rowActions.CreateTableRowPropertyModifiers(TableDepth, tableStyleIndex);
				DocTableActions tableActions = new DocTableActions(output, row.Table);
				tableActions.CreateTablePropertyModifiers();
				return output.ToArray();
			}
		}
		int GetParentSectionWidthInModelUnits(Table table) {
			DocumentModelPosition pos = DocumentModelPosition.FromParagraphStart(table.PieceTable, table.FirstRow.FirstCell.StartParagraphIndex);
			SectionIndex sectionIndex = table.DocumentModel.FindSectionIndex(pos.LogPosition);
			Section section = table.DocumentModel.Sections[sectionIndex];
			SectionMargins sectionMargins = section.Margins;
			int margins = sectionMargins.Left + sectionMargins.Right;
			return section.Page.Width - margins;
		}
	}
	#endregion
	#region DocNotesExporter
	public class DocNotesExporter {
		#region Fields
		List<int> footNoteReferences;
		List<int> footNotePositions;
		List<int> endNoteReferences;
		List<int> endNotePositions;
		List<bool> footNoteFlags;
		List<bool> endNoteFlags;
		#endregion
		public DocNotesExporter() {
			this.footNoteReferences = new List<int>();
			this.footNotePositions = new List<int>();
			this.endNoteReferences = new List<int>();
			this.endNotePositions = new List<int>();
			this.footNoteFlags = new List<bool>();
			this.endNoteFlags = new List<bool>();
		}
		#region Properties
		protected List<int> FootNoteReferences { get { return footNoteReferences; } }
		public List<int> FootNotePositions { get { return footNotePositions; } }
		protected List<int> EndNoteReferences { get { return endNoteReferences; } }
		public List<int> EndNotePositions { get { return endNotePositions; } }
		protected List<bool> FootNoteFlags { get { return footNoteFlags; } }
		protected List<bool> EndNoteFlags { get { return endNoteFlags; } }
		#endregion
		public void FinishFootNotePositions(int lastFootNoteEnd, int lastPosition) {
			FootNotePositions.Add(lastFootNoteEnd);
			FootNotePositions.Add(lastPosition);
		}
		public void FinishEndNotePositions(int lastEndNoteEnd) {
			EndNotePositions.Add(lastEndNoteEnd - 1);
			EndNotePositions.Add(lastEndNoteEnd);
		}
		public void AddFootNoteReferenceEntry(int characterPosition, bool isAutoNumbered) {
			FootNoteReferences.Add(characterPosition);
			this.footNoteFlags.Add(isAutoNumbered);
		}
		public void AddEndNoteReferenceEntry(int characterPosition, bool isAutoNumbered) {
			EndNoteReferences.Add(characterPosition);
			this.endNoteFlags.Add(isAutoNumbered);
		}
		public void FinishFootNoteReferences(int lastCharacterPosition) {
			FootNoteReferences.Add(lastCharacterPosition);
		}
		public void FinishEndNoteReferences(int lastCharacterPosition) {
			EndNoteReferences.Add(lastCharacterPosition);
		}
		public void ExportFootNoteTables(FileInformationBlock fib, BinaryWriter writer) {
			fib.FootNotesReferenceOffset = (int)writer.BaseStream.Position;
			ExportNoteReferences(writer, FootNoteReferences, FootNoteFlags);
			fib.FootNotesReferenceSize = (int)(writer.BaseStream.Position - fib.FootNotesReferenceOffset);
			fib.FootNotesTextOffset = (int)writer.BaseStream.Position;
			ExportNotePositions(writer, FootNotePositions);
			fib.FootNotesTextSize = (int)(writer.BaseStream.Position - fib.FootNotesTextOffset);
		}
		public void ExportEndNoteTables(FileInformationBlock fib, BinaryWriter writer) {
			fib.EndNotesReferenceOffset = (int)writer.BaseStream.Position;
			ExportNoteReferences(writer, EndNoteReferences, EndNoteFlags);
			fib.EndNotesReferenceSize = (int)(writer.BaseStream.Position - fib.EndNotesReferenceOffset);
			fib.EndnotesTextOffset = (int)writer.BaseStream.Position;
			ExportNotePositions(writer, EndNotePositions);
			fib.EndnotesTextSize = (int)(writer.BaseStream.Position - fib.EndnotesTextOffset);
		}
		void ExportNoteReferences(BinaryWriter writer, List<int> references, List<bool> flags) {
			int count = references.Count;
			for (int i = 0; i < count; i++)
				writer.Write(references[i]);
			count = flags.Count;
			for (int i = 0; i < count; i++) {
				if (flags[i])
					writer.Write((short)(i + 1));
				else
					writer.Write(0);
			}
		}
		void ExportNotePositions(BinaryWriter writer, List<int> positions) {
			int count = positions.Count;
			for (int i = 0; i < count; i++)
				writer.Write(positions[i]);
		}
	}
	#endregion
	#region DocFloatingObjectsExporter
	public class DocFloatingObjectsExporter {
		#region Fields
		DocContentState state;
		OfficeArtContent artContent;
		FileShapeAddressesExporter shapeAddressesExporter;
		TextBoxesExporter textBoxesExporter;
		#endregion
		public DocFloatingObjectsExporter(DocumentModelUnitConverter unitConverter) {
			this.shapeAddressesExporter = new FileShapeAddressesExporter(unitConverter);
			this.textBoxesExporter = new TextBoxesExporter();
			this.artContent = new OfficeArtContent();
			this.artContent.InitMainDocumentDrawing();
		}
		#region Properties
		protected internal DocContentState State { get { return state; } set { SetState(value); } }
		protected internal OfficeArtContent OfficeArtContent { get { return artContent; } }
		protected internal FileShapeAddressesExporter ShapeAddressesExporter { get { return shapeAddressesExporter; } }
		protected internal TextBoxesExporter TextBoxesExporter { get { return textBoxesExporter; } }
		#endregion
		void SetState(DocContentState state) {
			this.state = state;
			ShapeAddressesExporter.State = state;
			TextBoxesExporter.State = state;
		}
		public void RegisterFloatingObject(FloatingObjectAnchorRun run, int characterPosition) {
			int shapeIdentifier = ShapeAddressesExporter.CreateShapeIdentifier(run, characterPosition);
			RegisterFloatingObjectCore(run, characterPosition, shapeIdentifier);
		}
		void RegisterFloatingObjectCore(FloatingObjectAnchorRun run, int characterPosition, int shapeIdentifier) {
			PictureFloatingObjectContent pictureContent = run.Content as PictureFloatingObjectContent;
			if (pictureContent != null)
				RegisterPictureFloatingObject(pictureContent.Image, shapeIdentifier, run, ShapeAddressesExporter.UnitConverter);
			TextBoxFloatingObjectContent textBoxContent = run.Content as TextBoxFloatingObjectContent;
			if (textBoxContent != null)
				RegisterTextBoxFloatingObject(textBoxContent, shapeIdentifier, run, ShapeAddressesExporter.UnitConverter);
		}
		void RegisterPictureFloatingObject(OfficeImage image, int shapeIdentifier, FloatingObjectAnchorRun run, DocumentModelUnitConverter unitConverter) {
			OfficeArtContent.InsertPictureFloatingObject(image, State, shapeIdentifier, run, unitConverter);
		}
		void RegisterTextBoxFloatingObject(TextBoxFloatingObjectContent content, int shapeIdentifier, FloatingObjectAnchorRun run, DocumentModelUnitConverter unitConverter) {
			int textBoxId = TextBoxesExporter.RegisterTextBoxFloatingObject(content, shapeIdentifier);
			OfficeArtContent.InsertTextBoxFloatingObject(State, shapeIdentifier, textBoxId, run, unitConverter);
		}
		public void ExportFloatingObjectsInfo(FileInformationBlock fib, BinaryWriter writer, BinaryWriter embeddedWriter) {
			ShapeAddressesExporter.ExportFileShapeAddresses(fib, writer);
			TextBoxesExporter.ExportTextBoxesTables(fib, writer);
			ExportOfficeArtContent(fib, writer, embeddedWriter);
		}
		protected internal virtual void ExportOfficeArtContent(FileInformationBlock fib, BinaryWriter writer, BinaryWriter embeddedWriter) {
			fib.DrawingObjectTableOffset = (int)writer.BaseStream.Position;
			OfficeArtContent.DrawingContainer.FileDrawingBlock.HeaderFloatingObjectsCount = ShapeAddressesExporter.HeaderAddresses.AddressesCount;
			OfficeArtContent.DrawingContainer.FileDrawingBlock.MainDocumentFloatingObjectsCount = ShapeAddressesExporter.MainDocumentAddresses.AddressesCount;
			OfficeArtContent.Write(writer, embeddedWriter);
			fib.DrawingObjectTableSize = (int)(writer.BaseStream.Position - fib.DrawingObjectTableOffset);
		}
		public void Finish(int lastCharacterPosition) {
			ShapeAddressesExporter.Finish(lastCharacterPosition);
			TextBoxesExporter.FinishDocument(lastCharacterPosition);
		}
	}
	#endregion
	#region FileShapeAddressesExporter
	public class FileShapeAddressesExporter {
		#region Fields
		DocumentModelUnitConverter unitConverter;
		DocContentState state;
		FileShapeAddressTable mainDocumentAddresses;
		FileShapeAddressTable headerAddresses;
		#endregion
		public FileShapeAddressesExporter(DocumentModelUnitConverter unitConverter) {
			this.unitConverter = unitConverter;
			this.mainDocumentAddresses = new FileShapeAddressTable();
			this.headerAddresses = new FileShapeAddressTable();
		}
		#region Properties
		protected internal DocContentState State { get { return state; } set { state = value; } }
		protected internal DocumentModelUnitConverter UnitConverter { get { return unitConverter; } }
		protected internal FileShapeAddressTable MainDocumentAddresses { get { return mainDocumentAddresses; } }
		protected internal FileShapeAddressTable HeaderAddresses { get { return headerAddresses; } }
		#endregion
		public int CreateShapeIdentifier(FloatingObjectAnchorRun run, int characterPosition) {
			int shapeIdentifier = CalcShapeIdentifier();						
			ExportFloatingObjectProperties(run.FloatingObjectProperties, characterPosition, shapeIdentifier);
			return shapeIdentifier;
		}
		protected internal virtual void ExportFloatingObjectProperties(FloatingObjectProperties floatingObjectProperties, int characterPosition, int shapeIdentifier) {
			FileShapeAddress address = new FileShapeAddress();
			address.ShapeIdentifier = shapeIdentifier;
			SetFileShapeAddressProperties(floatingObjectProperties, address);
			if (State == DocContentState.MainDocument)
				MainDocumentAddresses.AddEntry(characterPosition, address);
			if (State == DocContentState.HeadersFootersStory)
				HeaderAddresses.AddEntry(characterPosition, address);
		}
		protected internal virtual void ExportFileShapeAddresses(FileInformationBlock fib, BinaryWriter writer) {
			if (MainDocumentAddresses.AddressesCount > 0) {
				fib.MainDocumentFileShapeTableOffset = (int)writer.BaseStream.Position;
				MainDocumentAddresses.Write(writer);
				fib.MainDocumentFileShapeTableSize = (int)(writer.BaseStream.Position - fib.MainDocumentFileShapeTableOffset);
			}
			if (HeaderAddresses.AddressesCount > 0) {
				fib.HeadersFootersFileShapeTableOffset = (int)writer.BaseStream.Position;
				HeaderAddresses.Write(writer);
				fib.HeadersFootersFileShapeTableSize = (int)(writer.BaseStream.Position - fib.HeadersFootersFileShapeTableOffset);
			}
		}
		void SetFileShapeAddressProperties(FloatingObjectProperties floatingObjectProperties, FileShapeAddress address) {
			SetLocked(floatingObjectProperties, address);
			SetHorizontalPositionType(floatingObjectProperties, address);
			SetVerticalPositionType(floatingObjectProperties, address);
			SetTextWrapSide(floatingObjectProperties, address);
			SetTextWrapType(floatingObjectProperties, address);
			SetIsBehindText(floatingObjectProperties, address);
			SetLeft(floatingObjectProperties, address);
			SetTop(floatingObjectProperties, address);
			SetRight(floatingObjectProperties, address);
			SetBottom(floatingObjectProperties, address);
		}
		void SetLocked(FloatingObjectProperties floatingObjectProperties, FileShapeAddress address) {
			if (floatingObjectProperties.UseLocked)
				address.Locked = floatingObjectProperties.Locked;
		}
		void SetHorizontalPositionType(FloatingObjectProperties floatingObjectProperties, FileShapeAddress address) {
			if (floatingObjectProperties.UseHorizontalPositionType)
				address.HorisontalPositionType = floatingObjectProperties.HorizontalPositionType;
		}
		void SetVerticalPositionType(FloatingObjectProperties floatingObjectProperties, FileShapeAddress address) {
			if (floatingObjectProperties.UseVerticalPositionType)
				address.VericalPositionType = floatingObjectProperties.VerticalPositionType;
		}
		void SetTextWrapSide(FloatingObjectProperties floatingObjectProperties, FileShapeAddress address) {
			if (floatingObjectProperties.UseTextWrapSide)
				address.TextWrapSide = floatingObjectProperties.TextWrapSide;
		}
		void SetTextWrapType(FloatingObjectProperties floatingObjectProperties, FileShapeAddress address) {
			if (floatingObjectProperties.TextWrapType == FloatingObjectTextWrapType.None)
				address.TextWrapType = DocFloatingObjectTextWrapTypeCalculator.WrapTypeBehindText;
			else
				address.TextWrapType = floatingObjectProperties.TextWrapType;
		}
		void SetIsBehindText(FloatingObjectProperties floatingObjectProperties, FileShapeAddress address) {
			address.IsBehindDoc = floatingObjectProperties.IsBehindDoc;
		}
		void SetLeft(FloatingObjectProperties floatingObjectProperties, FileShapeAddress address) {			
			address.Left = UnitConverter.ModelUnitsToTwips(floatingObjectProperties.Offset.X);
		}
		void SetRight(FloatingObjectProperties floatingObjectProperties, FileShapeAddress address) {
			address.Right = UnitConverter.ModelUnitsToTwips(floatingObjectProperties.Offset.X + floatingObjectProperties.ActualSize.Width);
		}
		void SetTop(FloatingObjectProperties floatingObjectProperties, FileShapeAddress address) {
			address.Top = UnitConverter.ModelUnitsToTwips(floatingObjectProperties.Offset.Y);
		}
		void SetBottom(FloatingObjectProperties floatingObjectProperties, FileShapeAddress address) {
			address.Bottom = UnitConverter.ModelUnitsToTwips(floatingObjectProperties.Offset.Y + floatingObjectProperties.ActualSize.Height);
		}
		public void Finish(int lastCharacterPosition) {
			MainDocumentAddresses.Finish(lastCharacterPosition);
			HeaderAddresses.Finish(lastCharacterPosition);
		}
		int CalcShapeIdentifier() {
			return State == DocContentState.MainDocument ? OfficeArtConstants.DefaultMainDocumentShapeIdentifier + MainDocumentAddresses.AddressesCount + 1
				: OfficeArtConstants.DefaultHeaderShapeIdentifier + HeaderAddresses.AddressesCount + 1;
		}
	}
	#endregion
	#region TextBoxesExporter
	public class TextBoxesExporter {
		#region Fields
		DocContentState state;
		TextBoxesExporterState mainDocument;
		TextBoxesExporterState headerDocument;
		TextBoxesExporterState activeState;
		#endregion
		public TextBoxesExporter() {
			this.mainDocument = new TextBoxesExporterState();
			this.headerDocument = new TextBoxesExporterState();
		}
		#region Properties
		public DocContentState State { get { return state; } 
			set {
				if (value == DocContentState.TextBoxes)
					activeState = mainDocument;
				else if (value == DocContentState.HeaderTextBoxes)
					activeState = headerDocument;
				else
					activeState = null;
				state = value;
			} 
		}
		public TextBoxesExporterState ActiveExporterState { get { return activeState; } }
		protected internal TextBoxesExporterState MainDocument { get { return mainDocument; } }
		protected internal TextBoxesExporterState HeaderDocument { get { return headerDocument; } }
		#endregion
		public int RegisterTextBoxFloatingObject(TextBoxFloatingObjectContent content, int shapeIdentifier) {
			TextBoxesExporterState exporterState;
			if (State == DocContentState.MainDocument)
				exporterState = MainDocument;
			else if (State == DocContentState.HeadersFootersStory)
				exporterState = HeaderDocument;
			else {
				Exceptions.ThrowInternalException();
				return -1;
			}
			return exporterState.RegisterTextBoxFloatingObject(content, shapeIdentifier);
		}
		public void AddTextBoxTableEntry(int characterPosition, int shapeIdentifier, bool isLastInChain) {
			ActiveExporterState.AddTextBoxTableEntry(characterPosition, shapeIdentifier, isLastInChain);
		}
		public Dictionary<int, TextBoxContentType> GetCurrentTextBoxes() {
			if (ActiveExporterState != null)
				return ActiveExporterState.TextBoxes;
			return new Dictionary<int, TextBoxContentType>();
		}
		public void ExportTextBoxesTables(FileInformationBlock fib, BinaryWriter writer) {
			ExportMainDocumentTextBoxesTable(fib, writer);
			ExportHeaderTextBoxesTable(fib, writer);
		}
		public bool ShouldInsertEmptyParagraph() {
			return (ActiveExporterState != null && ActiveExporterState.TextBoxes.Count > 0);
		}
		void ExportMainDocumentTextBoxesTable(FileInformationBlock fib, BinaryWriter writer) {
			fib.MainDocumentTextBoxesTextOffset = (int)writer.BaseStream.Position;
			int count = MainDocument.Positions.Count;
			for (int i = 0; i < count; i++)
				writer.Write(MainDocument.Positions[i]);
			count = MainDocument.TextBoxesIdentifiers.Count;
			for (int i = 0; i < count; i++)
				MainDocument.TextBoxesIdentifiers[i].Write(writer);
			fib.MainDocumentTextBoxesTextSize = (int)(writer.BaseStream.Position - fib.MainDocumentTextBoxesTextOffset);
			fib.MainTextBoxBreakTableOffset = (int)writer.BaseStream.Position;
			MainDocument.BreakDescriptors.Write(writer);
			fib.MainTextBoxBreakTableSize = (int)(writer.BaseStream.Position - fib.MainTextBoxBreakTableOffset);
		}
		void ExportHeaderTextBoxesTable(FileInformationBlock fib, BinaryWriter writer) {
			fib.HeaderTextBoxesTextOffset = (int)writer.BaseStream.Position;
			int count = HeaderDocument.Positions.Count;
			for (int i = 0; i < count; i++)
				writer.Write(HeaderDocument.Positions[i]);
			count = HeaderDocument.TextBoxesIdentifiers.Count;
			for (int i = 0; i < count; i++)
				HeaderDocument.TextBoxesIdentifiers[i].Write(writer);
			fib.HeaderTextBoxesTextSize = (int)(writer.BaseStream.Position - fib.HeaderTextBoxesTextOffset);
			fib.HeadersFootersTextBoxBreakTableOffset = (int)writer.BaseStream.Position;
			HeaderDocument.BreakDescriptors.Write(writer);
			fib.HeadersFootersTextBoxBreakTableSize = (int)(writer.BaseStream.Position - fib.HeadersFootersTextBoxBreakTableOffset);
		}
		public void FinishCurrentState(int characterPosition) {
			ActiveExporterState.FinishState(characterPosition);
		}
		public void FinishDocument(int lastCharacterPosition) {
			MainDocument.FinishDocument(lastCharacterPosition);
			HeaderDocument.FinishDocument(lastCharacterPosition);
		}
	}
	#endregion
	#region TextBoxExporterState
	public class TextBoxesExporterState {
		Dictionary<int, TextBoxContentType> textBoxes;
		List<int> positions;
		List<FileTextBoxIdentifier> textBoxesIdentifiers;
		BreakDescriptorTable breakDescriptors;
		public TextBoxesExporterState() {
			this.textBoxes = new Dictionary<int, TextBoxContentType>();
			this.positions = new List<int>();
			this.textBoxesIdentifiers = new List<FileTextBoxIdentifier>();
			this.breakDescriptors = new BreakDescriptorTable();
		}
		public Dictionary<int, TextBoxContentType> TextBoxes { get { return textBoxes; } }
		public List<int> Positions { get { return positions; } }
		public List<FileTextBoxIdentifier> TextBoxesIdentifiers { get { return textBoxesIdentifiers; } }
		public BreakDescriptorTable BreakDescriptors { get { return breakDescriptors; } }
		public int RegisterTextBoxFloatingObject(TextBoxFloatingObjectContent content, int shapeIdentifier) {
			if (!TextBoxes.ContainsKey(shapeIdentifier)) {
				TextBoxes.Add(shapeIdentifier, content.TextBox);
				return TextBoxes.Count;
			}
			Exceptions.ThrowInternalException();
			return -1;
		}
		public void AddTextBoxTableEntry(int characterPosition, int shapeIdentifier, bool isLastInChain) {
			AddTextBoxPosition(characterPosition);
			AddTextBoxReusableInfo(shapeIdentifier, isLastInChain);
			AddBreakDescriptor(characterPosition);
		}
		void AddTextBoxPosition(int position) {
			Positions.Add(position);
		}
		void AddTextBoxReusableInfo(int shapeIdentifier, bool isLastInChain) {
			TextBoxesIdentifiers.Add(new FileTextBoxIdentifier(shapeIdentifier, isLastInChain));
		}
		void AddBreakDescriptor(int position) {
			BreakDescriptors.AddEntry(position);
		}
		public void FinishState(int lastCharacterPosition) {
			AddTextBoxPosition(lastCharacterPosition);
			AddTextBoxReusableInfo(0, true);
			BreakDescriptors.Finish(lastCharacterPosition);
		}
		public void FinishDocument(int lastCharacterPosition) {
			if (Positions.Count > 0) {
				Positions.Add(lastCharacterPosition);
				BreakDescriptors.CharacterPositions.Add(lastCharacterPosition);
			}
		}
	}
	#endregion
	#region FileTextBoxIdentifier
	public class FileTextBoxIdentifier {
		#region TextBoxReusableInfo
		public struct TextBoxReusableInfo {
			int nextItem;
			int reusableChainLength;
			public static readonly TextBoxReusableInfo NonReusable = new TextBoxReusableInfo(1, 0);
			public static readonly TextBoxReusableInfo LastInfoInChain = new TextBoxReusableInfo(-1, 0);
			public TextBoxReusableInfo(int nextItem, int reusableChainLength) {
				this.nextItem = nextItem;
				this.reusableChainLength = reusableChainLength;
			}
			#region Properties
			public int NextItem { get { return nextItem; } set { nextItem = value; } }
			public int ReusableChainLength { get { return reusableChainLength; } set { reusableChainLength = value; } }
			#endregion
			public void Write(BinaryWriter writer) {
				writer.Write(NextItem);
				writer.Write(ReusableChainLength);
			}
		}
		#endregion
		#region Fields
		const int itxbxsDest = -1;
		const int txidUndo = 0;
		TextBoxReusableInfo reusableInfo;
		bool isReusable = false;
		int shapeIdentifier;
		#endregion
		public FileTextBoxIdentifier(int shapeIdentifier, bool isLastInChain) {
			this.shapeIdentifier = shapeIdentifier;
			this.reusableInfo = isLastInChain ? TextBoxReusableInfo.LastInfoInChain : TextBoxReusableInfo.NonReusable;
		}
		#region Properties
		public TextBoxReusableInfo ReusableInfo { get { return reusableInfo; } }
		public bool IsReusable { get { return isReusable; } set { isReusable = value; } }
		public int ShapeIdentifier { get { return shapeIdentifier; } set { shapeIdentifier = value; } }
		#endregion
		public void Write(BinaryWriter writer) {
			ReusableInfo.Write(writer);
			writer.Write(Convert.ToInt16(isReusable));
			writer.Write(itxbxsDest);
			writer.Write(shapeIdentifier);
			writer.Write(txidUndo);
		}
	}
	#endregion
}
