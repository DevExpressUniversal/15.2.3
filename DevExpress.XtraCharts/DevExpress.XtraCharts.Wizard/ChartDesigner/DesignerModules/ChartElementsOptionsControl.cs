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
using System.Windows.Forms;
using DevExpress.XtraCharts.Designer.Native;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Designer {
	public partial class ChartElementsOptionsControl : XtraUserControl {
		readonly Dictionary<Type, ElementsOptionsControlBase> optionsControls = new Dictionary<Type, ElementsOptionsControlBase>();
		OptionsControlFactory optionsControlFactory;
		DesignerChartElementModelBase previousSelectedModel;
		DesignerChartElementModelBase selectedModel;
		internal Dictionary<Type, ElementsOptionsControlBase> OptionsControls { get { return optionsControls; } }
		public DesignerChartElementModelBase SelectedModel {
			get { return selectedModel; }
			set {
				if (selectedModel != value) {
					previousSelectedModel = selectedModel;
					selectedModel = value;
					OnSelectedModelChanged();
				}
			}
		}
		public ChartElementsOptionsControl() {
			InitializeComponent();
			optionsControlFactory = new OptionsControlFactory();
		}
		bool TryGetOptionsControl(Type type, out ElementsOptionsControlBase optionsControl) {
			if (optionsControls.TryGetValue(type, out optionsControl)) {
				return true;
			}
			optionsControl = optionsControlFactory.CreateOptionsControl(type);
			if (optionsControl == null)
				return false;
			optionsControls.Add(type, optionsControl);
			return true;
		}
		bool NeedReloadModel(object commandParameter, ElementsOptionsControlBase optionsControl) {
			string setPropertyParameter = commandParameter as string;
			return optionsControl is SeriesOptionsControl && (setPropertyParameter == "View" || commandParameter is ViewType);
		}
		void OnSelectedModelChanged() {
			if (previousSelectedModel != null) {
				ElementsOptionsControlBase previousOptionsControl = null;
				if (TryGetOptionsControl(previousSelectedModel.GetType(), out previousOptionsControl))
					((IView)previousOptionsControl).RemoveListener(previousSelectedModel);
			}
			Controls.Clear();
			if (selectedModel != null) {
				ElementsOptionsControlBase optionsControl = null;
				if (TryGetOptionsControl(selectedModel.GetType(), out optionsControl)) {
					((IView)optionsControl).AddListener(selectedModel);
					((IView)optionsControl).LoadModel(selectedModel);
					Controls.Add(optionsControl);
				}
			}
		}
		internal void UpdateOptions(object commandParameter) {			
			ElementsOptionsControlBase optionsControl = null;
			if (selectedModel == null) {
				if (previousSelectedModel != null) {
					ElementsOptionsControlBase previousOptionsControl = null;
					if (TryGetOptionsControl(previousSelectedModel.GetType(), out previousOptionsControl))
						((IView)previousOptionsControl).RemoveListener(previousSelectedModel);
				}
				Controls.Clear();
				return;
			}
			if (TryGetOptionsControl(selectedModel.GetType(), out optionsControl)) {
				if (NeedReloadModel(commandParameter, optionsControl)) {
					((IView)optionsControl).AddListener(selectedModel);
					((IView)optionsControl).LoadModel(selectedModel);
				} else {
					((IView)optionsControl).UpdateData();
					((IView)optionsControl).UpdateActivity(commandParameter as string);
				}
			}
		}		
	}
	public class OptionsControlFactory {
		readonly Dictionary<Type, Func<ElementsOptionsControlBase>> optionsControlFactory = new Dictionary<Type, Func<ElementsOptionsControlBase>>();
		public OptionsControlFactory() {
			optionsControlFactory.Add(typeof(AxisXModel), () => { return new AxisOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(AxisYModel), () => { return new AxisOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(PolarAxisXModel), () => { return new PolarAxisOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(RadarAxisXModel), () => { return new RadarAxisXOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(RadarAxisYModel), () => { return new RadarAxisYOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(SecondaryAxisXModel), () => { return new AxisOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(SecondaryAxisYModel), () => { return new AxisOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(AxisX3DModel), () => { return new Axis3DOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(AxisY3DModel), () => { return new Axis3DOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(GanttAxisXModel), () => { return new AxisOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(SwiftPlotDiagramAxisXModel), () => { return new Axis2DOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(SwiftPlotDiagramAxisYModel), () => { return new Axis2DOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(SwiftPlotDiagramSecondaryAxisXModel), () => { return new Axis2DOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(SwiftPlotDiagramSecondaryAxisYModel), () => { return new Axis2DOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(DesignerChartModel), () => { return new ChartOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(LegendModel), () => { return new LegendOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(XYDiagramModel), () => { return new XYDiagramOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(SimpleDiagramModel), () => { return new SimpleDiagramOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(SimpleDiagram3DModel), () => { return new SimpleDiagram3DOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(XYDiagram3DModel), () => { return new XYDiagram3DOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(FunnelDiagram3DModel), () => { return new SimpleDiagram3DOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(RadarDiagramModel), () => { return new RadarDiagramOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(PolarDiagramModel), () => { return new RadarDiagramOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(SwiftPlotDiagramModel), () => { return new SwiftPlotOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(GanttDiagramModel), () => { return new XYDiagramOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(XYDiagramDefaultPaneModel), () => { return new PaneOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(XYDiagramPaneModel), () => { return new PaneOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(DesignerSeriesModel), () => { return new SeriesOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(DesignerSeriesModelBase), () => { return new SeriesBaseOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(ImageAnnotationModel), () => { return new ImageAnnotationOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(TextAnnotationModel), () => { return new TextAnnotationOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(FibonacciIndicatorModel), () => { return new FibonacciIndicatorOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(ExponentialMovingAverageModel), () => { return new MovingAverageOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(SimpleMovingAverageModel), () => { return new MovingAverageOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(TriangularMovingAverageModel), () => { return new MovingAverageOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(WeightedMovingAverageModel), () => { return new MovingAverageOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(TripleExponentialMovingAverageTemaModel), () => { return new MovingAverageOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(RegressionLineModel), () => { return new RegressionLineOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(TrendLineModel), () => { return new TrendLineOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(ConstantLineModel), () => { return new ConstantLineOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(ScaleBreakModel), () => { return new ScaleBreakOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(StripModel), () => { return new StripOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(ChartTitleModel), () => { return new ChartTitleOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(SeriesTitleModel), () => { return new SeriesTitleOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(SideBySideBarSeriesLabelModel), () => { return new BarSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(StackedBarSeriesLabelModel), () => { return new StackedBarSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(FullStackedBarSeriesLabelModel), () => { return new StackedBarSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(Bar3DSeriesLabelModel), () => { return new Bar3DSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(StackedBar3DSeriesLabelModel), () => { return new StackedBar3DSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(FullStackedBar3DSeriesLabelModel), () => { return new StackedBar3DSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(PointSeriesLabelModel), () => { return new PointSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(BubbleSeriesLabelModel), () => { return new BubbleSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(StackedLineSeriesLabelModel), () => { return new PointSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(Line3DSeriesLabelModel), () => { return new Line3DSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(StackedLine3DSeriesLabelModel), () => { return new Line3DSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(FullStackedAreaSeriesLabelModel), () => { return new FullStackedAreaSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(Area3DSeriesLabelModel), () => { return new Line3DSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(StackedArea3DSeriesLabelModel), () => { return new StackedArea3DSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(FullStackedArea3DSeriesLabelModel), () => { return new StackedArea3DSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(RangeBarSeriesLabelModel), () => { return new RangeBarSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(RangeAreaSeriesLabelModel), () => { return new RangeAreaSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(RangeArea3DSeriesLabelModel), () => { return new RangeArea3DSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(PieSeriesLabelModel), () => { return new PieSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(DoughnutSeriesLabelModel), () => { return new PieSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(NestedDoughnutSeriesLabelModel), () => { return new PieSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(Pie3DSeriesLabelModel), () => { return new Pie3DSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(Doughnut3DSeriesLabelModel), () => { return new Pie3DSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(FunnelSeriesLabelModel), () => { return new FunnelSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(Funnel3DSeriesLabelModel), () => { return new Funnel3DSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(RadarPointSeriesLabelModel), () => { return new PointSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(StockSeriesLabelModel), () => { return new StockSeriesLabelOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(AverageTrueRangeModel), () => { return new AverageTrueRangeOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(ChaikinsVolatilityModel), () => { return new ChaikinsVolatilityOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(CommodityChannelIndexModel), () => { return new CommodityChannelIndexOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(DetrendedPriceOscillatorModel), () => { return new DetrendedPriceOscillatorOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(MassIndexModel), () => { return new MassIndexOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(MovingAverageConvergenceDivergenceModel), () => { return new MovingAverageConvergenceDivergenceOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(RateOfChangeModel), () => { return new RateOfChangeOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(RelativeStrengthIndexModel), () => { return new RelativeStrengthIndexOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(StandardDeviationModel), () => { return new StandardDeviationOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(TripleExponentialMovingAverageTrixModel), () => { return new TripleExponentialMovingAverageTrixOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(WilliamsRModel), () => { return new WilliamsROptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(BollingerBandsModel), () => { return new BollingerBandsIndicatorOptions() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(MedianPriceModel), () => { return new MedianPriceOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(TypicalPriceModel), () => { return new TypicalPriceOptionsControl() { Dock = DockStyle.Fill }; });
			optionsControlFactory.Add(typeof(WeightedCloseModel), () => { return new WeightedCloseOptionsControl() { Dock = DockStyle.Fill }; });
		}
		public ElementsOptionsControlBase CreateOptionsControl(Type type) {
			if (optionsControlFactory.ContainsKey(type))
				return optionsControlFactory[type]();
			return null;
		}
	}
}
