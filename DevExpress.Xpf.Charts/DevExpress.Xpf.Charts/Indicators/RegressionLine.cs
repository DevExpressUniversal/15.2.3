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

using System.Windows;
using System.Windows.Media;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Charts.Native;
using System.ComponentModel;
using DevExpress.Xpf.Utils;
using System;
namespace DevExpress.Xpf.Charts {
	public class RegressionLine : Indicator {
		public static readonly DependencyProperty ValueLevelProperty = DependencyPropertyManager.Register("ValueLevel",
		  typeof(ValueLevel), typeof(RegressionLine), new PropertyMetadata(ValueLevel.Value, ChartElementHelper.Update));
		Point startPoint, endPoint;
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public ValueLevel ValueLevel { 
			get { return (ValueLevel)GetValue(ValueLevelProperty); }
			set { SetValue(ValueLevelProperty, value); }
		}
		public RegressionLine() {
			DefaultStyleKey = typeof(RegressionLine);
		}
		public RegressionLine(ValueLevel valueLevel) : this(){
			this.ValueLevel = valueLevel;
		}
		public override Geometry CreateGeometry() {
			GeometryGroup geometry = new GeometryGroup();
			LineGeometry figure = new LineGeometry() { StartPoint = startPoint, EndPoint = endPoint };
			geometry.Children.Add(figure);
			return geometry;
		}
		protected internal override void CalculateLayout(IRefinedSeries refinedSeries) {
			Item.IndicatorGeometry = null;
			RegressionLineCalculator calculator = new RegressionLineCalculator();
			double x1 = ((IAxisData)XYSeries.ActualAxisX).VisualRange.Min;
			double x2 = ((IAxisData)XYSeries.ActualAxisX).VisualRange.Max;
			double rangeYMin = ((IAxisData)XYSeries.ActualAxisY).VisualRange.Min;
			double rangeYMax = ((IAxisData)XYSeries.ActualAxisY).VisualRange.Max;
			if (!refinedSeries.SeriesView.IsCorrectValueLevel((ValueLevelInternal)ValueLevel))
				return;
			Tuple<GRealPoint2D, GRealPoint2D> linePoints = calculator.Calculate(refinedSeries, x1, x2, (ValueLevelInternal)ValueLevel);
			if (linePoints == null || (linePoints.Item1.Y < rangeYMin && linePoints.Item2.Y < rangeYMin) || (linePoints.Item1.Y > rangeYMax && linePoints.Item2.Y > rangeYMax))
				return;
			double y1 = linePoints.Item1.Y;
			double y2 = linePoints.Item2.Y;
			PaneMapping mapping = new PaneMapping(Pane, XYSeries);
			startPoint = mapping.GetDiagramPoint(x1, y1);
			endPoint = mapping.GetDiagramPoint(x2, y2);
			Item.IndicatorGeometry = new IndicatorGeometry(this);
		}
		protected override void Assign(Indicator indicator) {
			base.Assign(indicator);
			RegressionLine regressionLine = indicator as RegressionLine;
			if (regressionLine != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, regressionLine, ValueLevelProperty);
			}
		}
		protected override Indicator CreateInstance() {
			return new RegressionLine();
		}
	}
}
