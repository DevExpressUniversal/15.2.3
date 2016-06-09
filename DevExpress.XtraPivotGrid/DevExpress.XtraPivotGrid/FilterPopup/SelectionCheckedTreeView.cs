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
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System.Collections;
using DevExpress.XtraPivotGrid.Localization;
using System.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using System.Collections.Generic;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.XtraEditors {
	[ToolboxItem(false)]
	public class SelectionCheckedTreeViewControl : CheckedTreeViewControl {
		public SelectionCheckedTreeViewControl(bool isOneLevel) : base(isOneLevel) {
			SelectionMode = SelectionMode.One;
			this.ItemCheck += OnItemCheck;
		}
		public void ExpandIndeterminateNodes() {
			for(int i = 1; i < Items.Count; i++) {
				if(Items[i].CheckState == CheckState.Indeterminate && !Items[i].IsExpanded) {
					ChangeExpanded(Items[i], true);
				}
			}
		}
		protected override void SetSelectedIndex(CheckedTreeViewItem selectedItem) {
			if(selectedItem != null)
				base.SetSelectedIndex(selectedItem);
			else
				SelectedIndex = CheckedItemIndex;
		}
		void OnItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e) {
			if(e.State == CheckState.Checked)
				SetItemCheckState(e.Index, e.State);
		}
		bool itemsStateInProgress;
		void BeginSetItemsState() {
			itemsStateInProgress = true; 
		}
		void EndSetItemsState() {
			itemsStateInProgress = false;
		}
		public override void SetItemCheckState(int index, CheckState value) {
			if(itemsStateInProgress) return;
			BeginSetItemsState();
			if(value == CheckState.Checked) {
				if(!IsShowAllItem(index)) {
					UnCheckAll(-1);
					SetParentItemState();
				}
			}
			base.SetItemCheckState(index, value);
			EndSetItemsState();
		}
		public override void ToggleItem(int index) {
			if(index < 0 || index > ItemCount - 1) return;
			SetItemCheckState(index, CheckState.Checked);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			int oldSelectedIndex = SelectedIndex;
			base.OnKeyDown(e);
			if(oldSelectedIndex != SelectedIndex)
				ToggleItem(SelectedIndex);
		}
		protected override void SetItemCheckStateCore(int index, CheckState value) {
			BeginUpdate();
			CheckedListBoxSetItemCheckStateCore(index, value);
			((SelectionCheckedTreeViewItem)Items[index]).SyncWithFilterItem();
			EndUpdate();
		}
		protected override ListBoxItemCollection CreateItemsCollection() {
			return new SelectionCheckedTreeViewItemCollection();
		}
		protected override void RemoveSubItems(CheckedTreeViewItem nodeItem) {
			if(IsAnySubItemChecked(nodeItem.SubItems))
				nodeItem.CheckState = CheckState.Checked;
			base.RemoveSubItems(nodeItem);
		}
		public override void InvertCheckState() {
			throw new Exception("Check state inverting is not supported.");
		}
		bool IsAnySubItemChecked(List<CheckedTreeViewItem> subItems) {
			foreach(CheckedTreeViewItem item in subItems) {
				if(item.CheckState == CheckState.Checked)
					return true;
			}
			return false;
		}
		bool IsShowAllItem(int index) {
			return ((SelectionCheckedTreeViewItem)Items[index]).IsShowAllItem;
		}
		void SetParentItemState() {
			if(SelectedIndex == -1 || (Items[SelectedIndex].CheckState == CheckState.Checked && CheckedItemsCount == 1)) return;
			if(Items[SelectedIndex].Parent != null)
				Items[SelectedIndex].Parent.CheckState = CheckState.Indeterminate;
		}
		void UnCheckAll(int ignoreIndex) {
			for(int i = 0; i < Items.Count; i++) {
				if(i == ignoreIndex) continue;
				Items[i].CheckState = CheckState.Unchecked;
			}
		}
		int CheckedItemIndex {
			get {
				int index = -1;
				for(int i = 0; i < Items.Count; i++) {
					if(Items[i].CheckState == CheckState.Checked) {
						if(index == -1)
							index = i;
						else
							return -1;
					}
				}
				return index;
			}
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new SelectionCheckedTreeViewInfo(this);
		}
	}
	public class SelectionCheckedTreeViewInfo : CheckedTreeViewInfo {
		public SelectionCheckedTreeViewInfo(SelectionCheckedTreeViewControl treeView)
			: base(treeView) {
		}
		protected override BaseListBoxViewInfo.ItemInfo CreateItemInfo(Rectangle bounds, object item, string text, int index) {
			bounds = OffsetBounds(bounds, LevelOffset * OwnerControl.GetItemLevel(index));
			Rectangle checkBounds = OwnerControl.GetItemIsLastLevel(index) ? bounds : OffsetBounds(bounds, FullOpenCloseButtonWidth);
			TreeItemInfo itemInfo = new TreeItemInfo(OwnerControl, checkBounds, item, text, index, index == -1 ? CheckState.Unchecked : OwnerControl.GetItemCheckState(index),
				true, OwnerControl.GetItemIsExpanded(index), OwnerControl.GetItemCanExpand(index));
			itemInfo.Bounds = bounds;
			UpdateOpenCloseButtonBounds(itemInfo.OpenCloseButtonArgs, bounds);
			return itemInfo;
		}
	}
}
namespace DevExpress.XtraEditors.Controls {
	[ListBindable(false)]
	public class SelectionCheckedTreeViewItemCollection : CheckedTreeViewItemCollection {
		public SelectionCheckedTreeViewItemCollection() : base() { }
		public SelectionCheckedTreeViewItemCollection(int capacity) : base(capacity) { }
		protected override CheckedListBoxItem CreateCheckedListBoxItem(object value, string description, CheckState checkState, bool enabled) {
			return new SelectionCheckedTreeViewItem(value, description, checkState, enabled, IsOneLevel);
		}
	}
	public class SelectionCheckedTreeViewItem : CheckedTreeViewItem {
		public SelectionCheckedTreeViewItem(object value, string description, CheckState checkState, bool enabled, bool isOneLevel)
			: base(value, description, checkState, enabled, isOneLevel) {
		}
		public SelectionCheckedTreeViewItem(object value, CheckedTreeViewItem parent)
			: base(value, parent, false, parent.CheckState) {
		}
		public SelectionCheckedTreeViewItem(object value, CheckedTreeViewItem parent, bool? isChecked)
			: base(value, parent, false, GetCheckState(isChecked)) {
		}
		public SelectionCheckedTreeViewItem(object value, CheckedTreeViewItem parent, bool isLastLevel, bool? isChecked)
			: base(value, parent, isLastLevel, GetCheckState(isChecked)) {
		}
		public SelectionCheckedTreeViewItem(object value, CheckedTreeViewItem parent, bool isLastLevel, CheckState checkState)
			: base(value, parent, isLastLevel, checkState) {
		}
		protected override CheckedTreeViewItem CreateTreeViewItem(object value, bool isLastLevel, bool? checkState) {
			return new SelectionCheckedTreeViewItem(value, this, isLastLevel, checkState);
		}
		public override CheckState CheckState {
			get { return base.CheckState; }
			set {
				if(CheckState == value && !IsShowAllItem && value != CheckState.Checked) return;
				if(value == CheckState.Checked) {
					SetCheckStateCore(CheckState.Unchecked);
					SetCheckStateCore(value, true);
				} else
					SetCheckStateCore(value);
				if(value == CheckState.Checked || value == CheckState.Indeterminate) {
					if(HasParent)
						Parent.CheckState = CheckState.Indeterminate;
				}
				SyncWithFilterItem();
			}
		}
		public bool IsShowAllItem { get { return Value as String == PivotGridLocalizer.GetString(PivotGridStringId.FilterShowAll); } }
		protected internal void SyncWithFilterItem() {
			if(IsShowAllItem) return;
			bool? state;
			if(CheckState == CheckState.Indeterminate)
				state = null;
			else
				state = CheckState == CheckState.Checked;
			PivotGroupFilterItem item = (PivotGroupFilterItem)Value;
			item.IsChecked = state;
			if(state != true) return;
			item = item.Parent;
			while(item != null && item.IsChecked == false) {
				item.IsChecked = null;
				item = item.Parent;
			}
		}
	}
}
