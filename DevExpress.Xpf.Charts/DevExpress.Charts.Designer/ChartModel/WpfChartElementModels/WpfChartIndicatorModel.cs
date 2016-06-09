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

using DevExpress.Xpf.Charts;
using System.Collections.Generic;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Charts.Designer.Native {
	public class WpfChartIndicatorModel : ChartModelElement {
		List<ValueLevelComboBoxPresentation> valueLevels = new List<ValueLevelComboBoxPresentation>();
		WpfChartPaneModel pane;
		WpfChartAxisModel axisY;
		string IndicatorTypeName {
			get {
				if (Indicator == null)
					return "null";
				else
					return typeof(Indicator).Name;
			}
		}
		public WpfChartDiagramModel Diagram {
			get { return (WpfChartDiagramModel)GetParent<WpfChartDiagramModel>(); }
		}
		public override IEnumerable<ChartModelElement> Children {
			get { return null; }
		}
		public Indicator Indicator {
			get { return (Indicator)ChartElement; }
		}
		public string Name {
			get {
				if (!string.IsNullOrEmpty(Indicator.LegendText))
					return Indicator.LegendText;
				else
					return ChartDesignerLocalizer.GetLocalizedIndicatorTypeName(Indicator.GetType());
			}
		}
		public Brush Brush {
			get { return Indicator.Brush ?? GetActualIndicatorBrush(); }
			set {
				if (Indicator.Brush != value) {
					Indicator.Brush = value;
					OnPropertyChanged("Brush");
				}
			}
		}
		public int Thickness {
			get {
				if (Indicator.LineStyle != null)
					return Indicator.LineStyle.Thickness;
				else
					return (int)LineStyle.ThicknessProperty.GetMetadata(typeof(LineStyle)).DefaultValue;
			}
			set {
				if (Indicator.LineStyle != null && Indicator.LineStyle.Thickness != value) {
					Indicator.LineStyle.Thickness = value;
					OnPropertyChanged("Thickness");
				}
			}
		}
		public string LegendText {
			get { return Indicator.LegendText; }
			set {
				if (Indicator.LegendText != value) {
					Indicator.LegendText = value;
					OnPropertyChanged("LegendText");
				}
			}
		}
		public bool ShowInLegend {
			get { return Indicator.ShowInLegend; }
			set {
				if (Indicator.ShowInLegend != value) {
					Indicator.ShowInLegend = value;
					OnPropertyChanged("ShowInLegend");
				}
			}
		}
		public ValueLevel ValueLevel {
			get {
				if (Indicator is RegressionLine)
					return ((RegressionLine)Indicator).ValueLevel;
				else if (Indicator is MovingAverage)
					return ((MovingAverage)Indicator).ValueLevel;
				else if (Indicator is DetrendedPriceOscillator)
					return ((DetrendedPriceOscillator)Indicator).ValueLevel;
				else if (Indicator is BollingerBands)
					return ((BollingerBands)Indicator).ValueLevel;
				else if (Indicator is MovingAverageConvergenceDivergence)
					return ((MovingAverageConvergenceDivergence)Indicator).ValueLevel;
				else if (Indicator is RateOfChange)
					return ((RateOfChange)Indicator).ValueLevel;
				else if (Indicator is RelativeStrengthIndex)
					return ((RelativeStrengthIndex)Indicator).ValueLevel;
				else if (Indicator is StandardDeviation)
					return ((StandardDeviation)Indicator).ValueLevel;
				else if (Indicator is TripleExponentialMovingAverageTrix)
					return ((TripleExponentialMovingAverageTrix)Indicator).ValueLevel;
				else
					return ValueLevel.Value;
			}
			set {
				if (Indicator is RegressionLine) {
					if (((RegressionLine)Indicator).ValueLevel != value) {
						((RegressionLine)Indicator).ValueLevel = value;
						OnPropertyChanged("ValueLevel");
					}
				}
				else if (Indicator is MovingAverage) {
					if (((MovingAverage)Indicator).ValueLevel != value) {
						((MovingAverage)Indicator).ValueLevel = value;
						OnPropertyChanged("ValueLevel");
					}
				}
				else if (Indicator is DetrendedPriceOscillator) {
					if (((DetrendedPriceOscillator)Indicator).ValueLevel != value) {
						((DetrendedPriceOscillator)Indicator).ValueLevel = value;
						OnPropertyChanged("ValueLevel");
					}
				}
				else if (Indicator is BollingerBands) {
					if (((BollingerBands)Indicator).ValueLevel != value) {
						((BollingerBands)Indicator).ValueLevel = value;
						OnPropertyChanged("ValueLevel");
					}
				}
				else if (Indicator is MovingAverageConvergenceDivergence) {
					if (((MovingAverageConvergenceDivergence)Indicator).ValueLevel != value) {
						((MovingAverageConvergenceDivergence)Indicator).ValueLevel = value;
						OnPropertyChanged("ValueLevel");
					}
				}
				else if (Indicator is RateOfChange) {
					if (((RateOfChange)Indicator).ValueLevel != value) {
						((RateOfChange)Indicator).ValueLevel = value;
						OnPropertyChanged("ValueLevel");
					}
				}
				else if (Indicator is RelativeStrengthIndex) {
					if (((RelativeStrengthIndex)Indicator).ValueLevel != value) {
						((RelativeStrengthIndex)Indicator).ValueLevel = value;
						OnPropertyChanged("ValueLevel");
					}
				}
				else if (Indicator is StandardDeviation) {
					if (((StandardDeviation)Indicator).ValueLevel != value) {
						((StandardDeviation)Indicator).ValueLevel = value;
						OnPropertyChanged("ValueLevel");
					}
				}
				else if (Indicator is TripleExponentialMovingAverageTrix) {
					if (((TripleExponentialMovingAverageTrix)Indicator).ValueLevel != value) {
						((TripleExponentialMovingAverageTrix)Indicator).ValueLevel = value;
						OnPropertyChanged("ValueLevel");
					}
				}
				else
					WriteUnsupportedPropertySetWarning("ValueLevel", IndicatorTypeName);
			}
		}
		public object Argument1 {
			get {
				if (Indicator is FinancialIndicator)
					return ((FinancialIndicator)Indicator).Argument1;
				else
					return null;
			}
			set {
				if (Indicator is FinancialIndicator) {
					if (((FinancialIndicator)Indicator).Argument1 != value) {
						((FinancialIndicator)Indicator).Argument1 = value;
						OnPropertyChanged("Argument1");
					}
				}
				else
					WriteUnsupportedPropertySetWarning("Argument1", IndicatorTypeName);
			}
		}
		public object Argument2 {
			get {
				if (Indicator is FinancialIndicator)
					return ((FinancialIndicator)Indicator).Argument2;
				else
					return null;
			}
			set {
				if (Indicator is FinancialIndicator) {
					if (((FinancialIndicator)Indicator).Argument2 != value) {
						((FinancialIndicator)Indicator).Argument2 = value;
						OnPropertyChanged("Argument2");
					}
				}
				else
					WriteUnsupportedPropertySetWarning("Argument2", IndicatorTypeName);
			}
		}
		public ValueLevel ValueLevel1 {
			get {
				if (Indicator is FinancialIndicator)
					return ((FinancialIndicator)Indicator).ValueLevel1;
				else
					return ValueLevel.Value;
			}
			set {
				if (Indicator is FinancialIndicator) {
					if (((FinancialIndicator)Indicator).ValueLevel1 != value) {
						((FinancialIndicator)Indicator).ValueLevel1 = value;
						OnPropertyChanged("ValueLevel1");
					}
				}
				else
					WriteUnsupportedPropertySetWarning("ValueLevel1", IndicatorTypeName);
			}
		}
		public ValueLevel ValueLevel2 {
			get {
				if (Indicator is FinancialIndicator)
					return ((FinancialIndicator)Indicator).ValueLevel2;
				else
					return ValueLevel.Value;
			}
			set {
				if (Indicator is FinancialIndicator) {
					if (((FinancialIndicator)Indicator).ValueLevel2 != value) {
						((FinancialIndicator)Indicator).ValueLevel2 = value;
						OnPropertyChanged("ValueLevel2");
					}
				}
				else
					WriteUnsupportedPropertySetWarning("ValueLevel2", IndicatorTypeName);
			}
		}
		public bool ExtrapolateToInfinity {
			get {
				if (Indicator is TrendLine)
					return ((TrendLine)Indicator).ExtrapolateToInfinity;
				else
					return false;
			}
			set {
				if (Indicator is TrendLine) {
					if (((TrendLine)Indicator).ExtrapolateToInfinity != value) {
						((TrendLine)Indicator).ExtrapolateToInfinity = value;
						OnPropertyChanged("ExtrapolateToInfinity");
					}
				}
				else
					WriteUnsupportedPropertySetWarning("ExtrapolateToInfinity", IndicatorTypeName);
			}
		}
		public bool ShowLevel23_6 {
			get {
				if (Indicator is FibonacciIndicator)
					return ((FibonacciIndicator)Indicator).ShowLevel23_6;
				else
					return false;
			}
			set {
				if (Indicator is FibonacciIndicator) {
					if (((FibonacciIndicator)Indicator).ShowLevel23_6 != value) {
						((FibonacciIndicator)Indicator).ShowLevel23_6 = value;
						OnPropertyChanged("ShowLevel23_6");
					}
				}
				else
					WriteUnsupportedPropertySetWarning("ShowLevel23_6", IndicatorTypeName);
			}
		}
		public bool ShowLevel76_4 {
			get {
				if (Indicator is FibonacciIndicator)
					return ((FibonacciIndicator)Indicator).ShowLevel76_4;
				else
					return false;
			}
			set {
				if (Indicator is FibonacciIndicator) {
					if (((FibonacciIndicator)Indicator).ShowLevel76_4 != value) {
						((FibonacciIndicator)Indicator).ShowLevel76_4 = value;
						OnPropertyChanged("ShowLevel76_4");
					}
				}
				else
					WriteUnsupportedPropertySetWarning("ShowLevel76_4", IndicatorTypeName);
			}
		}
		public bool ShowLevel100 {
			get {
				if (Indicator is FibonacciArcs)
					return ((FibonacciArcs)Indicator).ShowLevel100;
				else
					return false;
			}
			set {
				if (Indicator is FibonacciArcs) {
					if (((FibonacciArcs)Indicator).ShowLevel100 != value) {
						((FibonacciArcs)Indicator).ShowLevel100 = value;
						OnPropertyChanged("ShowLevel100");
					}
				}
				else
					WriteUnsupportedPropertySetWarning("ShowLevel100", IndicatorTypeName);
			}
		}
		public bool ShowLevel0 {
			get {
				if (Indicator is FibonacciFans)
					return ((FibonacciFans)Indicator).ShowLevel0;
				else
					return false;
			}
			set {
				if (Indicator is FibonacciFans) {
					if (((FibonacciFans)Indicator).ShowLevel0 != value) {
						((FibonacciFans)Indicator).ShowLevel0 = value;
						OnPropertyChanged("ShowLevel0");
					}
				}
				else
					WriteUnsupportedPropertySetWarning("ShowLevel0", IndicatorTypeName);
			}
		}
		public bool ShowAdditionalLevels {
			get {
				if (Indicator is FibonacciRetracement)
					return ((FibonacciRetracement)Indicator).ShowAdditionalLevels;
				else
					return false;
			}
			set {
				if (Indicator is FibonacciRetracement) {
					if (((FibonacciRetracement)Indicator).ShowAdditionalLevels != value) {
						((FibonacciRetracement)Indicator).ShowAdditionalLevels = value;
						OnPropertyChanged("ShowAdditionalLevels");
					}
				}
				else
					WriteUnsupportedPropertySetWarning("ShowAdditionalLevels", IndicatorTypeName);
			}
		}
		public double EnvelopePercent {
			get {
				if (Indicator is MovingAverage)
					return ((MovingAverage)Indicator).EnvelopePercent;
				else
					return 0;
			}
			set {
				if (Indicator is MovingAverage) {
					if (((MovingAverage)Indicator).EnvelopePercent != value) {
						((MovingAverage)Indicator).EnvelopePercent = value;
						OnPropertyChanged("EnvelopePercent");
					}
				}
				else
					WriteUnsupportedPropertySetWarning("EnvelopePercent", IndicatorTypeName);
			}
		}
		public double StandardDeviationMultiplier {
			get {
				if (Indicator is BollingerBands)
					return ((BollingerBands)Indicator).StandardDeviationMultiplier;
				else
					return 0;
			}
			set {
				if (Indicator is BollingerBands) {
					if (((BollingerBands)Indicator).StandardDeviationMultiplier != value) {
						((BollingerBands)Indicator).StandardDeviationMultiplier = value;
						OnPropertyChanged("StandardDeviationMultiplier");
					}
				}
				else
					WriteUnsupportedPropertySetWarning("EnvelopePercent", IndicatorTypeName);
			}
		}
		public int PointsCount {
			get {
				if (Indicator is MovingAverage)
					return ((MovingAverage)Indicator).PointsCount;
				else if (Indicator is BollingerBands)
					return ((BollingerBands)Indicator).PointsCount;
				else if (Indicator is AverageTrueRange)
					return ((AverageTrueRange)Indicator).PointsCount;
				else if (Indicator is ChaikinsVolatility)
					return ((ChaikinsVolatility)Indicator).PointsCount;
				else if (Indicator is CommodityChannelIndex)
					return ((CommodityChannelIndex)Indicator).PointsCount;
				else if (Indicator is DetrendedPriceOscillator)
					return ((DetrendedPriceOscillator)Indicator).PointsCount;
				else if (Indicator is RateOfChange)
					return ((RateOfChange)Indicator).PointsCount;
				else if (Indicator is RelativeStrengthIndex)
					return ((RelativeStrengthIndex)Indicator).PointsCount;
				else if (Indicator is StandardDeviation)
					return ((StandardDeviation)Indicator).PointsCount;
				else if (Indicator is TripleExponentialMovingAverageTrix)
					return ((TripleExponentialMovingAverageTrix)Indicator).PointsCount;
				else if (Indicator is WilliamsR)
					return ((WilliamsR)Indicator).PointsCount;
				else
					return 0;
			}
			set {
				if (Indicator is MovingAverage && ((MovingAverage)Indicator).PointsCount != value) {
					((MovingAverage)Indicator).PointsCount = value;
					OnPropertyChanged("PointsCount");
				}
				else if (Indicator is BollingerBands && ((BollingerBands)Indicator).PointsCount != value) {
					((BollingerBands)Indicator).PointsCount = value;
					OnPropertyChanged("PointsCount");
				}
				else if (Indicator is AverageTrueRange && ((AverageTrueRange)Indicator).PointsCount != value) {
					((AverageTrueRange)Indicator).PointsCount = value;
					OnPropertyChanged("PointsCount");
				}
				else if (Indicator is ChaikinsVolatility && ((ChaikinsVolatility)Indicator).PointsCount != value) {
					((ChaikinsVolatility)Indicator).PointsCount = value;
					OnPropertyChanged("PointsCount");
				}
				else if (Indicator is CommodityChannelIndex && ((CommodityChannelIndex)Indicator).PointsCount != value) {
					((CommodityChannelIndex)Indicator).PointsCount = value;
					OnPropertyChanged("PointsCount");
				}
				else if (Indicator is DetrendedPriceOscillator && ((DetrendedPriceOscillator)Indicator).PointsCount != value) {
					((DetrendedPriceOscillator)Indicator).PointsCount = value;
					OnPropertyChanged("PointsCount");
				}
				else if (Indicator is RateOfChange && ((RateOfChange)Indicator).PointsCount != value) {
					((RateOfChange)Indicator).PointsCount = value;
					OnPropertyChanged("PointsCount");
				}
				else if (Indicator is RelativeStrengthIndex && ((RelativeStrengthIndex)Indicator).PointsCount != value) {
					((RelativeStrengthIndex)Indicator).PointsCount = value;
					OnPropertyChanged("PointsCount");
				}
				else if (Indicator is StandardDeviation && ((StandardDeviation)Indicator).PointsCount != value) {
					((StandardDeviation)Indicator).PointsCount = value;
					OnPropertyChanged("PointsCount");
				}
				else if (Indicator is TripleExponentialMovingAverageTrix && ((TripleExponentialMovingAverageTrix)Indicator).PointsCount != value) {
					((TripleExponentialMovingAverageTrix)Indicator).PointsCount = value;
					OnPropertyChanged("PointsCount");
				}
				else if (Indicator is WilliamsR && ((WilliamsR)Indicator).PointsCount != value) {
					((WilliamsR)Indicator).PointsCount = value;
					OnPropertyChanged("PointsCount");
				}
			}
		}
		public int SumPointsCount {
			get {
				if (Indicator is MassIndex)
					return ((MassIndex)Indicator).SumPointsCount;
				else
					return 0;
			}
			set {
				if (Indicator is MassIndex && ((MassIndex)Indicator).SumPointsCount != value) {
					((MassIndex)Indicator).SumPointsCount = value;
					OnPropertyChanged("SumPointsCount");
				}
				else
					WriteUnsupportedPropertySetWarning("SumPointsCount", IndicatorTypeName);
			}
		}
		public int MovingAveragePointsCount {
			get {
				if (Indicator is MassIndex)
					return ((MassIndex)Indicator).MovingAveragePointsCount;
				else
					return 0;
			}
			set {
				if (Indicator is MassIndex && ((MassIndex)Indicator).MovingAveragePointsCount != value) {
					((MassIndex)Indicator).MovingAveragePointsCount = value;
					OnPropertyChanged("MovingAveragePointsCount");
				}
				else
					WriteUnsupportedPropertySetWarning("MovingAveragePointsCount", IndicatorTypeName);
			}
		}
		public int LongPeriod {
			get {
				if (Indicator is MovingAverageConvergenceDivergence)
					return ((MovingAverageConvergenceDivergence)Indicator).LongPeriod;
				else
					return 0;
			}
			set {
				if (Indicator is MovingAverageConvergenceDivergence && ((MovingAverageConvergenceDivergence)Indicator).LongPeriod != value) {
					((MovingAverageConvergenceDivergence)Indicator).LongPeriod = value;
					OnPropertyChanged("LongPeriod");
				}
				else
					WriteUnsupportedPropertySetWarning("LongPeriod", IndicatorTypeName);
			}
		}
		public int ShortPeriod {
			get {
				if (Indicator is MovingAverageConvergenceDivergence)
					return ((MovingAverageConvergenceDivergence)Indicator).ShortPeriod;
				else
					return 0;
			}
			set {
				if (Indicator is MovingAverageConvergenceDivergence && ((MovingAverageConvergenceDivergence)Indicator).ShortPeriod != value) {
					((MovingAverageConvergenceDivergence)Indicator).ShortPeriod = value;
					OnPropertyChanged("ShortPeriod");
				}
				else
					WriteUnsupportedPropertySetWarning("ShortPeriod", IndicatorTypeName);
			}
		}
		public int SignalSmoothingPeriod {
			get {
				if (Indicator is MovingAverageConvergenceDivergence)
					return ((MovingAverageConvergenceDivergence)Indicator).SignalSmoothingPeriod;
				else
					return 0;
			}
			set {
				if (Indicator is MovingAverageConvergenceDivergence && ((MovingAverageConvergenceDivergence)Indicator).SignalSmoothingPeriod != value) {
					((MovingAverageConvergenceDivergence)Indicator).SignalSmoothingPeriod = value;
					OnPropertyChanged("SignalSmoothingPeriod");
				}
				else
					WriteUnsupportedPropertySetWarning("SignalSmoothingPeriod", IndicatorTypeName);
			}
		}
		public MovingAverageKind MovingAverageKind {
			get {
				if (Indicator is MovingAverage)
					return ((MovingAverage)Indicator).MovingAverageKind;
				else
					return MovingAverageKind.MovingAverage;
			}
			set {
				if (Indicator is MovingAverage) {
					if (((MovingAverage)Indicator).MovingAverageKind != value) {
						((MovingAverage)Indicator).MovingAverageKind = value;
						OnPropertyChanged("MovingAverageKind");
					}
				}
				else
					WriteUnsupportedPropertySetWarning("MovingAverageKind", IndicatorTypeName);
			}
		}
		public List<ValueLevelComboBoxPresentation> ValueLevels {
			get { return valueLevels; }
		}
		public WpfChartPaneModel Pane {
			get { return pane; }
			set {
				if (pane != value) {
					pane = value;
					XYDiagram2D.SetIndicatorPane((SeparatePaneIndicator)Indicator, pane.Pane);
					OnPropertyChanged("Pane");
				}
			}
		}
		public WpfChartAxisModel AxisY {
			get { return axisY; }
			set {
				if (axisY != value) {
					axisY = value;
					SecondaryAxisY2D axis = axisY.Axis as SecondaryAxisY2D;
					XYDiagram2D.SetIndicatorAxisY((SeparatePaneIndicator)Indicator, axis);
					OnPropertyChanged("AxisY");
				}
			}
		}
		public WpfChartIndicatorModel(ChartModelElement parent, Indicator indicator)
		: base(parent, indicator) {
			Series indicatorOwnerSeries = ChartDesignerPropertiesProvider.GetIndicatorOwner(indicator);
			if (indicatorOwnerSeries is FinancialSeries2D) {
				valueLevels.Add(new ValueLevelComboBoxPresentation(ValueLevel.High));
				valueLevels.Add(new ValueLevelComboBoxPresentation(ValueLevel.Low));
				valueLevels.Add(new ValueLevelComboBoxPresentation(ValueLevel.Open));
				valueLevels.Add(new ValueLevelComboBoxPresentation(ValueLevel.Close));
			}
			else if (indicatorOwnerSeries is RangeBarSeries2D || indicatorOwnerSeries is RangeAreaSeries2D) {
				valueLevels.Add(new ValueLevelComboBoxPresentation(ValueLevel.Value));
				valueLevels.Add(new ValueLevelComboBoxPresentation(ValueLevel.Value2));
			}
			else
				valueLevels.Add(new ValueLevelComboBoxPresentation(ValueLevel.Value));
			if (Indicator is SeparatePaneIndicator) {
				UpdatePane();
				UpdateAxisY();
			}
			PropertyGridModel = WpfChartIndicatorPropertyGridModel.CreatePropertyGridModelForIndicator(this);
		}
		Brush GetActualIndicatorBrush() {
			int i = 0;
			foreach (ChartModelElement element in Parent.Children) {
				if (element == this) {
					WpfChartModel chartModel = (WpfChartModel)GetParent<WpfChartModel>();
					return new SolidColorBrush(chartModel.Chart.IndicatorsPalette[i]);
				}
				i++;
			}
			return null;
		}
		void UpdatePane() {
			Pane seriesPane = XYDiagram2D.GetIndicatorPane((SeparatePaneIndicator)Indicator);
			WpfChartDiagramModel diagramModel = Diagram;
			if (seriesPane == null || diagramModel.DefaultPaneModel.Pane == seriesPane)
				Pane = diagramModel.DefaultPaneModel;
			else
				if (diagramModel.PanesCollectionModel.ModelCollection.Count > 0)
				foreach (ChartModelElement paneModel in diagramModel.PanesCollectionModel.ModelCollection)
					if (((WpfChartPaneModel)paneModel).Pane == seriesPane) {
						Pane = (WpfChartPaneModel)paneModel;
						break;
					}
		}
		void UpdateAxisY() {
			WpfChartDiagramModel diagramModel = Diagram;
			SecondaryAxisY2D seriesAxisY = XYDiagram2D.GetIndicatorAxisY((SeparatePaneIndicator)Indicator);
			if (seriesAxisY == null)
				AxisY = diagramModel.PrimaryAxisModelY;
			else
				if (diagramModel.SecondaryAxesCollectionModelY.ModelCollection.Count > 0)
				foreach (ChartModelElement axisModel in diagramModel.SecondaryAxesCollectionModelY.ModelCollection)
					if (((WpfChartAxisModel)axisModel).Axis == seriesAxisY) {
						AxisY = (WpfChartAxisModel)axisModel;
						break;
					}
		}
		protected override void UpdateChildren() {
			if (Indicator is SeparatePaneIndicator) {
				UpdatePane();
				UpdateAxisY();
			}
		}
	}
}
