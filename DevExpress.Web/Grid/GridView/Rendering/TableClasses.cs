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
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Data;
using DevExpress.Web.Rendering;
using DevExpress.Web.Localization;
using DevExpress.Utils;
using DevExpress.Web.Internal.InternalCheckBox;
namespace DevExpress.Web.Rendering {
	public abstract class GridHtmlHeaderContent : InternalTable {
		GridHeaderStyle headerStyle;
		public GridHtmlHeaderContent(GridRenderHelper renderHelper, IWebGridColumn column, GridHeaderLocation location) {
			Column = column;
			Location = location;
			RenderHelper = renderHelper;
		}
		public IWebGridColumn Column { get; private set; }
		public GridHeaderLocation Location { get; private set; }
		public GridRenderHelper RenderHelper { get; private set; }
		public ASPxGridBase Grid { get { return RenderHelper.Grid; } }
		protected abstract bool ShowFilterButton { get; }
		protected abstract bool IsFilterActive { get; }
		protected DevExpress.Data.ColumnSortOrder ColumnSortOrder {
			get {
				var dataColumn = Column as IWebGridDataColumn;
				return dataColumn != null ? dataColumn.SortOrder : DevExpress.Data.ColumnSortOrder.None;
			}
		}
		protected bool ShowSortImage { get { return ColumnSortOrder != DevExpress.Data.ColumnSortOrder.None; } }
		protected TableRow MainRow { get; private set; }
		protected TableCell TextCell { get; private set; }
		protected TableCell ImagesCell { get; private set; }
		protected Image SortImage { get; private set; }
		protected Image FilterImage { get; private set; }
		protected WebControl ImageAlignControl { get; private set; }
		protected virtual bool IsTextCellHasControls { get { return false; } }
		protected GridHeaderStyle HeaderStyle {
			get {
				if(headerStyle == null)
					headerStyle = RenderHelper.GetHeaderStyle(Column);
				return headerStyle;
			}
		}
		protected override void CreateControlHierarchy() {
			MainRow = RenderUtils.CreateTableRow();
			Rows.Add(MainRow);
			CreateTextCell();
			CreateImagesCell();
			CreateSortButton();
			CreateFilterButton();
		}
		protected virtual void CreateTextCell() {
			TextCell = RenderUtils.CreateTableCell();
			if(Column.GetAllowEllipsisInText()) {
				var mainRowCell = RenderUtils.CreateTableCell();
				MainRow.Cells.Add(mainRowCell);
				var table = RenderUtils.CreateTable();
				var row = RenderUtils.CreateTableRow();
				RenderUtils.PutInControlsSequentially(mainRowCell, table, row, TextCell);
			} else {
				MainRow.Cells.Add(TextCell);
			}
			CreateTextCellContent();
		}
		protected virtual void CreateTextCellContent() {
			var text = RenderHelper.TextBuilder.GetHeaderCaption(Column);
			if(string.IsNullOrEmpty(text)) {
				TextCell.Text = "&nbsp;";
				return;
			}
			var dataColumn = Column as GridViewDataColumn;
			if(Grid.IsAccessibilityCompliantRender(true) && dataColumn != null && dataColumn.GetAllowSort())
				CreateAccesibleCaptionLink(text);
			else
				TextCell.Text = text;
		}
		protected void CreateImagesCell() {
			if(IsTextCellHasControls)
				return;
			ImagesCell = RenderUtils.CreateTableCell();
			MainRow.Cells.Add(ImagesCell);
			ImageAlignControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			ImagesCell.Controls.Add(ImageAlignControl);
			ImageAlignControl.Controls.Add(RenderUtils.CreateLiteralControl("&nbsp;"));
		}
		protected void CreateAccesibleCaptionLink(string text) {
			var link = RenderUtils.CreateHyperLink();
			TextCell.Controls.Add(link);
			link.Text = text;
			link.NavigateUrl = string.Format("javascript:{0}", RenderHelper.Scripts.GetAccessibleSortClick(Grid.GetColumnGlobalIndex(Column)));
			RenderUtils.SetStringAttribute(link, "onmousedown", GridViewRenderHelper.CancelBubbleJs);
			if(RenderUtils.IsHtml5Mode(this) && Grid.IsAccessibilityCompliantRender()) {
				string label = string.Format(AccessibilityUtils.GridViewHeaderLinkFormatString, text);
				if((Column as GridViewDataColumn).SortOrder != DevExpress.Data.ColumnSortOrder.None)
					label += AccessibilityUtils.GetStringSortOrder((Column as GridViewDataColumn).SortOrder);
				RenderUtils.SetStringAttribute(link, "aria-label", label);
			}
		}
		protected void CreateSortButton() {
			if(!ShowSortImage)
				return;
			SortImage = RenderUtils.CreateImage();
			ImagesCell.Controls.Add(SortImage);
		}
		protected void CreateFilterButton() {
			if(!ShowFilterButton)
				return;
			FilterImage = RenderUtils.CreateImage();
			ImagesCell.Controls.Add(FilterImage);
		}
		protected override void PrepareControlHierarchy() {
			CellPadding = 0;
			CellSpacing = 0;
			Width = Unit.Percentage(100);
			if(RenderHelper.AllowColumnResizing && RenderUtils.Browser.IsIE)
				Style[HtmlTextWriterStyle.TextOverflow] = "ellipsis";
			PrepareTextCell();
			PrepareImagesCell();
			PrepareSortImage();
			PrepareFilterImage();
		}
		protected virtual void PrepareTextCell() {
			HeaderStyle.AssignToControl(TextCell, AttributesRange.Font);
			HeaderStyle.AssignToControl(TextCell, AttributesRange.Cell);
			RenderUtils.SetPaddings(TextCell, HeaderStyle.Paddings);
			if(IsTextCellHasControls)
				TextCell.HorizontalAlign = HorizontalAlign.Center;
			if(Location == GridHeaderLocation.Group && HeaderStyle.Paddings.GetPaddingLeft().IsEmpty)
				RenderUtils.SetStyleUnitAttribute(TextCell, "padding-left", Unit.Pixel(1));
			if(Location == GridHeaderLocation.Customization) {
				HorizontalAlign popupContentAlign = RenderHelper.GetCustomizationWindowContentStyle().HorizontalAlign;
				if(popupContentAlign != HorizontalAlign.NotSet)
					RenderUtils.SetHorizontalAlign(TextCell, popupContentAlign);
			}
			RenderUtils.AllowEllipsisInText(TextCell, Column.GetAllowEllipsisInText());
			if(Column.GetAllowEllipsisInText()) {
				var textCellTable = (WebControl)TextCell.Parent.Parent;
				textCellTable.Style["table-layout"] = "fixed";
				textCellTable.Width = Unit.Percentage(100);
			}
		}
		protected void PrepareImagesCell() {
			if(ImagesCell == null)
				return;
			RenderUtils.SetPaddings(ImagesCell, HeaderStyle.Paddings);
			ImagesCell.Style[HtmlTextWriterStyle.TextAlign] = RenderHelper.IsRightToLeft ? "left" : "right";
			ImagesCell.Width = 1;
			if(ImageAlignControl != null)
				RenderUtils.SetVerticalAlignClass(ImageAlignControl, HeaderStyle.VerticalAlign);
		}
		protected void PrepareSortImage() {
			if(SortImage == null)
				return;
			RenderHelper.GetHeaderSortImage(ColumnSortOrder).AssignToControl(SortImage, Grid.DesignMode);
			RenderUtils.SetVerticalAlignClass(SortImage, HeaderStyle.VerticalAlign);
			RenderUtils.SetStyleUnitAttribute(SortImage, RenderHelper.IsRightToLeft ? "maring-right" : "margin-left", HeaderStyle.GetSortingImageSpacing());
			if(FilterImage != null)
				RenderUtils.SetStyleUnitAttribute(SortImage, RenderHelper.IsRightToLeft ? "margin-left" : "margin-right", HeaderStyle.GetFilterImageSpacing());
		}
		protected void PrepareFilterImage() {
			if(FilterImage == null)
				return;
			string imageName = IsFilterActive ? GridViewImages.HeaderFilterActiveName : GridViewImages.HeaderFilterName;
			RenderHelper.AssignImageToControl(imageName, FilterImage);
			if(IsEnabled)
				FilterImage.CssClass = RenderUtils.CombineCssClasses(FilterImage.CssClass, GridViewRenderHelper.HeaderFilterButtonClassName);
			RenderUtils.SetCursor(FilterImage, RenderUtils.GetDefaultCursor());
			RenderUtils.SetVerticalAlignClass(FilterImage, HeaderStyle.VerticalAlign);
		}
	}
	public class GridViewHtmlHeaderContent : GridHtmlHeaderContent, IInternalCheckBoxOwner {
		GridCommandColumnButtonControl newButtonControl;
		InternalCheckboxControl selectAllCheckbox;
		public GridViewHtmlHeaderContent(GridViewColumn column, GridHeaderLocation location)
			: base(column.Grid.RenderHelper, column, location) {
		}
		public new GridViewColumn Column { get { return (GridViewColumn)base.Column; } }
		public new ASPxGridView Grid { get { return (ASPxGridView)base.Grid; } }
		public new GridViewRenderHelper RenderHelper { get { return (GridViewRenderHelper)base.RenderHelper; } }
		protected WebDataProxy DataProxy { get { return RenderHelper.DataProxy; } }
		protected override bool ShowFilterButton { get { return Column.GetHasFilterButton(); } }
		protected override bool IsFilterActive { get { return Column.GetIsFiltered(); } }
		protected GridViewSelectAllCheckBoxMode SelectAllButtonMode {
			get {
				var commandColumn = Column as GridViewCommandColumn;
				return commandColumn != null ? commandColumn.SelectAllCheckboxMode : GridViewSelectAllCheckBoxMode.None;
			}
		}
		protected GridCommandColumnButtonControl NewButtonControl { get { return newButtonControl; } }
		protected InternalCheckboxControl SelectAllCheckbox { get { return selectAllCheckbox; } }
		protected override bool IsTextCellHasControls { get { return SelectAllCheckbox != null || NewButtonControl != null; } }
		protected override void CreateTextCellContent() {
			if(RenderHelper.AddHeaderCaptionTemplateControl(Column, TextCell, Location))
				return;
			if(AddNewCommandButton() | AddSelectAllCheckbox())
				return;
			base.CreateTextCellContent();
		}
		protected bool AddSelectAllCheckbox() {
			var commandColumn = Column as GridViewCommandColumn;
			if(!RenderHelper.HasAnySelectAllCheckbox || commandColumn == null || commandColumn.SelectAllCheckboxMode == GridViewSelectAllCheckBoxMode.None)
				return false;
			this.selectAllCheckbox = new InternalCheckboxControl(this);
			SelectAllCheckbox.Enabled = IsSelectAllCheckboxEnabled();
			TextCell.Controls.Add(SelectAllCheckbox);
			return true;
		}
		bool IsSelectAllCheckboxEnabled() {
			if(SelectAllButtonMode != GridViewSelectAllCheckBoxMode.Page)
				return true;
			int startIndex = DataProxy.VisibleStartIndex;
			int endIndex = startIndex + RenderHelper.DataProxy.VisibleRowCountOnPage - 1;
			for(int visibleIndex = startIndex; visibleIndex <= endIndex; visibleIndex++) {
				if(DataProxy.GetRowType(visibleIndex) == WebRowType.Data)
					return true;
			}
			return false;
		}
		protected bool AddNewCommandButton() {
			var commandColumn = Column as GridViewCommandColumn;
			if(commandColumn != null && commandColumn.ShowNewButtonInHeader) {
				this.newButtonControl = RenderHelper.CreateCommandButtonControl(commandColumn, GridCommandButtonType.New, -1, true);
				if(NewButtonControl != null)
					TextCell.Controls.Add(NewButtonControl);
			}
			return NewButtonControl != null;
		}
		protected override void PrepareTextCell() {
			base.PrepareTextCell();
			if(NewButtonControl != null) {
				GridViewCommandColumnStyle itemStyle = RenderHelper.GetCommandColumnItemStyle(Column);
				NewButtonControl.AssignInnerControlStyle(itemStyle);
			}
			if(SelectAllCheckbox != null && !SelectAllCheckbox.Enabled)
				SelectAllCheckbox.MainElement.CssClass = RenderUtils.CombineCssClasses(SelectAllCheckbox.MainElement.CssClass, "dxgv_cd");
		}
		Dictionary<string, string> GetAccessibilityCheckBoxAttributes() {
			if(!(RenderUtils.IsHtml5Mode(this) && Grid.IsAccessibilityCompliantRender()))
				return null;
			return AccessibilityUtils.CreateCheckBoxAttributes("checkbox", (this as IInternalCheckBoxOwner).CheckState);
		}
		#region IInternalCheckBoxOwner Members
		bool IInternalCheckBoxOwner.IsInputElementRequired { get { return true; } }
		bool IInternalCheckBoxOwner.ClientEnabled { get { return SelectAllCheckbox.Enabled; } }
		AppearanceStyleBase IInternalCheckBoxOwner.InternalCheckBoxStyle { get { return RenderHelper.GetCheckBoxStyle(); } }
		CheckState IInternalCheckBoxOwner.CheckState {
			get {
				if(!SelectAllCheckbox.Enabled)
					return CheckState.Unchecked;
				if(SelectAllButtonMode == GridViewSelectAllCheckBoxMode.Page)
					return RenderHelper.DataProxy.Selection.IsSelectedAllRowsOnPage(RenderHelper.DataProxy.PageIndex);
				return RenderHelper.DataProxy.Selection.IsSelectedAllRows();
			}
		}
		string IInternalCheckBoxOwner.GetCheckBoxInputID() {
			return GridViewRenderHelper.SelectAllButtonID + RenderHelper.GetColumnGlobalIndex(Column);
		}
		InternalCheckBoxImageProperties IInternalCheckBoxOwner.GetCurrentCheckableImage() {
			return RenderHelper.GetCheckImage(((IInternalCheckBoxOwner)this).CheckState, false);
		}
		Dictionary<string, string> IInternalCheckBoxOwner.AccessibilityCheckBoxAttributes { get { return GetAccessibilityCheckBoxAttributes(); } }
		#endregion
	}
	public class GridTableCell : InternalTableCell {
		public GridTableCell(GridRenderHelper renderHelper)
			: this(renderHelper, false, false) {
		}
		public GridTableCell(GridRenderHelper renderHelper, bool removeLeftBorder, bool removeRightBorder) {
			RenderHelper = renderHelper;
			RemoveLeftBorder = removeLeftBorder;
			RemoveRightBorder = removeRightBorder;
		}
		protected GridRenderHelper RenderHelper { get; private set; }
		protected ASPxGridScripts Scripts { get { return RenderHelper.Scripts; } }
		protected string GridClientID { get { return RenderHelper.Grid.ClientID; } }
		protected bool RemoveLeftBorder { get; private set; }
		protected bool RemoveRightBorder { get; private set; }
		protected internal bool RemoveBottomBorder { get; set; }
		protected virtual bool GetRemoveLeftBorder() {
			return RemoveLeftBorder;
		}
		protected virtual bool GetRemoveRightBorder() {
			return RemoveRightBorder && RenderHelper.AllowRemoveCellRightBorder;
		}
		protected virtual bool GetRemoveBottomBorder() {
			return RemoveBottomBorder;
		}
		protected override void PrepareControlHierarchy() {
			if(GetRemoveLeftBorder())
				RenderUtils.SetStyleUnitAttribute(this, "border-left-width", 0);
			if(GetRemoveRightBorder())
				RenderUtils.SetStyleUnitAttribute(this, "border-right-width", 0);
			if(GetRemoveBottomBorder())
				RenderUtils.SetStyleUnitAttribute(this, "border-bottom-width", 0);
			if(HorizontalAlign == HorizontalAlign.NotSet && RenderHelper.IsRightToLeft && RenderHelper.RequireFixedTableLayout)
				HorizontalAlign = HorizontalAlign.Right;
		}
	}
	[ViewStateModeById]
	public abstract class GridViewTableRow : InternalTableRow {
		GridViewRenderHelper renderHelper;
		int visibleIndex;
		bool hasGroupFooter;
		public GridViewTableRow(GridViewRenderHelper renderHelper, int visibleIndex)
			: this(renderHelper, visibleIndex, false) {
		}
		public GridViewTableRow(GridViewRenderHelper renderHelper, int visibleIndex, bool hasGroupFooter) {
			this.renderHelper = renderHelper;
			this.visibleIndex = visibleIndex;
			this.hasGroupFooter = hasGroupFooter;
		}
		public ASPxGridView Grid { get { return RenderHelper.Grid; } }
		public int VisibleIndex { get { return visibleIndex; } }
		public abstract GridViewRowType RowType { get; }
		protected virtual bool RequireRenderParentRows { get { return false; } }
		protected bool HasParentRows { get { return RequireRenderParentRows && VisibleIndex > -1 && VisibleIndex == DataProxy.VisibleStartIndex && DataProxy.HasParentRows; } }
		protected GridViewRenderHelper RenderHelper { get { return renderHelper; } }
		protected IList<GridViewColumnVisualTreeNode> LeafColumns { get { return Grid.ColumnHelper.Leafs; } }
		protected WebDataProxy DataProxy { get { return RenderHelper.DataProxy; } }
		protected int GroupSpanCount { get { return RenderHelper.GroupCount; } }
		protected virtual int ColumIndentCount { get { return GroupSpanCount; } }
		protected virtual bool RenderDetailIndent { get { return RenderHelper.HasDetailButton; } }
		protected virtual bool RenderAdaptiveDetailIndentOnTheLeft { get { return RenderHelper.HasAdaptiveDetailButtonOnTheLeft; } }
		protected virtual bool RenderAdaptiveDetailIndentOnTheRight { get { return RenderHelper.HasAdaptiveDetailButtonOnTheRight; } }
		protected virtual void CreateLeftIndentCells() {
			if(ColumIndentCount > 0) {
				int startIndex = 0;
				if(HasParentRows && !RenderHelper.UseEndlessPaging && !RenderHelper.UseFixedGroups) {
					Cells.Add(new GridViewTableInvisibleParentsRowDataCell(RenderHelper, VisibleIndex));
					startIndex++;
				}
				AddGroupIndentCellsCore(startIndex);
			}
			if(RenderAdaptiveDetailIndentOnTheLeft)
				Cells.Add(CreateAdaptiveDetailButtonCell());
			if(RenderDetailIndent)
				Cells.Add(CreateDetailButtonCell());
		}
		protected virtual void CreateRightIndentCells() {
			if(RenderAdaptiveDetailIndentOnTheRight)
				Cells.Add(CreateAdaptiveDetailButtonCell());
		}
		protected virtual void AddGroupIndentCellsCore(int startIndex) {
			for(int i = startIndex; i < ColumIndentCount; i++)
				Cells.Add(CreateIndentTableCell());
		}
		protected virtual TableCell CreateAdaptiveDetailButtonCell() {
			return new GridViewTableNoColumnsAdaptiveCell(RenderHelper);
		}
		protected virtual TableCell CreateDetailButtonCell() {
			TableCell cell = new GridViewTableNoColumnsCell(RenderHelper);
			RenderUtils.AppendDefaultDXClassName(cell, GridViewStyles.GridDetailIndentCellCssClass);
			return cell;
		}
		protected virtual TableCell CreateIndentTableCell() {
			return new GridViewTableIndentCell(RenderHelper);
		}
		protected bool HasGroupFooter { get { return hasGroupFooter; } }
		public virtual bool RemoveExtraIndentBottomBorder() { return false; }
	}
	[ViewStateModeById]
	public class GridViewTableHeaderRow : InternalTableRow {
		GridViewRenderHelper renderHelper;
		int layoutLevel;
		public GridViewTableHeaderRow(GridViewRenderHelper renderHelper, int layoutLevel) {
			this.renderHelper = renderHelper;
			this.layoutLevel = layoutLevel;
		}
		protected GridViewRenderHelper RenderHelper { get { return renderHelper; } }
		protected GridViewColumnHelper ColumnHelper { get { return RenderHelper.ColumnHelper; } }
		protected internal int LayoutLevel { get { return layoutLevel; } }
		protected List<List<GridViewColumnVisualTreeNode>> Layout { get { return ColumnHelper.Layout; } }
		protected int GroupSpanCount { get { return RenderHelper.GroupCount; } }
		protected override void CreateControlHierarchy() {
			ID = GridViewRenderHelper.HeaderRowID + LayoutLevel;
			if(ColumnHelper.Leafs.Count == 0) {
				Cells.Add(new GridViewTableEmptyHeaderCell(RenderHelper));
				return;
			}
			if(LayoutLevel == 0) {
				for(int i = 0; i < GroupSpanCount; i++)
					AddEmptyHeaderCell(false, false);
				if(RenderHelper.HasAdaptiveDetailButtonOnTheLeft)
					AddEmptyHeaderCell(true, true);
				if(RenderHelper.HasDetailButton)
					AddEmptyHeaderCell(true, false);
			}
			foreach(GridViewColumnVisualTreeNode node in Layout[LayoutLevel])
				AddHeaderCell(node, ShouldRemoveRightBorder(node));
			if(LayoutLevel == 0 && RenderHelper.HasAdaptiveDetailButtonOnTheRight)
				AddEmptyHeaderCell(true, true);
			RenderHelper.AddHorzScrollExtraCell(this);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(RenderUtils.Browser.Platform.IsMSTouchUI)
				RenderUtils.AppendDefaultDXClassName(this, RenderHelper.Styles.MSTouchDraggableMarkerCssClassName);
		}
		protected bool ShouldRemoveRightBorder(GridViewColumnVisualTreeNode node) {
			GridViewColumnVisualTreeNode current = node;
			while(current != null && current.Parent != null) {
				List<GridViewColumnVisualTreeNode> children = current.Parent.ChildrenEx;
				int childIndex = RenderHelper.IsRightToLeft ? 0 : children.Count - 1;
				if(children[childIndex] != current)
					return false;
				current = current.Parent;
			}
			return true;
		}
		protected virtual void AddHeaderCell(GridViewColumnVisualTreeNode node, bool removeRightBorder) {
			GridTableHeaderCell cell = new GridViewTableHeaderCell(RenderHelper, node.Column, GridHeaderLocation.Row, true, removeRightBorder);
			Cells.Add(cell);
			if(node.ColSpan > 1)
				cell.ColumnSpan = node.ColSpan;
			if(node.RowSpan > 1)
				cell.RowSpan = node.RowSpan;
		}
		protected virtual void AddEmptyHeaderCell(bool detailIndent, bool isAdaptive) {
			GridViewTableHeaderIndentCell cell = new GridViewTableHeaderIndentCell(RenderHelper, detailIndent);
			Cells.Add(cell);
			if(Layout.Count > 1)
				cell.RowSpan = Layout.Count;
			if(isAdaptive)
				RenderHelper.AppendAdaptiveIndentCellCssClassName(cell);
		}
	}
	public class GridViewTableEmptyHeaderCell : GridTableCell {
		public GridViewTableEmptyHeaderCell(GridViewRenderHelper renderHelper) : base(renderHelper) { }
		protected new GridViewRenderHelper RenderHelper { get { return (GridViewRenderHelper)base.RenderHelper; } }
		protected override void CreateControlHierarchy() {
			ID = GridViewRenderHelper.EmptyHeader;
			Text = RenderHelper.Grid.SettingsText.GetEmptyHeaders();
			ColumnSpan = RenderHelper.TotalSpanCount;
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetHeaderPanelStyle().AssignToControl(this, true);
			Width = RenderHelper.GetNarrowCellWidth();
			base.PrepareControlHierarchy();
		}
	}
	public class GridTableHeaderCellBase : GridTableCell {
		GridHeaderLocation location;
		public GridTableHeaderCellBase(GridRenderHelper renderHelper, GridHeaderLocation location, bool removeLeftBorder, bool removeRightBorder)
			: base(renderHelper, removeLeftBorder, removeRightBorder) {
			this.location = location;
		}
		public GridHeaderLocation Location { get { return location; } }
		protected virtual bool IsClickable { get { return false; } }
		protected virtual bool IsTopLevelHeaderCell {
			get {
				var parentRow = Parent as GridViewTableHeaderRow;
				return parentRow != null && parentRow.LayoutLevel == 0;
			}
		}
		protected override void PrepareControlHierarchy() {
			if(Location == GridHeaderLocation.Row) {
				if(!RenderHelper.RequireHeaderTopBorder || !IsTopLevelHeaderCell)
					RenderUtils.SetStyleUnitAttribute(this, "border-top-width", 0);
			}
			if(!IsClickable || !RenderHelper.IsGridEnabled) {
				Style[HtmlTextWriterStyle.Cursor] = "default";
			}
			base.PrepareControlHierarchy();
		}
	}
	public enum GridHeaderLocation { Row, Group, Customization }
	public class GridViewTableHeaderCell : GridTableHeaderCell {
		public GridViewTableHeaderCell(GridViewRenderHelper renderHelper, GridViewColumn column, GridHeaderLocation location, bool removeLeftBorder, bool removeRightBorder)
			: base(renderHelper, column, location, removeLeftBorder, removeRightBorder) {
		}
		public new GridViewColumn Column { get { return (GridViewColumn)base.Column; } }
		protected bool HasToSetWidth {
			get {
				if(Location == GridHeaderLocation.Customization)
					return false;
				if(RenderHelper.RequireFixedTableLayout)
					return false;
				if(!IsLeaf)
					return false;
				return true;
			}
		}
		protected override bool IsClickable { get { return Column.IsClickable(); } }
		protected override Control CreateGridHeaderContent() {
			return new GridViewHtmlHeaderContent(Column, Location);
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(!string.IsNullOrEmpty(Column.ToolTip))
				ToolTip = Column.ToolTip;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(HasToSetWidth)
				Width = Column.Width;
		}
	}
	public abstract class GridTableHeaderCell : GridTableHeaderCellBase {
		public GridTableHeaderCell(GridRenderHelper renderHelper, IWebGridColumn column, GridHeaderLocation location, bool removeLeftBorder, bool removeRightBorder)
			: base(renderHelper, location, removeLeftBorder, removeRightBorder) {
			Column = column;
		}
		public IWebGridColumn Column { get; private set; }
		protected WebControl GridHeaderContent { get; private set; }
		protected bool IsLeaf { get { return RenderHelper.ColumnHelper.IsLeaf(Column); } }
		protected override HtmlTextWriterTag TagKey {
			get { return RenderHelper.Grid.IsAccessibilityCompliantRender() ? HtmlTextWriterTag.Th : base.TagKey; }
		}
		protected abstract Control CreateGridHeaderContent();
		protected override void CreateControlHierarchy() {
			ID = GenerateID();
			if(RenderHelper.AddHeaderTemplateControl(Column, this, Location))
				return;
			GridHeaderContent = (WebControl)CreateGridHeaderContent();
			Controls.Add(GridHeaderContent);
		}
		string GenerateID() {
			string result = "col" + RenderHelper.GetColumnGlobalIndex(Column);
			if(Location == GridHeaderLocation.Group)
				result = "group" + result;
			return result;
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetHeaderStyle(Column).AssignToControl(this, true);
			Attributes.Add(TouchUtils.TouchMouseDownEventName, Scripts.GetHeaderColumnMouseDown());
			if(RenderHelper.Grid.IsAccessibilityCompliantRender()) {
				RenderUtils.SetStringAttribute(this, "scope", "col");
				if(!RenderHelper.Grid.IsAriaSupported())
					RenderUtils.SetStringAttribute(this, "abbr", RenderHelper.TextBuilder.GetHeaderCaption(Column));
				else
					RenderUtils.SetStringAttribute(GridHeaderContent, "role", "presentation");
			}
			base.PrepareControlHierarchy();
		}
	}
	public class GridViewTableHeaderIndentCell : GridTableHeaderCellBase {
		public GridViewTableHeaderIndentCell(GridViewRenderHelper renderHeler, bool isDetail)
			: base(renderHeler, GridHeaderLocation.Row, !renderHeler.IsRightToLeft, renderHeler.IsRightToLeft) {
			IsDetailIndent = isDetail;
		}
		protected new GridViewRenderHelper RenderHelper { get { return (GridViewRenderHelper)base.RenderHelper; } }
		protected bool IsDetailIndent { get; private set; }
		protected override void CreateControlHierarchy() {
			Image image = RenderUtils.CreateImage();
			Controls.Add(image);	
			image.AlternateText = "img";
			image.Style[HtmlTextWriterStyle.Visibility] = "hidden";
			var imageName = IsDetailIndent ? GridViewImages.DetailExpandedButtonName : GridViewImages.ExpandedButtonName;
			RenderHelper.AssignImageToControl(imageName, image);
			image.AlternateText = string.Empty;
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetHeaderStyle(null).AssignToControl(this);
			Width = RenderHelper.GetNarrowCellWidth();
			Style[HtmlTextWriterStyle.Cursor] = "default";
			base.PrepareControlHierarchy();
		}
	}
	public abstract class GridViewTableGroupAndDataRow : GridViewTableRow {
		public GridViewTableGroupAndDataRow(GridViewRenderHelper renderHelper, int visibleIndex, bool hasGroupFooter)
			: base(renderHelper, visibleIndex, hasGroupFooter) {
		}
		protected internal abstract AppearanceStyle GetRowStyle();
		protected internal abstract AppearanceStyle GetRegularRowStyle();
	}
	public class GridViewTableGroupRow : GridViewTableGroupAndDataRow {
		int groupLevel;
		bool isGroupButtonLive;
		Image fixedGroupRowImage;
		const string FixedGroupIconClassName = GridViewStyles.GridPrefix + "FGI";
		public GridViewTableGroupRow(GridViewRenderHelper renderHelper, int visibleIndex, bool hasGroupFooter)
			: this(renderHelper, visibleIndex, hasGroupFooter, true) { }
		public GridViewTableGroupRow(GridViewRenderHelper renderHelper, int visibleIndex, bool hasGroupFooter, bool isGroupButtonLive)
			: base(renderHelper, visibleIndex, hasGroupFooter) {
			this.groupLevel = DataProxy.GetRowLevel(VisibleIndex);
			this.isGroupButtonLive = isGroupButtonLive;
		}
		protected override bool RequireRenderParentRows { get { return true; } }
		public override GridViewRowType RowType { get { return GridViewRowType.Group; } }
		protected GridViewDataColumn Column { get { return Grid.SortedColumns[GroupLevel] as GridViewDataColumn; } }
		protected int ColumnSpanCount { get { return RenderHelper.TotalSpanCount; } }
		public int GroupLevel { get { return groupLevel; } }
		protected override int ColumIndentCount { get { return GroupLevel; } }
		protected bool IsGroupButtonLive { get { return isGroupButtonLive; } }
		protected bool IsExpanded { get { return DataProxy.IsRowExpanded(VisibleIndex); } }
		protected Image FixedGroupRowImage { get { return fixedGroupRowImage; } }
		protected override void CreateControlHierarchy() {
			ID = RenderHelper.GetGroupRowId(VisibleIndex);
			CreateLeftIndentCells();
			if(RenderHelper.AddGroupRowTemplateControl(VisibleIndex, Column, this, ColumnSpanCount - GroupLevel))
				return;
			if(Grid.Settings.ShowGroupButtons)
				CreateButtonCell();
			CreateContentCell();
			CreateRightIndentCells();
		}
		protected override bool RenderDetailIndent { get { return false; } }
		protected override bool RenderAdaptiveDetailIndentOnTheRight { get { return base.RenderAdaptiveDetailIndentOnTheRight || base.RenderAdaptiveDetailIndentOnTheLeft; } }
		protected override bool RenderAdaptiveDetailIndentOnTheLeft { get { return false; } }
		protected virtual void CreateButtonCell() {
			Cells.Add(new GridViewTableGroupButtonCell(RenderHelper, VisibleIndex, IsGroupButtonLive));
		}
		protected virtual int GetColSpanCount() {
			return ColumnSpanCount - GroupLevel - (Grid.Settings.ShowGroupButtons ? 1 : 0) - (RenderAdaptiveDetailIndentOnTheRight ? 1 : 0);
		}
		protected virtual void CreateContentCell() {
			TableCell res = new GridTableCell(RenderHelper, false, true);
			res.ColumnSpan = GetColSpanCount();
			RenderHelper.AppendGridCssClassName(res);
			Cells.Add(res);
			if(!RenderHelper.AddGroupRowContentTemplateControl(VisibleIndex, Column, res)) {
				var literalControl = RenderUtils.CreateLiteralControl();
				literalControl.Text = GetDisplayText();
				if(!DataProxy.IsGroupRowFitOnPage(VisibleIndex) && !RenderHelper.UseEndlessPaging)
					literalControl.Text += " " + Grid.SettingsText.GetGroupContinuedOnNextPage();
				res.Controls.Add(literalControl);
			}
			if(RenderHelper.UseFixedGroups) {
				this.fixedGroupRowImage = RenderUtils.CreateImage();
				res.Controls.Add(FixedGroupRowImage);
				res.Style[HtmlTextWriterStyle.Position] = "relative";
			}
		}
		protected override void PrepareControlHierarchy() {
			GetRowStyle().AssignToControl(this, false);
			PrepareFixedGroupRowImage();
			base.PrepareControlHierarchy();
			Grid.RaiseHtmlRowPrepared(this);
		}
		void PrepareFixedGroupRowImage() {
			if(FixedGroupRowImage == null)
				return;
			FixedGroupRowImage.CssClass = FixedGroupIconClassName;
			RenderHelper.AssignImageToControl(GridViewImages.FixedGroupRowName, FixedGroupRowImage);
			if(RenderHelper.DataProxy.VisibleStartIndex <= VisibleIndex)
				FixedGroupRowImage.Style[HtmlTextWriterStyle.Display] = "none";
		}
		protected virtual string GetDisplayText() {
			return RenderHelper.TextBuilder.GetGroupRowText(Column, VisibleIndex);
		}
		public override bool RemoveExtraIndentBottomBorder() {
			bool last = VisibleIndex == DataProxy.VisibleStartIndex + DataProxy.VisibleRowCountOnPage - 1;
			if(last && Grid.Settings.ShowFooter)
				return HasGroupFooter;
			return IsExpanded || HasGroupFooter;
		}
		protected internal override AppearanceStyle GetRegularRowStyle() {
			return RenderHelper.GetGroupRowStyle();
		}
		protected internal override AppearanceStyle GetRowStyle() {
			return GetRegularRowStyle();
		}
	}
	public class GridViewTableGroupFooterRow : GridViewTableRow {
		int groupLevel;
		bool isLastFooterRow;
		public GridViewTableGroupFooterRow(GridViewRenderHelper renderHelper, int visibleIndex, bool isLastFooterRow)
			: base(renderHelper, visibleIndex) {
			this.groupLevel = DataProxy.GetRowLevel(VisibleIndex);
			this.isLastFooterRow = isLastFooterRow;
		}
		protected int GroupLevel { get { return groupLevel; } }
		public override GridViewRowType RowType { get { return GridViewRowType.GroupFooter; } }
		protected GridViewDataColumn Column { get { return Grid.SortedColumns[GroupLevel] as GridViewDataColumn; } }
		protected int FooterIndentCount { get { return RenderHelper.GroupCount - GroupLevel - 1; } }
		protected bool HasGroupFooterTemplate { get { return Grid.Templates.GroupFooterRow != null; } }
		protected override void CreateControlHierarchy() {
			CreateLeftIndentCells();
			if(RenderHelper.AddGroupFooterRowTemplateControl(this, Column, GetTemplateCellSpanCount(), VisibleIndex))
				return;
			for(int i = 0; i < LeafColumns.Count; i++) {
				TableCell cell = CreateFooterCell(LeafColumns[i].Column, RenderHelper.ShouldRemoveLeftBorder(i), RenderHelper.ShouldRemoveRightBorder(i));
				Cells.Add(cell);
			}
			if(LeafColumns.Count == 0) {
				Cells.Add(new GridViewTableNoColumnsCell(RenderHelper));
			}
			CreateRightIndentCells();
		}
		protected override void AddGroupIndentCellsCore(int startIndex) {
			if(ColumIndentCount == 0)
				return;
			int dataIndentCount = ColumIndentCount - FooterIndentCount;
			for(int i = 0; i < dataIndentCount - 1; i++) {
				Cells.Add(CreateIndentTableCell());
			}
			Cells.Add(CreateGroupFooterIndentDataTableCell());
			if(FooterIndentCount > 0 && !HasGroupFooterTemplate) {
				Cells.Add(CreateGroupFooterIndentTableCell());
			}
		}
		protected virtual TableCell CreateFooterCell(GridViewColumn column, bool removeLeftBorder, bool removeRightBorder) {
			return new GridViewTableGroupFooterCell(RenderHelper, Column, column, VisibleIndex, removeLeftBorder, removeRightBorder);
		}
		protected virtual TableCell CreateGroupFooterIndentTableCell() {
			return new GridViewTableGroupFooterIndentCell(RenderHelper, FooterIndentCount);
		}
		protected virtual TableCell CreateGroupFooterIndentDataTableCell() {
			return new GridViewTableIndentCell(RenderHelper);
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetGroupFooterStyle().AssignToControl(this, false);
			Grid.RaiseHtmlRowPrepared(this);
		}
		protected bool IsLastFooterRow { get { return isLastFooterRow; } }
		public override bool RemoveExtraIndentBottomBorder() {
			return !IsLastFooterRow;
		}
		protected virtual int GetTemplateCellSpanCount() {
			int result = FooterIndentCount + LeafColumns.Count;
			if(LeafColumns.Count == 0)
				result++;
			return result;
		}
	}
	public class GridViewTableParentGroupRow : GridViewTableGroupRow {
		public GridViewTableParentGroupRow(GridViewRenderHelper renderHelper, int visibleIndex)
			: base(renderHelper, visibleIndex, true, false) { }
		protected override int GetColSpanCount() {
			return RenderHelper.GroupCount - GroupLevel;
		}
	}
	public class GridViewTableGroupButtonCell : GridTableCell {
		int visibleIndex;
		bool isGroupButtonLive;
		public GridViewTableGroupButtonCell(GridViewRenderHelper renderHelper, int visibleIndex, bool isGroupButtonLive)
			: base(renderHelper, false, true) {
			this.visibleIndex = visibleIndex;
			this.isGroupButtonLive = isGroupButtonLive;
		}
		public int VisibleIndex { get { return visibleIndex; } }
		protected new GridViewRenderHelper RenderHelper { get { return (GridViewRenderHelper)base.RenderHelper; } }
		protected new ASPxGridViewScripts Scripts { get { return (ASPxGridViewScripts)base.Scripts; } }
		protected bool IsGroupButtonLive { get { return isGroupButtonLive; } }
		protected WebDataProxy DataProxy { get { return RenderHelper.DataProxy; } }
		protected bool IsRowExpanded { get { return DataProxy.IsRowExpanded(VisibleIndex); } }
		protected bool Accessible { get { return RenderHelper.Grid.IsAccessibilityCompliantRender(true); } }
		protected override void CreateControlHierarchy() {
			Image image = RenderUtils.CreateImage();
			string imageName = string.Empty;
			if(IsRowExpanded)
				imageName = RenderHelper.IsRightToLeft ? GridViewImages.ExpandedButtonRtlName : GridViewImages.ExpandedButtonName;
			else
				imageName = RenderHelper.IsRightToLeft ? GridViewImages.CollapsedButtonRtlName : GridViewImages.CollapsedButtonName;
			RenderHelper.AssignImageToControl(imageName, image);
			HyperLink link = null;
			if(Accessible) {
				link = RenderUtils.CreateHyperLink();
				Controls.Add(link);
				link.Controls.Add(image);
			}
			else {
				Controls.Add(image);
			}
			if(IsGroupButtonLive) {
				string js = IsRowExpanded ? Scripts.GetCollapseRowFunction(VisibleIndex) : Scripts.GetExpandRowFunction(VisibleIndex);
				if(link != null) {
					link.NavigateUrl = string.Format("javascript:{0}", js);
					RenderUtils.SetStringAttribute(link, "onclick", GridViewRenderHelper.CancelBubbleJs);
				}
				else {
					RenderUtils.SetStringAttribute(image, "onclick", js + ";" + GridViewRenderHelper.CancelBubbleJs);
					if(RenderHelper.IsGridEnabled)
						RenderUtils.SetCursor(image, RenderUtils.GetPointerCursor());
				}
			}
		}
		protected override void PrepareControlHierarchy() {
			Width = RenderHelper.GetNarrowCellWidth();
			RenderHelper.AppendGridCssClassName(this);
			base.PrepareControlHierarchy();
		}
	}
	public class GridViewTablePreviewRow : GridViewTableRow {
		public GridViewTablePreviewRow(GridViewRenderHelper renderHelper, int visibleIndex, bool hasGroupFooter)
			: base(renderHelper, visibleIndex, hasGroupFooter) { }
		public override GridViewRowType RowType { get { return GridViewRowType.Preview; } }
		protected override void CreateControlHierarchy() {
			ID = RenderHelper.GetPreviewRowId(VisibleIndex);
			CreateLeftIndentCells();
			if(RenderHelper.AddPreviewRowTemplateControl(VisibleIndex, this, LeafColumns.Count))
				return;
			Cells.Add(CreatePreviewCell());
			CreateRightIndentCells();
		}
		protected virtual TableCell CreatePreviewCell() {
			TableCell cell = RenderUtils.CreateTableCell();
			cell.ColumnSpan = LeafColumns.Count;
			string text = RenderHelper.TextBuilder.GetPreviewText(VisibleIndex);
			if(string.IsNullOrEmpty(text))
				text = "&nbsp;";
			cell.Text = text;
			RenderHelper.AppendGridCssClassName(cell);
			return cell;
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetPreviewRowStyle().AssignToControl(this, false);
			base.PrepareControlHierarchy();
			Grid.RaiseHtmlRowPrepared(this);
		}
		public override bool RemoveExtraIndentBottomBorder() {
			return HasGroupFooter;
		}
	}
	public class GridViewTableEmptyDataRow : GridViewTableRow {
		GridCommandColumnButtonControl newButtonControl;
		WebControl textContainer;
		public GridViewTableEmptyDataRow(GridViewRenderHelper renderHelper) : base(renderHelper, -1) { }
		public override GridViewRowType RowType { get { return GridViewRowType.EmptyDataRow; } }
		protected GridCommandColumnButtonControl NewButtonControl { get { return newButtonControl; } }
		protected WebControl TextContainer { get { return textContainer; } }
		protected override void CreateControlHierarchy() {
			ID = GridViewRenderHelper.EmptyDataRowID;
			CreateLeftIndentCells();
			if(RenderHelper.AddEmptyDataRowTemplateControl(this, LeafColumns.Count))
				return;
			Cells.Add(CreateEmptyDataCell());
			CreateRightIndentCells();
			RenderHelper.AddHorzScrollExtraCell(this);
		}
		protected virtual TableCell CreateEmptyDataCell() {
			TableCell cell = new GridTableCell(RenderHelper);
			cell.ColumnSpan = LeafColumns.Count;
			var column = RenderHelper.FirstCommandColumnWithNewButton;
			if(column != null) {
				this.newButtonControl = RenderHelper.CreateCommandButtonControl(column, GridCommandButtonType.New, 0, true); 
				if(NewButtonControl != null)
					cell.Controls.Add(NewButtonControl);
			}
			this.textContainer = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			this.TextContainer.Controls.Add(RenderUtils.CreateLiteralControl(Grid.SettingsText.GetEmptyDataRow()));
			cell.Controls.Add(TextContainer);
			RenderHelper.AppendGridCssClassName(cell);
			return cell;
		}
		protected override void PrepareControlHierarchy() {
			if(NewButtonControl != null)
				RenderHelper.GetCommandColumnItemStyle(RenderHelper.FirstCommandColumnWithNewButton).AssignToControl(NewButtonControl);
			GridViewDataRowStyle emptyDataRowStyle = RenderHelper.GetEmptyDataRowStyle();
			emptyDataRowStyle.AssignToControl(this, false);
			if(TextContainer != null) {
				GridViewDataRowStyle textContainerStyle = new GridViewDataRowStyle();
				textContainerStyle.ForeColor = emptyDataRowStyle.ForeColor;
				textContainerStyle.HorizontalAlign = emptyDataRowStyle.HorizontalAlign;
				textContainerStyle.AssignToControl(this.textContainer);
			}
			base.PrepareControlHierarchy();
			Grid.RaiseHtmlRowPrepared(this);
			if(RenderHelper.AllowBatchEditing && Grid.VisibleRowCount > 0)
				Style[HtmlTextWriterStyle.Display] = "none";
		}
	}
	public class GridViewTablePagerEmptyCell : GridViewTableBaseCell {
		public GridViewTablePagerEmptyCell(GridViewRenderHelper renderHelper, GridViewColumn column, int visibleIndex, bool removeRightBorder)
			: base(renderHelper, null, column, visibleIndex, false, removeRightBorder) {
		}
		protected override void CreateControlHierarchy() {
			Text = "&nbsp;";
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetDataCellStyle(Column).AssignToControl(this, true);
			RenderHelper.AppendGridCssClassName(this);
			base.PrepareControlHierarchy();
		}
	}
	public class GridViewTablePagerEmptyRow : GridViewTableRow {
		public GridViewTablePagerEmptyRow(GridViewRenderHelper renderHelper, int visibleIndex) : base(renderHelper, visibleIndex) { }
		public override GridViewRowType RowType { get { return GridViewRowType.PagerEmptyRow; } }
		protected override void CreateControlHierarchy() {
			CreateLeftIndentCells();
			for(int i = 0; i < LeafColumns.Count; i++) {
				Cells.Add(CreateEmptyDataCell(LeafColumns[i].Column, i == LeafColumns.Count - 1));
			}
			CreateRightIndentCells();
			RenderHelper.AddHorzScrollExtraCell(this);
		}
		protected virtual TableCell CreateEmptyDataCell(GridViewColumn column, bool removeRightBorder) {
			return new GridViewTablePagerEmptyCell(RenderHelper, column, VisibleIndex, removeRightBorder);
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetDataRowStyle(VisibleIndex).AssignToControl(this, false);
			base.PrepareControlHierarchy();
			Grid.RaiseHtmlRowPrepared(this);
		}
	}
	public class GridViewTableDetailRow : GridViewTableRow {
		public GridViewTableDetailRow(GridViewRenderHelper renderHelper, int visibleIndex)
			: base(renderHelper, visibleIndex) { }
		public override GridViewRowType RowType { get { return GridViewRowType.Detail; } }
		protected override void CreateControlHierarchy() {
			ID = GetID();
			CreateLeftIndentCells();
			CreateContentCell();
			CreateRightIndentCells();
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetDetailRowStyle().AssignToControl(this, false);
			int contentCellIndex = Cells.Count - 1;
			if(RenderAdaptiveDetailIndentOnTheRight)
				contentCellIndex--;
			TableCell cell = Cells[contentCellIndex];
			RenderHelper.GetDetailCellStyle().AssignToControl(cell, true);
			ClearCellPaddings(cell);
			Grid.RaiseHtmlRowPrepared(this);
		}
		protected virtual string GetID() {
			return RenderHelper.GetDetailRowId(VisibleIndex);
		}
		protected virtual void CreateContentCell() {
			RenderHelper.AddDetailRowTemplateControl(VisibleIndex, this, LeafColumns.Count);
		}
		protected virtual void ClearCellPaddings(TableCell cell) {
			cell.Style[RenderHelper.IsRightToLeft ? "padding-right" : "padding-left"] = "0";
		}
	}
	public class GridViewTableAdaptiveDetailRow : GridViewTableDetailRow {
		public GridViewTableAdaptiveDetailRow(GridViewRenderHelper renderHelper)
			: base(renderHelper, -1) { }
		protected GridViewFormLayoutProperties FormLayoutProperties {
			get { return RenderHelper.Grid.SettingsAdaptivity.AdaptiveDetailLayoutProperties; }
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.AppendDefaultDXClassName(this, GridViewStyles.GridAdaptiveDetailRowCssClass);
			RenderUtils.SetVisibility(this, false, true);
		}
		protected override string GetID() {
			return RenderHelper.GetSampleAdaptiveDetailRowId();
		}
		protected override TableCell CreateIndentTableCell() {
			return new GridViewTableAdaptiveDetailIndentCell(RenderHelper);
		}
		protected override void CreateContentCell() {
			Control contentControl = RenderHelper.AddAdaptiveDetailRowControl(this, LeafColumns.Count);
			if(Grid.SettingsAdaptivity.AdaptivityMode == GridViewAdaptivityMode.HideDataCellsWindowLimit && !FormLayoutProperties.Items.IsEmpty) {
				ASPxFormLayout formLayout = CreateFormLayout();
				contentControl.Controls.Add(formLayout);
				formLayout.ForEach(delegate(LayoutItemBase item) {
					ContentPlaceholderLayoutItem placeHolderItem = item as ContentPlaceholderLayoutItem;
					if(placeHolderItem != null && placeHolderItem.IsVisible())
						CreateItemContent(placeHolderItem);
				});
			}
		}
		protected internal void PopulateDefaultLayout(ASPxFormLayout formLayout) {
			FormLayoutProperties prop = Grid.GenerateAdaptiveDefaultLayout(false);
			formLayout.ColCount = prop.ColCount;
			formLayout.Items.Assign(prop.Items);
		}
		protected void CreateItemContent(ContentPlaceholderLayoutItem layoutItem) {
			GridViewColumnLayoutItem columnLayoutItem = layoutItem as GridViewColumnLayoutItem;
			if(columnLayoutItem == null || columnLayoutItem.Column == null)
				return;
			var index = RenderHelper.GetColumnGlobalIndex(columnLayoutItem.Column);
			if(columnLayoutItem.Column is GridViewCommandColumn) {
				layoutItem.ShowCaption = DefaultBoolean.False;
				layoutItem.CssClass = RenderUtils.CombineCssClasses(GridViewStyles.GridAdaptiveDetailCommandCellCssClass, GridViewStyles.GridAdaptiveDetailLayoutItemContentCssClass + index);
			}
			else
				layoutItem.NestedControlCellStyle.CssClass = GridViewStyles.GridAdaptiveDetailLayoutItemContentCssClass + index;
		}
		protected ASPxFormLayout CreateFormLayout() {
			FormLayoutProperties.ValidateLayoutItemColumnNames();
			ASPxFormLayout formLayout = new ASPxFormLayout();
			formLayout.ID = GridRenderHelper.AdaptiveFormLayoutID;
			formLayout.ParentSkinOwner = RenderHelper.Grid;
			formLayout.Width = Unit.Percentage(100);
			formLayout.CssClass = FormLayoutStyles.ViewFormLayoutSystemClassName;
			formLayout.EnableViewState = false;
			formLayout.Properties.Assign(FormLayoutProperties);
			formLayout.Properties.DataOwner = FormLayoutProperties.DataOwner;
			formLayout.Styles.LayoutItem.CaptionCell.CssClass = GridViewStyles.GridAdaptiveDetailCaptionCellCssClass;
			formLayout.Styles.LayoutItem.NestedControlCell.CssClass = GridViewStyles.GridAdaptiveDetailDataCellCssClass;
			return formLayout;
		}
		protected override void ClearCellPaddings(TableCell cell) {
		}
	}
	public class GridViewTableDataRow : GridViewTableGroupAndDataRow, IValueProvider {
		int dataRowIndex;
		public GridViewTableDataRow(GridViewRenderHelper renderHelper, int visibleIndex, int dataRowIndex, bool hasGroupFooter)
			: base(renderHelper, visibleIndex, hasGroupFooter) {
			this.dataRowIndex = dataRowIndex;
		}
		public override GridViewRowType RowType { get { return GridViewRowType.Data; } }
		protected override bool RequireRenderParentRows { get { return true; } }
		protected int DataRowIndex { get { return dataRowIndex; } }
		protected bool IsSelected { get { return DataProxy.Selection.IsRowSelected(VisibleIndex); } }
		protected bool IsFocused { get { return DataProxy.IsRowFocused(VisibleIndex); } }
		protected bool IsDetailButtonExpanded { get { return RenderHelper.HasDetailRow(VisibleIndex); } }
		protected override void CreateControlHierarchy() {
			CreateLeftIndentCells();
			ID = RenderHelper.GetDataRowId(VisibleIndex);
			if(RenderHelper.AddDataRowTemplateControl(VisibleIndex, this, LeafColumns.Count))
				return;
			for(int i = 0; i < LeafColumns.Count; i++)
				Cells.Add(CreateContentCell(LeafColumns[i].Column, i));
			if(LeafColumns.Count == 0)
				Cells.Add(new GridViewTableNoColumnsCell(RenderHelper));
			CreateRightIndentCells();
			RenderHelper.AddHorzScrollExtraCell(this);
		}
		protected override TableCell CreateAdaptiveDetailButtonCell() {
			return new GridViewTableAdaptiveDetailButtonCell(RenderHelper, VisibleIndex);
		}
		protected override TableCell CreateDetailButtonCell() {
			return new GridViewTableDetailButtonCell(RenderHelper, VisibleIndex, IsDetailButtonExpanded);
		}
		protected override void PrepareControlHierarchy() {
			GetRowStyle().AssignToControl(this);
			base.PrepareControlHierarchy();
			Grid.RaiseHtmlRowPrepared(this);
		}
		TableCell CreateContentCell(GridViewColumn column, int index) {
			return RenderHelper.CreateContentCell(this, column, index, VisibleIndex);
		}
		object IValueProvider.GetValue(string fieldName) { return DataProxy.GetRowValue(VisibleIndex, fieldName); }
		public override bool RemoveExtraIndentBottomBorder() {
			return HasGroupFooter;
		}
		protected internal override AppearanceStyle GetRegularRowStyle() {
			if(Grid.EditingRowVisibleIndex == VisibleIndex)
				return RenderHelper.GetEditFormDisplayRowStyle();
			return RenderHelper.GetDataRowStyle(DataRowIndex);
		}
		protected internal override AppearanceStyle GetRowStyle() {
			var rowStyle = new GridViewDataRowStyle();
			rowStyle.CopyFrom(GetRegularRowStyle());
			rowStyle.CopyFrom(RenderHelper.GetConditionalFormatItemStyle(VisibleIndex));
			return rowStyle;
		}
	}
	public class GridViewTableIndentCell : GridTableCell {
		public GridViewTableIndentCell(GridViewRenderHelper renderHelper)
			: this(renderHelper, 1) {
		}
		public GridViewTableIndentCell(GridViewRenderHelper renderHelper, int columnSpan)
			: base(renderHelper, false, false) {
			if(columnSpan > 1)
				ColumnSpan = columnSpan;
		}
		protected override void CreateControlHierarchy() {
			Text = "&nbsp;";
		}
		protected virtual void AddIndentCellClassName() {
			RenderHelper.AppendIndentCellCssClassName(this);
		}
		protected virtual Unit GetCellWidth() { return Unit.Pixel(0); }
		protected override void PrepareControlHierarchy() {
			Width = GetCellWidth();
			AddIndentCellClassName();
			RenderHelper.AppendGridCssClassName(this);
			base.PrepareControlHierarchy();
		}
		protected override bool GetRemoveLeftBorder() { return !RenderHelper.IsRightToLeft; }
		protected override bool GetRemoveRightBorder() { return RenderHelper.IsRightToLeft; }
	}
	public class GridViewTableNoColumnsCell : GridViewTableIndentCell {
		public GridViewTableNoColumnsCell(GridViewRenderHelper renderHelper)
			: base(renderHelper, 1) { }
		protected override bool GetRemoveLeftBorder() { return true; }
		protected override bool GetRemoveRightBorder() { return true; }
		protected override Unit GetCellWidth() { return Unit.Empty; }
	}
	public class GridViewTableNoColumnsAdaptiveCell : GridViewTableNoColumnsCell {
		public GridViewTableNoColumnsAdaptiveCell(GridViewRenderHelper renderHelper)
			: base(renderHelper) { }
		protected override void AddIndentCellClassName() {
			RenderHelper.AppendAdaptiveIndentCellCssClassName(this);
		}
	}
	public class GridViewTableAdaptiveDetailIndentCell : GridViewTableIndentCell {
		public GridViewTableAdaptiveDetailIndentCell(GridViewRenderHelper renderHelper)
			: base(renderHelper) {
		}
		public GridViewTableAdaptiveDetailIndentCell(GridViewRenderHelper renderHelper, int columnSpan)
			: base(renderHelper, columnSpan) {
		}
		protected override bool GetRemoveLeftBorder() { return !RenderHelper.IsRightToLeft; }
		protected override bool GetRemoveRightBorder() { return RenderHelper.IsRightToLeft; }
		protected override bool GetRemoveBottomBorder() { return false; }
	}
	public class GridViewTableGroupFooterIndentCell : GridViewTableIndentCell {
		public GridViewTableGroupFooterIndentCell(GridViewRenderHelper renderHelper, int columnSpan) : base(renderHelper, columnSpan) { }
		protected override void AddIndentCellClassName() {
		}
		protected override bool GetRemoveRightBorder() { return true; }
	}
	public class GridViewTableInvisibleParentsRowDataCell : GridViewTableIndentCell {
		Image image;
		int visibleIndex;
		public GridViewTableInvisibleParentsRowDataCell(GridViewRenderHelper renderHelper, int visibleIndex)
			: base(renderHelper, 1) {
			this.visibleIndex = visibleIndex;
		}
		protected new GridViewRenderHelper RenderHelper { get { return (GridViewRenderHelper)base.RenderHelper; } }
		protected new ASPxGridViewScripts Scripts { get { return (ASPxGridViewScripts)base.Scripts; } }
		protected Image Image { get { return image; } }
		protected int VisibleIndex { get { return visibleIndex; } }
		protected override void CreateControlHierarchy() {
			ID = GridViewRenderHelper.ParentRowsID;
			this.image = RenderUtils.CreateImage();
			Controls.Add(image);
		}
		protected override void PrepareControlHierarchy() {
			Attributes["onmouseover"] = Scripts.GetShowParentRowsWindowFunction();
			Attributes["onmouseout"] = Scripts.GetHideParentRowsWindowFunction(false);
			RenderHelper.AssignImageToControl(GridViewImages.ParentGroupRowsName, Image);
			Style[HtmlTextWriterStyle.Padding] = "0";
			base.PrepareControlHierarchy();
		}
	}
	public class GridViewTableDetailButtonCell : GridViewTableIndentCell {
		int visibleIndex = -1;
		bool isDetailButtonExpanded = false;
		public GridViewTableDetailButtonCell(GridViewRenderHelper renderHelper, int visibleIndex,
			bool isDetailButtonExpanded)
			: base(renderHelper, 1) {
			this.visibleIndex = visibleIndex;
			this.isDetailButtonExpanded = isDetailButtonExpanded;
		}
		protected new GridViewRenderHelper RenderHelper { get { return (GridViewRenderHelper)base.RenderHelper; } }
		protected new ASPxGridViewScripts Scripts { get { return (ASPxGridViewScripts)base.Scripts; } }
		protected override bool GetRemoveLeftBorder() { return false; }
		protected override bool GetRemoveRightBorder() { return true; }
		protected int VisibleIndex { get { return visibleIndex; } }
		protected bool IsDetailButtonExpanded { get { return isDetailButtonExpanded; } }
		protected object KeyValue { get { return RenderHelper.DataProxy.GetRowKeyValue(VisibleIndex); } }
		protected bool Accessible { get { return RenderHelper.Grid.IsAccessibilityCompliantRender(true); } }
		protected override void AddIndentCellClassName() { }
		GridViewDetailRowButtonState CalculatedButtonState;
		Image Image;
		HyperLink Link;
		protected override void CreateControlHierarchy() {
			CalculatedButtonState = GetButtonState();
			Image = RenderUtils.CreateImage();
			if(Accessible && CalculatedButtonState == GridViewDetailRowButtonState.Visible) {
				Link = RenderUtils.CreateHyperLink();
				Controls.Add(Link);
				Link.Controls.Add(Image);
			}
			else {
				Controls.Add(Image);
			}
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetDetailButtonStyle().AssignToControl(this, true);
			base.PrepareControlHierarchy();
			if(RenderHelper.RequireFixedTableLayout) {
				Style[HtmlTextWriterStyle.Overflow] = "visible";
				Width = Unit.Empty;
			}
			RenderHelper.AssignImageToControl(CalcImageName(), Image);
			if(CalculatedButtonState == GridViewDetailRowButtonState.Visible) {
				if(Link != null) {
					Link.NavigateUrl = string.Format("javascript:{0}", GetScriptFunction());
					RenderUtils.SetStringAttribute(Link, "onclick", GridViewRenderHelper.CancelBubbleJs);
				}
				else {
					RenderUtils.SetStringAttribute(Image, "onclick", GetScriptFunction() + ";" + GridViewRenderHelper.CancelBubbleJs);
					if(RenderHelper.IsGridEnabled)
						RenderUtils.SetCursor(Image, RenderUtils.GetPointerCursor());
				}
			}
			else {
				Image.Style[HtmlTextWriterStyle.Visibility] = "hidden";
			}
		}
		protected virtual string GetScriptFunction() {
			return IsDetailButtonExpanded ? Scripts.GetHideDetailRowFunction(VisibleIndex) : Scripts.GetShowDetailRowFunction(VisibleIndex);
		}
		protected virtual GridViewDetailRowButtonState GetButtonState() {
			if(RenderHelper.AllowBatchEditing && VisibleIndex < 0)
				return GridViewDetailRowButtonState.Hidden;
			ASPxGridViewDetailRowButtonEventArgs args = new ASPxGridViewDetailRowButtonEventArgs(VisibleIndex, KeyValue, IsDetailButtonExpanded);
			RenderHelper.Grid.RaiseDetailRowGetButtonVisibility(args);
			return args.ButtonState;
		}
		protected virtual string CalcImageName() {
			if(IsDetailButtonExpanded)
				return RenderHelper.IsRightToLeft ? GridViewImages.DetailExpandedButtonRtlName : GridViewImages.DetailExpandedButtonName;
			return RenderHelper.IsRightToLeft ? GridViewImages.DetailCollapsedButtonRtlName : GridViewImages.DetailCollapsedButtonName;
		}
	}
	public class GridViewTableAdaptiveDetailButtonCell : GridViewTableIndentCell {
		const string
			AdaptiveDetailShowButtonCssClass = "dxgvADSB",
			AdaptiveDetailHideButtonCssClass = "dxgvADHB";
		int visibleIndex = -1;
		public GridViewTableAdaptiveDetailButtonCell(GridViewRenderHelper renderHelper, int visibleIndex)
			: base(renderHelper) {
			this.visibleIndex = visibleIndex;
		}
		protected int VisibleIndex { get { return visibleIndex; } }
		protected GridCommandColumnButtonControl ShowButton { get; set; }
		protected GridCommandColumnButtonControl HideButton { get; set; }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			ShowButton = RenderHelper.CreateCommandButtonControl(GridCommandButtonType.ShowAdaptiveDetail, VisibleIndex, false);
			Controls.Add(ShowButton);
			HideButton = RenderHelper.CreateCommandButtonControl(GridCommandButtonType.HideAdaptiveDetail, VisibleIndex, false);
			Controls.Add(HideButton);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(ShowButton != null)
				ShowButton.AssignInnerControlCssClass(AdaptiveDetailShowButtonCssClass);
			if(HideButton != null)
				HideButton.AssignInnerControlCssClass(AdaptiveDetailHideButtonCssClass);
		}
		protected override void AddIndentCellClassName() {
			RenderHelper.AppendAdaptiveIndentCellCssClassName(this);
		}
		protected override bool GetRemoveBottomBorder() {
			return false;
		}
	}
	public class GridViewTableBaseCell : GridTableCell {
		GridViewColumn column;
		int visibleIndex;
		GridViewTableDataRow row;
		public GridViewTableBaseCell(GridViewRenderHelper renderHelper, GridViewTableDataRow row, GridViewColumn column, int visibleIndex, bool removeLeftBorder, bool removeRightBorder)
			: base(renderHelper, removeLeftBorder, removeRightBorder) {
			this.column = column;
			this.row = row;
			this.visibleIndex = visibleIndex;
		}
		protected new GridViewRenderHelper RenderHelper { get { return (GridViewRenderHelper)base.RenderHelper; } }
		protected GridViewTableDataRow Row { get { return row; } }
		protected WebDataProxy DataProxy { get { return RenderHelper.DataProxy; } }
		public GridViewColumn Column { get { return column; } }
		public int VisibleIndex { get { return visibleIndex; } }
		protected override bool GetRemoveLeftBorder() { return RemoveLeftBorder || !RenderHelper.ShowVerticalGridLine; }
		protected override bool GetRemoveRightBorder() { return base.GetRemoveRightBorder() || !RenderHelper.ShowVerticalGridLine; }
	}
	public class GridViewCommandItemsCell : GridCommandButtonsCell {
		public GridViewCommandItemsCell(GridViewRenderHelper renderHelper, IEnumerable<GridCommandButtonType> buttonTypes, bool insideEditForm)
			: base(renderHelper, buttonTypes) {
			InsideEditForm = insideEditForm;
		}
		protected bool InsideEditForm { get; private set; }
		protected bool PostponeButtonClick {
			get {
				if(!InsideEditForm)
					return false;
				return !RenderHelper.RequireRenderEditFormPopup;
			}
		}
		protected new GridViewRenderHelper RenderHelper { get { return (GridViewRenderHelper)base.RenderHelper; } }
		protected override Unit ItemSpacing { get { return RenderHelper.Styles.CommandColumn.Spacing; } }
		protected override GridCommandColumnButtonControl CreateButtonControl(GridCommandButtonType buttonType) {
			var visibleIndex = InsideEditForm ? Grid.DataProxy.EditingRowVisibleIndex : -1;
			return CreateButtonControl(buttonType, Grid, visibleIndex, PostponeButtonClick, InsideEditForm);
		}
		public static void CreateUpdateButton(WebControl container, ASPxGridBase grid, bool postponeClick) {
			CreateCommandButtonCore(container, grid, GridCommandButtonType.Update, postponeClick);
		}
		public static void CreateCancelButton(WebControl container, ASPxGridBase grid, bool postponeClick) {
			CreateCommandButtonCore(container, grid, GridCommandButtonType.Cancel, postponeClick);
		}
		protected static void CreateCommandButtonCore(WebControl container, ASPxGridBase grid, GridCommandButtonType buttonType, bool postponeClick) {
			var buttonControl = CreateButtonControl(buttonType, grid, -1, postponeClick, true);
			if(buttonControl != null)
				container.Controls.Add(buttonControl);
		}
		protected static GridCommandColumnButtonControl CreateButtonControl(GridCommandButtonType buttonType, ASPxGridBase grid, int visibleIndex, bool postponeClick, bool findColumn) {
			GridViewCommandColumn column = null;
			if(findColumn)
				column = FindCommandColumn(grid);
			return grid.RenderHelper.CreateCommandButtonControl(column, buttonType, visibleIndex, postponeClick);
		}
		protected static GridViewCommandColumn FindCommandColumn(ASPxGridBase grid) {
			var commandColumns = grid.ColumnHelper.AllColumns.OfType<GridViewCommandColumn>().ToList();
			if(commandColumns.Count == 0)
				return null;
			if(commandColumns.Count == 1)
				return commandColumns[0];
			var column = commandColumns.FirstOrDefault(c => c.ShowUpdateButton);
			if(column != null)
				return column;
			column = commandColumns.FirstOrDefault(c => c.ShowEditButton || c.ShowNewButton || c.ShowNewButtonInHeader);
			return column;
		}
	}
	public abstract class GridViewTableBaseCommandCell : GridViewTableBaseCell {
		public GridViewTableBaseCommandCell(GridViewRenderHelper renderHelper, GridViewTableDataRow row, GridViewCommandColumn column, int visibleIndex, bool removeLeftBorder, bool removeRightBorder)
			: base(renderHelper, row, column, visibleIndex, removeLeftBorder, removeRightBorder) {
		}
		public new GridViewCommandColumn Column { get { return base.Column as GridViewCommandColumn; } }
		protected new ASPxGridViewScripts Scripts { get { return (ASPxGridViewScripts)base.Scripts; } }
		public abstract GridViewTableCommandCellType CellType { get; }
		protected new GridViewRenderHelper RenderHelper { get { return (GridViewRenderHelper)base.RenderHelper; } }
		protected ASPxGridView Grid { get { return RenderHelper.Grid; } }
		protected bool IsGridEnabled { get { return Grid.IsEnabled(); } }
		protected bool IsRowEditing { get { return CellType == GridViewTableCommandCellType.Data && RenderHelper.DataProxy.IsRowEditing(VisibleIndex); } }
		protected override void PrepareControlHierarchy() {
			GridViewCommandColumnStyle style = RenderHelper.GetCommandColumnStyle(Column);
			style.AssignToControl(this, true);
			GridViewCommandColumnStyle itemStyle = RenderHelper.GetCommandColumnItemStyle(Column);
			itemStyle.CssClass = RenderUtils.CombineCssClasses(itemStyle.CssClass, "dxgv__cci");
			for(var i = 0; i < Controls.Count; i++) {
				var control = Controls[i] as WebControl;
				if(control == null || control is GridCommandColumnSpacer)
					continue;
				GridCommandColumnButtonControl button = control as GridCommandColumnButtonControl;
				if(button != null)
					button.AssignInnerControlStyle(itemStyle);
				else
					itemStyle.AssignToControl(control, true);
			}
			RenderHelper.AppendGridCssClassName(this);
			Grid.RaiseHtmlCommandCellPrepared(this);
			base.PrepareControlHierarchy();
			RenderHelper.SetCellWidthIfRequired(Column, this, VisibleIndex);
		}
		protected void CreateCustomCommandButtons() {
			foreach(GridViewCommandColumnCustomButton button in Column.CustomButtons)
				CreateCustomButtonControl(button);
		}
		protected void CreateCustomButtonControl(GridViewCommandColumnCustomButton button) {
			ASPxGridViewCustomButtonEventArgs e = new ASPxGridViewCustomButtonEventArgs(button, VisibleIndex, CellType, IsRowEditing);
			Grid.RaiseCustomButtonInitialize(e);
			switch(e.Visible) {
				case DefaultBoolean.False:
					return;
				case DefaultBoolean.Default:
					if(!button.IsVisible(CellType, IsRowEditing))
						return;
					break;
			}
			AddCommandButtonControl(new GridCommandColumnButtonControl(e, Grid, Scripts.GetCustomButtonFuncArgs, true));
		}
		protected void CreateCommandButton(GridCommandButtonType buttonType, bool checkVisibility) {
			if(checkVisibility && !Column.CanShowCommandButton(buttonType))
				return;
			CreateCommandButton(buttonType);
		}
		protected void CreateCommandButton(GridCommandButtonType buttonType) {
			var buttonControl = RenderHelper.CreateCommandButtonControl(Column, buttonType, VisibleIndex, true);
			AddCommandButtonControl(buttonControl);
		}
		protected void AddCommandButtonControl(GridCommandColumnButtonControl buttonControl) {
			if(buttonControl != null) {
				CreateSpacerIfNeeded();
				Controls.Add(buttonControl);
			}
		}
		protected virtual void CreateSpacerIfNeeded() {
			if(Controls.Count < 1)
				return;
			Unit spacing = RenderHelper.GetCommandColumnStyle(Column).Spacing;
			if(!spacing.IsEmpty)
				Controls.Add(new GridCommandColumnSpacer(spacing));
		}
	}
	public class GridViewTableFilterRowCommandCell : GridViewTableBaseCommandCell {
		public GridViewTableFilterRowCommandCell(GridViewRenderHelper renderHelper, GridViewCommandColumn column, int visibleIndex, bool removeLeftBorder, bool removeRightBorder)
			: base(renderHelper, null, column, visibleIndex, removeLeftBorder, removeRightBorder) {
		}
		public override GridViewTableCommandCellType CellType { get { return GridViewTableCommandCellType.Filter; } }
		protected override void CreateControlHierarchy() {
			if(RenderHelper.AddFilterCellTemplateControl(Column, this))
				return;
			if(RenderHelper.AllowMultiColumnAutoFilter && Column.ShowApplyFilterButton)
				CreateCommandButton(GridCommandButtonType.ApplyFilter);
			if(!string.IsNullOrEmpty(Grid.FilterExpression))
				CreateCommandButton(GridCommandButtonType.ClearFilter, true);
			CreateCustomCommandButtons();
			if(Controls.Count == 0)
				Text = "&nbsp;";
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			HorizontalAlign = HorizontalAlign.Center;
		}
	}
	public class GridViewTableCommandCell : GridViewTableBaseCommandCell, IInternalCheckBoxOwner {
		bool checkEnabled;
		InternalCheckboxControl checkBox;
		public GridViewTableCommandCell(GridViewRenderHelper renderHelper, GridViewCommandColumn column, int visibleIndex, bool removeLeftBorder, bool removeRightBorder)
			: this(renderHelper, null, column, visibleIndex, removeLeftBorder, removeRightBorder) {
		}
		public GridViewTableCommandCell(GridViewRenderHelper renderHelper, GridViewTableDataRow row, GridViewCommandColumn column, int visibleIndex, bool removeLeftBorder, bool removeRightBorder)
			: base(renderHelper, row, column, visibleIndex, removeLeftBorder, removeRightBorder) {
		}
		public override GridViewTableCommandCellType CellType { get { return GridViewTableCommandCellType.Data; } }
		protected new ASPxGridViewScripts Scripts { get { return (ASPxGridViewScripts)base.Scripts; } }
		protected bool CheckEnabled { get { return checkEnabled; } set { checkEnabled = value; } }
		protected InternalCheckboxControl CheckBox { get { return checkBox; } }
		protected bool IsEditorButton { get { return Column.ShowEditButton || Column.ShowNewButton || Column.ShowNewButtonInHeader || RenderHelper.CommandColumnsCount == 1; } }
		protected IEnumerable<GridViewCommandColumn> VisibleCommandColumns { get { return RenderHelper.ColumnHelper.AllVisibleColumns.OfType<GridViewCommandColumn>(); } }
		protected bool SelectRowByCellClick {
			get { return Column.ShowSelectCheckbox && !ShowButtons && !Grid.SettingsBehavior.AllowSelectByRowClick; }
		}
		protected bool ShowButtons {
			get { return Column.ShowEditButton || Column.ShowNewButton || Column.ShowDeleteButton || Column.ShowSelectButton || !Column.CustomButtons.IsEmpty; }
		}
		protected override void CreateControlHierarchy() {
			if(IsRowEditing) {
				if(RenderHelper.Grid.SettingsEditing.Mode == GridViewEditingMode.Inline)
					CreateInlineEditUpdateCancelButtons();
				CreateCustomCommandButtons(); 
				if(Controls.Count == 0)
					Text = "&nbsp;";
				return;
			}
			CreateCommandButton(GridCommandButtonType.Edit, true);
			CreateCommandButton(GridCommandButtonType.New, true);
			CreateCommandButton(GridCommandButtonType.Delete, true);
			CreateCommandButton(GridCommandButtonType.Select, true);
			CreateCustomCommandButtons();
			if(Column.ShowSelectCheckbox) {
				ASPxGridViewCommandButtonEventArgs e = new ASPxGridViewCommandButtonEventArgs(Column, ColumnCommandButtonType.SelectCheckbox, VisibleIndex, IsRowEditing);
				Grid.RaiseCommandButtonInitialize(e);
				CheckEnabled = IsGridEnabled && e.Enabled;
				if(e.Visible) {
					CreateSelectCheckbox();
					if(Row is GridViewTableBatchEditEtalonDataRow)
						CheckBox.Enabled = false;
					else if(SelectRowByCellClick && CheckEnabled)
						Attributes["onclick"] = Scripts.GetSelectRowFunction(VisibleIndex);
				}
			}
			if(Controls.Count == 0)
				Text = "&nbsp;";
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			HorizontalAlign = HorizontalAlign.Center;
			if(CheckBox != null && !CheckEnabled)
				CheckBox.MainElement.CssClass = RenderUtils.CombineCssClasses(CheckBox.MainElement.CssClass, Grid.Styles.DisabledCheckboxClassName);
		}
		protected virtual void CreateInlineEditUpdateCancelButtons() {
			if(CanCreateUpdateButton())
				CreateCommandButton(GridCommandButtonType.Update);
			if(CanCreateCancelButton())
				CreateCommandButton(GridCommandButtonType.Cancel);
		}
		protected bool CanCreateUpdateButton() {
			if(Column.ShowUpdateButton)
				return true;
			if(IsEditorButton && VisibleCommandColumns.All(c => !c.ShowUpdateButton))
				return true;
			return false;
		}
		protected bool CanCreateCancelButton() {
			if(Column.ShowCancelButton)
				return true;
			if(IsEditorButton && VisibleCommandColumns.All(c => !c.ShowCancelButton))
				return true;
			return false;
		}
		protected virtual void CreateSelectCheckbox() {
			CreateSpacerIfNeeded();
			this.checkBox = new InternalCheckboxControl(this);
			Controls.Add(CheckBox);
		}
		Dictionary<string, string> GetAccessibilityCheckBoxAttributes() {
			if(!(RenderUtils.IsHtml5Mode(this) && Grid.IsAccessibilityCompliantRender()))
				return null;
			return AccessibilityUtils.CreateCheckBoxAttributes("checkbox", (this as IInternalCheckBoxOwner).CheckState);
		}
		#region IInternalCheckBoxOwner
		CheckState IInternalCheckBoxOwner.CheckState {
			get { return DataProxy.Selection.IsRowSelected(VisibleIndex) ? CheckState.Checked : CheckState.Unchecked; }
		}
		bool IInternalCheckBoxOwner.ClientEnabled { get { return CheckEnabled; } }
		InternalCheckBoxImageProperties IInternalCheckBoxOwner.GetCurrentCheckableImage() {
			return RenderHelper.GetCheckImage((this as IInternalCheckBoxOwner).CheckState, RenderHelper.AllowSelectSingleRowOnly);
		}
		string IInternalCheckBoxOwner.GetCheckBoxInputID() {
			return RenderHelper.GetSelectButtonId(VisibleIndex);
		}
		bool IInternalCheckBoxOwner.IsInputElementRequired {
			get { return true; }
		}
		AppearanceStyleBase IInternalCheckBoxOwner.InternalCheckBoxStyle {
			get { return RenderHelper.GetCheckBoxStyle(); }
		}
		Dictionary<string, string> IInternalCheckBoxOwner.AccessibilityCheckBoxAttributes { get { return GetAccessibilityCheckBoxAttributes(); } }
		#endregion
	}
	public class GridViewTableDataCell : GridViewTableBaseCell {
		public GridViewTableDataCell(GridViewRenderHelper renderHelper, GridViewTableDataRow row, GridViewDataColumn column, int visibleIndex, bool removeLeftBorder, bool removeRightBorder)
			: base(renderHelper, row, column, visibleIndex, removeLeftBorder, removeRightBorder) {
		}
		public ASPxGridView Grid { get { return RenderHelper.Grid; } }
		protected new GridViewRenderHelper RenderHelper { get { return (GridViewRenderHelper)base.RenderHelper; } }
		protected override void CreateControlHierarchy() {
			if(!RenderHelper.AddDataItemTemplateControl(VisibleIndex, DataColumn, this)) {
				RenderHelper.AddDisplayControlToDataCell(this, DataColumn, VisibleIndex, Row);
			}
		}
		public GridViewDataColumn DataColumn { get { return base.Column as GridViewDataColumn; } }
		protected override void PrepareControlHierarchy() {
			var style = RenderHelper.GetDataCellStyle(DataColumn);
			style.AssignToControl(this, AttributesRange.All, false, true);
			style.Paddings.AssignToControl(this);
			var condStyle = RenderHelper.GetConditionalFormatCellStyle(DataColumn, VisibleIndex);
			if(!condStyle.IsEmpty)
				condStyle.AssignToControl(this);
			RenderHelper.AppendGridCssClassName(this);
			RenderUtils.AllowEllipsisInText(this, DataColumn.GetAllowEllipsisInText());
			base.PrepareControlHierarchy();
			Grid.RaiseHtmlDataCellPrepared(this);
			RenderHelper.SetCellWidthIfRequired(Column, this, VisibleIndex);
		}
	}
	public class GridViewTableEmptyBandCell : GridViewTableBaseCell {
		public GridViewTableEmptyBandCell(GridViewRenderHelper renderHelper, GridViewColumn column, int visibleIndex, bool removeLeftBorder, bool removeRightBorder)
			: base(renderHelper, null, column, visibleIndex, removeLeftBorder, removeRightBorder) {
		}
		protected override void CreateControlHierarchy() {
			Controls.Add(RenderUtils.CreateLiteralControl("&nbsp;"));
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderHelper.AppendGridCssClassName(this);
		}
	}
	public class GridViewHtmlAdaptiveHeaderPanel : ASPxInternalWebControl {
		public GridViewHtmlAdaptiveHeaderPanel(GridViewRenderHelper renderHelper)
			: base() {
			RenderHelper = renderHelper;
		}
		protected GridViewRenderHelper RenderHelper { get; set; }
		protected GridViewColumnHelper ColumnHelper { get { return RenderHelper.Grid.ColumnHelper; } }
		protected WebControl SampleAdaptiveHeader { get; private set; }
		protected TableRow AdaptiveHeaderTableRow { get; private set; }
		protected bool IsEmptyHeader { get { return ColumnHelper.Leafs.Count == 0; } }
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected override void CreateControlHierarchy() {
			if(IsEmptyHeader)
				Controls.Add(new LiteralControl(RenderHelper.Grid.SettingsText.GetEmptyHeaders()));
			else {
				SampleAdaptiveHeader = RenderUtils.CreateDiv();
				SampleAdaptiveHeader.ID = GridViewRenderHelper.AdaptiveHeaderID;
				AdaptiveHeaderTableRow = RenderUtils.CreateTableRow();
				RenderUtils.PutInControlsSequentially(this, SampleAdaptiveHeader, RenderUtils.CreateTable(), AdaptiveHeaderTableRow);
			}
		}
		protected override void PrepareControlHierarchy() {
			if(IsEmptyHeader)
				RenderUtils.SetStyleStringAttribute(this, "padding", "10px 7px");
			RenderHelper.GetAdaptiveHeaderPanelStyle().AssignToControl(this);
			RenderUtils.SetVisibility(this, false, true);
			if(SampleAdaptiveHeader != null) {
				SampleAdaptiveHeader.CssClass = GridViewStyles.GridAdaptiveHeaderCssClass;
				AdaptiveHeaderTableRow.CssClass = GridViewStyles.GridAdaptiveHeaderTableRowCssClass;
				RenderUtils.SetVisibility(SampleAdaptiveHeader, false, true);
			}
		}
	}
	public class GridViewHtmlAdaptiveFooterPanel : GridHtmlSummaryPanel {
		public GridViewHtmlAdaptiveFooterPanel(GridViewRenderHelper renderHelper) : base(renderHelper) { }
		public new GridViewRenderHelper RenderHelper { get { return base.RenderHelper as GridViewRenderHelper; } }
		public new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetAdaptiveFooterPanelStyle().AssignToControl(this);
			SummaryItemControls.ForEach(c => c.CssClass = GridViewStyles.GridAdaptiveFooterSummaryDivCssClass);
		}
	}
	public class GridViewGroupPanel : ASPxInternalWebControl {
		List<TableCell> groupPanelColumnIndents;
		GridViewRenderHelper renderHelper;
		Table groupHeadersTable;
		public GridViewGroupPanel(GridViewRenderHelper renderHelper) {
			this.renderHelper = renderHelper;
		}
		protected ASPxGridView Grid { get { return RenderHelper.Grid; } }
		protected ASPxGridViewTextSettings SettingsText { get { return Grid.SettingsText; } }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected override bool HasRootTag() { return true; }
		protected GridViewRenderHelper RenderHelper { get { return renderHelper; } }
		protected int GroupCount { get { return RenderHelper.GroupCount; } }
		protected Table GroupHeadersTable { get { return groupHeadersTable; } }
		protected override void CreateControlHierarchy() {
			if(GroupCount > 0) {
				CreateGroupHeaders();
			}
			else {
				ID = GridViewRenderHelper.GroupPanelID;
				LiteralControl textControl = new LiteralControl();
				Controls.Add(textControl);
				textControl.Text = GroupPanelText;
			}
		}
		protected string GroupPanelText { get { return SettingsText.GetGroupPanel(); } }
		protected virtual void CreateGroupHeaders() {
			this.groupPanelColumnIndents = new List<TableCell>();
			this.groupHeadersTable = RenderUtils.CreateTable();
			Controls.Add(GroupHeadersTable);
			GroupHeadersTable.GridLines = GridLines.None;
			GroupHeadersTable.BorderStyle = BorderStyle.None;
			TableRow row = RenderUtils.CreateTableRow();
			GroupHeadersTable.Rows.Add(row);
			for(int i = 0; i < GroupCount; i++) {
				row.Cells.Add(new GridViewTableHeaderCell(RenderHelper, Grid.SortedColumns[i] as GridViewDataColumn, GridHeaderLocation.Group, false, false));
				if(i < GroupCount - 1) {
					TableCell cell = RenderUtils.CreateTableCell();
					cell.Text = "&nbsp;";
					row.Cells.Add(cell);
					this.groupPanelColumnIndents.Add(cell);
				}
			}
			TableCell groupCell = RenderUtils.CreateTableCell();
			row.Cells.Add(groupCell);
			groupCell.ID = GridViewRenderHelper.GroupPanelID;
			groupCell.Width = Unit.Percentage(100);
		}
		protected override void PrepareControlHierarchy() {
			AppearanceStyle groupStyle = RenderHelper.GetGroupPanelStyle();
			groupStyle.AssignToControl(this, true);
			if(this.groupPanelColumnIndents != null) {
				foreach(TableCell cell in this.groupPanelColumnIndents) {
					cell.Width = groupStyle.Spacing;
				}
			}
			if(GroupHeadersTable != null) {
				if(RenderUtils.Browser.Family.IsWebKit)
					GroupHeadersTable.Width = Unit.Percentage(100); 
			}
			string contextMenuScript = RenderHelper.Scripts.GetContextMenu();
			if(!string.IsNullOrEmpty(contextMenuScript))
				RenderUtils.SetStringAttribute(this, "oncontextmenu", contextMenuScript);
			if(RenderUtils.Browser.Platform.IsMSTouchUI)
				RenderUtils.AppendDefaultDXClassName(this, Grid.Styles.MSTouchDraggableMarkerCssClassName);
		}
	}
	public class GridViewTableFilterRow : GridViewTableRow {
		const int visibleIndex = -1;
		public GridViewTableFilterRow(GridViewRenderHelper renderHelper)
			: base(renderHelper, visibleIndex) {
			ID = GridViewRenderHelper.FilterRowID;
		}
		public override GridViewRowType RowType { get { return GridViewRowType.Filter; } }
		protected override void CreateControlHierarchy() {
			CreateLeftIndentCells();
			if(RenderHelper.AddFilterRowTemplateControl(this, LeafColumns.Count))
				return;
			for(int i = 0; i < LeafColumns.Count; i++) {
				TableCell cell = CreateFilterCell(LeafColumns[i].Column, RenderHelper.ShouldRemoveRightBorder(i));
				this.Cells.Add(cell);
			}
			CreateRightIndentCells();
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetFilterRowStyle().AssignToControl(this, true);
			Grid.RaiseHtmlRowPrepared(this);
		}
		protected virtual TableCell CreateFilterCell(GridViewColumn column, bool removeRightBorder) {
			if(column is GridViewCommandColumn && !RenderHelper.HasTemplate(Grid.Templates.FilterCell, column.FilterTemplate)) {
				return new GridViewTableFilterRowCommandCell(RenderHelper, column as GridViewCommandColumn, visibleIndex, false, removeRightBorder);
			}
			return new GridViewTableFilterEditorCell(RenderHelper, false, removeRightBorder, column);
		}
	}
	#region OldFilterCell
	#endregion OldFilterCell
	public class GridViewTableFilterEditorCell : GridTableCell {
		ASPxEditBase editor;
		GridViewColumn column;
		TableCell editorCell;
		Image menuImage;
		public GridViewTableFilterEditorCell(GridViewRenderHelper renderHelper, bool removeLeftBorder, bool removeRightBorder, GridViewColumn column)
			: base(renderHelper, removeLeftBorder, removeRightBorder) {
			this.column = column;
		}
		protected new GridViewRenderHelper RenderHelper { get { return (GridViewRenderHelper)base.RenderHelper; } }
		protected new ASPxGridViewScripts Scripts { get { return (ASPxGridViewScripts)base.Scripts; } }
		protected GridViewColumn Column { get { return column; } }
		protected GridViewDataColumn DataColumn { get { return Column as GridViewDataColumn; } }
		protected ASPxGridView Grid { get { return Column.Grid; } }
		protected ASPxEditBase Editor { get { return editor; } }
		protected TableCell EditorCell { get { return editorCell; } }
		protected Image MenuImage { get { return menuImage; } }
		protected override void CreateControlHierarchy() {
			if(RenderHelper.AddFilterCellTemplateControl(Column, this))
				return;
			if(!Column.GetAllowAutoFilter()) {
				Text = "&nbsp;";
				return;
			}
			TableCell editContainer = this;
			if(RenderHelper.IsFilterRowMenuIconVisible(Column)) {
				Table table = RenderUtils.CreateTable();
				table.Width = Unit.Percentage(100);
				TableRow row = RenderUtils.CreateTableRow();
				this.editorCell = RenderUtils.CreateTableCell();
				TableCell imageCell = RenderUtils.CreateTableCell();
				Controls.Add(table);
				table.Rows.Add(row);
				row.Cells.Add(EditorCell);
				row.Cells.Add(imageCell);
				this.menuImage = RenderUtils.CreateImage();
				imageCell.Controls.Add(MenuImage);
				editContainer = this.editorCell;
			}
			this.editor = RenderHelper.CreateAutoFilterEditor(editContainer, DataColumn, RenderHelper.GetColumnAutoFilterText(DataColumn), EditorInplaceMode.Inplace);
		}
		protected override void PrepareControlHierarchy() {
			if(DataColumn != null) {
				GridViewFilterCellStyle style = RenderHelper.GetFilterCellStyle(DataColumn);
				style.AssignToControl(this, true);
				PrepareEditor();
				PrepareEditorCell(style);
			}
			PrepareMenuImage();
			RenderHelper.AppendGridCssClassName(this);
			base.PrepareControlHierarchy();
		}
		protected virtual void PrepareEditor() {
			if(Editor == null)
				return;
			if(Editor.Width.IsEmpty)
				Editor.Width = Unit.Percentage(100);
			if(Editor is ASPxSpinEdit)
				EditorsIntegrationHelper.LockClientValueChanged(Editor as ASPxSpinEdit);
			var buttonEdit = Editor as ASPxButtonEditBase;
			if(buttonEdit != null)
				buttonEdit.ForceShowClearButtonAlways = true;
			SetEditorEvents();
		}
		protected virtual void PrepareEditorCell(GridViewFilterCellStyle style) {
			if(EditorCell == null)
				return;
			EditorCell.Width = Unit.Percentage(100);
			Unit spacing = style.Spacing;
			if(spacing.IsEmpty)
				spacing = 2;
			EditorCell.Style[RenderHelper.IsRightToLeft ? "padding-left" : "padding-right"] = spacing.ToString();
		}
		protected virtual void PrepareMenuImage() {
			if(MenuImage == null)
				return;
			RenderHelper.GetImage(GridViewImages.FilterRowButtonName).AssignToControl(MenuImage, DesignMode);
			RenderUtils.SetCursor(MenuImage, RenderUtils.GetPointerCursor());
			RenderUtils.SetStringAttribute(MenuImage, "onclick", Scripts.GetFilterRowMenuImageClick(Grid.GetColumnGlobalIndex(Column)));
		}
		protected virtual void SetEditorEvents() {
			var events = Editor.GetClientSideEvents();
			var editEvents = events as EditClientSideEvents;
			if(editEvents != null && !RenderHelper.AllowMultiColumnAutoFilter)
				editEvents.ValueChanged = Scripts.GetFilterOnChangedFunction();
			var textEditEvents = events as TextEditClientSideEvents;
			var textBoxEvents = events as TextBoxClientSideEvents;
			var useKeyPressTimer = !RenderHelper.AllowMultiColumnAutoFilter && textBoxEvents != null && DataColumn.Settings.AllowAutoFilterTextInputTimer != DefaultBoolean.False;
			var keyPressScriptFunc = useKeyPressTimer ? Scripts.GetFilterOnKeyPressFunction() : Scripts.GetFilterOnSpecKeyPressFunction();
			if(textEditEvents != null) {
				if(RenderUtils.Browser.IsOpera)
					textEditEvents.KeyPress = keyPressScriptFunc;
				else
					textEditEvents.KeyDown = keyPressScriptFunc;
			}
		}
	}
	public class GridViewTableFooterRow : GridViewTableRow {
		public GridViewTableFooterRow(GridViewRenderHelper renderHelper) : base(renderHelper, -1) { }
		public override GridViewRowType RowType { get { return GridViewRowType.Footer; } }
		protected override void CreateControlHierarchy() {
			ID = GridViewRenderHelper.FooterRowID;
			CreateLeftIndentCells();
			if(RenderHelper.AddFooterRowTemplateControl(this, LeafColumns.Count))
				return;
			for(int i = 0; i < LeafColumns.Count; i++) {
				TableCell cell = CreateFooterCell(LeafColumns[i].Column, RenderHelper.ShouldRemoveLeftBorder(i), RenderHelper.ShouldRemoveRightBorder(i));
				this.Cells.Add(cell);
			}
			if(LeafColumns.Count == 0)
				Cells.Add(new GridViewTableIndentCell(RenderHelper));
			CreateRightIndentCells();
			RenderHelper.AddHorzScrollExtraCell(this);
		}
		protected virtual TableCell CreateFooterCell(GridViewColumn column, bool removeLeftBorder, bool removeRightBorder) {
			return new GridViewTableFooterCell(RenderHelper, column, removeLeftBorder, removeRightBorder);
		}
		protected override void AddGroupIndentCellsCore(int startIndex) {
			Cells.Add(CreateIndentTableCell());
		}
		protected override TableCell CreateIndentTableCell() {
			return new GridViewTableIndentCell(RenderHelper, ColumIndentCount);
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetFooterStyle().AssignToControl(this, false);
			Grid.RaiseHtmlRowPrepared(this);
		}
	}
	public class GridViewTableFooterCell : GridTableCell {
		GridViewColumn column;
		public GridViewTableFooterCell(GridViewRenderHelper renderHelper, GridViewColumn column, bool removeLeftBorder, bool removeRightBorder)
			: base(renderHelper, removeLeftBorder, removeRightBorder) {
			this.column = column;
		}
		protected new GridViewRenderHelper RenderHelper { get { return (GridViewRenderHelper)base.RenderHelper; } }
		protected ASPxGridView Grid { get { return RenderHelper.Grid; } }
		protected GridViewColumn Column { get { return column; } }
		protected override void CreateControlHierarchy() {
			if(!RenderHelper.AddFooterCellTemplateControl(Column, this)) {
				string text = new GridViewTextBuilder(Grid).GetFooterCaption(Column, "<br/>");
				LiteralControl literal = RenderUtils.CreateLiteralControl(string.IsNullOrEmpty(text) ? "&nbsp;" : text);
				Controls.Add(literal);
			}
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetFooterCellStyle(Column).AssignToControl(this, true);
			if(RenderHelper.AllowColumnResizing)
				Style[HtmlTextWriterStyle.Overflow] = "hidden";
			Grid.RaiseHtmlFooterCellPrepared(Column, -1, this);
			RenderHelper.AppendGridCssClassName(this);
			base.PrepareControlHierarchy();
		}
	}
	public class GridViewTableGroupFooterCell : GridTableCell {
		GridViewColumn column;
		GridViewDataColumn groupedColumn;
		int visibleIndex;
		public GridViewTableGroupFooterCell(GridViewRenderHelper renderHelper, GridViewDataColumn groupedColumn, GridViewColumn column, int visibleIndex, bool removeLeftBorder, bool removeRightBorder)
			: base(renderHelper, removeLeftBorder, removeRightBorder) {
			this.groupedColumn = groupedColumn;
			this.column = column;
			this.visibleIndex = visibleIndex;
		}
		protected new GridViewRenderHelper RenderHelper { get { return (GridViewRenderHelper)base.RenderHelper; } }
		protected ASPxGridView Grid { get { return RenderHelper.Grid; } }
		protected GridViewColumn Column { get { return column; } }
		protected GridViewDataColumn GroupedColumn { get { return groupedColumn; } }
		protected int VisibleIndex { get { return visibleIndex; } }
		protected override void CreateControlHierarchy() {
			if(RenderHelper.AddGroupFooterCellTemplateControl(this, GroupedColumn, Column, VisibleIndex))
				return;
			string text = new GridViewTextBuilder(Grid).GetGroupRowFooterText(Column, VisibleIndex);
			Text = string.IsNullOrEmpty(text) ? "&nbsp;" : text;
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetGroupFooterCellStyle(Column).AssignToControl(this, true);
			if(RenderHelper.AllowColumnResizing)
				Style[HtmlTextWriterStyle.Overflow] = "hidden";
			Grid.RaiseHtmlFooterCellPrepared(Column, VisibleIndex, this);
			RenderHelper.AppendGridCssClassName(this);
			base.PrepareControlHierarchy();
		}
	}
	public class GridViewTableStatusCell : GridTableCell {
		Table mainTable;
		TableRow mainRow;
		TableCell cell;
		public GridViewTableStatusCell(GridViewRenderHelper renderHelper)
			: base(renderHelper) {
		}
		protected new GridViewRenderHelper RenderHelper { get { return (GridViewRenderHelper)base.RenderHelper; } }
		protected ASPxGridView Grid { get { return RenderHelper.Grid; } }
		protected Table MainTable { get { return mainTable; } }
		protected TableRow MainRow { get { return mainRow; } }
		protected TableCell Cell { get { return cell; } }
		protected override void CreateControlHierarchy() {
			this.mainTable = RenderUtils.CreateTable();
			Controls.Add(MainTable);
			this.mainRow = RenderUtils.CreateTableRow();
			this.mainTable.Rows.Add(MainRow);
			this.cell = RenderUtils.CreateTableCell();
			MainRow.Cells.Add(Cell);
		}
	}
	public class GridViewTableHorzScrollExtraCell : GridTableCell {
		const string HorizontalExtraCellClassName = "dxgvHEC";
		public GridViewTableHorzScrollExtraCell(GridViewRenderHelper renderHelper)
			: base(renderHelper) {
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.AppendDefaultDXClassName(this, HorizontalExtraCellClassName);
		}
	}
	public class GridViewTableArmatureRow : InternalTableRow {
		const string
			ArmatureRowClassName = "dxgvArm",
			DetailIndentClassName = "dxgvDI",
			GroupIndentClassName = "dxgvGI";
		GridViewRenderHelper renderHelper;
		List<TableCell> indentCells = new List<TableCell>();
		List<TableCell> adaptiveIndentCells = new List<TableCell>();
		List<TableCell> groupIndentCells = new List<TableCell>();
		List<TableCell> columnLeafCells = new List<TableCell>();
		public GridViewTableArmatureRow(GridViewRenderHelper renderHelper) {
			this.renderHelper = renderHelper;
		}
		protected GridViewRenderHelper RenderHelper { get { return renderHelper; } }
		protected GridViewColumnHelper ColumnHelper { get { return RenderHelper.ColumnHelper; } }
		protected List<TableCell> IndentCells { get { return indentCells; } }
		protected List<TableCell> AdaptiveIndentCells { get { return adaptiveIndentCells; } }
		protected List<TableCell> GroupIndentCells { get { return groupIndentCells; } }
		protected List<TableCell> ColumnLeafCells { get { return columnLeafCells; } }
		protected override void CreateControlHierarchy() {
			IndentCells.Clear();
			AdaptiveIndentCells.Clear();
			GroupIndentCells.Clear();
			ColumnLeafCells.Clear();
			for(int i = 0; i < RenderHelper.IndentColumnCount; i++) {
				TableCell cell = RenderUtils.CreateTableCell();
				Cells.Add(cell);
				if(i < RenderHelper.GroupCount)
					GroupIndentCells.Add(cell);
				else {
					IndentCells.Add(cell);
					if(i == RenderHelper.IndentColumnCount - 1 && RenderHelper.HasAdaptiveDetailButtonOnTheLeft)
						AdaptiveIndentCells.Add(cell);
				}
			}
			for(int i = 0; i < ColumnHelper.Leafs.Count; i++) {
				TableCell cell = RenderUtils.CreateTableCell();
				Cells.Add(cell);
				ColumnLeafCells.Add(cell);
			}
			if(RenderHelper.HasAdaptiveDetailButtonOnTheRight) {
				TableCell cell = RenderUtils.CreateTableCell();
				Cells.Add(cell);
				IndentCells.Add(cell);
				AdaptiveIndentCells.Add(cell);
			}
			if(RenderHelper.RequireExtraCell)
				Cells.Add(RenderUtils.CreateTableCell());
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AppendDefaultDXClassName(this, ArmatureRowClassName);
			int indentWidth = GetIndentWidth();
			for(int i = 0; i < IndentCells.Count; i++) {
				IndentCells[i].Width = indentWidth;
				RenderUtils.AppendDefaultDXClassName(IndentCells[i], DetailIndentClassName);
			}
			int adaptiveIndentWidth = GetAdaptiveIndentWidth();
			for(int i = 0; i < AdaptiveIndentCells.Count; i++) {
				AdaptiveIndentCells[i].Width = adaptiveIndentWidth;
				RenderUtils.AppendDefaultDXClassName(AdaptiveIndentCells[i], GridViewStyles.GridAdaptiveIndentCellCssClass);
			}
			for(int i = 0; i < GroupIndentCells.Count; i++) {
				GroupIndentCells[i].Width = indentWidth;
				RenderUtils.AppendDefaultDXClassName(GroupIndentCells[i], GroupIndentClassName);
			}
			for(int i = 0; i < ColumnLeafCells.Count; i++)
				ColumnLeafCells[i].Width = GetWidth(ColumnHelper.Leafs[i].Column);
		}
		Unit GetWidth(GridViewColumn column) {
			Unit width = column.Width;
			if(width.IsEmpty && RenderHelper.ShowHorizontalScrolling)
				width = 100;
			if(!RenderHelper.AllowColumnResizing || width.IsEmpty || width.Type != UnitType.Pixel)
				return width;
			return Math.Max((int)width.Value, column.GetColumnMinWidth());
		}
		protected int GetIndentWidth() {
			int result = RenderHelper.Styles.GroupButtonWidth + 7;
			if(result < 14)
				result = 14;
			return result;
		}
		protected int GetAdaptiveIndentWidth() {
			return RenderHelper.Styles.AdaptiveDetailButtonWidth;
		}
	}
	public class GridViewTableBatchEditEtalonDataRow : GridViewTableDataRow {
		public GridViewTableBatchEditEtalonDataRow(GridViewRenderHelper renderHelper, int visibleIndex)
			: base(renderHelper, visibleIndex, -1, false) { }
		public override GridViewRowType RowType { get { return GridViewRowType.BatchEditNewDataRow; } }
	}
}
