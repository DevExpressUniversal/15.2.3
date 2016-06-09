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
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using System;
using System.Drawing;
using DevExpress.XtraRichEdit.Tables.Native;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Mouse {
	#region DefaultMouseHandlerState
	public class DefaultMouseHandlerState : RichEditMouseHandlerState {
		public DefaultMouseHandlerState(RichEditMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		#region Properties
		public override bool AutoScrollEnabled { get { return false; } }
		public override bool CanShowToolTip { get { return true; } }
		public override bool StopClickTimerOnStart { get { return false; } }
		#endregion
		public override void OnMouseDown(MouseEventArgs e) {
			if ((e.Button & MouseButtons.Left) == 0)
				return;
			Point physicalPoint = new Point(e.X, e.Y);
			HandleMouseDown(physicalPoint);
		}
		protected internal virtual void HandleMouseDown(Point physicalPoint) {
			RichEditHitTestResult hitTestResult = CalculateExactPageAreaHitTest(physicalPoint);
			if (hitTestResult.IsValid(DocumentLayoutDetailsLevel.PageArea) && ShouldSwitchActivePieceTableOnSingleClick(hitTestResult)) {
				ChangeActivePieceTableCommand command = new ChangeActivePieceTableCommand(Control, hitTestResult.PageArea.PieceTable, hitTestResult.PageArea.Section, hitTestResult.Page.PageIndex);
				Control.BeginUpdate();
				command.PerformChangeSelection();
				Control.EndUpdate();
			}
			hitTestResult = CalculateHitTest(physicalPoint);
			if (hitTestResult == null) {
				hitTestResult = Control.InnerControl.ActiveView.CalculateNearestPageHitTest(physicalPoint, false);
				if (hitTestResult != null) {
					if (!HandleHotZoneMouseDown(hitTestResult)) {
						if (ShouldSelectFloatingObject(hitTestResult))
							SelectFloatingObject(hitTestResult, true);
						else if (ShouldDeactivateFloatingObject(hitTestResult))
							DeactivateFloatingObject(hitTestResult);
						EnhancedSelectionManager selectionManager = new EnhancedSelectionManager(ActivePieceTable);
						if (selectionManager.ShouldSelectCommentMoreButton(hitTestResult)) {
							Control.ShowReviewingPaneForm(DocumentModel, hitTestResult.CommentViewInfo, false);
						}
						if (selectionManager.ShouldSelectCommentContent(hitTestResult)) {
							SelectComment(hitTestResult);
						}
						if ((selectionManager.ShouldSelectComment(hitTestResult)) && (!selectionManager.ShouldSelectCommentContent(hitTestResult)) && (!selectionManager.ShouldSelectCommentMoreButton(hitTestResult))) {
							SelectComment(hitTestResult);
							ParagraphsDocumentModelIterator iterator = new ParagraphsDocumentModelIterator(ActivePieceTable);
							ExtendSelection(iterator);
							return;
						}
					}
				}
				return;
			}
			HandleMouseDown(hitTestResult);
		}
		protected internal virtual bool HandleHotZoneMouseDown(RichEditHitTestResult hitTestResult) {
			HotZone hotZone = Control.InnerControl.ActiveView.SelectionLayout.CalculateHotZone(hitTestResult, Control.InnerControl.ActiveView);
			return HandleHotZoneMouseDownCore(hitTestResult, hotZone);
		}
		protected internal virtual bool HandleHotZoneMouseDownCore(RichEditHitTestResult hitTestResult, HotZone hotZone) {
			if (hotZone == null || !Control.InnerControl.IsEditable)
				return false;
			if (!hotZone.BeforeActivate(MouseHandler, hitTestResult)) {
				HandleMouseDown(hitTestResult.PhysicalPoint);
				return true;
			}
			hotZone.Activate(MouseHandler, hitTestResult);
			return true;
		}
		protected internal virtual void HandleMouseDown(RichEditHitTestResult hitTestResult) {
			if (HandleHotZoneMouseDown(hitTestResult))
				return;
			EnhancedSelectionManager selectionManager = new EnhancedSelectionManager(ActivePieceTable);
			if (selectionManager.ShouldSelectCommentContent(hitTestResult)) {
				if (Control.IsVisibleReviewingPane())
					Control.ShowReviewingPaneForm(DocumentModel, hitTestResult.CommentViewInfo, false);
				SelectComment(hitTestResult);
			}
			if (selectionManager.ShouldSelectCommentMoreButton(hitTestResult)) {
				Control.ShowReviewingPaneForm(DocumentModel, hitTestResult.CommentViewInfo, false);
				return;
			}
			TableRowViewInfoBase tableRow = selectionManager.CalculateTableRowToResize(hitTestResult);
			if (selectionManager.ShouldResizeTableRow(Control, hitTestResult, tableRow)) {
				BeginResizeTableRowState(hitTestResult, tableRow);
				return;
			}
			VirtualTableColumn virtualColumn = selectionManager.CalculateTableCellsToResizeHorizontally(hitTestResult);
			if (selectionManager.ShouldResizeTableCellsHorizontally(Control, hitTestResult, virtualColumn)) {
				BeginResizeTableCellsHorizontallyState(hitTestResult, virtualColumn);
				return;
			}
			if (selectionManager.ShouldSelectEntireTableColumn(hitTestResult))
				BeginTableColumnsSelectionState(hitTestResult);
			else if (selectionManager.ShouldSelectEntireTableRow(hitTestResult))
				BeginTableRowsSelectionState(hitTestResult);
			else if (selectionManager.ShouldSelectEntireTableCell(hitTestResult))
				BeginTableCellsSelectionState(hitTestResult);
			else if (ShouldSelectFloatingObject(hitTestResult))
				SelectFloatingObject(hitTestResult, true);
			else if (selectionManager.ShouldSelectEntireRow(hitTestResult))
				BeginLineSelection(hitTestResult);
			else if (ShouldStartMultiSelection())
				BeginMultiSelection(hitTestResult);
			else if (ShouldExtendSelectionToCursor()) {
				if (TryProcessHyperlinkClick(hitTestResult))
					return;
				ExtendSelectionToCursor(hitTestResult);
			}
			else if (ShouldSelectPicture(hitTestResult)) {
				if (TryProcessHyperlinkClick(hitTestResult))
					return;
				SelectPicture(hitTestResult);
			}
			else {
				if (Object.ReferenceEquals(hitTestResult.PieceTable, ActivePieceTable))
					BeginCharacterSelection(hitTestResult);
				else {
					if (ShouldDeactivateFloatingObject(hitTestResult))
						DeactivateFloatingObject(hitTestResult);
				}
			}
		}
		protected internal virtual void BeginResizeTableRowState(RichEditHitTestResult hitTestResult, TableRowViewInfoBase tableRow) {
			ResizeTableRowMouseHandlerState resizeState = CreateResizeTableRowState(hitTestResult, tableRow);
			SwitchStateToDragState(hitTestResult, resizeState);
		}
		protected internal virtual void BeginResizeTableCellsHorizontallyState(RichEditHitTestResult hitTestResult, VirtualTableColumn column) {
			ResizeTableVirtualColumnMouseHandlerState resizeState = CreateResizeVirtualTableColumnState(hitTestResult, column);
			SwitchStateToDragState(hitTestResult, resizeState);
		}
		protected internal virtual void BeginTableColumnsSelectionState(RichEditHitTestResult hitTestResult) {
			if (ShouldStartMultiSelection())
				DocumentModel.Selection.BeginMultiSelection(ActivePieceTable);
			SelectTableColumnsCommand command = new SelectTableColumnsCommand(Control);
			command.Rows = hitTestResult.TableRow.Row.Table.Rows;
			TableCell tableCell = hitTestResult.TableCell.Cell;
			int startColumnIndex = tableCell.GetStartColumnIndexConsiderRowGrid();
			int endColumnIndex = tableCell.GetEndColumnIndexConsiderRowGrid(startColumnIndex);
			command.StartColumnIndex = startColumnIndex;
			command.EndColumnIndex = endColumnIndex;
			command.CanCalculateExecutionParameters = false;
			command.Execute();
			ContinueSelectionByTableColumnsMouseHandlerState dragState = new ContinueSelectionByTableColumnsMouseHandlerState(MouseHandler, BoxAccessor.Box, hitTestResult);
			dragState.StartColumnIndex = startColumnIndex;
			dragState.NestedLevel = tableCell.Table.NestedLevel;
			SwitchStateToDragState(hitTestResult, dragState);
		}
		protected internal virtual void BeginTableRowsSelectionState(RichEditHitTestResult hitTestResult) {
			if (ShouldStartMultiSelection())
				DocumentModel.Selection.BeginMultiSelection(ActivePieceTable);
			SelectTableRowCommand command = new SelectTableRowCommand(Control);
			command.CanCalculateExecutionParameters = false;
			command.ShouldEnsureCaretVisibleVerticallyAfterUpdate = false;
			TableRow row = hitTestResult.TableRow.Row;
			command.Rows = row.Table.Rows;
			TableCell restartCell = row.Table.GetFirstCellInVerticalMergingGroup(row.FirstCell);
			int rowIndex = restartCell.Row.IndexInTable;
			command.StartRowIndex = rowIndex;
			command.EndRowIndex = rowIndex;
			command.Execute();
			ContinueSelectionByStartTableRowsMouseHandlerState dragState = new ContinueSelectionByStartTableRowsMouseHandlerState(MouseHandler, BoxAccessor.Row, hitTestResult);
			dragState.StartRowIndex = rowIndex;
			SwitchStateToDragState(hitTestResult, dragState);
		}
		protected internal virtual void BeginTableCellsSelectionState(RichEditHitTestResult hitTestResult) {
			if (ShouldStartMultiSelection())
				DocumentModel.Selection.BeginMultiSelection(ActivePieceTable);
			SelectTableCellCommand command = new SelectTableCellCommand(Control);
			command.Cell = hitTestResult.TableCell.Cell;
			command.ShouldEnsureCaretVisibleVerticallyBeforeUpdate = false;
			command.ShouldEnsureCaretVisibleVerticallyAfterUpdate = false;
			command.Execute();
			MouseHandlerState dragState = new ContinueSelectionByStartTableCellsMouseHandlerState(MouseHandler, BoxAccessor.Box, hitTestResult);
			SwitchStateToDragState(hitTestResult, dragState);
		}
		protected internal virtual bool TryProcessHyperlinkClick(RichEditHitTestResult hitTestResult) {
			if (!Object.ReferenceEquals(hitTestResult.PieceTable, ActivePieceTable))
				return false;
			HyperlinkMouseClickHandler handler = new HyperlinkMouseClickHandler(Control);
			return handler.TryProcessHyperlinkClick(hitTestResult);
		}
		protected internal virtual bool ShouldDeactivateFloatingObject(RichEditHitTestResult hitTestResult) {
			if (hitTestResult.DetailsLevel < DocumentLayoutDetailsLevel.Page)
				return false;
			if (!ActivePieceTable.IsTextBox)
				return false;
			FloatingObjectBox floatingObjectBox = hitTestResult.FloatingObjectBox;
			if (floatingObjectBox == null)
				return true;
			else
				return !Object.ReferenceEquals(floatingObjectBox.GetFloatingObjectRun().PieceTable, ActivePieceTable);
		}
		protected internal virtual void DeactivateFloatingObject(RichEditHitTestResult hitTestResult) {
			FloatingObjectBox floatingObjectBox = hitTestResult.FloatingObjectBox;
			if (floatingObjectBox == null) {
				Control.BeginUpdate();
				try {
					if (ActivePieceTable.IsTextBox) {
						TextBoxContentType textBox = (TextBoxContentType)ActivePieceTable.ContentType;
						MouseHandler.ChangeActivePieceTable(textBox.AnchorRun.PieceTable, hitTestResult);
					}
					else
						MouseHandler.ChangeActivePieceTable(DocumentModel.MainPieceTable);
					ClearMultiSelection();
					PlaceCaretToPhysicalPoint(hitTestResult.PhysicalPoint);
					HandleMouseDown(hitTestResult.PhysicalPoint);
				}
				finally {
					Control.EndUpdate();
				}
			}
			else {
				FloatingObjectAnchorRun run = floatingObjectBox.GetFloatingObjectRun();
				if (!Object.ReferenceEquals(run.PieceTable, ActivePieceTable))
					SelectFloatingObject(hitTestResult, false);
			}
		}
		protected internal virtual bool ShouldActivateFloatingObjectTextBox(FloatingObjectBox floatingObjectBox, FloatingObjectAnchorRun run, Point logicalPoint) {
			if (run == null || floatingObjectBox.DocumentLayout == null)
				return false;
			TextBoxFloatingObjectContent textBoxContent = run.Content as TextBoxFloatingObjectContent;
			if (textBoxContent == null)
				return false;
			Point point = floatingObjectBox.TransformPointBackward(logicalPoint);
			EnhancedSelectionManager selectionManager = new EnhancedSelectionManager(ActivePieceTable);
			return selectionManager.GetTextBoxContentBounds(floatingObjectBox, textBoxContent).Contains(point);
		}
		protected internal virtual bool ActivateFloatingObjectTextBox(FloatingObjectAnchorRun run, RichEditHitTestResult hitTestResult) {
			if (run == null)
				return false;
			Point physicalPoint = hitTestResult.PhysicalPoint;
			TextBoxFloatingObjectContent textBoxContent = run.Content as TextBoxFloatingObjectContent;
			if (textBoxContent != null) {
				Control.BeginUpdate();
				try {
					MouseHandler.ChangeActivePieceTable(textBoxContent.TextBox.PieceTable, hitTestResult);
					ClearMultiSelection();
					PlaceCaretToPhysicalPoint(physicalPoint);
				}
				finally {
					Control.EndUpdate();
				}
				return true;
			}
			return false;
		}
		protected internal virtual bool ActivateComment(RichEditHitTestResult hitTestResult) {
			Point physicalPoint = hitTestResult.PhysicalPoint;
			CommentViewInfo comment = hitTestResult.CommentViewInfo;
			if (comment != null) {
				PieceTable commentPieceTable = comment.Comment.Content.PieceTable;
				if (Object.ReferenceEquals(commentPieceTable, DocumentModel.ActivePieceTable))
					return true;
				Control.BeginUpdate();
				try {
					if (!Object.ReferenceEquals(commentPieceTable, DocumentModel.ActivePieceTable))
						MouseHandler.ChangeActivePieceTable(commentPieceTable, hitTestResult);
					ClearMultiSelection();
					PlaceCaretInCommentToPhysicalPoint(physicalPoint);
				}
				finally {
					Control.EndUpdate();
				}
				return true;
			}
			return false;
		}
		protected internal virtual bool ShouldSelectFloatingObject(RichEditHitTestResult hitTestResult) {
			if (hitTestResult.DetailsLevel < DocumentLayoutDetailsLevel.Page)
				return false;
			FloatingObjectBox floatingObjectBox = hitTestResult.FloatingObjectBox;
			if (floatingObjectBox == null || RichEditMouseHandler.IsInlinePictureBoxHit(hitTestResult))
				return false;
			if (Object.ReferenceEquals(floatingObjectBox.PieceTable, ActivePieceTable))
				return true;
			EnhancedSelectionManager selectionManager = new EnhancedSelectionManager(ActivePieceTable);
			return selectionManager.ShouldSelectFloatingObject(floatingObjectBox, hitTestResult.LogicalPoint);
		}
		protected internal virtual void SelectFloatingObject(RichEditHitTestResult hitTestResult, bool allowDrag) {
			Debug.Assert(hitTestResult.FloatingObjectBox != null);
			FloatingObjectBox floatingObjectBox = hitTestResult.FloatingObjectBox;
			FloatingObjectAnchorRun run = floatingObjectBox.GetFloatingObjectRun();
			DocumentLogPosition logPosition = run.PieceTable.GetRunLogPosition(floatingObjectBox.StartPos.RunIndex);
			DocumentLogPosition endLogPosition = logPosition + 1;
			if (ActivePieceTable != run.PieceTable)
				MouseHandler.ChangeActivePieceTable(run.PieceTable, hitTestResult);
			Paragraph paragraph = run.PieceTable.Runs[floatingObjectBox.StartPos.RunIndex].Paragraph;
			MergedFrameProperties currentFrameProperties = paragraph.GetMergedFrameProperties();
			if (currentFrameProperties != null && !paragraph.IsInCell()) {
				logPosition = paragraph.LogPosition;
				endLogPosition = logPosition + paragraph.Length;
				if (!paragraph.IsLast) {
					ParagraphIndex nextParagraphIndex = paragraph.Index + 1;
					ParagraphCollection paragraphs = run.PieceTable.Paragraphs;
					while (currentFrameProperties.CanMerge(paragraphs[nextParagraphIndex].GetMergedFrameProperties())) {
						endLogPosition += paragraphs[nextParagraphIndex].Length;
						if (paragraphs.Last.Index <= nextParagraphIndex + 1)
							nextParagraphIndex++;
						else
							break;
					}
				}
			}
			SetSelection(logPosition, endLogPosition);
			if (ShouldActivateFloatingObjectTextBox(floatingObjectBox, run, hitTestResult.LogicalPoint) && ActivateFloatingObjectTextBox(run, hitTestResult))
				allowDrag = false;
			Paragraph frameParagraph = run.PieceTable.Runs[floatingObjectBox.StartPos.RunIndex].Paragraph;
			if (frameParagraph.GetMergedFrameProperties() != null && !frameParagraph.IsInCell())
				allowDrag = false;
			if (allowDrag && ShouldBeginDragExistingSelection(hitTestResult))
				BeginDragExistingSelection(hitTestResult, false);
		}
		protected internal virtual void SelectComment(RichEditHitTestResult hitTestResult) {
			if (!ActivateComment(hitTestResult)) {
				BeginDragExistingSelection(hitTestResult, false);
			}
		}
		protected internal virtual bool ShouldSelectPicture(RichEditHitTestResult hitTestResult) {
			if (!Object.ReferenceEquals(hitTestResult.PieceTable, ActivePieceTable))
				return false;
			if (hitTestResult.DetailsLevel < DocumentLayoutDetailsLevel.Box)
				return false;
			if ((hitTestResult.Accuracy & HitTestAccuracy.ExactBox) == 0 && (hitTestResult.Accuracy & HitTestAccuracy.ExactCharacter) == 0)
				return false;
			if (hitTestResult.Box is InlinePictureBox)
				return true;
			CustomRunBox customRunBox = hitTestResult.Box as CustomRunBox;
			return customRunBox != null && customRunBox.GetCustomRun(hitTestResult.PieceTable).GetRectangularObject() != null;
		}
		protected internal virtual void SelectPicture(RichEditHitTestResult hitTestResult) {
			DocumentLogPosition start = hitTestResult.Box.GetFirstPosition(ActivePieceTable).LogPosition;
			SetSelection(start, start + 1);
			if (ShouldBeginDragExistingSelection(hitTestResult))
				BeginDragExistingSelection(hitTestResult, false);
		}
		protected internal virtual bool ShouldStartMultiSelection() {
			return KeyboardHandler.IsControlPressed && this.DocumentModel.Selection.Length > 0;
		}
		protected internal void BeginLineSelection(RichEditHitTestResult hitTestResult) {
			SelectLineCommand command = new SelectLineCommand(Control);
			command.PhysicalPoint = hitTestResult.PhysicalPoint;
			command.ExecuteCore();
			ClearMultiSelection();
			MouseHandlerState dragState = new ContinueSelectionByLinesMouseHandlerState(MouseHandler, BoxAccessor.Row, hitTestResult);
			SwitchStateToDragState(hitTestResult, dragState);
		}
		protected internal void SwitchStateToDragState(RichEditHitTestResult hitTestResult, MouseHandlerState dragState) {
			BeginMouseDragHelperState newState = new BeginMouseDragHelperState(MouseHandler, dragState, hitTestResult.PhysicalPoint);
			MouseHandler.SwitchStateCore(newState, hitTestResult.PhysicalPoint);
		}
		protected internal virtual void BeginCharacterSelection(RichEditHitTestResult hitTestResult) {
			if (ShouldBeginDragExistingSelection(hitTestResult))
				BeginDragExistingSelection(hitTestResult, true);
			else {
				ClearMultiSelection();
				BeginCharacterSelectionCore(hitTestResult);
			}
		}
		protected internal virtual bool ShouldBeginDragExistingSelection(RichEditHitTestResult hitTestResult) {
			DocumentModelPosition pos;
			if (hitTestResult.FloatingObjectBox != null && !ActivePieceTable.ContentType.IsTextBox)
				pos = hitTestResult.FloatingObjectBox.GetFirstPosition(ActivePieceTable);
			else if (hitTestResult.Character != null) {
				InlinePictureBox inlinePictureBox = hitTestResult.Box as InlinePictureBox;
				if (inlinePictureBox != null)
					pos = hitTestResult.Box.GetFirstPosition(ActivePieceTable);
				else
					pos = hitTestResult.Character.GetFirstPosition(ActivePieceTable);
			}
			else
				pos = hitTestResult.Box.GetFirstPosition(ActivePieceTable);
			if (pos == null)
				return false;
			SelectionLayout selectionLayout = this.Control.InnerControl.ActiveView.SelectionLayout;
			bool result = selectionLayout.HitTest(pos.LogPosition, hitTestResult.LogicalPoint);
			if (result) {
				HeaderFooterSelectionLayout headerFooterSelectionLayout = selectionLayout as HeaderFooterSelectionLayout;
				if (headerFooterSelectionLayout != null)
					return headerFooterSelectionLayout.PreferredPageIndex == hitTestResult.Page.PageIndex;
			}
			return result;
		}
		protected internal virtual void BeginDragExistingSelection(RichEditHitTestResult hitTestResult, bool resetSelectionOnMouseUp) {
			if (MouseHandler.DeactivateTextBoxPieceTableIfNeed(hitTestResult.PieceTable, hitTestResult)) {
				HandleMouseDown(hitTestResult.PhysicalPoint);
				return;
			}
			if (!(Control.UseStandardDragDropMode) && (!Control.InnerControl.Options.Behavior.DragAllowed || !Control.InnerControl.Options.Behavior.DropAllowed))
				return;
			MouseHandlerState dragState = CreateDragContentState(hitTestResult);
			BeginContentDragHelperState newState = new BeginContentDragHelperState(MouseHandler, dragState, hitTestResult.PhysicalPoint);
			newState.ResetSelectionOnMouseUp = resetSelectionOnMouseUp;
			MouseHandler.SwitchStateCore(newState, hitTestResult.PhysicalPoint);
		}
		protected internal virtual MouseHandlerState CreateDragContentState(RichEditHitTestResult hitTestResult) {
			if (hitTestResult.FloatingObjectBox != null && !RichEditMouseHandler.IsInlinePictureBoxHit(hitTestResult))
				return new DragFloatingObjectManuallyMouseHandlerState(MouseHandler, hitTestResult);
			if (Control.UseStandardDragDropMode)
				return new DragContentStandardMouseHandlerState(MouseHandler);
			else
				return new DragContentManuallyMouseHandlerState(MouseHandler);
		}
		protected internal virtual ResizeTableRowMouseHandlerState CreateResizeTableRowState(RichEditHitTestResult hitTestResult, TableRowViewInfoBase tableRow) {
			return new ResizeTableRowMouseHandlerState(MouseHandler, hitTestResult, tableRow);
		}
		protected internal virtual ResizeTableVirtualColumnMouseHandlerState CreateResizeVirtualTableColumnState(RichEditHitTestResult hitTestResult, VirtualTableColumn column) {
			return new ResizeTableVirtualColumnMouseHandlerState(MouseHandler, hitTestResult, column);
		}
		protected internal virtual void BeginCharacterSelectionCore(RichEditHitTestResult hitTestResult) {
			MouseHandlerState dragState = new ContinueSelectionByCharactersMouseHandlerState(MouseHandler, BoxAccessor.Character, hitTestResult);
			BeginMouseDragHyperlinkClickHandleHelperState newState = new BeginMouseDragHyperlinkClickHandleHelperState(MouseHandler, dragState, hitTestResult.PhysicalPoint);
			MouseHandler.SwitchStateCore(newState, hitTestResult.PhysicalPoint);
			PlaceCaretToPhysicalPointCommand command = new PlaceCaretToPhysicalPointCommand2(Control);
			command.SuppressClearOutdatedSelectionItems = true;
			command.PhysicalPoint = hitTestResult.PhysicalPoint;
			command.ExecuteCore();
		}
		protected internal virtual void BeginMultiSelection(RichEditHitTestResult hitTestResult) {
			if (!Object.ReferenceEquals(hitTestResult.PieceTable, ActivePieceTable))
				return;
			DocumentModel.Selection.BeginMultiSelection(ActivePieceTable);
			BeginCharacterSelectionCore(hitTestResult);
		}
		void ClearMultiSelection() {
			DocumentModel.Selection.ClearMultiSelection();
		}
		void PlaceCaretToPhysicalPoint(Point physicalPoint) {
			PlaceCaretToPhysicalPointCommand placeCaretCommand = new PlaceCaretToPhysicalPointCommand2(Control);
			placeCaretCommand.PhysicalPoint = physicalPoint;
			placeCaretCommand.ExecuteCore();
		}
		void PlaceCaretInCommentToPhysicalPoint(Point physicalPoint) {
			PlaceCaretToPhysicalPointCommand placeCaretCommand = new PlaceCaretInCommentToPhysicalPointCommand(Control);
			placeCaretCommand.PhysicalPoint = physicalPoint;
			placeCaretCommand.ExecuteCore();
		}
		protected internal virtual void BeginWordsSelection(RichEditHitTestResult hitTestResult) {
			MouseHandlerState dragState = new ContinueSelectionByWordsMouseHandlerState(MouseHandler, BoxAccessor.Character, hitTestResult);
			BeginMouseDragHelperState newState = new RichEditBeginMouseDragHelperState(MouseHandler, dragState, hitTestResult.PhysicalPoint);
			MouseHandler.SwitchStateCore(newState, hitTestResult.PhysicalPoint);
			WordsDocumentModelIterator iterator = new WordsDocumentModelIterator(ActivePieceTable);
			if (ShouldStartMultiSelection()) {
				if (!Object.ReferenceEquals(hitTestResult.PieceTable, ActivePieceTable))
					return;
				DocumentModelPosition position = BoxAccessor.Character.GetBox(hitTestResult).GetFirstPosition(ActivePieceTable);
				ExtendMultiSelection(iterator, position);
			}
			else
				ExtendSelection(iterator);
		}
		protected internal virtual void BeginParagraphsSelection(RichEditHitTestResult hitTestResult) {
			MouseHandlerState dragState = new ContinueSelectionByParagraphsMouseHandlerState(MouseHandler, BoxAccessor.Character, hitTestResult);
			BeginMouseDragHelperState newState = new RichEditBeginMouseDragHelperState(MouseHandler, dragState, hitTestResult.PhysicalPoint);
			MouseHandler.SwitchStateCore(newState, hitTestResult.PhysicalPoint);
			ParagraphsDocumentModelIterator iterator = new ParagraphsDocumentModelIterator(ActivePieceTable);
			ExtendSelection(iterator);
		}
		protected internal virtual bool ShouldExtendSelectionToCursor() {
			return KeyboardHandler.IsShiftPressed;
		}
		protected internal virtual void ExtendSelectionToCursor(RichEditHitTestResult hitTestResult) {
			if (!Object.ReferenceEquals(hitTestResult.PieceTable, ActivePieceTable))
				return;
			PlaceCaretToPhysicalPointCommand command = new ExtendSelectionToPhysicalPointCommand(Control);
			command.PhysicalPoint = hitTestResult.PhysicalPoint;
			command.ExecuteCore();
		}
		protected internal RichEditHitTestResult CalculateHitTest(Point point) {
			return Control.InnerControl.ActiveView.CalculateNearestCharacterHitTest(point, ActivePieceTable);
		}
		protected internal virtual RichEditHitTestResult CalculateExactPageAreaHitTest(Point point) {
			RichEditHitTestRequest request = new RichEditHitTestRequest(ActivePieceTable);
			request.PhysicalPoint = point;
			request.DetailsLevel = DocumentLayoutDetailsLevel.Character;
			request.Accuracy = HitTestAccuracy.ExactPage | HitTestAccuracy.ExactPageArea | HitTestAccuracy.NearestColumn | HitTestAccuracy.NearestTableRow | HitTestAccuracy.NearestTableCell | HitTestAccuracy.NearestRow | HitTestAccuracy.NearestBox | HitTestAccuracy.NearestCharacter;
			request.SearchAnyPieceTable = true;
			RichEditHitTestResult result = Control.InnerControl.ActiveView.HitTestCore(request, true);
			return result;
		}
		protected internal virtual PlaceCaretToPhysicalPointCommand CreatePlaceCaretToPhysicalPointCommand(bool shift) {
			if (shift)
				return new ExtendSelectionToPhysicalPointCommand(Control);
			else
				return new PlaceCaretToPhysicalPointCommand2(Control);
		}
		protected internal virtual void ExtendMultiSelection(PieceTableIterator iterator, DocumentModelPosition currentModelPosition) {
			DocumentModelPosition start;
			if (iterator.IsNewElement(currentModelPosition))
				start = currentModelPosition;
			else
				start = iterator.MoveBack(currentModelPosition);
			DocumentModelPosition end = iterator.MoveForward(currentModelPosition);
			DocumentModel.Selection.BeginMultiSelection(ActivePieceTable);
			SetSelection(start.LogPosition, end.LogPosition, false);
		}
		protected internal virtual void ExtendSelection(PieceTableIterator iterator) {
			DocumentModel documentModel = DocumentModel;
			DocumentModelPosition currentModelPosition = documentModel.Selection.Interval.Start.Clone();
			DocumentModelPosition start;
			if (iterator.IsNewElement(currentModelPosition))
				start = currentModelPosition;
			else
				start = iterator.MoveBack(currentModelPosition);
			DocumentModelPosition end = iterator.MoveForward(currentModelPosition);
			SetSelection(start.LogPosition, end.LogPosition);
		}
		protected internal virtual void SetSelection(DocumentLogPosition start, DocumentLogPosition end) {
			SetSelection(start, end, true);
		}
		protected internal virtual void SetSelection(DocumentLogPosition start, DocumentLogPosition end, bool clearMultiSelection) {
			DocumentModel documentModel = DocumentModel;
			documentModel.BeginUpdate();
			try {
				Selection selection = documentModel.Selection;
				selection.BeginUpdate();
				try {
					if (clearMultiSelection) {
						selection.ClearMultiSelection();
						selection.Start = start;
						selection.End = end;
					}
					else {
						selection.SetInterval(start, end);
					}
					selection.SetStartCell(start);
					ValidateFieldSelection(documentModel, selection);
				}
				finally {
					selection.EndUpdate();
				}
				documentModel.ActivePieceTable.ApplyChangesCore(DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetRuler, RunIndex.DontCare, RunIndex.DontCare);
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		void ValidateFieldSelection(DocumentModel documentModel, Selection selection) {
			FieldIsSelectValidator validator = new FieldIsSelectValidator(documentModel.ActivePieceTable);
			validator.ValidateSelection(selection);
		}
		public override void OnMouseDoubleClick(MouseEventArgs e) {
			Point physicalPoint = new Point(e.X, e.Y);
			RichEditHitTestResult hitTestResult;
			hitTestResult = CalculateExactPageAreaHitTest(physicalPoint);
			if (hitTestResult.IsValid(DocumentLayoutDetailsLevel.PageArea) && ShouldSwitchActivePieceTable(hitTestResult)) {
				ChangeActivePieceTableCommand command = new ChangeActivePieceTableCommand(Control, hitTestResult.PageArea.PieceTable, hitTestResult.PageArea.Section, hitTestResult.Page.PageIndex);
				command.Execute();
				return;
			}
			if (!hitTestResult.IsValid(DocumentLayoutDetailsLevel.PageArea) && ShouldSwitchActivePieceTable(hitTestResult) && (hitTestResult.CommentViewInfo != null) && (hitTestResult.CommentLocation == CommentLocationType.CommentContent)) {
				SelectComment(hitTestResult);
			}
			hitTestResult = CalculateHitTest(physicalPoint);
			if (hitTestResult == null)
				return;
			if (hitTestResult.CommentViewInfo != null && hitTestResult.CommentLocation != CommentLocationType.CommentContent)
				return;
			if (!Object.ReferenceEquals(hitTestResult.PieceTable, ActivePieceTable))
				return;
			PerformEnhancedSelection(hitTestResult);
		}
		void PerformEnhancedSelection(RichEditHitTestResult hitTestResult) {
			EnhancedSelectionManager selectionManager = new EnhancedSelectionManager(ActivePieceTable);
			if (selectionManager.ShouldSelectEntireTableCell(hitTestResult))
				BeginTableRowsSelectionState(hitTestResult);
			else if (selectionManager.ShouldSelectEntireRow(hitTestResult))
				BeginParagraphsSelection(hitTestResult);
			else
				BeginWordsSelection(hitTestResult);
		}
		protected internal virtual bool ShouldSwitchActivePieceTableOnSingleClick(RichEditHitTestResult hitTestResult) {
			if (hitTestResult.PageArea != null && Object.ReferenceEquals(ActivePieceTable, hitTestResult.PageArea.PieceTable))
				return false;
			return ActivePieceTable.IsComment || hitTestResult.CommentViewInfo != null;
		}
		protected internal virtual bool ShouldSwitchActivePieceTable(RichEditHitTestResult hitTestResult) {
			if (hitTestResult.PageArea != null && Object.ReferenceEquals(ActivePieceTable, hitTestResult.PageArea.PieceTable))
				return false;
			if (hitTestResult.FloatingObjectBox != null) {
				FloatingObjectAnchorRun run = hitTestResult.FloatingObjectBox.GetFloatingObjectRun();
				TextBoxFloatingObjectContent content = run.Content as TextBoxFloatingObjectContent;
				if (content != null) {
					if (Object.ReferenceEquals(content.TextBox.PieceTable, ActivePieceTable))
						return false;
				}
			}
			return
				Control.InnerControl.ActiveViewType == RichEditViewType.PrintLayout;
		}
		public override void OnMouseTripleClick(MouseEventArgs e) {
			Point physicalPoint = new Point(e.X, e.Y);
			RichEditHitTestResult hitTestResult = CalculateHitTest(physicalPoint);
			if (hitTestResult == null)
				return;
			if (!Object.ReferenceEquals(hitTestResult.PieceTable, ActivePieceTable))
				return;
			EnhancedSelectionManager selectionManager = new EnhancedSelectionManager(ActivePieceTable);
			if (selectionManager.ShouldSelectCommentContent(hitTestResult)) {
				CommentViewInfo commentViewInfo = hitTestResult.CommentViewInfo;
				if (CalculateParagraphRow(commentViewInfo).Bounds.Bottom > commentViewInfo.ContentBounds.Height) {
					Control.ShowReviewingPaneForm(DocumentModel, commentViewInfo, true);
					SelectComment(hitTestResult);
				}
				else {
					ParagraphsDocumentModelIterator iterator = new ParagraphsDocumentModelIterator(ActivePieceTable);
					ExtendSelection(iterator);
				}
				return;
			}
			if (selectionManager.ShouldSelectEntireRow(hitTestResult)) {
				SelectAllCommand command = new SelectAllCommand(Control);
				command.Execute();
			}
			else
				BeginParagraphsSelection(hitTestResult);
		}
		Row CalculateParagraphRow(CommentViewInfo commentViewInfo) {
			DocumentLayout documentLayout = commentViewInfo.CommentDocumentLayout;
			Paragraph firstCommentParagraph = commentViewInfo.Comment.Content.PieceTable.Paragraphs[new ParagraphIndex(0)];
			DocumentLogPosition logPosition = firstCommentParagraph.EndLogPosition;
			DocumentLayoutPosition layoutPosition = documentLayout.CreateLayoutPosition(ActivePieceTable, logPosition, 0);
			layoutPosition.Page = documentLayout.Pages[0];
			layoutPosition.PageArea = documentLayout.Pages[0].Areas[0];
			layoutPosition.IncreaseDetailsLevel(DocumentLayoutDetailsLevel.PageArea);
			layoutPosition.Update(documentLayout.Pages, DocumentLayoutDetailsLevel.Row);
			return layoutPosition.Row;
		}
		protected override void OnMouseMoveCore(MouseEventArgs e, Point physicalPoint, RichEditHitTestResult hitTestResult) {
			base.OnMouseMoveCore(e, physicalPoint, hitTestResult);
			if (IsHyperlinkActive() && Control.InnerControl.IsHyperlinkModifierKeysPress())
				SetMouseCursor(RichEditCursors.Hand);
			else {
				MouseCursorCalculator calculator = Control.InnerControl.CreateMouseCursorCalculator();
				SetMouseCursor(calculator.Calculate(hitTestResult, physicalPoint));
			}
		}
		protected internal virtual bool IsHyperlinkActive() {
			return MouseHandler.ActiveObject is Field && DocumentModel.ActivePieceTable.IsHyperlinkField((Field)MouseHandler.ActiveObject);
		}
		public override bool OnPopupMenu(MouseEventArgs e) {
			if ((e.Button & MouseButtons.Right) == 0)
				return false;
			RichEditHitTestResult hitTestResult = CalculateHitTest(new Point(e.X, e.Y));
			if (hitTestResult == null)
				return false;
			FloatingObjectBox floatingObjectBox = hitTestResult.FloatingObjectBox;
			if (ShouldSelectFloatingObject(hitTestResult) || (floatingObjectBox != null && !ActivePieceTable.IsTextBox)) {
				if (!IsFloatingObjectSelected(floatingObjectBox))
					SelectFloatingObject(hitTestResult, false);
				return false;
			}
			if (!Object.ReferenceEquals(hitTestResult.PieceTable, ActivePieceTable))
				return false;
			DocumentLogPosition logPosition = hitTestResult.Character.GetFirstPosition(ActivePieceTable).LogPosition;
			if (IsMouseDownInSelection(logPosition))
				return false;
			ClearMultiSelection();
			PlaceCaretToPhysicalPoint(hitTestResult.PhysicalPoint);
			return true;
		}
		bool IsFloatingObjectSelected(FloatingObjectBox floatingObjectBox) {
			Selection selection = DocumentModel.Selection;
			FloatingObjectAnchorRun run = floatingObjectBox.GetFloatingObjectRun();
			if (!Object.ReferenceEquals(selection.PieceTable, run.PieceTable))
				return false;
			DocumentLogPosition logPosition = run.PieceTable.GetRunLogPosition(floatingObjectBox.StartPos.RunIndex);
			return selection.Start == logPosition && selection.End == (logPosition + 1);
		}
		protected internal virtual bool IsMouseDownInSelection(DocumentLogPosition logPosition) {
			Selection selection = DocumentModel.Selection;
			int selectionsCount = selection.Items.Count;
			for (int i = 0; i < selectionsCount; i++) {
				SelectionItem selectionItem = selection.Items[i];
				if (selectionItem.NormalizedStart <= logPosition && logPosition <= selectionItem.NormalizedEnd)
					return true;
			}
			return false;
		}
		public override void OnDragEnter(DragEventArgs e) {
			base.OnDragEnter(e);
			IDataObject dataObject = e.Data;
			if (dataObject == null)
				return;
			DragContentMouseHandlerStateBase state = CreateDragState(dataObject);
			bool isCommandEnabled = Control.InnerControl.Options.Behavior.DropAllowed;
			if (state.CanDropData(e.Data) && isCommandEnabled)
				MouseHandler.SwitchStateCore(state, new Point(e.X, e.Y));
		}
		protected internal virtual bool IsExternalDrag(IDataObject dataObject) {
			try {
				string hashCode = dataObject.GetData(DragContentStandardMouseHandlerState.RichEditDataFormatSelection) as string;
				return String.IsNullOrEmpty(hashCode) || hashCode != Control.GetHashCode().ToString();
			}
			catch (System.Security.SecurityException) {
				return true; 
			}
		}
		protected internal virtual DragContentMouseHandlerStateBase CreateDragState(IDataObject dataObject) {
			return IsExternalDrag(dataObject) ? CreateExternalDragState() : CreateInternalDragState();
		}
		protected internal virtual DragContentMouseHandlerStateBase CreateExternalDragState() {
			return new DragExternalContentMouseHandlerState(MouseHandler);
		}
		protected internal virtual DragContentMouseHandlerStateBase CreateInternalDragState() {
			return MouseHandler.CreateInternalDragState();
		}
	}
	#endregion
}
