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
using System.Windows.Input;
using System.Collections;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data;
using System.Collections.Specialized;
using System.ComponentModel;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Data;
using System;
namespace DevExpress.Xpf.Grid.Native {
	public abstract class SelectionStrategyBase {
		#region Properties
#if DEBUGTEST
		internal static bool TestIsKeyboardFocusWithin = false;
#endif
		protected DataViewBase view;
		object oldSelection;
		protected bool IsDataControlSync { get { return DataControl != null && DataControl.isSync; } }
		protected virtual bool IsServerMode { get { return DataControl.IsServerMode; } }
		protected bool CanGetSelectedItems { get { return !view.IsDesignTime && IsDataControlSync && !IsServerMode; } }
		protected bool CanUpdateSelectedItems { get { return CanGetSelectedItems && SelectedItems != null; } }
		protected bool CanSelectSelectedItem { get { return IsDataControlSync && (DataControl.ItemsSource != null || IsOriginationDataControl); } }
		internal bool IsSelectionLocked { get { return SelectionLocker.IsLocked; } }
		internal virtual bool IsFocusedRowHandleLocked { get { return false; } }
		protected bool UpdateSelectedItemWasLocked { get; private set; }
		protected DataControlBase DataControl { get { return view.DataControl; } }
		protected DataProviderBase DataProviderBase { get { return DataControl.DataProviderBase; } }
		protected IList SelectedItems { get { return DataControl.SelectedItems; } }
		protected bool IsShiftPressed { get { return ModifierKeysHelper.IsShiftPressed(Keyboard.Modifiers); } }
		protected bool IsControlPressed { get { return ModifierKeysHelper.IsCtrlPressed(Keyboard.Modifiers); } }
		DataControlBase OriginationDataControl { get { return DataControl.GetOriginationDataControl(); } }
		IList OriginationSelectedItems { get { return OriginationDataControl.SelectedItems; } }
		bool IsDetailDataControl { get { return !DataControl.IsOriginationDataControl(); } }
		bool IsOriginationDataControl { get { return DataControl.IsOriginationDataControlCore(); } }
		SelectionStrategyBase OriginationSelectionStrategy { get { return OriginationDataControl.DataView.SelectionStrategy; } }
		protected readonly Locker SelectionLocker = new Locker();
		protected readonly Locker SelectedItemChangedLocker = new Locker();
		internal readonly Locker SelectionChangedLocker = new Locker();
		#endregion
		protected SelectionStrategyBase(DataViewBase view) {
			this.view = view;
		}
		#region Methods
		internal static bool? AreAllItemsSelected(int selectedRowCount, int rowCount) {
			if(selectedRowCount == 0)
				return false;
			if(selectedRowCount == rowCount)
				return true;
			return null;
		}
		public virtual void SelectRow(int rowHandle) { }
		public void SelectItem(object item) {
			if(!IsOriginationDataControl) {
				SelectRow(DataControl.DataProviderBase.FindRowByRowValue(item));
			}
			else {
				SelectItemInOriginationGrid(item);
			}
		}
		public virtual void UnselectRow(int rowHandle) { }
		public void UnselectItem(object item) {
			if(!IsOriginationDataControl) {
				UnselectRow(DataControl.DataProviderBase.FindRowByRowValue(item));
			}
			else {
				UnselectItemInOriginationGrid(item);
			}
		}
		public virtual void SelectCell(int rowHandle, ColumnBase column) { }
		public virtual void UnselectCell(int rowHandle, ColumnBase column) { }
		public virtual void SelectAll() { }
		public virtual void SelectAllMasterDetail(bool? allItemsSelected) { }
		public virtual void BeginSelection() { }
		public virtual void EndSelection() { }
		public virtual void ClearSelection() { }
		public virtual int[] GetSelectedRows() { return new int[] { }; }
		public virtual CellBase[] GetSelectedCells() { return new CellBase[] { }; }
		public virtual bool IsRowSelected(int rowHandle) { return false; }
		public virtual bool IsCellSelected(int rowHandle, ColumnBase column) { return false; }
		public virtual void SelectRowForce() { }
		public virtual void SetCellsSelection(int startRowHandle, ColumnBase startColumn, int endRowHandle, ColumnBase endColumn, bool setSelected) { }
		public virtual void ToggleRowsSelection() { }
		public virtual bool? GetAllItemsSelected() { return false; }
		public virtual bool? GetAllItemsSelected(int rowHandle) { return false; }
		internal virtual void SelectRowRecursively(int groupRowHandle) { }
		internal virtual void UnselectRowRecursively(int groupRowHandle) { }
		public virtual void OnItemChanged(ListChangedEventArgs e) { }
		public virtual void OnFocusedRowDataChanged() { }
		public virtual void OnFocusedRowHandleChanged(int oldRowHandle) { }
		public virtual void OnFocusedColumnChanged() { }
		public virtual void OnAssignedToGrid() {
			if(IsDataControlSync)
				OnAssignedToGridCore();
		}
		protected virtual void OnAssignedToGridCore() { }
		void ClearCurrentSelection() {
			oldSelection = null;
		}
		public virtual void OnDataSourceReset() {
			if(IsDataControlSync)
				OnDataSourceResetCore();
		}
		protected virtual void OnDataSourceResetCore() {
			ClearCurrentSelection();
		}
		public virtual void OnDataControlInitialized() {
			if(!DataControl.HasValue(DataControlBase.SelectedItemsProperty))
				ForceCreateSelectedItems();
		}
		public virtual void OnSelectionChanged(DevExpress.Data.SelectionChangedEventArgs e) {
			UpdateBorderForFocusedElement();
			if(oldSelection != null)
				UpdateVisualState(oldSelection);
			UpdateCachedSelection();
			UpdateVisualState(oldSelection);
			ProcessSelectionChanged(e);
			OriginationSelectionStrategy.SelectionChangedLocker.DoLockedActionIfNotLocked(() => {
				RaiseSelectionChanged(e);
			});
		}
		public void RaiseSelectionChanged(DevExpress.Data.SelectionChangedEventArgs e) {
			DataControl.RaiseSelectionChanged(e);
			view.RaiseSelectionChanged(e);
		}
		public void UpdateCachedSelection() {
			oldSelection = GetSelection();
		}
		internal virtual void OnBeforeMouseLeftButtonDown(MouseButtonEventArgs e) {
			OnBeforeMouseLeftButtonDown(e.OriginalSource as DependencyObject);
		}
		public virtual void OnBeforeMouseLeftButtonDown(DependencyObject originalSource) { }
		internal virtual void OnAfterMouseLeftButtonDown(IDataViewHitInfo hitInfo, StylusDevice stylus, int clickCount) {
			OnAfterMouseLeftButtonDown(hitInfo);
		}
		public virtual void OnAfterMouseLeftButtonDown(IDataViewHitInfo hitInfo) { }
		public virtual void OnMouseLeftButtonUp(MouseButtonEventArgs e) { }
		public virtual bool ShouldInvertSelectionOnPreviewKeyDown(KeyEventArgs e) {
			return false;
		}
		public virtual void OnInvertSelection() { }
		public virtual void OnDoubleClick(MouseButtonEventArgs e) { }
		public virtual void CreateMouseSelectionActions(int rowHandle, bool isIndicator) { }
		public void UpdateBorderForFocusedElement() {
			if(DataControl.GetRootDataControl().IsKeyboardFocusWithin
#if DEBUGTEST
				|| TestIsKeyboardFocusWithin
#endif
				) {
				if(UpdateBorderForFocusedElementCore())
					return;
			}
			view.ClearFocusedRectangle();
		}
		public virtual bool UpdateBorderForFocusedElementCore() { return false; }
		public virtual void CopyToClipboard() { }
		public virtual void CopyMasterDetailToClipboard() {
			CopyToClipboard();
		}
		public virtual void UpdateSelectionRect(int rowHandle, ColumnBase column) { }
		public abstract SelectionState GetRowSelectionState(int rowHandle);
		public SelectionState GetCellSelectionState(int rowHandle, bool isFocused, bool isSelected) {
			if(!CanSelectCell(rowHandle))
				return SelectionState.None;
			return GetCellSelectionStateCore(rowHandle, isFocused, isSelected);
		}
		protected virtual bool CanSelectCell(int rowHandle) {
			return view.IsFocusedView && view.CanSelectCellInRow(rowHandle);
		}
		protected virtual SelectionState GetCellSelectionStateCore(int rowHandle, bool isFocused, bool isSelected) {
			return isFocused ? SelectionState.Focused : SelectionState.None;
		}
		protected virtual void UpdateVisualState(object oldSelection) {
		}
		protected virtual object GetSelection() {
			return null;
		}
		protected void DoSelectionAction(Action action) {
			BeginSelection();
			try {
				action();
			}
			finally {
				EndSelection();
			}
		}
		protected void SelectRangeCore(int startRowHandle, int endRowHandle, Action<int> selectRowAction) {
			if(startRowHandle == DataControlBase.InvalidRowHandle || endRowHandle == DataControlBase.InvalidRowHandle) return;
			if(startRowHandle == endRowHandle) {
				selectRowAction(startRowHandle);
				return;
			}
			int startIndex = DataProviderBase.GetRowVisibleIndexByHandle(startRowHandle),
				endIndex = DataProviderBase.GetRowVisibleIndexByHandle(endRowHandle);
			if(startIndex < 0 || endIndex < 0)
				return;
			if(startIndex > endIndex) {
				int a = endIndex;
				endIndex = startIndex;
				startIndex = a;
			}
			DoSelectionAction(() => {
				for(int n = startIndex; n <= endIndex; n++) {
					int rowHandle = DataControl.GetRowHandleByVisibleIndexCore(n);
#if DEBUGTEST
					if(rowHandle == DataControlBase.InvalidRowHandle)
						System.Diagnostics.Debug.Assert(false, "Visible index < 0");
#endif
					selectRowAction(rowHandle);
				}
			});
		}
		public virtual void SelectOnlyThisRange(int startRowHandle, int endRowHandle) {
			DoSelectionAction(() => {
				ClearMasterDetailSelection();
				SelectRange(startRowHandle, endRowHandle);
			});
		}
		public virtual void SelectRange(int startRowHandle, int endRowHandle) {
			SelectRangeCore(startRowHandle, endRowHandle, rowHandle => SelectRow(rowHandle));
		}
		#endregion
		#region SelectedItems
		protected bool IsSameSelection(IEnumerable<object> previousSelection, IEnumerable<object> currentSelection) {
			return previousSelection != null && previousSelection.SequenceEqual(currentSelection);
		}
		void ProcessSelectionChanged(DevExpress.Data.SelectionChangedEventArgs e) {
			if(!CanUpdateSelectedItems || view.DataProviderBase.IsGroupRowHandle(e.ControllerRow))
				return;
			SelectionLocker.DoLockedActionIfNotLocked(() => {
				switch(e.Action) {
					case CollectionChangeAction.Add:
						AddToSelectedItems(DataControl.GetRow(e.ControllerRow));
						break;
					case CollectionChangeAction.Remove:
						OnRemoveItem(e.ControllerRow);
						break;
					default:
						UpdateSelectedItemsCore();
						break;
				}
				UpdateSelectedItem();
			});
		}
		void OnRemoveItem(int rowHandle) {
			object item = DataControl.GetRow(rowHandle);
			int index = SelectedItems.IndexOf(item);
			if(index >= 0) {
				if(!IsRowSelected(rowHandle))
					RemoveFromSelectedItems(item);
			}
			else {
				OnRemoveItemFromItemsSource();
			}
		}
		void OnRemoveItemFromItemsSource() {
			var removedItem = SelectedItems.Cast<object>().Except(GetSelectedItems()).FirstOrDefault();
			if(removedItem != null)
				RemoveFromSelectedItems(removedItem);
		}
		public void UpdateSelectedItems() {
			if(CanUpdateSelectedItems)
				UpdateSelectedItemsCore();
		}
		protected void UpdateSelectedItemsCore() {
			var selectedItemsCore = GetSelectedItems();
			if(IsSameSelection(SelectedItems.Cast<object>(), selectedItemsCore))
				return;
			BeginUpdateSelectedItems();
			try {
				ClearSelectedItems();
				foreach(object item in selectedItemsCore) {
					AddToSelectedItems(item);
				}
			}
			finally {
				OriginationSelectionStrategy.SelectionLocker.DoLockedAction(() => {
					EndUpdateSelectedItems();
				});
			}
			UpdateSelectedItem();
		}
		protected IList<object> GetSelectedItems() {
			List<object> selectedItems = new List<object>();
			if(!CanGetSelectedItems)
				return selectedItems;
			foreach(int itemHandle in GetSelectedRows()) {
				if(ShouldAddToSelectedItems(itemHandle))
					selectedItems.Add(DataControl.GetRow(itemHandle));
			}
			return selectedItems;
		}
		protected virtual bool ShouldAddToSelectedItems(int rowHandle) {
			return rowHandle >= 0;
		}
		public virtual void OnSelectedItemsChanged(NotifyCollectionChangedEventArgs e) { }
		public void SelectItems(IList items) {
			if(!IsOriginationDataControl) {
				SelectItemsCore(items);
			}
			else {
				SelectItemsInOriginationGrid(items);
			}
		}
		void SelectItemsCore(IList items) {
			BeginSelection();
			try {
				ClearSelection();
				if(items != null) {
					foreach(object item in items)
						SelectItem(item);
				}
			}
			finally {
				EndSelection();
			}
		}
		public virtual void ProcessSelectedItemsChanged() { }
		void ForceCreateSelectedItems() {
			SelectionLocker.DoLockedAction(() => {
				DataControl.SelectedItems = new ObservableCollectionCore<object>();
				UpdateSelectedItems();
			});
		}
		protected void AddToSelectedItems(object item) {
			if(IsDetailDataControl)
				AddToOriginationSelectedItems(item);
			SelectedItems.Add(item);
		}
		protected void ReplaceFirstSelectedItem(object newItem) {
			if(IsDetailDataControl)
				ReplaceFirstOriginationSelectedItem(newItem);
			SelectedItems[0] = newItem;
		}
		protected void RemoveFromSelectedItems(object item) {
			if(IsDetailDataControl)
				RemoveFromOriginationSelectedItems(item);
			SelectedItems.Remove(item);
		}
		protected void ClearSelectedItems() {
			if(IsDetailDataControl)
				ClearOriginationSelectedItems();
			SelectedItems.Clear();
		}
		void BeginUpdateSelectedItems() {
			if(SelectedItems is ILockable)
				((ILockable)SelectedItems).BeginUpdate();
			if(IsDetailDataControl && OriginationSelectedItems is ILockable)
				((ILockable)OriginationSelectedItems).BeginUpdate();
		}
		void EndUpdateSelectedItems() {
			if(SelectedItems is ILockable)
				((ILockable)SelectedItems).EndUpdate();
			if(IsDetailDataControl && OriginationSelectedItems is ILockable)
				((ILockable)OriginationSelectedItems).EndUpdate();
		}
		#endregion
		#region SelectedItem
		public void OnSelectedItemChanged(object oldValue) {
			ProcessSelectedItemChanged();
			if(IsDetailDataControl)
				UpdateSelectedItemInOriginationGrid();
			if(!IsOriginationDataControl)
				DataControl.RaiseSelectedItemChanged(oldValue);
		}
		protected virtual void ProcessSelectedItemChanged() {
			if(!IsSelectionLocked) {
				if(CanSelectSelectedItem) {
					UpdateSelectedItemWasLocked = false;
					OnSelectedItemChangedCore();
				}
				else if(DataControl.SelectedItem != null)
					UpdateSelectedItemWasLocked = true;
			}
		}
		protected internal virtual void UpdateSelectedItem() {
			if(!UpdateSelectedItemWasLocked && !view.IsDesignTime) {
				object selectedItem = DataControl.HasSelectedItems ? SelectedItems[0] : null;
				if(!ReferenceEquals(DataControl.SelectedItem, selectedItem))
					SetSelectedItem(selectedItem);
			}
		}
		void SetSelectedItem(object selectedItem) {
			SetSelectedItem(DataControl, selectedItem);
		}
		static void SetSelectedItem(DataControlBase dataControl, object selectedItem) {
			dataControl.SetCurrentValue(DataControlBase.SelectedItemProperty, selectedItem);
		}
		protected bool RestoreSelectedItem() {
			bool canRestore = UpdateSelectedItemWasLocked && CanSelectSelectedItem;
			if(canRestore) {
				UpdateSelectedItemWasLocked = false;
				OnSelectedItemChangedCore();
			}
			return canRestore;
		}
		protected virtual void OnSelectedItemChangedCore() {
			SelectedItemChangedLocker.DoLockedActionIfNotLocked(() => {
				SelectItems(new List<object> { DataControl.SelectedItem });
			});
		}
		#endregion
		#region MasterDetail
		void AddToOriginationSelectedItems(object item) {
			OriginationSelectionStrategy.SelectionLocker.DoLockedActionIfNotLocked(() => {
				if(OriginationSelectedItems != null)
					OriginationSelectedItems.Add(item);
			});
		}
		void RemoveFromOriginationSelectedItems(object item) {
			OriginationSelectionStrategy.SelectionLocker.DoLockedActionIfNotLocked(() => {
				if(OriginationSelectedItems != null)
					OriginationSelectedItems.Remove(item);
			});
		}
		void ClearOriginationSelectedItems() {
			if(OriginationSelectedItems == null)
				return;
			OriginationSelectionStrategy.SelectionLocker.DoLockedActionIfNotLocked(() => {
				foreach(var item in SelectedItems) {
					OriginationSelectedItems.Remove(item);
				}
			});
		}
		void ReplaceFirstOriginationSelectedItem(object newItem) {
			OriginationSelectionStrategy.SelectionLocker.DoLockedActionIfNotLocked(() => {
				if(OriginationSelectedItems != null && OriginationSelectedItems.Count > 0)
					OriginationSelectedItems[0] = newItem;
			});
		}
		internal virtual void SelectAllMasterDetail() {
			DoMasterDetailSelectionAction(() => {
				bool? allItemsSelected = GetAllItemsSelectedMasterDetail();
				UpdateAllVisibleDetailDataControls(detail => detail.DataView.SelectionStrategy.SelectAllMasterDetail(allItemsSelected));
			});
		}
		void UpdateAllVisibleDetailDataControls(Action<DataControlBase> action) {
			DataControl.UpdateAllDetailDataControls(detail => {
				List<KeyValuePair<DataViewBase, int>> visibleIndexChain = new List<KeyValuePair<DataViewBase, int>>();
				detail.DataControlParent.CollectViewVisibleIndexChain(visibleIndexChain);
				bool isVisible = visibleIndexChain.Where(index => index.Value < 0).Count() == 0;
				if(isVisible)
					action(detail);
			});
		}
		protected internal void DoMasterDetailSelectionAction(Action action) {
			BeginMasterDetailSelection();
			try {
				action();
			}
			finally {
				EndMasterDetailSelection();
			}
		}
		void BeginMasterDetailSelection() {
			DataControl.UpdateAllOriginationDataControls(detail => {
				if(detail.DataView.GetActualSelectionMode() != MultiSelectMode.MultipleRow)
					detail.DataView.SelectionStrategy.BeginUpdateSelectedItems();
			});
			DataControl.UpdateAllDetailDataControls(detail => detail.BeginSelection());
		}
		void EndMasterDetailSelection() {
			DataControl.UpdateAllDetailDataControls(detail => detail.EndSelection());
			DataControl.UpdateAllOriginationDataControls(detail => {
				if(detail.DataView.GetActualSelectionMode() == MultiSelectMode.MultipleRow)
					return;
				var selectionStrategy = detail.DataView.SelectionStrategy;
				selectionStrategy.SelectionLocker.DoLockedAction(() => {
					selectionStrategy.EndUpdateSelectedItems();
				});
			});
		}
		protected void ClearMasterDetailSelection() {
			DataControl.ClearMasterDetailSelection();
		}
		internal virtual void SelectOnlyThisMasterDetailRange(int startCommonVisibleIndex, int endCommonVisibleIndex) {
			DoMasterDetailSelectionAction(() => {
				ClearMasterDetailSelection();
				SelectMasterDetailRange(startCommonVisibleIndex, endCommonVisibleIndex);
			});
		}
		void SelectMasterDetailRange(int startCommonVisibleIndex, int endCommonVisibleIndex) {
			SelectMasterDetailRangeCore(startCommonVisibleIndex, endCommonVisibleIndex);
		}
		void SelectMasterDetailRangeCore(int startCommonVisibleIndex, int endCommonVisibleIndex) {
			if(startCommonVisibleIndex == endCommonVisibleIndex) {
				SelectItemByCommonVisibleIndex(startCommonVisibleIndex);
				return;
			}
			if(startCommonVisibleIndex < 0 || endCommonVisibleIndex < 0)
				return;
			int minIndex = Math.Min(startCommonVisibleIndex, endCommonVisibleIndex);
			int maxIndex = Math.Max(startCommonVisibleIndex, endCommonVisibleIndex);
			for(int commonVisibleIndex = minIndex; commonVisibleIndex <= maxIndex; commonVisibleIndex++) {
				SelectItemByCommonVisibleIndex(commonVisibleIndex);
			}
		}
		void SelectItemByCommonVisibleIndex(int commonVisibleIndex) {
			var pair = DataControl.FindViewAndVisibleIndexByCommonVisibleIndex(commonVisibleIndex);
			var dataControl = pair.Key.DataControl;
			var rowHandle = dataControl.GetRowHandleByVisibleIndexCore(pair.Value);
			if(dataControl.SelectionMode != MultiSelectMode.MultipleRow)
				dataControl.SelectItem(rowHandle);
		}
		public bool? GetAllItemsSelectedMasterDetail() {
			int selectedRowCount = GetGlobalSelectedRowCount();
			int totalRowCount = GetGlobalVisibleRowCount();
			return AreAllItemsSelected(selectedRowCount, totalRowCount); 
		}
		int GetGlobalVisibleRowCount() {
			int visibleRowCount = 0;
			DataControl.UpdateAllDetailDataControls(dataControl => {
				visibleRowCount += dataControl.VisibleRowCount;
				if(dataControl.DataView.ShouldDisplayBottomRow)
					visibleRowCount--;
			});
			return visibleRowCount;
		}
		internal int GetGlobalSelectedRowCount() {
			int selectedRowCount = 0;
			DataControl.UpdateAllDetailDataControls(dataControl => selectedRowCount += dataControl.GetSelectedRowHandles().Length);
			return selectedRowCount;
		}
		void SelectItemsInOriginationGrid(IList selectedItems) {
			SelectionChangedLocker.DoLockedAction(() => {
				DoMasterDetailSelectionAction(() => {
					ClearDetailsSelection();
					SelectItemsInDetails(selectedItems);
				});
			});
			DataControl.RaiseSelectionChanged(new SelectionChangedEventArgs(CollectionChangeAction.Refresh, DataControlBase.InvalidRowHandle));
		}
		void ClearDetailsSelection() {
			DataControl.GetRootDataControl().UpdateAllDetailDataControls(ClearDetailDataControlSelection, ClearDetailDataControlSelection);
		}
		void ClearDetailDataControlSelection(DataControlBase dataControl) {
			if(dataControl.GetOriginationDataControl() == DataControl)
				dataControl.UnselectAll();
		}
		void SelectItemsInDetails(IList selectedItems) {
			if(selectedItems == null)
				return;
			foreach(var item in selectedItems) {
				SelectItemInOriginationGrid(item);
			}
		}
		void SelectItemInOriginationGrid(object item) {
			DataControlBase dataControl = DataControl.DataControlOwner.FindDetailDataControlByRow(item);
			if(dataControl != null)
				dataControl.DataView.SelectionStrategy.SelectItem(item);
		}
		void UnselectItemInOriginationGrid(object item) {
			DataControlBase dataControl = DataControl.DataControlOwner.FindDetailDataControlByRow(item);
			if(dataControl != null)
				dataControl.DataView.SelectionStrategy.UnselectItem(item);
		}
		void UpdateSelectedItemInOriginationGrid() {
			OriginationSelectionStrategy.SelectedItemChangedLocker.DoLockedActionIfNotLocked(() => {
				if(!view.IsDesignTime) {
					object selectedItem = OriginationDataControl.HasSelectedItems ? OriginationSelectedItems[0] : null;
					if(!ReferenceEquals(OriginationDataControl.SelectedItem, selectedItem))
						SetSelectedItem(OriginationDataControl, selectedItem);
				}
			});
		}
		#endregion
		#region Keyboard
		public virtual void OnBeforeProcessKeyDown(System.Windows.Input.KeyEventArgs e) { }
		public virtual void OnAfterProcessKeyDown(System.Windows.Input.KeyEventArgs e) { }
		public virtual void OnNavigationComplete(bool isTabPressed) { }
		public virtual void OnNavigationCanceled() { }
		#endregion
	}
	public abstract class MultiSelectionStrategyBase : SelectionStrategyBase {
		public MultiSelectionStrategyBase(DataViewBase view) : base(view) { }
		public override void OnDataControlInitialized() {
			if(DataControl.SelectedItems != null)
				ProcessSelectedItemsChanged();
			else if(DataControl.SelectedItem != null)
				ProcessSelectedItemChanged();
			else
				OnDataSourceReset();
			base.OnDataControlInitialized();
		}
		public override void ProcessSelectedItemsChanged() {
			OnSelectedItemsChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		public override void OnSelectedItemsChanged(NotifyCollectionChangedEventArgs e) {
			if(!CanGetSelectedItems)
				return;
			SelectionLocker.DoLockedActionIfNotLocked(() => {
				if(e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace) {
					foreach(object item in e.OldItems)
						UnselectItem(item);
				}
				if(e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace) {
					foreach(object item in e.NewItems)
						SelectItem(item);
				}
				if(e.Action == NotifyCollectionChangedAction.Reset) {
					SelectItems(SelectedItems);
				}
				UpdateSelectedItem();
			});
		}
		public sealed override bool ShouldInvertSelectionOnPreviewKeyDown(KeyEventArgs e) {
			if(e.Key == Key.Space && view.ActiveEditor == null) {
				if(IsControlPressed)
					return true;
				if(!view.IsKeyboardFocusInSearchPanel())
					return ShouldInvertSelectionOnSpace();
			}
			return false;
		}
		protected abstract bool ShouldInvertSelectionOnSpace();
		public override void OnItemChanged(ListChangedEventArgs e) {
			if(e.ListChangedType != ListChangedType.ItemChanged || !CanUpdateSelectedItems)
				return;
			int rowHandle = e.NewIndex;
			if(IsRowSelected(rowHandle) && !SelectedItems.Contains(DataControl.GetRow(rowHandle)))
				UnselectRow(rowHandle);
		}
	}
	public abstract class SelectionStrategyRowBase : MultiSelectionStrategyBase {
		protected bool HasValidationError { get { return view.HasValidationError && view.IsEditing; } }
		public bool IsMultipleMode { get { return view.GetActualSelectionMode() == MultiSelectMode.MultipleRow; } }
		public bool IsExtendedMode { get { return !IsMultipleMode; } }
		protected int DataRowCount { get { return DataControl.DataProviderBase.DataRowCount; } }
		protected virtual int TotalRowCount {
			get {
				int count = DataControl.VisibleRowCount;
				if(DataControl.DataView.ShouldDisplayBottomRow)
					count--;
				return count;
			}
		}
		protected int SelectedRowCount { get { return DataControl.DataProviderBase.Selection.Count; } }
		public SelectionStrategyRowBase(DataViewBase view) : base(view) { }
		public abstract void SetFocusedRowSelected();
		protected override void OnDataSourceResetCore() {
			base.OnDataSourceResetCore();
			bool isLookUpMode = view.SearchPanelColumnProvider != null && view.SearchPanelColumnProvider.IsSearchLookUpMode;
			if(!isLookUpMode) {
				if(RestoreSelectedItem())
					return;
				if(DataControl.AllowUpdateSelectedItems && view.IsRootView)
					SetFocusedRowSelected();
				else
					ProcessSelectedItemsChanged();
			}
			else {
				OnDataSourceResetInLookUpMode();
			}
		}
		protected virtual void OnDataSourceResetInLookUpMode() {
			ProcessSelectedItemsChanged();
			UpdateSelectedItems();
		}
		protected override object GetSelection() {
			return GetSelectedRows();
		}
		protected override void UpdateVisualState(object oldSelection) {
			foreach(int rowHandle in (int[])oldSelection) {
				view.UpdateRowDataByRowHandle(rowHandle, rowData => {
					rowData.UpdateIsSelected(view.IsRowSelected(rowHandle));
				});
			}
		}
		void SelectAllCore() {
			view.DataProviderBase.Selection.SelectAll();
		}
		public override void SelectAll() {
			SelectAllCore();
		}
		public override void SelectAllMasterDetail(bool? allItemsSelected) {
			if(IsMultipleMode)
				ToggleRowsSelection(allItemsSelected);
			else
				SelectAllCore();
		}
		internal void SelectRowCore(int rowHandle) {
			view.DataProviderBase.Selection.SetSelected(rowHandle, true);
		}
		internal void UnselectRowCore(int rowHandle) {
			view.DataProviderBase.Selection.SetSelected(rowHandle, false);
		}
		public override void ToggleRowsSelection() {
			ToggleRowsSelection(GetAllItemsSelected());
		}
		public void ToggleRowsSelection(bool? allItemsSelected) {
			if(allItemsSelected.HasValue && allItemsSelected.Value)
				ClearSelection();
			else
				SelectAll();
		}
		protected virtual void SelectAllRows() {
			DoSelectionAction(() => {
				SelectDataRows();
			});
		}
		protected void SelectDataRows() {
			for(int rowHandle = 0; rowHandle < DataRowCount; rowHandle++)
				SelectRowCore(rowHandle);
		}
		public override bool? GetAllItemsSelected() {
			return AreAllItemsSelected(SelectedRowCount, TotalRowCount);
		}
		Locker DoubleClickLocker = new Locker();
		public override void OnDoubleClick(MouseButtonEventArgs e) {
			if(IsMultipleMode && e.LeftButton == MouseButtonState.Pressed) {
				view.EditorSetInactiveAfterClick = false;
				DoubleClickLocker.DoLockedAction(() => {
					DataControl.FindTargetView(e.OriginalSource).InplaceEditorOwner.ProcessMouseLeftButtonDown(e);
				});
			}
		}
		bool ShouldOpenEditor(StylusDevice stylus, int clickCount) {
			if(!IsMultipleMode)
				return false;
			if(clickCount == 2 || DoubleClickLocker.IsLocked)
				return true;
			return stylus != null && view.CurrentCellEditor.Return(edit => edit.IsEditorVisible, () => false);
		}
		internal override void OnAfterMouseLeftButtonDown(IDataViewHitInfo hitInfo, StylusDevice stylus, int clickCount) {
			if(ShouldOpenEditor(stylus, clickCount))
				return;
			base.OnAfterMouseLeftButtonDown(hitInfo, stylus, clickCount);
		}
		protected override bool ShouldInvertSelectionOnSpace() {
			if(view.RootView.NavigationStyle != GridViewNavigationStyle.Row)
				return false;
			return IsMultipleMode || !IsRowSelected(view.FocusedRowHandle);
		}
	}
	class SelectionAnchorCell : CellBase {
		public DataViewBase View { get; private set; }
		public SelectionAnchorCell(DataViewBase view, int rowHandle, ColumnBase column) : base(rowHandle, column) {
			View = view;
		}
	}
}
