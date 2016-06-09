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
using System.ComponentModel;
using System.Windows.Shapes;
namespace DevExpress.Xpf.Editors.Internal {
	public abstract class SparklineMappingBase {
		public static SparklineMappingBase CreateMapping(SparklineViewType viewType, IList<SparklinePoint> sortedPoints, Bounds bounds, InternalRange argumentRange, InternalRange valueRange) {
			if (bounds.Width <= 0 || bounds.Height <= 0)
				return null;
			switch (viewType) {
				case SparklineViewType.Line:
				case SparklineViewType.Area:
					return new LineSparklineMapping(sortedPoints, bounds, argumentRange, valueRange);
				case SparklineViewType.Bar:
					return new BarSparklineMapping(sortedPoints, bounds, argumentRange, valueRange);
				case SparklineViewType.WinLoss:
					return new WinLossSparklineMapping(sortedPoints, bounds, argumentRange, valueRange);
				default:
					return null;
			}
		}
		readonly Bounds bounds;
		readonly InternalRange argumentRange;
		readonly InternalRange valueRange;
		readonly double scaleX;
		readonly double yZeroValue;
		readonly double screenYZeroValue;
		protected double MinY { get { return valueRange.ActualMin; } }
		protected double MaxY { get { return valueRange.ActualMax; } }
		protected double MinX { get { return argumentRange.ActualMin; } }
		protected double MaxX { get { return argumentRange.ActualMax; } }
		public double MinPointsDistancePx { get; private set; }
		public double ScreenYZeroValue { get { return screenYZeroValue; } }
		public double ScaleX { get { return scaleX; } }
		public Bounds Bounds { get { return bounds; } }
		public SparklineMappingBase(IList<SparklinePoint> sortedPoints, Bounds bounds, InternalRange argumentRange, InternalRange valueRange) {
			this.bounds = bounds;
			this.argumentRange = argumentRange;
			this.valueRange = valueRange;
			double minPointDistance = GetMinPointDistance(sortedPoints, argumentRange);
			if (valueRange.Auto)
				CorrectValueRange(valueRange);
			if (argumentRange.Auto)
				CorrectArgumentRange(argumentRange, minPointDistance);
			scaleX = CalculateScaleX();
			MinPointsDistancePx = scaleX * minPointDistance;
			yZeroValue = CalculateZeroValue();
			screenYZeroValue = MapYValueToScreen(yZeroValue);
		}
		double GetMinPointDistance(IList<SparklinePoint> ActualPoints, InternalRange argumentRange) {
			if (ActualPoints.Count == 0)
				return 0;
			double minDistance = double.NaN;
			for (int i = 1; i < ActualPoints.Count; i++) {
				double current = ActualPoints[i].Argument;
				double previous = ActualPoints[i - 1].Argument;
				if (argumentRange.ContainsValue(current) || argumentRange.ContainsValue(previous)) {
					double currentMinDistance = current - previous;
					if (double.IsNaN(minDistance))
						minDistance = currentMinDistance;
					else if (minDistance > currentMinDistance)
						minDistance = currentMinDistance;
				}
			}
			if (double.IsNaN(minDistance))
				return 0.5;
			return minDistance;
		}
		double CalculateScaleX() {
			return MaxX != MinX ? ((double)Bounds.Width) / (MaxX - MinX) : 0;
		}
		protected virtual void CorrectArgumentRange(InternalRange argumentRange, double minPointDistance) { 
		}
		protected virtual void CorrectValueRange(InternalRange valueRange) {
			if (valueRange.Min == valueRange.Max) {
				valueRange.CorrectionMin = -0.5;
				valueRange.CorrectionMax = 0.5;
			}
		}
		protected virtual double CalculateZeroValue() {
			return MinY > 0 ? MinY : MaxY < 0 ? MaxY : 0;
		}
		public double MapYValueToScreen(double value) {
			return Bounds.Y + ((MaxY - value) / (MaxY - MinY)) * Bounds.Height;
		}
		public double MapXValueToScreen(double value) {
			return Bounds.X + (value - MinX) * ScaleX;
		}
		public bool isPointVisible(double x) {
			if (x > MaxX || x < MinX)
				return false;
			return true;
		}
	}
	public class ExtremePointIndexes {
		public int Min { get; set; }
		public int Max { get; set; }
		public int Start { get; set; }
		public int End { get; set; }
		public bool IsEmpty { get { return Start == -1 && End == -1; } }
		public ExtremePointIndexes(IEnumerable<SparklinePoint> sortedItems) {
			Min = Max =Start = End = -1;
			if (sortedItems != null)
				Fill(sortedItems);
		}
		void Fill(IEnumerable<SparklinePoint> sortedItems) {
			double minValue = double.MaxValue;
			double maxValue = double.MinValue;
			int index = 0;
			foreach (SparklinePoint item in sortedItems) {
				if (SparklineMathUtils.IsValidDouble(item.Value)) {
					if (Start < 0)
						Start = index;
					if (Start >= 0)
						End = index;
					if (item.Value < minValue) {
						minValue = item.Value;
						Min = index;
					}
					if (item.Value > maxValue) {
						maxValue = item.Value;
						Max = index;
					}
				}
				index++;
			}
		}
		public bool IsMinPoint(int pointIndex) {
			return pointIndex == Min;
		}
		public bool IsMaxPoint(int pointIndex) {
			return pointIndex == Max;
		}
		public bool IsStartPoint(int pointIndex) {
			return pointIndex == Start;
		}
		public bool IsEndPoint(int pointIndex) {
			return pointIndex == End;
		}
	}
}
