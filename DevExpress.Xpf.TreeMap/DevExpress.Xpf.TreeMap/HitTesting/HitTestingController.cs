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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.TreeMap.Native;
namespace DevExpress.Xpf.TreeMap.Native {
	public class HitTestController {
		TreeMapControl treeMap;
		List<IHitTestableElement> currentElements = null;
		public HitTestController(TreeMapControl treeMap) {
			this.treeMap = treeMap;
		}
		void AddUniqueHitTestableElement(IHitTestableElement hitTestableElement) {
			if (hitTestableElement != null && !currentElements.Contains(hitTestableElement))
				currentElements.Add(hitTestableElement);
		}
		IHitTestableElement GetParentHitTestableElement(DependencyObject obj) {
			DependencyObject parent = obj;
			while (parent != null && !(parent is TreeMapControl)) {
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
			VisualTreeHelper.HitTest(treeMap, null, OnHitTestResult, new PointHitTestParameters(point));
		}
		void PrepareHitTestableElements(Rect region) {
			Geometry geometry = new RectangleGeometry(region);
			VisualTreeHelper.HitTest(treeMap, null, OnHitTestResult, new GeometryHitTestParameters(geometry));
		}
		public List<IHitTestableElement> FindElements(Point point) {
			currentElements = new List<IHitTestableElement>();
			PrepareHitTestableElements(point);
			return currentElements;
		}	   
	}
}
