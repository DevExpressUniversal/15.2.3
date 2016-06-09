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
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.Data;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using System.Collections;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraExport;
using DevExpress.XtraGrid.Export;
using DevExpress.XtraGrid.FilterEditor;
using DevExpress.XtraEditors.Repository;
using DevExpress.Data.Helpers;
using System.Collections.Generic;
using DevExpress.XtraGrid.GroupSummaryEditor;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.Data.Summary;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Filtering; 
using DevExpress.Data.Filtering;					 
using DevExpress.XtraEditors.Frames;
using DevExpress.XtraGrid.Extension;
namespace DevExpress.XtraGrid.Menu {
	public class ViewMenu : DXPopupMenu {
		BaseView view;
		public ViewMenu(BaseView view) {  
			this.view = view;
			IsRightToLeft = View.IsRightToLeft;
		}
		public override void Dispose() {
			this.view = null;
			base.Dispose();
		}
		public virtual void Init(object info) {
			IsRightToLeft = View.IsRightToLeft;
			CreateItems();
		}
		protected virtual BaseView ViewCore { get { return view; } }
		public BaseView View { get { return ViewCore; } }
		protected virtual void CreateItems() {
		}
		protected DXMenuItem CreateMenuItem(GridStringId id, Image image, bool enabled, bool beginGroup) {
			return CreateMenuItem(id, image, (Image)null, enabled, beginGroup);
		}
		protected DXMenuItem CreateMenuItem(GridStringId id, Image image, Image largeImage, bool enabled, bool beginGroup) {
			return CreateMenuItem(GridLocalizer.Active.GetLocalizedString(id), image, largeImage, id, enabled, beginGroup);
		}
		protected DXMenuItem CreateMenuItem(GridStringId id, Image image, bool enabled) {
			return CreateMenuItem(id, image, (Image)null, enabled);
		}
		protected DXMenuItem CreateMenuItem(GridStringId id, Image image, Image largeImage, bool enabled) {
			return CreateMenuItem(id, image, largeImage, enabled, false);
		}
		protected DXMenuItem CreateMenuItem(GridStringId id, Image image) {
			return CreateMenuItem(id, image, (Image)null);
		}
		protected DXMenuItem CreateMenuItem(GridStringId id, Image image, Image largeImage) {
			return CreateMenuItem(GridLocalizer.Active.GetLocalizedString(id), image, largeImage, id, true);
		}
		protected DXMenuItem CreateMenuItem(string caption, Image image, object tag, bool enabled) {
			return CreateMenuItem(caption, image, (Image)null, tag, enabled);
		}
		protected DXMenuItem CreateMenuItem(string caption, Image image, Image largeImage, object tag, bool enabled) {
			return CreateMenuItem(caption, image, largeImage, tag, enabled, false);
		}
		protected virtual DXMenuItem CreateMenuItem(string caption, Image image, object tag, bool enabled, bool beginGroup) {
			return CreateMenuItem(caption, image, null, tag, enabled, beginGroup);
		}
		protected virtual DXMenuItem CreateMenuItem(string caption, Image image, Image largeImage, object tag, bool enabled, bool beginGroup) {
			DXMenuItem item = new DXMenuItem(caption, new EventHandler(OnMenuItemClick), image);
			item.LargeImage = largeImage;
			item.Enabled = enabled;
			item.BeginGroup = beginGroup;
			item.Tag = tag;
			return item;
		}
		protected virtual DXSubMenuItem CreateSubMenuItem(GridStringId id, Image image) {
			return CreateSubMenuItem(id, image, (Image)null);
		}
		protected virtual DXSubMenuItem CreateSubMenuItem(GridStringId id, Image image, Image largeImage) {
			return CreateSubMenuItem(GridLocalizer.Active.GetLocalizedString(id), image, largeImage, id, true, false);
		}
		protected virtual DXSubMenuItem CreateSubMenuItem(string caption, Image image, object tag, bool enabled, bool beginGroup) {
			return CreateSubMenuItem(caption, image, (Image)null, tag, enabled, beginGroup);
		}
		protected virtual DXSubMenuItem CreateSubMenuItem(string caption, Image image, Image largeImage, object tag, bool enabled, bool beginGroup) {
			DXSubMenuItem item = new DXSubMenuItem(caption, new EventHandler(OnBeforePopup));
			item.Image = image;
			item.LargeImage = largeImage;
			item.Enabled = enabled;
			item.BeginGroup = beginGroup;
			item.Tag = tag;
			return item;
		}
		protected DXMenuItem CreateMenuCheckItem(GridStringId id, bool check, Image image) {
			return CreateMenuCheckItem(GridLocalizer.Active.GetLocalizedString(id), check, image, id, true);
		}
		protected DXMenuItem CreateMenuCheckItem(GridStringId id, bool check, Image image, bool enabled) {
			return CreateMenuCheckItem(id, check, image, (Image)null, enabled);
		}
		protected DXMenuItem CreateMenuCheckItem(GridStringId id, bool check, Image image, Image largeImage, bool enabled) {
			return CreateMenuCheckItem(GridLocalizer.Active.GetLocalizedString(id), check, image, largeImage, id, enabled, false);
		}
		protected DXMenuItem CreateMenuCheckItem(GridStringId id, bool check, Image image, bool enabled, bool beginGroup) {
			return CreateMenuCheckItem(id, check, image, (Image)null, enabled, beginGroup);
		}
		protected DXMenuItem CreateMenuCheckItem(GridStringId id, bool check, Image image, Image largeImage, bool enabled, bool beginGroup) {
			return CreateMenuCheckItem(GridLocalizer.Active.GetLocalizedString(id), check, image, largeImage, id, enabled, beginGroup);
		}
		protected DXMenuItem CreateMenuCheckItem(string caption, bool check, Image image, object tag, bool enabled) {
			return CreateMenuCheckItem(caption, check, image, tag, enabled, false);
		}
		protected virtual DXMenuItem CreateMenuCheckItem(string caption, bool check, Image image, object tag, bool enabled, bool beginGroup) {
			return CreateMenuCheckItem(caption, check, image, (Image)null, tag, enabled, beginGroup);
		}
		protected virtual DXMenuItem CreateMenuCheckItem(string caption, bool check, Image image, Image largeImage, object tag, bool enabled, bool beginGroup) {
			DXMenuCheckItem item = new DXMenuCheckItem(caption, check);
			item.Image = image;
			item.LargeImage = largeImage;
			item.Enabled = enabled;
			item.Click += new EventHandler(OnMenuItemClick);
			item.CheckedChanged += new EventHandler(OnMenuItemCheckedChanged);
			item.BeginGroup = beginGroup;
			item.Tag = tag;
			return item;
		}
		protected string GetHideShowCaptionForGroupPanel(bool p) {
			return string.Format("{0}", p ?
				GridLocalizer.Active.GetLocalizedString(GridStringId.MenuGroupPanelHide) :
				GridLocalizer.Active.GetLocalizedString(GridStringId.MenuGroupPanelShow));
		}
		protected virtual bool RaiseClickEvent(object sender, EventArgs e) {
			return false;
		}
		protected virtual void OnMenuItemCheckedChanged(object sender, EventArgs e) { }
		protected virtual void OnMenuItemClick(object sender, EventArgs e) { }
		protected virtual void OnBeforePopup(object sender, EventArgs e) { }
		public virtual void Show(Point pos) {
			if(View == null || View.GridControl == null) return;
			if(!View.IsDesignMode && View.GridControl != null && View.GridControl.MenuManager != null)
				View.GridControl.MenuManager.ShowPopupMenu(this, View.GridControl, pos);
			else
				MenuManagerHelper.ShowMenu(this, View.ElementsLookAndFeel, null, View.GridControl, pos);
		}
	}
	public class GridViewMenu : ViewMenu {
		public GridViewMenu(GridView view) : base(view) { }
		public new GridView View { get { return base.View as GridView; } }
		public virtual GridMenuType MenuType { get { return GridMenuType.User; } }
		protected override bool RaiseClickEvent(object sender, EventArgs e) {
			if(View == null) return false;
			GridMenuItemClickEventArgs ee = e as GridMenuItemClickEventArgs;
			if(ee == null) ee = new GridMenuItemClickEventArgs(null, null, null, SummaryItemType.Custom, string.Empty, MenuType, sender as DXMenuItem);
			View.InvokeMenuItemClick(ee);
			return ee.Handled;
		}
	}
	public class GridViewFooterMenu : GridViewMenu {
		public GridViewFooterMenu(GridView view) : base(view) { }
		GridColumn column = null;
		GridSummaryItem summaryItem = null;
		bool isGroupSummary = false;
		bool allowMultiSummary = false;
		DXSubMenuItem addSummaryItem = null;
		public override void Init(object info) {
			GridHitInfo hitInfo = info as GridHitInfo;
			this.column = hitInfo.Column;
			if(hitInfo.HitTest == GridHitTest.Footer) {
				allowMultiSummary = true;
				if(hitInfo.FooterCell != null && hitInfo.FooterCell.SummaryItem != null)
					SummaryItem = hitInfo.FooterCell.SummaryItem;
			}
			if(hitInfo.HitTest == GridHitTest.RowFooter) {
				this.isGroupSummary = true;
				SummaryItem = FindSummary(View.GroupSummary);
			}
			if(hitInfo.HitTest == GridHitTest.RowGroupCell) {
				this.isGroupSummary = true;
				SummaryItem = FindSummary(View.GroupSummary);
			}
			base.Init(info);
		}
		public override GridMenuType MenuType { get { return GridMenuType.Summary; } }
		RepositoryItem ColumnEditor {
			get {
				if(Column == null) return null;
				return Column.RealColumnEdit;
			}
		}
		Type GetColumnType() {
			if(Column == null || ColumnEditor == null) return null;
				Type type = Column.ColumnType;
				Type underlyingType = Nullable.GetUnderlyingType(type);
				if(underlyingType != null)
					type = underlyingType;
			if(IsLookUpEditor) return null;
			return type;
			}
		bool IsSummaryTypeAvailable(SummaryItemType sumType) {
			Type type = GetColumnType();
			if(type == null) return false;
			return SummaryItemTypeHelper.CanApplySummary(sumType, type);
		}
		bool IsLookUpEditor {
			get {
				return (ColumnEditor is RepositoryItemImageComboBox ||
					ColumnEditor is RepositoryItemLookUpEdit ||
					ColumnEditor is RepositoryItemGridLookUpEdit);
			}
		}
		protected GridGroupSummaryItem FindSummary(GridSummaryItemCollection collection) {
			GridGroupSummaryItem res = null;
			foreach(GridGroupSummaryItem item in collection) {
				if(item.ShowInGroupColumnFooter == Column) {
					if(item.SummaryType != SummaryItemType.None || res == null) res = item; 
				}
			}
			return res;
		}
		public GridColumn Column { get { return column; } }
		public IFormatProvider Format {
			get {
				if(Column == null || Column.DisplayFormat == null) return null;
				return Column.DisplayFormat.Format;
			}
		}
		public GridSummaryItem SummaryItem { 
			get { return summaryItem; }
			protected set { summaryItem = value; }
		}
		bool IsEmpty { 
			get {
				if(Column == null || !Column.AllowSummaryMenu) return true;
				if(SummaryItem == null) return false;
				return SummaryItem.FieldName == string.Empty; 
			} 
		}
		public bool IsGroupSummary { get { return isGroupSummary; } }
		public SummaryItemType SummaryType { get { return SummaryItem == null ? SummaryItemType.None : SummaryItem.SummaryType; } }
		static SummaryItemType[] Summary = new SummaryItemType[] {
																	 SummaryItemType.Sum, SummaryItemType.Min, SummaryItemType.Max, SummaryItemType.Count, 
																	 SummaryItemType.Average, SummaryItemType.Custom, SummaryItemType.None};
		protected virtual bool AllowAddNewSummaryItem { 
			get { 
				return AllowMultiSummary && AllowNewSummaryType(SummaryItemType.None) && View.AllowAddNewSummaryItemMenu && Column.Summary.Count != 0; 
			} 
		}
		bool AllowClearSummaryItems { get { return AllowMultiSummary && View.AllowAddNewSummaryItemMenu && Column.Summary.Count > 1; } }
		protected virtual bool AllowMultiSummary { 
			get { return allowMultiSummary; } 
		}
		protected virtual bool AllowSummary { get { return IsSummaryTypeAvailable(SummaryItemType.Sum) && !IsEmpty; } }
		protected virtual bool AllowMinMax { get { return IsSummaryTypeAvailable(SummaryItemType.Min) && !IsEmpty; } }
		protected DXSubMenuItem AddSummaryItem {
			get { return addSummaryItem; }
			set { addSummaryItem = value; }
		}
		bool IsTypeExist(SummaryItemType type) {
			foreach(GridColumnSummaryItem item in column.Summary)
				if(item.SummaryType == type) return true;
			return false;
		}
		bool AllowSummaryItem(SummaryItemType type) { 
			if(IsTypeExist(type)) return false;
			switch(type) {
				case SummaryItemType.Average: 
				case SummaryItemType.Sum: return AllowSummary;
				case SummaryItemType.Max:
				case SummaryItemType.Min: return AllowMinMax;
			}
			return true;
		}
		protected bool AllowNewSummaryType(SummaryItemType currentType) {
			SummaryItemType[] types = new SummaryItemType[] { SummaryItemType.Count, SummaryItemType.Sum, SummaryItemType.Average, SummaryItemType.Max, SummaryItemType.Min };
			foreach(SummaryItemType type in types)
				if(type != currentType && AllowSummaryItem(type)) return true;
			return false;
		}
		protected virtual bool ItemsEnabled {
			get {
				return SummaryType != SummaryItemType.Custom;
			}
		}
		protected override void CreateItems() {
			Items.Clear();
			if(View.IsCheckboxSelector(Column)) return;
			if(SummaryItem == null && !AllowNewSummaryType(SummaryItemType.None) && !IsGroupSummary) return;
			bool isEnabled = ItemsEnabled;
			if(isEnabled && AllowAddNewSummaryItem) {
				AddSummaryItem = CreateSubMenuItem(GridStringId.MenuFooterAddSummaryItem, null);
				Items.Add(AddSummaryItem);
				if(AllowSummaryItem(SummaryItemType.Sum)) AddSummaryItem.Items.Add(CreateMenuItem(GridStringId.MenuFooterSum, GridMenuImages.Footer.Images[0], GridMenuImages.FooterLarge.Images[0]));
				if(AllowSummaryItem(SummaryItemType.Min)) AddSummaryItem.Items.Add(CreateMenuItem(GridStringId.MenuFooterMin, GridMenuImages.Footer.Images[1], GridMenuImages.FooterLarge.Images[1]));
				if(AllowSummaryItem(SummaryItemType.Max)) AddSummaryItem.Items.Add(CreateMenuItem(GridStringId.MenuFooterMax, GridMenuImages.Footer.Images[2], GridMenuImages.FooterLarge.Images[2]));
				if(AllowSummaryItem(SummaryItemType.Count)) AddSummaryItem.Items.Add(CreateMenuItem(GridStringId.MenuFooterCount, GridMenuImages.Footer.Images[3], GridMenuImages.FooterLarge.Images[3]));
				if(AllowSummaryItem(SummaryItemType.Average)) AddSummaryItem.Items.Add(CreateMenuItem(GridStringId.MenuFooterAverage, GridMenuImages.Footer.Images[4], GridMenuImages.FooterLarge.Images[4]));
			}
			Items.Add(CreateMenuCheckItem(GridStringId.MenuFooterSum, SummaryType == SummaryItemType.Sum, GridMenuImages.Footer.Images[0], GridMenuImages.FooterLarge.Images[0], AllowSummary && isEnabled, isEnabled && AllowAddNewSummaryItem));
			Items.Add(CreateMenuCheckItem(GridStringId.MenuFooterMin, SummaryType == SummaryItemType.Min, GridMenuImages.Footer.Images[1], GridMenuImages.FooterLarge.Images[1], AllowMinMax && isEnabled));
			Items.Add(CreateMenuCheckItem(GridStringId.MenuFooterMax, SummaryType == SummaryItemType.Max, GridMenuImages.Footer.Images[2], GridMenuImages.FooterLarge.Images[2], AllowMinMax && isEnabled));
			Items.Add(CreateMenuCheckItem(GridStringId.MenuFooterCount, SummaryType == SummaryItemType.Count, GridMenuImages.Footer.Images[3], GridMenuImages.FooterLarge.Images[3], isEnabled));
			Items.Add(CreateMenuCheckItem(GridStringId.MenuFooterAverage, SummaryType == SummaryItemType.Average, GridMenuImages.Footer.Images[4], GridMenuImages.FooterLarge.Images[4], AllowSummary && isEnabled));
			Items.Add(CreateMenuCheckItem(GridStringId.MenuFooterNone, SummaryType == SummaryItemType.None, null, isEnabled, true));
			if(isEnabled && AllowClearSummaryItems)
				Items.Add(CreateMenuItem(GridStringId.MenuFooterClearSummaryItems, null, true, true));
		}
		protected bool GetSummaryType(DXMenuItem item, ref SummaryItemType summaryType) {
			if(!(item.Tag is GridStringId) || Column == null) return false;
			GridStringId id = (GridStringId)item.Tag;
			summaryType = SummaryItemType.None;
			switch(id) {
				case GridStringId.MenuFooterSum : summaryType = SummaryItemType.Sum; break; 
				case GridStringId.MenuFooterMin : summaryType = SummaryItemType.Min; break; 
				case GridStringId.MenuFooterMax : summaryType = SummaryItemType.Max; break; 
				case GridStringId.MenuFooterCount : summaryType = SummaryItemType.Count; break; 
				case GridStringId.MenuFooterAverage : summaryType = SummaryItemType.Average; break; 
				case GridStringId.MenuFooterNone : summaryType = SummaryItemType.None; break; 
				default: 
					return false;
			}
			return true;
		}
		protected IFormatProvider GetSummaryFormatProvider(SummaryItemType summaryType) {
			if(summaryType == SummaryItemType.Count || summaryType == SummaryItemType.None) return null;
			return Format;
		}
		protected override void OnMenuItemClick(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			SummaryItemType summaryType = SummaryItemType.None;
			bool validItem = GetSummaryType(item, ref summaryType);
			GridMenuItemClickEventArgs ee = new GridMenuItemClickEventArgs(Column, IsGroupSummary ? View.GroupSummary : null, SummaryItem, summaryType, View.GetSummaryFormat(Column, summaryType), MenuType, item); 
			if(RaiseClickEvent(sender, ee)) return;
			UpdateSummaryFormat(ee);
			if(validItem) {
				if(IsGroupSummary) {
					if(SummaryType != ee.SummaryType || (SummaryItem != null && SummaryItem.DisplayFormat != ee.SummaryFormat)) {
						if(SummaryItem != null) SummaryItem.SetSummary(ee.SummaryType, ee.SummaryFormat, GetSummaryFormatProvider(ee.SummaryType));
						else
							View.GroupSummary.Add(ee.SummaryType, ee.Column.FieldName, ee.Column, ee.SummaryFormat, GetSummaryFormatProvider(ee.SummaryType));
					}
				}
				else {
					if(AddSummaryItem != null && item.Collection == AddSummaryItem.Items) {
						SummaryItem = Column.Summary.Add(ee.SummaryType, "", ee.SummaryFormat, GetSummaryFormatProvider(ee.SummaryType));
					}
					if(SummaryItem == null) {
						SummaryItem = Column.Summary.FindEmptyOrCreate(ee.SummaryType, "", ee.SummaryFormat, GetSummaryFormatProvider(ee.SummaryType));
					}
					if(SummaryItem.SummaryType != ee.SummaryType || SummaryItem.DisplayFormat != ee.SummaryFormat) {
						SummaryItem.SetSummary(ee.SummaryType, ee.SummaryFormat, GetSummaryFormatProvider(ee.SummaryType));
					}
					ClearEmptySummaryItems(column);
				}
			}
			else {
				if(GridStringId.MenuFooterClearSummaryItems.Equals(item.Tag)) Column.Summary.Clear();
			}
		}
		protected virtual void UpdateSummaryFormat(GridMenuItemClickEventArgs ee) {
			if(Column.ColumnType == typeof(TimeSpan))
				ee.SummaryFormat = ee.SummaryFormat.Replace(":0.##", "");
		}
		static void ClearEmptySummaryItems(GridColumn column) {
			for(int i = column.Summary.Count - 1; i >= 0; i--)
				if(column.Summary[i].SummaryType == SummaryItemType.None)
					column.Summary.RemoveAt(i);
		}
	}
	public class GridViewBandMenu : GridViewColumnMenu {
		public GridViewBandMenu(GridView view) : base(view) { }
		GridBand band = null;
		protected override bool ColumnMenu { get { return false; } }
		public override void Init(object info) {
			this.band = info as GridBand;
			base.Init(info);
		}
	}
	internal class GridViewMenuHelper {
		[ThreadStatic]
		static Image hSplitImage = null;
		internal static void HideSplit(GridControl grid) {
			if(grid.IsSplitGrid)
				grid.SplitContainer.HideSplitView();
		}
		internal static void ShowSplit(GridControl grid) {
			if(grid.IsSplitGrid)
				grid.SplitContainer.ShowSplitView();
		}
		internal static Image GetSplitImage(GridView view) {
			if(view.GridControl.IsSplitGrid && view.GridControl.SplitContainer.Horizontal)
				return HorizontalSplitImage;
			return GridMenuImages.Column.Images[14]; ;
		}
		static Image HorizontalSplitImage {
			get {
				if(hSplitImage == null) {
					hSplitImage = GridMenuImages.Column.Images[14].Clone() as Image;
					hSplitImage.RotateFlip(RotateFlipType.Rotate90FlipY);
				}
				return hSplitImage;
			}
		}
	}
	public class GridViewColumnMenu : GridViewMenu {
		public GridViewColumnMenu(GridView view) : base(view) { }
		GridColumn column = null;
		public override void Init(object info) {
			this.column = info as GridColumn;
			base.Init(info);
		}
		protected virtual bool ColumnMenu { get { return true; } }
		protected override BaseView ViewCore { get { return Column != null ? Column.View : base.ViewCore; } }
		public override GridMenuType MenuType { get { return GridMenuType.Column; } }
		public GridColumn Column { get { return column; } }
		protected ColumnSortOrder SortOrder { get { return Column == null ? ColumnSortOrder.None : Column.SortOrder; } }
		protected int GroupIndex { get { return Column == null ? -1 : Column.GroupIndex; } }
		bool IsGroupColumn { get { return GroupIndex != -1; } }
		bool IsColumnFiltered {
			get {
				if(!View.OptionsCustomization.AllowFilter) return false;
				if(Column.FilterInfo.Type != ColumnFilterType.None) return true;
				return false;
			}
		}
		bool AllowClearColumnSort {
			get {
				return View.CanSortColumn(Column) && !IsGroupColumn && Column.SortOrder != ColumnSortOrder.None;
			}
		}
		bool AllowClearAllSort {
			get {
				if(View.IsEditFormVisible) return false;
				if(View.SortInfo.GroupCount >= View.SortInfo.Count) return false;
				if(AllowClearColumnSort && View.SortInfo.Count - View.SortInfo.GroupCount <= 1) return false;
				return true;
			}
		}
		RepositoryItem ColumnEditor {							 
			get {
				if(Column == null) return null;
				return Column.RealColumnEdit;
			}
		}
		protected override void CreateItems() {
			Items.Clear();
			if(Column != null) {
				if(IsGroupColumn) {
					Items.Add(CreateMenuItem(GridStringId.MenuGroupPanelFullExpand, GridMenuImages.GroupPanel.Images[0], GridMenuImages.GroupPanelLarge.Images[0], View.SortInfo.GroupCount > 0));
					Items.Add(CreateMenuItem(GridStringId.MenuGroupPanelFullCollapse, GridMenuImages.GroupPanel.Images[1], GridMenuImages.GroupPanelLarge.Images[1], View.SortInfo.GroupCount > 0));
				}
				Items.Add(CreateMenuCheckItem(GridStringId.MenuColumnSortAscending, SortOrder == ColumnSortOrder.Ascending, GridMenuImages.Column.Images[1], GridMenuImages.ColumnLarge.Images[1], View.CanSortColumn(Column), IsGroupColumn));
				Items.Add(CreateMenuCheckItem(GridStringId.MenuColumnSortDescending, SortOrder == ColumnSortOrder.Descending, GridMenuImages.Column.Images[2], GridMenuImages.ColumnLarge.Images[2], View.CanSortColumn(Column)));
				if(AllowClearColumnSort)
					Items.Add(CreateMenuItem(GridStringId.MenuColumnClearSorting, null));
				if(AllowClearAllSort)
					Items.Add(CreateMenuItem(GridStringId.MenuColumnClearAllSorting, null));
				if(IsGroupColumn) CreateSummarySortInfo();
				Items.Add(CreateMenuItem(IsGroupColumn ? GridStringId.MenuColumnUnGroup : GridStringId.MenuColumnGroup, GridMenuImages.Column.Images[3], GridMenuImages.ColumnLarge.Images[3], View.CanGroupColumn(Column), true));
			}
			Items.Add(CreateMenuItem(GetHideShowCaptionForGroupPanel(View.OptionsView.ShowGroupPanel),
				GridMenuImages.Column.Images[4], GridMenuImages.ColumnLarge.Images[4], GridStringId.MenuColumnGroupBox, true));
			if(Column == null && AllowClearAllSort) Items.Add(CreateMenuItem(GridStringId.MenuColumnClearAllSorting, null));
			if(View.OptionsMenu.ShowSplitItem && View.IsSplitView && !View.IsDesignMode)
				Items.Add(CreateMenuItem(View.IsSplitVisible ? GridStringId.MenuHideSplitItem : GridStringId.MenuShowSplitItem, GridViewMenuHelper. GetSplitImage(View)));
			if(Column != null) {
				if(IsGroupColumn && (Column.ColumnType == typeof(DateTime) || Column.ColumnType == typeof(DateTime?))
					&& !View.IsDesignMode) CreateGroupIntervalInfo();
				if(IsGroupColumn && !View.IsDesignMode) CreateGroupSummaryEditorInfo();
				if(Column.Visible || IsGroupColumn)
					Items.Add(CreateMenuItem(GridStringId.MenuColumnRemoveColumn, null,
						Column.OptionsColumn.AllowShowHide && !(!IsGroupColumn && View.VisibleColumns.Count == 1 && View.SortInfo.GroupCount != 0), true));
				if(!Column.Visible && !IsGroupColumn)
					Items.Add(CreateMenuItem(GridStringId.MenuColumnShowColumn, null, Column.OptionsColumn.AllowShowHide, true));
			}
			if(View.OptionsCustomization.AllowColumnMoving)
				Items.Add(CreateMenuItem(GetStringColumnCustomization(View), GridMenuImages.Column.Images[5], GridMenuImages.ColumnLarge.Images[5], View.CustomizationForm == null, Column == null));
			if(Column != null && (!IsGroupColumn || View.OptionsView.ShowGroupedColumns))
				Items.Add(CreateMenuItem(GridStringId.MenuColumnBestFit, GridMenuImages.Column.Images[6], GridMenuImages.ColumnLarge.Images[6], View.CanResizeColumn(Column) && !Column.OptionsColumn.FixedWidth));
			if(!(View is DevExpress.XtraGrid.Views.BandedGrid.BandedGridView && View.OptionsView.ColumnAutoWidth)
				&& View.OptionsCustomization.AllowColumnResizing) {
				Items.Add(CreateMenuItem(GridStringId.MenuColumnBestFitAllColumns, null, View.VisibleColumns.Count > 0));
			}
			bool beginGroup = true;
			if(Column != null) {
				if((View.IsDesignMode || object.Equals(View.GridControl.Tag, "Design")) && View.OptionsCustomization.AllowFilter) {
					Items.Add(CreateMenuCheckItem(GridStringId.MenuColumnFilter, View.IsColumnAllowFilter(Column), GridMenuImages.Column.Images[10], GridMenuImages.ColumnLarge.Images[10], true, beginGroup));
					beginGroup = false;
				}
				if(IsColumnFiltered && View.IsColumnAllowFilter(Column)) {
					Items.Add(CreateMenuItem(GridStringId.MenuColumnClearFilter, GridMenuImages.Column.Images[7], GridMenuImages.ColumnLarge.Images[7], true, beginGroup));
					beginGroup = false;
				}
			}
			if(GridCriteriaHelper.IsAllowFilterEditorForColumn(View, Column) && !View.IsDesignMode) {
				Items.Add(CreateMenuItem(GridStringId.MenuColumnFilterEditor, GridMenuImages.Column.Images[8], GridMenuImages.ColumnLarge.Images[8], true, beginGroup));
				if(!View.OptionsFind.AlwaysVisible && View.OptionsFind.AllowFindPanel && (!View.IsDetailView || View.IsZoomedView))
					Items.Add(CreateMenuItem(
						View.IsFindPanelVisible ? GridStringId.MenuColumnFindFilterHide : GridStringId.MenuColumnFindFilterShow, null));
				if(View.OptionsMenu.ShowAutoFilterRowItem && ColumnMenu)
					Items.Add(CreateMenuItem(
						View.OptionsView.ShowAutoFilterRow ? GridStringId.MenuColumnAutoFilterRowHide : GridStringId.MenuColumnAutoFilterRowShow,
						null));
			}
			if(Column != null) {
				if(Column.AllowFilterModeChanging && !View.IsServerMode && Column.OptionsFilter.AllowFilter && View.OptionsCustomization.AllowFilter) {
					DXSubMenuItem parentItem = CreateSubMenuItem(GridStringId.MenuColumnFilterMode, null);
					Items.Add((DXMenuItem)parentItem);
					parentItem.Items.Add(CreateMenuCheckItem(GridStringId.MenuColumnFilterModeValue, Column.FilterMode == ColumnFilterMode.Value, null));
					parentItem.Items.Add(CreateMenuCheckItem(GridStringId.MenuColumnFilterModeDisplayText, Column.FilterMode == ColumnFilterMode.DisplayText, null));
				}
				if(Column.ShowUnboundExpressionMenu && !View.IsDesignMode)
					Items.Add(CreateMenuItem(GridStringId.MenuColumnExpressionEditor, GridMenuImages.Column.Images[12], GridMenuImages.ColumnLarge.Images[12], true, true));
				if(View.OptionsMenu.ShowConditionalFormattingItem && !View.IsDesignMode && !DenyConditionFormatEditor) {
					DXSubMenuItem conditionalFormattingItem = CreateSubMenuItem(GridStringId.MenuColumnConditionalFormatting, GridMenuImages.Column.Images[15], GridMenuImages.ColumnLarge.Images[15]);
					conditionalFormattingItem.BeginGroup = true;
					GridFormatRuleMenuItems formatRulesItems = new GridFormatRuleMenuItems(View, Column, conditionalFormattingItem.Items);
					if(formatRulesItems.Count > 0)
						Items.Add((DXMenuItem)conditionalFormattingItem);
				}
			}
		}
		bool DenyConditionFormatEditor {
			get {
				if(column == null) return false;
				return (ColumnEditor is RepositoryItemPictureEdit ||
					ColumnEditor is RepositoryItemImageEdit ||
					ColumnEditor is RepositoryItemBaseProgressBar ||
					ColumnEditor is RepositoryItemBreadCrumbEdit ||
					ColumnEditor is RepositoryItemRangeTrackBar ||
					ColumnEditor is RepositoryItemSparklineEdit ||
					ColumnEditor is RepositoryItemTokenEdit
					);
			}
		}
		internal static GridStringId GetStringColumnCustomization(ColumnView view) {
			if(view is DevExpress.XtraGrid.Views.BandedGrid.BandedGridView) return GridStringId.MenuColumnBandCustomization;
			return GridStringId.MenuColumnColumnCustomization;
		}
		void CreateSummarySortInfo() {
			if(!View.OptionsMenu.ShowGroupSortSummaryItems) return;
			if(View.GroupSummary.ActiveCount == 0) return;
			if(View.GroupSummarySortInfo.Count > 0) {
				Items.Add(CreateMenuItem(GridStringId.MenuColumnResetGroupSummarySort, null));
			}
			DXSubMenuItem menu = new DXSubMenuItem(GridLocalizer.Active.GetLocalizedString(GridStringId.MenuColumnSortGroupBySummaryMenu));
			menu.Tag = GridStringId.MenuColumnSortGroupBySummaryMenu;
			foreach(GridGroupSummaryItem item in View.GroupSummary) {
				if(item.SummaryType == SummaryItemType.None) continue;
				DXMenuCheckItem menuItemAsc = CreateSummarySortMenuItem(item, ColumnSortOrder.Ascending);
				DXMenuCheckItem menuItemDesc = CreateSummarySortMenuItem(item, ColumnSortOrder.Descending);
				menuItemAsc.BeginGroup = true && menu.Items.Count > 0;
				menu.Items.Add(menuItemAsc);
				menu.Items.Add(menuItemDesc);
			}
			if(menu.Items.Count > 0) Items.Add(menu);
		}
		string GetDisplaySummaryName(GridSummaryItem item) {
			string fname = item.FieldName;
			if(!string.IsNullOrEmpty(fname)) {
				GridColumn col = View.Columns[fname];
				if(col != null) return col.GetTextCaption();
				return MasterDetailHelper.SplitPascalCaseString(fname);
			}
			return Column.GetTextCaption();
		}
		string GetSummaryTypeDescription(SummaryItemType type) {
			switch(type) {
				case SummaryItemType.Sum: return GridLocalizer.Active.GetLocalizedString(GridStringId.MenuColumnSumSummaryTypeDescription);
				case SummaryItemType.Min: return GridLocalizer.Active.GetLocalizedString(GridStringId.MenuColumnMinSummaryTypeDescription);
				case SummaryItemType.Max: return GridLocalizer.Active.GetLocalizedString(GridStringId.MenuColumnMaxSummaryTypeDescription);
				case SummaryItemType.Count: return GridLocalizer.Active.GetLocalizedString(GridStringId.MenuColumnCountSummaryTypeDescription);
				case SummaryItemType.Average: return GridLocalizer.Active.GetLocalizedString(GridStringId.MenuColumnAverageSummaryTypeDescription);
				case SummaryItemType.Custom: return GridLocalizer.Active.GetLocalizedString(GridStringId.MenuColumnCustomSummaryTypeDescription);
			}
			return string.Format("{0}", type);
		}
		DXMenuCheckItem CreateSummarySortMenuItem(GridGroupSummaryItem item, ColumnSortOrder sortOrder) {
			DXMenuCheckItem res = new DXMenuCheckItem(string.Format(GridLocalizer.Active.GetLocalizedString(GridStringId.MenuColumnGroupSummarySortFormat),
				 GetDisplaySummaryName(item),
				 GetSummaryTypeDescription(item.SummaryType), GridLocalizer.Active.GetLocalizedString(sortOrder == ColumnSortOrder.Ascending ? GridStringId.MenuColumnSortAscending : GridStringId.MenuColumnSortDescending)));
			res.Tag = item;
			if(sortOrder == ColumnSortOrder.Ascending) {
				res.Click += new EventHandler(OnSummarySortAscending);
			} else {
				res.Click += new EventHandler(OnSummarySortDescending);
			}
			foreach(GroupSummarySortInfo info in View.GroupSummarySortInfo) {
				if(info.GroupColumn == Column && info.SummaryItem == item)
					res.Checked = info.SortOrder == sortOrder;
			}
			return res;
		}
		void CreateGroupIntervalInfo() {
			if(!View.OptionsMenu.ShowDateTimeGroupIntervalItems) return;
			DXSubMenuItem menu = new DXSubMenuItem(GridLocalizer.Active.GetLocalizedString(GridStringId.MenuColumnGroupIntervalMenu));
			menu.Tag = GridStringId.MenuColumnGroupIntervalMenu;
			Items.Add(menu);
			menu.Items.Add(CreateGroupIntervalItem(ColumnGroupInterval.Date & ColumnGroupInterval.Default, GridStringId.MenuColumnGroupIntervalDay));
			menu.Items.Add(CreateGroupIntervalItem(ColumnGroupInterval.DateMonth, GridStringId.MenuColumnGroupIntervalMonth));
			menu.Items.Add(CreateGroupIntervalItem(ColumnGroupInterval.DateYear, GridStringId.MenuColumnGroupIntervalYear));
			menu.Items.Add(CreateGroupIntervalItem(ColumnGroupInterval.DateRange, GridStringId.MenuColumnGroupIntervalSmart));
		}
		DXMenuItem CreateGroupIntervalItem(ColumnGroupInterval groupInterval, GridStringId gridStringId) {
			DXMenuCheckItem menuItem = new DXMenuCheckItem();
			menuItem.Caption = GridLocalizer.Active.GetLocalizedString(gridStringId);
			menuItem.Click += new EventHandler(OnDateTimeGroupIntervalMenuItemClick);
			menuItem.Tag = groupInterval;
			menuItem.Checked = groupInterval == Column.GroupInterval;
			return menuItem;
		}
		void CreateGroupSummaryEditorInfo() {
			if(!View.OptionsMenu.ShowGroupSummaryEditorItem) return;
			DXMenuItem menuItem = new DXMenuItem(GridLocalizer.Active.GetLocalizedString(GridStringId.MenuColumnGroupSummaryEditor));
			menuItem.Image = GridMenuImages.Column.Images[11];
			menuItem.LargeImage = GridMenuImages.ColumnLarge.Images[11];
			Items.Add(menuItem);
			menuItem.Click += new EventHandler(OnCreateGroupSummaryEditorClick);
		}
		void OnCreateGroupSummaryEditorClick(object sender, EventArgs e) {
			if(RaiseClickEvent(sender, new GridMenuItemClickEventArgs(Column, null, null, SummaryItemType.None, string.Empty, MenuType, sender as DXMenuItem))) return;
			using(GroupSummaryEditorForm form = new GroupSummaryEditorForm(View, Column)) {
				View.InitDialogFormProperties(form);
				form.ShowDialog();
			}
		}
		void OnDateTimeGroupIntervalMenuItemClick(object sender, EventArgs e) {
			if(RaiseClickEvent(sender, new GridMenuItemClickEventArgs(Column, null, null, SummaryItemType.None, string.Format("{0}", ((DXMenuItem)sender).Tag), MenuType, sender as DXMenuItem))) return;
			Column.GroupInterval = (ColumnGroupInterval)((DXMenuItem)sender).Tag;
		}
		void OnSummarySortDescending(object sender, EventArgs e) {
			GroupBySummary(sender, ((DXMenuItem)sender).Tag as GridGroupSummaryItem, ColumnSortOrder.Descending);
		}
		void OnSummarySortAscending(object sender, EventArgs e) {
			GroupBySummary(sender, ((DXMenuItem)sender).Tag as GridGroupSummaryItem, ColumnSortOrder.Ascending);
		}
		void GroupBySummary(object sender, GridGroupSummaryItem summaryItem, ColumnSortOrder sortOrder) {
			DXMenuCheckItem checkItem = sender as DXMenuCheckItem;
			if(checkItem == null || !checkItem.Checked) return;
			if(!IsGroupColumn) return;
			if(RaiseClickEvent(sender, new GridMenuItemClickEventArgs(Column, null, summaryItem, SummaryItemType.Custom, sortOrder.ToString(), MenuType, sender as DXMenuItem))) return;
			List<GroupSummarySortInfo> items = new List<GroupSummarySortInfo>();
			foreach(GroupSummarySortInfo info in View.GroupSummarySortInfo) {
				if(info.GroupColumn == Column || info.GroupLevel == Column.GroupIndex) continue;
				items.Add(info);
			}
			items.Add(new GroupSummarySortInfo(summaryItem, Column, sortOrder));
			View.GroupSummarySortInfo.ClearAndAddRange(items.ToArray());
		}
		void ChangeColumnSorting(GridColumn column, ColumnSortOrder sortOrder) {
			if(column == null) return;
			View.GroupSummarySortInfo.Clear();
			column.SortOrder = sortOrder;
		}
		protected override void OnMenuItemClick(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			if(RaiseClickEvent(sender, new GridMenuItemClickEventArgs(Column, null, null, SummaryItemType.Custom, "", MenuType, item))) return;
			if(!(item.Tag is GridStringId) || View == null) return;
			GridStringId id = (GridStringId)item.Tag;
			switch(id) {
				case GridStringId.MenuGroupPanelFullExpand: View.ExpandAllGroups(); break;
				case GridStringId.MenuGroupPanelFullCollapse: View.CollapseAllGroups(); break;
				case GridStringId.MenuColumnSortAscending: ChangeColumnSorting(Column, ColumnSortOrder.Ascending); break;
				case GridStringId.MenuColumnSortDescending: ChangeColumnSorting(Column, ColumnSortOrder.Descending); break;
				case GridStringId.MenuColumnClearSorting: ChangeColumnSorting(Column, ColumnSortOrder.None); break;
				case GridStringId.MenuColumnClearAllSorting: View.ClearSorting(); break;
				case GridStringId.MenuColumnUnGroup:
					if(Column != null) Column.UnGroup();
					break;
				case GridStringId.MenuColumnRemoveColumn:
					if(Column != null) {
						column.UnGroup();
						Column.Visible = false;
					}
					break;
				case GridStringId.MenuColumnShowColumn:
					if(Column != null) {
						Column.Visible = true;
					}
					break;
				case GridStringId.MenuColumnGroup:
					if(Column != null) Column.Group();
					break;
				case GridStringId.MenuColumnColumnCustomization:
				case GridStringId.MenuColumnBandCustomization:
					View.ColumnsCustomization();
					break;
				case GridStringId.MenuColumnBestFit:
					if(Column != null) Column.BestFit();
					break;
				case GridStringId.MenuColumnFilter:
					if(Column != null) {
						Column.OptionsFilter.AllowFilter = !Column.OptionsFilter.AllowFilter;
						View.LayoutChanged();
					}
					break;
				case GridStringId.MenuColumnClearFilter:
					if(Column != null) Column.FilterInfo = ColumnFilterInfo.Empty;
					break;
				case GridStringId.MenuColumnBestFitAllColumns:
					View.BestFitColumns();
					break;
				case GridStringId.MenuColumnGroupBox:
					View.OptionsView.ShowGroupPanel = !View.OptionsView.ShowGroupPanel;
					break;
				case GridStringId.MenuColumnFilterEditor:
					View.ShowFilterEditor(Column);
					break;
				case GridStringId.MenuColumnAutoFilterRowHide:
				case GridStringId.MenuColumnAutoFilterRowShow:
					View.OptionsView.ShowAutoFilterRow = !View.OptionsView.ShowAutoFilterRow;
					break;
				case GridStringId.MenuColumnFindFilterHide:
					View.HideFindPanel();
					break;
				case GridStringId.MenuColumnFindFilterShow:
					View.ShowFindPanel();
					break;
				case GridStringId.MenuColumnExpressionEditor:
					View.ShowUnboundExpressionEditor(Column);
					break;
				case GridStringId.MenuColumnResetGroupSummarySort:
					View.GroupSummarySortInfo.Clear();
					break;
				case GridStringId.MenuColumnFilterModeValue:
					if(Column != null) {
						Column.ClearFilter();
						Column.FilterMode = ColumnFilterMode.Value;
					}
					break;
				case GridStringId.MenuColumnFilterModeDisplayText:
					if(Column != null) {
						Column.ClearFilter();
						Column.FilterMode = ColumnFilterMode.DisplayText;
					}
					break;
				case GridStringId.MenuHideSplitItem:
					GridViewMenuHelper.HideSplit(View.GridControl);
					break;
				case GridStringId.MenuShowSplitItem:
					GridViewMenuHelper.ShowSplit(View.GridControl);
					break;
			}
		}
	}
	public class GridViewGroupPanelMenu : GridViewMenu {
		public GridViewGroupPanelMenu(GridView view) : base(view) { }
		protected override void CreateItems() {
			Items.Clear();
			Items.Add(CreateMenuItem(GridStringId.MenuGroupPanelFullExpand, GridMenuImages.GroupPanel.Images[0], GridMenuImages.GroupPanelLarge.Images[0], View.SortInfo.GroupCount > 0));
			Items.Add(CreateMenuItem(GridStringId.MenuGroupPanelFullCollapse, GridMenuImages.GroupPanel.Images[1], GridMenuImages.GroupPanelLarge.Images[1], View.SortInfo.GroupCount > 0));
			Items.Add(CreateMenuItem(GridStringId.MenuGroupPanelClearGrouping, GridMenuImages.GroupPanel.Images[2], GridMenuImages.GroupPanelLarge.Images[2], View.SortInfo.GroupCount > 0, true));
			Items.Add(CreateMenuItem(GetHideShowCaptionForGroupPanel(View.OptionsView.ShowGroupPanel),
				GridMenuImages.Column.Images[4], GridMenuImages.ColumnLarge.Images[4], GridStringId.MenuColumnGroupBox, true));
			if(View.OptionsMenu.ShowSplitItem && View.IsSplitView)
				Items.Add(CreateMenuItem(View.IsSplitVisible ? GridStringId.MenuHideSplitItem : GridStringId.MenuShowSplitItem, GridViewMenuHelper.GetSplitImage(View)));
		}
		public override GridMenuType MenuType { get { return GridMenuType.Group; } }
		protected override void OnMenuItemClick(object sender, EventArgs e) {
			if(RaiseClickEvent(sender, null)) return;
			DXMenuItem item = sender as DXMenuItem;
			if(!(item.Tag is GridStringId)) return;
			GridStringId id = (GridStringId)item.Tag;
			switch(id) {
				case GridStringId.MenuGroupPanelFullExpand : View.ExpandAllGroups(); break;
				case GridStringId.MenuGroupPanelFullCollapse : View.CollapseAllGroups(); break;
				case GridStringId.MenuGroupPanelClearGrouping : View.ClearGrouping(); break;
				case GridStringId.MenuColumnGroupBox:
					View.OptionsView.ShowGroupPanel = !View.OptionsView.ShowGroupPanel;
					break;
				case GridStringId.MenuHideSplitItem:
					GridViewMenuHelper.HideSplit(View.GridControl);
					break;
				case GridStringId.MenuShowSplitItem:
					GridViewMenuHelper.ShowSplit(View.GridControl);
					break;
			}
		}
	}
	public class GridMenuImages {
		[ThreadStatic]
		static ImageCollection groupPanel = null;
		public static ImageCollection GroupPanel {
			get {
				if(groupPanel == null) groupPanel = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraGrid.Images.GridGroupMenu.png", typeof(GridMenuImages).Assembly, new Size(16, 16));
				return groupPanel;
			}
		}
		[ThreadStatic]
		static ImageCollection column = null;
		public static ImageCollection Column {
			get {
				if(column == null) column = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraGrid.Images.GridColumnMenu.png", typeof(GridMenuImages).Assembly, new Size(16, 16));
				return column;
			}
		}
		[ThreadStatic]
		static ImageCollection footer = null;
		public static ImageCollection Footer {
			get {
				if(footer == null) footer = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraGrid.Images.GridFooterMenu.png", typeof(GridMenuImages).Assembly, new Size(16, 16));
				return footer;
			}
		}
		[ThreadStatic]
		static ImageCollection groupPanelLarge = null;
		public static ImageCollection GroupPanelLarge {
			get {
				if(groupPanelLarge == null) groupPanelLarge = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraGrid.Images.GridGroupMenu32x32.png", typeof(GridMenuImages).Assembly, new Size(32, 32));
				return groupPanelLarge;
			}
		}
		[ThreadStatic]
		static ImageCollection columnLarge = null;
		public static ImageCollection ColumnLarge {
			get {
				if(columnLarge == null) columnLarge = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraGrid.Images.GridColumnMenu32x32.png", typeof(GridMenuImages).Assembly, new Size(32, 32));
				return columnLarge;
			}
		}
		[ThreadStatic]
		static ImageCollection footerLarge = null;
		public static ImageCollection FooterLarge {
			get {
				if(footerLarge == null) footerLarge = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraGrid.Images.GridFooterMenu32x32.png", typeof(GridMenuImages).Assembly, new Size(32, 32));
				return footerLarge;
			}
		}
	}
	public class GridFormatRuleMenuHelper {
		public static FormatRuleColumnInfo GetFormatRuleColumnInfoByGridColumn(GridColumn column) {
			if(column == null) 
				return new FormatRuleColumnInfo() { Name = string.Empty, ColumnType = null, RepositoryItem = null };
			return new FormatRuleColumnInfo() { Name = column.FieldName, ColumnType = column.ColumnType, 
				RepositoryItem = column.RealColumnEdit, OwnerControl = column.View.GridControl, MenuManager = column.View.GridControl.MenuManager, IsServerMode = column.View.IsServerMode};
		}
	}
	public class GridFormatRuleMenuItems : FormatRuleMenuItems {
		GridColumn gridColumn = null;
		GridView view = null;
		public GridFormatRuleMenuItems(GridView view, GridColumn column, DXMenuItemCollection collection) : 
			base(GridFormatRuleMenuHelper.GetFormatRuleColumnInfoByGridColumn(column), collection) {
				gridColumn = column;
				this.view = view;
				UpdateItems();
		}
		protected override void InitRuleFormProperties(XtraEditors.FormatRule.Forms.FormatRuleEditFormBase form) {
			view.InitDialogFormProperties(form);
		}
		protected override void DataBarItemClick(object sender, EventArgs e) {
			if(gridColumn == null || view == null) return;
			DXMenuItem item = sender as DXMenuItem;
			FormatConditionRuleDataBar rule = new FormatConditionRuleDataBar();
			RemoveRule(view, gridColumn, typeof(FormatConditionRuleDataBar), rule);
			rule.Appearance.Reset();
			rule.AppearanceNegative.Reset();
			rule.PredefinedName = string.Format("{0}", item.Tag);
			view.FormatRules.Add(gridColumn, rule);
		}
		protected override void IconSetItemClick(object sender, EventArgs e) {
			if(gridColumn == null || view == null) return;
			DXMenuItem item = sender as DXMenuItem;
			RemoveRule(view, gridColumn, typeof(FormatConditionRuleIconSet), null);
			FormatConditionRuleIconSet rule = new FormatConditionRuleIconSet();
			rule.IconSet = item.Tag as FormatConditionIconSet;
			view.FormatRules.Add(gridColumn, rule);
		}
		protected override void ColorScaleItemClick(object sender, EventArgs e) {
			if(gridColumn == null || view == null) return;
			DXMenuItem item = sender as DXMenuItem;
			RemoveRule(view, gridColumn, typeof(FormatConditionRule2ColorScale), null);
			RemoveRule(view, gridColumn, typeof(FormatConditionRule3ColorScale), null);
			view.FormatRules.Add(gridColumn, CreateColorScaleRule(string.Format("{0}", item.Tag)));
		}
		FormatConditionRule2ColorScale CreateColorScaleRule(string key) {
			FormatConditionRule2ColorScale rule;
			if(key.Split(',').Length > 2) 
				rule = new FormatConditionRule3ColorScale();
			else rule = new FormatConditionRule2ColorScale();
			rule.PredefinedName = key;
			return rule;
		}
		static void RemoveRule(GridView view, GridColumn gridColumn, Type type, FormatConditionRuleBase ruleBase) {
			view.BeginUpdate();
			try {
				for(int i = view.FormatRules.Count - 1; i >= 0; i--) {
					GridFormatRule rule = view.FormatRules[i];
					if(AllowRemove(rule, gridColumn, type)) {
						if(ruleBase != null)
							ruleBase.Assign(rule.Rule);
						view.FormatRules.RemoveAt(i);
					}
				}
			} finally {
				view.EndUpdate();
			}
		}
		static bool AllowRemove(GridFormatRule rule, GridColumn gridColumn, Type type) {
			if(rule == null) return false;
			if(type == null && gridColumn == null) return true;
			if(type == null) return gridColumn == rule.Column;
			if(gridColumn == null) return rule.Rule.GetType() == type;
			return rule.Rule.GetType() == type && gridColumn == rule.Column;
		}
		protected override void ManageColumnRulesItemClick(object sender, EventArgs e) {
			if(gridColumn == null) return;
			gridColumn.ShowFormatRulesManager();
		}
		protected override void ClearColumnRulesItemClick(object sender, EventArgs e) {
			if(gridColumn == null || view == null) return;
			RemoveRule(view, gridColumn, null, null);
		}
		protected override void ClearAllRulesItemClick(object sender, EventArgs e) {
			if(view == null) return;
			RemoveRule(view, null, null, null);
		}
		protected override bool IsRuleColumnExists {
			get {
				if(gridColumn == null || view == null) return false;
				foreach(GridFormatRule rule in view.FormatRules)
					if(gridColumn == rule.Column) return true;
				return false;
			}
		}
		protected override bool IsRuleExists {
			get {
				if(view == null) return false;
				return view.FormatRules.Count > 0;
			}
		}
		protected override void UpdateFormatConditionRules(FormatConditionRuleAppearanceBase rule, bool applyToRow) {
			view.FormatRules.Add(gridColumn, rule).ApplyToRow = applyToRow;
		}
		protected override void ExpressionBuilderClick(object sender, EventArgs e) {
			BaseEdit edit = sender as BaseEdit;
			if(view == null || edit == null) return;
			using(FilterColumnCollection filterColumns = view.CreateFilterColumnCollection()) {
				FilterColumn filterColumn = GridCriteriaHelper.GetFilterColumnByGridColumn(filterColumns, gridColumn);
				using(ExpressionBuilder builder = new ExpressionBuilder(filterColumns, view.GridControl.MenuManager, view.GridControl.LookAndFeel, view, filterColumn, edit.EditValue)) {
					if(view.WorkAsLookup) builder.TopMost = true;
					view.InitDialogFormProperties(builder);
					if(builder.ShowDialog(view.GridControl) == DialogResult.OK)
						edit.EditValue = builder.FilterCriteria;
				}
			}
		}
		protected override void ExpressionBuilderCustomDisplayText(object sender, XtraEditors.Controls.CustomDisplayTextEventArgs e) {
			BaseEdit edit = sender as BaseEdit;
			CriteriaOperator criteria = e.Value as CriteriaOperator;
			if(!ReferenceEquals(criteria, null)) {
				e.DisplayText = view.GetFilterDisplayText(criteria);
			}
		}
		protected override FilterColumn GetDefaultFilterColumn(FilterColumnCollection fcCollection) {
			return GridCriteriaHelper.GetFilterColumnByGridColumn(fcCollection, gridColumn);
		}
		protected override FilterColumnCollection GetFilterColumns() {
			return view.CreateFilterColumnCollection();
		}
	}
	internal class ExpressionBuilder : FilterBuilder {
		CriteriaOperator criteria;
		public ExpressionBuilder(FilterColumnCollection columns, IDXMenuManager manager, UserLookAndFeel lookAndFeel, ColumnView view, FilterColumn fColumn, object criteria)
			: base(columns, manager, lookAndFeel, view, fColumn) {
				this.criteria = criteria as CriteriaOperator;
				if(!ReferenceEquals(this.criteria, null))
					fcMain.FilterCriteria = this.criteria;
				fcMain.ShowOperandTypeIcon = true;
		}
		protected override void CreateButtonCaptions() {
			base.CreateButtonCaptions();
			HideApplyButton();
			this.ShowIcon = false;
			this.Text = DevExpress.XtraEditors.Controls.Localizer.Active.GetLocalizedString(XtraEditors.Controls.StringId.FormatRuleMenuItemCustomCondition);
		}
		protected override void ApplyFilter() { }
	}
}
namespace DevExpress.XtraGrid.Extension {
	public static class FormatRulesManagerExtension {
		public static void ShowFormatRulesManager(this GridColumn gridColumn) {
			if(gridColumn == null) return;
			var view = gridColumn.View as GridView;
			if(view == null) return;
			var filterColumns = view.CreateFilterColumnCollection();
			var filterColumnDefault = GridCriteriaHelper.GetFilterColumnByGridColumn(filterColumns, gridColumn);
			using(ManagerRuleForm<GridFormatRule, GridColumn> manager =
				new ManagerRuleForm<GridFormatRule, GridColumn>(
					view.FormatRules,
					view.Columns,
					filterColumns,
					filterColumnDefault,
					gridColumn.FieldName,
					view.GridControl.MenuManager,
					GetNameColumns(view))) {
				view.InitDialogFormProperties(manager);
				manager.ShowDialog();
			}
		}
		static List<ColumnNameInfo> GetNameColumns(GridView view) {
			var nameColumns = new List<ColumnNameInfo>();
			foreach(GridColumn item in view.Columns) 
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
