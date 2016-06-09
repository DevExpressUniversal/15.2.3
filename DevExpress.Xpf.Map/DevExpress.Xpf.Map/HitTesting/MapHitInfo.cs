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
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Collections.Generic;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Map.Native;
using System.Linq;
namespace DevExpress.Xpf.Map {
	public class MapHitInfo {
		readonly List<IHitTestableElement> hitTestableElements;
		readonly MapControl map;
		public bool InCustomElement { get { return CustomElement != null; } }
		public bool InMapLine { get { return MapLine != null; } }
		public bool InMapRectangle { get { return MapRectangle != null; } }
		public bool InMapEllipse { get { return MapEllipse != null; } }
		public bool InMapPolyline { get { return MapPolyline != null; } }
		public bool InMapPolygon { get { return MapPolygon != null; } }
		public bool InMapPath { get { return MapPath != null; } }
		public bool InMapDot { get { return MapDot != null; } }
		public bool InMapPushpin { get { return MapPushpin != null; } }
		public bool InMapBubble { get { return MapBubble != null; } }
		public bool InMapPie { get { return MapPie != null; } }
		public MapCustomElement CustomElement { get { return GetElementByType(typeof(MapCustomElement)) as MapCustomElement; } }
		public MapLine MapLine { get { return GetElementByType(typeof(MapLine)) as MapLine; } }
		public MapRectangle MapRectangle { get { return GetElementByType(typeof(MapRectangle)) as MapRectangle; } }
		public MapEllipse MapEllipse { get { return GetElementByType(typeof(MapEllipse)) as MapEllipse; } }
		public MapPolyline MapPolyline { get { return GetElementByType(typeof(MapPolyline)) as MapPolyline; } }
		public MapPolygon MapPolygon { get { return GetElementByType(typeof(MapPolygon)) as MapPolygon; } }
		public MapPath MapPath { get { return GetElementByType(typeof(MapPath)) as MapPath; } }
		public MapDot MapDot { get { return GetElementByType(typeof(MapDot)) as MapDot; } }
		public MapPushpin MapPushpin { get { return GetElementByType(typeof(MapPushpin)) as MapPushpin; } }
		public MapBubble MapBubble { get { return GetElementByType(typeof(MapBubble)) as MapBubble; } }
		public MapPie MapPie { get { return GetElementByType(typeof(MapPie)) as MapPie; } }
		public object[] HitObjects { get { return (from obj in hitTestableElements where obj.Element != null && obj.IsHitTestVisible && !(obj.Element is ShapeTitle) select obj.Element).ToArray(); } }
		internal MapHitInfo(MapControl map, Point point) {
			this.map = map;
			hitTestableElements = map.Visibility == Visibility.Visible ? new HitTestController(map).FindElements(point) : new List<IHitTestableElement>();
		}
		protected Object GetElementByType(Type elementType) {
			foreach (IHitTestableElement hitTestableElement in hitTestableElements) {
				if (hitTestableElement.IsHitTestVisible && hitTestableElement.Element != null && elementType.IsAssignableFrom(hitTestableElement.Element.GetType()))
					return hitTestableElement.Element;
			}
			return null;
		}
	}	
}
namespace DevExpress.Xpf.Map.Native {
	public class HitTestController {
		readonly MapControl map;
		List<IHitTestableElement> currentElements = null;
		public HitTestController(MapControl map) {
			this.map = map;
		}
		void AddUniqueHitTestableElement(IHitTestableElement hitTestableElement) {
			if (hitTestableElement != null && !currentElements.Contains(hitTestableElement))
				currentElements.Add(hitTestableElement);
		}
		IHitTestableElement GetParentHitTestableElement(DependencyObject obj) {
			DependencyObject parent = obj;
			while (parent != null && !(parent is MapControl)) {
				IHitTestableElement hitTestableElement = parent as IHitTestableElement;
				if (hitTestableElement != null)
					return hitTestableElement;
				parent = VisualTreeHelper.GetParent(parent);
			}
			return null;
		}
		HitTestResultBehavior OnHitTestResult(HitTestResult result) {
			AddUniqueHitTestableElement(GetParentHitTestableElement(result.VisualHit));
			return HitTestResultBehavior.Continue;
		}
		void PrepareHitTestableElements(Point point) {
			VisualTreeHelper.HitTest(map, null, OnHitTestResult, new PointHitTestParameters(point));
		}
		void PrepareHitTestableElements(Rect region) {
			Geometry geometry = new RectangleGeometry(region);
			VisualTreeHelper.HitTest(map, null, OnHitTestResult, new GeometryHitTestParameters(geometry));
		}
		public List<IHitTestableElement> FindElements(Point point) {
			currentElements = new List<IHitTestableElement>();
			PrepareHitTestableElements(point);
			return currentElements;
		}
		public List<IHitTestableElement> FindElemensByRegion(Rect region) {
			currentElements = new List<IHitTestableElement>();
			PrepareHitTestableElements(region);
			return currentElements;
		}
	}
}
