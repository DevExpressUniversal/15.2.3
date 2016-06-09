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
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Grid.Native {
	public abstract class GridViewNavigationBase {
		DataViewBase view;
		protected internal Locker NavigationMouseLocker = new Locker();
		protected GridViewNavigationBase(DataViewBase view) {
			this.view = view;
		}
		public DataViewBase View { get { return view; } }
		bool IsRightToLeft { get { return view.DataControl != null && view.DataControl.FlowDirection == FlowDirection.RightToLeft; } }
		protected DataControlBase DataControl { get { return view.DataControl; } }
		public virtual void OnLeft(bool isCtrlPressed) { }
		public virtual void OnRight(bool isCtrlPressed) { }
		public virtual void OnUp(bool isCtrlPressed) { }
		public virtual void OnDown() { }
		public virtual void OnPageUp() { }
		public virtual void OnPageUp(KeyEventArgs e) { OnPageUp(); }
		public virtual void OnPageDown() { }
		public virtual void OnPageDown(KeyEventArgs e) { OnPageDown(); }
		public virtual void OnHome(KeyEventArgs e) { }
		public virtual void OnEnd(KeyEventArgs e) { }
		public virtual bool OnPlus(bool isCtrlPressed) { return false; }
		public virtual bool OnMinus(bool isCtrlPressed) { return false; }
		public virtual void OnTab(bool isShiftPressed) { }
		public virtual bool CanSelectCell { get { return false; } }
		public virtual bool GetIsFocusedCell(int rowHandle, ColumnBase column) {
			return false;
		}
		public virtual bool ShouldRaiseRowAutomationEvents { get { return false; } }
		protected internal virtual void UpdateRowsState() {
		}
		protected internal virtual void ProcessMouse(DependencyObject originalSource) { }
		protected internal virtual void ProcessKey(KeyEventArgs e) {
			if(View.IsEditFormVisible)
				return;
			if(RightToLeftHelper.IsLeftKey(e.Key, IsRightToLeft)) {
				OnLeft(ModifierKeysHelper.IsCtrlPressed(ModifierKeysHelper.GetKeyboardModifiers(e)));
				e.Handled = true;
				return;
			}
			if(RightToLeftHelper.IsRightKey(e.Key, IsRightToLeft)) {
				OnRight(ModifierKeysHelper.IsCtrlPressed(ModifierKeysHelper.GetKeyboardModifiers(e)));
				e.Handled = true;
				return;
			}
			switch(e.Key) {
				case Key.Up:
					OnUp(ModifierKeysHelper.IsCtrlPressed(ModifierKeysHelper.GetKeyboardModifiers(e)));
					e.Handled = true;
					break;
				case Key.Down:
					OnDown();
					e.Handled = true;
					break;
				case Key.PageUp:
					OnPageUp(e);
					e.Handled = true;
					break;
				case Key.PageDown:
					OnPageDown(e);
					e.Handled = true;
					break;
				case Key.Home:
					OnHome(e);
					e.Handled = true;
					break;
				case Key.End:
					OnEnd(e);
					e.Handled = true;
					break;
				case Key.OemPlus:
				case Key.Add:
					if(!View.IsKeyboardFocusInSearchPanel())
						e.Handled = OnPlus(ModifierKeysHelper.IsOnlyCtrlPressed(ModifierKeysHelper.GetKeyboardModifiers(e)));
					break;
				case Key.OemMinus:
				case Key.Subtract:
					if(!View.IsKeyboardFocusInSearchPanel())
						e.Handled = OnMinus(ModifierKeysHelper.IsOnlyCtrlPressed(ModifierKeysHelper.GetKeyboardModifiers(e)));
					break;
				case Key.Tab:
					if(!ModifierKeysHelper.IsCtrlPressed(ModifierKeysHelper.GetKeyboardModifiers(e)) && !View.AllowLeaveFocusOnTab && !View.IsKeyboardFocusInSearchPanel()) {
						OnTab(ModifierKeysHelper.IsShiftPressed(ModifierKeysHelper.GetKeyboardModifiers(e)));
						e.Handled = true;
					}
					break;
			}
		}
		protected internal virtual void ClearAllStates() { }
		protected virtual bool ShouldExpandRow() {
			return View.IsExpandableRowFocused() && !View.IsExpanded(View.FocusedRowHandle);
		}
		protected virtual bool ShouldCollapseRow() {
			return View.IsExpandableRowFocused() && View.IsExpanded(View.FocusedRowHandle);
		}
	}
	public abstract class GridViewRowNavigationBase : GridViewNavigationBase {
		protected GridViewRowNavigationBase(DataViewBase view) : base(view) { }
		public override void OnLeft(bool isCtrlPressed) {
			if(View.IsAdditionalRowFocused)
				return;
			if(View.FocusedRowElement == null)
				return;
			if(View.IsExpandableRowFocused()) {
				if(View.IsExpanded(View.FocusedRowHandle))
					View.CollapseFocusedRowCore();
				else
					View.MoveParentRow();
			}
			else {
				IScrollInfo scrollInfo = View.RootDataPresenter;
				if(scrollInfo.ExtentWidth <= scrollInfo.ViewportWidth)
					View.MoveParentRow();
				else
					scrollInfo.LineLeft();
			}
		}
		public override void OnRight(bool isCtrlPressed) {
			if(View.IsAdditionalRowFocused) {
				return;
			}
			if(View.FocusedRowElement == null)
				return;
			if(View.IsExpandableRowFocused()) {
				if(!View.IsExpanded(View.FocusedRowHandle))
					View.ExpandFocusedRowCore();
				else
					View.MoveNextRow();
			} else {
				IScrollInfo scrollInfo = View.RootDataPresenter;
				scrollInfo.LineRight();
			}
		}
		public override void OnUp(bool isCtrlPressed) {
			View.MovePrevRow();
		}
		public override void OnDown() {
			View.MoveNextRow();
		}
		public override void OnPageUp() {
			View.MovePrevPage();
		}
		public override void OnPageDown() {
			View.MoveNextPage();
		}
		public override void OnHome(KeyEventArgs e) {
			View.MoveFirstOrFirstMasterRow();
		}
		public override void OnEnd(KeyEventArgs e) {
			View.MoveLastOrLastMasterRow();
		}
		public override bool OnPlus(bool isCtrlPressed) {
			if(View.IsAutoFilterRowFocused) {
				return false;
			}
			if(isCtrlPressed) {
				if(View.DataControl.MasterDetailProvider.SetMasterRowExpanded(View.FocusedRowHandle, true, null)) 
					return true;
			}
			return View.ExpandFocusedRowCore();
		}
		public override bool OnMinus(bool isCtrlPressed) {
			if(View.IsAutoFilterRowFocused) {
				return false;
			}
			if(isCtrlPressed) {
				if(View.DataControl.MasterDetailProvider.SetMasterRowExpanded(View.FocusedRowHandle, false, null))
					return true;
			}
			return View.CollapseFocusedRowCore();
		}
		protected void TabNavigation(bool isShiftPressed) {
			if(isShiftPressed) {
				View.SelectRowForce();
				View.MovePrevCell(true);
			} else
				View.MoveNextCell(true);
		}
		public override bool ShouldRaiseRowAutomationEvents { get { return true; } }
		protected virtual void UpdateRowsStateCore() {
			GridRowsEnumerator en = View.CreateVisibleRowsEnumerator();
			while(en.MoveNext()) {
				FrameworkElement row = en.CurrentRow;
				if(row != null) {
					SetRowFocus(row, en.CurrentRowData.IsFocusedRow());
				}
			}
		}
		protected internal override void ClearAllStates() {
			if(View == null || View.DataControl == null)
				return;
			GridRowsEnumerator en = new GridRowsEnumerator(View, View.RootNodeContainer);
			while(en.MoveNext()) {
				SetRowFocus(en.CurrentRow, false);
			}
		}
		protected internal override void ProcessMouse(DependencyObject originalSource) {
			int handle = View.FindRowHandle(originalSource);
			if(handle == DataControlBase.InvalidRowHandle) return;
			NavigationMouseLocker.DoLockedAction(() => View.FocusViewAndRow(View, handle));
		}
		protected void ProcessMouseOnCell(DependencyObject originalSource) {
			DependencyObject cell = DataViewBase.FindParentCell(originalSource);
			if(cell == null && DataViewBase.FindParentRow(originalSource) == null)
				return;
			if(cell != null) {
				int navigationIndex = ColumnBase.GetNavigationIndex(cell);
				if(View.InplaceEditorOwner.IsActiveEditorHaveValidationError()) {
					DependencyObject row = DataViewBase.FindParentRow(cell);
					if(IsRowAndColumnChanged(DataViewBase.GetRowHandle(row).Value, navigationIndex)) {
						View.RowsStateDirty = true;
						return;
					}
				}
				bool canMoveToCell = View.VisibleColumnsCore[navigationIndex].AllowFocus || View.FocusedRowHandle == DataControlBase.AutoFilterRowHandle;
				if(!canMoveToCell) {
					if(DataControl.CurrentColumn != null && !DataControl.CurrentColumn.AllowFocus)
						DataControl.ReInitializeCurrentColumn();
					return;
				}
				View.NavigationIndex = navigationIndex;
			}
			View.RowsStateDirty = true;
		}
		protected virtual void SetRowFocus(DependencyObject row, bool focus) {
			DataViewBase.SetIsFocusedRow(row, focus);
			if(focus) View.ProcessFocusedElement();
		}
		protected void UpdateCellsStateCore(DependencyObject row, bool focusedRow) {
			GridCellsEnumerator en = new GridCellsEnumerator((FrameworkElement)row);
			while(en.MoveNext()) {
				bool focusCell = focusedRow && en.CurrentNavigationIndex == en.RowCurrentView.NavigationIndex && en.Current is UIElement && UIElementHelper.IsVisibleInTree((UIElement)en.Current);
				DataViewBase.SetIsFocusedCell(en.Current, focusCell);
				if(focusCell)
					en.RowCurrentView.CurrentCell = en.Current;
			}
		}
		protected void ClearAllCellsState(DependencyObject row) {
			GridCellsEnumerator en = new GridCellsEnumerator((FrameworkElement)row);
			while(en.MoveNext()) {
				DataViewBase.SetIsFocusedCell(en.Current, false);
			}
		}
		protected internal sealed override void UpdateRowsState() {
			if(View == null || View.DataControl == null)
				return;
			UpdateRowsStateCore();
		}
		protected void SetRowFocusOnCell(DependencyObject row, bool focus) {
			if(DataViewBase.GetIsFocusedRow(row) == focus && row != View.FocusedRowElement && !focus)
				return;
			UpdateCellsStateCore(row, focus);
		}
		protected void ClearAllCellsStates() {
			GridRowsEnumerator en = new GridRowsEnumerator(View, View.RootNodeContainer);
			while(en.MoveNext()) {
				ClearAllCellsState(en.CurrentRow);
				SetRowFocus(en.CurrentRow, false);
			}
			View.CurrentCell = null;
		}
		bool IsRowAndColumnChanged(int rowNavigationIndex, int cellNavigationIndex) {
			return (View.FocusedRowHandle != rowNavigationIndex && cellNavigationIndex != View.NavigationIndex);
		}
	}
}
