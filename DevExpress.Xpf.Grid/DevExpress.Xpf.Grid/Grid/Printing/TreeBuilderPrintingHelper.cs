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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Grid.Printing;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.Utils;
using DevExpress.XtraPrinting.DataNodes;
using System.Collections.Generic;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting;
using System.Collections;
using DevExpress.Xpf.Printing;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm.Native;
using System.Reflection;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Xpf.Core.ConditionalFormatting;
namespace DevExpress.Xpf.Grid {
	public static class TreeBuilderPrintingHelper {
		public static IList<SummaryItemBase> CloneGroupSummaries(GridSummaryItemCollection groupSummary) {
			return CopyList<SummaryItemBase>(new SimpleBridgeList<SummaryItemBase, GridSummaryItem>(groupSummary, item => new GridSummaryItem() { DisplayFormat = item.DisplayFormat, FieldName = item.FieldName, ShowInColumn = item.ShowInColumn, SummaryType = item.SummaryType, Visible = item.Visible, ShowInGroupColumnFooter = item.ShowInGroupColumnFooter }));
		}
		static IList<T> CopyList<T>(IList<T> source) {
			List<T> list = new List<T>(source.Count);
			foreach(T column in source) {
				list.Add(column);
			}
			return list;
		}
		public static void UpdatePrintGroupRowInfo(PrintRowInfoBase rowInfo, GroupRowData groupRowData, GroupSummaryDisplayMode groupSummaryDisplayMode, GroupSummaryDisplayMode printGroupSummaryDisplayMode, string groupRowText, int detailLevel) {
			PrintGroupRowInfo groupRowInfo = new PrintGroupRowInfo();
			groupRowInfo.IsExpanded = groupRowData.IsRowExpanded;
			groupRowInfo.CaptionCell = CreateGroupCaptionCellInfo(groupRowData, rowInfo, groupRowInfo, printGroupSummaryDisplayMode, detailLevel);
			groupRowInfo.GroupCells = CreateGroupCellInfos(groupRowData, rowInfo, groupRowInfo, groupSummaryDisplayMode, printGroupSummaryDisplayMode, detailLevel);
			groupRowInfo.FirstColumnCell = CreateFirstColumnGroupCellInfo(groupRowData, rowInfo, groupRowInfo, groupSummaryDisplayMode, printGroupSummaryDisplayMode, groupRowText, detailLevel);
			groupRowInfo.IsLast = ((GroupNode)groupRowData.node).PrintInfo.IsLast;
			ApplyNearEmptyInfos(groupRowInfo);
			GridPrintingHelper.SetPrintGroupRowInfo(groupRowData, groupRowInfo);
		}
		static PrintGroupRowCellInfo CreateFirstColumnGroupCellInfo(GroupRowData rowData, PrintRowInfoBase rowInfo, PrintGroupRowInfo groupRowInfo, GroupSummaryDisplayMode groupSummaryDisplayMode, GroupSummaryDisplayMode printGroupSummaryDisplayMode, string groupRowText, int detailLevel) {
			PrintGroupRowCellInfo firstCellInfo = CreatePrintGroupRowCellInfo(rowData, rowInfo, groupRowInfo, detailLevel);
			firstCellInfo.VisibleIndex = 1;
			firstCellInfo.Text = printGroupSummaryDisplayMode == GroupSummaryDisplayMode.Default ? groupRowText : GetAlignedGroupSummaryText(rowData, rowData.CellData[0].Column, groupSummaryDisplayMode, printGroupSummaryDisplayMode);
			firstCellInfo.Position = groupRowInfo.GroupCells.Count == 0 ? PrintGroupCellPosition.Separator : PrintGroupCellPosition.None;
			return firstCellInfo;
		}
		static string GetAlignedGroupSummaryText(GroupRowData rowData, ColumnBase column, GroupSummaryDisplayMode groupSummaryDisplayMode, GroupSummaryDisplayMode printGroupSummaryDisplayMode) {
			List<string> summaryList = new List<string>();
			for(int i = 0; i < rowData.GroupSummaryData.Count; i++) {
				if(rowData.GroupSummaryData[i].Column == column)
					summaryList.Add(GetGroupSummaryText(rowData, rowData.GroupSummaryData[i], groupSummaryDisplayMode, printGroupSummaryDisplayMode));
			}
			return String.Join(", ", summaryList);
		}
		public static string GetGroupRowText(GroupRowData rowData, GroupSummaryDisplayMode groupSummaryDisplayMode, GroupSummaryDisplayMode printGroupSummaryDisplayMode, string stringFormat, bool useFirstIndent) {
			GridColumnData columnData = (GridColumnData)rowData.GroupValue;
			string res = "";
			if(rowData.GroupSummaryData.Count > 0 && useFirstIndent)
				res += " ";
			for(int i = 0; i < rowData.GroupSummaryData.Count; i++) {
				res += string.Format(stringFormat, GetGroupSummaryText(rowData, rowData.GroupSummaryData[i], groupSummaryDisplayMode, printGroupSummaryDisplayMode));
				if(!rowData.GroupSummaryData[i].IsLast)
					res += ", ";
			}
			return res;
		}
		static string GetGroupSummaryText(GroupRowData item, GridGroupSummaryData summaryData, GroupSummaryDisplayMode groupSummaryDisplayMode, GroupSummaryDisplayMode printGroupSummaryDisplayMode) {
			if(printGroupSummaryDisplayMode == groupSummaryDisplayMode)
				return summaryData.Text;
			object value = null;
			item.treeBuilder.TryGetGroupSummaryValue(item, summaryData.SummaryItem, out value);
			TableView view = item.View as TableView;
			if(view == null) return null;
			return item.GetGroupSummaryText(summaryData.SummaryItem, summaryData.Column, view, value, true);
		}
		static void ApplyNearEmptyInfos(PrintGroupRowInfo rowInfo) {
			List<PrintGroupRowCellInfo> allInfos = new List<PrintGroupRowCellInfo>(rowInfo.GroupCells);
			allInfos.Add(rowInfo.CaptionCell);
			allInfos.Add(rowInfo.FirstColumnCell);
			foreach(PrintGroupRowCellInfo info in allInfos) {
				int leftVisibleIndex = info.VisibleIndex - 1;
				PrintGroupRowCellInfo leftInfo = allInfos.FirstOrDefault(i => i.VisibleIndex == leftVisibleIndex);
				if(leftInfo == null || String.IsNullOrEmpty(leftInfo.Text))
					info.IsLeftGroupSummaryValueEmpty = true;
				int rightVisibleIndex = info.VisibleIndex + 1;
				PrintGroupRowCellInfo rightInfo = allInfos.FirstOrDefault(i => i.VisibleIndex == rightVisibleIndex);
				if(rightInfo == null || String.IsNullOrEmpty(rightInfo.Text))
					info.IsRightGroupSummaryValueEmpty = true;
			}
		}
		static PrintGroupRowCellInfo CreateGroupCaptionCellInfo(GroupRowData rowData, PrintRowInfoBase rowInfo, PrintGroupRowInfo groupRowInfo, GroupSummaryDisplayMode printGroupSummaryDisplayMode, int detailLevel) {
			PrintGroupRowCellInfo captionCellInfo = CreatePrintGroupRowCellInfo(rowData, rowInfo, groupRowInfo, detailLevel);
			captionCellInfo.Text = GetGroupRowTextStart(rowData, (GridColumnData)rowData.GroupValue);
			captionCellInfo.Width = GetFirstColumnWidth(rowData, rowInfo, printGroupSummaryDisplayMode);
			captionCellInfo.VisibleIndex = 0;
			captionCellInfo.Position = PrintGroupCellPosition.Default;
			return captionCellInfo;
		}
		static PrintGroupRowCellInfo CreatePrintGroupRowCellInfo(GroupRowData rowData, PrintRowInfoBase rowInfo, PrintGroupRowInfo groupRowInfo, int detailLevel) {
			PrintGroupRowCellInfo cellInfo = new PrintGroupRowCellInfo();
			cellInfo.DetailLevel = detailLevel;
			cellInfo.GroupLevel = rowData.GroupLevel;
			GridPrintingHelper.SetPrintGroupRowInfo(cellInfo, groupRowInfo);
			cellInfo.PrintGroupRowStyle = rowInfo.PrintGroupRowStyle;
			return cellInfo;
		}
		static double GetFirstColumnWidth(RowData rowData, PrintRowInfoBase rowInfo, GroupSummaryDisplayMode printGroupSummaryDisplayMode) {
			return printGroupSummaryDisplayMode == GroupSummaryDisplayMode.Default ? rowInfo.TotalHeaderWidth : GridPrintingHelper.GetPrintCellInfo(rowData.CellData[0]).PrintColumnWidth;
		}
		static string GetGroupRowTextStart(GroupRowData rowData, GridColumnData columnData) {
			return string.Format(rowData.View.GetLocalizedString(GridControlStringId.GridGroupRowDisplayTextFormat), ((GroupRowNodePrintInfo)rowData.DataRowNode.PrintInfo).GroupColumnHeaderCaption, columnData.Value);
		}
		static List<PrintGroupRowCellInfo> CreateGroupCellInfos(GroupRowData rowData, PrintRowInfoBase rowInfo, PrintGroupRowInfo groupRowInfo, GroupSummaryDisplayMode groupSummaryDisplayMode, GroupSummaryDisplayMode printGroupSummaryDisplayMode, int detailLevel) {
			if(printGroupSummaryDisplayMode == GroupSummaryDisplayMode.Default)
				return new List<PrintGroupRowCellInfo>();
			List<PrintGroupRowCellInfo> result = new List<PrintGroupRowCellInfo>();
			for(int i = 1; i < rowData.CellData.Count; i++) {
				PrintGroupRowCellInfo groupCellInfo = CreatePrintGroupRowCellInfo(rowData, rowInfo, groupRowInfo, detailLevel);
				groupCellInfo.VisibleIndex = i + 1;
				groupCellInfo.Width = GridPrintingHelper.GetPrintCellInfo(rowData.CellData[i]).PrintColumnWidth;
				groupCellInfo.Position = i == rowData.CellData.Count - 1 ? PrintGroupCellPosition.Last : PrintGroupCellPosition.Default;
				groupCellInfo.Text = GetAlignedGroupSummaryText(rowData, rowData.CellData[i].Column, groupSummaryDisplayMode, printGroupSummaryDisplayMode);
				result.Add(groupCellInfo);
			}
			return result;
		}
	}
}
