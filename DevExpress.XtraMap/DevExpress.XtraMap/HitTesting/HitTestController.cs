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
using DevExpress.Map.Native;
using DevExpress.Utils;
using DevExpress.XtraMap.Drawing;
namespace DevExpress.XtraMap.Native {
	internal sealed class HitTestItemKeyGenerator {
		long sequence;
		public long GenerateKey() {
			return ++sequence;
		}
	}
	public class MapHitTestController : MapDisposableObject, IHitTestableRegistrator {
		const int HitTestableRegionsCountV = 16;
		const int HitTestableRegionsCountH = 16;
		const double RegionStepV = 1.0 / (double)HitTestableRegionsCountV;
		const double RegionStepH = 1.0 / (double)HitTestableRegionsCountH;
		static int GetRegionIndex(double boundValue, double regionStep, int totalRegionsCount) {
			return Math.Min((int)Math.Floor(boundValue / regionStep), totalRegionsCount - 1);
		}
		static int GetVRegionIndex(double yPos) {
			return Math.Max(0, Math.Min(HitTestableRegionsCountV, GetRegionIndex(yPos, RegionStepV, HitTestableRegionsCountV)));
		}
		static int GetHRegionIndex(double xPos) {
			return Math.Max(0, Math.Min(HitTestableRegionsCountH, GetRegionIndex(xPos, RegionStepH, HitTestableRegionsCountH)));
		}
		static Point CalculateRegion(MapUnit unscaledMapUnit) {
			return new Point(GetHRegionIndex(unscaledMapUnit.X), GetVRegionIndex(unscaledMapUnit.Y));
		}
		readonly InnerMap map;
		readonly HitTestItemPool hitTestGeometryPool;
		MapHitTestableRegion[,] hitTestableRegions;
		bool shouldRecreateRegions = false;
		List<IHitTestableElement> locatableElements = new List<IHitTestableElement>();
		HitTestItemKeyGenerator keyGenerator = new HitTestItemKeyGenerator();
		protected internal bool ShouldRecreateRegions { get { return shouldRecreateRegions; } }
		protected internal List<IHitTestableElement> LocatableElements { get { return locatableElements; } }
		protected InnerMap Map { get { return map; } }
		protected SortedLayerCollection Layers { get { return Map.ActualLayers; } }
		protected MapUnitConverter UnitConverter { get { return Map.UnitConverter; } }
		protected HitTestItemPool HitTestGeometryPool { get { return hitTestGeometryPool; } }
		internal HitTestItemKeyGenerator KeyGenerator { get { return keyGenerator;  } }
		#region IHitTestableRegistrator implementation
		void IHitTestableRegistrator.RegisterItem(IHitTestableElement item) {
			SetElementKey(item, KeyGenerator.GenerateKey());
			UpdateItemInRegions(item);
		}
		void IHitTestableRegistrator.UnregisterItem(IHitTestableElement item) {
			RemoveMapItemFromRegions(item);
		}
		void IHitTestableRegistrator.UpdateItem(IHitTestableElement item) {
			UpdateItemInRegions(item);
		}
		void IHitTestableRegistrator.InvalidateItems(MapItemsLayerBase layer) {
			ClearHitGemetryPool(layer);
		}
		#endregion
		public MapHitTestController(InnerMap map) {
			Guard.ArgumentNotNull(map, "map");
			this.map = map;
			this.hitTestGeometryPool = new HitTestItemPool();
			InitializeHitTestableRegions();
		}
		void InitializeHitTestableRegions() {
			hitTestableRegions = new MapHitTestableRegion[HitTestableRegionsCountH, HitTestableRegionsCountV];
			for (int vPos = 0; vPos < HitTestableRegionsCountV; vPos++)
				for (int hPos = 0; hPos < HitTestableRegionsCountH; hPos++)
					hitTestableRegions[hPos, vPos] = new MapHitTestableRegion();
		}
		void PerformAddMapItemToRegion(MapHitTestableRegion region, IHitTestableElement hitTestable) {
			region.MapItems.Add(hitTestable);
		}
		void PerformRemoveMapItemFromRegion(MapHitTestableRegion region, IHitTestableElement hitTestable) {
			region.MapItems.Remove(hitTestable);
		}
		void ForeachRegionInRange(RegionRange range, IHitTestableElement hitTestable, Action<MapHitTestableRegion, IHitTestableElement> action) {
			for (int hPos = range.Left; hPos <= range.Right; hPos++)
				for(int vPos = range.Top; vPos <= range.Bottom; vPos++)
					action(hitTestableRegions[hPos, vPos], hitTestable);
		}
		IHitTestableElement[] ExtractHitTestableElements(IHitTestableElement hitTestable) {
			IHitTestableOwner compositeHitTestable = hitTestable as IHitTestableOwner;
			return compositeHitTestable != null ? compositeHitTestable.Elements : new IHitTestableElement[] { hitTestable };
		}
		void AddMapItemToRegions(IHitTestableElement hitTestable) {
			IHitTestableElement[] elements = ExtractHitTestableElements(hitTestable);
			foreach (IHitTestableElement element in elements) {
				MapRect unitBounds = element.UnitBounds;
				if(unitBounds != MapRect.Empty && !(element is ILocatableRenderItem)) {
					RegionRange range = CreateRegionRange(element.UnitBounds);
					ForeachRegionInRange(range, element, PerformAddMapItemToRegion);
					element.RegionRange = range;
				} 
				else
					LocatableElements.Add(element);
			}
		}
		void RemoveMapItemFromRegions(IHitTestableElement hitTestable) {
			IHitTestableElement[] elements = ExtractHitTestableElements(hitTestable);
			foreach (IHitTestableElement element in elements) {
				RegionRange range = element.RegionRange;
				if (range != RegionRange.Empty) {
					ForeachRegionInRange(range, element, PerformRemoveMapItemFromRegion);
					element.RegionRange = RegionRange.Empty;
				}
			}
		}
		IHitTestableElement[] HitTestByRegion(MapHitTestableRegion region, MapUnit scaledHitUnit, Point screenPoint) {
			HashSet<IHitTestableElement> result = new HashSet<IHitTestableElement>();
			for (int i = 0; i < region.MapItems.Count; i++) {
				IHitTestableElement hitTestable = region.MapItems[i];
				if(hitTestable.IsHitTestVisible)
					result.UnionWith(hitTestable.HitTest(scaledHitUnit, screenPoint));
			}
			foreach (IHitTestableElement hitTestable in locatableElements) {
				lock (hitTestable.Locker) {
					if(hitTestable.IsHitTestVisible)
						result.UnionWith(hitTestable.HitTest(scaledHitUnit, screenPoint));
				}
			}
			IHitTestableElement[] resultElements = new IHitTestableElement[result.Count];
			result.CopyTo(resultElements);
			return resultElements;
		}
		void AddHitTestableElement(List<IHitTestableElement> result, IHitTestableElement element) {
			if (element != null) result.Add(element);
		}
		public IHitTestableElement[] HitTest(Point screenPoint) {
			if (Layers.Count <= 0)
				return new IHitTestableElement[0];
			MapUnit unscaledHitUnit = Map.ScreenPointToMapUnit(new MapPoint(screenPoint.X, screenPoint.Y));
			Point regionPoint = CalculateRegion(unscaledHitUnit);
			MapHitTestableRegion region = hitTestableRegions[regionPoint.X, regionPoint.Y];
			RecreateRegionsIfNecessary();
			MapUnit scaledUnit = unscaledHitUnit * MapItem.RenderScaleFactor;
			IHitTestableElement[] result;
			try { 
				result = HitTestByRegion(region, scaledUnit, screenPoint);
			}
			catch{
				result = new IHitTestableElement[0];
			}
			return result;
		}
		void UpdateItemInRegions(IHitTestableElement hitTestable) {
			if (!shouldRecreateRegions) {
				IHitTestableElement[] elements = ExtractHitTestableElements(hitTestable);
				foreach (IHitTestableElement element in elements) {
					RegionRange newRange = CreateRegionRange(element.UnitBounds);
					if (newRange != element.RegionRange) {
						shouldRecreateRegions = true;
						break;
					}
				}
			}
		}
		void ClearRegions() {
			locatableElements.Clear();
			foreach (MapHitTestableRegion region in hitTestableRegions)
				region.MapItems.Clear();
		}
		void RecreateRegions() {
			shouldRecreateRegions = false;
			ClearRegions();
			lock(Layers) {
				foreach(LayerBase layer in Layers) {
					MapItemsLayerBase itemsLayer = layer as MapItemsLayerBase;
					if(itemsLayer != null)
						foreach(IHitTestableElement item in itemsLayer.DataItems) {
							AddMapItemToRegions(item);
						}
				}
			}
		}
		IEnumerable<MapItem> GetItemsFromElements(IList<IHitTestableElement> elements) {
			return elements.Select(element => GetOwnerMapItem(element)).Where(item => item != null);
		}
		protected internal void RecreateRegionsIfNecessary() {
			if(shouldRecreateRegions)
				RecreateRegions();
		}
		internal RegionRange CreateRegionRange(MapRect bounds) {
			return new RegionRange(GetHRegionIndex(bounds.Left), GetHRegionIndex(bounds.Top), GetHRegionIndex(bounds.Right), GetHRegionIndex(bounds.Bottom));
		}
		internal IEnumerable<MapItem> GetItemsInRegion(MapHitTestableRegion region) {
			return GetItemsFromElements(region.MapItems);
		}
		internal IEnumerable<MapItem> GetItemsInCentralRegions(RegionRange regionRange) {
			List<MapItem> result = new List<MapItem>();
			for(int i = regionRange.Left + 1; i < regionRange.Right; i++)
				for(int j = regionRange.Top + 1; j < regionRange.Bottom; j++)
					result.AddRange(GetItemsInRegion(hitTestableRegions[i, j]));
			return result;
		}
		internal IEnumerable<MapItem> GetItemsHorizontalBorderRegions(RegionRange regionRange, int horizontalRegion) {
			List<MapItem> result = new List<MapItem>();
			for(int i = regionRange.Left; i <= regionRange.Right; i++)
				result.AddRange(GetItemsInRegion(hitTestableRegions[i, horizontalRegion]));
			return result;
		}
		internal IEnumerable<MapItem> GetItemsVerticalBorderRegions(RegionRange regionRange, int verticalRegion) {
			List<MapItem> result = new List<MapItem>();
			for(int i = regionRange.Top + 1; i < regionRange.Bottom; i++)
				result.AddRange(GetItemsInRegion(hitTestableRegions[verticalRegion, i]));
			return result;
		}
		internal IEnumerable<MapItem> GetItemsInBorderRegions(RegionRange regionRange) {
			HashSet<MapItem> result = new HashSet<MapItem>();
			result.UnionWith(GetItemsHorizontalBorderRegions(regionRange, regionRange.Top));
			if(regionRange.Top != regionRange.Bottom)
				result.UnionWith(GetItemsHorizontalBorderRegions(regionRange, regionRange.Bottom));
			result.UnionWith(GetItemsVerticalBorderRegions(regionRange, regionRange.Left));
			if(regionRange.Left != regionRange.Right)
				result.UnionWith(GetItemsVerticalBorderRegions(regionRange, regionRange.Right));
			return result;
		}
		internal IEnumerable<MapItem> GetLocatableItems(Rectangle regionInPixels) {
			List<MapItem> result = new List<MapItem>();
			foreach(IHitTestableElement hitTestableItem in locatableElements) {
				Rectangle itemBounds = CalculateItemBounds(hitTestableItem);
				if(RectUtils.IntersectsExcludeBounds(regionInPixels, itemBounds))
					result.Add(GetOwnerMapItem(hitTestableItem));
			}
			return result;
		}
		Rectangle CalculateItemBounds(IHitTestableElement item) {
			LayerBase layer = GetLayer(item);
			ILocatableRenderItem locatableItem = item as ILocatableRenderItem;
			if(layer == null || locatableItem == null)
				return Rectangle.Empty;
			MapPoint screenLocation = UnitConverter.MapUnitToScreenPoint(locatableItem.Location * (1 / MapShape.RenderScaleFactor), false);
			Size itemSize = locatableItem.SizeInPixels;
			return new Rectangle(new Point((int)screenLocation.X - itemSize.Width / 2, (int)screenLocation.Y - itemSize.Height / 2), itemSize);
		}
		MapItem GetOwnerMapItem(IHitTestableElement element) {
			IHitTestableOwner owner = element.Owner;
			return owner != null ? owner as MapItem : element as MapItem;
		}
		LayerBase GetLayer(IHitTestableElement hitTestableItem) {
			MapItem item = GetOwnerMapItem(hitTestableItem);
			return item != null ? item.Layer : null;
		}
		protected void SetElementKey(IHitTestableElement item, long key) {
			item.Key = new HitTestKey(item.GeometryType, key);
		}
		protected void ClearHitGemetryPool(MapItemsLayerBase layer) {
			HitTestGeometryPool.ClearItems(layer); 
		}
		protected void RegisterHitGeometry(IHitTestableElement element) {
			HitTestGeometryPool.RegisterItem(element);
		}
		protected void UnregisterHitGeometry(IHitTestableElement element) {
			HitTestGeometryPool.UnregisterItem(element);
		}
		protected override void DisposeOverride() {
			ClearRegions();
		}
	}
}
