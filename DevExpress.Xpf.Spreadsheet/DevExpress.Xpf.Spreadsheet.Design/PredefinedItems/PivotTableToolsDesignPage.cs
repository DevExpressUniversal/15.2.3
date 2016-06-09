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
		public static BarInfo PivotTableToolsDesignLayout { get { return pivotTableToolsDesignLayout; } }
		static BarSubItemInfo subtotalsSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "PivotTableDoNotShowSubtotals", "PivotTableShowAllSubtotalsAtBottom", "PivotTableShowAllSubtotalsAtTop" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
		static BarSubItemInfo grandTotalsSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "PivotTableGrandTotalsOffRowsColumns", "PivotTableGrandTotalsOnRowsColumns", "PivotTableGrandTotalsOnRowsOnly", "PivotTableGrandTotalsOnColumnsOnly" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
		static BarSubItemInfo reportLayoutSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "PivotTableShowCompactForm", "PivotTableShowOutlineForm", "PivotTableShowTabularForm", "PivotTableRepeatAllItemLabels", "PivotTableDoNotRepeatItemLabels" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
		static BarSubItemInfo blankRowsSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "PivotTableInsertBlankLineEachItem", "PivotTableRemoveBlankLineEachItem" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
		static readonly BarInfo pivotTableToolsDesignLayout = new BarInfo(
			"PivotTable Tools",
			"Design",
			"Layout",
			new BarInfoItems(
				new string[] { "PivotTableLayoutSubtotalsGroup", "PivotTableLayoutGrandTotalsGroup", "PivotTableLayoutReportLayoutGroup", "PivotTableLayoutBlankRowsGroup" },
				new BarItemInfo[] { subtotalsSubItem, grandTotalsSubItem, reportLayoutSubItem, blankRowsSubItem }
			),
			String.Empty,
			"Caption_PageCategoryPivotTableTools",
			"Caption_PivotTableDesign",
			"Caption_PivotTableDesignLayout",
			"ToolsPivotTableCommandGroup"
		);
		public static BarInfo PivotTableToolsDesignPivotTableStyleOptions { get { return pivotTableToolsDesignPivotTableStyleOptions; } }
		static readonly BarInfo pivotTableToolsDesignPivotTableStyleOptions = new BarInfo(
			"PivotTable Tools",
			"Design",
			"PivotTable Style Options",
			new BarInfoItems(
				new string[] { "PivotTableToggleRowHeaders", "PivotTableToggleColumnHeaders", "PivotTableToggleBandedRows", "PivotTableToggleBandedColumns" },
				new BarItemInfo[] { BarItemInfos.CheckEditItem, BarItemInfos.CheckEditItem, BarItemInfos.CheckEditItem, BarItemInfos.CheckEditItem }
			),
			String.Empty,
			"Caption_PageCategoryPivotTableTools",
			"Caption_PivotTableDesign",
			"Caption_PivotTableDesignPivotTableStyleOptions",
			"ToolsPivotTableCommandGroup"
		);
	}
}
