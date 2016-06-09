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

using DevExpress.Map;
using DevExpress.Map.Native;
using DevExpress.Services;
using DevExpress.Services.Internal;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Printing;
using DevExpress.XtraMap.Services;
using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
namespace DevExpress.XtraMap.Native {
	public class InnerMap : MapDisposableObject, IOwnedElement, ISupportInitialize, IMapAnimatableItem, IPrintable,
		IServiceContainer, ISkinProvider, IUIThreadRunner, IMapView, ISupportUnitConverter {
		const int WM_CAPTURECHANGED = 0x215;
		const int MaxEventsCount = 200;
		internal const int DefaultMaxScaleLineWidth = 130;
		internal const int DefaultMapSize = 512;
		internal const float DefaultZoomPadding = 0.15f;
		internal const double DefaultZoomLevel = 1.0;
		internal const double DefaultMinZoomLevel = 1.0;
		internal const double DefaultMaxZoomLevel = 20.0;
		internal const double LimitMinZoomLevel = 0.1;
		internal const double LimitMaxZoomLevel = 20.0;
		internal const bool DefaultEnableZooming = true;
		internal const bool DefaultEnableScrolling = true;
		internal const bool DefaultShowToolTips = true;
		internal const bool DefaultEnableAnimation = true;
		internal const ElementSelectionMode DefaultSelectionMode = ElementSelectionMode.Extended;
		internal const RenderMode DefaultRenderMode = RenderMode.Auto; 
		internal const int DefaultMoveOffset = 10;
		internal static readonly Color DefaultBackColor = Color.FromArgb(255, 0x5F, 0x8B, 0x95);
		protected internal static readonly Size DefaultInitialSize = new Size(DefaultMapSize, DefaultMapSize);
		public readonly static Size DefaultMapControlSize = new Size(400, 200);
		protected internal static CoordPoint DefaultCenterPoint { get {return new GeoPoint(); } }
		protected internal static MapCoordinateSystem DefaultCoordinateSystem { get { return new GeoMapCoordinateSystem(); } }
		object owner;
		Rectangle clientRectangle;
		Rectangle contentRectangle;
		Color backColor = DefaultBackColor;
		MapItemStyle backgroundStyle;
		bool enableZooming = DefaultEnableZooming;
		bool enableScrolling = DefaultEnableScrolling;
		bool enableAnimation = DefaultEnableAnimation;
		bool showToolTips = DefaultShowToolTips;
		bool loading = false;
		string actualSkinName;
		object imageList;
		int eventsCount = 0;
		LayerCollection layers;
		LegendCollection legends;
		MapOverlayCollection overlays;
		SortedLayerCollection actualLayers;
		IMapEventHandler eventHandler;
		MapMouseHandler mouseHandler;
		IMapItemFactory mapItemFactory;
		MapOperationHelper operationHelper;
		MapHitTestController hitTestController;
		InteractionController interactionController;
		MapCoordinateSystem coordinateSystem;
		RenderController renderController;
		NavigationPanelOptions navigationPanelOptions;
		PrintOptions printOptions;
		ElementSelectionMode selectionMode = DefaultSelectionMode;
		RenderMode renderMode;
		BorderStyles borderStyle = BorderStyles.Default;
		MapSupportObjectChangedListener<LayerBase> layerListener;
		IMapControlEventsListener externalListener;
		ISynchronizeInvoke externalInvoker;
		List<IMapNotificationObserver> notificationObservers;
		MapPrinter printer;
		ServiceManager serviceManager;
		AnimationController animationController;
		CoordPoint centerPoint = DefaultCenterPoint;
		double zoomLevel = DefaultZoomLevel;
		double minZoomLevel = DefaultMinZoomLevel;
		double maxZoomLevel = DefaultMaxZoomLevel;
		MapUnitConverter unitConverter;
		Size initialMapSize = DefaultInitialSize;
		MiniMap miniMap;
		MapViewportInternal currentViewPort;
		CoordPoint anchorPoint = new GeoPoint();
		bool keyboardShifting;
#if DEBUG
		bool showDebugInfo = false;
#endif
		MapRect RenderBounds { get; set; }
		protected internal ISynchronizeInvoke ExternalInvoker { get { return externalInvoker != null ? externalInvoker : OwnedControl as ISynchronizeInvoke; } }
		protected internal List<IMapNotificationObserver> NotificationObservers { get { return notificationObservers; } }
		#region Onwed Control properties
		protected internal IMapControl OwnedControl { get { return owner is IMapControl ? (IMapControl)owner : EmptyInnerMapOwner.Instance; } }
		internal bool IsSkinActive {
			get {
				return OwnedControl.IsSkinActive && !String.IsNullOrEmpty(ActualSkinName);
			}
		}
		protected internal string ActualSkinName { get { return actualSkinName; } set { actualSkinName = value; } }
		protected internal bool IsDesignMode { get { return OwnedControl.IsDesignMode; } }
		protected internal bool IsDesigntimeProcess { get { return OwnedControl.IsDesigntimeProcess; } }
		protected internal IntPtr Handle { get { return OwnedControl.Handle; } }
		protected internal virtual bool Capture { get { return OwnedControl.Capture; } set { OwnedControl.Capture = value; } }
		protected internal Cursor Cursor { get { return OwnedControl.Cursor; } set { OwnedControl.Cursor = value; } }
		protected internal Rectangle ClientRectangle { get { return clientRectangle; } }
		protected internal Rectangle ContentRectangle { get { return contentRectangle; } }
		protected internal IMapControlEventsListener EventListener { get { return externalListener != null ? externalListener : OwnedControl as IMapControlEventsListener; } }
		#endregion
		string ISkinProvider.SkinName { get { return ActualSkinName; } }
		protected bool IsRenderControllerEnabled { get { return renderController != null; } }
		protected bool IsRenderControllerInitialized { get { return IsRenderControllerEnabled && renderController.IsInitialized; } }
		internal MapMouseHandler MouseHandler {
			get {
				if(mouseHandler == null) {
					mouseHandler = CreateMouseHandler();
					mouseHandler.Initialize();
				}
				return mouseHandler;
			}
		}
		internal RenderController RenderController { get { return renderController; } }
		internal AnimationController AnimationController {
			get {
				if(animationController == null)
					animationController = CreateAnimationController();
				return animationController;
			}
		}
		internal IMapItemFactory MapItemFactory { get { return mapItemFactory; } }
		internal IMapEventHandler EventHandler { get { return eventHandler; } }
		internal MapOperationHelper OperationHelper { get { return operationHelper; } }
		internal MapHitTestController HitTestController { get { return hitTestController; } }
		internal InteractionController InteractionController { get { return interactionController; } }
		internal SortedLayerCollection ActualLayers { get { return actualLayers; } }
		internal MapItemStyle BackgroundStyle { get { return backgroundStyle; } }
		internal CoordPoint AnchorPoint { get { return anchorPoint; } }
		internal CoordPoint ActualCenterPoint {
			get {
				CoordPoint point = OperationHelper.CanAnimate() ? AnimationController.AnimatedCenterPoint : CenterPoint;
				return coordinateSystem.CreateNormalizedPoint(point);
			}
		}
		internal double ActualZoomLevel {
			get {
				return OperationHelper.CanAnimate() ? AnimationController.AnimatedZoomLevel : ZoomLevel;
			}
		}
		internal RenderMode RenderMode {
			get { return renderMode; }
			set {
				if(renderMode == value)
					return;
				this.renderMode = value;
				OnRenderModeChanged();
			}
		}
		internal bool KeyboardShifting { get { return keyboardShifting; } }
#if DEBUG
		internal bool ShowDebugInfo {
			get { return showDebugInfo; }
			set {
				if(showDebugInfo == value)
					return;
				showDebugInfo = value;
				OnShowDebugInfoChanged();
			}
		}
#endif
		protected internal ServiceManager ServiceManager { get { return serviceManager; } }
		protected internal IMouseHandlerService MouseHandlerService { get { return GetService<IMouseHandlerService>(); } }
		protected internal MapUnitConverter UnitConverter {
			get {
				if(unitConverter == null)
					unitConverter = new MapViewUnitConverter(this, CoordinateSystem);
				return unitConverter;
			}
		}
		protected internal RenderMode ActualRenderMode {
			get {
				return GetActualRenderMode();
			}
		}
		protected virtual bool AnimationInProgress { get { return AnimationController.InProgress; } }
		protected internal MapViewportInternal CurrentViewport { get { return currentViewPort; } }
		protected internal MapRect Viewport {
			get { return CurrentViewport.AnimatedViewportRect; }
			set {
				if(CurrentViewport.AnimatedViewportRect != value)
					SetViewportRect(value); 
			}
		}
		public BorderStyles BorderStyle {
			get { return borderStyle; }
			set {
				if(borderStyle == value)
					return;
				borderStyle = value;
				OnBorderStyleChanged();
			}
		}
		public Color BackColor {
			get { return backColor; }
			set {
				if(backColor == value)
					return;
				backColor = value;
				OnBackColorChanged();
			}
		}
		[Category(SRCategoryNames.Map), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraMap.Design.LayerCollectionEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor))]
		public LayerCollection Layers {
			get { return layers; }
		}
		[Category(SRCategoryNames.Map), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraMap.Design.LayerCollectionEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor))]
		public LegendCollection Legends {
			get { return legends; }
		}
		public MapOverlayCollection Overlays {
			get { return overlays; }
		}
		[Category(SRCategoryNames.Map)]
		public CoordPoint CenterPoint {
			get { return centerPoint; }
			set { SetCenterPointInternal(value, OperationHelper.CanAnimate(), false); }
		}
		[Category(SRCategoryNames.Map), DefaultValue(DefaultZoomLevel)]
		public double ZoomLevel {
			get { return zoomLevel; }
			set {
				SetZoomLevelInternal(value, OperationHelper.CanAnimate(), false);
			}
		}
		[Category(SRCategoryNames.Map), DefaultValue(DefaultMinZoomLevel)]
		public double MinZoomLevel {
			get { return minZoomLevel; }
			set {
				double zoomLevel = ValidateMinMaxZoomLevel(value);
				zoomLevel = ValidateMinZoomLevel(zoomLevel);
				minZoomLevel = zoomLevel;
				OnMinMaxZoomLevelChanged();
			}
		}
		[Category(SRCategoryNames.Map), DefaultValue(DefaultMaxZoomLevel)]
		public double MaxZoomLevel {
			get { return maxZoomLevel; }
			set {
				double zoomLevel = ValidateMinMaxZoomLevel(value);
				zoomLevel = ValidateMaxZoomLevel(zoomLevel);
				maxZoomLevel = zoomLevel;
				OnMinMaxZoomLevelChanged();
			}
		}
		[Category(SRCategoryNames.Options), DefaultValue(DefaultEnableAnimation)]
		public bool EnableAnimation {
			get { return enableAnimation; }
			set {
				enableAnimation = value;
				this.eventsCount = 0;
			}
		}
		[Category(SRCategoryNames.Options), DefaultValue(DefaultEnableZooming)]
		public bool EnableZooming {
			get { return enableZooming; }
			set {
				if(enableZooming == value)
					return;
				enableZooming = value;
				OnEnableZoomingChanged();
			}
		}
		[Category(SRCategoryNames.Options), DefaultValue(DefaultEnableScrolling)]
		public bool EnableScrolling {
			get { return enableScrolling; }
			set {
				if(enableScrolling == value)
					return;
				enableScrolling = value;
				OnEnableScrollingChanged();
			}
		}
		[Category(SRCategoryNames.Options), DefaultValue(DefaultShowToolTips)]
		public bool ShowToolTips {
			get { return showToolTips; }
			set {
				if(showToolTips == value)
					return;
				showToolTips = value;
			}
		}
		[Category(SRCategoryNames.Map), DefaultValue(DefaultSelectionMode)]
		public ElementSelectionMode SelectionMode {
			get { return selectionMode; }
			set {
				if(selectionMode == value)
					return;
				selectionMode = value;
			}
		}
		[
		Category(SRCategoryNames.Map), DefaultValue(null),
		TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))
		]
		public object ImageList {
			get { return imageList; }
			set {
				if(imageList == value)
					return;
				UnsubscribeImageCollectionEvents(imageList);
				imageList = value;
				SubscribeImageCollectionEvents(imageList);
				OnImageListChanged();
			}
		}
		public MapCoordinateSystem CoordinateSystem {
			get { return coordinateSystem; }
			set {
				if(value == null)
					value = DefaultCoordinateSystem;
				if(Object.Equals(coordinateSystem, value))
					return;
				MapUtils.SetOwner(coordinateSystem, null);
				coordinateSystem = value;
				MapUtils.SetOwner(CoordinateSystem, this);
				OnCoordinateSystemChanged();
			}
		}
		[Category(SRCategoryNames.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NavigationPanelOptions NavigationPanelOptions { get { return navigationPanelOptions; } }
		[Category(SRCategoryNames.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PrintOptions PrintOptions { get { return printOptions; } }
		public MapPrinter Printer {
			get {
				if(printer == null)
					printer = CreatePrinter();
				return printer;
			}
		}
		[
		Category(SRCategoryNames.Behavior)]
		public Size InitialMapSize {
			get { return initialMapSize; }
			set {
				if(initialMapSize == value)
					return;
				initialMapSize = value;
				EnsureBoundingRect(Layers);
				UpdateViewportRect(false);
				Render();
			}
		}
		public MiniMap MiniMap {
			get { return miniMap; }
			set {
				if(Object.Equals(miniMap, value))
					return;
				MapUtils.SetOwner(miniMap, null);
				this.miniMap = value;
				if(miniMap != null)
					miniMap.Bounds = RenderController.CalculateMiniMapBounds(miniMap);
				MapUtils.SetOwner(miniMap, this);
				OnMiniMapChanged();
			}
		}
		#region Events
		public event EventHandler RenderingInitialized;
		internal void RaiseRenderingInitialized() {
			if(RenderingInitialized != null)
				RenderingInitialized(this, EventArgs.Empty);
		}
		internal bool RaiseMapItemClick(MouseEventArgs e, MapItem item) {
			MapItemClickEventArgs args = new MapItemClickEventArgs(item, e);
			if(EventListener != null)
				EventListener.NotifyMapItemClick(args);
			return args.Handled;
		}
		internal bool RaiseMapItemDoubleClick(MouseEventArgs e, MapItem item) {
			MapItemClickEventArgs args = new MapItemClickEventArgs(item, e);
			if(EventListener != null)
				EventListener.NotifyMapItemDoubleClick(args);
			return args.Handled;
		}
		protected virtual HyperlinkClickEventArgs GetHyperlinkClickEventArgs(MouseEventArgs e, MapPointer pointer) {
			return pointer.TryClickHyperlink(e);
		}
		internal bool RaiseHyperlinkClick(MouseEventArgs e, MapPointer pointer) {
			HyperlinkClickEventArgs hyperArgs = GetHyperlinkClickEventArgs(e, pointer);
			if(EventListener != null && hyperArgs != null) {
				EventListener.NotifyHyperlinkClick(hyperArgs);
				return true;
			}
			return false;
		}
		internal void RaiseLegendItemCreating(LegendItemCreatingEventArgs e) {
			if(EventListener != null)
				EventListener.NotifyLegendItemCreating(e);
		}
		public event MapItemHighlightingEventHandler MapItemHighlighting;
		internal void RaiseMapItemHighlighting(MapItemHighlightingEventArgs args) {
			if(MapItemHighlighting != null)
				MapItemHighlighting(this, args);
		}
		internal void RaiseOverlaysArranged(OverlaysArrangedEventArgs args) {
			if(EventListener != null)
				EventListener.NotifyOverlaysArranged(args);
		}
		#endregion
		public InnerMap()
			: this(true) {
		}
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				if(owner == value)
					return;
				owner = value;
				OnOwnerChanged();
			}
		}
		protected virtual void OnOwnerChanged() {
			SetClientRectangle(OwnedControl.ClientRectangle);
		}
		#endregion
		#region IPrintable implementation
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			if(printer != null)
				printer = CreatePrinter();
			printer.Initialize(ps, link);
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			this.printer.Finalize(ps, link);
			this.printer.Dispose();
			this.printer = null;
		}
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
			printer.CreateArea(areaName, graph);
		}
		bool IPrintable.CreatesIntersectedBricks {
			get { return printer.CreatesIntersectedBricks; }
		}
		void IPrintable.AcceptChanges() {
			printer.AcceptChanges();
		}
		void IPrintable.RejectChanges() {
			printer.RejectChanges();
		}
		void IPrintable.ShowHelp() {
			printer.ShowHelp();
		}
		bool IPrintable.SupportsHelp() {
			return printer.SupportsHelp();
		}
		bool IPrintable.HasPropertyEditor() {
			return printer.HasPropertyEditor();
		}
		UserControl IPrintable.PropertyEditorControl { get { return printer.PropertyEditorControl; } }
		#endregion
		#region ISupportInitialize implementation
		void ISupportInitialize.BeginInit() {
			loading = true;
		}
		void ISupportInitialize.EndInit() {
			loading = false;
			OnImageListChanged();
			AnimationController.SynchronizeCenterAndZoom();
		}
		#endregion
		#region IMapAnimatableItem Members
		bool IMapAnimatableItem.EnableAnimation { get { return EnableAnimation; } }
		void IMapAnimatableItem.FrameChanged(object sender, AnimationAction action, double progress) {
			OnFrameChanged(sender, action);
		}
		#endregion
		#region IUIThreadRunner Members
		bool IUIThreadRunner.AllowInvoke { get { return OwnedControl is ISynchronizeInvoke && !loading && !IsDesignMode; } }
		void IUIThreadRunner.BeginInvoke(Action action) {
			TryUIInvoke(action, true);
		}
		void IUIThreadRunner.Invoke(Action action) {
			TryUIInvoke(action, false);
		}
		#endregion
		#region IServiceContainer Members
		void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			ServiceManager.AddService(serviceType, callback, promote);
		}
		void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback) {
			ServiceManager.AddService(serviceType, callback);
		}
		void IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote) {
			ServiceManager.AddService(serviceType, serviceInstance, promote);
		}
		void IServiceContainer.AddService(Type serviceType, object serviceInstance) {
			ServiceManager.AddService(serviceType, serviceInstance);
		}
		void IServiceContainer.RemoveService(Type serviceType, bool promote) {
			ServiceManager.RemoveService(serviceType, promote);
		}
		void IServiceContainer.RemoveService(Type serviceType) {
			ServiceManager.RemoveService(serviceType);
		}
		#endregion
		#region IServiceProvider Members
		object IServiceProvider.GetService(Type serviceType) {
			return ServiceManager.GetService(serviceType);
		}
		#endregion
		#region IMapView implementation
		double IMapView.ZoomLevel { get { return ActualZoomLevel; } }
		CoordPoint IMapView.CenterPoint { get { return ActualCenterPoint; } }
		MapViewportInternal IMapView.Viewport { get { return CurrentViewport; } }
		bool IMapView.ReadyForRender { get { return !IsDesignMode; } }
		Size IMapView.InitialMapSize { get { return InitialMapSize; } }
		bool IMapView.AnimationInProgress { get { return AnimationController.InProgress; } }
		MapRect IMapView.RenderBounds { get { return RenderBounds; } }
		CoordPoint IMapView.AnchorPoint { get { return AnchorPoint; } }		
		IMapItemStyleProvider IMapView.StyleProvider { get { return RenderController.DefaultItemStyleProvider; } }
		IMapEventHandler IMapView.EventHandler { get { return EventHandler; } }
		#endregion
		#region ISupportUnitConverter implementation
		MapUnitConverter ISupportUnitConverter.UnitConverter { get { return UnitConverter; } }
		#endregion
		internal MapSize GetMapSizeInPixels(double zoomLevel) {
			MapSizeCore size = MathUtils.CalcMapSizeInPixels(zoomLevel, new MapSizeCore(InitialMapSize.Width, InitialMapSize.Height));
			return new MapSize(size.Width, size.Height);
		}
		void Initialize() {
			this.coordinateSystem = new GeoMapCoordinateSystem(this);
			this.clientRectangle = new Rectangle(Point.Empty, DefaultMapControlSize);
			this.contentRectangle = this.clientRectangle;
			this.contentRectangle.Inflate(-2, -2);
			this.actualLayers = new SortedLayerCollection(this);
			this.layers = new LayerCollection(this);
			this.legends = new LegendCollection(this);
			InitializeOverlayCollection();
			this.currentViewPort = new MapViewportInternal();
			this.operationHelper = new MapOperationHelper(this);
			this.backgroundStyle = new MapItemStyle() { Stroke = backColor };
			SetEventHandlerInternal(null);
			this.hitTestController = new MapHitTestController(this);
			this.renderController = CreateRenderController();
			interactionController = new InteractionController(this);
			this.mapItemFactory = new DefaultMapItemFactory();
			this.navigationPanelOptions = new NavigationPanelOptions();
			this.printOptions = new PrintOptions();
			this.layerListener = new MapSupportObjectChangedListener<LayerBase>(ActualLayers);
			this.notificationObservers = new List<IMapNotificationObserver>();
			SubscribeEvents();
			this.serviceManager = new ServiceManager();
			InitializeServices();
		}
		void InitializeOverlayCollection() {
			this.overlays = new MapOverlayCollection(this);
			((IChangedCallbackOwner)overlays).SetParentCallback(InvalidateViewInfo);
		}
		RenderMode GetActualRenderMode() {
			if(RenderMode == RenderMode.DirectX || RenderMode == RenderMode.Auto)
				return OperationHelper.CanUseDirectX() ? RenderMode.DirectX : RenderMode.GdiPlus;
			return XtraMap.RenderMode.GdiPlus;
		}
		void PrepareAllLayersData() {
			foreach(LayerBase layer in Layers) {
				if(layer.CheckVisibility())
					PrepareLayerData(layer as VectorItemsLayer);
			}
		}
		void PrepareLayerData(VectorItemsLayer layer) {
			IRenderItemProvider provider = layer as IRenderItemProvider;
			if(provider != null) {
				lock(layer.UpdateLocker) {
					provider.PrepareData();
				}
			}
		}
		void TryUIInvoke(Action action, bool beginInvoke) {
			if(OwnedControl == null) {
				action();
				return;
			}
			if(!OwnedControl.IsDisposed && OwnedControl.IsHandleCreated)
				UIInvoke(action, beginInvoke);
		}
		void UIInvoke(Action action, bool beginInvoke) {
			ISynchronizeInvoke invoker = OwnedControl as ISynchronizeInvoke;
			if(beginInvoke)
				MapUtils.BeginInvokeAction(invoker, action);
			else
				MapUtils.InvokeAction(invoker, action);
		}
		double CalculateNextZoomOutLevel(int largeChange, double smallChange) {
			double roundedZoomLevel = Math.Round(ZoomLevel + largeChange);
			return roundedZoomLevel >= 1.0 ? ZoomLevel + largeChange : ZoomLevel + smallChange;
		}
		double CalculateNextZoomInLevel(int largeChange, double smallChange) {
			return ZoomLevel < 1.0 ? ZoomLevel + smallChange : ZoomLevel + largeChange;
		}
		void UnsubscribeImageCollectionEvents(object imageCollection) {
			ImageCollection oldImageCollection = imageCollection as ImageCollection;
			if(oldImageCollection != null)
				oldImageCollection.Changed -= ImageListChanged;
		}
		void SubscribeImageCollectionEvents(object imageCollection) {
			ImageCollection newImageCollection = imageCollection as ImageCollection;
			if(newImageCollection != null)
				newImageCollection.Changed += ImageListChanged;
		}
		void ImageListChanged(object sender, EventArgs e) {
			OnImageListChanged();
		}
		void UpdateLayerImageHolders(MapItemsLayerBase itemsLayer) {
			itemsLayer.UpdateImageHolders(imageList);
		}
		void OnImageListChanged() {
			if(ImageList == null || loading)
				return;
			ExecuteForEachItemsLayer(UpdateLayerImageHolders);
			foreach(MapOverlay overlay in Overlays)
				overlay.UpdateImageHolders(imageList);
			Render();
		}
		void OnLayerListenerChanged(object sender, EventArgs e) {
			EventHandler.OnLayerSupportObjectChanged();
		}
		void OnActualLayersChanged(object sender, CollectionChangedEventArgs<LayerBase> e) {
			EventHandler.OnLayersChanged(e);
			if((e.Action == CollectionChangedAction.Add) && (ActualLayers.Count == 1))
				AnimationController.SynchronizeCenterAndZoom();
			if(Legends != null && Legends.Count > 0) {
				if(e.Action == CollectionChangedAction.Clear)
					Legends.DetachLayer(null);
				if(e.Action == CollectionChangedAction.Remove)
					Legends.DetachLayer(e.Element);
			}
		}
		void OnLegendsCollectionChanged(object sender, CollectionChangedEventArgs<MapLegendBase> e) {
			EventHandler.OnLegendsChanged(e);
		}
		void OnMiniMapChanged() {
			EventHandler.OnMiniMapChanged();
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			EventHandler.OnAppearanceChanged();
		}
		void OnOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			EventHandler.OnOptionsChanged(e);
		}
		void OnCenterPointChanged() {
			if(IsRenderControllerEnabled) {
				UpdateNavigationPanel(MouseHandler.MousePosition, true);
				RenderController.PerformUpdate(UpdateActionType.Render);
			}
			NotifyObservers(new CenterPointChangedNotification() { CenterPoint = this.CenterPoint });
		}
		void ResetLayerItemsStyles(MapItemsLayerBase itemsLayer) {
			itemsLayer.ResetItemsStyle(MapItemUpdateType.Style);
		}
		void OnBackColorChanged() {
			backgroundStyle.Stroke = backColor;
			UpdateMiniMapRenderContext();
			ExecuteForEachItemsLayer(ResetLayerItemsStyles);
			PerformRenderControllerUpdate(UpdateActionType.Render);
		}
		void PerformRenderControllerUpdate(UpdateActionType update) {
			if(IsRenderControllerEnabled)
				RenderController.PerformUpdate(update);
		}
		void ExecuteForEachItemsLayer(Action<MapItemsLayerBase> action) {
			lock(ActualLayers) {
				foreach(LayerBase layer in ActualLayers) {
					MapItemsLayerBase itemsLayer = layer as MapItemsLayerBase;
					if(itemsLayer != null)
						action(itemsLayer);
				}
			}
		}
		void OnZoomLevelChanged() {
			if(IsRenderControllerInitialized) {
				RenderController.UpdateZoomTrackBarController(ZoomLevel);
				UpdateNavigationPanel(MouseHandler.MousePosition, false);
				RenderController.PerformUpdate(UpdateActionType.Render);
			}
			NotifyObservers(new ZoomChangedNotification() { ZoomLevel = this.ZoomLevel });
		}
		void OnMinMaxZoomLevelChanged() {
			if (RenderController != null)
				RenderController.UpdateTrackBarZoomLimits(MinZoomLevel, MaxZoomLevel);
			ZoomLevel = ValidateZoomLevel(ZoomLevel);
		}
		void OnCoordinateSystemChanged() {
			this.unitConverter = null;
			UpdateLayout();
			UpdateAnchorPoint(CoordinateSystem.Center); 
			NavigationPanelOptions.ResetPatterns(CoordinateSystem.PointType == CoordPointType.Geo);
			ResetViewInfos();
		}
		void OnEnableZoomingChanged() {
			ResetViewInfos();
		}
		void OnEnableScrollingChanged() {
			ResetViewInfos();
		}
		void OnFrameChanged(object sender, AnimationAction action) {
			AnimationController.FrameChanged(sender, action);
			if (action == AnimationAction.Complete)
				this.eventsCount = 0;
		}
		void ResetViewInfos() {
			if(RenderController != null)
				RenderController.PerformUpdate(UpdateActionType.Render | UpdateActionType.InvalidateViewInfo);
		}
		void OnRenderModeChanged() {
			RenderController.UpdateRenderer(ActualRenderMode, OwnedControl.Handle);
			Render();
		}
		void SetZoomLevelInternal(double newZoomLevel, bool animated, bool isRuntime) {
			double newValue = ValidateZoomLevel(newZoomLevel);
			if(newValue == zoomLevel)
				return;
			if(animated) {
				AnimationController.AnimationStartMode = isRuntime ? MapAnimationStartMode.Runtime : MouseHandler.CalculateAnimationStartMode();
				AnimationController.StartZoomAnimation(zoomLevel);
				zoomLevel = newValue;
			} else {
				zoomLevel = newValue;
				AnimationController.SynchronizeCenterAndZoom();
				UpdateViewportRect(false);
			}
			OnZoomLevelChanged();
		}
		void SetCenterPointInternal(CoordPoint newCenterPoint, bool animated, bool isRuntime) {
			if(newCenterPoint == null || !DevExpress.Map.Native.CoordinateSystemHelper.IsNumericCoordPoint(newCenterPoint))
				newCenterPoint = CreateDefaultCenterPoint();
			if(!CoordinateSystemHelper.IsNumericCoordPoint(newCenterPoint))
				Exceptions.ThrowIncorrectCoordPointException();
			if(centerPoint == newCenterPoint)
				return;
			if(animated) {
				AnimationController.AnimationStartMode = isRuntime ? MapAnimationStartMode.Runtime : MouseHandler.CalculateAnimationStartMode();
				AnimationController.InitializeScrollAnimation();
				centerPoint = newCenterPoint;
				AnimationController.StartScrollAnimation();
			} else {
				centerPoint = newCenterPoint;
				AnimationController.SynchronizeCenterAndZoom();
				UpdateViewportRect(false);
			}
			OnCenterPointChanged();
		}
		MapRect CalculateRenderBounds() {
			MapUnit p1 = ScreenPointToMapUnit(new MapPoint());
			Size contentSize = ContentRectangle.Size;
			MapUnit p2 = ScreenPointToMapUnit(new MapPoint(contentSize.Width, contentSize.Height));
			return MapRect.FromLTRB(p1.X, p1.Y, p2.X, p2.Y);
		}
		void DoEvents() {
			if (this.eventsCount < MaxEventsCount) {
				this.eventsCount++;
				Application.DoEvents();
			}
		}
		protected internal CoordPoint CreateDefaultCenterPoint() {
			return CoordinateSystem.CreatePoint(0, 0);
		}
		void UpdateContentRectangle(Rectangle clientRectangle) {
			this.contentRectangle = clientRectangle;
			if(IsRenderControllerInitialized) {
				this.contentRectangle = MapUtils.GetContentBoundsWithoutBorders(RenderController, clientRectangle);
				RenderController.SetClientSize(contentRectangle);
			}
			InteractionController.SetClientSize(contentRectangle);
		}
#if DEBUG
		void OnShowDebugInfoChanged() {
			RenderController.UpdateShowDebugInfo(ShowDebugInfo);
			Render();
		}
#endif
		void RecreateColorizersPredefinedColors() {
			foreach (LayerBase layer in Layers)
				layer.ApplyPredefinedColorSchema();
		}
		void UpdateItemsLayout(MapItemsLayerBase layer) {
			layer.UpdateItemsLayout();
		}
		void UpdateMiniMapRenderContext() {
			if (MiniMap != null) MiniMap.UpdateRenderContext();
		}
		Size GetMapBaseSizeInPixels() {
			return InitialMapSize;
		}
		#region Event subscription
		void SubscribeEvents() {
			SubscribeOptionsEvent();
			SubscribeActualLayersEvents();
			SubscribeLegendEvents();
			this.layerListener.Changed += OnLayerListenerChanged;
		}
		void SubscribeActualLayersEvents() {
			ActualLayers.CollectionChanged += OnActualLayersChanged;
		}
		void SubscribeLegendEvents() {
			Legends.CollectionChanged += OnLegendsCollectionChanged;
		}
		void SubscribeOptionsEvent() {
			NavigationPanelOptions.Changed += OnOptionsChanged;
		}
		void UnsubscribeEvents() {
			UnsubscribeOptionsEvent();
			UnsubscribeActualLayersEvents();
			UnsubscribeLegendEvents();
			this.layerListener.Changed -= OnLayerListenerChanged;
		}
		void UnsubscribeOptionsEvent() {
			if(NavigationPanelOptions != null)
				NavigationPanelOptions.Changed -= OnOptionsChanged;
		}
		void UnsubscribeLegendEvents() {
			if(Legends != null)
				Legends.CollectionChanged -= OnLegendsCollectionChanged;
		}
		void UnsubscribeActualLayersEvents() {
			if(ActualLayers != null)
				ActualLayers.CollectionChanged -= OnActualLayersChanged;
		}
		#endregion
		protected virtual void InitializeServices() {
			ServiceManager.AddService(typeof(IKeyboardHandlerService), new MapKeyboardHandlerService(this, CreateKeyboardHandler()));
			ServiceManager.AddService(typeof(IPrintableService), new MapPrintableService(this));
			ServiceManager.AddService(typeof(IInnerMapService), new InnerMapService(this));
			ServiceManager.AddService(typeof(IZoomToRegionService), new ZoomToRegionService(this));
			ServiceManager.AddService(typeof(IMouseHandlerService), new MapMouseHandlerService(this));
		}
		protected virtual RenderController CreateRenderController() {
			return new RenderController(this);
		}
		protected virtual AnimationController CreateAnimationController() {
			return new AnimationController(this);
		}
		protected virtual MapPrinter CreatePrinter() {
			return new MapPrinter(this);
		}
		protected virtual Utils.KeyboardHandler.KeyboardHandler CreateKeyboardHandler() {
			return new MapKeyboardHandler(this);
		}
		protected virtual MapMouseHandler CreateMouseHandler() {
			if(IsDesignMode || IsDesigntimeProcess)
				return new DesignMapMouseHandler(this);
			return new MapMouseHandler(this);
		}
		protected override void DisposeOverride() {
			UnsubscribeEvents();
			if(layerListener != null) {
				layerListener.Dispose();
				layerListener = null;
			}
			ReleaseRenderController();
			if(hitTestController != null) {
				hitTestController.Dispose();
				hitTestController = null;
			}
			if(animationController != null) {
				animationController.Dispose();
				animationController = null;
			}
			if(mouseHandler != null) {
				mouseHandler.Dispose();
				mouseHandler = null;
			}
			if(interactionController != null)
				interactionController.Dispose();
			if(Legends != null) {
				MapUtils.DisposeObjectList(Legends);
				legends = null;
			}
			if(Overlays != null) {
				MapUtils.DisposeObjectList(Overlays);
				overlays = null;
			}
			if(Layers != null) {
				MapUtils.DisposeObjectList(Layers);
				layers = null;
			}
			if(ActualLayers != null) {
				actualLayers = null;
			}
			if(eventHandler != null) {
				eventHandler = null;
			}
			if(serviceManager != null) {
				serviceManager.Dispose();
				serviceManager = null;
			}
			if(animationController != null)
				animationController = null;
			if(this.printer != null) {
				this.printer.Dispose();
				this.printer = null;
			}
			if(notificationObservers != null) {
				notificationObservers.Clear();
			}
			navigationPanelOptions = null;
			printOptions = null;
		}
		internal void SetEventHandlerInternal(IMapEventHandler handler) {
			this.eventHandler = handler != null ? handler : new MapEventHandler(this);
		}
		internal void Shift(Point offset) {
			Shift(offset, true);
		}
		internal void Shift(Point offset, bool useAnimation) {
			if(OperationHelper.CanScroll()) {
				lock(ActualLayers) {
					bool animated = operationHelper.CanAnimate() && useAnimation;
					SetCenterPointInternal(Move(CenterPoint, offset), animated, false);
				}
				DoEvents(); 
			}
		}
		internal void KeyboardShift(Point offset) {
			this.keyboardShifting = true;
			Capture = true;
			Shift(offset);
			Capture = false;
			this.keyboardShifting = false;
		}
		internal bool OnExportMapItem(MapItem item) {
			if(EventListener == null)
				return true;
			ExportMapItemEventArgs args = new ExportMapItemEventArgs(item);
			EventListener.NotifyExportMapItem(args);
			return !args.Cancel;
		}
		internal void OnSelectionChanging(MapSelectionChangingEventArgs args) {
			if(EventListener != null)
				EventListener.NotifySelectionChanging(args);
		}
		internal void OnSelectionChanged() {
			if(EventListener != null)
				EventListener.NotifySelectionChanged();
		}
		internal void OnProjectionChanged() {
			this.unitConverter = null;
			UpdateLayout();
			Render();
		}
		internal void OnMeasureUnitChanged() {
			UpdateNavigationPanel(MouseHandler.MousePosition, true);
			InvalidateViewInfo();
		}
		internal void ReArrangeMiniMap() {
			PerformRenderControllerUpdate(UpdateActionType.ArrangeMiniMap | UpdateActionType.InvalidateViewInfo | UpdateActionType.Render);
		}
		internal void InvalidateViewInfo() {
			PerformRenderControllerUpdate(UpdateActionType.InvalidateViewInfo | UpdateActionType.Render);
		}
		internal double ValidateZoomLevel(double value) {
			return MathUtils.MinMax(value, MinZoomLevel, MaxZoomLevel);
		}
		internal double ValidateMinMaxZoomLevel(double value) {
			return MathUtils.MinMax(value, InnerMap.LimitMinZoomLevel, InnerMap.LimitMaxZoomLevel);
		}
		internal double ValidateMinZoomLevel(double value) {
			return value > MaxZoomLevel ? MaxZoomLevel : value;
		}
		internal double ValidateMaxZoomLevel(double value) {
			return value < MinZoomLevel ? MinZoomLevel : value;
		}
		internal void ReleaseRenderController() {
			if(renderController != null) {
				renderController.Dispose();
				renderController = null;
			}
		}
		internal void EnsureBoundingRect(ICollection<LayerBase> layers) {
			CoordinateSystem.ResetBoundingBox();
			foreach(LayerBase layer in layers) {
				MapItemsLayerBase itemsLayer = layer as MapItemsLayerBase;
				if(itemsLayer != null && itemsLayer.CheckVisibility()) 
					CoordinateSystem.UpdateBoundingBox(itemsLayer.BoundingRect);
			}
			CoordinateSystem.CorrectBoundingBox();
			foreach (LayerBase layer in layers) {
				MapItemsLayerBase itemsLayer = layer as MapItemsLayerBase;
				if (itemsLayer != null && itemsLayer.CheckVisibility()) 
					itemsLayer.UpdateItemsLayout();
			}
			UpdateAnchorPoint(CoordinateSystem.Center);
			CoordinateSystem.SetNeedUpdateBoundingBox(false);
			if(RenderController != null)
				RenderController.UpdateRenderContext(false);
			ForceRender();
		}
		internal void UpdateAnchorPoint(CoordPoint point) {
			this.anchorPoint = point;
		}
		internal void UpdateLayout() {
			ExecuteForEachItemsLayer(UpdateItemsLayout);
		}
		internal void UpdateNavigationPanel() {
			UpdateNavigationPanel(MouseHandler.MousePosition, false);
		}
		internal void UpdateNavigationPanel(Point point, bool coordinatesOnly) {
			if(ActualLayers == null || ActualLayers.Count == 0)
				return;
			if(!operationHelper.CanShowNavigationPanel())
				return;
			if(operationHelper.CanShowCoordinates())
				RenderController.ActivePoint = CoordinateSystem.MeasurePoint(UnitConverter.ScreenPointToCoordPoint(new MapPoint(point.X, point.Y)));
			if(operationHelper.CanUpdateNavigationPanelScale() && !coordinatesOnly)
				RenderController.KilometersScale = CoordinateSystem.CalculateKilometersScale(CenterPoint, ActualCenterPoint, ActualZoomLevel, CurrentViewport.ViewportInPixels, InitialMapSize);
		}
		internal void UpdateOverlays() {
			RenderController.RegisterOverlaysUpdate();
		}
		internal void UpdateLegendsItems() {
			foreach(MapLegendBase legend in Legends)
				legend.Invalidate();
		}
		internal void HandleGestureZoom(Point center, double zoomDelta) {
			if(EnableZooming && zoomDelta != 1.0) {
				double zoomDeltaPercent = zoomDelta - 1.0;
				double newZoomValue = ZoomLevel + ZoomLevel * zoomDeltaPercent;
				double newZoomLevel = ValidateZoomLevel(newZoomValue);
				Zoom(newZoomLevel, new MapPoint(center.X, center.Y));
			}
		}
		internal void HandleLookAndFeelChanged(string activeSkinName) {
			ActualSkinName = activeSkinName;
			if (RenderController == null)
				return;
			RecreateColorizersPredefinedColors();
			InteractionController.NotifyLookAndFeelChanged();
			UpdateActionType updates = UpdateActionType.UpdateStyles | UpdateActionType.InvalidateViewInfo | UpdateActionType.Render | UpdateActionType.ClearSkinElementCache;
			PerformRenderControllerUpdate(updates);
		}
		internal void HandleGesturePan(Point delta, ref Point overPan) {
			if(delta == Point.Empty)
				return;
			CoordPoint prevCenter = CenterPoint;
			Shift(new Point(-delta.X, -delta.Y), false);
			CoordPoint newCenter = CenterPoint;
			if(prevCenter.GetY() == newCenter.GetY())
				overPan.Y += delta.Y;
			if(prevCenter.GetX() == newCenter.GetX())
				overPan.X += delta.X;
		}
		internal void InitRenderController() {
			RenderController.Initialize(OwnedControl.Handle);
		}
		internal void InitializeRendering() {
			if(CanInitRenderController()) {
				InitRenderController();
				SetClientRectangle(OwnedControl.ClientRectangle);
				UpdateMiniMapRenderContext();
				RaiseRenderingInitialized();
			}
		}
		internal void UpdateTrackBarToolTip(Point point, bool visible) {
			if(!OperationHelper.CanShowToolTips())
				return;
			if(visible) {
				OwnedControl.ShowToolTip(ZoomLevel.ToString("##.##"), point);
			} else
				HideToolTip();
		}
		internal void HideToolTip() {
			OwnedControl.HideToolTip();
		}
		internal void OnLayerInsert(LayerBase value) {
			ExecuteSafeRenderAction(() => {
				lock(actualLayers) {
					actualLayers.Add(value);
				}
			});
		}
		internal void OnLayerRemove(LayerBase value) {
			ExecuteSafeRenderAction(() => {
				lock(actualLayers) {
					actualLayers.Remove(value);
				}
			});
		}
		internal void OnLayersClear() {
			ExecuteSafeRenderAction(() => {
				lock(actualLayers) {
					actualLayers.Clear();
				}
			});
		}
		internal void RecreateActualLayers(LayerCollection layers) {
			ExecuteSafeRenderAction(() => {
				lock(actualLayers) {
					actualLayers.BeginUpdate();
					try {
						actualLayers.Clear();
						actualLayers.AddRange(layers);
					} finally {
						actualLayers.EndUpdate();
					}
				}
			});
		}
		internal void UpdateLayerRenderItems(LayerBase updatedLayer) {
			if(IsRenderControllerEnabled)
				RenderController.UpdateRenderItems(updatedLayer);
		}
		internal void PrepareRenderController() {
			if(RenderController == null)
				this.renderController = CreateRenderController();
		}
		internal MapHitInfo CalcHitInfo(Point hitPoint, IMapUiHitInfo uiHitInfo) {
			return new MapHitInfo(hitPoint, hitTestController.HitTest(hitPoint), uiHitInfo);
		}
		internal List<object> CollectAllSelectedItems() {
			List<object> selectedItems = new List<object>();
			foreach(LayerBase item in ActualLayers) {
				MapItemsLayerBase layer = item as MapItemsLayerBase;
				if(layer == null)
					continue;
				IList<object> items = layer.SelectedItems;
				if(items != null)
					selectedItems.AddRange(items);
			}
			return selectedItems;
		}
		internal IRenderItemStyle OnDrawMapItem(MapItem item) {
			return EventListener != null ? EventListener.NotifyDrawMapItem(item) : null;
		}
		internal MapToolTipInfo CalculateToolTipInfo(Point point) {
			MapHitInfo hitInfo = GetHitInfo(point);
			if(hitInfo == null || hitInfo.HitObjects.Length <= 0)
				return null;
			ISupportToolTip item = hitInfo.HitObjects[hitInfo.HitObjects.Length - 1] as ISupportToolTip;
			if(item == null)
				return null;
			string text = item.CalculateToolTipText();
			return new MapToolTipInfo() { HitItem = item.ActiveObject, Text = text };
		}
		internal MapRect GetViewportBounds(bool useSprings) {
			return useSprings ? CurrentViewport.AnimatedViewportRect : CoordinateSystem.CalculateViewport(ActualZoomLevel, ActualCenterPoint, CurrentViewport.ViewportInPixels, InitialMapSize);
		}
		internal MapUnit ScreenPointToMapUnit(MapPoint point) {
			MapRect viewport = GetViewportBounds(true);
			return CoordinateSystem.ScreenPointToMapUnit(point, viewport, CurrentViewport.ViewportInPixels);
		}
		internal CoordPoint MoveBeforeZooming(CoordPoint centerPoint, MapPoint anchorPoint, double currentZoomLevel, double newZoomLevel) {
			MapSize mapSize = GetMapSizeInPixels(currentZoomLevel);
			MapSize newMapSize = GetMapSizeInPixels(newZoomLevel);
			double factorWidth = newMapSize.Width / mapSize.Width;
			double factorHeight = newMapSize.Height / mapSize.Height;
			MapUnit center = CoordinateSystem.CoordPointToMapUnit(centerPoint, true);
			MapUnit anchor = CoordinateSystem.ScreenPointToMapUnit(anchorPoint, GetViewportBounds(OperationHelper.CanAnimate()), CurrentViewport.ViewportInPixels);
			MapUnit newCenter = new MapUnit(anchor.X + (center.X - anchor.X) / factorWidth, anchor.Y + (center.Y - anchor.Y) / factorHeight);
			return CoordinateSystem.MapUnitToCoordPoint(newCenter);
		}
		internal void UpdateMiniMapLookAndFeel() {
			if (MiniMap != null)
				MiniMap.UpdateLookAndFeel();
		}
		internal void ResetErrorPanel() {
			OperationHelper.ResetErrorPanelVisibility();
			InvalidateViewInfo();
		}
		internal void ResetMiniMapErrorPanel() {
			OperationHelper.ResetMiniMapErrorPanelVisibility();
			InvalidateViewInfo();
		}
		protected internal virtual MapHitInfo GetHitInfo(Point point) {
			return MouseHandler.CurrentHitInfo;
		}
		protected internal virtual void OnBorderStyleChanged() {
			UpdateContentRectangle(ClientRectangle);
			Render();
		}
		protected internal InnerMap(object owner) {
			this.owner = owner;
			Initialize();
		}
		protected internal InnerMap(bool init) {
			Initialize();
			if(init)
				InitializeRendering();
		}
		protected internal T GetService<T>() {
			IServiceProvider provider = (IServiceProvider)this;
			return (T)provider.GetService(typeof(T));
		}
		protected internal bool IsInputKey() {
			return !IsDesignMode;
		}
		protected internal void OnKeyDown(KeyEventArgs e) {
			IKeyboardHandlerService svc = GetService<IKeyboardHandlerService>();
			if(svc != null) svc.OnKeyDown(e);
		}
		protected internal void OnKeyUp(KeyEventArgs e) {
			IKeyboardHandlerService svc = GetService<IKeyboardHandlerService>();
			if(svc != null) svc.OnKeyUp(e);
		}
		protected internal void OnKeyPress(KeyPressEventArgs e) {
			IKeyboardHandlerService svc = GetService<IKeyboardHandlerService>();
			if(svc != null) svc.OnKeyPress(e);
		}
		protected internal void RemoveChildControl(Control child) {
			OwnedControl.RemoveChildControl(child);
		}
		protected internal void AddChildControl(Control child) {
			OwnedControl.AddChildControl(child);
		}
		protected internal void OnHandleDestroyed() {
			ReleaseRenderController();
		}
		protected internal void OnPaint(PaintEventArgs e) {
			RenderController.Render(e.Graphics);
		}
		protected internal void OnMouseWheel(MouseEventArgs e) {
			if(MouseHandlerService != null)
				MouseHandlerService.OnMouseWheel(e);
		}
		protected internal void OnMouseDown(MouseEventArgs e) {
			if(MouseHandlerService != null)
				MouseHandlerService.OnMouseDown(e);
		}
		protected internal void OnMouseUp(MouseEventArgs e) {
			if(MouseHandlerService != null)
				MouseHandlerService.OnMouseUp(e);
		}
		protected internal void OnMouseMove(MouseEventArgs e) {
			if(MouseHandlerService != null)
				MouseHandlerService.OnMouseMove(e);
		}
		protected internal bool CanInitRenderController() {
			return IsRenderControllerEnabled && !IsRenderControllerInitialized;
		}
		protected internal Graphics CreateGraphics() {
			return OwnedControl.CreateGraphics();
		}
		protected internal CoordPoint Move(CoordPoint centerPoint, Point offset) {
			MapPoint point = UnitConverter.CoordPointToScreenPoint(centerPoint);
			return UnitConverter.ScreenPointToCoordPoint(new MapPoint(point.X + offset.X, point.Y + offset.Y), false);
		}
		protected internal virtual void SetClientSize(Size size) {
			CurrentViewport.ViewportInPixels = size;
			if(ActualLayers != null)
				foreach(LayerBase layer in ActualLayers) {
					layer.OnSetClientSize(size);
				}
			UpdateViewportRect(false);
		}
		protected internal CoordPoint GetMinimumCoordinate() {
			return CoordinateSystem.CreatePoint(CoordinateSystem.BoundingBox.X1, CoordinateSystem.BoundingBox.Y1);
		}
		protected internal bool AllowHitTest(bool isClicked) {
			return (!Capture || !EnableScrolling) && (!AnimationInProgress || isClicked); 
		}
		public void DataBind() {
			RenderController.ExecuteSafeAction(PrepareAllLayersData);
		}
		public void DataBind(VectorItemsLayer layer) {
			if(layer == null || !layer.CheckVisibility())
				return;
			RenderController.ExecuteSafeAction(PrepareLayerData, layer);
		}
		public void SetExternalInvoker(ISynchronizeInvoke invoker) {
			this.externalInvoker = invoker;
		}
		public void SetExternalListener(IMapControlEventsListener listener) {
			this.externalListener = listener;
		}
		public void SetMapItemFactory(IMapItemFactory factory) {
			if(mapItemFactory == null)
				mapItemFactory = new DefaultMapItemFactory();
			if(mapItemFactory == factory)
				return;
			if(mapItemFactory.GetType() == typeof(DefaultMapItemFactory) && factory.GetType() == typeof(DefaultMapItemFactory))
				return;
			this.mapItemFactory = factory;
		}
		public void Render() {
			PerformRenderControllerUpdate(UpdateActionType.Render);
		}
		public void ZoomOut() {
			double zoomLevel = CalculateNextZoomOutLevel(-1, -0.1);
			Zoom(zoomLevel);
		}
		public void ZoomIn() {
			double zoomLevel = CalculateNextZoomInLevel(1, 0.1);
			Zoom(zoomLevel);
		}
		public void Zoom(double zoomLevel) {
			Zoom(zoomLevel, true);
		}
		public void Zoom(double newZoomLevel, MapPoint anchorPoint) {
			Zoom(newZoomLevel, anchorPoint, true);
		}
		public void Zoom(double zoomLevel, bool animated) {
			Zoom(zoomLevel, new MapPoint(ContentRectangle.Width / 2.0, ContentRectangle.Height / 2.0), animated);
		}
		public void Zoom(double newZoomLevel, MapPoint anchorPoint, bool animated) {
			if(!OperationHelper.CanZoom())
				return;
			if(ZoomLevel == newZoomLevel)
				return;
			animated = animated && OperationHelper.CanAnimate();
			CoordPoint newCenter = MoveBeforeZooming(ActualCenterPoint, anchorPoint, ActualZoomLevel, newZoomLevel);
			if(animated) {
				AnimationController.InitialZoomAnchorPoint = anchorPoint;
				AnimationController.InitializeZoomAnimation();
				if(AnimationController.NeedChangeCenterWhileZooming)
					centerPoint = newCenter;
				Application.DoEvents();
			} else
				centerPoint = newCenter;
			SetZoomLevelInternal(newZoomLevel, animated, true);
		}
		public void SuspendRender() {
			if(IsRenderControllerEnabled)
				RenderController.SuspendRender();
		}
		public void ResumeRender() {
			if(IsRenderControllerEnabled)
				RenderController.ResumeRender();
		}
		public void ForceRender() {
			Render();
		}
		public void SetClientRectangle(Rectangle bounds) {
			this.clientRectangle = bounds;
			UpdateContentRectangle(bounds);
			Render();
		}
		protected internal Rectangle GetViewAreaBounds() {
			if(operationHelper.CanShowNavigationPanel())
				return RectUtils.CutFromBottom(contentRectangle, NavigationPanelOptions.Height);
			return contentRectangle;
		}
		public void SetCenterPoint(CoordPoint centerPoint, bool animated) {
			animated = operationHelper.CanAnimate() && animated;
			SetCenterPointInternal(centerPoint, animated, true);
		}
		public void ExecuteSafeRenderAction(Action action) {
			if(IsRenderControllerEnabled)
				RenderController.ExecuteSafeAction(action);
			else
				action();
		}
		public void ZoomToFit(IEnumerable<MapItem> items, double paddingFactor) {
			if(!OperationHelper.CanZoom() || items == null)
				return;
			CoordBounds bounds = CoordPointHelper.SelectItemsBounds(items);
			ZoomToFit(bounds, paddingFactor);
		}
		public void ZoomToFit(CoordBounds bounds, double paddingFactor) {
			if(bounds.IsEmpty || !OperationHelper.CanZoom())
				return;
			CoordPoint topLeft = UnitConverter.PointFactory.CreatePoint(bounds.X1, bounds.Y1);
			CoordPoint bottomRight = UnitConverter.PointFactory.CreatePoint(bounds.X2, bounds.Y2);
			ZoomToRegion(topLeft, bottomRight, paddingFactor);
		}
		public void ZoomToRegion(CoordPoint p1, CoordPoint p2) {
			ZoomToRegion(p1, p2, DefaultZoomPadding);
		}
		public void ZoomToRegion(CoordPoint p1, CoordPoint p2, double paddingFactor) {
			if(!OperationHelper.CanZoom())
				return;
			CoordPoint normalized1 = CoordinateSystemHelper.CreateNormalizedPoint(p1);
			CoordPoint normalized2 = CoordinateSystemHelper.CreateNormalizedPoint(p2);
			CoordBounds bounds = new CoordBounds(normalized1, normalized2);
			CoordPoint topLeft = UnitConverter.PointFactory.CreatePoint(bounds.X1, bounds.Y1);
			CoordPoint bottomRight = UnitConverter.PointFactory.CreatePoint(bounds.X2, bounds.Y2);
			MapUnit minUnit = CoordinateSystem.CoordPointToMapUnit(topLeft, true);
			MapUnit maxUnit = CoordinateSystem.CoordPointToMapUnit(bottomRight, true);
			MapUnit centerUnit = new MapUnit((minUnit.X + maxUnit.X) / 2.0, (minUnit.Y + maxUnit.Y) / 2.0);
			CoordPoint center = CoordinateSystem.MapUnitToCoordPoint(centerUnit);
			IZoomToRegionService svc = GetService<IZoomToRegionService>();
			if(svc != null) svc.ZoomToRegion(topLeft, bottomRight, center, paddingFactor);
		}
		public void SelectItemsByRegion(CoordPoint p1, CoordPoint p2) {
			Point point1 = UnitConverter.CoordPointToScreenPoint(p1).ToPoint();
			Point point2 = UnitConverter.CoordPointToScreenPoint(p2).ToPoint();
			Point pLT = new Point(Math.Min(point1.X, point2.X), Math.Min(point1.Y, point2.Y));
			Point pRB = new Point(Math.Max(point1.X, point2.X), Math.Max(point1.Y, point2.Y));
			SelectItemsByRegion(new Rectangle(pLT.X, pLT.Y, pRB.X - pLT.X, pRB.Y - pLT.Y));
		}
		public void SelectItemsByRegion(Rectangle screenRegion) {
			screenRegion = RectUtils.ValidateDimensions(screenRegion);
			InteractionController.SelectItemsByRegion(screenRegion);
		}
		protected internal CoordPoint GetViewLeftTopInCoordPoint() {
			MapUnit unit = new MapUnit(CurrentViewport.AnimatedViewportRect.Left, CurrentViewport.AnimatedViewportRect.Top);
			return CoordinateSystem.MapUnitToCoordPoint(unit);
		}
		protected internal CoordPoint GetViewRightBottomInCoordPoint() {
			MapUnit unit = new MapUnit(CurrentViewport.AnimatedViewportRect.Right, CurrentViewport.AnimatedViewportRect.Bottom);
			return CoordinateSystem.MapUnitToCoordPoint(unit);
		}
		protected internal MapRect CalculateViewportRegion(CoordPoint leftTop, CoordPoint rightBottom) {
			MapUnit leftTopUnit = CoordinateSystem.CoordPointToMapUnit(leftTop, true);
			MapUnit rightBottomUnit = CoordinateSystem.CoordPointToMapUnit(rightBottom, true);
			MapPoint leftTopPt = UnitConverter.MapUnitToScreenPoint(leftTopUnit, false);
			MapPoint rightBottomPt = UnitConverter.MapUnitToScreenPoint(rightBottomUnit, false);
			double x = Math.Min(leftTopPt.X, rightBottomPt.X);
			double y = Math.Min(leftTopPt.Y, rightBottomPt.Y);
			return new MapRect(x, y, Math.Abs(rightBottomPt.X - leftTopPt.X), Math.Abs(rightBottomPt.Y - leftTopPt.Y));
		}
		protected internal double CalculateRegionZoom(CoordPoint leftTop, CoordPoint rightBottom, double zoomLevel, double padding) {
			MapRect viewPortRect = CalculateViewportRegion(leftTop, rightBottom);
			if(RectUtils.IsBoundsEmpty(viewPortRect))
				return zoomLevel;
			double zoomCoeff = 1.0f;
			Size viewportInPixels = CurrentViewport.ViewportInPixels;
			bool adjustByHeight = (viewPortRect.Height / viewportInPixels.Height) < (viewPortRect.Width / viewportInPixels.Width);
			if(adjustByHeight)
				zoomCoeff = viewportInPixels.Width / viewPortRect.Width;
			else
				zoomCoeff = viewportInPixels.Height / viewPortRect.Height;
			MapSize currentSize = GetMapSizeInPixels(zoomLevel);
			MapSize newSize = new MapSize(currentSize.Width * zoomCoeff, currentSize.Height * zoomCoeff);
			Size baseSize = GetMapBaseSizeInPixels();
			double baseLen = adjustByHeight ? baseSize.Height : baseSize.Width;
			double newLen = adjustByHeight ? newSize.Height : newSize.Width;
			double value = newLen / baseLen;
			padding = MathUtils.MinMax(padding, 0.0, 1.0);
			value *= 1.0 - padding;
			if(value < 1.0) {
				return value;
			}
			double newZoomLevel = Math.Log(value, 2) + 1;
			return newZoomLevel;
		}
		internal CoordBounds CalculateLayersItemsBounds(IEnumerable<LayerBase> layers) {
			return CoordPointHelper.SelectLayersItemsBounds(layers);
		}
		public MapHitInfo CalcHitInfo(Point hitPoint) {
			IMapUiHitInfo uiHitInfo = CalcUiHitInfo(hitPoint);
			return CalcHitInfo(hitPoint, uiHitInfo);
		}
		internal IMapUiHitInfo CalcUiHitInfo(Point point) {
			if(operationHelper.CanShowMiniMap() && MiniMap.Bounds.Contains(point))
				return new MapUiHitInfo(point, MapHitUiElementType.MiniMap);
			return RenderController.CalcUiHitInfo(point);
		}
		#region viewport methods
		protected internal void UpdateViewportRect(bool animationComplete) {
			MapRect viewportRect = CoordinateSystem.CalculateViewport(ActualZoomLevel, ActualCenterPoint, CurrentViewport.ViewportInPixels, InitialMapSize);
			if(animationComplete)
				SetViewportRect(viewportRect);
			else
				Viewport = viewportRect;
		}
		void SetViewportRect(MapRect value) {
			CurrentViewport.AnimatedViewportRect = value;
			UpdateLayersViewport();
			RenderBounds = CalculateRenderBounds();
		}
		protected void UpdateLayersViewport() {
			if(Layers != null)
				foreach(LayerBase layer in Layers) {
					layer.ResetViewport();
					layer.RaiseViewportChanged();
					layer.ViewportUpdated();
				}
		}
		#endregion
		#region printing methods
		public void ShowPrintPreview() {
			DeferredActionCommand.Execute(this, () => { Printer.ShowPreview(); });
		}
		public void ShowRibbonPrintPreview() {
			DeferredActionCommand.Execute(this, () => { Printer.ShowRibbonPreview(); });
		}
		public void Print() {
			DeferredActionCommand.Execute(this, () => { Printer.Print(); });
		}
		public void ShowPrintDialog() {
			DeferredActionCommand.Execute(this, () => { Printer.PrintDialog(); });
		}
		public void ExportToPdf(string filePath) {
			DeferredActionCommand.Execute(this, () => { Printer.ExportToPdf(filePath); });
		}
		public void ExportToPdf(string filePath, PdfExportOptions options) {
			DeferredActionCommand.Execute(this, () => { Printer.ExportToPdf(filePath, options); });
		}
		public void ExportToPdf(Stream stream) {
			DeferredActionCommand.Execute(this, () => { Printer.ExportToPdf(stream); });
		}
		public void ExportToPdf(Stream stream, PdfExportOptions options) {
			DeferredActionCommand.Execute(this, () => { Printer.ExportToPdf(stream, options); });
		}
		public void ExportToImage(string filePath, ImageFormat format) {
			DeferredActionCommand.Execute(this, () => { Printer.ExportToImage(filePath, format); });
		}
		public void ExportToImage(Stream stream, ImageFormat format) {
			DeferredActionCommand.Execute(this, () => { Printer.ExportToImage(stream, format); });
		}
		public void ExportToRtf(string filePath) {
			DeferredActionCommand.Execute(this, () => { Printer.ExportToRtf(filePath); });
		}
		public void ExportToRtf(Stream stream) {
			DeferredActionCommand.Execute(this, () => { Printer.ExportToRtf(stream); });
		}
		public void ExportToMht(string filePath) {
			DeferredActionCommand.Execute(this, () => { Printer.ExportToMht(filePath); });
		}
		public void ExportToXls(string filePath) {
			DeferredActionCommand.Execute(this, () => { Printer.ExportToXls(filePath); });
		}
		public void ExportToXls(Stream stream) {
			DeferredActionCommand.Execute(this, () => { Printer.ExportToXls(stream); });
		}
		public void ExportToXlsx(string filePath) {
			DeferredActionCommand.Execute(this, () => { Printer.ExportToXlsx(filePath); });
		}
		public void ExportToXlsx(Stream stream) {
			DeferredActionCommand.Execute(this, () => { Printer.ExportToXlsx(stream); });
		}
		#endregion
		#region observers
		internal void AddNotificationObserver(IMapNotificationObserver observer) {
			if(!NotificationObservers.Contains(observer))
				NotificationObservers.Add(observer);
		}
		internal void RemoveNotificationObserver(IMapNotificationObserver observer) {
			if(NotificationObservers.Contains(observer))
				NotificationObservers.Remove(observer);
		}
		internal void NotifyObservers(MapNotification info) {
			foreach(IMapNotificationObserver observer in NotificationObservers)
				observer.HandleChanged(info);
		}
		#endregion  
	}
}
