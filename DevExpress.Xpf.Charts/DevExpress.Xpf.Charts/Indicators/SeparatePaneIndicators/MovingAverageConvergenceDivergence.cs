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
	public class MovingAverageConvergenceDivergence : SeparatePaneIndicator {
		const int DefaultLongPeriod = 26;
		const int DefaultShortPeriod = 12;
		const int DefaultSignalSmoothingPeriod = 9;
		public static readonly DependencyProperty LongPeriodProperty = DependencyPropertyManager.Register("LongPeriod", typeof(int), typeof(MovingAverageConvergenceDivergence), new PropertyMetadata(DefaultLongPeriod, ChartElementHelper.Update));
		public static readonly DependencyProperty ShortPeriodProperty = DependencyPropertyManager.Register("ShortPeriod", typeof(int), typeof(MovingAverageConvergenceDivergence), new PropertyMetadata(DefaultShortPeriod, ChartElementHelper.Update));
		public static readonly DependencyProperty SignalSmoothingPeriodProperty = DependencyPropertyManager.Register("SignalSmoothingPeriod", typeof(int), typeof(MovingAverageConvergenceDivergence), new PropertyMetadata(DefaultShortPeriod, ChartElementHelper.Update));
		public static readonly DependencyProperty ValueLevelProperty = DependencyPropertyManager.Register("ValueLevel", typeof(ValueLevel), typeof(MovingAverageConvergenceDivergence), new PropertyMetadata(ValueLevel.Close, ChartElementHelper.Update));
		readonly MovingAverageConvergenceDivergenceCalculator calculator = new MovingAverageConvergenceDivergenceCalculator();
		[Category(Categories.Behavior),
		 XtraSerializableProperty]
		public ValueLevel ValueLevel {
			get { return (ValueLevel)GetValue(ValueLevelProperty); }
			set { SetValue(ValueLevelProperty, value); }
		}
		[Category(Categories.Behavior),
		XtraSerializableProperty]
		public int LongPeriod {
			get { return (int)GetValue(LongPeriodProperty); }
			set { SetValue(LongPeriodProperty, value); }
		}
		[Category(Categories.Behavior),
		 XtraSerializableProperty]
		public int ShortPeriod {
			get { return (int)GetValue(ShortPeriodProperty); }
			set { SetValue(ShortPeriodProperty, value); }
		}
		[Category(Categories.Behavior),
		 XtraSerializableProperty]
		public int SignalSmoothingPeriod {
			get { return (int)GetValue(SignalSmoothingPeriodProperty); }
			set { SetValue(SignalSmoothingPeriodProperty, value); }
		}
		public MovingAverageConvergenceDivergence() {
			DefaultStyleKey = typeof(MovingAverageConvergenceDivergence);
		}
		protected override MinMaxValues GetMinMaxValues(IMinMaxValues visualRangeOfOtherAxisForFiltering) {
			return GetFilteredMinMaxY(this.calculator.MacdPoints, visualRangeOfOtherAxisForFiltering, new MinMaxValues(calculator.MinY, calculator.MaxY));
		}
		protected override Indicator CreateInstance() {
			return new MovingAverageConvergenceDivergence();
		}
		protected override void Assign(Indicator indicator) {
			base.Assign(indicator);
			MovingAverageConvergenceDivergence macd = indicator as MovingAverageConvergenceDivergence;
			if (macd != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, macd, LongPeriodProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, macd, ShortPeriodProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, macd, SignalSmoothingPeriodProperty);
			}
		}
		protected internal override void CalculateLayout(IRefinedSeries refinedSeries) {
			base.CalculateLayout(refinedSeries);
			calculator.Calculate(refinedSeries, ShortPeriod, LongPeriod, SignalSmoothingPeriod, (ValueLevelInternal)ValueLevel);
			Item.IndicatorGeometry = new IndicatorGeometry(this);
		}
		public override Geometry CreateGeometry() {
			PathGeometry pathGeometry = new PathGeometry();
			if (calculator.MacdPoints != null && calculator.MacdPoints.Count > 0 && calculator.SignalPoints != null && calculator.SignalPoints.Count > 0) {
				PaneMapping mapping = new PaneMapping(ActualPane, XYSeries.ActualAxisX, ActualAxisY);
				PathFigure macdFigure = new PathFigure();
				macdFigure.StartPoint = mapping.GetDiagramPoint(calculator.MacdPoints[0]);
				for (int i=1; i< calculator.MacdPoints.Count; i++) {
					Point screenPoint = mapping.GetDiagramPoint(calculator.MacdPoints[i]);
					macdFigure.Segments.Add(new LineSegment() { Point = screenPoint });
				}
				pathGeometry.Figures.Add(macdFigure);
				PathFigure signalFigure = new PathFigure();
				signalFigure.StartPoint = mapping.GetDiagramPoint(calculator.SignalPoints[0]);
				foreach (GRealPoint2D point in calculator.SignalPoints) {
					Point screenPoint = mapping.GetDiagramPoint(point);
					signalFigure.Segments.Add(new LineSegment() { Point = screenPoint });
				}
				pathGeometry.Figures.Add(signalFigure);
			}
			return pathGeometry;
		}
	}
}
