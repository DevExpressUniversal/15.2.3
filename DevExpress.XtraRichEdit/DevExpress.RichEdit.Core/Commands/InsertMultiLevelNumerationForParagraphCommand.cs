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
using System.Text;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Commands.Helper;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Internal;
namespace DevExpress.XtraRichEdit.Commands {
	#region InsertMultiLevelNumerationForParagraphCommand
	public class InsertMultiLevelListCommand : NumberingListCommandBase {
		AbstractNumberingListIndex templateListIndex;
		public InsertMultiLevelListCommand(IRichEditControl control)
			: base(control) {
			this.templateListIndex = AbstractNumberingListIndex.InvalidValue;
		}
		public InsertMultiLevelListCommand(IRichEditControl control, AbstractNumberingListIndex templateListIndex)
			: base(control) {
			this.templateListIndex = templateListIndex;
		}
		#region Properties
		protected internal virtual NumberingType NumberingListType { get { return NumberingType.MultiLevel; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertMultilevelList; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertMultilevelListDescription; } }
		protected internal int NestingLevel { get; set; }
		#endregion
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			base.UpdateUIStateCore(state);
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.Numbering.MultiLevel,state.Enabled);
			ApplyDocumentProtectionToSelectedParagraphs(state);
			state.Checked = AreAllParagraphsHasValidNumberingListType();
		}
		protected internal override void ModifyParagraphs(List<ParagraphInterval> paragraphIntervals) {
			if (templateListIndex == AbstractNumberingListIndex.InvalidValue)
				templateListIndex = GetNumberingListIndex();
			ActiveView.EnsureFormattingCompleteForSelection();			
			if (ShouldAddNumberingListToParagraphs(paragraphIntervals)) {
				ListIndex = GetListIndex(paragraphIntervals);
				AddParagraphLayoutPositionIndex(paragraphIntervals);
				FillParagraphsLevelIndex(paragraphIntervals);
				InsertNumberingRangeForParagraph(paragraphIntervals);
			}
		}
		protected internal override void ModifyParagraphsCore(ParagraphIndex startParagraphIndex, ParagraphIndex endParagraphIndex) {
		}
		protected virtual bool ShouldAddNumberingListToParagraphs(List<ParagraphInterval> paragraphIntervals) {
			Paragraph paragraph = ActivePieceTable.Paragraphs[paragraphIntervals[0].Start];
			if (!paragraph.IsInList())
				return true;
			NumberingListIndex numberingListIndex = paragraph.GetNumberingListIndex();
			NumberingList numberingList = DocumentModel.NumberingLists[numberingListIndex];
			return templateListIndex != GetEqualsTemplateListIndex(numberingList);
		}
		protected internal virtual AbstractNumberingListIndex GetNumberingListIndex() {
			return NumberingListHelper.GetAbstractListIndexByType(NumberingListsTemplate, NumberingListType);
		}
		protected internal virtual NumberingListIndex GetListIndex(List<ParagraphInterval> paragraphIntervals) {
			ParagraphInterval firstInterval = paragraphIntervals[0];
			ParagraphIndex startParagraphIndex = firstInterval.Start;
			ParagraphInterval lastInterval = paragraphIntervals[paragraphIntervals.Count - 1];
			ParagraphIndex endParagraphIndex = lastInterval.End;
			NumberingListIndexCalculator calculator = DocumentModel.CommandsCreationStrategy.CreateNumberingListIndexCalculator(DocumentModel, NumberingListType);
			NumberingListIndex result = calculator.GetListIndex(startParagraphIndex, endParagraphIndex);
			if (result >= NumberingListIndex.MinValue) {
				ContinueList = calculator.ContinueList;
				NestingLevel = calculator.NestingLevel;
				return result;
			}
			return calculator.CreateNewList(NumberingListsTemplate[templateListIndex]);
		}
		protected internal virtual bool AreAllParagraphsHasValidNumberingListType() {
			ParagraphIndex startParagraphIndex = DocumentModel.Selection.Interval.NormalizedStart.ParagraphIndex;
			ParagraphIndex endParagraphIndex = GetEndSelectedParagraphIndex();
			for (ParagraphIndex i = startParagraphIndex; i <= endParagraphIndex; i++) {
				Paragraph paragraph = ActivePieceTable.Paragraphs[i];
				if (!paragraph.IsInList() || GetLevelType(paragraph) != NumberingListType)
					return false;
			}
			return true;
		}
		protected virtual ParagraphIndex GetEndSelectedParagraphIndex() {
			DocumentModelPosition selectionEnd = DocumentModel.Selection.Interval.NormalizedEnd;
			ParagraphIndex lastSelectedParagraphIndex = selectionEnd.ParagraphIndex;
			PieceTable pieceTable = selectionEnd.PieceTable;			
			ParagraphIndex lastParagraphIndex = new ParagraphIndex(pieceTable.Paragraphs.Count - 1);
			if (DocumentModel.Selection.Length == 0)
				return lastSelectedParagraphIndex > lastParagraphIndex ? lastParagraphIndex : lastSelectedParagraphIndex;
			if (lastSelectedParagraphIndex > lastParagraphIndex)
				return lastParagraphIndex;
			Paragraph paragraph = pieceTable.Paragraphs[lastSelectedParagraphIndex];
			if (paragraph.LogPosition < selectionEnd.LogPosition)
				return lastSelectedParagraphIndex;
			else
				return lastSelectedParagraphIndex - 1;
		}
		protected internal AbstractNumberingListIndex GetEqualsTemplateListIndex(NumberingList numberingList) {
			AbstractNumberingListIndex count = new AbstractNumberingListIndex(NumberingListsTemplate.Count);
			AbstractNumberingList abstractNumberingList = numberingList.AbstractNumberingList;
			for (AbstractNumberingListIndex i = new AbstractNumberingListIndex(0); i < count; i++) {
				AbstractNumberingList numberingListTemplate = NumberingListsTemplate[i];
				if (abstractNumberingList.IsEqual(numberingListTemplate))
					return i;
			}
			return new AbstractNumberingListIndex(-1);
		}
		protected internal override void ChangeSelection(Selection selection) {
		}
		protected internal virtual void FillParagraphsLevelIndex(List<ParagraphInterval> paragraphIntervals) {
			int count = paragraphIntervals.Count;
			for (int paragraphIntervalIndex = 0; paragraphIntervalIndex < count; paragraphIntervalIndex++) {
				ParagraphInterval paragraphInterval = paragraphIntervals[paragraphIntervalIndex];
				ParagraphIndex startParagraphIndex = paragraphInterval.Start;
				ParagraphIndex endParagraphIndex = paragraphInterval.End;
			for (ParagraphIndex i = startParagraphIndex; i <= endParagraphIndex; i++) {
				Row row = ParagraphLayoutPositionIndex[i].Row;
				int boxIndex = ParagraphLayoutPositionIndex[i].BoxIndex;
				Box box = GetBox(boxIndex, row.Boxes);
				Paragraph paragraph = ActivePieceTable.Runs[box.StartPos.RunIndex].Paragraph;
				if (box is ParagraphMarkBox && !IsStartWhiteSpaceParagraph(startParagraphIndex, endParagraphIndex, paragraph))
					continue;
					int rowIndent = GetRowIndent(row, ParagraphLayoutPositionIndex[i]);
					int whiteSpaceBoxLength = box.Bounds.Left - row.Boxes[boxIndex].Bounds.Left;
					int leftIndent = rowIndent + whiteSpaceBoxLength;
					ParagraphsLevelIndex.Add(i, CalculateParagraphListLevel(paragraph, leftIndent));
				}
			}
		}
		protected internal virtual int CalculateParagraphListLevel(Paragraph paragraph, int leftIndent) {
			if (NestingLevel != 0)
				return NestingLevel;
			return GetLevelByCurrentIndent(DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(leftIndent), ListIndex);
		}
		protected internal bool HaveCurrentNumberingListType() {
			return DocumentModel.AbstractNumberingLists.HasListOfType(NumberingListType);
		}
		protected internal bool IsStartWhiteSpaceParagraph(ParagraphIndex startParagraphIndex, ParagraphIndex endParagraphIndex, Paragraph paragraph) {
			if (startParagraphIndex != endParagraphIndex && paragraph.Index != startParagraphIndex)
				return false;
			if (paragraph.Length <= 1 && startParagraphIndex != endParagraphIndex)
				return false;
			return true;
		}
		protected internal void InsertNumberingRangeForParagraph(List<ParagraphInterval> paragraphIntervals) {
			int count = paragraphIntervals.Count;
			for (int paragraphIntervalIndex = count - 1; paragraphIntervalIndex >= 0; paragraphIntervalIndex--) {
				ParagraphInterval paragraphInterval = paragraphIntervals[paragraphIntervalIndex];
				ParagraphIndex startParagraphIndex = paragraphInterval.Start;
				ParagraphIndex endParagraphIndex = paragraphInterval.End;
			for (ParagraphIndex i = endParagraphIndex; i >= startParagraphIndex; i--) {
				DeleteOldNumberingListRange(i);
				DeleteWhiteSpaceBox(ParagraphLayoutPositionIndex[i].BoxIndex, ParagraphLayoutPositionIndex[i].Row.Boxes);
			}
			}
			foreach (ParagraphIndex key in ParagraphsLevelIndex.Keys) {
				Paragraph paragraph = ActivePieceTable.Paragraphs[key];
				AddNumberingListToParagraph(paragraph, ListIndex, ParagraphsLevelIndex[key]);
			}
		}
		protected virtual void AddNumberingListToParagraph(Paragraph paragraph, NumberingListIndex listIndex, int listLevelIndex) {
			TableCell cell = paragraph.GetCell();
			if (cell != null && cell.VerticalMerging == MergingState.Continue)
				return;
			ActivePieceTable.AddNumberingListToParagraph(paragraph, listIndex, listLevelIndex);
			paragraph.ParagraphProperties.ResetUse(ParagraphFormattingOptions.Mask.UseFirstLineIndent | ParagraphFormattingOptions.Mask.UseLeftIndent);
		}
		protected internal virtual void AddParagraphLayoutPositionIndex(List<ParagraphInterval> paragraphIntervals) {
			int count = paragraphIntervals.Count;
			for(int i = 0; i < count; i++) {
				ParagraphInterval paragraphInterval = paragraphIntervals[i];
				ParagraphIndex startParagraphIndex = paragraphInterval.Start;
				ParagraphIndex endParagraphIndex = paragraphInterval.End;
				for (ParagraphIndex j = startParagraphIndex; j <= endParagraphIndex; j++)
					AddParagraphLayoutPositionIndexCore(ActivePieceTable.Paragraphs[j]);
			}
		}
		protected internal virtual void AddParagraphLayoutPositionIndexCore(Paragraph paragraph) {
			ParagraphLayoutPosition paragraphPosition = CreateAndUpdateParagraphLayoutPosition(paragraph);
			ParagraphLayoutPositionIndex.Add(paragraph.Index, paragraphPosition);
		}
		protected internal virtual void DeleteWhiteSpaceBox(int boxIndex, BoxCollection boxes) {
			int length = 0;
			int i = boxIndex;
			while (i < boxes.Count && !boxes[i].IsNotWhiteSpaceBox) {
				length += boxes[i].EndPos.Offset - boxes[i].StartPos.Offset + 1;
				i++;
			}
			DocumentLogPosition logPosition = ActivePieceTable.Runs[boxes[boxIndex].StartPos.RunIndex].Paragraph.LogPosition;
			ActivePieceTable.DeleteContent(logPosition, length, false);
		}
		protected internal virtual int GetLevelByCurrentIndent(int leftIndent, NumberingListIndex listIndex) {
			ListLevelCollection<IOverrideListLevel> levels = DocumentModel.NumberingLists[listIndex].Levels;
			for (int i = 0; i < levels.Count; i++) {
				int actualNumberingPosition = GetActualNumberingPosition(levels[i]);
				if (leftIndent <= actualNumberingPosition)
					return i;
			}
			return levels.Count - 1;
		}
		private int GetActualNumberingPosition(IOverrideListLevel listLevel) {
			if (listLevel.FirstLineIndentType == ParagraphFirstLineIndent.Hanging)
				return listLevel.LeftIndent - listLevel.FirstLineIndent;
			else
				return listLevel.LeftIndent;
		}
		protected internal virtual void DeleteOldNumberingListRange(ParagraphIndex index) {
			if (ActivePieceTable.Paragraphs[index].IsInList())
				ActivePieceTable.RemoveNumberingFromParagraph(ActivePieceTable.Paragraphs[index]);
		}
		protected internal void AssignLevelsIndents(ParagraphIndex index) {
			Paragraph paragraph = ActivePieceTable.Paragraphs[index];
			ListLevelCollection<IOverrideListLevel> levels = DocumentModel.NumberingLists[ListIndex].Levels;
			int firstLevelLeftIndent = levels[0].LeftIndent;
			if (paragraph.LeftIndent != firstLevelLeftIndent)
				AssignLevelsIndentsCore(levels, paragraph);
		}
		protected internal void AssignLevelsIndentsCore(ListLevelCollection<IOverrideListLevel> levels, Paragraph paragraph) {
			for (int i = 0; i < levels.Count; i++) {
				IListLevel level = levels[i];
				ParagraphProperties levelParagraphProperties = level.ParagraphProperties;
				StoreOriginalLevelLeftIndent(level);
				level.ListLevelProperties.OriginalLeftIndent = levelParagraphProperties.LeftIndent;
				levelParagraphProperties.LeftIndent += paragraph.LeftIndent;
			}
		}
		protected virtual void StoreOriginalLevelLeftIndent(IListLevel level) {
			level.ListLevelProperties.OriginalLeftIndent = 0;
		}
	}
	#endregion
	public enum InsertListMode {
		KeepLevelIndex,
		CalculateLevelIndexByIndent,
		ChangeLevelIndex
	}
	public class InsertListFormCommand : InsertMultiLevelListCommand {
		readonly AbstractNumberingList list;
		readonly int levelIndex;
		readonly InsertListMode mode;
		public InsertListFormCommand(IRichEditControl control, AbstractNumberingList list, int levelIndex, InsertListMode mode)
			: base(control) {
			Guard.ArgumentNotNull(list, "list");
			this.list = list;
			this.levelIndex = levelIndex;
			this.mode = mode;
		}
		protected override bool ShouldAddNumberingListToParagraphs(List<ParagraphInterval> paragraphIntervals) {
			return true;
		}
		protected internal override NumberingListIndex GetListIndex(List<ParagraphInterval> paragraphIntervals) {
			NumberingListIndexCalculator calculator = DocumentModel.CommandsCreationStrategy.CreateNumberingListIndexCalculator(DocumentModel, NumberingListType);
			return calculator.CreateNewList(list);
		}
		protected override void AddNumberingListToParagraph(Paragraph paragraph, NumberingListIndex listIndex, int listLevelIndex) {
			base.AddNumberingListToParagraph(paragraph, listIndex, listLevelIndex);
			paragraph.ParagraphProperties.ResetUse(ParagraphFormattingOptions.Mask.UseFirstLineIndent | ParagraphFormattingOptions.Mask.UseLeftIndent);
		}
		protected internal override int CalculateParagraphListLevel(Paragraph paragraph, int leftIndent) {
			switch (mode) {
				default:
				case InsertListMode.CalculateLevelIndexByIndent:
					return base.CalculateParagraphListLevel(paragraph, leftIndent);
				case InsertListMode.KeepLevelIndex:
					if (paragraph.IsInList())
						return paragraph.GetListLevelIndex();
					else
						return base.CalculateParagraphListLevel(paragraph, leftIndent);
				case InsertListMode.ChangeLevelIndex:
					return levelIndex;
			}
		}
	}
}
