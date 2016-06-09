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

using DevExpress.Web.Data;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Rendering {
	public class GridViewUpdatableContainer : GridUpdatableContainer {
		public GridViewUpdatableContainer(ASPxGridView grid)
			: base(grid) {
		}
		protected new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		protected new GridViewRenderHelper RenderHelper { get { return base.RenderHelper as GridViewRenderHelper; } }
		public GridViewContextMenu GroupPanelContextMenu { get; private set; }
		public GridViewContextMenu ColumnsContextMenu { get; private set; }
		public GridViewContextMenu RowsContextMenu { get; private set; }
		public GridViewContextMenu FooterContextMenu { get; private set; }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(!DesignMode) {
				Controls.Add(CreateHiddenImage(GridViewImages.ElementName_ArrowDragDownImage, GridViewImages.DragAndDropArrowDownName));
				Controls.Add(CreateHiddenImage(GridViewImages.ElementName_ArrowDragUpImage, GridViewImages.DragAndDropArrowUpName));
				Controls.Add(CreateHiddenImage(GridViewImages.ElementName_DragHideColumnImage, GridViewImages.DragAndDropHideColumnName));
			}
		}
		protected override Table CreateMainTable() {
			return new GridViewHtmlTable(RenderHelper);
		}
		protected override GridHtmlScrollableControl CreateScrollableControl() {
			return new GridViewHtmlScrollableControl(RenderHelper);
		}
		protected override void CreateAdaptiveHeaderPanel() {
			if(Grid.Settings.ShowColumnHeaders && RenderHelper.IsHideDataCellsWindowLimitMode())
				AddControl(new GridViewHtmlAdaptiveHeaderPanel(RenderHelper), GridViewRenderHelper.AdaptiveHeaderPanelID);
		}
		protected override void CreateGroupPanel() {
			if(Grid.Settings.ShowGroupPanel)
				Controls.Add(new GridViewGroupPanel(RenderHelper));
		}
		protected override void CreateFooterPanel() {
			if(Grid.Settings.ShowFooter && RenderHelper.IsHideDataCellsWindowLimitMode())
				AddControl(new GridViewHtmlAdaptiveFooterPanel(RenderHelper), GridViewRenderHelper.AdaptiveFooterPanelID);
		}
		protected override void CreateFixedColumnsScroll() {
			if(RenderHelper.HasFixedColumns)
				AddControl(new GridViewHtmlFixedColumnsScrollableControl(RenderHelper), GridViewRenderHelper.FixedColumnsScrollableContainerID);
		}
		protected override void CreatePopupControls() {
			base.CreatePopupControls();
			if(DataProxy.HasParentRows)
				Controls.Add(new GridViewHtmlParentRowsWindow(RenderHelper, DataProxy));
			if(RenderHelper.RequireRenderFilterRowMenu)
				Controls.Add(new GridViewFilterRowMenu(Grid));
			CreateContextMenu();
		}
		protected override GridCustomizationWindow CreateCustWindowControl() {
			return new GridViewCustomizationWindow(Grid);
		}
		protected override GridEditFormPopup CreateEditFormPopupControl() {
			return new GridViewEditFormPopup(Grid, DataProxy.EditingRowVisibleIndex);
		}
		protected override GridHtmlStatusBar CreateStatusBar() {
			return new GridViewHtmlStatusBar(RenderHelper);
		}
		protected void CreateContextMenu() {
			if(RenderHelper.RequireRenderGroupPanelContextMenu)
				Controls.Add(GroupPanelContextMenu = new GridViewContextMenu(Grid, GridViewContextMenuType.GroupPanel));
			if(RenderHelper.RequireRenderColumnsContextMenu)
				Controls.Add(ColumnsContextMenu = new GridViewContextMenu(Grid, GridViewContextMenuType.Columns));
			if(RenderHelper.RequireRenderRowsContextMenu)
				Controls.Add(RowsContextMenu = new GridViewContextMenu(Grid, GridViewContextMenuType.Rows));
			if(RenderHelper.RequireRenderFooterContextMenu)
				Controls.Add(FooterContextMenu = new GridViewContextMenu(Grid, GridViewContextMenuType.Footer));
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareContextMenus();
		}
		protected void PrepareContextMenus() {
			var contextMenus = new GridViewContextMenu[] { GroupPanelContextMenu, ColumnsContextMenu, RowsContextMenu, FooterContextMenu };
			foreach(var menu in contextMenus) {
				if(menu != null)
					menu.PrepareItemImages();
			}
		}
		WebControl CreateHiddenImage(string id, string imageName) {
			Image image = RenderUtils.CreateImage();
			image.ID = id;
			image.Style.Add(HtmlTextWriterStyle.Position, "absolute");
			image.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
			image.Style.Add(HtmlTextWriterStyle.Top, "-100px");
			RenderHelper.AssignImageToControl(imageName, image);
			return image;
		}
	}
	public class GridViewCustomizationWindow : GridCustomizationWindow {
		public GridViewCustomizationWindow(ASPxGridView grid)
			: base(grid) {
		}
		protected new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		protected new GridViewRenderHelper RenderHelper { get { return base.RenderHelper as GridViewRenderHelper; } }
		protected override TableCell CreateHeaderCell(IWebGridColumn column) {
			return new GridViewTableHeaderCell(RenderHelper, column as GridViewColumn, GridHeaderLocation.Customization, false, false);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			string contextMenuScript = RenderHelper.Scripts.GetContextMenu();
			if(!string.IsNullOrEmpty(contextMenuScript))
				RenderUtils.SetStringAttribute(this, "oncontextmenu", contextMenuScript);
		}
	}
	public class GridViewEditFormPopup : GridEditFormPopup {
		public GridViewEditFormPopup(ASPxGridView grid, int visibleIndex)
			: base(grid, visibleIndex) {
		}
		protected new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		protected new GridViewRenderHelper RenderHelper { get { return base.RenderHelper as GridViewRenderHelper; } }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			Grid.RaiseHtmlEditFormCreated(ContentDiv); 
		}
		protected override WebControl CreateContentContainer() {
			return new GridViewEditFormTable(RenderHelper, VisibleIndex);
		}
		protected override WebControl CreateErrorHtmlControl() {
			var table = RenderUtils.CreateTable();
			table.Width = Unit.Percentage(100);
			if(RenderHelper.HasEditingError) {
				GridViewTableRow row = new GridViewTableEditingErrorRow(RenderHelper, false);
				table.Rows.Add(row);
				Grid.RaiseHtmlRowCreated(row);
			} else {
				TableRow row = new InternalTableRow();
				row.ID = GridViewRenderHelper.EditingRowID;
				row.Cells.Add(new InternalTableCell());
				table.Rows.Add(row);
			}
			return table;
		}
		protected override string GetPopupElementID() {
			if(RenderHelper.DataProxy.VisibleRowCountOnPage > 0)
				return RenderHelper.GetRowId(GetShowingVisibleIndex());
			return base.GetPopupElementID();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			var mainTable = ContentContainer as Table;
			if(mainTable != null) {
				mainTable.GridLines = GridLines.None;
				mainTable.CellSpacing = 3;
				mainTable.CellPadding = 0;
			}
		}
	}
	public class GridViewHtmlStatusBar : GridHtmlStatusBar {
		public GridViewHtmlStatusBar(GridViewRenderHelper renderHelper)
			: base(renderHelper) {
		}
		protected new GridViewRenderHelper RenderHelper { get { return base.RenderHelper as GridViewRenderHelper; } }
		protected override TableCell CreateCommandItemsCell() {
			return new GridViewCommandItemsCell(RenderHelper, GetAllowedCommandItems(), false);
		}
	}
	[ToolboxItem(false)]
	public class GridViewHtmlParentRowsWindow : ASPxPopupControl {
		GridViewRenderHelper renderHelper;
		WebDataProxy dataProxy;
		public GridViewHtmlParentRowsWindow(GridViewRenderHelper renderHelper, WebDataProxy dataProxy) {
			this.renderHelper = renderHelper;
			this.dataProxy = dataProxy;
			EnableViewState = false;
			ShowOnPageLoad = false;
			ShowHeader = false;
			PopupVerticalAlign = PopupVerticalAlign.Above;
			PopupHorizontalOffset = -1;
			ParentSkinOwner = Grid;
			PopupAnimationType = AnimationType.Fade;
		}
		protected GridViewRenderHelper RenderHelper { get { return renderHelper; } }
		protected ASPxGridView Grid { get { return RenderHelper.Grid; } }
		protected ASPxGridViewScripts Scripts { get { return RenderHelper.Scripts; } }
		protected WebDataProxy DataProxy { get { return dataProxy; } }
		protected override void CreateControlHierarchy() {
			ID = GridViewRenderHelper.ParentRowsWindowID;
			base.CreateControlHierarchy();
			Controls.Add(CreateTable());
			EnsureChildControlsRecursive(this, false);
		}
		protected virtual Table CreateTable() {
			Table table = RenderUtils.CreateTable();
			List<int> list = DataProxy.GetParentRows();
			for(int i = 0; i < list.Count; i++) {
				table.Rows.Add(new GridViewTableParentGroupRow(RenderHelper, list[i]));
			}
			return table;
		}
		protected override void PrepareControlHierarchy() {
			ControlStyle.CopyFrom(Grid.StylesPopup.Common.Style);
			ContentStyle.CopyFrom(GetContentStyle());
			base.PrepareControlHierarchy();
			Attributes["onmouseout"] = Scripts.GetHideParentRowsWindowFunction(true);
			this.Width = Unit.Pixel(10);
		}
		protected AppearanceStyleBase GetContentStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(Grid.StylesPopup.Common.Content);
			style.Paddings.CopyFrom(new Paddings(0));
			return style;
		}
	}
	[ToolboxItem(false)]
	public class GridViewFilterRowMenu : ASPxPopupMenu {
		ASPxGridView grid;
		public GridViewFilterRowMenu(ASPxGridView grid)
			: base(grid) {
			this.grid = grid;
			CreateItems();
			EnableViewState = false;
			ParentSkinOwner = grid;
			ControlStyle.CopyFrom(Grid.Styles.FilterRowMenu);
			ItemStyle.CopyFrom(Grid.Styles.FilterRowMenuItem);
			PopupVerticalAlign = PopupVerticalAlign.Below;
			EnableClientSideAPIInternal = true;
			ID = GridViewRenderHelper.FilterRowMenuID;
			ClientSideEvents.ItemClick = Grid.RenderHelper.Scripts.GetFilterRowMenuItemClick();
		}
		protected ASPxGridView Grid { get { return grid; } }
		void CreateItems() {
			CreateItem(ASPxGridViewStringId.AutoFilterBeginsWith, AutoFilterCondition.BeginsWith);
			CreateItem(ASPxGridViewStringId.AutoFilterContains, AutoFilterCondition.Contains);
			CreateItem(ASPxGridViewStringId.AutoFilterDoesNotContain, AutoFilterCondition.DoesNotContain);
			CreateItem(ASPxGridViewStringId.AutoFilterEndsWith, AutoFilterCondition.EndsWith);
			CreateItem(ASPxGridViewStringId.AutoFilterEquals, AutoFilterCondition.Equals);
			CreateItem(ASPxGridViewStringId.AutoFilterNotEqual, AutoFilterCondition.NotEqual);
			CreateItem(ASPxGridViewStringId.AutoFilterLess, AutoFilterCondition.Less);
			CreateItem(ASPxGridViewStringId.AutoFilterLessOrEqual, AutoFilterCondition.LessOrEqual);
			CreateItem(ASPxGridViewStringId.AutoFilterGreater, AutoFilterCondition.Greater);
			CreateItem(ASPxGridViewStringId.AutoFilterGreaterOrEqual, AutoFilterCondition.GreaterOrEqual);
			CreateItem(ASPxGridViewStringId.AutoFilterLike, AutoFilterCondition.Like, ASPxGridViewStringId.AutoFilterLikeToolTip);
		}
		MenuItem CreateItem(ASPxGridViewStringId textStringId, AutoFilterCondition value, ASPxGridViewStringId tooltipStringId) {
			MenuItem item = CreateItem(textStringId, value);
			item.ToolTip = ASPxGridViewLocalizer.GetString(tooltipStringId);
			return item;
		}
		MenuItem CreateItem(ASPxGridViewStringId stringId, AutoFilterCondition value) {
			MenuItem item = Items.Add(ASPxGridViewLocalizer.GetString(stringId), GetMenuItemId(value));
			item.GroupName = "_";
			return item;
		}
		string GetMenuItemId(AutoFilterCondition condition) {
			return string.Format("{0}|{1}", (int)condition, GetPossibleFilterRowTypeKinds(condition));
		}
		string GetPossibleFilterRowTypeKinds(AutoFilterCondition condition) {
			if(condition == AutoFilterCondition.Like)
				return BaseFilterHelper.LikeConditionSymbol;
			string result = string.Empty;
			foreach(FilterRowTypeKind kind in Enum.GetValues(typeof(FilterRowTypeKind))) {
				if(BaseFilterHelper.IsValidCondition(kind, condition))
					result += BaseFilterHelper.GetFilterRowTypeKindSymbol(kind);
			}
			return result;
		}
	}
	[ToolboxItem(false)]
	public class GridViewContextMenu : ASPxPopupMenu {
		GridViewContextMenuItem rootItem;
		public GridViewContextMenu(ASPxGridView grid, GridViewContextMenuType type)
			: base(grid) {
			MenuType = type;
			ID = string.Format("{0}_{1}", GridViewRenderHelper.ContextMenuID, MenuType);
			EnableViewState = false;
			Grid = grid;
			ParentSkinOwner = Grid as ISkinOwner;
			MenuHelper = new GridViewContextMenuHelper(Grid, this);
			CustomJSProperties += ContextMenu_CustomJSProperties;
			ClientSideEvents.ItemClick = Grid.RenderHelper.Scripts.GetContextMenuItemClick();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override MenuItem RootItem {
			get {
				if(rootItem == null)
					rootItem = new GridViewContextMenuItem(this);
				return rootItem;
			}
		}
		public new GridViewContextMenuItemCollection Items { get { return RootItem.Items as GridViewContextMenuItemCollection; } }
		public GridViewContextMenuType MenuType { get; private set; }
		protected GridViewContextMenuHelper MenuHelper { get; private set; }
		protected ASPxGridView Grid { get; private set; }
		protected GridViewRenderHelper GridRenderHelper { get { return Grid.RenderHelper; } }
		public void PrepareItemImages() {
			MenuHelper.PrepareItemImages();
		}
		protected override void PrepareControlHierarchy() {
			Styles.CopyFrom(GetContextMenuStyle());
			base.PrepareControlHierarchy();
		}
		protected virtual GridViewContextMenuStyle GetContextMenuStyle() {
			switch(MenuType) {
				case GridViewContextMenuType.GroupPanel:
					return GridRenderHelper.GetGroupPanelContextMenuStyle();
				case GridViewContextMenuType.Columns:
					return GridRenderHelper.GetColumnContextMenuStyle();
				case GridViewContextMenuType.Rows:
					return GridRenderHelper.GetRowContextMenuStyle();
				case GridViewContextMenuType.Footer:
					return GridRenderHelper.GetFooterContextMenuStyle();
			}
			return null;
		}
		void ContextMenu_CustomJSProperties(object sender, CustomJSPropertiesEventArgs e) {
			JSProperties["cpItemsInfo"] = MenuHelper.GetClientInfo();
			JSProperties["cpType"] = (int)MenuType;
		}
	}
	[ToolboxItem(false)]
	public class ASPxGridViewPager : ASPxGridPager {
		public ASPxGridViewPager(ASPxGridView grid)
			: base(grid) {
		}
		protected new ASPxGridView Grid { get { return (ASPxGridView)base.Grid; } }
		protected override bool IsAdaptivityEnebled() {
			return base.IsAdaptivityEnebled() || Grid.IsAdaptivityEnabled();
		}
	}
}
