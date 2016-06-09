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
using System.Text;
using DevExpress.Xpf.Grid.Printing;
using System.Windows;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Data;
using DevExpress.XtraPrinting.DataNodes;
using DevExpress.Xpf.Utils;
using System.Windows.Media;
using DevExpress.Xpf.Printing;
using System.Windows.Controls;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid.TreeList {
	public enum PrintRowState { Default, Expanded, Collapsed }
	public class TreeListPrintRowInfo : PrintRowInfo {
		protected readonly object ExpandedButtonKey = new object(),
								  CollapsedButtonKey = new object();
		PrintRowState rowState;
		public PrintRowState RowState {
			get {
				return rowState;
			}
			internal set {
				PrintButtonTargetType = GetPrintButtonTargetType(value); ;
				PrintButtonKey = GetPrintButtonKey(value);
				if(rowState == value) return;
				rowState = value;
				OnPropertyChanged("RowState");
			}
		}
		protected override bool GetIsPrintTopRowVisible(bool isVisible) { return false; }
		TargetType GetPrintButtonTargetType(PrintRowState newRowState) {
			return newRowState == PrintRowState.Default ? TargetType.Text : TargetType.Image;
		}
		object GetPrintButtonKey(PrintRowState newRowState) {
			if(newRowState == PrintRowState.Default) return null;
			return newRowState == PrintRowState.Expanded ? ExpandedButtonKey : CollapsedButtonKey; 
		}
		ImageSource image;
		public ImageSource Image {
			get {
				return image;
			}
			internal set {
				if(image == value) return;
				image = value;
				OnPropertyChanged("Image");
			}
		}
		Thickness printImageIndentBorderThickness;
		public Thickness PrintImageIndentBorderThickness {
			get {
				return printImageIndentBorderThickness;
			}
			internal set {
				if(printImageIndentBorderThickness == value) return;
				printImageIndentBorderThickness = value;
				OnPropertyChanged("PrintImageIndentBorderThickness");
			}
		}
		double printImageIndent;
		public double PrintImageIndent {
			get {
				return printImageIndent;
			}
			internal set {
				if(printImageIndent == value) return;
				printImageIndent = value;
				OnPropertyChanged("PrintImageIndent");
			}
		}
		Thickness printButtonIndentBorderThickness;
		public Thickness PrintButtonIndentBorderThickness {
			get {
				return printButtonIndentBorderThickness;
			}
			internal set {
				if(printButtonIndentBorderThickness == value) return;
				printButtonIndentBorderThickness = value;
				OnPropertyChanged("PrintButtonIndentBorderThickness");
			}
		}
		double printButtonIndent;
		public double PrintButtonIndent {
			get {
				return printButtonIndent;
			}
			internal set {
				if(printButtonIndent == value) return;
				printButtonIndent = value;
				OnPropertyChanged("PrintButtonIndent");
			}
		}
		TargetType printButtonTargetType;
		public TargetType PrintButtonTargetType {
			get {
				return printButtonTargetType;
			}
			private set {
				if(printButtonTargetType == value) return;
				printButtonTargetType = value;
				OnPropertyChanged("PrintButtonTargetType");
			}
		}
		object printButtonKey;
		public object PrintButtonKey {
			get {
				return printButtonKey;
			}
			private set {
				if(printButtonKey == value) return;
				printButtonKey = value;
				OnPropertyChanged("PrintButtonKey");
			}
		}
	}
	public class TreeListPrintingDataTreeBuilder : PrintingDataTreeBuilder {
		protected new PrintRowInfo basePrintRowInfo { get { return (PrintRowInfo)base.basePrintRowInfo; } }
		public new TreeListView View { get { return base.View as TreeListView; } }
		Dictionary<SummaryItemBase, object> printServiceSummaryData;
		protected Dictionary<ColumnBase, string> TotalSummaries = new Dictionary<ColumnBase, string>();
		protected string FixedLeftSummary = String.Empty;
		protected string FixedRightSummary = String.Empty;
		public TreeListPrintingDataTreeBuilder(TreeListView view, double totalHeaderWidth, BandsLayoutBase bandsLayout)
			: base(view, totalHeaderWidth, bandsLayout) {
			RootNodeContainer.EnumerateOnlySelectedRows = view.PrintSelectedRowsOnly;
			UpdatePrintingTotalSummaries();
		}
		void UpdatePrintingTotalSummaries() {
			if(View.PrintSelectedRowsOnly)
				UpdatePrintingTotalSummaries_PrintSelectedRows();
			else
				UpdatePrintingTotalSummaries_AllRows();
		}
		void UpdatePrintingTotalSummaries_AllRows() {
			TotalSummaries = View.ColumnsCore.Cast<ColumnBase>().ToDictionary(c => c, c => c.TotalSummaryText);
			FixedLeftSummary = View.GetFixedSummariesLeftString();
			FixedRightSummary = View.GetFixedSummariesRightString();
		}
		void UpdatePrintingTotalSummaries_PrintSelectedRows() {
			Dictionary<TreeListNode, TreeListDataProvider.SummaryItem> summaryData = new Dictionary<TreeListNode, TreeListDataProvider.SummaryItem>();
			View.TreeListDataProvider.CalcSummaryCore(View.TreeListDataProvider.TotalSummaryCore, summaryData, View.PrintSelectedRowsOnly);
			TotalSummaries = View.DataControl.ColumnsCore.Cast<ColumnBase>().ToDictionary(c => c, c => GetTotalSummaryValue(c, summaryData));
			IList<GridTotalSummaryData> fixedSummariesLeft = new List<GridTotalSummaryData>();
			IList<GridTotalSummaryData> fixedSummariesRight = new List<GridTotalSummaryData>();
			FixedTotalSummaryHelper.GenerateTotalSummaries(View.FixedSummariesHelper.FixedSummariesLeftCore, View.ColumnsCore, (summaryItem) => View.TreeListDataProvider.GetSummaryValueCore(View.TreeListDataProvider.RootNode, summaryItem, summaryData), fixedSummariesLeft);
			FixedTotalSummaryHelper.GenerateTotalSummaries(View.FixedSummariesHelper.FixedSummariesRightCore, View.ColumnsCore, (summaryItem) => View.TreeListDataProvider.GetSummaryValueCore(View.TreeListDataProvider.RootNode, summaryItem, summaryData), fixedSummariesRight);
			FixedLeftSummary = FixedTotalSummaryHelper.GetFixedSummariesString(fixedSummariesLeft);
			FixedRightSummary = FixedTotalSummaryHelper.GetFixedSummariesString(fixedSummariesRight);
		}
		string GetTotalSummaryValue(ColumnBase column, Dictionary<TreeListNode, TreeListDataProvider.SummaryItem> summaryData) {
			return column.GetTotalSummaryText((summaryItem) => View.TreeListDataProvider.GetSummaryValueCore(View.TreeListDataProvider.RootNode, summaryItem, summaryData));
		}
		protected override string GetTotalSummaryText(ColumnBase column) {
			if(!TotalSummaries.ContainsKey(column)) return String.Empty;
			return TotalSummaries[column] ?? String.Empty;
		}
		protected override string GetFixedLeftTotalSummaryText() {
			return FixedLeftSummary;
		}
		protected override string GetFixedRightTotalSummaryText() {
			return FixedRightSummary;
		}
		public override int VisibleCount { get { return PrintAllNodes ? View.TotalNodesCount : base.VisibleCount; } }
		protected bool PrintAllNodes { get { return View.PrintAllNodes; } }
		protected bool PrintNodeImages { get { return View.PrintNodeImages; } }
		protected override PrintRowInfoBase CreateBasePrintRowInfo() {
			TreeListPrintRowInfo actualInfo = new TreeListPrintRowInfo();
			actualInfo.PrintColumnHeaderStyle = View.PrintColumnHeaderStyle;
			actualInfo.PrintFixedFooterStyle = View.PrintFixedTotalSummaryStyle;
			actualInfo.PrintRowIndentStyle = View.PrintRowIndentStyle;
			actualInfo.PrintDataIndentBorderThickness = new Thickness(0, 0, 0, GetBorderThickness());
			actualInfo.PrintDataIndentMargin = FillControl.EmptyThickness;
			actualInfo.PrintDataIndent = 0;
			actualInfo.TotalHeaderWidth = TotalHeaderWidth;
			actualInfo.IsPrintColumnHeadersVisible = View.PrintColumnHeaders && View.ShowColumnHeaders;
			actualInfo.IsPrintBandHeadersVisible = View.DataControl.BandsLayoutCore != null && View.ShowBandsPanel && View.PrintBandHeaders;
			actualInfo.RowState = PrintRowState.Default;
			actualInfo.Image = null;
			actualInfo.PrintImageIndentBorderThickness = new Thickness(0, 0, 0, GetBorderThickness());
			actualInfo.PrintImageIndent = 0;
			actualInfo.PrintButtonIndentBorderThickness = new Thickness(0, 0, 0, GetBorderThickness());
			actualInfo.PrintButtonIndent = 0;
			actualInfo.BandsLayout = BandsLayout;
			return actualInfo;
		}
		protected override double GetHeaderFooterLeftIndent() {
			return 0;
		}
		protected override void SetHeadersPrintRowInfo(HeadersData headersData, PrintRowInfoBase printRowInfo) {
			GridPrintingHelper.SetPrintRowInfo(headersData, (PrintRowInfo)printRowInfo);
		}
		protected override RowData CreateReusingRowData() {
			return new TreeListRowData(this);
		}
		protected override int GetActualRowLevel(ColumnsRowDataBase rowData) {
			if(rowData is HeadersData)
				return base.GetActualRowLevel(rowData);
			return rowData.Level + 1 + (PrintNodeImages ? 1 : 0);
		}
		int GetActualNextNodeLevel(RowNodePrintInfo info) {
			return info.NextNodeLevel + 1 + (PrintNodeImages ? 1 : 0);
		}
		internal override void UpdateRowData(RowData rowData) {
			Thickness buttonIndentBorderThickness, imageIndentBorderThickness, indentMargin;
			double buttonIndent, imageIndent, width = TotalHeaderWidth - GridPrintingHelper.GroupIndent * GetActualRowLevel(rowData);
			RowNodePrintInfo printInfo = rowData.DataRowNode.PrintInfo;
			int actualRowLevel = GetActualRowLevel(rowData);
			int actualNextRowLevel = printInfo.RowPosition == RowPosition.Bottom ? 0 : GetActualNextNodeLevel(printInfo);
			if(rowData.Level == 0 && (printInfo.RowPosition == RowPosition.Bottom || printInfo.RowPosition == RowPosition.Single) && IsPrintFooters()) {
				buttonIndentBorderThickness = new Thickness(0, 0, 0, GetBorderThickness());
				buttonIndent = GridPrintingHelper.GroupIndent;
				indentMargin = FillControl.EmptyThickness;
				imageIndent = PrintNodeImages ? GridPrintingHelper.GroupIndent : 0;
				imageIndentBorderThickness = new Thickness(0, 0, 0, GetBorderThickness());
			}
			else if(rowData.Level > printInfo.NextNodeLevel) {
				double delta = actualRowLevel - actualNextRowLevel;
				if(delta > 1) {
					buttonIndentBorderThickness = new Thickness(0, 0, 0, GetBorderThickness());
					buttonIndent = GridPrintingHelper.GroupIndent * (delta - (PrintNodeImages ? 1 : 0));
					indentMargin = new Thickness((actualNextRowLevel) * GridPrintingHelper.GroupIndent, 0, 0, 0);
				}
				else {
					buttonIndentBorderThickness = PrintNodeImages ? FillControl.EmptyThickness : new Thickness(0, 0, 0, GetBorderThickness());
					buttonIndent = GridPrintingHelper.GroupIndent * delta;
					indentMargin = new Thickness((actualNextRowLevel - (PrintNodeImages ? 1 : 0)) * GridPrintingHelper.GroupIndent, 0, 0, 0);
				}
				imageIndent = PrintNodeImages ? GridPrintingHelper.GroupIndent : 0;
				imageIndentBorderThickness = new Thickness(0, 0, 0, GetBorderThickness()); 
			}
			else {
				imageIndent = PrintNodeImages ? GridPrintingHelper.GroupIndent : 0;
				imageIndentBorderThickness = FillControl.EmptyThickness;
				buttonIndentBorderThickness = FillControl.EmptyThickness;
				buttonIndent = GridPrintingHelper.GroupIndent;
				indentMargin = new Thickness(GridPrintingHelper.GroupIndent * rowData.Level, 0, 0, 0);
			}
			TreeListPrintRowInfo actualInfo = GridPrintingHelper.GetPrintRowInfo(rowData) as TreeListPrintRowInfo;
			if(actualInfo == null) {
				actualInfo = new TreeListPrintRowInfo();
				GridPrintingHelper.SetPrintRowInfo(rowData, actualInfo);
			}
			actualInfo.PrintColumnHeaderStyle = basePrintRowInfo.PrintColumnHeaderStyle;
			actualInfo.PrintFixedFooterStyle = basePrintRowInfo.PrintFixedFooterStyle;
			actualInfo.PrintRowIndentStyle = basePrintRowInfo.PrintRowIndentStyle;
			actualInfo.PrintDataIndentBorderThickness = new Thickness(0, 0, 0, GetBorderThickness());
			actualInfo.PrintDataIndentMargin = indentMargin;
			actualInfo.TotalHeaderWidth = width;
			actualInfo.IsPrintColumnHeadersVisible = basePrintRowInfo.IsPrintColumnHeadersVisible;
			actualInfo.IsPrintBandHeadersVisible = basePrintRowInfo.IsPrintBandHeadersVisible;
			actualInfo.RowState = GetRowState(rowData);
			actualInfo.Image = GetRowImage(rowData);
			actualInfo.PrintImageIndentBorderThickness = imageIndentBorderThickness;
			actualInfo.PrintImageIndent = imageIndent;
			actualInfo.PrintButtonIndentBorderThickness = buttonIndentBorderThickness;
			actualInfo.PrintButtonIndent = buttonIndent;
			actualInfo.BandsLayout = basePrintRowInfo.BandsLayout;
		}
		protected virtual PrintRowState GetRowState(RowData rowData) {
			if(!View.PrintExpandButtons) return PrintRowState.Default;
			TreeListNode node = View.GetNodeByRowHandle(rowData.RowHandle.Value);
			if(node == null || !node.HasVisibleChildren) return PrintRowState.Default;
			if(PrintAllNodes) return PrintRowState.Expanded;
			return node.IsExpanded ? PrintRowState.Expanded : PrintRowState.Collapsed;
		}
		protected virtual ImageSource GetRowImage(RowData rowData) {
			if(!PrintNodeImages) return null;
			return ((TreeListRowData)rowData).GetImageSource();
		} 
		public override void GenerateAllItems() {
			RootNodeContainer.ReGenerateItemsCore(0, VisibleCount);
			RootNodeContainer.PrintInfo = new NodeContainerPrintInfo() { IsGroupRowsContainer = false };
			VirtualItemsEnumerator en = new VirtualItemsEnumerator(RootNodeContainer);
			DataRowNode prevNode = null;
			while(en.MoveNext()) {
				DataRowNode node = en.Current as DataRowNode;
				node.PrintInfo = new RowNodePrintInfo();
				node.PrintInfo.RowPosition = ((DataNodeContainer)en.CurrentContainer).GetRowPosition(node);
				if(prevNode != null)
					prevNode.PrintInfo.NextNodeLevel = node.Level;
				prevNode = node;
			}
			if(View.PrintSelectedRowsOnly && prevNode != null)
				prevNode.PrintInfo.RowPosition = RowPosition.Bottom;
			GridPrintingHelper.SetPrintFixedFooterTextLeft(HeadersData, View.GetFixedSummariesLeftString());
			GridPrintingHelper.SetPrintFixedFooterTextRight(HeadersData, View.GetFixedSummariesRightString());
			CreatePrintSerivceSummaryData();
			UpdateTotalSummary(HeadersData);
		}
		protected virtual void CreatePrintSerivceSummaryData() {
			if(!View.ViewBehavior.GetServiceSummaries().Any()) return;
			TreeListDataProvider.SummaryItem item = View.TreeListDataProvider.GetRootSummaryItem();
			if(item == null) return;
			printServiceSummaryData = item.Where(x => x.Key is ServiceSummaryItem).ToDictionary(x => x.Key, x => x.Value.Value);
		} 
		public override IDataNode CreateDetailPrintingNode(NodeContainer container, RowNode rowNode, XtraPrinting.DataNodes.IDataNode node, int index) {
			return new GridDetailPrintingNode(container, rowNode, this, node, index);
		}
		public override IDataNode CreateGroupPrintingNode(NodeContainer container, RowNode groupNode, XtraPrinting.DataNodes.IDataNode node, int index, Size pageSize) {
			return null;
		}
		public override IDataNode CreateMasterDetailPrintingNode(NodeContainer container, RowNode rowNode, IDataNode parentNode, int index, Size pageSize) {
			throw new NotImplementedException();
		}
		internal override object GetWpfRow(RowData rowData, int listSourceRowIndex) {
			return View.TreeListDataProvider.GetWpfRow(rowData.RowHandle);
		}
		internal override object GetRowValue(RowData rowData) {
			return View.TreeListDataProvider.GetRowValue(rowData.RowHandle.Value);
		}
		internal override object GetCellValue(RowData rowData, string fieldName) {
			return View.TreeListDataProvider.GetRowValue(rowData.RowHandle.Value, fieldName);
		}
		protected internal override int GetRowLevelByControllerRow(int rowHandle) {
			return View.TreeListDataProvider.GetRowLevelByControllerRowCore(rowHandle, !PrintAllNodes);
		}
		protected internal override int GetRowLevelByVisibleIndex(int visibleIndex) {
			if(PrintAllNodes && (visibleIndex < VisibleCount))
				return GetRowLevelByControllerRow(visibleIndex);
			return base.GetRowLevelByVisibleIndex(visibleIndex);
		}
		protected internal override int GetRowHandleByVisibleIndexCore(int visibleIndex) {
			if(PrintAllNodes)
				return visibleIndex;
			return base.GetRowHandleByVisibleIndexCore(visibleIndex);
		}
		protected internal override int GetRowVisibleIndexByHandleCore(int rowHandle) {
			if(PrintAllNodes)
				return rowHandle;
			return base.GetRowVisibleIndexByHandleCore(rowHandle);
		}
		protected override bool IsPrintTotalSummary() {
			return View.PrintTotalSummary;
		}
		protected override bool IsPrintFixedTotalSummary() {
			return View.PrintFixedTotalSummary;
		}
		internal override object GetServiceTotalSummaryValue(ServiceSummaryItem item) {
			if(printServiceSummaryData == null) return null;
			object value = null;
			printServiceSummaryData.TryGetValue(item, out value);
			return value;
		}
	}
}
