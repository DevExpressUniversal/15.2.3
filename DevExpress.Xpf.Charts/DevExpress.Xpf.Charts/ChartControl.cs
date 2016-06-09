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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Design.DataAccess;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Printing.BrickCollection;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.DataNodes;
namespace DevExpress.Xpf.Charts {
	[DXToolboxBrowsableAttribute,
	TemplatePart(Name = "PART_NavigationLayer", Type = typeof(NavigationLayer)),
	ContentProperty("Diagram"),
	SupportDXTheme(TypeInAssembly = typeof(ChartControl)),
	DataAccessMetadata("TypedDataSet,EntityFramework,LinqToSql,IEnumerable", DataSourceProperty = "DataSource")]
	public class ChartControl : Control, IChartElement, ICoreReference, IWeakEventListener, IDockTarget, IEventArgsConverterSource, IPrintableControl {
		const string serializeAppName = "ChartControl";
		const bool defaultCrosshairEnabled = true;
		internal const double DefaultConstraint = 300;
		#region Dependency Properties
		static readonly DependencyPropertyKey TitlesPropertyKey = DependencyPropertyManager.RegisterReadOnly("Titles", typeof(TitleCollection), typeof(ChartControl), new PropertyMetadata());
		public static readonly DependencyProperty TitlesProperty = TitlesPropertyKey.DependencyProperty;
		public static readonly DependencyProperty DiagramProperty = DependencyPropertyManager.Register("Diagram", typeof(Diagram), typeof(ChartControl), new PropertyMetadata(DiagramPropertyChanged));
		public static readonly DependencyProperty LegendProperty = DependencyPropertyManager.Register("Legend", typeof(Legend), typeof(ChartControl), new PropertyMetadata(ChartElementHelper.ChangeOwnerAndUpdate));
		public static readonly DependencyProperty DataSourceProperty = DependencyPropertyManager.Register("DataSource", typeof(object), typeof(ChartControl), new PropertyMetadata(null, DataSourcePropertyChanged));
		public static readonly DependencyProperty PaletteProperty = DependencyPropertyManager.Register("Palette", typeof(Palette), typeof(ChartControl), new PropertyMetadata(PalettePropertyChanged));
		public static readonly DependencyProperty IndicatorsPaletteProperty = DependencyPropertyManager.Register("IndicatorsPalette", typeof(IndicatorsPalette), typeof(ChartControl), new PropertyMetadata());
		public static readonly DependencyProperty AnimationModeProperty = DependencyPropertyManager.Register("AnimationMode", typeof(ChartAnimationMode), typeof(ChartControl), new PropertyMetadata(ChartAnimationMode.Disabled, AnimationModePropertyChanged));
		public static readonly DependencyProperty ToolTipEnabledProperty = DependencyPropertyManager.Register("ToolTipEnabled", typeof(bool?), typeof(ChartControl), new PropertyMetadata(null));
		public static readonly DependencyProperty ToolTipOptionsProperty = DependencyPropertyManager.Register("ToolTipOptions", typeof(ToolTipOptions), typeof(ChartControl), new PropertyMetadata(null));
		public static readonly DependencyProperty ToolTipControllerProperty = DependencyPropertyManager.Register("ToolTipController", typeof(ChartToolTipController), typeof(ChartControl), new PropertyMetadata(null));
		public static readonly DependencyProperty ToolTipItemProperty = DependencyPropertyManager.Register("ToolTipItem", typeof(ToolTipItem), typeof(ChartControl), new PropertyMetadata(null));
		public static readonly DependencyProperty CrosshairEnabledProperty = DependencyPropertyManager.Register("CrosshairEnabled", typeof(bool?), typeof(ChartControl), new PropertyMetadata(defaultCrosshairEnabled, CrosshairEnabledPropertyChanged));
		public static readonly DependencyProperty AutoLayoutProperty = DependencyPropertyManager.Register("AutoLayout", typeof(bool), typeof(ChartControl), new PropertyMetadata(true, AutoLayoutPropertyChanged));
		public static readonly DependencyProperty CrosshairOptionsProperty = DependencyPropertyManager.Register("CrosshairOptions", typeof(CrosshairOptions), typeof(ChartControl), new PropertyMetadata(null, CrosshairOptionsPropertyChanged));
		public static readonly DependencyProperty SelectedItemProperty = DependencyPropertyManager.Register("SelectedItem", typeof(object), typeof(ChartControl), new PropertyMetadata(null, OnSelectedItemChanged));
		public static readonly DependencyProperty SelectedItemsProperty = DependencyPropertyManager.Register("SelectedItems", typeof(IList), typeof(ChartControl), new PropertyMetadata(null, OnSelectedItemsChanged));
		public static readonly DependencyProperty SelectionModeProperty = DependencyPropertyManager.Register("SelectionMode", typeof(ElementSelectionMode), typeof(ChartControl), new PropertyMetadata(ElementSelectionMode.None));
		public static readonly DependencyProperty SeriesSelectionModeProperty = DependencyPropertyManager.Register("SeriesSelectionMode", typeof(SeriesSelectionMode), typeof(ChartControl), new PropertyMetadata(SeriesSelectionMode.Point));
		static readonly DependencyPropertyKey AnimationRecordsPropertyKey = DependencyPropertyManager.RegisterReadOnly("AnimationRecords", typeof(ChartAnimationRecordCollection), typeof(ChartControl), new PropertyMetadata());
		public static readonly DependencyProperty AnimationRecordsProperty = AnimationRecordsPropertyKey.DependencyProperty;
		public static readonly DependencyProperty PrintOptionsProperty = DependencyPropertyManager.Register("PrintOptions", typeof(ChartPrintOptions), typeof(ChartControl), new PropertyMetadata(null));
		[Obsolete(ObsoleteMessages.EnableAnimationProperty)]
		public static readonly DependencyProperty EnableAnimationProperty = DependencyPropertyManager.Register("EnableAnimation", typeof(bool), typeof(ChartControl), new PropertyMetadata(false, EnableAnimationPropertyChanged));
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty SeriesPointHitTestInfoProperty = DependencyPropertyManager.Register("SeriesPointHitTestInfo", typeof(RefinedPoint), typeof(ChartControl));
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty SeriesHitTestInfoProperty = DependencyPropertyManager.Register("SeriesHitTestInfo", typeof(Series), typeof(ChartControl));
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty Axis3DHitTestInfoProperty = DependencyPropertyManager.Register("Axis3DHitTestInfo", typeof(Axis3D), typeof(ChartControl));
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty Diagram3DHitTestInfoProperty = DependencyPropertyManager.Register("Diagram3DHitTestInfo", typeof(Diagram3D), typeof(ChartControl));
		#endregion
		public static readonly RoutedEvent BoundDataChangedEvent = EventManager.RegisterRoutedEvent("BoundDataChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ChartControl));
		public static readonly RoutedEvent CustomDrawSeriesEvent = EventManager.RegisterRoutedEvent("CustomDrawSeries", RoutingStrategy.Bubble, typeof(CustomDrawSeriesEventHandler), typeof(ChartControl));
		public static readonly RoutedEvent CustomDrawSeriesPointEvent = EventManager.RegisterRoutedEvent("CustomDrawSeriesPoint", RoutingStrategy.Bubble, typeof(CustomDrawSeriesPointEventHandler), typeof(ChartControl));
		public static readonly RoutedEvent CustomDrawCrosshairEvent = EventManager.RegisterRoutedEvent("CustomDrawCrosshair", RoutingStrategy.Bubble, typeof(CustomDrawCrosshairEventHandler), typeof(ChartControl));
		public static readonly RoutedEvent QueryChartCursorEvent = EventManager.RegisterRoutedEvent("QueryChartCursor", RoutingStrategy.Bubble, typeof(QueryChartCursorEventHandler), typeof(ChartControl));
		public static readonly RoutedEvent AxisScaleChangedEvent = EventManager.RegisterRoutedEvent("AxisScaleChanged", RoutingStrategy.Bubble, typeof(AxisScaleChangedEventHandler), typeof(ChartControl));
		static ChartControl() {
			DXSerializer.SerializationProviderProperty.OverrideMetadata(typeof(ChartControl), new UIPropertyMetadata(new ChartSerializationProvider()));
			DXSerializer.SerializationIDDefaultProperty.OverrideMetadata(typeof(ChartControl), new UIPropertyMetadata(ChartSerializationController.DefaultID));
		}
		static void DiagramPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartControl chart = d as ChartControl;
			ChartElementHelper.ChangeOwner(d, e);
			if (chart != null && chart.Diagram != null) {
				chart.Diagram.UpdateBinding();
				ChartElementHelper.Update(d, new PropertyUpdateInfo<IDiagram>(d, "Diagram", e.OldValue as IDiagram, e.NewValue as IDiagram));
			}
		}
		static void DataSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartControl chart = d as ChartControl;
			if (chart != null) {
				if (chart.Diagram != null) {
					foreach (Series series in chart.Diagram.Series)
						series.ResetDataSourceScale();
				}
				chart.bindingBehavior.UpdateActualDataSource(e.NewValue);
				chart.AddInvalidate(InvalidateMeasureFlags.MeasureDiagram | InvalidateMeasureFlags.Legend | InvalidateMeasureFlags.Titles);
			}
		}
		static void PalettePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartControl chart = d as ChartControl;
			if (chart != null)
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as Palette, e.NewValue as Palette, chart);
			ChartElementHelper.UpdateChartPaletteWithClearDiagramCache(d, e);
		}
		static void AnimationModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartControl chartControl = (ChartControl)d;
			if (chartControl != null) {
				ChartElementHelper.Update(chartControl);
				if (chartControl.Diagram != null)
					foreach (Series series in chartControl.Diagram.Series)
						series.OnAnimationChanged();
			}
		}
		static void CrosshairEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartControl chart = d as ChartControl;
			if (chart != null)
				ChartElementHelper.Update(d, new PropertyUpdateInfo(d, "CrosshairEnabled"));
		}
		static void AutoLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartControl chart = d as ChartControl;
			if (chart != null) {
				chart.actualAutoLayout = (bool)e.NewValue;
				chart.InvalidateMeasure();
			}
		}
		static void CrosshairOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartControl chart = d as ChartControl;
			if (chart != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as CrosshairOptions, e.NewValue as CrosshairOptions, chart);
				ChartElementHelper.Update(d);
			}
		}
		static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartControl chart = d as ChartControl;
			if (chart != null)
				chart.selectionController.OnUpdateSelectedItem(e.NewValue);
		}
		static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartControl chart = d as ChartControl;
			if (chart != null)
				chart.selectionController.OnUpdateSelectedItems(e.NewValue as IList);
		}
		static void EnableAnimationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var chartControl = (ChartControl)d;
			bool enableAnimation = (bool)e.NewValue;
			if (enableAnimation == true)
				chartControl.AnimationMode = ChartAnimationMode.OnLoad;
			else
				chartControl.AnimationMode = ChartAnimationMode.Disabled;
		}
		internal static void SetSeriesPointHitTestInfo(Model3D model, RefinedPoint refinedPoint) {
			Graphics3DUtils.HandleModel(model, delegate(GeometryModel3D geometryModel) {
				geometryModel.SetValue(ChartControl.SeriesPointHitTestInfoProperty, refinedPoint);
			});
		}
		internal static void SetSeriesHitTestInfo(Model3D model, Series series) {
			Graphics3DUtils.HandleModel(model, delegate(GeometryModel3D geometryModel) {
				geometryModel.SetValue(ChartControl.SeriesHitTestInfoProperty, series);
			});
		}
		internal static void SetAxis3DHitTestInfo(Model3D model, Axis3D axis) {
			Graphics3DUtils.HandleModel(model, delegate(GeometryModel3D geometryModel) {
				geometryModel.SetValue(ChartControl.Axis3DHitTestInfoProperty, axis);
			});
		}
		internal static void SetDiagram3DHitTestInfo(Model3D model, Diagram3D diagram3D) {
			Graphics3DUtils.HandleModel(model, delegate(GeometryModel3D geometryModel) {
				geometryModel.SetValue(ChartControl.Diagram3DHitTestInfoProperty, diagram3D);
			});
		}
		readonly List<object> logicalChildren = new List<object>();
		readonly BindingBehavior bindingBehavior;
		readonly NavigationController navigationController;
		readonly ChartToolTipController toolTipController;
		readonly SelectionController selectionController;
		readonly EventArgsConverter eventArgsConverter;
		NavigationLayer navigationLayer;
		CrosshairPanel crosshairPanel;
		int customDrawSeriesPointEventSubscribersCount = 0;
		int customDrawCrosshairEventSubscribersCount = 0;
		InvalidateMeasureFlags shouldInvalidate;
		bool actualAutoLayout = true;
		PrintSizeMode? userPrintSizeMode = null;
		HitTestResult result;
		Window parentWindow;
		bool ActualToolTipEnabled {
			get {
				if (Diagram != null) {
					foreach (Series series in Diagram.Series)
						if (series.ActualToolTipEnabled)
							return true;
				}
				return false;
			}
		}
		internal NavigationController NavigationController {
			get { return navigationController; }
		}
		protected override IEnumerator LogicalChildren {
			get { return logicalChildren.GetEnumerator(); }
		}
		internal PrintSizeMode ActualPrintSizeMode {
			get { return CalculateActualPrintSizeMode(); }
		}
		internal SelectionController SelectionController {
			get { return selectionController; }
		}
		internal ToolTipOptions ActualToolTipOptions {
			get { return ToolTipOptions ?? new ToolTipOptions(); }
		}
		internal CrosshairOptions ActualCrosshairOptions {
			get { return CrosshairOptions ?? new CrosshairOptions(); }
		}
		internal object ActualDataSource {
			get { return bindingBehavior.ActualDataSource; }
		}
		internal NavigationLayer NavigationLayer {
			get { return navigationLayer; }
		}
		internal CrosshairPanel CrosshairPanel {
			get { return crosshairPanel; }
		}
		internal bool PointsToolTipEnabled {
			get { return ActualToolTipOptions.ShowForPoints && ActualToolTipEnabled; } }
		internal bool SeriesToolTipEnabled {
			get { return ActualToolTipOptions.ShowForSeries && ActualToolTipEnabled; } }
		internal bool ActualCrosshairEnabled {
			get {
				if (Diagram != null) {
					foreach (Series series in Diagram.Series)
						if (series.ActualCrosshairEnabled)
							return true;
				}
				return false;
			}
		}
		internal bool ActualAutoLayout {
			get { return actualAutoLayout; }
		}
		internal bool CrosshairSupported {
			get {
				return Diagram != null && Diagram is XYDiagram2D;
			}
		}
		internal bool LegendUseCheckBoxes {
			get {
				return Legend != null && Legend.UseCheckBoxes;
			}
		}
		internal bool Loading { get; set; }
		public bool ShouldRaiseCustomDrawSeriesPointEvent {
			get { return customDrawSeriesPointEventSubscribersCount > 0; }
		}
		public bool ShouldRaiseCustomDrawCrosshairEvent {
			get { return customDrawCrosshairEventSubscribersCount > 0; }
		}
		public ChartToolTipController ActualToolTipController {
			get { return ToolTipController != null ? ToolTipController : toolTipController; }
		}
		[Category(Categories.Behavior),
		NonTestableProperty,
		XtraSerializableProperty]
		public bool? ToolTipEnabled {
			get { return (bool?)GetValue(ToolTipEnabledProperty); }
			set { SetValue(ToolTipEnabledProperty, value); }
		}
		[Category(Categories.Behavior),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)]
		public ToolTipOptions ToolTipOptions {
			get { return (ToolTipOptions)GetValue(ToolTipOptionsProperty); }
			set { SetValue(ToolTipOptionsProperty, value); }
		}
		[Category(Categories.Behavior),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)]
		public ChartToolTipController ToolTipController {
			get { return (ChartToolTipController)GetValue(ToolTipControllerProperty); }
			set { SetValue(ToolTipControllerProperty, value); }
		}
		[DXBrowsable(false),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ToolTipItem ToolTipItem {
			get { return (ToolTipItem)GetValue(ToolTipItemProperty); }
			set { SetValue(ToolTipItemProperty, value); }
		}
		[Category(Categories.Behavior),
		NonTestableProperty,
		XtraSerializableProperty]
		public bool? CrosshairEnabled {
			get { return (bool?)GetValue(CrosshairEnabledProperty); }
			set { SetValue(CrosshairEnabledProperty, value); }
		}
		[Category(Categories.Behavior),
		XtraSerializableProperty]
		public bool AutoLayout {
			get { return (bool)GetValue(AutoLayoutProperty); }
			set { SetValue(AutoLayoutProperty, value); }
		}
		[Category(Categories.Behavior),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)]
		public CrosshairOptions CrosshairOptions {
			get { return (CrosshairOptions)GetValue(CrosshairOptionsProperty); }
			set { SetValue(CrosshairOptionsProperty, value); }
		}
		[NonCategorized]
		public object SelectedItem {
			get { return (object)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}
		[NonCategorized,
		NonTestableProperty]
		public IList SelectedItems {
			get { return (IList)GetValue(SelectedItemsProperty); }
			set { SetValue(SelectedItemsProperty, value); }
		}
		[NonCategorized]
		public ElementSelectionMode SelectionMode {
			get { return (ElementSelectionMode)GetValue(SelectionModeProperty); }
			set { SetValue(SelectionModeProperty, value); }
		}
		[NonCategorized]
		public SeriesSelectionMode SeriesSelectionMode {
			get { return (SeriesSelectionMode)GetValue(SeriesSelectionModeProperty); }
			set { SetValue(SeriesSelectionModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ChartControlTitles"),
#endif
		Category(Categories.Elements),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)]
		public TitleCollection Titles {
			get { return (TitleCollection)GetValue(TitlesProperty); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ChartControlDiagram"),
#endif
		Category(Categories.Elements),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)]
		public Diagram Diagram {
			get { return (Diagram)GetValue(DiagramProperty); }
			set { SetValue(DiagramProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ChartControlLegend"),
#endif
		Category(Categories.Elements),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)]
		public Legend Legend {
			get { return (Legend)GetValue(LegendProperty); }
			set { SetValue(LegendProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ChartControlDataSource"),
#endif
		Category(Categories.Data)]
		public object DataSource {
			get { return GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ChartControlPalette"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)]
		public Palette Palette {
			get { return (Palette)GetValue(PaletteProperty); }
			set { SetValue(PaletteProperty, value); }
		}
		[Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)]
		public IndicatorsPalette IndicatorsPalette {
			get { return (IndicatorsPalette)GetValue(IndicatorsPaletteProperty); }
			set { SetValue(IndicatorsPaletteProperty, value); }
		}
		[Obsolete(ObsoleteMessages.EnableAnimationProperty),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)]
		public bool EnableAnimation {
			get { return (bool)GetValue(EnableAnimationProperty); }
			set { SetValue(EnableAnimationProperty, value); }
		}
		[
		Category(Categories.Animation),
		XtraSerializableProperty]
		public ChartAnimationMode AnimationMode {
			get { return (ChartAnimationMode)GetValue(AnimationModeProperty); }
			set { SetValue(AnimationModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ChartControlAnimationRecords"),
#endif
		Category(Categories.Animation)]
		public ChartAnimationRecordCollection AnimationRecords {
			get { return (ChartAnimationRecordCollection)GetValue(AnimationRecordsProperty); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ChartControlPrintOptions"),
#endif
		Category(Categories.Common)]
		public ChartPrintOptions PrintOptions {
			get { return (ChartPrintOptions)GetValue(PrintOptionsProperty); }
			set { SetValue(PrintOptionsProperty, value); }
		}
		#region Hidden properties
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new HorizontalAlignment HorizontalContentAlignment { get { return base.HorizontalContentAlignment; } set { base.HorizontalContentAlignment = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new VerticalAlignment VerticalContentAlignment { get { return base.VerticalContentAlignment; } set { base.VerticalContentAlignment = value; } }
		#endregion
		public event RoutedEventHandler BoundDataChanged {
			add { AddHandler(BoundDataChangedEvent, value); }
			remove { RemoveHandler(BoundDataChangedEvent, value); }
		}
		public event CustomDrawSeriesEventHandler CustomDrawSeries {
			add { AddHandler(CustomDrawSeriesEvent, value); }
			remove { RemoveHandler(CustomDrawSeriesEvent, value); }
		}
		public event CustomDrawSeriesPointEventHandler CustomDrawSeriesPoint {
			add {
				AddHandler(CustomDrawSeriesPointEvent, value);
				customDrawSeriesPointEventSubscribersCount++;
			}
			remove {
				RemoveHandler(CustomDrawSeriesPointEvent, value);
				customDrawSeriesPointEventSubscribersCount--;
			}
		}
		public event CustomDrawCrosshairEventHandler CustomDrawCrosshair {
			add {
				AddHandler(CustomDrawCrosshairEvent, value);
				customDrawCrosshairEventSubscribersCount++;
			}
			remove {
				RemoveHandler(CustomDrawCrosshairEvent, value);
				customDrawCrosshairEventSubscribersCount--;
			}
		}
		public event QueryChartCursorEventHandler QueryChartCursor {
			add { AddHandler(QueryChartCursorEvent, value); }
			remove { RemoveHandler(QueryChartCursorEvent, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ChartControlAxisScaleChanged"),
#endif
 Category(Categories.Behavior)]
		public event AxisScaleChangedEventHandler AxisScaleChanged {
			add { AddHandler(AxisScaleChangedEvent, value); }
			remove { RemoveHandler(AxisScaleChangedEvent, value); }
		}
		public ChartControl() {
			DefaultStyleKey = typeof(ChartControl);
			this.SetValue(TitlesPropertyKey, ChartElementHelper.CreateInstance<ChartTitleCollection>(this));
			bindingBehavior = new BindingBehavior();
			bindingBehavior.ActualDataSourceChanged += delegate {
				if (Diagram != null)
					Diagram.UpdateBinding();
			};
			navigationController = new NavigationController(this);
			selectionController = new SelectionController(this);
			eventArgsConverter = new EventArgsConverter(this);
			KeyUp += new KeyEventHandler(navigationController.KeyUp);
			KeyDown += new KeyEventHandler(NavigationController.KeyDown);
			MouseMove += new MouseEventHandler(NavigationController.MouseMove);
			MouseWheel += new MouseWheelEventHandler(NavigationController.MouseWheel);
			MouseLeftButtonDown += new MouseButtonEventHandler(NavigationController.MouseLeftButtonDown);
			MouseLeftButtonUp += new MouseButtonEventHandler(NavigationController.MouseLeftButtonUp);
			MouseLeave += new MouseEventHandler(NavigationController.MouseLeave);
			MouseEnter += new MouseEventHandler(NavigationController.MouseEnter);
			Loaded += ChartControl_Loaded;
			Unloaded += ChartControl_Unloaded;
			toolTipController = new ChartToolTipController();
			Initialize();
			LayoutUpdated += delegate {
				try {
					if (shouldInvalidate > 0) {
						InvalidateMeasure();
						if ((shouldInvalidate & InvalidateMeasureFlags.MeasureDiagram) > 0 && Diagram != null)
							Diagram.InvalidateMeasure();
						if ((shouldInvalidate & InvalidateMeasureFlags.ArrangeDiagram) > 0 && Diagram != null)
							Diagram.InvalidateArrange();
						if ((shouldInvalidate & InvalidateMeasureFlags.Legend) > 0 && Legend != null && !Legend.Visible.HasValue)
							Legend.InvalidateMeasure();
						if ((shouldInvalidate & InvalidateMeasureFlags.Titles) > 0)
							foreach (Title title in this.Titles)
								if (!title.Visible.HasValue)
									title.InvalidateMeasure();
					}
				}
				finally {
					shouldInvalidate = 0;
				}
			};
			SizeChanged += ChartControl_SizeChanged;
		}
		void ChartControl_SizeChanged(object sender, SizeChangedEventArgs e) {
			ChartElementHelper.Update(this);
		}
		#region IChartElement Members
		IChartElement IOwnedElement.Owner {
			get { return null; }
			set { }
		}
		bool IChartElement.Changed(ChartUpdate args) {
			if ((args.Change & ChartElementChange.UpdateChartPalette) != 0)
				if (Diagram != null)
					return !Diagram.TryUpdatePalette();
			return true;
		}
		ViewController INotificationOwner.Controller {
			get {
				return Diagram != null ? Diagram.ViewController : null;
			}
		}
		#endregion
		#region IWeakEventListener implementation
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if (managerType != typeof(PropertyChangedWeakEventManager))
				return false;
			if (sender is CrosshairOptions) {
				if (e is PropertyChangedEventArgs)
					ChartElementHelper.Update((IChartElement)this, new PropertyUpdateInfo(sender, ((PropertyChangedEventArgs)e).PropertyName));
				else
					ChartElementHelper.Update(this);
				return true;
			}
			ChartElementHelper.UpdateWithClearDiagramCache(this);
			return true;
		}
		#endregion
		#region IToolTipDockTarget implementation
		Rect IDockTarget.GetBounds() {
			return new Rect(new Point(0, 0), RenderSize);
		}
		#endregion
		#region IEventArgsConverterSource implementation
		class EventArgsConverter : IDataRowEventArgsConverter {
			readonly ChartControl chartControl;
			public EventArgsConverter(ChartControl chartControl) {
				this.chartControl = chartControl;
			}
			object IDataRowEventArgsConverter.GetDataRow(System.Windows.RoutedEventArgs e) {
				IHitTestableElement hitTestableElement = GetParentHitTestableElement(e.OriginalSource as DependencyObject);
				if (hitTestableElement != null) {
					RefinedPoint refinedPoint = hitTestableElement.AdditionalElement is RefinedPoint ? hitTestableElement.AdditionalElement as RefinedPoint : hitTestableElement.Element as RefinedPoint;
					SeriesPoint seriesPoint = refinedPoint != null ? SeriesPoint.GetSeriesPoint(refinedPoint.SeriesPoint) : null;
					if (seriesPoint != null) {
						if (seriesPoint.Series != null && seriesPoint.Series.IsAutoPointsAdded)
							return seriesPoint.Tag;
						return seriesPoint;
					}
				}
				return null;
			}
			IHitTestableElement GetParentHitTestableElement(DependencyObject dependencyObject) {
				for (DependencyObject parent = dependencyObject; parent != null && !(parent is ChartControl); parent = VisualTreeHelper.GetParent(parent))
					if (parent is IHitTestableElement)
						return (IHitTestableElement)parent;
				return null;
			}
		}
		object IEventArgsConverterSource.EventArgsConverter { get { return eventArgsConverter; } }
		#endregion
		#region IPrintableControl implementation
		bool IPrintableControl.CanCreateRootNodeAsync {
			get { return false; }
		}
		IRootDataNode IPrintableControl.CreateRootNode(Size usablePageSize, Size reportHeaderSize, Size reportFooterSize, Size pageHeaderSize, Size pageFooterSize) {
			return new ChartRootDataNode(this, usablePageSize, pageHeaderSize, pageFooterSize);
		}
		void IPrintableControl.CreateRootNodeAsync(Size usablePageSize, Size reportHeaderSize, Size reportFooterSize, Size pageHeaderSize, Size pageFooterSize) {
		}
		event EventHandler<DevExpress.Data.Utils.ServiceModel.ScalarOperationCompletedEventArgs<IRootDataNode>> IPrintableControl.CreateRootNodeCompleted {
			add { }
			remove { }
		}
		IVisualTreeWalker IPrintableControl.GetCustomVisualTreeWalker() {
			return null;
		}
		void IPrintableControl.PagePrintedCallback(IEnumerator pageBrickEnumerator, Dictionary<IVisualBrick, IOnPageUpdater> brickUpdaters) {
		}
		#endregion
		void ChartControl_Loaded(object sender, RoutedEventArgs e) {
			parentWindow = LayoutHelper.FindLayoutOrVisualParentObject<Window>(this);
			if (parentWindow != null) {
				parentWindow.Deactivated += new EventHandler(NavigationController.ApplicationDeactivated);
				parentWindow.Activated += new EventHandler(NavigationController.ApplicationActivated);
			}
		}
		void ChartControl_Unloaded(object sender, RoutedEventArgs e) {
			if (parentWindow != null) {
				parentWindow.Deactivated -= new EventHandler(NavigationController.ApplicationDeactivated);
				parentWindow.Activated -= new EventHandler(NavigationController.ApplicationActivated);
			}
		}
		void Initialize() {
			SnapsToDevicePixels = true;
			this.SetValue(AnimationRecordsPropertyKey, ChartElementHelper.CreateInstance<ChartAnimationRecordCollection>(this));
			ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(NavigationController.ManipulationStarted);
			ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(NavigationController.ManipulationDelta);
			ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(NavigationController.ManipulationCompleted);
		}
		void IChartElement.AddChild(object child) {
			UIElement element = child as UIElement;
			if (!logicalChildren.Contains(child)) {
				logicalChildren.Add(child);
				AddLogicalChild(child);
			}
		}
		void IChartElement.RemoveChild(object child) {
			UIElement element = child as UIElement;
			if (logicalChildren.Contains(child)) {
				logicalChildren.Remove(child);
				RemoveLogicalChild(child);
			}
		}
		void UpdatePrintSizeMode(PrintSizeMode? sizeMode) {
			this.userPrintSizeMode = sizeMode;
		}
		void ExecutePrintActionWithSizeMode(PrintSizeMode sizeMode, Action printAction) {
			UpdatePrintSizeMode(sizeMode);
			printAction();
			UpdatePrintSizeMode(null);
		}
		HitTestResultBehavior HitTestResult(HitTestResult result) {
			this.result = result;
			return HitTestResultBehavior.Stop;
		}
		ChartHitInfo CalcHitInfoInternal(HitTestResult hitTestResult, Point point, double radius = 0) {
			if (result is RayMeshGeometry3DHitTestResult) {
				RayMeshGeometry3DHitTestResult ray3DResult = (RayMeshGeometry3DHitTestResult)result;
				RefinedPoint seriesPoint3D = ray3DResult.ModelHit.GetValue(ChartControl.SeriesPointHitTestInfoProperty) as RefinedPoint;
				Series series3D = ray3DResult.ModelHit.GetValue(ChartControl.SeriesHitTestInfoProperty) as Series;
				Axis3D axis3D = ray3DResult.ModelHit.GetValue(ChartControl.Axis3DHitTestInfoProperty) as Axis3D;
				Diagram3D diagram3D = ray3DResult.ModelHit.GetValue(ChartControl.Diagram3DHitTestInfoProperty) as Diagram3D;
				if (seriesPoint3D != null) {
					ChartHitInfo hitInfo = new ChartHitInfo(seriesPoint3D, series3D);
					return hitInfo;
				}
				else if (series3D != null)
					return new ChartHitInfo(seriesPoint3D, series3D);
				else if (axis3D != null)
					return new ChartHitInfo(axis3D);
				else if (diagram3D != null)
					return new ChartHitInfo(diagram3D);
			}
			ChartHitInfo hitInfo2D = new ChartHitInfo(this, point, radius);
			return hitInfo2D;
		}
		PrintSizeMode CalculateActualPrintSizeMode() {
			if (userPrintSizeMode.HasValue)
				return userPrintSizeMode.Value;
			if (PrintOptions != null)
				return PrintOptions.SizeMode;
			return ChartPrintOptions.DefaultPrintSizeMode;
		}
		protected override Size MeasureOverride(Size constraint) {
			if (Double.IsInfinity(constraint.Width))
				constraint.Width = DefaultConstraint;
			if (Double.IsInfinity(constraint.Height))
				constraint.Height = DefaultConstraint;
			if (Diagram != null)
				Diagram.ClearViewportCache();
			Size size = base.MeasureOverride(constraint);
			AutoLayoutController autoLayoutController = new AutoLayoutController();
			autoLayoutController.CalculateLayout(this, constraint);
			return size;
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			Size size = DesiredSize;
			Thickness margin = Margin;
			double width = Math.Max(0, size.Width - margin.Left - margin.Right);
			double height = Math.Max(0, size.Height - margin.Top - margin.Bottom);
			return base.ArrangeOverride(new Size(width, height));
		}
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new ChartControlAutomationPeer(this);
		}
		internal void RaiseBoundDataChangedEvent() {
			RaiseEvent(new RoutedEventArgs(BoundDataChangedEvent));
		}
		internal void InvalidateChartElementPanel() {
			ChartElementPanel chartElementPanel = LayoutHelper.FindElement(this, element => element is ChartElementPanel) as ChartElementPanel;
			if (chartElementPanel != null) {
				chartElementPanel.InvalidateMeasure();
			}
		}
		internal void UpdateSelection() {
			selectionController.UpdateElementsSelection();
		}
		internal void AddInvalidate(InvalidateMeasureFlags flags) {
			shouldInvalidate |= flags;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			navigationLayer = GetTemplateChild("PART_NavigationLayer") as NavigationLayer;
			crosshairPanel = GetTemplateChild("PART_CrosshairLabelsContainer") as CrosshairPanel;
		}
		public void UpdateData() {
			if (Diagram != null)
				Diagram.UpdateBinding();
		}
		public void Animate() {
			Diagram diagram = Diagram;
			if (diagram != null)
				diagram.ViewController.Animate();
		}
		public void SaveToFile(string fileName) {
			DXSerializer.SerializeSingleObject(this, fileName, serializeAppName);
		}
		public void SaveToStream(Stream stream) {
			DXSerializer.SerializeSingleObject(this, stream, serializeAppName);
		}
		public void LoadFromFile(string fileName) {
			BeginInit();
			DXSerializer.DeserializeSingleObject(this, fileName, serializeAppName);
			EndInit();
		}
		public void LoadFromStream(Stream stream) {
			BeginInit();
			DXSerializer.DeserializeSingleObject(this, stream, serializeAppName);
			EndInit();
		}
		public override void BeginInit() {
			base.BeginInit();
			Loading = true;
		}
		public override void EndInit() {
			base.EndInit();
			if (Diagram != null) {
				foreach (Series series in Diagram.Series) {
					series.Points.ForceEndInit();
				}
			}
			Loading = false;
			ChartElementHelper.Update((IChartElement)this, new OnLoadEndUpdateInfo(this), ChartElementChange.ClearDiagramCache | ChartElementChange.UpdateChartBinding | ChartElementChange.UpdateActualPanes | ChartElementChange.UpdateXYDiagram2DItems);
		}
		public ChartHitInfo CalcHitInfo(Point point) {
			this.result = null;
			VisualTreeHelper.HitTest(this, new HitTestFilterCallback(HitTestingHelper.HitFilter), new HitTestResultCallback(HitTestResult), new PointHitTestParameters(point));
			return CalcHitInfoInternal(this.result, point);
		}
		public ChartHitInfo CalcHitInfo(Point point, double radius) {
			result = null;
			if (Diagram is Diagram3D)
				return CalcHitInfo(point);
			double side = 2 * radius;
			Geometry square = new RectangleGeometry(new Rect(point.X - radius, point.Y - radius, side, side));
			VisualTreeHelper.HitTest(this, new HitTestFilterCallback(HitTestingHelper.HitFilter), new HitTestResultCallback(HitTestResult), new GeometryHitTestParameters(square));
			return CalcHitInfoInternal(this.result, point, radius);
		}
		#region Print methods
		public void Print() {
			PrintHelper.Print(this);
		}
		public void PrintDirect() {
			PrintHelper.PrintDirect(this);
		}
		public void PrintDirect(System.Printing.PrintQueue queue) {
			PrintHelper.PrintDirect(this, queue);
		}
		public void Print(PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.Print(this); }
			);
		}
		public void PrintDirect(PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.PrintDirect(this); }
			);
		}
		public void PrintDirect(System.Printing.PrintQueue queue, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.PrintDirect(this, queue); }
			);
		}
		#endregion
		#region Print preview methods
		#region Print preview
		public void ShowPrintPreview(FrameworkElement owner) {
			PrintHelper.ShowPrintPreview(owner, this);
		}
		public void ShowPrintPreview(FrameworkElement owner, string documentName) {
			PrintHelper.ShowPrintPreview(owner, this, documentName);
		}
		public void ShowPrintPreview(FrameworkElement owner, string documentName, string title) {
			PrintHelper.ShowPrintPreview(owner, this, documentName, title);
		}
		public void ShowPrintPreview(Window owner) {
			PrintHelper.ShowPrintPreview(owner, this);
		}
		public void ShowPrintPreview(Window owner, string documentName) {
			PrintHelper.ShowPrintPreview(owner, this, documentName);
		}
		public void ShowPrintPreview(Window owner, string documentName, string title) {
			PrintHelper.ShowPrintPreview(owner, this, documentName, title);
		}
		public void ShowPrintPreview(FrameworkElement owner, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ShowPrintPreview(owner, this); }
			);
		}
		public void ShowPrintPreview(FrameworkElement owner, string documentName, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ShowPrintPreview(owner, this, documentName); }
			);
		}
		public void ShowPrintPreview(FrameworkElement owner, string documentName, string title, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ShowPrintPreview(owner, this, documentName, title); }
			);
		}
		public void ShowPrintPreview(Window owner, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ShowPrintPreview(owner, this); }
			);
		}
		public void ShowPrintPreview(Window owner, string documentName, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ShowPrintPreview(owner, this, documentName); }
			);
		}
		public void ShowPrintPreview(Window owner, string documentName, string title, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ShowPrintPreview(owner, this, documentName, title); }
			);
		}
		#endregion
		#region Print preview dialog
		public void ShowPrintPreviewDialog(Window owner) {
			PrintHelper.ShowPrintPreviewDialog(owner, this);
		}
		public void ShowPrintPreviewDialog(Window owner, string documentName) {
			PrintHelper.ShowPrintPreviewDialog(owner, this, documentName);
		}
		public void ShowPrintPreviewDialog(Window owner, string documentName, string title) {
			PrintHelper.ShowPrintPreviewDialog(owner, this, documentName, title);
		}
		public void ShowPrintPreviewDialog(Window owner, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ShowPrintPreviewDialog(owner, this); }
			);
		}
		public void ShowPrintPreviewDialog(Window owner, string documentName, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ShowPrintPreviewDialog(owner, this, documentName); }
			);
		}
		public void ShowPrintPreviewDialog(Window owner, string documentName, string title, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ShowPrintPreviewDialog(owner, this, documentName, title); }
			);
		}
		#endregion
		#region Print preview Ribbon
		public void ShowRibbonPrintPreview(FrameworkElement owner) {
			PrintHelper.ShowRibbonPrintPreview(owner, this);
		}
		public void ShowRibbonPrintPreview(FrameworkElement owner, string documentName) {
			PrintHelper.ShowRibbonPrintPreview(owner, this, documentName);
		}
		public void ShowRibbonPrintPreview(FrameworkElement owner, string documentName, string title) {
			PrintHelper.ShowRibbonPrintPreview(owner, this, documentName, title);
		}
		public void ShowRibbonPrintPreview(Window owner) {
			PrintHelper.ShowRibbonPrintPreview(owner, this);
		}
		public void ShowRibbonPrintPreview(Window owner, string documentName) {
			PrintHelper.ShowRibbonPrintPreview(owner, this, documentName);
		}
		public void ShowRibbonPrintPreview(Window owner, string documentName, string title) {
			PrintHelper.ShowRibbonPrintPreview(owner, this, documentName, title);
		}
		public void ShowRibbonPrintPreview(FrameworkElement owner, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ShowRibbonPrintPreview(owner, this); }
			);
		}
		public void ShowRibbonPrintPreview(FrameworkElement owner, string documentName, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ShowRibbonPrintPreview(owner, this, documentName); }
			);
		}
		public void ShowRibbonPrintPreview(FrameworkElement owner, string documentName, string title, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ShowRibbonPrintPreview(owner, this, documentName, title); }
			);
		}
		public void ShowRibbonPrintPreview(Window owner, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ShowRibbonPrintPreview(owner, this); }
			);
		}
		public void ShowRibbonPrintPreview(Window owner, string documentName, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ShowRibbonPrintPreview(owner, this, documentName); }
			);
		}
		public void ShowRibbonPrintPreview(Window owner, string documentName, string title, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ShowRibbonPrintPreview(owner, this, documentName, title); }
			);
		}
		#endregion
		#region Print preview dialog Ribbon
		public void ShowRibbonPrintPreviewDialog(Window owner) {
			PrintHelper.ShowRibbonPrintPreviewDialog(owner, this);
		}
		public void ShowRibbonPrintPreviewDialog(Window owner, string documentName) {
			PrintHelper.ShowRibbonPrintPreviewDialog(owner, this, documentName);
		}
		public void ShowRibbonPrintPreviewDialog(Window owner, string documentName, string title) {
			PrintHelper.ShowRibbonPrintPreviewDialog(owner, this, documentName, title);
		}
		public void ShowRibbonPrintPreviewDialog(Window owner, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ShowRibbonPrintPreviewDialog(owner, this); }
			);
		}
		public void ShowRibbonPrintPreviewDialog(Window owner, string documentName, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ShowRibbonPrintPreviewDialog(owner, this, documentName); }
			);
		}
		public void ShowRibbonPrintPreviewDialog(Window owner, string documentName, string title, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ShowRibbonPrintPreviewDialog(owner, this, documentName, title); }
			);
		}
		#endregion
		#endregion
		#region Export methods
		#region RTF
		public void ExportToRtf(string filePath) {
			PrintHelper.ExportToRtf(this, filePath);
		}
		public void ExportToRtf(string filePath, RtfExportOptions options) {
			PrintHelper.ExportToRtf(this, filePath, options);
		}
		public void ExportToRtf(Stream stream) {
			PrintHelper.ExportToRtf(this, stream);
		}
		public void ExportToRtf(Stream stream, RtfExportOptions options) {
			PrintHelper.ExportToRtf(this, stream, options);
		}
		public void ExportToRtf(string filePath, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToRtf(this, filePath); }
			);
		}
		public void ExportToRtf(string filePath, RtfExportOptions options, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToRtf(this, filePath, options); }
			);
		}
		public void ExportToRtf(Stream stream, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToRtf(this, stream); }
			);
		}
		public void ExportToRtf(Stream stream, RtfExportOptions options, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToRtf(this, stream, options); }
			);
		}
		#endregion
		#region PDF
		public void ExportToPdf(string filePath) {
			PrintHelper.ExportToPdf(this, filePath);
		}
		public void ExportToPdf(string filePath, PdfExportOptions options) {
			PrintHelper.ExportToPdf(this, filePath, options);
		}
		public void ExportToPdf(Stream stream) {
			PrintHelper.ExportToPdf(this, stream);
		}
		public void ExportToPdf(Stream stream, PdfExportOptions options) {
			PrintHelper.ExportToPdf(this, stream, options);
		}
		public void ExportToPdf(string filePath, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToPdf(this, filePath); }
			);
		}
		public void ExportToPdf(string filePath, PdfExportOptions options, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToPdf(this, filePath, options); }
			);
		}
		public void ExportToPdf(Stream stream, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToPdf(this, stream); }
			);
		}
		public void ExportToPdf(Stream stream, PdfExportOptions options, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToPdf(this, stream, options); }
			);
		}
		#endregion
		#region XLS
		public void ExportToXls(string filePath) {
			PrintHelper.ExportToXls(this, filePath);
		}
		public void ExportToXls(string filePath, XlsExportOptions options) {
			PrintHelper.ExportToXls(this, filePath, options);
		}
		public void ExportToXls(Stream stream) {
			PrintHelper.ExportToXls(this, stream);
		}
		public void ExportToXls(Stream stream, XlsExportOptions options) {
			PrintHelper.ExportToXls(this, stream, options);
		}
		public void ExportToXls(string filePath, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToXls(this, filePath); }
			);
		}
		public void ExportToXls(string filePath, XlsExportOptions options, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToXls(this, filePath, options); }
			);
		}
		public void ExportToXls(Stream stream, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToXls(this, stream); }
			);
		}
		public void ExportToXls(Stream stream, XlsExportOptions options, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToXls(this, stream, options); }
			);
		}
		#endregion
		#region XLSX
		public void ExportToXlsx(Stream stream) {
			PrintHelper.ExportToXlsx(this, stream);
		}
		public void ExportToXlsx(Stream stream, XlsxExportOptions options) {
			PrintHelper.ExportToXlsx(this, stream, options);
		}
		public void ExportToXlsx(string filePath) {
			PrintHelper.ExportToXlsx(this, filePath);
		}
		public void ExportToXlsx(string filePath, XlsxExportOptions options) {
			PrintHelper.ExportToXlsx(this, filePath, options);
		}
		public void ExportToXlsx(Stream stream, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToXlsx(this, stream); }
			);
		}
		public void ExportToXlsx(Stream stream, XlsxExportOptions options, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToXlsx(this, stream, options); }
			);
		}
		public void ExportToXlsx(string filePath, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToXlsx(this, filePath); }
			);
		}
		public void ExportToXlsx(string filePath, XlsxExportOptions options, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToXlsx(this, filePath, options); }
			);
		}
		#endregion
		#region HTML
		public void ExportToHtml(string filePath) {
			PrintHelper.ExportToHtml(this, filePath);
		}
		public void ExportToHtml(string filePath, HtmlExportOptions options) {
			PrintHelper.ExportToHtml(this, filePath, options);
		}
		public void ExportToHtml(Stream stream) {
			PrintHelper.ExportToHtml(this, stream);
		}
		public void ExportToHtml(Stream stream, HtmlExportOptions options) {
			PrintHelper.ExportToHtml(this, stream, options);
		}
		public void ExportToHtml(string filePath, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToHtml(this, filePath); }
			);
		}
		public void ExportToHtml(string filePath, HtmlExportOptions options, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToHtml(this, filePath, options); }
			);
		}
		public void ExportToHtml(Stream stream, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToHtml(this, stream); }
			);
		}
		public void ExportToHtml(Stream stream, HtmlExportOptions options, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToHtml(this, stream, options); }
			);
		}
		#endregion
		#region MHT
		public void ExportToMht(string filePath) {
			PrintHelper.ExportToMht(this, filePath);
		}
		public void ExportToMht(string filePath, MhtExportOptions options) {
			PrintHelper.ExportToMht(this, filePath, options);
		}
		public void ExportToMht(Stream stream) {
			PrintHelper.ExportToMht(this, stream);
		}
		public void ExportToMht(Stream stream, MhtExportOptions options) {
			PrintHelper.ExportToMht(this, stream, options);
		}
		public void ExportToMht(string filePath, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToMht(this, filePath); }
			);
		}
		public void ExportToMht(string filePath, MhtExportOptions options, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToMht(this, filePath, options); }
			);
		}
		public void ExportToMht(Stream stream, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToMht(this, stream); }
			);
		}
		public void ExportToMht(Stream stream, MhtExportOptions options, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToMht(this, stream, options); }
			);
		}
		#endregion
		#region XPS
		public void ExportToXps(string filePath) {
			PrintHelper.ExportToXps(this, filePath);
		}
		public void ExportToXps(string filePath, XpsExportOptions options) {
			PrintHelper.ExportToXps(this, filePath, options);
		}
		public void ExportToXps(Stream stream) {
			PrintHelper.ExportToXps(this, stream);
		}
		public void ExportToXps(Stream stream, XpsExportOptions options) {
			PrintHelper.ExportToXps(this, stream, options);
		}
		public void ExportToXps(string filePath, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToXps(this, filePath); }
			);
		}
		public void ExportToXps(string filePath, XpsExportOptions options, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToXps(this, filePath, options); }
			);
		}
		public void ExportToXps(Stream stream, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToXps(this, stream); }
			);
		}
		public void ExportToXps(Stream stream, XpsExportOptions options, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToXps(this, stream, options); }
			);
		}
		#endregion
		#region Image
		public void ExportToImage(string filePath) {
			PrintHelper.ExportToImage(this, filePath);
		}
		public void ExportToImage(string filePath, ImageExportOptions options) {
			PrintHelper.ExportToImage(this, filePath, options);
		}
		public void ExportToImage(Stream stream) {
			PrintHelper.ExportToImage(this, stream);
		}
		public void ExportToImage(Stream stream, ImageExportOptions options) {
			PrintHelper.ExportToImage(this, stream, options);
		}
		public void ExportToImage(string filePath, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToImage(this, filePath); }
			);
		}
		public void ExportToImage(string filePath, ImageExportOptions options, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToImage(this, filePath, options); }
			);
		}
		public void ExportToImage(Stream stream, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToImage(this, stream); }
			);
		}
		public void ExportToImage(Stream stream, ImageExportOptions options, PrintSizeMode sizeMode) {
			ExecutePrintActionWithSizeMode(
				sizeMode,
				delegate() { PrintHelper.ExportToImage(this, stream, options); }
			);
		}
		#endregion
		#endregion
	}
}
