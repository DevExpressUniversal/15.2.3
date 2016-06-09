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
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Utils.StructuredStorage.Writer;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Import.Doc;
using DevExpress.XtraRichEdit.Fields;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Export.Doc {
	#region TextCodes
	public static class TextCodes {
		public static readonly char InlinePicture = '\x01';
		public static readonly char AutoNumberedFootNoteReference = '\x02';
		public static readonly char FootNoteSeparatorCharacter = '\x03';
		public static readonly char FootNoteContinuationCharacter = '\x04';
		public static readonly char AnnotationReference = '\x05';
		public static readonly char TableUnitMark = '\x07';
		public static readonly char FloatingObjectAnchor = '\x08';
		public static readonly char SectionMark = '\x0c';
		public static readonly char ParagraphMark = '\x0d';
		public static readonly char ColumnBreak = '\x0e';
		public static readonly char FieldBegin = '\x13';
		public static readonly char FieldSeparator = '\x14';
		public static readonly char FieldEnd = '\x15';
		public static readonly char SpecialSymbol = '\x28';
	}
	#endregion
	#region DocExporter
	public class DocExporter : DocumentModelExporter, IDisposable, IDocExporter {
		#region TextStreamBorders
		protected internal class TextStreamBorders {
			public int MainDocumentLength { get; set; }
			public int FootNotesTextLength { get; set; }
			public int HeadersFootersTextLength { get; set ; }
			public int CommentsTextLength { get; set; }
			public int EndNotesTextLength { get; set; }
			public int TextBoxesTextLength { get; set; }
			public int HeaderTextBoxesTextLength { get; set; }
			public bool ShouldWriteEmptyParagraphAtEnd() {
				return FootNotesTextLength != 0 ||
					HeadersFootersTextLength != 0 ||
					CommentsTextLength != 0 ||
					EndNotesTextLength != 0 ||
					TextBoxesTextLength != 0 ||
					HeaderTextBoxesTextLength != 0;
			}
		}
		#endregion
		#region Fields
		readonly DocDocumentExporterOptions options;
		const int textStartOffset = 0x0800;
		const int emptyShapeIdentifier = 0;
		Stream outputStream;
		BinaryWriter mainStreamWriter;
		BinaryWriter tableStreamWriter;
		BinaryWriter dataStreamWriter;
		DocDataWriter docDataWriter;
		ExportImagesIterator imagesIterator;
		ExportFieldsIterator fieldsIterator;
		PieceTable activePieceTable;
		DocHeadersFootersPositions headersFootersPositions;
		int currentCharacterPosition;
		int currentDocumentPartStart;
		TextStreamBorders textStreamBorders;
		#endregion
		#region Constructors
		public DocExporter(DocumentModel documentModel, DocDocumentExporterOptions options)
			: base(documentModel) {
			Guard.ArgumentNotNull(options, "options");
			this.options = options;
		}
		#endregion
		#region Properties
		protected internal override bool ShouldExportHiddenText { get { return true; } }
		protected internal DocDocumentExporterOptions Options { get { return options; } }
		protected BinaryWriter MainStreamWriter { get { return mainStreamWriter; } }
		protected BinaryWriter TableStreamWriter { get { return tableStreamWriter; } }
		protected BinaryWriter DataStreamWriter { get { return dataStreamWriter; } }
		protected internal DocDataWriter DocDataWriter { get { return docDataWriter; } }
		protected ExportImagesIterator ImagesIterator { get { return imagesIterator; } }
		protected ExportFieldsIterator FieldsIterator { get { return fieldsIterator; } }
		protected PieceTable ActivePieceTable { get { return activePieceTable; } set { SetActivePieceTable(value); } }
		protected TextStreamBorders TextBorders { get { return textStreamBorders; } }
		public int CurrentCharacterPosition { get { return currentCharacterPosition; } set { currentCharacterPosition = value; } }
		public int CurrentDocumentPartStart { get { return currentDocumentPartStart; } set { currentDocumentPartStart = value; } }
		protected DocHeadersFootersPositions HeadersFootersPositions { get { return headersFootersPositions; } }
		#endregion
		public virtual void Export(Stream outputStream) {
			this.outputStream = outputStream;
			Export();
		}
		public override void Export() {
			if (this.outputStream == null)
				Exceptions.ThrowInternalException();
			Initialize();
			CreateDocumentContent();
			Write();
		}
		void Initialize() {
			ChunkedMemoryStream mainStream = new ChunkedMemoryStream();
			mainStream.Seek(FileInformationBlock.FIBSize, SeekOrigin.Begin);
			ChunkedMemoryStream tableStream = new ChunkedMemoryStream();
			ChunkedMemoryStream dataStream = new ChunkedMemoryStream();
			this.mainStreamWriter = new BinaryWriter(mainStream);
			this.tableStreamWriter = new BinaryWriter(tableStream);
			this.dataStreamWriter = new BinaryWriter(dataStream);
			this.docDataWriter = new DocDataWriter(DataStreamWriter, Options, DocumentModel);
			this.imagesIterator = new ExportImagesIterator();
			this.fieldsIterator = new ExportFieldsIterator();
			this.textStreamBorders = new TextStreamBorders();
		}
		void Write() {
			StructuredStorageWriter structuredStorageWriter = new StructuredStorageWriter();
			AddStreamDirectoryEntry(structuredStorageWriter, DocumentStreams.MainStreamName, MainStreamWriter);
			AddStreamDirectoryEntry(structuredStorageWriter, DocumentStreams.Stream1TableName, TableStreamWriter);
			if (DataStreamWriter.BaseStream.Length > 0)
				AddStreamDirectoryEntry(structuredStorageWriter, DocumentStreams.DataStreamName, DataStreamWriter);
			structuredStorageWriter.Write(outputStream);
		}
		void AddStreamDirectoryEntry(StructuredStorageWriter writer, string name, BinaryWriter binaryWriter) {
			writer.RootDirectoryEntry.AddStreamDirectoryEntry(name, binaryWriter.BaseStream);
			binaryWriter.Flush();
		}
		protected internal void CreateDocumentContent() {
			DocDataWriter.StylesExporter.CreateStyleSheet();
			DocDataWriter.ListsExporter.CreateLists(DocDataWriter.StylesExporter);
			CreateDocumentContentCore();
			FinishDocumentContentCreation();
		}
		void CreateDocumentContentCore() {
			ExportMainText();
			ExportFootNotes();
			ExportHeadersFooters();
			ExportCommentsContent();
			ExportEndNotes();
			ExportTextBoxes();
		}
		void FinishDocumentContentCreation() {
			if (TextBorders.ShouldWriteEmptyParagraphAtEnd())
				ExportEmptyParagraphWithLastParagraphProperties();
			DocDataWriter.Finish(CurrentCharacterPosition);
			AlignMainStreamOffset();
			MainStreamWriter.Write(DocDataWriter.GetFormattedDiskPages());
			AlignMainStreamOffset();
			DocDataWriter.UpdateSectionsOffsets((int)MainStreamWriter.BaseStream.Position);
			MainStreamWriter.Write(DocDataWriter.GetSectionProperties());
			FileInformationBlock fib = CreateFileInformationBlock();
			fib.Write(MainStreamWriter);
		}
		void ExportMainText() {
			UpdateState(DocContentState.MainDocument);
			ActivePieceTable = DocumentModel.MainPieceTable;
			base.Export();
			TextBorders.MainDocumentLength = CurrentCharacterPosition;
		}
		void ExportCommentsContent() {
			if (DocumentModel.MainPieceTable.Comments.Count == 0)
				return;
			UpdateState(DocContentState.Comments);			
			ExportCommentsContentCore();
			ExportEmptyParagraph();
			TextBorders.CommentsTextLength = GetRelativeCharacterPosition();
			DocDataWriter.CommentsExporter.FinishCommentPositions(TextBorders.CommentsTextLength);
		}
		void ExportCommentsContentCore() {
			int count = DocumentModel.MainPieceTable.Comments.Count;
			for (int i = 0; i < count; i++) {
				CommentContentType commentContent = DocumentModel.MainPieceTable.Comments[i].Content;
				if (!commentContent.IsReferenced)
					continue;
				DocDataWriter.CommentsExporter.CommentsContentPositions.Add(GetRelativeCharacterPosition());				
				ActivePieceTable = commentContent.PieceTable;
				PerformExportPieceTable(ActivePieceTable, ExportPieceTable);
			}
		}
		void ExportFootNotes() {
			if (!DocumentModel.DocumentCapabilities.FootNotesAllowed || DocumentModel.FootNotes.Count == 0)
				return;
			UpdateState(DocContentState.Footnotes);
			ExportEmptyParagraphCore();
			ExportFootNotesCore();
			ExportEmptyParagraph();
			TextBorders.FootNotesTextLength = GetRelativeCharacterPosition();
			DocDataWriter.NotesExporter.FinishFootNotePositions(TextBorders.FootNotesTextLength - 1, CurrentCharacterPosition);
		}
		void ExportFootNotesCore() {
			int count = DocumentModel.FootNotes.Count;
			for (int i = 0; i < count; i++) {
				FootNote footNote = DocumentModel.FootNotes[i];
				if (!footNote.IsReferenced)
					continue;
				DocDataWriter.NotesExporter.FootNotePositions.Add(GetRelativeCharacterPosition());
				ActivePieceTable = footNote.PieceTable;
				PerformExportPieceTable(ActivePieceTable, ExportPieceTable);
			}
		}
		void ExportHeadersFooters() {
			if (!ShouldExportHeadersFootersInfo())
				return;
			this.headersFootersPositions = new DocHeadersFootersPositions();
			UpdateState(DocContentState.HeadersFootersStory);
			ExportFootnoteSeparators();
			DocumentModel.Sections.ForEach(ExportSectionHeadersFootersFiltered);
			ExportGuardParagraphMark();
			TextBorders.HeadersFootersTextLength = GetRelativeCharacterPosition();
			HeadersFootersPositions.CharacterPositions.Add(TextBorders.HeadersFootersTextLength + 2);
		}
		bool ShouldExportHeadersFootersInfo() {
			if (DocumentModel.Headers.Count > 0 || DocumentModel.Footers.Count > 0)
				return true;
			if (DocumentModel.FootNotes.Count > 0 || DocumentModel.EndNotes.Count > 0)
				return true;
			return false;
		}
		void ExportEndNotes() {
			if (!DocumentModel.DocumentCapabilities.EndNotesAllowed || DocumentModel.EndNotes.Count == 0)
				return;
			UpdateState(DocContentState.Endnotes);
			ExportEmptyParagraphCore();
			ExportEndNotesCore();
			ExportEmptyParagraph();
			TextBorders.EndNotesTextLength = GetRelativeCharacterPosition();
			DocDataWriter.NotesExporter.FinishEndNotePositions(TextBorders.EndNotesTextLength);
		}
		void ExportEndNotesCore() {
			int count = DocumentModel.EndNotes.Count;
			for (int i = 0; i < count; i++) {
				EndNote endNote = DocumentModel.EndNotes[i];
				if (!endNote.IsReferenced)
					continue;
				DocDataWriter.NotesExporter.EndNotePositions.Add(GetRelativeCharacterPosition());
				ActivePieceTable = endNote.PieceTable;
				PerformExportPieceTable(ActivePieceTable, ExportPieceTable);
			}
		}
		void ExportTextBoxes() {
			ExportTextBoxesCore(DocContentState.TextBoxes);
			ExportTextBoxesCore(DocContentState.HeaderTextBoxes);
		}
		void ExportTextBoxesCore(DocContentState state) {
			UpdateState(state);
			TextBoxesExporter exporter = DocDataWriter.FloatingObjectsExporter.TextBoxesExporter;
			Dictionary<int, TextBoxContentType> textBoxes = exporter.GetCurrentTextBoxes();
			ExportTextBoxesContent(textBoxes);
			if (textBoxes.Count > 0)
				exporter.FinishCurrentState(GetRelativeCharacterPosition());
			if (exporter.ShouldInsertEmptyParagraph())
				ExportEmptyParagraph();
			SetTextBoxesLength(state);
		}
		void SetTextBoxesLength(DocContentState state) {
			if (state == DocContentState.TextBoxes)
				TextBorders.TextBoxesTextLength = GetRelativeCharacterPosition();
			if (state == DocContentState.HeaderTextBoxes)
				TextBorders.HeaderTextBoxesTextLength = GetRelativeCharacterPosition();
		}
		void ExportTextBoxesContent(Dictionary<int, TextBoxContentType> textBoxes) {
			TextBoxesExporter textBoxExporter = DocDataWriter.FloatingObjectsExporter.TextBoxesExporter;
			foreach (KeyValuePair<int, TextBoxContentType> pair in textBoxes) {
				textBoxExporter.AddTextBoxTableEntry(GetRelativeCharacterPosition(), pair.Key, false);
				ActivePieceTable = pair.Value.PieceTable;
				PerformExportPieceTable(ActivePieceTable, ExportPieceTable);
				ExportEmptyParagraph();
			}
		}
	   void ExportFootnoteSeparators() {
			HeadersFootersPositions.CharacterPositions.Add(GetRelativeCharacterPosition());
			AddDefaultSeparators(); 
			AddDefaultSeparators(); 
		}
		void AddDefaultSeparators() {
			ExportDefaultFootnoteSeparator();
			HeadersFootersPositions.CharacterPositions.Add(GetRelativeCharacterPosition());
			ExportDefaultFootnoteContinuation();
			HeadersFootersPositions.CharacterPositions.Add(GetRelativeCharacterPosition());
			HeadersFootersPositions.CharacterPositions.Add(GetRelativeCharacterPosition());
		}
		void ExportDefaultCommentSeparators() {
			string commentSeparator = new string(TextCodes.AnnotationReference, 1);
			ExportTextRunBaseCore(null, commentSeparator, true);
		}
		void ExportDefaultFootnoteSeparator() {
			ExportItemWithGuardParagraphMark(new string(TextCodes.FootNoteSeparatorCharacter, 1));
		}
		void ExportDefaultFootnoteContinuation() {
			ExportItemWithGuardParagraphMark(new string(TextCodes.FootNoteContinuationCharacter, 1));
		}
		void ExportItemWithGuardParagraphMark(string item) {
			ExportEmptyParagraphCore();
			ExportTextRunBaseCore(null, item, true);
			ExportEmptyParagraphRun();
			ExportGuardParagraphMark();
		}
		void ExportSectionHeadersFootersFiltered(Section section) {
			if (ShouldExportSection(section))
				ExportSectionHeadersFooters(section);
		}
		protected internal override void ExportSectionHeadersFootersCore(Section section) {
			if (!DocumentModel.DocumentCapabilities.HeadersFootersAllowed)
				return;
			ExportEvenPageHeader(section);
			ExportOddPageHeader(section);
			ExportEvenPageFooter(section);
			ExportOddPageFooter(section);
			ExportFirstPageHeader(section);
			ExportFirstPageFooter(section);
		}
		void ExportEvenPageHeader(Section section) {
			if (section.InnerEvenPageHeader != null) {
				ActivePieceTable = section.InnerEvenPageHeader.PieceTable;
				ExportEvenPageHeader(section.InnerEvenPageHeader, section.Headers.IsLinkedToPrevious(HeaderFooterType.Even));
				ExportEmptyParagraph();
			}
			HeadersFootersPositions.CharacterPositions.Add(GetRelativeCharacterPosition());
		}
		void ExportOddPageHeader(Section section) {
			if (section.InnerOddPageHeader != null) {
				ActivePieceTable = section.InnerOddPageHeader.PieceTable;
				ExportOddPageHeader(section.InnerOddPageHeader, section.Headers.IsLinkedToPrevious(HeaderFooterType.Odd));
				ExportEmptyParagraph();
			}
			HeadersFootersPositions.CharacterPositions.Add(GetRelativeCharacterPosition());
		}
		void ExportEvenPageFooter(Section section) {
			if (section.InnerEvenPageFooter != null) {
				ActivePieceTable = section.InnerEvenPageFooter.PieceTable;
				ExportEvenPageFooter(section.InnerEvenPageFooter, section.Headers.IsLinkedToPrevious(HeaderFooterType.Even));
				ExportEmptyParagraph();
			}
			HeadersFootersPositions.CharacterPositions.Add(GetRelativeCharacterPosition());
		}
		void ExportOddPageFooter(Section section) {
			if (section.InnerOddPageFooter != null) {
				ActivePieceTable = section.InnerOddPageFooter.PieceTable;
				ExportOddPageFooter(section.InnerOddPageFooter, section.Headers.IsLinkedToPrevious(HeaderFooterType.Odd));
				ExportEmptyParagraph();
			}
			HeadersFootersPositions.CharacterPositions.Add(GetRelativeCharacterPosition());
		}
		void ExportFirstPageHeader(Section section) {
			if (section.InnerFirstPageHeader != null) {
				ActivePieceTable = section.InnerFirstPageHeader.PieceTable;
				ExportFirstPageHeader(section.InnerFirstPageHeader, section.Headers.IsLinkedToPrevious(HeaderFooterType.First));
				ExportEmptyParagraph();
			}
			HeadersFootersPositions.CharacterPositions.Add(GetRelativeCharacterPosition());
		}
		void ExportFirstPageFooter(Section section) {
			if (section.InnerFirstPageFooter != null) {
				ActivePieceTable = section.InnerFirstPageFooter.PieceTable;
				ExportFirstPageFooter(section.InnerFirstPageFooter, section.Headers.IsLinkedToPrevious(HeaderFooterType.First));
				ExportEmptyParagraph();
			}
			HeadersFootersPositions.CharacterPositions.Add(GetRelativeCharacterPosition());
		}
		void AlignMainStreamOffset() {
			if (MainStreamWriter.BaseStream.Position % DocContentBuilder.SectorSize != 0) {
				long offset = (MainStreamWriter.BaseStream.Position / DocContentBuilder.SectorSize + 1) * DocContentBuilder.SectorSize;
				MainStreamWriter.BaseStream.Seek(offset, SeekOrigin.Begin);
			}
		}
		protected FileInformationBlock CreateFileInformationBlock() {
			FileInformationBlock result = new FileInformationBlock();
			result.Flags = FileInformationFlags.TableStreamType | FileInformationFlags.ExtendedCharset | FileInformationFlags.QuickSaves;
			if (ImagesIterator.HasPictures)
				result.Flags |= FileInformationFlags.HasPictures;
			ExportStyles(result);
			ExportFootNoteTables(result);
			ExportCommentsTables(result);
			ExportSectionProperties(result);
			ExportEndNoteTables(result);
			ExportDocumentProperties(result);
			ExportDocumentVariables(result);
			ExportFonts(result);
			ExportComplexFileInformation(result);
			ExportFormattingBinTables(result);
			ExportHeadersFootersPositionsTable(result);
			ExportListFormatInformation(result);
			ExportListOverrideFormatInformation(result);
			ExportDocumentFormatRecords(result);
			ExportFieldTables(result);
			ExportBookmarks(result);
			ExportRangeEditPermissions(result);
			ExportFloatingObjectsInfo(result);
			ExportTextSettings(result);
			ExportRmdThreading(result);
			SetTextStreamBorders(result);
			return result;
		}
		void ExportStyles(FileInformationBlock fib) {
			fib.StyleSheetOffset = (int)TableStreamWriter.BaseStream.Position;
			DocDataWriter.StylesExporter.WriteStyleSheet(TableStreamWriter);
			fib.StyleSheetSize = (int)(TableStreamWriter.BaseStream.Position - fib.StyleSheetOffset);
		}
		void ExportFootNoteTables(FileInformationBlock fib) {
			DocDataWriter.NotesExporter.ExportFootNoteTables(fib, TableStreamWriter);
		}
		void ExportCommentsTables(FileInformationBlock fib) {
			DocDataWriter.CommentsExporter.ExportCommentsTables(fib, TableStreamWriter);
		}
		void ExportEndNoteTables(FileInformationBlock fib) {
			DocDataWriter.NotesExporter.ExportEndNoteTables(fib, TableStreamWriter);
		}
		void ExportSectionProperties(FileInformationBlock fib) {
			fib.SectionTableOffset = (int)TableStreamWriter.BaseStream.Position;
			DocDataWriter.WriteSectionPositions(TableStreamWriter);
			fib.SectionTableSize = (int)(TableStreamWriter.BaseStream.Position - fib.SectionTableOffset);
			fib.ParagraphHeightsOffset = (int)(TableStreamWriter.BaseStream.Position);
			fib.ParagraphHeightsSize = (int)(fib.ParagraphHeightsOffset - TableStreamWriter.BaseStream.Position);
		}
		void ExportDocumentProperties(FileInformationBlock fib) {
			DocDocumentProperties dop = DocDocumentProperties.CreateDefault();
			Section section = DocumentModel.Sections[new SectionIndex(0)];
			dop.GutterPosition = section.Margins.GutterAlignment == SectionGutterAlignment.Top ? GutterPosition.Top : GutterPosition.Side;
			DocumentProperties documentProperties = DocumentModel.DocumentProperties;
			dop.DifferentOddAndEvenPages = documentProperties.DifferentOddAndEvenPages;
			dop.DisplayBackgroundShape = documentProperties.DisplayBackgroundShape;
			dop.DefaultTabWidth = (short)documentProperties.DefaultTabWidth;
			SetNotesProperties(dop, section);
			SetDocumentProtection(dop);
			fib.DocumentPropertiesOffset = (int)TableStreamWriter.BaseStream.Position;
			dop.Write(TableStreamWriter);
			fib.DocumentPropertiesSize = (int)(TableStreamWriter.BaseStream.Position - fib.DocumentPropertiesOffset);
		}
		void SetNotesProperties(DocDocumentProperties dop, Section section) {
			dop.FootNoteInitialNumber = section.FootNote.StartingNumber;
			dop.FootNoteNumberingRestartType = section.FootNote.NumberingRestartType;
			dop.FootNotePosition = section.FootNote.Position;
			dop.EndnoteInitialNumber = section.EndNote.StartingNumber;
			dop.EndNoteNumberingRestartType = section.EndNote.NumberingRestartType;
			dop.EndNotePosition = section.EndNote.Position;
		}
		void SetDocumentProtection(DocDocumentProperties dop) {
			DocumentProtectionProperties protectionProperties = DocumentModel.ProtectionProperties;
			dop.EnforceProtection = protectionProperties.EnforceProtection;
			dop.ProtectionType = protectionProperties.ProtectionType;
			if (protectionProperties.Word2003PasswordHash != null) {
#if DEBUGTEST
				Debug.Assert(protectionProperties.Word2003PasswordHash.Length == 4);
#endif
				dop.PasswordHash = protectionProperties.Word2003PasswordHash;
				Array.Reverse(dop.PasswordHash);
			}
		}
		void ExportDocumentVariables(FileInformationBlock fib) {
			fib.DocumentVariablesOffset = (int)TableStreamWriter.BaseStream.Position;
			DocumentVariables docVars = DocumentVariables.FromVariablesCollection(DocumentModel.Variables, DocumentModel.DocumentProperties);
			docVars.Write(TableStreamWriter);
			fib.DocumentVariablesSize = (int)(TableStreamWriter.BaseStream.Position - fib.DocumentVariablesOffset);
		}
		void ExportFonts(FileInformationBlock fib) {
			fib.FontTableOffset = (int)TableStreamWriter.BaseStream.Position;
			short fontsCount = (short)DocDataWriter.StylesExporter.FontsCount;
			TableStreamWriter.Write(fontsCount);
			TableStreamWriter.Write((short)0); 
			for (int fontNameIndex = 0; fontNameIndex < fontsCount; fontNameIndex++) {
				DocFontFamilyName font = new DocFontFamilyName();
				string fontName = DocDataWriter.StylesExporter.GetFontNameByIndex(fontNameIndex);
				font.Charset = (byte)DocumentModel.FontCache.GetCharsetByFontName(fontName);
				font.FontName = fontName;
				font.Write(TableStreamWriter);
			}
			fib.FontTableSize = (int)(TableStreamWriter.BaseStream.Position - fib.FontTableOffset);
		}
		void ExportComplexFileInformation(FileInformationBlock fib) {
			DocPieceTable pieceTable = DocPieceTable.CreateDefault(textStartOffset, CurrentCharacterPosition);
			ComplexFileInformation clxInfo = new ComplexFileInformation();
			clxInfo.PieceTable = pieceTable.ToByteArray();
			fib.ComplexFileInformationOffset = (int)TableStreamWriter.BaseStream.Position;
			clxInfo.Write(TableStreamWriter);
			fib.ComplexFileInformationSize = (int)(TableStreamWriter.BaseStream.Position - fib.ComplexFileInformationOffset);
		}
		void ExportFormattingBinTables(FileInformationBlock fib) {
			fib.CharacterTableOffset = (int)TableStreamWriter.BaseStream.Position;
			DocDataWriter.WriteCharactersBinTable(TableStreamWriter);
			fib.CharacterTableSize = (int)(TableStreamWriter.BaseStream.Position - fib.CharacterTableOffset);
			fib.ParagraphTableOffset = (int)TableStreamWriter.BaseStream.Position;
			DocDataWriter.WriteParagraphsBinTable(TableStreamWriter);
			fib.ParagraphTableSize = (int)(TableStreamWriter.BaseStream.Position - fib.ParagraphTableOffset);
		}
		void ExportHeadersFootersPositionsTable(FileInformationBlock fib) {
			if (HeadersFootersPositions == null)
				return;
			fib.HeadersFootersPositionsOffset = (int)TableStreamWriter.BaseStream.Position;
			HeadersFootersPositions.Write(TableStreamWriter);
			fib.HeadersFootersPositionsSize = (int)(TableStreamWriter.BaseStream.Position - fib.HeadersFootersPositionsOffset);
		}
		void ExportListFormatInformation(FileInformationBlock fib) {
			fib.ListFormatInformationOffset = (int)TableStreamWriter.BaseStream.Position;
			DocDataWriter.ListsExporter.WriteListFormatInformation(TableStreamWriter);
			fib.ListFormatInformationSize = (int)(TableStreamWriter.BaseStream.Position - fib.ListFormatInformationOffset);
		}
		void ExportDocumentFormatRecords(FileInformationBlock fib) {
			if (DocDataWriter.ListsExporter.ListStyles.Count == 0)
				return;
			fib.DocumentFileRecordsOffset = (int)TableStreamWriter.BaseStream.Position;
			DocumentFileRecords docFileRecords = new DocumentFileRecords();
			docFileRecords.ListStyles.AddRange(DocDataWriter.ListsExporter.ListStyles);
			docFileRecords.Write(TableStreamWriter);
			fib.DocumentFileRecordsSize = (int)(TableStreamWriter.BaseStream.Position - fib.DocumentFileRecordsOffset);
		}
		void ExportListOverrideFormatInformation(FileInformationBlock fib) {
			fib.ListFormatOverrideInformationOffset = (int)TableStreamWriter.BaseStream.Position;
			DocDataWriter.ListsExporter.WriteListOverrideFormatInformation(TableStreamWriter);
			fib.ListFormatOverrideInformationSize = (int)(TableStreamWriter.BaseStream.Position - fib.ListFormatOverrideInformationOffset);
		}
		void ExportFieldTables(FileInformationBlock fib) {
			DocDataWriter.ExportFieldTables(fib, TableStreamWriter);
		}
		void ExportBookmarks(FileInformationBlock fib) {
			DocDataWriter.BookmarkIterator.Write(fib, TableStreamWriter);
		}
		void ExportRangeEditPermissions(FileInformationBlock fib) {
			DocDataWriter.PermissionIterator.Write(fib, TableStreamWriter);
		}
		void ExportFloatingObjectsInfo(FileInformationBlock fib) {
			DocDataWriter.FloatingObjectsExporter.ExportFloatingObjectsInfo(fib, TableStreamWriter, MainStreamWriter);
		}
		void ExportTextSettings(FileInformationBlock fib) {
			fib.FirstCharacterFileOffset = FileInformationBlock.FIBSize;
			fib.LastCharacterFileOffset = FileInformationBlock.FIBSize + CurrentCharacterPosition * 2;
			fib.LastByteFileOffset = (int)MainStreamWriter.BaseStream.Length;
		}
		void ExportRmdThreading(FileInformationBlock fib) {
			fib.RmdThreadingOffset = (int)TableStreamWriter.BaseStream.Position;
			new RmdThreading().Write(TableStreamWriter);
			fib.RmdThreadingSize = (int)(TableStreamWriter.BaseStream.Position - fib.RmdThreadingOffset);
		}
		void SetTextStreamBorders(FileInformationBlock fib) {
			fib.MainDocumentLength = TextBorders.MainDocumentLength;
			fib.FootNotesLength = TextBorders.FootNotesTextLength;
			fib.HeadersFootersLength = TextBorders.HeadersFootersTextLength;
			fib.CommentsLength = TextBorders.CommentsTextLength;
			fib.EndNotesLength = TextBorders.EndNotesTextLength;
			fib.MainDocumentTextBoxesLength = TextBorders.TextBoxesTextLength;
			fib.HeaderTextBoxesLength = TextBorders.HeaderTextBoxesTextLength;
		}
		protected internal override void ExportSection(Section section) {
			DocDataWriter.WriteSection(CurrentCharacterPosition, section);
			ExportParagraphs(section.FirstParagraphIndex, section.LastParagraphIndex);
		}
		protected internal override void ExportSectionRun(SectionRun run) {
			string section = new string(TextCodes.SectionMark, 1);
			ExportTextRunBaseCore(run, section);
		}
		protected internal override void ExportParagraphs(ParagraphIndex from, ParagraphIndex to) {
			if (ActivePieceTable.IsComment) {
				from = ExportFirstCommentParagraph(PieceTable.Paragraphs[from]);
				from++;				
			}
			base.ExportParagraphs(from, to);
		}
		ParagraphIndex ExportFirstCommentParagraph(Paragraph paragraph) {
			BeginParagraphExport(paragraph);
			ExportDefaultCommentSeparators();
			return base.ExportParagraph(paragraph);
		}		
		protected internal override ParagraphIndex ExportParagraph(Paragraph paragraph) {
			BeginParagraphExport(paragraph);						
			return base.ExportParagraph(paragraph);
		}
		void BeginParagraphExport(Paragraph paragraph) {
			int paragraphStyleIndex = DocDataWriter.StylesExporter.GetParagraphStyleIndex(paragraph.ParagraphStyleIndex);
			if (paragraph.GetCell() != null)
				DocDataWriter.WriteInTableParagraph(CurrentCharacterPosition, paragraphStyleIndex, paragraph);
			else
				DocDataWriter.WriteParagraph(CurrentCharacterPosition, paragraphStyleIndex, paragraph);
			NumberingListIndex listIndex = paragraph.NumberingListIndex;
			if (listIndex == NumberingListIndex.NoNumberingList || listIndex == NumberingListIndex.ListIndexNotSetted)
				return;
			int index = ((IConvertToInt<NumberingListIndex>)listIndex).ToInt();
			DocListOverrideLevelInformation levelInfo = DocDataWriter.ListsExporter.ListOverrideInfo.FormatOverrideData[index];
			if (levelInfo.CharacterPosition != DocListsExporter.EmptyCharacterPosition) return;
			levelInfo.CharacterPosition = CurrentCharacterPosition;
		}
		protected internal virtual void ExportEmptyParagraph() {
			ExportEmptyParagraphCore();
			ExportEmptyParagraphRun();
		}
		void ExportEmptyParagraphWithLastParagraphProperties() {
			Paragraph paragraph = DocumentModel.MainPieceTable.Paragraphs.Last;
			int paragraphStyleIndex = DocDataWriter.StylesExporter.GetParagraphStyleIndex(paragraph.ParagraphStyleIndex);
			DocDataWriter.WriteParagraph(CurrentCharacterPosition, paragraphStyleIndex, paragraph);
			ExportTextRunBaseCore(DocumentModel.MainPieceTable.Runs.Last, new string(TextCodes.ParagraphMark, 1));
		}
		void ExportGuardParagraphMark() {
			ExportEmptyParagraphCore();
			MainStreamWriter.Write(Encoding.Unicode.GetBytes(new string(TextCodes.ParagraphMark, 1)));
			CurrentCharacterPosition++;
		}
		void ExportEmptyParagraphCore() {
			int paragraphStyleIndex = DocDataWriter.StylesExporter.GetParagraphStyleIndex(DocumentModel.ParagraphStyles.DefaultItemIndex);
			DocDataWriter.WriteParagraph(CurrentCharacterPosition, paragraphStyleIndex);
		}
		void ExportEmptyParagraphRun() {
			string paragraph = new string(TextCodes.ParagraphMark, 1);
			ExportTextRunBaseCore(paragraph);
		}
		protected internal override void ExportParagraphRun(ParagraphRun run) {
			Paragraph paragraph = run.Paragraph;
			TableCell cell = paragraph.GetCell();
			bool isLastParagraphInTableCell = cell != null && cell.EndParagraphIndex == paragraph.Index;
			string paragraphTerminator = (isLastParagraphInTableCell) ? DocDataWriter.TablesExporter.GetTableUnitMark()
				: new string(TextCodes.ParagraphMark, 1);
			ExportTextRunBaseCore(run, paragraphTerminator);
		}
		protected internal override void ExportTextRun(TextRun run) {
			string text = run.GetPlainText(ActivePieceTable.TextBuffer);
			ExportTextRunBaseCore(run, text);
		}
		protected internal override void ExportInlinePictureRun(InlinePictureRun run) {
			string inlinePicture = new string(TextCodes.InlinePicture, 1);
			MainStreamWriter.Write(Encoding.Unicode.GetBytes(inlinePicture));
			int characterStyleIndex = DocDataWriter.StylesExporter.GetCharacterStyleIndex(run.CharacterStyleIndex);
			DocDataWriter.WriteInlinePictureRun(CurrentCharacterPosition, characterStyleIndex, (int)DataStreamWriter.BaseStream.Position, run.CharacterProperties);
			ImagesIterator.WritePictureDescriptor(DataStreamWriter, run);
			CurrentCharacterPosition++;
		}
		protected internal override void ExportFloatingObjectAnchorRun(FloatingObjectAnchorRun run) {
			DocDataWriter.FloatingObjectsExporter.RegisterFloatingObject(run, GetRelativeCharacterPosition());
			string floatingObjectAnchor = new string(TextCodes.FloatingObjectAnchor, 1);
			ExportTextRunBaseCore(run, floatingObjectAnchor, true);
		}
		protected internal override void ExportBookmarkStart(Bookmark bookmark) {
			base.ExportBookmarkStart(bookmark);
			DocDataWriter.BookmarkIterator.AddBookmarkStart(bookmark.Name, CurrentCharacterPosition);
		}
		protected internal override void ExportBookmarkEnd(Bookmark bookmark) {
			base.ExportBookmarkEnd(bookmark);
			DocDataWriter.BookmarkIterator.AddBookmarkEnd(bookmark.Name, CurrentCharacterPosition);
		}
		protected internal override void ExportRangePermissionStart(RangePermission rangePermission) {
			base.ExportRangePermissionStart(rangePermission);
			DocDataWriter.PermissionIterator.AddPermissionStart(rangePermission, CurrentCharacterPosition);
		}
		protected internal override void ExportRangePermissionEnd(RangePermission rangePermission) {
			base.ExportRangePermissionEnd(rangePermission);
			DocDataWriter.PermissionIterator.AddPermissionEnd(rangePermission, CurrentCharacterPosition);
		}
		protected internal override void ExportCommentStart(Comment comment) {
			base.ExportCommentStart(comment);
			DocDataWriter.CommentsExporter.AddCommentStart(Convert.ToString(comment.Index), CurrentCharacterPosition);
		}
		protected internal override void ExportCommentEnd(Comment comment) {
			base.ExportCommentEnd(comment);
			DocDataWriter.CommentsExporter.AddCommentEnd(comment, CurrentCharacterPosition);
			ExportDefaultCommentSeparators();
		}
		protected internal override void ExportFieldCodeStartRun(FieldCodeStartRun run) {
			base.ExportFieldCodeStartRun(run);
			DocDataWriter.FieldTable.AddEntry(GetRelativeCharacterPosition(), FieldsIterator.CreateFieldBeginDescriptor(ActivePieceTable));
			string fieldCodeStart = new string(TextCodes.FieldBegin, 1);
			ExportTextRunBaseCore(run, fieldCodeStart, true);
		}
		protected internal override void ExportFieldCodeEndRun(FieldCodeEndRun run) {
			base.ExportFieldCodeEndRun(run);
			DocFieldSeparatorDescriptor fieldSeparatorDescriptor = new DocFieldSeparatorDescriptor();
			DocDataWriter.FieldTable.AddEntry(GetRelativeCharacterPosition(), fieldSeparatorDescriptor);
			string fieldCodeEnd = new string(TextCodes.FieldSeparator, 1);
			ExportTextRunBaseCore(run, fieldCodeEnd, true);
		}
		protected internal override void ExportFieldResultEndRun(FieldResultEndRun run) {
			base.ExportFieldResultEndRun(run);
			DocFieldEndDescriptor fieldEndDescriptor = FieldsIterator.CreateFieldEndDescriptor(FieldLevel);
			DocDataWriter.FieldTable.AddEntry(GetRelativeCharacterPosition(), fieldEndDescriptor);
			string fieldResultEnd = new string(TextCodes.FieldEnd, 1);
			ExportTextRunBaseCore(run, fieldResultEnd, true);
		}
		protected internal override void ExportFootNoteRun(FootNoteRun run) {
			if (!DocumentModel.DocumentCapabilities.FootNotesAllowed)
				return;
			if (ActivePieceTable.IsMain) {
				base.ExportFootNoteRun(run);
				DocDataWriter.NotesExporter.AddFootNoteReferenceEntry(GetRelativeCharacterPosition(), true);
			}
			string footNoteReference = new string(TextCodes.AutoNumberedFootNoteReference, 1);
			ExportTextRunBaseCore(run, footNoteReference, true);
		}
		protected internal override void ExportEndNoteRun(EndNoteRun run) {
			if (!DocumentModel.DocumentCapabilities.EndNotesAllowed)
				return;
			if (ActivePieceTable.IsMain) {
				base.ExportEndNoteRun(run);
				DocDataWriter.NotesExporter.AddEndNoteReferenceEntry(GetRelativeCharacterPosition(), true);
			}
			string endNoteReference = new string(TextCodes.AutoNumberedFootNoteReference, 1);
			ExportTextRunBaseCore(run, endNoteReference, true);
		}
		protected internal override void ExportRow(TableRow row, DevExpress.XtraRichEdit.Internal.TableInfo tableInfo) {
			base.ExportRow(row, tableInfo);
			ExportRowMark(row);
		}
		protected internal override ParagraphIndex ExportTable(DevExpress.XtraRichEdit.Internal.TableInfo tableInfo) {
			DocDataWriter.TablesExporter.AdvanceNext(tableInfo);
			ParagraphIndex index = base.ExportTable(tableInfo);
			DocDataWriter.TablesExporter.FinishTable();
			return index;
		}
		void ExportRowMark(TableRow row) {
			string rowMark = DocDataWriter.TablesExporter.GetTableUnitMark();
			int paragraphStyleIndex = DocDataWriter.StylesExporter.GetParagraphStyleIndex(DocumentModel.ParagraphStyles.DefaultItemIndex);
			DocDataWriter.WriteParagraph(CurrentCharacterPosition, paragraphStyleIndex, row);
			ExportTextRunBaseCore(rowMark);
		}
		void ExportTextRunBaseCore(string text) {
			ExportTextRunBaseCore(null, text, false);
		}
		void ExportTextRunBaseCore(TextRunBase run, string text) {
			ExportTextRunBaseCore(run, text, false);
		}
		void ExportTextRunBaseCore(TextRunBase run, string text, bool special) {
			MainStreamWriter.Write(Encoding.Unicode.GetBytes(text));
			DocDataWriter.WriteTextRun(CurrentCharacterPosition, run, special);
			CurrentCharacterPosition += text.Length;
		}
		protected void UpdateState(DocContentState state) {
			CurrentDocumentPartStart = CurrentCharacterPosition;
			DocDataWriter.SetState(state);
		}
		void SetActivePieceTable(PieceTable pieceTable) {
			this.activePieceTable = pieceTable;
			FieldsIterator.UpdateFieldsInfo(pieceTable);
		}
		protected int GetRelativeCharacterPosition() {
			return CurrentCharacterPosition - CurrentDocumentPartStart;
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (this.docDataWriter != null) {
					this.docDataWriter.Dispose();
					this.docDataWriter = null;
				}
				IDisposable mainStreamWriter = MainStreamWriter as IDisposable;
				if (mainStreamWriter != null) {
					mainStreamWriter.Dispose();
					mainStreamWriter = null;
				}
				IDisposable tableStreamWriter = TableStreamWriter as IDisposable;
				if (tableStreamWriter != null) {
					tableStreamWriter.Dispose();
					tableStreamWriter = null;
				}
				IDisposable dataWriter = DataStreamWriter;
				if (dataWriter != null) {
					dataWriter.Dispose();
					dataWriter = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
	}
	#endregion
	#region ExportImagesIterator
	public class ExportImagesIterator {
		#region Fields
		int currentPictureIndex = 1; 
		DocContentState state;
		#endregion
		#region Properties
		protected int CurrentPictureIndex { get { return currentPictureIndex; } set { currentPictureIndex = value; } }
		public bool HasPictures { get { return currentPictureIndex > 1; } }
		#endregion
		public void WritePictureDescriptor(BinaryWriter writer, InlinePictureRun run) {
			PictureDescriptor pictureDescriptor = new PictureDescriptor(run, CalculateCurrentShapeId(), CurrentPictureIndex);
			pictureDescriptor.Write(writer);
			CurrentPictureIndex++;
		}
		public void SetState(DocContentState state) {
			if (this.state == state)
				return;
			this.state = state;
			CurrentPictureIndex = 1;
		}
		int CalculateCurrentShapeId() {
			if (this.state == DocContentState.MainDocument)
				return OfficeArtConstants.DefaultMainDocumentShapeIdentifier + CurrentPictureIndex;
			if (this.state == DocContentState.HeadersFootersStory)
				return OfficeArtConstants.DefaultHeaderShapeIdentifier + CurrentPictureIndex;
			Exceptions.ThrowInternalException();
			return 0;
		}
	}
	#endregion
	#region ExportFieldsIterator
	public class ExportFieldsIterator {
		#region Fields
		Field[] currentFields;
		int currentFieldIndex;
		Stack<Field> fields;
		#endregion
		public ExportFieldsIterator() {
			fields = new Stack<Field>();
		}
		#region Properties
		protected Field[] CurrentFields { get { return currentFields; } set { currentFields = value; } }
		protected int CurrentFieldIndex { get { return currentFieldIndex; } set { currentFieldIndex = value; } }
		public Field CurrentField { get { return currentFields[currentFieldIndex]; } }
		#endregion
		public void UpdateFieldsInfo(PieceTable pieceTable) {
			CurrentFieldIndex = 0;
			CurrentFields = new Field[pieceTable.Fields.Count];
			pieceTable.Fields.CopyTo(currentFields, 0);
			Array.Sort(currentFields, new FieldStartComparer());
		}
		public DocFieldBeginDescriptor CreateFieldBeginDescriptor(PieceTable pieceTable) {
			if (CurrentFieldIndex >= CurrentFields.Length)
				return DocFieldBeginDescriptor.Empty;
			Field field = CurrentFields[CurrentFieldIndex];
			DocumentFieldIterator iterator = new DocumentFieldIterator(pieceTable, field);
			DocumentModel model = pieceTable.DocumentModel;
			FieldScanner scanner = new FieldScanner(iterator, model.MaxFieldSwitchLength, model.EnableFieldNames, pieceTable.SupportFieldCommonStringFormat);
			Token token = scanner.Scan();
			CurrentFieldIndex++;
			fields.Push(field);
			return new DocFieldBeginDescriptor(token);
		}
		public DocFieldEndDescriptor CreateFieldEndDescriptor(int fieldLevel) {
			Field field = fields.Pop();
			DocFieldEndDescriptor fieldEndDescriptor = new DocFieldEndDescriptor();
			if (fieldLevel > 0)
				fieldEndDescriptor.Properties |= FieldProperties.Nested;
			if (field.Locked)
				fieldEndDescriptor.Properties |= FieldProperties.Locked;
			return fieldEndDescriptor;
		}
	}
	#endregion
}
