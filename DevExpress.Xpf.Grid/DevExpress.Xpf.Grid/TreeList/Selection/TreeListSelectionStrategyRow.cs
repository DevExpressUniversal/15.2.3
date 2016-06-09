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

using System.Windows;
using System;
using DevExpress.Xpf.Grid.Native;
using System.Windows.Input;
using DevExpress.Xpf.Editors.Helpers;
using System.Linq;
using DevExpress.Xpf.Core;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Data.Browsing;
using DevExpress.Utils;
#endif
namespace DevExpress.Xpf.Grid.TreeList {
	public class TreeListSelectionStrategyRow : SelectionStrategyRowBase {
		int selectionAnchorRowHandle, oldFocusedRowHandle;
		bool isSelectionLocked;
		public TreeListSelectionStrategyRow(TreeListView view)
			: base(view) {
		}
		protected TreeListView TreeListView { get { return this.view as TreeListView; } }
		protected TreeListDataProvider DataProvider { get { return TreeListView.TreeListDataProvider; } }
		#region SelectionMethods
		public override bool IsRowSelected(int rowHandle) {
			return DataProvider.Selection.GetSelected(rowHandle);
		}
		public virtual bool IsRowSelected(TreeListNode node) {
			return DataProvider.TreeListSelection.GetSelected(node);
		}
		public override void SelectRow(int rowHandle) {
			if(!IsValidRowHandle(rowHandle)) return;
			DataProvider.Selection.SetSelected(rowHandle, true);
		}
		public virtual void SelectRow(TreeListNode node) {
			if(node == null) return;
			DataProvider.TreeListSelection.SetSelected(node, true);
		}
		public override void UnselectRow(int rowHandle) {
			if(!IsValidRowHandle(rowHandle)) return;
			DataProvider.Selection.SetSelected(rowHandle, false);
		}
		public virtual void UnselectRow(TreeListNode node) {
			if(node == null) return;
			DataProvider.TreeListSelection.SetSelected(node, false);
		}
		public override void ClearSelection() {
			DataProvider.Selection.Clear();
		}
		public override void BeginSelection() {
			DataProvider.Selection.BeginSelection();
		}
		public override void EndSelection() {
			DataProvider.Selection.EndSelection();
		}
		protected override void SelectAllRows() {
			view.DataProviderBase.Selection.SelectAll();
		}
		public override int[] GetSelectedRows() {
			return DataProvider.Selection.GetSelectedRows();
		}
		public virtual TreeListNode[] GetSelectedNodes() {
			return DataProvider.TreeListSelection.GetSelectedNodes();
		}
		protected virtual bool IsValidRowHandle(int rowHandle) {
			return DataProvider.IsValidRowHandle(rowHandle);
		}
		public override void OnInvertSelection() {
			if(!IsValidRowHandle(TreeListView.FocusedRowHandle)) return;
			InvertRowSelection(TreeListView.FocusedRowHandle);
		}
		protected virtual void InvertRowSelection(int rowHandle) {
			if(IsRowSelected(rowHandle))
				UnselectRow(rowHandle);
			else
				SelectRow(rowHandle);
		}
		public override SelectionState GetRowSelectionState(int rowHandle) {
			if(rowHandle == view.FocusedRowHandle)
				return IsRowSelected(rowHandle) ? SelectionState.Focused : SelectionState.None;
			return IsRowSelected(rowHandle) ? SelectionState.Selected : SelectionState.None;
		}
		protected override SelectionState GetCellSelectionStateCore(int rowHandle, bool isFocused, bool isSelected) {
			if(!IsRowSelected(rowHandle))
				return SelectionState.None;
			return base.GetCellSelectionStateCore(rowHandle, isFocused, isSelected);
		}
		public override bool UpdateBorderForFocusedElementCore() {
			if(TreeListView.FocusedRowHandle == GridControl.AutoFilterRowHandle) {
				if(TreeListView.CurrentCellEditor != null)
					TreeListView.SetFocusedRectangleOnCell();
				return true;
			}
			if(!IsRowSelected(view.FocusedRowHandle) || view.ShowFocusedRectangle) {
				if(TreeListView.NavigationStyle == GridViewNavigationStyle.Cell)
					TreeListView.SetFocusedRectangleOnCell();
				else
					TreeListView.SetFocusedRectangleOnRow();
				return true;
			}
			return false;
		}
		public override void SetFocusedRowSelected() {
			SetSingleRowSelectedCore(TreeListView.FocusedRowHandle);
		}
		void SetSingleRowSelectedCore(int rowHandle) {
			if(!IsValidRowHandle(rowHandle)) return;
			BeginSelection();
			try {
				ClearSelection();
				SelectRow(rowHandle);
				SetSelectionAnchorRowHandle(rowHandle);
			} finally {
				EndSelection();
			}
		}
		protected override void OnAssignedToGridCore() {
			base.OnAssignedToGridCore();
			SetFocusedRowSelected();
		}
		protected void SetSelectionAnchorRowHandle(int rowHandle) {
			this.selectionAnchorRowHandle = rowHandle;
		}
		public override void OnFocusedRowHandleChanged(int oldRowHandle) {
			if(!isSelectionLocked) {
				SetSelectionAnchorRowHandle(view.FocusedRowHandle);
				oldFocusedRowHandle = TreeListView.FocusedRowHandle;
			}
		}
		public override void CopyToClipboard() {
			int[] selectedRows = GetSelectedRows();
			if((selectedRows.Length == 0) && !view.IsInvalidFocusedRowHandle) {
				selectedRows = new int[] { view.FocusedRowHandle };
			}
			TreeListView.DataControl.CopyRowsToClipboard(selectedRows);
		}
		#endregion
		#region MouseSelection
		public override void OnBeforeMouseLeftButtonDown(DependencyObject originalSource) {
			base.OnBeforeMouseLeftButtonDown(originalSource);
			oldFocusedRowHandle = TreeListView.FocusedRowHandle;
			isSelectionLocked = true;
			view.EditorSetInactiveAfterClick = false;
		}
		public override void OnAfterMouseLeftButtonDown(IDataViewHitInfo hitInfo) {
			base.OnAfterMouseLeftButtonDown(hitInfo);
			OnAfterMouseLeftButtonDownCore(hitInfo);
			isSelectionLocked = false;
		}
		Action postponedSelectionAction = null;
		protected virtual void OnAfterMouseLeftButtonDownCore(IDataViewHitInfo hitInfo) {
			postponedSelectionAction = null;
			if(view.IsEditing) return;
			TreeListViewHitInfo treeListHitInfo = (TreeListViewHitInfo)hitInfo;
			if(treeListHitInfo.InNodeIndent && !treeListHitInfo.InNodeImage) return;
			if(!IsValidRowHandle(hitInfo.RowHandle) || HasValidationError) return;
			if(Mouse.RightButton == MouseButtonState.Pressed && (IsRowSelected(hitInfo.RowHandle) || IsControlPressed || IsMultipleMode)) {
				SetSelectionAnchorRowHandle(hitInfo.RowHandle);
				return;
			}
			if(IsMultipleMode) {
				SetSelectionAnchorRowHandle(hitInfo.RowHandle);
				TreeListView.EditorSetInactiveAfterClick = true;
				if(IsRowSelected(hitInfo.RowHandle) && !IsControlPressed && !IsShiftPressed) {
					postponedSelectionAction = () => InvertRowSelection(hitInfo.RowHandle);
				}
				else {
					InvertRowSelection(hitInfo.RowHandle);
				}
				return;
			}
			if(!IsShiftPressed) {
				SetSelectionAnchorRowHandle(hitInfo.RowHandle);
				if(!IsControlPressed && IsRowSelected(hitInfo.RowHandle) && TreeListView.DataControl.GetSelectedRowHandles().Length > 1) {
					TreeListView.EditorSetInactiveAfterClick = true;
					postponedSelectionAction = delegate {
						SetSingleRowSelectedCore(hitInfo.RowHandle);
					};
					return;
				}
			}
			if(IsRowSelected(hitInfo.RowHandle) && !IsShiftPressed && !IsControlPressed)
				return;
			if(!IsShiftPressed)
				if(IsControlPressed) {
					InvertRowSelection(hitInfo.RowHandle);
					TreeListView.EditorSetInactiveAfterClick = true;
					return;
				}
			BeginSelection();
			try {
				if(!IsControlPressed)
					ClearSelection();
				if(!IsShiftPressed) {
					if(!IsControlPressed)
						SelectRow(hitInfo.RowHandle);
			   } else {
					SelectRange(TreeListView.FocusedRowHandle, selectionAnchorRowHandle);
					TreeListView.EditorSetInactiveAfterClick = true;
				}
			} finally {
				EndSelection();
			}
		}
		public override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			if(postponedSelectionAction != null && view.FocusedRowHandle == view.GetRowHandleByMouseEventArgs(e))
				postponedSelectionAction();
			postponedSelectionAction = null;
		}
		#endregion
		#region KeyboardSelection
		public override void OnBeforeProcessKeyDown(System.Windows.Input.KeyEventArgs e) {
			base.OnBeforeProcessKeyDown(e);
			isSelectionLocked = true;
			oldFocusedRowHandle = TreeListView.FocusedRowHandle;
		}
		public override void OnAfterProcessKeyDown(System.Windows.Input.KeyEventArgs e) {
#if !SILVERLIGHT
			if(e.Key == Key.Next || e.Key == Key.Prior)
#else
			if(e.Key == Key.PageDown || e.Key == Key.PageUp)
#endif
				return;
			OnNavigationComplete(e.Key == Key.Tab && view.NavigationStyle == GridViewNavigationStyle.Row);
		}
		public override void OnNavigationComplete(bool isTabPressed) {
			OnNavigationCompleteCore(isTabPressed);
			isSelectionLocked = false;
		}
		protected virtual void OnNavigationCompleteCore(bool isTabPressed) {
			if(oldFocusedRowHandle == TreeListView.FocusedRowHandle || !IsValidRowHandle(TreeListView.FocusedRowHandle) || IsMultipleMode)
				return;
			BeginSelection();
			try {
				if(!IsControlPressed && !isTabPressed)
					ClearSelection();
				if(!IsShiftPressed || isTabPressed) {
					if(!IsControlPressed)
						SelectRow(TreeListView.FocusedRowHandle);
					SetSelectionAnchorRowHandle(TreeListView.FocusedRowHandle);
				} else {
					SelectRange(TreeListView.FocusedRowHandle, selectionAnchorRowHandle);
				}
			} finally {
				EndSelection();
			}
		}
		protected override bool ShouldInvertSelectionOnSpace() {
			return base.ShouldInvertSelectionOnSpace() && !TreeListView.ShowCheckboxes;
		}
		#endregion
	}
}
