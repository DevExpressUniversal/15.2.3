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
using System.Windows.Forms;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Design {
	public partial class IndicatorTypeForm : XtraForm {
		readonly XYDiagram2DSeriesViewBase seriesView;
		public Indicator Indicator {
			get {
				GalleryItem selected = gcIndicators.Gallery.GetCheckedItem();
				Indicator indicator = null;
				var selectedIndicatorType = selected.Tag as Type;
				if (selectedIndicatorType != null)
					indicator = (Indicator)Activator.CreateInstance(selectedIndicatorType);
				if (selected.Tag is FibonacciIndicatorKind)
					indicator = new FibonacciIndicator((FibonacciIndicatorKind)selected.Tag);
				if (indicator != null)
					indicator.Name = seriesView.Indicators.GenerateName(indicator.GetType().Name + " ");
				return indicator;
			}
		}
		public IndicatorTypeForm(XYDiagram2DSeriesViewBase view) {
			InitializeComponent();
			DialogResult = DialogResult.Cancel;
			this.seriesView = view;
			GalleryControlGallery gallery = gcIndicators.Gallery;
			GalleryItemGroup generalGroup = gallery.Groups[0];
			GalleryItemGroup fibonacciIndicatorsGroup = gallery.Groups[1];
			GalleryItemGroup movingAverageGroup = gallery.Groups[2];
			GalleryItemGroup oscillatorsGroup = gallery.Groups[3];
			GalleryItemGroup trendIndicatorsGroup = gallery.Groups[4];
			FillGeneralGroup(generalGroup);
			FillFibonacciGroup(fibonacciIndicatorsGroup);
			FillMovingAverageGroup(movingAverageGroup);
			FillOscillatorsGroup(oscillatorsGroup);
			FillTrendIndicatorsGroup(trendIndicatorsGroup);
		}
		void FillGeneralGroup(GalleryItemGroup generalGroup) {
			var trendLineGalleryItem = new GalleryItem() {
				Caption = ChartLocalizer.GetString(ChartStringId.IndTrendLine),
				Tag = typeof(TrendLine)
			};
			generalGroup.Items.Add(trendLineGalleryItem);
			var regressionLine = new GalleryItem() {
				Caption = ChartLocalizer.GetString(ChartStringId.IndRegressionLine),
				Tag = typeof(RegressionLine)
			};
			generalGroup.Items.Add(regressionLine);
			var medianPrice = new GalleryItem() {
				Caption = ChartLocalizer.GetString(ChartStringId.IndMedianPrice),
				Tag = typeof(MedianPrice)
			};
			generalGroup.Items.Add(medianPrice);
			var typicalPrice = new GalleryItem() {
				Caption = ChartLocalizer.GetString(ChartStringId.IndTypicalPrice),
				Tag = typeof(TypicalPrice)
			};
			generalGroup.Items.Add(typicalPrice);
			var weightedClose = new GalleryItem() {
				Caption = ChartLocalizer.GetString(ChartStringId.IndWeightedClose),
				Tag = typeof(WeightedClose)
			};
			generalGroup.Items.Add(weightedClose);
		}
		void FillFibonacciGroup(GalleryItemGroup fibonacciGroup) {
			var fans = new GalleryItem() {
				Caption = ChartLocalizer.GetString(ChartStringId.FibonacciFans),
				Tag = FibonacciIndicatorKind.FibonacciFans
			};
			fibonacciGroup.Items.Add(fans);
			var arcs = new GalleryItem() {
				Caption = ChartLocalizer.GetString(ChartStringId.FibonacciArcs),
				Tag = FibonacciIndicatorKind.FibonacciArcs
			};
			fibonacciGroup.Items.Add(arcs);
			var retracement = new GalleryItem() {
				Caption = ChartLocalizer.GetString(ChartStringId.FibonacciRetracement),
				Tag = FibonacciIndicatorKind.FibonacciRetracement
			};
			fibonacciGroup.Items.Add(retracement);
		}
		void FillMovingAverageGroup(GalleryItemGroup movingAverageGroup) {
			var simple = new GalleryItem() {
				Caption = ChartLocalizer.GetString(ChartStringId.IndSimpleMovingAverage),
				Tag = typeof(SimpleMovingAverage)
			};
			movingAverageGroup.Items.Add(simple);
			var exponential = new GalleryItem() {
				Caption = ChartLocalizer.GetString(ChartStringId.IndExponentialMovingAverage),
				Tag = typeof(ExponentialMovingAverage)
			};
			movingAverageGroup.Items.Add(exponential);
			var triangular = new GalleryItem() {
				Caption = ChartLocalizer.GetString(ChartStringId.IndTriangularMovingAverage),
				Tag = typeof(TriangularMovingAverage)
			};
			movingAverageGroup.Items.Add(triangular);
			var weighted = new GalleryItem() {
				Caption = ChartLocalizer.GetString(ChartStringId.IndWeightedMovingAverage),
				Tag = typeof(WeightedMovingAverage)
			};
			movingAverageGroup.Items.Add(weighted);
			var tema = new GalleryItem() {
				Caption = ChartLocalizer.GetString(ChartStringId.IndTripleExponentialMovingAverageTema),
				Tag = typeof(TripleExponentialMovingAverageTema)
			};
			movingAverageGroup.Items.Add(tema);
		}
		void FillOscillatorsGroup(GalleryItemGroup oscillatorsGroup) {
			var atr = new GalleryItem() {
				Caption = ChartLocalizer.GetString(ChartStringId.IndAverageTrueRange),
				Tag = typeof(AverageTrueRange)
			};
			oscillatorsGroup.Items.Add(atr);
			var cho = new GalleryItem() {
				Caption = ChartLocalizer.GetString(ChartStringId.IndChaikinsVolatility),
				Tag = typeof(ChaikinsVolatility)
			};
			oscillatorsGroup.Items.Add(cho);
			var cci = new GalleryItem() {
				Caption = ChartLocalizer.GetString(ChartStringId.IndCommodityChannelIndex),
				Tag = typeof(CommodityChannelIndex)
			};
			oscillatorsGroup.Items.Add(cci);
			var dpo = new GalleryItem() {
				Caption = ChartLocalizer.GetString(ChartStringId.IndDetrendedPriceOscillator),
				Tag = typeof(DetrendedPriceOscillator)
			};
			oscillatorsGroup.Items.Add(dpo);
			var macd = new GalleryItem() {
				Caption = ChartLocalizer.GetString(ChartStringId.IndMovingAverageConvergenceDivergence),
				Tag = typeof(MovingAverageConvergenceDivergence)
			};
			oscillatorsGroup.Items.Add(macd);
			var rateOfChange = new GalleryItem() {
				Caption = ChartLocalizer.GetString(ChartStringId.IndRateOfChange),
				Tag = typeof(RateOfChange)
			};
			oscillatorsGroup.Items.Add(rateOfChange);
			var rsi = new GalleryItem() {
				Caption = ChartLocalizer.GetString(ChartStringId.IndRelativeStrengthIndex),
				Tag = typeof(RelativeStrengthIndex)
			};
			oscillatorsGroup.Items.Add(rsi);
			var williamsR = new GalleryItem() {
				Caption = ChartLocalizer.GetString(ChartStringId.IndWilliamsR),
				Tag = typeof(WilliamsR)
			};
			oscillatorsGroup.Items.Add(williamsR);
		}
		void FillTrendIndicatorsGroup(GalleryItemGroup trendIndicatorsGroup) {
			var bb = new GalleryItem() {
				Caption = ChartLocalizer.GetString(ChartStringId.IndBollingerBands),
				Tag = typeof(BollingerBands)
			};
			trendIndicatorsGroup.Items.Add(bb);
			var mi = new GalleryItem() {
				Caption = ChartLocalizer.GetString(ChartStringId.IndMassIndex),
				Tag = typeof(MassIndex)
			};
			trendIndicatorsGroup.Items.Add(mi);
			var stdDev = new GalleryItem() {
				Caption = ChartLocalizer.GetString(ChartStringId.IndStandardDeviation),
				Tag = typeof(StandardDeviation)
			};
			trendIndicatorsGroup.Items.Add(stdDev);
		}
		void btnOk_Click(object sender, EventArgs e) {
			if (Indicator != null)
				DialogResult = DialogResult.OK;
			Hide();
		}
		void btnCancel_Click(object sender, EventArgs e) {
			Hide();
		}
		void gcIndicators_Gallery_ItemDoubleClick(object sender, GalleryItemClickEventArgs e) {
			if (Indicator != null)
				DialogResult = DialogResult.OK;
			Hide();
		}
	}
}
