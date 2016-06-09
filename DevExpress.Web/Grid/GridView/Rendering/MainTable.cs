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
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Data;
namespace DevExpress.Web.Rendering {
	public class GridViewHtmlScrollableControl : GridHtmlScrollableControl {
		public GridViewHtmlScrollableControl(GridViewRenderHelper helper)
			: base(helper) { 
		}
		protected string HeaderScrollDivContainerClassName { get { return GridPrefix + "HSDC"; } }
		protected string FooterScrollDivContainerClassName { get { return GridPrefix + "FSDC"; } }
		protected new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		protected new GridViewRenderHelper RenderHelper { get { return base.RenderHelper as GridViewRenderHelper; } }
		protected new ASPxGridViewSettings Settings { get { return base.Settings as ASPxGridViewSettings; } }
		protected WebControl HeaderScrollDiv { get; private set; }
		protected WebControl FooterScrollDiv { get; private set; }
		protected WebControl HeaderDivContainer { get; private set; }
		protected WebControl FooterDivContainer { get; private set; }
		protected GridViewHtmlTableRenderPart ContentTableRenderPart { get { return HasVertScrollBar ? GridViewHtmlTableRenderPart.Content : GridViewHtmlTableRenderPart.All; } }
		protected override void CreateHeader() {
			if(!HasVertScrollBar || !Settings.ShowColumnHeaders && !Settings.ShowFilterRow || RenderHelper.RequireEndlessPagingPartialLoad)
				return;
			HeaderDivContainer = CreateDiv();
			HeaderScrollDiv = CreateDiv(HeaderDivContainer);
			CreateTable(GridViewHtmlTableRenderPart.Header, GridViewRenderHelper.HeaderTableID, HeaderScrollDiv);
		}
		protected override void CreateFooter() {
			if(!HasVertScrollBar || !Settings.ShowFooter || RenderHelper.RequireEndlessPagingPartialLoad)
				return;
			FooterDivContainer = CreateDiv();
			FooterScrollDiv = CreateDiv(FooterDivContainer);
			CreateTable(GridViewHtmlTableRenderPart.Footer, GridViewRenderHelper.FooterTableID, FooterScrollDiv);
		}
		protected override Table CreateContentTable(WebControl container) {
			return CreateTable(ContentTableRenderPart, GridViewRenderHelper.MainTableID, ContentScrollDiv);
		}
		GridViewHtmlTable CreateTable(GridViewHtmlTableRenderPart renderPart, string id, WebControl container) {
			var table = new GridViewHtmlTable(RenderHelper, renderPart) { ID = id };
			container.Controls.Add(table);
			return table;
		}
		protected override void PrepareControlHierarchy() {
			PrepareHeader();
			PrepareContent();
			PrepareFooter();
		}
		protected override void PrepareContent() {
			base.PrepareContent();
			if(RenderHelper.HasFixedColumns) {
				ContentScrollDiv.Style[HtmlTextWriterStyle.OverflowX] = "hidden";
				RenderUtils.AppendMSTouchDraggableClassNameIfRequired(ContentScrollDiv);
			}
			if(RenderHelper.UseFixedGroups)
				ContentScrollDiv.Style.Add(HtmlTextWriterStyle.Position, "relative");
		}
		protected override void PrepareHeader() {
			if(HeaderScrollDiv == null) return;
			PrepareScrollDiv(HeaderScrollDiv);
			HeaderScrollDiv.Style[HtmlTextWriterStyle.Overflow] = "hidden";
			HeaderDivContainer.CssClass = HeaderScrollDivContainerClassName;
		}
		protected override void PrepareFooter() {
			if(FooterScrollDiv == null)
				return;
			PrepareScrollDiv(FooterScrollDiv);
			FooterScrollDiv.Style[HtmlTextWriterStyle.Overflow] = "hidden";
			FooterDivContainer.CssClass = FooterScrollDivContainerClassName;
		}
	}
	public class GridViewHtmlFixedColumnsScrollableControl : GridHtmlScrollableControlBase {
		const string FixedColumnsScrollDivClassName = GridViewStyles.GridPrefix + "FCSD";
		WebControl scrollDiv, contentDiv; 
		public GridViewHtmlFixedColumnsScrollableControl(GridViewRenderHelper helper)
			: base(helper) {
		}
		protected WebControl ScrollDiv { get { return scrollDiv; } }
		protected WebControl ContentDiv { get { return contentDiv; } }
		protected override void CreateControlHierarchy() {
			this.scrollDiv = CreateDiv();
			ScrollDiv.ID = GridViewRenderHelper.FixedColumnsDivID;
			this.contentDiv = CreateDiv(ScrollDiv);
			ContentDiv.ID = GridViewRenderHelper.FixedColumnsContentDivID;
		}
		protected override void PrepareControlHierarchy() {
			PrepareScrollDiv(ScrollDiv);
			ScrollDiv.Style[HtmlTextWriterStyle.OverflowX] = "scroll";
			ScrollDiv.Style[HtmlTextWriterStyle.OverflowY] = "hidden";
			RenderUtils.AppendDefaultDXClassName(ScrollDiv, FixedColumnsScrollDivClassName);
			ContentDiv.Height = 17; 
			ContentDiv.Width = 5000;
		}
	}
	public enum GridViewHtmlTableRenderPart { All, Header, Content, Footer }
	[ViewStateModeById]
	public class GridViewHtmlTable : GridHtmlMainTable {
		private const double VirtualScrollingReserveSizeFactor = 0.5;
		GridViewHtmlTableRenderPart renderPart;
		int dataRowIndex;
		public GridViewHtmlTable(GridViewRenderHelper renderHelper) : this(renderHelper, GridViewHtmlTableRenderPart.All) { }
		public GridViewHtmlTable(GridViewRenderHelper renderHelper, GridViewHtmlTableRenderPart renderPart) : base(renderHelper) {
			this.renderPart = renderPart;
			this.dataRowIndex = RenderHelper.UseEndlessPaging ? Grid.EndlessPagingHelper.ClientDataRowCount : 0;
		}
		protected new GridViewRenderHelper RenderHelper { get { return base.RenderHelper as GridViewRenderHelper; } }
		protected new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		protected GridViewHtmlTableRenderPart RenderPart { get { return renderPart; } }
		protected GridViewColumnHelper ColumnHelper { get { return Grid.ColumnHelper; } }
		protected ASPxGridViewScripts Scripts { get { return RenderHelper.Scripts; } }
		protected IList<GridViewColumnVisualTreeNode> LeafColumns { get { return ColumnHelper.Leafs; } }
		protected bool CanRenderPart(GridViewHtmlTableRenderPart part) { return part == RenderPart || RenderPart == GridViewHtmlTableRenderPart.All; }
		protected override bool RequireTouchDraggableClass { 
			get { return base.RequireTouchDraggableClass || RenderPart == GridViewHtmlTableRenderPart.Header && RenderHelper.HasFixedColumns && RenderHelper.ShowVerticalScrolling; } 
		}
		protected bool RequireAccessibilityHeaderRow {
			get { return Grid.AccessibilityCompliant && RenderHelper.ShowVerticalScrolling && (RenderPart == GridViewHtmlTableRenderPart.Content || RenderPart == GridViewHtmlTableRenderPart.Footer); }
		}
		protected override void CreateControlHierarchy() {
			if(RenderHelper.RequireFixedTableLayout && !RenderHelper.RequireEndlessPagingPartialLoad)
				Rows.Add(new GridViewTableArmatureRow(RenderHelper));
			if(CanRenderPart(GridViewHtmlTableRenderPart.Header)) {
				CreateHeaders();
				CreateFilterRow();
			} else
				if(RequireAccessibilityHeaderRow)
					CreateAccessibilityHeaderRow();
			if(CanRenderPart(GridViewHtmlTableRenderPart.Content)) {				
				CreateNewRow(GridViewNewItemRowPosition.Top);
				CreateRows();
				CreateNewRow(GridViewNewItemRowPosition.Bottom);
			}
			if(CanRenderPart(GridViewHtmlTableRenderPart.Footer)) {
				CreateFooter();
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(RenderHelper.RequireFixedTableLayout) {
				Style["table-layout"] = "fixed";
				Style[HtmlTextWriterStyle.Overflow] = "hidden";
				if(RenderHelper.AllowColumnResizing)
					Style[HtmlTextWriterStyle.TextOverflow] = "ellipsis";
				if(RenderHelper.IsRightToLeft && RenderHelper.HasFixedColumns)
					Style["float"] = "right";
			}
			if(RenderPart == GridViewHtmlTableRenderPart.All || RenderPart == GridViewHtmlTableRenderPart.Header)
				Caption = Grid.Caption;
			if(!RenderUtils.IsHtml5Mode(this))
				RenderUtils.SetStringAttribute(this, "summary", Grid.SummaryText);
			if(RenderHelper.IsRightToLeft)
				Attributes["dir"] = "rtl";
			string contextMenuScript = Scripts.GetContextMenu();
			if(!string.IsNullOrEmpty(contextMenuScript))
				Attributes["oncontextmenu"] = contextMenuScript;
			new BottomBorderRemovalHelper(Rows, RenderHelper).Run();
			if(!RenderHelper.UseEndlessPaging)
				GridRenderHelper.NormalizeColSpans(Rows);
			if(RenderHelper.RequireLastVisibleRowBottomBorder && (RenderPart == GridViewHtmlTableRenderPart.Content || RenderPart == GridViewHtmlTableRenderPart.All))
				RenderUtils.AppendDefaultDXClassName(this, GridViewRenderHelper.RequireLastVisibleRowBottomBorderCssClass);
		}
		protected virtual void CreateHeaders() {
			if(!Grid.Settings.ShowColumnHeaders) return;			
			Rows.Add(new GridViewTableHeaderRow(RenderHelper, 0));
			for(int i = 1; i < ColumnHelper.Layout.Count; i++)
				Rows.Add(new GridViewTableHeaderRow(RenderHelper, i));
		}
		protected virtual void CreateAccessibilityHeaderRow() {
			var row = RenderUtils.CreateTableRow();
			Controls.Add(row);
			row.ID = string.Format(GridViewRenderHelper.AccessibilityHeaderRowFormat, RenderPart);
			foreach(var leaf in ColumnHelper.Leafs) {
				var cell = new TableHeaderCell();
				row.Cells.Add(cell);
				cell.Text = RenderHelper.TextBuilder.GetHeaderCaption(leaf.Column);
				RenderUtils.SetStringAttribute(cell, "scope", "col");
			}
			row.CssClass = AccessibilityUtils.InvisibleRowCssClassName;
		}
		protected virtual void CreateFilterRow() {
			if(!Grid.Settings.ShowFilterRow || LeafColumns.Count == 0)
				return;
			AddRowAndRaiseRowCreated(new GridViewTableFilterRow(RenderHelper));
		}
		protected virtual void CreateRows() {
			if(RenderHelper.HasAdaptivity)
				CreateSampleAdaptiveDetailRow();
			if(RenderHelper.RequireEndlessPagingPartialLoad && Grid.EndlessPagingHelper.PrevEditedRowIndex > -1)
				CreateRow(Grid.EndlessPagingHelper.PrevEditedRowIndex);
			CreateParentGroupRows();
			if(RenderHelper.IsVirtualSmoothScrolling)
				CreateReserveTopRows();
			for(int i = 0; i < DataProxy.VisibleRowCountOnPage; i++)
				CreateRow(i + DataProxy.VisibleStartIndex);
			if(RenderHelper.IsVirtualSmoothScrolling)
				CreateReserveBottomRows();
			if(RenderHelper.HasEmptyDataRow)
				CreateEmptyRow();
			for (int i = 0; i < RenderHelper.EmptyPagerDataRowCount; i++)
				CreatePagerEmptyRow(DataProxy.VisibleRowCountOnPage + i);
		}
		protected virtual void CreateParentGroupRows() {
			if(!RenderHelper.UseFixedGroups || RenderHelper.UseEndlessPaging)
				return;
			foreach(var visibleIndex in DataProxy.GetParentRows()) {
				CreateRow(visibleIndex);
			}
		}
		protected virtual void CreateNewRow(GridViewNewItemRowPosition rowPosition) {
			if(rowPosition != Grid.SettingsEditing.NewItemRowPosition) 
				return;
			if(RenderHelper.UseEndlessPaging && !RenderHelper.CanCreateNewRowInEndlessPaging)
				return;
			if(DataProxy.IsNewRowEditing)
				CreateEditRow(WebDataProxy.NewItemRow);
			else if(RenderHelper.AllowBatchEditing)
				CreateEtalonNewDataRow();
		}
		protected virtual void CreateEmptyRow() {
			AddRowAndRaiseRowCreated(new GridViewTableEmptyDataRow(RenderHelper));
		}
		protected virtual void CreatePagerEmptyRow(int rowVisibleIndex) {
			AddRowAndRaiseRowCreated(new GridViewTablePagerEmptyRow(RenderHelper, rowVisibleIndex));
		}
		protected virtual void CreateRow(int visibleIndex) {
			List<int> groupFooterVisibleIndexes = RenderHelper.GetGroupFooterVisibleIndexes(visibleIndex);
			CreateRowCore(visibleIndex, groupFooterVisibleIndexes);
			if(groupFooterVisibleIndexes != null) {
				CreateGroupFooterRow(groupFooterVisibleIndexes);
			}
		}
		protected virtual void CreateRowCore(int visibleIndex, List<int> groupFooterVisibleIndexes) {
			bool hasGroupFooter = groupFooterVisibleIndexes != null && groupFooterVisibleIndexes.Count > 0;
			if(DataProxy.GetRowType(visibleIndex) == WebRowType.Group) {
				CreateGroupRow(visibleIndex, hasGroupFooter);
				return;
			}
			if(DataProxy.IsRowEditing(visibleIndex)) {
				CreateEditRow(visibleIndex, hasGroupFooter);
			}
			CreateDataPreviewDetailRows(visibleIndex, hasGroupFooter);
		}
		protected virtual void AddRowAndRaiseRowCreated(GridViewTableRow row) {
			Rows.Add(row);
			Grid.RaiseHtmlRowCreated(row);
		}
		protected virtual void CreateDataPreviewDetailRows(int visibleIndex, bool hasGroupFooter) {
			bool hasDetailRow = RenderHelper.HasDetailRow(visibleIndex);
			if(!DataProxy.IsRowEditing(visibleIndex)) {
				bool hasPreviewRow = RenderHelper.HasPreviewRow(visibleIndex);
				AddRowAndRaiseRowCreated(CreateDataRow(visibleIndex, !hasPreviewRow && hasGroupFooter));
				if(hasPreviewRow) {
					AddRowAndRaiseRowCreated(CreatePreviewRow(visibleIndex, hasGroupFooter));
				}
			}
			if(hasDetailRow) {
				AddRowAndRaiseRowCreated(CreateDetailRow(visibleIndex));
			}
		}
		protected virtual GridViewTableRow CreateDetailRow(int visibleIndex) {
			return new GridViewTableDetailRow(RenderHelper, visibleIndex);
		}
		protected virtual void CreateSampleAdaptiveDetailRow() {
			AddRowAndRaiseRowCreated(new GridViewTableAdaptiveDetailRow(RenderHelper));
		}
		protected virtual GridViewTableRow CreatePreviewRow(int visibleIndex, bool hasGroupFooter) {
			return new GridViewTablePreviewRow(RenderHelper, visibleIndex, hasGroupFooter);
		}
		protected virtual void CreateGroupRow(int visibleIndex, bool hasGroupFooter) {
			AddRowAndRaiseRowCreated(new GridViewTableGroupRow(RenderHelper, visibleIndex, hasGroupFooter));
		}
		protected virtual void CreateGroupFooterRow(List<int> visibleIndexes) {
			for(int i = 0; i < visibleIndexes.Count; i++)
				AddRowAndRaiseRowCreated(new GridViewTableGroupFooterRow(RenderHelper, visibleIndexes[i], i == visibleIndexes.Count - 1));
		}
		protected virtual GridViewTableDataRow CreateDataRow(int visibleIndex) {
			return CreateDataRow(visibleIndex, false);
		}
		protected virtual GridViewTableDataRow CreateDataRow(int visibleIndex, bool hasGroupFooter) {
			return new GridViewTableDataRow(RenderHelper, visibleIndex, dataRowIndex++, hasGroupFooter);
		}
		protected virtual void CreateEditRow(int visibleIndex) { 
			CreateEditRow(visibleIndex, false); 
		}
		protected virtual void CreateEditRow(int visibleIndex, bool hasGroupFooter) {
			if(Grid.SettingsEditing.DisplayEditingRow && !DataProxy.IsNewRowEditing) {
				GridViewTableDataRow dataRow = (GridViewTableDataRow)CreateDataRow(visibleIndex, hasGroupFooter && Grid.SettingsEditing.IsPopupEditForm);
				AddRowAndRaiseRowCreated(dataRow);
			}
			if(Grid.SettingsEditing.IsInline) {
				AddRowAndRaiseRowCreated(new GridViewTableInlineEditRow(RenderHelper, visibleIndex, hasGroupFooter));
			}
			if(Grid.SettingsEditing.IsEditForm) {
				AddRowAndRaiseRowCreated(new GridViewTableEditFormRow(RenderHelper, visibleIndex, hasGroupFooter));
			}
			if(RenderHelper.HasEditingError && !Grid.SettingsEditing.IsPopupEditForm) {
				AddRowAndRaiseRowCreated(new GridViewTableEditingErrorRow(RenderHelper, false, hasGroupFooter));
			}
		}
		protected virtual void CreateEtalonNewDataRow() {
			if(Grid.BatchEditHelper.NewRowInitValues.Count == 0)
				DataProxy.DoInitNewRow();
			if(Grid.BatchEditHelper.NewRowInitValues.Count == 0 && !DataProxy.HasAnyColumn && !string.IsNullOrEmpty(Grid.DataSourceID))
				return;
			var row = new GridViewTableBatchEditEtalonDataRow(RenderHelper, WebDataProxy.NewItemRow);
			AddRowAndRaiseRowCreated(row);
			row.Style["display"] = "none";
		}
		protected override bool RequireUseDblClick {
			get { return !string.IsNullOrEmpty(Grid.ClientSideEvents.RowDblClick) || CanRenderPart(GridViewHtmlTableRenderPart.Content) && RenderHelper.IsBatchEditDblClickStartAction; }
		}
		protected bool ShowFooter { get { return Grid.Settings.ShowFooter; } }
		protected virtual void CreateFooter() {
			if(!ShowFooter) return;			
			AddRowAndRaiseRowCreated(new GridViewTableFooterRow(RenderHelper));
		}
		void CreateReserveTopRows() {
			if(DataProxy.VisibleStartIndex == 0)
				return;
			var startIndex = DataProxy.VisibleStartIndex - GetRowsReserveSize();
			if(DataProxy.IsLastPage) {
				var lastPageRowsDeficit = Math.Max(0, DataProxy.PageSize - DataProxy.VisibleRowCountOnPage);
				startIndex = Math.Max(0, startIndex - lastPageRowsDeficit);
			}
			CreateRows(startIndex, DataProxy.VisibleStartIndex);
		}
		void CreateReserveBottomRows() {
			var startIndex = DataProxy.VisibleStartIndex + DataProxy.PageSize;
			var endIndex = Math.Min(startIndex + GetRowsReserveSize(), DataProxy.VisibleRowCount);
			if(startIndex < endIndex)
				CreateRows(startIndex, endIndex);
		}
		void CreateRows(int startIndex, int endIndex) {
			Enumerable.Range(startIndex, endIndex - startIndex).ToList().ForEach(CreateRow);
		}
		int GetRowsReserveSize() {
			return Convert.ToInt32(Math.Round(DataProxy.PageSize * VirtualScrollingReserveSizeFactor));
		}
	}
	class BottomBorderRemovalHelper {
		List<TableRow> Rows;
		GridViewRenderHelper RenderHelper;
		Dictionary<GridViewTableRow, int> LevelDeltas;
		public BottomBorderRemovalHelper(TableRowCollection rows, GridViewRenderHelper helper)
			: this(rows.OfType<TableRow>().ToList(), helper) {
		}
		public BottomBorderRemovalHelper(List<TableRow> rows, GridViewRenderHelper helper) {
			Rows = rows;
			RenderHelper = helper;
		}
		public void Run() {
			CalcLevelDeltas();
			foreach(GridViewTableRow row in LevelDeltas.Keys) {
				if(LevelDeltas[row] == Int32.MinValue)
					continue;
				RemoveBottomBorder(row.Cells, GetCellCountForBorderRemoval(row));
			}
		}
		void CalcLevelDeltas() {
			LevelDeltas = new Dictionary<GridViewTableRow, int>();
			GridViewTableRow prevRow = null;
			foreach(GridViewTableRow currentRow in CreateGridRowsIterator()) {
				if(prevRow != null)
					LevelDeltas[prevRow] = CalcLevelDelta(prevRow, currentRow);
				prevRow = currentRow;
			}
			if(prevRow != null)
				LevelDeltas[prevRow] = CalcLastRowLevelDelltas(prevRow);
			if(RenderHelper.AllowBatchEditing && RenderHelper.Grid.SettingsEditing.NewItemRowPosition == GridViewNewItemRowPosition.Bottom)
				LoadBatchEditLastVisibleDataRowDelta();
		}
		void LoadBatchEditLastVisibleDataRowDelta() {
			var etalonRow = Rows.OfType<GridViewTableBatchEditEtalonDataRow>().SingleOrDefault();
			if(etalonRow == null)
				return;
			var lastDataRow = Rows.OfType<GridViewTableDataRow>().LastOrDefault(x => x != etalonRow);
			if(lastDataRow != null)
				LevelDeltas[lastDataRow] = LevelDeltas[etalonRow] = CalcLevelDelta(lastDataRow, lastDataRow);
		}
		IEnumerable<GridViewTableRow> CreateGridRowsIterator() {
			foreach(TableRow row in Rows) {
				GridViewTableRow gridRow = row as GridViewTableRow;
				if(gridRow != null)
					yield return gridRow;
			}
		}
		int CalcLevelDelta(GridViewTableRow row, GridViewTableRow nextRow) {
			bool isDataRowAdjacency = (row is GridViewTableDataRow || row is GridViewTablePagerEmptyRow) && (nextRow is GridViewTableDataRow || nextRow is GridViewTablePagerEmptyRow);
			if(isDataRowAdjacency && !RenderHelper.ShowHorizontalGridLine)
				return Int32.MaxValue;
			return GetRowLevel(nextRow) - GetRowLevel(row);
		}
		int CalcLastRowLevelDelltas(GridViewTableRow row) {
			if(RenderHelper.AllowBatchEditing && row is GridViewTableDataRow)
				return RenderHelper.ShowHorizontalGridLine ? Int32.MinValue : Int32.MaxValue;
			if(RenderHelper.UseEndlessPaging)
				return CalcEndlessPagingLastPageRowLevelDelta(row.VisibleIndex);
			bool showBorder = RenderHelper.RequireRenderBottomPagerControl || RenderHelper.ShowVerticalScrolling;
			return showBorder ? Int32.MinValue : Int32.MaxValue;
		}
		int CalcEndlessPagingLastPageRowLevelDelta(int lastRowVisibleIndex) {
			if(!RenderHelper.ShowHorizontalGridLine)
				return int.MaxValue;
			if(lastRowVisibleIndex != RenderHelper.DataProxy.VisibleRowCount - 1)
				return GetRowLevel(lastRowVisibleIndex + 1) - GetRowLevel(lastRowVisibleIndex);
			return 0;
		}
		int GetRowLevel(GridViewTableRow row) {
			return GetRowLevel(row.VisibleIndex);
		}
		int GetRowLevel(int visibleIndex) {
			return RenderHelper.DataProxy.GetRowLevel(visibleIndex);
		}
		int GetCellCountForBorderRemoval(GridViewTableRow row) {
			int delta = LevelDeltas[row];
			if(delta == Int32.MaxValue)
				return row.Cells.Count;
			int result = GetRowLevel(row);
			if(row.RemoveExtraIndentBottomBorder())
				result++;
			if(delta <= 0)
				result += delta;
			return result;
		}
		void RemoveBottomBorder(TableCellCollection cells, int count) {
			if(count > cells.Count)
				return;
			for(int i = 0; i < count; i++) {
				var cellEx = cells[i] as GridTableCell;
				if(cellEx != null)
					cellEx.RemoveBottomBorder = true;
			}
		}
	}
}
