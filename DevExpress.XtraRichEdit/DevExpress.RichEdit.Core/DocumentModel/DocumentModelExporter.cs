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
using System.IO;
using System.Text;
using System.Xml;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Utils.Internal;
using Debug = System.Diagnostics.Debug;
#if SL
using System.Windows.Media;
#else
using System.IO.Compression;
using DevExpress.XtraRichEdit.Model;
using System.Diagnostics;
#endif
namespace DevExpress.XtraRichEdit.Internal {
	using DevExpress.XtraRichEdit.Model;
	#region VerticalMergeCellsCollection
	public class VerticalMergeCellsCollection : Dictionary<TableCell, VerticalMergeCellProperties> {
		public int GetRowSpan(TableCell cell) {
			if (ContainsKey(cell))
				return this[cell].RowSpan;
			return 1;
		}
		public VerticalMergeCellProperties GetMergedCellProperties(TableCell cell) {
			if (ContainsKey(cell))
				return this[cell];
			return new VerticalMergeCellProperties(1, null);
		}
	}
	#endregion
	#region VerticalMergeCellProperties
	public struct VerticalMergeCellProperties {
		int rowSpan;
		BorderBase actualBottomBorder;
		public VerticalMergeCellProperties(int rowSpan, BorderBase bottomBorder) {
			this.rowSpan = rowSpan;
			this.actualBottomBorder = bottomBorder;
		}
		public int RowSpan { get { return rowSpan; } set { rowSpan = value; } }
		public BorderBase ActualBottomBorder { get { return actualBottomBorder; } set { actualBottomBorder = value; } }
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Model {
	public delegate void ExportPieceTableDelegate();
	#region FootNoteExportInfo
	public class FootNoteExportInfo {
		string numberText;
		readonly int number;
		readonly PieceTable note;
		int id;
		public FootNoteExportInfo(PieceTable note, int number, string numberText) {
			Guard.ArgumentNotNull(note, "note");
			this.note = note;
			this.number = number;
			this.numberText = numberText;
		}
		public string NumberText { get { return numberText; } set { numberText = value; } }
		public int Number { get { return number; } }
		public PieceTable Note { get { return note; } }
		public int Id { get { return id; } set { id = value; } }
	}
	#endregion
	#region DocumentModelExporter (abstract class)
	public abstract class DocumentModelExporter : IDocumentModelExporter {
		#region Fields
		readonly DocumentModel documentModel;
		PieceTable pieceTable;
		PieceTableNumberingListCountersManager pieceTableNumberingListCounters;
		int fieldLevel;
		Stack<VisitableDocumentIntervalBoundaryIterator> visitableDocumentIntervalsIteratorStack;
		readonly ProgressIndication progressIndication;
		int exportedParagraphCount;
		int footNoteNumber;
		int endNoteNumber;
		readonly List<FootNoteExportInfo> footNoteExportInfos;
		readonly List<FootNoteExportInfo> endNoteExportInfos;
		Section currentSection;
		#endregion
		protected DocumentModelExporter(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = PrepareModelForExport(documentModel);
			this.pieceTable = this.documentModel.MainPieceTable;
			this.pieceTableNumberingListCounters = new PieceTableNumberingListCountersManager();
			this.progressIndication = CreateProgressIndication();
			this.visitableDocumentIntervalsIteratorStack = new Stack<VisitableDocumentIntervalBoundaryIterator>();
			this.footNoteExportInfos = new List<FootNoteExportInfo>();
			this.endNoteExportInfos = new List<FootNoteExportInfo>();
		}
		#region Properties
		public PieceTable PieceTable { get { return pieceTable; } }
		public DocumentModel DocumentModel { get { return documentModel; } }
		protected PieceTableNumberingListCountersManager PieceTableNumberingListCounters { get { return pieceTableNumberingListCounters; } }
		protected internal virtual bool ShouldExportHiddenText { get { return false; } }
		protected internal ProgressIndication ProgressIndication { get { return progressIndication; } }
		protected internal int ExportedParagraphCount { get { return exportedParagraphCount; } set { exportedParagraphCount = value; } }
		protected int FieldLevel { get { return this.fieldLevel; } }
		protected Stack<VisitableDocumentIntervalBoundaryIterator> VisitableDocumentIntervalsIteratorStack { get { return visitableDocumentIntervalsIteratorStack; } }
		protected VisitableDocumentIntervalBoundaryIterator VisitableDocumentIntervalsIterator { get { return visitableDocumentIntervalsIteratorStack.Count > 0 ? visitableDocumentIntervalsIteratorStack.Peek() : null; } }
		protected internal virtual bool ShouldCalculateFootNoteAndEndNoteNumbers { get { return false; } }
		protected internal List<FootNoteExportInfo> FootNoteExportInfos { get { return footNoteExportInfos; } }
		protected internal List<FootNoteExportInfo> EndNoteExportInfos { get { return endNoteExportInfos; } }
		#endregion
		protected virtual DocumentModel PrepareModelForExport(DocumentModel documentModel) { return documentModel; }
		public virtual void Export() {
			ProgressIndication.Begin(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_Saving), 0, CalculateTotalParagraphCount(), 0);
			try {
				PerformExportPieceTable(DocumentModel.MainPieceTable, ExportDocument);
			}
			finally {
				ProgressIndication.End();
			}
		}
		protected internal virtual void PerformExportPieceTable(PieceTable pieceTable, ExportPieceTableDelegate pieceTableExporter) {
			PieceTable originalPieceTable = this.pieceTable;
			PieceTableNumberingListCountersManager originalPieceTableNumberingListCounters = this.pieceTableNumberingListCounters;
			this.pieceTable = pieceTable;
			this.pieceTableNumberingListCounters = new PieceTableNumberingListCountersManager();
			this.pieceTableNumberingListCounters.BeginCalculateCounters();
			try {
				PushVisitableDocumentIntervalBoundaryIterator();
				if (!ShouldSplitRuns())
					pieceTableExporter();
				else {
					DocumentModel.BeginUpdate();
					try {
						CompositeHistoryItem transaction = DocumentModel.History.Transaction;
						int transactionItemCountBeforeExecute = transaction.Items.Count;
						SplitRuns();
						pieceTableExporter();
						if(!DocumentModel.ModelForExport) {
							for(int i = transaction.Items.Count - 1; i >= transactionItemCountBeforeExecute; i--) {
								transaction.Items[i].Undo();
								transaction.Items.RemoveAt(i);
							}
						}
						InvalidateParagraphsBoxes();
					}
					finally {
						DocumentModel.EndUpdate();
					}
				}
			}
			finally {
				PopVisitableDocumentIntervalBoundaryIterator();
				this.pieceTableNumberingListCounters.EndCalculateCounters();
				if (originalPieceTable == null)
					this.pieceTable = documentModel.MainPieceTable;
				else
					this.pieceTable = originalPieceTable;
				if (originalPieceTableNumberingListCounters == null)
					this.pieceTableNumberingListCounters = new PieceTableNumberingListCountersManager();
				else
					this.pieceTableNumberingListCounters = originalPieceTableNumberingListCounters;
			}
		}
		protected internal virtual void ExportPieceTable() {
			ExportParagraphs(ParagraphIndex.Zero, new ParagraphIndex(PieceTable.Paragraphs.Count - 1));
			TryToExportBookmarks(new RunIndex(PieceTable.Runs.Count - 1), 1);
		}
		protected internal virtual void SplitRuns() {
			SplitRunsByBookmarkBoundaries();
			InvalidateParagraphsBoxes();
		}
		void InvalidateParagraphsBoxes() {
			foreach (Paragraph paragraph in pieceTable.Paragraphs) {
				paragraph.BoxCollection.InvalidateBoxes();
			}
		}
		protected internal virtual bool ShouldSplitRuns() {
			return VisitableDocumentIntervalsIterator.Boundaries.Count > 0;
		}
		void SplitRunsByBookmarkBoundaries() {
			VisitableDocumentIntervalBoundaryCollection boundaries = VisitableDocumentIntervalsIterator.Boundaries;
			int count = boundaries.Count;
			for (int i = 0; i < count; i++)
				SplitRunByModelPosition(boundaries[i].Position);
		}
		void SplitRunByModelPosition(DocumentModelPosition pos) {
			if (pos.RunOffset != 0 && pos.LogPosition <= PieceTable.DocumentEndLogPosition)
				PieceTable.SplitTextRun(pos.ParagraphIndex, pos.RunIndex, pos.RunOffset);
		}
		protected internal virtual void ExportDocument() {
			DocumentModel.Sections.ForEach(ExportSectionFiltered);
			TryToExportBookmarks(new RunIndex(PieceTable.Runs.Count - 1), 1);
		}
		protected internal virtual int CalculateTotalParagraphCount() {
			int result = 0;
			SectionCollection sections = DocumentModel.Sections;
			SectionIndex count = new SectionIndex(sections.Count);
			for (SectionIndex i = new SectionIndex(0); i < count; i++)
				result += CalculateSectionParagraphCount(sections[i]);
			return result;
		}
		protected internal virtual int CalculateSectionParagraphCount(Section section) {
			int result = section.LastParagraphIndex - section.FirstParagraphIndex + 1;
			result += CalculatePieceTableParagraphCount(section.InnerFirstPageHeader);
			result += CalculatePieceTableParagraphCount(section.InnerOddPageHeader);
			result += CalculatePieceTableParagraphCount(section.InnerEvenPageHeader);
			result += CalculatePieceTableParagraphCount(section.InnerFirstPageFooter);
			result += CalculatePieceTableParagraphCount(section.InnerOddPageFooter);
			result += CalculatePieceTableParagraphCount(section.InnerEvenPageFooter);
			return result;
		}
		protected internal virtual int CalculatePieceTableParagraphCount(SectionHeaderFooterBase headerFooter) {
			if (headerFooter == null)
				return 0;
			else
				return headerFooter.PieceTable.Paragraphs.Count;
		}
		protected internal virtual void PushVisitableDocumentIntervalBoundaryIterator() {
			visitableDocumentIntervalsIteratorStack.Push(CreateVisitableDocumentIntervalBoundaryIterator());
		}
		protected virtual VisitableDocumentIntervalBasedObjectBoundaryIterator CreateVisitableDocumentIntervalBoundaryIterator() {
			return new VisitableDocumentIntervalBasedObjectBoundaryIterator(PieceTable);
		}
		protected internal virtual void PopVisitableDocumentIntervalBoundaryIterator() {
			Debug.Assert(visitableDocumentIntervalsIteratorStack.Count > 0);
			visitableDocumentIntervalsIteratorStack.Pop();
		}
		protected internal virtual void ExportSectionFiltered(Section section) {
			if (ShouldExportSection(section))
				ExportSection(section);
			else {
				ExportedParagraphCount += CalculateSectionParagraphCount(section);
				ProgressIndication.SetProgress(ExportedParagraphCount);
			}
		}
		protected internal virtual void ExportSection(Section section) {
			this.currentSection = section;
			if (ShouldCalculateFootNoteAndEndNoteNumbers) {
				CalculateFootNoteNumber(section);
				CalculateEndNoteNumber(section);
			}
			ExportSectionHeadersFooters(section);
			ExportParagraphs(section.FirstParagraphIndex, section.LastParagraphIndex);
		}
		protected internal virtual void CalculateFootNoteNumber(Section section) {
			this.footNoteNumber = CalculateFootNoteNumberCore(section.FootNote, footNoteNumber);
		}
		protected internal virtual void CalculateEndNoteNumber(Section section) {
			this.endNoteNumber = CalculateFootNoteNumberCore(section.EndNote, footNoteNumber);
		}
		protected internal virtual int CalculateFootNoteNumberCore(SectionFootNote properties, int value) {
			if (properties.NumberingRestartType == LineNumberingRestart.NewSection)
				return properties.StartingNumber - 1; 
			else
				return value;
		}
		protected internal virtual void ExportSectionHeadersFooters(Section section) {
			ExportSectionHeadersFootersCore(section);
		}
		protected internal virtual void ExportSectionHeadersFootersCore(Section section) {
			if (!DocumentModel.DocumentCapabilities.HeadersFootersAllowed)
				return;
			if (section.InnerFirstPageHeader != null)
				ExportFirstPageHeader(section.InnerFirstPageHeader, section.Headers.IsLinkedToPrevious(HeaderFooterType.First));
			if (section.InnerOddPageHeader != null)
				ExportOddPageHeader(section.InnerOddPageHeader, section.Headers.IsLinkedToPrevious(HeaderFooterType.Odd));
			if (section.InnerEvenPageHeader != null)
				ExportEvenPageHeader(section.InnerEvenPageHeader, section.Headers.IsLinkedToPrevious(HeaderFooterType.Even));
			if (section.InnerFirstPageFooter != null)
				ExportFirstPageFooter(section.InnerFirstPageFooter, section.Footers.IsLinkedToPrevious(HeaderFooterType.First));
			if (section.InnerOddPageFooter != null)
				ExportOddPageFooter(section.InnerOddPageFooter, section.Footers.IsLinkedToPrevious(HeaderFooterType.Odd));
			if (section.InnerEvenPageFooter != null)
				ExportEvenPageFooter(section.InnerEvenPageFooter, section.Footers.IsLinkedToPrevious(HeaderFooterType.Even));
		}
		protected internal virtual void ExportFirstPageHeader(SectionHeader sectionHeader, bool linkedToPrevious) {
			PerformExportPieceTable(sectionHeader.PieceTable, ExportPieceTable);
		}
		protected internal virtual void ExportOddPageHeader(SectionHeader sectionHeader, bool linkedToPrevious) {
			PerformExportPieceTable(sectionHeader.PieceTable, ExportPieceTable);
		}
		protected internal virtual void ExportEvenPageHeader(SectionHeader sectionHeader, bool linkedToPrevious) {
			PerformExportPieceTable(sectionHeader.PieceTable, ExportPieceTable);
		}
		protected internal virtual void ExportFirstPageFooter(SectionFooter sectionFooter, bool linkedToPrevious) {
			PerformExportPieceTable(sectionFooter.PieceTable, ExportPieceTable);
		}
		protected internal virtual void ExportOddPageFooter(SectionFooter sectionFooter, bool linkedToPrevious) {
			PerformExportPieceTable(sectionFooter.PieceTable, ExportPieceTable);
		}
		protected internal virtual void ExportEvenPageFooter(SectionFooter sectionFooter, bool linkedToPrevious) {
			PerformExportPieceTable(sectionFooter.PieceTable, ExportPieceTable);
		}
		protected internal virtual void ExportParagraphs(ParagraphIndex from, ParagraphIndex to) {
			for (ParagraphIndex i = from; i <= to; i++) {
				i = ExportParagraphFiltered(PieceTable.Paragraphs[i]);
			}
		}
		protected internal ParagraphIndex ExportParagraphFiltered(Paragraph paragraph) {
			ExportedParagraphCount++;
			ProgressIndication.SetProgress(ExportedParagraphCount);
			if (!ShouldExportParagraph(paragraph))
				return paragraph.Index;
			TableCell paragraphCell = paragraph.GetCell();
			if (ShouldUseCustomSaveTableMethod() || paragraphCell == null ||
				!DocumentModel.DocumentCapabilities.TablesAllowed)
				return ExportParagraph(paragraph);
			else
				return ExportRootTable(paragraphCell.Table);
		}
		protected internal virtual bool ShouldUseCustomSaveTableMethod() {
			return false;
		}
		ParagraphIndex ExportRootTable(Table table) {
			TableInfo tableInfo = new TableInfo(table);
			if (table.NestedLevel > 0) {
				Table rootTable = tableInfo.GetRootTable();
				TableInfo rootTableInfo = new TableInfo(rootTable);
				return ExportTable( rootTableInfo);
			}
			return ExportTable(tableInfo);
		}
		Stack<Color> tableBackgroundColorStack = new Stack<Color>();
		protected internal Color CurrentTableBackgroundColor {
			get {
				if (tableBackgroundColorStack.Count <= 0)
					return DXColor.Empty;
				else
					return tableBackgroundColorStack.Peek();
			}
		}
		protected internal void PushCurrentTableBackgroundColor(Color color) {
			if (DXColor.IsTransparentOrEmpty(color))
				color = CurrentTableBackgroundColor;
			tableBackgroundColorStack.Push(color);
		}
		protected internal void PopCurrentTableBackgroundColor() {
			tableBackgroundColorStack.Pop();
		}
		protected internal virtual ParagraphIndex ExportTable(TableInfo tableInfo) {
			PushCurrentTableBackgroundColor(tableInfo.Table.BackgroundColor);
			int rowsCount = tableInfo.Table.Rows.Count;
			for (int i = 0; i < rowsCount; i++)
				ExportRow(tableInfo.Table.Rows[i], tableInfo);
			PopCurrentTableBackgroundColor();
			return tableInfo.Table.EndParagraphIndex;
		}
		protected internal virtual void ExportRow(TableRow row, TableInfo tableInfo) {
			int count = row.Cells.Count;
			for (int cellIndex = 0; cellIndex < count; cellIndex++)
				ExportCell(row.Cells[cellIndex], tableInfo);
		}
		protected internal virtual void ExportCell(TableCell cell, TableInfo tableInfo) {
			PushCurrentTableBackgroundColor(cell.BackgroundColor);
			ParagraphIndex startParagraphIndex = cell.StartParagraphIndex;
			ParagraphIndex endParagraphIndex = cell.EndParagraphIndex;
			for (ParagraphIndex parIndex = startParagraphIndex; parIndex <= endParagraphIndex; parIndex++) {
				TableCell paragraphCell = PieceTable.Paragraphs[parIndex].GetCell();
				if (paragraphCell.Table.NestedLevel > tableInfo.NestedLevel) {
					TableInfo paragraphTable = new TableInfo(paragraphCell.Table);
					TableInfo parentTableInfo = new TableInfo(paragraphTable.GetParentTable(tableInfo.NestedLevel + 1));
					parIndex = ExportTable(parentTableInfo);
				}
				else {
					Paragraph paragraph = PieceTable.Paragraphs[parIndex];
					ExportParagraph(paragraph);
				}
			}
			PopCurrentTableBackgroundColor();
		}
		protected virtual TableInfo CreateTableInfo(Table table) {
			return new TableInfo(table);
		}
		protected internal virtual ParagraphIndex ExportParagraph(Paragraph paragraph) {
			ExportParagraphRuns(paragraph);
			return paragraph.Index;
		}
		protected internal virtual void ExportParagraphRuns(Paragraph paragraph) {
			RunIndex lastRunIndex = paragraph.LastRunIndex;
			for (RunIndex i = paragraph.FirstRunIndex; i <= lastRunIndex; i++) {
				TryToExportBookmarks(i, 0);
				ExportRun(i);
			}
		}
		protected internal virtual void ExportRun(RunIndex i) {
			PieceTable.Runs[i].Export(this);
		}
		protected internal virtual void TryToExportBookmarks(RunIndex runIndex, int runOffset) {
			if (VisitableDocumentIntervalsIterator == null)
				return;
			for (; !VisitableDocumentIntervalsIterator.IsDone(); VisitableDocumentIntervalsIterator.MoveNext()) {
				VisitableDocumentIntervalBoundary boundary = VisitableDocumentIntervalsIterator.Current;
				if (boundary.Position.RunIndex != runIndex || boundary.Position.RunOffset != runOffset)
					break;
				boundary.Export(this);
			}
		}
		protected internal virtual void ExportTextRun(TextRun run) {
		}
		protected internal virtual void ExportCustomRun(CustomRun run) {
		}
		protected internal virtual void ExportSeparatorTextRun(SeparatorTextRun run) {
		}
		protected internal virtual void ExportDataContainerRun(DataContainerRun run) {
		}
		protected internal virtual void ExportInlineObjectRun(InlineObjectRun run) {
		}
		protected internal virtual void ExportImageReference(FloatingObjectAnchorRun run) {
		}
		protected internal virtual void ExportInlinePictureRun(InlinePictureRun run) {
		}
		protected internal virtual void ExportParagraphRun(ParagraphRun run) {
		}
		protected internal virtual void ExportSectionRun(SectionRun run) {
		}
		protected internal virtual void ExportFieldCodeStartRun(FieldCodeStartRun run) {
			fieldLevel++;
		}
		protected internal virtual void ExportFieldCodeEndRun(FieldCodeEndRun run) {
			fieldLevel--;
		}
		protected internal virtual void ExportFieldResultEndRun(FieldResultEndRun run) {
		}
		protected internal virtual void ExportFootNoteRun(FootNoteRun run) {
			this.footNoteNumber++;
		}
		protected internal virtual void ExportEndNoteRun(EndNoteRun run) {
			this.endNoteNumber++;
		}
		protected internal virtual void ExportFloatingObjectAnchorRun(FloatingObjectAnchorRun run) {
		}
		protected internal virtual FootNoteExportInfo CreateFootNoteExportInfo(FootNoteRun note) {
			return new FootNoteExportInfo(note.Note.PieceTable, footNoteNumber, currentSection.FootNote.FormatCounterValue(footNoteNumber));
		}
		protected internal virtual FootNoteExportInfo FindFootNoteExportInfoByNote(List<FootNoteExportInfo> list, PieceTable pieceTable) {
			int count = list.Count;
			for (int i = 0; i < count; i++)
				if (list[i].Note == pieceTable)
					return list[i];
			return null;
		}
		protected internal virtual FootNoteExportInfo CreateFootNoteExportInfo(EndNoteRun note) {
			return new FootNoteExportInfo(note.Note.PieceTable, endNoteNumber, currentSection.EndNote.FormatCounterValue(endNoteNumber));
		}
		protected internal virtual void ExportBookmarkStart(Bookmark bookmark) {
		}
		protected internal virtual void ExportBookmarkEnd(Bookmark bookmark) {
		}
		protected internal virtual void ExportRangePermissionStart(RangePermission rangePermission) {
		}
		protected internal virtual void ExportRangePermissionEnd(RangePermission rangePermission) {
		}
		protected internal virtual void ExportCommentStart(Comment comment) {
		}
		protected internal virtual void ExportCommentEnd(Comment comment) {
		}
		protected internal virtual bool ShouldExportSection(Section section) {
			if (ShouldExportHiddenText)
				return true;
			return !section.IsHidden();
		}
		protected internal virtual bool ShouldExportParagraph(Paragraph paragraph) {
			if (ShouldExportHiddenText)
				return true;
			return !paragraph.IsHidden();
		}
		protected internal virtual bool ShouldExportRun(TextRunBase run) {
			if (ShouldExportHiddenText)
				return true;
			if (fieldLevel > 0)
				return false;
			return !run.Hidden;
		}
		protected internal virtual bool ShouldExportInlinePicture(InlinePictureRun run) {
			return !InternalOfficeImageHelper.GetSuppressStore(run.Image) && ShouldExportRun(run);
		}
		protected internal virtual ProgressIndication CreateProgressIndication() {
			return new ProgressIndication(DocumentModel);
		}
		protected internal virtual string GetNumberingListText(Paragraph paragraph) {
			return paragraph.GetNumberingListText(this.pieceTableNumberingListCounters.CalculateNextCounters(paragraph));
		}
		#region IDocumentModelExporter Members
		void IDocumentModelExporter.Export(SeparatorTextRun run) {
			if (ShouldExportRun(run))
				ExportSeparatorTextRun(run);
		}
		void IDocumentModelExporter.Export(DataContainerRun run) {
			if (ShouldExportRun(run))
				ExportDataContainerRun(run);
		}
		void IDocumentModelExporter.Export(CustomRun run) {
			if (ShouldExportRun(run))
				ExportCustomRun(run);
		}
		void IDocumentModelExporter.Export(TextRun run) {
			if (ShouldExportRun(run))
				ExportTextRun(run);
		}
		void IDocumentModelExporter.Export(InlineObjectRun run) {
			if (ShouldExportRun(run))
				ExportInlineObjectRun(run);
		}
		void IDocumentModelExporter.Export(InlinePictureRun run) {
			if (ShouldExportInlinePicture(run))
				ExportInlinePictureRun(run);
		}
		void IDocumentModelExporter.Export(ParagraphRun run) {
			if (ShouldExportRun(run))
				ExportParagraphRun(run);
		}
		void IDocumentModelExporter.Export(SectionRun run) {
			if (ShouldExportRun(run))
				ExportSectionRun(run);
		}
		void IDocumentModelExporter.Export(FieldCodeStartRun run) {
			ExportFieldCodeStartRun(run);
		}
		void IDocumentModelExporter.Export(FieldCodeEndRun run) {
			ExportFieldCodeEndRun(run);
		}
		void IDocumentModelExporter.Export(FieldResultEndRun run) {
			ExportFieldResultEndRun(run);
		}
		void IDocumentModelExporter.Export(FootNoteRun run) {
			if (ShouldExportRun(run))
				ExportFootNoteRun(run);
		}
		void IDocumentModelExporter.Export(EndNoteRun run) {
			if (ShouldExportRun(run))
				ExportEndNoteRun(run);
		}
		void IDocumentModelExporter.Export(FloatingObjectAnchorRun run) {
			if (ShouldExportRun(run))
				ExportFloatingObjectAnchorRun(run);
		}
		#endregion
	}
	#endregion
	public abstract class XmlBasedDocumentModelExporter : DocumentModelExporter {
		DateTime now;
		protected XmlBasedDocumentModelExporter(DocumentModel documentModel)
			: base(documentModel) {
			this.now = DateTime.Now;
		}
		protected internal abstract InternalZipArchive Package { get; }
		protected DateTime Now { get { return now; } }
		protected internal virtual XmlWriterSettings CreateXmlWriterSettings() {
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = false;
			settings.Encoding = DXEncoding.UTF8NoByteOrderMarks;
			settings.CheckCharacters = true;
			settings.OmitXmlDeclaration = false;
			return settings;
		}
		protected internal virtual Stream CreateDeflateStream(Stream baseStream) {
			return new DeflateStream(baseStream, CompressionMode.Compress, true);
		}
		protected internal virtual void BeforeCreateXmlContent(XmlWriter writer) {
		}
		protected internal virtual Stream CreateXmlContent(Action<XmlWriter> action) {
			using (MemoryStream stream = new MemoryStream()) {
				using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8)) {
					using (XmlWriter writer = XmlWriter.Create(streamWriter, CreateXmlWriterSettings())) {
						BeforeCreateXmlContent(writer);
						action(writer);
						writer.Flush();
						return new MemoryStream(stream.GetBuffer(), 0, (int)stream.Length);
					}
				}
			}
		}
		protected internal virtual CompressedStream CreateCompressedXmlContent(Action<XmlWriter> action) {
			return CreateCompressedXmlContent(action, Encoding.UTF8);
		}
		protected internal virtual CompressedStream CreateCompressedXmlContent(Action<XmlWriter> action, Encoding encoding) {
			if (Package == null)
				Exceptions.ThrowInternalException();
			using (MemoryStream stream = new MemoryStream()) {
				using (Stream deflateStream = CreateDeflateStream(stream)) {
					using (Crc32Stream crc32Stream = new Crc32Stream(deflateStream)) {
						using (ByteCountStream byteCountStream = new ByteCountStream(crc32Stream)) {
							using (StreamWriter streamWriter = new StreamWriter(byteCountStream, encoding)) {
								using (XmlWriter writer = XmlWriter.Create(streamWriter, CreateXmlWriterSettings())) {
									BeforeCreateXmlContent(writer);
									action(writer);
								}
							}
#if DXPORTABLE
							deflateStream.Dispose();
#else
							deflateStream.Close();
#endif
							CompressedStream result = new CompressedStream();
							result.Stream = new MemoryStream(stream.GetBuffer(), 0, (int)stream.Length);
							result.Crc32 = (int)crc32Stream.WriteCheckSum;
							result.UncompressedSize = byteCountStream.WriteCheckSum;
							return result;
						}
					}
				}
			}
		}
		protected internal virtual void AddPackageContent(string fileName, Stream content) {
			Package.Add(fileName, now, content);
		}
		protected internal virtual void AddPackageContent(string fileName, byte[] content) {
			Package.Add(fileName, now, content);
		}
		protected internal virtual void AddCompressedPackageContent(string fileName, CompressedStream content) {
			Package.AddCompressed(fileName, now, content);
		}
		protected internal virtual void AddPackageImage(string fileName, OfficeImage image) {
			byte[] imageBytes = GetImageBytes(image);
			Package.Add(fileName, Now, imageBytes);
		}
		protected internal virtual string GetImageExtension(OfficeImage image) {
			string result = OfficeImage.GetExtension(image.RawFormat);
			if (String.IsNullOrEmpty(result))
				result = "png";
			return result;
		}
		protected internal virtual Stream GetImageBytesStream(OfficeImage image) {
			return image.GetImageBytesStreamSafe(image.RawFormat);
		}
		protected internal virtual byte[] GetImageBytes(OfficeImage image) {
			return image.GetImageBytesSafe(image.RawFormat);
		}
	}
#region BookmarkBoundaryOrder
	public enum BookmarkBoundaryOrder {
		Start = 0,
		End = 1
	}
#endregion
#region VisitableDocumentIntervalBoundary (abstract class)
	public abstract class VisitableDocumentIntervalBoundary {
		readonly VisitableDocumentInterval interval;
		int intervalIndex = -1;
		protected VisitableDocumentIntervalBoundary(VisitableDocumentInterval interval) {
			Guard.ArgumentNotNull(interval, "interval");
			this.interval = interval;
		}
		public VisitableDocumentInterval VisitableInterval { get { return interval; } }
		public abstract DocumentModelPosition Position { get; }
		public abstract BookmarkBoundaryOrder Order { get; } 
		public int IntervalIndex { get { return intervalIndex; } set { intervalIndex = value; } }
		public abstract void Export(DocumentModelExporter exporter);
		protected internal abstract VisitableDocumentIntervalBox CreateBox();
	}
#endregion
#region BookmarkStartBoundary
	public class BookmarkStartBoundary : VisitableDocumentIntervalBoundary {
		public BookmarkStartBoundary(Bookmark bookmark)
			: base(bookmark) {
		}
		public override DocumentModelPosition Position { get { return VisitableInterval.Interval.Start; } }
		public override BookmarkBoundaryOrder Order { get { return BookmarkBoundaryOrder.Start; } }
		public override void Export(DocumentModelExporter exporter) {
			exporter.ExportBookmarkStart((Bookmark)VisitableInterval);
		}
		protected internal override VisitableDocumentIntervalBox CreateBox() {
			return new BookmarkStartBox();
		}
	}
#endregion
#region BookmarkEndBoundary
	public class BookmarkEndBoundary : VisitableDocumentIntervalBoundary {
		public BookmarkEndBoundary(Bookmark bookmark)
			: base(bookmark) {
		}
		public override DocumentModelPosition Position { get { return VisitableInterval.Interval.End; } }
		public override BookmarkBoundaryOrder Order { get { return BookmarkBoundaryOrder.End; } }
		public override void Export(DocumentModelExporter exporter) {
			exporter.ExportBookmarkEnd((Bookmark)VisitableInterval);
		}
		protected internal override VisitableDocumentIntervalBox CreateBox() {
			return new BookmarkEndBox();
		}
	}
#endregion
#region RangePermissionBoundary (abstract class)
	public abstract class RangePermissionBoundary : VisitableDocumentIntervalBoundary {
		protected RangePermissionBoundary(RangePermission bookmark)
			: base(bookmark) {
		}
		protected internal abstract void OnRunInserted(ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId);
		protected internal abstract void OnRunMerged(ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId);
	}
#endregion
#region RangePermissionStartBoundary
	public class RangePermissionStartBoundary : RangePermissionBoundary {
		public RangePermissionStartBoundary(RangePermission bookmark)
			: base(bookmark) {
		}
		public override DocumentModelPosition Position { get { return VisitableInterval.Interval.Start; } }
		public override BookmarkBoundaryOrder Order { get { return BookmarkBoundaryOrder.Start; } }
		public override void Export(DocumentModelExporter exporter) {
			exporter.ExportRangePermissionStart((RangePermission)VisitableInterval);
		}
		protected internal override VisitableDocumentIntervalBox CreateBox() {
			return new BookmarkStartBox();
		}
		protected internal override void OnRunInserted(ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			((RangePermission)VisitableInterval).OnRunInsertedStart(paragraphIndex, newRunIndex, length, historyNotificationId);
		}
		protected internal override void OnRunMerged(ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			((RangePermission)VisitableInterval).OnRunMergedStart(paragraphIndex, newRunIndex, length);
		}
	}
#endregion
#region RangePermissionEndBoundary
	public class RangePermissionEndBoundary : RangePermissionBoundary {
		public RangePermissionEndBoundary(RangePermission bookmark)
			: base(bookmark) {
		}
		public override DocumentModelPosition Position { get { return VisitableInterval.Interval.End; } }
		public override BookmarkBoundaryOrder Order { get { return BookmarkBoundaryOrder.End; } }
		public override void Export(DocumentModelExporter exporter) {
			exporter.ExportRangePermissionEnd((RangePermission)VisitableInterval);
		}
		protected internal override VisitableDocumentIntervalBox CreateBox() {
			return new BookmarkEndBox();
		}
		protected internal override void OnRunInserted(ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			((RangePermission)VisitableInterval).OnRunInsertedEnd(paragraphIndex, newRunIndex, length, historyNotificationId);
		}
		protected internal override void OnRunMerged(ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			((RangePermission)VisitableInterval).OnRunMergedEnd(paragraphIndex, newRunIndex, length);
		}
	}
#endregion
#region CommentStartBoundary
	public class CommentStartBoundary : VisitableDocumentIntervalBoundary {
		public CommentStartBoundary(Comment comment)
			: base(comment) {
		}
		public override DocumentModelPosition Position { get { return VisitableInterval.Interval.Start; } }
		public override BookmarkBoundaryOrder Order { get { return BookmarkBoundaryOrder.Start; } }
		public override void Export(DocumentModelExporter exporter) {
			exporter.ExportCommentStart((Comment)VisitableInterval);
		}
		protected internal override VisitableDocumentIntervalBox CreateBox() {
			return new CommentStartBox();
		}
	}
#endregion
#region CommentEndBoundary
	public class CommentEndBoundary : VisitableDocumentIntervalBoundary {
		public CommentEndBoundary(Comment comment)
			: base(comment) {
		}
		public override DocumentModelPosition Position { get { return VisitableInterval.Interval.End; } }
		public override BookmarkBoundaryOrder Order { get { return BookmarkBoundaryOrder.End; } }
		public override void Export(DocumentModelExporter exporter) {
			exporter.ExportCommentEnd((Comment)VisitableInterval);
		}
		protected internal override VisitableDocumentIntervalBox CreateBox() {
			return new CommentEndBox();
		}
	}
#endregion
#region VisitableDocumentIntervalBoundaryCollection
	public class VisitableDocumentIntervalBoundaryCollection : List<VisitableDocumentIntervalBoundary> {
	}
#endregion
#region VisitableDocumentIntervalBoundaryComparer
	public class VisitableDocumentIntervalBoundaryComparer : IComparer<VisitableDocumentIntervalBoundary> {
#region IComparer<BookmarkBoundary> Members
		public int Compare(VisitableDocumentIntervalBoundary x, VisitableDocumentIntervalBoundary y) {
			int result = x.Position.LogPosition - y.Position.LogPosition;
			if (result != 0)
				return result;
			else {
				result = x.IntervalIndex - y.IntervalIndex;
				if (result != 0)
					return result;
				else
					return x.Order - y.Order;
			}
		}
#endregion
	}
#endregion
#region VisitableDocumentIntervalStartBoundaryFactory
	public class VisitableDocumentIntervalStartBoundaryFactory : IDocumentIntervalVisitor {
		VisitableDocumentIntervalBoundary boundary;
		public VisitableDocumentIntervalBoundary Boundary { get { return boundary; } }
		public void Visit(Bookmark bookmark) {
			boundary = new BookmarkStartBoundary(bookmark);
		}
		public void Visit(RangePermission rangePermission) {
			boundary = new RangePermissionStartBoundary(rangePermission);
		}
		public void Visit(Comment comment) {
			boundary = new CommentStartBoundary(comment);
		}
		protected void SetBoundary(VisitableDocumentIntervalBoundary boundary) {
			this.boundary = boundary;
		}
	}										 
#endregion
#region VisitableDocumentIntervalEndBoundaryFactory
	public class VisitableDocumentIntervalEndBoundaryFactory : IDocumentIntervalVisitor {
		VisitableDocumentIntervalBoundary boundary;
		public VisitableDocumentIntervalBoundary Boundary { get { return boundary; } }
		public void Visit(Bookmark bookmark) {
			boundary = new BookmarkEndBoundary(bookmark);
		}
		public void Visit(RangePermission rangePermission) {
			boundary = new RangePermissionEndBoundary(rangePermission);
		}
		public void Visit(Comment comment) {
			boundary = new CommentEndBoundary(comment);
		}
		protected void SetBoundary(VisitableDocumentIntervalBoundary boundary) {
			this.boundary = boundary;
		}
	}
#endregion
#region VisitableDocumentIntervalBoundaryIterator
	public class VisitableDocumentIntervalBoundaryIterator {
		int currentIndex;
		readonly PieceTable pieceTable;
		readonly VisitableDocumentIntervalBoundaryCollection visitableDocumentIntervalBoundaries;
		readonly bool includeHiddenIntervals;
		public VisitableDocumentIntervalBoundaryIterator(PieceTable pieceTable, bool includeHiddenIntervals) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.includeHiddenIntervals = includeHiddenIntervals;
			this.visitableDocumentIntervalBoundaries = new VisitableDocumentIntervalBoundaryCollection();
			InitializeBoundaries();
		}
		public VisitableDocumentIntervalBoundaryIterator(PieceTable pieceTable)
			: this(pieceTable, true) {
		}
		public PieceTable PieceTable { get { return pieceTable; } }
		public VisitableDocumentIntervalBoundary Current { get { return visitableDocumentIntervalBoundaries[currentIndex]; } }
		protected internal VisitableDocumentIntervalBoundaryCollection Boundaries { get { return visitableDocumentIntervalBoundaries; } }
		public virtual bool IsDone() {
			return currentIndex >= visitableDocumentIntervalBoundaries.Count;
		}
		public virtual void MoveNext() {
			currentIndex++;
		}
		public virtual void Reset() {
			currentIndex = 0;
		}
		protected internal virtual void InitializeBoundaries() {
			PopulateBoundaries();
			visitableDocumentIntervalBoundaries.Sort(new VisitableDocumentIntervalBoundaryComparer());
		}
		protected internal virtual void PopulateBoundaries() {
			visitableDocumentIntervalBoundaries.Clear();
			PopulateBoundariesCore();
		}
		protected internal virtual void PopulateBoundariesCore() {
			PopulateBoundariesCore(PieceTable.GetBookmarks(includeHiddenIntervals));
		}
		protected internal virtual void PopulateBoundariesCore<T>(List<T> intervals) where T : VisitableDocumentInterval {
			VisitableDocumentIntervalStartBoundaryFactory startBoundaryFactory = CreateVisitableDocumentIntervalStartBoundaryFactory();
			VisitableDocumentIntervalEndBoundaryFactory endBoundaryFactory = CreateVisitableDocumentIntervalEndBoundaryFactory();
			int count = intervals.Count;
			for (int i = 0; i < count; i++) {
				VisitableDocumentInterval interval = intervals[i];
				if (!IsVisibleInterval(interval))
					continue;
				interval.Visit(startBoundaryFactory);
				VisitableDocumentIntervalBoundary startBoundary = startBoundaryFactory.Boundary;
				startBoundary.IntervalIndex = i;
				visitableDocumentIntervalBoundaries.Add(startBoundary);
				interval.Visit(endBoundaryFactory);
				VisitableDocumentIntervalBoundary endBoundary = endBoundaryFactory.Boundary;
				endBoundary.IntervalIndex = i;
				visitableDocumentIntervalBoundaries.Add(endBoundary);
			}
		}
		protected internal virtual VisitableDocumentIntervalStartBoundaryFactory CreateVisitableDocumentIntervalStartBoundaryFactory() {
			return new VisitableDocumentIntervalStartBoundaryFactory();
		}
		protected internal virtual VisitableDocumentIntervalEndBoundaryFactory CreateVisitableDocumentIntervalEndBoundaryFactory() {
			return new VisitableDocumentIntervalEndBoundaryFactory();
		}
		protected internal virtual bool IsVisibleInterval(VisitableDocumentInterval interval) {
			return true;
		}
	}
#endregion
#region VisitableDocumentIntervalBasedObjectBoundaryIterator
	public class VisitableDocumentIntervalBasedObjectBoundaryIterator : VisitableDocumentIntervalBoundaryIterator {
		public VisitableDocumentIntervalBasedObjectBoundaryIterator(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal override void PopulateBoundariesCore() {
			base.PopulateBoundariesCore();
			PopulateBoundariesCore(PieceTable.RangePermissions.InnerList);
			PopulateBoundariesCore(PieceTable.Comments.InnerList);
		}
	}
#endregion
#region RangePermissionBoundaryIterator
	public class RangePermissionBoundaryIterator : VisitableDocumentIntervalBoundaryIterator {
		public RangePermissionBoundaryIterator(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal override void PopulateBoundariesCore() {
			PopulateBoundariesCore(PieceTable.RangePermissions.InnerList);
		}
		protected internal override bool IsVisibleInterval(VisitableDocumentInterval interval) {
			RangePermission rangePermission = interval as RangePermission;
			if (rangePermission == null)
				return false;
			if (!PieceTable.DocumentModel.ProtectionProperties.EnforceProtection)
				return true;
			return PieceTable.IsPermissionGranted(rangePermission);
		}
	}
#endregion
#region CommentBoundaryIterator
	public class CommentBoundaryIterator : VisitableDocumentIntervalBoundaryIterator {
		public CommentBoundaryIterator(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal override void PopulateBoundariesCore() {
			PopulateBoundariesCore(PieceTable.Comments.InnerList);
		}
		protected internal override bool IsVisibleInterval(VisitableDocumentInterval interval) {
			Comment comment = interval as Comment;
			if ((comment == null) || !PieceTable.DocumentModel.CommentOptions.VisibleAuthors.Contains(comment.Author) || 
				((PieceTable.DocumentModel.CommentOptions.Visibility != RichEditCommentVisibility.Visible) && !PieceTable.DocumentModel.CommentOptions.HighlightCommentedRange))
				return false;
			return true;
		}
	}
#endregion
}
namespace DevExpress.XtraRichEdit.Internal {
#region TableInfo
	public class TableInfo {
#region Fields
		const int RootTableNestingLevel = 0;
		readonly Table table;
		int nestedLevel = -1;
		VerticalMergeCellsCollection verticalMergedCellsInTable;
#endregion
		public TableInfo(Table table) {
			Guard.ArgumentNotNull(table, "table");
			this.table = table;
			this.nestedLevel = GetNestingLevel(table);
			this.verticalMergedCellsInTable = GetVerticalMergedCells();
		}
#region Properties
		public Table Table { get { return table; } }
		private DocumentModel DocumentModel { get { return table.DocumentModel; } }
		public int NestedLevel { get { return nestedLevel; } }
#endregion
		public int GetNestingLevel(Table table) {
			int nestingLevel = 0;
			TableCell parentCell = table.ParentCell;
			while (parentCell != null) {
				Debug.Assert(parentCell.Row != null);
				Debug.Assert(parentCell.Row.Table != null);
				Table parentTable = parentCell.Row.Table;
				parentCell = parentTable.ParentCell;
				nestingLevel++;
			}
			return nestingLevel;
		}
		protected internal virtual List<TableGridInterval> GetTableGridIntervals() {
			SimpleTableWidthsCalculator widthsCalculator = new SimpleTableWidthsCalculator(DocumentModel.ToDocumentLayoutUnitConverter);
			TableGridCalculator calculator = new TableGridCalculator(DocumentModel, widthsCalculator, Int32.MaxValue);
			return calculator.CalculateGridIntervals(Table);
		}
		protected internal virtual TableGrid GetTableGrid(int parentWidth) {
			SimpleTableWidthsCalculator widthsCalculator = new SimpleTableWidthsCalculator(DocumentModel.ToDocumentLayoutUnitConverter, parentWidth);
			TableGridCalculator calculator = new TableGridCalculator(DocumentModel, widthsCalculator, Int32.MaxValue);
			return calculator.CalculateTableGrid(table, parentWidth);
		}
		protected internal virtual int GetParentSectionWidth() {
			DocumentModelPosition pos = DocumentModelPosition.FromParagraphStart(table.PieceTable, table.FirstRow.FirstCell.StartParagraphIndex);
			SectionIndex sectionIndex = table.DocumentModel.FindSectionIndex(pos.LogPosition);
			Section section = DocumentModel.Sections[sectionIndex];
			SectionMargins sectionMargins = section.Margins;
			int margins = sectionMargins.Left + sectionMargins.Right;
			return section.Page.Width - margins;
		}
		public Table GetRootTable() {
			return GetParentTable(RootTableNestingLevel);
		}
		public Table GetParentTable(int nestingLevel) {
			if (nestingLevel == NestedLevel)
				return this.table;
			Table currentTable = this.table;
			int currentTableNestingLevel = NestedLevel;
			while (currentTableNestingLevel > nestingLevel) {
				TableCell parentCell = currentTable.ParentCell;
				Debug.Assert(parentCell != null);
				currentTable = parentCell.Row.Table;
				currentTableNestingLevel--;
			}
			return currentTable;
		}
		protected internal int GetCellRowSpan(TableCell cell) {
			return verticalMergedCellsInTable.GetRowSpan(cell);
		}
		public VerticalMergeCellProperties GetMergedCellProperties(TableCell cell) {
			return verticalMergedCellsInTable.GetMergedCellProperties(cell);
		}
		internal VerticalMergeCellsCollection GetVerticalMergedCells() {
			VerticalMergeCellsCollection result = new VerticalMergeCellsCollection();
			TableRowCollection rows = table.Rows;
			for (int i = 0; i < rows.Count; i++) {
				TableRow currentRow = rows[i];
				for (int j = 0; j < currentRow.Cells.Count; j++) {
					TableCell currentCell = currentRow.Cells[j];
					switch (currentCell.VerticalMerging) {
						case MergingState.Restart:
							VerticalMergeCellProperties mergeCellProperties = CalculateMergeCellProperties(currentCell);
							result.Add(currentCell, mergeCellProperties);
							break;
						case MergingState.Continue:
							result.Add(currentCell, new VerticalMergeCellProperties(0, null));
							break;
					}
				}
			}
			return result;
		}
		internal VerticalMergeCellProperties CalculateMergeCellProperties(TableCell currentCell) {
			int columnIndex = TableCellVerticalBorderCalculator.GetStartColumnIndex(currentCell, false);
			List<TableCell> verticalMergedCells = TableCellVerticalBorderCalculator.GetVerticalSpanCells(currentCell, columnIndex, false);
			int verticalMergeCellsCount = verticalMergedCells.Count;
			BorderBase actualBottomCellBorder = verticalMergedCells[verticalMergeCellsCount - 1].GetActualBottomCellBorder();
			return new VerticalMergeCellProperties(verticalMergeCellsCount, actualBottomCellBorder);
		}
	}
#endregion
}
