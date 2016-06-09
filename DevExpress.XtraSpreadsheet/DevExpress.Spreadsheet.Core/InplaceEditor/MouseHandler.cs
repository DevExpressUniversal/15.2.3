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
using DevExpress.Office.Drawing;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Mouse;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Utils.KeyboardHandler;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Internal {
	#region InplaceEditorMouseHandler
	public class InplaceEditorMouseHandler : SpreadsheetMouseHandler {
		readonly InnerCellInplaceEditor inplaceEditor;
		readonly MouseHandler defaultMouseHandler;
		public InplaceEditorMouseHandler(ISpreadsheetControl control, InnerCellInplaceEditor inplaceEditor)
			: base(control) {
			Guard.ArgumentNotNull(inplaceEditor, "inplaceEditor");
			this.inplaceEditor = inplaceEditor;
			this.defaultMouseHandler = control.InnerControl.MouseHandler;
		}
		public InnerCellInplaceEditor InplaceEditor { get { return inplaceEditor; } }
		public MouseHandler DefaultMouseHandler { get { return defaultMouseHandler; } }
		protected internal MouseEventArgs ScreenMouseEventArgs { get; private set; }
		protected internal override MouseHandlerState CreateDefaultState() {
			return new InplaceEditorDefaultMouseHandlerState(this);
		}
		protected override MouseEventArgs ConvertMouseEventArgs(MouseEventArgs screenMouseEventArgs) {
			ScreenMouseEventArgs = screenMouseEventArgs;
			return base.ConvertMouseEventArgs(screenMouseEventArgs);
		}
	}
	#endregion
	#region InplaceEditorDefaultMouseHandlerState
	public class InplaceEditorDefaultMouseHandlerState : SpreadsheetMouseHandlerState {
		public InplaceEditorDefaultMouseHandlerState(InplaceEditorMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		public new InplaceEditorMouseHandler MouseHandler { get { return (InplaceEditorMouseHandler)base.MouseHandler; } }
		public MouseHandler DefaultMouseHandler { get { return MouseHandler.DefaultMouseHandler; } }
		public InnerCellInplaceEditor InplaceEditor { get { return MouseHandler.InplaceEditor; } }
		public bool IsModalMessageShown { get { return InplaceEditor.IsModalMessageShown; } }
		public override void OnMouseDown(MouseEventArgs e) {
			if (IsModalMessageShown)
				return;
			if (HandleHotZoneMouseDown(e))
				return;
			if (InplaceEditor.CanAddFormulaRange()) {
				SpreadsheetHitTestResult hitTestResult = CalculateHitTest(new Point(e.X, e.Y));
				EnhancedSelectionManager selectionManager = new EnhancedSelectionManager(DocumentModel.ActiveSheet, Control.InnerControl);
				if (selectionManager.ShouldSelectColumn(hitTestResult)) {
					SelectColumnsFormulaRangeMouseHandlerState state = new SelectColumnsFormulaRangeMouseHandlerState(MouseHandler, hitTestResult);
					MouseHandler.SwitchStateCore(state, new Point(e.X, e.Y));
				}
				else if (selectionManager.ShouldSelectRow(hitTestResult)) {
					SelectRowsFormulaRangeMouseHandlerState state = new SelectRowsFormulaRangeMouseHandlerState(MouseHandler, hitTestResult);
					MouseHandler.SwitchStateCore(state, new Point(e.X, e.Y));
				}
				else {
					SelectCellsFormulaRangeMouseHandlerState state = new SelectCellsFormulaRangeMouseHandlerState(MouseHandler, hitTestResult);
					MouseHandler.SwitchStateCore(state, new Point(e.X, e.Y));
				}
				return;
			}
			if (!InplaceEditor.CanClose())
				return;
			bool commitSuccessful = InplaceEditor.Commit(false, false);
			if (commitSuccessful) {
				InplaceEditor.Deactivate(true);
				if (DefaultMouseHandler != null)
					DefaultMouseHandler.OnMouseDown(MouseHandler.ScreenMouseEventArgs);
				return;
			}
			else
				InplaceEditor.SetFocusToEditor();
		}
		public override void OnMouseUp(MouseEventArgs e) {
			if (IsModalMessageShown)
				return;
			if (DefaultMouseHandler != null)
				DefaultMouseHandler.OnMouseUp(MouseHandler.ScreenMouseEventArgs);
		}
		public override void OnMouseMove(MouseEventArgs e) {
			if (IsModalMessageShown)
				return;
			if (HandleHotZoneMouseMove(e))
				return;
			if (DefaultMouseHandler != null)
				DefaultMouseHandler.OnMouseMove(MouseHandler.ScreenMouseEventArgs);
		}
		public override void OnMouseWheel(MouseEventArgs e) {
			if (IsModalMessageShown)
				return;
			if (DefaultMouseHandler != null)
				DefaultMouseHandler.OnMouseWheel(MouseHandler.ScreenMouseEventArgs);
		}
		protected internal virtual SpreadsheetHitTestResult CalculateHitTest(Point point) {
			return Control.InnerControl.ActiveView.CalculatePageHitTest(point);
		}
		protected bool HandleHotZoneMouseDown(MouseEventArgs e) {
			if (!Control.InnerControl.IsInplaceEditorActive)
				return false;
			SpreadsheetHitTestResult hitTestResult = CalculateHitTest(new Point(e.X, e.Y));
			if (hitTestResult == null)
				return false;
			return HandleHotZoneMouseDown(hitTestResult);
		}
		protected internal virtual bool HandleHotZoneMouseDown(SpreadsheetHitTestResult hitTestResult) {
			if (hitTestResult == null)
				return false;
			HotZone hotZone = Control.InnerControl.InplaceEditor.CalculateHotZone(hitTestResult.LogicalPoint);
			return HandleHotZoneMouseDownCore(hitTestResult, hotZone);
		}
		protected internal virtual bool HandleHotZoneMouseDownCore(SpreadsheetHitTestResult hitTestResult, HotZone hotZone) {
			if (hotZone == null || !Control.InnerControl.IsEditable)
				return false;
			hotZone.Activate(MouseHandler, hitTestResult);
			return true;
		}
		bool HandleHotZoneMouseMove(MouseEventArgs e) {
			if (!Control.InnerControl.IsInplaceEditorActive)
				return false;
			SpreadsheetHitTestResult hitTestResult = CalculateHitTest(new Point(e.X, e.Y));
			if (hitTestResult == null)
				return false;
			return HandleHotZoneMouseMove(hitTestResult);
		}
		protected internal virtual bool HandleHotZoneMouseMove(SpreadsheetHitTestResult hitTestResult) {
			if (hitTestResult == null)
				return false;
			HotZone hotZone = Control.InnerControl.InplaceEditor.CalculateHotZone(hitTestResult.LogicalPoint);
			return HandleHotZoneMouseMoveCore(hitTestResult, hotZone);
		}
		protected internal virtual bool HandleHotZoneMouseMoveCore(SpreadsheetHitTestResult hitTestResult, HotZone hotZone) {
			if (hotZone == null || !Control.InnerControl.IsEditable)
				return false;
			SpreadsheetCursor cursor = hotZone.Cursor;
			if (cursor != null)
				SetMouseCursor(cursor);
			else
				SetMouseCursor(SpreadsheetCursors.Default);
			return true;
		}
	}
	#endregion
	#region DragFormulaRangeManuallyMouseHandlerState
	public class DragFormulaRangeManuallyMouseHandlerState : DragRangeManuallyMouseHandlerStateBase {
		readonly int referencedRangeIndex;
		public DragFormulaRangeManuallyMouseHandlerState(SpreadsheetMouseHandler mouseHandler, SpreadsheetHitTestResult hitTestResult, int referencedRangeIndex)
			: base(mouseHandler, hitTestResult) {
			this.referencedRangeIndex = referencedRangeIndex;
			Initialize();
		}
		#region Properties
		protected override bool AllowCancel { get { return false; } }
		protected override CellRange OriginalRange {
			get {
				if (!Control.InnerControl.IsInplaceEditorActive)
					return null;
				InnerCellInplaceEditor inplaceEditor = Control.InnerControl.InplaceEditor;
				FormulaReferencedRanges ranges = inplaceEditor.ReferencedRanges;
				if (referencedRangeIndex < 0 || referencedRangeIndex >= ranges.Count)
					return null;
				FormulaReferencedRange referencedRange = ranges[referencedRangeIndex];
				if (referencedRange.IsReadOnly)
					return null;
				return referencedRange.CellRange as CellRange;
			}
		}
		#endregion
		protected internal override void ShowVisualFeedbackCore(Rectangle bounds, Page page) {
		}
		protected internal override DragDropEffects ContinueDrag(Point point, DragDropEffects allowedEffects, IDataObject dataObject) {
			if (CalculateHitTest(point) == null)
				return DragDropEffects.None;
			if (UpdateVisualFeedback())
				CommitDrag(point, dataObject);
			return allowedEffects & DragDropEffects.Move;
		}
		protected internal override bool CommitDrag(CellPosition commitPosition, IDataObject dataObject) {
			CellRange sourceRange = OriginalRange;
			if (sourceRange == null)
				return false;
			commitPosition = new CellPosition(commitPosition.Column, commitPosition.Row, sourceRange.TopLeft.ColumnType, sourceRange.TopLeft.RowType);
			CellPosition bottomRightPosition = new CellPosition(commitPosition.Column + sourceRange.Width - 1, commitPosition.Row + sourceRange.Height - 1, sourceRange.BottomRight.ColumnType, sourceRange.BottomRight.RowType);
			sourceRange.TopLeft = commitPosition;
			sourceRange.BottomRight = bottomRightPosition;
			InnerCellInplaceEditor inplaceEditor = Control.InnerControl.InplaceEditor;
			inplaceEditor.ReplaceRangeText(this.referencedRangeIndex, false);
			return true;
		}
		protected override void CalculateAutoScrollParameters(Point p, Rectangle bounds) {
		}
	}
	#endregion
	#region FormulaRangeAnchor
	public enum FormulaRangeAnchor {
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight,
	}
	#endregion
	#region ResizeFormulaRangeManuallyMouseHandlerState
	public class ResizeFormulaRangeManuallyMouseHandlerState : DragRangeManuallyMouseHandlerStateBase {
		readonly int referencedRangeIndex;
		CellPosition fixedPosition;
		CellPosition originalTopLeftPosition;
		CellPosition originalBottomRightPosition;
		public ResizeFormulaRangeManuallyMouseHandlerState(SpreadsheetMouseHandler mouseHandler, SpreadsheetHitTestResult hitTestResult, int referencedRangeIndex, FormulaRangeAnchor anchor)
			: base(mouseHandler, hitTestResult) {
			this.referencedRangeIndex = referencedRangeIndex;
			Initialize();
			this.fixedPosition = CalculateFixedPosition(OriginalRange, anchor);
			this.CellOffset = Point.Empty;
			this.OriginalRangeSize = new Size(1, 1);
			CellRange originalRange = OriginalRange;
			if (originalRange != null) {
				this.originalTopLeftPosition = originalRange.TopLeft;
				this.originalBottomRightPosition = originalRange.BottomRight;
			}
		}
		#region Properties
		protected override bool AllowCancel { get { return false; } }
		protected override CellRange OriginalRange {
			get {
				if (!Control.InnerControl.IsInplaceEditorActive)
					return null;
				InnerCellInplaceEditor inplaceEditor = Control.InnerControl.InplaceEditor;
				FormulaReferencedRanges ranges = inplaceEditor.ReferencedRanges;
				if (referencedRangeIndex < 0 || referencedRangeIndex >= ranges.Count)
					return null;
				FormulaReferencedRange referencedRange = ranges[referencedRangeIndex];
				if (referencedRange.IsReadOnly)
					return null;
				return referencedRange.CellRange as CellRange;
			}
		}
		#endregion
		protected internal override void ShowVisualFeedbackCore(Rectangle bounds, Page page) {
		}
		protected internal override DragDropEffects ContinueDrag(Point point, DragDropEffects allowedEffects, IDataObject dataObject) {
			if (CalculateHitTest(point) == null)
				return DragDropEffects.None;
			if (UpdateVisualFeedback())
				CommitDrag(point, dataObject);
			return allowedEffects & DragDropEffects.Move;
		}
		protected internal override bool CommitDrag(CellPosition commitPosition, IDataObject dataObject) {
			CellRange sourceRange = OriginalRange;
			if (sourceRange == null)
				return false;
			ModifyRange(sourceRange, commitPosition);
			InnerCellInplaceEditor inplaceEditor = Control.InnerControl.InplaceEditor;
			inplaceEditor.ReplaceRangeText(this.referencedRangeIndex, true);
			return true;
		}
		void ModifyRange(CellRange sourceRange, CellPosition commitPosition) {
			CellPosition topLeft = new CellPosition(Math.Min(commitPosition.Column, fixedPosition.Column), Math.Min(commitPosition.Row, fixedPosition.Row), originalTopLeftPosition.ColumnType, originalTopLeftPosition.RowType);
			CellPosition bottomRight = new CellPosition(Math.Max(commitPosition.Column, fixedPosition.Column), Math.Max(commitPosition.Row, fixedPosition.Row), originalBottomRightPosition.ColumnType, originalBottomRightPosition.RowType);
			sourceRange.TopLeft = topLeft;
			sourceRange.BottomRight = bottomRight;
		}
		CellPosition CalculateFixedPosition(CellRange range, FormulaRangeAnchor anchor) {
			switch (anchor) {
				default:
				case FormulaRangeAnchor.TopLeft:
					return range.BottomRight;
				case FormulaRangeAnchor.BottomRight:
					return range.TopLeft;
				case FormulaRangeAnchor.TopRight:
					return new CellPosition(range.TopLeft.Column, range.BottomRight.Row, range.TopLeft.ColumnType, range.BottomRight.RowType);
				case FormulaRangeAnchor.BottomLeft:
					return new CellPosition(range.BottomRight.Column, range.TopLeft.Row, range.BottomRight.ColumnType, range.TopLeft.RowType);
			}
		}
		protected override void CalculateAutoScrollParameters(Point p, Rectangle bounds) {
		}
	}
	#endregion
	#region SelectFormulaRangeMouseHandlerState
	public class SelectCellsFormulaRangeMouseHandlerState : ContinueSelectionByCellsMouseHandlerState {
		CellRange sourceRange;
		FormulaReferencedRange formulaRange;
		CellPosition activeCell;
		CellPosition previousPosition;
		public SelectCellsFormulaRangeMouseHandlerState(SpreadsheetMouseHandler mouseHandler, SpreadsheetHitTestResult hitTestResult)
			: this(mouseHandler, hitTestResult, null, CellPosition.InvalidValue) {
		}
		public SelectCellsFormulaRangeMouseHandlerState(SpreadsheetMouseHandler mouseHandler, SpreadsheetHitTestResult hitTestResult, FormulaReferencedRange formulaRange, CellPosition activeCell)
			: base(mouseHandler) {
				Worksheet sheet = DocumentModel.ActiveSheet;
				CellPosition cellPosition = hitTestResult.CellPosition;
				this.activeCell = cellPosition;
				if (!activeCell.IsValid)
					activeCell = cellPosition;
				int rangeLength = formulaRange == null ? 0 : formulaRange.CellRange.ToString(DocumentModel.DataContext).Length;
				InnerCellInplaceEditor editor = Control.InnerControl.InplaceEditor;
				CellPosition topLeft = cellPosition;
				CellPosition bottomRight = cellPosition;
				if (KeyboardHandler.IsShiftPressed) {
					int top = Math.Min(cellPosition.Row, activeCell.Row);
					int left = Math.Min(cellPosition.Column, activeCell.Column);
					int bottom = Math.Max(cellPosition.Row, activeCell.Row);
					int right = Math.Max(cellPosition.Column, activeCell.Column);
					topLeft = new CellPosition(left, top);
					bottomRight = new CellPosition(right, bottom);
					this.activeCell = activeCell;
				}
				sourceRange = SheetViewSelection.ExpandRangeToMergedCellSizeSingle(new CellRange(sheet, topLeft, bottomRight));
				if (!editor.IsEditReferenceMode || formulaRange == null)
					this.formulaRange = new FormulaReferencedRange(sourceRange.Clone(), editor.SelectionStart, editor.SelectionLength, false);
				else {
					this.formulaRange = new FormulaReferencedRange(sourceRange.Clone(), formulaRange.Position, rangeLength, false);
					if (KeyboardHandler.IsShiftPressed)
						sourceRange = new CellRange(sheet, activeCell, activeCell);
				}
				previousPosition = cellPosition;
				editor.ReplaceRangeText(this.formulaRange, true);
		}
		protected override void ContinueSelectionCore(SpreadsheetHitTestResult hitTestResult) {
			if (formulaRange == null)
				return;
			CellRange referencedRange = formulaRange.CellRange as CellRange;
			if (referencedRange == null)
				return;
			CellPosition newPosition = hitTestResult.CellPosition;
			if (newPosition.EqualsPosition(previousPosition))
				return;
			int rangeLength = referencedRange.ToString(DocumentModel.DataContext).Length;
			int top = Math.Min(sourceRange.TopLeft.Row, newPosition.Row);
			int left = Math.Min(sourceRange.TopLeft.Column, newPosition.Column);
			int bottom = Math.Max(sourceRange.BottomRight.Row, newPosition.Row);
			int right = Math.Max(sourceRange.BottomRight.Column, newPosition.Column);
			referencedRange.TopLeft = new CellPosition(left, top);
			referencedRange.BottomRight = new CellPosition(right, bottom);
			CellRange expandedRange = SheetViewSelection.ExpandRangeToMergedCellSizeSingle(referencedRange);
			if (!referencedRange.EqualsPosition(expandedRange)) {
				referencedRange.TopLeft = expandedRange.TopLeft;
				referencedRange.BottomRight = expandedRange.BottomRight;
			}
			formulaRange = new FormulaReferencedRange(referencedRange, formulaRange.Position, rangeLength, true);
			InnerCellInplaceEditor inplaceEditor = Control.InnerControl.InplaceEditor;
			previousPosition = newPosition;
			inplaceEditor.ReplaceRangeText(formulaRange, true);
		}
		public override void OnMouseUp(MouseEventArgs e) {
			INameBoxControl nameBox = (INameBoxControl)Control.GetService(typeof(INameBoxControl));
			if (nameBox != null)
				nameBox.SelectionMode = false;
			InplaceEditorMouseHandler inplaceMouseHandler = MouseHandler as InplaceEditorMouseHandler;
			if (inplaceMouseHandler == null)
				return;
			WaitingForSelectionFormulaRangeMouseHandlerState newState = new WaitingForSelectionFormulaRangeMouseHandlerState(inplaceMouseHandler, formulaRange, activeCell);
			Point point = new Point(e.X, e.Y);
			MouseHandler.SwitchStateCore(newState, point);
		}
	}
	#endregion
	#region SelectColumnsFormulaRangeMouseHandlerState
	public class SelectColumnsFormulaRangeMouseHandlerState : ContinueSelectionByColumnsMouseHandlerState {
		CellIntervalRange sourceRange;
		FormulaReferencedRange formulaRange;
		CellPosition activeCell;
		PositionType positionType = PositionType.Relative;
		int previousColumnIndex = -1;
		public SelectColumnsFormulaRangeMouseHandlerState(SpreadsheetMouseHandler mouseHandler, SpreadsheetHitTestResult hitTestResult)
			: this(mouseHandler, hitTestResult, null, CellPosition.InvalidValue) {
		}
		public SelectColumnsFormulaRangeMouseHandlerState(SpreadsheetMouseHandler mouseHandler, SpreadsheetHitTestResult hitTestResult, FormulaReferencedRange formulaRange, CellPosition activeCell)
			: base(mouseHandler) {
			Worksheet sheet = DocumentModel.ActiveSheet;
			int headerIndex = -1;
			if (hitTestResult.HeaderBox != null)
				headerIndex = hitTestResult.HeaderBox.ModelIndex;
			if (headerIndex == previousColumnIndex)
				return;
			int firstColumn = headerIndex;
			int lastColumn = headerIndex;
			if (KeyboardHandler.IsShiftPressed) {
				this.activeCell = activeCell;
				int activeColumn = activeCell.Column;
				firstColumn = Math.Min(firstColumn, activeColumn);
				lastColumn = Math.Max(lastColumn, activeColumn);
			}
			else
				this.activeCell = new CellPosition(headerIndex, 0);
			int rangeLength = formulaRange == null ? 0 : formulaRange.CellRange.ToString(DocumentModel.DataContext).Length;
			InnerCellInplaceEditor editor = Control.InnerControl.InplaceEditor;
			sourceRange = (CellIntervalRange)SheetViewSelection.ExpandRangeToMergedCellSizeSingle(CellIntervalRange.CreateColumnInterval(sheet, firstColumn, positionType, lastColumn, positionType));
			if (!editor.IsEditReferenceMode || formulaRange == null)
				this.formulaRange = new FormulaReferencedRange(sourceRange.Clone(), editor.SelectionStart, editor.SelectionLength, false);
			else {
				this.formulaRange = new FormulaReferencedRange(sourceRange.Clone(), formulaRange.Position, rangeLength, false);
				if (KeyboardHandler.IsShiftPressed) {
					int activeColumn = this.activeCell.Column;
					sourceRange = (CellIntervalRange)SheetViewSelection.ExpandRangeToMergedCellSizeSingle(CellIntervalRange.CreateColumnInterval(sheet, activeColumn, positionType, activeColumn, positionType));
				}
			}
			previousColumnIndex = headerIndex;
			editor.ReplaceRangeText(this.formulaRange, true);
		}
		protected override void ContinueSelectionCore(int column) {
			if (formulaRange == null || previousColumnIndex == column)
				return;
			int rangeLength = formulaRange.CellRange.ToString(DocumentModel.DataContext).Length;
			int startIndex = Math.Min(sourceRange.LeftColumnIndex, column);
			int endIndex = Math.Max(sourceRange.RightColumnIndex, column);
			CellIntervalRange referencedRange = CellIntervalRange.CreateColumnInterval(DocumentModel.ActiveSheet, startIndex, positionType, endIndex, positionType);
			referencedRange = (CellIntervalRange)SheetViewSelection.ExpandRangeToMergedCellSizeSingle(referencedRange);
			formulaRange = new FormulaReferencedRange(referencedRange, formulaRange.Position, rangeLength, false);
			previousColumnIndex = column;
			InnerCellInplaceEditor inplaceEditor = Control.InnerControl.InplaceEditor;
			inplaceEditor.ReplaceRangeText(formulaRange, true);
		}
		public override void OnMouseUp(MouseEventArgs e) {
			INameBoxControl nameBox = (INameBoxControl)Control.GetService(typeof(INameBoxControl));
			if (nameBox != null)
				nameBox.SelectionMode = false;
			InplaceEditorMouseHandler inplaceMouseHandler = MouseHandler as InplaceEditorMouseHandler;
			if (inplaceMouseHandler == null)
				return;
			WaitingForSelectionFormulaRangeMouseHandlerState newState = new WaitingForSelectionFormulaRangeMouseHandlerState(inplaceMouseHandler, formulaRange, activeCell);
			Point point = new Point(e.X, e.Y);
			MouseHandler.SwitchStateCore(newState, point);
		}
	}
	#endregion
	#region SelectRowsFormulaRangeMouseHandlerState
	public class SelectRowsFormulaRangeMouseHandlerState : ContinueSelectionByRowsMouseHandlerState {
		CellIntervalRange sourceRange;
		FormulaReferencedRange formulaRange;
		CellPosition activeCell;
		PositionType positionType = PositionType.Relative;
		int previousColumnIndex = -1;
		public SelectRowsFormulaRangeMouseHandlerState(SpreadsheetMouseHandler mouseHandler, SpreadsheetHitTestResult hitTestResult)
			: this(mouseHandler, hitTestResult, null, CellPosition.InvalidValue) {
		}
		public SelectRowsFormulaRangeMouseHandlerState(SpreadsheetMouseHandler mouseHandler, SpreadsheetHitTestResult hitTestResult, FormulaReferencedRange formulaRange, CellPosition activeCell)
			: base(mouseHandler) {
			Worksheet sheet = DocumentModel.ActiveSheet;
			int headerIndex = -1;
			if (hitTestResult.HeaderBox != null)
				headerIndex = hitTestResult.HeaderBox.ModelIndex;
			if (headerIndex == previousColumnIndex)
				return;
			int firstRow = headerIndex;
			int lastRow = headerIndex;
			if (KeyboardHandler.IsShiftPressed) {
				this.activeCell = activeCell;
				int activeRow = activeCell.Row;
				firstRow = Math.Min(firstRow, activeRow);
				lastRow = Math.Max(lastRow, activeRow);
			}
			else
				this.activeCell = new CellPosition(0, headerIndex);
			int rangeLength = formulaRange == null ? 0 : formulaRange.CellRange.ToString(DocumentModel.DataContext).Length;
			InnerCellInplaceEditor editor = Control.InnerControl.InplaceEditor;
			sourceRange = (CellIntervalRange)SheetViewSelection.ExpandRangeToMergedCellSizeSingle(CellIntervalRange.CreateRowInterval(sheet, firstRow, positionType, lastRow, positionType));
			if (!editor.IsEditReferenceMode || formulaRange == null)
				this.formulaRange = new FormulaReferencedRange(sourceRange.Clone(), editor.SelectionStart, editor.SelectionLength, false);
			else {
				this.formulaRange = new FormulaReferencedRange(sourceRange.Clone(), formulaRange.Position, rangeLength, false);
				if (KeyboardHandler.IsShiftPressed) {
					int activeRow = this.activeCell.Row;
					sourceRange = (CellIntervalRange)SheetViewSelection.ExpandRangeToMergedCellSizeSingle(CellIntervalRange.CreateRowInterval(sheet, activeRow, positionType, activeRow, positionType));
				}
			}
			previousColumnIndex = headerIndex;
			editor.ReplaceRangeText(this.formulaRange, true);
		}
		protected override void ContinueSelectionCore(int row) {
			if (formulaRange == null || row == previousColumnIndex)
				return;
			int rangeLength = formulaRange.CellRange.ToString(DocumentModel.DataContext).Length;
			int startIndex = Math.Min(sourceRange.TopRowIndex, row);
			int endIndex = Math.Max(sourceRange.BottomRowIndex, row);
			CellIntervalRange referencedRange = CellIntervalRange.CreateRowInterval(DocumentModel.ActiveSheet, startIndex, positionType, endIndex, positionType);
			referencedRange = (CellIntervalRange)SheetViewSelection.ExpandRangeToMergedCellSizeSingle(referencedRange);
			formulaRange = new FormulaReferencedRange(referencedRange, formulaRange.Position, rangeLength, false);
			previousColumnIndex = row;
			InnerCellInplaceEditor inplaceEditor = Control.InnerControl.InplaceEditor;
			inplaceEditor.ReplaceRangeText(formulaRange, true);
		}
		public override void OnMouseUp(MouseEventArgs e) {
			INameBoxControl nameBox = (INameBoxControl)Control.GetService(typeof(INameBoxControl));
			if (nameBox != null)
				nameBox.SelectionMode = false;
			InplaceEditorMouseHandler inplaceMouseHandler = MouseHandler as InplaceEditorMouseHandler;
			if (inplaceMouseHandler == null)
				return;
			WaitingForSelectionFormulaRangeMouseHandlerState newState = new WaitingForSelectionFormulaRangeMouseHandlerState(inplaceMouseHandler, formulaRange, activeCell);
			Point point = new Point(e.X, e.Y);
			MouseHandler.SwitchStateCore(newState, point);
		}
	}
	#endregion
	#region WaitingForSelectionFormulaRangeMouseHandlerState
	public class WaitingForSelectionFormulaRangeMouseHandlerState : InplaceEditorDefaultMouseHandlerState {
		FormulaReferencedRange formulaRange;
		CellPosition activeCell;
		public WaitingForSelectionFormulaRangeMouseHandlerState(InplaceEditorMouseHandler mouseHandler, FormulaReferencedRange formulaRange, CellPosition activeCell)
			: base(mouseHandler) {
				this.formulaRange = formulaRange;
				this.activeCell = activeCell;
				InplaceEditor.SetEditReferenceMode();
		}
		public override void OnMouseDown(MouseEventArgs e) {
			if (IsModalMessageShown)
				return;
			if (HandleHotZoneMouseDown(e))
				return;
			if (KeyboardHandler.IsControlPressed) {
				InplaceEditor.BeginMultiSelection();
			}
			if (!InplaceEditor.IsEditReferenceMode)
				activeCell = CellPosition.InvalidValue;
			Point point = new Point(e.X, e.Y);
			SpreadsheetHitTestResult hitTestResult = CalculateHitTest(point);
			EnhancedSelectionManager selectionManager = new EnhancedSelectionManager(DocumentModel.ActiveSheet, Control.InnerControl);
			if (selectionManager.ShouldSelectColumn(hitTestResult)) {
				SelectColumnsFormulaRangeMouseHandlerState state = new SelectColumnsFormulaRangeMouseHandlerState(MouseHandler, hitTestResult, formulaRange, activeCell);
				MouseHandler.SwitchStateCore(state, new Point(e.X, e.Y));
			}
			else if (selectionManager.ShouldSelectRow(hitTestResult)) {
				SelectRowsFormulaRangeMouseHandlerState state = new SelectRowsFormulaRangeMouseHandlerState(MouseHandler, hitTestResult, formulaRange, activeCell);
				MouseHandler.SwitchStateCore(state, new Point(e.X, e.Y));
			}
			else {
				SelectCellsFormulaRangeMouseHandlerState state = new SelectCellsFormulaRangeMouseHandlerState(MouseHandler, hitTestResult, formulaRange, activeCell);
				MouseHandler.SwitchStateCore(state, new Point(e.X, e.Y));
			}
		}
	}
	#endregion
}
