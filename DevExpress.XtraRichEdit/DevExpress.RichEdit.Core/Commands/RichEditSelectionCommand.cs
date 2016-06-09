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
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
#if SL
using System.Windows.Controls.Primitives;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region RichEditSelectionCommand (abstract class)
	public abstract class RichEditSelectionCommand : RichEditCaretBasedCommand {
		protected RichEditSelectionCommand(IRichEditControl control)
			: base(control) {
			ShouldEnsureCaretVisibleVerticallyBeforeUpdate = true;
			ShouldEnsureCaretVisibleVerticallyAfterUpdate = true;
		}
		#region Properties
		protected internal abstract bool TryToKeepCaretX { get; }
		protected internal abstract bool TreatStartPositionAsCurrent { get; }
		protected internal abstract bool ExtendSelection { get; }
		protected internal abstract DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get; }
		protected internal virtual bool ShouldUpdateCaretY { get { return true; } }
		protected internal bool ShouldEnsureCaretVisibleVerticallyBeforeUpdate { get; set; }
		protected internal bool ShouldEnsureCaretVisibleVerticallyAfterUpdate { get; set; }
		#endregion
		protected internal abstract bool CanChangePosition(DocumentModelPosition pos);
		protected internal abstract DocumentLogPosition ChangePosition(DocumentModelPosition pos);
		protected internal virtual void PerformModifyModel() {
		}
		protected internal virtual DocumentModelPosition CalculateSelectionCurrentPosition(Selection selection) {
			if (ExtendSelection)
				return selection.Interval.End.Clone();
			else {
				DocumentModelPosition start = selection.Interval.Start;
				DocumentModelPosition end = selection.Interval.End;
				if (start.LogPosition > end.LogPosition) {
					DocumentModelPosition temp = start;
					start = end;
					end = temp;
				}
				if (TreatStartPositionAsCurrent)
					return start.Clone(); 
				else
					return end.Clone(); 
			}
		}
		protected internal virtual DocumentLogPosition ExtendSelectionStartToParagraphMark(Selection selection, DocumentLogPosition logPosition) {
			EnhancedSelectionManager selectionManager = new EnhancedSelectionManager(ActivePieceTable);
			return selectionManager.ExtendSelectionStartToParagraphMark(selection, logPosition);
		}
		protected internal virtual DocumentLogPosition ExtendSelectionEndToParagraphMark(Selection selection, DocumentLogPosition logPosition) {
			EnhancedSelectionManager selectionManager = new EnhancedSelectionManager(ActivePieceTable);
			return selectionManager.ExtendSelectionEndToParagraphMark(selection, logPosition);
		}
		protected internal virtual void UpdateTableSelectionAfterSelectionUpdated(DocumentLogPosition logPosition) {
			DocumentModel.Selection.SetStartCell(logPosition);
		}
		protected internal virtual void ChangeSelection(Selection selection) {
			DocumentModelPosition pos = CalculateSelectionCurrentPosition(selection);
			DocumentLogPosition logPosition = ChangePosition(pos);
			bool usePreviousBoxBounds = false;
			if (!ExtendSelection)
				logPosition = Algorithms.Min(logPosition, ActivePieceTable.DocumentEndLogPosition);
			else {
				logPosition = Algorithms.Min(logPosition, ActivePieceTable.DocumentEndLogPosition + 1);
				DocumentLogPosition newLogPosition = ExtendSelectionEndToParagraphMark(selection, logPosition);
				if (newLogPosition != logPosition)
					usePreviousBoxBounds = true;
			}
			RunIndex oldSelectionEndRunIndex = selection.Interval.End.RunIndex;
			int oldSelectionLength = selection.Length;
			usePreviousBoxBounds |= ApplyNewPositionToSelectionEnd(selection, logPosition);
			ChangeSelectionStart(selection, logPosition);
			if (!ExtendSelection)
				selection.End = selection.Start;
			selection.UsePreviousBoxBounds = usePreviousBoxBounds;
			if (!ExtendSelection)
				selection.ClearMultiSelection();
			TextRunCollection runs = DocumentModel.ActivePieceTable.Runs;
			TableCell previousLastCell = runs[oldSelectionEndRunIndex].Paragraph.GetCell();
			TableCell currentLastCell = runs[selection.Interval.End.RunIndex].Paragraph.GetCell();
			if (!(oldSelectionLength > selection.Length && currentLastCell != null && previousLastCell == null)) 
				UpdateTableSelectionAfterSelectionUpdated(logPosition);
			ValidateSelection(selection, selection.Length > oldSelectionLength);
		}
		protected internal virtual void ValidateSelection(Selection selection, bool isSelectionExtended) {
			if (isSelectionExtended) {
				FieldIsSelectValidator validator = new FieldIsSelectValidator(ActivePieceTable);
				validator.ValidateSelection(selection);
			}
			else {
				FieldIsUnselectValidator validator = new FieldIsUnselectValidator(ActivePieceTable);
				validator.ValidateSelection(selection);
			}
		}
		protected internal virtual bool ApplyNewPositionToSelectionEnd(Selection selection, DocumentLogPosition logPosition) {
			selection.End = logPosition;
			return false;
		}
		protected internal virtual void ChangeSelectionStart(Selection selection, DocumentLogPosition logPosition) {
			if (!ExtendSelection)
				selection.Start = selection.VirtualEnd;
			else {
				selection.Start = ExtendSelectionStartToParagraphMark(selection, logPosition);
			}
		}
		protected internal override void ExecuteCore() {
			CheckExecutedAtUIThread();
			Control.BeginUpdate();
			try {
				UpdateCaretPosition();
				if (ShouldEnsureCaretVisibleVerticallyBeforeUpdate)
					EnsureCaretVisibleVertically();
#if (DEBUG)
				if (UpdateCaretPositionBeforeChangeSelectionDetailsLevel > DocumentLayoutDetailsLevel.None)
					Debug.Assert(CaretPosition.PageViewInfo != null);
#endif
				BeforeUpdate();
				bool shouldRedrawControl;
				DocumentModel.BeginUpdate();
				try {
					PerformModifyModel();
					shouldRedrawControl = PerformChangeSelection();
				}
				finally {
					DocumentModel.EndUpdate();
				}
				AfterUpdate();
				if (ShouldEnsureCaretVisibleVerticallyAfterUpdate && !DocumentModel.ActivePieceTable.IsComment)
					EnsureCaretVisibleVertically();
#if (DEBUG)
				if (UpdateCaretPositionBeforeChangeSelectionDetailsLevel > DocumentLayoutDetailsLevel.None && !DocumentModel.ActivePieceTable.IsComment)
					Debug.Assert(CaretPosition.PageViewInfo != null);
#endif
				if (!TryToKeepCaretX) {
					if (CaretPosition.LayoutPosition.Character != null)
						CaretPosition.X = CaretPosition.CalculateCaretBounds().X;
				}
				if (ShouldUpdateCaretY && CaretPosition.LayoutPosition.TableCell != null) {
					TableCellViewInfo cell = CaretPosition.LayoutPosition.TableCell;
					CaretPosition.TableCellTopAnchorIndex = (cell != null) ? cell.TopAnchorIndex : -1;
				}
				if (DocumentModel.ActivePieceTable.IsComment)
					Control.InnerControl.ActiveView.OnActivePieceTableChanged(); 
				if (shouldRedrawControl ) {
					UpdateCaretPosition(DocumentLayoutDetailsLevel.Character);
					Control.RedrawEnsureSecondaryFormattingComplete(RefreshAction.Selection);
				}
			}
			finally {
				Control.EndUpdate();
			}
		}
		protected internal virtual void BeforeUpdate() {
		}
		protected internal virtual void AfterUpdate() {
		}
		protected internal virtual bool PerformChangeSelection() {
			Selection selection = DocumentModel.Selection;
			int initialSelectionLength = selection.Length;
			DocumentModel.BeginUpdate();
			try {
				ChangeSelection(selection);
			}
			finally {
				DocumentModel.EndUpdate();
			}
			int currentSelectionLength = selection.Length;
			return (currentSelectionLength != 0 || initialSelectionLength != 0);
		}
		protected internal virtual void EnsureCaretVisibleVertically() {
			if (DocumentModel.IsUpdateLocked)
				return;
			RichEditCommand command = CreateEnsureCaretVisibleVerticallyCommand();
			command.Execute();
			UpdateCaretPosition();
			if (!TryToKeepCaretX) {
				command = new EnsureCaretVisibleHorizontallyCommand(Control);
				command.Execute();
			}
		}
		protected internal virtual ParagraphIndex GetSelectionEndParagraphIndex() {
			SelectionRangeCollection selection = DocumentModel.Selection.GetSortedSelectionCollection();
			ParagraphIndex endParagraphIndex = ActivePieceTable.FindParagraphIndex(selection.Last.End);
			return endParagraphIndex;
		}
		protected internal virtual bool IsSelectionEndInTableCell() {
			ParagraphIndex index = GetSelectionEndParagraphIndex();
			return ActivePieceTable.Paragraphs[index].IsInCell();
		}
		protected internal virtual bool IsSelectionEndAfterTableCell() {
			ParagraphIndex index = GetSelectionEndParagraphIndex();
			if (index == ParagraphIndex.Zero)
				return false;
			bool result = ActivePieceTable.Paragraphs[index - 1].IsInCell()
							&& !ActivePieceTable.Paragraphs[index].IsInCell();
			return result;
		}
		protected internal virtual TableCell GetSelectionEndTableCell() {
			return ActivePieceTable.Paragraphs[GetSelectionEndParagraphIndex()].GetCell();
		}
		protected internal virtual RichEditCommand CreateEnsureCaretVisibleVerticallyCommand() {
			return new EnsureCaretVisibleVerticallyCommand(Control);
		}
		protected internal virtual void UpdateCaretPosition() {
			UpdateCaretPosition(UpdateCaretPositionBeforeChangeSelectionDetailsLevel);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
#if !SL
			CheckExecutedAtUIThread();
#endif
			DocumentModelPosition pos = CalculateSelectionCurrentPosition(DocumentModel.Selection);
			state.Enabled = CanChangePosition(pos);
			state.Visible = true;
			state.Checked = false;
		}
		protected internal virtual DocumentLogPosition GetLeftVisibleLogPosition(DocumentModelPosition pos) {
			IVisibleTextFilter textFilter = ActivePieceTable.NavigationVisibleTextFilter;
			DocumentLogPosition result = textFilter.GetVisibleLogPosition(pos, true);
			if (result < ActivePieceTable.DocumentStartLogPosition)
				return pos.LogPosition;
			else
				return result;
		}
		protected internal void EnsureCaretVisible() {
			if (!DocumentModel.IsUpdateLocked)
				ActiveView.EnsureCaretVisible();
			else
				DocumentModel.DeferredChanges.EnsureCaretVisible = true;
		}
#if SL
		protected internal virtual void EmulateVerticalScroll(ScrollEventType scrollEventType) {
			ActiveView.VerticalScrollController.EmulateScroll(scrollEventType);
		}
		protected internal virtual void EmulateHorizontalScroll(ScrollEventType scrollEventType) {
			ActiveView.HorizontalScrollController.EmulateScroll(scrollEventType);
		}
#endif
	}
	#endregion
}
