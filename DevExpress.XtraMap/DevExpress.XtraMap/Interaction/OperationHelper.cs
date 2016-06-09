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
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraMap.Drawing;
namespace DevExpress.XtraMap.Native {
	public class MapOperationHelper {
		InnerMap map;
		bool? enableHighlighting;
		bool? enableSelection;
		bool? errorPanelVisible;
		bool? miniMapErrorPanelVisible;
		protected InnerMap Map { get { return map; } }
		protected NavigationPanelOptions NavigationPanelOptions { get { return Map.NavigationPanelOptions; } }
		protected internal bool? EnableHighlighting {
			get {
				if (!enableHighlighting.HasValue)
					enableHighlighting = CalculateEnableHighlighting();
				return enableHighlighting;
			}
		}
		protected internal bool? EnableSelection {
			get {
				if(!enableSelection.HasValue)
					enableSelection = CalculateEnableSelection();
				return enableSelection;
			}
		}
		public MapOperationHelper(InnerMap map) {
			Guard.ArgumentNotNull(map, "map");
			this.map = map;
		}
		internal bool IsLayersVisible() {
			return (Map.ActualLayers != null && Map.ActualLayers.Count > 0); 
		}
		bool CalculateEnableHighlighting() {
			int count = Map.ActualLayers.Count;
			for (int i = 0; i < count; i++) {
				MapItemsLayerBase itemsLayer = Map.ActualLayers[i] as MapItemsLayerBase;
				if (itemsLayer != null && itemsLayer.EnableHighlighting) {
					return true;
				}
			}
			return false;
		}
		bool CalculateEnableSelection() {
			int count = Map.ActualLayers.Count;
			for(int i = 0; i < count; i++) {
				MapItemsLayerBase itemsLayer = Map.ActualLayers[i] as MapItemsLayerBase;
				if(itemsLayer != null && itemsLayer.EnableSelection) {
					return true;
				}
			}
			return false;
		}
		bool CalculateErrorPanelVisibility(SortedLayerCollection layers) {
			int count = layers.Count;
			for (int i = 0; i < count; i++) {
				ImageTilesLayer tilesLayer = layers[i] as ImageTilesLayer;
				if(tilesLayer != null && tilesLayer.Visible) {
					MapDataProviderBase dataProvider = tilesLayer.DataProvider;
					if (dataProvider != null && dataProvider.ShouldShowInvalidKeyMessage)
						return true;
				}
			}
			return false;
		}
		bool IsOwnedControlAssigned() { 
			IMapControl  owner = Map.OwnedControl;
			return owner != null && !EmptyInnerMapOwner.Instance.Equals(owner);
		}
		public bool CanShowNavigationPanel() {
			return NavigationPanelOptions.Visible;
		}
		public bool CanShowNavigationPanelScale() {
			return Map.IsDesignMode || (CanShowNavigationPanel() && IsLayersVisible());
		}
		public bool CanShowMilesScale() {
			return CanShowNavigationPanelScale() && NavigationPanelOptions.ShowMilesScale;
		}
		public bool CanShowKilometersScale() {
			return CanShowNavigationPanelScale() && NavigationPanelOptions.ShowKilometersScale;
		}
		public bool CanShowCoordinates() {
			return CanShowNavigationPanel() && NavigationPanelOptions.ShowCoordinates;
		}
		public bool CanShowZoomTrackbar() {
			return CanShowNavigationPanel() && NavigationPanelOptions.ShowZoomTrackbar && (CanZoom() || Map.IsDesignMode);
		}
		public bool CanUpdateNavigationPanelScale() {
			return CanShowZoomTrackbar() || CanShowMilesScale() || CanShowKilometersScale();
		}
		public bool CanShowScrollButtons() {
			return CanShowNavigationPanel() && NavigationPanelOptions.ShowScrollButtons && (CanScroll() || Map.IsDesignMode);
		}
		public bool CanAnimate() {
			return Map.EnableAnimation && IsLayersVisible() && IsOwnedControlAssigned();
		}
		public bool CanZoom() {
			return Map.EnableZooming && IsLayersVisible();
		}
		public bool CanScroll() {
			return Map.EnableScrolling && IsLayersVisible();
		}
		public bool CanChangeSelection() {
			return EnableSelection.Value;
		}
		public bool CanHighlight() {
			return EnableHighlighting.Value;
		}
		public bool CanShowErrorPanel() {
			if (!errorPanelVisible.HasValue)
				errorPanelVisible = CalculateErrorPanelVisibility(Map.ActualLayers);
			return errorPanelVisible.Value;
		}
		public bool CanShowMiniMapErrorPanel() {
			if (!CanShowMiniMap())
				return false;
			if (!miniMapErrorPanelVisible.HasValue)
				miniMapErrorPanelVisible = CalculateErrorPanelVisibility(Map.MiniMap.ActualLayers);
			return miniMapErrorPanelVisible.Value;
		}
		public bool CanShowLegend(MapLegendBase legend) {
			if(legend == null || !legend.ActualVisible)
				return false;
			return legend.CanDisplayItems();
		}
		public bool CanShowOverlay(MapOverlay overlay) {
			return overlay != null && overlay.Visible;
		}
		public bool CanShowImageTilesLayer(ProjectionBase tilesProviderProjection, Size baseMapSize) {
			GeoMapCoordinateSystem coordSystem = Map.CoordinateSystem as GeoMapCoordinateSystem;
			bool projectionsEqual = coordSystem != null ? tilesProviderProjection == coordSystem.Projection : false;
			bool mapSizesEqual = baseMapSize == Map.InitialMapSize;
			return projectionsEqual && mapSizesEqual;
		}
		public bool CanShowMiniMap() {
			return Map.MiniMap != null && Map.MiniMap.Visible;
		}
		internal void ResetEnableSelection() {
			enableSelection = null;
		}
		internal void ResetEnableHighlighting() {
			enableHighlighting = null;
		}
		internal void ResetErrorPanelVisibility() {
			errorPanelVisible = null;
		}
		internal void ResetMiniMapErrorPanelVisibility() {
			miniMapErrorPanelVisible = null;
		}
		internal bool CanShowToolTips() {
			return Map.ShowToolTips;
		}
		internal bool CanSelectedByRegion() {
			return  Map.SelectionMode == ElementSelectionMode.Extended || Map.SelectionMode == ElementSelectionMode.Multiple;
		}
		internal bool IsSearchActive() {
			return Map.InteractionController.IsSearchPanelVisible;
		}
		internal bool CanShowSearchPanel(InformationLayer layer) {
			if(layer == null) return false;
			if(!layer.Visible) return false;
			BingSearchDataProvider searchProvider = layer.DataProvider as BingSearchDataProvider;
			return searchProvider != null ? searchProvider.ShowSearchPanel : false;
		}
		internal bool CanUseDirectX() {
			if(((IOwnedElement)Map).Owner == null)
				return false;
			return !Map.OwnedControl.IsDesignMode && D3DRenderer.AllowUseDirectX;
		}
		internal bool CanUseGestures() {
			if(Map.RenderController == null || !Map.RenderController.ViewInfo.IsReady)
				return false;
			if(Map.MouseHandler.CurrentUiHitInfo != null && Map.MouseHandler.CurrentUiHitInfo.HitElement != MapHitUiElementType.None)
				return false;
			return true;
		}
		internal bool CanClearLayerItems(MapItemsLayerBase layerToSkip, MapItemsLayerBase layer) {
			return layer != null && layer != layerToSkip && layer.Visible;
		}
	}
}
