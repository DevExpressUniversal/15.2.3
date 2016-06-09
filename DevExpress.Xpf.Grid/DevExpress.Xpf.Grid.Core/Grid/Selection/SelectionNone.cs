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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.Xpf.Grid.Native {
	class SelectionStrategyNone : SelectionStrategyBase {
		internal override bool IsFocusedRowHandleLocked { get { return UpdateSelectedItemWasLocked; } }
		protected override bool IsServerMode { get { return DataControl.DataProviderBase.IsAsyncServerMode; } }
		public SelectionStrategyNone(DataViewBase view) : base(view) { }
		public override bool IsRowSelected(int rowHandle) {
			if(!view.IsFocusedView) return false;
			return (rowHandle == view.FocusedRowHandle);
		}
		public override SelectionState GetRowSelectionState(int rowHandle) {
			if(!view.IsFocusedView) return SelectionState.None;
			return rowHandle == view.FocusedRowHandle ? SelectionState.Focused : SelectionState.None;
		}
		public override int[] GetSelectedRows() {
			if(view.IsInvalidFocusedRowHandle || !view.IsFocusedView)
				return new int[] { };
			return new int[] { view.FocusedRowHandle };
		}
		public override void OnFocusedRowDataChanged() {
			SetFocusedRowSelected();
		}
		protected override void OnDataSourceResetCore() {
			base.OnDataSourceResetCore();
			if(!RestoreSelectedItem())
				UpdateSelectedItems();
		}
		protected override void OnAssignedToGridCore() {
			base.OnAssignedToGridCore();
			UpdateSelectedItems();
		}
		public override void OnDataControlInitialized() {
			if(DataControl.SelectedItem != null)
				ProcessSelectedItemChanged();
			else
				OnDataSourceReset();
			base.OnDataControlInitialized();
		}
		protected virtual bool ShowFocusedRectangle { get { return view.ShowFocusedRectangle; } }
		public override bool UpdateBorderForFocusedElementCore() {
			if(!ShowFocusedRectangle) return false;
			if(DataControl.IsGroupRowHandleCore(view.FocusedRowHandle)) {
				view.SetFocusedRectangleOnGroupRow();
				return true;
			}
			if(view.IsAdditionalRowFocused) {
				view.SetFocusedRectangleOnCell();
				return true;
			}
			if(view.NavigationStyle == GridViewNavigationStyle.Cell)
				view.SetFocusedRectangleOnCell();
			else
				view.SetFocusedRectangleOnRow();
			return true;
		}
		public override void CopyToClipboard() {
			if(!view.IsInvalidFocusedRowHandle)
				DataControl.CopyCurrentItemToClipboard();
		}
		protected override bool ShouldAddToSelectedItems(int rowHandle) {
			return base.ShouldAddToSelectedItems(rowHandle) || (view.IsNewItemRowHandle(rowHandle) && view.ViewBehavior.IsNewItemRowEditing);
		}
		protected override void OnSelectedItemChangedCore() {
			if(!IsSelectionLocked && !ReferenceEquals(DataControl.CurrentItem, DataControl.SelectedItem)) {
				SelectedItemChangedLocker.DoLockedActionIfNotLocked(() => {
					DataControl.SetCurrentItemCore(DataControl.SelectedItem);
				});
			}
		}
		public override void ProcessSelectedItemsChanged() {
			UpdateSelectedItems();
		}
		public void SetFocusedRowSelected() {
			if(!CanUpdateSelectedItems || !DataControl.AllowUpdateCurrentItem)
				return;
			SelectionLocker.DoLockedAction(() => {
				IList<object> selectedItemsCore = GetSelectedItems();
				if(!IsSameSelection(SelectedItems.Cast<object>(), selectedItemsCore))
					SetFocusedRowSelected(selectedItemsCore);
				UpdateSelectedItem();
			});
		}
		void SetFocusedRowSelected(IList<object> selectedItemsCore) {
			if(selectedItemsCore.Count > 0) {
				if(SelectedItems.Count == 0)
					AddToSelectedItems(selectedItemsCore[0]);
				else if(SelectedItems.Count == 1)
					ReplaceFirstSelectedItem(selectedItemsCore[0]);
				else
					UpdateSelectedItemsCore();
			}
			else {
				ClearSelectedItems();
			}
		}
		protected internal override void UpdateSelectedItem() {
			if(DataControl.AllowUpdateCurrentItem)
				base.UpdateSelectedItem();
		}
		#region Mouse
		public override void OnBeforeMouseLeftButtonDown(System.Windows.DependencyObject originalSource) {
			base.OnBeforeMouseLeftButtonDown(originalSource);
			if(!IsControlPressed && !IsShiftPressed)
				ClearMasterDetailSelection();
		}
		#endregion
	}
}
