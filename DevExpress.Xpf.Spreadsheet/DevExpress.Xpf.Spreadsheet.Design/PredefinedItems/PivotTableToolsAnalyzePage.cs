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

extern alias Platform;
using System;
using Platform::DevExpress.Xpf.Core.Design;
using Platform::DevExpress.Xpf.Ribbon.Design;
namespace DevExpress.Xpf.Spreadsheet.Design {
	public static partial class BarInfos {
		public static BarInfo PivotTableToolsAnalyzePivotTable { get { return pivotTableToolsAnalyzePivotTable; } }
		static readonly BarInfo pivotTableToolsAnalyzePivotTable = new BarInfo(
			"PivotTable Tools",
			"Analyze",
			"PivotTable",
			new BarInfoItems(
				new string[] { "OptionsPivotTable" },
				new BarItemInfo[] { BarItemInfos.Button }
			),
			String.Empty,
			"Caption_PageCategoryPivotTableTools",
			"Caption_PivotTableAnalyze",
			"Caption_PivotTableAnalyzePivotTable",
			"ToolsPivotTableCommandGroup"
		);
		public static BarInfo PivotTableToolsAnalyzeActiveField { get { return pivotTableToolsAnalyzeActiveField; } }
		static readonly BarInfo pivotTableToolsAnalyzeActiveField = new BarInfo(
			"PivotTable Tools",
			"Analyze",
			"Active Field",
			new BarInfoItems(
				new string[] { "SelectFieldTypePivotTable", "PivotTableExpandField", "PivotTableCollapseField" },
				new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.ButtonSmallWithTextRibbonStyle, BarItemInfos.ButtonSmallWithTextRibbonStyle }
			),
			String.Empty,
			"Caption_PageCategoryPivotTableTools",
			"Caption_PivotTableAnalyze",
			"Caption_PivotTableAnalyzeActiveField",
			"ToolsPivotTableCommandGroup"
		);
		public static BarInfo PivotTableToolsAnalyzeData { get { return pivotTableToolsAnalyzeData; } }
		static BarSubItemInfo refreshSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "RefreshPivotTable", "RefreshAllPivotTable" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
		static readonly BarInfo pivotTableToolsAnalyzeData = new BarInfo(
			"PivotTable Tools",
			"Analyze",
			"Data",
			new BarInfoItems(
				new string[] { "PivotTableDataRefreshGroup", "ChangeDataSourcePivotTable" },
				new BarItemInfo[] { refreshSubItem, BarItemInfos.Button }
			),
			String.Empty,
			"Caption_PageCategoryPivotTableTools",
			"Caption_PivotTableAnalyze",
			"Caption_PivotTableAnalyzeData",
			"ToolsPivotTableCommandGroup"
		);
		public static BarInfo PivotTableToolsAnalyzeActions { get { return pivotTableToolsAnalyzeActions; } }
		static BarSubItemInfo clearSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "ClearAllPivotTable", "ClearFiltersPivotTable" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
		static BarSubItemInfo selectSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "SelectValuesPivotTable", "SelectLabelsPivotTable", "SelectEntirePivotTable" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
		static readonly BarInfo pivotTableToolsAnalyzeActions = new BarInfo(
			"PivotTable Tools",
			"Analyze",
			"Actions",
			new BarInfoItems(
				new string[] { "PivotTableActionsClearGroup", "PivotTableActionsSelectGroup", "MovePivotTable" },
				new BarItemInfo[] { clearSubItem, selectSubItem, BarItemInfos.Button }
			),
			String.Empty,
			"Caption_PageCategoryPivotTableTools",
			"Caption_PivotTableAnalyze",
			"Caption_PivotTableAnalyzeActions",
			"ToolsPivotTableCommandGroup"
		);
		public static BarInfo PivotTableToolsAnalyzeShow { get { return pivotTableToolsAnalyzeShow; } }
		static readonly BarInfo pivotTableToolsAnalyzeShow = new BarInfo(
			"PivotTable Tools",
			"Analyze",
			"Show",
			new BarInfoItems(
				new string[] { "FieldListPanelPivotTable", "ShowPivotTableExpandCollapseButtons", "ShowPivotTableFieldHeaders" },
				new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check }
			),
			String.Empty,
			"Caption_PageCategoryPivotTableTools",
			"Caption_PivotTableAnalyze",
			"Caption_PivotTableAnalyzeShow",
			"ToolsPivotTableCommandGroup"
		);
	}
}
