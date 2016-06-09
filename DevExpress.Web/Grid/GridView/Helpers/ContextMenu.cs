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

using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Web.Data;
using DevExpress.Web.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
namespace DevExpress.Web {
	public enum GridViewContextMenuType { GroupPanel, Columns, Rows, Footer }
	public enum GridViewContextMenuCommand {
		 FullExpand, FullCollapse,
		 SortAscending, SortDescending, ClearSorting,
		 ClearFilter, ShowFilterEditor, ShowFilterRow, ShowFilterRowMenu,
		 GroupByColumn, UngroupColumn, ClearGrouping, ShowGroupPanel, ShowSearchPanel,
		 ShowColumn, HideColumn, ShowCustomizationWindow, ShowFooter,
		 NewRow, EditRow, DeleteRow,
		 ExpandRow, CollapseRow, ExpandDetailRow, CollapseDetailRow,
		 Refresh,
		 SummarySum, SummaryMin, SummaryMax, SummaryCount, SummaryAverage, SummaryNone,
		 Custom
	}
	public class GridViewContextMenuItemCollection : MenuItemCollection {
		public GridViewContextMenuItemCollection() 
			: base() {
		}
		public GridViewContextMenuItemCollection(GridViewContextMenuItem menuItem)
			: base(menuItem) {
		}
		public GridViewContextMenuItem Add(GridViewContextMenuCommand command) {
			return Add(command, string.Empty);
		}
		public GridViewContextMenuItem Add(GridViewContextMenuCommand command, string name) {
			return Add(command, name);
		}
		public GridViewContextMenuItem Add(GridViewContextMenuCommand command, string name, string text) {
			var item = new GridViewContextMenuItem(command, name) { Text = text };
			Add(item);
			return item;
		}
		public new GridViewContextMenuItem Add(string text) {
			return Add(text, "", "", "", "");
		}
		public new GridViewContextMenuItem Add(string text, string name) {
			return Add(text, name, "", "", "");
		}
		public new GridViewContextMenuItem Add(string text, string name, string imageUrl) {
			return Add(text, name, imageUrl, "", "");
		}
		public new GridViewContextMenuItem Add(string text, string name, string imageUrl, string navigateUrl) {
			return Add(text, name, imageUrl, navigateUrl, "");
		}
		public new GridViewContextMenuItem Add(string text, string name, string imageUrl, string navigateUrl, string target) {
			var item = new GridViewContextMenuItem(text, name, imageUrl, navigateUrl, target);
			Add(item);
			return item;
		}
		public void Add(params GridViewContextMenuItem[] items) { 
			base.Add(items); 
		}
		public new GridViewContextMenuItem FindByName(string name) {
			return base.FindByName(name) as GridViewContextMenuItem;
		}
		public new GridViewContextMenuItem FindByText(string text) {
			return base.FindByText(text) as GridViewContextMenuItem;
		}
		public GridViewContextMenuItem FindByCommand(GridViewContextMenuCommand command) {
			return (GridViewContextMenuItem)FindRecursive(delegate(MenuItem item) {
				return ((GridViewContextMenuItem)item).Command == command;
			});
		}
		public void Insert(int index, GridViewContextMenuItem item) {
			base.Insert(index, item);
		}
		public int IndexOfCommand(GridViewContextMenuCommand command) {
			return IndexOf(delegate(MenuItem item) {
				return ((GridViewContextMenuItem)item).Command == command;
			});
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override void Add(MenuItem item) { base.Add(item); }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new void Add(params MenuItem[] items) { base.Add(items); }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new MenuItem Add() { return base.Add(); }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new void Insert(int index, MenuItem item) { base.Insert(index, item); }
	}
	public class GridViewContextMenuItem : MenuItem {
		public GridViewContextMenuItem()
			: base() {
		}
		public GridViewContextMenuItem(GridViewContextMenuCommand command) 
			: this(command, command.ToString()) { 
		}
		public GridViewContextMenuItem(GridViewContextMenuCommand command, string name)
			: base() {
			Command = command;
			Name = name;
		}
		public GridViewContextMenuItem(string text)
			: this(text, "", "", "", "") {
		}
		public GridViewContextMenuItem(string text, string name)
			: this(text, name, "", "", "") {
		}
		public GridViewContextMenuItem(string text, string name, string imageUrl)
			: this(text, name, imageUrl, "", "") {
		}
		public GridViewContextMenuItem(string text, string name, string imageUrl, string navigateUrl)
			: this(text, name, imageUrl, navigateUrl, "") {
		}
		public GridViewContextMenuItem(string text, string name, string imageUrl, string navigateUrl, string target)
			: base(text, name, imageUrl, navigateUrl, target) {
			Command = GridViewContextMenuCommand.Custom;
		}
		protected internal GridViewContextMenuItem(GridViewContextMenu menu) 
			: base(menu) { 
		}
#if !SL
	[DevExpressWebLocalizedDescription("GridViewContextMenuItemCommand")]
#endif
		public GridViewContextMenuCommand Command { get; set; }
#if !SL
	[DevExpressWebLocalizedDescription("GridViewContextMenuItemItems")]
#endif
		public new GridViewContextMenuItemCollection Items { get { return base.Items as GridViewContextMenuItemCollection; } }
		protected override MenuItemCollection CreateItemsCollection() {
			return new GridViewContextMenuItemCollection(this);
		}
	}
}
namespace DevExpress.Web.Internal {
	public class GridViewContextMenuHelper {
		const int EmptyElementIndex = -1;
		DefaultBoolean hasVisibleHeaderColumns = DefaultBoolean.Default;
		public GridViewContextMenuHelper(ASPxGridView grid, GridViewContextMenu menu) {
			MenuType = menu.MenuType;
			Grid = grid;
			Items = menu.Items;
			ItemInfo = new Dictionary<string, GridViewContextMenuItemInfo>();
			PopulateItems();
		}
		public GridViewContextMenuType MenuType { get; private set; }
		public GridViewContextMenuItemCollection Items { get; private set; }
		protected ASPxGridView Grid { get; private set; }
		protected GridViewRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		protected ASPxGridViewTextSettings SettingsText { get { return Grid.SettingsText; } }
		protected WebDataProxy DataProxy { get { return Grid.DataProxy; } }
		protected GridViewColumnHelper ColumnHelper { get { return Grid.ColumnHelper; } }
		protected internal Dictionary<string, GridViewContextMenuItemInfo> ItemInfo { get; private set; }
		protected bool HasVisibleHeaderColumns {
			get {
				if(hasVisibleHeaderColumns == DefaultBoolean.Default) {
					hasVisibleHeaderColumns = DefaultBoolean.False;
					foreach(var column in ColumnHelper.AllVisibleDataColumns) {
						if(column.GroupIndex == -1)
							hasVisibleHeaderColumns = DefaultBoolean.True;
					}
				}
				return hasVisibleHeaderColumns == DefaultBoolean.True;
			}
		}
		public void SetVisible(GridViewContextMenuItem item, bool visible) {
			if(item == null)
				return;
			IterateNotEmptyInfoElements(item.IndexPath, i => i.Visible = visible);
			SetVisible(item, EmptyElementIndex, visible);
		}
		public void SetVisible(GridViewContextMenuItem item, GridViewColumn column, bool visible) {
			if(item == null || column == null)
				return;
			SetVisible(item, GetColumnIndex(column), visible);
		}
		public void SetVisible(GridViewContextMenuItem item, int groupElementIndex, bool visible) {
			var itemInfo = CreateItemInfo(item, groupElementIndex);
			itemInfo.Visible = visible;
		}
		public void SetEnabled(GridViewContextMenuItem item, bool enabled) {
			if(item == null)
				return;
			IterateNotEmptyInfoElements(item.IndexPath, i => i.Enabled = enabled);
			SetEnabled(item, EmptyElementIndex, enabled);
		}
		public void SetEnabled(GridViewContextMenuItem item, GridViewColumn column, bool enabled) {
			if(item == null || column == null)
				return;
			SetEnabled(item, GetColumnIndex(column), enabled);
		}
		public void SetEnabled(GridViewContextMenuItem item, int groupElementIndex, bool enabled) {
			var itemInfo = CreateItemInfo(item, groupElementIndex);
			itemInfo.Enabled = enabled;
		}
		protected void IterateNotEmptyInfoElements(string itemPath, Action<GridViewContextMenuItemInfo> action) { 
			foreach(var key in ItemInfo.Keys) {
				var itemInfo = ItemInfo[key];
				if(itemInfo.GroupElementIndex != EmptyElementIndex && itemInfo.Path == itemPath)
					action(itemInfo);
			}
		}
		protected void PopulateItems() {
			switch(MenuType) {
				case GridViewContextMenuType.GroupPanel:
					CreateGroupPanelItems();
					break;
				case GridViewContextMenuType.Columns:
					CreateColumnItems();
					break;
				case GridViewContextMenuType.Rows:
					CreateRowItems();
					break;
				case GridViewContextMenuType.Footer:
					CreateFooterItems();
					break;
				default:
					break;
			}
			Grid.RaiseFillContextMenuItems(new ASPxGridViewContextMenuEventArgs(this));
		}
		protected virtual void CreateGroupPanelItems() {
			CreateItem(GridViewContextMenuCommand.FullExpand);
			CreateItem(GridViewContextMenuCommand.FullCollapse);
			CreateItem(GridViewContextMenuCommand.ClearGrouping);
			CreateItem(GridViewContextMenuCommand.ShowGroupPanel);
		}
		protected virtual void CreateColumnItems() {
			CreateItem(GridViewContextMenuCommand.FullExpand);
			CreateItem(GridViewContextMenuCommand.FullCollapse);
			CreateItem(GridViewContextMenuCommand.SortAscending, true);
			CreateItem(GridViewContextMenuCommand.SortDescending);
			CreateItem(GridViewContextMenuCommand.ClearSorting);
			CreateItem(GridViewContextMenuCommand.GroupByColumn, true);
			CreateItem(GridViewContextMenuCommand.UngroupColumn);
			CreateItem(GridViewContextMenuCommand.ShowGroupPanel);
			CreateItem(GridViewContextMenuCommand.ShowColumn, true);
			CreateItem(GridViewContextMenuCommand.HideColumn);
			CreateItem(GridViewContextMenuCommand.ShowCustomizationWindow);
			CreateItem(GridViewContextMenuCommand.ClearFilter, true);
			CreateItem(GridViewContextMenuCommand.ShowSearchPanel);
			CreateItem(GridViewContextMenuCommand.ShowFilterEditor);
			CreateItem(GridViewContextMenuCommand.ShowFilterRow);
			CreateItem(GridViewContextMenuCommand.ShowFilterRowMenu);
			CreateItem(GridViewContextMenuCommand.ShowFooter);
		}
		protected virtual void CreateRowItems() {
			CreateItem(GridViewContextMenuCommand.ExpandRow, true);
			CreateItem(GridViewContextMenuCommand.CollapseRow);
			CreateItem(GridViewContextMenuCommand.ExpandDetailRow, true);
			CreateItem(GridViewContextMenuCommand.CollapseDetailRow);
			CreateItem(GridViewContextMenuCommand.NewRow, true);
			CreateItem(GridViewContextMenuCommand.EditRow);
			CreateItem(GridViewContextMenuCommand.DeleteRow);
			CreateItem(GridViewContextMenuCommand.Refresh, true);
		}
		protected virtual void CreateFooterItems() {
			CreateItem(GridViewContextMenuCommand.SummarySum, true);
			CreateItem(GridViewContextMenuCommand.SummaryMin);
			CreateItem(GridViewContextMenuCommand.SummaryMax);
			CreateItem(GridViewContextMenuCommand.SummaryCount);
			CreateItem(GridViewContextMenuCommand.SummaryAverage);
			CreateItem(GridViewContextMenuCommand.SummaryNone, true);
		}
		public void CreateItem(GridViewContextMenuCommand command, bool beginGroup = false) {
			var item = new GridViewContextMenuItem(command) { Text = GetItemText(command), BeginGroup = beginGroup };
			Items.Add(item);
			if(IsCheckableItem(item))
				item.GroupName = string.Format("gvCMItemGrp_{0}", item.Index);
		}
		protected virtual bool IsCheckableItem(GridViewContextMenuItem item) {
			switch(item.Command) {
				case GridViewContextMenuCommand.SortAscending:
				case GridViewContextMenuCommand.SortDescending:
				case GridViewContextMenuCommand.ShowFilterRow:
				case GridViewContextMenuCommand.ShowFilterRowMenu:
				case GridViewContextMenuCommand.ShowGroupPanel:
				case GridViewContextMenuCommand.ShowSearchPanel:
				case GridViewContextMenuCommand.ShowCustomizationWindow:
				case GridViewContextMenuCommand.ShowFooter:
				case GridViewContextMenuCommand.SummarySum:
				case GridViewContextMenuCommand.SummaryMin:
				case GridViewContextMenuCommand.SummaryMax:
				case GridViewContextMenuCommand.SummaryCount:
				case GridViewContextMenuCommand.SummaryAverage:
				case GridViewContextMenuCommand.Custom:
					return true;
			}
			return false;
		}
		protected virtual bool GetItemVisible(GridViewContextMenuCommand command) {
			if(command == GridViewContextMenuCommand.Custom)
				return true;
			switch(MenuType) {
				case GridViewContextMenuType.GroupPanel:
					return GetGroupPanelMenuItemVisible(command);
				case GridViewContextMenuType.Columns:
					return GetColumnsMenuItemVisible(command);
				case GridViewContextMenuType.Rows:
					return GetRowsMenuItemVisible(command);
				case GridViewContextMenuType.Footer:
					return GetFooterMenuItemVisible(command);
			}
			return true;
		}
		protected virtual bool GetGroupPanelMenuItemVisible(GridViewContextMenuCommand command) {
			switch(command) {
				case GridViewContextMenuCommand.FullExpand:
					return Grid.SettingsContextMenu.GroupPanelMenuItemVisibility.FullExpand;
				case GridViewContextMenuCommand.FullCollapse:
					return Grid.SettingsContextMenu.GroupPanelMenuItemVisibility.FullCollapse;
				case GridViewContextMenuCommand.ClearGrouping:
					return Grid.SettingsContextMenu.GroupPanelMenuItemVisibility.ClearGrouping;
				case GridViewContextMenuCommand.ShowGroupPanel:
					return Grid.SettingsContextMenu.GroupPanelMenuItemVisibility.ShowGroupPanel;
			}
			return false;
		}
		protected virtual bool GetColumnsMenuItemVisible(GridViewContextMenuCommand command) {
			switch(command) {
				case GridViewContextMenuCommand.FullExpand:
					return Grid.SettingsContextMenu.ColumnMenuItemVisibility.FullExpand;
				case GridViewContextMenuCommand.FullCollapse:
					return Grid.SettingsContextMenu.ColumnMenuItemVisibility.FullCollapse;
				case GridViewContextMenuCommand.SortAscending:
					return Grid.SettingsContextMenu.ColumnMenuItemVisibility.SortAscending;
				case GridViewContextMenuCommand.SortDescending:
					return Grid.SettingsContextMenu.ColumnMenuItemVisibility.SortDescending;
				case GridViewContextMenuCommand.ClearSorting:
					return Grid.SettingsContextMenu.ColumnMenuItemVisibility.ClearSorting;
				case GridViewContextMenuCommand.GroupByColumn:
					return Grid.SettingsContextMenu.ColumnMenuItemVisibility.GroupByColumn;
				case GridViewContextMenuCommand.UngroupColumn:
					return Grid.SettingsContextMenu.ColumnMenuItemVisibility.UngroupColumn;
				case GridViewContextMenuCommand.ShowGroupPanel:
					return Grid.SettingsContextMenu.ColumnMenuItemVisibility.ShowGroupPanel;
				case GridViewContextMenuCommand.ShowSearchPanel:
					return Grid.SettingsContextMenu.ColumnMenuItemVisibility.ShowSearchPanel;
				case GridViewContextMenuCommand.ShowColumn:
					return Grid.SettingsContextMenu.ColumnMenuItemVisibility.ShowColumn;
				case GridViewContextMenuCommand.HideColumn:
					return Grid.SettingsContextMenu.ColumnMenuItemVisibility.HideColumn;
				case GridViewContextMenuCommand.ShowCustomizationWindow:
					return Grid.SettingsContextMenu.ColumnMenuItemVisibility.ShowCustomizationWindow;
				case GridViewContextMenuCommand.ClearFilter:
					return Grid.SettingsContextMenu.ColumnMenuItemVisibility.ClearFilter;
				case GridViewContextMenuCommand.ShowFilterEditor:
					return Grid.SettingsContextMenu.ColumnMenuItemVisibility.ShowFilterEditor;
				case GridViewContextMenuCommand.ShowFilterRow:
					return Grid.SettingsContextMenu.ColumnMenuItemVisibility.ShowFilterRow;
				case GridViewContextMenuCommand.ShowFilterRowMenu:
					return Grid.SettingsContextMenu.ColumnMenuItemVisibility.ShowFilterRowMenu;
				case GridViewContextMenuCommand.ShowFooter:
					return Grid.SettingsContextMenu.ColumnMenuItemVisibility.ShowFooter;
			}
			return false;
		}
		protected virtual bool GetRowsMenuItemVisible(GridViewContextMenuCommand command) {
			switch(command) {
				case GridViewContextMenuCommand.NewRow:
					return Grid.SettingsContextMenu.RowMenuItemVisibility.NewRow;
				case GridViewContextMenuCommand.EditRow:
					return Grid.SettingsContextMenu.RowMenuItemVisibility.EditRow;
				case GridViewContextMenuCommand.DeleteRow:
					return Grid.SettingsContextMenu.RowMenuItemVisibility.DeleteRow;
				case GridViewContextMenuCommand.ExpandRow:
					return Grid.SettingsContextMenu.RowMenuItemVisibility.ExpandRow;
				case GridViewContextMenuCommand.CollapseRow:
					return Grid.SettingsContextMenu.RowMenuItemVisibility.CollapseRow;
				case GridViewContextMenuCommand.ExpandDetailRow:
					return Grid.SettingsContextMenu.RowMenuItemVisibility.ExpandDetailRow;
				case GridViewContextMenuCommand.CollapseDetailRow:
					return Grid.SettingsContextMenu.RowMenuItemVisibility.CollapseDetailRow;
				case GridViewContextMenuCommand.Refresh:
					return Grid.SettingsContextMenu.RowMenuItemVisibility.Refresh;
			}
			return false;
		}
		protected virtual bool GetFooterMenuItemVisible(GridViewContextMenuCommand command) {
			switch(command) {
				case GridViewContextMenuCommand.SummarySum:
					return Grid.SettingsContextMenu.FooterMenuItemVisibility.SummarySum;
				case GridViewContextMenuCommand.SummaryMin:
					return Grid.SettingsContextMenu.FooterMenuItemVisibility.SummaryMin;
				case GridViewContextMenuCommand.SummaryMax:
					return Grid.SettingsContextMenu.FooterMenuItemVisibility.SummaryMax;
				case GridViewContextMenuCommand.SummaryCount:
					return Grid.SettingsContextMenu.FooterMenuItemVisibility.SummaryCount;
				case GridViewContextMenuCommand.SummaryAverage:
					return Grid.SettingsContextMenu.FooterMenuItemVisibility.SummaryAverage;
				case GridViewContextMenuCommand.SummaryNone:
					return Grid.SettingsContextMenu.FooterMenuItemVisibility.SummaryNone;
			}
			return false;
		}
		protected virtual string GetItemText(GridViewContextMenuCommand command) {
			switch(command) {
				case GridViewContextMenuCommand.FullExpand:
					return SettingsText.GetContextMenuFullExpand();
				case GridViewContextMenuCommand.FullCollapse:
					return SettingsText.GetContextMenuFullCollapse();
				case GridViewContextMenuCommand.SortAscending:
					return SettingsText.GetContextMenuSortAscending();
				case GridViewContextMenuCommand.SortDescending:
					return SettingsText.GetContextMenuSortDescending();
				case GridViewContextMenuCommand.ClearSorting:
					return SettingsText.GetContextMenuClearSorting();
				case GridViewContextMenuCommand.ClearFilter:
					return SettingsText.GetContextMenuClearFilter();
				case GridViewContextMenuCommand.ShowFilterEditor:
					return SettingsText.GetContextMenuShowFilterEditor();
				case GridViewContextMenuCommand.ShowFilterRow:
					return SettingsText.GetContextMenuShowFilterRow();
				case GridViewContextMenuCommand.ShowFilterRowMenu:
					return SettingsText.GetContextMenuShowFilterRowMenu();
				case GridViewContextMenuCommand.ShowFooter:
					return SettingsText.GetContextMenuShowFooter();
				case GridViewContextMenuCommand.GroupByColumn:
					return SettingsText.GetContextMenuGroupByColumn();
				case GridViewContextMenuCommand.UngroupColumn:
					return SettingsText.GetContextMenuUngroupColumn();
				case GridViewContextMenuCommand.ClearGrouping:
					return SettingsText.GetContextMenuClearGrouping();
				case GridViewContextMenuCommand.ShowGroupPanel:
					return SettingsText.GetContextMenuShowGroupPanel();
				case GridViewContextMenuCommand.ShowSearchPanel:
					return SettingsText.GetContextMenuShowSearchPanel();
				case GridViewContextMenuCommand.ShowColumn:
					return SettingsText.GetContextMenuShowColumn();
				case GridViewContextMenuCommand.HideColumn:
					return SettingsText.GetContextMenuHideColumn();
				case GridViewContextMenuCommand.ShowCustomizationWindow:
					return SettingsText.GetContextMenuShowCustomizationWindow();
				case GridViewContextMenuCommand.NewRow:
					return SettingsText.GetContextMenuNewRow();
				case GridViewContextMenuCommand.EditRow:
					return SettingsText.GetContextMenuEditRow();
				case GridViewContextMenuCommand.DeleteRow:
					return SettingsText.GetContextMenuDeleteRow();
				case GridViewContextMenuCommand.ExpandRow:
					return SettingsText.GetContextMenuExpandRow();
				case GridViewContextMenuCommand.CollapseRow:
					return SettingsText.GetContextMenuCollapseRow();
				case GridViewContextMenuCommand.ExpandDetailRow:
					return SettingsText.GetContextMenuExpandDetailRow();
				case GridViewContextMenuCommand.CollapseDetailRow:
					return SettingsText.GetContextMenuCollapseDetailRow();
				case GridViewContextMenuCommand.Refresh:
					return SettingsText.GetContextMenuRefresh();
				case GridViewContextMenuCommand.SummarySum:
					return SettingsText.GetContextMenuSummarySum();
				case GridViewContextMenuCommand.SummaryMin:
					return SettingsText.GetContextMenuSummaryMin();
				case GridViewContextMenuCommand.SummaryMax:
					return SettingsText.GetContextMenuSummaryMax();
				case GridViewContextMenuCommand.SummaryAverage:
					return SettingsText.GetContextMenuSummaryAverage();
				case GridViewContextMenuCommand.SummaryCount:
					return SettingsText.GetContextMenuSummaryCount();
				case GridViewContextMenuCommand.SummaryNone:
					return SettingsText.GetContextMenuSummaryNone();
			}
			return string.Empty;
		}
		public object GetClientInfo() {
			IterateMenuItems(EnsureItemInfo);
			Grid.RaiseContextMenuItemVisibility(new ASPxGridViewContextMenuItemVisibilityEventArgs(this));
			return GenerateJson();
		}
		protected virtual object GenerateJson() {
			var result = new Dictionary<string, object>();
			foreach(var pathGroup in ItemInfo.Values.GroupBy(i => i.Path)) {
				var groupInfo = new List<object>();
				groupInfo.Add(GetGroupInfo(pathGroup.GroupBy(i => i.Visible)));
				groupInfo.Add(GetGroupInfo(pathGroup.GroupBy(i => i.Enabled)));
				groupInfo.Add(GetGroupInfo(pathGroup.GroupBy(i => i.Checked)));
				result[pathGroup.Key] = groupInfo;
			}
			return result;
		}
		protected List<object> GetGroupInfo(IEnumerable<IGrouping<bool, GridViewContextMenuItemInfo>> groups) {
			bool saveVisible;
			var result = new List<object>();
			var infoList = GetItemInfoList(groups, out saveVisible);
			result.Add(saveVisible ? 1 : 0);
			if(infoList != null)
				result.Add(infoList);
			return result;
		}
		protected int[] GetItemInfoList(IEnumerable<IGrouping<bool, GridViewContextMenuItemInfo>> groups, out bool saveVisible) {
			var dict = groups.ToDictionary(g => g.Key, g => g.ToList());
			if(dict.Count == 1) {
				saveVisible = !dict.Keys.First();
				return null;
			}
			var minGroupCount = dict.Values.Min(i => i.Count);
			var minGroupKey = dict.First(g => g.Value.Count == minGroupCount).Key;
			var minGroupList = dict[minGroupKey].Select(i => i.GroupElementIndex);
			saveVisible = minGroupKey;
			return minGroupList.ToArray();
		}
		protected virtual void EnsureItemInfo(GridViewContextMenuItem item) {
			switch(MenuType) {
				case GridViewContextMenuType.GroupPanel:
					EnsureGroupPanelItemInfo(item);
					break;
				case GridViewContextMenuType.Columns:
					EnsureEmptyHeaderInfo(item);
					EnsureColumnItemInfo(item);
					break;
				case GridViewContextMenuType.Rows:
					EnsureEmptyRowInfo(item);
					EnsureRowItemInfo(item);
					break;
				case GridViewContextMenuType.Footer:
					EnsureFooterItemInfo(item);
					break;
			}
		}
		protected virtual void EnsureGroupPanelItemInfo(GridViewContextMenuItem item) {
			switch(item.Command) {
				case GridViewContextMenuCommand.FullExpand:
					CreateGroupPanelItemInfo(item, GetGroupPanelFullExpandVisible, GetFullExpandEnabled);
					break;
				case GridViewContextMenuCommand.FullCollapse:
					CreateGroupPanelItemInfo(item, GetGroupPanelFullCollapseVisible, GetFullExpandEnabled);
					break;
				case GridViewContextMenuCommand.ClearGrouping:
					CreateGroupPanelItemInfo(item, GetGroupPanelClearGroupingVisible, GetClearGroupingEnabled);
					break;
				case GridViewContextMenuCommand.ShowGroupPanel:
					CreateGroupPanelItemInfo(item, GetGroupPanelShowGroupPanelVisible, GetShowGroupPanelEnabled, GetShowGroupPanelChecked);
					break;
				case GridViewContextMenuCommand.Custom:
					CreateGroupPanelItemInfo(item);
					break;
			}
		}
		protected virtual void EnsureColumnItemInfo(GridViewContextMenuItem item) {
			switch(item.Command) {
				case GridViewContextMenuCommand.FullExpand:
				case GridViewContextMenuCommand.FullCollapse:
					IterateColumns(item, GetFullExpandVisible, GetFullExpandEnabled);
					break;
				case GridViewContextMenuCommand.SortAscending:
					IterateColumns(item, GetAscendingVisible, GetSortAscendingEnabled, GetSortAscendingChecked);
					break;
				case GridViewContextMenuCommand.SortDescending:
					IterateColumns(item, GetDescendingVisible, GetSortDescendingEnabled, GetSortDescendingChecked);
					break;
				case GridViewContextMenuCommand.ClearSorting:
					IterateColumns(item, GetClearSortingVisible, GetClearSortingEnabled);
					break;
				case GridViewContextMenuCommand.ClearFilter:
					IterateColumns(item, GetShowClearFilterVisible, GetShowClearFilterEnabled);
					break;
				case GridViewContextMenuCommand.ShowFilterEditor:
					IterateColumns(item, GetShowFilterEditorVisible, GetShowFilterEditorEnabled);
					break;
				case GridViewContextMenuCommand.ShowFilterRow:
					IterateColumns(item, GetShowFilterRowVisible, GetShowFilterRowEnabled, GetShowFilterRowChecked);
					break;
				case GridViewContextMenuCommand.ShowFilterRowMenu:
					IterateColumns(item, GetShowFilterRowMenuVisible, GetShowFilterRowMenuEnabled, GetShowFilterRowMenuChecked);
					break;
				case GridViewContextMenuCommand.GroupByColumn:
					IterateColumns(item, GetGroupByColumnVisible, GetGroupByColumnEnabled);
					break;
				case GridViewContextMenuCommand.UngroupColumn:
					IterateColumns(item, GetUngroupColumnVisible, GetUngroupColumnEnabled);
					break;
				case GridViewContextMenuCommand.ShowGroupPanel:
					IterateColumns(item, GetShowGroupPanelVisible, GetShowGroupPanelEnabled, GetShowGroupPanelChecked);
					break;
				case GridViewContextMenuCommand.ShowSearchPanel:
					IterateColumns(item, GetShowSearchPanelVisible, GetShowSearchPanelEnabled, GetShowSearchPanelChecked);
					break;
				case GridViewContextMenuCommand.ShowColumn:
					IterateColumns(item, GetShowColumnVisible, GetShowColumnEnabled);
					break;
				case GridViewContextMenuCommand.HideColumn:
					IterateColumns(item, GetHideColumnVisible, GetHideColumnEnabled);
					break;
				case GridViewContextMenuCommand.ShowCustomizationWindow:
					IterateColumns(item, GetShowCustomizationWindowVisible, GetShowCustomizationWindowEnabled);
					break;
				case GridViewContextMenuCommand.ShowFooter:
					IterateColumns(item, GetShowFooterVisible, GetShowFooterEnabled, GetShowFooterChecked);
					break;
				case GridViewContextMenuCommand.Custom:
					IterateColumns(item);
					break;
			}
		}
		protected virtual void EnsureEmptyHeaderInfo(GridViewContextMenuItem item) {
			switch(item.Command) {
				case GridViewContextMenuCommand.ShowFilterEditor:
					CreateItemInfo(item, EmptyElementIndex, GetShowFilterEditorVisible(null), GetShowFilterEditorEnabled(null));
					break;
				case GridViewContextMenuCommand.ShowGroupPanel:
					CreateItemInfo(item, EmptyElementIndex, GetShowGroupPanelVisible(), GetShowGroupPanelEnabled(), GetShowGroupPanelChecked(null));
					break;
				case GridViewContextMenuCommand.ShowCustomizationWindow:
					CreateItemInfo(item, EmptyElementIndex, GetShowCustomizationWindowVisible(), GetShowCustomizationWindowEnabled());
					break;
				case GridViewContextMenuCommand.ShowFooter:
					CreateItemInfo(item, EmptyElementIndex, GetShowFooterVisible(null), GetShowFooterEnabled(null), GetShowFooterChecked(null));
					break;
				case GridViewContextMenuCommand.Custom:
					CreateItemInfo(item, EmptyElementIndex, true, true, item.Checked);
					break;
				default:
					CreateItemInfo(item, EmptyElementIndex, false, false);
					break;
			}
		}
		protected virtual void EnsureRowItemInfo(GridViewContextMenuItem item) {
			switch(item.Command) {
				case GridViewContextMenuCommand.NewRow:
					IterateVisibleRows(item, GetNewRowVisible, GetNewRowEnabled);
					break;
				case GridViewContextMenuCommand.EditRow:
					IterateVisibleRows(item, GetEditRowVisible, GetEditRowEnabled);
					break;
				case GridViewContextMenuCommand.DeleteRow:
					IterateVisibleRows(item, GetDeleteRowVisible, GetDeleteRowEnabled);
					break;
				case GridViewContextMenuCommand.ExpandRow:
					IterateVisibleRows(item, GetExpandRowVisible, GetExpandRowEnabled);
					break;
				case GridViewContextMenuCommand.CollapseRow:
					IterateVisibleRows(item, GetExpandRowVisible, GetCollapseRowEnabled);
					break;
				case GridViewContextMenuCommand.ExpandDetailRow:
					IterateVisibleRows(item, GetExpandDetailRowVisible, GetExpandDetailRowEnabled);
					break;
				case GridViewContextMenuCommand.CollapseDetailRow:
					IterateVisibleRows(item, GetExpandDetailRowVisible, GetCollapseDetailRowEnabled);
					break;
				case GridViewContextMenuCommand.Refresh:
					IterateVisibleRows(item, GetRefreshVisible, GetRefreshEnabled);
					break;
				case GridViewContextMenuCommand.Custom:
					IterateVisibleRows(item);
					break;
			}
		}
		protected virtual void EnsureEmptyRowInfo(GridViewContextMenuItem item) {
			switch(item.Command) {
				case GridViewContextMenuCommand.NewRow:
					CreateItemInfo(item, EmptyElementIndex, GetNewRowVisible(-1), GetNewRowEnabled(-1));
					break;
				case GridViewContextMenuCommand.Refresh:
					CreateItemInfo(item, EmptyElementIndex, GetRefreshVisible(-1), GetRefreshEnabled(-1));
					break;
				case GridViewContextMenuCommand.Custom:
					CreateItemInfo(item, EmptyElementIndex, true, true, item.Checked);
					break;
				default:
					CreateItemInfo(item, EmptyElementIndex, false, false);
					break;
			}
		}
		protected virtual void EnsureFooterItemInfo(GridViewContextMenuItem item) {
			switch(item.Command) {
				case GridViewContextMenuCommand.SummarySum:
					IterateColumns(item, GetSummarySumVisible, GetSummarySumEnabled, GetSummarySumChecked);
					break;
				case GridViewContextMenuCommand.SummaryMin:
					IterateColumns(item, GetSummaryMinVisible, GetSummaryMinEnabled, GetSummaryMinChecked);
					break;
				case GridViewContextMenuCommand.SummaryMax:
					IterateColumns(item, GetSummaryMaxVisible, GetSummaryMaxEnabled, GetSummaryMaxChecked);
					break;
				case GridViewContextMenuCommand.SummaryAverage:
					IterateColumns(item, GetSummaryAverageVisible, GetSummaryAverageEnabled, GetSummaryAverageChecked);
					break;
				case GridViewContextMenuCommand.SummaryCount:
					IterateColumns(item, GetSummaryCountVisible, GetSummaryCountEnabled, GetSummaryCountChecked);
					break;
				case GridViewContextMenuCommand.SummaryNone:
					IterateColumns(item, GetSummaryNoneVisible, GetSummaryNoneEnabled);
					break;
				case GridViewContextMenuCommand.Custom:
					IterateColumns(item);
					break;
			}
		}
		protected virtual void IterateColumns(GridViewContextMenuItem item) {
			IterateColumns(item, null, null, (column) => { return item.Checked; });
		}
		protected virtual void IterateColumns(GridViewContextMenuItem item, Func<GridViewColumn, bool> getVisible, Func<GridViewColumn, bool> getEnabled) {
			IterateColumns(item, getVisible, getEnabled, null);
		}
		protected virtual void IterateColumns(GridViewContextMenuItem item, Func<GridViewColumn, bool> getVisible, Func<GridViewColumn, bool> getEnabled, Func<GridViewColumn, bool> getChecked) {
			var isItemVisible = GetItemVisible(item.Command);
			foreach(var column in ColumnHelper.AllColumns) {
				var visible = isItemVisible && (getVisible != null ? getVisible(column) : true);
				var enabled = getEnabled != null ? getEnabled(column) : true;
				var isChecked = getChecked != null ? getChecked(column) : false;
				CreateItemInfo(item, column, visible, enabled, isChecked);
			}
		}
		protected virtual void IterateVisibleRows(GridViewContextMenuItem item) {
			IterateVisibleRows(item, null, null);
		}
		protected virtual void IterateVisibleRows(GridViewContextMenuItem item, Func<int, bool> getVisible, Func<int, bool> getEnabled) {
			var isItemVisible = GetItemVisible(item.Command);
			var start = DataProxy.VisibleStartIndex;
			var end = start + DataProxy.VisibleRowCountOnPage;
			for(var i = start; i < end; i++) {
				var visible = isItemVisible && (getVisible != null ? getVisible(i) : true);
				var enabled = getEnabled != null ? getEnabled(i) : true;
				CreateItemInfo(item, i, visible, enabled, item.Checked);
			}
		}
		protected virtual void CreateGroupPanelItemInfo(GridViewContextMenuItem item) {
			CreateItemInfo(item, EmptyElementIndex, true, true, item.Checked);
		}
		protected virtual void CreateGroupPanelItemInfo(GridViewContextMenuItem item, Func<bool> getVisible, Func<bool> getEnabled) {
			CreateItemInfo(item, EmptyElementIndex, getVisible(), getEnabled());
		}
		protected virtual void CreateGroupPanelItemInfo(GridViewContextMenuItem item, Func<bool> getVisible, Func<bool> getEnabled, Func<bool> getChecked) {
			CreateItemInfo(item, EmptyElementIndex, getVisible(), getEnabled(), getChecked());
		}
		protected virtual GridViewContextMenuItemInfo CreateItemInfo(GridViewContextMenuItem item, GridViewColumn column, bool visible, bool enabled) {
			return CreateItemInfo(item, GetColumnIndex(column), visible, enabled);
		}
		protected virtual GridViewContextMenuItemInfo CreateItemInfo(GridViewContextMenuItem item, GridViewColumn column, bool visible, bool enabled, bool isChecked) {
			return CreateItemInfo(item, GetColumnIndex(column), visible, enabled, isChecked);
		}
		protected virtual GridViewContextMenuItemInfo CreateItemInfo(GridViewContextMenuItem item, int groupElementIndex, bool visible, bool enabled) {
			return CreateItemInfo(item, groupElementIndex, visible, enabled, false);
		}
		protected virtual GridViewContextMenuItemInfo CreateItemInfo(GridViewContextMenuItem item, int groupElementIndex, bool visible, bool enabled, bool isChecked) {
			var result = CreateItemInfo(item, groupElementIndex);
			result.Visible = visible;
			result.Enabled = enabled;
			result.Checked = isChecked;
			return result;
		}
		protected virtual GridViewContextMenuItemInfo CreateItemInfo(GridViewContextMenuItem item, int groupElementIndex) {
			var key = GridViewContextMenuItemInfo.GenerateItemKey(item.IndexPath, groupElementIndex);
			if(!ItemInfo.ContainsKey(key))
				ItemInfo[key] = new GridViewContextMenuItemInfo(item.IndexPath, groupElementIndex);
			return ItemInfo[key];
		}
		protected internal bool GetFullExpandVisible() {
			return Grid.SettingsBehavior.AllowGroup && Grid.GroupCount != 0;
		}
		protected internal bool GetFullExpandEnabled() {
			return true;
		}
		protected internal bool GetGroupPanelFullExpandVisible() {
			return GetItemVisible(GridViewContextMenuCommand.FullExpand) && GetFullExpandVisible();
		}
		public virtual bool GetFullExpandVisible(GridViewColumn column) {
			var dataColumn = column as GridViewDataColumn;
			return Grid.SettingsBehavior.AllowGroup && dataColumn != null && dataColumn.GroupIndex != -1;
		}
		public virtual bool GetFullExpandEnabled(GridViewColumn column) {
			return true;
		}
		protected internal bool GetGroupPanelFullCollapseVisible() {
			return GetItemVisible(GridViewContextMenuCommand.FullCollapse) && GetFullExpandVisible();
		}
		public virtual bool GetAscendingVisible(GridViewColumn column) {
			return Grid.SettingsBehavior.AllowSort && column.GetAllowSort();
		}
		public virtual bool GetSortAscendingEnabled(GridViewColumn column) {
			return true;
		}
		public virtual bool GetSortAscendingChecked(GridViewColumn column) {
			var dataColumn = column as GridViewDataColumn;
			return dataColumn != null && dataColumn.SortOrder == ColumnSortOrder.Ascending;
		}
		public virtual bool GetDescendingVisible(GridViewColumn column) {
			return Grid.SettingsBehavior.AllowSort && column.GetAllowSort();
		}
		public virtual bool GetSortDescendingEnabled(GridViewColumn column) {
			return true;
		}
		public virtual bool GetSortDescendingChecked(GridViewColumn column) {
			var dataColumn = column as GridViewDataColumn;
			return dataColumn != null && dataColumn.SortOrder == ColumnSortOrder.Descending;
		}
		public virtual bool GetClearSortingVisible(GridViewColumn column) {
			var dataColumn = column as GridViewDataColumn;
			return Grid.SettingsBehavior.AllowSort && dataColumn != null && dataColumn.GetAllowSort() && dataColumn.SortIndex != -1;
		}
		public virtual bool GetClearSortingEnabled(GridViewColumn column) {
			var dataColumn = column as GridViewDataColumn;
			return Grid.SettingsBehavior.AllowSort && dataColumn != null && dataColumn.SortOrder != ColumnSortOrder.None;
		}
		public virtual bool GetShowClearFilterVisible(GridViewColumn column) {
			return HasFilterRow && column.GetIsFiltered();
		}
		public virtual bool GetShowClearFilterEnabled(GridViewColumn column) {
			return true;
		}
		public virtual bool GetShowFilterEditorVisible(GridViewColumn column) {
			return ColumnHelper.AllVisibleDataColumns.Count > 0 && Grid.Settings.ShowFilterBar != GridViewStatusBarMode.Hidden;
		}
		public virtual bool GetShowFilterEditorEnabled(GridViewColumn column) {
			return true;
		}
		public virtual bool GetShowFilterRowVisible(GridViewColumn column) {
			return true;
		}
		public virtual bool GetShowFilterRowEnabled(GridViewColumn column) {
			return true;
		}
		public virtual bool GetShowFilterRowChecked(GridViewColumn column) {
			return HasFilterRow;
		}
		public virtual bool GetShowFilterRowMenuVisible(GridViewColumn column) {
			return HasFilterRow;
		}
		public virtual bool GetShowFilterRowMenuEnabled(GridViewColumn column) {
			return true;
		}
		public virtual bool GetShowFilterRowMenuChecked(GridViewColumn column) {
			return Grid.Settings.ShowFilterRowMenu;
		}
		public virtual bool GetGroupByColumnVisible(GridViewColumn column) {
			var dataColumn = column as GridViewDataColumn;
			return Grid.SettingsBehavior.AllowGroup && dataColumn != null && dataColumn.GetAllowGroup() && dataColumn.GroupIndex == -1;
		}
		public virtual bool GetGroupByColumnEnabled(GridViewColumn column) {
			return true;
		}
		public virtual bool GetUngroupColumnVisible(GridViewColumn column) {
			var dataColumn = column as GridViewDataColumn;
			return Grid.SettingsBehavior.AllowGroup && dataColumn != null && dataColumn.GetAllowGroup() && dataColumn.GroupIndex != -1;
		}
		public virtual bool GetUngroupColumnEnabled(GridViewColumn column) {
			return true;
		}
		protected internal bool GetGroupPanelClearGroupingVisible() {
			return GetItemVisible(GridViewContextMenuCommand.ClearGrouping) && GetClearGroupingVisible();
		}
		protected internal bool GetClearGroupingVisible() {
			if(Grid.SettingsBehavior.AllowGroup)
				return true;
			foreach(var column in ColumnHelper.AllDataColumns)
				if(column.GroupIndex != -1)
					return true;
			return false;
		}
		protected internal bool GetClearGroupingEnabled() {
			return Grid.SettingsBehavior.AllowGroup && Grid.GroupCount != 0;
		}
		protected internal bool GetGroupPanelShowGroupPanelVisible() {
			return GetItemVisible(GridViewContextMenuCommand.ShowGroupPanel) && GetShowGroupPanelVisible();
		}
		public virtual bool GetShowGroupPanelVisible() {
			return GetShowGroupPanelVisible(null);
		}
		public virtual bool GetShowGroupPanelVisible(GridViewColumn column) {
			return Grid.SettingsBehavior.AllowGroup;
		}
		public virtual bool GetShowGroupPanelEnabled() {
			return GetShowGroupPanelEnabled(null);
		}
		public virtual bool GetShowGroupPanelEnabled(GridViewColumn column) {
			return true;
		}
		public virtual bool GetShowGroupPanelChecked() {
			return GetShowGroupPanelChecked(null);
		}
		public virtual bool GetShowGroupPanelChecked(GridViewColumn column) {
			return Grid.Settings.ShowGroupPanel;
		}
		public virtual bool GetShowSearchPanelVisible(GridViewColumn column) {
			return true;
		}
		public virtual bool GetShowSearchPanelEnabled(GridViewColumn column) {
			return true;
		}
		public virtual bool GetShowSearchPanelChecked(GridViewColumn column) {
			return Grid.SettingsSearchPanel.Visible;
		}
		public virtual bool GetShowColumnVisible(GridViewColumn column) {
			return Grid.SettingsBehavior.EnableCustomizationWindow && !column.Visible;
		}
		public virtual bool GetShowColumnEnabled(GridViewColumn column) {
			return true;
		}
		public virtual bool GetHideColumnVisible(GridViewColumn column) {
			return Grid.SettingsBehavior.EnableCustomizationWindow && column.Visible;
		}
		public virtual bool GetHideColumnEnabled(GridViewColumn column) {
			return true;
		}
		public virtual bool GetShowCustomizationWindowEnabled(GridViewColumn column) {
			return true;
		}
		public virtual bool GetShowCustomizationWindowVisible(GridViewColumn column) {
			return Grid.SettingsBehavior.EnableCustomizationWindow;
		}
		public virtual bool GetShowCustomizationWindowEnabled() {
			return true;
		}
		public virtual bool GetShowCustomizationWindowVisible() {
			return Grid.SettingsBehavior.EnableCustomizationWindow;
		}
		public virtual bool GetShowFooterVisible(GridViewColumn column) {
			return true;
		}
		public virtual bool GetShowFooterEnabled(GridViewColumn column) {
			return true;
		}
		public virtual bool GetShowFooterChecked(GridViewColumn column) {
			return Grid.Settings.ShowFooter;
		}
		public virtual bool GetNewRowVisible(int visibleIndex) {
			return AllowInsert;
		}
		public virtual bool GetNewRowEnabled(int visibleIndex) {
			return true;
		}
		public virtual bool GetEditRowVisible(int visibleIndex) {
			return AllowUpdate && IsDataRow(visibleIndex);
		}
		public virtual bool GetEditRowEnabled(int visibleIndex) {
			return !IsEditingRow(visibleIndex);
		}
		public virtual bool GetDeleteRowVisible(int visibleIndex) {
			return AllowDelete && IsDataRow(visibleIndex);
		}
		public virtual bool GetDeleteRowEnabled(int visibleIndex) {
			return true;
		}
		public virtual bool GetExpandRowVisible(int visibleIndex) {
			return Grid.GroupCount > 0 && DataProxy.GetRowType(visibleIndex) == WebRowType.Group;
		}
		public virtual bool GetExpandRowEnabled(int visibleIndex) {
			return !DataProxy.IsRowExpanded(visibleIndex);
		}
		public virtual bool GetCollapseRowVisible(int visibleIndex) {
			return Grid.GroupCount > 0 && DataProxy.GetRowType(visibleIndex) == WebRowType.Group;
		}
		public virtual bool GetCollapseRowEnabled(int visibleIndex) {
			return !GetExpandRowEnabled(visibleIndex);
		}
		public virtual bool GetExpandDetailRowVisible(int visibleIndex) {
			return Grid.SettingsDetail.ShowDetailRow && RenderHelper.HasDetailRows && DataProxy.GetRowType(visibleIndex) != WebRowType.Group;
		}
		public virtual bool GetExpandDetailRowEnabled(int visibleIndex) {
			return !GetCollapseDetailRowEnabled(visibleIndex);
		}
		public virtual bool GetCollapseDetailRowVisible(int visibleIndex) {
			return Grid.SettingsDetail.ShowDetailRow && GetExpandDetailRowVisible(visibleIndex);
		}
		public virtual bool GetCollapseDetailRowEnabled(int visibleIndex) {
			return Grid.DetailRows.IsVisible(visibleIndex);
		}
		public virtual bool GetRefreshVisible(int arg) {
			return Grid.SettingsContextMenu.RowMenuItemVisibility.Refresh;
		}
		public virtual bool GetRefreshEnabled(int arg) {
			return true;
		}
		protected virtual bool GetSummarySumVisible(GridViewColumn column) {
			return GetSummaryVisible(column, GridViewContextMenuCommand.SummarySum, SummaryItemType.Sum);
		}
		protected virtual bool GetSummarySumEnabled(GridViewColumn column) {
			return true;
		}
		protected internal bool GetSummarySumChecked(GridViewColumn column) {
			return HasSummary(column, SummaryItemType.Sum);
		}
		protected virtual bool GetSummaryMinVisible(GridViewColumn column) {
			return GetSummaryVisible(column, GridViewContextMenuCommand.SummaryMin, SummaryItemType.Min);
		}
		protected virtual bool GetSummaryMinEnabled(GridViewColumn column) {
			return true;
		}
		protected internal bool GetSummaryMinChecked(GridViewColumn column) {
			return HasSummary(column, SummaryItemType.Min);
		}
		protected virtual bool GetSummaryMaxVisible(GridViewColumn column) {
			return GetSummaryVisible(column, GridViewContextMenuCommand.SummaryMax, SummaryItemType.Max);
		}
		protected virtual bool GetSummaryMaxEnabled(GridViewColumn column) {
			return true;
		}
		protected internal bool GetSummaryMaxChecked(GridViewColumn column) {
			return HasSummary(column, SummaryItemType.Max);
		}
		protected virtual bool GetSummaryCountVisible(GridViewColumn column) {
			return GetSummaryVisible(column, GridViewContextMenuCommand.SummaryCount, SummaryItemType.Count);
		}
		protected virtual bool GetSummaryCountEnabled(GridViewColumn column) {
			return true;
		}
		protected internal bool GetSummaryCountChecked(GridViewColumn column) {
			return HasSummary(column, SummaryItemType.Count);
		}
		protected virtual bool GetSummaryAverageVisible(GridViewColumn column) {
			return GetSummaryVisible(column, GridViewContextMenuCommand.SummaryAverage, SummaryItemType.Average);
		}
		protected virtual bool GetSummaryAverageEnabled(GridViewColumn column) {
			return true;
		}
		protected internal bool GetSummaryAverageChecked(GridViewColumn column) {
			return HasSummary(column, SummaryItemType.Average);
		}
		protected virtual bool GetSummaryNoneVisible(GridViewColumn column) {
			var dataColumn = column as GridViewDataColumn;
			return dataColumn != null && Grid.TotalSummary.Count(i => i.FieldName == dataColumn.FieldName) != 0;
		}
		protected virtual bool GetSummaryNoneEnabled(GridViewColumn column) {
			return true;
		}
		protected virtual bool GetSummaryVisible(GridViewColumn column, GridViewContextMenuCommand command, SummaryItemType type) {
			var dataColumn = column as GridViewDataColumn;
			if(dataColumn == null || !dataColumn.Visible || !ColumnAccessSummary(dataColumn, command))
				return false;
			return Grid.Settings.ShowFooter;
		}
		protected bool ColumnAccessSummary(GridViewDataColumn dataColumn, GridViewContextMenuCommand summaryCommand) {
			if(!ValidateSummaryItemColumnType(dataColumn))
				return false;
			var dataType = dataColumn.GetDataType();
			if(dataType == typeof(DateTime))
				return summaryCommand == GridViewContextMenuCommand.SummaryMin || summaryCommand == GridViewContextMenuCommand.SummaryMax || 
					summaryCommand == GridViewContextMenuCommand.SummaryCount;
			if(dataType == typeof(bool))
				return summaryCommand != GridViewContextMenuCommand.SummaryAverage;
			if(dataType == typeof(string))
				return summaryCommand != GridViewContextMenuCommand.SummarySum && summaryCommand != GridViewContextMenuCommand.SummaryAverage;
			return true;
		}
		bool ValidateSummaryItemColumnType(GridViewDataColumn dataColumn) {
			var columnType = dataColumn.GetType();
			return columnType != typeof(GridViewDataColorEditColumn) && columnType != typeof(GridViewDataBinaryImageColumn) && columnType != typeof(GridViewDataButtonEditColumn);
		}
		public void PrepareItemImages() {
			IterateMenuItems(PrepareItemImage);
		}
		protected virtual void PrepareItemImage(GridViewContextMenuItem item) {
			if(!item.Image.IsEmpty)
				return;
			var properties = GetImageProperties(item.Command);
			if(properties != null)
				item.Image.CopyFrom(properties);
		}
		protected virtual ImageProperties GetImageProperties(GridViewContextMenuCommand command) {
			var imageName = GetImageName(command);
			if(string.IsNullOrEmpty(imageName))
				return null;
			var result = RenderHelper.GetImage(imageName);
			return !result.IsEmpty ? result : null;
		}
		protected virtual string GetImageName(GridViewContextMenuCommand command) {
			switch(command) {
				case GridViewContextMenuCommand.FullExpand:
					return GridViewImages.ContextMenuFullExpandName;
				case GridViewContextMenuCommand.FullCollapse:
					return GridViewImages.ContextMenuFullCollapseName;
				case GridViewContextMenuCommand.SortAscending:
					return GridViewImages.ContextMenuSortAscendingName;
				case GridViewContextMenuCommand.SortDescending:
					return GridViewImages.ContextMenuSortDescendingName;
				case GridViewContextMenuCommand.ClearSorting:
					return GridViewImages.ContextMenuClearSortingName;
				case GridViewContextMenuCommand.ClearFilter:
					return GridViewImages.ContextMenuClearFilterName;
				case GridViewContextMenuCommand.ShowFilterEditor:
					return GridViewImages.ContextMenuShowFilterEditorName;
				case GridViewContextMenuCommand.ShowFilterRow:
					return GridViewImages.ContextMenuShowFilterRowName;
				case GridViewContextMenuCommand.ShowFilterRowMenu:
					return GridViewImages.ContextMenuShowFilterRowMenuName;
				case GridViewContextMenuCommand.GroupByColumn:
					return GridViewImages.ContextMenuGroupByColumnName;
				case GridViewContextMenuCommand.UngroupColumn:
					return GridViewImages.ContextMenuUngroupColumnName;
				case GridViewContextMenuCommand.ClearGrouping:
					return GridViewImages.ContextMenuClearGroupingName;
				case GridViewContextMenuCommand.ShowGroupPanel:
					return GridViewImages.ContextMenuShowGroupPanelName;
				case GridViewContextMenuCommand.ShowSearchPanel:
					return GridViewImages.ContextMenuShowSearchPanelName;
				case GridViewContextMenuCommand.ShowColumn:
					return GridViewImages.ContextMenuShowColumnName;
				case GridViewContextMenuCommand.HideColumn:
					return GridViewImages.ContextMenuHideColumnName;
				case GridViewContextMenuCommand.ShowCustomizationWindow:
					return GridViewImages.ContextMenuShowCustomizationWindowName;
				case GridViewContextMenuCommand.NewRow:
					return GridViewImages.ContextMenuNewRowName;
				case GridViewContextMenuCommand.EditRow:
					return GridViewImages.ContextMenuEditRowName;
				case GridViewContextMenuCommand.DeleteRow:
					return GridViewImages.ContextMenuDeleteRowName;
				case GridViewContextMenuCommand.ExpandRow:
					return GridViewImages.ContextMenuExpandRowName;
				case GridViewContextMenuCommand.CollapseRow:
					return GridViewImages.ContextMenuCollapseRowName;
				case GridViewContextMenuCommand.ExpandDetailRow:
					return GridViewImages.ContextMenuExpandDetailRowName;
				case GridViewContextMenuCommand.CollapseDetailRow:
					return GridViewImages.ContextMenuCollapseDetailRowName;
				case GridViewContextMenuCommand.Refresh:
					return GridViewImages.ContextMenuRefreshName;
				case GridViewContextMenuCommand.SummarySum:
					return GridViewImages.ContextMenuSummarySumName;
				case GridViewContextMenuCommand.SummaryMin:
					return GridViewImages.ContextMenuSummaryMinName;
				case GridViewContextMenuCommand.SummaryMax:
					return GridViewImages.ContextMenuSummaryMaxName;
				case GridViewContextMenuCommand.SummaryAverage:
					return GridViewImages.ContextMenuSummaryAverageName;
				case GridViewContextMenuCommand.SummaryCount:
					return GridViewImages.ContextMenuSummaryCountName;
				case GridViewContextMenuCommand.SummaryNone:
					return GridViewImages.ContextMenuSummaryNoneName;
				case GridViewContextMenuCommand.ShowFooter:
					return GridViewImages.ContextMenuShowFooterName;
			}
			return string.Empty;
		}
		protected internal int GetColumnIndex(GridViewColumn column) {
			return ColumnHelper.GetColumnGlobalIndex(column);
		}
		protected void IterateMenuItems(Action<GridViewContextMenuItem> action) {
			IterateMenuItemsCore(Items, action);
		}
		protected void IterateMenuItemsCore(GridViewContextMenuItemCollection items, Action<GridViewContextMenuItem> action) {
			foreach(GridViewContextMenuItem item in items) {
				action(item);
				IterateMenuItemsCore(item.Items, action);
			}
		}
		bool HasSummary(GridViewColumn column, SummaryItemType type) {
			return Grid.GetVisibleTotalSummaryItems(column).Any(i => i.SummaryType == type);
		}
		bool IsEditingRow(int visibleIndex) {
			return DataProxy.EditingRowVisibleIndex == visibleIndex;
		}
		bool IsDataRow(int visibleIndex) {
			return DataProxy.GetRowType(visibleIndex) == WebRowType.Data;
		}
		bool HasKeyFieldName { get { return !string.IsNullOrEmpty(Grid.KeyFieldName); } }
		bool HasFilterRow { get { return Grid.Settings.ShowFilterRow; } }
		bool AllowInsert { get { return Grid.SettingsDataSecurity.AllowInsert && HasKeyFieldName && Grid.SettingsContextMenu.RowMenuItemVisibility.NewRow; } }
		bool AllowUpdate { get { return Grid.SettingsDataSecurity.AllowEdit && HasKeyFieldName && Grid.SettingsContextMenu.RowMenuItemVisibility.EditRow; } }
		bool AllowDelete { get { return Grid.SettingsDataSecurity.AllowDelete && HasKeyFieldName && Grid.SettingsContextMenu.RowMenuItemVisibility.DeleteRow; } }
	}
	public class GridViewContextMenuItemInfo {
		const string KeyFormat = "{0}_{1}";
		public GridViewContextMenuItemInfo(string path, int groupElementIndex) {
			Path = path;
			GroupElementIndex = groupElementIndex;
			Visible = Enabled = true;
		}
		public string Path { get; private set; }
		public int GroupElementIndex { get; private set; }
		public bool Visible { get; set; }
		public bool Enabled { get; set; }
		public bool Checked { get; set; }
		public static string GenerateItemKey(string path, int groupElementIndex) {
			return string.Format(KeyFormat, path, groupElementIndex);
		}
	}
}
