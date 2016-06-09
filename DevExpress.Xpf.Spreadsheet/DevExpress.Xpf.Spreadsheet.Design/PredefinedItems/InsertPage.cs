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
		#region Tables
		public static BarInfo Tables { get { return tables; } }
		static readonly BarInfo tables = new BarInfo(
			String.Empty,
			"Insert",
			"Tables",
			new BarInfoItems(
				new string[] { "InsertPivotTable", "InsertTable" },
				new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button }
			),
			String.Empty,
			String.Empty,
			"Caption_PageInsert",
			"Caption_GroupTables"
		);
		#endregion
		#region Illustrations
		public static BarInfo Illustrations { get { return illustrations; } }
		static readonly BarInfo illustrations = new BarInfo(
			String.Empty,
			"Insert",
			"Illustrations",
			new BarInfoItems(
				new string[] { "InsertPicture" },
				new BarItemInfo[] { BarItemInfos.Button }
			),
			String.Empty,
			String.Empty,
			"Caption_PageInsert",
			"Caption_GroupIllustrations"
		);
		#endregion
		#region Charts
		#region ColumnCharts
		static readonly GalleryItemGroupInfo Column2DSubItem = new GalleryItemGroupInfo(
			new BarInfoItems(
				new string[] { "InsertChartColumnClustered2D", "InsertChartColumnStacked2D", "InsertChartColumnPercentStacked2D" },
				new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
			),
			true,
			"MenuCmd_InsertChartColumn2DCommandGroup"
		);
		static readonly GalleryItemGroupInfo Column3DSubItem = new GalleryItemGroupInfo(
			new BarInfoItems(
				new string[] { "InsertChartColumnClustered3D", "InsertChartColumnStacked3D", "InsertChartColumnPercentStacked3D", "InsertChartColumn3D" },
				new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
			),
			true,
			"MenuCmd_InsertChartColumn3DCommandGroup"
		);
		static readonly GalleryItemGroupInfo CylinderSubItem = new GalleryItemGroupInfo(
			new BarInfoItems(
				new string[] { "InsertChartCylinderClustered", "InsertChartCylinderStacked", "InsertChartCylinderPercentStacked", "InsertChartCylinder" },
				new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
			),
			true,
			"MenuCmd_InsertChartCylinderCommandGroup"
		);
		static readonly GalleryItemGroupInfo ConeSubItem = new GalleryItemGroupInfo(
			new BarInfoItems(
				new string[] { "InsertChartConeClustered", "InsertChartConeStacked", "InsertChartConePercentStacked", "InsertChartCone" },
				new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
			),
			true,
			"MenuCmd_InsertChartConeCommandGroup"
		);
		static readonly GalleryItemGroupInfo PyramidSubItem = new GalleryItemGroupInfo(
			new BarInfoItems(
				new string[] { "InsertChartPyramidClustered", "InsertChartPyramidStacked", "InsertChartPyramidPercentStacked", "InsertChartPyramid" },
				new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
			),
			true,
			"MenuCmd_InsertChartPyramidCommandGroup"
		);
		static readonly BarDropDownGalleryItemInfo ColumnChartsSubItem = new BarDropDownGalleryItemInfo(
			new BarInfoItems(
				new string[] { "InsertChartColumn2DCommandGroup", "InsertChartColumn3DCommandGroup", "InsertChartCylinderCommandGroup", "InsertChartConeCommandGroup", "InsertChartPyramidCommandGroup" },
				new BarItemInfo[] { Column2DSubItem, Column3DSubItem, CylinderSubItem, ConeSubItem, PyramidSubItem }
			)
		) { IsItemCaptionVisible = false, IsItemDescriptionVisible = false, ColumnCount = 4 };
		#endregion
		#region LineCharts
		static readonly GalleryItemGroupInfo Line2DSubItem = new GalleryItemGroupInfo(
			new BarInfoItems(
				new string[] { "InsertChartLine", "InsertChartStackedLine", "InsertChartPercentStackedLine", "InsertChartLineWithMarkers", "InsertChartStackedLineWithMarkers", "InsertChartPercentStackedLineWithMarkers" },
				new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
			),
			true,
			"MenuCmd_InsertChartLine2DCommandGroup"
		);
		static readonly GalleryItemGroupInfo Line3DSubItem = new GalleryItemGroupInfo(
			new BarInfoItems(
				new string[] { "InsertChartLine3D" },
				new BarItemInfo[] { BarItemInfos.GalleryItem }
			),
			true,
			"MenuCmd_InsertChartLine3DCommandGroup"
		);
		static readonly BarDropDownGalleryItemInfo LineChartsSubItem = new BarDropDownGalleryItemInfo(
			new BarInfoItems(
				new string[] { "InsertChartLine2DCommandGroup", "InsertChartLine3DCommandGroup" },
				new BarItemInfo[] { Line2DSubItem, Line3DSubItem }
			)
		) { IsItemCaptionVisible = false, IsItemDescriptionVisible = false, ColumnCount = 4 };
		#endregion
		#region PieCharts
		static readonly GalleryItemGroupInfo Pie2DSubItem = new GalleryItemGroupInfo(
			new BarInfoItems(
				new string[] { "InsertChartPie2D", "InsertChartPieExploded2D" },
				new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
			),
			true,
			"MenuCmd_InsertChartPie2DCommandGroup"
		);
		static readonly GalleryItemGroupInfo Pie3DSubItem = new GalleryItemGroupInfo(
			new BarInfoItems(
				new string[] { "InsertChartPie3D", "InsertChartPieExploded3D" },
				new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
			),
			true,
			"MenuCmd_InsertChartPie3DCommandGroup"
		);
		static readonly GalleryItemGroupInfo Doughnut2DSubItem = new GalleryItemGroupInfo(
			new BarInfoItems(
				new string[] { "InsertChartDoughnut2D", "InsertChartDoughnutExploded2D" },
				new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
			),
			true,
			"MenuCmd_InsertChartDoughnut2DCommandGroup"
		);
		static readonly BarDropDownGalleryItemInfo PieChartsSubItem = new BarDropDownGalleryItemInfo(
			new BarInfoItems(
				new string[] { "InsertChartPie2DCommandGroup", "InsertChartPie3DCommandGroup", "InsertChartDoughnut2DCommandGroup" },
				new BarItemInfo[] { Pie2DSubItem, Pie3DSubItem, Doughnut2DSubItem }
			)
		) { IsItemCaptionVisible = false, IsItemDescriptionVisible = false, ColumnCount = 2 };
		#endregion
		#region BarCharts
		static readonly GalleryItemGroupInfo Bar2DSubItem = new GalleryItemGroupInfo(
			new BarInfoItems(
				new string[] { "InsertChartBarClustered2D", "InsertChartBarStacked2D", "InsertChartBarPercentStacked2D" },
				new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
			),
			true,
			"MenuCmd_InsertChartBar2DCommandGroup"
		);
		static readonly GalleryItemGroupInfo Bar3DSubItem = new GalleryItemGroupInfo(
			new BarInfoItems(
				new string[] { "InsertChartBarClustered3D", "InsertChartBarStacked3D", "InsertChartBarPercentStacked3D" },
				new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
			),
			true,
			"MenuCmd_InsertChartBar3DCommandGroup"
		);
		static readonly GalleryItemGroupInfo HorizontalCylinderSubItem = new GalleryItemGroupInfo(
			new BarInfoItems(
				new string[] { "InsertChartHorizontalCylinderClustered", "InsertChartHorizontalCylinderStacked", "InsertChartHorizontalCylinderPercentStacked" },
				new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
			),
			true,
			"MenuCmd_InsertChartHorizontalCylinderCommandGroup"
		);
		static readonly GalleryItemGroupInfo HorizontalConeSubItem = new GalleryItemGroupInfo(
			new BarInfoItems(
				new string[] { "InsertChartHorizontalConeClustered", "InsertChartHorizontalConeStacked", "InsertChartHorizontalConePercentStacked" },
				new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
			),
			true,
			"MenuCmd_InsertChartHorizontalConeCommandGroup"
		);
		static readonly GalleryItemGroupInfo HorizontalPyramidSubItem = new GalleryItemGroupInfo(
			new BarInfoItems(
				new string[] { "InsertChartHorizontalPyramidClustered", "InsertChartHorizontalPyramidStacked", "InsertChartHorizontalPyramidPercentStacked" },
				new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
			),
			true,
			"MenuCmd_InsertChartHorizontalPyramidCommandGroup"
		);
		static readonly BarDropDownGalleryItemInfo BarChartsSubItem = new BarDropDownGalleryItemInfo(
			new BarInfoItems(
				new string[] { "InsertChartBar2DCommandGroup", "InsertChartBar3DCommandGroup", "InsertChartHorizontalCylinderCommandGroup", "InsertChartHorizontalConeCommandGroup", "InsertChartHorizontalPyramidCommandGroup" },
				new BarItemInfo[] { Bar2DSubItem, Bar3DSubItem, HorizontalCylinderSubItem, HorizontalConeSubItem, HorizontalPyramidSubItem }
			)
		) { IsItemCaptionVisible = false, IsItemDescriptionVisible = false, ColumnCount = 3 };
		#endregion
		#region AreaCharts
		static readonly GalleryItemGroupInfo Area2DSubItem = new GalleryItemGroupInfo(
			new BarInfoItems(
				new string[] { "InsertChartArea", "InsertChartStackedArea", "InsertChartPercentStackedArea" },
				new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
			),
			true,
			"MenuCmd_InsertChartArea2DCommandGroup"
		);
		static readonly GalleryItemGroupInfo Area3DSubItem = new GalleryItemGroupInfo(
			new BarInfoItems(
				new string[] { "InsertChartArea3D", "InsertChartStackedArea3D", "InsertChartPercentStackedArea3D" },
				new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
			),
			true,
			"MenuCmd_InsertChartArea3DCommandGroup"
		);
		static readonly BarDropDownGalleryItemInfo AreaChartsSubItem = new BarDropDownGalleryItemInfo(
			new BarInfoItems(
				new string[] { "InsertChartArea2DCommandGroup", "InsertChartArea3DCommandGroup" },
				new BarItemInfo[] { Area2DSubItem, Area3DSubItem }
			)
		) { IsItemCaptionVisible = false, IsItemDescriptionVisible = false, ColumnCount = 3 };
		#endregion
		#region ScatterCharts
		static readonly GalleryItemGroupInfo ScatterSubItem = new GalleryItemGroupInfo(
			new BarInfoItems(
				new string[] { "InsertChartScatterMarkers", "InsertChartScatterSmoothLinesAndMarkers", "InsertChartScatterSmoothLines", "InsertChartScatterLinesAndMarkers", "InsertChartScatterLines" },
				new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
			),
			true,
			"MenuCmd_InsertChartScatterCommandGroup"
		);
		static readonly GalleryItemGroupInfo BubbleSubItem = new GalleryItemGroupInfo(
			new BarInfoItems(
				new string[] { "InsertChartBubble", "InsertChartBubble3D" },
				new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
			),
			true,
			"MenuCmd_InsertChartBubbleCommandGroup"
		);
		static readonly BarDropDownGalleryItemInfo ScatterChartsSubItem = new BarDropDownGalleryItemInfo(
			new BarInfoItems(
				new string[] { "InsertChartScatterCommandGroup", "InsertChartBubbleCommandGroup" },
				new BarItemInfo[] { ScatterSubItem, BubbleSubItem }
			)
		) { IsItemCaptionVisible = false, IsItemDescriptionVisible = false, ColumnCount = 2 };
		#endregion
		#region OtherCharts
		static readonly GalleryItemGroupInfo StockSubItem = new GalleryItemGroupInfo(
			new BarInfoItems(
				new string[] { "InsertChartStockHighLowClose", "InsertChartStockOpenHighLowClose", "InsertChartStockVolumeHighLowClose", "InsertChartStockVolumeOpenHighLowClose" },
				new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
			),
			true,
			"MenuCmd_InsertChartStockCommandGroup"
		);
		static readonly GalleryItemGroupInfo RadarSubItem = new GalleryItemGroupInfo(
			new BarInfoItems(
				new string[] { "InsertChartRadar", "InsertChartRadarWithMarkers", "InsertChartRadarFilled" },
				new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
			),
			true,
			"MenuCmd_InsertChartRadarCommandGroup"
		);
		static readonly BarDropDownGalleryItemInfo OtherChartsSubItem = new BarDropDownGalleryItemInfo(
			new BarInfoItems(
				new string[] { "InsertChartStockCommandGroup", "InsertChartRadarCommandGroup" },
				new BarItemInfo[] { StockSubItem, RadarSubItem }
			)
		) { IsItemCaptionVisible = false, IsItemDescriptionVisible = false, ColumnCount = 4 };
		#endregion
		public static BarInfo Charts { get { return charts; } }
		static readonly BarInfo charts = new BarInfo(
			String.Empty,
			"Insert",
			"Charts",
			new BarInfoItems(
				new string[] { "InsertChartColumnCommandGroup", "InsertChartLineCommandGroup", "InsertChartPieCommandGroup", "InsertChartBarCommandGroup", "InsertChartAreaCommandGroup", "InsertChartScatterCommandGroup", "InsertChartOtherCommandGroup" },
				new BarItemInfo[] { ColumnChartsSubItem, LineChartsSubItem, PieChartsSubItem, BarChartsSubItem, AreaChartsSubItem, ScatterChartsSubItem, OtherChartsSubItem }
			),
			String.Empty,
			String.Empty,
			"Caption_PageInsert",
			"Caption_GroupCharts"
		);
		#endregion
		#region Links
		public static BarInfo Links { get { return links; } }
		static readonly BarInfo links = new BarInfo(
			String.Empty,
			"Insert",
			"Links",
			new BarInfoItems(
				new string[] { "InsertHyperlink" },
				new BarItemInfo[] { BarItemInfos.Button }
			),
			String.Empty,
			String.Empty,
			"Caption_PageInsert",
			"Caption_GroupLinks"
		);
		#endregion
		#region Symbols
		public static BarInfo Symbols { get { return symbols; } }
		static readonly BarInfo symbols = new BarInfo(
			String.Empty,
			"Insert",
			"Symbols",
			new BarInfoItems(
				new string[] { "InsertSymbol" },
				new BarItemInfo[] { BarItemInfos.Button }
			),
			String.Empty,
			String.Empty,
			"Caption_PageInsert",
			"Caption_GroupSymbols"
		);
		#endregion
	}
}
