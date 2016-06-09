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
using System.Text;
namespace DevExpress.Xpf.Grid.Printing {
	public class CardViewPrintingDataTreeBuilder : GridPrintingDataTreeBuilderBase {
		public new CardView View { get { return base.View as CardView; } }
		protected new CardViewPrintRowInfo basePrintRowInfo { get { return (CardViewPrintRowInfo)base.basePrintRowInfo; } }
		bool PrintAutoCardWidth = false;
		int PrintMaximumCardColumns = 0;
		Thickness PrintCardMargin;
		public CardViewPrintingDataTreeBuilder(CardView view, double pageWidth, ItemsGenerationStrategyBase itemsGenerationStrategy) 
			: base(view, pageWidth, null, itemsGenerationStrategy)  {
			PrintAutoCardWidth = view.PrintAutoCardWidth;
			PrintMaximumCardColumns = view.PrintMaximumCardColumns;
			PrintCardMargin = view.PrintCardMargin;
		}
		internal override void UpdateRowData(RowData rowData) {
			double width = TotalHeaderWidth;
			bool isPrevGroupRowCollapsed = false;
			Thickness printDataIndentBorderThickness = default(Thickness);
			bool isGroupBottomBorderVisible = true;
			if(rowData is GroupRowData) {
				width -= GridPrintingHelper.GroupIndent * ((GroupRowData)rowData).Level;
				RowNodePrintInfo nodePrintInfo = rowData.DataRowNode.PrintInfo;
				if(View.DataControl.IsGroupRowHandleCore(nodePrintInfo.PrevRowHandle))
					isPrevGroupRowCollapsed = true;
				if(View.DataControl.IsGroupRowHandleCore(nodePrintInfo.NextRowHandle)) {
					if(nodePrintInfo.NextNodeLevel <= rowData.Level)
						printDataIndentBorderThickness = new Thickness(0, 0, 0, 0);
				}
				else if(nodePrintInfo.NextRowHandle == DataControlBase.InvalidRowHandle && IsPrintFooters()) {
					printDataIndentBorderThickness = new Thickness(0, 0, 0, 0);
					isGroupBottomBorderVisible = false;
				}
			}
			double printDataIndent = GridPrintingHelper.GroupIndent * (rowData is GroupRowData ? rowData.Level : Math.Max(0, rowData.Level - 1));
			CardViewPrintRowInfo actualInfo = CardViewPrintingHelper.GetPrintCardInfo(rowData);
			if(actualInfo == null) {
				actualInfo = new CardViewPrintRowInfo();
				CardViewPrintingHelper.SetPrintCardInfo(rowData, actualInfo);
			}
			#region SetActualInfo
			ClonePrintRowInfoProperties(actualInfo, basePrintRowInfo);
			actualInfo.TotalHeaderWidth = width;
			actualInfo.IsPrevGroupRowCollapsed = isPrevGroupRowCollapsed;
			actualInfo.PrintCardsRowWidth = TotalHeaderWidth;
			actualInfo.PrintCardTemplate = basePrintRowInfo.PrintCardTemplate;
			actualInfo.PrintCardRowIndentTemplate = basePrintRowInfo.PrintCardRowIndentTemplate;
			actualInfo.PrintCardContentTemplate = basePrintRowInfo.PrintCardContentTemplate;
			actualInfo.PrintCardHeaderTemplate = basePrintRowInfo.PrintCardHeaderTemplate;
			actualInfo.PrintCardMargin = basePrintRowInfo.PrintCardMargin;
			actualInfo.PrintAutoCardWidth = basePrintRowInfo.PrintAutoCardWidth;
			actualInfo.PrintMaximumCardColumns = basePrintRowInfo.PrintMaximumCardColumns;
			actualInfo.PrintDataIndent = printDataIndent;
			actualInfo.PrintCardWidth = GetPrintCardWidth(printDataIndent);
			actualInfo.PrintDataIndentBorderThickness = printDataIndentBorderThickness;
			actualInfo.PrintTotalSummarySeparatorStyle = basePrintRowInfo.PrintTotalSummarySeparatorStyle;
			actualInfo.IsGroupBottomBorderVisible = isGroupBottomBorderVisible;
			#endregion
		}
		protected override PrintRowInfoBase CreateBasePrintRowInfoObject() {
			return new CardViewPrintRowInfo();
		}
		protected override PrintRowInfoBase CreateBasePrintRowInfo() {
			CardViewPrintRowInfo actualInfo = (CardViewPrintRowInfo)base.CreateBasePrintRowInfo();
			actualInfo.PrintCardsRowWidth = TotalHeaderWidth;
			actualInfo.PrintCardTemplate = View.PrintCardTemplate;
			actualInfo.PrintCardRowIndentTemplate = View.PrintCardRowIndentTemplate;
			actualInfo.PrintCardContentTemplate = View.PrintCardContentTemplate;
			actualInfo.PrintCardHeaderTemplate = View.PrintCardHeaderTemplate;
			actualInfo.FixedTotalSummaryTopBorderVisible = !IsPrintFooter();
			actualInfo.PrintCardMargin = View.PrintCardMargin;
			actualInfo.PrintAutoCardWidth = View.PrintAutoCardWidth;
			actualInfo.PrintMaximumCardColumns = View.PrintMaximumCardColumns;
			actualInfo.PrintTotalSummarySeparatorStyle = View.PrintTotalSummarySeparatorStyle;
			actualInfo.TotalSummaries = GetTotalSummaries(actualInfo);
			return actualInfo;
		}
		double GetPrintCardWidth(double dataIndentWidth) {
			if(!PrintAutoCardWidth)
				return Double.NaN;
			double freePageWidth = TotalHeaderWidth - dataIndentWidth;
			for(int i = 0; i < PrintMaximumCardColumns; i++) {
				Thickness cardMargin = CardViewPrintingHelper.GetActualPrintCardMargin(PrintCardMargin, i == 0);
				freePageWidth -= cardMargin.Left + cardMargin.Right;
			}
			return Math.Ceiling(freePageWidth / PrintMaximumCardColumns) - 1d;
		}
		private List<PrintTotalSummaryItem> GetTotalSummaries(CardViewPrintRowInfo actualInfo) {
			List<PrintTotalSummaryItem> result = new List<PrintTotalSummaryItem>();
			List<Tuple<Style, string>> summaryValues = new List<Tuple<Style, string>>();
			foreach(GridColumn column in View.VisibleColumns) {
				string[] summaries = GetTotalSummaryText(column).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				foreach(string summaryValue in summaries) {
					if(String.IsNullOrWhiteSpace(summaryValue))
						continue;
					summaryValues.Add(new Tuple<Style, string>(column.ActualPrintTotalSummaryStyle, summaryValue));
				}
			}
			for(int i = 0; i < summaryValues.Count; i++) {
				if(result.Count > 0)
					result.Add(new PrintTotalSummaryItem() { TotalSummaryText = "  /", PrintTotalSummaryStyle = actualInfo.PrintTotalSummarySeparatorStyle });
				result.Add(new PrintTotalSummaryItem() { TotalSummaryText = (result.Count > 0 ? "  " : "") + summaryValues[i].Item2, PrintTotalSummaryStyle = summaryValues[i].Item1 });
			}
			return result;
		}
		protected override void UpdateTotalSummary(ColumnsRowDataBase rowData) {
			CardViewPrintRowInfo rowInfo = CardViewPrintingHelper.GetPrintCardInfo(rowData);
			rowInfo.TotalSummaries = GetTotalSummaries(rowInfo);
		}
		public List<DataRowNode> AllNodes;
		protected override bool GetPrintGroupFooters() {
			return false;
		}
		protected override void ProcessNode(DataRowNode node) {
			if(AllNodes == null)
				AllNodes = new List<DataRowNode>();
			AllNodes.Add(node);
		}
		protected override double GetHeaderFooterLeftIndent() {
			return 0;
		}
		protected override void SetHeadersPrintRowInfo(HeadersData headersData, PrintRowInfoBase printRowInfo) {
			CardViewPrintingHelper.SetPrintCardInfo(headersData, (CardViewPrintRowInfo)printRowInfo);
		}
		public override IDataNode CreateMasterDetailPrintingNode(NodeContainer container, RowNode rowNode, IDataNode parentNode, int index, Size pageSize) {
			throw new NotImplementedException();
		}
		public override IDataNode CreateDetailPrintingNode(NodeContainer container, RowNode rowNode, IDataNode parentNode, int index) {
			return new GridCardViewPrintingNode(container, rowNode, this, parentNode, index);
		}
		public override IDataNode CreateGroupPrintingNode(NodeContainer container, RowNode groupNode, IDataNode parentNode, int index, Size pageSize) {
			return new CardViewGroupPrintingNode(container, (GroupNode)groupNode, this, parentNode, index, pageSize);
		}
		protected override bool IsGeneratedControl(GridControl grid) {
			return false;
		}
		internal override void UpdateGroupRowData(RowData rowData) {
			GroupRowData groupRowData = (GroupRowData)rowData;
			if(groupRowData is GroupSummaryRowData)
				return;
			TreeBuilderPrintingHelper.UpdatePrintGroupRowInfo(CardViewPrintingHelper.GetPrintCardInfo(rowData), groupRowData, GroupSummaryDisplayMode.Default, GroupSummaryDisplayMode.Default, GetGroupRowText(groupRowData), 0);
		}
		protected virtual string GetGroupRowText(GroupRowData rowData) {
			return TreeBuilderPrintingHelper.GetGroupRowText(rowData, GroupSummaryDisplayMode.Default, GroupSummaryDisplayMode.Default, "{0}", false);
		}
	}
}
