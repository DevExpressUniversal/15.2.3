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
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class FibonacciRetracement : FibonacciIndicator{
		const int leftOrRightLabelIndent = 1; 
		public static readonly DependencyProperty ShowAdditionalLevelsProperty = DependencyPropertyManager.Register("ShowAdditionalLevels",
		  typeof(bool), typeof(FibonacciRetracement), new PropertyMetadata(false, ChartElementHelper.Update));
		List<FibonacciLine> fibonacciLines;
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool ShowAdditionalLevels {
			get { return (bool)GetValue(ShowAdditionalLevelsProperty); }
			set { SetValue(ShowAdditionalLevelsProperty, value); }
		}
		public FibonacciRetracement() {
			DefaultStyleKey = typeof(FibonacciRetracement);
		}
		public override Geometry CreateGeometry() {
			GeometryGroup geometry = new GeometryGroup();
			foreach (FibonacciLine line in this.fibonacciLines) {
				geometry.Children.Add(new LineGeometry() { StartPoint = line.ScreenStart.ToPoint(), EndPoint = line.ScreenEnd.ToPoint() });
			}
			return geometry;
		}
		protected internal override void CalculateLayout(IRefinedSeries refinedSeries) {
			base.CalculateLayout(refinedSeries);
			AxisScaleTypeMap axisXScaleTypeMap = ((IAxisData)XYSeries.ActualAxisX).AxisScaleTypeMap;
			CultureInfo cultureInfo = CultureInfo.InvariantCulture;
			double maxAxisXValue = ((IAxisData)XYSeries.ActualAxisX).VisualRange.Max;
			FibonacciRetracementCalculator calculator = new FibonacciRetracementCalculator();
			this.fibonacciLines = calculator.Calculate(refinedSeries, axisXScaleTypeMap, cultureInfo,
													   Argument1, (ValueLevelInternal)ValueLevel1,
													   Argument2, (ValueLevelInternal)ValueLevel2,
													   maxAxisXValue, GetFibonacciLevels());
			if (!calculator.Calculated)
				return;
			PaneMapping paneMapping = new PaneMapping(Pane, XYSeries);
			foreach (FibonacciLine line in fibonacciLines) {
				Point point1 = paneMapping.GetDiagramPoint(line.Start);
				Point point2 = paneMapping.GetDiagramPoint(line.End);
				MathUtils.CorrectSmoothLine(Item.LineStyle.Thickness, ref point1, ref point2);
				line.ScreenStart = point1.ToGRealPoint2D();
				line.ScreenEnd = point2.ToGRealPoint2D();
			}
			Item.IndicatorGeometry = new IndicatorGeometry(this);
		}
		protected override IList<double> GetFibonacciLevels() {
			IList<double> list = base.GetFibonacciLevels();
			list.Add(0.0);
			list.Add(1.0);
			if (ShowAdditionalLevels) {
				list.Add(1.618);
				list.Add(2.618);
				list.Add(4.236);
			}
			return list;
		}
		protected override Indicator CreateInstance() {
			return new FibonacciRetracement();
		}
		protected override void Assign(Indicator indicator) {
			base.Assign(indicator);
			FibonacciRetracement fibonacciRetracement = indicator as FibonacciRetracement;
			if (fibonacciRetracement != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, fibonacciRetracement, ShowAdditionalLevelsProperty);
			}
		}
		protected internal override void CreateLabelItems() {
			base.CreateLabelItems();
			if (fibonacciLines == null)
				return;
			LabelItems = new List<IndicatorLabelItem>();
			foreach (FibonacciLine line in fibonacciLines) {
				string text = Math.Round(line.Level*100, 3).ToString() + "%";
				IndicatorLabelItem labelItem = new IndicatorLabelItem(this, text, line);
				LabelItems.Add(labelItem);
			}
		}
		protected internal override IndicatorLabelLayout CalculateLabelLayout(IndicatorLabelItem labelItem, Size labelSize, object dataForLayoutCalculation) {
			FibonacciLine fibonacciLine = dataForLayoutCalculation as FibonacciLine;
			if (fibonacciLine == null)
				return null;
			Point centerLabelLocation = fibonacciLine.ScreenEnd.ToPoint();
			double additionalOffset = Item.LineStyle.Thickness / 2;
			if (Pane.Rotated)
				if (Pane.Diagram.ActualAxisX.IsReversed)
					centerLabelLocation += new Vector( labelSize.Height / 2, labelSize.Width / 2 + leftOrRightLabelIndent + additionalOffset);
				else
					centerLabelLocation += new Vector(-labelSize.Height / 2, labelSize.Width / 2 + leftOrRightLabelIndent + additionalOffset);
			else
				if (Pane.Diagram.ActualAxisX.IsReversed)
					centerLabelLocation += new Vector( labelSize.Width / 2 + leftOrRightLabelIndent, labelSize.Height / 2 + additionalOffset);
				else
					centerLabelLocation += new Vector(-labelSize.Width / 2 - leftOrRightLabelIndent, labelSize.Height / 2 + additionalOffset);
			IndicatorLabelLayout layout = new IndicatorLabelLayout(centerLabelLocation, labelSize);
			return layout;
		}
	}
}
