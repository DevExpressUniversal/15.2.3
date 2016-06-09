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
		#region TableStyles
		public static BarInfo TableDesignStyles { get { return tableDesignStyles; } }
		static readonly BarInfo tableDesignStyles = new BarInfo(
			"Table Tools",
			"Design",
			"Table Styles",
			new BarInfoItems(
				new string[] { "TableChangeCellsShading", "TableChangeBorders" },
				new BarItemInfo[] {
					BarItemInfos.BackColorSplitButton,
					new BarSubItemInfo(new BarInfoItems(
						new string[] { "TableToggleBottomBorder", "TableToggleTopBorder", "TableToggleLeftBorder", "TableToggleRightBorder", "TableResetAllBorders", "TableToggleAllBorders", "TableToggleOutsideBorder", "TableToggleInsideBorder", "TableToggleInsideHorizontalBorder", "TableToggleInsideVerticalBorder" },
						new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Button, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check })
					)
				}
			),
			String.Empty,
			"Caption_PageCategoryTableTools",
			"Caption_PageTableDesign",
			"Caption_GroupTableStyles",
			"ToolsTableCommandGroup"
		);
		#endregion
		#region DrawBorders
		public static BarInfo TableDesignDrawBorders { get { return tableDesignDrawBorders; } }
		static readonly BarInfo tableDesignDrawBorders = new BarInfo(
			"Table Tools",
			"Design",
			"Draw Borders",
			new BarInfoItems(
				new string[] { "TableChangeCellsBorderColor"  },
				new BarItemInfo[] { BarItemInfos.ForeColorSplitButton,  }
			),
			String.Empty,
			"Caption_PageCategoryTableTools",
			"Caption_PageTableDesign",
			"Caption_GroupTableDrawBorders",
			"ToolsTableCommandGroup"
		);
		#endregion
	}
}
