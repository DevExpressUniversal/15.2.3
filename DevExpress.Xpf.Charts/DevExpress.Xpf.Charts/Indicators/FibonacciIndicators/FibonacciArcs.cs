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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class FibonacciArcs : FibonacciIndicator {
		const double horisontalIndent = 1.0;
		public static readonly DependencyProperty ShowLevel100Property = DependencyPropertyManager.Register("ShowLevel100",
		   typeof(bool), typeof(FibonacciArcs), new PropertyMetadata(false, ChartElementHelper.Update));
		List<FibonacciCircle> fibonacciCircles;
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool ShowLevel100 {
			get { return (bool)GetValue(ShowLevel100Property); }
			set { SetValue(ShowLevel100Property, value); }
		}
		public FibonacciArcs() {
			DefaultStyleKey = typeof(FibonacciArcs);
		}
		public override Geometry CreateGeometry() {
			GeometryGroup geometryGroup = new GeometryGroup();
			foreach(FibonacciCircle circle in fibonacciCircles) {
				EllipseGeometry circleGeometry = new EllipseGeometry() { Center = circle.ScreenCenter.ToPoint(), RadiusX = circle.ScreenRadius, RadiusY = circle.ScreenRadius };
				geometryGroup.Children.Add(circleGeometry);
			}
			return geometryGroup;
		}
		protected internal override void CalculateLayout(IRefinedSeries refinedSeries) {
			base.CalculateLayout(refinedSeries);
			AxisScaleTypeMap axisXScaleTypeMap = ((IAxisData)XYSeries.ActualAxisX).AxisScaleTypeMap;
			CultureInfo cultureInfo = CultureInfo.InvariantCulture;
			double maxAxisXValue = ((IAxisData)XYSeries.ActualAxisX).VisualRange.Max;
			FibonacciArcsCalculator calculator = new FibonacciArcsCalculator();
			this.fibonacciCircles = calculator.Calculate(refinedSeries, axisXScaleTypeMap, cultureInfo,
													   Argument1, (ValueLevelInternal)ValueLevel1,
													   Argument2, (ValueLevelInternal)ValueLevel2,
													   maxAxisXValue, GetFibonacciLevels());
			if (!calculator.Calculated)
				return;
			PaneMapping paneMapping = new PaneMapping(Pane, XYSeries);
			foreach (FibonacciCircle circle in fibonacciCircles) {
				Point screenCenter = paneMapping.GetDiagramPoint(circle.Center);
				circle.ScreenCenter = screenCenter.ToGRealPoint2D();
				circle.ScreenPointInCircle = paneMapping.GetDiagramPoint(circle.PointInCircle).ToGRealPoint2D();
				circle.ScreenRadius = MathUtils.CalcDistance(screenCenter, circle.ScreenPointInCircle.ToPoint());
			}
			Item.IndicatorGeometry = new IndicatorGeometry(this);
		}
		protected internal override void CreateLabelItems() {
			base.CreateLabelItems();
			if(fibonacciCircles == null)
				return;
			LabelItems = new List<IndicatorLabelItem>();
			foreach(FibonacciCircle circle in fibonacciCircles) {
				string text = Math.Round(circle.Level, 3).ToString();
				IndicatorLabelItem labelItem = new IndicatorLabelItem(this, text, circle);
				LabelItems.Add(labelItem);
			}
		}
		protected internal override IndicatorLabelLayout CalculateLabelLayout(IndicatorLabelItem labelItem, Size labelSize, object dataForLayoutCalculation) {
			FibonacciCircle fibonacciCircle = dataForLayoutCalculation as FibonacciCircle;
			if(fibonacciCircle == null)
				return null;
			Vector directingVector = new Vector(0, fibonacciCircle.ScreenPointInCircle.Y - fibonacciCircle.ScreenCenter.Y);
			directingVector.Normalize();
			double offset = Item.LineStyle.Thickness / 2;
			if(Pane.Rotated)
				offset += labelSize.Width / 2 + horisontalIndent;
			else
				offset += labelSize.Height / 2;
			Point centerLabelPoint = fibonacciCircle.ScreenCenter.ToPoint() + directingVector * (fibonacciCircle.ScreenRadius + offset);
			double additionalOffset = Item.LineStyle.Thickness / 2;
			return new IndicatorLabelLayout(centerLabelPoint, labelSize);
		}
		protected override IList<double> GetFibonacciLevels() {
			IList<double> levels = base.GetFibonacciLevels();
			if(ShowLevel100)
				levels.Add(1);
			return levels;
		}
		protected override Indicator CreateInstance() {
			return new FibonacciArcs();
		}
		protected override void Assign(Indicator indicator) {
			base.Assign(indicator);
			FibonacciArcs fibonacciArcs = indicator as FibonacciArcs;
			if (fibonacciArcs != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, fibonacciArcs, ShowLevel100Property);
			}
		}
	}
}
