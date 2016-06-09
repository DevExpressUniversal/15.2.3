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
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class CandleStickSeries2D : FinancialSeries2D {
		public static readonly DependencyProperty CandleWidthProperty = DependencyPropertyManager.Register("CandleWidth", 
			typeof(double), typeof(CandleStickSeries2D),  new PropertyMetadata(0.5, ChartElementHelper.Update), CandleWidthValidation);
		public static readonly DependencyProperty ModelProperty = DependencyPropertyManager.Register("Model", 
			typeof(CandleStick2DModel), typeof(CandleStickSeries2D), new PropertyMetadata(ChartElementHelper.Update));
		static bool CandleWidthValidation(object candleWidth) {
			return (double)candleWidth > 0;
		}
		protected override double BarWidth { get { return CandleWidth; } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CandleStickSeries2DCandleWidth"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public double CandleWidth {
			get { return (double)GetValue(CandleWidthProperty); }
			set { SetValue(CandleWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CandleStickSeries2DModel"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public CandleStick2DModel Model {
			get { return (CandleStick2DModel)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}
		public CandleStickSeries2D() {
			DefaultStyleKey = typeof(CandleStickSeries2D);
		}
		Geometry CreateCandleStick(Point candleStickLeftTop, bool isInvertedCandle) {
			GeometryGroup candleStickGeometry = new GeometryGroup();
			candleStickGeometry.Children.Add(new RectangleGeometry() { Rect = new Rect(candleStickLeftTop.X + 2, candleStickLeftTop.Y, 1, 2) });
			candleStickGeometry.Children.Add(new RectangleGeometry() { Rect = new Rect(candleStickLeftTop.X + 2, candleStickLeftTop.Y + 8, 1, 2) });
			Rect candleRect = new Rect(candleStickLeftTop.X, candleStickLeftTop.Y + 2, 5, 6);
			if (isInvertedCandle) {
				candleStickGeometry.Children.Add(new RectangleGeometry() { Rect = candleRect });
			}
			else {
				candleStickGeometry.Children.Add(new RectangleGeometry() { Rect = new Rect(candleRect.Left, candleRect.Top, candleRect.Width, 1) });
				candleStickGeometry.Children.Add(new RectangleGeometry() { Rect = new Rect(candleRect.Left, candleRect.Bottom - 1, candleRect.Width, 1) });
				candleStickGeometry.Children.Add(new RectangleGeometry() { Rect = new Rect(candleRect.Left, candleRect.Top + 1, 1, candleRect.Height - 2) });
				candleStickGeometry.Children.Add(new RectangleGeometry() { Rect = new Rect(candleRect.Right - 1, candleRect.Top + 1, 1, candleRect.Height - 2) });
			}
			return candleStickGeometry;
		}
		protected override Series CreateObjectForClone() {
			return new CandleStickSeries2D();
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			CandleStickSeries2D candleStickSeries2D = series as CandleStickSeries2D;
			if (candleStickSeries2D != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, candleStickSeries2D, CandleWidthProperty);
				if (CopyPropertyValueHelper.IsValueSet(candleStickSeries2D, ModelProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, candleStickSeries2D, ModelProperty))
						Model = candleStickSeries2D.Model.CloneModel();
			}
		}
		protected override int CalculateSeriesPointWidth(IAxisMapping mapping) {
			return Math.Max(1, mapping.GetRoundedInterval(CandleWidth));
		}
		protected override void CorrectOpenClosePositions(ref double openPosition, ref double closePosition) {
			Render2DHelper.CorrectBounds(ref openPosition, ref closePosition);
		}
		protected override FinancialSeries2DPointLayout CreateFinancialSeriesPointLayout(IFinancialPoint refinedPoint, Rect viewport, Rect bounds, FinancialLayoutPortions layoutPortions) {
			return new CandleStickSeries2DPointLayout(viewport, bounds, layoutPortions, refinedPoint != null ? refinedPoint.Close < refinedPoint.Open : false);
		}
		protected internal override PointModel GetModel(RangeValueLevel valueLevel) {
			return Model;
		} 
	}
}
