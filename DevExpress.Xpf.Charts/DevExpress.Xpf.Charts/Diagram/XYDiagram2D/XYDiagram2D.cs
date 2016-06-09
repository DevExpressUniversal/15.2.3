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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum XYDiagram2DElementType {
		None,
		DiagramContent,
		Mirror,
		Pseudo3DMirror,
		Pseudo3DBarSeriesContainer,
		CrosshairContainer,
		Selection
	}
	[
	TemplatePart(Name = "PART_LogicalTreeHolder", Type = typeof(ChartItemsControl)),
	TemplatePart(Name = "PART_Panes", Type = typeof(ChartItemsControl))
	]
	public sealed class XYDiagram2D : Diagram2D, IXYDiagram, IWeakEventListener {
		const double defaultAxisLength = 500;
		internal static readonly DependencyPropertyKey SecondaryAxesXPropertyKey = DependencyPropertyManager.RegisterReadOnly("SecondaryAxesX",
			typeof(SecondaryAxisXCollection), typeof(XYDiagram2D), new PropertyMetadata());
		internal static readonly DependencyPropertyKey SecondaryAxesYPropertyKey = DependencyPropertyManager.RegisterReadOnly("SecondaryAxesY",
			typeof(SecondaryAxisYCollection), typeof(XYDiagram2D), new PropertyMetadata());
		internal static readonly DependencyPropertyKey PanesPropertyKey = DependencyPropertyManager.RegisterReadOnly("Panes",
			typeof(PaneCollection), typeof(XYDiagram2D), new PropertyMetadata(null));
		static readonly DependencyPropertyKey ActualPanesPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualPanes",
			typeof(ObservableCollection<Pane>), typeof(XYDiagram2D), new PropertyMetadata(null));
		public static readonly DependencyProperty SecondaryAxesXProperty = SecondaryAxesXPropertyKey.DependencyProperty;
		public static readonly DependencyProperty SecondaryAxesYProperty = SecondaryAxesYPropertyKey.DependencyProperty;
		public static readonly DependencyProperty PanesProperty = PanesPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ActualPanesProperty = ActualPanesPropertyKey.DependencyProperty;
		public static readonly DependencyProperty CrosshairSeriesLabelForegroundProperty = DependencyPropertyManager.Register("CrosshairSeriesLabelForeground", typeof(Brush), typeof(XYDiagram2D), new PropertyMetadata(null));
		public static readonly DependencyProperty RotatedProperty = DependencyPropertyManager.Register("Rotated", typeof(bool), typeof(XYDiagram2D), new PropertyMetadata(RotatedPropertyChanged));
		public static readonly DependencyProperty PaneOrientationProperty = DependencyPropertyManager.Register("PaneOrientation",
			typeof(Orientation), typeof(XYDiagram2D), new PropertyMetadata(Orientation.Vertical, ChartElementHelper.Update));
		public static readonly DependencyProperty DefaultPaneProperty = DependencyPropertyManager.Register("DefaultPane", typeof(Pane), typeof(XYDiagram2D), new PropertyMetadata(DefaultPanePropertyChanged));
		public static readonly DependencyProperty AxisXProperty = DependencyPropertyManager.Register("AxisX", typeof(AxisX2D), typeof(XYDiagram2D), new PropertyMetadata(AxisXPropertyChanged));
		public static readonly DependencyProperty AxisYProperty = DependencyPropertyManager.Register("AxisY", typeof(AxisY2D), typeof(XYDiagram2D), new PropertyMetadata(AxisYPropertyChanged));
		public static readonly DependencyProperty EnableAxisXNavigationProperty = DependencyPropertyManager.Register("EnableAxisXNavigation",
			typeof(bool), typeof(XYDiagram2D), new PropertyMetadata(EnableNavigationPropertyChanged));
		public static readonly DependencyProperty EnableAxisYNavigationProperty = DependencyPropertyManager.Register("EnableAxisYNavigation",
			typeof(bool), typeof(XYDiagram2D), new PropertyMetadata(EnableNavigationPropertyChanged));
		public static readonly DependencyProperty NavigationOptionsProperty = DependencyPropertyManager.Register("NavigationOptions",
			typeof(NavigationOptions), typeof(XYDiagram2D), new PropertyMetadata(null, NavigationOptionsPropertyChanged));
		public static readonly DependencyProperty BarDistanceProperty = DependencyPropertyManager.Register("BarDistance", typeof(double),
			typeof(XYDiagram2D), new PropertyMetadata(0.0, ChartElementHelper.Update), new ValidateValueCallback(BarDistanceValidation));
		public static readonly DependencyProperty BarDistanceFixedProperty = DependencyPropertyManager.Register("BarDistanceFixed", typeof(int),
			typeof(XYDiagram2D), new PropertyMetadata(1, ChartElementHelper.Update), new ValidateValueCallback(BarDistanceFixedValidation));
		public static readonly DependencyProperty EqualBarWidthProperty = DependencyPropertyManager.Register("EqualBarWidth",
			typeof(bool), typeof(XYDiagram2D), new PropertyMetadata(false, ChartElementHelper.Update));
		public static readonly DependencyProperty LabelsResolveOverlappingMinIndentProperty = DependencyPropertyManager.Register("LabelsResolveOverlappingMinIndent",
			typeof(int), typeof(XYDiagram2D), new PropertyMetadata(-1, ChartElementHelper.Update));
		public static readonly DependencyProperty SelectionTemplateProperty = DependencyPropertyManager.Register("SelectionTemplate", typeof(DataTemplate), typeof(XYDiagram2D));
		public static readonly DependencyProperty ElementsProperty = DependencyPropertyManager.Register("Elements", typeof(ObservableCollection<object>), typeof(XYDiagram2D));
		public static readonly DependencyProperty PanesPanelProperty = DependencyPropertyManager.Register("PanesPanel", typeof(ItemsPanelTemplate), typeof(XYDiagram2D));
		public static readonly DependencyProperty ElementTypeProperty = DependencyPropertyManager.RegisterAttached("ElementType",
			typeof(XYDiagram2DElementType), typeof(XYDiagram2D), new PropertyMetadata(XYDiagram2DElementType.None));
		public static readonly DependencyProperty SeriesPaneProperty = DependencyPropertyManager.RegisterAttached("SeriesPane",
			typeof(Pane), typeof(XYDiagram2D), new PropertyMetadata(null, SeriesPanePropertyChanged));
		public static readonly DependencyProperty SeriesAxisXProperty = DependencyPropertyManager.RegisterAttached("SeriesAxisX",
			typeof(SecondaryAxisX2D), typeof(XYDiagram2D), new PropertyMetadata(null, SeriesAxisXPropertyChanged));
		public static readonly DependencyProperty SeriesAxisYProperty = DependencyPropertyManager.RegisterAttached("SeriesAxisY",
			typeof(SecondaryAxisY2D), typeof(XYDiagram2D), new PropertyMetadata(null, SeriesAxisYPropertyChanged));
		public static readonly DependencyProperty IndicatorPaneProperty = DependencyPropertyManager.RegisterAttached("IndicatorPane",
			typeof(Pane), typeof(XYDiagram2D), new PropertyMetadata(null, IndicatorPanePropertyChanged));
		public static readonly DependencyProperty IndicatorAxisYProperty = DependencyPropertyManager.RegisterAttached("IndicatorAxisY",
			typeof(SecondaryAxisY2D), typeof(XYDiagram2D), new PropertyMetadata(null, IndicatorAxisYPropertyChanged));
		public static readonly RoutedEvent ScrollEvent = EventManager.RegisterRoutedEvent("Scroll", RoutingStrategy.Bubble, typeof(XYDiagram2DScrollEventHandler), typeof(XYDiagram2D));
		public static readonly RoutedEvent ZoomEvent = EventManager.RegisterRoutedEvent("Zoom", RoutingStrategy.Bubble, typeof(XYDiagram2DZoomEventHandler), typeof(XYDiagram2D));
		static XYDiagram2D() {
			Type ownerType = typeof(XYDiagram2D);
			CommandManager.RegisterClassCommandBinding(ownerType,
				new CommandBinding(XYDiagram2DCommands.ScrollHorizontally,
					(d, e) => ((XYDiagram2D)d).ScrollHorizontally((int)e.Parameter), (d, e) => ((XYDiagram2D)d).OnCanScrollHorizontally(e)));
			CommandManager.RegisterClassCommandBinding(ownerType,
				new CommandBinding(XYDiagram2DCommands.ScrollVertically,
					(d, e) => ((XYDiagram2D)d).ScrollVertically((int)e.Parameter), (d, e) => ((XYDiagram2D)d).OnCanScrollVertically(e)));
			CommandManager.RegisterClassCommandBinding(ownerType,
				new CommandBinding(XYDiagram2DCommands.ScrollAxisXTo,
					(d, e) => ((XYDiagram2D)d).ScrollAxisXTo((double)e.Parameter), (d, e) => ((XYDiagram2D)d).OnCanScrollAxisXTo(e)));
			CommandManager.RegisterClassCommandBinding(ownerType,
				new CommandBinding(XYDiagram2DCommands.ScrollAxisYTo,
					(d, e) => ((XYDiagram2D)d).ScrollAxisYTo((double)e.Parameter), (d, e) => ((XYDiagram2D)d).OnCanScrollAxisYTo(e)));
			CommandManager.RegisterClassCommandBinding(ownerType,
				new CommandBinding(XYDiagram2DCommands.SetAxisXRange,
					(d, e) => ((XYDiagram2D)d).SetAxisXRange((AxisRangePositions)e.Parameter), (d, e) => ((XYDiagram2D)d).OnCanSetAxisXRange(e)));
			CommandManager.RegisterClassCommandBinding(ownerType,
				new CommandBinding(XYDiagram2DCommands.SetAxisYRange,
					(d, e) => ((XYDiagram2D)d).SetAxisXRange((AxisRangePositions)e.Parameter), (d, e) => ((XYDiagram2D)d).OnCanSetAxisYRange(e)));
			CommandManager.RegisterClassCommandBinding(ownerType,
				new CommandBinding(XYDiagram2DCommands.SetAxisXZoomRatio,
					(d, e) => ((XYDiagram2D)d).SetAxisXZoomRatio((double)e.Parameter), (d, e) => ((XYDiagram2D)d).OnCanSetAxisXZoomRatio(e)));
			CommandManager.RegisterClassCommandBinding(ownerType,
				new CommandBinding(XYDiagram2DCommands.SetAxisYZoomRatio,
					(d, e) => ((XYDiagram2D)d).SetAxisYZoomRatio((double)e.Parameter), (d, e) => ((XYDiagram2D)d).OnCanSetAxisYZoomRatio(e)));
			CommandManager.RegisterClassCommandBinding(ownerType,
				new CommandBinding(XYDiagram2DCommands.ZoomIntoRectangle,
					(d, e) => ((XYDiagram2D)d).ZoomIntoRectangle((Rect)e.Parameter), (d, e) => ((XYDiagram2D)d).OnCanZoomIntoRectangle(e)));
			CommandManager.RegisterClassCommandBinding(ownerType,
				new CommandBinding(XYDiagram2DCommands.ZoomIn, (d, e) => ((XYDiagram2D)d).ZoomIn((Point?)e.Parameter), (d, e) => ((XYDiagram2D)d).OnCanZoomIn(e)));
			CommandManager.RegisterClassCommandBinding(ownerType,
				new CommandBinding(XYDiagram2DCommands.ZoomOut, (d, e) => ((XYDiagram2D)d).ZoomOut((Point?)e.Parameter), (d, e) => ((XYDiagram2D)d).OnCanZoomOut(e)));
			CommandManager.RegisterClassCommandBinding(ownerType,
				new CommandBinding(XYDiagram2DCommands.UndoZoom, (d, e) => ((XYDiagram2D)d).UndoZoom(), (d, e) => ((XYDiagram2D)d).OnCanUndoZoom(e)));
		}
		static bool BarDistanceValidation(object barDistance) {
			return (double)barDistance >= 0;
		}
		static bool BarDistanceFixedValidation(object barDistanceFixed) {
			return (int)barDistanceFixed >= 0;
		}
		static void RotatedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			XYDiagram2D diagram = d as XYDiagram2D;
			if (diagram != null) {
				foreach (Pane pane in diagram.ActualPanes)
					pane.UpdateScrollBarItemsOrientation();
			}
			ChartElementHelper.Update(d, e);
		}
		static void DefaultPanePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			XYDiagram2D diagram = d as XYDiagram2D;
			if (diagram != null) {
				Pane pane = e.NewValue as Pane;
				if (pane == null) {
					diagram.actualDefaultPane = new Pane();
					e = new DependencyPropertyChangedEventArgs(e.Property, e.OldValue, diagram.actualDefaultPane);
				}
				else
					diagram.actualDefaultPane = pane;
			}
			ChartElementHelper.ChangeOwnerAndUpdate(d, e, ChartElementChange.UpdateXYDiagram2DItems | ChartElementChange.UpdateActualPanes);
		}
		static void AxisXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			XYDiagram2D diagram = d as XYDiagram2D;
			if (diagram != null) {
				AxisX2D axis = e.NewValue as AxisX2D;
				AxisX2D oldAxis = diagram.actualAxisX;
				if (axis == null) {
					diagram.actualAxisX = new AxisX2D();
					e = new DependencyPropertyChangedEventArgs(e.Property, e.OldValue, diagram.actualAxisX);
				}
				else
					diagram.actualAxisX = axis;
				ChartElementHelper.ChangeOwner(d, e);
				ChartElementHelper.Update(d, new PropertyUpdateInfo<IAxisData>(diagram, "AxisX", oldAxis, axis), ChartElementChange.UpdateXYDiagram2DItems);
			}
		}
		static void AxisYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			XYDiagram2D diagram = d as XYDiagram2D;
			if (diagram != null) {
				AxisY2D axis = e.NewValue as AxisY2D;
				AxisY2D oldAxis = diagram.actualAxisY;
				if (axis == null) {
					diagram.actualAxisY = new AxisY2D();
					e = new DependencyPropertyChangedEventArgs(e.Property, e.OldValue, diagram.actualAxisY);
				}
				else
					diagram.actualAxisY = axis;
				ChartElementHelper.ChangeOwner(d, e);
				ChartElementHelper.Update(d, new PropertyUpdateInfo<IAxisData>(diagram, "AxisY", oldAxis, axis), ChartElementChange.UpdateXYDiagram2DItems);
			}
		}
		static void NavigationOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			XYDiagram2D diagram = d as XYDiagram2D;
			if (diagram != null)
				diagram.OnNavigationOptionsPropertyChanged(e.OldValue as NavigationOptions, e.NewValue as NavigationOptions);
		}
		static void EnableNavigationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			XYDiagram2D diagram = d as XYDiagram2D;
			if (diagram != null)
				foreach (Pane pane in diagram.ActualPanes) {
					ZoomCacheEx zoomCache = pane.ZoomCacheEx;
					if (zoomCache != null)
						zoomCache.Clear();
				}
			ChartElementHelper.Update(d, e);
		}
		static void SeriesPanePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartElementHelper.Update(d, new PropertyUpdateInfo<IAxisData>(d, "Pane", e.OldValue as IAxisData, e.NewValue as IAxisData), ChartElementChange.UpdatePanesItems);
		}
		static void SeriesAxisXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			XYSeries series = d as XYSeries;
			if (series != null)
				series.AxisXInternal = e.NewValue as SecondaryAxisX2D;
			ChartElementHelper.Update(d, new PropertyUpdateInfo<IAxisData>(d, "AxisX", e.OldValue as IAxisData, e.NewValue as IAxisData), ChartElementChange.UpdatePanesItems);
		}
		static void SeriesAxisYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			XYSeries series = d as XYSeries;
			if (series != null)
				series.AxisYInternal = e.NewValue as SecondaryAxisY2D;
			ChartElementHelper.Update(d, new PropertyUpdateInfo<IAxisData>(d, "AxisY", e.OldValue as IAxisData, e.NewValue as IAxisData), ChartElementChange.UpdatePanesItems);
		}
		static void IndicatorPanePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SeparatePaneIndicator indicator = d as SeparatePaneIndicator;
			if (indicator != null)
				indicator.PaneInternal = e.NewValue as Pane;
			ChartElementHelper.Update(d, new PropertyUpdateInfo<IAxisData>(d, "Pane", e.OldValue as IAxisData, e.NewValue as IAxisData), ChartElementChange.UpdatePanesItems);
		}
		static void IndicatorAxisYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SeparatePaneIndicator indicator = d as SeparatePaneIndicator;
			if (indicator != null)
				indicator.AxisYInternal = e.NewValue as SecondaryAxisY2D;
			ChartElementHelper.Update(d, new PropertyUpdateInfo<IAxisData>(d, "AxisY", e.OldValue as IAxisData, e.NewValue as IAxisData), ChartElementChange.UpdatePanesItems);
		}
		[NonCategorized]
		public static XYDiagram2DElementType GetElementType(UIElement element) {
			return (XYDiagram2DElementType)element.GetValue(ElementTypeProperty);
		}
		public static void SetElementType(UIElement element, XYDiagram2DElementType elementType) {
			element.SetValue(ElementTypeProperty, elementType);
		}
		[Category(Categories.Layout)]
		public static Pane GetSeriesPane(XYSeries series) {
			return (Pane)series.GetValue(SeriesPaneProperty);
		}
		public static void SetSeriesPane(XYSeries series, Pane pane) {
			series.SetValue(SeriesPaneProperty, pane);
		}
		[Category(Categories.Layout)]
		public static SecondaryAxisX2D GetSeriesAxisX(XYSeries series) {
			return (SecondaryAxisX2D)series.GetValue(SeriesAxisXProperty);
		}
		public static void SetSeriesAxisX(XYSeries series, SecondaryAxisX2D axis) {
			series.SetValue(SeriesAxisXProperty, axis);
		}
		[Category(Categories.Layout)]
		public static SecondaryAxisY2D GetSeriesAxisY(XYSeries series) {
			return (SecondaryAxisY2D)series.GetValue(SeriesAxisYProperty);
		}
		public static void SetSeriesAxisY(XYSeries series, SecondaryAxisY2D axis) {
			series.SetValue(SeriesAxisYProperty, axis);
		}
		[Category(Categories.Layout)]
		public static Pane GetIndicatorPane(SeparatePaneIndicator indicator) {
			return (Pane)indicator.GetValue(IndicatorPaneProperty);
		}
		public static void SetIndicatorPane(SeparatePaneIndicator indicator, Pane pane) {
			indicator.SetValue(IndicatorPaneProperty, pane);
		}
		[Category(Categories.Layout)]
		public static SecondaryAxisY2D GetIndicatorAxisY(SeparatePaneIndicator indicator) {
			return (SecondaryAxisY2D)indicator.GetValue(IndicatorAxisYProperty);
		}
		public static void SetIndicatorAxisY(SeparatePaneIndicator indicator, SecondaryAxisY2D axis) {
			indicator.SetValue(IndicatorAxisYProperty, axis);
		}
		readonly ZoomCacheEx zoomCacheEx = new ZoomCacheEx();
		readonly List<DelegateCommand<object>> commands = new List<DelegateCommand<object>>();
		readonly SecondaryAxisXCollection secondaryAxesX;
		readonly SecondaryAxisYCollection secondaryAxesY;
		readonly Dictionary<IPane, List<Indicator>> indicatorsRepository = new Dictionary<IPane, List<Indicator>>();
		PanesPanelLayoutManager panesPanelManager;
		bool useViewportCache;
		double offsetX;
		double offsetY;
		AxisX2D actualAxisX;
		AxisY2D actualAxisY;
		ChartItemsControl paneItemsControl;
		Panel panesRootPanel;
		Pane actualDefaultPane;
		Pane paneUnderMouse;
		Point lastPosition;
		Point startRectanglePoint;
		Rect lastPrimaryViewport = Rect.Empty;
		AxisScrollBar focusedScrollBar;
		AxisScrollBarThumbResizer focusedResizer;
		AxisScrollBarThumb focusedThumb;
		Point? manipulationStartPoint;
		double manipulationStartAxisXMin;
		double manipulationStartAxisXMax;
		double manipulationStartAxisYMin;
		double manipulationStartAxisYMax;
		List<ISeriesPoint> highlightedPoints = new List<ISeriesPoint>();
		CrosshairManager crosshairManager;
		PaneAxesContainerRepository paneAxesContainerRepository;
		AxisPaneContainer axisPaneRepository;
		CrosshairInfoEx lastCrosshairInfo;
		Point? lastCrosshairLocation = null;
		Point preRenderOffset = new Point(0, 0);
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DSecondaryAxesX"),
#endif
		Category(Categories.Elements),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)
		]
		public SecondaryAxisXCollection SecondaryAxesX { get { return (SecondaryAxisXCollection)GetValue(SecondaryAxesXProperty); } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DSecondaryAxesY"),
#endif
		Category(Categories.Elements),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)
		]
		public SecondaryAxisYCollection SecondaryAxesY { get { return (SecondaryAxisYCollection)GetValue(SecondaryAxesYProperty); } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DPanes"),
#endif
		Category(Categories.Elements),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)
		]
		public PaneCollection Panes { get { return (PaneCollection)GetValue(PanesProperty); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ObservableCollection<Pane> ActualPanes { get { return (ObservableCollection<Pane>)GetValue(ActualPanesProperty); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Brush CrosshairSeriesLabelForeground {
			get { return (Brush)GetValue(CrosshairSeriesLabelForegroundProperty); }
			set { SetValue(CrosshairSeriesLabelForegroundProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DRotated"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool Rotated {
			get { return (bool)GetValue(RotatedProperty); }
			set { SetValue(RotatedProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DPaneOrientation"),
#endif
		Category(Categories.Layout),
		XtraSerializableProperty
		]
		public Orientation PaneOrientation {
			get { return (Orientation)GetValue(PaneOrientationProperty); }
			set { SetValue(PaneOrientationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DDefaultPane"),
#endif
		Category(Categories.Elements),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Pane DefaultPane {
			get { return (Pane)GetValue(DefaultPaneProperty); }
			set { SetValue(DefaultPaneProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DAxisX"),
#endif
		Category(Categories.Elements),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public AxisX2D AxisX {
			get { return (AxisX2D)GetValue(AxisXProperty); }
			set { SetValue(AxisXProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DAxisY"),
#endif
		Category(Categories.Elements),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public AxisY2D AxisY {
			get { return (AxisY2D)GetValue(AxisYProperty); }
			set { SetValue(AxisYProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DEnableAxisXNavigation"),
#endif
		Category(Categories.Navigation),
		XtraSerializableProperty
		]
		public bool EnableAxisXNavigation {
			get { return (bool)GetValue(EnableAxisXNavigationProperty); }
			set { SetValue(EnableAxisXNavigationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DEnableAxisYNavigation"),
#endif
		Category(Categories.Navigation),
		XtraSerializableProperty
		]
		public bool EnableAxisYNavigation {
			get { return (bool)GetValue(EnableAxisYNavigationProperty); }
			set { SetValue(EnableAxisYNavigationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DNavigationOptions"),
#endif
		Category(Categories.Navigation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public NavigationOptions NavigationOptions {
			get { return (NavigationOptions)GetValue(NavigationOptionsProperty); }
			set { SetValue(NavigationOptionsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DBarDistance"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public double BarDistance {
			get { return (double)GetValue(BarDistanceProperty); }
			set { SetValue(BarDistanceProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DBarDistanceFixed"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public int BarDistanceFixed {
			get { return (int)GetValue(BarDistanceFixedProperty); }
			set { SetValue(BarDistanceFixedProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DEqualBarWidth"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool EqualBarWidth {
			get { return (bool)GetValue(EqualBarWidthProperty); }
			set { SetValue(EqualBarWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DLabelsResolveOverlappingMinIndent"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public int LabelsResolveOverlappingMinIndent {
			get { return (int)GetValue(LabelsResolveOverlappingMinIndentProperty); }
			set { SetValue(LabelsResolveOverlappingMinIndentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DSelectionTemplate"),
#endif
 Category(Categories.Presentation)]
		public DataTemplate SelectionTemplate {
			get { return (DataTemplate)GetValue(SelectionTemplateProperty); }
			set { SetValue(SelectionTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DPanesPanel"),
#endif
 Category(Categories.Layout)]
		public ItemsPanelTemplate PanesPanel {
			get { return (ItemsPanelTemplate)GetValue(PanesPanelProperty); }
			set { SetValue(PanesPanelProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ObservableCollection<object> Elements {
			get { return (ObservableCollection<object>)GetValue(ElementsProperty); }
			set { SetValue(ElementsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DScroll"),
#endif
 Category(Categories.Behavior)]
		public event XYDiagram2DScrollEventHandler Scroll {
			add { AddHandler(ScrollEvent, value); }
			remove { RemoveHandler(ScrollEvent, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DZoom"),
#endif
 Category(Categories.Behavior)]
		public event XYDiagram2DZoomEventHandler Zoom {
			add { AddHandler(ZoomEvent, value); }
			remove { RemoveHandler(ZoomEvent, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DScrollHorizontallyCommand"),
#endif
 Category(Categories.Action)]
		public ICommand ScrollHorizontallyCommand { get; private set; }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DScrollVerticallyCommand"),
#endif
 Category(Categories.Action)]
		public ICommand ScrollVerticallyCommand { get; private set; }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DScrollAxisXToCommand"),
#endif
 Category(Categories.Action)]
		public ICommand ScrollAxisXToCommand { get; private set; }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DScrollAxisYToCommand"),
#endif
 Category(Categories.Action)]
		public ICommand ScrollAxisYToCommand { get; private set; }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DSetAxisXRangeCommand"),
#endif
 Category(Categories.Action)]
		public ICommand SetAxisXRangeCommand { get; private set; }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DSetAxisYRangeCommand"),
#endif
 Category(Categories.Action)]
		public ICommand SetAxisYRangeCommand { get; private set; }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DSetAxisXZoomRatioCommand"),
#endif
 Category(Categories.Action)]
		public ICommand SetAxisXZoomRatioCommand { get; private set; }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DSetAxisYZoomRatioCommand"),
#endif
 Category(Categories.Action)]
		public ICommand SetAxisYZoomRatioCommand { get; private set; }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DZoomIntoRectangleCommand"),
#endif
 Category(Categories.Action)]
		public ICommand ZoomIntoRectangleCommand { get; private set; }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DZoomInCommand"),
#endif
 Category(Categories.Action)]
		public ICommand ZoomInCommand { get; private set; }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DZoomOutCommand"),
#endif
 Category(Categories.Action)]
		public ICommand ZoomOutCommand { get; private set; }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DUndoZoomCommand"),
#endif
 Category(Categories.Action)]
		public ICommand UndoZoomCommand { get; private set; }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("XYDiagram2DActualDefaultPane")]
#endif
		public Pane ActualDefaultPane { get { return actualDefaultPane; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("XYDiagram2DActualAxisX")]
#endif
		public AxisX2D ActualAxisX { get { return actualAxisX; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("XYDiagram2DActualAxisY")]
#endif
		public AxisY2D ActualAxisY { get { return actualAxisY; } }
		IEnumerable<Axis2D> Axes {
			get {
				yield return actualAxisX;
				foreach (SecondaryAxisX2D axisX in SecondaryAxesXInternal)
					yield return axisX;
				yield return actualAxisY;
				foreach (SecondaryAxisY2D axisY in SecondaryAxesYInternal)
					yield return axisY;
			}
		}
		Pane FocusedPane {
			get {
				ChartControl chart = ChartControl;
				Pane result = chart != null ? FocusManager.GetFocusedElement(chart) as Pane : null;
				if (result == null || ((IOwnedElement)result).Owner == null || ((IOwnedElement)result).Owner != this)
					return null;
				else
					return result;
			}
		}
		bool IsAxisLabelItemsMeasured {
			get {
				foreach (Pane pane in ActualPanes)
					foreach (Axis2DItem axisItem in pane.AxisItems) {
						IEnumerable<AxisLabelItem> items = axisItem.LabelItems;
						if (items != null)
							foreach (AxisLabelItem labelItem in items)
								if (labelItem.Size.IsEmpty)
									return false;
					}
				return true;
			}
		}
		internal bool UseViewportCache { get { return useViewportCache; } }
		protected internal override bool IsKeyboardNavigationEnabled { get { return ActualNavigationOptions.UseKeyboard; } }
		protected internal override bool IsMouseNavigationEnabled { get { return ActualNavigationOptions.UseMouse; } }
		protected internal override bool IsManipulationNavigationEnabled { get { return ActualNavigationOptions.UseTouchDevice; } }
		protected internal override Rect ActualViewport { get { return actualDefaultPane.Viewport; } }
		internal bool IsNavigationEnabled { get { return IsNavigationAxisXEnabled || IsNavigationAxisYEnabled; } }
		internal bool IsNavigationAxisXEnabled {
			get {
				foreach (Pane pane in ActualPanes)
					if (pane.EnableAxisXNavigation.HasValue && pane.EnableAxisXNavigation.Value)
						return true;
				return EnableAxisXNavigation;
			}
		}
		internal bool IsNavigationAxisYEnabled {
			get {
				foreach (Pane pane in ActualPanes)
					if (pane.EnableAxisYNavigation.HasValue && pane.EnableAxisYNavigation.Value)
						return true;
				return EnableAxisYNavigation;
			}
		}
		internal Panel PanesRootPanel {
			get {
				if (paneItemsControl != null && panesRootPanel == null)
					panesRootPanel = LayoutHelper.FindElement(paneItemsControl, element => element is Panel) as Panel;
				return panesRootPanel;
			}
		}
		internal PaneAxesContainerRepository PaneAxesContainerRepository { get { return paneAxesContainerRepository; } }
		internal AxisPaneContainer AxisPaneContainer { get { return axisPaneRepository; } }
		internal ZoomCacheEx ZoomCacheEx { get { return zoomCacheEx; } }
		internal NavigationOptions ActualNavigationOptions { get { return NavigationOptions ?? new NavigationOptions(); } }
		internal ChartItemsControl PaneItemsControl { get { return paneItemsControl; } }
		internal SecondaryAxisXCollection SecondaryAxesXInternal { get { return secondaryAxesX; } }
		internal SecondaryAxisYCollection SecondaryAxesYInternal { get { return secondaryAxesY; } }
		protected internal override CompatibleViewType CompatibleViewType { get { return CompatibleViewType.XYView; } }
		internal Dictionary<IPane, List<Indicator>> IndicatorsRepository { get { return indicatorsRepository; } }
		public XYDiagram2D() : base() {
			DefaultStyleKey = typeof(XYDiagram2D);
			actualDefaultPane = new Pane();
			actualDefaultPane.ChangeOwner(null, this);
			actualAxisX = new AxisX2D();
			actualAxisX.ChangeOwner(null, this);
			actualAxisY = new AxisY2D();
			actualAxisY.ChangeOwner(null, this);
			this.SetValue(SecondaryAxesXPropertyKey, ChartElementHelper.CreateInstance<SecondaryAxisXCollection>(this));
			this.SetValue(SecondaryAxesYPropertyKey, ChartElementHelper.CreateInstance<SecondaryAxisYCollection>(this));
			this.SetValue(PanesPropertyKey, ChartElementHelper.CreateInstance<PaneCollection>(this));
			this.secondaryAxesX = SecondaryAxesX;
			this.secondaryAxesY = SecondaryAxesY;
			UpdateActualPanes(true);
			crosshairManager = new CrosshairManager(this);
			ScrollHorizontallyCommand = CreateDelegateCommand(parameter => ScrollHorizontally((int)parameter), parameter => CanScrollHorizontally((int)parameter));
			ScrollVerticallyCommand = CreateDelegateCommand(parameter => ScrollVertically((int)parameter), parameter => CanScrollVertically((int)parameter));
			ScrollAxisXToCommand = CreateDelegateCommand(parameter => ScrollAxisXTo((double)parameter), parameter => CanScrollAxisXTo((double)parameter));
			ScrollAxisYToCommand = CreateDelegateCommand(parameter => ScrollAxisYTo((double)parameter), parameter => CanScrollAxisYTo((double)parameter));
			SetAxisXRangeCommand = CreateDelegateCommand(parameter => SetAxisXRange((AxisRangePositions)parameter), parameter => CanSetAxisXRange());
			SetAxisYRangeCommand = CreateDelegateCommand(parameter => SetAxisYRange((AxisRangePositions)parameter), parameter => CanSetAxisYRange());
			SetAxisXZoomRatioCommand = CreateDelegateCommand(parameter => SetAxisXZoomRatio((double)parameter), parameter => CanSetAxisXZoomRatio());
			SetAxisYZoomRatioCommand = CreateDelegateCommand(parameter => SetAxisYZoomRatio((double)parameter), parameter => CanSetAxisYZoomRatio());
			ZoomIntoRectangleCommand = CreateDelegateCommand(parameter => ZoomIntoRectangle((Rect)parameter), parameter => CanZoomIntoRectangle());
			ZoomInCommand = CreateDelegateCommand(parameter => ZoomIn((Point?)parameter), parameter => CanZoomIn());
			ZoomOutCommand = CreateDelegateCommand(parameter => ZoomOut((Point?)parameter), parameter => CanZoomOut());
			UndoZoomCommand = CreateDelegateCommand(parameter => UndoZoom(), parameter => CanUndoZoom());
			EndInit();
		}
		#region IXYDiagram implementation
		bool IXYDiagram.Rotated {
			get { return this.Rotated; }
		}
		void IXYDiagram.UpdateCrosshairData(IList<RefinedSeries> seriesCollection) {
			crosshairManager.UpdateCrosshairData(seriesCollection);
		}
		IList<IPane> IXYDiagram.Panes {
			get {
				List<IPane> panes = new List<IPane>(ActualPanes.Count);
				panes.AddRange(ActualPanes);
				return panes;
			}
		}
		IAxisData IXYDiagram.AxisX { get { return actualAxisX; } }
		IAxisData IXYDiagram.AxisY { get { return actualAxisY; } }
		IEnumerable<IAxisData> IXYDiagram.SecondaryAxesX { get { return SecondaryAxesXInternal; } }
		IEnumerable<IAxisData> IXYDiagram.SecondaryAxesY { get { return SecondaryAxesYInternal; } }
		bool IXYDiagram.ScrollingEnabled { get { return IsNavigationEnabled; } }
		ICrosshairOptions IXYDiagram.CrosshairOptions {
			get {
				ChartControl chart = ChartControl;
				return chart != null ? chart.ActualCrosshairOptions : null;
			}
		}
		IList<IPane> IXYDiagram.GetCrosshairSyncPanes(IPane focusedPane, bool isHorizontalSync) {
			return GetCrosshairSyncPanes(focusedPane, isHorizontalSync);
		}
		InternalCoordinates IXYDiagram.MapPointToInternal(IPane pane, GRealPoint2D point) {
			return MapPointToInternal((Pane)pane, new Point(point.X, point.Y));
		}
		GRealPoint2D IXYDiagram.MapInternalToPoint(IPane pane, IAxisData axisX, IAxisData axisY, double argument, double value) {
			ControlCoordinates coords = MapInternalToPoint((Pane)pane, (AxisX2D)axisX, (AxisY2D)axisY, argument, value);
			return new GRealPoint2D(coords.Point.X, coords.Point.Y);
		}
		List<IPaneAxesContainer> IXYDiagram.GetPaneAxesContainers(IList<RefinedSeries> activeSeries) {
			IList<IPane> actualPanes = new List<IPane>();
			foreach (IPane pane in ActualPanes)
				actualPanes.Add(pane);
			paneAxesContainerRepository = new PaneAxesContainerRepository(ActualAxisX, ActualAxisY, SecondaryAxesXInternal, SecondaryAxesYInternal, actualPanes, activeSeries);
			axisPaneRepository = new AxisPaneContainer(paneAxesContainerRepository);
			List<IPaneAxesContainer> containers = new List<IPaneAxesContainer>();
			foreach (IPane pane in actualPanes) {
				IPaneAxesContainer container = paneAxesContainerRepository.GetContaiter(pane);
				if (container != null)
					containers.Add(container);
			}
			return containers;
		}
		void IXYDiagram.UpdateAutoMeasureUnits() {
		}
		int IXYDiagram.GetAxisXLength(IAxisData axis) {
			return 0;
		}
		#endregion
		#region IWeakEventListener impementation
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if (managerType == typeof(PropertyChangedWeakEventManager)) {
				if (sender is NavigationOptions) {
					PropertyChangedEventArgs args = e as PropertyChangedEventArgs;
					if (args != null && args.PropertyName == "UseScrollBars")
						UpdateScrollBarItems();
					return true;
				}
			}
			return false;
		}
		#endregion
		public void ScrollHorizontally(int dx) {
			actualDefaultPane.ScrollHorizontally(dx);
		}
		public bool CanScrollHorizontally(int dx) {
			return actualDefaultPane.CanScrollHorizontally(dx);
		}
		public void ScrollVertically(int dy) {
			actualDefaultPane.ScrollVertically(dy);
		}
		public bool CanScrollVertically(int dy) {
			return actualDefaultPane.CanScrollVertically(dy);
		}
		public void ScrollAxisXTo(double xPosition) {
			actualDefaultPane.ScrollAxisXTo(xPosition);
		}
		public bool CanScrollAxisXTo(double xPosition) {
			return actualDefaultPane.CanScrollAxisXTo(xPosition);
		}
		public void ScrollAxisYTo(double yPosition) {
			actualDefaultPane.ScrollAxisYTo(yPosition);
		}
		public bool CanScrollAxisYTo(double yPosition) {
			return actualDefaultPane.CanScrollAxisYTo(yPosition);
		}
		public void SetAxisXRange(AxisRangePositions positions) {
			actualDefaultPane.SetAxisXRange(positions);
		}
		public bool CanSetAxisXRange() {
			return actualDefaultPane.CanSetAxisXRange();
		}
		public void SetAxisYRange(AxisRangePositions positions) {
			actualDefaultPane.SetAxisYRange(positions);
		}
		public bool CanSetAxisYRange() {
			return actualDefaultPane.CanSetAxisYRange();
		}
		public void SetAxisXZoomRatio(double ratio) {
			actualDefaultPane.SetAxisXZoomRatio(ratio);
		}
		public bool CanSetAxisXZoomRatio() {
			return actualDefaultPane.CanSetAxisXZoomRatio();
		}
		public void SetAxisYZoomRatio(double ratio) {
			actualDefaultPane.SetAxisYZoomRatio(ratio);
		}
		public bool CanSetAxisYZoomRatio() {
			return actualDefaultPane.CanSetAxisYZoomRatio();
		}
		public void ZoomIntoRectangle(Rect rectangle) {
			actualDefaultPane.ZoomIntoRectangle(rectangle);
		}
		public bool CanZoomIntoRectangle() {
			return actualDefaultPane.CanZoomIntoRectangle();
		}
		public void ZoomIn(Point? zoomLocation) {
			actualDefaultPane.ZoomIn(zoomLocation);
		}
		public bool CanZoomIn() {
			return actualDefaultPane.CanZoomIn();
		}
		public void ZoomOut(Point? zoomLocation) {
			actualDefaultPane.ZoomOut(zoomLocation);
		}
		public bool CanZoomOut() {
			return actualDefaultPane.CanZoomOut();
		}
		public void UndoZoom() {
			zoomCacheEx.Pop();
		}
		public bool CanUndoZoom() {
			return !zoomCacheEx.IsEmpty;
		}
		public bool ShouldSerializeElements(XamlDesignerSerializationManager manager) {
			return false;
		}
		public bool ShouldSerializeActualPanes(XamlDesignerSerializationManager manager) {
			return false;
		}
		void OnCanScrollHorizontally(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanScrollHorizontally((int)e.Parameter);
		}
		void OnCanScrollVertically(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanScrollVertically((int)e.Parameter);
		}
		void OnCanScrollAxisXTo(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanScrollAxisXTo((double)e.Parameter);
		}
		void OnCanScrollAxisYTo(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanScrollAxisYTo((double)e.Parameter);
		}
		void OnCanSetAxisXRange(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanSetAxisXRange();
		}
		void OnCanSetAxisYRange(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanSetAxisYRange();
		}
		void OnCanSetAxisXZoomRatio(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanSetAxisXZoomRatio();
		}
		void OnCanSetAxisYZoomRatio(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanSetAxisYZoomRatio();
		}
		void OnCanZoomIntoRectangle(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanZoomIntoRectangle();
		}
		void OnCanZoomIn(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanZoomIn();
		}
		void OnCanZoomOut(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanZoomOut();
		}
		void OnCanUndoZoom(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanUndoZoom();
		}
		Point GetOverPan(int dx, int dy) {
			Pane focusedPane = FocusedPane;
			if (focusedPane == null)
				return new Point(dx, dy);
			Rect viewport = focusedPane.Viewport;
			viewport.X += dx;
			viewport.Y += dy;
			Rect rangeBounds = focusedPane.RangeBounds;
			double overPanX = 0;
			double overPanY = 0;
			if (viewport.Left < rangeBounds.Left)
				overPanX = viewport.Left - rangeBounds.Left;
			else if (viewport.Right > rangeBounds.Right)
				overPanX = viewport.Right - rangeBounds.Right;
			if (viewport.Top < rangeBounds.Top)
				overPanY = viewport.Top - rangeBounds.Top;
			else if (viewport.Bottom > rangeBounds.Bottom)
				overPanY = viewport.Bottom - rangeBounds.Bottom;
			if (focusedScrollBar != null)
				return focusedScrollBar.Orientation == Orientation.Horizontal ? new Point(overPanX, 0) : new Point(0, overPanY);
			return new Point(-overPanX, -overPanY);
		}
		HitTestResultBehavior UpdateFocusResult(HitTestResult result) {
			return HitTestResultBehavior.Continue;
		}
		HitTestFilterBehavior UpdatePaneUnderMouseFilter(DependencyObject obj) {
			Pane pane = obj as Pane;
			if (pane != null && pane.Diagram.ChartControl == ChartControl) {
				paneUnderMouse = pane;
				return HitTestFilterBehavior.Stop;
			}
			return HitTestFilterBehavior.Continue;
		}
		HitTestFilterBehavior UpdateFocusFilter(DependencyObject obj) {
			Pane pane = obj as Pane;
			if (pane != null && pane.Diagram.ChartControl == ChartControl) {
				pane.Focus();
				paneUnderMouse = pane;
			}
			AxisScrollBar scrollBar = obj as AxisScrollBar;
			if (scrollBar != null)
				focusedScrollBar = scrollBar;
			AxisScrollBarThumb thumb = obj as AxisScrollBarThumb;
			if (thumb != null)
				focusedThumb = thumb;
			AxisScrollBarThumbResizer resizer = obj as AxisScrollBarThumbResizer;
			if (resizer != null) {
				focusedResizer = resizer;
				return HitTestFilterBehavior.Stop;
			}
			return HitTestFilterBehavior.Continue;
		}
		bool InViewport(Point position, Pane pane) {
			if (pane == null)
				return false;
			Rect paneRect = LayoutHelper.GetRelativeElementRect(pane, ChartControl);
			Rect viewport = pane.Viewport;
			viewport = new Rect(paneRect.X + viewport.X, paneRect.Y + viewport.Y, viewport.Width, viewport.Height);
			return position.X > viewport.Left && position.Y > viewport.Top && position.X < viewport.Right - 1 && position.Y < viewport.Bottom - 1;
		}
		bool CanScrollPane(Pane pane) {
			return pane != null && (pane.CanScrollHorizontally(1) || pane.CanScrollHorizontally(-1) || pane.CanScrollVertically(1) || pane.CanScrollVertically(-1));
		}
		bool ShouldChangeScrollBarsCommands(NavigationOptions oldOptions, NavigationOptions newOptions) {
			if (newOptions == null)
				return oldOptions != null && !oldOptions.UseScrollBars;
			if (oldOptions == null)
				return !newOptions.UseScrollBars;
			return oldOptions.UseScrollBars != newOptions.UseScrollBars;
		}
		DelegateCommand<object> CreateDelegateCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod) {
			DelegateCommand<object> command = DelegateCommandFactory.Create<object>(executeMethod, canExecuteMethod, false);
			commands.Add(command);
			return command;
		}
		void UpdateScrollBarItems() {
			foreach (Pane pane in ActualPanes)
				pane.UpdateScrollBarItems();
		}
		void OnNavigationOptionsPropertyChanged(NavigationOptions oldOptions, NavigationOptions newOptions) {
			CommonUtils.SubscribePropertyChangedWeakEvent(oldOptions, newOptions, this);
			if (ShouldChangeScrollBarsCommands(oldOptions, newOptions))
				UpdateScrollBarItems();
		}
		void HitTest(Point position, HitTestFilterCallback filter) {
			VisualTreeHelper.HitTest(ChartControl, filter, new HitTestResultCallback(UpdateFocusResult), new PointHitTestParameters(position));
		}
		void UpdateFocus(Point position) {
			focusedScrollBar = null;
			focusedThumb = null;
			focusedResizer = null;
			HitTest(position, new HitTestFilterCallback(UpdateFocusFilter));
		}
		double GetZeroPointOffset(IAxisData axis, double axisLengthy) {
			IMinMaxValues wholeRange = axis.WholeRange;
			double delta = wholeRange.Min - axis.AxisScaleTypeMap.NativeToInternal(((IAxisRangeData)wholeRange).MinValue);
			return delta * axisLengthy / wholeRange.Delta;
		}
		PredefinedBar2DModel GetModelFromSeries(Series series) {
			BarSeries2D barSeries = series as BarSeries2D;
			if (barSeries != null) {
				Bar2DModel barModel = barSeries.Model;
				if (barModel != null)
					return barModel as PredefinedBar2DModel;
			}
			return null;
		}
		Rect GetCorrectionRectForLabel(SeriesLabel label) {
			Rect correctionRect = Rect.Empty;
			if (label.ResolveOverlappingMode == ResolveOverlappingMode.None || label.ResolveOverlappingMode == ResolveOverlappingMode.HideOverlapped) {
				IEnumerable<SeriesLabelItem> items = label.Items;
				if (items != null)
					foreach (SeriesLabelItem labelItem in items)
						correctionRect = GraphicsUtils.UnionRect(correctionRect, labelItem.Layout != null ? labelItem.Layout.Bounds : Rect.Empty);
			}
			return correctionRect;
		}
		Rect GetCorrectionRectForModel(PredefinedBar2DModel model, Point controlPoint, bool yAxisReversed, bool rotated) {
			if (model != null) {
				int correctionSize = yAxisReversed ? model.ReversedCorrectionSize : model.CorrectionSize;
				if (correctionSize > 0) {
					if (yAxisReversed) {
						if (rotated)
							return new Rect(controlPoint, new Size(correctionSize, 1));
						else
							return new Rect(new Point(controlPoint.X, controlPoint.Y - correctionSize), new Size(1, correctionSize));
					}
					else {
						if (rotated)
							return new Rect(new Point(controlPoint.X - correctionSize, controlPoint.Y), new Size(correctionSize, 1));
						else
							return new Rect(controlPoint, new Size(1, correctionSize));
					}
				}
			}
			return Rect.Empty;
		}
		void ApplyRangesStates(IAxisData axis, MinMaxValues correctedRange) {
			if (!correctedRange.HasValues)
				return;
			IVisualAxisRangeData visualRange = axis.VisualRange;
			IWholeAxisRangeData wholeRange = axis.WholeRange;
			if (wholeRange.CorrectionMode == RangeCorrectionMode.Auto) {
				wholeRange.Min = correctedRange.Min;
				wholeRange.Max = correctedRange.Max;
				if (visualRange.CorrectionMode == RangeCorrectionMode.Auto) {
					visualRange.Min = correctedRange.Min;
					visualRange.Max = correctedRange.Max;
				}
			}
		}
		Rect GetCorrectionRect(List<Series> seriesList, bool rotated, Point zeroPointForScrollingRange, bool yAxisReversed) {
			Rect correctionRect = Rect.Empty;
			foreach (Series series in seriesList) {
				Rect correctionRectForLabel = GetCorrectionRectForLabel(series.ActualLabel);
				correctionRect = GraphicsUtils.UnionRect(correctionRect, correctionRectForLabel);
				Rect correctionRectForModel = GetCorrectionRectForModel(GetModelFromSeries(series), zeroPointForScrollingRange, yAxisReversed, rotated);
				correctionRect = GraphicsUtils.UnionRect(correctionRect, correctionRectForModel);
			}
			return correctionRect;
		}
		void CheckOutOfBounds(OutOfBoundsCheckerEx checker, IList<ISeries> seriesOnPane, bool rotated, Point zeroPointForScrollingRange, bool yAxisReversed) {
			List<Series> seriesForAxis = GetSeriesForAxis(checker.Axis, seriesOnPane);
			Rect correctionRect = GetCorrectionRect(seriesForAxis, rotated, zeroPointForScrollingRange, yAxisReversed);
			if (correctionRect.IsEmpty)
				return;
			GRealRect2D rect = new GRealRect2D(correctionRect.X, correctionRect.Y, correctionRect.Width, correctionRect.Height);
			checker.CheckOutOfBounds(rect);
		}
		List<Series> GetSeriesForAxis(IAxisData axis, IList<ISeries> seriesOnPane) {
			IList<ISeries> allSeries = ViewController.SeriesController.GetSeriesByAxis(axis);
			List<Series> seriesList = new List<Series>();
			foreach (ISeries series in allSeries) {
				if (seriesOnPane.Contains(series))
					seriesList.Add(series as Series);
			}
			return seriesList;
		}
		void CorrectRangesBySeriesLabels(Pane pane, IList<ISeries> seriesOnPane, bool rotated, Point zeroPointForScrollingRange, bool yAxisReversed) {
			PaneAxesContainer paneAxesData = pane.PaneAxesContainer;
			GRect2D paneRect = GraphicsUtils.ConvertRect(pane.Viewport);
			List<OutOfBoundsCheckerEx> checkers = new List<OutOfBoundsCheckerEx>();
			checkers.Add(new OutOfBoundsCheckerEx(paneAxesData.PrimaryAxisX, !pane.Rotated, paneRect, paneAxesData.PrimaryAxisX.WholeRange));
			checkers.Add(new OutOfBoundsCheckerEx(paneAxesData.PrimaryAxisY, pane.Rotated, paneRect, paneAxesData.PrimaryAxisY.WholeRange));
			foreach (IAxisData axis in paneAxesData.SecondaryAxesX)
				checkers.Add(new OutOfBoundsCheckerEx(axis, !pane.Rotated, paneRect, axis.WholeRange));
			foreach (IAxisData axis in paneAxesData.SecondaryAxesY)
				checkers.Add(new OutOfBoundsCheckerEx(axis, !pane.Rotated, paneRect, axis.WholeRange));
			foreach (OutOfBoundsCheckerEx checker in checkers)
				CheckOutOfBounds(checker, seriesOnPane, rotated, zeroPointForScrollingRange, yAxisReversed);
			Dictionary<IAxisData, MinMaxValues> correctedRanges = new Dictionary<IAxisData, MinMaxValues>();
			foreach (OutOfBoundsCheckerEx checker in checkers)
				correctedRanges.Add(checker.Axis, checker.GetCorrectedWholeRange());
			foreach (IAxisData axis in correctedRanges.Keys)
				ApplyRangesStates(axis, correctedRanges[axis]);
		}
		void CalculateLayout() {
			bool isAxisLabelItemsMeasured = IsAxisLabelItemsMeasured;
			IEnumerable<Pane> panes = ActualPanes;
			if (isAxisLabelItemsMeasured && IsNavigationEnabled) {
				foreach (Pane pane in panes) {
					pane.CalculateOuterItemsLayoutAndViewport(false);
					PaneAxesContainer paneAxesData = pane.PaneAxesContainer;
					IEnumerable<SeriesItem> seriesItemsCollection = pane.SeriesItems;
					if (paneAxesData != null && seriesItemsCollection != null) {
						bool rotated = pane.Rotated;
						Rect actualViewport = pane.Viewport;
						IAxisData yAxis = paneAxesData.PrimaryAxisY;
						bool yAxisReversed = yAxis.Reverse;
						IAxisData horizontalAxis;
						IAxisData verticalAxis;
						if (rotated) {
							horizontalAxis = yAxis;
							verticalAxis = paneAxesData.PrimaryAxisX;
						}
						else {
							horizontalAxis = paneAxesData.PrimaryAxisX;
							verticalAxis = yAxis;
						}
						double horizontalAddition = GetZeroPointOffset(horizontalAxis, actualViewport.Width);
						double verticalAddition = GetZeroPointOffset(verticalAxis, actualViewport.Height);
						Point zeroPointForRange;
						Point zeroPointForScrollingRange;
						if (horizontalAxis.Reverse) {
							zeroPointForRange = new Point(actualViewport.Right, actualViewport.Top);
							zeroPointForScrollingRange = new Point(zeroPointForRange.X + horizontalAddition, zeroPointForRange.Y - verticalAddition);
						}
						else {
							zeroPointForRange = new Point(actualViewport.Left, actualViewport.Bottom);
							zeroPointForScrollingRange = new Point(zeroPointForRange.X - horizontalAddition, zeroPointForRange.Y + verticalAddition);
						}
						List<ISeries> seriesOnPane = new List<ISeries>();
						foreach (SeriesItem seriesItem in seriesItemsCollection)
							seriesOnPane.Add(seriesItem.Series as ISeries);
						CorrectRangesBySeriesLabels(pane, seriesOnPane, rotated, zeroPointForScrollingRange, yAxisReversed);
					}
				}
			}
			bool shouldLayoutUpdate = false;
			foreach (Pane pane in panes) {
				shouldLayoutUpdate |= pane.UpdateAxesLabelItems(pane.DesiredSize);
				pane.UpdateAxisItems();
			}
			if (isAxisLabelItemsMeasured && !shouldLayoutUpdate) {
				foreach (Pane pane in panes)
					pane.CalculateOuterItemsLayoutAndViewport(true);
				panesPanelManager = PanesPanelLayoutManager.Create(this);
				if (panesPanelManager != null)
					panesPanelManager.CorrectLayout();
				foreach (Pane pane in panes) {
					Rect viewport = pane.Viewport;
					pane.CalculateOuterItemsLayout();
					((IZoomablePane)pane).RangeLimitsUpdated();
					foreach (Axis2DItem axisItem in pane.AxisItems)
						axisItem.UpdateGeometry(pane.GetGridAndTextData(axisItem.Axis as Axis2D), viewport);
					IList<SeriesItem> seriesItemsCollection = pane.SeriesItems;
					if (seriesItemsCollection != null) {
						List<XYSeries> seriesCollection = new List<XYSeries>();
						foreach (SeriesItem seriesItem in seriesItemsCollection) {
							XYSeries series = seriesItem.Series as XYSeries;
							if (series != null)
								seriesCollection.Add(series);
						}
						SeriesLabel2DLayoutCache labelsLayoutCache = pane.LabelsLayoutCache;
						Rect rangeBounds = pane.RangeBounds;
						if (viewport != labelsLayoutCache.Viewport || rangeBounds.Width != labelsLayoutCache.RangeBounds.Width || rangeBounds.Height != labelsLayoutCache.RangeBounds.Height)
							labelsLayoutCache.Invalidate();
						labelsLayoutCache.Viewport = viewport;
						labelsLayoutCache.RangeBounds = rangeBounds;
						foreach (XYSeries2D series in seriesCollection) {
							series.CreateSeriesPointsLayout();
							series.CreateSeriesLabelsLayout();
						}
						if (labelsLayoutCache.UpdateCacheItems(seriesCollection)) {
							ResolveOverlappingHelper.Process(seriesCollection, IsNavigationEnabled ? pane.RangeBounds : pane.Bounds, LabelsResolveOverlappingMinIndent);
							labelsLayoutCache.CompleteCacheItems();
						}
						else
							labelsLayoutCache.UpdateLabelBounds();
						foreach (XYSeries2D series in seriesCollection) {
							series.CreateSeriesLabelConnectorsLayout();
							series.UpdateSeriesLabelsClipBounds();
						}
					}
					pane.UpdateAxesElementsItemsLayout();
					pane.CalculateIndicatorItems();
					pane.UpdateIndicatorItems();
					pane.InvalidateInnerPanels();
				}
			}
			else
				shouldLayoutUpdate = true;
			if (shouldLayoutUpdate && ChartControl != null)
				ChartControl.AddInvalidate(InvalidateMeasureFlags.MeasureDiagram);
			Rect newViewport = ActualDefaultPane.Viewport;
			if (newViewport != lastPrimaryViewport) {
				lastPrimaryViewport = newViewport;
				ChartControl chart = ChartControl;
				if (chart != null)
					chart.InvalidateChartElementPanel();
			}
		}
		void AddIndicatorToRepository(Indicator indicator, IPane pane) {
			if (!IndicatorsRepository.ContainsKey(pane))
				IndicatorsRepository.Add(pane, new List<Indicator>());
			IndicatorsRepository[pane].Add(indicator);
		}
		Rect GetHorizontalRect(Rect rect, Pane pane) {
			return new Rect(0, rect.Y, pane.Viewport.Width, rect.Height);
		}
		Rect GetVerticalRect(Rect rect, Pane pane) {
			return new Rect(rect.X, 0, rect.Width, pane.Viewport.Height);
		}
		Pane GetPaneByPoint(Point p) {
			foreach (Pane pane in ActualPanes) {
				if (pane.PanePanel != null && ChartControl != null && pane.SeriesItems.Count > 0) {
					Rect? mappingBounds = pane.LastMappingBounds;
					if (mappingBounds.HasValue && mappingBounds.Value.Contains(p))
						return pane;
				}
			}
			return null;
		}
		List<IPane> GetCrosshairSyncPanes(IPane focusedPane, bool isHorizontalSync) {
			List<IPane> crosshairPanes = new List<IPane>();
			if (panesPanelManager != null)
				crosshairPanes.AddRange(panesPanelManager.GetPaneLine((Pane)focusedPane, isHorizontalSync));
			else
				crosshairPanes.Add(focusedPane);
			return crosshairPanes;
		}
		ControlCoordinates CalcDiagramToPointCoordinates(Pane pane, AxisX2D axisX, AxisY2D axisY, object argument, object value) {
			double argumentInternal = axisX.ScaleMap.NativeToInternal(argument);
			bool argumentInRange = axisX.IsValueInRange(argumentInternal);
			if (axisX.ScaleMap.ScaleType == ActualScaleType.Qualitative && !argumentInRange)
				return new ControlCoordinates(pane);
			double valueInternal = axisY.ScaleMap.NativeToInternal(value);
			return MapInternalToPoint(pane, axisX, axisY, argumentInternal, valueInternal);
		}
		ControlCoordinates DiagramToPointInternal(object argument, object value) {
			PaneAxesContainer paneAxesData = ActualDefaultPane.PaneAxesContainer;
			if (paneAxesData == null)
				return new ControlCoordinates(ActualDefaultPane);
			AxisX2D axisX = (AxisX2D)paneAxesData.PrimaryAxisX;
			AxisY2D axisY = (AxisY2D)paneAxesData.PrimaryAxisY;
			if (axisX == null || axisY == null)
				return new ControlCoordinates(ActualDefaultPane);
			DiagramToPointUtils.CheckValue(argument, axisX.ScaleMap.ScaleType);
			DiagramToPointUtils.CheckValue(value, axisY.ScaleMap.ScaleType);
			return CalcDiagramToPointCoordinates(ActualDefaultPane, axisX, axisY, argument, value);
		}
		ControlCoordinates DiagramToPointInternal(object argument, object value, AxisX2D axisX, AxisY2D axisY) {
			if (AxisPaneContainer == null)
				return new ControlCoordinates(ActualDefaultPane);
			Pane pane = AxisPaneContainer.GetFirstPaneByAxes(axisX, axisY) as Pane;
			return pane == null ? new ControlCoordinates(ActualDefaultPane) : CalcDiagramToPointCoordinates(pane, axisX, axisY, argument, value);
		}
		ControlCoordinates DiagramToPointInternal(object argument, object value, AxisX2D axisX, AxisY2D axisY, Pane pane) {
			PaneAxesContainer paneAxisData = pane.PaneAxesContainer;
			return (paneAxisData == null || !paneAxisData.AxesX.Contains(axisX) || !paneAxisData.AxesY.Contains(axisY)) ?
				new ControlCoordinates(pane) : CalcDiagramToPointCoordinates(pane, axisX, axisY, argument, value);
		}
		internal void RaiseZoomEvent(NavigationType navigationType, RangesSnapshot oldRange, RangesSnapshot newRange, Pane pane) {
			XYDiagram2DZoomEventArgs eventArgs = new XYDiagram2DZoomEventArgs((XYDiagram2DZoomEventType)navigationType, pane,
				AxisX, AxisY, oldRange.XRange, oldRange.YRange, newRange.XRange, newRange.YRange);
			DiagramLayoutChanged = true;
			RaiseEvent(eventArgs);
			InvalidateMeasure();
		}
		internal void RaiseScrollEvent(ScrollingOrientation orientation, NavigationType navigationType, RangesSnapshot oldRange, RangesSnapshot newRange, Pane pane) {
			XYDiagram2DScrollEventArgs eventArgs = new XYDiagram2DScrollEventArgs((XYDiagram2DScrollOrientation)orientation, (XYDiagram2DScrollEventType)navigationType, pane,
				AxisX, AxisY, oldRange.XRange, oldRange.YRange, newRange.XRange, newRange.YRange);
			RaiseEvent(eventArgs);
			InvalidateMeasure();
		}
		internal void RaiseAxisScaleChangedEvent(AxisBase axis, NumericScaleOptionsBase oldScaleOptions, NumericScaleOptionsBase newScaleOptions) {
			NumericScaleChangedEventArgs eventArgs = AxisScaleChangedEventArgsHelper.Create(axis, oldScaleOptions, newScaleOptions);
			if (eventArgs == null)
				return;
			if (ChartControl != null)
				ChartControl.RaiseEvent(eventArgs);
			InvalidateMeasure();
		}
		internal void RaiseAxisScaleChangedEvent(AxisBase axis, DateTimeScaleOptionsBase oldScaleOptions, DateTimeScaleOptionsBase newScaleOptions) {
			DateTimeScaleChangedEventArgs eventArgs = AxisScaleChangedEventArgsHelper.Create(axis, oldScaleOptions, newScaleOptions);
			if (eventArgs == null)
				return;
			if (ChartControl != null)
				ChartControl.RaiseEvent(eventArgs);
			InvalidateMeasure();
		}
		protected internal override List<VisibilityLayoutRegion> GetElementsForAutoLayout(Size size) {
			List<VisibilityLayoutRegion> models = new List<VisibilityLayoutRegion>();
			models.Add(new VisibilityLayoutRegion(GetPaneSizeForAutoLayout(ActualDefaultPane, size), ActualDefaultPane.GetElementsForAutoLayout()));
			foreach (Pane pane in Panes)
				models.Add(new VisibilityLayoutRegion(GetPaneSizeForAutoLayout(pane, size), pane.GetElementsForAutoLayout()));
			return models;
		}
		GRealSize2D GetPaneSizeForAutoLayout(Pane pane, Size constraint) {
			double widthFactor = pane.Bounds.Size.Width / DesiredSize.Width;
			double heightFactor = pane.Bounds.Size.Height / DesiredSize.Height;
			return new GRealSize2D(constraint.Width * widthFactor, constraint.Height * heightFactor);
		}
		protected override Size MeasureOverride(Size availableSize) {
			if (!UseViewportCache || DiagramLayoutChanged)
				ClearViewportCache();
			DiagramLayoutChanged = false;
			if (Elements == null) {
				UpdateVisualItems();
				UpdateLogicalElements();
			}
			else
				foreach (Pane pane in ActualPanes)
					pane.EnsureItems();
			availableSize = new Size(Math.Floor(availableSize.Width), Math.Floor(availableSize.Height));
			Size result = base.MeasureOverride(availableSize);
			UpdateAutomaticMeasurement(true);
			CalculateLayout();
			return result;
		}
		protected override void EnsureSeriesItems() {
			foreach (Axis2D axis in Axes) {
				axis.UpdateVisibilityInPanes();
				if (axisPaneRepository != null)
					axis.ActualVisibilityInPanes.UpdateVisibilityInPanes(axisPaneRepository.GetPanes(axis));
			}
			actualAxisX.UpdateAxisValueContainers();
			actualAxisY.UpdateAxisValueContainers();
			foreach (AxisBase axis in SecondaryAxesXInternal)
				axis.UpdateAxisValueContainers();
			foreach (AxisBase axis in SecondaryAxesYInternal)
				axis.UpdateAxisValueContainers();
			if (paneAxesContainerRepository != null)
				paneAxesContainerRepository.UpdateRanges();
			foreach (Pane pane in ActualPanes)
				pane.Update();
		}
		protected internal override void UpdateSeriesItems() {
			base.UpdateSeriesItems();
			foreach (Pane pane in ActualPanes) {
				pane.LabelsLayoutCache.Invalidate();
				pane.UpdateSeriesItems();
			}
			if (lastCrosshairLocation != null) {
				ResetPointsHighlighting();
				UpdateCrosshairLocation(lastCrosshairLocation.Value);
				if (ChartControl != null && ChartControl.CrosshairPanel != null)
					CommonUtils.InvalidateArrange(ChartControl.CrosshairPanel);
			}
			IndicatorsRepository.Clear();
			foreach (Series series in Series) {
				XYSeries2D xySeries2D = series as XYSeries2D;
				if (xySeries2D != null && xySeries2D.Visible) {
					foreach (Indicator indicator in xySeries2D.Indicators) {
						SeparatePaneIndicator separatePaneIndicator = indicator as SeparatePaneIndicator;
						if (separatePaneIndicator != null) {
							IPane indicatorPane = separatePaneIndicator.ActualPane;
							AddIndicatorToRepository(separatePaneIndicator, indicatorPane);
						}
						else
							AddIndicatorToRepository(indicator, xySeries2D.ActualPane);
					}
				}
			}
		}
		protected override List<LegendItem> CreateLegendItems() {
			List<LegendItem> legendItems = base.CreateLegendItems();
			foreach (Axis2D axis in Axes)
				axis.FillConstantLinesLegendItems(legendItems);
			foreach (Axis2D axis in Axes)
				axis.FillStripsLegendItems(legendItems);
			foreach (Pane pane in ActualPanes)
				pane.FillIndicatorsLegendItems(legendItems);
			return legendItems;
		}
		protected internal override void ProcessMouseMove(Point chartPosition, MouseEventArgs e) {
			paneUnderMouse = null;
			HitTest(chartPosition, new HitTestFilterCallback(UpdatePaneUnderMouseFilter));
			Pane focusedPane = FocusedPane;
			if (focusedPane != null && focusedPane.PanePanel != null) {
				Point position = e.GetPosition(focusedPane.PanePanel);
				lastPosition = position;
				Point positionInNavigationLayer = e.GetPosition(ChartControl.NavigationLayer);
				offsetX = positionInNavigationLayer.X - position.X;
				offsetY = positionInNavigationLayer.Y - position.Y;
			}
		}
		protected internal override void RaiseCanExecutedChanged() {
			base.RaiseCanExecutedChanged();
			commands.ForEach(command => command.RaiseCanExecuteChanged());
			foreach (Pane pane in ActualPanes)
				pane.RaiseCanExecutedChanged();
		}
		protected internal override void NavigationZoomIn() {
			Pane focusedPane = FocusedPane;
			if (focusedPane != null && focusedPane.ZoomInCommand.CanExecute(null))
				focusedPane.ZoomInCommand.Execute(null);
		}
		protected internal override void NavigationZoomIn(Point position) {
			Pane focusedPane = FocusedPane;
			if (focusedPane != null && focusedPane.CanZoomIn())
				focusedPane.ZoomIn(position);
		}
		protected internal override void NavigationZoomOut() {
			Pane focusedPane = FocusedPane;
			if (focusedPane != null && focusedPane.ZoomOutCommand.CanExecute(null))
				focusedPane.ZoomOutCommand.Execute(null);
		}
		protected internal override void NavigationUndoZoom() {
			if (UndoZoomCommand.CanExecute(null))
				UndoZoomCommand.Execute(null);
		}
		protected internal override bool NavigationScrollHorizontally(int delta, NavigationType navigationType) {
			Pane focusedPane = FocusedPane;
			if (focusedPane != null && focusedPane.CanScrollHorizontally(delta)) {
				focusedPane.ScrollHorizontally(delta, navigationType);
				return true;
			}
			return false;
		}
		protected internal override bool NavigationScrollVertically(int delta, NavigationType navigationType) {
			Pane focusedPane = FocusedPane;
			if (focusedPane != null && focusedPane.CanScrollVertically(delta)) {
				focusedPane.ScrollVertically(delta, navigationType);
				return true;
			}
			return false;
		}
		protected internal override bool NavigationZoom(Point position, int delta, ZoomingKind zoomingKind, bool isDragging) {
			if (!isDragging)
				UpdateFocus(position);
			Pane focusedPane = FocusedPane;
			if ((isDragging && focusedPane != null) || InViewport(position, focusedPane))
				return focusedPane.PerformZoom(delta, zoomingKind);
			return false;
		}
		protected internal override bool NavigationBeginDrag(Point position, MouseButtonEventArgs e, bool isShiftKey) {
			UpdateFocus(position);
			Pane focusedPane = FocusedPane;
			if (InViewport(position, focusedPane) && focusedPane.PanePanel != null && IsNavigationEnabled) {
				Point relativePosition = e.GetPosition(focusedPane.PanePanel);
				lastPosition = relativePosition;
				startRectanglePoint = relativePosition;
				return isShiftKey ? NavigationCanZoomIntoRectangle() : true;
			}
			return false;
		}
		protected internal override bool NavigationCanZoomIntoRectangle() {
			Pane focusedPane = FocusedPane;
			if (focusedPane != null)
				return focusedPane.CanZoomIntoRectangle();
			return false;
		}
		protected internal override void NavigationZoomIntoRectangle(Rect rectangle) {
			Pane focusedPane = FocusedPane;
			if (focusedPane != null)
				focusedPane.ZoomIntoRectangle(rectangle);
		}
		protected internal override void NavigationShowSelection(Point chartPosition, MouseEventArgs e) {
			Pane focusedPane = FocusedPane;
			if (focusedPane != null) {
				Point position = e.GetPosition(focusedPane.PanePanel);
				Rect viewport = focusedPane.Viewport;
				double x = Math.Max(Math.Min(viewport.Width, position.X), 0);
				double y = Math.Max(Math.Min(viewport.Height, position.Y), 0);
				Rect rect = new Rect(startRectanglePoint, new Point(x, y));
				if (EnableAxisXNavigation) {
					if (!EnableAxisYNavigation)
						rect = Rotated ? GetHorizontalRect(rect, focusedPane) : GetVerticalRect(rect, focusedPane);
				}
				else if (EnableAxisYNavigation)
					rect = Rotated ? GetVerticalRect(rect, focusedPane) : GetHorizontalRect(rect, focusedPane);
				ChartControl.NavigationLayer.ShowSelection(new Rect(rect.X + offsetX, rect.Y + offsetY, rect.Width, rect.Height));
			}
		}
		protected internal override void NavigationDrag(Point chartPosition, int dx, int dy, NavigationType navigationType, MouseEventArgs e) {
			Pane focusedPane = FocusedPane;
			if (focusedPane != null && focusedPane.CanScroll(dx, dy))
				focusedPane.Scroll(dx, dy, navigationType);
		}
		protected internal override bool NavigationCanZoomIn(Point chartPosition, bool useFocusedPane) {
			if (useFocusedPane) {
				Pane focusedPane = FocusedPane;
				if (focusedPane != null)
					return focusedPane.CanZoomIn();
				else
					return false;
			}
			else
				return (InViewport(chartPosition, paneUnderMouse) && paneUnderMouse.CanZoomIn());
		}
		protected internal override bool NavigationCanZoomOut(Point chartPosition, bool useFocusedPane) {
			if (useFocusedPane) {
				Pane focusedPane = FocusedPane;
				if (focusedPane != null)
					return focusedPane.CanZoomOut();
				else
					return false;
			}
			else
				return (InViewport(chartPosition, paneUnderMouse) && paneUnderMouse.CanZoomOut());
		}
		protected internal override bool NavigationInDiagram(Point chartPosition) {
			return InViewport(chartPosition, paneUnderMouse);
		}
		protected internal override bool NavigationCanDrag(Point chartPosition, bool useFocusedPane) {
			if (useFocusedPane) {
				Pane focusedPane = FocusedPane;
				return focusedPane != null && CanScrollPane(focusedPane);
			}
			else
				return InViewport(chartPosition, paneUnderMouse) && CanScrollPane(paneUnderMouse);
		}
		protected internal override bool ManipulationStart(Point point) {
			UpdateFocus(point);
			Pane focusedPane = FocusedPane;
			if (focusedPane == null)
				return true;
			if (!InViewport(point, focusedPane)) {
				if (focusedScrollBar != null && focusedThumb != null)
					focusedScrollBar.StartThumbDrag();
				else
					return false;
				return true;
			}
			manipulationStartPoint = point;
			PaneAxesContainer paneAxesData = focusedPane.PaneAxesContainer;
			if (paneAxesData != null) {
				manipulationStartAxisXMin = paneAxesData.PrimaryAxisX.VisualRange.Min;
				manipulationStartAxisXMax = paneAxesData.PrimaryAxisX.VisualRange.Max;
				manipulationStartAxisYMin = paneAxesData.PrimaryAxisY.VisualRange.Min;
				manipulationStartAxisYMax = paneAxesData.PrimaryAxisY.VisualRange.Max;
			}
			return true;
		}
		protected internal override void ManipulationZoom(double scaleX, double scaleY) {
			Pane focusedPane = FocusedPane;
			if ((focusedPane != null) && (focusedPane.CanZoomIn() || focusedPane.CanZoomOut())) {
				PaneAxesContainer paneAxesData = focusedPane.PaneAxesContainer;
				if (paneAxesData != null && manipulationStartPoint.HasValue)
					focusedPane.Zoom(manipulationStartPoint.Value, manipulationStartAxisXMin, manipulationStartAxisXMax,
					   manipulationStartAxisYMin, manipulationStartAxisYMax, 1 / scaleX, 1 / scaleY, ZoomingKind.Gesture, NavigationType.Gesture);
			}
		}
		protected internal override Point ManipulationDrag(Point translation) {
			int dx = (int)MathUtils.StrongRound(translation.X);
			int dy = (int)MathUtils.StrongRound(translation.Y);
			if (focusedScrollBar != null) {
				if (focusedScrollBar.Orientation == Orientation.Horizontal && translation.X != 0 || focusedScrollBar.Orientation == Orientation.Vertical && translation.Y != 0) {
					if (focusedResizer != null)
						focusedScrollBar.DragThumbResizer(focusedResizer, translation.X, translation.Y);
					else {
						focusedScrollBar.ThumbDragDelta(translation.X, translation.Y);
						return GetOverPan(dx, dy);
					}
				}
			}
			else {
				Pane focusedPane = FocusedPane;
				if (focusedPane != null) {
					if (focusedPane.CanScroll(dx, dy))
						focusedPane.Scroll(dx, dy, NavigationType.Gesture);
					return GetOverPan(-dx, -dy);
				}
			}
			return new Point();
		}
		protected internal override void UpdateActualPanes(bool shouldUpdate) {
			PaneCollection panes = Panes;
			if (shouldUpdate || (panes != null && panes.ShouldUpdateActualPanes(ActualPanes))) {
				ObservableCollection<Pane> newActualPanes = new ObservableCollection<Pane>();
				newActualPanes.Add(actualDefaultPane);
				if (panes != null)
					foreach (Pane pane in panes)
						newActualPanes.Add(pane);
				this.SetValue(ActualPanesPropertyKey, newActualPanes);
			}
		}
		protected internal override void UpdateVisualItems() {
			CleanStripItemsInStrips();
			foreach (Pane pane in ActualPanes)
				pane.UpdateVisualItems();
		}
		void CleanStripItemsInStrips() {
			foreach (Strip strip in ActualAxisX.Strips)
				strip.StripItems.Clear();
			foreach (Strip strip in ActualAxisY.Strips)
				strip.StripItems.Clear();
			foreach (SecondaryAxisX2D axis in SecondaryAxesXInternal)
				foreach (Strip strip in axis.Strips)
					strip.StripItems.Clear();
			foreach (SecondaryAxisY2D axis in SecondaryAxesYInternal)
				foreach (Strip strip in axis.Strips)
					strip.StripItems.Clear();
		}
		protected internal override void UpdateLogicalElements() {
			ObservableCollection<object> elements = new ObservableCollection<object>();
			foreach (Series series in Series) {
				elements.Add(series.ActualLabel);
				XYSeries2D xySeries2D = series as XYSeries2D;
				if (xySeries2D != null)
					foreach (Indicator indicator in xySeries2D.Indicators)
						elements.Add(indicator);
			}
			foreach (Axis2D axis in Axes) {
				elements.Add(axis);
				foreach (Strip strip in axis.Strips)
					elements.Add(strip);
				axis.ConstantLinesBehind.FillConstantLinesAndTitles(elements);
				axis.ConstantLinesInFront.FillConstantLinesAndTitles(elements);
				elements.Add(axis.ActualLabel);
				if (axis.Title != null)
					elements.Add(axis.Title);
			}
			Elements = elements;
		}
		protected internal override void ClearViewportCache() {
			foreach (Axis2D axis in Axes)
				axis.OverlappingCache = null;
			foreach (Pane pane in ActualPanes)
				pane.ClearViewportCache();
		}
		protected internal override void EnableCache() {
			useViewportCache = true;
		}
		protected internal override void DisableCache() {
			useViewportCache = false;
			ClearViewportCache();
		}
		protected internal override void CheckMeasureUnits() {
			IList<AxisBase> affectedAxes = UpdateAutomaticMeasurement(false);
			ViewController.SendDataAgreggationUpdates(affectedAxes);
		}
		internal double GetAxisLength(AxisBase axisBase) {
			IList<IPane> panes = axisPaneRepository.GetPanes(axisBase);
			if (panes != null && panes.Count > 0)
				return GetAxisLength((Pane)panes[0], axisBase.IsValuesAxis);
			return double.NaN;
		}
		internal IList<AxisBase> UpdateAutomaticMeasurement(bool updateViewController) {
			List<AxisBase> affectedAxes = new List<AxisBase>();
			ObservableCollection<Pane> panes = ActualPanes;
			foreach (Pane pane in panes)
				affectedAxes.AddRange(UpdateAutomaticMeasurementUnits(pane));
			if (affectedAxes.Count > 0 && updateViewController) {
				ViewController.SendDataAgreggationUpdates(affectedAxes);
				ChartElementHelper.Update((IChartElement)this, ChartElementChange.UpdateXYDiagram2DItems);
			}
			return affectedAxes;
		}
		internal void CompleteDeserializing() {
			foreach (Series series in Series)
				if (series is XYSeries2D)
					((XYSeries2D)series).CompleteDeserializing();
			foreach (Axis2D axes in Axes)
				axes.CompleteDeserializing();
		}
		IList<AxisBase> UpdateAutomaticMeasurementUnits(Pane pane, IList<IAxisData> axes) {
			List<AxisBase> affectedAxes = new List<AxisBase>();
			if (axes.Count > 0) {
				double axisLength = GetAxisLength(pane, axes[0].IsValueAxis);
				if (!GeometricUtils.IsValidDouble(axisLength) || axisLength == 0)
					axisLength = defaultAxisLength;
				foreach (AxisBase axis in axes) {
					if (axis.UpdateAutomaticMeasureUnit(axisLength))
						affectedAxes.Add(axis);
				}
			}
			return affectedAxes;
		}
		IList<AxisBase> UpdateAutomaticMeasurementUnits(Pane pane) {
			List<AxisBase> affectedAxes = new List<AxisBase>();
			PaneAxesContainer paneAxesContainer = pane.PaneAxesContainer;
			if (paneAxesContainer != null) {
				affectedAxes.AddRange(UpdateAutomaticMeasurementUnits(pane, paneAxesContainer.AxesX));
				affectedAxes.AddRange(UpdateAutomaticMeasurementUnits(pane, paneAxesContainer.AxesY));
			}
			return affectedAxes;
		}
		double GetAxisLength(Pane pane, bool isValueAxis) {
			return !isValueAxis ^ Rotated ? pane.Bounds.Width : pane.Bounds.Height;
		}
		T GetItem<T>(ObservableCollection<T> items, int index) where T : ICrosshairLabelItem, new() {
			while (items.Count <= index)
				items.Add(new T());
			return items[index];
		}
		void FillCrosshairAxisLabelItem(CrosshairAxisLabelItem crosshairAxisLabelItem, CrosshairAxisInfo labelInfo, CrosshairAxisLabelElement axesLabelElement) {
			crosshairAxisLabelItem.LabelInfo = labelInfo;
			Axis2D Axis2D = (Axis2D)labelInfo.Axis;
			CrosshairAxisLabelPresentationData presentationData = crosshairAxisLabelItem.PresentationData;
			presentationData.Axis = Axis2D;
			presentationData.Text = axesLabelElement.Text;
			presentationData.Background = axesLabelElement.Background;
			presentationData.Foreground = axesLabelElement.Foreground;
			presentationData.FontWeight = axesLabelElement.FontWeight;
			presentationData.FontStyle = axesLabelElement.FontStyle;
			presentationData.FontStretch = axesLabelElement.FontStretch;
			presentationData.FontSize = axesLabelElement.FontSize;
			presentationData.FontFamily = axesLabelElement.FontFamily;
			presentationData.CrosshairAxisLabelTemplate = axesLabelElement.CrosshairLabelTemplate;
		}
		void UpdateCrosshairSeriesLabels(CrosshairDrawInfo crosshairDrawInfo, List<CrosshairLabelInfoEx> labelsInfo) {
			ObservableCollection<CrosshairSeriesLabelItem> crosshairSeriesLabelItems = CrosshairSeriesLabelItems;
			CrosshairSeriesLabelItem crosshairSeriesLabelItem;
			int itemIndex = 0;
			for (int i = 0; i < labelsInfo.Count; i++) {
				crosshairSeriesLabelItem = GetItem(crosshairSeriesLabelItems, itemIndex);
				if (FillCrosshairSeriesLabelItem(crosshairSeriesLabelItem, labelsInfo[i], crosshairDrawInfo))
					itemIndex++;
			}
			if (crosshairSeriesLabelItems.Count > itemIndex) {
				int count = crosshairSeriesLabelItems.Count;
				for (int i = itemIndex; i < count; i++)
					crosshairSeriesLabelItems.RemoveAt(itemIndex);
			}
		}
		bool FillCrosshairSeriesLabelItem(CrosshairSeriesLabelItem crosshairSeriesLabelItem, CrosshairLabelInfoEx labelInfo, CrosshairDrawInfo crosshairDrawInfo) {
			crosshairSeriesLabelItem.Layout = labelInfo;
			ObservableCollection<CrosshairSeriesLabelPresentationData> presentationData = new ObservableCollection<CrosshairSeriesLabelPresentationData>();
			foreach (CrosshairElementGroup elementGroup in crosshairDrawInfo.CrosshairElementGroups) {
				if (crosshairDrawInfo.GetGroupLabelInfo(elementGroup) == labelInfo) {
					CrosshairGroupHeaderElement groupHeaderElement = elementGroup.HeaderElement;
					if (groupHeaderElement != null && !String.IsNullOrEmpty(groupHeaderElement.Text) && groupHeaderElement.Visible)
						presentationData.Add(CreateGroupHeaderPresentationData(groupHeaderElement));
					foreach (CrosshairElement element in elementGroup.CrosshairElements) {
						if (element.Visible && element.LabelElement.Visible) {
							CrosshairSeriesLabelPresentationData seriesLabelPresentationData = CreateCrosshairSeriesLabelPresentationData(element.RefinedSeries, element.RefinedPoint, element.LabelElement);
							if (seriesLabelPresentationData != null)
								presentationData.Add(seriesLabelPresentationData);
						}
					}
				}
			}
			if (presentationData.Count == 0)
				return false;
			crosshairSeriesLabelItem.PresentationData = presentationData;
			return true;
		}
		CrosshairSeriesLabelPresentationData CreateCrosshairSeriesLabelPresentationData(IRefinedSeries refinedSeries, RefinedPoint refinedPoint, CrosshairLabelElement labelElement) {
			if (!labelElement.Visible)
				return null;
			CrosshairSeriesLabelPresentationData crosshairSeriesLabelPresentationData;
			XYSeries2D series = (XYSeries2D)refinedSeries.Series;
			CrosshairMarkerItem markerItem = series.CreateCrosshairMarkerItem(refinedSeries, refinedPoint);
			markerItem.MarkerBrush = labelElement.MarkerBrush;
			markerItem.MarkerLineBrush = labelElement.MarkerBrush;
			crosshairSeriesLabelPresentationData = new CrosshairSeriesLabelPresentationData() {
				Text = labelElement.Text,
				HeaderText = labelElement.HeaderText,
				FooterText = labelElement.FooterText,
				Series = series,
				MarkerItem = markerItem,
				CrosshairSeriesLabelTemplate = labelElement.CrosshairLabelTemplate,
				Foreground = labelElement.Foreground,
				FontFamily = labelElement.FontFamily,
				FontSize = labelElement.FontSize,
				FontStretch = labelElement.FontStretch,
				FontStyle = labelElement.FontStyle,
				FontWeight = labelElement.FontWeight,
				TextVisibility = labelElement.TextVisible ? Visibility.Visible : Visibility.Collapsed,
				MarkerVisibility = labelElement.MarkerVisible ? Visibility.Visible : Visibility.Collapsed,
				ElementAlignment = labelElement.MarkerVisible ? HorizontalAlignment.Left : HorizontalAlignment.Right,
				HeaderTextVisibility = String.IsNullOrEmpty(labelElement.HeaderText) ? Visibility.Collapsed : Visibility.Visible,
				FooterTextVisibility = String.IsNullOrEmpty(labelElement.FooterText) ? Visibility.Collapsed : Visibility.Visible,
				SeriesPoint = refinedPoint != null ? SeriesPoint.GetSeriesPoint(refinedPoint.SeriesPoint) : null
			};
			return crosshairSeriesLabelPresentationData;
		}
		CrosshairSeriesLabelPresentationData CreateGroupHeaderPresentationData(CrosshairGroupHeaderElement groupHeaderElement) {
			CrosshairSeriesLabelPresentationData groupHeaderPresentationData = new CrosshairSeriesLabelPresentationData() {
				Text = groupHeaderElement.Text,
				HeaderText = "",
				FooterText = "",
				MarkerItem = null,
				CrosshairSeriesLabelTemplate = groupHeaderElement.CrosshairLabelTemplate,
				Foreground = groupHeaderElement.Foreground,
				FontFamily = groupHeaderElement.FontFamily,
				FontSize = groupHeaderElement.FontSize,
				FontStretch = groupHeaderElement.FontStretch,
				FontStyle = groupHeaderElement.FontStyle,
				FontWeight = groupHeaderElement.FontWeight,
				TextVisibility = Visibility.Visible,
				MarkerVisibility = Visibility.Collapsed,
				ElementAlignment = HorizontalAlignment.Left,
				HeaderTextVisibility = Visibility.Collapsed,
				FooterTextVisibility = Visibility.Collapsed
			};
			return groupHeaderPresentationData;
		}
		void UpdateCrosshairPaneElements(CrosshairDrawInfo crosshairDrawInfo) {
			ObservableCollection<CrosshairAxisLabelItem> crosshairAxisLabelItems = CrosshairAxisLabelItems;
			int itemIndex = 0;
			foreach (CrosshairPaneDrawInfo paneElement in crosshairDrawInfo.PaneElements) {
				if (paneElement.CursorLine == null)
					continue;
				Pane pane = paneElement.Pane;
				ObservableCollection<CrosshairLinePresentationData> crosshairLinesGeometry = pane.CrosshairLinesGeometry;
				crosshairLinesGeometry.Clear();
				foreach (CrosshairElementDrawInfo elementDrawInfo in paneElement.ElementsDrawInfo) {
					CrosshairLineElement pointLineElement = elementDrawInfo.Element.LineElement;
					if (elementDrawInfo.Element.Visible && pointLineElement.Visible) {
						CrosshairLine crossLine = elementDrawInfo.ElementLine;
						crosshairLinesGeometry.Add(CreateCrosshairLinePresentationData(pane, crossLine, pointLineElement));
					}
					CrosshairAxisLabelElement axisLabelElement = elementDrawInfo.Element.AxisLabelElement;
					if (elementDrawInfo.Element.Visible && axisLabelElement.Visible) {
						CrosshairAxisLabelItem axisLabelItem = GetItem(crosshairAxisLabelItems, itemIndex);
						itemIndex++;
						FillCrosshairAxisLabelItem(axisLabelItem, elementDrawInfo.ElementAxisInfo, axisLabelElement);
					}
				}
				CrosshairLineElement cursorLineElement = crosshairDrawInfo.CursorLineElement;
				if (cursorLineElement.Visible)
					crosshairLinesGeometry.Add(CreateCrosshairLinePresentationData(pane, paneElement.CursorLine, cursorLineElement));
				foreach (KeyValuePair<CrosshairAxisLabelElement, CrosshairAxisInfo> cursorAxisElement in paneElement.CursorAxesInfo) {
					CrosshairAxisLabelElement axisLabelElement = cursorAxisElement.Key;
					if (axisLabelElement.Visible) {
						CrosshairAxisLabelItem axisLabelItem = GetItem(crosshairAxisLabelItems, itemIndex);
						itemIndex++;
						FillCrosshairAxisLabelItem(axisLabelItem, cursorAxisElement.Value, axisLabelElement);
					}
				}
			}
			if (crosshairAxisLabelItems.Count > itemIndex) {
				int count = crosshairAxisLabelItems.Count;
				for (int i = itemIndex; i < count; i++)
					crosshairAxisLabelItems.RemoveAt(itemIndex);
			}
		}
		CrosshairLinePresentationData CreateCrosshairLinePresentationData(Pane pane, CrosshairLine crossLine, CrosshairLineElement lineElement) {
			Rect mappingBounds = pane.LastMappingBounds.Value;
			double xOffset = -mappingBounds.X + pane.Viewport.X;
			double yOffset = -mappingBounds.Y + pane.Viewport.Y;
			return CreateCrosshairLinePresentationData(
				crossLine.Line.Start.X + xOffset, crossLine.Line.Start.Y + yOffset,
				crossLine.Line.End.X + xOffset, crossLine.Line.End.Y + yOffset, lineElement);
		}
		CrosshairLinePresentationData CreateCrosshairLinePresentationData(double x1, double y1, double x2, double y2, CrosshairLineElement lineElement) {
			CrosshairLinePresentationData linePresentationData = new CrosshairLinePresentationData();
			linePresentationData.X1 = x1;
			linePresentationData.Y1 = y1;
			linePresentationData.X2 = x2;
			linePresentationData.Y2 = y2;
			linePresentationData.LineStyle = lineElement.LineStyle;
			linePresentationData.Brush = lineElement.Brush;
			return linePresentationData;
		}
		void CreateAndRaiseEvent(CrosshairDrawInfo crosshairDrawInfos) {
			if (ChartControl.ShouldRaiseCustomDrawCrosshairEvent) {
				CustomDrawCrosshairEventArgs args = new CustomDrawCrosshairEventArgs(ChartControl.CustomDrawCrosshairEvent, crosshairDrawInfos.CursorLineElement,
					crosshairDrawInfos.CrosshairAxisLabelElements, crosshairDrawInfos.CrosshairElementGroups);
				ChartControl.RaiseEvent(args);
			}
		}
		void ResetPointsHighlighting() {
			foreach (ISeriesPoint point in highlightedPoints) {
				SeriesPoint seriesPoint = SeriesPoint.GetSeriesPoint(point);
				XYSeries2D xySeries = seriesPoint.Series as XYSeries2D;
				if (xySeries != null)
					xySeries.SetPointState(point, null, false);
			}
			highlightedPoints.Clear();
		}
		void HighlightPoints(List<CrosshairSeriesPointEx> points, List<CrosshairElementGroup> elements, List<ISeriesPoint> resetedPoints) {
			foreach (CrosshairSeriesPointEx crosshairPoint in points) {
				RefinedPoint refinedPoint = crosshairPoint.RefinedPoint;
				ISeriesPoint point = refinedPoint.SeriesPoint;
				SeriesPoint seriesPoint = SeriesPoint.GetSeriesPoint(point);
				if (seriesPoint != null) {
					XYSeries2D xySeries = seriesPoint.Series as XYSeries2D;
					if (xySeries != null) {
						foreach (CrosshairElementGroup group in elements) {
							foreach (CrosshairElement element in group.CrosshairElements) {
								if (element.Series == crosshairPoint.RefinedSeries.Series && element.Visible) {
									if (!highlightedPoints.Contains(point)) {
										xySeries.SetPointState(point, refinedPoint, true);
										highlightedPoints.Add(point);
									}
									resetedPoints.Remove(point);
								}
							}
						}
					}
				}
			}
		}
		void UnhighlightPoints(List<ISeriesPoint> resetedPoints) {
			foreach (ISeriesPoint point in resetedPoints) {
				highlightedPoints.Remove(point);
				SeriesPoint seriesPoint = SeriesPoint.GetSeriesPoint(point);
				XYSeries2D xySeries = seriesPoint.Series as XYSeries2D;
				if (xySeries != null)
					xySeries.SetPointState(point, null, false);
			}
		}
		GRealRect2D CalculateCrosshairLabelsConstraint(Point controlAnchorPosition) {
			if (ChartControl.FlowDirection == FlowDirection.RightToLeft)
				controlAnchorPosition = new Point(ChartControl.ActualWidth - controlAnchorPosition.X, controlAnchorPosition.Y);
			Point screenOffset = DevExpress.Xpf.Core.Native.ScreenHelper.GetScreenPoint(ChartControl);
			if (ChartControl.FlowDirection == FlowDirection.RightToLeft)
				screenOffset = new Point(screenOffset.X - ChartControl.ActualWidth, screenOffset.Y);
			Point screenAnchorPoint = new Point(screenOffset.X + controlAnchorPosition.X, screenOffset.Y + controlAnchorPosition.Y);
			Rect popupScreenRect = DevExpress.Xpf.Core.Native.ScreenHelper.GetScreenRect(screenAnchorPoint);
			GRealRect2D constraint = new GRealRect2D(popupScreenRect.X - screenOffset.X, popupScreenRect.Y - screenOffset.Y, popupScreenRect.Width, popupScreenRect.Height);
			return constraint;
		}
		Point CalculateLocalScreenPosition(Point controlAnchorPosition) {
			Point screenOffset = DevExpress.Xpf.Core.Native.ScreenHelper.GetScreenPoint(ChartControl);
			Point screenAnchorPoint = new Point(screenOffset.X + controlAnchorPosition.X, screenOffset.Y + controlAnchorPosition.Y);
			Rect popupScreenRect = DevExpress.Xpf.Core.Native.ScreenHelper.GetScreenRect(screenAnchorPoint);
			return new Point(screenAnchorPoint.X - popupScreenRect.X, screenAnchorPoint.Y - popupScreenRect.Y);
		}
		Rect CorrectBoundsByScreenSize(GRealRect2D initialBounds, GRealSize2D boundsSize) {
			if (ChartControl.FlowDirection == FlowDirection.RightToLeft)
				initialBounds = new GRealRect2D(ChartControl.ActualWidth - initialBounds.Left, initialBounds.Top, initialBounds.Width, initialBounds.Height);
			Point screenOffset = DevExpress.Xpf.Core.Native.ScreenHelper.GetScreenPoint(ChartControl);
			if (ChartControl.FlowDirection == FlowDirection.RightToLeft)
				screenOffset = new Point(screenOffset.X - ChartControl.ActualWidth, screenOffset.Y);
			Point localScreenPosition = CalculateLocalScreenPosition(new Point(initialBounds.Left, initialBounds.Top));
			Point screenAnchorPosition = new Point(localScreenPosition.X - AnnotationPanel.PopupContentShadowPadding,
				localScreenPosition.Y - AnnotationPanel.PopupContentShadowPadding);
			if (screenAnchorPosition.X < 0)
				screenOffset.X -= screenAnchorPosition.X;
			if (screenAnchorPosition.Y < 0)
				screenAnchorPosition.Y -= screenOffset.Y;
			return new Rect(initialBounds.Left + screenOffset.X, initialBounds.Top + screenOffset.Y, boundsSize.Width, boundsSize.Height);
		}
		protected internal override void UpdateCrosshairLocation(Point? crosshairLocation) {
			lastCrosshairLocation = crosshairLocation;
			if (crosshairLocation != null) {
				Pane focusedPane = GetPaneByPoint(crosshairLocation.Value);
				if (focusedPane != null) {
					GRealPoint2D cursorLocation = new GRealPoint2D(crosshairLocation.Value.X, crosshairLocation.Value.Y);
					lastCrosshairInfo = crosshairManager.CalculateCrosshairInfo(cursorLocation, CalculateCrosshairLabelsConstraint(crosshairLocation.Value),
						paneAxesContainerRepository.Values, focusedPane,
						new List<IRefinedSeries>(ViewController.SeriesController.ActiveRefinedSeries));
					if (lastCrosshairInfo != null) {
						CrosshairDrawInfo crosshairDrawInfo = new CrosshairDrawInfo(ChartControl.ActualCrosshairOptions, this);
						List<CrosshairLabelInfoEx> labelsInfo = new List<CrosshairLabelInfoEx>();
						List<ISeriesPoint> resetedPoints = new List<ISeriesPoint>(highlightedPoints);
						foreach (Pane pane in ActualPanes) {
							pane.UpdateCrosshairAppearance();
							CrosshairPaneInfoEx crosshairPaneInfo = lastCrosshairInfo.GetByPane(pane);
							if (crosshairPaneInfo != null && crosshairPaneInfo.LabelsInfo != null) {
								labelsInfo.AddRange(crosshairPaneInfo.LabelsInfo);
								pane.UpdateCrosshairLabelsInfo(crosshairPaneInfo.LabelsInfo);
								crosshairDrawInfo.AddCrosshairPaneInfo(crosshairPaneInfo, pane);
							}
						}
						CreateAndRaiseEvent(crosshairDrawInfo);
						foreach (Pane pane in ActualPanes) {
							CrosshairPaneInfoEx crosshairPaneInfo = lastCrosshairInfo.GetByPane(pane);
							if (crosshairPaneInfo != null && ChartControl.ActualCrosshairOptions.HighlightPoints)
								HighlightPoints(crosshairPaneInfo.SeriesPoints, crosshairDrawInfo.CrosshairElementGroups, resetedPoints);
						}
						UnhighlightPoints(resetedPoints);
						UpdateCrosshairSeriesLabels(crosshairDrawInfo, labelsInfo);
						UpdateCrosshairPaneElements(crosshairDrawInfo);
						return;
					}
				}
			}
			ResetPointsHighlighting();
			lastCrosshairInfo = null;
			foreach (Pane pane in ActualPanes)
				pane.ClearCrosshair();
			if (CrosshairAxisLabelItems.Count > 0)
				CrosshairAxisLabelItems.Clear();
			if (CrosshairSeriesLabelItems.Count > 0)
				CrosshairSeriesLabelItems.Clear();
		}
		internal void CompleteCrosshairLayout() {
			if (lastCrosshairInfo == null)
				return;
			ObservableCollection<CrosshairAxisLabelItem> crosshairAxisLabelItems = CrosshairAxisLabelItems;
			foreach (CrosshairAxisLabelItem item in crosshairAxisLabelItems) {
				item.LabelInfo.Size = new GRealSize2D(item.Size.Width, item.Size.Height);
				GRealRect2D bounds = item.LabelInfo.Bounds;
				item.Bounds = new Rect(bounds.Left, bounds.Top, bounds.Width, bounds.Height);
			}
			ObservableCollection<CrosshairSeriesLabelItem> crosshairSeriesLabelItems = CrosshairSeriesLabelItems;
			foreach (CrosshairSeriesLabelItem item in crosshairSeriesLabelItems) {
				item.Layout.Bounds = GRealRect2D.Empty;
				((CrosshairLabelInfoEx)item.Layout).Size = new GRealSize2D(item.Size.Width, item.Size.Height);
			}
			lastCrosshairInfo.CompleteLabelsLayout();
			foreach (CrosshairSeriesLabelItem item in crosshairSeriesLabelItems) {
				IAnnotationLayout layout = item.Layout;
				double xOffset = AnnotationPanel.PopupContentShadowPadding;
				double yOffset = AnnotationPanel.PopupContentShadowPadding;
				if (layout.Location == AnnotationLocation.TopRight || layout.Location == AnnotationLocation.BottomRight)
					xOffset = -xOffset;
				if (layout.Location == AnnotationLocation.BottomLeft || layout.Location == AnnotationLocation.BottomRight)
					yOffset = -yOffset;
				Point location = CorrectPointByTransform(new Point(layout.Bounds.Left + xOffset, layout.Bounds.Top + yOffset));
				item.Bounds = CorrectBoundsByScreenSize(new GRealRect2D(location.X, location.Y, layout.Bounds.Width, layout.Bounds.Height), layout.Size);
				item.UpdateLayoutProperties();
			}
		}
		Point CorrectPointByTransform(Point point) {
			Point controlOffset = ChartControl.PointToScreen(new Point(0, 0));
			Point controlBoundsOffset = ChartControl.PointToScreen(new Point(ChartControl.ActualWidth, ChartControl.ActualHeight));
			double scaleX = Math.Abs(controlBoundsOffset.X - controlOffset.X) / ChartControl.ActualWidth;
			double scaleY = Math.Abs(controlBoundsOffset.Y - controlOffset.Y) / ChartControl.ActualHeight;
			Point correctedPoint = ScreenHelper.GetScaledPoint(new Point(point.X * scaleX, point.Y * scaleY));
			return correctedPoint;
		}
		Rect CalculateRelativeAxesRect(Point point, Rect lastMappingBounds) {
			point.X -= lastMappingBounds.Left;
			point.Y -= lastMappingBounds.Top;
			if (point.X < 0 || point.X > lastMappingBounds.Width || point.Y < 0 || point.Y > lastMappingBounds.Height)
				return Rect.Empty;
			point.Y = lastMappingBounds.Height - point.Y;
			double x, y, xLength, yLength;
			if (Rotated) {
				x = point.Y;
				y = point.X;
				xLength = lastMappingBounds.Height;
				yLength = lastMappingBounds.Width;
			}
			else {
				x = point.X;
				y = point.Y;
				xLength = lastMappingBounds.Width;
				yLength = lastMappingBounds.Height;
			}
			return new Rect(x, y, xLength, yLength);
		}
		InternalCoordinates MapPointToInternal(Pane pane, Point point) {
			PaneAxesContainer paneAxesData = paneAxesContainerRepository.GetContaiter(pane);
			if (paneAxesData == null)
				return null;
			Rect lastMappingBounds = pane.LastMappingBounds.Value;
			if (lastMappingBounds.IsEmpty)
				return null;
			Rect axesRect = CalculateRelativeAxesRect(point, lastMappingBounds);
			if (axesRect.IsEmpty)
				return null;
			InternalCoordinates coords = new InternalCoordinates(pane);
			Axis2D axisX = (Axis2D)paneAxesData.PrimaryAxisX;
			if (axisX != null) {
				double? argument = axisX.CalcInternalValue(pane, axesRect.X, axesRect.Width);
				if (argument.HasValue)
					coords.AddAxisXValue(axisX, argument.Value);
			}
			foreach (Axis2D secondaryAxisX in paneAxesData.SecondaryAxesX) {
				double? argument = secondaryAxisX.CalcInternalValue(pane, axesRect.X, axesRect.Width);
				if (argument.HasValue)
					coords.AddAxisXValue(secondaryAxisX, argument.Value);
			}
			Axis2D axisY = (Axis2D)paneAxesData.PrimaryAxisY;
			if (axisY != null) {
				double? value = axisY.CalcInternalValue(pane, axesRect.Y, axesRect.Height);
				if (value.HasValue)
					coords.AddAxisYValue(axisY, value.Value);
			}
			foreach (Axis2D secondaryAxisY in paneAxesData.SecondaryAxesY) {
				double? value = secondaryAxisY.CalcInternalValue(pane, axesRect.Y, axesRect.Height);
				if (value.HasValue)
					coords.AddAxisYValue(secondaryAxisY, value.Value);
			}
			return coords;
		}
		ControlCoordinates MapInternalToPoint(Pane pane, AxisX2D axisX, AxisY2D axisY, double argument, double value) {
			double axisXValue = axisX.CalcAxisValue(pane, argument);
			double axisYValue = axisY.CalcAxisValue(pane, value);
			Rect mappingBounds = pane.LastMappingBounds.Value;
			double x, y;
			if (Rotated) {
				x = axisYValue;
				y = axisXValue;
			}
			else {
				x = axisXValue;
				y = axisYValue;
			}
			y = mappingBounds.Height - y;
			x += mappingBounds.Left;
			y += mappingBounds.Top;
			bool argumentInRange = axisX.IsValueInRange(argument);
			bool valueInRange = axisY.IsValueInRange(value);
			ControlCoordinatesVisibility visibility = argumentInRange && valueInRange ? ControlCoordinatesVisibility.Visible : ControlCoordinatesVisibility.Hidden;
			return new ControlCoordinates(pane, axisX, axisY, visibility, new Point(x, y));
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			paneItemsControl = GetTemplateChild("PART_Panes") as ChartItemsControl;
			panesRootPanel = null;
		}
		public DiagramCoordinates PointToDiagram(Point p) {
			DiagramCoordinates coordinates = new DiagramCoordinates();
			Pane pane = GetPaneByPoint(p);
			if (pane == null)
				return coordinates;
			coordinates.SetPane(pane);
			PaneAxesContainer paneAxesData = pane.PaneAxesContainer;
			if (paneAxesData == null)
				return coordinates;
			Rect? lastMappingBounds = pane.LastMappingBounds;
			if (!lastMappingBounds.HasValue)
				return coordinates;
			Rect axesRect = CalculateRelativeAxesRect(p, lastMappingBounds.Value);
			if (axesRect.IsEmpty)
				return coordinates;
			AxisX2D axisX = (AxisX2D)paneAxesData.PrimaryAxisX;
			AxisY2D axisY = (AxisY2D)paneAxesData.PrimaryAxisY;
			if (axisX != null && axisY != null) {
				coordinates.SetAxes(axisX, axisY);
				double internalArgument = axisX.CalcInternalValue(pane, axesRect.X, axesRect.Width);
				double internalValue = axisY.CalcInternalValue(pane, axesRect.Y, axesRect.Height);
				coordinates.SetArgumentAndValue(internalArgument, internalValue);
			}
			foreach (Axis2D secondaryAxisX in paneAxesData.SecondaryAxesX)
				coordinates.AddAxisValue(secondaryAxisX, secondaryAxisX.CalcInternalValue(pane, axesRect.X, axesRect.Width));
			foreach (Axis2D secondaryAxisY in paneAxesData.SecondaryAxesY)
				coordinates.AddAxisValue(secondaryAxisY, secondaryAxisY.CalcInternalValue(pane, axesRect.Y, axesRect.Height));
			return coordinates;
		}
		#region DiagramToPoint
		public ControlCoordinates DiagramToPoint(string argument, double value) {
			return DiagramToPointInternal(argument, value);
		}
		public ControlCoordinates DiagramToPoint(string argument, DateTime value) {
			return DiagramToPointInternal(argument, value);
		}
		public ControlCoordinates DiagramToPoint(double argument, double value) {
			return DiagramToPointInternal(argument, value);
		}
		public ControlCoordinates DiagramToPoint(double argument, DateTime value) {
			return DiagramToPointInternal(argument, value);
		}
		public ControlCoordinates DiagramToPoint(DateTime argument, double value) {
			return DiagramToPointInternal(argument, value);
		}
		public ControlCoordinates DiagramToPoint(DateTime argument, DateTime value) {
			return DiagramToPointInternal(argument, value);
		}
		public ControlCoordinates DiagramToPoint(string argument, double value, AxisX2D axisX, AxisY2D axisY) {
			return DiagramToPointInternal(DiagramToPointUtils.CheckValue(argument, axisX.ScaleMap.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleMap.ScaleType), axisX, axisY);
		}
		public ControlCoordinates DiagramToPoint(string argument, DateTime value, AxisX2D axisX, AxisY2D axisY) {
			return DiagramToPointInternal(DiagramToPointUtils.CheckValue(argument, axisX.ScaleMap.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleMap.ScaleType), axisX, axisY);
		}
		public ControlCoordinates DiagramToPoint(double argument, double value, AxisX2D axisX, AxisY2D axisY) {
			return DiagramToPointInternal(DiagramToPointUtils.CheckValue(argument, axisX.ScaleMap.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleMap.ScaleType), axisX, axisY);
		}
		public ControlCoordinates DiagramToPoint(double argument, DateTime value, AxisX2D axisX, AxisY2D axisY) {
			return DiagramToPointInternal(DiagramToPointUtils.CheckValue(argument, axisX.ScaleMap.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleMap.ScaleType), axisX, axisY);
		}
		public ControlCoordinates DiagramToPoint(DateTime argument, double value, AxisX2D axisX, AxisY2D axisY) {
			return DiagramToPointInternal(DiagramToPointUtils.CheckValue(argument, axisX.ScaleMap.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleMap.ScaleType), axisX, axisY);
		}
		public ControlCoordinates DiagramToPoint(DateTime argument, DateTime value, AxisX2D axisX, AxisY2D axisY) {
			return DiagramToPointInternal(DiagramToPointUtils.CheckValue(argument, axisX.ScaleMap.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleMap.ScaleType), axisX, axisY);
		}
		public ControlCoordinates DiagramToPoint(string argument, double value, AxisX2D axisX, AxisY2D axisY, Pane pane) {
			return DiagramToPointInternal(DiagramToPointUtils.CheckValue(argument, axisX.ScaleMap.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleMap.ScaleType), axisX, axisY, pane);
		}
		public ControlCoordinates DiagramToPoint(string argument, DateTime value, AxisX2D axisX, AxisY2D axisY, Pane pane) {
			return DiagramToPointInternal(DiagramToPointUtils.CheckValue(argument, axisX.ScaleMap.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleMap.ScaleType), axisX, axisY, pane);
		}
		public ControlCoordinates DiagramToPoint(double argument, double value, AxisX2D axisX, AxisY2D axisY, Pane pane) {
			return DiagramToPointInternal(DiagramToPointUtils.CheckValue(argument, axisX.ScaleMap.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleMap.ScaleType), axisX, axisY, pane);
		}
		public ControlCoordinates DiagramToPoint(double argument, DateTime value, AxisX2D axisX, AxisY2D axisY, Pane pane) {
			return DiagramToPointInternal(DiagramToPointUtils.CheckValue(argument, axisX.ScaleMap.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleMap.ScaleType), axisX, axisY, pane);
		}
		public ControlCoordinates DiagramToPoint(DateTime argument, double value, AxisX2D axisX, AxisY2D axisY, Pane pane) {
			return DiagramToPointInternal(DiagramToPointUtils.CheckValue(argument, axisX.ScaleMap.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleMap.ScaleType), axisX, axisY, pane);
		}
		public ControlCoordinates DiagramToPoint(DateTime argument, DateTime value, AxisX2D axisX, AxisY2D axisY, Pane pane) {
			return DiagramToPointInternal(DiagramToPointUtils.CheckValue(argument, axisX.ScaleMap.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleMap.ScaleType), axisX, axisY, pane);
		}
		#endregion
	}
}
