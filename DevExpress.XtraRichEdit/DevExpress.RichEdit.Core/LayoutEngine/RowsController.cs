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
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	public enum CanFitCurrentRowToColumnResult {
		RowFitted,
		PlainRowNotFitted,
		FirstCellRowNotFitted,
		TextAreasRecreated, 
		RestartDueFloatingObject, 
	}
	public enum CompleteFormattingResult {
		Success,
		OrphanedFloatingObjects
	}
	#region RowsController
	public class RowsController {
		#region Fields
		Row currentRow;
		int paragraphLeft;
		int paragraphRight;
		int currentRowIndent;
		int regularRowIndent;
		Column currentColumn;
		IColumnController columnController;
		int startWordBoxIndex;
		int heightBeforeWord;
		Paragraph paragraph;
		LineSpacingCalculatorBase lineSpacingCalculator;
		int defaultRowHeight;
		int maxAscentAndFree;
		int maxPictureHeight;
		int maxDescent;
		int maxAscentBeforeWord;
		int maxDescentBeforeWord;
		int maxPictureHeightBeforeWord;
		RunIndex runIndexBeforeWord;
		RunIndex currentRunIndex;
		RowProcessingFlags rowProcessingFlagsBeforeWord;
		readonly PieceTable pieceTable;
		bool suppressHorizontalOverfull;
		readonly TablesController tablesController;
		readonly TabsController tabsController;
		Rectangle currentParagraphNumberingListBounds;
		int initialLineNumber;
		int lineNumber;
		int lineNumberStep = 1;
		int spacingAfterPrevParagraph;
		bool isSpacingAfterPrevParagraphValid;
		bool supportsColumnAndPageBreaks = true;
		FloatingObjectsLayout floatingObjectsLayout;
		ParagraphFramesLayout paragraphFramesLayout;
		CurrentHorizontalPositionController horizontalPositionController;
		bool invisibleEmptyParagraphInCellAfterNestedTable;
		DocumentModelPosition restartModelPosition;
		bool matchHorizontalTableIndentsToTextEdge;
		ParagraphIndex frameParagraphIndex;
		bool forceFormattingComplete = false;
		#endregion
		public RowsController(PieceTable pieceTable, ColumnController columnController, bool matchHorizontalTableIndentsToTextEdge) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			Guard.ArgumentNotNull(columnController, "columnController");
			this.columnController = columnController;
			this.tabsController = new TabsController();
			this.tablesController = CreateTablesController();
			this.pieceTable = pieceTable;
			this.currentRunIndex = RunIndex.DontCare;
			this.floatingObjectsLayout = columnController.PageAreaController.PageController.FloatingObjectsLayout;
			Debug.Assert(floatingObjectsLayout != null);
			this.paragraphFramesLayout = columnController.PageAreaController.PageController.ParagraphFramesLayout;
			Debug.Assert(paragraphFramesLayout != null);
			this.horizontalPositionController = CreateCurrentHorizontalPosition();
			this.matchHorizontalTableIndentsToTextEdge = matchHorizontalTableIndentsToTextEdge;
			this.frameParagraphIndex = new ParagraphIndex(-1);
		}
		#region Properties
		internal bool MatchHorizontalTableIndentsToTextEdge { get { return matchHorizontalTableIndentsToTextEdge; } set { matchHorizontalTableIndentsToTextEdge = value; } }
		internal IColumnController ColumnController { get { return columnController; } }
		internal Column CurrentColumn { get { return currentColumn; } set { currentColumn = value; } }
		internal Row CurrentRow { get { return currentRow; } set { currentRow = value; } }
		internal int CurrentHorizontalPosition { get { return horizontalPositionController.CurrentHorizontalPosition; } }
		internal CurrentHorizontalPositionController HorizontalPositionController { get { return horizontalPositionController; } set { horizontalPositionController = value; } }
		internal int CurrentRowIndent { get { return currentRowIndent; } set { currentRowIndent = value; } }
		protected int RegularRowIndent { get { return regularRowIndent; } set { regularRowIndent = value; } }
		internal int DefaultRowHeight { get { return defaultRowHeight; } }
		internal int ParagraphLeft { get { return paragraphLeft; } set { paragraphLeft = value; } }
		internal bool SuppressHorizontalOverfull { get { return suppressHorizontalOverfull; } set { suppressHorizontalOverfull = value; } }
		internal TablesController TablesController { get { return tablesController; } }
		internal FloatingObjectsLayout FloatingObjectsLayout { get { return floatingObjectsLayout; } }
		internal ParagraphFramesLayout ParagraphFramesLayout { get { return paragraphFramesLayout; } }
		internal Paragraph Paragraph { get { return paragraph; } }
		internal DocumentModelPosition RestartModelPosition { get { return restartModelPosition; } set { restartModelPosition = value; } }
		internal DocumentModelPosition LastRestartDueToFloatingObjectModelPosition { get; set; }
		internal Row ParagraphFirstRowOnPage { get; set; }
		internal Column ParagraphFirstColumnOnPage { get; set; }
		public int InitialLineNumber {
			get { return initialLineNumber; }
			set {
				initialLineNumber = value;
				LineNumber = value;
			}
		}
		public int LineNumber { get { return lineNumber; } set { lineNumber = value; } }
		public int LineNumberStep { get { return lineNumberStep; } set { lineNumberStep = Math.Max(1, value); } }
		public PieceTable PieceTable { get { return pieceTable; } }
		public DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		public virtual bool SupportsColumnAndPageBreaks { get { return supportsColumnAndPageBreaks; } set { supportsColumnAndPageBreaks = value; } }
		public ParagraphIndex FrameParagraphIndex { get { return frameParagraphIndex; } set { frameParagraphIndex = value; } }
		internal bool ForceFormattingComplete { get { return forceFormattingComplete; } }
		#endregion
		#region Events
		#region BeginNextSectionFormatting
		EventHandler onBeginNextSectionFormatting;
		public event EventHandler BeginNextSectionFormatting { add { onBeginNextSectionFormatting += value; } remove { onBeginNextSectionFormatting -= value; } }
		protected internal virtual void RaiseBeginNextSectionFormatting() {
			if (onBeginNextSectionFormatting != null)
				onBeginNextSectionFormatting(this, EventArgs.Empty);
		}
		#endregion
		#region TableStarted
		EventHandler onTableStarted;
		protected virtual void RaiseTableStarted() {
			if (onTableStarted != null)
				onTableStarted(this, EventArgs.Empty);
		}
		public event EventHandler TableStarted { add { onTableStarted += value; } remove { onTableStarted -= value; } }
		#endregion
		#endregion
		protected virtual CurrentHorizontalPositionController CreateCurrentHorizontalPosition() {
			return new CurrentHorizontalPositionController(this);
		}
		protected virtual CurrentHorizontalPositionController CreateCurrentHorizontalPosition(int position) {
			return new CurrentHorizontalPositionController(this, position);
		}
		public void OnFloatingObjectsLayoutChanged() {
			this.floatingObjectsLayout = columnController.PageAreaController.PageController.FloatingObjectsLayout;
			Debug.Assert(floatingObjectsLayout != null);
		}
		public virtual void Reset(Section section, bool keepFloatingObjects) {
			this.columnController = GetTopLevelColumnController();
			InitialLineNumber = section.LineNumbering.StartingLineNumber;
			LineNumberStep = section.LineNumbering.Step;
			BeginSectionFormatting(section); 
			CurrentColumn = ColumnController.GetNextColumn(null, keepFloatingObjects);
			CurrentRow = ColumnController.CreateRow();
			CurrentRow.Bounds = new Rectangle(0, CurrentColumn.Bounds.Top, 0, 0);
			RecreateHorizontalPositionController();
			HorizontalPositionController.OnCurrentRowHeightChanged(false);
			paragraph = PieceTable.Paragraphs.First;
			AssignCurrentRowLineNumber();
			TablesController.Reset();
		}
		public void SetColumnController(IColumnController newColumnController) {
			if (CurrentRow.Boxes.Count > 0)
				Exceptions.ThrowInternalException();
			this.columnController = newColumnController;
			Row newRow = ColumnController.CreateRow();
			newRow.Bounds = CurrentRow.Bounds;
			newRow.LineNumber = CurrentRow.LineNumber;
			CurrentRow = newRow;
		}
		void ClearInvalidatedContentCore(Column column, FormatterPosition pos, ParagraphIndex paragraphIndex) {
			column.ClearInvalidatedParagraphFrames(paragraphIndex);
			RowCollection rows = column.Rows;
			int rowIndex = rows.BinarySearchBoxIndex(pos);
			if (rowIndex < 0) {
				rowIndex = ~rowIndex;
			}
			if (rowIndex < rows.Count)
				rows.RemoveRange(rowIndex, rows.Count - rowIndex);
			paragraph = PieceTable.Paragraphs[paragraphIndex];
			ApplyColumnBounds();
		}
		public virtual void ClearInvalidatedContentFromTheStartOfRowAtCurrentPage(Column column, FormatterPosition pos, ParagraphIndex paragraphIndex) {
			ClearInvalidatedContentCore(column, pos, paragraphIndex);
			RecreateHorizontalPositionController();
		}
		public virtual void ClearInvalidatedContent(Column column, FormatterPosition pos, ParagraphIndex paragraphIndex) {
			ClearInvalidatedContentCore(column, pos, paragraphIndex);
			TablesController.ClearInvalidatedContent();
			this.columnController = GetTopLevelColumnController();
			RecreateHorizontalPositionController();
		}
		protected internal virtual void RecreateHorizontalPositionController() {
			int position = horizontalPositionController.CurrentHorizontalPosition;
			if (floatingObjectsLayout.Items.Count > 0 || paragraphFramesLayout.Items.Count > 0) {
				if (horizontalPositionController.GetType() != typeof(FloatingObjectsCurrentHorizontalPositionController)) {
					this.horizontalPositionController = new FloatingObjectsCurrentHorizontalPositionController(this, position);
				}
			}
			else {
				if (horizontalPositionController.GetType() != typeof(CurrentHorizontalPositionController)) {
					this.horizontalPositionController = CreateCurrentHorizontalPosition(position);
				}
			}
			this.horizontalPositionController.OnCurrentRowHeightChanged(false);
		}
		IColumnController GetTopLevelColumnController() {
			IColumnController currentController = ColumnController;
			for (; ; ) {
				TableCellColumnController tableCellColumnController = currentController as TableCellColumnController;
				if (tableCellColumnController != null)
					currentController = tableCellColumnController.Parent;
				else
					return currentController;
			}
		}
		protected internal virtual void BeginSectionFormatting(Section section) {
			ApplySectionStart(section, ColumnController.TopLevelColumnsCount);
			spacingAfterPrevParagraph = 0;
			isSpacingAfterPrevParagraphValid = true;
		}
		protected internal virtual void RestartFormattingFromTheStartOfSection(Section section, Column column) {
			CurrentColumn = column;
			CreateNewCurrentRowAfterRestart(section, column);
			ApplySectionStart(section, ColumnController.TopLevelColumnsCount);
			suppressLineNumberingRecalculationForLastPage = true;
			try {
				CompleteCurrentColumnFormatting();
				MoveRowToNextColumn();
			}
			finally {
				suppressLineNumberingRecalculationForLastPage = false;
			}
		}
		protected internal virtual void RestartFormattingFromTheMiddleOfSection(Section section, Column column) {
			isSpacingAfterPrevParagraphValid = false;
			CurrentColumn = column;
			CreateNewCurrentRowAfterRestart(section, column);
			RecreateHorizontalPositionController();
		}
		protected internal virtual void RestartFormattingFromTheStartOfRowAtCurrentPage(Section section, Column column) {
			isSpacingAfterPrevParagraphValid = false;
			if (!(CurrentColumn is TableCellColumn)) {
				if (column == null)
					column = ColumnController.GetNextColumn(column, true);
				CurrentColumn = column;
			}
			CreateNewCurrentRowAfterRestart(section, CurrentColumn);
		}
		protected internal virtual void CreateNewCurrentRowAfterRestart(Section section, Column column) {
			Row lastRow = column.GetOwnRows().Last;
			InitialLineNumber = section.LineNumbering.StartingLineNumber;
			if (lastRow == null) {
				Column prevColumn = ColumnController.GetPreviousColumn(column);
				if (prevColumn == null || prevColumn.IsEmpty)
					LineNumber = InitialLineNumber; 
				else
					LineNumber = Math.Abs(prevColumn.Rows.Last.LineNumber);
			}
			else
				LineNumber = Math.Abs(lastRow.LineNumber);
			ParagraphIndex paragraphIndex = paragraph.Index - 1;
			if (paragraphIndex >= ParagraphIndex.Zero) {
				if (!PieceTable.Paragraphs[paragraphIndex].SuppressLineNumbers && !TablesController.IsInsideTable)
					LineNumber++;
			}
			LineNumberStep = section.LineNumbering.Step;
			CreateNewCurrentRow(lastRow, true);
		}
		void AssignCurrentRowLineNumber() {
			if (!paragraph.SuppressLineNumbers && !TablesController.IsInsideTable && (LineNumber % LineNumberStep) == 0)
				CurrentRow.LineNumber = LineNumber;
			else
				CurrentRow.LineNumber = -LineNumber;
		}
		protected internal virtual TablesController CreateTablesController() {
			return new TablesController(ColumnController.PageAreaController.PageController, this);
		}
		public virtual void StartNewWord() {
			startWordBoxIndex = CurrentRow.Boxes.Count;
			heightBeforeWord = CurrentRow.Height;
			maxAscentBeforeWord = maxAscentAndFree;
			maxPictureHeightBeforeWord = maxPictureHeight;
			maxDescentBeforeWord = maxDescent;
			runIndexBeforeWord = currentRunIndex;
			rowProcessingFlagsBeforeWord = CurrentRow.ProcessingFlags;
		}
		protected internal virtual void AddSingleDecimalTabInTable() {
			if (CurrentHorizontalPosition > paragraphRight)
				SuppressHorizontalOverfull = true;
			tabsController.SaveLastTabPosition(-1);
		}
		protected internal virtual void AddTabBox(TabInfo tab, BoxInfo boxInfo) {
			int newPosition;
			if (tab.Alignment == TabAlignmentType.Left) {
				int tabPosition = tab.GetLayoutPosition(DocumentModel.ToDocumentLayoutUnitConverter);
				newPosition = tabPosition + CurrentColumn.Bounds.Left;
				boxInfo.Size = new Size(newPosition - CurrentHorizontalPosition, boxInfo.Size.Height);
			}
			else
				newPosition = CurrentHorizontalPosition;
			if (newPosition > paragraphRight && !tab.IsDefault)
				SuppressHorizontalOverfull = true;
			tabsController.SaveLastTabPosition(CurrentRow.Boxes.Count);
			TabSpaceBox box = (TabSpaceBox)AddBox(ParagraphBoxFormatter.tabSpaceBox, boxInfo);
			box.TabInfo = tab;
			box.LeaderCount = TabsController.CalculateLeaderCount(box, PieceTable);
		}
		void ExpandLastTab(ParagraphBoxFormatter formatter) {
			int offset = tabsController.CalcLastTabWidth(CurrentRow, formatter.Measurer);
#if DEBUGTEST
			Debug.Assert(offset >= 0);
#endif
			HorizontalPositionController.IncrementCurrentHorizontalPosition(offset);
		}
		protected internal virtual Box AddBox(Box boxType, BoxInfo boxInfo) {
			Box box = GetBox(boxType, boxInfo);
			box.Bounds = HorizontalPositionController.CalculateBoxBounds(boxInfo);
			ISpaceBox spaceBox = box as ISpaceBox;
			if (spaceBox != null)
				spaceBox.MinWidth = boxInfo.Size.Width;
			CurrentRow.Boxes.Add(box);
			HorizontalPositionController.IncrementCurrentHorizontalPosition(boxInfo.Size.Width);
			RunIndex runIndex = boxInfo.StartPos.RunIndex;
			TextRunBase run = PieceTable.Runs[runIndex];
			CurrentRow.ProcessingFlags |= run.RowProcessingFlags;
			return box;
		}
		protected internal virtual void AddNumberingListBox(NumberingListBox box, ListLevelProperties properties, ParagraphBoxFormatter formatter) {
			currentParagraphNumberingListBounds = box.Bounds;
			Rectangle bounds = CalculateNumberingListBoxBounds(box, properties, formatter);
			box.Bounds = bounds;
			HorizontalPositionController.SetCurrentHorizontalPosition(bounds.Right);
		}
		Rectangle CalculateNumberingListBoxBounds(NumberingListBox box, ListLevelProperties properties, ParagraphBoxFormatter formatter) {
			int left = CurrentRow.Bounds.Left;
			Rectangle result = CalculateNumberingListBoxBoundsCore(box, properties, formatter);
			CurrentRow.TextOffset = result.Right - left;
			FloatingObjectsCurrentHorizontalPositionController controller = HorizontalPositionController as FloatingObjectsCurrentHorizontalPositionController;
			if (controller == null)
				return result;
			if (CanFitNumberingListBoxToCurrentRow(result.Size))
				return result;
			while (controller.AdvanceHorizontalPositionToNextTextArea()) {
				left = controller.CurrentHorizontalPosition;
				result = CalculateNumberingListBoxBoundsCore(box, properties, formatter);
				if (CanFitNumberingListBoxToCurrentRow(result.Size))
					break;
			}
			CurrentRow.TextOffset = result.Right - left;
			return result;
		}
		Rectangle CalculateNumberingListBoxBoundsCore(NumberingListBox box, ListLevelProperties properties, ParagraphBoxFormatter formatter) {
			Rectangle bounds = box.Bounds;
			ListNumberAlignment alignment = properties.Alignment;
			int numberingListBoxLeft = CalculateNumberingListBoxLeft(bounds.Size.Width, alignment);
			bounds = new Rectangle(new Point(numberingListBoxLeft, 0), bounds.Size);
			CurrentRow.NumberingListBox = box;
			NumberingListBoxWithSeparator boxWithSeparator = box as NumberingListBoxWithSeparator;
			if (boxWithSeparator != null) {
				if (boxWithSeparator.SeparatorBox is ISpaceBox) {
					bounds.Width += boxWithSeparator.SeparatorBox.Bounds.Width;
				}
				else {
					Debug.Assert(boxWithSeparator.SeparatorBox is TabSpaceBox);
					int separatorWidth = properties.Legacy ? GetLegacyNumberingListBoxSeparatorWidth(boxWithSeparator, bounds, properties, formatter) : GetNumberingListBoxSeparatorWidth(boxWithSeparator, bounds, properties, formatter);
					Rectangle separatorBounds = boxWithSeparator.SeparatorBox.Bounds;
					separatorBounds.Width = separatorWidth;
					separatorBounds.X = bounds.Right;
					bounds.Width += separatorWidth;
					boxWithSeparator.SeparatorBox.Bounds = separatorBounds;
				}
			}
			return bounds;
		}
		int GetNumberingListBoxSeparatorWidth(NumberingListBoxWithSeparator box, Rectangle bounds, ListLevelProperties properties, ParagraphBoxFormatter formatter) {
			HorizontalPositionController.SetCurrentHorizontalPosition(bounds.Right);
			TabInfo tabInfo = GetNextTab(formatter);
			TabInfo actualTabInfo = GetActualTabInfo(tabInfo, properties);
			int tabRight = actualTabInfo.GetLayoutPosition(DocumentModel.ToDocumentLayoutUnitConverter);
			int tabLeft = bounds.Right - CurrentColumn.Bounds.Left;
			int separatorWidth = tabRight - tabLeft;
			if (separatorWidth < 0) {
				tabRight = tabInfo.GetLayoutPosition(DocumentModel.ToDocumentLayoutUnitConverter);
				separatorWidth = tabRight - tabLeft;
			}
			return separatorWidth;
		}
		int GetLegacyNumberingListBoxSeparatorWidth(NumberingListBoxWithSeparator box, Rectangle bounds, ListLevelProperties properties, ParagraphBoxFormatter formatter) {
			int startHorizontalPosition = CurrentHorizontalPosition;
			HorizontalPositionController.SetCurrentHorizontalPosition(bounds.Right);
			int legacySpace = DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(properties.LegacySpace);
			int legacyIndent = DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(properties.LegacyIndent);
			int textRight = Math.Max(startHorizontalPosition + legacyIndent, bounds.Right + legacySpace);
			if (paragraph.FirstLineIndentType == ParagraphFirstLineIndent.Hanging) {
				textRight = Math.Max(textRight, DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(paragraph.FirstLineIndent) + startHorizontalPosition);
			}
			return Math.Max(textRight - CurrentHorizontalPosition, 0);
		}
		TabInfo GetActualTabInfo(TabInfo tabInfo, ListLevelProperties properties) {
			if (paragraph.FirstLineIndentType == ParagraphFirstLineIndent.Hanging) {
				TabInfo indentTabInfo = new TabInfo(paragraph.LeftIndent);
				int indentTabPosition = indentTabInfo.GetLayoutPosition(DocumentModel.ToDocumentLayoutUnitConverter);
				int tabPosition = tabInfo.GetLayoutPosition(DocumentModel.ToDocumentLayoutUnitConverter);
				if (tabInfo.IsDefault || tabPosition > indentTabPosition)
					return indentTabInfo;
				else
					return tabInfo;
			}
			else
				return tabInfo;
		}
		int CalculateNumberingListBoxLeft(int boxWidth, ListNumberAlignment alignment) {
			int boundsLeft = CurrentHorizontalPosition;
			switch (alignment) {
				case ListNumberAlignment.Left:
					return boundsLeft;
				case ListNumberAlignment.Center:
					return boundsLeft - boxWidth / 2;
				case ListNumberAlignment.Right:
					return boundsLeft - boxWidth;
				default:
					Exceptions.ThrowInternalException();
					return 0;
			}
		}
		protected internal virtual bool CanAddSectionBreakToPrevRow() {
			return CurrentColumn.Rows.Last != null;
		}
		protected internal virtual void AddSectionBreakBoxToPrevRow(Box boxType, BoxInfo boxInfo) {
			Row lastRow = CurrentColumn.Rows.Last;
			Box box = GetBox(boxType, boxInfo);
			box.Bounds = new Rectangle(new Point(lastRow.Boxes.Last.Bounds.Right, 0), boxInfo.Size);
			lastRow.Boxes.Add(box);
		}
		protected internal virtual Box GetBox(Box boxType, BoxInfo boxInfo) {
			Box box;
			if (boxInfo.Box != null && !(boxInfo.Box is FloatingObjectAnchorBox) && !(boxInfo.Box is ParagraphFrameBox)) {
				box = boxInfo.Box;
				Debug.Assert(box.StartPos == boxInfo.StartPos);
				Debug.Assert(box.EndPos == boxInfo.EndPos);
			}
			else {
				box = boxType.CreateBox();
				box.StartPos = boxInfo.StartPos;
				box.EndPos = boxInfo.EndPos;
			}
			return box;
		}
		internal void UpdateCurrentRowHeight(BoxInfo boxInfo) {
			RunIndex runIndex = boxInfo.StartPos.RunIndex;
			if (runIndex == currentRunIndex && !boxInfo.ForceUpdateCurrentRowHeight)
				return;
			if (!boxInfo.ForceUpdateCurrentRowHeight)
				currentRunIndex = runIndex;
			UpdateMaxAscentAndDescent(boxInfo);
			int prevRowHeight = CurrentRow.Height;
			CurrentRow.Height = lineSpacingCalculator.CalcRowHeight(CurrentRow.Height, maxAscentAndFree + maxDescent);
			if (prevRowHeight != CurrentRow.Height)
				HorizontalPositionController.OnCurrentRowHeightChanged(false);
			TablesController.UpdateCurrentCellHeight();
		}
		internal void UpdateCurrentRowHeightFromInlineObjectRun(TextRunBase run, Box box, IObjectMeasurer measurer) {
			BoxInfo boxInfo = new BoxInfo();
			run.Measure(boxInfo, measurer);
			this.maxPictureHeight = Math.Max(maxPictureHeight, boxInfo.Size.Height);
			CurrentRow.Height = lineSpacingCalculator.CalcRowHeightFromInlineObject(CurrentRow.Height, maxPictureHeight, maxDescent);
			TablesController.UpdateCurrentCellHeight();
		}
		internal void UpdateCurrentRowHeightFromFloatingObject(FloatingObjectAnchorRun run, IObjectMeasurer measurer) {
			this.maxAscentAndFree = Math.Max(maxAscentAndFree, GetFontAscentAndFree(run));
			int pictureHeight = Math.Max(maxAscentAndFree, measurer.MeasureFloatingObject(run).Height);
			CurrentRow.Height = lineSpacingCalculator.CalcRowHeight(CurrentRow.Height, pictureHeight);
			TablesController.UpdateCurrentCellHeight();
		}
		internal CanFitCurrentRowToColumnResult CanFitCurrentRowToColumn() {
			int rowHeight = lineSpacingCalculator.CalcRowHeight(CurrentRow.Height, CurrentRow.Height);
			return CanFitCurrentRowToColumnCore(rowHeight);
		}
		internal int MoveCurrentRowDownToFitTable(int tableWidth, int tableTop) {
			return HorizontalPositionController.MoveCurrentRowDownToFitTable(tableWidth, tableTop);
		}
		public TextArea GetTextAreaForTable() {
			return HorizontalPositionController.GetTextAreaForTable();
		}
		public CanFitCurrentRowToColumnResult CanFitCurrentRowToColumn(BoxInfo newBoxInfo) {
			bool emptyColumn = CurrentColumn.Rows.Count <= 0;
			if (newBoxInfo.StartPos.RunIndex > ColumnController.PageLastRunIndex && !emptyColumn)
				return CanFitCurrentRowToColumnResult.PlainRowNotFitted;
			if (newBoxInfo.Box == null) {
				int boxIndex = newBoxInfo.StartPos.BoxIndex;
				if (boxIndex < Paragraph.BoxCollection.Count) {
					Box box = Paragraph.BoxCollection[boxIndex];
					if (box is FloatingObjectAnchorBox)
						newBoxInfo.Box = box;
				}
			}
			FloatingObjectAnchorBox floatingObjectAnchorBox = newBoxInfo.Box as FloatingObjectAnchorBox;
			if (floatingObjectAnchorBox != null) {
				if (!CanFitFloatingObject(floatingObjectAnchorBox, newBoxInfo, emptyColumn))
					return CanFitCurrentRowToColumnResult.RestartDueFloatingObject;
				else
					return CanFitCurrentRowToColumnResult.RowFitted;
			}
			ParagraphFrameBox paragraphFrameBox = newBoxInfo.Box as ParagraphFrameBox;
			if (this.Paragraph.FrameProperties != null && paragraphFrameBox != null) {
				if (!CanFitParagraphFrame(newBoxInfo, emptyColumn))
					return CanFitCurrentRowToColumnResult.RestartDueFloatingObject;
				else
					return CanFitCurrentRowToColumnResult.RowFitted;
			}
			int rowHeight = CalcNewCurrentRowHeight(newBoxInfo);
			return CanFitCurrentRowToColumnCore(rowHeight);
		}
		public CanFitCurrentRowToColumnResult CanFitCurrentRowWithFrameToColumn(BoxInfo newBoxInfo) {
			bool emptyColumn = CurrentColumn.Rows.Count <= 0;
			if (newBoxInfo.StartPos.RunIndex > ColumnController.PageLastRunIndex && !emptyColumn)
				return CanFitCurrentRowToColumnResult.PlainRowNotFitted;
			ParagraphFrameBox paragraphFrameBox = newBoxInfo.Box as ParagraphFrameBox;
			if (this.Paragraph.GetMergedFrameProperties() != null && paragraphFrameBox != null) {
				if (!CanFitParagraphFrame(newBoxInfo, emptyColumn))
					return CanFitCurrentRowToColumnResult.RestartDueFloatingObject;
			}
			int rowHeight = CalcNewCurrentRowHeight(newBoxInfo);
			return CanFitCurrentRowToColumnCore(rowHeight);
		}
		public bool CanFitFloatingObject(FloatingObjectAnchorBox floatingObjectAnchorBox, BoxInfo newBoxInfo, bool emptyColumn) {
			TextRunBase currentRun = PieceTable.Runs[newBoxInfo.StartPos.RunIndex];
			FloatingObjectAnchorRun objectAnchorRun = (FloatingObjectAnchorRun)currentRun;
			FloatingObjectBox floatingObjectBox = null;
			if (this.FloatingObjectsLayout.ContainsRun(objectAnchorRun)) {
				floatingObjectBox = FloatingObjectsLayout.GetFloatingObject(objectAnchorRun);
				if (floatingObjectBox.LockPosition)
					return true;
				floatingObjectBox.WasRestart = false;
			}
			FloatingObjectSizeAndPositionController controller = CreateFloatingObjectSizeAndPositionController();
			controller.UpdateFloatingObjectBox(floatingObjectAnchorBox);
			currentRun = PieceTable.Runs[floatingObjectAnchorBox.StartPos.RunIndex];
			FloatingObjectAnchorRun run = (FloatingObjectAnchorRun)currentRun;
			if (run.ExcludeFromLayout)
				return true;
			if (run.FloatingObjectProperties.TextWrapType == FloatingObjectTextWrapType.None)
				return true;
			DocumentModelPosition restartPosition = CalculateRestartPositionDueFloatingObject(floatingObjectAnchorBox);
			if (restartPosition == null || emptyColumn)
				return true;
			if (restartPosition.RunIndex == floatingObjectAnchorBox.StartPos.RunIndex && restartPosition.RunOffset == floatingObjectAnchorBox.StartPos.Offset) {
				this.LastRestartDueToFloatingObjectModelPosition = restartPosition;
				return true;
			}
			bool objectAdded = AddFloatingObjectToLayout(floatingObjectAnchorBox, newBoxInfo);
			if (objectAdded && CanRestartDueFloatingObject()) {
				if (floatingObjectBox == null)
					floatingObjectBox = FloatingObjectsLayout.GetFloatingObject(objectAnchorRun);
				floatingObjectBox.WasRestart = true;
				this.RestartModelPosition = restartPosition;
				this.LastRestartDueToFloatingObjectModelPosition = restartPosition;
				return false;
			}
			return true;
		}
		public bool CanFitParagraphFrame(BoxInfo newBoxInfo, bool emptyColumn) {
			TextRunBase currentRun = PieceTable.Runs[newBoxInfo.StartPos.RunIndex];
			ParagraphFrameBox paragraphFrameBox = null;
			ParagraphFrameBox paragraphFrameBoxInfo = newBoxInfo.Box as ParagraphFrameBox;
			Paragraph currentParagraph = currentRun.Paragraph;
			if (this.ParagraphFramesLayout.ContainsParagraph(currentParagraph)) {
				paragraphFrameBox = ParagraphFramesLayout.GetFloatingObject(currentParagraph);
				if (paragraphFrameBox != null) {
					if (paragraphFrameBox.LockPosition)
						return true;
					paragraphFrameBox.WasRestart = false;
				}
			}
			FloatingObjectSizeAndPositionController controller = CreateFloatingObjectSizeAndPositionController();
			controller.UpdateParagraphFrameBox(paragraphFrameBoxInfo);
			return true;
		}
		protected virtual FloatingObjectSizeAndPositionController CreateFloatingObjectSizeAndPositionController() {
			return new FloatingObjectSizeAndPositionController(this);
		}
		protected internal virtual bool CanRestartDueFloatingObject() {
			return true;
		}
		protected internal virtual void ApplyFloatingObjectBoxProperties(FloatingObjectBox floatingObjectBox, FloatingObjectProperties floatingObjectProperties) {
			floatingObjectBox.CanPutTextAtLeft = floatingObjectProperties.CanPutTextAtLeft;
			floatingObjectBox.CanPutTextAtRight = floatingObjectProperties.CanPutTextAtRight;
			floatingObjectBox.PutTextAtLargestSide = floatingObjectProperties.PutTextAtLargestSide;
			floatingObjectBox.PieceTable = floatingObjectProperties.PieceTable;
		}
		protected internal virtual void ApplyFloatingObjectBoxBounds(FloatingObjectBox floatingObjectBox, FloatingObjectAnchorBox anchorBox) {
			floatingObjectBox.ExtendedBounds = anchorBox.ActualBounds;
			floatingObjectBox.Bounds = anchorBox.ShapeBounds;
			floatingObjectBox.ContentBounds = anchorBox.ContentBounds;
			floatingObjectBox.SetActualSizeBounds(anchorBox.ActualSizeBounds);
		}
		protected internal virtual void ApplyParagraphFrameBoxBounds(ParagraphFrameBox paragraphFrameBox, ParagraphFrameBox newParagraphFrameBox) {
			paragraphFrameBox.ExtendedBounds = newParagraphFrameBox.ActualBounds;
			paragraphFrameBox.Bounds = newParagraphFrameBox.ShapeBounds;
			paragraphFrameBox.ContentBounds = newParagraphFrameBox.ContentBounds;
			paragraphFrameBox.SetActualSizeBounds(newParagraphFrameBox.ActualSizeBounds);
		}
		protected internal virtual void UpdateCurrentTableCellBottom(FloatingObjectBox floatingObjectBox) {
			FloatingObjectProperties floatingObjectProperties = floatingObjectBox.GetFloatingObjectRun().FloatingObjectProperties;
			if (floatingObjectProperties.TextWrapType != FloatingObjectTextWrapType.None)
				TablesController.UpdateCurrentCellBottom(floatingObjectBox.ExtendedBounds.Bottom);
		}
		protected internal void AddFloatingObjectToLayoutCore(FloatingObjectBox floatingObjectBox, BoxInfo boxInfo) {
			TextRunBase currentRun = PieceTable.Runs[boxInfo.StartPos.RunIndex];
			FloatingObjectAnchorRun objectAnchorRun = currentRun as FloatingObjectAnchorRun;
			floatingObjectBox.StartPos = boxInfo.StartPos;
			ApplyFloatingObjectBoxProperties(floatingObjectBox, objectAnchorRun.FloatingObjectProperties);
			FloatingObjectAnchorBox anchorBox = (FloatingObjectAnchorBox)boxInfo.Box;
			anchorBox.SetFloatingObjectRun(objectAnchorRun);
			ApplyFloatingObjectBoxBounds(floatingObjectBox, anchorBox);
			if (objectAnchorRun.Content is TextBoxFloatingObjectContent)
				FormatFloatingObjectTextBox(floatingObjectBox, objectAnchorRun.Content as TextBoxFloatingObjectContent, anchorBox);
			FloatingObjectsLayout.Add(objectAnchorRun, floatingObjectBox);
			UpdateCurrentTableCellBottom(floatingObjectBox);
			RecreateHorizontalPositionController();
		}
		protected internal void AddParagraphFrameToLayoutCore(ParagraphFrameBox paragraphFrameBox, BoxInfo boxInfo) {
			paragraphFrameBox.StartPos = boxInfo.StartPos;
			ApplyParagraphFrameBoxBounds(paragraphFrameBox, boxInfo.Box as ParagraphFrameBox);
			FormatParagraphFrameTextBox(paragraphFrameBox);
			ParagraphFramesLayout.AddParagraphFrameBox(paragraphFrameBox);
			RecreateHorizontalPositionController();
		}
		bool ShouldChangeExistingFloatingObjectBounds(FloatingObjectBox floatingObjectBox, FloatingObjectAnchorBox anchorBox) {
			Rectangle existingBounds = floatingObjectBox.ExtendedBounds;
			Rectangle newBounds = anchorBox.ActualBounds;
			return newBounds != existingBounds;
		}
		protected internal bool AddFloatingObjectToLayout(FloatingObjectAnchorBox floatingObjectAnchorBox, BoxInfo boxInfo) {
			TextRunBase currentRun = PieceTable.Runs[boxInfo.StartPos.RunIndex];
			FloatingObjectAnchorRun objectAnchorRun = currentRun as FloatingObjectAnchorRun;
			if (objectAnchorRun.ExcludeFromLayout)
				return false;
			Debug.Assert(boxInfo.Box != null && boxInfo.Box is FloatingObjectAnchorBox && boxInfo.Box == floatingObjectAnchorBox);
			if (FloatingObjectsLayout.ContainsRun(objectAnchorRun)) {
				FloatingObjectBox floatingObjectBox = FloatingObjectsLayout.GetFloatingObject(objectAnchorRun);
				if (objectAnchorRun.FloatingObjectProperties.TextWrapType != FloatingObjectTextWrapType.None)
					floatingObjectBox.LockPosition = true;
				FloatingObjectAnchorBox anchorBox = (FloatingObjectAnchorBox)boxInfo.Box;
				if (ShouldChangeExistingFloatingObjectBounds(floatingObjectBox, anchorBox)) {
					ApplyFloatingObjectBoxBounds(floatingObjectBox, anchorBox);
					if (objectAnchorRun.Content is TextBoxFloatingObjectContent)
						FormatFloatingObjectTextBox(floatingObjectBox, objectAnchorRun.Content as TextBoxFloatingObjectContent, anchorBox);
				}
				return false;
			}
			else {
				FloatingObjectBox floatingObjectBox = new FloatingObjectBox();
				AddFloatingObjectToLayoutCore(floatingObjectBox, boxInfo);
				if (objectAnchorRun.FloatingObjectProperties.TextWrapType != FloatingObjectTextWrapType.None)
					floatingObjectBox.LockPosition = true;
				return true;
			}
		}
		protected internal bool AddParagraphFrameToLayout(ParagraphFrameBox paragrahFrameBox, BoxInfo boxInfo) {
			TextRunBase currentRun = PieceTable.Runs[boxInfo.StartPos.RunIndex];
			Debug.Assert(boxInfo.Box != null);
			Paragraph paragraph = currentRun.Paragraph;
			if (ParagraphFramesLayout.ContainsParagraph(paragraph)) {
				return false;
			}
			else {
				AddParagraphFrameToLayoutCore(paragrahFrameBox, boxInfo);
				if (paragraph.GetMergedFrameProperties().Info.TextWrapType != ParagraphFrameTextWrapType.None)
					paragrahFrameBox.LockPosition = true;
				return true;
			}
		}
		protected internal virtual void FormatFloatingObjectTextBox(FloatingObjectBox floatingObjectBox, TextBoxFloatingObjectContent content, FloatingObjectAnchorBox anchorBox) {
			using (TextBoxFloatingObjectContentPrinter printer = new TextBoxFloatingObjectContentPrinter(content.TextBox, ColumnController.PageAreaController.PageController.Pages.Last.PageNumberSource, ColumnController.Measurer)) {
				Rectangle textBoxContent = floatingObjectBox.GetTextBoxContentBounds();
				printer.ColumnLocation = textBoxContent.Location;
				printer.ColumnSize = GetColumnSize(anchorBox, content, textBoxContent);
				int actualSize = printer.Format(printer.ColumnLocation.Y + printer.ColumnSize.Height);
				if (printer.FrameParagraphIndex != new ParagraphIndex(-1))
					FrameParagraphIndex = printer.FrameParagraphIndex;
				actualSize -= printer.ColumnLocation.Y;
				if (content.TextBoxProperties.ResizeShapeToFitText)
					ResizeFloatingObjectBoxToFitText(floatingObjectBox, content, anchorBox, textBoxContent, actualSize);
				floatingObjectBox.DocumentLayout = printer.DocumentLayout;
				PageArea textBoxArea = floatingObjectBox.DocumentLayout.Pages.First.Areas.First;
				Rectangle actualPageAreaBounds = textBoxArea.Columns.First.Bounds;
				actualPageAreaBounds.Height = actualSize;
				textBoxArea.Bounds = actualPageAreaBounds;
			}
		}
		protected internal virtual void FormatParagraphFrameTextBox(ParagraphFrameBox paragraphFrameBox) {
			TextBoxContentType textBox = new TextBoxContentType(paragraphFrameBox.PieceTable);
			using (TextBoxFloatingObjectContentPrinter printer = new TextBoxFloatingObjectContentPrinter(textBox, ColumnController.PageAreaController.PageController.Pages.Last.PageNumberSource, ColumnController.Measurer)) {
				printer.FrameParagraphIndex = paragraphFrameBox.ParagraphIndex;
				printer.ColumnLocation = paragraphFrameBox.ContentBounds.Location;
				printer.ColumnSize = GetParagraphFrameColumnSize(paragraphFrameBox);
				printer.Format(printer.ColumnSize.Height);
				FrameParagraphIndex = printer.FrameParagraphIndex;
				paragraphFrameBox.DocumentLayout = printer.DocumentLayout;
				PageArea pageArea = paragraphFrameBox.DocumentLayout.Pages.First.Areas.First;
				Column column = pageArea.Columns.First;
				RowCollection rows = column.Rows;
				int rowsHeight = rows.Last.Bounds.Bottom - rows.First.Bounds.Top;
				MergedFrameProperties frameProperties = paragraphFrameBox.FrameProperties;
				ParagraphFrameHorizontalRule hRule = frameProperties.Info.HorizontalRule;
				if (hRule == ParagraphFrameHorizontalRule.Auto || (hRule == ParagraphFrameHorizontalRule.AtLeast && paragraphFrameBox.Bounds.Height < rowsHeight))
					SetActualBoundsForEmptyHeightFrameBox(paragraphFrameBox, rowsHeight);
				if (frameProperties.Info.Width == 0) {
					MaxWidthCalculator calculator = new MaxWidthCalculator();
					int elementBoundsWidth = calculator.GetActualMaxWidth(rows);
					SetActualBoundsForEmptyWidthFrameBox(paragraphFrameBox, frameProperties.Info.HorizontalPositionAlignment, elementBoundsWidth);
				}
				Rectangle actualPageAreaBounds = column.Bounds;
				actualPageAreaBounds.Height = paragraphFrameBox.ActualSizeBounds.Height;
				pageArea.Bounds = actualPageAreaBounds;
				column.Bounds = actualPageAreaBounds;
			}
		}
		Size GetParagraphFrameColumnSize(ParagraphFrameBox paragraphFrameBox) {
			Rectangle bounds = paragraphFrameBox.ContentBounds;
			Size result = new Size();
			result.Height = int.MaxValue / 4;
			if (paragraphFrameBox.GetParagraph().GetMergedFrameProperties().Info.Width == 0)
				result.Width = CurrentColumn.ActualSizeBounds.Width;
			else
				result.Width = bounds.Size.Width;
			return result;
		}
		private static Rectangle GetBounds(Point location, int width, int height) {
			return new Rectangle(location, new Size(width, height));
		}
		private static void SetActualBoundsForEmptyHeightFrameBox(ParagraphFrameBox paragraphFrameBox, int elementBoundsHeight) {
			paragraphFrameBox.Bounds = GetBounds(paragraphFrameBox.Bounds.Location, paragraphFrameBox.Bounds.Width, elementBoundsHeight);
			paragraphFrameBox.ShapeBounds = GetBounds(paragraphFrameBox.ShapeBounds.Location, paragraphFrameBox.ShapeBounds.Width, elementBoundsHeight);
			paragraphFrameBox.ContentBounds = GetBounds(paragraphFrameBox.ContentBounds.Location, paragraphFrameBox.ContentBounds.Width, elementBoundsHeight);
			paragraphFrameBox.ExtendedBounds = GetBounds(paragraphFrameBox.ExtendedBounds.Location, paragraphFrameBox.ExtendedBounds.Width, elementBoundsHeight);
			paragraphFrameBox.ActualBounds = GetBounds(paragraphFrameBox.ActualBounds.Location, paragraphFrameBox.ActualBounds.Width, elementBoundsHeight);
			paragraphFrameBox.SetActualSizeBounds(GetBounds(paragraphFrameBox.ActualSizeBounds.Location, paragraphFrameBox.ActualSizeBounds.Width, elementBoundsHeight));
		}
		private static Point GetBoundsLocation(Rectangle bounds, int positionOffset) {
			return new Point(bounds.Location.X - positionOffset, bounds.Location.Y);
		}
		private static void SetActualBoundsForEmptyWidthFrameBox(ParagraphFrameBox paragraphFrameBox, ParagraphFrameHorizontalPositionAlignment alignment, int elementBoundsWidth) {
			int positionOffset = 0;
			if (alignment == ParagraphFrameHorizontalPositionAlignment.Right || alignment == ParagraphFrameHorizontalPositionAlignment.Outside)
				positionOffset = elementBoundsWidth;
			if (alignment == ParagraphFrameHorizontalPositionAlignment.Center)
				positionOffset = elementBoundsWidth / 2;
			Point boundsLocation = GetBoundsLocation(paragraphFrameBox.Bounds, positionOffset);
			Point shapeLocation = GetBoundsLocation(paragraphFrameBox.ShapeBounds, positionOffset);
			Point contentLocation = GetBoundsLocation(paragraphFrameBox.ContentBounds, positionOffset);
			Point extendedLocation = GetBoundsLocation(paragraphFrameBox.ExtendedBounds, positionOffset);
			Point actualLocation = GetBoundsLocation(paragraphFrameBox.ActualBounds, positionOffset);
			Point actualSizeLocation = GetBoundsLocation(paragraphFrameBox.ActualSizeBounds, positionOffset);
			paragraphFrameBox.Bounds = GetBounds(boundsLocation, elementBoundsWidth, paragraphFrameBox.Bounds.Height);
			paragraphFrameBox.ShapeBounds = GetBounds(shapeLocation, elementBoundsWidth, paragraphFrameBox.ShapeBounds.Height);
			paragraphFrameBox.ContentBounds = GetBounds(contentLocation, elementBoundsWidth, paragraphFrameBox.ContentBounds.Height);
			paragraphFrameBox.ExtendedBounds = GetBounds(extendedLocation, elementBoundsWidth, paragraphFrameBox.ExtendedBounds.Height);
			paragraphFrameBox.ActualBounds = GetBounds(actualLocation, elementBoundsWidth, paragraphFrameBox.ActualBounds.Height);
			paragraphFrameBox.SetActualSizeBounds(GetBounds(actualSizeLocation, elementBoundsWidth, paragraphFrameBox.ActualSizeBounds.Height));
			ParagraphBoxCollection boxCollection = paragraphFrameBox.GetParagraph().BoxCollection;
			int count = boxCollection.Count;
			for (int i = 0; i < count; i++) {
				Box currentBox = boxCollection[i];
				Point currentBoxLocation = GetBoundsLocation(currentBox.Bounds, positionOffset);
				currentBox.Bounds = new Rectangle(currentBoxLocation, currentBox.Bounds.Size);
			}
		}
		Size GetColumnSize(FloatingObjectAnchorBox anchorBox, TextBoxFloatingObjectContent content, Rectangle textBoxContent) {
			if (content.TextBoxProperties.ResizeShapeToFitText)
				return new Size(textBoxContent.Width, Int32.MaxValue / 4);
			else {
				if (!content.TextBoxProperties.Upright)
					return new Size(textBoxContent.Width, Int32.MaxValue / 4);
				else {
					FloatingObjectAnchorRun objectAnchorRun = (FloatingObjectAnchorRun)PieceTable.Runs[anchorBox.StartPos.RunIndex];
					float angle = (DocumentModel.UnitConverter.ModelUnitsToDegreeF(objectAnchorRun.Shape.Rotation) % 360f);
					if (angle < 0)
						angle += 360;
					if ((angle < 45) || (angle >= 135 && angle < 225) || (angle >= 315))
						return textBoxContent.Size;
					else
						return new Size(textBoxContent.Size.Height, Int32.MaxValue / 4);
				}
			}
		}
		void ResizeFloatingObjectBoxToFitText(FloatingObjectBox floatingObjectBox, TextBoxFloatingObjectContent content, FloatingObjectAnchorBox anchorBox, Rectangle textBoxContent, int actualSize) {
			DocumentModelUnitToLayoutUnitConverter unitConverter = DocumentModel.ToDocumentLayoutUnitConverter;
			FloatingObjectProperties floatingObjectProperties = content.TextBox.AnchorRun.FloatingObjectProperties;
			int deltaHeight = actualSize - textBoxContent.Height;
			if (deltaHeight == 0)
				return;
			floatingObjectBox.IncreaseHeight(deltaHeight);
			anchorBox.IncreaseHeight(deltaHeight);
			int newActualHeight = floatingObjectProperties.ActualHeight + unitConverter.ToModelUnits(deltaHeight);
			if (newActualHeight != content.OriginalSize.Height) {
				IRectangularScalableObject rectangularObject = content.TextBox.AnchorRun;
				floatingObjectProperties.DisableHistory = true;
				try {
					rectangularObject.ScaleY = 100 * newActualHeight / (float)content.OriginalSize.Height;
				}
				finally {
					floatingObjectProperties.DisableHistory = false;
				}
			}
		}
		protected internal virtual DocumentModelPosition CalculateRestartPositionDueFloatingObject(FloatingObjectAnchorBox floatingObjectAnchorBox) {
			int x = this.CurrentHorizontalPosition;
			int y = this.CurrentRow.Bounds.Top;
			return CalculateRestartPositionDueFloatingObject(floatingObjectAnchorBox, floatingObjectAnchorBox.ActualBounds, FloatingObjectsLayout.ContainsRun(floatingObjectAnchorBox.GetFloatingObjectRun(PieceTable)), new Point(x, y));
		}
		protected internal virtual DocumentModelPosition CalculateRestartPositionDueParagraphFrame(ParagraphFrameBox paragraphFrameBox) {
			int x = this.CurrentHorizontalPosition;
			int y = this.CurrentRow.Bounds.Top;
			return CalculateRestartPositionDueFloatingObject(paragraphFrameBox, paragraphFrameBox.ActualBounds, ParagraphFramesLayout.ContainsParagraph(paragraphFrameBox.ParagraphIndex), new Point(x, y));
		}
		protected internal virtual DocumentModelPosition CalculateRestartPositionDueFloatingObject(FloatingObjectAnchorBox floatingObjectAnchorBox, Point currentPosition) {
			Rectangle bounds = floatingObjectAnchorBox.ActualBounds;
			if (bounds.Left >= currentPosition.X && bounds.Top >= currentPosition.Y)
				return null;
			PageController pageController = ColumnController.PageAreaController.PageController;
			PageCollection pages = pageController.Pages;
			Page currentPage = pages.Last;
			if (currentPage.IsEmpty) {
				if (FloatingObjectsLayout.ContainsRun(floatingObjectAnchorBox.GetFloatingObjectRun(PieceTable)))
					return null; 
				if (CurrentRow.IsFirstParagraphRow)
					return DocumentModelPosition.FromParagraphStart(PieceTable, Paragraph.Index);
				if (CurrentRow.Boxes.Count > 0)
					return CurrentRow.GetFirstPosition(PieceTable);
				else
					return floatingObjectAnchorBox.GetFirstPosition(PieceTable);
			}
			RichEditHitTestRequest request = new RichEditHitTestRequest(PieceTable);
			request.LogicalPoint = bounds.Location;
			request.IgnoreInvalidAreas = true;
			request.DetailsLevel = DocumentLayoutDetailsLevel.Row;
			request.Accuracy = HitTestAccuracy.NearestPageArea | HitTestAccuracy.NearestColumn | HitTestAccuracy.NearestRow;
			RichEditHitTestResult result = new RichEditHitTestResult(pageController.DocumentLayout, PieceTable);
			result.Page = currentPage;
			result.IncreaseDetailsLevel(DocumentLayoutDetailsLevel.Page);
			BoxHitTestCalculator calculator = pageController.CreateHitTestCalculator(request, result);
			TableCellColumnController tableCellColumnController = this.ColumnController as TableCellColumnController;
			if (tableCellColumnController != null) {
				result.PageArea = ColumnController.PageAreaController.Areas.Last;
				result.Column = tableCellColumnController.CurrentTopLevelColumn;
				result.IncreaseDetailsLevel(DocumentLayoutDetailsLevel.Column);
				calculator.CalcHitTest(this.CurrentColumn);
				if (!result.IsValid(DocumentLayoutDetailsLevel.Row)) {
					if (FloatingObjectsLayout.ContainsRun(floatingObjectAnchorBox.GetFloatingObjectRun(PieceTable)))
						return null; 
					return floatingObjectAnchorBox.GetFirstPosition(PieceTable);
				}
			}
			else
				calculator.CalcHitTest(currentPage);
			if (!result.IsValid(DocumentLayoutDetailsLevel.Row)) {
				Debug.Assert(result.IsValid(DocumentLayoutDetailsLevel.PageArea));
				Debug.Assert(result.PageArea.Bounds.Bottom <= bounds.Location.Y);
				return null;
			}
			Debug.Assert(result.IsValid(DocumentLayoutDetailsLevel.Row));
			Debug.Assert(result.Row != null);
			RowCollection rows = result.Column.Rows;
			if (rows.Last == result.Row) {
				Rectangle rowBounds = result.Row.Bounds;
				if (bounds.Top > rowBounds.Bottom && !rowBounds.IntersectsWith(bounds))
					return null;
			}
			TableCell cell = GetTableCell(result);
			if (cell != null) { 
				if (tableCellColumnController == null) { 
					Table table = cell.Table;
					while (table.ParentCell != null)
						table = table.ParentCell.Table;
					TableCell lastTableCell = table.Rows.Last.Cells.Last;
					return DocumentModelPosition.FromParagraphStart(PieceTable, lastTableCell.EndParagraphIndex + 1);
				}
			}
			return result.Row.GetFirstPosition(PieceTable);
		}
		protected internal virtual DocumentModelPosition CalculateRestartPositionDueFloatingObject(SinglePositionBox box, Rectangle bounds, bool isContains, Point currentPosition) {
			if (bounds.Left >= currentPosition.X && bounds.Top >= currentPosition.Y)
				return null;
			PageController pageController = ColumnController.PageAreaController.PageController;
			PageCollection pages = pageController.Pages;
			Page currentPage = pages.Last;
			if (currentPage.IsEmpty) {
				if (isContains)
					return null; 
				if (CurrentRow.IsFirstParagraphRow)
					return DocumentModelPosition.FromParagraphStart(PieceTable, Paragraph.Index);
				if (CurrentRow.Boxes.Count > 0)
					return CurrentRow.GetFirstPosition(PieceTable);
				else
					return box.GetFirstPosition(PieceTable);
			}
			if (this.Paragraph.GetMergedFrameProperties() != null)
				return DocumentModelPosition.FromParagraphStart(PieceTable, Paragraph.Index);
			RichEditHitTestRequest request = new RichEditHitTestRequest(PieceTable);
			request.LogicalPoint = new Point(Math.Max(bounds.Location.X, ParagraphLeft), bounds.Location.Y);
			request.IgnoreInvalidAreas = true;
			request.DetailsLevel = DocumentLayoutDetailsLevel.Row;
			request.Accuracy = HitTestAccuracy.NearestPageArea | HitTestAccuracy.NearestColumn | HitTestAccuracy.NearestRow;
			RichEditHitTestResult result = new RichEditHitTestResult(pageController.DocumentLayout, PieceTable);
			result.Page = currentPage;
			result.IncreaseDetailsLevel(DocumentLayoutDetailsLevel.Page);
			BoxHitTestCalculator calculator = pageController.CreateHitTestCalculator(request, result);
			TableCellColumnController tableCellColumnController = this.ColumnController as TableCellColumnController;
			if (tableCellColumnController != null) {
				result.PageArea = ColumnController.PageAreaController.Areas.Last;
				result.Column = tableCellColumnController.CurrentTopLevelColumn;
				result.IncreaseDetailsLevel(DocumentLayoutDetailsLevel.Column);
				calculator.CalcHitTest(this.CurrentColumn);
				if (!result.IsValid(DocumentLayoutDetailsLevel.Row)) {
					if (isContains)
						return null; 
					return box.GetFirstPosition(PieceTable);
				}
			}
			else
				calculator.CalcHitTest(currentPage);
			if (!result.IsValid(DocumentLayoutDetailsLevel.Row)) {
				Debug.Assert(result.IsValid(DocumentLayoutDetailsLevel.PageArea));
				Debug.Assert(result.PageArea.Bounds.Bottom <= bounds.Location.Y);
				return null;
			}
			Debug.Assert(result.IsValid(DocumentLayoutDetailsLevel.Row));
			Debug.Assert(result.Row != null);
			RowCollection rows = result.Column.Rows;
			if (rows.Last == result.Row) {
				Rectangle rowBounds = result.Row.Bounds;
				if (bounds.Top > rowBounds.Bottom && !rowBounds.IntersectsWith(bounds))
					return null;
			}
			TableCell cell = GetTableCell(result);
			if (cell != null) { 
				Table currentTable = tableCellColumnController != null ? tableCellColumnController.CurrentCell.Cell.Table : null;
				if (currentTable != cell.Table) { 
					Table table = cell.Table;
					while (table.ParentCell != null && table.ParentCell.Table != currentTable)
						table = table.ParentCell.Table;
					TableCell lastTableCell = table.Rows.Last.Cells.Last;
					return DocumentModelPosition.FromParagraphStart(PieceTable, lastTableCell.EndParagraphIndex + 1);
				}
			}
			return result.Row.GetFirstPosition(PieceTable);
		}
		TableCell GetTableCell(RichEditHitTestResult result) {
			if (result.TableCell != null)
				return result.TableCell.Cell;
			TableCellRow row = result.Row as TableCellRow;
			if (row != null)
				return row.CellViewInfo.Cell;
			return null;
		}
		CanFitCurrentRowToColumnResult CanFitCurrentRowToColumnCore(int newRowHeight) {
			return HorizontalPositionController.CanFitCurrentRowToColumn(newRowHeight);
		}
		internal bool CanFitNumberingListBoxToCurrentRow(Size boxSize) {
			return HorizontalPositionController.CanFitNumberingListBoxToCurrentRow(boxSize);
		}
		internal bool CanFitBoxToCurrentRow(Size boxSize) {
			return HorizontalPositionController.CanFitBoxToCurrentRow(boxSize);
		}
		internal int GetMaxBoxWidth() {
			return HorizontalPositionController.GetMaxBoxWidth();
		}
		internal bool IsPositionOutsideRightParagraphBound(int pos) {
			return pos > paragraphRight;
		}
		internal int CalcNewCurrentRowHeight(BoxInfo newBoxInfo) {
			return lineSpacingCalculator.CalcRowHeight(CurrentRow.Height, CalcRowHeightWithBox(newBoxInfo));
		}
		protected internal virtual void OnCellStart() {
			spacingAfterPrevParagraph = 0;
			isSpacingAfterPrevParagraphValid = true;
		}
		int CalcRowHeightWithBox(BoxInfo boxInfo) {
			int ascentAndFree = GetFontAscentAndFree(boxInfo);
			if (boxInfo.Box != null && boxInfo.Box.IsInlinePicture)
				ascentAndFree = Math.Max(ascentAndFree, boxInfo.Size.Height);
			return Math.Max(maxAscentAndFree, ascentAndFree) + Math.Max(maxDescent, GetFontDescent(boxInfo));
		}
		void UpdateMaxAscentAndDescent(BoxInfo boxInfo) {
			this.maxAscentAndFree = Math.Max(maxAscentAndFree, GetFontAscentAndFree(boxInfo));
			this.maxDescent = Math.Max(maxDescent, GetFontDescent(boxInfo));
		}
		protected virtual int GetFontAscentAndFree(BoxInfo newBoxInfo) {
			return newBoxInfo.GetFontInfo(PieceTable).GetBaseAscentAndFree(DocumentModel);
		}
		protected virtual int GetFontDescent(BoxInfo newBoxInfo) {
			return newBoxInfo.GetFontInfo(PieceTable).GetBaseDescent(DocumentModel);
		}
		protected virtual int GetFontAscentAndFree(TextRunBase run) {
			return DocumentModel.FontCache[run.FontCacheIndex].GetBaseAscentAndFree(DocumentModel);
		}
		protected virtual int GetFontDescent(InlinePictureRun run) {
			return DocumentModel.FontCache[run.FontCacheIndex].GetBaseDescent(DocumentModel);
		}
		public void RemoveLastTextBoxes() {
			BoxCollection boxes = CurrentRow.Boxes;
			boxes.RemoveRange(startWordBoxIndex, boxes.Count - startWordBoxIndex);
			CurrentRow.Height = heightBeforeWord;
			maxAscentAndFree = maxAscentBeforeWord;
			maxDescent = maxDescentBeforeWord;
			maxPictureHeight = maxPictureHeightBeforeWord;
			currentRunIndex = runIndexBeforeWord;
			CurrentRow.ProcessingFlags = rowProcessingFlagsBeforeWord;
			int count = boxes.Count;
			if (count > 0)
				HorizontalPositionController.RollbackCurrentHorizontalPositionTo(boxes[count - 1].Bounds.Right);
			else
				if (CurrentRow.NumberingListBox == null)
					HorizontalPositionController.SetCurrentRowInitialHorizontalPosition();
				else
					HorizontalPositionController.RollbackCurrentHorizontalPositionTo(CurrentRow.NumberingListBox.Bounds.Right);
			tabsController.UpdateLastTabPosition(count);
		}
		public void TryToRemoveLastTabBox() {
			BoxCollection boxes = CurrentRow.Boxes;
			int index = boxes.Count - 1;
#if DEBUGTEST
			Debug.Assert(index >= 0);
#endif
			TabSpaceBox tab = boxes[index] as TabSpaceBox;
			if (tab != null) {
				HorizontalPositionController.RollbackCurrentHorizontalPositionTo(tab.Bounds.X);
				boxes.RemoveAt(index);
				tabsController.UpdateLastTabPosition(index);
			}
		}
		protected internal virtual LineSpacingCalculatorBase CreateLineSpacingCalculator() {
			return LineSpacingCalculatorBase.Create(paragraph);
		}
		void PrepareCurrentRowBounds(Paragraph paragraph, bool beginFromParagraphStart) {
			this.lineSpacingCalculator = CreateLineSpacingCalculator();
			defaultRowHeight = lineSpacingCalculator.DefaultRowHeight;
			ObtainRowIndents();
			if (!beginFromParagraphStart)
				currentRowIndent = regularRowIndent;
			tabsController.BeginParagraph(paragraph);
			ApplyColumnBounds();
			CurrentRow.Bounds = CalcDefaultRowBounds(CurrentRow.Bounds.Y);
		}
		public void BeginParagraph(Paragraph paragraph, bool beginFromParagraphStart) {
			Debug.Assert(Object.ReferenceEquals(paragraph.PieceTable, PieceTable));
			EnsureSpacingAfterPrevParagraphValid(paragraph);
			this.paragraph = paragraph;
			PrepareCurrentRowBounds(paragraph, beginFromParagraphStart); 
			TablesController.BeginParagraph(paragraph);
			PrepareCurrentRowBounds(paragraph, beginFromParagraphStart);
			int spacingBefore = CalculateSpacingBefore();
			invisibleEmptyParagraphInCellAfterNestedTable = ShouldIgnoreParagraphHeight(paragraph);
			if (invisibleEmptyParagraphInCellAfterNestedTable) {
				CurrentRow.ProcessingFlags |= RowProcessingFlags.LastInvisibleEmptyCellRowAfterNestedTable;
				CurrentRow.Height = 0;
				spacingBefore = 0;
			}
			CurrentRow.SpacingBefore = spacingBefore;
			CurrentRow.ProcessingFlags |= RowProcessingFlags.FirstParagraphRow;
			if (paragraph.GetMergedFrameProperties() == null || FrameParagraphIndex != new ParagraphIndex(-1))
				OffsetCurrentRow(spacingBefore);
			HorizontalPositionController.OnCurrentRowHeightChanged(false);
			HorizontalPositionController.SetCurrentRowInitialHorizontalPosition();
			AssignCurrentRowLineNumber();
			if (tabsController.SingleDecimalTabInTable)
				AddSingleDecimalTabInTable();
			if (beginFromParagraphStart) {
				ParagraphFirstRowOnPage = CurrentRow;
				ParagraphFirstColumnOnPage = CurrentColumn;
			}
			else
				ObtainParagraphFirstRowPosition(paragraph);
		}
		protected internal virtual int CalculateSpacingBefore() {
			int spacingBefore = DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(paragraph.ContextualSpacingBefore);
			if (spacingBefore < spacingAfterPrevParagraph)
				return 0;
			return spacingBefore - spacingAfterPrevParagraph;
		}
		private void ObtainParagraphFirstRowPosition(Paragraph paragraph) {
		}
		void EnsureSpacingAfterPrevParagraphValid(Paragraph currentParagraph) {
			if (isSpacingAfterPrevParagraphValid)
				return;
			ParagraphIndex prevParagraphIndex = currentParagraph.Index - 1;
			spacingAfterPrevParagraph = GetActualSpacingAfter(prevParagraphIndex);
			isSpacingAfterPrevParagraphValid = true;
		}
		int GetActualSpacingAfter(ParagraphIndex paragraphIndex) {
			if (paragraphIndex < ParagraphIndex.Zero)
				return 0;
			Paragraph paragraph = PieceTable.Paragraphs[paragraphIndex];
			if (!ShouldIgnoreParagraphHeight(paragraph))
				return GetContextualSpacingAfter(paragraph);
			return 0;
		}
		bool ShouldIgnoreParagraphHeight(Paragraph paragraph) {
			DocumentModel documentModel = paragraph.DocumentModel;
			if (documentModel.IsSpecialEmptyParagraphAfterInnerTable(paragraph, TablesController.GetCurrentCell()) && !documentModel.IsCursorInParagraph(paragraph))
				return true;
			return false;
		}
		protected internal virtual int GetContextualSpacingAfter(Paragraph paragraph) {
			return DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(paragraph.ContextualSpacingAfter);
		}
		public void EndParagraph() {
			int spacingAfter = GetContextualSpacingAfter(paragraph);
			if (invisibleEmptyParagraphInCellAfterNestedTable)
				spacingAfter = 0;
			Row lastRow = IncreaseLastRowHeight(spacingAfter);
			lastRow.LastParagraphRowOriginalHeight = lastRow.Height - spacingAfter;
			lastRow.ProcessingFlags |= RowProcessingFlags.LastParagraphRow;
			TablesController.EndParagraph(lastRow);
			OffsetCurrentRow(spacingAfter);
			spacingAfterPrevParagraph = spacingAfter;
			isSpacingAfterPrevParagraphValid = true;
		}
		public void EndSection() {
			EndParagraph();
			if (paragraph.Index + 1 < new ParagraphIndex(PieceTable.Paragraphs.Count))
				RaiseBeginNextSectionFormatting();
			ColumnController.PageAreaController.PageController.NextSection = true;
			CompleteCurrentColumnFormatting();
			MoveRowToNextColumn();
		}
		public void ClearRow(bool keepTextAreas) {
			CurrentRow.Boxes.Clear();
			CurrentRow.ClearBoxRanges();
			InitCurrentRow(keepTextAreas);
		}
		void InitCurrentRow(bool keepTextAreas) {
			SuppressHorizontalOverfull = false;
			HorizontalPositionController.OnCurrentRowHeightChanged(keepTextAreas);
			HorizontalPositionController.SetCurrentRowInitialHorizontalPosition();
			maxAscentAndFree = 0;
			maxPictureHeight = 0;
			maxDescent = 0;
			currentRunIndex = RunIndex.DontCare;
			CurrentRow.ProcessingFlags &= RowProcessingFlags.Clear;
			tabsController.ClearLastTabPosition();
			if (tabsController.SingleDecimalTabInTable)
				AddSingleDecimalTabInTable();
			AssignCurrentRowLineNumber();
		}
		public void MoveRowToNextColumn() {
			TablesController.BeforeMoveRowToNextColumn();
			MoveCurrentRowToNextColumnCore();
			TablesController.AfterMoveRowToNextColumn();
		}
		public void MoveRowToNextPage() {
			TablesController.BeforeMoveRowToNextPage();
			ColumnController.ResetToFirstColumn();
			MoveCurrentRowToNextColumnCore();
			TablesController.AfterMoveRowToNextPage();
		}
		public void OnPageBreakBeforeParagraph() {
			ColumnController.ResetToFirstColumn();
		}
		protected internal virtual CompleteFormattingResult CompleteCurrentColumnFormatting() {
			return ColumnController.CompleteCurrentColumnFormatting(CurrentColumn);
		}
		protected internal virtual void MoveCurrentRowToNextColumnCore() {
			ClearRow(false);
			CurrentColumn = ColumnController.GetNextColumn(CurrentColumn, false);
			ApplyColumnBounds();
			CurrentRow.Bounds = CalcDefaultRowBounds(CurrentColumn.Bounds.Top);
			if (ColumnController.ShouldZeroSpacingBeforeWhenMoveRowToNextColumn)
				CurrentRow.SpacingBefore = 0;
			else
				OffsetCurrentRow(CurrentRow.SpacingBefore);
			HorizontalPositionController.OnCurrentRowHeightChanged(false);
			if (CurrentRow.NumberingListBox != null) {
				CurrentRow.NumberingListBox.Bounds = currentParagraphNumberingListBounds;
				CurrentRow.NumberingListBox = null;
			}
		}
		protected internal virtual LineNumberingRestart GetEffectiveLineNumberingRestartType(Section section) {
			return section.LineNumbering.NumberingRestartType;
		}
		bool suppressLineNumberingRecalculationForLastPage;
		protected internal virtual void OnPageFormattingComplete(Section section, Page page) {
			if (suppressLineNumberingRecalculationForLastPage)
				return;
			if (section.FirstParagraphIndex > paragraph.Index) {
				SectionIndex sectionIndex = PieceTable.LookupSectionIndexByParagraphIndex(paragraph.Index);
				if (sectionIndex >= new SectionIndex(0))
					section = DocumentModel.Sections[sectionIndex];
			}
			if (GetEffectiveLineNumberingRestartType(section) == LineNumberingRestart.NewPage) {
				InitialLineNumber = section.LineNumbering.StartingLineNumber;
				LineNumberStep = section.LineNumbering.Step;
				AssignCurrentRowLineNumber();
			}
			CreateLineNumberBoxes(section, page);
		}
		protected internal virtual void CreateLineNumberBoxes(Section section, Page page) {
			if (section.LineNumbering.Step <= 0)
				return;
			PageAreaCollection areas = page.Areas;
			int count = areas.Count;
			for (int i = 0; i < count; i++)
				CreateLineNumberBoxes(section, areas[i]);
		}
		protected internal virtual void CreateLineNumberBoxes(Section section, PageArea pageArea) {
			pageArea.LineNumbers.Clear();
			ColumnCollection columns = pageArea.Columns;
			int count = columns.Count;
			for (int i = 0; i < count; i++)
				CreateLineNumberBoxes(section, columns[i], pageArea.LineNumbers);
		}
		protected internal virtual void CreateLineNumberBoxes(Section section, Column column, LineNumberBoxCollection lineNumbers) {
			BoxMeasurer measurer = columnController.Measurer;
			if (measurer == null)
				return;
			int distance = DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(section.LineNumbering.Distance);
			if (distance <= 0)
				distance = DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(DocumentModel.UnitConverter.DocumentsToModelUnits(75)); 
			BoxInfo boxInfo = new BoxInfo();
			boxInfo.StartPos = new FormatterPosition(RunIndex.Zero, 0, 0);
			boxInfo.EndPos = new FormatterPosition(RunIndex.Zero, 0, 0);
			FontInfo fontInfo = DocumentModel.FontCache[DocumentModel.LineNumberRun.FontCacheIndex];
			RowCollection rows = column.Rows;
			int count = rows.Count;
			for (int i = 0; i < count; i++) {
				Row row = rows[i];
				if (row.LineNumber > 0) {
					string text = row.LineNumber.ToString();
					LineNumberBox box = new LineNumberBox(DocumentModel.LineNumberRun, row, text);
					boxInfo.Box = box;
					measurer.MeasureText(boxInfo, text, fontInfo);
					Rectangle bounds = row.Bounds;
					Size size = boxInfo.Size;
					bounds.X = column.Bounds.X - distance - size.Width;
					bounds.Width = size.Width;
					box.Bounds = bounds;
					lineNumbers.Add(box);
				}
			}
		}
		public void EndRow(ParagraphBoxFormatter formatter) {
			EndRowCore(paragraph, formatter.Measurer);
		}
		internal void EndRowCore(Paragraph paragraph, BoxMeasurer measurer) {
			HorizontalPositionController.IncrementCurrentHorizontalPosition(tabsController.CalcLastTabWidth(CurrentRow, measurer));
			CurrentRow.Paragraph = paragraph;
#if DEBUGTEST
			ColumnController cc = this.ColumnController as ColumnController;
			if (cc != null)
				Debug.Assert(cc.PageAreaController.PageController.Pages.Last.SecondaryFormattingComplete == false);
#endif
			if (ShouldAddParagraphBackgroundFrameBox(paragraph))
				CurrentColumn.AddParagraphFrame(paragraph);
			this.forceFormattingComplete = false;
			if (CurrentColumn.Rows.Count > 0 && paragraph.GetMergedFrameProperties() == null) {
				if (PieceTable.IsHeader) {
					int headerOffset = DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(ColumnController.PageAreaController.PageController.CurrentSection.Margins.HeaderOffset);
					int maxHeaderBottom = (int)(ColumnController.PageAreaController.PageController.PageBounds.Height / 2) + headerOffset;
					this.forceFormattingComplete = CurrentRow.Bounds.Bottom > maxHeaderBottom;
				}
				if (PieceTable.IsFooter) {
					int footerOffset = DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(ColumnController.PageAreaController.PageController.CurrentSection.Margins.FooterOffset);
					int maxFooterBottom = ColumnController.PageAreaController.PageController.PageBounds.Bottom - footerOffset;
					this.forceFormattingComplete = CurrentRow.Bounds.Bottom > maxFooterBottom;
				}
			}
			if (ForceFormattingComplete) {
				this.currentRowIndent = this.regularRowIndent;
				TablesController.OnCurrentRowFinished();
				HorizontalPositionController.OnCurrentRowFinished();
				Row newRow = new Row();
				newRow.Bounds = CalcDefaultRowBounds(CurrentRow.Bounds.Top);
				CurrentRow = newRow;
				InitCurrentRow(false);
			}
			else {
				CurrentColumn.Rows.Add(CurrentRow);
				if (CurrentRow.ContainsFootNotes) {
				}
				if (!paragraph.SuppressLineNumbers && !TablesController.IsInsideTable)
					LineNumber++;
				int descent;
				if (ShouldUseMaxDescent())
					descent = maxDescent;
				else
					descent = 0;
				int lineSpacing = lineSpacingCalculator.CalculateSpacing(CurrentRow.Bounds.Height, maxAscentAndFree, descent, maxPictureHeight);
				Rectangle r = CurrentRow.Bounds;
				int spaceBefore = CurrentRow.SpacingBefore;
				r.Y -= spaceBefore;
				r.Height = lineSpacing + spaceBefore;
				CurrentRow.Bounds = r;
				this.currentRowIndent = this.regularRowIndent;
				TablesController.OnCurrentRowFinished();
				HorizontalPositionController.OnCurrentRowFinished();
				CreateNewCurrentRow(CurrentRow, false);
			}
		}
		bool ShouldAddParagraphBackgroundFrameBox(Paragraph paragraph) {
			if (DXColor.IsTransparentOrEmpty(paragraph.BackColor))
				return false;
			return paragraph.GetMergedFrameProperties() == null || paragraph.IsInCell();
		}
		protected internal virtual bool ShouldUseMaxDescent() {
			int count = CurrentRow.Boxes.Count;
			if (count <= 0)
				return true;
			if (!(CurrentRow.Boxes.First.GetRun(PieceTable) is InlineObjectRun))
				return true;
			if (count == 1)
				return false;
			if (count == 2 && CurrentRow.Boxes.Last is ParagraphMarkBox)
				return false;
			return true;
		}
		protected internal virtual void CreateNewCurrentRow(Row prevCurrentRow, bool afterRestart) {
			Row newRow = ColumnController.CreateRow();
			int prevBottom = CalcPrevBottom(prevCurrentRow, afterRestart, newRow);
			newRow.Bounds = CalcDefaultRowBounds(prevBottom);
			CurrentRow = newRow;
			InitCurrentRow(false);
		}
		int CalcPrevBottom(Row prevRow, bool afterRestart, Row newRow) {
			if (prevRow == null)
				return CurrentColumn.Bounds.Top;
			if (!afterRestart)
				return prevRow.Bounds.Bottom;
			TableCellRow tableCellRow = prevRow as TableCellRow;
			if (tableCellRow == null)
				return prevRow.Bounds.Bottom;
			else {
				TableCellRow newTableCellRow = newRow as TableCellRow;
				if (newTableCellRow == null || newTableCellRow.CellViewInfo.Cell.Table != tableCellRow.CellViewInfo.Cell.Table)
					return tableCellRow.CellViewInfo.TableViewInfo.GetTableBottom();
				else
					return prevRow.Bounds.Bottom;
			}
		}
		void ApplyColumnBounds() {
			DocumentModelUnitToLayoutUnitConverter unitConverter = DocumentModel.ToDocumentLayoutUnitConverter;
			this.ParagraphLeft = CurrentColumn.Bounds.Left + unitConverter.ToLayoutUnits(paragraph.LeftIndent);
			this.paragraphRight = CurrentColumn.Bounds.Right - unitConverter.ToLayoutUnits(paragraph.RightIndent);
			HorizontalPositionController.SetCurrentRowInitialHorizontalPosition();
			tabsController.ColumnLeft = CurrentColumn.Bounds.Left;
			tabsController.ParagraphRight = this.paragraphRight;
		}
		protected internal virtual void OnDeferredBeginParagraph() {
			ObtainRowIndents();
			RecalcRowBounds();
			HorizontalPositionController.OnCurrentRowHeightChanged(false);
			HorizontalPositionController.SetCurrentRowInitialHorizontalPosition();
		}
		protected internal virtual void RecalcRowBounds() {
			Rectangle bounds = CurrentRow.Bounds;
			CurrentRow.Bounds = new Rectangle(ParagraphLeft + currentRowIndent, bounds.Top, paragraphRight - ParagraphLeft - currentRowIndent, bounds.Height);
		}
		protected internal virtual void ObtainRowIndents() {
			switch (paragraph.FirstLineIndentType) {
				default:
				case ParagraphFirstLineIndent.None:
					this.currentRowIndent = 0;
					this.regularRowIndent = 0;
					break;
				case ParagraphFirstLineIndent.Indented:
					this.currentRowIndent = DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(paragraph.FirstLineIndent);
					this.regularRowIndent = 0;
					break;
				case ParagraphFirstLineIndent.Hanging:
					this.currentRowIndent = -DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(paragraph.FirstLineIndent);
					this.regularRowIndent = 0;
					break;
			}
		}
		public TabInfo GetNextTab(ParagraphBoxFormatter formatter) {
			ExpandLastTab(formatter);
			int position = DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(CurrentHorizontalPosition - CurrentColumn.Bounds.Left);
			int roundingFix = 0; 
			for (; ; ) {
				TabInfo tabInfo = tabsController.GetNextTab(roundingFix + position);
				int layoutTabPosition = DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(tabInfo.Position);
				if (layoutTabPosition + CurrentColumn.Bounds.Left > CurrentHorizontalPosition)
					return tabInfo;
				else
					roundingFix++;
			}
		}
		Rectangle CalcDefaultRowBounds(int y) {
			return new Rectangle(ParagraphLeft + currentRowIndent, y, paragraphRight - ParagraphLeft - currentRowIndent, DefaultRowHeight);
		}
		void OffsetCurrentRow(int offset) {
			Debug.Assert(offset >= 0);
			Rectangle r = CurrentRow.Bounds;
			r.Offset(0, offset);
			CurrentRow.Bounds = r;
		}
		Row IncreaseLastRowHeight(int delta) {
			Debug.Assert(delta >= 0);
			Row row = CurrentColumn.Rows.Last;
			Rectangle r = row.Bounds;
			r.Height += delta;
			row.Bounds = r;
			return row;
		}
		protected internal virtual void EnforceNextRowToStartFromNewPage() {
			if (CurrentColumn != null) {
				CurrentRow.Bounds = CalcDefaultRowBounds(CurrentColumn.Bounds.Bottom);
				HorizontalPositionController.OnCurrentRowHeightChanged(false);
			}
		}
		protected internal virtual void ApplySectionStart(Section section, int currentColumnsCount) {
			switch (section.GeneralSettings.StartType) {
				case SectionStartType.Continuous:
					break;
				case SectionStartType.Column:
					if (section.GetActualColumnsCount() != currentColumnsCount)
						EnforceNextRowToStartFromNewPage();
					break;
				case SectionStartType.EvenPage:
				case SectionStartType.OddPage:
				case SectionStartType.NextPage:
					EnforceNextRowToStartFromNewPage();
					break;
				default:
					break;
			}
			LineNumberingRestart restart = GetEffectiveLineNumberingRestartType(section);
			if (restart == LineNumberingRestart.NewSection) {
				InitialLineNumber = section.LineNumbering.StartingLineNumber;
				LineNumberStep = section.LineNumbering.Step;
			}
			else if (restart == LineNumberingRestart.NewPage) {
				InitialLineNumber = section.LineNumbering.StartingLineNumber;
				LineNumberStep = section.LineNumbering.Step;
			}
		}
		public void NewTableStarted() {
			RaiseTableStarted();
		}
		public virtual TableGridCalculator CreateTableGridCalculator(DocumentModel documentModel, TableWidthsCalculator widthsCalculator, int maxTableWidth) {
			return new TableGridCalculator(documentModel, widthsCalculator, maxTableWidth, DocumentModel.LayoutOptions.PrintLayoutView.AllowTablesToExtendIntoMargins, false);
		}
		public void AddPageBreakBeforeRow() {
			Column topLevelColumn = CurrentColumn.TopLevelColumn;
			Rectangle columnBounds = topLevelColumn.Bounds;
			columnBounds.Height = Math.Max(CurrentRow.Bounds.Top - columnBounds.Top, 0);
			topLevelColumn.Bounds = columnBounds;
		}
	}
	public class FloatingObjectSizeController {
		readonly PieceTable pieceTable;
		public FloatingObjectSizeController(PieceTable pieceTable) {
			this.pieceTable = pieceTable;
		}
		protected DocumentModel DocumentModel { get { return PieceTable.DocumentModel; } }
		protected PieceTable PieceTable { get { return pieceTable; } }
		public void UpdateFloatingObjectBox(FloatingObjectAnchorBox floatingObjectAnchorBox) {
			FloatingObjectAnchorRun objectAnchorRun = floatingObjectAnchorBox.GetFloatingObjectRun(PieceTable);
			bool isTextBox = objectAnchorRun.Content is TextBoxFloatingObjectContent;
			FloatingObjectProperties floatingObjectProperties = objectAnchorRun.FloatingObjectProperties;
			IFloatingObjectLocation rotatedLocation = CreateRotatedFloatingObjectLocation(floatingObjectProperties, objectAnchorRun.Shape.Rotation);
			Rectangle rotatedShapeBounds = CalculateAbsoluteFloatingObjectShapeBounds(rotatedLocation, objectAnchorRun.Shape, isTextBox);
			Rectangle shapeBounds = CalculateAbsoluteFloatingObjectShapeBounds(floatingObjectProperties, objectAnchorRun.Shape, isTextBox);
			if (floatingObjectProperties.HorizontalPositionAlignment != FloatingObjectHorizontalPositionAlignment.None)
				shapeBounds = CenterRectangleHorizontally(shapeBounds, rotatedShapeBounds);
			else {
				Rectangle rect = CenterRectangleHorizontally(rotatedShapeBounds, shapeBounds);
				rotatedShapeBounds = ValidateRotatedShapeHorizontalPosition(rect, floatingObjectProperties);
				shapeBounds = CenterRectangleHorizontally(shapeBounds, rotatedShapeBounds);
			}
			if (floatingObjectProperties.VerticalPositionAlignment != FloatingObjectVerticalPositionAlignment.None)
				shapeBounds = CenterRectangleVertically(shapeBounds, rotatedShapeBounds);
			else {
				Rectangle rect = CenterRectangleVertically(rotatedShapeBounds, shapeBounds);
				rotatedShapeBounds = ValidateRotatedShapeVerticalPosition(rect, floatingObjectProperties);
				shapeBounds = CenterRectangleVertically(shapeBounds, rotatedShapeBounds);
			}
			floatingObjectAnchorBox.ShapeBounds = shapeBounds;
			floatingObjectAnchorBox.ContentBounds = CalculateAbsoluteFloatingObjectContentBounds(floatingObjectProperties, objectAnchorRun.Shape, floatingObjectAnchorBox.ShapeBounds);
			floatingObjectAnchorBox.ActualBounds = CalculateActualAbsoluteFloatingObjectBounds(floatingObjectProperties, rotatedShapeBounds);
			if (isTextBox) {
				DocumentModelUnitToLayoutUnitConverter converter = DocumentModel.ToDocumentLayoutUnitConverter;
				Rectangle actualSizeBounds = floatingObjectAnchorBox.ShapeBounds;
				int outlineWidth = converter.ToLayoutUnits(objectAnchorRun.Shape.OutlineWidth);
				actualSizeBounds.Y += outlineWidth / 2;
				actualSizeBounds.Height -= outlineWidth;
				floatingObjectAnchorBox.SetActualSizeBounds(actualSizeBounds);
			}
			else
				floatingObjectAnchorBox.SetActualSizeBounds(floatingObjectAnchorBox.ContentBounds);
		}
		private static int GetActualBorderWidth(bool useBorder, BorderInfo border, DocumentModelUnitToLayoutUnitConverter converter) {
			return useBorder ? converter.ToLayoutUnits(border.Width + border.Offset) : 0;
		}
		public void UpdateParagraphFrameBox(ParagraphFrameBox paragraphFrameBox) {
			ParagraphProperties paragraphProperties = paragraphFrameBox.GetParagraph().ParagraphProperties;
			MergedFrameProperties frameProperties = paragraphFrameBox.FrameProperties;
			Rectangle shapeBounds = CalculateAbsoluteParagraphFrameShapeBounds(frameProperties);
			DocumentModelUnitToLayoutUnitConverter converter = DocumentModel.ToDocumentLayoutUnitConverter;
			paragraphFrameBox.ShapeBounds = shapeBounds;
			paragraphFrameBox.ContentBounds = paragraphFrameBox.ShapeBounds;
			int leftBorderWidthWithOffset = GetActualBorderWidth(paragraphProperties.UseLeftBorder, paragraphProperties.LeftBorder, converter);
			int rightBorderWidthWithOffset = GetActualBorderWidth(paragraphProperties.UseRightBorder, paragraphProperties.RightBorder, converter);
			int topBorderWidthWithOffset = GetActualBorderWidth(paragraphProperties.UseTopBorder, paragraphProperties.TopBorder, converter);
			int bottomBorderWidthWithOffset = GetActualBorderWidth(paragraphProperties.UseBottomBorder, paragraphProperties.BottomBorder, converter);
			const int padding = 30; 
			int horizontalPadding = frameProperties.Info.HorizontalPadding != 0 ? converter.ToLayoutUnits(padding) : 0;
			const int borderOffset = 15; 
			int leftPosition = leftBorderWidthWithOffset != 0 ? horizontalPadding + leftBorderWidthWithOffset + converter.ToLayoutUnits(borderOffset) : 0;
			int rightPosition = rightBorderWidthWithOffset != 0 ? horizontalPadding + rightBorderWidthWithOffset + converter.ToLayoutUnits(borderOffset) : 0;
			Rectangle newShapeBounds = paragraphFrameBox.ShapeBounds;
			paragraphFrameBox.ShapeBounds = new Rectangle(newShapeBounds.X - leftPosition, newShapeBounds.Y, newShapeBounds.Width + leftPosition + rightPosition, newShapeBounds.Height);
			paragraphFrameBox.ContentBounds = new Rectangle(newShapeBounds.X, newShapeBounds.Y + topBorderWidthWithOffset, newShapeBounds.Width, newShapeBounds.Height - topBorderWidthWithOffset - bottomBorderWidthWithOffset);
			paragraphFrameBox.ActualBounds = paragraphFrameBox.ShapeBounds;
			paragraphFrameBox.SetActualSizeBounds(paragraphFrameBox.ShapeBounds);
		}
		protected virtual Rectangle ValidateRotatedShapeHorizontalPosition(Rectangle rect, FloatingObjectProperties properties) {
			return rect;
		}
		protected virtual Rectangle ValidateRotatedShapeVerticalPosition(Rectangle rect, FloatingObjectProperties properties) {
			return rect;
		}
		IFloatingObjectLocation CreateRotatedFloatingObjectLocation(IFloatingObjectLocation properties, int angle) {
			Matrix matrix = new Matrix();
			matrix.Rotate(DocumentModel.UnitConverter.ModelUnitsToDegreeF(-angle));
			Size actualSize = new Size();
			FloatingObjectProperties floatingObjectProperties = properties as FloatingObjectProperties;
			if (floatingObjectProperties != null)
				actualSize = floatingObjectProperties.ActualSize;
			FrameProperties frameProperties = properties as FrameProperties;
			if (frameProperties != null)
				actualSize = new Size(frameProperties.Width, frameProperties.Height);
			Rectangle boundingRectangle = RectangleUtils.BoundingRectangle(new Rectangle(Point.Empty, actualSize), matrix);
			Rectangle relativeBoundingRectangle = RectangleUtils.BoundingRectangle(new Rectangle(Point.Empty, new Size(properties.RelativeWidth.Width, properties.RelativeHeight.Height)), matrix);
			ExplicitFloatingObjectLocation result = new ExplicitFloatingObjectLocation();
			result.ActualWidth = boundingRectangle.Width;
			result.ActualHeight = boundingRectangle.Height;
			result.OffsetX = properties.OffsetX;
			result.OffsetY = properties.OffsetY;
			result.HorizontalPositionAlignment = properties.HorizontalPositionAlignment;
			result.VerticalPositionAlignment = properties.VerticalPositionAlignment;
			result.HorizontalPositionType = properties.HorizontalPositionType;
			result.VerticalPositionType = properties.VerticalPositionType;
			result.TextWrapType = properties.TextWrapType;
			if (properties.UseRelativeWidth) {
				result.UseRelativeWidth = true;
				result.RelativeWidth = new FloatingObjectRelativeWidth(properties.RelativeWidth.From, relativeBoundingRectangle.Width);
			}
			if (properties.UseRelativeHeight) {
				result.UseRelativeHeight = true;
				result.RelativeHeight = new FloatingObjectRelativeHeight(properties.RelativeHeight.From, relativeBoundingRectangle.Height);
			}
			if (floatingObjectProperties != null && floatingObjectProperties.UsePercentOffset) {
				result.PercentOffsetX = properties.PercentOffsetX;
				result.PercentOffsetY = properties.PercentOffsetY;
			}
			return result;
		}
		protected internal virtual Rectangle CalculateAbsoluteFloatingObjectShapeBounds(IFloatingObjectLocation location, Shape shape, bool isTextBox) {
			DocumentModelUnitToLayoutUnitConverter unitConverter = DocumentModel.ToDocumentLayoutUnitConverter;
			Rectangle bounds = CalculateAbsoluteFloatingObjectBounds(location);
			Point position = bounds.Location;
			Size size = bounds.Size;
			if (!DXColor.IsTransparentOrEmpty(shape.OutlineColor)) {
				if (isTextBox) {
					size.Height += shape.OutlineWidth;
				}
				else {
					int outlineWidth = unitConverter.ToLayoutUnits(shape.OutlineWidth);
					position.X -= outlineWidth;
					position.Y -= outlineWidth;
					size.Width += 2 * shape.OutlineWidth;
					size.Height += 2 * shape.OutlineWidth;
				}
			}
			int width = unitConverter.ToLayoutUnits(size.Width);
			int height = unitConverter.ToLayoutUnits(size.Height);
			return new Rectangle(position, new Size(width, height));
		}
		protected internal virtual Rectangle CalculateAbsoluteParagraphFrameShapeBounds(IParagraphFrameLocation location) {
			DocumentModelUnitToLayoutUnitConverter unitConverter = DocumentModel.ToDocumentLayoutUnitConverter;
			Rectangle bounds = CalculateAbsoluteParagraphFrameBounds(location);
			int width = unitConverter.ToLayoutUnits(bounds.Size.Width);
			int height = unitConverter.ToLayoutUnits(bounds.Size.Height);
			return new Rectangle(bounds.Location, new Size(width, height));
		}
		Rectangle CenterRectangleHorizontally(Rectangle rectangle, Rectangle where) {
			rectangle.X = where.X + (where.Width - rectangle.Width) / 2;
			return rectangle;
		}
		Rectangle CenterRectangleVertically(Rectangle rectangle, Rectangle where) {
			rectangle.Y = where.Y + (where.Height - rectangle.Height) / 2;
			return rectangle;
		}
		protected internal virtual Rectangle CalculateAbsoluteFloatingObjectContentBounds(IFloatingObjectLocation floatingObjectProperties, Shape shape, Rectangle shapeBounds) {
			if (DXColor.IsTransparentOrEmpty(shape.OutlineColor))
				return shapeBounds;
			DocumentModelUnitToLayoutUnitConverter unitConverter = DocumentModel.ToDocumentLayoutUnitConverter;
			int width = unitConverter.ToLayoutUnits(shape.OutlineWidth);
			Rectangle result = shapeBounds;
			result.X += width;
			result.Width -= 2 * width;
			result.Y += width;
			result.Height -= 2 * width;
			return result;
		}
		protected internal virtual Rectangle CalculateActualAbsoluteFloatingObjectBounds(FloatingObjectProperties floatingObjectProperties, Rectangle bounds) {
			DocumentModelUnitToLayoutUnitConverter unitConverter = DocumentModel.ToDocumentLayoutUnitConverter;
			bounds.X -= unitConverter.ToLayoutUnits(floatingObjectProperties.LeftDistance);
			bounds.Y -= unitConverter.ToLayoutUnits(floatingObjectProperties.TopDistance);
			bounds.Width += unitConverter.ToLayoutUnits(floatingObjectProperties.LeftDistance + floatingObjectProperties.RightDistance);
			bounds.Height += unitConverter.ToLayoutUnits(floatingObjectProperties.TopDistance + floatingObjectProperties.BottomDistance);
			return bounds;
		}
		protected internal virtual Rectangle CalculateAbsoluteFloatingObjectBounds(IFloatingObjectLocation location) {
			return new Rectangle(Point.Empty, new Size(location.ActualWidth, location.ActualHeight));
		}
		protected internal virtual Rectangle CalculateAbsoluteParagraphFrameBounds(IParagraphFrameLocation location) {
			return new Rectangle(Point.Empty, new Size(location.Width, location.Height));
		}
	}
	public class FloatingObjectSizeAndPositionController : FloatingObjectSizeController {
		readonly RowsController rowsController;
		public FloatingObjectSizeAndPositionController(RowsController rowsController)
			: base(rowsController.PieceTable) {
			this.rowsController = rowsController;
		}
		protected RowsController RowsController { get { return rowsController; } }
		protected Column CurrentColumn { get { return RowsController.CurrentColumn; } }
		protected IColumnController ColumnController { get { return RowsController.ColumnController; } }
		protected CurrentHorizontalPositionController HorizontalPositionController { get { return RowsController.HorizontalPositionController; } }
		protected Row CurrentRow { get { return RowsController.CurrentRow; } }
		protected int CurrentHorizontalPosition { get { return RowsController.CurrentHorizontalPosition; } }
		protected internal override Rectangle CalculateAbsoluteFloatingObjectBounds(IFloatingObjectLocation location) {
			FloatingObjectTargetPlacementInfo placementInfo = CalculatePlacementInfo(location);
			Size sz = new Size(CalculateAbsoluteFloatingObjectWidth(location, placementInfo), CalculateAbsoluteFloatingObjectHeight(location, placementInfo));
			Point pt = new Point(CalculateAbsoluteFloatingObjectX(location, placementInfo, sz.Width), CalculateAbsoluteFloatingObjectY(location, placementInfo, sz.Height));
			return new Rectangle(pt, sz);
		}
		protected internal override Rectangle CalculateAbsoluteParagraphFrameBounds(IParagraphFrameLocation location) {
			FloatingObjectTargetPlacementInfo placementInfo = CalculatePlacementInfo(null);
			Size sz = new Size(location.Width, location.Height);
			Point pt = new Point(CalculateAbsoluteParagraphFrameX(location, placementInfo, sz.Width), CalculateAbsoluteParagraphFrameY(location, placementInfo, sz.Height));
			return new Rectangle(pt, sz);
		}
		protected FloatingObjectTargetPlacementInfo CalculatePlacementInfo(IFloatingObjectLocation location) {
			FloatingObjectTargetPlacementInfo result = new FloatingObjectTargetPlacementInfo();
			Page page = ColumnController.PageAreaController.PageController.Pages.Last;
			result.PageBounds = (location != null && location.TextWrapType != FloatingObjectTextWrapType.None) ? ColumnController.GetCurrentPageBounds(page, CurrentColumn) : page.Bounds;
			result.PageClientBounds = (location != null && location.TextWrapType != FloatingObjectTextWrapType.None) ? ColumnController.GetCurrentPageClientBounds(page, CurrentColumn) : page.ClientBounds;
			result.ColumnBounds = HorizontalPositionController.CalculateFloatingObjectColumnBounds(CurrentColumn);
			result.OriginalColumnBounds = CurrentColumn.Bounds;
			result.OriginX = CurrentHorizontalPosition;
			result.OriginY = CurrentRow.Bounds.Y;
			return result;
		}
		protected internal virtual int CalculateAbsoluteFloatingObjectX(IFloatingObjectLocation location, FloatingObjectTargetPlacementInfo placementInfo, int actualWidth) {
			FloatingObjectHorizontalPositionCalculator calculator = new FloatingObjectHorizontalPositionCalculator(DocumentModel.ToDocumentLayoutUnitConverter);
			return calculator.CalculateAbsoluteFloatingObjectX(location, placementInfo, actualWidth);
		}
		protected internal virtual int CalculateAbsoluteFloatingObjectY(IFloatingObjectLocation location, FloatingObjectTargetPlacementInfo placementInfo, int actualHeight) {
			FloatingObjectVerticalPositionCalculator calculator = new FloatingObjectVerticalPositionCalculator(DocumentModel.ToDocumentLayoutUnitConverter);
			return calculator.CalculateAbsoluteFloatingObjectY(location, placementInfo, actualHeight);
		}
		protected internal virtual int CalculateAbsoluteFloatingObjectWidth(IFloatingObjectLocation location, FloatingObjectTargetPlacementInfo placementInfo) {
			FloatingObjectHorizontalPositionCalculator calculator = new FloatingObjectHorizontalPositionCalculator(DocumentModel.ToDocumentLayoutUnitConverter);
			return calculator.CalculateAbsoluteFloatingObjectWidth(location, placementInfo);
		}
		protected internal virtual int CalculateAbsoluteFloatingObjectHeight(IFloatingObjectLocation location, FloatingObjectTargetPlacementInfo placementInfo) {
			FloatingObjectVerticalPositionCalculator calculator = new FloatingObjectVerticalPositionCalculator(DocumentModel.ToDocumentLayoutUnitConverter);
			return calculator.CalculateAbsoluteFloatingObjectHeight(location, placementInfo);
		}
		protected internal virtual int CalculateAbsoluteParagraphFrameX(IParagraphFrameLocation location, FloatingObjectTargetPlacementInfo placementInfo, int actualWidth) {
			ParagraphFrameHorizontalPositionCalculator calculator = new ParagraphFrameHorizontalPositionCalculator(DocumentModel.ToDocumentLayoutUnitConverter);
			return calculator.CalculateAbsoluteFloatingObjectX(location, placementInfo, actualWidth);
		}
		protected internal virtual int CalculateAbsoluteParagraphFrameY(IParagraphFrameLocation location, FloatingObjectTargetPlacementInfo placementInfo, int actualHeight) {
			ParagraphFrameVerticalPositionCalculator calculator = new ParagraphFrameVerticalPositionCalculator(DocumentModel.ToDocumentLayoutUnitConverter);
			return calculator.CalculateAbsoluteFloatingObjectY(location, placementInfo, actualHeight);
		}
		protected override Rectangle ValidateRotatedShapeHorizontalPosition(Rectangle shapeBounds, FloatingObjectProperties properties) {
			if (properties.TextWrapType == FloatingObjectTextWrapType.None )
				return shapeBounds;
			Rectangle pageBounds = CalculatePlacementInfo(properties).PageBounds;
			int overflow = shapeBounds.X + shapeBounds.Width - pageBounds.Right;
			if (overflow > 0)
				shapeBounds.X -= overflow;
			shapeBounds.X = Math.Max(pageBounds.Left, shapeBounds.X);
			return shapeBounds;
		}
		protected override Rectangle ValidateRotatedShapeVerticalPosition(Rectangle shapeBounds, FloatingObjectProperties properties) {
			if (properties.TextWrapType == FloatingObjectTextWrapType.None )
				return shapeBounds;
			int height = shapeBounds.Height;
			int bottom = shapeBounds.Y + height;
			Rectangle pageBounds = CalculatePlacementInfo(properties).PageBounds;
			if (bottom >= pageBounds.Bottom)
				shapeBounds.Y -= bottom - pageBounds.Bottom;
			if (shapeBounds.Y < pageBounds.Top)
				shapeBounds.Y = pageBounds.Top;
			return shapeBounds;
		}
	}
	#endregion
	public class CurrentHorizontalPositionController {
		readonly RowsController rowsController;
		int currentHorizontalPosition;
		public CurrentHorizontalPositionController(RowsController rowsController)
			: this(rowsController, 0) {
		}
		public CurrentHorizontalPositionController(RowsController rowsController, int position) {
			Guard.ArgumentNotNull(rowsController, "rowsController");
			this.rowsController = rowsController;
			this.currentHorizontalPosition = position;
		}
		internal int CurrentHorizontalPosition { get { return currentHorizontalPosition; } }
		protected int InnerCurrentHorizontalPosition { get { return currentHorizontalPosition; } set { currentHorizontalPosition = value; } }
		public RowsController RowsController { get { return rowsController; } }
		protected internal virtual void IncrementCurrentHorizontalPosition(int delta) {
			this.currentHorizontalPosition += delta;
		}
		protected internal virtual void RollbackCurrentHorizontalPositionTo(int value) {
			Debug.Assert(value <= currentHorizontalPosition);
			this.currentHorizontalPosition = value;
		}
		protected internal virtual void SetCurrentRowInitialHorizontalPosition() {
			this.currentHorizontalPosition = rowsController.ParagraphLeft + rowsController.CurrentRowIndent;
		}
		protected internal virtual void OnCurrentRowHeightChanged(bool keepTextAreas) {
		}
		protected internal virtual Rectangle CalculateBoxBounds(BoxInfo boxInfo) {
			return new Rectangle(new Point(CurrentHorizontalPosition, 0), boxInfo.Size);
		}
		protected internal virtual CanFitCurrentRowToColumnResult CanFitCurrentRowToColumn(int newRowHeight) {
			return RowsController.TablesController.CanFitRowToColumn(RowsController.CurrentRow.Bounds.Top + newRowHeight, RowsController.CurrentColumn);
		}
		protected internal virtual bool CanFitBoxToCurrentRow(Size boxSize) {
			return CurrentHorizontalPosition + boxSize.Width <= rowsController.CurrentRow.Bounds.Right || rowsController.SuppressHorizontalOverfull;
		}
		protected internal virtual int GetMaxBoxWidth() {
			if (rowsController.SuppressHorizontalOverfull)
				return Int32.MaxValue / 2;
			else
				return Math.Max(1, rowsController.CurrentRow.Bounds.Right - CurrentHorizontalPosition);
		}
		protected internal virtual bool CanFitNumberingListBoxToCurrentRow(Size boxSize) {
			return true;
		}
		protected internal virtual void SetCurrentHorizontalPosition(int value) {
			this.currentHorizontalPosition = value;
		}
		protected internal virtual void OnCurrentRowFinished() {
		}
		protected internal virtual int MoveCurrentRowDownToFitTable(int tableWidth, int tableTop) {
			return tableTop;
		}
		public virtual TextArea GetTextAreaForTable() {
			Rectangle bounds = RowsController.CurrentColumn.Bounds;
			return new TextArea(bounds.Left, bounds.Right);
		}
		protected internal virtual Rectangle CalculateFloatingObjectColumnBounds(Column currentColumn) {
			return currentColumn.Bounds;
		}
	}
	public class FloatingObjectsCurrentHorizontalPositionController : CurrentHorizontalPositionController {
		List<TextArea> textAreas;
		int currentTextAreaIndex;
		int lastRowHeight;
		int minLeftArea;
		int minArea;
		public FloatingObjectsCurrentHorizontalPositionController(RowsController rowsController)
			: base(rowsController) {
			minLeftArea = rowsController.DocumentModel.LayoutUnitConverter.TwipsToLayoutUnits(180);
			minArea = rowsController.DocumentModel.LayoutUnitConverter.TwipsToLayoutUnits(144);
		}
		public FloatingObjectsCurrentHorizontalPositionController(RowsController rowsController, int position)
			: base(rowsController, position) {
			minLeftArea = rowsController.DocumentModel.LayoutUnitConverter.TwipsToLayoutUnits(180);
			minArea = rowsController.DocumentModel.LayoutUnitConverter.TwipsToLayoutUnits(144);
		}
		protected internal override void OnCurrentRowHeightChanged(bool keepTextAreas) {
			Rectangle currentRowBounds = RowsController.CurrentRow.Bounds;
			OnCurrentRowHeightChanged(currentRowBounds.Height, currentRowBounds, keepTextAreas, false);
		}
		bool OnCurrentRowHeightChanged(int height, Rectangle currentRowBounds, bool keepTextAreas, bool ignoreFloatingObjects) {
			FloatingObjectsLayout layout = RowsController.FloatingObjectsLayout;
			currentRowBounds.Height = height;
			IList<FloatingObjectBox> floatingObjects = ignoreFloatingObjects ? new List<FloatingObjectBox>() : layout.GetObjectsInRectangle(currentRowBounds);
			ParagraphFramesLayout paragraphFramesLayout = RowsController.ParagraphFramesLayout;
			IList<ParagraphFrameBox> paragraphFrames = ignoreFloatingObjects ? new List<ParagraphFrameBox>() : paragraphFramesLayout.GetObjectsInRectangle(currentRowBounds);
			List<TextArea> prevTextAreas = this.textAreas;
			if (prevTextAreas == null || !keepTextAreas)
				this.textAreas = CalculateTextAreas(paragraphFrames, floatingObjects, currentRowBounds);
			this.lastRowHeight = currentRowBounds.Height;
			if (textAreas.Count > 0)
				InnerCurrentHorizontalPosition = Math.Max(InnerCurrentHorizontalPosition, textAreas[0].Start);
			UpdateCurrentTextAreaIndex();
			return AreTextAreasChanged(prevTextAreas, textAreas);
		}
		public List<TextArea> CalculateTextAreas(IList<ParagraphFrameBox> paragraphFrameItems, IList<FloatingObjectBox> floatingObjectItems, Rectangle bounds) {
			List<ParagraphFrameBox> processedParagraphFrames = new List<ParagraphFrameBox>();
			List<FloatingObjectBox> processedFloatingObjects = new List<FloatingObjectBox>();
			TextAreaCollectionEx result = new TextAreaCollectionEx();
			int left = Math.Min(bounds.Left, bounds.Right);
			int right = Math.Max(left + 1, bounds.Right);
			result.Add(new TextArea(left, right));
			int framesCount = paragraphFrameItems.Count;
			for (int i = 0; i < framesCount; i++)
				RowsController.ParagraphFramesLayout.ProcessParagraphFrame(paragraphFrameItems[i], processedParagraphFrames, result, bounds);
			int floatingObjectsCount = floatingObjectItems.Count;
			for (int i = 0; i < floatingObjectsCount; i++)
				RowsController.FloatingObjectsLayout.ProcessFloatingObject(floatingObjectItems[i], processedFloatingObjects, result, bounds);
			result.Sort();
			List<TextArea> resultList = result.InnerList;
			bool keepInitialArea = resultList.Count == 1 && resultList[0].Start == left && resultList[0].End == right;
			if (!keepInitialArea)
				RemoveShortAreas(resultList, left, right);
			return resultList;
		}
		void RemoveShortAreas(List<TextArea> areas, int left, int right) {
			if (right - left <= Math.Min(minArea, minLeftArea))
				return;
			int count = areas.Count;
			for (int i = count - 1; i >= 0; i--) {
				TextArea area = areas[i];
				int length = area.End - area.Start;
				if ((area.Start == left && length < this.minLeftArea) || (area.Start != left && length < minArea))
					areas.RemoveAt(i);
			}
		}
		bool AreTextAreasChanged(List<TextArea> prevTextAreas, List<TextArea> textAreas) {
			if (prevTextAreas == null)
				return true;
			int count = textAreas.Count;
			if (count != prevTextAreas.Count)
				return true;
			for (int i = 0; i < count; i++) {
				if (prevTextAreas[i].Start != textAreas[i].Start || prevTextAreas[i].End != textAreas[i].End)
					return true;
			}
			return false;
		}
		protected internal override void SetCurrentRowInitialHorizontalPosition() {
			if (textAreas.Count > 0) {
				InnerCurrentHorizontalPosition = textAreas[0].Start;
				currentTextAreaIndex = 0;
			}
			else {
				base.SetCurrentRowInitialHorizontalPosition();
				UpdateCurrentTextAreaIndex();
			}
		}
		protected internal override void IncrementCurrentHorizontalPosition(int delta) {
			base.IncrementCurrentHorizontalPosition(delta);
			InnerCurrentHorizontalPosition = UpdateCurrentTextAreaIndex();
		}
		protected internal override void RollbackCurrentHorizontalPositionTo(int value) {
			base.RollbackCurrentHorizontalPositionTo(value);
			UpdateCurrentTextAreaIndex();
		}
		protected internal override void SetCurrentHorizontalPosition(int value) {
			base.SetCurrentHorizontalPosition(value);
			UpdateCurrentTextAreaIndex();
		}
		int UpdateCurrentTextAreaIndex() {
			int newTextAreaIndex = Algorithms.BinarySearch(textAreas, new TextAreaAndXComparable(InnerCurrentHorizontalPosition));
			if (newTextAreaIndex < 0) {
				newTextAreaIndex = ~newTextAreaIndex;
				if (newTextAreaIndex >= textAreas.Count)
					return InnerCurrentHorizontalPosition;
			}
			if (this.currentTextAreaIndex != newTextAreaIndex) {
				this.currentTextAreaIndex = newTextAreaIndex;
				return textAreas[currentTextAreaIndex].Start;
			}
			else
				return InnerCurrentHorizontalPosition;
		}
		protected internal override int MoveCurrentRowDownToFitTable(int tableWidth, int tableTop) {
			int maxY = this.RowsController.CurrentColumn.Bounds.Bottom;
			Rectangle topTableRowBounds = RowsController.CurrentColumn.Bounds;
			topTableRowBounds.Height = 0;
			topTableRowBounds.Y = tableTop;
			for (int y = tableTop; y <= maxY; y++) {
				bool textAreasRecreated = OnCurrentRowHeightChanged(0, topTableRowBounds, false, false);
				if (textAreasRecreated || y == tableTop) {
					bool tableFit = textAreas.Count == 1 && textAreas[0].Width >= tableWidth;
					if (tableFit)
						return topTableRowBounds.Y;
				}
				topTableRowBounds.Y++;
			}
			topTableRowBounds.Y = tableTop;
			OnCurrentRowHeightChanged(0, topTableRowBounds, false, true);
			return tableTop;
		}
		public override TextArea GetTextAreaForTable() {
			if (textAreas == null || textAreas.Count < 1) {
				Debug.Assert(false);
				return base.GetTextAreaForTable();
			}
			else
				return textAreas[0];
		}
		protected internal override CanFitCurrentRowToColumnResult CanFitCurrentRowToColumn(int newRowHeight) {
			if (newRowHeight <= lastRowHeight && textAreas.Count > 0)
				return base.CanFitCurrentRowToColumn(newRowHeight);
			Rectangle currentRowBounds = RowsController.CurrentRow.Bounds;
			bool textAreasRecreated = OnCurrentRowHeightChanged(newRowHeight, currentRowBounds, false, false);
			for (; ; ) {
				if (textAreas.Count > 0) {
					RowsController.CurrentRow.Bounds = currentRowBounds;
					if (textAreasRecreated)
						return CanFitCurrentRowToColumnResult.TextAreasRecreated;
					return base.CanFitCurrentRowToColumn(newRowHeight);
				}
				currentRowBounds.Y++;
				currentRowBounds.Height = RowsController.DefaultRowHeight;
				if (currentRowBounds.Bottom > RowsController.CurrentColumn.Bounds.Bottom) {
					if (RowsController.CurrentColumn.Rows.Count == 0)
						textAreasRecreated = OnCurrentRowHeightChanged(newRowHeight, currentRowBounds, false, true);
					RowsController.CurrentRow.Bounds = currentRowBounds;
					if (textAreasRecreated)
						return CanFitCurrentRowToColumnResult.TextAreasRecreated;
					else
						return base.CanFitCurrentRowToColumn(newRowHeight);
				}
				bool recreated = OnCurrentRowHeightChanged(newRowHeight, currentRowBounds, false, false);
				textAreasRecreated = textAreasRecreated || recreated;
			}
		}
		protected internal override bool CanFitNumberingListBoxToCurrentRow(Size boxSize) {
			if (RowsController.SuppressHorizontalOverfull)
				return true;
			if (textAreas.Count <= 0)
				return false;
			return CurrentHorizontalPosition + boxSize.Width <= textAreas[currentTextAreaIndex].End;
		}
		protected internal override bool CanFitBoxToCurrentRow(Size boxSize) {
			if (RowsController.SuppressHorizontalOverfull)
				return true;
			if (textAreas.Count <= 0)
				return false;
			if (CurrentHorizontalPosition + boxSize.Width <= textAreas[currentTextAreaIndex].End)
				return true;
			int count = textAreas.Count;
			for (int i = currentTextAreaIndex + 1; i < count; i++) {
				if (textAreas[i].Width > boxSize.Width) {
					return true;
				}
			}
			return false;
		}
		protected internal override int GetMaxBoxWidth() {
			if (RowsController.SuppressHorizontalOverfull)
				return Int32.MaxValue / 2;
			if (textAreas.Count <= 0)
				return 0;
			int maxWidth = textAreas[currentTextAreaIndex].End - CurrentHorizontalPosition;
			int count = textAreas.Count;
			for (int i = currentTextAreaIndex + 1; i < count; i++) {
				maxWidth = Math.Max(maxWidth, textAreas[i].Width);
			}
			return maxWidth;
		}
		protected internal override Rectangle CalculateBoxBounds(BoxInfo boxInfo) {
			if (textAreas.Count <= 0) {
				CanFitCurrentRowToColumn(boxInfo.Size.Height);
				if (!(boxInfo.Box is SpaceBoxa) && !(boxInfo.Box is SingleSpaceBox))
					RowsController.UpdateCurrentRowHeight(boxInfo);
				if (textAreas.Count <= 0)
					return base.CalculateBoxBounds(boxInfo);
				Debug.Assert(textAreas.Count > 0);
			}
			if (CurrentHorizontalPosition + boxInfo.Size.Width > textAreas[currentTextAreaIndex].End && ShouldBeMovedToNextTextArea(boxInfo.Box)) {
				int count = textAreas.Count;
				for (int i = currentTextAreaIndex + 1; i < count; i++) {
					if (textAreas[i].Width > boxInfo.Size.Width) {
						InnerCurrentHorizontalPosition = textAreas[i].Start;
						currentTextAreaIndex = i;
						break;
					}
				}
			}
			return base.CalculateBoxBounds(boxInfo);
		}
		protected void AppendRowBoxRange(RowBoxRangeCollection boxRanges, int firstBoxIndex, int lastBoxIndex, int textAreaIndex) {
			Row currentRow = RowsController.CurrentRow;
			if (textAreaIndex < 0)
				textAreaIndex = Math.Min(textAreas.Count - 1, ~textAreaIndex);
			int left = textAreas[textAreaIndex].Start;
			int right = textAreas[textAreaIndex].End;
			Rectangle currentRowBounds = currentRow.Bounds;
			Rectangle bounds = new Rectangle(left, currentRowBounds.Top, right - left, currentRowBounds.Height);
			boxRanges.Add(new RowBoxRange(firstBoxIndex, lastBoxIndex, bounds));
		}
		protected internal override void OnCurrentRowFinished() {
			if (textAreas.Count <= 0)
				return;
			Row currentRow = RowsController.CurrentRow;
			if (textAreas.Count == 1) {
				TryCreateDefaultBoxRange(currentRow);
				return;
			}
			BoxCollection boxes = currentRow.Boxes;
			int count = boxes.Count;
			if (count <= 0)
				return;
			int firstBoxIndex = 0;
			int textAreaIndex = Algorithms.BinarySearch(textAreas, new TextAreaAndXComparable(boxes[firstBoxIndex].Bounds.X));
			for (int i = 1; i < count; i++) {
				if (boxes[i - 1].Bounds.Right != boxes[i].Bounds.Left) {
					AppendRowBoxRange(currentRow.BoxRanges, firstBoxIndex, i - 1, textAreaIndex);
					firstBoxIndex = i;
					textAreaIndex = Algorithms.BinarySearch(textAreas, new TextAreaAndXComparable(boxes[i].Bounds.X));
				}
			}
			AppendRowBoxRange(currentRow.BoxRanges, firstBoxIndex, count - 1, textAreaIndex);
		}
		void TryCreateDefaultBoxRange(Row currentRow) {
			if (textAreas.Count == 1 && textAreas[0].Start == currentRow.Bounds.Left && textAreas[0].End == currentRow.Bounds.Right)
				return;
			AppendRowBoxRange(currentRow.BoxRanges, 0, currentRow.Boxes.Count - 1, 0);
		}
		protected internal virtual bool ShouldBeMovedToNextTextArea(Box box) {
			return !(
				box is ISpaceBox ||
				box is ParagraphMarkBox ||
				box is LineBreakBox ||
				box is SectionMarkBox ||
				box is PageBreakBox ||
				box is ColumnBreakBox);
		}
		public bool AdvanceHorizontalPositionToNextTextArea() {
			if (currentTextAreaIndex + 1 >= textAreas.Count)
				return false;
			currentTextAreaIndex++;
			InnerCurrentHorizontalPosition = textAreas[currentTextAreaIndex].Start;
			return true;
		}
	}
	public class TextAreaAndXComparable : IComparable<TextArea> {
		readonly int x;
		public TextAreaAndXComparable(int x) {
			this.x = x;
		}
		#region IComparable<TextArea> Members
		public int CompareTo(TextArea other) {
			if (x < other.Start)
				return 1;
			else if (x > other.Start) {
				if (x <= other.End)
					return 0;
				else
					return -1;
			}
			else
				return 0;
		}
		#endregion
	}
}
