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
		#region ChartDesignChartData
		public static BarInfo ChartDesignChartData { get { return chartDesignChartData; } }
		static readonly BarInfo chartDesignChartData = CreateChartDesignChartData();
		static BarInfo CreateChartDesignChartData() {
			return new BarInfo(
						"Chart Tools",
						"Design",
						"Data",
						new BarInfoItems(
							new string[] { "ChartSwitchRowColumn", "ChartSelectData" },
							new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button }
						),
						String.Empty,
						"Caption_PageCategoryChartTools",
						"Caption_PageChartsDesign",
						"Caption_GroupChartsDesignData",
						"ToolsChartCommandGroup"
					);
		}
		#endregion
		#region ChartDesignChartLayouts
		public static BarInfo ChartDesignChartLayouts { get { return chartDesignChartLayouts; } }
		static readonly BarInfo chartDesignChartLayouts = CreateChartDesignChartLayouts();
		static BarInfo CreateChartDesignChartLayouts() {
			return new BarInfo(
						"Chart Tools",
						"Design",
						"Chart Layouts",
						new BarInfoItems(
							new string[] { String.Empty },
							new BarItemInfo[] { new GalleryChartLayoutsItemInfo() }
						),
						String.Empty,
						"Caption_PageCategoryChartTools",
						"Caption_PageChartsDesign",
						"Caption_GroupChartsDesignLayouts",
						"ToolsChartCommandGroup"
					);
		}
		#endregion
	}
}
