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

using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Web.ASPxPivotGrid.Html;
using DevExpress.Web.Internal;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web.ASPxPivotGrid.HtmlControls {
	public class PivotTableHtmlTable : PivotInternalTable {
		public PivotTableHtmlTable(ASPxPivotGrid pivotGrid)
			: base(pivotGrid) {
		}
		protected ScriptHelper ScriptHelper { get { return PivotGrid.ScriptHelper; } }
		protected PivotWebVisualItems VisualItems { get { return Data.VisualItems; } }
		int ColumnAreaLevelCount { get { return VisualItems.GetLevelCount(true); } }
		int RowAreaColumnCount { get { return Math.Max(VisualItems.GetLevelCount(false), Data.OptionsView.ShowRowHeaders ? Data.GetFieldCountByArea(PivotArea.RowArea) : 1); } }
		int RowCount { get { return VisualItems.RowCount; } }
		int ColumnCount { get { return VisualItems.ColumnCount; } }
		int ColumnSpan {
			get {
				int count = ColumnCount;
				return (count == 0 && Data.OptionsView.ShowColumnHeaders) ? RowAreaColumnCount + 1 : RowAreaColumnCount + count;
			}
		}
		protected override void CreateControlHierarchy() {
			CreateDataAndColumnHeaders();
			InternalTableRow[] columnFieldsRows = CreateRowHeadersAndColumnFields();
			InternalTableRow hiddenRow = CreateRowFieldsAndDataCells();
			if(!PivotGrid.DesignMode)
				CreateVertScrollBarCells(columnFieldsRows[0], hiddenRow);
		}
		void CreateDataAndColumnHeaders() {
			if(Data.OptionsView.ShowDataHeaders || Data.OptionsView.ShowColumnHeaders)
				CreateDataAndColumnHeadersCore();
		}
		void CreateDataAndColumnHeadersCore() {
			InternalTableRow row = new InternalTableRow();
			Rows.Add(row);
			AddHeader(row, PivotArea.DataArea, RowAreaColumnCount);
			if(Data.OptionsView.ShowColumnHeaders || ColumnCount != 0) {
				AddHeader(row, PivotArea.ColumnArea, ColumnCount);
			}
		}
		InternalTableCell CreateHiddenCell(string id, int colSpan, int rowSpan, string cssClass) {
			InternalTableCell cell = new NamingContainerTableCell {
				ID = id,
				ColumnSpan = colSpan,
				RowSpan = rowSpan,
				EnableViewState = false
			};
			if(!String.IsNullOrEmpty(cssClass)) {
				PivotGrid.Styles.CreateStyleCopyByName<AppearanceStyle>(cssClass).AssignToControl(cell);
			}
			cell.Style[HtmlTextWriterStyle.Display] = "none";
			return cell;
		}
		void CreateHorzScrollBarRow() {
			InternalTableRow row = new InternalTableRow();
			row.Cells.Add(CreateHiddenCell(ElementNames.HorzScrollBarRowAreaCell, RowAreaColumnCount, 1, PivotGridStyles.HorzScrollBarRowAreaCellStyleName));
			InternalTableCell scrollBarCell = CreateHiddenCell(ElementNames.HorzScrollBarContainerCell, 1, 1, PivotGridStyles.HorzScrollBarContainerCellStyleName);
			row.Cells.Add(scrollBarCell);
			WebControl outerDiv = RenderUtils.CreateDiv();
			outerDiv.ClientIDMode = System.Web.UI.ClientIDMode.Predictable;
			outerDiv.ID = ElementNames.ScrollableCellViewPortDiv;
			outerDiv.Style[HtmlTextWriterStyle.OverflowX] = "scroll";
			outerDiv.Style[HtmlTextWriterStyle.OverflowY] = "hidden";
			outerDiv.Style[HtmlTextWriterStyle.Width] = "100%";
			outerDiv.Style[HtmlTextWriterStyle.Display] = "none";
			WebControl innerDiv = RenderUtils.CreateDiv();
			innerDiv.ClientIDMode = System.Web.UI.ClientIDMode.Predictable;
			innerDiv.ID = ElementNames.ScrollableCellScrollableDiv;
			outerDiv.Controls.Add(innerDiv);
			WebControl decoratorDiv = RenderUtils.CreateDiv();
			decoratorDiv.ID = ElementNames.ScrollableCellDecoratorDiv;
			decoratorDiv.Style[HtmlTextWriterStyle.Overflow] = "hidden";
			decoratorDiv.Controls.Add(outerDiv);
			WebControl rootDiv = RenderUtils.CreateDiv();
			rootDiv.ID = ElementNames.ScrollableCellRootDiv;
			rootDiv.Controls.Add(decoratorDiv);
			scrollBarCell.Controls.Add(rootDiv);
			row.Cells.Add(CreateHiddenCell(ElementNames.ScrollBarEdgeCell, 1, 1, PivotGridStyles.ScrollBarEdgeCellStyleName));
			Rows.Add(row);
		}
		void CreateVertScrollBarCells(InternalTableRow columnFieldsRow, InternalTableRow rowFieldsRow) {
			InternalTableCell scrollBarCell = CreateHiddenCell(ElementNames.VertScrollBarContainerCell, 1, 1, PivotGridStyles.VertScrollBarContainerCellStyleName);
			if(rowFieldsRow != null)
				rowFieldsRow.Cells.Add(scrollBarCell);
			WebControl outerDiv = RenderUtils.CreateDiv();
			outerDiv.ID = ElementNames.ScrollableCellViewPortDiv;
			outerDiv.ClientIDMode = System.Web.UI.ClientIDMode.Predictable;
			outerDiv.Style[HtmlTextWriterStyle.OverflowY] = "scroll";
			outerDiv.Style[HtmlTextWriterStyle.OverflowX] = "hidden";
			outerDiv.Style[HtmlTextWriterStyle.Height] = "0px";
			outerDiv.Style[HtmlTextWriterStyle.Display] = "none";
			WebControl innerDiv = RenderUtils.CreateDiv();
			innerDiv.ID = ElementNames.ScrollableCellScrollableDiv;
			innerDiv.ClientIDMode = System.Web.UI.ClientIDMode.Predictable;
			outerDiv.Controls.Add(innerDiv);
			WebControl decoratorDiv = RenderUtils.CreateDiv();
			decoratorDiv.ID = ElementNames.ScrollableCellDecoratorDiv;
			decoratorDiv.Style[HtmlTextWriterStyle.Overflow] = "hidden";
			decoratorDiv.Controls.Add(outerDiv);
			WebControl rootDiv = RenderUtils.CreateDiv();
			rootDiv.ID = ElementNames.ScrollableCellRootDiv;
			rootDiv.Controls.Add(decoratorDiv);
			scrollBarCell.Controls.Add(rootDiv);
		}
		InternalTableCell CreateHiddenScrollableTableCell(string id, int rowSpan, int colSpan) {
			InternalTableCell cell = CreateHiddenCell(id, colSpan, rowSpan, null);
			WebControl rootDiv = RenderUtils.CreateDiv();
			rootDiv.ID = ElementNames.ScrollableCellRootDiv;
			WebControl decoratorDiv = RenderUtils.CreateDiv();
			decoratorDiv.ID = ElementNames.ScrollableCellDecoratorDiv;
			decoratorDiv.Style[HtmlTextWriterStyle.Overflow] = "hidden";
			WebControl viewPortDiv = RenderUtils.CreateDiv();
			viewPortDiv.ID = ElementNames.ScrollableCellViewPortDiv;
			viewPortDiv.ClientIDMode = System.Web.UI.ClientIDMode.Predictable;
			WebControl scrollableDiv = RenderUtils.CreateDiv();
			scrollableDiv.ID = ElementNames.ScrollableCellScrollableDiv;
			scrollableDiv.ClientIDMode = System.Web.UI.ClientIDMode.Predictable;
			PivotInternalTable table = new PivotInternalTable(PivotGrid);
			table.AddColGroup(ElementNames.ScrollableTableColGroup).AddCol();
			TableRow emptyRow = RenderUtils.CreateTableRow();
			TableCell emptyCell = RenderUtils.CreateTableCell();
			emptyRow.Cells.Add(emptyCell);
			table.Rows.Add(emptyRow);
			table.ID = ElementNames.ScrollableCellDataTable;
			table.Style[HtmlTextWriterStyle.Position] = "relative";
			viewPortDiv.Controls.Add(table);
			viewPortDiv.Controls.Add(scrollableDiv);
			decoratorDiv.Controls.Add(viewPortDiv);
			rootDiv.Controls.Add(decoratorDiv);
			cell.Controls.Add(rootDiv);
			return cell;
		}
		InternalTableRow[] CreateRowHeadersAndColumnFields() {
			InternalTableRow[] rows = CreateRows(ColumnAreaLevelCount);
			CreateRowHeaders(rows);
			if(!PivotGrid.DesignMode) {
				rows[0].Cells.Add(CreateHiddenScrollableTableCell(ElementNames.ColumnValuesScrollableCell, ColumnAreaLevelCount, 1));
				rows[0].Cells.Add(CreateHiddenCell(ElementNames.VertScrollBarColumnAreaCell, 1, ColumnAreaLevelCount, PivotGridStyles.VertScrollBarColumnAreaCellStyleName));
			}
			CreateColumnFields(rows);
			return rows;
		}
		void CreateColumnFields(InternalTableRow[] columnFields) {
			int itemCount = VisualItems.GetItemCount(true);
			IList<PivotGridHtmlTemplatedFieldValueCell> lastCells = new List<PivotGridHtmlTemplatedFieldValueCell>();
			int maxLastLevelIndex = 0;
			for(int i = 0; i < itemCount; i++) {
				PivotFieldValueItem item = VisualItems.GetItem(true, i);
				PivotGridHtmlTemplatedFieldValueCell cell = PivotGridHtmlFieldCellCreator.CreateFieldValueCell(Data, item, VisualItems.GetSortedBySummaryFields(true, i));
				maxLastLevelIndex = PivotGridHtmlFieldCellCreator.AddMaxLastLevelIndexCell(maxLastLevelIndex, item.MaxLastLevelIndex, lastCells, cell);
				columnFields[item.Level].Controls.Add(cell);
			}
			foreach(PivotGridHtmlTemplatedFieldValueCell cell in lastCells) {
				RenderUtils.AppendDefaultDXClassName(cell, "lastHorzCell");
			}
		}
		void CreateRowHeaders(InternalTableRow[] rows) {
			InternalTableRow row = rows[0];
			int rowSpan = rows.Length;
			row.ID = ScriptHelper.GetHeaderTableID(PivotArea.RowArea);
			if(!Data.PivotGrid.RenderHelper.GetHeadersVisible(PivotArea.RowArea, true) || !Data.OptionsView.ShowRowHeaders) {
				PivotGridHtmlEmptyAreaCellContainer rowAreaContainer = new PivotGridHtmlEmptyAreaCellContainer(Data, PivotArea.RowArea);
				row.Controls.Add(rowAreaContainer);
				rowAreaContainer.RowSpan = rowSpan;
				if(RowAreaColumnCount > 1)
					rowAreaContainer.ColumnSpan = RowAreaColumnCount;
			} else {
				if(Data.OptionsView.RowTotalsLocation == PivotRowTotalsLocation.Tree)
					row = CreateRowTreeHeadersWrapper(row, rowSpan); 
				List<PivotFieldItemBase> fields = Data.GetFieldItemsByArea(PivotArea.RowArea);
				foreach(PivotFieldItemBase field in fields) {
					PivotGridHtmlRowHeaderContainer headerContainer = new PivotGridHtmlRowHeaderContainer(Data, field, rowSpan);
					row.Controls.Add(headerContainer);
				}
				for(int i = fields.Count; i < RowAreaColumnCount; i++) {
					EmptyPivotGridHtmlRowHeaderContainer emptyCell = new EmptyPivotGridHtmlRowHeaderContainer(Data, rowSpan);
					row.Controls.Add(emptyCell);
				}
			}
		}
		InternalTableRow CreateRowTreeHeadersWrapper(InternalTableRow row, int rowSpan) {
			InternalTableCell container = new InternalTableCell();
			container.RowSpan = rowSpan;
			if(RowAreaColumnCount > 1)
				container.ColumnSpan = RowAreaColumnCount;
			row.Cells.Add(container);
			InternalTable containerTable = new InternalTable();
			containerTable.BorderStyle = BorderStyle.None;
			containerTable.CellPadding = 0;
			containerTable.CellSpacing = 0;
			Data.Styles.GetAreaStyle(PivotArea.RowArea).AssignToControl(container);
			container.Controls.Add(containerTable);
			InternalTableRow containerRow = new InternalTableRow();
			containerTable.Rows.Add(containerRow);
			row = containerRow;
			return row;
		}
		InternalTableRow CreateRowFieldsAndDataCells() {
			InternalTableRow hiddenRow = null;
			if(!PivotGrid.DesignMode) {
				hiddenRow = CreateHiddenCellsRow();
				CreateHorzScrollBarRow();
			}
			InternalTableRow[] rows = CreateRows(RowCount);
			CreateRowFieldsAndDataCells(rows, rows);
			return hiddenRow;
		}
		InternalTableRow CreateHiddenCellsRow() {
			InternalTableRow[] rows = CreateRows(1);
			InternalTableRow row = rows[0];
			row.Cells.Add(CreateHiddenScrollableTableCell(ElementNames.RowValuesScrollableCell, 1, RowAreaColumnCount));
			row.Cells.Add(CreateHiddenScrollableTableCell(ElementNames.DataCellsScrollableCell, 1, 1));
			return row;
		}
		void CreateRowFieldsAndDataCells(InternalTableRow[] rowFields, InternalTableRow[] dataCells) {
			int rowIndex = 0, itemCount = VisualItems.GetItemCount(false);
			IList<PivotGridHtmlTemplatedFieldValueCell> lastCells = new List<PivotGridHtmlTemplatedFieldValueCell>();
			int maxLastLevelIndex = 0;
			for(int i = 0; i < itemCount; i++) {
				PivotFieldValueItem item = VisualItems.GetItem(false, i);
				PivotGridHtmlTemplatedFieldValueCell cell = PivotGridHtmlFieldCellCreator.CreateFieldValueCell(Data, item, VisualItems.GetSortedBySummaryFields(false, i));
				rowFields[rowIndex].Controls.Add(cell);
				if(item.IsLastFieldLevel) {
					CreateDataCells(dataCells[rowIndex], item, rowIndex);
					rowIndex++;
				}
				maxLastLevelIndex = PivotGridHtmlFieldCellCreator.AddMaxLastLevelIndexCell(maxLastLevelIndex, item.MinLastLevelIndex + item.SpanCount, lastCells, cell);
			}
			foreach(PivotGridHtmlTemplatedFieldValueCell cell in lastCells) {
				RenderUtils.AppendDefaultDXClassName(cell, "lastVertCell");
			}
		}
		void CreateDataCells(InternalTableRow row, PivotFieldValueItem rowItem, int rowIndex) {
			int lastLevelItemCount = VisualItems.GetLastLevelItemCount(true);
			for(int i = 0; i < lastLevelItemCount; i++) {
				PivotGridHtmlDataCell cell = new PivotGridHtmlDataCell(Data, VisualItems.CreateCellItem(VisualItems.GetLastLevelItem(true, i), rowItem, i, rowIndex));
				if(i == lastLevelItemCount - 1)
					RenderUtils.AppendDefaultDXClassName(cell, "lastHorzCell");
				if(rowIndex == RowCount - 1)
					RenderUtils.AppendDefaultDXClassName(cell, "lastVertCell");
				row.Controls.Add(cell);
			}
		}
	}
}
