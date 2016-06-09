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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Design.DataAccess;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Design;
using DevExpress.Xpf.Charts.Localization;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum ScaleType {
		Qualitative = Scale.Qualitative,
		Numerical = Scale.Numerical,
		DateTime = Scale.DateTime,
		Auto = Scale.Auto
	}
	[
	ContentProperty("Points"),
	DataAccessMetadata("TypedDataSet,EntityFramework,LinqToSql,IEnumerable", DataSourceProperty = "DataSource")
	]
	public abstract class Series : ChartElement, ISeries, ISeriesTemplate, ISeriesView, ILegendVisible, IWeakEventListener, IAnimatableElement, IWizardDataProvider, IInteractiveElement, IPatternHolder {
		internal const string StoryboardResourceKey = "storyboard";
		static readonly DependencyPropertyKey ActualLabelPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualLabel",
			typeof(SeriesLabel), typeof(Series), new PropertyMetadata(null, ActualLabelChanged));
		static readonly DependencyPropertyKey DataFiltersPropertyKey = DependencyPropertyManager.RegisterReadOnly("DataFilters", typeof(DataFilterCollection), typeof(Series), new PropertyMetadata());
		static readonly DependencyPropertyKey PointsPropertyKey = DependencyPropertyManager.RegisterReadOnly("Points", typeof(SeriesPointCollection), typeof(Series), new PropertyMetadata());
		public static readonly DependencyProperty ActualLabelProperty = ActualLabelPropertyKey.DependencyProperty;
		public static readonly DependencyProperty DataFiltersProperty = DataFiltersPropertyKey.DependencyProperty;
		public static readonly DependencyProperty PointsProperty = PointsPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ArgumentDataMemberProperty = DependencyPropertyManager.Register("ArgumentDataMember",
			typeof(string), typeof(Series), new PropertyMetadata(String.Empty, OnBindingChanged));
		public static readonly DependencyProperty ArgumentScaleTypeProperty = DependencyPropertyManager.Register("ArgumentScaleType",
			typeof(ScaleType), typeof(Series), new PropertyMetadata(ScaleType.Auto, ArgumentScaleTypePropertyChanged));
		public static readonly DependencyProperty DataFiltersConjunctionModeProperty = DependencyPropertyManager.Register("DataFiltersConjunctionMode",
			typeof(ConjunctionTypes), typeof(Series), new PropertyMetadata(ConjunctionTypes.And, DataFiltersConjunctionModeChanged));
		public static readonly DependencyProperty DataSourceProperty = DependencyPropertyManager.Register("DataSource",
			typeof(object), typeof(Series), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, DataSourcePropertyChanged, new CoerceValueCallback(CoerceDataSource)));
		public static readonly DependencyProperty DisplayNameProperty = DependencyPropertyManager.Register("DisplayName",
			typeof(string), typeof(Series), new PropertyMetadata(String.Empty, ChartElementHelper.UpdateWithClearDiagramCache));
		public static readonly DependencyProperty LabelProperty = DependencyPropertyManager.Register("Label",
			typeof(SeriesLabel), typeof(Series), new PropertyMetadata(LabelChanged));
		[Obsolete(ObsoleteMessages.PointOptionsProperty)]
		public static readonly DependencyProperty PointOptionsProperty = DependencyPropertyManager.Register("PointOptions",
			typeof(PointOptions), typeof(Series), new PropertyMetadata(PointOptionsPropertyChanged));
		[Obsolete(ObsoleteMessages.LegendPointOptionsProperty)]
		public static readonly DependencyProperty LegendPointOptionsProperty = DependencyPropertyManager.Register("LegendPointOptions",
			typeof(PointOptions), typeof(Series), new PropertyMetadata(LegendPointOptionsPropertyChanged));
		public static readonly DependencyProperty LegendTextPatternProperty = DependencyPropertyManager.Register("LegendTextPattern",
			typeof(string), typeof(Series), new PropertyMetadata(String.Empty, LegendTextPatternPropertyChanged));
		public static readonly DependencyProperty ShowInLegendProperty = DependencyPropertyManager.Register("ShowInLegend",
			typeof(bool), typeof(Series), new PropertyMetadata(true, ChartElementHelper.ChangeOwnerAndUpdate));
		public static readonly DependencyProperty ValueDataMemberProperty = DependencyPropertyManager.Register("ValueDataMember",
			typeof(string), typeof(Series), new PropertyMetadata(String.Empty, OnBindingChanged));
		public static readonly DependencyProperty ColorDataMemberProperty = DependencyPropertyManager.Register("ColorDataMember",
			typeof(string), typeof(Series), new PropertyMetadata(String.Empty, OnBindingChanged));
		public static readonly DependencyProperty ValueScaleTypeProperty = DependencyPropertyManager.Register("ValueScaleType",
			typeof(ScaleType), typeof(Series), new PropertyMetadata(ScaleType.Numerical, ValueScaleTypePropertyChanged), ValidateValueScaleType);
		public static readonly DependencyProperty VisibleProperty = DependencyPropertyManager.Register("Visible",
			typeof(bool), typeof(Series), new PropertyMetadata(true, VisiblePropertyChanged));
		public static readonly DependencyProperty CheckedInLegendProperty = DependencyPropertyManager.Register("CheckedInLegend",
			typeof(bool), typeof(Series), new PropertyMetadata(true, CheckedInLegendPropertyChanged));
		public static readonly DependencyProperty CheckableInLegendProperty = DependencyPropertyManager.Register("CheckableInLegend",
			typeof(bool), typeof(Series), new PropertyMetadata(true, ChartElementHelper.Update));
		public static readonly DependencyProperty AnimationAutoStartModeProperty = DependencyPropertyManager.Register("AnimationAutoStartMode",
			typeof(AnimationAutoStartMode), typeof(Series), new PropertyMetadata(AnimationAutoStartMode.PlayOnce, ChartElementHelper.Update));
		public static readonly DependencyProperty LegendMarkerTemplateProperty = DependencyPropertyManager.Register("LegendMarkerTemplate",
			typeof(DataTemplate), typeof(Series), new PropertyMetadata(null, ChartElementHelper.Update));
		public static readonly DependencyProperty ToolTipEnabledProperty = DependencyPropertyManager.Register("ToolTipEnabled",
			typeof(bool?), typeof(Series), new PropertyMetadata(null, ChartElementHelper.Update));
		public static readonly DependencyProperty ToolTipHintProperty = DependencyPropertyManager.Register("ToolTipHint",
			typeof(object), typeof(Series));
		public static readonly DependencyProperty ToolTipSeriesPatternProperty = DependencyPropertyManager.Register("ToolTipSeriesPattern",
			typeof(string), typeof(Series), new PropertyMetadata(String.Empty, ChartElementHelper.Update));
		public static readonly DependencyProperty ToolTipSeriesTemplateProperty = DependencyPropertyManager.Register("ToolTipSeriesTemplate",
			typeof(DataTemplate), typeof(Series), new PropertyMetadata(null, ToolTipTemplatePropertyChanged));
		public static readonly DependencyProperty ToolTipPointPatternProperty = DependencyPropertyManager.Register("ToolTipPointPattern",
			typeof(string), typeof(Series), new PropertyMetadata(String.Empty, ChartElementHelper.Update));
		public static readonly DependencyProperty ToolTipPointTemplateProperty = DependencyPropertyManager.Register("ToolTipPointTemplate",
			typeof(DataTemplate), typeof(Series), new PropertyMetadata(null, ToolTipPointTemplatePropertyChanged));
		public static readonly DependencyProperty ToolTipHintDataMemberProperty = DependencyPropertyManager.Register("ToolTipHintDataMember",
			typeof(string), typeof(Series), new PropertyMetadata(String.Empty, OnBindingChanged));
		public static readonly DependencyProperty LabelsVisibilityProperty = DependencyPropertyManager.Register("LabelsVisibility",
			typeof(bool), typeof(Series), new PropertyMetadata(false, ChartElementHelper.UpdateWithClearDiagramCache));
		public static readonly DependencyProperty ColorizerProperty = DependencyPropertyManager.Register("Colorizer",
			typeof(ChartColorizerBase), typeof(Series), new PropertyMetadata(OnColorizerPropertyChanged));
		static void ActualLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartElementHelper.ChangeOwnerAndUpdate(d, e, ChartElementChange.ClearDiagramCache | ChartElementChange.UpdateXYDiagram2DItems);
		}
		static void ArgumentScaleTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Series series = d as Series;
			if (series != null)
				series.ArgumentScaleTypeInternal = (ScaleType)e.NewValue;
			ChartElementHelper.Update(d, e, ChartElementChange.ClearDiagramCache | ChartElementChange.UpdateSeriesBinding);
		}
		static void ValueScaleTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Series series = d as Series;
			if (series != null)
				series.ValueScaleTypeInternal = (ScaleType)e.NewValue;
			ChartElementHelper.Update(d, e, ChartElementChange.ClearDiagramCache | ChartElementChange.UpdateSeriesBinding);
		}
		static void DataFiltersConjunctionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Series series = d as Series;
			if (series != null && series.DataFilters.Count > 0)
				ChartElementHelper.Update(d, e, ChartElementChange.ClearDiagramCache | ChartElementChange.UpdateSeriesBinding);
			else
				ChartElementHelper.UpdateWithClearDiagramCache(d, e);
		}
		static object CoerceDataSource(DependencyObject d, object value) {
			if (value == null)
				return value;
			Series series = d as Series;
			if (series != null && !series.IsSeriesTemplate && series.IsManualPointsAdded)
				throw new InvalidOperationException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPointsCreation));
			return value;
		}
		static void DataSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Series series = d as Series;
			if (series != null)
				series.bindingBehavior.UpdateActualDataSource(e.NewValue);
		}
		static void LabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Series series = d as Series;
			if (series != null) {
				SeriesLabel label = e.NewValue as SeriesLabel;
				if (label == null)
					series.SetValue(Series.ActualLabelPropertyKey, new SeriesLabel());
				else
					series.SetValue(Series.ActualLabelPropertyKey, label);
			}
		}
		static void PointOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Series series = d as Series;
			if (series != null) {
				series.pointOptions = (PointOptions)e.NewValue;
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as PointOptions, e.NewValue as PointOptions, series);
				series.UpdateLabelTextPattern();
			}
			ChartElementHelper.UpdateWithClearDiagramCache(d, e);
		}
		static void LegendPointOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Series series = d as Series;
			if (series != null) {
				series.legendPointOptions = (PointOptions)e.NewValue;
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as PointOptions, e.NewValue as PointOptions, series);
				series.UpdateLegendTextPattern();
			}
			ChartElementHelper.UpdateWithClearDiagramCache(d, e);
		}
		static void LegendTextPatternPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Series series = d as Series;
			if (series != null && !series.changeLocker)
				ChartElementHelper.UpdateWithClearDiagramCache(d, e);
		}
		static bool ValidateValueScaleType(object scaleType) {
			return (ScaleType)scaleType != ScaleType.Qualitative;
		}
		static void VisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartElementHelper.Update(d, new PropertyUpdateInfo(d, "Visible"), ChartElementChange.ClearDiagramCache);
		}
		static void CheckedInLegendPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartElementHelper.Update(d, new PropertyUpdateInfo(d, e.Property.GetName()), ChartElementChange.ClearDiagramCache);
		}
		static void ToolTipTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Series series = d as Series;
			if (series != null)
				series.toolTipTemplateChanged = e.NewValue != null;
			ChartElementHelper.Update(d, e);
		}
		static void ToolTipPointTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Series series = d as Series;
			if (series != null)
				series.toolTipPointTemplateChanged = e.NewValue != null;
			ChartElementHelper.Update(d, e);
		}
		static void OnColorizerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Series series = d as Series;
			if (series != null) {
				series.ResetDataSourceScale();
				if (e.OldValue != e.NewValue)
					CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as ChartColorizerBase, e.NewValue as ChartColorizerBase, series);
			}
			ChartElementHelper.UpdateSeriesBindingWithClearDiagramCache(d, e);
		}
		protected static void PointAnimationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Series series = d as Series;
			if (series != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as SeriesPointAnimationBase, e.NewValue as SeriesPointAnimationBase, series);
				series.OnAnimationChanged();
			}
			ChartElementHelper.Update(d, e);
		}
		protected static void SeriesAnimationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Series series = d as Series;
			if (series != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as SeriesAnimationBase, e.NewValue as SeriesAnimationBase, series);
				series.OnAnimationChanged();
			}
			ChartElementHelper.Update(d, e);
		}
		internal static void OnBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Series series = d as Series;
			if (series != null)
				series.ResetDataSourceScale();
			ChartElementHelper.Update(d, e, ChartElementChange.ClearDiagramCache | ChartElementChange.UpdateSeriesBinding);
		}
		readonly SeriesCache cache;
		readonly SeriesItem item;
		readonly PointOptionsContainer pointOptionsContainer;
		readonly LegendPointOptionsContainer legendPointOptionsContainer;
		readonly AnimationProgress seriesProgress;
		readonly SeriesPointCollectionDecorator pointCollectionDecorator;
		SeriesData seriesData;
		PointOptions pointOptions;
		PointOptions legendPointOptions;
		AnimationState animationState = AnimationState.BeforeFirst;
		DataFilter autoSeriesFilter;
		BindingBehavior bindingBehavior;
		ScaleType argumentScaleByCore = ScaleType.Auto;
		ScaleType argumentScaleByDataSource = ScaleType.Auto;
		LegendItem legendItem;
		ColorizerController colorizerController;
		bool isSelected = false;
		bool isHighlighted = false;
		bool isAutoSeries = false;
		bool isAutoPointsAdded = false;
		bool isLoaded = false;
		bool toolTipTemplateChanged;
		bool toolTipPointTemplateChanged;
		bool legendTextPatternAutoCreated = false;
		bool labelTextPatternAutoCreated = false;
		bool storyboardStarted = false;
		bool changeLocker = false;
		ScaleType argumentScaleType = ScaleType.Auto;
		ScaleType valueScaleType = ScaleType.Numerical;
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public SeriesLabel ActualLabel { get { return (SeriesLabel)GetValue(ActualLabelProperty); } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesDataFilters"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)
		]
		public DataFilterCollection DataFilters { get { return (DataFilterCollection)GetValue(DataFiltersProperty); } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesPoints"),
#endif
		Category(Categories.Elements),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)
		]
		public SeriesPointCollection Points { get { return (SeriesPointCollection)GetValue(PointsProperty); } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesArgumentDataMember"),
#endif
		Category(Categories.Data),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string ArgumentDataMember {
			get { return (string)GetValue(ArgumentDataMemberProperty); }
			set { SetValue(ArgumentDataMemberProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesArgumentScaleType"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public ScaleType ArgumentScaleType {
			get { return (ScaleType)GetValue(ArgumentScaleTypeProperty); }
			set { SetValue(ArgumentScaleTypeProperty, value); }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public ScaleType ActualArgumentScaleType {
			get {
				if (ArgumentScaleTypeInternal == ScaleType.Auto)
					return GetActualArgumentScaleType();
				return ArgumentScaleTypeInternal;
			}
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesDataFiltersConjunctionMode"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty
		]
		public ConjunctionTypes DataFiltersConjunctionMode {
			get { return (ConjunctionTypes)GetValue(DataFiltersConjunctionModeProperty); }
			set { SetValue(DataFiltersConjunctionModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesDataSource"),
#endif
		Category(Categories.Data),
		NonTestableProperty
		]
		public object DataSource {
			get { return GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesDisplayName"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty,
		NonTestableProperty
		]
		public string DisplayName {
			get { return (string)GetValue(DisplayNameProperty); }
			set { SetValue(DisplayNameProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesLabel"),
#endif
		Category(Categories.Elements),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public SeriesLabel Label {
			get { return (SeriesLabel)GetValue(LabelProperty); }
			set { SetValue(LabelProperty, value); }
		}
		[
		Obsolete(ObsoleteMessages.PointOptionsProperty),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public PointOptions PointOptions {
			get { return (PointOptions)GetValue(PointOptionsProperty); }
			set { SetValue(PointOptionsProperty, value); }
		}
		[
		Obsolete(ObsoleteMessages.LegendPointOptionsProperty),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public PointOptions LegendPointOptions {
			get { return (PointOptions)GetValue(LegendPointOptionsProperty); }
			set { SetValue(LegendPointOptionsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesLegendTextPattern"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public string LegendTextPattern {
			get { return (string)GetValue(LegendTextPatternProperty); }
			set { SetValue(LegendTextPatternProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesShowInLegend"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool ShowInLegend {
			get { return (bool)GetValue(ShowInLegendProperty); }
			set { SetValue(ShowInLegendProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesValueDataMember"),
#endif
		Category(Categories.Data),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string ValueDataMember {
			get { return (string)GetValue(ValueDataMemberProperty); }
			set { SetValue(ValueDataMemberProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesColorDataMember"),
#endif
		Category(Categories.Data),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string ColorDataMember {
			get { return (string)GetValue(ColorDataMemberProperty); }
			set { SetValue(ColorDataMemberProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesValueScaleType"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public ScaleType ValueScaleType {
			get { return (ScaleType)GetValue(ValueScaleTypeProperty); }
			set { SetValue(ValueScaleTypeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesVisible"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesCheckedInLegend"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool CheckedInLegend {
			get { return (bool)GetValue(CheckedInLegendProperty); }
			set { SetValue(CheckedInLegendProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesCheckableInLegend"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool CheckableInLegend {
			get { return (bool)GetValue(CheckableInLegendProperty); }
			set { SetValue(CheckableInLegendProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesAnimationAutoStartMode"),
#endif
		Category(Categories.Animation),
		XtraSerializableProperty
		]
		public AnimationAutoStartMode AnimationAutoStartMode {
			get { return (AnimationAutoStartMode)GetValue(AnimationAutoStartModeProperty); }
			set { SetValue(AnimationAutoStartModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesLegendMarkerTemplate"),
#endif
		Category(Categories.Presentation)
		]
		public DataTemplate LegendMarkerTemplate {
			get { return (DataTemplate)GetValue(LegendMarkerTemplateProperty); }
			set { SetValue(LegendMarkerTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesToolTipEnabled"),
#endif
		Category(Categories.Behavior),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public bool? ToolTipEnabled {
			get { return (bool?)GetValue(ToolTipEnabledProperty); }
			set { SetValue(ToolTipEnabledProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesToolTipHint"),
#endif
		Category(Categories.Common),
		NonTestableProperty,
		XtraSerializableProperty
		]
		[TypeConverter(typeof(StringToObjectConverter))]
		public object ToolTipHint {
			get { return (object)GetValue(ToolTipHintProperty); }
			set { SetValue(ToolTipHintProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesToolTipSeriesPattern"),
#endif
		Category(Categories.Common),
		XtraSerializableProperty
		]
		public string ToolTipSeriesPattern {
			get { return (string)GetValue(ToolTipSeriesPatternProperty); }
			set { SetValue(ToolTipSeriesPatternProperty, value); }
		}
		[
		Category(Categories.Common)
		]
		public DataTemplate ToolTipSeriesTemplate {
			get { return (DataTemplate)GetValue(ToolTipSeriesTemplateProperty); }
			set { SetValue(ToolTipSeriesTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesToolTipPointPattern"),
#endif
		Category(Categories.Common),
		XtraSerializableProperty
		]
		public string ToolTipPointPattern {
			get { return (string)GetValue(ToolTipPointPatternProperty); }
			set { SetValue(ToolTipPointPatternProperty, value); }
		}
		[
		Category(Categories.Common)
		]
		public DataTemplate ToolTipPointTemplate {
			get { return (DataTemplate)GetValue(ToolTipPointTemplateProperty); }
			set { SetValue(ToolTipPointTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesToolTipHintDataMember"),
#endif
		Category(Categories.Data),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string ToolTipHintDataMember {
			get { return (string)GetValue(ToolTipHintDataMemberProperty); }
			set { SetValue(ToolTipHintDataMemberProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesLabelsVisibility"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool LabelsVisibility {
			get { return (bool)GetValue(LabelsVisibilityProperty); }
			set { SetValue(LabelsVisibilityProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesColorizer"),
#endif
		Category(Categories.Appearance),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public ChartColorizerBase Colorizer {
			get { return (ChartColorizerBase)GetValue(ColorizerProperty); }
			set { SetValue(ColorizerProperty, value); }
		}
		#region Hidden properties
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Brush Background { get { return base.Background; } set { base.Background = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Brush BorderBrush { get { return base.BorderBrush; } set { base.BorderBrush = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Thickness BorderThickness { get { return base.BorderThickness; } set { base.BorderThickness = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Brush Foreground { get { return base.Foreground; } set { base.Foreground = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Thickness Padding { get { return base.Padding; } set { base.Padding = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new double Opacity { get { return base.Opacity; } set { base.Opacity = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Brush OpacityMask { get { return base.OpacityMask; } set { base.OpacityMask = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Effect Effect { get { return base.Effect; } set { base.Effect = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Transform RenderTransform { get { return base.RenderTransform; } set { base.RenderTransform = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Point RenderTransformOrigin { get { return base.RenderTransformOrigin; } set { base.RenderTransformOrigin = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Visibility Visibility { get { return base.Visibility; } set { base.Visibility = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Geometry Clip { get { return base.Clip; } set { base.Clip = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Thickness Margin { get { return base.Margin; } set { base.Margin = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Transform LayoutTransform { get { return base.LayoutTransform; } set { base.LayoutTransform = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Focusable { get { return base.Focusable; } set { base.Focusable = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object ToolTip { get { return base.ToolTip; } set { base.ToolTip = value; } }
		#endregion
		public SeriesItem Item { get { return item; } }
		Storyboard Storyboard {
			get {
				if (!Resources.Contains(StoryboardResourceKey)) {
					Storyboard storyboard = new Storyboard();
					storyboard.Completed += new EventHandler(OnStoryboardCompleted);
					Resources.Add(StoryboardResourceKey, storyboard);
				}
				return (Storyboard)Resources[StoryboardResourceKey];
			}
		}
		bool AnimationEnabled { get { return Chart != null && Chart.AnimationMode != ChartAnimationMode.Disabled; } }
		protected bool IsSelected { get { return isSelected; } }
		protected SeriesData SeriesData {
			get { return seriesData; }
			set { seriesData = value; }
		}
		protected ChartControl Chart { get { return Diagram != null ? Diagram.ChartControl : null; } }
		protected abstract Type PointInterfaceType { get; }
		protected abstract bool Is3DView { get; }
		protected abstract CompatibleViewType CompatibleViewType { get; }
		protected virtual bool NeedFilterVisiblePoints { get { return false; } }
		protected virtual bool NeedSeriesInteraction { get { return false; } }
		protected virtual bool NeedSeriesGroupsInteraction { get { return false; } }
		protected virtual LineStyle LegendMarkerLineStyle { get { return new LineStyle(1); } }
		protected virtual int PointDimension { get { return 1; } }
		protected virtual string DefaultLegendTextPattern { get { return "{" + PatternUtils.ValuePlaceholder + "}"; } }
		protected virtual string DefaultLabelTextPattern { get { return "{" + PatternUtils.ValuePlaceholder + "}"; } }
		protected virtual bool ShouldSortPoints { get { return true; } }
		internal SeriesCache Cache { get { return cache; } }
		internal bool ActualToolTipEnabled {
			get {
				if (ToolTipEnabled.HasValue)
					return ToolTipEnabled.Value;
				if (Chart != null) {
					if (Chart.ToolTipEnabled.HasValue)
						return Chart.ToolTipEnabled.Value;
					return !Chart.CrosshairSupported;
				}
				return false;
			}
		}
		internal PointOptions PointOptionsInternal { get { return pointOptions; } }
		internal PointOptions LegendPointOptionsInternal { get { return legendPointOptions; } }
		internal PointOptionsContainer PointOptionsContainer { get { return pointOptionsContainer; } }
		internal LegendPointOptionsContainer LegendPointOptionsContainer { get { return legendPointOptionsContainer; } }
		internal AnimationProgress SeriesProgress { get { return seriesProgress; } }
		internal Diagram Diagram { get { return ((IOwnedElement)this).Owner as Diagram; } }
		internal ColorizerController ColorizerController { get { return colorizerController; } }
		internal bool IsAutoSeries { get { return isAutoSeries; } }
		internal bool IsAutoPointsAdded { get { return isAutoPointsAdded; } }
		internal bool IsManualPointsAdded { get { return Points.Count > 0 && !isAutoPointsAdded; } }
		internal bool IsSeriesTemplate { get { return Diagram != null && Object.ReferenceEquals(this, Diagram.SeriesTemplate); } }
		internal bool ToolTipTemplateChanged { get { return toolTipTemplateChanged; } }
		internal bool ToolTipPointTemplateChanged { get { return toolTipPointTemplateChanged; } }
		internal bool ShouldCalculatePointsData { get { return Chart != null && (Chart.ShouldRaiseCustomDrawSeriesPointEvent || ArePointsVisible || ActualColorEach || LabelsVisibility || AnimationEnabled); } }
		internal ScaleType ArgumentScaleTypeInternal { get { return argumentScaleType; } set { argumentScaleType = value; } }
		internal ScaleType ValueScaleTypeInternal { get { return valueScaleType; } set { valueScaleType = value; } }
		internal string ActualToolTipPointPattern {
			get { return ToolTipPointPattern != "" ? ToolTipPointPattern : ToolTipPointValuesConverter.DefaulPointPattern; }
		}
		internal string ActualLegendTextPattern {
			get {
				if (legendTextPatternAutoCreated) {
					UpdateLegendTextPattern();
				}
				return string.IsNullOrEmpty(LegendTextPattern) ? DefaultLegendTextPattern : LegendTextPattern;
			}
		}
		internal string ActualLabelTextPattern {
			get {
				if (labelTextPatternAutoCreated) {
					UpdateLabelTextPattern();
				}
				return string.IsNullOrEmpty(ActualLabel.TextPattern) ? DefaultLabelTextPattern : ActualLabel.TextPattern;
			}
		}
		protected internal Palette Palette {
			get {
				ChartControl chartControl = ChartElementHelper.GetOwner<ChartControl>(this);
				if (chartControl == null)
					return null;
				Palette palette = chartControl.Palette;
				return palette == null ? new OfficePalette() : palette;
			}
		}
		protected internal abstract bool ActualColorEach { get; }
		protected internal abstract Type SupportedDiagramType { get; }
		protected internal virtual VisualSelectionType SupportedSelectionType { get { return VisualSelectionType.None; } }
		protected internal virtual VisualSelectionType SupportedLegendMarkerSelectionType { get { return SupportedSelectionType; } }
		protected internal virtual bool ActualCrosshairEnabled {
			get {
				return false;
			}
		}
		protected internal virtual bool IsLabelConnectorItemVisible { get { return false; } }
		protected internal virtual FadeInMode AutoFadeInMode { get { return FadeInMode.PointsAndLabels; } }
		protected internal virtual bool LabelConnectorSupported { get { return true; } }
		protected internal virtual bool LabelsResolveOverlappingSupported { get { return false; } }
		protected internal virtual double SeriesDepth { get { return 0.6; } }
		protected internal virtual bool ArePointsVisible { get { return false; } }
		protected internal virtual ToolTipPointDataToStringConverter ToolTipPointValuesConverter { get { return new ToolTipValueToStringConverter(this); } }
		protected internal virtual Color BaseColor {
			get {
				Palette palette = Palette;
				return palette == null ? Colors.Black : palette[Diagram.ViewController.RefinedSeriesForLegend.IndexOf((RefinedSeries)Diagram.ViewController.GetRefinedSeries(this))];
			}
		}
		protected internal virtual bool InFrontOfAxes { get { return false; } }
		protected internal virtual string DefaultPointToolTipPattern {
			get {
				string argumentPattern = "{A" + GetDefaultArgumentFormat() + "}";
				string valuePattern = " : " + "{V" + GetDefaultFormat(ValueScaleTypeInternal) + "}";
				return argumentPattern + valuePattern;
			}
		}
		public Series()
			: base() {
			BeginInit();
			cache = new SeriesCache(this);
			item = new SeriesItem(this);
			pointOptionsContainer = new PointOptionsContainer(this);
			legendPointOptionsContainer = new LegendPointOptionsContainer(this);
			seriesProgress = new AnimationProgress(this);
			this.SetValue(ActualLabelPropertyKey, new SeriesLabel());
			ActualLabel.ChangeOwner(null, this);
			this.SetValue(PointsPropertyKey, ChartElementHelper.CreateInstance<SeriesPointCollection>(this));
			pointCollectionDecorator = new SeriesPointCollectionDecorator(Points);
			this.SetValue(DataFiltersPropertyKey, ChartElementHelper.CreateInstance<DataFilterCollection>(this));
			colorizerController = new ColorizerController(this);
			bindingBehavior = new BindingBehavior();
			bindingBehavior.ActualDataSourceChanged += ActualDataSourceChanged;
			Loaded += new RoutedEventHandler(OnLoaded);
			Points.CollectionChanged += Points_CollectionChanged;
			EndInit();
		}
		#region ISeriesBase implementation
		Scale ISeriesBase.ArgumentScaleType { get { return (Scale)ActualArgumentScaleType; } }
		Scale ISeriesBase.UserArgumentScaleType {
			get {
				if (ArgumentScaleTypeInternal == ScaleType.Auto)
					return (Scale)argumentScaleByDataSource;
				return (Scale)ArgumentScaleTypeInternal;
			}
		}
		Scale ISeriesBase.ValueScaleType { get { return (Scale)ValueScaleTypeInternal; } }
		ISeriesView ISeriesBase.SeriesView { get { return this; } }
		#endregion
		#region ISeries implementation
		string ISeries.Name { get { return DisplayName; } }
		IEnumerable<ISeriesPoint> ISeries.Points { get { return Points; } }
		IList<ISeriesPoint> ISeries.ActualPoints { get { return pointCollectionDecorator; } }
		IList<ISeriesPoint> ISeries.PointsToInsert { get { return null; } }
		IList<ISeriesPoint> ISeries.PointsToRemove { get { return null; } }
		bool ISeries.ArePointsSorted { get { return false; } }
		bool ISeries.ShouldSortPointsInfo { get { return ShouldSortPoints; } }
		bool ISeries.Visible { get { return this.Visible; } }
		bool ISeries.LabelsVisibility { get { return this.LabelsVisibility; } }
		bool ISeries.ShouldBeDrawnOnDiagram {
			get {
				bool useLegendCheckBox = Chart != null && Chart.Legend != null ? Chart.Legend.UseCheckBoxes && CheckableInLegend : false;
				return useLegendCheckBox ? Visible && CheckedInLegend : Visible;
			}
		}
		IList<string> ISeries.ValueDataMembers { get { return GetValueDataMembers(); } }
		IEnumerable<IDataFilter> ISeries.DataFilters {
			get {
				List<IDataFilter> dataFilters = new List<IDataFilter>();
				if (autoSeriesFilter != null)
					dataFilters.Add(autoSeriesFilter);
				foreach (DataFilter filter in DataFilters)
					if (filter != null)
						dataFilters.Add(filter);
				return dataFilters;
			}
		}
		Conjunction ISeries.DataFiltersConjunction { get { return (Conjunction)DataFiltersConjunctionMode; } }
		SeriesPointKeyNative ISeries.SeriesPointsSortingKey { get { return SeriesPointKeyNative.Argument; } }
		SortMode ISeries.SeriesPointsSortingMode { get { return SortMode.None; } }
		void ISeries.SetArgumentScaleType(Scale scale) {
			argumentScaleByCore = (ScaleType)scale;
		}
		ISeriesPoint ISeriesPointFactory.CreateSeriesPoint(object argument) {
			return null;
		}
		ISeriesPoint ISeriesPointFactory.CreateSeriesPoint(ISeries owner, object argument, object[] values, object tag, object[] colors) {
			ISeriesPoint point = values == null ? new SeriesPoint(argument, 0.0, Double.NaN, DateTime.MinValue, null, tag, string.Empty) :
				CreateSeriesPoint(argument, 0.0, values, null, tag, string.Empty, colors.Length > 0 ? colors[0] : null);
			((IOwnedElement)point).Owner = owner as IChartElement;
			return point;
		}
		ISeriesPoint ISeriesPointFactory.CreateSeriesPoint(ISeries owner, object argument, double internalArgument, object[] values, double[] internalValues, object tag) {
			return ((ISeriesPointFactory)this).CreateSeriesPoint(owner, argument, internalArgument, values, internalValues, tag, null);
		}
		ISeriesPoint ISeriesPointFactory.CreateSeriesPoint(ISeries owner, object argument, double internalArgument, object[] values, double[] internalValues, object tag, object hint) {
			ISeriesPoint point = values == null ? new SeriesPoint(argument, internalArgument, Double.NaN, DateTime.MinValue, null, tag, hint) :
				CreateSeriesPoint(argument, internalArgument, values, internalValues, tag, hint, null);
			((IOwnedElement)point).Owner = owner as IChartElement;
			return point;
		}
		ISeriesPoint ISeriesPointFactory.CreateSeriesPoint(ISeries owner, object argument, double internalArgument, object[] values, double[] internalValues, object tag, object hint, object color) {
			ISeriesPoint point = values == null ? new SeriesPoint(argument, internalArgument, Double.NaN, DateTime.MinValue, null, tag, hint) :
				CreateSeriesPoint(argument, internalArgument, values, internalValues, tag, hint, color);
			((IOwnedElement)point).Owner = owner as IChartElement;
			return point;
		}
		void ISeries.AddSeriesPoint(ISeriesPoint point) {
			SeriesPoint pointItem = point as SeriesPoint;
			if (pointItem != null)
				Points.Add(pointItem);
		}
		void ISeries.ClearColorCache() {
			ColorizerController.ClearBrushCache();
		}
		#endregion
		#region ISeriesTemplate implementation
		ISeries ISeriesTemplate.CreateSeriesForBinding(string seriesName, object seriesValue) {
			Series newSeries = null;
			newSeries = CreateCopyForBinding();
			newSeries.DisplayName = seriesName;
			return newSeries;
		}
		String ISeriesTemplate.ArgumentDataMember { get { return this.ArgumentDataMember; } }
		String ISeriesTemplate.ToolTipHintDataMember { get { return this.ToolTipHintDataMember; } }
		IList<string> ISeriesTemplate.ValueDataMembers { get { return GetValueDataMembers(); } }
		#endregion
		#region ISeriesView implementation
		bool ISeriesView.NeedSeriesInteraction {
			get { return NeedSeriesInteraction; }
		}
		bool ISeriesView.NeedSeriesGroupsInteraction {
			get { return NeedSeriesGroupsInteraction; }
		}
		bool ISeriesView.ShouldSortPoints {
			get { return ShouldSortPoints; }
		}
		Type ISeriesView.PointInterfaceType {
			get { return PointInterfaceType; }
		}
		CompatibleViewType ISeriesView.CompatibleViewType {
			get { return CompatibleViewType; }
		}
		bool ISeriesView.Is3DView {
			get { return Is3DView; }
		}
		bool ISeriesView.NeedFilterVisiblePoints {
			get { return NeedFilterVisiblePoints; }
		}
		MinMaxValues ISeriesView.CalculateMinMaxPointRangeValues(CrosshairSeriesPointEx point, double range, bool isHorizontalCrosshair, IXYDiagram diagram, CrosshairPaneInfoEx crosshairPaneInfo, CrosshairSnapModeCore snapMode) {
			return CalculateMinMaxPointRangeValues(point, range, isHorizontalCrosshair, diagram, crosshairPaneInfo, snapMode);
		}
		SeriesContainer ISeriesView.CreateContainer() {
			return CreateContainer();
		}
		SeriesInteractionContainer ISeriesView.CreateSeriesGroupsContainer() {
			return CreateSeriesGroupsContainer();
		}
		IList<ISeriesPoint> ISeriesView.GenerateRandomPoints(Scale argumentScaleType, Scale valueScaleType) {
			return null;
		}
		RangeValue ISeriesView.GetMinMax(IPointInteraction interaction, int index) {
			return RangeValue.Empty;
		}
		double ISeriesView.GetRefinedPointAbsMin(RefinedPoint refinedPoint) {
			return GetRefinedPointAbsMin(refinedPoint);
		}
		double ISeriesView.GetRefinedPointMax(RefinedPoint point) {
			return GetRefinedPointMax(point);
		}
		double ISeriesView.GetRefinedPointMin(RefinedPoint point) {
			return GetRefinedPointMin(point);
		}
		double ISeriesView.GetRefinedPointsMax(IList<RefinedPoint> points) {
			double max = double.MinValue;
			foreach (RefinedPoint point in points)
				max = Math.Max(max, ((ISeriesView)this).GetRefinedPointMin(point));
			return max;
		}
		double ISeriesView.GetRefinedPointsMin(IList<RefinedPoint> points) {
			double min = double.MaxValue;
			for (int i = 0; i < points.Count; i++)
				min = Math.Min(min, ((ISeriesView)this).GetRefinedPointMin(points[i]));
			return min;
		}
		bool ISeriesView.IsCorrectValueLevel(ValueLevelInternal valueLevel) {
			return IsCorrectValueLevel(valueLevel);
		}
		#endregion
		#region ILegendVisible implementation
		DataTemplate ILegendVisible.LegendMarkerTemplate { get { return LegendMarkerTemplate; } }
		bool ILegendVisible.CheckedInLegend { get { return CheckedInLegend; } }
		bool ILegendVisible.CheckableInLegend { get { return CheckableInLegend; } }
		#endregion
		#region IWeakEventListener implementation
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			return PerformWeakEvent(managerType, sender, e);
		}
		#endregion
		#region IAnimatableElement implementation
		ChartAnimationMode IAnimatableElement.AnimationMode { get { return Chart != null ? Chart.AnimationMode : ChartAnimationMode.Disabled; } }
		AnimationState IAnimatableElement.AnimationState { get { return animationState; } }
		AnimationAutoStartMode IAnimatableElement.AnimationAutoStartMode { get { return AnimationAutoStartMode; } }
		void IAnimatableElement.ProgressChanged(AnimationProgress source) {
			SeriesProgressChanged();
		}
		#endregion
		#region IWizardDataProvider implementation
		Type IWizardDataProvider.PointInterfaceType {
			get { return this.PointInterfaceType; }
		}
		#endregion
		#region IInteractiveElement implementation
		bool IInteractiveElement.IsHighlighted {
			get { return isHighlighted; }
			set {
				if (isHighlighted != value) {
					isHighlighted = value;
					OnIsHighlightedChanged();
				}
			}
		}
		bool IInteractiveElement.IsSelected {
			get { return isSelected; }
			set {
				if (isSelected != value) {
					isSelected = value;
					OnIsSelectedChanged(isSelected);
				}
			}
		}
		object IInteractiveElement.Content {
			get { return this; }
		}
		void OnIsHighlightedChanged() {
		}
		protected virtual void OnIsSelectedChanged(bool isSelected) {
			if (legendItem != null)
				legendItem.IsSelected = isSelected;
			SetPointsSelection(isSelected);
		}
		#endregion
		#region IPatternHolder
		PatternDataProvider IPatternHolder.GetDataProvider(PatternConstants patternConstant) {
			return GetDataProvider(patternConstant);
		}
		string IPatternHolder.PointPattern {
			get {
				if (!string.IsNullOrEmpty(this.ToolTipPointPattern))
					return this.ToolTipPointPattern;
				return DefaultPointToolTipPattern;
			}
		}
		#endregion
		void Points_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (Chart == null)
				return;
			if (Chart.AnimationMode == ChartAnimationMode.OnDataChanged)
				Chart.Animate();
			if (Chart.SelectedItem != null)
				Chart.UpdateSelection();
		}
		void OnLoaded(object sender, EventArgs e) {
			isLoaded = true;
			if (DesignerProperties.GetIsInDesignMode(this) || AnimationAutoStartMode == AnimationAutoStartMode.PlayOnce)
				Animate();
		}
		void OnStoryboardCompleted(object sender, EventArgs e) {
			ClockGroup clockGroup = sender as ClockGroup;
			if (clockGroup != null) {
				ClockGroup childClockGroup = clockGroup.Children.Count > 0 ? clockGroup.Children[0] as ClockGroup : null;
				DoubleAnimation animation = childClockGroup != null && childClockGroup.Children.Count > 0 ?
					childClockGroup.Children[0].Timeline as DoubleAnimation : null;
				if (animation != null && !AnimationHelper.ContainsAnimation(Storyboard, animation))
					return;
			}
			FinishAnimation();
		}
		LegendItem CreateLegendItem(object sourceElement, Color color, string text, bool isSelected) {
			color.A = (byte)(color.A * GetOpacity());
			SolidColorBrush brush = new SolidColorBrush(color);
			return new LegendItem(this, sourceElement, text, brush, GetPenBrush(brush), LegendMarkerLineStyle, SupportedLegendMarkerSelectionType, true) { IsSelected = isSelected };
		}
		void UpdateLegendItemsForSeries(ICollection<LegendItem> items, ISeriesItem seriesItem) {
			string text = seriesItem.LegendText;
			if (String.IsNullOrEmpty(text)) {
				if (!DesignerProperties.GetIsInDesignMode(this))
					return;
				text = ChartLocalizer.GetString(ChartStringId.EmptyLegendText);
			}
			LegendItem legendItem = CreateLegendItem(this, seriesItem.DrawOptions.Color, text, isSelected);
			this.legendItem = legendItem;
			items.Add(legendItem);
		}
		void UpdateLegendItemsForSeriesPoints(ICollection<LegendItem> items, ISeriesItem seriesItem) {
			foreach (ISeriesPointData pointData in seriesItem.PointData) {
				string text = pointData.LegendText;
				if (String.IsNullOrEmpty(text)) {
					if (!DesignerProperties.GetIsInDesignMode(this) ||
						(PointOptionsHelper.ConvertToPointView(ActualLegendTextPattern) != PointViewKind.SeriesName))
						continue;
					text = ChartLocalizer.GetString(ChartStringId.EmptyLegendText);
				}
				SeriesPoint seriesPoint = SeriesPoint.GetSeriesPoint(pointData.SeriesPoint);
				LegendItem legendItem = CreateLegendItem(pointData.RefinedPoint, pointData.Color, text, ((IInteractiveElement)seriesPoint).IsSelected);
				pointData.LegendItem = legendItem;
				items.Add(legendItem);
			}
		}
		void ActualDataSourceChanged() {
			if (Diagram != null && Diagram.Series.Contains(this)) {
				ResetDataSourceScale();
				UpdateSeriesBinding(true);
			}
			if (Chart != null && Chart.AnimationMode == ChartAnimationMode.OnDataChanged)
				Chart.Animate();
		}
		IEnumerable<AnimationProgress> GetAnimationProgess() {
			yield return seriesProgress;
			foreach (SeriesPointData seriesPointData in Item.SeriesPointDataList) {
				yield return seriesPointData.PointProgress;
				yield return seriesPointData.LabelProgress;
			}
		}
		void SetPointsSelection(bool isSelected) {
			foreach (SeriesPoint point in Points) {
				((IInteractiveElement)point).IsSelected = isSelected;
			}
		}
		void PrepareStoryboard() {
			SeriesPointAnimationBase pointAnimation = GetActualPointAnimation();
			if (pointAnimation != null)
				pointAnimation.PrepareSeriesStoryboard(Storyboard, this);
			SeriesAnimationBase seriesAnimation = GetActualSeriesAnimation();
			if (seriesAnimation != null)
				seriesAnimation.PrepareSeriesStoryboard(Storyboard, this);
		}
		void StartAnimation() {
			animationState = AnimationState.InProgress;
			foreach (AnimationProgress progress in GetAnimationProgess())
				progress.Start();
		}
		void FinishAnimation() {
			animationState = AnimationState.Completed;
			foreach (AnimationProgress progress in GetAnimationProgess())
				progress.Finish();
		}
		ScaleType GetActualArgumentScaleType() {
			if (argumentScaleByDataSource == ScaleType.Auto) {
				object dataSource = GetDataSource();
				if (dataSource != null && !string.IsNullOrEmpty(ArgumentDataMember))
					argumentScaleByDataSource = (ScaleType)BindingHelperCore.GetScaleType(dataSource, ArgumentDataMember);
			}
			if (argumentScaleByDataSource != ScaleType.Auto)
				return argumentScaleByDataSource;
			if (IsSeriesTemplate)
				return ScaleType.Numerical;
			return argumentScaleByCore;
		}
		protected PatternDataProvider GetXYDataProvider(PatternConstants patternConstant) {
			IXYSeriesView xyView = ((IXYSeriesView)this);
			IAxisData axisXData = xyView.AxisXData;
			IAxisData axisYData = xyView.AxisYData;
			switch (patternConstant) {
				case PatternConstants.Argument:
					return new PointPatternDataProvider(patternConstant, axisXData != null ? axisXData.AxisScaleTypeMap : null);
				case PatternConstants.Value:
				case PatternConstants.PercentValue:
				case PatternConstants.Value1:
				case PatternConstants.Value2:
				case PatternConstants.OpenValue:
				case PatternConstants.CloseValue:
				case PatternConstants.HighValue:
				case PatternConstants.LowValue:
				case PatternConstants.Weight:
				case PatternConstants.PointHint:
				case PatternConstants.ValueDuration:
					return new PointPatternDataProvider(patternConstant, axisYData != null ? axisYData.AxisScaleTypeMap : null);
				case PatternConstants.Series:
				case PatternConstants.SeriesGroup:
					return new SeriesPatternDataProvider(patternConstant);
			}
			return null;
		}
		protected abstract PatternDataProvider GetDataProvider(PatternConstants patternConstant);
		protected abstract bool IsCorrectValueLevel(ValueLevelInternal valueLevel);
		protected string GetDefaultFormat(ScaleType scaleType) {
			if (scaleType == ScaleType.DateTime)
				return ":d";
			return "";
		}
		protected string GetDefaultArgumentFormat() {
			IXYSeriesView view = this as IXYSeriesView;
			if (view != null && view.AxisXData != null && ArgumentScaleTypeInternal == ScaleType.DateTime && view.AxisXData.Label != null)
				return ":" + PatternUtils.GetArgumentFormat(view.AxisXData.Label.TextPattern);
			return GetDefaultFormat(ArgumentScaleTypeInternal);
		}
		internal bool GetActualVisible() {
			if (Visible) {
				if (Chart != null && Chart.LegendUseCheckBoxes)
					return !CheckableInLegend || CheckedInLegend;
				return true;
			}
			return false;
		}
		internal bool AreValueDataMembersSetted() {
			foreach (string dataMember in GetValueDataMembers())
				if (String.IsNullOrEmpty(dataMember))
					return false;
			return true;
		}
		internal Series CopyPropertiesForBinding(Series source) {
			try {
				ChangedLocker.Lock();
				AssignForBinding(source);
			}
			finally {
				ChangedLocker.Unlock();
			}
			return this;
		}
		internal Series CreateCopyForBinding() {
			Series seriesCopy = CreateObjectForClone();
			seriesCopy.CopyPropertiesForBinding(this);
			seriesCopy.isAutoSeries = true;
			return seriesCopy;
		}
		internal string GetPointLegendText(RefinedPoint point) {
			if (ActualColorEach && ShowInLegend) {
				PatternParser patternParser = new PatternParser(ActualLegendTextPattern, this);
				patternParser.SetContext(point, this);
				return patternParser.GetText();
			}
			else
				return String.Empty;
		}
		internal string GetSeriesLegendText() {
			return ShowInLegend && (Chart.LegendUseCheckBoxes || !ActualColorEach) ? DisplayName : String.Empty;
		}
		internal void SetAutoSeriesFilter(DataFilter filter) {
			autoSeriesFilter = filter;
		}
		internal void UpdateDataFilters() {
			foreach (DataFilter filter in DataFilters)
				if (filter.ActualType == null)
					filter.UpdateActualParams();
		}
		internal void UpdateLegendItems(ICollection<LegendItem> items, ISeriesItem seriesItem) {
			if (ShowInLegend) {
				if (Colorizer is ILegendItemsProvider && ((ILegendItemsProvider)Colorizer).ShowInLegend) {
					IList<LegendItem> legendItems = colorizerController.GetLegendItems();
					foreach (LegendItem item in legendItems)
						items.Add(item);
				}
				else if (ActualColorEach && seriesItem.HasPoints && !Chart.LegendUseCheckBoxes)
					UpdateLegendItemsForSeriesPoints(items, seriesItem);
				else
					UpdateLegendItemsForSeries(items, seriesItem);
			}
		}
		internal bool UpdateSeriesBinding(object dataSource) {
			if (isAutoPointsAdded) {
				Points.BeginInit();
				isAutoPointsAdded = false;
				Points.Clear();
				Points.EndInit(false);
				colorizerController.ClearBrushCache();
			}
			if (dataSource != null && !String.IsNullOrEmpty(ArgumentDataMember) && AreValueDataMembersSetted()) {
				if (IsManualPointsAdded)
					throw new InvalidOperationException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPointsCreation));
				UpdateDataFilters();
				SimpleSeriesBindingProcedure bindingProcedure = new SimpleSeriesBindingProcedure(this, null, dataSource, String.Empty);
				IList<ISeriesPoint> points = bindingProcedure.CreateBindingPoints();
				Points.BeginInit();
				Points.AddRange(points);
				Points.EndInit(false);
				CompleteBinding();
			}
			return isAutoPointsAdded;
		}
		internal void CompleteBinding() {
			if (Points.Count > 0)
				isAutoPointsAdded = true;
		}
		internal bool UpdateSeriesBinding(bool raiseEvent) {
			bool dataWasChanged = UpdateSeriesBinding(GetDataSource());
			if (dataWasChanged && raiseEvent && Diagram != null)
				Diagram.RaiseBoundDataChangedEvent();
			return dataWasChanged;
		}
		internal SeriesPointAnimationBase GetActualPointAnimation() {
			if (GetPointAnimation() != null)
				return GetPointAnimation();
			if (AnimationEnabled) {
				SeriesPointAnimationBase defaultPointAnimation = CreateDefaultPointAnimation();
				AnimationHelper.PrepareDefaultPointAnimation(defaultPointAnimation, CreateDefaultSeriesAnimation(), this, Diagram);
				return defaultPointAnimation;
			}
			return null;
		}
		internal SeriesAnimationBase GetActualSeriesAnimation() {
			if (GetSeriesAnimation() != null)
				return GetSeriesAnimation();
			if (AnimationEnabled) {
				SeriesAnimationBase defaultSeriesAnimation = CreateDefaultSeriesAnimation();
				AnimationHelper.PrepareDefaultSeriesAnimation(defaultSeriesAnimation, this, Diagram);
				return defaultSeriesAnimation;
			}
			return null;
		}
		internal void OnAnimationChanged() {
			if (isLoaded) {
				bool shouldAnimate = DesignerProperties.GetIsInDesignMode(this) || animationState == AnimationState.InProgress;
				FinishAnimation();
				if (shouldAnimate)
					Animate();
			}
		}
		internal void UpdatePointsSelection() {
			if (isSelected)
				SetPointsSelection(isSelected);
		}
		protected internal virtual List<SeriesPointItem> GetPointItemsForLabels() {
			List<SeriesPointItem> pointItems = new List<SeriesPointItem>();
			if (LabelsVisibility)
				foreach (SeriesPointItem pointItem in Item.AllPointItems)
					if (!string.IsNullOrEmpty(pointItem.PresentationData.LabelText))
						pointItems.Add(pointItem);
			return pointItems;
		}
		protected virtual Color GetSeriesPointColor(RefinedPoint refinedPoint, bool isSelected) {
			SeriesPointCache seriesPointCache = cache.GetSeriesPointCache(refinedPoint.SeriesPoint);
			if (seriesPointCache != null)
				return isSelected ? VisualSelectionHelper.GetSelectedPointColor(seriesPointCache.Color) : seriesPointCache.Color;
			ChartDebug.Fail("SeriesPointCache cant't be null.");
			return cache.DrawOptions.Color;
		}
		internal double GetOpacity() {
			double opacity = 1.0;
			ISupportTransparency supportTransparency = this as ISupportTransparency;
			if (supportTransparency != null)
				opacity = 1.0 - supportTransparency.Transparency;
			return opacity;
		}
		internal Color GetSeriesPointColor(RefinedPoint refinedPoint) {
			IInteractiveElement seriesPoint = SeriesPoint.GetSeriesPoint(refinedPoint.SeriesPoint);
			return GetSeriesPointColor(refinedPoint, seriesPoint.IsSelected);
		}
		internal Color GetPointLabelColor(RefinedPoint refinedPoint) {
			return GetSeriesPointColor(refinedPoint, false);
		}
		protected internal virtual SeriesData CreateSeriesData() {
			return null;
		}
		protected internal virtual CrosshairMarkerItem CreateCrosshairMarkerItem(IRefinedSeries refinedSeries, RefinedPoint refinedPoint) {
			return null;
		}
		public object GetDataSource() {
			if (bindingBehavior.ActualDataSource != null)
				return bindingBehavior.ActualDataSource;
			if (Diagram != null && Diagram.ChartControl != null)
				return Diagram.ChartControl.ActualDataSource;
			return null;
		}
		public void Animate() {
			if (storyboardStarted)
				Storyboard.Stop();
			if (Storyboard.Children.Count != 0)
				Storyboard.Children.Clear();
			PrepareStoryboard();
			StartAnimation();
			var action = new Action(delegate {
				Storyboard.Begin();
				storyboardStarted = true;
			});
			Dispatcher.BeginInvoke(action, DispatcherPriority.Loaded);
		}
		public IEnumerable<AnimationKind> GetPredefinedPointAnimationKinds() {
			List<AnimationKind> animationKinds = new List<AnimationKind>();
			FillPredefinedPointAnimationKinds(animationKinds);
			return animationKinds;
		}
		public IEnumerable<AnimationKind> GetPredefinedSeriesAnimationKinds() {
			List<AnimationKind> animationKinds = new List<AnimationKind>();
			FillPredefinedSeriesAnimationKinds(animationKinds);
			return animationKinds;
		}
		public override void EndInit() {
			bool flag = false;
			if (Diagram != null && Diagram.Series.Contains(this)) {
				flag = true;
				Diagram.ViewController.BeginUpdateData();
				UpdateSeriesBinding(true);
			}
			base.EndInit();
			if (flag)
				Diagram.ViewController.EndUpdateData();
			else
				ChartElementHelper.Update((IChartElement)this, ChartElementChange.ClearDiagramCache);
		}
		public virtual SeriesPointAnimationBase GetPointAnimation() {
			return null;
		}
		public virtual void SetPointAnimation(SeriesPointAnimationBase value) {
		}
		public virtual SeriesAnimationBase GetSeriesAnimation() {
			return null;
		}
		public virtual void SetSeriesAnimation(SeriesAnimationBase value) {
		}
		protected abstract Series CreateObjectForClone();
		protected abstract void FillPredefinedPointAnimationKinds(List<AnimationKind> animationKinds);
		protected virtual bool PerformWeakEvent(Type managerType, object sender, EventArgs e) {
			bool success = false;
			if (managerType == typeof(PropertyChangedWeakEventManager)) {
				if (sender is AnimationBase) {
					OnAnimationChanged();
					success = true;
				}
				else if (sender is PointOptions) {
					if (sender == LegendPointOptionsContainer.PointOptions)
						UpdateLegendTextPattern();
					if (sender == PointOptionsContainer.PointOptions)
						UpdateLabelTextPattern();
					ChartElementHelper.UpdateWithClearDiagramCache(this);
					success = true;
				}
				else if (sender is ChartColorizerBase) {
					ResetDataSourceScale();
					ChartElementHelper.Update((DependencyObject)this, ChartElementChange.ClearDiagramCache | ChartElementChange.UpdateSeriesBinding);
					success = true;
				}
			}
			return success;
		}
		protected virtual bool IsCompatible(Series series) {
			return SupportedDiagramType.Equals(series.SupportedDiagramType);
		}
		protected virtual void AssignForBinding(Series series) {
			if (series != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, series, TemplateProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, series, StyleProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, series, VisibleProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, series, CheckedInLegendProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, series, CheckableInLegendProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, series, ShowInLegendProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, series, ArgumentDataMemberProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, series, ValueDataMemberProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, series, ColorDataMemberProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, series, ArgumentScaleTypeProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, series, ValueScaleTypeProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, series, AnimationAutoStartModeProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, series, DataFiltersConjunctionModeProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, series, LegendMarkerTemplateProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, series, ToolTipEnabledProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, series, ToolTipSeriesPatternProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, series, ToolTipSeriesTemplateProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, series, ToolTipPointPatternProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, series, ToolTipPointTemplateProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, series, ToolTipHintDataMemberProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, series, LabelsVisibilityProperty);
				CopyPropertyValueHelper.CopyPropertyValueByRef(this, series, ColorizerProperty);
				if (series.DataFilters != null && series.DataFilters.Count > 0) {
					DataFilters.Clear();
					foreach (DataFilter dataFilter in series.DataFilters) {
						DataFilter newDataFilter = dataFilter.CloneForBinding();
						DataFilters.Add(newDataFilter);
					}
				}
				if (CopyPropertyValueHelper.IsValueSet(series, LabelProperty)) {
					if (CopyPropertyValueHelper.VerifyValues(this, series, LabelProperty)) {
						Label = new SeriesLabel();
						Label.Assign(series.Label);
					}
				}
				else {
					this.SetValue(ActualLabelPropertyKey, new SeriesLabel());
					ActualLabel.Assign(series.ActualLabel);
				}
				argumentScaleByDataSource = series.argumentScaleByDataSource;
				argumentScaleByCore = series.argumentScaleByCore;
				LegendTextPattern = series.LegendTextPattern;
			}
		}
		protected virtual ISeriesPoint CreateSeriesPoint(object argument, double internalArgument, object[] values, double[] internalValues, object tag, object hint, object color) {
			double value = Double.NaN;
			DateTime dateTimeValue = DateTime.MinValue;
			SolidColorBrush pointBrush = ColorizerController.GetSeriesPointBrush(argument, values, color);
			if (values[0] is double)
				value = (double)values[0];
			else if (values[0] is DateTime)
				dateTimeValue = (DateTime)values[0];
			return new SeriesPoint(argument, internalArgument, value, dateTimeValue, internalValues, tag, hint, pointBrush);
		}
		protected virtual IList<string> GetValueDataMembers() {
			return new string[] { ValueDataMember };
		}
		protected virtual void SeriesProgressChanged() {
		}
		protected virtual void FillPredefinedSeriesAnimationKinds(List<AnimationKind> animationKinds) {
		}
		protected internal void ResetDataSourceScale() {
			argumentScaleByDataSource = ScaleType.Auto;
		}
		protected override bool ProcessChanging(ChartUpdate updateInfo) {
			if (IsSeriesTemplate) {
				if (updateInfo.ShouldUpdateSeriesBinding)
					updateInfo.Change |= ChartElementChange.UpdateAutoSeries;
				else
					updateInfo.Change |= ChartElementChange.UpdateAutoSeriesProperties;
			}
			else {
				if (!isAutoSeries && updateInfo.ShouldUpdateSeriesBinding) {
					if (Diagram != null && Diagram.Series.Contains(this))
						UpdateSeriesBinding(true);
				}
			}
			return true;
		}
		protected internal virtual PointModel GetModel(RangeValueLevel valueLevel) {
			return null;
		}
		protected internal virtual void ChangeSeriesPointSelection(SeriesPoint seriesPoint, bool isSelected) {
			List<SeriesPointItem> pointItems = Item.GetPointItems(seriesPoint);
			if (pointItems != null)
				foreach (SeriesPointItem item in pointItems) {
					item.IsSelected = isSelected;
				}
		}
		protected internal virtual bool IsPointValueVisible(RangeValueLevel valueLevel) {
			return ArePointsVisible;
		}
		protected internal virtual RangeValueLevel GetValueLevelForLabel(RefinedPoint refinedPoint) {
			return RangeValueLevel.Value1;
		}
		protected internal virtual string[] GetLabelsTexts(RefinedPoint refinedPoint) {
			string labelText = String.Empty;
			if (LabelsVisibility) {
				if (ActualLabel.Formatter == null) {
					PatternParser patternParser = new PatternParser(ActualLabelTextPattern, this);
					patternParser.SetContext(refinedPoint, this);
					labelText = patternParser.GetText();
				}
				else
					labelText = ActualLabel.Formatter.GetDataLabelText(refinedPoint.SeriesPoint);
			}
			return new string[] { labelText };
		}
		protected internal virtual double[] GetAnimatedPointValues(SeriesPoint point) {
			return ValueScaleTypeInternal == ScaleType.Numerical ? new double[] { point.Value } : point.InternalValues;
		}
		protected internal virtual double[] GetPointValues(SeriesPoint point) {
			return new double[] { point.NonAnimatedValue };
		}
		protected internal virtual void SetPointValues(SeriesPoint seriesPoint, double[] values, DateTime[] dateTimeValues) {
			if (values != null && values.Length > 0)
				seriesPoint.Value = values[0];
			if (dateTimeValues != null && dateTimeValues.Length > 0)
				seriesPoint.DateTimeValue = dateTimeValues[0];
		}
		protected internal virtual DateTime[] GetPointDateTimeValues(SeriesPoint point) {
			return new DateTime[] { point.DateTimeValue };
		}
		protected internal virtual Brush GetPenBrush(SolidColorBrush brush) {
			return brush;
		}
		protected internal virtual Color GetPointOriginalColorForCustomDraw(IRefinedSeries refinedSeries, int pointIndex, Color seriesColor) {
			return ActualColorEach ? Palette[pointIndex] : seriesColor;
		}
		protected internal virtual SolidColorBrush MixColor(Color color) {
			return ColorUtils.MixWithDefaultColor(color);
		}
		protected internal virtual SeriesPointAnimationBase CreateDefaultPointAnimation() {
			return null;
		}
		protected internal virtual SeriesAnimationBase CreateDefaultSeriesAnimation() {
			return null;
		}
		protected internal virtual void CompletePointLayout(SeriesPointItem pointItem) {
		}
		protected internal virtual SeriesPointItem[] CreateSeriesPointItems(RefinedPoint refinedPoint, SeriesPointData seriesPointData) {
			return new SeriesPointItem[] { new SeriesPointItem(this, seriesPointData) };
		}
		protected internal virtual string[] GetAvailablePointPatternPlaceholders() {
			return ToolTipPatternUtils.BaseViewPointPatterns;
		}
		protected internal virtual string[] GetAvailableSeriesPatternPlaceholders() {
			return ToolTipPatternUtils.BaseViewSeriesPatterns;
		}
		protected abstract double GetRefinedPointMax(RefinedPoint point);
		protected abstract double GetRefinedPointMin(RefinedPoint point);
		protected abstract double GetRefinedPointAbsMin(RefinedPoint point);
		protected virtual SeriesContainer CreateContainer() {
			return new XYSeriesContainer(this);
		}
		protected virtual SeriesInteractionContainer CreateSeriesGroupsContainer() {
			return new SideBySideInteractionContainer(this);
		}
		protected internal virtual MinMaxValues CalculateMinMaxPointRangeValues(CrosshairSeriesPointEx point, double range, bool isHorizontalCrosshair, IXYDiagram diagram,
			CrosshairPaneInfoEx crosshairPaneInfo, CrosshairSnapModeCore snapMode) {
			return CrosshairManager.CalculateMinMaxMarkerSeriesRangeValues(point, range, isHorizontalCrosshair);
		}
		protected internal void UpdateLegendTextPattern() {
			legendTextPatternAutoCreated = true;
			changeLocker = true;
			LegendTextPattern = legendPointOptionsContainer.ConstructPatternFromPointOptions(this, ActualArgumentScaleType, ValueScaleTypeInternal);
			changeLocker = false;
		}
		protected internal void UpdateLabelTextPattern() {
			if (Label == null)
				Label = new SeriesLabel();
			labelTextPatternAutoCreated = true;
			Label.UpdateTextPattern(pointOptionsContainer.ConstructPatternFromPointOptions(this, ActualArgumentScaleType, ValueScaleTypeInternal));
		}
		protected internal virtual string ConstructValuePattern(PointOptionsContainerBase pointOptionsContainer, ScaleType valueScaleType) {
			return pointOptionsContainer.ConstructValuePattern(valueScaleType);
		}
	}
}
