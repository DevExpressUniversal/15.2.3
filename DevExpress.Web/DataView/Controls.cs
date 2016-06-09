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

using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
namespace DevExpress.Web.Internal {
	public class ItemContentInfo {
		public WebControl Container;
		public DataViewItem Item;
	}
	public class DVControlBase : ASPxInternalWebControl {
		private ASPxDataViewBase fDataView = null;
		public ASPxDataViewBase DataView {
			get { return fDataView; }
		}
		public DVControlBase(ASPxDataViewBase dataView) {
			fDataView = dataView;
		}
	}
	public class DVMainControl : DVControlBase {
		private Table fTable = null;
		private TableCell fTableCell = null;
		private DVContentControl fContentControl = null;
		public DVMainControl(ASPxDataViewBase dataView)
			: base(dataView) {
		}
		public DVContentControl ContentControl { get { return fContentControl; } }
		public string RenderEndlessPagingContainerContent() {
			var epContainer = ContentControl.EndlessPagingContainer;
			return epContainer != null ? epContainer.RenderContent() : null;
		}
		protected override void ClearControlFields() {
			fTable = null;
			fTableCell = null;
		}
		protected override void CreateControlHierarchy() {
			fTable = RenderUtils.CreateTable();
			Controls.Add(fTable);
			TableRow row = RenderUtils.CreateTableRow();
			fTable.Controls.Add(row);
			PopulateTableRow(row);
		}
		protected virtual void PopulateTableRow(TableRow row) {
			fTableCell = RenderUtils.CreateTableCell();
			fTableCell.ID = DataView.GetContentCellID();
			row.Cells.Add(fTableCell);
			fContentControl = new DVContentControl(DataView);
			fContentControl.ID = DataView.GetContentControlID();
			fTableCell.Controls.Add(fContentControl);
		}
		protected override void PrepareControlHierarchy() {
			AppearanceStyleBase style = DataView.GetControlStyle();
			RenderUtils.AssignAttributes(DataView, fTable); 
			style.AssignToControl(fTable, AttributesRange.Common | AttributesRange.Font);
			RenderUtils.SetVisibility(fTable, DataView.IsClientVisible(), true);
			style.AssignToControl(fTableCell, AttributesRange.Cell);
			RenderUtils.AppendDefaultDXClassName(fTableCell, DataView.GetCssClassNamePrefix("Ctrl"));
			RenderUtils.SetPaddings(fTableCell, DataView.GetPaddings());
			RenderUtils.SetVerticalAlign(fTableCell, VerticalAlign.Top);
			fTableCell.Height = !DataView.Height.IsEmpty ? Unit.Percentage(100) : Unit.Empty;
		}
	}
	public class DVContentControl : DVControlBase {
		private TableRow fPagerPanelTopRow = null;
		private TableCell fPagerPanelTopCell = null;
		private TableCell fPagerPanelTopIndentCell = null;
		private TableRow fPagerPanelTopIndentRow = null;
		private TableRow fPagerPanelBottomRow = null;
		private TableCell fPagerPanelBottomCell = null;
		private TableCell fPagerPanelBottomIndentCell = null;
		private TableRow fPagerPanelBottomIndentRow = null;
		private DVPagerPanelControl fPagerPanelTop = null;
		private DVPagerPanelControl fPagerPanelBottom = null;
		private WebControl fItemsScroller = null;
		private Control fItemsControl = null;
		private TableCell fItemsCell = null;
		private Table fTable = null;
		private Table emptyDataTable;
		private TableCell emptyDataCell;
		public DVContentControl(ASPxDataViewBase dataView)
			: base(dataView) {
		}
		protected internal DataViewEndlessPagingContainer EndlessPagingContainer { get; private set; }
		public void RenderEndlessPagingItems(HtmlTextWriter writer) {
			if(DataView.LayoutInternal == Layout.Table)
				((DVItemsControl)fItemsControl).RenderEndlessPagingItems(writer);
			if(DataView.LayoutInternal == Layout.Flow)
				((DVFlowItemsControl)fItemsControl).RenderEndlessPagingItems(writer);
		}
		protected override void ClearControlFields() {
			this.fTable = null;
			this.fItemsCell = null;
			this.fItemsScroller = null;
			this.fPagerPanelTop = null;
			this.fPagerPanelTopRow = null;
			this.fPagerPanelTopCell = null;
			this.fPagerPanelTopIndentCell = null;
			this.fPagerPanelTopIndentRow = null;
			this.fPagerPanelBottom = null;
			this.fPagerPanelBottomRow = null;
			this.fPagerPanelBottomCell = null;
			this.fPagerPanelBottomIndentCell = null;
			this.fPagerPanelBottomIndentRow = null;
			this.emptyDataTable = null;
			this.emptyDataCell = null;
		}
		protected override void CreateControlHierarchy() {
			if(!DataView.HasVisibleItems())
				CreateEmptyDataContent();
			else {
				fTable = RenderUtils.CreateTable(true);
				Controls.Add(fTable);
				if(DataView.HasTopPagerPanel())
					CreateTopPagerPanel(fTable);
				CreateContent();
				if(DataView.HasBottomPagerPanel())
					CreateBottomPagerPanel(fTable);
			}
		}
		protected override void PrepareControlHierarchy() {
			if(this.emptyDataTable != null) {
				this.emptyDataTable.Width = Unit.Percentage(100);
				DataViewEmptyDataStyle cellStyle = DataView.GetEmptyDataStyle();
				if(cellStyle.HorizontalAlign != HorizontalAlign.NotSet) {
					RenderUtils.SetStringAttribute(this.emptyDataCell, "align",
						cellStyle.HorizontalAlign.ToString().ToLowerInvariant());
					cellStyle.HorizontalAlign = HorizontalAlign.NotSet;
				}
				cellStyle.AssignToControl(this.emptyDataCell, true);
			} else {
				fTable.Width = Unit.Percentage(100);
				fTable.Height = !DataView.Height.IsEmpty ? Unit.Percentage(100) : Unit.Empty;
				if(fPagerPanelTopRow != null)
					fPagerPanelTopRow.Height = !DataView.Height.IsEmpty ? 1 : Unit.Empty;
				if(fPagerPanelTopCell != null) {
					DataView.GetPagerPanelStyle().AssignToControl(fPagerPanelTopCell);
					fPagerPanelTopCell.HorizontalAlign = DataView.GetPagerAlignHorizontalAlign();
					RenderUtils.SetPaddings(fPagerPanelTopCell, DataView.GetPagerPanelPaddings());
				}
				if(fPagerPanelTopIndentRow != null && fPagerPanelTopIndentCell != null) {
					fPagerPanelTopIndentCell.CssClass = DataView.GetPagerPanelSpacingClassName();
					fPagerPanelTopIndentCell.Height = DataView.GetPagerPanelSpacing();
					fPagerPanelTopIndentRow.Visible = fPagerPanelTopIndentCell.Height.Value > 0;
				}
				DataView.GetContentStyle().AssignToControl(fItemsCell);
				RenderUtils.SetPaddings(fItemsCell, DataView.GetContentPaddings());
				fItemsCell.VerticalAlign = !DataView.Height.IsEmpty ? VerticalAlign.Top : VerticalAlign.NotSet;
				if(fItemsScroller != null) 
					fItemsScroller.Style.Add("overflow", "auto");
				if(fPagerPanelBottomIndentRow != null && fPagerPanelBottomIndentCell != null) {
					fPagerPanelBottomIndentCell.CssClass = DataView.GetPagerPanelSpacingClassName();
					fPagerPanelBottomIndentCell.Height = DataView.GetPagerPanelSpacing();
					fPagerPanelBottomIndentRow.Visible = fPagerPanelBottomIndentCell.Height.Value > 0;
				}
				if(fPagerPanelBottomRow != null)
					fPagerPanelBottomRow.Height = !DataView.Height.IsEmpty ? 1 : Unit.Empty;
				if(fPagerPanelBottomCell != null) {
					DataView.GetPagerPanelStyle().AssignToControl(fPagerPanelBottomCell);
					fPagerPanelBottomCell.HorizontalAlign = DataView.GetPagerAlignHorizontalAlign();
					RenderUtils.SetPaddings(fPagerPanelBottomCell, DataView.GetPagerPanelPaddings());
				}
			}
		}
		protected internal void CreateEmptyDataContent() {
			this.emptyDataTable = RenderUtils.CreateTable(true);
			Controls.Add(this.emptyDataTable);
			if(DataView.HasTopPagerPanel())
				CreateTopPagerPanel(this.emptyDataTable);
			TableRow row = RenderUtils.CreateTableRow();
			this.emptyDataTable.Controls.Add(row);
			this.emptyDataCell = RenderUtils.CreateTableCell();
			row.Cells.Add(this.emptyDataCell);
			if(DataView.EmptyDataTemplate != null) {
				DataViewTemplateContainer container = new DataViewTemplateContainer(DataView);
				container.AddToHierarchy(this.emptyDataCell, null);
				DataView.EmptyDataTemplate.InstantiateIn(container);
			} else
				this.emptyDataCell.Controls.Add(new LiteralControl(DataView.GetEmptyDataText()));
			if(DataView.HasBottomPagerPanel())
				CreateBottomPagerPanel(this.emptyDataTable);
		}
		protected internal void CreateContent() {
			TableRow row = RenderUtils.CreateTableRow();
			fTable.Controls.Add(row);
			fItemsCell = RenderUtils.CreateTableCell();
			fItemsCell.ID = DataView.GetItemsCellID();
			row.Cells.Add(fItemsCell);
			WebControl itemsParent = fItemsCell;
			if(DataView.IsScrollingEnabled()) {
				fItemsScroller = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
				fItemsScroller.ID = DataView.GetItemsScrollerID();
				itemsParent.Controls.Add(fItemsScroller);
				itemsParent = fItemsScroller;
			}
			fItemsControl = CreateItemsControl();
			itemsParent.Controls.Add(fItemsControl);
			if(DataView.UseEndlessPaging) {
				var showMoreRow = RenderUtils.CreateTableRow();
				var showMoreCell = RenderUtils.CreateTableCell();
				EndlessPagingContainer = new DataViewEndlessPagingContainer(DataView) { ID = "EPContainer" };
				fTable.Controls.Add(showMoreRow);
				showMoreRow.Cells.Add(showMoreCell);
				showMoreCell.Controls.Add(EndlessPagingContainer);
			}
		}
		protected internal Control CreateItemsControl() {
			Control itemsControl = null;
			if(DataView.LayoutInternal == Layout.Table)
				itemsControl = new DVItemsControl(DataView);
			else if(DataView.LayoutInternal == Layout.Flow)
				itemsControl = new DVFlowItemsControl(DataView);
			return itemsControl;
		}
		protected void CreateTopPagerPanel(Table parentTable) {
			fPagerPanelTopRow = RenderUtils.CreateTableRow();
			parentTable.Controls.Add(fPagerPanelTopRow);
			fPagerPanelTopCell = RenderUtils.CreateTableCell();
			fPagerPanelTopRow.Cells.Add(fPagerPanelTopCell);
			fPagerPanelTop = new DVPagerPanelControl(DataView, PagerPanelPosition.Top);
			fPagerPanelTopCell.Controls.Add(fPagerPanelTop);
			fPagerPanelTopIndentRow = RenderUtils.CreateTableRow();
			parentTable.Controls.Add(fPagerPanelTopIndentRow);
			fPagerPanelTopIndentCell = RenderUtils.CreateTableCell();
			fPagerPanelTopIndentRow.Cells.Add(fPagerPanelTopIndentCell);
		}
		protected void CreateBottomPagerPanel(Table parentTable) {
			fPagerPanelBottomIndentRow = RenderUtils.CreateTableRow();
			parentTable.Controls.Add(fPagerPanelBottomIndentRow);
			fPagerPanelBottomIndentCell = RenderUtils.CreateTableCell();
			fPagerPanelBottomIndentRow.Cells.Add(fPagerPanelBottomIndentCell);
			fPagerPanelBottomRow = RenderUtils.CreateTableRow();
			parentTable.Controls.Add(fPagerPanelBottomRow);
			fPagerPanelBottomCell = RenderUtils.CreateTableCell();
			fPagerPanelBottomRow.Cells.Add(fPagerPanelBottomCell);
			fPagerPanelBottom = new DVPagerPanelControl(DataView, PagerPanelPosition.Bottom);
			fPagerPanelBottomCell.Controls.Add(fPagerPanelBottom);
		}
	}
	public class DVItemsControl : DVControlBase {
		private Table fMainTable = null;
		private List<ItemContentInfo> fItemsContentInfo = null;
		private List<TableCell> fItemSpacerCells = null;
		private List<TableRow> fSpacerRows = null;
		private List<TableCell> fSpacerRowItemCells = null;
		private List<TableCell> fSpacerRowSpacerCells = null;
		public DVItemsControl(ASPxDataViewBase dataView)
			: base(dataView) {
		}
		protected override void ClearControlFields() {
			fMainTable = null;
			fItemsContentInfo = null;
			fItemSpacerCells = null;
			fSpacerRows = null;
			fSpacerRowItemCells = null;
			fSpacerRowSpacerCells = null;
		}
		protected override void CreateControlHierarchy() {
			fItemsContentInfo = new List<ItemContentInfo>();
			fItemSpacerCells = new List<TableCell>();
			fSpacerRows = new List<TableRow>();
			fSpacerRowItemCells = new List<TableCell>();
			fSpacerRowSpacerCells = new List<TableCell>();
			fMainTable = RenderUtils.CreateTable(false);
			if(RenderUtils.Browser.IsIE) { 
				WebControl div = new WebControl(HtmlTextWriterTag.Div);
				Controls.Add(div);
				div.Controls.Add(fMainTable);
			} else {
				Controls.Add(fMainTable);
			}
			int itemNumber = 0;
			for(int j = 0; j < DataView.RowCount; j++) {
				if(itemNumber < DataView.VisibleItemsList.Count || DataView.IsEmptyRowsVisible()) {
					TableRow row = RenderUtils.CreateTableRow();
					fMainTable.Rows.Add(row);
					for(int i = 0; i < DataView.ColumnCountInternal; i++) {
						ItemContentInfo contentInfo = DataView.CreateItemContentInfo();
						fItemsContentInfo.Add(contentInfo);
						CreateItemCell(row, contentInfo, itemNumber);
						if(i < DataView.ColumnCountInternal - 1)
							 CreateItemSpacerCell(row);
						itemNumber++;
					}
					if(NeedAddSpacerRow(itemNumber, j)) {
						TableRow spacerRow = RenderUtils.CreateTableRow();
						fMainTable.Rows.Add(spacerRow);
						for(int i = 0; i < DataView.ColumnCountInternal; i++) {
							CreateSpacerRowItemCell(spacerRow);
							if(i < DataView.ColumnCountInternal - 1)
								CreateSpacerRowSpacerCell(spacerRow);
						}
						fSpacerRows.Add(spacerRow);
					}
				}
			}
		}
		bool NeedAddSpacerRow(int itemNumber, int rowIndex) {
			if(DataView.UseEndlessPaging && DataView.PageIndex < DataView.PageCount - 1)
				return true;
			return (itemNumber < DataView.VisibleItemsList.Count || DataView.IsEmptyRowsVisible()) && rowIndex < DataView.RowCount - 1;
		}
		protected override void PrepareControlHierarchy() {
			fMainTable.Width = Unit.Percentage(100);
			if(!DataView.IsScrollingEnabled())
				fMainTable.Height = Unit.Percentage(100);
			PrepareItemCells();
			PrepareSpacerCells();
		}
		protected void PrepareItemCells() {
			foreach(ItemContentInfo contentInfo in fItemsContentInfo) {
				contentInfo.Container.Height = DataView.GetItemHeight();
				contentInfo.Container.Width = DataView.GetItemWidth();
				DataViewItemStyle style = (contentInfo.Item != null) ? DataView.GetItemStyle() : DataView.GetEmptyItemStyle();
				Paddings paddings = (contentInfo.Item != null) ? DataView.GetItemPaddings() : DataView.GetEmptyItemPaddings();
				PrepareItemCellStyle(contentInfo.Container, style, paddings);
				if(contentInfo.Item != null) 
					DataView.PrepareItemControl(contentInfo);
				else 
					DataView.PrepareEmptyItemControl(contentInfo);
			}
		}
		protected void PrepareItemCellStyle(WebControl control, DataViewItemStyle style, Paddings paddings) {
			style.AssignToControl(control, AttributesRange.All, false, true);
			RenderUtils.SetPaddings(control, DataView.GetItemPaddings());
		}
		protected void PrepareSpacerCells() {
			foreach(TableCell cell in fItemSpacerCells) {
				cell.Visible = HasItemSpacers();
				if(HasItemSpacers())
					RenderUtils.PrepareIndentCell(cell, DataView.GetItemSpacing(), Unit.Empty);
				}
			foreach(TableRow row in fSpacerRows)
				row.Visible = HasItemSpacers();
			foreach(TableCell cell in fSpacerRowItemCells) {
				cell.Visible = HasItemSpacers();
				if(HasItemSpacers())
					RenderUtils.PrepareIndentCell(cell, Unit.Empty, DataView.GetItemSpacing());
				}
			foreach(TableCell cell in fSpacerRowSpacerCells)
				cell.Visible = HasItemSpacers();
		}
		protected void CreateItemCell(TableRow parent, ItemContentInfo contentInfo, int index) {
			contentInfo.Container = RenderUtils.CreateTableCell();
			parent.Cells.Add((TableCell)contentInfo.Container);
			if(index < DataView.VisibleItemsList.Count) {
				contentInfo.Item = DataView.VisibleItemsList[index] as DataViewItem;
				DataView.CreateItemControl(contentInfo, contentInfo.Container);
			} else {
				DataView.CreateEmptyItemControl(contentInfo, contentInfo.Container);
			}
		}
		protected void CreateItemSpacerCell(TableRow parent) {
			TableCell cell = RenderUtils.CreateIndentCell();
			parent.Cells.Add(cell);
			fItemSpacerCells.Add(cell);
		}
		protected void CreateSpacerRowItemCell(TableRow parent) {
			TableCell cell = RenderUtils.CreateIndentCell();
			parent.Cells.Add(cell);
			fSpacerRowItemCells.Add(cell);
		}
		protected void CreateSpacerRowSpacerCell(TableRow parent) {
			TableCell cell = RenderUtils.CreateTableCell();
			parent.Cells.Add(cell);
			fSpacerRowSpacerCells.Add(cell);
		}
		protected bool HasItemSpacers() {
			return DataView.GetItemSpacing().Value > 0;
		}
		public void RenderEndlessPagingItems(HtmlTextWriter writer) {
			EnsureChildControls();
			PrepareControlHierarchy();
			foreach(TableRow row in fMainTable.Rows)
				row.RenderControl(writer);
		}
	}
	public class DVFlowItemsControl : DVControlBase {
		private Table mainTable;
		private List<ItemContentInfo> itemsContentInfo;
		public DVFlowItemsControl(ASPxDataViewBase dataView)
			: base(dataView) {
		}
		protected TableCell ItemsContainer { get; private set; }
		protected bool IsRightToLeft {
			get { return (DataView as ISkinOwner).IsRightToLeft(); }
		}
		protected override void ClearControlFields() {
			this.mainTable = null;
			this.itemsContentInfo = null;
		}
		protected override void CreateControlHierarchy() {
			this.itemsContentInfo = new List<ItemContentInfo>();
			this.mainTable = RenderUtils.CreateTable(false);
			Controls.Add(this.mainTable);
			TableRow row = RenderUtils.CreateTableRow();
			this.mainTable.Rows.Add(row);
			ItemsContainer = new TableCell();
			row.Cells.Add(ItemsContainer);
			int itemCountOnPage = DataView.VisibleItemsList.Count;
			for(int i = 0; i < itemCountOnPage; i++) {
				ItemContentInfo contentInfo = DataView.CreateItemContentInfo();
				this.itemsContentInfo.Add(contentInfo);
				CreateItemDiv(ItemsContainer, contentInfo, i);
			}
		}
		protected override void PrepareControlHierarchy() {
			DataView.GetFlowItemsContainerStyle().AssignToControl(this.mainTable);
			if(DataView.GetItemSpacing().Value != 0)
				RenderUtils.SetMargins(this.mainTable, DataView.GetFlowItemsContainerSpacing());
			PrepareItemDivs();
		}
		private void CreateItemDiv(TableCell parent, ItemContentInfo contentInfo, int index) {
			contentInfo.Container = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			parent.Controls.Add(contentInfo.Container);
			contentInfo.Item = DataView.VisibleItemsList[index] as DataViewItem;
			DataView.CreateItemControl(contentInfo, contentInfo.Container);
		}
		private void PrepareItemDivs() {
			foreach(ItemContentInfo contentInfo in this.itemsContentInfo) {
				contentInfo.Container.Height = DataView.GetItemHeight();
				contentInfo.Container.Width = DataView.GetItemWidth();
				DataView.GetFlowItemStyle().AssignToControl(contentInfo.Container);
				RenderUtils.SetPaddings(contentInfo.Container, DataView.GetItemPaddings());
				RenderUtils.SetMargins(contentInfo.Container, DataView.GetFlowItemSpacing());
				DataView.PrepareItemControl(contentInfo);
				if(IsRightToLeft)
					contentInfo.Container.Style["float"] = "right";
			}
		}
		public void RenderEndlessPagingItems(HtmlTextWriter writer){
			EnsureChildControls();
			PrepareControlHierarchy();
			foreach(Control control in ItemsContainer.Controls)
				control.RenderControl(writer);
		}
	}
	public class DVPagerPanelControl : DVControlBase {
		private PagerPanelPosition fPosition = PagerPanelPosition.Top;
		private TableCell fPagerCell = null;
		private ASPxPagerBase fPager = null;
		public PagerPanelPosition Position {
			get { return fPosition; }
		}
		public DVPagerPanelControl(ASPxDataViewBase dataView, PagerPanelPosition position)
			: base(dataView) {
			fPosition = position;
		}
		protected override void ClearControlFields() {
			fPager = null;
			fPagerCell = null;
		}
		protected override void CreateControlHierarchy() {
			if(!HasPager()) return;
			if(DataView.PagerPanelLeftTemplate == null && DataView.PagerPanelRightTemplate == null)
				CreatePager(this);
			else {
				Table table = RenderUtils.CreateTable();
				Controls.Add(table);
				TableRow row = RenderUtils.CreateTableRow();
				table.Controls.Add(row);
				table.Width = Unit.Percentage(100);
				if(DataView.PagerPanelLeftTemplate != null)
					CreateTemplateCell(row, DataView.PagerPanelLeftTemplate, PagerPanelTemplatePosition.Left);
				fPagerCell = RenderUtils.CreateTableCell();
				row.Cells.Add(fPagerCell);
				fPagerCell.HorizontalAlign = DataView.GetPagerStyle().HorizontalAlign;
				fPagerCell.Width = Unit.Percentage(100);
				CreatePager(fPagerCell);
				if(DataView.PagerPanelRightTemplate != null)
					CreateTemplateCell(row, DataView.PagerPanelRightTemplate, PagerPanelTemplatePosition.Right);
			}
		}
		private void CreatePager(Control parent) {
			fPager = DataView.CreatePager();
			fPager.ID = DataView.GetPagerID(Position);
			parent.Controls.Add(fPager);
		}
		protected override void PrepareControlHierarchy() {
			if(fPager != null) {
				PreparePagerProperties();
				PreparePagerStyles();
			}
		}
		protected bool HasPager() {
			return DataView.HasTopPager() && (Position == PagerPanelPosition.Top) ||
				DataView.HasBottomPager() && (Position == PagerPanelPosition.Bottom);
		}
		protected void PreparePagerStyles() {
			fPager.ControlStyle.CopyFrom(DataView.GetPagerStyle());
			fPager.DisabledStyle.CopyFrom(DataView.GetPagerDisabledStyle());
			fPager.ButtonStyle.CopyFrom(DataView.GetPagerButtonStyle());
			fPager.DisabledButtonStyle.CopyFrom(DataView.GetPagerDisabledButtonStyle());
			fPager.PageNumberStyle.CopyFrom(DataView.GetPagerPageNumberStyle());
			fPager.CurrentPageNumberStyle.CopyFrom(DataView.GetPagerCurrentPageNumberStyle());
			(fPager.ControlStyle as PagerStyle).SeparatorStyle.CopyFrom(DataView.GetPagerSeparatorStyle());
			fPager.SummaryStyle.CopyFrom(DataView.GetPagerSummaryStyle());
			fPager.PageSizeItemStyle.CopyFrom(DataView.GetPagerPageSizeItemStyle());
		}
		protected void PreparePagerProperties() {
			fPager.PagerSettings.Assign(DataView.PagerSettings);
			fPager.ItemsPerPage = DataView.ItemsPerPage;
			fPager.ItemCount = DataView.DataItemCount;
			fPager.SetPageIndex(DataView.PageIndex);
			fPager.SetAllPagesSavedPageIndex(DataView.AllPagesSavedPageIndex);
			fPager.ShowSeparators = DataView.PagerSettings.ShowSeparators;
			if(DataView.PagerAlign == PagerAlign.Justify)
				fPager.Width = Unit.Percentage(100);
			fPager.Enabled = DataView.IsEnabled();
			fPager.SeparatorHeight = DataView.GetPagerStyle().SeparatorHeight;
			fPager.SeparatorWidth = DataView.GetPagerStyle().SeparatorWidth;
			fPager.SeparatorColor = DataView.GetPagerStyle().SeparatorColor;
			fPager.SeparatorPaddings.Assign(DataView.GetPagerStyle().SeparatorPaddings);
			fPager.SeparatorBackgroundImage.Assign(DataView.GetPagerStyle().SeparatorBackgroundImage);
			fPager.ItemSpacing = DataView.GetPagerStyle().ItemSpacing;
			if(fPagerCell != null) {
				RenderUtils.SetPaddings(fPagerCell, DataView.GetPagerStyle().Paddings);
				fPagerCell.HorizontalAlign = DataView.GetPagerAlignHorizontalAlign();
			}
		}
		protected virtual void CreateTemplateCell(TableRow parentRow, ITemplate template, PagerPanelTemplatePosition templatePosition) {
			DataViewPagerPanelTemplateContainer container = new DataViewPagerPanelTemplateContainer(DataView, Position, templatePosition);
			template.InstantiateIn(container);
			TableCell cell = RenderUtils.CreateTableCell();
			parentRow.Cells.Add(cell);
			container.AddToHierarchy(cell, DataView.GetPagerPanelTemplateContainerID(Position, templatePosition));
		}
	}
	[ToolboxItem(false)]
	public class DVPager : ASPxPagerBase {
		private ASPxDataViewBase dataView = null;
		public DVPager(ASPxDataViewBase dataView)
			: base(dataView) {
			this.dataView = dataView;
			EnableViewState = false;
			ParentSkinOwner = DataView;
		}
		public ASPxDataViewBase DataView {
			get { return dataView; }
		}
		protected override bool RequireInlineLayout { get { return true; } }
		protected override PagerSettingsEx CreatePagerSettings(ASPxPagerBase pager) {
			return new DataViewPagerSettings(pager);
		}
		protected internal override int GetPageSize() {
			return DataView.GetPageSize();
		}
		protected override string GetItemElementOnClick(string id) {
			return DataView.IsCallBacksEnabled() ? DataView.GetPagerOnClickFunction(id) : base.GetItemElementOnClick(id);
		}
		protected override string GetPageSizeChangedHandler() {
			return DataView.IsCallBacksEnabled() ? DataView.GetPageSizeChangedHandler() : base.GetPageSizeChangedHandler();
		}
		protected internal void ProcessPagerCallback(string eventArgument) {
			ProcessPagerEvent(eventArgument);
		}
		protected override bool HasContent() {
			return dataView.AlwaysShowPager || base.HasContent();
		}
		protected override void OnPageIndexChanging(PagerPageEventArgs e) {
			DataView.ChangePageIndex(e.NewPageIndex);
		}
		protected override void OnPageSizeChanging(PagerPageSizeEventArgs e) {
			DataView.ChangePageSize(e.NewPageSize);
		}
		protected override SEOTarget GetSEOTarget(params object[] args) {
			return new SEOTarget(DataView.SEOTargetName, GetSeoTargetValue(args));
		}
		protected override int GetNewPageIndex(string eventArgument) {
			return ASPxPagerBase.GetNewPageIndex(eventArgument, PageIndex, DataView.PagerPageCountEvaluator, false);
		}
		protected internal override string GetButtonNavigateUrl(int pageIndex) {
			return GetButtonNavigateUrl(pageIndex, DataView.GetPageSize());
		}
	}
	[ViewStateModeById]
	public class DataViewEndlessPagingContainer: InternalHtmlControl {
		public DataViewEndlessPagingContainer(ASPxDataViewBase dataView)
			: base(HtmlTextWriterTag.Div) {
			DataView = dataView;
		}
		protected ASPxDataViewBase DataView { get; private set; }
		public string RenderContent() {
			PrepareControlHierarchy();
			var result = string.Empty;
			foreach(Control control in Controls) {
				result += RenderUtils.GetRenderResult(control);
			}
			return result;
		}
		protected override void CreateControlHierarchy() {
			if(DataView.PageIndex >= DataView.PageCount - 1)
				return;
			base.CreateControlHierarchy();
			var link = RenderUtils.CreateHyperLink(true, true);
			link.Text = GetLinkText();
			link.NavigateUrl = "javascript:void(0)";
			RenderUtils.SetStringAttribute(link, "onclick", string.Format("ASPx.DVEPClick('{0}')", DataView.ClientID));
			Controls.Add(link);
		}
		string GetLinkText() {
			var endlesssPagingSettings = DataView.PagerSettings as IDataViewEndlessPagingSettigns;
			if(endlesssPagingSettings == null)
				return null;
			if(!string.IsNullOrEmpty(endlesssPagingSettings.ShowMoreItemsText))
				return endlesssPagingSettings.ShowMoreItemsText;
			return ASPxperienceLocalizer.GetString(ASPxperienceStringId.DataView_ShowMoreItemsText);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			DataViewPagerShowMoreItemsContainerStyle style = DataView.GetPagerShowMoreItemsContainerStyle();
			style.AssignToControl(this);
		}
	}
}
