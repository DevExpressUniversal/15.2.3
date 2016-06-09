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
using DevExpress.Xpf.Charts;
using System.Collections.ObjectModel;
using System;
using DevExpress.Xpf.Charts.Native;
using System.Reflection;
using DevExpress.Charts.Native;
namespace DevExpress.Charts.Designer.Native {
	public enum ComplexLabelPosition {
		Disabled,
		Bar2DCenter,
		Bar2DOutside,
		PieInside,
		PieOutside,
		PieTwoColumns,
		Bubble2DCenter,
		Marker3DCenter,
		Marker3DTop,
		FinancialEnabled,
		Bar3DEnabled,
		Area3DEnabled,
		Angle0,
		Angle45,
		Angle90,
		Angle135,
		Angle180,
		Angle225,
		Angle270,
		Angle315,
		RangeAreaMaxValueLabel,
		RangeAreaMinValueLabel,
		RangeAreaOneLabel,
		RangeAreaTwoLabels,
		RangeAreaValue1Label,
		RangeAreaValue2Label,
		RangeBarMaxValueLabel,
		RangeBarMinValueLabel,
		RangeBarOneLabel,
		RangeBarTwoLabels,
		FunnelCenter,
		FunnelLeft,
		FunnelLeftColumn,
		FunnelRight,
		FunnelRightColumn,
		Bar2DAuto,
	}
	public class WpfChartSeriesModel : ChartModelElement {
		readonly WpfChartIndicatorCollectionModel indicatorCollectionModel;
		readonly WpfChartSeriesPointCollectionModel seriesPointCollectionModel;
		readonly object predefinedDataSource;
		public static object ComplexToFact(ComplexLabelPosition position) {
			switch (position) {
				case ComplexLabelPosition.Bar2DCenter:
					return Bar2DLabelPosition.Center;
				case ComplexLabelPosition.Bar2DOutside:
					return Bar2DLabelPosition.Outside;
				case ComplexLabelPosition.Bar2DAuto:
					return Bar2DLabelPosition.Auto;
				case ComplexLabelPosition.PieInside:
					return PieLabelPosition.Inside;
				case ComplexLabelPosition.PieOutside:
					return PieLabelPosition.Outside;
				case ComplexLabelPosition.PieTwoColumns:
					return PieLabelPosition.TwoColumns;
				case ComplexLabelPosition.Bubble2DCenter:
					return Bubble2DLabelPosition.Center;
				case ComplexLabelPosition.Marker3DCenter:
					return Marker3DLabelPosition.Center;
				case ComplexLabelPosition.Marker3DTop:
					return Marker3DLabelPosition.Top;
				case ComplexLabelPosition.RangeAreaMaxValueLabel:
					return RangeAreaLabelKind.MaxValueLabel;
				case ComplexLabelPosition.RangeAreaMinValueLabel:
					return RangeAreaLabelKind.MinValueLabel;
				case ComplexLabelPosition.RangeAreaOneLabel:
					return RangeAreaLabelKind.OneLabel;
				case ComplexLabelPosition.RangeAreaTwoLabels:
					return RangeAreaLabelKind.TwoLabels;
				case ComplexLabelPosition.RangeAreaValue1Label:
					return RangeAreaLabelKind.Value1Label;
				case ComplexLabelPosition.RangeAreaValue2Label:
					return RangeAreaLabelKind.Value2Label;
				case ComplexLabelPosition.RangeBarMaxValueLabel:
					return RangeBarLabelKind.MaxValueLabel;
				case ComplexLabelPosition.RangeBarMinValueLabel:
					return RangeBarLabelKind.MinValueLabel;
				case ComplexLabelPosition.RangeBarOneLabel:
					return RangeBarLabelKind.OneLabel;
				case ComplexLabelPosition.RangeBarTwoLabels:
					return RangeBarLabelKind.TwoLabels;
				case ComplexLabelPosition.FunnelCenter:
					return Funnel2DLabelPosition.Center;
				case ComplexLabelPosition.FunnelLeft:
					return Funnel2DLabelPosition.Left;
				case ComplexLabelPosition.FunnelRight:
					return Funnel2DLabelPosition.Right;
				case ComplexLabelPosition.FunnelLeftColumn:
					return Funnel2DLabelPosition.LeftColumn;
				case ComplexLabelPosition.FunnelRightColumn:
					return Funnel2DLabelPosition.RightColumn;
				case ComplexLabelPosition.Disabled:
				default:
					return null;
			}
		}
		public static double ComplexToAngle(ComplexLabelPosition complexLabelPosition) {
			switch (complexLabelPosition) {
				case ComplexLabelPosition.Angle0:
					return 0.0d;
				case ComplexLabelPosition.Angle45:
					return 45.0d;
				case ComplexLabelPosition.Angle90:
					return 90.0d;
				case ComplexLabelPosition.Angle135:
					return 135.0d;
				case ComplexLabelPosition.Angle180:
					return 180.0d;
				case ComplexLabelPosition.Angle225:
					return 225.0d;
				case ComplexLabelPosition.Angle270:
					return 270.0d;
				case ComplexLabelPosition.Angle315:
					return 315.0d;
			}
			return double.NaN;
		}
		public static void SetComplexLabelPosition(Series series, ComplexLabelPosition complexLabelPosition) {
			if (complexLabelPosition == ComplexLabelPosition.Disabled) {
				series.LabelsVisibility = false;
			}
			else {
				series.LabelsVisibility = true;
				if (series is BarSideBySideSeries2D)
					BarSideBySideSeries2D.SetLabelPosition(series.ActualLabel, (Bar2DLabelPosition)ComplexToFact(complexLabelPosition));
				else if (series is PieSeries2D)
					PieSeries2D.SetLabelPosition(series.ActualLabel, (PieLabelPosition)ComplexToFact(complexLabelPosition));
				else if (series is PieSeries3D)
					PieSeries3D.SetLabelPosition(series.ActualLabel, (PieLabelPosition)ComplexToFact(complexLabelPosition));
				else if (series is FunnelSeries2D)
					FunnelSeries2D.SetLabelPosition(series.ActualLabel, (Funnel2DLabelPosition)ComplexToFact(complexLabelPosition));
				else if (series is MarkerSeries3D)
					MarkerSeries3D.SetLabelPosition(series.ActualLabel, (Marker3DLabelPosition)ComplexToFact(complexLabelPosition));
				else if (series is BubbleSeries2D) {
					object bubblePosition = ComplexToFact(complexLabelPosition);
					if (bubblePosition != null)
						BubbleSeries2D.SetLabelPosition(series.ActualLabel, (Bubble2DLabelPosition)bubblePosition);
					else {
						double angle = ComplexToAngle(complexLabelPosition);
						if (!double.IsNaN(angle)) {
							BubbleSeries2D.SetLabelPosition(series.ActualLabel, Bubble2DLabelPosition.Outside);
							MarkerSeries2D.SetAngle(series.ActualLabel, angle);
						}
					}
				}
				else if (series is MarkerSeries2D)
					MarkerSeries2D.SetAngle(series.ActualLabel, ComplexToAngle(complexLabelPosition));
				else if (series is CircularSeries2D)
					CircularSeries2D.SetAngle(series.ActualLabel, ComplexToAngle(complexLabelPosition));
				else if (series is RangeAreaSeries2D)
					RangeAreaSeries2D.SetLabelKind(series.ActualLabel, (RangeAreaLabelKind)ComplexToFact(complexLabelPosition));
				else if (series is RangeBarSeries2D)
					RangeBarSeries2D.SetLabelKind(series.ActualLabel, (RangeBarLabelKind)ComplexToFact(complexLabelPosition));
			}
		}
		WpfChartPaneModel pane;
		WpfChartAxisModel axisX;
		WpfChartAxisModel axisY;
		object dataSource;
		bool isSeriesTemplatePreview;
		DataMemberInfo argumentDataMember;
		DataMemberInfo valueDataMember;
		DataMemberInfo value2DataMember;
		DataMemberInfo weightDataMember;
		DataMemberInfo lowValueDataMember;
		DataMemberInfo highValueDataMember;
		DataMemberInfo openValueDataMember;
		DataMemberInfo closeValueDataMember;
		DataMemberInfo colorDataMember;
		public WpfChartDiagramModel Diagram {
			get { return (WpfChartDiagramModel)GetParent<WpfChartDiagramModel>(); }
		}
		public override IEnumerable<ChartModelElement> Children {
			get { return new ChartModelElement[] { indicatorCollectionModel, seriesPointCollectionModel }; }
		}
		public override ChartModelElement SelectionOverride {
			get {
				if (IsAutoSeries)
					return Diagram.SeriesTemplateModel;
				return base.SelectionOverride;
			}
		}
		public WpfChartSeriesPointCollectionModel SeriesPointCollectionModel {
			get { return seriesPointCollectionModel; }
		}
		public WpfChartIndicatorCollectionModel IndicatorCollectionModel {
			get { return indicatorCollectionModel; }
		}
		public Series Series {
			get { return (Series)ChartElement; }
		}
		public SeriesLabel Label {
			get { return Series.ActualLabel; }
		}
		public string Name {
			get { return Series.DisplayName; }
			set {
				if (Series.DisplayName != value) {
					Series.DisplayName = value;
					OnPropertyChanged("Name");
				}
			}
		}
		public bool IsVisible {
			get { return Series.Visible; }
			set {
				if (Series.Visible != value) {
					Series.Visible = value;
					OnPropertyChanged("IsVisible");
				}
			}
		}
		public int LabelConnectorThickness {
			get { return Series.ActualLabel.ConnectorThickness; }
			set {
				if (Series.ActualLabel.ConnectorThickness != value) {
					Series.ActualLabel.ConnectorThickness = value;
					OnPropertyChanged("LabelConnectorThickness");
				}
			}
		}
		public bool LabelConnectorVisible {
			get { return Series.ActualLabel.ConnectorVisible; }
			set {
				if (Series.ActualLabel.ConnectorVisible != value) {
					Series.ActualLabel.ConnectorVisible = value;
					OnPropertyChanged("LabelConnectorVisible");
				}
			}
		}
		public int LabelIndent {
			get { return Series.ActualLabel.Indent; }
			set {
				if (Series.ActualLabel.Indent != value) {
					Series.ActualLabel.Indent = value;
					OnPropertyChanged("LabelIndent");
				}
			}
		}
		public ResolveOverlappingMode LabelResolveOverlappingModel {
			get { return Series.ActualLabel.ResolveOverlappingMode; }
			set {
				if (Series.ActualLabel.ResolveOverlappingMode != value) {
					Series.ActualLabel.ResolveOverlappingMode = value;
					OnPropertyChanged("LabelResolveOverlappingModel");
				}
			}
		}
		public bool IsAutoSeries {
			get { return ChartDesignerPropertiesProvider.GetIsAutoSeries(Series); }
		}
		public bool IsSeriesTemplate { 
			get { return Series == Diagram.Diagram.SeriesTemplate; } 
		}
		public bool IsSeriesTemplatePreview {
			get { return isSeriesTemplatePreview; }
			set { isSeriesTemplatePreview = value; }
		}
		public bool IsAutoPointsAdded { 
			get { return ChartDesignerPropertiesProvider.GetIsAutoPointsAdded(Series); }
		}	   
		public ComplexLabelPosition ComplexLabelPosition {
			get {
				if ((Series != null) && Series.LabelsVisibility) {
					if (Series is BarSideBySideSeries2D)
						return FactToComplex(BarSideBySideSeries2D.GetLabelPosition(Series.ActualLabel));
					else if (Series is PieSeries2D)
						return FactToComplex(PieSeries2D.GetLabelPosition(Series.ActualLabel));
					else if (Series is FunnelSeries2D)
						return FactToComplex(FunnelSeries2D.GetLabelPosition(Series.ActualLabel));
					else if (Series is PieSeries3D)
						return FactToComplex(PieSeries3D.GetLabelPosition(Series.ActualLabel));
					else if (Series is MarkerSeries3D)
						return FactToComplex(MarkerSeries3D.GetLabelPosition(Series.ActualLabel));
					else if (Series is BubbleSeries2D)
						return FactToComplex(BubbleSeries2D.GetLabelPosition(Series.ActualLabel));
					else if (Series is FinancialSeries2D)
						return ComplexLabelPosition.FinancialEnabled;
					else if (Series is BarSeries3D)
						return ComplexLabelPosition.Bar3DEnabled;
					else if (Series is AreaSeries3D)
						return ComplexLabelPosition.Area3DEnabled;
					else if (Series is MarkerSeries2D)
						return AngleToComplex(MarkerSeries2D.GetAngle(Series.ActualLabel));
					else if (Series is CircularSeries2D)
						return AngleToComplex(CircularSeries2D.GetAngle(Series.ActualLabel));
					else if (Series is BarStackedSeries2D)
						return ComplexLabelPosition.Bar2DCenter;
					else if (Series is RangeAreaSeries2D)
						return RangeAreaLabelKindToComplexLabelPosition(RangeAreaSeries2D.GetLabelKind(Series.ActualLabel));
					else if (Series is RangeBarSeries2D)
						return RangeBarLabelKindToComplexLabelPosition(RangeBarSeries2D.GetLabelKind(Series.ActualLabel));
				}
				return ComplexLabelPosition.Disabled;
			}
			set {
				if (ComplexLabelPosition != value) {
					SetComplexLabelPosition(Series, value);
					OnPropertyChanged("ComplexLabelPosition");
				}
			}
		}
		public WpfChartPaneModel Pane {
			get { return pane; }
			set {
				if (pane != value) {
					pane = value;
					XYDiagram2D.SetSeriesPane((XYSeries)Series, pane.Pane);
					OnPropertyChanged("Pane");
				}
			}
		}
		public WpfChartAxisModel AxisX {
			get { return axisX; }
			set {
				if (axisX != value) {
					axisX = value;
					SecondaryAxisX2D axis = axisX.Axis as SecondaryAxisX2D;
					XYDiagram2D.SetSeriesAxisX((XYSeries)Series, axis);
					OnPropertyChanged("AxisX");
				}
			}
		}
		public WpfChartAxisModel AxisY {
			get { return axisY; }
			set {
				if (axisY != value) {
					axisY = value;
					SecondaryAxisY2D axis = axisY.Axis as SecondaryAxisY2D;
					XYDiagram2D.SetSeriesAxisY((XYSeries)Series, axis);
					OnPropertyChanged("AxisY");
				}
			}
		}
		public object PredefinedDataSource {
			get { return predefinedDataSource; } 
		}
		public object DataSource {
			get { return dataSource; }
			set {
				if (dataSource != value) {
					dataSource = value;
					OnPropertyChanged("DataSource");
				}
			}
		}
		public DataMemberInfo ArgumentDataMember {
			get { return argumentDataMember; }
			set {
				if (argumentDataMember.DataMember != value.DataMember) {
					argumentDataMember = value;
					Series.ArgumentDataMember = argumentDataMember.DataMember;
					OnPropertyChanged("ArgumentDataMember");
				}
			}
		}
		public DataMemberInfo ValueDataMember {
			get { return valueDataMember; }
			set {
				if (valueDataMember.DataMember != value.DataMember) {
					valueDataMember = value;
					Series.ValueDataMember = valueDataMember.DataMember;
					OnPropertyChanged("ValueDataMember");
				}
			}
		}
		public DataMemberInfo Value2DataMember {
			get { return value2DataMember; }
			set {
				if (value2DataMember.DataMember != value.DataMember) {
					value2DataMember = value;
					if (Series is RangeBarSeries2D)
						((RangeBarSeries2D)Series).Value2DataMember = value2DataMember.DataMember;
					if (Series is RangeAreaSeries2D)
						((RangeAreaSeries2D)Series).Value2DataMember = value2DataMember.DataMember;
					OnPropertyChanged("Value2DataMember");
				}
			}
		}
		public DataMemberInfo WeightDataMember {
			get {
				return weightDataMember;
			}
			set {
				if (weightDataMember.DataMember != value.DataMember) {
					weightDataMember = value;
					if (Series is BubbleSeries2D)
						((BubbleSeries2D)Series).WeightDataMember = weightDataMember.DataMember;
					if (Series is BubbleSeries3D)
						((BubbleSeries3D)Series).WeightDataMember = weightDataMember.DataMember;
					OnPropertyChanged("WeightDataMember");
				}
			}
		}
		public DataMemberInfo LowValueDataMember {
			get {
				return lowValueDataMember;
			}
			set {
				if (lowValueDataMember.DataMember != value.DataMember) {
					lowValueDataMember = value;
					if (Series is FinancialSeries2D)
						((FinancialSeries2D)Series).LowValueDataMember = lowValueDataMember.DataMember;
					OnPropertyChanged("LowValueDataMember");
				}
			}
		}
		public DataMemberInfo HighValueDataMember {
			get {
				return highValueDataMember;
			}
			set {
				if (highValueDataMember.DataMember != value.DataMember) {
					highValueDataMember = value;
					if (Series is FinancialSeries2D)
						((FinancialSeries2D)Series).HighValueDataMember = highValueDataMember.DataMember;
					OnPropertyChanged("HighValueDataMember");
				}
			}
		}
		public DataMemberInfo OpenValueDataMember {
			get {
				return openValueDataMember;
			}
			set {
				if (openValueDataMember.DataMember != value.DataMember) {
					openValueDataMember = value;
					if (Series is FinancialSeries2D)
						((FinancialSeries2D)Series).OpenValueDataMember = openValueDataMember.DataMember;
					OnPropertyChanged("OpenValueDataMember");
				}
			}
		}
		public DataMemberInfo CloseValueDataMember {
			get {
				return closeValueDataMember;
			}
			set {
				if (closeValueDataMember.DataMember != value.DataMember) {
					closeValueDataMember = value;
					if (Series is FinancialSeries2D)
						((FinancialSeries2D)Series).CloseValueDataMember = closeValueDataMember.DataMember;
					OnPropertyChanged("CloseValueDataMember");
				}
			}
		}
		public DataMemberInfo ColorDataMember {
			get { return colorDataMember; }
			set {
				if (colorDataMember.DataMember != value.DataMember) {
					colorDataMember = value;
					Series.ColorDataMember = colorDataMember.DataMember;
					OnPropertyChanged("ColorDataMember");
				}
			}
		}
		public double HoleRadiusPercent { 
			get {
				if (Series is PieSeries)
					return ((PieSeries)Series).HoleRadiusPercent;
				else
					return 0.0;
			}
			set {
				if (Series is PieSeries) {
					((PieSeries)Series).HoleRadiusPercent = value;
					OnPropertyChanged("HoleRadiusPercent");
				}
				else
					WriteUnsupportedPropertySetWarning("HoleRadiusPercent", Series.GetType().Name);
			}
		}
		public int PointDistance {
			get {
				if (Series is FunnelSeries2D)
					return ((FunnelSeries2D)Series).PointDistance;
				else
					return 0;
			}
			set {
				if (Series is FunnelSeries2D) {
					((FunnelSeries2D)Series).PointDistance = value;
					OnPropertyChanged("PointDistance");
				}
				else
					WriteUnsupportedPropertySetWarning("PointDistance", Series.GetType().Name);
			}
		}
		public bool AlignToCenter {
			get {
				if (Series is FunnelSeries2D)
					return ((FunnelSeries2D)Series).AlignToCenter;
				else
					return false;
			}
			set {
				if (Series is FunnelSeries2D) {
					((FunnelSeries2D)Series).AlignToCenter = value;
					OnPropertyChanged("AlignToCenter");
				}
				else
					WriteUnsupportedPropertySetWarning("AlignToCenter", Series.GetType().Name);
			}
		}
		public bool HeightToWidthRatioAuto {
			get {
				if (Series is FunnelSeries2D)
					return ((FunnelSeries2D)Series).HeightToWidthRatioAuto;
				else
					return false;
			}
			set {
				if (Series is FunnelSeries2D) {
					((FunnelSeries2D)Series).HeightToWidthRatioAuto = value;
					OnPropertyChanged("HeightToWidthRatioAuto");
				}
				else
					WriteUnsupportedPropertySetWarning("HeightToWidthRatioAuto", Series.GetType().Name);
			}
		}
		public double HeightToWidthRatio {
			get {
				if (Series is FunnelSeries2D)
					return ((FunnelSeries2D)Series).HeightToWidthRatio;
				else
					return 0.0;
			}
			set {
				if (Series is FunnelSeries2D) {
					((FunnelSeries2D)Series).HeightToWidthRatio = value;
					OnPropertyChanged("HeightToWidthRatio");
				}
				else
					WriteUnsupportedPropertySetWarning("HeightToWidthRatio", Series.GetType().Name);
			}
		}
		public ScaleType ActualArgumentScaleType {
			get { return Series.ActualArgumentScaleType; }
		}
		public ScaleType ArgumentScaleType { 
			get { return Series.ArgumentScaleType; } 
		}
		public ScaleType ValueScaleType {
			get { return Series.ValueScaleType; }
			set {
				if (Series.ValueScaleType != value) {
					Series.ValueScaleType = value;
					OnPropertyChanged("ValueScaleType");
				}
			}
		}
		public object Group {
			get {
				if (Series is NestedDonutSeries2D)
					return ((NestedDonutSeries2D)Series).Group;
				else
					return 0.0;
			}
			set {
				if (Series is NestedDonutSeries2D) {
					((NestedDonutSeries2D)Series).Group = value;
					OnPropertyChanged("Group");
				}
				else
					WriteUnsupportedPropertySetWarning("Group", Series.GetType().Name);
			}
		}
		public double InnerIndent {
			get {
				if (Series is NestedDonutSeries2D)
					return ((NestedDonutSeries2D)Series).InnerIndent;
				else
					return NestedDonutSeries2D.DefaultInnerIndent;
			}
			set {
				if (Series is NestedDonutSeries2D) {
					((NestedDonutSeries2D)Series).InnerIndent = value;
					OnPropertyChanged("InnerIndent");
				}
				else
					WriteUnsupportedPropertySetWarning("InnerIndent", Series.GetType().Name);
			}
		}
		public double Weight {
			get {
				if (Series is NestedDonutSeries2D)
					return ((NestedDonutSeries2D)Series).Weight;
				else
					return NestedDonutSeries2D.DefaultWeight;
			}
			set {
				if (Series is NestedDonutSeries2D) {
					((NestedDonutSeries2D)Series).Weight = value;
					OnPropertyChanged("Weight");
				}
				else
					WriteUnsupportedPropertySetWarning("Weight", Series.GetType().Name);
			}
		}
		public WpfChartSeriesModel(ChartModelElement parent, Series series)
			: base(parent, series) {
			this.seriesPointCollectionModel = new WpfChartSeriesPointCollectionModel(this, series.Points);
			if (Series is XYSeries2D) {
				this.indicatorCollectionModel = new WpfChartIndicatorCollectionModel(this, ((XYSeries2D)Series).Indicators);
				UpdatePane();
				UpdateAxes();
			}
			predefinedDataSource = series.DataSource;
			if (series.DataSource != null)
				dataSource = series.DataSource;
			else {
				UpdateDataMembers(series);
				if (series.DataSource == null && Diagram.ChartModel.DataSource != null &&
					(this.IsSeriesTemplate ||
					!String.IsNullOrEmpty(ArgumentDataMember.DataMember) ||
					!String.IsNullOrEmpty(ValueDataMember.DataMember) ||
					!String.IsNullOrEmpty(Value2DataMember.DataMember) ||
					!String.IsNullOrEmpty(WeightDataMember.DataMember) ||
					!String.IsNullOrEmpty(LowValueDataMember.DataMember) ||
					!String.IsNullOrEmpty(HighValueDataMember.DataMember) ||
					!String.IsNullOrEmpty(OpenValueDataMember.DataMember) ||
					!String.IsNullOrEmpty(CloseValueDataMember.DataMember) ||
					!String.IsNullOrEmpty(ColorDataMember.DataMember)))
					dataSource = Diagram.ChartModel.DataSource;
			}
			UpdateDataMembers(series);
			PropertyGridModel = WpfChartSeriesPropertyGridModel.CreatePropertyGridModelForSeries(this, Diagram.ChartModel);
		}
		ComplexLabelPosition RangeBarLabelKindToComplexLabelPosition(RangeBarLabelKind rangeBarLabelKind) {
			switch (rangeBarLabelKind) {
				case RangeBarLabelKind.MaxValueLabel:
					return ComplexLabelPosition.RangeBarMaxValueLabel;
				case RangeBarLabelKind.MinValueLabel:
					return ComplexLabelPosition.RangeBarMinValueLabel;
				case RangeBarLabelKind.OneLabel:
					return ComplexLabelPosition.RangeBarOneLabel;
				case RangeBarLabelKind.TwoLabels:
					return ComplexLabelPosition.RangeBarTwoLabels;
				default:
					throw new ChartDesignerException("Unknown RangeBarLabelKind.");
			}
		}
		ComplexLabelPosition RangeAreaLabelKindToComplexLabelPosition(RangeAreaLabelKind rangeAreaLabelKind) {
			switch (rangeAreaLabelKind) {
				case RangeAreaLabelKind.MaxValueLabel:
					return ComplexLabelPosition.RangeAreaMaxValueLabel;
				case RangeAreaLabelKind.MinValueLabel:
					return ComplexLabelPosition.RangeAreaMinValueLabel;
				case RangeAreaLabelKind.OneLabel:
					return ComplexLabelPosition.RangeAreaOneLabel;
				case RangeAreaLabelKind.TwoLabels:
					return ComplexLabelPosition.RangeAreaTwoLabels;
				case RangeAreaLabelKind.Value1Label:
					return ComplexLabelPosition.RangeAreaValue1Label;
				case RangeAreaLabelKind.Value2Label:
					return ComplexLabelPosition.RangeAreaValue2Label;
				default:
					throw new ChartDesignerException("Unknown RangeAreaLabelKind");
			}
		}
		ComplexLabelPosition FactToComplex(object position) {
			if (position == null)
				return ComplexLabelPosition.Disabled;
			if (position.GetType() == typeof(Bar2DLabelPosition)) {
				switch ((Bar2DLabelPosition)position) {
					case Bar2DLabelPosition.Center:
						return ComplexLabelPosition.Bar2DCenter;
					case Bar2DLabelPosition.Outside:
						return ComplexLabelPosition.Bar2DOutside;
					case Bar2DLabelPosition.Auto:
						return ComplexLabelPosition.Bar2DAuto;
					default:
						return ComplexLabelPosition.Disabled;
				}
			}
			else if (position.GetType() == typeof(PieLabelPosition)) {
				switch ((PieLabelPosition)position) {
					case PieLabelPosition.Inside:
						return ComplexLabelPosition.PieInside;
					case PieLabelPosition.Outside:
						return ComplexLabelPosition.PieOutside;
					case PieLabelPosition.TwoColumns:
						return ComplexLabelPosition.PieTwoColumns;
					default:
						return ComplexLabelPosition.Disabled;
				}
			}
			else if (position.GetType() == typeof(Funnel2DLabelPosition)) {
				switch ((Funnel2DLabelPosition)position) {
					case Funnel2DLabelPosition.Center:
						return ComplexLabelPosition.FunnelCenter;
					case Funnel2DLabelPosition.Left:
						return ComplexLabelPosition.FunnelLeft;
					case Funnel2DLabelPosition.LeftColumn:
						return ComplexLabelPosition.FunnelLeftColumn;
					case Funnel2DLabelPosition.Right:
						return ComplexLabelPosition.FunnelRight;
					case Funnel2DLabelPosition.RightColumn:
						return ComplexLabelPosition.FunnelRightColumn;
					default:
						return ComplexLabelPosition.Disabled;
				}
			}
			else if (position.GetType() == typeof(Marker3DLabelPosition)) {
				switch ((Marker3DLabelPosition)position) {
					case Marker3DLabelPosition.Center:
						return ComplexLabelPosition.Marker3DCenter;
					case Marker3DLabelPosition.Top:
						return ComplexLabelPosition.Marker3DTop;
					default:
						return ComplexLabelPosition.Disabled;
				}
			}
			else if (position.GetType() == typeof(Bubble2DLabelPosition)) {
				switch ((Bubble2DLabelPosition)position) {
					case Bubble2DLabelPosition.Center:
						return ComplexLabelPosition.Bubble2DCenter;
					case Bubble2DLabelPosition.Outside:
						return AngleToComplex(MarkerSeries2D.GetAngle(Series.ActualLabel));
					default:
						return ComplexLabelPosition.Disabled;
				}
			}
			return ComplexLabelPosition.Disabled;
		}
		ComplexLabelPosition AngleToComplex(double angle) {
			int discreteAngle = (int)Math.Floor((angle % 360.0d) / 45.0d);
			switch (discreteAngle) { 
				case 0:
					return ComplexLabelPosition.Angle0;
				case 1:
					return ComplexLabelPosition.Angle45;
				case 2:
					return ComplexLabelPosition.Angle90;
				case 3:
					return ComplexLabelPosition.Angle135;
				case 4:
					return ComplexLabelPosition.Angle180;
				case 5:
					return ComplexLabelPosition.Angle225;
				case 6:
					return ComplexLabelPosition.Angle270;
				case 7:
					return ComplexLabelPosition.Angle315;
			}
			return ComplexLabelPosition.Angle0;
		}
		void UpdatePane() {
			Pane seriesPane = XYDiagram2D.GetSeriesPane((XYSeries)Series);
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
		void UpdateAxes() {
			SecondaryAxisX2D seriesAxisX = XYDiagram2D.GetSeriesAxisX((XYSeries)Series);
			WpfChartDiagramModel diagramModel = Diagram;
			if (seriesAxisX == null)
				AxisX = diagramModel.PrimaryAxisModelX;
			else
				if (diagramModel.SecondaryAxesCollectionModelX.ModelCollection.Count > 0)
					foreach (ChartModelElement axisModel in diagramModel.SecondaryAxesCollectionModelX.ModelCollection)
						if (((WpfChartAxisModel)axisModel).Axis == seriesAxisX) {
							AxisX = (WpfChartAxisModel)axisModel;
							break;
						}
			SecondaryAxisY2D seriesAxisY = XYDiagram2D.GetSeriesAxisY((XYSeries)Series);
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
		void UpdateDataMembers(Series series) {
			argumentDataMember = new DataMemberInfo(GetDataMemberName(dataSource, series.ArgumentDataMember), series.ArgumentDataMember);
			valueDataMember = new DataMemberInfo(GetDataMemberName(dataSource, series.ValueDataMember), series.ValueDataMember);
			string value2Member = string.Empty;
			if (Series is RangeBarSeries2D)
				value2Member = ((RangeBarSeries2D)Series).Value2DataMember;
			if (Series is RangeAreaSeries2D)
				value2Member = ((RangeAreaSeries2D)Series).Value2DataMember;
			value2DataMember = new DataMemberInfo(GetDataMemberName(dataSource, value2Member), value2Member);
			string weightMember = string.Empty;
			if (Series is BubbleSeries2D)
				weightMember = ((BubbleSeries2D)Series).WeightDataMember;
			if (Series is BubbleSeries3D)
				weightMember = ((BubbleSeries3D)Series).WeightDataMember;
			weightDataMember = new DataMemberInfo(GetDataMemberName(dataSource, weightMember), weightMember);
			string lowValueMember = string.Empty;
			if (Series is FinancialSeries2D)
				lowValueMember = ((FinancialSeries2D)Series).LowValueDataMember;
			lowValueDataMember = new DataMemberInfo(GetDataMemberName(dataSource, lowValueMember), lowValueMember);
			string highValueMember = string.Empty;
			if (Series is FinancialSeries2D)
				highValueMember = ((FinancialSeries2D)Series).HighValueDataMember;
			highValueDataMember = new DataMemberInfo(GetDataMemberName(dataSource, highValueMember), highValueMember);
			string openValueMember = string.Empty;
			if (Series is FinancialSeries2D)
				openValueMember = ((FinancialSeries2D)Series).OpenValueDataMember;
			openValueDataMember = new DataMemberInfo(GetDataMemberName(dataSource, openValueMember), openValueMember);
			string closeValueMember = string.Empty;
			if (Series is FinancialSeries2D)
				closeValueMember = ((FinancialSeries2D)Series).CloseValueDataMember;
			closeValueDataMember = new DataMemberInfo(GetDataMemberName(dataSource, closeValueMember), closeValueMember);
			colorDataMember = new DataMemberInfo(GetDataMemberName(dataSource, series.ColorDataMember), series.ColorDataMember);
		}
		protected override void UpdateChildren() {
			if (Series is XYSeries2D) {
				UpdatePane();
				UpdateAxes();
			}
		}
		public int GetSelfIndex() {
			return Parent is WpfChartSeriesCollectionModel ? Diagram.Diagram.Series.IndexOf(Series) : -1;
		}
		public int GetSelfDesigntimeIndex() {
			if (Parent is WpfChartSeriesCollectionModel) {
				int autoSeriesCount = 0;
				foreach (Series series in Diagram.Diagram.Series)
					if (ChartDesignerPropertiesProvider.GetIsAutoSeries(series))
						autoSeriesCount++;
				return Diagram.Diagram.Series.IndexOf(Series) - autoSeriesCount;
			}
			else
				return -1;
		}
		[SkipOnPropertyChangedMethodCall]
		public void SetChartDataSource(object dataSource) {
			this.dataSource = dataSource;
			Series.DataSource = null;
			OnPropertyChanged("DataSource");
		}
		[SkipOnPropertyChangedMethodCall]
		public void SetSeriesDataSource() {
			this.dataSource = predefinedDataSource;
			Series.DataSource = predefinedDataSource;
			OnPropertyChanged("DataSource");
		}
	}
}
