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

using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors.Filtering;
namespace DevExpress.XtraPivotGrid.FilterDropDown {
	public class PivotGroupFilterPopupContainerForm : PivotFilterPopupContainerForm {
		bool IsOneLevel { get { return Field.Group.Count == 1; } }
		public PivotGroupFilterPopupContainerForm(PivotFilterPopupContainerEdit ownerEdit)
			: base(ownerEdit) {
		}
		protected override void OnCheckListBoxMouseClick(object sender, MouseEventArgs e) { }
		public new CheckedTreeViewControl CheckListBox { get { return (CheckedTreeViewControl)base.CheckListBox; } }
		protected new CheckedTreeViewItem ShowAllItem { get { return CheckListBox.Items[0]; } }
		protected override void InitializeToolbarButtons() {
			InitializeToolbarButtonsCore(Options.ToolbarButtons & ~FilterPopupToolbarButtons.ShowOnlyAvailableItems);
		}
		protected override void InitializeToolbarButtonShowNewValues(PivotToolbarCheckButton btnShowNewValues) {
			FilterItems.ShowNewValues = Group.ShowNewValues;
			btnShowNewValues.IsChecked = Group.ShowNewValues;
			btnShowNewValues.CheckedChanged += (s, e) => FilterItems.ShowNewValues = e.IsChecked;
		}
		protected override CheckedListBoxControl CreateCheckListBox() {
			CheckedTreeViewControl res;
			res = IsRadioMode ? new SelectionCheckedTreeViewControl(IsOneLevel) : new CheckedTreeViewControl(IsOneLevel);
			base.InitializeCheckListBox(res);
			res.RetrieveChildElements += OnRetrieveChildElements;
			res.ActionsQueue = Data;
			return res;
		}
		protected override DXPopupMenu CreateContextMenu() {
			DXPopupMenu res = base.CreateContextMenu();
			res.Items.Add(CreateContextMenuItem(PivotGridStringId.PopupMenuCollapseAll, ContextMenuItemType.CollapseAll));
			res.Items.Add(CreateContextMenuItem(PivotGridStringId.PopupMenuExpandAll, ContextMenuItemType.ExpandAll));
			return res;
		}
		protected override void CollapseAllItems() {
			CheckListBox.CollapseLevelItems(CheckListBox.SelectedItem);
		}
		protected override void ExpandAllItems() {
			CheckListBox.ExpandLevelItems(CheckListBox.SelectedItem);
		}
		protected override void ShowContextMenu(Point pos) {
			int itemIndex = CheckListBox.IndexFromPoint(pos);
			bool itemFound = itemIndex >= 0;
			for(int i = 1; i <= 2; i++) {
				ContextMenu.Items[ContextMenu.Items.Count - i].Visible = itemFound;
			}
			if(itemFound) CheckListBox.SelectedIndex = itemIndex;
			base.ShowContextMenu(pos);
		}
		protected override void FillList() {
			base.FillList();
			if(IsRadioMode && GetCheckedFilterValuesCount(Group.FilterValues) == 1) {
				((SelectionCheckedTreeViewControl)CheckListBox).ExpandIndeterminateNodes();
			}
		}
		protected override void AddShowAllItem() {
			base.AddShowAllItem();
			ShowAllItem.LoadEmptyChildren();
		}
		int GetCheckedFilterValuesCount(PivotGroupFilterValues filterValues) {
			return GetCheckedFilterValuesCountCore(filterValues.Values);
		}
		int GetCheckedFilterValuesCountCore(PivotGroupFilterValuesCollection values) {
			int count = 0;
			foreach(PivotGroupFilterValue item in values) {
				if(item.ChildValues == null || item.ChildValues.Count == 0) {
					count++;
				} else {
					count += GetCheckedFilterValuesCountCore(item.ChildValues);
				}
			}
			return count;
		}
		public new PivotGroupFilterItems FilterItems { get { return (PivotGroupFilterItems)base.FilterItems; } }
		void OnRetrieveChildElements(object sender, RetrieveChildElementsEventArgs e) {
			OnRetrieveChildElements(sender, e, FilterItems, IsDeferredFillingCore);
		}
		public static void OnRetrieveChildElements(object sender, RetrieveChildElementsEventArgs e, PivotGroupFilterItems filterItems, bool isDeferredFillingCore) {
			if(e.IsEmpty) return;
			e.IsLastLevel = e.Branch.Length == filterItems.LevelCount - 1;
			List<object> list = filterItems.GetChildValues(e.Branch);
			if(list != null) {
				OnRetrieveChildElementsCore(e, list);
				return;
			}
			if(isDeferredFillingCore) {
				e.IsAsync = true;
				filterItems.LoadValuesAsync(e.Branch, result => {
					OnRetrieveChildElementsCore(e, (List<object>)result.Value ?? filterItems.GetChildValues(e.Branch));
					e.Callback();
				});
				return;
			}
			list = filterItems.LoadValues(e.Branch) ?? filterItems.GetChildValues(e.Branch);
			OnRetrieveChildElementsCore(e, list);
		}
		static void OnRetrieveChildElementsCore(RetrieveChildElementsEventArgs e, List<object> list) {
			e.ChildElements.Clear();
			e.CheckStates.Clear();
			foreach(PivotGroupFilterItem item in list) {
				e.ChildElements.Add(item);
				e.CheckStates.Add(item.IsChecked);
			}
		}
		protected override void UnselectIfNoCheckedItems() {
			if(!IsRadioMode) return;
			if(((SelectionCheckedTreeViewControl)CheckListBox).CheckedItemsCount == 0) {
				CheckListBox.SelectedIndex = -1;
			}
		}
	}
}
