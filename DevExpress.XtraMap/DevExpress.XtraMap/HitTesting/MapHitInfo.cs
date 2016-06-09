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

using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
namespace DevExpress.XtraMap {
	public enum MapHitUiElementType { None, NavigationPanel, ZoomTrackBar, ZoomTrackBarThumb, ZoomIn, ZoomOut, ScrollButtons, SearchPanel, MiniMap, Overlay }
	public interface IMapUiHitInfo {
		Point HitPoint { get; }
		MapHitUiElementType HitElement { get; }
	}
	public class MapUiHitInfo : IMapUiHitInfo {
		readonly Point hitPoint;
		readonly MapHitUiElementType hitElement;
		public Point HitPoint { get { return hitPoint; } }
		public MapHitUiElementType HitElement { get { return hitElement; } }
		public MapUiHitInfo(Point hitPoint, MapHitUiElementType hitElement) {
			this.hitPoint = hitPoint;
			this.hitElement = hitElement;
		}
	}
	public class MapOverlayHitInfo : MapUiHitInfo {
		readonly MapOverlay overlay;
		readonly MapOverlayItemBase overlayItem;
		public MapOverlay Overlay { get { return overlay; } }
		public MapOverlayItemBase OverlayItem { get { return overlayItem; } }
		public MapOverlayHitInfo(Point hitPoint, MapHitUiElementType hitElement, MapOverlay overlay, MapOverlayItemBase overlayItem)
			: base(hitPoint, hitElement) {
			this.overlay = overlay;
			this.overlayItem = overlayItem;
		}
	}
	public class MapHitInfo {
		MapRectangle mapRectangle;
		MapPolygon mapPolygon;
		MapPushpin mapPushpin;
		MapEllipse mapEllipse;
		MapCustomElement mapCustomElement;
		MapCallout mapCallout;
		MapPath mapPath;
		MapDot mapDot;
		MapLine mapLine;
		MapPolyline mapPolyline;
		MapBubble mapBubble;
		MapPie mapPie;
		PieSegment pieSegment;
		readonly IHitTestableElement[] hitObjects;
		readonly Point hitPoint;
		readonly IMapUiHitInfo uiHitInfo;
		public Point HitPoint { get { return hitPoint; } }
		public bool InMapDot { get { return MapDot != null; } }
		public bool InMapCustomElement { get { return MapCustomElement != null; } }
		public bool InMapCallout { get { return MapCallout != null; } }
		public bool InMapLine { get { return MapLine != null; } }
		public bool InMapPath { get { return MapPath != null; } }
		public bool InMapPolygon { get { return MapPolygon != null; } }
		public bool InMapPolyline { get { return MapPolyline != null; } }
		public bool InMapPushpin { get { return MapPushpin != null; } }
		public bool InMapRectangle { get { return MapRectangle != null; } }
		public bool InMapEllipse { get { return MapEllipse != null; } }
		public bool InMapBubble { get { return MapBubble != null; } }
		public bool InMapPie { get { return MapPie != null; } }
		public bool InPieSegment { get { return PieSegment != null; } }
		public bool InUIElement { get { return UiHitInfo != null && UiHitInfo.HitElement != MapHitUiElementType.None; } }
		public IMapUiHitInfo UiHitInfo { get { return uiHitInfo; } }
		public object[] HitObjects { get { return hitObjects; } }
		public MapDot MapDot {
			get {
				if(mapDot == null)
					mapDot = FindObjectByType(typeof(MapDot)) as MapDot;
				return mapDot;
			}
		}
		public MapCustomElement MapCustomElement {
			get {
				if(mapCustomElement == null)
					mapCustomElement = FindObjectByType(typeof(MapCustomElement)) as MapCustomElement;
				return mapCustomElement;
			}
		}
		public MapCallout MapCallout {
			get {
				if(mapCallout == null)
					mapCallout = FindObjectByType(typeof(MapCallout)) as MapCallout;
				return mapCallout;
			}
		}
		public MapLine MapLine {
			get {
				if(mapLine == null)
					mapLine = FindObjectByType(typeof(MapLine)) as MapLine;
				return mapLine;
			}
		}
		public MapPath MapPath {
			get {
				if(mapPath == null)
					mapPath = FindObjectByType(typeof(MapPath)) as MapPath;
				return mapPath;
			}
		}
		public MapPolygon MapPolygon {
			get {
				if(mapPolygon == null)
					mapPolygon = FindObjectByType(typeof(MapPolygon)) as MapPolygon;
				return mapPolygon;
			}
		}
		public MapPolyline MapPolyline {
			get {
				if(mapPolyline == null)
					mapPolyline = FindObjectByType(typeof(MapPolyline)) as MapPolyline;
				return mapPolyline;
			}
		}
		public MapPushpin MapPushpin {
			get {
				if(mapPushpin == null)
					mapPushpin = FindObjectByType(typeof(MapPushpin)) as MapPushpin;
				return mapPushpin;
			}
		}
		public MapEllipse MapEllipse {
			get {
				if(mapEllipse == null)
					mapEllipse = FindObjectByType(typeof(MapEllipse)) as MapEllipse;
				return mapEllipse;
			}
		}
		public MapRectangle MapRectangle {
			get {
				if(mapRectangle == null)
					mapRectangle = FindObjectByType(typeof(MapRectangle)) as MapRectangle;
				return mapRectangle;
			}
		}
		public MapBubble MapBubble {
			get {
				if(mapBubble == null)
					mapBubble = FindObjectByType(typeof(MapBubble)) as MapBubble;
				return mapBubble;
			}
		}
		public MapPie MapPie {
			get {
				if(PieSegment != null) {
					return PieSegment.MapPie;
				}
				if(mapPie == null)
					mapPie = FindObjectByType(typeof(MapPie)) as MapPie;
				return mapPie;
			}
		}
		public PieSegment PieSegment {
			get {
				if(pieSegment == null) {
					pieSegment = FindObjectByType(typeof(PieSegment)) as PieSegment;
				}
				return pieSegment;
			}
		}
		internal MapHitInfo(Point hitPoint, IHitTestableElement[] hitObjects, IMapUiHitInfo uiHitInfo) {
			this.hitPoint = hitPoint;
			this.hitObjects = hitObjects;
			this.uiHitInfo = uiHitInfo;
		}
		internal MapItem GetHitMapItem() {
			int count = hitObjects.Length;
			if(count == 1)
				return hitObjects[0] as MapItem;
			for(int i = count - 1; i >= 0; i--) {
				MapItem item = hitObjects[i] as MapItem;
				if(item != null) {
					return item;
				}
			}
			return null;
		}
		object FindObjectByType(Type type) {
			object founded = null;
			IInteractiveElement selected = null;
			foreach(IHitTestableElement hitTestable in hitObjects) {
				if(hitTestable.GetType().IsAssignableFrom(type)) {
					founded = hitTestable;
					IInteractiveElement interactive = hitTestable as IInteractiveElement;
					if(interactive != null) {
						if(interactive.IsHighlighted)
							return interactive;
						if(interactive.IsSelected) {
							selected = interactive;
						}
					}
				}
			}
			return selected != null ? selected : founded;
		}
	}
}
namespace DevExpress.XtraMap.Native {
	public class MapHitTestableRegion {
		readonly List<IHitTestableElement> mapItems;
		public List<IHitTestableElement> MapItems { get { return mapItems; } }
		public MapHitTestableRegion() {
			this.mapItems = new List<IHitTestableElement>();
		}
	}
	public struct RegionRange {
		static RegionRange empty = new RegionRange(-1, -1, -1, -1);
		public static RegionRange Empty { get { return empty; } }
		public static bool operator ==(RegionRange range1, RegionRange range2) {
			return range1.left == range2.left && range1.top == range2.top && range1.right == range2.right && range1.bottom == range2.bottom;
		}
		public static bool operator !=(RegionRange range1, RegionRange range2) {
			return !(range1 == range2);
		}
		readonly int left;
		readonly int right;
		readonly int top;
		readonly int bottom;
		public int Left { get { return left; } }
		public int Right { get { return right; } }
		public int Top { get { return top; } }
		public int Bottom { get { return bottom; } }
		public RegionRange(int left, int top, int right, int bottom) {
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
		}
		public override bool Equals(object obj) {
			if(obj != null && obj is RegionRange)
				return (RegionRange)obj == this;
			return false;
		}
		public override int GetHashCode() {
			return left.GetHashCode() ^ top.GetHashCode() ^ right.GetHashCode() ^ bottom.GetHashCode();
		}
	}
}
