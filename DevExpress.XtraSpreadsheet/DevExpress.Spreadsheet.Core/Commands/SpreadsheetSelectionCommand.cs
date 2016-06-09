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
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Layout.Engine;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region SpreadsheetSelectionCommand (abstract class)
	public abstract class SpreadsheetSelectionCommand : SpreadsheetMenuItemSimpleCommand {
		protected SpreadsheetSelectionCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		protected virtual SheetViewSelection Selection { get { return DocumentModel.ActiveSheet.GetActualSelection(); } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		protected virtual bool ExpandsSelection { get { return false; } }
		protected internal ModelWorksheetView SheetView { get { return ActiveSheet.ActiveView; } }
		#endregion
		protected internal override void ExecuteCore() {
			DocumentLayout documentLayout = this.InnerControl.DesignDocumentLayout;
			DocumentModel.BeginUpdateFromUI(); 
			try {
				PerformModifyModel();
				ChangeSelection();
				RecalculateLayoutEnsureActiveCellVisible(documentLayout);
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		protected virtual void RecalculateLayoutEnsureActiveCellVisible(DocumentLayout documentLayout) {
			DocumentLayoutAnchor anchor = CalculateDocumentLayoutAnchor(documentLayout, CalculateAnchorCell());
			if (anchor != null)
				InnerControl.CalculateDocumentLayout(anchor);
		}
		protected virtual CellPosition CalculateAnchorCell() {
			if (ExpandsSelection) {
				CellPosition position = CalculateAnchorCellHorizontalDirection(Selection.ActiveCell);
				return CalculateAnchorCellVerticalDirection(position);
			}
			else
				return Selection.ActiveCell;
		}
		CellPosition CalculateAnchorCellHorizontalDirection(CellPosition position) {
			CellRange activeRange = Selection.ActiveRange;
			if (position.Column == activeRange.TopLeft.Column)
				return new CellPosition(activeRange.BottomRight.Column, position.Row);
			if (position.Column == activeRange.BottomRight.Column)
				return new CellPosition(activeRange.TopLeft.Column, position.Row);
			return position;
		}
		CellPosition CalculateAnchorCellVerticalDirection(CellPosition position) {
			CellRange activeRange = Selection.ActiveRange;
			if (position.Row == activeRange.TopLeft.Row)
				return new CellPosition(position.Column, activeRange.BottomRight.Row);
			if (position.Row == activeRange.BottomRight.Row)
				return new CellPosition(position.Column, activeRange.TopLeft.Row);
			return position;
		}
		bool UpdateAnchorVerticalPosition(DocumentLayoutAnchor anchor, ScrollInfo scrollInfo) {
			if (anchor.CellPosition.Row >= scrollInfo.ScrollBottomRowModelIndex - 1 && scrollInfo.ScrollBottomRowModelIndex != ActiveSheet.MaxRowCount - 1) {
				anchor.VerticalFarAlign = true;
				anchor.CellPosition = new CellPosition(scrollInfo.ScrollLeftColumnModelIndex, anchor.CellPosition.Row);
				return true;
			}
			else {
				anchor.VerticalFarAlign = false;
				if (anchor.CellPosition.Row < scrollInfo.ScrollTopRowModelIndex && (SheetView.SplitState != ViewSplitState.Split || anchor.CellPosition.Row >= SheetView.VerticalSplitPosition)) {
					anchor.CellPosition = new CellPosition(scrollInfo.ScrollLeftColumnModelIndex, anchor.CellPosition.Row);
					return true;
				}
				else
					return false;
			}
		}
		bool UpdateAnchorHorizontalPosition(DocumentLayoutAnchor anchor, ScrollInfo scrollInfo) {
			if (anchor.CellPosition.Column >= scrollInfo.ScrollRightColumnModelIndex - 1 && scrollInfo.ScrollRightColumnModelIndex != ActiveSheet.MaxColumnCount - 1) {
				anchor.HorizontalFarAlign = true;
				anchor.CellPosition = new CellPosition(anchor.CellPosition.Column, scrollInfo.ScrollTopRowModelIndex);
				return true;
			}
			else {
				anchor.HorizontalFarAlign = false;
				if (anchor.CellPosition.Column < scrollInfo.ScrollLeftColumnModelIndex && (SheetView.SplitState != ViewSplitState.Split || anchor.CellPosition.Column >= SheetView.HorizontalSplitPosition)) {
					anchor.CellPosition = new CellPosition(anchor.CellPosition.Column, scrollInfo.ScrollTopRowModelIndex);
					return true;
				}
				else
					return false;
			}
		}
		DocumentLayoutAnchor CalculateDocumentLayoutAnchor(DocumentLayout documentLayout, CellPosition position) {
			if (IsForbiddenScrolling(position)) 
				return null;
			DocumentLayoutAnchor anchor = new DocumentLayoutAnchor();
			if (ActiveSheet.IsFrozen()) {
				CellPosition frozenCell = SheetView.FrozenCell;
				if (position.Column >= frozenCell.Column || position.Row >= frozenCell.Row)
					position = new CellPosition(Math.Max(position.Column, frozenCell.Column), Math.Max(position.Row, frozenCell.Row));
			}
			anchor.CellPosition = position;
			ScrollInfo scrollInfo = documentLayout.ScrollInfo;
			if (scrollInfo == null) 
				return anchor;
			bool verticalUpdated = UpdateAnchorVerticalPosition(anchor, scrollInfo);
			bool horizontalUpdated = UpdateAnchorHorizontalPosition(anchor, scrollInfo);
			if (!horizontalUpdated && !verticalUpdated)
				return null;
			else {
				if (verticalUpdated)
					anchor.CellPosition = new CellPosition(Math.Min(position.Column, anchor.CellPosition.Column), anchor.CellPosition.Row);
				return anchor;
			}
		}
		bool IsForbiddenScrolling(CellPosition newTopLeftCell) {
			if (!ActiveSheet.IsFrozen())
				return false;
			CellPosition topLeftCell = SheetView.TopLeftCell;
			bool isNewColumnLessFrozenColumn = newTopLeftCell.Column < topLeftCell.Column;
			if (ActiveSheet.IsOnlyColumnsFrozen())
				return isNewColumnLessFrozenColumn;
			bool isNewRowLessFrozenRow = newTopLeftCell.Row < topLeftCell.Row;
			if (ActiveSheet.IsOnlyRowsFrozen())
				return isNewRowLessFrozenRow;
			return isNewRowLessFrozenRow || isNewColumnLessFrozenColumn;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = true;
			state.Visible = true;
			state.Checked = false;
		}
		protected internal abstract bool ChangeSelection();
		protected internal virtual void PerformModifyModel() {
		}
	}
	#endregion
}
