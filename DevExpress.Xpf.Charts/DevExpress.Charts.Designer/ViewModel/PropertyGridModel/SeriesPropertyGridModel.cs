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
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Charts.Designer.Native {
	public class ValueScaleTypeConverter : EnumConverter {
		public ValueScaleTypeConverter(Type type)
			: base(type) {
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			List<ScaleType> valueScaleTypes = new List<ScaleType>();
			valueScaleTypes.Add(ScaleType.Auto);
			valueScaleTypes.Add(ScaleType.DateTime);
			valueScaleTypes.Add(ScaleType.Numerical);
			return new StandardValuesCollection(valueScaleTypes);
		}
	}
	public abstract class WpfChartSeriesPropertyGridModel : PropertyGridModelBase {
		public static WpfChartSeriesPropertyGridModel CreatePropertyGridModelForSeries(WpfChartSeriesModel seriesModel, WpfChartModel chartModel) {
			if (seriesModel.Series is AreaSeries3D)
				return new WpfChartAreaSeries3DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is BubbleSeries3D)
				return new WpfChartBubbleSeries3DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is PointSeries3D)
				return new WpfChartPointSeries3DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is BarSeries3D)
				return new WpfChartBarSeries3DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is PieSeries3D)
				return new WpfChartPieSeries3DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is NestedDonutSeries2D)
				return new WpfChartNestedDonutSeries2DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is PieSeries2D)
				return new WpfChartPieSeries2DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is FunnelSeries2D)
				return new WpfChartFunnelSeries2DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is SplineAreaFullStackedSeries2D)
				return new WpfChartSplineAreaFullStackedSeries2DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is SplineAreaStackedSeries2D)
				return new WpfChartSplineAreaStackedSeries2DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is StockSeries2D)
				return new WpfChartStockSeries2DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is CandleStickSeries2D)
				return new WpfChartCandleStickSeries2DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is CircularLineSeries2D)
				return new WpfChartCircularLineSeries2DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is CircularAreaSeries2D)
				return new WpfChartCircularAreaSeries2DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is CircularSeries2D)
				return new WpfChartCircularSeries2DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is SplineAreaSeries2D)
				return new WpfChartSplineAreaSeries2DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is AreaStackedSeries2D)
				return new WpfChartAreaStackedSeries2DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is AreaStepSeries2D)
				return new WpfChartAreaStepSeries2DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is AreaSeries2D)
				return new WpfChartAreaSeries2DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is LineStepSeries2D)
				return new WpfChartLineStepSeries2DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is SplineSeries2D)
				return new WpfChartSplineSeries2DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is LineSeries2D)
				return new WpfChartLineSeries2DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is RangeAreaSeries2D)
				return new WpfChartRangeAreaSeries2DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is RangeBarSeries2D)
				return new WpfChartRangeBarSeries2DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is BarSideBySideStackedSeries2D)
				return new WpfChartBarSideBySideStackedSeries2DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is BarSideBySideFullStackedSeries2D)
				return new WpfChartBarSideBySideFullStackedSeries2DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is BarSideBySideSeries2D)
				return new WpfChartBarSideBySideSeries2DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is BarSeries2D)
				return new WpfChartBarSeries2DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is BubbleSeries2D)
				return new WpfChartBubbleSeries2DPropertyGridModel(chartModel, seriesModel);
			if (seriesModel.Series is PointSeries2D)
				return new WpfChartPointSeries2DPropertyGridModel(chartModel, seriesModel);
			return null;
		}
		readonly SetSeriesPropertyCommand setSeriesPropertyCommand;
		readonly SeriesPointPropertyGridModelCollection points;
		readonly IndicatorPropertyGridModelCollection indicators;
		WpfChartSeriesModel seriesModel;
		WpfChartSeriesLabelPropertyGridModel seriesLabel = null;
		ChartColorizerBasePropertyGridModel colorizer = null;
		protected Series Series { get { return seriesModel.Series; } }
		protected override ICommand SetObjectPropertyCommand { get { return setSeriesPropertyCommand; } }
		[Category(Categories.Presentation)]
		public string DisplayName {
			get { return Series.DisplayName; }
			set {
				SetProperty(new ChangeSeriesNameCommand(ChartModel), value);
			}
		}
		[Category(Categories.Behavior)]
		public bool LabelsVisibility {
			get { return Series.LabelsVisibility; }
			set {
				SetProperty("LabelsVisibility", value);
			}
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Elements),
		DefaultValue(null)
		]
		public WpfChartSeriesLabelPropertyGridModel Label {
			get { return seriesLabel; }
			set {
				SeriesLabel newValue = value != null ? new SeriesLabel() : null;
				SetProperty("Label", newValue);
			}
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Presentation),
		DefaultValue(null)
		]
		public ChartColorizerBasePropertyGridModel Colorizer {
			get { return colorizer; }
			set {
				if (value != null)
					SetProperty("Colorizer", value.CreateColorizer());
				else
					SetProperty("Colorizer", null);
			}
		}
		[Category(Categories.Common)]
		public string ToolTipSeriesPattern {
			get { return Series.ToolTipSeriesPattern; }
			set {
				SetProperty("ToolTipSeriesPattern", value);
			}
		}
		[Category(Categories.Common)]
		public string ToolTipPointPattern {
			get { return Series.ToolTipPointPattern; }
			set {
				SetProperty("ToolTipPointPattern", value);
			}
		}
		[Category(Categories.Common)]
		public string LegendTextPattern {
			get { return Series.LegendTextPattern; }
			set { SetProperty("LegendTextPattern", value); }
		}
		[Category(Categories.Data)]
		public string ArgumentDataMember {
			get { return Series.ArgumentDataMember; }
			set {
				SetProperty(new SelectArgumentDataMemberCommand(ChartModel), new DataMemberInfo(value, value));
			}
		}
		[Category(Categories.Data)]
		public string ValueDataMember {
			get { return Series.ValueDataMember; }
			set {
				SetProperty(new SelectValueDataMemberCommand(ChartModel), new DataMemberInfo(value, value));
			}
		}
		[Category(Categories.Data)]
		public string ColorDataMember {
			get { return Series.ColorDataMember; }
			set {
				SetProperty(new SelectColorDataMemberCommand(ChartModel), new DataMemberInfo(value, value));
			}
		}
		[Category(Categories.Data)]
		public string ToolTipHintDataMember {
			get { return Series.ToolTipHintDataMember; }
			set {
				SetProperty("ToolTipHintDataMember", new DataMemberInfo(value, value));
			}
		}
		[Category(Categories.Behavior)]
		public ScaleType ArgumentScaleType {
			get { return Series.ArgumentScaleType; }
			set {
				SetProperty("ArgumentScaleType", value);
			}
		}
		[
		TypeConverter(typeof(ValueScaleTypeConverter)),
		Category(Categories.Behavior)
		]
		public ScaleType ValueScaleType {
			get { return Series.ValueScaleType; }
			set {
				SetProperty("ValueScaleType", value);
			}
		}
		[Category(Categories.Behavior)]
		public bool ShowInLegend {
			get { return Series.ShowInLegend; }
			set {
				SetProperty("ShowInLegend", value);
			}
		}
		[Category(Categories.Common)]
		public object ToolTipHint {
			get { return Series.ToolTipHint; }
			set {
				SetProperty("ToolTipHint", value);
			}
		}
		[Category(Categories.Behavior)]
		public bool? ToolTipEnabled {
			get { return Series.ToolTipEnabled; }
			set {
				SetProperty("ToolTipEnabled", value);
			}
		}
		[Category(Categories.Behavior)]
		public bool Visible {
			get { return Series.Visible; }
			set {
				SetProperty("Visible", value);
			}
		}
		[Category(Categories.Elements)]
		public SeriesPointPropertyGridModelCollection Points {
			get { return points; }
		}
		[Category(Categories.Elements)]
		public IndicatorPropertyGridModelCollection Indicators {
			get { return indicators; }
		}
		public WpfChartSeriesPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel) {
			this.seriesModel = seriesModel;
			setSeriesPropertyCommand = new SetSeriesPropertyCommand(ChartModel);
			points = new SeriesPointPropertyGridModelCollection(ChartModel, seriesModel);
			indicators = new IndicatorPropertyGridModelCollection(ChartModel, seriesModel);
			seriesModel.SeriesPointCollectionModel.CollectionUpdated += SeriesPointCollectionUpdated;
			if (seriesModel.IndicatorCollectionModel != null)
				seriesModel.IndicatorCollectionModel.CollectionUpdated += IndicatorCollectionUpdated;
			UpdateInternal();
		}
		void SeriesPointCollectionUpdated(ChartCollectionUpdateEventArgs args) {
			foreach (InsertedItem item in args.AddedItems)
				if (item.Index < points.Count)
					points[item.Index].PointModel = (WpfChartSeriesPointModel)item.Item;
				else
					points.Insert(item.Index, (WpfChartSeriesPointPropertyGridModel)item.Item.PropertyGridModel);
			List<WpfChartSeriesPointPropertyGridModel> removedPointModels = new List<WpfChartSeriesPointPropertyGridModel>();
			foreach (WpfChartSeriesPointModel point in args.RemovedItems)
				foreach (WpfChartSeriesPointPropertyGridModel pointModel in points)
					if (pointModel.PointModel == point)
						removedPointModels.Add(pointModel);
			foreach (WpfChartSeriesPointPropertyGridModel removedPoint in removedPointModels)
				points.Remove(removedPoint);
		}
		void IndicatorCollectionUpdated(ChartCollectionUpdateEventArgs args) {
			foreach (InsertedItem item in args.AddedItems)
				if (item.Index < indicators.Count)
					indicators[item.Index].UpdateModelElement((WpfChartIndicatorModel)item.Item);
				else
					indicators.Insert(item.Index, (WpfChartIndicatorPropertyGridModel)item.Item.PropertyGridModel);
			List<WpfChartIndicatorPropertyGridModel> removedIndicatorModels = new List<WpfChartIndicatorPropertyGridModel>();
			foreach (WpfChartIndicatorModel indicator in args.RemovedItems)
				foreach (WpfChartIndicatorPropertyGridModel indicatorModel in indicators)
					if (indicatorModel.IndicatorModel == indicator)
						removedIndicatorModels.Add(indicatorModel);
			foreach (WpfChartIndicatorPropertyGridModel removedIndicator in removedIndicatorModels)
				indicators.Remove(removedIndicator);
		}
		protected virtual WpfChartSeriesLabelPropertyGridModel CreateSeriesLabelModel() {
			return new WpfChartSeriesLabelPropertyGridModel(ChartModel, Series.Label);
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (Series.Label != null) {
				if (seriesLabel != null && Series.Label != seriesLabel.Label || seriesLabel == null)
					seriesLabel = CreateSeriesLabelModel();
			}
			else
				seriesLabel = null;
			if (Series.Colorizer != null) {
				if (colorizer == null || Series.Colorizer != colorizer.Colorizer)
					colorizer = ChartColorizerBasePropertyGridModel.CreatePropertyGridModel(ChartModel, Series.Colorizer, "Colorizer.");
			}
			else
				colorizer = null;
		}
	}
	public abstract class WpfChartXYSeriesPropertyGridModel : WpfChartSeriesPropertyGridModel {
		new XYSeries Series { get { return base.Series as XYSeries; } }
		[Category(Categories.Presentation)]
		public bool ColorEach {
			get { return Series.ColorEach; }
			set {
				SetProperty("ColorEach", value);
			}
		}
		[Category(Categories.Presentation)]
		public SolidColorBrush Brush {
			get { return Series.Brush; }
			set {
				SetProperty("Brush", value);
			}
		}
		public WpfChartXYSeriesPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
		}
	}
	public abstract class WpfChartXYSeries2DPropertyGridModel : WpfChartXYSeriesPropertyGridModel {
		new XYSeries2D Series { get { return base.Series as XYSeries2D; } }
		[Category(Categories.Behavior)]
		public bool? CrosshairEnabled {
			get { return Series.CrosshairEnabled; }
			set {
				SetProperty("CrosshairEnabled", value);
			}
		}
		[Category(Categories.Behavior)]
		public bool? CrosshairLabelVisibility {
			get { return Series.CrosshairLabelVisibility; }
			set {
				SetProperty("CrosshairLabelVisibility", value);
			}
		}
		[Category(Categories.Behavior)]
		public string CrosshairLabelPattern {
			get { return Series.CrosshairLabelPattern; }
			set {
				SetProperty("CrosshairLabelPattern", value);
			}
		}
		public WpfChartXYSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
		}
	}
	public abstract class WpfChartMarkerSeries2DPropertyGridModel : WpfChartXYSeries2DPropertyGridModel {
		public WpfChartMarkerSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
		}
		protected override WpfChartSeriesLabelPropertyGridModel CreateSeriesLabelModel() {
			return new WpfChartSeriesLabelMarkerSeries2DPropertyGridModel(ChartModel, Series.Label);
		}
	}
	public class WpfChartPointSeries2DPropertyGridModel : WpfChartMarkerSeries2DPropertyGridModel {
		new PointSeries2D Series { get { return base.Series as PointSeries2D; } }
		[Category(Categories.Presentation)]
		public int MarkerSize {
			get { return Series.MarkerSize; }
			set {
				SetProperty("MarkerSize", value);
			}
		}
		[Category(Categories.Presentation)]
		public Marker2DModel MarkerModel {
			get { return Series.MarkerModel; }
			set {
				SetProperty(new Marker2DModelCommand(ChartModel, new Marker2DKind(value.GetType(), "")), true);
			}
		}
		[Category(Categories.Animation)]
		public Marker2DAnimationBase PointAnimation {
			get { return Series.PointAnimation; }
			set {
				SetProperty("PointAnimation", value);
			}
		}
		public WpfChartPointSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
		}
	}
	public class WpfChartBubbleSeries2DPropertyGridModel : WpfChartMarkerSeries2DPropertyGridModel {
		new BubbleSeries2D Series { get { return base.Series as BubbleSeries2D; } }
		[Category(Categories.Data)]
		public string WeightDataMember {
			get { return Series.WeightDataMember; }
			set {
				SetProperty(new SelectWeightDataMemberCommand(ChartModel), new DataMemberInfo(value, value));
			}
		}
		[Category(Categories.Behavior)]
		public double MinSize {
			get { return Series.MinSize; }
			set {
				SetProperty("MinSize", value);
			}
		}
		[Category(Categories.Behavior)]
		public double MaxSize {
			get { return Series.MaxSize; }
			set {
				SetProperty("MaxSize", value);
			}
		}
		[Category(Categories.Presentation)]
		public Marker2DModel MarkerModel {
			get { return Series.MarkerModel; }
			set {
				SetProperty("MarkerModel", value);
			}
		}
		[Category(Categories.Presentation)]
		public double Transparency {
			get { return Series.Transparency; }
			set {
				SetProperty("Transparency", value);
			}
		}
		[Category(Categories.Animation)]
		public Marker2DAnimationBase PointAnimation {
			get { return Series.PointAnimation; }
			set {
				SetProperty("PointAnimation", value);
			}
		}
		public WpfChartBubbleSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
		}
		protected override WpfChartSeriesLabelPropertyGridModel CreateSeriesLabelModel() {
			return new WpfChartSeriesLabelBubbleSeries2DPropertyGridModel(ChartModel, Series.Label);
		}
	}
	public abstract class WpfChartBarSeries2DBasePropertyGridModel : WpfChartXYSeries2DPropertyGridModel {
		new BarSeries2DBase Series { get { return base.Series as BarSeries2DBase; } }
		[Category(Categories.Behavior)]
		public double BarWidth {
			get { return Series.BarWidth; }
			set {
				SetProperty("BarWidth", value);
			}
		}
		[Category(Categories.Animation)]
		public Bar2DAnimationBase PointAnimation {
			get { return Series.PointAnimation; }
			set {
				SetProperty("PointAnimation", value);
			}
		}
		public WpfChartBarSeries2DBasePropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
		}
	}
	public class WpfChartBarSeries2DPropertyGridModel : WpfChartBarSeries2DBasePropertyGridModel {
		new BarSeries2D Series { get { return base.Series as BarSeries2D; } }
		[Category(Categories.Presentation)]
		public Bar2DModel Model {
			get { return Series.Model; }
			set {
				SetProperty("Model", value);
			}
		}
		public WpfChartBarSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
		}
	}
	public class WpfChartBarSideBySideSeries2DPropertyGridModel : WpfChartBarSeries2DPropertyGridModel {
		public WpfChartBarSideBySideSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
		}
		protected override WpfChartSeriesLabelPropertyGridModel CreateSeriesLabelModel() {
			return new WpfChartSeriesLabelBarSideBySideSeries2DPropertyGridModel(ChartModel, Series.Label);
		}
	}
	public class WpfChartBarSideBySideFullStackedSeries2DPropertyGridModel : WpfChartBarSeries2DPropertyGridModel {
		new BarSideBySideFullStackedSeries2D Series { get { return base.Series as BarSideBySideFullStackedSeries2D; } }
		[Category(Categories.Behavior)]
		public object StackedGroup {
			get { return Series.StackedGroup; }
			set {
				SetProperty("StackedGroup", value);
			}
		}
		public WpfChartBarSideBySideFullStackedSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
		}
	}
	public class WpfChartBarSideBySideStackedSeries2DPropertyGridModel : WpfChartBarSeries2DPropertyGridModel {
		new BarSideBySideStackedSeries2D Series { get { return base.Series as BarSideBySideStackedSeries2D; } }
		[Category(Categories.Behavior)]
		public object StackedGroup {
			get { return Series.StackedGroup; }
			set {
				SetProperty("StackedGroup", value);
			}
		}
		public WpfChartBarSideBySideStackedSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
		}
	}
	public class WpfChartRangeBarSeries2DPropertyGridModel : WpfChartBarSeries2DBasePropertyGridModel {
		new RangeBarSeries2D Series { get { return base.Series as RangeBarSeries2D; } }
		[Category(Categories.Data)]
		public string Value2DataMember {
			get { return Series.Value2DataMember; }
			set {
				SetProperty("Value2DataMember", new DataMemberInfo(value, value));
			}
		}
		[Category(Categories.Presentation)]
		public RangeBar2DModel Model {
			get { return Series.Model; }
			set {
				SetProperty("Model", value);
			}
		}
		[Category(Categories.Presentation)]
		public Marker2DModel MinMarkerModel {
			get { return Series.MinMarkerModel; }
			set {
				SetProperty("MinMarkerModel", value);
			}
		}
		[Category(Categories.Presentation)]
		public int MinMarkerSize {
			get { return Series.MinMarkerSize; }
			set {
				SetProperty("MinMarkerSize", value);
			}
		}
		[Category(Categories.Behavior)]
		public bool MinMarkerVisible {
			get { return Series.MinMarkerVisible; }
			set {
				SetProperty("MinMarkerVisible", value);
			}
		}
		[Category(Categories.Presentation)]
		public Marker2DModel MaxMarkerModel {
			get { return Series.MaxMarkerModel; }
			set {
				SetProperty("MaxMarkerModel", value);
			}
		}
		[Category(Categories.Presentation)]
		public int MaxMarkerSize {
			get { return Series.MaxMarkerSize; }
			set {
				SetProperty("MaxMarkerSize", value);
			}
		}
		[Category(Categories.Behavior)]
		public bool MaxMarkerVisible {
			get { return Series.MaxMarkerVisible; }
			set {
				SetProperty("MaxMarkerVisible", value);
			}
		}
		[Category(Categories.Data)]
		public string LabelValueSeparator {
			get { return Series.LabelValueSeparator; }
			set {
				SetProperty("LabelValueSeparator", value);
			}
		}
		[Category(Categories.Data)]
		public string LegendValueSeparator {
			get { return Series.LegendValueSeparator; }
			set {
				SetProperty("LegendValueSeparator", value);
			}
		}
		public WpfChartRangeBarSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
		}
		protected override WpfChartSeriesLabelPropertyGridModel CreateSeriesLabelModel() {
			return new WpfChartSeriesLabelRangeBarSeries2DPropertyGridModel(ChartModel, Series.Label);
		}
	}
	public class WpfChartRangeAreaSeries2DPropertyGridModel : WpfChartXYSeries2DPropertyGridModel {
		WpfChartSeriesBorderPropertyGridModel border1;
		WpfChartSeriesBorderPropertyGridModel border2;
		new RangeAreaSeries2D Series { get { return base.Series as RangeAreaSeries2D; } }
		[Category(Categories.Data)]
		public string Value2DataMember {
			get { return Series.Value2DataMember; }
			set {
				SetProperty("Value2DataMember", new DataMemberInfo(value, value));
			}
		}
		[Category(Categories.Presentation)]
		public Marker2DModel Marker1Model {
			get { return Series.Marker1Model; }
			set {
				SetProperty("Marker1Model", value);
			}
		}
		[Category(Categories.Presentation)]
		public int Marker1Size {
			get { return Series.Marker1Size; }
			set {
				SetProperty("Marker1Size", value);
			}
		}
		[Category(Categories.Behavior)]
		public bool Marker1Visible {
			get { return Series.Marker1Visible; }
			set {
				SetProperty("Marker1Visible", value);
			}
		}
		[Category(Categories.Presentation)]
		public Marker2DModel Marker2Model {
			get { return Series.Marker2Model; }
			set {
				SetProperty("Marker2Model", value);
			}
		}
		[Category(Categories.Presentation)]
		public int Marker2Size {
			get { return Series.Marker2Size; }
			set {
				SetProperty("Marker2Size", value);
			}
		}
		[Category(Categories.Behavior)]
		public bool Marker2Visible {
			get { return Series.Marker2Visible; }
			set {
				SetProperty("Marker2Visible", value);
			}
		}
		[Category(Categories.Animation)]
		public Marker2DAnimationBase PointAnimation {
			get { return Series.PointAnimation; }
			set {
				SetProperty("PointAnimation", value);
			}
		}
		[Category(Categories.Animation)]
		public Area2DAnimationBase SeriesAnimation {
			get { return Series.SeriesAnimation; }
			set {
				SetProperty("SeriesAnimation", value);
			}
		}
		[Category(Categories.Presentation)]
		public double Transparency {
			get { return Series.Transparency; }
			set {
				SetProperty("Transparency", value);
			}
		}
		[Category(Categories.Data)]
		public string LabelValueSeparator {
			get { return Series.LabelValueSeparator; }
			set {
				SetProperty("LabelValueSeparator", value);
			}
		}
		[Category(Categories.Data)]
		public string LegendValueSeparator {
			get { return Series.LegendValueSeparator; }
			set {
				SetProperty("LegendValueSeparator", value);
			}
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Presentation)
		]
		public WpfChartSeriesBorderPropertyGridModel Border1 {
			get { return border1; }
			set {
				SetProperty("Border1", new SeriesBorder());
			}
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Presentation)
		]
		public WpfChartSeriesBorderPropertyGridModel Border2 {
			get { return border2; }
			set {
				SetProperty("Border2", new SeriesBorder());
			}
		}
		public WpfChartRangeAreaSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
			UpdateInternal();
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (Series.Border1 != null) {
				if (border1 != null && Series.Border1 != border1.Border || border1 == null)
					border1 = new WpfChartSeriesBorderPropertyGridModel(ChartModel, Series.Border1, "Border1.");
			}
			else
				border1 = null;
			if (Series.Border2 != null) {
				if (border2 != null && Series.Border2 != border2.Border || border2 == null)
					border2 = new WpfChartSeriesBorderPropertyGridModel(ChartModel, Series.Border2, "Border2.");
			}
			else
				border2 = null;
		}
		protected override WpfChartSeriesLabelPropertyGridModel CreateSeriesLabelModel() {
			return new WpfChartSeriesLabelRangeAreaSeries2DPropertyGridModel(ChartModel, Series.Label);
		}
	}
	public class WpfChartLineSeries2DPropertyGridModel : WpfChartMarkerSeries2DPropertyGridModel {
		WpfChartLineStylePropertyGridModel lineStyle;
		new LineSeries2D Series { get { return base.Series as LineSeries2D; } }
		[Category(Categories.Presentation)]
		public Marker2DModel MarkerModel {
			get { return Series.MarkerModel; }
			set {
				SetProperty("MarkerModel", value);
			}
		}
		[Category(Categories.Presentation)]
		public int MarkerSize {
			get { return Series.MarkerSize; }
			set {
				SetProperty("MarkerSize", value);
			}
		}
		[Category(Categories.Presentation)]
		public bool MarkerVisible {
			get { return Series.MarkerVisible; }
			set {
				SetProperty("MarkerVisible", value);
			}
		}
		[Category(Categories.Animation)]
		public Marker2DAnimationBase PointAnimation {
			get { return Series.PointAnimation; }
			set {
				SetProperty("PointAnimation", value);
			}
		}
		[Category(Categories.Animation)]
		public Line2DAnimationBase SeriesAnimation {
			get { return Series.SeriesAnimation; }
			set {
				SetProperty("SeriesAnimation", value);
			}
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Presentation)
		]
		public WpfChartLineStylePropertyGridModel LineStyle {
			get { return lineStyle; }
			set {
				SetProperty("LineStyle", new LineStyle());
			}
		}
		public WpfChartLineSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
			UpdateInternal();
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (Series.LineStyle != null) {
				if (lineStyle != null && Series.LineStyle != lineStyle.LineStyle || lineStyle == null)
					lineStyle = new WpfChartLineStylePropertyGridModel(ChartModel, Series.LineStyle, SetObjectPropertyCommand, "LineStyle.");
			}
			else
				lineStyle = null;
		}
	}
	public class WpfChartSplineSeries2DPropertyGridModel : WpfChartLineSeries2DPropertyGridModel {
		new SplineSeries2D Series { get { return base.Series as SplineSeries2D; } }
		[Category(Categories.Presentation)]
		public double LineTension {
			get { return Series.LineTension; }
			set {
				SetProperty("LineTension", value);
			}
		}
		public WpfChartSplineSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
			UpdateInternal();
		}
	}
	public class WpfChartLineStepSeries2DPropertyGridModel : WpfChartLineSeries2DPropertyGridModel {
		new LineStepSeries2D Series { get { return base.Series as LineStepSeries2D; } }
		[Category(Categories.Behavior)]
		public bool InvertedStep {
			get { return Series.InvertedStep; }
			set {
				SetProperty("InvertedStep", value);
			}
		}
		public WpfChartLineStepSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
		}
	}
	public class WpfChartAreaSeries2DPropertyGridModel : WpfChartMarkerSeries2DPropertyGridModel {
		WpfChartSeriesBorderPropertyGridModel border;
		new AreaSeries2D Series { get { return base.Series as AreaSeries2D; } }
		[Category(Categories.Presentation)]
		public Marker2DModel MarkerModel {
			get { return Series.MarkerModel; }
			set {
				SetProperty("MarkerModel", value);
			}
		}
		[Category(Categories.Presentation)]
		public int MarkerSize {
			get { return Series.MarkerSize; }
			set {
				SetProperty("MarkerSize", value);
			}
		}
		[Category(Categories.Behavior)]
		public bool MarkerVisible {
			get { return Series.MarkerVisible; }
			set {
				SetProperty("MarkerVisible", value);
			}
		}
		[Category(Categories.Presentation)]
		public double Transparency {
			get { return Series.Transparency; }
			set {
				SetProperty("Transparency", value);
			}
		}
		[Category(Categories.Animation)]
		public Marker2DAnimationBase PointAnimation {
			get { return Series.PointAnimation; }
			set {
				SetProperty("PointAnimation", value);
			}
		}
		[Category(Categories.Animation)]
		public Area2DAnimationBase SeriesAnimation {
			get { return Series.SeriesAnimation; }
			set {
				SetProperty("SeriesAnimation", value);
			}
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Presentation)
		]
		public WpfChartSeriesBorderPropertyGridModel Border {
			get { return border; }
			set {
				SetProperty("Border", new SeriesBorder());
			}
		}
		public WpfChartAreaSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
			UpdateInternal();
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (Series.Border != null) {
				if (border != null && Series.Border != border.Border || border == null)
					border = new WpfChartSeriesBorderPropertyGridModel(ChartModel, Series.Border, "Border.");
			}
			else
				border = null;
		}
	}
	public class WpfChartAreaStepSeries2DPropertyGridModel : WpfChartAreaSeries2DPropertyGridModel {
		new AreaStepSeries2D Series { get { return base.Series as AreaStepSeries2D; } }
		[Category(Categories.Behavior)]
		public bool InvertedStep {
			get { return Series.InvertedStep; }
			set {
				SetProperty("InvertedStep", value);
			}
		}
		public WpfChartAreaStepSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
		}
	}
	public class WpfChartAreaStackedSeries2DPropertyGridModel : WpfChartMarkerSeries2DPropertyGridModel {
		WpfChartSeriesBorderPropertyGridModel border;
		new AreaStackedSeries2D Series { get { return base.Series as AreaStackedSeries2D; } }
		[Category(Categories.Presentation)]
		public double Transparency {
			get { return Series.Transparency; }
			set {
				SetProperty("Transparency", value);
			}
		}
		[Category(Categories.Animation)]
		public AreaStacked2DFadeInAnimation PointAnimation {
			get { return Series.PointAnimation; }
			set {
				SetProperty("PointAnimation", value);
			}
		}
		[Category(Categories.Animation)]
		public Area2DAnimationBase SeriesAnimation {
			get { return Series.SeriesAnimation; }
			set {
				SetProperty("SeriesAnimation", value);
			}
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Presentation)
		]
		public WpfChartSeriesBorderPropertyGridModel Border {
			get { return border; }
			set {
				SetProperty("Border", new SeriesBorder());
			}
		}
		public WpfChartAreaStackedSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
			UpdateInternal();
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (Series.Border != null) {
				if (border != null && Series.Border != border.Border || border == null)
					border = new WpfChartSeriesBorderPropertyGridModel(ChartModel, Series.Border, "Border.");
			}
			else
				border = null;
		}
	}
	public class WpfChartSplineAreaStackedSeries2DPropertyGridModel : WpfChartAreaStackedSeries2DPropertyGridModel {
		new SplineAreaStackedSeries2D Series { get { return base.Series as SplineAreaStackedSeries2D; } }
		[Category(Categories.Presentation)]
		public double LineTension {
			get { return Series.LineTension; }
			set {
				SetProperty("LineTension", value);
			}
		}
		public WpfChartSplineAreaStackedSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
			UpdateInternal();
		}
	}
	public class WpfChartSplineAreaFullStackedSeries2DPropertyGridModel : WpfChartAreaStackedSeries2DPropertyGridModel {
		new SplineAreaFullStackedSeries2D Series { get { return base.Series as SplineAreaFullStackedSeries2D; } }
		[Category(Categories.Presentation)]
		public double LineTension {
			get { return Series.LineTension; }
			set {
				SetProperty("LineTension", value);
			}
		}
		public WpfChartSplineAreaFullStackedSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
			UpdateInternal();
		}
	}
	public class WpfChartSplineAreaSeries2DPropertyGridModel : WpfChartAreaSeries2DPropertyGridModel {
		new SplineAreaSeries2D Series { get { return base.Series as SplineAreaSeries2D; } }
		[Category(Categories.Presentation)]
		public double LineTension {
			get { return Series.LineTension; }
			set {
				SetProperty("LineTension", value);
			}
		}
		public WpfChartSplineAreaSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
			UpdateInternal();
		}
	}
	public class WpfChartCircularSeries2DPropertyGridModel : WpfChartXYSeriesPropertyGridModel {
		new CircularSeries2D Series { get { return base.Series as CircularSeries2D; } }
		[Category(Categories.Presentation)]
		public Marker2DModel MarkerModel {
			get { return Series.MarkerModel; }
			set {
				SetProperty("MarkerModel", value);
			}
		}
		[Category(Categories.Presentation)]
		public int MarkerSize {
			get { return Series.MarkerSize; }
			set {
				SetProperty("MarkerSize", value);
			}
		}
		[Category(Categories.Animation)]
		public CircularMarkerAnimationBase PointAnimation {
			get { return Series.PointAnimation; }
			set {
				SetProperty("PointAnimation", value);
			}
		}
		public WpfChartCircularSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
		}
		protected override WpfChartSeriesLabelPropertyGridModel CreateSeriesLabelModel() {
			return new WpfChartSeriesLabelCircularSeries2DPropertyGridModel(ChartModel, Series.Label);
		}
	}
	public class WpfChartCircularAreaSeries2DPropertyGridModel : WpfChartCircularSeries2DPropertyGridModel {
		WpfChartSeriesBorderPropertyGridModel border;
		new CircularAreaSeries2D Series { get { return base.Series as CircularAreaSeries2D; } }
		[Category(Categories.Presentation)]
		public bool MarkerVisible {
			get { return Series.MarkerVisible; }
			set {
				SetProperty("MarkerVisible", value);
			}
		}
		[Category(Categories.Presentation)]
		public double Transparency {
			get { return Series.Transparency; }
			set {
				SetProperty("Transparency", value);
			}
		}
		[Category(Categories.Animation)]
		public CircularAreaAnimationBase SeriesAnimation {
			get { return Series.SeriesAnimation; }
			set {
				SetProperty("SeriesAnimation", value);
			}
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Presentation)
		]
		public WpfChartSeriesBorderPropertyGridModel Border {
			get { return border; }
			set {
				SetProperty("Border", new SeriesBorder());
			}
		}
		public WpfChartCircularAreaSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
			UpdateInternal();
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (Series.Border != null) {
				if (border != null && Series.Border != border.Border || border == null)
					border = new WpfChartSeriesBorderPropertyGridModel(ChartModel, Series.Border, "Border.");
			}
			else
				border = null;
		}
	}
	public class WpfChartCircularLineSeries2DPropertyGridModel : WpfChartCircularSeries2DPropertyGridModel {
		WpfChartLineStylePropertyGridModel lineStyle;
		new CircularLineSeries2D Series { get { return base.Series as CircularLineSeries2D; } }
		[Category(Categories.Presentation)]
		public bool Closed {
			get { return Series.Closed; }
			set {
				SetProperty("Closed", value);
			}
		}
		[Category(Categories.Presentation)]
		public bool MarkerVisible {
			get { return Series.MarkerVisible; }
			set {
				SetProperty("MarkerVisible", value);
			}
		}
		[Category(Categories.Animation)]
		public CircularLineAnimationBase SeriesAnimation {
			get { return Series.SeriesAnimation; }
			set {
				SetProperty("SeriesAnimation", value);
			}
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Presentation)
		]
		public WpfChartLineStylePropertyGridModel LineStyle {
			get { return lineStyle; }
			set {
				SetProperty("LineStyle", new LineStyle());
			}
		}
		public WpfChartCircularLineSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
			UpdateInternal();
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (Series.LineStyle != null) {
				if (lineStyle != null && Series.LineStyle != lineStyle.LineStyle || lineStyle == null)
					lineStyle = new WpfChartLineStylePropertyGridModel(ChartModel, Series.LineStyle, SetObjectPropertyCommand, "LineStyle.");
			}
			else
				lineStyle = null;
		}
	}
	public abstract class WpfChartFinancialSeries2DPropertyGridModel : WpfChartXYSeries2DPropertyGridModel {
		WpfChartReductionStockOptionsPropertyGridModel reductionOptions;
		new FinancialSeries2D Series { get { return base.Series as FinancialSeries2D; } }
		[Browsable(false)]
		public new string ValueDataMember {
			get { return String.Empty; }
			set { ; }
		}
		[Category(Categories.Data)]
		public string LowValueDataMember {
			get { return Series.LowValueDataMember; }
			set {
				SetProperty(new SelectLowValueDataMemberCommand(ChartModel), new DataMemberInfo(value, value));
			}
		}
		[Category(Categories.Data)]
		public string HighValueDataMember {
			get { return Series.HighValueDataMember; }
			set {
				SetProperty(new SelectHighValueDataMemberCommand(ChartModel), new DataMemberInfo(value, value));
			}
		}
		[Category(Categories.Data)]
		public string OpenValueDataMember {
			get { return Series.OpenValueDataMember; }
			set {
				SetProperty(new SelectOpenValueDataMemberCommand(ChartModel), new DataMemberInfo(value, value));
			}
		}
		[Category(Categories.Data)]
		public string CloseValueDataMember {
			get { return Series.CloseValueDataMember; }
			set {
				SetProperty(new SelectCloseValueDataMemberCommand(ChartModel), new DataMemberInfo(value, value));
			}
		}
		[Category(Categories.Animation)]
		public Stock2DAnimationBase PointAnimation {
			get { return Series.PointAnimation; }
			set {
				SetProperty("PointAnimation", value);
			}
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior)
		]
		public WpfChartReductionStockOptionsPropertyGridModel ReductionOptions {
			get { return reductionOptions; }
			set {
				SetProperty("ReductionOptions", new ReductionStockOptions());
			}
		}
		public WpfChartFinancialSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
			UpdateInternal();
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (Series.ReductionOptions != null) {
				if (reductionOptions != null && Series.ReductionOptions != reductionOptions.ReductionOptions || reductionOptions == null)
					reductionOptions = new WpfChartReductionStockOptionsPropertyGridModel(ChartModel, Series.ReductionOptions);
			}
			else
				reductionOptions = null;
		}
	}
	public class WpfChartCandleStickSeries2DPropertyGridModel : WpfChartFinancialSeries2DPropertyGridModel {
		new CandleStickSeries2D Series { get { return base.Series as CandleStickSeries2D; } }
		[Category(Categories.Presentation)]
		public double CandleWidth {
			get { return Series.CandleWidth; }
			set {
				SetProperty("CandleWidth", value);
			}
		}
		[Category(Categories.Presentation)]
		public CandleStick2DModel Model {
			get { return Series.Model; }
			set {
				SetProperty("Model", value);
			}
		}
		public WpfChartCandleStickSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
		}
	}
	public class WpfChartStockSeries2DPropertyGridModel : WpfChartFinancialSeries2DPropertyGridModel {
		new StockSeries2D Series { get { return base.Series as StockSeries2D; } }
		[Category(Categories.Presentation)]
		public double LevelLineLength {
			get { return Series.LevelLineLength; }
			set {
				SetProperty("LevelLineLength", value);
			}
		}
		[Category(Categories.Behavior)]
		public StockType ShowOpenClose {
			get { return Series.ShowOpenClose; }
			set {
				SetProperty("ShowOpenClose", value);
			}
		}
		[Category(Categories.Presentation)]
		public Stock2DModel Model {
			get { return Series.Model; }
			set {
				SetProperty("Model", value);
			}
		}
		public WpfChartStockSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
		}
	}
	public abstract class WpfChartPieSeriesPropertyGridModel : WpfChartSeriesPropertyGridModel {
		new PieSeries Series { get { return base.Series as PieSeries; } }
		[Category(Categories.Behavior)]
		public double HoleRadiusPercent {
			get { return Series.HoleRadiusPercent; }
			set {
				SetProperty("HoleRadiusPercent", value);
			}
		}
		[Category(Categories.Behavior)]
		public int LabelsResolveOverlappingMinIndent {
			get { return Series.LabelsResolveOverlappingMinIndent; }
			set {
				SetProperty("LabelsResolveOverlappingMinIndent", value);
			}
		}
		public WpfChartPieSeriesPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
		}
		protected override WpfChartSeriesLabelPropertyGridModel CreateSeriesLabelModel() {
			return new WpfChartSeriesLabelPieSeriesPropertyGridModel(ChartModel, Series.Label);
		}
	}
	public class WpfChartPieSeries2DPropertyGridModel : WpfChartPieSeriesPropertyGridModel {
		new PieSeries2D Series { get { return base.Series as PieSeries2D; } }
		[Category(Categories.Behavior)]
		public double Rotation {
			get { return Series.Rotation; }
			set {
				SetProperty("Rotation", value);
			}
		}
		[Category(Categories.Behavior)]
		public PieSweepDirection SweepDirection {
			get { return Series.SweepDirection; }
			set {
				SetProperty("SweepDirection", value);
			}
		}
		[Category(Categories.Presentation)]
		public Pie2DModel Model {
			get { return Series.Model; }
			set {
				SetProperty("Model", value);
			}
		}
		[Category(Categories.Animation)]
		public Pie2DSeriesPointAnimationBase PointAnimation {
			get { return Series.PointAnimation; }
			set {
				SetProperty("PointAnimation", value);
			}
		}
		[Category(Categories.Animation)]
		public Pie2DSeriesAnimationBase SeriesAnimation {
			get { return Series.SeriesAnimation; }
			set {
				SetProperty("SeriesAnimation", value);
			}
		}
		public WpfChartPieSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
		}
	}
	public class WpfChartNestedDonutSeries2DPropertyGridModel : WpfChartPieSeries2DPropertyGridModel {
		new NestedDonutSeries2D Series { get { return base.Series as NestedDonutSeries2D; } }
		[Category(Categories.Behavior)]
		public object Group {
			get { return Series.Group; }
			set { SetProperty("Group", value); }
		}
		[Category(Categories.Behavior)]
		public double InnerIndent {
			get { return Series.InnerIndent; }
			set { SetProperty("InnerIndent", value); }
		}
		[Category(Categories.Data)]
		public double Weight {
			get { return Series.Weight; }
			set { SetProperty("Weight", value); }
		}
		public WpfChartNestedDonutSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) { }
	}
	public class WpfChartPieSeries3DPropertyGridModel : WpfChartPieSeriesPropertyGridModel {
		new PieSeries3D Series { get { return base.Series as PieSeries3D; } }
		[Category(Categories.Presentation)]
		public Pie3DModel Model {
			get { return Series.Model; }
			set {
				SetProperty("Model", value);
			}
		}
		public WpfChartPieSeries3DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
		}
	}
	public class WpfChartBarSeries3DPropertyGridModel : WpfChartXYSeriesPropertyGridModel {
		new BarSeries3D Series { get { return base.Series as BarSeries3D; } }
		[Category(Categories.Presentation)]
		public double BarWidth {
			get { return Series.BarWidth; }
			set {
				SetProperty("BarWidth", value);
			}
		}
		[Category(Categories.Presentation)]
		public Bar3DModel Model {
			get { return Series.Model; }
			set {
				SetProperty("Model", value);
			}
		}
		public WpfChartBarSeries3DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
		}
	}
	public abstract class WpfChartMarkerSeries3DPropertyGridModel : WpfChartXYSeriesPropertyGridModel {
		new MarkerSeries3D Series { get { return base.Series as MarkerSeries3D; } }
		[Category(Categories.Presentation)]
		public Marker3DModel Model {
			get { return Series.Model; }
			set {
				SetProperty("Model", value);
			}
		}
		public WpfChartMarkerSeries3DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
		}
		protected override WpfChartSeriesLabelPropertyGridModel CreateSeriesLabelModel() {
			return new WpfChartSeriesLabelMarkerSeries3DPropertyGridModel(ChartModel, Series.Label);
		}
	}
	public class WpfChartPointSeries3DPropertyGridModel : WpfChartMarkerSeries3DPropertyGridModel {
		new PointSeries3D Series { get { return base.Series as PointSeries3D; } }
		[Category(Categories.Presentation)]
		public double MarkerSize {
			get { return Series.MarkerSize; }
			set {
				SetProperty("MarkerSize", value);
			}
		}
		public WpfChartPointSeries3DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
		}
	}
	public class WpfChartBubbleSeries3DPropertyGridModel : WpfChartMarkerSeries3DPropertyGridModel {
		new BubbleSeries3D Series { get { return base.Series as BubbleSeries3D; } }
		[Category(Categories.Behavior)]
		public double MaxSize {
			get { return Series.MaxSize; }
			set {
				SetProperty("MaxSize", value);
			}
		}
		[Category(Categories.Behavior)]
		public double MinSize {
			get { return Series.MinSize; }
			set {
				SetProperty("MinSize", value);
			}
		}
		[Category(Categories.Data)]
		public string WeightDataMember {
			get { return Series.WeightDataMember; }
			set {
				SetProperty(new SelectWeightDataMemberCommand(ChartModel), new DataMemberInfo(value, value));
			}
		}
		public WpfChartBubbleSeries3DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
		}
	}
	public class WpfChartAreaSeries3DPropertyGridModel : WpfChartXYSeriesPropertyGridModel {
		new AreaSeries3D Series { get { return base.Series as AreaSeries3D; } }
		[Category(Categories.Presentation)]
		public double AreaWidth {
			get { return Series.AreaWidth; }
			set {
				SetProperty("AreaWidth", value);
			}
		}
		public WpfChartAreaSeries3DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
		}
	}
	public class WpfChartSeriesBorderPropertyGridModel : PropertyGridModelBase {
		readonly SetSeriesPropertyCommand setSeriesPropertyCommand;
		readonly SeriesBorder border;
		readonly string propertyPath;
		WpfChartLineStylePropertyGridModel lineStyle;
		protected internal SeriesBorder Border { get { return border; } }
		protected override ICommand SetObjectPropertyCommand { get { return setSeriesPropertyCommand; } }
		[Category(Categories.Common)]
		public SolidColorBrush Brush {
			get { return border.Brush; }
			set {
				SetProperty(propertyPath + "Brush", value);
			}
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Common)
		]
		public WpfChartLineStylePropertyGridModel LineStyle {
			get { return lineStyle; }
			set {
				SetProperty(propertyPath + "LineStyle", new LineStyle());
			}
		}
		public WpfChartSeriesBorderPropertyGridModel()
			: this(null, null, string.Empty) {
		}
		public WpfChartSeriesBorderPropertyGridModel(WpfChartModel chartModel, SeriesBorder border, string propertyPath)
			: base(chartModel) {
			this.border = border;
			this.propertyPath = propertyPath;
			setSeriesPropertyCommand = new SetSeriesPropertyCommand(ChartModel);
			UpdateInternal();
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (border != null && Border.LineStyle != null) {
				if (lineStyle != null && Border.LineStyle != lineStyle.LineStyle || lineStyle == null)
					lineStyle = new WpfChartLineStylePropertyGridModel(ChartModel, Border.LineStyle, setSeriesPropertyCommand, propertyPath + "LineStyle.");
			}
			else
				lineStyle = null;
		}
	}
	public class WpfChartReductionStockOptionsPropertyGridModel : PropertyGridModelBase {
		const string propertyPath = "ReductionOptions.";
		readonly SetSeriesPropertyCommand setSeriesPropertyCommand;
		readonly ReductionStockOptions reductionOptions;
		protected override ICommand SetObjectPropertyCommand { get { return setSeriesPropertyCommand; } }
		protected internal ReductionStockOptions ReductionOptions { get { return reductionOptions; } }
		[Category(Categories.Brushes)]
		public SolidColorBrush Brush {
			get { return reductionOptions.Brush; }
			set {
				SetProperty(propertyPath + "Brush", value);
			}
		}
		[Category(Categories.Behavior)]
		public StockLevel Level {
			get { return reductionOptions.Level; }
			set {
				SetProperty(propertyPath + "Level", value);
			}
		}
		[Category(Categories.Behavior)]
		public bool Enabled {
			get { return reductionOptions.Enabled; }
			set {
				SetProperty(propertyPath + "Enabled", value);
			}
		}
		public WpfChartReductionStockOptionsPropertyGridModel()
			: this(null, null) {
		}
		public WpfChartReductionStockOptionsPropertyGridModel(WpfChartModel chartModel, ReductionStockOptions reductionOptions)
			: base(chartModel) {
			this.reductionOptions = reductionOptions;
			setSeriesPropertyCommand = new SetSeriesPropertyCommand(ChartModel);
		}
	}
	public class WpfChartFunnelSeries2DPropertyGridModel : WpfChartSeriesPropertyGridModel {
		new FunnelSeries2D Series { get { return base.Series as FunnelSeries2D; } }
		WpfChartSeriesBorderPropertyGridModel border;
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Presentation)
		]
		public WpfChartSeriesBorderPropertyGridModel Border {
			get { return border; }
			set {
				SetProperty("Border", new SeriesBorder());
			}
		}
		[Category(Categories.Behavior)]
		public int PointDistance {
			get { return Series.PointDistance; }
			set { SetProperty("PointDistance", value); }
		}
		[Category(Categories.Behavior)]
		public double HeightToWidthRatio {
			get { return Series.HeightToWidthRatio; }
			set { SetProperty("HeightToWidthRatio", value); }
		}
		[Category(Categories.Behavior)]
		public bool HeightToWidthRatioAuto {
			get { return Series.HeightToWidthRatioAuto; }
			set { SetProperty("HeightToWidthRatioAuto", value); }
		}
		[Category(Categories.Behavior)]
		public bool AlignToCenter {
			get { return Series.AlignToCenter; }
			set { SetProperty("AlignToCenter", value); }
		}
		[Category(Categories.Animation)]
		public Funnel2DSeriesPointAnimationBase PointAnimation {
			get { return Series.PointAnimation; }
			set { SetProperty("PointAnimation", value); }
		}
		public WpfChartFunnelSeries2DPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesModel seriesModel)
			: base(chartModel, seriesModel) {
			UpdateInternal();
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (Series.Border != null) {
				if (border != null && Series.Border != border.Border || border == null)
					border = new WpfChartSeriesBorderPropertyGridModel(ChartModel, Series.Border, "Border.");
			}
			else
				border = null;
		}
	}
}
