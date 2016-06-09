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
using System.Drawing;
using System.Drawing.Design;
using DevExpress.Utils.Design;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Designer.Native {
	public abstract class ChartElementNamedModel : DesignerChartElementModelBase {
		readonly ChartElementNamed chartElementNamed;
		protected ChartElementNamed ChartElementNamed { get { return chartElementNamed; } }
		protected internal override ChartElement ChartElement { get { return chartElementNamed; } }
		[Category("Misc")]
		public string Name {
			get { return ChartElementNamed.Name; }
			set { SetProperty("Name", value); }
		}
		public ChartElementNamedModel(ChartElementNamed chartElementNamed, CommandManager commandManager) : base(commandManager) {
			this.chartElementNamed = chartElementNamed;
		}
	}
	[HasOptionsControl]
	public abstract class IndicatorModel : ChartElementNamedModel, ISupportModelVisibility {
		LineStyleModel lineStyleModel;
		protected Indicator Indicator { get { return (Indicator)base.ChartElementNamed; } }
		protected internal override bool HasOptionsControl { get { return true; } }
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.IndicatorKey; } }
		[PropertyForOptions,
		Category(Categories.Behavior),
		TypeConverter(typeof(BooleanTypeConverter))]
		public Boolean Visible {
			get { return Indicator.Visible; }
			set { SetProperty("Visible", value); }
		}
		[
		PropertyForOptions,
		Category("Misc")]
		public new string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		[Category(Categories.Appearance)]
		public Color Color {
			get { return Indicator.Color; }
			set { SetProperty("Color", value); }
		}
		[Category(Categories.Behavior), TypeConverter(typeof(BooleanTypeConverter))]
		public Boolean CheckedInLegend {
			get { return Indicator.CheckedInLegend; }
			set { SetProperty("CheckedInLegend", value); }
		}
		[Category(Categories.Behavior), TypeConverter(typeof(BooleanTypeConverter))]
		public Boolean CheckableInLegend {
			get { return Indicator.CheckableInLegend; }
			set { SetProperty("CheckableInLegend", value); }
		}
		[PropertyForOptions,
		Category(Categories.Behavior),
		TypeConverter(typeof(BooleanTypeConverter))]
		public Boolean ShowInLegend {
			get { return Indicator.ShowInLegend; }
			set { SetProperty("ShowInLegend", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Appearance)]
		public LineStyleModel LineStyle { get { return lineStyleModel; } }
		public IndicatorModel(Indicator indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
			Update();
		}
		protected override void AddChildren() {
			if(lineStyleModel != null)
				Children.Add(lineStyleModel);
			base.AddChildren();
		}
		public override void Update() {
			this.lineStyleModel = new LineStyleModel(Indicator.LineStyle, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeElement(this, parentCollection);
		}
	}
	public abstract class SingleLevelIndicatorModel : IndicatorModel {
		protected new SingleLevelIndicator Indicator { get { return (SingleLevelIndicator)base.Indicator; } }
		[PropertyForOptions(Categories.Behavior),
		Category(Categories.Behavior)]
		public ValueLevel ValueLevel {
			get { return Indicator.ValueLevel; }
			set { SetProperty("ValueLevel", value); }
		}
		public SingleLevelIndicatorModel(SingleLevelIndicator indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
		}
	}
	public abstract class FinancialIndicatorModel : IndicatorModel {
		FinancialIndicatorPointModel point1Model;
		FinancialIndicatorPointModel point2Model;
		protected new FinancialIndicator Indicator { get { return (FinancialIndicator)base.Indicator; } }
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		AllocateToGroup("Point 1"),
		Category(Categories.Behavior)]
		public FinancialIndicatorPointModel Point1 { get { return point1Model; } }
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		AllocateToGroup("Point 2"),
		Category(Categories.Behavior)]
		public FinancialIndicatorPointModel Point2 { get { return point2Model; } }
		public FinancialIndicatorModel(FinancialIndicator indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
			Update();
		}
		protected override void AddChildren() {
			if(point1Model != null)
				Children.Add(point1Model);
			if(point2Model != null)
				Children.Add(point2Model);
			base.AddChildren();
		}
		public override void Update() {
			this.point1Model = new FinancialIndicatorPointModel(Indicator.Point1, CommandManager);
			this.point2Model = new FinancialIndicatorPointModel(Indicator.Point2, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	public abstract class SubsetBasedIndicatorModel : SingleLevelIndicatorModel {
		protected new SubsetBasedIndicator Indicator { get { return (SubsetBasedIndicator)base.Indicator; } }
		[PropertyForOptions(Categories.Behavior),
		Category(Categories.Behavior)]
		public int PointsCount {
			get { return Indicator.PointsCount; }
			set { SetProperty("PointsCount", value); }
		}
		public SubsetBasedIndicatorModel(SubsetBasedIndicator indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
		}
	}
	public abstract class MovingAverageModel : SubsetBasedIndicatorModel {
		LineStyleModel envelopeLineStyleModel;
		protected new MovingAverage Indicator { get { return (MovingAverage)base.Indicator; } }
		[PropertyForOptions(Categories.Behavior),
		Category(Categories.Behavior)]
		public MovingAverageKind Kind {
			get { return Indicator.Kind; }
			set { SetProperty("Kind", value); }
		}
		[
		PropertyForOptions(Categories.Behavior),
		DependentUpon("Kind"),
		Category(Categories.Behavior)]
		public double EnvelopePercent {
			get { return Indicator.EnvelopePercent; }
			set { SetProperty("EnvelopePercent", value); }
		}
		[Category(Categories.Appearance)]
		public Color EnvelopeColor {
			get { return Indicator.EnvelopeColor; }
			set { SetProperty("EnvelopeColor", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Appearance)]
		public LineStyleModel EnvelopeLineStyle { get { return envelopeLineStyleModel; } }
		[Category(Categories.Appearance)]
		public new Color Color {
			get { return Indicator.Color; }
			set { SetProperty("Color", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Appearance)]
		public new LineStyleModel LineStyle { get { return base.LineStyle; } }
		public MovingAverageModel(MovingAverage indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
			Update();
		}
		protected override void AddChildren() {
			if(envelopeLineStyleModel != null)
				Children.Add(envelopeLineStyleModel);
			base.AddChildren();
		}
		public override void Update() {
			this.envelopeLineStyleModel = new LineStyleModel(Indicator.EnvelopeLineStyle, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(TriangularMovingAverage)), TypeConverter(typeof(MovingAverageTypeConverter))]
	public class TriangularMovingAverageModel : MovingAverageModel {
		protected new TriangularMovingAverage Indicator { get { return (TriangularMovingAverage)base.Indicator; } }
		public TriangularMovingAverageModel(TriangularMovingAverage indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
		}
	}
	[ModelOf(typeof(WeightedMovingAverage)), TypeConverter(typeof(MovingAverageTypeConverter))]
	public class WeightedMovingAverageModel : MovingAverageModel {
		protected new WeightedMovingAverage Indicator { get { return (WeightedMovingAverage)base.Indicator; } }
		public WeightedMovingAverageModel(WeightedMovingAverage indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
		}
	}
	[ModelOf(typeof(ExponentialMovingAverage)), TypeConverter(typeof(MovingAverageTypeConverter))]
	public class ExponentialMovingAverageModel : MovingAverageModel {
		protected new ExponentialMovingAverage Indicator { get { return (ExponentialMovingAverage)base.Indicator; } }
		public ExponentialMovingAverageModel(ExponentialMovingAverage indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
		}
	}
	[ModelOf(typeof(SimpleMovingAverage)), TypeConverter(typeof(MovingAverageTypeConverter))]
	public class SimpleMovingAverageModel : MovingAverageModel {
		protected new SimpleMovingAverage Indicator { get { return (SimpleMovingAverage)base.Indicator; } }
		public SimpleMovingAverageModel(SimpleMovingAverage indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
		}
	}
	[ModelOf(typeof(RegressionLine))]
	public class RegressionLineModel : SingleLevelIndicatorModel {
		protected new RegressionLine Indicator { get { return (RegressionLine)base.Indicator; } }
		public RegressionLineModel(RegressionLine indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
		}
	}
	[ModelOf(typeof(FibonacciIndicator)), TypeConverter(typeof(FibonacciIndicator.TypeConverter))]
	public class FibonacciIndicatorModel : FinancialIndicatorModel {
		FibonacciIndicatorLabelModel labelModel;
		LineStyleModel baseLevelLineStyleModel;
		protected new FibonacciIndicator Indicator { get { return (FibonacciIndicator)base.Indicator; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		AllocateToGroup("Label", 12),
		Category("Elements")]
		public FibonacciIndicatorLabelModel Label { get { return labelModel; } }
		[Category(Categories.Appearance)]
		public Color BaseLevelColor {
			get { return Indicator.BaseLevelColor; }
			set { SetProperty("BaseLevelColor", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Appearance)]
		public LineStyleModel BaseLevelLineStyle { get { return baseLevelLineStyleModel; } }
		[PropertyForOptions(Categories.Behavior),
		DependentUpon("Kind"),
		Category(Categories.Behavior),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowLevel0 {
			get { return Indicator.ShowLevel0; }
			set { SetProperty("ShowLevel0", value); }
		}
		[PropertyForOptions(Categories.Behavior),
		DependentUpon("Kind"),
		Category(Categories.Behavior),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowLevel100 {
			get { return Indicator.ShowLevel100; }
			set { SetProperty("ShowLevel100", value); }
		}
		[PropertyForOptions(Categories.Behavior),
		Category(Categories.Behavior),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowLevel23_6 {
			get { return Indicator.ShowLevel23_6; }
			set { SetProperty("ShowLevel23_6", value); }
		}
		[PropertyForOptions(Categories.Behavior),
		Category(Categories.Behavior),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowLevel76_4 {
			get { return Indicator.ShowLevel76_4; }
			set { SetProperty("ShowLevel76_4", value); }
		}
		[
		PropertyForOptions(Categories.Behavior),
		DependentUpon("Kind"),
		Category(Categories.Behavior),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowAdditionalLevels {
			get { return Indicator.ShowAdditionalLevels; }
			set { SetProperty("ShowAdditionalLevels", value); }
		}
		[Category(Categories.Behavior)]
		public FibonacciIndicatorKind Kind {
			get { return Indicator.Kind; }
			set { SetProperty("Kind", value); }
		}
		public FibonacciIndicatorModel(FibonacciIndicator indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
			Update();
		}
		protected override void AddChildren() {
			if(labelModel != null)
				Children.Add(labelModel);
			if(baseLevelLineStyleModel != null)
				Children.Add(baseLevelLineStyleModel);
			base.AddChildren();
		}
		public override void Update() {
			this.labelModel = new FibonacciIndicatorLabelModel(Indicator.Label, CommandManager);
			this.baseLevelLineStyleModel = new LineStyleModel(Indicator.BaseLevelLineStyle, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(TrendLine))]
	public class TrendLineModel : FinancialIndicatorModel {
		protected new TrendLine Indicator { get { return (TrendLine)base.Indicator; } }
		[PropertyForOptions(Categories.Behavior),
		Category(Categories.Behavior),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool ExtrapolateToInfinity {
			get { return Indicator.ExtrapolateToInfinity; }
			set { SetProperty("ExtrapolateToInfinity", value); }
		}
		public TrendLineModel(TrendLine indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
		}
	}
	[ModelOf(typeof(FinancialIndicatorPoint))]
	public class FinancialIndicatorPointModel : DesignerChartElementModelBase {
		readonly FinancialIndicatorPoint point;
		protected FinancialIndicatorPoint Point { get { return point; } }
		protected internal override ChartElement ChartElement { get { return point; } }
		[PropertyForOptions, TypeConverter(typeof(ValueLevelTypeConterter))]
		public ValueLevel ValueLevel {
			get { return Point.ValueLevel; }
			set { SetProperty("ValueLevel", value); }
		}
		[PropertyForOptions, TypeConverter(typeof(FinancialIndicatorPointArgumentTypeConverter))]
		public object Argument {
			get { return Point.Argument; }
			set { SetProperty("Argument", value); }
		}
		public FinancialIndicatorPointModel(FinancialIndicatorPoint point, CommandManager commandManager)
			: base(commandManager) {
			this.point = point;
		}
	}
	[ModelOf(typeof(FibonacciIndicatorLabel))]
	public class FibonacciIndicatorLabelModel : TitleBaseModel {
		protected FibonacciIndicatorLabel Label { get { return (FibonacciIndicatorLabel)Title; } }
		[PropertyForOptions(Categories.Appearance),
		DependentUpon("Visible")]
		public Color BaseLevelTextColor {
			get { return Label.BaseLevelTextColor; }
			set { SetProperty("BaseLevelTextColor", value); }
		}
		public FibonacciIndicatorLabelModel(FibonacciIndicatorLabel label, CommandManager commandManager)
			: base(label, commandManager) {
		}
	}
	public abstract class SeparatePaneIndicatorModel : IndicatorModel {
		XYDiagramPaneBaseModel paneModel;
		AxisYBaseModel axisYModel;
		protected SeparatePaneIndicator SeparatePaneIndicator { get { return (SeparatePaneIndicator)base.Indicator; } }
		[Editor("DevExpress.XtraCharts.Designer.Native.PaneModelUITypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		PropertyForOptions(Categories.Layout),
		Category(Categories.Layout),
		UseAsSimpleProperty]
		public XYDiagramPaneBaseModel Pane {
			get { return paneModel; }
			set { SetProperty("Pane", value); }
		}
		[Editor("DevExpress.XtraCharts.Designer.Native.AxisYModelUITypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		PropertyForOptions(Categories.Layout),
		Category(Categories.Layout),
		UseAsSimpleProperty]
		public AxisYBaseModel AxisY {
			get { return axisYModel; }
			set { SetProperty("AxisY", value); }
		}
		public SeparatePaneIndicatorModel(SeparatePaneIndicator indicator, CommandManager commandManager) 
			: base(indicator, commandManager) { }
		public override void Update() {
			DesignerChartModel chartModel = FindParent<DesignerChartModel>();
			if (chartModel == null)
				return;
			if (SeparatePaneIndicator.Pane != null)
				this.paneModel = (XYDiagramPaneBaseModel)chartModel.FindElementModel(SeparatePaneIndicator.Pane);
			else
				this.paneModel = null;
			if (SeparatePaneIndicator.AxisY != null)
				this.axisYModel = (AxisYBaseModel)chartModel.FindElementModel(SeparatePaneIndicator.AxisY);
			else
				this.axisYModel = null;
			ClearChildren();
			AddChildren();
			base.Update();
		}
		protected override void ProcessMessage(ViewMessage message) {
			if (message.Name == "Pane")
				SetProperty(message.Name, ((XYDiagramPaneBaseModel)message.Value).Pane);
			else if (message.Name == "AxisY")
				SetProperty(message.Name, ((AxisYBaseModel)message.Value).Axis);
			else
				base.ProcessMessage(message);
		}
	}
	[ModelOf(typeof(StandardDeviation))]
	public class StandardDeviationModel : SeparatePaneIndicatorModel {
		[PropertyForOptions(Categories.Behavior),
		 Category(Categories.Behavior)]
		public ValueLevel ValueLevel {
			get { return ((StandardDeviation)Indicator).ValueLevel; }
			set { SetProperty("ValueLevel", value); }
		}
		[PropertyForOptions(Categories.Behavior),
		 Category(Categories.Behavior)]
		public int PointsCount {
			get { return ((StandardDeviation)Indicator).PointsCount; }
			set { SetProperty("PointsCount", value); }
		}
		public StandardDeviationModel(StandardDeviation indicator, CommandManager commandManager) 
			: base(indicator, commandManager) {
		}
	}
	[ModelOf(typeof(AverageTrueRange))]
	public class AverageTrueRangeModel : SeparatePaneIndicatorModel {
		[PropertyForOptions(Categories.Behavior),
		Category(Categories.Behavior)]
		public int PointsCount {
			get { return ((AverageTrueRange)Indicator).PointsCount; }
			set { SetProperty("PointsCount", value); }
		}
		public AverageTrueRangeModel(AverageTrueRange indicator, CommandManager commandManager) 
			: base(indicator, commandManager) {
		}
	}
	[ModelOf(typeof(CommodityChannelIndex))]
	public class CommodityChannelIndexModel : SeparatePaneIndicatorModel {
		[PropertyForOptions(Categories.Behavior),
		Category(Categories.Behavior)]
		public int PointsCount {
			get { return ((CommodityChannelIndex)Indicator).PointsCount; }
			set { SetProperty("PointsCount", value); }
		}
		public CommodityChannelIndexModel(CommodityChannelIndex indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
		}
	}
	[ModelOf(typeof(DetrendedPriceOscillator))]
	public class DetrendedPriceOscillatorModel : SeparatePaneIndicatorModel {
		[PropertyForOptions(Categories.Behavior),
		Category(Categories.Behavior)]
		public ValueLevel ValueLevel {
			get { return ((DetrendedPriceOscillator)Indicator).ValueLevel; }
			set { SetProperty("ValueLevel", value); }
		}
		[PropertyForOptions(Categories.Behavior),
		Category(Categories.Behavior)]
		public int PointsCount {
			get { return ((DetrendedPriceOscillator)Indicator).PointsCount; }
			set { SetProperty("PointsCount", value); }
		}
		public DetrendedPriceOscillatorModel(DetrendedPriceOscillator indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
		}
	}
	[ModelOf(typeof(MassIndex))]
	public class MassIndexModel : SeparatePaneIndicatorModel {
		public MassIndexModel(MassIndex indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
		}
		[PropertyForOptions(Categories.Behavior),
		Category(Categories.Behavior)]
		public int MovingAveragePointsCount {
			get { return ((MassIndex)Indicator).MovingAveragePointsCount; }
			set { SetProperty("MovingAveragePointsCount", value); }
		}
		[PropertyForOptions(Categories.Behavior),
		 Category(Categories.Behavior)]
		public int SumPointsCount {
			get { return ((MassIndex)Indicator).SumPointsCount; }
			set { SetProperty("SumPointsCount", value); }
		}
	}
	[ModelOf(typeof(MedianPrice))]
	public class MedianPriceModel : IndicatorModel {
		public MedianPriceModel(MedianPrice indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
		}
	}
	[ModelOf(typeof(MovingAverageConvergenceDivergence))]
	public class MovingAverageConvergenceDivergenceModel : SeparatePaneIndicatorModel {
		MovingAverageConvergenceDivergence Macd { get { return (MovingAverageConvergenceDivergence)base.Indicator; }  }
		[PropertyForOptions,
		Category(Categories.Behavior)]
		public int LongPeriod {
			get { return Macd.LongPeriod; }
			set { SetProperty("LongPeriod", value); }
		}
		[PropertyForOptions,
		 Category(Categories.Behavior)]
		public int ShortPeriod {
			get { return Macd.ShortPeriod; }
			set { SetProperty("ShortPeriod", value); }
		}
		[PropertyForOptions,
		 Category(Categories.Behavior)]
		public int SignalSmoothingPeriod {
			get { return Macd.SignalSmoothingPeriod; }
			set { SetProperty("SignalSmoothingPeriod", value); }
		}
		[Category(Categories.Appearance)]
		public Color SignalLineColor {
			get { return Macd.SignalLineColor; }
			set { SetProperty("SignalLineColor", value); }
		}
		[Category(Categories.Appearance),
		 TypeConverter(typeof(ExpandableObjectConverter))]
		public LineStyle SignalLineStyle {
			get { return Macd.SignalLineStyle; }
		}
		[PropertyForOptions,
		 Category(Categories.Behavior)]
		public ValueLevel ValueLevel {
			get { return Macd.ValueLevel; }
			set { SetProperty("ValueLevel", value); }
		}
		public MovingAverageConvergenceDivergenceModel(MovingAverageConvergenceDivergence indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
		}
	}
	[ModelOf(typeof(RateOfChange))]
	public class RateOfChangeModel : SeparatePaneIndicatorModel {
		[PropertyForOptions(Categories.Behavior),
		Category(Categories.Behavior)]
		public int PointsCount {
			get { return ((RateOfChange)Indicator).PointsCount; }
			set { SetProperty("PointsCount", value); }
		}
		[PropertyForOptions(Categories.Behavior),
		Category(Categories.Behavior)]
		public ValueLevel ValueLevel {
			get { return ((RateOfChange)Indicator).ValueLevel; }
			set { SetProperty("ValueLevel", value); }
		}
		public RateOfChangeModel(RateOfChange indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
		}
	}
	[ModelOf(typeof(RelativeStrengthIndex))]
	public class RelativeStrengthIndexModel : SeparatePaneIndicatorModel {
		public RelativeStrengthIndexModel(RelativeStrengthIndex indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
		}
		[PropertyForOptions(Categories.Behavior),
		Category(Categories.Behavior)]
		public int PointsCount {
			get { return ((RelativeStrengthIndex)Indicator).PointsCount; }
			set { SetProperty("PointsCount", value); }
		}
		[PropertyForOptions,
		 Category(Categories.Behavior)]
		public ValueLevel ValueLevel {
			get { return ((RelativeStrengthIndex)Indicator).ValueLevel; }
			set { SetProperty("ValueLevel", value); }
		}
	}
	[ModelOf(typeof(TripleExponentialMovingAverageTema)), TypeConverter(typeof(MovingAverageTypeConverter))]
	public class TripleExponentialMovingAverageTemaModel : MovingAverageModel {
		public TripleExponentialMovingAverageTemaModel(TripleExponentialMovingAverageTema indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
		}
	}
	[ModelOf(typeof(TypicalPrice))]
	public class TypicalPriceModel : IndicatorModel {
		public TypicalPriceModel(TypicalPrice indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
		}
	}
	[ModelOf(typeof(ChaikinsVolatility))]
	public class ChaikinsVolatilityModel : SeparatePaneIndicatorModel {
		[PropertyForOptions(Categories.Behavior),
		 Category(Categories.Behavior)]
		public int PointsCount {
			get { return ((ChaikinsVolatility)Indicator).PointsCount; }
			set { SetProperty("PointsCount", value); }
		}
		public ChaikinsVolatilityModel(ChaikinsVolatility indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
		}
	}
	[ModelOf(typeof(WeightedClose))]
	public class WeightedCloseModel : IndicatorModel {
		public WeightedCloseModel(WeightedClose indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
		}
	}
	[ModelOf(typeof(WilliamsR))]
	public class WilliamsRModel : SeparatePaneIndicatorModel {
		[PropertyForOptions(Categories.Behavior),
		 Category(Categories.Behavior)]
		public int PointsCount {
			get { return ((WilliamsR)Indicator).PointsCount; }
			set { SetProperty("PointsCount", value); }
		}
		public WilliamsRModel(WilliamsR indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
		}
	}
	[ModelOf(typeof(TripleExponentialMovingAverageTrix))]
	public class TripleExponentialMovingAverageTrixModel : SeparatePaneIndicatorModel {
		public TripleExponentialMovingAverageTrixModel(TripleExponentialMovingAverageTrix indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
		}
		[PropertyForOptions,
		Category(Categories.Behavior)]
		public int PointsCount {
			get { return ((TripleExponentialMovingAverageTrix)Indicator).PointsCount; }
			set { SetProperty("PointsCount", value); }
		}
		[PropertyForOptions,
		 Category(Categories.Behavior)]
		public ValueLevel ValueLevel {
			get { return ((TripleExponentialMovingAverageTrix)Indicator).ValueLevel; }
			set { SetProperty("ValueLevel", value); }
		}
	}
	[ModelOf(typeof(BollingerBands))]
	public class BollingerBandsModel : IndicatorModel {
		public BollingerBandsModel(BollingerBands indicator, CommandManager commandManager)
			: base(indicator, commandManager) {
		}
		[PropertyForOptions(Categories.Behavior),
		Category(Categories.Behavior)]
		public int PointsCount {
			get { return ((BollingerBands)Indicator).PointsCount; }
			set { SetProperty("PointsCount", value); }
		}
		[PropertyForOptions,
		 Category(Categories.Behavior)]
		public ValueLevel ValueLevel {
			get { return ((BollingerBands)Indicator).ValueLevel; }
			set { SetProperty("ValueLevel", value); }
		}
		[PropertyForOptions,
		 Category(Categories.Appearance)]
		public Color BandsColor {
			get { return ((BollingerBands)Indicator).BandsColor; }
			set { SetProperty("BandsColor", value); }
		}
		[Category(Categories.Appearance),
		 TypeConverter(typeof(ExpandableObjectConverter))]
		public LineStyle BandsLineStyle {
			get { return ((BollingerBands)Indicator).BandsLineStyle; }
		}
		[PropertyForOptions,
		 Category(Categories.Behavior)]
		public double StandardDeviationMultiplier {
			get { return ((BollingerBands)Indicator).StandardDeviationMultiplier; }
			set { SetProperty("StandardDeviationMultiplier", value); }
		}
	}
}
