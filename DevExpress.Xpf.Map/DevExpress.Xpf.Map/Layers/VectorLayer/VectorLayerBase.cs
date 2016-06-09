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

using DevExpress.Data.Svg;
using DevExpress.Map;
using DevExpress.Map.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
namespace DevExpress.Xpf.Map {
	[TemplatePart(Name = "PART_Elements", Type = typeof(VectorLayerItemsControl))]
	public abstract class VectorLayerBase : LayerBase {
		#region Dependency properties
		public static readonly DependencyProperty EnableHighlightingProperty = DependencyPropertyManager.Register("EnableHighlighting",
			typeof(bool), typeof(VectorLayerBase), new PropertyMetadata(true));
		public static readonly DependencyProperty ToolTipPatternProperty = DependencyPropertyManager.Register("ToolTipPattern",
			typeof(string), typeof(VectorLayerBase), new PropertyMetadata(null, OnToolTipPatternPropertyChanged));
		public static readonly DependencyProperty ToolTipContentTemplateProperty = DependencyPropertyManager.Register("ToolTipContentTemplate",
			typeof(DataTemplate), typeof(VectorLayerBase), new PropertyMetadata(null));
		public static readonly DependencyProperty AllowResetSelectionProperty = DependencyPropertyManager.Register("AllowResetSelection",
			typeof(bool), typeof(VectorLayerBase), new PropertyMetadata(true));
		public static readonly DependencyProperty SelectedItemProperty = DependencyPropertyManager.Register("SelectedItem",
			typeof(object), typeof(VectorLayerBase), new PropertyMetadata(null, OnSelectedItemChanged));
		public static readonly DependencyProperty SelectedItemsProperty = DependencyPropertyManager.Register("SelectedItems",
			typeof(IList), typeof(VectorLayerBase), new PropertyMetadata(null, OnSelectedItemsChanged));
		public static readonly DependencyProperty EnableSelectionProperty = DependencyPropertyManager.Register("EnableSelection",
				typeof(bool), typeof(VectorLayerBase), new PropertyMetadata(true, OnEnableSelectionChanged));
		public static readonly DependencyProperty ToolTipEnabledProperty = DependencyPropertyManager.Register("ToolTipEnabled",
			typeof(bool?), typeof(VectorLayerBase), new PropertyMetadata(null));
		[Obsolete(ObsoleteMessages.VectorLayerBase_InitialMapSize), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty InitialMapSizeProperty = DependencyPropertyManager.Register("InitialMapSize",
			typeof(Size), typeof(VectorLayerBase), new PropertyMetadata(DefaultMapSize, InitialMapSizePropertyChanged));
		[Obsolete(ObsoleteMessages.VectorLayerBase_MapProjection), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty MapProjectionProperty = DependencyPropertyManager.Register("MapProjection",
			typeof(ProjectionBase), typeof(VectorLayerBase));
		#endregion
		[Category(Categories.Behavior)]
		public bool EnableHighlighting {
			get { return (bool)GetValue(EnableHighlightingProperty); }
			set { SetValue(EnableHighlightingProperty, value); }
		}
		[NonCategorized]
		public string ToolTipPattern {
			get { return (string)GetValue(ToolTipPatternProperty); }
			set { SetValue(ToolTipPatternProperty, value); }
		}
		[NonCategorized]
		public DataTemplate ToolTipContentTemplate {
			get { return (DataTemplate)GetValue(ToolTipContentTemplateProperty); }
			set { SetValue(ToolTipContentTemplateProperty, value); }
		}
		[Category(Categories.Behavior)]
		public bool EnableSelection {
			get { return (bool)GetValue(EnableSelectionProperty); }
			set { SetValue(EnableSelectionProperty, value); }
		}
		[Category(Categories.Behavior)]
		public bool AllowResetSelection {
			get { return (bool)GetValue(AllowResetSelectionProperty); }
			set { SetValue(AllowResetSelectionProperty, value); }
		}
		[NonCategorized]
		public object SelectedItem {
			get { return (object)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}
		[NonCategorized]
		public IList SelectedItems {
			get { return (IList)GetValue(SelectedItemsProperty); }
			set { SetValue(SelectedItemsProperty, value); }
		}
		[NonCategorized]
		public bool? ToolTipEnabled {
			get { return (bool?)GetValue(ToolTipEnabledProperty); }
			set { SetValue(ToolTipEnabledProperty, value); }
		}
		#region Obsolete properties
		[Obsolete(ObsoleteMessages.VectorLayerBase_MapProjection),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		TypeConverter(typeof(ExpandableObjectConverter))]
		public ProjectionBase MapProjection {
			get { return (ProjectionBase)GetValue(MapProjectionProperty); }
			set { SetValue(MapProjectionProperty, value); }
		}
		[Obsolete(ObsoleteMessages.VectorLayerBase_InitialMapSize, true),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Size InitialMapSize {
			get { return (Size)GetValue(InitialMapSizeProperty); }
			set { SetValue(InitialMapSizeProperty, value); }
		}
		#endregion
		static void InitialMapSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			VectorLayerBase layer = d as VectorLayerBase;
			if (layer != null)
				layer.UpdateInitialMapSize((Size)e.NewValue);
		}
		static void OnToolTipPatternPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			VectorLayerBase layer = d as VectorLayerBase;
			if (layer != null)
				layer.HideToolTip();
		}
		static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			VectorLayerBase layer = d as VectorLayerBase;
			if(layer != null && e.NewValue != e.OldValue)
				layer.OnUpdateSelectedItem(e.NewValue);
		}
		static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			VectorLayerBase layer = d as VectorLayerBase;
			if (layer != null)
				layer.OnUpdateSelectedItems(e.OldValue as IList, e.NewValue as IList);
		}
		static void OnEnableSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			VectorLayerBase layer = d as VectorLayerBase;
			if(layer != null && !layer.EnableSelection && layer.Visibility == Visibility.Visible)
				layer.SelectionController.ClearSelectedItems();
		}
		static bool ValidateMapProjection(object value) {
			return value != null;
		}
		bool isLoaded = false;
		VectorLayerItemsControl itemsControl = null;
		VectorLayerPanel layerPanel = null;
		GeoMapCoordinateSystem geoCoordinateSystem = new GeoMapCoordinateSystem();
		MapColorizer colorizer;
		Locker selectedItemsEventLocker = new Locker();
		readonly SelectedItemsController selectionController;
		Size initialMapSizeWhenNoMap = MapControl.DefaultInitialMapSize;
		Size InitialMapSizeInternal { get { return Map == null ? initialMapSizeWhenNoMap : Map.InitialMapSize; } }
		ISupportProjection ProjectionConteiner { get { return View != null ? View.CoordinateSystem as ISupportProjection : null; } }
		protected internal MapColorizer Colorizer {
			get { return colorizer; }
			set {
				if(Object.Equals(colorizer, value))
					return;
				CommonUtils.SetItemOwner(colorizer, null);
				CommonUtils.SetItemOwner(value, this);
				colorizer = value;
				OnColorizerChanged();
			}
		}
		protected internal SelectedItemsController SelectionController { get { return selectionController; } }
		protected internal override ProjectionBase ActualProjection { get { return ProjectionConteiner != null ? ProjectionConteiner.Projection : geoCoordinateSystem.Projection; } } 
		protected abstract MapDataAdapterBase DataAdapter { get; }
		internal bool ActualToolTipEnable {
			get {
				if (ToolTipEnabled.HasValue)
					return ToolTipEnabled.Value;
				return (Map != null) ? Map.ToolTipEnabled : false;
			}
		}
		internal VectorLayerPanel LayerPanel {
			get {
				if (layerPanel == null)
					layerPanel = CommonUtils.GetChildPanel(itemsControl) as VectorLayerPanel;
				return layerPanel;
			}
		}
		internal IList<MapItem> DataItems { get { return DataAdapter != null ? (IList<MapItem>)DataAdapter.ActualItems : new MapItem[0]; } }
		internal VectorLayerItemsControl ElementsItemsControl {
			get { return itemsControl; }
			set {
				if (itemsControl != value) {
					SetVisualItemsSource(null);
					itemsControl = value;
					UpdateItemsSource(true);
				}
			}
		}
		public VectorLayerBase() {
			DefaultStyleKey = typeof(VectorLayerBase);
			selectionController = new SelectedItemsController(this);
			Loaded += new RoutedEventHandler(VectorLayerBase_Loaded);
		}
		void VectorLayerBase_Loaded(object sender, RoutedEventArgs e) {
			if (!isLoaded) {
				isLoaded = true;
				if (DataAdapter != null)
					DataAdapter.LoadDataInternal();
				selectionController.UpdateActualItemsSelection();
				OnDataLoaded();
			}
		}
		void UpdateLegendsVisibility() {
			if(Map != null)
				Map.UpdateLayerLegendsVisibility(this);
		}
		void SetVisualItemsSource(IEnumerable source) {
			if (itemsControl != null)
				itemsControl.ItemsSource = source;
		}
		void OnUpdateSelectedItem(object item) {
			selectionController.OnUpdateSelectedItem(item);
		}
		void OnUpdateSelectedItems(IList oldList, IList newList) {
			if ((oldList == null || oldList.Count == 0) && (newList == null || newList.Count == 0))
				return;
			selectionController.OnUpdateSelectedItems(newList);
		}
		void SetItemOwner(MapItem item, VectorLayerBase owner) {
			IOwnedElement ownedElement = item as IOwnedElement;
			if ((ownedElement != null) && (ownedElement.Owner != this))
				ownedElement.Owner = this;
		}
		void UpdateInitialMapSize(Size size) {
			if (Map != null)
				Map.SetInitialMapSize(size);
			else
				initialMapSizeWhenNoMap = size;
		}
		void OnColorizerChanged() {
			if (Map != null)
				Map.OnColorizerChanged();
		}
		void UpdateToolTipPosition() {
			if(Map != null)
				Map.UpdateToolTipPosition();
		}
		Point CalcLeftTopLocationInPixels() {
			if (View == null)
				return new Point();
			Rect viewport = ActualViewport;
			double unitFactorX = viewport.Width > 0 ? View.ViewportInPixels.Width / viewport.Width : 0.0;
			double unitFactorY = viewport.Height > 0 ? View.ViewportInPixels.Height / viewport.Height : 0.0;
			return new Point(-viewport.X * unitFactorX, -viewport.Y * unitFactorY);
		}
		XpfSvgPointConverterBase CreateSvgPointConverter(bool isGeoSystem) {
			if(isGeoSystem)
				return new XpfSvgGeoPointConverter(Map.CoordinateSystem, this);
			return new XpfSvgCartesianPointConverter(Map != null ? Map.CoordinateSystem : CommonUtils.SvgDefaultCoordinateSystem, this);
		}
		ShpExporter CreateShpExporter() {
			return new ShpExporter();
		}
		KmlExporter<MapItem> CreateKmlExporter() {
			return new KmlExporter<MapItem>();
		}
		protected internal SvgExporter<MapItem> CreateSvgExporter(double scale) {
			bool isGeoMap = Map != null && Map.CoordinateSystem.PointType == CoordPointType.Geo;
			CoordBounds layerBounds = GetBoundingRect();
			SvgExporter<MapItem> exporter = new SvgExporter<MapItem>();
			exporter.ScaleFactor = scale <= 0.0 ? 1.0 : scale;
			exporter.PointConverter = CreateSvgPointConverter(isGeoMap);
			exporter.CanvasSize = isGeoMap ? new SvgSize(Map.InitialMapSize.Width, Map.InitialMapSize.Height) : new SvgSize(layerBounds.Width, layerBounds.Height);
			return exporter;
		}
		protected virtual void NotifyViewportUpdated() {
			if(DataAdapter != null)
				DataAdapter.OnViewportUpdated(GetViewport());
		}
		protected void UpdateItemsContainerLocation(bool applyZoomStrategy) {
			if (itemsControl != null) {
				Point location = !applyZoomStrategy ? CalcLeftTopLocationInPixels() : new Point();
				Canvas.SetLeft(itemsControl, location.X);
				Canvas.SetTop(itemsControl, location.Y);
			}
		}
		protected void ApplyShapeAppearance() {
			foreach (MapItem item in DataItems)
				item.ApplyAppearance();
		}
		protected internal void UpdateItemsSource(bool clearVirtualizingClusters) {
			if(DataAdapter != null) {
				MapVectorItemCollection items = DataAdapter.ActualItems;
				items.ClearVirtualizingCollection(clearVirtualizingClusters);
				SetVisualItemsSource(items.VirtualizingCollection);
				if(ElementsItemsControl != null)
					items.FillVirtualizingCollection();
			}
		}
		protected override void BeforeSizeProgressCompleting() {
			base.BeforeSizeProgressCompleting();
			NotifyViewportUpdated();
		}
		protected override void ViewportUpdated(bool zoomLevelChanged) {
			base.ViewportUpdated(zoomLevelChanged);
			if(!UseSprings && zoomLevelChanged)
				NotifyViewportUpdated();
			bool applyZoomStrategy = CanApplyZoomStrategy;
			UpdateItemsContainerLocation(applyZoomStrategy);
			if(zoomLevelChanged || applyZoomStrategy)
				Invalidate();
			else
				UpdateToolTipPosition();
		}
		protected override Size GetMapSizeInPixels(double zoomLevel) {
			double coeff = Math.Max(0, (zoomLevel < 1.0) ? zoomLevel : Math.Pow(2.0, zoomLevel - 1));
			return new Size(coeff * InitialMapSizeInternal.Width, coeff * InitialMapSizeInternal.Height);
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			Clip = new RectangleGeometry() { Rect = new Rect(0, 0, arrangeBounds.Width, arrangeBounds.Height) };
			return base.ArrangeOverride(arrangeBounds);
		}
		protected internal override Size GetMapBaseSizeInPixels() {
			return InitialMapSizeInternal;
		}
		protected internal override void CheckCompatibility() {
			if (ElementsItemsControl != null)
				ElementsItemsControl.Visibility = CalculateItemsVisibility();
		}
		Visibility CalculateItemsVisibility() {
			if(DataAdapter != null && !DataAdapter.IsCSCompatibleTo(ActualCoordinateSystem))
				return Visibility.Collapsed;
			return Visibility.Visible;
		}
		protected internal override Point CoordPointToScreenPointZeroOffset(CoordPoint point, bool useSpringsAnimation, bool shouldNormalize) {
			if (shouldNormalize)
				point = CoordinateSystemHelper.CreateNormalizedPoint(point);
			return MapUnitToScreenZeroOffset(ActualCoordinateSystem.CoordPointToMapUnit(point));
		}
		protected internal override void UpdateItemsLayout() {
			foreach (MapItem item in DataItems)
				item.UpdateLayout();
		}
		protected override void OnIsVisibleChanged() {
			base.OnIsVisibleChanged();
			HideToolTip();
			UpdateLegendsVisibility();
		}
		protected override DataLoadedEventArgs CreateDataLoadedEventArgs() {
			return new MapItemsLoadedEventArgs(DataItems);
		}
		protected override void OwnerChanged() {
			base.OwnerChanged();
			UpdateBoundingRect();
		}
		internal void ColorizeItems() {
			if (isLoaded && Map != null) {
				if (Colorizer != null) {
					Colorizer.Reset();
					foreach (MapItem item in DataItems)
						item.Colorize(colorizer);
				}
				else
					ResetColors();
			}
		}
		internal void ResetColors() {
			foreach (MapItem item in DataItems) {
				item.ResetColor();
			}
		}
		internal void OnSelectionChanged() {
			if (Map != null && !selectedItemsEventLocker.IsLocked)
				Map.RaiseSelectionChanged();
		}
		internal void UpdateItemSelection(ElementSelectionMode selectionMode, ModifierKeys keyModifiers, MapItemInfo item) {
			SelectionController.UpdateItemSelection(selectionMode, keyModifiers, item);
		}
		internal void ClearSelectionWithoutEvent() {
			selectedItemsEventLocker.Lock();
			SelectionController.ClearSelectedItems();
			selectedItemsEventLocker.Unlock();
		}
		internal void UpdateLegends() {
			if(Map != null)
				Map.UpdateLegends();
		}
		internal void OnDataLoaded() {
			ColorizeItems();
			UpdateLegends();
			RaiseDataLoaded();
		}
		internal void HideToolTip() {
			if(Map != null)
				Map.HideToolTip();
		}
		internal void Invalidate() {
			if(LayerPanel != null)
				LayerPanel.InvalidateMeasure();
			UpdateToolTipPosition();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			layerPanel = null;
			ElementsItemsControl = GetTemplateChild("PART_Elements") as VectorLayerItemsControl;
			selectionController.UpdateActualItemsSelection();
			CheckCompatibility();
		}
		public MapViewport GetViewport() {
			if(View == null)
				return new MapViewport();
			return new MapViewport() { CenterPoint = View.CenterPoint, ZoomLevel = View.ZoomLevel }; 
		}
		#region Export methods
		public void ExportToKml(string filePath) {
			KmlExporter<MapItem> exporter = CreateKmlExporter();
			exporter.Export(filePath, DataItems);
		}
		public void ExportToKml(Stream stream) {
			KmlExporter<MapItem> exporter = CreateKmlExporter();
			exporter.Export(stream, DataItems);
		}
		public void ExportToSvg(string filePath, double scale) {
			SvgExporter<MapItem> exporter = CreateSvgExporter(scale);
			exporter.Export(filePath, DataItems);
		}
		public void ExportToSvg(string filePath) {
			ExportToSvg(filePath, 1.0);
		}
		public void ExportToSvg(Stream stream, double scale) {
			SvgExporter<MapItem> exporter = CreateSvgExporter(scale);
			exporter.Export(stream, DataItems);
		}
		public void ExportToSvg(Stream stream) {
			ExportToSvg(stream, 1.0);
		}
		public void ExportToShp(string filePath, ShpExportOptions options) {
			ShpExporter exporter = CreateShpExporter();
			exporter.Export(filePath, DataItems, (ShpRecordTypes)options.ShapeType, options.ExportToDbf, null);
		}
		#endregion
	}
}
