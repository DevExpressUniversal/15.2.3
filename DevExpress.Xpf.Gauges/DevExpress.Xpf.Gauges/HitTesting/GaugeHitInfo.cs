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
using System.Windows.Media;
using System.Collections.Generic;
using DevExpress.Xpf.Gauges.Native;
namespace DevExpress.Xpf.Gauges {
	public abstract class GaugeHitInfoBase {
		readonly List<IHitTestableElement> hitTestableElements = new List<IHitTestableElement>();
		readonly AnalogGaugeControl gauge;
		internal GaugeHitInfoBase(AnalogGaugeControl gauge, Point point) {
			this.gauge = gauge; 
			hitTestableElements = new HitTestController(gauge).FindElements(point);
		}
		protected Object GetElementByType(Type elementType) {
			foreach (IHitTestableElement hitTestableElement in hitTestableElements) {
				if (hitTestableElement.IsHitTestVisible && hitTestableElement.Element != null && elementType.IsAssignableFrom(hitTestableElement.Element.GetType()))
					return hitTestableElement.Element;
			}
			return null;
		}
		protected Object GetElementParentByType(Type elementType) {
			Object parent = null;
			foreach (IHitTestableElement hitTestableElement in hitTestableElements) {
				if (hitTestableElement.IsHitTestVisible && hitTestableElement.Parent != null && elementType.IsAssignableFrom(hitTestableElement.Parent.GetType())) {
					parent = hitTestableElement.Parent;
					break;
				}
			}
			return parent ?? GetElementByType(elementType);
		}
	}
	public class CircularGaugeHitInfo : GaugeHitInfoBase {
		public bool InScale { get { return Scale != null; } }
		public bool InNeedle { get { return Needle != null; } }
		public bool InMarker { get { return Marker != null; } }
		public bool InRangeBar { get { return RangeBar != null; } }
		public bool InRange { get { return Range != null; } }
		public ArcScale Scale { get { return GetElementParentByType(typeof(ArcScale)) as ArcScale; } }
		public ArcScaleNeedle Needle { get { return GetElementByType(typeof(ArcScaleNeedle)) as ArcScaleNeedle; } }
		public ArcScaleMarker Marker { get { return GetElementByType(typeof(ArcScaleMarker)) as ArcScaleMarker; } }
		public ArcScaleRangeBar RangeBar { get { return GetElementByType(typeof(ArcScaleRangeBar)) as ArcScaleRangeBar; } }
		public ArcScaleRange Range { get { return GetElementByType(typeof(ArcScaleRange)) as ArcScaleRange; } }
		internal CircularGaugeHitInfo(CircularGaugeControl gauge, Point point) : base(gauge, point) {			
		}				
	}
	public class LinearGaugeHitInfo : GaugeHitInfoBase {
		public bool InScale { get { return Scale != null; } }
		public bool InMarker { get { return Marker != null; } }
		public bool InRangeBar { get { return RangeBar != null; } }
		public bool InLevelBar { get { return LevelBar != null; } }
		public bool InRange { get { return Range != null; } }
		public LinearScale Scale { get { return GetElementParentByType(typeof(LinearScale)) as LinearScale; } }
		public LinearScaleMarker Marker { get { return GetElementByType(typeof(LinearScaleMarker)) as LinearScaleMarker; } }
		public LinearScaleRangeBar RangeBar { get { return GetElementByType(typeof(LinearScaleRangeBar)) as LinearScaleRangeBar; } }
		public LinearScaleLevelBar LevelBar { get { return GetElementByType(typeof(LinearScaleLevelBar)) as LinearScaleLevelBar; } }
		public LinearScaleRange Range { get { return GetElementByType(typeof(LinearScaleRange)) as LinearScaleRange; } }
		internal LinearGaugeHitInfo(LinearGaugeControl gauge, Point point) : base(gauge, point) {
		}
	}
}
namespace DevExpress.Xpf.Gauges.Native { 
	public class HitTestController {
		readonly AnalogGaugeControl gauge;
		List<IHitTestableElement> currentElements = null;
		public HitTestController(AnalogGaugeControl gauge) {
			this.gauge = gauge;
		}
		void AddUniqueHitTestableElement(IHitTestableElement hitTestableElement) {
			if (hitTestableElement != null && !currentElements.Contains(hitTestableElement))
				currentElements.Add(hitTestableElement);
		}
		IHitTestableElement GetParentHitTestableElement(DependencyObject obj) {
			DependencyObject parent = obj;
			while (parent != null && !(parent is CircularGaugeControl)) {
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
			VisualTreeHelper.HitTest(gauge, null, OnHitTestResult, new PointHitTestParameters(point));
		}
		public List<IHitTestableElement> FindElements(Point point) {
			currentElements = new List<IHitTestableElement>();
			PrepareHitTestableElements(point);
			return currentElements;
		}
	}
}
