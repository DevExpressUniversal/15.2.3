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
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Editors.Helpers;
using System.Linq;
using DevExpress.Data;
using System.Collections.Generic;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Grid.Native {
	public class SelectionStrategyRow : SelectionStrategyRowBase {
		#region Properties
		protected bool IsRightMouseButtonPressed { get { return Mouse.RightButton == MouseButtonState.Pressed; } }
		protected SelectionActionBase SelectionAction { 
			get { return RootView.GlobalSelectionAction; }
			set { RootView.SetSelectionAction(value); }
		}
		protected GridViewBase GridView { get { return (GridViewBase)view; } }
		protected GridViewBase RootView { get { return (GridViewBase)view.RootView; } }
		protected GridControl Grid { get { return (GridControl)DataControl; } }
		protected int GroupRowCount { get { return DataControl.DataProviderBase.DataController.GroupRowCount; } }
		protected bool IsAllItemsSelected { get { return GetAllItemsSelected().HasValue && GetAllItemsSelected().Value; } }
		protected bool IsAllItemsUnselected { get { return SelectedRowCount == 0; } }
		protected bool IsOneDataRowSelected { get { return SelectedRowCount == 1 && !Grid.IsGroupRowHandle(GetSelectedRows()[0]); } }
		#endregion
		public SelectionStrategyRow(GridViewBase view) : base(view) { }
		#region Methods
		public override void SelectRow(int rowHandle) {
			SelectRowCore(rowHandle);
		}
		public override void UnselectRow(int rowHandle) {
			UnselectRowCore(rowHandle);
		}
		public override void ClearSelection() {
			if(view.DataProviderBase != null)
				view.DataProviderBase.Selection.Clear();
		}
		public override bool IsRowSelected(int rowHandle) {
			return view.DataProviderBase.Selection.GetSelected(rowHandle);
		}
		public override SelectionState GetRowSelectionState(int rowHandle) {
			if(!IsRowSelected(rowHandle))
				return SelectionState.None;
			if(rowHandle == view.FocusedRowHandle && view.IsFocusedView)
				return SelectionState.Focused;
			return SelectionState.Selected;
		}
		protected override SelectionState GetCellSelectionStateCore(int rowHandle, bool isFocused, bool isSelected) {
			if(!IsRowSelected(rowHandle))
				return SelectionState.None;
			return base.GetCellSelectionStateCore(rowHandle, isFocused, isSelected);
		}
		public override int[] GetSelectedRows() {
			return view.DataProviderBase.Selection.GetSelectedRows();
		}
		public override void OnAssignedToGrid() {
			base.OnAssignedToGrid();
			GridView.SetSelectionAnchor();
		}
		protected override void OnAssignedToGridCore() {
			if(DataControl.DataProviderBase.Selection.Count != 0) {
				UpdateSelection();
			}
			else if(view.IsRootView) {
				SetFocusedRowSelected();
			}
		}
		protected void UpdateSelection() {
			DevExpress.Data.SelectionChangedEventArgs e = new DevExpress.Data.SelectionChangedEventArgs(CollectionChangeAction.Refresh, GridControl.InvalidRowHandle);
			GridView.OnSelectionChanged(e);
		}
		public override void SetFocusedRowSelected() {
			DataControl.UpdateCurrentItem();
			ISelectionController selectionController = view.DataProviderBase.Selection;
			selectionController.BeginSelection();
			try {
				selectionController.Clear();
				selectionController.SetSelected(view.FocusedRowHandle, true);
				selectionController.SetActuallyChanged();
			} finally {
				selectionController.EndSelection();
			}
		}
		public override void BeginSelection() {
			view.DataProviderBase.Selection.BeginSelection();
		}
		public override void EndSelection() {
			if(view.DataProviderBase.Selection.IsSelectionLocked)
				view.DataProviderBase.Selection.EndSelection();
		}
		public override void OnFocusedRowHandleChanged(int oldRowHandle) {
			if(CanExecuteSelectionActionOnFocusedRowHandleChanged()) {
				ExecuteSelectionAction();
			} else {
				GridView.SetSelectionAnchor();
			}
		}
		protected virtual bool CanExecuteSelectionActionOnFocusedRowHandleChanged() {
			return SelectionAction != null && SelectionAction.CanFocusChangeDeleteAction;
		}
		protected virtual void ExecuteSelectionAction() {
			GridView.ExecuteSelectionAction();
		}
		public override void SelectRowForce() {
			if(IsExtendedMode)
				SelectionAction = new OnlyThisSelectionAction(GridView);
		}
		public sealed override void OnInvertSelection() {
			InvertSelectionForRow(view.FocusedRowHandle);
		}
		protected void InvertSelectionForRow(int rowHandle) {
			SetIsRowSelectedInternal(rowHandle, !IsRowSelected(rowHandle));
		}
		protected virtual void SetIsRowSelectedInternal(int rowHandle, bool isSelected) {
			view.DataProviderBase.Selection.SetSelected(rowHandle, isSelected);
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
			if(!IsRowSelected(view.FocusedRowHandle) || view.ShowFocusedRectangle) {
				if(RootView.NavigationStyle == GridViewNavigationStyle.Cell)
					view.SetFocusedRectangleOnCell();
				else
					view.SetFocusedRectangleOnRow();
				return true;
			}
			return false;
		}
		public override void CopyToClipboard() {
			int[] selectedRows = GetSelectedRows();
			if((selectedRows.Length == 0) && !view.IsInvalidFocusedRowHandle) {
				selectedRows = new int[] { view.FocusedRowHandle };
			}
			GridView.DataControl.CopyRowsToClipboard(selectedRows);
		}
		public override void CopyMasterDetailToClipboard() {
			Grid.CopyAllSelectedItemsToClipboard();
		}
		#endregion
		#region MouseSelection
		protected virtual bool ShouldProcessMouseClick(IDataViewHitInfo hitInfo) {
			return hitInfo.RowHandle != GridControl.InvalidRowHandle && !(IsMultipleMode && RootView.IsExpandButton(hitInfo));
		}
		public override void OnBeforeMouseLeftButtonDown(DependencyObject originalSource) {
			IDataViewHitInfo hitInfo = view.RootView.CalcHitInfoCore(originalSource);
			SelectionAction = null;
			if(!ShouldProcessMouseClick(hitInfo) || IsMultipleMode)
				return;
			int rowHandle = GridView.GetRowHandleByTreeElement(originalSource);
			bool canInvertSelection = CanInvertSelection(hitInfo);
			if(!canInvertSelection)
				SelectRowOnLMouseDown(rowHandle, originalSource);
		}
		public override void OnAfterMouseLeftButtonDown(IDataViewHitInfo hitInfo) {
			if(HasValidationError || IsRightMouseButtonPressed || !ShouldProcessMouseClick(hitInfo))
				return;
			int rowHandle = hitInfo.RowHandle;
			bool canInvertSelection = CanInvertSelection(hitInfo);
			if(canInvertSelection)
				InvertSelectionOnClick(hitInfo);
		}
		protected virtual bool CanInvertSelection(IDataViewHitInfo hitInfo) {
			if(IsControlPressed || IsMultipleMode) {
				if(!hitInfo.IsRowCell || hitInfo.RowHandle != view.FocusedRowHandle || hitInfo.Column != view.DataControl.CurrentColumn || view.ActiveEditor == null) {
					return true;
				}
			}
			return false;
		}
		protected virtual void InvertSelectionOnClick(IDataViewHitInfo hitInfo) {
			view.EditorSetInactiveAfterClick = true;
			if(CanInvertSelectionOnLMouseDown(hitInfo))
				InvertSelectionForRow(hitInfo.RowHandle);
			else
				SelectionAction = new InvertSelectionOnMouseUpAction(GridView);
		}
		protected virtual bool CanInvertSelectionOnLMouseDown(IDataViewHitInfo hitInfo) {
			return !view.IsRowSelected(hitInfo.RowHandle) || IsControlPressed || IsShiftPressed;
		}
		void SelectRowOnLMouseDown(int rowHandle, DependencyObject originalSource) {
			bool isIndicatorPressed = RootView.ViewBehavior.IsRowIndicator(originalSource) && !IsRightMouseButtonPressed;
			if(CanSelectRowOnLMouseDown(rowHandle, isIndicatorPressed)) {
				SelectRowOnLMouseDown(rowHandle);
			}
			else {
				CreateMouseSelectionActions(rowHandle, isIndicatorPressed);
			}
		}
		public override void CreateMouseSelectionActions(int rowHandle, bool isIndicatorPressed) {
			if(ModifierKeysHelper.IsShiftPressed(Keyboard.Modifiers)) {
				SelectionAction = new AddRowsToSelectionAction(GridView);
				return;
			}
			if(ModifierKeysHelper.IsCtrlPressed(Keyboard.Modifiers)) {
				SelectionAction = new AddOneRowToSelectionAction(GridView);
				return;
			}
			if(rowHandle == GridControl.NewItemRowHandle) {
				view.EditorSetInactiveAfterClick = false;
				SelectionAction = new OnlyThisSelectionAction(GridView);
				return;
			}
			if(rowHandle == GridControl.AutoFilterRowHandle) {
				view.EditorSetInactiveAfterClick = false;
				return;
			}
			if(view.IsRowSelected(rowHandle) && !isIndicatorPressed) {
				if(GridView.GetSelectedRowHandlesCore().Length > 1) {
					SelectionAction = new OnlyThisSelectionMouseUpAction(GridView);
				}
				else {
					view.EditorSetInactiveAfterClick = false;
				}
			}
			else {
				SelectionAction = new OnlyThisSelectionAction(GridView);
				if(GridView.GetSelectedRowHandlesCore().Length > 1) {
					view.EditorSetInactiveAfterClick = true;
				}
			}
		}
		bool CanSelectRowOnLMouseDown(int rowHandle, bool isIndicatorPressed) {
			if(IsControlPressed || rowHandle != view.FocusedRowHandle || !view.IsFocusedView)
				return false;
			return !view.IsRowSelected(rowHandle) || isIndicatorPressed;
		}
		void SelectRowOnLMouseDown(int rowHandle) {
			BeginSelection();
			try {
				ClearMasterDetailSelection();
				GridView.SetSelectionAnchor();
				if(rowHandle != GridControl.AutoFilterRowHandle && rowHandle != GridControl.NewItemRowHandle) {
					SelectRow(rowHandle);
				}
			}
			finally {
				EndSelection();
			}
		}
		public override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			if(view.FocusedRowHandle != GridView.GetRowHandleByMouseEventArgs(e)) {
				SelectionAction = null;
				return;
			}
			if((SelectionAction != null) && (!SelectionAction.CanFocusChangeDeleteAction)) {
				ExecuteSelectionAction();
			}
		}
		#endregion
		#region KeyboardSelection
		int oldFocusedRowHandle = GridControl.InvalidRowHandle;
		public override void OnBeforeProcessKeyDown(KeyEventArgs e) {
			oldFocusedRowHandle = view.FocusedRowHandle;
			if(IsNavigationKey(e)) {
				if(ModifierKeysHelper.IsShiftPressed(ModifierKeysHelper.GetKeyboardModifiers(e))) {
					SelectionAction = new AddRowsToSelectionAction(GridView);
				}
				else if(!ModifierKeysHelper.IsCtrlPressed(ModifierKeysHelper.GetKeyboardModifiers(e))) {
					SelectionAction = new OnlyThisSelectionAction(GridView);
				}
			}
		}
		public override void OnAfterProcessKeyDown(KeyEventArgs e) {
			if((e.Key != Key.Next) && (e.Key != Key.Prior)) {
				OnNavigationComplete(e.Key == Key.Tab);
			}
		}
		protected static bool IsNavigationKey(KeyEventArgs e) {
			return (e.Key == Key.Up) || (e.Key == Key.Down) ||
				   (e.Key == Key.PageUp) || (e.Key == Key.PageDown) ||
				   (e.Key == Key.Right) || (e.Key == Key.Left) || (e.Key == Key.Tab) ||
				   (e.Key == Key.Home) || (e.Key == Key.End);
		}
		public override void OnNavigationComplete(bool IsTabPressed) {
			if(oldFocusedRowHandle == view.FocusedRowHandle)
				SelectionAction = null;
		}
		public override void OnNavigationCanceled() {
			SelectionAction = null;
		}
		#endregion
		protected override void SelectAllRows() {
			DoSelectionAction(() => {
				SelectDataRows();
				SelectGroupRows();
			});
		}
		protected void SelectGroupRows() {
			GroupRowInfoCollection groupInfo = DataControl.DataProviderBase.DataController.GroupInfo;
			for(int index = 0; index < GroupRowCount; index++)
				SelectRowCore(groupInfo[index].Handle);
		}
	}
	public class SelectionStrategyCheckBoxRow : SelectionStrategyRow {
		TableView View { get { return (TableView)view; } }
		Locker CheckBoxSelectionChangedLocker = new Locker();
		Locker UpdateGroupRowsLocker = new Locker();
		protected override int TotalRowCount { get { return DataRowCount + GroupRowCount; } }
		public SelectionStrategyCheckBoxRow(TableView view) : base(view) { }
		public override bool? GetAllItemsSelected(int rowHandle) {
			return AreAllDescendantsSelected(rowHandle);
		}
		#region Helpers
		int GetRootGroupHandle(int groupRowHandle) {
			return DataProviderBase.DataController.GroupInfo.GetGroupRowInfoByHandle(groupRowHandle).RootGroup.Handle;
		}
		void DoLockedSelectionAction(Action action) {
			UpdateGroupRowsLocker.DoLockedAction(() => {
				DoSelectionAction(() => {
					action();
				});
			});
		}
		bool IsRowSelectedOrUnselected(SelectionChangedEventArgs e) {
			return e.Action != CollectionChangeAction.Refresh;
		}
		bool IsDataRowSelectedOrUnselected(SelectionChangedEventArgs e) {
			return !Grid.IsGroupRowHandle(e.ControllerRow) && IsRowSelectedOrUnselected(e);
		}
		#endregion
		#region MouseSelection
		bool IsDataRowCheckBox(IDataViewHitInfo hitInfo) {
			return hitInfo.Column == View.CheckBoxSelectorColumn;
		}
		bool IsGroupRowPressed(IDataViewHitInfo hitInfo) {
			return Grid.IsGroupRowHandle(hitInfo.RowHandle);
		}
		protected override bool ShouldProcessMouseClick(IDataViewHitInfo hitInfo) {
			if(hitInfo.RowHandle == DataControlBase.AutoFilterRowHandle)
				return false;
			if(View.RetainSelectionOnClickOutsideCheckBoxSelector)
				return IsDataRowCheckBox(hitInfo);
			if(IsGroupRowPressed(hitInfo) && !IsShiftPressed)
				return false;
			return base.ShouldProcessMouseClick(hitInfo);
		}
		protected override bool CanInvertSelection(IDataViewHitInfo hitInfo) {
			if(base.CanInvertSelection(hitInfo))
				return true;
			return IsDataRowCheckBox(hitInfo) && !IsShiftPressed;
		}
		protected override bool CanInvertSelectionOnLMouseDown(IDataViewHitInfo hitInfo) {
			if(IsDataRowCheckBox(hitInfo))
				return IsShiftPressed || IsControlPressed;
			return base.CanInvertSelectionOnLMouseDown(hitInfo);
		}
		public override void OnBeforeMouseLeftButtonDown(DependencyObject originalSource) {
			View.EditorSetInactiveAfterClick = false;
			base.OnBeforeMouseLeftButtonDown(originalSource);
		}
		#endregion
		#region SelectionMethods
		protected override void OnAssignedToGridCore() {
			UpdateSelection();
		}
		public override void SetFocusedRowSelected() { }
		public override void SelectRow(int rowHandle) {
			if(!Grid.IsGroupRowHandle(rowHandle))
				SelectRowCore(rowHandle);
		}
		internal override void SelectRowRecursively(int rowHandle) {
			if(Grid.IsGroupRowHandle(rowHandle))
				SelectAllItems(rowHandle);
			else
				SelectRowCore(rowHandle);
		}
		void SelectAllItems(int groupRowHandle) {
			DoLockedSelectionAction(() => {
				IterateChildren(groupRowHandle, rowHandle => SelectRowRecursively(rowHandle));
				UpdateGroupRow(groupRowHandle, true);
				CheckRowSelection(GetRootGroupHandle(groupRowHandle));
			});
		}
		public override void UnselectRow(int rowHandle) {
			if(!Grid.IsGroupRowHandle(rowHandle))
				UnselectRowCore(rowHandle);
		}
		internal override void UnselectRowRecursively(int rowHandle) {
			if(Grid.IsGroupRowHandle(rowHandle))
				UnselectAllItems(rowHandle);
			else
				UnselectRowCore(rowHandle);
		}
		void UnselectAllItems(int groupRowHandle) {
			DoLockedSelectionAction(() => {
				IterateChildren(groupRowHandle, rowHandle => UnselectRowRecursively(rowHandle));
				UpdateGroupRow(groupRowHandle, false);
				CheckRowSelection(GetRootGroupHandle(groupRowHandle));
			});
		}
		public override void SelectAll() {
			SelectAllRows();
		}
		protected override bool ShouldInvertSelectionOnSpace() {
			return base.ShouldInvertSelectionOnSpace() || (DataControl.CurrentColumn == View.CheckBoxSelectorColumn || Grid.IsGroupRowHandle(View.FocusedRowHandle));
		}
		protected override void SetIsRowSelectedInternal(int rowHandle, bool isSelected) {
			if(isSelected)
				SelectRowRecursively(rowHandle);
			else
				UnselectRowRecursively(rowHandle);
		}
		protected override bool CanExecuteSelectionActionOnFocusedRowHandleChanged() {
			if(base.CanExecuteSelectionActionOnFocusedRowHandleChanged())
				return !Grid.IsGroupRowHandle(View.FocusedRowHandle) || !ModifierKeysHelper.NoModifiers(ModifierKeysHelper.GetKeyboardModifiers());
			return false;
		}
		public override void SelectRange(int startRowHandle, int endRowHandle) {
			HashSet<int> rootGroups = new HashSet<int>();
			DoLockedSelectionAction(() => {
				SelectRangeCore(startRowHandle, endRowHandle, rowHandle => {
					if(Grid.IsGroupRowHandle(rowHandle)) {
						if(Grid.IsGroupRowExpanded(rowHandle))
							return;
						SelectRowRecursively(rowHandle);
					}
					else {
						SelectRowCore(rowHandle);
					}
					int parentRowHandle = Grid.GetParentRowHandle(rowHandle);
					if(Grid.IsGroupRowHandle(parentRowHandle)) {
						rootGroups.Add(GetRootGroupHandle(parentRowHandle));
					}
				});
				UpdateGroupRowsSelection(rootGroups);
			});
		}
		public override void SelectOnlyThisRange(int startRowHandle, int endRowHandle) {
			DoLockedSelectionAction(() => {
				DataControl.UnselectAll();
				SetGroupRowsSelection(false);
				SelectRange(startRowHandle, endRowHandle);
			});
		}
		public override bool? GetAllItemsSelected() {
			return AreAllItemsSelected(SelectedRowCount, TotalRowCount);
		}
		#endregion
		#region GroupChildren
		bool? AreAllChildrenSelected(int groupRowHandle) {
			if(IsAllItemsUnselected)
				return false;
			if(IsAllItemsSelected)
				return true;
			int childrenCount = Grid.GetChildRowCount(groupRowHandle);
			int selectedChildrenCount = 0;
			IterateChildren(groupRowHandle, rowHandle => {
				if(View.IsRowSelected(rowHandle))
					selectedChildrenCount++;
			});
			return AreAllItemsSelected(selectedChildrenCount, childrenCount);
		}
		void IterateChildren(int groupRowHandle, Action<int> action) {
			int childrenCount = Grid.GetChildRowCount(groupRowHandle);
			for(int childIndex = 0; childIndex < childrenCount; childIndex++)
				action(Grid.GetChildRowHandle(groupRowHandle, childIndex));
		}
		bool? HasOneSelectedChild(int groupRowHandle) {
			if(Grid.GetChildRowCount(groupRowHandle) == 1)
				return true;
			return null;
		}
		#endregion
		#region GroupDescendants
		bool? AreAllDescendantsSelected(int groupRowHandle) {
			if(IsAllItemsUnselected)
				return false;
			if(IsAllItemsSelected)
				return true;
			if(IsOneDataRowSelected)
				return AreAllDescendantsSelected(groupRowHandle, Grid.GetSelectedRowHandles()[0]);
			return AreAllItemsSelected(GetSelectedDescendantsCount(groupRowHandle), GetDescendantsCount(groupRowHandle));
		}
		bool? AreAllDescendantsSelected(int groupRowHandle, int selectedRowHandle) {
			int ancestorRowHandle = Grid.GetParentRowHandle(selectedRowHandle);
			if(ancestorRowHandle == groupRowHandle) {
				return HasOneSelectedChild(groupRowHandle);
			}
			while(true) {
				ancestorRowHandle = Grid.GetParentRowHandle(ancestorRowHandle);
				if(!Grid.IsGroupRowHandle(ancestorRowHandle))
					return false;
				if(ancestorRowHandle == groupRowHandle)
					return AreAllDescendantsSelected(AreAllChildrenSelected(groupRowHandle), true);
			}
		}
		int GetDescendantsCount(int groupRowHandle) {
			return GetDescendantsCount(groupRowHandle, dataRowHandle => true);
		}
		int GetSelectedDescendantsCount(int groupRowHandle) {
			return GetDescendantsCount(groupRowHandle, dataRowHandle => View.IsRowSelected(dataRowHandle));
		}
		int GetDescendantsCount(int groupRowHandle, Func<int, bool> dataRowFilter) {
			int childrenCount = 0;
			IterateDescendants(groupRowHandle, rowHandle => {
				if(!Grid.IsGroupRowHandle(rowHandle) && dataRowFilter(rowHandle))
					childrenCount++;
			});
			return childrenCount;
		}
		void IterateDescendants(int groupRowHandle, Action<int> action) {
			Stack<int> rowHandles = new Stack<int>(new List<int> { groupRowHandle });
			while(rowHandles.Count != 0) {
				int rowHandle = rowHandles.Pop();
				if(Grid.IsGroupRowHandle(rowHandle)) {
					PushChildren(rowHandles, rowHandle);
				}
				action(rowHandle);
			}
		}
		void PushChildren(Stack<int> rowHandles, int groupRowHandle) {
			IterateChildren(groupRowHandle, rowHandle => rowHandles.Push(rowHandle));
		}
		#endregion
		#region OnSelectionChanged
		public override void OnSelectionChanged(SelectionChangedEventArgs e) {
			CheckBoxSelectionChangedLocker.DoLockedActionIfNotLocked(() => {
				UpdateGroupRowsSelection(e);
				base.OnSelectionChanged(e);
				UpdateCheckBoxes(e);
			});
			if(CheckBoxSelectionChangedLocker.IsLocked && IsRowSelectedOrUnselected(e))
				RaiseSelectionChanged(e);
		}
		void UpdateGroupRowsSelection(SelectionChangedEventArgs e) {
			if(Grid.GroupCount == 0 || UpdateGroupRowsLocker.IsLocked)
				return;
			if(IsOneDataRowSelected) {
				UpdateGroupRowsSelection(Grid.GetSelectedRowHandles()[0]);
			}
			else if(IsDataRowSelectedOrUnselected(e)) {
				UpdateGroupRowSelection(Grid.GetParentRowHandle(e.ControllerRow), e.Action == CollectionChangeAction.Add);
			}
			else if(e.Action == CollectionChangeAction.Refresh) {
				UpdateGroupRowsSelection();
			}
		}
		void UpdateGroupRowsSelection(int selectedRowHandle) {
			DoSelectionAction(() => {
				SetGroupRowsSelection(false);
				int groupRowHandle = Grid.GetParentRowHandle(selectedRowHandle);
				bool? isSelected = HasOneSelectedChild(groupRowHandle);
				UpdateGroupRow(groupRowHandle, isSelected);
				UpdateGroupRowSelection(Grid.GetParentRowHandle(groupRowHandle), true);
			});
		}
		void UpdateGroupRowSelection(int groupRowHandle, bool hasSelectedDescendant) {
			if(!Grid.IsGroupRowHandle(groupRowHandle))
				return;
			bool? allChildrenSelected = AreAllChildrenSelected(groupRowHandle);
			bool? allDescendantsSelected = AreAllDescendantsSelected(allChildrenSelected, hasSelectedDescendant);
			UpdateGroupRow(groupRowHandle, allDescendantsSelected);
			UpdateGroupRowSelection(Grid.GetParentRowHandle(groupRowHandle), !allDescendantsSelected.HasValue || allDescendantsSelected.Value);
		}
		bool? AreAllDescendantsSelected(bool? allChildrenSelected, bool hasSelectedDescendant) {
			if(allChildrenSelected.HasValue && allChildrenSelected.Value)
				return true;
			if(!allChildrenSelected.HasValue || hasSelectedDescendant)
				return null;
			return false;
		}
#if DEBUGTEST
		public static int updateGroupRowsSelectionCount = 0;
#endif
		void UpdateGroupRowsSelection() {
#if DEBUGTEST
			updateGroupRowsSelectionCount++;
#endif
			DoSelectionAction(() => {
				if(IsAllItemsUnselected) {
					SetGroupRowsSelection(false);
				}
				else if(IsAllItemsSelected) {
					SetGroupRowsSelection(true);
				}
				else {
					var rootGroups = Grid.DataController.GroupInfo.Where(groupInfo => groupInfo.Level == 0).Select(groupInfo => groupInfo.Handle);
					UpdateGroupRowsSelection(rootGroups);
				}
			});
		}
		void UpdateGroupRowsSelection(IEnumerable<int> groupRowHandles) {
			foreach(int rowHandle in groupRowHandles) {
				CheckRowSelection(rowHandle);
			}
		}
		void SetGroupRowsSelection(bool isSelected) {
			foreach(GroupRowInfo rowInfo in Grid.DataController.GroupInfo) {
				UpdateGroupRow(rowInfo.Handle, isSelected);
			}
		}
		bool? CheckRowSelection(int rowHandle) {
			if(!Grid.IsGroupRowHandle(rowHandle))
				return View.IsRowSelected(rowHandle);
			bool hasSelectedDescendant = false;
			int selectedChildrenCount = 0;
			IterateChildren(rowHandle, childRowHandle => {
				bool? isSelected = CheckRowSelection(childRowHandle);
				if(isSelected.HasValue && isSelected.Value)
					selectedChildrenCount++;
				if(!isSelected.HasValue || isSelected.Value)
					hasSelectedDescendant = true;
			});
			bool? allChildrenSelected = AreAllItemsSelected(selectedChildrenCount, Grid.GetChildRowCount(rowHandle));
			bool? allDescendantsSelected = AreAllDescendantsSelected(allChildrenSelected, hasSelectedDescendant);
			UpdateGroupRow(rowHandle, allDescendantsSelected);
			return allDescendantsSelected;
		}
		void UpdateCheckBoxes(DevExpress.Data.SelectionChangedEventArgs e) {
			View.UpdateCellDataValues(View.CheckBoxSelectorColumn);
		}
		void UpdateGroupRow(int groupRowHandle, bool? isSelected) {
			if(isSelected.HasValue && isSelected.Value)
				SelectRowCore(groupRowHandle);
			else
				UnselectRowCore(groupRowHandle);
			View.UpdateRowDataByRowHandle(groupRowHandle, rowData => {
				GroupRowData groupRowData = rowData as GroupRowData;
				if(groupRowData != null)
					groupRowData.SetAllItemsSelected(isSelected);
			});
		}
		#endregion
		#region MasterDetail
		internal override void SelectOnlyThisMasterDetailRange(int startCommonVisibleIndex, int endCommonVisibleIndex) {
			int startRowHandle = DataControl.GetRowHandleByVisibleIndexCore(startCommonVisibleIndex);
			int endRowHandle = DataControl.GetRowHandleByVisibleIndexCore(endCommonVisibleIndex);
			SelectOnlyThisRange(startRowHandle, endRowHandle);
		}
		#endregion
	}
	class SelectionAnchor {
		public readonly GridViewBase View;
		public readonly int RowHandle;
		public SelectionAnchor(GridViewBase view, int rowHandle) {
			View = view;
			RowHandle = rowHandle;
		}
	}
	class TableViewSelectionStrategyNone : SelectionStrategyNone {
		#region Properties
		protected SelectionActionBase SelectionAction {
			get { return RootView.GlobalSelectionAction; }
			set { RootView.SetSelectionAction(value); }
		}
		protected GridViewBase GridView { get { return (GridViewBase)view; } }
		protected GridViewBase RootView { get { return (GridViewBase)view.RootView; } }
		GridControl Grid { get { return (GridControl)view.DataControl; } }
		#endregion
		public TableViewSelectionStrategyNone(GridViewBase view) : base(view) { }
		#region Methods
		public override void OnAssignedToGrid() {
			base.OnAssignedToGrid();
			GridView.SetSelectionAnchor();
		}
		protected virtual bool CanExecuteSelectionActionOnFocusedRowHandleChanged() {
			return SelectionAction != null && SelectionAction.CanFocusChangeDeleteAction;
		}
		public override void OnFocusedRowHandleChanged(int oldRowHandle) {
			if(CanExecuteSelectionActionOnFocusedRowHandleChanged()) {
				ExecuteSelectionAction();
			}
			else {
				GridView.SetSelectionAnchor();
			}
		}
		protected virtual void ExecuteSelectionAction() {
			GridView.ExecuteSelectionAction();
		}
		public override void CopyMasterDetailToClipboard() {
			Grid.CopyAllSelectedItemsToClipboard();
		}
		#endregion
		#region KeyboardSelection
		int oldFocusedRowHandle = GridControl.InvalidRowHandle;
		public override void OnBeforeProcessKeyDown(KeyEventArgs e) {
			oldFocusedRowHandle = view.FocusedRowHandle;
			if(IsNavigationKey(e)) {
				if(ModifierKeysHelper.IsShiftPressed(ModifierKeysHelper.GetKeyboardModifiers(e))) {
					SelectionAction = new AddRowsToSelectionAction(GridView);
				}
				else if(!ModifierKeysHelper.IsCtrlPressed(ModifierKeysHelper.GetKeyboardModifiers(e))) {
					SelectionAction = new OnlyThisSelectionAction(GridView);
				}
			}
		}
		public override void OnAfterProcessKeyDown(KeyEventArgs e) {
			if((e.Key != Key.Next) && (e.Key != Key.Prior)) {
				OnNavigationComplete(e.Key == Key.Tab);
			}
		}
		protected static bool IsNavigationKey(KeyEventArgs e) {
			return (e.Key == Key.Up) || (e.Key == Key.Down) ||
				   (e.Key == Key.PageUp) || (e.Key == Key.PageDown) ||
				   (e.Key == Key.Right) || (e.Key == Key.Left) || (e.Key == Key.Tab) ||
				   (e.Key == Key.Home) || (e.Key == Key.End);
		}
		public override void OnNavigationComplete(bool IsTabPressed) {
			if(oldFocusedRowHandle == view.FocusedRowHandle)
				SelectionAction = null;
		}
		public override void OnNavigationCanceled() {
			SelectionAction = null;
		}
		#endregion
	}
}
