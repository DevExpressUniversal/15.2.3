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
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[
	TemplatePart(Name = "PART_Domain", Type = typeof(DomainPanel)),
	TemplatePart(Name = "PART_DomainBackground", Type = typeof(Border)),
	TemplatePart(Name = "PART_InterlaceControls", Type = typeof(ItemsControl)),
	TemplatePart(Name = "PART_Strips", Type = typeof(StripsItemsControl)),
	TemplatePart(Name = "PART_GridLines", Type = typeof(ItemsControl)),
	TemplatePart(Name = "PART_ConstantLinesBehind", Type = typeof(ItemsControl)),
	TemplatePart(Name = "PART_Series", Type = typeof(ItemsControl)),
	TemplatePart(Name = "PART_Indicators", Type = typeof(ItemsControl)),
	TemplatePart(Name = "PART_ConstantLinesInFront", Type = typeof(ItemsControl)),
	TemplatePart(Name = "PART_IndicatorLabels", Type = typeof(ItemsControl)),
	TemplatePart(Name = "PART_ConstantLineTitles", Type = typeof(ItemsControl)),
	TemplatePart(Name = "PART_DomainBorder", Type = typeof(Border)),
	TemplatePart(Name = "PART_Pseudo3DSeries", Type = typeof(ItemsControl)),
	TemplatePart(Name = "PART_Mirror", Type = typeof(ChartMirrorControl)),
	TemplatePart(Name = "PART_Pseudo3DMirror", Type = typeof(ChartMirrorControl)),
	]
	public class Pane : ChartElement, IZoomablePane, IWeakEventListener, IDockTarget, IInteractiveElement {
		const double zoomFactor = 3.0;
		public static readonly DependencyProperty EnableAxisXNavigationProperty = DependencyPropertyManager.Register("EnableAxisXNavigation",
			typeof(bool?), typeof(Pane), new PropertyMetadata(EnableNavigationPropertyChanged));
		public static readonly DependencyProperty EnableAxisYNavigationProperty = DependencyPropertyManager.Register("EnableAxisYNavigation",
			typeof(bool?), typeof(Pane), new PropertyMetadata(EnableNavigationPropertyChanged));
		public static readonly DependencyProperty AxisXScrollBarOptionsProperty = DependencyPropertyManager.Register("AxisXScrollBarOptions",
			typeof(ScrollBarOptions), typeof(Pane), new PropertyMetadata(null, ScrollBarOptionsPropertyChanged));
		public static readonly DependencyProperty AxisYScrollBarOptionsProperty = DependencyPropertyManager.Register("AxisYScrollBarOptions",
			typeof(ScrollBarOptions), typeof(Pane), new PropertyMetadata(null, ScrollBarOptionsPropertyChanged));
		public static readonly DependencyProperty DomainBrushProperty = DependencyPropertyManager.Register("DomainBrush", typeof(Brush), typeof(Pane));
		public static readonly DependencyProperty DomainBorderBrushProperty = DependencyPropertyManager.Register("DomainBorderBrush", typeof(Brush), typeof(Pane));
		public static readonly DependencyProperty MirrorHeightProperty = DependencyPropertyManager.Register("MirrorHeight",
			typeof(double), typeof(Pane), new PropertyMetadata(0.0, ChartElementHelper.Update), MirrorHeightValidation);
		static readonly DependencyPropertyKey SeriesItemsPropertyKey = DependencyPropertyManager.RegisterReadOnly("SeriesItems",
			typeof(ReadOnlyObservableCollection<SeriesItem>), typeof(Pane), new PropertyMetadata(null));
		static readonly DependencyPropertyKey AxisXScrollBarItemPropertyKey = DependencyPropertyManager.RegisterReadOnly("AxisXScrollBarItem", typeof(ScrollBarItem), typeof(Pane), new PropertyMetadata(null));
		static readonly DependencyPropertyKey AxisYScrollBarItemPropertyKey = DependencyPropertyManager.RegisterReadOnly("AxisYScrollBarItem", typeof(ScrollBarItem), typeof(Pane), new PropertyMetadata(null));
		static readonly DependencyPropertyKey StripItemsPropertyKey = DependencyPropertyManager.RegisterReadOnly("StripItems",
			typeof(ReadOnlyObservableCollection<StripItem>), typeof(Pane), new PropertyMetadata(null));
		static readonly DependencyPropertyKey ConstantLineBehindItemsPropertyKey = DependencyPropertyManager.RegisterReadOnly("ConstantLineBehindItems",
			typeof(ReadOnlyObservableCollection<ConstantLineItem>), typeof(Pane), new PropertyMetadata(null));
		static readonly DependencyPropertyKey ConstantLineInFrontItemsPropertyKey = DependencyPropertyManager.RegisterReadOnly("ConstantLineInFrontItems",
			typeof(ReadOnlyObservableCollection<ConstantLineItem>), typeof(Pane), new PropertyMetadata(null));
		static readonly DependencyPropertyKey ConstantLineTitleItemsPropertyKey = DependencyPropertyManager.RegisterReadOnly("ConstantLineTitleItems",
			typeof(ReadOnlyObservableCollection<ConstantLineTitleItem>), typeof(Pane), new PropertyMetadata(null));
		static readonly DependencyPropertyKey IndicatorItemsPropertyKey = DependencyPropertyManager.RegisterReadOnly("IndicatorItems",
		  typeof(ReadOnlyObservableCollection<IndicatorItem>), typeof(Pane), new PropertyMetadata(null));
		static readonly DependencyPropertyKey IndicatorLabelItemsPropertyKey = DependencyPropertyManager.RegisterReadOnly("IndicatorLabelItems",
		 typeof(ReadOnlyObservableCollection<IndicatorLabelItem>), typeof(Pane), new PropertyMetadata(null));
		static readonly DependencyPropertyKey CrosshairLinesGeometryPropertyKey = DependencyPropertyManager.RegisterReadOnly("CrosshairLinesGeometry", typeof(ObservableCollection<CrosshairLinePresentationData>),
			typeof(Pane), new PropertyMetadata(null));
		public static readonly DependencyProperty CrosshairArgumentBrushProperty = DependencyPropertyManager.Register("CrosshairArgumentBrush", typeof(Brush), typeof(Pane));
		public static readonly DependencyProperty CrosshairValueBrushProperty = DependencyPropertyManager.Register("CrosshairValueBrush", typeof(Brush), typeof(Pane));
		public static readonly DependencyProperty CrosshairArgumentLineStyleProperty = DependencyPropertyManager.Register("CrosshairArgumentLineStyle", typeof(LineStyle), typeof(Pane));
		public static readonly DependencyProperty CrosshairValueLineStyleProperty = DependencyPropertyManager.Register("CrosshairValueLineStyle", typeof(LineStyle), typeof(Pane));
		public static readonly DependencyProperty CrosshairLinesGeometryProperty = CrosshairLinesGeometryPropertyKey.DependencyProperty;
		public static readonly DependencyProperty SeriesItemsProperty = SeriesItemsPropertyKey.DependencyProperty;
		public static readonly DependencyProperty AxisXScrollBarItemProperty = AxisXScrollBarItemPropertyKey.DependencyProperty;
		public static readonly DependencyProperty AxisYScrollBarItemProperty = AxisYScrollBarItemPropertyKey.DependencyProperty;
		public static readonly DependencyProperty StripItemsProperty = StripItemsPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ConstantLineBehindItemsProperty = ConstantLineBehindItemsPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ConstantLineInFrontItemsProperty = ConstantLineInFrontItemsPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ConstantLineTitleItemsProperty = ConstantLineTitleItemsPropertyKey.DependencyProperty;
		public static readonly DependencyProperty IndicatorItemsProperty = IndicatorItemsPropertyKey.DependencyProperty;
		public static readonly DependencyProperty IndicatorLabelItemsProperty = IndicatorLabelItemsPropertyKey.DependencyProperty;
		public static readonly DependencyProperty GridLinesProperty = DependencyPropertyManager.Register("GridLines", typeof(ObservableCollection<UIElement>), typeof(Pane));
		public static readonly DependencyProperty InterlaceControlsProperty = DependencyPropertyManager.Register("InterlaceControls", typeof(ObservableCollection<UIElement>), typeof(Pane));
		public static readonly DependencyProperty PaneItemsProperty = DependencyPropertyManager.Register("PaneItems", typeof(ObservableCollection<object>), typeof(Pane));
		public static readonly DependencyProperty SeriesLabelItemsProperty = DependencyPropertyManager.Register("SeriesLabelItems", typeof(ObservableCollection<SeriesLabelItem>), typeof(Pane));
		public static readonly DependencyProperty Pseudo3DPointItemsProperty = DependencyPropertyManager.Register("Pseudo3DPointItems", typeof(ObservableCollection<SeriesPointItem>), typeof(Pane));
		static Pane() {
			Type ownerType = typeof(Pane);
			FocusableProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(true));
			CommandManager.RegisterClassCommandBinding(ownerType,
				new CommandBinding(XYDiagram2DCommands.ScrollHorizontally, (d, e) => ((Pane)d).ScrollHorizontally((int)e.Parameter), (d, e) => ((Pane)d).OnCanScrollHorizontally(e)));
			CommandManager.RegisterClassCommandBinding(ownerType,
				new CommandBinding(XYDiagram2DCommands.ScrollVertically, (d, e) => ((Pane)d).ScrollVertically((int)e.Parameter), (d, e) => ((Pane)d).OnCanScrollVertically(e)));
			CommandManager.RegisterClassCommandBinding(ownerType,
				new CommandBinding(XYDiagram2DCommands.ScrollAxisXTo, (d, e) => ((Pane)d).ScrollAxisXTo((double)e.Parameter), (d, e) => ((Pane)d).OnCanScrollAxisXTo(e)));
			CommandManager.RegisterClassCommandBinding(ownerType,
				new CommandBinding(XYDiagram2DCommands.ScrollAxisYTo, (d, e) => ((Pane)d).ScrollAxisYTo((double)e.Parameter), (d, e) => ((Pane)d).OnCanScrollAxisYTo(e)));
			CommandManager.RegisterClassCommandBinding(ownerType,
				new CommandBinding(XYDiagram2DCommands.SetAxisXRange, (d, e) => ((Pane)d).SetAxisXRange((AxisRangePositions)e.Parameter), (d, e) => ((Pane)d).OnCanSetAxisXRange(e)));
			CommandManager.RegisterClassCommandBinding(ownerType,
				new CommandBinding(XYDiagram2DCommands.SetAxisYRange, (d, e) => ((Pane)d).SetAxisXRange((AxisRangePositions)e.Parameter), (d, e) => ((Pane)d).OnCanSetAxisYRange(e)));
			CommandManager.RegisterClassCommandBinding(ownerType,
				new CommandBinding(XYDiagram2DCommands.SetAxisXZoomRatio, (d, e) => ((Pane)d).SetAxisXZoomRatio((double)e.Parameter), (d, e) => ((Pane)d).OnCanSetAxisXZoomRatio(e)));
			CommandManager.RegisterClassCommandBinding(ownerType,
				new CommandBinding(XYDiagram2DCommands.SetAxisYZoomRatio, (d, e) => ((Pane)d).SetAxisYZoomRatio((double)e.Parameter), (d, e) => ((Pane)d).OnCanSetAxisYZoomRatio(e)));
			CommandManager.RegisterClassCommandBinding(ownerType,
				new CommandBinding(XYDiagram2DCommands.ZoomIntoRectangle, (d, e) => ((Pane)d).ZoomIntoRectangle((Rect)e.Parameter), (d, e) => ((Pane)d).OnCanZoomIntoRectangle(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(XYDiagram2DCommands.ZoomIn, (d, e) => ((Pane)d).ZoomIn((Point?)e.Parameter), (d, e) => ((Pane)d).OnCanZoomIn(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(XYDiagram2DCommands.ZoomOut, (d, e) => ((Pane)d).ZoomOut((Point?)e.Parameter), (d, e) => ((Pane)d).OnCanZoomOut(e)));
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ScrollBarItem AxisXScrollBarItem { get { return (ScrollBarItem)GetValue(AxisXScrollBarItemProperty); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ScrollBarItem AxisYScrollBarItem { get { return (ScrollBarItem)GetValue(AxisYScrollBarItemProperty); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ReadOnlyObservableCollection<SeriesItem> SeriesItems { get { return (ReadOnlyObservableCollection<SeriesItem>)GetValue(SeriesItemsProperty); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ReadOnlyObservableCollection<StripItem> StripItems { get { return (ReadOnlyObservableCollection<StripItem>)GetValue(StripItemsProperty); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ReadOnlyObservableCollection<ConstantLineItem> ConstantLineBehindItems { get { return (ReadOnlyObservableCollection<ConstantLineItem>)GetValue(ConstantLineBehindItemsProperty); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ReadOnlyObservableCollection<ConstantLineItem> ConstantLineInFrontItems { get { return (ReadOnlyObservableCollection<ConstantLineItem>)GetValue(ConstantLineInFrontItemsProperty); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ReadOnlyObservableCollection<ConstantLineTitleItem> ConstantLineTitleItems { get { return (ReadOnlyObservableCollection<ConstantLineTitleItem>)GetValue(ConstantLineTitleItemsProperty); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ReadOnlyObservableCollection<IndicatorItem> IndicatorItems { get { return (ReadOnlyObservableCollection<IndicatorItem>)GetValue(IndicatorItemsProperty); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ReadOnlyObservableCollection<IndicatorLabelItem> IndicatorLabelItems { get { return (ReadOnlyObservableCollection<IndicatorLabelItem>)GetValue(IndicatorLabelItemsProperty); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ObservableCollection<UIElement> GridLines {
			get { return (ObservableCollection<UIElement>)GetValue(GridLinesProperty); }
			set { SetValue(GridLinesProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ObservableCollection<UIElement> InterlaceControls {
			get { return (ObservableCollection<UIElement>)GetValue(InterlaceControlsProperty); }
			set { SetValue(InterlaceControlsProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ObservableCollection<object> PaneItems {
			get { return (ObservableCollection<object>)GetValue(PaneItemsProperty); }
			set { SetValue(PaneItemsProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ObservableCollection<SeriesLabelItem> SeriesLabelItems {
			get { return (ObservableCollection<SeriesLabelItem>)GetValue(SeriesLabelItemsProperty); }
			set { SetValue(SeriesLabelItemsProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ObservableCollection<SeriesPointItem> Pseudo3DPointItems {
			get { return (ObservableCollection<SeriesPointItem>)GetValue(Pseudo3DPointItemsProperty); }
			set { SetValue(Pseudo3DPointItemsProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ObservableCollection<CrosshairLinePresentationData> CrosshairLinesGeometry { get { return (ObservableCollection<CrosshairLinePresentationData>)GetValue(CrosshairLinesGeometryProperty); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Brush CrosshairArgumentBrush {
			get { return (Brush)GetValue(CrosshairArgumentBrushProperty); }
			set { SetValue(CrosshairArgumentBrushProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Brush CrosshairValueBrush {
			get { return (Brush)GetValue(CrosshairValueBrushProperty); }
			set { SetValue(CrosshairValueBrushProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public LineStyle CrosshairArgumentLineStyle {
			get { return (LineStyle)GetValue(CrosshairArgumentLineStyleProperty); }
			set { SetValue(CrosshairArgumentLineStyleProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public LineStyle CrosshairValueLineStyle {
			get { return (LineStyle)GetValue(CrosshairValueLineStyleProperty); }
			set { SetValue(CrosshairValueLineStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DEnableAxisXNavigation"),
#endif
		Category(Categories.Navigation),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public bool? EnableAxisXNavigation {
			get { return (bool?)GetValue(EnableAxisXNavigationProperty); }
			set { SetValue(EnableAxisXNavigationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram2DEnableAxisYNavigation"),
#endif
		Category(Categories.Navigation),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public bool? EnableAxisYNavigation {
			get { return (bool?)GetValue(EnableAxisYNavigationProperty); }
			set { SetValue(EnableAxisYNavigationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PaneAxisXScrollBarOptions"),
#endif
		Category(Categories.Navigation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public ScrollBarOptions AxisXScrollBarOptions {
			get { return (ScrollBarOptions)GetValue(AxisXScrollBarOptionsProperty); }
			set { SetValue(AxisXScrollBarOptionsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PaneAxisYScrollBarOptions"),
#endif
		Category(Categories.Navigation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public ScrollBarOptions AxisYScrollBarOptions {
			get { return (ScrollBarOptions)GetValue(AxisYScrollBarOptionsProperty); }
			set { SetValue(AxisYScrollBarOptionsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PaneDomainBrush"),
#endif
		Category(Categories.Brushes),
		XtraSerializableProperty
		]
		public Brush DomainBrush {
			get { return (Brush)GetValue(DomainBrushProperty); }
			set { SetValue(DomainBrushProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PaneDomainBorderBrush"),
#endif
		Category(Categories.Brushes),
		XtraSerializableProperty
		]
		public Brush DomainBorderBrush {
			get { return (Brush)GetValue(DomainBorderBrushProperty); }
			set { SetValue(DomainBorderBrushProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PaneMirrorHeight"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public double MirrorHeight {
			get { return (double)GetValue(MirrorHeightProperty); }
			set { SetValue(MirrorHeightProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PaneScrollHorizontallyCommand"),
#endif
 Category(Categories.Action)]
		public ICommand ScrollHorizontallyCommand { get; private set; }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PaneScrollVerticallyCommand"),
#endif
 Category(Categories.Action)]
		public ICommand ScrollVerticallyCommand { get; private set; }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PaneScrollAxisXToCommand"),
#endif
 Category(Categories.Action)]
		public ICommand ScrollAxisXToCommand { get; private set; }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PaneScrollAxisYToCommand"),
#endif
 Category(Categories.Action)]
		public ICommand ScrollAxisYToCommand { get; private set; }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PaneSetAxisXRangeCommand"),
#endif
 Category(Categories.Action)]
		public ICommand SetAxisXRangeCommand { get; private set; }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PaneSetAxisYRangeCommand"),
#endif
 Category(Categories.Action)]
		public ICommand SetAxisYRangeCommand { get; private set; }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PaneSetAxisXZoomRatioCommand"),
#endif
 Category(Categories.Action)]
		public ICommand SetAxisXZoomRatioCommand { get; private set; }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PaneSetAxisYZoomRatioCommand"),
#endif
 Category(Categories.Action)]
		public ICommand SetAxisYZoomRatioCommand { get; private set; }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PaneZoomIntoRectangleCommand"),
#endif
 Category(Categories.Action)]
		public ICommand ZoomIntoRectangleCommand { get; private set; }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PaneZoomInCommand"),
#endif
 Category(Categories.Action)]
		public ICommand ZoomInCommand { get; private set; }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PaneZoomOutCommand"),
#endif
 Category(Categories.Action)]
		public ICommand ZoomOutCommand { get; private set; }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Transform LayoutTransform { get { return base.LayoutTransform; } set { base.LayoutTransform = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXBrowsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SelectionInfo SelectionInfo { get { return selectionInfo; } }
		internal static bool MirrorHeightValidation(object mirrorHeight) {
			return (double)mirrorHeight >= 0;
		}
		static void EnableNavigationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Pane pane = d as Pane;
			if (pane != null) {
				ZoomCacheEx zoomCache = pane.ZoomCacheEx;
				if (zoomCache != null)
					zoomCache.Clear();
			}
			ChartElementHelper.Update(d, e);
		}
		static void ScrollBarOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Pane pane = d as Pane;
			if (pane != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as ScrollBarOptions, e.NewValue as ScrollBarOptions, pane);
				pane.UpdateScrollBarItemOptions();
			}
			ChartElementHelper.Update(d, e);
		}
		readonly Dictionary<AxisBase, GridAndTextDataEx> gridAndTextData = new Dictionary<AxisBase, GridAndTextDataEx>();
		readonly List<Axis2DItem> axisItems = new List<Axis2DItem>();
		readonly List<DelegateCommand<object>> commands = new List<DelegateCommand<object>>();
		readonly SeriesLabel2DLayoutCache labelsLayoutCache = new SeriesLabel2DLayoutCache();
		List<CrosshairLabelInfoEx> crosshairLabelsInfo;
		List<AxisLabelResolveOverlappingCache> resolveOverlappingCache;
		PaneItemsControl paneItemsControl;
		DomainPanel panePanel;
		FrameworkElement constantLineTitlesItemsControl;
		ChartMirrorControl mirrorControl;
		ChartMirrorControl pseudo3DMirrorControl;
		Rect viewport;
		Rect cachedViewport = Rect.Empty;
		Rect? rangeBounds;
		SelectionInfo selectionInfo;
		IndicatorElementsPanel indicatorsPanel;
		IndicatorLabelElementsPanel indicatorLabelsPanel;
		XYDiagram2DPanel diagramPanel;
		ConstantLineTitlePanel constantLineTitlesPanel;
		int IPane.PaneIndex {
			get {
				XYDiagram2D diagram = Diagram;
				return diagram == null ? -1 : diagram.Panes.IndexOf(this);
			}
		}
		GRealRect2D? IPane.MappingBounds {
			get {
				Rect? mappingBounds = LastMappingBounds;
				if (mappingBounds.HasValue)
					return new GRealRect2D(mappingBounds.Value.X, mappingBounds.Value.Y, mappingBounds.Value.Width, mappingBounds.Value.Height);
				return null;
			}
		}
		IndicatorElementsPanel IndicatorsPanel {
			get {
				if (indicatorsPanel == null)
					indicatorsPanel = LayoutHelper.FindElement(this, element => element is IndicatorElementsPanel) as IndicatorElementsPanel;
				return indicatorsPanel;
			}
		}
		IndicatorLabelElementsPanel IndicatorLabelsPanel {
			get {
				if (indicatorLabelsPanel == null)
					indicatorLabelsPanel = LayoutHelper.FindElement(this, element => element is IndicatorLabelElementsPanel) as IndicatorLabelElementsPanel;
				return indicatorLabelsPanel;
			}
		}
		XYDiagram2DPanel DiagramPanel {
			get {
				if (diagramPanel == null)
					diagramPanel = LayoutHelper.FindElement(this, element => element is XYDiagram2DPanel) as XYDiagram2DPanel;
				return diagramPanel;
			}
		}
		ConstantLineTitlePanel ConstantLineTitlesPanel {
			get {
				if (constantLineTitlesItemsControl != null && constantLineTitlesPanel == null)
					constantLineTitlesPanel = LayoutHelper.FindElement(constantLineTitlesItemsControl, element => element is ConstantLineTitlePanel) as ConstantLineTitlePanel;
				return constantLineTitlesPanel;
			}
		}
		IEnumerable<IAxisData> Axes {
			get {
				PaneAxesContainer paneAxesData = PaneAxesContainer;
				return paneAxesData == null ? new IAxisData[0] : paneAxesData.Axes;
			}
		}
		ScrollBarOptions ActualAxisXScrollBarOptions { get { return AxisXScrollBarOptions ?? new ScrollBarOptions(); } }
		ScrollBarOptions ActualAxisYScrollBarOptions { get { return AxisYScrollBarOptions ?? new ScrollBarOptions(); } }
		internal bool ActualEnableAxisXNavigation {
			get {
				PaneAxesContainer paneAxesData = PaneAxesContainer;
				if (paneAxesData == null || paneAxesData.AxesX.Count == 0)
					return false;
				bool? enableAxisXNavigation = EnableAxisXNavigation;
				if (enableAxisXNavigation.HasValue)
					return enableAxisXNavigation.Value;
				XYDiagram2D diagram = Diagram;
				return diagram != null && diagram.EnableAxisXNavigation;
			}
		}
		internal bool ActualEnableAxisYNavigation {
			get {
				PaneAxesContainer paneAxesData = PaneAxesContainer;
				if (paneAxesData == null || paneAxesData.AxesY.Count == 0)
					return false;
				bool? enableAxisYNavigation = EnableAxisYNavigation;
				if (enableAxisYNavigation.HasValue)
					return enableAxisYNavigation.Value;
				XYDiagram2D diagram = Diagram;
				return diagram != null && diagram.EnableAxisYNavigation;
			}
		}
		bool ActualCanZoomIn {
			get {
				PaneAxesContainer paneAxesData = PaneAxesContainer;
				return paneAxesData != null && ((ActualEnableAxisXNavigation && paneAxesData.CanZoomInAxis(paneAxesData.PrimaryAxisX)
					|| (ActualEnableAxisYNavigation && paneAxesData.CanZoomInAxis(paneAxesData.PrimaryAxisY))));
			}
		}
		internal XYDiagram2D Diagram { get { return ((IChartElement)this).Owner as XYDiagram2D; } }
		internal Rect Bounds { get { return new Rect(new Point(0, 0), DesiredSize); } }
		internal Rect Viewport {
			get { return viewport; }
			set {
				if (value != viewport) {
					viewport = value;
					InvalidateArrange();
				}
			}
		}
		internal List<Axis2DItem> AxisItems { get { return axisItems; } }
		internal PaneAxesContainer PaneAxesContainer {
			get {
				XYDiagram2D diagram = Diagram;
				if (diagram == null || diagram.PaneAxesContainerRepository == null)
					return null;
				return diagram.PaneAxesContainerRepository.GetContaiter(this);
			}
		}
		internal bool Rotated {
			get {
				XYDiagram2D diagram = Diagram;
				return diagram != null && diagram.Rotated;
			}
		}
		internal bool NavigationEnabled { get { return ActualEnableAxisXNavigation || ActualEnableAxisYNavigation; } }
		internal ZoomCacheEx ZoomCacheEx {
			get {
				XYDiagram2D diagram = Diagram;
				return diagram == null ? null : diagram.ZoomCacheEx;
			}
		}
		internal DomainPanel PanePanel { get { return panePanel; } }
		internal Transform ViewportTransform {
			get {
				TransformGroup transform = new TransformGroup();
				if (Rotated) {
					transform.Children.Add(new RotateTransform() { Angle = -90 });
					transform.Children.Add(new ScaleTransform() { ScaleY = -1 });
				}
				transform.Children.Add(new ScaleTransform() { ScaleY = -1, CenterY = viewport.Height / 2 });
				return transform;
			}
		}
		internal Transform ViewportRenderTransform {
			get {
				TransformGroup transform = new TransformGroup();
				transform.Children.Add(ViewportTransform);
				transform.Children.Add(new TranslateTransform() { X = viewport.X, Y = viewport.Y });
				return transform;
			}
		}
		internal Rect RangeBounds {
			get {
				if (!rangeBounds.HasValue) {
					PaneAxesContainer paneAxesData = PaneAxesContainer;
					if (paneAxesData == null || !NavigationEnabled)
						rangeBounds = viewport;
					else {
						IAxisData horizontalAxis, verticalAxis;
						if (Rotated) {
							horizontalAxis = paneAxesData.PrimaryAxisY;
							verticalAxis = paneAxesData.PrimaryAxisX;
						}
						else {
							horizontalAxis = paneAxesData.PrimaryAxisX;
							verticalAxis = paneAxesData.PrimaryAxisY;
						}
						IMinMaxValues horizontalVisualRange = horizontalAxis.VisualRange;
						IMinMaxValues horizontalWholeRange = horizontalAxis.WholeRange;
						IMinMaxValues verticalVisualRange = verticalAxis.VisualRange;
						IMinMaxValues verticalWholeRange = verticalAxis.WholeRange;
						double horizontalScaleFactor = horizontalWholeRange.Delta / horizontalVisualRange.Delta;
						double horizontalOffset = (horizontalAxis.Reverse ? (horizontalWholeRange.Max - horizontalVisualRange.Max) : (horizontalVisualRange.Min - horizontalWholeRange.Min)) /							horizontalWholeRange.Delta * viewport.Width * horizontalScaleFactor;
						double verticalScaleFactor = verticalWholeRange.Delta / verticalVisualRange.Delta;
						double verticalOffset = (verticalAxis.Reverse ? (verticalVisualRange.Min - verticalWholeRange.Min) : (verticalWholeRange.Max - verticalVisualRange.Max)) /							verticalWholeRange.Delta * viewport.Height * verticalScaleFactor;
						rangeBounds = new Rect(viewport.Left - horizontalOffset, viewport.Top - verticalOffset, viewport.Width * horizontalScaleFactor, viewport.Height * verticalScaleFactor);
					}
				}
				return rangeBounds.Value;
			}
		}
		internal Rect? LastMappingBounds {
			get {
				XYDiagram2D diagram = Diagram;
				if (diagram == null || diagram.ChartControl == null || PanePanel == null)
					return Rect.Empty;
				if (diagram.ChartControl.FindCommonVisualAncestor(PanePanel) == null)
					return Rect.Empty;
				Rect mappingBounds = LayoutHelper.GetRelativeElementRect(PanePanel, diagram.ChartControl);
				if (mappingBounds.Width >= 1 && mappingBounds.Height >= 1) {
					mappingBounds.Width--;
					mappingBounds.Height--;
				}
				return mappingBounds;
			}
		}
		internal SeriesLabel2DLayoutCache LabelsLayoutCache { get { return labelsLayoutCache; } }
		internal bool LegendUseCheckBoxes { get { return Diagram != null && Diagram.ChartControl != null && Diagram.ChartControl.LegendUseCheckBoxes; } }
		public Pane() {
			DefaultStyleKey = typeof(Pane);
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
			selectionInfo = new SelectionInfo();
			this.SetValue(CrosshairLinesGeometryPropertyKey, new ObservableCollection<CrosshairLinePresentationData>());
		}
		#region IZoomablePane implementation
		bool IZoomablePane.Rotated { get { return Rotated; } }
		bool IZoomablePane.ScrollingByXEnabled { get { return ActualEnableAxisXNavigation; } }
		bool IZoomablePane.ScrollingByYEnabled { get { return ActualEnableAxisYNavigation; } }
		bool IZoomablePane.ZoomingByXEnabled { get { return ActualEnableAxisXNavigation; } }
		bool IZoomablePane.ZoomingByYEnabled { get { return ActualEnableAxisYNavigation; } }
		double IZoomablePane.AxisXMaxZoomPercent { get { return Diagram.ActualNavigationOptions.AxisXMaxZoomPercent; } }
		double IZoomablePane.AxisYMaxZoomPercent { get { return Diagram.ActualNavigationOptions.AxisYMaxZoomPercent; } }
		ZoomCacheEx IZoomablePane.ZoomCacheEx { get { return ZoomCacheEx; } }
		GRect2D IZoomablePane.Bounds {
			get {
				XYDiagram2D diagram = Diagram;
				if (diagram == null)
					return new GRect2D();
				DomainPanel panel = PanePanel;
				return GraphicsUtils.ConvertRect(new Rect(panel == null ? new Point() : panel.GetPosition(diagram.ChartControl), new Size(viewport.Width, viewport.Height)));
			}
		}
		void IZoomablePane.RangeLimitsUpdated() {
			PaneAxesContainer paneAxesData = PaneAxesContainer;
			if (paneAxesData != null) {
				Axis2D axis = paneAxesData.PrimaryAxisX as Axis2D;
				if (axis != null)
					axis.RangeLimitsUpdated();
				axis = paneAxesData.PrimaryAxisY as Axis2D;
				if (axis != null)
					axis.RangeLimitsUpdated();
			}
		}
		void IZoomablePane.BeginZooming() {
		}
		void IZoomablePane.EndZooming(NavigationType navigationType, RangesSnapshot oldRange, RangesSnapshot newRange) {
			XYDiagram2D diagram = Diagram;
			if (diagram != null)
				diagram.RaiseZoomEvent(navigationType, oldRange, newRange, this);
		}
		void IZoomablePane.EndScrolling(ScrollingOrientation orientation, NavigationType navigationType, RangesSnapshot oldRange, RangesSnapshot newRange) {
			XYDiagram2D diagram = Diagram;
			if (diagram != null)
				diagram.RaiseScrollEvent(orientation, navigationType, oldRange, newRange, this);
		}
		#endregion
		#region IToolTipDockTarget implementation
		Rect IDockTarget.GetBounds() {
			Rect paneRect = LayoutHelper.GetRelativeElementRect(this, ((IChartElement)this).Owner.Owner as ChartControl);
			return new Rect(paneRect.Left + Viewport.Left, paneRect.Top + Viewport.Top, Viewport.Width, Viewport.Height);
		}
		#endregion
		#region IInteractiveElement implementation
		bool IInteractiveElement.IsHighlighted {
			get { return selectionInfo.IsHighlighted; }
			set { selectionInfo.IsHighlighted = value; }
		}
		bool IInteractiveElement.IsSelected {
			get { return selectionInfo.IsSelected; }
			set { selectionInfo.IsSelected = value; }
		}
		object IInteractiveElement.Content { get { return this; } }
		#endregion
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if (managerType != typeof(PropertyChangedWeakEventManager) || !(sender is ScrollBarOptions))
				return false;
			UpdateScrollBarItemOptions();
			ChartElementHelper.Update(this);
			return true;
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
		internal List<ISupportVisibilityControlElement> GetElementsForAutoLayout() {
			List<Axis2D> axes = new List<Axis2D>();
			if (PaneAxesContainer != null) {
				foreach (IAxisData axisData in PaneAxesContainer.Axes) {
					Axis2D axis = axisData as Axis2D;
					if (axis != null)
						axes.Add(axis);
				}
			}
			List<ISupportVisibilityControlElement> elements = new List<ISupportVisibilityControlElement>();
			foreach (Axis2D axis in axes)
				if (axis.Title != null && !axis.Title.Visible.HasValue)
					elements.Add(axis.Title);
			axes.RemoveAll(x => { return x.Visible.HasValue; });
			elements.AddRange(axes.ConvertAll(x => { return x as ISupportVisibilityControlElement; }));
			return elements;
		}
		void UpdateAxisScrollBarCommand(ScrollBarItem scrollBarItem, ICommand command) {
			if (scrollBarItem != null) {
				XYDiagram2D diagram = Diagram;
				if (diagram != null)
					scrollBarItem.Command = diagram.ActualNavigationOptions.UseScrollBars ? command : null;
			}
		}
		void UpdateScrollBarItemOptions() {
			ScrollBarItem item = AxisXScrollBarItem;
			if (item != null)
				item.Options = ActualAxisXScrollBarOptions;
			item = AxisYScrollBarItem;
			if (item != null)
				item.Options = ActualAxisYScrollBarOptions;
		}
		internal void UpdateCrosshairAppearance() {
			if (Diagram != null && Diagram.ChartControl != null && Diagram.ChartControl.CrosshairOptions != null) {
				CrosshairArgumentBrush = Diagram.ChartControl.ActualCrosshairOptions.ActualArgumentLineBrush;
				CrosshairValueBrush = Diagram.ChartControl.ActualCrosshairOptions.ActualValueLineBrush;
				CrosshairArgumentLineStyle = Diagram.ChartControl.ActualCrosshairOptions.ActualArgumentLineStyle;
				CrosshairValueLineStyle = Diagram.ChartControl.ActualCrosshairOptions.ActualValueLineStyle;
			}
		}
		GPoint2D GetZoomCenterPoint(Point? point) {
			XYDiagram2D diagram = Diagram;
			Point center = point ?? new Point(viewport.Left + viewport.Width / 2, viewport.Top + viewport.Height / 2);
			return new GPoint2D(MathUtils.StrongRound(center.X), MathUtils.StrongRound(center.Y));
		}
		Dictionary<AxisPosition, Axis2DItem> GetFirstAxesItems() {
			Dictionary<AxisPosition, Axis2DItem> firstAxesItems = new Dictionary<AxisPosition, Axis2DItem>();
			foreach (Axis2DItem axisItem in axisItems) {
				Axis2D axis = axisItem.Axis as Axis2D;
				AxisPosition axisPosition = axis.Position;
				if (!firstAxesItems.ContainsKey(axisPosition) && (axis.ActualVisible))
					firstAxesItems.Add(axisPosition, axisItem);
				axisItem.SetScrollBarLayout(null);
			}
			return firstAxesItems;
		}
		DelegateCommand<object> CreateDelegateCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod) {
			DelegateCommand<object> command = DelegateCommandFactory.Create<object>(executeMethod, canExecuteMethod, false);
			commands.Add(command);
			return command;
		}
		internal GridAndTextDataEx GetGridAndTextData(AxisBase axis) {
			GridAndTextDataEx item;
			return gridAndTextData.TryGetValue(axis, out item) ? item : null;
		}
		internal void RaiseCanExecutedChanged() {
			commands.ForEach(command => command.RaiseCanExecuteChanged());
		}
		internal void Update() {
			UpdateScrollBarItems();
			XYDiagram2D diagram = Diagram;
			ObservableCollection<SeriesItem> collection = new ObservableCollection<SeriesItem>();
			if (diagram != null)
				if (diagram.ChartControl != null)
					foreach (RefinedSeries refinedSeries in diagram.ViewController.ActiveRefinedSeries) {
						XYSeries2D xySeries = refinedSeries.Series as XYSeries2D;
						if (xySeries != null && xySeries.ActualPane == this && xySeries.GetActualVisible()) {
							if (xySeries is AreaStackedSeries2D)
								collection.Insert(0, xySeries.Item);
							else
								collection.Add(xySeries.Item);
						}
					}
			int count = collection.Count;
			ReadOnlyObservableCollection<SeriesItem> actualSeriesItems = SeriesItems;
			if (actualSeriesItems == null || actualSeriesItems.Count != count)
				this.SetValue(SeriesItemsPropertyKey, new ReadOnlyObservableCollection<SeriesItem>(collection));
			else
				for (int i = 0; i < count; i++)
					if (!Object.ReferenceEquals(actualSeriesItems[i], collection[i])) {
						this.SetValue(SeriesItemsPropertyKey, new ReadOnlyObservableCollection<SeriesItem>(collection));
						return;
					}
			UpdateIndicatorItems();
			UpdateCrosshairAppearance();
		}
		internal void CalculateIndicatorItems() {
			if (Diagram != null && Diagram.IndicatorsRepository.ContainsKey(this)) {
				List<Indicator> indicators = Diagram.IndicatorsRepository[this];
				foreach (Indicator indicator in indicators) {
					XYSeries2D series = indicator.XYSeries;
					if (series != null && series.GetActualVisible()) {
						indicator.CalculateLayout(series.Item.RefinedSeries);
						indicator.CreateLabelItems();
					}
				}
			}
		}
		internal void UpdateIndicatorItems() {
			ObservableCollection<IndicatorItem> indicatorCollection = new ObservableCollection<IndicatorItem>();
			ObservableCollection<IndicatorLabelItem> indicatorLabelCollection = new ObservableCollection<IndicatorLabelItem>();
			if (Diagram != null && Diagram.IndicatorsRepository.ContainsKey(this)) {
				List<Indicator> indicators = Diagram.IndicatorsRepository[this];
				int indicatorIndex = 0;
				IndicatorsPalette indicatorsPalette = Diagram.ChartControl != null ? Diagram.ChartControl.IndicatorsPalette : new DefaultIndicatorsPalette();
				foreach (Indicator indicator in indicators) {
					if (indicator.Visible) {
						indicator.Item.Brush = indicator.BrushInternal != null || indicatorsPalette == null ? indicator.BrushInternal : new SolidColorBrush(indicatorsPalette[indicatorIndex]);
						indicatorIndex++;
						indicatorCollection.Add(indicator.Item);
						if (indicator.LabelItems != null && indicator.Item.Label.Visible) {
							foreach (IndicatorLabelItem labelItem in indicator.LabelItems)
								indicatorLabelCollection.Add(labelItem);
						}
					}
				}
			}
			this.SetValue(IndicatorItemsPropertyKey, indicatorCollection.Count > 0 ? new ReadOnlyObservableCollection<IndicatorItem>(indicatorCollection) : null);
			this.SetValue(IndicatorLabelItemsPropertyKey, indicatorCollection.Count > 0 ? new ReadOnlyObservableCollection<IndicatorLabelItem>(indicatorLabelCollection): null);
		}
		internal bool UpdateAxesLabelItems(Size size) {
			XYDiagram2D diagram = Diagram;
			bool navigationEnabled = diagram != null && diagram.IsNavigationEnabled;
			bool axisXNavigationEnabled = ActualEnableAxisXNavigation;
			bool axisYNavigationEnabled = ActualEnableAxisYNavigation;
			gridAndTextData.Clear();
			foreach (Axis2D axis in Axes) {
				double axisLength = axis.IsVertical ? size.Height : size.Width;
				IMinMaxValues visualRange = ((IAxisData)axis).VisualRange;
				if (axisLength > 0 && !Double.IsNaN(visualRange.Min) && !Double.IsNaN(visualRange.Max)) {
					bool shouldUseScrollingLimits = axis.IsValuesAxis ? axisYNavigationEnabled : axisXNavigationEnabled;
					var wholeRange = shouldUseScrollingLimits ? ((IAxisData)axis).WholeRange : visualRange;
					gridAndTextData.Add(axis, new GridAndTextDataEx(axis.GetSeries(), axis, navigationEnabled, visualRange, wholeRange, axisLength, false));
				}
			}
			bool isAxesLabelItemsUpdated = false;
			foreach (Axis2DItem item in axisItems) {
				GridAndTextDataEx itemGridAndTextData = GetGridAndTextData(item.Axis as Axis2D);
				isAxesLabelItemsUpdated |= item.UpdateLabelItems(itemGridAndTextData != null ? itemGridAndTextData.TextData : null);
			}
			return isAxesLabelItemsUpdated;
		}
		internal void UpdateAxisItems() {
			ObservableCollection<object> items = new ObservableCollection<object>();
			foreach (Axis2DItem axisItem in axisItems) {
				items.Add(axisItem);
				IEnumerable<AxisLabelItem> labelItems = axisItem.LabelItems;
				if (labelItems != null)
					foreach (AxisLabelItem labelItem in labelItems)
						items.Add(labelItem);
				AxisTitleItem titleItem = axisItem.TitleItem;
				if (titleItem != null)
					items.Add(titleItem);
			}
			foreach (Axis2DItem axisItem in axisItems)
				items.Add(axisItem.SelectionGeometryItem);
			PaneItems = items;
		}
		internal void UpdateVisualItems() {
			axisItems.Clear();
			ObservableCollection<UIElement> gridLines = new ObservableCollection<UIElement>();
			ObservableCollection<UIElement> interlaceControls = new ObservableCollection<UIElement>();
			ObservableCollection<StripItem> stripItems = new ObservableCollection<StripItem>();
			ObservableCollection<ConstantLineItem> constantLineBehindItems = new ObservableCollection<ConstantLineItem>();
			ObservableCollection<ConstantLineItem> constantLineInFrontItems = new ObservableCollection<ConstantLineItem>();
			ObservableCollection<ConstantLineTitleItem> constantLineTitleItems = new ObservableCollection<ConstantLineTitleItem>();
			foreach (Axis2D axis in Axes) {
				gridLines.Add(new GridLinesControl(axis));
				interlaceControls.Add(new InterlaceControl(axis));
				foreach (Strip strip in axis.Strips) {
					StripItem item = new StripItem(strip);
					stripItems.Add(item);
					strip.StripItems.Add(item);
				}
				axis.ConstantLinesBehind.FillConstantLineItems(constantLineBehindItems, constantLineTitleItems);
				axis.ConstantLinesInFront.FillConstantLineItems(constantLineInFrontItems, constantLineTitleItems);
				if (axis.ActualVisibilityInPanes.IsPaneVisible(this))
					axisItems.Add(new Axis2DItem(axis, axis.Title == null ? null : new AxisTitleItem(axis.Title)));
			}
			GridLines = gridLines;
			InterlaceControls = interlaceControls;
			this.SetValue(StripItemsPropertyKey, new ReadOnlyObservableCollection<StripItem>(stripItems));
			this.SetValue(ConstantLineBehindItemsPropertyKey, new ReadOnlyObservableCollection<ConstantLineItem>(constantLineBehindItems));
			this.SetValue(ConstantLineInFrontItemsPropertyKey, new ReadOnlyObservableCollection<ConstantLineItem>(constantLineInFrontItems));
			this.SetValue(ConstantLineTitleItemsPropertyKey, new ReadOnlyObservableCollection<ConstantLineTitleItem>(constantLineTitleItems));
			UpdateAxisItems();
		}
		internal void UpdateSeriesItems() {
			ReadOnlyCollection<SeriesItem> seriesItemsCollection = SeriesItems;
			if (seriesItemsCollection != null) {
				List<SeriesPointItem> pseudo3DPointItems = new List<SeriesPointItem>();
				List<XYSeries2D> pseudo3DSeriesCollection = new List<XYSeries2D>();
				foreach (SeriesItem seriesItem in seriesItemsCollection) {
					if (seriesItem.Series.InFrontOfAxes) {
						foreach (SeriesPointItem pointItem in seriesItem.AllPointItems)
							pseudo3DPointItems.Add(pointItem);
						XYSeries2D series = seriesItem.Series as XYSeries2D;
						if (series != null)
							pseudo3DSeriesCollection.Add(series);
					}
				}
				pseudo3DPointItems.Sort(new SeriesPointDrawingOrderComparer(pseudo3DSeriesCollection));
				ObservableCollection<SeriesPointItem> pseudo3DPointPresentations = new ObservableCollection<SeriesPointItem>();
				foreach (SeriesPointItem pointItem in pseudo3DPointItems)
					pseudo3DPointPresentations.Add(pointItem);
				this.SetValue(Pseudo3DPointItemsProperty,pseudo3DPointPresentations.Count > 0 ? pseudo3DPointPresentations : null);
				ObservableCollection<SeriesLabelItem> seriesLabelItems = new ObservableCollection<SeriesLabelItem>();
				foreach (SeriesItem seriesItem in seriesItemsCollection) {
					SeriesLabel label = seriesItem.Series.ActualLabel;
					if (label != null) {
						IEnumerable<SeriesLabelItem> items = label.Items;
						if (items != null)
							foreach (SeriesLabelItem labelItem in items)
								seriesLabelItems.Add(labelItem);
					}
				}
				SeriesLabelItems = seriesLabelItems.Count > 0 ? seriesLabelItems : null;
			}
		}
		internal void EnsureItems() {
			if (PaneItems == null)
				UpdateAxisItems();
		}
		internal void UpdateScrollBarItems() {
			ScrollBarItem scrollBarItem = AxisXScrollBarItem;
			if (ActualEnableAxisXNavigation) {
				if (scrollBarItem == null) {
					scrollBarItem = new ScrollBarItem() { Options = ActualAxisXScrollBarOptions };
					this.SetValue(AxisXScrollBarItemPropertyKey, scrollBarItem);
				}
				UpdateAxisScrollBarCommand(scrollBarItem, SetAxisXRangeCommand);
			}
			else if (scrollBarItem != null)
				this.SetValue(AxisXScrollBarItemPropertyKey, null);
			scrollBarItem = AxisYScrollBarItem;
			if (ActualEnableAxisYNavigation) {
				if (scrollBarItem == null) {
					scrollBarItem = new ScrollBarItem() { Options = ActualAxisYScrollBarOptions };
					this.SetValue(AxisYScrollBarItemPropertyKey, scrollBarItem);
				}
				UpdateAxisScrollBarCommand(scrollBarItem, SetAxisYRangeCommand);
			}
			else if (scrollBarItem != null)
				this.SetValue(AxisYScrollBarItemPropertyKey, null);
			UpdateScrollBarItemsOrientation();
		}
		internal void UpdateScrollBarItemsOrientation() {
			XYDiagram2D diagram = Diagram;
			bool rotated = diagram != null && diagram.Rotated;
			ScrollBarItem item = AxisXScrollBarItem;
			if (item != null)
				item.Orientation = rotated ? Orientation.Vertical : Orientation.Horizontal;
			item = AxisYScrollBarItem;
			if (item != null)
				item.Orientation = rotated ? Orientation.Horizontal : Orientation.Vertical;
		}
		internal void CalculateOuterItemsLayout() {
			CalculateOuterItemsLayout(GetFirstAxesItems(), true);
		}
		Thickness CalculateOuterItemsLayout(Dictionary<AxisPosition, Axis2DItem> firstAxesItems, bool storeLabelsToCache) {
			ScrollBarItem axisXScrollBarItem = AxisXScrollBarItem;
			ScrollBarItem axisYScrollBarItem = AxisYScrollBarItem;
			IEnumerable<SeriesItem> seriesItemsCollection = SeriesItems;
			if (seriesItemsCollection != null)
				foreach (SeriesItem seriesItem in seriesItemsCollection) {
					XYSeries2D series = seriesItem.Series as XYSeries2D;
					if (series != null)
						series.CreateSeriesLabelsLayout();
				}
			if (axisXScrollBarItem != null)
				axisXScrollBarItem.CreateScrollBarLayout(firstAxesItems, viewport);
			if (axisYScrollBarItem != null)
				axisYScrollBarItem.CreateScrollBarLayout(firstAxesItems, viewport);
			Thickness axesOffsets = new Thickness(0);
			resolveOverlappingCache = null;
			foreach (Axis2DItem axisItem in axisItems) {
				AxisLabelResolveOverlappingCache cacheItem = null;
				Axis2DElementsLayoutCalculator.CalculateLayout(axisItem, viewport, Bounds, NavigationEnabled, ref axesOffsets, firstAxesItems.ContainsValue(axisItem), ref cacheItem);
				if (resolveOverlappingCache == null)
					resolveOverlappingCache = new List<AxisLabelResolveOverlappingCache>();
				if (cacheItem != null && storeLabelsToCache)
					resolveOverlappingCache.Add(cacheItem);
			}
			return axesOffsets;
		}
		internal void UpdateCache() {
			if (resolveOverlappingCache != null)
				foreach (AxisLabelResolveOverlappingCache cache in resolveOverlappingCache) {
					cache.Store();
				}
		}
		internal void ClearViewportCache() {
			cachedViewport = Rect.Empty;
		}
		internal void CalculateOuterItemsLayoutAndViewport(bool shouldUpdateViewportCache) {
			foreach (Axis2DItem axisItem in axisItems)
				axisItem.TitleIndent = 0.0;
			rangeBounds = null;
			Rect bounds = Bounds;
			Rect viewport;
			if (Diagram.UseViewportCache && NavigationEnabled && !cachedViewport.IsEmpty)
				viewport = cachedViewport;
			else
				viewport = bounds;
			Dictionary<AxisPosition, Axis2DItem> firstAxesItems = GetFirstAxesItems();
			for (; ; ) {
				Viewport = viewport;
				Thickness axesOffsets = CalculateOuterItemsLayout(firstAxesItems, shouldUpdateViewportCache);
				if (viewport.Width <= 0 || viewport.Height <= 0)
					break;
				Rect actualRect = new Rect(new Point(viewport.Left + axesOffsets.Left, viewport.Top + axesOffsets.Top),
										   new Point(viewport.Right + axesOffsets.Right, viewport.Bottom + Math.Max(axesOffsets.Bottom, MirrorHeight)));
				foreach (Axis2DItem axisItem in axisItems) {
					Rect labelRect = axisItem.LabelRect;
					if (labelRect != RectExtensions.Zero)
						actualRect.Union(labelRect);
				}
				if (NavigationEnabled) {
					actualRect = LayoutElementHelper.UnionRect(actualRect, AxisXScrollBarItem);
					actualRect = LayoutElementHelper.UnionRect(actualRect, AxisYScrollBarItem);
				}
				else {
					IEnumerable<SeriesItem> seriesItemsCollection = SeriesItems;
					if (seriesItemsCollection != null)
						foreach (SeriesItem seriesItem in seriesItemsCollection) {
							IEnumerable<SeriesLabelItem> labelItems = seriesItem.Series.ActualLabel.Items;
							if (labelItems != null)
								foreach (SeriesLabelItem labelItem in labelItems)
									actualRect = LayoutElementHelper.UnionRect(actualRect, labelItem);
						}
				}
				Thickness correction = new Thickness(MathUtils.StrongRound(Math.Max(bounds.Left - actualRect.Left, 0)), MathUtils.StrongRound(Math.Max(bounds.Top - actualRect.Top, 0)),
													 MathUtils.StrongRound(Math.Max(actualRect.Right - bounds.Right, 0)), MathUtils.StrongRound(Math.Max(actualRect.Bottom - bounds.Bottom, 0)));
				if (GraphicsUtils.IsThicknessEmpty(correction)) {
					UpdateCache();
					break;
				}
				double left = viewport.Left + correction.Left;
				double top = viewport.Top + correction.Top;
				viewport = new Rect(left, top, Math.Max(viewport.Right - correction.Right - left, 0), Math.Max(viewport.Bottom - correction.Bottom - top, 0));
			}
			if (shouldUpdateViewportCache)
				cachedViewport = viewport;
		}
		internal void ScrollHorizontally(int dx, NavigationType navigationType) {
			PaneAxesContainer paneAxesData = PaneAxesContainer;
			if (paneAxesData != null)
				paneAxesData.Scroll(dx, 0, true, navigationType);
		}
		internal void ScrollVertically(int dy, NavigationType navigationType) {
			PaneAxesContainer paneAxesData = PaneAxesContainer;
			if (paneAxesData != null)
				paneAxesData.Scroll(0, dy, true, navigationType);
		}
		internal void Scroll(int dx, int dy, NavigationType navigationType) {
			PaneAxesContainer paneAxesData = PaneAxesContainer;
			if (paneAxesData != null)
				paneAxesData.Scroll(dx, dy, true, navigationType);
		}
		internal bool CanScroll(int dx, int dy) {
			PaneAxesContainer paneAxesData = PaneAxesContainer;
			return paneAxesData != null && paneAxesData.CanScroll(dx, dy, true);
		}
		internal bool PerformZoom(int delta, ZoomingKind kind) {
			PaneAxesContainer paneAxesData = PaneAxesContainer;
			if (paneAxesData != null && (delta > 0 && ActualCanZoomIn || delta < 0 && CanZoomOut())) {
				paneAxesData.Zoom(delta, kind);
				return true;
			}
			return false;
		}
		internal void Zoom(Point center, double manipulationStartAxisXMin, double manipulationStartAxisXMax, double manipulationStartAxisYMin, double manipulationStartAxisYMax, double xZoomPercent, double yZoomPercent, ZoomingKind zoomingKind, NavigationType navigationType) {
			PaneAxesContainer paneAxesData = PaneAxesContainer;
			if (paneAxesData != null)
				paneAxesData.Zoom(GetZoomCenterPoint(center), manipulationStartAxisXMin, manipulationStartAxisXMax, manipulationStartAxisYMin,
					manipulationStartAxisYMax, xZoomPercent, yZoomPercent, zoomingKind, navigationType);
		}
		internal GRealRect2D GetLabelBounds(Axis2D axis) {
			foreach (object item in PaneItems) {
				if (item is Axis2DItem && ((Axis2DItem)item).Axis == axis) {
					Rect labelRect = ((Axis2DItem)item).LabelRect;
					Rect mappingBounds = LastMappingBounds.Value;
					double x = labelRect.Left - viewport.Left + mappingBounds.Left;
					double y = labelRect.Top - viewport.Top + mappingBounds.Top;
					return labelRect.IsEmpty ? GRealRect2D.Empty : new GRealRect2D(x, y, labelRect.Width, labelRect.Height);
				}
			}
			return GRealRect2D.Empty;
		}
		internal void ClearCrosshair() {
			if (CrosshairLinesGeometry != null)
				CrosshairLinesGeometry.Clear();
			crosshairLabelsInfo = null;
		}
		internal void UpdateCrosshairLabelsInfo(List<CrosshairLabelInfoEx> labelsInfos) {
			crosshairLabelsInfo = labelsInfos;
		}
		internal void CompleteCrosshairLabelsLayout() {
			if (crosshairLabelsInfo != null) {
				Rect mappingBounds = LastMappingBounds.Value;
				GRealRect2D constraintBounds = new GRealRect2D(mappingBounds.X, mappingBounds.Y, viewport.Width, viewport.Height);
				List<IAnnotationLayout> crosshairSeriesLabelLayouts = new List<IAnnotationLayout>();
				foreach (CrosshairLabelInfoEx labelInfo in crosshairLabelsInfo)
					crosshairSeriesLabelLayouts.Add(labelInfo);
				AnnotationLayoutCalculator.CalculateAutoLayout(crosshairSeriesLabelLayouts, constraintBounds);
			}
		}
		internal void InvalidateInnerPanels() {
			if (ConstantLineTitlesPanel != null)
				ConstantLineTitlesPanel.InvalidateMeasure();
			if (IndicatorsPanel != null)
				IndicatorsPanel.InvalidateMeasure();
			if (IndicatorLabelsPanel != null)
				IndicatorLabelsPanel.InvalidateMeasure();
			if (DiagramPanel != null)
				DiagramPanel.InvalidateMeasure();
		}
		internal void UpdateAxesElementsItemsLayout() {
			foreach (GridLinesControl gridLinesControl in GridLines) {
				GridAndTextDataEx gridAndTextData = GetGridAndTextData(gridLinesControl.Axis);
				gridLinesControl.UpdateItems(gridAndTextData != null ? gridAndTextData.GridData : null, viewport);
			}
			foreach (InterlaceControl interlaceControl in InterlaceControls) {
				IList<InterlacedData> interlacedData = new InterlacedData[0];
				AxisBase axis = interlaceControl.Axis;
				if (axis.Interlaced) {
					GridAndTextDataEx gridAndTextData = GetGridAndTextData(interlaceControl.Axis);
					if (gridAndTextData != null)
						interlacedData = gridAndTextData.GridData.InterlacedData;
				}
				interlaceControl.UpdateInterlaceItems(interlacedData, viewport);
			}
			foreach (StripItem stripItem in StripItems)
				stripItem.UpdateLayout(Viewport, LastMappingBounds);
			foreach (ConstantLineItem constantLineItem in ConstantLineBehindItems)
				constantLineItem.UpdateLayout(Viewport, LastMappingBounds);
			foreach (ConstantLineItem constantLineItem in ConstantLineInFrontItems)
				constantLineItem.UpdateLayout(Viewport, LastMappingBounds);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			paneItemsControl = GetTemplateChild("PART_PaneItemsControl") as PaneItemsControl;
			panePanel = GetTemplateChild("PART_Domain") as DomainPanel;
			constantLineTitlesItemsControl = GetTemplateChild("PART_ConstantLineTitles") as FrameworkElement;
			mirrorControl = GetTemplateChild("PART_Mirror") as ChartMirrorControl;
			pseudo3DMirrorControl = GetTemplateChild("PART_Pseudo3DMirror") as ChartMirrorControl;
			indicatorsPanel = null;
			indicatorLabelsPanel = null;
			diagramPanel = null;
			constantLineTitlesPanel = null;
		}
		public void ScrollHorizontally(int dx) {
			ScrollHorizontally(dx, NavigationType.Command);
		}
		public bool CanScrollHorizontally(int dx) {
			PaneAxesContainer paneAxesData = PaneAxesContainer;
			return paneAxesData != null && paneAxesData.CanScroll(dx, 0, false);
		}
		public void ScrollVertically(int dy) {
			ScrollVertically(dy, NavigationType.Command);
		}
		public bool CanScrollVertically(int dy) {
			PaneAxesContainer paneAxesData = PaneAxesContainer;
			return paneAxesData != null && paneAxesData.CanScroll(0, dy, false);
		}
		public void ScrollAxisXTo(double xPosition) {
			PaneAxesContainer paneAxesData = PaneAxesContainer;
			if (paneAxesData != null)
				paneAxesData.ScrollTo(xPosition, ScrollingOrientation.AxisXScroll, NavigationType.Command);
		}
		public bool CanScrollAxisXTo(double xPosition) {
			PaneAxesContainer paneAxesData = PaneAxesContainer;
			return paneAxesData != null && paneAxesData.CanScrollTo(xPosition, ScrollingOrientation.AxisXScroll);
		}
		public void ScrollAxisYTo(double yPosition) {
			PaneAxesContainer paneAxesData = PaneAxesContainer;
			if (paneAxesData != null)
				paneAxesData.ScrollTo(yPosition, ScrollingOrientation.AxisYScroll, NavigationType.Command);
		}
		public bool CanScrollAxisYTo(double yPosition) {
			PaneAxesContainer paneAxesData = PaneAxesContainer;
			return paneAxesData != null && paneAxesData.CanScrollTo(yPosition, ScrollingOrientation.AxisYScroll);
		}
		public void SetAxisXRange(AxisRangePositions positions) {
			PaneAxesContainer paneAxesData = PaneAxesContainer;
			if (paneAxesData != null)
				paneAxesData.SetAxisXRange(positions.Position1, positions.Position2, positions.NavigationType);
		}
		public bool CanSetAxisXRange() {
			return ActualEnableAxisXNavigation;
		}
		public void SetAxisYRange(AxisRangePositions positions) {
			PaneAxesContainer paneAxesData = PaneAxesContainer;
			if (paneAxesData != null)
				paneAxesData.SetAxisYRange(positions.Position1, positions.Position2, positions.NavigationType);
		}
		public bool CanSetAxisYRange() {
			return ActualEnableAxisYNavigation;
		}
		public void SetAxisXZoomRatio(double ratio) {
			PaneAxesContainer paneAxesData = PaneAxesContainer;
			if (paneAxesData != null)
				paneAxesData.SetAxisXZoom(ratio, NavigationType.Command);
		}
		public bool CanSetAxisXZoomRatio() {
			return ActualEnableAxisXNavigation;
		}
		public void SetAxisYZoomRatio(double ratio) {
			PaneAxesContainer paneAxesData = PaneAxesContainer;
			if (paneAxesData != null)
				paneAxesData.SetAxisYZoom(ratio, NavigationType.Command);
		}
		public bool CanSetAxisYZoomRatio() {
			return ActualEnableAxisYNavigation;
		}
		public void ZoomIntoRectangle(Rect rectangle) {
			PaneAxesContainer paneAxesData = PaneAxesContainer;
			if (paneAxesData != null)
				paneAxesData.ZoomIn(GraphicsUtils.ConvertRect(rectangle));
		}
		public bool CanZoomIntoRectangle() {
			return ActualCanZoomIn;
		}
		public void ZoomIn(Point? zoomLocation) {
			PaneAxesContainer paneAxesData = PaneAxesContainer;
			if (paneAxesData != null)
				PaneAxesContainer.Zoom(GetZoomCenterPoint(zoomLocation), 1.0 / zoomFactor, ZoomingKind.ZoomIn, NavigationType.ZoomIn);
		}
		public bool CanZoomIn() {
			return ActualCanZoomIn;
		}
		public void ZoomOut(Point? zoomLocation) {
			PaneAxesContainer paneAxesData = PaneAxesContainer;
			if (paneAxesData != null)
				paneAxesData.Zoom(GetZoomCenterPoint(zoomLocation), zoomFactor, ZoomingKind.ZoomOut, NavigationType.ZoomOut);
		}
		public bool CanZoomOut() {
			PaneAxesContainer paneAxesData = PaneAxesContainer;
			return paneAxesData != null &&
				(ActualEnableAxisXNavigation && PaneAxesContainer.CanZoomOutAxis(paneAxesData.PrimaryAxisX)) || (ActualEnableAxisYNavigation && PaneAxesContainer.CanZoomOutAxis(paneAxesData.PrimaryAxisY));
		}
		public bool ShouldSerializeSeriesItems(XamlDesignerSerializationManager manager) {
			return false;
		}
		public bool ShouldSerializeStripItems(XamlDesignerSerializationManager manager) {
			return false;
		}
		public bool ShouldSerializeConstantLineBehindItems(XamlDesignerSerializationManager manager) {
			return false;
		}
		public bool ShouldSerializeConstantLineInFrontItems(XamlDesignerSerializationManager manager) {
			return false;
		}
		public bool ShouldSerializeConstantLineTitleItems(XamlDesignerSerializationManager manager) {
			return false;
		}
		public bool ShouldSerializeIndicatorItems(XamlDesignerSerializationManager manager) {
			return false;
		}
		public bool ShouldSerializeIndicatorLabelItems(XamlDesignerSerializationManager manager) {
			return false;
		}
		public bool ShouldSerializeGridLines(XamlDesignerSerializationManager manager) {
			return false;
		}
		public bool ShouldSerializeInterlaceControls(XamlDesignerSerializationManager manager) {
			return false;
		}
		public bool ShouldSerializePaneItems(XamlDesignerSerializationManager manager) {
			return false;
		}
		public bool ShouldSerializeSeriesLabels(XamlDesignerSerializationManager manager) {
			return false;
		}
		public bool ShouldSerializeSeriesLabelItems(XamlDesignerSerializationManager manager) {
			return false;
		}
		public bool ShouldSerializePseudo3DPointItems(XamlDesignerSerializationManager manager) {
			return false;
		}
		public bool ShouldSerializeCrosshairLinesGeometry(XamlDesignerSerializationManager manager) {
			return false;
		}
		internal void FillIndicatorsLegendItems(List<LegendItem> legendItems) {
			UpdateIndicatorItems();
			ReadOnlyObservableCollection<IndicatorItem> indicators = IndicatorItems;
			if (indicators != null) {
				foreach (IndicatorItem indicatorItem in IndicatorItems)
					if (indicatorItem.ShowInLegend) {
						LegendItem indicatorLegendItem = new LegendItem(indicatorItem.Indicator, indicatorItem.Indicator, indicatorItem.LegendText, null, indicatorItem.Brush, indicatorItem.LineStyle, indicatorItem.XYSeries2D.GetActualVisible());
						indicatorItem.LegendItem = indicatorLegendItem;
						legendItems.Add(indicatorLegendItem);
					}
			}
		}
	}
	public class PaneCollection : ChartElementCollection<Pane>, IEnumerable<IPane> {
		protected override ChartElementChange Change {
			get { return base.Change | ChartElementChange.UpdateXYDiagram2DItems | ChartElementChange.UpdateActualPanes; }
		}
		internal bool ShouldUpdateActualPanes(IList<Pane> actualPanes) {
			if (actualPanes != null && actualPanes.Count - 1 == this.Count) {
				for (int i = 0; i < Count; i++)
					if (!object.ReferenceEquals(this[i], actualPanes[i + 1]))
						return true;
				return false;
			}
			return true;
		}
		IEnumerator<IPane> IEnumerable<IPane>.GetEnumerator() {
			foreach (IPane pane in this)
				yield return pane;
		}
	}
}
namespace DevExpress.Xpf.Charts.Native {
	public sealed class VisibilityControllerState {
		static VisibilityControllerState state;
		List<PaneAutoLayoutWrapper> wrappers;
		public static VisibilityControllerState State {
			get {
				if (state == null)
					state = new VisibilityControllerState();
				return state;
			}
		}
		VisibilityControllerState() {
			wrappers = new List<PaneAutoLayoutWrapper>();
		}
		public void Add(PaneAutoLayoutWrapper element) {
			if (!wrappers.Contains(element))
				wrappers.Add(element);
		}
		public void Add(List<PaneAutoLayoutWrapper> elements) {
			foreach (PaneAutoLayoutWrapper element in elements)
				Add(element);
		}
		public void Reset() {
			wrappers = new List<PaneAutoLayoutWrapper>();
		}
	}
	public class PaneAutoLayoutWrapper {
		readonly Pane pane;
		readonly List<ISupportVisibilityControlElement> elements;
		readonly Size minSize;
		public Rect Bounds { get { return pane.Bounds; } }
		public PaneAutoLayoutWrapper(Pane pane, List<ISupportVisibilityControlElement> elements, Size minSize) {
			this.pane = pane;
			this.elements = elements;
			this.minSize = minSize;
		}
	}
}
