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
using DevExpress.Map.Native;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
namespace DevExpress.XtraMap {
	public abstract class MapItemsLayerBase : LayerBase, IHitTestableRegistrator, IMapStyleOwner, ILegendDataProvider {
		const string DefaultShapeTitlesPattern = "{NAME}";
		const VisibilityMode DefaultShapeTitlesVisibility = VisibilityMode.Auto;
		IMapDataAdapter dataAdapter;
		readonly SelectedItemCollection selectedItems;
		readonly SelectedItemsController selectionController;
		string toolTipPattern = string.Empty;
		bool enableHighlighting = true;
		bool enableSelection = true;
		int itemImageIndex = MapPointer.DefaultImageIndex;
		MapItemStyle itemStyle;
		MapItemStyle selectedItemStyle;
		MapItemStyle highlightedItemStyle;
		LayerColoredSkinElementCache coloredSkinElementCache;
		bool shouldRaiseDataLoadedEvent;
		bool nativeBoundsAreValid;
		string shapeTitlesPattern = DefaultShapeTitlesPattern;
		VisibilityMode shapeTitlesVisibility = DefaultShapeTitlesVisibility;
		MapColorizer colorizer;
		Size initialMapSizeWhenNoMap = InnerMap.DefaultInitialSize;
		Size InitialMapSizeInternal { get { return Map == null ? initialMapSizeWhenNoMap : Map.InitialMapSize; } }
		protected virtual IMapEventHandler EventHandler { get { return View != null ? View.EventHandler : null; } }
		protected IHitTestableRegistrator OwnerHitTestableRegistrator { get { return Map != null ? Map.HitTestController : null; } }
		protected bool ShouldRaiseDataLoadedEvent { get { return shouldRaiseDataLoadedEvent; } set { shouldRaiseDataLoadedEvent = value; } }
		protected internal bool NativeBoundsAreValid {
			get { return nativeBoundsAreValid || (Map != null && Map.CoordinateSystem.PointType == CoordPointType.Geo); }
			set { nativeBoundsAreValid = value; } 
		}
		protected IMapDataAdapter DataAdapter { get { return dataAdapter; } }
		protected internal bool IsValidData { get { return DataAdapter != null && DataAdapter.GetLayer() == this; } }
		protected internal SelectedItemsController SelectionController { get { return selectionController; } }
		protected internal abstract CoordBounds BoundingRect { get; }
		protected internal IEnumerable<MapItem> DataItems { get { return IsValidData ? DataAdapter.Items : new MapItem[0]; } }
		protected internal MapItemStyle BackgroundStyle { get { return Map != null ? Map.BackgroundStyle : null; } }
		protected internal IMapItemStyleProvider DefaultItemStyleProvider { get { return View != null ? View.StyleProvider : null; } }
		protected internal LayerColoredSkinElementCache ColoredSkinElementCache {
			get {
				if (coloredSkinElementCache == null)
					coloredSkinElementCache = CreateColoredSkinElementCache();
				return coloredSkinElementCache;
			}
		}
		protected internal MapColorizer Colorizer {
			get { return colorizer; }
			set {
				if (Object.Equals(colorizer, value))
					return;
				MapUtils.SetOwner(colorizer, null);
				MapUtils.SetOwner(value, this);
				colorizer = value;
				OnColorizerChanged();
			}
		}
		protected internal bool NeedEnsureBoundingRect { get { return DataAdapter != null && DataAdapter.Clusterer != null && 
																	 DataAdapter.Clusterer.IsBusy ? false : !NativeBoundsAreValid; } }
		[Browsable(false)]
		public SelectedItemCollection SelectedItems { get { return selectedItems; } }
		[Browsable(false), DefaultValue(null)]
		public object SelectedItem {
			get { return selectedItems.Count > 0 ? selectedItems[0] : null; ; }
			set {
				if(Object.Equals(SelectedItem, value) && (SelectedItems.Count == 1))
					return;
				selectionController.SetSelectedItem(value);
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemsLayerBaseToolTipPattern"),
#endif
		Category(SRCategoryNames.Appearance), DefaultValue(""), Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		public string ToolTipPattern {
			get { return toolTipPattern; }
			set {
				if (toolTipPattern == value)
					return;
				toolTipPattern = value;
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemsLayerBaseEnableSelection"),
#endif
		DefaultValue(true), Category(SRCategoryNames.Behavior)]
		public bool EnableSelection {
			get { return enableSelection; }
			set {
				if (enableSelection == value)
					return;
				enableSelection = value;
				OnEnableSelectionChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemsLayerBaseEnableHighlighting"),
#endif
		DefaultValue(true), Category(SRCategoryNames.Behavior)]
		public bool EnableHighlighting {
			get { return enableHighlighting; }
			set {
				if (enableHighlighting == value)
					return;
				enableHighlighting = value;
				OnEnableHighlightingChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemsLayerBaseItemImageIndex"),
#endif
		DefaultValue(MapPointer.DefaultImageIndex), Category(SRCategoryNames.Appearance)]
		public int ItemImageIndex {
			get { return itemImageIndex; }
			set {
				if (itemImageIndex == value)
					return;
				itemImageIndex = value;
				OnItemImageIndexChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemsLayerBaseShapeTitlesPattern"),
#endif
		Category(SRCategoryNames.Appearance), DefaultValue(DefaultShapeTitlesPattern)]
		public string ShapeTitlesPattern {
			get { return shapeTitlesPattern; }
			set {
				if (shapeTitlesPattern == value) return;
				shapeTitlesPattern = value;
				ApplyShapeTitleOptions();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemsLayerBaseShapeTitlesVisibility"),
#endif
		Category(SRCategoryNames.Appearance), DefaultValue(DefaultShapeTitlesVisibility)]
		public VisibilityMode ShapeTitlesVisibility {
			get { return shapeTitlesVisibility; }
			set {
				if (shapeTitlesVisibility == value) return;
				shapeTitlesVisibility = value;
				ApplyShapeTitleOptions();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemsLayerBaseItemStyle"),
#endif
		Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public MapItemStyle ItemStyle { get { return itemStyle; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemsLayerBaseHighlightedItemStyle"),
#endif
		Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public MapItemStyle HighlightedItemStyle { get { return highlightedItemStyle; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemsLayerBaseSelectedItemStyle"),
#endif
		Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public MapItemStyle SelectedItemStyle { get { return selectedItemStyle; } }
		protected MapItemsLayerBase() {
			selectedItems = new SelectedItemCollection(this);
			selectionController = new SelectedItemsController(this);
			SetDataAdapterInternal(CreateDataAdapter());
			itemStyle = new MapItemStyle();
			MapUtils.SetOwner(itemStyle, this);
			highlightedItemStyle = new MapItemStyle();
			MapUtils.SetOwner(highlightedItemStyle, this);
			selectedItemStyle = new MapItemStyle();
			MapUtils.SetOwner(selectedItemStyle, this);
		}
		#region IStyleOwner implementation
		void IMapStyleOwner.OnStyleChanged() {
			ResetItemsStyle(MapItemUpdateType.Style);
			InvalidateRender();
		}
		#endregion
		#region IHitTestableRegistrator members
		void IHitTestableRegistrator.RegisterItem(IHitTestableElement item) {
			if (OwnerHitTestableRegistrator != null) OwnerHitTestableRegistrator.RegisterItem(item);
		}
		void IHitTestableRegistrator.UnregisterItem(IHitTestableElement item) {
			if (OwnerHitTestableRegistrator != null) OwnerHitTestableRegistrator.UnregisterItem(item);
		}
		void IHitTestableRegistrator.UpdateItem(IHitTestableElement item) {
			if(OwnerHitTestableRegistrator != null) OwnerHitTestableRegistrator.UpdateItem(item);
		}
		void IHitTestableRegistrator.InvalidateItems(MapItemsLayerBase layer) {
			if(OwnerHitTestableRegistrator != null) OwnerHitTestableRegistrator.InvalidateItems(layer);
		}
		#endregion
		#region ILegendDataProvider implementation
		IList<MapLegendItemBase> ILegendDataProvider.CreateItems(MapLegendBase legend) {
			return CreateLegendItems(legend);
		}
		#endregion
		void SubscribeDataAdapterEvents() {
			if (this.dataAdapter != null) dataAdapter.DataChanged += OnDataAdapterDataChanged;
		}
		void UnsubscribeDataAdapterEvents() {
			if (this.dataAdapter != null) dataAdapter.DataChanged -= OnDataAdapterDataChanged;
		}
		void OnDataAdapterDataChanged(object sender, DataAdapterChangedEventArgs e) {
			ApplyDataChanged(e.UpdateType);
		}
		void NotifyDataChanged() {
			if (EventHandler != null) EventHandler.OnLayerChanged(this);
		}
		bool IsDataItemsInUpdate() {
			IBatchUpdateable batchUpdateable = DataItems as IBatchUpdateable;
			return batchUpdateable != null ? batchUpdateable.IsUpdateLocked : false;
		}
		void ApplyShapeTitleOptions() {
			EnumerateDataItems((d) => d.ApplyTitleOptions(), true);
			InvalidateRender();
		}
		void OnEnableSelectionChanged() {
			if (Map != null) Map.OperationHelper.ResetEnableSelection();
			if (!EnableSelection) SelectionController.ClearSelection();
			else SelectionController.ApplySelection();
		}
		void OnEnableHighlightingChanged() {
			if (Map != null) Map.OperationHelper.ResetEnableHighlighting();
		}
		void OnItemImageIndexChanged() {
			InnerMap innerMap = GetInnerMap();
			UpdateImageHolders(innerMap != null ? innerMap.ImageList : null);
		}
		void UpdateItems() {
			IHitTestableRegistrator registrator = this as IHitTestableRegistrator;
			EnumerateDataItems((d) => {
				d.OnLayerInitialized(this);
				registrator.RegisterItem(d);
				d.ResetStyle();
			}, false);
		}
		void ClearItemsHitTesting() {
			IHitTestableRegistrator registrator = OwnerHitTestableRegistrator;
			if (registrator == null) return;
			registrator.InvalidateItems(this);
			foreach (MapItem item in DataItems)
				registrator.UnregisterItem(item);
		}
		void InvalidateColors() {
			ApplyPredefinedColorSchema();
			UpdateLegends();			
		}
		void RecreateColorizerPredefinedColors() {
			if (Colorizer == null)
				return;
			ISkinProvider skinProvider = (Map != null && !string.IsNullOrEmpty(Map.ActualSkinName)) ? Map : null;
			PredefinedColorsColorizer pcc = Colorizer as PredefinedColorsColorizer;
			if (pcc != null) {
				ColorCollection colors = ColorizerPaletteHelper.GetPredefinedColors(pcc.PredefinedColorSchema, skinProvider);
				pcc.UpdatePredefinedColors(colors);
			}
		}
		void ResetColorizerColor() {
			lock (UpdateLocker) {
				foreach (MapItem item in DataItems) {
					ResetItemColor(item);
					IRenderItemContainer renderItemContainer = item as IRenderItemContainer;
					if(renderItemContainer != null)
						foreach(IRenderItem containerPart in renderItemContainer.Items)
							ResetItemColor(containerPart);
				}
			}
		}
		void ResetItemColor(object item) {
			IColorizerElement colorizerElement = item as IColorizerElement;
			if(colorizerElement != null)
				colorizerElement.ColorizerColor = Color.Empty;
		}
		void UpdateLegendItems() {
			if (Map != null) {
				Map.UpdateLegendsItems();
			}
		}
		void ReleaseItems(IMapDataAdapter dataAdapter) {
			foreach(MapItem item in dataAdapter.Items)
				MapUtils.SetOwner(item, null);
		}
		void ClearItemsHitTestGeometry() {
			IHitTestableRegistrator hitController = OwnerHitTestableRegistrator;
			if(hitController != null)
				hitController.InvalidateItems(this);
		}
		WinSvgPointConverterBase CreateSvgPointConverter(bool isGeoSystem) {
			if(isGeoSystem)
				return new WinSvgGeoPointConverter(Map.CoordinateSystem, this);
			return new WinSvgCartesianPointConverter(Map != null ? Map.CoordinateSystem : MapUtils.SvgDefaultCoordinateSystem, this);
		}
		ShpExporter CreateShpExporter() {
			return new ShpExporter();
		}
		KmlExporter<MapItem> CreateKmlExporter() {
			return new KmlExporter<MapItem>();
		}
		protected internal SvgExporter<MapItem> CreateSvgExporter(double scale) {
			bool isGeoMap = Map != null && Map.CoordinateSystem.PointType == CoordPointType.Geo;
			SvgExporter<MapItem> exporter = new SvgExporter<MapItem>();
			exporter.ScaleFactor = scale <= 0.0 ? 1.0 : scale;
			exporter.PointConverter = CreateSvgPointConverter(isGeoMap);
			exporter.CanvasSize = isGeoMap ? new SvgSize(Map.InitialMapSize.Width, Map.InitialMapSize.Height) : new SvgSize(BoundingRect.Width, BoundingRect.Height);
			return exporter;
		}
		protected abstract IMapDataAdapter CreateDataAdapter();
		protected virtual void EnumerateDataItems(Action<MapItem> action, bool suspendUpdate) {
			if (DataAdapter != null) {
				IBatchUpdateable batchUpdateable = DataAdapter.Items as IBatchUpdateable;
				bool doUpdate = suspendUpdate && batchUpdateable != null;
				if (doUpdate) batchUpdateable.BeginUpdate();
				try {
					IEnumerable<MapItem> items = DataAdapter.Items;
					foreach (MapItem item in items) {
						action(item);
					}
				} finally {
					if (doUpdate) batchUpdateable.EndUpdate();
				}
			}
		}
		protected virtual LayerColoredSkinElementCache CreateColoredSkinElementCache() {
			return new LayerColoredSkinElementCache();
		}
		protected virtual void ApplyDataChanged(MapUpdateType updateType) {
			if ((updateType & MapUpdateType.Data) == MapUpdateType.Data)
				NotifyDataChanged();
			if ((updateType & MapUpdateType.Style) == MapUpdateType.Style)
				ResetItemsStyle(MapItemUpdateType.Layout);
			if ((updateType & MapUpdateType.ViewInfo) == MapUpdateType.ViewInfo) {
				IUIThreadRunner runner = Map as IUIThreadRunner;
				if (runner != null && runner.AllowInvoke)
					runner.BeginInvoke(() => {
						InvalidateColors();
					});
			}
			if ((updateType & MapUpdateType.Render) == MapUpdateType.Render)
				InvalidateRender();
		}
		protected virtual IList<MapLegendItemBase> CreateLegendItems(MapLegendBase legend) {
			ILegendDataProvider provider = Colorizer as ILegendDataProvider;
			return provider != null ? provider.CreateItems(legend) : new List<MapLegendItemBase>();
		}
		protected override DataLoadedEventArgs CreateDataLoadedEventArgs() {
			return new MapItemsLoadedEventArgs(DataItems);
		}
		protected override object GetUpdateLocker() {
			ILockableObject lockable = DataAdapter as ILockableObject;
			return lockable != null ? lockable.UpdateLocker : base.GetUpdateLocker();
		}
		protected override bool CanInvalidateRender() {
			return base.CanInvalidateRender() && !IsDataItemsInUpdate();
		}
		protected override MapColorizer GetColorizer(IRenderItem item) {
			return Colorizer;
		}
		protected bool CanRenderItems() {
			if (View == null || !IsValidData)
				return false;
			ISupportUnitConverter support = View as ISupportUnitConverter;
			return support != null ? DataAdapter.IsCSCompatibleTo(support.UnitConverter.CoordinateSystem) : false;
		}
		protected override IEnumerable<IRenderItem> GetRenderItems() {
			if (CanRenderItems()) {
				foreach (MapItem item in DataItems)
					yield return (IRenderItem)item;
			}
		}
		protected override void DisposeOverride() {
			if (coloredSkinElementCache != null) {
				coloredSkinElementCache.Dispose();
				coloredSkinElementCache = null;
			}
			ReleaseDataAdapter();
			base.DisposeOverride();
		}
		protected void ReleaseDataAdapter() {
			if(dataAdapter != null) {
				UnsubscribeDataAdapterEvents();
				dataAdapter.SetLayer(null);
				ReleaseItems(dataAdapter);
				MapUtils.DisposeObject(dataAdapter);
				dataAdapter = null;
			}
		}
		protected override void OwnerChanged() {
			base.OwnerChanged();
			if (Map != null) {
				UpdateItems();
			}
			else {
				if (coloredSkinElementCache != null) {
					coloredSkinElementCache.Reset();
				}
			}
		}
		protected override void ChangeOwner(object newOwner) {
			ClearItemsHitTesting();
			base.ChangeOwner(newOwner);
			if (Map != null)
				Map.CoordinateSystem.SetNeedUpdateBoundingBox(true);
		}
		protected void SetDataAdapterInternal(IMapDataAdapter dataAdapter) {
			UnsubscribeDataAdapterEvents();
			if(dataAdapter != null) dataAdapter.SetLayer(null);
			this.dataAdapter = dataAdapter;
			if(dataAdapter != null) dataAdapter.SetLayer(this);
			SubscribeDataAdapterEvents();
			ApplyDataChanged(MapUpdateType.Data);
			ApplyShapeTitleOptions();
			UpdateBoundingRect();
			UpdateItems();
			InvalidateRender();
		}
		internal IRenderItemStyle RaiseDrawMapItem(MapItem item) {
			return Map != null ? Map.OnDrawMapItem(item) : null;
		}
		internal bool RaiseExportMapItem(MapItem item) {
			return Map != null ? Map.OnExportMapItem(item) : true;
		}
		internal override void AfterRender() {
			if (ShouldRaiseDataLoadedEvent && NativeBoundsAreValid) {
				RaiseDataLoaded();
				ShouldRaiseDataLoadedEvent = false;
			}
		}
		internal bool RaiseItemSelectionChanging(IEnumerable<object> newValue) {
			MapSelectionChangingEventArgs args = new MapSelectionChangingEventArgs(newValue);
			if (Map != null) Map.OnSelectionChanging(args);
			newValue = args.Selection;
			return !args.Cancel;
		}
		internal void RaiseItemSelectionChanged() {
			if (Map != null) Map.OnSelectionChanged();
		}
		internal void UpdateImageHolders(object imageList) {
			foreach (MapItem item in DataItems)
				MapUtils.UpdateImageContainer(item as IImageContainer, imageList);
		}
		internal void UpdateImageHolder(IImageContainer imageHolder) {
			if (Map != null)
				imageHolder.UpdateImage(Map.ImageList);
		}
		internal void ResetItemsStyle(MapItemUpdateType updateType) {
			EnumerateDataItems((d) => d.StyleChanged(updateType), false);
		}
		internal void ResetColoredSkinElementCache() {
			if (coloredSkinElementCache != null)
				coloredSkinElementCache.Reset();
		}
		internal void UpdateItemsLayout() {
			EnumerateDataItems(d => d.RegisterLayoutUpdate(), true);
		}	   
		protected internal override Size GetMapBaseSizeInPixels() {
			return InitialMapSizeInternal;
		}
		protected internal override MapSize GetMapSizeInPixels(double zoomLevel) {
			if (zoomLevel < 1.0)
				return new MapSize(zoomLevel * InitialMapSizeInternal.Width, zoomLevel * InitialMapSizeInternal.Height);
			double level = Math.Max(0.0, zoomLevel - 1.0);
			double coeff = Math.Pow(2.0, level);
			return new MapSize(coeff * InitialMapSizeInternal.Width, coeff * InitialMapSizeInternal.Height);
		}
		protected internal void OnColorizerChanged() {
			InvalidateColors();
		}
		protected internal void InvalidateViewInfo() {
			if(Map != null)
				Map.InvalidateViewInfo();
		}
		protected internal void UpdateBoundingRect() {
			 NativeBoundsAreValid = false;
		}
		protected internal void UpdateLegends() {
			UpdateLegendItems();
			InvalidateViewInfo();
		}
		protected internal virtual void EnsureBoundingRect() {
			NativeBoundsAreValid = true;
		}
		protected internal override void ApplyPredefinedColorSchema() {
			base.ApplyPredefinedColorSchema();
			RecreateColorizerPredefinedColors();
			ResetColorizerColor();
		}
		protected internal override void ViewportUpdated() {
			base.ViewportUpdated();
			lock(UpdateLocker) {
				if(DataAdapter != null && View != null && !View.AnimationInProgress)
					DataAdapter.OnViewportUpdated(GetViewport());
				ClearItemsHitTestGeometry();
				EnumerateDataItems((d) => d.ReleaseHitTestGeometry(), false);
			}
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
