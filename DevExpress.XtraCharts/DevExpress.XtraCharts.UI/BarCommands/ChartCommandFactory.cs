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
using System.Linq;
using System.Text;
using System.Reflection;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Commands {
	public class ChartCommandFactory {
		static readonly Type[] constructorParametersInterface = new Type[] { typeof(IChartContainer) };
		IChartContainer chartControl;
		Dictionary<ChartCommandId, ConstructorInfo> commandConstructorTable = new Dictionary<ChartCommandId, ConstructorInfo>();
		public ChartCommandFactory(IChartContainer chartControl) {
			this.chartControl = chartControl;
			PopulateConstructorTable();
		}
		void AddConstructor(ChartCommandId commandId, Type commandType) {
			ConstructorInfo constructorInfo = commandType.GetConstructor(constructorParametersInterface);
			if (constructorInfo != null)
				commandConstructorTable.Add(commandId, constructorInfo);
		}
		void PopulateConstructorTable() {
			AddConstructor(ChartCommandId.CreateBarChart, typeof(CreateBarChartCommand));
			AddConstructor(ChartCommandId.CreateBar3DChart, typeof(CreateBar3DChartCommand));
			AddConstructor(ChartCommandId.CreateFullStackedBarChart, typeof(CreateFullStackedBarChartCommand));
			AddConstructor(ChartCommandId.CreateFullStackedBar3DChart, typeof(CreateFullStackedBar3DChartCommand));
			AddConstructor(ChartCommandId.CreateManhattanBarChart, typeof(CreateManhattanBarChartCommand));
			AddConstructor(ChartCommandId.CreateSideBySideFullStackedBarChart, typeof(CreateSideBySideFullStackedBarChartCommand));
			AddConstructor(ChartCommandId.CreateSideBySideFullStackedBar3DChart, typeof(CreateSideBySideFullStackedBar3DChartCommand));
			AddConstructor(ChartCommandId.CreateSideBySideStackedBarChart, typeof(CreateSideBySideStackedBarChartCommand));
			AddConstructor(ChartCommandId.CreateSideBySideStackedBar3DChart, typeof(CreateSideBySideStackedBar3DChartCommand));
			AddConstructor(ChartCommandId.CreateStackedBarChart, typeof(CreateStackedBarChartCommand));
			AddConstructor(ChartCommandId.CreateStackedBar3DChart, typeof(CreateStackedBar3DChartCommand));
			AddConstructor(ChartCommandId.CreateBarChartPlaceHolder, typeof(CreateBarChartPlaceHolderCommand));
			AddConstructor(ChartCommandId.CreatePieChart, typeof(CreatePieChartCommand));
			AddConstructor(ChartCommandId.CreatePie3DChart, typeof(CreatePie3DChartCommand));
			AddConstructor(ChartCommandId.CreateDoughnutChart, typeof(CreateDoughnutChartCommand));
			AddConstructor(ChartCommandId.CreateNestedDoughnutChart, typeof(CreateNestedDoughnutChartCommand));
			AddConstructor(ChartCommandId.CreateDoughnut3DChart, typeof(CreateDoughnut3DChartCommand));
			AddConstructor(ChartCommandId.CreatePieChartPlaceHolder, typeof(CreatePieChartPlaceHolderCommand));
			AddConstructor(ChartCommandId.CreateAreaChart, typeof(CreateAreaChartCommand));
			AddConstructor(ChartCommandId.CreateArea3DChart, typeof(CreateArea3DChartCommand));
			AddConstructor(ChartCommandId.CreateFullStackedAreaChart, typeof(CreateFullStackedAreaChartCommand));
			AddConstructor(ChartCommandId.CreateFullStackedArea3DChart, typeof(CreateFullStackedArea3DChartCommand));
			AddConstructor(ChartCommandId.CreateFullStackedSplineAreaChart, typeof(CreateFullStackedSplineAreaChartCommand));
			AddConstructor(ChartCommandId.CreateFullStackedSplineArea3DChart, typeof(CreateFullStackedSplineArea3DChartCommand));
			AddConstructor(ChartCommandId.CreateSplineAreaChart, typeof(CreateSplineAreaChartCommand));
			AddConstructor(ChartCommandId.CreateSplineArea3DChart, typeof(CreateSplineArea3DChartCommand));
			AddConstructor(ChartCommandId.CreateStackedAreaChart, typeof(CreateStackedAreaChartCommand));
			AddConstructor(ChartCommandId.CreateStackedArea3DChart, typeof(CreateStackedArea3DChartCommand));
			AddConstructor(ChartCommandId.CreateStackedSplineAreaChart, typeof(CreateStackedSplineAreaChartCommand));
			AddConstructor(ChartCommandId.CreateStackedSplineArea3DChart, typeof(CreateStackedSplineArea3DChartCommand));
			AddConstructor(ChartCommandId.CreateStepAreaChart, typeof(CreateStepAreaChartCommand));
			AddConstructor(ChartCommandId.CreateStepArea3DChart, typeof(CreateStepArea3DChartCommand));
			AddConstructor(ChartCommandId.CreateAreaChartPlaceHolder, typeof(CreateAreaChartPlaceHolderCommand));
			AddConstructor(ChartCommandId.CreateLineChart, typeof(CreateLineChartCommand));
			AddConstructor(ChartCommandId.CreateLine3DChart, typeof(CreateLine3DChartCommand));
			AddConstructor(ChartCommandId.CreateFullStackedLineChart, typeof(CreateFullStackedLineChartCommand));
			AddConstructor(ChartCommandId.CreateFullStackedLine3DChart, typeof(CreateFullStackedLine3DChartCommand));
			AddConstructor(ChartCommandId.CreateScatterLineChart, typeof(CreateScatterLineChartCommand));
			AddConstructor(ChartCommandId.CreateSplineChart, typeof(CreateSplineChartCommand));
			AddConstructor(ChartCommandId.CreateSpline3DChart, typeof(CreateSpline3DChartCommand));
			AddConstructor(ChartCommandId.CreateStackedLineChart, typeof(CreateStackedLineChartCommand));
			AddConstructor(ChartCommandId.CreateStackedLine3DChart, typeof(CreateStackedLine3DChartCommand));
			AddConstructor(ChartCommandId.CreateStepLineChart, typeof(CreateStepLineChartCommand));
			AddConstructor(ChartCommandId.CreateStepLine3DChart, typeof(CreateStepLine3DChartCommand));
			AddConstructor(ChartCommandId.CreateLineChartPlaceHolder, typeof(CreateLineChartPlaceHolderCommand));
			AddConstructor(ChartCommandId.CreatePointChart, typeof(CreatePointChartCommand));
			AddConstructor(ChartCommandId.CreateBubbleChart, typeof(CreateBubbleChartCommand));
			AddConstructor(ChartCommandId.CreateFunnelChart, typeof(CreateFunnelChartCommand));
			AddConstructor(ChartCommandId.CreateFunnel3DChart, typeof(CreateFunnel3DChartCommand));
			AddConstructor(ChartCommandId.CreateRangeBarChart, typeof(CreateRangeBarChartCommand));
			AddConstructor(ChartCommandId.CreateSideBySideRangeBarChart, typeof(CreateSideBySideRangeBarChartCommand));
			AddConstructor(ChartCommandId.CreateRangeAreaChart, typeof(CreateRangeAreaChartCommand));
			AddConstructor(ChartCommandId.CreateRangeArea3DChart, typeof(CreateRangeArea3DChartCommand));
			AddConstructor(ChartCommandId.CreateRadarPointChart, typeof(CreateRadarPointChartCommand));
			AddConstructor(ChartCommandId.CreateRadarLineChart, typeof(CreateRadarLineChartCommand));
			AddConstructor(ChartCommandId.CreateScatterRadarLineChart, typeof(CreateScatterRadarLineChartCommand));
			AddConstructor(ChartCommandId.CreateRadarAreaChart, typeof(CreateRadarAreaChartCommand));
			AddConstructor(ChartCommandId.CreatePolarPointChart, typeof(CreatePolarPointChartCommand));
			AddConstructor(ChartCommandId.CreatePolarLineChart, typeof(CreatePolarLineChartCommand));
			AddConstructor(ChartCommandId.CreateScatterPolarLineChart, typeof(CreateScatterPolarLineChartCommand));
			AddConstructor(ChartCommandId.CreatePolarAreaChart, typeof(CreatePolarAreaChartCommand));
			AddConstructor(ChartCommandId.CreateStockChart, typeof(CreateStockChartCommand));
			AddConstructor(ChartCommandId.CreateCandleStickChart, typeof(CreateCandleStickChartCommand));
			AddConstructor(ChartCommandId.CreateGanttChart, typeof(CreateGanttChartCommand));
			AddConstructor(ChartCommandId.CreateSideBySideGanttChart, typeof(CreateSideBySideGanttChartCommand));
			AddConstructor(ChartCommandId.CreateOtherSeriesTypesChartPlaceHolder, typeof(CreateOtherSeriesTypesChartPlaceHolderCommand));
			AddConstructor(ChartCommandId.CreateRotatedBarChart, typeof(CreateRotatedBarChartCommand));
			AddConstructor(ChartCommandId.CreateRotatedFullStackedBarChart, typeof(CreateRotatedFullStackedBarChartCommand));
			AddConstructor(ChartCommandId.CreateRotatedSideBySideFullStackedBarChart, typeof(CreateRotatedSideBySideFullStackedBarChartCommand));
			AddConstructor(ChartCommandId.CreateRotatedSideBySideStackedBarChart, typeof(CreateRotatedSideBySideStackedBarChartCommand));
			AddConstructor(ChartCommandId.CreateRotatedStackedBarChart, typeof(CreateRotatedStackedBarChartCommand));
			AddConstructor(ChartCommandId.CreateRotatedBarChartPlaceHolder, typeof(CreateRotatedBarChartPlaceHolderCommand));
			AddConstructor(ChartCommandId.ChangeAppearance, typeof(ChangeAppearanceCommand));
			AddConstructor(ChartCommandId.ChangePalette, typeof(ChangePaletteCommand));
			AddConstructor(ChartCommandId.ChangePalettePlaceHolder, typeof(ChangePalettePlaceHolderCommand));
			AddConstructor(ChartCommandId.RunWizard, typeof(RunWizardCommand));
			AddConstructor(ChartCommandId.RunDesigner, typeof(RunDesignerCommand));
			AddConstructor(ChartCommandId.SaveAsTemplate, typeof(SaveAsTemplateCommand));
			AddConstructor(ChartCommandId.LoadTemplate, typeof(LoadTemplateCommand));
			AddConstructor(ChartCommandId.PrintPreview, typeof(PrintPreviewCommand));
			AddConstructor(ChartCommandId.Print, typeof(PrintCommand));
			AddConstructor(ChartCommandId.ExportPlaceHolder, typeof(ExportPlaceHolderCommand));
			AddConstructor(ChartCommandId.ExportToPDF, typeof(ExportToPDFCommand));
			AddConstructor(ChartCommandId.ExportToHTML, typeof(ExportToHTMLCommand));
			AddConstructor(ChartCommandId.ExportToMHT, typeof(ExportToMHTCommand));
			AddConstructor(ChartCommandId.ExportToXLS, typeof(ExportToXLSCommand));
			AddConstructor(ChartCommandId.ExportToXLSX, typeof(ExportToXLSXCommand));
			AddConstructor(ChartCommandId.ExportToRTF, typeof(ExportToRTFCommand));
			AddConstructor(ChartCommandId.ExportToImagePlaceHolder, typeof(ExportToImagePlaceHolderCommand));
			AddConstructor(ChartCommandId.ExportToBMP, typeof(ExportToBMPCommand));
			AddConstructor(ChartCommandId.ExportToGIF, typeof(ExportToGIFCommand));
			AddConstructor(ChartCommandId.ExportToJPEG, typeof(ExportToJPEGCommand));
			AddConstructor(ChartCommandId.ExportToPNG, typeof(ExportToPNGCommand));
			AddConstructor(ChartCommandId.ExportToTIFF, typeof(ExportToTIFFCommand));
			AddConstructor(ChartCommandId.Column2DGroupPlaceHolder, typeof(Column2DGroupPlaceHolderCommand));
			AddConstructor(ChartCommandId.Column3DGroupPlaceHolder, typeof(Column3DGroupPlaceHolderCommand));
			AddConstructor(ChartCommandId.ColumnConeGroupPlaceHolder, typeof(ColumnConeGroupPlaceHolderCommand));
			AddConstructor(ChartCommandId.ColumnCylinderGroupPlaceHolder, typeof(ColumnCylinderGroupPlaceHolderCommand));
			AddConstructor(ChartCommandId.ColumnPyramidGroupPlaceHolder, typeof(ColumnPyramidGroupPlaceHolderCommand));
			AddConstructor(ChartCommandId.Line2DGroupPlaceHolder, typeof(Line2DGroupPlaceHolderCommand));
			AddConstructor(ChartCommandId.Line3DGroupPlaceHolder, typeof(Line3DGroupPlaceHolderCommand));
			AddConstructor(ChartCommandId.Pie2DGroupPlaceHolder, typeof(Pie2DGroupPlaceHolderCommand));
			AddConstructor(ChartCommandId.Pie3DGroupPlaceHolder, typeof(Pie3DGroupPlaceHolderCommand));
			AddConstructor(ChartCommandId.Bar2DGroupPlaceHolder, typeof(Bar2DGroupPlaceHolderCommand));
			AddConstructor(ChartCommandId.Area2DGroupPlaceHolder, typeof(Area2DGroupPlaceHolderCommand));
			AddConstructor(ChartCommandId.Area3DGroupPlaceHolder, typeof(Area3DGroupPlaceHolderCommand));
			AddConstructor(ChartCommandId.PointGroupPlaceHolder, typeof(PointGroupPlaceHolderCommand));
			AddConstructor(ChartCommandId.FunnelGroupPlaceHolder, typeof(FunnelGroupPlaceHolderCommand));
			AddConstructor(ChartCommandId.FinancialGroupPlaceHolder, typeof(FinancialGroupPlaceHolderCommand));
			AddConstructor(ChartCommandId.RadarGroupPlaceHolder, typeof(RadarGroupPlaceHolderCommand));
			AddConstructor(ChartCommandId.PolarGroupPlaceHolder, typeof(PolarGroupPlaceHolderCommand));
			AddConstructor(ChartCommandId.RangeGroupPlaceHolder, typeof(RangeGroupPlaceHolderCommand));
			AddConstructor(ChartCommandId.GanttGroupPlaceHolder, typeof(GanttGroupPlaceHolderCommand));
			AddConstructor(ChartCommandId.CreateConeBar3DChart, typeof(CreateConeBar3DChartCommand));
			AddConstructor(ChartCommandId.CreateConeFullStackedBar3DChart, typeof(CreateConeFullStackedBar3DChartCommand));
			AddConstructor(ChartCommandId.CreateConeManhattanBarChart, typeof(CreateConeManhattanBarChartCommand));
			AddConstructor(ChartCommandId.CreateConeSideBySideFullStackedBar3DChart, typeof(CreateConeSideBySideFullStackedBar3DChartCommand));
			AddConstructor(ChartCommandId.CreateConeSideBySideStackedBar3DChart, typeof(CreateConeSideBySideStackedBar3DChartCommand));
			AddConstructor(ChartCommandId.CreateConeStackedBar3DChart, typeof(CreateConeStackedBar3DChartCommand));
			AddConstructor(ChartCommandId.CreatePyramidBar3DChart, typeof(CreatePyramidBar3DChartCommand));
			AddConstructor(ChartCommandId.CreatePyramidFullStackedBar3DChart, typeof(CreatePyramidFullStackedBar3DChartCommand));
			AddConstructor(ChartCommandId.CreatePyramidManhattanBarChart, typeof(CreatePyramidManhattanBarChartCommand));
			AddConstructor(ChartCommandId.CreatePyramidSideBySideFullStackedBar3DChart, typeof(CreatePyramidSideBySideFullStackedBar3DChartCommand));
			AddConstructor(ChartCommandId.CreatePyramidSideBySideStackedBar3DChart, typeof(CreatePyramidSideBySideStackedBar3DChartCommand));
			AddConstructor(ChartCommandId.CreatePyramidStackedBar3DChart, typeof(CreatePyramidStackedBar3DChartCommand));
			AddConstructor(ChartCommandId.CreateCylinderBar3DChart, typeof(CreateCylinderBar3DChartCommand));
			AddConstructor(ChartCommandId.CreateCylinderFullStackedBar3DChart, typeof(CreateCylinderFullStackedBar3DChartCommand));
			AddConstructor(ChartCommandId.CreateCylinderManhattanBarChart, typeof(CreateCylinderManhattanBarChartCommand));
			AddConstructor(ChartCommandId.CreateCylinderSideBySideFullStackedBar3DChart, typeof(CreateCylinderSideBySideFullStackedBar3DChartCommand));
			AddConstructor(ChartCommandId.CreateCylinderSideBySideStackedBar3DChart, typeof(CreateCylinderSideBySideStackedBar3DChartCommand));
			AddConstructor(ChartCommandId.CreateCylinderStackedBar3DChart, typeof(CreateCylinderStackedBar3DChartCommand));
			AddConstructor(ChartCommandId.ChangeAppearancePlaceHolder, typeof(ChangeAppearancePlaceHolderCommand));
		}
		public ChartCommand CreateCommand(ChartCommandId commandId) {
			ConstructorInfo constructorInfo;
			if (commandConstructorTable.TryGetValue(commandId, out constructorInfo))
				return (ChartCommand)constructorInfo.Invoke(new object[] { chartControl });
			else
				return null;
		}
	}
}
