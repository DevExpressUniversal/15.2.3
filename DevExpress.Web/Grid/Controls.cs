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
using DevExpress.Web.FilterControl;
using DevExpress.Web.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Rendering {
	[ViewStateModeById]
	public abstract class GridUpdatableContainer : ASPxInternalWebControl {
		Table dataTable;
		public GridUpdatableContainer(ASPxGridBase grid) {
			Grid = grid;
		}
		protected ASPxGridBase Grid { get; private set; }
		protected GridRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		protected WebDataProxy DataProxy { get { return Grid.DataProxy; } }
		protected ASPxGridLoadingPanelSettings SettingsLoadingPanel { get { return Grid.SettingsLoadingPanel; } }
		protected ASPxGridScripts Scripts { get { return RenderHelper.Scripts; } }
		public IGridEndlessPagingItemsContainer DataItemsContainer { get { return DataTable as IGridEndlessPagingItemsContainer; } }
		protected internal Table DataTable { get { return ScrollableControl != null ? ScrollableControl.ContentTable : dataTable; } }
		protected GridHtmlScrollableControl ScrollableControl { get; private set; }
		public GridEndlessPagingUpdatableContainer EndlessPagingUpdatableContainer { get; private set; }
		protected GridCustomizationWindow CustomizationWindow { get; private set; }
		protected GridEditFormPopup PopupEditForm { get; private set; }
		protected WebFilterControlPopup PopupFilterControlForm { get; private set; }
		protected HeaderFilterPopup HeaderFilterPopupControl { get; private set; }
		protected LoadingPanelControl LoadingPanel { get; private set; }
		protected WebControl LoadingDiv { get; private set; }
		protected virtual bool CanRenderPopupControls { get { return !DesignMode && RenderHelper.IsGridEnabled; } }
		protected bool HasLoadingPanel { get { return !DesignMode && SettingsLoadingPanel.Mode != GridViewLoadingPanelMode.Disabled; } }
		protected override void CreateControlHierarchy() {
			if(Grid.IsErrorOnCallbackCore) return;
			Grid.OnBeforeCreateControlHierarchy();
			if(Grid.Settings.ShowTitlePanel)
				AddControl(new GridHtmlTitle(Grid), GridRenderHelper.TitleID);
			if(RenderHelper.RequireRenderTopPagerControl)
				AddControl(new GridHtmlTopPagerPanel(RenderHelper), GridRenderHelper.TopPagerPanelID);
			if(Grid.SettingsSearchPanel.Visible)
				AddControl(new GridHtmlSearchPanel(Grid), GridRenderHelper.SearchPanelID);
			CreateCustomizationPanel();
			CreateGroupPanel();
			CreateAdaptiveHeaderPanel();
			if(RenderHelper.HasScrolling) 
				AddControl(ScrollableControl = CreateScrollableControl(), GridRenderHelper.ScrollableContainerID);
			else
				AddControl(this.dataTable = CreateMainTable(), GridRenderHelper.MainTableID);
			if(!DesignMode || !RenderHelper.HasScrolling)
				CreateFixedColumnsScroll();
			if(RenderHelper.UseEndlessPaging)
				CreateEndlessPagingAdditionalControls();
			CreateFooterPanel();
			if(CanRenderPopupControls)
				CreatePopupControls();
			if(RenderHelper.RequireRenderBottomPagerControl)
				AddControl(new GridHtmlBottomPagerPanel(RenderHelper), GridRenderHelper.BottomPagerPanelID);
			if(RenderHelper.RequireRenderFilterBar)
				AddControl(new WebFilterControlPopupRow(Grid), GridRenderHelper.FilterBarID);
			if(RenderHelper.RequireRenderStatusBar)
				AddControl(CreateStatusBar(), GridRenderHelper.StatusBarID);
			if(RenderHelper.AllowBatchEditing)
				AddControl(CreateBatchEditorsContainer(), GridRenderHelper.BatchEditorsContainerID);
			if(HasLoadingPanel)
				CreateLoadingPanel();
		}
		protected abstract Table CreateMainTable();
		protected abstract GridHtmlStatusBar CreateStatusBar();
		protected abstract GridHtmlScrollableControl CreateScrollableControl();
		protected abstract GridCustomizationWindow CreateCustWindowControl();
		protected abstract GridEditFormPopup CreateEditFormPopupControl();
		protected virtual void CreateFooterPanel() { }
		protected virtual void CreateCustomizationPanel() { }
		protected virtual void CreateGroupPanel() { }
		protected virtual void CreateAdaptiveHeaderPanel() { }
		protected virtual void CreateFixedColumnsScroll() { }
		protected virtual GridBatchEditorsContainer CreateBatchEditorsContainer() {
			return new GridBatchEditorsContainer(RenderHelper);
		}
		protected virtual void CreateEndlessPagingAdditionalControls() {
			AddControl(EndlessPagingUpdatableContainer = new GridEndlessPagingUpdatableContainer(RenderHelper), GridRenderHelper.EndlessPagingUpdatableContainerID);
		}
		protected virtual void CreatePopupControls() {
			CreateCustomizationWindow();
			CreatePopupEditForm();
			CreatePopupFilterControlForm();
			CreateHeaderFilterControlPopup();
		}
		protected virtual void CreateCustomizationWindow() {
			if(!RenderHelper.RequireRenderCustomizationWindow) return;
			CustomizationWindow = CreateCustWindowControl();
			Controls.Add(CustomizationWindow);
			CustomizationWindow.EnableViewState = false;
			if(!string.IsNullOrEmpty(Grid.ClientSideEvents.CustomizationWindowCloseUp))
				CustomizationWindow.CloseUp = Scripts.GetCustomizationWindowCloseUpHandler();
			if(RenderUtils.Browser.IsIE)
				CustomizationWindow.ClientSideEvents.Shown = "ASPx.GVCustWindowShown_IE";
		}
		protected virtual void CreatePopupEditForm() {
			if(!RenderHelper.RequireRenderEditFormPopup) return;
			PopupEditForm = CreateEditFormPopupControl();
			Control container = this;
			if(RenderHelper.UseEndlessPaging)
				container = EndlessPagingUpdatableContainer;
			container.Controls.Add(PopupEditForm);
			PopupEditForm.EnableViewState = false;
			PopupEditForm.ClientSideEvents.CloseUp = Scripts.GetClosePopupEditFormFunction();
		}
		protected virtual void CreatePopupFilterControlForm() {
			if(!Grid.IsFilterControlVisible) return;
			PopupFilterControlForm = CreateFilterControlPopup();
			Control container = this;
			if(RenderHelper.UseEndlessPaging)
				container = EndlessPagingUpdatableContainer;
			container.Controls.Add(PopupFilterControlForm);
			PopupFilterControlForm.EnableViewState = false;
		}
		protected virtual WebFilterControlPopup CreateFilterControlPopup() {
			return new WebFilterControlPopup(Grid);
		}
		protected void CreateHeaderFilterControlPopup() {
			if(!RenderHelper.RequireRenderHeaderFilterPopup) return;
			HeaderFilterPopupControl = new HeaderFilterPopup(Grid);
			Controls.Add(HeaderFilterPopupControl);
		}
		protected override void PrepareControlHierarchy() {
			if(LoadingPanel != null)
				RenderHelper.PrepareLoadingPanel(LoadingPanel);
			if(LoadingDiv != null)
				RenderHelper.PrepareLoadingDiv(LoadingDiv);
		}
		protected virtual void CreateLoadingPanel() {
			LoadingPanel = new LoadingPanelControl(RenderHelper.IsRightToLeft);
			LoadingPanel.EnableViewState = false;
			Controls.Add(LoadingPanel);
			LoadingPanel.ID = Grid.GetLoadingPanelIDInternal();
			bool isPopup = SettingsLoadingPanel.Mode == GridViewLoadingPanelMode.ShowAsPopup || SettingsLoadingPanel.Mode == GridViewLoadingPanelMode.Default;
			LoadingPanel.Image = GetLoadingPanelImageProperties(isPopup ? Grid.Images.LoadingPanel : Grid.Images.LoadingPanelOnStatusBar);
			if(!isPopup && SettingsLoadingPanel.ImagePosition != ImagePosition.Left && SettingsLoadingPanel.ImagePosition != ImagePosition.Right) {
				LoadingPanel.Settings = new SettingsLoadingPanel(null);
				LoadingPanel.Settings.Assign(SettingsLoadingPanel);
				LoadingPanel.Settings.ImagePosition = ImagePosition.Left;
			} else {
				LoadingPanel.Settings = SettingsLoadingPanel;
			}
			LoadingDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			LoadingDiv.ID = Grid.GetLoadingDivIDInternal();
			Controls.Add(LoadingDiv);
		}
		private ImageProperties GetLoadingPanelImageProperties(ImageProperties loadingPanelProperties) {
			ImageProperties properties = new ImageProperties();
			properties.CopyFrom(Grid.Images.GetDefaultLoadingImageProperties());
			properties.CopyFrom(loadingPanelProperties);
			return properties;
		}
		protected void AddControl(Control control, string id) {
			control.ID = id;
			Controls.Add(control);
		}
	}
	public class GridHtmlMainTable : InternalTable, IGridEndlessPagingItemsContainer {
		public GridHtmlMainTable(GridRenderHelper renderHelper) {
			RenderHelper = renderHelper;
		}
		protected GridRenderHelper RenderHelper { get; private set; }
		protected ASPxGridBase Grid { get { return RenderHelper.Grid; } }
		protected WebDataProxy DataProxy { get { return Grid.DataProxy; } }
		protected virtual bool RequireTouchDraggableClass { get { return Grid.IsSwipeGesturesEnabled(); } }
		protected virtual bool RequireUseDblClick { get { return false; } }
		protected override void PrepareControlHierarchy() {
			Attributes["onclick"] = RenderHelper.Scripts.GetMainTableClickFunction();
			if(RequireUseDblClick)
				Attributes["ondblclick"] = RenderHelper.Scripts.GetMainTableDblClickFunction();
			CellPadding = 0;
			CellSpacing = 0;
			RenderHelper.GetMainTableStyle().AssignToControl(this);
			Width = Unit.Percentage(100); 
			Style["empty-cells"] = "show"; 
			if(RenderHelper.IsRightToLeft)
				Attributes["dir"] = "rtl";
			if(RequireTouchDraggableClass)
				RenderUtils.AppendMSTouchDraggableClassNameIfRequired(this);
		}
		protected virtual IEnumerable<WebControl> GetEndlessPagingItems() {
			return Rows.OfType<WebControl>();
		}
		IEnumerable<WebControl> IGridEndlessPagingItemsContainer.Items { get { return GetEndlessPagingItems(); } }
	}
	public abstract class GridHtmlSummaryPanel : ASPxInternalWebControl {
		const string DefaultSeparator = ";";
		public GridHtmlSummaryPanel(GridRenderHelper renderHelper) {
			RenderHelper = renderHelper;
		}
		protected GridRenderHelper RenderHelper { get; private set; }
		protected ASPxGridBase Grid { get { return RenderHelper.Grid; } }
		protected List<WebControl> SummaryItemControls { get; set; }
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected override void CreateControlHierarchy() {
			SummaryItemControls = new List<WebControl>();
			var summaries = Grid.GetActiveTotalSummaryItems();
			foreach(ASPxSummaryItemBase item in summaries) {
				var text = RenderHelper.TextBuilder.GetSummaryItemText(item);
				if(item != summaries.Last())
					text = String.Concat(text, DefaultSeparator);
				var summmaryDiv = RenderUtils.CreateDiv();
				Controls.Add(summmaryDiv);
				summmaryDiv.Controls.Add(new LiteralControl(text));
				SummaryItemControls.Add(summmaryDiv);
			}
		}
	}
	public abstract class GridHtmlStatusBar : InternalTable {
		public GridHtmlStatusBar(GridRenderHelper renderHelper) {
			RenderHelper = renderHelper;
		}
		protected ASPxGridBase Grid { get { return RenderHelper.Grid; } }
		protected GridRenderHelper RenderHelper { get; private set; }
		protected TableRow MainRow { get { return Rows[0]; } }
		protected TableCell RootCell { get; private set; }
		protected TableCell LoadingContainer { get { return MainRow.Cells.Count > 1 ? MainRow.Cells[1] : null; } }
		protected override void CreateControlHierarchy() {
			Rows.Add(RenderUtils.CreateTableRow());
			var rootCell = RenderUtils.CreateTableCell();
			if(!RenderHelper.AddStatusBarTemplateControl(rootCell)) {
				if(RenderHelper.AllowBatchEditing)
					rootCell = CreateCommandItemsCell(); 
				else
					rootCell.Text = "&nbsp;";
			}
			RootCell = rootCell;
			MainRow.Cells.Add(RootCell);
			if(Grid.SettingsLoadingPanel.Mode == GridViewLoadingPanelMode.ShowOnStatusBar) {
				MainRow.Cells.Add(RenderUtils.CreateTableCell());
				LoadingContainer.ID = GridRenderHelper.LoadingPanelContainerID;
			}
		}
		protected abstract TableCell CreateCommandItemsCell();
		protected List<GridCommandButtonType> GetAllowedCommandItems() {
			return new List<GridCommandButtonType>() { GridCommandButtonType.Update, GridCommandButtonType.Cancel };
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetStatusBarStyle().AssignToControl(this, true);
			Width = Unit.Percentage(100);
			RootCell.Width = Unit.Percentage(100);
			RenderHelper.AppendGridCssClassName(MainRow);
			base.PrepareControlHierarchy();
		}
	}
	public class GridHtmlTitle : InternalTable {
		public GridHtmlTitle(ASPxGridBase grid) {
			Grid = grid;
		}
		protected ASPxGridBase Grid { get; private set; }
		protected ASPxGridTextSettings SettingsText { get { return Grid.SettingsText; } }
		protected GridRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		protected WebControl MainCell { get { return Rows[0].Cells[0]; } }
		protected override void CreateControlHierarchy() {
			Rows.Add(RenderUtils.CreateTableRow());
			Rows[0].Cells.Add(RenderUtils.CreateTableCell());
			if(!RenderHelper.AddTitleTemplateControl(MainCell)) {
				MainCell.Controls.Add(RenderUtils.CreateLiteralControl(SettingsText.Title));
			}
		}
		protected override void PrepareControlHierarchy() {
			Width = Unit.Percentage(100);
			CellSpacing = 0;
			CellPadding = 0;
			RenderHelper.GetTitleStyle().AssignToControl(MainCell, true);
		}
	}
	public abstract class GridHtmlPagerPanelBase : ASPxInternalWebControl {
		public GridHtmlPagerPanelBase(GridRenderHelper renderHelper) {
			RenderHelper = renderHelper;
		}
		protected abstract string PagerID { get; }
		protected abstract GridViewPagerBarPosition Position { get; }
		protected GridRenderHelper RenderHelper { get; private set; }
		protected ASPxGridBase Grid { get { return RenderHelper.Grid; } }
		protected WebControl PagerPanel { get; private set; }
		protected override void CreateChildControls() {
			PagerPanel = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Controls.Add(PagerPanel);
			if(!RenderHelper.AddPagerBarTemplateControl(PagerPanel, Position, PagerID))
				PagerPanel.Controls.Add(Grid.CreatePagerControl(PagerID));
		}
	}
	public class GridHtmlTopPagerPanel : GridHtmlPagerPanelBase {
		public GridHtmlTopPagerPanel(GridRenderHelper renderHelper) : base(renderHelper) { }
		protected override string PagerID { get { return GridRenderHelper.TopPagerID; } }
		protected override GridViewPagerBarPosition Position { get { return GridViewPagerBarPosition.Top; } }
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetPagerTopPanelStyle().AssignToControl(PagerPanel, true);
		}
	}
	public class GridHtmlBottomPagerPanel : GridHtmlPagerPanelBase {
		public GridHtmlBottomPagerPanel(GridRenderHelper renderHelper) : base(renderHelper) { }
		protected override string PagerID { get { return GridRenderHelper.BottomPagerID; } }
		protected override GridViewPagerBarPosition Position { get { return GridViewPagerBarPosition.Bottom; } }
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetPagerBottomPanelStyle().AssignToControl(PagerPanel, true);
		}
	}
	public class GridHtmlSearchPanel : ASPxInternalWebControl {
		public GridHtmlSearchPanel(ASPxGridBase grid) {
			Grid = grid;
		}
		public ASPxGridBase Grid { get; private set; }
		public GridRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected Table MainTable { get; private set; }
		protected TableRow Row { get; private set; }
		protected TableCell EditorCell { get; private set; }
		protected ASPxEditBase Editor { get; private set; }
		protected override void CreateControlHierarchy() {
			MainTable = RenderUtils.CreateTable();
			Controls.Add(MainTable);
			Row = RenderUtils.CreateTableRow();
			MainTable.Rows.Add(Row);
			CreateSearchEditorCell();
			CreateButton(GridCommandButtonType.ApplySearchPanelFilter);
			CreateButton(GridCommandButtonType.ClearSearchPanelFilter);
		}
		protected virtual void CreateSearchEditorCell() {
			EditorCell = CreateCell();
			Editor = RenderHelper.CreateSearchPanelEditor(EditorCell);
		}
		protected virtual void CreateButton(GridCommandButtonType buttonType) {
			if(!CheckButtonRenderRequired(buttonType))
				return;
			var cell = CreateCell();
			var button = RenderHelper.CreateCommandButtonControl(buttonType, -1, false);
			cell.Controls.Add(button);
		}
		protected virtual bool CheckButtonRenderRequired(GridCommandButtonType buttonType) {
			switch(buttonType) {
				case GridCommandButtonType.ApplySearchPanelFilter:
					return Grid.SettingsSearchPanel.ShowApplyButton;
				case GridCommandButtonType.ClearSearchPanelFilter:
					return Grid.SettingsSearchPanel.ShowClearButton;
			}
			return false;
		}
		protected TableCell CreateCell() {
			var cell = RenderUtils.CreateTableCell();
			Row.Cells.Add(cell);
			return cell;
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetSearchPanelStyle().AssignToControl(this);
			EditorCell.Width = Unit.Percentage(100);
			var edit = Editor as ASPxEdit;
			if(edit != null)
				edit.ForceUseValueChangedClientEvent();
			if(Editor.Width.IsEmpty)
				Editor.Width = Unit.Percentage(100);
		}
	}
	[ViewStateModeById]
	public class GridEndlessPagingUpdatableContainer : InternalHtmlControl {
		public GridEndlessPagingUpdatableContainer(GridRenderHelper renderHelper)
			: base(HtmlTextWriterTag.Div) {
			RenderHelper = renderHelper;
		}
		protected GridRenderHelper RenderHelper { get; private set; }
		public override void RenderBeginTag(HtmlTextWriter writer) {
			if(RenderHelper.RequireEndlessPagingPartialLoad)
				return;
			base.RenderBeginTag(writer);
		}
		public override void RenderEndTag(HtmlTextWriter writer) {
			if(RenderHelper.RequireEndlessPagingPartialLoad)
				return;
			base.RenderEndTag(writer);
		}
	}
	public class GridBatchEditorsContainer : ASPxInternalWebControl {
		public GridBatchEditorsContainer(GridRenderHelper renderHelper) {
			RenderHelper = renderHelper;
		}
		public GridRenderHelper RenderHelper { get; private set; }
		public ASPxGridBase Grid { get { return RenderHelper.Grid; } }
		public GridColumnHelper ColumnHelper { get { return Grid.ColumnHelper; } }
		public GridBatchEditHelper BatchEditHelper { get { return Grid.BatchEditHelper; } }
		protected Table CellErrorTable { get; private set; }
		protected TableCell ErrorImageCell { get; private set; }
		protected Image ErrorImage { get; private set; }
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected override void CreateControlHierarchy() {
			foreach(var column in ColumnHelper.EditColumns)
				BatchEditHelper.CreateEditor(column, CreateEditorContainer(column));
			CreateCellErrorTable();
		}
		protected virtual void CreateCellErrorTable() {
			CellErrorTable = RenderUtils.CreateTable();
			CellErrorTable.ID = GridRenderHelper.BatchEditCellErrorTableID;
			Controls.Add(CellErrorTable);
			var row = RenderUtils.CreateTableRow();
			CellErrorTable.Rows.Add(row);
			var textCell = RenderUtils.CreateTableCell();
			row.Cells.Add(textCell);
			ErrorImageCell = RenderUtils.CreateTableCell();
			row.Cells.Add(ErrorImageCell);
			ErrorImage = RenderUtils.CreateImage();
			ErrorImageCell.Controls.Add(ErrorImage);
		}
		protected override void PrepareControlHierarchy() {
			foreach(var control in RenderHelper.EditingRowEditorList) {
				var edit = control as ASPxEdit;
				if(edit == null) continue;
				var skipResize = (edit is ASPxCheckBox) || (edit is ASPxBinaryImage);
				if(!skipResize && edit.Width.IsEmpty)
					edit.Width = Unit.Percentage(100);
			}
			Style["display"] = "none"; 
			RenderHelper.GetBatchEditErrorCellImage().AssignToControl(ErrorImage, Grid.DesignMode);
			CellErrorTable.Width = Unit.Percentage(100);
			ErrorImageCell.CssClass = Grid.Styles.BatchEditErrorCellClassName;
		}
		protected WebControl CreateEditorContainer(IWebGridDataColumn column) {
			var div = RenderUtils.CreateDiv();
			div.ID = string.Format("{0}{1}", GridRenderHelper.BatchEditorContainerID, ColumnHelper.GetColumnGlobalIndex(column));
			div.Width = Unit.Percentage(100);
			if(RenderUtils.IsHtml5Mode(this)) {
				var cellStyle = RenderHelper.GetDataCellStyle(column as GridViewColumn); 
				if(cellStyle.HorizontalAlign != HorizontalAlign.NotSet)
					RenderUtils.SetHorizontalAlignCssAttributes(div, cellStyle.HorizontalAlign.ToString());
			}
			if(RenderHelper.GetColumnEdit(column) is CheckBoxProperties)
				div.Style["vertical-align"] = "middle";
			Controls.Add(div);
			return div;
		}
	}
	public class GridHtmlEditFormPopupContainer : InternalWebControl {
		public GridHtmlEditFormPopupContainer() : base(HtmlTextWriterTag.Div) { }
	}
	[ToolboxItem(false)]
	public class ASPxGridPager : ASPxPagerBase {
		public ASPxGridPager(ASPxGridBase grid)
			: base(grid) {
			Grid = grid;
			EnableViewState = false;
			PagerSettings.Assign(Grid.SettingsPager);
			Styles.Assign(Grid.StylesPager);
			ParentSkinOwner = Grid;
			if(Grid.SettingsPager.PageSizeItemSettings.Visible)
				Width = Unit.Percentage(100);
		}
		public override int PageCount { get { return Grid.DataProxy.PageCount; } }
		public override int PageIndex { get { return Grid.DataProxy.PageIndex; } }
		public override int ItemCount { get { return Grid.DataProxy.VisibleRowCount; } }
		public override int ItemsPerPage { get { return Grid.DataProxy.PageSize; } }
		protected ASPxGridBase Grid { get; private set; }
		protected GridRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		protected ASPxGridScripts Scripts { get { return Grid.RenderHelper.Scripts; } }
		protected override bool RequireInlineLayout { get { return true; } }
		protected override string GetItemElementOnClick(string id) {
			return Scripts.GetPagerOnClickFunction(id);
		}
		protected override string GetPageSizeChangedHandler() {
			return Scripts.GetPagerOnPageSizeChange();
		}
		protected override SEOTarget GetSEOTarget(params object[] args) {
			if(args.Length > 1)
				return new SEOTarget(RenderHelper.GetSEOID(), RenderHelper.SEO.SaveState((int)args[0], (int)args[1]));
			return new SEOTarget(RenderHelper.GetSEOID(), RenderHelper.SEO.SaveState((int)args[0]));
		}
		protected override bool HasContent() { return true; }
		protected override void PrepareControlHierarchy() {
			Font.CopyFrom(Grid.Font);
			if(Page != null) {
				ApplyStyleSheetSkin(Page);
			}
			base.PrepareControlHierarchy();
		}
	}
	[ToolboxItem(false)]
	public abstract class GridCustomizationPanel : ASPxInternalWebControl {
		public GridCustomizationPanel(ASPxGridBase grid) {
			Grid = grid;
		}
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }				
		protected ASPxGridBase Grid { get; private set; }
		protected GridRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		protected GridColumnHelper ColumnHelper { get { return Grid.ColumnHelper; } }
		protected abstract List<IWebGridColumn> CustomizationColumns { get; }
	}
	[ToolboxItem(false)]
	public abstract class GridCustomizationWindow : ASPxPopupControl {
		public GridCustomizationWindow(ASPxGridBase grid)
			: base(grid) {
			ID = GridRenderHelper.CustomizationWindowID;
			Grid = grid;
			ShowOnPageLoad = false;
			ParentSkinOwner = Grid;
			AllowDragging = true;
			PopupAnimationType = AnimationType.Fade;
			CloseAction = CloseAction.CloseButton;
		}
		protected ASPxGridBase Grid { get; private set; }
		protected GridRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		protected GridColumnHelper ColumnHelper { get { return Grid.ColumnHelper; } }
		public string CloseUp { get { return ((PopupControlClientSideEvents)base.ClientSideEventsInternal).CloseUp; } set { ((PopupControlClientSideEvents)base.ClientSideEventsInternal).CloseUp = value; } }
		protected Table MainTable { get; private set; }
		protected WebControl ContentDiv { get; private set; }
		protected internal override Paddings GetContentPaddings(PopupWindow window) {
			return new Paddings(0);
		}
		protected override void ClearControlFields() {
			MainTable = null;
			ContentDiv = null;
		}
		protected override bool BindContainersOnCreate() {
			return false;
		}
		protected override void CreateControlHierarchy() {
			if(Request != null)
				LoadPostData(Request.Params);
			Font.CopyFrom(Grid.Font);
			HeaderText = Grid.SettingsText.GetCustomizationWindowCaption();
			base.CreateControlHierarchy();
			ContentDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			ContentDiv.ID = "Scroller";
			MainTable = RenderUtils.CreateTable();
			CreateHeaders();
			ContentDiv.Controls.Add(GridRenderHelper.DecorateTableForScrollableDiv(MainTable, RenderHelper.IsRightToLeft));
			Controls.Add(ContentDiv);
			EnsureChildControlsRecursive(this, false);
		}
		protected override void PrepareControlHierarchy() {
			CloseButtonImage.CopyFrom(Grid.Images.CustomizationWindowClose); 
			ControlStyle.CopyFrom(RenderHelper.GetCustomizationWindowControlStyle());
			CloseButtonStyle.CopyFrom(RenderHelper.GetCustomizationWindowCloseButtonStyle());
			HeaderStyle.CopyFrom(RenderHelper.GetCustomizationWindowHeaderStyle());
			ContentStyle.CopyFrom(RenderHelper.GetCustomizationWindowContentStyle());
			PopupHorizontalAlign = RenderHelper.GetCustomizationWindowHorizontalAlign();
			PopupHorizontalOffset = RenderHelper.GetCustomizationWindowHorizontalOffset();
			PopupVerticalAlign = RenderHelper.GetCustomizationWindowVerticalAlign();
			PopupVerticalOffset = RenderHelper.GetCustomizationWindowVerticalOffset();
			CloseOnEscape = RenderHelper.GetCustomizationWindowCloseOnEscape();
			base.PrepareControlHierarchy();
			RenderHelper.GetCustomizationStyle().AssignToControl(ContentDiv);
			MainTable.GridLines = GridLines.None;
			MainTable.CellSpacing = 3;
			MainTable.CellPadding = 0;
			Width = Unit.Pixel(1);
			MainTable.Width = Unit.Percentage(100);
			ContentDiv.Height = RenderHelper.GetCustomizationWindowHeight();
			ContentDiv.Width = RenderHelper.GetCustomizationWindowWidth();
			ContentDiv.Style[HtmlTextWriterStyle.Overflow] = "auto";
			if(RenderUtils.Browser.Platform.IsMSTouchUI)
				RenderUtils.AppendDefaultDXClassName(MainTable, Grid.Styles.MSTouchDraggableMarkerCssClassName);
		}
		protected void CreateHeaders() {
			foreach(var column in ColumnHelper.CustWindowColumns)
				AddHeaderRow(CreateHeaderCell(column));
			if(ColumnHelper.CustWindowColumns.Count == 0)
				AddHeaderRow(RenderUtils.CreateTableCell());
		}
		void AddHeaderRow(TableCell headerCell) {
			var row = RenderUtils.CreateTableRow();
			row.Cells.Add(headerCell);
			MainTable.Rows.Add(row);
		}
		protected abstract TableCell CreateHeaderCell(IWebGridColumn column);
	}
	[ToolboxItem(false), ViewStateModeById]
	public abstract class GridEditFormPopup : ASPxPopupControl {
		public GridEditFormPopup(ASPxGridBase grid, int visibleIndex)
			: base(grid) {
			Grid = grid;
			VisibleIndex = visibleIndex;
			ShowOnPageLoad = false;
			PopupAnimationType = AnimationType.Fade;
			ParentSkinOwner = Grid;
			AllowDragging = true;
			CloseAction = CloseAction.CloseButton;
			Width = 600;
		}
		protected ASPxGridBase Grid { get; private set; }
		protected GridRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		protected int VisibleIndex { get; private set; }
		protected WebControl ContentContainer { get; private set; }
		protected WebControl ContentDiv { get; private set; }
		protected Unit WindowWidth { get { return RenderHelper.GetPopupEditFormWidth(); } }
		protected Unit WindowHeight { get { return RenderHelper.GetPopupEditFormHeight(); } }
		protected internal override Paddings GetContentPaddings(PopupWindow window) {
			Paddings paddings = RenderHelper.GetPopupEditFormContentStyle().Paddings;
			if(paddings.IsEmpty)
				return new Paddings(0);
			return paddings;
		}
		protected override void ClearControlFields() {
			ContentContainer = null;
			ContentDiv = null;
		}
		protected override bool BindContainersOnCreate() {
			return false;
		}
		protected override void CreateControlHierarchy() {
			ID = GridRenderHelper.PopupEditFormID;
			Font.CopyFrom(Grid.Font);
			HeaderText = Grid.SettingsText.GetPopupEditFormCaption();
			AllowResize = RenderHelper.GetPopupEditFormAllowResize();
			ResizingMode = RenderHelper.GetPopupEditFormResizeMode();
			Modal = RenderHelper.GetPopupEditFormModal();
			ShowHeader = RenderHelper.GetPopupEditFormShowHeader();
			MinWidth = RenderHelper.GetPopupEditFormMinWidth();
			MinHeight = RenderHelper.GetPopupEditFormMinHeight();
			CloseOnEscape = RenderHelper.GetPopupEditFormCloseOnEscape();
			base.CreateControlHierarchy();
			ContentDiv = new GridHtmlEditFormPopupContainer();
			Controls.Add(ContentDiv);
			if(!RenderHelper.AddEditFormTemplateControl(ContentDiv, VisibleIndex)) {
				ContentContainer = CreateContentContainer();
				ContentDiv.Controls.Add(ContentContainer);
			}
			ContentDiv.Controls.Add(CreateErrorHtmlControl());
			EnsureChildControlsRecursive(this, false);
		}
		protected abstract WebControl CreateContentContainer();
		protected abstract WebControl CreateErrorHtmlControl();
		protected override void PrepareControlHierarchy() {
			CloseButtonImage.CopyFrom(Grid.Images.PopupEditFormWindowClose);
			ControlStyle.CopyFrom(RenderHelper.GetPopupEditFormControlStyle());
			CloseButtonStyle.CopyFrom(RenderHelper.GetPopupEditFormCloseButtonStyle());
			HeaderStyle.CopyFrom(RenderHelper.GetPopupEditFormHeaderStyle());
			ContentStyle.CopyFrom(RenderHelper.GetPopupEditFormContentStyle());
			ModalBackgroundStyle.CopyFrom(RenderHelper.GetPopupEditFormModalBackgroundStyle());
			base.PrepareControlHierarchy();
			if(!WindowWidth.IsEmpty)
				Width = WindowWidth;
			if(!WindowHeight.IsEmpty)
				ContentDiv.Height = WindowHeight;
			ClientSideEvents.Init = RenderHelper.Scripts.GetPopupEditFormOnInitFunction();
			PopupElementID = GetPopupElementID();
			PopupHorizontalAlign = RenderHelper.GetPopupEditFormHorizontalAlign();
			PopupVerticalAlign = RenderHelper.GetPopupEditFormVerticalAlign();
			PopupHorizontalOffset = RenderHelper.GetPopupEditFormHorizontalOffset();
			PopupVerticalOffset = RenderHelper.GetPopupEditFormVerticalOffset();
			RenderHelper.GetPopupEditFormStyle().AssignToControl(ContentDiv, true);
			if(!WindowHeight.IsEmpty) 
				ContentDiv.Style[HtmlTextWriterStyle.Overflow] = "auto";
			ContentDiv.Style[HtmlTextWriterStyle.OverflowX] = "hidden";
		}
		protected virtual string GetPopupElementID() {
			return Grid.UniqueID;
		}
		protected virtual int GetShowingVisibleIndex() {
			if(VisibleIndex >= 0) return VisibleIndex;
			return RenderHelper.DataProxy.VisibleStartIndex;
		}
		protected override bool NeedCreateHierarchyOnInit() {
			return false;
		}
	}
	public class GridHtmlScrollableControlBase : ASPxInternalWebControl {
		public GridHtmlScrollableControlBase(GridRenderHelper helper) {
			RenderHelper = helper;
		}
		protected GridRenderHelper RenderHelper { get; private set; }
		protected ASPxGridBase Grid { get { return RenderHelper.Grid; } }
		protected ASPxGridSettings Settings { get { return Grid.Settings; } }
		protected WebControl CreateDiv() {
			return CreateDiv(this);
		}
		protected WebControl CreateDiv(WebControl parent) {
			WebControl div = RenderUtils.CreateDiv();
			parent.Controls.Add(div);
			return div;
		}
		protected virtual void PrepareScrollDiv(WebControl div) {
			if(RenderHelper.IsRightToLeft)
				div.Attributes["dir"] = "ltr";
			var width = RenderHelper.GetRootTableWidth();
			if(RenderHelper.ShowHorizontalScrolling && width.Type == UnitType.Percentage && !DesignMode)
				width = 1;
			div.Width = width;
		}
	}
	public abstract class GridHtmlScrollableControl : GridHtmlScrollableControlBase {
		public GridHtmlScrollableControl(GridRenderHelper helper)
			: base(helper) {
		}
		protected string GridPrefix { get { return Grid.Styles.GetCssClassNamePrefix(); } }
		protected string ContentScrollDivClassName { get { return GridPrefix + "CSD"; } }
		public Table ContentTable { get; private set; }
		protected WebControl ContentScrollDiv { get; private set; }
		protected bool HasVertScrollBar { get { return RenderHelper.ShowVerticalScrolling; } }
		protected virtual bool CanCreateContentTable {
			get {
				if(RenderHelper.RequireEndlessPagingPartialLoad)
					return Grid.EndlessPagingHelper.ReplacePrevEditedRow || !RenderHelper.HasEmptyDataRow;
				return true;
			}
		}
		protected abstract Table CreateContentTable(WebControl container);
		protected override void CreateControlHierarchy() {
			CreateHeader();
			CreateContent();
			CreateFooter();
		}
		protected override void PrepareControlHierarchy() {
			PrepareHeader();
			PrepareContent();
			PrepareFooter();
		}
		protected virtual void CreateHeader() { }
		protected virtual void CreateFooter() { }
		protected virtual void PrepareHeader() { }
		protected virtual void PrepareFooter() { }
		protected virtual void CreateContent() {
			ContentScrollDiv = CreateDiv();
			AddTopBottomVirtualScrollMargin(Grid.DataProxy.VisibleStartIndex > 0);
			if(CanCreateContentTable)
				ContentTable = CreateContentTable(ContentScrollDiv);
			AddTopBottomVirtualScrollMargin(true);
			if(RenderHelper.UseEndlessPaging && !Grid.EndlessPagingHelper.PartialLoad)
				CreateEndlessPagingLoadingPanelContainer();
		}
		protected virtual void CreateEndlessPagingLoadingPanelContainer() {
			if(Grid.SettingsLoadingPanel.Mode != GridViewLoadingPanelMode.Default || Grid.PageCount <= 1)
				return;
			var div = CreateDiv(ContentScrollDiv);
			div.ID = GridRenderHelper.EndlessPagingLoadingPanelContainerID;
		}
		protected virtual void AddTopBottomVirtualScrollMargin(bool visible) {
			if(!RenderHelper.IsVirtualScrolling)
				return;
			WebControl div = CreateDiv(ContentScrollDiv);
			div.Width = 1;
			div.Visible = visible;
		}
		protected virtual void PrepareContent() {
			PrepareScrollDiv(ContentScrollDiv);
			RenderUtils.AppendDefaultDXClassName(ContentScrollDiv, ContentScrollDivClassName);
			RenderUtils.SetScrollBars(ContentScrollDiv, RenderHelper.HorizontalScrollBarMode, RenderHelper.VerticalScrollBarMode, DesignMode);
			if(HasVertScrollBar)
				ContentScrollDiv.Height = Settings.VerticalScrollableHeight;
			if(DesignMode && RenderHelper.HorizontalScrollBarMode != ScrollBarMode.Hidden)
				RenderUtils.SetStyleStringAttribute(ContentScrollDiv, "float", "left");
		}
	}
	public class GridHtmlFilterByValueContainer : GridHtmlFilterContainer {
		public GridHtmlFilterByValueContainer(IWebGridDataColumn column, bool includeFilteredOut)
			: base(column, includeFilteredOut) {
		}
		protected bool IsCheckedList { get { return Column.Adapter.HeaderFilterMode == GridHeaderFilterMode.CheckedList; } }
		protected ASPxListBox ListBox { get; private set; }
		protected ASPxCheckBox SelectAllCheckBox { get; private set; }
		protected Table Table { get; private set; }
		protected TableCell SelectAllCell { get; private set; }
		protected TableCell SeparatorCell { get; private set; }
		protected TableCell ListBoxCell { get; private set; }
		protected override void CreateControlHierarchy() {
			Table = CreateTable();
			if(IsCheckedList) {
				CreateSelectAllCheckBox();
				CreateSeparator();
			}
			CreateListBox();
			if(SelectAllCheckBox != null)
				SetSelectAllCheckState();
		}
		void CreateSelectAllCheckBox() {
			SelectAllCell = CreateCell();
			SelectAllCheckBox = new ASPxCheckBox();
			SelectAllCheckBox.ID = GridRenderHelper.HeaderFilterSelectAllCheckBoxID;
			SelectAllCell.Controls.Add(SelectAllCheckBox);
			SelectAllCheckBox.ParentSkinOwner = Grid;
			SelectAllCheckBox.AllowGrayed = true;
			SelectAllCheckBox.AllowGrayedByClick = false;
		}
		void CreateListBox() {
			ListBoxCell = CreateCell();
			ListBox = new ASPxListBox();
			ListBox.ID = GridRenderHelper.HeaderFilterListBoxID;
			ListBoxCell.Controls.Add(ListBox);
			ListBox.ParentSkinOwner = Grid;
			ListBox.EnableClientSideAPI = true;
			ListBox.EncodeHtml = false;
			ListBox.SelectionMode = IsCheckedList ? ListEditSelectionMode.CheckColumn : ListEditSelectionMode.Single;
			EditorsIntegrationHelper.DisableScrolling(ListBox);
			PopulateFiltersListBoxItems(ListBox);
		}
		void SetSelectAllCheckState() {
			CheckState state = ListBox.SelectedIndices.Count == 0 ? CheckState.Unchecked : CheckState.Indeterminate;
			if(ListBox.SelectedIndices.Count == ListBox.Items.Count)
				state = CheckState.Checked;
			SelectAllCheckBox.CheckState = state;
		}
		void CreateSeparator() {
			SeparatorCell = CreateCell();
			SeparatorCell.Controls.Add(RenderUtils.CreateDiv());
		}
		Table CreateTable() {
			Table table = RenderUtils.CreateTable();
			table.Width = Unit.Percentage(100);
			Controls.Add(table);
			return table;
		}
		TableCell CreateCell() {
			TableRow row = new InternalTableRow();
			Table.Rows.Add(row);
			TableCell cell = RenderUtils.CreateTableCell();
			row.Cells.Add(cell);
			return cell;
		}
		protected override void PrepareControlHierarchy() {
			if(IsCheckedList) {
				SelectAllCheckBox.CheckBoxStyle.Assign(Grid.StylesEditors.CheckBox);
				SelectAllCheckBox.CheckBoxFocusedStyle.Assign(Grid.StylesEditors.CheckBoxFocused);
				SelectAllCheckBox.CheckedImage.Assign(Grid.ImagesEditors.CheckBoxChecked);
				SelectAllCheckBox.UncheckedImage.Assign(Grid.ImagesEditors.CheckBoxUnchecked);
				SelectAllCheckBox.GrayedImage.Assign(Grid.ImagesEditors.CheckBoxGrayed);
				SelectAllCheckBox.Text = Grid.SettingsText.GetHeaderFilterSelectAll();
				RenderHelper.GetHFSelectAllCellStyle().AssignToControl(SelectAllCell);
				SeparatorCell.CssClass = Grid.Styles.HFSeparatorCellClassName;
			}
			PrepareListBoxHierarchy();
		}
		void PrepareListBoxHierarchy() {
			ListBox.Width = Unit.Percentage(100);
			ListBox.Height = Unit.Percentage(100);
			ListBox.Border.BorderStyle = BorderStyle.None;
			ListBox.CheckBoxStyle.Assign(Grid.StylesEditors.CheckBox);
			ListBox.CheckBoxFocusedStyle.Assign(Grid.StylesEditors.CheckBoxFocused);
			ListBox.CheckBoxCheckedImage.Assign(Grid.ImagesEditors.CheckBoxChecked);
			ListBox.CheckBoxUncheckedImage.Assign(Grid.ImagesEditors.CheckBoxUnchecked);
			ListBox.ItemStyle.CopyFrom(RenderHelper.GetHeaderFilterPopupItemStyle());
			ListBox.ClientSideEvents.Init = RenderHelper.Scripts.GetHFInitHandler(Grid.GetColumnGlobalIndex(Column));
			ListBoxCell.CssClass = Grid.Styles.HFListBoxCellClassName;
		}
	}
	public class GridHtmlFilterByDateRangeContainer : GridHtmlFilterContainer {
		public GridHtmlFilterByDateRangeContainer(IWebGridDataColumn column, bool includeFilteredOut)
			: base(column, includeFilteredOut) {
		}
		protected GridDataColumnHeaderFilterSettings SettingsHeaderFilter { get { return Column.Adapter.SettingsHeaderFilter; } }
		protected bool ShowCalendar { get { return SettingsHeaderFilter.Mode == GridHeaderFilterMode.DateRangeCalendar; } }
		protected ASPxCalendar Calendar { get; private set; }
		protected ASPxDateEdit FromDateEdit { get; private set; }
		protected ASPxDateEdit ToDateEdit { get; private set; }
		protected ASPxCheckBoxList ListBox { get; private set; }
		protected override void CreateControlHierarchy() {
			Controls.Add(CreateCustomDateRangeContainer());
			ListBox = CreatePredefinedDateRangesListBox();
			Controls.Add(ListBox);
		}
		protected Control CreateCustomDateRangeContainer() {
			if(ShowCalendar)
				return Calendar = CreateCalendar();
			var container = new Control();
			FromDateEdit = CreateDateRangePickerEditor(GridRenderHelper.HeaderFilterFromDateEditID);
			ToDateEdit = CreateDateRangePickerEditor(GridRenderHelper.HeaderFilterToDateEditID);
			container.Controls.Add(FromDateEdit);
			container.Controls.Add(ToDateEdit);
			return container;
		}
		ASPxCalendar CreateCalendar() {
			var calendar = new ASPxCalendar();
			calendar.ID = GridRenderHelper.HeaderFilterCalendarID;
			calendar.ParentSkinOwner = Grid;
			return calendar;
		}
		ASPxDateEdit CreateDateRangePickerEditor(string id) {
			var dateEdit = new ASPxDateEdit();
			dateEdit.ParentSkinOwner = Grid;
			dateEdit.ID = id;
			return dateEdit;
		}
		protected ASPxCheckBoxList CreatePredefinedDateRangesListBox() {
			var listBox = new ASPxCheckBoxList();
			listBox.ID = GridRenderHelper.HeaderFilterListBoxID;
			listBox.RepeatColumns = 2;
			listBox.ParentSkinOwner = Grid;
			listBox.EnableClientSideAPI = true;
			listBox.EncodeHtml = false;
			return listBox;
		}
		protected override void PrepareControlHierarchy() {
			if(Calendar != null)
				PrepareCalendar();
			if(FromDateEdit != null && ToDateEdit != null)
				PrepareDateRangeEditors();
			PrepareListBoxHierarchy();
		}
		void PrepareCalendar() {
			SettingsHeaderFilter.DateRangeCalendarSettings.AssignToControl(Calendar);
			RenderUtils.AppendDefaultDXClassName(Calendar, Grid.Styles.HFDRCalendarClassName);
			foreach(var range in HeaderFilterHelper.GetActiveDateRanges()) {
				for(DateTime date = range.Item1; date < range.Item2; date = date.AddDays(1)) {
					Calendar.SelectedDates.Add(date);
				}
			}
		}
		void PrepareDateRangeEditors() {
			FromDateEdit.Properties.DisplayFormatString = SettingsHeaderFilter.DateRangePickerSettings.DisplayFormatString;
			FromDateEdit.Properties.Caption = "From";
			FromDateEdit.Properties.RootStyle.CssClass = RenderUtils.CombineCssClasses(FromDateEdit.Properties.RootStyle.CssClass, Grid.Styles.HFDRDateRangePickerClassName);
			ToDateEdit.Properties.DisplayFormatString = SettingsHeaderFilter.DateRangePickerSettings.DisplayFormatString;
			ToDateEdit.Properties.DateRangeSettings.MinDayCount = SettingsHeaderFilter.DateRangePickerSettings.MinDayCount;
			ToDateEdit.Properties.DateRangeSettings.MaxDayCount = SettingsHeaderFilter.DateRangePickerSettings.MaxDayCount;
			ToDateEdit.Properties.DateRangeSettings.StartDateEditID = FromDateEdit.ID;
			ToDateEdit.Properties.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithTooltip;
			ToDateEdit.Properties.Caption = "To";
			ToDateEdit.Properties.RootStyle.CssClass = RenderUtils.CombineCssClasses(ToDateEdit.Properties.RootStyle.CssClass, Grid.Styles.HFDRDateRangePickerClassName);
			var range = HeaderFilterHelper.GetDateRangePickerBoundaries();
			if(range != null) {
				FromDateEdit.Value = range.Item1;
				if(range.Item2.HasValue)
					ToDateEdit.Value = ((DateTime)range.Item2).AddDays(-1);
			}
		}
		void PrepareListBoxHierarchy() {
			ListBox.Width = Unit.Percentage(100);
			ListBox.Border.BorderStyle = BorderStyle.None;
			ListBox.Paddings.PaddingTop = 0;
			ListBox.CheckBoxStyle.Assign(Grid.StylesEditors.CheckBox);
			ListBox.CheckBoxFocusedStyle.Assign(Grid.StylesEditors.CheckBoxFocused);
			ListBox.CheckedImage.Assign(Grid.ImagesEditors.CheckBoxChecked);
			ListBox.UncheckedImage.Assign(Grid.ImagesEditors.CheckBoxUnchecked);
			ListBox.ClientSideEvents.Init = RenderHelper.Scripts.GetHFInitHandler(Grid.GetColumnGlobalIndex(Column));
			ListBox.RepeatColumns = SettingsHeaderFilter.DateRangePeriodsSettings.RepeatColumns;
			PopulateFiltersListBoxItems(ListBox);
		}
	}
	public abstract class GridHtmlFilterContainer : ASPxInternalWebControl {
		public GridHtmlFilterContainer(IWebGridDataColumn column, bool includeFilteredOut) {
			Column = column;
			HeaderFilterHelper = new GridHeaderFilterHelper(column, includeFilteredOut);
		}
		protected IWebGridDataColumn Column { get; private set; }
		protected GridHeaderFilterHelper HeaderFilterHelper { get; private set; }
		protected ASPxGridBase Grid { get { return Column.Adapter.Grid; } }
		protected GridRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		protected virtual void PopulateFiltersListBoxItems(ASPxListEdit listEdit) {
			var items = listEdit.Items;
			items.Clear();
			foreach(FilterValue filterValue in HeaderFilterHelper.FilterValues) {
				items.Add(CreateItem(filterValue));
			}
			PrepareListBoxSeparators(listEdit);
		}
		protected void PrepareListBoxSeparators(ASPxListEdit listEdit) {
			if (HeaderFilterHelper.SeparatorIndeces.Count >= 0)
				listEdit.JSProperties["cpFSI"] = HeaderFilterHelper.SeparatorIndeces;
		}
		ListEditItem CreateItem(FilterValue filterValue) {
			ListEditItem item = new ListEditItem(filterValue.DisplayText, filterValue.HtmlValue);
			item.Selected = HeaderFilterHelper.IsFilterValueActive(filterValue);
			return item;
		}
	}
	public delegate object[] GetCommandColumnButtonClickHandlerArgs(string id, int visibleIndex);
	[ToolboxItem(false)]
	public class GridCommandColumnButtonControl : ASPxWebControlBase {
		#region Button adapter classes
		protected abstract class ButtonAdapter {
			const string
				ButtonIDPostfix = "DXCBtn",
				BatchEditButtonIDFormat = "{0}%DXItemIndex{1}%";
			public ButtonAdapter(ASPxGridBase grid) {
				Grid = grid;
			}
			public abstract string Text { get; }
			public abstract ImageProperties Image { get; }
			public abstract ButtonControlStyles Styles { get; }
			public abstract string ButtonID { get; }
			public abstract int VisibleIndex { get; }
			public abstract bool ButtonEnabled { get; }
			public abstract GridCommandButtonRenderMode ButtonRenderType { get; }
			public ASPxGridBase Grid { get; private set; }
			string buttonControlID;
			public string ButtonControlID {
				get {
					if(string.IsNullOrEmpty(buttonControlID))
						buttonControlID = GenerateButtonControlID();
					return buttonControlID;
				}
			}
			string GenerateButtonControlID() {
				int newButtonIndex = Grid.CommandButtonHelper.GetNewIndex();
				if(VisibleIndex == WebDataProxy.NewItemRow && Grid.RenderHelper.AllowBatchEditing)
					return string.Format(BatchEditButtonIDFormat, ButtonIDPostfix, newButtonIndex);
				return ButtonIDPostfix + newButtonIndex;
			}
		}
		class CommandButtonAdapter : ButtonAdapter {
			public CommandButtonAdapter(ASPxGridCommandButtonEventArgs args, ASPxGridBase grid)
				: base(grid) {
				Args = args;
			}
			protected ASPxGridCommandButtonEventArgs Args { get; private set; }
			public override string ButtonID { get { return string.Empty; } }
			public override ImageProperties Image { get { return Args.Image; } }
			public override ButtonControlStyles Styles { get { return Args.Styles; } }
			public override string Text { get { return Args.Text; } }
			public override int VisibleIndex { get { return Args.VisibleIndex; } }
			public override bool ButtonEnabled { get { return Args.Enabled; } }
			public override GridCommandButtonRenderMode ButtonRenderType { get { return Args.ButtonRenderType; } }
		}
		class CustomButtonAdapter : ButtonAdapter {
			public CustomButtonAdapter(ASPxGridCustomCommandButtonEventArgs args, ASPxGridBase grid)
				: base(grid) {
				Args = args;
			}
			protected ASPxGridCustomCommandButtonEventArgs Args { get; private set; }
			public override string ButtonID { get { return Args.ButtonID; } }
			public override ImageProperties Image { get { return Args.Image; } }
			public override ButtonControlStyles Styles { get { return Args.Styles; } }
			public override string Text { get { return Args.Text; } }
			public override int VisibleIndex { get { return Args.VisibleIndex; } }
			public override bool ButtonEnabled { get { return Args.Enabled; } }
			public override GridCommandButtonRenderMode ButtonRenderType { get { return Args.ButtonRenderType; } }
		}
		#endregion
		GetCommandColumnButtonClickHandlerArgs getClickHandlerArgs;
		ASPxCommandButton buttonControl;
		bool postponeClick;
		public GridCommandColumnButtonControl(ASPxGridCommandButtonEventArgs args, ASPxGridBase grid, GetCommandColumnButtonClickHandlerArgs getClickHandlerArgs, bool postponeClick)
			: this(new CommandButtonAdapter(args, grid), getClickHandlerArgs, postponeClick) {
		}
		public GridCommandColumnButtonControl(ASPxGridCustomCommandButtonEventArgs args, ASPxGridBase grid, GetCommandColumnButtonClickHandlerArgs getClickHandlerArgs, bool postponeClick)
			: this(new CustomButtonAdapter(args, grid), getClickHandlerArgs, postponeClick) {
		}
		protected GridCommandColumnButtonControl(ButtonAdapter adapter, GetCommandColumnButtonClickHandlerArgs getClickHandlerArgs, bool postponeClick) {
			AdapterInstance = adapter;
			this.getClickHandlerArgs = getClickHandlerArgs;
			this.postponeClick = postponeClick;
			EnableViewState = false;
		}
		protected ButtonAdapter AdapterInstance { get; private set; }
		protected GetCommandColumnButtonClickHandlerArgs GetClickHandlerArgs { get { return getClickHandlerArgs; } }
		protected int VisibleIndex { get { return AdapterInstance.VisibleIndex; } }
		protected ASPxGridBase Grid { get { return AdapterInstance.Grid; } }
		protected bool IsGridDesignMode { get { return Grid != null ? Grid.DesignMode : false; } }
		ASPxCommandButton ButtonControl { get { return buttonControl; } }
		protected bool PostponeClick { get { return postponeClick; } }
		protected override object SaveViewState() {
			return null;
		}
		protected override void CreateControlHierarchy() {
			this.buttonControl = new ASPxCommandButton();
			ButtonControl.ID = AdapterInstance.ButtonControlID;
			ButtonControl.RenderMode = AdapterInstance.ButtonRenderType == GridCommandButtonRenderMode.Button ? ButtonRenderMode.Button : ButtonRenderMode.Link;
			ButtonControl.ParentSkinOwner = Grid;
			if(AdapterInstance.ButtonRenderType != GridCommandButtonRenderMode.Image)
				ButtonControl.Text = GetButtonText();
			ButtonControl.Enabled = AdapterInstance.ButtonEnabled && Grid.RenderHelper.IsGridEnabled;
			ButtonControl.EncodeHtml = Grid.SettingsCommandButton.EncodeHtml;
			Controls.Add(ButtonControl);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			ButtonControl.RenderStyles.CopyFrom(AdapterInstance.Styles);
			ButtonControl.AddClickArguments(GetClickHandlerArgs(AdapterInstance.ButtonID, VisibleIndex), PostponeClick ? 1 : 0);
			Grid.CommandButtonHelper.CommandButtonClientIDList.Add(ButtonControl.ClientID);
			PrepareImageControl();
		}
		protected void PrepareImageControl() {
			ButtonControl.Image.CopyFrom(AdapterInstance.Image);
			if(AdapterInstance.ButtonRenderType == GridCommandButtonRenderMode.Image)
				ButtonControl.Font.Size = FontUnit.Empty;
			if(AdapterInstance.ButtonRenderType == GridCommandButtonRenderMode.Image && ButtonControl.Image.IsEmpty)
				ButtonControl.Image.Url = EmptyImageProperties.GetGlobalEmptyImage(Page).Url;
			if(string.IsNullOrEmpty(ButtonControl.Image.ToolTip))
				ButtonControl.Image.ToolTip = GetButtonText();
			if(string.IsNullOrEmpty(ButtonControl.Image.AlternateText))
				ButtonControl.Image.AlternateText = AdapterInstance.Text;
		}
		protected internal void AssignInnerControlStyle(AppearanceStyleBase style) {
			if(ButtonControl != null)
				style.AssignToControl(ButtonControl, true);
		}
		protected internal void AssignInnerControlCssClass(string cssClass) {
			RenderUtils.AppendDefaultDXClassName(ButtonControl, cssClass);
		}
		protected string GetButtonText() {
			string result = AdapterInstance.Text;
			if(!ButtonControl.Enabled && AdapterInstance.ButtonRenderType == GridCommandButtonRenderMode.Link)
				result += "&nbsp;";
			return result;
		}
	}
	public abstract class GridCommandButtonsCell : GridTableCell {
		public GridCommandButtonsCell(GridRenderHelper renderHelper, IEnumerable<GridCommandButtonType> buttonTypes)
			: base(renderHelper) {
			ButtonTypes = new List<GridCommandButtonType>(buttonTypes);
		}
		protected ASPxGridBase Grid { get { return RenderHelper.Grid; } }
		protected List<GridCommandButtonType> ButtonTypes { get; private set; }
		protected WebControl ButtonsContainer { get; private set; }
		protected override void CreateControlHierarchy() {
			CreateButtonsContainer();
			for(int i = 0; i < ButtonTypes.Count; i++)
				CreateCommandButton(ButtonTypes[i], i == ButtonTypes.Count - 1);
		}
		protected void CreateButtonsContainer() {
			ButtonsContainer = this;
			if(RenderUtils.IsHtml5Mode(this)) {
				ButtonsContainer = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
				Controls.Add(ButtonsContainer);
			}
		}
		protected virtual void CreateCommandButton(GridCommandButtonType buttonType, bool isLast) {
			var buttonControl = CreateButtonControl(buttonType);
			if(buttonControl == null)
				return;
			ButtonsContainer.Controls.Add(buttonControl);
			if(!isLast && !ItemSpacing.IsEmpty)
				ButtonsContainer.Controls.Add(new GridCommandColumnSpacer(ItemSpacing));
		}
		protected abstract GridCommandColumnButtonControl CreateButtonControl(GridCommandButtonType buttonType);
		protected abstract Unit ItemSpacing { get; }
		protected override void PrepareControlHierarchy() {
			var style = RenderHelper.GetCommandItemsCellStyle();
			style.AssignToControl(this, true);
			HorizontalAlign = GetHorizontalAlign();
			base.PrepareControlHierarchy();
		}
		protected virtual HorizontalAlign GetHorizontalAlign() {
			return RenderHelper.IsRightToLeft ? HorizontalAlign.Left : HorizontalAlign.Right;
		}
	}
	public class GridCommandColumnSpacer : InternalWebControl {
		public GridCommandColumnSpacer(Unit width)
			: base(HtmlTextWriterTag.Span) {
			RenderUtils.SetHorizontalMargins(this, Unit.Empty, width);
			var fontSize = RenderUtils.Browser.IsFirefox ? 2 : 1;
			Font.Size = FontUnit.Parse(fontSize + "px");
			Controls.Add(RenderUtils.CreateLiteralControl("&nbsp;"));
		}
	}
}
