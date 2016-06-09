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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Mvvm.Native;
namespace DevExpress.Charts.Designer.Native {
	public class IndicatorItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartSimpleMovingAveragePropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewSimpleMovingAverage)),
					new TypeInfo(typeof(WpfChartWeightedMovingAveragePropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewWeightedMovingAverage)),
					new TypeInfo(typeof(WpfChartExponentialMovingAveragePropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewExponentialMovingAverage)),
					new TypeInfo(typeof(WpfChartTriangularMovingAveragePropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewTriangularMovingAverage)),
					new TypeInfo(typeof(WpfChartRegressionLinePropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewRegressionLine)),
					new TypeInfo(typeof(WpfChartTrendLinePropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewTrendLine)),
					new TypeInfo(typeof(WpfChartFibonacciRetracementPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewFibonacciRetracement)),
					new TypeInfo(typeof(WpfChartFibonacciFansPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewFibonacciFans)),
					new TypeInfo(typeof(WpfChartFibonacciArcsPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewFibonacciArcs)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return Activator.CreateInstance(type.Type);
		}
	}
	public class IndicatorPropertyGridModelCollection : ObservableCollection<WpfChartIndicatorPropertyGridModel> {
		WpfChartModel chartModel;
		WpfChartSeriesModel seriesModel;
		public IndicatorPropertyGridModelCollection(WpfChartModel chartModel, WpfChartSeriesModel seriesModel) {
			this.chartModel = chartModel;
			this.seriesModel = seriesModel;
		}
		void ExecuteCommand(ICommand command, WpfChartSeriesModel seriesModel) {
			command.Execute(seriesModel);
		}
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					foreach (WpfChartIndicatorPropertyGridModel newIndicator in e.NewItems)
						if (newIndicator.IndicatorModel == null) {
							if (newIndicator is WpfChartSimpleMovingAveragePropertyGridModel)
								ExecuteCommand(new AddIndicatorCommand<SimpleMovingAverage>(chartModel), seriesModel);
							else if (newIndicator is WpfChartWeightedMovingAveragePropertyGridModel)
								ExecuteCommand(new AddIndicatorCommand<WeightedMovingAverage>(chartModel), seriesModel);
							else if (newIndicator is WpfChartExponentialMovingAveragePropertyGridModel)
								ExecuteCommand(new AddIndicatorCommand<ExponentialMovingAverage>(chartModel), seriesModel);
							else if (newIndicator is WpfChartTriangularMovingAveragePropertyGridModel)
								ExecuteCommand(new AddIndicatorCommand<TriangularMovingAverage>(chartModel), seriesModel);
							else if (newIndicator is WpfChartRegressionLinePropertyGridModel)
								ExecuteCommand(new AddIndicatorCommand<RegressionLine>(chartModel), seriesModel);
							else if (newIndicator is WpfChartTrendLinePropertyGridModel)
								ExecuteCommand(new AddIndicatorCommand<TrendLine>(chartModel), seriesModel);
							else if (newIndicator is WpfChartFibonacciRetracementPropertyGridModel)
								ExecuteCommand(new AddIndicatorCommand<FibonacciRetracement>(chartModel), seriesModel);
							else if (newIndicator is WpfChartFibonacciFansPropertyGridModel)
								ExecuteCommand(new AddIndicatorCommand<FibonacciFans>(chartModel), seriesModel);
							else if (newIndicator is WpfChartFibonacciArcsPropertyGridModel)
								ExecuteCommand(new AddIndicatorCommand<FibonacciArcs>(chartModel), seriesModel);
							break;
						}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (WpfChartIndicatorPropertyGridModel oldIndicator in e.OldItems) {
						if (seriesModel.IndicatorCollectionModel.ModelCollection.Contains(oldIndicator.IndicatorModel)) {
							RemoveIndicatorCommand command = new RemoveIndicatorCommand(chartModel);
							((ICommand)command).Execute(oldIndicator.Indicator);
							break;
						}
					}
					break;
				default:
					break;
			}
		}
	}
	public abstract class WpfChartIndicatorPropertyGridModel : PropertyGridModelBase {
		public static WpfChartIndicatorPropertyGridModel CreatePropertyGridModelForIndicator(WpfChartIndicatorModel indicatorModel) {
			Indicator indicator = indicatorModel.Indicator;
			if (indicator is SimpleMovingAverage)
				return new WpfChartSimpleMovingAveragePropertyGridModel(indicatorModel);
			else if (indicator is WeightedMovingAverage)
				return new WpfChartWeightedMovingAveragePropertyGridModel(indicatorModel);
			else if (indicator is ExponentialMovingAverage)
				return new WpfChartExponentialMovingAveragePropertyGridModel(indicatorModel);
			else if (indicator is TriangularMovingAverage)
				return new WpfChartTriangularMovingAveragePropertyGridModel(indicatorModel);
			else if (indicator is TripleExponentialMovingAverageTema)
				return new WpfChartTripleExponentialMovingAverageTemaPropertyGridModel(indicatorModel);
			else if (indicator is RegressionLine)
				return new WpfChartRegressionLinePropertyGridModel(indicatorModel);
			else if (indicator is TrendLine)
				return new WpfChartTrendLinePropertyGridModel(indicatorModel);
			else if (indicator is FibonacciRetracement)
				return new WpfChartFibonacciRetracementPropertyGridModel(indicatorModel);
			else if (indicator is FibonacciFans)
				return new WpfChartFibonacciFansPropertyGridModel(indicatorModel);
			else if (indicator is FibonacciArcs)
				return new WpfChartFibonacciArcsPropertyGridModel(indicatorModel);
			else if (indicator is AverageTrueRange)
				return new WpfChartAverageTrueRangePropertyGridModel(indicatorModel);
			else if (indicator is ChaikinsVolatility)
				return new WpfChartChaikinsVolatilityPropertyGridModel(indicatorModel);
			else if (indicator is CommodityChannelIndex)
				return new WpfChartCommodityChannelIndexPropertyGridModel(indicatorModel);
			else if (indicator is DetrendedPriceOscillator)
				return new WpfChartDetrendedPriceOscillatorPropertyGridModel(indicatorModel);
			else if (indicator is MassIndex)
				return new WpfChartMassIndexPropertyGridModel(indicatorModel);
			else if (indicator is MovingAverageConvergenceDivergence)
				return new WpfChartMovingAverageConvergenceDivergencePropertyGridModel(indicatorModel);
			else if (indicator is RateOfChange)
				return new WpfChartRateOfChangePropertyGridModel(indicatorModel);
			else if (indicator is RelativeStrengthIndex)
				return new WpfChartRelativeStrengthIndexPropertyGridModel(indicatorModel);
			else if (indicator is StandardDeviation)
				return new WpfChartStandardDeviationPropertyGridModel(indicatorModel);
			else if (indicator is TripleExponentialMovingAverageTrix)
				return new WpfChartTripleExponentialMovingAverageTrixPropertyGridModel(indicatorModel);
			else if (indicator is WilliamsR)
				return new WpfChartWilliamsRPropertyGridModel(indicatorModel);
			else if (indicator is BollingerBands)
				return new WpfChartBollingerBandsPropertyGridModel(indicatorModel);
			else if (indicator is MedianPrice)
				return new WpfChartMedianPricePropertyGridModel(indicatorModel);
			else if (indicator is TypicalPrice)
				return new WpfChartTypicalPricePropertyGridModel(indicatorModel);
			else if (indicator is WeightedClose)
				return new WpfChartWeightedClosePropertyGridModel(indicatorModel);
			else
				throw new ArgumentException("The WpfChartIndicatorPropertyGridModel.CreatePropertyGridModelForIndicator can't give a PropertyGridModel for the indicator of " + indicatorModel.Indicator.GetType().Name + "type.");
		}
		SetIndicatorPropertyCommand setPropertyCommand;
		WpfChartLineStylePropertyGridModel lineStyle;
		protected internal Indicator Indicator { get { return IndicatorModel.Indicator; } }
		protected internal WpfChartIndicatorModel IndicatorModel { get { return ModelElement as WpfChartIndicatorModel; } }
		protected override ICommand SetObjectPropertyCommand { get { return setPropertyCommand; } }
		[Category(Categories.Behavior)]
		public bool Visible {
			get { return Indicator.Visible; }
			set { SetProperty("Visible", value); }
		}
		[Category(Categories.Brushes)]
		public Brush Brush {
			get { return Indicator.Brush; }
			set { SetProperty("Brush", value); }
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Presentation)
		]
		public WpfChartLineStylePropertyGridModel LineStyle {
			get { return lineStyle; }
			set { SetProperty("LineStyle", new LineStyle()); }
		}
		[Category(Categories.Presentation)]
		public string LegendText {
			get { return Indicator.LegendText; }
			set { SetProperty("LegendText", value); }
		}
		[Category(Categories.Behavior)]
		public bool ShowInLegend {
			get { return Indicator.ShowInLegend; }
			set { SetProperty("ShowInLegend", value); }
		}
		public WpfChartIndicatorPropertyGridModel() : this(null) {
		}
		public WpfChartIndicatorPropertyGridModel(WpfChartIndicatorModel indicatorModel) : base(indicatorModel) {
			UpdateInternal();
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (IndicatorModel == null || Indicator == null)
				return;
			if (Indicator.LineStyle != null) {
				if (lineStyle != null && Indicator.LineStyle != lineStyle.LineStyle || lineStyle == null)
					lineStyle = new WpfChartLineStylePropertyGridModel(IndicatorModel, Indicator.LineStyle, SetObjectPropertyCommand, "LineStyle.");
			}
			else
				lineStyle = null;
		}
		protected override void UpdateCommands() {
			base.UpdateCommands();
			setPropertyCommand = new SetIndicatorPropertyCommand(ChartModel);
		}
	}
	public abstract class WpfChartFinancialIndicatorPropertyGridModel : WpfChartIndicatorPropertyGridModel {
		new protected internal FinancialIndicator Indicator { get { return base.Indicator as FinancialIndicator; } }
		[Category(Categories.Behavior)]
		public object Argument1 {
			get { return Indicator.Argument1; }
			set { SetProperty("Argument1", value); }
		}
		[Category(Categories.Behavior)]
		public object Argument2 {
			get { return Indicator.Argument2; }
			set { SetProperty("Argument2", value); }
		}
		[Category(Categories.Behavior)]
		public ValueLevel ValueLevel1 {
			get { return Indicator.ValueLevel1; }
			set { SetProperty("ValueLevel1", value); }
		}
		[Category(Categories.Behavior)]
		public ValueLevel ValueLevel2 {
			get { return Indicator.ValueLevel2; }
			set { SetProperty("ValueLevel2", value); }
		}
		public WpfChartFinancialIndicatorPropertyGridModel() : base() {
		}
		public WpfChartFinancialIndicatorPropertyGridModel(WpfChartIndicatorModel indicatorModel) : base(indicatorModel) {
		}
	}
	public abstract class WpfChartFibonacciIndicatorPropertyGridModel : WpfChartFinancialIndicatorPropertyGridModel {
		new protected internal FibonacciIndicator Indicator { get { return base.Indicator as FibonacciIndicator; } }
		[Category(Categories.Behavior)]
		public bool ShowLevel23_6 {
			get { return Indicator.ShowLevel23_6; }
			set { SetProperty("ShowLevel23_6", value); }
		}
		[Category(Categories.Behavior)]
		public bool ShowLevel76_4 {
			get { return Indicator.ShowLevel76_4; }
			set { SetProperty("ShowLevel76_4", value); }
		}
		[Category(Categories.Elements)]
		public IndicatorLabel Label {
			get { return Indicator.Label; }
			set { SetProperty("Label", value); }
		}
		public WpfChartFibonacciIndicatorPropertyGridModel() : base() {
		}
		public WpfChartFibonacciIndicatorPropertyGridModel(WpfChartIndicatorModel indicatorModel) : base(indicatorModel) {
		}
	}
	public abstract class WpfChartMovingAveragePropertyGridModel : WpfChartIndicatorPropertyGridModel {
		new protected internal MovingAverage Indicator { get { return base.Indicator as MovingAverage; } }
		[Category(Categories.Behavior)]
		public ValueLevel ValueLevel {
			get { return Indicator.ValueLevel; }
			set { SetProperty("ValueLevel", value); }
		}
		[Category(Categories.Behavior)]
		public MovingAverageKind MovingAverageKind {
			get { return Indicator.MovingAverageKind; }
			set { SetProperty("MovingAverageKind", value); }
		}
		[Category(Categories.Behavior)]
		public double EnvelopePercent {
			get { return Indicator.EnvelopePercent; }
			set { SetProperty("EnvelopePercent", value); }
		}
		[Category(Categories.Behavior)]
		public int PointsCount {
			get { return Indicator.PointsCount; }
			set { SetProperty("PointsCount", value); }
		}
		public WpfChartMovingAveragePropertyGridModel() : base() {
		}
		public WpfChartMovingAveragePropertyGridModel(WpfChartIndicatorModel indicatorModel) : base(indicatorModel) {
		}
	}
	public class WpfChartSimpleMovingAveragePropertyGridModel : WpfChartMovingAveragePropertyGridModel {
		new protected internal SimpleMovingAverage Indicator { get { return base.Indicator as SimpleMovingAverage; } }
		public WpfChartSimpleMovingAveragePropertyGridModel() : base() {
		}
		public WpfChartSimpleMovingAveragePropertyGridModel(WpfChartIndicatorModel indicatorModel) : base(indicatorModel) {
		}
	}
	public class WpfChartWeightedMovingAveragePropertyGridModel : WpfChartMovingAveragePropertyGridModel {
		new protected internal WeightedMovingAverage Indicator { get { return base.Indicator as WeightedMovingAverage; } }
		public WpfChartWeightedMovingAveragePropertyGridModel()
			: base() {
		}
		public WpfChartWeightedMovingAveragePropertyGridModel(WpfChartIndicatorModel indicatorModel) : base(indicatorModel) {
		}
	}
	public class WpfChartExponentialMovingAveragePropertyGridModel : WpfChartMovingAveragePropertyGridModel {
		new protected internal ExponentialMovingAverage Indicator { get { return base.Indicator as ExponentialMovingAverage; } }
		public WpfChartExponentialMovingAveragePropertyGridModel()
			: base() {
		}
		public WpfChartExponentialMovingAveragePropertyGridModel(WpfChartIndicatorModel indicatorModel) : base(indicatorModel) {
		}
	}
	public class WpfChartTriangularMovingAveragePropertyGridModel : WpfChartMovingAveragePropertyGridModel {
		new protected internal TriangularMovingAverage Indicator { get { return base.Indicator as TriangularMovingAverage; } }
		public WpfChartTriangularMovingAveragePropertyGridModel()
			: base() {
		}
		public WpfChartTriangularMovingAveragePropertyGridModel(WpfChartIndicatorModel indicatorModel) : base(indicatorModel) {
		}
	}
	public class WpfChartRegressionLinePropertyGridModel : WpfChartIndicatorPropertyGridModel {
		new protected internal RegressionLine Indicator { get { return base.Indicator as RegressionLine; } }
		[Category(Categories.Behavior)]
		public ValueLevel ValueLevel {
			get { return Indicator.ValueLevel; }
			set { SetProperty("ValueLevel", value); }
		}
		public WpfChartRegressionLinePropertyGridModel() : base() {
		}
		public WpfChartRegressionLinePropertyGridModel(WpfChartIndicatorModel indicatorModel) : base(indicatorModel) {
		}
	}
	public class WpfChartTrendLinePropertyGridModel : WpfChartFinancialIndicatorPropertyGridModel {
		new protected internal TrendLine Indicator { get { return base.Indicator as TrendLine; } }
		[Category(Categories.Behavior)]
		public bool ExtrapolateToInfinity {
			get { return Indicator.ExtrapolateToInfinity; }
			set { SetProperty("ExtrapolateToInfinity", value); }
		}
		public WpfChartTrendLinePropertyGridModel() : base() {
		}
		public WpfChartTrendLinePropertyGridModel(WpfChartIndicatorModel indicatorModel) : base(indicatorModel) {
		}
	}
	public class WpfChartFibonacciRetracementPropertyGridModel : WpfChartFibonacciIndicatorPropertyGridModel {
		new protected internal FibonacciRetracement Indicator { get { return base.Indicator as FibonacciRetracement; } }
		[Category(Categories.Behavior)]
		public bool ShowAdditionalLevels {
			get { return Indicator.ShowAdditionalLevels; }
			set { SetProperty("ShowAdditionalLevels", value); }
		}
		public WpfChartFibonacciRetracementPropertyGridModel()
			: base() {
		}
		public WpfChartFibonacciRetracementPropertyGridModel(WpfChartIndicatorModel indicatorModel) : base(indicatorModel) {
		}
	}
	public class WpfChartFibonacciFansPropertyGridModel : WpfChartFibonacciIndicatorPropertyGridModel {
		new protected internal FibonacciFans Indicator { get { return base.Indicator as FibonacciFans; } }
		[Category(Categories.Behavior)]
		public bool ShowLevel0 {
			get { return Indicator.ShowLevel0; }
			set { SetProperty("ShowLevel0", value); }
		}
		public WpfChartFibonacciFansPropertyGridModel()
			: base() {
		}
		public WpfChartFibonacciFansPropertyGridModel(WpfChartIndicatorModel indicatorModel) : base(indicatorModel) {
		}
	}
	public class WpfChartFibonacciArcsPropertyGridModel : WpfChartFibonacciIndicatorPropertyGridModel {
		new protected internal FibonacciArcs Indicator { get { return base.Indicator as FibonacciArcs; } }
		[Category(Categories.Behavior)]
		public bool ShowLevel100 {
			get { return Indicator.ShowLevel100; }
			set { SetProperty("ShowLevel100", value); }
		}
		public WpfChartFibonacciArcsPropertyGridModel()
			: base() {
		}
		public WpfChartFibonacciArcsPropertyGridModel(WpfChartIndicatorModel indicatorModel) : base(indicatorModel) {
		}
	}
	public abstract class WpfChartSeparatePaneIndicatorPropertyGridModel : WpfChartIndicatorPropertyGridModel {
		public WpfChartSeparatePaneIndicatorPropertyGridModel(WpfChartIndicatorModel indicatorModel)
			: base(indicatorModel) { }
		public WpfChartSeparatePaneIndicatorPropertyGridModel() { }
	}
	public class WpfChartAverageTrueRangePropertyGridModel : WpfChartSeparatePaneIndicatorPropertyGridModel {
		public WpfChartAverageTrueRangePropertyGridModel(WpfChartIndicatorModel indicatorModel)
			: base(indicatorModel) { }
		public WpfChartAverageTrueRangePropertyGridModel() { }
		AverageTrueRange AverageTrueRange { get { return (AverageTrueRange)Indicator; } }
		[Category(Categories.Behavior)]
		public int PointsCount {
			get { return AverageTrueRange.PointsCount; }
			set { SetProperty("PointsCount", value); }
		}
	}
	public class WpfChartChaikinsVolatilityPropertyGridModel : WpfChartSeparatePaneIndicatorPropertyGridModel {
		public WpfChartChaikinsVolatilityPropertyGridModel(WpfChartIndicatorModel indicatorModel)
			: base(indicatorModel) { }
		public WpfChartChaikinsVolatilityPropertyGridModel() { }
		ChaikinsVolatility ChaikinsVolatility { get { return (ChaikinsVolatility)Indicator; } }
		[Category(Categories.Behavior)]
		public int PointsCount {
			get { return ChaikinsVolatility.PointsCount; }
			set { SetProperty("PointsCount", value); }
		}
	}
	public class WpfChartCommodityChannelIndexPropertyGridModel : WpfChartSeparatePaneIndicatorPropertyGridModel {
		public WpfChartCommodityChannelIndexPropertyGridModel(WpfChartIndicatorModel indicatorModel)
			: base(indicatorModel) { }
		public WpfChartCommodityChannelIndexPropertyGridModel() { }
		CommodityChannelIndex CommodityChannelIndex { get { return (CommodityChannelIndex)Indicator; } }
		[Category(Categories.Behavior)]
		public int PointsCount {
			get { return CommodityChannelIndex.PointsCount; }
			set { SetProperty("PointsCount", value); }
		}
	}
	public class WpfChartDetrendedPriceOscillatorPropertyGridModel : WpfChartSeparatePaneIndicatorPropertyGridModel {
		public WpfChartDetrendedPriceOscillatorPropertyGridModel(WpfChartIndicatorModel indicatorModel)
			: base(indicatorModel) { }
		public WpfChartDetrendedPriceOscillatorPropertyGridModel() { }
		DetrendedPriceOscillator DetrendedPriceOscillator { get { return (DetrendedPriceOscillator)Indicator; } }
		[Category(Categories.Behavior)]
		public int PointsCount {
			get { return DetrendedPriceOscillator.PointsCount; }
			set { SetProperty("PointsCount", value); }
		}
		[Category(Categories.Behavior)]
		public ValueLevel ValueLevel {
			get { return DetrendedPriceOscillator.ValueLevel; }
			set { SetProperty("ValueLevel", value); }
		}
	}
	public class WpfChartMassIndexPropertyGridModel : WpfChartSeparatePaneIndicatorPropertyGridModel {
		public WpfChartMassIndexPropertyGridModel(WpfChartIndicatorModel indicatorModel)
			: base(indicatorModel) { }
		public WpfChartMassIndexPropertyGridModel() { }
		MassIndex MassIndex { get { return (MassIndex)Indicator; } }
		[Category(Categories.Behavior)]
		public int MovingAveragePointsCount {
			get { return MassIndex.MovingAveragePointsCount; }
			set { SetProperty("MovingAveragePointsCount", value); }
		}
		[Category(Categories.Behavior)]
		public int SumPointsCount {
			get { return MassIndex.SumPointsCount; }
			set { SetProperty("SumPointsCount", value); }
		}
	}
	public class WpfChartMovingAverageConvergenceDivergencePropertyGridModel : WpfChartSeparatePaneIndicatorPropertyGridModel {
		public WpfChartMovingAverageConvergenceDivergencePropertyGridModel(WpfChartIndicatorModel indicatorModel)
			: base(indicatorModel) { }
		public WpfChartMovingAverageConvergenceDivergencePropertyGridModel() { }
		MovingAverageConvergenceDivergence MovingAverageConvergenceDivergence { get { return (MovingAverageConvergenceDivergence)Indicator; } }
		[Category(Categories.Behavior)]
		public int LongPeriod {
			get { return MovingAverageConvergenceDivergence.LongPeriod; }
			set { SetProperty("LongPeriod", value); }
		}
		[Category(Categories.Behavior)]
		public int ShortPeriod {
			get { return MovingAverageConvergenceDivergence.ShortPeriod; }
			set { SetProperty("ShortPeriod", value); }
		}
		[Category(Categories.Behavior)]
		public int SignalSmoothingPeriod {
			get { return MovingAverageConvergenceDivergence.SignalSmoothingPeriod; }
			set { SetProperty("SignalSmoothingPeriod", value); }
		}
		[Category(Categories.Behavior)]
		public ValueLevel ValueLevel {
			get { return MovingAverageConvergenceDivergence.ValueLevel; }
			set { SetProperty("ValueLevel", value); }
		}
	}
	public class WpfChartRateOfChangePropertyGridModel : WpfChartSeparatePaneIndicatorPropertyGridModel {
		public WpfChartRateOfChangePropertyGridModel(WpfChartIndicatorModel indicatorModel)
			: base(indicatorModel) { }
		public WpfChartRateOfChangePropertyGridModel() { }
		RateOfChange RateOfChange { get { return (RateOfChange)Indicator; } }
		[Category(Categories.Behavior)]
		public int PointsCount {
			get { return RateOfChange.PointsCount; }
			set { SetProperty("PointsCount", value); }
		}
		[Category(Categories.Behavior)]
		public ValueLevel ValueLevel {
			get { return RateOfChange.ValueLevel; }
			set { SetProperty("ValueLevel", value); }
		}
	}
	public class WpfChartRelativeStrengthIndexPropertyGridModel : WpfChartSeparatePaneIndicatorPropertyGridModel {
		public WpfChartRelativeStrengthIndexPropertyGridModel(WpfChartIndicatorModel indicatorModel)
			: base(indicatorModel) { }
		public WpfChartRelativeStrengthIndexPropertyGridModel() { }
		RelativeStrengthIndex RelativeStrengthIndex { get { return (RelativeStrengthIndex)Indicator; } }
		[Category(Categories.Behavior)]
		public int PointsCount {
			get { return RelativeStrengthIndex.PointsCount; }
			set { SetProperty("PointsCount", value); }
		}
		[Category(Categories.Behavior)]
		public ValueLevel ValueLevel {
			get { return RelativeStrengthIndex.ValueLevel; }
			set { SetProperty("ValueLevel", value); }
		}
	}
	public class WpfChartTripleExponentialMovingAverageTrixPropertyGridModel : WpfChartSeparatePaneIndicatorPropertyGridModel {
		public WpfChartTripleExponentialMovingAverageTrixPropertyGridModel(WpfChartIndicatorModel indicatorModel)
			: base(indicatorModel) { }
		public WpfChartTripleExponentialMovingAverageTrixPropertyGridModel() { }
		TripleExponentialMovingAverageTrix TripleExponentialMovingAverageTrix { get { return (TripleExponentialMovingAverageTrix)Indicator; } }
		[Category(Categories.Behavior)]
		public int PointsCount {
			get { return TripleExponentialMovingAverageTrix.PointsCount; }
			set { SetProperty("PointsCount", value); }
		}
		[Category(Categories.Behavior)]
		public ValueLevel ValueLevel {
			get { return TripleExponentialMovingAverageTrix.ValueLevel; }
			set { SetProperty("ValueLevel", value); }
		}
	}
	public class WpfChartStandardDeviationPropertyGridModel : WpfChartSeparatePaneIndicatorPropertyGridModel {
		public WpfChartStandardDeviationPropertyGridModel(WpfChartIndicatorModel indicatorModel)
			: base(indicatorModel) { }
		public WpfChartStandardDeviationPropertyGridModel() { }
		StandardDeviation StandardDeviation { get { return (StandardDeviation)Indicator; } }
		[Category(Categories.Behavior)]
		public int PointsCount {
			get { return StandardDeviation.PointsCount; }
			set { SetProperty("PointsCount", value); }
		}
		[Category(Categories.Behavior)]
		public ValueLevel ValueLevel {
			get { return StandardDeviation.ValueLevel; }
			set { SetProperty("ValueLevel", value); }
		}
	}
	public class WpfChartWilliamsRPropertyGridModel : WpfChartSeparatePaneIndicatorPropertyGridModel {
		public WpfChartWilliamsRPropertyGridModel(WpfChartIndicatorModel indicatorModel)
			: base(indicatorModel) { }
		public WpfChartWilliamsRPropertyGridModel() { }
		WilliamsR WilliamsR { get { return (WilliamsR)Indicator; } }
		[Category(Categories.Behavior)]
		public int PointsCount {
			get { return WilliamsR.PointsCount; }
			set { SetProperty("PointsCount", value); }
		}
	}
	public class WpfChartBollingerBandsPropertyGridModel : WpfChartIndicatorPropertyGridModel {
		public WpfChartBollingerBandsPropertyGridModel(WpfChartIndicatorModel indicatorModel)
			: base(indicatorModel) { }
		public WpfChartBollingerBandsPropertyGridModel() { }
		BollingerBands BollingerBands { get { return (BollingerBands)Indicator; } }
		[Category(Categories.Behavior)]
		public int PointsCount {
			get { return BollingerBands.PointsCount; }
			set { SetProperty("PointsCount", value); }
		}
		[Category(Categories.Behavior)]
		public double StandardDeviationMultiplier {
			get { return BollingerBands.StandardDeviationMultiplier; }
			set { SetProperty("StandardDeviationMultiplier", value); }
		}
		[Category(Categories.Behavior)]
		public ValueLevel ValueLevel {
			get { return BollingerBands.ValueLevel; }
			set { SetProperty("ValueLevel", value); }
		}
	}
	public class WpfChartTripleExponentialMovingAverageTemaPropertyGridModel : WpfChartMovingAveragePropertyGridModel {
		public WpfChartTripleExponentialMovingAverageTemaPropertyGridModel(WpfChartIndicatorModel indicatorModel)
			: base(indicatorModel) { }
		public WpfChartTripleExponentialMovingAverageTemaPropertyGridModel() { }
	}
	public class WpfChartTypicalPricePropertyGridModel : WpfChartIndicatorPropertyGridModel {
		public WpfChartTypicalPricePropertyGridModel(WpfChartIndicatorModel indicatorModel)
			: base(indicatorModel) { }
		public WpfChartTypicalPricePropertyGridModel() { }
	}
	public class WpfChartMedianPricePropertyGridModel : WpfChartIndicatorPropertyGridModel {
		public WpfChartMedianPricePropertyGridModel(WpfChartIndicatorModel indicatorModel)
			: base(indicatorModel) { }
		public WpfChartMedianPricePropertyGridModel() { }
	}
	public class WpfChartWeightedClosePropertyGridModel : WpfChartIndicatorPropertyGridModel {
		public WpfChartWeightedClosePropertyGridModel(WpfChartIndicatorModel indicatorModel)
			: base(indicatorModel) { }
		public WpfChartWeightedClosePropertyGridModel() { }
	}
}
