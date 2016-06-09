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
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class TrendLine : FinancialIndicator {
		public static readonly DependencyProperty ExtrapolateToInfinityProperty = DependencyPropertyManager.Register("ExtrapolateToInfinity",
		  typeof(bool), typeof(TrendLine), new PropertyMetadata(true, ChartElementHelper.Update));
		GRealLine2D indicatorLine;
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool ExtrapolateToInfinity {
			get { return (bool)GetValue(ExtrapolateToInfinityProperty); }
			set { SetValue(ExtrapolateToInfinityProperty, value); }
		}
		public TrendLine() {
			DefaultStyleKey = typeof(TrendLine);
		}
		protected override void Assign(Indicator indicator) {
			base.Assign(indicator);
			TrendLine trendLine = indicator as TrendLine;
			if (trendLine != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, trendLine, ExtrapolateToInfinityProperty);
			}
		}
		protected override Indicator CreateInstance() {
			return new TrendLine();
		}
		protected internal override void CalculateLayout(IRefinedSeries refinedSeries) {
			base.CalculateLayout(refinedSeries);
			AxisScaleTypeMap axisXScaleTypeMap = ((IAxisData)XYSeries.ActualAxisX).AxisScaleTypeMap;
			CultureInfo cultureInfo = CultureInfo.InvariantCulture;
			double maxAxisXValue = ((IAxisData)XYSeries.ActualAxisX).VisualRange.Max;
			TrendLineCalculator calculator = new TrendLineCalculator();
			this.indicatorLine = calculator.Calculate(refinedSeries, axisXScaleTypeMap, cultureInfo,
													  Argument1, (ValueLevelInternal)ValueLevel1,
													  Argument2, (ValueLevelInternal)ValueLevel2, 
													  ExtrapolateToInfinity, maxAxisXValue);
			Item.IndicatorGeometry = new IndicatorGeometry(this);
		}
		public override Geometry CreateGeometry() {
			PaneMapping paneMapping = new PaneMapping(Pane, XYSeries);
			Geometry geometry = new LineGeometry() {
				StartPoint = paneMapping.GetDiagramPoint(this.indicatorLine.Start),
				EndPoint = paneMapping.GetDiagramPoint(this.indicatorLine.End)
			};
			return geometry;
		}
	}
}
