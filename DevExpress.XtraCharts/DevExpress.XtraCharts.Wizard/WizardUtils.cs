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

using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Localization;
using DevExpress.Utils;
using DevExpress.XtraCharts.Designer.Native;
using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Linq;
using DevExpress.XtraCharts.Native;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public delegate string ExtractNameMethod<ItemClass>(ItemClass item);
	public class ItemContainer<ItemClass> {
		ExtractNameMethod<ItemClass> ExtractName;
		ItemClass item;
		public ItemContainer(ItemClass item, ExtractNameMethod<ItemClass> extractName) {
			this.ExtractName = extractName;
			this.item = item;
		}
		public ItemClass Item { get { return item; } }
		public string Name { get { return ExtractName(item); } }
		public ItemContainer() {
		}
		public override string ToString() {
			return Name;
		}
	}
	public class ItemContainerMap<ItemClass> {
		ExtractNameMethod<ItemClass> ExtractName;
		public ItemContainer<ItemClass> this[ItemClass item] { get { return items[item]; } }
		Dictionary<ItemClass, ItemContainer<ItemClass>> items = new Dictionary<ItemClass, ItemContainer<ItemClass>>();
		public ItemContainerMap(ExtractNameMethod<ItemClass> extractName) {
			this.ExtractName = extractName;
		}
		public ItemContainer<ItemClass> Add(ItemClass item) {
			ItemContainer<ItemClass> container = new ItemContainer<ItemClass>(item, ExtractName);
			this.items.Add(item, container);
			return container;
		}
	}
	public static class ComboHelper { 
		public static void FillHatchStyle(ComboBoxEdit comboBox) {
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchMin));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchHorizontal));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchVertical));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchForwardDiagonal));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchBackwardDiagonal));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchMax));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchCross));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchLargeGrid));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchDiagonalCross));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchPercent05));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchPercent10));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchPercent20));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchPercent25));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchPercent30));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchPercent40));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchPercent50));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchPercent60));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchPercent70));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchPercent75));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchPercent80));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchPercent90));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchLightDownwardDiagonal));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchLightUpwardDiagonal));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchDarkDownwardDiagonal));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchDarkUpwardDiagonal));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchWideDownwardDiagonal));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchWideUpwardDiagonal));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchLightVertical));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchLightHorizontal));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchNarrowVertical));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchNarrowHorizontal));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchDarkVertical));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchDarkHorizontal));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchDashedDownwardDiagonal));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchDashedUpwardDiagonal));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchDashedHorizontal));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchDashedVertical));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchSmallConfetti));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchLargeConfetti));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchZigZag));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchWave));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchDiagonalBrick));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchHorizontalBrick));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchWeave));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchPlaid));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchDivot));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchDottedGrid));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchDottedDiamond));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchShingle));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchTrellis));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchSphere));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchSmallGrid));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchSmallCheckerBoard));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchLargeCheckerBoard));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchOutlinedDiamond));
			comboBox.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.WizHatchSolidDiamond));
		}
	}
	public class CheckEditHelper {
		public static void SetCheckEditState(CheckEdit checkEdit, DefaultBoolean value) {
			checkEdit.CheckState = value == DefaultBoolean.Default ? CheckState.Indeterminate : 
				value == DefaultBoolean.True ? CheckState.Checked : CheckState.Unchecked;
		}
		public static DefaultBoolean GetCheckEditState(CheckEdit checkEdit) {
			return checkEdit.CheckState == CheckState.Indeterminate ? DefaultBoolean.Default :
				checkEdit.CheckState == CheckState.Checked ? DefaultBoolean.True : DefaultBoolean.False;
		}
	}
	public interface IXtraChartsWizardReference {
	}
	[SecuritySafeCritical]
	public static class ReflectionHelper {
		public static Assembly XtraChartAssembly { get { return typeof(Chart).Assembly; } }
		public static Assembly WizardAssembly { get { return typeof(ReflectionHelper).Assembly; } }
		public static Type[] GetTypeDescendants(Type ancestorType) {
			return GetTypeDescendants(WizardAssembly, ancestorType, new List<Type>());
		}
		public static Type[] GetTypeDescendants(Assembly assembly, Type ancestorType, List<Type> ignoredTypes) {
			Type[] types = assembly.GetTypes();
			List<Type> result = new List<Type>();
			foreach(Type item in types) {
				object[] obsoleteAttributes = item.GetCustomAttributes(typeof(ObsoleteAttribute), true);
				bool itTestClass = item.Namespace != null && item.Namespace.ToLower().Contains(".test");
				if(ancestorType.IsAssignableFrom(item) && item.IsClass && !item.IsAbstract && !ignoredTypes.Contains(item) && obsoleteAttributes.Length == 0 && !itTestClass)
					result.Add(item);
			}
			return result.ToArray();
		}
		public static bool HasAttribute<T>(PropertyInfo property) {
			if(property == null)
				return false;
			object[] attributes = property.GetCustomAttributes(typeof(T), false);
			return attributes != null && attributes.Length > 0;
		}
		public static bool HasAttribute<T>(Type type) {
			if(type == null)
				return false;
			object[] attributes = type.GetCustomAttributes(typeof(T), false);
			return attributes != null && attributes.Length > 0;
		}
		public static T GetAttribute<T>(PropertyInfo property) where T : class {
			if(property == null)
				return null;
			object[] attributes = property.GetCustomAttributes(typeof(T), false);
			return attributes != null && attributes.Length > 0 ? attributes[0] as T : null;
		}
		public static T GetAttribute<T>(Type type) where T : class {
			if(type == null)
				return null;
			object[] attributes = type.GetCustomAttributes(typeof(T), false);
			return attributes != null && attributes.Length > 0 ? attributes[0] as T : null;
		}
		public static object CreateEmptyObject(Type type) {
			return FormatterServices.GetUninitializedObject(type);
		}
		public static PropertyInfo GetComplexProperty(Type type, string propertyName) {
			string[] parts = propertyName.Split(new char[] {'.'}, 2);
			PropertyInfo property = type.GetProperty(parts[0]);
			if(parts.Length == 1 || property == null)
				return property;
			return GetComplexProperty(property.PropertyType, parts[1]);
		}
		public static object GetComplexPropertyValue(string propertyName, object obj) {
			Type type = obj.GetType();
			string[] parts = propertyName.Split(new char[] { '.' }, 2);
			PropertyInfo property = type.GetProperty(parts[0]);
			if(property == null)
				return null;
			if(parts.Length == 1)
				return property.GetValue(obj, null);
			return GetComplexPropertyValue(parts[1], property.GetValue(obj, null));
		}
		public static PropertyInfo GetProperty(string propertyName, Type type) {
			PropertyInfo[] infos = type.GetProperties().Where(info => info.Name == propertyName).ToArray();
			if (infos.Length == 0)
				return null;
			if (infos.Length == 1)
				return infos[0];
			PropertyInfo declaredProperty = null;
			Type declaringType = type;
			while (declaredProperty == null && !declaringType.Equals(typeof(DesignerChartElementModelBase))) {
				PropertyInfo[] declaredInfos = infos.Where(info => info.DeclaringType.Equals(declaringType)).ToArray();
				if (declaredInfos.Length > 1)
					return null;
				if (declaredInfos.Length == 1)
					return declaredInfos[0];
				if (declaredInfos.Length == 0)
					declaringType = declaringType.BaseType;
			}
			return null;
		}
	}
	public static class ModelHelper {
		public static DesignerChartElementModelBase CreateModelByInstanceAndType(object chartElement, Type type, CommandManager commandManager) {
			Type[] modelTypes = ReflectionHelper.GetTypeDescendants(typeof(DesignerChartElementModelBase));
			foreach(Type modelType in modelTypes) {
				object[] attributes = modelType.GetCustomAttributes(typeof(ModelOf), true);
				if(attributes.Length < 1)
					continue;
				ModelOf modelAttribute = (ModelOf)attributes[0];
				Type chartElementType = modelAttribute.ChartElementType;
				if(chartElementType != null && chartElementType.Equals(type))
					return (DesignerChartElementModelBase)Activator.CreateInstance(modelType, new object[] { chartElement, commandManager });
			}
			return null;
		}
		public static DesignerChartElementModelBase CreateModelInstance(object chartElement, CommandManager commandManager) {
			if(chartElement == null)
				return null;
			return CreateModelByInstanceAndType(chartElement, chartElement.GetType(), commandManager);
		}
		public static SeriesViewModelBase CreateSeriesViewModelInstance(SeriesViewBase view, CommandManager commandManager) {
			if(view == null)
				return null;
			if(view is SideBySideBarSeriesView)
				return new SideBySideBarViewModel((SideBySideBarSeriesView)view, commandManager);
			if(view is SideBySideFullStackedBarSeriesView)
				return new SideBySideFullStackedBarViewModel((SideBySideFullStackedBarSeriesView)view, commandManager);
			if(view is FullStackedBarSeriesView)
				return new FullStackedBarViewModel((FullStackedBarSeriesView)view, commandManager);
			if(view is SideBySideStackedBarSeriesView)
				return new SideBySideStackedBarViewModel((SideBySideStackedBarSeriesView)view, commandManager);
			if(view is StackedBarSeriesView)
				return new StackedBarViewModel((StackedBarSeriesView)view, commandManager);
			if(view is NestedDoughnutSeriesView)
				return new NestedDoughnutViewModel((NestedDoughnutSeriesView)view, commandManager);
			if(view is DoughnutSeriesView)
				return new DoughnutViewModel((DoughnutSeriesView)view, commandManager);
			if(view is PieSeriesView)
				return new PieViewModel((PieSeriesView)view, commandManager);
			if(view is FunnelSeriesView)
				return new FunnelViewModel((FunnelSeriesView)view, commandManager);
			if(view is FullStackedLineSeriesView)
				return new FullStackedLineViewModel((FullStackedLineSeriesView)view, commandManager);
			if(view is StackedLineSeriesView)
				return new StackedLineViewModel((StackedLineSeriesView)view, commandManager);
			if(view is StepLineSeriesView)
				return new StepLineViewModel((StepLineSeriesView)view, commandManager);
			if(view is SplineSeriesView)
				return new SplineViewModel((SplineSeriesView)view, commandManager);
			if(view is ScatterLineSeriesView)
				return new ScatterLineViewModel((ScatterLineSeriesView)view, commandManager);
			if(view is StepAreaSeriesView)
				return new StepAreaViewModel((StepAreaSeriesView)view, commandManager);
			if(view is SplineAreaSeriesView)
				return new SplineAreaViewModel((SplineAreaSeriesView)view, commandManager);
			if(view is StackedSplineAreaSeriesView)
				return new StackedSplineAreaViewModel((StackedSplineAreaSeriesView)view, commandManager);
			if(view is FullStackedSplineAreaSeriesView)
				return new FullStackedSplineAreaViewModel((FullStackedSplineAreaSeriesView)view, commandManager);
			if(view is FullStackedAreaSeriesView)
				return new FullStackedAreaViewModel((FullStackedAreaSeriesView)view, commandManager);
			if(view is StackedAreaSeriesView)
				return new StackedAreaViewModel((StackedAreaSeriesView)view, commandManager);
			if(view is RangeAreaSeriesView)
				return new RangeAreaViewModel((RangeAreaSeriesView)view, commandManager);
			if(view is AreaSeriesView)
				return new AreaViewModel((AreaSeriesView)view, commandManager);
			if(view is LineSeriesView)
				return new LineViewModel((LineSeriesView)view, commandManager);
			if(view is PointSeriesView)
				return new PointViewModel((PointSeriesView)view, commandManager);
			if(view is BubbleSeriesView)
				return new BubbleViewModel((BubbleSeriesView)view, commandManager);
			if(view is SwiftPlotSeriesView)
				return new SwiftPlotViewModel((SwiftPlotSeriesView)view, commandManager);
			if(view is StockSeriesView)
				return new StockViewModel((StockSeriesView)view, commandManager);
			if(view is CandleStickSeriesView)
				return new CandleStickViewModel((CandleStickSeriesView)view, commandManager);
			if(view is SideBySideRangeBarSeriesView)
				return new SideBySideRangeBarViewModel((SideBySideRangeBarSeriesView)view, commandManager);
			if(view is OverlappedRangeBarSeriesView)
				return new OverlappedRangeBarViewModel((OverlappedRangeBarSeriesView)view, commandManager);
			if(view is SideBySideGanttSeriesView)
				return new SideBySideGanttViewModel((SideBySideGanttSeriesView)view, commandManager);
			if(view is OverlappedGanttSeriesView)
				return new OverlappedGanttViewModel((OverlappedGanttSeriesView)view, commandManager);
			if(view is PolarPointSeriesView)
				return new PolarPointViewModel((PolarPointSeriesView)view, commandManager);
			if(view is ScatterPolarLineSeriesView)
				return new ScatterPolarLineViewModel((ScatterPolarLineSeriesView)view, commandManager);
			if(view is PolarLineSeriesView)
				return new PolarLineViewModel((PolarLineSeriesView)view, commandManager);
			if(view is PolarAreaSeriesView)
				return new PolarAreaViewModel((PolarAreaSeriesView)view, commandManager);
			if(view is ScatterRadarLineSeriesView)
				return new ScatterRadarLineViewModel((ScatterRadarLineSeriesView)view, commandManager);
			if(view is RadarAreaSeriesView)
				return new RadarAreaViewModel((RadarAreaSeriesView)view, commandManager);
			if(view is RadarLineSeriesView)
				return new RadarLineViewModel((RadarLineSeriesView)view, commandManager);
			if(view is RadarPointSeriesView)
				return new RadarPointViewModel((RadarPointSeriesView)view, commandManager);
			if(view is SideBySideBar3DSeriesView)
				return new SideBySideBar3DViewModel((SideBySideBar3DSeriesView)view, commandManager);
			if(view is SideBySideStackedBar3DSeriesView)
				return new SideBySideStackedBar3DViewModel((SideBySideStackedBar3DSeriesView)view, commandManager);
			if(view is SideBySideFullStackedBar3DSeriesView)
				return new SideBySideFullStackedBar3DViewModel((SideBySideFullStackedBar3DSeriesView)view, commandManager);
			if(view is FullStackedBar3DSeriesView)
				return new FullStackedBar3DViewModel((FullStackedBar3DSeriesView)view, commandManager);
			if(view is StackedBar3DSeriesView)
				return new StackedBar3DViewModel((StackedBar3DSeriesView)view, commandManager);
			if(view is ManhattanBarSeriesView)
				return new ManhattanBarViewModel((ManhattanBarSeriesView)view, commandManager);
			if(view is Doughnut3DSeriesView)
				return new Doughnut3DViewModel((Doughnut3DSeriesView)view, commandManager);
			if(view is Pie3DSeriesView)
				return new Pie3DViewModel((Pie3DSeriesView)view, commandManager);
			if(view is Funnel3DSeriesView)
				return new Funnel3DViewModel((Funnel3DSeriesView)view, commandManager);
			if(view is FullStackedLine3DSeriesView)
				return new FullStackedLine3DViewModel((FullStackedLine3DSeriesView)view, commandManager);
			if(view is StackedLine3DSeriesView)
				return new StackedLine3DViewModel((StackedLine3DSeriesView)view, commandManager);
			if(view is StepLine3DSeriesView)
				return new StepLine3DViewModel((StepLine3DSeriesView)view, commandManager);
			if(view is FullStackedSplineArea3DSeriesView)
				return new FullStackedSplineArea3DViewModel((FullStackedSplineArea3DSeriesView)view, commandManager);
			if(view is FullStackedArea3DSeriesView)
				return new FullStackedArea3DViewModel((FullStackedArea3DSeriesView)view, commandManager);
			if(view is StackedSplineArea3DSeriesView)
				return new StackedSplineArea3DViewModel((StackedSplineArea3DSeriesView)view, commandManager);
			if(view is StackedArea3DSeriesView)
				return new StackedArea3DViewModel((StackedArea3DSeriesView)view, commandManager);
			if(view is StepArea3DSeriesView)
				return new StepArea3DViewModel((StepArea3DSeriesView)view, commandManager);
			if(view is SplineArea3DSeriesView)
				return new SplineArea3DViewModel((SplineArea3DSeriesView)view, commandManager);
			if(view is RangeArea3DSeriesView)
				return new RangeArea3DViewModel((RangeArea3DSeriesView)view, commandManager);
			if(view is Area3DSeriesView)
				return new Area3DViewModel((Area3DSeriesView)view, commandManager);
			if(view is Spline3DSeriesView)
				return new Spline3DViewModel((Spline3DSeriesView)view, commandManager);
			if(view is Line3DSeriesView)
				return new Line3DViewModel((Line3DSeriesView)view, commandManager);
			return null;
		}
		public static DesignerDiagramModel CreateDiagramModelInstance(Diagram diagram, CommandManager commandManager, Chart chart) {
			if(diagram == null)
				return null;
			if(diagram is FunnelDiagram3D)
				return new FunnelDiagram3DModel((FunnelDiagram3D)diagram, commandManager);
			if(diagram is SimpleDiagram3D)
				return new SimpleDiagram3DModel((SimpleDiagram3D)diagram, commandManager);
			if(diagram is SwiftPlotDiagram)
				return new SwiftPlotDiagramModel((SwiftPlotDiagram)diagram, commandManager, chart);
			if(diagram is GanttDiagram)
				return new GanttDiagramModel((GanttDiagram)diagram, commandManager, chart);
			if(diagram is XYDiagram)
				return new XYDiagramModel((XYDiagram)diagram, commandManager, chart);
			if(diagram is PolarDiagram)
				return new PolarDiagramModel((PolarDiagram)diagram, commandManager);
			if(diagram is RadarDiagram)
				return new RadarDiagramModel((RadarDiagram)diagram, commandManager);
			if(diagram is SimpleDiagram)
				return new SimpleDiagramModel((SimpleDiagram)diagram, commandManager);
			if(diagram is XYDiagram3D)
				return new XYDiagram3DModel((XYDiagram3D)diagram, commandManager);
			return null;
		}
		public static IndicatorModel CreateIndicatorModelInstance(Indicator indicator, CommandManager commandManager) {
			if (indicator == null)
				return null;
			else if (indicator is FibonacciIndicator)
				return new FibonacciIndicatorModel((FibonacciIndicator)indicator, commandManager);
			else if (indicator is TrendLine)
				return new TrendLineModel((TrendLine)indicator, commandManager);
			else if (indicator is RegressionLine)
				return new RegressionLineModel((RegressionLine)indicator, commandManager);
			else if (indicator is ExponentialMovingAverage)
				return new ExponentialMovingAverageModel((ExponentialMovingAverage)indicator, commandManager);
			else if (indicator is SimpleMovingAverage)
				return new SimpleMovingAverageModel((SimpleMovingAverage)indicator, commandManager);
			else if (indicator is TriangularMovingAverage)
				return new TriangularMovingAverageModel((TriangularMovingAverage)indicator, commandManager);
			else if (indicator is WeightedMovingAverage)
				return new WeightedMovingAverageModel((WeightedMovingAverage)indicator, commandManager);
			else if (indicator is StandardDeviation)
				return new StandardDeviationModel((StandardDeviation)indicator, commandManager);
			else if (indicator is AverageTrueRange)
				return new AverageTrueRangeModel((AverageTrueRange)indicator, commandManager);
			else if (indicator is CommodityChannelIndex)
				return new CommodityChannelIndexModel((CommodityChannelIndex)indicator, commandManager);
			else if (indicator is DetrendedPriceOscillator)
				return new DetrendedPriceOscillatorModel((DetrendedPriceOscillator)indicator, commandManager);
			else if (indicator is MassIndex)
				return new MassIndexModel((MassIndex)indicator, commandManager);
			else if (indicator is MedianPrice)
				return new MedianPriceModel((MedianPrice)indicator, commandManager);
			else if (indicator is MovingAverageConvergenceDivergence)
				return new MovingAverageConvergenceDivergenceModel((MovingAverageConvergenceDivergence)indicator, commandManager);
			else if (indicator is RateOfChange)
				return new RateOfChangeModel((RateOfChange)indicator, commandManager);
			else if (indicator is RelativeStrengthIndex)
				return new RelativeStrengthIndexModel((RelativeStrengthIndex)indicator, commandManager);
			else if (indicator is TripleExponentialMovingAverageTema)
				return new TripleExponentialMovingAverageTemaModel((TripleExponentialMovingAverageTema)indicator, commandManager);
			else if (indicator is TypicalPrice)
				return new TypicalPriceModel((TypicalPrice)indicator, commandManager);
			else if (indicator is ChaikinsVolatility)
				return new ChaikinsVolatilityModel((ChaikinsVolatility)indicator, commandManager);
			else if (indicator is WeightedClose)
				return new WeightedCloseModel((WeightedClose)indicator, commandManager);
			else if (indicator is WilliamsR)
				return new WilliamsRModel((WilliamsR)indicator, commandManager);
			else if (indicator is TripleExponentialMovingAverageTrix)
				return new TripleExponentialMovingAverageTrixModel((TripleExponentialMovingAverageTrix)indicator, commandManager);
			else if (indicator is BollingerBands)
				return new BollingerBandsModel((BollingerBands)indicator, commandManager);
			return null;
		}
		public static AnnotationModel CreateAnnotationModelInstance(Annotation annotation, CommandManager commandManager) {
			if(annotation == null)
				return null;
			if(annotation is TextAnnotation)
				return new TextAnnotationModel((TextAnnotation)annotation, commandManager);
			if(annotation is ImageAnnotation)
				return new ImageAnnotationModel((ImageAnnotation)annotation, commandManager);
			return null;
		}
		public static AnnotationAnchorPointModel CreateAnnotationAnchorPointModelInstance(AnnotationAnchorPoint anchorPoint, CommandManager commandManager) {
			if(anchorPoint == null)
				return null;
			if(anchorPoint is ChartAnchorPoint)
				return new ChartAnchorPointModel((ChartAnchorPoint)anchorPoint, commandManager);
			if(anchorPoint is PaneAnchorPoint)
				return new PaneAnchorPointModel((PaneAnchorPoint)anchorPoint, commandManager);
			if(anchorPoint is SeriesPointAnchorPoint)
				return new SeriesPointAnchorPointModel((SeriesPointAnchorPoint)anchorPoint, commandManager);
			return null;
		}
		public static AnnotationShapePositionModel CreateAnnotationShapePositionModelInstance(AnnotationShapePosition shapePosition, CommandManager commandManager) {
			if(shapePosition == null)
				return null;
			if(shapePosition is FreePosition)
				return new FreePositionModel((FreePosition)shapePosition, commandManager);
			if(shapePosition is RelativePosition)
				return new RelativePositionModel((RelativePosition)shapePosition, commandManager);
			return null;
		}
		public static BorderBaseModel CreateBorderModelInstance(BorderBase border, CommandManager commandManager) {
			if(border == null)
				return null;
			if(border is CustomBorder)
				return new CustomBorderModel((CustomBorder)border, commandManager);
			if(border is OutsideRectangularBorder)
				return new OutsideRectangularBorderModel((OutsideRectangularBorder)border, commandManager);
			if(border is InsideRectangularBorder)
				return new InsideRectangularBorderModel((InsideRectangularBorder)border, commandManager);
			return null;
		}
		public static FillOptionsBaseModel CreateFillOptionsBaseModelInstance(FillOptionsBase options, CommandManager commandManager) {
			if(options == null)
				return null;
			if(options is SolidFillOptions)
				return new SolidFillOptionsModel((SolidFillOptions)options, commandManager);
			if(options is RectangleGradientFillOptions)
				return new RectangleGradientFillOptionsModel((RectangleGradientFillOptions)options, commandManager);
			if(options is PolygonGradientFillOptions)
				return new PolygonGradientFillOptionsModel((PolygonGradientFillOptions)options, commandManager);
			if(options is HatchFillOptions)
				return new HatchFillOptionsModel((HatchFillOptions)options, commandManager);
			return null;
		}
		public static SeriesLabelBaseModel CreateSeriesLabelBaseModelInstance(SeriesLabelBase label, CommandManager commandManager) {
			if(label == null)
				return null;
			if(label is RangeArea3DSeriesLabel)
				return new RangeArea3DSeriesLabelModel((RangeArea3DSeriesLabel)label, commandManager);
			if(label is RangeAreaSeriesLabel)
				return new RangeAreaSeriesLabelModel((RangeAreaSeriesLabel)label, commandManager);
			if(label is BubbleSeriesLabel)
				return new BubbleSeriesLabelModel((BubbleSeriesLabel)label, commandManager);
			if(label is StackedLineSeriesLabel)
				return new StackedLineSeriesLabelModel((StackedLineSeriesLabel)label, commandManager);
			if(label is RadarPointSeriesLabel)
				return new RadarPointSeriesLabelModel((RadarPointSeriesLabel)label, commandManager);
			if(label is PointSeriesLabel)
				return new PointSeriesLabelModel((PointSeriesLabel)label, commandManager);
			if(label is Funnel3DSeriesLabel)
				return new Funnel3DSeriesLabelModel((Funnel3DSeriesLabel)label, commandManager);
			if(label is FunnelSeriesLabel)
				return new FunnelSeriesLabelModel((FunnelSeriesLabel)label, commandManager);
			if(label is StackedLine3DSeriesLabel)
				return new StackedLine3DSeriesLabelModel((StackedLine3DSeriesLabel)label, commandManager);
			if(label is Area3DSeriesLabel)
				return new Area3DSeriesLabelModel((Area3DSeriesLabel)label, commandManager);
			if(label is Line3DSeriesLabel)
				return new Line3DSeriesLabelModel((Line3DSeriesLabel)label, commandManager);
			if(label is NestedDoughnutSeriesLabel)
				return new NestedDoughnutSeriesLabelModel((NestedDoughnutSeriesLabel)label, commandManager);
			if(label is DoughnutSeriesLabel)
				return new DoughnutSeriesLabelModel((DoughnutSeriesLabel)label, commandManager);
			if(label is Doughnut3DSeriesLabel)
				return new Doughnut3DSeriesLabelModel((Doughnut3DSeriesLabel)label, commandManager);
			if(label is Pie3DSeriesLabel)
				return new Pie3DSeriesLabelModel((Pie3DSeriesLabel)label, commandManager);
			if(label is PieSeriesLabel)
				return new PieSeriesLabelModel((PieSeriesLabel)label, commandManager);
			if(label is Bar3DSeriesLabel)
				return new Bar3DSeriesLabelModel((Bar3DSeriesLabel)label, commandManager);
			if(label is FullStackedArea3DSeriesLabel)
				return new FullStackedArea3DSeriesLabelModel((FullStackedArea3DSeriesLabel)label, commandManager);
			if(label is StackedArea3DSeriesLabel)
				return new StackedArea3DSeriesLabelModel((StackedArea3DSeriesLabel)label, commandManager);
			if(label is FullStackedAreaSeriesLabel)
				return new FullStackedAreaSeriesLabelModel((FullStackedAreaSeriesLabel)label, commandManager);
			if(label is RangeBarSeriesLabel)
				return new RangeBarSeriesLabelModel((RangeBarSeriesLabel)label, commandManager);
			if(label is SideBySideBarSeriesLabel)
				return new SideBySideBarSeriesLabelModel((SideBySideBarSeriesLabel)label, commandManager);
			if(label is FullStackedBar3DSeriesLabel)
				return new FullStackedBar3DSeriesLabelModel((FullStackedBar3DSeriesLabel)label, commandManager);
			if(label is StackedBar3DSeriesLabel)
				return new StackedBar3DSeriesLabelModel((StackedBar3DSeriesLabel)label, commandManager);
			if(label is FullStackedBarSeriesLabel)
				return new FullStackedBarSeriesLabelModel((FullStackedBarSeriesLabel)label, commandManager);
			if(label is StackedBarSeriesLabel)
				return new StackedBarSeriesLabelModel((StackedBarSeriesLabel)label, commandManager);
			if(label is StockSeriesLabel)
				return new StockSeriesLabelModel((StockSeriesLabel)label, commandManager);
			return null;
		}
		public static ChartColorizerBaseModel CreateChartColorizerBaseModelInstance(ChartColorizerBase colorizer, CommandManager commandManager) {
			if(colorizer == null)
				return null;
			if(colorizer is ColorObjectColorizer)
				return new ColorObjectColorizerModel((ColorObjectColorizer)colorizer, commandManager);
			if(colorizer is KeyColorColorizer)
				return new KeyColorColorizerModel((KeyColorColorizer)colorizer, commandManager);
			if(colorizer is RangeColorizer)
				return new RangeColorizerModel((RangeColorizer)colorizer, commandManager);
			return null;
		}
		public static CrosshairLabelPositionModel CreateCrosshairLabelPositionModelInstance(CrosshairLabelPosition labelPosition, CommandManager commandManager) {
			if(labelPosition == null)
				return null;
			if(labelPosition is CrosshairMousePosition)
				return new CrosshairMousePositionModel((CrosshairMousePosition)labelPosition, commandManager);
			if(labelPosition is CrosshairFreePosition)
				return new CrosshairFreePositionModel((CrosshairFreePosition)labelPosition, commandManager);
			return null;
		}
		public static ToolTipPositionModel CreateToolTipPositionModelInstance(ToolTipPosition tooltipPosition, CommandManager commandManager) {
			if(tooltipPosition == null)
				return null;
			if(tooltipPosition is ToolTipMousePosition)
				return new ToolTipMousePositionModel((ToolTipMousePosition)tooltipPosition, commandManager);
			if(tooltipPosition is ToolTipRelativePosition)
				return new ToolTipRelativePositionModel((ToolTipRelativePosition)tooltipPosition, commandManager);
			if(tooltipPosition is ToolTipFreePosition)
				return new ToolTipFreePositionModel((ToolTipFreePosition)tooltipPosition, commandManager);
			return null;
		}
		public static XYDiagramPaneBaseModel CreatePaneModelInstance(XYDiagramPaneBase xyDiagramPaneBase, CommandManager commandManager) {
			if(xyDiagramPaneBase == null)
				return null;
			if(xyDiagramPaneBase is XYDiagramDefaultPane)
				return new XYDiagramDefaultPaneModel((XYDiagramDefaultPane)xyDiagramPaneBase, commandManager);
			if(xyDiagramPaneBase is XYDiagramPane)
				return new XYDiagramPaneModel((XYDiagramPane)xyDiagramPaneBase, commandManager);
			return null;
		}
		public static AxisModelBase CreateAxisModelInstance(AxisBase axis, CommandManager commandManager) {
			if(axis == null)
				return null;
			if(axis is AxisX3D)
				return new AxisX3DModel((AxisX3D)axis, commandManager);
			if(axis is AxisY3D)
				return new AxisY3DModel((AxisY3D)axis, commandManager);
			if(axis is SwiftPlotDiagramAxisX)
				return new SwiftPlotDiagramAxisXModel((SwiftPlotDiagramAxisX)axis, commandManager);
			if(axis is SwiftPlotDiagramAxisY)
				return new SwiftPlotDiagramAxisYModel((SwiftPlotDiagramAxisY)axis, commandManager);
			if(axis is SwiftPlotDiagramSecondaryAxisX)
				return new SwiftPlotDiagramSecondaryAxisXModel((SwiftPlotDiagramSecondaryAxisX)axis, commandManager);
			if(axis is SwiftPlotDiagramSecondaryAxisY)
				return new SwiftPlotDiagramSecondaryAxisYModel((SwiftPlotDiagramSecondaryAxisY)axis, commandManager);
			if(axis is GanttAxisX)
				return new GanttAxisXModel((GanttAxisX)axis, commandManager);
			if(axis is AxisX)
				return new AxisXModel((AxisX)axis, commandManager);
			if(axis is AxisY)
				return new AxisYModel((AxisY)axis, commandManager);
			if(axis is PolarAxisX)
				return new PolarAxisXModel((PolarAxisX)axis, commandManager);
			if(axis is RadarAxisX)
				return new RadarAxisXModel((RadarAxisX)axis, commandManager);
			if(axis is RadarAxisY)
				return new RadarAxisYModel((RadarAxisY)axis, commandManager);
			if(axis is SecondaryAxisX)
				return new SecondaryAxisXModel((SecondaryAxisX)axis, commandManager);
			if(axis is SecondaryAxisY)
				return new SecondaryAxisYModel((SecondaryAxisY)axis, commandManager);
			return null;
		}
		public static RelationModel CreateRelationModel(Relation relation, SeriesPoint point, CommandManager commandManager) {
			if(relation == null)
				return null;
			if(relation is TaskLink)
				return new TaskLinkModel((TaskLink)relation, point, commandManager);
			return new RelationModel(relation, point, commandManager);
		}
	}
}
namespace DevExpress.XtraCharts.Design {
	public static class SeriesGroupsHelper {
		public static SeriesGroupWrappers CreateSeriesGroupWrappers(SeriesBase seriesBase) {
			SeriesGroupWrappers wrappers = new SeriesGroupWrappers();
			if(seriesBase != null) {
				DataContainer container = ((IOwnedElement)seriesBase).Owner as DataContainer;
				if(seriesBase is Series && container != null) {
					foreach(Series series in container.Series) {
						ISupportSeriesGroups view = series.View as ISupportSeriesGroups;
						if(view != null)
							wrappers.AddGroup(view.SeriesGroup);
					}
				} else {
					ISupportSeriesGroups view = seriesBase.View as ISupportSeriesGroups;
					wrappers.AddGroup(view.SeriesGroup);
				}
			}
			return wrappers;
		}
		public static object[] CreateSeriesGroupArray(SeriesBase seriesBase) {
			List<object> groups = new List<object>();
			if(seriesBase != null) {
				DataContainer container = ((IOwnedElement)seriesBase).Owner as DataContainer;
				if(seriesBase is Series && container != null) {
					foreach(Series series in container.Series) {
						ISupportSeriesGroups view = series.View as ISupportSeriesGroups;
						if(view != null && !groups.Contains(view.SeriesGroup) && view.SeriesGroup != null)
							groups.Add(view.SeriesGroup);
					}
				} else {
					ISupportSeriesGroups view = seriesBase.View as ISupportSeriesGroups;
					if(view.SeriesGroup != null)
						groups.Add(view.SeriesGroup);
				}
			}
			return groups.ToArray();
		}
	}
}
