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
	public abstract class GridPrintingDataTreeBuilderBase : PrintingDataTreeBuilder {
		readonly IList<SummaryItemBase> groupSummaries;
		internal DataTemplate PrintGroupRowTemplate { get; private set; }
		public new GridViewBase View { get { return base.View as GridViewBase; } }
		protected GridDataProvider GridDataProvider { get { return (GridDataProvider)View.DataProviderBase; } }
		internal GroupRowData reusingGroupRowData;
		Dictionary<SummaryItemBase, object> totalSummary;
		Dictionary<int, int> OriginalRowHandles = null;
		public GridPrintingDataTreeBuilderBase(GridViewBase view, double pageWidth, BandsLayoutBase bandsLayout, ItemsGenerationStrategyBase itemsGenerationStrategy, MasterDetailPrintInfo masterDetailPrintInfo = null)
			: base(view, pageWidth, bandsLayout, masterDetailPrintInfo) {
			this.itemsGenerationStrategy = itemsGenerationStrategy;
			PrintGroupRowTemplate = View.PrintGroupRowTemplate;
			groupSummaries = TreeBuilderPrintingHelper.CloneGroupSummaries(View.Grid.GroupSummary);
			reusingGroupRowData = new GroupRowData(this);
		}
		ItemsGenerationStrategyBase itemsGenerationStrategy;
		internal ItemsGenerationStrategyBase ItemsGenerationStrategy {
			get {
				if(itemsGenerationStrategy == null) {
					itemsGenerationStrategy = CreateItemsGenerationStrategy();
				}
				return itemsGenerationStrategy;
			}
		}
		ItemsGenerationStrategyBase CreateItemsGenerationStrategy() {
			return View.CreateItemsGenerationStrategy();
		}
		public int GetOriginalRowHandle(int rowHandle) {
			if(!View.PrintSelectedRowsOnly || OriginalRowHandles == null) return rowHandle;
			int realRowHandle = default(int);
			if(OriginalRowHandles.TryGetValue(rowHandle, out realRowHandle)) return realRowHandle;
			return rowHandle;
		}
		protected override PrintRowInfoBase GetHeaderFooterPrintInfo() {
			PrintRowInfoBase info = CreateBasePrintRowInfo();
			int detailLevel = GetDetailLevel();
			double leftIndent = GetHeaderFooterLeftIndent();
			info.PrintDataIndentMargin = new Thickness(info.PrintDataIndentMargin.Left + leftIndent, info.PrintDataIndentMargin.Top, info.PrintDataIndentMargin.Right, info.PrintDataIndentMargin.Bottom);
			info.IsPrintHeaderBottomIndentVisible = !IsPrintFooters() && detailLevel > 0 && View.Grid.VisibleRowCount == 0;
			if(detailLevel > 0) {
				info.IsPrintFooterBottomIndentVisible = IsPrintFooter() && !IsPrintFixedFooter();
				info.IsPrintFixedFooterBottomIndentVisible = IsPrintFixedFooter();
			}
			return info;
		}
		protected override double GetHeaderFooterLeftIndent() {
			return (GetDetailLevel() + MasterDetailPrintInfo.DetailGroupLevel) * GridPrintingHelper.GroupIndent;
		}
		protected override PrintRowInfoBase CreateBasePrintRowInfo() {
			PrintRowInfoBase actualInfo = CreateBasePrintRowInfoObject();
			actualInfo.PrintGroupRowStyle = View.PrintGroupRowStyle;
			actualInfo.PrintFixedFooterStyle = View.PrintFixedTotalSummaryStyle;
			actualInfo.PrintRowIndentStyle = View.PrintRowIndentStyle;
			actualInfo.PrintDataIndentBorderThickness = new Thickness(0, 0, 0, GetBorderThickness());
			actualInfo.PrintDataIndentMargin = FillControl.EmptyThickness;
			actualInfo.PrintDataIndent = 0;
			actualInfo.TotalHeaderWidth = TotalHeaderWidth;
			actualInfo.PrintCellStyle = View.PrintCellStyle;
			return actualInfo;
		}
		protected abstract PrintRowInfoBase CreateBasePrintRowInfoObject();
		protected void ClonePrintRowInfoProperties(PrintRowInfoBase actualInfo, PrintRowInfoBase basePrintRowInfo) {
			actualInfo.PrintColumnHeaderStyle = basePrintRowInfo.PrintColumnHeaderStyle;
			actualInfo.PrintGroupRowStyle = basePrintRowInfo.PrintGroupRowStyle;
			actualInfo.PrintFixedFooterStyle = basePrintRowInfo.PrintFixedFooterStyle;
			actualInfo.PrintRowIndentStyle = basePrintRowInfo.PrintRowIndentStyle;
			actualInfo.PrintCellStyle = basePrintRowInfo.PrintCellStyle;
		}
		protected override bool IsPrintTotalSummary() {
			return View.PrintTotalSummary;
		}
		protected override bool IsPrintFixedTotalSummary() {
			return View.PrintFixedTotalSummary;
		}
		protected bool IsPrintFooter(TableView view) {
			return view.ShouldPrintTotalSummary && PrintFooterTemplate != null;
		}
		protected bool IsPrintFixedFooter(TableView view) {
			return view.ShouldPrintFixedTotalSummary && PrintFixedFooterTemplate != null;
		}
		internal override ColumnBase GetGroupColumnByNode(DataRowNode node) {
			return GetGroupRowNodePrintInfo(node).GroupColumn;
		}
		internal override object GetGroupValueByNode(DataRowNode node) {
			return GetGroupRowNodePrintInfo(node).GroupValue;
		}
		internal override string GetGroupRowDisplayTextByNode(DataRowNode node) {
			return GetGroupRowNodePrintInfo(node).GroupDisplayText;
		}
		internal static GroupRowNodePrintInfo GetGroupRowNodePrintInfo(DataRowNode node) {
			return (GroupRowNodePrintInfo)node.PrintInfo;
		}
		internal override IList<SummaryItemBase> GetGroupSummaries() {
			return groupSummaries;
		}
		internal override object GetRowValue(RowData rowData) {
			return ItemsGenerationStrategy.GetRowValue(rowData);
		}
		internal override object GetCellValue(RowData rowData, string fieldName) {
			return ItemsGenerationStrategy.GetCellValue(rowData, fieldName);
		}
		internal override bool TryGetGroupSummaryValue(RowData rowData, SummaryItemBase item, out object value) {
			return GetGroupRowNodePrintInfo(rowData.DataRowNode).GroupSummaryValues.TryGetValue(item, out value);
		}
		protected abstract bool IsGeneratedControl(GridControl grid);
		protected override string GetTotalSummaryText(ColumnBase column) {
			return ItemsGenerationStrategy.GetTotalSummaryText(column);
		}
		protected override string GetFixedLeftTotalSummaryText() {
			return ItemsGenerationStrategy.GetFixedTotalSummaryLeftText();
		}
		protected override string GetFixedRightTotalSummaryText() {
			return ItemsGenerationStrategy.GetFixedTotalSummaryRightText();
		}
		public override void GenerateAllItems() {
			GridControl grid = View.Grid;
			try {
				if(!IsGeneratedControl(grid))
					grid.LockUpdateLayout = true;
				ItemsGenerationStrategy.PrepareDataControllerAndPerformPrintingAction(() => GenerateAllItemsAndCalcPrintInfo());
			}
			finally {
				if(!IsGeneratedControl(grid))
					grid.LockUpdateLayout = false;
			}
		}
		void GenerateAllItemsAndCalcPrintInfo() {
			View.layoutUpdatedLocker.DoLockedAction(() => {
				GridDataProvider.VisibleIndicesProvider.ShowGroupSummaryFooterInternal = GetPrintGroupFooters();
				GridDataProvider.VisibleIndicesProvider.InvalidateCache();
				try {
					RootNodeContainer.ReGenerateItemsCore(0, View.Grid.VisibleRowCount + (GetPrintGroupFooters() ? View.CalcGroupSummaryVisibleRowCount() : 0));
					UpdateTotalSummary(HeadersData);
					CalcNodesPrintInfo();
					StoreTotalSummary();
				}
				finally {
					GridDataProvider.VisibleIndicesProvider.InvalidateCache();
					GridDataProvider.VisibleIndicesProvider.ShowGroupSummaryFooterInternal = true;
				}
			});
		}
		protected abstract bool GetPrintGroupFooters();
		internal override object GetWpfRow(RowData rowData, int listSourceRowIndex) {
			return GridDataProvider.GetWpfRowByListIndex(rowData.DataRowNode.PrintInfo.ListIndex);
		}
		void CalcNodesPrintInfo() {
			OriginalRowHandles = ItemsGenerationStrategy.With(strategy => strategy as ItemsGenerationServerStrategy).With(strategy => strategy.SelectedRowsInfo).With(info => info.OriginalRowHandles);
			VirtualItemsEnumerator en = new VirtualItemsEnumerator(RootNodeContainer);
			List<DataRowNode> prevNodes = new List<DataRowNode>();
			UpdateContainerPrintInfo(RootNodeContainer);
			DataRowNode previousNode = null;
			Dictionary<ColumnBase, int> mergeValueCounters = new Dictionary<ColumnBase, int>();
			while(en.MoveNext()) {
				DataRowNode node = en.Current as DataRowNode;
				ProcessNode(node);
				GroupNode groupNode = node as GroupNode;
				node.PrintInfo = groupNode != null ?
					CreateGroupRowNodePrintInfo(node) :
					CreateRowNodePrintInfo(node, mergeValueCounters);
				node.PrintInfo.RowPosition = ((DataNodeContainer)en.CurrentContainer).GetRowPosition(node);
				node.PrintInfo.ListIndex = GridDataProvider.GetListIndexByRowHandle(node.RowHandle.Value);
				node.PrintInfo.PrevRowHandle = previousNode == null ? DataControlBase.InvalidRowHandle : previousNode.RowHandle.Value;
				node.PrintInfo.PrevRowPosition = previousNode == null ? RowPosition.Middle : previousNode.PrintInfo.RowPosition;
				node.PrintInfo.IsSelected = View.IsRowSelected(node.RowHandle.Value);
				if(groupNode != null) {
					UpdateContainerPrintInfo(groupNode.NodesContainer);
					foreach(DataRowNode prevNode in prevNodes) {
						prevNode.PrintInfo.NextNodeLevel = node.Level;
					}
					prevNodes.Clear();
				}
				prevNodes.Add(node);
				if(previousNode != null)
					previousNode.PrintInfo.NextRowHandle = node.RowHandle.Value;
				previousNode = node;
			}
			if(previousNode != null) {
				previousNode.PrintInfo.NextRowHandle = DataControlBase.InvalidRowHandle;
				previousNode.PrintInfo.IsLast = true;
			}
		}
		protected abstract void ProcessNode(DataRowNode node);
		void UpdateContainerPrintInfo(DataNodeContainer container) {
			container.PrintInfo = new NodeContainerPrintInfo() { IsGroupRowsContainer = container.IsGroupRowsContainer };
		}
		protected virtual RowNodePrintInfo CreateRowNodePrintInfo(DataRowNode node, Dictionary<ColumnBase, int> mergeValueCounters) {
			return new RowNodePrintInfo();
		}
		void StoreTotalSummary() {
			if(View.ViewBehavior.GetServiceSummaries().Any()) {
				totalSummary = View.Grid.DataController.TotalSummary.Cast<SummaryItem>().ToDictionary(x => (SummaryItemBase)x.Key, x => x.SummaryValue);
			}
		}
		internal override object GetServiceTotalSummaryValue(ServiceSummaryItem item) {
			object result = null;
			totalSummary.Do(x => x.TryGetValue(item, out result));
			return result;
		}
		GroupRowNodePrintInfo CreateGroupRowNodePrintInfo(DataRowNode node) {
			GridColumn column = (GridColumn)View.GetColumnBySortLevel(node.Level);
			Dictionary<SummaryItemBase, object> summaryValues = new Dictionary<SummaryItemBase, object>();
			for(int i = 0; i < View.Grid.GroupSummary.Count; i++) {
				object value = null;
				if(GridDataProvider.TryGetGroupSummaryValue(node.RowHandle.Value, View.Grid.GroupSummary[i], out value))
					summaryValues[groupSummaries[i]] = value;
			}
			return new GroupRowNodePrintInfo() {
				GroupColumn = column,
				GroupColumnHeaderCaption = column.HeaderCaption,
				GroupValue = View.GetGroupDisplayValue(node.RowHandle.Value),
				GroupDisplayText = View.GetGroupRowDisplayText(node.RowHandle.Value),
				GroupSummaryValues = summaryValues
			};
		}
		internal override void OnRootNodeDispose() {
			base.OnRootNodeDispose();
			ItemsGenerationStrategy.Clear();
		}
	}
}
