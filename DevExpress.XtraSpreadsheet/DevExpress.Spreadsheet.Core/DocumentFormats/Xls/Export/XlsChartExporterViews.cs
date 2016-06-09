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
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.Office.Services;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.Utils.Zip;
using System.Xml;
using DevExpress.Office;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Export.Xls {
	public partial class XlsChartExporter {
		#region Fields
		const int minRotation = 0;
		const int maxRotation = 44;
		int[] viewLinesCrc = new int[4];
		#endregion
		#region Views
		void WriteChartViewFormat(bool varyColors) {
			XlsCommandChartViewFormat command = new XlsCommandChartViewFormat();
			command.VaryColors = varyColors;
			command.ZOrder = viewIndex;
			command.Write(StreamWriter);
		}
		void WriteBarChartView(int gapWidth, int overlap, BarChartGrouping grouping, BarChartDirection direction) {
			XlsCommandChartViewBar command = new XlsCommandChartViewBar();
			command.GapWidth = gapWidth;
			command.Overlap = overlap;
			command.PercentStacked = grouping == BarChartGrouping.PercentStacked;
			command.Stacked = grouping == BarChartGrouping.Stacked || grouping == BarChartGrouping.PercentStacked;
			command.Transpose = direction == BarChartDirection.Bar;
			command.Write(StreamWriter);
		}
		void WriteLineChartView(ChartGrouping grouping) {
			XlsCommandChartViewLine command = new XlsCommandChartViewLine();
			command.PercentStacked = grouping == ChartGrouping.PercentStacked;
			command.Stacked = grouping == ChartGrouping.Stacked || grouping == ChartGrouping.PercentStacked;
			command.Write(StreamWriter);
		}
		void WritePieChartView(int firstSliceAngle, int doughnutHoleSize, bool showLeaderLines) {
			XlsCommandChartViewPie command = new XlsCommandChartViewPie();
			command.FirstSliceAngle = firstSliceAngle;
			command.DoughnutHoleSize = doughnutHoleSize;
			command.ShowLeaderLines = showLeaderLines;
			command.Write(StreamWriter);
		}
		void WriteAreaChartView(ChartGrouping grouping) {
			XlsCommandChartViewArea command = new XlsCommandChartViewArea();
			command.PercentStacked = grouping == ChartGrouping.PercentStacked;
			command.Stacked = grouping == ChartGrouping.Stacked || grouping == ChartGrouping.PercentStacked;
			command.Write(StreamWriter);
		}
		void WriteScatterChartView() {
			XlsCommandChartViewScatter command = new XlsCommandChartViewScatter();
			command.IsBubble = false;
			command.Write(StreamWriter);
		}
		void WriteBubbleChartView(BubbleChartView view) {
			XlsCommandChartViewScatter command = new XlsCommandChartViewScatter();
			command.IsBubble = true;
			command.BubbleScale = view.BubbleScale;
			command.SizeRepresents = view.SizeRepresents;
			command.ShowNegBubbles = view.ShowNegBubbles;
			command.Write(StreamWriter);
		}
		void WriteRadarChartView() {
			XlsCommandChartViewRadar command = new XlsCommandChartViewRadar();
			command.ShowCatLabels = true;
			command.Write(StreamWriter);
		}
		void WriteRadarAreaChartView() {
			XlsCommandChartViewRadarArea command = new XlsCommandChartViewRadarArea();
			command.ShowCatLabels = true;
			command.Write(StreamWriter);
		}
		void WriteSurfaceChartView(bool wireframe) {
			XlsCommandChartViewSurface command = new XlsCommandChartViewSurface();
			command.Wireframe = wireframe;
			command.Shade3D = true;
			command.Write(StreamWriter);
		}
		void WriteOfPieChartView(OfPieChartView view) {
			XlsCommandChartViewOfPie command = new XlsCommandChartViewOfPie();
			command.OfPieType = view.OfPieType;
			command.SplitType = view.SplitType;
			command.SplitPos = view.SplitPos;
			command.SecondPieSize = view.SecondPieSize;
			command.GapWidth = view.GapWidth;
			command.Write(StreamWriter);
		}
		void WriteOfPieChartViewCustom(OfPieChartView view) {
			XlsCommandChartViewOfPieCustom command = new XlsCommandChartViewOfPieCustom();
			int numberOfPoints = 0;
			if (view.Series.Count > 0) {
				PieSeries series = view.Series[0] as PieSeries;
				numberOfPoints = (int)series.Values.ValuesCount;
			}
			command.NumberOfPoints = numberOfPoints;
			command.SecondPiePoints.AddRange(view.SecondPiePoints.InnerList);
			command.Write(StreamWriter);
		}
		void WriteCrtLink() {
			commandCrtLink.Write(StreamWriter);
		}
		void WriteView3D(IChartView view) {
			XlsCommandChartView3D command = new XlsCommandChartView3D();
			if (view.ViewType == ChartViewType.Surface) {
				command.XRotation = 90;
				command.YRotation = 0;
				command.NotPieChart = true;
				command.Height = 34;
				command.Depth = 100;
				command.Perspective = 0;
				command.GapDepth = 150;
				command.AutoHeight = true;
				command.HasPerspective = true;
				command.Clustered = false;
			}
			else {
				View3DOptions view3D = Chart.View3D;
				int xRotation = view3D.XRotation;
				int yRotation = view3D.YRotation % 360;
				bool rightAngleAxisOff = !view3D.RightAngleAxes;
				bool isPie3d = view.ViewType == ChartViewType.Pie3D;
				if (IsBar3DWithBarDirection(view)) {
					command.XRotation = GetBar3dXRotation(xRotation);
					command.YRotation = GetBar3dYRotation(yRotation);
					command.AutoHeight = rightAngleAxisOff && view3D.AutoHeight;
					command.HasPerspective = false;
				}
				else {
					command.XRotation = xRotation;
					command.YRotation = yRotation;
					command.AutoHeight = isPie3d ? false : rightAngleAxisOff;
					command.HasPerspective = isPie3d ? false : rightAngleAxisOff;
				}
				command.NotPieChart = !isPie3d;
				command.Height = view3D.HeightPercent;
				command.Depth = view3D.DepthPercent;
				command.Perspective = view3D.Perspective;
				IChartViewWithGapDepth viewWithGapDepth = view as IChartViewWithGapDepth;
				command.GapDepth = viewWithGapDepth != null ? viewWithGapDepth.GapDepth : 150;
				Bar3DChartView bar3DChartView = view as Bar3DChartView;
				command.Clustered = bar3DChartView != null ? (bar3DChartView.Grouping == BarChartGrouping.Clustered) : false;
			}
			command.Write(StreamWriter);
		}
		#region ViewLines
		void WriteViewLines(IChartView view) {
			WriteViewDropLines(view as ISupportsDropLines);
			WriteViewHiLowLines(view as ISupportsHiLowLines);
			WriteViewSeriesLines(view as ISupportsSeriesLines);
			WriteViewLeaderLines(view as ChartViewWithDataLabels);
		}
		void WriteViewDropLines(ISupportsDropLines view) {
			if (view == null || !view.ShowDropLines)
				return;
			XlsCommandChartCrtLine command = new XlsCommandChartCrtLine();
			command.Value = XlsChartDefs.DropLinesIndex;
			command.Write(StreamWriter);
			ResetCrc32();
			XlsChartLineFormatExportHelper helper = new XlsChartLineFormatExportHelper(this, view.DropLinesProperties);
			helper.Element = ChartElement.OtherLines;
			helper.WriteContent();
			viewLinesCrc[XlsChartDefs.DropLinesIndex] = Crc32.CrcValue;
		}
		void WriteViewHiLowLines(ISupportsHiLowLines view) {
			if (view == null || !view.ShowHiLowLines)
				return;
			XlsCommandChartCrtLine command = new XlsCommandChartCrtLine();
			command.Value = XlsChartDefs.HiLowLinesIndex;
			command.Write(StreamWriter);
			ResetCrc32();
			XlsChartLineFormatExportHelper helper = new XlsChartLineFormatExportHelper(this, view.HiLowLinesProperties);
			helper.Element = ChartElement.OtherLines;
			helper.WriteContent();
			viewLinesCrc[XlsChartDefs.HiLowLinesIndex] = Crc32.CrcValue;
		}
		void WriteViewSeriesLines(ISupportsSeriesLines view) {
			if (view == null || !view.IsSeriesLinesApplicable || view.SeriesLines.Count == 0)
				return;
			XlsCommandChartCrtLine command = new XlsCommandChartCrtLine();
			command.Value = XlsChartDefs.SeriesLinesIndex;
			command.Write(StreamWriter);
			ResetCrc32();
			XlsChartLineFormatExportHelper helper = new XlsChartLineFormatExportHelper(this, view.SeriesLines[0]);
			helper.Element = ChartElement.OtherLines;
			helper.WriteContent();
			viewLinesCrc[XlsChartDefs.SeriesLinesIndex] = Crc32.CrcValue;
		}
		void WriteViewLeaderLines(ChartViewWithDataLabels view) {
			if (view == null || !view.DataLabels.IsVisible || !view.DataLabels.ShowLeaderLines)
				return;
			XlsCommandChartCrtLine command = new XlsCommandChartCrtLine();
			command.Value = XlsChartDefs.LeaderLinesIndex;
			command.Write(StreamWriter);
			ResetCrc32();
			XlsChartLineFormatExportHelper helper = new XlsChartLineFormatExportHelper(this, view.DataLabels.LeaderLinesProperties);
			helper.Element = ChartElement.OtherLines;
			helper.WriteContent();
			viewLinesCrc[XlsChartDefs.LeaderLinesIndex] = Crc32.CrcValue;
		}
		#endregion
		#region ViewShapeFormats
		void WriteViewShapeFormats(IChartView view) {
			WriteViewShapeDropLines(view as ISupportsDropLines);
			WriteViewShapeHiLowLines(view as ISupportsHiLowLines);
			WriteViewShapeSeriesLines(view as ISupportsSeriesLines);
			WriteViewShapeLeaderLines(view as ChartViewWithDataLabels);
		}
		void WriteViewShapeDropLines(ISupportsDropLines view) {
			if (view == null || !view.ShowDropLines)
				return;
			Crc32.CrcValue = viewLinesCrc[XlsChartDefs.DropLinesIndex];
			WriteChartGroupStartBlock();
			WriteShapeFormat(view.DropLinesProperties, XlsChartDefs.DropLinesContext);
		}
		void WriteViewShapeHiLowLines(ISupportsHiLowLines view) {
			if (view == null || !view.ShowHiLowLines)
				return;
			Crc32.CrcValue = viewLinesCrc[XlsChartDefs.HiLowLinesIndex];
			WriteChartGroupStartBlock();
			WriteShapeFormat(view.HiLowLinesProperties, XlsChartDefs.HiLowLinesContext);
		}
		void WriteViewShapeSeriesLines(ISupportsSeriesLines view) {
			if (view == null || !view.IsSeriesLinesApplicable || view.SeriesLines.Count == 0)
				return;
			Crc32.CrcValue = viewLinesCrc[XlsChartDefs.SeriesLinesIndex];
			WriteChartGroupStartBlock();
			WriteShapeFormat(view.SeriesLines[0], XlsChartDefs.SeriesLinesContext);
		}
		void WriteViewShapeLeaderLines(ChartViewWithDataLabels view) {
			if (view == null || !view.DataLabels.IsVisible || !view.DataLabels.ShowLeaderLines)
				return;
			Crc32.CrcValue = viewLinesCrc[XlsChartDefs.LeaderLinesIndex];
			WriteChartGroupStartBlock();
			WriteShapeFormat(view.DataLabels.LeaderLinesProperties, XlsChartDefs.LeaderLinesContext);
		}
		#endregion
		#endregion
		#region IChartViewVisitor Members
		public void Visit(Area3DChartView view) {
			WriteChartViewFormat(view.VaryColors);
			WriteBegin();
			WriteAreaChartView(view.Grouping);
			WriteCrtLink();
			WriteView3D(view);
			WriteLegend();
			WriteViewLines(view);
			WriteDataLabExtContents(view.DataLabels, view.ViewType, true);
			WriteViewShapeFormats(view);
			WriteView3DExtProperties(view);
			WriteChartGroupEndBlock();
			WriteEnd();
		}
		public void Visit(AreaChartView view) {
			WriteChartViewFormat(view.VaryColors);
			WriteBegin();
			WriteAreaChartView(view.Grouping);
			WriteCrtLink();
			WriteLegend();
			WriteViewLines(view);
			WriteDataLabExtContents(view.DataLabels, view.ViewType, true);
			WriteViewShapeFormats(view);
			WriteChartGroupEndBlock();
			WriteEnd();
		}
		public void Visit(Bar3DChartView view) {
			WriteChartViewFormat(view.VaryColors);
			WriteBegin();
			WriteBarChartView(view.GapWidth, 0, view.Grouping, view.BarDirection);
			WriteCrtLink();
			WriteView3D(view);
			WriteLegend();
			WriteViewLines(view);
			WriteViewShapeFormats(view);
			WriteView3DExtProperties(view);
			WriteChartGroupEndBlock();
			WriteEnd();
		}
		public void Visit(BarChartView view) {
			WriteChartViewFormat(view.VaryColors);
			WriteBegin();
			WriteBarChartView(view.GapWidth, view.Overlap, view.Grouping, view.BarDirection);
			WriteCrtLink();
			WriteLegend();
			WriteViewLines(view);
			WriteViewShapeFormats(view);
			WriteChartGroupEndBlock();
			WriteEnd();
		}
		public void Visit(BubbleChartView view) {
			WriteChartViewFormat(view.VaryColors);
			WriteBegin();
			WriteBubbleChartView(view);
			WriteCrtLink();
			WriteLegend();
			WriteViewLines(view);
			WriteViewShapeFormats(view);
			WriteChartGroupEndBlock();
			WriteEnd();
		}
		public void Visit(DoughnutChartView view) {
			WriteChartViewFormat(view.VaryColors);
			WriteBegin();
			WritePieChartView(view.FirstSliceAngle, view.HoleSize, view.DataLabels.ShowLeaderLines);
			WriteCrtLink();
			WriteLegend();
			WriteViewLines(view);
			WriteViewShapeFormats(view);
			WriteChartGroupEndBlock();
			WriteEnd();
		}
		public void Visit(Line3DChartView view) {
			WriteChartViewFormat(view.VaryColors);
			WriteBegin();
			WriteLineChartView(view.Grouping);
			WriteCrtLink();
			WriteView3D(view);
			WriteLegend();
			WriteViewLines(view);
			WriteViewShapeFormats(view);
			WriteView3DExtProperties(view);
			WriteChartGroupEndBlock();
			WriteEnd();
		}
		public void Visit(LineChartView view) {
			WriteChartViewFormat(view.VaryColors);
			WriteBegin();
			WriteLineChartView(view.Grouping);
			WriteCrtLink();
			WriteLegend();
			WriteViewLines(view);
			WriteViewShapeFormats(view);
			WriteChartGroupEndBlock();
			WriteEnd();
		}
		public void Visit(OfPieChartView view) {
			WriteChartViewFormat(view.VaryColors);
			WriteBegin();
			WriteOfPieChartView(view);
			if (view.SplitType == OfPieSplitType.Custom)
				WriteOfPieChartViewCustom(view);
			WriteCrtLink();
			WriteLegend();
			WriteViewLines(view);
			WriteViewShapeFormats(view);
			WriteChartGroupEndBlock();
			WriteEnd();
		}
		public void Visit(Pie3DChartView view) {
			WriteChartViewFormat(view.VaryColors);
			WriteBegin();
			WritePieChartView(0, 0, view.DataLabels.ShowLeaderLines);
			WriteCrtLink();
			WriteView3D(view);
			WriteLegend();
			WriteViewLines(view);
			WriteViewShapeFormats(view);
			WriteView3DExtProperties(view);
			WriteChartGroupEndBlock();
			WriteEnd();
		}
		public void Visit(PieChartView view) {
			WriteChartViewFormat(view.VaryColors);
			WriteBegin();
			WritePieChartView(view.FirstSliceAngle, 0, view.DataLabels.ShowLeaderLines);
			WriteCrtLink();
			WriteLegend();
			WriteViewLines(view);
			WriteViewShapeFormats(view);
			WriteChartGroupEndBlock();
			WriteEnd();
		}
		public void Visit(RadarChartView view) {
			bool isFilledRadar = view.RadarStyle == RadarChartStyle.Filled;
			WriteChartViewFormat(view.VaryColors);
			WriteBegin();
			if (isFilledRadar)
				WriteRadarAreaChartView();
			else
				WriteRadarChartView();
			WriteCrtLink();
			WriteLegend();
			WriteViewLines(view);
			if (isFilledRadar)
				WriteDataLabExtContents(view.DataLabels, view.ViewType, true);
			WriteViewShapeFormats(view);
			WriteChartGroupEndBlock();
			WriteEnd();
		}
		public void Visit(ScatterChartView view) {
			WriteChartViewFormat(view.VaryColors);
			WriteBegin();
			WriteScatterChartView();
			WriteCrtLink();
			WriteLegend();
			WriteViewLines(view);
			WriteViewShapeFormats(view);
			WriteChartGroupEndBlock();
			WriteEnd();
		}
		public void Visit(StockChartView view) {
			WriteChartViewFormat(false);
			WriteBegin();
			WriteLineChartView(ChartGrouping.Standard);
			WriteCrtLink();
			WriteLegend();
			WriteViewLines(view);
			WriteViewShapeFormats(view);
			WriteChartGroupEndBlock();
			WriteEnd();
		}
		public void Visit(Surface3DChartView view) {
			WriteChartViewFormat(false);
			WriteBegin();
			WriteSurfaceChartView(view.Wireframe);
			WriteCrtLink();
			WriteView3D(view);
			WriteLegend();
			WriteViewLines(view);
			WriteViewShapeFormats(view);
			WriteView3DExtProperties(view);
			WriteChartGroupEndBlock();
			WriteEnd();
		}
		public void Visit(SurfaceChartView view) {
			WriteChartViewFormat(false);
			WriteBegin();
			WriteSurfaceChartView(view.Wireframe);
			WriteCrtLink();
			WriteView3D(view);
			WriteLegend();
			WriteViewLines(view);
			WriteViewShapeFormats(view);
			WriteChartGroupEndBlock();
			WriteEnd();
		}
		#endregion
		#region WriteView3DExtProperties
		void WriteView3DExtProperties(IChartView view) {
			XlsChartExtProperties extProperties = new XlsChartExtProperties();
			extProperties.Parent = XlsChartExtPropParent.View3D;
			ChartViewType viewType = view.ViewType;
			View3DOptions view3d = Chart.View3D;
			bool rightAngleAxisOff = !view3d.RightAngleAxes;
			bool isBar3DWithBarDirection = IsBar3DWithBarDirection(view);
			if (isBar3DWithBarDirection) {
				extProperties.Items.Add(new XlsChartExtPropRightAngAxOff() { Value = rightAngleAxisOff });
				if (rightAngleAxisOff)
					extProperties.Items.Add(new XlsChartExtPropPerspective() { Value = view3d.Perspective });
				int yRotation = view3d.YRotation % 360;
				if (!IsBar3dCompliant(yRotation))
					extProperties.Items.Add(new XlsChartExtPropRotationY() { Value = yRotation });
				int xRotation = view3d.XRotation;
				if (!IsBar3dCompliant(xRotation))
					extProperties.Items.Add(new XlsChartExtPropRotationX() { Value = xRotation });
			}
			if (viewType == ChartViewType.Pie3D && rightAngleAxisOff)
				extProperties.Items.Add(new XlsChartExtPropPerspective() { Value = view3d.Perspective });
			if (viewType != ChartViewType.Pie3D && view3d.AutoHeight)
				extProperties.Items.Add(new XlsChartExtPropHeightPercent() { Value = view3d.HeightPercent });
			if (extProperties.Items.Count > 0)
				WriteChartGroupStartBlock();
			WriteExtProperties(extProperties);
		}
		bool IsBar3DWithBarDirection(IChartView view) {
			Bar3DChartView bar3dChartView = view as Bar3DChartView;
			return bar3dChartView != null ? bar3dChartView.BarDirection == BarChartDirection.Bar : false;
		}
		int GetBar3dXRotation(int modelXRotation) {
			if (modelXRotation > maxRotation)
				return maxRotation;
			if (modelXRotation < minRotation)
				return minRotation;
			return modelXRotation;
		}
		int GetBar3dYRotation(int modelYRotation) {
			return IsBar3dCompliant(modelYRotation) ? modelYRotation : maxRotation;
		}
		bool IsBar3dCompliant(int rotation) {
			return rotation >= minRotation && rotation <= maxRotation;
		}
		#endregion
	}
}
