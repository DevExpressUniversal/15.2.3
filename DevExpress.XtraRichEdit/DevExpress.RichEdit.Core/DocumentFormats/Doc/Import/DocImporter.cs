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
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraRichEdit.Import.Doc {
	public delegate void InsertDocObjectDelegate(IDocObject docObject, PieceTable pieceTable, InputPosition inputPosition);
	public class DocImporter : DocumentModelImporter, IDisposable, IDocImporter {
		#region Fields
		readonly DocumentImporterOptions options;
		readonly Stack<DocImportPieceTableInfo> importInfoStack;
		DocContentBuilder contentBuilder;
		SectionIndex currentSectionIndex;
		DocStylesImportHelper stylesImportHelper;
		DocListsImportHelper listsImportHelper;
		Dictionary<TextRunBase, CharacterInfo> delayedFormatting;
		Dictionary<DocObjectType, InsertDocObjectDelegate> docObjectDispatcher;
		Dictionary<TableRow, List<DocTableCellHorizontalMerging>> cellsHorizontalMerging;
		#endregion
		public DocImporter(DocumentModel documentModel, DocDocumentImporterOptions options)
			: base(documentModel) {
			Guard.ArgumentNotNull(options, "options");
			this.options = options;
			this.importInfoStack = new Stack<DocImportPieceTableInfo>();
			CreateDocObjectDispatcher();
		}
		#region Properties
		public new DocumentModel DocumentModel { get { return (DocumentModel)base.DocumentModel; } }
		public DocumentModelUnitConverter UnitConverter { get { return DocumentModel.UnitConverter; } }
		protected internal DocDocumentImporterOptions Options { get { return (DocDocumentImporterOptions)options; } }
		protected internal DocContentBuilder ContentBuilder { get { return contentBuilder; } }
		protected Stack<DocImportPieceTableInfo> ImportInfoStack { get { return importInfoStack; } }
		protected DocImportPieceTableInfo ActiveImportInfo { get { return ImportInfoStack.Peek(); } }
		protected DocStylesImportHelper StylesImportHelper { get { return stylesImportHelper; } }
		protected DocListsImportHelper ListsImportHelper { get { return listsImportHelper; } }
		protected Dictionary<DocObjectType, InsertDocObjectDelegate> DocObjectDispatcher { get { return docObjectDispatcher; } }
		public Dictionary<TableRow, List<DocTableCellHorizontalMerging>> CellsHorizontalMerging { get { return cellsHorizontalMerging; } }
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (this.contentBuilder != null) {
					this.contentBuilder.Dispose();
					this.contentBuilder = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~DocImporter() {
			Dispose(false);
		}
		#endregion
		void CreateDocObjectDispatcher() {
			this.docObjectDispatcher = new Dictionary<DocObjectType, InsertDocObjectDelegate>();
			this.docObjectDispatcher.Add(DocObjectType.InlineImage, InsertImage);
			this.docObjectDispatcher.Add(DocObjectType.PictureFloatingObject, InsertPictureFloatingObject);
			this.docObjectDispatcher.Add(DocObjectType.TextBoxFloatingObject, InsertTextBoxFloatingObject);
			this.docObjectDispatcher.Add(DocObjectType.TextRun, InsertText);
			this.docObjectDispatcher.Add(DocObjectType.FieldBegin, InsertFieldBegin);
			this.docObjectDispatcher.Add(DocObjectType.FieldSeparator, InsertFieldSeparator);
			this.docObjectDispatcher.Add(DocObjectType.FieldEnd, InsertFieldEnd);
			this.docObjectDispatcher.Add(DocObjectType.HyperlinkFieldData, InsertHyperlinkFieldData);
			this.docObjectDispatcher.Add(DocObjectType.AutoNumberedFootnoteReference, InsertFootNoteReference);
			this.docObjectDispatcher.Add(DocObjectType.AnnotationReference, InsertAnnotationReference);
			this.docObjectDispatcher.Add(DocObjectType.EndnoteReference, InsertEndnoteReference);
			this.docObjectDispatcher.Add(DocObjectType.NoteNumber, InsertNoteNumber);
			this.docObjectDispatcher.Add(DocObjectType.TableCell, InsertTableCell);
			this.docObjectDispatcher.Add(DocObjectType.TableRow, InsertTableRow);
			this.docObjectDispatcher.Add(DocObjectType.Paragraph, InsertParagraph);
			this.docObjectDispatcher.Add(DocObjectType.Section, InsertSection);
			this.docObjectDispatcher.Add(DocObjectType.ExpectedFieldBegin, InsertExpectedObjectAsText);
			this.docObjectDispatcher.Add(DocObjectType.ExpectedFieldSeparator, InsertExpectedObjectAsText);
			this.docObjectDispatcher.Add(DocObjectType.ExpectedFieldEnd, InsertExpectedObjectAsText);
		}
		public void Import(Stream stream) {
			DocumentModel.BeginSetContent();
			try {
				cellsHorizontalMerging = new Dictionary<TableRow, List<DocTableCellHorizontalMerging>>();
				this.contentBuilder = new DocContentBuilder(DocumentModel, (DocDocumentImporterOptions)this.options);
				ContentBuilder.BuildDocumentContent(stream, DocumentModel);
				ImportDocument();
			}
			catch {
				cellsHorizontalMerging = new Dictionary<TableRow, List<DocTableCellHorizontalMerging>>();
				DocumentModel.ClearDocumentCore(true, true);
				throw;
			}
			finally {
				DocumentModel.EndSetContent(DocumentModelChangeType.LoadNewDocument, true, false, options.UpdateField.GetNativeOptions(), ApplyHorizontalMerging);
			}
		}
		void ApplyHorizontalMerging() {
			HashSet<Table> tables = new HashSet<Table>();
			foreach (var keyValuePair in cellsHorizontalMerging) {
				List<DocTableCellHorizontalMerging> cellsMerging = keyValuePair.Value;
				cellsMerging.Sort((a,b) => b.FirstCellIndex - a.FirstCellIndex);
				foreach (var merging in cellsMerging)
					MergeCellsHorizontally(keyValuePair.Key, merging);
				tables.Add(keyValuePair.Key.Table);
			}
			foreach (Table table in tables)
				table.NormalizeCellColumnSpans();
		}
		void MergeCellsHorizontally(TableRow row, DocTableCellHorizontalMerging merging) {
			PieceTable pieceTable = row.PieceTable;
			int span = merging.Count;
			int firstCellIndex = merging.FirstCellIndex;
			int summaryWidth = 0;
			for (int i = span - 2; i >= 0; i--) {
				int cellIndex = i + firstCellIndex + 1;
				TableCell cell = row.Cells[cellIndex]; 
				if(cell.PreferredWidth.Type == WidthUnitType.ModelUnits)
					summaryWidth += cell.PreferredWidth.Value;
				pieceTable.TableCellsManager.RemoveTableCell(cell);
				pieceTable.DeleteContent(pieceTable.Paragraphs[cell.StartParagraphIndex].LogPosition, 1, false);
				row.Cells.DeleteInternal(cellIndex);
			}
			TableCell firstCell = row.Cells[firstCellIndex];
			firstCell.ColumnSpan += span - 1;
			if (firstCell.Properties.PreferredWidth.Type == WidthUnitType.ModelUnits)
				firstCell.Properties.PreferredWidth.Value += summaryWidth;
		}
		protected internal virtual void ImportDocument() {
			this.delayedFormatting = new Dictionary<TextRunBase, CharacterInfo>();
			this.stylesImportHelper = new DocStylesImportHelper(ContentBuilder, DocumentModel);
			this.listsImportHelper = new DocListsImportHelper(ContentBuilder, DocumentModel);
			ImportContent();
			DocumentModel.NormalizeZOrder();
		}
		protected internal virtual void ImportContent() {
			Initialize();
			CreateMainText();
			SetHeadersFooters();
			SetDocumentProperties();
			SetDocumentVariables();
		}
		void Initialize() {
			ListsImportHelper.InitializeAbstractLists();
			ListsImportHelper.InitializeLists();
			StylesImportHelper.SetDocunentDefaults();
			StylesImportHelper.InitializeStyles();
			ListsImportHelper.LinkNumberingListStyles(StylesImportHelper);
		}
		void CreateMainText() {
			DocObjectCollection docObjects = ContentBuilder.Iterator.MainTextDocObjects;
			this.currentSectionIndex = new SectionIndex(0);
			InsertEmbeddedContent(DocumentModel.MainPieceTable, docObjects);
			ApplySectionProperties(docObjects[docObjects.Count - 1].PropertyContainer);
		}
		void SetDocumentProperties() {
			SetPageBackground();
			SetDocumentProtection();
		}
		void SetPageBackground() {
			DocumentModel.DocumentProperties.DisplayBackgroundShape = ContentBuilder.DocumentProperties.DisplayBackgroundShape;
		}
		void SetDocumentProtection() {
			DocumentProtectionProperties properties = DocumentModel.ProtectionProperties;
			DocDocumentProperties documentProperties = ContentBuilder.DocumentProperties;
			properties.EnforceProtection = documentProperties.EnforceProtection;
			properties.ProtectionType = documentProperties.ProtectionType;
			properties.Word2003PasswordHash = documentProperties.PasswordHash;
		}
		void SetHeadersFooters() {
			DocumentModel.DocumentProperties.DifferentOddAndEvenPages = ContentBuilder.DocumentProperties.DifferentOddAndEvenPages;
			for (int i = 0; i < DocumentModel.Sections.Count; i++)
				SetHeadersFootersCore(i);
		}
		void SetHeadersFootersCore(int sectionIndex) {
			SetEvenPageHeader(sectionIndex);
			SetOddPageHeader(sectionIndex);
			SetEvenPageFooter(sectionIndex);
			SetOddPageFooter(sectionIndex);
			SetFirstPageHeader(sectionIndex);
			SetFirstPageFooter(sectionIndex);
		}
		void SetEvenPageHeader(int sectionIndex) {
			DocObjectCollection docObjects = ContentBuilder.Iterator.HeadersFooters.GetEvenPageHeaderObjects(sectionIndex);
			Section currentSection = DocumentModel.Sections[new SectionIndex(sectionIndex)];
			if (docObjects.Count == 0) {
				currentSection.Headers.LinkToPrevious(HeaderFooterType.Even);
				return;
			}
			currentSection.Headers.Create(HeaderFooterType.Even);
			SectionHeader evenPageHeader = currentSection.InnerEvenPageHeader;
			PieceTable evenPageHeaderPieceTable = evenPageHeader.PieceTable;
			InsertEmbeddedContent(evenPageHeaderPieceTable, docObjects);
		}
		void SetEvenPageFooter(int sectionIndex) {
			DocObjectCollection docObjects = ContentBuilder.Iterator.HeadersFooters.GetEvenPageFooterObjects(sectionIndex);
			Section currentSection = DocumentModel.Sections[new SectionIndex(sectionIndex)];
			if (docObjects.Count == 0) {
				currentSection.Footers.LinkToPrevious(HeaderFooterType.Even);
				return;
			}
			currentSection.Footers.Create(HeaderFooterType.Even);
			SectionFooter evenPageFooter = currentSection.InnerEvenPageFooter;
			PieceTable evenPageFooterPieceTable = evenPageFooter.PieceTable;
			InsertEmbeddedContent(evenPageFooterPieceTable, docObjects);
		}
		void SetOddPageHeader(int sectionIndex) {
			DocObjectCollection docObjects = ContentBuilder.Iterator.HeadersFooters.GetOddPageHeaderObjects(sectionIndex);
			Section currentSection = DocumentModel.Sections[new SectionIndex(sectionIndex)];
			int count = docObjects.Count;
			if (count == 0) {
				currentSection.Headers.LinkToPrevious(HeaderFooterType.Odd);
				return;
			}
			currentSection.Headers.Create(HeaderFooterType.Odd);
			SectionHeader oddPageHeader = currentSection.InnerOddPageHeader;
			PieceTable oddPageHeaderPieceTable = oddPageHeader.PieceTable;
			InsertEmbeddedContent(oddPageHeaderPieceTable, docObjects);
		}
		void SetOddPageFooter(int sectionIndex) {
			DocObjectCollection docObjects = ContentBuilder.Iterator.HeadersFooters.GetOddPageFooterObjects(sectionIndex);
			Section currentSection = DocumentModel.Sections[new SectionIndex(sectionIndex)];
			int count = docObjects.Count;
			if (count == 0) {
				currentSection.Footers.LinkToPrevious(HeaderFooterType.Odd);
				return;
			}
			currentSection.Footers.Create(HeaderFooterType.Odd);
			SectionFooter oddPageFooter = currentSection.InnerOddPageFooter;
			PieceTable oddPageFooterPieceTable = oddPageFooter.PieceTable;
			InsertEmbeddedContent(oddPageFooterPieceTable, docObjects);
		}
		void SetFirstPageHeader(int sectionIndex) {
			DocObjectCollection docObjects = ContentBuilder.Iterator.HeadersFooters.GetFirstPageHeaderObjects(sectionIndex);
			Section currentSection = DocumentModel.Sections[new SectionIndex(sectionIndex)];
			int count = docObjects.Count;
			if (count == 0) {
				currentSection.Headers.LinkToPrevious(HeaderFooterType.First);
				return;
			}
			currentSection.Headers.Create(HeaderFooterType.First);
			SectionHeader firstPageHeader = currentSection.InnerFirstPageHeader;
			PieceTable firstPageHeaderPieceTable = firstPageHeader.PieceTable;
			InsertEmbeddedContent(firstPageHeaderPieceTable, docObjects);
		}
		void SetFirstPageFooter(int sectionIndex) {
			DocObjectCollection docObjects = ContentBuilder.Iterator.HeadersFooters.GetFirstPageFooterObjects(sectionIndex);
			Section currentSection = DocumentModel.Sections[new SectionIndex(sectionIndex)];
			int count = docObjects.Count;
			if (count == 0) {
				currentSection.Footers.LinkToPrevious(HeaderFooterType.First);
				return;
			}
			currentSection.Footers.Create(HeaderFooterType.First);
			SectionFooter firstPageFooter = currentSection.InnerFirstPageFooter;
			PieceTable firstPageFooterPieceTable = firstPageFooter.PieceTable;
			InsertEmbeddedContent(firstPageFooterPieceTable, docObjects);
		}
		void SetDocumentVariables() {
			ContentBuilder.DocumentVariables.SetVariables(DocumentModel.Variables, DocumentModel.DocumentProperties);
		}
		void ProcessDocObject(IDocObject docObject, PieceTable pieceTable, InputPosition inputPosition) {
			ContentBuilder.Iterator.AdvancePosition(inputPosition.LogPosition, docObject.Position, docObject.Length);
			InsertDocObject(docObject, pieceTable, inputPosition);
		}
		void Finish(PieceTable targetPieceTable) {
			bool hideLastParagraph = ActiveImportInfo.TablesImporter.Finish();
			if (ActiveImportInfo.FieldsImporter.ImportFieldStack.Count != 0)
				ThrowInvalidDocFile();
			ContentBuilder.Iterator.InsertBookmarks(targetPieceTable);
			targetPieceTable.FixLastParagraph();
			targetPieceTable.FixTables();
			if (!targetPieceTable.IsMain && hideLastParagraph)
				FixAbsentLastParagraph(targetPieceTable);
			ImportInfoStack.Pop();
		}
		void FixAbsentLastParagraph(PieceTable targetPieceTable) {
			TextRunBase lastRun = targetPieceTable.Runs[new RunIndex(targetPieceTable.Runs.Count - 1)];
			lastRun.BeginUpdate();
			try {
				lastRun.DoubleFontSize = 2;
				lastRun.Hidden = true;
			}
			finally {
				lastRun.EndUpdate();
			}
		}
		void InsertDocObject(IDocObject docObject, PieceTable pieceTable, InputPosition inputPosition) {
			InsertDocObjectDelegate action;
			if (DocObjectDispatcher.TryGetValue(docObject.DocObjectType, out action)) {
				ContentBuilder.FontManager.SetFontName(docObject.PropertyContainer);
				action(docObject, pieceTable, inputPosition);
			}
		}
		void InsertText(IDocObject docObject, PieceTable pieceTable, InputPosition inputPosition) {
			InsertTextCore(((DocTextRun)docObject).Text, docObject.PropertyContainer.CharacterInfo, pieceTable, inputPosition);
		}
		void InsertExpectedObjectAsText(IDocObject docObject, PieceTable pieceTable, InputPosition inputPosition) {
			InsertTextCore(((ExpectedDocObject)docObject).Text, docObject.PropertyContainer.CharacterInfo, pieceTable, inputPosition);
		}
		void InsertTextCore(string plainText, CharacterInfo info, PieceTable pieceTable, InputPosition pos) {
			SpecifyCharacterFormattingForInputPosition(info, pos);
			plainText = StringHelper.ReplaceParagraphMarksWithLineBreaks(plainText);
			if (!String.IsNullOrEmpty(plainText))
				pieceTable.InsertTextCore(pos, plainText);
			AddDelayedTextFormatting(info, pieceTable);
		}
		void InsertPictureFloatingObject(IDocObject docObject, PieceTable pieceTable, InputPosition inputPosition) {
			InsertPictureFloatingObjectCore((DocPictureFloatingObject)docObject, inputPosition);
		}
		void InsertPictureFloatingObjectCore(DocPictureFloatingObject floatingObject, InputPosition inputPosition) {
			SpecifyCharacterFormattingForInputPosition(floatingObject.PropertyContainer.CharacterInfo, inputPosition);
			FloatingObjectAnchorRun run = ActiveImportInfo.PieceTable.InsertFloatingObjectAnchorCore(inputPosition);
			run.FloatingObjectProperties.CopyFrom(floatingObject.Formatting);
			floatingObject.ApplyShapeProperties(run.Shape);
			run.SetContent(new PictureFloatingObjectContent(run, floatingObject.Image));
		}
		void InsertTextBoxFloatingObject(IDocObject docObject, PieceTable pieceTable, InputPosition inputPosition) {
			InsertTextBoxFloatingObjectCore((DocTextBoxFloatingObject)docObject, inputPosition);
		}
		void InsertTextBoxFloatingObjectCore(DocTextBoxFloatingObject textBox, InputPosition inputPosition) {
			SpecifyCharacterFormattingForInputPosition(textBox.PropertyContainer.CharacterInfo, inputPosition);
			PieceTable pieceTable = ActiveImportInfo.PieceTable;
			FloatingObjectAnchorRun run = pieceTable.InsertFloatingObjectAnchorCore(inputPosition);
			run.FloatingObjectProperties.CopyFrom(textBox.Formatting);
			textBox.ApplyShapeProperties(run.Shape);
			AddDelayedTextFormatting(textBox.PropertyContainer.CharacterInfo, pieceTable);
			TextBoxContentType textBoxContentType = new TextBoxContentType(DocumentModel);
			TextBoxFloatingObjectContent content = new TextBoxFloatingObjectContent(run, textBoxContentType);
			run.SetContent(content);
			textBox.ApplyTextBoxProperties(content.TextBoxProperties);
			DocFloatingObjectsIterator iterator = ContentBuilder.Iterator.FloatingObjectsIterator;
			int id = textBox.ShapeId;
			DocObjectCollection textBoxContent = (pieceTable == DocumentModel.MainPieceTable) ? iterator.GetMainTextBoxObjects(id)
				: iterator.GetHeaderTextBoxObjects(id);
			InsertEmbeddedContent(textBoxContentType.PieceTable, textBoxContent);
		}
		void InsertImage(IDocObject docObject, PieceTable pieceTable, InputPosition inputPosition) {
			DocImage image = (DocImage)docObject;
			if (image != null)
				InsertImageCore(image, pieceTable, inputPosition);
		}
		void InsertImageCore(DocImage image, PieceTable pieceTable, InputPosition position) {
			SpecifyCharacterFormattingForInputPosition(image.PropertyContainer.CharacterInfo, position);
			pieceTable.AppendImage(position, image.Image, image.ScaleX, image.ScaleY);
			AddDelayedImageFormatting(image.PropertyContainer.CharacterInfo, pieceTable);
		}
		void InsertFieldBegin(IDocObject docObject, PieceTable pieceTable, InputPosition inputPosition) {
			ActiveImportInfo.FieldsImporter.ProcessFieldBegin(inputPosition);
		}
		void InsertFieldSeparator(IDocObject docObject, PieceTable pieceTable, InputPosition inputPosition) {
			ActiveImportInfo.FieldsImporter.ProcessFieldSeparator(inputPosition);
		}
		void InsertFieldEnd(IDocObject docObject, PieceTable pieceTable, InputPosition inputPosition) {
			ActiveImportInfo.FieldsImporter.ProcessFieldEnd(inputPosition, docObject.PropertyContainer);
		}
		void InsertHyperlinkFieldData(IDocObject docObject, PieceTable pieceTable, InputPosition inputPosition) {
			DocHyperlinkFieldData hyperlinkData = (DocHyperlinkFieldData)docObject;
			ActiveImportInfo.FieldsImporter.ProcessHyperlinkData(hyperlinkData);
		}
		void InsertFootNoteReference(IDocObject docObject, PieceTable pieceTable, InputPosition inputPosition) {
			FootNote footNote = new FootNote(DocumentModel);
			PieceTable pieceTableFootNote = footNote.PieceTable;
			int index = DocumentModel.FootNotes.Count;
			DocumentModel.UnsafeEditor.InsertFirstParagraph(pieceTableFootNote);
			DocumentModel.FootNotes.Add(footNote);
			CharacterInfo info = docObject.PropertyContainer.CharacterInfo;
			SpecifyCharacterFormattingForInputPosition(info, inputPosition);
			FootNoteRun run = pieceTable.InsertFootNoteRun(inputPosition, index);
			AddDelayedTextFormatting(info, pieceTable);
			footNote.ReferenceRun = run;
			InsertFootNoteContent(footNote, docObject.Position);
		}
		void InsertFootNoteContent(FootNote note, int characterPosition) {
			DocObjectCollection noteContent = ContentBuilder.Iterator.NotesIterator.GetFootNoteObjects(characterPosition);
			InsertEmbeddedContent(note.PieceTable, noteContent);
		}
		void InsertEndnoteReference(IDocObject docObject, PieceTable pieceTable, InputPosition inputPosition) {
			EndNote endNote = new EndNote(DocumentModel);
			PieceTable endNotePieceTable = endNote.PieceTable;
			int index = DocumentModel.EndNotes.Count;
			DocumentModel.UnsafeEditor.InsertFirstParagraph(endNotePieceTable);
			DocumentModel.EndNotes.Add(endNote);
			CharacterInfo info = docObject.PropertyContainer.CharacterInfo;
			SpecifyCharacterFormattingForInputPosition(info, inputPosition);
			EndNoteRun run = pieceTable.InsertEndNoteRun(inputPosition, index);
			AddDelayedTextFormatting(info, pieceTable);
			endNote.ReferenceRun = run;
			InsertEndNoteContent(endNote, docObject.Position);
		}
		void InsertEndNoteContent(EndNote note, int characterPosition) {
			DocObjectCollection noteContent = ContentBuilder.Iterator.NotesIterator.GetEndNoteObjects(characterPosition);
			InsertEmbeddedContent(note.PieceTable, noteContent);
		}
		internal void InsertEmbeddedContent(PieceTable pieceTable, DocObjectCollection content) {
			ContentBuilder.BeginEmbeddedContent();
			UpdateActiveImportInfo(pieceTable);
			InputPosition position = new InputPosition(pieceTable);
			int count = content.Count;
			for (int i = 0; i < count; i++)
				ProcessDocObject(content[i], pieceTable, position);
			Finish(pieceTable);
			ContentBuilder.EndEmbeddedContent();
		}
		void InsertNoteNumber(IDocObject docObject, PieceTable pieceTable, InputPosition inputPosition) {
			CharacterInfo info = docObject.PropertyContainer.CharacterInfo;
			SpecifyCharacterFormattingForInputPosition(info, inputPosition);
			if (pieceTable.ContentType is FootNote)
				TryInsertFootNoteNumber(inputPosition, pieceTable);
			else if (pieceTable.ContentType is EndNote)
				TryInsertEndNoteNumber(inputPosition, pieceTable);
			AddDelayedTextFormatting(info, pieceTable);
		}
		void TryInsertFootNoteNumber(InputPosition position, PieceTable pieceTable) {
			int index = DocumentModel.FootNotes.Count - 1;
			if (index >= 0)
				pieceTable.InsertFootNoteRun(position, index);
		}
		void TryInsertEndNoteNumber(InputPosition position, PieceTable pieceTable) {
			int index = DocumentModel.EndNotes.Count - 1;
			if (index >= 0)
				pieceTable.InsertEndNoteRun(position, index);
		}
		void InsertAnnotationReference(IDocObject docObject, PieceTable pieceTable, InputPosition inputPosition) {
			if (!pieceTable.IsMain)
				return;
			CommentContentType commentContent = new CommentContentType(DocumentModel);
			InsertCommentContent(commentContent, docObject.Position);
			ContentBuilder.Iterator.CommentsIterator.InsertComment(pieceTable, commentContent, docObject.Position);
		}
		void InsertCommentContent(CommentContentType commentContent, int characterPosition) {
			DocObjectCollection docCommentContent = ContentBuilder.Iterator.CommentsIterator.GetCommentsContent(characterPosition);
			InsertEmbeddedContent(commentContent.PieceTable, docCommentContent);
		}
		void InsertTableCell(IDocObject docObject, PieceTable pieceTable, InputPosition inputPosition) {			
			InsertParagraph(docObject, pieceTable, inputPosition);
			ActiveImportInfo.TablesImporter.CellEndReached(docObject.PropertyContainer);
		}
		void InsertTableRow(IDocObject docObject, PieceTable pieceTable, InputPosition inputPosition) {
			DocPropertyContainer propertyContainer = docObject.PropertyContainer;
			propertyContainer.Update(ChangeActionTypes.Table);
			int tableStyleIndex = propertyContainer.TableInfo.TableStyleIndex;
			int styleIndex = tableStyleIndex >= 0 ? StylesImportHelper.GetStyleIndex(propertyContainer.TableInfo.TableStyleIndex, StyleType.TableStyle) : -1;
			propertyContainer.TableInfo.TableStyleIndex = styleIndex;
			ActiveImportInfo.TablesImporter.RowEndReached(propertyContainer);
		}
		void ProcessParagraphListInfoIndex(PieceTable pieceTable, ParagraphIndex index, ParagraphInfo info) {
			Paragraph paragraph = pieceTable.Paragraphs[index];
			int listInfoIndex = info.ListInfoIndex;
			int listLevel = info.ListLevel;
			NumberingListIndex actualListIndex;
			if (listLevel == 0x0c)
				return;
			if (listInfoIndex == 0) {
				actualListIndex = paragraph.ParagraphStyle.GetNumberingListIndex();
				if (actualListIndex < NumberingListIndex.MinValue)
					return;
				if (listLevel >= DocumentModel.NumberingLists[actualListIndex].Levels.Count)
					listLevel = 0;
				pieceTable.AddNumberingListToParagraph(paragraph, NumberingListIndex.NoNumberingList, listLevel);
			}
			if (listInfoIndex > 0 && listInfoIndex - 1 < DocumentModel.NumberingLists.Count) {
				pieceTable.AddNumberingListToParagraph(paragraph, new NumberingListIndex(listInfoIndex - 1), listLevel);
			}
		}
		void SpecifyCharacterFormattingForInputPosition(CharacterInfo info, InputPosition position) {
			if (info == null) {
				position.CharacterFormatting.ResetUse(CharacterFormattingOptions.Mask.UseAll);
				position.CharacterStyleIndex = DocumentModel.CharacterStyles.DefaultItemIndex;
				return;
			}
			position.CharacterStyleIndex = StylesImportHelper.GetStyleIndex(info.FormattingInfo.StyleIndex, StyleType.CharacterStyle);
			bool isInverted = info.FormattingInfo.ContainsInvertedProperties();
			if (!isInverted) {
				CharacterFormattingInfo formattingInfo = StylesImportHelper.GetCharacterFormattingInfo(info.FormattingInfo);
				position.CharacterFormatting.CopyFrom(formattingInfo, info.FormattingOptions);
			}
			else 
				DocumentModel.ResetMerging();
		}
		void AddDelayedTextFormatting(CharacterInfo info, PieceTable pieceTable) {
			AddDelayedFormattingCore(info, pieceTable, pieceTable.LastInsertedRunInfo.Run);
		}
		void AddDelayedImageFormatting(CharacterInfo info, PieceTable pieceTable) {
			AddDelayedFormattingCore(info, pieceTable, pieceTable.LastInsertedInlinePictureRunInfo.Run);
		}
		void AddDelayedFormattingCore(CharacterInfo info, PieceTable pieceTable, TextRunBase run) {
			if (run == null || info == null || !info.FormattingInfo.ContainsInvertedProperties())
				return;
			this.delayedFormatting.Add(run, info);
			DocumentModel.ResetMerging();
		}
		void ProcessDelayedFormatting(Paragraph paragraph) {
			MergedCharacterProperties properties = paragraph.GetMergedCharacterProperties();
			foreach (TextRunBase run in this.delayedFormatting.Keys) {
				CharacterInfo characterInfo = this.delayedFormatting[run];
				CharacterFormattingInfo info = DocCharacterFormattingHelper.GetMergedCharacterFormattingInfo(characterInfo.FormattingInfo, properties.Info);
				run.CharacterProperties.CopyFrom(new MergedProperties<CharacterFormattingInfo, CharacterFormattingOptions>(info, characterInfo.FormattingOptions));
			}
			this.delayedFormatting = new Dictionary<TextRunBase, CharacterInfo>();
		}
		void InsertParagraph(IDocObject docObject, PieceTable pieceTable, InputPosition inputPosition) {
			ParagraphIndex paragraphIndex = inputPosition.ParagraphIndex;
			DocPropertyContainer propertyContainer = docObject.PropertyContainer;
			SetCharacterProperties(inputPosition, propertyContainer);
			pieceTable.InsertParagraphCore(inputPosition);
			SetParagraphProperties(pieceTable, paragraphIndex, propertyContainer);
		}
		void SetCharacterProperties(InputPosition inputPosition, DocPropertyContainer propertyContainer) {
			if (propertyContainer.CharacterInfo == null) {
				inputPosition.CharacterFormatting.ResetUse(CharacterFormattingOptions.Mask.UseAll);
				return;
			}
			DocCharacterFormattingInfo info = propertyContainer.CharacterInfo.FormattingInfo;
			CharacterFormattingOptions formattingOptions = propertyContainer.CharacterInfo.FormattingOptions;
			CharacterFormattingInfo formattingInfo = StylesImportHelper.GetCharacterFormattingInfo(info);
			inputPosition.CharacterFormatting.CopyFrom(formattingInfo, formattingOptions);
			inputPosition.CharacterStyleIndex = StylesImportHelper.GetStyleIndex(info.StyleIndex, StyleType.CharacterStyle);
		}
		void SetParagraphProperties(PieceTable pieceTable, ParagraphIndex paragraphIndex, DocPropertyContainer propertyContainer) {
			Paragraph paragraph = pieceTable.Paragraphs[paragraphIndex];
			ParagraphInfo paragraphInfo = propertyContainer.ParagraphInfo;
			if (paragraphInfo != null) {
				paragraph.ParagraphProperties.CopyFrom(new MergedParagraphProperties(paragraphInfo.FormattingInfo, paragraphInfo.FormattingOptions));
				paragraph.SetOwnTabs(paragraphInfo.Tabs);
				paragraph.ParagraphStyleIndex = StylesImportHelper.GetStyleIndex(paragraphInfo.ParagraphStyleIndex, StyleType.ParagraphStyle);
				ProcessParagraphListInfoIndex(pieceTable, paragraphIndex, paragraphInfo);
				FrameInfo frameInfo = propertyContainer.FrameInfo;
				if (frameInfo != null && DocumentModel.DocumentCapabilities.ParagraphFramesAllowed) {
					paragraph.FrameProperties = new FrameProperties(paragraph);
					paragraph.FrameProperties.CopyFrom(new MergedFrameProperties(frameInfo.FormattingInfo, frameInfo.FormattingOptions));
				}
			}
			ActiveImportInfo.TablesImporter.ParagraphAdded(paragraph, propertyContainer);
			if (this.delayedFormatting.Count > 0)
				ProcessDelayedFormatting(paragraph);
		}
		void InsertSection(IDocObject docObject, PieceTable pieceTable, InputPosition inputPosition) {
			ParagraphIndex paragraphIndex = inputPosition.ParagraphIndex;
			var propContainer = docObject.PropertyContainer;
			if (propContainer.CharacterInfo != null) {
				DocCharacterFormattingInfo info = propContainer.CharacterInfo.FormattingInfo;
				CharacterFormattingInfo formattingInfo = StylesImportHelper.GetCharacterFormattingInfo(info);
				CharacterFormattingOptions formattingOptions = propContainer.CharacterInfo.FormattingOptions;
				inputPosition.CharacterFormatting.CopyFrom(formattingInfo, formattingOptions);
			}
			ActiveImportInfo.TablesImporter.BeforeSectionAdded();
			DocumentModel.MainPieceTable.InsertSectionParagraphCore(inputPosition);
			DocumentModel.SafeEditor.PerformInsertSectionCore(paragraphIndex); 
			if (propContainer.ParagraphInfo != null)
				SetParagraphProperties(pieceTable, paragraphIndex, propContainer);
			ApplySectionProperties(propContainer);
			this.currentSectionIndex++;
		}
		void ApplySectionProperties(DocPropertyContainer propertyContainer) {
			if (propertyContainer.SectionInfo == null || ((IConvertToInt<SectionIndex>)this.currentSectionIndex).ToInt() >= DocumentModel.Sections.Count)
				return;
			Section currentSection = DocumentModel.Sections[this.currentSectionIndex];
			currentSection.Columns.CopyFrom(propertyContainer.SectionInfo.SectionColumns);
			currentSection.Margins.CopyFrom(propertyContainer.SectionInfo.SectionMargins);
			if (ContentBuilder.DocumentProperties.GutterPosition == GutterPosition.Top)
				currentSection.Margins.GutterAlignment = SectionGutterAlignment.Top;
			propertyContainer.SectionInfo.SectionPage.ValidatePaperKind(DocumentModel.UnitConverter);
			currentSection.Page.CopyFrom(propertyContainer.SectionInfo.SectionPage);
			currentSection.GeneralSettings.CopyFrom(propertyContainer.SectionInfo.SectionGeneralSettings);
			currentSection.PageNumbering.CopyFrom(propertyContainer.SectionInfo.SectionPageNumbering);
			currentSection.LineNumbering.CopyFrom(propertyContainer.SectionInfo.SectionLineNumbering);
			currentSection.FootNote.CopyFrom(propertyContainer.SectionInfo.FootNote);
			currentSection.EndNote.CopyFrom(propertyContainer.SectionInfo.EndNote);
		}
		void UpdateActiveImportInfo(PieceTable pieceTable) {
			ImportInfoStack.Push(new DocImportPieceTableInfo(this, pieceTable));
		}
		internal static void ThrowInvalidDocFile() {
			throw new ArgumentException("Invalid Doc file");
		}
		public override void ThrowInvalidFile() {
			throw new ArgumentException("Invalid Doc file");
		}
		public void AddCellsHorizontalMerging(TableRow row, int firstCellIndex, int span) {
			List<DocTableCellHorizontalMerging> list;
			if (!cellsHorizontalMerging.TryGetValue(row, out list)) {
				list = new List<DocTableCellHorizontalMerging>();
				cellsHorizontalMerging.Add(row, list);
			}
			list.Add(new DocTableCellHorizontalMerging(firstCellIndex, span));
		}
	}
	public class DocTableCellHorizontalMerging {
		readonly int firstCellIndex;
		readonly int count;
		public DocTableCellHorizontalMerging(int firstCellIndex, int count) {
			this.firstCellIndex = firstCellIndex;
			this.count = count;
		}
		public int FirstCellIndex { get { return firstCellIndex; } }
		public int Count { get { return count; } }
	}
	#region DocImportPieceTableInfo
	public class DocImportPieceTableInfo {
		#region Fields
		PieceTable pieceTable;
		ImportPieceTableInfo pieceTableInfo;
		DocFieldsImporter fieldsImporter;
		DocTablesImporter tablesImporter;
		#endregion
		public DocImportPieceTableInfo(DocImporter importer, PieceTable pieceTable) {
			this.pieceTable = pieceTable;
			this.pieceTableInfo = new ImportPieceTableInfo(pieceTable);
			this.tablesImporter = new DocTablesImporter(importer, pieceTable);
			this.fieldsImporter = new DocFieldsImporter(pieceTable, this.pieceTableInfo.FieldInfoStack);
		}
		#region Properties
		public PieceTable PieceTable { get { return pieceTable; } }
		public ImportPieceTableInfo PieceTableInfo { get { return pieceTableInfo; } }
		public DocFieldsImporter FieldsImporter { get { return fieldsImporter; } }
		public DocTablesImporter TablesImporter { get { return tablesImporter; } }
		#endregion
	}
	#endregion
}
