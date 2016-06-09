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
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum StockType {
		Both,
		Open,
		Close
	}
	public class StockSeries2D : FinancialSeries2D {
		public static readonly DependencyProperty LevelLineLengthProperty = DependencyPropertyManager.Register("LevelLineLength", 
			typeof(double), typeof(StockSeries2D), new PropertyMetadata(0.25, ChartElementHelper.Update), LevelLineLengthValidation);
		public static readonly DependencyProperty ShowOpenCloseProperty = DependencyPropertyManager.Register("ShowOpenClose", 
			typeof(StockType), typeof(StockSeries2D), new PropertyMetadata(StockType.Both, ChartElementHelper.Update));
		public static readonly DependencyProperty ModelProperty = DependencyPropertyManager.Register("Model",
			typeof(Stock2DModel), typeof(StockSeries2D), new PropertyMetadata(ChartElementHelper.Update));
		protected override double BarWidth { get { return LevelLineLength * 2; } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("StockSeries2DLevelLineLength"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public double LevelLineLength {
			get { return (double)GetValue(LevelLineLengthProperty); }
			set { SetValue(LevelLineLengthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("StockSeries2DShowOpenClose"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public StockType ShowOpenClose {
			get { return (StockType)GetValue(ShowOpenCloseProperty); }
			set { SetValue(ShowOpenCloseProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("StockSeries2DModel"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Stock2DModel Model {
			get { return (Stock2DModel)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}
		static bool LevelLineLengthValidation(object levelLineLength) {
			return (double)levelLineLength > 0;
		}
		protected internal override ToolTipPointDataToStringConverter ToolTipPointValuesConverter {
			get {
				bool showOpen = ShowOpenClose == StockType.Open || ShowOpenClose == StockType.Both;
				bool showClose = ShowOpenClose == StockType.Close || ShowOpenClose == StockType.Both;
				return new ToolTipFinancialValueToStringConverter(this, showOpen, showClose);
			}
		}
		public StockSeries2D() {
			DefaultStyleKey = typeof(StockSeries2D);
		}
		protected override Series CreateObjectForClone() {
			return new StockSeries2D();
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			StockSeries2D stockSeries2D = series as StockSeries2D;
			if (stockSeries2D != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, stockSeries2D, LevelLineLengthProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, stockSeries2D, ShowOpenCloseProperty);
				if (CopyPropertyValueHelper.IsValueSet(stockSeries2D, ModelProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, stockSeries2D, ModelProperty))
						Model = stockSeries2D.Model.CloneModel();
			}
		}
		protected internal override PointModel GetModel(RangeValueLevel valueLevel) {
			return Model;
		}
		protected override int CalculateSeriesPointWidth(IAxisMapping mapping) {
			return mapping.GetRoundedInterval(LevelLineLength) * 2 + 1;
		}
		protected override void CorrectOpenClosePositions(ref double openPosition, ref double closePosition) {
			openPosition = MathUtils.StrongRound(openPosition) + 0.5;
			closePosition = MathUtils.StrongRound(closePosition) + 0.5;
		}
		protected override FinancialSeries2DPointLayout CreateFinancialSeriesPointLayout(IFinancialPoint refinedPoint, Rect viewport, Rect bounds, FinancialLayoutPortions layoutPortions) {
			return new StockSeries2DPointLayout(viewport, bounds, layoutPortions, ShowOpenClose);
		}
	}
}
