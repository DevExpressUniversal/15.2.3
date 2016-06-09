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
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using System.Collections;
using DevExpress.Utils.Menu;
using DevExpress.XtraTreeList.Localization;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.StyleFormatConditions;
using DevExpress.XtraEditors.Filtering;
using DevExpress.Data.Filtering;												
using DevExpress.XtraEditors.Repository;
using System.Collections.Generic;
using DevExpress.XtraEditors.Frames;
using DevExpress.XtraTreeList.Extension;
namespace DevExpress.XtraTreeList.Menu {							  
	public enum TreeListMenuType { User, Summary, Column, Node };		 
	public class TreeListMenu : DXPopupMenu {					 
		private TreeList treeList;
		public TreeListMenu(TreeList treeList) {
			this.treeList = treeList;
			this.IsRightToLeft = this.treeList.IsRightToLeft;
		}
		public TreeList TreeList { get { return treeList; }}
		public override void Dispose() {
			base.Dispose();
		}
		public virtual void Init(object info) {
			CreateItems();
		}
		protected virtual void CreateItems() {
		}
		protected DXMenuItem CreateMenuItem(TreeListStringId id, Image image, bool enabled, bool beginGroup) {
			return CreateMenuItem(TreeListLocalizer.Active.GetLocalizedString(id), image, id, enabled, beginGroup);
		}
		protected DXMenuItem CreateMenuItem(TreeListStringId id, Image image, bool enabled) {
			return CreateMenuItem(id, image, enabled, false);
		}
		protected DXMenuItem CreateMenuItem(TreeListStringId id, Image image) {
			return CreateMenuItem(TreeListLocalizer.Active.GetLocalizedString(id), image, id, true);
		}
		protected DXMenuItem CreateMenuItem(string caption, Image image, object tag, bool enabled) {
			return CreateMenuItem(caption, image, tag, true, false);
		}
		protected virtual DXMenuItem CreateMenuItem(string caption, Image image, object tag, bool enabled, bool beginGroup) {
			DXMenuItem item = new DXMenuItem(caption, new EventHandler(OnMenuItemClick), image);
			item.Enabled = enabled;
			item.BeginGroup = beginGroup;
			item.Tag = tag;
			return item;
		}
		protected DXMenuItem CreateMenuCheckItem(TreeListStringId id, bool check, Image image) {
			return CreateMenuCheckItem(TreeListLocalizer.Active.GetLocalizedString(id), check, image, id, true);
		}
		protected DXMenuItem CreateMenuCheckItem(TreeListStringId id, bool check, Image image, bool enabled) {
			return CreateMenuCheckItem(TreeListLocalizer.Active.GetLocalizedString(id), check, image, id, enabled, false);
		}
		protected DXMenuItem CreateMenuCheckItem(TreeListStringId id, bool check, Image image, bool enabled, bool beginGroup) {
			return CreateMenuCheckItem(TreeListLocalizer.Active.GetLocalizedString(id), check, image, id, enabled, beginGroup);
		}
		protected DXMenuItem CreateMenuCheckItem(string caption, bool check, Image image, object tag, bool enabled) {
			return CreateMenuCheckItem(caption, check, image, tag, enabled, false);
		}
		protected virtual DXMenuItem CreateMenuCheckItem(string caption, bool check, Image image, object tag, bool enabled, bool beginGroup) {
			DXMenuCheckItem item = new DXMenuCheckItem(caption, check);
			item.Image = image;
			item.Enabled = enabled;
			item.Click += new EventHandler(OnMenuItemClick);
			item.CheckedChanged += new EventHandler(OnMenuItemCheckedChanged);
			item.BeginGroup = beginGroup;
			item.Tag = tag;
			return item;
		}
		protected virtual DXSubMenuItem CreateSubMenuItem(TreeListStringId id, Image image) {
			return CreateSubMenuItem(TreeListLocalizer.Active.GetLocalizedString(id), image, id, true, false);
		}
		protected virtual DXSubMenuItem CreateSubMenuItem(string caption, Image image, object tag, bool enabled, bool beginGroup) {
			DXSubMenuItem item = new DXSubMenuItem(caption, new EventHandler(OnMenuBeforePopup));
			item.Image = image;
			item.Enabled = enabled;
			item.BeginGroup = beginGroup;
			item.Tag = tag;
			return item;
		}
		public virtual TreeListMenuType MenuType { get { return TreeListMenuType.User; } }
		protected virtual bool RaiseClickEvent(object sender, EventArgs e) {
			TreeListMenuItemClickEventArgs ee = e as TreeListMenuItemClickEventArgs;
			if(ee == null) ee = new TreeListMenuItemClickEventArgs(null, false, SummaryItemType.Custom, "", MenuType, sender as DXMenuItem);
			treeList.InvokeMenuItemClick(ee);
			return ee.Handled;
		}
		protected virtual void OnMenuItemCheckedChanged(object sender, EventArgs e) {
		}
		protected virtual void OnMenuItemClick(object sender, EventArgs e) {
		}
		protected virtual void OnMenuBeforePopup(object sender, EventArgs e) { 
		}
		public virtual void Show(Point pos) {
			if(!TreeList.IsDesignMode && TreeList.MenuManager != null)
				TreeList.MenuManager.ShowPopupMenu(this, TreeList, pos);
			else
				MenuManagerHelper.ShowMenu(this, TreeList.LookAndFeel, null, TreeList, pos);
		}
	}
	public class TreeListColumnMenu : TreeListMenu {
		public TreeListColumnMenu(TreeList treeList) : base(treeList) { }
		TreeListColumn column = null;
		public override void Init(object info) {
			this.column = info as TreeListColumn;
			base.Init(info);
		}
		public override TreeListMenuType MenuType { get { return TreeListMenuType.Column; } }
		public TreeListColumn Column { get { return column; } }
		protected SortOrder ColumnSortOrder { get { return Column == null ? SortOrder.None : Column.SortOrder; } }
		protected override void CreateItems() {
			Items.Clear();
			if(Column != null) {
				Items.Add(CreateMenuCheckItem(TreeListStringId.MenuColumnSortAscending, ColumnSortOrder == SortOrder.Ascending, TreeListMenuImages.Column.Images[1], Column.OptionsColumn.AllowSort));
				Items.Add(CreateMenuCheckItem(TreeListStringId.MenuColumnSortDescending, ColumnSortOrder == SortOrder.Descending, TreeListMenuImages.Column.Images[2], Column.OptionsColumn.AllowSort));
				Items.Add(CreateMenuItem(TreeListStringId.MenuColumnClearSorting, null, Column.OptionsColumn.AllowSort && Column.SortOrder != SortOrder.None));
			}
			Items.Add(CreateMenuItem(GetMenuColumnCustomizationStringId(), TreeListMenuImages.Column.Images[5], true, Column != null));
			if(Column != null) 
				Items.Add(CreateMenuItem(TreeListStringId.MenuColumnBestFit, TreeListMenuImages.Column.Images[6], TreeList.CanResizeColumn(Column)));
			Items.Add(CreateMenuItem(TreeListStringId.MenuColumnBestFitAllColumns, null, true, true));
			if(TreeList.OptionsBehavior.EnableFiltering) {
				bool beginGroup = true;
				if(Column != null) {
					if(IsColumnFiltered(column)) {
						Items.Add(CreateMenuItem(TreeListStringId.MenuColumnClearFilter, TreeListMenuImages.Column.Images[7], true, beginGroup));
						beginGroup = false;
					}
				}
				if(AllowColumnFilterEditor(Column)) {
					Items.Add(CreateMenuItem(TreeListStringId.MenuColumnFilterEditor, TreeListMenuImages.Column.Images[8], true, beginGroup));
					beginGroup = false;
				}
				if(!TreeList.OptionsFind.AlwaysVisible && TreeList.OptionsFind.AllowFindPanel) {
					Items.Add(CreateMenuItem(TreeList.FindPanelVisible ? TreeListStringId.MenuColumnFindFilterHide : TreeListStringId.MenuColumnFindFilterShow, null));
					beginGroup = false;
				}
				if(TreeList.OptionsMenu.ShowAutoFilterRowItem) {
					Items.Add(CreateMenuItem(TreeList.OptionsView.ShowAutoFilterRow ? TreeListStringId.MenuColumnAutoFilterRowHide : TreeListStringId.MenuColumnAutoFilterRowShow, null, true, beginGroup));
					beginGroup = false;
				}
			}
			if(Column != null && Column.ShowUnboundExpressionMenu && !TreeList.IsDesignMode)
				Items.Add(CreateMenuItem(TreeListStringId.MenuColumnExpressionEditor, TreeListMenuImages.Column.Images[12], true, true));
			if(TreeList.OptionsMenu.ShowConditionalFormattingItem && Column != null && CanShowConditionalFormattingItem(Column) && !TreeList.IsDesignMode) {
				DXSubMenuItem conditionalFormattingItem = CreateSubMenuItem(TreeListStringId.MenuColumnConditionalFormatting, TreeListMenuImages.Column.Images[15]);
				conditionalFormattingItem.BeginGroup = true;
				TreeListFormatRuleMenuItems formatRuleItems = new TreeListFormatRuleMenuItems(TreeList, Column, conditionalFormattingItem.Items);
				if(formatRuleItems.Count > 0)
					Items.Add((DXMenuItem)conditionalFormattingItem);
			}
		}
		protected virtual bool CanShowConditionalFormattingItem(TreeListColumn column) {
			RepositoryItem item = column.GetActualColumnEdit();
			if(item is RepositoryItemBaseProgressBar || item is RepositoryItemPictureEdit || item is RepositoryItemImageEdit || item is RepositoryItemRangeTrackBar
				|| item is RepositoryItemBreadCrumbEdit || item is RepositoryItemTokenEdit || item is RepositoryItemSparklineEdit)
				return false;
			return true;
		}
		protected virtual TreeListStringId GetMenuColumnCustomizationStringId() {
			if(TreeList.ActualShowBands)
				return TreeListStringId.MenuColumnBandCustomization;
			return TreeListStringId.MenuColumnColumnCustomization;
		}
		bool IsColumnFiltered(TreeListColumn column) {
			if(!column.OptionsFilter.AllowFilter) return false;
			return !column.FilterInfo.IsEmpty;
		}
		bool AllowColumnFilterEditor(TreeListColumn column) { 
			if(column == null) return TreeList.OptionsFilter.AllowFilterEditor;
			return column.OptionsFilter.AllowFilter && TreeList.OptionsFilter.AllowFilterEditor;
		}
		protected override void OnMenuItemClick(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			if(RaiseClickEvent(sender, new TreeListMenuItemClickEventArgs(Column, false, SummaryItemType.Custom, "", MenuType, item))) return;
			if(!(item.Tag is TreeListStringId)) return;
			TreeListStringId id = (TreeListStringId)item.Tag;
			switch(id) {
				case TreeListStringId.MenuColumnSortAscending:
					if(Column != null) Column.SortOrder = SortOrder.Ascending;
					break;
				case TreeListStringId.MenuColumnSortDescending:
					if(Column != null) Column.SortOrder = SortOrder.Descending;
					break;
				case TreeListStringId.MenuColumnClearSorting:
					Column.SortOrder = SortOrder.None;
					break;
				case TreeListStringId.MenuColumnColumnCustomization:
				case TreeListStringId.MenuColumnBandCustomization:
					TreeList.ColumnsCustomization();
					break;
				case TreeListStringId.MenuColumnBestFit:
					if(Column != null) Column.BestFit();
					break;
				case TreeListStringId.MenuColumnBestFitAllColumns:
					TreeList.BestFitColumns();
					break;
				case TreeListStringId.MenuColumnAutoFilterRowHide:
				case TreeListStringId.MenuColumnAutoFilterRowShow:
					TreeList.OptionsView.ShowAutoFilterRow = !TreeList.OptionsView.ShowAutoFilterRow;
					break;
				case TreeListStringId.MenuColumnClearFilter:
					if(Column != null) TreeList.ClearColumnFilter(Column);
					break;
				case TreeListStringId.MenuColumnFilterEditor:
					TreeList.ShowFilterEditor(Column);
					break;
				case TreeListStringId.MenuColumnFindFilterShow:
					TreeList.ShowFindPanel();
					break;
				case TreeListStringId.MenuColumnFindFilterHide:
					TreeList.HideFindPanel();
					break;
				case TreeListStringId.MenuColumnExpressionEditor:
					TreeList.ShowUnboundExpressionEditor(Column);
					break;
			}
		}
	}
	public class TreeListBandMenu : TreeListColumnMenu {
		TreeListBand band;
		public TreeListBandMenu(TreeList treeList) : base(treeList) { }
		public override void Init(object info) {
			this.band = info as TreeListBand;
			base.Init(info);
		}
		public TreeListBand Band { get { return band; } }
	}
	public class TreeListFooterMenu : TreeListMenu {
		public TreeListFooterMenu(TreeList treeList) : base(treeList) { }
		TreeListColumn column = null;
		bool isFooter = false;
		public override void Init(object info) {
			TreeListHitTest hitTest = info as TreeListHitTest;
			this.column = hitTest.FooterItem.Column;
			isFooter = hitTest.HitInfoType == HitInfoType.SummaryFooter;
			base.Init(info);
		}
		public override TreeListMenuType MenuType { get { return TreeListMenuType.Summary; } }
		protected bool IsSumAvgAvailable {
			get {
				if(Column == null) return false;
				Type type = Column.ColumnType;
				if(type.Equals(typeof(DateTime))) return false;
				return type.IsValueType;
			}
		}
		public TreeListColumn Column { get { return column; } }
		protected override void CreateItems() {
			Items.Clear();
			Items.Add(CreateMenuCheckItem(TreeListStringId.MenuFooterSum, SummaryType == SummaryItemType.Sum, TreeListMenuImages.Footer.Images[0], IsSumAvgAvailable));
			Items.Add(CreateMenuCheckItem(TreeListStringId.MenuFooterMin, SummaryType == SummaryItemType.Min, TreeListMenuImages.Footer.Images[1]));
			Items.Add(CreateMenuCheckItem(TreeListStringId.MenuFooterMax, SummaryType == SummaryItemType.Max, TreeListMenuImages.Footer.Images[2]));
			Items.Add(CreateMenuCheckItem(TreeListStringId.MenuFooterCount, SummaryType == SummaryItemType.Count, TreeListMenuImages.Footer.Images[3]));
			Items.Add(CreateMenuCheckItem(TreeListStringId.MenuFooterAverage, SummaryType == SummaryItemType.Average, TreeListMenuImages.Footer.Images[4], IsSumAvgAvailable));
			Items.Add(CreateMenuCheckItem(TreeListStringId.MenuFooterNone, SummaryType == SummaryItemType.None, null, true, true));
			if(isFooter)
				Items.Add(CreateMenuCheckItem(TreeListStringId.MenuFooterAllNodes, Column.AllNodesSummary, TreeListMenuImages.Footer.Images[6], true, true));
		}
		public SummaryItemType SummaryType { get { return Column == null ? SummaryItemType.None : (isFooter ? Column.SummaryFooter : Column.RowFooterSummary); } }
		protected bool GetSummaryType(DXMenuItem item, ref SummaryItemType summaryType) {
			if(!(item.Tag is TreeListStringId) || Column == null) return false;
			TreeListStringId id = (TreeListStringId)item.Tag;
			summaryType = SummaryItemType.None;
			switch(id) {
				case TreeListStringId.MenuFooterSum : summaryType = SummaryItemType.Sum; break; 
				case TreeListStringId.MenuFooterMin : summaryType = SummaryItemType.Min; break; 
				case TreeListStringId.MenuFooterMax : summaryType = SummaryItemType.Max; break; 
				case TreeListStringId.MenuFooterCount : summaryType = SummaryItemType.Count; break; 
				case TreeListStringId.MenuFooterAverage : summaryType = SummaryItemType.Average; break; 
				case TreeListStringId.MenuFooterNone : summaryType = SummaryItemType.None; break; 
				default: 
					return false;
			}
			return true;
		}
		protected string GetFormat(SummaryItemType sumType) {
			string res = "";
			switch(sumType) {
				case SummaryItemType.Sum : res = TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.MenuFooterSumFormat); break; 
				case SummaryItemType.Min : res = TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.MenuFooterMinFormat); break; 
				case SummaryItemType.Max : res = TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.MenuFooterMaxFormat); break; 
				case SummaryItemType.Count : res = TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.MenuFooterCountFormat); break; 
				case SummaryItemType.Average : res = TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.MenuFooterAverageFormat); break; 
			}
			return res;
		}
		protected override void OnMenuItemClick(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			SummaryItemType summaryType = SummaryItemType.None;
			bool validItem = GetSummaryType(item, ref summaryType);
			TreeListMenuItemClickEventArgs ee = new TreeListMenuItemClickEventArgs(Column, isFooter, summaryType, GetFormat(summaryType), MenuType, item); 
			if(RaiseClickEvent(sender, ee)) return;
			if(validItem) { 
				if(isFooter) {
					if(Column.SummaryFooter != ee.SummaryType || Column.SummaryFooterStrFormat != ee.SummaryFormat) {
						Column.SummaryFooter = ee.SummaryType;
						Column.SummaryFooterStrFormat = ee.SummaryFormat;
					}
				} else {
					if(Column.RowFooterSummary != ee.SummaryType || Column.RowFooterSummaryStrFormat != ee.SummaryFormat) {
						Column.RowFooterSummary = ee.SummaryType;
						Column.RowFooterSummaryStrFormat = ee.SummaryFormat;
					}
				}
			} else {
				Column.AllNodesSummary = !Column.AllNodesSummary;
			}
		}
	}
	public class TreeListNodeMenu : TreeListMenu {
		TreeListNode node;
		public TreeListNodeMenu(TreeList treeList)
			: base(treeList) {
		}
		public TreeListNode Node { get { return node; } }
		public override void Init(object info) {
			base.Init(info);
			this.node = info as TreeListNode;
		}
		public override TreeListMenuType MenuType { get { return TreeListMenuType.Node; } }
	}
	public class TreeListMenuImages {
		[ThreadStatic]
		static ImageCollection column = null;
		public static ImageCollection Column {
			get {
				if(column == null) column = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraTreeList.Images.GridColumnMenu.png", typeof(TreeListMenuImages).Assembly, new Size(16, 16));
				return column;
			}
		}
		[ThreadStatic]
		static ImageCollection footer = null;
		public static ImageCollection Footer {
			get {
				if(footer == null) footer = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraTreeList.Images.GridFooterMenu.png", typeof(TreeListMenuImages).Assembly, new Size(16, 16));
				return footer;							  
			}
		}
	}
	public class TreeListFormatRuleMenuItems : FormatRuleMenuItems {
		public TreeListFormatRuleMenuItems(TreeList treeList, TreeListColumn column, DXMenuItemCollection collection) :
			base(TreeListFormatRuleMenuItems.CreateFormatRuleColumnInfo(column), collection) {
				Column = column;
				TreeList = treeList;
				UpdateItems();
		}
		protected TreeList TreeList { get; private set; }
		protected TreeListColumn Column { get; private set; }
		protected override bool IsRuleColumnExists {
			get {
				foreach(TreeListFormatRule rule in TreeList.FormatRules)
					if(Column == rule.Column) return true;
				return false;
			}
		}
		protected override bool IsRuleExists { get { return TreeList.FormatRules.Count > 0; } }
		protected override void ManageColumnRulesItemClick(object sender, EventArgs e) {		
			if(Column == null) return;
			Column.ShowFormatRulesManager();
		}
		protected override void ClearColumnRulesItemClick(object sender, EventArgs e) {
			ClearColumnRules(Column);
		}
		protected override void ClearAllRulesItemClick(object sender, EventArgs e) {
			ClearColumnRules(null);
		}
		protected virtual void ClearColumnRules(TreeListColumn column) {
			RemoveRules(column, null);
		}
		protected virtual void RemoveRules(TreeListColumn column, Type ruleType, FormatConditionRuleBase ruleBase = null) {
			TreeList.BeginUpdate();
			try {
				for(int i = TreeList.FormatRules.Count - 1; i >= 0; i--) {
					TreeListFormatRule rule = TreeList.FormatRules[i];
					if(CanRemoveRule(rule, column, ruleType)) {
						if(ruleBase != null)
							ruleBase.Assign(rule.Rule);
						TreeList.FormatRules.RemoveAt(i);
					}
				}
			}
			finally {
				TreeList.EndUpdate();
			}
		}
		protected virtual bool CanRemoveRule(TreeListFormatRule rule, TreeListColumn column, Type ruleType) {
			if(rule == null) return false;
			if(ruleType == null && column == null) return true;
			if(ruleType == null || rule.Rule == null) return column == rule.Column;
			if(column == null) return rule.Rule.GetType() == ruleType;
			return rule.Rule.GetType() == ruleType && column == rule.Column;
		}
		protected override void UpdateFormatConditionRules(FormatConditionRuleAppearanceBase rule, bool applyToRow) {
			TreeList.FormatRules.Add(Column, rule).ApplyToRow = applyToRow;
		}
		protected override void DataBarItemClick(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			FormatConditionRuleDataBar rule = new FormatConditionRuleDataBar();
			RemoveRules(Column, typeof(FormatConditionRuleDataBar), rule);
			rule.Appearance.Reset();
			rule.AppearanceNegative.Reset();
			rule.PredefinedName = string.Format("{0}", item.Tag);
			TreeList.FormatRules.Add(Column, rule);
		}
		protected override void IconSetItemClick(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			RemoveRules(Column, typeof(FormatConditionRuleIconSet), null);
			FormatConditionRuleIconSet rule = new FormatConditionRuleIconSet();
			rule.IconSet = item.Tag as FormatConditionIconSet;
			TreeList.FormatRules.Add(Column, rule);
		}
		protected override void ColorScaleItemClick(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			RemoveRules(Column, typeof(FormatConditionRule2ColorScale), null);
			RemoveRules(Column, typeof(FormatConditionRule3ColorScale), null);
			TreeList.FormatRules.Add(Column, CreateColorScaleRule(string.Format("{0}", item.Tag)));
		}
		FormatConditionRule2ColorScale CreateColorScaleRule(string key) {
			FormatConditionRule2ColorScale rule;
			if(key.Split(',').Length > 2)
				rule = new FormatConditionRule3ColorScale();
			else rule = new FormatConditionRule2ColorScale();
			rule.PredefinedName = key;
			return rule;
		}
		protected override FilterColumn GetDefaultFilterColumn(FilterColumnCollection fcCollection) {
			return TreeList.GetFilterColumnByTreeListColumn(fcCollection, Column);
		}
		protected override FilterColumnCollection GetFilterColumns() {
			return TreeList.CreateFilterColumnCollection();
		}
		protected override bool UseEditFormForCustomCondition { get { return false; } }
		protected static FormatRuleColumnInfo CreateFormatRuleColumnInfo(TreeListColumn column) {
			return new FormatRuleColumnInfo() { Name = column.FieldName, ColumnType = column.ColumnType, RepositoryItem = column.GetActualColumnEdit(), OwnerControl = column.TreeList, MenuManager = column.TreeList.MenuManager };
		}
	}
}
namespace DevExpress.XtraTreeList.Extension {
	public static class FormatRulesManagerExtension {
		public static void ShowFormatRulesManager(this TreeListColumn column) {
			if(column == null) return;
			var treeList = column.TreeList;
			if(treeList == null) return;
			var filterColumns = treeList.CreateFilterColumnCollection();
			var filterColumnDefault = treeList.GetFilterColumnByTreeListColumn(filterColumns, column);
			using(ManagerRuleForm<TreeListFormatRule, TreeListColumn> manager =
				new ManagerRuleForm<TreeListFormatRule, TreeListColumn>(
								treeList.FormatRules,
								treeList.Columns,
								filterColumns,
								filterColumnDefault,
								column.FieldName,
								treeList.MenuManager,
								GetNameColumns(treeList))) {
				manager.ShowDialog();
			}
		}
		static List<ColumnNameInfo> GetNameColumns(TreeList treeList) {
			var nameColumns = new List<ColumnNameInfo>();
			foreach(TreeListColumn item in treeList.Columns) 
				nameColumns.Add(new ColumnNameInfo() {
									Key = item.FieldName,
									Value = item.GetCaption(),
									Name = item.Name,
									Visible = item.Visible && item.OptionsColumn.ShowInCustomizationForm
								});
			return nameColumns;
}
	}
}
