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
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using DevExpress.Xpf.Core.Native;
#if !SL
namespace DevExpress.Xpf.Core.HitTest {
#else
namespace DevExpress.Xpf.Core {
#endif
	public enum HitTestFilterBehavior {
		ContinueSkipSelfAndChildren = 0, ContinueSkipChildren = 2,
		ContinueSkipSelf = 4, Continue = 6, Stop = 8
	}
	public enum HitTestResultBehavior { Stop = 0, Continue = 1 }
	public delegate HitTestFilterBehavior HitTestFilterCallback(DependencyObject potentialHitTestTarget);
	public delegate HitTestResultBehavior HitTestResultCallback(HitTestResult result);
	public class HitTestResult {
		public DependencyObject VisualHit { get; private set; }
		public HitTestResult(DependencyObject visualHit) {
			VisualHit = visualHit;
		}
	}
	public class PointHitTestResult : HitTestResult {
		public Point PointHit { get; private set; }
		public PointHitTestResult(DependencyObject visualHit, Point pointHit)
			: base(visualHit) {
			PointHit = pointHit;
		}
	}
	public abstract class HitTestParameters {
#if SL
		public virtual bool UseNativeHitTest {
			get { return true; }
			protected set { throw new NotImplementedException(); }
		}
#endif
	}
	public class PointHitTestParameters : HitTestParameters {
		public Point HitPoint { get; protected set; }
#if SL
		public override bool UseNativeHitTest { get; protected set; }
		public PointHitTestParameters(Point point, bool useNativeBehavior) {
			HitPoint = point;
			UseNativeHitTest = useNativeBehavior;
		}
#endif
		public PointHitTestParameters(Point point) {
			HitPoint = point;
#if SL
			UseNativeHitTest = true;
#endif
		}
	}
	public static class HitTestHelper {
		static UIElement GetRootElement(UIElement element) {
#if !SL
			return LayoutHelper.GetTopLevelVisual(element);
#else
			return Application.Current.RootVisual;
#endif
		}
		public static Point TransformPointToRoot(UIElement element, Point pt) {
			UIElement rootElement = GetRootElement(element);
			Point point = element.TransformToVisual(rootElement).Transform(pt);
			if(((FrameworkElement)rootElement).FlowDirection == FlowDirection.RightToLeft)
#if !SL
				if(rootElement is ContentControl && ((ContentControl)rootElement).Content != null && ((ContentControl)rootElement).Content is FrameworkElement)
					point.X = ((FrameworkElement)((ContentControl)rootElement).Content).ActualWidth - point.X;
#else
				point.X = AppHelper.HostWidth - point.X;
#endif
			return point;
		}
		public static HitTestResult HitTest(UIElement reference, Point point) {
			List<UIElement> elements = GetElements(reference, new PointHitTestParameters(point));
			if(elements.Count == 0)
				return null;
			return new HitTestResult(elements[0]);
		}
		public static HitTestResult HitTest(UIElement reference, Point point, bool skipDisabledElements) {
			if(skipDisabledElements)
				return HitTest(reference, point);
			List<UIElement> elements = GetElements(reference, point, false);
			if(elements.Count == 0)
				return null;
			return new HitTestResult(elements[elements.Count - 1]);
		}
		public static bool IsHitTest(UIElement reference, UIElement topElement, Point point) {
			UIElement rootElement = GetRootElement(topElement);
			if(reference is FrameworkElement && ((FrameworkElement)reference).FlowDirection == FlowDirection.RightToLeft && rootElement is FrameworkElement) {
				point.X = rootElement.RenderSize.Width - point.X;
			}
			List<UIElement> elements = GetElements(topElement, new PointHitTestParameters(point));
			if(elements.Count == 0)
				return false;
			return elements.Contains(reference);
		}
		public static void HitTest(UIElement reference, HitTestFilterCallback filterCallback,
			HitTestResultCallback resultCallback, PointHitTestParameters hitTestParameters) {
				HitTest(reference, filterCallback, resultCallback, hitTestParameters, false, true);
		}
		public static void HitTest(UIElement reference, HitTestFilterCallback filterCallback,
			HitTestResultCallback resultCallback, PointHitTestParameters hitTestParameters, bool isReverce, bool useFindElementsInHostCoordinates = true) {
			if (resultCallback == null && filterCallback == null)
				return;
			List<UIElement> elements = useFindElementsInHostCoordinates ? GetElements(reference, hitTestParameters) : GetElements(reference, hitTestParameters.HitPoint, false);
			if(!isReverce)
				elements.Reverse();
			HitTestFilterBehavior filterBehaviorRes = HitTestFilterBehavior.Continue;
			UIElement parentForSkipChildren = null;
			for (int i = 0; i < elements.Count; i++) {
				filterBehaviorRes = RaiseFilterCallback(filterCallback, elements[i], parentForSkipChildren == null);
				if (filterBehaviorRes == HitTestFilterBehavior.Stop)
					return;
				if (filterBehaviorRes == HitTestFilterBehavior.ContinueSkipSelf)
					continue;
				if (filterBehaviorRes == HitTestFilterBehavior.ContinueSkipChildren)
					parentForSkipChildren = elements[i];
				else if (filterBehaviorRes == HitTestFilterBehavior.ContinueSkipSelfAndChildren) {
					parentForSkipChildren = elements[i];
					continue;
				}
				if (VisualTreeHelper.GetParent(elements[i]) == parentForSkipChildren)
					continue;
				else
					parentForSkipChildren = null;
				if (RaiseResultCallback(resultCallback, elements[i]) == HitTestResultBehavior.Stop)
					return;
			}
		}
		static bool IsPointInElementBounds(UIElement root, UIElement element, Point point) {
			if(element.Visibility == Visibility.Collapsed || !element.IsHitTestVisible)
				return false;
			Rect r = element.TransformToVisual(root).TransformBounds(new Rect(0, 0, element.RenderSize.Width, element.RenderSize.Height));
			return point.X >= r.X && point.X <= r.X + r.Width && point.Y >= r.Y && point.Y <= r.Y + r.Height;
		}
		static bool CanSkipElement(UIElement topElement) {
			if(topElement is Panel && ((Panel)topElement).Background == null)
				return true;
			if(topElement is Control && ((Control)topElement).Background == null)
				return true;
			if(topElement is ContentPresenter)
				return true;
			if(topElement is ItemsPresenter)
				return true;
			return false;			
		}
		static List<UIElement> GetElements(UIElement topElement, Point point, bool skipDisabledElements) {
			if(skipDisabledElements)
				return GetElements(topElement, new PointHitTestParameters(point));
			List<UIElement> res = new List<UIElement>();
			UIElement root = GetRootElement(topElement);
			if(root == null)
				return res;
			if(!IsPointInElementBounds(root, topElement, point))
				return res;
			if(!CanSkipElement(topElement))
				res.Add(topElement);	
			int count = VisualTreeHelper.GetChildrenCount(topElement);
			for(int i = 0; i < count; i++) {
				UIElement child = VisualTreeHelper.GetChild(topElement, i) as UIElement;
				if(child == null)
					continue;
				if(IsPointInElementBounds(root, child, point)) {
					res.AddRange(GetElements(child, point, false));
				}
			}
			return res;
		}
		static List<UIElement> GetElements(UIElement reference, PointHitTestParameters pointHitTestParameters) {
			return new List<UIElement>(FindElementsInHostCoordinates(reference, pointHitTestParameters));
		}
		private static IEnumerable<UIElement> FindElementsInHostCoordinates(UIElement reference, PointHitTestParameters pointHitTestParameters) {
			Point point = pointHitTestParameters.HitPoint;
#if SL
			if(pointHitTestParameters.UseNativeHitTest)
				return VisualTreeHelper.FindElementsInHostCoordinates(point, reference);
#endif
			List<UIElement> result = new List<UIElement>();
			FindElementsInHostCoordinatesCore(reference, reference, point, result);
			return result;
		}
		private static void FindElementsInHostCoordinatesCore(UIElement root, UIElement reference, Point point, List<UIElement> result) {
			int count = VisualTreeHelper.GetChildrenCount(reference);
			for(int i = count - 1; i >= 0; i--) {
				UIElement child = VisualTreeHelper.GetChild(reference, i) as UIElement;
				if(child == null || child.Visibility == Visibility.Collapsed)
					continue;
				Rect rect = DevExpress.Xpf.Core.Native.LayoutHelper.GetRelativeElementRect(child, root);
				if(rect.Contains(point)) {
					result.Add(child);
					FindElementsInHostCoordinatesCore(root, child, point, result);
				}
			}
		}
		static HitTestFilterBehavior RaiseFilterCallback(HitTestFilterCallback filterCallback,
			DependencyObject patentialHitTestTarget, bool raise) {
			if (filterCallback == null || !raise)
				return HitTestFilterBehavior.Continue;
			return filterCallback(patentialHitTestTarget);
		}
		static HitTestResultBehavior RaiseResultCallback(HitTestResultCallback resultCallback,
			DependencyObject visualHit) {
			if (resultCallback == null)
				return HitTestResultBehavior.Continue;
			return resultCallback(new HitTestResult(visualHit));
		}
	}
}
