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
	public class MassIndex : SeparatePaneIndicator {
		const int DefaultMaPointsCount = 9;  
		const int DefaultSumPointsCount = 25;
		public static readonly DependencyProperty MovingAveragePointsCountProperty = DependencyPropertyManager.Register("MovingAveragePointsCount", typeof(int), typeof(MassIndex), new PropertyMetadata(DefaultMaPointsCount, ChartElementHelper.Update));
		public static readonly DependencyProperty SumPointsCountProperty = DependencyPropertyManager.Register("SumPointsCount", typeof(int), typeof(MassIndex), new PropertyMetadata(DefaultSumPointsCount, ChartElementHelper.Update));
		readonly MassIndexCalculator calculator = new MassIndexCalculator();
		List<GRealPoint2D> indicatorPoints;
		[Category(Categories.Behavior),
		XtraSerializableProperty]
		public int MovingAveragePointsCount {
			get { return (int)GetValue(MovingAveragePointsCountProperty); }
			set { SetValue(MovingAveragePointsCountProperty, value); }
		}
		[Category(Categories.Behavior),
		 XtraSerializableProperty]
		public int SumPointsCount {
			get { return (int)GetValue(SumPointsCountProperty); }
			set { SetValue(SumPointsCountProperty, value); }
		}
		public MassIndex() {
			DefaultStyleKey = typeof(MassIndex);
		}
		protected override MinMaxValues GetMinMaxValues(IMinMaxValues visualRangeOfOtherAxisForFiltering) {
			return GetFilteredMinMaxY(this.indicatorPoints, visualRangeOfOtherAxisForFiltering, new MinMaxValues(calculator.MinY, calculator.MaxY));
		}
		protected override Indicator CreateInstance() {
			return new MassIndex();
		}
		protected override void Assign(Indicator indicator) {
			base.Assign(indicator);
			MassIndex massIndex = indicator as MassIndex;
			if (massIndex != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, massIndex, MovingAveragePointsCountProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, massIndex, SumPointsCountProperty);
			}
		}
		protected internal override void CalculateLayout(IRefinedSeries refinedSeries) {
			base.CalculateLayout(refinedSeries);
			this.indicatorPoints = calculator.Calculate(refinedSeries, MovingAveragePointsCount, SumPointsCount);
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
