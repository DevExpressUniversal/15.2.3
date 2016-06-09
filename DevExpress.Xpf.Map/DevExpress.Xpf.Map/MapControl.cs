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
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using DevExpress.Map;
using DevExpress.Map.Native;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Map.Printing;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Printing.BrickCollection;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.DataNodes;
namespace DevExpress.Xpf.Map {
	[DXToolboxBrowsableAttribute,
	ContentProperty("Layers"),
	TemplatePart(Name = "PART_DraggingElement", Type = typeof(Rectangle)),
	TemplatePart(Name = "PART_ContainerOfRectangleToZoomIn", Type = typeof(Canvas)),
	TemplatePart(Name = "PART_RectangleToZoomIn", Type = typeof(ContentPresenter)),
	TemplatePart(Name = "PART_InvalidKeyPanel", Type = typeof(Grid)),
	TemplatePart(Name = "PART_OverlaysContainer", Type = typeof(OverlayItemsControl))]
	public class MapControl : Control, IWeakEventListener, IMapView, ISupportProjection, IPrintableControl, IInvalidKeyPanelHolder {
		protected internal static ProjectionBase DefaultMapProjection { get { return new SphericalMercatorProjection(); } }
		protected internal static readonly Size DefaultInitialMapSize = new Size(DefaultMapSize, DefaultMapSize);
		protected internal static MapCoordinateSystem DefaultCoordinateSystem { get { return new GeoMapCoordinateSystem(); } }
		const double defaultWidth = 300.0;
		const double defaultHeight = 300.0;
		internal const double DefaultMapSize = 512;
		internal const float DefaultZoomPadding = 0.15f;
		internal const double DefaultMinZoomLevel = 1.0;
		internal const double DefaultMaxZoomLevel = 20.0;
		internal const double LimitMinZoomLevel = 0.1;
		internal const double LimitMaxZoomLevel = 20.0;
		internal const bool DefaultUseSprings = true;
		#region Dependency properties
		internal static readonly DependencyPropertyKey LayersPropertyKey = DependencyPropertyManager.RegisterReadOnly("Layers",
			typeof(LayerCollection), typeof(MapControl), new PropertyMetadata());
		public static readonly DependencyProperty LayersProperty = LayersPropertyKey.DependencyProperty;
		internal static readonly DependencyPropertyKey LegendsPropertyKey = DependencyPropertyManager.RegisterReadOnly("Legends",
		   typeof(LegendCollection), typeof(MapControl), new PropertyMetadata());
		public static readonly DependencyProperty MiniMapProperty = DependencyPropertyManager.Register("MiniMap",
			typeof(MiniMap), typeof(MapControl), new PropertyMetadata(null, MiniMapPropertyChanged));
		public static readonly DependencyProperty LegendsProperty = LegendsPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ZoomLevelProperty = DependencyPropertyManager.Register("ZoomLevel",
			typeof(double), typeof(MapControl), new FrameworkPropertyMetadata(1.0, UpdateMap, ZoomLevelPropertyCoerceValue), ZoomLevelValidation);
		public static readonly DependencyProperty MinZoomLevelProperty = DependencyPropertyManager.Register("MinZoomLevel",
			typeof(double), typeof(MapControl), new FrameworkPropertyMetadata(DefaultMinZoomLevel, MinMaxZoomLevelPropertyChanged), MinZoomLevelValidation);
		public static readonly DependencyProperty MaxZoomLevelProperty = DependencyPropertyManager.Register("MaxZoomLevel",
			typeof(double), typeof(MapControl), new FrameworkPropertyMetadata(DefaultMaxZoomLevel, MinMaxZoomLevelPropertyChanged), MaxZoomLevelValidation);
		public static readonly DependencyProperty CenterPointProperty = DependencyPropertyManager.Register("CenterPoint",
			typeof(CoordPoint), typeof(MapControl), new PropertyMetadata(new GeoPoint(0, 0), UpdateMap, CoerceCenterPoint));
		public static readonly DependencyProperty EnableScrollingProperty = DependencyPropertyManager.Register("EnableScrolling",
			typeof(bool), typeof(MapControl), new PropertyMetadata(true, EnableScrollingPropertyChanged));
		public static readonly DependencyProperty EnableZoomingProperty = DependencyPropertyManager.Register("EnableZooming",
			typeof(bool), typeof(MapControl), new PropertyMetadata(true, EnableZoomingPropertyChanged));
		public static readonly DependencyProperty ScrollButtonsOptionsProperty = DependencyPropertyManager.Register("ScrollButtonsOptions",
			typeof(ScrollButtonsOptions), typeof(MapControl), new PropertyMetadata(ScrollButtonsOptionsPropertyChanged));
		public static readonly DependencyProperty CoordinatesPanelOptionsProperty = DependencyPropertyManager.Register("CoordinatesPanelOptions",
			typeof(CoordinatesPanelOptions), typeof(MapControl), new PropertyMetadata(CoordinatesPanelOptionsPropertyChanged));
		public static readonly DependencyProperty ScalePanelOptionsProperty = DependencyPropertyManager.Register("ScalePanelOptions",
			typeof(ScalePanelOptions), typeof(MapControl), new PropertyMetadata(ScalePanelOptionsPropertyChanged));
		public static readonly DependencyProperty ZoomTrackbarOptionsProperty = DependencyPropertyManager.Register("ZoomTrackbarOptions",
			typeof(ZoomTrackbarOptions), typeof(MapControl), new PropertyMetadata(ZoomTrackbarOptionsPropertyChanged));
		static readonly DependencyPropertyKey ActualScrollButtonsOptionsPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualScrollButtonsOptions",
			typeof(ScrollButtonsOptions), typeof(MapControl), new PropertyMetadata(null, ActualOptionsPropertyChanged));
		public static readonly DependencyProperty ActualScrollButtonsOptionsProperty = ActualScrollButtonsOptionsPropertyKey.DependencyProperty;
		static readonly DependencyPropertyKey ActualCoordinatesPanelOptionsPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualCoordinatesPanelOptions",
			typeof(CoordinatesPanelOptions), typeof(MapControl), new PropertyMetadata(null, ActualOptionsPropertyChanged));
		public static readonly DependencyProperty ActualCoordinatesPanelOptionsProperty = ActualCoordinatesPanelOptionsPropertyKey.DependencyProperty;
		static readonly DependencyPropertyKey ActualScalePanelOptionsPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualScalePanelOptions",
			typeof(ScalePanelOptions), typeof(MapControl), new PropertyMetadata(null, ActualOptionsPropertyChanged));
		public static readonly DependencyProperty ActualScalePanelOptionsProperty = ActualScalePanelOptionsPropertyKey.DependencyProperty;
		static readonly DependencyPropertyKey ActualZoomTrackbarOptionsPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualZoomTrackbarOptions",
			typeof(ZoomTrackbarOptions), typeof(MapControl), new PropertyMetadata(null, ActualOptionsPropertyChanged));
		public static readonly DependencyProperty ActualZoomTrackbarOptionsProperty = ActualZoomTrackbarOptionsPropertyKey.DependencyProperty;
		public static readonly DependencyProperty SelectionModeProperty = DependencyPropertyManager.Register("SelectionMode",
			typeof(ElementSelectionMode), typeof(MapControl), new PropertyMetadata(ElementSelectionMode.Extended));
		public static readonly DependencyProperty ToolTipEnabledProperty = DependencyPropertyManager.Register("ToolTipEnabled",
			typeof(bool), typeof(MapControl), new PropertyMetadata(false));
		public static readonly DependencyProperty MouseWheelZoomingStepProperty = DependencyPropertyManager.Register("MouseWheelZoomingStep",
			typeof(double), typeof(MapControl), new PropertyMetadata(1.0), MouseWheelZoomingStepValidation);	  
		public static readonly DependencyProperty ToolTipTemplateProperty = DependencyPropertyManager.Register("ToolTipTemplate",
			typeof(DataTemplate), typeof(MapControl), new PropertyMetadata(null));
		public static readonly DependencyProperty ZoomRegionTemplateProperty = DependencyPropertyManager.Register("ZoomRegionTemplate",
			typeof(DataTemplate), typeof(MapControl));
		public static readonly DependencyProperty CacheOptionsProperty = DependencyPropertyManager.Register("CacheOptions",
			typeof(CacheOptions), typeof(MapControl), new PropertyMetadata());
		public static readonly DependencyProperty PrintOptionsProperty = DependencyPropertyManager.Register("PrintOptions",
			typeof(MapPrintOptions), typeof(MapControl), new PropertyMetadata(null));
		public static readonly DependencyProperty InitialMapSizeProperty = DependencyPropertyManager.Register("InitialMapSize",
			typeof(Size), typeof(MapControl), new PropertyMetadata(DefaultInitialMapSize, UpdateMap));
		public static readonly DependencyProperty CoordinateSystemProperty = DependencyPropertyManager.Register("CoordinateSystem",
			typeof(MapCoordinateSystem), typeof(MapControl), new PropertyMetadata(DefaultCoordinateSystem, CoordinateSystemPropertyChanged));
		public static readonly DependencyProperty UseSpringsProperty = DependencyPropertyManager.Register("UseSprings",
			typeof(bool), typeof(MapControl), new PropertyMetadata(DefaultUseSprings));
		#endregion
		static MapControl() {
			Type ownerType = typeof(MapControl);
			CommandManager.RegisterClassCommandBinding(ownerType,
				new CommandBinding(MapControlCommands.Scroll,
					(d, e) => ((MapControl)d).Scroll((Point)e.Parameter), (d, e) => ((MapControl)d).OnCanScroll(e)));
			CommandManager.RegisterClassCommandBinding(ownerType,
				new CommandBinding(MapControlCommands.Zoom,
					(d, e) => ((MapControl)d).Zoom((int)e.Parameter), (d, e) => ((MapControl)d).OnCanZoom(e)));
		}
		static void UpdateMap(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapControl map = d as MapControl;
			if (map != null) {
				map.UpdateViewport();
				map.UpdateMiniMapViewport();
				map.HideToolTip();
			}
		}
		static void MiniMapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapControl map = d as MapControl;
			if (map != null)
				map.UpdateMiniMap(e.OldValue as MiniMap, e.NewValue as MiniMap);
		}
		static object ZoomLevelPropertyCoerceValue(DependencyObject d, object value) {
			MapControl map = d as MapControl;
			double currentZoomLevel = (double)value;
			if (map != null)
				currentZoomLevel = map.ValidateZoomLevel(currentZoomLevel);
			return currentZoomLevel;
		}
		static void MinMaxZoomLevelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapControl map = d as MapControl;
			if (map != null) {
				double zoomLevel = map.ValidateZoomLevel(map.ZoomLevel);
				if (map.ZoomLevel != zoomLevel)
					map.SetZoomLevel(zoomLevel);
			}
		}
		static void EnableScrollingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapControl map = d as MapControl;
			if (map != null)
				((DelegateCommand<Point>)map.ScrollCommand).RaiseCanExecuteChanged();
		}
		static void EnableZoomingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapControl map = d as MapControl;
			if (map != null)
				((DelegateCommand<double>)map.ZoomCommand).RaiseCanExecuteChanged();
		}
		static bool ZoomLevelValidation(object value) {
			return MinZoomLevelValidation(value) && MaxZoomLevelValidation(value);
		}
		static bool MinZoomLevelValidation(object value) {
			double minZoomLevel = (double)value;
			return minZoomLevel >= LimitMinZoomLevel && minZoomLevel <= LimitMaxZoomLevel;
		}
		static bool MaxZoomLevelValidation(object value) {
			double maxZoomLevel = (double)value;
			return maxZoomLevel >= LimitMinZoomLevel && maxZoomLevel <= LimitMaxZoomLevel;
		}
		static bool MouseWheelZoomingStepValidation(object value) {
			return (double)value > 0;
		}
		static void ScrollButtonsOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapControl map = d as MapControl;
			if (map != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as ScrollButtonsOptions, e.NewValue as ScrollButtonsOptions, map);
				ScrollButtonsOptions options = e.NewValue as ScrollButtonsOptions;
				if (options == null)
					map.SetValue(MapControl.ActualScrollButtonsOptionsPropertyKey, new ScrollButtonsOptions());
				else
					map.SetValue(MapControl.ActualScrollButtonsOptionsPropertyKey, options);
			}
		}
		static void CoordinatesPanelOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapControl map = d as MapControl;
			if (map != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as CoordinatesPanelOptions, e.NewValue as CoordinatesPanelOptions, map);
				CoordinatesPanelOptions options = e.NewValue as CoordinatesPanelOptions;
				if (options == null)
					map.SetValue(MapControl.ActualCoordinatesPanelOptionsPropertyKey, new CoordinatesPanelOptions());
				else
					map.SetValue(MapControl.ActualCoordinatesPanelOptionsPropertyKey, options);
			}
		}
		static void ScalePanelOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapControl map = d as MapControl;
			if (map != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as ScalePanelOptions, e.NewValue as ScalePanelOptions, map);
				ScalePanelOptions options = e.NewValue as ScalePanelOptions;
				if (options == null)
					map.SetValue(MapControl.ActualScalePanelOptionsPropertyKey, new ScalePanelOptions());
				else
					map.SetValue(MapControl.ActualScalePanelOptionsPropertyKey, options);
			}
		}
		static void ZoomTrackbarOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapControl map = d as MapControl;
			if (map != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as ZoomTrackbarOptions, e.NewValue as ZoomTrackbarOptions, map);
				ZoomTrackbarOptions options = e.NewValue as ZoomTrackbarOptions;
				if (options == null)
					map.SetValue(MapControl.ActualZoomTrackbarOptionsPropertyKey, new ZoomTrackbarOptions());
				else
					map.SetValue(MapControl.ActualZoomTrackbarOptionsPropertyKey, options);
			}
		}
		static void ActualOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapControl map = d as MapControl;
			if (map != null && e.OldValue != e.NewValue)
				map.UpdateOverlayInfos();
		}
		static bool ValidateMapProjection(object value) {
			return value != null;
		}
		static void CoordinateSystemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapControl map = d as MapControl;
			if (map != null) {
				CommonUtils.SetOwnerForValues(e.OldValue, e.NewValue, map);
				map.UpdateCoordinateSystem();
			}
		}
		static object CoerceCenterPoint(DependencyObject d, object baseValue) {
			if (baseValue == null)
				return new GeoPoint(0, 0);
			return baseValue;
		}
		Panel overlaysPanel;
		ToolTipInfo toolTipInfo;
		MapViewController viewController;
		ItemsControl layersContainer;
		OverlayItemsControl overlaysContainer;
		CacheOptions defaultCacheOptions = new CacheOptions();
		Grid invalidKeyPanel = null;
		Size viewportInPixels = Size.Empty;
		Rect viewport = new Rect(0, 0, 1, 1);
		CoordinatesPanelInfo coordinatesPanelInfo;
		readonly OverlayInfoCollection externalOverlayInfos = new OverlayInfoCollection();
		readonly OverlayInfoCollection overlayInfos = new OverlayInfoCollection();
		readonly ScalePanelInfo scalePanelInfo;
		readonly ScrollButtonsInfo scrollButtonsInfo;
		readonly ZoomTrackbarInfo zoomTrackbarInfo;
		readonly NavigationController navigationController;
		Panel OverlaysPanel {
			get {
				if (overlaysPanel == null)
					overlaysPanel = CommonUtils.GetChildPanel(overlaysContainer);
				return overlaysPanel;
			}
		}
		internal MapCoordinateSystem ActualCoordinateSystem { get { return CoordinateSystem != null ? CoordinateSystem : DefaultCoordinateSystem; } }
		internal ItemsControl LayersContainer {
			get { return layersContainer; }
			set {
				if (layersContainer != value) {
					layersContainer = value;
					if (layersContainer != null)
						layersContainer.ItemsSource = Layers;
				}
			}
		}
		internal NavigationController NavigationController { get { return navigationController; } }
		internal MapViewController ViewController { get { return viewController; } }
		internal Rect Viewport { get { return viewport; } }
		internal Size ViewportInPixels { get { return viewportInPixels; } }
		internal CoordPoint ActualCenterPoint { get { return ActualCoordinateSystem.CreateNormalizedPoint(CenterPoint); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ToolTipInfo ToolTipInfo {
			get { return toolTipInfo; }
		}
		[Category(Categories.Data)]
		public LayerCollection Layers {
			get { return (LayerCollection)GetValue(LayersProperty); }
		}
		[Category(Categories.Appearance)]
		public LegendCollection Legends {
			get { return (LegendCollection)GetValue(LegendsProperty); }
		}
		[Category(Categories.Appearance)]
		public MiniMap MiniMap {
			get { return (MiniMap)GetValue(MiniMapProperty); }
			set { SetValue(MiniMapProperty, value); }
		}
		[Category(Categories.Behavior)]
		public double ZoomLevel {
			get { return (double)GetValue(ZoomLevelProperty); }
			set { SetValue(ZoomLevelProperty, value); }
		}
		[Category(Categories.Behavior)]
		public double MinZoomLevel {
			get { return (double)GetValue(MinZoomLevelProperty); }
			set { SetValue(MinZoomLevelProperty, value); }
		}
		[Category(Categories.Behavior)]
		public double MaxZoomLevel {
			get { return (double)GetValue(MaxZoomLevelProperty); }
			set { SetValue(MaxZoomLevelProperty, value); }
		}
		[Category(Categories.Behavior), TypeConverter(typeof(GeoPointConverter))]
		public CoordPoint CenterPoint {
			get { return (CoordPoint)GetValue(CenterPointProperty); }
			set { SetValue(CenterPointProperty, value); }
		}
		[Category(Categories.Behavior)]
		public bool EnableScrolling {
			get { return (bool)GetValue(EnableScrollingProperty); }
			set { SetValue(EnableScrollingProperty, value); }
		}
		[Category(Categories.Behavior)]
		public bool EnableZooming {
			get { return (bool)GetValue(EnableZoomingProperty); }
			set { SetValue(EnableZoomingProperty, value); }
		}
		[Category(Categories.Presentation)]
		public ScrollButtonsOptions ScrollButtonsOptions {
			get { return (ScrollButtonsOptions)GetValue(ScrollButtonsOptionsProperty); }
			set { SetValue(ScrollButtonsOptionsProperty, value); }
		}
		[Category(Categories.Presentation)]
		public CoordinatesPanelOptions CoordinatesPanelOptions {
			get { return (CoordinatesPanelOptions)GetValue(CoordinatesPanelOptionsProperty); }
			set { SetValue(CoordinatesPanelOptionsProperty, value); }
		}
		[Category(Categories.Presentation)]
		public ScalePanelOptions ScalePanelOptions {
			get { return (ScalePanelOptions)GetValue(ScalePanelOptionsProperty); }
			set { SetValue(ScalePanelOptionsProperty, value); }
		}
		[Category(Categories.Presentation)]
		public ZoomTrackbarOptions ZoomTrackbarOptions {
			get { return (ZoomTrackbarOptions)GetValue(ZoomTrackbarOptionsProperty); }
			set { SetValue(ZoomTrackbarOptionsProperty, value); }
		}
		[NonCategorized]
		public ScrollButtonsOptions ActualScrollButtonsOptions {
			get { return (ScrollButtonsOptions)GetValue(ActualScrollButtonsOptionsProperty); }
		}
		[NonCategorized]
		public CoordinatesPanelOptions ActualCoordinatesPanelOptions {
			get { return (CoordinatesPanelOptions)GetValue(ActualCoordinatesPanelOptionsProperty); }
		}
		[NonCategorized]
		public ScalePanelOptions ActualScalePanelOptions {
			get { return (ScalePanelOptions)GetValue(ActualScalePanelOptionsProperty); }
		}
		[NonCategorized]
		public ZoomTrackbarOptions ActualZoomTrackbarOptions {
			get { return (ZoomTrackbarOptions)GetValue(ActualZoomTrackbarOptionsProperty); }
		}
		[NonCategorized]
		public ElementSelectionMode SelectionMode {
			get { return (ElementSelectionMode)GetValue(SelectionModeProperty); }
			set { SetValue(SelectionModeProperty, value); }
		}
		[NonCategorized]
		public bool ToolTipEnabled {
			get { return (bool)GetValue(ToolTipEnabledProperty); }
			set { SetValue(ToolTipEnabledProperty, value); }
		}
		[NonCategorized]
		public DataTemplate ToolTipTemplate {
			get { return (DataTemplate)GetValue(ToolTipTemplateProperty); }
			set { SetValue(ToolTipTemplateProperty, value); }
		}
		[Category(Categories.Action)]
		public ICommand ScrollCommand {
			get;
			private set;
		}
		[Category(Categories.Action)]
		public ICommand ZoomCommand {
			get;
			private set;
		}
		[Category(Categories.Presentation)]
		public DataTemplate ZoomRegionTemplate {
			get { return (DataTemplate)GetValue(ZoomRegionTemplateProperty); }
			set { SetValue(ZoomRegionTemplateProperty, value); }
		}
		[Category(Categories.Behavior)]
		public CacheOptions CacheOptions {
			get { return (CacheOptions)GetValue(CacheOptionsProperty); }
			set { SetValue(CacheOptionsProperty, value); }
		}
		[Category(Categories.Behavior)]
		public MapPrintOptions PrintOptions {
			get { return (MapPrintOptions)GetValue(PrintOptionsProperty); }
			set { SetValue(PrintOptionsProperty, value); }
		}
		[Category(Categories.Behavior)]
		public Size InitialMapSize {
			get { return (Size)GetValue(InitialMapSizeProperty); }
			set { SetValue(InitialMapSizeProperty, value); }
		}
		[Category(Categories.Behavior)]
		public MapCoordinateSystem CoordinateSystem {
			get { return (MapCoordinateSystem)GetValue(CoordinateSystemProperty); }
			set { SetValue(CoordinateSystemProperty, value); }
		}
		[Category(Categories.Behavior)]
		public double MouseWheelZoomingStep {
			get { return (double)GetValue(MouseWheelZoomingStepProperty); }
			set { SetValue(MouseWheelZoomingStepProperty, value); }
		}
		[Category(Categories.Behavior)]
		public bool UseSprings {
			get { return (bool)GetValue(UseSpringsProperty); }
			set { SetValue(UseSpringsProperty, value); }
		}
		#region Hidden properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CacheOptions ActualCacheOptions {
			get { return CacheOptions != null ? CacheOptions : defaultCacheOptions; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new HorizontalAlignment HorizontalContentAlignment { get { return base.HorizontalContentAlignment; } set { base.HorizontalContentAlignment = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new VerticalAlignment VerticalContentAlignment { get { return base.VerticalContentAlignment; } set { base.VerticalContentAlignment = value; } }
		#endregion
		public event MapItemSelectionChangedEventHandler SelectionChanged;
		public event LegendItemCreatingEventHandler LegendItemCreating;
		public MapControl() {
			DefaultStyleKey = typeof(MapControl);
			scalePanelInfo = new ScalePanelInfo(this);
			scrollButtonsInfo = new ScrollButtonsInfo(this);
			zoomTrackbarInfo = new ZoomTrackbarInfo(this);
			coordinatesPanelInfo = new CoordinatesPanelInfo(this);
			this.SetValue(LayersPropertyKey, new LayerCollection(this));
			this.SetValue(LegendsPropertyKey, new LegendCollection(this));
			this.SetValue(ActualScrollButtonsOptionsPropertyKey, new ScrollButtonsOptions());
			this.SetValue(ActualCoordinatesPanelOptionsPropertyKey, new CoordinatesPanelOptions());
			this.SetValue(ActualScalePanelOptionsPropertyKey, new ScalePanelOptions());
			this.SetValue(ActualZoomTrackbarOptionsPropertyKey, new ZoomTrackbarOptions());
			this.SetValue(CoordinateSystemProperty, DefaultCoordinateSystem);
			Layers.CollectionChanged += LayersCollectionChanged;
			navigationController = new NavigationController(this);
			viewController = new MapViewController(this);
			toolTipInfo = new ToolTipInfo();
			SubscribeNavigationEvents();
			ScrollCommand = DelegateCommandFactory.Create<Point>(parameter => Scroll(parameter), parameter => CanScroll(parameter), false);
			ZoomCommand = DelegateCommandFactory.Create<double>(parameter => Zoom(parameter), parameter => CanZoom(parameter), false);
			scrollButtonsInfo.Command = ScrollCommand;
			zoomTrackbarInfo.Command = ZoomCommand;
		}
		#region IWeakEventListener implementation
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			return PerformWeakEvent(managerType, sender, e);
		}
		#endregion
		#region IMapView implementation
		double IMapView.ZoomLevel { get { return ZoomLevel; } }
		CoordPoint IMapView.CenterPoint { get { return ActualCenterPoint; } }
		Size IMapView.InitialMapSize { get { return InitialMapSize; } }
		MapCoordinateSystem IMapView.CoordinateSystem { get { return ActualCoordinateSystem; } }
		Rect IMapView.Viewport { get { return Viewport; } }
		Size IMapView.ViewportInPixels { get { return ViewportInPixels; } }
		#endregion
		#region IPrintableControl implementation
		bool IPrintableControl.CanCreateRootNodeAsync {
			get { return false; }
		}
		IRootDataNode IPrintableControl.CreateRootNode(Size usablePageSize, Size reportHeaderSize, Size reportFooterSize, Size pageHeaderSize, Size pageFooterSize) {
			return new MapRootDataNode(this, usablePageSize);
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
		#region ISupportProjection implementation 
		ProjectionBase ISupportProjection.Projection {
			get {
				ISupportProjection supportProjection = ActualCoordinateSystem as ISupportProjection;
				if (supportProjection != null)
					return supportProjection.Projection;
				return null;
			}
		}
		#endregion
		#region IInvalidKeyPanelHolder implementation
		Grid IInvalidKeyPanelHolder.InvalidKeyPanel {
			get { return this.invalidKeyPanel; }
		}
		#endregion
		void SubscribeNavigationEvents() {
			MouseLeftButtonUp += new MouseButtonEventHandler(navigationController.MouseLeftButtonUp);
			MouseLeftButtonDown += new MouseButtonEventHandler(navigationController.MouseLeftButtonDown);
			MouseMove += new MouseEventHandler(navigationController.MouseMove);
			MouseMove += new MouseEventHandler(MapControl_MouseMove);
			MouseWheel += new MouseWheelEventHandler(navigationController.MouseWheel);
			MouseDown += new MouseButtonEventHandler(navigationController.MouseDown);
			ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(navigationController.ManipulationStarted);
			ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(navigationController.ManipulationCompleted);
			ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(navigationController.ManipulationDelta);
		}
		void OnCanScroll(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanScroll((Point)e.Parameter);
		}
		void OnCanZoom(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanZoom((int)e.Parameter);
		}
		void LayersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (Legends != null && Legends.Count > 0) {
				if (e.Action == NotifyCollectionChangedAction.Reset)
					Legends.DetachLayers(null);
				if (e.Action == NotifyCollectionChangedAction.Remove)
					Legends.DetachLayers(e.OldItems);
			}
		}
		void UpdateMiniMapViewport() {
			if (MiniMap != null)
				MiniMap.UpdateViewport();
		}
		void UpdateCoordinateSystem() {
			UpdateViewport();
			UpdateBoundingRect();
			UpdateMiniMapViewport();
			UpdateCoordiantesPanelInfo();
			UpdateOverlayInfos();
		}
		void UpdateMiniMap(MiniMap oldMiniMap, MiniMap newMiniMap) {
			if (oldMiniMap != null) {
				UnregisterOverlayInfo(oldMiniMap);
				((IOwnedElement)oldMiniMap).Owner = null;
			}
			if (newMiniMap != null) {
				RegisterOverlayInfo(newMiniMap);
				((IOwnedElement)newMiniMap).Owner = this;
			}
		}
		bool PerformWeakEvent(Type managerType, object sender, EventArgs e) {
			bool success = false;
			if (managerType == typeof(PropertyChangedWeakEventManager)) {
				if ((sender is MapDependencyObject)) {
					OnOptionsChanged();
					success = true;
				}
			}
			return success;
		}
		void MapControl_MouseMove(object sender, MouseEventArgs e) {
			Point mousePoint = e.GetPosition(this);
			CoordPoint point = ActualCoordinateSystem.ScreenPointToCoordPoint(mousePoint, Viewport, viewportInPixels);
			UpdateCoordinatesPanel(point);
		}
		void OnOptionsChanged() {
			InvalidateOverlays();
		}
		void UpdateCoordinatesPanel(CoordPoint point) {
			coordinatesPanelInfo.CoordPoint = point; 
		}
		void ColorizeItems() {
			foreach(LayerBase layer in Layers) {
				VectorLayerBase vectorLayer = layer as VectorLayerBase;
				if(vectorLayer != null)
					vectorLayer.ColorizeItems();
			}
		}
		List<object> CollectAllSelectedItems() {
			List<object> selectedItems = new List<object>();
			foreach (LayerBase layer in Layers) {
				VectorLayerBase itemsOwner = layer as VectorLayerBase;
				if (itemsOwner != null) {
					IList items = itemsOwner.SelectedItems;
					if (items != null)
						foreach (object item in items)
							selectedItems.Add(item);
				}
			}
			return selectedItems;
		}
		void UpdateOverlayInfos() {
			overlayInfos.Clear();
			scrollButtonsInfo.Options = ActualScrollButtonsOptions;
			coordinatesPanelInfo.Options = ActualCoordinatesPanelOptions;
			scalePanelInfo.Options = ActualScalePanelOptions;
			zoomTrackbarInfo.Options = ActualZoomTrackbarOptions;
			foreach (IOverlayInfo item in externalOverlayInfos) {
				overlayInfos.Add(item);
			}
			overlayInfos.Add(scrollButtonsInfo);
			overlayInfos.Add(zoomTrackbarInfo);
			if (coordinatesPanelInfo != null)
				overlayInfos.Add(coordinatesPanelInfo);
			overlayInfos.Add(scalePanelInfo);
			foreach (MapLegendBase legend in Legends) {
				overlayInfos.Add(legend);
			}
		}
		void ResetColorizerColor() {
			foreach (LayerBase layer in Layers) {
				VectorLayerBase mapVectorLayer = layer as VectorLayerBase;
				if (mapVectorLayer != null)
					mapVectorLayer.ResetColors();
			}
		}
		void UpdateViewport() {
			if (!viewportInPixels.IsEmpty) {
				viewport = ActualCoordinateSystem.CalculateViewport(ZoomLevel, ActualCenterPoint, ViewportInPixels, InitialMapSize);
				foreach (LayerBase layer in Layers)
					layer.Viewport = viewport;
			}
		}
		Size GetMapSizeInPixels(double zoomLevel) {
			MapSizeCore size = MathUtils.CalcMapSizeInPixels(zoomLevel, new MapSizeCore(InitialMapSize.Width, InitialMapSize.Height));
			return new Size(size.Width, size.Height);
		}
		internal double ValidateZoomLevel(double newZoomLevel) {
			return MathUtils.MinMax(newZoomLevel, MinZoomLevel, MaxZoomLevel);
		}
		internal double ValidateMinMaxZoomLevel(double value) {
			return MathUtils.MinMax(value, LimitMinZoomLevel, LimitMaxZoomLevel);
		}
		internal void RegisterOverlayInfo(MiniMap miniMap) {
			if (!externalOverlayInfos.Contains(miniMap)) {
				externalOverlayInfos.Add(miniMap);
				UpdateOverlayInfos();
			}
		}
		internal void UnregisterOverlayInfo(MiniMap miniMap) {
			if(externalOverlayInfos.Contains(miniMap)) {
				externalOverlayInfos.Remove(miniMap);
				UpdateOverlayInfos();
			}
		}
		internal void UpdateBoundingRect() {
			ActualCoordinateSystem.ResetBoundingBox();
			foreach (LayerBase layer in Layers) {
				layer.CheckCompatibility();
				if (layer.Visibility == Visibility.Visible) {
					CoordBounds bounds = layer.GetBoundingRect();
					ActualCoordinateSystem.UpdateBoundingBox(bounds);
				}
			}
			ActualCoordinateSystem.CorrectBoundingBox();
			foreach (LayerBase layer in Layers)
				layer.UpdateItemsLayout();
		}
		internal void UpdateLegends() {
			foreach (MapLegendBase legend in Legends) {
				if (legend != null)
					legend.UpdateItems();
			}
		}	  
		internal void LegendsCollectionChanged() {
			UpdateOverlayInfos();
		}
		internal void RaiseSelectionChanged() {
			if (SelectionChanged != null && SelectionMode != ElementSelectionMode.None) {
				List<object> selectedItems = CollectAllSelectedItems();
				SelectionChanged(this, new MapItemSelectionChangedEventArgs(selectedItems));
			}
		}
		internal void UpdateScalePanel() {
			if (Layers.Count > 0) {
				double maxScaleBarWidth = MapScalePanel.MaxScaleBarWidth;
				MapUnit mapUnitCenterPoint = ActualCoordinateSystem.CoordPointToMapUnit(ActualCenterPoint, true);
				Point screenCenterPoint = ActualCoordinateSystem.MapUnitToScreenPoint(mapUnitCenterPoint, Viewport, ViewportInPixels);
				double hemisphereMultiplier = CenterPoint.GetX() >= 0 ? 1.0 : -1.0;
				CoordPoint scaleCoordPoint = ActualCoordinateSystem.ScreenPointToCoordPoint(new Point(screenCenterPoint.X - hemisphereMultiplier * maxScaleBarWidth, screenCenterPoint.Y), Viewport, viewportInPixels);
				scalePanelInfo.KilometersScale = ActualCoordinateSystem.CalculateKilometersScale(ActualCenterPoint, Math.Abs(scaleCoordPoint.GetX() - ActualCenterPoint.GetX()));
			}
		}
		internal void Move(Point offset) {
			MapUnit centerInMapUnit = ActualCoordinateSystem.CoordPointToMapUnit(ActualCenterPoint, true);
			Point center = ActualCoordinateSystem.MapUnitToScreenPoint(centerInMapUnit, Viewport, viewportInPixels);
			center.Offset(offset.X, offset.Y);
			CoordPoint newCenter = ActualCoordinateSystem.ScreenPointToCoordPoint(center, Viewport, viewportInPixels);
			SetCenterPoint(newCenter);
		}
		internal void MoveBeforeZooming(Point anchorPoint, double newZoomLevel) {
			Size mapSize = GetMapSizeInPixels(ZoomLevel);
			Size newMapSize = GetMapSizeInPixels(newZoomLevel);
			double factorWidth = newMapSize.Width / mapSize.Width;
			double factorHeight = newMapSize.Height / mapSize.Height;
			MapUnit center = ActualCoordinateSystem.CoordPointToMapUnit(ActualCenterPoint);
			MapUnit anchor = ActualCoordinateSystem.ScreenPointToMapUnit(anchorPoint, Viewport, ViewportInPixels);
			MapUnit newCenter = MapUnit.Normalize(new MapUnit(anchor.X + (center.X - anchor.X) / factorWidth, anchor.Y + (center.Y - anchor.Y) / factorHeight));
			CoordPoint newCenterPoint = ActualCoordinateSystem.MapUnitToCoordPoint(newCenter);
			SetCenterPoint(newCenterPoint);
		}
		internal void LeftMouseClick(Point point) {
			ViewController.UpdateToolTipAndSelection(point, Keyboard.Modifiers);
			foreach (LayerBase layer in Layers)
				layer.LeftMouseClick(point);
		}
		internal void OnColorizerChanged() {
			ResetColorizerColor();
			ColorizeItems();
			UpdateLegends();
		}
		internal void SetZoomLevel(double zoomLevel, Point anchorPoint) {
			double newZoomLevel = ValidateZoomLevel(zoomLevel);
			MoveBeforeZooming(anchorPoint, newZoomLevel);
			SetZoomLevel(newZoomLevel);
		}
		internal void SetZoomLevel(double zoomLevel) {
			double resultZoomLevel = zoomLevel;
			if (zoomLevel < MinZoomLevel)
				resultZoomLevel = MinZoomLevel;
			if (zoomLevel > MaxZoomLevel)
				resultZoomLevel = MaxZoomLevel;
			if (resultZoomLevel != ZoomLevel)
				HideToolTip();
			this.SetCurrentValue(ZoomLevelProperty, resultZoomLevel);
		}
		internal void SetCenterPoint(CoordPoint centerPoint) {
			if (centerPoint != CenterPoint)
				HideToolTip();
			this.SetCurrentValue(CenterPointProperty, centerPoint);
		}
		internal void UpdateLayerLegendsVisibility(LayerBase layer) {
			if(layer != null){
				foreach(MapLegendBase legendBase in Legends) {
					ItemsLayerLegend legend = legendBase as ItemsLayerLegend;
					if(legend != null && legend.Layer == layer)
						legend.Visibility = layer.Visibility;
				}
			}
		}
		internal void RaiseLegendItemCreating(LegendItemCreatingEventArgs e) {
			if(LegendItemCreating != null) LegendItemCreating(this, e);
		}
		internal void RestoreVisualTreeOnPrint() {
			if (LayersContainer != null)
				LayersContainer.ItemsSource = Layers;
			if (overlaysContainer != null)
				overlaysContainer.ItemsSource = overlayInfos;
		}
		internal void ClearVisualTreeOnPrint() {
			if (LayersContainer != null)
				LayersContainer.ItemsSource = null;
			if (overlaysContainer != null)
				overlaysContainer.ItemsSource = null;
		}
		internal void InvalidateOverlays() {
			if (OverlaysPanel != null)
				OverlaysPanel.InvalidateMeasure();
		}
		internal void HideToolTip() {
			ViewController.HideToolTip();
		}
		internal void UpdateToolTipPosition() {
			ViewController.UpdateToolTipPosition();
		}
		internal void SetInitialMapSize(Size size) {
			this.SetCurrentValue(InitialMapSizeProperty, size);
		}
		protected override Size MeasureOverride(Size constraint) {
			double constraintWidth = double.IsInfinity(constraint.Width) ? defaultWidth : constraint.Width;
			double constraintHeight = double.IsInfinity(constraint.Height) ? defaultHeight : constraint.Height;
			viewportInPixels = new Size(constraintWidth, constraintHeight);
			Size res = base.MeasureOverride(constraint);
			UpdateViewport();
			return res;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			LayersContainer = GetTemplateChild("PART_LayersContainer") as ItemsControl;
			overlaysContainer = GetTemplateChild("PART_OverlaysContainer") as OverlayItemsControl;
			invalidKeyPanel = GetTemplateChild("PART_InvalidKeyPanel") as Grid;
			overlaysContainer.ItemsSource = overlayInfos;
			NavigationController.ZoomRegionPresenter = GetTemplateChild("PART_RectangleToZoomIn") as ContentPresenter;
			UpdateLegends();
			UpdateCoordiantesPanelInfo();
			UpdateOverlayInfos();
		}
		void UpdateCoordiantesPanelInfo() {
			switch (ActualCoordinateSystem.PointType) {
				case CoordPointType.Geo:
					coordinatesPanelInfo = new GeoCoordinatesPanelInfo(this);
					break;
				case CoordPointType.Cartesian:
					coordinatesPanelInfo = new CartesianCoordinatesPanelInfo(this, ((CartesianMapCoordinateSystem)ActualCoordinateSystem).MeasureUnit.Abbreviation);
					break;
				case CoordPointType.Unknown:
				default:
					coordinatesPanelInfo = null;
					break;
			}
		}
		public void Scroll(Point offset) {
			Move(offset);
		}
		public bool CanScroll(Point offset) {
			return EnableScrolling;
		}
		public void Zoom(double zoomLevel) {
			SetZoomLevel(zoomLevel, new Point(ActualWidth / 2.0, ActualHeight / 2.0));
		}
		public bool CanZoom(double zoomLevel) {
			return EnableZooming && (zoomLevel >= MinZoomLevel) && (zoomLevel <= MaxZoomLevel);
		}
		public MapHitInfo CalcHitInfo(Point point) {
			return new MapHitInfo(this, point);
		}
		public void SelectItemsByRegion(CoordPoint p1, CoordPoint p2) {
			if(Layers.Count == 0)
				return;
			MapUnit mapUnitX1 = ActualCoordinateSystem.CoordPointToMapUnit(p1, false);
			Point pX1 = ActualCoordinateSystem.MapUnitToScreenPoint(mapUnitX1, Viewport, ViewportInPixels);
			MapUnit mapUnitX2 = ActualCoordinateSystem.CoordPointToMapUnit(p2, false);
			Point pX2 = ActualCoordinateSystem.MapUnitToScreenPoint(mapUnitX2, Viewport, ViewportInPixels);
			SelectItemsByRegion(new Rect(pX1, pX2));
		}
		public void SelectItemsByRegion(Rect screenRegion){
			if (!screenRegion.IsEmpty)
				ViewController.SelectItemsByRegion(screenRegion);
		}
		#region ZoomToRegion, ZoomToFit methods
		protected internal Rect CalculateViewportRegion(CoordPoint leftTop, CoordPoint rightBottom) {
			MapUnit leftTopUnit = ActualCoordinateSystem.CoordPointToMapUnit(leftTop);
			MapUnit rightBottomUnit = ActualCoordinateSystem.CoordPointToMapUnit(rightBottom);
			Point leftTopPt = ActualCoordinateSystem.MapUnitToScreenPoint(leftTopUnit, Viewport, ViewportInPixels);
			Point rightBottomPt = ActualCoordinateSystem.MapUnitToScreenPoint(rightBottomUnit, Viewport, ViewportInPixels);
			double x = Math.Min(leftTopPt.X, rightBottomPt.X);
			double y = Math.Min(leftTopPt.Y, rightBottomPt.Y);
			return new Rect(x, y, Math.Abs(rightBottomPt.X - leftTopPt.X), Math.Abs(rightBottomPt.Y - leftTopPt.Y));
		}
		protected internal double CalculateRegionZoom(CoordPoint leftTop, CoordPoint rightBottom, double zoomLevel, double padding) {
			Rect viewPortRect = CalculateViewportRegion(leftTop, rightBottom);
			double zoomCoeff = 1.0f;
			Size viewportInPixels = ViewportInPixels;
			bool adjustByHeight = (viewPortRect.Height / viewportInPixels.Height) < (viewPortRect.Width / viewportInPixels.Width);
			if (adjustByHeight)
				zoomCoeff = viewportInPixels.Width / viewPortRect.Width;
			else
				zoomCoeff = viewportInPixels.Height / viewPortRect.Height;
			Size currentSize = GetMapSizeInPixels(zoomLevel);
			Size newSize = new Size(currentSize.Width * zoomCoeff, currentSize.Height * zoomCoeff);
			double baseLen = adjustByHeight ? InitialMapSize.Height : InitialMapSize.Width;
			double newLen = adjustByHeight ? newSize.Height : newSize.Width;
			double value = newLen / baseLen;
			padding = Math.Min(1.0, Math.Max(padding, 0.0));
			value *= 1.0 - padding;
			if (value < 1.0) {
				return value;
			}
			double newZoomLevel = Math.Log(value, 2) + 1;
			return newZoomLevel;
		}
		protected internal void ZoomToRegionCore(CoordPoint leftTop, CoordPoint rightBottom, CoordPoint centerPoint, double zoomPadding) {
			double zoomLevel = CalculateRegionZoom(leftTop, rightBottom, ZoomLevel, zoomPadding);
			if (EnableZooming) {
				Zoom(zoomLevel);
				SetCenterPoint(centerPoint);
			}
		}
		protected internal CoordBounds SelectItemsBounds(IEnumerable<MapItem> items) {
			return CoordPointHelper.SelectItemsBounds(items);
		}
		protected internal CoordBounds CalculateLayersItemsBounds(IEnumerable<LayerBase> layers) {
			return CoordPointHelper.SelectLayersItemsBounds(layers);
		}
		public void ZoomToRegion(CoordPoint p1, CoordPoint p2, double paddingFactor = DefaultZoomPadding) {
			if (!EnableZooming || Layers.Count == 0)
				return;
			CoordPoint topLeftNormalized = CoordinateSystemHelper.CreateNormalizedPoint(p1);
			CoordPoint bottomRightNormalized = CoordinateSystemHelper.CreateNormalizedPoint(p2);
			MapUnit minUnit = ActualCoordinateSystem.CoordPointToMapUnit(topLeftNormalized, false);
			MapUnit maxUnit = ActualCoordinateSystem.CoordPointToMapUnit(bottomRightNormalized, false);
			MapUnit centerUnit = new MapUnit((minUnit.X + maxUnit.X) / 2.0, (minUnit.Y + maxUnit.Y) / 2.0);
			CoordPoint center = ActualCoordinateSystem.MapUnitToCoordPoint(centerUnit);
			ZoomToRegionCore(topLeftNormalized, bottomRightNormalized, center, paddingFactor);
		}
		public void ZoomToFit(IEnumerable<MapItem> items, double paddingFactor = DefaultZoomPadding) {
			if (items == null || !EnableZooming)
				return;
			CoordBounds bounds = SelectItemsBounds(items);
			ZoomToFit(bounds, paddingFactor);
		}
		public void ZoomToFit(CoordBounds bounds, double paddingFactor = DefaultZoomPadding) {
			if (bounds.IsEmpty || !EnableZooming)
				return;
			CoordPoint topLeft = ActualCoordinateSystem.PointFactory.CreatePoint(bounds.X1, bounds.Y1);
			CoordPoint bottomRight = ActualCoordinateSystem.PointFactory.CreatePoint(bounds.X2, bounds.Y2);
			ZoomToRegion(topLeft, bottomRight, paddingFactor);
		}
		public void ZoomToFitLayerItems(double paddingFactor = DefaultZoomPadding) {
			ZoomToFitLayerItems(Layers, paddingFactor);
		}
		public void ZoomToFitLayerItems(IEnumerable<LayerBase> layers, double paddingFactor = DefaultZoomPadding) {
			CoordBounds bounds = CalculateLayersItemsBounds(layers);
			ZoomToFit(bounds, paddingFactor);
		}
		#endregion
		#region Print preview
		public void ShowPrintPreview(Window owner) {
			ShowPrintPreview(owner, string.Empty, string.Empty);
		}
		public void ShowPrintPreview(Window owner, string documentName, string title) {
			PrintHelper.ShowPrintPreview(owner, this, documentName, title);
		}
		public void ShowPrintPreviewDialog(Window owner) {
			ShowPrintPreviewDialog(owner, string.Empty, string.Empty);
		}
		public void ShowPrintPreviewDialog(Window owner, string documentName, string title) {
			PrintHelper.ShowPrintPreviewDialog(owner, this, documentName, title);
		}
		#endregion
		#region Ribbon Print preview
		public void ShowRibbonPrintPreview(Window owner) {
			ShowRibbonPrintPreview(owner, string.Empty, string.Empty);
		}
		public void ShowRibbonPrintPreview(Window owner, string documentName, string title) {
			PrintHelper.ShowRibbonPrintPreview(owner, this, documentName, title);
		}
		public void ShowRibbonPrintPreviewDialog(Window owner) {
			ShowRibbonPrintPreviewDialog(owner, string.Empty, string.Empty);
		}
		public void ShowRibbonPrintPreviewDialog(Window owner, string documentName, string title) {
			PrintHelper.ShowRibbonPrintPreviewDialog(owner, this, documentName, title);
		}
		#endregion
		#region Export methods
		#region RTF
		public void ExportToRtf(string filePath) {
			PrintHelper.ExportToRtf(this, filePath, new RtfExportOptions());
		}
		public void ExportToRtf(string filePath, RtfExportOptions options) {
			PrintHelper.ExportToRtf(this, filePath, options);
		}
		public void ExportToRtf(Stream stream) {
			PrintHelper.ExportToRtf(this, stream, new RtfExportOptions());
		}
		public void ExportToRtf(Stream stream, RtfExportOptions options) {
			PrintHelper.ExportToRtf(this, stream, options);
		}
		#endregion
		#region PDF
		public void ExportToPdf(string filePath) {
			PrintHelper.ExportToPdf(this, filePath, new PdfExportOptions());
		}
		public void ExportToPdf(string filePath, PdfExportOptions options) {
			PrintHelper.ExportToPdf(this, filePath, options);
		}
		public void ExportToPdf(Stream stream) {
			PrintHelper.ExportToPdf(this, stream, new PdfExportOptions());
		}
		public void ExportToPdf(Stream stream, PdfExportOptions options) {
			PrintHelper.ExportToPdf(this, stream, options);
		}
		#endregion
		#region XLS
		public void ExportToXls(string filePath) {
			PrintHelper.ExportToXls(this, filePath, new XlsExportOptions());
		}
		public void ExportToXls(string filePath, XlsExportOptions options) {
			PrintHelper.ExportToXls(this, filePath, options);
		}
		public void ExportToXls(Stream stream) {
			PrintHelper.ExportToXls(this, stream, new XlsExportOptions());
		}
		public void ExportToXls(Stream stream, XlsExportOptions options) {
			PrintHelper.ExportToXls(this, stream, options);
		}
		#endregion
		#region XLSX
		public void ExportToXlsx(string filePath) {
			PrintHelper.ExportToXlsx(this, filePath, new XlsxExportOptions());
		}
		public void ExportToXlsx(string filePath, XlsxExportOptions options) {
			PrintHelper.ExportToXlsx(this, filePath, options);
		}
		public void ExportToXlsx(Stream stream) {
			PrintHelper.ExportToXlsx(this, stream, new XlsxExportOptions());
		}
		public void ExportToXlsx(Stream stream, XlsxExportOptions options) {
			PrintHelper.ExportToXlsx(this, stream, options);
		}
		#endregion
		#region HTML
		public void ExportToHtml(string filePath) {
			PrintHelper.ExportToHtml(this, filePath, new HtmlExportOptions());
		}
		public void ExportToHtml(string filePath, HtmlExportOptions options) {
			PrintHelper.ExportToHtml(this, filePath, options);
		}
		public void ExportToHtml(Stream stream) {
			PrintHelper.ExportToHtml(this, stream, new HtmlExportOptions());
		}
		public void ExportToHtml(Stream stream, HtmlExportOptions options) {
			PrintHelper.ExportToHtml(this, stream, options);
		}
		#endregion
		#region MHT
		public void ExportToMht(string filePath) {
			PrintHelper.ExportToMht(this, filePath, new MhtExportOptions());
		}
		public void ExportToMht(string filePath, MhtExportOptions options) {
			PrintHelper.ExportToMht(this, filePath, options);
		}
		public void ExportToMht(Stream stream) {
			PrintHelper.ExportToMht(this, stream, new MhtExportOptions());
		}
		public void ExportToMht(Stream stream, MhtExportOptions options) {
			PrintHelper.ExportToMht(this, stream, options);
		}
		#endregion
		#region XPS
		public void ExportToXps(string filePath) {
			PrintHelper.ExportToXps(this, filePath, new XpsExportOptions());
		}
		public void ExportToXps(string filePath, XpsExportOptions options) {
			PrintHelper.ExportToXps(this, filePath, options);
		}
		public void ExportToXps(Stream stream) {
			PrintHelper.ExportToXps(this, stream, new XpsExportOptions());
		}
		public void ExportToXps(Stream stream, XpsExportOptions options) {
			PrintHelper.ExportToXps(this, stream, options);
		}
		#endregion
		#region Image
		public void ExportToImage(string filePath) {
			PrintHelper.ExportToImage(this, filePath, new ImageExportOptions());
		}
		public void ExportToImage(string filePath, ImageExportOptions options) {
			PrintHelper.ExportToImage(this, filePath, options);
		}
		public void ExportToImage(Stream stream) {
			PrintHelper.ExportToImage(this, stream, new ImageExportOptions());
		}
		public void ExportToImage(Stream stream, ImageExportOptions options) {
			PrintHelper.ExportToImage(this, stream, options);
		}
		#endregion
		#endregion
	}
}
