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

using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
#if DXPORTABLE
using DevExpress.Compatibility.System.Drawing;
#else
using System.Drawing;
#endif
using System;
using System.Collections.Generic;
using DevExpress.XtraRichEdit.Services;
using System.Text;
using DevExpress.Office;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region RowFormatterFinalState
	public enum RowFormatterFinalState {
		None,
		RowEnd,
		ColumnEnd,
		SectionEnd,
		PageEnd,
		ParagraphEnd,
		DocumentEnd
	}
	#endregion
	#region RowFormattingResult
	public class RowFormattingResult {
		readonly RowFormatterFinalState finalState;
		readonly Row row;
		readonly RowFormatterState nextState;
		readonly List<FloatingObjectBox> floatingObjects;
		internal RowFormattingResult(Row row, RowFormatterFinalState finalState, RowFormatterState nextState, List<FloatingObjectBox> floatingObjects) {
			this.row = row;
			this.finalState = finalState;
			this.nextState = nextState;
			this.floatingObjects = floatingObjects;
		}
		public Row Row { get { return row; } }
		public RowFormatterFinalState FinalState { get { return finalState; } }
		public RowFormatterState NextState { get { return nextState; } }
		public List<FloatingObjectBox> FloatingObjects { get { return floatingObjects; } }
	}
	#endregion
	#region RowFormatter
	public class RowFormatter {
		static readonly FormatterPosition InvalidPosition = new FormatterPosition(new RunIndex(-1), -1, -1);
		readonly ILayoutBoxIterator iterator;
		readonly TabsController tabsController;
		RowFormatterStateBase state;
		Row currentRow;
		int availableRowWidth;
		int startX;
		int currentX;
		FormatterPosition wordStartPosition;
		FormatterPosition lastTabPosition;
		FormatterPosition lastBoxPosition;
		int boxCountBeforeWordStart;
		bool suppressHorizontalOverfull;
		LineSpacingCalculatorBase lineSpacingCalculator;
		int maxAscentAndFree;
		int maxPictureHeight;
		int maxDescent;
		int maxAscentBeforeWord;
		int maxDescentBeforeWord;
		int maxPictureHeightBeforeWord;
		int rowHeightBeforeWord;
		RunIndex runIndexBeforeWord;
		RowProcessingFlags rowProcessingFlagsBeforeWord;
		int lineNumber;
		int lineNumberStep = 1;
		RunIndex currentRunIndex;
		List<FloatingObjectBox> floatingObjects;
		public RowFormatter(ILayoutBoxIterator iterator) {
			Guard.ArgumentNotNull(iterator, "iterator");
			this.iterator = iterator;
			this.suppressHorizontalOverfull = false;
			this.tabsController = new TabsController();
			this.tabsController.PieceTable = iterator.PieceTable;
		}
		public ILayoutBoxIterator Iterator { get { return iterator; } }
		public PieceTable PieceTable { get { return Iterator.PieceTable; } }
		public DocumentModel DocumentModel { get { return PieceTable.DocumentModel; } }
		public int LineNumber { get { return lineNumber; } set { lineNumber = value; } }
		public int LineNumberStep { get { return lineNumberStep; } set { lineNumberStep = Math.Max(1, value); } }
		public bool SuppressHyphenation { get { return Iterator.SuppressHyphenation; } }
		internal bool SuppressHorizontalOverfull { get { return suppressHorizontalOverfull; } set { suppressHorizontalOverfull = value; } }
		internal Row CurrentRow { get { return this.currentRow; } }
		internal TabsController TabsController { get { return tabsController; } }
		Paragraph Paragraph { get { return CurrentRow != null ? CurrentRow.Paragraph : null; } }
		public RowFormattingResult Format(int x, int availableWidth, RowFormatterState initialState, TabFormattingInfo tabsInfo) {
			InitializeFormatter(x, availableWidth);
			InitializeCurrentRow(initialState);
			InitializeTabsController(tabsInfo);
			ChangeState(initialState);
			StartNewWord();
			while (!this.state.IsRowFormattingComplete() && !Iterator.IsEnd) {
				BoxInfo boxInfo = Iterator.GetNextBox();
				Debug.Assert(boxInfo != null);
				this.lastBoxPosition = boxInfo.StartPos;
				Box box = boxInfo.Box;
				box.Bounds = CalculateBoxBounds(boxInfo);
				box.Accept(this.state);
			}
			if (Iterator.IsEnd)
				EndRow(RowFormatterFinalState.DocumentEnd);
			return this.state.Result;
		}
		Rectangle CalculateBoxBounds(BoxInfo boxInfo) {
			return new Rectangle(new Point(this.currentX, 0), boxInfo.Size);
		}
		void InitializeFormatter(int x, int availableWidth) {
			this.lineSpacingCalculator = CreateLineSpacingCalculator();
			this.startX = x;
			this.currentX = x;
			this.availableRowWidth = availableWidth;
			this.maxAscentAndFree = 0;
			this.maxDescent = 0;
			this.maxPictureHeight = 0;
			this.suppressHorizontalOverfull = false;
			this.currentRunIndex = RunIndex.DontCare;
			this.lastTabPosition = InvalidPosition;
			this.lastBoxPosition = InvalidPosition;
			this.wordStartPosition = InvalidPosition;
			this.floatingObjects = new List<FloatingObjectBox>();
		}
		void InitializeTabsController(TabFormattingInfo tabsInfo) {
			this.tabsController.ClearLastTabPosition();
			this.tabsController.Tabs = tabsInfo;
			this.tabsController.ColumnLeft = this.startX;
			this.tabsController.ParagraphRight = this.startX + this.availableRowWidth;
			if (this.tabsController.TabsCount == 1 && this.tabsController[0].Alignment == TabAlignmentType.Decimal && Paragraph.IsInCell())
				AddSingleDecimalTabInTable();
		}
		void InitializeCurrentRow(RowFormatterState initialState) {
			this.currentRow = CreateRow();
			this.currentRow.Paragraph = Iterator.Paragraph;
			this.currentRow.InitialState = initialState;
			this.currentRow.Bounds = new Rectangle(this.startX, 0, 0, this.lineSpacingCalculator.DefaultRowHeight);
			this.currentRow.ProcessingFlags &= RowProcessingFlags.Clear;
			AssignCurrentRowLineNumber();
			if (initialState == RowFormatterState.ParagraphStart || initialState == RowFormatterState.SectionStart)
				ApplySpacingBefore();
			if (initialState == RowFormatterState.ParagraphStart)
				CurrentRow.ProcessingFlags |= RowProcessingFlags.FirstParagraphRow;
		}
		protected virtual Row CreateRow() {
			return new Row();
		}
		void ApplySpacingBefore() {
			int spacingBefore = CalculateSpacingBefore(Paragraph);
			bool invisibleEmptyParagraphInCellAfterNestedTable = ShouldIgnoreParagraphHeight(Paragraph);
			if (invisibleEmptyParagraphInCellAfterNestedTable) {
				CurrentRow.ProcessingFlags |= RowProcessingFlags.LastInvisibleEmptyCellRowAfterNestedTable;
				CurrentRow.Height = 0;
				spacingBefore = 0;
			}
			CurrentRow.SpacingBefore = spacingBefore;
		}
		void OffsetCurrentRow(int offset) {
			Debug.Assert(offset >= 0);
			Rectangle r = CurrentRow.Bounds;
			r.Offset(0, offset);
			CurrentRow.Bounds = r;
		}
		int CalculateSpacingBefore(Paragraph paragraph) {
			ParagraphIndex prevParagraphIndex = paragraph.Index - 1;
			int spacingAfterPrevParagraph = GetActualSpacingAfter(prevParagraphIndex);
			int spacingBefore = DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(Paragraph.ContextualSpacingBefore);
			if (spacingBefore < spacingAfterPrevParagraph)
				return 0;
			return spacingBefore - spacingAfterPrevParagraph;
		}
		int GetActualSpacingAfter(ParagraphIndex paragraphIndex) {
			if (paragraphIndex < ParagraphIndex.Zero)
				return 0;
			Paragraph paragraph = PieceTable.Paragraphs[paragraphIndex];
			if (!ShouldIgnoreParagraphHeight(paragraph))
				return GetContextualSpacingAfter();
			return 0;
		}
		bool ShouldIgnoreParagraphHeight(Paragraph paragraph) {
			TableCell currentCell = paragraph.GetCell();
			return DocumentModel.IsSpecialEmptyParagraphAfterInnerTable(paragraph, currentCell) && !DocumentModel.IsCursorInParagraph(paragraph);
		}
		int GetContextualSpacingAfter() {
			return DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(Paragraph.ContextualSpacingAfter);
		}
		void AddSingleDecimalTabInTable() {
			if (CurrentRow.Width >= this.availableRowWidth)
				this.suppressHorizontalOverfull = true;
			this.tabsController.SaveLastTabPosition(-1);
		}
		void AssignCurrentRowLineNumber() {
			if (!Paragraph.SuppressLineNumbers && !Paragraph.IsInCell() && (LineNumber % LineNumberStep) == 0)
				CurrentRow.LineNumber = LineNumber;
			else
				CurrentRow.LineNumber = -LineNumber;
		}
		Box GetBox(BoxInfo boxInfo) {
			return boxInfo.Box;
		}
		protected virtual LineSpacingCalculatorBase CreateLineSpacingCalculator() {
			return LineSpacingCalculatorBase.Create(Iterator.Paragraph);
		}
		public virtual void StartNewWord(FormatterPosition pos) {
			this.boxCountBeforeWordStart = CurrentRow.Boxes.Count;
			this.wordStartPosition = pos;
			this.rowHeightBeforeWord = this.currentRow.Height;
			this.maxAscentBeforeWord = this.maxAscentAndFree;
			this.maxPictureHeightBeforeWord = this.maxPictureHeight;
			this.maxDescentBeforeWord = this.maxDescent;
			this.rowProcessingFlagsBeforeWord = CurrentRow.ProcessingFlags;
			this.runIndexBeforeWord = this.currentRunIndex;
		}
		public void StartNewWord() {
			StartNewWord(Iterator.Position);
		}
		public void StartNewTab(FormatterPosition position) {
			this.lastTabPosition = position;
		}
		public virtual void ResetToWordStart() {
			Iterator.Position = this.wordStartPosition;
			this.currentRow.Height = this.rowHeightBeforeWord;
			this.maxAscentAndFree = this.maxAscentBeforeWord;
			this.maxPictureHeight = this.maxPictureHeightBeforeWord;
			this.maxDescent = this.maxDescentBeforeWord;
			CurrentRow.ProcessingFlags = this.rowProcessingFlagsBeforeWord;
			this.currentRunIndex = this.runIndexBeforeWord;
			BoxCollection boxes = this.currentRow.Boxes;
			this.currentRow.Boxes.RemoveRange(this.boxCountBeforeWordStart, boxes.Count - this.boxCountBeforeWordStart);
			int count = boxes.Count;
			int x;
			if (count > 0)
				x = boxes[count - 1].Bounds.Right;
			else if (CurrentRow.NumberingListBox == null)
				x = this.startX;
			else
				x = CurrentRow.NumberingListBox.Bounds.Right;
			CurrentRow.Width -= this.currentX - x;
			this.currentX = x;
			this.tabsController.UpdateLastTabPosition(count);
		}
		void TryToRemoveLastTabBox() {
			BoxCollection boxes = CurrentRow.Boxes;
			int index = boxes.Count - 1;
#if DEBUGTEST
			Debug.Assert(index >= 0);
#endif
			TabSpaceBox tab = boxes[index] as TabSpaceBox;
			if (tab == null)
				return;
			int x = tab.Bounds.X;
			CurrentRow.Width -= this.currentX - x;
			this.currentX = x;
			boxes.RemoveAt(index);
			this.tabsController.UpdateLastTabPosition(index);
		}
		public void ResetToBoxStart(Box box) {
			Iterator.Position = box.StartPos;
		}
		public void EndRow(RowFormatterFinalState finalState, RowFormatterState nextState) {
			if (!Paragraph.SuppressLineNumbers && !CurrentRow.IsTabelCellRow)
				LineNumber++;
			int descent;
			if (ShouldUseMaxDescent())
				descent = this.maxDescent;
			else
				descent = 0;
			Debug.Assert(CurrentRow.Boxes.Count > 0);
			if (CurrentRow.Height == 0)
				UpdateCurrentRowHeight(CurrentRow.Boxes.Last, true);
			int lineSpacing = this.lineSpacingCalculator.CalculateSpacing(CurrentRow.Height, this.maxAscentAndFree, descent, this.maxPictureHeight);
			Rectangle r = CurrentRow.Bounds;
			int spaceBefore = CurrentRow.SpacingBefore;
			r.Y -= spaceBefore;
			r.Height = lineSpacing + spaceBefore;
			CurrentRow.Bounds = r;
			CurrentRow.NextState = nextState;
			if (finalState == RowFormatterFinalState.ParagraphEnd) {
				CurrentRow.ProcessingFlags |= RowProcessingFlags.LastParagraphRow;
				ApplySpacingAfter();
			}
			this.state = new EndRowState(this, new RowFormattingResult(CurrentRow, finalState, nextState, this.floatingObjects));
		}
		void ApplySpacingAfter() {
			int spacingAfter = GetContextualSpacingAfter();
			bool invisibleEmptyParagraphInCellAfterNestedTable = ShouldIgnoreParagraphHeight(CurrentRow.Paragraph);
			if (invisibleEmptyParagraphInCellAfterNestedTable)
				spacingAfter = 0;
			CurrentRow.LastParagraphRowOriginalHeight = CurrentRow.Height;
			IncreaseCurrentRowHeight(spacingAfter);
		}
		void IncreaseCurrentRowHeight(int delta) {
			Debug.Assert(delta >= 0);
			Rectangle r = CurrentRow.Bounds;
			r.Height += delta;
			CurrentRow.Bounds = r;
		}
		public void EndRow(RowFormatterFinalState result) {
			EndRow(result, RowFormatterState.EmptyRow);
		}
		public void EndRow() {
			EndRow(RowFormatterFinalState.RowEnd);
		}
		bool ShouldUseMaxDescent() {
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
		internal void UpdateCurrentRowHeight(Box box, bool forceUpdate) {
			RunIndex runIndex = box.StartPos.RunIndex;
			if (runIndex == this.currentRunIndex && !forceUpdate)
				return;
			if (!forceUpdate)
				this.currentRunIndex = runIndex;
			UpdateMaxAscentAndDescent(box);
			this.currentRow.Height = this.lineSpacingCalculator.CalcRowHeight(this.currentRow.Height, this.maxAscentAndFree + this.maxDescent);
		}
		internal void UpdateCurrentRowHeight(Box box) {
			UpdateCurrentRowHeight(box, false);
		}
		void UpdateMaxAscentAndDescent(Box box) {
			this.maxAscentAndFree = Math.Max(maxAscentAndFree, GetFontAscentAndFree(box));
			this.maxDescent = Math.Max(maxDescent, GetFontDescent(box));
		}
		protected virtual int GetFontAscentAndFree(Box box) {
			return box.GetFontInfo(PieceTable).GetBaseAscentAndFree(DocumentModel);
		}
		protected virtual int GetFontDescent(Box box) {
			return box.GetFontInfo(PieceTable).GetBaseDescent(DocumentModel);
		}
		public void ChangeState(RowFormatterState state) {
			this.state = CreateState(state);
		}
		internal RowFormatterStateBase CreateState(RowFormatterState state) {
			switch (state) {
				case RowFormatterState.DocumentStart:
				case RowFormatterState.PageStart:
				case RowFormatterState.SectionStart:
				case RowFormatterState.EmptyRow:
					return new EmptyRowState(this);
				case RowFormatterState.EmptyRowFirstSyllable:
					return new EmptyRowFirstSyllableState(this);
				case RowFormatterState.RowWithTextOnly:
					return new RowWithTextOnlyState(this);
				case RowFormatterState.RowWithTextRemainedSyllables:
					return new RowWithTextRemainedSyllablesState(this);
				case RowFormatterState.RowEndedWithHyphen:
					return new RowEndedWithHyphenState(this);
				case RowFormatterState.RowBreakAfterParagraphStart:
				case RowFormatterState.ParagraphStart:
					return new ParagraphStartState(this);
				case RowFormatterState.ParagraphEnd:
					return new ParagraphEndState(this);
				case RowFormatterState.RowWithSpacesOnly:
					return new RowWithSpacesOnlyState(this);
				case RowFormatterState.RowEndedWithSpaces:
					return new RowEndedWithSpacesState(this);
				case RowFormatterState.RowEndedWithText:
					return new RowEndedWithTextState(this);
				case RowFormatterState.RowEndedWithTextFirstSyllable:
					return new RowEndedWithTextFirstSyllableState(this);
				default:
					Exceptions.ThrowArgumentException("state", state);
					return null;
			}
		}
		public bool TryAddTextBox(Box box, bool wordStart) {
			if (CanAddBox(box)) {
				if (wordStart)
					StartNewWord(box.StartPos);
				AddTextBox(box);
				return true;
			}
			return false;
		}
		public bool CanAddBox(Box box) {
			return this.suppressHorizontalOverfull || CurrentRow.Width + box.Bounds.Width <= this.availableRowWidth;
		}
		public void AddTextBox(Box box) {
			AddBox(box);
			UpdateCurrentRowHeight(box);
		}
		public void AddTextBoxByChars(Box box, bool canExceedRowBounds) {
			int maxWidth = this.availableRowWidth - CurrentRow.Width;
			ResetToBoxStart(box);
			BoxInfo boxInfo = Iterator.GetNextBox(maxWidth, canExceedRowBounds);
			if (boxInfo != null) {
				boxInfo.Box.Bounds = CalculateBoxBounds(boxInfo);
				AddTextBox(boxInfo.Box);
			}
		}
		public void AddTextBoxByChars(Box box) {
			AddTextBoxByChars(box, false);
		}
		protected internal virtual void AddTabBox(TabInfo tab, TabSpaceBox box) {
			int newPosition;
			if (tab.Alignment == TabAlignmentType.Left) {
				int tabPosition = tab.GetLayoutPosition(DocumentModel.ToDocumentLayoutUnitConverter);
				newPosition = tabPosition + this.startX;
				Rectangle bounds = box.Bounds;
				bounds.Width = newPosition - this.currentX;
				box.Bounds = bounds;
			}
			else
				newPosition = this.currentX;
			if (newPosition > this.availableRowWidth && !tab.IsDefault)
				this.suppressHorizontalOverfull = true;
			tabsController.SaveLastTabPosition(CurrentRow.Boxes.Count);
			box.TabInfo = tab;
			box.LeaderCount = TabsController.CalculateLeaderCount(box, PieceTable);
			AddBox(box);
		}
		public bool TryAddTabBox(TabSpaceBox box) {
			TabInfo tabInfo = GetNextTab();
			int tabPosition = tabInfo.GetLayoutPosition(DocumentModel.ToDocumentLayoutUnitConverter);
			if (tabInfo.IsDefault && !SuppressHorizontalOverfull && tabPosition > this.availableRowWidth)
				return false;
			StartNewTab(box.StartPos);
			AddTabBox(tabInfo, box);
			this.lastTabPosition = box.StartPos;
			return true;
		}
		public void AddSpaceBox(Box box) {
			AddBox(box);
			ResetLastTabPosition();
		}
		protected void ResetLastTabPosition() {
			this.lastTabPosition = InvalidPosition;
		}
		public void ResetToLastTabStart() {
			ResetLastTabPosition();
			TryToRemoveLastTabBox();
		}
		TabInfo GetNextTab() {
			int offset = tabsController.CalcLastTabWidth(CurrentRow, Iterator.Measurer);
#if DEBUGTEST
			Debug.Assert(offset >= 0);
#endif
			this.currentX += offset;
			CurrentRow.Width -= offset;
			int position = DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(this.currentX - this.startX);
			int roundingFix = 0; 
			while (true) {
				TabInfo tabInfo = tabsController.GetNextTab(roundingFix + position);
				int layoutTabPosition = DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(tabInfo.Position);
				if (layoutTabPosition + this.startX > this.currentX)
					return tabInfo;
				else
					roundingFix++;
			}
		}
		public void AddBox(Box box) {
			CurrentRow.Boxes.Add(box);
			RunIndex runIndex = box.StartPos.RunIndex;
			TextRunBase run = PieceTable.Runs[runIndex];
			CurrentRow.ProcessingFlags |= run.RowProcessingFlags;
			this.currentX += box.Bounds.Width;
			CurrentRow.Width += box.Bounds.Width;
		}
		public virtual void TryAddNumberingListBox() {
			if (CurrentRow.NumberingListBox != null || !Paragraph.IsInList())
				return;
			NumberingListBox box = Paragraph.BoxCollection.NumberingListBox;
			CurrentRow.NumberingListBox = box;
			box.Bounds = box.InitialBounds;
			NumberingListIndex index = Paragraph.GetNumberingListIndex();
			int listLevelIndex = Paragraph.GetListLevelIndex();
			IListLevel listLevel = Paragraph.DocumentModel.NumberingLists[index].Levels[listLevelIndex];
			UpdateCurrentRowHeight(box);
			int left = CurrentRow.Bounds.Left;
			Rectangle bounds = CalculateNumberingListBoxBounds(box, listLevel.ListLevelProperties);
			CurrentRow.TextOffset = bounds.Right - left;
			box.Bounds = bounds;
			this.currentX = bounds.Right;
		}
		Rectangle CalculateNumberingListBoxBounds(NumberingListBox box, ListLevelProperties properties) {
			Rectangle bounds = box.Bounds;
			ListNumberAlignment alignment = properties.Alignment;
			int numberingListBoxLeft = CalculateNumberingListBoxLeft(bounds.Size.Width, alignment);
			bounds = new Rectangle(new Point(numberingListBoxLeft, 0), bounds.Size);
			NumberingListBoxWithSeparator boxWithSeparator = box as NumberingListBoxWithSeparator;
			if (boxWithSeparator != null) {
				if (boxWithSeparator.SeparatorBox is ISpaceBox)
					bounds.Width += boxWithSeparator.SeparatorBox.Bounds.Width;
				else {
					Debug.Assert(boxWithSeparator.SeparatorBox is TabSpaceBox);
					int separatorWidth = properties.Legacy ? GetLegacyNumberingListBoxSeparatorWidth(boxWithSeparator, bounds, properties) : GetNumberingListBoxSeparatorWidth(boxWithSeparator, bounds, properties);
					Rectangle separatorBounds = boxWithSeparator.SeparatorBox.Bounds;
					separatorBounds.Width = separatorWidth;
					separatorBounds.X = bounds.Right;
					bounds.Width += separatorWidth;
					boxWithSeparator.SeparatorBox.Bounds = separatorBounds;
				}
			}
			return bounds;
		}
		int CalculateNumberingListBoxLeft(int boxWidth, ListNumberAlignment alignment) {
			int boundsLeft = this.currentX;
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
		int GetLegacyNumberingListBoxSeparatorWidth(NumberingListBoxWithSeparator box, Rectangle bounds, ListLevelProperties properties) {
			int startHorizontalPosition = this.currentX;
			this.currentX = bounds.Right;
			int legacySpace = DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(properties.LegacySpace);
			int legacyIndent = DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(properties.LegacyIndent);
			int textRight = Math.Max(startHorizontalPosition + legacyIndent, bounds.Right + legacySpace);
			Paragraph paragraph = box.GetRun(PieceTable).Paragraph;
			if (paragraph.FirstLineIndentType == ParagraphFirstLineIndent.Hanging)
				textRight = Math.Max(textRight, DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(paragraph.FirstLineIndent) + startHorizontalPosition);
			return Math.Max(textRight - this.currentX, 0);
		}
		int GetNumberingListBoxSeparatorWidth(NumberingListBoxWithSeparator box, Rectangle bounds, ListLevelProperties properties) {
			this.currentX = bounds.Right;
			TabInfo tabInfo = GetNextTab();
			Paragraph paragraph = box.GetRun(PieceTable).Paragraph;
			TabInfo actualTabInfo = GetActualTabInfo(paragraph, tabInfo, properties);
			int tabRight = actualTabInfo.GetLayoutPosition(DocumentModel.ToDocumentLayoutUnitConverter);
			int tabLeft = bounds.Right - this.startX;
			int separatorWidth = tabRight - tabLeft;
			if (separatorWidth < 0) {
				tabRight = tabInfo.GetLayoutPosition(DocumentModel.ToDocumentLayoutUnitConverter);
				separatorWidth = tabRight - tabLeft;
			}
			return separatorWidth;
		}
		TabInfo GetActualTabInfo(Paragraph paragraph, TabInfo tabInfo, ListLevelProperties properties) {
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
		public virtual void HyphenateNextWord() {
			Iterator.HyphenateNextWord();
		}
		public bool AddFloatingObjectBox(FloatingObjectAnchorBox box) {
			TextRunBase currentRun = PieceTable.Runs[box.StartPos.RunIndex];
			FloatingObjectAnchorRun objectAnchorRun = currentRun as FloatingObjectAnchorRun;
			if (objectAnchorRun.ExcludeFromLayout)
				return false;
			FloatingObjectBox floatingObjectBox = new FloatingObjectBox();
			AddFloatingObject(box, floatingObjectBox);
			if (objectAnchorRun.FloatingObjectProperties.TextWrapType != FloatingObjectTextWrapType.None)
				floatingObjectBox.LockPosition = true;
			return true;
		}
		void ApplyFloatingObjectBoxBounds(FloatingObjectBox floatingObjectBox, FloatingObjectAnchorBox anchorBox) {
			floatingObjectBox.ExtendedBounds = anchorBox.ActualBounds;
			floatingObjectBox.Bounds = anchorBox.ShapeBounds;
			floatingObjectBox.ContentBounds = anchorBox.ContentBounds;
			floatingObjectBox.SetActualSizeBounds(anchorBox.ActualSizeBounds);
		}
		void AddFloatingObject(FloatingObjectAnchorBox anchorBox, FloatingObjectBox floatingObjectBox) {
			TextRunBase currentRun = PieceTable.Runs[anchorBox.StartPos.RunIndex];
			FloatingObjectAnchorRun objectAnchorRun = currentRun as FloatingObjectAnchorRun;
			floatingObjectBox.StartPos = anchorBox.StartPos;
			ApplyFloatingObjectBoxProperties(floatingObjectBox, objectAnchorRun.FloatingObjectProperties);
			anchorBox.SetFloatingObjectRun(objectAnchorRun);
			ApplyFloatingObjectBoxBounds(floatingObjectBox, anchorBox);
			this.floatingObjects.Add(floatingObjectBox);
		}
		void ApplyFloatingObjectBoxProperties(FloatingObjectBox floatingObjectBox, FloatingObjectProperties floatingObjectProperties) {
			floatingObjectBox.CanPutTextAtLeft = floatingObjectProperties.CanPutTextAtLeft;
			floatingObjectBox.CanPutTextAtRight = floatingObjectProperties.CanPutTextAtRight;
			floatingObjectBox.PutTextAtLargestSide = floatingObjectProperties.PutTextAtLargestSide;
			floatingObjectBox.PieceTable = floatingObjectProperties.PieceTable;
		}
	}
	#endregion
	#region RowFormatterState
	public enum RowFormatterState {
		EmptyRow,
		EmptyRowFirstSyllable,
		ParagraphStart,
		PageStart,
		SectionStart,
		DocumentStart,
		RowWithSpacesOnly,
		RowEndedWithSpaces,
		RowEndedWithText,
		RowEndedWithTextFirstSyllable,
		RowWithTextOnly,
		RowWithTextRemainedSyllables,
		RowBreakAfterParagraphStart,
		ParagraphEnd,
		RowEnd,
		RowEndedWithHyphen
	}
	#endregion
	#region RowFormatterStateBase
	public abstract class RowFormatterStateBase : IRowBoxesVisitor {
		readonly RowFormatter rowFormatter;
		protected RowFormatterStateBase(RowFormatter rowFormatter) {
			Guard.ArgumentNotNull(rowFormatter, "rowFormatter");
			this.rowFormatter = rowFormatter;
		}
		public virtual RowFormattingResult Result { get { return null; } }
		public RowFormatter RowFormatter { get { return rowFormatter; } }
		public virtual bool IsRowFormattingComplete() {
			return false;
		}
		public virtual void AddSectionBox(Box box) {
			RowFormatter.AddBox(box);
			EndRow(RowFormatterFinalState.SectionEnd, RowFormatterState.SectionStart);
		}
		public virtual void AddParagraphMarkBox(Box box) {
			RowFormatter.AddBox(box);
			RowFormatter.ChangeState(RowFormatterState.ParagraphEnd);
		}
		public virtual void AddLineBreakBox(Box box) {
			RowFormatter.AddBox(box);
			EndRow(RowFormatterFinalState.RowEnd, RowFormatterState.EmptyRow);
		}
		public virtual void AddPageBreakBox(Box box) {
			RowFormatter.AddBox(box);
			EndRow(RowFormatterFinalState.PageEnd, RowFormatterState.EmptyRow);
		}
		public virtual void AddColumnBreakBox(Box box) {
			RowFormatter.AddBox(box);
			EndRow(RowFormatterFinalState.ColumnEnd, RowFormatterState.EmptyRow);
		}
		protected virtual void EndRow(RowFormatterFinalState result, RowFormatterState nextState) {
			RowFormatter.EndRow(result, nextState);
		}
		public virtual void AddSpaceBox(Box box) {
			RowFormatter.AddSpaceBox(box);
		}
		public virtual void AddTabSpaceBox(Box box) {
		}
		public virtual void AddDashBox(Box box) {
		}
		public virtual void AddTextBox(Box box) {
		}
		public virtual void AddInlinePictureBox(Box box) {
		}
		public virtual void AddFloatingObjectAnchorBox(FloatingObjectAnchorBox box) {
			RowFormatter.AddFloatingObjectBox(box);
		}
		public virtual void AddLayoutDependentTextBox(LayoutDependentTextBox box) {
			AddTextBox(box);
		}
		public virtual void AddHyphenBox(Box box) {
		}
		#region IRowBoxesVisitor implementation
		void IRowBoxesVisitor.Visit(SectionMarkBox box) {
			AddSectionBox(box);
		}
		void IRowBoxesVisitor.Visit(ParagraphMarkBox box) {
			AddParagraphMarkBox(box);
		}
		void IRowBoxesVisitor.Visit(LineBreakBox box) {
			AddLineBreakBox(box);
		}
		void IRowBoxesVisitor.Visit(PageBreakBox box) {
			AddPageBreakBox(box);
		}
		void IRowBoxesVisitor.Visit(ColumnBreakBox box) {
			AddColumnBreakBox(box);
		}
		void IRowBoxesVisitor.Visit(SpaceBoxa box) {
			AddSpaceBox(box);
		}
		void IRowBoxesVisitor.Visit(SingleSpaceBox box) {
			AddSpaceBox(box);
		}
		void IRowBoxesVisitor.Visit(TabSpaceBox box) {
			AddTabSpaceBox(box);
		}
		void IRowBoxesVisitor.Visit(DashBox box) {
			AddDashBox(box);
		}
		void IRowBoxesVisitor.Visit(TextBox box) {
			AddTextBox(box);
		}
		void IRowBoxesVisitor.Visit(InlinePictureBox box) {
			AddInlinePictureBox(box);
		}
		void IRowBoxesVisitor.Visit(FloatingObjectAnchorBox box) {
			AddFloatingObjectAnchorBox(box);
		}
		void IRowBoxesVisitor.Visit(LayoutDependentTextBox box) {
			AddLayoutDependentTextBox(box);
		}
		void IRowBoxesVisitor.Visit(HyphenBox box) {
			AddHyphenBox(box);
		}
		#endregion
	}
	#endregion
	#region EmptyRowState
	public class EmptyRowState : RowFormatterStateBase {
		public EmptyRowState(RowFormatter rowFormatter)
			: base(rowFormatter) {
		}
		public override void AddTextBox(Box box) {
			if (RowFormatter.TryAddTextBox(box, true))
				RowFormatter.ChangeState(RowFormatterState.RowWithTextOnly);
			else if (RowFormatter.SuppressHyphenation) {
				RowFormatter.AddTextBoxByChars(box, true);
				RowFormatter.ChangeState(RowFormatterState.RowEnd);
			}
			else {
				RowFormatter.ResetToWordStart();
				RowFormatter.HyphenateNextWord();
				RowFormatter.ChangeState(RowFormatterState.EmptyRowFirstSyllable);
			}
		}
		public override void AddSpaceBox(Box box) {
			RowFormatter.AddSpaceBox(box);
			RowFormatter.ChangeState(RowFormatterState.RowWithSpacesOnly);
		}
	}
	#endregion
	#region EmptyRowFirstSyllableState
	public class EmptyRowFirstSyllableState : EmptyRowState {
		public EmptyRowFirstSyllableState(RowFormatter rowFormatter)
			: base(rowFormatter) {
		}
		public override void AddTextBox(Box box) {
			if (!RowFormatter.TryAddTextBox(box, false)) {
				RowFormatter.AddTextBoxByChars(box, true);
				RowFormatter.EndRow(RowFormatterFinalState.RowEnd, RowFormatterState.EmptyRowFirstSyllable);
			}
		}
		public override void AddHyphenBox(Box box) {
			if (RowFormatter.CanAddBox(box)) {
				RowFormatter.StartNewWord();
				RowFormatter.ChangeState(RowFormatterState.RowWithTextRemainedSyllables);
			}
			else
				RowFormatter.EndRow(RowFormatterFinalState.RowEnd, RowFormatterState.EmptyRowFirstSyllable);
		}
	}
	#endregion
	#region RowWithTextRemainedSyllablesState
	public class RowWithTextRemainedSyllablesState : RowWithTextOnlyState {
		public RowWithTextRemainedSyllablesState(RowFormatter rowFormatter)
			: base(rowFormatter) {
		}
		public override void AddTextBox(Box box) {
			if (!RowFormatter.TryAddTextBox(box, false))
				EndSyllable();
		}
		public override void AddHyphenBox(Box box) {
			if (RowFormatter.CanAddBox(box))
				RowFormatter.StartNewWord();
			else
				EndSyllable();
		}
		void EndSyllable() {
			RowFormatter.ResetToWordStart();
			RowFormatter.ChangeState(RowFormatterState.RowEndedWithHyphen);
		}
	}
	#endregion
	#region RowWithTextOnlyState
	public class RowWithTextOnlyState : RowFormatterStateBase {
		public RowWithTextOnlyState(RowFormatter rowFormatter)
			: base(rowFormatter) {
		}
		public override void AddSpaceBox(Box box) {
			RowFormatter.AddSpaceBox(box);
			RowFormatter.ChangeState(RowFormatterState.RowEndedWithSpaces);
		}
	}
	#endregion
	#region RowEndedWithTextState
	public class RowEndedWithTextState : RowFormatterStateBase {
		public RowEndedWithTextState(RowFormatter rowFormatter)
			: base(rowFormatter) {
		}
	}
	#endregion
	#region RowEndedWithTextFirstSyllableState
	public class RowEndedWithTextFirstSyllableState : EmptyRowFirstSyllableState {
		public RowEndedWithTextFirstSyllableState(RowFormatter rowFormatter)
			: base(rowFormatter) {
		}
		public override void AddTextBox(Box box) {
			if (!RowFormatter.TryAddTextBox(box, false)) {
				RowFormatter.ResetToWordStart();
				RowFormatter.ResetToLastTabStart();
				RowFormatter.EndRow();
			}
		}
	}
	#endregion
	#region ParagraphStartState
	public class ParagraphStartState : EmptyRowState {
		public ParagraphStartState(RowFormatter rowFormatter)
			: base(rowFormatter) {
		}
		public override void AddTextBox(Box box) {
			RowFormatter.TryAddNumberingListBox();
			base.AddTextBox(box);
		}
		public override void AddDashBox(Box box) {
			RowFormatter.TryAddNumberingListBox();
			base.AddDashBox(box);
		}
		public override void AddFloatingObjectAnchorBox(FloatingObjectAnchorBox box) {
			RowFormatter.TryAddNumberingListBox();
			base.AddFloatingObjectAnchorBox(box);
		}
		public override void AddInlinePictureBox(Box box) {
			RowFormatter.TryAddNumberingListBox();
			base.AddInlinePictureBox(box);
		}
		public override void AddParagraphMarkBox(Box box) {
			RowFormatter.TryAddNumberingListBox();
			base.AddParagraphMarkBox(box);
		}
		public override void AddSpaceBox(Box box) {
			RowFormatter.TryAddNumberingListBox();
			base.AddSpaceBox(box);
		}
		public override void AddTabSpaceBox(Box box) {
			RowFormatter.TryAddNumberingListBox();
			base.AddTabSpaceBox(box);
		}
		protected override void EndRow(RowFormatterFinalState result, RowFormatterState nextState) {
			switch (result) {
				case RowFormatterFinalState.ColumnEnd:
				case RowFormatterFinalState.PageEnd:
				case RowFormatterFinalState.SectionEnd:
					base.EndRow(result, RowFormatterState.RowBreakAfterParagraphStart);
					break;
				default:
					base.EndRow(result, nextState);
					break;
			}
		}
	}
	#endregion
	#region ParagraphEndState
	public class ParagraphEndState : EmptyRowState {
		public ParagraphEndState(RowFormatter rowFormatter)
			: base(rowFormatter) {
		}
		public override void AddTextBox(Box box) {
			ProcessNotSectionBreakBox(box);
		}
		public override void AddDashBox(Box box) {
			ProcessNotSectionBreakBox(box);
		}
		public override void AddFloatingObjectAnchorBox(FloatingObjectAnchorBox box) {
			ProcessNotSectionBreakBox(box);
		}
		public override void AddInlinePictureBox(Box box) {
			ProcessNotSectionBreakBox(box);
		}
		public override void AddParagraphMarkBox(Box box) {
			ProcessNotSectionBreakBox(box);
		}
		public override void AddSpaceBox(Box box) {
			ProcessNotSectionBreakBox(box);
		}
		public override void AddTabSpaceBox(Box box) {
			ProcessNotSectionBreakBox(box);
		}
		public override void AddColumnBreakBox(Box box) {
			ProcessNotSectionBreakBox(box);
		}
		public override void AddLayoutDependentTextBox(LayoutDependentTextBox box) {
			ProcessNotSectionBreakBox(box);
		}
		public override void AddLineBreakBox(Box box) {
			ProcessNotSectionBreakBox(box);
		}
		public override void AddPageBreakBox(Box box) {
			ProcessNotSectionBreakBox(box);
		}
		public override void AddSectionBox(Box box) {
			RowFormatter.AddBox(box);
			EndRow(RowFormatterFinalState.SectionEnd, RowFormatterState.ParagraphStart);
		}
		void ProcessNotSectionBreakBox(Box box) {
			RowFormatter.ResetToBoxStart(box);
			EndRow(RowFormatterFinalState.ParagraphEnd, RowFormatterState.ParagraphStart);
		}
	}
	#endregion
	#region RowEndedWithHyphenState
	public class RowEndedWithHyphenState : RowFormatterStateBase {
		public RowEndedWithHyphenState(RowFormatter rowFormatter)
			: base(rowFormatter) {
		}
		public override void AddColumnBreakBox(Box box) {
			Exceptions.ThrowInternalException();
		}
		public override void AddDashBox(Box box) {
			Exceptions.ThrowInternalException();
		}
		public override void AddFloatingObjectAnchorBox(FloatingObjectAnchorBox box) {
			Exceptions.ThrowInternalException();
		}
		public override void AddInlinePictureBox(Box box) {
			Exceptions.ThrowInternalException();
		}
		public override void AddLineBreakBox(Box box) {
			Exceptions.ThrowInternalException();
		}
		public override void AddPageBreakBox(Box box) {
			Exceptions.ThrowInternalException();
		}
		public override void AddParagraphMarkBox(Box box) {
			Exceptions.ThrowInternalException();
		}
		public override void AddSectionBox(Box box) {
			Exceptions.ThrowInternalException();
		}
		public override void AddSpaceBox(Box box) {
			Exceptions.ThrowInternalException();
		}
		public override void AddTabSpaceBox(Box box) {
			Exceptions.ThrowInternalException();
		}
		public override void AddTextBox(Box box) {
			Exceptions.ThrowInternalException();
		}
		public override void AddHyphenBox(Box box) {
			RowFormatter.AddBox(box);
			RowFormatter.EndRow(RowFormatterFinalState.RowEnd, RowFormatterState.EmptyRowFirstSyllable);
		}
	}
	#endregion
	#region RowWithSpacesOnlyState
	public class RowWithSpacesOnlyState : RowFormatterStateBase {
		public RowWithSpacesOnlyState(RowFormatter rowFormatter)
			: base(rowFormatter) {
		}
		public override void AddSpaceBox(Box box) {
			RowFormatter.AddSpaceBox(box);
		}
	}
	#endregion
	#region RowEndedWithSpacesState
	public class RowEndedWithSpacesState : RowFormatterStateBase {
		public RowEndedWithSpacesState(RowFormatter rowFormatter)
			: base(rowFormatter) {
		}
		public override void AddTextBox(Box box) {
			if (RowFormatter.TryAddTextBox(box, true))
				RowFormatter.ChangeState(RowFormatterState.RowEndedWithText);
			else {
				RowFormatter.ResetToWordStart();
				if (RowFormatter.SuppressHyphenation) {
					RowFormatter.ResetToLastTabStart();
					RowFormatter.EndRow();
				}
				else {
					RowFormatter.HyphenateNextWord();
					RowFormatter.ChangeState(RowFormatterState.RowEndedWithTextFirstSyllable);
				}
			}
		}
	}
	#endregion
	#region EndRowState
	public class EndRowState : RowFormatterStateBase {
		readonly RowFormattingResult result;
		public EndRowState(RowFormatter rowFormatter, RowFormattingResult result)
			: base(rowFormatter) {
			this.result = result;
		}
		public override RowFormattingResult Result { get { return result; } }
		public override bool IsRowFormattingComplete() {
			return true;
		}
		public override void AddColumnBreakBox(Box box) {
			Exceptions.ThrowInternalException();
		}
		public override void AddDashBox(Box box) {
			Exceptions.ThrowInternalException();
		}
		public override void AddFloatingObjectAnchorBox(FloatingObjectAnchorBox box) {
			Exceptions.ThrowInternalException();
		}
		public override void AddInlinePictureBox(Box box) {
			Exceptions.ThrowInternalException();
		}
		public override void AddLineBreakBox(Box box) {
			Exceptions.ThrowInternalException();
		}
		public override void AddPageBreakBox(Box box) {
			Exceptions.ThrowInternalException();
		}
		public override void AddParagraphMarkBox(Box box) {
			Exceptions.ThrowInternalException();
		}
		public override void AddSectionBox(Box box) {
			Exceptions.ThrowInternalException();
		}
		public override void AddSpaceBox(Box box) {
			Exceptions.ThrowInternalException();
		}
		public override void AddTabSpaceBox(Box box) {
			Exceptions.ThrowInternalException();
		}
		public override void AddTextBox(Box box) {
			Exceptions.ThrowInternalException();
		}
		public override void AddHyphenBox(Box box) {
			Exceptions.ThrowInternalException();
		}
	}
	#endregion
	#region ILayoutBoxIterator interface
	public interface ILayoutBoxIterator {
		PieceTable PieceTable { get; }
		FormatterPosition Position { get; set; }
		Paragraph Paragraph { get; }
		BoxInfo GetNextBox();
		BoxInfo GetNextBox(int width, bool canExceed);
		bool IsEnd { get; }
		BoxMeasurer Measurer { get; }
		bool HyphenateNextWord();
		bool SuppressHyphenation { get; }
	}
	#endregion
	#region LayoutBoxIterator
	public class LayoutBoxIterator : ILayoutBoxIterator {
		class HyphenInfo {
			public HyphenInfo(FormatterPosition hyphenPosition, FormatterPosition boxPosition) {
				HyphenPosition = hyphenPosition;
				BoxPosition = boxPosition;
			}
			public FormatterPosition HyphenPosition { get; private set; }
			public FormatterPosition BoxPosition { get; private set; }
		}
		const int splitTextBinarySearchBounds = 300;
		readonly PieceTable pieceTable;
		readonly BoxMeasurer boxMeasurer;
		readonly IHyphenationService hyphenationService;
		FormatterPosition position;
		FormatterPosition wordEndPosition;
		int boxIndex;
		ParagraphBoxCollection boxes;
		List<HyphenInfo> hyphenPositions;
		int hyphenIndex;
		bool suppressHyphenation;
		public LayoutBoxIterator(PieceTable pieceTable, BoxMeasurer boxMeasurer) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			Guard.ArgumentNotNull(boxMeasurer, "boxMeasurer");
			this.boxMeasurer = boxMeasurer;
			this.position = new FormatterPosition();
			this.hyphenationService = DocumentModel.GetService<IHyphenationService>();
			InvalidateWordHyphensInfo();
		}
		public PieceTable PieceTable { get { return pieceTable; } }
		public BoxMeasurer Measurer { get { return boxMeasurer; } }
		public Paragraph Paragraph { get { return PieceTable.Runs[RunIndex].Paragraph; } }
		public FormatterPosition Position {
			get { return position; }
			set {
				if (Position == value)
					return;
				FormatterPosition oldValue = Position;
				SetPosition(value);
				OnPositionChanged(oldValue, value);
			}
		}
		public bool IsEnd { get { return IsDocumentEnd && !(HasNextBox() || MoveToNextBox()); } }
		public bool SuppressHyphenation { get { return suppressHyphenation; } }
		RunIndex RunIndex { get { return Position.RunIndex; } }
		int BoxIndex { get { return boxIndex; } }
		int Offset { get { return Position.Offset; } }
		bool IsDocumentEnd { get { return RunIndex >= new RunIndex(pieceTable.Runs.Count - 1); } }
		DocumentModel DocumentModel { get { return PieceTable.DocumentModel; } }
		char CurrentChar { get { return PieceTable.TextBuffer[PieceTable.Runs[RunIndex].StartIndex + Offset]; } }
		void OnPositionChanged(FormatterPosition oldValue, FormatterPosition newValue) {
			this.boxIndex = Position.BoxIndex;
			if (newValue.RunIndex <= PieceTable.Runs[oldValue.RunIndex].Paragraph.LastRunIndex)
				this.boxes = Paragraph.BoxCollection;
			else
				this.boxes = null;
			if (this.wordEndPosition >= Position)
				this.hyphenIndex = FindHyphenPositionIndex();
			this.suppressHyphenation = (this.hyphenationService == null || Paragraph.SuppressHyphenation || !DocumentModel.DocumentProperties.HyphenateDocument);
		}
		BoxInfo GetNextBoxCore() {
			do {
				if (this.boxes != null && HasNextBox()) {
					BoxInfo boxInfo = CreateCurrentBoxInfo();
					MoveToNextBox();
					return boxInfo;
				}
			} while (MoveToNextBox());
			return null;
		}
		protected internal virtual bool MoveToNextBox() {
			do {
				if (this.boxes == null) {
					FormatParagraph();
					this.boxes = Paragraph.BoxCollection;
					this.boxIndex = 0;
				}
				else
					this.boxIndex++;
				if (HasNextBox()) {
					FormatterPosition startPos = this.boxes[this.boxIndex].StartPos;
					SetPosition(startPos.RunIndex, startPos.Offset, this.boxIndex);
					return true;
				}
			} while (MovePositionToNextParagraph());
			return false;
		}
		bool HasNextBox() {
			return this.boxes != null && this.boxIndex < this.boxes.Count;
		}
		protected virtual BoxInfo CreateCurrentBoxInfo() {
			BoxInfo boxInfo = new BoxInfo();
			Box box = this.boxes[BoxIndex];
			boxInfo.StartPos = new FormatterPosition(Position.RunIndex, Position.Offset, BoxIndex);
			boxInfo.EndPos = new FormatterPosition(box.EndPos.RunIndex, box.EndPos.Offset, BoxIndex);
			if (Position == box.StartPos) {
				boxInfo.Size = box.Bounds.Size;
			}
			else {
				Measurer.MeasureText(boxInfo);
				box = CreateNewBox(box, boxInfo);
			}
			box.StartPos.SetBoxIndex(BoxIndex);
			box.EndPos.SetBoxIndex(BoxIndex);
			boxInfo.Box = box;
			return boxInfo;
		}
		Box CreateNewBox(Box box, BoxInfo boxInfo) {
			Box result = box.CreateBox();
			result.StartPos = boxInfo.StartPos;
			result.EndPos = boxInfo.EndPos;
			return result;
		}
		protected virtual bool MovePositionToNextParagraph() {
			if (!IsDocumentEnd) {
				Position = new FormatterPosition(Paragraph.LastRunIndex + 1, 0, 0);
				return true;
			}
			else
				return false;
		}
		protected virtual void FormatParagraph() {
			if (!Paragraph.BoxCollection.IsValid) {
				ParagraphCharacterIterator characterIterator = new ParagraphCharacterIterator(Paragraph, PieceTable, PieceTable.VisibleTextFilter);
				if (characterIterator.RunIndex <= Paragraph.LastRunIndex) {
					ParagraphCharacterFormatter preFormatter = new ParagraphCharacterFormatter(pieceTable, Measurer);
					preFormatter.Format(characterIterator);
				}
				else
					Paragraph.BoxCollection.Clear();
			}
			Paragraph.BoxCollection.ParagraphStartRunIndex = Paragraph.FirstRunIndex;
		}
		public virtual BoxInfo GetNextBox(int width, bool canExceed) {
			BoxInfo result = GetNextBox();
			if (result == null)
				return null;
			if (result.Size.Width <= width)
				return result;
			Box box = result.Box;
			if (result.StartPos < result.EndPos && (AdjustEndPositionToFit(result, width) || canExceed)) {
				result.Box = CreateNewBox(box, result);
				SetPosition(result.EndPos.RunIndex, result.EndPos.Offset + 1, result.EndPos.BoxIndex);
				this.boxIndex = Position.BoxIndex;
				return result;
			}
			else if (result.StartPos == result.EndPos && canExceed) {
				return result;
			}
			else {
				SetPosition(result.StartPos);
				this.boxIndex = Position.BoxIndex;
				return null;
			}
		}
		public virtual BoxInfo GetNextBox() {
			if (this.wordEndPosition < Position || this.hyphenIndex >= this.hyphenPositions.Count)
				return GetNextBoxCore();
			HyphenInfo hyphenInfo = this.hyphenPositions[this.hyphenIndex];
			FormatterPosition hyphenPosition = hyphenInfo.HyphenPosition;
			FormatterPosition boxPosition = hyphenInfo.BoxPosition;
			if (Position == hyphenPosition) {
				this.hyphenIndex++;
				return CreateHyphenBoxInfo(boxPosition);
			}
			BoxInfo boxInfo = GetNextBoxCore();
			if (hyphenPosition > boxInfo.EndPos)
				return boxInfo;
			boxInfo.EndPos = boxPosition;
			boxInfo.Box = CreateNewBox(boxInfo.Box, boxInfo);
			Measurer.MeasureText(boxInfo);
			SetPosition(hyphenPosition);
			this.boxIndex = hyphenPosition.BoxIndex;
			return boxInfo;
		}
		BoxInfo CreateHyphenBoxInfo(FormatterPosition position) {
			BoxInfo boxInfo = new BoxInfo();
			boxInfo.StartPos = position;
			boxInfo.EndPos = boxInfo.StartPos;
			boxInfo.Size = Measurer.MeasureHyphen(boxInfo.StartPos, boxInfo);
			HyphenBox box = new HyphenBox();
			box.StartPos = boxInfo.StartPos;
			box.EndPos = boxInfo.EndPos;
			boxInfo.Box = box;
			return boxInfo;
		}
		public virtual bool HyphenateNextWord() {
			InvalidateWordHyphensInfo();
			if (SuppressHyphenation)
				return false;
			StringBuilder word = new StringBuilder();
			List<FormatterPosition> positions = new List<FormatterPosition>();
			FormatterPosition wordStartPosition = Position;
			bool isWordEnd = false;
			while (!IsDocumentEnd && !isWordEnd) {
				BoxInfo boxInfo = GetNextBoxCore();
				string boxText = boxInfo.Box.GetText(PieceTable);
				FormatterPosition position = boxInfo.StartPos;
				int length = boxText.Length;
				for (int i = 0; i < length; i++) {
					char ch = boxText[i];
					isWordEnd = IsEndOfWord(ch);
					if (isWordEnd)
						break;
					positions.Add(position);
					position.SetOffset(position.Offset + 1);
					word.Append(ch);
				}
			}
			Position = wordStartPosition;
			if (positions.Count == 0)
				return false;
			this.wordEndPosition = positions[positions.Count - 1];
			List<int> hyphenIndices = this.hyphenationService.Hyphenize(word.ToString());
			int hyphenIndicesCount = hyphenIndices.Count;
			if (hyphenIndicesCount <= 0)
				return false;
			int count = positions.Count;
			int hyphenIndex = 0;
			for (int i = 1; i < count; i++) {
				if (i == hyphenIndices[hyphenIndex]) {
					this.hyphenPositions.Add(new HyphenInfo(positions[i], positions[i - 1]));
					hyphenIndex++;
				}
				if (hyphenIndex >= hyphenIndicesCount)
					break;
			}
			return true;
		}
		void InvalidateWordHyphensInfo() {
			this.hyphenPositions = new List<HyphenInfo>();
			this.hyphenIndex = 0;
			this.wordEndPosition = new FormatterPosition(new RunIndex(-1), -1, -1);
		}
		BoxInfo CreateBoxInfo(FormatterPosition startPos, FormatterPosition endPos) {
			BoxInfo result = new BoxInfo();
			result.StartPos = startPos;
			result.EndPos = endPos;
			return result;
		}
		static bool IsEndOfWord(char ch) {
			return IsWhiteSpace(ch) || IsBreak(ch);
		}
		static bool IsBreak(char ch) {
			return ch == Characters.LineBreak || ch == Characters.PageBreak || ch == Characters.ColumnBreak;
		}
		static bool IsWhiteSpace(char ch) {
			return ch == ' ' || ch == '\t';
		}
		void SetPosition(RunIndex runIndex, int offset, int boxIndex) {
			this.position.SetRunIndex(runIndex);
			this.position.SetOffset(offset);
			this.position.SetBoxIndex(boxIndex);
		}
		int FindHyphenPositionIndex() {
			int count = this.hyphenPositions.Count;
			for (int i = 0; i < count; i++) {
				FormatterPosition pos = this.hyphenPositions[i].HyphenPosition;
				if (pos.RunIndex > Position.RunIndex || (pos.RunIndex == Position.RunIndex && pos.Offset >= Position.Offset))
					return i;
			}
			return count;
		}
		void SetPosition(FormatterPosition pos) {
			SetPosition(pos.RunIndex, pos.Offset, pos.BoxIndex);
		}
		bool AdjustEndPositionToFit(BoxInfo boxInfo, int maxWidth) {
			if (boxInfo.StartPos == boxInfo.EndPos)
				return false;
			boxInfo.Box = null;
			FormatterPosition originalEndPos = boxInfo.EndPos;
			if (Measurer.TryAdjustEndPositionToFit(boxInfo, maxWidth)) {
				Measurer.MeasureText(boxInfo);
				if (boxInfo.Size.Width <= maxWidth)
					return true;
				else
					boxInfo.EndPos = originalEndPos;
			}
			int offset = 0;
			if (boxInfo.EndPos.Offset - boxInfo.StartPos.Offset >= splitTextBinarySearchBounds) {
				int low = boxInfo.StartPos.Offset;
				int hi = boxInfo.StartPos.Offset + splitTextBinarySearchBounds;
				offset = BinarySearchFittedBox(boxInfo, low, hi, maxWidth);
				if (~offset > hi) {
					offset = 0;
					boxInfo.EndPos = originalEndPos;
				}
			}
			if (offset == 0)
				offset = BinarySearchFittedBox(boxInfo, maxWidth);
			Debug.Assert(offset < 0);
			offset = ~offset - 1;
			if (offset < boxInfo.StartPos.Offset) {
				boxInfo.EndPos = new FormatterPosition(boxInfo.EndPos.RunIndex, boxInfo.StartPos.Offset, boxInfo.EndPos.BoxIndex);
				return false;
			}
			else {
				boxInfo.EndPos = new FormatterPosition(boxInfo.EndPos.RunIndex, offset, boxInfo.EndPos.BoxIndex);
				Measurer.MeasureText(boxInfo);
				return true;
			}
		}
		int BinarySearchFittedBox(BoxInfo boxInfo, int low, int hi, int maxWidth) {
			FormatterPosition endPos = boxInfo.EndPos;
			while (low <= hi) {
				int median = (low + ((hi - low) >> 1));
				boxInfo.EndPos = new FormatterPosition(endPos.RunIndex, median, endPos.BoxIndex);
				Measurer.MeasureText(boxInfo);
				if (boxInfo.Size.Width <= maxWidth)
					low = median + 1;
				else
					hi = median - 1;
			}
			return ~low;
		}
		int BinarySearchFittedBox(BoxInfo boxInfo, int maxWidth) {
			return BinarySearchFittedBox(boxInfo, boxInfo.StartPos.Offset, boxInfo.EndPos.Offset, maxWidth);
		}
	}
	#endregion
}
