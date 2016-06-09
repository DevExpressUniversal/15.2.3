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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.Web.ASPxScheduler.Rendering {
	#region WebViewRenderer
	public class WebViewRenderer {
		#region Fields
		SchedulerViewBase view;
		ISchedulerWebViewInfoBase viewInfo;
		AnchorCollection anchors;
		Table webTable;
		#endregion
		public WebViewRenderer(SchedulerViewBase view, ISchedulerWebViewInfoBase viewInfo) {
			if (view == null)
				Exceptions.ThrowArgumentNullException("view");
			if (viewInfo == null)
				Exceptions.ThrowArgumentNullException("viewInfo");
			this.view = view;
			this.viewInfo = viewInfo;
		}
		#region Properties
		public SchedulerViewBase View { get { return view; } }
		public ISchedulerWebViewInfoBase ViewInfo { get { return viewInfo; } }
		public Table WebTable { get { return webTable; } }
		#endregion
		public virtual void Render(Control parent) {
			SchedulerTable table = ViewInfo.CreateTable();
			this.webTable = RenderMainTable(parent, table);
			anchors = new AnchorCollection();
			ViewInfo.CreateLeftTopAnchor(anchors);
			ViewInfo.CreateRightBottomAnchor(anchors);
			AssignId();
			this.webTable.ID = "horzTable";
		}
		protected internal virtual Table RenderMainTable(Control parent, SchedulerTable table) {
			Table result = table.CreateTable();
			RenderUtils.SetStyleStringAttribute(result, "border-collapse", "separate");
			parent.Controls.Add(result);
			RenderTableCore(result, table, 0, table.Rows.Count, null);
			return result;
		}
		protected internal virtual void RenderTableCore(Table result, SchedulerTable table, int firstRowIndex, int rowCount, TableCell topRightCell) {
			for (int rowIndex = firstRowIndex; rowIndex < rowCount; rowIndex++) {
				InternalTableRow row = new InternalTableRow();
				result.Rows.Add(row);
				RenderCells(row, table.Rows[rowIndex].Cells);
				if (rowIndex == firstRowIndex && topRightCell != null)
					row.Cells.Add(topRightCell);
			}
		}
		protected internal virtual void RenderCells(InternalTableRow row, SchedulerCellCollection cells) {
			int count = cells.Count;
			for (int i = 0; i < count; i++) {
				InternalSchedulerCell cell = (InternalSchedulerCell)cells[i];
				row.Controls.Add(cell);
				if (cell.InnerTables.Count > 0)
					RenderInnerTables(cell);
			}
		}
		protected internal virtual void RenderInnerTables(InternalSchedulerCell cell) {
			SchedulerTableCollection tableCollection = cell.InnerTables;
			int count = tableCollection.Count;
			for (int i = 0; i < count; i++) {
				SchedulerTable table = tableCollection[i];
				RenderMainTable(cell, table);
			}
		}
		protected internal virtual void AssignId() {
			ViewInfo.AssignCellsIds();
		}
		public virtual void GetCreateClientViewInfoScript(StringBuilder sb, string localVarName) {
			WebCellContainerCollection containers = ViewInfo.GetContainers();
			int count = containers.Count;
			int horizontalContainerIndex = 0;
			int verticalContainerIndex = 0;
			for (int i = 0; i < count; i++) {
				IWebCellContainer container = containers[i];
				CellContainerClientInfo containerInfo = GetCellContainerClientInfo(container);
				string containerStartTime = HtmlConvertor.ToScript(containerInfo.StartTime, typeof(DateTime));
				string cellsDuration = ToScriptArray(containerInfo.CellsDurationInfo);
				string cellLocations = ToScriptArray(containerInfo.CellsLocationInfo);
				string args = String.Format("{0}, {1}, {2}, {3}, {4}", containerInfo.TotalCellsCount, containerStartTime, cellsDuration, SchedulerIdHelper.GenerateScriptResourceId(container.Resource), cellLocations);
				if (containers[i].ContainerType == CellContainerType.Horizontal) {
					string comperssedCellHeader = String.Empty;
					if (containerInfo.MiddleCompressedCellsHeaderLocationInfo != null) {
						object[] compressedCellsHeaderLocation = CreateLocationTransmittedObject(containerInfo.MiddleCompressedCellsHeaderLocationInfo);
						comperssedCellHeader = String.Format(", {0}", HtmlConvertor.ToJSON(compressedCellsHeaderLocation));
					}
					sb.AppendFormat("{0}.AddHorizontalContainer({1}, {2}{3});\n", localVarName, horizontalContainerIndex, args, comperssedCellHeader);
					horizontalContainerIndex++;
				}
				else {
					sb.AppendFormat("{0}.AddVerticalContainer({1}, {2});\n", localVarName, verticalContainerIndex, args);
					verticalContainerIndex++;
				}
			}
			RenderInitialDatesScript(sb, localVarName);
			RenderAnchorsScript(sb, localVarName);
			if (View.Type == SchedulerViewType.Timeline) {
				GenerateTimelineHeadersInfo(sb, localVarName);
			}
		}
		protected internal virtual Table GetContainerParentTable(IWebCellContainer container) {
			return this.WebTable;
		}
		protected internal virtual Table GetHeaderParentTable() {
			return this.WebTable;
		}
		private void GenerateTimelineHeadersInfo(StringBuilder sb, string localVarName) {
			WebTimelineViewInfoBase info = (WebTimelineViewInfoBase)this.ViewInfo;
			int count = info.Header.HeaderLevels.Count - 1;
			InternalSchedulerCell lastCell = info.Header.LastBindCell;
			for (int i = 0; i < count; i++) {
				WebTimeScaleHeaderLevel headerLevel = info.Header.HeaderLevels[i];
				GenerateHeaderLevelInfo(headerLevel, i, lastCell, sb, localVarName);
			}
		}
		void GenerateHeaderLevelInfo(WebTimeScaleHeaderLevel headerLevel, int indx, InternalSchedulerCell lastCell, StringBuilder sb, string localVarName) {
			int count = headerLevel.Headers.Count;
			string args = null;
			Table parentTable = GetHeaderParentTable();
			for (int i = 0; i < count; i++) {
				WebTimeScaleHeader cell = headerLevel.Headers[i];
				string cellLocation = CellLocationHelper.GetCellPathLocationString(cell, parentTable);
				string baseCellLocation = CellLocationHelper.GetCellPathLocationString(cell.Offset.LinkCell, parentTable);
				args = String.Format("{0}, {1}, {2}, {3}", indx, cellLocation, cell.Offset.RelativeOffset, baseCellLocation);
				sb.AppendFormat("{0}.AddTimelineHeader({1});\n", localVarName, args);
			}
			if (count > 0) {
				string lastCellLocation = CellLocationHelper.GetCellPathLocationString(lastCell, parentTable);
				args = String.Format("{0}, {1}, {2}, {3}", indx, "[]", 100, lastCellLocation);
				sb.AppendFormat("{0}.AddTimelineHeader({1});\n", localVarName, args);
			}
		}
		protected internal virtual string ToScriptArray(CellsDurationInfo[] cellsDuration) {
			int count = cellsDuration.Length;
			object[] list = new object[2 * count];
			for (int i = 0; i < count; i++) {
				CellsDurationInfo durations = cellsDuration[i];
				long totalMilliseconds = (long)durations.SingleCellDuration.TotalMilliseconds;
				list[2 * i] = durations.CellsCount;
				list[2 * i + 1] = durations.Visible ? totalMilliseconds : -totalMilliseconds;
			}
			return HtmlConvertor.ToJSON(list);
		}
		protected internal virtual string ToScriptArray(CellsLocationInfo[] cellsLocation) {
			int count = cellsLocation.Length;
			object[] list = new object[count];
			for (int i = 0; i < count; i++) {
				CellsLocationInfo location = cellsLocation[i];
				list[i] = CreateLocationTransmittedObject(location);
			}
			return HtmlConvertor.ToJSON(list);
		}
		object[] CreateLocationTransmittedObject(CellsLocationInfo location) {
			if (location.IsCompressedCell)
				return new object[] { location.CellCount, location.FirstRow, location.FirstColumn, location.IsCompressedCell };
			return new object[] { location.CellCount, location.FirstRow, location.FirstColumn };
		}
		protected internal virtual CellContainerClientInfo GetCellContainerClientInfo(IWebCellContainer container) {
			CellContainerClientInfo info = new CellContainerClientInfo();
			info.TotalCellsCount = container.CellCount;
			info.StartTime = container[0].Interval.Start;
			info.CellsDurationInfo = PrepareCompressedCellsDurationInfo(container);
			info.CellsLocationInfo = PrepareCompressedCellsLocationInfo(container);
			info.MiddleCompressedCellsHeaderLocationInfo = PrepareCompressedCellsHeaderLocationInfo(container);
			return info;
		}
		protected internal virtual CellsDurationInfo[] PrepareCompressedCellsDurationInfo(IWebCellContainer container) {
			int count = container.CellCount;
			int cellIndex = 0;
			bool visible = false;
			List<CellsDurationInfo> cellsDurationInfo = new List<CellsDurationInfo>();
			while (cellIndex < count) {
				CellsDurationInfo duration = GetNextDurationInfo(container, cellIndex, visible);
				cellIndex += duration.CellsCount;
				cellsDurationInfo.Add(duration);
				visible = duration.Visible;
			}
			return cellsDurationInfo.ToArray();
		}
		protected internal virtual CellsLocationInfo[] PrepareCompressedCellsLocationInfo(IWebCellContainer container) {
			Table containerParentTable = GetContainerParentTable(container);
			int count = container.CellCount;
			int cellIndex = 0;
			List<CellsLocationInfo> cellsLocationInfo = new List<CellsLocationInfo>();
			while (cellIndex < count) {
				CellsLocationInfo location = GetNextLocationInfo(container, cellIndex, containerParentTable, ObtainContentCell);
				cellIndex += location.CellCount;
				cellsLocationInfo.Add(location);
			}
			return cellsLocationInfo.ToArray();
		}
		protected internal virtual CellsLocationInfo PrepareCompressedCellsHeaderLocationInfo(IWebCellContainer container) {
			Table containerParentTable = GetContainerParentTable(container);
			int count = container.CellCount;
			int lastHeaderCellIndx = -1;
			for (int i = 0; i < count; i++) {
				WebDateCell cell = container[i] as WebDateCell;
				if (cell == null)
					continue;
				if (cell.IsCompressedCell)
					lastHeaderCellIndx = i;
			}
			if (lastHeaderCellIndx < 0)
				return null;
			return GetNextLocationInfo(container, lastHeaderCellIndx, containerParentTable, ObtainHeaderCell);
		}
		protected internal virtual CellsLocationInfo GetNextLocationInfo(IWebCellContainer container, int startIndex, Table containerParentTable, ObtainCellHandler obtainCellHandler) {
			CellsLocationInfo result = new CellsLocationInfo();
			CalcLocationBegin(container, startIndex, containerParentTable, result, obtainCellHandler);
			if (container.ContainerType == CellContainerType.Horizontal) {
				result.CellCount = GetNextLocationInfoCoreHorzContainer(container, startIndex, containerParentTable, result, obtainCellHandler);
			}
			else
				result.CellCount = GetNextLocationInfoCoreVertContainer(container, startIndex, containerParentTable, result, obtainCellHandler);
			return result;
		}
		protected internal virtual void CalcLocationBegin(IWebCellContainer container, int startIndex, Table containerParentTable, CellsLocationInfo result, ObtainCellHandler obtainCellHandler) {
			TableCell cell = obtainCellHandler(container[startIndex]);
			TableRow firstRow = (TableRow)cell.Parent;
			result.FirstRow = containerParentTable.Rows.GetRowIndex(firstRow);
			result.FirstColumn = firstRow.Cells.GetCellIndex(cell);
			result.IsCompressedCell = IsCompressedCell(container[startIndex]);
		}
		protected internal delegate TableCell ObtainCellHandler(IWebTimeCell webTimeCell);
		protected virtual TableCell ObtainContentCell(IWebTimeCell webTimeCell) {
			return webTimeCell.ContentCell;
		}
		protected virtual TableCell ObtainHeaderCell(IWebTimeCell cell) {
			WebDateCell webDateCell = cell as WebDateCell;
			if (webDateCell == null)
				return null;
			return webDateCell.Header;
		}
		protected internal virtual int GetNextLocationInfoCoreHorzContainer(IWebCellContainer container, int startIndex, Table containerParentTable, CellsLocationInfo result, ObtainCellHandler obtainCellHandler) {
			int count = container.CellCount;
			for (int i = startIndex + 1; i < count; i++) {
				if (IsCompressedCell(container[i]))
					return i - startIndex;
				TableCell cell = obtainCellHandler(container[i]);
				TableRow row = (TableRow)cell.Parent;
				int rowIndex = containerParentTable.Rows.GetRowIndex(row);
				if (rowIndex != result.FirstRow)
					return i - startIndex;
				int columnIndex = row.Cells.GetCellIndex((TableCell)cell);
				if (columnIndex != result.FirstColumn + i)
					return i - startIndex;
			}
			return count - startIndex;
		}
		protected internal virtual int GetNextLocationInfoCoreVertContainer(IWebCellContainer container, int startIndex, Table containerParentTable, CellsLocationInfo result, ObtainCellHandler obtainCellHandler) {
			int count = container.CellCount;
			for (int i = startIndex + 1; i < count; i++) {
				TableCell cell = obtainCellHandler(container[i]);
				TableRow row = (TableRow)cell.Parent;
				int rowIndex = containerParentTable.Rows.GetRowIndex(row);
				if (rowIndex != result.FirstRow + i)
					return i - startIndex;
				int columnIndex = row.Cells.GetCellIndex((TableCell)cell);
				if (columnIndex != result.FirstColumn)
					return i - startIndex;
			}
			return count - startIndex;
		}
		bool IsCompressedCell(IWebTimeCell cell) {
			WebDateCell dateCell = cell as WebDateCell;
			if (dateCell == null)
				return false;
			return dateCell.IsCompressedCell;
		}
		protected internal virtual CellsDurationInfo GetNextDurationInfo(IWebCellContainer container, int startIndex, bool prevDurationInfoVisible) {
			if (container == null)
				Exceptions.ThrowArgumentNullException("container");
			ITimeCell cell = container[startIndex];
			if (prevDurationInfoVisible) {
				XtraSchedulerDebug.Assert(startIndex > 0);
				ITimeCell prevCell = container[startIndex - 1];
				if (cell.Interval.Start != prevCell.Interval.End)
					return new CellsDurationInfo(0, cell.Interval.Start - prevCell.Interval.End, false);
			}
			TimeSpan duration = cell.Interval.Duration;
			DateTime prevCellIntervalEnd = container[startIndex].Interval.End;
			int index = startIndex + 1;
			while (index < container.CellCount) {
				cell = container[index];
				TimeInterval interval = cell.Interval;
				if (interval.Duration != duration || interval.Start != prevCellIntervalEnd)
					break;
				prevCellIntervalEnd = interval.End;
				index++;
			}
			return new CellsDurationInfo(index - startIndex, duration, true);
		}
		public virtual void ApplyStyles() {
		}
		protected internal virtual void RenderAnchorsScript(StringBuilder sb, string localVarName) {
			int count = anchors.Count;
			for (int i = 0; i < count; i++) {
				Anchor anchor = anchors[i];
				string resourceId = SchedulerIdHelper.GenerateResourceId(anchor.Resource);
				TableCell cell = anchor.Cell;
				TableRow row = (TableRow)cell.Parent;
				Table table = (Table)row.Parent;
				int cellIndex = row.Cells.GetCellIndex(cell);
				int rowIndex = table.Rows.GetRowIndex(row);
				sb.AppendFormat("{0}.AddAnchor({1}, {2}, \"{3}_{4}\");\n", localVarName, rowIndex, cellIndex, resourceId, anchor.AnchorType);
			}
		}
		protected internal virtual void RenderInitialDatesScript(StringBuilder sb, string localVarName) {
			sb.AppendFormat("{0}.ClearDatesForFormats();\n", localVarName);
			HeaderFormatSeparatorBase headerFormatSeparator = ViewInfo.HeaderFormatSeparator;
			if (headerFormatSeparator == null)
				return;
			headerFormatSeparator.RemoveHeadersWithCustomCaption();
			Table parentTable = GetHeaderParentTable();
			headerFormatSeparator.RenderScript(sb, localVarName, parentTable);
		}
	}
	#endregion
	#region DayViewWebRenderer
	public class DayViewWebRenderer : WebViewRenderer {
		TableCell scrollHeaderCell;
		TableCell bodyTableCell;
		Table horizontalContainersTable;
		Table verticalContainersTable;
		int syncColumnCount;
		int syncMasterRowIndex;
		int syncSlaveRowIndex;
		public DayViewWebRenderer(DayView view, ISchedulerWebViewInfoBase viewInfo)
			: base(view, viewInfo) {
		}
		public new DayView View { get { return (DayView)base.View; } }
		internal TableCell ScrollHeaderCell {
			get { return scrollHeaderCell; }
		}
		protected internal override Table RenderMainTable(Control parent, SchedulerTable table) {
			WebDayViewInfoBase viewInfo = (WebDayViewInfoBase)ViewInfo;
			Table result = RenderUtils.CreateTable();
			result.Width = Unit.Percentage(100);
			parent.Controls.Add(result);
			int maxColumnCount = CalcMaxColumnCount(table);
			if (viewInfo.TimeRulerHeaderSpan > 0) {
				this.syncColumnCount = maxColumnCount;
				RenderHeaderTable(result, table, maxColumnCount);
			}
			else
				this.syncColumnCount = 0;
			RenderBodyTable(result, table, maxColumnCount);
			return result;
		}
		void PrepareSyncCells(Table headerTable, Table bodyTable, int slaveRowIndex, int masterRowIndex) {
			TableRow slaveRow = (headerTable == null) ? null : headerTable.Rows[slaveRowIndex];
			TableRow masterRow = bodyTable.Rows[masterRowIndex];
			TableRow baseRow = bodyTable.Rows[0];
			int count = baseRow.Cells.Count;
			for (int i = 0; i < count; i++) {
				InternalSchedulerCell baseCell = baseRow.Cells[i] as InternalSchedulerCell;
				if (baseCell == null)
					continue;
				baseCell.ApplyStyleToWebControl(masterRow.Cells[i]);
				if (slaveRow != null) {
					baseCell.ApplyStyleToWebControl(slaveRow.Cells[i]);
					ClearVerticalPaddingsAndBorders(slaveRow.Cells[i]);
				}
			}
			if (slaveRow != null) {
				slaveRow.Style.Add(HtmlTextWriterStyle.Visibility, "visible");
				slaveRow.Height = Unit.Pixel(1);
				if (IsRequareScrollbar())
					ApplySlaveRowScrollbarCellStyles(slaveRow);
			}
		}
		void ClearVerticalPaddingsAndBorders(TableCell cell) {
			cell.Style.Add(HtmlTextWriterStyle.PaddingTop, "0");
			cell.Style.Add(HtmlTextWriterStyle.PaddingBottom, "0");
			cell.Style.Add("border-top-width", "0");
			cell.Style.Add("border-bottom-width", "0");
			cell.Height = Unit.Pixel(1);
		}
		protected internal virtual void RenderHeaderTable(Table parent, SchedulerTable table, int maxColumnCount) {
			WebDayViewInfoBase viewInfo = (WebDayViewInfoBase)ViewInfo;
			InternalTableRow row = new InternalTableRow();
			parent.Rows.Add(row);
			InternalTableCell cell = new InternalTableCell();
			row.Cells.Add(cell);
			WebControl horizontalContainer = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			cell.Controls.Add(horizontalContainer);
			RenderUtils.AppendDefaultDXClassName(horizontalContainer, "dxscDayHdrsContainer");
			RenderUtils.SetStyleStringAttribute(horizontalContainer, "z-index", "1");
			horizontalContainer.ID = "horizontalContainer";
			Table headerTable = RenderUtils.CreateTable();
			RenderUtils.AppendDefaultDXClassName(headerTable, "dxscDayHdrsTbl");
			horizontalContainer.Controls.Add(headerTable);
			this.syncSlaveRowIndex = RenderSyncSlaveRow(headerTable, maxColumnCount);
			PrepareSchedulerHeaderTableForScrollSupport(table, viewInfo);
			RenderTableCore(headerTable, table, 0, viewInfo.TimeRulerHeaderSpan, ScrollHeaderCell);
			this.horizontalContainersTable = headerTable;
			horizontalContainersTable.ID = "horzContainerTable";
			DayView view = (DayView)View;
			if (view.ShowAllDayArea) {
				WebControl scrollableContainer = RenderUtils.CreateWebControl(RenderUtils.Browser.IsIE ? HtmlTextWriterTag.Span : HtmlTextWriterTag.Div);
				scrollableContainer.ID = "scrollableContainer";
				horizontalContainer.Controls.Add(scrollableContainer);
				if (!view.Styles.AllDayAreaHeight.IsEmpty)
					RenderUtils.AppendDefaultDXClassName(scrollableContainer, "dxscDayADAScrollContainer");
				else
					RenderUtils.AppendDefaultDXClassName(scrollableContainer, "dxscDayADAContainer");
			}
		}
		protected virtual void PrepareSchedulerHeaderTableForScrollSupport(SchedulerTable table, WebDayViewInfoBase viewInfo) {
			if (IsRequareScrollbar()) {
				this.scrollHeaderCell = RenderUtils.CreateTableCell();
				ScrollHeaderCell.RowSpan = viewInfo.TimeRulerHeaderSpan;
			}
			else {
				for (int i = 0; i < table.Rows.Count; i++) {
					SchedulerCellCollection cells = table.Rows[i].Cells;
					int lastCellIndex = cells.Count - 1;
					InternalSchedulerCell lastCellInRow = (InternalSchedulerCell)cells[lastCellIndex];
					lastCellInRow.IgnoreBorderSide |= IgnoreBorderSide.Right;
				}
			}
		}
		protected internal virtual void RenderBodyTable(Table parent, SchedulerTable table, int maxColumnCount) {
			WebDayViewInfoBase viewInfo = (WebDayViewInfoBase)ViewInfo;
			DayView view = (DayView)View;
			InternalTableRow row = new InternalTableRow();
			parent.Rows.Add(row);
			bodyTableCell = new InternalTableCell();
			row.Cells.Add(bodyTableCell);
			WebControl scrollContainer = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			bodyTableCell.Controls.Add(scrollContainer);
			RenderUtils.AppendDefaultDXClassName(scrollContainer, "dxscDayScrollContainer");
			if (!IsRequareScrollbar()) {
				scrollContainer.Style.Add(HtmlTextWriterStyle.OverflowY, "hidden");
			}
			else {
				scrollContainer.Height = view.Styles.ScrollAreaHeight;
			}
			scrollContainer.ID = "verticalScrollContainer";
			WebControl scrollContent = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			scrollContainer.Controls.Add(scrollContent);
			scrollContent.ID = "verticalContainer";
			RenderUtils.AppendDefaultDXClassName(scrollContent, "dxscDayScrollContent");
			if (RenderUtils.Browser.IsIE)
				scrollContent.Style.Add(HtmlTextWriterStyle.Width, "expression(offsetParent.clientWidth);");
			Table bodyTable = RenderUtils.CreateTable();
			scrollContent.Controls.Add(bodyTable);
			RenderUtils.AppendDefaultDXClassName(bodyTable, "dxscDayScrollBodyTable");
			bodyTable.ID = "vertTable";
			RenderTableCore(bodyTable, table, viewInfo.TimeRulerHeaderSpan, table.Rows.Count, null);
			this.syncMasterRowIndex = RenderSyncMasterRow(bodyTable, maxColumnCount);
			this.verticalContainersTable = bodyTable;
		}
		protected internal virtual void AddMoreButtonControl(WebControl MoreButtonControl) {
			this.bodyTableCell.Controls.Add(MoreButtonControl);
		}
		protected internal virtual int CalcMaxColumnCount(SchedulerTable table) {
			if (table.Rows.Count <= 0)
				return 1;
			SchedulerRow row = table.Rows[0];
			int result = 0;
			int count = row.Cells.Count;
			for (int i = 0; i < count; i++)
				result += Math.Max(1, row.Cells[i].ColumnSpan);
			return result;
		}
		protected internal virtual int RenderSyncMasterRow(Table table, int cellCount) {
			InternalTableRow row = new InternalTableRow();
			table.Rows.Add(row);
			for (int i = 0; i < cellCount; i++) {
				TableCell cell = RenderUtils.CreateTableCell();
				row.Cells.Add(cell);
			}
			return table.Rows.Count - 1;
		}
		protected internal virtual int RenderSyncSlaveRow(Table table, int cellCount) {
			InternalTableRow row = new InternalTableRow();
			table.Rows.Add(row);
			int slaveRowCellCount = cellCount;
			if (IsRequareScrollbar()) {
				slaveRowCellCount += 1;
			}
			for (int i = 0; i < slaveRowCellCount; i++) {
				TableCell cell = RenderUtils.CreateTableCell();
				row.Cells.Add(cell);
			}
			return table.Rows.Count - 1;
		}
		bool IsRequareScrollbar() {
			WebDayViewInfoBase viewInfo = (WebDayViewInfoBase)ViewInfo;
			return !viewInfo.View.Styles.ScrollAreaHeight.IsEmpty;
		}
		protected internal virtual void ApplySlaveRowScrollbarCellStyles(TableRow slaveRow) {
			AppearanceStyleBase scrollbarCellStyle = View.Styles.GetSlaveRowScrollItemStyle();
			scrollbarCellStyle.AssignToControl(slaveRow.Cells[slaveRow.Cells.Count - 1]);
		}
		public override void ApplyStyles() {
			base.ApplyStyles();
			if (ScrollHeaderCell != null) {
				DayViewStylesHelper helper = new DayViewStylesHelper(View, ViewInfo, View.Control.Styles);
				helper.GetRightTopCornerStyle().AssignToControl(ScrollHeaderCell);
			}
			PrepareSyncCells(this.horizontalContainersTable, this.verticalContainersTable, this.syncSlaveRowIndex, this.syncMasterRowIndex);
		}
		protected internal override Table GetContainerParentTable(IWebCellContainer container) {
			if (container.ContainerType == CellContainerType.Horizontal)
				return this.horizontalContainersTable;
			else
				return this.verticalContainersTable;
		}
		protected internal override Table GetHeaderParentTable() {
			return this.horizontalContainersTable;
		}
		public override void GetCreateClientViewInfoScript(StringBuilder sb, string localVarName) {
			base.GetCreateClientViewInfoScript(sb, localVarName);
			sb.AppendFormat("{0}.SetSyncCells({1},{2},{3});\n", localVarName, syncColumnCount, syncMasterRowIndex, syncSlaveRowIndex);
			DayView view = (DayView)View;
			string currentTimeScaleMenuName = String.Format("{0}!{1}", SchedulerMenuItemId.SwitchTimeScale, view.TimeScale);
			sb.AppendFormat("{0}.SetCurrentTimeScaleMenuItemName(\"{1}\");", localVarName, currentTimeScaleMenuName);
			SetClientTimeRulersInfo(sb, localVarName);
		}
		void SetClientTimeRulersInfo(StringBuilder sb, string localVarName) {
			WebDayViewInfoBase viewInfo = (WebDayViewInfoBase)ViewInfo;
			List<WebTimeRuler> timeRulers = viewInfo.TimeRulers;
			int timeRulerCount = timeRulers.Count;
			String[] result = new String[timeRulerCount];
			for (int i = 0; i < timeRulerCount; i++) {
				WebTimeRuler ruler = timeRulers[i];
				if (ruler.Elements.Count < 1)
					continue;
				WebTimeRulerElement timeRulerItem = ruler.Elements[0];
				WebTimeRulerItem cell = timeRulerItem.GetLeftItem();
				string location = CellLocationHelper.GetCellLocationString(cell, this.verticalContainersTable);
				bool timeMarkerVisibility = CalculateTimeMarkerVisibility(View.TimeMarkerVisibility, ruler.TimeRuler.TimeMarkerVisibility);
				result[i] = String.Format("{{p:{0},v:{1}}}", location, HtmlConvertor.ToJSON(timeMarkerVisibility));
			}
			sb.AppendFormat("{0}.SetTimeRulers([{1}]);", localVarName, String.Join(",", result));
		}
		bool CalculateTimeMarkerVisibility(TimeMarkerVisibility timeMarkerVisibility, TimeMarkerVisibility? timeRulerTimeMarkerVisibility) {
			TimeMarkerVisibility actualTimeMarkerVisibility = (timeRulerTimeMarkerVisibility.HasValue) ? timeRulerTimeMarkerVisibility.Value : timeMarkerVisibility;
			if (actualTimeMarkerVisibility == TimeMarkerVisibility.Never)
				return false;
			if (actualTimeMarkerVisibility == TimeMarkerVisibility.Always)
				return true;
			DateTime clientNowTime = View.Control.TimeZoneHelper.ToClientTime(DateTime.Now, TimeZoneInfo.Local.Id);
			return View.GetVisibleIntervals().Interval.Contains(clientNowTime);
		}
	}
	#endregion
	#region TimelineViewWebRenderer
	public class TimelineViewWebRenderer : WebViewRenderer {
		public TimelineViewWebRenderer(SchedulerViewBase view, ISchedulerWebViewInfoBase viewInfo)
			: base(view, viewInfo) {
		}
		public override void GetCreateClientViewInfoScript(StringBuilder sb, string localVarName) {
			base.GetCreateClientViewInfoScript(sb, localVarName);
			TimelineView view = (TimelineView)this.View;
			int count = view.Scales.Count;
			int[] visibleScales = new int[count];
			int[] enabledScales = new int[count];
			for (int i = 0; i < count; i++) {
				TimeScale scale = view.Scales[i];
				visibleScales[i] = scale.Visible ? 1 : 0;
				enabledScales[i] = scale.Enabled ? 1 : 0;
			}
			sb.AppendFormat("{0}.MeasureVRHLocation = {1};\n", localVarName, GetMeasureVResourceHeaderPosition());
			sb.AppendFormat("{0}.SetTimelineScalesInfo({1},{2});\n", localVarName, HtmlConvertor.ToJSON(enabledScales),
				HtmlConvertor.ToJSON(visibleScales));
		}
		protected virtual string GetMeasureVResourceHeaderPosition() {
			string result = "null";
			ISchedulerCellProvider cellProvider = ViewInfo as ISchedulerCellProvider;
			if (cellProvider != null) {
				InternalSchedulerCell header = cellProvider.GetCell(WebElementType.VerticalResourceHeader, 0);
				if (header != null)
					result = GetResourceHeaderLocationString(header);
			}
			return result;
		}
		protected internal virtual string GetResourceHeaderLocationString(InternalSchedulerCell header) {
			TableRow row = (TableRow)header.Parent;
			Table parent = (Table)row.Parent;
			return CellLocationHelper.GetCellLocationString(header, parent);
		}
	}
	#endregion
	#region CellContainerClientInfo
	public class CellContainerClientInfo {
		CellsDurationInfo[] cellsDurationInfo;
		CellsLocationInfo[] cellsLocationInfo;
		CellsLocationInfo middleCompressedCellsHeaderLocationInfo;
		int totalCellsCount;
		DateTime startTime;
		public CellsDurationInfo[] CellsDurationInfo { get { return cellsDurationInfo; } set { cellsDurationInfo = value; } }
		public CellsLocationInfo[] CellsLocationInfo { get { return cellsLocationInfo; } set { cellsLocationInfo = value; } }
		public CellsLocationInfo MiddleCompressedCellsHeaderLocationInfo { get { return middleCompressedCellsHeaderLocationInfo; } set { middleCompressedCellsHeaderLocationInfo = value; } }
		public int TotalCellsCount { get { return totalCellsCount; } set { totalCellsCount = value; } }
		public DateTime StartTime { get { return startTime; } set { startTime = value; } }
	}
	#endregion
	#region CellsDurationInfo
	public class CellsDurationInfo {
		int cellsCount;
		TimeSpan singleCellDuration;
		bool visible;
		public CellsDurationInfo(int cellsCount, TimeSpan singleCellDuration, bool visible) {
			this.cellsCount = cellsCount;
			this.singleCellDuration = singleCellDuration;
			this.visible = visible;
		}
		public int CellsCount { get { return cellsCount; } }
		public bool Visible { get { return visible; } }
		public TimeSpan SingleCellDuration { get { return singleCellDuration; } }
	}
	#endregion
	public class CellsLocationInfo {
		int cellCount;
		int firstRow;
		int firstColumn;
		bool isCompressedCell;
		public int CellCount { get { return cellCount; } set { cellCount = value; } }
		public int FirstRow { get { return firstRow; } set { firstRow = value; } }
		public int FirstColumn { get { return firstColumn; } set { firstColumn = value; } }
		public bool IsCompressedCell { get { return isCompressedCell; } set { isCompressedCell = value; } }
	}
	public abstract class HorizontalMerger {
		public static HorizontalMerger Create(SchedulerTable leftTable, SchedulerTable rightTable) {
			if (!CanMergeHorizontal(leftTable, rightTable))
				Exceptions.ThrowInternalException();
			int leftTableTopFixedSpanRowCount = CalculateTopFixedSpanRowCount(leftTable);
			int rightTableTopFixedSpanRowCount = CalculateTopFixedSpanRowCount(rightTable);
			if (leftTableTopFixedSpanRowCount != 0 && rightTableTopFixedSpanRowCount != 0)
				return new HorizontalFixedRowMerger();
			else
				return new HorizontalOrdinaryMerger();
		}
		static bool CanMergeHorizontal(SchedulerTable leftTable, SchedulerTable rightTable) {
			int leftTableRowCount = leftTable.Rows.Count;
			int rightTableRowCount = rightTable.Rows.Count;
			if (leftTableRowCount == rightTableRowCount)
				return true;
			int leftTableTopFixedSpanRowCount = CalculateTopFixedSpanRowCount(leftTable);
			int rightTableTopFixedSpanRowCount = CalculateTopFixedSpanRowCount(rightTable);
			if (leftTableTopFixedSpanRowCount == 0 || rightTableTopFixedSpanRowCount == 0)
				return true;
			int leftTableFreeRowCount = leftTableRowCount - leftTableTopFixedSpanRowCount;
			int rightTableFreeRowCount = rightTableRowCount - rightTableTopFixedSpanRowCount;
			if (leftTableFreeRowCount != 1 && rightTableFreeRowCount != 1)
				return false;
			if (leftTableFreeRowCount == 1)
				return rightTableRowCount > leftTableTopFixedSpanRowCount;
			else
				return leftTableRowCount > rightTableTopFixedSpanRowCount;
		}
		protected internal static int CalculateTopFixedSpanRowCount(SchedulerTable table) {
			SchedulerRowCollection rows = table.Rows;
			int rowCount = rows.Count;
			for (int rowIndex = 0; rowIndex < rowCount; rowIndex++) {
				SchedulerCellCollection cells = rows[rowIndex].Cells;
				int columnCount = cells.Count;
				for (int columnIndex = 0; columnIndex < columnCount; columnIndex++) {
					if (!(cells[columnIndex]).FixedRowSpan)
						return rowIndex;
				}
			}
			return rowCount;
		}
		protected HorizontalMerger() {
		}
		public SchedulerTable Merge(SchedulerTable leftTable, SchedulerTable rightTable) {
			int resultRowCount = GetResultCount(leftTable.Rows.Count, rightTable.Rows.Count);
			SchedulerTable result = CreateEmptyTable(resultRowCount);
			int leftTableSpanFactor = ModifyTable(leftTable, resultRowCount);
			AppendTableToRight(result, leftTable, leftTableSpanFactor);
			int rightTableSpanFactor = ModifyTable(rightTable, resultRowCount);
			AppendTableToRight(result, rightTable, rightTableSpanFactor);
			return result;
		}
		protected abstract int ModifyTable(SchedulerTable table, int resultRowCount);
		protected abstract int GetResultCount(int leftTableRowCount, int rightTableRowCount);
		void AppendTableToRight(SchedulerTable destination, SchedulerTable source, int rowSpanFactor) {
			int sourceRowCount = source.Rows.Count;
			for (int rowIndex = 0; rowIndex < sourceRowCount; rowIndex++) {
				SchedulerCellCollection cells = source.Rows[rowIndex].Cells;
				destination.Rows[rowIndex * rowSpanFactor].Cells.AddRange(cells);
			}
		}
		internal SchedulerTable CreateEmptyTable(int rowCount) {
			SchedulerTable table = new SchedulerTable();
			for (int i = 0; i < rowCount; i++)
				table.Rows.Add(new SchedulerRow());
			return table;
		}
		internal static int GetActualRowSpan(int rowSpan) {
			return rowSpan == 0 ? 1 : rowSpan;
		}
	}
	public class HorizontalOrdinaryMerger : HorizontalMerger {
		protected override int ModifyTable(SchedulerTable table, int resultRowCount) {
			int tableRowCount = table.Rows.Count;
			if (tableRowCount == 0)
				return 1;
			int rowSpanFactor = resultRowCount / tableRowCount;
			for (int rowIndex = 0; rowIndex < tableRowCount; rowIndex++) {
				SchedulerCellCollection cells = table.Rows[rowIndex].Cells;
				UpdateRowSpans(cells, rowSpanFactor);
			}
			return rowSpanFactor;
		}
		protected override int GetResultCount(int leftTableRowCount, int rightTableRowCount) {
			if (leftTableRowCount == 0)
				return rightTableRowCount;
			if (rightTableRowCount == 0)
				return leftTableRowCount;
			return MathUtils.LCM(leftTableRowCount, rightTableRowCount);
		}
		internal void UpdateRowSpans(SchedulerCellCollection cells, int rowSpanFactor) {
			if (cells == null)
				Exceptions.ThrowArgumentNullException("cells");
			int cellCount = cells.Count;
			for (int cellIndex = 0; cellIndex < cellCount; cellIndex++) {
				ISchedulerCell cell = cells[cellIndex];
				if (cell.FixedRowSpan && rowSpanFactor != 1)
					Exceptions.ThrowInternalException();
				cell.RowSpan = GetActualRowSpan(cell.RowSpan) * rowSpanFactor;
			}
		}
	}
	public class HorizontalFixedRowMerger : HorizontalMerger {
		protected override int ModifyTable(SchedulerTable table, int resultRowCount) {
			int tableRowCount = table.Rows.Count;
			int tableTopFixedSpanRowCount = CalculateTopFixedSpanRowCount(table);
			if (tableRowCount - tableTopFixedSpanRowCount == 1)
				UpdateRowSpans(table.Rows[tableRowCount - 1], resultRowCount - tableTopFixedSpanRowCount);
			return 1;
		}
		protected override int GetResultCount(int leftTableRowCount, int rightTableRowCount) {
			return Math.Max(leftTableRowCount, rightTableRowCount);
		}
		void UpdateRowSpans(SchedulerRow row, int newRowSpan) {
			if (row == null)
				Exceptions.ThrowArgumentNullException("row");
			SchedulerCellCollection cells = row.Cells;
			if (newRowSpan <= 0)
				Exceptions.ThrowArgumentException("newRowSpan", newRowSpan);
			int cellCount = cells.Count;
			for (int cellIndex = 0; cellIndex < cellCount; cellIndex++) {
				ISchedulerCell cell = cells[cellIndex];
				if (cell.FixedRowSpan)
					Exceptions.ThrowInternalException();
				cell.RowSpan = newRowSpan;
			}
		}
	}
	public class VerticalMerger {
		public VerticalMerger() {
		}
		public SchedulerTable Merge(SchedulerTable topTable, SchedulerTable bottomTable) {
			int topTableColumnCount = CalculateColumnCount(topTable);
			int bottomTableColumnCount = CalculateColumnCount(bottomTable);
			int resultTableColumnCount = CalculateResultTableColumnCount(topTableColumnCount, bottomTableColumnCount);
			ModifyTableColumnSpan(topTable, topTableColumnCount, resultTableColumnCount);
			ModifyTableColumnSpan(bottomTable, bottomTableColumnCount, resultTableColumnCount);
			SchedulerTable resultTable = new SchedulerTable();
			AppendTableToBottom(resultTable, topTable);
			AppendTableToBottom(resultTable, bottomTable);
			return resultTable;
		}
		int CalculateResultTableColumnCount(int topTableColumnCount, int bottomTableColumnCount) {
			if (topTableColumnCount == 0)
				return bottomTableColumnCount;
			if (bottomTableColumnCount == 0)
				return topTableColumnCount;
			return MathUtils.LCM(topTableColumnCount, bottomTableColumnCount);
		}
		void AppendTableToBottom(SchedulerTable destinationTable, SchedulerTable sourceTable) {
			destinationTable.Rows.AddRange(sourceTable.Rows);
		}
		void ModifyTableColumnSpan(SchedulerTable table, int tableColumnCount, int resultTableColumnCount) {
			if (tableColumnCount == 0)
				return;
			int columnSpanFactor = resultTableColumnCount / tableColumnCount;
			SchedulerRowCollection rows = table.Rows;
			int rowCount = rows.Count;
			for (int rowIndex = 0; rowIndex < rowCount; rowIndex++) {
				SchedulerCellCollection cells = rows[rowIndex].Cells;
				int cellCount = cells.Count;
				for (int cellIndex = 0; cellIndex < cellCount; cellIndex++)
					cells[cellIndex].ColumnSpan = GetActualColumnSpan(cells[cellIndex].ColumnSpan) * columnSpanFactor;
			}
		}
		int CalculateColumnCount(SchedulerTable topTable) {
			if (topTable.Rows.Count == 0)
				return 0;
			SchedulerCellCollection cells = topTable.Rows[0].Cells;
			int cellCount = cells.Count;
			int columnCount = 0;
			for (int cellIndex = 0; cellIndex < cellCount; cellIndex++)
				columnCount += GetActualColumnSpan(cells[cellIndex].ColumnSpan);
			return columnCount;
		}
		int GetActualColumnSpan(int columnSpan) {
			return columnSpan == 0 ? 1 : columnSpan;
		}
	}
	public static class TableHelper {
		internal static TimeOfDayIntervalCollection SplitInterval(TimeOfDayInterval interval, TimeSpan timeScale) {
			TimeOfDayIntervalCollection intervals = new TimeOfDayIntervalCollection();
			TimeSpan start = interval.Start;
			while (start < interval.End) {
				intervals.Add(new TimeOfDayInterval(start, start + timeScale));
				start += timeScale;
			}
			return intervals;
		}
		internal static void StretchLastRow(SchedulerTable table, int lastRowNewSpan) {
			int rowCount = table.Rows.Count;
			if (rowCount == 0)
				return;
			SchedulerRow lastRow = table.Rows[rowCount - 1];
			int cellsCount = lastRow.Cells.Count;
			if (cellsCount == 0)
				return;
			int maxRowSpan = 0;
			for (int i = 0; i < cellsCount; i++) {
				maxRowSpan = Math.Max(maxRowSpan, lastRow.Cells[i].RowSpan);
				lastRow.Cells[i].RowSpan = lastRowNewSpan;
			}
			for (int i = 0; i < lastRowNewSpan - HorizontalMerger.GetActualRowSpan(maxRowSpan); i++)
				table.Rows.Add(new SchedulerRow());
		}
		static TableRow CreateTableRowWithEmptyCell() {
			TableRow row = RenderUtils.CreateTableRow();
			TableCell cell = RenderUtils.CreateTableCell();
			row.Cells.Add(cell);
			return row;
		}
		public static Table AddNewTableRow(Table table, WebControl[] controls) {
			table.Rows.Add(CreateTableRow(controls));
			return table;
		}
		public static Table AddNewTableRow(Table table, WebControl[] controls, string[] classNames) {
			table.Rows.Add(CreateTableRow(controls, classNames));
			return table;
		}
		static TableRow CreateTableRowTemplated(WebControl[] controls, Action<TableCell, int> initCellByIndxHandler) {
			int count = controls.Length;
			if (count <= 0)
				return CreateTableRowWithEmptyCell();
			TableRow row = RenderUtils.CreateTableRow();
			for (int i = 0; i < count; i++) {
				TableCell cell = RenderUtils.CreateTableCell();
				if (initCellByIndxHandler != null)
					initCellByIndxHandler(cell, i);
				row.Cells.Add(cell);
				if (controls[i] != null)
					cell.Controls.Add(controls[i]);
			}
			return row;
		}
		public static Table CreateSingleRowTable(WebControl[] controls) {
			Table result = RenderUtils.CreateTable();
			result.Rows.Add(CreateTableRow(controls));
			return result;
		}
		public static Table CreateSingleRowTable(WebControl[] controls, string[] classNames) {
			Table result = RenderUtils.CreateTable();
			result.Rows.Add(CreateTableRow(controls, classNames));
			return result;
		}
		public static TableRow CreateTableRow(WebControl[] controls) {
			return CreateTableRowTemplated(controls, null);
		}
		public static TableRow CreateTableRow(WebControl[] controls, string[] classNames) {
			int classNamesCount = classNames.Length;
			return CreateTableRowTemplated(controls, (cell, indx) => {
				if (indx < classNamesCount)
					cell.CssClass = classNames[indx];
			});
		}
		public static void AddControlToSingleCellTable(Table table, WebControl control) {
			WebControl cell = GetSingleCellTableContentCell(table);
			cell.Controls.Add(control);
		}
		public static TableCell GetSingleCellTableContentCell(Table table) {
			XtraSchedulerDebug.Assert(table.Rows.Count == 1);
			XtraSchedulerDebug.Assert(table.Rows[0].Cells.Count == 1);
			return table.Rows[0].Cells[0];
		}
		public static SchedulerTable MergeVertical(SchedulerTable topTable, SchedulerTable bottomTable) {
			if (topTable == null)
				Exceptions.ThrowArgumentNullException("topTable");
			if (bottomTable == null)
				Exceptions.ThrowArgumentNullException("bottomTable");
			VerticalMerger merger = new VerticalMerger();
			return merger.Merge(topTable, bottomTable);
		}
		public static SchedulerTable MergeHorizontal(SchedulerTable leftTable, SchedulerTable rightTable) {
			if (leftTable == null)
				Exceptions.ThrowArgumentNullException("leftTable");
			if (rightTable == null)
				Exceptions.ThrowArgumentNullException("rightTable");
			HorizontalMerger horizontalMerger = HorizontalMerger.Create(leftTable, rightTable);
			return horizontalMerger.Merge(leftTable, rightTable);
		}
		public static Table CreateTableWithOneCell() {
			Table result = RenderUtils.CreateTable();
			TableRow row = RenderUtils.CreateTableRow();
			result.Rows.Add(row);
			TableCell cell = RenderUtils.CreateTableCell();
			row.Cells.Add(cell);
			return result;
		}
		public static SchedulerTable CreateTableWithOneCell(ISchedulerCell cell) {
			if (cell == null)
				Exceptions.ThrowArgumentNullException("cell");
			SchedulerTable table = new SchedulerTable();
			SchedulerRow row = new SchedulerRow();
			row.Cells.Add(cell);
			table.Rows.Add(row);
			return table;
		}
		public static SchedulerTable CreateTableWithOneCell(ISchedulerCell cell, int rowSpan) {
			rowSpan = Math.Max(1, rowSpan);
			SchedulerTable table = new SchedulerTable();
			for (int i = 0; i < rowSpan; i++) {
				SchedulerRow row = new SchedulerRow();
				table.Rows.Add(row);
			}
			table.Rows[0].Cells.Add(cell);
			return table;
		}
	}
	public class ASPxSelectedAppointmentAdorner : ASPxInternalWebControl {
		WebControl adornerDiv;
		ASPxScheduler control;
		public ASPxSelectedAppointmentAdorner(ASPxScheduler control) {
			this.control = control;
		}
		internal ASPxScheduler Control { get { return control; } }
		WebControl AdornerDiv { get { return adornerDiv; } }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.adornerDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Controls.Add(adornerDiv);
			adornerDiv.ID = "aptAdornerDiv";
			adornerDiv.Controls.Add(new LiteralControl("&nbsp;"));
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.AppendDefaultDXClassName(AdornerDiv, "dxscAptSelDiv");
		}
		internal void RenderCommonScript(StringBuilder sb, string localVarName) {
		}
	}
}
