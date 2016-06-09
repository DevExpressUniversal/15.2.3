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
using System.Globalization;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Utils;
using ChartsModel = DevExpress.Charts.Model;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.Office.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model.ModelChart {
	public class ModelChartBuilder : IChartViewVisitor {
		#region Fields
		Chart spreadsheetChart;
		ChartsModel.Chart modelChart;
		#endregion
		public ModelChartBuilder(Chart chart) {
			this.spreadsheetChart = chart;
		}
		public ChartsModel.Chart ModelChart { get { return modelChart; } }
		public Chart SpreadsheetChart { get { return spreadsheetChart; } }
		public int SeriesIndex { get; set; }
		public ChartsModel.Chart BuildModelChart() {
			CreateModelChart();
			CreateViews();
			CreateLegend();
			CreateTitle();
			return modelChart;
		}
		void CreateModelChart() {
			this.modelChart = null;
			if(this.spreadsheetChart.Views.Count > 0)
				this.spreadsheetChart.Views[0].Visit(this);
		}
		void CreateViews() {
			if (this.modelChart == null)
				return;
			ModelViewBuilderFactory factory = new ModelViewBuilderFactory(this);
			foreach (IChartView view in SpreadsheetChart.Views) {
				IModelViewBuilder viewBuilder = factory.CreateViewBuilder(view);
				if(viewBuilder != null)
					viewBuilder.Build();
			}
		}
		void CreateLegend() {
			if (ModelChart == null)
				return;
			Legend legend = spreadsheetChart.Legend;
			if (legend.Visible) {
				ChartsModel.Legend modelLegend = new ChartsModel.Legend();
				modelLegend.EnableAntialiasing = spreadsheetChart.GetActualTextAntialiasing();
				modelLegend.LegendPosition = GetLegendPosition(legend.Position);
				modelLegend.Orientation = GetLegendOrientation(legend.Position);
				modelLegend.Overlay = legend.Overlay;
				if (legend.ShapeProperties.OutlineType != OutlineType.None) {
					DocumentModel documentModel = legend.DocumentModel;
					Color color = legend.ShapeProperties.OutlineColor.FinalColor;
					modelLegend.Border = new ChartsModel.Border(modelLegend);
					modelLegend.Border.Color = new ChartsModel.ColorARGB(color.A, color.R, color.G, color.B);
					modelLegend.Border.Thickness = Math.Max(1, (int)documentModel.UnitConverter.ModelUnitsToPixelsF(legend.ShapeProperties.Outline.Width, DocumentModel.Dpi));
				}
				this.modelChart.Legend = modelLegend;
			}
		}
		void CreateTitle() {
			if (ModelChart == null || !SpreadsheetChart.Title.Visible)
				return;
			ChartsModel.ChartTitle title = new ChartsModel.ChartTitle();
			title.EnableAntialiasing = spreadsheetChart.GetActualTextAntialiasing();
			title.Lines = SpreadsheetChart.Title.Text.Lines;
			this.modelChart.Titles.Add(title);
		}
		void SetupView3D(ChartsModel.IOptions3D options) {
			View3DOptions view3D = spreadsheetChart.View3D;
			options.RotationAngleX = view3D.XRotation;
			options.RotationAngleY = -view3D.YRotation;
			options.RotationAngleZ = 0;
			options.PerspectiveAngle = view3D.RightAngleAxes ? 0 : view3D.Perspective / 2;
			options.EnableAntialiasing = spreadsheetChart.GetActualAntialiasing();
		}
		void SetupPieView3D(ChartsModel.IOptions3D options) {
			View3DOptions view3D = spreadsheetChart.View3D;
			options.RotationAngleX = 270 + view3D.XRotation;
			options.RotationAngleY = 0;
			options.RotationAngleZ = 90 - view3D.YRotation;
			options.PerspectiveAngle = view3D.RightAngleAxes ? 0 : view3D.Perspective / 2;
			options.EnableAntialiasing = spreadsheetChart.GetActualAntialiasing();
		}
		#region IChartViewVisitor Members
		public void Visit(Area3DChartView view) {
			ChartsModel.Cartesian3DChart chart = new ChartsModel.Cartesian3DChart();
			SetupView3D(chart.Options3D);
			CreateCatValAxes(chart);
			this.modelChart = chart;
		}
		public void Visit(AreaChartView view) {
			ChartsModel.CartesianChart chart = new ChartsModel.CartesianChart();
			CreateCatValAxes(chart);
			this.modelChart = chart;
		}
		public void Visit(Bar3DChartView view) {
			ChartsModel.Cartesian3DChart chart = new ChartsModel.Cartesian3DChart();
			SetupView3D(chart.Options3D);
			chart.Options3D.RotationAngleZ = view.BarDirection == BarChartDirection.Bar ? -90 : 0;
			CreateCatValAxes(chart);
			this.modelChart = chart;
		}
		public void Visit(BarChartView view) {
			ChartsModel.CartesianChart chart = new ChartsModel.CartesianChart();
			chart.Rotated = view.BarDirection == BarChartDirection.Bar;
			chart.BarDistanceFixed = 0;
			CreateCatValAxes(chart);
			this.modelChart = chart;
		}
		public void Visit(BubbleChartView view) {
			ChartsModel.CartesianChart chart = new ChartsModel.CartesianChart();
			CreateXYAxes(chart);
			this.modelChart = chart;
		}
		public void Visit(DoughnutChartView view) {
			ChartsModel.PieChart chart = new ChartsModel.PieChart();
			this.modelChart = chart;
		}
		public void Visit(Line3DChartView view) {
			ChartsModel.Cartesian3DChart chart = new ChartsModel.Cartesian3DChart();
			SetupView3D(chart.Options3D);
			CreateCatValAxes(chart);
			this.modelChart = chart;
		}
		public void Visit(LineChartView view) {
			ChartsModel.CartesianChart chart = new ChartsModel.CartesianChart();
			CreateCatValAxes(chart);
			this.modelChart = chart;
		}
		public void Visit(OfPieChartView view) {
		}
		public void Visit(Pie3DChartView view) {
			ChartsModel.Pie3DChart chart = new ChartsModel.Pie3DChart();
			SetupPieView3D(chart.Options3D);
			this.modelChart = chart;
		}
		public void Visit(PieChartView view) {
			ChartsModel.PieChart chart = new ChartsModel.PieChart();
			this.modelChart = chart;
		}
		public void Visit(RadarChartView view) {
			ChartsModel.RadarChart chart = new ChartsModel.RadarChart();
			CreateCatValAxes(chart);
			chart.Direction = ChartsModel.DirectionMode.Clockwise;
			chart.Style = ChartsModel.CircularDiagramStyle.Polygon;
			this.modelChart = chart;
		}
		public void Visit(ScatterChartView view) {
			ChartsModel.CartesianChart chart = new ChartsModel.CartesianChart();
			CreateXYAxes(chart);
			this.modelChart = chart;
		}
		public void Visit(StockChartView view) {
			ChartsModel.CartesianChart chart = new ChartsModel.CartesianChart();
			CreateCatValAxes(chart);
			this.modelChart = chart;
		}
		public void Visit(Surface3DChartView view) {
		}
		public void Visit(SurfaceChartView view) {
		}
		#endregion
		void CreateCatValAxes(ChartsModel.CartesianChart chart) {
			chart.ArgumentAxis = CreateModelAxis(SpreadsheetChart.PrimaryAxes.Find(AxisDataType.Agrument));
			chart.ValueAxis = CreateModelAxis(SpreadsheetChart.PrimaryAxes.Find(AxisDataType.Value));
			chart.SecondaryArgumentAxes.Add(CreateModelAxis(SpreadsheetChart.SecondaryAxes.Find(AxisDataType.Agrument)));
			chart.SecondaryValueAxes.Add(CreateModelAxis(SpreadsheetChart.SecondaryAxes.Find(AxisDataType.Value)));
		}
		void CreateCatValAxes(ChartsModel.RadarChart chart) {
			AxisBase axis = SpreadsheetChart.PrimaryAxes.Find(AxisDataType.Agrument);
			ChartsModel.RadarAxisX argumentAxis = new ChartsModel.RadarAxisX();
			if (axis != null)
				SetupModelNonPolarAxis(axis, argumentAxis);
			else
				SetupMissingModelAxis(argumentAxis);
			chart.ArgumentAxis = argumentAxis;
			axis = SpreadsheetChart.PrimaryAxes.Find(AxisDataType.Value);
			ChartsModel.CircularAxisY valueAxis = new ChartsModel.CircularAxisY();
			if (axis != null)
				SetupModelNonPolarAxis(axis, valueAxis);
			else
				SetupMissingModelAxis(valueAxis);
			chart.ValueAxis = valueAxis;
		}
		void CreateXYAxes(ChartsModel.CartesianChart chart) {
			chart.ArgumentAxis = CreateModelAxis(SpreadsheetChart.PrimaryAxes.GetItem(0));
			chart.ValueAxis = CreateModelAxis(SpreadsheetChart.PrimaryAxes.GetItem(1));
			chart.SecondaryArgumentAxes.Add(CreateModelAxis(SpreadsheetChart.SecondaryAxes.GetItem(0)));
			chart.SecondaryValueAxes.Add(CreateModelAxis(SpreadsheetChart.SecondaryAxes.GetItem(1)));
		}
		ChartsModel.Axis CreateModelAxis(AxisBase axis) {
			if (axis == null)
				return CreateMissingModelAxis();
			ChartsModel.Axis modelAxis = new ChartsModel.Axis();
			SetupModelAxis(axis, modelAxis);
			return modelAxis;
		}
		ChartsModel.Axis CreateMissingModelAxis() {
			ChartsModel.Axis modelAxis = new ChartsModel.Axis();
			SetupMissingModelAxis(modelAxis);
			return modelAxis;
		}
		void SetupMissingModelAxis(ChartsModel.AxisBase modelAxis) {
			modelAxis.Visible = false;
			modelAxis.GridLinesVisible = false;
			modelAxis.GridLinesMinorVisible = false;
		}
		void SetupModelAxis(AxisBase axis, ChartsModel.Axis modelAxis) {
			modelAxis.Position = GetAxisPosition(axis.Position);
			modelAxis.Reverse = axis.Scaling.Orientation == AxisOrientation.MaxMin;
			modelAxis.Title = new ChartsModel.AxisTitle(modelAxis);
			modelAxis.Title.Text = axis.Title.Text.PlainText;
			modelAxis.Title.Visible = axis.Title.Visible;
			modelAxis.Title.EnableAntialiasing = spreadsheetChart.GetActualTextAntialiasing();
			if (axis.ShapeProperties.Outline.Fill == DrawingFill.None || axis.Delete)
				SpreadsheetChart.MakeAxisTransparent(modelAxis);
			SetupModelNonPolarAxis(axis, modelAxis);
		}
		object GetAxisRangeValue(AxisBase axis, double value) {
			DateAxis dateAxis = axis as DateAxis;
			if(dateAxis != null) {
				VariantValue variantValue = value;
				return variantValue.ToDateTime(axis.DocumentModel.DataContext);
			}
			return value;
		}
		void SetupModelNonPolarAxis(AxisBase axis, ChartsModel.NonPolarAxis modelAxis) {
			if (axis.Scaling.LogScale) {
				modelAxis.Logarithmic = true;
				modelAxis.LogarithmicBase = axis.Scaling.LogBase;
			}
			if (axis.Scaling.FixedMin || axis.Scaling.FixedMax) {
				ChartsModel.AxisRange axisRange = new ChartsModel.AxisRange(modelAxis);
				if (axis.Scaling.FixedMin)
					axisRange.MinValue = GetAxisRangeValue(axis, axis.Scaling.Min);
				if (axis.Scaling.FixedMax)
					axisRange.MaxValue = GetAxisRangeValue(axis, axis.Scaling.Max);
				modelAxis.Range = axisRange;
			}
			modelAxis.GridLinesVisible = axis.ShowMajorGridlines;
			modelAxis.GridLinesMinorVisible = axis.ShowMinorGridlines;
			DateAxis dateAxis = axis as DateAxis;
			if (dateAxis != null)
				SetupDateAxis(modelAxis, dateAxis);
			else {
				AxisMMUnitsBase axisMMUnits = axis as AxisMMUnitsBase;
				if (axisMMUnits != null) {
					modelAxis.AutoGrid = !axisMMUnits.FixedMajorUnit;
					if (axisMMUnits.FixedMajorUnit)
						modelAxis.GridSpacing = axisMMUnits.MajorUnit;
				}
			}
			modelAxis.Label = new ChartsModel.AxisLabel(modelAxis);
			modelAxis.Label.Visible = axis.TickLabelPos != TickLabelPosition.None && !axis.Delete;
			modelAxis.Label.Formatter = axis;
			modelAxis.Label.EnableAntialiasing = spreadsheetChart.GetActualTextAntialiasing();
			SetupAxisTickMarks(axis, modelAxis);
		}
		void SetupDateAxis(ChartsModel.NonPolarAxis modelAxis, DateAxis dateAxis) {
			TimeUnits timeUnits = dateAxis.GetTimeUnits();
			modelAxis.AutoGrid = !dateAxis.FixedMajorUnit && timeUnits == TimeUnits.Auto;
			if (dateAxis.FixedMajorUnit)
				modelAxis.GridSpacing = dateAxis.MajorUnit;
			switch (timeUnits) {
				case TimeUnits.Auto:
				case TimeUnits.Days:
					modelAxis.GridAlignment = ChartsModel.DateTimeGridAlignment.Day;
					break;
				case TimeUnits.Months:
					modelAxis.GridAlignment = ChartsModel.DateTimeGridAlignment.Month;
					break;
				case TimeUnits.Years:
					modelAxis.GridAlignment = ChartsModel.DateTimeGridAlignment.Year;
					break;
			}
		}
		void SetupAxisTickMarks(AxisBase axis, ChartsModel.NonPolarAxis modelAxis) {
			SetupAxisTickMarksCore(axis, modelAxis as ChartsModel.CircularAxisY);
			SetupAxisTickMarksCore(axis, modelAxis as ChartsModel.Axis);
		}
		void SetupAxisTickMarksCore(AxisBase axis, ChartsModel.CircularAxisY modelAxis) {
			if (modelAxis == null)
				return;
			modelAxis.TickmarksVisible = axis.MajorTickMark != TickMark.None && !axis.Delete;
			modelAxis.TickmarksMinorVisible = axis.MinorTickMark != TickMark.None && !axis.Delete;
			modelAxis.TickmarksCrossAxis = axis.MajorTickMark == TickMark.Cross;
		}
		void SetupAxisTickMarksCore(AxisBase axis, ChartsModel.Axis modelAxis) {
			if (modelAxis == null)
				return;
			modelAxis.TickmarksVisible = axis.MajorTickMark != TickMark.None && !axis.Delete;
			modelAxis.TickmarksMinorVisible = axis.MinorTickMark != TickMark.None && !axis.Delete;
			modelAxis.TickmarksCrossAxis = axis.MajorTickMark == TickMark.Cross;
		}
		ChartsModel.AxisPosition GetAxisPosition(AxisPosition position) {
			switch (position) {
				case AxisPosition.Top: return ChartsModel.AxisPosition.Top;
				case AxisPosition.Bottom: return ChartsModel.AxisPosition.Bottom;
				case AxisPosition.Left: return ChartsModel.AxisPosition.Left;
			}
			return ChartsModel.AxisPosition.Right;
		}
		ChartsModel.LegendPosition GetLegendPosition(LegendPosition position) {
			switch (position) {
				case LegendPosition.TopRight: return ChartsModel.LegendPosition.TopRight;
				case LegendPosition.Top: return ChartsModel.LegendPosition.Top;
				case LegendPosition.Bottom: return ChartsModel.LegendPosition.Bottom;
				case LegendPosition.Left: return ChartsModel.LegendPosition.Left;
			}
			return ChartsModel.LegendPosition.Right;
		}
		ChartsModel.LegendOrientation GetLegendOrientation(LegendPosition position) {
			return (position == LegendPosition.Top || position == LegendPosition.Bottom) ? ChartsModel.LegendOrientation.Horizontal : ChartsModel.LegendOrientation.Vertical;
		}
	}
}
