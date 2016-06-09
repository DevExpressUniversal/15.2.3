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

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class TripleExponentialMovingAverageTrix : SeparatePaneIndicator {
		const int DefaultPointsCount = 15;
		public static readonly DependencyProperty ValueLevelProperty = DependencyPropertyManager.Register("ValueLevel", typeof(ValueLevel), typeof(TripleExponentialMovingAverageTrix), new PropertyMetadata(ValueLevel.Close, ChartElementHelper.Update));
		public static readonly DependencyProperty PointsCountProperty = DependencyPropertyManager.Register("PointsCount", typeof(int), typeof(TripleExponentialMovingAverageTrix), new PropertyMetadata(DefaultPointsCount, ChartElementHelper.Update));
		readonly TripleExponentialMovingAverageTrixCalculator calculator = new TripleExponentialMovingAverageTrixCalculator();
		List<GRealPoint2D> indicatorPoints;
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
		public TripleExponentialMovingAverageTrix() {
			DefaultStyleKey = typeof(TripleExponentialMovingAverageTrix);
		}
		protected override MinMaxValues GetMinMaxValues(IMinMaxValues visualRangeOfOtherAxisForFiltering) {
			return GetFilteredMinMaxY(this.indicatorPoints, visualRangeOfOtherAxisForFiltering, new MinMaxValues(calculator.MinY, calculator.MaxY));
		}
		protected override Indicator CreateInstance() {
			return new TripleExponentialMovingAverageTrix();
		}
		protected override void Assign(Indicator indicator) {
			base.Assign(indicator);
			TripleExponentialMovingAverageTrix trix = indicator as TripleExponentialMovingAverageTrix;
			if (trix != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, trix, ValueLevelProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, trix, PointsCountProperty);
			}
		}
		protected internal override void CalculateLayout(IRefinedSeries refinedSeries) {
			base.CalculateLayout(refinedSeries);
			this.indicatorPoints = calculator.Calculate(refinedSeries, PointsCount, (ValueLevelInternal)ValueLevel);
			Item.IndicatorGeometry = new IndicatorGeometry(this);
		}
		public override Geometry CreateGeometry() {
			PathGeometry pathGeometry = new PathGeometry();
			if (this.indicatorPoints != null && indicatorPoints.Count > 0) {
				PaneMapping mapping = new PaneMapping(ActualPane, XYSeries.ActualAxisX, ActualAxisY);
				PathFigure pathFigure = new PathFigure();
				pathFigure.StartPoint = mapping.GetDiagramPoint(indicatorPoints[0]);
				foreach (GRealPoint2D point in indicatorPoints) {
					Point screenPoint = mapping.GetDiagramPoint(point);
					pathFigure.Segments.Add(new LineSegment() { Point = screenPoint });
				}
				pathGeometry.Figures.Add(pathFigure);
			}
			return pathGeometry;
		}
	}
}
