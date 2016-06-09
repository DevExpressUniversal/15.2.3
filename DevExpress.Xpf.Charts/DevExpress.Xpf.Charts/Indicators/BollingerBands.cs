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

using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class BollingerBands : Indicator, IAffectsAxisRange {
		const int DefaultPointsCount = 20;
		const ValueLevel DefaultValueLevel = ValueLevel.Close;
		const double DefaultDeviationMultiplier = 2.0;
		public static readonly DependencyProperty ValueLevelProperty = DependencyPropertyManager.Register("ValueLevel", typeof(ValueLevel), typeof(BollingerBands), new PropertyMetadata(DefaultValueLevel, ChartElementHelper.Update));
		public static readonly DependencyProperty PointsCountProperty = DependencyPropertyManager.Register("PointsCount", typeof(int), typeof(BollingerBands), new PropertyMetadata(DefaultPointsCount, ChartElementHelper.Update));
		public static readonly DependencyProperty StandardDeviationMultiplierProperty = DependencyPropertyManager.Register("StandardDeviationMultiplier", typeof(double), typeof(BollingerBands), new PropertyMetadata(DefaultDeviationMultiplier, ChartElementHelper.Update));
		readonly BollingerBandsCalculator calculator = new BollingerBandsCalculator();
		[Category(Categories.Behavior),
		XtraSerializableProperty]
		public ValueLevel ValueLevel {
			get { return (ValueLevel)GetValue(ValueLevelProperty); }
			set { SetValue(ValueLevelProperty, value); }
		}
		[Category(Categories.Behavior),
		XtraSerializableProperty]
		public int PointsCount {
			get { return (int)GetValue(PointsCountProperty); }
			set { SetValue(PointsCountProperty, value); }
		}
		[Category(Categories.Behavior),
		 XtraSerializableProperty]
		public double StandardDeviationMultiplier {
			get { return (double)GetValue(StandardDeviationMultiplierProperty); }
			set { SetValue(StandardDeviationMultiplierProperty, value); }
		}
		public BollingerBands() {
			DefaultStyleKey = typeof(BollingerBands);
		}
		#region IAffectsAxisRange
		MinMaxValues IAffectsAxisRange.GetMinMaxValues(IMinMaxValues visualRangeOfOtherAxisForFiltering) {
			return GetFilteredMinMaxY(visualRangeOfOtherAxisForFiltering);
		}
		IAxisData IAffectsAxisRange.AxisYData {
			get { return XYSeries.ActualAxisY; }
		}
		#endregion
		MinMaxValues GetFilteredMinMaxY(IMinMaxValues visualRangeOfOtherAxisForFiltering) {
			bool isRangeEmpty = double.IsNaN(visualRangeOfOtherAxisForFiltering.Delta);
			if (isRangeEmpty)
				return new MinMaxValues(calculator.MinY, calculator.MaxY);
			if (calculator.UpperBandPoints == null || calculator.LowerBandPoints == null)
				return MinMaxValues.Empty;
			int minIndex = -1;
			ChartDebug.Assert(calculator.UpperBandPoints.Count == calculator.LowerBandPoints.Count);
			for (int i = 0; i < calculator.UpperBandPoints.Count; i++) {
				if (calculator.UpperBandPoints[i].X > visualRangeOfOtherAxisForFiltering.Min) {
					minIndex = i;
					break;
				}
			}
			int maxIndex = -1;
			for (int i = calculator.UpperBandPoints.Count - 1; i > -1; i--) {
				if (calculator.UpperBandPoints[i].X < visualRangeOfOtherAxisForFiltering.Max) {
					maxIndex = i;
					break;
				}
			}
			if (minIndex == -1 || maxIndex == -1)
				return MinMaxValues.Empty;
			double minValue = calculator.LowerBandPoints[minIndex].Y;
			double maxValue = calculator.UpperBandPoints[minIndex].Y;
			for (int i = minIndex + 1; i < maxIndex; i++) {
				if (calculator.LowerBandPoints[i].Y < minValue)
					minValue = calculator.LowerBandPoints[i].Y;
				if (calculator.UpperBandPoints[i].Y > maxValue)
					maxValue = calculator.UpperBandPoints[i].Y;
			}
			return new MinMaxValues(minValue, maxValue);
		}
		protected override Indicator CreateInstance() {
			return new BollingerBands();
		}
		protected override void Assign(Indicator indicator) {
			base.Assign(indicator);
			BollingerBands commodityChannelIndex = indicator as BollingerBands;
			if (commodityChannelIndex != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, commodityChannelIndex, ValueLevelProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, commodityChannelIndex, PointsCountProperty);
			}
		}
		protected internal override void CalculateLayout(IRefinedSeries refinedSeries) {
			base.CalculateLayout(refinedSeries);
			calculator.Calculate(refinedSeries, PointsCount, StandardDeviationMultiplier, (ValueLevelInternal)ValueLevel);
			Item.IndicatorGeometry = new IndicatorGeometry(this);
		}
		public override Geometry CreateGeometry() {
			PathGeometry pathGeometry = new PathGeometry();
			if (calculator.Calculated) {
				PaneMapping mapping = new PaneMapping(XYSeries.ActualPane, XYSeries.ActualAxisX, XYSeries.ActualAxisY);
				PathFigure movingAverageFigure = new PathFigure();
				movingAverageFigure.StartPoint = mapping.GetDiagramPoint(calculator.MovingAveragePoints[0]);
				for (int i = 1; i < calculator.MovingAveragePoints.Count; i++) {
					Point screenPoint = mapping.GetDiagramPoint(calculator.MovingAveragePoints[i]);
					movingAverageFigure.Segments.Add(new LineSegment() { Point = screenPoint });
				}
				pathGeometry.Figures.Add(movingAverageFigure);
				PathFigure upperBandFigure = new PathFigure();
				upperBandFigure.StartPoint = mapping.GetDiagramPoint(calculator.UpperBandPoints[0]);
				foreach (GRealPoint2D point in calculator.UpperBandPoints) {
					Point screenPoint = mapping.GetDiagramPoint(point);
					upperBandFigure.Segments.Add(new LineSegment() { Point = screenPoint });
				}
				pathGeometry.Figures.Add(upperBandFigure);
				PathFigure lowerBandFigure = new PathFigure();
				lowerBandFigure.StartPoint = mapping.GetDiagramPoint(calculator.LowerBandPoints[0]);
				foreach (GRealPoint2D point in calculator.LowerBandPoints) {
					Point screenPoint = mapping.GetDiagramPoint(point);
					lowerBandFigure.Segments.Add(new LineSegment() { Point = screenPoint });
				}
				pathGeometry.Figures.Add(lowerBandFigure);
			}
			return pathGeometry;
		}
	}
}
