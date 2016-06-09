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
using System.Windows;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Grid.TreeList;
using DevExpress.XtraPrinting.DataNodes;
using DevExpress.Xpf.Grid;
using DevExpress.Utils;
using DevExpress.Data;
using System.Linq;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Grid.Printing {
	public abstract class PrintingDataTreeBuilder : PrintingDataTreeBuilderBase {
#if DEBUGTEST
		public static Action AfterGenerateNodeTreeAction;
		public static PrintingDataTreeBuilderBase CurrentPrintingTreeBuilder;
		public void ForceSetMasterDetailPrintInfo(MasterDetailPrintInfo masterDetailPrintInfo) {
			this.masterDetailPrintInfo = masterDetailPrintInfo;
		}
#endif
		public ITableView PrintView { get { return base.View as ITableView; } }
		protected BandsLayoutBase BandsLayout { get; private set; }
		protected PrintRowInfoBase basePrintRowInfo { get; set; }
		public override bool SupportsMasterDetail { get { return false; } }
		MasterDetailPrintInfo masterDetailPrintInfo;
		protected internal MasterDetailPrintInfo MasterDetailPrintInfo {
			get {
				if(masterDetailPrintInfo == null)
					masterDetailPrintInfo = GetDefaultMasterDetailPrintInfo();
				return masterDetailPrintInfo; 
			}
			protected set {
				masterDetailPrintInfo = value;
			} 
		}
		protected virtual MasterDetailPrintInfo GetDefaultMasterDetailPrintInfo() {
			return null;
		}
		public PrintingDataTreeBuilder(DataViewBase view, double totalHeaderWidth, BandsLayoutBase bandsLayout, MasterDetailPrintInfo masterDetailPrintInfo = null)
			: base(view, totalHeaderWidth) {
			BandsLayout = bandsLayout;
			MasterDetailPrintInfo = masterDetailPrintInfo;
			basePrintRowInfo = GetHeaderFooterPrintInfo();
			SetHeadersPrintRowInfo(HeadersData, basePrintRowInfo);
			UpdateBandData();
			HeadersData.UpdateCellData();
			UpdateIsTotalSummaryLeftBorderVisible();
		}
		protected abstract double GetHeaderFooterLeftIndent();
		protected abstract void SetHeadersPrintRowInfo(HeadersData headersData, PrintRowInfoBase printRowInfo);
		void UpdateIsTotalSummaryLeftBorderVisible() {
			if(BandsLayout != null)
				return;
			var list = HeadersData.CellDataCache.ToList();
			for(int i = 0; i < list.Count; i++) {
				UpdateIsLeftTotalSummaryBorderVisible(list[i].Key, HeadersData, GridPrintingHelper.GetPrintCellInfo(list[i].Value));
			}
		}
		void UpdateBandData() {
			if(BandsLayout != null)
				SetCloneBandPrintInfo(View.DataControl.BandsLayoutCore.PrintableBands, BandsLayout.VisibleBands);
		}
		void SetCloneBandPrintInfo(IEnumerable<BandBase> sourceBands, List<BandBase> cloneBands) {
			var sourceBandsList = sourceBands.ToList();
			for(int i = 0; i < sourceBandsList.Count; i++)
				SetCloneBandPrintInfo(sourceBandsList[i], cloneBands[i]);
		}
		void SetCloneBandPrintInfo(BandBase source, BandBase clone) {
			PrintCellInfo cellInfo = new PrintCellInfo(
				isLast: false,
				totalSummaryText: null,
				printTotalSummaryStyle: null,
				printColumnWidth: GridPrintingHelper.GetPrintColumnWidth(clone),
				headerCaption: source.HeaderCaption,
				printColumnHeaderStyle: source.ActualPrintBandHeaderStyle,
				printCellStyle: null,
				isColumnHeaderVisible: basePrintRowInfo.IsPrintColumnHeadersVisible,
				detailLevel: 0,
				horizontalHeaderContentAlignment: source.HorizontalHeaderContentAlignment,
				hasTopElement: clone.HasTopElement,
				isRight: !clone.HasRightSibling);
			GridPrintingHelper.SetPrintCellInfo(clone, cellInfo);
			PrintBandInfo bandInfo = new PrintBandInfo(source);
			GridPrintingHelper.SetPrintBandInfo(clone, bandInfo);
			SetCloneBandPrintInfo(source.PrintableBands, clone.VisibleBands);
		}
		protected virtual PrintRowInfoBase GetHeaderFooterPrintInfo() {
			return CreateBasePrintRowInfo();
		}
		protected abstract PrintRowInfoBase CreateBasePrintRowInfo();
		protected virtual int GetActualRowLevel(ColumnsRowDataBase rowData) {
			return rowData.Level;
		}
		internal override void UpdateColumnData(ColumnsRowDataBase rowData, GridColumnData cellData, ColumnBase column) {
			base.UpdateColumnData(rowData, cellData, column);
			PrintCellInfo baseInfo = GetBasePrintCellInfo(cellData, column);
			PrintCellInfo actualPrintCellInfo = null;
			if(IsFirstColumn(cellData.Column)) {
				actualPrintCellInfo = GridPrintingHelper.GetPrintCellInfo(cellData);
				double printColumnWidth = baseInfo.PrintColumnWidth - GridPrintingHelper.GroupIndent * GetActualRowLevel(rowData);
				if(actualPrintCellInfo == null) {
					actualPrintCellInfo = new PrintCellInfo(
					isLast: baseInfo.IsLast,
					totalSummaryText: baseInfo.TotalSummaryText,
					printTotalSummaryStyle: baseInfo.PrintTotalSummaryStyle,
					printColumnWidth: printColumnWidth,
					headerCaption: baseInfo.HeaderCaption,
					printColumnHeaderStyle: baseInfo.PrintColumnHeaderStyle,
					printCellStyle: baseInfo.PrintCellStyle,
					isColumnHeaderVisible: baseInfo.IsColumnHeaderVisible,
					horizontalHeaderContentAlignment: baseInfo.HorizontalHeaderContentAlignment,
					detailLevel: GetDetailLevel(),
					hasTopElement: baseInfo.HasTopElement,
					isRight: baseInfo.IsRight);
				} else {
					actualPrintCellInfo.PrintColumnWidth = printColumnWidth;
				}
			} else {
				actualPrintCellInfo = baseInfo;
			}
			GridPrintingHelper.SetPrintCellInfo(cellData, actualPrintCellInfo);
			if(GridPrintingHelper.GetPrintFixedFooterTextLeft(HeadersData) == null)
				GridPrintingHelper.SetPrintFixedFooterTextLeft(HeadersData, GetFixedLeftTotalSummaryText());
			if(GridPrintingHelper.GetPrintFixedFooterTextRight(HeadersData) == null)
				GridPrintingHelper.SetPrintFixedFooterTextRight(HeadersData, GetFixedRightTotalSummaryText());
		}
		protected virtual void UpdateTotalSummary(ColumnsRowDataBase rowData) {
			foreach(var data in HeadersData.CellData) {
				PrintCellInfo info = GridPrintingHelper.GetPrintCellInfo(data);
				info.TotalSummaryText = GetTotalSummaryText(data.Column);
				UpdateIsLeftTotalSummaryBorderVisible(data.Column, HeadersData, info);
			}
			GridPrintingHelper.SetPrintFixedFooterTextLeft(HeadersData, GetFixedLeftTotalSummaryText());
			GridPrintingHelper.SetPrintFixedFooterTextRight(HeadersData, GetFixedRightTotalSummaryText());
		}
		protected virtual string GetFixedLeftTotalSummaryText() {
			return View.GetFixedSummariesLeftString();
		}
		protected virtual string GetFixedRightTotalSummaryText() {
			return View.GetFixedSummariesRightString();
		}
		bool IsFirstColumn(ColumnBase column) {
			return BandsLayout == null ? column == GetVisibleColumns().First() : !GridPrintingHelper.GetPrintHasLeftSibling(column);
		}
		bool IsLastColumn(ColumnBase column) {
			return column == GetVisibleColumns().Last();
		}
		bool IsRightColumn(ColumnBase column) {
			return BandsLayout == null ? IsLastColumn(column) : !GridPrintingHelper.GetPrintHasRightSibling(column);
		}
		void UpdateIsLeftTotalSummaryBorderVisible(ColumnBase column, ColumnsRowDataBase rowData, PrintCellInfo printCellInfo) {
			if(column.IsFirst || !String.IsNullOrWhiteSpace(GetTotalSummaryText(column))) {
				printCellInfo.IsTotalSummaryLeftBorderVisible = true;
				return;
			}
			int prevVisibleIndex = column.VisibleIndex - 1;
			GridColumnData leftData = rowData.CellDataCache.Values.FirstOrDefault(cd => cd.VisibleIndex == prevVisibleIndex);
			if(leftData == null || leftData.Column == null) {
				printCellInfo.IsTotalSummaryLeftBorderVisible = true;
				return;
			}
			printCellInfo.IsTotalSummaryLeftBorderVisible = !String.IsNullOrWhiteSpace(GetTotalSummaryText(leftData.Column));
		}
		protected virtual string GetTotalSummaryText(ColumnBase column) {
			return column.TotalSummaryText;
		}
		protected PrintCellInfo GetBasePrintCellInfo(ColumnBase column) {
			PrintCellInfo printCellInfo = GridPrintingHelper.GetPrintCellInfo(HeadersData.GetCellDataByColumn(column, false));
			if(printCellInfo != null)
				return printCellInfo;
			return new PrintCellInfo(
				isLast: IsLastColumn(column),
				totalSummaryText: " ",
				printTotalSummaryStyle: column.ActualPrintTotalSummaryStyle,
				printColumnWidth: GridPrintingHelper.GetPrintColumnWidth(column),
				headerCaption: column.HeaderCaption,
				printColumnHeaderStyle: column.ActualPrintColumnHeaderStyle,
				printCellStyle: column.ActualPrintCellStyle,
				isColumnHeaderVisible: basePrintRowInfo != null ? basePrintRowInfo.IsPrintColumnHeadersVisible : false,
				detailLevel: GetDetailLevel(),
				horizontalHeaderContentAlignment: column.HorizontalHeaderContentAlignment,
				hasTopElement: (basePrintRowInfo!= null ? basePrintRowInfo.IsPrintBandHeadersVisible : false) || column.HasTopElement,
				isRight: IsRightColumn(column));
		}
		PrintCellInfo GetBasePrintCellInfo(GridColumnData cellData, ColumnBase column) {
			return GetBasePrintCellInfo(column);
		}
		internal override ColumnBase GetGroupColumnByNode(DataRowNode node) {
			return null;
		}
		internal override object GetGroupValueByNode(DataRowNode node) {
			return null;
		}
		internal override IList<SummaryItemBase> GetGroupSummaries() {
			return null;
		}
		internal override bool TryGetGroupSummaryValue(RowData rowData, SummaryItemBase item, out object value) {
			value = null;
			return false;
		}
		internal override string GetGroupRowDisplayTextByNode(DataRowNode node) {
			return null;
		}
		protected bool IsPrintFooters() {
			return IsPrintFooter() || IsPrintFixedFooter();
		}
		protected bool IsPrintFooter() {
			return IsPrintTotalSummary() && View.ShowTotalSummary && PrintFooterTemplate != null;
		}
		protected bool IsPrintFixedFooter() {
			return IsPrintFixedTotalSummary() && View.ShowFixedTotalSummary && PrintFixedFooterTemplate != null;
		}
		protected abstract bool IsPrintTotalSummary();
		protected abstract bool IsPrintFixedTotalSummary();
		protected virtual int GetDetailLevel() { 
			return 0;
		}
		public abstract IDataNode CreateMasterDetailPrintingNode(NodeContainer container, RowNode rowNode, IDataNode parentNode, int index, Size pageSize);
		protected override IList<ColumnBase> GetPrintableColumns() {
			if(BandsLayout != null)
				return BandsLayout.GetVisibleColumns();
			return base.GetPrintableColumns();
		}
	}
	public class GridPrintingDataTreeBuilder : GridPrintingDataTreeBuilderBase, ISupportMasterDetailPrinting {
		protected new PrintRowInfo basePrintRowInfo { get { return (PrintRowInfo)base.basePrintRowInfo; } }
		internal DataTemplate PrintGroupFooterTemplate { get; private set; }
		public new TableView View { get { return base.View as TableView; } }
		protected new GridDataProvider GridDataProvider { get { return View.GridDataProvider; } }
		readonly bool AllowPartialGrouping;
		public GridPrintingDataTreeBuilder(TableView view, double totalHeaderWidth, ItemsGenerationStrategyBase itemsGenerationStrategy, BandsLayoutBase bandsLayout, MasterDetailPrintInfo masterDetailPrintInfo = null)
			: base(view, totalHeaderWidth, bandsLayout, itemsGenerationStrategy, masterDetailPrintInfo) {
			AllowPartialGrouping = view.AllowPartialGrouping;
			PrintGroupFooterTemplate = View.PrintGroupFooterTemplate;
		}
		protected override PrintRowInfoBase CreateBasePrintRowInfo() {
			PrintRowInfo actualInfo = (PrintRowInfo)base.CreateBasePrintRowInfo();
			actualInfo.DetailLevel = GetDetailLevel();
			actualInfo.BandsLayout = BandsLayout;
			actualInfo.IsPrintTopDetailRowVisible = false;
			actualInfo.IsPrintBottomDetailIndentVisible = false;
			actualInfo.IsPrintDetailTopIndentVisible = false;
			actualInfo.IsPrintDetailBottomIndentVisible = false;
			actualInfo.PrintDataTopIndentBorderThickness = FillControl.EmptyThickness;
			actualInfo.IsPrintBottomLastDetailIndentVisible = false;
			actualInfo.PrintTopRowIndentMargin = FillControl.EmptyThickness;
			actualInfo.PrintTopRowWidth = TotalHeaderWidth;
			actualInfo.IsPrintHeaderBottomIndentVisible = false;
			actualInfo.IsPrintFooterBottomIndentVisible = false;
			actualInfo.IsPrintFixedFooterBottomIndentVisible = false;
			actualInfo.PrintColumnHeaderStyle = View.PrintColumnHeaderStyle;
			actualInfo.IsPrintColumnHeadersVisible = View.PrintColumnHeaders && View.ShowColumnHeaders;
			actualInfo.IsPrintBandHeadersVisible = View.DataControl.BandsLayoutCore != null && View.ShowBandsPanel && View.PrintBandHeaders;
			TableView masterView = View.Grid.GetMasterGrid() != null ? View.Grid.GetMasterGrid().View as TableView : null;
			actualInfo.DetailTopIndent = masterView != null ? masterView.PrintDetailTopIndent : View.PrintDetailTopIndent;
			actualInfo.DetailBottomIndent = masterView != null ? masterView.PrintDetailBottomIndent : View.PrintDetailBottomIndent;
			return actualInfo;
		}
		protected override PrintRowInfoBase CreateBasePrintRowInfoObject() {
			return new PrintRowInfo();
		}
		protected override MasterDetailPrintInfo GetDefaultMasterDetailPrintInfo() {
			return new MasterDetailPrintInfo(View.AllowPrintDetails, View.AllowPrintEmptyDetails, View.PrintAllDetails, this);
		}
		Dictionary<DataControlDetailDescriptor, IDescriptorAndDataControlBase> cache = new Dictionary<DataControlDetailDescriptor, IDescriptorAndDataControlBase>();
		public IDescriptorAndDataControlBase GetDescriptorAndGridControl(DataControlDetailDescriptor descriptor) {
			IDescriptorAndDataControlBase result;
			if(!cache.TryGetValue(descriptor, out result)) {
				result = new DescriptorAndGridControl(descriptor);
				cache.Add(descriptor, result);
			}
			return result;
		}
		public bool IsGeneratedControl(DataControlBase grid) {
			foreach(var dgc in cache.Values) {
				if(dgc.Grid == grid)
					return true;
			}
			return false;
		}
		protected override void SetHeadersPrintRowInfo(HeadersData headersData, PrintRowInfoBase printRowInfo) {
			GridPrintingHelper.SetPrintRowInfo(headersData, (PrintRowInfo)printRowInfo);
		}
		protected override bool IsGeneratedControl(GridControl grid) {
			return MasterDetailPrintInfo.RootPrintingDataTreeBuilder.IsGeneratedControl(grid);
		}
		internal override void UpdateRowData(RowData rowData) {
			double width = TotalHeaderWidth;
			if(rowData is GroupRowData) {
				width -= GridPrintingHelper.GroupIndent * ((GroupRowData)rowData).Level;
			}
			Thickness printDataIndentBorderThickness = default(Thickness);
			Thickness printDataIndentMargin;
			double printDataIndent;
			RowNodePrintInfo nodePrintInfo = rowData.DataRowNode.PrintInfo;
			double bottomThickness = GetBorderThickness();
			double detailLeftIndent = (GetDetailLevel() + MasterDetailPrintInfo.DetailGroupLevel) * GridPrintingHelper.GroupIndent;
			bool isMasterRowExpanded = IsMasterRowExpanded(rowData.RowHandle.Value);
			if(nodePrintInfo.RowPosition == RowPosition.Bottom || nodePrintInfo.RowPosition == RowPosition.Single) {
				bool isNeedEmptyLine = isMasterRowExpanded || (nodePrintInfo.NextRowHandle == DataControlBase.InvalidRowHandle && !IsPrintFooters());
				printDataIndentBorderThickness = new Thickness(0, 0, 0, isNeedEmptyLine ? 0 : bottomThickness);
				int nextLevel = nodePrintInfo.NextNodeLevel;
				printDataIndent = GridPrintingHelper.GroupIndent * (rowData.Level - nextLevel);
				printDataIndentMargin = new Thickness(detailLeftIndent + nextLevel * GridPrintingHelper.GroupIndent, 0, 0, 0);
			}
			else {
				printDataIndentBorderThickness = FillControl.EmptyThickness;
				printDataIndent = GridPrintingHelper.GroupIndent * rowData.Level;
				printDataIndentMargin = new Thickness(detailLeftIndent, 0, 0, 0);
			}
			bool isPrintBottomDetailIndentVisible = false;
			bool isPrintBottomLastDetailIndentVisible = false;
			if(!(IsPrintFooters() && (nodePrintInfo.RowPosition == RowPosition.Bottom || nodePrintInfo.RowPosition == RowPosition.Single))) {
				if(GetAllowPrintDetailsValue()) {
					if(isMasterRowExpanded) {
						isPrintBottomDetailIndentVisible = true;
					}
					else {
						if(nodePrintInfo.NextRowHandle != DataControlBase.InvalidRowHandle && !View.Grid.IsGroupRowHandle(nodePrintInfo.NextRowHandle) && (nodePrintInfo.RowPosition == RowPosition.Bottom || nodePrintInfo.RowPosition == RowPosition.Single))
							isPrintBottomDetailIndentVisible = GetDetailLevel() > 0 && MasterDetailPrintInfo.PrintDetailType == PrintDetailType.Last;
					}
				}
				if(!isPrintBottomDetailIndentVisible && GetDetailLevel() > 0) {
					if(nodePrintInfo.NextRowHandle == DataControlBase.InvalidRowHandle) {
						isPrintBottomLastDetailIndentVisible = !(rowData is GroupSummaryRowData && IsPrintFooters());
					}
				}
			}
			if(!isPrintBottomDetailIndentVisible && !isPrintBottomLastDetailIndentVisible) {
				if(isMasterRowExpanded)
					isPrintBottomDetailIndentVisible = true;
			}
			Thickness printDataTopIndentBorderThickness = FillControl.EmptyThickness;
			Thickness printTopRowIndentMargin = FillControl.EmptyThickness;
			double printTopRowWidth = basePrintRowInfo.PrintTopRowWidth;
			bool isPrintTopDetailRowVisible = false;
			if(GetAllowPrintDetailsValue()) {
				if(IsPrevMaserRowExpanded) {
					isPrintTopDetailRowVisible = true;
					printDataTopIndentBorderThickness = new Thickness(0, 0, 0, bottomThickness);
					double grupIndent = GridPrintingHelper.GroupIndent * rowData.Level;
					printTopRowIndentMargin = new Thickness(detailLeftIndent + grupIndent, 0, 0, 0);
					printTopRowWidth -= grupIndent;
				}
			}
			if(prevRowHandle == DataControlBase.InvalidRowHandle || nodePrintInfo.PrevRowHandle == prevRowHandle) {
				prevRowHandle = rowData.RowHandle.Value;
				IsPrevMaserRowExpanded = isMasterRowExpanded;
			}
			#region PartialGroupingSupport
			bool showRowBreak = false;
			bool showIndentRowBreak = false;
			if(AllowPartialGrouping) {
				var visibleIndexes = View.DataControl.DataProviderBase.DataController.GetVisibleIndexes();
				if(visibleIndexes.IsSingleGroupRow(rowData.RowHandle.Value)) {
					showRowBreak = true;
				}
				else {
					showRowBreak = View.Grid.IsGroupRowHandle(nodePrintInfo.NextRowHandle) || visibleIndexes.IsSingleGroupRow(nodePrintInfo.NextRowHandle);
				}
				if(showRowBreak) {
					if(nodePrintInfo.RowPosition != RowPosition.Bottom && nodePrintInfo.RowPosition != RowPosition.Single && View.Grid.IsGroupRowHandle(nodePrintInfo.NextRowHandle) && nodePrintInfo.NextNodeLevel < rowData.Level && !isMasterRowExpanded) {
						showIndentRowBreak = true;
						printDataIndentBorderThickness = new Thickness(0, 0, 0, bottomThickness);
					}
				}
			}
			#endregion
			PrintRowInfo actualInfo = GridPrintingHelper.GetPrintRowInfo(rowData);
			if(actualInfo == null) {
				actualInfo = new PrintRowInfo();
				GridPrintingHelper.SetPrintRowInfo(rowData, actualInfo);
			}
			#region SetActualInfo
			ClonePrintRowInfoProperties(actualInfo, basePrintRowInfo);
			actualInfo.IsPrintColumnHeadersVisible = basePrintRowInfo.IsPrintColumnHeadersVisible;
			actualInfo.IsPrintBandHeadersVisible = basePrintRowInfo.IsPrintBandHeadersVisible;
			actualInfo.PrintDataIndentBorderThickness = printDataIndentBorderThickness;
			actualInfo.PrintDataIndentMargin = printDataIndentMargin;
			actualInfo.PrintDataIndent = printDataIndent;
			actualInfo.TotalHeaderWidth = width;
			actualInfo.ShowRowBreak = showRowBreak;
			actualInfo.ShowIndentRowBreak = showIndentRowBreak;
			actualInfo.IsPrintBottomDetailIndentVisible = isPrintBottomDetailIndentVisible;
			actualInfo.PrintDataTopIndentBorderThickness = printDataTopIndentBorderThickness;
			actualInfo.IsPrintTopDetailRowVisible = isPrintTopDetailRowVisible;
			actualInfo.IsPrintBottomLastDetailIndentVisible = isPrintBottomLastDetailIndentVisible;
			actualInfo.PrintTopRowIndentMargin = printTopRowIndentMargin;
			actualInfo.PrintTopRowWidth = printTopRowWidth;
			actualInfo.DetailLevel = GetDetailLevel();
			actualInfo.PrintRowDataBottomIndentControlStyle = basePrintRowInfo.PrintRowDataBottomIndentControlStyle;
			actualInfo.PrintRowDataBottomLastIndentControlStyle = basePrintRowInfo.PrintRowDataBottomLastIndentControlStyle;
			actualInfo.DetailTopIndent = basePrintRowInfo.DetailTopIndent;
			actualInfo.DetailBottomIndent = basePrintRowInfo.DetailBottomIndent;
			actualInfo.BandsLayout = basePrintRowInfo.BandsLayout;
			#endregion
		}
		int prevRowHandle = DataControlBase.InvalidRowHandle;
		bool IsPrevMaserRowExpanded = false;
		bool IsMasterRowExpanded(int rowHandle) {
			if(rowHandle == DataControlBase.InvalidRowHandle || rowHandle < 0)
				return false;
			if(!GetAllowPrintDetailsValue())
				return false;
			if(GetPrintAllDetailsValue() && View.Grid.DetailDescriptor != null)
				return IsFakeDetailExpanded(rowHandle);
			if(!MasterDetailPrintHelper.IsMasterRowExpanded(this, rowHandle))
				return false;
			DataControlBase dataControl = MasterDetailPrintHelper.FindDetailDataControl(this, rowHandle, MasterDetailPrintHelper.GetActiveDetailDescriptor(this, rowHandle));
			if(dataControl == null)
				return false;
			return IsMasterRowExpandedCore(dataControl, rowHandle);
		}
		bool IsMasterRowExpandedCore(DataControlBase dataControl, int rowHandle) {
			if(MasterDetailPrintHelper.IsDetailContainsRows(this, rowHandle, dataControl))
				return true;
			if(!GetAllowPrintEmptyDetailsValue())
				return false;
			TableView view = dataControl.viewCore as TableView;
			if(view == null)
				return false;
			return IsGridHeaderFooterVisible(view);
		}
		bool IsFakeDetailExpanded(int rowHandle) {
			List<IDescriptorAndDataControlBase> descriptors = new List<IDescriptorAndDataControlBase>();
			MasterDetailPrintHelper.GetAllDetailDescriptors(View.Grid).ForEach(descr => descriptors.Add(MasterDetailPrintInfo.RootPrintingDataTreeBuilder.GetDescriptorAndGridControl(descr)));
			foreach(var descr in descriptors) {
				bool isDescriptorGenerated = true;
				DataControlBase detailGrid = MasterDetailPrintHelper.FindDetailDataControl(this, ReusingRowData.RowHandle.Value, (DataControlDetailDescriptor)descr.Descriptor);
				if(detailGrid == null || !((TableView)detailGrid.viewCore).PrintSelectedRowsOnly)
					detailGrid = descr.GetDetailGridControl(this, out isDescriptorGenerated);
				if(IsMasterRowExpandedCore(detailGrid, rowHandle))
					return true;
			}
			return false;
		}
		internal bool GetAllowPrintDetailsValue() {
			return MasterDetailPrintInfo.AllowPrintDetails.ToBoolean(GridPrintingHelper.DefaultAllowPrintDetails);
		}
		internal bool GetAllowPrintEmptyDetailsValue() {
			return MasterDetailPrintInfo.AllowPrintEmptyDetails.ToBoolean(GridPrintingHelper.DefaultAllowPrintEmptyDetails);
		}
		internal bool GetPrintAllDetailsValue() {
			return MasterDetailPrintInfo.PrintAllDetails.ToBoolean(GridPrintingHelper.DefaultPrintAllDetails);
		}
		internal bool IsGridHeaderFooterVisible(TableView view) {
			return view.PrintColumnHeaders || IsPrintFooter(view) || IsPrintFixedFooter(view);
		}
		protected override int GetDetailLevel() {
			int level = -1;
			View.DataControl.EnumerateThisAndParentDataControls(dataControl => { level++; });
			return level;
		}
		internal override void UpdateGroupRowData(RowData rowData) {
			GroupRowData groupRowData = (GroupRowData)rowData;
			if(groupRowData is GroupSummaryRowData)
				UpdatePrintGroupSummaryInfo(groupRowData);
			else TreeBuilderPrintingHelper.UpdatePrintGroupRowInfo(GridPrintingHelper.GetPrintRowInfo(rowData), groupRowData, View.GroupSummaryDisplayMode, View.PrintGroupSummaryDisplayMode, GetGroupRowText(groupRowData), GetDetailLevel());
		}
		void UpdatePrintGroupSummaryInfo(GroupRowData groupRowData) {
			foreach(GridGroupSummaryColumnData summary in groupRowData.FixedNoneGroupSummaryData) {
				double printColumnWidth = GridPrintingHelper.GetPrintCellInfo(groupRowData.GetCellDataByColumn(summary.Column)).PrintColumnWidth;
				string groupFooter = summary.Value.ToString();
				Style printGroupFooterStyle = View.PrintGroupFooterStyle;
				bool isRight = !summary.Column.HasRightSibling;
				PrintGroupSummaryInfo info = new PrintGroupSummaryInfo(printColumnWidth, groupFooter, printGroupFooterStyle, isRight);
				GridPrintingHelper.SetPrintGroupSummaryInfo(summary, info);
			}
		}
		protected virtual string GetGroupRowText(GroupRowData rowData) {
			return TreeBuilderPrintingHelper.GetGroupRowText(rowData, View.GroupSummaryDisplayMode, View.PrintGroupSummaryDisplayMode, "({0})", true);
		}
		public override IDataNode CreateDetailPrintingNode(NodeContainer container, RowNode rowNode, IDataNode node, int index) {
			return new GridDetailPrintingNode(container, rowNode, this, node, index);
		}
		public override IDataNode CreateGroupPrintingNode(NodeContainer container, RowNode groupNode, IDataNode node, int index, Size pageSize) {
			return groupNode is GroupSummaryRowNode ? new GridGroupSummaryPrintingNode(container, (GroupNode)groupNode, this, node, index, pageSize) : 
				new GridGroupPrintingNode(container, (GroupNode)groupNode, this, node, index, pageSize);
		}
		public override IDataNode CreateMasterDetailPrintingNode(NodeContainer container, RowNode rowNode, IDataNode node, int index, Size pageSize) {
			return new GridMasterDetailPrintingNode(container, rowNode, this, node, index, pageSize);
		}
		protected override RowNodePrintInfo CreateRowNodePrintInfo(DataRowNode node, Dictionary<ColumnBase, int> mergeValueCounters) {
			RowNodePrintInfo printInfo = base.CreateRowNodePrintInfo(node, mergeValueCounters);
			if(View.ActualAllowCellMerge) {
				printInfo.MergeValues = new Dictionary<ColumnBase, int>();
				foreach(ColumnBase column in GetVisibleColumns()) {
					int? mergeValue = GetMergeValue(column, node.RowHandle.Value, mergeValueCounters);
					if(mergeValue.HasValue)
						printInfo.MergeValues[column] = mergeValue.Value;
				}
			}
			return printInfo;
		}
		int? GetMergeValue(ColumnBase column, int rowHandle, Dictionary<ColumnBase, int> mergeValueCounters) {
			if(!View.IsMergedCell(rowHandle, column)) return null;
			int visibleIndex = View.DataControl.GetRowVisibleIndexByHandleCore(rowHandle);
			if(!View.IsPrevRowCellMerged(visibleIndex, column, false) && mergeValueCounters.ContainsKey(column))
				mergeValueCounters[column]++;
			int counter;
			if(mergeValueCounters.TryGetValue(column, out counter))
				return counter;
			mergeValueCounters[column] = 0;
			return 0;
		}
		protected override bool GetPrintGroupFooters() {
			return View.PrintGroupFooters;
		}
		protected override void ProcessNode(DataRowNode node) { }
	}
}
