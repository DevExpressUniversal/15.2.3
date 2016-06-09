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

using DevExpress.Xpf.Utils.Themes;
namespace DevExpress.Xpf.Charts.Themes {
	public enum ChartThemeKeys {
		XYDiagram3DWrapperTemplate,
		SimpleDiagram3DWrapperTemplate,
		[BlendVisibility(false)]
		ChartPaletteName,
		ChartPalette,
		ChartPaddingValue,
		ChartBorderThickness,
		ChartWrapperTemplate,
		LegendWrapperTemplate,
		LegendItemMargin,
		LegendItemTextMargin,
		LegendMarkerWidth,
		LegendMarkerHeight,
		LegendPaddingValue,
		LegendBorderThickness,
		LegendCheckBoxTemplate,
		IndentFromDiagramValue,
		XYDiagram2DWrapperTemplate,
		AxisLabelTemplate,
		AxisLabelFontSize,
		AxisLabelPadding,
		AxisTitlePadding,
		AxisTitleFontSize,
		AxisScrollBarTemplate,
		SimpleDiagram2DWrapperTemplate,
		SeriesLabelTemplate,
		BarSeriesLegendMarkerTemplate,
		MarkerSeriesLegendMarkerTemplate,
		LineSeriesLegendMarkerTemplate,
		AreaSeriesLegendMarkerTemplate,
		AreaSplineSeriesLegendMarkerTemplate,
		RangeAreaSeriesLegendMarkerTemplate,
		PieSeriesLegendMarkerTemplate,
		NestedDonutLegendMarkerTemplate,
		FunnelSeriesLegendMarkerTemplate,
		CandleStickSeriesLegendMarkerTemplate,
		TitleTemplate,
		TitleFontSize,
		TitleMarginValue,
		ToolTipPresentationTemplate,
		ToolTipTemplate,
		CrosshairSeriesLabelTemplate,
		CrosshairAxisLabelTemplate,
		CrosshairAxisLabelPresentationTemplate,
		CrosshairSeriesLabelContentTemplate,
		ConstantLineLegendMarkerTemplate,
		StripLegendMarkerTemplate,
		IndicatorLegendMarkerDefaultTemplate,		
	}
	public class ChartControlThemeKeyExtension : ThemeKeyExtensionBase<ChartThemeKeys> { }
	public enum ChartBrushesThemeKeys {
		Domain3DBrush,
		Axis3DInterlacedBrush,
		ChartBackgroundBrush,
		ChartBorderBrush,
		Domain2DBrush,
		DomainBorderBrush,
		SeriesLabelForeground,
		AxisBrush,
		Axis2DInterlacedBrush,
		StripBrush,
		StripBorderColor,
		ConstantLineBrush,
		ConstantLineTitleForeground,
		GridLineBrush,
		MinorGridLineBrush,
		AxisLabelForeground,
		AxisTitleForeground,
		LegendBorderBrush,
		LegendForeground,
		LegendBackgroundBrush,
		TitleForeground,
		ToolTipForeground,
		CrosshairSeriesLabelForeground,
		RangeControlClientViewBrush,
		RangeControlClientViewMarkerBrush,
		RangeControlClientViewAreaOpacity,
		RangeControlClientLabelTemplate,
		RangeControlClientGridLineBrush,
	}
	public class ChartBrushesThemeKeyExtension : ThemeKeyExtensionBase<ChartBrushesThemeKeys> { }
}
