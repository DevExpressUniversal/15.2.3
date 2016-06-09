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
using DevExpress.Utils;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Compatibility.System.Drawing;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Utils.Commands;
namespace DevExpress.XtraSpreadsheet.Mouse {
	#region DefaultMouseHandlerState
	public class DefaultMouseHandlerState : SpreadsheetMouseHandlerState {
		delegate void SelectRangeDelegate(CellPosition cellPosition);
		public DefaultMouseHandlerState(SpreadsheetMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		#region Properties
		public override bool AutoScrollEnabled { get { return false; } }
		public override bool CanShowToolTip { get { return true; } }
		public override bool StopClickTimerOnStart { get { return false; } }
		protected Worksheet ActiveSheet { get { return DocumentModel.ActiveSheet; } }
		protected SheetViewSelection ActiveSelection { get { return ActiveSheet.GetActualSelection(); } }
		protected internal InnerSpreadsheetControl InnerControl { get { return Control.InnerControl; } }
		#endregion
		public override void Start() {
			base.Start();
			StartCommentTimer();
		}
		public override void Finish() {
			base.Finish();
			StopCommentTimer();
		}
		void StartCommentTimer() {
			MouseHandler.StartCommentTimer();
		}
		void StopCommentTimer() {
			MouseHandler.StopCommentTimer();
		}
		void TryHideComment() {
			MouseHandler.HideHoveredComment(false);
		}
		public override void OnMouseDown(MouseEventArgs e) {
			if ((e.Button & MouseButtons.Left) != 0) {
				TryCloseInplaceEditors();
				Point physicalPoint = new Point(e.X, e.Y);
				HandleLeftMouseDown(physicalPoint);
			}
			else if ((e.Button & MouseButtons.Right) != 0) {
				TryCloseInplaceEditors();
				TryHideComment();
				Point physicalPoint = new Point(e.X, e.Y);
				HandleRightMouseDown(physicalPoint);
			}
		}
		void TryCloseInplaceEditors() {
			TryCloseCommentInplaceEditor();
		}
		void TryCloseCommentInplaceEditor() {
			CommentCloseEditorCommand command = new CommentCloseEditorCommand(Control);
			command.Execute();
		}
		void TryCloseDataValidationInplaceEditor() {
			InnerControl.DeactivateDataValidationInplaceEditor();
		}
		protected internal virtual void HandleLeftMouseDown(Point physicalPoint) {
			SpreadsheetHitTestResult hitTestResult = CalculateHitTest(physicalPoint);
			if (hitTestResult == null)
				return;
			HandleLeftMouseDown(hitTestResult);
		}
		protected internal virtual void HandleLeftMouseDown(SpreadsheetHitTestResult hitTestResult) {
			if (HandleHotZoneMouseDown(hitTestResult))
				return;
			TryCloseDataValidationInplaceEditor();
			DocumentModel.BeginUpdateFromUI(); 
			try {
				HandleLeftMouseDownCore(hitTestResult);
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		protected internal void HandleLeftMouseDownCore(SpreadsheetHitTestResult hitTestResult) {
			EnhancedSelectionManager selectionManager = new EnhancedSelectionManager(DocumentModel.ActiveSheet, InnerControl);
			if (selectionManager.ShouldSelectComment(hitTestResult))
				SelectAndEditComment(hitTestResult);
			else if (selectionManager.ShouldSelectPicture(hitTestResult)) {
				if (ShouldBeginMultiSelection()) {
					BeginPictureMultiSelection(hitTestResult);
					return;
				}
				IDrawingObject picture = DocumentModel.ActiveSheet.DrawingObjects[hitTestResult.PictureBox.DrawingIndex];
				if (!String.IsNullOrEmpty(picture.DrawingObject.Properties.HyperlinkClickUrl) && !DocumentModel.ActiveSheet.Selection.IsDrawingSelected)
					TryProcessHyperlinkPictureUriClick(picture);
				else
					SelectPicture(hitTestResult);
			}
			else if (ShouldBeginMultiSelection())
				BeginMultiSelection(hitTestResult);
			else if (selectionManager.ShouldSelectAll(hitTestResult)) {
				SelectAll();
			}
			else if (selectionManager.ShouldResizeColumn(hitTestResult, false)) {
				BeginColumnResize(selectionManager, hitTestResult);
			}
			else if (selectionManager.ShouldSelectColumn(hitTestResult)) {
				BeginColumnSelection(hitTestResult);
			}
			else if (selectionManager.ShouldResizeRow(hitTestResult, false)) {
				BeginRowResize(selectionManager, hitTestResult);
			}
			else if (selectionManager.ShouldSelectRow(hitTestResult)) {
				BeginRowSelection(hitTestResult);
			}
			else if (selectionManager.IsHyperlinkActive(hitTestResult)) {
				BeginCellSelectionCore(hitTestResult);
				BeginProcessHyperlinkClick(hitTestResult);
			}
			else if (selectionManager.ShouldSelectGroup(hitTestResult)) {
				BeginGroupProcessing(hitTestResult);
			}
			else {
				BeginCellSelection(hitTestResult);
			}
		}
		protected internal virtual void HandleRightMouseDown(Point physicalPoint) {
			SpreadsheetHitTestResult hitTestResult = CalculateHitTest(physicalPoint);
			if (hitTestResult == null)
				return;
			TryCloseDataValidationInplaceEditor();
			DocumentModel.BeginUpdateFromUI(); 
			try {
				HandleRightMouseDown(hitTestResult);
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		protected internal virtual void HandleRightMouseDown(SpreadsheetHitTestResult hitTestResult) {
			EnhancedSelectionManager selectionManager = new EnhancedSelectionManager(DocumentModel.ActiveSheet, InnerControl);
			if (selectionManager.ShouldSelectComment(hitTestResult))
				SelectAndEditComment(hitTestResult);
			else if (selectionManager.ShouldSelectAll(hitTestResult)) {
				SelectAll();
			}
			else if (selectionManager.ShouldSelectColumn(hitTestResult) || selectionManager.ShouldResizeColumn(hitTestResult, false)) {
				if (!IsHeaderInColumnSelection(hitTestResult))
					SelectColumn(hitTestResult);
			}
			else if (selectionManager.ShouldSelectRow(hitTestResult) || selectionManager.ShouldResizeRow(hitTestResult, false)) {
				if (!IsHeaderInRowSelection(hitTestResult))
					SelectRow(hitTestResult);
			}
			else if (selectionManager.ShouldSelectPicture(hitTestResult)) {
				if (ShouldBeginMultiSelection())
					BeginPictureMultiSelection(hitTestResult);
				else
					SelectPicture(hitTestResult);
			}
			else if (selectionManager.ShouldSelectGroup(hitTestResult)) {
			}
			else if (!IsMouseDownInSelection(hitTestResult.CellPosition))
				BeginCellSelectionCore(hitTestResult);
		}
		public override void OnMouseDoubleClick(MouseEventArgs e) {
			if ((e.Button & MouseButtons.Left) != 0) {
				Point physicalPoint = new Point(e.X, e.Y);
				DocumentModel.BeginUpdateFromUI();
				try {
					HandleLeftMouseDoubleClick(physicalPoint);
				}
				finally {
					DocumentModel.EndUpdateFromUI();
				}
			}
		}
		void HandleLeftMouseDoubleClick(Point physicalPoint) {
			SpreadsheetHitTestResult hitTestResult = CalculateHitTest(physicalPoint);
			if (hitTestResult == null)
				return;
			HandleLeftMouseDoubleClick(hitTestResult);
		}
		void HandleLeftMouseDoubleClick(SpreadsheetHitTestResult hitTestResult) {
			EnhancedSelectionManager selectionManager = new EnhancedSelectionManager(DocumentModel.ActiveSheet, InnerControl);
			if (selectionManager.ShouldResizeColumn(hitTestResult, true) && DocumentModel.BehaviorOptions.Column.AutoFitAllowed) {
				FormatAutoFitColumnWidthUsingMouseCommand command = Control.CreateCommand(SpreadsheetCommandId.FormatAutoFitColumnWidthUsingMouse) as FormatAutoFitColumnWidthUsingMouseCommand;
				if (command != null) {
					command.ColumnIndex = hitTestResult.HeaderBox.ModelIndex;
					command.Execute();
				}
			}
			else if (selectionManager.ShouldResizeRow(hitTestResult, true) && DocumentModel.BehaviorOptions.Row.AutoFitAllowed) {
				FormatAutoFitRowHeightUsingMouseCommand command = Control.CreateCommand(SpreadsheetCommandId.FormatAutoFitRowHeightUsingMouse) as FormatAutoFitRowHeightUsingMouseCommand;
				if (command != null) {
					command.RowIndex = hitTestResult.HeaderBox.ModelIndex;
					command.Execute();
				}
			}
			else if (!HandleValidationHotZoneClick(hitTestResult)) {
				InplaceBeginEditCommand command = new InplaceBeginEditCommand(Control);
				command.Mode = CellEditorMode.Edit;
				command.Execute();
			}
		}
		bool HandleValidationHotZoneClick(SpreadsheetHitTestResult hitTestResult) {
			SpreadsheetView view = InnerControl.ActiveView;
			HotZone hotZone = view.CalculateHotZone(hitTestResult.LogicalPoint);
			DataValidationHotZone dataValidationHotZone = hotZone as DataValidationHotZone;
			if (dataValidationHotZone == null)
				return false;
			dataValidationHotZone.Activate(MouseHandler, hitTestResult);
			return true;
		}
		protected internal void TryProcessHyperlinkPictureUriClick(IDrawingObject picture) {
			HyperlinkPictureUrlMouseClickHandler handler = new HyperlinkPictureUrlMouseClickHandler(Control, picture.DrawingObject);
			handler.TryProcessHyperlinkUriClick();
		}
		protected internal virtual void BeginProcessHyperlinkClick(SpreadsheetHitTestResult hitTestResult) {
			if (hitTestResult == null)
				return;
			SwitchStateToContinueProcessHyperlinkClick(hitTestResult.PhysicalPoint);
		}
		protected internal virtual bool HandleHotZoneMouseDown(SpreadsheetHitTestResult hitTestResult) {
			if (hitTestResult == null)
				return false;
			SpreadsheetView view = InnerControl.ActiveView;
			HotZone hotZone = view.CalculateHotZone(hitTestResult.LogicalPoint);
			return HandleHotZoneMouseDownCore(hitTestResult, hotZone);
		}
		protected internal virtual bool HandleHotZoneMouseDownCore(SpreadsheetHitTestResult hitTestResult, HotZone hotZone) {
			if (hotZone == null || !InnerControl.IsEditable)
				return false;
			hotZone.Activate(MouseHandler, hitTestResult);
			MouseHandler.StopClickTimer();
			return true;
		}
		protected internal void SwitchStateToDragState(SpreadsheetHitTestResult hitTestResult, MouseHandlerState dragState) {
			if (!DocumentModel.BehaviorOptions.DragAllowed)
				return;
			BeginMouseDragHelperState newState = new BeginMouseDragHelperState(MouseHandler, dragState, hitTestResult.PhysicalPoint);
			newState.CancelOnPopupMenu = true;
			newState.CancelOnRightMouseUp = true;
			MouseHandler.SwitchStateCore(newState, hitTestResult.PhysicalPoint);
		}
		protected internal virtual bool ShouldBeginMultiSelection() {
			if (!DocumentModel.BehaviorOptions.Selection.AllowMultiSelection)
				return false;
			return KeyboardHandler.IsControlPressed || KeyboardHandler.IsShiftPressed;
		}
		protected internal virtual void BeginMultiSelection(SpreadsheetHitTestResult hitTestResult) {
			if (hitTestResult.HeaderBox != null) {
				if (hitTestResult.HeaderBox.BoxType == HeaderBoxType.ColumnHeader) {
					if (AllowExtendSelection())
						SwitchStateToContinueSelectionByColumns(hitTestResult.PhysicalPoint);
					if (KeyboardHandler.IsShiftPressed)
						SelectRange(hitTestResult, ExtendColumnSelection);
					else
						SelectRange(hitTestResult, AppendColumnSelection);
				}
				else if (hitTestResult.HeaderBox.BoxType == HeaderBoxType.RowHeader) {
					if (AllowExtendSelection())
						SwitchStateToContinueSelectionByRows(hitTestResult.PhysicalPoint);
					if (KeyboardHandler.IsShiftPressed)
						SelectRange(hitTestResult, ExtendRowSelection);
					else
						SelectRange(hitTestResult, AppendRowSelection);
				}
			}
			else {
				if (AllowExtendSelection())
					SwitchStateToContinueSelectionByCells(hitTestResult.PhysicalPoint);
				if (KeyboardHandler.IsShiftPressed)
					SelectRange(hitTestResult, ExtendSelection);
				else
					SelectRange(hitTestResult, AppendSelection);
			}
		}
		void AppendSelection(CellPosition position) {
			if (AllowExtendSelection())
				ActiveSelection.AppendActiveSelection(position);
			else
				SelectCell(position);
		}
		void AppendColumnSelection(CellPosition position) {
			if (AllowExtendSelection()) {
				CellIntervalRange range = CellIntervalRange.CreateColumnInterval(DocumentModel.ActiveSheet, position.Column, PositionType.Absolute, position.Column, PositionType.Absolute);
				ActiveSelection.AppendActiveSelection(range, false);
			}
			else {
				CellPosition cell = GetColumnActiveViewPosition(position);
				SelectCell(cell);
			}
		}
		void AppendRowSelection(CellPosition position) {
			if (AllowExtendSelection()) {
				CellIntervalRange range = CellIntervalRange.CreateRowInterval(DocumentModel.ActiveSheet, position.Row, PositionType.Absolute, position.Row, PositionType.Absolute);
				ActiveSelection.AppendActiveSelection(range, false);
			}
			else {
				CellPosition cell = GetRowActiveViewPosition(position);
				SelectCell(cell);
			}
		}
		protected internal virtual void BeginCellSelection(SpreadsheetHitTestResult hitTestResult) {
			if (hitTestResult == null)
				return;
			if (AllowExtendSelection())
				SwitchStateToContinueSelectionByCells(hitTestResult.PhysicalPoint);
			BeginCellSelectionCore(hitTestResult);
		}
		protected internal virtual void SelectAll() {
			SwitchStateToEmpty();
			SelectAllCore();
		}
		protected internal virtual void SwitchStateToEmpty() {
			MouseHandlerState newState = new EmptyMouseHandlerState(MouseHandler);
			MouseHandler.SwitchStateCore(newState, Point.Empty);
		}
		protected internal void SelectAllCore() {
			DocumentModel.BeginUpdate();
			try {
				CellPosition activeCell = DocumentModel.ActiveSheet.ActiveView.TopLeftCell;
				if (AllowExtendSelection())
					ActiveSelection.SelectAll(activeCell, true);
				else
					SelectCell(activeCell);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal virtual void BeginColumnResize(EnhancedSelectionManager selectionManager, SpreadsheetHitTestResult hitTestResult) {
			if (hitTestResult == null)
				return;
			Rectangle headerBounds = hitTestResult.HeaderBox.Bounds;
			int columnCorrection = selectionManager.CalculateActualColumnResizeDelta(headerBounds.Width);
			if (headerBounds.Left + columnCorrection > hitTestResult.LogicalPoint.X)
				CorrectPossiblePreviousHeader(hitTestResult);
			SwitchStateToContinueResizeColumn(hitTestResult);
		}
		protected internal virtual void BeginRowResize(EnhancedSelectionManager selectionManager, SpreadsheetHitTestResult hitTestResult) {
			if (hitTestResult == null)
				return;
			Rectangle headerBounds = hitTestResult.HeaderBox.Bounds;
			int rowCorrection = selectionManager.CalculateActualRowResizeDelta(headerBounds.Height);
			if (headerBounds.Top + rowCorrection > hitTestResult.LogicalPoint.Y)
				CorrectPossiblePreviousHeader(hitTestResult);
			SwitchStateToContinueResizeRow(hitTestResult);
		}
		void CorrectPossiblePreviousHeader(SpreadsheetHitTestResult hitTestResult) {
			if (!hitTestResult.HeaderBox.HasZeroPrevious) {
				if (hitTestResult.HeaderBox.Previous.BoxType != HeaderBoxType.SelectAllButton)
					hitTestResult.HeaderBox = hitTestResult.HeaderBox.Previous;
				return;
			}
			HeaderTextBox previous = hitTestResult.HeaderBox.GetZeroPrevious(this.DocumentModel.ActiveSheet);
			if (previous != null)
				hitTestResult.HeaderBox = previous;
		}
		protected internal virtual void BeginColumnSelection(SpreadsheetHitTestResult hitTestResult) {
			if (hitTestResult == null)
				return;
			if (AllowExtendSelection())
				SwitchStateToContinueSelectionByColumns(hitTestResult.PhysicalPoint);
			SelectColumn(hitTestResult);
		}
		protected internal virtual void BeginRowSelection(SpreadsheetHitTestResult hitTestResult) {
			if (hitTestResult == null)
				return;
			if (AllowExtendSelection())
				SwitchStateToContinueSelectionByRows(hitTestResult.PhysicalPoint);
			SelectRow(hitTestResult);
		}
		protected internal void SelectColumn(SpreadsheetHitTestResult hitTestResult) {
			DocumentModel.BeginUpdate();
			try {
				Worksheet activeSheet = DocumentModel.ActiveSheet;
				int columnIndex = hitTestResult.HeaderBox.ModelIndex;
				CellIntervalRange range = CellIntervalRange.CreateColumnInterval(activeSheet, columnIndex, PositionType.Absolute, columnIndex, PositionType.Absolute);
				CellPosition activeCell = GetColumnActiveViewPosition(range.TopLeft);
				if (AllowExtendSelection())
					ActiveSelection.SetSelection(range, activeCell, false);
				else
					SelectCell(activeCell);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal void SelectRow(SpreadsheetHitTestResult hitTestResult) {
			DocumentModel.BeginUpdate();
			try {
				Worksheet activeSheet = DocumentModel.ActiveSheet;
				int rowIndex = hitTestResult.HeaderBox.ModelIndex;
				CellIntervalRange range = CellIntervalRange.CreateRowInterval(activeSheet, rowIndex, PositionType.Absolute, rowIndex, PositionType.Absolute);
				CellPosition activeCell = GetRowActiveViewPosition(range.TopLeft);
				if (AllowExtendSelection())
					ActiveSelection.SetSelection(range, activeCell, false);
				else
					SelectCell(activeCell);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal virtual void SwitchStateToContinueSelectionByCells(Point mousePosition) {
			MouseHandlerState newState = new ContinueSelectionByCellsMouseHandlerState(MouseHandler);
			MouseHandler.SwitchStateCore(newState, mousePosition);
		}
		protected internal virtual void SwitchStateToContinueSelectionByColumns(Point mousePosition) {
			MouseHandlerState newState = new ContinueSelectionByColumnsMouseHandlerState(MouseHandler);
			MouseHandler.SwitchStateCore(newState, mousePosition);
		}
		protected internal virtual void SwitchStateToContinueProcessHyperlinkClick(Point mousePosition) {
			MouseHandlerState newState = new HyperlinkMouseHandlerState(MouseHandler);
			MouseHandler.SwitchStateCore(newState, mousePosition);
		}
		protected internal virtual void SwitchStateToContinueResizeColumn(SpreadsheetHitTestResult hitTestResult) {
			MouseHandlerState newState = new ContinueResizeColumnsMouseHandlerState(MouseHandler, hitTestResult);
			MouseHandler.SwitchStateCore(newState, hitTestResult.PhysicalPoint);
		}
		protected internal virtual void SwitchStateToContinueResizeRow(SpreadsheetHitTestResult hitTestResult) {
			MouseHandlerState newState = new ContinueResizeRowsMouseHandlerState(MouseHandler, hitTestResult);
			MouseHandler.SwitchStateCore(newState, hitTestResult.PhysicalPoint);
		}
		protected internal virtual void SwitchStateToContinueSelectionByRows(Point mousePosition) {
			MouseHandlerState newState = new ContinueSelectionByRowsMouseHandlerState(MouseHandler);
			MouseHandler.SwitchStateCore(newState, mousePosition);
		}
		protected internal virtual void BeginCellSelectionCore(SpreadsheetHitTestResult hitTestResult) {
			SelectRange(hitTestResult, SelectCell);
		}
		protected internal virtual void BeginGroupProcessing(SpreadsheetHitTestResult hitTestResult) {
			if (hitTestResult == null)
				return;
			SwitchStateToContinueGroupProcessing(hitTestResult);
		}
		protected internal virtual void SwitchStateToContinueGroupProcessing(SpreadsheetHitTestResult hitTestResult) {
			MouseHandlerState newState = new GroupProcessingState(MouseHandler, hitTestResult.GroupBox);
			MouseHandler.SwitchStateCore(newState, hitTestResult.PhysicalPoint);
		}
		void SelectCell(CellPosition position) {
			ActiveSelection.SetSelection(position);
		}
		bool AllowExtendSelection() {
			return DocumentModel.BehaviorOptions.Selection.AllowExtendSelection;
		}
		void ExtendSelection(CellPosition position) {
			if (AllowExtendSelection())
				ActiveSelection.ExtendActiveRangeToPosition(position);
			else
				SelectCell(position);
		}
		void ExtendColumnSelection(CellPosition position) {
			if (AllowExtendSelection())
				ActiveSelection.ExtendActiveRangeToColumn(position.Column);
			else {
				CellPosition cell = GetColumnActiveViewPosition(position);
				SelectCell(cell);
			}
		}
		void ExtendRowSelection(CellPosition position) {
			if (AllowExtendSelection())
				ActiveSelection.ExtendActiveRangeToRow(position.Row);
			else {
				CellPosition cell = GetRowActiveViewPosition(position);
				SelectCell(cell);
			}
		}
		CellPosition GetColumnActiveViewPosition(CellPosition original) {
			return new CellPosition(original.Column, DocumentModel.ActiveSheet.ActiveView.TopLeftCell.Row);
		}
		CellPosition GetRowActiveViewPosition(CellPosition original) {
			return new CellPosition(DocumentModel.ActiveSheet.ActiveView.TopLeftCell.Column, original.Row);
		}
		void SelectRange(SpreadsheetHitTestResult hitTestResult, SelectRangeDelegate select) {
			if (hitTestResult == null)
				return;
			DocumentModel.BeginUpdate();
			try {
				if (hitTestResult.HeaderBox == null)
					select(hitTestResult.CellPosition);
				else select(new CellPosition(hitTestResult.HeaderBox.ModelIndex, hitTestResult.HeaderBox.ModelIndex));
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal void SelectPicture(SpreadsheetHitTestResult hitTestResult) {
			DocumentModel.BeginUpdate();
			try {
				ActiveSelection.SetSelectedDrawingIndex(hitTestResult.PictureBox.DrawingIndex);
			}
			finally {
				DocumentModel.EndUpdate();
			}
			if (ShouldBeginDragExistingSelection(hitTestResult))
				BeginDragExistingSelection(hitTestResult);
		}
		protected internal void SelectAndEditComment(SpreadsheetHitTestResult hitTestResult) {
			DocumentModel.BeginUpdate();
			try {
				CommentBox box = hitTestResult.CommentBox;
				SelectComment(box);
				EditComment(box);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void SelectComment(CommentBox box) {
			int index = box.GetCommentIndex();
			ActiveSelection.SelectComment(index);
		}
		void EditComment(CommentBox box) {
			if (DocumentModel.BehaviorOptions.Comment.EditAllowed)
				InnerControl.ActivateCommentInplaceEditor(box);
		}
		protected internal void BeginPictureMultiSelection(SpreadsheetHitTestResult hitTestResult) {
			DocumentModel.BeginUpdate();
			try {
				ActiveSelection.ToggleDrawingSelection(hitTestResult.PictureBox.DrawingIndex);
			}
			finally {
				DocumentModel.EndUpdate();
			}
			if (ShouldBeginDragExistingSelection(hitTestResult))
				BeginDragExistingSelection(hitTestResult);
		}
		protected internal virtual bool ShouldBeginDragExistingSelection(SpreadsheetHitTestResult hitTestResult) {
			return hitTestResult.PictureBox != null && DocumentModel.BehaviorOptions.Drawing.MoveAllowed && DocumentModel.BehaviorOptions.DragAllowed;
		}
		protected internal virtual void BeginDragExistingSelection(SpreadsheetHitTestResult hitTestResult) {
			MouseHandlerState dragState = CreateDragContentState(hitTestResult);
			if (dragState == null)
				return;
			SwitchStateToDragState(hitTestResult, dragState);
		}
		protected internal virtual MouseHandlerState CreateDragContentState(SpreadsheetHitTestResult hitTestResult) {
			if (hitTestResult.PictureBox != null && DocumentModel.BehaviorOptions.Drawing.MoveAllowed)
				return new DragFloatingObjectManuallyMouseHandlerState(MouseHandler, hitTestResult);
			return null;
		}
		protected internal virtual SpreadsheetHitTestResult CalculateHitTest(Point point) {
			return InnerControl.ActiveView.CalculatePageHitTest(point);
		}
		protected override void OnMouseMoveCore(MouseEventArgs e, Point physicalPoint, SpreadsheetHitTestResult hitTestResult) {
			MouseHandler.ProcessComment(hitTestResult);
			base.OnMouseMoveCore(e, physicalPoint, hitTestResult);
			MouseCursorCalculator calculator = InnerControl.CreateMouseCursorCalculator();
			SetMouseCursor(calculator.Calculate(hitTestResult, physicalPoint));
			SelectActiveHeaderBox(hitTestResult);
			SelectActiveGroupBox(hitTestResult);
		}
		protected internal void SelectActiveGroupBox(SpreadsheetHitTestResult hitTestResult) {
			GroupItemsPage groupPage = InnerControl.DesignDocumentLayout.GroupItemsPage;
			if (groupPage == null)
				return;
			OutlineLevelBox groupBox = hitTestResult != null ? hitTestResult.GroupBox : null;
			if (groupPage.ActiveBox == groupBox)
				return;
			else groupPage.ActiveBox = groupBox;
			if (groupBox != null) {
				if (groupBox.IsExpandedButton && !InnerControl.Options.InnerBehavior.Group.CollapseAllowed ||
					groupBox.IsCollapsedButton && !InnerControl.Options.InnerBehavior.Group.ExpandAllowed)
					groupBox.OutlineLevelBoxSelectType = OutlineLevelBoxSelectType.None;
				else
					groupBox.OutlineLevelBoxSelectType = OutlineLevelBoxSelectType.Hovered;
			}
			InnerControl.Owner.Redraw();
		}
		protected internal void SelectActiveHeaderBox(SpreadsheetHitTestResult hitTestResult) {
			HeaderPage headerPage = InnerControl.DesignDocumentLayout.HeaderPage;
			if (headerPage == null)
				return;
			if (hitTestResult == null) {
				InnerControl.ResetAndRedrawHeader();
				return;
			}
			HeaderTextBox headerBox = hitTestResult.HeaderBox;
			if (headerBox == null) {
				if (headerPage.ActiveColumnIndex != HeaderPage.InvalidActiveColumnIndex || headerPage.ActiveRowIndex != HeaderPage.InvalidActiveRowIndex)
					InnerControl.ResetAndRedrawHeader();
				return;
			}
			if (!IsActiveHeaderBoxChanged(headerBox, headerPage))
				return;
			headerPage.InvalidateContent();
			EnhancedSelectionManager selectionManager = new EnhancedSelectionManager(DocumentModel.ActiveSheet, InnerControl);
			if (headerBox.BoxType == HeaderBoxType.ColumnHeader && !selectionManager.ShouldResizeColumn(hitTestResult, false))
				headerPage.ActiveColumnIndex = headerBox.ModelIndex;
			else if (headerBox.BoxType == HeaderBoxType.RowHeader && !selectionManager.ShouldResizeRow(hitTestResult, false))
				headerPage.ActiveRowIndex = headerBox.ModelIndex;
			InnerControl.Owner.Redraw();
		}
		protected internal bool IsActiveHeaderBoxChanged(HeaderTextBox headerBox, HeaderPage headerPage) {
			if (headerBox.BoxType == HeaderBoxType.ColumnHeader)
				return headerPage.ActiveColumnIndex != headerBox.ModelIndex;
			if (headerBox.BoxType == HeaderBoxType.RowHeader)
				return headerPage.ActiveRowIndex != headerBox.ModelIndex;
			return false;
		}
		protected internal virtual bool IsMouseDownInSelection(CellPosition position) {
			IList<CellRange> selectedRanges = ActiveSelection.SelectedRanges;
			int count = selectedRanges.Count;
			for (int i = 0; i < count; i++) {
				CellRange selectedRange = selectedRanges[i];
				if (selectedRange.ContainsRange(new CellRange(DocumentModel.ActiveSheet, position, position)))
					return true;
			}
			return false;
		}
		protected internal virtual bool IsHeaderInColumnSelection(SpreadsheetHitTestResult hitTestResult) {
			HeaderTextBox headerBox = hitTestResult.HeaderBox;
			if (headerBox == null)
				return false;
			int modelIndex = headerBox.ModelIndex;
			IList<CellRange> selectedRanges = ActiveSelection.SelectedRanges;
			int count = selectedRanges.Count;
			for (int i = 0; i < count; i++) {
				CellRange selectedRange = selectedRanges[i];
				if (selectedRange.Height == IndicesChecker.MaxRowCount && selectedRange.TopLeft.Column <= modelIndex && modelIndex <= selectedRange.BottomRight.Column)
					return true;
			}
			return false;
		}
		protected internal virtual bool IsHeaderInRowSelection(SpreadsheetHitTestResult hitTestResult) {
			HeaderTextBox headerBox = hitTestResult.HeaderBox;
			if (headerBox == null)
				return false;
			int modelIndex = headerBox.ModelIndex;
			IList<CellRange> selectedRanges = ActiveSelection.SelectedRanges;
			int count = selectedRanges.Count;
			for (int i = 0; i < count; i++) {
				CellRange selectedRange = selectedRanges[i];
				if (selectedRange.Width == IndicesChecker.MaxColumnCount && selectedRange.TopLeft.Row <= modelIndex && modelIndex <= selectedRange.BottomRight.Row)
					return true;
			}
			return false;
		}
		public override void OnDragEnter(DragEventArgs e) {
			base.OnDragEnter(e);
			IDataObject dataObject = e.Data;
			if (dataObject == null)
				return;
			CancellableDragMouseHandlerStateBase state = CreateDragState(dataObject);
			if (state == null)
				return;
			bool isCommandEnabled = InnerControl.Options.InnerBehavior.DropAllowed;
			if ( isCommandEnabled)
				MouseHandler.SwitchStateCore(state, new Point(e.X, e.Y));
		}
		protected internal virtual bool IsExternalDrag(IDataObject dataObject) {
			try {
				return true;
			}
			catch (System.Security.SecurityException) {
				return true; 
			}
		}
		protected internal virtual CancellableDragMouseHandlerStateBase  CreateDragState(IDataObject dataObject) {
			return IsExternalDrag(dataObject) ? CreateExternalDragState() : CreateInternalDragState();
		}
		protected internal virtual CancellableDragMouseHandlerStateBase CreateExternalDragState() {
			return new DragExternalContentMouseHandlerState(MouseHandler);
		}
		protected internal virtual CancellableDragMouseHandlerStateBase CreateInternalDragState() {
			return null;
		}
	}
	#endregion
}
