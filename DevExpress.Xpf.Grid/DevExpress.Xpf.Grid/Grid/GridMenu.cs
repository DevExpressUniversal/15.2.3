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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Data;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Summary;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Grid.Themes;
using System.Collections;
using System.Reflection;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Xpf.Core.ConditionalFormatting.Themes;
namespace DevExpress.Xpf.Grid {
	public class DefaultColumnMenuItemNamesBase {
		public const string SortAscending = "ItemSortAscending", SortDescending = "ItemSortDescending", ClearSorting = "ItemClearSorting", ColumnChooser = "ItemColumnChooser",
							ClearFilter = "ClearFilter", FilterEditor = "FilterEditor",
							FixedStyle = "FixedStyle", FixedNone = "FixedStyleNone", FixedLeft = "FixedStyleLeft", FixedRight = "FixedStyleRight",
							SearchPanel = "SearchPanel"
							;
	}
	public class DefaultColumnMenuItemNames : DefaultColumnMenuItemNamesBase {
		public const string FullExpand = "ItemFullExpand", FullCollapse = "ItemFullCollapse", GroupColumn = "ItemGroupColumn", UnGroupColumn = "ItemUnGroupColumn", GroupBox = "ItemGroupBox", GroupSummaryEditor = "ItemGroupSummaryEditor",
							ClearGrouping = "ItemClearGrouping", BestFit = "BestFit", BestFitColumns = "BestFitColumns", UnboundExpressionEditor = "UnboundExpressionEditor",
							MenuColumnGroupInterval = "GroupInterval",
							MenuColumnGroupIntervalNone = "GroupIntervalNone", MenuColumnGroupIntervalDay = "GroupIntervalDay", MenuColumnGroupIntervalMonth = "GroupIntervalMonth",
							MenuColumnGroupIntervalYear = "GroupIntervalYear", MenuColumnGroupIntervalSmart = "GroupIntervalSmart",
							ClearGroupSummarySorting = "ItemClearGroupSummarySorting",
							SortBySummary = "ItemSortBySummary",
							ConditionalFormatting = "ConditionalFormatting",
							ConditionalFormatting_HighlightCellsRules = "ConditionalFormatting_HighlightCellsRules",
							ConditionalFormatting_HighlightCellsRules_GreaterThan = "ConditionalFormatting_HighlightCellsRules_GreaterThan",
							ConditionalFormatting_HighlightCellsRules_LessThan = "ConditionalFormatting_HighlightCellsRules_LessThan",
							ConditionalFormatting_HighlightCellsRules_Between = "ConditionalFormatting_HighlightCellsRules_Between",
							ConditionalFormatting_HighlightCellsRules_EqualTo = "ConditionalFormatting_HighlightCellsRules_EqualTo",
							ConditionalFormatting_HighlightCellsRules_TextThatContains = "ConditionalFormatting_HighlightCellsRules_TextThatContains",
							ConditionalFormatting_HighlightCellsRules_ADateOccurring = "ConditionalFormatting_HighlightCellsRules_ADateOccurring",
							ConditionalFormatting_HighlightCellsRules_CustomCondition = "ConditionalFormatting_HighlightCellsRules_CustomCondition",
							ConditionalFormatting_TopBottomRules = "ConditionalFormatting_TopBottomRules",
							ConditionalFormatting_TopBottomRules_Top10Items = "ConditionalFormatting_TopBottomRules_Top10Items",
							ConditionalFormatting_TopBottomRules_Bottom10Items = "ConditionalFormatting_TopBottomRules_Bottom10Items",
							ConditionalFormatting_TopBottomRules_Top10Percent = "ConditionalFormatting_TopBottomRules_Top10Percent",
							ConditionalFormatting_TopBottomRules_Bottom10Percent = "ConditionalFormatting_TopBottomRules_Bottom10Percent",
							ConditionalFormatting_TopBottomRules_AboveAverage = "ConditionalFormatting_TopBottomRules_AboveAverage",
							ConditionalFormatting_TopBottomRules_BelowAverage = "ConditionalFormatting_TopBottomRules_BelowAverage",
							ConditionalFormatting_DataBars = "ConditionalFormatting_DataBars",
							ConditionalFormatting_ColorScales = "ConditionalFormatting_ColorScales",
							ConditionalFormatting_IconSets = "ConditionalFormatting_IconSets",
							ConditionalFormatting_ClearRules = "ConditionalFormatting_ClearRules",
							ConditionalFormatting_ClearRules_FromAllColumns = "ConditionalFormatting_ClearRules_FromAllColumns",
							ConditionalFormatting_ClearRules_FromCurrentColumns = "ConditionalFormatting_ClearRules_FromCurrentColumns",
							ConditionalFormatting_ManageRules = "Manage_Rules";
	}
	public class DefaultSummaryMenuItemNames {
		public const string Sum = "ItemSum", Min = "ItemMin", Max = "ItemMax", Count = "ItemCount", Average = "ItemAverage", Customize = "ItemCustomize";
	}
	public class GridGroupSummaryHelper : GridSummaryHelper {
		public GridGroupSummaryHelper(DataViewBase view)
			: base(view) {
		}
		protected override ISummaryItemOwner SummaryItems {
			get { return ((GridControl)view.DataControl).GroupSummary; }
		}
		protected override string GetEditorTitle() {
			return view.GetLocalizedString(GridControlStringId.GroupSummaryEditorFormCaption);
		}
		protected internal override bool CanUseSummaryItem(ISummaryItem item) {
			IGroupFooterSummaryItem groupSummaryItem = item as IGroupFooterSummaryItem;
			if(groupSummaryItem != null && !string.IsNullOrEmpty(groupSummaryItem.ShowInGroupColumnFooter))
			   return false;
			if(!(view is IGroupSummaryDisplayMode) || item.SummaryType != SummaryItemType.Count)
				return base.CanUseSummaryItem(item);
			IGroupSummaryDisplayMode viewWithSummaryDisplayMode = view as IGroupSummaryDisplayMode;
			if(viewWithSummaryDisplayMode.GroupSummaryDisplayMode == GroupSummaryDisplayMode.Default) {
				return item.FieldName == "";
			}
			return base.CanUseSummaryItem(item);
		}
		public override FloatingContainer ShowSummaryEditor() {
			return ShowSummaryEditor(SummaryEditorType.GroupSummary);
		}
	}
	public abstract class ColumnMenuInfoBase : GridMenuInfo, IConditionalFormattingDialogBuilder {
		protected ColumnSortOrder SortOrder { get { return Column == null ? ColumnSortOrder.None : Column.SortOrder; } }
		HeaderPresenterType HeaderPresenterType { get; set; }
		public override GridMenuType MenuType {
			get { return GridMenuType.Column; }
		}
		public override bool CanCreateItems {
			get { return View.IsColumnMenuEnabled; }
		}
		public override BarManagerMenuController MenuController {
			get { return View.ColumnMenuController; }
		}
		public ColumnMenuInfoBase(DataControlPopupMenu menu)
			: base(menu) { }
		protected void CreateFixedStyleItems() {
			if(View.ViewBehavior.CanShowFixedColumnMenu && CanCreateFixedStyleMenu()) {
				BarSubItem subItem = CreateBarSubItem(DefaultColumnMenuItemNames.FixedStyle, GridControlStringId.MenuColumnFixedStyle, true, null, null);
				CreateFixedStyleItem(DefaultColumnMenuItemNames.FixedNone, GridControlStringId.MenuColumnFixedNone, ImageHelper.GetImage("FixedNone"), subItem.ItemLinks, FixedStyle.None);
				CreateFixedStyleItem(DefaultColumnMenuItemNames.FixedLeft, GridControlStringId.MenuColumnFixedLeft, ImageHelper.GetImage("FixedLeft"), subItem.ItemLinks, FixedStyle.Left);
				CreateFixedStyleItem(DefaultColumnMenuItemNames.FixedRight, GridControlStringId.MenuColumnFixedRight, ImageHelper.GetImage("FixedRight"), subItem.ItemLinks, FixedStyle.Right);
				if(!View.CanBecameFixed(BaseColumn)) {
					subItem.ItemLinks[1].IsEnabled = false;
					subItem.ItemLinks[2].IsEnabled = false;
				}
			}
		}
		protected virtual bool CanCreateFixedStyleMenu() {
			return true;
		}
		void CreateFixedStyleItem(string name, GridControlStringId id, ImageSource image, BarItemLinkCollection links, FixedStyle fixedStyle) {
			Menu.CreateBarCheckItem(name, View.GetLocalizedString(id), BaseColumn.Fixed == fixedStyle, false, image, links).Command = DelegateCommandFactory.Create(
									delegate() { BaseColumn.Fixed = fixedStyle; },
									delegate { return true; }, false);
		}
		protected void CreateSortingItems() {
			CreateBarCheckItem(DefaultColumnMenuItemNames.SortAscending, GridControlStringId.MenuColumnSortAscending, SortOrder == ColumnSortOrder.Ascending, !CanClearSorting(), ImageHelper.GetImage(DefaultColumnMenuItemNames.SortAscending),
				DelegateCommandFactory.Create(
					delegate() { SetColumnSortOrder(ColumnSortOrder.Ascending); },
					delegate() { return CanSortColumn(Column); }, false));
			CreateBarCheckItem(DefaultColumnMenuItemNames.SortDescending, GridControlStringId.MenuColumnSortDescending, SortOrder == ColumnSortOrder.Descending, false, ImageHelper.GetImage(DefaultColumnMenuItemNames.SortDescending),
				DelegateCommandFactory.Create(
					delegate() { SetColumnSortOrder(ColumnSortOrder.Descending); },
					delegate() { return CanSortColumn(Column); }, false));
			CreateBarButtonItem(DefaultColumnMenuItemNames.ClearSorting, GridControlStringId.MenuColumnClearSorting, false, ImageHelper.GetImage(DefaultColumnMenuItemNames.ClearSorting),
				DelegateCommandFactory.Create(
					delegate() { ClearSorting(); },
					delegate() { return CanSortColumn(Column) && CanClearSorting() && Column.SortOrder != ColumnSortOrder.None; }, false));
		}
		protected bool CanSortColumn(ColumnBase column) {
			return DataControl.DataControlOwner.CanSortColumn(column);
		}
		protected virtual bool CanClearSorting() {
			return true;
		}
		void ClearSorting() {
			DataControl.SortInfoCore.OnColumnHeaderClickRemoveSort(Column.FieldName);
		}
		void SetColumnSortOrder(ColumnSortOrder sortOrder) {
			SetColumnSortOrderCore(sortOrder);
		}
		void SetColumnSortOrderCore(ColumnSortOrder sortOrder) {
			if(Column.SortOrder != sortOrder)
				View.SortInfoCore.OnColumnHeaderClickAddSort(Column.FieldName, GridSortInfo.GetSortDirectionBySortOrder(sortOrder));
		}
		protected void CreateColumnChooserItems() {
			if(!View.IsColumnChooserVisible)
				CreateBarButtonItem(DefaultColumnMenuItemNames.ColumnChooser, DataControl.AllowBandChooser ? GridControlStringId.MenuColumnShowColumnBandChooser : GridControlStringId.MenuColumnShowColumnChooser, true, ImageHelper.GetImage(DefaultColumnMenuItemNames.ColumnChooser), View.Commands.ShowColumnChooser);
			else
				CreateBarButtonItem(DefaultColumnMenuItemNames.ColumnChooser, DataControl.AllowBandChooser ? GridControlStringId.MenuColumnHideColumnBandChooser : GridControlStringId.MenuColumnHideColumnChooser, true, ImageHelper.GetImage(DefaultColumnMenuItemNames.ColumnChooser), View.Commands.HideColumnChooser);
		}
		protected void CreateFilterControlItems() {
			bool showFilterSeparator = true;
			if(Column.IsFiltered && Column.ActualAllowColumnFiltering) {
				BarButtonItem clearColumnFilterItem = CreateBarButtonItem(DefaultColumnMenuItemNames.ClearFilter, GridControlStringId.MenuColumnClearFilter, true, ImageHelper.GetImage(DefaultColumnMenuItemNames.ClearFilter), Column.Commands.ClearColumnFilter);
				showFilterSeparator = false;
			}
			if(IsAllowFilterEditorForColumn(View, Column)) {
				CreateFilterEditorItem(showFilterSeparator);
			}
		}
		protected void CreateFilterEditorItem(bool showFilterSeparator) {
			BarButtonItem filterEditorItem = CreateBarButtonItem(DefaultColumnMenuItemNames.FilterEditor, GridControlStringId.MenuColumnFilterEditor, showFilterSeparator, ImageHelper.GetImage(DefaultColumnMenuItemNames.FilterEditor), View.Commands.ShowFilterEditor);
			filterEditorItem.CommandParameter = Column;
		}
		protected void CreateSearchPanelItems() {
			if(!View.IsRootView || View.ShowSearchPanelMode != ShowSearchPanelMode.Default)
				return;
			if(!View.ActualShowSearchPanel)
				CreateBarButtonItem(DefaultColumnMenuItemNames.SearchPanel, GridControlStringId.MenuColumnShowSearchPanel, false, ImageHelper.GetImage(DefaultColumnMenuItemNames.SearchPanel), View.Commands.ShowSearchPanel);
			else
				CreateBarButtonItem(DefaultColumnMenuItemNames.SearchPanel, GridControlStringId.MenuColumnHideSearchPanel, false, ImageHelper.GetImage(DefaultColumnMenuItemNames.SearchPanel), View.Commands.HideSearchPanel);
		}
		protected void CreateExpressionEditorItems() {
			if(Column.AllowUnboundExpressionEditor) {
				BarButtonItem expressionEditorItem = CreateBarButtonItem(DefaultColumnMenuItemNames.UnboundExpressionEditor, GridControlStringId.MenuColumnUnboundExpressionEditor, true, ImageHelper.GetImage(DefaultColumnMenuItemNames.UnboundExpressionEditor), View.Commands.ShowUnboundExpressionEditor);
				expressionEditorItem.CommandParameter = Column;
			}
		}
		protected virtual void CreateConditionalFormattingMenuItems() {
			if(HeaderPresenterType != HeaderPresenterType.Headers)
				return;
			ITableView tableView = View as ITableView;
			if(tableView == null || !tableView.TableViewBehavior.UseLightweightTemplatesHasFlag(UseLightweightTemplates.Row) || !Column.AllowConditionalFormattingMenu.GetValueOrDefault(tableView.AllowConditionalFormattingMenu))
				return;
			var director = new ConditionalFormattingDialogDirector(new DataControlDialogContext(Column), View.Commands as IConditionalFormattingCommands, this, View);
			director.AllowConditionalFormattingManager = tableView.AllowConditionalFormattingManager;
			director.IsServerMode = DataControl.DataProviderBase.IsServerMode || DataControl.DataProviderBase.IsAsyncServerMode;
			director.CreateMenuItems(Column);
		}
		#region IConditionalFormattingDialogBuilder Members
		BarButtonItem IConditionalFormattingDialogBuilder.CreateBarButtonItem(BarItemLinkCollection links, string name, string content, bool beginGroup, ImageSource image, ICommand command, object commandParameter) {
			return CreateBarButtonItem(links, name, content, beginGroup, image, command, commandParameter);
		}
		BarSplitButtonItem IConditionalFormattingDialogBuilder.CreateBarSplitButtonItem(BarItemLinkCollection links, string name, string content, bool beginGroup, ImageSource image) {
			return CreateBarSplitButtonItem(links, name, content, beginGroup, image);
		}
		BarSubItem IConditionalFormattingDialogBuilder.CreateBarSubItem(string name, string content, bool beginGroup, ImageSource image, ICommand command) {
			return CreateBarSubItem(name, content, beginGroup, image, command);
		}
		BarSubItem IConditionalFormattingDialogBuilder.CreateBarSubItem(BarItemLinkCollection links, string name, string content, bool beginGroup, ImageSource image, ICommand command) {
			return CreateBarSubItem(links, name, content, beginGroup, image, command);
		}
		#endregion
		bool IsAllowFilterEditorForColumn(DataViewBase view, ColumnBase column) {
			if(column == null)
				return IsAllowFilterEditor(view);
			return IsAllowFilterEditor(view) && column.ActualAllowColumnFiltering;
		}
		bool IsAllowFilterEditor(DataViewBase view) {
			if(!view.AllowFilterEditor)
				return false;
			if(!CriteriaToTreeProcessor.IsConvertibleOperator(DataControl.FilterCriteria))
				return false;
			foreach(ColumnBase column in view.ColumnsCore)
				if(column.ActualAllowColumnFiltering)
					return true;
			return false;
		}
		public sealed override bool Initialize(IInputElement value) {
			var gridHeader = LayoutHelper.FindParentObject<BaseGridHeader>(value as DependencyObject);
			HeaderPresenterType = GridColumn.GetHeaderPresenterType(gridHeader);
			InitializeCore(gridHeader);
			return base.Initialize(value);
		}
		protected virtual void InitializeCore(BaseGridHeader gridHeader) {
			base.BaseColumn = GridColumnHeader.GetGridColumn(gridHeader);
		}
		protected sealed override void CreateItems() {
			if(BaseColumn != null) {
				CreateItemsCore();
			}
		}
		protected abstract void CreateItemsCore();
	}
	public class GridBandMenuInfo : GridColumnMenuInfo {
		public GridBandMenuInfo(GridPopupMenu menu) : base(menu) { }
		protected override void CreateItemsCore() {
			CreateGroupPanelItem();
			CreateColumnChooserItems();
			CreateFilterEditorItem(true);
			CreateSearchPanelItems();
			CreateFixedStyleItems();
		}
		protected override bool CanCreateFixedStyleMenu() {
			return Band.Owner == DataControl.BandsLayoutCore;
		}
		public override GridMenuType MenuType {
			get { return GridMenuType.Band; }
		}
		public override BarManagerMenuController MenuController {
			get { return ((TableView)View).BandMenuController; }
		}
		public BandBase Band { get { return (BandBase)BaseColumn; } }
	}
	public class GridGroupRowMenuInfo : GridMenuInfo {
		public GridGroupRowMenuInfo(GridPopupMenu menu)
			: base(menu) {
		}
		public GroupRowData Row { get; private set; }
		public new GridViewBase View { get { return (GridViewBase)base.View; } }
		protected override void CreateItems() {  }
		public override bool Initialize(IInputElement value) {
			Row = RowData.FindRowData(value as DependencyObject) as GroupRowData;
			return base.Initialize(value);
		}
		public override GridMenuType MenuType {
			get { return GridMenuType.GroupRow; }
		}
		public override bool CanCreateItems {
			get { return View.IsGroupRowMenuEnabled; }
		}
		public override BarManagerMenuController MenuController {
			get { return View.GroupRowMenuController; }
		}
		protected override void ExecuteMenuController() {
			base.ExecuteMenuController();
			Menu.ExecuteOriginationViewMenuController((view) => { return ((GridViewBase)view).GroupRowMenuController; });
		}
	}
	public class GridColumnMenuInfo : ColumnMenuInfoBase {
		public new GridViewBase View { get { return (GridViewBase)base.View; } }
		public GridControl Grid { get { return (GridControl)DataControl; } }
		public new GridColumn Column { get { return (GridColumn)base.Column; } }
		protected GridViewBase TargetView { 
			get {
				if(View.HasClonedDetails)
					return (GridViewBase)View.DataControl.DetailClones.First().viewCore;
				return View;
			} 
		}
		internal readonly GridGroupSummaryHelper summaryHelper;
		public GridColumnMenuInfo(GridPopupMenu menu)
			: base(menu) {
			summaryHelper = new GridGroupSummaryHelper(TargetView);
		}
		protected override void CreateItemsCore() {
			CreateFullExpandCollapseItems();
			CreateSortingItems();
			CreateGroupSummarySortInfoItems();
			CreateGroupingItems();
			CreateColumnChooserItems();
			CreateBestFitItems();
			CreateGroupSummaryEditorItems();
			CreateExpressionEditorItems();
			CreateFilterControlItems();
			CreateSearchPanelItems();
			CreateFixedStyleItems();
			CreateConditionalFormattingMenuItems();
		}
		protected override bool CanClearSorting() {
			return !Column.IsGrouped;
		}
		protected override bool CanCreateFixedStyleMenu() {
			return (!Column.IsGrouped || View.ShowGroupedColumns) && Grid.Bands.Count == 0 && !TableView.IsCheckBoxSelectorColumn(Column.FieldName);
		}
		void CreateGroupSummaryEditorItems() {
			if(Column.IsGrouped) {
				CreateBarButtonItem(DefaultColumnMenuItemNames.GroupSummaryEditor, GridControlStringId.MenuColumnGroupSummaryEditor, true, null,
					DelegateCommandFactory.Create(
						delegate() { summaryHelper.ShowSummaryEditor(); },
						delegate { return View.IsRootView || View.HasClonedDetails; }, false));
			}
		}
		private void CreateBestFitItems() {
			if(View is TableView) {
				BarButtonItem bestFitItem = CreateBarButtonItem(DefaultColumnMenuItemNames.BestFit, GridControlStringId.MenuColumnBestFit, false, ImageHelper.GetImage(DefaultColumnMenuItemNames.BestFit), ((View as TableView).Commands as TableViewCommands).BestFitColumn);
				bestFitItem.CommandParameter = Column;
				bestFitItem.IsVisible = View.ViewBehavior.CanBestFitColumnCore(Column) && View.IsColumnVisibleInHeaders(Column);
				BarButtonItem bestFitColumnsItem = CreateBarButtonItem(DefaultColumnMenuItemNames.BestFitColumns, GridControlStringId.MenuColumnBestFitColumns, false, null, ((View as TableView).Commands as TableViewCommands).BestFitColumns);
				bestFitColumnsItem.IsVisible = View.ViewBehavior.CanBestFitAllColumns();
			}
		}
		private void CreateGroupingItems() {
			CreateGroupByItem();
			CreateGroupPanelItem();
			if(Column.IsGrouped && IsDateTimeColumn() && View.AllowDateTimeGroupIntervalMenu && IsGroupIntervalSupported()) {
				BarSubItem subItem = CreateBarSubItem(DefaultColumnMenuItemNames.MenuColumnGroupInterval, GridControlStringId.MenuColumnGroupInterval, false, null, null);
				CreateGroupIntervalItem(DefaultColumnMenuItemNames.MenuColumnGroupIntervalNone, GridControlStringId.MenuColumnGroupIntervalNone, subItem.ItemLinks, XtraGrid.ColumnGroupInterval.Default);
				CreateGroupIntervalItem(DefaultColumnMenuItemNames.MenuColumnGroupIntervalDay, GridControlStringId.MenuColumnGroupIntervalDay, subItem.ItemLinks, XtraGrid.ColumnGroupInterval.Date);
				CreateGroupIntervalItem(DefaultColumnMenuItemNames.MenuColumnGroupIntervalMonth, GridControlStringId.MenuColumnGroupIntervalMonth, subItem.ItemLinks, XtraGrid.ColumnGroupInterval.DateMonth);
				CreateGroupIntervalItem(DefaultColumnMenuItemNames.MenuColumnGroupIntervalYear, GridControlStringId.MenuColumnGroupIntervalYear, subItem.ItemLinks, XtraGrid.ColumnGroupInterval.DateYear);
				CreateGroupIntervalItem(DefaultColumnMenuItemNames.MenuColumnGroupIntervalSmart, GridControlStringId.MenuColumnGroupIntervalSmart, subItem.ItemLinks, XtraGrid.ColumnGroupInterval.DateRange);
			}
		}
		void CreateGroupByItem() {
			if(!Column.IsGrouped && Column.Visible && !View.ShowGroupedColumns && View.IsLastVisibleColumn(Column)) return;
			string groupItemName = Column.IsGrouped ? DefaultColumnMenuItemNames.UnGroupColumn : DefaultColumnMenuItemNames.GroupColumn;
			CreateBarButtonItem(groupItemName, (Column.IsGrouped ? GridControlStringId.MenuColumnUnGroup : GridControlStringId.MenuColumnGroup), true, ImageHelper.GetImage(groupItemName),
				DelegateCommandFactory.Create(
						delegate() { GroupColumn(); },
						delegate() { return DataControl.DataControlOwner.CanGroupColumn(Column); }, false));
		}
		bool IsGroupIntervalSupported() {
			return !(View.DataProviderBase.IsICollectionView || View.DataControl.IsWcfSource());
		}
		protected void CreateGroupPanelItem() {
			CreateBarButtonItem(DefaultColumnMenuItemNames.GroupBox, !View.ShowGroupPanel ? GridControlStringId.MenuColumnShowGroupPanel : GridControlStringId.MenuColumnHideGroupPanel, false, ImageHelper.GetImage(DefaultColumnMenuItemNames.GroupBox),
				DelegateCommandFactory.Create(
					delegate() { View.ShowGroupPanel = !View.ShowGroupPanel; },
					delegate { return true; }, false));
		}
		bool IsDateTimeColumn() {
			Type columnType = TargetView.GetColumnType(Column);
			if(columnType == null)
				return false;
			if(Type.GetTypeCode(columnType) == TypeCode.DateTime) {
				return true;
			}
			else {
				Type nullableType = Nullable.GetUnderlyingType(columnType);
				if((nullableType != null) && (Type.GetTypeCode(nullableType) == TypeCode.DateTime)) {
					return true;
				}
			}
			return false;
		}
		private void CreateGroupSummarySortInfoItems() {
			if(Column.IsGrouped && Grid.GroupSummary.Count > 0) {
				CreateSummarySortInfo();
			}
		}
		private void CreateFullExpandCollapseItems() {
			if(Column.IsGrouped) {
				CreateBarButtonItem(DefaultColumnMenuItemNames.FullExpand, GridControlStringId.MenuGroupPanelFullExpand, false, ImageHelper.GetImage(DefaultColumnMenuItemNames.FullExpand), View.GridViewCommands.ExpandAllGroups);
				CreateBarButtonItem(DefaultColumnMenuItemNames.FullCollapse, GridControlStringId.MenuGroupPanelFullCollapse, false, ImageHelper.GetImage(DefaultColumnMenuItemNames.FullCollapse), View.GridViewCommands.CollapseAllGroups);
			}
		}
		void CreateGroupIntervalItem(string name, GridControlStringId id, BarItemLinkCollection links, XtraGrid.ColumnGroupInterval groupIntreval) {
			Menu.CreateBarCheckItem(name, View.GetLocalizedString(id), Column.GroupInterval == groupIntreval, false, null, links).Command = DelegateCommandFactory.Create(
									delegate() { Column.GroupInterval = groupIntreval; },
									delegate { return true; }, false);
		}
		void GroupColumn() {
			if(Column.IsGrouped) {
				Grid.UngroupBy(Column);
			}
			else {
				Column.Visible = true;
				Grid.GroupBy(Column, Column.SortOrder == ColumnSortOrder.None ? ColumnSortOrder.Ascending : Column.SortOrder);
			}
		}
		protected virtual void CreateSummarySortInfo() {
			if(Grid.GroupSummarySortInfo.Count > 0)
				CreateBarButtonItem(DefaultColumnMenuItemNames.ClearGroupSummarySorting, GridControlStringId.MenuColumnResetGroupSummarySort, false, null,
					DelegateCommandFactory.Create(
						delegate() { Grid.GroupSummarySortInfo.Clear(); },
						delegate() { return true; }, false));
			BarSubItem subItem = CreateBarSubItem(DefaultColumnMenuItemNames.SortBySummary, GridControlStringId.MenuColumnSortGroupBySummaryMenu, false, null, null);
			foreach(GridSummaryItem item in Grid.GroupSummary) {
				if(!item.Visible) continue;
				if(item.SummaryType == SummaryItemType.None) continue;
				CreateSummarySortMenuItem(item, ColumnSortOrder.Ascending, subItem.ItemLinks, subItem.ItemLinks.Count > 0);
				CreateSummarySortMenuItem(item, ColumnSortOrder.Descending, subItem.ItemLinks, false);
			}
		}
		protected BarButtonItem CreateSummarySortMenuItem(GridSummaryItem item, ColumnSortOrder order, BarItemLinkCollection links, bool beginGroup) {
			string formatStr = View.GetLocalizedString(GridControlStringId.MenuColumnGroupSummarySortFormat);
			string orderStr = order == ColumnSortOrder.Ascending ? View.GetLocalizedString(GridControlStringId.MenuColumnSortBySummaryAscending) : View.GetLocalizedString(GridControlStringId.MenuColumnSortBySummaryDescending);
			GridControlStringId? id = GridControlLocalizer.GetMenuSortByGroupSummaryStringId(item.SummaryType);
			string summaryType = (id.HasValue) ? View.GetLocalizedString(id.Value) : item.SummaryType.ToString();
			BarButtonItem buttonItem = Menu.CreateBarButtonItem(Menu.GetItemName() + item.SummaryType + order + item.GetHashCode(), string.Format(formatStr, GetDisplaySummaryName(item),
				summaryType, orderStr), beginGroup, null, links);
			buttonItem.Command = DelegateCommandFactory.Create(new Action(delegate() { GroupBySummary(item, order); }), new Func<bool>(delegate() { return GetIsGroupSummaryEnabled(Column.FieldName, item, order); }), false);
			return buttonItem;
		}
		bool GetIsGroupSummaryEnabled(string fieldName, GridSummaryItem item, ColumnSortOrder sortOrder) {
			bool enabled = true;
			foreach(GridGroupSummarySortInfo info in Grid.GroupSummarySortInfo) {
				if(info.FieldName == fieldName && info.SummaryItem == item)
					enabled = info.GetSortOrder() != sortOrder;
			}
			return enabled;
		}
		void GroupBySummary(GridSummaryItem summaryItem, ColumnSortOrder sortOrder) {
			if(!Column.IsGrouped) return;
			List<GridGroupSummarySortInfo> items = new List<GridGroupSummarySortInfo>();
			foreach(GridGroupSummarySortInfo info in Grid.GroupSummarySortInfo) {
				if(info.FieldName == Column.FieldName ) continue;
				items.Add(info);
			}
			items.Add(new GridGroupSummarySortInfo(summaryItem, Column.FieldName, sortOrder == ColumnSortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending));
			Grid.GroupSummarySortInfo.ClearAndAddRange(items.ToArray());
		}
		string GetDisplaySummaryName(GridSummaryItem item) {
			return GridColumn.GetSummaryDisplayName(View.Columns[item.FieldName], item);
		}
		protected override void ExecuteMenuController() {
			base.ExecuteMenuController();
			Menu.ExecuteOriginationViewMenuController((view) => { return view.ColumnMenuController; });
		}
	}
	public class GridTotalSummaryMenuInfo : GridMenuInfo {
		protected readonly GridTotalSummaryHelper summaryHelper;
		protected GridSummaryItemsEditorController Controller { get { return summaryHelper.Controller; } }
		protected virtual ISummaryItemOwner SummaryItemsCore { get { return DataControl.TotalSummaryCore; } }
		internal GridTotalSummaryHelper SummaryHelper {
			get { return summaryHelper; }
		}
		public GridTotalSummaryMenuInfo(DataControlPopupMenu menu)
			: base(menu) {
			summaryHelper = CreateSummaryHelper();
		}
		protected virtual GridTotalSummaryHelper CreateSummaryHelper() {
			return new GridTotalSummaryHelper(View, () => Column);
		}
		public override bool Initialize(IInputElement value) {
			SubscribeEvents();
			InitializeCore(value);
			return base.Initialize(value);
		}
		protected virtual void SubscribeEvents() {
			SummaryItemsCore.CollectionChanged += OnSummaryItemsCollectionChanged;
		}
		protected virtual void UnsubcribeEvents() {
			SummaryItemsCore.CollectionChanged -= OnSummaryItemsCollectionChanged;
		}
		public override void Uninitialize() {
			base.Uninitialize();
			UnsubcribeEvents();
		}
		protected virtual void OnSummaryItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			if(synchronizingSummaries > 0) return;
			synchronizingSummaries++;
			try {
				ISummaryItemOwner collection = (ISummaryItemOwner)sender;
				foreach(SummaryItemBase item in collection)
					if(!Controller.Items.Contains(item) && item.Visible && SummaryHelper.CanUseSummaryItem(item))
						Controller.Items.Add(item);
				foreach(ISummaryItem item in new List<ISummaryItem>(Controller.Items)) {
					if(!collection.Contains(item))
						Controller.Items.Remove(item);
				}
			}
			finally {
				synchronizingSummaries--;
			}
		}
		protected virtual void InitializeCore(IInputElement value) {
			FrameworkElement summaryElement = value as FrameworkElement;
			if(summaryElement != null) {
				GridColumnData data = summaryElement.DataContext as GridColumnData;
				if(data != null)
					BaseColumn = data.Column;
			}
		}
		protected override void CreateItems() {
			CreateBarCheckItem(DefaultSummaryMenuItemNames.Sum, GridControlStringId.MenuFooterSum, Controller.HasSummary(Column.FieldName, SummaryItemType.Sum), false, ImageHelper.GetImage(DefaultSummaryMenuItemNames.Sum),
				DelegateCommandFactory.Create(
					delegate() { SetSummary(Column.FieldName, SummaryItemType.Sum); },
					delegate() { return Controller.CanApplySummary(SummaryItemType.Sum, Column.FieldName); }), false);
			CreateBarCheckItem(DefaultSummaryMenuItemNames.Min, GridControlStringId.MenuFooterMin, Controller.HasSummary(Column.FieldName, SummaryItemType.Min), false, ImageHelper.GetImage(DefaultSummaryMenuItemNames.Min),
				DelegateCommandFactory.Create(
					delegate() { SetSummary(Column.FieldName, SummaryItemType.Min); },
					delegate() { return Controller.CanApplySummary(SummaryItemType.Min, Column.FieldName); ; }), false);
			CreateBarCheckItem(DefaultSummaryMenuItemNames.Max, GridControlStringId.MenuFooterMax, Controller.HasSummary(Column.FieldName, SummaryItemType.Max), false, ImageHelper.GetImage(DefaultSummaryMenuItemNames.Max),
				DelegateCommandFactory.Create(
					delegate() { SetSummary(Column.FieldName, SummaryItemType.Max); },
					delegate() { return Controller.CanApplySummary(SummaryItemType.Max, Column.FieldName); ; }), false);
			CreateBarCheckItem(DefaultSummaryMenuItemNames.Count, GridControlStringId.MenuFooterCount, Controller.HasSummary(Column.FieldName, SummaryItemType.Count), false, ImageHelper.GetImage(DefaultSummaryMenuItemNames.Count),
				DelegateCommandFactory.Create(
					delegate() { SetSummary(Column.FieldName, SummaryItemType.Count); },
					delegate() { return Controller.CanApplySummary(SummaryItemType.Count, Column.FieldName); }), false);
			CreateBarCheckItem(DefaultSummaryMenuItemNames.Average, GridControlStringId.MenuFooterAverage, Controller.HasSummary(Column.FieldName, SummaryItemType.Average), false, ImageHelper.GetImage(DefaultSummaryMenuItemNames.Average),
				DelegateCommandFactory.Create(
					delegate() { SetSummary(Column.FieldName, SummaryItemType.Average); },
					delegate() { return Controller.CanApplySummary(SummaryItemType.Average, Column.FieldName); }), false);
			CreateBarButtonItem(DefaultSummaryMenuItemNames.Customize, GridControlStringId.MenuFooterCustomize, true, null,
				DelegateCommandFactory.Create(
					delegate() { summaryHelper.ShowSummaryEditor(); },
					delegate { return Controller.Count > 0; }, false));
		}
		protected virtual BarCheckItem CreateBarCheckItem(string name, string content, bool? isChecked, bool beginGroup, ImageSource image, ICommand command, bool closeMenuOnClick) {
			BarCheckItem item = CreateBarCheckItem(name, content, isChecked, beginGroup, image, command);
			item.CloseSubMenuOnClick = closeMenuOnClick;
			return item;
		}
		protected virtual BarCheckItem CreateBarCheckItem(string name, GridControlStringId id, bool? isChecked, bool beginGroup, ImageSource image, ICommand command, bool closeMenuOnClick) {
			BarCheckItem item = CreateBarCheckItem(name, id, isChecked, beginGroup, image, command);
			item.CloseSubMenuOnClick = closeMenuOnClick;
			return item;
		}
		public override GridMenuType MenuType {
			get { return GridMenuType.TotalSummary; }
		}
		public override bool CanCreateItems {
			get { return View.IsTotalSummaryMenuEnabled && !IsCheckBoxSelectorColumnMenu(); }
		}
		internal bool IsCheckBoxSelectorColumnMenu() {
			return Column != null && TableView.IsCheckBoxSelectorColumn(Column.FieldName);
		}
		public override BarManagerMenuController MenuController {
			get { return View.TotalSummaryMenuController; }
		}
		int synchronizingSummaries = 0;
		protected virtual void SetSummary(string fieldName, SummaryItemType type) {
			synchronizingSummaries++;
			try {
				Controller.SetSummary(fieldName, type, !Controller.HasSummary(fieldName, type)); Controller.Apply();
			}
			finally {
				synchronizingSummaries--;
			}
		}
		protected override void ExecuteMenuController() {
			base.ExecuteMenuController();
			Menu.ExecuteOriginationViewMenuController((view) => { return view.TotalSummaryMenuController; });
		}
	}
	public class GridGroupFooterSummaryMenuInfo : GridTotalSummaryMenuInfo {
		public GridGroupFooterSummaryMenuInfo(DataControlPopupMenu menu) : base(menu) {  }
		public new GridViewBase View { get { return (GridViewBase)base.View; } }
		protected override GridTotalSummaryHelper CreateSummaryHelper() {
			return new GridGroupFooterSummaryHelper(View, () => Column);
		}
		public override GridMenuType MenuType { get { return GridMenuType.GroupFooterSummary;  } }
		public override bool CanCreateItems { get { return View.IsGroupFooterMenuEnabled && !IsCheckBoxSelectorColumnMenu(); } }
		public override BarManagerMenuController MenuController { get { return View.GroupFooterMenuController;  } }
		protected override ISummaryItemOwner SummaryItemsCore { get { return DataControl.GroupSummaryCore; } }
	}
	public class GridGroupPanelMenuInfo : GridMenuInfo {
		public new GridViewBase View { get { return (GridViewBase)base.View; } }
		public GridGroupPanelMenuInfo(GridPopupMenu menu)
			: base(menu) {
		}
		protected override void CreateItems() {
			CreateBarButtonItem(DefaultColumnMenuItemNames.FullExpand, GridControlStringId.MenuGroupPanelFullExpand, false, ImageHelper.GetImage(DefaultColumnMenuItemNames.FullExpand), View.GridViewCommands.ExpandAllGroups);
			CreateBarButtonItem(DefaultColumnMenuItemNames.FullCollapse, GridControlStringId.MenuGroupPanelFullCollapse, false, ImageHelper.GetImage(DefaultColumnMenuItemNames.FullCollapse), View.GridViewCommands.CollapseAllGroups);
			CreateBarButtonItem(DefaultColumnMenuItemNames.ClearGrouping, GridControlStringId.MenuGroupPanelClearGrouping, true, ImageHelper.GetImage(DefaultColumnMenuItemNames.ClearGrouping), View.GridViewCommands.ClearGrouping);
		}
		public override GridMenuType MenuType {
			get { return GridMenuType.GroupPanel; }
		}
		public override bool CanCreateItems {
			get { return View.IsGroupPanelMenuEnabled; }
		}
		public override BarManagerMenuController MenuController {
			get { return View.GroupPanelMenuController; }
		}
	}
	public class GridTotalSummaryPanelMenuInfo : GridTotalSummaryMenuInfo {
		public GridTotalSummaryPanelMenuInfo(DataControlPopupMenu menu)
			: base(menu) {
		}
		protected override GridTotalSummaryHelper CreateSummaryHelper() {
			return new GridTotalSummaryPanelHelper(View, () => Column);
		}
		protected override void CreateItems() {
			CreateBarCheckItem(DefaultSummaryMenuItemNames.Count, GridControlStringId.MenuFooterCount,
				IsCountButtonEnabled(), false,
				ImageHelper.GetImage(DefaultSummaryMenuItemNames.Count), DelegateCommandFactory.Create(
					delegate() { SetSummary(String.Empty, SummaryItemType.Count); },
					delegate() { return true; }), false);
			CreateBarButtonItem(DefaultSummaryMenuItemNames.Customize, GridControlStringId.MenuFooterCustomize, true, null,
				DelegateCommandFactory.Create(
					delegate() { summaryHelper.ShowSummaryEditor(); },
					delegate { return true; }, false));
		}
		bool IsCountButtonEnabled() {
			if(View.FixedSummariesHelper.FixedSummariesRightCore.Count + View.FixedSummariesHelper.FixedSummariesLeftCore.Count == 0)
				return false;
			foreach(SummaryItemBase item in View.FixedSummariesHelper.FixedSummariesRightCore) {
				if(item.SummaryType == SummaryItemType.Count)
					return true;
			}
			foreach(SummaryItemBase item in View.FixedSummariesHelper.FixedSummariesLeftCore) {
				if(item.SummaryType == SummaryItemType.Count)
					return true;
			}
			return false;
		}
		public override GridMenuType MenuType {
			get { return GridMenuType.FixedTotalSummary; }
		}
		protected override void SetSummary(string fieldName, SummaryItemType type) {
			Controller.SetSummary(fieldName, type, !Controller.HasSummary(type));
			Controller.Apply();
		}
	}
	public class GridPopupMenu : DataControlPopupMenu {
		public GridPopupMenu(GridViewBase view)
			: base(view) {
		}
		protected override MenuInfoBase CreateMenuInfoCore(GridMenuType? type) {
			switch(type) {
				case GridMenuType.Column:
					return new GridColumnMenuInfo(this);
				case GridMenuType.TotalSummary:
					return new GridTotalSummaryMenuInfo(this);
				case GridMenuType.RowCell:
					return new GridCellMenuInfo(this);
				case GridMenuType.GroupRow:
					return new GridGroupRowMenuInfo(this);
				case GridMenuType.GroupPanel:
					return new GridGroupPanelMenuInfo(this);
				case GridMenuType.FixedTotalSummary:
					return new GridTotalSummaryPanelMenuInfo(this);
				case GridMenuType.Band:
					return new GridBandMenuInfo(this);
				case GridMenuType.GroupFooterSummary:
					return new GridGroupFooterSummaryMenuInfo(this);
			}
			return null;
		}
	}
	#region Helpers
	public class ImageHelper {
		static Dictionary<string, BitmapImage> images = new Dictionary<string, BitmapImage>();
		static BitmapImage LoadImage(string imageName) {
			string resourcePath = string.Format("DevExpress.Xpf.Grid.Images.{0}.png", imageName);
			using(System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath)) {
				return DevExpress.Xpf.Core.Native.ImageHelper.CreateImageFromStream(stream);
			}
		}
		public static BitmapImage GetImage(string imageName) {
			BitmapImage image;
			if(!images.TryGetValue(imageName, out image)) {
				image = LoadImage(imageName);
				images.Add(imageName, image);
			}
			return image;
		}
	}
	#endregion
}
