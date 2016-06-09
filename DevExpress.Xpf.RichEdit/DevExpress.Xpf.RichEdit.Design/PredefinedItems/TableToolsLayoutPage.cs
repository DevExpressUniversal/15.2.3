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
namespace DevExpress.Xpf.RichEdit.Design {
	public static partial class BarInfos {
		#region Table
		public static BarInfo TableLayoutTable { get { return tableLayoutTable; } }
		static readonly BarInfo tableLayoutTable = new BarInfo(
			"Table Tools",
			"Layout",
			"Table",
			new BarInfoItems(
				new string[] { "TableSelectElement", "TableToggleShowGridlines", "TableProperties" },
				new BarItemInfo[] {
					new BarSubItemInfo(new BarInfoItems(
						new string[] { "TableSelectCell", "TableSelectColumn", "TableSelectRow", "TableSelect" },
						new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check })
					),
					BarItemInfos.Check, BarItemInfos.Button }
			),
			String.Empty,
			"Caption_PageCategoryTableTools",
			"Caption_PageTableLayout",
			"Caption_GroupTableTable",
			"ToolsTableCommandGroup"
		);
		#endregion
		#region RowsAndColumns
		public static BarInfo TableLayoutRowsAndColumns { get { return tableLayoutRowsAndColumns; } }
		static readonly BarInfo tableLayoutRowsAndColumns = new BarInfo(
			"Table Tools",
			"Layout",
			"Rows & Columns",
			new BarInfoItems(
				new string[] { "TableDeleteElement", "TableInsertRowAbove", "TableInsertRowBelow", "TableInsertColumnToLeft", "TableInsertColumnToRight", "TableInsertCells" },
				new BarItemInfo[] { 
					new BarSubItemInfo(new BarInfoItems(
						new string[] { "TableDeleteCell", "TableDeleteColumn", "TableDeleteRow", "TableDelete" },
						new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check })
					),
					BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			),
			String.Empty,
			"Caption_PageCategoryTableTools",
			"Caption_PageTableLayout",
			"Caption_GroupTableRowsAndColumns",
			"ToolsTableCommandGroup"
		);
		#endregion
		#region Merge
		public static BarInfo TableLayoutMerge { get { return tableLayoutMerge; } }
		static readonly BarInfo tableLayoutMerge = new BarInfo(
			"Table Tools",
			"Layout",
			"Merge",
			new BarInfoItems(
				new string[] { "TableMergeCells", "TableSplitCells", "TableSplit" },
				new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			),
			String.Empty,
			"Caption_PageCategoryTableTools",
			"Caption_PageTableLayout",
			"Caption_GroupTableMerge",
			"ToolsTableCommandGroup"
		);
		#endregion
		#region CellSize
		public static BarInfo TableLayoutCellSize { get { return tableLayoutCellSize; } }
		static readonly BarInfo tableLayoutCellSize = new BarInfo(
			"Table Tools",
			"Layout",
			"Cell Size",
			new BarInfoItems(
				new string[] { "ToggleTableAutoFit" },
				new BarItemInfo[] { 
					new BarSubItemInfo(new BarInfoItems(
						new string[] { "ToggleTableAutoFitContents", "ToggleTableAutoFitWindow", "ToggleTableFixedColumnWidth" },
						new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check })
					) 
				}
			),
			"TableProperties",
			"Caption_PageCategoryTableTools",
			"Caption_PageTableLayout",
			"Caption_GroupTableCellSize",
			"ToolsTableCommandGroup"
		);
		#endregion
		#region Alignment
		public static BarInfo TableLayoutAlignment { get { return tableLayoutAlignment; } }
		static readonly BarInfo tableLayoutAlignment = new BarInfo(
			"Table Tools",
			"Layout",
			"Alignment",
			new BarInfoItems(
				new string[] { "TableToggleCellsTopLeftAlignment", "TableToggleCellsTopCenterAlignment", "TableToggleCellsTopRightAlignment", "TableToggleCellsMiddleLeftAlignment", "TableToggleCellsMiddleCenterAlignment", "TableToggleCellsMiddleRightAlignment", "TableToggleCellsBottomLeftAlignment", "TableToggleCellsBottomCenterAlignment", "TableToggleCellsBottomRightAlignment", "TableOptions" },
				new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Button },
				new int[] { 0, 3, 6, 1, 4, 7, 2, 5, 8, 9 },
				new int[] { 3, 6 },
				new int[] { 9 }
			),
			String.Empty,
			"Caption_PageCategoryTableTools",
			"Caption_PageTableLayout",
			"Caption_GroupTableAlignment",
			"ToolsTableCommandGroup"
		);
		#endregion
	}
}
