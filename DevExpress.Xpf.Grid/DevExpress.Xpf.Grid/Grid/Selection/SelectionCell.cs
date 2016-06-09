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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
#if SL
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using DevExpress.Data;
using DevExpress.Utils;
#endif
namespace DevExpress.Xpf.Grid.Native {
	public abstract class SelectionStrategyCellBase : MultiSelectionStrategyBase {
		protected DataViewBase DataView { get { return view as DataViewBase; } }
		ColumnBase CurrentColumn { get { return (ColumnBase)view.DataControl.CurrentColumn; } }
		public SelectionStrategyCellBase(DataViewBase view) : base(view) { }
		public override bool IsRowSelected(int rowHandle) {
			if(!view.IsFocusedView) return false;
			return view.DataProviderBase.Selection.GetSelected(rowHandle);
		}
		public override SelectionState GetRowSelectionState(int rowHandle) {
			if(!view.IsFocusedView || !view.DataControl.IsGroupRowHandleCore(rowHandle))
				return SelectionState.None;
			if(rowHandle == view.FocusedRowHandle)
				return IsRowSelected(rowHandle) ? SelectionState.Focused : SelectionState.None;
			return IsRowSelected(rowHandle) ? SelectionState.Selected : SelectionState.None;
		}
		protected override SelectionState GetCellSelectionStateCore(int rowHandle, bool isFocused, bool isSelected) {
			if(isFocused) {
				if(view.IsEditing)
					return SelectionState.None;
				return isSelected ? SelectionState.FocusedAndSelected : SelectionState.None;
			}
			return isSelected ? SelectionState.Selected : SelectionState.None;
		}
		public override int[] GetSelectedRows() {
			return view.DataProviderBase.Selection.GetSelectedRows();
		}
		public override void BeginSelection() {
			view.DataProviderBase.Selection.BeginSelection();
		}
		public override void EndSelection() {
			view.DataProviderBase.Selection.EndSelection();
		}
		Locker updateSelectionBoundsLocker = new Locker();
		public override void OnFocusedRowHandleChanged(int oldRowHandle) {
			UpdateSelectionBounds();
		}
		public override void OnFocusedColumnChanged() {
			UpdateSelectionBounds();
		}
		public override void SelectCell(int rowHandle, ColumnBase column) {
			SelectCellCore(rowHandle, column, false);
		}
		public override void SelectRow(int rowHandle) {
			if(view.DataControl.IsGroupRowHandleCore(rowHandle)) {
				view.DataProviderBase.Selection.SetSelected(rowHandle, true);
				return;
			}
			if(view.VisibleColumnsCore.Count == 0)
				return;
			SetCellsSelection(rowHandle, view.VisibleColumnsCore[0], rowHandle, view.VisibleColumnsCore[view.VisibleColumnsCore.Count - 1], true);
		}
		public override void UnselectRow(int rowHandle) {
			if(view.DataControl.IsGroupRowHandleCore(rowHandle)) {
				view.DataProviderBase.Selection.SetSelected(rowHandle, false);
				return;
			}
			if(view.VisibleColumnsCore.Count == 0)
				return;
			SetCellsSelection(rowHandle, view.VisibleColumnsCore[0], rowHandle, view.VisibleColumnsCore[view.VisibleColumnsCore.Count - 1], false);
		}
		internal override void SelectAllMasterDetail() {
			SelectAll();
		}
		public override void SelectAll() {
			if(view.DataProviderBase.VisibleCount == 0 || view.VisibleColumnsCore.Count == 0)
				return;
			BeginSelection();
			ClearSelection();
			for(int i = 0; i < view.DataControl.VisibleRowCount; i++)
				SelectRow(view.DataControl.GetRowHandleByVisibleIndexCore(i));
			EndSelection();
		}
		protected void SelectCellCore(int rowHandle, ColumnBase column, bool useSelectionCount) {
			if(column == null)
				return;
			Dictionary<ColumnBase, int> selectedColumns = GetSelectedCells(rowHandle);
			if(selectedColumns == null)
				selectedColumns = new Dictionary<ColumnBase, int>();
			if(selectedColumns.ContainsKey(column)) {
				if(useSelectionCount) selectedColumns[column]++;
				return;
			}
			selectedColumns[column] = 0;
			view.DataProviderBase.Selection.SetActuallyChanged();
			if(GetSelectedCells(rowHandle) == null)
				SetCellSelection(rowHandle, selectedColumns);
			else
				RefreshSelectionForce(rowHandle);
		}
		public override void UnselectCell(int rowHandle, ColumnBase column) {
			UnselectCellCore(rowHandle, column, false);
		}
		protected void UnselectCellCore(int rowHandle, ColumnBase column, bool useSelectionCount) {
			Dictionary<ColumnBase, int> selectedColumns = GetSelectedCells(rowHandle);
			if(selectedColumns == null || !selectedColumns.ContainsKey(column))
				return;
			if(useSelectionCount && selectedColumns[column] != 0) {
				selectedColumns[column]--;
				return;
			}
			selectedColumns.Remove(column);
			view.DataProviderBase.Selection.SetActuallyChanged();
			if(selectedColumns.Count == 0)
				view.DataProviderBase.Selection.SetSelected(rowHandle, false);
			else
				RefreshSelectionForce(rowHandle);
		}
		void RefreshSelectionForce(int rowHandle) {
			if(!view.DataProviderBase.Selection.IsSelectionLocked) {
				DevExpress.Data.SelectionChangedEventArgs e = new DevExpress.Data.SelectionChangedEventArgs(CollectionChangeAction.Refresh, rowHandle);
				view.OnSelectionChanged(e);
			}
		}
		public override CellBase[] GetSelectedCells() {
			List<CellBase> list = new List<CellBase>();
			foreach(int rowHandle in view.DataProviderBase.Selection.GetSelectedRows()) {
				if(GetSelectedCells(rowHandle) == null)
					continue;
				foreach(ColumnBase column in GetSelectedCells(rowHandle).Keys)
					if(column.Visible)
						list.Add(CreateCell(rowHandle, column));
			}
			return list.ToArray<CellBase>();
		}
		Dictionary<ColumnBase, int> GetSelectedCells(int rowHandle) {
			return view.DataProviderBase.Selection.GetSelectedObject(rowHandle) as Dictionary<ColumnBase, int>;
		}
		void SetCellSelection(int rowHandle, Dictionary<ColumnBase, int> list) {
			view.DataProviderBase.Selection.SetSelected(rowHandle, true, list);
		}
		public override bool IsCellSelected(int rowHandle, ColumnBase column) {
			if(!view.IsFocusedView) return false;
			Dictionary<ColumnBase, int> selectedColumns = GetSelectedCells(rowHandle);
			if(selectedColumns == null || !selectedColumns.ContainsKey((ColumnBase)column))
				return false;
			return true;
		}
		public override void ClearSelection() {
			if(view.DataProviderBase != null)
				view.DataProviderBase.Selection.Clear();
		}
		public override void SetCellsSelection(int startRowHandle, ColumnBase startColumn, int endRowHandle, ColumnBase endColumn, bool setSelected) {
			SetCellsSelectionCore(startRowHandle, startColumn.VisibleIndex, endRowHandle, endColumn.VisibleIndex, setSelected, false);
		}
		protected void SetCellsSelectionCore(int startRowHandle, int startColumnIndex, int endRowHandle, int endColumnIndex, bool setSelected, bool useSelectionCount) {
			BeginSelection();
			try {
				DataView.ViewBehavior.IterateCells(startRowHandle, startColumnIndex, endRowHandle, endColumnIndex,
					delegate(int rowHandle, ColumnBase column) {
						if(setSelected)
							SelectCellCore(rowHandle, column, useSelectionCount);
						else
							UnselectCellCore(rowHandle, column, useSelectionCount);
					});
			}
			finally {
				EndSelection();
			}
		}
		int oldRowHandle;
		ColumnBase oldColumn;
		public override void OnBeforeMouseLeftButtonDown(DependencyObject originalSource) {
			updateSelectionBoundsLocker.Lock();
			oldRowHandle = view.FocusedRowHandle;
			oldColumn = CurrentColumn;
		}
		CellBase selectionAnchor;
		bool IsElementFocused(IDataViewHitInfo hitInfo) {
			return view.FocusedRowHandle == hitInfo.RowHandle && (CurrentColumn == hitInfo.Column || view.DataControl.IsGroupRowHandleCore(view.FocusedRowHandle));
		}
		public override void OnAfterMouseLeftButtonDown(IDataViewHitInfo hitInfo) {
			OnAfterMouseLeftButtonDownCore(hitInfo);
			updateSelectionBoundsLocker.Unlock();
		}
		void OnAfterMouseLeftButtonDownCore(IDataViewHitInfo hitInfo) {
			if(view.IsEditing)
				return;
			ITableViewHitInfo tableViewHitInfo = (ITableViewHitInfo)hitInfo;
			if(!IsValidRowHandle(hitInfo.RowHandle))
				return;
			if(oldRowHandle == view.FocusedRowHandle && oldColumn == CurrentColumn &&
				!IsElementFocused(tableViewHitInfo) && !IsCellSelected(view.FocusedRowHandle, CurrentColumn) && ModifierKeysHelper.IsCtrlPressed(Keyboard.Modifiers)
				&& !tableViewHitInfo.IsRowIndicator)
				return;
			if(Mouse.RightButton == MouseButtonState.Pressed)
				if(oldRowHandle == view.FocusedRowHandle || ModifierKeysHelper.IsCtrlPressed(Keyboard.Modifiers)) {
					UpdateSelectionBoundsForce();
					return;
				}
			if(!ModifierKeysHelper.IsShiftPressed(Keyboard.Modifiers)) {
				UpdateSelectionBoundsForce();
			}
			if(CurrentColumn != null && IsCellSelected(view.FocusedRowHandle, CurrentColumn) && Mouse.RightButton == MouseButtonState.Pressed)
				return;
			BeginSelection();
			if(!ModifierKeysHelper.IsCtrlPressed(Keyboard.Modifiers) && !ModifierKeysHelper.IsShiftPressed(Keyboard.Modifiers))
				ClearSelection();
			if(!ModifierKeysHelper.IsShiftPressed(Keyboard.Modifiers)) {
				if(ModifierKeysHelper.IsCtrlPressed(Keyboard.Modifiers))
					OnInvertSelectionCore(tableViewHitInfo.IsRowIndicator);
				else
					if(view.DataControl.IsGroupRowHandleCore(view.FocusedRowHandle) || tableViewHitInfo.IsRowIndicator)
						SelectRow(view.FocusedRowHandle);
					else
						SelectCell(view.FocusedRowHandle, CurrentColumn);
			}
			else {
				if(selectionAnchor == null) {
					selectionAnchor = IsValidRowHandle(oldRowHandle) ? CreateCell(oldRowHandle, oldColumn) : CreateCell(view.FocusedRowHandle, CurrentColumn);
				}
				if(!tableViewHitInfo.IsRowIndicator)
					SelectAnchorRange(view.FocusedRowHandle, CurrentColumn.VisibleIndex, selectionAnchor.RowHandleCore, selectionAnchor.ColumnCore.VisibleIndex);
				else
					SelectAnchorRange(view.FocusedRowHandle, 0, selectionAnchor.RowHandleCore, view.VisibleColumnsCore.Count);
			}
			EndSelection();
		}
		void UpdateSelectionBounds() {
			if(!updateSelectionBoundsLocker.IsLocked)
				UpdateSelectionBoundsForce();
		}
		void UpdateSelectionBoundsForce() {
			selectionAnchor = IsValidRowHandle(view.FocusedRowHandle) && CurrentColumn != null ? CreateCell(view.FocusedRowHandle, CurrentColumn) : null;
			oldSelectionRectangle = Rectangle.Empty;
			oldRowHandle = view.FocusedRowHandle;
			oldColumn = CurrentColumn;
		}
		bool IsValidRowHandle(int rowHandle) {
			return rowHandle != GridControl.InvalidRowHandle && rowHandle != GridControl.NewItemRowHandle && rowHandle != GridControl.AutoFilterRowHandle;
		}
		public override void UpdateSelectionRect(int rowHandle, ColumnBase column) {
			if(selectionAnchor == null || selectionAnchor.RowHandleCore != view.FocusedRowHandle || selectionAnchor.ColumnCore != CurrentColumn) {
				selectionAnchor = CreateCell(view.FocusedRowHandle, CurrentColumn);
				oldSelectionRectangle = Rectangle.Empty;
			}
			if(selectionAnchor.ColumnCore != null)
				SelectAnchorRange(selectionAnchor.RowHandleCore, selectionAnchor.ColumnCore.VisibleIndex, rowHandle, column.VisibleIndex);
		}
		public override void OnBeforeProcessKeyDown(KeyEventArgs e) {
			updateSelectionBoundsLocker.Lock();
			oldRowHandle = view.FocusedRowHandle;
			oldColumn = CurrentColumn;
		}
		public override void OnAfterProcessKeyDown(KeyEventArgs e) {
#if !SILVERLIGHT
			if(e.Key == Key.Next || e.Key == Key.Prior)
#else
			if(e.Key == Key.PageDown || e.Key == Key.PageUp)
#endif
				return;
			OnNavigationComplete(e.Key == Key.Tab);
		}
		public override void OnNavigationComplete(bool isTabPressed) {
			OnNavigationCompleteCore(isTabPressed);
			updateSelectionBoundsLocker.Unlock();
		}
		void OnNavigationCompleteCore(bool isTabPressed) {
			if(oldRowHandle == view.FocusedRowHandle && oldColumn == CurrentColumn)
				return;
			if(!IsValidRowHandle(view.FocusedRowHandle))
				return;
			BeginSelection();
			if(!ModifierKeysHelper.IsCtrlPressed(Keyboard.Modifiers) && !(ModifierKeysHelper.IsShiftPressed(Keyboard.Modifiers) && !isTabPressed))
				ClearSelection();
			if(!ModifierKeysHelper.IsShiftPressed(Keyboard.Modifiers) || isTabPressed) {
				if(!ModifierKeysHelper.IsCtrlPressed(Keyboard.Modifiers))
					SelectCell(view.FocusedRowHandle, CurrentColumn);
				UpdateSelectionBoundsForce();
			}
			else {
				if(selectionAnchor == null) {
					selectionAnchor = IsValidRowHandle(oldRowHandle) ? CreateCell(oldRowHandle, oldColumn) : CreateCell(view.FocusedRowHandle, CurrentColumn);
				}
				SelectAnchorRange(view.FocusedRowHandle, view.NavigationIndex, selectionAnchor.RowHandleCore, selectionAnchor.ColumnCore.VisibleIndex);
			}
			EndSelection();
		}
		Rectangle oldSelectionRectangle;
		void SelectAnchorRange(int rowHandle1, int colIndex1, int rowHandle2, int colIndex2) {
			if(rowHandle1 > rowHandle2) {
				int a = rowHandle1; rowHandle1 = rowHandle2; rowHandle2 = a;
			}
			if(colIndex1 > colIndex2) {
				int a = colIndex1; colIndex1 = colIndex2; colIndex2 = a;
			}
			Rectangle rect = new Rectangle(colIndex1, rowHandle1, colIndex2 - colIndex1 + 1, rowHandle2 - rowHandle1 + 1);
			if(!oldSelectionRectangle.IsEmpty && oldSelectionRectangle.IntersectsWith(rect)) {
				SetCellsSelectionCore(oldSelectionRectangle.Top, oldSelectionRectangle.Left, oldSelectionRectangle.Bottom - 1, oldSelectionRectangle.Right - 1, false, true);
			}
			oldSelectionRectangle = rect;
			SetCellsSelectionCore(rowHandle1, colIndex1, rowHandle2, colIndex2, true, true);
		}
		public override void OnInvertSelection() {
			OnInvertSelectionCore(false);
		}
		void OnInvertSelectionCore(bool invertSelectionForEntireRow) {
			if(!IsValidRowHandle(view.FocusedRowHandle))
				return;
			if(view.DataControl.IsGroupRowHandleCore(view.FocusedRowHandle) || invertSelectionForEntireRow) {
				if(IsRowSelected(view.FocusedRowHandle))
					UnselectRow(view.FocusedRowHandle);
				else
					SelectRow(view.FocusedRowHandle);
				return;
			}
			if(IsCellSelected(view.FocusedRowHandle, CurrentColumn))
				UnselectCell(view.FocusedRowHandle, CurrentColumn);
			else
				SelectCell(view.FocusedRowHandle, CurrentColumn);
		}
		public override bool UpdateBorderForFocusedElementCore() {
			if(view.DataControl.IsGroupRowHandleCore(view.FocusedRowHandle) && (!IsRowSelected(view.FocusedRowHandle) || view.ShowFocusedRectangle)) {
				view.SetFocusedRectangleOnGroupRow();
				return true;
			}
			if(view.FocusedRowHandle == GridControl.NewItemRowHandle || view.FocusedRowHandle == GridControl.AutoFilterRowHandle) {
				view.SetFocusedRectangleOnCell();
				return true;
			}
			if(CurrentColumn != null && (!IsCellSelected(view.FocusedRowHandle, CurrentColumn) || view.ShowFocusedRectangle)) {
				view.SetFocusedRectangleOnCell();
				return true;
			}
			return false;
		}
		protected abstract CellBase CreateCell(int rowHandle, ColumnBase column);
		protected override object GetSelection() {
			Dictionary<int, List<ColumnBase>> selection = new Dictionary<int, List<ColumnBase>>();
			foreach(int rowHandle in view.DataProviderBase.Selection.GetSelectedRows()) {
				selection[rowHandle] = new List<ColumnBase>();
				Dictionary<ColumnBase, int> selectedCells = GetSelectedCells(rowHandle);
				if(selectedCells == null) continue;
				foreach(ColumnBase column in selectedCells.Keys)
					selection[rowHandle].Add(column);
			}
			return selection;
		}
		protected override void UpdateVisualState(object oldSelection) {
			Dictionary<int, List<ColumnBase>> selection = (Dictionary<int, List<ColumnBase>>)oldSelection;
			foreach(int rowHandle in selection.Keys) {
				List<ColumnBase> currentColumns = null;
				view.UpdateRowDataByRowHandle(rowHandle, rowData => {
					Dictionary<ColumnBase, int> selectedCells = GetSelectedCells(rowHandle);
					if(currentColumns == null) rowData.UpdateIsSelected(selectedCells != null || IsRowSelected(rowHandle));
					foreach(ColumnBase column in selection[rowHandle]) {
						if(rowData.CellDataCache.ContainsKey(column) && (currentColumns == null || !currentColumns.Contains(column)))
							((GridCellData)rowData.CellDataCache[column]).UpdateIsSelected(rowHandle, selectedCells != null && selectedCells.Keys.Contains(column));
					}
				});
			}
		}
		protected override bool ShouldInvertSelectionOnSpace() {
			return false;
		}
	}
	public class SelectionStrategyCell : SelectionStrategyCellBase {
		public SelectionStrategyCell(TableView view)
			: base(view) {
		}
		protected TableView TableView { get { return DataView as TableView; } }
		public override void CopyToClipboard() {
			TableView.CopySelectedCellsToClipboard();
		}
		protected override CellBase CreateCell(int rowHandle, ColumnBase column) {
			return ((ITableView)TableView).CreateGridCell(rowHandle, column); 
		}
	}
}
