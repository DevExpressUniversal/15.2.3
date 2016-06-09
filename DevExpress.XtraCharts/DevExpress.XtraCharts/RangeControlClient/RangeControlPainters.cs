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
using System.Drawing;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class RangeControlMapping {
		public static RangeControlMapping Create(WholeRange range) {
			RangeDataBase rangeData = range.RangeData;
			AxisScaleTypeMap scaleMap = rangeData.Axis.ScaleTypeMap;
			double minRefined = scaleMap.InternalToRefinedExact(rangeData.MinValueInternal);
			double maxRefined = scaleMap.InternalToRefinedExact(rangeData.MaxValueInternal);
			double sideMargin = scaleMap.NativeToRefined(rangeData.MinValue) - minRefined;
			return new RangeControlMapping(new MinMaxValues(minRefined, maxRefined), sideMargin);
		}
		readonly double sideMargin;
		readonly IMinMaxValues wholeRange;
		readonly IMinMaxValues dataRange;
		readonly double rangeControlSideMargin;
		readonly IMinMaxValues rangeControlRange;
		internal IMinMaxValues RangeControlRange { get { return rangeControlRange; } }
		internal IMinMaxValues DataRange { get { return dataRange; } }
		internal double ChartSideMargin { get { return sideMargin; } }
		protected RangeControlMapping(IMinMaxValues wholeRange, double sideMargin) {
			this.wholeRange = wholeRange;
			this.sideMargin = sideMargin;
			this.dataRange = new MinMaxValues(wholeRange.Min + sideMargin, wholeRange.Max - sideMargin);
			this.rangeControlSideMargin = dataRange.Delta > 0 ? dataRange.Delta * 0.01 : 0.5;
			this.rangeControlRange = new MinMaxValues(dataRange.Min - rangeControlSideMargin, dataRange.Max + rangeControlSideMargin);			
		}
		public double RangeValueToValue(double rangeValue) {
			if (rangeValue < rangeControlRange.Min)
				return wholeRange.Min;
			if (rangeValue < dataRange.Min)
				return dataRange.Min - (dataRange.Min - rangeValue) / rangeControlSideMargin * sideMargin;
			if (rangeValue >= dataRange.Min && rangeValue <= dataRange.Max)
				return rangeValue;
			if (rangeValue < rangeControlRange.Max)
				return dataRange.Max + (rangeValue - dataRange.Max) / rangeControlSideMargin * sideMargin; 
			return wholeRange.Max;
		}
		public MinMaxValues EnsureRangeInsideWholeRange(IMinMaxValues range) {
			ChartDebug.Assert(range.Min < range.Max, "Range.Min must be less than Range.Max.");
			double min, max;
			if (range.Max < wholeRange.Min || range.Min > wholeRange.Max) {
				min = wholeRange.Min;
				max = wholeRange.Max;
			}
			else {
				min = RangeValueToValue(range.Min);
				max = RangeValueToValue(range.Max);
			}
			return new MinMaxValues(min, max);
		}
		public double ValueToRangeValue(double value) {
			if (value < wholeRange.Min)
				return rangeControlRange.Min;
			if (value < dataRange.Min)
				return dataRange.Min - (dataRange.Min - value) / sideMargin * rangeControlSideMargin; 
			if (value >= dataRange.Min && value <= dataRange.Max)
				return value;
			if (value < wholeRange.Max)
				return dataRange.Max + (value - dataRange.Max) / sideMargin * rangeControlSideMargin; 
			return rangeControlRange.Max;
		}
		public double ValueToNormalValue(double value) {
			return (ValueToRangeValue(value) - rangeControlRange.Min) / rangeControlRange.Delta; 
		}
		public double NormalValueToValue(double normalValue) {
			return RangeValueToValue(rangeControlRange.Min + rangeControlRange.Delta * normalValue);			
		}
		public bool IsCorrectGridValue(double value) {
			return value >= dataRange.Min && value <= dataRange.Max;
		}
	}
	public partial class RangeControlPainter {
		const double verticalMargin = 0.1;
		readonly GdiPlusRenderer renderer;
		readonly RangeControlMapping mapping;
		readonly XYDiagram2D diagram;
		readonly double rangeY;
		readonly double minY;
		readonly IRangeControlPaint paint;		
		IRenderer Renderer {
			get { return this.renderer as IRenderer; }
		}
		XYDiagram2D Diagram {
			get { return this.diagram; }
		}
		double RangeY {
			get { return this.rangeY; }
		}
		AxisScaleTypeMap AxisXScaleMap { get { return diagram.ActualAxisX.ScaleTypeMap; } }
#if DEBUGTEST
		internal double MinY {
			get { return minY; }
		}
#endif
		public RangeControlPainter(XYDiagram2D diagram, IRangeControlPaint paint) {
			this.renderer = new GdiPlusRenderer();
			this.paint = paint;
			this.diagram = diagram;
			Axis2D axisX = diagram.ActualAxisX;
			Axis2D axisY = diagram.ActualAxisY;
			mapping = RangeControlMapping.Create(axisX.WholeRange);
			IMinMaxValues axisYRange = FitYRange(Diagram.ViewController.GetAxisRange(axisY));
			this.rangeY = axisYRange.Delta;
			this.minY = axisYRange.Min;			
		}
		IMinMaxValues FitYRange(IMinMaxValues range) {
			double margin = range.Delta * verticalMargin;
			if (margin < 0.5)
				margin = 0.5;
			return new MinMaxValues(range.Min - margin, range.Max + margin);
		}
		double ProjectValue(XYDiagram2DSeriesViewBase seriesView, RefinedPoint refinedPoint) {
			double value = ((IValuePoint)refinedPoint).Value;
			if (seriesView is FinancialSeriesViewBase) {
				switch (seriesView.RangeControlOptions.ValueLevel) {
					case ValueLevel.Open:
						value = ((IFinancialPoint)refinedPoint).Open;
						break;
					case ValueLevel.Close:
						value = ((IFinancialPoint)refinedPoint).Close;
						break;
					case ValueLevel.Low:
						value = ((IFinancialPoint)refinedPoint).Low;
						break;
					case ValueLevel.High:
						value = ((IFinancialPoint)refinedPoint).High;
						break;
				}
			} else if (seriesView is IStackedView)
				value = ((IStackedPoint)refinedPoint).MaxValue;
			return MathUtils.StrongRound((double)paint.ContentBounds.Top + ((double)paint.ContentBounds.Height - (value - minY) / RangeY * (double)paint.ContentBounds.Height));
		}
		void DrawLineSeries(IRefinedSeries series, byte alpha) {
			XYDiagram2DSeriesViewBase xySeriesView = series.SeriesView as XYDiagram2DSeriesViewBase;
			if (xySeriesView != null) {
				PointF? prevPoint = null;
				Color lineColor = Color.FromArgb(alpha, xySeriesView.ActualColor);
				foreach (RefinedPoint refinedPoint in series.Points) {
					if (!refinedPoint.IsEmpty) {
						PointF currentPoint = new PointF();
						double refinedValue = AxisXScaleMap.InternalToRefined(refinedPoint.Argument);
						currentPoint.X = paint.CalcX(mapping.ValueToNormalValue(refinedValue));
						currentPoint.Y = (float)ProjectValue(xySeriesView, refinedPoint);
						if (prevPoint != null)
							Renderer.DrawLine((PointF)prevPoint, currentPoint, lineColor, 1);
						prevPoint = currentPoint;
					}
					else
						prevPoint = null;
				}
			}
		}
		void DrawAreaSeries(IRefinedSeries series, byte alpha) {
			XYDiagram2DSeriesViewBase xySeriesView = series.SeriesView as XYDiagram2DSeriesViewBase;
			if (xySeriesView != null) {
				Color polygonColor = Color.FromArgb(alpha, xySeriesView.ActualColor);
				LineStrip polygon = new LineStrip();
				foreach (RefinedPoint refinedPoint in series.Points) {
					if (!refinedPoint.IsEmpty) {
						double refinedValue = AxisXScaleMap.InternalToRefined(refinedPoint.Argument);
						GRealPoint2D currentPoint = new GRealPoint2D(
							paint.CalcX(mapping.ValueToNormalValue(refinedValue)),
							ProjectValue(xySeriesView, refinedPoint));
						if (polygon.IsEmpty)
							polygon.Add(new GRealPoint2D(currentPoint.X, paint.ContentBounds.Bottom));
						polygon.Add(currentPoint);
					}
					else {
						DrawPolygon(polygonColor, polygon);
						polygon.Clear();
					}
				}
				DrawPolygon(polygonColor, polygon);
			}
		}
		void DrawPolygon(Color polygonColor, LineStrip polygon) {
			if (!polygon.IsEmpty) {
				polygon.Add(new GRealPoint2D(polygon[polygon.Count - 1].X, paint.ContentBounds.Bottom));
				Renderer.FillPolygon(polygon, polygonColor);
			}
		}
		void DrawSeries(IRefinedSeries series, byte alpha) {
			XYDiagram2DSeriesViewBase seriesView = series.SeriesView as XYDiagram2DSeriesViewBase;			
			switch (seriesView.RangeControlOptions.ViewType) {
				case RangeControlViewType.Area:
					DrawAreaSeries(series, alpha);
					break;
				case RangeControlViewType.Line:
					DrawLineSeries(series, alpha);
					break;
			}			
		}
		bool ShouldDrawSeries(XYDiagram2DSeriesViewBase view) {
			if (view != null && view.RangeControlOptions != null && view.RangeControlOptions.Visible)
				return object.ReferenceEquals(view.ActualAxisX, Diagram.ActualAxisX) && object.ReferenceEquals(view.ActualAxisY, Diagram.ActualAxisY);
			return false;
		}
		byte GetSeriesAlpha(int numStacked) {
			int numSeries = Diagram.ViewController.ActiveSeriesCount - numStacked;
			if (numSeries > 0)
				return (byte)(255.0 / numSeries);
			return 255;
		}
		byte GetSeriesTransparency(XYDiagram2DSeriesViewBase view, byte defaultTransparency) {
			byte? transparency = view.RangeControlOptions.SeriesTransparency;
			return (transparency == null) ? defaultTransparency : transparency.Value;
		}
		void PrepareDrawing() {
			Renderer.Reset(paint.Graphics, paint.ContentBounds);
		}
		void FinishDrawing() {
			Renderer.Present();
		}
		public void Draw() {
			PrepareDrawing();
			int alreadyDrawedStackedSeries = 0;
			var activeSeries = Diagram.ViewController.ActiveRefinedSeries;
			if (activeSeries != null) {
				for (int index = activeSeries.Count - 1; index >= 0; index--) {
					IRefinedSeries series = activeSeries[index];
					XYDiagram2DSeriesViewBase view = series.SeriesView as XYDiagram2DSeriesViewBase;
					if (ShouldDrawSeries(view) && view is IStackedView && view.RangeControlOptions.ViewType == RangeControlViewType.Area) {
						DrawSeries(series, GetSeriesTransparency(view, 255));
						alreadyDrawedStackedSeries++;
					}
				}
				for (int index = 0; index < activeSeries.Count; index++) {
					IRefinedSeries series = activeSeries[index];
					XYDiagram2DSeriesViewBase view = series.SeriesView as XYDiagram2DSeriesViewBase;
					if (ShouldDrawSeries(view) && !(series.SeriesView is IStackedView && view.RangeControlOptions.ViewType == RangeControlViewType.Area)) {
						byte alpha = view.RangeControlOptions.ViewType == RangeControlViewType.Area ? GetSeriesAlpha(alreadyDrawedStackedSeries) : (byte)255;
						DrawSeries(series, GetSeriesTransparency(view, alpha));
					}
				}
			}
			FinishDrawing();
		}
	}
}
