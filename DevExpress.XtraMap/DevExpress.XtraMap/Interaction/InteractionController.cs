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
using DevExpress.Utils;
using DevExpress.XtraMap.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraMap.Native {
	public class InteractionController : MapDisposableObject {
		const int GridStep = 3;
		InnerMap map;
		IInteractiveElement highlightedElement;
		MapSearchController searchController;
		bool doubleClicked = false;
		protected internal InnerMap Map { get { return map; } }
		public bool IsSearchPanelVisible { get { return searchController.IsSearchPanelVisible; } }
		public InteractionController(InnerMap map) {
			Guard.ArgumentNotNull(map, "map");
			this.map = map;
			searchController = new MapSearchController(this);
		}
		void SetElementHighlighted(IInteractiveElement element, bool value) {
			if(element == null)
				return;
			if(value) {
				MapItem item = element as MapItem;
				MapItemHighlightingEventArgs args = new MapItemHighlightingEventArgs(item);
				Map.RaiseMapItemHighlighting(args);
				if(args.Cancel)
					return;
			}
			element.IsHighlighted = value;
		}
		IInteractiveElement FindTopElement(object[] elements, Predicate<IInteractiveElement> match) {
			for(int i = elements.Length - 1; i >= 0; i--) {
				IInteractiveElement element = ObtainInteractiveElement(elements[i]);
				if(match(element))
					return element;
			}
			return null;
		}
		IList<IInteractiveElement> PopulateInteractiveElements(object[] elements, Predicate<IInteractiveElement> match) {
			IList<IInteractiveElement> result = new List<IInteractiveElement>();
			for(int i = 0; i < elements.Length; i++) {
				IInteractiveElement element = ObtainInteractiveElement(elements[i]);
				if(match(element))
					result.Add(element);
			}
			return result;
		}
		void UpdateInteractiveElementSelection(IInteractiveElement element) {
			if(element == null || !element.EnableSelection)
				return;
			MapItem item = element as MapItem;
			MapItemsLayerBase layer = item != null ? item.Layer as MapItemsLayerBase : null;
			if(layer != null) {
				SelectionAction action = SelectionHelper.CalculateSelectionAction(map.SelectionMode, map.MouseHandler.KeyModifiers, element.IsSelected, layer.EnableSelection);
				UpdateSelection(layer, item, action);
			}
		}
		void ReplaceSelection(MapItemsLayerBase layer, object objectToSelect) {
			IList<object> objectsToSelect = new List<object>() { objectToSelect };
			MapSelectionChangingEventArgs args = new MapSelectionChangingEventArgs(objectsToSelect);
			NotifySelectionChanging(args);
			if(args.Cancel)
				return;
			objectsToSelect = args.Selection;
			ClearAllSelectedItems(layer, false);
			layer.SelectionController.InternalSetSelectedItems(objectsToSelect);
			NotifySelectionChanged();
		}
		object GetActualObjectToSelect(VectorItemsLayer vectorLayer, object item) {
			return vectorLayer != null ? vectorLayer.GetItemSourceObject(item as MapItem) : item;
		}
		void ReplaceHighlightedElement(IInteractiveElement newElement) {
			if(Object.Equals(highlightedElement, newElement))
				return;
			SetElementHighlighted(highlightedElement, false);
			SetElementHighlighted(newElement, true);
			highlightedElement = newElement;
		}
		SelectionAction CalculateSelectionAction(MapItemsLayerBase layer) {
			return layer.EnableSelection ? SelectionAction.Add : SelectionAction.None;
		}
		void HandleClickByLayers(Point location) {
			lock(Map.ActualLayers) {
				foreach(LayerBase item in Map.ActualLayers) {
					IMapClickHandler layer = item as IMapClickHandler;
					if (layer != null) layer.OnPointClick(new MapPoint(location.X, location.Y), new GeoPoint(0, 0));
				}
			}
		}
		bool CanChangeSelection() {
			return map.SelectionMode != ElementSelectionMode.None && map.OperationHelper.CanChangeSelection();
		}
		void UpdateSelectionByHitTest(MapHitInfo hitInfo) {
			if(!CanChangeSelection())
				return;
			IInteractiveElement element = FindTopElement(hitInfo.HitObjects, (s) => { return s != null; });
			if(element != null)
				UpdateInteractiveElementSelection(element);
			else {
				if(map.MouseHandler.KeyModifiers == Keys.None)
					ClearAllSelectedItems(null);
			}
		}
		bool IsGeometriesIntersectionRegion(MapRect regionRect, IList<IList<MapUnit>> geometries) {
			foreach(IList<MapUnit> geometry in geometries)
				if(IsGeometryIntersections(regionRect, geometry))
					return true;
			return false;
		}
		bool IsRegionContainPoint(MapRect regionRect, IList<MapUnit> unitPoints) {
			foreach(MapUnit point in unitPoints)
				if(regionRect.Contains(new MapPoint(point.X, point.Y)))
					return true;
			return false;
		}
		bool IsHorizontalIntersection(double minX, double maxX, double y, MapRect regionRect) {
			return MathUtils.ValueBetween(y, regionRect.Top, regionRect.Bottom) && (MathUtils.ValueBetween(regionRect.Left, minX, maxX) || MathUtils.ValueBetween(regionRect.Right, minX, maxX));
		}
		bool IsVerticalIntersection(double minY, double maxY, double x, MapRect regionRect) {
			return MathUtils.ValueBetween(x, regionRect.Left, regionRect.Right) && (MathUtils.ValueBetween(regionRect.Top, minY, maxY) || MathUtils.ValueBetween(regionRect.Bottom, minY, maxY));
		}
		bool IsSlopingIntersection(double dx, double dy, MapUnit minPoint, MapUnit maxPoint, MapUnit startPoint, MapRect regionRect) {
			double k = dy / dx;
			double b = startPoint.Y - startPoint.X * k;
			MapUnit min = new MapUnit(Math.Max(minPoint.X, regionRect.Left), Math.Max(minPoint.Y, regionRect.Top));
			MapUnit max = new MapUnit(Math.Min(maxPoint.X, regionRect.Right), Math.Min(maxPoint.Y, regionRect.Bottom));
			if(MathUtils.ValueBetween((regionRect.Top - b) / k, min.X, max.X))
				return true;
			if(MathUtils.ValueBetween((regionRect.Bottom - b) / k, min.X, max.X))
				return true;
			if(MathUtils.ValueBetween(regionRect.Left * k + b, min.Y, max.Y))
				return true;
			if(MathUtils.ValueBetween(regionRect.Right * k + b, min.Y, max.Y))
				return true;
			return false;
		}
		bool IsLineIntersectionRegion(MapRect regionRect, MapUnit point1, MapUnit point2) {
			double dx = point2.X - point1.X;
			double dy = point2.Y - point1.Y;
			double minX = Math.Min(point1.X, point2.X);
			double maxX = Math.Max(point1.X, point2.X);
			double minY = Math.Min(point1.Y, point2.Y);
			double maxY = Math.Max(point1.Y, point2.Y);
			MapUnit minPoint = new MapUnit(minX, minY);
			MapUnit maxPoint = new MapUnit(maxX, maxY);
			if(dy == 0.0 && IsHorizontalIntersection(minX, maxX, point1.Y, regionRect) || dx == 0.0 && IsVerticalIntersection(minY, maxY, point1.X, regionRect))
				return true;
			if(dx == 0.0 || dy == 0.0)
				return false;
			if(IsSlopingIntersection(dx, dy, minPoint, maxPoint, point1, regionRect))
				return true;
			return false;
		}
		bool IsGeometryIntersections(MapRect regionRect, IList<MapUnit> geometryPoints) {
			int length = geometryPoints.Count;
			if(length <= 1)
				return false;
			for(int i = 0; i < length - 1; i++)
				if(IsLineIntersectionRegion(regionRect, geometryPoints[i], geometryPoints[i + 1]))
					return true;
			return false;
		}
		IList<MapUnit> GetItemPoints(MapItem item) {
			return item.GetUnitPoints();
		}
		void AppendCenterItems(ISet<object> items, RegionRange regionRange) {
			IEnumerable<MapItem> hitTestedItems = map.HitTestController.GetItemsInCentralRegions(regionRange);
			items.UnionWith(hitTestedItems);
		}
		void AppendBorderItems(ISet<object> items, RegionRange regionRange, MapRect regionInUnits) {
			MapRect scaledRegionInUnits = regionInUnits * MapItem.RenderScaleFactor;
			IEnumerable<MapItem> hitTestedItems = map.HitTestController.GetItemsInBorderRegions(regionRange);
			foreach(MapItem item in hitTestedItems) {
				if(!MapRect.IsIntersected(regionInUnits, item.Bounds))
					continue;
				IList<MapUnit> unitPoints = GetItemPoints(item);
				if(IsRegionContainPoint(scaledRegionInUnits, unitPoints)) {
					items.Add(item);
					continue;
				}
				IList<IList<MapUnit>> segmentGeometries = item.GetSegmentGeometries(unitPoints);
				if(IsGeometriesIntersectionRegion(scaledRegionInUnits, segmentGeometries))
					items.Add(item);
			}
		}
		void AppendCornerItems(ISet<object> items, RegionRange regionRange, Rectangle regionInPixels) {
			List<Point> screenPoints = new List<Point>() { new Point(regionInPixels.Left, regionInPixels.Top), new Point(regionInPixels.Left, regionInPixels.Bottom), new Point(regionInPixels.Right, regionInPixels.Top), new Point(regionInPixels.Right, regionInPixels.Bottom) };
			foreach(Point point in screenPoints) {
				MapHitInfo hitInfo = map.CalcHitInfo(point);
				IList<IInteractiveElement> elements = PopulateInteractiveElements(hitInfo.HitObjects, (s) => { return s != null && s.EnableSelection; });
				foreach(IInteractiveElement element in elements)
					items.Add(element);
			}
		}
		void AppendLocatableItems(ISet<object> items, Rectangle regionInPixels) {
			IEnumerable<MapItem> hitTestedItems = map.HitTestController.GetLocatableItems(regionInPixels);
			items.UnionWith(hitTestedItems);
		}
		protected void NotifySelectionChanging(MapSelectionChangingEventArgs args) {
			Map.OnSelectionChanging(args);
		}
		protected void NotifySelectionChanged() {
			Map.OnSelectionChanged();
		}
		protected bool HandleMapItemClick(MapHitInfo hitInfo, MouseEventArgs e) {
			if(hitInfo.HitObjects.Length > 0) {
				MapItem hitMapItem = hitInfo.GetHitMapItem();
				if(hitMapItem != null) {
					MapPointer pointer = hitMapItem as MapPointer;
					if(pointer != null && Map.RaiseHyperlinkClick(e, pointer))
						return true;
					return RaiseMapItemClick(e, hitMapItem);
				}
			}
			return false;
		}
		bool RaiseMapItemClick(MouseEventArgs e, MapItem hitMapItem) {
			if(e.Clicks == 2 && !doubleClicked) {
				doubleClicked = true;
				return Map.RaiseMapItemDoubleClick(e, hitMapItem);
			}
			if(!doubleClicked)
				return Map.RaiseMapItemClick(e, hitMapItem);
			doubleClicked = false;
			return false;
		}
		protected override void DisposeOverride() {
			if(searchController != null) {
				searchController.Dispose();
				searchController = null;
			}
		}
		internal void UpdateSelection(MapItemsLayerBase layer, MapItem item, SelectionAction action) {
			if(action == SelectionAction.None)
				return;
			object objectToSelect = GetActualObjectToSelect(layer as VectorItemsLayer, item);
			switch(action) {
				case SelectionAction.None:
					return;
				case SelectionAction.ReplaceAll: {
						ReplaceSelection(layer, objectToSelect);
						break;
					}
				case SelectionAction.Add: {
						layer.SelectionController.AddSelectedItem(objectToSelect);
						break;
					}
				case SelectionAction.Remove: {
						layer.SelectionController.RemoveSelectedItem(objectToSelect);
						break;
					}
				case SelectionAction.Clear: {
						ClearAllSelectedItems(null);
						break;
					}
			}
		}
		internal IInteractiveElement ObtainInteractiveElement(object element) {
			IInteractiveElement result = element as IInteractiveElement;
			if(result != null) return result;
			IInteractiveElementProvider provider = element as IInteractiveElementProvider;
			return provider != null ? provider.GetInteractiveElement() : null;
		}
		internal void ClearAllSelectedItems(MapItemsLayerBase layerToSkip) {
			ClearAllSelectedItems(layerToSkip, true);
		}
		internal void ClearAllSelectedItems(MapItemsLayerBase layerToSkip, bool notify) {
			foreach(LayerBase item in map.ActualLayers) {
				MapItemsLayerBase layer = item as MapItemsLayerBase;
				if(map.OperationHelper.CanClearLayerItems(layerToSkip, layer))
					if(notify)
						layer.SelectionController.ClearSelectedItems();
					else
						layer.SelectionController.InternalClearSelectedItems();
			}
		}
		internal Rectangle CalculateSearchPanelBounds(Rectangle contentBounds) {
			return searchController.CalculateSearchPanelBounds(contentBounds);
		}
		internal void SearchPanelReset() {
			searchController.ResetSearchPanel();
		}
		internal void UpdateSearchPanel() {
			searchController.UpdateSearchPanel();
		}
		public void HandleMouseMove(MouseEventArgs e, MapHitInfo hitInfo) {
			HandleMapItemMouseMove(e, hitInfo);
			if(!map.OperationHelper.CanHighlight())
				return;
			IInteractiveElement el = FindTopElement(hitInfo.HitObjects, (s) => { return s != null && s.EnableHighlighting; });
			ReplaceHighlightedElement(el);
		}
		void HandleMapItemMouseMove(MouseEventArgs e, MapHitInfo hitInfo) {
			if(hitInfo.HitObjects.Length != 1)
				return;
			MapItem item = hitInfo.HitObjects[0] as MapItem;
			if (item != null) item.HandleMapItemMouseMove(e);
		}
		bool RaiseSelectionChanging(List<object> items) {
			MapSelectionChangingEventArgs args = new MapSelectionChangingEventArgs(items.ToArray());
			Map.OnSelectionChanging(args);
			items.Clear();
			items.AddRange(args.Selection);
			return !args.Cancel;
		}
		void RaiseSelectionChanged() {
			Map.OnSelectionChanged();
		}
		void SetShouldProcessEvents(bool shouldProcessEvents) {
			foreach (LayerBase layer in Map.ActualLayers) {
				VectorItemsLayer itemsLayer = layer as VectorItemsLayer;
				if (itemsLayer != null)
					itemsLayer.SelectionController.ShouldProcessEvents = shouldProcessEvents;
			}
		}
		void ProcessSelection(List<object> selectedItems) {
			if (!RaiseSelectionChanging(selectedItems))
				return;
			SetShouldProcessEvents(false);
			ClearAllSelectedItems(null);
			foreach (MapItem item in selectedItems) {
				MapItemsLayerBase layer = item.Layer;
				UpdateSelection(layer, item, CalculateSelectionAction(layer));
			}
			SetShouldProcessEvents(true);
			RaiseSelectionChanged();
		}
		MapRect ScreenToMapRect(Rectangle screenRect) {
			MapUnit leftTop = Map.ScreenPointToMapUnit(new MapPoint(screenRect.Left, screenRect.Top));
			MapUnit rightBottom = Map.ScreenPointToMapUnit(new MapPoint(screenRect.Right, screenRect.Bottom));
			return MapRect.FromLTRB(leftTop.X, leftTop.Y, rightBottom.X, rightBottom.Y);
		}
		public void ResetHighlightedElement() {
			SetElementHighlighted(highlightedElement, false);
		}
		public void HandleMouseUp(MouseEventArgs e, IMapUiHitInfo uiHitInfo, MapHitInfo hitInfo) {
			if(uiHitInfo.HitElement != MapHitUiElementType.None)
				return;
			UpdateSelectionByHitTest(hitInfo);
			bool handled = HandleMapItemClick(hitInfo, e);
			if(!handled && e.Button == MouseButtons.Left)
				HandleClickByLayers(e.Location);
		}
		public List<object> GetItemsByRegion(Rectangle regionInPixel) {
			ISet<object> selectedItems = new HashSet<object>();
			MapRect regionInUnits = ScreenToMapRect(regionInPixel);
			RegionRange regionRange = map.HitTestController.CreateRegionRange(regionInUnits);
			map.HitTestController.RecreateRegionsIfNecessary();
			AppendCenterItems(selectedItems, regionRange);
			AppendBorderItems(selectedItems, regionRange, regionInUnits);
			AppendCornerItems(selectedItems, regionRange, regionInPixel);
			AppendLocatableItems(selectedItems, regionInPixel);
			return new List<object>(selectedItems);
		}
		public void SelectItemsByRegion(Rectangle region) {
			if(map.SelectionMode == ElementSelectionMode.None)
				return;
			List<object> selectedItems = GetItemsByRegion(region); 
			ProcessSelection(selectedItems);
		}
		public void NotifyLookAndFeelChanged() {
			searchController.UpdatePanelStyle(Map.IsSkinActive ? Map : null);
		}
		public void SetClientSize(Rectangle contentRectangle) {
			searchController.LayoutSearchPanel(contentRectangle);
		}
	}
}
