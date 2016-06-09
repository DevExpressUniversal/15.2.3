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

using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace DevExpress.Sparkline.Core {
	public abstract class SparklineMappingBase {
		static SparklineMappingBase Create(SparklineViewType viewType, Rectangle bounds) {
			if ((bounds.Width <= 0) || (bounds.Height <= 0))
				return null;
			switch (viewType) {
				case SparklineViewType.Bar:
					return new BarSparklineMapping(bounds);
				case SparklineViewType.WinLoss:
					return new WinLossSparklineMapping(bounds);
				case SparklineViewType.Line:
				case SparklineViewType.Area:
				default:
					return new LineSparklineMapping(bounds);
			}
		}
		public static SparklineMappingBase Create(SparklineViewType viewType, Rectangle bounds, SparklineDataProvider dataProvider) {
			SparklineMappingBase mapping = Create(viewType, bounds);
			if (mapping != null)
				mapping.Initialize(dataProvider);
			return mapping;
		}
		public static SparklineMappingBase Create(SparklineViewType viewType, Rectangle bounds, SparklineDataProvider dataProvider, SparklineInteractionRanges ranges, Matrix normalTransform) {
			SparklineMappingBase mapping = Create(viewType, bounds);
			if (mapping != null)
				mapping.Initialize(dataProvider, ranges, normalTransform);
			return mapping;
		}
		readonly Rectangle bounds;
		double scaleX;
		double yZeroValue;
		int screenYZeroValue;
		int lastPointIndex;
		SparklineIndexRange valuePointRange;
		SparklineRangeData argumentRange;
		SparklineRangeData valueRange;
		Matrix normalTransform;
		double minPointsDistancePx;
		public double MinPointsDistancePx { get { return minPointsDistancePx; } }
		public int ScreenYZeroValue { get { return screenYZeroValue; } }
		public double ScaleX { get { return scaleX; } }
		public Rectangle Bounds { get { return bounds; } }
		protected SparklineMappingBase(Rectangle bounds) {
			this.bounds = bounds;
		}
		void Initialize(SparklineDataProvider dataProvider) {
			this.lastPointIndex = dataProvider.SortedPoints.Count - 1;
			SparklineRangeData dataArgumentRange = dataProvider.DataArgumentRange;
			double minPointDistance = GetMinInternalArgumentDistance(dataProvider.SortedPoints, dataArgumentRange);
			argumentRange = CorrectArgumentRange(minPointDistance, dataArgumentRange);
			valueRange = CorrectValueRange(dataProvider.DataValueRange);
			valuePointRange = dataProvider.ValuePointRange;
			scaleX = CalculateScaleX();
			minPointsDistancePx = scaleX * minPointDistance;
			yZeroValue = CalculateZeroValue();
			screenYZeroValue = MapYValueToScreen(yZeroValue);
		}
		void Initialize(SparklineDataProvider dataProvider, SparklineInteractionRanges ranges, Matrix normalTransform) {
			this.normalTransform = normalTransform;
			this.lastPointIndex = dataProvider.SortedPoints.Count - 1;
			SparklineRangeData dataArgumentRange = ranges.ActualDataArgumentRange;
			argumentRange = CorrectArgumentRange(ranges.MinPointDistance, dataArgumentRange);
			valueRange = CorrectValueRange(ranges.DataValueRange);
			valuePointRange = dataProvider.ValuePointRange;
			scaleX = CalculateScaleX();
			minPointsDistancePx = scaleX * ranges.MinPointDistance;
			yZeroValue = CalculateZeroValue();
			screenYZeroValue = MapYValueToScreen(yZeroValue);
		}
		double GetMinInternalArgumentDistance(IList<SparklinePoint> points, SparklineRangeData argumentRange) {
			if (points.Count == 0)
				return 0;
			double minDistance = double.NaN;
			for (int i = 1; i < points.Count; i++) {
				double current = points[i].Argument;
				double previous = points[i - 1].Argument;
				if (argumentRange.InRange(current) || argumentRange.InRange(previous)) {
					double currentMinDistance = current - previous;
					if (double.IsNaN(minDistance) || (minDistance > currentMinDistance))
						minDistance = currentMinDistance;
				}
			}
			if (double.IsNaN(minDistance))
				return 0.5;
			return minDistance;
		}
		double CalculateScaleX() {
			return argumentRange.Max != argumentRange.Min ? ((double)Bounds.Width) / (argumentRange.Max - argumentRange.Min) : 0;
		}
		protected virtual SparklineRangeData CorrectArgumentRange(double minPointDistance, SparklineRangeData range) {
			return range;
		}
		protected virtual SparklineRangeData CorrectValueRange(SparklineRangeData range) {
			if (range.Min == range.Max)
				return new SparklineRangeData(range.Min - 0.5, range.Max + 0.5);
			return range;
		}
		protected virtual double CorrectValue(double value) {
			return value;
		}
		protected virtual double CalculateZeroValue() {
			return valueRange.Min > 0 ? valueRange.Min : valueRange.Max < 0 ? valueRange.Max : 0;
		}
		public bool IsMinValuePoint(int pointIndex) {
			return pointIndex == valuePointRange.Min;
		}
		public bool IsMaxValuePoint(int pointIndex) {
			return pointIndex == valuePointRange.Max;
		}
		public bool IsStartPoint(int pointIndex) {
			return pointIndex == 0;
		}
		public bool IsEndPoint(int pointIndex) {
			return pointIndex == lastPointIndex;
		}
		public int MapYValueToScreen(double value) {
			return Bounds.Y + SparklineMathUtils.Round((valueRange.Max - CorrectValue(value)) / (valueRange.Max - valueRange.Min) * Bounds.Height);
		}
		public int MapXValueToScreen(double value) {
			return Bounds.X + SparklineMathUtils.Round((value - argumentRange.Min) * ScaleX);
		}
		public Point MapPoint(double argument, double value) {
			double normalValue = 0.0;
			if (valueRange.Delta != 0.0)
				normalValue = (valueRange.Max - CorrectValue(value)) / valueRange.Delta;
			double normalArgument = 0.0;
			if (argumentRange.Delta != 0.0)
				normalArgument = (argument - argumentRange.Min) / argumentRange.Delta;
			PointF normalPoint = new PointF((float)normalArgument, (float)normalValue);
			if (normalTransform != null) {
				PointF[] buffer = new PointF[] { normalPoint };
				normalTransform.TransformPoints(buffer);
				normalPoint = buffer[0];
			}
			return new Point(SparklineMathUtils.Round(Bounds.X + normalPoint.X * bounds.Width), SparklineMathUtils.Round(Bounds.Y + normalPoint.Y * bounds.Height));
		}
	}
}
