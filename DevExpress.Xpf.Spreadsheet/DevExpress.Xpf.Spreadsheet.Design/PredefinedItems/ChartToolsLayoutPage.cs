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
		#region ChartToolsLayoutLabels
		#region ChartTitleSubItem
		static readonly BarDropDownGalleryItemInfo ChartTitleSubItem = new BarDropDownGalleryItemInfo(
		   new BarInfoItems(
			   new string[] { "ChartTitleCommandGroup" },
			   new BarItemInfo[] {
				   new GalleryItemGroupInfo(
					   new BarInfoItems(
						   new string[] { "ChartTitleNone", "ChartTitleCenteredOverlay", "ChartTitleAbove" },
						   new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
					   ),
					   false,
					   "MenuCmd_ChartTitleCommandGroup"
				   )
			   }
		   )
		) { IsItemCaptionVisible = true, IsItemDescriptionVisible = true, ColumnCount = 1 };
		#endregion
		#region AxisTitlesSubItem
		static readonly BarDropDownGalleryItemInfo PrimaryHorizontalAxisTitlesSubItem = new BarDropDownGalleryItemInfo(
		   new BarInfoItems(
			   new string[] { "ChartPrimaryHorizontalAxisTitleCommandGroup" },
			   new BarItemInfo[] {
				   new GalleryItemGroupInfo(
					   new BarInfoItems(
						   new string[] { "ChartPrimaryHorizontalAxisTitleNone", "ChartPrimaryHorizontalAxisTitleBelow" },
						   new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
					   ),
					   false,
					   "MenuCmd_ChartPrimaryHorizontalAxisTitleCommandGroup"
				   )
			   }
		   )
		) { IsItemCaptionVisible = true, IsItemDescriptionVisible = true, ColumnCount = 1 };
		static readonly BarDropDownGalleryItemInfo PrimaryVerticalAxisTitlesSubItem = new BarDropDownGalleryItemInfo(
		   new BarInfoItems(
			   new string[] { "ChartPrimaryVerticalAxisTitleCommandGroup" },
			   new BarItemInfo[] {
				   new GalleryItemGroupInfo(
					   new BarInfoItems(
						   new string[] { "ChartPrimaryVerticalAxisTitleNone", "ChartPrimaryVerticalAxisTitleRotated", "ChartPrimaryVerticalAxisTitleVertical", "ChartPrimaryVerticalAxisTitleHorizontal" },
						   new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
					   ),
					   false,
					   "MenuCmd_ChartPrimaryVerticalAxisTitleCommandGroup"
				   )
			   }
		   )
		) { IsItemCaptionVisible = true, IsItemDescriptionVisible = true, ColumnCount = 1 };
		static readonly BarSubItemInfo AxisTitlesSubItem = new BarSubItemInfo(
			new BarInfoItems(
				new string[] { "ChartPrimaryHorizontalAxisTitleCommandGroup", "ChartPrimaryVerticalAxisTitleCommandGroup" },
				new BarItemInfo[] { PrimaryHorizontalAxisTitlesSubItem, PrimaryVerticalAxisTitlesSubItem }
			)
		);
		#endregion
		#region LegendSubItem
		static readonly BarDropDownGalleryItemInfo LegendSubItem = new BarDropDownGalleryItemInfo(
		   new BarInfoItems(
			   new string[] { "ChartLegendCommandGroup" },
			   new BarItemInfo[] {
				   new GalleryItemGroupInfo(
					   new BarInfoItems(
						   new string[] { "ChartLegendNone", "ChartLegendAtRight", "ChartLegendAtTop", "ChartLegendAtLeft", "ChartLegendAtBottom", "ChartLegendOverlayAtRight", "ChartLegendOverlayAtLeft" },
						   new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
					   ),
					   false,
					   "MenuCmd_ChartLegendCommandGroup"
				   )
			   }
		   )
		) { IsItemCaptionVisible = true, IsItemDescriptionVisible = true, ColumnCount = 1 };
		#endregion
		#region DataLabelsSubItem
		static readonly BarDropDownGalleryItemInfo DataLabelsSubItem = new BarDropDownGalleryItemInfo(
		   new BarInfoItems(
			   new string[] { "ChartDataLabelsCommandGroup" },
			   new BarItemInfo[] {
				   new GalleryItemGroupInfo(
					   new BarInfoItems(
						   new string[] { "ChartDataLabelsNone", "ChartDataLabelsDefault", "ChartDataLabelsCenter", "ChartDataLabelsInsideEnd", "ChartDataLabelsInsideBase", "ChartDataLabelsOutsideEnd", "ChartDataLabelsBestFit", "ChartDataLabelsLeft", "ChartDataLabelsRight", "ChartDataLabelsAbove", "ChartDataLabelsBelow" },
						   new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
					   ),
					   false,
					   "MenuCmd_ChartDataLabelsCommandGroup"
				   )
			   }
		   )
		) { IsItemCaptionVisible = true, IsItemDescriptionVisible = true, ColumnCount = 1 };
		#endregion
		public static BarInfo ChartToolsLayoutLabels { get { return chartToolsLayoutLabels; } }
		static readonly BarInfo chartToolsLayoutLabels = new BarInfo(
			"Chart Tools",
			"Layout",
			"Labels",
			new BarInfoItems(
				new string[] { "ChartTitleCommandGroup", "ChartAxisTitlesCommandGroup", "ChartLegendCommandGroup", "ChartDataLabelsCommandGroup" },
				new BarItemInfo[] { ChartTitleSubItem, AxisTitlesSubItem, LegendSubItem, DataLabelsSubItem }
			),
			String.Empty,
			"Caption_PageCategoryChartTools",
			"Caption_PageChartsLayout",
			"Caption_GroupChartsLayoutLabels",
			"ToolsChartCommandGroup"
		);
		#endregion
		#region ChartToolsLayoutAxes
		#region AxesSubItem
		static readonly BarDropDownGalleryItemInfo PrimaryHorizontalAxesSubItem = new BarDropDownGalleryItemInfo(
		   new BarInfoItems(
			   new string[] { "ChartPrimaryHorizontalAxisCommandGroup" },
			   new BarItemInfo[] {
				   new GalleryItemGroupInfo(
					   new BarInfoItems(
						   new string[] { "ChartHidePrimaryHorizontalAxis", "ChartPrimaryHorizontalAxisLeftToRight", "ChartPrimaryHorizontalAxisRightToLeft", "ChartPrimaryHorizontalAxisHideLabels", "ChartPrimaryHorizontalAxisDefault", "ChartPrimaryHorizontalAxisScaleLogarithm", "ChartPrimaryHorizontalAxisScaleThousands", "ChartPrimaryHorizontalAxisScaleMillions", "ChartPrimaryHorizontalAxisScaleBillions" },
						   new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
					   ),
					   false,
					   "MenuCmd_ChartPrimaryHorizontalAxisCommandGroup"
				   )
			   }
		   )
		) { IsItemCaptionVisible = true, IsItemDescriptionVisible = true, ColumnCount = 1 };
		static readonly BarDropDownGalleryItemInfo PrimaryVerticalAxesSubItem = new BarDropDownGalleryItemInfo(
		   new BarInfoItems(
			   new string[] { "ChartPrimaryVerticalAxisCommandGroup" },
			   new BarItemInfo[] {
				   new GalleryItemGroupInfo(
					   new BarInfoItems(
						   new string[] { "ChartHidePrimaryVerticalAxis", "ChartPrimaryVerticalAxisLeftToRight", "ChartPrimaryVerticalAxisRightToLeft", "ChartPrimaryVerticalAxisHideLabels", "ChartPrimaryVerticalAxisDefault", "ChartPrimaryVerticalAxisScaleLogarithm", "ChartPrimaryVerticalAxisScaleThousands", "ChartPrimaryVerticalAxisScaleMillions", "ChartPrimaryVerticalAxisScaleBillions" },
						   new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
					   ),
					   false,
					   "MenuCmd_ChartPrimaryVerticalAxisCommandGroup"
				   )
			   }
		   )
		) { IsItemCaptionVisible = true, IsItemDescriptionVisible = true, ColumnCount = 1 };
		static readonly BarSubItemInfo AxesSubItem = new BarSubItemInfo(
			new BarInfoItems(
				new string[] { "ChartPrimaryHorizontalAxisCommandGroup", "ChartPrimaryVerticalAxisCommandGroup" },
				new BarItemInfo[] { PrimaryHorizontalAxesSubItem, PrimaryVerticalAxesSubItem  }
			)
		);
		#endregion
		#region GridlinesSubItem
		static readonly BarDropDownGalleryItemInfo PrimaryHorizontalGridlinesSubItem = new BarDropDownGalleryItemInfo(
		   new BarInfoItems(
			   new string[] { "ChartPrimaryHorizontalGridlinesCommandGroup" },
			   new BarItemInfo[] {
				   new GalleryItemGroupInfo(
					   new BarInfoItems(
						   new string[] { "ChartPrimaryHorizontalGridlinesNone", "ChartPrimaryHorizontalGridlinesMajor", "ChartPrimaryHorizontalGridlinesMinor", "ChartPrimaryHorizontalGridlinesMajorAndMinor" },
						   new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
					   ),
					   false,
					   "MenuCmd_ChartPrimaryHorizontalGridlinesCommandGroup"
				   )
			   }
		   )
		) { IsItemCaptionVisible = true, IsItemDescriptionVisible = true, ColumnCount = 1 };
		static readonly BarDropDownGalleryItemInfo PrimaryVerticalGridlinesSubItem = new BarDropDownGalleryItemInfo(
		   new BarInfoItems(
			   new string[] { "ChartPrimaryVerticalGridlinesCommandGroup" },
			   new BarItemInfo[] {
				   new GalleryItemGroupInfo(
					   new BarInfoItems(
						   new string[] { "ChartPrimaryVerticalGridlinesNone", "ChartPrimaryVerticalGridlinesMajor", "ChartPrimaryVerticalGridlinesMinor", "ChartPrimaryVerticalGridlinesMajorAndMinor" },
						   new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
					   ),
					   false,
					   "MenuCmd_ChartPrimaryVerticalGridlinesCommandGroup"
				   )
			   }
		   )
		) { IsItemCaptionVisible = true, IsItemDescriptionVisible = true, ColumnCount = 1 };
		static readonly BarSubItemInfo GridlinesSubItem = new BarSubItemInfo(
			new BarInfoItems(
				new string[] { "ChartPrimaryHorizontalGridlinesCommandGroup", "ChartPrimaryVerticalGridlinesCommandGroup" },
				new BarItemInfo[] { PrimaryHorizontalGridlinesSubItem, PrimaryVerticalGridlinesSubItem }
			)
		);
		#endregion
		public static BarInfo ChartToolsLayoutAxes { get { return chartToolsLayoutAxes; } }
		static readonly BarInfo chartToolsLayoutAxes = new BarInfo(
			"Chart Tools",
			"Layout",
			"Axes",
			new BarInfoItems(
				new string[] { "ChartAxesCommandGroup", "ChartGridlinesCommandGroup" },
				new BarItemInfo[] { AxesSubItem, GridlinesSubItem }
			),
			String.Empty,
			"Caption_PageCategoryChartTools",
			"Caption_PageChartsLayout",
			"Caption_GroupChartsLayoutAxes",
			"ToolsChartCommandGroup"
		);
		#endregion
		#region ChartToolsLayoutAnalysis
		#region LinesSubItem
		static readonly BarDropDownGalleryItemInfo LinesSubItem = new BarDropDownGalleryItemInfo(
		   new BarInfoItems(
			   new string[] { "ChartLinesCommandGroup" },
			   new BarItemInfo[] {
				   new GalleryItemGroupInfo(
					   new BarInfoItems(
						   new string[] { "ChartLinesNone", "ChartShowDropLines", "ChartShowHighLowLines", "ChartShowDropLinesAndHighLowLines", "ChartShowSeriesLines" },
						   new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
					   ),
					   false,
					   "MenuCmd_ChartLinesCommandGroup"
				   )
			   }
		   )
		) { IsItemCaptionVisible = true, IsItemDescriptionVisible = true, ColumnCount = 1 };
		#endregion
		#region UpDownBarsSubItem
		static readonly BarDropDownGalleryItemInfo UpDownBarsSubItem = new BarDropDownGalleryItemInfo(
		   new BarInfoItems(
			   new string[] { "ChartUpDownBarsCommandGroup" },
			   new BarItemInfo[] {
				   new GalleryItemGroupInfo(
					   new BarInfoItems(
						   new string[] { "ChartHideUpDownBars", "ChartShowUpDownBars" },
						   new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
					   ),
					   false,
					   "MenuCmd_ChartUpDownBarsCommandGroup"
				   )
			   }
		   )
		) { IsItemCaptionVisible = true, IsItemDescriptionVisible = true, ColumnCount = 1 };
		#endregion
		#region ErrorBarsSubItem
		static readonly BarDropDownGalleryItemInfo ErrorBarsSubItem = new BarDropDownGalleryItemInfo(
		   new BarInfoItems(
			   new string[] { "ChartErrorBarsCommandGroup" },
			   new BarItemInfo[] {
				   new GalleryItemGroupInfo(
					   new BarInfoItems(
						   new string[] { "ChartErrorBarsNone", "ChartErrorBarsPercentage", "ChartErrorBarsStandardError", "ChartErrorBarsStandardDeviation" },
						   new BarItemInfo[] { BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem, BarItemInfos.GalleryItem }
					   ),
					   false,
					   "MenuCmd_ChartErrorBarsCommandGroup"
				   )
			   }
		   )
		) { IsItemCaptionVisible = true, IsItemDescriptionVisible = true, ColumnCount = 1 };
		#endregion
		public static BarInfo ChartToolsLayoutAnalysis { get { return chartToolsLayoutAnalysis; } }
		static readonly BarInfo chartToolsLayoutAnalysis = new BarInfo(
			"Chart Tools",
			"Layout",
			"Analysis",
			new BarInfoItems(
				new string[] { "ChartLinesCommandGroup", "ChartUpDownBarsCommandGroup", "ChartErrorBarsCommandGroup" },
				new BarItemInfo[] { LinesSubItem, UpDownBarsSubItem, ErrorBarsSubItem }
			),
			String.Empty,
			"Caption_PageCategoryChartTools",
			"Caption_PageChartsLayout",
			"Caption_GroupChartsLayoutAnalysis",
			"ToolsChartCommandGroup"
		);
		#endregion
	}
}
